using System.ComponentModel.DataAnnotations;

namespace ConfigStore.Api.Dto.Input {
    public class ApplicationDto {
        [Required]
        [MinLength(3)]
        [MaxLength(16)]
        public string Name { get; set; }
    }
}