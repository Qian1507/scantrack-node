namespace ScanTrackNode.Services;

public class HeartbeatService : BackgroundService
{
    private readonly NodeRegistry _registry;
    private readonly ILogger<HeartbeatService> _logger;

    // Inject NodeRegistry and logger through dependency injection.
    public HeartbeatService(
        NodeRegistry registry,
        ILogger<HeartbeatService> logger)
    {
        _registry = registry;
        _logger = logger;
    }

    // This method runs continuously in the background
    // while the application is running.
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            // Wait one hour before sending the next heartbeat.
            await Task.Delay(TimeSpan.FromHours(1), ct);

            // Write a log entry before sending the heartbeat.
            _logger.LogInformation("Sending heartbeat to registry...");

            // Re-register this node in the registry.
            // The registry treats this POST /nodes request as a heartbeat.
            await _registry.RegisterSelfAsync();
        }
    }
}