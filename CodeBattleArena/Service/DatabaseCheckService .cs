namespace CodeBattleArena.Service
{
    public class DatabaseCheckService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseCheckService> _logger;
        public DatabaseCheckService(IServiceProvider serviceProvider, ILogger<DatabaseCheckService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Создаем новый скоуп для получения необходимых сервисов
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<DBService>();

                        // Ваш код проверки или обновления данных в базе данных
                        await CheckDatabaseAsync(dbContext);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking the database.");
                }

                // Ждем 10 минут перед следующей проверкой
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        private Task CheckDatabaseAsync(DBService dbContext)
        {
            dbContext.SessionService.DeleteExpiredSessions(DateTime.Now);

            return Task.CompletedTask;
        }
    }
}
