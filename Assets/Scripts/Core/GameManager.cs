using UnityEngine;
using UnityEngine.Events;
using Fusion;
using System.Collections.Generic;
using System.Linq;

namespace KanjiFlipGame.Core
{
    /// <summary>
    /// ゲーム全体の状態を管理するマネージャー
    /// 2〜8人の多人数プレイ、ラウンド制、ポイント制を管理します
    /// </summary>
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Networked, OnChangedRender(nameof(OnTopicChangedInternal))]
        public string CurrentTopic { get; set; } = "";

        [Networked, OnChangedRender(nameof(OnAnswerChangedInternal))]
        public string LastAnswer { get; set; } = "";

        [Networked, OnChangedRender(nameof(OnStateChangedInternal))]
        public GameState CurrentState { get; set; } = GameState.Waiting;

        [Networked]
        public int CurrentRound { get; set; } = 0;

        [Networked]
        public PlayerRef CurrentAnswerer { get; set; } = PlayerRef.None;

        [Networked, Capacity(8)]
        public NetworkDictionary<PlayerRef, int> PlayerScores => default;

        [Networked, Capacity(8)]
        public NetworkDictionary<PlayerRef, NetworkBool> PlayerReadyStates => default;

        [Networked, Capacity(8)]
        public NetworkDictionary<PlayerRef, NetworkBool> PlayerConsentStates => default;

        // 出題キュー（Shared ModeなのでRPC経由で管理）
        private List<SubmittedFlip> _submissionQueue = new List<SubmittedFlip>();
        
        public struct SubmittedFlip
        {
            public PlayerRef Author;
            public string FlipDataJson;
        }

        // ローカルプレイヤーの役割
        private PlayerRole _localPlayerRole = PlayerRole.None;

        // イベント
        public UnityEvent<GameState> OnGameStateChanged = new UnityEvent<GameState>();
        public UnityEvent<PlayerRole> OnPlayerRoleChanged = new UnityEvent<PlayerRole>();
        public UnityEvent<string> OnTopicChanged = new UnityEvent<string>();
        public UnityEvent<bool> OnAnswerResult = new UnityEvent<bool>();
        public UnityEvent OnScoreUpdated = new UnityEvent();
        public UnityEvent<string> OnFlipDisplayed = new UnityEvent<string>();
        public UnityEvent OnReadyStatesUpdated = new UnityEvent();

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        public override void Spawned()
        {
            if (Instance == null) Instance = this;
            Debug.Log("GameManagerがネットワーク上に生成されました");
            
            // ロビー状態から開始
            if (Object.HasStateAuthority && CurrentState == GameState.Waiting)
            {
                SetGameState(GameState.Lobby);
            }
        }

        #region マッチング・準備完了管理

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_SetReady(PlayerRef player, NetworkBool isReady)
        {
            PlayerReadyStates.Set(player, isReady);
            CheckStartConditions();
            OnReadyStatesUpdated?.Invoke();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_SetConsent(PlayerRef player, NetworkBool isConsented)
        {
            PlayerConsentStates.Set(player, isConsented);
            CheckStartConditions();
            OnReadyStatesUpdated?.Invoke();
        }

        private void CheckStartConditions()
        {
            if (!Object.HasStateAuthority || CurrentState != GameState.Lobby) return;

            var players = Runner.ActivePlayers.ToList();
            int playerCount = players.Count;
            if (playerCount == 0) return;

            bool allReady = players.All(p => PlayerReadyStates.TryGet(p, out var ready) && ready);
            
            if (allReady)
            {
                if (playerCount >= 4)
                {
                    // 4人以上なら即開始
                    Host_StartGame();
                }
                else
                {
                    // 4人未満なら全員の同意が必要
                    bool allConsented = players.All(p => PlayerConsentStates.TryGet(p, out var consented) && consented);
                    if (allConsented)
                    {
                        Host_StartGame();
                    }
                }
            }
        }

        public bool IsPlayerReady(PlayerRef player)
        {
            return PlayerReadyStates.TryGet(player, out var ready) && ready;
        }

        public bool IsPlayerConsented(PlayerRef player)
        {
            return PlayerConsentStates.TryGet(player, out var consented) && consented;
        }

        #endregion

        #region ゲーム進行制御

        /// <summary>
        /// ホストがゲームを開始する
        /// </summary>
        public void Host_StartGame()
        {
            if (Object.HasStateAuthority)
            {
                CurrentRound = 0;
                // 全プレイヤーのスコアをリセット
                foreach (var player in Runner.ActivePlayers)
                {
                    PlayerScores.Set(player, 0);
                }
                StartNextRound();
            }
        }

        /// <summary>
        /// 次のラウンドを開始
        /// </summary>
        private void StartNextRound()
        {
            CurrentRound++;
            if (CurrentRound > 5)
            {
                EndGame();
                return;
            }

            // 回答者をランダムに選出
            var players = Runner.ActivePlayers.ToList();
            CurrentAnswerer = players[Random.Range(0, players.Count)];
            
            // お題を自動選出
            CurrentTopic = "太陽"; // 暫定
            
            _submissionQueue.Clear();
            SetGameState(GameState.Questioning);
            
            // 各クライアントに役割を通知
            RPC_UpdateLocalRoles();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_UpdateLocalRoles()
        {
            if (Runner.LocalPlayer == CurrentAnswerer)
                SetPlayerRole(PlayerRole.Answerer);
            else
                SetPlayerRole(PlayerRole.Questioner);
        }

        private void EndGame()
        {
            SetGameState(GameState.GameOver);
            Debug.Log("ゲーム終了。結果発表フェーズへ");
        }

        #endregion

        #region 同期プロパティ通知

        public void SetGameState(GameState newState)
        {
            if (CurrentState != newState) CurrentState = newState;
        }

        private void OnStateChangedInternal()
        {
            OnGameStateChanged?.Invoke(CurrentState);
            Debug.Log($"ゲーム状態が同期されました: {CurrentState}");
        }

        private void OnTopicChangedInternal()
        {
            OnTopicChanged?.Invoke(CurrentTopic);
        }

        private void OnAnswerChangedInternal()
        {
            OnScoreUpdated?.Invoke();
        }

        #endregion

        #region プレイヤー役割・ポイント管理

        public void SetPlayerRole(PlayerRole role)
        {
            if (_localPlayerRole != role)
            {
                _localPlayerRole = role;
                OnPlayerRoleChanged?.Invoke(role);
            }
        }

        public PlayerRole LocalPlayerRole => _localPlayerRole;
        public bool IsQuestioner => _localPlayerRole == PlayerRole.Questioner;
        public bool IsAnswerer => _localPlayerRole == PlayerRole.Answerer;

        public void AddScore(PlayerRef player, int amount)
        {
            if (Object.HasStateAuthority)
            {
                if (PlayerScores.TryGet(player, out int currentScore))
                {
                    PlayerScores.Set(player, currentScore + amount);
                }
                else
                {
                    PlayerScores.Set(player, amount);
                }
            }
        }

        #endregion

        #region 出題キュー管理

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_SubmitFlip(PlayerRef author, string flipDataJson)
        {
            _submissionQueue.Add(new SubmittedFlip { Author = author, FlipDataJson = flipDataJson });
            
            if (CurrentState == GameState.Questioning)
            {
                SetGameState(GameState.Answering);
                RPC_DisplayNextFlip();
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_DisplayNextFlip()
        {
            if (_submissionQueue.Count > 0)
            {
                var flip = _submissionQueue[0];
                OnFlipDisplayed?.Invoke(flip.FlipDataJson);
                Debug.Log($"次のフリップを表示: 作者={flip.Author}");
            }
        }

        #endregion

        #region 回答・判定

        public void OnAnswererSubmitted(string answer)
        {
            if (IsAnswerer) RPC_SubmitAnswer(Runner.LocalPlayer, answer);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_SubmitAnswer(PlayerRef player, string answer)
        {
            LastAnswer = answer;
            bool isCorrect = answer.Equals(CurrentTopic, System.StringComparison.OrdinalIgnoreCase);
            
            if (isCorrect)
            {
                AddScore(CurrentAnswerer, 3);
                if (_submissionQueue.Count > 0)
                {
                    AddScore(_submissionQueue[0].Author, 5);
                }
                ShowResult(true);
                Invoke(nameof(StartNextRound), 3f);
            }
            else
            {
                _submissionQueue.RemoveAt(0);
                if (_submissionQueue.Count > 0)
                {
                    RPC_DisplayNextFlip();
                }
                else
                {
                    SetGameState(GameState.Questioning);
                }
                OnAnswerResult?.Invoke(false);
            }
        }

        public void ShowResult(bool isCorrect)
        {
            SetGameState(GameState.ShowingResult);
            OnAnswerResult?.Invoke(isCorrect);
        }

        #endregion
    }
}