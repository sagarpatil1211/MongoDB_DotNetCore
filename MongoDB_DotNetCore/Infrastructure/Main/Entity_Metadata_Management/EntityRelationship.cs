//using VJ1Core.Infrastructure.Main;

public class EntityRelationship
{
    public string ParentEntityType { get; set; }
    public string ChildEntityType { get; set; }
    public string ChildEntityTableName { get; set; }
    public string ParentEntityForeignKeyColumnNames { get; set; }
    public bool NullParentAllowed { get; set; }
    public EntityRelationshipTypes RelationshipType { get; set; }
    public string ChildEntityCollectionPropertyNameInParent { get; set; }
    public string ErrorMessageIfNullParent { get; set; }

    public static EntityRelationship CreateInstance(string parentEntityType,
                                                    string childEntityType,
                                                    string childEntityTableName,
                                                    string parentEntityForeignKeyColumnNames,
                                                    bool nullParentAllowed,
                                                    EntityRelationshipTypes relationshipType,
                                                    string childEntityCollectionPropertyNameInParent,
                                                    string errorMessageIfNullParent)
    {
        return new EntityRelationship(parentEntityType, childEntityType, 
            childEntityTableName, parentEntityForeignKeyColumnNames, 
            nullParentAllowed, relationshipType,
            childEntityCollectionPropertyNameInParent, errorMessageIfNullParent);
    }

    private EntityRelationship() { }
    private EntityRelationship(string parentEntityType,
                                string childEntityType,
                                string childEntityTableName,
                                string parentEntityForeignKeyColumnNames,
                                bool nullParentAllowed,
                                EntityRelationshipTypes relationshipType,
                                string childEntityCollectionPropertyNameInParent,
                                string errorMessageIfNullParent)
    {
        ParentEntityType = parentEntityType;
        ChildEntityType = childEntityType;
        ChildEntityTableName = childEntityTableName;
        ParentEntityForeignKeyColumnNames = parentEntityForeignKeyColumnNames;
        NullParentAllowed = nullParentAllowed;
        RelationshipType = relationshipType;
        ChildEntityCollectionPropertyNameInParent = childEntityCollectionPropertyNameInParent;
        ErrorMessageIfNullParent = errorMessageIfNullParent;
    }
}
