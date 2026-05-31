# TextMeshPro 日本語フォント設定ガイド

このゲームでは日本語（漢字）を表示するため、TextMeshPro用の日本語フォントアセットが必要です。

## 📝 設定手順

### ステップ1: TMP Essential Resources のインポート（初回のみ）

1. Unity エディタで **Window** → **TextMeshPro** → **Import TMP Essential Resources** を選択
2. Import ウィンドウが開いたら **Import** ボタンをクリック
3. 完了するまで待つ

### ステップ2: 日本語フォントの準備

#### オプションA: Noto Sans JP をダウンロード（推奨）

1. ブラウザで Google Fonts にアクセス: https://fonts.google.com/noto/specimen/Noto+Sans+JP
2. **Download family** ボタンをクリック
3. ダウンロードしたZIPファイルを解凍
4. `NotoSansJP-Regular.ttf` または `NotoSansJP-Medium.ttf` を見つける
5. Unity の **Assets/Fonts** フォルダにドラッグ＆ドロップ
   - Fontsフォルダがない場合は作成してください

#### オプションB: 他の日本語フォントを使用

- M+ FONTS
- 源ノ角ゴシック（Source Han Sans）
- その他のフリーフォント

**重要:** 商用利用可能なフォントを選んでください。

### ステップ3: フォントアセットの作成

#### 簡易版（テスト用、小さいサイズ）

1. **Window** → **TextMeshPro** → **Font Asset Creator** を開く

2. **キャラクターファイルを先に作成**（重要！）：
   - Unityの Project ウィンドウで右クリック → **Create** → **Text File**
   - 名前を `KanjiCharacters.txt` にする
   - ダブルクリックして開く（外部エディタで開きます）
   - 小学1、2年生の漢字240字をコピー＆ペースト：
     ```
     一右雨円王音下火花貝学気九休玉金空月犬見五口校左三山子四糸字耳七車手十出女小上森人水正生青夕石赤千川先早草足村大男竹中虫町天田土二日入年白八百文木本名目立力林六引羽雲園遠何科夏家歌画回会海絵外角楽活間丸岩顔汽記帰弓牛魚京強教近兄形計元言原戸古午後語工公広交光考行高黄合谷国黒今才細作算止市矢姉思紙寺自時室社弱首秋週春書少場色食心新親図数西声星晴切雪船線前組走多太体台地池知茶昼長鳥朝直通弟店点電刀冬当東答頭同道読内南肉馬売買麦半番父風分聞米歩母方北毎妹万明鳴毛門夜野友用曜来里理話
     ```
   - 保存してUnityに戻る
   - ⚠️ Unityでファイルが認識されるまで数秒待つ

3. **Font Asset Creatorで設定**（順番が重要）：
   
   **a. Source Font File（一番上）:**
   - インポートした日本語フォントを選択（例：NotoSansJP-Regular）
   - 右側の小さな丸いアイコンをクリックして選択
   
   **b. Atlas Population Mode（中段）:**
   - **Character Set**: ドロップダウンで `Characters from File` を選択
   - ⚠️ これを選択すると、すぐ下に **Character File** フィールドが表示されます
   
   **c. Character File（今表示された！）:**
   - 右側の小さな丸いアイコンをクリック
   - 作成した `KanjiCharacters.txt` を選択
   
   **d. その他の設定:**
   - **Sampling Point Size**: `Auto Sizing`
   - **Padding**: `5`
   - **Packing Method**: `Optimum`
   - **Atlas Resolution**: `2048 x 2048`

4. **Generate Font Atlas** をクリック
   - 生成が完了するまで待つ（数秒～数十秒）
   - 下部のプレビューエリアに漢字が表示されます

5. **Save** ボタンを押して保存
   - 保存先: `Assets/Fonts/`
   - ファイル名: `NotoSansJP_Kanji SDF`

#### 完全版（全漢字対応、大きいサイズ）

より多くの漢字に対応したい場合：

