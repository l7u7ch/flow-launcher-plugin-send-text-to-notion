# 仕様書 — Flow.Launcher.Plugin.Notion

---

## 1. 概要

Flow Launcher 上から1コマンドで Notion データベースにメモを送信するプラグイン。
ユーザーはアクションキーワードに続けてテキストを入力し、Enter を押すだけでメモを記録できる。

---

## 2. 動作環境

| 項目          | 要件                                            |
| ------------- | ----------------------------------------------- |
| OS            | Windows 10 / 11 (x64)                           |
| Flow Launcher | 1.19 以降（net9.0 対応版）                      |
| .NET Runtime  | .NET 9.0 (Flow Launcher 同梱)                   |
| Notion        | ワークスペース管理者または Integration 作成権限 |

---

## 3. ファイル構成

```
Flow.Launcher.Plugin.Notion.csproj   # プロジェクト定義（リポジトリルート）
src/
├── plugin.json                          # プラグインマニフェスト
├── Main.cs                              # エントリーポイント (IAsyncPlugin, ISettingProvider)
├── NotionClient.cs                      # Notion API クライアント
├── Settings.cs                          # 設定 POCO
├── SettingsControl.xaml                 # 設定 UI (WPF)
├── SettingsControl.xaml.cs              # 設定 UI コードビハインド
└── Images/
    └── notion.png                       # プラグインアイコン (64×64 px)
```

---

## 4. 機能仕様

### 4.1 メモ送信

| 項目                 | 仕様                                                          |
| -------------------- | ------------------------------------------------------------- |
| アクションキーワード | `*`（グローバル。plugin.json で変更可）                       |
| 入力形式             | `<メモテキスト>`                                              |
| 送信先               | 設定済みの Notion データベース（1件固定）                     |
| ページ構造           | タイトルプロパティのみ。本文なし。                            |
| 送信タイミング       | Enter 押下時（`AsyncAction` 内で HTTP 呼び出し）              |
| 成功時               | Flow Launcher の `ShowMsg` で通知を表示し、ウィンドウを閉じる |
| 失敗時               | エラー内容を `ShowMsg` で表示し、ウィンドウを開いたまま保つ   |

### 4.2 クエリ状態ごとの表示

| 状態                                          | 表示内容                                                                          |
| --------------------------------------------- | --------------------------------------------------------------------------------- |
| 設定未完了（ApiToken または DatabaseId が空） | 「設定が必要です」→ クリックで設定画面を開く                                      |
| テキスト未入力（空クエリ）                    | 「メモのテキストを入力して Enter で送信」ヒントを表示（クリックしても何もしない） |
| テキスト入力済み                              | 「Notion に送信: "…"」と Database ID の先頭8桁プレビューを表示                    |

### 4.3 設定項目

| 設定名            | 型     | デフォルト | UI 部品     | 説明                                                               |
| ----------------- | ------ | ---------- | ----------- | ------------------------------------------------------------------ |
| ApiToken          | string | `""`       | PasswordBox | Notion Internal Integration のトークン (`secret_...`)              |
| DatabaseId        | string | `""`       | TextBox     | 送信先データベースの ID（32桁 hex、ハイフンあり/なし両対応）       |
| TitlePropertyName | string | `"Name"`   | TextBox     | データベースのタイトル列名（空欄のとき `"Name"` にフォールバック） |

設定は Flow Launcher が `%APPDATA%\FlowLauncher\Settings\Plugins\Flow.Launcher.Plugin.Notion\Settings.json` に JSON 形式で自動保存する。

---

## 5. Notion API 仕様

### エンドポイント

```
POST https://api.notion.com/v1/pages
```

### リクエストヘッダー

```
Authorization: Bearer <ApiToken>
Notion-Version: 2022-06-28
Content-Type: application/json
```

### リクエストボディ

DatabaseId のハイフンは送信前に自動除去する。

```json
{
  "parent": {
    "database_id": "<DatabaseId（ハイフンなし）>"
  },
  "properties": {
    "<TitlePropertyName>": {
      "title": [{ "text": { "content": "<メモテキスト>" } }]
    }
  }
}
```

### レスポンス

