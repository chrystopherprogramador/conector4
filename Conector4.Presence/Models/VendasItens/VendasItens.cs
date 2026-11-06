using System;
using System.Collections.Generic;
using System.Text;

namespace Conector4.Presence.Models.VendasItens
{
    public class VendasItensResponse
    {
        public string status { get; set; }
        public DateTime serverTime { get; set; }
        public int versionCode { get; set; }
        public string method { get; set; }
        public string nextStartedAt { get; set; }
        public int nextOffSet { get; set; }

        public List<VendaItem> data { get; set; }
    }

    public class VendaItem
    {
        public string situacaoVenda { get; set; }
        public string idCompra { get; set; }
        public int seriePedido { get; set; }
        public int numeroPedido { get; set; }
        public int controleItem { get; set; }

        public string idProduto { get; set; }
        public string nomeProduto { get; set; }

        public decimal quantidadeProduto { get; set; }
        public decimal precoUnitario { get; set; }
        public decimal desconto { get; set; }
        public decimal acrescimo { get; set; }
        public decimal frete { get; set; }
        public decimal seguro { get; set; }
        public decimal outrasDespesas { get; set; }
        public decimal icmsVlrSt { get; set; }
        public decimal fcpValorSt { get; set; }
        public decimal ipiValor { get; set; }
        public decimal precoCusto { get; set; }
        public decimal precoLiquido { get; set; }

        public string transacao { get; set; }

        public string codigoCategoria { get; set; }
        public string categoriaProduto { get; set; }
        public string codigoSubcategoria { get; set; }
        public string subcategoriaProduto { get; set; }

        public string corProduto { get; set; }
        public string tamanhoProduto { get; set; }

        public string idLoja { get; set; }
        public string lojaNome { get; set; }
        public string lojaFantasia { get; set; }

        public string dataCompra { get; set; } // formato BR (03/10/2023)

        public int controle { get; set; }
        public int terminal { get; set; }

        public int vendedorControle { get; set; }
        public string vendedorNome { get; set; }

        public string codigo { get; set; }
        public string codigoLinha { get; set; }
        public string linhaProduto { get; set; }
        public string codigoColecao { get; set; }
        public string colecaoProduto { get; set; }

        public string codigoMarca { get; set; }
        public string marca { get; set; }

        public string referenciaFornecedor { get; set; }

        public decimal fidelidadePontos { get; set; }
        public decimal fidelidadeValor { get; set; }

        public DateTime dataAtualizacao { get; set; }

        public string participaFidelidade { get; set; }

        public int numero { get; set; }
    }
}
