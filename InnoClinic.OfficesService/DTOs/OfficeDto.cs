using MongoDB.Bson.Serialization.Attributes;

namespace InnoClinic.OfficesService.DTOs
{
    public class OfficeDto
    {
        [BsonElement("address")]
        public string Address { get; set; }

        [BsonElement("photo_id")]
        public string PhotoId { get; set; }

        [BsonElement("registry_phone_number")]
        public string RegistryPhoneNumber { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; }
    }
}
