using System;
using System.Collections.Generic;
using System.Text;

public class TimedProcessManager
{
    //private static Dictionary<string, Action> m_hourlyProcesses = new Dictionary<string, Action>();
    //// Key = String in mm format, the timer will check the timing by making an mm format string from
    //// the SignalTime and then execute these processes accordingly

    //private static Dictionary<string, Action> m_dailyProcesses = new Dictionary<string, Action>();
    //// Key = String in hhmm format, the timer will check the timing by making an hhmm format string from
    //// the SignalTime and then execute these processes accordingly

    //private static System.Timers.Timer ProcessExecutionTimer { get; set; } = null;

    //private static string m_previousHourValue = string.Empty;
    //private static string m_previousMinuteValue = string.Empty;

    //static TimedProcessManager()
    //{
    //    ProcessExecutionTimer = new System.Timers.Timer(60 * 1000); // Will fire every minute, but will help detect the change from 
    //                                                                // one hour to the other by observing when "minute" value is zero

    //    ProcessExecutionTimer.Elapsed += (s, ev) =>
    //        {
    //            var dtNow = ev.SignalTime;

    //            if (dtNow.Minute != 0)
    //            {
    //                m_previousMinuteValue = dtNow.Minute;
    //            }
    //            else // Minute == 0 means the hour has changed
    //            {
    //                if (m_previousMinuteValue != dtNow.Minute)
    //                {
    //                    if (dtNow.Hour != 0)
    //                    {
    //                        m_previousHourValue = dtNow.Hour;
    //                    }
    //                    else // Hour == 0 means the day has changed
    //                    {
    //                        if (m_previousHourValue != dtNow.Hour)
    //                        {
    //                            m_previousHourValue = dtNow.Hour;
    //                        }
    //                    }
    //                }

    //                m_previousMinuteValue = dtNow.Minute;
    //            }
    //        };

    //    var dtStart = DateTime.Now;

    //    m_previousHourValue = dtStart.Hour;
    //    m_previousMinuteValue = dtStart.Minute;

    //    ProcessExecutionTimer.Start();
    //}

    //public static void RegisterHourlyProcess(Action process)
    //{
    //    m_hourlyProcesses.Add(process);
    //}

    //public static void RegisterDailyProcess(Action process)
    //{
    //    m_dailyProcesses.Add(process);
    //}

    //private static void ExecuteHourlyProcesses()
    //{
    //    foreach(Action proc in m_hourlyProcesses)
    //    {
    //        proc();
    //    }
    //}
}
