using Microsoft.AspNetCore.SignalR.Client;
using System;

public class RequestResponseHubConnectionRetryPolicy : IRetryPolicy
{
    public TimeSpan? NextRetryDelay(RetryContext retryContext)
    {
        return TimeSpan.FromSeconds(5);
    }
}
