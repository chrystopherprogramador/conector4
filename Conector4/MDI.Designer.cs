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
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(224, 224, 224);
            panel1.Controls.Add(logo);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(228, 450);
            panel1.TabIndex = 0;
            // 
            // logo
            // 
            logo.Image = Properties.Resources.ChatGPT_Image_19_de_dez__de_2025__10_47_48_removebg_preview;
            logo.Location = new Point(-78, -87);
            logo.Name = "logo";
            logo.Size = new Size(384, 329);
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
            tabControl.Location = new Point(228, 0);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(572, 450);
            tabControl.TabIndex = 1;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btnBuscar);
            tabPage1.Controls.Add(cmbServicoMensageria);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(gridMensageria);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(564, 422);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Mensageria";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(274, 12);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(96, 32);
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
            cmbServicoMensageria.Location = new Point(57, 18);
            cmbServicoMensageria.Name = "cmbServicoMensageria";
            cmbServicoMensageria.Size = new Size(211, 23);
            cmbServicoMensageria.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 21);
            label1.Name = "label1";
            label1.Size = new Size(45, 15);
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
            gridMensageria.Location = new Point(6, 56);
            gridMensageria.Name = "gridMensageria";
            gridMensageria.ReadOnly = true;
            gridMensageria.Size = new Size(550, 358);
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
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(564, 422);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Logs";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // chcRefresh
            // 
            chcRefresh.AutoSize = true;
            chcRefresh.Location = new Point(367, 13);
            chcRefresh.Name = "chcRefresh";
            chcRefresh.Size = new Size(109, 19);
            chcRefresh.TabIndex = 12;
            chcRefresh.Text = "Refresh Aut (5s)";
            chcRefresh.UseVisualStyleBackColor = true;
            chcRefresh.CheckedChanged += chcRefresh_CheckedChanged;
            // 
            // txtSize
            // 
            txtSize.Location = new Point(329, 10);
            txtSize.Name = "txtSize";
            txtSize.Size = new Size(37, 23);
            txtSize.TabIndex = 5;
            txtSize.Text = "50";
            // 
            // txtKeyword
            // 
            txtKeyword.Location = new Point(250, 39);
            txtKeyword.Name = "txtKeyword";
            txtKeyword.Size = new Size(222, 23);
            txtKeyword.TabIndex = 7;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(90, 42);
            label6.Name = "label6";
            label6.Size = new Size(39, 15);
            label6.TabIndex = 11;
            label6.Text = "Status";
            // 
            // cmbStatus
            // 
            cmbStatus.FormattingEnabled = true;
            cmbStatus.Items.AddRange(new object[] { "Info", "Warning", "Error", "Assembly", "Payload" });
            cmbStatus.Location = new Point(135, 39);
            cmbStatus.Name = "cmbStatus";
            cmbStatus.Size = new Size(109, 23);
            cmbStatus.TabIndex = 6;
            cmbStatus.SelectedIndexChanged += cmbStatus_SelectedIndexChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(223, 13);
            label5.Name = "label5";
            label5.Size = new Size(12, 15);
            label5.TabIndex = 9;
            label5.Text = "-";
            // 
            // txtFinal
            // 
            txtFinal.Location = new Point(241, 10);
            txtFinal.Mask = "00/00/0000";
            txtFinal.Name = "txtFinal";
            txtFinal.Size = new Size(82, 23);
            txtFinal.TabIndex = 4;
            txtFinal.ValidatingType = typeof(DateTime);
            // 
            // btnBuscarLog
            // 
            btnBuscarLog.Location = new Point(478, 6);
            btnBuscarLog.Name = "btnBuscarLog";
            btnBuscarLog.Size = new Size(75, 54);
            btnBuscarLog.TabIndex = 8;
            btnBuscarLog.Text = "Buscar";
            btnBuscarLog.UseVisualStyleBackColor = true;
            btnBuscarLog.Click += btnBuscarLog_Click;
            // 
            // txtInicial
            // 
            txtInicial.Location = new Point(135, 10);
            txtInicial.Mask = "00/00/0000";
            txtInicial.Name = "txtInicial";
            txtInicial.Size = new Size(82, 23);
            txtInicial.TabIndex = 3;
            txtInicial.ValidatingType = typeof(DateTime);
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(90, 13);
            label4.Name = "label4";
            label4.Size = new Size(31, 15);
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
            GridLogs.Location = new Point(3, 68);
            GridLogs.Name = "GridLogs";
            GridLogs.ReadOnly = true;
            GridLogs.Size = new Size(550, 346);
            GridLogs.TabIndex = 1;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.png_clipart_logo_elasticsearch_scalable_graphics_distributed_database_text_logo;
            pictureBox1.Location = new Point(6, 6);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(83, 58);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(GridServico);
            tabPage3.Controls.Add(groupBox1);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(564, 422);
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
            GridServico.Location = new Point(3, 152);
            GridServico.Name = "GridServico";
            GridServico.ReadOnly = true;
            GridServico.Size = new Size(550, 262);
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
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(553, 143);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Cadastro";
            // 
            // btnRefreshSer
            // 
            btnRefreshSer.Location = new Point(425, 17);
            btnRefreshSer.Name = "btnRefreshSer";
            btnRefreshSer.Size = new Size(119, 29);
            btnRefreshSer.TabIndex = 11;
            btnRefreshSer.Text = "Refresh";
            btnRefreshSer.UseVisualStyleBackColor = true;
            btnRefreshSer.Click += btnRefreshSer_ClickAsync;
            // 
            // txtCodigo
            // 
            txtCodigo.BorderStyle = BorderStyle.FixedSingle;
            txtCodigo.Enabled = false;
            txtCodigo.Location = new Point(330, 17);
            txtCodigo.Name = "txtCodigo";
            txtCodigo.Size = new Size(89, 23);
            txtCodigo.TabIndex = 10;
            // 
            // btnGravar
            // 
            btnGravar.Location = new Point(330, 98);
            btnGravar.Name = "btnGravar";
            btnGravar.Size = new Size(117, 31);
            btnGravar.TabIndex = 5;
            btnGravar.Text = "&Gravar";
            btnGravar.UseVisualStyleBackColor = true;
            btnGravar.Click += btnGravar_Click;
            // 
            // txtMetodo
            // 
            txtMetodo.BorderStyle = BorderStyle.FixedSingle;
            txtMetodo.Location = new Point(130, 104);
            txtMetodo.Name = "txtMetodo";
            txtMetodo.Size = new Size(194, 23);
            txtMetodo.TabIndex = 4;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(6, 106);
            label8.Name = "label8";
            label8.Size = new Size(52, 15);
            label8.TabIndex = 7;
            label8.Text = "Método:";
            // 
            // txtClasse
            // 
            txtClasse.BorderStyle = BorderStyle.FixedSingle;
            txtClasse.Location = new Point(130, 75);
            txtClasse.Name = "txtClasse";
            txtClasse.Size = new Size(142, 23);
            txtClasse.TabIndex = 3;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(6, 77);
            label7.Name = "label7";
            label7.Size = new Size(43, 15);
            label7.TabIndex = 5;
            label7.Text = "Classe:";
            // 
            // chAtivoServico
            // 
            chAtivoServico.AutoSize = true;
            chAtivoServico.FlatStyle = FlatStyle.Flat;
            chAtivoServico.Location = new Point(195, 46);
            chAtivoServico.Name = "chAtivoServico";
            chAtivoServico.Size = new Size(51, 19);
            chAtivoServico.TabIndex = 2;
            chAtivoServico.Text = "Ativo";
            chAtivoServico.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.Location = new Point(130, 46);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(59, 23);
            textBox1.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 48);
            label3.Name = "label3";
            label3.Size = new Size(121, 15);
            label3.TabIndex = 2;
            label3.Text = "Time-Set (Segundos):";
            // 
            // txtNomeServico
            // 
            txtNomeServico.BorderStyle = BorderStyle.FixedSingle;
            txtNomeServico.CharacterCasing = CharacterCasing.Upper;
            txtNomeServico.Location = new Point(130, 17);
            txtNomeServico.Name = "txtNomeServico";
            txtNomeServico.Size = new Size(194, 23);
            txtNomeServico.TabIndex = 0;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 19);
            label2.Name = "label2";
            label2.Size = new Size(101, 15);
            label2.TabIndex = 0;
            label2.Text = "Nome do Serviço:";
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(pnGrafico);
            tabPage4.Controls.Add(gridMetrics);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3);
            tabPage4.Size = new Size(564, 422);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Métricas";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // pnGrafico
            // 
            pnGrafico.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnGrafico.Location = new Point(8, 132);
            pnGrafico.Name = "pnGrafico";
            pnGrafico.Size = new Size(548, 282);
            pnGrafico.TabIndex = 9;
            // 
            // gridMetrics
            // 
            gridMetrics.AllowUserToAddRows = false;
            gridMetrics.AllowUserToDeleteRows = false;
            gridMetrics.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gridMetrics.BackgroundColor = Color.White;
            gridMetrics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridMetrics.Location = new Point(8, 6);
            gridMetrics.Name = "gridMetrics";
            gridMetrics.ReadOnly = true;
            gridMetrics.Size = new Size(550, 120);
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
            // MDI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(800, 450);
            Controls.Add(tabControl);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
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
            ResumeLayout(false);
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
    }
}
