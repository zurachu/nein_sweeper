using R3;

public class TimerModel
{
    public SerializableReactiveProperty<float> CurrentTime = new(0f);

    private bool isRunning = false;

    public Observable<Unit> RunningUpdateObservable()
    {
        return Observable.EveryUpdate().Where(_ => isRunning);
    }

    public void Run()
    {
        CurrentTime.Value = 0;
        isRunning = true;
    }

    public void Stop()
    {
        isRunning = false;
    }

    public void Reset()
    {
        Stop();
        CurrentTime.Value = 0;
    }
}
