using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Conector4.Presence.Models.Fornecedores
{
    public class moFornecedores
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("data")]
        public List<moFornecedorData> Data { get; set; }
    }

    public class moFornecedorData
    {
        [JsonPropertyName("codigo")]
        public int Codigo { get; set; }

        [JsonPropertyName("fantasia")]
        public string Fantasia { get; set; }

        [JsonPropertyName("razaoSocial")]
        public string RazaoSocial { get; set; }

        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; }
    }

}
