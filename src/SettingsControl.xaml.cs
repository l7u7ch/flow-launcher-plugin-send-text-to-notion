using System.Windows;
using System.Windows.Controls;
using iNKORE.UI.WPF.Modern.Common;
using iNKORE.UI.WPF.Modern.Controls.Helpers;

public partial class SettingsControl : UserControl
{
    private readonly Settings _settings;

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
        TitlePropertyNameBox.Text = _settings.TitlePropertyName == "Name" ? string.Empty : _settings.TitlePropertyName;
        UpdatePlaceholders();
    }

    private void ApiTokenBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        _settings.ApiToken = ApiTokenBox.Password;
    }

    private void ShowApiTokenCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        PasswordBoxHelper.SetPasswordRevealMode(
            ApiTokenBox,
            ShowApiTokenCheckBox.IsChecked == true
                ? PasswordRevealMode.Visible
                : PasswordRevealMode.Hidden
        );
    }

    private void DatabaseIdBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _settings.DatabaseId = DatabaseIdBox.Text.Trim();
        DatabaseIdPlaceholder.Visibility = string.IsNullOrEmpty(DatabaseIdBox.Text)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void TitlePropertyNameBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var name = TitlePropertyNameBox.Text.Trim();
        _settings.TitlePropertyName = string.IsNullOrEmpty(name) ? "Name" : name;
        TitlePropertyNamePlaceholder.Visibility = string.IsNullOrEmpty(TitlePropertyNameBox.Text)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void UpdatePlaceholders()
    {
        DatabaseIdPlaceholder.Visibility = string.IsNullOrEmpty(DatabaseIdBox.Text)
            ? Visibility.Visible
            : Visibility.Collapsed;
        TitlePropertyNamePlaceholder.Visibility = string.IsNullOrEmpty(TitlePropertyNameBox.Text)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
