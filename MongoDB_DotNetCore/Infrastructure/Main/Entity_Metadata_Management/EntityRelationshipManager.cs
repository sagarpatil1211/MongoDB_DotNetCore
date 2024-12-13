using System.Linq;
using System.Collections.Generic;

//name1space VJCore.Infrastructure.Main.Entity_Metadata_Management;

public class EntityRelationshipManager
{
    public static void RegisterEntityRelationship(string childEntityType,
                                                    string childEntityTableName,
                                                    string parentEntityType,
                                                    string parentEntityForeignKeyColumnNamesInChild,
                                                    bool nullParentAllowed,
                                                    EntityRelationshipTypes relationshipType,
                                                    string childEntityCollectionPropertyNameInParent = "",
                                                    string errorMessageIfNullParent = "")
    {
        if (relationshipType == EntityRelationshipTypes.Parent_Child
                && childEntityCollectionPropertyNameInParent.Trim().Length == 0)
        {
            throw new DomainException($"Child Entity Collection Property Name in Parent " +
                $"not specified for Child Entity Type '{childEntityType}' " +
                $"in Parent Entity Type '{parentEntityType}'.");
        }

        var lstRelationships = GetChildRelationshipsForEntityType(parentEntityType);
        bool relationshipExists = false;

        foreach (var rel in lstRelationships)
        {
            if (rel.ChildEntityType == childEntityType
                    & rel.ChildEntityTableName == childEntityTableName
                    & rel.ParentEntityForeignKeyColumnNames == parentEntityForeignKeyColumnNamesInChild)
            {
                relationshipExists = true;
                break;
            }
        }

        if (!relationshipExists)
        {
            var relNew = EntityRelationship.CreateInstance(parentEntityType,
                                                           childEntityType,
                                                           childEntityTableName,
                                                           parentEntityForeignKeyColumnNamesInChild,
                                                           nullParentAllowed,
                                                           relationshipType,
                                                           childEntityCollectionPropertyNameInParent,
                                                           errorMessageIfNullParent);
            lstRelationships.Add(relNew);
            m_entityRelationshipMap[parentEntityType] = lstRelationships;
        }
    }

    public static List<EntityRelationship> GetChildRelationshipsForEntityType(string entityType)
    {
        return m_entityRelationshipMap.TryGetValue(entityType, out List<EntityRelationship> value) 
            ? value 
            : new List<EntityRelationship>();
    }

    public static List<EntityRelationship> GetParentRelationshipsForEntityType(string entityType)
    {
        return m_entityRelationshipMap.SelectMany(kvp => kvp.Value.Where(rel => rel.ChildEntityType == entityType)).ToList();
    }

    public static string GetForeignKeysInChildEntityTypeForParentEntityType(string childEntityType, string parentEntityType)
    {
        if (m_entityRelationshipMap.TryGetValue(parentEntityType, out List<EntityRelationship> value))
        {
            var rel = value.Find(rel => rel.ChildEntityType == childEntityType);
            return rel.ParentEntityForeignKeyColumnNames.Replace(" ", string.Empty);
        }

        return string.Empty;
    }

    private EntityRelationshipManager() { }

    private static readonly Dictionary<string, List<EntityRelationship>> m_entityRelationshipMap = new Dictionary<string, List<EntityRelationship>>();
}