using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
//using VJ1Core.Infrastructure.Main;

//name1space VJCore.Infrastructure;

public class CacheDataContainer<T, TRequest> where T : BaseObject where TRequest : new()
{
    private readonly ConcurrentDictionary<string, T> dictCache = new();

    public Func<DbCommand, TRequest, bool, List<T>> CacheEntityListFetcher { get; }
    public Func<T, string> PrimaryKeyValuesFormulator { get; }
    public string MasterTableName { get; }

    public CacheDataContainer(Func<DbCommand, TRequest, bool, List<T>> cacheEntityListFetcher,
        Func<T, string> primaryKeyValuesFormulator,
        string masterTableName)
    {
        CacheEntityListFetcher = cacheEntityListFetcher;
        PrimaryKeyValuesFormulator = primaryKeyValuesFormulator;
        MasterTableName = masterTableName;
    }

    public void InitializeAndPublishCacheData(DbCommand cmd)
    {
        InitializeCache(cmd);
        PublishCacheData(cmd);
    }

    public void InitializeCache(DbCommand cmd)
    {
        var lst = CacheEntityListFetcher(cmd, new TRequest(), false);
        dictCache.Clear();

        foreach (var e in lst)
        {
            string pkv = PrimaryKeyValuesFormulator(e);
            dictCache.TryAdd(pkv, e);
        }
    }

    public T GetEntity(string primaryKeyValues)
    {
        if (!dictCache.TryGetValue(primaryKeyValues, out var result)) return default;
        return result;
    }

    public List<T> GetEntities()
    {
        return dictCache.Values.ToList();
    }

    public void MergeIntoCache(T value)
    {
        string pkv = PrimaryKeyValuesFormulator(value);
        dictCache.AddOrUpdate(pkv, value, (r, v) => value);
    }

    public void RemoveFromCache(T value)
    {
        string pkv = PrimaryKeyValuesFormulator(value);
        RemoveFromCache(pkv);
    }

    public void RemoveFromCache(string primaryKeyValues)
    {
        dictCache.TryRemove(primaryKeyValues, out var _);
    }

    public void PublishCacheData(DbCommand cmd)
    {
        var req = new TRequest();
        var lst = CacheEntityListFetcher(cmd, req, false);

        TransportData td = new();
        foreach (var e in lst) e.MergeIntoTransportData(td);

        string topic = $"{SessionController.CustomerCode}/CacheData/{MasterTableName}";

        PayloadPacket pkt = new()
        {
            Topic = topic,
            Payload = td.ConvertToJsonObject()
        };

        SessionController.HubConnection.PublishPayloadPacket(topic, pkt, true);
    }
}