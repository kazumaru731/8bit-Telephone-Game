using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KanjiFlipGame.Core;
using KanjiFlipGame.Kanji;
using System.Collections.Generic;
using System.Linq;

namespace KanjiFlipGame.UI
{
    public class QuestionerUI : MonoBehaviour
    {
        [Header("UI要素")]
        [SerializeField] private TextMeshProUGUI _topicText;
        [SerializeField] private TMP_InputField _kanjiInputField;
        [SerializeField] private Button _addKanjiButton;
        [SerializeField] private Button _completeButton;
        [SerializeField] private TextMeshProUGUI _kanjiCountText;
        [SerializeField] private GameObject _questionerPanel;
        [SerializeField] private GameObject _waitingPanel;
        [SerializeField] private TextMeshProUGUI _waitingMessageText;
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private TextMeshProUGUI _answererAnswerText;
        [SerializeField] private TextMeshProUGUI _resultText;

        [Header("予測変換")]
        [SerializeField] private GameObject _suggestionPanel;
        [SerializeField] private RectTransform _suggestionContent;
        [SerializeField] private GameObject _suggestionButtonPrefab; // nullの場合は動的に生成

        [Header("参照")]
        [SerializeField] private KanjiFlipper _kanjiFlipper;
        [SerializeField] private KanjiInputValidator _kanjiInputValidator;

        private List<Button> _activeSuggestionButtons = new List<Button>();
        private bool _sortByStrokeCount = false;

        void Start()
        {
            if (_addKanjiButton != null)
                _addKanjiButton.onClick.AddListener(OnAddKanjiClicked);

            if (_completeButton != null)
                _completeButton.onClick.AddListener(OnCompleteClicked);

            if (_kanjiInputField != null)
                _kanjiInputField.onValueChanged.AddListener(OnInputFieldValueChanged);

            GameManager.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
            GameManager.Instance.OnAnswerResult.AddListener(OnAnswerResult);

            if (_suggestionPanel != null) _suggestionPanel.SetActive(false);

            UpdateUI();
        }

        void Update()
        {
            UpdateKanjiCount();
        }

        public void OnAddKanjiClicked()
        {
            if (_kanjiInputField == null || _kanjiFlipper == null || _kanjiInputValidator == null)
                return;

            string input = _kanjiInputField.text;
            AddTextToFlipper(input);
        }

        private void AddTextToFlipper(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            string err;
            if (_kanjiInputValidator.ValidateInput(text, out err))
            {
                foreach (char c in text)
                {
                    if (!_kanjiFlipper.CanAddKanji()) break;
                    _kanjiFlipper.AddKanji(c.ToString());
                }
                _kanjiInputField.text = "";
                if (_suggestionPanel != null) _suggestionPanel.SetActive(false);
            }
            else
            {
                Debug.LogWarning(err);
            }
        }

        private void OnInputFieldValueChanged(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (_suggestionPanel != null) _suggestionPanel.SetActive(false);
                return;
            }

            // 最後の1文字がひらがなの場合、予測変換を表示
            UpdateSuggestions(value);
        }

        private void UpdateSuggestions(string input)
        {
            if (KanjiDatabase.Instance == null || _suggestionContent == null) return;

            // 入力の最後の文字（または全体）で検索
            var results = KanjiDatabase.Instance.SearchKanji(input, _sortByStrokeCount);

            if (results.Count == 0)
            {
                if (_suggestionPanel != null) _suggestionPanel.SetActive(false);
                return;
            }

            if (_suggestionPanel != null) _suggestionPanel.SetActive(true);

            // 既存のボタンをクリア（プールするか、Destroy）
            foreach (var btn in _activeSuggestionButtons)
            {
                if (btn != null) Destroy(btn.gameObject);
            }
            _activeSuggestionButtons.Clear();

            // 新しいボタンを生成
            foreach (var kanji in results.Take(20)) // 最大20件
            {
                GameObject btnGo;
                if (_suggestionButtonPrefab != null)
                {
                    btnGo = Instantiate(_suggestionButtonPrefab, _suggestionContent);
                }
                else
                {
                    btnGo = CreateDefaultSuggestionButton(kanji.@char);
                }

                Button btn = btnGo.GetComponent<Button>();
                var kChar = kanji.@char; // クロージャ用
                btn.onClick.AddListener(() => OnSuggestionSelected(kChar));
                _activeSuggestionButtons.Add(btn);
            }
        }

