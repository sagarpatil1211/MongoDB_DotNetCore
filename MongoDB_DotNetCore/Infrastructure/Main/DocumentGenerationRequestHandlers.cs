using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class DocumentGenerationRequestHandlers
{
    private static Dictionary<string, Func<TransportData, DocumentStreamContainer>> m_handlers = new Dictionary<string, Func<TransportData, DocumentStreamContainer>>();

    public static void RegisterDocumentGenerationRequestHandler(string fetchRequestType, Func<TransportData, DocumentStreamContainer> handler)
    {
        m_handlers[fetchRequestType] = handler;
    }

    public static Func<TransportData, DocumentStreamContainer> GetDocumentGenerationRequestHandler(string fetchRequestType)
    {
        if (m_handlers.ContainsKey(fetchRequestType)) return m_handlers[fetchRequestType];
        return null;
    }
}
