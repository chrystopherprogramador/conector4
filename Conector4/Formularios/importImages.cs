using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;

namespace Conector4.Formularios
{
    public partial class importImages : Form
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public importImages()
        {
            InitializeComponent();
        }

        private void importImages_Load(object sender, EventArgs e)
        {
            // Configurar EPPlus para uso não comercial
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            
            // Configurar HttpClient
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
            
            // Adicionar botão para gerar layout de exemplo
            Button btnLayout = new Button
            {
                Text = "?? Gerar Layout Exemplo",
                Location = new Point(750, 25),
                Size = new Size(190, 30),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };
            btnLayout.Click += BtnGerarLayout_Click;
            groupBox1.Controls.Add(btnLayout);
        }

        private void BtnGerarLayout_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Title = "Salvar Layout de Exemplo";
                    saveFileDialog.Filter = "Arquivo Excel (*.xlsx)|*.xlsx";
                    saveFileDialog.FileName = $"Layout_Imagens_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        GerarLayoutExemplo(saveFileDialog.FileName);

                        MessageBox.Show(
                            $"Layout de exemplo gerado com sucesso!\n\nArquivo salvo em:\n{saveFileDialog.FileName}",
                            "Sucesso",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        var resultado = MessageBox.Show(
                            "Deseja abrir o arquivo agora?",
                            "Abrir Arquivo",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (resultado == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = saveFileDialog.FileName,
                                UseShellExecute = true
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar layout: {ex.Message}",
                              "Erro",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void GerarLayoutExemplo(string caminhoArquivo)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Imagens");

                // Cabeçalhos
                worksheet.Cells[1, 1].Value = "Nome";
                worksheet.Cells[1, 2].Value = "Imagem 1";
                worksheet.Cells[1, 3].Value = "Imagem 2";
                worksheet.Cells[1, 4].Value = "Imagem 3";
                worksheet.Cells[1, 5].Value = "Imagem 4";

                // Estilizar cabeçalho
                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 12;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Exemplos com URLs de teste
                worksheet.Cells[2, 1].Value = "Produto_Exemplo_01";
                worksheet.Cells[2, 2].Value = "https://via.placeholder.com/600/FF0000/FFFFFF?text=Imagem+1";
                worksheet.Cells[2, 3].Value = "https://via.placeholder.com/600/00FF00/FFFFFF?text=Imagem+2";
                worksheet.Cells[2, 4].Value = "https://via.placeholder.com/600/0000FF/FFFFFF?text=Imagem+3";
                worksheet.Cells[2, 5].Value = "https://via.placeholder.com/600/FFFF00/000000?text=Imagem+4";

                worksheet.Cells[3, 1].Value = "Produto_Exemplo_02";
                worksheet.Cells[3, 2].Value = "https://via.placeholder.com/800/800080/FFFFFF?text=Img1";
                worksheet.Cells[3, 3].Value = "https://via.placeholder.com/800/FFA500/FFFFFF?text=Img2";
                worksheet.Cells[3, 4].Value = "https://via.placeholder.com/800/FFC0CB/000000?text=Img3";
                worksheet.Cells[3, 5].Value = "";

                // Ajustar largura das colunas
                worksheet.Column(1).Width = 30;
                worksheet.Column(2).Width = 60;
                worksheet.Column(3).Width = 60;
                worksheet.Column(4).Width = 60;
                worksheet.Column(5).Width = 60;

                // Congelar primeira linha
                worksheet.View.FreezePanes(2, 1);

                // Adicionar instruções
                ExcelWorksheet instrucoes = package.Workbook.Worksheets.Add("Instruções");
                
                instrucoes.Cells[1, 1].Value = "INSTRUÇÕES DE USO";
                instrucoes.Cells[1, 1].Style.Font.Size = 16;
                instrucoes.Cells[1, 1].Style.Font.Bold = true;
                
                instrucoes.Cells[3, 1].Value = "1. ESTRUTURA DO EXCEL:";
                instrucoes.Cells[3, 1].Style.Font.Bold = true;
                instrucoes.Cells[4, 1].Value = "   • Coluna 1 (Nome): Nome da pasta onde as imagens serão salvas";
                instrucoes.Cells[5, 1].Value = "   • Coluna 2-5: URLs das imagens (PNG, JPG, etc)";
                
                instrucoes.Cells[7, 1].Value = "2. FORMATO DAS URLs:";
                instrucoes.Cells[7, 1].Style.Font.Bold = true;
                instrucoes.Cells[8, 1].Value = "   • Devem ser URLs completas começando com http:// ou https://";
                instrucoes.Cells[9, 1].Value = "   • Exemplo: https://exemplo.com/imagens/foto.jpg";
                
                instrucoes.Cells[11, 1].Value = "3. ESTRUTURA DE PASTAS:";
                instrucoes.Cells[11, 1].Style.Font.Bold = true;
                instrucoes.Cells[12, 1].Value = "   • As imagens serão salvas em: PastaRaiz\\Nome\\NomeImagem.jpg";
                instrucoes.Cells[13, 1].Value = "   • Exemplo: C:\\Imagens\\Produto_Exemplo_01\\foto1.jpg";
                
                instrucoes.Cells[15, 1].Value = "4. OBSERVAÇÕES:";
                instrucoes.Cells[15, 1].Style.Font.Bold = true;
                instrucoes.Cells[16, 1].Value = "   • Nem todas as colunas de imagem precisam estar preenchidas";
                instrucoes.Cells[17, 1].Value = "   • O sistema cria automaticamente as pastas necessárias";
                instrucoes.Cells[18, 1].Value = "   • O nome do arquivo será extraído da URL";
                instrucoes.Cells[19, 1].Value = "   • URLs inválidas serão ignoradas e registradas como erro";
                
                instrucoes.Column(1).Width = 80;
                
                // Salvar
                FileInfo file = new FileInfo(caminhoArquivo);
                package.SaveAs(file);
            }
        }

        private async void btnBuscarExcel_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Selecione o arquivo Excel com as URLs das imagens";
                openFileDialog.Filter = "Arquivos Excel (*.xlsx)|*.xlsx|Todos os arquivos (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtCaminhoExcel.Text = openFileDialog.FileName;
                    await CarregarExcelAsync(openFileDialog.FileName);
                }
            }
        }

        private void btnBuscarPastaDestino_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Selecione a pasta raiz onde as imagens serão salvas";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtPastaDestino.Text = folderDialog.SelectedPath;
                }
            }
        }

        private async Task CarregarExcelAsync(string caminhoArquivo)
        {
            try
            {
                btnBuscarExcel.Enabled = false;
                progressBar.Value = 0;

                await Task.Run(() =>
                {
                    FileInfo fileInfo = new FileInfo(caminhoArquivo);

                    using (ExcelPackage package = new ExcelPackage(fileInfo))
                    {
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

                        int totalLinhas = worksheet.Dimension?.Rows ?? 0;
                        int totalColunas = worksheet.Dimension?.Columns ?? 0;

                        if (totalLinhas == 0 || totalColunas < 5)
                        {
                            this.Invoke(new Action(() =>
                            {
                                MessageBox.Show("A planilha deve ter pelo menos 5 colunas (Nome, Imagem1, Imagem2, Imagem3, Imagem4).",
                                              "Aviso",
                                              MessageBoxButtons.OK,
                                              MessageBoxIcon.Warning);
                            }));
                            return;
                        }

                        DataTable dt = new DataTable();

                        // Adicionar colunas
                        dt.Columns.Add("Nome");
                        dt.Columns.Add("Imagem 1");
                        dt.Columns.Add("Imagem 2");
                        dt.Columns.Add("Imagem 3");
                        dt.Columns.Add("Imagem 4");
                        dt.Columns.Add("Status");

                        // Atualizar progresso inicial
                        this.Invoke(new Action(() =>
                        {
                            progressBar.Maximum = totalLinhas - 1;
                            progressBar.Value = 0;
                            lblProgresso.Text = $"0 / {totalLinhas - 1}";
                        }));

                        // Ler dados (pulando cabeçalho)
                        for (int row = 2; row <= totalLinhas; row++)
                        {
                            DataRow dr = dt.NewRow();
                            
                            dr["Nome"] = worksheet.Cells[row, 1].Value?.ToString() ?? string.Empty;
                            dr["Imagem 1"] = worksheet.Cells[row, 2].Value?.ToString() ?? string.Empty;
                            dr["Imagem 2"] = worksheet.Cells[row, 3].Value?.ToString() ?? string.Empty;
                            dr["Imagem 3"] = worksheet.Cells[row, 4].Value?.ToString() ?? string.Empty;
                            dr["Imagem 4"] = worksheet.Cells[row, 5].Value?.ToString() ?? string.Empty;
                            dr["Status"] = "Pendente";

                            dt.Rows.Add(dr);

                            // Atualizar progresso
                            int linhaAtual = row - 1;
                            this.Invoke(new Action(() =>
                            {
                                progressBar.Value = linhaAtual;
                                lblProgresso.Text = $"{linhaAtual} / {totalLinhas - 1}";
                            }));
                        }

                        // Carregar no grid
                        this.Invoke(new Action(() =>
                        {
                            gridResult.DataSource = dt;
                            
                            // Ajustar largura das colunas
                            if (gridResult.Columns.Count > 0)
                            {
                                gridResult.Columns["Nome"].Width = 200;
                                gridResult.Columns["Status"].Width = 150;
                            }

                            lblTotal.Text = $"Total: {dt.Rows.Count} registros";
                            progressBar.Value = progressBar.Maximum;

                            MessageBox.Show($"Arquivo carregado com sucesso!\n\nTotal de registros: {dt.Rows.Count}",
                                          "Sucesso",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Information);
                        }));
                    }
                });
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
                btnBuscarExcel.Enabled = true;
            }
        }

        private async void btnBaixarImagens_Click(object sender, EventArgs e)
        {
            // Validar
            if (gridResult.DataSource == null || gridResult.Rows.Count == 0)
            {
                MessageBox.Show("Carregue um arquivo Excel primeiro.",
                              "Aviso",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPastaDestino.Text))
            {
                MessageBox.Show("Selecione a pasta de destino.",
                              "Aviso",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(txtPastaDestino.Text))
            {
                MessageBox.Show("A pasta de destino não existe.",
                              "Erro",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
                return;
            }

            // Confirmar
            var resultado = MessageBox.Show(
                $"Deseja baixar as imagens para a pasta:\n{txtPastaDestino.Text}?",
                "Confirmar Download",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes)
                return;

            try
            {
                btnBaixarImagens.Enabled = false;
                btnBuscarExcel.Enabled = false;
                btnBuscarPastaDestino.Enabled = false;

                await BaixarTodasImagensAsync();

                MessageBox.Show("Download de imagens concluído!",
                              "Sucesso",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro durante o download: {ex.Message}",
                              "Erro",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
            finally
            {
                btnBaixarImagens.Enabled = true;
                btnBuscarExcel.Enabled = true;
                btnBuscarPastaDestino.Enabled = true;
            }
        }

        private async Task BaixarTodasImagensAsync()
        {
            int totalRegistros = gridResult.Rows.Count;
            int sucesso = 0;
            int falhas = 0;

            progressBar.Value = 0;
            progressBar.Maximum = totalRegistros;

            for (int i = 0; i < totalRegistros; i++)
            {
                var row = gridResult.Rows[i];
                if (row.IsNewRow)
                    continue;

                string nome = row.Cells["Nome"].Value?.ToString() ?? string.Empty;
                
                if (string.IsNullOrWhiteSpace(nome))
                {
                    row.Cells["Status"].Value = "? Nome vazio";
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                    falhas++;
                    continue;
                }

                // Criar pasta do produto
                string pastaProduto = Path.Combine(txtPastaDestino.Text, nome);
                
                try
                {
                    if (!Directory.Exists(pastaProduto))
                    {
                        Directory.CreateDirectory(pastaProduto);
                    }

                    // Baixar as 4 imagens
                    List<string> resultados = new List<string>();
                    int imagensBaixadas = 0;

                    for (int imgIndex = 1; imgIndex <= 4; imgIndex++)
                    {
                        string url = row.Cells[$"Imagem {imgIndex}"].Value?.ToString() ?? string.Empty;

                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            try
                            {
                                bool baixado = await BaixarImagemAsync(url, pastaProduto);
                                if (baixado)
                                {
                                    imagensBaixadas++;
                                    resultados.Add($"? Img{imgIndex}");
                                }
                                else
                                {
                                    resultados.Add($"? Img{imgIndex}");
                                }
                            }
                            catch (Exception ex)
                            {
                                resultados.Add($"? Img{imgIndex}: {ex.Message}");
                            }
                        }
                    }

                    if (imagensBaixadas > 0)
                    {
                        row.Cells["Status"].Value = $"? {imagensBaixadas}/4 - {string.Join(", ", resultados)}";
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        sucesso++;
                    }
                    else
                    {
                        row.Cells["Status"].Value = "?? Nenhuma imagem baixada";
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                        falhas++;
                    }
                }
                catch (Exception ex)
                {
                    row.Cells["Status"].Value = $"? Erro: {ex.Message}";
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                    falhas++;
                }

                // Atualizar progresso
                progressBar.Value = i + 1;
                lblProgresso.Text = $"{i + 1} / {totalRegistros}";
                lblTotal.Text = $"? Sucesso: {sucesso} | ? Falhas: {falhas}";
                
                Application.DoEvents();
            }

            progressBar.Value = progressBar.Maximum;
        }

        private async Task<bool> BaixarImagemAsync(string url, string pastaDestino)
        {
            try
            {
                // Validar URL
                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                    return false;

                // Baixar imagem
                byte[] imageBytes = await _httpClient.GetByteArrayAsync(url);

                // Extrair nome do arquivo da URL
                string nomeArquivo = Path.GetFileName(uri.LocalPath);
                
                // Se não conseguir extrair, usar um nome padrão
                if (string.IsNullOrWhiteSpace(nomeArquivo) || !Path.HasExtension(nomeArquivo))
                {
                    nomeArquivo = $"imagem_{Guid.NewGuid()}.jpg";
                }

                // Caminho completo
                string caminhoCompleto = Path.Combine(pastaDestino, nomeArquivo);

                // Salvar arquivo
                await File.WriteAllBytesAsync(caminhoCompleto, imageBytes);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
