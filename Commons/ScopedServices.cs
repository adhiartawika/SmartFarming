namespace backend.Commons
{
    public interface IMyScopedService
    {
        Task DoWork(CancellationToken cancellationToken);
    }

    public class ScopedServices : IMyScopedService
    {
        private readonly ILogger<ScopedServices> _logger;

        public ScopedServices(ILogger<ScopedServices> logger)
        {
            _logger = logger;
        }

        public async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{now} MyScopedService is working.", DateTime.Now.ToString("T"));
            await Task.Delay(1000 * 20, cancellationToken);
        }
    }
}