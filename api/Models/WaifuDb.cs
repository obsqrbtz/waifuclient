using System;
using System.Collections.Generic;
using LiteDB;

namespace api.Models;

public class WaifuDb : IDisposable
{
    public LiteDatabase Db;
    private ILiteCollection<WaifuDbEntry> _collection;
    private string _path = "waifu.db";
    private string _name = "Waifu";
    private bool _disposed = false;
    public WaifuDb()
    {
        Db = new LiteDatabase(_path);
        _collection = Db.GetCollection<WaifuDbEntry>(_name);
    }
    public bool EntryExists(string url)
    {
        WaifuDbEntry? existingItems = _collection.FindOne(x => x.Url == url);
        return existingItems is not null;
    }
    public void Add(string url, string type, string category)
    {
        if (!EntryExists(url))
        {
            WaifuDbEntry waifu = new()
            { 
                Url = url,
                Type = type,
                Category = category
            };
            _ = _collection.Insert(waifu);
            _ = _collection.EnsureIndex(x => x.Url);
        }
    }
    public IEnumerable<WaifuDbEntry> FetchAll() => _collection.FindAll();
    public IEnumerable<WaifuDbEntry> FetchLiked() => _collection.Find(x => x.Liked);

    public void Update(IEnumerable<WaifuDbEntry> entries)
    {
        foreach(var entry in entries)
        {
            _ = _collection.Update(entry);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Db.Dispose();
            }              
            _disposed = true;
        }
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    ~WaifuDb()
    {
        Dispose(false);
    }
}
public class WaifuDbEntry
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Type {get; set;} = string.Empty;
    public string Category {get; set;} = string.Empty;
    public bool Liked { get; set; }
    public override string ToString() => $"{Id} | {Url} | {Type} | {Category}";
}