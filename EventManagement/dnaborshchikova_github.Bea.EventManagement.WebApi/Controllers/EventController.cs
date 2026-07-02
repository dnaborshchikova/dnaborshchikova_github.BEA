using dnaborshchikova_github.Bea.EventManagement.Core.Interfaces;
using dnaborshchikova_github.Bea.EventManagement.Core.Models;
using dnaborshchikova_github.Bea.EventManagement.WebApi.Handlers;
using dnaborshchikova_github.Bea.EventManagement.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace dnaborshchikova_github.Bea.EventManagement.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly EventBatchHandler _eventBatchHandler;
        private readonly ILogger<EventController> _logger;

        public EventController(IEventService eventService, EventBatchHandler eventBatchHandler
            , ILogger<EventController> logger)
        {
            _eventService = eventService;
            _eventBatchHandler = eventBatchHandler;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> IngestEventRequest([FromBody] IngestEventDto ingestEventDto)
        {
            if (ingestEventDto == null)
                return BadRequest("No events provided");

            var cashRegisterEvent = new CashRegisterEvent(ingestEventDto.Id, ingestEventDto.Date
                , ingestEventDto.UserId, ingestEventDto.EventType, ingestEventDto.Data);

            await _eventService.SaveEventAsync(cashRegisterEvent);

            return Ok();
        }

        [HttpPost("batch")]
        public async Task<IActionResult> IngestEventBatch([FromBody] List<IngestEventDto> events)
        {
            if (events == null || events.Count == 0)
                return BadRequest();

            _logger.LogInformation("Batch received. Count={Count}", events.Count);

            var batchConvertResult = _eventBatchHandler.ConvertDtoToCashRegisterEvent(events);
            _logger.LogInformation("Batch validated. Ok={Ok}, Errors={Errors}",
                batchConvertResult.Events.Count, batchConvertResult.Errors.Count);

            if (batchConvertResult.Errors.Count > 0)
            {
                foreach (var error in batchConvertResult.Errors)
                    _logger.LogInformation($"Validation error: {error}");
            }

            if (batchConvertResult.Events.Count == 0)
            {
                _logger.LogInformation("Event save skipped. No valid events.");
                return Ok();
            }

            await _eventService.SaveEventBatchAsync(batchConvertResult.Events);
            _logger.LogInformation("Batch saved. Count={Count}", batchConvertResult.Events.Count);

            return Ok();
        }
    }
}
