# 開発とリリース

## 実装境界

| 責務 | 実装 |
| --- | --- |
| Flow Launcher との連携とクエリ状態 | `src/Main.cs` |
| Notion Pages API 通信 | `src/NotionClient.cs` |
| 設定モデルと設定 UI | `src/Settings.cs`、`src/SettingsControl.xaml`、`src/SettingsControl.xaml.cs` |
| プラグイン定義 | `src/plugin.json` |
| ビルド定義 | `Flow.Launcher.Plugin.SendTextToNotion.csproj` |
| リリース自動化 | `.github/workflows/release.yml` |

利用者から見た動作と外部 API の契約は、[機能仕様](specification.md)を正本とします。

## ビルド

.NET 9 SDK を用意し、リポジトリルートで実行します。

```powershell
dotnet build -c Release
```

出力先は `bin\Release\net9.0-windows\` です。

## ローカルインストール

ビルド出力を Flow Launcher のプラグインフォルダーへコピーします。

```powershell
$src = ".\bin\Release\net9.0-windows"
$dest = "$env:APPDATA\FlowLauncher\Plugins\Flow.Launcher.Plugin.SendTextToNotion-1.0.0"
Copy-Item -Recurse -Force $src $dest
```

コピー後に Flow Launcher を再起動し、**Settings > Plugins** に Send Text to Notion が表示されることを確認します。

## リリース

GitHub Actions の **Release > Run workflow** を実行すると、Windows 上で依存関係の復元と Release ビルドを行い、`Flow.Launcher.Plugin.SendTextToNotion.zip` を GitHub Release に添付します。

リリースタグとタイトルは `src/plugin.json` の `Version` を正本として、`v{Version}` および `Send Text to Notion v{Version}` を使用します。公開版を更新する際は、Release 実行前に `Version` を更新してください。同じバージョンで再実行した場合は、既存 Release の ZIP を更新します。

## Flow Launcher Plugin Store

Plugin Store への初回掲載では、`src/plugin.json` の `ID`、`Name`、`Description`、`Author`、`Version`、`Language`、`Website` を使い、配布 ZIP、ソース、アイコン CDN の URL を指定した Manifest を Flow Launcher の Plugins Manifest リポジトリへ提出します。掲載 PR の作成と送信は、このリポジトリの Release 公開後に別途実行します。
