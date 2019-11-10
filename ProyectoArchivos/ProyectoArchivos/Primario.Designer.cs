namespace ProyectoArchivos
{
    partial class Primario
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
            this.comboPK = new System.Windows.Forms.ComboBox();
            this.comboSub_BloquePK = new System.Windows.Forms.ComboBox();
            this.dtGrid_PK = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Direccion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dtGrid_Sub_BloquesPK = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dtGrid_PK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtGrid_Sub_BloquesPK)).BeginInit();
            this.SuspendLayout();
            // 
            // comboPK
            // 
            this.comboPK.FormattingEnabled = true;
            this.comboPK.Location = new System.Drawing.Point(134, 36);
            this.comboPK.Name = "comboPK";
            this.comboPK.Size = new System.Drawing.Size(254, 21);
            this.comboPK.TabIndex = 2;
            // 
            // comboSub_BloquePK
            // 
            this.comboSub_BloquePK.FormattingEnabled = true;
            this.comboSub_BloquePK.Location = new System.Drawing.Point(416, 36);
            this.comboSub_BloquePK.Name = "comboSub_BloquePK";
            this.comboSub_BloquePK.Size = new System.Drawing.Size(254, 21);
            this.comboSub_BloquePK.TabIndex = 3;
            // 
            // dtGrid_PK
            // 
            this.dtGrid_PK.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtGrid_PK.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.Direccion});
            this.dtGrid_PK.Location = new System.Drawing.Point(134, 66);
            this.dtGrid_PK.Name = "dtGrid_PK";
            this.dtGrid_PK.ReadOnly = true;
            this.dtGrid_PK.Size = new System.Drawing.Size(254, 372);
            this.dtGrid_PK.TabIndex = 4;
            // 
            // ID
            // 
            this.ID.HeaderText = "Información";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            // 
            // Direccion
            // 
            this.Direccion.HeaderText = "Dirección";
            this.Direccion.Name = "Direccion";
            this.Direccion.ReadOnly = true;
            // 
            // dtGrid_Sub_BloquesPK
            // 
            this.dtGrid_Sub_BloquesPK.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtGrid_Sub_BloquesPK.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6});
            this.dtGrid_Sub_BloquesPK.Location = new System.Drawing.Point(416, 66);
            this.dtGrid_Sub_BloquesPK.Name = "dtGrid_Sub_BloquesPK";
            this.dtGrid_Sub_BloquesPK.ReadOnly = true;
            this.dtGrid_Sub_BloquesPK.Size = new System.Drawing.Size(254, 372);
            this.dtGrid_Sub_BloquesPK.TabIndex = 5;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Información";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Dirección";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // Primario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dtGrid_Sub_BloquesPK);
            this.Controls.Add(this.dtGrid_PK);
            this.Controls.Add(this.comboSub_BloquePK);
            this.Controls.Add(this.comboPK);
            this.Name = "Primario";
            this.Text = "Primario";
            this.Load += new System.EventHandler(this.Primario_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtGrid_PK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtGrid_Sub_BloquesPK)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboPK;
        private System.Windows.Forms.ComboBox comboSub_BloquePK;
        private System.Windows.Forms.DataGridView dtGrid_PK;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Direccion;
        private System.Windows.Forms.DataGridView dtGrid_Sub_BloquesPK;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    }
}