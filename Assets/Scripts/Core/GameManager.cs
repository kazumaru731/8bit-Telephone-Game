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
        public UnityEvent<string> OnFlipDisplayed = new UnityEvent<string>(); // 追加

        public override void Spawned()
        {
            if (Instance == null) Instance = this;
            Debug.Log("GameManagerがネットワーク上に生成されました");
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

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
            
            // お題を自動選出（後でKanjiDatabase連携）
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
            
            // もし誰も出題していなければ即座に回答フェーズへ
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
                // 正解：ポイント加算
                AddScore(CurrentAnswerer, 3);
                if (_submissionQueue.Count > 0)
                {
                    AddScore(_submissionQueue[0].Author, 5);
                }
                ShowResult(true);
                // 次のラウンドへ（少し待機してから）
                Invoke(nameof(StartNextRound), 3f);
            }
            else
            {
                // 不正解：次のフリップへ
                _submissionQueue.RemoveAt(0);
                if (_submissionQueue.Count > 0)
                {
                    RPC_DisplayNextFlip();
                }
                else
                {
                    // キューが空なら出題待ちに戻る
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
