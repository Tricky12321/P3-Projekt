namespace P3_Projekt
{
    partial class BoerglumAbbeyStorageandSale
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataGridView_Receipt = new System.Windows.Forms.DataGridView();
            this.column_Product = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Image = new System.Windows.Forms.DataGridViewImageColumn();
            this.button1 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Receipt)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.ItemSize = new System.Drawing.Size(100, 50);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(30, 3);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1895, 1025);
            this.tabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView_Receipt);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Location = new System.Drawing.Point(4, 54);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1887, 967);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Salg";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridView_Receipt
            // 
            this.dataGridView_Receipt.AllowUserToAddRows = false;
            this.dataGridView_Receipt.AllowUserToDeleteRows = false;
            this.dataGridView_Receipt.BackgroundColor = System.Drawing.SystemColors.ActiveBorder;
            this.dataGridView_Receipt.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView_Receipt.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Receipt.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.column_Product,
            this.Column_Image});
            this.dataGridView_Receipt.Location = new System.Drawing.Point(1087, 6);
            this.dataGridView_Receipt.Name = "dataGridView_Receipt";
            this.dataGridView_Receipt.ReadOnly = true;
            this.dataGridView_Receipt.RowTemplate.Height = 28;
            this.dataGridView_Receipt.Size = new System.Drawing.Size(689, 859);
            this.dataGridView_Receipt.TabIndex = 3;
            // 
            // column_Product
            // 
            this.column_Product.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.column_Product.HeaderText = "Produkt";
            this.column_Product.Name = "column_Product";
            this.column_Product.ReadOnly = true;
            // 
            // Column_Image
            // 
            this.Column_Image.HeaderText = "";
            this.Column_Image.Image = global::P3_Projekt.Properties.Resources.Red_Cross;
            this.Column_Image.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Stretch;
            this.Column_Image.Name = "Column_Image";
            this.Column_Image.ReadOnly = true;
            this.Column_Image.Width = 30;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(881, 386);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 58);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 54);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1887, 967);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Lager";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 54);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1887, 967);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Statistics";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // BoerglumAbbeyStorageandSale
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1898, 1024);
            this.Controls.Add(this.tabControl);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "BoerglumAbbeyStorageandSale";
            this.Text = "Børglum Kloster Lager og Salg";
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Receipt)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView_Receipt;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_Product;
        private System.Windows.Forms.DataGridViewImageColumn Column_Image;
    }
}

