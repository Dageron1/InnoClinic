namespace InnoClinic.OfficesService.Data;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string OfficesCollectionName { get; set; } = null!;
}
