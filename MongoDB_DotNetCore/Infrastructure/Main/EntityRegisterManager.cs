using VJCore.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

public class EntityRegisterManager
{
    //public static int GetLastObjectRecordVersion(string entityRef)
    //{
    //    var result = 0;

    //    var dau = SessionController.DAU;

    //    using (var cmd = dau.CreateCommand())
    //    {
    //        cmd.CommandText = "select lastobjectrecordversion from entityregister where ref = @EntityRef";
    //        cmd.AddParameterWithValue("@EntityRef", entityRef);

    //        result = Utils.GetInt32(cmd.ExecuteScalar2());
    //    }

    //    return result;
    //}
}