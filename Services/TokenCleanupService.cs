namespace EtaAnnouncer.Services
{
    public class TokenCleanupService(IServiceScopeFactory serviceScopeFactory, ILogger<TokenCleanupService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var refreshTokenService = scope.ServiceProvider.GetRequiredService<RefreshTokenService>();
                await refreshTokenService.RemoveExpiredOrRevokedTokensAsync();
                logger.LogInformation("Removed expired or revoked tokens.");
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
