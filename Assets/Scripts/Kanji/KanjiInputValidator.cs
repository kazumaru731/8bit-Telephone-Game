using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace KanjiFlipGame.Kanji
{
    /// <summary>
    /// 漢字入力の検証を行うクラス
    /// 小学1、2年生で習う漢字のみを許可します
    /// </summary>
    public class KanjiInputValidator : MonoBehaviour
    {
        [Header("設定")]
        [Tooltip("最大文字数（デフォルトは8文字）")]
        [SerializeField] private int _maxCharacters = 8;

        /// <summary>
        /// 入力された文字列が有効かどうかを検証
        /// </summary>
        /// <param name="input">検証する文字列</param>
        /// <param name="errorMessage">エラーメッセージ（エラーがある場合）</param>
        /// <returns>有効な場合true</returns>
        public bool ValidateInput(string input, out string errorMessage)
        {
            errorMessage = string.Empty;

            // 空文字チェック
            if (string.IsNullOrEmpty(input))
            {
                errorMessage = "文字を入力してください";
                return false;
            }

            // 文字数チェック
            if (input.Length > _maxCharacters)
            {
                errorMessage = _maxCharacters + " 文字以内で入力してください";
                return false;
            }

            // 許可された漢字かチェック
            List<char> invalidChars = new List<char>();
            foreach (char c in input)
            {
                if (!KanjiDatabase.Instance.IsAllowedKanji(c))
                {
                    invalidChars.Add(c);
                }
            }

            if (invalidChars.Count > 0)
            {
                string invalidCharsStr = string.Join(", ", invalidChars.Select(c => c.ToString()).ToArray());
                errorMessage = "使用できない文字が含まれています: " + invalidCharsStr + "\n小学1、2年生で習う漢字のみ使用できます";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 単一の文字が有効な漢字かどうかを検証
        /// </summary>
        /// <param name="character">検証する文字</param>
        /// <returns>有効な場合true</returns>
        public bool ValidateCharacter(char character)
        {
            return KanjiDatabase.Instance.IsAllowedKanji(character);
        }

        /// <summary>
        /// 最大文字数を取得
        /// </summary>
        public int MaxCharacters => _maxCharacters;

        /// <summary>
        /// 最大文字数を設定
        /// </summary>
        public void SetMaxCharacters(int max)
        {
            _maxCharacters = Mathf.Max(1, max);
        }
    }
}
