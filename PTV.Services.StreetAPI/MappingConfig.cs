using AutoMapper;
using Street.Services.CrudAPI.Models;
using Street.Services.CrudAPI.Models.Dto;

namespace Street.Services.CrudAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<StreetDto, StreetInfo>();
                config.CreateMap<StreetInfo, StreetDto >();
            });
            return mappingConfig;
        }
    }
}
