using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{

	/// <summary>
	/// Summary description for SpriteCategories.
	/// </summary>
	public class frmSpriteCategory : System.Windows.Forms.Form
	{
      #region Embedded Classes
      class SpriteDefRowWrapper
      {
         public ProjectDataset.SpriteDefinitionRow row;
         public SpriteDefRowWrapper(ProjectDataset.SpriteDefinitionRow row)
         {
            this.row = row;
         }
         public override string ToString()
         {
            return row.Name;
         }
      }
      #endregion

      #region Private Members
      private ProjectDataset.SpriteCategoryRow m_Category;
      #endregion

      #region Form Designer Components
      private System.Windows.Forms.Label lblName;
      private System.Windows.Forms.TextBox txtName;
      #endregion
      private SGDK2.DataChangeNotifier dataMonitor;
      private System.Windows.Forms.CheckedListBox chlSpriteDefs;
      private System.Windows.Forms.CheckBox chkCheckedOnly;
      private System.ComponentModel.IContainer components;

      #region Initialization and Clean-up
      public frmSpriteCategory()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         String sName;
         Int32 nIdx = 1;
         do
            sName = "New Sprite Category " + (nIdx++).ToString();
         while (ProjectData.GetCounter(sName) != null);

         m_Category = ProjectData.AddSpriteCategory(sName);
         txtName.Text = sName;
         ProjectData.AcceptChanges();
      }

      public frmSpriteCategory(ProjectDataset.SpriteCategoryRow drCategory)
      {
         InitializeComponent();

         m_Category = drCategory;
         txtName.Text = drCategory.Name;
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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmSpriteCategory));
         this.lblName = new System.Windows.Forms.Label();
         this.txtName = new System.Windows.Forms.TextBox();
         this.dataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.chlSpriteDefs = new System.Windows.Forms.CheckedListBox();
         this.chkCheckedOnly = new System.Windows.Forms.CheckBox();
         this.SuspendLayout();
         // 
         // lblName
         // 
         this.lblName.Location = new System.Drawing.Point(8, 8);
         this.lblName.Name = "lblName";
         this.lblName.Size = new System.Drawing.Size(88, 20);
         this.lblName.TabIndex = 0;
         this.lblName.Text = "Name:";
         this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtName
         // 
         this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtName.Location = new System.Drawing.Point(96, 8);
         this.txtName.Name = "txtName";
         this.txtName.Size = new System.Drawing.Size(208, 20);
         this.txtName.TabIndex = 1;
         this.txtName.Text = "";
         this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
         this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
         // 
         // dataMonitor
         // 
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         this.dataMonitor.SpriteDefinitionRowDeleted += new SGDK2.ProjectDataset.SpriteDefinitionRowChangeEventHandler(this.dataMonitor_SpriteDefinitionRowDeleted);
         this.dataMonitor.SpriteCategoryRowDeleted += new SGDK2.ProjectDataset.SpriteCategoryRowChangeEventHandler(this.dataMonitor_SpriteCategoryRowDeleted);
         this.dataMonitor.SpriteDefinitionRowChanged += new SGDK2.ProjectDataset.SpriteDefinitionRowChangeEventHandler(this.dataMonitor_SpriteDefinitionRowChanged);
         // 
         // chlSpriteDefs
         // 
         this.chlSpriteDefs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.chlSpriteDefs.CheckOnClick = true;
         this.chlSpriteDefs.Location = new System.Drawing.Point(96, 56);
         this.chlSpriteDefs.Name = "chlSpriteDefs";
         this.chlSpriteDefs.Size = new System.Drawing.Size(208, 154);
         this.chlSpriteDefs.TabIndex = 2;
         this.chlSpriteDefs.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chlSpriteDefs_ItemCheck);
         // 
         // chkCheckedOnly
         // 
         this.chkCheckedOnly.Location = new System.Drawing.Point(96, 32);
         this.chkCheckedOnly.Name = "chkCheckedOnly";
         this.chkCheckedOnly.Size = new System.Drawing.Size(208, 16);
         this.chkCheckedOnly.TabIndex = 3;
         this.chkCheckedOnly.Text = "Only Show Checked Items";
         this.chkCheckedOnly.CheckedChanged += new System.EventHandler(this.chkCheckedOnly_CheckedChanged);
         // 
         // frmSpriteCategory
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(312, 221);
         this.Controls.Add(this.chkCheckedOnly);
         this.Controls.Add(this.chlSpriteDefs);
         this.Controls.Add(this.txtName);
         this.Controls.Add(this.lblName);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "frmSpriteCategory";
         this.Text = "Sprite Category";
         this.ResumeLayout(false);

      }
		#endregion

      #region Private Members
      private void PopulateSpriteDefs()
      {
         chlSpriteDefs.Items.Clear();
         foreach(ProjectDataset.SpriteDefinitionRow drSpriteDef in ProjectData.SpriteDefinition)
         {
            if (ProjectData.IsSpriteDefinitionInCategory(m_Category.Name, drSpriteDef.Name))
            {
               chlSpriteDefs.Items.Add(new SpriteDefRowWrapper(drSpriteDef), true);
            }
            else if (!chkCheckedOnly.Checked)
            {
               chlSpriteDefs.Items.Add(new SpriteDefRowWrapper(drSpriteDef), false);
            }
         }
      }
      #endregion

      #region Overrides
      protected override void OnLoad(EventArgs e)
      {
         PopulateSpriteDefs();
         base.OnLoad (e);
      }
      #endregion

      #region Event Handlers
      private void txtName_Validated(object sender, System.EventArgs e)
      {
         m_Category.Name = txtName.Text;
         ProjectData.AcceptChanges();
      }

      private void txtName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         string sValid = ProjectData.ValidateName(txtName.Text);

         if (sValid != null)
         {
            if (DialogResult.Cancel == MessageBox.Show(this, sValid, "Sprite Category Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtName.Text = m_Category.Name;
            e.Cancel = true;
         }
         ProjectDataset.SpriteCategoryRow dr = ProjectData.GetSpriteCategory(txtName.Text);
         if ((null != dr) && (m_Category != dr))
         {
            if (DialogResult.Cancel == MessageBox.Show(this, txtName.Text + " already exists", "Sprite Category Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtName.Text = m_Category.Name;
            e.Cancel = true;
         }               
      }

      private void dataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void dataMonitor_SpriteCategoryRowDeleted(object sender, SGDK2.ProjectDataset.SpriteCategoryRowChangeEvent e)
      {
         if (e.Row == m_Category)
            this.Close();
      }

      private void chkCheckedOnly_CheckedChanged(object sender, System.EventArgs e)
      {
         PopulateSpriteDefs();
      }

      private void chlSpriteDefs_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
      {
         if (e.NewValue == CheckState.Checked)
            ProjectData.AddSpriteDefinitionToCategory(m_Category, ((SpriteDefRowWrapper)chlSpriteDefs.Items[e.Index]).row);
         else
            ProjectData.RemoveSpriteDefinitionFromCategory(m_Category.Name, chlSpriteDefs.Items[e.Index].ToString());
         ProjectData.AcceptChanges();
      }

      private void dataMonitor_SpriteDefinitionRowChanged(object sender, SGDK2.ProjectDataset.SpriteDefinitionRowChangeEvent e)
      {
         if (e.Action == System.Data.DataRowAction.Add)
         {
            chlSpriteDefs.Items.Add(new SpriteDefRowWrapper((ProjectDataset.SpriteDefinitionRow)e.Row), false);
         }
         else
         {
            for(int nIdx = 0; nIdx < chlSpriteDefs.Items.Count; nIdx++)
            {
               ProjectDataset.SpriteDefinitionRow dr = ((SpriteDefRowWrapper)chlSpriteDefs.Items[nIdx]).row;
               if (dr == e.Row)
               {
                  chlSpriteDefs.Items[nIdx] = chlSpriteDefs.Items[nIdx];
                  return;
               }
            }
         }
      }

      private void dataMonitor_SpriteDefinitionRowDeleted(object sender, SGDK2.ProjectDataset.SpriteDefinitionRowChangeEvent e)
      {
         for(int nIdx = 0; nIdx < chlSpriteDefs.Items.Count; nIdx++)
         {
            ProjectDataset.SpriteDefinitionRow dr = ((SpriteDefRowWrapper)chlSpriteDefs.Items[nIdx]).row;
            if (dr == e.Row)
               chlSpriteDefs.Items.RemoveAt(nIdx);
         }
      }
      #endregion	
   }
}
