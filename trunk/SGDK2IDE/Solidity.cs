using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace SGDK2
{
	/// <summary>
	/// Summary description for Solidity.
	/// </summary>
	public class frmSolidity : System.Windows.Forms.Form
	{
      ProjectDataset.SolidityRow m_Solidity;
      ProjectDataset.TilesetRow m_CategoryTileset = null;

      #region Windows Forms Designer members
      private System.Windows.Forms.Label lblName;
      private System.Windows.Forms.TextBox txtName;
      private System.Windows.Forms.DataGrid grdMappings;
      private SGDK2.DataChangeNotifier dataMonitor;
      private System.ComponentModel.IContainer components;
      #endregion

      #region Initialization and Clean-up
		public frmSolidity()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         String sName;
         Int32 nIdx = 1;
         do
            sName = "New Solidity " + (nIdx++).ToString();
         while (ProjectData.GetSolidity(sName) != null);

         m_Solidity = ProjectData.AddSolidity(sName);
         txtName.Text = sName;
         ProjectData.AcceptChanges();

         InitializeGrid();
      }

      public frmSolidity(ProjectDataset.SolidityRow solidity)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         m_Solidity = solidity;
         txtName.Text = solidity.Name;
         InitializeGrid();
      }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
      #endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.components = new System.ComponentModel.Container();
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmSolidity));
         this.lblName = new System.Windows.Forms.Label();
         this.txtName = new System.Windows.Forms.TextBox();
         this.grdMappings = new System.Windows.Forms.DataGrid();
         this.dataMonitor = new SGDK2.DataChangeNotifier(this.components);
         ((System.ComponentModel.ISupportInitialize)(this.grdMappings)).BeginInit();
         this.SuspendLayout();
         // 
         // lblName
         // 
         this.lblName.Location = new System.Drawing.Point(8, 8);
         this.lblName.Name = "lblName";
         this.lblName.Size = new System.Drawing.Size(56, 21);
         this.lblName.TabIndex = 0;
         this.lblName.Text = "Name:";
         this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtName
         // 
         this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtName.Location = new System.Drawing.Point(64, 8);
         this.txtName.Name = "txtName";
         this.txtName.Size = new System.Drawing.Size(232, 20);
         this.txtName.TabIndex = 1;
         this.txtName.Text = "";
         this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
         this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
         // 
         // grdMappings
         // 
         this.grdMappings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.grdMappings.CaptionText = "Category Shapes";
         this.grdMappings.DataMember = "";
         this.grdMappings.HeaderForeColor = System.Drawing.SystemColors.ControlText;
         this.grdMappings.Location = new System.Drawing.Point(8, 40);
         this.grdMappings.Name = "grdMappings";
         this.grdMappings.Size = new System.Drawing.Size(288, 168);
         this.grdMappings.TabIndex = 7;
         this.grdMappings.CurrentCellChanged += new System.EventHandler(this.grdMappings_CurrentCellChanged);
         // 
         // dataMonitor
         // 
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         this.dataMonitor.SolidityRowDeleted += new SGDK2.ProjectDataset.SolidityRowChangeEventHandler(this.dataMonitor_SolidityRowDeleted);
         // 
         // frmSolidity
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(304, 213);
         this.Controls.Add(this.grdMappings);
         this.Controls.Add(this.txtName);
         this.Controls.Add(this.lblName);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "frmSolidity";
         this.Text = "Solidity";
         ((System.ComponentModel.ISupportInitialize)(this.grdMappings)).EndInit();
         this.ResumeLayout(false);

      }
		#endregion

      #region Private Methods
      private void InitializeGrid()
      {
         grdMappings.SetDataBinding(ProjectData.Solidity, "SoliditySolidityShape");

         DataGridTableStyle ts = new DataGridTableStyle();
         ts.MappingName = "SolidityShape";

         PropertyDescriptorCollection pdc = BindingContext[ProjectData.Solidity, "SoliditySolidityShape"].GetItemProperties();

         DataGridComboBoxColumn col = new DataGridComboBoxColumn(pdc["TilesetName"], ProjectData.Tileset.Select(), ProjectData.Tileset.NameColumn.ColumnName);
         col.MappingName = "TilesetName";
         col.HeaderText = "Tileset";
         ts.GridColumnStyles.Add(col);
         col.Control.SelectionChangeCommitted += new EventHandler(Category_SelectionChangeCommitted);

         col = new DataGridComboBoxColumn(pdc["CategoryName"], new ProjectDataset.CategoryRow[] {}, ProjectData.Category.NameColumn.ColumnName);
         col.MappingName = "CategoryName";
         col.HeaderText = "Category";
         ts.GridColumnStyles.Add(col);

         col = new DataGridComboBoxColumn(pdc["ShapeName"], ProjectData.TileShape.Select(), ProjectData.TileShape.NameColumn.ColumnName);
         col.MappingName = "ShapeName";
         col.HeaderText = "Shape";
         ts.GridColumnStyles.Add(col);

         grdMappings.TableStyles.Add(ts);
      }
      #endregion

      #region Event Handlers
      private void txtName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         string sValid = ProjectData.ValidateName(txtName.Text);

         if (sValid != null)
         {
            if (DialogResult.Cancel == MessageBox.Show(this, sValid, "Solidity Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtName.Text = m_Solidity.Name;
            e.Cancel = true;
         }
         ProjectDataset.SolidityRow tr = ProjectData.GetSolidity(txtName.Text);
         if ((null != tr) && (m_Solidity != tr))
         {
            if (DialogResult.Cancel == MessageBox.Show(this, txtName.Text + " already exists", "Solidity Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtName.Text = m_Solidity.Name;
            e.Cancel = true;
         }               
      }

      private void dataMonitor_SolidityRowDeleted(object sender, SGDK2.ProjectDataset.SolidityRowChangeEvent e)
      {
         if (e.Row == m_Solidity)
            this.Close();
      }

      private void dataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void txtName_Validated(object sender, System.EventArgs e)
      {
         m_Solidity.Name = txtName.Text;
         ProjectData.AcceptChanges();
      }

      private void grdMappings_CurrentCellChanged(object sender, System.EventArgs e)
      {
         CurrencyManager cm = ((CurrencyManager)grdMappings.BindingContext[ProjectData.Solidity, "SoliditySolidityShape"]);

         if (grdMappings.CurrentRowIndex < 0)
            return;

         ProjectDataset.SolidityShapeRow dr = (ProjectDataset.SolidityShapeRow)((DataRowView)cm.Current).Row;

         DataGridComboBoxColumn categoryColumn = ((DataGridComboBoxColumn)grdMappings.TableStyles["SolidityShape"].GridColumnStyles["CategoryName"]);

         if (dr.IsNull(ProjectData.SolidityShape.TilesetNameColumn))
         {
            categoryColumn.Control.Items.Clear();
            return;
         }
         ProjectDataset.TilesetRow tileset = ProjectData.GetTileSet(dr.TilesetName);
         if (m_CategoryTileset != tileset)
         {
            categoryColumn.Control.Items.Clear();
            categoryColumn.Control.Items.AddRange(tileset.GetCategoryRows());
            m_CategoryTileset = tileset;
         }
      }

      private void Category_SelectionChangeCommitted(object sender, EventArgs e)
      {
         ProjectDataset.TilesetRow tileset = (ProjectDataset.TilesetRow)((DataGridComboBox)sender).SelectedItem;
         DataGridComboBoxColumn categoryColumn = ((DataGridComboBoxColumn)grdMappings.TableStyles["SolidityShape"].GridColumnStyles["CategoryName"]);
         
         categoryColumn.Control.Items.Clear();
         categoryColumn.Control.Items.AddRange(tileset.GetCategoryRows());
         m_CategoryTileset = tileset;
      }
      #endregion
   }
}
