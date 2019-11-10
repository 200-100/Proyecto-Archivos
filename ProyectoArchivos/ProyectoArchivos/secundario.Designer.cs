namespace ProyectoArchivos
{
    partial class secundario
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
            this.comboFK = new System.Windows.Forms.ComboBox();
            this.comboAtributosFK = new System.Windows.Forms.ComboBox();
            this.comboSub_BloqueFK = new System.Windows.Forms.ComboBox();
            this.dtGrid_FK = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dtGrid_Sub_BloquesFK = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dtGrid_FK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtGrid_Sub_BloquesFK)).BeginInit();
            this.SuspendLayout();
            // 
            // comboFK
            // 
            this.comboFK.FormattingEnabled = true;
            this.comboFK.Location = new System.Drawing.Point(22, 57);
            this.comboFK.Name = "comboFK";
            this.comboFK.Size = new System.Drawing.Size(158, 21);
            this.comboFK.TabIndex = 8;
            // 
            // comboAtributosFK
            // 
            this.comboAtributosFK.FormattingEnabled = true;
            this.comboAtributosFK.Location = new System.Drawing.Point(22, 99);
            this.comboAtributosFK.Name = "comboAtributosFK";
            this.comboAtributosFK.Size = new System.Drawing.Size(158, 21);
            this.comboAtributosFK.TabIndex = 9;
            // 
            // comboSub_BloqueFK
            // 
            this.comboSub_BloqueFK.FormattingEnabled = true;
            this.comboSub_BloqueFK.Location = new System.Drawing.Point(22, 144);
            this.comboSub_BloqueFK.Name = "comboSub_BloqueFK";
            this.comboSub_BloqueFK.Size = new System.Drawing.Size(158, 21);
            this.comboSub_BloqueFK.TabIndex = 10;
            // 
            // dtGrid_FK
            // 
            this.dtGrid_FK.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtGrid_FK.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this.dtGrid_FK.Location = new System.Drawing.Point(206, 33);
            this.dtGrid_FK.Name = "dtGrid_FK";
            this.dtGrid_FK.ReadOnly = true;
            this.dtGrid_FK.Size = new System.Drawing.Size(247, 405);
            this.dtGrid_FK.TabIndex = 11;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "ID";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Dirección";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dtGrid_Sub_BloquesFK
            // 
            this.dtGrid_Sub_BloquesFK.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtGrid_Sub_BloquesFK.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2});
            this.dtGrid_Sub_BloquesFK.Location = new System.Drawing.Point(474, 33);
            this.dtGrid_Sub_BloquesFK.Name = "dtGrid_Sub_BloquesFK";
            this.dtGrid_Sub_BloquesFK.ReadOnly = true;
            this.dtGrid_Sub_BloquesFK.Size = new System.Drawing.Size(169, 405);
            this.dtGrid_Sub_BloquesFK.TabIndex = 12;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Direcciones de la Información";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // secundario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dtGrid_Sub_BloquesFK);
            this.Controls.Add(this.dtGrid_FK);
            this.Controls.Add(this.comboSub_BloqueFK);
            this.Controls.Add(this.comboAtributosFK);
            this.Controls.Add(this.comboFK);
            this.Name = "secundario";
            this.Text = "secundario";
            ((System.ComponentModel.ISupportInitialize)(this.dtGrid_FK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtGrid_Sub_BloquesFK)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboFK;
        private System.Windows.Forms.ComboBox comboAtributosFK;
        private System.Windows.Forms.ComboBox comboSub_BloqueFK;
        private System.Windows.Forms.DataGridView dtGrid_FK;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridView dtGrid_Sub_BloquesFK;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    }
}