namespace Prima.Core.Server.Data.Internal;

public class TimerDataObject : IDisposable
{
    private readonly object _lock = new object();
    public string Name { get; set; }

    public string Id { get; set; }

    public double IntervalInMs { get; set; }

    public Action Callback { get; set; }

    public bool Repeat { get; set; }

    public double RemainingTimeInMs = 0;

    public double DelayInMs { get; set; }


    public void DecrementRemainingTime(double deltaTime)
    {
        if (Monitor.TryEnter(_lock))
        {
            try
            {
                if (DelayInMs > 0)
                {
                    DelayInMs -= deltaTime;
                    if (DelayInMs > 0)
                    {
                        return;
                    }
                }

                RemainingTimeInMs -= deltaTime;
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }
    }

    public void ResetRemainingTime()
    {
        if (Monitor.TryEnter(_lock))
        {
            try
            {
                RemainingTimeInMs = IntervalInMs;
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }
    }


    public override string ToString()
    {
        return $"Timer: {Name}, Id: {Id}, Interval: {IntervalInMs}, RemainingTime: {RemainingTimeInMs}, Repeat: {Repeat}";
    }

    public void Dispose()
    {
        Callback = null;
        Name = null;
        Id = null;
        IntervalInMs = 0;
        RemainingTimeInMs = 0;
        Repeat = false;
        DelayInMs = 0;

        GC.SuppressFinalize(this);
    }
}
