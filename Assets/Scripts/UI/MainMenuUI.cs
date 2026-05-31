using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KanjiFlipGame.Core;

namespace KanjiFlipGame.UI
{
    /// <summary>
    /// ゲーム開始前のメインメニューを制御するクラス
    /// 役割選択（出題者/回答者）とゲーム開始を管理します
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("UI要素")]
        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private Button _questionerButton;
        [SerializeField] private Button _answererButton;
        [SerializeField] private TMP_InputField _topicInputField;
        [SerializeField] private GameObject _topicInputPanel;

        void Start()
        {
            // ボタンイベントの設定
            if (_questionerButton != null)
                _questionerButton.onClick.AddListener(OnQuestionerButtonClicked);
                
            if (_answererButton != null)
                _answererButton.onClick.AddListener(OnAnswererButtonClicked);

            // 初期状態でお題入力パネルを非表示
            if (_topicInputPanel != null)
                _topicInputPanel.SetActive(false);
        }

        /// <summary>
        /// 出題者ボタンが押された
        /// </summary>
        public void OnQuestionerButtonClicked()
        {
            // お題入力パネルを表示
            if (_topicInputPanel != null)
                _topicInputPanel.SetActive(true);
        }

        /// <summary>
        /// お題を確定して出題者としてゲーム開始
        /// </summary>
        public void StartAsQuestioner()
        {
            string topic = _topicInputField != null ? _topicInputField.text : "テストお題";
            
            if (string.IsNullOrEmpty(topic))
            {
                Debug.LogWarning("お題を入力してください");
                return;
            }

            // ゲームを開始
            GameManager.Instance.StartNewGame(PlayerRole.Questioner, topic);
            
            // メインメニューを非表示
            if (_mainMenuPanel != null)
                _mainMenuPanel.SetActive(false);
        }

        /// <summary>
        /// 回答者ボタンが押された
        /// </summary>
        public void OnAnswererButtonClicked()
        {
            // 回答者としてゲーム開始
            GameManager.Instance.StartNewGame(PlayerRole.Answerer);
            
            // メインメニューを非表示
            if (_mainMenuPanel != null)
                _mainMenuPanel.SetActive(false);
        }

        void OnDestroy()
        {
            // イベントリスナーのクリーンアップ
            if (_questionerButton != null)
                _questionerButton.onClick.RemoveListener(OnQuestionerButtonClicked);
                
            if (_answererButton != null)
                _answererButton.onClick.RemoveListener(OnAnswererButtonClicked);
        }
    }
}
