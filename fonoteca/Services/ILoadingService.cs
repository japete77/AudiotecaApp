namespace fonoteca.Services
{
    public interface ILoadingService
    {
        Task<IDisposable> Show(string message = null);
    }
}
