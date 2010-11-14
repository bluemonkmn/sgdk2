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
	/// Summary description for SpriteDefinition.
	/// </summary>
   public class frmSpriteDefinition : System.Windows.Forms.Form
   {
      #region Embedded Classes
      class SpriteFrame : IProvideFrame
      {
         #region IProvideFrame Members

         private ProjectDataset.SpriteFrameRow row;
         private bool selected;

         public SpriteFrame(ProjectDataset.SpriteFrameRow row)
         {
            this.row = row;
            this.selected = false;
         }

         public int FrameIndex
         {
            get
            {
               return row.FrameValue;
            }
         }

         public int[] FrameIndexes
         {
            get
            {
               return new int[] {row.FrameValue};
            }
         }

         public bool IsSelected
         {
            get
            {
               return selected;
            }
            set
            {
               selected = value;
            }
         }

         #endregion

         public ProjectDataset.SpriteFrameRow Row
         {
            get
            {
               return row;
            }
         }
      }
      #endregion

      #region Non-control members
      private ProjectDataset.SpriteDefinitionRow m_SpriteDef;
      private RuleTable m_AvailableRules = null;
      private RemotingServices.RemotePropertyInfo[] m_SpriteProperties = null;
      private EnumTable m_Enums = null;
      string m_OldRuleName = null;
      int m_OldSequence = -1;
      string m_OldType = null;
      bool m_OldEndIf = false;
      private Hashtable m_TreeNodes = new Hashtable();
      private bool m_Loading = false;
      private string m_PreparedFunction = string.Empty;
      private Hashtable m_CustomObjects = null; // .[TypeName] -> RemoteGlobalAccessorInfo[]
      private ProjectDataset.SpriteRuleRow m_SelectAfterLoad = null;
      #endregion

      #region Windows Forms Designer Components
      private System.Windows.Forms.TabControl tabSpriteDefinition;
      private System.Windows.Forms.TabPage tabStates;
      private System.Windows.Forms.TabPage tabParameters;
      private System.Windows.Forms.TabPage tabRules;
      private System.Windows.Forms.Panel pnlSpriteHeader;
      private System.Windows.Forms.Label lblName;
      private System.Windows.Forms.TextBox txtName;
      private System.Windows.Forms.Panel pnlSpriteState;
      private System.Windows.Forms.Label lblStateName;
      private System.Windows.Forms.TextBox txtStateName;
      private System.Windows.Forms.Label lblFrameset;
      private System.Windows.Forms.ComboBox cboFrameset;
      private System.Windows.Forms.Splitter StateTreeSplitter;
      private System.Windows.Forms.Panel pnlFrames;
      private System.Windows.Forms.Label lblStateFrames;
      private SGDK2.GraphicBrowser StateFrames;
      private System.Windows.Forms.Panel pnlFrameDetails;
      private System.Windows.Forms.NumericUpDown updRepeatCount;
      private System.Windows.Forms.Label lblRepeatCount;
      private SGDK2.GraphicBrowser AvailableFrames;
      private System.Windows.Forms.Label lblAvailableFrames;
      private System.Windows.Forms.MainMenu mnuSprites;
      private SGDK2.DataChangeNotifier DataMonitor;
      private System.Windows.Forms.MenuItem mnuSpriteDefinition;
      private System.Windows.Forms.MenuItem mnuAddState;
      private System.Windows.Forms.MenuItem mnuDeleteState;
      private System.Windows.Forms.MenuItem mnuAddFrame;
      private System.Windows.Forms.MenuItem mnuRemoveFrame;
      private System.Windows.Forms.ListBox lstSpriteStates;
      private System.Windows.Forms.DataGrid grdParameters;
      private System.Windows.Forms.MenuItem mnuAddAction;
      private System.Windows.Forms.MenuItem mnuRemoveRule;
      private System.Windows.Forms.Label lblRuleName;
      private System.Windows.Forms.TextBox txtRuleName;
      private System.Windows.Forms.Label lblWidthHeight;
      private System.Windows.Forms.TextBox txtWidth;
      private System.Windows.Forms.TextBox txtHeight;
      private System.Windows.Forms.Label lblComma;
      private System.Windows.Forms.TreeView tvwRules;
      private System.Windows.Forms.ImageList imlSpriteDefinition;
      private System.Windows.Forms.TextBox txtErrors;
      private System.Windows.Forms.CheckBox chkEndIf;
      private System.Windows.Forms.Label lblOutput;
      private System.Windows.Forms.ComboBox cboOutput;
      private System.Windows.Forms.Label lblParam3;
      private System.Windows.Forms.ComboBox cboParam3;
      private System.Windows.Forms.ComboBox cboRuleType;
      private System.Windows.Forms.Label lblParam2;
      private System.Windows.Forms.Label lblParam1;
      private System.Windows.Forms.ComboBox cboParam2;
      private System.Windows.Forms.ComboBox cboParam1;
      private System.Windows.Forms.CheckBox chkNot;
      private System.Windows.Forms.ComboBox cboFunction;
      private System.Windows.Forms.Panel pnlRules;
      private System.Windows.Forms.ToolBar tbrRules;
      private System.Windows.Forms.ToolBarButton tbbNewRule;
      private System.Windows.Forms.ToolBarButton tbbDeleteRule;
      private System.Windows.Forms.ToolBarButton tbbRuleSeparator;
      private System.Windows.Forms.ToolBarButton tbbMoveRuleUp;
      private System.Windows.Forms.ToolBarButton tbbMoveRuleDown;
      private System.Windows.Forms.Splitter StateSplitter;
      private System.Windows.Forms.Splitter RuleSplitter;
      private System.Windows.Forms.MenuItem mnuMoveRuleUp;
      private System.Windows.Forms.MenuItem mnuMoveRuleDown;
      private System.Windows.Forms.Panel pnlStateList;
      private System.Windows.Forms.ToolBar tbrStates;
      private System.Windows.Forms.ToolBarButton tbbAddState;
      private System.Windows.Forms.ToolBarButton tbbDeleteState;
      private System.Windows.Forms.TextBox txtHelpText;
      private System.Windows.Forms.ToolBarButton tbbStateSeparator;
      private System.Windows.Forms.ToolBarButton tbbMoveStateUp;
      private System.Windows.Forms.ToolBarButton tbbMoveStateDown;
      private System.Windows.Forms.MenuItem mnuMoveStateUp;
      private System.Windows.Forms.MenuItem mnuMoveStateDown;
      private System.Windows.Forms.Label lblMaskAlpha;
      private System.Windows.Forms.TextBox txtMaskAlpha;
      private System.Windows.Forms.Button btnMaskAlpha;
      private System.Windows.Forms.Timer tmrPopulateRules;
      private System.Windows.Forms.MenuItem mnuSpriteDefSeparator;
      private System.Windows.Forms.MenuItem mnuSpriteDefSeparator2;
      private System.Windows.Forms.MenuItem mnuExport;
      private System.Windows.Forms.CheckBox chkSuspended;
      private System.Windows.Forms.MenuItem mnuSpriteDefSeparator3;
      private System.Windows.Forms.MenuItem mnuRotateWizard;
      private System.Windows.Forms.ToolBarButton tbbPreview;
      private System.Windows.Forms.ToolBarButton tbbStateSeparator2;
      private System.Windows.Forms.MenuItem mnuPreviewAnimation;
      private System.Windows.Forms.MenuItem mnuCopyRules;
      private System.Windows.Forms.MenuItem mnuCopyRuleChildren;
      private System.Windows.Forms.MenuItem mnuCopyRuleOnly;
      private System.Windows.Forms.MenuItem mnuCopyAllRules;
      private System.Windows.Forms.MenuItem mnuPasteRules;
      private System.Windows.Forms.MenuItem mnuPasteRuleAbove;
      private System.Windows.Forms.MenuItem mnuPasteRuleBelow;
      private System.Windows.Forms.ImageList imlTree;
      private System.Windows.Forms.ToolBarButton tbbRuleSeparator2;
      private System.Windows.Forms.ToolBarButton tbbToggleSuspend;
      private System.Windows.Forms.MenuItem mnuToggleSuspend;
      private MenuItem mnuCutRules;
      private MenuItem mnuCutRuleChildren;
      private MenuItem mnuCutRuleOnly;
      private MenuItem mnuCutAllRules;
      private MenuItem mnuConvertToFunc;
      private MenuItem mnuConvertAllRules;
      private MenuItem mnuConvertSelectedRule;
      private ToolBarButton tbbConvertToFunc;
      private Label lblBaseClass;
      private ComboBox cboBaseClass;
      private System.ComponentModel.IContainer components;
      #endregion

      #region Initialization and Clean-up
      public frmSpriteDefinition()
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         String sName;
         Int32 nIdx = 1;
         do
            sName = "New Sprite Definition " + (nIdx++).ToString();
         while (ProjectData.GetSpriteDefinition(sName) != null);
         m_SpriteDef = ProjectData.AddSpriteDefinition(sName, CodeGenerator.SpriteBaseClass);
         txtName.Text = sName;
         cboBaseClass.Text = m_SpriteDef.BaseClass;

         EnableState(false);
         FillFramesets();
         FillStates();
         FillBaseClasses();
         InitParameters();

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"html/b70a3d81-1ff7-429f-b4f9-d65bafe1f09d.htm");
         SGDK2IDE.g_HelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
      }

      public frmSpriteDefinition(ProjectDataset.SpriteDefinitionRow drSpriteDef)
      {
         InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         m_SpriteDef = drSpriteDef;
         txtName.Text = drSpriteDef.Name;
         cboBaseClass.Text = m_SpriteDef.BaseClass;

         EnableState(false);
         FillFramesets();
         FillStates();
         FillBaseClasses();
         InitParameters();

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"html/b70a3d81-1ff7-429f-b4f9-d65bafe1f09d.htm");
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSpriteDefinition));
         this.tabSpriteDefinition = new System.Windows.Forms.TabControl();
         this.tabStates = new System.Windows.Forms.TabPage();
         this.AvailableFrames = new SGDK2.GraphicBrowser();
         this.lblAvailableFrames = new System.Windows.Forms.Label();
         this.StateSplitter = new System.Windows.Forms.Splitter();
         this.pnlFrames = new System.Windows.Forms.Panel();
         this.StateFrames = new SGDK2.GraphicBrowser();
         this.pnlFrameDetails = new System.Windows.Forms.Panel();
         this.btnMaskAlpha = new System.Windows.Forms.Button();
         this.txtMaskAlpha = new System.Windows.Forms.TextBox();
         this.lblMaskAlpha = new System.Windows.Forms.Label();
         this.updRepeatCount = new System.Windows.Forms.NumericUpDown();
         this.lblRepeatCount = new System.Windows.Forms.Label();
         this.lblStateFrames = new System.Windows.Forms.Label();
         this.pnlSpriteState = new System.Windows.Forms.Panel();
         this.lblStateName = new System.Windows.Forms.Label();
         this.txtStateName = new System.Windows.Forms.TextBox();
         this.lblFrameset = new System.Windows.Forms.Label();
         this.cboFrameset = new System.Windows.Forms.ComboBox();
         this.lblWidthHeight = new System.Windows.Forms.Label();
         this.txtWidth = new System.Windows.Forms.TextBox();
         this.lblComma = new System.Windows.Forms.Label();
         this.txtHeight = new System.Windows.Forms.TextBox();
         this.StateTreeSplitter = new System.Windows.Forms.Splitter();
         this.pnlStateList = new System.Windows.Forms.Panel();
         this.lstSpriteStates = new System.Windows.Forms.ListBox();
         this.tbrStates = new System.Windows.Forms.ToolBar();
         this.tbbAddState = new System.Windows.Forms.ToolBarButton();
         this.tbbDeleteState = new System.Windows.Forms.ToolBarButton();
         this.tbbStateSeparator = new System.Windows.Forms.ToolBarButton();
         this.tbbMoveStateUp = new System.Windows.Forms.ToolBarButton();
         this.tbbMoveStateDown = new System.Windows.Forms.ToolBarButton();
         this.tbbStateSeparator2 = new System.Windows.Forms.ToolBarButton();
         this.tbbPreview = new System.Windows.Forms.ToolBarButton();
         this.imlSpriteDefinition = new System.Windows.Forms.ImageList(this.components);
         this.tabParameters = new System.Windows.Forms.TabPage();
         this.grdParameters = new System.Windows.Forms.DataGrid();
         this.tabRules = new System.Windows.Forms.TabPage();
         this.tvwRules = new System.Windows.Forms.TreeView();
         this.imlTree = new System.Windows.Forms.ImageList(this.components);
         this.tbrRules = new System.Windows.Forms.ToolBar();
         this.tbbNewRule = new System.Windows.Forms.ToolBarButton();
         this.tbbDeleteRule = new System.Windows.Forms.ToolBarButton();
         this.tbbRuleSeparator = new System.Windows.Forms.ToolBarButton();
         this.tbbMoveRuleUp = new System.Windows.Forms.ToolBarButton();
         this.tbbMoveRuleDown = new System.Windows.Forms.ToolBarButton();
         this.tbbRuleSeparator2 = new System.Windows.Forms.ToolBarButton();
         this.tbbToggleSuspend = new System.Windows.Forms.ToolBarButton();
         this.tbbConvertToFunc = new System.Windows.Forms.ToolBarButton();
         this.RuleSplitter = new System.Windows.Forms.Splitter();
         this.pnlRules = new System.Windows.Forms.Panel();
         this.chkSuspended = new System.Windows.Forms.CheckBox();
         this.txtHelpText = new System.Windows.Forms.TextBox();
         this.txtRuleName = new System.Windows.Forms.TextBox();
         this.chkNot = new System.Windows.Forms.CheckBox();
         this.cboParam3 = new System.Windows.Forms.ComboBox();
         this.cboFunction = new System.Windows.Forms.ComboBox();
         this.cboRuleType = new System.Windows.Forms.ComboBox();
         this.chkEndIf = new System.Windows.Forms.CheckBox();
         this.txtErrors = new System.Windows.Forms.TextBox();
         this.lblOutput = new System.Windows.Forms.Label();
         this.cboOutput = new System.Windows.Forms.ComboBox();
         this.lblParam2 = new System.Windows.Forms.Label();
         this.lblParam1 = new System.Windows.Forms.Label();
         this.cboParam2 = new System.Windows.Forms.ComboBox();
         this.cboParam1 = new System.Windows.Forms.ComboBox();
         this.lblParam3 = new System.Windows.Forms.Label();
         this.lblRuleName = new System.Windows.Forms.Label();
         this.pnlSpriteHeader = new System.Windows.Forms.Panel();
         this.cboBaseClass = new System.Windows.Forms.ComboBox();
         this.lblBaseClass = new System.Windows.Forms.Label();
         this.txtName = new System.Windows.Forms.TextBox();
         this.lblName = new System.Windows.Forms.Label();
         this.mnuSprites = new System.Windows.Forms.MainMenu(this.components);
         this.mnuSpriteDefinition = new System.Windows.Forms.MenuItem();
         this.mnuAddState = new System.Windows.Forms.MenuItem();
         this.mnuDeleteState = new System.Windows.Forms.MenuItem();
         this.mnuMoveStateUp = new System.Windows.Forms.MenuItem();
         this.mnuMoveStateDown = new System.Windows.Forms.MenuItem();
         this.mnuAddFrame = new System.Windows.Forms.MenuItem();
         this.mnuRemoveFrame = new System.Windows.Forms.MenuItem();
         this.mnuPreviewAnimation = new System.Windows.Forms.MenuItem();
         this.mnuSpriteDefSeparator = new System.Windows.Forms.MenuItem();
         this.mnuAddAction = new System.Windows.Forms.MenuItem();
         this.mnuRemoveRule = new System.Windows.Forms.MenuItem();
         this.mnuMoveRuleUp = new System.Windows.Forms.MenuItem();
         this.mnuMoveRuleDown = new System.Windows.Forms.MenuItem();
         this.mnuCopyRules = new System.Windows.Forms.MenuItem();
         this.mnuCopyRuleChildren = new System.Windows.Forms.MenuItem();
         this.mnuCopyRuleOnly = new System.Windows.Forms.MenuItem();
         this.mnuCopyAllRules = new System.Windows.Forms.MenuItem();
         this.mnuCutRules = new System.Windows.Forms.MenuItem();
         this.mnuCutRuleChildren = new System.Windows.Forms.MenuItem();
         this.mnuCutRuleOnly = new System.Windows.Forms.MenuItem();
         this.mnuCutAllRules = new System.Windows.Forms.MenuItem();
         this.mnuPasteRules = new System.Windows.Forms.MenuItem();
         this.mnuPasteRuleAbove = new System.Windows.Forms.MenuItem();
         this.mnuPasteRuleBelow = new System.Windows.Forms.MenuItem();
         this.mnuToggleSuspend = new System.Windows.Forms.MenuItem();
         this.mnuConvertToFunc = new System.Windows.Forms.MenuItem();
         this.mnuConvertAllRules = new System.Windows.Forms.MenuItem();
         this.mnuConvertSelectedRule = new System.Windows.Forms.MenuItem();
         this.mnuSpriteDefSeparator2 = new System.Windows.Forms.MenuItem();
         this.mnuExport = new System.Windows.Forms.MenuItem();
         this.mnuSpriteDefSeparator3 = new System.Windows.Forms.MenuItem();
         this.mnuRotateWizard = new System.Windows.Forms.MenuItem();
         this.DataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.tmrPopulateRules = new System.Windows.Forms.Timer(this.components);
         this.tabSpriteDefinition.SuspendLayout();
         this.tabStates.SuspendLayout();
         this.pnlFrames.SuspendLayout();
         this.pnlFrameDetails.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.updRepeatCount)).BeginInit();
         this.pnlSpriteState.SuspendLayout();
         this.pnlStateList.SuspendLayout();
         this.tabParameters.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.grdParameters)).BeginInit();
         this.tabRules.SuspendLayout();
         this.pnlRules.SuspendLayout();
         this.pnlSpriteHeader.SuspendLayout();
         this.SuspendLayout();
         // 
         // tabSpriteDefinition
         // 
         this.tabSpriteDefinition.Controls.Add(this.tabStates);
         this.tabSpriteDefinition.Controls.Add(this.tabParameters);
         this.tabSpriteDefinition.Controls.Add(this.tabRules);
         this.tabSpriteDefinition.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tabSpriteDefinition.Location = new System.Drawing.Point(0, 32);
         this.tabSpriteDefinition.Name = "tabSpriteDefinition";
         this.tabSpriteDefinition.SelectedIndex = 0;
         this.tabSpriteDefinition.Size = new System.Drawing.Size(565, 385);
         this.tabSpriteDefinition.TabIndex = 3;
         this.tabSpriteDefinition.SelectedIndexChanged += new System.EventHandler(this.tabSpriteDefinition_SelectedIndexChanged);
         // 
         // tabStates
         // 
         this.tabStates.Controls.Add(this.AvailableFrames);
         this.tabStates.Controls.Add(this.lblAvailableFrames);
         this.tabStates.Controls.Add(this.StateSplitter);
         this.tabStates.Controls.Add(this.pnlFrames);
         this.tabStates.Controls.Add(this.pnlSpriteState);
         this.tabStates.Controls.Add(this.StateTreeSplitter);
         this.tabStates.Controls.Add(this.pnlStateList);
         this.tabStates.Location = new System.Drawing.Point(4, 22);
         this.tabStates.Name = "tabStates";
         this.tabStates.Size = new System.Drawing.Size(557, 359);
         this.tabStates.TabIndex = 0;
         this.tabStates.Text = "States";
         // 
         // AvailableFrames
         // 
         this.AvailableFrames.AllowDrop = true;
         this.AvailableFrames.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.AvailableFrames.CellBorders = false;
         this.AvailableFrames.CellPadding = new System.Drawing.Size(0, 0);
         this.AvailableFrames.CellSize = new System.Drawing.Size(0, 0);
         this.AvailableFrames.CurrentCellIndex = -1;
         this.AvailableFrames.Dock = System.Windows.Forms.DockStyle.Fill;
         this.AvailableFrames.Frameset = null;
         this.AvailableFrames.FramesToDisplay = null;
         this.AvailableFrames.GraphicSheet = null;
         this.AvailableFrames.Location = new System.Drawing.Point(133, 221);
         this.AvailableFrames.Name = "AvailableFrames";
         this.AvailableFrames.SheetImage = null;
         this.AvailableFrames.Size = new System.Drawing.Size(424, 138);
         this.AvailableFrames.TabIndex = 10;
         // 
         // lblAvailableFrames
         // 
         this.lblAvailableFrames.Dock = System.Windows.Forms.DockStyle.Top;
         this.lblAvailableFrames.Location = new System.Drawing.Point(133, 205);
         this.lblAvailableFrames.Name = "lblAvailableFrames";
         this.lblAvailableFrames.Size = new System.Drawing.Size(424, 16);
         this.lblAvailableFrames.TabIndex = 9;
         this.lblAvailableFrames.Text = "Available Frames:";
         // 
         // StateSplitter
         // 
         this.StateSplitter.BackColor = System.Drawing.SystemColors.ControlDark;
         this.StateSplitter.Dock = System.Windows.Forms.DockStyle.Top;
         this.StateSplitter.Location = new System.Drawing.Point(133, 200);
         this.StateSplitter.Name = "StateSplitter";
         this.StateSplitter.Size = new System.Drawing.Size(424, 5);
         this.StateSplitter.TabIndex = 8;
         this.StateSplitter.TabStop = false;
         // 
         // pnlFrames
         // 
         this.pnlFrames.Controls.Add(this.StateFrames);
         this.pnlFrames.Controls.Add(this.pnlFrameDetails);
         this.pnlFrames.Controls.Add(this.lblStateFrames);
         this.pnlFrames.Dock = System.Windows.Forms.DockStyle.Top;
         this.pnlFrames.Location = new System.Drawing.Point(133, 56);
         this.pnlFrames.Name = "pnlFrames";
         this.pnlFrames.Size = new System.Drawing.Size(424, 144);
         this.pnlFrames.TabIndex = 6;
         // 
         // StateFrames
         // 
         this.StateFrames.AllowDrop = true;
         this.StateFrames.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.StateFrames.CellBorders = false;
         this.StateFrames.CellPadding = new System.Drawing.Size(0, 0);
         this.StateFrames.CellSize = new System.Drawing.Size(0, 0);
         this.StateFrames.CurrentCellIndex = -1;
         this.StateFrames.Dock = System.Windows.Forms.DockStyle.Fill;
         this.StateFrames.Frameset = null;
         this.StateFrames.FramesToDisplay = null;
         this.StateFrames.GraphicSheet = null;
         this.StateFrames.Location = new System.Drawing.Point(0, 16);
         this.StateFrames.Name = "StateFrames";
         this.StateFrames.SheetImage = null;
         this.StateFrames.Size = new System.Drawing.Size(424, 104);
         this.StateFrames.TabIndex = 1;
         this.StateFrames.DragOver += new System.Windows.Forms.DragEventHandler(this.StateFrames_DragOver);
         this.StateFrames.CurrentCellChanged += new System.EventHandler(this.StateFrames_CurrentCellChanged);
         this.StateFrames.DragDrop += new System.Windows.Forms.DragEventHandler(this.StateFrames_DragDrop);
         this.StateFrames.KeyDown += new System.Windows.Forms.KeyEventHandler(this.StateFrames_KeyDown);
         // 
         // pnlFrameDetails
         // 
         this.pnlFrameDetails.Controls.Add(this.btnMaskAlpha);
         this.pnlFrameDetails.Controls.Add(this.txtMaskAlpha);
         this.pnlFrameDetails.Controls.Add(this.lblMaskAlpha);
         this.pnlFrameDetails.Controls.Add(this.updRepeatCount);
         this.pnlFrameDetails.Controls.Add(this.lblRepeatCount);
         this.pnlFrameDetails.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.pnlFrameDetails.Location = new System.Drawing.Point(0, 120);
         this.pnlFrameDetails.Name = "pnlFrameDetails";
         this.pnlFrameDetails.Padding = new System.Windows.Forms.Padding(1, 2, 0, 2);
         this.pnlFrameDetails.Size = new System.Drawing.Size(424, 24);
         this.pnlFrameDetails.TabIndex = 7;
         // 
         // btnMaskAlpha
         // 
         this.btnMaskAlpha.Dock = System.Windows.Forms.DockStyle.Left;
         this.btnMaskAlpha.Location = new System.Drawing.Point(321, 2);
         this.btnMaskAlpha.Name = "btnMaskAlpha";
         this.btnMaskAlpha.Size = new System.Drawing.Size(24, 20);
         this.btnMaskAlpha.TabIndex = 4;
         this.btnMaskAlpha.Text = "...";
         this.btnMaskAlpha.Click += new System.EventHandler(this.btnMaskAlpha_Click);
         // 
         // txtMaskAlpha
         // 
         this.txtMaskAlpha.Dock = System.Windows.Forms.DockStyle.Left;
         this.txtMaskAlpha.Location = new System.Drawing.Point(273, 2);
         this.txtMaskAlpha.Name = "txtMaskAlpha";
         this.txtMaskAlpha.ReadOnly = true;
         this.txtMaskAlpha.Size = new System.Drawing.Size(48, 20);
         this.txtMaskAlpha.TabIndex = 3;
         // 
         // lblMaskAlpha
         // 
         this.lblMaskAlpha.Dock = System.Windows.Forms.DockStyle.Left;
         this.lblMaskAlpha.Location = new System.Drawing.Point(169, 2);
         this.lblMaskAlpha.Name = "lblMaskAlpha";
         this.lblMaskAlpha.Size = new System.Drawing.Size(104, 20);
         this.lblMaskAlpha.TabIndex = 2;
         this.lblMaskAlpha.Text = "Mask Alpha Level:";
         this.lblMaskAlpha.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // updRepeatCount
         // 
         this.updRepeatCount.Dock = System.Windows.Forms.DockStyle.Left;
         this.updRepeatCount.Location = new System.Drawing.Point(105, 2);
         this.updRepeatCount.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
         this.updRepeatCount.Name = "updRepeatCount";
         this.updRepeatCount.Size = new System.Drawing.Size(64, 20);
         this.updRepeatCount.TabIndex = 1;
         this.updRepeatCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
         this.updRepeatCount.ValueChanged += new System.EventHandler(this.updRepeatCount_ValueChanged);
         this.updRepeatCount.Validated += new System.EventHandler(this.updRepeatCount_Validated);
         // 
         // lblRepeatCount
         // 
         this.lblRepeatCount.Dock = System.Windows.Forms.DockStyle.Left;
         this.lblRepeatCount.Location = new System.Drawing.Point(1, 2);
         this.lblRepeatCount.Name = "lblRepeatCount";
         this.lblRepeatCount.Size = new System.Drawing.Size(104, 20);
         this.lblRepeatCount.TabIndex = 0;
         this.lblRepeatCount.Text = "Repeat Count:";
         this.lblRepeatCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblStateFrames
         // 
         this.lblStateFrames.Dock = System.Windows.Forms.DockStyle.Top;
         this.lblStateFrames.Location = new System.Drawing.Point(0, 0);
         this.lblStateFrames.Name = "lblStateFrames";
         this.lblStateFrames.Size = new System.Drawing.Size(424, 16);
         this.lblStateFrames.TabIndex = 0;
         this.lblStateFrames.Text = "Frames in Current State:";
         // 
         // pnlSpriteState
         // 
         this.pnlSpriteState.Controls.Add(this.lblStateName);
         this.pnlSpriteState.Controls.Add(this.txtStateName);
         this.pnlSpriteState.Controls.Add(this.lblFrameset);
         this.pnlSpriteState.Controls.Add(this.cboFrameset);
         this.pnlSpriteState.Controls.Add(this.lblWidthHeight);
         this.pnlSpriteState.Controls.Add(this.txtWidth);
         this.pnlSpriteState.Controls.Add(this.lblComma);
         this.pnlSpriteState.Controls.Add(this.txtHeight);
         this.pnlSpriteState.Dock = System.Windows.Forms.DockStyle.Top;
         this.pnlSpriteState.Location = new System.Drawing.Point(133, 0);
         this.pnlSpriteState.Name = "pnlSpriteState";
         this.pnlSpriteState.Size = new System.Drawing.Size(424, 56);
         this.pnlSpriteState.TabIndex = 5;
         // 
         // lblStateName
         // 
         this.lblStateName.Location = new System.Drawing.Point(8, 8);
         this.lblStateName.Name = "lblStateName";
         this.lblStateName.Size = new System.Drawing.Size(80, 20);
         this.lblStateName.TabIndex = 0;
         this.lblStateName.Text = "State Name:";
         this.lblStateName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtStateName
         // 
         this.txtStateName.Location = new System.Drawing.Point(88, 8);
         this.txtStateName.Name = "txtStateName";
         this.txtStateName.Size = new System.Drawing.Size(120, 20);
         this.txtStateName.TabIndex = 1;
         this.txtStateName.Validated += new System.EventHandler(this.txtStateName_Validated);
         this.txtStateName.Validating += new System.ComponentModel.CancelEventHandler(this.txtStateName_Validating);
         // 
         // lblFrameset
         // 
         this.lblFrameset.Location = new System.Drawing.Point(216, 8);
         this.lblFrameset.Name = "lblFrameset";
         this.lblFrameset.Size = new System.Drawing.Size(80, 21);
         this.lblFrameset.TabIndex = 2;
         this.lblFrameset.Text = "Frameset:";
         this.lblFrameset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // cboFrameset
         // 
         this.cboFrameset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.cboFrameset.DisplayMember = "Name";
         this.cboFrameset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboFrameset.Location = new System.Drawing.Point(296, 8);
         this.cboFrameset.Name = "cboFrameset";
         this.cboFrameset.Size = new System.Drawing.Size(117, 21);
         this.cboFrameset.TabIndex = 3;
         this.cboFrameset.SelectionChangeCommitted += new System.EventHandler(this.cboFrameset_SelectionChangeCommitted);
         // 
         // lblWidthHeight
         // 
         this.lblWidthHeight.Location = new System.Drawing.Point(8, 32);
         this.lblWidthHeight.Name = "lblWidthHeight";
         this.lblWidthHeight.Size = new System.Drawing.Size(80, 20);
         this.lblWidthHeight.TabIndex = 6;
         this.lblWidthHeight.Text = "Width, height:";
         this.lblWidthHeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtWidth
         // 
         this.txtWidth.Location = new System.Drawing.Point(88, 32);
         this.txtWidth.Name = "txtWidth";
         this.txtWidth.Size = new System.Drawing.Size(48, 20);
         this.txtWidth.TabIndex = 7;
         this.txtWidth.Validated += new System.EventHandler(this.txtWidth_Validated);
         this.txtWidth.Validating += new System.ComponentModel.CancelEventHandler(this.txtSize_Validating);
         // 
         // lblComma
         // 
         this.lblComma.Location = new System.Drawing.Point(136, 32);
         this.lblComma.Name = "lblComma";
         this.lblComma.Size = new System.Drawing.Size(24, 20);
         this.lblComma.TabIndex = 8;
         this.lblComma.Text = ",";
         this.lblComma.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
         // 
         // txtHeight
         // 
         this.txtHeight.Location = new System.Drawing.Point(160, 32);
         this.txtHeight.Name = "txtHeight";
         this.txtHeight.Size = new System.Drawing.Size(48, 20);
         this.txtHeight.TabIndex = 9;
         this.txtHeight.Validated += new System.EventHandler(this.txtHeight_Validated);
         this.txtHeight.Validating += new System.ComponentModel.CancelEventHandler(this.txtSize_Validating);
         // 
         // StateTreeSplitter
         // 
         this.StateTreeSplitter.Location = new System.Drawing.Point(128, 0);
         this.StateTreeSplitter.Name = "StateTreeSplitter";
         this.StateTreeSplitter.Size = new System.Drawing.Size(5, 359);
         this.StateTreeSplitter.TabIndex = 11;
         this.StateTreeSplitter.TabStop = false;
         // 
         // pnlStateList
         // 
         this.pnlStateList.Controls.Add(this.lstSpriteStates);
         this.pnlStateList.Controls.Add(this.tbrStates);
         this.pnlStateList.Dock = System.Windows.Forms.DockStyle.Left;
         this.pnlStateList.Location = new System.Drawing.Point(0, 0);
         this.pnlStateList.Name = "pnlStateList";
         this.pnlStateList.Size = new System.Drawing.Size(128, 359);
         this.pnlStateList.TabIndex = 12;
         // 
         // lstSpriteStates
         // 
         this.lstSpriteStates.DisplayMember = "Name";
         this.lstSpriteStates.Dock = System.Windows.Forms.DockStyle.Fill;
         this.lstSpriteStates.IntegralHeight = false;
         this.lstSpriteStates.Location = new System.Drawing.Point(0, 25);
         this.lstSpriteStates.Name = "lstSpriteStates";
         this.lstSpriteStates.Size = new System.Drawing.Size(128, 334);
         this.lstSpriteStates.TabIndex = 4;
         this.lstSpriteStates.SelectedIndexChanged += new System.EventHandler(this.lstSpriteStates_SelectedIndexChanged);
         // 
         // tbrStates
         // 
         this.tbrStates.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.tbbAddState,
            this.tbbDeleteState,
            this.tbbStateSeparator,
            this.tbbMoveStateUp,
            this.tbbMoveStateDown,
            this.tbbStateSeparator2,
            this.tbbPreview});
         this.tbrStates.Divider = false;
         this.tbrStates.DropDownArrows = true;
         this.tbrStates.ImageList = this.imlSpriteDefinition;
         this.tbrStates.Location = new System.Drawing.Point(0, 0);
         this.tbrStates.Name = "tbrStates";
         this.tbrStates.ShowToolTips = true;
         this.tbrStates.Size = new System.Drawing.Size(128, 25);
         this.tbrStates.TabIndex = 0;
         this.tbrStates.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbrStates_ButtonClick);
         // 
         // tbbAddState
         // 
         this.tbbAddState.ImageIndex = 0;
         this.tbbAddState.Name = "tbbAddState";
         this.tbbAddState.ToolTipText = "Add a new state";
         // 
         // tbbDeleteState
         // 
         this.tbbDeleteState.ImageIndex = 1;
         this.tbbDeleteState.Name = "tbbDeleteState";
         this.tbbDeleteState.ToolTipText = "Delete the selected state";
         // 
         // tbbStateSeparator
         // 
         this.tbbStateSeparator.Name = "tbbStateSeparator";
         this.tbbStateSeparator.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
         // 
         // tbbMoveStateUp
         // 
         this.tbbMoveStateUp.ImageIndex = 2;
         this.tbbMoveStateUp.Name = "tbbMoveStateUp";
         this.tbbMoveStateUp.ToolTipText = "Move the selected state up one slot";
         // 
         // tbbMoveStateDown
         // 
         this.tbbMoveStateDown.ImageIndex = 3;
         this.tbbMoveStateDown.Name = "tbbMoveStateDown";
         this.tbbMoveStateDown.ToolTipText = "Move the selected state down one slot";
         // 
         // tbbStateSeparator2
         // 
         this.tbbStateSeparator2.Name = "tbbStateSeparator2";
         this.tbbStateSeparator2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
         // 
         // tbbPreview
         // 
         this.tbbPreview.ImageIndex = 4;
         this.tbbPreview.Name = "tbbPreview";
         this.tbbPreview.ToolTipText = "Preview state animation";
         // 
         // imlSpriteDefinition
         // 
         this.imlSpriteDefinition.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlSpriteDefinition.ImageStream")));
         this.imlSpriteDefinition.TransparentColor = System.Drawing.Color.Magenta;
         this.imlSpriteDefinition.Images.SetKeyName(0, "");
         this.imlSpriteDefinition.Images.SetKeyName(1, "");
         this.imlSpriteDefinition.Images.SetKeyName(2, "");
         this.imlSpriteDefinition.Images.SetKeyName(3, "");
         this.imlSpriteDefinition.Images.SetKeyName(4, "");
         this.imlSpriteDefinition.Images.SetKeyName(5, "");
         this.imlSpriteDefinition.Images.SetKeyName(6, "Code.bmp");
         // 
         // tabParameters
         // 
         this.tabParameters.Controls.Add(this.grdParameters);
         this.tabParameters.Location = new System.Drawing.Point(4, 22);
         this.tabParameters.Name = "tabParameters";
         this.tabParameters.Size = new System.Drawing.Size(557, 359);
         this.tabParameters.TabIndex = 1;
         this.tabParameters.Text = "Parameters";
         // 
         // grdParameters
         // 
         this.grdParameters.AllowNavigation = false;
         this.grdParameters.CaptionVisible = false;
         this.grdParameters.DataMember = "";
         this.grdParameters.Dock = System.Windows.Forms.DockStyle.Fill;
         this.grdParameters.HeaderForeColor = System.Drawing.SystemColors.ControlText;
         this.grdParameters.Location = new System.Drawing.Point(0, 0);
         this.grdParameters.Name = "grdParameters";
         this.grdParameters.Size = new System.Drawing.Size(557, 359);
         this.grdParameters.TabIndex = 35;
         // 
         // tabRules
         // 
         this.tabRules.Controls.Add(this.tvwRules);
         this.tabRules.Controls.Add(this.tbrRules);
         this.tabRules.Controls.Add(this.RuleSplitter);
         this.tabRules.Controls.Add(this.pnlRules);
         this.tabRules.Location = new System.Drawing.Point(4, 22);
         this.tabRules.Name = "tabRules";
         this.tabRules.Size = new System.Drawing.Size(557, 359);
         this.tabRules.TabIndex = 2;
         this.tabRules.Text = "Rules";
         // 
         // tvwRules
         // 
         this.tvwRules.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tvwRules.HideSelection = false;
         this.tvwRules.ImageIndex = 0;
         this.tvwRules.ImageList = this.imlTree;
         this.tvwRules.Location = new System.Drawing.Point(0, 25);
         this.tvwRules.Name = "tvwRules";
         this.tvwRules.SelectedImageIndex = 0;
         this.tvwRules.Size = new System.Drawing.Size(184, 334);
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
         // tbrRules
         // 
         this.tbrRules.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.tbbNewRule,
            this.tbbDeleteRule,
            this.tbbRuleSeparator,
            this.tbbMoveRuleUp,
            this.tbbMoveRuleDown,
            this.tbbRuleSeparator2,
            this.tbbToggleSuspend,
            this.tbbConvertToFunc});
         this.tbrRules.Divider = false;
         this.tbrRules.DropDownArrows = true;
         this.tbrRules.ImageList = this.imlSpriteDefinition;
         this.tbrRules.Location = new System.Drawing.Point(0, 0);
         this.tbrRules.Name = "tbrRules";
         this.tbrRules.ShowToolTips = true;
         this.tbrRules.Size = new System.Drawing.Size(184, 25);
         this.tbrRules.TabIndex = 28;
         this.tbrRules.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbrRules_ButtonClick);
         // 
         // tbbNewRule
         // 
         this.tbbNewRule.ImageIndex = 0;
         this.tbbNewRule.Name = "tbbNewRule";
         this.tbbNewRule.ToolTipText = "Add a new rule";
         // 
         // tbbDeleteRule
         // 
         this.tbbDeleteRule.ImageIndex = 1;
         this.tbbDeleteRule.Name = "tbbDeleteRule";
         this.tbbDeleteRule.ToolTipText = "Delete the selected rule";
         // 
         // tbbRuleSeparator
         // 
         this.tbbRuleSeparator.Name = "tbbRuleSeparator";
         this.tbbRuleSeparator.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
         // 
         // tbbMoveRuleUp
         // 
         this.tbbMoveRuleUp.ImageIndex = 2;
         this.tbbMoveRuleUp.Name = "tbbMoveRuleUp";
         this.tbbMoveRuleUp.ToolTipText = "Move selected rule up";
         // 
         // tbbMoveRuleDown
         // 
         this.tbbMoveRuleDown.ImageIndex = 3;
         this.tbbMoveRuleDown.Name = "tbbMoveRuleDown";
         this.tbbMoveRuleDown.ToolTipText = "Move selected rule down";
         // 
         // tbbRuleSeparator2
         // 
         this.tbbRuleSeparator2.Name = "tbbRuleSeparator2";
         this.tbbRuleSeparator2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
         // 
         // tbbToggleSuspend
         // 
         this.tbbToggleSuspend.ImageIndex = 5;
         this.tbbToggleSuspend.Name = "tbbToggleSuspend";
         this.tbbToggleSuspend.ToolTipText = "Toggle suspend for this rule and all children";
         // 
         // tbbConvertToFunc
         // 
         this.tbbConvertToFunc.ImageIndex = 6;
         this.tbbConvertToFunc.Name = "tbbConvertToFunc";
         this.tbbConvertToFunc.ToolTipText = "Convert selected rule and children to function";
         // 
         // RuleSplitter
         // 
         this.RuleSplitter.Dock = System.Windows.Forms.DockStyle.Right;
         this.RuleSplitter.Location = new System.Drawing.Point(184, 0);
         this.RuleSplitter.Name = "RuleSplitter";
         this.RuleSplitter.Size = new System.Drawing.Size(5, 359);
         this.RuleSplitter.TabIndex = 13;
         this.RuleSplitter.TabStop = false;
         // 
         // pnlRules
         // 
         this.pnlRules.Controls.Add(this.chkSuspended);
         this.pnlRules.Controls.Add(this.txtHelpText);
         this.pnlRules.Controls.Add(this.txtRuleName);
         this.pnlRules.Controls.Add(this.chkNot);
         this.pnlRules.Controls.Add(this.cboParam3);
         this.pnlRules.Controls.Add(this.cboFunction);
         this.pnlRules.Controls.Add(this.cboRuleType);
         this.pnlRules.Controls.Add(this.chkEndIf);
         this.pnlRules.Controls.Add(this.txtErrors);
         this.pnlRules.Controls.Add(this.lblOutput);
         this.pnlRules.Controls.Add(this.cboOutput);
         this.pnlRules.Controls.Add(this.lblParam2);
         this.pnlRules.Controls.Add(this.lblParam1);
         this.pnlRules.Controls.Add(this.cboParam2);
         this.pnlRules.Controls.Add(this.cboParam1);
         this.pnlRules.Controls.Add(this.lblParam3);
         this.pnlRules.Controls.Add(this.lblRuleName);
         this.pnlRules.Dock = System.Windows.Forms.DockStyle.Right;
         this.pnlRules.Location = new System.Drawing.Point(189, 0);
         this.pnlRules.Name = "pnlRules";
         this.pnlRules.Size = new System.Drawing.Size(368, 359);
         this.pnlRules.TabIndex = 27;
         // 
         // chkSuspended
         // 
         this.chkSuspended.Location = new System.Drawing.Point(8, 32);
         this.chkSuspended.Name = "chkSuspended";
         this.chkSuspended.Size = new System.Drawing.Size(344, 16);
         this.chkSuspended.TabIndex = 5;
         this.chkSuspended.Text = "Suspend this rule";
         this.chkSuspended.CheckedChanged += new System.EventHandler(this.chkSuspended_CheckedChanged);
         // 
         // txtHelpText
         // 
         this.txtHelpText.BackColor = System.Drawing.SystemColors.Info;
         this.txtHelpText.ForeColor = System.Drawing.SystemColors.InfoText;
         this.txtHelpText.Location = new System.Drawing.Point(8, 80);
         this.txtHelpText.Multiline = true;
         this.txtHelpText.Name = "txtHelpText";
         this.txtHelpText.ReadOnly = true;
         this.txtHelpText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtHelpText.Size = new System.Drawing.Size(349, 32);
         this.txtHelpText.TabIndex = 16;
         // 
         // txtRuleName
         // 
         this.txtRuleName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtRuleName.Enabled = false;
         this.txtRuleName.Location = new System.Drawing.Point(128, 8);
         this.txtRuleName.Name = "txtRuleName";
         this.txtRuleName.Size = new System.Drawing.Size(229, 20);
         this.txtRuleName.TabIndex = 4;
         this.txtRuleName.Validated += new System.EventHandler(this.txtRuleName_Validated);
         this.txtRuleName.Validating += new System.ComponentModel.CancelEventHandler(this.txtRuleName_Validating);
         // 
         // chkNot
         // 
         this.chkNot.Enabled = false;
         this.chkNot.Location = new System.Drawing.Point(72, 56);
         this.chkNot.Name = "chkNot";
         this.chkNot.Size = new System.Drawing.Size(56, 21);
         this.chkNot.TabIndex = 14;
         this.chkNot.Text = "Not";
         this.chkNot.CheckedChanged += new System.EventHandler(this.chkNot_CheckedChanged);
         // 
         // cboParam3
         // 
         this.cboParam3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.cboParam3.Enabled = false;
         this.cboParam3.Location = new System.Drawing.Point(128, 168);
         this.cboParam3.Name = "cboParam3";
         this.cboParam3.Size = new System.Drawing.Size(229, 21);
         this.cboParam3.TabIndex = 22;
         this.cboParam3.SelectionChangeCommitted += new System.EventHandler(this.cboParam_SelectionChangeCommitted);
         this.cboParam3.Validated += new System.EventHandler(this.cboParam_Validated);
         // 
         // cboFunction
         // 
         this.cboFunction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.cboFunction.Enabled = false;
         this.cboFunction.Location = new System.Drawing.Point(128, 56);
         this.cboFunction.Name = "cboFunction";
         this.cboFunction.Size = new System.Drawing.Size(229, 21);
         this.cboFunction.TabIndex = 15;
         this.cboFunction.SelectedIndexChanged += new System.EventHandler(this.cboFunction_SelectedIndexChanged);
         this.cboFunction.Validated += new System.EventHandler(this.cboFunction_Validated);
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
         this.cboRuleType.TabIndex = 13;
         this.cboRuleType.SelectedIndexChanged += new System.EventHandler(this.cboRuleType_SelectedIndexChanged);
         // 
         // chkEndIf
         // 
         this.chkEndIf.Enabled = false;
         this.chkEndIf.Location = new System.Drawing.Point(8, 216);
         this.chkEndIf.Name = "chkEndIf";
         this.chkEndIf.Size = new System.Drawing.Size(120, 24);
         this.chkEndIf.TabIndex = 25;
         this.chkEndIf.Text = "End If/End While";
         this.chkEndIf.CheckedChanged += new System.EventHandler(this.chkEndIf_CheckedChanged);
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
         this.txtErrors.Size = new System.Drawing.Size(349, 104);
         this.txtErrors.TabIndex = 26;
         this.txtErrors.Visible = false;
         // 
         // lblOutput
         // 
         this.lblOutput.Enabled = false;
         this.lblOutput.Location = new System.Drawing.Point(8, 192);
         this.lblOutput.Name = "lblOutput";
         this.lblOutput.Size = new System.Drawing.Size(120, 21);
         this.lblOutput.TabIndex = 23;
         this.lblOutput.Text = "Output to:";
         this.lblOutput.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // cboOutput
         // 
         this.cboOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.cboOutput.Enabled = false;
         this.cboOutput.Location = new System.Drawing.Point(128, 192);
         this.cboOutput.Name = "cboOutput";
         this.cboOutput.Size = new System.Drawing.Size(229, 21);
         this.cboOutput.TabIndex = 24;
         this.cboOutput.Validated += new System.EventHandler(this.cboParam_Validated);
         // 
         // lblParam2
         // 
         this.lblParam2.Enabled = false;
         this.lblParam2.Location = new System.Drawing.Point(8, 144);
         this.lblParam2.Name = "lblParam2";
         this.lblParam2.Size = new System.Drawing.Size(120, 21);
         this.lblParam2.TabIndex = 19;
         this.lblParam2.Text = "Parameter 2:";
         this.lblParam2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblParam1
         // 
         this.lblParam1.Enabled = false;
         this.lblParam1.Location = new System.Drawing.Point(8, 120);
         this.lblParam1.Name = "lblParam1";
         this.lblParam1.Size = new System.Drawing.Size(120, 21);
         this.lblParam1.TabIndex = 17;
         this.lblParam1.Text = "Parameter 1:";
         this.lblParam1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // cboParam2
         // 
         this.cboParam2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.cboParam2.Enabled = false;
         this.cboParam2.Location = new System.Drawing.Point(128, 144);
         this.cboParam2.Name = "cboParam2";
         this.cboParam2.Size = new System.Drawing.Size(229, 21);
         this.cboParam2.TabIndex = 20;
         this.cboParam2.SelectionChangeCommitted += new System.EventHandler(this.cboParam_SelectionChangeCommitted);
         this.cboParam2.Validated += new System.EventHandler(this.cboParam_Validated);
         // 
         // cboParam1
         // 
         this.cboParam1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.cboParam1.Enabled = false;
         this.cboParam1.Location = new System.Drawing.Point(128, 120);
         this.cboParam1.Name = "cboParam1";
         this.cboParam1.Size = new System.Drawing.Size(229, 21);
         this.cboParam1.TabIndex = 18;
         this.cboParam1.SelectionChangeCommitted += new System.EventHandler(this.cboParam_SelectionChangeCommitted);
         this.cboParam1.Validated += new System.EventHandler(this.cboParam_Validated);
         // 
         // lblParam3
         // 
         this.lblParam3.Enabled = false;
         this.lblParam3.Location = new System.Drawing.Point(8, 168);
         this.lblParam3.Name = "lblParam3";
         this.lblParam3.Size = new System.Drawing.Size(120, 21);
         this.lblParam3.TabIndex = 21;
         this.lblParam3.Text = "Parameter 3:";
         this.lblParam3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblRuleName
         // 
         this.lblRuleName.Enabled = false;
         this.lblRuleName.Location = new System.Drawing.Point(8, 8);
         this.lblRuleName.Name = "lblRuleName";
         this.lblRuleName.Size = new System.Drawing.Size(120, 20);
         this.lblRuleName.TabIndex = 3;
         this.lblRuleName.Text = "Rule Name:";
         this.lblRuleName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // pnlSpriteHeader
         // 
         this.pnlSpriteHeader.Controls.Add(this.cboBaseClass);
         this.pnlSpriteHeader.Controls.Add(this.lblBaseClass);
         this.pnlSpriteHeader.Controls.Add(this.txtName);
         this.pnlSpriteHeader.Controls.Add(this.lblName);
         this.pnlSpriteHeader.Dock = System.Windows.Forms.DockStyle.Top;
         this.pnlSpriteHeader.Location = new System.Drawing.Point(0, 0);
         this.pnlSpriteHeader.Name = "pnlSpriteHeader";
         this.pnlSpriteHeader.Size = new System.Drawing.Size(565, 32);
         this.pnlSpriteHeader.TabIndex = 0;
         // 
         // cboBaseClass
         // 
         this.cboBaseClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.cboBaseClass.FormattingEnabled = true;
         this.cboBaseClass.Location = new System.Drawing.Point(316, 7);
         this.cboBaseClass.Name = "cboBaseClass";
         this.cboBaseClass.Size = new System.Drawing.Size(234, 21);
         this.cboBaseClass.TabIndex = 4;
         this.cboBaseClass.TextUpdate += new System.EventHandler(this.cboBaseClass_TextUpdate);
         this.cboBaseClass.TextChanged += new System.EventHandler(this.cboBaseClass_TextUpdate);
         // 
         // lblBaseClass
         // 
         this.lblBaseClass.AutoSize = true;
         this.lblBaseClass.Location = new System.Drawing.Point(248, 10);
         this.lblBaseClass.Name = "lblBaseClass";
         this.lblBaseClass.Size = new System.Drawing.Size(62, 13);
         this.lblBaseClass.TabIndex = 3;
         this.lblBaseClass.Text = "Base Class:";
         // 
         // txtName
         // 
         this.txtName.Location = new System.Drawing.Point(65, 7);
         this.txtName.Name = "txtName";
         this.txtName.Size = new System.Drawing.Size(160, 20);
         this.txtName.TabIndex = 2;
         this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
         this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
         // 
         // lblName
         // 
         this.lblName.Location = new System.Drawing.Point(12, 6);
         this.lblName.Name = "lblName";
         this.lblName.Size = new System.Drawing.Size(64, 20);
         this.lblName.TabIndex = 1;
         this.lblName.Text = "Name:";
         this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // mnuSprites
         // 
         this.mnuSprites.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuSpriteDefinition});
         // 
         // mnuSpriteDefinition
         // 
         this.mnuSpriteDefinition.Index = 0;
         this.mnuSpriteDefinition.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuAddState,
            this.mnuDeleteState,
            this.mnuMoveStateUp,
            this.mnuMoveStateDown,
            this.mnuAddFrame,
            this.mnuRemoveFrame,
            this.mnuPreviewAnimation,
            this.mnuSpriteDefSeparator,
            this.mnuAddAction,
            this.mnuRemoveRule,
            this.mnuMoveRuleUp,
            this.mnuMoveRuleDown,
            this.mnuCopyRules,
            this.mnuCutRules,
            this.mnuPasteRules,
            this.mnuToggleSuspend,
            this.mnuConvertToFunc,
            this.mnuSpriteDefSeparator2,
            this.mnuExport,
            this.mnuSpriteDefSeparator3,
            this.mnuRotateWizard});
         this.mnuSpriteDefinition.Text = "&Sprite Definition";
         // 
         // mnuAddState
         // 
         this.mnuAddState.Index = 0;
         this.mnuAddState.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
         this.mnuAddState.Text = "&Add State";
         this.mnuAddState.Click += new System.EventHandler(this.OnAddState);
         // 
         // mnuDeleteState
         // 
         this.mnuDeleteState.Index = 1;
         this.mnuDeleteState.Shortcut = System.Windows.Forms.Shortcut.CtrlD;
         this.mnuDeleteState.Text = "&Delete State";
         this.mnuDeleteState.Click += new System.EventHandler(this.OnDeleteState);
         // 
         // mnuMoveStateUp
         // 
         this.mnuMoveStateUp.Index = 2;
         this.mnuMoveStateUp.Text = "Move &State Up";
         this.mnuMoveStateUp.Click += new System.EventHandler(this.OnMoveStateUp);
         // 
         // mnuMoveStateDown
         // 
         this.mnuMoveStateDown.Index = 3;
         this.mnuMoveStateDown.Text = "Move State Do&wn";
         this.mnuMoveStateDown.Click += new System.EventHandler(this.OnMoveStateDown);
         // 
         // mnuAddFrame
         // 
         this.mnuAddFrame.Index = 4;
         this.mnuAddFrame.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
         this.mnuAddFrame.Text = "Add &Frame to State";
         this.mnuAddFrame.Click += new System.EventHandler(this.mnuAddFrame_Click);
         // 
         // mnuRemoveFrame
         // 
         this.mnuRemoveFrame.Index = 5;
         this.mnuRemoveFrame.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
         this.mnuRemoveFrame.Text = "&Remove Frame from State";
         this.mnuRemoveFrame.Click += new System.EventHandler(this.mnuRemoveFrame_Click);
         // 
         // mnuPreviewAnimation
         // 
         this.mnuPreviewAnimation.Index = 6;
         this.mnuPreviewAnimation.Text = "&Preview State Animation";
         this.mnuPreviewAnimation.Click += new System.EventHandler(this.OnPreviewAnimation);
         // 
         // mnuSpriteDefSeparator
         // 
         this.mnuSpriteDefSeparator.Index = 7;
         this.mnuSpriteDefSeparator.Text = "-";
         // 
         // mnuAddAction
         // 
         this.mnuAddAction.Index = 8;
         this.mnuAddAction.Text = "Add Ru&le";
         this.mnuAddAction.Click += new System.EventHandler(this.OnAddRule);
         // 
         // mnuRemoveRule
         // 
         this.mnuRemoveRule.Index = 9;
         this.mnuRemoveRule.Text = "Remo&ve Rule";
         this.mnuRemoveRule.Click += new System.EventHandler(this.OnDeleteRule);
         // 
         // mnuMoveRuleUp
         // 
         this.mnuMoveRuleUp.Index = 10;
         this.mnuMoveRuleUp.Text = "Move Rule &Up";
         this.mnuMoveRuleUp.Click += new System.EventHandler(this.OnMoveRuleUp);
         // 
         // mnuMoveRuleDown
         // 
         this.mnuMoveRuleDown.Index = 11;
         this.mnuMoveRuleDown.Text = "&Move Rule Down";
         this.mnuMoveRuleDown.Click += new System.EventHandler(this.OnMoveRuleDown);
         // 
         // mnuCopyRules
         // 
         this.mnuCopyRules.Index = 12;
         this.mnuCopyRules.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuCopyRuleChildren,
            this.mnuCopyRuleOnly,
            this.mnuCopyAllRules});
         this.mnuCopyRules.Text = "&Copy Rule";
         // 
         // mnuCopyRuleChildren
         // 
         this.mnuCopyRuleChildren.Index = 0;
         this.mnuCopyRuleChildren.Text = "&Copy Selected Rule Including Children";
         this.mnuCopyRuleChildren.Click += new System.EventHandler(this.mnuCopyRules_Click);
         // 
         // mnuCopyRuleOnly
         // 
         this.mnuCopyRuleOnly.Index = 1;
         this.mnuCopyRuleOnly.Text = "Copy &Selected Rule Only";
         this.mnuCopyRuleOnly.Click += new System.EventHandler(this.mnuCopyRules_Click);
         // 
         // mnuCopyAllRules
         // 
         this.mnuCopyAllRules.Index = 2;
         this.mnuCopyAllRules.Text = "Copy &All Rules";
         this.mnuCopyAllRules.Click += new System.EventHandler(this.mnuCopyRules_Click);
         // 
         // mnuCutRules
         // 
         this.mnuCutRules.Index = 13;
         this.mnuCutRules.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuCutRuleChildren,
            this.mnuCutRuleOnly,
            this.mnuCutAllRules});
         this.mnuCutRules.Text = "Cu&t Rule";
         // 
         // mnuCutRuleChildren
         // 
         this.mnuCutRuleChildren.Index = 0;
         this.mnuCutRuleChildren.Text = "&Cut Selected Rule Including Children";
         this.mnuCutRuleChildren.Click += new System.EventHandler(this.mnuCopyRules_Click);
         // 
         // mnuCutRuleOnly
         // 
         this.mnuCutRuleOnly.Index = 1;
         this.mnuCutRuleOnly.Text = "Cut &Selected Rule Only";
         this.mnuCutRuleOnly.Click += new System.EventHandler(this.mnuCopyRules_Click);
         // 
         // mnuCutAllRules
         // 
         this.mnuCutAllRules.Index = 2;
         this.mnuCutAllRules.Text = "Cut &All Rules";
         this.mnuCutAllRules.Click += new System.EventHandler(this.mnuCopyRules_Click);
         // 
         // mnuPasteRules
         // 
         this.mnuPasteRules.Enabled = false;
         this.mnuPasteRules.Index = 14;
         this.mnuPasteRules.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuPasteRuleAbove,
            this.mnuPasteRuleBelow});
         this.mnuPasteRules.Text = "Past&e Rules";
         // 
         // mnuPasteRuleAbove
         // 
         this.mnuPasteRuleAbove.Index = 0;
         this.mnuPasteRuleAbove.Text = "Paste &Above Selected Rule";
         this.mnuPasteRuleAbove.Click += new System.EventHandler(this.mnuPasteRules_Click);
         // 
         // mnuPasteRuleBelow
         // 
         this.mnuPasteRuleBelow.Index = 1;
         this.mnuPasteRuleBelow.Text = "Paste &Below Selected Rule";
         this.mnuPasteRuleBelow.Click += new System.EventHandler(this.mnuPasteRules_Click);
         // 
         // mnuToggleSuspend
         // 
         this.mnuToggleSuspend.Index = 15;
         this.mnuToggleSuspend.Text = "To&ggle Suspend for This and Child Rules";
         this.mnuToggleSuspend.Click += new System.EventHandler(this.OnToggleSuspend);
         // 
         // mnuConvertToFunc
         // 
         this.mnuConvertToFunc.Index = 16;
         this.mnuConvertToFunc.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuConvertAllRules,
            this.mnuConvertSelectedRule});
         this.mnuConvertToFunc.Text = "Convert to Fu&nction";
         // 
         // mnuConvertAllRules
         // 
         this.mnuConvertAllRules.Index = 0;
         this.mnuConvertAllRules.Text = "Convert &All Enabled Rules";
         this.mnuConvertAllRules.Click += new System.EventHandler(this.mnuConvertToFunc_Click);
         // 
         // mnuConvertSelectedRule
         // 
         this.mnuConvertSelectedRule.Index = 1;
         this.mnuConvertSelectedRule.Text = "Convert &Selected Rule and Children";
         this.mnuConvertSelectedRule.Click += new System.EventHandler(this.mnuConvertToFunc_Click);
         // 
         // mnuSpriteDefSeparator2
         // 
         this.mnuSpriteDefSeparator2.Index = 17;
         this.mnuSpriteDefSeparator2.Text = "-";
         // 
         // mnuExport
         // 
         this.mnuExport.Index = 18;
         this.mnuExport.Text = "E&xport to template...";
         this.mnuExport.Click += new System.EventHandler(this.mnuExport_Click);
         // 
         // mnuSpriteDefSeparator3
         // 
         this.mnuSpriteDefSeparator3.Index = 19;
         this.mnuSpriteDefSeparator3.Text = "-";
         // 
         // mnuRotateWizard
         // 
         this.mnuRotateWizard.Index = 20;
         this.mnuRotateWizard.Text = "R&otating Sprite State Wizard";
         this.mnuRotateWizard.Click += new System.EventHandler(this.mnuRotateWizard_Click);
         // 
         // DataMonitor
         // 
         this.DataMonitor.SpriteDefinitionRowDeleted += new SGDK2.ProjectDataset.SpriteDefinitionRowChangeEventHandler(this.DataMonitor_SpriteDefinitionRowDeleted);
         this.DataMonitor.SpriteParameterRowChanged += new SGDK2.ProjectDataset.SpriteParameterRowChangeEventHandler(this.DataMonitor_SpriteParameterRowChanged);
         this.DataMonitor.SpriteRuleRowDeleting += new SGDK2.ProjectDataset.SpriteRuleRowChangeEventHandler(this.dataMonitor_SpriteRuleRowChanging);
         this.DataMonitor.SpriteRuleRowChanged += new SGDK2.ProjectDataset.SpriteRuleRowChangeEventHandler(this.dataMonitor_SpriteRuleRowChanged);
         this.DataMonitor.SpriteStateRowChanging += new SGDK2.ProjectDataset.SpriteStateRowChangeEventHandler(this.DataMonitor_SpriteStateRowChanging);
         this.DataMonitor.FramesetRowDeleted += new SGDK2.ProjectDataset.FramesetRowChangeEventHandler(this.DataMonitor_FramesetRowDeleted);
         this.DataMonitor.Clearing += new System.EventHandler(this.DataMonitor_Clearing);
         this.DataMonitor.SpriteStateRowDeleted += new SGDK2.ProjectDataset.SpriteStateRowChangeEventHandler(this.DataMonitor_SpriteStateRowDeleted);
         this.DataMonitor.FramesetRowChanged += new SGDK2.ProjectDataset.FramesetRowChangeEventHandler(this.DataMonitor_FramesetRowChanged);
         this.DataMonitor.FramesetRowChanging += new SGDK2.ProjectDataset.FramesetRowChangeEventHandler(this.DataMonitor_FramesetRowChanging);
         this.DataMonitor.SpriteRuleRowChanging += new SGDK2.ProjectDataset.SpriteRuleRowChangeEventHandler(this.dataMonitor_SpriteRuleRowChanging);
         this.DataMonitor.SpriteStateRowChanged += new SGDK2.ProjectDataset.SpriteStateRowChangeEventHandler(this.DataMonitor_SpriteStateRowChanged);
         this.DataMonitor.SpriteRuleRowDeleted += new SGDK2.ProjectDataset.SpriteRuleRowChangeEventHandler(this.dataMonitor_SpriteRuleRowChanged);
         // 
         // tmrPopulateRules
         // 
         this.tmrPopulateRules.Tick += new System.EventHandler(this.tmrPopulateRules_Tick);
         // 
         // frmSpriteDefinition
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(565, 417);
         this.Controls.Add(this.tabSpriteDefinition);
         this.Controls.Add(this.pnlSpriteHeader);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Menu = this.mnuSprites;
         this.Name = "frmSpriteDefinition";
         this.Text = "Sprite Definition";
         this.tabSpriteDefinition.ResumeLayout(false);
         this.tabStates.ResumeLayout(false);
         this.pnlFrames.ResumeLayout(false);
         this.pnlFrameDetails.ResumeLayout(false);
         this.pnlFrameDetails.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.updRepeatCount)).EndInit();
         this.pnlSpriteState.ResumeLayout(false);
         this.pnlSpriteState.PerformLayout();
         this.pnlStateList.ResumeLayout(false);
         this.pnlStateList.PerformLayout();
         this.tabParameters.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.grdParameters)).EndInit();
         this.tabRules.ResumeLayout(false);
         this.tabRules.PerformLayout();
         this.pnlRules.ResumeLayout(false);
         this.pnlRules.PerformLayout();
         this.pnlSpriteHeader.ResumeLayout(false);
         this.pnlSpriteHeader.PerformLayout();
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
               reflector = CodeGenerator.CreateInstanceAndUnwrap("RemoteReflector",
                  CodeGenerator.SpritesNamespace + "." + CodeGenerator.NameToVariable(m_SpriteDef.Name))
                  as RemotingServices.IRemoteTypeInfo;
            }
            catch (System.Exception)
            {
               reflector = CodeGenerator.CreateInstanceAndUnwrap("RemoteReflector", "SpriteBase")
                  as RemotingServices.IRemoteTypeInfo;
            }

            RemotingServices.RemoteMethodInfo[] localRuleList = reflector.GetMethods();
            RemotingServices.RemoteMethodInfo[] globalRuleList = reflector.GetGlobalFunctions();
            RemotingServices.RemoteMethodInfo[] ruleList = new SGDK2.RemotingServices.RemoteMethodInfo[localRuleList.Length + globalRuleList.Length];
            localRuleList.CopyTo(ruleList, 0);
            globalRuleList.CopyTo(ruleList, localRuleList.Length);

            m_AvailableRules = new RuleTable();
            m_Enums = new EnumTable();
            m_SpriteProperties = reflector.GetProperties();
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
            if (!onlyBools)
               chkNot.Checked = false;
            chkNot.Enabled = onlyBools;

            PopulateParameter(lblParam1, cboParam1, RemotingServices.RemoteParameterInfo.Empty);
            PopulateParameter(lblParam2, cboParam2, RemotingServices.RemoteParameterInfo.Empty);
            PopulateParameter(lblParam3, cboParam3, RemotingServices.RemoteParameterInfo.Empty);
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
         try
         {
            RemotingServices.IRemoteTypeInfo reflector = CodeGenerator.CreateInstanceAndUnwrap(
               "RemoteReflector", TypeName) as RemotingServices.IRemoteTypeInfo;
            m_CustomObjects[TypeName] = reflector.GetGlobalProvidersOfSelf();
         }
         catch(System.Exception ex)
         {
            txtErrors.Text = ex.Message;
            txtErrors.Visible = true;
            return;
         }
      }

      private EnumTable.EnumDetails GetEnumInfo(string enumName)
      {
         try
         {
            string errs;
            CodeGenerator gen = new CodeGenerator();
            gen.GenerateLevel = CodeGenerator.CodeLevel.ExcludeRules;
            errs = gen.CompileTempAssembly(false);
            if ((errs != null) && (errs.Length > 0))
            {
               txtErrors.Text = errs;
               txtErrors.Visible = true;
               return new EnumTable.EnumDetails();
            }
            else
            {
               txtErrors.Text = String.Empty;
               txtErrors.Visible = false;
            }

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

      private void FillBaseClasses()
      {
         try
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

            RemotingServices.IRemoteTypeInfo reflector = CodeGenerator.CreateInstanceAndUnwrap(
               "RemoteReflector", CodeGenerator.SpriteBaseClass) as RemotingServices.IRemoteTypeInfo;
            RemotingServices.RemoteTypeName[] bases = reflector.GetDerivedClasses(true);
            cboBaseClass.Items.Add(CodeGenerator.SpriteBaseClass);
            for(int i = 0; i < bases.Length; i++)
               cboBaseClass.Items.Add(bases[i].FullName);
         }
         catch(System.Exception ex)
         {
            txtErrors.Text = ex.Message;
            txtErrors.Visible = true;
         }
      }

      private void FillFramesets()
      {
         cboFrameset.Items.Clear();
         foreach(System.Data.DataRowView drv in ProjectData.Frameset.DefaultView)
         {
            ProjectDataset.FramesetRow fr = (ProjectDataset.FramesetRow)drv.Row;
            cboFrameset.Items.Add(fr);
         }
      }

      private void FillStates()
      {
         lstSpriteStates.Items.Clear();
         foreach(ProjectDataset.SpriteStateRow sr in ProjectData.GetSortedSpriteStates(m_SpriteDef))
         {
            lstSpriteStates.Items.Add(sr);
         }
      }

      private void LoadState(ProjectDataset.SpriteStateRow state)
      {
         txtStateName.Text = state.Name;
         for (int i=0; i<cboFrameset.Items.Count; i++)
         {
            if (cboFrameset.Items[i] == state.FramesetRow)
            {
               cboFrameset.SelectedIndex = i;
               break;
            }
         }

         txtWidth.Text = state.SolidWidth.ToString();
         txtHeight.Text = state.SolidHeight.ToString();
         AvailableFrames.Frameset = StateFrames.Frameset = state.FramesetRow;
         LoadSpriteFrames(state);
         // Ensure that repeat/delay field gets reset
         if (StateFrames.CellCount > 0)
            StateFrames.CurrentCellIndex = 0;
      }

      private void LoadSpriteFrames(ProjectDataset.SpriteStateRow state)
      {
         FrameList lst = new FrameList();
         foreach(ProjectDataset.SpriteFrameRow frame in ProjectData.GetSortedSpriteFrames(state))
         {
            lst.Add(new SpriteFrame(frame));
         }
         StateFrames.FramesToDisplay = lst;
      }

      private ProjectDataset.SpriteStateRow GetCurrentState()
      {
         return (ProjectDataset.SpriteStateRow)lstSpriteStates.SelectedItem;
      }

      private void EnableState(bool enable)
      {
         txtStateName.Enabled = lblStateName.Enabled = cboFrameset.Enabled =
            lblFrameset.Enabled = lblStateFrames.Enabled = StateFrames.Enabled = lblAvailableFrames.Enabled =
            AvailableFrames.Enabled = lblRepeatCount.Enabled = updRepeatCount.Enabled =
            btnMaskAlpha.Enabled = lblMaskAlpha.Enabled = txtMaskAlpha.Enabled =
            txtWidth.Enabled = txtHeight.Enabled = lblWidthHeight.Enabled = lblComma.Enabled =
            enable;
      }

      private void InitParameters()
      {
         grdParameters.SetDataBinding(ProjectData.SpriteDefinition, "SpriteDefinitionSpriteParameter");
         CurrencyManager cur = (CurrencyManager)this.BindingContext[ProjectData.SpriteDefinition];
         cur.Position = ProjectData.SpriteDefinition.DefaultView.Find(m_SpriteDef.Name);
         DataGridTableStyle ts = new DataGridTableStyle();
         ts.MappingName = "SpriteParameter";
         /*PropertyDescriptorCollection pdc = BindingContext[ProjectData.SpriteParameter, null].GetItemProperties();

         DataGridTextBoxColumn tbcol = new System.Windows.Forms.DataGridTextBoxColumn(pdc["Name"]);
         tbcol.MappingName = "Name";
         tbcol.HeaderText = "Name";
         ts.GridColumnStyles.Add(tbcol);

         DataGridComboBoxColumn cbcol = new DataGridComboBoxColumn(pdc["Type"], new string[] {"Int", "Decimal", "Sprite", "SpritePlan", "Area"});
         cbcol.MappingName = "Type";
         cbcol.HeaderText = "Type";
         ts.GridColumnStyles.Add(cbcol);*/

         grdParameters.TableStyles.Add(ts);
         grdParameters.TableStyles["SpriteParameter"].GridColumnStyles["Name"].Width = 300;
         ProjectData.SpriteParameter.ColumnChanging += new System.Data.DataColumnChangeEventHandler(SpriteParameter_ColumnChanging);
      }

      private void PrepareFunction(string funcName)
      {
         if (m_PreparedFunction == funcName)
            return;

         if ((m_AvailableRules == null) || !m_AvailableRules.Contains(funcName))
         {
            PopulateParameter(lblParam1, cboParam1, RemotingServices.RemoteParameterInfo.Unknown);
            PopulateParameter(lblParam2, cboParam2, RemotingServices.RemoteParameterInfo.Unknown);
            PopulateParameter(lblParam3, cboParam3, RemotingServices.RemoteParameterInfo.Unknown);
            lblOutput.Enabled = true;
            cboOutput.Enabled = true;
            txtHelpText.Text = "The specified function name could not be located or the project failed to compile.";
            m_PreparedFunction = string.Empty;
            return;
         }

         RemotingServices.RemoteMethodInfo mi = m_AvailableRules[funcName];
         
         txtHelpText.Text = mi.Description;
         
         if (mi.Arguments.Length <= 0)
            PopulateParameter(lblParam1, cboParam1, RemotingServices.RemoteParameterInfo.Empty);
         else
            PopulateParameter(lblParam1, cboParam1, mi.Arguments[0]);
         if (mi.Arguments.Length <= 1)
            PopulateParameter(lblParam2, cboParam2, RemotingServices.RemoteParameterInfo.Empty);
         else
            PopulateParameter(lblParam2, cboParam2, mi.Arguments[1]);
         if (mi.Arguments.Length <= 2)
            PopulateParameter(lblParam3, cboParam3, RemotingServices.RemoteParameterInfo.Empty);
         else
            PopulateParameter(lblParam3, cboParam3, mi.Arguments[2]);

         if ((string.Compare(mi.ReturnType.Name, typeof(Int32).Name) == 0) ||
            (string.Compare(mi.ReturnType.Name, typeof(Int16).Name) == 0))
         {
            lblOutput.Enabled = true;
            cboOutput.Enabled = true;
            cboOutput.Items.Clear();
            FillComboWithParams(cboOutput, false);
            FillComboWithIntVars(cboOutput, false);
            FillComboWithNumberMembers(cboOutput, true, mi.ReturnType.FullName);
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

      private void FillComboWithParams(ComboBox cboParams, bool isRef)
      {
         foreach(ProjectDataset.SpriteParameterRow prow in m_SpriteDef.GetSpriteParameterRows())
         {
            cboParams.Items.Add((isRef ? "ref " : "") + CodeGenerator.NameToVariable(prow.Name));
         }
      }

      private void FillComboWithIntVars(ComboBox cboTarget, bool isRef)
      {
         foreach (DataRowView drv in ProjectData.Counter.DefaultView)
            cboTarget.Items.Add((isRef ? "ref " : "") + CodeGenerator.CounterClass + "." + CodeGenerator.NameToVariable(
               ((ProjectDataset.CounterRow)drv.Row).Name) + ".CurrentValue");
      }
      
      private void FillComboWithCounters(ComboBox cboTarget)
      {
         foreach (DataRowView drv in ProjectData.Counter.DefaultView)
            cboTarget.Items.Add(CodeGenerator.CounterClass + "." + CodeGenerator.NameToVariable(
               ((ProjectDataset.CounterRow)drv.Row).Name));
      }

      private void FillComboWithSpriteInstances(ComboBox cboTarget)
      {
         cboTarget.Items.Add("this");
         cboTarget.Items.Add("lastCreatedSprite");
         foreach (RemotingServices.RemotePropertyInfo pi in m_SpriteProperties)
            if (pi.Type.Name == CodeGenerator.SpriteBaseClass)
               cboTarget.Items.Add(CodeGenerator.NameToVariable(pi.Name));
      }

      private void FillComboWithSolidity(ComboBox cboTarget)
      {
         foreach (DataRowView drv in ProjectData.Solidity.DefaultView)
         {
            cboTarget.Items.Add(CodeGenerator.SolidityClassName + "." + CodeGenerator.NameToVariable(
               ((ProjectDataset.SolidityRow)drv.Row).Name));
         }
      }
      
      private void FillComboWithNumberMembers(ComboBox cboTarget, bool forOutput, string targetType)
      {
         foreach(RemotingServices.RemotePropertyInfo pi in m_SpriteProperties)
         {
            if ((pi.Type.Name == typeof(System.Int32).Name) ||
               (pi.Type.Name == typeof(System.Int16).Name) ||
               (pi.Type.Name == typeof(System.Single).Name) ||
               (pi.Type.Name == typeof(System.Double).Name))
            {
               if (forOutput && ((pi.Flags & RemotingServices.MemberFlags.CanWrite) != 0))
               {
                  if ((pi.Type.Name == typeof(System.Single).Name) || 
                     (pi.Type.Name == typeof(System.Double).Name) ||
                     (targetType == pi.Type.FullName))
                     cboTarget.Items.Add(pi.Name);
               }
               else if (!forOutput && ((pi.Flags & RemotingServices.MemberFlags.CanRead) != 0))
               {
                  cboTarget.Items.Add(pi.Name);
               }
            }
         }
      }

      private void FillComboWithSpriteStates(ComboBox cboTarget)
      {
         foreach(ProjectDataset.SpriteStateRow drState in ProjectData.GetSortedSpriteStates(m_SpriteDef))
            cboTarget.Items.Add("(int)" + CodeGenerator.SpritesNamespace + "." +
               CodeGenerator.NameToVariable(m_SpriteDef.Name) + "." +
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

      private void FillComboWithSpriteCollections(ComboBox cboTarget)
      {
         foreach(DataRowView drv in ProjectData.SpriteCategory.DefaultView)
         {
            cboTarget.Items.Add("ParentLayer." + CodeGenerator.SpriteCategoriesFieldName + "." + CodeGenerator.NameToVariable(
               ((ProjectDataset.SpriteCategoryRow)drv.Row).Name));
         }
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

      private void PopulateParameter(Label lblParameter, ComboBox cboParameter, RemotingServices.RemoteParameterInfo param)
      {
         cboParameter.Items.Clear();
         cboParameter.Text = string.Empty;
         cboParameter.SelectedIndex = -1;

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

         if (param.Editors != null)
         {
            foreach(string editor in param.Editors)
            {
               switch(editor)
               {
                  case "SpriteState":
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
         else if (string.Compare(param.Type.Name, CodeGenerator.SpriteCollectionClassName) == 0)
         {
            FillComboWithSpriteCollections(cboParameter);
         }
         else if (string.Compare(param.Type.Name, CodeGenerator.TilesetClass) == 0)
         {
            FillComboWithTilesets(cboParameter);
         }
         else if (string.Compare(param.Type.FullName, typeof(Int32).FullName + "&") == 0)
         {
            // Integer passed by reference
            FillComboWithParams(cboParameter, true);
            FillComboWithIntVars(cboParameter, true);
         }
         else if(string.Compare(param.Type.Name, "Counter") == 0)
         {
            FillComboWithCounters(cboParameter);
         }
         else if((string.Compare(param.Type.Name, "SpriteBase") == 0) || (param.Type.FullName.EndsWith("Sprites." + m_SpriteDef.Name)))
         {
            FillComboWithSpriteInstances(cboParameter);
         }
         else if(string.Compare(param.Type.Name, "Solidity") == 0)
         {
            FillComboWithSolidity(cboParameter);
         }
         else
         {
            foreach(string typeName in new string[]
            {
               typeof(Int32).Name, typeof(Int16).Name,
               typeof(Double).Name, typeof(Single).Name
            })
            {
               if (string.Compare(typeName, param.Type.Name) == 0)
               {
                  FillComboWithParams(cboParameter, false);
                  FillComboWithIntVars(cboParameter, false);
                  FillComboWithNumberMembers(cboParameter, false, typeName);
                  break;
               }
            }
         }
      }
      
      private void LoadRule(ProjectDataset.SpriteRuleRow drRule)
      {
         try
         {
            m_Loading = true;
            txtRuleName.Text = drRule.Name;

            LoadFunctions(IsRuleTypeConditional(drRule.Type), false, true);

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

            LoadParameters(drRule);

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

      private void LoadParameters(ProjectDataset.SpriteRuleRow drRule)
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
            cboParam1.Text = drRule.Parameter1;

         if (drRule.IsParameter2Null())
            cboParam2.SelectedIndex = -1;
         else
            cboParam2.Text = drRule.Parameter2;

         if (drRule.IsParameter3Null())
            cboParam3.SelectedIndex = -1;
         else
            cboParam3.Text = drRule.Parameter3;

         if (drRule.IsResultParameterNull())
            cboOutput.SelectedIndex = -1;
         else
            cboOutput.Text = drRule.ResultParameter;
      }

      private void QueuePopulateRules()
      {
         tmrPopulateRules.Stop();
         tmrPopulateRules.Start();
      }

      private void PopulateRules()
      {
         ProjectDataset.SpriteRuleRow cur = CurrentRule;
         tvwRules.SelectedNode = null;
         tvwRules.Nodes.Clear();
         m_TreeNodes.Clear();
         TreeNode parentNode = null;
         foreach(ProjectDataset.SpriteRuleRow drRule in ProjectData.GetSortedSpriteRules(m_SpriteDef,true))
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

      private int GetRuleImage(ProjectDataset.SpriteRuleRow drRule)
      {
         int result;

         switch(drRule.Type)
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

         if (drRule.Suspended)
            return result + 8;
         else
            return result;
      }

      private ProjectDataset.SpriteRuleRow CurrentRule
      {
         get
         {
            if (tvwRules.SelectedNode != null)
               return ProjectData.GetSpriteRule(m_SpriteDef, tvwRules.SelectedNode.Text);
            return null;
         }
      }

      private TreeNode GetNodeFromRow(ProjectDataset.SpriteRuleRow drRule)
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
      private void SuspendSelection(bool suspend)
      {
         if (suspend)
            lstSpriteStates.SelectedIndexChanged -= new System.EventHandler(this.lstSpriteStates_SelectedIndexChanged);
         else
            lstSpriteStates.SelectedIndexChanged += new System.EventHandler(this.lstSpriteStates_SelectedIndexChanged);
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
         ProjectDataset.SpriteRuleRow drRule = CurrentRule;
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
         result.Add(new ProjectData.CopiedRule(ProjectData.GetSpriteRule(m_SpriteDef, parent.Text)));
         foreach(TreeNode child in parent.Nodes)
            result.AddRange(GetNodeWithChildList(child));
         return (ProjectData.CopiedRule[])result.ToArray(typeof(ProjectData.CopiedRule));
      }

      private void ConvertAllRules()
      {
         System.Collections.ArrayList nodes = new ArrayList();

         foreach(TreeNode rule in tvwRules.Nodes)
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
         ProjectDataset.SpriteRuleRow[] rules;
         ArrayList selectedRules = new ArrayList();
         foreach (ProjectData.CopiedRule rule in copiedRules)
         {
            if (!rule.Suspended)
               selectedRules.Add(ProjectData.GetSpriteRule(m_SpriteDef, rule.Name));
         }
         rules = (ProjectDataset.SpriteRuleRow[])selectedRules.ToArray(typeof(ProjectDataset.SpriteRuleRow));

         System.Collections.Specialized.StringCollection reservedNames = new System.Collections.Specialized.StringCollection();
         try
         {
            LoadFunctions(false, false, false);
            foreach (DictionaryEntry de in m_AvailableRules)
            {
               reservedNames.Add(de.Key.ToString());
            }
            foreach (RemotingServices.RemotePropertyInfo pi in m_SpriteProperties)
            {
               reservedNames.Add(pi.Name);
            }
            reservedNames.Add(CodeGenerator.NameToVariable(m_SpriteDef.Name));
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(this, "Error inspecting compiled project for list of reserved names (" + ex.Message + ")", "Convert Rules to Function", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
         reservedNames.Add("ExecuteRules");
         frmConvertToFunction frm = new frmConvertToFunction(rules, reservedNames);
         frm.ShowDialog(this);
         frm.Dispose();
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
         foreach (ProjectDataset.SpriteRuleRow drRule in ProjectData.GetSortedSpriteRules(m_SpriteDef, true))
         {
            drRule.Delete();
         }
      }

      private void DeleteRules(bool includeChildren)
      {
         ProjectDataset.SpriteRuleRow drRule = CurrentRule;
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
            m_SelectAfterLoad = ProjectData.GetSpriteRule(m_SpriteDef, selAfterDel.Text);
         if (includeChildren)
            DeleteRuleAndChildren(tvwRules.SelectedNode);
         else
            ProjectData.DeleteSpriteRule(drRule);
      }

      private void DeleteRuleAndChildren(TreeNode parent)
      {
         foreach (TreeNode child in parent.Nodes)
            DeleteRuleAndChildren(child);
         ProjectData.DeleteSpriteRule(ProjectData.GetSpriteRule(m_SpriteDef, parent.Text));
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
            if ((toPaste.Length > 0) && (!toPaste[0].IsSpriteDefinitionRule))
            {
               if (DialogResult.OK != MessageBox.Show(this, "The rules being pasted were copied from a plan. Plan rules may not be compatible with sprite definition rules, and may require corrections before they can function as such.", "Paste Rules", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                  return;
            }
            foreach(ProjectData.CopiedRule rule in toPaste)
            {
               string name = rule.Name;
               int i = 0;
               while (ProjectData.GetSpriteRule(m_SpriteDef, name) != null)
               {
                  name = rule.Name + (++i).ToString();
               }
               ProjectData.InsertSpriteRule(m_SpriteDef, name, rule.Type, sequence, rule.Function, rule.Parameter1, rule.Parameter2, rule.Parameter3, rule.ResultParameter, rule.EndIf, rule.Suspended);
               if (sequence >= 0)
                  sequence++;
            }
         }
         else
            EnablePasteRules();
      }
      #endregion

      #region Public Static Members
      public static void Edit(Form MdiParent, ProjectDataset.SpriteDefinitionRow EditRow)
      {
         foreach(Form frm in MdiParent.MdiChildren)
         {
            frmSpriteDefinition f = frm as frmSpriteDefinition;
            if (f != null)
            {
               if (f.m_SpriteDef == EditRow)
               {
                  f.Activate();
                  return;
               }
            }
         }

         frmSpriteDefinition frmNew = new frmSpriteDefinition(EditRow);
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

      #region Event Handlers
      private void txtName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         string sValid = ProjectData.ValidateName(txtName.Text);

         if (sValid != null)
         {
            if (DialogResult.Cancel == MessageBox.Show(this, sValid, "Sprite Definition Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtName.Text = m_SpriteDef.Name;
            e.Cancel = true;
         }
         ProjectDataset.SpriteDefinitionRow sr = ProjectData.GetSpriteDefinition(txtName.Text);
         if ((null != sr) && (m_SpriteDef != sr))
         {
            if (DialogResult.Cancel == MessageBox.Show(this, txtName.Text + " already exists", "Sprite Definition Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtName.Text = m_SpriteDef.Name;
            e.Cancel = true;
         }
      }

      private void DataMonitor_SpriteDefinitionRowDeleted(object sender, SGDK2.ProjectDataset.SpriteDefinitionRowChangeEvent e)
      {
         if (e.Row == m_SpriteDef)
            this.Close();
      }

      private void DataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void txtName_Validated(object sender, System.EventArgs e)
      {
         if ((m_SpriteDef.RowState != DataRowState.Deleted) && (m_SpriteDef.RowState != DataRowState.Detached) &&
            (m_SpriteDef.Name != txtName.Text))
         {
            ProjectData.EnforceConstraints = false;
            m_SpriteDef.Name = txtName.Text;
            ProjectData.EnforceConstraints = true;
            this.BindingContext = new BindingContext();
            ((CurrencyManager)this.BindingContext[ProjectData.SpriteDefinition]).Position =
               ProjectData.SpriteDefinition.DefaultView.Find(m_SpriteDef.Name);
         }
      }

      private void OnAddState(object sender, System.EventArgs e)
      {
         if (ProjectData.Frameset.DefaultView.Count <= 0)
         {
            MessageBox.Show(this, "Please create a frameset before creating sprite states", "Add Sprite State", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); 
            return;
         }
         String sName;
         Int32 nIdx = 1;
         do
            sName = "New Sprite State " + (nIdx++).ToString();
         while (ProjectData.GetSpriteState(m_SpriteDef.Name, sName) != null);

         short insertIndex;
         if (lstSpriteStates.SelectedItem is ProjectDataset.SpriteStateRow)
            insertIndex = (short)(((ProjectDataset.SpriteStateRow)lstSpriteStates.SelectedItem).Sequence + 1);
         else
            insertIndex = -1;

         lstSpriteStates.SelectedItem = ProjectData.AddSpriteState(m_SpriteDef, sName,
            (ProjectDataset.FramesetRow)ProjectData.Frameset.DefaultView[0].Row,
            32, 32, insertIndex);
      }

      private void StateFrames_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
      {
         GraphicBrowser gb = e.Data.GetData(typeof(GraphicBrowser)) as GraphicBrowser;
         if (gb == null)
            return;

         if (gb == StateFrames)
         {
            if (0 != (e.KeyState & 8))
               e.Effect = System.Windows.Forms.DragDropEffects.Copy;
            else
               e.Effect = System.Windows.Forms.DragDropEffects.Move;
         }
         else if (gb.Frameset == AvailableFrames.Frameset)
            e.Effect = System.Windows.Forms.DragDropEffects.Copy;
      }

      private void StateFrames_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
      {
         GraphicBrowser gb = e.Data.GetData(typeof(GraphicBrowser)) as GraphicBrowser;

         Point ptDrop = StateFrames.PointToClient(new Point(e.X,e.Y));
         short nCellIndex = (short)StateFrames.GetCellAtXY(ptDrop.X, ptDrop.Y,
            HitFlags.AllowExtraCell | HitFlags.GetNearest);

         try
         {
            ProjectDataset.SpriteStateRow row =  GetCurrentState();

            if (e.Effect == DragDropEffects.Move)
            {
               for (int idx = 0; idx < StateFrames.FramesToDisplay.Count; idx++)
               {
                  if (StateFrames.FramesToDisplay[idx].IsSelected)
                  {
                     short nNewIndex = nCellIndex;
                     if (nCellIndex > idx)
                        nNewIndex--;
                     ProjectData.MoveSpriteFrame((StateFrames.FramesToDisplay[idx] as
                        SpriteFrame).Row, nNewIndex);
                     IProvideFrame fra = StateFrames.FramesToDisplay[idx];
                     StateFrames.FramesToDisplay.RemoveAt(idx);
                     StateFrames.FramesToDisplay.Insert(nNewIndex, fra);
                     StateFrames.CurrentCellIndex = nNewIndex;
                  }
               }
            }
            else
            {
               foreach (SGDK2.ProjectDataset.FrameRow fr in gb.GetSelectedFrames())
               {
                  double dblAlpha;
                  byte byAlpha = 0;
                  if (Double.TryParse(txtMaskAlpha.Text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out dblAlpha))
                  {
                     byAlpha = (byte)dblAlpha;
                  }
                  ProjectDataset.SpriteFrameRow sfr = SGDK2.ProjectData.InsertFrame(
                     row, nCellIndex, fr.FrameValue, (short)updRepeatCount.Value, byAlpha);
                  StateFrames.FramesToDisplay.Insert(nCellIndex++, new SpriteFrame(sfr));
               }
            }
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(ex.Message, "Drag Sprite Frames", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
         StateFrames.Invalidate();      
      }

      private void cboFrameset_SelectionChangeCommitted(object sender, System.EventArgs e)
      {
         try
         {
            ProjectDataset.FramesetRow fr = (ProjectDataset.FramesetRow)cboFrameset.SelectedItem;
            ProjectDataset.SpriteStateRow sr = GetCurrentState();
            if ((fr != sr.FramesetRow) && (sr.GetSpriteFrameRows().Length > 0))
            {
               switch (MessageBox.Show(this, "The Frameset has been changed.  Do you want to delete all frames for this state?", "Confirm Frameset Change", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3))
               {
                  case DialogResult.Yes:
                     foreach (ProjectDataset.SpriteFrameRow sfr in ProjectData.GetSortedSpriteFrames(sr))
                        sfr.Delete();
                     break;
                  case DialogResult.Cancel:
                     cboFrameset.SelectedItem = sr.FramesetRow;
                     return;
               }
            }
            AvailableFrames.Frameset = StateFrames.Frameset = sr.FramesetRow = fr;
            LoadSpriteFrames(sr);
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void updRepeatCount_ValueChanged(object sender, System.EventArgs e)
      {
         try
         {
            if ((StateFrames.CurrentCellIndex < 0) || m_Loading)
               return;
            for(int i=0; i < StateFrames.CellCount; i++)
               if ((StateFrames.Selected[i]) && (((SpriteFrame)StateFrames.FramesToDisplay[i]).Row.Duration != (short)updRepeatCount.Value))
                  ((SpriteFrame)StateFrames.FramesToDisplay[i]).Row.Duration = (short)updRepeatCount.Value;
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void StateFrames_CurrentCellChanged(object sender, System.EventArgs e)
      {
         try
         {
            if (StateFrames.CurrentCellIndex >= 0)
            {
               m_Loading = true;
               updRepeatCount.Value = ((SpriteFrame)(StateFrames.FramesToDisplay[StateFrames.CurrentCellIndex])).Row.Duration;
               txtMaskAlpha.Text = ((SpriteFrame)(StateFrames.FramesToDisplay[StateFrames.CurrentCellIndex])).Row.MaskAlphaLevel.ToString();
               m_Loading = false;
            }
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(ex.Message, "Load Sprite Frame Delay", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }      
      }

      private void DataMonitor_SpriteStateRowChanged(object sender, SGDK2.ProjectDataset.SpriteStateRowChangeEvent e)
      {
         if ((e.Action == System.Data.DataRowAction.Add) && (e.Row.SpriteDefinitionRow == m_SpriteDef))
         {
            for (int i = 0; i < lstSpriteStates.Items.Count; i++)
               if (((ProjectDataset.SpriteStateRow)lstSpriteStates.Items[i]).Sequence > e.Row.Sequence)
               {
                  lstSpriteStates.Items.Insert(i, e.Row);
                  return;
               }
            lstSpriteStates.Items.Add(e.Row);
         }
      }

      private void DataMonitor_SpriteStateRowChanging(object sender, SGDK2.ProjectDataset.SpriteStateRowChangeEvent e)
      {
         if ((e.Action == System.Data.DataRowAction.Change) && (e.Row.SpriteDefinitionRow == m_SpriteDef))
         {
            if (!e.Row.HasVersion(System.Data.DataRowVersion.Current) ||
               (string.Compare(e.Row["Name", System.Data.DataRowVersion.Current].ToString(), e.Row.Name) != 0) ||
               ((short)(e.Row["Sequence",System.Data.DataRowVersion.Current]) != e.Row.Sequence))
            {
               for (int i=0; i<lstSpriteStates.Items.Count; i++)
               {
                  if (lstSpriteStates.Items[i] == e.Row)
                  {
                     if ((i > 0) &&
                        ((ProjectDataset.SpriteStateRow)lstSpriteStates.Items[i-1]).Sequence >
                        ((ProjectDataset.SpriteStateRow)lstSpriteStates.Items[i]).Sequence)
                     {
                        object tmp = lstSpriteStates.Items[i-1];
                        lstSpriteStates.Items[i-1] = lstSpriteStates.Items[i];
                        lstSpriteStates.Items[i] = tmp;
                     }
                     else if ((i < lstSpriteStates.Items.Count - 1) && 
                        ((ProjectDataset.SpriteStateRow)lstSpriteStates.Items[i]).Sequence >
                        ((ProjectDataset.SpriteStateRow)lstSpriteStates.Items[i+1]).Sequence)
                     {
                        object tmp = lstSpriteStates.Items[i];
                        lstSpriteStates.Items[i] = lstSpriteStates.Items[i+1];
                        lstSpriteStates.Items[i+1] = tmp;
                     }
                     else
                        lstSpriteStates.Items[i] = e.Row; // Update name
                  }
               }
            }
         }
      }

      private void DataMonitor_SpriteStateRowDeleted(object sender, SGDK2.ProjectDataset.SpriteStateRowChangeEvent e)
      {
         if (lstSpriteStates.Items.Contains(e.Row))
         {
            if (lstSpriteStates.SelectedItems.Contains(e.Row))
               StateFrames.FramesToDisplay.Clear();
            lstSpriteStates.Items.Remove(e.Row);
         }
      }

      private void txtStateName_Validated(object sender, System.EventArgs e)
      {
         ProjectDataset.SpriteStateRow drState = GetCurrentState();
         if (drState == null)
            return;
         if (String.Compare(drState.Name, txtStateName.Text) == 0)
            return;
         drState.Name = txtStateName.Text;
      }

      private void txtStateName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         string sValid = ProjectData.ValidateName(txtStateName.Text);

         if (sValid != null)
         {
            if (DialogResult.Cancel == MessageBox.Show(this, sValid, "Sprite State Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtStateName.Text = GetCurrentState().Name;
            e.Cancel = true;
         }
         ProjectDataset.SpriteStateRow sr = ProjectData.GetSpriteState(m_SpriteDef.Name, txtStateName.Text);
         if ((null != sr) && (GetCurrentState() != sr))
         {
            if (DialogResult.Cancel == MessageBox.Show(this, txtStateName.Text + " already exists", "Sprite State Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtStateName.Text = GetCurrentState().Name;
            e.Cancel = true;
         }      
      }

      private void txtWidth_Validated(object sender, System.EventArgs e)
      {
         GetCurrentState().SolidWidth = short.Parse(txtWidth.Text);
      }

      private void txtHeight_Validated(object sender, System.EventArgs e)
      {
         GetCurrentState().SolidHeight = short.Parse(txtHeight.Text);      
      }

      private void txtSize_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         double dblResult;
         if (!Double.TryParse(((TextBox)sender).Text, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out dblResult))
         {
            MessageBox.Show(this, ((TextBox)sender).Text + " is not a valid number.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
            return;
         }

         if (dblResult > 9999)
            ((TextBox)sender).Text = "9999";
         if (dblResult < 1)
            ((TextBox)sender).Text = "1";
      }

      private void lstSpriteStates_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (lstSpriteStates.SelectedIndex < 0)
         {
            EnableState(false);
            return;
         }
         EnableState(true);
         LoadState((ProjectDataset.SpriteStateRow)lstSpriteStates.SelectedItem);
      }

      private void OnDeleteState(object sender, System.EventArgs e)
      {
         if (GetCurrentState() == null)
         {
            MessageBox.Show(this, "Please select a state to delete first", "Delete Sprite State", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }

         ProjectData.DeleteSpriteState(GetCurrentState());
      }

      private void OnMoveStateUp(object sender, System.EventArgs e)
      {
         if (GetCurrentState() == null)
         {
            MessageBox.Show(this, "Please select a state to move first", "Move Sprite State Up", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         
         int selIdx = lstSpriteStates.SelectedIndex;

         if (selIdx > 0)
         {
            ProjectDataset.SpriteStateRow move = GetCurrentState();
            SuspendSelection(true);
            lstSpriteStates.SelectedIndex = -1;
            ProjectData.MoveSpriteState(move, false);
            lstSpriteStates.SelectedIndex = selIdx - 1;
            SuspendSelection(false);
         }
      }

      private void OnMoveStateDown(object sender, System.EventArgs e)
      {
         if (GetCurrentState() == null)
         {
            MessageBox.Show(this, "Please select a state to move first", "Move Sprite State Down", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         int selIdx = lstSpriteStates.SelectedIndex;

         if (selIdx < lstSpriteStates.Items.Count - 1)
         {
            ProjectDataset.SpriteStateRow move = GetCurrentState();
            SuspendSelection(true);
            lstSpriteStates.SelectedIndex = -1;
            ProjectData.MoveSpriteState(move, true);
            lstSpriteStates.SelectedIndex = selIdx + 1;
            SuspendSelection(false);
         }
      }

      private void mnuAddFrame_Click(object sender, System.EventArgs e)
      {
         if (GetCurrentState() == null)
         {
            MessageBox.Show(this, "Please select a state to which the frame will be added first", "Add Frame to State", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         if (AvailableFrames.CurrentCellIndex < 0)
         {
            MessageBox.Show(this, "Please select a frame from Available Frames first", "Add Frame to State", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         foreach (ProjectDataset.FrameRow fr in AvailableFrames.GetSelectedFrames())
         {
            double dblAlpha;
            byte byAlpha = 0;
            if (Double.TryParse(txtMaskAlpha.Text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out dblAlpha))
            {
               byAlpha = (byte)dblAlpha;
            }
            StateFrames.FramesToDisplay.Add(new SpriteFrame(ProjectData.InsertFrame(GetCurrentState(), -1, fr.FrameValue, (short)updRepeatCount.Value, byAlpha)));
         }
      }

      private void mnuRemoveFrame_Click(object sender, System.EventArgs e)
      {
         ProjectDataset.SpriteStateRow sr = GetCurrentState();
         
         if (sr == null)
            return;

         if (DialogResult.Yes != MessageBox.Show(this, "Are you sure you want to remove the selected " + StateFrames.GetSelectedCellCount().ToString() + " frame(s) from state " + sr.Name + "?", "Remove Frames", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            return;

         FrameList DeleteFrames = new FrameList();

         for (int i = 0; i < StateFrames.FramesToDisplay.Count; i++)
         {
            if (StateFrames.Selected[i])
               DeleteFrames.Add(StateFrames.FramesToDisplay[i]);
         }

         foreach(SpriteFrame sf in DeleteFrames)
         {
            ProjectData.DeleteSpriteFrame(sf.Row);
            StateFrames.FramesToDisplay.Remove(sf);
         }
      }

      private void DataMonitor_FramesetRowChanged(object sender, SGDK2.ProjectDataset.FramesetRowChangeEvent e)
      {
         if ((e.Action == System.Data.DataRowAction.Add) && (!cboFrameset.Items.Contains(e.Row)))
            cboFrameset.Items.Add(e.Row);      
      }

      private void DataMonitor_FramesetRowChanging(object sender, SGDK2.ProjectDataset.FramesetRowChangeEvent e)
      {
         if (e.Action == System.Data.DataRowAction.Change)
         {
            for (int i=0; i < cboFrameset.Items.Count; i++)
            {
               if (cboFrameset.Items[i] == e.Row)
               {
                  cboFrameset.Items[i] = e.Row;
                  break;
               }
            }
         }      
      }

      private void DataMonitor_FramesetRowDeleted(object sender, SGDK2.ProjectDataset.FramesetRowChangeEvent e)
      {
         if (cboFrameset.Items.Contains(e.Row))
         {
            if (cboFrameset.SelectedItem == e.Row)
            {
               AvailableFrames.Frameset = null;
               StateFrames.Frameset = null;
            }
            cboFrameset.Items.Remove(e.Row);
         }
      }

      private void SpriteParameter_ColumnChanging(object sender, System.Data.DataColumnChangeEventArgs e)
      {
         if (!(e.ProposedValue is string))
            return;
         if (e.Column != ProjectData.SpriteParameter.NameColumn)
            return;
         string sValid = ProjectData.ValidateName(e.ProposedValue.ToString());
         if ((sValid != null) && (sValid.Length > 0))
         {
            MessageBox.Show(this, sValid, "Sprite Parameter Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            if (e.Row.IsNull(ProjectData.SpriteParameter.NameColumn))
            {
               if (e.ProposedValue is string)
                  e.ProposedValue = e.Row[ProjectData.SpriteParameter.NameColumn];
               return;
            }
            if (((ProjectDataset.SpriteParameterRow)e.Row).Name != e.ProposedValue.ToString())
               e.ProposedValue = ((ProjectDataset.SpriteParameterRow)e.Row).Name;
         }
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
         EnableFields();
         PrepareFunction(cboFunction.SelectedItem.ToString());
         if (CurrentRule != null)
            LoadParameters(CurrentRule);
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

      private void updRepeatCount_Validated(object sender, System.EventArgs e)
      {
         // Force control to pick up new value
         decimal dummy = ((NumericUpDown)sender).Value;      
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
         ProjectDataset.SpriteRuleRow drRule = ProjectData.GetSpriteRule(m_SpriteDef, node.Text);
         drRule.Suspended = suspend;
         node.ImageIndex = node.SelectedImageIndex = GetRuleImage(drRule);
         foreach(TreeNode child in node.Nodes)
            ToggleSuspend(child, suspend);
      }

      private void OnAddRule(object sender, System.EventArgs e)
      {
         if (!Validate())
            return;

         int i = 0;
         string newName;
         do
         {
            newName = "Rule " + (++i).ToString();
         } while (ProjectData.GetSpriteRule(m_SpriteDef, newName) != null);
         int newSeq = -1;
         if (CurrentRule != null)
            newSeq = CurrentRule.Sequence + 1;
         m_SelectAfterLoad = ProjectData.InsertSpriteRule(m_SpriteDef, newName, "Do", newSeq, string.Empty, null, null, null, null, false, false);
      }
      private void OnDeleteRule(object sender, System.EventArgs e)
      {
         if (CurrentRule == null)
         {
            MessageBox.Show(this, "Select a rule first", "Delete Rule", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         ProjectData.DeleteSpriteRule(CurrentRule);
      }
      private void OnMoveRuleUp(object sender, System.EventArgs e)
      {
         if (!Validate())
            return;

         if (CurrentRule == null)
         {
            MessageBox.Show(this, "Select a rule first", "Move Rule Up", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         ProjectData.MoveSpriteRule(CurrentRule, false);
      }
      private void OnMoveRuleDown(object sender, System.EventArgs e)
      {
         if (!Validate())
            return;

         if (CurrentRule == null)
         {
            MessageBox.Show(this, "Select a rule first", "Move Rule Down", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }      
         ProjectData.MoveSpriteRule(CurrentRule, true);
      }
      private void tbrStates_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
      {
         if (e.Button == tbbAddState)
            OnAddState(tbbAddState, e);
         if (e.Button == tbbDeleteState)
            OnDeleteState(tbbDeleteState, e);
         if (e.Button == tbbMoveStateUp)
            OnMoveStateUp(tbbMoveStateUp, e);
         if (e.Button == tbbMoveStateDown)
            OnMoveStateDown(tbbMoveStateDown, e);
         if (e.Button == tbbPreview)
            OnPreviewAnimation(tbbPreview, e);
      }

      private void tbrRules_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
      {
         if (e.Button == tbbNewRule)
            OnAddRule(tbbNewRule, e);
         if (e.Button == tbbDeleteRule)
            OnDeleteRule(tbbDeleteRule, e);
         if (e.Button == tbbMoveRuleUp)
            OnMoveRuleUp(tbbMoveRuleUp, e);
         if (e.Button == tbbMoveRuleDown)
            OnMoveRuleDown(tbbMoveRuleDown, e);
         if (e.Button == tbbToggleSuspend)
            OnToggleSuspend(tbbToggleSuspend, e);
         if (e.Button == tbbConvertToFunc)
            ConvertRuleAndChildren();
      }

      private void dataMonitor_SpriteRuleRowChanging(object sender, SGDK2.ProjectDataset.SpriteRuleRowChangeEvent e)
      {
         if (((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.Delete)) && (e.Row.SpriteDefinitionRow == m_SpriteDef))
         {
            m_OldRuleName = e.Row[ProjectData.SpriteRule.NameColumn,DataRowVersion.Current].ToString();
            m_OldSequence = (int)e.Row[ProjectData.SpriteRule.SequenceColumn,DataRowVersion.Current];
            m_OldType = e.Row[ProjectData.SpriteRule.TypeColumn,DataRowVersion.Current].ToString();
            m_OldEndIf = (bool)e.Row[ProjectData.SpriteRule.EndIfColumn,DataRowVersion.Current];
         }
      }

      private void dataMonitor_SpriteRuleRowChanged(object sender, SGDK2.ProjectDataset.SpriteRuleRowChangeEvent e)
      {
         switch(e.Action)
         {
            case DataRowAction.Add:
               if (e.Row.SpriteDefinitionRow == m_SpriteDef)
                  QueuePopulateRules();
               break;
            case DataRowAction.Change:
               if ((e.Row.SpriteDefinitionRow == m_SpriteDef) && (m_OldRuleName != null))
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

      private void DataMonitor_SpriteParameterRowChanged(object sender, SGDK2.ProjectDataset.SpriteParameterRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            ProjectData.ResyncParameters(m_SpriteDef);
         }
      }

      private void chkEndIf_CheckedChanged(object sender, System.EventArgs e)
      {
         if (m_Loading)
            return;
         if (CurrentRule != null)
            CurrentRule.EndIf = chkEndIf.Checked;
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
            CurrentRule.Parameter1 = fldValue;
         if (sender == cboParam2)
            CurrentRule.Parameter2 = fldValue;
         if (sender == cboParam3)
            CurrentRule.Parameter3 = fldValue;
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

      private void tabSpriteDefinition_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (tabSpriteDefinition.SelectedTab==tabRules)
         {
            QueuePopulateRules();
            SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"html/476b45aa-2b60-4be3-9887-643de3cc421c.htm");
         }
         else if (tabSpriteDefinition.SelectedTab==tabStates)
            SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"html/b70a3d81-1ff7-429f-b4f9-d65bafe1f09d.htm");
         else
            SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"html/1b29c16a-1be0-4a46-b0b2-1f023b465b15.htm");
      }

      private void txtRuleName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         ProjectDataset.SpriteRuleRow sr = ProjectData.GetSpriteRule(m_SpriteDef, txtRuleName.Text);
         if ((null != sr) && (CurrentRule != sr))
         {
            if (DialogResult.Cancel == MessageBox.Show(this, txtRuleName.Text + " already exists", "Sprite Rule Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtRuleName.Text = CurrentRule.Name;
            e.Cancel = true;
         }
      }

      private void btnMaskAlpha_Click(object sender, System.EventArgs e)
      {
         int count = StateFrames.GetSelectedCellCount();
         if (count <= 0)
         {
            MessageBox.Show(this, "Select a frame first", "Set Collision Mask Alpha Level", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         int[] SelectedFrames = new int[count];
         int selIdx = 0;
         for (int i = 0; i < StateFrames.CellCount; i++)
            if (StateFrames.Selected[i])
               SelectedFrames[selIdx++] = i;
         new frmCollisionMask(GetCurrentState(), SelectedFrames).ShowDialog(this);
         txtMaskAlpha.Text = ((SpriteFrame)(StateFrames.FramesToDisplay[StateFrames.CurrentCellIndex])).Row.MaskAlphaLevel.ToString();
      }

      private void StateFrames_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Delete)
         {
            mnuRemoveFrame_Click(sender, null);
         }
      }

      private void tmrPopulateRules_Tick(object sender, System.EventArgs e)
      {
         tmrPopulateRules.Stop();
         PopulateRules();
      }
 
      private void mnuExport_Click(object sender, System.EventArgs e)
      {
         if (!Validate())
            return;

         ProjectDataset dsExport = new ProjectDataset();
         
         try
         {
            dsExport.EnforceConstraints = false;
            dsExport.Merge(new System.Data.DataRow[] {m_SpriteDef});
            ProjectDataset.SpriteStateRow[] states = ProjectData.GetSortedSpriteStates(m_SpriteDef);
            dsExport.Merge(states);
            foreach (ProjectDataset.SpriteStateRow state in states)
            {
               dsExport.Merge(ProjectData.GetSortedSpriteFrames(state));
               ProjectDataset.FrameRow[] frames = ProjectData.GetSortedFrameRows(state.FramesetRow);
               dsExport.Merge(frames);
               dsExport.Merge(new System.Data.DataRow[] {state.FramesetRow});
               foreach (ProjectDataset.FrameRow frame in frames)
               {
                  dsExport.Merge(new System.Data.DataRow[] {ProjectData.GetGraphicSheet(frame.GraphicSheet)});
               }
            }
            dsExport.Merge(ProjectData.GetSortedSpriteParameters(m_SpriteDef));
            dsExport.Merge(ProjectData.GetSortedSpriteRules(m_SpriteDef,true));
            dsExport.EnforceConstraints = true;

            string comment = frmInputBox.GetInput(this, "Export Sprite Definition", "Enter any comments to save with the template", "Exported sprite definition \"" + m_SpriteDef.Name + "\"");
            if (comment == null)
               return;
            
            if (ProjectData.ProjectRow.IsCreditsNull())
               dsExport.Project.AddProjectRow(GameDisplayMode.m640x480x24.ToString(), true, comment, null, null, 1, 1, string.Empty);
            else
               dsExport.Project.AddProjectRow(GameDisplayMode.m640x480x24.ToString(), true, comment, null, null, 1, 1, ProjectData.ProjectRow.Credits);

            System.Windows.Forms.SaveFileDialog dlgSave = new SaveFileDialog();
            dlgSave.OverwritePrompt = true;
            dlgSave.CheckPathExists = true;
            dlgSave.DefaultExt = "sgdk2";
            dlgSave.Filter = "SGDK 2 Data File (*.sgdk2)|*.sgdk2|All Files (*.*)|*.*";
            dlgSave.FilterIndex = 1;
            dlgSave.Title = "Export Sprite Definition";
            if (DialogResult.OK == dlgSave.ShowDialog(this))
               dsExport.WriteXml(dlgSave.FileName, XmlWriteMode.WriteSchema);
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Export Sprite Definition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
      }

      private void mnuRotateWizard_Click(object sender, System.EventArgs e)
      {
         using (frmRotateWizard frm = new frmRotateWizard(m_SpriteDef))
            frm.ShowDialog(this);
      }

      private void OnPreviewAnimation(object sender, System.EventArgs e)
      {
         if (lstSpriteStates.SelectedIndex >= 0)
         {
            frmAnimPreview frm = new frmAnimPreview(GetCurrentState());
            frm.Owner = this;
            frm.Show();
         }
         else
         {
            MessageBox.Show(this, "Select a sprite state first.", "Preview State Animation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
      }
 
      private void cboParam_SelectionChangeCommitted(object sender, System.EventArgs e)
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

      private void mnuPasteRules_Click(object sender, System.EventArgs e)
      {
         if (sender == mnuPasteRuleAbove)
            PasteRules(false);
         else if (sender == mnuPasteRuleBelow)
            PasteRules(true);
      }

      private void mnuCopyRules_Click(object sender, System.EventArgs e)
      {
         if ((sender == mnuCopyAllRules) || (sender == mnuCutAllRules))
            CopyAllRules();
         else if ((sender == mnuCopyRuleOnly) || (sender == mnuCutRuleOnly))
            CopyRules(false);
         else if ((sender == mnuCopyRuleChildren) || (sender == mnuCutRuleChildren))
            CopyRules(true);

         if (sender == mnuCutAllRules)
            DeleteAllRules();
         else if (sender == mnuCutRuleOnly)
            DeleteRules(false);
         else if (sender == mnuCutRuleChildren)
            DeleteRules(true);

         EnablePasteRules();
      }

      private void mnuConvertToFunc_Click(object sender, EventArgs e)
      {
         if (sender == mnuConvertAllRules)
            ConvertAllRules();
         else
            ConvertRuleAndChildren();
      }

      private void cboBaseClass_TextUpdate(object sender, EventArgs e)
      {
         m_SpriteDef.BaseClass = cboBaseClass.Text;
         CodeGenerator.ResetTempAssembly();
         m_AvailableRules = null;
      }
      #endregion
   }
}