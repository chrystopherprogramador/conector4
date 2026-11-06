using System;
using System.Collections.Generic;
using System.Text;

namespace Conector4.Presence.Models.VendasHeader
{

    public class VendasHeader
    {
        public string status { get; set; }
        public DateTime serverTime { get; set; }
        public int versionCode { get; set; }
        public string method { get; set; }
        public string nextStartedAt { get; set; }
        public int nextOffSet { get; set; }

        public List<Venda> data { get; set; }
    }
    public class Venda
    {

        public string situacaoVenda { get; set; }
        public string clienteCodigo { get; set; }
        public string clienteNome { get; set; }
        public string clienteFantasia { get; set; }
        public string clienteEmail { get; set; }
        public string cpfCnpj { get; set; }
        public string clientePessoa { get; set; }
        public string clienteSexo { get; set; }
        public string clienteNascimento { get; set; }
        public string clienteTelefone { get; set; }
        public string clienteEndereco { get; set; }
        public string clienteNumero { get; set; }
        public string clienteComplemento { get; set; }
        public string clienteBairro { get; set; }
        public string clienteCidade { get; set; }
        public string clienteEstado { get; set; }
        public string clienteCep { get; set; }

        public string idLoja { get; set; }
        public DateTime dataCompra { get; set; }

        public string? codigoTransportadora { get; set; }
        public string? fantasiaTransportadora { get; set; }

        public int controle { get; set; }
        public int terminal { get; set; }
        public string idCompra { get; set; }

        public string nomeLoja { get; set; }
        public string nomeFantasiaLoja { get; set; }

        public int codigoVendedor { get; set; }
        public string nomeVendedor { get; set; }

        public decimal subtotal { get; set; }
        public decimal totalDesconto { get; set; }
        public decimal totalFrete { get; set; }
        public decimal totalSeguro { get; set; }
        public decimal totalOutrasDespesas { get; set; }
        public decimal totalIcmsValorSt { get; set; }
        public decimal totalFcpValorSt { get; set; }
        public decimal totalIpi { get; set; }
        public decimal total { get; set; }

        public int fidelidadePontos { get; set; }
        public decimal fidelidadeValor { get; set; }

        public DateTime dataAt { get; set; }
        public string integracaoNoPedido { get; set; }
        public string tipoLoja { get; set; }
        public string operacaoProduto { get; set; }
        public string tipoNota { get; set; }
        public string modelo { get; set; }
        public string modeloDescricao { get; set; }

        public int quantidadeTotalProduto { get; set; }
        public int numero { get; set; }


    }
}
