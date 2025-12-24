using Conector4.General;
using Conector4.General.Elastic_Search;
using Conector4.General.Rabbit;
using Conector4.Mensageria;
using Conector4.Presence.Controllers.NovaPasta;
using Conector4.Presence.Controllers.PRODUTOS_TAMANHO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ScottPlot.WinForms;
namespace Conector4
{
    public partial class MDI : Form
    {


        Fn fn = new Fn();


        RabbitProducer producer = new RabbitProducer();
        clsFila Fila = new clsFila();
        private readonly ElasticLogger _logger = new ElasticLogger();
        private clsConfig _clsConfig = new clsConfig();

        // Guards to prevent overlapping runs
        private int _isRunningTabPreco = 0;
        private int _isRunningProdutoSku = 0;
        private int _isRunningGrade = 0;
        // Max pages to process per timer tick to keep cycles short
        private const int MaxPagesPerRun = 100000;

        // buffers de 30 minutos (1800 segundos)
        private double[] processados = new double[1800];
        private double[] falhados = new double[1800];
        private double[] andamento = new double[1800];
        private int index = 0;

        // referência ao FormsPlot e às séries
        private ScottPlot.WinForms.FormsPlot formsPlot;
        private object spProcessados;
        private object spFalhados;
        private object spAndamento;


        public MDI()
        {
            InitializeComponent();
        }

        private async void tmRabiit_Tick(object sender, EventArgs e)
        {

            await Fila.fila();
        }



        private async void MDI_Load(object sender, EventArgs e)
        {
            cmbServicoMensageria.Items.Clear();
            cmbServicoMensageria.Items.Add("Presence.TabPreco");
            cmbServicoMensageria.Items.Add("Presence.TabPrecoInd");
            cmbServicoMensageria.Items.Add("Presence.ProdutoSKU");
            cmbServicoMensageria.Items.Add("Presence.Produtos_Tamanho");
            await InitTimersFromDb();
            var ds = await fn.returnServico(); GridServico.DataSource = ds.Tables[0];

            formsPlot = new ScottPlot.WinForms.FormsPlot
            {
                Dock = DockStyle.Fill
            };
            pnGrafico.Controls.Add(formsPlot);

            spProcessados = formsPlot.Plot.Add.Signal(processados);
            ((dynamic)spProcessados).Label = "Processados";

            spFalhados = formsPlot.Plot.Add.Signal(falhados);
            ((dynamic)spFalhados).Label = "Falhados";

            spAndamento = formsPlot.Plot.Add.Signal(andamento);
            ((dynamic)spAndamento).Label = "Em Andamento";

            formsPlot.Plot.Legend.IsVisible = true;
            formsPlot.Refresh();



        }


        // Dicionário para controlar execução por nome de serviço
        // Dicionário de locks por serviço
        private readonly Dictionary<string, SemaphoreSlim> _serviceLocks = new();

        private async Task InitTimersFromDb()
        {
            clsConfig confg = new clsConfig();
            using var conn = new NpgsqlConnection(confg.getConfigBD());
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand("SELECT nome, ativo, time, classe, metodo FROM servicos", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string nome = reader.GetString(0);
                bool ativo = reader.GetInt32(1) == 1;
                int intervalo = reader.GetInt32(2);
                string classe = reader.GetString(3);
                string metodo = reader.GetString(4);

                if (!ativo) continue;

                // Cria lock para este serviço
                _serviceLocks[nome] = new SemaphoreSlim(1, 1);

                var timer = new System.Windows.Forms.Timer();
                timer.Interval = Math.Max(500, intervalo * 1000);
                timer.Tick += async (s, e) =>
                {
                    var sem = _serviceLocks[nome];

                    // Tenta entrar sem esperar (se já está rodando, ignora)
                    if (!await sem.WaitAsync(0))
                    {
                        await _logger.LogAsync("Info", $"Timer {nome} ignorado porque já está em execução");
                        return;
                    }

                    try
                    {
                        await _logger.LogAsync("Info", $"Timer {nome} disparado");

                        var type2 = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .FirstOrDefault(t => t.FullName == classe);

                        if (type2 == null)
                        {
                            await _logger.LogAsync("Error", $"Classe {classe} não encontrada");
                            return;
                        }

                        var instance = Activator.CreateInstance(type2);

                        var method = type2.GetMethod(metodo);
                        if (method == null)
                        {
                            await _logger.LogAsync("Error", $"Método {metodo} não encontrado na classe {classe}");
                            return;
                        }

                        var task = (Task)method.Invoke(instance, null);
                        await task;
                    }
                    catch (Exception ex)
                    {
                        await _logger.LogAsync("Error", $"Timer {nome} falhou: {ex.Message}");
                    }
                    finally
                    {
                        sem.Release(); // libera lock
                    }
                };
                timer.Start();
            }
        }