        private GameObject CreateDefaultSuggestionButton(string kanji)
        {
            GameObject btnGo = new GameObject("SuggBtn", typeof(RectTransform), typeof(Image), typeof(Button));
            btnGo.transform.SetParent(_suggestionContent, false);
            
            GameObject txtGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            txtGo.transform.SetParent(btnGo.transform, false);
            
            var txt = txtGo.GetComponent<TextMeshProUGUI>();
            txt.text = kanji;
            txt.fontSize = 24;
            txt.color = Color.black;
            txt.alignment = TextAlignmentOptions.Center;
            
            var r = txtGo.GetComponent<RectTransform>();
            r.anchorMin = Vector2.zero;
            r.anchorMax = Vector2.one;
            r.sizeDelta = Vector2.zero;
            
            return btnGo;
        }

        private void OnSuggestionSelected(string kanji)
        {
            AddTextToFlipper(kanji);
            _kanjiInputField.ActivateInputField();
        }

        public void OnCompleteClicked()
        {
            if (_kanjiFlipper == null || _kanjiFlipper.CurrentKanjiCount == 0) return;

            // フリップデータをシリアライズ
            var flipData = _kanjiFlipper.GetFlipData();
            string json = JsonUtility.ToJson(flipData);

            // ネットワーク経由で送信（早押しキューに登録）
            GameManager.Instance.RPC_SubmitFlip(GameManager.Instance.Runner.LocalPlayer, json);

            // 自身は送信完了したので、出題パネルを閉じて待機
            ShowWaitingPanel("出題済み。判定を待っています...");
            
            // ローカルのフリップをクリア
            _kanjiFlipper.ClearAll();
        }

        private void OnGameStateChanged(GameState newState)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (GameManager.Instance.LocalPlayerRole != PlayerRole.Questioner)
            {
                HideAllPanels();
                return;
            }

            GameState currentState = GameManager.Instance.CurrentState;
            switch (currentState)
            {
                case GameState.Questioning:
                    // まだ出題ボタンを押していない場合は出題パネルを表示
                    if (!_waitingPanel.activeSelf)
                    {
                        ShowQuestionerPanel();
                    }
                    break;

                case GameState.Answering:
                    // 出題者が回答中を確認できるよう、状況に応じて表示
                    // (プラン通りなら、出題済みなら待機パネルにメッセージが出ているはず)
                    break;
                
                case GameState.ShowingResult:
                    // OnAnswerResultで処理
                    break;

                default:
                    HideAllPanels();
                    break;
            }
        }

        private void ShowQuestionerPanel()
        {
            if (_questionerPanel != null) _questionerPanel.SetActive(true);
            if (_waitingPanel != null) _waitingPanel.SetActive(false);
            if (_resultPanel != null) _resultPanel.SetActive(false);
            if (_topicText != null) _topicText.text = "お題: " + GameManager.Instance.CurrentTopic;
        }

        private void ShowWaitingPanel(string message)
        {
            if (_questionerPanel != null) _questionerPanel.SetActive(false);
            if (_waitingPanel != null)
            {
                _waitingPanel.SetActive(true);
                if (_waitingMessageText != null) _waitingMessageText.text = message;
            }
            if (_resultPanel != null) _resultPanel.SetActive(false);
        }

        private void ShowResultPanel(string answererAnswer, bool isCorrect)
        {
            if (_questionerPanel != null) _questionerPanel.SetActive(false);
            if (_waitingPanel != null) _waitingPanel.SetActive(false);
            if (_resultPanel != null)
            {
                _resultPanel.SetActive(true);
                if (_answererAnswerText != null) _answererAnswerText.text = " 回答者の答え: " + answererAnswer;
                if (_resultText != null) _resultText.text = isCorrect ? "○" : "×";
            }
        }

        private void HideAllPanels()
        {
            if (_questionerPanel != null) _questionerPanel.SetActive(false);
            if (_waitingPanel != null) _waitingPanel.SetActive(false);
            if (_resultPanel != null) _resultPanel.SetActive(false);
        }

        private void UpdateKanjiCount()
        {
            if (_kanjiCountText != null && _kanjiFlipper != null)
            {
                _kanjiCountText.text = _kanjiFlipper.CurrentKanjiCount + " / " + _kanjiFlipper.MaxKanjiCount;
            }
        }

        private void OnAnswerResult(bool isCorrect)
        {
            ShowResultPanel(GameManager.Instance.LastAnswer, isCorrect);
        }

        void OnDestroy()
        {
            if (_addKanjiButton != null) _addKanjiButton.onClick.RemoveListener(OnAddKanjiClicked);
            if (_completeButton != null) _completeButton.onClick.RemoveListener(OnCompleteClicked);
            if (_kanjiInputField != null) _kanjiInputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged.RemoveListener(OnGameStateChanged);
                GameManager.Instance.OnAnswerResult.RemoveListener(OnAnswerResult);
            }
        }
    }
}
