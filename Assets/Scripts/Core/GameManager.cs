using UnityEngine;
using UnityEngine.Events;

namespace KanjiFlipGame.Core
{
    /// <summary>
    /// ゲーム全体の状態を管理するマネージャー
    /// シングルトンパターンで実装されています
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        [Header("ゲーム設定")]
        [SerializeField] private string _currentTopic = "";
        
        // 現在のゲーム状態
        private GameState _currentState = GameState.Waiting;
        
        // ローカルプレイヤーの役割
        private PlayerRole _localPlayerRole = PlayerRole.None;

        // イベント
        public UnityEvent<GameState> OnGameStateChanged = new UnityEvent<GameState>();
        public UnityEvent<PlayerRole> OnPlayerRoleChanged = new UnityEvent<PlayerRole>();
        public UnityEvent<string> OnTopicChanged = new UnityEvent<string>();
        public UnityEvent<bool> OnAnswerResult = new UnityEvent<bool>();

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #region ゲーム状態管理

        /// <summary>
        /// ゲーム状態を変更
        /// </summary>
        public void SetGameState(GameState newState)
        {
            if (_currentState != newState)
            {
                _currentState = newState;
                OnGameStateChanged?.Invoke(newState);
                Debug.Log($"ゲーム状態が変更されました: {newState}");
            }
        }

        /// <summary>
        /// 現在のゲーム状態を取得
        /// </summary>
        public GameState CurrentState => _currentState;

        #endregion

        #region プレイヤー役割管理

        /// <summary>
        /// ローカルプレイヤーの役割を設定
        /// </summary>
        public void SetPlayerRole(PlayerRole role)
        {
            if (_localPlayerRole != role)
            {
                _localPlayerRole = role;
                OnPlayerRoleChanged?.Invoke(role);
                Debug.Log($"プレイヤーの役割が設定されました: {role}");
            }
        }

        /// <summary>
        /// ローカルプレイヤーの役割を取得
        /// </summary>
        public PlayerRole LocalPlayerRole => _localPlayerRole;

        /// <summary>
        /// ローカルプレイヤーが出題者かどうか
        /// </summary>
        public bool IsQuestioner => _localPlayerRole == PlayerRole.Questioner;

        /// <summary>
        /// ローカルプレイヤーが回答者かどうか
        /// </summary>
        public bool IsAnswerer => _localPlayerRole == PlayerRole.Answerer;

        #endregion

        #region お題管理

        /// <summary>
        /// お題を設定（出題者用）
        /// </summary>
        public void SetTopic(string topic)
        {
            _currentTopic = topic;
            OnTopicChanged?.Invoke(topic);
            Debug.Log($"お題が設定されました: {topic}");
        }

        /// <summary>
        /// 現在のお題を取得
        /// </summary>
        public string CurrentTopic => _currentTopic;

        #endregion

        #region ゲームフロー制御

        /// <summary>
        /// 新しいゲームを開始
        /// </summary>
        public void StartNewGame(PlayerRole playerRole, string topic = "")
        {
            SetPlayerRole(playerRole);
            
            if (playerRole == PlayerRole.Questioner)
            {
                SetTopic(topic);
                SetGameState(GameState.Questioning);
            }
            else
            {
                SetGameState(GameState.Waiting);
            }
        }

        /// <summary>
        /// 出題者がフリップを完成させた
        /// </summary>
        public void OnQuestionerFinished()
        {
            if (_localPlayerRole == PlayerRole.Questioner)
            {
                SetGameState(GameState.Answering);
                Debug.Log("出題者がフリップを完成させました。回答者の回答を待っています");
            }
        }

        /// <summary>
        /// 回答者が回答を送信
        /// </summary>
        public void OnAnswererSubmitted(string answer)
        {
            if (_localPlayerRole == PlayerRole.Answerer)
            {
                Debug.Log($"回答者が回答を送信しました: {answer}");
                // ここで回答を検証（実際にはネットワーク経由で出題者に送信）
                // 現在はローカルプロトタイプなので直接検証
                bool isCorrect = CheckAnswer(answer);
                ShowResult(isCorrect);
            }
        }

        /// <summary>
        /// 回答を検証（簡易版、後でネットワーク対応にする）
        /// </summary>
        private bool CheckAnswer(string answer)
        {
            // 現在は単純な文字列比較
            // 後でネットワーク経由で出題者が正誤を判定する仕組みに変更
            return answer.Equals(_currentTopic, System.StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 結果を表示
        /// </summary>
        public void ShowResult(bool isCorrect)
        {
            SetGameState(GameState.ShowingResult);
            OnAnswerResult?.Invoke(isCorrect);
            Debug.Log($"結果: {(isCorrect ? "正解！" : "不正解")}");
        }

        /// <summary>
        /// ゲームをリセット
        /// </summary>
        public void ResetGame()
        {
            _currentTopic = "";
            _localPlayerRole = PlayerRole.None;
            SetGameState(GameState.Waiting);
            Debug.Log("ゲームがリセットされました");
        }

        #endregion
    }
}
