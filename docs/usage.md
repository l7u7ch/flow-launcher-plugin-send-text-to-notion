# セットアップと使い方

## 必要な環境

- Windows 10 または 11（x64）
- Flow Launcher 1.19 以降（.NET 9.0 対応版）
- Notion の Internal Integration を作成できる権限

## インストール

1. GitHub Release から `Flow.Launcher.Plugin.Notion.zip` を取得します。
2. ZIP を `%APPDATA%\FlowLauncher\Plugins` 配下の任意のフォルダーへ展開します。
3. 展開先の直下に `plugin.json` があることを確認します。
4. Flow Launcher を再起動します。

ソースから導入する場合は、[開発とリリース](development.md#ローカルインストール)を参照してください。

## Notion の準備

1. Notion の **Settings > My connections > Develop or manage integrations** で Internal Integration を作成し、API トークンを取得します。
2. 送信先データベースの **… > Connect to** から、その Integration を接続します。
3. データベース URL から Database ID を取得します。
4. データベースのタイトルプロパティ名を確認します。

## プラグインの設定

Flow Launcher の **Settings > Plugins > Notion** を開き、次の値を入力します。

- Notion Internal Integration の API Token
- 送信先の Database ID
- 書き込み先のタイトルプロパティ名

設定値の正確な取り扱いは、[設定仕様](specification.md#設定)を参照してください。

## メモの送信

1. Flow Launcher を開きます。
2. メモを入力します。
3. 表示された Notion の送信候補を選択し、Enter を押します。

既定のアクションキーワードは `*`（グローバル）です。送信するページにはタイトルだけが設定されます。

## トラブルシューティング

| 表示 | 確認すること |
| --- | --- |
| HTTP 400 | 設定したタイトルプロパティ名がデータベースと一致しているか |
| HTTP 401 | Integration の API Token が正しいか |
| HTTP 404 | Database ID が正しいか、データベースに Integration を接続したか |
| ネットワークエラー | ネットワーク接続やプロキシ設定に問題がないか |

対応範囲は[機能仕様の制限](specification.md#制限)を参照してください。
