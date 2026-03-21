using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public sealed class NotionClient
{
    private static readonly HttpClient _http = new();
    private const string NotionVersion = "2022-06-28";
    private const string PagesUrl = "https://api.notion.com/v1/pages";

    public async Task<(bool Success, string ErrorMessage)> SendMemoAsync(
        string apiToken,
        string databaseId,
        string memoText,
        string titlePropertyName,
        CancellationToken cancellationToken = default
    )
    {
        // Strip hyphens from database ID if present
        var dbId = databaseId.Replace("-", "");

        var body = BuildBody(dbId, memoText, titlePropertyName);
        var json = JsonSerializer.Serialize(body);

        using var request = new HttpRequestMessage(HttpMethod.Post, PagesUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
        request.Headers.Add("Notion-Version", NotionVersion);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            using var response = await _http.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
                return (true, string.Empty);

            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            return (false, $"HTTP {(int)response.StatusCode}: {errorBody}");
        }
        catch (OperationCanceledException)
        {
            return (false, "Request cancelled.");
        }
        catch (HttpRequestException ex)
        {
            return (false, ex.Message);
        }
    }

    private static object BuildBody(string databaseId, string memoText, string titlePropertyName)
    {
        var titleContent = new[] { new { text = new { content = memoText } } };

        var properties = new Dictionary<string, object>
        {
            [titlePropertyName] = new { title = titleContent },
        };

        return new { parent = new { database_id = databaseId }, properties };
    }
}
