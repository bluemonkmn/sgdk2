using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace SGDK2
{
   /// <summary>
   /// Summary description for Plan.
   /// </summary>
   public class frmPlanEdit : System.Windows.Forms.Form
   {
      #region Non-control members
      private ProjectDataset.SpritePlanRow m_Plan;
      private RuleTable m_AvailableRules = null;
      private RemotingServices.RemotePropertyInfo[] m_PlanProperties = null;
      private EnumTable m_Enums = null;
      string m_OldRuleName = null;
      int m_OldSequence = -1;
      string m_OldType = null;
      bool m_OldEndIf = false;
      private Hashtable m_TreeNodes = new Hashtable();
      private bool m_Loading = false;
      SpriteCodeRef m_SpriteContext = null;
      #endregion

      #region Embedded Classes
      private class SpriteCodeRef
      {
         private ProjectDataset.SpriteRow m_drSprite;

         public SpriteCodeRef(ProjectDataset.SpriteRow drSprite)
         {
            m_drSprite = drSprite;
         }

         public ProjectDataset.SpriteRow SpriteRow
         {
            get
            {
               return m_drSprite;
            }
         }

         public override string ToString()
         {
            return CodeGenerator.SpritePlanParentField + ".m_" + CodeGenerator.NameToVariable(m_drSprite.Name);
         }
      }
      #endregion

      #region Form Designer Members
      private System.Windows.Forms.Label lblName;
      private System.Windows.Forms.TextBox txtName;
      private System.Windows.Forms.GroupBox grpRules;
      private System.Windows.Forms.Panel pnlName;
      private System.Windows.Forms.TreeView tvwRules;
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
      private System.Windows.Forms.TextBox txtErrors;
      private System.Windows.Forms.Splitter splitterRules;
      private System.Windows.Forms.ToolBarButton tbbSep;
      private System.Windows.Forms.ToolBarButton tbbMoveUp;
      private System.Windows.Forms.ToolBarButton tbbMoveDown;
      private System.Windows.Forms.Panel pnlRule;
      private System.Windows.Forms.TextBox txtHelpText;
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
         
         PopulateRules();
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
         this.tbbSep = new System.Windows.Forms.ToolBarButton();
         this.tbbMoveUp = new System.Windows.Forms.ToolBarButton();
         this.tbbMoveDown = new System.Windows.Forms.ToolBarButton();
         this.imlPlan = new System.Windows.Forms.ImageList(this.components);
         this.splitterRules = new System.Windows.Forms.Splitter();
         this.pnlRule = new System.Windows.Forms.Panel();
         this.txtHelpText = new System.Windows.Forms.TextBox();
         this.txtErrors = new System.Windows.Forms.TextBox();
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
         this.pnlRule.SuspendLayout();
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
         this.txtName.Size = new System.Drawing.Size(436, 20);
         this.txtName.TabIndex = 1;
         this.txtName.Text = "";
         this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
         this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
         // 
         // grpRules
         // 
         this.grpRules.Controls.Add(this.tvwRules);
         this.grpRules.Controls.Add(this.tbRules);
         this.grpRules.Controls.Add(this.splitterRules);
         this.grpRules.Controls.Add(this.pnlRule);
         this.grpRules.Dock = System.Windows.Forms.DockStyle.Fill;
         this.grpRules.Location = new System.Drawing.Point(0, 24);
         this.grpRules.Name = "grpRules";
         this.grpRules.Size = new System.Drawing.Size(512, 349);
         this.grpRules.TabIndex = 2;
         this.grpRules.TabStop = false;
         this.grpRules.Text = "Rules";
         // 
         // tvwRules
         // 
         this.tvwRules.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tvwRules.HideSelection = false;
         this.tvwRules.ImageIndex = -1;
         this.tvwRules.Location = new System.Drawing.Point(3, 41);
         this.tvwRules.Name = "tvwRules";
         this.tvwRules.SelectedImageIndex = -1;
         this.tvwRules.Size = new System.Drawing.Size(157, 305);
         this.tvwRules.TabIndex = 0;
         this.tvwRules.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwRules_AfterSelect);
         // 
         // tbRules
         // 
         this.tbRules.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                                   this.tbbNewRule,
                                                                                   this.tbbDeleteRule,
                                                                                   this.tbbSep,
                                                                                   this.tbbMoveUp,
                                                                                   this.tbbMoveDown});
         this.tbRules.Divider = false;
         this.tbRules.DropDownArrows = true;
         this.tbRules.ImageList = this.imlPlan;
         this.tbRules.Location = new System.Drawing.Point(3, 16);
         this.tbRules.Name = "tbRules";
         this.tbRules.ShowToolTips = true;
         this.tbRules.Size = new System.Drawing.Size(157, 25);
         this.tbRules.TabIndex = 3;
         this.tbRules.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbRules_ButtonClick);
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
         // tbbSep
         // 
         this.tbbSep.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
         // 
         // tbbMoveUp
         // 
         this.tbbMoveUp.ImageIndex = 2;
         this.tbbMoveUp.ToolTipText = "Move selected rule up";
         // 
         // tbbMoveDown
         // 
         this.tbbMoveDown.ImageIndex = 3;
         this.tbbMoveDown.ToolTipText = "Move selected rule down";
         // 
         // imlPlan
         // 
         this.imlPlan.ImageSize = new System.Drawing.Size(15, 15);
         this.imlPlan.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlPlan.ImageStream")));
         this.imlPlan.TransparentColor = System.Drawing.Color.Magenta;
         // 
         // splitterRules
         // 
         this.splitterRules.Dock = System.Windows.Forms.DockStyle.Right;
         this.splitterRules.Location = new System.Drawing.Point(160, 16);
         this.splitterRules.Name = "splitterRules";
         this.splitterRules.Size = new System.Drawing.Size(5, 330);
         this.splitterRules.TabIndex = 1;
         this.splitterRules.TabStop = false;
         // 
         // pnlRule
         // 
         this.pnlRule.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.pnlRule.Controls.Add(this.txtHelpText);
         this.pnlRule.Controls.Add(this.txtErrors);
         this.pnlRule.Controls.Add(this.lblOutput);
         this.pnlRule.Controls.Add(this.cboOutput);
         this.pnlRule.Controls.Add(this.txtRuleName);
         this.pnlRule.Controls.Add(this.lblRuleName);
         this.pnlRule.Controls.Add(this.chkEndIf);
         this.pnlRule.Controls.Add(this.lblParam3);
         this.pnlRule.Controls.Add(this.cboParam3);
         this.pnlRule.Controls.Add(this.cboRuleType);
         this.pnlRule.Controls.Add(this.lblParam2);
         this.pnlRule.Controls.Add(this.lblParam1);
         this.pnlRule.Controls.Add(this.cboParam2);
         this.pnlRule.Controls.Add(this.cboParam1);
         this.pnlRule.Controls.Add(this.chkNot);
         this.pnlRule.Controls.Add(this.cboFunction);
         this.pnlRule.Dock = System.Windows.Forms.DockStyle.Right;
         this.pnlRule.Location = new System.Drawing.Point(165, 16);
         this.pnlRule.Name = "pnlRule";
         this.pnlRule.Size = new System.Drawing.Size(344, 330);
         this.pnlRule.TabIndex = 2;
         // 
         // txtHelpText
         // 
         this.txtHelpText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtHelpText.BackColor = System.Drawing.SystemColors.Info;
         this.txtHelpText.ForeColor = System.Drawing.SystemColors.InfoText;
         this.txtHelpText.Location = new System.Drawing.Point(8, 72);
         this.txtHelpText.Multiline = true;
         this.txtHelpText.Name = "txtHelpText";
         this.txtHelpText.ReadOnly = true;
         this.txtHelpText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtHelpText.Size = new System.Drawing.Size(320, 32);
         this.txtHelpText.TabIndex = 37;
         this.txtHelpText.Text = "";
         // 
         // txtErrors
         // 
         this.txtErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtErrors.Location = new System.Drawing.Point(8, 240);
         this.txtErrors.Multiline = true;
         this.txtErrors.Name = "txtErrors";
         this.txtErrors.ReadOnly = true;
         this.txtErrors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtErrors.Size = new System.Drawing.Size(320, 80);
         this.txtErrors.TabIndex = 36;
         this.txtErrors.Text = "";
         this.txtErrors.Visible = false;
         // 
         // lblOutput
         // 
         this.lblOutput.Enabled = false;
         this.lblOutput.Location = new System.Drawing.Point(8, 184);
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
         this.cboOutput.Enabled = false;
         this.cboOutput.Location = new System.Drawing.Point(136, 184);
         this.cboOutput.Name = "cboOutput";
         this.cboOutput.Size = new System.Drawing.Size(192, 21);
         this.cboOutput.TabIndex = 35;
         this.cboOutput.Validated += new System.EventHandler(this.cboParam_Validated);
         // 
         // txtRuleName
         // 
         this.txtRuleName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtRuleName.Enabled = false;
         this.txtRuleName.Location = new System.Drawing.Point(96, 8);
         this.txtRuleName.Name = "txtRuleName";
         this.txtRuleName.Size = new System.Drawing.Size(232, 20);
         this.txtRuleName.TabIndex = 33;
         this.txtRuleName.Text = "";
         this.txtRuleName.Validating += new System.ComponentModel.CancelEventHandler(this.txtRuleName_Validating);
         this.txtRuleName.Validated += new System.EventHandler(this.txtRuleName_Validated);
         // 
         // lblRuleName
         // 
         this.lblRuleName.Enabled = false;
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
         this.chkEndIf.Location = new System.Drawing.Point(8, 208);
         this.chkEndIf.Name = "chkEndIf";
         this.chkEndIf.Size = new System.Drawing.Size(120, 24);
         this.chkEndIf.TabIndex = 31;
         this.chkEndIf.Text = "End If";
         this.chkEndIf.CheckedChanged += new System.EventHandler(this.chkEndIf_CheckedChanged);
         // 
         // lblParam3
         // 
         this.lblParam3.Enabled = false;
         this.lblParam3.Location = new System.Drawing.Point(8, 160);
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
         this.cboParam3.Location = new System.Drawing.Point(136, 160);
         this.cboParam3.Name = "cboParam3";
         this.cboParam3.Size = new System.Drawing.Size(192, 21);
         this.cboParam3.TabIndex = 30;
         this.cboParam3.Validated += new System.EventHandler(this.cboParam_Validated);
         // 
         // cboRuleType
         // 
         this.cboRuleType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboRuleType.Enabled = false;
         this.cboRuleType.Items.AddRange(new object[] {
                                                         "Do",
                                                         "If",
                                                         "And",
                                                         "Or",
                                                         "ElseIf",
                                                         "Else",
                                                         "EndIf"});
         this.cboRuleType.Location = new System.Drawing.Point(8, 48);
         this.cboRuleType.Name = "cboRuleType";
         this.cboRuleType.Size = new System.Drawing.Size(56, 21);
         this.cboRuleType.TabIndex = 22;
         this.cboRuleType.SelectedIndexChanged += new System.EventHandler(this.cboRuleType_SelectedIndexChanged);
         // 
         // lblParam2
         // 
         this.lblParam2.Enabled = false;
         this.lblParam2.Location = new System.Drawing.Point(8, 136);
         this.lblParam2.Name = "lblParam2";
         this.lblParam2.Size = new System.Drawing.Size(128, 21);
         this.lblParam2.TabIndex = 27;
         this.lblParam2.Text = "Parameter 2:";
         this.lblParam2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblParam1
         // 
         this.lblParam1.Enabled = false;
         this.lblParam1.Location = new System.Drawing.Point(8, 112);
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
         this.cboParam2.Location = new System.Drawing.Point(136, 136);
         this.cboParam2.Name = "cboParam2";
         this.cboParam2.Size = new System.Drawing.Size(192, 21);
         this.cboParam2.TabIndex = 28;
         this.cboParam2.Validated += new System.EventHandler(this.cboParam_Validated);
         // 
         // cboParam1
         // 
         this.cboParam1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cboParam1.Enabled = false;
         this.cboParam1.Location = new System.Drawing.Point(136, 112);
         this.cboParam1.Name = "cboParam1";
         this.cboParam1.Size = new System.Drawing.Size(192, 21);
         this.cboParam1.TabIndex = 26;
         this.cboParam1.Validated += new System.EventHandler(this.cboParam_Validated);
         // 
         // chkNot
         // 
         this.chkNot.Enabled = false;
         this.chkNot.Location = new System.Drawing.Point(80, 48);
         this.chkNot.Name = "chkNot";
         this.chkNot.Size = new System.Drawing.Size(56, 21);
         this.chkNot.TabIndex = 23;
         this.chkNot.Text = "Not";
         this.chkNot.CheckedChanged += new System.EventHandler(this.chkNot_CheckedChanged);
         // 
         // cboFunction
         // 
         this.cboFunction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cboFunction.Enabled = false;
         this.cboFunction.Location = new System.Drawing.Point(136, 48);
         this.cboFunction.Name = "cboFunction";
         this.cboFunction.Size = new System.Drawing.Size(192, 21);
         this.cboFunction.TabIndex = 24;
         this.cboFunction.Validated += new System.EventHandler(this.cboFunction_Validated);
         this.cboFunction.SelectedIndexChanged += new System.EventHandler(this.cboFunction_SelectedIndexChanged);
         // 
         // pnlName
         // 
         this.pnlName.Controls.Add(this.txtName);
         this.pnlName.Controls.Add(this.lblName);
         this.pnlName.Dock = System.Windows.Forms.DockStyle.Top;
         this.pnlName.DockPadding.All = 2;
         this.pnlName.Location = new System.Drawing.Point(0, 0);
         this.pnlName.Name = "pnlName";
         this.pnlName.Size = new System.Drawing.Size(512, 24);
         this.pnlName.TabIndex = 3;
         // 
         // dataMonitor
         // 
         this.dataMonitor.SpritePlanRowDeleted += new SGDK2.ProjectDataset.SpritePlanRowChangeEventHandler(this.dataMonitor_SpritePlanRowDeleted);
         this.dataMonitor.PlanRuleRowChanged += new SGDK2.ProjectDataset.PlanRuleRowChangeEventHandler(this.dataMonitor_PlanRuleRowChanged);
         this.dataMonitor.PlanRuleRowDeleted += new SGDK2.ProjectDataset.PlanRuleRowChangeEventHandler(this.dataMonitor_PlanRuleRowChanged);
         this.dataMonitor.PlanRuleRowChanging += new SGDK2.ProjectDataset.PlanRuleRowChangeEventHandler(this.dataMonitor_PlanRuleRowChanging);
         this.dataMonitor.PlanRuleRowDeleting += new SGDK2.ProjectDataset.PlanRuleRowChangeEventHandler(this.dataMonitor_PlanRuleRowChanging);
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         // 
         // frmPlanEdit
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(512, 373);
         this.Controls.Add(this.grpRules);
         this.Controls.Add(this.pnlName);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "frmPlanEdit";
         this.Text = "Edit Plan";
         this.grpRules.ResumeLayout(false);
         this.pnlRule.ResumeLayout(false);
         this.pnlName.ResumeLayout(false);
         this.ResumeLayout(false);

      }
      #endregion

      #region Private Methods
      private void LoadFunctions(bool onlyBools, bool forceRefresh)
      {
         if (forceRefresh || (m_AvailableRules == null))
         {
            CodeGenerator gen = new CodeGenerator();
            string errs;
            gen.GenerateLevel = CodeGenerator.CodeLevel.ExcludeRules;
            errs = gen.CompileTempAssembly(false);
            if ((errs != null) && (errs.Length > 0))
            {
               txtErrors.Text = errs;
               txtErrors.Visible = true;
               return;
            }

            txtErrors.Visible = false;
            RemotingServices.IRemoteTypeInfo reflector = CodeGenerator.CreateInstanceAndUnwrap(
               "RemoteReflector", "PlanBase") as RemotingServices.IRemoteTypeInfo;

            RemotingServices.RemoteMethodInfo[] ruleList = reflector.GetMethods();

            m_AvailableRules = new RuleTable();
            m_Enums = new EnumTable();
            m_PlanProperties = reflector.GetProperties();
            foreach(RemotingServices.RemoteMethodInfo mi in ruleList)
            {
               if ((mi.Description != null) && (mi.Description.Length > 0))
               {
                  foreach(string allowedType in new string[]
                     {
                        typeof(Boolean).Name, typeof(Int32).Name, typeof(Int16).Name,
                        typeof(Double).Name, typeof(Single).Name, typeof(void).Name
                     })
                  {
                     if (string.Compare(allowedType, mi.ReturnType) == 0)
                     {
                        m_AvailableRules[mi.MethodName] = mi;
                        break;
                     }
                  }
               }
            }

            m_AvailableRules.InsertOperators();
         }
         
         cboFunction.Items.Clear();
         RemotingServices.RemoteMethodInfo[] rules = new SGDK2.RemotingServices.RemoteMethodInfo[m_AvailableRules.Count];
         m_AvailableRules.Rules.CopyTo(rules, 0);
         System.Array.Sort(rules, new RemotingServices.RemoteMethodComparer());
         foreach(RemotingServices.RemoteMethodInfo mi in rules)
         {
            if ((string.Compare(mi.ReturnType,typeof(Boolean).Name)==0) || !onlyBools)
               cboFunction.Items.Add(mi.MethodName);
         }
         if (!onlyBools)
            chkNot.Checked = false;
         chkNot.Enabled = onlyBools;

         PopulateParameter(lblParam1, cboParam1, RemotingServices.RemoteParameterInfo.Empty, true);
         PopulateParameter(lblParam2, cboParam2, RemotingServices.RemoteParameterInfo.Empty, true);
         PopulateParameter(lblParam3, cboParam3, RemotingServices.RemoteParameterInfo.Empty, true);
         lblOutput.Enabled = false;
         cboOutput.Enabled = false;
         cboOutput.SelectedIndex = -1;
      }

      private void PrepareFunction(string funcName)
      {
         m_SpriteContext = null;

         if ((m_AvailableRules == null) || !m_AvailableRules.Contains(funcName))
         {
            PopulateParameter(lblParam1, cboParam1, RemotingServices.RemoteParameterInfo.Unknown, true);
            PopulateParameter(lblParam2, cboParam2, RemotingServices.RemoteParameterInfo.Unknown, true);
            PopulateParameter(lblParam3, cboParam3, RemotingServices.RemoteParameterInfo.Unknown, true);
            lblOutput.Enabled = true;
            cboOutput.Enabled = true;
            txtHelpText.Text = "The specified function name could not be located or the project failed to compile.";
            return;
         }

         RemotingServices.RemoteMethodInfo mi = m_AvailableRules[funcName];

         txtHelpText.Text = mi.Description;

         if (mi.Arguments.Length <= 0)
            PopulateParameter(lblParam1, cboParam1, RemotingServices.RemoteParameterInfo.Empty, true);
         else
            PopulateParameter(lblParam1, cboParam1, mi.Arguments[0], !m_Loading);
         if (mi.Arguments.Length <= 1)
            PopulateParameter(lblParam2, cboParam2, RemotingServices.RemoteParameterInfo.Empty, true);
         else
            PopulateParameter(lblParam2, cboParam2, mi.Arguments[1], !m_Loading);
         if (mi.Arguments.Length <= 2)
            PopulateParameter(lblParam3, cboParam3, RemotingServices.RemoteParameterInfo.Empty, true);
         else
            PopulateParameter(lblParam3, cboParam3, mi.Arguments[2], !m_Loading);

         if ((string.Compare(mi.ReturnType, typeof(Int32).Name) == 0) ||
            (string.Compare(mi.ReturnType, typeof(Int16).Name) == 0))
         {
            lblOutput.Enabled = true;
            cboOutput.Enabled = true;
            cboOutput.Items.Clear();
            if (m_SpriteContext != null)
               FillComboWithParams(cboOutput);
            FillComboWithIntVars(cboOutput);
         }
         else
         {
            lblOutput.Enabled = false;
            cboOutput.Enabled = false;
            cboOutput.Text = String.Empty;
            cboOutput.SelectedIndex = -1;
         }
      }

      private void FillComboWithIntVars(ComboBox cboTarget)
      {
         foreach (DataRowView drv in ProjectData.Counter.DefaultView)
            cboTarget.Items.Add(CodeGenerator.CounterClass + "." + CodeGenerator.NameToVariable(
               ((ProjectDataset.CounterRow)drv.Row).Name) + ".CurrentValue");
         foreach (RemotingServices.RemotePropertyInfo pi in m_PlanProperties)
         {
            if (pi.Type == typeof(Int32).Name)
               cboTarget.Items.Add(CodeGenerator.NameToVariable(pi.Name));
         }
      }

      private void FillComboWithCounters(ComboBox cboTarget)
      {
         foreach (DataRowView drv in ProjectData.Counter.DefaultView)
            cboTarget.Items.Add(CodeGenerator.CounterClass + "." + CodeGenerator.NameToVariable(
               ((ProjectDataset.CounterRow)drv.Row).Name));
      }

      private void FillComboWithParams(ComboBox cboParams)
      {
         ProjectDataset.SpriteDefinitionRow spriteDef = m_SpriteContext.SpriteRow.SpriteStateRowParent.SpriteDefinitionRow;
         foreach(ProjectDataset.SpriteParameterRow prow in ProjectData.GetSortedSpriteParameters(spriteDef))
         {
            cboParams.Items.Add(m_SpriteContext.ToString() + "." + CodeGenerator.NameToVariable(prow.Name));
         }
      }

      private void FillComboWithSpriteStates(ComboBox cboTarget)
      {
         ProjectDataset.SpriteDefinitionRow spriteDef = m_SpriteContext.SpriteRow.SpriteStateRowParent.SpriteDefinitionRow;
         foreach(ProjectDataset.SpriteStateRow drState in ProjectData.GetSortedSpriteStates(spriteDef))
            cboTarget.Items.Add("(int)" + CodeGenerator.SpritesNamespace + "." +
               CodeGenerator.NameToVariable(spriteDef.Name) + "." +
               CodeGenerator.SpriteStateEnumName + "." +
               CodeGenerator.NameToVariable(drState.Name));
      }

      private void PopulateParameter(Label lblParameter, ComboBox cboParameter, RemotingServices.RemoteParameterInfo param, bool clearValue)
      {
         cboParameter.Items.Clear();
         if (clearValue)
         {
            cboParameter.Text = string.Empty;
            cboParameter.SelectedIndex = -1;
         }

         if (param.IsUnknown())
         {
            lblParameter.Text = "???";
            lblParameter.Enabled = true;
            cboParameter.Enabled = true;
            return;
         }

         if (param.IsEmpty())
         {
            lblParameter.Text = "N/A";
            lblParameter.Enabled = false;
            cboParameter.Enabled = false;
            cboParameter.SelectedIndex = -1;
            return;
         }

         lblParameter.Enabled = true;
         cboParameter.Enabled = true;
         lblParameter.Text = param.Name + ": ";

         cboParameter.Items.Clear();

         if (param.IsEnum)
         {
            string[] enumVals;
            if (m_Enums.Contains(param.TypeName))
               enumVals = m_Enums[param.TypeName];
            else
               enumVals = m_Enums[param.TypeName] = GetEnumInfo(param.TypeName);

            foreach (string enumVal in enumVals)
               cboParameter.Items.Add(enumVal);
            return;
         }

         if (string.Compare(param.TypeName, typeof(bool).Name)==0)
         {
            cboParameter.Items.Add("false");
            cboParameter.Items.Add("true");
         }
         else if (string.Compare(param.TypeName, typeof(System.Drawing.Point).Name) == 0)
         {
            foreach(ProjectDataset.SpritePlanRow drPlan in ProjectData.GetSortedSpritePlans(m_Plan.LayerRowParent))
            {
               ProjectDataset.CoordinateRow[] drCoords = ProjectData.GetSortedCoordinates(drPlan);
               if (drCoords.Length == 1)
                  cboParameter.Items.Add(CodeGenerator.SpritePlanParentField + ".m_" + CodeGenerator.NameToVariable(drPlan.Name) + "[0]");
            }
            cboParameter.Items.Add(CodeGenerator.SpritePlanParentField + ".GetMousePosition()");
         }
         else if (string.Compare(param.TypeName, "SpriteBase") == 0)
         {
            foreach(ProjectDataset.SpriteRow drSprite in ProjectData.GetSortedSpriteRows(m_Plan.LayerRowParent))
            {
               cboParameter.Items.Add(new SpriteCodeRef(drSprite));
            }
         }
         else if (string.Compare(param.TypeName, "SpriteCollection") == 0)
         {
            foreach(DataRowView drv in ProjectData.SpriteCategory.DefaultView)
            {
               cboParameter.Items.Add(CodeGenerator.SpritePlanParentField + "." + CodeGenerator.NameToVariable(((ProjectDataset.SpriteCategoryRow)drv.Row).Name));
            }
         }
         else if (param.Editors != null)
         {
            foreach(string editor in param.Editors)
            {
               if (string.Compare(editor, "SpriteState", true) == 0)
               {
                  if (m_SpriteContext != null)
                     FillComboWithSpriteStates(cboParameter);
               }
            }
         }
         else if(string.Compare(param.TypeName, "Counter") == 0)
         {
            FillComboWithCounters(cboParameter);
         }
         else 
         {
            foreach(string typeName in new string[]
            {
               typeof(Int32).Name, typeof(Int16).Name,
               typeof(Double).Name, typeof(Single).Name
            })
            {
               if (string.Compare(typeName, param.TypeName) == 0)
               {
                  cboParameter.Items.Clear();
                  if (m_SpriteContext != null)
                     FillComboWithParams(cboParameter);
                  FillComboWithIntVars(cboParameter);
                  break;
               }
            }
         }
      }

      private string[] GetEnumInfo(string enumName)
      {
         string errs;
         CodeGenerator gen = new CodeGenerator();
         gen.GenerateLevel = CodeGenerator.CodeLevel.ExcludeRules;
         errs = gen.CompileTempAssembly(false);
         if (errs != null)
         {
            txtErrors.Text = errs;
            txtErrors.Visible = true;
            return new string[] {};
         }

         txtErrors.Visible = false;
         
         try
         {
            RemotingServices.IRemoteTypeInfo reflector = CodeGenerator.CreateInstanceAndUnwrap(
               "RemoteReflector", enumName) as RemotingServices.IRemoteTypeInfo;

            string[] remoteResults = reflector.GetEnumVals();

            return remoteResults;
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(this, ex.ToString(), "GetEnumInfo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return new string[] {};
         }
      }
      
      private void LoadRule(ProjectDataset.PlanRuleRow drRule)
      {
         try
         {
            m_Loading = true;
            txtRuleName.Text = drRule.Name;
            
            LoadFunctions(IsRuleTypeConditional(drRule.Type), false);

            if (cboFunction.Items.Contains(drRule.Function))
            {
               chkNot.Checked = false;
               cboFunction.Text = drRule.Function;
            }
            else if (drRule.Function.StartsWith("!"))
            {
               chkNot.Checked = true;
               cboFunction.Text = drRule.Function.Substring(1);
            }
            else
            {
               chkNot.Checked = false;
               cboFunction.Text = drRule.Function;
            }

            PrepareFunction(cboFunction.Text);

            cboRuleType.Text = drRule.Type;

            if (CurrentRule != null)
            {
               if (!cboParam1.Enabled)
                  CurrentRule.SetParameter1Null();
               if (!cboParam2.Enabled)
                  CurrentRule.SetParameter2Null();
               if (!cboParam3.Enabled)
                  CurrentRule.SetParameter3Null();
               if (!cboOutput.Enabled)
                  CurrentRule.SetResultParameterNull();
            }

            if (drRule.IsParameter1Null())
               cboParam1.SelectedIndex = -1;
            else
            {
               cboParam1.Text = drRule.Parameter1;
               DetectSpriteContext(0, drRule.Parameter1, cboParam1);
            }

            if (drRule.IsParameter2Null())
               cboParam2.SelectedIndex = -1;
            else
            {
               cboParam2.Text = drRule.Parameter2;
               DetectSpriteContext(1, drRule.Parameter2, cboParam2);
            }

            if (drRule.IsParameter3Null())
               cboParam3.SelectedIndex = -1;
            else
            {
               cboParam3.Text = drRule.Parameter3;
               DetectSpriteContext(2, drRule.Parameter3, cboParam3);
            }

            if (drRule.IsResultParameterNull())
               cboOutput.SelectedIndex = -1;
            else
               cboOutput.Text = drRule.ResultParameter;

            chkEndIf.Checked = drRule.EndIf;
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Load Rule", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
         finally
         {
            m_Loading = false;
         }
      }
      
      private void PopulateRules()
      {
         ProjectDataset.PlanRuleRow cur = CurrentRule;
         tvwRules.Nodes.Clear();
         m_TreeNodes.Clear();
         TreeNode parentNode = null;
         foreach(ProjectDataset.PlanRuleRow drRule in ProjectData.GetSortedPlanRules(m_Plan))
         {
            if (parentNode == null)
            {
               parentNode = tvwRules.Nodes.Add(drRule.Name);
               m_TreeNodes[drRule.Name] = parentNode;
               if (!DoesRuleTypeNest(drRule.Type))
                  parentNode = null;
               continue;
            }
            TreeNode curNode = parentNode.Nodes.Add(drRule.Name);
            m_TreeNodes[drRule.Name] = curNode;
            if (DoesRuleTypeNest(drRule.Type))
            {
               parentNode = curNode;
               continue;
            }
            if (drRule.EndIf)
               parentNode = parentNode.Parent;
         }
         if (cur != null)
            tvwRules.SelectedNode = GetNodeFromRow(cur);
      }

      private ProjectDataset.PlanRuleRow GetNodeRow(TreeNode node)
      {
         return ProjectData.GetPlanRule(m_Plan, node.Text);
      }

      private ProjectDataset.PlanRuleRow CurrentRule
      {
         get
         {
            if (tvwRules.SelectedNode != null)
               return ProjectData.GetPlanRule(m_Plan, tvwRules.SelectedNode.Text);
            return null;
         }
      }

      private TreeNode GetNodeFromRow(ProjectDataset.PlanRuleRow drRule)
      {
         return m_TreeNodes[drRule.Name] as TreeNode;
      }

      private void EnableFields()
      {
         if (CurrentRule == null)
         {
            txtRuleName.Enabled = lblRuleName.Enabled =
               cboRuleType.Enabled = cboFunction.Enabled = 
               chkNot.Enabled = lblParam1.Enabled = cboParam1.Enabled =
               lblParam2.Enabled = cboParam2.Enabled =
               lblParam3.Enabled = cboParam3.Enabled =
               lblOutput.Enabled = cboOutput.Enabled =
               chkEndIf.Enabled = false;
            return;
         }

         txtRuleName.Enabled = lblRuleName.Enabled =
            cboRuleType.Enabled = true;

         if (String.Compare(cboRuleType.Text, "EndIf", true) == 0)
         {
            cboFunction.Enabled = 
               chkNot.Enabled = lblParam1.Enabled = cboParam1.Enabled =
               lblParam2.Enabled = cboParam2.Enabled =
               lblParam3.Enabled = cboParam3.Enabled =
               lblOutput.Enabled = cboOutput.Enabled =
               chkEndIf.Enabled = false;
            return;
         }
         
         cboFunction.Enabled = true;

         chkEndIf.Enabled = !IsRuleTypeConditional(cboRuleType.Text);
      }

      private bool IsRuleTypeConditional(string ruleType)
      {
         return (String.Compare(ruleType, "Do", true) != 0) &&
            (String.Compare(ruleType, "Else", true) != 0);
      }

      private bool DoesRuleTypeNest(string ruleType)
      {
         return (String.Compare(ruleType, "If", true) == 0) ||
            (String.Compare(ruleType, "ElseIf", true) == 0);
      }

      private void DetectSpriteContext(int argIdx, string fldValue, ComboBox cboSource)
      {
         RemotingServices.RemoteMethodInfo rmi = m_AvailableRules[cboFunction.Text];
         if ((argIdx >= 0) && (rmi.Arguments[argIdx].TypeName == "SpriteBase") && (cboSource.SelectedIndex >= 0) && (cboSource.Items[cboSource.SelectedIndex] is SpriteCodeRef))
         {
            SpriteCodeRef sprite = (SpriteCodeRef)cboSource.Items[cboSource.SelectedIndex];
            if (sprite != null)
            {
               m_SpriteContext = sprite;
               if ((rmi.Arguments.Length >= 1) && (argIdx != 0))
                  PopulateParameter(lblParam1, cboParam1, rmi.Arguments[0], false);
               if ((rmi.Arguments.Length >= 2) && (argIdx != 1))
                  PopulateParameter(lblParam2, cboParam2, rmi.Arguments[1], false);
               if ((rmi.Arguments.Length >= 3) && (argIdx != 2))
                  PopulateParameter(lblParam3, cboParam3, rmi.Arguments[2], false);
               if ((string.Compare(rmi.ReturnType, typeof(Int32).Name) == 0) ||
                  (string.Compare(rmi.ReturnType, typeof(Int16).Name) == 0))
               {
                  cboOutput.Items.Clear();
                  FillComboWithParams(cboOutput);
                  FillComboWithIntVars(cboOutput);
               }
            }
         }
      }
      #endregion

      #region Events Handlers
      private void txtName_Validated(object sender, System.EventArgs e)
      {
         if (m_Loading)
            return;
         m_Plan.Name = txtName.Text;
      }

      private void txtName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         if (m_Loading)
            return;

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

      private void cboRuleType_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (m_Loading)
            return;
         if (String.Compare(cboRuleType.Text, "EndIf", true) == 0)
            EnableFields();
         else
            LoadFunctions(IsRuleTypeConditional(cboRuleType.SelectedItem.ToString()), false);
         if (CurrentRule != null)
         {
            CurrentRule.Type = cboRuleType.Text;
            if (String.Compare(cboRuleType.Text, "EndIf", true) == 0)
            {
               CurrentRule.Function = cboRuleType.Text;
               CurrentRule.EndIf = true;
            }
         }
      }

      private void cboFunction_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         EnableFields();
         PrepareFunction(cboFunction.SelectedItem.ToString());
      }

      private void cboFunction_Validated(object sender, System.EventArgs e)
      {
         PrepareFunction(cboFunction.Text);

         if (m_Loading || (CurrentRule == null))
            return;

         if (chkNot.Checked)
            CurrentRule.Function = "!" + cboFunction.Text;
         else
            CurrentRule.Function = cboFunction.Text;
      }

      private void tbRules_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
      {
         if (e.Button == tbbNewRule)
         {
            tvwRules.Focus(); // Force validation
            string newPlanName;
            int i = 0;
            do
            {
               newPlanName = "Rule " + (++i).ToString();
            } while (ProjectData.GetPlanRule(m_Plan, newPlanName) != null);
            int newSeq = -1;
            if (CurrentRule != null)
               newSeq = CurrentRule.Sequence + 1;
            ProjectDataset.PlanRuleRow drRule = ProjectData.InsertPlanRule(m_Plan, newPlanName, "Do", newSeq, cboFunction.Text, null, null, null, null, false);
            tvwRules.SelectedNode = GetNodeFromRow(drRule);
         }
         else if (e.Button == tbbDeleteRule)
         {
            if (tvwRules.SelectedNode == null)
            {
               MessageBox.Show(this, "Select a rule first.", "Delete Rule", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return;
            }
            ProjectData.DeletePlanRule(CurrentRule);
         }
         else if (e.Button == tbbMoveUp)
         {
            if (tvwRules.SelectedNode == null)
            {
               MessageBox.Show(this, "Select a rule first.", "Move Rule Up", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return;
            }
            ProjectData.MovePlanRule(CurrentRule, false);
         }
         else if (e.Button == tbbMoveDown)
         {
            if (tvwRules.SelectedNode == null)
            {
               MessageBox.Show(this, "Select a rule first.", "Move Rule Down", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return;
            }
            ProjectData.MovePlanRule(CurrentRule, true);
         }
      }

      private void dataMonitor_PlanRuleRowChanging(object sender, SGDK2.ProjectDataset.PlanRuleRowChangeEvent e)
      {
         if (((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.Delete)) && (e.Row.SpritePlanRowParent == m_Plan))
         {
            m_OldRuleName = e.Row[ProjectData.PlanRule.NameColumn,DataRowVersion.Current].ToString();
            m_OldSequence = (int)e.Row[ProjectData.PlanRule.SequenceColumn,DataRowVersion.Current];
            m_OldType = e.Row[ProjectData.PlanRule.TypeColumn,DataRowVersion.Current].ToString();
            m_OldEndIf = (bool)e.Row[ProjectData.PlanRule.EndIfColumn,DataRowVersion.Current];
         }
      }

      private void dataMonitor_PlanRuleRowChanged(object sender, SGDK2.ProjectDataset.PlanRuleRowChangeEvent e)
      {
         switch(e.Action)
         {
            case DataRowAction.Add:
               if (e.Row.SpritePlanRowParent == m_Plan)
                  PopulateRules();
               break;
            case DataRowAction.Change:
               if ((e.Row.SpritePlanRowParent == m_Plan) && (m_OldRuleName != null))
               {
                  if ((String.Compare(m_OldRuleName, e.Row.Name) != 0))
                     tvwRules.SelectedNode.Text = e.Row.Name;
                  else if ((m_OldSequence != e.Row.Sequence) ||
                     (String.Compare(m_OldType,e.Row.Type) != 0) ||
                     (m_OldEndIf != e.Row.EndIf))
                     PopulateRules();
               }
               break;
            case DataRowAction.Delete:
               if (m_OldRuleName != null)
                  PopulateRules();
               EnableFields();
               break;
         }
         m_OldRuleName = null;
         m_OldSequence = -1;
      }

      private void chkEndIf_CheckedChanged(object sender, System.EventArgs e)
      {
         if (m_Loading)
            return;
         if (CurrentRule != null)
            CurrentRule.EndIf = chkEndIf.Checked;
      }

      private void tvwRules_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
      {
         LoadRule(CurrentRule);
         EnableFields();
      }

      private void cboParam_Validated(object sender, System.EventArgs e)
      {
         if (m_Loading || (CurrentRule == null))
            return;

         string fldValue = ((ComboBox)sender).Text;
         if (sender == cboParam1)
         {
            CurrentRule.Parameter1 = fldValue;
            DetectSpriteContext(0, fldValue, cboParam1);
         }
         if (sender == cboParam2)
         {
            CurrentRule.Parameter2 = fldValue;
            DetectSpriteContext(1, fldValue, cboParam2);
         }
         if (sender == cboParam3)
         {
            CurrentRule.Parameter3 = fldValue;
            DetectSpriteContext(2, fldValue, cboParam3);
         }
         if (sender == cboOutput)
            CurrentRule.ResultParameter = fldValue;
      }

      private void txtRuleName_Validated(object sender, System.EventArgs e)
      {
         if (m_Loading || (CurrentRule == null))
            return;
         CurrentRule.Name = txtRuleName.Text;      
      }

      private void chkNot_CheckedChanged(object sender, System.EventArgs e)
      {
         if (m_Loading)
            return;
         if (CurrentRule != null)
         {
            if (chkNot.Checked)
               CurrentRule.Function = "!" + cboFunction.Text;
            else
               CurrentRule.Function = cboFunction.Text;
         }
      }

      private void txtRuleName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         ProjectDataset.PlanRuleRow pr = ProjectData.GetPlanRule(m_Plan, txtRuleName.Text);
         if ((null != pr) && (CurrentRule != pr))
         {
            if (DialogResult.Cancel == MessageBox.Show(this, txtRuleName.Text + " already exists", "Plan Rule Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtRuleName.Text = CurrentRule.Name;
            e.Cancel = true;
         }
         ProjectDataset.SpriteRow sr = ProjectData.GetSprite(m_Plan.LayerRowParent, txtRuleName.Text);
         if (null != sr)
         {
            if (DialogResult.Cancel == MessageBox.Show(this, txtRuleName.Text + " conflicts with the name of an existing sprite on the same layer", "Plan Rule Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtRuleName.Text = CurrentRule.Name;
            e.Cancel = true;
         }
      }
      #endregion
   }
}
