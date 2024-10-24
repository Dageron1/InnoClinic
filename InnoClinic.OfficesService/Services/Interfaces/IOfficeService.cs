using InnoClinic.OfficesService.DTOs;
using InnoClinic.OfficesService.Entities;

namespace InnoClinic.OfficesService.Services.Interfaces;

public interface IOfficeService
{
    Task<List<Office>> GetAllOfficesAsync();
    Task<Office> GetOfficeByIdAsync(string id);
    Task CreateOfficeAsync(OfficeDto officeDto);
    Task UpdateOfficeAsync(string id, OfficeDto officeDto);
    Task DeleteOfficeAsync(string id);
    Task<List<Office>> GetActiveOfficesAsync();
}
