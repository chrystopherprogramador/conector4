using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Conector4.Presence.Models.PRODUTOS_TAMANHO
{
    public class moProdutosTamanhos
    {

        [JsonProperty("status")]
        public string? status { get; set; } = string.Empty;

        [JsonProperty("method")]
        public string? method { get; set; } = string.Empty;

        [JsonProperty("nextOffset")]
        public int? nextOffset { get; set; } = 0;

        [JsonProperty("data")]
        public List<moProdutosTamanhosRoot> data { get; set; }

    }

    public class moProdutosTamanhosRoot
    {
        [JsonProperty("codigo")]
        public string? codigo { get; set; } = string.Empty;
        [JsonProperty("tampos")]
        public string? posicao { get; set; } = string.Empty;
        [JsonProperty("tamanho")]
        public string tamanho { get; set; } = string.Empty;

    }
}
