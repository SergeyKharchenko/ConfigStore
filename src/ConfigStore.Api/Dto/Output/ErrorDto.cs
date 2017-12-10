using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigStore.Api.Enums;
using ConfigStore.Api.Extensions;

namespace ConfigStore.Api.Dto.Output {
    public class ErrorDto {
        public ErrorCodes Code { get; set; }

        public string Description { get; set; }

        private ErrorDto() { }

        public static ErrorDto Create(ErrorCodes errorCode) {
            return new ErrorDto {
                Code = errorCode,
                Description = errorCode.GetAttribute<DescriptionAttribute>().Description
            };
        }
    }
}