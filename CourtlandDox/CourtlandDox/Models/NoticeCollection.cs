using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace CourtlandDox.Models
{
    public class NoticeCollection
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [JsonProperty(PropertyName = "DocumentType")]
        public string DocumentType { get; set; }
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
    }

    
}