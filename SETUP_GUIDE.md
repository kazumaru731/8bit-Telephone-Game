# 漢字フリップゲーム - 手動設定ガイド

## ✅ 自動設定完了項目

以下の設定は**既に完了しています**：

### スクリプト参照の接続
- ✅ **MainMenuUI** の全参照（_mainMenuPanel、_questionerButton、_answererButton、_topicInputField、_topicInputPanel）
- ✅ **QuestionerUI** の全参照（12個のUI要素への参照）
- ✅ **KanjiFlipper** の _flipCanvas 参照

---

## 🔧 残りの手動設定（必須）

以下の作業をUnity エディタで実行してください。

### 1. KanjiElementプレハブの作成 ⭐ **最重要**

このプレハブがないと漢字を配置できません。

#### 手順：

1. **Hierarchy**で右クリック → **UI** → **Empty** を選択
2. 名前を「**KanjiElement**」に変更
3. 以下のコンポーネントを追加：
   - **Add Component** → **UI** → **Image**
   - **Add Component** → **Scripts** → **KanjiElement** (KanjiFlipGame.Kanji)

4. **RectTransform** の設定：
   - Width: `120`
   - Height: `120`

5. **Image** コンポーネントの設定：
   - Color: 透明にする（Alphaを `0` に設定）

6. **子オブジェクトとしてテキストを作成**：
   - KanjiElementを右クリック → **UI** → **Text - TextMeshPro**
   - 名前を「**KanjiText**」に変更
   
   - **RectTransformを親と同じサイズに設定**（2つの方法があります）：
   
     **方法A（簡単）：手動で値を設定**
     - Inspector で **Rect Transform** を見つける
     - 以下の値を手動で入力：
       - **Left**: `0`
       - **Top**: `0`
       - **Right**: `0`
       - **Bottom**: `0`
     
     **方法B（Anchor Preset使用）：**
     - Inspector で **Rect Transform** の左上にある**四角いアイコン**をクリック
       （「Anchor Presets」と表示される小さな四角）
     - 開いたメニューで、**右下の「Stretch/Stretch」**を選択
       （一番右下の、四方に矢印が伸びているアイコン）
     - **Alt/Option キーを押しながら**クリックすると、Position も同時に設定されます
   
   - **TextMeshProUGUI の設定**：
     - **Hierarchy で KanjiText オブジェクトを選択**
     - **Inspector** に **TextMeshPro - Text (UI)** コンポーネントが表示されます
     - そこで以下を設定：
       - **Alignment**: **Center** (Alignmentの中央ボタンをクリック)
       - **Font Size**: `60`
       - **Text**: `漢` (テスト用の文字を入力)
       - **Vertex Color**: 黒 (0, 0, 0, 255) または既に黒ならそのまま

7. **子オブジェクト「ResizeButtonsPanel」を作成**：
   - KanjiElementを右クリック → **UI** → **Empty**
   - 名前を「**ResizeButtonsPanel**」に変更
   - **Add Component** → **UI** → **Image**
   - **RectTransform**:
     - Anchor: **Top Center**
     - Pos Y: 少し上（漢字の上に表示されるように）
     - Width: `100`, Height: `40`
   - 初期状態で非表示にする（Inspectorのチェックを外す）

8. **ResizeButtonsPanelの子として2つのボタンを作成**：

   **Aボタン「IncreaseSizeButton」（+ボタン）**：
   - ResizeButtonsPanelを右クリック → **UI** → **Button**
   - 名前: `IncreaseSizeButton`
   - RectTransform: 左半分に配置
   - Image Color: 緑系 (0.3, 0.7, 0.4, 1)
   - 子のText (TMP): `+` に設定

   **Bボタン「DecreaseSizeButton」（-ボタン）**：
   - ResizeButtonsPanelを右クリック → **UI** → **Button**
   - 名前: `DecreaseSizeButton`
   - RectTransform: 右半分に配置
   - Image Color: 赤系 (0.7, 0.4, 0.4, 1)
   - 子のText (TMP): `-` に設定

9. **KanjiElementスクリプトの参照を設定**：
   - KanjiElementオブジェクトを選択
   - Inspector で **KanjiElement** コンポーネントを見つける
   - 以下をドラッグ＆ドロップで設定：
     - `Kanji Text` → **KanjiText** 子オブジェクト（のTextMeshProUGUIコンポーネント）
     - `Resize Buttons Panel` → ResizeButtonsPanel オブジェクト
     - `Increase Size Button` → IncreaseSizeButton
     - `Decrease Size Button` → DecreaseSizeButton

10. **プレハブ化**：
    - Hierarchy から **KanjiElement** オブジェクトを **Assets/Prefabs** フォルダにドラッグ
    - プレハブが作成されたら、Hierarchy の元のオブジェクトは削除してOK

11. **KanjiFlipperに設定**：
    - Hierarchy で **QuestionerPanel/FlipCanvas** を選択
    - Inspector で **Kanji Flipper** コンポーネントを見つける
    - `Kanji Element Prefab` に、作成した **KanjiElement プレハブ**をドラッグ

---

### 2. ボタンイベントの接続

各ボタンの **OnClick()** イベントを設定します。

#### 手順：

1. **QuestionerButton** を選択
   - Inspector → **Button** コンポーネント → **On Click ()** 
   - `+` をクリック
   - **MainMenuPanel** オブジェクトをドラッグ
   - Function: `MainMenuUI` → `OnQuestionerButtonClicked()`

