# Send Text to Notion

[English](README.md) | 日本語

Flow Launcher から入力したテキストを、指定した Notion データベースへ新しいページとして送信するプラグインです。

## 1. 必要環境

- Windows 10 または 11（x64）
- Flow Launcher 1.19 以降
- Notion の Internal Integration を作成できる権限

## 2. インストール

1. [GitHub Releases](https://github.com/l7u7ch/flow-launcher-plugin-send-text-to-notion/releases) から最新の `Flow.Launcher.Plugin.SendTextToNotion.zip` を取得します。
2. ZIP を `%APPDATA%\FlowLauncher\Plugins` 配下の任意のフォルダーへ展開します。
3. 展開先の直下に `plugin.json` があることを確認します。
4. Flow Launcher を再起動します。

## 3. Notion の準備

1. Notion の **Settings > My connections > Develop or manage integrations** で Internal Integration を作成し、API トークンを取得します。
2. 送信先データベースの **… > Connect to** から、その Integration を接続します。
3. データベース URL から Database ID を取得します。
4. データベースのタイトルプロパティ名を確認します。

## 4. プラグインの設定

Flow Launcher の **Settings > Plugins > Send Text to Notion** を開き、次の値を入力します。

- Notion Internal Integration の API Token
- 送信先の Database ID
- 書き込み先のタイトルプロパティ名

## 5. 使い方

1. Flow Launcher を開きます。
2. 送信したいテキストを入力します。
3. 表示された Notion の送信候補を選択し、Enter を押します。

既定のアクションキーワードは `*`（グローバル）です。
