using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimuladorCaixa.Application.DTO
{
    public class PropostaResumoDTO
    {
        [JsonPropertyName("dataReferencia")]
        [JsonConverter(typeof(JsonDateOnlyConverter))]
        public DateTime dataReferencia { get; set; }
        public List<ResumoDTO> simulacoes { get; set; }
    }

    public class ResumoDTO
    {
        public int codigoProduto { get; set; }
        public string descricaoProduto { get; set; }
        public decimal taxaMediaJuro { get; set; }
        public decimal valorMedioPrestacao { get; set; }
        public decimal valorTotalDesejado { get; set; }
        public decimal valorTotalCredito { get; set; }        
    }

    public class JsonDateOnlyConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-dd";
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), Format, null);
        }
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}
