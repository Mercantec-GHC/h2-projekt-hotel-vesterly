using System.ComponentModel.DataAnnotations;

namespace DomainModels.DB
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        public decimal Price { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public User Customer { get; set; }
        public Room Room { get; set; }
        public List<Extra> Extras { get; set; }
    }
}
