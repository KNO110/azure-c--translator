using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public sealed class TranslatorService
{
    private readonly string _key;
    private readonly string _endpoint;
    private readonly string _region;
    private static readonly HttpClient _http = new();

    public TranslatorService(IConfiguration cfg)
    {
        _key = cfg["Translator:Key"]!;
        _endpoint = cfg["Translator:Endpoint"]!.TrimEnd('/');
        _region = cfg["Translator:Region"]!;
    }

    public async Task<string> TranslateAsync(string text, string to, string? from = null)
    {
        var route = $"/translate?api-version=3.0&to={to}" +
                    (from is null ? string.Empty : $"&from={from}");

        using var req = new HttpRequestMessage(HttpMethod.Post, _endpoint + route);
        req.Headers.Add("Ocp-Apim-Subscription-Key", _key);
        req.Headers.Add("Ocp-Apim-Subscription-Region", _region);
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var body = JsonSerializer.Serialize(new[] { new { Text = text } });
        req.Content = new StringContent(body, Encoding.UTF8, "application/json");

        using var res = await _http.SendAsync(req);
        res.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        return doc.RootElement[0]
                  .GetProperty("translations")[0]
                  .GetProperty("text").GetString()!;
    }
}
