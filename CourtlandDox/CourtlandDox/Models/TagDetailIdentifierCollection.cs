using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace CourtlandDox.Models
{
    public class TagDetailIdentifierCollection
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [JsonProperty(PropertyName = "DocumentType")]
        public string DocumentType { get; set; }
        [JsonProperty(PropertyName = "NoticeId")]
        public string NoticeId { get; set; }
        [JsonProperty(PropertyName = "NameField")]
        public string NameField { get; set; }
        [JsonProperty(PropertyName = "NameExpression")]
        public string NameExpression { get; set; }
        [JsonProperty(PropertyName = "ValueExpression")]
        public string ValueExpression { get; set; }
    }
}