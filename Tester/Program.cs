using api.Models;
using LiteDB;

class Program
{
    private static ApiWrapper? _apiWrapper;
    private static WaifuDb _db;
    public static void Main()
    {
        _apiWrapper = new();
        _db = new();
        string type = "sfw";
        string category = "waifu";
        for (int i = 0; i < 10; i++)
        {
            NextWaifu().Wait();
            if (!string.IsNullOrEmpty(_apiWrapper.Url) && !_db.EntryExists(_apiWrapper.Url))
            {
                _db.Add(_apiWrapper.Url, type, category);
            }
        }
        var items = _db.FetchAll();
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }
    public static async Task<Stream?> NextWaifu()
    {
        if (_apiWrapper is null)
            return null;
        return await _apiWrapper.GetWaifu("sfw", "waifu");
    }
}
