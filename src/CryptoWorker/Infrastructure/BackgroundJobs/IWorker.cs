namespace CryptoWorker.Infrastructure.BackgroundJobs;

using System.Threading;
using System.Threading.Tasks;

public interface IWorker
{
    Task StopAsync(CancellationToken cancellationToken);
}