2. **AnswererButton** を選択
   - 同様に **On Click ()** に `+`
   - **MainMenuPanel** をドラッグ
   - Function: `MainMenuUI` → `OnAnswererButtonClicked()`

3. **TopicInputPanel/StartButton** を選択
   - 同様に **On Click ()** に `+`
   - **MainMenuPanel** をドラッグ
   - Function: `MainMenuUI` → `StartAsQuestioner()`

4. **QuestionerPanel/AddKanjiButton** を選択
   - **On Click ()** に `+`
   - **QuestionerPanel** をドラッグ
   - Function: `QuestionerUI` → `OnAddKanjiClicked()`

5. **QuestionerPanel/CompleteButton** を選択
   - **On Click ()** に `+`
   - **QuestionerPanel** をドラッグ
   - Function: `QuestionerUI` → `OnCompleteClicked()`

---

### 3. TextMeshPro のセットアップ（日本語フォント）

日本語を表示するためのフォント設定が必要です。

#### 手順：

1. **TMP Essentials のインポート**（初回のみ）：
   - メニュー: **Window** → **TextMeshPro** → **Import TMP Essential Resources**
   - Importボタンをクリック

2. **日本語フォントのインポート**（推奨：Noto Sans JP）：
   - Google Fonts などから Noto Sans JP をダウンロード
   - .ttf ファイルを Unity の **Assets/Fonts** フォルダにドラッグ

3. **フォントアセットの作成**：
   - メニュー: **Window** → **TextMeshPro** → **Font Asset Creator**
   - **Source Font File**: インポートした日本語フォントを選択
   - **Character Set**: `Unicode Range (Hex)` を選択
   - **Character Sequence**: `3000-9FFF` （漢字の範囲）
   - **Generate Font Atlas** をクリック（時間がかかります）
   - **Save** で `Assets/Fonts/NotoSansJP SDF` などの名前で保存

4. **全てのTextMeshProUGUIに日本語フォントを設定**：
   - 各テキストコンポーネントを選択
   - **Font Asset** に作成したフォントアセットを設定
   
   **設定が必要な箇所**：
   - MainMenuPanel/TitleText
   - MainMenuPanel/QuestionerButton/Text
   - MainMenuPanel/AnswererButton/Text
   - MainMenuPanel/TopicInputPanel/TopicLabel
   - MainMenuPanel/TopicInputPanel/StartButton/Text
   - QuestionerPanel/TopicDisplayText
   - QuestionerPanel/AddKanjiButton/Text
   - QuestionerPanel/CompleteButton/Text
   - QuestionerPanel/KanjiCountText
   - WaitingPanel/WaitingMessageText
   - ResultPanel/ResultText
   - KanjiElement プレハブの TextMeshProUGUI

---

### 4. InputField の確認（通常は自動設定済み）

TMP_InputField コンポーネントは通常、子オブジェクトとして自動的に Text と Placeholder が生成されています。念のため確認：

1. **TopicInputField** を確認：
   - 子に `Text Area` → `Text` と `Placeholder` があることを確認
   - なければ、手動で作成（通常は不要）

2. **KanjiInputField** を確認：
   - 同様に確認

---

## 🎮 テスト実行

全ての設定が完了したら、Unity エディタで **Play ボタン**を押してテスト：

### テスト手順：

1. **メインメニューが表示されるか確認**
2. **「出題者としてプレイ」ボタンをクリック**
3. お題入力パネルが表示されるか確認
4. **お題を入力**して「開始」ボタンをクリック
5. 出題者画面に切り替わるか確認
6. **お題が上部に表示されているか確認**
7. 漢字入力欄に漢字（例：「山」）を入力
8. **「決定」ボタンをクリック**
9. **✅ フリップに漢字が追加されるか確認**
10. 追加された漢字をドラッグして移動できるか確認
11. 漢字をクリックしてサイズ変更ボタンが表示されるか確認
12. 右クリックで回転できるか確認

### 期待される動作：

- ✅ メインメニューから出題者画面への遷移
- ✅ 漢字の追加（最大8文字）
- ✅ 漢字のドラッグ移動
- ✅ 漢字のサイズ変更（+ / - ボタン）
- ✅ 漢字の回転（右クリック）
- ✅ カウント表示の更新（「1 / 8」など）

---

## ❓ トラブルシューティング

### 問題：漢字が追加されない
- KanjiElement プレハブが正しく設定されているか確認
- Console でエラーメッセージを確認

### 問題：ボタンが反応しない
- OnClick イベントが正しく設定されているか確認
- EventSystem が存在するか確認（Hierarchy に EventSystem があるはず）

### 問題：日本語が「□」で表示される
- 日本語フォントアセットが作成されているか確認
- TextMeshProUGUI の Font Asset が設定されているか確認

### 問題：漢字のドラッグができない
- EventSystem が存在するか確認
- KanjiElement に GraphicRaycaster が必要（通常は親Canvasにある）

---

## 📝 次のステップ

手動設定が完了したら：

1. 基本的な動作確認
2. 回答者側のUI実装（必要に応じて）
3. フェーズ2: Photon PUN 2 のネットワーク統合

---

**ご不明な点があれば、お気軽にお聞きください！** 🚀
