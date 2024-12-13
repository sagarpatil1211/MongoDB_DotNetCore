using System;
using System.Collections.Generic;
using System.Text;

public class FetchRequestHandlers
{
    private static Dictionary<string, Action<TransportData>> m_handlers = new Dictionary<string, Action<TransportData>>();

    public static void RegisterFetchHandler(string fetchRequestType, Action<TransportData> handler)
    {
        m_handlers[fetchRequestType] = handler;
    }

    public static Action<TransportData> GetFetchHandler(string fetchRequestType)
    {
        if (m_handlers.ContainsKey(fetchRequestType)) return m_handlers[fetchRequestType];
        return null;
    }
}
