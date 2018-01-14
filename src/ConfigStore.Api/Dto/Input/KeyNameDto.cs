using System;
using System.ComponentModel.DataAnnotations;

namespace ConfigStore.Api.Dto.Input {
    public class KeyNameDto {
        [Required]
        public Guid Key { get; set; }
        
        [Required]
        [MinLength(2)]
        [MaxLength(47)]
        public string Name { get; set; }
    }
}