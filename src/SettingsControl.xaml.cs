using System.Windows;
using System.Windows.Controls;

public partial class SettingsControl : UserControl
{
    private readonly Settings _settings;
    private bool _isApiTokenVisible;

    public SettingsControl(Settings settings)
    {
        _settings = settings;
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ApiTokenBox.Password = _settings.ApiToken;
        VisibleApiTokenBox.Text = _settings.ApiToken;
        DatabaseIdBox.Text = _settings.DatabaseId;
        TitlePropertyNameBox.Text = _settings.TitlePropertyName == "Name" ? string.Empty : _settings.TitlePropertyName;
        UpdatePlaceholders();
    }

    private void ApiTokenBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        _settings.ApiToken = ApiTokenBox.Password;
        UpdateApiTokenPlaceholder();
    }

    private void VisibleApiTokenBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _settings.ApiToken = VisibleApiTokenBox.Text;
        UpdateApiTokenPlaceholder();
    }

    private void ToggleApiTokenVisibilityButton_Click(object sender, RoutedEventArgs e)
    {
        _isApiTokenVisible = !_isApiTokenVisible;

        if (_isApiTokenVisible)
        {
            VisibleApiTokenBox.Text = ApiTokenBox.Password;
            ApiTokenBox.Visibility = Visibility.Collapsed;
            VisibleApiTokenBox.Visibility = Visibility.Visible;
            ToggleApiTokenVisibilityButton.ToolTip = "Hide token";
            UpdateApiTokenPlaceholder();
            VisibleApiTokenBox.Focus();
            return;
        }

        ApiTokenBox.Password = VisibleApiTokenBox.Text;
        VisibleApiTokenBox.Visibility = Visibility.Collapsed;
        ApiTokenBox.Visibility = Visibility.Visible;
        ToggleApiTokenVisibilityButton.ToolTip = "Show token";
        UpdateApiTokenPlaceholder();
        ApiTokenBox.Focus();
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
        UpdateApiTokenPlaceholder();
        DatabaseIdPlaceholder.Visibility = string.IsNullOrEmpty(DatabaseIdBox.Text)
            ? Visibility.Visible
            : Visibility.Collapsed;
        TitlePropertyNamePlaceholder.Visibility = string.IsNullOrEmpty(TitlePropertyNameBox.Text)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void UpdateApiTokenPlaceholder()
    {
        var token = _isApiTokenVisible ? VisibleApiTokenBox.Text : ApiTokenBox.Password;
        ApiTokenPlaceholder.Visibility = string.IsNullOrEmpty(token)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
