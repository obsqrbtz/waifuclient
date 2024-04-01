using System;
using System.Collections.Generic;
using LiteDB;

namespace api.Models;

public class WaifuDb
{
    public LiteDatabase Db;
    private ILiteCollection<WaifuDbEntry> _collection;
    public WaifuDb()
    {
        Db = new LiteDatabase("waifu.db");
        _collection = Db.GetCollection<WaifuDbEntry>("Waifu");
    }
    public bool EntryExists(string url)
    {
        var existingItems = _collection.FindOne(x => x.Url == url);
        return existingItems is not null;
    }
    public void Add(string url, string type, string category)
    {
        if (!EntryExists(url))
        {
            var waifu = new WaifuDbEntry
            { 
                Url = url,
                Type = type,
                Category = category
            };
            _collection.Insert(waifu);
            _collection.EnsureIndex(x => x.Url);
        }
    }
    public IEnumerable<WaifuDbEntry> FetchAll()
    {
        return _collection.FindAll();
    }
}
public class WaifuDbEntry
{
    public int Id { get; set; }
    public string Url {get; set;}
    public string Type {get; set;}
    public string Category {get; set;}
    public override string ToString()
    {
        return $"{Id} | {Url} | {Type} | {Category}";
    }
}