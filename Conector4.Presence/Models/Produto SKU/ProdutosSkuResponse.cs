using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Conector4.Presence.Models.Produto_SKU
{
    public class ProdutosSkuResponse
    {
        [JsonProperty("status")]
        public string? Status { get; set; } = string.Empty;

        [JsonProperty("serverTime")]
        public DateTime? ServerTime { get; set; }

        [JsonProperty("versionCode")]
        public int? VersionCode { get; set; }

        [JsonProperty("method")]
        public string? Method { get; set; } = string.Empty;

        [JsonProperty("nextStartedAt")]
        public DateTime? NextStartedAt { get; set; }

        [JsonProperty("nextOffSet")]
        public int? NextOffSet { get; set; }

        [JsonProperty("data")]
        public List<ProdutoSkuItem>? Data { get; set; } = new();
    }

    public class ProdutoSkuItem
    {
        [JsonProperty("status")]
        public string? Status { get; set; } = string.Empty;

        [JsonProperty("codigo")]
        public string? Codigo { get; set; } = string.Empty;

        [JsonProperty("sku")]
        public string? Sku { get; set; } = string.Empty;

        [JsonProperty("codigoTamanho")]
        public string? CodigoTamanho { get; set; } = string.Empty;

        [JsonProperty("tamanho")]
        public string? Tamanho { get; set; } = string.Empty;

        [JsonProperty("codigoCor")]
        public string? CodigoCor { get; set; } = string.Empty;

        [JsonProperty("cor")]
        public string? Cor { get; set; } = string.Empty;

        [JsonProperty("descricao")]
        public string? Descricao { get; set; } = string.Empty;

        [JsonProperty("narrativa")]
        public string? Narrativa { get; set; } = string.Empty;

        [JsonProperty("unidadeMedida")]
        public string? UnidadeMedida { get; set; } = string.Empty;

        [JsonProperty("composicao")]
        public string? Composicao { get; set; } = string.Empty;

        [JsonProperty("observacao")]
        public string? Observacao { get; set; } = string.Empty;

        [JsonProperty("departamento")]
        public string? Departamento { get; set; } = string.Empty;

        [JsonProperty("grupo")]
        public string? Grupo { get; set; } = string.Empty;

        [JsonProperty("subgrupo")]
        public string? Subgrupo { get; set; } = string.Empty;

        [JsonProperty("linha")]
        public string? Linha { get; set; } = string.Empty;

        [JsonProperty("marca")]
        public string? Marca { get; set; } = string.Empty;

        [JsonProperty("colecao")]
        public string? Colecao { get; set; } = string.Empty;

        [JsonProperty("fornecedor")]
        public int? Fornecedor { get; set; }

        [JsonProperty("altura")]
        public decimal? Altura { get; set; }

        [JsonProperty("largura")]
        public decimal? Largura { get; set; }

        [JsonProperty("comprimento")]
        public decimal? Comprimento { get; set; }

        [JsonProperty("peso")]
        public decimal? Peso { get; set; }

        [JsonProperty("dataCadastro")]
        public DateTime? DataCadastro { get; set; }

        [JsonProperty("dataAtualizacao")]
        public DateTime? DataAtualizacao { get; set; }

        [JsonProperty("codigoAuxiliar")]
        public string? CodigoAuxiliar { get; set; } = string.Empty;
    }
}
