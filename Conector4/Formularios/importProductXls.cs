using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Conector4.Formularios
{
    public partial class importProductXls : Form
    {
        public importProductXls()
        {
            InitializeComponent();
        }

        private async void btnBuscar_Click(object sender, EventArgs e)
        {
            // 1 - Procurar arquivo Excel com OpenFileDialog
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Selecione o arquivo Excel";
                openFileDialog.Filter = "Arquivos Excel (*.xlsx)|*.xlsx|Arquivos Excel Antigos (*.xls)|*.xls|Todos os arquivos (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 2 - Colocar o caminho do arquivo em txtCaminho
                        string caminhoArquivo = openFileDialog.FileName;
                        txtCaminho.Text = caminhoArquivo;

                        // Desabilitar botão durante o processamento
                        btnBuscar.Enabled = false;
                        ProgressBar.Value = 0;

                        // 3 - Carregar o arquivo Excel em GridResult
                        await CarregarArquivoExcelAsync(caminhoArquivo);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao carregar arquivo Excel: {ex.Message}",
                                      "Erro",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
                    }
                    finally
                    {
                        btnBuscar.Enabled = true;
                    }
                }
            }
        }


        private async Task CarregarArquivoExcelAsync(string caminhoArquivo)
        {
            await Task.Run(async () =>
            {
                FileInfo fileInfo = new FileInfo(caminhoArquivo);

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    // Pegar a primeira planilha
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    if (worksheet == null)
                    {
                        this.Invoke(new Action(() =>
                        {
                            MessageBox.Show("Nenhuma planilha encontrada no arquivo.",
                                          "Aviso",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Warning);
                        }));
                        return;
                    }

                    // Obter dimensões da planilha
                    int totalLinhas = worksheet.Dimension?.Rows ?? 0;
                    int totalColunas = worksheet.Dimension?.Columns ?? 0;

                    if (totalLinhas == 0 || totalColunas == 0)
                    {
                        this.Invoke(new Action(() =>
                        {
                            MessageBox.Show("A planilha está vazia.",
                                          "Aviso",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Warning);
                        }));
                        return;
                    }

                    // Criar DataTable
                    DataTable dt = new DataTable();

                    // Adicionar colunas baseado na primeira linha (cabeçalho)
                    for (int col = 1; col <= totalColunas; col++)
                    {
                        var cellValue = worksheet.Cells[1, col].Value;
                        string columnName = cellValue?.ToString() ?? $"Coluna{col}";
                        dt.Columns.Add(columnName);
                    }

                    // Atualizar progresso inicial
                    this.Invoke(new Action(() =>
                    {
                        ProgressBar.Maximum = totalLinhas - 1;
                        ProgressBar.Value = 0;
                        txtDe.Text = "1";
                        txtAte.Text = (totalLinhas - 1).ToString();
                    }));

                    int batchSize = 100; // Atualizar UI a cada 100 linhas
                    int processedRows = 0;

                    // Adicionar linhas de dados (pulando a primeira linha que é o cabeçalho)
                    for (int row = 2; row <= totalLinhas; row++)
                    {
                        DataRow dr = dt.NewRow();
                        string descricaoCorreta = string.Empty;

                        for (int col = 1; col <= totalColunas; col++)
                        {
                            var cellValue = worksheet.Cells[row, col].Value;

                            dr[col - 1] = cellValue ?? DBNull.Value;
                        }

                        dt.Rows.Add(dr);
                        processedRows++;

                        // Atualizar progresso EM LOTES (muito mais rápido!)
                        if (processedRows % batchSize == 0 || row == totalLinhas)
                        {
                            int linhaAtual = row - 1;
                            this.Invoke(new Action(() =>
                            {
                                ProgressBar.Value = linhaAtual;
                                txtDe.Text = linhaAtual.ToString();
                            }));
                        }
                    }
                }
            });
        }

        private void importProductXls_Load(object sender, EventArgs e)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }

        private void btnImportar_Click(object sender, EventArgs e)
        {

        }
    }
}
