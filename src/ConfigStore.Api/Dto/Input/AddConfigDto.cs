using System.ComponentModel.DataAnnotations;

namespace ConfigStore.Api.Dto.Input {
    public class AddConfigDto {
        [Required]
        [MaxLength(47)]
        public string ConfigName { get; set; }

        [Required]
        public string ConfigValue { get; set; }
    }
}