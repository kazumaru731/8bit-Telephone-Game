# 漢字フリップゲーム - セットアップガイド

## プロジェクト概要

小学1、2年生で習う漢字を使った伝言ゲーム風のオンライン対戦ゲームです。

- **出題者**: 漢字8文字以内でフリップを作成し、お題を表現
- **回答者**: フリップを見てお題を当てる
- **対応プラットフォーム**: PC、iOS、Android（計画）

## 現在の実装状況

✅ **フェーズ1: ローカルプロトタイプ（スクリプト実装完了）**

### 完成した機能

- 漢字データベース（小学1、2年生の240字）
- 漢字入力検証（8文字制限、許可された漢字のみ）
- 漢字操作機能
  - ドラッグ&ドロップ
  - サイズ変更
  - 回転（右クリック）
- ゲーム状態管理
- UI制御スクリプト（出題者用、回答者用）

### 未完了の作業

🔲 Unity エディタでのUI構築（次のステップ）
🔲 プレハブ作成
🔲 TextMeshPro セットアップ
🔲 日本語フォント設定

---

## プロジェクト構造

```
Assets/
├── Scenes/
│   └── GameScene.unity              # メインゲームシーン
├── Scripts/
│   ├── Core/                        # コアシステム
│   │   ├── PlayerRole.cs           # プレイヤー役割定義
│   │   └── GameManager.cs          # ゲーム状態管理
│   ├── Kanji/                       # 漢字システム
│   │   ├── KanjiDatabase.cs        # 漢字データベース
│   │   ├── KanjiInputValidator.cs  # 入力検証
│   │   ├── KanjiElement.cs         # 漢字要素（ドラッグ、回転など）
│   │   └── KanjiFlipper.cs         # フリップ管理
│   └── UI/                          # UIシステム
│       ├── MainMenuUI.cs           # メインメニュー
│       ├── QuestionerUI.cs         # 出題者UI
│       └── AnswererUI.cs           # 回答者UI
├── Resources/
│   └── KanjiData/
│       ├── Grade1Kanji.txt         # 小学1年生の漢字（80字）
│       └── Grade2Kanji.txt         # 小学2年生の漢字（160字）
└── Prefabs/                         # プレハブ（未作成）
```

---

## 次のステップ

### 1. TextMeshPro のインポート

Unity エディタで：
1. `Window > TextMeshPro > Import TMP Essential Resources`
2. 日本語フォント（例：Noto Sans JP）をインポート
3. Font Asset Creator で日本語フォントアセットを作成

### 2. UI要素の構築

Unity エディタ上で以下のパネルとUI要素を作成：

- メインメニューパネル
- 出題者用パネル
- 回答者用パネル
- 待機パネル
- 結果パネル

### 3. KanjiElementプレハブの作成

フリップ上の漢字要素のプレハブを作成：
- TextMeshProUGUI コンポーネント
- サイズ変更ボタン
- KanjiElement スクリプト

### 4. スクリプト参照の設定

各UIスクリプトのInspectorで、UI要素への参照を設定

### 5. テスト実行

Unity エディタでGameSceneを再生し、動作確認

---

## 将来の実装（フェーズ2以降）

- Photon PUN 2 の統合
- オンラインマルチプレイヤー機能
- フレンド対戦/ランダムマッチング
- モバイル対応（iOS、Android）

---

## 詳細ドキュメント

- [実装計画](file:///C:/Users/user/.gemini/antigravity/brain/fc93d8e8-1769-4849-95f4-f86704aad7c2/implementation_plan.md)
- [タスクリスト](file:///C:/Users/user/.gemini/antigravity/brain/fc93d8e8-1769-4849-95f4-f86704aad7c2/task.md)
- [実装完了レポート](file:///C:/Users/user/.gemini/antigravity/brain/fc93d8e8-1769-4849-95f4-f86704aad7c2/walkthrough.md)
