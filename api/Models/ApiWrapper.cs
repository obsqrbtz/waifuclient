using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace api.Models;

public class Waifu
{
    public string url { get; set; } = string.Empty;
}
public class ApiWrapper
{
    private static HttpClient? _httpClient;
    private Waifu? _responseWaifu;
    private const string _apiUrl = "https://api.waifu.pics/";
    public string? ProxyUrl = string.Empty; // "http://192.168.6.4:3128";
    public string? Url => _responseWaifu is not null 
        ? _responseWaifu.url
        : null;
    public Stream? WaifuStream { get; private set; }
    public static List<string> Types { get; set; } = ["sfw", "nsfw"];
    public static List<string> SfwCategories { get; } =
    [
        "waifu",
                "neko",
                "shinobu",
                "megumin",
                "bully",
                "cuddle",
                "cry",
                "hug",
                "awoo",
                "kiss",
                "lick",
                "pat",
                "smug",
                "bonk",
                "yeet",
                "blush",
                "smile",
                "wave",
                "highfive",
                "handhold",
                "nom",
                "bite",
                "glomp",
                "slap",
                "kill",
                "kick",
                "happy",
                "wink",
                "poke",
                "dance",
                "cringe"
    ];
    public static List<string> NsfwCategories { get; } =
    [
        "waifu",
            "neko",
            "trap",
            "blowjob"
    ];
    public ApiWrapper()
    {
        if (!string.IsNullOrEmpty(ProxyUrl))
        {
            WebProxy proxy = new()
            {
                Address = new Uri(ProxyUrl),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,
            };

            HttpClientHandler httpClientHandler = new()
            {
                Proxy = proxy,
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            _httpClient = new HttpClient(handler: httpClientHandler, disposeHandler: true);
        }
        else
        {
            _httpClient = new();
        }
    }
    public async Task<Stream?> GetWaifu(string type, string category)
    {
        if (_httpClient is null)
            return null;
        string url = $"{_apiUrl}{type}/{category}";
        string response = await _httpClient.GetStringAsync(url);
        _responseWaifu = JsonSerializer.Deserialize<Waifu>(response);
        if (_responseWaifu is null)
            return null;
        return await DownloadImage(_responseWaifu.url);
    }
    private async Task<Stream?> DownloadImage(string url)
    {
        try
        {
            if (_httpClient is null)
                return null;
            byte[] imgBuff = await _httpClient.GetByteArrayAsync(url);
            WaifuStream = new MemoryStream(imgBuff);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            WaifuStream = null;
        }
        return WaifuStream;
    }
}