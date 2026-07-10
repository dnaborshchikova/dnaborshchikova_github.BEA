using dnaborshchikova_github.Bea.EventManagement.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace dnaborshchikova_github.Bea.EventManagement.Core.Validators
{
    public interface ICashRegisterEventDtoValidator
    {
        ValidationResult Validate(CashRegisterEventDto dto);
    }
}