1. Font Asset Creator で：
   - **Character Set**: `Unicode Range (Hex)`
   - **Character Sequence (Hex)**: `4E00-9FFF, 3040-309F, 30A0-30FF`
     - `4E00-9FFF`: CJK統合漢字
     - `3040-309F`: ひらがな
     - `30A0-30FF`: カタカナ
   - **Atlas Resolution**: `4096 x 4096` または `8192 x 8192`（大きめ）

2. **Generate Font Atlas** をクリック
   - ⚠️ 警告: 生成に時間がかかります（数分）
   - メモリも多く使用します

3. 保存

### ステップ4: フォントアセットを各テキストに設定

作成したフォントアセットを、ゲーム内の全てのTextMeshProUGUIコンポーネントに設定します。

#### 設定が必要なオブジェクト（全て選択して一括設定可能）：

1. Hierarchy で以下を **Ctrl+クリック** で複数選択：
   - `MainMenuPanel/TitleText`
   - `MainMenuPanel/QuestionerButton/Text`
   - `MainMenuPanel/AnswererButton/Text`
   - `MainMenuPanel/TopicInputPanel/TopicLabel`
   - `MainMenuPanel/TopicInputPanel/StartButton/Text`
   - `MainMenuPanel/TopicInputPanel/TopicInputField/Text Area/Text`
   - `MainMenuPanel/TopicInputPanel/TopicInputField/Text Area/Placeholder`

2. Inspector で **TextMeshPro - Text (UI)** コンポーネントを見つける

3. **Font Asset** フィールドに、作成したフォントアセット（例：`NotoSansJP_Kanji SDF`）をドラッグ

4. 同様に、以下も設定：
   - `QuestionerPanel/TopicDisplayText`
   - `QuestionerPanel/AddKanjiButton/Text`
   - `QuestionerPanel/CompleteButton/Text`
   - `QuestionerPanel/KanjiCountText`
   - `QuestionerPanel/KanjiInputField/Text Area/Text`
   - `QuestionerPanel/KanjiInputField/Text Area/Placeholder`
   - `WaitingPanel/WaitingMessageText`
   - `ResultPanel/ResultText`

5. **KanjiElement プレハブ**も忘れずに：
   - `Assets/Prefabs/KanjiElement` をダブルクリック
   - `KanjiElement/KanjiText` を選択
   - Font Asset を設定
   - 保存

### ステップ5: 確認

Unity エディタで **Play** ボタンを押して、日本語が正しく表示されるか確認：
- タイトル「漢字フリップゲーム」が表示される
- ボタンのテキストが表示される
- 漢字入力欄に漢字を入力できる

---

## 🚀 簡易版（すぐにテストしたい場合）

フォントアセット作成を省略して、とりあえず動作確認したい場合：

1. **Liberation Sans SDF**（Unityデフォルト）を使用
   - 各TextMeshProUGUIの Font Asset を `LiberationSans SDF` に設定
   - ⚠️ 一部の漢字が表示されない可能性があります

2. 動作確認後、正式な日本語フォントアセットを作成して差し替え

---

## ❓ トラブルシューティング

### 問題: 漢字が「□」で表示される

**原因:** フォントアセットに該当する漢字が含まれていない

**解決策:**
1. Font Asset Creator で **Character Set** を `Unicode Range (Hex)` に変更
2. **Character Sequence** に `4E00-9FFF` を追加
3. 再生成

### 問題: フォント生成が遅い・固まる

**原因:** 漢字の数が多すぎてメモリ不足

**解決策:**
1. **Atlas Resolution** を小さくする（例：`2048 x 2048`）
2. Character Set を `Characters from File` に変更し、必要最小限の漢字のみ生成

### 問題: フォントがぼやける

**原因:** Atlas Resolution が小さすぎる

**解決策:**
1. Font Asset Creator で **Atlas Resolution** を `4096 x 4096` 以上に設定
2. 再生成

---

これで日本語フォントの設定は完了です！🎉
