using Akka.Actor;
//using Serilog;
using System;
//using VJ1Core.Infrastructure.Config;

public class TimeServerPulseGenerator : ReceiveActor
{
    public const string GeneratePulseCommand = "GeneratePulse";

    private MessageHubConnection m_hubConnection = null;

    protected override SupervisorStrategy SupervisorStrategy()
    {
        return new OneForOneStrategy(_ => Directive.Resume);
    }

    private void Initialize()
    {
        ServerHostingConfig.ReadFromFile();

        m_hubConnection = new MessageHubConnection(ServerHostingConfig.BaseUrl, TimeServerHub.HubName);
        m_hubConnection.Connect();
    }

    public TimeServerPulseGenerator()
    {
        Initialize();

        Receive<string>(msg =>
        {
            try
            {
                switch (msg)
                {
                    case GeneratePulseCommand:
                        var dtNow = DateTime.Now;
                        var strDTNow = DTU.ConvertToString(dtNow);

                        m_hubConnection.CallMethodOnHub("NotifyTime", strDTNow);
                        break;
                }
            }
            catch // (Exception ex)
            {
                //Log.Error(ex.Message);
            }
        });
    }
}
