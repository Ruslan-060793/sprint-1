namespace Sprint_1.Services;

public class BookingProcessingService(
    IBookingService bookingService,
    ILogger<BookingProcessingService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Фоновый сервис обработки бронирований запущен.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var pendingBookings = bookingService.GetPendingBookings();

                foreach (var booking in pendingBookings)
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;

                    logger.LogInformation(
                        "Обработка бронирования {BookingId} для события {EventId}...",
                        booking.Id, booking.EventId);

                    try
                    {
                        await bookingService.ProcessBookingAsync(booking, stoppingToken);

                        logger.LogInformation(
                            "Бронирование {BookingId} подтверждено.", booking.Id);
                    }
                    catch (OperationCanceledException)
                    {
                        throw; // Пробрасываем отмену на уровень выше
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex,
                            "Бронирование {BookingId} отклонено из-за ошибки.", booking.Id);
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка в цикле обработки бронирований.");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }

        logger.LogInformation("Фоновый сервис обработки бронирований остановлен.");
    }
}

