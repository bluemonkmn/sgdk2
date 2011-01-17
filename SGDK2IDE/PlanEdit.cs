/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
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
      public SpriteCodeRef m_SpriteContext = null;
      private Hashtable m_CustomObjects = null; // .[TypeName] -> RemoteGlobalAccessorInfo[]
      private string m_PreparedFunction = string.Empty;
      public const string SelectSpriteParameterItem = "<Select sprite parameter...>";
      public const string SelectWritableSpriteParameterItem = "<Select writable sprite parameter...>";
      private const string SpriteFunctionItem = "<Select sprite function...>";
      private ProjectDataset.PlanRuleRow m_SelectAfterLoad = null;
      #endregion

      #region Embedded Classes
      public class SpriteCodeRef
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
      private System.Windows.Forms.CheckBox chkSuspended;
      private System.Windows.Forms.MainMenu mnuMain;
      private System.Windows.Forms.MenuItem mnuPlan;
      private System.Windows.Forms.MenuItem mnuMoveRuleUp;
      private System.Windows.Forms.MenuItem mnuMoveRuleDown;
      private System.Windows.Forms.MenuItem mnuNewRule;
      private System.Windows.Forms.MenuItem mnuDeleteRule;
      private System.Windows.Forms.TextBox txtPriority;
      private System.Windows.Forms.Label lblPriority;
      private System.Windows.Forms.Timer tmrPopulate;
      private System.Windows.Forms.MenuItem mnuCopyRules;
      private System.Windows.Forms.MenuItem mnuCopyChildren;
      private System.Windows.Forms.MenuItem mnuCopySelected;
      private System.Windows.Forms.MenuItem mnuCopyAll;
      private System.Windows.Forms.MenuItem mnuPasteRules;
      private System.Windows.Forms.MenuItem mnuPasteAbove;
      private System.Windows.Forms.MenuItem mnuPasteBelow;
      private System.Windows.Forms.ImageList imlTree;
      private System.Windows.Forms.ToolBarButton tbbSep2;
      private System.Windows.Forms.ToolBarButton tbbToggleSuspend;
      private System.Windows.Forms.MenuItem mnuToggleSuspend;
      private MenuItem mnuCutRules;
      private MenuItem mnuCutChildren;
      private MenuItem mnuCutSelected;
      private MenuItem mnuCutAll;
      private MenuItem mnuConvertToFunc;
      private MenuItem mnuConvertAll;
      private MenuItem mnuConvertSelected;
      private ToolBarButton tbbConvertToFunc;
      private ComboBox cboBaseClass;
      private Label lblBaseClass;
      private System.ComponentModel.IContainer components;
      #endregion

      #region Initialization and Clean-up
      public frmPlanEdit(ProjectDataset.LayerRow parent)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         String sName;
         Int32 nIdx = 1;
         do
            sName = parent.MapRow.Name + " Plan " + (nIdx++).ToString();
         while (ProjectData.GetSpritePlan(parent, sName) != null);

         m_Plan = ProjectData.AddSpritePlan(parent, sName, 1, CodeGenerator.PlanBaseClassName);
         txtName.Text = sName;
         cboBaseClass.Text = m_Plan.BaseClass;
         txtPriority.Text = m_Plan.Priority.ToString();
         PopulateBaseClasses();

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"html/9cc40393-eb18-4a5d-b579-a2006b78e398.htm");
         SGDK2IDE.g_HelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
      }

      public frmPlanEdit(ProjectDataset.SpritePlanRow plan)
      {
         InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         m_Plan = plan;
         txtName.Text = plan.Name;
         cboBaseClass.Text = m_Plan.BaseClass;
         txtPriority.Text = plan.Priority.ToString();
         PopulateBaseClasses();
         
         QueuePopulateRules();

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"html/9cc40393-eb18-4a5d-b579-a2006b78e398.htm");
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPlanEdit));
         this.lblName = new System.Windows.Forms.Label();
         this.txtName = new System.Windows.Forms.TextBox();
         this.grpRules = new System.Windows.Forms.GroupBox();
         this.tvwRules = new System.Windows.Forms.TreeView();
         this.imlTree = new System.Windows.Forms.ImageList(this.components);
         this.tbRules = new System.Windows.Forms.ToolBar();
         this.tbbNewRule = new System.Windows.Forms.ToolBarButton();
         this.tbbDeleteRule = new System.Windows.Forms.ToolBarButton();
         this.tbbSep = new System.Windows.Forms.ToolBarButton();
         this.tbbMoveUp = new System.Windows.Forms.ToolBarButton();
         this.tbbMoveDown = new System.Windows.Forms.ToolBarButton();
         this.tbbSep2 = new System.Windows.Forms.ToolBarButton();
         this.tbbToggleSuspend = new System.Windows.Forms.ToolBarButton();
         this.tbbConvertToFunc = new System.Windows.Forms.ToolBarButton();
         this.imlPlan = new System.Windows.Forms.ImageList(this.components);
         this.splitterRules = new System.Windows.Forms.Splitter();
         this.pnlRule = new System.Windows.Forms.Panel();
         this.chkSuspended = new System.Windows.Forms.CheckBox();
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
         this.cboBaseClass = new System.Windows.Forms.ComboBox();
         this.lblBaseClass = new System.Windows.Forms.Label();
         this.lblPriority = new System.Windows.Forms.Label();
         this.txtPriority = new System.Windows.Forms.TextBox();
         this.dataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.mnuMain = new System.Windows.Forms.MainMenu(this.components);
         this.mnuPlan = new System.Windows.Forms.MenuItem();
         this.mnuMoveRuleUp = new System.Windows.Forms.MenuItem();
         this.mnuMoveRuleDown = new System.Windows.Forms.MenuItem();
         this.mnuNewRule = new System.Windows.Forms.MenuItem();
         this.mnuDeleteRule = new System.Windows.Forms.MenuItem();
         this.mnuCopyRules = new System.Windows.Forms.MenuItem();
         this.mnuCopyChildren = new System.Windows.Forms.MenuItem();
         this.mnuCopySelected = new System.Windows.Forms.MenuItem();
         this.mnuCopyAll = new System.Windows.Forms.MenuItem();
         this.mnuCutRules = new System.Windows.Forms.MenuItem();
         this.mnuCutChildren = new System.Windows.Forms.MenuItem();
         this.mnuCutSelected = new System.Windows.Forms.MenuItem();
         this.mnuCutAll = new System.Windows.Forms.MenuItem();
         this.mnuPasteRules = new System.Windows.Forms.MenuItem();
         this.mnuPasteAbove = new System.Windows.Forms.MenuItem();
         this.mnuPasteBelow = new System.Windows.Forms.MenuItem();
         this.mnuToggleSuspend = new System.Windows.Forms.MenuItem();
         this.mnuConvertToFunc = new System.Windows.Forms.MenuItem();
         this.mnuConvertAll = new System.Windows.Forms.MenuItem();
         this.mnuConvertSelected = new System.Windows.Forms.MenuItem();
         this.tmrPopulate = new System.Windows.Forms.Timer(this.components);
         this.grpRules.SuspendLayout();
         this.pnlRule.SuspendLayout();
         this.pnlName.SuspendLayout();
         this.SuspendLayout();
         // 
         // lblName
         // 
         this.lblName.Location = new System.Drawing.Point(2, 2);
         this.lblName.Name = "lblName";
         this.lblName.Size = new System.Drawing.Size(72, 20);
         this.lblName.TabIndex = 0;
         this.lblName.Text = "Name:";
         this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtName
         // 
         this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtName.Location = new System.Drawing.Point(74, 2);
         this.txtName.Name = "txtName";
         this.txtName.Size = new System.Drawing.Size(321, 20);
         this.txtName.TabIndex = 1;
         this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
         this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
         // 
         // grpRules
         // 
         this.grpRules.Controls.Add(this.tvwRules);
         this.grpRules.Controls.Add(this.tbRules);
         this.grpRules.Controls.Add(this.splitterRules);
         this.grpRules.Controls.Add(this.pnlRule);
         this.grpRules.Dock = System.Windows.Forms.DockStyle.Fill;
         this.grpRules.Location = new System.Drawing.Point(0, 53);
         this.grpRules.Name = "grpRules";
         this.grpRules.Size = new System.Drawing.Size(533, 356);
         this.grpRules.TabIndex = 2;
         this.grpRules.TabStop = false;
         this.grpRules.Text = "Rules";
         // 
         // tvwRules
         // 
         this.tvwRules.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tvwRules.HideSelection = false;
         this.tvwRules.ImageIndex = 0;
         this.tvwRules.ImageList = this.imlTree;
         this.tvwRules.Location = new System.Drawing.Point(3, 41);
         this.tvwRules.Name = "tvwRules";
         this.tvwRules.SelectedImageIndex = 0;
         this.tvwRules.Size = new System.Drawing.Size(154, 312);
         this.tvwRules.TabIndex = 0;
         this.tvwRules.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwRules_AfterSelect);
         // 
         // imlTree
         // 
         this.imlTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlTree.ImageStream")));
         this.imlTree.TransparentColor = System.Drawing.Color.Magenta;
         this.imlTree.Images.SetKeyName(0, "");
         this.imlTree.Images.SetKeyName(1, "");
         this.imlTree.Images.SetKeyName(2, "");
         this.imlTree.Images.SetKeyName(3, "");
         this.imlTree.Images.SetKeyName(4, "");
         this.imlTree.Images.SetKeyName(5, "");
         this.imlTree.Images.SetKeyName(6, "");
         this.imlTree.Images.SetKeyName(7, "");
         this.imlTree.Images.SetKeyName(8, "");
         this.imlTree.Images.SetKeyName(9, "");
         this.imlTree.Images.SetKeyName(10, "");
         this.imlTree.Images.SetKeyName(11, "");
         this.imlTree.Images.SetKeyName(12, "");
         this.imlTree.Images.SetKeyName(13, "");
         this.imlTree.Images.SetKeyName(14, "");
         this.imlTree.Images.SetKeyName(15, "");
         // 
         // tbRules
         // 
         this.tbRules.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.tbbNewRule,
            this.tbbDeleteRule,
            this.tbbSep,
            this.tbbMoveUp,
            this.tbbMoveDown,
            this.tbbSep2,
            this.tbbToggleSuspend,
            this.tbbConvertToFunc});
         this.tbRules.Divider = false;
         this.tbRules.DropDownArrows = true;
         this.tbRules.ImageList = this.imlPlan;
         this.tbRules.Location = new System.Drawing.Point(3, 16);
         this.tbRules.Name = "tbRules";
         this.tbRules.ShowToolTips = true;
         this.tbRules.Size = new System.Drawing.Size(154, 25);
         this.tbRules.TabIndex = 3;
         this.tbRules.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbRules_ButtonClick);
         // 
         // tbbNewRule
         // 
         this.tbbNewRule.ImageIndex = 0;
         this.tbbNewRule.Name = "tbbNewRule";
         this.tbbNewRule.ToolTipText = "Add new rule";
         // 
         // tbbDeleteRule
         // 
         this.tbbDeleteRule.ImageIndex = 1;
         this.tbbDeleteRule.Name = "tbbDeleteRule";
         this.tbbDeleteRule.ToolTipText = "Delete selected rule";
         // 
         // tbbSep
         // 
         this.tbbSep.Name = "tbbSep";
         this.tbbSep.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
         // 
         // tbbMoveUp
         // 
         this.tbbMoveUp.ImageIndex = 2;
         this.tbbMoveUp.Name = "tbbMoveUp";
         this.tbbMoveUp.ToolTipText = "Move selected rule up";
         // 
         // tbbMoveDown
         // 
         this.tbbMoveDown.ImageIndex = 3;
         this.tbbMoveDown.Name = "tbbMoveDown";
         this.tbbMoveDown.ToolTipText = "Move selected rule down";
         // 
         // tbbSep2
         // 
         this.tbbSep2.Name = "tbbSep2";
         this.tbbSep2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
         // 
         // tbbToggleSuspend
         // 
         this.tbbToggleSuspend.ImageIndex = 4;
         this.tbbToggleSuspend.Name = "tbbToggleSuspend";
         this.tbbToggleSuspend.ToolTipText = "Toggle suspend for this rule and its children";
         // 
         // tbbConvertToFunc
         // 
         this.tbbConvertToFunc.ImageIndex = 5;
         this.tbbConvertToFunc.Name = "tbbConvertToFunc";
         this.tbbConvertToFunc.ToolTipText = "Convert selected rule and children to function";
         // 
         // imlPlan
         // 
         this.imlPlan.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlPlan.ImageStream")));
         this.imlPlan.TransparentColor = System.Drawing.Color.Magenta;
         this.imlPlan.Images.SetKeyName(0, "");
         this.imlPlan.Images.SetKeyName(1, "");
         this.imlPlan.Images.SetKeyName(2, "");
         this.imlPlan.Images.SetKeyName(3, "");
         this.imlPlan.Images.SetKeyName(4, "");
         this.imlPlan.Images.SetKeyName(5, "Code.bmp");
         // 
         // splitterRules
         // 
         this.splitterRules.Dock = System.Windows.Forms.DockStyle.Right;
         this.splitterRules.Location = new System.Drawing.Point(157, 16);
         this.splitterRules.Name = "splitterRules";
         this.splitterRules.Size = new System.Drawing.Size(5, 337);
         this.splitterRules.TabIndex = 1;
         this.splitterRules.TabStop = false;
         // 
         // pnlRule
         // 
         this.pnlRule.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.pnlRule.Controls.Add(this.chkSuspended);
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
         this.pnlRule.Location = new System.Drawing.Point(162, 16);
         this.pnlRule.Name = "pnlRule";
         this.pnlRule.Size = new System.Drawing.Size(368, 337);
         this.pnlRule.TabIndex = 2;
         // 
         // chkSuspended
         // 
         this.chkSuspended.Location = new System.Drawing.Point(8, 32);
         this.chkSuspended.Name = "chkSuspended";
         this.chkSuspended.Size = new System.Drawing.Size(344, 16);
         this.chkSuspended.TabIndex = 7;
         this.chkSuspended.Text = "Suspend this rule";
         this.chkSuspended.CheckedChanged += new System.EventHandler(this.chkSuspended_CheckedChanged);
         // 
         // txtHelpText
         // 
         this.txtHelpText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtHelpText.BackColor = System.Drawing.SystemColors.Info;
         this.txtHelpText.ForeColor = System.Drawing.SystemColors.InfoText;
         this.txtHelpText.Location = new System.Drawing.Point(8, 80);
         this.txtHelpText.Multiline = true;
         this.txtHelpText.Name = "txtHelpText";
         this.txtHelpText.ReadOnly = true;
         this.txtHelpText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtHelpText.Size = new System.Drawing.Size(344, 32);
         this.txtHelpText.TabIndex = 11;
         // 
         // txtErrors
         // 
         this.txtErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtErrors.Location = new System.Drawing.Point(8, 248);
         this.txtErrors.Multiline = true;
         this.txtErrors.Name = "txtErrors";
         this.txtErrors.ReadOnly = true;
         this.txtErrors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtErrors.Size = new System.Drawing.Size(344, 79);
         this.txtErrors.TabIndex = 34;
         this.txtErrors.Visible = false;
         // 
         // lblOutput
         // 
         this.lblOutput.Enabled = false;
         this.lblOutput.Location = new System.Drawing.Point(8, 192);
         this.lblOutput.Name = "lblOutput";
         this.lblOutput.Size = new System.Drawing.Size(128, 21);
         this.lblOutput.TabIndex = 31;
         this.lblOutput.Text = "Output to:";
         this.lblOutput.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // cboOutput
         // 
         this.cboOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.cboOutput.Enabled = false;
         this.cboOutput.Location = new System.Drawing.Point(136, 192);
         this.cboOutput.Name = "cboOutput";
         this.cboOutput.Size = new System.Drawing.Size(216, 21);
         this.cboOutput.TabIndex = 32;
         this.cboOutput.SelectedIndexChanged += new System.EventHandler(this.cboParam_SelectedIndexChanged);
         this.cboOutput.Validated += new System.EventHandler(this.cboParam_Validated);
         // 
         // txtRuleName
         // 
         this.txtRuleName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtRuleName.Enabled = false;
         this.txtRuleName.Location = new System.Drawing.Point(96, 8);
         this.txtRuleName.Name = "txtRuleName";
         this.txtRuleName.Size = new System.Drawing.Size(256, 20);
         this.txtRuleName.TabIndex = 6;
         this.txtRuleName.Validated += new System.EventHandler(this.txtRuleName_Validated);
         this.txtRuleName.Validating += new System.ComponentModel.CancelEventHandler(this.txtRuleName_Validating);
         // 
         // lblRuleName
         // 
         this.lblRuleName.Enabled = false;
         this.lblRuleName.Location = new System.Drawing.Point(8, 8);
         this.lblRuleName.Name = "lblRuleName";
         this.lblRuleName.Size = new System.Drawing.Size(88, 20);
         this.lblRuleName.TabIndex = 5;
         this.lblRuleName.Text = "Rule Name:";
         this.lblRuleName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // chkEndIf
         // 
         this.chkEndIf.Enabled = false;
         this.chkEndIf.Location = new System.Drawing.Point(8, 216);
         this.chkEndIf.Name = "chkEndIf";
         this.chkEndIf.Size = new System.Drawing.Size(120, 24);
         this.chkEndIf.TabIndex = 33;
         this.chkEndIf.Text = "End If/End While";
         this.chkEndIf.CheckedChanged += new System.EventHandler(this.chkEndIf_CheckedChanged);
         // 
         // lblParam3
         // 
         this.lblParam3.Enabled = false;
         this.lblParam3.Location = new System.Drawing.Point(8, 168);
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
         this.cboParam3.Location = new System.Drawing.Point(136, 168);
         this.cboParam3.Name = "cboParam3";
         this.cboParam3.Size = new System.Drawing.Size(216, 21);
         this.cboParam3.TabIndex = 30;
         this.cboParam3.SelectionChangeCommitted += new System.EventHandler(this.cboParam_SelectionChangeCommitted);
         this.cboParam3.SelectedIndexChanged += new System.EventHandler(this.cboParam_SelectedIndexChanged);
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
            "End",
            "While"});
         this.cboRuleType.Location = new System.Drawing.Point(8, 56);
         this.cboRuleType.Name = "cboRuleType";
         this.cboRuleType.Size = new System.Drawing.Size(56, 21);
         this.cboRuleType.TabIndex = 8;
         this.cboRuleType.SelectedIndexChanged += new System.EventHandler(this.cboRuleType_SelectedIndexChanged);
         // 
         // lblParam2
         // 
         this.lblParam2.Enabled = false;
         this.lblParam2.Location = new System.Drawing.Point(8, 144);
         this.lblParam2.Name = "lblParam2";
         this.lblParam2.Size = new System.Drawing.Size(128, 21);
         this.lblParam2.TabIndex = 27;
         this.lblParam2.Text = "Parameter 2:";
         this.lblParam2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblParam1
         // 
         this.lblParam1.Enabled = false;
         this.lblParam1.Location = new System.Drawing.Point(8, 120);
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
         this.cboParam2.Location = new System.Drawing.Point(136, 144);
         this.cboParam2.Name = "cboParam2";
         this.cboParam2.Size = new System.Drawing.Size(216, 21);
         this.cboParam2.TabIndex = 28;
         this.cboParam2.SelectionChangeCommitted += new System.EventHandler(this.cboParam_SelectionChangeCommitted);
         this.cboParam2.SelectedIndexChanged += new System.EventHandler(this.cboParam_SelectedIndexChanged);
         this.cboParam2.Validated += new System.EventHandler(this.cboParam_Validated);
         // 
         // cboParam1
         // 
         this.cboParam1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.cboParam1.Enabled = false;
         this.cboParam1.Location = new System.Drawing.Point(136, 120);
         this.cboParam1.Name = "cboParam1";
         this.cboParam1.Size = new System.Drawing.Size(216, 21);
         this.cboParam1.TabIndex = 26;
         this.cboParam1.SelectionChangeCommitted += new System.EventHandler(this.cboParam_SelectionChangeCommitted);
         this.cboParam1.SelectedIndexChanged += new System.EventHandler(this.cboParam_SelectedIndexChanged);
         this.cboParam1.Validated += new System.EventHandler(this.cboParam_Validated);
         // 
         // chkNot
         // 
         this.chkNot.Enabled = false;
         this.chkNot.Location = new System.Drawing.Point(80, 56);
         this.chkNot.Name = "chkNot";
         this.chkNot.Size = new System.Drawing.Size(56, 21);
         this.chkNot.TabIndex = 9;
         this.chkNot.Text = "Not";
         this.chkNot.CheckedChanged += new System.EventHandler(this.chkNot_CheckedChanged);
         // 
         // cboFunction
         // 
         this.cboFunction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.cboFunction.Enabled = false;
         this.cboFunction.Location = new System.Drawing.Point(136, 56);
         this.cboFunction.Name = "cboFunction";
         this.cboFunction.Size = new System.Drawing.Size(216, 21);
         this.cboFunction.TabIndex = 10;
         this.cboFunction.SelectedIndexChanged += new System.EventHandler(this.cboFunction_SelectedIndexChanged);
         this.cboFunction.Validated += new System.EventHandler(this.cboFunction_Validated);
         // 
         // pnlName
         // 
         this.pnlName.Controls.Add(this.cboBaseClass);
         this.pnlName.Controls.Add(this.lblBaseClass);
         this.pnlName.Controls.Add(this.txtName);
         this.pnlName.Controls.Add(this.lblPriority);
         this.pnlName.Controls.Add(this.lblName);
         this.pnlName.Controls.Add(this.txtPriority);
         this.pnlName.Dock = System.Windows.Forms.DockStyle.Top;
         this.pnlName.Location = new System.Drawing.Point(0, 0);
         this.pnlName.Name = "pnlName";
         this.pnlName.Padding = new System.Windows.Forms.Padding(2);
         this.pnlName.Size = new System.Drawing.Size(533, 53);
         this.pnlName.TabIndex = 3;
         // 
         // cboBaseClass
         // 
         this.cboBaseClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.cboBaseClass.FormattingEnabled = true;
         this.cboBaseClass.Location = new System.Drawing.Point(74, 28);
         this.cboBaseClass.Name = "cboBaseClass";
         this.cboBaseClass.Size = new System.Drawing.Size(320, 21);
         this.cboBaseClass.TabIndex = 44;
         this.cboBaseClass.Validated += new System.EventHandler(this.cboBaseClass_Validated);
         // 
         // lblBaseClass
         // 
         this.lblBaseClass.AutoSize = true;
         this.lblBaseClass.Location = new System.Drawing.Point(2, 31);
         this.lblBaseClass.Name = "lblBaseClass";
         this.lblBaseClass.Size = new System.Drawing.Size(62, 13);
         this.lblBaseClass.TabIndex = 43;
         this.lblBaseClass.Text = "Base Class:";
         // 
         // lblPriority
         // 
         this.lblPriority.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.lblPriority.Location = new System.Drawing.Point(395, 2);
         this.lblPriority.Name = "lblPriority";
         this.lblPriority.Size = new System.Drawing.Size(64, 20);
         this.lblPriority.TabIndex = 41;
         this.lblPriority.Text = "Priority:";
         this.lblPriority.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // txtPriority
         // 
         this.txtPriority.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.txtPriority.Location = new System.Drawing.Point(459, 2);
         this.txtPriority.Name = "txtPriority";
         this.txtPriority.Size = new System.Drawing.Size(72, 20);
         this.txtPriority.TabIndex = 42;
         this.txtPriority.Validated += new System.EventHandler(this.txtPriority_Validated);
         this.txtPriority.Validating += new System.ComponentModel.CancelEventHandler(this.txtPriority_Validating);
         // 
         // dataMonitor
         // 
         this.dataMonitor.PlanRuleRowChanging += new SGDK2.ProjectDataset.PlanRuleRowChangeEventHandler(this.dataMonitor_PlanRuleRowChanging);
         this.dataMonitor.PlanRuleRowChanged += new SGDK2.ProjectDataset.PlanRuleRowChangeEventHandler(this.dataMonitor_PlanRuleRowChanged);
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         this.dataMonitor.PlanRuleRowDeleting += new SGDK2.ProjectDataset.PlanRuleRowChangeEventHandler(this.dataMonitor_PlanRuleRowChanging);
         this.dataMonitor.PlanRuleRowDeleted += new SGDK2.ProjectDataset.PlanRuleRowChangeEventHandler(this.dataMonitor_PlanRuleRowChanged);
         this.dataMonitor.SpritePlanRowDeleted += new SGDK2.ProjectDataset.SpritePlanRowChangeEventHandler(this.dataMonitor_SpritePlanRowDeleted);
         // 
         // mnuMain
         // 
         this.mnuMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuPlan});
         // 
         // mnuPlan
         // 
         this.mnuPlan.Index = 0;
         this.mnuPlan.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuMoveRuleUp,
            this.mnuMoveRuleDown,
            this.mnuNewRule,
            this.mnuDeleteRule,
            this.mnuCopyRules,
            this.mnuCutRules,
            this.mnuPasteRules,
            this.mnuToggleSuspend,
            this.mnuConvertToFunc});
         this.mnuPlan.Text = "&Plan";
         // 
         // mnuMoveRuleUp
         // 
         this.mnuMoveRuleUp.Index = 0;
         this.mnuMoveRuleUp.Text = "Move Rule &Up";
         this.mnuMoveRuleUp.Click += new System.EventHandler(this.OnMoveRuleUp);
         // 
         // mnuMoveRuleDown
         // 
         this.mnuMoveRuleDown.Index = 1;
         this.mnuMoveRuleDown.Text = "Move Rule &Down";
         this.mnuMoveRuleDown.Click += new System.EventHandler(this.OnMoveRuleDown);
         // 
         // mnuNewRule
         // 
         this.mnuNewRule.Index = 2;
         this.mnuNewRule.Text = "&Add New Rule";
         this.mnuNewRule.Click += new System.EventHandler(this.OnAddRule);
         // 
         // mnuDeleteRule
         // 
         this.mnuDeleteRule.Index = 3;
         this.mnuDeleteRule.Text = "Dele&te Rule";
         this.mnuDeleteRule.Click += new System.EventHandler(this.OnDeleteRule);
         // 
         // mnuCopyRules
         // 
         this.mnuCopyRules.Index = 4;
         this.mnuCopyRules.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuCopyChildren,
            this.mnuCopySelected,
            this.mnuCopyAll});
         this.mnuCopyRules.Text = "&Copy Rules";
         // 
         // mnuCopyChildren
         // 
         this.mnuCopyChildren.Index = 0;
         this.mnuCopyChildren.Text = "&Copy Selected Rule Including Children";
         this.mnuCopyChildren.Click += new System.EventHandler(this.mnuCopyRules_Click);
         // 
         // mnuCopySelected
         // 
         this.mnuCopySelected.Index = 1;
         this.mnuCopySelected.Text = "Copy &Selected Rule Only";
         this.mnuCopySelected.Click += new System.EventHandler(this.mnuCopyRules_Click);
         // 
         // mnuCopyAll
         // 
         this.mnuCopyAll.Index = 2;
         this.mnuCopyAll.Text = "Copy &All Rules";
         this.mnuCopyAll.Click += new System.EventHandler(this.mnuCopyRules_Click);
         // 
         // mnuCutRules
         // 
         this.mnuCutRules.Index = 5;
         this.mnuCutRules.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuCutChildren,
            this.mnuCutSelected,
            this.mnuCutAll});
         this.mnuCutRules.Text = "Cut &Rules";
         // 
         // mnuCutChildren
         // 
         this.mnuCutChildren.Index = 0;
         this.mnuCutChildren.Text = "&Cut Selected Rule Including Children";
         this.mnuCutChildren.Click += new System.EventHandler(this.mnuCopyRules_Click);
         // 
         // mnuCutSelected
         // 
         this.mnuCutSelected.Index = 1;
         this.mnuCutSelected.Text = "Cut &Selected Rule Only";
         this.mnuCutSelected.Click += new System.EventHandler(this.mnuCopyRules_Click);
         // 
         // mnuCutAll
         // 
         this.mnuCutAll.Index = 2;
         this.mnuCutAll.Text = "Cut &All Rules";
         this.mnuCutAll.Click += new System.EventHandler(this.mnuCopyRules_Click);
         // 
         // mnuPasteRules
         // 
         this.mnuPasteRules.Enabled = false;
         this.mnuPasteRules.Index = 6;
         this.mnuPasteRules.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuPasteAbove,
            this.mnuPasteBelow});
         this.mnuPasteRules.Text = "&Paste Rules";
         // 
         // mnuPasteAbove
         // 
         this.mnuPasteAbove.Index = 0;
         this.mnuPasteAbove.Text = "Paste &Above Selected Rule";
         this.mnuPasteAbove.Click += new System.EventHandler(this.mnuPasteRules_Click);
         // 
         // mnuPasteBelow
         // 
         this.mnuPasteBelow.Index = 1;
         this.mnuPasteBelow.Text = "Paste &Below Selected Rule";
         this.mnuPasteBelow.Click += new System.EventHandler(this.mnuPasteRules_Click);
         // 
         // mnuToggleSuspend
         // 
         this.mnuToggleSuspend.Index = 7;
         this.mnuToggleSuspend.Text = "Toggle &Suspend for This and Children";
         this.mnuToggleSuspend.Click += new System.EventHandler(this.OnToggleSuspend);
         // 
         // mnuConvertToFunc
         // 
         this.mnuConvertToFunc.Index = 8;
         this.mnuConvertToFunc.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuConvertAll,
            this.mnuConvertSelected});
         this.mnuConvertToFunc.Text = "Convert to &Function";
         // 
         // mnuConvertAll
         // 
         this.mnuConvertAll.Index = 0;
         this.mnuConvertAll.Text = "Convert &All Rules";
         this.mnuConvertAll.Click += new System.EventHandler(this.mnuConvertToFunc_Click);
         // 
         // mnuConvertSelected
         // 
         this.mnuConvertSelected.Index = 1;
         this.mnuConvertSelected.Text = "Convert &Selected Rule and Children";
         this.mnuConvertSelected.Click += new System.EventHandler(this.mnuConvertToFunc_Click);
         // 
         // tmrPopulate
         // 
         this.tmrPopulate.Tick += new System.EventHandler(this.tmrPopulate_Tick);
         // 
         // frmPlanEdit
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(533, 409);
         this.Controls.Add(this.grpRules);
         this.Controls.Add(this.pnlName);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Menu = this.mnuMain;
         this.Name = "frmPlanEdit";
         this.Text = "Edit Plan";
         this.grpRules.ResumeLayout(false);
         this.grpRules.PerformLayout();
         this.pnlRule.ResumeLayout(false);
         this.pnlRule.PerformLayout();
         this.pnlName.ResumeLayout(false);
         this.pnlName.PerformLayout();
         this.ResumeLayout(false);

      }
      #endregion

      #region Private Methods
      private void LoadFunctions(bool onlyBools, bool forceRefresh, bool updateUI)
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

            RemotingServices.IRemoteTypeInfo reflector;
            try
            {
               reflector = CodeGenerator.CreateInstanceAndUnwrap(
                   "RemoteReflector", CodeGenerator.NameToMapClass(m_Plan.MapName) + "+" +
                   CodeGenerator.NameToVariable(m_Plan.LayerName) + "_Lyr+" +
                   CodeGenerator.NameToVariable(m_Plan.Name)) as RemotingServices.IRemoteTypeInfo;
            }
            catch (System.Exception)
            {
               reflector = CodeGenerator.CreateInstanceAndUnwrap("RemoteReflector", "PlanBase")
                  as RemotingServices.IRemoteTypeInfo;
            }

            RemotingServices.RemoteMethodInfo[] localRuleList = reflector.GetMethods();
            RemotingServices.RemoteMethodInfo[] globalRuleList = reflector.GetGlobalFunctions();
            RemotingServices.RemoteMethodInfo[] ruleList = new SGDK2.RemotingServices.RemoteMethodInfo[localRuleList.Length + globalRuleList.Length];
            localRuleList.CopyTo(ruleList, 0);
            globalRuleList.CopyTo(ruleList, localRuleList.Length);

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
                     if (string.Compare(allowedType, mi.ReturnType.Name) == 0)
                     {
                        m_AvailableRules[mi.MethodName] = mi;
                        break;
                     }
                  }
               }
            }

            m_AvailableRules.InsertOperators();
         }

         if (updateUI)
         {
            cboFunction.Items.Clear();
            RemotingServices.RemoteMethodInfo[] rules = new SGDK2.RemotingServices.RemoteMethodInfo[m_AvailableRules.Count];
            m_AvailableRules.Rules.CopyTo(rules, 0);
            System.Array.Sort(rules, new RemotingServices.RemoteMethodComparer());
            foreach (RemotingServices.RemoteMethodInfo mi in rules)
            {
               if ((string.Compare(mi.ReturnType.Name, typeof(Boolean).Name) == 0) || !onlyBools)
                  cboFunction.Items.Add(mi.MethodName);
            }
            cboFunction.Items.Add(SpriteFunctionItem);
            if (!onlyBools)
               chkNot.Checked = false;
            chkNot.Enabled = onlyBools;

            PopulateParameter(lblParam1, cboParam1, RemotingServices.RemoteParameterInfo.Empty, true);
            PopulateParameter(lblParam2, cboParam2, RemotingServices.RemoteParameterInfo.Empty, true);
            PopulateParameter(lblParam3, cboParam3, RemotingServices.RemoteParameterInfo.Empty, true);
            m_PreparedFunction = String.Empty;
            lblOutput.Enabled = false;
            cboOutput.Enabled = false;
            cboOutput.SelectedIndex = -1;
         }
      }

      private void LoadCustomObjectsProviding(string TypeName)
      {
         if ((m_CustomObjects != null) && (m_CustomObjects.ContainsKey(TypeName)))
            return;
         if (m_CustomObjects == null)
            m_CustomObjects = new Hashtable();

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
            "RemoteReflector", TypeName) as RemotingServices.IRemoteTypeInfo;

         m_CustomObjects[TypeName] = reflector.GetGlobalProvidersOfSelf();
      }

      private bool ParseSpriteFunction(string funcName, out RemotingServices.RemoteMethodInfo methodInfo)
      {
         System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(
            CodeGenerator.SpritePlanParentField + @"\." + @"m_(\w+)\.(\w+)");
         System.Text.RegularExpressions.Match match = re.Match(funcName);
         if ((match != null) && (match.Success))
         {
            string spriteName = match.Groups[1].Captures[0].Value.Replace("_", " ");
            string function = match.Groups[2].Captures[0].Value;
            ProjectDataset.SpriteRow drSprite = ProjectData.GetSprite(m_Plan.LayerRowParent, spriteName);
            if (drSprite != null)
            {
               string errs;
               CodeGenerator gen = new CodeGenerator();
               gen.GenerateLevel = CodeGenerator.CodeLevel.ExcludeRules;
               errs = gen.CompileTempAssembly(false);
               if (errs != null)
               {
                  txtErrors.Text = errs;
                  txtErrors.Visible = true;
                  methodInfo = new RemotingServices.RemoteMethodInfo();
                  return false;
               }

               txtErrors.Visible = false;

               RemotingServices.IRemoteTypeInfo reflector = CodeGenerator.CreateInstanceAndUnwrap(
               "RemoteReflector", CodeGenerator.SpritesNamespace + "." + 
               CodeGenerator.NameToVariable(drSprite.DefinitionName)) as RemotingServices.IRemoteTypeInfo;
               RemotingServices.RemoteMethodInfo[] mi = reflector.GetMethods();
               for (int i = 0; i < mi.Length; i++)
               {
                  if (string.Compare(mi[i].MethodName, function, false) == 0)
                  {
                     methodInfo = mi[i];
                     return true;
                  }
               }
            }
         }
         methodInfo = new RemotingServices.RemoteMethodInfo();
         return false;
      }

      private void PrepareFunction(string funcName)
      {
         if (m_PreparedFunction == funcName)
            return;

         m_SpriteContext = null;

         RemotingServices.RemoteMethodInfo mi;

         if ((m_AvailableRules == null) || !m_AvailableRules.Contains(funcName))
         {
            bool isSpriteFunction;
            try
            {
               isSpriteFunction = ParseSpriteFunction(funcName, out mi);
            }
            catch (System.Exception ex)
            {
               MessageBox.Show(this, "Error parsing function as sprite function: " + ex.Message, "Select Function", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               isSpriteFunction = false;
               mi = new RemotingServices.RemoteMethodInfo(); // Avoid compiler error
            }

            if (!isSpriteFunction)
            {
               PopulateParameter(lblParam1, cboParam1, RemotingServices.RemoteParameterInfo.Unknown, true);
               PopulateParameter(lblParam2, cboParam2, RemotingServices.RemoteParameterInfo.Unknown, true);
               PopulateParameter(lblParam3, cboParam3, RemotingServices.RemoteParameterInfo.Unknown, true);
               lblOutput.Enabled = true;
               cboOutput.Enabled = true;
               txtHelpText.Text = "The specified function name could not be located or the project failed to compile.";
               m_PreparedFunction = string.Empty;
               return;
            }
         }
         else
            mi = m_AvailableRules[funcName];

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

         if ((string.Compare(mi.ReturnType.Name, typeof(Int32).Name) == 0) ||
            (string.Compare(mi.ReturnType.Name, typeof(Int16).Name) == 0))
         {
            lblOutput.Enabled = true;
            cboOutput.Enabled = true;
            cboOutput.Items.Clear();
            FillComboWithParams(cboOutput, false);
            FillComboWithIntVars(cboOutput, false);
         }
         else
         {
            lblOutput.Enabled = false;
            cboOutput.Enabled = false;
            cboOutput.Text = String.Empty;
            cboOutput.SelectedIndex = -1;
         }
         m_PreparedFunction = funcName;
      }

      private void FillComboWithIntVars(ComboBox cboTarget, bool isRef)
      {
         foreach (DataRowView drv in ProjectData.Counter.DefaultView)
            cboTarget.Items.Add(CodeGenerator.CounterClass + "." + CodeGenerator.NameToVariable(
               ((ProjectDataset.CounterRow)drv.Row).Name) + ".CurrentValue");
         foreach (RemotingServices.RemotePropertyInfo pi in m_PlanProperties)
         {
            if (pi.Type.Name == typeof(Int32).Name)
               cboTarget.Items.Add((isRef ? "ref " : "") + CodeGenerator.NameToVariable(pi.Name));
         }
      }

      private void FillComboWithCounters(ComboBox cboTarget)
      {
         foreach (DataRowView drv in ProjectData.Counter.DefaultView)
            cboTarget.Items.Add(CodeGenerator.CounterClass + "." + CodeGenerator.NameToVariable(
               ((ProjectDataset.CounterRow)drv.Row).Name));
      }

      private void FillComboWithTilesets(ComboBox cboTarget)
      {
         foreach (DataRowView drv in ProjectData.Tileset.DefaultView)
         {
            cboTarget.Items.Add(CodeGenerator.TilesetClass + "." +
               CodeGenerator.NameToVariable(
               ((ProjectDataset.TilesetRow)drv.Row).Name));
         }
      }

      private void FillComboWithParams(ComboBox cboParams, bool isRef)
      {
         if (m_SpriteContext != null)
         {
            ProjectDataset.SpriteDefinitionRow spriteDef = m_SpriteContext.SpriteRow.SpriteStateRowParent.SpriteDefinitionRow;
            foreach(ProjectDataset.SpriteParameterRow prow in ProjectData.GetSortedSpriteParameters(spriteDef, true))
            {
               cboParams.Items.Add((isRef ? "ref " : "") + m_SpriteContext.ToString() + "." + CodeGenerator.NameToVariable(prow.Name));
            }
         }
         if (isRef)
            cboParams.Items.Add(SelectWritableSpriteParameterItem);
         else
            cboParams.Items.Add(SelectSpriteParameterItem);
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

      private void FillComboWithSpriteDefinitions(ComboBox cboTarget)
      {
         foreach(System.Data.DataRowView drv in ProjectData.SpriteDefinition.DefaultView)
         {
            ProjectDataset.SpriteDefinitionRow drSpriteDef = (ProjectDataset.SpriteDefinitionRow)drv.Row;
            cboTarget.Items.Add("typeof(Sprites." + CodeGenerator.NameToVariable(drSpriteDef.Name) + ")");
         }
      }

      private void FillComboWithMapTypes(ComboBox cboTarget)
      {
         foreach(DataRowView drv in ProjectData.Map.DefaultView)
            cboTarget.Items.Add("typeof(" + CodeGenerator.NameToMapClass(((ProjectDataset.MapRow)drv.Row).Name) + ")");
      }

      private void FillComboWithCustomObjects(ComboBox cboTarget, RemotingServices.RemoteParameterInfo param)
      {
         LoadCustomObjectsProviding(param.Type.FullName);
         if (m_CustomObjects.ContainsKey(param.Type.FullName))
         {
            foreach(RemotingServices.RemoteGlobalAccessorInfo p in (RemotingServices.RemoteGlobalAccessorInfo[])m_CustomObjects[param.Type.FullName])
               cboTarget.Items.Add(p.Type.FullName + "." + p.MemberName);
         }
      }

      private void FillComboWithSpriteInstances(ComboBox cboTarget)
      {
         cboTarget.Items.Add("lastCreatedSprite");
         foreach(ProjectDataset.SpriteRow drSprite in ProjectData.GetSortedSpriteRows(m_Plan.LayerRowParent, true))
         {
            cboTarget.Items.Add(new SpriteCodeRef(drSprite));
         }
         foreach (RemotingServices.RemotePropertyInfo pi in m_PlanProperties)
            if (pi.Type.Name == CodeGenerator.SpriteBaseClass)
               cboTarget.Items.Add(CodeGenerator.NameToVariable(pi.Name));
      }

      private void FillComboWithSpriteCollections(ComboBox cboTarget)
      {
         foreach(DataRowView drv in ProjectData.SpriteCategory.DefaultView)
         {
            cboTarget.Items.Add("ParentLayer." + CodeGenerator.SpriteCategoriesFieldName + "." + CodeGenerator.NameToVariable(
               ((ProjectDataset.SpriteCategoryRow)drv.Row).Name));
         }
      }

      private void FillComboWithPlans(ComboBox cboTarget)
      {
         foreach(ProjectDataset.LayerRow drLayer in m_Plan.LayerRowParent.MapRow.GetLayerRows())
         {
            foreach (ProjectDataset.SpritePlanRow drPlan in ProjectData.GetSortedSpritePlans(drLayer, true))
            {
               if (drLayer == m_Plan.LayerRowParent)
                  cboTarget.Items.Add("m_ParentLayer.m_" + CodeGenerator.NameToVariable(drPlan.Name));
               else
                  cboTarget.Items.Add("m_ParentLayer.m_ParentMap.m_" + CodeGenerator.NameToVariable(drLayer.Name) + ".m_" + CodeGenerator.NameToVariable(drPlan.Name));
            }
         }
      }

      public void PopulateParameter(Label lblParameter, ComboBox cboParameter, RemotingServices.RemoteParameterInfo param, bool clearValue)
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
            EnumTable.EnumDetails enumVals;
            if (m_Enums.Contains(param.Type.FullName))
               enumVals = m_Enums[param.Type.FullName];
            else
               enumVals = m_Enums[param.Type.FullName] = GetEnumInfo(param.Type.FullName);

            foreach (string enumVal in enumVals.names)
               cboParameter.Items.Add(enumVal);

            if (enumVals.isFlags)
               cboParameter.Items.Add(new EnumOptionSelector(param.Type.Name, enumVals));
            
            return;
         }

         if (string.Compare(param.Type.FullName, typeof(bool).FullName)==0)
         {
            cboParameter.Items.Add("false");
            cboParameter.Items.Add("true");
         }
         else if (string.Compare(param.Type.FullName, typeof(System.Drawing.Point).FullName) == 0)
         {
            foreach(ProjectDataset.SpritePlanRow drPlan in ProjectData.GetSortedSpritePlans(m_Plan.LayerRowParent, true))
            {
               ProjectDataset.CoordinateRow[] drCoords = ProjectData.GetSortedCoordinates(drPlan);
               if (drCoords.Length == 1)
                  cboParameter.Items.Add(CodeGenerator.SpritePlanParentField + ".m_" + CodeGenerator.NameToVariable(drPlan.Name) + "[0]");
            }
            cboParameter.Items.Add(CodeGenerator.ParentLayerProperty + ".GetMousePosition()");
         }
         else if (string.Compare(param.Type.Name, CodeGenerator.SpriteBaseClass) == 0)
         {
            FillComboWithSpriteInstances(cboParameter);
         }
         else if (string.Compare(param.Type.Name, CodeGenerator.SpriteCollectionClassName) == 0)
         {
            FillComboWithSpriteCollections(cboParameter);
         }
         else if (string.Compare(param.Type.Name, CodeGenerator.PlanBaseClassName) == 0)
         {
            FillComboWithPlans(cboParameter);
         }
         else if (param.Editors != null)
         {
            foreach(string editor in param.Editors)
            {
               switch(editor)
               {
                  case "SpriteState":
                     if (m_SpriteContext != null)
                        FillComboWithSpriteStates(cboParameter);
                     break;
                  case "MapType":
                     FillComboWithMapTypes(cboParameter);
                     break;
                  case "CustomObject":
                     FillComboWithCustomObjects(cboParameter, param);
                     break;
                  case "SpriteDefinition":
                     FillComboWithSpriteDefinitions(cboParameter);
                     break;
                  case "Message":
                     cboParameter.Items.Add(frmEditMessage.messageEditorItem);
                     break;
               }
            }
         }
         else if (string.Compare(param.Type.Name, typeof(Int32).Name + "&") == 0)
         {
            // Integer passed by reference
            FillComboWithParams(cboParameter, true);
            FillComboWithIntVars(cboParameter, true);
         }
         else if (string.Compare(param.Type.Name, "Counter") == 0)
         {
            FillComboWithCounters(cboParameter);
         }
         else if (string.Compare(param.Type.Name, "Tileset") == 0)
         {
            FillComboWithTilesets(cboParameter);
         }
         else if (param.Type.FullName.StartsWith("Sprites."))
         {
            foreach (ProjectDataset.SpriteRow drSprite in ProjectData.GetSortedSpriteRows(m_Plan.LayerRowParent, true))
               if (param.Type.FullName.EndsWith("." + drSprite.DefinitionName))
                  cboParameter.Items.Add(new SpriteCodeRef(drSprite));
         }
         else
         {
            foreach (string typeName in new string[]
            {
               typeof(Int32).Name, typeof(Int16).Name,
               typeof(Double).Name, typeof(Single).Name
            })
            {
               if (string.Compare(typeName, param.Type.Name) == 0)
               {
                  FillComboWithParams(cboParameter, false);
                  FillComboWithIntVars(cboParameter, false);
                  break;
               }
            }
         }
      }

      private EnumTable.EnumDetails GetEnumInfo(string enumName)
      {
         string errs;
         CodeGenerator gen = new CodeGenerator();
         gen.GenerateLevel = CodeGenerator.CodeLevel.ExcludeRules;
         errs = gen.CompileTempAssembly(false);
         if (errs != null)
         {
            txtErrors.Text = errs;
            txtErrors.Visible = true;
            return new EnumTable.EnumDetails();
         }

         txtErrors.Visible = false;
         
         try
         {
            RemotingServices.IRemoteTypeInfo reflector = CodeGenerator.CreateInstanceAndUnwrap(
               "RemoteReflector", enumName) as RemotingServices.IRemoteTypeInfo;

            EnumTable.EnumDetails remoteResults = new EnumTable.EnumDetails();
            remoteResults.names = reflector.GetEnumVals();
            remoteResults.isFlags = reflector.IsFlags;

            return remoteResults;
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(this, ex.ToString(), "GetEnumInfo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return new EnumTable.EnumDetails();
         }
      }
      
      private void LoadRule(ProjectDataset.PlanRuleRow drRule)
      {
         try
         {
            m_Loading = true;
            txtRuleName.Text = drRule.Name;

            LoadFunctions(IsRuleTypeConditional(drRule.Type), false, true);


            if (!string.IsNullOrEmpty(drRule.Function))
            {
               string funcName = drRule.Function;
               bool invert = false;

               cboRuleType.Text = drRule.Type;

               int selIdx = cboFunction.FindStringExact(funcName);
               if (selIdx < 0)
               {
                  if (funcName.StartsWith("!"))
                  {
                     funcName = funcName.Substring(1);
                     invert = true;
                  }
                  selIdx = cboFunction.FindStringExact(funcName);
               }
               if (selIdx >= 0)
                  cboFunction.SelectedIndex = selIdx;
               else
                  cboFunction.SelectedIndex = cboFunction.Items.Add(funcName);

               chkNot.Checked = invert;

               PrepareFunction(funcName);

               LoadParameters(drRule);
            }


            chkEndIf.Checked = drRule.EndIf;
            chkSuspended.Checked = drRule.Suspended;
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
      
      private void LoadParameters(ProjectDataset.PlanRuleRow drRule)
      {
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
      }

      private void PopulateBaseClasses()
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
            "RemoteReflector", CodeGenerator.PlanBaseClassName) as RemotingServices.IRemoteTypeInfo;
         cboBaseClass.Items.Clear();
         cboBaseClass.Items.Add(CodeGenerator.PlanBaseClassName);
         RemotingServices.RemoteTypeName[] bases = reflector.GetDerivedClasses(true);
         for(int i = 0; i < bases.Length; i++)
            cboBaseClass.Items.Add(bases[i].FullName);
      }

      private void QueuePopulateRules()
      {
         tmrPopulate.Stop();
         tmrPopulate.Start();
      }

      private void PopulateRules()
      {
         ProjectDataset.PlanRuleRow cur = CurrentRule;
         tvwRules.Nodes.Clear();
         m_TreeNodes.Clear();
         TreeNode parentNode = null;
         foreach(ProjectDataset.PlanRuleRow drRule in ProjectData.GetSortedPlanRules(m_Plan, true))
         {
            if (parentNode == null)
            {
               parentNode = tvwRules.Nodes.Add(drRule.Name);
               parentNode.ImageIndex = parentNode.SelectedImageIndex = GetRuleImage(drRule);
               m_TreeNodes[drRule.Name] = parentNode;
               if (!DoesRuleTypeNest(drRule.Type))
                  parentNode = null;
               continue;
            }
            TreeNode curNode = parentNode.Nodes.Add(drRule.Name);
            curNode.ImageIndex = curNode.SelectedImageIndex = GetRuleImage(drRule);
            m_TreeNodes[drRule.Name] = curNode;
            if (DoesRuleTypeNest(drRule.Type))
            {
               parentNode = curNode;
               continue;
            }
            if (drRule.EndIf)
               parentNode = parentNode.Parent;
         }

         if (m_SelectAfterLoad != null)
         {
            tvwRules.SelectedNode = GetNodeFromRow(m_SelectAfterLoad);
            m_SelectAfterLoad = null;
         }
         else if (cur != null)
            tvwRules.SelectedNode = GetNodeFromRow(cur);
      }

      private int GetRuleImage(ProjectDataset.PlanRuleRow drPlan)
      {
         int result;

         switch(drPlan.Type)
         {
            case "Do":
               result = 0;
               break;
            case "If":
               result = 1;
               break;
            case "Else":
               result = 2;
               break;
            case "End":
               result = 3;
               break;
            case "ElseIf":
               result = 4;
               break;
            case "While":
               result = 5;
               break;
            case "And":
               result = 6;
               break;
            case "Or":
               result = 7;
               break;
            default:
               return -1;
         }

         if (drPlan.Suspended)
            return result + 8;
         else
            return result;
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
               chkEndIf.Enabled = chkSuspended.Enabled = false;
            return;
         }

         txtRuleName.Enabled = lblRuleName.Enabled =
            cboRuleType.Enabled = chkSuspended.Enabled = true;

         if (String.Compare(cboRuleType.Text, "End", true) == 0)
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
            (String.Compare(ruleType, "ElseIf", true) == 0) ||
            (String.Compare(ruleType, "While", true) == 0);
      }

      private void DetectSpriteContext(int argIdx, string fldValue, ComboBox cboSource)
      {
         if (m_AvailableRules == null)
            return;

         if (!m_AvailableRules.Contains(cboFunction.Text))
            return;

         RemotingServices.RemoteMethodInfo rmi = m_AvailableRules[cboFunction.Text];
         if ((argIdx >= 0) && (rmi.Arguments[argIdx].Type.Name == CodeGenerator.SpriteBaseClass) && (cboSource.SelectedIndex >= 0) && (cboSource.Items[cboSource.SelectedIndex] is SpriteCodeRef))
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
               if ((string.Compare(rmi.ReturnType.Name, typeof(Int32).Name) == 0) ||
                  (string.Compare(rmi.ReturnType.Name, typeof(Int16).Name) == 0))
               {
                  cboOutput.Items.Clear();
                  FillComboWithParams(cboOutput, false);
                  FillComboWithIntVars(cboOutput, false);
               }
            }
         }
      }

      private void EnablePasteRules()
      {
         mnuPasteRules.Enabled = Clipboard.GetDataObject().GetDataPresent(typeof(ProjectData.CopiedRule[]));
      }

      private void CopyAllRules()
      {
         System.Collections.ArrayList result = new ArrayList();

         foreach(TreeNode rule in tvwRules.Nodes)
         {
            result.AddRange(GetNodeWithChildList(rule));
         }
         Clipboard.SetDataObject((ProjectData.CopiedRule[])result.ToArray(typeof(ProjectData.CopiedRule)));
      }

      private void CopyRules(bool includeChildren)
      {
         ProjectDataset.PlanRuleRow drRule = CurrentRule;
         if (drRule == null)
         {
            MessageBox.Show(this, "Select a rule before selecting this command.", "Copy Rules", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         if (!includeChildren)
         {
            Clipboard.SetDataObject(new ProjectData.CopiedRule[] {new ProjectData.CopiedRule(drRule)});
            return;
         }
         Clipboard.SetDataObject(GetNodeWithChildList(tvwRules.SelectedNode));
      }

      private ProjectData.CopiedRule[] GetNodeWithChildList(TreeNode parent)
      {
         System.Collections.ArrayList result = new ArrayList();
         result.Add(new ProjectData.CopiedRule(ProjectData.GetPlanRule(m_Plan, parent.Text)));
         foreach(TreeNode child in parent.Nodes)
            result.AddRange(GetNodeWithChildList(child));
         return (ProjectData.CopiedRule[])result.ToArray(typeof(ProjectData.CopiedRule));
      }

      private void ConvertAllRules()
      {
         System.Collections.ArrayList nodes = new ArrayList();

         foreach (TreeNode rule in tvwRules.Nodes)
         {
            nodes.AddRange(GetNodeWithChildList(rule));
         }
         ConvertRules((ProjectData.CopiedRule[])nodes.ToArray(typeof(ProjectData.CopiedRule)));
      }

      private void ConvertRuleAndChildren()
      {
         if (tvwRules.SelectedNode == null)
         {
            MessageBox.Show(this, "Select a rule before selecting this command.", "Convert Rules to Function", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }

         ConvertRules(GetNodeWithChildList(tvwRules.SelectedNode));
      }

      private void ConvertRules(ProjectData.CopiedRule[] copiedRules)
      {
         ProjectDataset.PlanRuleRow[] rules;
         ArrayList selectedRules = new ArrayList();
         foreach (ProjectData.CopiedRule rule in copiedRules)
         {
            if (!rule.Suspended)
               selectedRules.Add(ProjectData.GetPlanRule(m_Plan, rule.Name));
         }
         rules = (ProjectDataset.PlanRuleRow[])selectedRules.ToArray(typeof(ProjectDataset.PlanRuleRow));

         System.Collections.Specialized.StringCollection reservedNames = new System.Collections.Specialized.StringCollection();
         try
         {
            LoadFunctions(false, false, false);
            foreach (DictionaryEntry de in m_AvailableRules)
            {
               reservedNames.Add(de.Key.ToString());
            }
            foreach (RemotingServices.RemotePropertyInfo pi in m_PlanProperties)
            {
               reservedNames.Add(pi.Name);
            }
            reservedNames.Add(CodeGenerator.NameToVariable(m_Plan.Name));
            reservedNames.Add(CodeGenerator.NameToVariable(m_Plan.LayerName + "_Lyr"));
            reservedNames.Add(CodeGenerator.NameToMapClass(m_Plan.MapName));
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(this, "Error inspecting compiled project for list of reserved names (" + ex.Message + ")", "Convert Rules to Function", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
         reservedNames.Add("ExecuteRules");
         using (frmConvertToFunction frm = new frmConvertToFunction(rules, reservedNames))
         {
            frm.ShowDialog(this);
         }
         try
         {
            LoadFunctions(false, true, false);
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Convert Rules to Function", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
      }

      private void DeleteAllRules()
      {
         foreach (ProjectDataset.PlanRuleRow drRule in ProjectData.GetSortedPlanRules(m_Plan, true))
         {
            drRule.Delete();
         }
      }

      private void DeleteRules(bool includeChildren)
      {
         ProjectDataset.PlanRuleRow drRule = CurrentRule;
         if (drRule == null)
         {
            MessageBox.Show(this, "Select a rule before selecting this command.", "Delete Rules", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         TreeNode selAfterDel = tvwRules.SelectedNode.NextNode;
         if (!includeChildren && tvwRules.SelectedNode.Nodes.Count > 0)
            selAfterDel = tvwRules.SelectedNode.Nodes[0];
         if (selAfterDel == null)
            selAfterDel = tvwRules.SelectedNode.PrevNode;
         if (selAfterDel == null)
            selAfterDel = tvwRules.SelectedNode.Parent;
         if (selAfterDel != null)
            m_SelectAfterLoad = ProjectData.GetPlanRule(m_Plan, selAfterDel.Text);
         if (includeChildren)
            DeleteRuleAndChildren(tvwRules.SelectedNode);
         else
            ProjectData.DeletePlanRule(drRule);
      }

      private void DeleteRuleAndChildren(TreeNode parent)
      {
         foreach (TreeNode child in parent.Nodes)
            DeleteRuleAndChildren(child);
         ProjectData.DeletePlanRule(ProjectData.GetPlanRule(m_Plan, parent.Text));
      }

      private void PasteRules(bool after)
      {
         if (Clipboard.GetDataObject().GetDataPresent(typeof(ProjectData.CopiedRule[])))
         {
            int sequence;
            if (CurrentRule == null)
            {
               sequence = -1;
            }
            else
            {
               sequence = CurrentRule.Sequence;
               if (after)
                  sequence++;
            }
            ProjectData.CopiedRule[] toPaste = (ProjectData.CopiedRule[])Clipboard.GetDataObject().GetData(typeof(ProjectData.CopiedRule[]));
            if ((toPaste.Length > 0) && (!toPaste[0].IsPlanRule))
            {
               if (DialogResult.OK != MessageBox.Show(this, "The rules being pasted were copied from a sprite definition. Sprite definition rules may not be compatible with plan rules, and may require corrections before they can function as such.", "Paste Rules", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                  return;
            }
            foreach(ProjectData.CopiedRule rule in toPaste)
            {
               string name = rule.Name;
               int i = 0;
               while (ProjectData.GetPlanRule(m_Plan, name) != null)
               {
                  name = rule.Name + (++i).ToString();
               }
               ProjectData.InsertPlanRule(m_Plan, name, rule.Type, sequence, rule.Function, rule.Parameter1, rule.Parameter2, rule.Parameter3, rule.ResultParameter, rule.EndIf, rule.Suspended);
               if (sequence >= 0)
                  sequence++;
            }
         }
         else
            EnablePasteRules();
      }
      #endregion

      #region Public Static Members
      public static void Edit(Form MdiParent, ProjectDataset.SpritePlanRow EditRow)
      {
         foreach(Form frm in MdiParent.MdiChildren)
         {
            frmPlanEdit f = frm as frmPlanEdit;
            if (f != null)
            {
               if (f.m_Plan == EditRow)
               {
                  f.Activate();
                  return;
               }
            }
         }

         frmPlanEdit frmNew = new frmPlanEdit(EditRow);
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
      protected override void OnActivated(EventArgs e)
      {
         base.OnActivated (e);
         EnablePasteRules();
      }

      #endregion

      #region Events Handlers
      private void txtName_Validated(object sender, System.EventArgs e)
      {
         if (m_Loading)
            return;
         m_Plan.Name = txtName.Text;
      }

      private void cboBaseClass_Validated(object sender, EventArgs e)
      {
         if (m_Loading)
            return;
         m_Plan.BaseClass = cboBaseClass.Text;
         CodeGenerator.ResetTempAssembly();
         m_AvailableRules = null;
         m_Enums = null;
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
         ProjectDataset.SpritePlanRow drPlan = ProjectData.GetSpritePlan(m_Plan.LayerRowParent, txtName.Text);
         if ((null != drPlan) && (m_Plan != drPlan))
         {
            if (DialogResult.Cancel == MessageBox.Show(this, txtName.Text + " already exists", "Plan Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtName.Text = m_Plan.Name;
            e.Cancel = true;
         }

         ProjectDataset.SpriteRow drSprite = ProjectData.GetSprite(m_Plan.LayerRowParent, txtName.Text);
         if (null != drSprite)
         {
            if (DialogResult.Cancel == MessageBox.Show(this, txtName.Text + " conflicts with a sprite by the same name", "Plan Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
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
         if (String.Compare(cboRuleType.Text, "End", true) == 0)
            EnableFields();
         else
            LoadFunctions(IsRuleTypeConditional(cboRuleType.SelectedItem.ToString()), false, true);
         if (CurrentRule != null)
         {
            CurrentRule.Type = cboRuleType.Text;
            if (String.Compare(cboRuleType.Text, "End", true) == 0)
            {
               CurrentRule.Function = cboRuleType.Text;
               CurrentRule.EndIf = true;
            }
         }
      }

      private void cboFunction_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if ((cboFunction.SelectedItem is string) &&
            (string.Compare(((string)cboFunction.SelectedItem), SpriteFunctionItem) == 0))
         {
            if (CurrentRule == null)
            {
               MessageBox.Show(this, "You must select a rule first.", "Select Sprite Function", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return;
            }
            using (frmSpriteFunctionSelector frm = new frmSpriteFunctionSelector(m_Plan.LayerRowParent, CurrentRule, this))
            {
               if (DialogResult.OK == frm.ShowDialog(this))
                  LoadRule(CurrentRule);
               else
                  cboFunction.SelectedIndex = -1;
            }
         }
         else
         {
            EnableFields();
            if (cboFunction.SelectedItem == null)
               PrepareFunction(string.Empty);
            else
               PrepareFunction(cboFunction.SelectedItem.ToString());
            if (CurrentRule != null)
               LoadParameters(CurrentRule);
         }
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

         if (CurrentRule != null)
            LoadParameters(CurrentRule);
      }

      private void tbRules_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
      {
         if (e.Button == tbbNewRule)
            OnAddRule(this, e);
         else if (e.Button == tbbDeleteRule)
            OnDeleteRule(this, e);
         else if (e.Button == tbbMoveUp)
            OnMoveRuleUp(this, e);
         else if (e.Button == tbbMoveDown)
            OnMoveRuleDown(this, e);
         else if (e.Button == tbbToggleSuspend)
            OnToggleSuspend(this, e);
         else if (e.Button == tbbConvertToFunc)
            ConvertRuleAndChildren();
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
                  QueuePopulateRules();
               break;
            case DataRowAction.Change:
               if ((e.Row.SpritePlanRowParent == m_Plan) && (m_OldRuleName != null))
               {
                  if ((String.Compare(m_OldRuleName, e.Row.Name) != 0))
                     tvwRules.SelectedNode.Text = e.Row.Name;
                  else if ((m_OldSequence != e.Row.Sequence) ||
                     (String.Compare(m_OldType,e.Row.Type) != 0) ||
                     (m_OldEndIf != e.Row.EndIf))
                     QueuePopulateRules();
               }
               break;
            case DataRowAction.Delete:
               if (m_OldRuleName != null)
                  QueuePopulateRules();
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
         if (CurrentRule != null)
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

      private void chkSuspended_CheckedChanged(object sender, System.EventArgs e)
      {
         if (m_Loading)
            return;
         if (CurrentRule != null)
         {
            CurrentRule.Suspended = chkSuspended.Checked;
            tvwRules.SelectedNode.ImageIndex =
               tvwRules.SelectedNode.SelectedImageIndex = GetRuleImage(CurrentRule);
         }
      }

      private void OnMoveRuleUp(object sender, System.EventArgs e)
      {
         if (!Validate())
            return;
         if (tvwRules.SelectedNode == null)
         {
            MessageBox.Show(this, "Select a rule first.", "Move Rule Up", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         ProjectData.MovePlanRule(CurrentRule, false);
      }

      private void OnMoveRuleDown(object sender, System.EventArgs e)
      {
         if (!Validate())
            return;
         if (tvwRules.SelectedNode == null)
         {
            MessageBox.Show(this, "Select a rule first.", "Move Rule Down", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         ProjectData.MovePlanRule(CurrentRule, true);      
      }

      private void OnToggleSuspend(object sender, System.EventArgs e)
      {
         if (!Validate())
            return;
         if (tvwRules.SelectedNode == null)
         {
            MessageBox.Show(this, "Select a rule first.", "Toggle Suspend for This and Children", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         ToggleSuspend(tvwRules.SelectedNode, !CurrentRule.Suspended);
         LoadRule(CurrentRule);
      }

      private void ToggleSuspend(TreeNode node, bool suspend)
      {
         ProjectDataset.PlanRuleRow drPlan = ProjectData.GetPlanRule(m_Plan, node.Text);
         drPlan.Suspended = suspend;
         node.ImageIndex = node.SelectedImageIndex = GetRuleImage(drPlan);
         foreach(TreeNode child in node.Nodes)
            ToggleSuspend(child, suspend);
      }

      private void OnAddRule(object sender, System.EventArgs e)
      {
         if (!Validate())
            return;

         string newPlanName;
         int i = 0;
         do
         {
            newPlanName = "Rule " + (++i).ToString();
         } while (ProjectData.GetPlanRule(m_Plan, newPlanName) != null);
         int newSeq = -1;
         if (CurrentRule != null)
            newSeq = CurrentRule.Sequence + 1;
         m_SelectAfterLoad = ProjectData.InsertPlanRule(m_Plan, newPlanName, "Do", newSeq, cboFunction.Text, null, null, null, null, false, false);
      }

      private void OnDeleteRule(object sender, System.EventArgs e)
      {
         if (tvwRules.SelectedNode == null)
         {
            MessageBox.Show(this, "Select a rule first.", "Delete Rule", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         ProjectData.DeletePlanRule(CurrentRule);
      }

      private void cboParam_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         ComboBox source = (ComboBox)sender;
         if (source.SelectedIndex < 0)
            return;
         frmSelectSpriteParameter frm;
         if ((source.Items[source.SelectedIndex].Equals(SelectSpriteParameterItem)) ||
            (source.Items[source.SelectedIndex].Equals(SelectWritableSpriteParameterItem)))
         {
            bool isRef = (source.Items[source.SelectedIndex].Equals(SelectWritableSpriteParameterItem));
            using (frm = new frmSelectSpriteParameter(m_Plan.LayerRowParent))
            {
               if (DialogResult.OK == frm.ShowDialog(this))
               {
                  m_SpriteContext = new SpriteCodeRef(frm.SpriteRow);
                  string newSel = (isRef ? "ref ":"") + m_SpriteContext.ToString() + "." + CodeGenerator.NameToVariable(frm.SpriteParameterRow.Name);
                  int selIdx = source.FindStringExact(newSel);
                  if (selIdx >= 0)
                     source.SelectedIndex = selIdx;
                  else
                     source.SelectedIndex = source.Items.Add(newSel);
               }
               else
                  source.SelectedIndex = -1;
            }
         }
      }

      private void cboParam_SelectionChangeCommitted(object sender, EventArgs e)
      {
         ComboBox source = (ComboBox)sender;
         if ((source.SelectedIndex >= 0) && (source.Items[source.SelectedIndex] is EnumOptionSelector))
         {
            string oldText = source.Text;
            string newValue = frmSpecifyFlags.GetOptions(this, (EnumOptionSelector)source.Items[source.SelectedIndex], oldText);
            if (newValue == null)
            {
               int selIdx = source.FindStringExact(oldText);
               if (selIdx >= 0)
                  source.SelectedIndex = selIdx;
               else
                  source.SelectedIndex = source.Items.Add(oldText);
            }
            else
            {
               int selIdx = source.FindStringExact(newValue);
               if (selIdx >= 0)
                  source.SelectedIndex = selIdx;
               else
                  source.SelectedIndex = source.Items.Add(newValue);
            }
         }
         else if ((source.SelectedIndex >= 0) && (source.Items[source.SelectedIndex] is string)
            && (string.Compare((string)source.Items[source.SelectedIndex], frmEditMessage.messageEditorItem) == 0))
         {
            string oldText = source.Text;
            string newText = frmEditMessage.EditMessage(source.Text, this);
            if (newText == null)
               newText = oldText;
            int selIdx = source.FindStringExact(newText);
            if (selIdx >= 0)
               source.SelectedIndex = selIdx;
            else
               source.SelectedIndex = source.Items.Add(newText);
         }
      }

      private void txtPriority_Validated(object sender, System.EventArgs e)
      {
         if (m_Loading)
            return;
         m_Plan.Priority = int.Parse(txtPriority.Text);
      }

      private void txtPriority_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         if (m_Loading)
            return;
         double priority;
         if (!Double.TryParse(txtPriority.Text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentUICulture, out priority))
         {
            MessageBox.Show(this, "Priority must be an integer", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
         }
      }

      private void tmrPopulate_Tick(object sender, System.EventArgs e)
      {
         tmrPopulate.Stop();
         PopulateRules();
      }

      private void mnuPasteRules_Click(object sender, System.EventArgs e)
      {
         if (sender == mnuPasteAbove)
            PasteRules(false);
         else if (sender == mnuPasteBelow)
            PasteRules(true);
      }

      private void mnuCopyRules_Click(object sender, System.EventArgs e)
      {
         if ((sender == mnuCopyAll) || (sender == mnuCutAll))
            CopyAllRules();
         else if ((sender == mnuCopySelected) || (sender == mnuCutSelected))
            CopyRules(false);
         else if ((sender == mnuCopyChildren) || (sender == mnuCutChildren))
            CopyRules(true);

         if (sender == mnuCutAll)
            DeleteAllRules();
         else if (sender == mnuCutSelected)
            DeleteRules(false);
         else if (sender == mnuCutChildren)
            DeleteRules(true);

         EnablePasteRules();
      }

      private void mnuConvertToFunc_Click(object sender, EventArgs e)
      {
         if (sender == mnuConvertAll)
            ConvertAllRules();
         else
            ConvertRuleAndChildren();
      }
      #endregion
   }
}
