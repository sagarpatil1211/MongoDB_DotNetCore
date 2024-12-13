using System.Collections.Generic;
using System.Linq;

public class SystemEventManager
{
    private static readonly Dictionary<long, string> m_eventGroups = new Dictionary<long, string>();
    // Key = Id, Value = Name

    private static readonly Dictionary<long, Dictionary<long, string>> m_eventTypes 
        = new Dictionary<long, Dictionary<long, string>>();
    // Key = EventGroupId, Value = Dictionary with Key = Id, Value = Name

    public static void RegisterEventGroup(long eventGroupIdentifier, string eventGroupName)
    {
        m_eventGroups[eventGroupIdentifier] = eventGroupName;
    }

    public static void RegisterEventType(long eventTypeIdentifier, string eventGroupName, 
        long eventGroupIdentifier)
    {
        if (!m_eventGroups.ContainsKey(eventGroupIdentifier)) 
            throw new DomainException($"Event Group with Id {eventGroupIdentifier} not registered.");
        
        if (!m_eventTypes.ContainsKey(eventGroupIdentifier)) 
            m_eventTypes.Add(eventGroupIdentifier, new Dictionary<long, string>());
        
        var dict = m_eventTypes[eventGroupIdentifier];
        dict[eventTypeIdentifier] = eventGroupName;

        m_eventTypes[eventTypeIdentifier] = dict;
    }

    public static List<RefName> GetListOfEventTypes(long eventGroupIdentifier)
    {
        if (!m_eventGroups.ContainsKey(eventGroupIdentifier)) return new List<RefName>();
        if (!m_eventTypes.ContainsKey(eventGroupIdentifier)) return new List<RefName>();

        var result = m_eventTypes[eventGroupIdentifier].Select(kvp => new RefName(kvp.Key, kvp.Value)).ToList();
        return result;
    }
}
