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
        [SerializeField] private GameObject _roomWaitingPanel;
        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private TMP_InputField _roomIdInputField;
        [SerializeField] private TextMeshProUGUI _statusText;

        void Start()
        {
            if (_joinButton != null)
                _joinButton.onClick.AddListener(OnJoinButtonClicked);
                
            if (_startGameButton != null)
                _startGameButton.onClick.AddListener(OnStartGameButtonClicked);

            if (_roomWaitingPanel != null)
                _roomWaitingPanel.SetActive(false);
                
            if (_startGameButton != null)
                _startGameButton.gameObject.SetActive(false);
        }

        private void Update()
        {
            // ホスト（マスタークライアント）のみゲーム開始ボタンを表示
            if (_roomWaitingPanel.activeSelf && _startGameButton != null)
            {
                bool isMaster = KanjiFlipGame.Network.NetworkLauncher.Instance.IsMaster;
                _startGameButton.gameObject.SetActive(isMaster);
            }
        }

        /// <summary>
        /// 入室ボタンが押された
        /// </summary>
        public void OnJoinButtonClicked()
        {
            string roomId = _roomIdInputField != null ? _roomIdInputField.text : "";
            
            if (string.IsNullOrEmpty(roomId))
            {
                KanjiFlipGame.Network.NetworkLauncher.Instance.StartRandomMatch(PlayerRole.None);
            }
            else
            {
                KanjiFlipGame.Network.NetworkLauncher.Instance.StartFriendMatch(roomId, PlayerRole.None);
            }

            _mainMenuPanel.SetActive(false);
            if (_roomWaitingPanel != null) _roomWaitingPanel.SetActive(true);
            if (_statusText != null) _statusText.text = "接続中...";
        }

        /// <summary>
        /// ホストがゲーム開始ボタンを押した
        /// </summary>
        public void OnStartGameButtonClicked()
        {
            GameManager.Instance.Host_StartGame();
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            if (_joinButton != null)
                _joinButton.onClick.RemoveListener(OnJoinButtonClicked);
                
            if (_startGameButton != null)
                _startGameButton.onClick.RemoveListener(OnStartGameButtonClicked);
        }
    }
}
