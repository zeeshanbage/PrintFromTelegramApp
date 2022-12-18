namespace GetMessageService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private TelegramClient _telegramClient;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _telegramClient = new TelegramClient(logger);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await _telegramClient.TelegramBotClientAsync(stoppingToken);
        }
    }
}