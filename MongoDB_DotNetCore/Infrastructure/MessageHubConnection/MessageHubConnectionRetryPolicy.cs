using Microsoft.AspNetCore.SignalR.Client;
using System;

public class MessageHubConnectionRetryPolicy : IRetryPolicy
{
    public TimeSpan? NextRetryDelay(RetryContext retryContext)
    {
        return TimeSpan.FromSeconds(5);
    }
}
