using System;
using System.Linq;
using System.Collections.Generic;

public class EntityConstraintsManager
{
    public static void RegisterUniqueConstraint(string entityType,
                                                string uniqueConstraintGroupName,
                                                string constraintViolationMessage,
                                                params string[] propertyNames)
    {
        if (!m_entityUniqueConstraintsMap.ContainsKey(entityType)) 
                m_entityUniqueConstraintsMap.Add(entityType, new Dictionary<string, List<string>>());

        if (!m_entityUniqueConstraintViolationMessages.ContainsKey(entityType))
            m_entityUniqueConstraintViolationMessages.Add(entityType, new Dictionary<string, string>());

        var groupwiseUniqueConstraintsMap = m_entityUniqueConstraintsMap[entityType];
        if (groupwiseUniqueConstraintsMap.ContainsKey(uniqueConstraintGroupName)) throw new DomainException("There can be only one unique constraint group name for an entity.");

        groupwiseUniqueConstraintsMap.Add(uniqueConstraintGroupName, new List<string>(propertyNames));

        var groupwiseUniqueConstraintViolationMessagesMap = m_entityUniqueConstraintViolationMessages[entityType];
        groupwiseUniqueConstraintViolationMessagesMap.Add(uniqueConstraintGroupName, constraintViolationMessage);
    }

    public static List<string> GetUniqueConstraintGroupNamesForEntityType(string entityType)
    {
        if (!m_entityUniqueConstraintsMap.ContainsKey(entityType)) return new List<string>();

        var groupwiseMap = m_entityUniqueConstraintsMap[entityType];
        return groupwiseMap.Keys.ToList();
    }

    public static string GetUniqueConstraintViolationMessage(string entityType, string constraintGroupName)
    {
        if (!m_entityUniqueConstraintViolationMessages.ContainsKey(entityType)) return string.Empty;

        var groupwiseMap = m_entityUniqueConstraintViolationMessages[entityType];

        if (!groupwiseMap.ContainsKey(constraintGroupName)) return string.Empty;

        return groupwiseMap[constraintGroupName];
    }

    public static List<string> GetUniquePropertyNamesForEntityType(string entityType, string constraintGroupName)
    {
        if (!m_entityUniqueConstraintsMap.ContainsKey(entityType)) return new List<string>();

        var groupwiseMap = m_entityUniqueConstraintsMap[entityType];

        if (!groupwiseMap.ContainsKey(constraintGroupName)) return new List<string>();

        return groupwiseMap[constraintGroupName];
    }

    private EntityConstraintsManager() { }

    private static readonly Dictionary<string, Dictionary<string, List<string>>> m_entityUniqueConstraintsMap = new Dictionary<string, Dictionary<string, List<string>>>();
    // Key = EntityType
    // In the Value: Key = Constraint Group, Value = Unique Property Names List

    private static readonly Dictionary<string, Dictionary<string, string>> m_entityUniqueConstraintViolationMessages = new Dictionary<string, Dictionary<string, string>>();
}
