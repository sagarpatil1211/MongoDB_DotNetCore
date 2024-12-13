//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;

//public sealed class ActorUniqueIdGenerator
//{
//    private readonly static int MinUserUniqueId = 101;

//    public static string GenerateAndStoreNextUniqueId(SqlCommand cmd, ActorTypes actorType)
//    {
//        cmd.Parameters.Clear();
//        cmd.Parameters.AddWithValue("@ActorType", Convert.ToInt32(actorType));

//        cmd.CommandText = "select id from actoruniqueid where actortype = @ActorType";
//        var currentId = Utils.GetInt32(cmd.ExecuteScalar());
//        if (currentId < (MinUserUniqueId - 1)) currentId = (MinUserUniqueId - 1);
//        var newId = currentId + 1;

//        cmd.CommandText = "delete from actoruniqueid where actortype = @ActorType";
//        cmd.ExecuteNonQuery();

//        cmd.CommandText = "insert into actoruniqueid (actortype, id) values (@ActorType, @NewId)";
//        cmd.Parameters.Clear();
//        cmd.Parameters.AddWithValue("@ActorType", Convert.ToInt32(actorType));
//        cmd.Parameters.AddWithValue("@NewId", newId);
//        cmd.ExecuteNonQuery();

//        var strNewId = newId.ToString("D6");

//        var result = $"{SessionController.CustomerCode}{Convert.ToInt32(actorType).ToString("D2")}{strNewId}";

//        return result;
//    }

//    public static string GenerateUniqueId(int idValue, ActorTypes actorType)
//    {
//        var strNewId = idValue.ToString("D6");
//        var result = $"{SessionController.CustomerCode}{Convert.ToInt32(actorType).ToString("D2")}{strNewId}";

//        return result;
//    }
//}
