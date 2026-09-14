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

GitHub Actions の **Release > Run workflow** を `main` ブランチから実行し、`patch`、`minor`、`major` のいずれかを選択します。ワークフローは最新の `vX.Y.Z` タグを基準に Semantic Versioning の番号を繰り上げます。

ワークフローは `src/plugin.json` の `Version` が最新タグと一致することを確認してから、次のバージョンを書き込み、Windows 上で依存関係の復元、Release ビルド、ZIP 作成を行います。成功後にのみ `main` へ `chore(release): vX.Y.Z` をコミットし、注釈付きの `vX.Y.Z` タグを push して、新しい GitHub Release に ZIP を添付します。既存タグや Release の上書きは行いません。

## Flow Launcher Plugin Store

Plugin Store への初回掲載では、`src/plugin.json` の `ID`、`Name`、`Description`、`Author`、`Version`、`Language`、`Website` を使い、配布 ZIP、ソース、アイコン CDN の URL を指定した Manifest を Flow Launcher の Plugins Manifest リポジトリへ提出します。掲載 PR の作成と送信は、このリポジトリの Release 公開後に別途実行します。
