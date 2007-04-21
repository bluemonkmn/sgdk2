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

         SGDK2IDE.LoadFormSettings(this);

         String sName;
         Int32 nIdx = 1;
         do
            sName = "New Solidity " + (nIdx++).ToString();
         while (ProjectData.GetSolidity(sName) != null);

         m_Solidity = ProjectData.AddSolidity(sName);
         txtName.Text = sName;

         InitializeGrid();

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"Solidity.html");
         SGDK2IDE.g_HelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
      }

      public frmSolidity(ProjectDataset.SolidityRow solidity)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         m_Solidity = solidity;
         txtName.Text = solidity.Name;
         InitializeGrid();

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"Solidity.html");
         SGDK2IDE.g_HelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
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
         this.txtName.Size = new System.Drawing.Size(304, 20);
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
         this.grdMappings.Size = new System.Drawing.Size(360, 216);
         this.grdMappings.TabIndex = 7;
         // 
         // dataMonitor
         // 
         this.dataMonitor.SolidityRowDeleted += new SGDK2.ProjectDataset.SolidityRowChangeEventHandler(this.dataMonitor_SolidityRowDeleted);
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         // 
         // frmSolidity
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(376, 261);
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
         grdMappings.BindingContext[ProjectData.Solidity].Position = ProjectData.Solidity.DefaultView.Find(m_Solidity.Name);

         DataGridTableStyle ts = new DataGridTableStyle();
         ts.MappingName = "SolidityShape";

         PropertyDescriptorCollection pdc = BindingContext[ProjectData.Solidity, "SoliditySolidityShape"].GetItemProperties();

         ProjectDataset.TileCategoryRow[] cats = new SGDK2.ProjectDataset.TileCategoryRow[ProjectData.TileCategory.DefaultView.Count];
         for(int i = 0; i < ProjectData.TileCategory.DefaultView.Count; i++)
            cats[i] = (ProjectDataset.TileCategoryRow)ProjectData.TileCategory.DefaultView[i].Row;
         DataGridComboBoxColumn col = new DataGridComboBoxColumn(pdc["CategoryName"], cats, ProjectData.TileCategory.NameColumn.ColumnName);
         col.MappingName = "CategoryName";
         col.HeaderText = "Category";
         col.Width = 150;
         ts.GridColumnStyles.Add(col);

         col = new DataGridComboBoxColumn(pdc["ShapeName"], GetTileShapes());
         col.MappingName = "ShapeName";
         col.HeaderText = "Shape";
         ts.GridColumnStyles.Add(col);
         col.Width = 150;

         grdMappings.TableStyles.Add(ts);
      }

      private string[] GetTileShapes()
      {
         CodeGenerator gen = new CodeGenerator();
         gen.GenerateLevel = CodeGenerator.CodeLevel.ExcludeRules;
         string errs = gen.CompileTempAssembly(false);
         if ((errs != null) && (errs.Length > 0))
         {
            MessageBox.Show(this, "Errors occurred while compiling a temporary project to generate a list of available tile shapes: " + errs, "Error Compiling Temporary Code", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return new string[] {};
         }
         RemotingServices.IRemoteTypeInfo reflector = CodeGenerator.CreateInstanceAndUnwrap(
            "RemoteReflector", "TileShape") as RemotingServices.IRemoteTypeInfo;

         RemotingServices.RemoteTypeName[] subclasses = reflector.GetSubclasses();
         string[] shapes = new string[subclasses.Length];
         for(int i = 0; i < subclasses.Length; i++)
            shapes[i] = subclasses[i].Name;
         return shapes;
      }
      #endregion

      #region Public Static Members
      public static void Edit(Form MdiParent, ProjectDataset.SolidityRow EditRow)
      {
         foreach(Form frm in MdiParent.MdiChildren)
         {
            frmSolidity f = frm as frmSolidity;
            if (f != null)
            {
               if (f.m_Solidity == EditRow)
               {
                  f.Activate();
                  return;
               }
            }
         }

         frmSolidity frmNew = new frmSolidity(EditRow);
         frmNew.MdiParent = MdiParent;
         frmNew.Show();
      }
      #endregion

      #region Overrides
      protected override void OnClosing(CancelEventArgs e)
      {
         base.OnClosing (e);
         SGDK2IDE.SaveFormSettings(this);
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
         // See Microsoft KB article 841096: 
         // http://support.microsoft.com/default.aspx?scid=kb;en-us;841096
         // Without hotfox/SP, textbox will not validate its content if another
         // MDI child is activated and that this form is activated again, while
         // the textbox still has focus.
         if ((m_Solidity.RowState != DataRowState.Deleted) && (m_Solidity.RowState != DataRowState.Detached) &&
            (m_Solidity.Name != txtName.Text))
         {
            m_Solidity.Name = txtName.Text;
            // Must re-create the binding context as there seems to be no other way to
            // re-sync the child rows with the new parent name as far as the grid is concerned.
            grdMappings.BindingContext = new BindingContext();
            grdMappings.BindingContext[ProjectData.Solidity].Position = ProjectData.Solidity.DefaultView.Find(m_Solidity.Name);
         }
      }
      #endregion
   }
}
