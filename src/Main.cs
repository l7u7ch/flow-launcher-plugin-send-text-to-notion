using System.Windows.Controls;
using Flow.Launcher.Plugin;

public sealed class Main : IAsyncPlugin, ISettingProvider
{
    private PluginInitContext _context = null!;
    private Settings _settings = null!;
    private readonly NotionClient _notionClient = new();

    private const string IconPath = "Images\\notion.png";

    // -------------------------------------------------------------------------
    // IAsyncPlugin
    // -------------------------------------------------------------------------

    public Task InitAsync(PluginInitContext context)
    {
        _context = context;
        _settings = context.API.LoadSettingJsonStorage<Settings>();
        return Task.CompletedTask;
    }

    public Task<List<Result>> QueryAsync(Query query, CancellationToken token)
    {
        // Settings not yet configured
        if (
            string.IsNullOrWhiteSpace(_settings.ApiToken)
            || string.IsNullOrWhiteSpace(_settings.DatabaseId)
        )
        {
            return Task.FromResult(
                new List<Result>
                {
                    new()
                    {
                        Title = "Notion Jot: 設定が必要です",
                        SubTitle =
                            "Settings > Plugins > Notion Jot で API トークンと Database ID を設定してください",
                        IcoPath = IconPath,
                        Score = 100,
                        Action = _ =>
                        {
                            _context.API.OpenSettingDialog();
                            return true;
                        },
                    },
                }
            );
        }

        var memoText = query.Search.Trim();

        // Empty query: usage hint
        if (string.IsNullOrEmpty(memoText))
        {
            return Task.FromResult(
                new List<Result>
                {
                    new()
                    {
                        Title = "Notion Jot",
                        SubTitle = "メモのテキストを入力して Enter で送信",
                        IcoPath = IconPath,
                        Score = 100,
                        Action = _ => false,
                    },
                }
            );
        }

        // Has text: offer to send
        var dbPreview =
            _settings.DatabaseId.Length > 8
                ? _settings.DatabaseId[..8] + "..."
                : _settings.DatabaseId;

        return Task.FromResult(
            new List<Result>
            {
                new()
                {
                    Title = $"Notion に送信: \"{memoText}\"",
                    SubTitle = $"Database: {dbPreview}",
                    IcoPath = IconPath,
                    Score = 100,
                    AsyncAction = async _ =>
                    {
                        var (success, error) = await _notionClient.SendMemoAsync(
                            _settings.ApiToken,
                            _settings.DatabaseId,
                            memoText,
                            _settings.TitlePropertyName
                        );

                        if (success)
                        {
                            _context.API.ShowMsg(
                                "Notion Jot 送信完了",
                                $"\"{memoText}\"",
                                IconPath
                            );
                            return true;
                        }
                        else
                        {
                            _context.API.ShowMsg("送信に失敗しました", error, IconPath);
                            return false;
                        }
                    },
                },
            }
        );
    }

    // -------------------------------------------------------------------------
    // ISettingProvider
    // -------------------------------------------------------------------------

    public Control CreateSettingPanel() => new SettingsControl(_settings);
}
