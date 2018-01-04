using System;
using System.ComponentModel.DataAnnotations;

namespace ConfigStore.Api.Dto.Input {
    public class KeyDto {
        [Required]
        public Guid Key { get; set; }
    }
}