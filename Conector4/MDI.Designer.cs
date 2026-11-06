namespace Conector4
{
    partial class MDI
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MDI));
            panel1 = new Panel();
            logo = new PictureBox();
            tabControl = new TabControl();
            tabPage1 = new TabPage();
            btnBuscar = new Button();
            cmbServicoMensageria = new ComboBox();
            label1 = new Label();
            gridMensageria = new DataGridView();
            tabPage2 = new TabPage();
            chcRefresh = new CheckBox();
            txtSize = new TextBox();
            txtKeyword = new TextBox();
            label6 = new Label();
            cmbStatus = new ComboBox();
            label5 = new Label();
            txtFinal = new MaskedTextBox();
            btnBuscarLog = new Button();
            txtInicial = new MaskedTextBox();
            label4 = new Label();
            GridLogs = new DataGridView();
            pictureBox1 = new PictureBox();
            tabPage3 = new TabPage();
            GridServico = new DataGridView();
            groupBox1 = new GroupBox();
            btnRefreshSer = new Button();
            txtCodigo = new TextBox();
            btnGravar = new Button();
            txtMetodo = new TextBox();
            label8 = new Label();
            txtClasse = new TextBox();
            label7 = new Label();
            chAtivoServico = new CheckBox();
            textBox1 = new TextBox();
            label3 = new Label();
            txtNomeServico = new TextBox();
            label2 = new Label();
            tabPage4 = new TabPage();
            pnGrafico = new Panel();
            gridMetrics = new DataGridView();
            tmRabiit = new System.Windows.Forms.Timer(components);
            tmTela = new System.Windows.Forms.Timer(components);
            tmRefreshLogs = new System.Windows.Forms.Timer(components);
            menuStrip1 = new MenuStrip();
            importaçãoToolStripMenuItem = new ToolStripMenuItem();
            viaEXCELToolStripMenuItem = new ToolStripMenuItem();
            produtosSKUToolStripMenuItem = new ToolStripMenuItem();
            produtosToolStripMenuItem = new ToolStripMenuItem();
            planilhaAPIToolStripMenuItem = new ToolStripMenuItem();
            alteraçãoToolStripMenuItem = new ToolStripMenuItem();
            marcasToolStripMenuItem = new ToolStripMenuItem();
            deparToolStripMenuItem = new ToolStripMenuItem();
            gruposToolStripMenuItem = new ToolStripMenuItem();
            subGruposToolStripMenuItem = new ToolStripMenuItem();
            eCsToolStripMenuItem = new ToolStripMenuItem();
            exportarDadosDeEPCToolStripMenuItem = new ToolStripMenuItem();
            dataViewToolStripMenuItem = new ToolStripMenuItem();
            importarImagensToolStripMenuItem = new ToolStripMenuItem();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logo).BeginInit();
            tabControl.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridMensageria).BeginInit();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)GridLogs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)GridServico).BeginInit();
            groupBox1.SuspendLayout();
            tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridMetrics).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(224, 224, 224);
            panel1.Controls.Add(logo);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 30);
            panel1.Margin = new Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(261, 570);
            panel1.TabIndex = 0;
            // 
            // logo
            // 
            logo.Image = Properties.Resources.ChatGPT_Image_19_de_dez__de_2025__10_47_48_removebg_preview;
            logo.Location = new Point(-89, -116);
            logo.Margin = new Padding(3, 4, 3, 4);
            logo.Name = "logo";
            logo.Size = new Size(439, 439);
            logo.SizeMode = PictureBoxSizeMode.Zoom;
            logo.TabIndex = 0;
            logo.TabStop = false;
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabPage1);
            tabControl.Controls.Add(tabPage2);
            tabControl.Controls.Add(tabPage3);
            tabControl.Controls.Add(tabPage4);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(261, 30);
            tabControl.Margin = new Padding(3, 4, 3, 4);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(653, 570);
            tabControl.TabIndex = 1;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btnBuscar);
            tabPage1.Controls.Add(cmbServicoMensageria);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(gridMensageria);
            tabPage1.Location = new Point(4, 29);
            tabPage1.Margin = new Padding(3, 4, 3, 4);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3, 4, 3, 4);
            tabPage1.Size = new Size(645, 537);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Mensageria";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(313, 16);
            btnBuscar.Margin = new Padding(3, 4, 3, 4);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(110, 43);
            btnBuscar.TabIndex = 3;
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = true;
            btnBuscar.Click += btnBuscar_Click;
            // 
            // cmbServicoMensageria
            // 
            cmbServicoMensageria.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbServicoMensageria.FormattingEnabled = true;
            cmbServicoMensageria.Items.AddRange(new object[] { "Presece.TabPreco", "Presence.ProdutoSKU" });
            cmbServicoMensageria.Location = new Point(65, 24);
            cmbServicoMensageria.Margin = new Padding(3, 4, 3, 4);
            cmbServicoMensageria.Name = "cmbServicoMensageria";
            cmbServicoMensageria.Size = new Size(241, 28);
            cmbServicoMensageria.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 28);
            label1.Name = "label1";
            label1.Size = new Size(57, 20);
            label1.TabIndex = 1;
            label1.Text = "Serviço";
            // 
            // gridMensageria
            // 
            gridMensageria.AllowUserToAddRows = false;
            gridMensageria.AllowUserToDeleteRows = false;
            gridMensageria.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridMensageria.BackgroundColor = Color.White;
            gridMensageria.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridMensageria.Location = new Point(7, 75);
            gridMensageria.Margin = new Padding(3, 4, 3, 4);
            gridMensageria.Name = "gridMensageria";
            gridMensageria.ReadOnly = true;
            gridMensageria.RowHeadersWidth = 51;
            gridMensageria.Size = new Size(628, 447);
            gridMensageria.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(chcRefresh);
            tabPage2.Controls.Add(txtSize);
            tabPage2.Controls.Add(txtKeyword);
            tabPage2.Controls.Add(label6);
            tabPage2.Controls.Add(cmbStatus);
            tabPage2.Controls.Add(label5);
            tabPage2.Controls.Add(txtFinal);
            tabPage2.Controls.Add(btnBuscarLog);
            tabPage2.Controls.Add(txtInicial);
            tabPage2.Controls.Add(label4);
            tabPage2.Controls.Add(GridLogs);
            tabPage2.Controls.Add(pictureBox1);
            tabPage2.Location = new Point(4, 29);
            tabPage2.Margin = new Padding(3, 4, 3, 4);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3, 4, 3, 4);
            tabPage2.Size = new Size(645, 537);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Logs";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // chcRefresh
            // 
            chcRefresh.AutoSize = true;
            chcRefresh.Location = new Point(419, 17);
            chcRefresh.Margin = new Padding(3, 4, 3, 4);
            chcRefresh.Name = "chcRefresh";
            chcRefresh.Size = new Size(135, 24);
            chcRefresh.TabIndex = 12;
            chcRefresh.Text = "Refresh Aut (5s)";
            chcRefresh.UseVisualStyleBackColor = true;
            chcRefresh.CheckedChanged += chcRefresh_CheckedChanged;
            // 
            // txtSize
            // 
            txtSize.Location = new Point(376, 13);
            txtSize.Margin = new Padding(3, 4, 3, 4);
            txtSize.Name = "txtSize";
            txtSize.Size = new Size(42, 27);
            txtSize.TabIndex = 5;
            txtSize.Text = "50";
            // 
            // txtKeyword
            // 
            txtKeyword.Location = new Point(286, 52);
            txtKeyword.Margin = new Padding(3, 4, 3, 4);
            txtKeyword.Name = "txtKeyword";
            txtKeyword.Size = new Size(253, 27);
            txtKeyword.TabIndex = 7;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(103, 56);
            label6.Name = "label6";
            label6.Size = new Size(49, 20);
            label6.TabIndex = 11;
            label6.Text = "Status";
            // 
            // cmbStatus
            // 
            cmbStatus.FormattingEnabled = true;
            cmbStatus.Items.AddRange(new object[] { "Info", "Warning", "Error", "Assembly", "Payload" });
            cmbStatus.Location = new Point(154, 52);
            cmbStatus.Margin = new Padding(3, 4, 3, 4);
            cmbStatus.Name = "cmbStatus";
            cmbStatus.Size = new Size(124, 28);
            cmbStatus.TabIndex = 6;
            cmbStatus.SelectedIndexChanged += cmbStatus_SelectedIndexChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(255, 17);
            label5.Name = "label5";
            label5.Size = new Size(15, 20);
            label5.TabIndex = 9;
            label5.Text = "-";
            // 
            // txtFinal
            // 
            txtFinal.Location = new Point(275, 13);
            txtFinal.Margin = new Padding(3, 4, 3, 4);
            txtFinal.Mask = "00/00/0000";
            txtFinal.Name = "txtFinal";
            txtFinal.Size = new Size(93, 27);
            txtFinal.TabIndex = 4;
            txtFinal.ValidatingType = typeof(DateTime);
            // 
            // btnBuscarLog
            // 
            btnBuscarLog.Location = new Point(546, 8);
            btnBuscarLog.Margin = new Padding(3, 4, 3, 4);
            btnBuscarLog.Name = "btnBuscarLog";
            btnBuscarLog.Size = new Size(86, 72);
            btnBuscarLog.TabIndex = 8;
            btnBuscarLog.Text = "Buscar";
            btnBuscarLog.UseVisualStyleBackColor = true;
            btnBuscarLog.Click += btnBuscarLog_Click;
            // 
            // txtInicial
            // 
            txtInicial.Location = new Point(154, 13);
            txtInicial.Margin = new Padding(3, 4, 3, 4);
            txtInicial.Mask = "00/00/0000";
            txtInicial.Name = "txtInicial";
            txtInicial.Size = new Size(93, 27);
            txtInicial.TabIndex = 3;
            txtInicial.ValidatingType = typeof(DateTime);
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(103, 17);
            label4.Name = "label4";
            label4.Size = new Size(41, 20);
            label4.TabIndex = 2;
            label4.Text = "Data";
            // 
            // GridLogs
            // 
            GridLogs.AllowUserToAddRows = false;
            GridLogs.AllowUserToDeleteRows = false;
            GridLogs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GridLogs.BackgroundColor = Color.White;
            GridLogs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            GridLogs.Location = new Point(3, 91);
            GridLogs.Margin = new Padding(3, 4, 3, 4);
            GridLogs.Name = "GridLogs";
            GridLogs.ReadOnly = true;
            GridLogs.RowHeadersWidth = 51;
            GridLogs.Size = new Size(629, 461);
            GridLogs.TabIndex = 1;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.png_clipart_logo_elasticsearch_scalable_graphics_distributed_database_text_logo;
            pictureBox1.Location = new Point(7, 8);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(95, 77);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(GridServico);
            tabPage3.Controls.Add(groupBox1);
            tabPage3.Location = new Point(4, 29);
            tabPage3.Margin = new Padding(3, 4, 3, 4);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(645, 537);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Serviços";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // GridServico
            // 
            GridServico.AllowUserToAddRows = false;
            GridServico.AllowUserToDeleteRows = false;
            GridServico.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GridServico.BackgroundColor = Color.White;
            GridServico.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            GridServico.Location = new Point(3, 203);
            GridServico.Margin = new Padding(3, 4, 3, 4);
            GridServico.Name = "GridServico";
            GridServico.ReadOnly = true;
            GridServico.RowHeadersWidth = 51;
            GridServico.Size = new Size(629, 349);
            GridServico.TabIndex = 1;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(btnRefreshSer);
            groupBox1.Controls.Add(txtCodigo);
            groupBox1.Controls.Add(btnGravar);
            groupBox1.Controls.Add(txtMetodo);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(txtClasse);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(chAtivoServico);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(txtNomeServico);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(3, 4);
            groupBox1.Margin = new Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 4, 3, 4);
            groupBox1.Size = new Size(632, 191);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Cadastro";
            // 
            // btnRefreshSer
            // 
            btnRefreshSer.Location = new Point(486, 23);
            btnRefreshSer.Margin = new Padding(3, 4, 3, 4);
            btnRefreshSer.Name = "btnRefreshSer";
            btnRefreshSer.Size = new Size(136, 39);
            btnRefreshSer.TabIndex = 11;
            btnRefreshSer.Text = "Refresh";
            btnRefreshSer.UseVisualStyleBackColor = true;
            btnRefreshSer.Click += btnRefreshSer_ClickAsync;
            // 
            // txtCodigo
            // 
            txtCodigo.BorderStyle = BorderStyle.FixedSingle;
            txtCodigo.Enabled = false;
            txtCodigo.Location = new Point(377, 23);
            txtCodigo.Margin = new Padding(3, 4, 3, 4);
            txtCodigo.Name = "txtCodigo";
            txtCodigo.Size = new Size(101, 27);
            txtCodigo.TabIndex = 10;
            // 
            // btnGravar
            // 
            btnGravar.Location = new Point(377, 131);
            btnGravar.Margin = new Padding(3, 4, 3, 4);
            btnGravar.Name = "btnGravar";
            btnGravar.Size = new Size(134, 41);
            btnGravar.TabIndex = 5;
            btnGravar.Text = "&Gravar";
            btnGravar.UseVisualStyleBackColor = true;
            btnGravar.Click += btnGravar_Click;
            // 
            // txtMetodo
            // 
            txtMetodo.BorderStyle = BorderStyle.FixedSingle;
            txtMetodo.Location = new Point(149, 139);
            txtMetodo.Margin = new Padding(3, 4, 3, 4);
            txtMetodo.Name = "txtMetodo";
            txtMetodo.Size = new Size(221, 27);
            txtMetodo.TabIndex = 4;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(7, 141);
            label8.Name = "label8";
            label8.Size = new Size(65, 20);
            label8.TabIndex = 7;
            label8.Text = "Método:";
            // 
            // txtClasse
            // 
            txtClasse.BorderStyle = BorderStyle.FixedSingle;
            txtClasse.Location = new Point(149, 100);
            txtClasse.Margin = new Padding(3, 4, 3, 4);
            txtClasse.Name = "txtClasse";
            txtClasse.Size = new Size(162, 27);
            txtClasse.TabIndex = 3;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(7, 103);
            label7.Name = "label7";
            label7.Size = new Size(53, 20);
            label7.TabIndex = 5;
            label7.Text = "Classe:";
            // 
            // chAtivoServico
            // 
            chAtivoServico.AutoSize = true;
            chAtivoServico.FlatStyle = FlatStyle.Flat;
            chAtivoServico.Location = new Point(223, 61);
            chAtivoServico.Margin = new Padding(3, 4, 3, 4);
            chAtivoServico.Name = "chAtivoServico";
            chAtivoServico.Size = new Size(62, 24);
            chAtivoServico.TabIndex = 2;
            chAtivoServico.Text = "Ativo";
            chAtivoServico.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.Location = new Point(149, 61);
            textBox1.Margin = new Padding(3, 4, 3, 4);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(67, 27);
            textBox1.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(7, 64);
            label3.Name = "label3";
            label3.Size = new Size(151, 20);
            label3.TabIndex = 2;
            label3.Text = "Time-Set (Segundos):";
            // 
            // txtNomeServico
            // 
            txtNomeServico.BorderStyle = BorderStyle.FixedSingle;
            txtNomeServico.CharacterCasing = CharacterCasing.Upper;
            txtNomeServico.Location = new Point(149, 23);
            txtNomeServico.Margin = new Padding(3, 4, 3, 4);
            txtNomeServico.Name = "txtNomeServico";
            txtNomeServico.Size = new Size(221, 27);
            txtNomeServico.TabIndex = 0;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(7, 25);
            label2.Name = "label2";
            label2.Size = new Size(127, 20);
            label2.TabIndex = 0;
            label2.Text = "Nome do Serviço:";
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(pnGrafico);
            tabPage4.Controls.Add(gridMetrics);
            tabPage4.Location = new Point(4, 29);
            tabPage4.Margin = new Padding(3, 4, 3, 4);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3, 4, 3, 4);
            tabPage4.Size = new Size(645, 537);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Métricas";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // pnGrafico
            // 
            pnGrafico.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnGrafico.Location = new Point(9, 176);
            pnGrafico.Margin = new Padding(3, 4, 3, 4);
            pnGrafico.Name = "pnGrafico";
            pnGrafico.Size = new Size(626, 376);
            pnGrafico.TabIndex = 9;
            // 
            // gridMetrics
            // 
            gridMetrics.AllowUserToAddRows = false;
            gridMetrics.AllowUserToDeleteRows = false;
            gridMetrics.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gridMetrics.BackgroundColor = Color.White;
            gridMetrics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridMetrics.Location = new Point(9, 8);
            gridMetrics.Margin = new Padding(3, 4, 3, 4);
            gridMetrics.Name = "gridMetrics";
            gridMetrics.ReadOnly = true;
            gridMetrics.RowHeadersWidth = 51;
            gridMetrics.Size = new Size(629, 160);
            gridMetrics.TabIndex = 8;
            // 
            // tmRabiit
            // 
            tmRabiit.Enabled = true;
            tmRabiit.Interval = 5000;
            tmRabiit.Tick += tmRabiit_Tick;
            // 
            // tmTela
            // 
            tmTela.Enabled = true;
            tmTela.Interval = 1000;
            tmTela.Tick += tmTela_Tick;
            // 
            // tmRefreshLogs
            // 
            tmRefreshLogs.Interval = 5000;
            tmRefreshLogs.Tick += tmRefreshLogs_Tick;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { importaçãoToolStripMenuItem, dataViewToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 3, 0, 3);
            menuStrip1.Size = new Size(914, 30);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            // 
            // importaçãoToolStripMenuItem
            // 
            importaçãoToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { viaEXCELToolStripMenuItem, exportarDadosDeEPCToolStripMenuItem, importarImagensToolStripMenuItem });
            importaçãoToolStripMenuItem.Name = "importaçãoToolStripMenuItem";
            importaçãoToolStripMenuItem.Size = new Size(100, 24);
            importaçãoToolStripMenuItem.Text = "Importação";
            // 
            // viaEXCELToolStripMenuItem
            // 
            viaEXCELToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { produtosSKUToolStripMenuItem, produtosToolStripMenuItem, marcasToolStripMenuItem, deparToolStripMenuItem, gruposToolStripMenuItem, subGruposToolStripMenuItem, eCsToolStripMenuItem });
            viaEXCELToolStripMenuItem.Name = "viaEXCELToolStripMenuItem";
            viaEXCELToolStripMenuItem.Size = new Size(243, 26);
            viaEXCELToolStripMenuItem.Text = "Via Excel";
            // 
            // produtosSKUToolStripMenuItem
            // 
            produtosSKUToolStripMenuItem.Name = "produtosSKUToolStripMenuItem";
            produtosSKUToolStripMenuItem.Size = new Size(195, 26);
            produtosSKUToolStripMenuItem.Text = "Produtos SKU";
            produtosSKUToolStripMenuItem.Click += produtosSKUToolStripMenuItem_Click;
            // 
            // produtosToolStripMenuItem
            // 
            produtosToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { planilhaAPIToolStripMenuItem, alteraçãoToolStripMenuItem });
            produtosToolStripMenuItem.Name = "produtosToolStripMenuItem";
            produtosToolStripMenuItem.Size = new Size(195, 26);
            produtosToolStripMenuItem.Text = "Produtos";
            // 
            // planilhaAPIToolStripMenuItem
            // 
            planilhaAPIToolStripMenuItem.Name = "planilhaAPIToolStripMenuItem";
            planilhaAPIToolStripMenuItem.Size = new Size(172, 26);
            planilhaAPIToolStripMenuItem.Text = "Planilha/API";
            planilhaAPIToolStripMenuItem.Click += planilhaAPIToolStripMenuItem_Click;
            // 
            // alteraçãoToolStripMenuItem
            // 
            alteraçãoToolStripMenuItem.Name = "alteraçãoToolStripMenuItem";
            alteraçãoToolStripMenuItem.Size = new Size(172, 26);
            alteraçãoToolStripMenuItem.Text = "Alteração";
            // 
            // marcasToolStripMenuItem
            // 
            marcasToolStripMenuItem.Name = "marcasToolStripMenuItem";
            marcasToolStripMenuItem.Size = new Size(195, 26);
            marcasToolStripMenuItem.Text = "Marcas";
            // 
            // deparToolStripMenuItem
            // 
            deparToolStripMenuItem.Name = "deparToolStripMenuItem";
            deparToolStripMenuItem.Size = new Size(195, 26);
            deparToolStripMenuItem.Text = "Departamentos";
            // 
            // gruposToolStripMenuItem
            // 
            gruposToolStripMenuItem.Name = "gruposToolStripMenuItem";
            gruposToolStripMenuItem.Size = new Size(195, 26);
            gruposToolStripMenuItem.Text = "Grupos";
            // 
            // subGruposToolStripMenuItem
            // 
            subGruposToolStripMenuItem.Name = "subGruposToolStripMenuItem";
            subGruposToolStripMenuItem.Size = new Size(195, 26);
            subGruposToolStripMenuItem.Text = "Sub-Grupos";
            // 
            // eCsToolStripMenuItem
            // 
            eCsToolStripMenuItem.Name = "eCsToolStripMenuItem";
            eCsToolStripMenuItem.Size = new Size(195, 26);
            eCsToolStripMenuItem.Text = "EC's";
            // 
            // exportarDadosDeEPCToolStripMenuItem
            // 
            exportarDadosDeEPCToolStripMenuItem.Name = "exportarDadosDeEPCToolStripMenuItem";
            exportarDadosDeEPCToolStripMenuItem.Size = new Size(243, 26);
            exportarDadosDeEPCToolStripMenuItem.Text = "Exportar dados de EPC";
            exportarDadosDeEPCToolStripMenuItem.Click += exportarDadosDeEPCToolStripMenuItem_Click;
            // 
            // dataViewToolStripMenuItem
            // 
            dataViewToolStripMenuItem.Name = "dataViewToolStripMenuItem";
            dataViewToolStripMenuItem.Size = new Size(91, 24);
            dataViewToolStripMenuItem.Text = "Data View";
            // 
            // importarImagensToolStripMenuItem
            // 
            importarImagensToolStripMenuItem.Name = "importarImagensToolStripMenuItem";
            importarImagensToolStripMenuItem.Size = new Size(243, 26);
            importarImagensToolStripMenuItem.Text = "Importar Imagens";
            importarImagensToolStripMenuItem.Click += importarImagensToolStripMenuItem_Click;
            // 
            // MDI
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(914, 600);
            Controls.Add(tabControl);
            Controls.Add(panel1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new Padding(3, 4, 3, 4);
            Name = "MDI";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Conector4";
            WindowState = FormWindowState.Maximized;
            Load += MDI_Load;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)logo).EndInit();
            tabControl.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridMensageria).EndInit();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)GridLogs).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)GridServico).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridMetrics).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private TabControl tabControl;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private PictureBox logo;
        private System.Windows.Forms.Timer tmRabiit;
        private ComboBox cmbServicoMensageria;
        private Label label1;
        private DataGridView gridMensageria;
        private Button btnBuscar;
        private System.Windows.Forms.Timer tmTela;
        private PictureBox pictureBox1;
        private DataGridView GridLogs;
        private Button btnBuscarLog;
        private MaskedTextBox txtInicial;
        private Label label4;
        private TextBox txtKeyword;
        private Label label6;
        private ComboBox cmbStatus;
        private Label label5;
        private MaskedTextBox txtFinal;
        private TextBox txtSize;
        private CheckBox chcRefresh;
        private System.Windows.Forms.Timer tmRefreshLogs;
        private TabPage tabPage4;
        private DataGridView gridMetrics;
        private GroupBox groupBox1;
        private TextBox txtNomeServico;
        private Label label2;
        private TextBox textBox1;
        private Label label3;
        private TextBox txtMetodo;
        private Label label8;
        private TextBox txtClasse;
        private Label label7;
        private CheckBox chAtivoServico;
        private Button btnGravar;
        private TextBox txtCodigo;
        private DataGridView GridServico;
        private Button btnRefreshSer;
        private Panel pnGrafico;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem importaçãoToolStripMenuItem;
        private ToolStripMenuItem viaEXCELToolStripMenuItem;
        private ToolStripMenuItem produtosSKUToolStripMenuItem;
        private ToolStripMenuItem produtosToolStripMenuItem;
        private ToolStripMenuItem marcasToolStripMenuItem;
        private ToolStripMenuItem deparToolStripMenuItem;
        private ToolStripMenuItem gruposToolStripMenuItem;
        private ToolStripMenuItem subGruposToolStripMenuItem;
        private ToolStripMenuItem eCsToolStripMenuItem;
        private ToolStripMenuItem dataViewToolStripMenuItem;
        private ToolStripMenuItem planilhaAPIToolStripMenuItem;
        private ToolStripMenuItem alteraçãoToolStripMenuItem;
        private ToolStripMenuItem exportarDadosDeEPCToolStripMenuItem;
        private ToolStripMenuItem importarImagensToolStripMenuItem;
    }
}
