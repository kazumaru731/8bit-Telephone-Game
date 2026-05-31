# 完全版フォントアセット作成ガイド

このガイドに従って、**全ての漢字を含むフォントアセット**を作成します。

## ⚠️ 重要な注意事項

- **生成に時間がかかります**（3〜10分程度）
- **メモリを多く使用します**（8GB以上のRAM推奨）
- **ファイルサイズが大きくなります**（10〜50MB程度）

---

## 📝 手順

### ステップ1: Font Asset Creator を開く

1. Unity エディタで **Window** → **TextMeshPro** → **Font Asset Creator** を選択
2. Font Asset Creator ウィンドウが開きます

### ステップ2: 設定を行う

以下の設定を**正確に**入力してください：

#### a. Source Font File
- 右側の丸いアイコン（ターゲットアイコン）をクリック
- `NotoSansJP-Regular` を選択

#### b. Sampling Point Size
- `Auto Sizing` を選択

#### c. Padding
- `5` と入力

#### d. Packing Method
- **`Fast`** を選択（重要！`Optimum`だと数時間かかることがあります）

#### e. Atlas Resolution
- **`4096 x 4096`** を選択
  - ⚠️ `8192 x 8192` は非常に重いため、まずは4096で試してください

#### f. Character Set（重要！）
- ドロップダウンで **`Unicode Range (Hex)`** を選択
- すると、すぐ下に **Character Sequence (Hex)** フィールドが表示されます

#### g. Character Sequence (Hex)
- 以下を**正確にコピー＆ペースト**してください：
  ```
  4E00-9FFF, 3040-309F, 30A0-30FF, 0020-007F
  ```

**各範囲の意味：**
- `4E00-9FFF`: CJK統合漢字（常用漢字を含む**全ての漢字**）
- `3040-309F`: ひらがな
- `30A0-30FF`: カタカナ
- `0020-007F`: 英数字と基本記号

### ステップ3: フォントアトラスを生成

1. ウィンドウ下部の **Generate Font Atlas** ボタンをクリック

2. **待つ**（重要！）
   - プログレスバーが表示されます
   - **3〜10分程度**かかります
   - Unity が固まったように見えても、そのまま待ってください

3. 完了すると、下部のプレビューエリアに文字が表示されます
   - スクロールして漢字が含まれていることを確認

### ステップ4: 保存

1. ウィンドウ下部の **Save** ボタンをクリック

2. 保存ダイアログが開きます：
   - **保存先**: `Assets/Fonts/`
   - **ファイル名**: `NotoSansJP_Full SDF`

3. **Save** をクリック

4. 完了！

---

## 🎯 次のステップ: フォントをUIに適用

作成したフォントアセット（`NotoSansJP_Full SDF`）を全てのTextMeshProUGUIに設定します。

### 一括設定の方法：

1. **Hierarchy** ウィンドウで、以下を **Ctrl+クリック** で全て選択：

   **MainMenuPanel 内:**
   - MainMenuPanel/TitleText
   - MainMenuPanel/QuestionerButton/Text
   - MainMenuPanel/AnswererButton/Text
   - MainMenuPanel/TopicInputPanel/TopicLabel
   - MainMenuPanel/TopicInputPanel/TopicInputField/Text Area/Text
   - MainMenuPanel/TopicInputPanel/TopicInputField/Text Area/Placeholder
   - MainMenuPanel/TopicInputPanel/StartButton/Text

   **QuestionerPanel 内:**
   - QuestionerPanel/TopicDisplayText
   - QuestionerPanel/KanjiInputField/Text Area/Text
   - QuestionerPanel/KanjiInputField/Text Area/Placeholder
   - QuestionerPanel/AddKanjiButton/Text
   - QuestionerPanel/CompleteButton/Text
   - QuestionerPanel/KanjiCountText

   **その他:**
   - WaitingPanel/WaitingMessageText
   - ResultPanel/ResultText

2. **Inspector** ウィンドウで **TextMeshPro - Text (UI)** コンポーネントを見つける

3. **Font Asset** フィールドに `NotoSansJP_Full SDF` をドラッグ

4. **KanjiElement プレハブ**も設定：
   - `Assets/Prefabs/KanjiElement` をダブルクリック
   - `KanjiElement/KanjiText` を選択
   - Font Asset を `NotoSansJP_Full SDF` に設定
   - 保存（Ctrl+S）

---

## ✅ 確認

1. Unity エディタで **Play** ボタンを押す

2. 以下を確認：
   - タイトル「漢字フリップゲーム」が正しく表示される
   - ボタンのテキスト（「出題者としてプレイ」など）が正しく表示される
   - 漢字入力欄に漢字を入力できる
   - **ただし、小学1、2年生の漢字以外は「使用できない文字」エラーが出る**

**期待される動作：**
- ✅ UI: 任意の漢字を表示可能（「完成」「決定」など）
- ✅ 入力: 小学1、2年生の漢字240字のみ許可

---

## ❓ トラブルシューティング

### 問題: 生成が途中で止まる／固まる

**原因:** メモリ不足

**解決策:**
1. Unity エディタを再起動
2. Atlas Resolution を `2048 x 2048` に下げる
3. Character Sequence を減らす（例：`4E00-9FA5`）

### 問題: 一部の漢字が「□」で表示される

**原因:** フォントファイルにその漢字が含まれていない

**解決策:**
- 別のフォント（例：Noto Sans CJK JP）を使用
- Character Sequence に追加の範囲を指定

---

## 📊 参考情報

**生成後のファイルサイズ目安:**
- `2048 x 2048`: 約5〜10MB
- `4096 x 4096`: 約20〜40MB
- `8192 x 8192`: 約80〜150MB

**生成時間の目安:**
- `2048 x 2048`: 1〜3分
- `4096 x 4096`: 3〜7分
- `8192 x 8192`: 10〜20分

---

完了したら、ゲームをテストして、全て正しく動作するか確認してください！🎉
