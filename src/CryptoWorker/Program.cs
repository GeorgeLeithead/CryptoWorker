using CryptoWorker.Infrastructure.BackgroundJobs;

var builder = Host.CreateApplicationBuilder(args);
var path = builder.Configuration.GetRequiredSection(nameof(ApplicationOptions)).Path;

builder.Services.AddOptions<ApplicationOptions>().BindConfiguration(path).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddHostedService<Worker>();
builder.Services.AddHttpClient<IWorker, Worker>(c =>
{
    var url = builder.Configuration["ApplicationOptions:BaseAddress"] ?? throw new InvalidOperationException("ApplicationOptions:BaseAddress is not set");
    c.BaseAddress = new(url);
}).AddStandardResilienceHandler(options =>
{
    options.Retry.Delay = TimeSpan.FromSeconds(3);
    options.Retry.MaxRetryAttempts = 7; // Convert.ToInt16(builder.Configuration["ApplicationOptions:MaxRetryAttempts"]);
    options.Retry.UseJitter = true;
    options.Retry.BackoffType = DelayBackoffType.Exponential;
    options.Retry.OnRetry = (args) => {
        Console.WriteLine($"Using TryAddBuilder:  Retry Attempt Number : {args.AttemptNumber} after {args.RetryDelay.TotalSeconds} seconds.");
        return default;
    };
});

//builder.Services.AddResiliencePipelineRegistry((_, y) => y.TryAddBuilder<HttpResponseMessage>("CryptoWorkerKey", (builder, _) => builder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>
//{
//    ShouldHandle = HttpPolicyExtensions.HandleTransientHttpError(),
//    Delay = TimeSpan.FromSeconds(1),
//    MaxRetryAttempts = 3,
//    UseJitter = true,
//    BackoffType = DelayBackoffType.Exponential,
//    OnRetry = (args) =>
//    {
//        Console.WriteLine($"Using TryAddBuilder:  Retry Attempt Number : {args.AttemptNumber} after {args.RetryDelay.TotalSeconds} seconds.");
//        return default;
//    }
//})));

var host = builder.Build();
host.Run();