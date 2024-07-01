namespace CryptoWorker.Infrastructure.BackgroundJobs;

public sealed class Worker : BackgroundService, IWorker
{
    private readonly ILogger<Worker> _logger;
    private readonly IOptions<ApplicationOptions> _options;
    private readonly HttpClient httpClient;

    public Worker(
        HttpClient httpClient,
        IOptions<ApplicationOptions> applicationOptions,
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<Worker> logger)
    {
        this.httpClient = httpClient;
        _logger = logger;
        _options = applicationOptions;
        _ = hostApplicationLifetime.ApplicationStarted.Register(() => _logger.LogInformation("STARTED - host application started at: {TimeStarted}.", DateTimeOffset.Now));
        _ = hostApplicationLifetime.ApplicationStopping.Register(() => _logger.LogInformation("STOPPING - host application stopping at: {TimeStopping}.", DateTimeOffset.Now));
        _ = hostApplicationLifetime.ApplicationStopped.Register(() => _logger.LogInformation("STOPPED - host application stopped at: {TimeStopped}.", DateTimeOffset.Now));
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        var stopWatch = Stopwatch.StartNew();
        await base.StopAsync(cancellationToken);
        _logger.LogInformation("Worker took {Ms} ms to stop.", stopWatch.ElapsedMilliseconds);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("RUNNING: {TimeRunning}, for page size {PageSize}, for {MaxPages} pages.", DateTimeOffset.Now, _options.Value.PageSize, _options.Value.MaxPages);
            }

            int pageNumber = 1;
            var uri = $"{_options.Value.BaseAddress}{_options.Value.Path}&{_options.Value.PageSizeParameter}={_options.Value.PageSize}&{_options.Value.PageParameter}=";
            try
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "CryptoWorker");
                var response = await httpClient.GetAsync($"{uri}{pageNumber}", stoppingToken);
                var responseText = await response.Content.ReadAsStringAsync(stoppingToken);

                _logger.LogInformation("Http status code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Http response content: {ResponseText}", responseText);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    _ = await response.Content.ReadFromJsonAsync(MarketSerializedContext.Default.ListMarkets, cancellationToken: stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feed.");
            }

            TimeSpan wakeUpAlarm = SleepSpan();
            _logger.LogInformation("SLEEPING: Awakening at {WakeUpAlarm}", DateTime.Now.AddTicks(wakeUpAlarm.Ticks).ToString(CultureInfo.CurrentCulture));
            await Task.Delay(wakeUpAlarm, stoppingToken);
        }
    }

    private TimeSpan SleepSpan()
    {
        DateTime today = DateTime.Now;
        DateTime tomorrow = today.Add(new TimeSpan(1, 0, 0, 0));
        var tomorrowAtScheduledHour = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, _options.Value.ScheduleCycleTime.Hour, _options.Value.ScheduleCycleTime.Minute, _options.Value.ScheduleCycleTime.Second, DateTimeKind.Utc);
        TimeSpan diff = tomorrowAtScheduledHour.Subtract(DateTime.Now);
        var minutesFromNow = diff.TotalHours > 24d ? diff.TotalMinutes - (24d * 60d) : diff.TotalMinutes;
        return TimeSpan.FromMinutes(minutesFromNow);
    }
}