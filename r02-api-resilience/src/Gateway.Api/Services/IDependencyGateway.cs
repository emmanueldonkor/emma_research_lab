namespace Gateway.Api.Services;

public interface IDependencyGateway
{
    Task<DependencyCallResult> CallBaselineAsync(CancellationToken cancellationToken);

    Task<DependencyCallResult> CallWithBoundedRetryAsync(CancellationToken cancellationToken);
}
