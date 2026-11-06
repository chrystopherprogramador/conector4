namespace Conector4.Formularios
{
    partial class importProductXls
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
            btnBaixaLayout = new Button();
            btnImportar = new Button();
            label8 = new Label();
            txtAte = new Label();
            label7 = new Label();
            txtDe = new Label();
            label5 = new Label();
            ProgressBar = new Conector4.Controls.SegmentedProgressBar();
            disTotal = new Label();
            label2 = new Label();
            btnBuscar = new Button();
            txtCaminho = new TextBox();
            label1 = new Label();
            gridResult = new DataGridView();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridResult).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnBaixaLayout);
            groupBox1.Controls.Add(btnImportar);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(txtAte);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(txtDe);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(ProgressBar);
            groupBox1.Controls.Add(disTotal);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(btnBuscar);
            groupBox1.Controls.Add(txtCaminho);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 3);
            groupBox1.Margin = new Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 4, 3, 4);
            groupBox1.Size = new Size(887, 169);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Configurações";
            // 
            // btnBaixaLayout
            // 
            btnBaixaLayout.Location = new Point(672, 10);
            btnBaixaLayout.Name = "btnBaixaLayout";
            btnBaixaLayout.Size = new Size(208, 26);
            btnBaixaLayout.TabIndex = 2;
            btnBaixaLayout.Text = "Baixar Layout";
            btnBaixaLayout.UseVisualStyleBackColor = true;
            // 
            // btnImportar
            // 
            btnImportar.Location = new Point(779, 37);
            btnImportar.Margin = new Padding(3, 4, 3, 4);
            btnImportar.Name = "btnImportar";
            btnImportar.Size = new Size(101, 31);
            btnImportar.TabIndex = 10;
            btnImportar.Text = "Importar";
            btnImportar.UseVisualStyleBackColor = true;
            btnImportar.Click += btnImportar_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(595, 85);
            label8.Name = "label8";
            label8.Size = new Size(75, 20);
            label8.TabIndex = 9;
            label8.Text = "registros...";
            // 
            // txtAte
            // 
            txtAte.AutoSize = true;
            txtAte.Location = new Point(547, 85);
            txtAte.Name = "txtAte";
            txtAte.Size = new Size(33, 20);
            txtAte.TabIndex = 8;
            txtAte.Text = "000";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(512, 85);
            label7.Name = "label7";
            label7.Size = new Size(26, 20);
            label7.TabIndex = 7;
            label7.Text = "de";
            // 
            // txtDe
            // 
            txtDe.AutoSize = true;
            txtDe.Location = new Point(464, 85);
            txtDe.Name = "txtDe";
            txtDe.Size = new Size(33, 20);
            txtDe.TabIndex = 6;
            txtDe.Text = "000";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(377, 85);
            label5.Name = "label5";
            label5.Size = new Size(88, 20);
            label5.TabIndex = 5;
            label5.Text = "Importando";
            // 
            // ProgressBar
            // 
            ProgressBar.Location = new Point(7, 108);
            ProgressBar.Margin = new Padding(3, 4, 3, 4);
            ProgressBar.Name = "ProgressBar";
            ProgressBar.Size = new Size(873, 41);
            ProgressBar.TabIndex = 1;
            // 
            // disTotal
            // 
            disTotal.AutoSize = true;
            disTotal.Location = new Point(111, 85);
            disTotal.Name = "disTotal";
            disTotal.Size = new Size(33, 20);
            disTotal.TabIndex = 4;
            disTotal.Text = "000";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(7, 85);
            label2.Name = "label2";
            label2.Size = new Size(112, 20);
            label2.TabIndex = 3;
            label2.Text = "Nº de Registros";
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(672, 37);
            btnBuscar.Margin = new Padding(3, 4, 3, 4);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(105, 32);
            btnBuscar.TabIndex = 2;
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = true;
            btnBuscar.Click += btnBuscar_Click;
            // 
            // txtCaminho
            // 
            txtCaminho.Enabled = false;
            txtCaminho.Location = new Point(111, 39);
            txtCaminho.Margin = new Padding(3, 4, 3, 4);
            txtCaminho.Name = "txtCaminho";
            txtCaminho.Size = new Size(554, 27);
            txtCaminho.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 44);
            label1.Name = "label1";
            label1.Size = new Size(106, 20);
            label1.TabIndex = 0;
            label1.Text = "Caminho Excel";
            // 
            // gridResult
            // 
            gridResult.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridResult.Location = new Point(12, 180);
            gridResult.Margin = new Padding(3, 4, 3, 4);
            gridResult.Name = "gridResult";
            gridResult.RowHeadersWidth = 51;
            gridResult.Size = new Size(887, 391);
            gridResult.TabIndex = 3;
            // 
            // importProductXls
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(918, 640);
            Controls.Add(groupBox1);
            Controls.Add(gridResult);
            Name = "importProductXls";
            Text = "importProductXls";
            Load += importProductXls_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridResult).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Button btnBaixaLayout;
        private Button btnImportar;
        private Label label8;
        private Label txtAte;
        private Label label7;
        private Label txtDe;
        private Label label5;
        private Controls.SegmentedProgressBar ProgressBar;
        private Label disTotal;
        private Label label2;
        private Button btnBuscar;
        private TextBox txtCaminho;
        private Label label1;
        private DataGridView gridResult;
    }
}