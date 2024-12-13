using System;
using System.Collections.Generic;
using System.Text;

public class CustomProcessRequestHandlers
{
    private static Dictionary<string, Action<TransportData>> m_handlers = new Dictionary<string, Action<TransportData>>();

    public static void RegisterCustomProcessHandler(string fetchRequestType, Action<TransportData> handler)
    {
        m_handlers[fetchRequestType] = handler;
    }

    public static Action<TransportData> GetCustomProcessHandler(string fetchRequestType)
    {
        if (m_handlers.ContainsKey(fetchRequestType)) return m_handlers[fetchRequestType];
        return null;
    }
}
