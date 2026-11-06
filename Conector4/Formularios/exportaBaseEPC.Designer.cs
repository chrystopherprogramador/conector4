namespace Conector4.Formularios
{
    partial class exportaBaseEPC
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
            groupBox1 = new System.Windows.Forms.GroupBox();
            lblTotalRegistros = new System.Windows.Forms.Label();
            lblStatus = new System.Windows.Forms.Label();
            progressBar = new System.Windows.Forms.ProgressBar();
            btnExportar = new System.Windows.Forms.Button();
            txtLoja = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lblTotalRegistros);
            groupBox1.Controls.Add(lblStatus);
            groupBox1.Controls.Add(progressBar);
            groupBox1.Controls.Add(btnExportar);
            groupBox1.Controls.Add(txtLoja);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new System.Drawing.Point(14, 80);
            groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            groupBox1.Size = new System.Drawing.Size(656, 200);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Configurações de Exportação";
            // 
            // lblTotalRegistros
            // 
            lblTotalRegistros.AutoSize = true;
            lblTotalRegistros.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            lblTotalRegistros.ForeColor = System.Drawing.Color.FromArgb(0, 120, 215);
            lblTotalRegistros.Location = new System.Drawing.Point(20, 165);
            lblTotalRegistros.Name = "lblTotalRegistros";
            lblTotalRegistros.Size = new System.Drawing.Size(133, 23);
            lblTotalRegistros.TabIndex = 5;
            lblTotalRegistros.Text = "Total: 0 registros";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new System.Drawing.Point(20, 95);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(136, 20);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Pronto para exportar";
            // 
            // progressBar
            // 
            progressBar.Location = new System.Drawing.Point(20, 125);
            progressBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(615, 30);
            progressBar.TabIndex = 3;
            // 
            // btnExportar
            // 
            btnExportar.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            btnExportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnExportar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnExportar.ForeColor = System.Drawing.Color.White;
            btnExportar.Location = new System.Drawing.Point(450, 35);
            btnExportar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnExportar.Name = "btnExportar";
            btnExportar.Size = new System.Drawing.Size(185, 40);
            btnExportar.TabIndex = 2;
            btnExportar.Text = "?? Exportar Base EPC";
            btnExportar.UseVisualStyleBackColor = false;
            btnExportar.Click += btnExportar_Click;
            // 
            // txtLoja
            // 
            txtLoja.Font = new System.Drawing.Font("Segoe UI", 12F);
            txtLoja.Location = new System.Drawing.Point(150, 38);
            txtLoja.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtLoja.MaxLength = 10;
            txtLoja.Name = "txtLoja";
            txtLoja.Size = new System.Drawing.Size(120, 34);
            txtLoja.TabIndex = 1;
            txtLoja.Text = "03";
            txtLoja.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 10F);
            label1.Location = new System.Drawing.Point(20, 43);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(124, 23);
            label1.TabIndex = 0;
            label1.Text = "Código da Loja:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            label2.ForeColor = System.Drawing.Color.FromArgb(0, 120, 215);
            label2.Location = new System.Drawing.Point(80, 20);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(328, 37);
            label2.TabIndex = 1;
            label2.Text = "Exportação Base de EPC";
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new System.Drawing.Point(14, 12);
            pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(60, 60);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // exportaBaseEPC
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(684, 295);
            Controls.Add(pictureBox1);
            Controls.Add(label2);
            Controls.Add(groupBox1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "exportaBaseEPC";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Exportar Base de EPC";
            Load += exportaBaseEPC_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLoja;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblTotalRegistros;
    }
}
