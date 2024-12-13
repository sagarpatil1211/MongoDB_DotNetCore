using System;
using System.Collections.Generic;

public class EntityNomenclature
{
    public static string GetSingularName(string entityType)
    {
        if (!m_nomenclatureMap.ContainsKey(entityType)) return string.Empty;
        var result = m_nomenclatureMap[entityType];
        return result.Item1;
    }

    public static string GetPluralName(string entityType)
    {
        if (!m_nomenclatureMap.ContainsKey(entityType)) return string.Empty;
        var result = m_nomenclatureMap[entityType];
        return result.Item2;
    }

    public static void RegisterEntityNomenclature(string entityType, string singularName, string pluralName)
    {
        Tuple<string, string> entry = new Tuple<string, string>(singularName, pluralName);
        m_nomenclatureMap.Add(entityType, entry);
    }

    private EntityNomenclature() { }
    private static readonly Dictionary<string, Tuple<string, string>> m_nomenclatureMap = new Dictionary<string, Tuple<string, string>>();
}
