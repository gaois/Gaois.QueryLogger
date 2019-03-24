using AutoMapper;
using System;

namespace Gaois.QueryLogger
{
    internal static class Mapping
    {
        private static readonly Lazy<IMapper> _mapper = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg => {
                // This line ensures that internal properties are also mapped over.
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
                cfg.AddProfile<MappingProfile>();
            });

            return config.CreateMapper();
        });

        public static IMapper Mapper => _mapper.Value;
    }

    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ConfigurationSettings, QueryLoggerSettings>();
        }
    }
}
