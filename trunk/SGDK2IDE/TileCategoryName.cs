/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for TileCategoryName.
	/// </summary>
	public class frmTileCategoryName : System.Windows.Forms.Form
	{
      #region Non-Control members
      private ProjectDataset.TileCategoryRow m_Category = null;
      #endregion

      #region Form Designer Members
      private System.Windows.Forms.Label lblName;
      private System.Windows.Forms.TextBox txtName;
      private System.Windows.Forms.GroupBox grpEdit;
      private System.Windows.Forms.Label lblTileset;
      private System.Windows.Forms.ComboBox cboTileset;
      private System.Windows.Forms.Button btnEdit;
      private SGDK2.DataChangeNotifier DataMonitor;
      private System.ComponentModel.IContainer components;
      #endregion

      #region Initialization and Clean-up
      public frmTileCategoryName()
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         int iNew;
         for (iNew = 1; ProjectData.GetTileCategory("New Category " + iNew.ToString()) != null; iNew++)
            ;
         m_Category = ProjectData.AddTileCategoryRow("New Category " + iNew.ToString());
         txtName.Text = m_Category.Name;
      }

		public frmTileCategoryName(ProjectDataset.TileCategoryRow row)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         txtName.Text = row.Name;
         m_Category = row;
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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmTileCategoryName));
         this.lblName = new System.Windows.Forms.Label();
         this.txtName = new System.Windows.Forms.TextBox();
         this.grpEdit = new System.Windows.Forms.GroupBox();
         this.btnEdit = new System.Windows.Forms.Button();
         this.cboTileset = new System.Windows.Forms.ComboBox();
         this.lblTileset = new System.Windows.Forms.Label();
         this.DataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.grpEdit.SuspendLayout();
         this.SuspendLayout();
         // 
         // lblName
         // 
         this.lblName.Location = new System.Drawing.Point(16, 8);
         this.lblName.Name = "lblName";
         this.lblName.Size = new System.Drawing.Size(56, 20);
         this.lblName.TabIndex = 0;
         this.lblName.Text = "Name:";
         this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtName
         // 
         this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtName.Location = new System.Drawing.Point(72, 8);
         this.txtName.Name = "txtName";
         this.txtName.Size = new System.Drawing.Size(212, 20);
         this.txtName.TabIndex = 1;
         this.txtName.Text = "";
         this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
         this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
         // 
         // grpEdit
         // 
         this.grpEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.grpEdit.Controls.Add(this.btnEdit);
         this.grpEdit.Controls.Add(this.cboTileset);
         this.grpEdit.Controls.Add(this.lblTileset);
         this.grpEdit.Location = new System.Drawing.Point(8, 48);
         this.grpEdit.Name = "grpEdit";
         this.grpEdit.Size = new System.Drawing.Size(284, 88);
         this.grpEdit.TabIndex = 2;
         this.grpEdit.TabStop = false;
         this.grpEdit.Text = "Edit Category Tiles";
         // 
         // btnEdit
         // 
         this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnEdit.Location = new System.Drawing.Point(204, 56);
         this.btnEdit.Name = "btnEdit";
         this.btnEdit.Size = new System.Drawing.Size(72, 24);
         this.btnEdit.TabIndex = 2;
         this.btnEdit.Text = "Edit...";
         this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
         // 
         // cboTileset
         // 
         this.cboTileset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cboTileset.DisplayMember = "Name";
         this.cboTileset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboTileset.Location = new System.Drawing.Point(64, 24);
         this.cboTileset.Name = "cboTileset";
         this.cboTileset.Size = new System.Drawing.Size(212, 21);
         this.cboTileset.TabIndex = 1;
         // 
         // lblTileset
         // 
         this.lblTileset.Location = new System.Drawing.Point(8, 24);
         this.lblTileset.Name = "lblTileset";
         this.lblTileset.Size = new System.Drawing.Size(56, 21);
         this.lblTileset.TabIndex = 0;
         this.lblTileset.Text = "Tileset:";
         this.lblTileset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // DataMonitor
         // 
         this.DataMonitor.TilesetRowDeleted += new SGDK2.ProjectDataset.TilesetRowChangeEventHandler(this.DataMonitor_TilesetRowDeleted);
         this.DataMonitor.TilesetRowChanged += new SGDK2.ProjectDataset.TilesetRowChangeEventHandler(this.DataMonitor_TilesetRowChanged);
         this.DataMonitor.TileCategoryRowDeleted += new SGDK2.ProjectDataset.TileCategoryRowChangeEventHandler(this.DataMonitor_TileCategoryRowDeleted);
         this.DataMonitor.Clearing += new System.EventHandler(this.DataMonitor_Clearing);
         // 
         // frmTileCategoryName
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(296, 149);
         this.Controls.Add(this.grpEdit);
         this.Controls.Add(this.txtName);
         this.Controls.Add(this.lblName);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "frmTileCategoryName";
         this.Text = "Tile Category Name";
         this.grpEdit.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion

      #region Public Static Members
      public static void Edit(Form MdiParent, ProjectDataset.TileCategoryRow EditRow)
      {
         foreach(Form frm in MdiParent.MdiChildren)
         {
            frmTileCategoryName f = frm as frmTileCategoryName;
            if (f != null)
            {
               if (f.m_Category == EditRow)
               {
                  f.Activate();
                  return;
               }
            }
         }

         frmTileCategoryName frmNew = new frmTileCategoryName(EditRow);
         frmNew.MdiParent = MdiParent;
         frmNew.Show();
      }
      #endregion

      #region Overrides
      protected override void OnLoad(EventArgs e)
      {
         foreach(System.Data.DataRowView drv in ProjectData.Tileset.DefaultView)
            cboTileset.Items.Add(drv.Row);
         base.OnLoad (e);
         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"TileCategories.html");
         SGDK2IDE.g_HelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
      }

      protected override void OnClosing(CancelEventArgs e)
      {
         base.OnClosing (e);
         SGDK2IDE.SaveFormSettings(this);
      }
      #endregion

      #region Private Properties
      private ProjectDataset.TilesetRow SelectedTileset
      {
         get
         {
            return (ProjectDataset.TilesetRow)cboTileset.SelectedItem;
         }
      }
      #endregion

      #region Event Handlers
      private void btnEdit_Click(object sender, System.EventArgs e)
      {
         if (cboTileset.SelectedIndex < 0)
         {
            MessageBox.Show(this, "Select a tileset first.", "Edit Category", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         frmTileCategory frm;
         if (ProjectData.GetCategorizedTileset(SelectedTileset.Name, m_Category.Name) == null)
            frm = new frmTileCategory(ProjectData.AddCategorizedTilesetRow(SelectedTileset, m_Category));
         else
            frm = new frmTileCategory(ProjectData.GetCategorizedTileset(SelectedTileset.Name, m_Category.Name));
         frm.MdiParent = MdiParent;
         frm.Show();
      }

      private void DataMonitor_TilesetRowChanged(object sender, SGDK2.ProjectDataset.TilesetRowChangeEvent e)
      {
         if (e.Action == System.Data.DataRowAction.Change)
         {
            for (int i = 0; i < cboTileset.Items.Count; i++)
            {
               cboTileset.Items[i] = cboTileset.Items[i];
            }
         }
         else if (e.Action == System.Data.DataRowAction.Add)
         {
            cboTileset.Items.Add(e.Row);
         }
      }

      private void DataMonitor_TilesetRowDeleted(object sender, SGDK2.ProjectDataset.TilesetRowChangeEvent e)
      {
         if (cboTileset.Items.Contains(e.Row))
            cboTileset.Items.Remove(e.Row);
      }

      private void DataMonitor_TileCategoryRowDeleted(object sender, SGDK2.ProjectDataset.TileCategoryRowChangeEvent e)
      {
         if (e.Row == m_Category)
            this.Close();
      }

      private void DataMonitor_Clearing(object sender, EventArgs e)
      {
         this.Close();
      }

      private void txtName_Validated(object sender, System.EventArgs e)
      {
         if (m_Category.Name != txtName.Text)
         {
            m_Category.Name = txtName.Text;
         }
      }

      private void txtName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         string sValid = ProjectData.ValidateName(txtName.Text);

         if (sValid != null)
         {
            if (DialogResult.Cancel == MessageBox.Show(this, sValid, "Category Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtName.Text = m_Category.Name;
            e.Cancel = true;
         }
         ProjectDataset.TileCategoryRow cr = ProjectData.GetTileCategory(m_Category.Name);
         if ((null != cr) && (m_Category != cr))
         {
            if (DialogResult.Cancel == MessageBox.Show(this, txtName.Text + " already exists", "Category Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtName.Text = m_Category.Name;
            e.Cancel = true;
         }         
      }
      #endregion
   }
}
