using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DomainModels.DB
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Status status { get; set; }

        [JsonIgnore]
        public List<Message> Messages { get; set; } = new List<Message>();
    }

    public enum Status
    {
        WorkInProgress,
        ClosedCompleted,
        ClosedSkipped
    }
}
