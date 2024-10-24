using AutoMapper;
using InnoClinic.OfficesService.DTOs;
using InnoClinic.OfficesService.Entities;

namespace InnoClinic.OfficesService.Mappers;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<OfficeDto, Office>();
        });
        return mappingConfig;
    }
}
