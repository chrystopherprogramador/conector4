using Conector4.General.Elastic_Search;
using Conector4.General.Rabbit;
using Conector4.Mensageria;
using Conector4.Presence.Controllers.Estoque;
using Conector4.Presence.Controllers.Fornecedores;
using Conector4.Presence.Controllers.NovaPasta;
using Conector4.Presence.Controllers.PRODUTOS_TAMANHO;
using Conector4.Presence.Controllers.Produtos;
using Conector4.Presence.Global;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Conector4.Presence.Controllers.VendasItens;

namespace Conector4
{
    public class Envio
    {
        RabbitProducer producer = new RabbitProducer();
        private readonly ElasticLogger _logger = new ElasticLogger();

        // Guards to prevent overlapping runs
        private int _isRunningTabPreco = 0;
        private int _isRunningProdutoSku = 0;
        private int _isRunningGrade = 0;

        private Processamento _process = new Processamento();
        // Max pages to process per timer tick to keep cycles short
        private const int MaxPagesPerRun = 100000;


        public Task EnviarPrecoParaFilaAsync()
        {
            return Task.WhenAll(
                _process.ProcessarPaginadoAsync(
                    offset => new crTabPreco().getTabPreco(1, offset),
                    "Presence.TabPreco",
                    "Tabela de Preço 1"
                ),
                _process.ProcessarPaginadoAsync(
                    offset => new crTabPreco().getTabPreco(2, offset),
                    "Presence.TabPreco",
                    "Tabela de Preço 2"
                )
            );
        }

        public Task ProcessarGradeAsync()
        {
            return _process.ProcessarPaginadoAsync(
                offset => new crProdutos_Tamanho().get(offset),
                "Presence.Produtos_Tamanho",
                "Produtos Tamanho"
            );
        }

        public Task EnviarProdutoSkuParaFilaAsync()
        {
            return _process.ProcessarPaginadoAsync(
                offset => new crProdutoSKU().get(offset),
                "Presence.ProdutoSKU",
                "Produtos SKU"
            );
        }

        public Task ProcessaFornecedorAsync()
        {
            return _process.ProcessarPaginadoAsync(
                offset => new crProdutoSKU().get(offset),
                "Presence.TabFornecedores",
                "Processar Fornecedores"
            );
        }

        public Task ProcessarEstoque()
        {
            return Task.WhenAll(
                _process.ProcessarPaginadoAsync(
               offset => new crEstoque().Get("02", offset),
               "Presence.Estoque",
               "Processar Estoques"),
               _process.ProcessarPaginadoAsync(
                offset => new crEstoque().Get("03", offset),
                "Presence.Estoque",
                "Processar Estoques")
               );
        }

        public Task ProcessarProdutosAsync()
        {
            return _process.ProcessarPaginadoAsync(
                offset => new crProdutos().Get(offset),
                "Presence.Produtos",
                "Processar Produtos"
            );
        }

        public Task ProcessarProdutosFaltantesAsync()
        {
            return _process.ProcessarPaginadoAsync(
                offset => new crProdutoSKU().get(offset),
                "Presence.ProcessarProdutosFaltantesAsync",
                "ProcessarProdutosFaltantesAsync"
            );
        }

        public Task ProcessarVendasHeader()
        {
            return _process.ProcessarPaginadoAsync(
                offset => new crVendaHeader().get(offset),
                "Presence.VendasHeader",
                "VendasHeader"
                );
        }

        public Task ProcessarVendasItens()
        {
            return _process.ProcessarPaginadoAsync(
                offset => new crVendasItens().get(offset),
                "Presence.VendasItens",
                "VendasItens"
                );
        }

    }
}
