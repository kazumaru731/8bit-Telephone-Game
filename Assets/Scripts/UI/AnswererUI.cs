using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KanjiFlipGame.Core;
using KanjiFlipGame.Kanji;

namespace KanjiFlipGame.UI
{
    /// <summary>
    /// 回答者用のUIを制御するクラス
    /// フリップ表示、回答入力、結果表示を管理します
    /// </summary>
    public class AnswererUI : MonoBehaviour
    {
        [Header("UI要素")]
        [SerializeField] private GameObject _answererPanel;
        [SerializeField] private GameObject _waitingPanel;
        [SerializeField] private TextMeshProUGUI _waitingMessageText;
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private TMP_InputField _answerInputField;
        [SerializeField] private Button _submitAnswerButton;
        [SerializeField] private TextMeshProUGUI _resultText;

        [Header("参照")]
        [SerializeField] private KanjiFlipper _kanjiFlipper;

        void Start()
        {
            // ボタンイベントの設定
            if (_submitAnswerButton != null)
                _submitAnswerButton.onClick.AddListener(OnSubmitAnswerClicked);

            // GameManagerのイベントを購読
            GameManager.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
            GameManager.Instance.OnAnswerResult.AddListener(OnAnswerResult);

            // 初期状態の設定
            UpdateUI();
        }

        /// <summary>
        /// 回答を送信ボタンが押された
        /// </summary>
        private void OnSubmitAnswerClicked()
        {
            if (_answerInputField == null)
                return;

            string answer = _answerInputField.text;

            if (string.IsNullOrEmpty(answer))
            {
                Debug.LogWarning("回答を入力してください");
                return;
            }

            // 回答を送信
            GameManager.Instance.OnAnswererSubmitted(answer);

            // 入力欄をクリア
            _answerInputField.text = "";
        }

        /// <summary>
        /// ゲーム状態が変更された時の処理
        /// </summary>
        private void OnGameStateChanged(GameState newState)
        {
            UpdateUI();
        }

        /// <summary>
        /// UIの表示を更新
        /// </summary>
        private void UpdateUI()
        {
            GameState currentState = GameManager.Instance.CurrentState;

            switch (currentState)
            {
                case GameState.Waiting:
                case GameState.Questioning:
                    // 出題者が考えている間は待機画面
                    ShowWaitingPanel("出題者が考えています...");
                    break;

                case GameState.Answering:
                    // 回答フェーズ：回答者パネルを表示
                    ShowAnswererPanel();
                    break;

                case GameState.ShowingResult:
                    // 結果表示
                    // OnAnswerResultで処理
                    break;

                default:
                    HideAllPanels();
                    break;
            }
        }

        /// <summary>
        /// 待機パネルを表示
        /// </summary>
        private void ShowWaitingPanel(string message)
        {
            if (_answererPanel != null)
                _answererPanel.SetActive(false);
                
            if (_waitingPanel != null)
            {
                _waitingPanel.SetActive(true);
                if (_waitingMessageText != null)
                    _waitingMessageText.text = message;
            }
                
            if (_resultPanel != null)
                _resultPanel.SetActive(false);
        }

        /// <summary>
        /// 回答者パネルを表示
        /// </summary>
        private void ShowAnswererPanel()
        {
            if (_answererPanel != null)
                _answererPanel.SetActive(true);
                
            if (_waitingPanel != null)
                _waitingPanel.SetActive(false);
                
            if (_resultPanel != null)
                _resultPanel.SetActive(false);

            // TODO: ネットワーク経由でフリップデータを受け取り、表示
            // 現在はローカルプロトタイプなので省略
        }

        /// <summary>
        /// 結果パネルを表示
        /// </summary>
        private void ShowResultPanel(bool isCorrect)
        {
            if (_answererPanel != null)
                _answererPanel.SetActive(false);
                
            if (_waitingPanel != null)
                _waitingPanel.SetActive(false);
                
            if (_resultPanel != null)
            {
                _resultPanel.SetActive(true);
                
                if (_resultText != null)
                    _resultText.text = isCorrect ? "○ 正解！" : "× 不正解";
            }
        }

        /// <summary>
        /// 全てのパネルを非表示
        /// </summary>
        private void HideAllPanels()
        {
            if (_answererPanel != null)
                _answererPanel.SetActive(false);
                
            if (_waitingPanel != null)
                _waitingPanel.SetActive(false);
                
            if (_resultPanel != null)
                _resultPanel.SetActive(false);
        }

        /// <summary>
        /// 回答結果を受け取った時の処理
        /// </summary>
        private void OnAnswerResult(bool isCorrect)
        {
            ShowResultPanel(isCorrect);
        }

        /// <summary>
        /// フリップデータを受信して表示（ネットワーク用）
        /// </summary>
        public void DisplayFlipData(FlipData flipData)
        {
            if (_kanjiFlipper != null)
            {
                _kanjiFlipper.LoadFlipData(flipData);
            }
        }

        void OnDestroy()
        {
            // イベントリスナーのクリーンアップ
            if (_submitAnswerButton != null)
                _submitAnswerButton.onClick.RemoveListener(OnSubmitAnswerClicked);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged.RemoveListener(OnGameStateChanged);
                GameManager.Instance.OnAnswerResult.RemoveListener(OnAnswerResult);
            }
        }
    }
}
