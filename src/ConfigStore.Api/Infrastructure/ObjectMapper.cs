using System;
using AutoMapper;
using ConfigStore.Api.Data.Models;

namespace ConfigStore.Api.Infrastructure {
    public static class ObjectMapper {
        private static readonly IMapper _mapper;

        static ObjectMapper() {
            var config = new MapperConfiguration(Configure);
            _mapper = config.CreateMapper();
        }

        private static void Configure(IMapperConfigurationExpression configuration) {
            //configuration.CreateMap<Application, ApprovalDto>();
            configuration.CreateMap<Application, object>()
                         .ConstructProjectionUsing(application => new { a = 2 });
        }

        public static TDest Map<TSource, TDest>(TSource source) {
            return _mapper.Map<TSource, TDest>(source);
        }

        public static object Map<TSource>(TSource source) {
            return _mapper.Map<TSource, object>(source);
        }
    }
}