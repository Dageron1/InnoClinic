using AutoMapper;
using InnoClinic.OfficesService.Data;
using InnoClinic.OfficesService.DTOs;
using InnoClinic.OfficesService.Entities;
using InnoClinic.OfficesService.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InnoClinic.OfficesService.Services;

public class OfficeService : IOfficeService
{
    private readonly IMongoCollection<Office> _officesCollection;
    private readonly IMapper _mapper;

    public OfficeService(IOptions<MongoDbSettings> mongoDbSettings, IMapper mapper)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

        _officesCollection = mongoDatabase.GetCollection<Office>(mongoDbSettings.Value.OfficesCollectionName);
        _mapper = mapper;
    }

    public async Task<List<Office>> GetAllOfficesAsync()
    {
        return await _officesCollection.Find(_ => true).ToListAsync();
    }
     
    public async Task<Office> GetOfficeByIdAsync(string id)
    {
       return await _officesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateOfficeAsync(OfficeDto officeDto)
    {
        var office = _mapper.Map<Office>(officeDto);
        await _officesCollection.InsertOneAsync(office);
    }

    public async Task UpdateOfficeAsync(string id, OfficeDto officeDto)
    {
        var office = _mapper.Map<Office>(officeDto);
        await _officesCollection.ReplaceOneAsync(x => x.Id == id, office);
    }

    public async Task DeleteOfficeAsync(string id)
    {
        await _officesCollection.DeleteOneAsync(x => x.Id == id);
    }

    public async Task<List<Office>> GetActiveOfficesAsync()
    {
        return await _officesCollection.Find(x => x.IsActive).ToListAsync();
    }
}
