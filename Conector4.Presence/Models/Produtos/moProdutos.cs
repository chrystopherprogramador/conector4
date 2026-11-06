using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Conector4.Presence.Models.Produtos
{
    public class moProdutos
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("nextOffset")]
        public int NextOffset { get; set; }

        [JsonPropertyName("data")]
        public List<moProduto> Data { get; set; }
    }

    public class moProduto
    {
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }

        [JsonPropertyName("gtin")]
        public string Gtin { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("narrativa")]
        public string Narrativa { get; set; }

        [JsonPropertyName("departamento")]
        public string Departamento { get; set; }

        [JsonPropertyName("grupo")]
        public string Grupo { get; set; }

        [JsonPropertyName("subgrupo")]
        public string Subgrupo { get; set; }

        [JsonPropertyName("linha")]
        public string Linha { get; set; }

        [JsonPropertyName("marca")]
        public int Marca { get; set; }

        [JsonPropertyName("colecao")]
        public string Colecao { get; set; }

        [JsonPropertyName("unidadeMedida")]
        public string UnidadeMedida { get; set; }

        [JsonPropertyName("fornecedor")]
        public string Fornecedor { get; set; }

        [JsonPropertyName("composicao")]
        public string Composicao { get; set; }

        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }

        [JsonPropertyName("observacao")]
        public string Observacao { get; set; }

        [JsonPropertyName("dataCadastro")]
        public DateTime DataCadastro { get; set; }

        [JsonPropertyName("altura")]
        public double Altura { get; set; }

        [JsonPropertyName("largura")]
        public double Largura { get; set; }

        [JsonPropertyName("comprimento")]
        public double Comprimento { get; set; }

        [JsonPropertyName("peso")]
        public double Peso { get; set; }

        [JsonPropertyName("participaFidelidade")]
        public string ParticipaFidelidade { get; set; }
    }
}
