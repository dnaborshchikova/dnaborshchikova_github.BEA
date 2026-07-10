using dnaborshchikova_github.Bea.EventManagement.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace dnaborshchikova_github.Bea.EventManagement.Core.Validators
{
    public class CashRegisterEventDtoValidator : ICashRegisterEventDtoValidator
    {
        public ValidationResult Validate(CashRegisterEventDto eventDto)
        {
            var errors = new List<string>();

            if (eventDto.Id == Guid.Empty)
                errors.Add("Id empty");

            if (eventDto.UserId == Guid.Empty)
                errors.Add("UserId empty");

            if (eventDto.Date == default)
                errors.Add("Date invalid");

            return errors.Count == 0
                ? ValidationResult.Success
                : new ValidationResult($"Event {eventDto.Id}: {string.Join(", ", errors)}");
        }
    }
}
