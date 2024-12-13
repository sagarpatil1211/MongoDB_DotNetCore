//using SSDBCA;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using VJCore.Infrastructure.Extensions;
//using VJ1Core.Infrastructure;
//using VJ1Core.Infrastructure.DatabaseMigration.Instructions;
//using VJ1Core.Infrastructure.DatabaseMigration.SQLiteConnectivity;
//using VJ1Core.Infrastructure.Main.Entity_Metadata_Management;

public partial class EntityTypes
{
    private static bool entityTypeRegistrationsDone = false;

    private static readonly ConcurrentDictionary<string, List<DenormalizedPropertyAttribute>> m_denormalizedPropertyAttributes
        = new ConcurrentDictionary<string, List<DenormalizedPropertyAttribute>>();
    // Key = EntityType, Value = List<DenormalizedPropertyAttribute>

    private static readonly ConcurrentDictionary<string, List<DenormalizedEnumPropertyNameAttribute>> m_denormalizedEnumPropertyNameAttributes
        = new ConcurrentDictionary<string, List<DenormalizedEnumPropertyNameAttribute>>();
    // Key = EntityType, Value = List<DenormalizedEnumPropertyNameAttribute>

    public static List<DenormalizedPropertyAttribute> GetDenormalizedPropertyAttributes(string entityType)
    {
        m_denormalizedPropertyAttributes.TryGetValue(entityType, out var result);
        return result;
    }

    public static List<DenormalizedEnumPropertyNameAttribute> GetDenormalizedEnumPropertyNameAttributes(string entityType)
    {
        m_denormalizedEnumPropertyNameAttributes.TryGetValue(entityType, out var result);
        return result;
    }

