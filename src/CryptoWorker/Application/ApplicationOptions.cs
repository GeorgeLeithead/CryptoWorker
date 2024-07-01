// Ignore Spelling: Milli

namespace CryptoWorker.Application;

public sealed class ApplicationOptions
{
    [Required]
    /// <summary>The number of items per page.</summary>
    public required int PageSize { get; set; }

    [Required]
    /// <summary>The page size parameter name.</summary>
    public required string PageSizeParameter { get; set; }

    [Required]
    /// <summary>The page parameter name.</summary>
    public required string PageParameter { get; set; }

    [Required]
    /// <summary>The end point API path.</summary>
    public required Uri Path { get; set; }

    [Required]
    /// <summary>The end point base address.</summary>
    public required string BaseAddress { get; set; }

    [Required]
    /// <summary>The maximum number of pages to retrieve.</summary>
    public required int MaxPages { get; set; }

    [Required]
    /// <summary>Time that the service is scheduled to cycle.</summary>
    public required TimeOnly ScheduleCycleTime { get; set; }

    [Required]
    /// <summary>Retry delay in seconds.</summary>
    public required int RetryDelay { get; set; }

    [Required]
    /// <summary>Number of retry attempts</summary>
    public required int RetryAttempts { get; set; }
}