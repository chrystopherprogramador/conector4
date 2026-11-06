using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using OfficeOpenXml;
using Conector4.Controls;
using Conector4.General;
using Npgsql;
using Conector4.Presence.Controllers.Produtos;
using Conector4.Presence.Controllers.NovaPasta;
using Conector4.Presence.Controllers.Tabela_de_Preco;

namespace Conector4.Formularios
{
    public partial class importProduts : Form
    {
        private clsConfig _config;

        public importProduts()
        {
            InitializeComponent();
            _config = new clsConfig();
        }

        private void importProduts_Load(object sender, EventArgs e)
        {
            // Configurar EPPlus para uso não comercial
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
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

        // Cache de marcas para evitar múltiplas consultas ao banco
        private Dictionary<string, (string Codigo, string Descricao)> _cacheMarcas = new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase);

        private async Task CarregarCacheMarcasAsync()
        {
            try
            {
                _cacheMarcas.Clear();
                
                await using var conn = new NpgsqlConnection(_config.getConfigBD());
                await conn.OpenAsync();

                string query = "SELECT codigo, descricao FROM marcas";
                await using var cmd = new NpgsqlCommand(query, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    string codigo = reader["codigo"]?.ToString() ?? string.Empty;
                    string descricao = reader["descricao"]?.ToString() ?? string.Empty;
                    
                    if (!string.IsNullOrEmpty(descricao))
                    {
                        _cacheMarcas[descricao] = (codigo, descricao);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Erro ao carregar cache de marcas: {ex.Message}");
            }
        }

        private (string Codigo, string Descricao) BuscarCodigoMarcaNoCache(string descricaoMarca)
        {
            if (string.IsNullOrWhiteSpace(descricaoMarca))
                return (string.Empty, string.Empty);

            // Busca exata
            if (_cacheMarcas.TryGetValue(descricaoMarca, out var resultado))
                return resultado;

            // Busca parcial (ILIKE simulado)
            var chave = descricaoMarca.Trim().ToLower();
            foreach (var kvp in _cacheMarcas)
            {
                if (kvp.Key.ToLower().Contains(chave))
                    return kvp.Value;
            }

            return (string.Empty, string.Empty);
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

                    // Carregar cache de marcas ANTES do loop
                    await CarregarCacheMarcasAsync();

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

                            // Se coluna 5 estiver vazia e coluna 6 tiver valor
                            if (col == 5 && (cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString())))
                            {
                                // Pegar valor da coluna 6
                                var coluna6Value = worksheet.Cells[row, 6].Value;

                                if (coluna6Value != null && !string.IsNullOrWhiteSpace(coluna6Value.ToString()))
                                {
                                    // Buscar código e descrição da marca NO CACHE (muito mais rápido!)
                                    var (codigoMarca, descricaoMarca) = BuscarCodigoMarcaNoCache(coluna6Value.ToString());

                                    if (!string.IsNullOrEmpty(codigoMarca))
                                    {
                                        dr[col - 1] = codigoMarca;
                                        descricaoCorreta = descricaoMarca;
                                    }
                                    else
                                    {
                                        dr[col - 1] = DBNull.Value;
                                    }
                                }
                                else
                                {
                                    dr[col - 1] = DBNull.Value;
                                }
                            }
                            // Se for coluna 6 e temos uma descrição correta do banco
                            else if (col == 6 && !string.IsNullOrEmpty(descricaoCorreta))
                            {
                                dr[col - 1] = descricaoCorreta;
                            }
                            else
                            {
                                dr[col - 1] = cellValue ?? DBNull.Value;
                            }
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

                    // Atualizar Grid e totalizador na thread da UI
                    this.Invoke(new Action(() =>
                    {
                        // Carregar dados no GridResult
                        gridResult.DataSource = dt;

                        // Marcar colunas importantes para importação
                        MarcarColunasImportantes();

                        // Totalizar registros em disTotal (total de linhas - 1 linha de cabeçalho)
                        int totalRegistros = totalLinhas - 1;
                        disTotal.Text = totalRegistros.ToString();

                        // Completar barra de progresso
                        ProgressBar.Value = ProgressBar.Maximum;

                        MessageBox.Show($"Arquivo carregado com sucesso!\n\nTotal de registros: {totalRegistros}",
                                      "Sucesso",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Information);
                    }));
                }
            });
        }

        private async void btnImportar_Click(object sender, EventArgs e)
        {
            // Verificar se há dados no grid
            if (gridResult.DataSource == null || gridResult.Rows.Count == 0)
            {
                MessageBox.Show("Não há dados para importar. Carregue um arquivo Excel primeiro.",
                              "Aviso",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            // Criar formulário personalizado para escolha
            using (var form = new Form())
            {
                form.Text = "Escolher Ação de Importação";
                form.Size = new Size(500, 290);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label
                {
                    Text = "Escolha o tipo de importação:",
                    Location = new Point(20, 20),
                    AutoSize = true,
                    Font = new Font("Arial", 10, FontStyle.Bold)
                };

                var rbBanco = new RadioButton
                {
                    Text = "Importar para o Banco de Dados (atualização em massa - produtos_sku)",
                    Location = new Point(20, 50),
                    Width = 450,
                    Checked = true
                };

                var rbRabbitSimples = new RadioButton
                {
                    Text = "Buscar na API e Publicar no RabbitMQ (Apenas Produtos)",
                    Location = new Point(20, 80),
                    Width = 450
                };

                var rbProcessarCompleto = new RadioButton
                {
                    Text = "Buscar na API e Processar DIRETO NO BANCO (Produtos + SKU + Estoque)",
                    Location = new Point(20, 110),
                    Width = 450
                };

                var rbAtualizarPrecos = new RadioButton
                {
                    Text = "Atualizar Preço de Todos os Produtos (via API e Banco)",
                    Location = new Point(20, 140),
                    Width = 450
                };

                var txtLoja = new TextBox
                {
                    Text = "01",
                    Location = new Point(270, 165),
                    Width = 50
                };

                var lblLoja = new Label
                {
                    Text = "Código da Loja (para estoque):",
                    Location = new Point(20, 168),
                    AutoSize = true
                };

                var btnOk = new Button
                {
                    Text = "Executar",
                    Location = new Point(250, 200),
                    DialogResult = DialogResult.OK,
                    Width = 100
                };

                var btnCancelar = new Button
                {
                    Text = "Cancelar",
                    Location = new Point(360, 200),
                    DialogResult = DialogResult.Cancel,
                    Width = 100
                };

                form.Controls.AddRange(new System.Windows.Forms.Control[] { label, rbBanco, rbRabbitSimples, rbProcessarCompleto, rbAtualizarPrecos, lblLoja, txtLoja, btnOk, btnCancelar });
                form.AcceptButton = btnOk;
                form.CancelButton = btnCancelar;

                if (form.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    // Desabilitar botões
                    btnBuscar.Enabled = false;
                    btnImportar.Enabled = false;

                    if (rbBanco.Checked)
                    {
                        // IMPORTAR PARA BANCO DE DADOS
                        ProgressBar.Value = 0;
                        ProgressBar.Maximum = gridResult.Rows.Count;
                        txtDe.Text = "0";
                        txtAte.Text = gridResult.Rows.Count.ToString();

                        await ImportarParaBancoDadosAsync();

                        MessageBox.Show("Importação para banco concluída com sucesso!",
                                      "Sucesso",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Information);
                    }
                    else if (rbRabbitSimples.Checked)
                    {
                        // BUSCAR NA API E PUBLICAR NO RABBITMQ (APENAS PRODUTOS)
                        await BuscarEPublicarNoRabbitMQAsync();
                    }
                    else if (rbProcessarCompleto.Checked)
                    {
                        // BUSCAR NA API E PROCESSAR DIRETO NO BANCO (PRODUTOS + SKU + ESTOQUE)
                        await BuscarEPublicarCompletoAsync(txtLoja.Text);
                    }
                    else if (rbAtualizarPrecos.Checked)
                    {
                        // ATUALIZAR PREÇOS DE TODOS OS PRODUTOS
                        await AtualizarPrecosAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro durante a operação: {ex.Message}",
                                  "Erro",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                }
                finally
                {
                    // Reabilitar botões
                    btnBuscar.Enabled = true;
                    btnImportar.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Lê códigos da coluna 1 do grid, busca na API e publica no RabbitMQ
        /// </summary>
        private async Task BuscarEPublicarNoRabbitMQAsync()
        {
            // Coletar códigos da PRIMEIRA coluna (índice 0)
            List<string> codigos = new List<string>();
            
            this.Invoke(() =>
            {
                for (int i = 0; i < gridResult.Rows.Count; i++)
                {
                    var row = gridResult.Rows[i];
                    if (!row.IsNewRow)
                    {
                        string codigo = row.Cells[0].Value?.ToString(); // Coluna 1 (índice 0)
                        
                        if (!string.IsNullOrWhiteSpace(codigo))
                        {
                            codigos.Add(codigo.Trim());
                        }
                    }
                }
            });

            if (codigos.Count == 0)
            {
                MessageBox.Show("Nenhum código encontrado na primeira coluna do grid.",
                              "Aviso",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            // Confirmar com o usuário
            var resultado = MessageBox.Show(
                $"Encontrados {codigos.Count} códigos na primeira coluna.\n\n" +
                $"Deseja buscar estes produtos na API e publicar na fila 'Presence.Produtos'?",
                "Confirmar Publicação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes)
                return;

            // Configurar progresso
            ProgressBar.Value = 0;
            ProgressBar.Maximum = codigos.Count;
            txtDe.Text = "0";
            txtAte.Text = codigos.Count.ToString();

            // Criar controller
            var controller = new crProdutosCod();

            // Progresso handler
            var progress = new Progress<(int Atual, int Total, string Codigo)>(tuple =>
            {
                this.Invoke(() =>
                {
                    ProgressBar.Value = tuple.Atual;
                    txtDe.Text = tuple.Atual.ToString();
                    
                    // Destacar linha no grid
                    for (int i = 0; i < gridResult.Rows.Count; i++)
                    {
                        if (gridResult.Rows[i].Cells[0].Value?.ToString() == tuple.Codigo)
                        {
                            gridResult.ClearSelection();
                            gridResult.Rows[i].Selected = true;
                            gridResult.FirstDisplayedScrollingRowIndex = Math.Max(0, i - 5);
                            break;
                        }
                    }
                });
            });

            // Executar busca e publicação
            var (sucesso, falhas, codigosComErro) = await controller.BuscarEPublicarCodigos(
                codigos,
                "Presence.Produtos",
                progress);

            // Colorir linhas no grid baseado no resultado
            this.Invoke(() =>
            {
                foreach (DataGridViewRow row in gridResult.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        string codigo = row.Cells[0].Value?.ToString();
                        
                        if (!string.IsNullOrEmpty(codigo))
                        {
                            if (codigosComErro.Contains(codigo))
                            {
                                row.DefaultCellStyle.BackColor = Color.LightCoral; // Erro
                            }
                            else
                            {
                                row.DefaultCellStyle.BackColor = Color.LightGreen; // Sucesso
                            }
                        }
                    }
                }

                ProgressBar.Value = ProgressBar.Maximum;
                gridResult.ClearSelection();
            });

            // Exibir resultado
            string mensagem = $"Busca e publicação concluída!\n\n" +
                            $"✅ Sucesso: {sucesso}\n" +
                            $"❌ Falhas: {falhas}\n\n" +
                            $"Total processado: {codigos.Count}";

            if (codigosComErro.Count > 0)
            {
                mensagem += $"\n\nCódigos com erro:\n{string.Join(", ", codigosComErro.Take(10))}";
                if (codigosComErro.Count > 10)
                    mensagem += $"\n... e mais {codigosComErro.Count - 10} códigos";
            }

            MessageBox.Show(mensagem,
                          "Resultado da Publicação",
                          MessageBoxButtons.OK,
                          sucesso > falhas ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Busca produtos, SKUs e estoque de forma completa e processa direto no banco
        /// </summary>
        private async Task BuscarEPublicarCompletoAsync(string loja = "01")
        {
            // Coletar códigos da PRIMEIRA coluna (índice 0)
            List<string> codigos = new List<string>();
            
            this.Invoke(() =>
            {
                for (int i = 0; i < gridResult.Rows.Count; i++)
                {
                    var row = gridResult.Rows[i];
                    if (!row.IsNewRow)
                    {
                        string codigo = row.Cells[0].Value?.ToString();
                        
                        if (!string.IsNullOrWhiteSpace(codigo))
                        {
                            codigos.Add(codigo.Trim());
                        }
                    }
                }
            });

            if (codigos.Count == 0)
            {
                MessageBox.Show("Nenhum código encontrado na primeira coluna do grid.",
                              "Aviso",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            // Criar formulário de log
            Form logForm = new Form
            {
                Text = "Processamento Completo - Produtos + SKU + Estoque",
                Size = new Size(1000, 700),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.Sizable,
                MinimizeBox = true,
                MaximizeBox = true
            };

            // TextBox para logs
            RichTextBox txtLog = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.LimeGreen,
                Font = new Font("Consolas", 9),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            // Panel inferior com progresso
            Panel panelBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            Label lblStatus = new Label
            {
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Text = "Iniciando..."
            };

            ProgressBar progressBar = new ProgressBar
            {
                Location = new Point(10, 35),
                Width = 960,
                Height = 25,
                Maximum = codigos.Count
            };

            Label lblContador = new Label
            {
                Location = new Point(10, 62),
                AutoSize = true,
                Text = $"0 / {codigos.Count}"
            };

            panelBottom.Controls.AddRange(new System.Windows.Forms.Control[] { lblStatus, progressBar, lblContador });
            logForm.Controls.AddRange(new System.Windows.Forms.Control[] { txtLog, panelBottom });

            // Mostrar formulário
            logForm.Show();
            Application.DoEvents();

            // Adicionar log inicial
            Action<string> logAction = (string mensagem) =>
            {
                logForm.Invoke((Action)(() =>
                {
                    // Aplicar cores
                    Color cor = Color.LimeGreen;
                    if (mensagem.Contains("✓")) cor = Color.Lime;
                    else if (mensagem.Contains("✗") || mensagem.Contains("[ERRO]")) cor = Color.Red;
                    else if (mensagem.Contains("⚠") || mensagem.Contains("[WARNING]")) cor = Color.Yellow;
                    else if (mensagem.Contains("[INFO]")) cor = Color.Cyan;
                    else if (mensagem.Contains("[PRODUTO]")) cor = Color.LightBlue;
                    else if (mensagem.Contains("[SKU]")) cor = Color.Orange;
                    else if (mensagem.Contains("[ESTOQUE]")) cor = Color.Magenta;
                    else if (mensagem.Contains("[RESULTADO]")) cor = Color.White;
                    else if (mensagem.Contains("═") || mensagem.Contains("╔") || mensagem.Contains("║")) cor = Color.Yellow;

                    txtLog.SelectionStart = txtLog.TextLength;
                    txtLog.SelectionLength = 0;
                    txtLog.SelectionColor = cor;
                    txtLog.AppendText(mensagem + "\n");
                    txtLog.ScrollToCaret();
                }));
            };

            logAction("╔═══════════════════════════════════════════════════════╗");
            logAction("║     SISTEMA DE PROCESSAMENTO COMPLETO                ║");
            logAction("║     Produtos + SKU + Estoque                          ║");
            logAction("╚═══════════════════════════════════════════════════════╝");
            logAction($"[INFO] Total de códigos a processar: {codigos.Count}");
            logAction($"[INFO] Loja configurada: {loja}");
            logAction($"[INFO] Data/Hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            logAction("");

            // Configurar progresso
            ProgressBar.Value = 0;
            ProgressBar.Maximum = codigos.Count;
            txtDe.Text = "0";
            txtAte.Text = codigos.Count.ToString();

            // Criar controller
            var controller = new crProdutosCod();

            // Progresso handler
            var progress = new Progress<(int Atual, int Total, string Codigo, string Etapa)>(tuple =>
            {
                this.Invoke(() =>
                {
                    ProgressBar.Value = tuple.Atual;
                    txtDe.Text = $"{tuple.Atual} ({tuple.Etapa})";
                    
                    // Atualizar progress bar do log
                    progressBar.Value = tuple.Atual;
                    lblContador.Text = $"{tuple.Atual} / {tuple.Total}";
                    lblStatus.Text = $"Processando: {tuple.Codigo} - {tuple.Etapa}";
                    
                    // Destacar linha no grid
                    for (int i = 0; i < gridResult.Rows.Count; i++)
                    {
                        if (gridResult.Rows[i].Cells[0].Value?.ToString() == tuple.Codigo)
                        {
                            gridResult.ClearSelection();
                            gridResult.Rows[i].Selected = true;
                            gridResult.FirstDisplayedScrollingRowIndex = Math.Max(0, i - 5);
                            break;
                        }
                    }
                });
            });

            try
            {
                // Executar processamento COMPLETO
                var (sucesso, falhas, resultadosPorTipo) = await controller.BuscarEProcessarCompleto(
                    codigos,
                    loja,
                    progress,
                    logAction);

                // Colorir linhas no grid baseado no resultado
                this.Invoke(() =>
                {
                    foreach (DataGridViewRow row in gridResult.Rows)
                    {
                        if (!row.IsNewRow && row.Cells[0].Value != null)
                        {
                            row.DefaultCellStyle.BackColor = Color.LightGreen;
                        }
                    }

                    ProgressBar.Value = ProgressBar.Maximum;
                    progressBar.Value = progressBar.Maximum;
                    lblStatus.Text = "Processamento Concluído!";
                    lblStatus.ForeColor = sucesso > falhas ? Color.Green : Color.Red;
                    gridResult.ClearSelection();
                });

                // Log final
                logAction("");
                logAction("╔═══════════════════════════════════════════════════════╗");
                logAction("║            PROCESSAMENTO FINALIZADO                   ║");
                logAction("╚═══════════════════════════════════════════════════════╝");

                // Adicionar botão de fechar
                Button btnFechar = new Button
                {
                    Text = "Fechar",
                    Location = new Point(890, 10),
                    Width = 80,
                    Height = 60,
                    DialogResult = DialogResult.OK
                };
                btnFechar.Click += (s, ev) => logForm.Close();
                panelBottom.Controls.Add(btnFechar);

                // Exibir resultado detalhado
                string mensagem = $"Processamento COMPLETO concluído!\n\n" +
                                $"📦 Produtos processados: {resultadosPorTipo["Produtos"]}\n" +
                                $"🏷️ SKUs processados: {resultadosPorTipo["SKUs"]}\n" +
                                $"📊 Estoques processados: {resultadosPorTipo["Estoques"]}\n\n" +
                                $"✅ Sucesso: {sucesso}\n" +
                                $"❌ Falhas: {falhas}\n\n" +
                                $"Total processado: {codigos.Count}\n" +
                                $"Loja: {loja}";

                MessageBox.Show(mensagem,
                              "Resultado do Processamento",
                              MessageBoxButtons.OK,
                              sucesso > falhas ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                logAction($"[ERRO CRÍTICO] {ex.Message}");
                logAction($"[ERRO CRÍTICO] {ex.StackTrace}");
                
                MessageBox.Show($"Erro crítico durante processamento:\n{ex.Message}",
                              "Erro",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }



        private async Task ImportarParaBancoDadosAsync()
        {
            await Task.Run(async () =>
            {
                await using var conn = new NpgsqlConnection(_config.getConfigBD());
                await conn.OpenAsync();

                int totalLinhas = 0;
                List<(int Index, string Sku, string Marca, string Departamento, string Grupo, string Subgrupo, string Linha, string Colecao)> dadosParaImportar = new();

                // Coletar TODOS os dados de uma vez (sem atualizações de UI)
                this.Invoke(() =>
                {
                    for (int i = 0; i < gridResult.Rows.Count; i++)
                    {
                        var row = gridResult.Rows[i];
                        if (!row.IsNewRow)
                        {
                            string sku = row.Cells[1].Value?.ToString();
                            
                            if (!string.IsNullOrWhiteSpace(sku))
                            {
                                dadosParaImportar.Add((
                                    i,
                                    sku,
                                    row.Cells[5].Value?.ToString(),
                                    row.Cells[6].Value?.ToString(),
                                    row.Cells[8].Value?.ToString(),
                                    row.Cells[10].Value?.ToString(),
                                    row.Cells[13].Value?.ToString(),
                                    row.Cells[14].Value?.ToString()
                                ));
                            }
                        }
                    }
                    totalLinhas = dadosParaImportar.Count;
                });

                if (totalLinhas == 0)
                    return;

                await using var transaction = await conn.BeginTransactionAsync();

                try
                {
                    // PASSO 1: Criar tabela temporária
                    string createTempTable = @"
                        CREATE TEMP TABLE temp_produtos_update (
                            sku VARCHAR,
                            marca VARCHAR,
                            departamento VARCHAR,
                            grupo VARCHAR,
                            subgrupo VARCHAR,
                            linha VARCHAR,
                            colecao VARCHAR
                        ) ON COMMIT DROP";

                    await using (var cmd = new NpgsqlCommand(createTempTable, conn, transaction))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }

                    // PASSO 2: Inserir dados em massa usando COPY (EXTREMAMENTE RÁPIDO!)
                    this.Invoke(() =>
                    {
                        ProgressBar.Value = 0;
                        txtDe.Text = "Preparando dados...";
                    });

                    string copyCommand = "COPY temp_produtos_update (sku, marca, departamento, grupo, subgrupo, linha, colecao) FROM STDIN (FORMAT BINARY)";
                    
                    await using (var writer = await conn.BeginBinaryImportAsync(copyCommand))
                    {
                        int batchProgress = 0;
                        int progressUpdateInterval = Math.Max(1000, totalLinhas / 20); // Atualizar 20 vezes no máximo

                        foreach (var dados in dadosParaImportar)
                        {
                            await writer.StartRowAsync();
                            await writer.WriteAsync(dados.Sku, NpgsqlTypes.NpgsqlDbType.Varchar);
                            await writer.WriteAsync(dados.Marca ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Varchar);
                            await writer.WriteAsync(dados.Departamento ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Varchar);
                            await writer.WriteAsync(dados.Grupo ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Varchar);
                            await writer.WriteAsync(dados.Subgrupo ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Varchar);
                            await writer.WriteAsync(dados.Linha ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Varchar);
                            await writer.WriteAsync(dados.Colecao ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Varchar);

                            batchProgress++;

                            // Atualizar progresso muito raramente
                            if (batchProgress % progressUpdateInterval == 0)
                            {
                                int currentProgress = batchProgress;
                                this.Invoke(() =>
                                {
                                    ProgressBar.Value = (int)((double)currentProgress / totalLinhas * ProgressBar.Maximum / 2); // 50% para COPY
                                    txtDe.Text = $"Carregando: {currentProgress}/{totalLinhas}";
                                });
                            }
                        }

                        await writer.CompleteAsync();
                    }

                    // PASSO 3: UPDATE em massa com JOIN (UMA ÚNICA QUERY!)
                    this.Invoke(() =>
                    {
                        txtDe.Text = "Atualizando banco...";
                    });

                    string bulkUpdate = @"
                        UPDATE produtos_sku ps
                        SET marca = t.marca,
                            departamento = t.departamento,
                            grupo = t.grupo,
                            subgrupo = t.subgrupo,
                            linha = t.linha,
                            colecao = t.colecao
                        FROM temp_produtos_update t
                        WHERE ps.sku = t.sku";

                    int linhasAtualizadas = 0;
                    await using (var cmd = new NpgsqlCommand(bulkUpdate, conn, transaction))
                    {
                        cmd.CommandTimeout = 300; // 5 minutos de timeout
                        linhasAtualizadas = await cmd.ExecuteNonQueryAsync();
                    }

                    // PASSO 4: Buscar SKUs que foram atualizados e não encontrados
                    this.Invoke(() =>
                    {
                        txtDe.Text = "Validando resultados...";
                    });

                    // Buscar SKUs que existem na temp mas não atualizaram (não existem na tabela real)
                    string findNotFound = @"
                        SELECT t.sku
                        FROM temp_produtos_update t
                        LEFT JOIN produtos_sku ps ON t.sku = ps.sku
                        WHERE ps.sku IS NULL";

                    HashSet<string> skusNaoEncontrados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    await using (var cmd = new NpgsqlCommand(findNotFound, conn, transaction))
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            skusNaoEncontrados.Add(reader.GetString(0));
                        }
                    }

                    // Commit da transação
                    await transaction.CommitAsync();

                    // PASSO 5: Atualizar cores do grid (APENAS NO FINAL!)
                    this.Invoke(() =>
                    {
                        txtDe.Text = "Atualizando interface...";
                        ProgressBar.Value = (int)(ProgressBar.Maximum * 0.9);

                        foreach (var dados in dadosParaImportar)
                        {
                            if (dados.Index < gridResult.Rows.Count)
                            {
                                var row = gridResult.Rows[dados.Index];
                                
                                if (skusNaoEncontrados.Contains(dados.Sku))
                                {
                                    row.DefaultCellStyle.BackColor = Color.LightCoral; // SKU não encontrado
                                }
                                else
                                {
                                    row.DefaultCellStyle.BackColor = Color.LightGreen; // Sucesso
                                }
                            }
                        }

                        ProgressBar.Value = ProgressBar.Maximum;
                        txtDe.Text = $"{linhasAtualizadas} atualizados, {skusNaoEncontrados.Count} não encontrados";
                        gridResult.ClearSelection();
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        private void MarcarColunasImportantes()
        {
            if (gridResult.Columns.Count == 0)
                return;

            // Resetar cores de todas as colunas
            foreach (DataGridViewColumn col in gridResult.Columns)
            {
                col.DefaultCellStyle.BackColor = Color.White;
                col.HeaderCell.Style.BackColor = Color.LightGray;
                col.HeaderCell.Style.Font = new Font(gridResult.Font, FontStyle.Regular);
            }

            // Marcar coluna SKU (Coluna 2 - índice 1) - Chave de busca
            if (gridResult.Columns.Count > 1)
            {
                gridResult.Columns[1].DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 200); // Amarelo claro
                gridResult.Columns[1].HeaderCell.Style.BackColor = Color.FromArgb(255, 235, 0); // Amarelo
                gridResult.Columns[1].HeaderCell.Style.Font = new Font(gridResult.Font, FontStyle.Bold);
                gridResult.Columns[1].HeaderCell.ToolTipText = "🔑 CHAVE DE BUSCA - Campo obrigatório usado no WHERE";
            }

            // Marcar colunas que serão atualizadas no banco
            var colunasParaAtualizar = new[]
            {
                new { Index = 5, Nome = "Marca", Descricao = "📝 Campo atualizado no banco" },
                new { Index = 6, Nome = "Departamento", Descricao = "📝 Campo atualizado no banco" },
                new { Index = 8, Nome = "Grupo", Descricao = "📝 Campo atualizado no banco" },
                new { Index = 10, Nome = "Subgrupo", Descricao = "📝 Campo atualizado no banco" },
                new { Index = 13, Nome = "Linha", Descricao = "📝 Campo atualizado no banco" },
                new { Index = 14, Nome = "Coleção", Descricao = "📝 Campo atualizado no banco" }
            };

            foreach (var coluna in colunasParaAtualizar)
            {
                if (gridResult.Columns.Count > coluna.Index)
                {
                    gridResult.Columns[coluna.Index].DefaultCellStyle.BackColor = Color.FromArgb(220, 240, 255); // Azul claro
                    gridResult.Columns[coluna.Index].HeaderCell.Style.BackColor = Color.FromArgb(79, 129, 189); // Azul
                    gridResult.Columns[coluna.Index].HeaderCell.Style.ForeColor = Color.White;
                    gridResult.Columns[coluna.Index].HeaderCell.Style.Font = new Font(gridResult.Font, FontStyle.Bold);
                    gridResult.Columns[coluna.Index].HeaderCell.ToolTipText = $"{coluna.Descricao} - {coluna.Nome}";
                }
            }

            // Criar label de legenda se não existir
            CriarLabelLegenda();
        }

        private void CriarLabelLegenda()
        {
            // Verificar se já existe a label
            var legendaExistente = this.Controls.Find("lblLegendaImportacao", true);
            if (legendaExistente.Length > 0)
            {
                legendaExistente[0].Visible = true;
                return;
            }

            // Criar nova label de legenda
            Label lblLegenda = new Label
            {
                Name = "lblLegendaImportacao",
                AutoSize = false,
                Width = gridResult.Width,
                Height = 45,
                Location = new Point(gridResult.Location.X, gridResult.Location.Y + gridResult.Height + 5),
                BackColor = Color.FromArgb(240, 240, 240),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5),
                Font = new Font(gridResult.Font.FontFamily, 9, FontStyle.Regular)
            };

            // Texto da legenda
            lblLegenda.Text = "🔑 Amarelo = SKU (Chave de busca) | " +
                             "📝 Azul = Campos atualizados (Marca, Departamento, Grupo, Subgrupo, Linha, Coleção) | " +
                             "⚪ Branco = Campos não importados";

            this.Controls.Add(lblLegenda);
            lblLegenda.BringToFront();
        }

        private void btnBaixaLayout_Click(object sender, EventArgs e)
        {
            try
            {
                // Diálogo para salvar arquivo
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Title = "Salvar Layout Padrão";
                    saveFileDialog.Filter = "Arquivo Excel (*.xlsx)|*.xlsx";
                    saveFileDialog.FileName = $"Layout_Importacao_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        GerarLayoutPadraoExcel(saveFileDialog.FileName);

                        MessageBox.Show(
                            $"Layout padrão gerado com sucesso!\n\nArquivo salvo em:\n{saveFileDialog.FileName}",
                            "Sucesso",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        // Perguntar se deseja abrir o arquivo
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

        private void GerarLayoutPadraoExcel(string caminhoArquivo)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                // Adicionar planilha
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Layout Importação");

                // Definir cabeçalhos das colunas
                worksheet.Cells[1, 1].Value = "Status";
                worksheet.Cells[1, 2].Value = "SKU";
                worksheet.Cells[1, 3].Value = "Código";
                worksheet.Cells[1, 4].Value = "Descrição";
                worksheet.Cells[1, 5].Value = "Código Marca";
                worksheet.Cells[1, 6].Value = "Marca";
                worksheet.Cells[1, 7].Value = "Departamento";
                worksheet.Cells[1, 8].Value = "Código Grupo";
                worksheet.Cells[1, 9].Value = "Grupo";
                worksheet.Cells[1, 10].Value = "Código Subgrupo";
                worksheet.Cells[1, 11].Value = "Subgrupo";
                worksheet.Cells[1, 12].Value = "Código Linha";
                worksheet.Cells[1, 13].Value = "Linha Desc";
                worksheet.Cells[1, 14].Value = "Linha";
                worksheet.Cells[1, 15].Value = "Coleção";

                // Estilizar cabeçalho
                using (var range = worksheet.Cells[1, 1, 1, 15])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 12;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                // Adicionar linhas de exemplo
                worksheet.Cells[2, 1].Value = "Ativo";
                worksheet.Cells[2, 2].Value = "SKU001";
                worksheet.Cells[2, 3].Value = "PROD001";
                worksheet.Cells[2, 4].Value = "Produto Exemplo 1";
                worksheet.Cells[2, 5].Value = ""; // Código Marca (deixar vazio para sistema buscar)
                worksheet.Cells[2, 6].Value = "Nike";
                worksheet.Cells[2, 7].Value = "Calçados";
                worksheet.Cells[2, 8].Value = "GRP001";
                worksheet.Cells[2, 9].Value = "Esportivo";
                worksheet.Cells[2, 10].Value = "SUB001";
                worksheet.Cells[2, 11].Value = "Tênis";
                worksheet.Cells[2, 12].Value = "LIN001";
                worksheet.Cells[2, 13].Value = "Running";
                worksheet.Cells[2, 14].Value = "Performance";
                worksheet.Cells[2, 15].Value = "Verão 2024";

                worksheet.Cells[3, 1].Value = "Ativo";
                worksheet.Cells[3, 2].Value = "SKU002";
                worksheet.Cells[3, 3].Value = "PROD002";
                worksheet.Cells[3, 4].Value = "Produto Exemplo 2";
                worksheet.Cells[3, 5].Value = ""; // Código Marca (deixar vazio)
                worksheet.Cells[3, 6].Value = "Adidas";
                worksheet.Cells[3, 7].Value = "Vestuário";
                worksheet.Cells[3, 8].Value = "GRP002";
                worksheet.Cells[3, 9].Value = "Casual";
                worksheet.Cells[3, 10].Value = "SUB002";
                worksheet.Cells[3, 11].Value = "Camiseta";
                worksheet.Cells[3, 12].Value = "LIN002";
                worksheet.Cells[3, 13].Value = "Lifestyle";
                worksheet.Cells[3, 14].Value = "Básica";
                worksheet.Cells[3, 15].Value = "Inverno 2024";

                worksheet.Cells[4, 1].Value = "Ativo";
                worksheet.Cells[4, 2].Value = "SKU003";
                worksheet.Cells[4, 3].Value = "PROD003";
                worksheet.Cells[4, 4].Value = "Produto Exemplo 3";
                worksheet.Cells[4, 5].Value = ""; // Código Marca (deixar vazio)
                worksheet.Cells[4, 6].Value = "Puma";
                worksheet.Cells[4, 7].Value = "Acessórios";
                worksheet.Cells[4, 8].Value = "GRP003";
                worksheet.Cells[4, 9].Value = "Fitness";
                worksheet.Cells[4, 10].Value = "SUB003";
                worksheet.Cells[4, 11].Value = "Mochila";
                worksheet.Cells[4, 12].Value = "LIN003";
                worksheet.Cells[4, 13].Value = "Training";
                worksheet.Cells[4, 14].Value = "Profissional";
                worksheet.Cells[4, 15].Value = "Primavera 2024";

                // Estilizar linhas de exemplo
                using (var range = worksheet.Cells[2, 1, 4, 15])
                {
                    range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                }

                // Destacar colunas importantes para importação
                using (var range = worksheet.Cells[1, 2, 4, 2]) // SKU
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 200)); // Amarelo claro
                }

                using (var range = worksheet.Cells[1, 6, 4, 6]) // Marca
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 230, 241)); // Azul claro
                }

                using (var range = worksheet.Cells[1, 7, 4, 7]) // Departamento
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 230, 241)); // Azul claro
                }

                using (var range = worksheet.Cells[1, 9, 4, 9]) // Grupo
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 230, 241)); // Azul claro
                }

                using (var range = worksheet.Cells[1, 11, 4, 11]) // Subgrupo
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 230, 241)); // Azul claro
                }

                using (var range = worksheet.Cells[1, 14, 4, 14]) // Linha
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 230, 241)); // Azul claro
                }

                using (var range = worksheet.Cells[1, 15, 4, 15]) // Coleção
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 230, 241)); // Azul claro
                }

                // Ajustar largura das colunas
                worksheet.Column(1).Width = 10;  // Status
                worksheet.Column(2).Width = 15;  // SKU
                worksheet.Column(3).Width = 15;  // Código
                worksheet.Column(4).Width = 30;  // Descrição
                worksheet.Column(5).Width = 12;  // Código Marca
                worksheet.Column(6).Width = 20;  // Marca
                worksheet.Column(7).Width = 20;  // Departamento
                worksheet.Column(8).Width = 12;  // Código Grupo
                worksheet.Column(9).Width = 20;  // Grupo
                worksheet.Column(10).Width = 15; // Código Subgrupo
                worksheet.Column(11).Width = 20; // Subgrupo
                worksheet.Column(12).Width = 12; // Código Linha
                worksheet.Column(13).Width = 15; // Linha Desc
                worksheet.Column(14).Width = 20; // Linha
                worksheet.Column(15).Width = 20; // Coleção

                // Congelar primeira linha (cabeçalho)
                worksheet.View.FreezePanes(2, 1);

                // Adicionar instruções em uma segunda aba
                ExcelWorksheet instrucoes = package.Workbook.Worksheets.Add("Instruções");
                
                instrucoes.Cells[1, 1].Value = "INSTRUÇÕES DE USO";
                instrucoes.Cells[1, 1].Style.Font.Size = 16;
                instrucoes.Cells[1, 1].Style.Font.Bold = true;
                
                instrucoes.Cells[3, 1].Value = "1. CAMPOS OBRIGATÓRIOS:";
                instrucoes.Cells[3, 1].Style.Font.Bold = true;
                instrucoes.Cells[4, 1].Value = "   • SKU (Coluna 2): Código único do produto - OBRIGATÓRIO";
                instrucoes.Cells[5, 1].Value = "   • Marca (Coluna 6): Nome da marca do produto";
                
                instrucoes.Cells[7, 1].Value = "2. CAMPOS ATUALIZADOS NO BANCO:";
                instrucoes.Cells[7, 1].Style.Font.Bold = true;
                instrucoes.Cells[8, 1].Value = "   • Coluna 6: Marca";
                instrucoes.Cells[9, 1].Value = "   • Coluna 7: Departamento";
                instrucoes.Cells[10, 1].Value = "   • Coluna 9: Grupo";
                instrucoes.Cells[11, 1].Value = "   • Coluna 11: Subgrupo";
                instrucoes.Cells[12, 1].Value = "   • Coluna 14: Linha";
                instrucoes.Cells[13, 1].Value = "   • Coluna 15: Coleção";
                
                instrucoes.Cells[15, 1].Value = "3. BUSCA AUTOMÁTICA DE MARCA:";
                instrucoes.Cells[15, 1].Style.Font.Bold = true;
                instrucoes.Cells[16, 1].Value = "   • Se a Coluna 5 (Código Marca) estiver VAZIA";
                instrucoes.Cells[17, 1].Value = "   • E a Coluna 6 (Marca) tiver um valor";
                instrucoes.Cells[18, 1].Value = "   • O sistema buscará automaticamente o código da marca no banco de dados";
                instrucoes.Cells[19, 1].Value = "   • E corrigirá a descrição da marca para o padrão do banco";
                
                instrucoes.Cells[21, 1].Value = "4. CORES DAS COLUNAS:";
                instrucoes.Cells[21, 1].Style.Font.Bold = true;
                instrucoes.Cells[22, 1].Value = "   • Amarelo: SKU (chave de busca)";
                instrucoes.Cells[23, 1].Value = "   • Azul claro: Campos que serão atualizados no banco";
                
                instrucoes.Cells[25, 1].Value = "5. APÓS IMPORTAÇÃO:";
                instrucoes.Cells[25, 1].Style.Font.Bold = true;
                instrucoes.Cells[26, 1].Value = "   • Verde: Registro atualizado com sucesso";
                instrucoes.Cells[27, 1].Value = "   • Vermelho claro: SKU não encontrado no banco";
                instrucoes.Cells[28, 1].Value = "   • Amarelo: SKU vazio (linha ignorada)";
                instrucoes.Cells[29, 1].Value = "   • Vermelho: Erro durante atualização";
                
                instrucoes.Cells[31, 1].Value = "6. DICAS:";
                instrucoes.Cells[31, 1].Style.Font.Bold = true;
                instrucoes.Cells[32, 1].Value = "   • Mantenha o formato das colunas";
                instrucoes.Cells[33, 1].Value = "   • Não altere os cabeçalhos";
                instrucoes.Cells[34, 1].Value = "   • Remova as linhas de exemplo antes de importar seus dados";
                instrucoes.Cells[35, 1].Value = "   • Certifique-se que os SKUs existem no banco de dados";
                instrucoes.Cells[36, 1].Value = "   • Deixe a Coluna 5 vazia para busca automática de marca";
                
                // Ajustar largura das colunas na aba de instruções
                instrucoes.Column(1).Width = 80;
                
                // Salvar arquivo
                FileInfo file = new FileInfo(caminhoArquivo);
                package.SaveAs(file);
            }
        }

        /// <summary>
        /// Atualiza os preços de todos os produtos na base através da API
        /// </summary>
        private async Task AtualizarPrecosAsync()
        {
            try
            {
                // Criar formulário para configurar parâmetros
                using (var configForm = new Form())
                {
                    configForm.Text = "Configurar Atualização de Preços";
                    configForm.Size = new Size(500, 360);
                    configForm.StartPosition = FormStartPosition.CenterParent;
                    configForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                    configForm.MaximizeBox = false;
                    configForm.MinimizeBox = false;

                    var lblInfo = new Label
                    {
                        Text = "Escolha a origem dos produtos e configure os parâmetros abaixo\n" +
                               "para evitar sobrecarga na API:",
                        Location = new Point(20, 20),
                        Width = 450,
                        Height = 40,
                        Font = new Font("Arial", 9)
                    };

                    // NOVA SEÇÃO: Escolher origem dos produtos
                    var gbOrigem = new GroupBox
                    {
                        Text = "Origem dos Produtos",
                        Location = new Point(20, 65),
                        Width = 450,
                        Height = 80
                    };

                    var rbBaseCompleta = new RadioButton
                    {
                        Text = "Atualizar TODA a base de dados",
                        Location = new Point(10, 20),
                        Width = 420,
                        Checked = true
                    };

                    var rbPlanilha = new RadioButton
                    {
                        Text = "Atualizar apenas produtos da planilha carregada no grid",
                        Location = new Point(10, 45),
                        Width = 420
                    };

                    gbOrigem.Controls.Add(rbBaseCompleta);
                    gbOrigem.Controls.Add(rbPlanilha);

                    var lblDelay = new Label
                    {
                        Text = "Delay entre requisições (ms):",
                        Location = new Point(20, 160),
                        AutoSize = true
                    };

                    var txtDelay = new TextBox
                    {
                        Text = "1500",
                        Location = new Point(250, 157),
                        Width = 80
                    };

                    var lblDelayInfo = new Label
                    {
                        Text = "(Recomendado: 1000-2000ms)",
                        Location = new Point(340, 160),
                        AutoSize = true,
                        ForeColor = Color.Gray,
                        Font = new Font("Arial", 8)
                    };

                    var lblBatchSize = new Label
                    {
                        Text = "Processar em lotes de:",
                        Location = new Point(20, 190),
                        AutoSize = true
                    };

                    var txtBatchSize = new TextBox
                    {
                        Text = "20",
                        Location = new Point(250, 187),
                        Width = 80
                    };

                    var lblBatchInfo = new Label
                    {
                        Text = "(Pausa de 10s a cada lote)",
                        Location = new Point(340, 190),
                        AutoSize = true,
                        ForeColor = Color.Gray,
                        Font = new Font("Arial", 8)
                    };

                    var lblMaxRetry = new Label
                    {
                        Text = "Tentativas em caso de erro:",
                        Location = new Point(20, 220),
                        AutoSize = true
                    };

                    var txtMaxRetry = new TextBox
                    {
                        Text = "3",
                        Location = new Point(250, 217),
                        Width = 80
                    };

                    var lblRetryInfo = new Label
                    {
                        Text = "(Recomendado: 2-5)",
                        Location = new Point(340, 220),
                        AutoSize = true,
                        ForeColor = Color.Gray,
                        Font = new Font("Arial", 8)
                    };

                    var btnOk = new Button
                    {
                        Text = "Iniciar",
                        Location = new Point(250, 260),
                        DialogResult = DialogResult.OK,
                        Width = 100
                    };

                    var btnCancelar = new Button
                    {
                        Text = "Cancelar",
                        Location = new Point(360, 260),
                        DialogResult = DialogResult.Cancel,
                        Width = 100
                    };

                    configForm.Controls.AddRange(new System.Windows.Forms.Control[] 
                    { 
                        lblInfo, 
                        gbOrigem,
                        lblDelay, txtDelay, lblDelayInfo,
                        lblBatchSize, txtBatchSize, lblBatchInfo,
                        lblMaxRetry, txtMaxRetry, lblRetryInfo,
                        btnOk, btnCancelar 
                    });
                    configForm.AcceptButton = btnOk;
                    configForm.CancelButton = btnCancelar;

                    if (configForm.ShowDialog() != DialogResult.OK)
                        return;

                    // Obter valores configurados
                    if (!int.TryParse(txtDelay.Text, out int delayMs))
                        delayMs = 1500;
                    if (!int.TryParse(txtBatchSize.Text, out int batchSize))
                        batchSize = 20;
                    if (!int.TryParse(txtMaxRetry.Text, out int maxRetries))
                        maxRetries = 3;

                    // Validar valores - AUMENTADOS OS MÍNIMOS
                    delayMs = Math.Max(500, Math.Min(10000, delayMs)); // Mínimo de 500ms
                    batchSize = Math.Max(5, Math.Min(100, batchSize)); // Lotes menores
                    maxRetries = Math.Max(1, Math.Min(10, maxRetries));

                    // Verificar qual origem foi escolhida
                    bool usarBaseCompleta = rbBaseCompleta.Checked;

                    await ExecutarAtualizacaoComControle(delayMs, batchSize, maxRetries, usarBaseCompleta);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro durante a atualização de preços:\n{ex.Message}",
                              "Erro",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Executa a atualização de preços com controle de taxa de requisições
        /// </summary>
        private async Task ExecutarAtualizacaoComControle(int delayMs, int batchSize, int maxRetries, bool usarBaseCompleta)
        {
            try
            {

                // Criar formulário de log
                Form logForm = new Form
                {
                    Text = "Atualização de Preços - Todos os Produtos",
                    Size = new Size(1000, 700),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.Sizable,
                    MinimizeBox = true,
                    MaximizeBox = true
                };

                // TextBox para logs
                RichTextBox txtLog = new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    BackColor = Color.Black,
                    ForeColor = Color.LimeGreen,
                    Font = new Font("Consolas", 9),
                    ScrollBars = RichTextBoxScrollBars.Vertical
                };

                // Panel inferior com progresso
                Panel panelBottom = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 80,
                    BackColor = Color.FromArgb(240, 240, 240)
                };

                Label lblStatus = new Label
                {
                    Location = new Point(10, 10),
                    AutoSize = true,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    Text = "Iniciando..."
                };

                ProgressBar progressBar = new ProgressBar
                {
                    Location = new Point(10, 35),
                    Width = 960,
                    Height = 25
                };

                Label lblContador = new Label
                {
                    Location = new Point(10, 62),
                    AutoSize = true,
                    Text = "0 / 0"
                };

                panelBottom.Controls.AddRange(new System.Windows.Forms.Control[] { lblStatus, progressBar, lblContador });
                logForm.Controls.AddRange(new System.Windows.Forms.Control[] { txtLog, panelBottom });

                // Mostrar formulário
                logForm.Show();
                Application.DoEvents();

                // Ação de log
                Action<string> logAction = (string mensagem) =>
                {
                    if (logForm.IsDisposed)
                        return;

                    logForm.Invoke((Action)(() =>
                    {
                        // Aplicar cores
                        Color cor = Color.LimeGreen;
                        if (mensagem.Contains("✓")) cor = Color.Lime;
                        else if (mensagem.Contains("✗") || mensagem.Contains("[ERRO]")) cor = Color.Red;
                        else if (mensagem.Contains("⚠") || mensagem.Contains("[WARNING]")) cor = Color.Yellow;
                        else if (mensagem.Contains("[INFO]")) cor = Color.Cyan;
                        else if (mensagem.Contains("[PREÇO]")) cor = Color.Gold;
                        else if (mensagem.Contains("[SUCESSO]")) cor = Color.LightGreen;
                        else if (mensagem.Contains("═") || mensagem.Contains("╔") || mensagem.Contains("║")) cor = Color.Yellow;

                        txtLog.SelectionStart = txtLog.TextLength;
                        txtLog.SelectionLength = 0;
                        txtLog.SelectionColor = cor;
                        txtLog.AppendText(mensagem + "\n");
                        txtLog.ScrollToCaret();
                    }));
                };

                logAction("╔═══════════════════════════════════════════════════════╗");
                logAction("║     ATUALIZAÇÃO DE PREÇOS DE TODOS OS PRODUTOS       ║");
                logAction("╚═══════════════════════════════════════════════════════╝");
                logAction($"[INFO] Data/Hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                logAction("");

                // Buscar códigos baseado na origem escolhida
                List<string> codigos = new List<string>();

                if (usarBaseCompleta)
                {
                    // OPÇÃO 1: Buscar todos os produtos do banco
                    logAction("[INFO] Origem: TODA A BASE DE DADOS");
                    logAction("[INFO] Buscando todos os produtos da base de dados...");
                    
                    await using (var conn = new NpgsqlConnection(_config.getConfigBD()))
                    {
                        await conn.OpenAsync();
                        string query = "SELECT DISTINCT codigo FROM produtos WHERE codigo IS NOT NULL AND codigo != ''";
                        
                        await using (var cmd = new NpgsqlCommand(query, conn))
                        await using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string codigo = reader["codigo"]?.ToString();
                                if (!string.IsNullOrWhiteSpace(codigo))
                                {
                                    codigos.Add(codigo.Trim());
                                }
                            }
                        }
                    }
                }
                else
                {
                    // OPÇÃO 2: Usar produtos da planilha carregada no grid
                    logAction("[INFO] Origem: PLANILHA CARREGADA NO GRID");
                    logAction("[INFO] Coletando produtos da planilha...");
                    
                    this.Invoke(() =>
                    {
                        // Verificar se há dados no grid
                        if (gridResult.DataSource == null || gridResult.Rows.Count == 0)
                        {
                            logAction("[WARNING] Nenhum dado encontrado no grid!");
                            return;
                        }

                        // Verificar quantidade de colunas
                        if (gridResult.Columns.Count < 3)
                        {
                            logAction($"[WARNING] O grid possui apenas {gridResult.Columns.Count} colunas. Esperado: pelo menos 3 colunas.");
                            logAction("[INFO] Tentando usar a primeira coluna disponível...");
                        }

                        // Determinar qual coluna usar (preferência: índice 2, senão índice 0)
                        int colunaIndice = gridResult.Columns.Count >= 3 ? 2 : 0;
                        string nomeColuna = gridResult.Columns[colunaIndice].HeaderText;
                        
                        logAction($"[INFO] Usando coluna: '{nomeColuna}' (índice {colunaIndice})");

                        // Coletar códigos da coluna escolhida
                        for (int i = 0; i < gridResult.Rows.Count; i++)
                        {
                            var row = gridResult.Rows[i];
                            if (!row.IsNewRow && row.Cells.Count > colunaIndice)
                            {
                                string codigo = row.Cells[colunaIndice].Value?.ToString();
                                
                                if (!string.IsNullOrWhiteSpace(codigo))
                                {
                                    codigos.Add(codigo.Trim());
                                }
                            }
                        }
                        
                        logAction($"[INFO] Coletados {codigos.Count} códigos do grid");
                    });
                }

                if (codigos.Count == 0)
                {
                    logAction("[WARNING] Nenhum produto encontrado para processar.");
                    MessageBox.Show("Nenhum produto encontrado para processar.",
                                  "Aviso",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                    logForm.Close();
                    return;
                }

                logAction($"[INFO] Total de produtos encontrados: {codigos.Count}");
                logAction("");

                // Configurar progresso
                progressBar.Maximum = codigos.Count;
                ProgressBar.Value = 0;
                ProgressBar.Maximum = codigos.Count;
                txtDe.Text = "0";
                txtAte.Text = codigos.Count.ToString();

                // Criar controller para buscar preços
                var controllerPrecos = new crTabPreco();
                var controllerProdutos = new crProdutosCod();

                int sucesso = 0;
                int falhas = 0;
                int semPreco = 0;
                int totalRequisicoes = 0;
                int rateLimitHits = 0; // Contador de vezes que bateu no rate limit

                // Processar cada código com controle de taxa
                for (int i = 0; i < codigos.Count; i++)
                {
                    string codigo = codigos[i];
                    bool sucessoNaRequisicao = false;
                    
                    // Tentar até maxRetries vezes
                    for (int tentativa = 1; tentativa <= maxRetries && !sucessoNaRequisicao; tentativa++)
                    {
                        try
                        {
                            if (tentativa > 1)
                            {
                                // Backoff exponencial MAIOR em caso de retry
                                int backoffDelay = delayMs * tentativa * 3; // Aumentado de 2 para 3
                                logAction($"[INFO] Tentativa {tentativa}/{maxRetries} para produto {codigo}");
                                logAction($"[INFO] Aguardando {backoffDelay}ms antes de tentar novamente...");
                                await Task.Delay(backoffDelay);
                            }

                            logAction($"[INFO] [{i + 1}/{codigos.Count}] Buscando preço do produto: {codigo}");
                            totalRequisicoes++;
                            
                            // Buscar preço do produto na API (tabela 1)
                            var (sucessoAPI, conteudo, erro) = await controllerPrecos.getTabPrecoInd(codigo, 1);
                            
                            if (sucessoAPI && !string.IsNullOrEmpty(conteudo))
                            {
                                // Parse do JSON de preços e salvar no banco
                                logAction($"[PREÇO] ✓ Preço do produto {codigo} obtido com sucesso");
                                logAction($"[INFO] Salvando preço no banco de dados...");
                                
                                try
                                {
                                    // Usar o método ImportarAsyncInd do controller de preços
                                    var precoInput = new crTabPrecoInput(_config.getConfigBD());
                                    await precoInput.ImportarAsyncInd(conteudo);
                                    
                                    logAction($"[SUCESSO] ✓ Produto {codigo} atualizado com sucesso no banco");
                                    sucesso++;
                                    sucessoNaRequisicao = true;
                                }
                                catch (Exception exSave)
                                {
                                    logAction($"[ERRO] ✗ Erro ao salvar preço do produto {codigo} no banco: {exSave.Message}");
                                    falhas++;
                                    sucessoNaRequisicao = true;
                                }
                            }
                            else if (!string.IsNullOrEmpty(erro))
                            {
                                // Verificar se é erro de rate limit
                                if (erro.Contains("429") || erro.ToLower().Contains("too many") || 
                                    erro.ToLower().Contains("rate limit") || erro.ToLower().Contains("throttle"))
                                {
                                    rateLimitHits++;
                                    logAction($"[WARNING] ⚠⚠⚠ RATE LIMIT ATINGIDO no produto {codigo} ⚠⚠⚠");
                                    
                                    // PAUSA LONGA quando bater no rate limit
                                    int pausaRateLimit = 30000; // 30 segundos
                                    logAction($"[INFO] 🛑 PAUSANDO POR {pausaRateLimit / 1000} SEGUNDOS para respeitar rate limit...");
                                    logAction($"[INFO] Total de rate limits atingidos até agora: {rateLimitHits}");
                                    
                                    await Task.Delay(pausaRateLimit);
                                    
                                    if (tentativa < maxRetries)
                                    {
                                        logAction($"[INFO] ⏰ Pausa concluída. Tentando novamente...");
                                        continue; // Tentar novamente
                                    }
                                    else
                                    {
                                        logAction($"[ERRO] ✗ Máximo de tentativas atingido para {codigo}");
                                        falhas++;
                                        sucessoNaRequisicao = true; // Parar de tentar
                                    }
                                }
                                else
                                {
                                    logAction($"[ERRO] ✗ Erro ao buscar preço do produto {codigo}: {erro}");
                                    falhas++;
                                    sucessoNaRequisicao = true; // Não tentar novamente para outros tipos de erro
                                }
                            }
                            else
                            {
                                logAction($"[WARNING] ⚠ Produto {codigo} sem preço disponível na API");
                                semPreco++;
                                sucessoNaRequisicao = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            logAction($"[ERRO] ✗ Exceção ao processar produto {codigo} (tentativa {tentativa}): {ex.Message}");
                            
                            if (tentativa >= maxRetries)
                            {
                                falhas++;
                                sucessoNaRequisicao = true;
                            }
                        }
                    }

                    // Atualizar progresso
                    this.Invoke(() =>
                    {
                        ProgressBar.Value = i + 1;
                        txtDe.Text = (i + 1).ToString();
                        progressBar.Value = i + 1;
                        lblContador.Text = $"{i + 1} / {codigos.Count}";
                        lblStatus.Text = $"Processando: {codigo} | Reqs: {totalRequisicoes} | RateLimit: {rateLimitHits}";
                    });

                    // Delay OBRIGATÓRIO entre requisições (mesmo que seja sucesso)
                    if (i < codigos.Count - 1) // Não precisa esperar no último
                    {
                        logAction($"[INFO] ⏳ Aguardando {delayMs}ms antes da próxima requisição...");
                        await Task.Delay(delayMs);
                    }

                    // Pausa MAIOR a cada lote
                    if ((i + 1) % batchSize == 0 && i < codigos.Count - 1)
                    {
                        int pausaLote = 10000; // 10 segundos (aumentado de 5)
                        logAction($"");
                        logAction($"[INFO] ═══════════════════════════════════════════════");
                        logAction($"[INFO] 🎯 Lote de {batchSize} produtos concluído!");
                        logAction($"[INFO] 🛑 PAUSANDO {pausaLote / 1000}s antes do próximo lote...");
                        logAction($"[INFO] 📊 Progresso: {i + 1}/{codigos.Count} ({((i + 1) * 100 / codigos.Count)}%)");
                        logAction($"[INFO] 🔄 Requisições totais: {totalRequisicoes}");
                        logAction($"[INFO] ⚠️  Rate limits encontrados: {rateLimitHits}");
                        logAction($"[INFO] ═══════════════════════════════════════════════");
                        logAction($"");
                        
                        await Task.Delay(pausaLote);
                    }
                }

                // Log final
                logAction("");
                logAction("╔═══════════════════════════════════════════════════════╗");
                logAction("║        ATUALIZAÇÃO DE PREÇOS FINALIZADA              ║");
                logAction("╚═══════════════════════════════════════════════════════╝");
                logAction($"[INFO] Origem dos produtos: {(usarBaseCompleta ? "BASE COMPLETA" : "PLANILHA DO GRID")}");
                logAction($"[INFO] Configurações utilizadas:");
                logAction($"[INFO]   - Delay entre requisições: {delayMs}ms");
                logAction($"[INFO]   - Tamanho do lote: {batchSize} produtos");
                logAction($"[INFO]   - Máximo de tentativas: {maxRetries}");
                logAction($"[INFO] Total de requisições realizadas: {totalRequisicoes}");
                logAction($"[INFO] Rate limits encontrados: {rateLimitHits}");
                logAction($"[INFO] Total processado: {codigos.Count}");
                logAction($"[SUCESSO] ✓ Sucesso: {sucesso}");
                logAction($"[WARNING] ⚠ Sem preço: {semPreco}");
                logAction($"[ERRO] ✗ Falhas: {falhas}");

                // Adicionar botão de fechar
                Button btnFechar = new Button
                {
                    Text = "Fechar",
                    Location = new Point(890, 10),
                    Width = 80,
                    Height = 60,
                    DialogResult = DialogResult.OK
                };
                btnFechar.Click += (s, ev) => logForm.Close();
                panelBottom.Controls.Add(btnFechar);

                lblStatus.Text = "Processamento Concluído!";
                lblStatus.ForeColor = sucesso > falhas ? Color.Green : Color.Red;

                // Exibir resultado
                string mensagem = $"Atualização de preços concluída!\n\n" +
                                $"📁 Origem: {(usarBaseCompleta ? "Base Completa" : "Planilha do Grid")}\n" +
                                $"📦 Total de produtos: {codigos.Count}\n" +
                                $"🔄 Total de requisições: {totalRequisicoes}\n" +
                                $"⚠️  Rate limits encontrados: {rateLimitHits}\n" +
                                $"✅ Sucesso: {sucesso}\n" +
                                $"⚠️ Sem preço: {semPreco}\n" +
                                $"❌ Falhas: {falhas}\n\n" +
                                $"⚙️ Delay usado: {delayMs}ms | Lote: {batchSize}";

                MessageBox.Show(mensagem,
                              "Resultado da Atualização",
                              MessageBoxButtons.OK,
                              sucesso > falhas ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro durante a atualização de preços:\n{ex.Message}",
                              "Erro",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }
    }
}
