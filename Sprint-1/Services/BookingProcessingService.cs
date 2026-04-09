using Sprint_1.Models;

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

                    // Имитация обращения к внешней системе
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

                    booking.Status = BookingStatus.Confirmed;
                    booking.ProcessedAt = DateTime.UtcNow;

                    bookingService.UpdateBooking(booking);

                    logger.LogInformation(
                        "Бронирование {BookingId} обработано. Статус: {Status}.",
                        booking.Id, booking.Status);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Штатное завершение при остановке приложения
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при обработке бронирований.");
            }

            // Пауза между циклами опроса
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }

        logger.LogInformation("Фоновый сервис обработки бронирований остановлен.");
    }
}

