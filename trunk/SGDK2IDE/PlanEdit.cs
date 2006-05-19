using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for Plan.
	/// </summary>
	public class frmPlanEdit : System.Windows.Forms.Form
	{
      #region Non-control members
      private ProjectDataset.SpritePlanRow m_Plan;
      #endregion

      #region Form Designer Members
      private System.Windows.Forms.Label lblName;
      private System.Windows.Forms.TextBox txtName;
      private System.Windows.Forms.GroupBox grpRules;
      private System.Windows.Forms.Panel pnlName;
      private System.Windows.Forms.TreeView tvwRules;
      private System.Windows.Forms.Splitter splitter1;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.ToolBar tbRules;
      private System.Windows.Forms.ImageList imlPlan;
      private System.Windows.Forms.ToolBarButton tbbNewRule;
      private System.Windows.Forms.ToolBarButton tbbDeleteRule;
      private System.Windows.Forms.CheckBox chkEndIf;
      private System.Windows.Forms.Label lblParam3;
      private System.Windows.Forms.ComboBox cboParam3;
      private System.Windows.Forms.ComboBox cboRuleType;
      private System.Windows.Forms.Label lblParam2;
      private System.Windows.Forms.Label lblParam1;
      private System.Windows.Forms.ComboBox cboParam2;
      private System.Windows.Forms.ComboBox cboParam1;
      private System.Windows.Forms.CheckBox chkNot;
      private System.Windows.Forms.ComboBox cboFunction;
      private System.Windows.Forms.Label lblRuleName;
      private System.Windows.Forms.TextBox txtRuleName;
      private System.Windows.Forms.Label lblOutput;
      private System.Windows.Forms.ComboBox cboOutput;
      private SGDK2.DataChangeNotifier dataMonitor;
      private System.ComponentModel.IContainer components;
      #endregion

      #region Initialization and Clean-up
		public frmPlanEdit(ProjectDataset.LayerRow parent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         String sName;
         Int32 nIdx = 1;
         do
            sName = parent.MapRow.Name + " Plan " + (nIdx++).ToString();
         while (ProjectData.GetSpritePlan(parent, sName) != null);

         m_Plan = ProjectData.AddSpritePlan(parent, sName, 1);
         txtName.Text = sName;
		}

      public frmPlanEdit(ProjectDataset.SpritePlanRow plan)
      {
         InitializeComponent();

         m_Plan = plan;
         txtName.Text = plan.Name;
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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmPlanEdit));
         this.lblName = new System.Windows.Forms.Label();
         this.txtName = new System.Windows.Forms.TextBox();
         this.grpRules = new System.Windows.Forms.GroupBox();
         this.tvwRules = new System.Windows.Forms.TreeView();
         this.tbRules = new System.Windows.Forms.ToolBar();
         this.tbbNewRule = new System.Windows.Forms.ToolBarButton();
         this.tbbDeleteRule = new System.Windows.Forms.ToolBarButton();
         this.imlPlan = new System.Windows.Forms.ImageList(this.components);
         this.splitter1 = new System.Windows.Forms.Splitter();
         this.panel1 = new System.Windows.Forms.Panel();
         this.lblOutput = new System.Windows.Forms.Label();
         this.cboOutput = new System.Windows.Forms.ComboBox();
         this.txtRuleName = new System.Windows.Forms.TextBox();
         this.lblRuleName = new System.Windows.Forms.Label();
         this.chkEndIf = new System.Windows.Forms.CheckBox();
         this.lblParam3 = new System.Windows.Forms.Label();
         this.cboParam3 = new System.Windows.Forms.ComboBox();
         this.cboRuleType = new System.Windows.Forms.ComboBox();
         this.lblParam2 = new System.Windows.Forms.Label();
         this.lblParam1 = new System.Windows.Forms.Label();
         this.cboParam2 = new System.Windows.Forms.ComboBox();
         this.cboParam1 = new System.Windows.Forms.ComboBox();
         this.chkNot = new System.Windows.Forms.CheckBox();
         this.cboFunction = new System.Windows.Forms.ComboBox();
         this.pnlName = new System.Windows.Forms.Panel();
         this.dataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.grpRules.SuspendLayout();
         this.panel1.SuspendLayout();
         this.pnlName.SuspendLayout();
         this.SuspendLayout();
         // 
         // lblName
         // 
         this.lblName.Dock = System.Windows.Forms.DockStyle.Left;
         this.lblName.Location = new System.Drawing.Point(2, 2);
         this.lblName.Name = "lblName";
         this.lblName.Size = new System.Drawing.Size(72, 20);
         this.lblName.TabIndex = 0;
         this.lblName.Text = "Name:";
         this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtName
         // 
         this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtName.Location = new System.Drawing.Point(74, 2);
         this.txtName.Name = "txtName";
         this.txtName.Size = new System.Drawing.Size(412, 20);
         this.txtName.TabIndex = 1;
         this.txtName.Text = "";
         this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
         this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
         // 
         // grpRules
         // 
         this.grpRules.Controls.Add(this.tvwRules);
         this.grpRules.Controls.Add(this.tbRules);
         this.grpRules.Controls.Add(this.splitter1);
         this.grpRules.Controls.Add(this.panel1);
         this.grpRules.Dock = System.Windows.Forms.DockStyle.Fill;
         this.grpRules.Location = new System.Drawing.Point(0, 24);
         this.grpRules.Name = "grpRules";
         this.grpRules.Size = new System.Drawing.Size(488, 357);
         this.grpRules.TabIndex = 2;
         this.grpRules.TabStop = false;
         this.grpRules.Text = "Rules";
         // 
         // tvwRules
         // 
         this.tvwRules.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tvwRules.ImageIndex = -1;
         this.tvwRules.Location = new System.Drawing.Point(3, 41);
         this.tvwRules.Name = "tvwRules";
         this.tvwRules.SelectedImageIndex = -1;
         this.tvwRules.Size = new System.Drawing.Size(181, 313);
         this.tvwRules.TabIndex = 0;
         // 
         // tbRules
         // 
         this.tbRules.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                                   this.tbbNewRule,
                                                                                   this.tbbDeleteRule});
         this.tbRules.Divider = false;
         this.tbRules.DropDownArrows = true;
         this.tbRules.ImageList = this.imlPlan;
         this.tbRules.Location = new System.Drawing.Point(3, 16);
         this.tbRules.Name = "tbRules";
         this.tbRules.ShowToolTips = true;
         this.tbRules.Size = new System.Drawing.Size(181, 25);
         this.tbRules.TabIndex = 3;
         // 
         // tbbNewRule
         // 
         this.tbbNewRule.ImageIndex = 0;
         this.tbbNewRule.ToolTipText = "Add new rule";
         // 
         // tbbDeleteRule
         // 
         this.tbbDeleteRule.ImageIndex = 1;
         this.tbbDeleteRule.ToolTipText = "Delete selected rule";
         // 
         // imlPlan
         // 
         this.imlPlan.ImageSize = new System.Drawing.Size(15, 15);
         this.imlPlan.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlPlan.ImageStream")));
         this.imlPlan.TransparentColor = System.Drawing.Color.Magenta;
         // 
         // splitter1
         // 
         this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
         this.splitter1.Location = new System.Drawing.Point(184, 16);
         this.splitter1.Name = "splitter1";
         this.splitter1.Size = new System.Drawing.Size(5, 338);
         this.splitter1.TabIndex = 1;
         this.splitter1.TabStop = false;
         // 
         // panel1
         // 
         this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.panel1.Controls.Add(this.lblOutput);
         this.panel1.Controls.Add(this.cboOutput);
         this.panel1.Controls.Add(this.txtRuleName);
         this.panel1.Controls.Add(this.lblRuleName);
         this.panel1.Controls.Add(this.chkEndIf);
         this.panel1.Controls.Add(this.lblParam3);
         this.panel1.Controls.Add(this.cboParam3);
         this.panel1.Controls.Add(this.cboRuleType);
         this.panel1.Controls.Add(this.lblParam2);
         this.panel1.Controls.Add(this.lblParam1);
         this.panel1.Controls.Add(this.cboParam2);
         this.panel1.Controls.Add(this.cboParam1);
         this.panel1.Controls.Add(this.chkNot);
         this.panel1.Controls.Add(this.cboFunction);
         this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
         this.panel1.Location = new System.Drawing.Point(189, 16);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(296, 338);
         this.panel1.TabIndex = 2;
         // 
         // lblOutput
         // 
         this.lblOutput.Enabled = false;
         this.lblOutput.Location = new System.Drawing.Point(8, 144);
         this.lblOutput.Name = "lblOutput";
         this.lblOutput.Size = new System.Drawing.Size(128, 21);
         this.lblOutput.TabIndex = 34;
         this.lblOutput.Text = "Output to:";
         this.lblOutput.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // cboOutput
         // 
         this.cboOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cboOutput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboOutput.Enabled = false;
         this.cboOutput.Location = new System.Drawing.Point(136, 144);
         this.cboOutput.Name = "cboOutput";
         this.cboOutput.Size = new System.Drawing.Size(144, 21);
         this.cboOutput.TabIndex = 35;
         // 
         // txtRuleName
         // 
         this.txtRuleName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtRuleName.Location = new System.Drawing.Point(96, 8);
         this.txtRuleName.Name = "txtRuleName";
         this.txtRuleName.Size = new System.Drawing.Size(184, 20);
         this.txtRuleName.TabIndex = 33;
         this.txtRuleName.Text = "";
         // 
         // lblRuleName
         // 
         this.lblRuleName.Location = new System.Drawing.Point(8, 8);
         this.lblRuleName.Name = "lblRuleName";
         this.lblRuleName.Size = new System.Drawing.Size(88, 20);
         this.lblRuleName.TabIndex = 32;
         this.lblRuleName.Text = "Rule Name:";
         this.lblRuleName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // chkEndIf
         // 
         this.chkEndIf.Enabled = false;
         this.chkEndIf.Location = new System.Drawing.Point(8, 168);
         this.chkEndIf.Name = "chkEndIf";
         this.chkEndIf.Size = new System.Drawing.Size(120, 24);
         this.chkEndIf.TabIndex = 31;
         this.chkEndIf.Text = "End If";
         // 
         // lblParam3
         // 
         this.lblParam3.Enabled = false;
         this.lblParam3.Location = new System.Drawing.Point(8, 120);
         this.lblParam3.Name = "lblParam3";
         this.lblParam3.Size = new System.Drawing.Size(128, 21);
         this.lblParam3.TabIndex = 29;
         this.lblParam3.Text = "Parameter 3:";
         this.lblParam3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // cboParam3
         // 
         this.cboParam3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cboParam3.Enabled = false;
         this.cboParam3.Location = new System.Drawing.Point(136, 120);
         this.cboParam3.Name = "cboParam3";
         this.cboParam3.Size = new System.Drawing.Size(144, 21);
         this.cboParam3.TabIndex = 30;
         // 
         // cboRuleType
         // 
         this.cboRuleType.Items.AddRange(new object[] {
                                                         "Do",
                                                         "If",
                                                         "And",
                                                         "Or"});
         this.cboRuleType.Location = new System.Drawing.Point(12, 48);
         this.cboRuleType.Name = "cboRuleType";
         this.cboRuleType.Size = new System.Drawing.Size(56, 21);
         this.cboRuleType.TabIndex = 22;
         // 
         // lblParam2
         // 
         this.lblParam2.Enabled = false;
         this.lblParam2.Location = new System.Drawing.Point(8, 96);
         this.lblParam2.Name = "lblParam2";
         this.lblParam2.Size = new System.Drawing.Size(128, 21);
         this.lblParam2.TabIndex = 27;
         this.lblParam2.Text = "Parameter 2:";
         this.lblParam2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblParam1
         // 
         this.lblParam1.Enabled = false;
         this.lblParam1.Location = new System.Drawing.Point(8, 72);
         this.lblParam1.Name = "lblParam1";
         this.lblParam1.Size = new System.Drawing.Size(128, 21);
         this.lblParam1.TabIndex = 25;
         this.lblParam1.Text = "Parameter 1:";
         this.lblParam1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // cboParam2
         // 
         this.cboParam2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cboParam2.Enabled = false;
         this.cboParam2.Location = new System.Drawing.Point(136, 96);
         this.cboParam2.Name = "cboParam2";
         this.cboParam2.Size = new System.Drawing.Size(144, 21);
         this.cboParam2.TabIndex = 28;
         // 
         // cboParam1
         // 
         this.cboParam1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cboParam1.Enabled = false;
         this.cboParam1.Location = new System.Drawing.Point(136, 72);
         this.cboParam1.Name = "cboParam1";
         this.cboParam1.Size = new System.Drawing.Size(144, 21);
         this.cboParam1.TabIndex = 26;
         // 
         // chkNot
         // 
         this.chkNot.Enabled = false;
         this.chkNot.Location = new System.Drawing.Point(76, 48);
         this.chkNot.Name = "chkNot";
         this.chkNot.Size = new System.Drawing.Size(56, 21);
         this.chkNot.TabIndex = 23;
         this.chkNot.Text = "Not";
         // 
         // cboFunction
         // 
         this.cboFunction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cboFunction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboFunction.Items.AddRange(new object[] {
                                                         "InputPressed",
                                                         "InputReleased",
                                                         "InputIsDown",
                                                         "CounterAtLeast",
                                                         "CounterEqual",
                                                         "ParameterAtLeast",
                                                         "ParameterEqual",
                                                         "IsCurrentState",
                                                         "HitSpriteInCategory",
                                                         "LandedOnSpriteInCategory",
                                                         "HitSolidTile",
                                                         "SolidBelow",
                                                         "SolidAbove",
                                                         "SolidLeft",
                                                         "SolidRight",
                                                         "HitTileInCategory",
                                                         "TouchingTileInCategory",
                                                         "HitAreaInCategory",
                                                         "ExitedAreaInCategory",
                                                         "TouchingAreaInCategory",
                                                         "Riding"});
         this.cboFunction.Location = new System.Drawing.Point(136, 48);
         this.cboFunction.Name = "cboFunction";
         this.cboFunction.Size = new System.Drawing.Size(144, 21);
         this.cboFunction.TabIndex = 24;
         // 
         // pnlName
         // 
         this.pnlName.Controls.Add(this.txtName);
         this.pnlName.Controls.Add(this.lblName);
         this.pnlName.Dock = System.Windows.Forms.DockStyle.Top;
         this.pnlName.DockPadding.All = 2;
         this.pnlName.Location = new System.Drawing.Point(0, 0);
         this.pnlName.Name = "pnlName";
         this.pnlName.Size = new System.Drawing.Size(488, 24);
         this.pnlName.TabIndex = 3;
         // 
         // dataMonitor
         // 
         this.dataMonitor.SpritePlanRowDeleted += new SGDK2.ProjectDataset.SpritePlanRowChangeEventHandler(this.dataMonitor_SpritePlanRowDeleted);
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         // 
         // frmPlanEdit
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(488, 381);
         this.Controls.Add(this.grpRules);
         this.Controls.Add(this.pnlName);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "frmPlanEdit";
         this.Text = "Edit Plan";
         this.grpRules.ResumeLayout(false);
         this.panel1.ResumeLayout(false);
         this.pnlName.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion

      private void txtName_Validated(object sender, System.EventArgs e)
      {
         m_Plan.Name = txtName.Text;
      }

      private void txtName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         string sValid = ProjectData.ValidateName(txtName.Text);

         if (sValid != null)
         {
            if (DialogResult.Cancel == MessageBox.Show(this, sValid, "Plan Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtName.Text = m_Plan.Name;
            e.Cancel = true;
         }
         ProjectDataset.SpritePlanRow dr = ProjectData.GetSpritePlan(m_Plan.LayerRowParent, txtName.Text);
         if ((null != dr) && (m_Plan != dr))
         {
            if (DialogResult.Cancel == MessageBox.Show(this, txtName.Text + " already exists", "Plan Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtName.Text = m_Plan.Name;
            e.Cancel = true;
         }      
      }

      private void dataMonitor_SpritePlanRowDeleted(object sender, SGDK2.ProjectDataset.SpritePlanRowChangeEvent e)
      {
         if (e.Row == m_Plan)
            this.Close();
      }

      private void dataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }
	}
}
