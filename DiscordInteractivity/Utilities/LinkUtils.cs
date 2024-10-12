namespace DiscordInteractivity.Utilities;

public static class LinkUtils
{
    public static string GetWebsiteName(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            uri = new UriBuilder(url).Uri;
        }

        return uri.Host;
    }

    public static async Task<bool> WebsiteExists(
        this HttpClient httpClient,
        string url,
        int timeout = 3000
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Head, url);

        var cancellationToken = new CancellationTokenSource(timeout).Token;

        try
        {
            var response = await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken
            );
            response.EnsureSuccessStatusCode();
        }
        catch
        {
            return false;
        }

        return true;
    }
}
