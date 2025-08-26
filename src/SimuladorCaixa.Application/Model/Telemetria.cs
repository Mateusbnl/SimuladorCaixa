using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace SimuladorCaixa.Application.Model
{
    public class Telemetria
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("endpoint")]
        public string endpoint { get; set; }

        [JsonPropertyName("metodo")]
        public string metodo { get; set; }

        [JsonPropertyName("statusCode")]
        public int statusCode { get; set; }

        [JsonPropertyName("sucesso")]
        public bool sucesso { get; set; }

        [JsonPropertyName("tempo")]
        public long tempo { get; set; }

        [JsonPropertyName("dataHora")]
        public DateTime dataHora { get; set; } = DateTime.Now;
    }
}
