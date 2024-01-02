using System.ComponentModel.DataAnnotations.Schema;

namespace JwtWebApi.Dtos
{
    public class CharacterUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lane { get; set; }
        public string Description { get; set; }
        public List<string>? ModifyParams { get; set; }
    }
}
