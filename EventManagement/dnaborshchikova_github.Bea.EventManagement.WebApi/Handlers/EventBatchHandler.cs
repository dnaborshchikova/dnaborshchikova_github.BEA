using dnaborshchikova_github.Bea.EventManagement.Core.Models;
using dnaborshchikova_github.Bea.EventManagement.WebApi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace dnaborshchikova_github.Bea.EventManagement.WebApi.Handlers
{
    public class EventBatchHandler
    {
        private readonly ILogger<EventBatchHandler> _logger;

        public EventBatchHandler(ILogger<EventBatchHandler> logger)
        {
            _logger = logger;
        }

        public EventBatchConvertResult ConvertDtoToCashRegisterEvent(List<IngestEventDto> ingestEventDtos)
        {
            var convertResult = new EventBatchConvertResult();

            foreach (var eventDto in ingestEventDtos)
            {
                var error = GetValidationError(eventDto);
                if (!string.IsNullOrEmpty(error))
                {
                    convertResult.Errors.Add(error);
                    continue;
                }                    

                var cashRegisterEvent = new CashRegisterEvent(eventDto.Id, eventDto.Date, eventDto.UserId
                    , eventDto.EventType, eventDto.Data);
                convertResult.Events.Add(cashRegisterEvent);
            }

            return convertResult;
        }

        private string GetValidationError(IngestEventDto eventDto)
        {
            var errors = new List<string>();

            if (eventDto.Id == Guid.Empty)
                errors.Add("Id empty");

            if (eventDto.UserId == Guid.Empty)
                errors.Add("UserId empty");

            if (eventDto.Date == default)
                errors.Add("Date invalid");

            return errors.Count > 0
                ? $"Event {eventDto.Id}: {string.Join(", ", errors)}"
                : string.Empty;
        }
    }
}
