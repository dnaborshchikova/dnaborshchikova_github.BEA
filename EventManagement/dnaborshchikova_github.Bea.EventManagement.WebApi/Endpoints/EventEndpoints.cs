using dnaborshchikova_github.Bea.EventManagement.Core.Models;
using dnaborshchikova_github.Bea.EventManagement.Core.Services;
using dnaborshchikova_github.Bea.EventManagement.WebApi.Handlers;

namespace dnaborshchikova_github.Bea.EventManagement.WebApi.Endpoints
{
    public static class EventEndpoints
    {
        public static IEndpointRouteBuilder MapEventEndpoints(
            this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/events");
            group.MapPost("/batch", IngestEventBatch);

            return app;
        }

        private static async Task<IResult> IngestEventBatch(List<CashRegisterEventDto> events
            , IEventService eventService, EventBatchHandler eventBatchHandler, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("EventEndpoints");

            if (events == null || events.Count == 0)
                return Results.BadRequest();

            logger.LogInformation("Batch received. Count={Count}", events.Count);

            var batchConvertResult = eventBatchHandler.ConvertDtoToCashRegisterEvent(events);
            logger.LogInformation("Batch validated. Ok={Ok}, Errors={Errors}",
                batchConvertResult.Events.Count, batchConvertResult.Errors.Count);

            foreach (var error in batchConvertResult.Errors)
                logger.LogInformation($"Validation error: {error}");

            if (batchConvertResult.Events.Count == 0)
            {
                logger.LogInformation("Event save skipped. No valid events.");
                return Results.Ok();
            }

            await eventService.SaveEventBatchAsync(batchConvertResult.Events);
            logger.LogInformation("Batch saved. Count={Count}", batchConvertResult.Events.Count);

            return Results.Ok();
        }
    }
}
