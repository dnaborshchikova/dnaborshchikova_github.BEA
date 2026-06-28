using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace dnaborshchikova_github.Bea.EventManagement.WebApi.Models
{
    public class IngestEventDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string EventType { get; set; }

        [Required]
        public string Data { get; set; }
    }
}
