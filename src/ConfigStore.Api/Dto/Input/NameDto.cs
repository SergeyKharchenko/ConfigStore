using System.ComponentModel.DataAnnotations;

namespace ConfigStore.Api.Dto.Input {
    public class NameDto {
        [Required]
        [MinLength(2)]
        [MaxLength(47)]
        public string Name { get; set; }
    }
}