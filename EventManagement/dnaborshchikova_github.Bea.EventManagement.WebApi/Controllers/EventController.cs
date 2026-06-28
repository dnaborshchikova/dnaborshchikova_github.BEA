using dnaborshchikova_github.Bea.EventManagement.Core.Interfaces;
using dnaborshchikova_github.Bea.EventManagement.Core.Models;
using dnaborshchikova_github.Bea.EventManagement.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace dnaborshchikova_github.Bea.EventManagement.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpPost]
        public async Task<IActionResult> IngestEventRequest([FromBody] IngestEventDto ingestEventDto)
        {
            if (ingestEventDto == null)
                return BadRequest("No events provided");

            var cashRegisterEvent = new CashRegisterEvent
            {
                Id = ingestEventDto.Id,
                Date = ingestEventDto.Date,
                UserId = ingestEventDto.UserId,
                EventType = ingestEventDto.EventType,
                Data = ingestEventDto.Data
            };

            await _eventService.CreateAsync(cashRegisterEvent);

            return Ok();
        }
    }
}
