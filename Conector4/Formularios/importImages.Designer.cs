namespace Conector4.Formularios
{
    partial class importImages
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            lblTotal = new Label();
            lblProgresso = new Label();
            label4 = new Label();
            progressBar = new ProgressBar();
            btnBaixarImagens = new Button();
            btnBuscarPastaDestino = new Button();
            txtPastaDestino = new TextBox();
            label3 = new Label();
            btnBuscarExcel = new Button();
            txtCaminhoExcel = new TextBox();
            label2 = new Label();
            label1 = new Label();
            gridResult = new DataGridView();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridResult).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lblTotal);
            groupBox1.Controls.Add(lblProgresso);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(progressBar);
            groupBox1.Controls.Add(btnBaixarImagens);
            groupBox1.Controls.Add(btnBuscarPastaDestino);
            groupBox1.Controls.Add(txtPastaDestino);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(btnBuscarExcel);
            groupBox1.Controls.Add(txtCaminhoExcel);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(960, 200);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Configurações";
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTotal.Location = new Point(20, 170);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(88, 15);
            lblTotal.TabIndex = 11;
            lblTotal.Text = "Total: 0 itens";
            // 
            // lblProgresso
            // 
            lblProgresso.AutoSize = true;
            lblProgresso.Location = new Point(750, 140);
            lblProgresso.Name = "lblProgresso";
            lblProgresso.Size = new Size(30, 15);
            lblProgresso.TabIndex = 10;
            lblProgresso.Text = "0 / 0";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(20, 140);
            label4.Name = "label4";
            label4.Size = new Size(62, 15);
            label4.TabIndex = 9;
            label4.Text = "Progresso:";
            // 
            // progressBar
            // 
            progressBar.Location = new Point(100, 137);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(640, 23);
            progressBar.TabIndex = 8;
            // 
            // btnBaixarImagens
            // 
            btnBaixarImagens.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnBaixarImagens.Location = new Point(750, 95);
            btnBaixarImagens.Name = "btnBaixarImagens";
            btnBaixarImagens.Size = new Size(190, 30);
            btnBaixarImagens.TabIndex = 7;
            btnBaixarImagens.Text = "?? Baixar Imagens";
            btnBaixarImagens.UseVisualStyleBackColor = true;
            btnBaixarImagens.Click += btnBaixarImagens_Click;
            // 
            // btnBuscarPastaDestino
            // 
            btnBuscarPastaDestino.Location = new Point(640, 95);
            btnBuscarPastaDestino.Name = "btnBuscarPastaDestino";
            btnBuscarPastaDestino.Size = new Size(100, 27);
            btnBuscarPastaDestino.TabIndex = 6;
            btnBuscarPastaDestino.Text = "Procurar...";
            btnBuscarPastaDestino.UseVisualStyleBackColor = true;
            btnBuscarPastaDestino.Click += btnBuscarPastaDestino_Click;
            // 
            // txtPastaDestino
            // 
            txtPastaDestino.Enabled = false;
            txtPastaDestino.Location = new Point(100, 97);
            txtPastaDestino.Name = "txtPastaDestino";
            txtPastaDestino.Size = new Size(534, 23);
            txtPastaDestino.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(20, 100);
            label3.Name = "label3";
            label3.Size = new Size(74, 15);
            label3.TabIndex = 4;
            label3.Text = "Pasta Destino:";
            // 
            // btnBuscarExcel
            // 
            btnBuscarExcel.Location = new Point(640, 62);
            btnBuscarExcel.Name = "btnBuscarExcel";
            btnBuscarExcel.Size = new Size(100, 27);
            btnBuscarExcel.TabIndex = 3;
            btnBuscarExcel.Text = "Procurar...";
            btnBuscarExcel.UseVisualStyleBackColor = true;
            btnBuscarExcel.Click += btnBuscarExcel_Click;
            // 
            // txtCaminhoExcel
            // 
            txtCaminhoExcel.Enabled = false;
            txtCaminhoExcel.Location = new Point(100, 64);
            txtCaminhoExcel.Name = "txtCaminhoExcel";
            txtCaminhoExcel.Size = new Size(534, 23);
            txtCaminhoExcel.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(20, 67);
            label2.Name = "label2";
            label2.Size = new Size(76, 15);
            label2.TabIndex = 1;
            label2.Text = "Arquivo Excel:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(20, 25);
            label1.Name = "label1";
            label1.Size = new Size(505, 15);
            label1.TabIndex = 0;
            label1.Text = "O Excel deve ter 5 colunas: Nome, Imagem 1 (URL), Imagem 2 (URL), Imagem 3 (URL), Imagem 4 (URL)";
            // 
            // gridResult
            // 
            gridResult.AllowUserToAddRows = false;
            gridResult.AllowUserToDeleteRows = false;
            gridResult.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridResult.Location = new Point(12, 218);
            gridResult.Name = "gridResult";
            gridResult.ReadOnly = true;
            gridResult.Size = new Size(960, 370);
            gridResult.TabIndex = 1;
            // 
            // importImages
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 600);
            Controls.Add(gridResult);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "importImages";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Importar e Baixar Imagens";
            Load += importImages_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridResult).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Label label1;
        private Button btnBuscarExcel;
        private TextBox txtCaminhoExcel;
        private Label label2;
        private Button btnBuscarPastaDestino;
        private TextBox txtPastaDestino;
        private Label label3;
        private Button btnBaixarImagens;
        private ProgressBar progressBar;
        private Label label4;
        private Label lblProgresso;
        private Label lblTotal;
        private DataGridView gridResult;
    }
}
