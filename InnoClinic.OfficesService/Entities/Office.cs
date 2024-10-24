using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace InnoClinic.OfficesService.Entities;

public class Office
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("address")]
    public string? Address { get; set; }

    [BsonElement("photo_id")]
    public string? PhotoId { get; set; }

    [BsonElement("registry_phone_number")]
    public string? RegistryPhoneNumber { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; }
}