| HTTP ステータス | 意味                                                                  |
| --------------- | --------------------------------------------------------------------- |
| 2xx             | 送信成功（`IsSuccessStatusCode` が true の場合）                      |
| 400             | リクエスト不正（プロパティ名の誤りなど）                              |
| 401             | 認証失敗（トークン誤り）                                              |
| 404             | データベースが見つからない（ID 誤り、またはインテグレーション未接続） |

エラー時は `HTTP {ステータスコード}: {レスポンスボディ}` の文字列を返す。
キャンセル時は `"Request cancelled."` を返す。
ネットワークエラー時は `HttpRequestException.Message` を返す。

---

## 6. クラス設計

### `Main` (src/Main.cs)

Flow Launcher プラグインのエントリーポイント。

| メンバー                   | 種別             | 説明                                                                                    |
| -------------------------- | ---------------- | --------------------------------------------------------------------------------------- |
| `_context`                 | field            | `PluginInitContext`（`InitAsync` で設定）           |
| `_settings`                | field            | `Settings`（`InitAsync` で設定）                    |
| `_notionClient`            | field            | `NotionClient`（コンストラクタで初期化）            |
| `InitAsync(context)`       | IAsyncPlugin     | `LoadSettingJsonStorage<Settings>()` で設定をロード |
| `QueryAsync(query, token)` | IAsyncPlugin     | 入力状態を判定し `List<Result>` を返す              |
| `CreateSettingPanel()`     | ISettingProvider | `SettingsControl(_settings)` を返す                 |

### `NotionClient` (src/NotionClient.cs)

Notion API との通信を担当。

| メンバー             | 種別              | 説明                                                                                 |
| -------------------- | ----------------- | ------------------------------------------------------------------------------------ |
| `_http`              | static HttpClient | ソケット枯渇防止のためスタティック                                                   |
| `SendMemoAsync(...)` | async Task        | API 呼び出し。`CancellationToken` 対応。`(bool Success, string ErrorMessage)` を返す |
| `BuildBody(...)`     | private static    | JSON ボディ用の匿名オブジェクトを構築                                                |

### `Settings` (src/Settings.cs)

設定値の POCO。Flow Launcher が JSON シリアライズ/デシリアライズを管理。

### `SettingsControl` (src/SettingsControl.xaml, src/SettingsControl.xaml.cs)

WPF UserControl。コンストラクタで `Settings` の参照を受け取り、`Loaded` イベントで各コントロールに値を反映する。各コントロールの変更イベントで `Settings` を直接書き換える。Flow Launcher が設定ウィンドウを閉じた時点で自動保存。

---

## 7. ビルド & インストール

### ビルド

```powershell
# リポジトリルートで実行
dotnet build -c Release
```

出力先: `bin\Release\net9.0-windows\`

### インストール

```powershell
$src  = ".\bin\Release\net9.0-windows"
$dest = "$env:APPDATA\FlowLauncher\Plugins\Notion-1.0.0"
Copy-Item -Recurse -Force $src $dest
```

Flow Launcher を再起動して Settings > Plugins に表示されることを確認。

---

## 8. Notion 側の事前設定

1. Notion の **Settings > My connections > Develop or manage integrations** で **Internal Integration** を作成し、API トークンを取得
2. 送信先データベースのページを開き「**…**」→「**Connect to**」でインテグレーションを接続
3. データベース URL から **Database ID** を取得
   例: `notion.so/workspace/{database-id}?v=...`
4. データベースのタイトル列の名前を確認（デフォルト: `Name`）

---

## 9. エラー対処

| エラー                      | 原因                                             | 対処                                              |
| --------------------------- | ------------------------------------------------ | ------------------------------------------------- |
| HTTP 401                    | トークンが無効                                   | Notion で Integration トークンを再確認            |
| HTTP 404 `object_not_found` | Database ID 誤り、またはインテグレーション未接続 | データベースに Integration を「Connect to」で接続 |
| HTTP 400 `validation_error` | タイトルプロパティ名が違う                       | 設定の「タイトルプロパティ名」を実際の列名に変更  |
| ネットワークエラー          | オフライン、プロキシ等                           | 接続を確認                                        |

---

## 10. 既知の制限

- 送信先データベースは1件のみ（複数切り替え非対応）
- タイトルプロパティ以外への書き込み非対応
- オフライン時の下書き保存機能なし
- Notion OAuth（ユーザートークン）非対応、Internal Integration のみ
