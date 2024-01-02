using System.ComponentModel.DataAnnotations.Schema;

namespace JwtWebApi.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lane { get; set; }
        public string Description { get; set; }
        [NotMapped()]
        public List<string>? ModifyParams { get; set; }
        public ICollection<Counter>? Counters { get; set; }
        public ICollection<CountedBy>? CountedBy { get; set; }

    }
    public class Counter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CharacterId { get; set; }
        public Character Character { get; set; }
    }

    public class CountedBy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CharacterId { get; set; }
        public Character Character { get; set; }
    }
}
