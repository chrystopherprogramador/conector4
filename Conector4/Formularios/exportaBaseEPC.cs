using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Conector4.General;
using Npgsql;
using OfficeOpenXml;

namespace Conector4.Formularios
{
    public partial class exportaBaseEPC : Form
    {
        private clsConfig _config;

        public exportaBaseEPC()
        {
            InitializeComponent();
            _config = new clsConfig();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }

        private void exportaBaseEPC_Load(object sender, EventArgs e)
        {
            // Configurações iniciais
            txtLoja.Text = "03";
            lblStatus.Text = "Pronto para exportar";
        }

        private async void btnExportar_Click(object sender, EventArgs e)
        {
            // Validar loja
            if (string.IsNullOrWhiteSpace(txtLoja.Text))
            {
                MessageBox.Show("Informe o código da loja.",
                              "Aviso",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            // Confirmar exportação
            var resultado = MessageBox.Show(
                $"Deseja exportar a base de EPC da loja {txtLoja.Text}?\n\n" +
                $"Os dados serão divididos em 5 planilhas Excel.",
                "Confirmar Exportação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes)
                return;

            // Escolher onde salvar
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Salvar Base de EPC";
                saveFileDialog.Filter = "Arquivo Excel (*.xlsx)|*.xlsx";
                saveFileDialog.FileName = $"Base_EPC_Loja_{txtLoja.Text}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    // Desabilitar botões
                    btnExportar.Enabled = false;
                    txtLoja.Enabled = false;

                    // Iniciar exportação
                    await ExportarBaseEPCAsync(saveFileDialog.FileName, txtLoja.Text);

                    MessageBox.Show(
                        $"Exportação concluída com sucesso!\n\n" +
                        $"Arquivo salvo em:\n{saveFileDialog.FileName}",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // Perguntar se deseja abrir
                    var abrirArquivo = MessageBox.Show(
                        "Deseja abrir o arquivo agora?",
                        "Abrir Arquivo",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (abrirArquivo == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = saveFileDialog.FileName,
                            UseShellExecute = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro durante a exportação:\n{ex.Message}",
                                  "Erro",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                }
                finally
                {
                    // Reabilitar botões
                    btnExportar.Enabled = true;
                    txtLoja.Enabled = true;
                    progressBar.Value = 0;
                    lblStatus.Text = "Pronto para exportar";
                }
            }
        }

        private async Task ExportarBaseEPCAsync(string caminhoArquivo, string loja)
        {
            await Task.Run(async () =>
            {
                this.Invoke((Action)(() =>
                {
                    progressBar.Value = 0;
                    progressBar.Maximum = 100;
                    lblStatus.Text = "Consultando dados...";
                }));

                // 1. BUSCAR DADOS DO BANCO
                DataTable dados = await BuscarDadosBaseEPCAsync(loja);

                if (dados == null || dados.Rows.Count == 0)
                {
                    this.Invoke((Action)(() =>
                    {
                        MessageBox.Show("Nenhum dado encontrado para exportação.",
                                      "Aviso",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Information);
                    }));
                    return;
                }

                this.Invoke((Action)(() =>
                {
                    lblStatus.Text = $"Total de registros: {dados.Rows.Count}";
                    lblTotalRegistros.Text = $"Total: {dados.Rows.Count:N0} registros";
                    progressBar.Value = 20;
                }));

                // 2. DIVIDIR EM 5 PLANILHAS
                int totalRegistros = dados.Rows.Count;
                int registrosPorPlanilha = (int)Math.Ceiling((double)totalRegistros / 5);

                this.Invoke((Action)(() =>
                {
                    lblStatus.Text = $"Criando arquivo Excel com 5 planilhas...";
                    progressBar.Value = 30;
                }));

                // 3. CRIAR ARQUIVO EXCEL
                using (ExcelPackage package = new ExcelPackage())
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int inicio = i * registrosPorPlanilha;
                        int fim = Math.Min(inicio + registrosPorPlanilha, totalRegistros);
                        int registrosNaPlanilha = fim - inicio;

                        if (registrosNaPlanilha <= 0)
                            break;

                        this.Invoke((Action)(() =>
                        {
                            lblStatus.Text = $"Gerando planilha {i + 1} de 5... ({registrosNaPlanilha} registros)";
                            progressBar.Value = 30 + (i * 14); // 30 + (0,1,2,3,4 * 14) = 30,44,58,72,86
                        }));

                        // Criar planilha
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"Dados {i + 1}");

                        // Adicionar cabeçalhos
                        for (int col = 0; col < dados.Columns.Count; col++)
                        {
                            worksheet.Cells[1, col + 1].Value = dados.Columns[col].ColumnName;
                        }

                        // Estilizar cabeçalho
                        using (var range = worksheet.Cells[1, 1, 1, dados.Columns.Count])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Font.Size = 11;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(68, 114, 196));
                            range.Style.Font.Color.SetColor(Color.White);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }

                        // Adicionar dados
                        int excelRow = 2;
                        for (int row = inicio; row < fim; row++)
                        {
                            for (int col = 0; col < dados.Columns.Count; col++)
                            {
                                var valor = dados.Rows[row][col];
                                worksheet.Cells[excelRow, col + 1].Value = valor == DBNull.Value ? "" : valor;
                            }
                            excelRow++;

                            // Atualizar progresso a cada 1000 linhas
                            if ((excelRow - 2) % 1000 == 0)
                            {
                                int progressoAtual = 30 + (i * 14) + ((excelRow - 2) * 14 / registrosNaPlanilha);
                                this.Invoke((Action)(() =>
                                {
                                    progressBar.Value = Math.Min(progressoAtual, 99);
                                }));
                            }
                        }

                        // Auto-ajustar colunas
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Congelar primeira linha
                        worksheet.View.FreezePanes(2, 1);

                        // Adicionar filtro
                        worksheet.Cells[1, 1, excelRow - 1, dados.Columns.Count].AutoFilter = true;
                    }

                    // 4. SALVAR ARQUIVO
                    this.Invoke((Action)(() =>
                    {
                        lblStatus.Text = "Salvando arquivo...";
                        progressBar.Value = 95;
                    }));

                    FileInfo file = new FileInfo(caminhoArquivo);
                    package.SaveAs(file);
                }

                this.Invoke((Action)(() =>
                {
                    progressBar.Value = 100;
                    lblStatus.Text = "Exportação concluída!";
                }));
            });
        }

