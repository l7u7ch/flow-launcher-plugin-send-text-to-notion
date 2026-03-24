using System.Windows;
using System.Windows.Controls;

public partial class SettingsControl : UserControl
{
    private readonly Settings _settings;
    private readonly ConfigStore _configStore = new();

    public SettingsControl(Settings settings)
    {
        _settings = settings;
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ApiTokenBox.Password = _settings.ApiToken;
        DatabaseIdBox.Text = _settings.DatabaseId;
        TitlePropertyNameBox.Text = _settings.TitlePropertyName;
    }

    private void ApiTokenBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        _settings.ApiToken = ApiTokenBox.Password;
        _configStore.Save(_settings);
    }

    private void DatabaseIdBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _settings.DatabaseId = DatabaseIdBox.Text.Trim();
        _configStore.Save(_settings);
    }

    private void TitlePropertyNameBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var name = TitlePropertyNameBox.Text.Trim();
        _settings.TitlePropertyName = string.IsNullOrEmpty(name) ? "Name" : name;
        _configStore.Save(_settings);
    }
}
