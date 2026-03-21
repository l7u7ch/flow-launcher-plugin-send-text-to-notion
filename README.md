# Notion Jot

[Flow Launcher](https://www.flowlauncher.com/) から1コマンドで Notion データベースにメモを送信するプラグイン。

## 使い方

Flow Launcher を開いてテキストを入力し、Enter を押すだけ。

```
<メモテキスト>
```

> アクションキーワードはデフォルトで `*`（グローバル）。`plugin.json` で変更可能。

## 動作環境

- Windows 10 / 11 (x64)
- Flow Launcher 1.19 以降（.NET 9.0 対応版）

## セットアップ

### 1. Notion インテグレーションの作成

1. [Notion Integrations](https://www.notion.so/my-integrations) で **Internal Integration** を作成し、API トークンを取得
2. 送信先データベースを開き「**…**」→「**Connect to**」でインテグレーションを接続

### 2. ビルド

```powershell
dotnet build -c Release
```

出力先: `bin\Release\net9.0-windows\`

### 3. インストール

```powershell
$src  = ".\bin\Release\net9.0-windows"
$dest = "$env:APPDATA\FlowLauncher\Plugins\NotionJot-1.0.0"
Copy-Item -Recurse -Force $src $dest
```

Flow Launcher を再起動し、Settings > Plugins に表示されることを確認。

### 4. プラグイン設定

| 設定名 | 説明 |
| --- | --- |
| API Token | Notion Internal Integration のトークン (`secret_...`) |
| Database ID | 送信先データベースの ID（ハイフンあり/なし両対応） |
| タイトルプロパティ名 | タイトル列の名前（デフォルト: `Name`） |

## トラブルシューティング

| エラー | 原因 | 対処 |
| --- | --- | --- |
| HTTP 401 | トークンが無効 | Integration トークンを再確認 |
| HTTP 404 | ID 誤り、またはインテグレーション未接続 | データベースに Integration を接続 |
| HTTP 400 | タイトルプロパティ名が違う | 設定の列名を実際の名前に変更 |

## 制限事項

- 送信先データベースは1件のみ
- タイトルプロパティ以外への書き込み非対応
- オフライン時の下書き保存機能なし