        private async Task<DataTable> BuscarDadosBaseEPCAsync(string loja)
        {
            DataTable dt = new DataTable();

            try
            {
                await using var conn = new NpgsqlConnection(_config.getConfigBD());
                await conn.OpenAsync();

                string query = @"
                    SELECT 
                        ps.codigo as ""Cod Pai"",
                        ps.sku as ""SKU"",
                        ps.descricao as ""Descrição"",
                        ps.narrativa as ""Narrativa"",
                        ps.unidademedida as ""Unidade de Venda"",
                        m.codigo as ""Cod. Marca"",
                        ps.marca as ""Marca"",
                        ps.departamento as ""Cod. Departamento"",
                        d.descricao as ""Departamento"",
                        ps.grupo as ""Cod. Grupo"",
                        g.descricao as ""Grupo"",
                        ps.subgrupo as ""Cod. SubGrupo"",
                        sg.descricao as ""SubGrupo"",
                        ps.linha as ""Cod. Linha"",
                        l.descricao as ""Linha"",
                        ps.colecao as ""Cod. Coleção"",
                        c.descricao as ""Coleção"",
                        COALESCE(es.quantidade,0) as ""Estoque"",
                        COALESCE(p.reffornecedor,'') as ""Referencia Fornecedor"",
                        COALESCE(ep.codigo_rfid,'') as ""EPC RFID"",
                        ps.status as ""Status""
                    FROM produtos_sku ps 
                    LEFT JOIN estoque es ON es.codigo = ps.sku AND es.loja = @loja
                    LEFT JOIN departamento d ON d.codigo = ps.departamento
                    LEFT JOIN grupos g ON g.codigo = ps.grupo
                    LEFT JOIN subgrupos sg ON sg.codigo = ps.subgrupo
                    LEFT JOIN linhas l ON l.codigo = ps.linha
                    LEFT JOIN colecoes c ON c.codigo = ps.colecao
                    LEFT JOIN marcas m ON UPPER(m.descricao) = UPPER(ps.marca)
                    LEFT JOIN produtos p ON p.codigo = ps.codigo
                    LEFT JOIN grade_produto gp ON gp.codigo = ps.codigo
                    LEFT JOIN grade ga ON ga.codigo = gp.grade
                    INNER JOIN epc ep ON ep.sku = ps.sku
                    ORDER BY ps.codigo, ps.sku";

                await using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@loja", loja);
                cmd.CommandTimeout = 300; // 5 minutos

                await using var reader = await cmd.ExecuteReaderAsync();
                dt.Load(reader);
            }
            catch (Exception ex)
            {
                this.Invoke((Action)(() =>
                {
                    MessageBox.Show($"Erro ao buscar dados:\n{ex.Message}",
                                  "Erro",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                }));
            }

            return dt;
        }
    }
}
