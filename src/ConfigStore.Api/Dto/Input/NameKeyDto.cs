using System;
using System.ComponentModel.DataAnnotations;

namespace ConfigStore.Api.Dto.Input {
    public class NameKeyDto : NameDto {
        [Required]
        public Guid Key { get; set; }
    }
}