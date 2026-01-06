# 漢字連想ゲーム

小学校1・2年生で習う漢字を使った連想ゲームアプリです。

## ゲームの遊び方

1. **プレイヤー設定**
   - 2人以上でプレイします
   - 各プレイヤーの名前を登録します
   - ラウンド数を設定します（1〜10ラウンド）

2. **ゲーム進行**
   - 各ラウンドで、1人が「当てる人」になります
   - 残りの人は「当てさせる人」として、お題を見ます
   - 当てさせる人は、小学1・2年生で習う漢字（8文字以内）を使ってヒントを作成します
   - 漢字は自由な位置、大きさ、回転で配置できます
   - 当てる人がお題を当てます

3. **得点**
   - 当てた人：1ポイント
   - 当てさせた人：各2ポイント
   - 全ラウンド終了後、最高得点の人が勝ちです

## 技術スタック

- **フレームワーク**: React Native / Expo
- **言語**: TypeScript
- **ナビゲーション**: React Navigation
- **対応プラットフォーム**: iOS, Android, Web

## 開発環境のセットアップ

### 前提条件

- Node.js (v18以上)
- npm または yarn

### インストール

```bash
# 依存関係のインストール
npm install

# または
yarn install
```

### 開発サーバーの起動

```bash
# Expo Goアプリで開発
npm start

# iOSシミュレーター（Macのみ）
npm run ios

# Androidエミュレーター
npm run android

# Webブラウザ
npm run web
```

## iOSリリース方法

### 1. Expo Application Services (EAS) の設定

```bash
# EAS CLIのインストール
npm install -g eas-cli

# Expoアカウントでログイン
eas login

# プロジェクトの初期化
eas build:configure
```

### 2. app.jsonの設定

`app.json`の以下の項目を更新してください：

- `expo.ios.bundleIdentifier`: あなたの一意のBundle ID（例: `com.yourcompany.kanjigame`）
- `expo.extra.eas.projectId`: EASプロジェクトID

### 3. ビルド

```bash
# iOS用にビルド（App Store提出用）
eas build --platform ios

# テスト用（シミュレーター）
eas build --platform ios --profile development
```

### 4. App Storeへの提出

1. [App Store Connect](https://appstoreconnect.apple.com/)にログイン
2. 新しいアプリを作成
3. EASでビルドしたアプリをアップロード
4. スクリーンショット、説明文などを設定
5. 審査に提出

### 必要なもの

- Apple Developer Programへの登録（年間$99）
- macOS（ビルドにはmacOSが必要、またはEASクラウドビルドを使用）

## Android リリース方法

### 1. ビルド

```bash
# Android用にビルド（Google Play提出用）
eas build --platform android
```

### 2. Google Play Consoleへの提出

1. [Google Play Console](https://play.google.com/console)にログイン
2. 新しいアプリを作成
3. EASでビルドしたAAB/APKをアップロード
4. ストア掲載情報を設定
5. 審査に提出

### 必要なもの

- Google Play Console開発者登録（一度限り$25）

## プロジェクト構造

```
kanji-game/
├── App.tsx                 # メインアプリコンポーネント
├── src/
│   ├── screens/           # 画面コンポーネント
│   │   ├── HomeScreen.tsx
│   │   ├── PlayerSetupScreen.tsx
│   │   ├── GameScreen.tsx
│   │   └── ResultScreen.tsx
│   ├── components/        # 再利用可能なコンポーネント
│   │   ├── KanjiCanvas.tsx
│   │   ├── KanjiSelector.tsx
│   │   └── AnswerInput.tsx
│   ├── data/              # データファイル
│   │   ├── elementaryKanji.ts
│   │   └── topics.ts
│   ├── types/             # TypeScript型定義
│   │   └── index.ts
│   ├── navigation/        # ナビゲーション設定
│   │   └── types.ts
│   └── utils/             # ユーティリティ関数
├── assets/                # 画像、フォントなど
└── app.json              # Expo設定ファイル
```

## 使用している漢字

- 小学1年生で習う漢字：80字
- 小学2年生で習う漢字：160字
- 合計：240字

## ライセンス

MIT

## 開発者

あなたの名前

## サポート

問題や質問がある場合は、GitHubのIssuesでお知らせください。
