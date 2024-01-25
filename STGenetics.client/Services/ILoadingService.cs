public interface ILoadingService
{
    event Action LoadingStarted;
    event Action LoadingEnded;

    void StartLoading();
    void EndLoading();
}

public class LoadingService : ILoadingService
{
    public event Action LoadingStarted;
    public event Action LoadingEnded;

    public void StartLoading() => LoadingStarted?.Invoke();
    public void EndLoading() => LoadingEnded?.Invoke();
}