        private async void btnBuscar_Click(object sender, EventArgs e)
        {
            var msgs = await Fila.GetQueueMessagesAsync(cmbServicoMensageria.Text);
            gridMensageria.AutoGenerateColumns = true;

            gridMensageria.DataSource = msgs.Select(x => new
            {
                TamanhoBytes = x.payload_bytes,
                Reentregue = x.redelivered,
                QtdMensagens = x.message_count,
                DeliveryMode = x.properties != null && x.properties.ContainsKey("delivery_mode")
                    ? x.properties["delivery_mode"]
                    : null,
                Encoding = "string",
                Payload = string.IsNullOrEmpty(x.payload)
                            ? ""
                            : x.payload.Length > 5000
                                ? x.payload.Substring(0, 5000) + "..."
                                : x.payload
            }).ToList();
        }

        private async void tmTela_Tick(object sender, EventArgs e)
        {

            var snapshot = Metrics.Snapshot();
            var rows = snapshot.Select(kv => new
            {
                Fila = kv.Key,
                Processados = kv.Value.Processed,
                Falhados = kv.Value.Failed,
                EmAndamento = kv.Value.InFlight
            }).ToList();

            gridMetrics.AutoGenerateColumns = true;
            gridMetrics.DataSource = rows;



            processados[index % 1800] = rows.Sum(r => r.Processados);
            falhados[index % 1800] = rows.Sum(r => r.Falhados);
            andamento[index % 1800] = rows.Sum(r => r.EmAndamento);

            index++;

            formsPlot.Refresh();



            if (cmbServicoMensageria.SelectedIndex == -1)
            {
                return;
            }

            var msgs = await Fila.GetQueueMessagesAsync(cmbServicoMensageria.Text);
            gridMensageria.AutoGenerateColumns = true;

            gridMensageria.DataSource = msgs.Select(x => new
            {
                TamanhoBytes = x.payload_bytes,
                Reentregue = x.redelivered,
                QtdMensagens = x.message_count,
                DeliveryMode = x.properties != null && x.properties.ContainsKey("delivery_mode")
                    ? x.properties["delivery_mode"]
                    : null,
                Encoding = "string",
                Payload = string.IsNullOrEmpty(x.payload)
                            ? ""
                            : x.payload.Length > 5000
                                ? x.payload.Substring(0, 5000) + "..."
                                : x.payload
            }).ToList();


        }

        private async Task refreshDadosLogs()
        {
            var logger = new ElasticLogger();

            // Pega os textos dos TextBox
            string dataInicial = txtInicial.Text.Trim();
            string dataFinal = txtFinal.Text.Trim();
            string status = string.IsNullOrWhiteSpace(cmbStatus.Text) ? null : cmbStatus.Text.Trim();
            string keyword = string.IsNullOrWhiteSpace(txtKeyword.Text) ? null : txtKeyword.Text.Trim();

            // Valida datas obrigatórias
            if (!DateTime.TryParse(dataInicial, out DateTime dataConvertidaInicial))
            {
                MessageBox.Show("Data inicial inválida. Informe no formato dd/MM/yyyy.");
                return;
            }

            if (!DateTime.TryParse(dataFinal, out DateTime dataConvertidaFinal))
            {
                MessageBox.Show("Data final inválida. Informe no formato dd/MM/yyyy.");
                return;
            }

            // Valida size obrigatório
            if (!int.TryParse(txtSize.Text, out int size) || size <= 0)
            {
                MessageBox.Show("Tamanho inválido. Informe um número inteiro maior que zero.");
                return;
            }

            // Formata datas no padrão yyyy-MM-dd
            string dataFormatadaInicial = dataConvertidaInicial.ToString("yyyy-MM-dd");
            string dataFormatadaFinal = dataConvertidaFinal.ToString("yyyy-MM-dd");

            // Chama o método de busca
            var logs = await logger.GetLogsByDateAsync(dataFormatadaInicial, dataFormatadaFinal, status, keyword, size);

            // Exibe no grid
            GridLogs.DataSource = logs;
        }
        private async void btnBuscarLog_Click(object sender, EventArgs e)
        {
            await refreshDadosLogs();
        }


