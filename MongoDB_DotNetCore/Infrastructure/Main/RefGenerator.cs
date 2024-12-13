using System;
using System.Collections.Generic;
using System.Data.Common;
using VJCore.Infrastructure.Extensions;
//using VJ1Core.Domain.User_Management;
//using VJ1Core.Infrastructure.Main;

public sealed class RefGenerator
{
    private readonly static long MinUserRef = 1001;

    private static readonly DataAccessUtils DAU = SessionController.DAU;

    public static PayloadPacket AllocateRefs(PayloadPacket incomingPkt)
    {
        TransactionResult tr = new TransactionResult();

        var cToken = DAU.OpenConnectionAndBeginTransaction();

        try
        {
            if (incomingPkt.PayloadDescriptor == "AllocateRefsRequest")
            {
                //SystemUserOperationsManager.CheckUserSessionValidityForPerformingOperations(incomingPkt);

                //var req = AllocateRefsRequest.Deserialize(incomingPkt.Payload.ToString());

                //var cmd = SessionController.DAU.CreateCommand();
                //var lstRefs = GenerateAndStoreNextRefs(cmd, req.Count);

                //var resp = new AllocateRefsResponse() { Refs = lstRefs };

                //tr.Successful = true;
                //tr.Tag = resp.Serialize();
                //tr.TagType = typeof(AllocateRefsResponse).FullName;
            }

            DAU.CommitTransaction(cToken);
        }
        catch (Exception ex)
        {
            DAU.RollbackTransaction(cToken);
            tr.AbsorbException(ex);
        }
        finally
        {
            DAU.CloseConnection(cToken);
        }

        PayloadPacket pktResult = PayloadPacket.FromTransactionResult(tr, incomingPkt);
        return pktResult;
    }

    public static long GenerateAndStoreNextRef(DbCommand cmd)
    {
        cmd.Parameters.Clear();

        cmd.CommandText = "select ref from entityref";
        var currentRef = Utils.GetInt64(cmd.ExecuteScalar2());
        if (currentRef < (MinUserRef - 1)) currentRef = (MinUserRef - 1);
        long newRef = currentRef + 1;

        cmd.CommandText = "delete from entityref";
        cmd.ExecuteNonQuery2();

        cmd.CommandText = "insert into entityref (ref) values (@NewRef)";
        cmd.Parameters.Clear();
        cmd.AddLongParameter("@NewRef", newRef);
        cmd.ExecuteNonQuery2();

        var strNewRef = newRef.ToString("D10");

        var strResult = $"{SessionController.CustomerLicenseNo}{strNewRef}";
        long result = long.Parse(strResult);

        return result;
    }

    public static List<long> GenerateAndStoreNextRefs(DbCommand cmd, long count)
    {
        var result = new List<long>();

        cmd.CommandText = "select ref from entityref";
        var currentRef = Utils.GetInt64(cmd.ExecuteScalar2());
        if (currentRef < (MinUserRef - 1)) currentRef = (MinUserRef - 1);

        cmd.CommandText = "delete from entityref";
        cmd.ExecuteNonQuery2();

        for (long newRef = currentRef + 1; newRef <= currentRef + count; newRef++)
        {
            cmd.CommandText = $"insert into entityref (ref) values ({newRef})";
            cmd.ExecuteNonQuery2();

            var strNewRef = $"{SessionController.CustomerLicenseNo}{newRef:D10}";
            result.Add(long.Parse(strNewRef));
        }

        return result;
    }

    public static long GenerateRefValue(long specialRef)
    {
        var strSpecialRef = specialRef.ToString("D10");

        var strResult = $"{SessionController.CustomerLicenseNo}{strSpecialRef}";
        long result = long.Parse(strResult);

        return result;
    }

    public static List<long> GenerateRefValues(long fromSpecialRef, long count)
    {
        var result = new List<long>();

        for (long specialRef = fromSpecialRef; specialRef <= (fromSpecialRef + count - 1); specialRef++)
        {
            var strSpecialRef = specialRef.ToString("D10");

            var strRef = $"{SessionController.CustomerLicenseNo}{strSpecialRef}";
            result.Add(long.Parse(strRef));
        }

        return result;
    }
}