    public static void PerformRegistrations(Action<string> consoleLogger = null)
    {
        if (entityTypeRegistrationsDone) return;

        var dbMigrationDAL = new SQLiteDataAccessService(string.Empty, (msg, title) => { });

        var assemblies = AssemblyManagement.GetAllAssemblies();

        //var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly assy in assemblies)
        {
            foreach (Type t in assy.GetTypes())
            {
                if (t.IsSubclassOf(typeof(BaseObject)) && !t.IsAbstract)
                {
                    foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                    {
                        if (m.Name == "RegisterEntityTypeAspects") m.Invoke(null, null);
                    }

                    string entityType = string.Empty;

                    foreach (PropertyInfo p in t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                    {
                        if (p.Name == "EntityType") entityType = p.GetValue(null).ToString();
                    }

                    if (entityType.Trim().Length == 0) throw new DomainException($"EntityType field not found on type {t.FullName}");

                    foreach (PropertyInfo p in t.GetProperties())
                    {
                        var dnpa = p.GetCustomAttribute<DenormalizedPropertyAttribute>();
                        if (dnpa != null)
                        {
                            m_denormalizedPropertyAttributes.AddOrUpdate(entityType,
                                new List<DenormalizedPropertyAttribute> { dnpa },
                                (eType, lst) =>
                                {
                                    lst.Add(dnpa);
                                    return lst;
                                });
                        }
                    }

                    foreach (PropertyInfo p in t.GetProperties())
                    {
                        var dnepa = p.GetCustomAttribute<DenormalizedEnumPropertyNameAttribute>();
                        if (dnepa != null)
                        {
                            m_denormalizedEnumPropertyNameAttributes.AddOrUpdate(entityType,
                                new List<DenormalizedEnumPropertyNameAttribute> { dnepa },
                                (eType, lst) =>
                                {
                                    lst.Add(dnepa);
                                    return lst;
                                });
                        }
                    }
                }

                if (t.GetCustomAttribute<NonPersistedFunctionalEntityAttribute>() != null)
                {
                    foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                    {
                        if (m.Name == "RegisterEntityTypeAspects") m.Invoke(null, null);
                    }
                }

                if (t.GetCustomAttribute<RealTimeDataEntityAttribute>() != null)
                {
                    foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                    {
                        if (m.Name == "RegisterEntityTypeAspects") m.Invoke(null, null);
                    }
                }
            }

            foreach (Type t in assy.GetTypes())
            {
                if (t.IsEnum)
                {
                    var persistedEnumAttr = t.GetCustomAttribute<PersistedEnumAttribute>();

                    if (persistedEnumAttr != null)
                    {
                        var tableName = persistedEnumAttr.DBTableName;
                        if (tableName.Trim().Length == 0) tableName = t.Name;

                        var instrCheck = dbMigrationDAL.GetCreateTableInstruction(tableName);

                        if (instrCheck == null)
                        {
                            var instr = new CreateTableInstruction(dbMigrationDAL)
                            {
                                TableName = tableName,
                                Query = $"CREATE TABLE {tableName} (Ref bigint NOT NULL, Name varchar(1024) NOT NULL)"
                            };

                            var _result = instr.Save();
                        }

                        var pkInstrCheck = dbMigrationDAL.GetSetPrimaryKeyOnTableInstruction(tableName, "Ref");

                        if (pkInstrCheck == null)
                        {
                            var pkInstr = new SetPrimaryKeyOnTableInstruction(dbMigrationDAL)
                            {
                                TableName = tableName,
                                Columns = "Ref"
                            };

                            var _result = pkInstr.Save();
                        }
                    }
                }
            }
        }

        foreach (string entityType in GetAllRegisteredEntityTypes())
        {
            var pkColumns = GetEntityTypePrimaryKeys(entityType);
            var tableName = GetDatabaseTableNameFromEntityType(entityType);

            var ctInstrCheck = dbMigrationDAL.GetCreateTableInstruction(tableName);

            if (ctInstrCheck is not null)
            {
                var pkInstrCheck = dbMigrationDAL.GetSetPrimaryKeyOnTableInstruction(tableName, pkColumns);

                if (pkInstrCheck == null)
                {
                    var pkInstr = new SetPrimaryKeyOnTableInstruction(dbMigrationDAL)
                    {
                        TableName = tableName,
                        Columns = pkColumns
                    };

                    var _result = pkInstr.Save();
                }

                var lstParentRelationships = EntityRelationshipManager.GetParentRelationshipsForEntityType(entityType);

                foreach (EntityRelationship rel in lstParentRelationships)
                {
                    var fkColumns = rel.ParentEntityForeignKeyColumnNames;

                    var indexInstrCheck = dbMigrationDAL.GetSetIndexOnTableInstruction(tableName, fkColumns);

                    if (indexInstrCheck == null)
                    {
                        var indexInstr = new SetIndexOnTableInstruction(dbMigrationDAL)
                        {
                            TableName = tableName,
                            Columns = fkColumns
                        };

                        var _result = indexInstr.Save();
                    }
                }
            }
            else
            {
                if (consoleLogger is not null)
                {
                    consoleLogger($"Table {tableName} needs to be created.");
                }
            }
        }

        entityTypeRegistrationsDone = true;
    }

    public static void EnsurePersistedEnumStringsSynchronizationInDB()
    {
        var DAU = SessionController.DAU;

        var assemblies = AssemblyManagement.GetAllAssemblies();

        //var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var cToken = DAU.OpenConnectionAndBeginTransaction();

        try
        {
            var cmd = DAU.CreateCommand();

            foreach (Assembly assy in assemblies)
            {
                foreach (Type t in assy.GetTypes())
                {
                    if (t.IsEnum)
                    {
                        var persistedEnumAttr = t.GetCustomAttribute<PersistedEnumAttribute>();

                        if (persistedEnumAttr != null)
                        {
                            var tableName = persistedEnumAttr.DBTableName;
                            if (tableName.Trim().Length == 0) tableName = t.Name;

                            foreach (FieldInfo f in t.GetFields())
                            {
                                var enumMemberStringAttr = f.GetCustomAttribute<EnumMemberStringAttribute>();

                                if (enumMemberStringAttr != null)
                                {
                                    var fieldString = enumMemberStringAttr.MemberString;
                                    if (fieldString.Trim().Length == 0) fieldString = f.Name;

                                    var enumValues = Enum.GetValues(t);

                                    foreach (var enumValue in enumValues)
                                    {
                                        var nameCheck = Enum.GetName(t, enumValue);
                                        if (nameCheck == f.Name)
                                        {
                                            var valueInt = Convert.ToInt32(enumValue);

                                            if (!m_enumStrings.TryGetValue(t.FullName, out Dictionary<int, string> value))
                                            {
                                                value = new Dictionary<int, string>();
                                                m_enumStrings.Add(t.FullName, value);
                                            }

                                            var dictStrings = value;
                                            dictStrings[valueInt] = fieldString;

                                            var refCountCheck = Utils.GetInt32(DataAccessUtils.GetScalarValueByQuery(cmd, $"SELECT COUNT(Ref) FROM {tableName} WHERE Ref = {valueInt}"));
                                            if (refCountCheck == 0)
                                            {
                                                cmd.CommandText = $"INSERT INTO {tableName} (Ref, Name) VALUES (@Ref, @Name)";
                                                cmd.Parameters.Clear();
                                                cmd.AddLongParameter("@Ref", valueInt);
                                                cmd.AddVarcharMaxParameter("@Name", fieldString);
                                                cmd.ExecuteNonQuery2();
                                            }
                                            else
                                            {
                                                var nameInDB = Utils.GetString(DataAccessUtils.GetScalarValueByQuery(cmd, $"SELECT Name FROM {tableName} WHERE Ref = {valueInt}"));
                                                if (nameInDB != fieldString)
                                                {
                                                    cmd.CommandText = $"UPDATE {tableName} SET Name = @Name WHERE Ref = @Ref";
                                                    cmd.Parameters.Clear();
                                                    cmd.AddVarcharMaxParameter("@Name", fieldString);
                                                    cmd.AddLongParameter("@Ref", valueInt);
                                                    cmd.ExecuteNonQuery2();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            DAU.CommitTransaction(cToken);
        }
        catch (Exception)
        {
            DAU.RollbackTransaction(cToken);
            throw;
        }
        finally
        {
            DAU.CloseConnection(cToken);
        }
    }

    public static void StoreEnumMemberStringsInMemory()
    {
        var assemblies = AssemblyManagement.GetAllAssemblies();

        //var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly assy in assemblies)
        {
            foreach (Type t in assy.GetTypes())
            {
                if (t.IsEnum)
                {
                    var persistedEnumAttr = t.GetCustomAttribute<PersistedEnumAttribute>();

                    if (persistedEnumAttr != null)
                    {
                        var tableName = persistedEnumAttr.DBTableName;
                        if (tableName.Trim().Length == 0) tableName = t.Name;

                        foreach (FieldInfo f in t.GetFields())
                        {
                            var enumMemberStringAttr = f.GetCustomAttribute<EnumMemberStringAttribute>();

                            if (enumMemberStringAttr != null)
                            {
                                var fieldString = enumMemberStringAttr.MemberString;
                                if (fieldString.Trim().Length == 0) fieldString = f.Name;

                                var enumValues = Enum.GetValues(t);

                                foreach (var enumValue in enumValues)
                                {
                                    var nameCheck = Enum.GetName(t, enumValue);
                                    if (nameCheck == f.Name)
                                    {
                                        var valueInt = Convert.ToInt32(enumValue);

                                        if (!m_enumStrings.TryGetValue(t.FullName, out Dictionary<int, string> value))
                                        {
                                            value = new Dictionary<int, string>();
                                            m_enumStrings.Add(t.FullName, value);
                                        }

                                        var dictStrings = value;
                                        dictStrings[valueInt] = fieldString;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public static Dictionary<string, Tuple<PropertyTypes, string>> GetPropertyTypeMap(string tName)
    {
        if (m_cachedPropertyTypeMaps.TryGetValue(tName, out Dictionary<string, Tuple<PropertyTypes, string>> value))
        {
            return value;
        }

        return GetPropertyTypeMap(Type.GetType(tName));
    }

    public static Dictionary<string, Tuple<PropertyTypes, string>> GetPropertyTypeMap(Type t)
        // Key = Property Name
        // Tuple Item 1 = Property Type
        // Tuple Item 2 = Specific Type in case of Collections and Complex Properties
    {
        if (m_cachedPropertyTypeMaps.TryGetValue(t.FullName, out Dictionary<string, Tuple<PropertyTypes, string>> value))
        {
            return value;
        }

        Dictionary<string, Tuple<PropertyTypes, string>> result
            = new Dictionary<string, Tuple<PropertyTypes, string>>();

        PropertyInfo[] props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (PropertyInfo p in props)
        {
            string typeFullName = p.PropertyType.FullName;

            if (typeFullName == typeof(string).FullName)
            {
                result[p.Name] = Tuple.Create(PropertyTypes.String, string.Empty);
            }
            else if (typeFullName == typeof(decimal).FullName)
            {
                result[p.Name] = Tuple.Create(PropertyTypes.Decimal, string.Empty);
            }
            else if (typeFullName == typeof(int).FullName)
            {
                result[p.Name] = Tuple.Create(PropertyTypes.Integer, string.Empty);
            }
            else if (typeFullName == typeof(long).FullName)
            {
                result[p.Name] = Tuple.Create(PropertyTypes.Long, string.Empty);
            }
            else if (typeFullName == typeof(bool).FullName)
            {
                result[p.Name] = Tuple.Create(PropertyTypes.Boolean, string.Empty);
            }
            else if (typeFullName == typeof(byte[]).FullName)
            {
                result[p.Name] = Tuple.Create(PropertyTypes.ByteArray, string.Empty);
            }
            else if (p.PropertyType.GetInterface(typeof(IEnumerable).FullName) != null)
            {
                if (p.PropertyType.GenericTypeArguments.Length > 0)
                {
                    foreach (Type tInternal in p.PropertyType.GenericTypeArguments)
                    {
                        result[p.Name] = Tuple.Create(PropertyTypes.Collection, tInternal.FullName);
                        break;
                    }
                }
                else
                {
                    result[p.Name] = Tuple.Create(PropertyTypes.Collection, string.Empty);
                }
            }
            else if (p.PropertyType.IsClass)
            {
                result[p.Name] = Tuple.Create(PropertyTypes.Complex, p.PropertyType.FullName);
            }
        }

        if (!m_cachedPropertyTypeMaps.ContainsKey(t.FullName))
        {
            m_cachedPropertyTypeMaps[t.FullName] = result;
        }

        return result;
    }

    private static readonly Dictionary<string, Dictionary<string, Tuple<PropertyTypes, string>>> m_cachedPropertyTypeMaps
        = new Dictionary<string, Dictionary<string, Tuple<PropertyTypes, string>>>();
}
