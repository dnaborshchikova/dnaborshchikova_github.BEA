using dnaborshchikova_github.Bea.EventManagement.Core.Models;
using dnaborshchikova_github.Bea.EventManagement.Core.Validators;
using dnaborshchikova_github.Bea.EventManagement.WebApi.Models;
using System.ComponentModel.DataAnnotations;

namespace dnaborshchikova_github.Bea.EventManagement.WebApi.Handlers
{
    public class EventBatchHandler
    {
        private readonly ICashRegisterEventDtoValidator _dtoValidator;
        private readonly ILogger<EventBatchHandler> _logger;

        public EventBatchHandler(ICashRegisterEventDtoValidator dtoValidator, ILogger<EventBatchHandler> logger)
        {
            _dtoValidator = dtoValidator;
            _logger = logger;
        }

        public EventBatchConvertResult ConvertDtoToCashRegisterEvent(List<CashRegisterEventDto> ingestEventDtos)
        {
            var convertResult = new EventBatchConvertResult();

            foreach (var eventDto in ingestEventDtos)
            {
                var validationResult = _dtoValidator.Validate(eventDto);

                if (validationResult != ValidationResult.Success)
                {
                    convertResult.Errors.Add(validationResult.ErrorMessage!);
                    continue;
                }

                var cashRegisterEvent = new CashRegisterEvent(eventDto.Id, eventDto.Date, eventDto.UserId
                    , eventDto.EventType, eventDto.Data);
                convertResult.Events.Add(cashRegisterEvent);
            }

            return convertResult;
        }
    }
}
