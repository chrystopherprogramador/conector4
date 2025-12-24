using Newtonsoft.Json;
using System;

namespace Conector4.Presence.Models.TabPreco
{

    public class moTabPreco
    {
        [JsonProperty("tabelaPreco")]
        public int Tabela_Preco { get; set; }
    }
    public class moTabPrecoRec
    {
        [JsonProperty("codigo")]
        public string Cod { get; set; } = string.Empty;

        [JsonProperty("tabelaPreco")]
        public int TabelaPreco { get; set; }

        [JsonProperty("precoCusto")]
        public decimal Preco_Custo { get; set; }

        [JsonProperty("precoVarejo")]
        public decimal Preco_Varejo { get; set; }

        [JsonProperty("precoAtacado")]
        public decimal Preco_Atacado { get; set; }

        [JsonProperty("precoRepresentante")]
        public decimal Preco_Representante { get; set; }

        [JsonProperty("precoVarejoPromocional")]
        public decimal Preco_Varejo_Promocional { get; set; }

        [JsonProperty("precoAtacadoPromocional")]
        public decimal Preco_Atacado_Promocional { get; set; }

        [JsonProperty("precoRepresentantePromocional")]
        public decimal Preco_Representante_Promocional { get; set; }

        [JsonProperty("dataInicio")]
        public DateTime Data_Inicio { get; set; }

        [JsonProperty("dataFinal")]
        public DateTime Data_Final { get; set; }
    }
}