        private void btnRefreshMetrics_Click(object sender, EventArgs e)
        {
            var snapshot = Metrics.Snapshot();
            var rows = snapshot.Select(kv => new
            {
                Fila = kv.Key,
                Processados = kv.Value.Processed,
                Falhados = kv.Value.Failed,
                EmAndamento = kv.Value.InFlight
            }).ToList();

            gridMetrics.AutoGenerateColumns = true;
            gridMetrics.DataSource = rows;
        }

        private void txtSize_Leave(object sender, EventArgs e)
        {
            if (txtSize.Text == string.Empty)
            {
                txtSize.Text = "5000";
            }
        }

        private async void tmRefreshLogs_Tick(object sender, EventArgs e)
        {
            await refreshDadosLogs();
        }

        private void chcRefresh_CheckedChanged(object sender, EventArgs e)
        {
            if (chcRefresh.Checked)
            {
                tmRefreshLogs.Enabled = true;
            }
            else
            {
                tmRefreshLogs.Enabled = false;
            }
        }




        private async void btnGravar_Click(object sender, EventArgs e)
        {
            try
            {
                // Captura os valores dos campos
                int codigo = string.IsNullOrWhiteSpace(txtCodigo.Text) ? 0 : int.Parse(txtCodigo.Text);
                string nome = txtNomeServico.Text;
                bool ativo = chAtivoServico.Checked;
                string classe = txtClasse.Text;
                string metodo = txtMetodo.Text;

                var _config = new clsConfig();

                await using var conn = new NpgsqlConnection(_config.getConfigBD());
                await conn.OpenAsync();

                string sql;

                if (codigo == 0) // Se não tiver código → INSERT
                {
                    sql = @"
                INSERT INTO servicos (nome, ativo, classe, metodo)
                VALUES (@nome, @ativo, @classe, @metodo);
            ";
                }
                else // Se tiver código → UPDATE
                {
                    sql = @"
                UPDATE servicos
                SET nome = @nome,
                    ativo = @ativo,
                    classe = @classe,
                    metodo = @metodo
                WHERE id = @id;
            ";
                }

                await using var cmd = new NpgsqlCommand(sql, conn);

                // Parâmetros comuns
                cmd.Parameters.AddWithValue("nome", nome);
                cmd.Parameters.AddWithValue("ativo", ativo);
                cmd.Parameters.AddWithValue("classe", classe);
                cmd.Parameters.AddWithValue("metodo", metodo);

                if (codigo != 0)
                    cmd.Parameters.AddWithValue("id", codigo);

                await cmd.ExecuteNonQueryAsync();

                MessageBox.Show(codigo == 0
                    ? "Registro inserido com sucesso!"
                    : "Registro atualizado com sucesso!");
                var ds = await fn.returnServico(); GridServico.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao gravar registro: " + ex.Message);
            }
        }

        private async void btnRefreshSer_ClickAsync(object sender, EventArgs e)
        {
            var ds = await fn.returnServico(); GridServico.DataSource = ds.Tables[0];
        }
        private void GridServico_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = GridServico.Rows[e.RowIndex];

                txtCodigo.Text = row.Cells["Codigo"].Value?.ToString();
                txtNomeServico.Text = row.Cells["Serviço"].Value?.ToString();
                chAtivoServico.Checked = row.Cells["Ativo"].Value != null &&
                                         bool.TryParse(row.Cells["Ativo"].Value.ToString(), out bool ativo) && ativo;
                txtClasse.Text = row.Cells["Classe"].Value?.ToString();
                txtMetodo.Text = row.Cells["Metodo"].Value?.ToString();
            }
        }

        private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
