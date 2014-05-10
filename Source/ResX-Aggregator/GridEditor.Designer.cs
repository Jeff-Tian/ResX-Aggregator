using ZiZhuJY.UI.UserControl;

namespace ZiZhuJY.ResX_Aggregator
{
    partial class GridEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this._dataGridControl = new ExcelDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this._dataGridControl)).BeginInit();
            this.SuspendLayout();
            // 
            // _dataGridControl
            // 
            this._dataGridControl.AllowUserToOrderColumns = true;
            this._dataGridControl.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this._dataGridControl.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this._dataGridControl.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._dataGridControl.DefaultCellStyle = dataGridViewCellStyle1;
            this._dataGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dataGridControl.Location = new System.Drawing.Point(0, 0);
            this._dataGridControl.Name = "_dataGridControl";
            this._dataGridControl.RowTemplate.Height = 23;
            this._dataGridControl.Size = new System.Drawing.Size(1253, 538);
            this._dataGridControl.TabIndex = 0;
            this._dataGridControl.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellMouseEnter);
            this._dataGridControl.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this._dataGridControl.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this._dataGridControl_EditingControlShowing);
            this._dataGridControl.BindingContextChanged += new System.EventHandler(this.dataGridView1_BindingContextChanged);
            this._dataGridControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            this._dataGridControl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dataGridView1_KeyPress);
            // 
            // GridEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._dataGridControl);
            this.Name = "GridEditor";
            this.Size = new System.Drawing.Size(1253, 538);
            ((System.ComponentModel.ISupportInitialize)(this._dataGridControl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
