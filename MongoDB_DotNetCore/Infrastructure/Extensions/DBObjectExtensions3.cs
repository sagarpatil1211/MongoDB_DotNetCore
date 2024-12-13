using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;

namespace VJCore.Infrastructure.Extensions;

public static class DBObjectExtensions3
{
    private static readonly Stopwatch sw = new Stopwatch();

    public static int ExecuteNonQuery2(this DbCommand cmd)
    {
        if (SessionController.EnableSQLCommandPerformanceLogging)
        {
            sw.Restart();
            string strCommand = cmd.GenerateParameterDeclarationsAndCommandStringForQueryWindow();

            int result = cmd.ExecuteNonQuery();

            long elapsedMs = sw.ElapsedMilliseconds;
            string stringToLog = string.Concat("ExecuteNonQuery", "|\n", strCommand, "|\n", elapsedMs.ToString(), "|\n\n\n");

            File.AppendAllText(SessionController.SQLCommandPerformanceLogFilePath, stringToLog);

            return result;
        }
        else
        {
            return cmd.ExecuteNonQuery();
        }

    }

    public static int Fill2(this DbDataAdapter dta, DataSet dts, string tableName)
    {
        if (SessionController.EnableSQLCommandPerformanceLogging)
        {
            sw.Restart();
            string strCommand = dta.SelectCommand.GenerateParameterDeclarationsAndCommandStringForQueryWindow();

            int result = dta.Fill(dts, tableName);

            long elapsedMs = sw.ElapsedMilliseconds;
            string stringToLog = string.Concat("FillDataSet", "|\n", strCommand, "|\n", elapsedMs.ToString(), "|\n\n\n");

            File.AppendAllText(SessionController.SQLCommandPerformanceLogFilePath, stringToLog);

            return result;
        }
        else
        {
            return dta.Fill(dts, tableName);
        }
    }

    public static int Fill2(this DbDataAdapter dta, DataTable dtb)
    {
        if (SessionController.EnableSQLCommandPerformanceLogging)
        {
            sw.Restart();
            string strCommand = dta.SelectCommand.GenerateParameterDeclarationsAndCommandStringForQueryWindow();

            int result = dta.Fill(dtb);

            long elapsedMs = sw.ElapsedMilliseconds;
            string stringToLog = string.Concat("FillDataTable", "|\n", strCommand, "|\n", elapsedMs.ToString(), "|\n\n\n");

            File.AppendAllText(SessionController.SQLCommandPerformanceLogFilePath, stringToLog);

            return result;
        }
        else
        {
            return dta.Fill(dtb);
        }
    }

    public static object ExecuteScalar2(this DbCommand cmd)
    {
        if (SessionController.EnableSQLCommandPerformanceLogging)
        {
            sw.Restart();
            string strCommand = cmd.GenerateParameterDeclarationsAndCommandStringForQueryWindow();

            object result = cmd.ExecuteScalar();

            long elapsedMs = sw.ElapsedMilliseconds;
            string stringToLog = string.Concat("ExecuteScalar", "|\n", strCommand, "|\n", elapsedMs.ToString(), "|\n\n\n");

            File.AppendAllText(SessionController.SQLCommandPerformanceLogFilePath, stringToLog);

            return result;
        }
        else
        {
            return cmd.ExecuteScalar();
        }
    }

    public static DbDataReader ExecuteReader2(this DbCommand cmd)
    {
        if (SessionController.EnableSQLCommandPerformanceLogging)
        {
            sw.Restart();
            string strCommand = cmd.GenerateParameterDeclarationsAndCommandStringForQueryWindow();

            DbDataReader result = cmd.ExecuteReader();

            long elapsedMs = sw.ElapsedMilliseconds;
            string stringToLog = string.Concat("ExecuteReader", "|\n", strCommand, "|\n", elapsedMs.ToString(), "|\n\n\n");

            File.AppendAllText(SessionController.SQLCommandPerformanceLogFilePath, stringToLog);

            return result;
        }
        else
        {
            return cmd.ExecuteReader();
        }
    }
}