using System.IO;
using System.Text.Json;

/// <summary>
/// %APPDATA%\NotionJot\config.json にトークン等の設定を永続化するクラス。
/// Flow Launcher の設定ストレージとは独立して動作するため、
/// テキストエディタから直接編集したり CLI ツールと設定を共有したりできる。
/// </summary>
public class ConfigStore
{
    private static readonly string DefaultDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "NotionJot"
    );

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    public string FilePath { get; }

    public ConfigStore(string? directory = null)
    {
        var dir = directory ?? DefaultDir;
        Directory.CreateDirectory(dir);
        FilePath = Path.Combine(dir, "config.json");
    }

    /// <summary>設定ファイルが存在するかどうかを返す。</summary>
    public bool Exists => File.Exists(FilePath);

    /// <summary>
    /// ファイルが存在すれば読み込み、存在しなければデフォルト値の Settings を返す。
    /// </summary>
    public Settings Load()
    {
        if (!Exists)
            return new Settings();

        try
        {
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<Settings>(json, JsonOptions) ?? new Settings();
        }
        catch
        {
            // 破損ファイル等の場合はデフォルト値にフォールバック
            return new Settings();
        }
    }

    /// <summary>設定をファイルに書き出す。</summary>
    public void Save(Settings settings)
    {
        var json = JsonSerializer.Serialize(settings, JsonOptions);
        File.WriteAllText(FilePath, json);
    }

    /// <summary>
    /// ローカルファイルの値を source にマージする。
    /// source 側に値がある場合はそちらを優先し、空の場合のみファイルの値で補完する。
    /// </summary>
    public static void MergeInto(Settings source, Settings fileValues)
    {
        if (string.IsNullOrWhiteSpace(source.ApiToken))
            source.ApiToken = fileValues.ApiToken;

        if (string.IsNullOrWhiteSpace(source.DatabaseId))
            source.DatabaseId = fileValues.DatabaseId;

        if (string.IsNullOrWhiteSpace(source.TitlePropertyName) || source.TitlePropertyName == "Name")
            if (!string.IsNullOrWhiteSpace(fileValues.TitlePropertyName))
                source.TitlePropertyName = fileValues.TitlePropertyName;
    }
}
