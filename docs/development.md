# 開発とリリース

## 実装境界

| 責務 | 実装 |
| --- | --- |
| Flow Launcher との連携とクエリ状態 | `src/Main.cs` |
| Notion Pages API 通信 | `src/NotionClient.cs` |
| 設定モデルと設定 UI | `src/Settings.cs`、`src/SettingsControl.xaml`、`src/SettingsControl.xaml.cs` |
| プラグイン定義 | `src/plugin.json` |
| ビルド定義 | `flow-launcher-plugin-send-text-to-notion.csproj` |
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
$dest = "$env:APPDATA\FlowLauncher\Plugins\flow-launcher-plugin-send-text-to-notion-1.0.0"
Copy-Item -Recurse -Force $src $dest
```

コピー後に Flow Launcher を再起動し、**Settings > Plugins** に Send Text to Notion が表示されることを確認します。

## リリース

GitHub Actions の **Release > Run workflow** を実行すると、Windows 上で依存関係の復元と Release ビルドを行い、`flow-launcher-plugin-send-text-to-notion.zip` を GitHub Release に添付します。

リリースタグは `build-{commit SHA}`、タイトルはコミット SHA の先頭7文字を含む `Build {short SHA}` です。同一コミットで再実行した場合は、既存リリースの ZIP を更新します。
