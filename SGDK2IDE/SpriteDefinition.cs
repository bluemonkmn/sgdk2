using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

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

      class RuleTable : System.Collections.DictionaryBase
      {
         public RemotingServices.RemoteMethodInfo this[string name]
         {
            get
            {
               return (RemotingServices.RemoteMethodInfo)InnerHashtable[name];
            }
            set
            {
               InnerHashtable[name] = value;
            }
         }

         public ICollection Rules
         {
            get
            {
               return InnerHashtable.Values;
            }
         }
      }

      class EnumTable : System.Collections.DictionaryBase
      {
         public string[] this[string name]
         {
            get
            {
               return InnerHashtable[name] as string[];
            }
            set
            {
               InnerHashtable[name] = value;
            }
         }

         public bool Contains(string name)
         {
            return InnerHashtable.Contains(name);
         }
      }
      #endregion

      private ProjectDataset.SpriteDefinitionRow m_SpriteDef;
      private RuleTable m_AvailableRules = null;
      private EnumTable m_Enums = null;

      #region Windows Forms Designer Components
      private System.Windows.Forms.TabControl tabSpriteDefinition;
      private System.Windows.Forms.TabPage tabStates;
      private System.Windows.Forms.TabPage tabParamegers;
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
      private System.Windows.Forms.Splitter splitter2;
      private System.Windows.Forms.MainMenu mnuSprites;
      private SGDK2.DataChangeNotifier DataMonitor;
      private System.Windows.Forms.Label lblSolidity;
      private System.Windows.Forms.ComboBox cboSolidity;
      private System.Windows.Forms.MenuItem mnuSpriteDefinition;
      private System.Windows.Forms.MenuItem mnuAddState;
      private System.Windows.Forms.MenuItem mnuDeleteState;
      private System.Windows.Forms.MenuItem mnuAddFrame;
      private System.Windows.Forms.MenuItem mnuRemoveFrame;
      private System.Windows.Forms.ListBox lstSpriteStates;
      private System.Windows.Forms.DataGrid grdParameters;
      private System.Windows.Forms.TreeView treeView1;
      private System.Windows.Forms.Splitter splitter1;
      private System.Windows.Forms.MenuItem mnuConditionSeparator;
      private System.Windows.Forms.MenuItem mnuAddAction;
      private System.Windows.Forms.MenuItem mnuRemoveRule;
      private System.Windows.Forms.Label lblRuleName;
      private System.Windows.Forms.TextBox txtRuleName;
      private System.Windows.Forms.GroupBox grpCondition;
      private System.Windows.Forms.ImageList imlArrows;
      private System.Windows.Forms.Button btnMoveUp;
      private System.Windows.Forms.Button btnMoveDown;
      private System.Windows.Forms.ComboBox cboRuleType;
      private System.Windows.Forms.Label lblParam2;
      private System.Windows.Forms.Label lblParam1;
      private System.Windows.Forms.ComboBox cboParam2;
      private System.Windows.Forms.ComboBox cboParam1;
      private System.Windows.Forms.CheckBox chkNot;
      private System.Windows.Forms.ComboBox cboFunction;
      private System.Windows.Forms.Label lblParam3;
      private System.Windows.Forms.ComboBox cboParam3;
      private System.Windows.Forms.Label lblOutput;
      private System.Windows.Forms.ComboBox cboOutput;
      private System.Windows.Forms.CheckBox chkEndIf;
      private System.Windows.Forms.Label lblCompileError;
      private System.ComponentModel.IContainer components;
      #endregion

      #region Initialization and Clean-up
		public frmSpriteDefinition()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         String sName;
         Int32 nIdx = 1;
         do
            sName = "New Sprite Definition " + (nIdx++).ToString();
         while (ProjectData.GetSpriteDefinition(sName) != null);
         m_SpriteDef = ProjectData.AddSpriteDefinition(sName);
         txtName.Text = sName;
         ProjectData.AcceptChanges();

         EnableState(false);
         FillSolidity();
         FillFramesets();
         FillStates();
         InitParameters();
      }

      public frmSpriteDefinition(ProjectDataset.SpriteDefinitionRow drSpriteDef)
      {
         InitializeComponent();

         m_SpriteDef = drSpriteDef;
         txtName.Text = drSpriteDef.Name;

         EnableState(false);
         FillSolidity();
         FillFramesets();
         FillStates();
         InitParameters();
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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmSpriteDefinition));
         this.tabSpriteDefinition = new System.Windows.Forms.TabControl();
         this.tabStates = new System.Windows.Forms.TabPage();
         this.AvailableFrames = new SGDK2.GraphicBrowser();
         this.lblAvailableFrames = new System.Windows.Forms.Label();
         this.splitter2 = new System.Windows.Forms.Splitter();
         this.pnlFrames = new System.Windows.Forms.Panel();
         this.StateFrames = new SGDK2.GraphicBrowser();
         this.pnlFrameDetails = new System.Windows.Forms.Panel();
         this.updRepeatCount = new System.Windows.Forms.NumericUpDown();
         this.lblRepeatCount = new System.Windows.Forms.Label();
         this.lblStateFrames = new System.Windows.Forms.Label();
         this.pnlSpriteState = new System.Windows.Forms.Panel();
         this.cboFrameset = new System.Windows.Forms.ComboBox();
         this.lblFrameset = new System.Windows.Forms.Label();
         this.txtStateName = new System.Windows.Forms.TextBox();
         this.lblStateName = new System.Windows.Forms.Label();
         this.lblSolidity = new System.Windows.Forms.Label();
         this.cboSolidity = new System.Windows.Forms.ComboBox();
         this.StateTreeSplitter = new System.Windows.Forms.Splitter();
         this.lstSpriteStates = new System.Windows.Forms.ListBox();
         this.tabParamegers = new System.Windows.Forms.TabPage();
         this.grdParameters = new System.Windows.Forms.DataGrid();
         this.tabRules = new System.Windows.Forms.TabPage();
         this.btnMoveDown = new System.Windows.Forms.Button();
         this.imlArrows = new System.Windows.Forms.ImageList(this.components);
         this.btnMoveUp = new System.Windows.Forms.Button();
         this.grpCondition = new System.Windows.Forms.GroupBox();
         this.chkEndIf = new System.Windows.Forms.CheckBox();
         this.lblOutput = new System.Windows.Forms.Label();
         this.cboOutput = new System.Windows.Forms.ComboBox();
         this.lblParam3 = new System.Windows.Forms.Label();
         this.cboParam3 = new System.Windows.Forms.ComboBox();
         this.cboRuleType = new System.Windows.Forms.ComboBox();
         this.lblParam2 = new System.Windows.Forms.Label();
         this.lblParam1 = new System.Windows.Forms.Label();
         this.cboParam2 = new System.Windows.Forms.ComboBox();
         this.cboParam1 = new System.Windows.Forms.ComboBox();
         this.chkNot = new System.Windows.Forms.CheckBox();
         this.cboFunction = new System.Windows.Forms.ComboBox();
         this.txtRuleName = new System.Windows.Forms.TextBox();
         this.lblRuleName = new System.Windows.Forms.Label();
         this.splitter1 = new System.Windows.Forms.Splitter();
         this.treeView1 = new System.Windows.Forms.TreeView();
         this.pnlSpriteHeader = new System.Windows.Forms.Panel();
         this.txtName = new System.Windows.Forms.TextBox();
         this.lblName = new System.Windows.Forms.Label();
         this.mnuSprites = new System.Windows.Forms.MainMenu();
         this.mnuSpriteDefinition = new System.Windows.Forms.MenuItem();
         this.mnuAddState = new System.Windows.Forms.MenuItem();
         this.mnuDeleteState = new System.Windows.Forms.MenuItem();
         this.mnuAddFrame = new System.Windows.Forms.MenuItem();
         this.mnuRemoveFrame = new System.Windows.Forms.MenuItem();
         this.mnuConditionSeparator = new System.Windows.Forms.MenuItem();
         this.mnuAddAction = new System.Windows.Forms.MenuItem();
         this.mnuRemoveRule = new System.Windows.Forms.MenuItem();
         this.DataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.lblCompileError = new System.Windows.Forms.Label();
         this.tabSpriteDefinition.SuspendLayout();
         this.tabStates.SuspendLayout();
         this.pnlFrames.SuspendLayout();
         this.pnlFrameDetails.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.updRepeatCount)).BeginInit();
         this.pnlSpriteState.SuspendLayout();
         this.tabParamegers.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.grdParameters)).BeginInit();
         this.tabRules.SuspendLayout();
         this.grpCondition.SuspendLayout();
         this.pnlSpriteHeader.SuspendLayout();
         this.SuspendLayout();
         // 
         // tabSpriteDefinition
         // 
         this.tabSpriteDefinition.Controls.Add(this.tabStates);
         this.tabSpriteDefinition.Controls.Add(this.tabParamegers);
         this.tabSpriteDefinition.Controls.Add(this.tabRules);
         this.tabSpriteDefinition.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tabSpriteDefinition.Location = new System.Drawing.Point(0, 32);
         this.tabSpriteDefinition.Name = "tabSpriteDefinition";
         this.tabSpriteDefinition.SelectedIndex = 0;
         this.tabSpriteDefinition.Size = new System.Drawing.Size(552, 361);
         this.tabSpriteDefinition.TabIndex = 0;
         this.tabSpriteDefinition.SelectedIndexChanged += new System.EventHandler(this.tabSpriteDefinition_SelectedIndexChanged);
         // 
         // tabStates
         // 
         this.tabStates.Controls.Add(this.AvailableFrames);
         this.tabStates.Controls.Add(this.lblAvailableFrames);
         this.tabStates.Controls.Add(this.splitter2);
         this.tabStates.Controls.Add(this.pnlFrames);
         this.tabStates.Controls.Add(this.pnlSpriteState);
         this.tabStates.Controls.Add(this.StateTreeSplitter);
         this.tabStates.Controls.Add(this.lstSpriteStates);
         this.tabStates.Location = new System.Drawing.Point(4, 22);
         this.tabStates.Name = "tabStates";
         this.tabStates.Size = new System.Drawing.Size(552, 343);
         this.tabStates.TabIndex = 0;
         this.tabStates.Text = "States";
         // 
         // AvailableFrames
         // 
         this.AvailableFrames.AllowDrop = true;
         this.AvailableFrames.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.AvailableFrames.CellPadding = new System.Drawing.Size(0, 0);
         this.AvailableFrames.CellSize = new System.Drawing.Size(0, 0);
         this.AvailableFrames.CurrentCellIndex = -1;
         this.AvailableFrames.Dock = System.Windows.Forms.DockStyle.Fill;
         this.AvailableFrames.Frameset = null;
         this.AvailableFrames.FramesToDisplay = null;
         this.AvailableFrames.GraphicSheet = null;
         this.AvailableFrames.Location = new System.Drawing.Point(125, 229);
         this.AvailableFrames.Name = "AvailableFrames";
         this.AvailableFrames.SheetImage = null;
         this.AvailableFrames.Size = new System.Drawing.Size(427, 114);
         this.AvailableFrames.TabIndex = 10;
         // 
         // lblAvailableFrames
         // 
         this.lblAvailableFrames.Dock = System.Windows.Forms.DockStyle.Top;
         this.lblAvailableFrames.Location = new System.Drawing.Point(125, 213);
         this.lblAvailableFrames.Name = "lblAvailableFrames";
         this.lblAvailableFrames.Size = new System.Drawing.Size(427, 16);
         this.lblAvailableFrames.TabIndex = 11;
         this.lblAvailableFrames.Text = "Available Frames:";
         // 
         // splitter2
         // 
         this.splitter2.BackColor = System.Drawing.SystemColors.ControlDark;
         this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
         this.splitter2.Location = new System.Drawing.Point(125, 208);
         this.splitter2.Name = "splitter2";
         this.splitter2.Size = new System.Drawing.Size(427, 5);
         this.splitter2.TabIndex = 12;
         this.splitter2.TabStop = false;
         // 
         // pnlFrames
         // 
         this.pnlFrames.Controls.Add(this.StateFrames);
         this.pnlFrames.Controls.Add(this.pnlFrameDetails);
         this.pnlFrames.Controls.Add(this.lblStateFrames);
         this.pnlFrames.Dock = System.Windows.Forms.DockStyle.Top;
         this.pnlFrames.Location = new System.Drawing.Point(125, 64);
         this.pnlFrames.Name = "pnlFrames";
         this.pnlFrames.Size = new System.Drawing.Size(427, 144);
         this.pnlFrames.TabIndex = 3;
         // 
         // StateFrames
         // 
         this.StateFrames.AllowDrop = true;
         this.StateFrames.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
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
         this.StateFrames.Size = new System.Drawing.Size(427, 104);
         this.StateFrames.TabIndex = 5;
         this.StateFrames.CurrentCellChanged += new System.EventHandler(this.StateFrames_CurrentCellChanged);
         this.StateFrames.DragDrop += new System.Windows.Forms.DragEventHandler(this.StateFrames_DragDrop);
         this.StateFrames.DragOver += new System.Windows.Forms.DragEventHandler(this.StateFrames_DragOver);
         // 
         // pnlFrameDetails
         // 
         this.pnlFrameDetails.Controls.Add(this.updRepeatCount);
         this.pnlFrameDetails.Controls.Add(this.lblRepeatCount);
         this.pnlFrameDetails.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.pnlFrameDetails.DockPadding.Bottom = 2;
         this.pnlFrameDetails.DockPadding.Left = 1;
         this.pnlFrameDetails.DockPadding.Top = 2;
         this.pnlFrameDetails.Location = new System.Drawing.Point(0, 120);
         this.pnlFrameDetails.Name = "pnlFrameDetails";
         this.pnlFrameDetails.Size = new System.Drawing.Size(427, 24);
         this.pnlFrameDetails.TabIndex = 4;
         // 
         // updRepeatCount
         // 
         this.updRepeatCount.Dock = System.Windows.Forms.DockStyle.Left;
         this.updRepeatCount.Location = new System.Drawing.Point(105, 2);
         this.updRepeatCount.Maximum = new System.Decimal(new int[] {
                                                                       255,
                                                                       0,
                                                                       0,
                                                                       0});
         this.updRepeatCount.Name = "updRepeatCount";
         this.updRepeatCount.Size = new System.Drawing.Size(64, 20);
         this.updRepeatCount.TabIndex = 4;
         this.updRepeatCount.Value = new System.Decimal(new int[] {
                                                                     1,
                                                                     0,
                                                                     0,
                                                                     0});
         this.updRepeatCount.ValueChanged += new System.EventHandler(this.updRepeatCount_ValueChanged);
         // 
         // lblRepeatCount
         // 
         this.lblRepeatCount.Dock = System.Windows.Forms.DockStyle.Left;
         this.lblRepeatCount.Location = new System.Drawing.Point(1, 2);
         this.lblRepeatCount.Name = "lblRepeatCount";
         this.lblRepeatCount.Size = new System.Drawing.Size(104, 20);
         this.lblRepeatCount.TabIndex = 3;
         this.lblRepeatCount.Text = "Repeat Count:";
         this.lblRepeatCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblStateFrames
         // 
         this.lblStateFrames.Dock = System.Windows.Forms.DockStyle.Top;
         this.lblStateFrames.Location = new System.Drawing.Point(0, 0);
         this.lblStateFrames.Name = "lblStateFrames";
         this.lblStateFrames.Size = new System.Drawing.Size(427, 16);
         this.lblStateFrames.TabIndex = 7;
         this.lblStateFrames.Text = "Frames in Current State:";
         // 
         // pnlSpriteState
         // 
         this.pnlSpriteState.Controls.Add(this.cboFrameset);
         this.pnlSpriteState.Controls.Add(this.lblFrameset);
         this.pnlSpriteState.Controls.Add(this.txtStateName);
         this.pnlSpriteState.Controls.Add(this.lblStateName);
         this.pnlSpriteState.Controls.Add(this.lblSolidity);
         this.pnlSpriteState.Controls.Add(this.cboSolidity);
         this.pnlSpriteState.Dock = System.Windows.Forms.DockStyle.Top;
         this.pnlSpriteState.Location = new System.Drawing.Point(125, 0);
         this.pnlSpriteState.Name = "pnlSpriteState";
         this.pnlSpriteState.Size = new System.Drawing.Size(427, 64);
         this.pnlSpriteState.TabIndex = 2;
         // 
         // cboFrameset
         // 
         this.cboFrameset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cboFrameset.DisplayMember = "Name";
         this.cboFrameset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboFrameset.Location = new System.Drawing.Point(296, 8);
         this.cboFrameset.Name = "cboFrameset";
         this.cboFrameset.Size = new System.Drawing.Size(120, 21);
         this.cboFrameset.TabIndex = 3;
         this.cboFrameset.SelectionChangeCommitted += new System.EventHandler(this.cboFrameset_SelectionChangeCommitted);
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
         // txtStateName
         // 
         this.txtStateName.Location = new System.Drawing.Point(88, 8);
         this.txtStateName.Name = "txtStateName";
         this.txtStateName.Size = new System.Drawing.Size(120, 20);
         this.txtStateName.TabIndex = 1;
         this.txtStateName.Text = "";
         this.txtStateName.Validating += new System.ComponentModel.CancelEventHandler(this.txtStateName_Validating);
         this.txtStateName.Validated += new System.EventHandler(this.txtStateName_Validated);
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
         // lblSolidity
         // 
         this.lblSolidity.Location = new System.Drawing.Point(8, 32);
         this.lblSolidity.Name = "lblSolidity";
         this.lblSolidity.Size = new System.Drawing.Size(80, 21);
         this.lblSolidity.TabIndex = 4;
         this.lblSolidity.Text = "Solidity:";
         this.lblSolidity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // cboSolidity
         // 
         this.cboSolidity.DisplayMember = "Name";
         this.cboSolidity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboSolidity.Location = new System.Drawing.Point(88, 32);
         this.cboSolidity.Name = "cboSolidity";
         this.cboSolidity.Size = new System.Drawing.Size(120, 21);
         this.cboSolidity.TabIndex = 5;
         // 
         // StateTreeSplitter
         // 
         this.StateTreeSplitter.Location = new System.Drawing.Point(120, 0);
         this.StateTreeSplitter.Name = "StateTreeSplitter";
         this.StateTreeSplitter.Size = new System.Drawing.Size(5, 343);
         this.StateTreeSplitter.TabIndex = 1;
         this.StateTreeSplitter.TabStop = false;
         // 
         // lstSpriteStates
         // 
         this.lstSpriteStates.DisplayMember = "Name";
         this.lstSpriteStates.Dock = System.Windows.Forms.DockStyle.Left;
         this.lstSpriteStates.Location = new System.Drawing.Point(0, 0);
         this.lstSpriteStates.Name = "lstSpriteStates";
         this.lstSpriteStates.Size = new System.Drawing.Size(120, 342);
         this.lstSpriteStates.TabIndex = 13;
         this.lstSpriteStates.SelectedIndexChanged += new System.EventHandler(this.lstSpriteStates_SelectedIndexChanged);
         // 
         // tabParamegers
         // 
         this.tabParamegers.Controls.Add(this.grdParameters);
         this.tabParamegers.Location = new System.Drawing.Point(4, 22);
         this.tabParamegers.Name = "tabParamegers";
         this.tabParamegers.Size = new System.Drawing.Size(552, 343);
         this.tabParamegers.TabIndex = 1;
         this.tabParamegers.Text = "Parameters";
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
         this.grdParameters.Size = new System.Drawing.Size(552, 343);
         this.grdParameters.TabIndex = 0;
         // 
         // tabRules
         // 
         this.tabRules.Controls.Add(this.btnMoveDown);
         this.tabRules.Controls.Add(this.btnMoveUp);
         this.tabRules.Controls.Add(this.grpCondition);
         this.tabRules.Controls.Add(this.txtRuleName);
         this.tabRules.Controls.Add(this.lblRuleName);
         this.tabRules.Controls.Add(this.splitter1);
         this.tabRules.Controls.Add(this.treeView1);
         this.tabRules.Location = new System.Drawing.Point(4, 22);
         this.tabRules.Name = "tabRules";
         this.tabRules.Size = new System.Drawing.Size(544, 335);
         this.tabRules.TabIndex = 2;
         this.tabRules.Text = "Rules";
         // 
         // btnMoveDown
         // 
         this.btnMoveDown.ImageIndex = 1;
         this.btnMoveDown.ImageList = this.imlArrows;
         this.btnMoveDown.Location = new System.Drawing.Point(152, 176);
         this.btnMoveDown.Name = "btnMoveDown";
         this.btnMoveDown.Size = new System.Drawing.Size(25, 24);
         this.btnMoveDown.TabIndex = 6;
         // 
         // imlArrows
         // 
         this.imlArrows.ImageSize = new System.Drawing.Size(11, 6);
         this.imlArrows.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlArrows.ImageStream")));
         this.imlArrows.TransparentColor = System.Drawing.Color.Magenta;
         // 
         // btnMoveUp
         // 
         this.btnMoveUp.ImageIndex = 0;
         this.btnMoveUp.ImageList = this.imlArrows;
         this.btnMoveUp.Location = new System.Drawing.Point(152, 136);
         this.btnMoveUp.Name = "btnMoveUp";
         this.btnMoveUp.Size = new System.Drawing.Size(25, 24);
         this.btnMoveUp.TabIndex = 5;
         // 
         // grpCondition
         // 
         this.grpCondition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.grpCondition.Controls.Add(this.lblCompileError);
         this.grpCondition.Controls.Add(this.chkEndIf);
         this.grpCondition.Controls.Add(this.lblOutput);
         this.grpCondition.Controls.Add(this.cboOutput);
         this.grpCondition.Controls.Add(this.lblParam3);
         this.grpCondition.Controls.Add(this.cboParam3);
         this.grpCondition.Controls.Add(this.cboRuleType);
         this.grpCondition.Controls.Add(this.lblParam2);
         this.grpCondition.Controls.Add(this.lblParam1);
         this.grpCondition.Controls.Add(this.cboParam2);
         this.grpCondition.Controls.Add(this.cboParam1);
         this.grpCondition.Controls.Add(this.chkNot);
         this.grpCondition.Controls.Add(this.cboFunction);
         this.grpCondition.Location = new System.Drawing.Point(184, 32);
         this.grpCondition.Name = "grpCondition";
         this.grpCondition.Size = new System.Drawing.Size(352, 296);
         this.grpCondition.TabIndex = 4;
         this.grpCondition.TabStop = false;
         this.grpCondition.Text = "Define Rule";
         // 
         // chkEndIf
         // 
         this.chkEndIf.Enabled = false;
         this.chkEndIf.Location = new System.Drawing.Point(16, 144);
         this.chkEndIf.Name = "chkEndIf";
         this.chkEndIf.Size = new System.Drawing.Size(120, 24);
         this.chkEndIf.TabIndex = 28;
         this.chkEndIf.Text = "End If";
         // 
         // lblOutput
         // 
         this.lblOutput.Enabled = false;
         this.lblOutput.Location = new System.Drawing.Point(16, 120);
         this.lblOutput.Name = "lblOutput";
         this.lblOutput.Size = new System.Drawing.Size(120, 21);
         this.lblOutput.TabIndex = 27;
         this.lblOutput.Text = "Output to:";
         this.lblOutput.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // cboOutput
         // 
         this.cboOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cboOutput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboOutput.Enabled = false;
         this.cboOutput.Location = new System.Drawing.Point(136, 120);
         this.cboOutput.Name = "cboOutput";
         this.cboOutput.Size = new System.Drawing.Size(208, 21);
         this.cboOutput.TabIndex = 26;
         // 
         // lblParam3
         // 
         this.lblParam3.Enabled = false;
         this.lblParam3.Location = new System.Drawing.Point(16, 96);
         this.lblParam3.Name = "lblParam3";
         this.lblParam3.Size = new System.Drawing.Size(120, 21);
         this.lblParam3.TabIndex = 25;
         this.lblParam3.Text = "Parameter 3:";
         this.lblParam3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // cboParam3
         // 
         this.cboParam3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cboParam3.Enabled = false;
         this.cboParam3.Location = new System.Drawing.Point(136, 96);
         this.cboParam3.Name = "cboParam3";
         this.cboParam3.Size = new System.Drawing.Size(208, 21);
         this.cboParam3.TabIndex = 24;
         // 
         // cboRuleType
         // 
         this.cboRuleType.Items.AddRange(new object[] {
                                                         "Do",
                                                         "If",
                                                         "And",
                                                         "Or"});
         this.cboRuleType.Location = new System.Drawing.Point(16, 24);
         this.cboRuleType.Name = "cboRuleType";
         this.cboRuleType.Size = new System.Drawing.Size(56, 21);
         this.cboRuleType.TabIndex = 23;
         this.cboRuleType.SelectedIndexChanged += new System.EventHandler(this.cboRuleType_SelectedIndexChanged);
         // 
         // lblParam2
         // 
         this.lblParam2.Enabled = false;
         this.lblParam2.Location = new System.Drawing.Point(16, 72);
         this.lblParam2.Name = "lblParam2";
         this.lblParam2.Size = new System.Drawing.Size(120, 21);
         this.lblParam2.TabIndex = 6;
         this.lblParam2.Text = "Parameter 2:";
         this.lblParam2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblParam1
         // 
         this.lblParam1.Enabled = false;
         this.lblParam1.Location = new System.Drawing.Point(16, 48);
         this.lblParam1.Name = "lblParam1";
         this.lblParam1.Size = new System.Drawing.Size(120, 21);
         this.lblParam1.TabIndex = 5;
         this.lblParam1.Text = "Parameter 1:";
         this.lblParam1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // cboParam2
         // 
         this.cboParam2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cboParam2.Enabled = false;
         this.cboParam2.Location = new System.Drawing.Point(136, 72);
         this.cboParam2.Name = "cboParam2";
         this.cboParam2.Size = new System.Drawing.Size(208, 21);
         this.cboParam2.TabIndex = 4;
         // 
         // cboParam1
         // 
         this.cboParam1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cboParam1.Enabled = false;
         this.cboParam1.Location = new System.Drawing.Point(136, 48);
         this.cboParam1.Name = "cboParam1";
         this.cboParam1.Size = new System.Drawing.Size(208, 21);
         this.cboParam1.TabIndex = 3;
         // 
         // chkNot
         // 
         this.chkNot.Enabled = false;
         this.chkNot.Location = new System.Drawing.Point(80, 24);
         this.chkNot.Name = "chkNot";
         this.chkNot.Size = new System.Drawing.Size(56, 21);
         this.chkNot.TabIndex = 1;
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
         this.cboFunction.Location = new System.Drawing.Point(136, 24);
         this.cboFunction.Name = "cboFunction";
         this.cboFunction.Size = new System.Drawing.Size(208, 21);
         this.cboFunction.TabIndex = 0;
         this.cboFunction.SelectedIndexChanged += new System.EventHandler(this.cboFunction_SelectedIndexChanged);
         // 
         // txtRuleName
         // 
         this.txtRuleName.Location = new System.Drawing.Point(272, 8);
         this.txtRuleName.Name = "txtRuleName";
         this.txtRuleName.Size = new System.Drawing.Size(160, 20);
         this.txtRuleName.TabIndex = 3;
         this.txtRuleName.Text = "";
         // 
         // lblRuleName
         // 
         this.lblRuleName.Location = new System.Drawing.Point(184, 8);
         this.lblRuleName.Name = "lblRuleName";
         this.lblRuleName.Size = new System.Drawing.Size(88, 20);
         this.lblRuleName.TabIndex = 2;
         this.lblRuleName.Text = "Rule Name:";
         this.lblRuleName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // splitter1
         // 
         this.splitter1.Location = new System.Drawing.Point(144, 0);
         this.splitter1.Name = "splitter1";
         this.splitter1.Size = new System.Drawing.Size(5, 335);
         this.splitter1.TabIndex = 1;
         this.splitter1.TabStop = false;
         // 
         // treeView1
         // 
         this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
         this.treeView1.ImageIndex = -1;
         this.treeView1.Location = new System.Drawing.Point(0, 0);
         this.treeView1.Name = "treeView1";
         this.treeView1.SelectedImageIndex = -1;
         this.treeView1.Size = new System.Drawing.Size(144, 335);
         this.treeView1.TabIndex = 0;
         // 
         // pnlSpriteHeader
         // 
         this.pnlSpriteHeader.Controls.Add(this.txtName);
         this.pnlSpriteHeader.Controls.Add(this.lblName);
         this.pnlSpriteHeader.Dock = System.Windows.Forms.DockStyle.Top;
         this.pnlSpriteHeader.Location = new System.Drawing.Point(0, 0);
         this.pnlSpriteHeader.Name = "pnlSpriteHeader";
         this.pnlSpriteHeader.Size = new System.Drawing.Size(552, 32);
         this.pnlSpriteHeader.TabIndex = 1;
         // 
         // txtName
         // 
         this.txtName.Location = new System.Drawing.Point(72, 8);
         this.txtName.Name = "txtName";
         this.txtName.Size = new System.Drawing.Size(160, 20);
         this.txtName.TabIndex = 1;
         this.txtName.Text = "";
         this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
         this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
         // 
         // lblName
         // 
         this.lblName.Location = new System.Drawing.Point(8, 8);
         this.lblName.Name = "lblName";
         this.lblName.Size = new System.Drawing.Size(64, 20);
         this.lblName.TabIndex = 0;
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
                                                                                            this.mnuAddFrame,
                                                                                            this.mnuRemoveFrame,
                                                                                            this.mnuConditionSeparator,
                                                                                            this.mnuAddAction,
                                                                                            this.mnuRemoveRule});
         this.mnuSpriteDefinition.Text = "&Sprite Definition";
         // 
         // mnuAddState
         // 
         this.mnuAddState.Index = 0;
         this.mnuAddState.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
         this.mnuAddState.Text = "&Add State";
         this.mnuAddState.Click += new System.EventHandler(this.mnuAddState_Click);
         // 
         // mnuDeleteState
         // 
         this.mnuDeleteState.Index = 1;
         this.mnuDeleteState.Shortcut = System.Windows.Forms.Shortcut.CtrlD;
         this.mnuDeleteState.Text = "&Delete State";
         this.mnuDeleteState.Click += new System.EventHandler(this.mnuDeleteState_Click);
         // 
         // mnuAddFrame
         // 
         this.mnuAddFrame.Index = 2;
         this.mnuAddFrame.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
         this.mnuAddFrame.Text = "Add &Frame to State";
         this.mnuAddFrame.Click += new System.EventHandler(this.mnuAddFrame_Click);
         // 
         // mnuRemoveFrame
         // 
         this.mnuRemoveFrame.Index = 3;
         this.mnuRemoveFrame.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
         this.mnuRemoveFrame.Text = "&Remove Frame from State";
         this.mnuRemoveFrame.Click += new System.EventHandler(this.mnuRemoveFrame_Click);
         // 
         // mnuConditionSeparator
         // 
         this.mnuConditionSeparator.Index = 4;
         this.mnuConditionSeparator.Text = "-";
         // 
         // mnuAddAction
         // 
         this.mnuAddAction.Index = 5;
         this.mnuAddAction.Text = "Add Ru&le";
         // 
         // mnuRemoveRule
         // 
         this.mnuRemoveRule.Index = 6;
         this.mnuRemoveRule.Text = "Remo&ve Rule";
         // 
         // DataMonitor
         // 
         this.DataMonitor.Clearing += new System.EventHandler(this.DataMonitor_Clearing);
         this.DataMonitor.SpriteStateRowChanging += new SGDK2.ProjectDataset.SpriteStateRowChangeEventHandler(this.DataMonitor_SpriteStateRowChanging);
         this.DataMonitor.SolidityRowChanged += new SGDK2.ProjectDataset.SolidityRowChangeEventHandler(this.DataMonitor_SolidityRowChanged);
         this.DataMonitor.SpriteStateRowChanged += new SGDK2.ProjectDataset.SpriteStateRowChangeEventHandler(this.DataMonitor_SpriteStateRowChanged);
         this.DataMonitor.SpriteDefinitionRowDeleted += new SGDK2.ProjectDataset.SpriteDefinitionRowChangeEventHandler(this.DataMonitor_SpriteDefinitionRowDeleted);
         this.DataMonitor.FramesetRowChanging += new SGDK2.ProjectDataset.FramesetRowChangeEventHandler(this.DataMonitor_FramesetRowChanging);
         this.DataMonitor.FramesetRowDeleted += new SGDK2.ProjectDataset.FramesetRowChangeEventHandler(this.DataMonitor_FramesetRowDeleted);
         this.DataMonitor.FramesetRowChanged += new SGDK2.ProjectDataset.FramesetRowChangeEventHandler(this.DataMonitor_FramesetRowChanged);
         this.DataMonitor.SpriteStateRowDeleted += new SGDK2.ProjectDataset.SpriteStateRowChangeEventHandler(this.DataMonitor_SpriteStateRowDeleted);
         this.DataMonitor.SolidityRowChanging += new SGDK2.ProjectDataset.SolidityRowChangeEventHandler(this.DataMonitor_SolidityRowChanging);
         this.DataMonitor.SolidityRowDeleted += new SGDK2.ProjectDataset.SolidityRowChangeEventHandler(this.DataMonitor_SolidityRowDeleted);
         // 
         // lblCompileError
         // 
         this.lblCompileError.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.lblCompileError.Location = new System.Drawing.Point(16, 176);
         this.lblCompileError.Name = "lblCompileError";
         this.lblCompileError.Size = new System.Drawing.Size(328, 112);
         this.lblCompileError.TabIndex = 29;
         // 
         // frmSpriteDefinition
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(552, 393);
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
         ((System.ComponentModel.ISupportInitialize)(this.updRepeatCount)).EndInit();
         this.pnlSpriteState.ResumeLayout(false);
         this.tabParamegers.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.grdParameters)).EndInit();
         this.tabRules.ResumeLayout(false);
         this.grpCondition.ResumeLayout(false);
         this.pnlSpriteHeader.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion

      #region Private Methods
      /// <summary>
      /// Debug Test
      /// </summary>
      /// <returns></returns>
      private string GetDefaultSpriteCode()
      {
         System.IO.Stream remoteReflectorStream  = System.Reflection.Assembly.GetAssembly(typeof(SGDK2IDE)).GetManifestResourceStream("SGDK2.SpriteBase.cs");
         System.IO.StreamReader readReflector = new System.IO.StreamReader(remoteReflectorStream);
         string remoteReflectorCode;
         remoteReflectorCode = readReflector.ReadToEnd();
         readReflector.Close();
         return remoteReflectorCode;
      }

      private string CompileTempAssembly(string code)
      {
         lblCompileError.Text = string.Empty;
         System.IO.Stream remoteReflectorStream  = System.Reflection.Assembly.GetAssembly(typeof(SGDK2IDE)).GetManifestResourceStream("SGDK2.RemoteReflector.cs");
         System.IO.StreamReader readReflector = new System.IO.StreamReader(remoteReflectorStream);
         string remoteReflectorCode;
         remoteReflectorCode = readReflector.ReadToEnd();
         readReflector.Close();
         Microsoft.CSharp.CSharpCodeProvider codeProvider = new Microsoft.CSharp.CSharpCodeProvider();
         System.CodeDom.Compiler.ICodeCompiler compiler = codeProvider.CreateCompiler();
         System.CodeDom.Compiler.CompilerParameters compilerParams = new System.CodeDom.Compiler.CompilerParameters(new string[] {}, System.IO.Path.Combine(Application.StartupPath, "TempSprite.dll"));
         compilerParams.ReferencedAssemblies.Add(System.IO.Path.Combine(Application.StartupPath, "SGDK2IDE.exe"));
         compilerParams.GenerateExecutable = false;
         System.CodeDom.Compiler.CompilerResults results = compiler.CompileAssemblyFromSourceBatch(compilerParams, new string[] {code, remoteReflectorCode});
         if (results.Errors.Count > 0)
         {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < results.Errors.Count; i++)
            {
               sb.Append(results.Errors[i].ToString() + Environment.NewLine);
            }
            lblCompileError.Text = sb.ToString();
            return null;
         }
         return compilerParams.OutputAssembly;
      }

      private RemotingServices.RemoteMethodInfo[] GetAvailableRules(string spriteCode)
      {
         try
         {
            string assemblyFilename = CompileTempAssembly(spriteCode);
            if (assemblyFilename == null)
               return new RemotingServices.RemoteMethodInfo[] {};

            AppDomain tempDomain = AppDomain.CreateDomain("TempDomain");
            RemotingServices.IRemoteTypeInfo reflector = tempDomain.CreateInstanceAndUnwrap(
               "TempSprite", "RemoteReflector", false,
               System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
               null, new object[] {"SpriteBase"}, null, null, null) as RemotingServices.IRemoteTypeInfo;

            RemotingServices.RemoteMethodInfo[] remoteResults = reflector.GetMethods();
            AppDomain.Unload(tempDomain);
            System.IO.File.Delete(assemblyFilename);
            return remoteResults;
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, "GetAvailableRules", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return new RemotingServices.RemoteMethodInfo[] {};
         }
      }

      private string[] GetEnumInfo(string spriteCode, string enumName)
      {
         try
         {
            string assemblyFilename = CompileTempAssembly(spriteCode);
            if (assemblyFilename == null)
               return new string[] {};

            AppDomain tempDomain = AppDomain.CreateDomain("TempDomain");
            RemotingServices.IRemoteTypeInfo reflector = tempDomain.CreateInstanceAndUnwrap(
               "TempSprite", "RemoteReflector", false,
               System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
               null, new object[] {enumName}, null, null, null) as RemotingServices.IRemoteTypeInfo;

            string[] remoteResults = reflector.GetEnumVals();

            AppDomain.Unload(tempDomain);
            System.IO.File.Delete(assemblyFilename);
            return remoteResults;
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(this, ex.ToString(), "GetEnumInfo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return new string[] {};
         }
      }

      private void FillSolidity()
      {
         cboSolidity.Items.Clear();
         foreach(ProjectDataset.SolidityRow dr in ProjectData.Solidity)
         {
            cboSolidity.Items.Add(dr);
         }
      }

      private void FillFramesets()
      {
         cboFrameset.Items.Clear();
         foreach(ProjectDataset.FramesetRow fr in ProjectData.Frameset)
         {
            cboFrameset.Items.Add(fr);
         }
      }

      private void FillStates()
      {
         lstSpriteStates.Items.Clear();
         foreach(ProjectDataset.SpriteStateRow sr in m_SpriteDef.GetSpriteStateRows())
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

         for (int i=0; i<cboSolidity.Items.Count; i++)
         {
            if (cboSolidity.Items[i] == state.SolidityRow)
            {
               cboSolidity.SelectedIndex = i;
               break;
            }
         }

         AvailableFrames.Frameset = StateFrames.Frameset = state.FramesetRow;
         LoadSpriteFrames(state);
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
            lblFrameset.Enabled = lblSolidity.Enabled = cboSolidity.Enabled =
            lblStateFrames.Enabled = StateFrames.Enabled = lblAvailableFrames.Enabled =
            AvailableFrames.Enabled = lblRepeatCount.Enabled = updRepeatCount.Enabled =
            enable;
      }

      private void InitParameters()
      {
         grdParameters.SetDataBinding(ProjectData.SpriteDefinition, "SpriteDefinitionSpriteParameter");
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
         ProjectData.SpriteParameter.ColumnChanging += new System.Data.DataColumnChangeEventHandler(SpriteParameter_ColumnChanging);
      }

      private void InitRules(bool onlyBools, bool bForceRefresh)
      {
         if (bForceRefresh || (m_AvailableRules == null))
         {
            RemotingServices.RemoteMethodInfo[] ruleList = GetAvailableRules(GetDefaultSpriteCode());
            m_AvailableRules = new RuleTable();
            m_Enums = new EnumTable();
            foreach(RemotingServices.RemoteMethodInfo mi in ruleList)
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
         cboFunction.Items.Clear();
         foreach(RemotingServices.RemoteMethodInfo mi in m_AvailableRules.Rules)
         {
            if ((string.Compare(mi.ReturnType,typeof(Boolean).Name)==0) || !onlyBools)
               cboFunction.Items.Add(mi.MethodName);
         }
         chkNot.Enabled = onlyBools;

         PopulateParameter(lblParam1, cboParam1, RemotingServices.RemoteParameterInfo.Empty);
         PopulateParameter(lblParam2, cboParam2, RemotingServices.RemoteParameterInfo.Empty);
         PopulateParameter(lblParam3, cboParam3, RemotingServices.RemoteParameterInfo.Empty);
         lblOutput.Enabled = false;
         cboOutput.Enabled = false;
         cboOutput.SelectedIndex = -1;
      }

      private void PrepareFunction(string funcName)
      {
         RemotingServices.RemoteMethodInfo mi = m_AvailableRules[funcName];
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

         if ((string.Compare(mi.ReturnType, typeof(Int32).Name) == 0) ||
            (string.Compare(mi.ReturnType, typeof(Int16).Name) == 0))
         {
            lblOutput.Enabled = true;
            cboOutput.Enabled = true;
            cboOutput.Items.Clear();
            FillComboWithParams(cboOutput);
         }
         else
         {
            lblOutput.Enabled = false;
            cboOutput.Enabled = false;
            cboOutput.SelectedIndex = -1;
         }
      }

      private void FillComboWithParams(ComboBox cboParams)
      {
         foreach(ProjectDataset.SpriteParameterRow prow in m_SpriteDef.GetSpriteParameterRows())
         {
            cboParams.Items.Add(prow.Name);
         }
      }

      private void PopulateParameter(Label lblParameter, ComboBox cboParameter, RemotingServices.RemoteParameterInfo param)
      {
         cboParameter.Items.Clear();
         cboParameter.Text = string.Empty;
         cboParameter.SelectedIndex = -1;

         if (param.IsEmpty())
         {
            lblParameter.Text = "N/A";
            lblParameter.Enabled = false;
            cboParameter.Enabled = false;
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
               enumVals = m_Enums[param.TypeName] = GetEnumInfo(GetDefaultSpriteCode(), param.TypeName);

            foreach (string enumVal in enumVals)
               cboParameter.Items.Add(enumVal);
            return;
         }

         if (string.Compare(param.TypeName, typeof(bool).Name)==0)
         {
            cboParameter.Items.Add(bool.FalseString);
            cboParameter.Items.Add(bool.TrueString);
         }

         foreach(string typeName in new string[]
            {
               typeof(Int32).Name, typeof(Int16).Name,
               typeof(Double).Name, typeof(Single).Name
            })
         {
            if (string.Compare(typeName, param.TypeName) == 0)
            {
               FillComboWithParams(cboParameter);
               break;
            }
         }
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
         m_SpriteDef.Name = txtName.Text;
         ProjectData.AcceptChanges();
      }

      private void mnuAddState_Click(object sender, System.EventArgs e)
      {
         if (ProjectData.Frameset.Rows.Count <= 0)
         {
            MessageBox.Show(this, "Please create a frameset before creating sprite states", "Add Sprite State", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); 
            return;
         }
         if (ProjectData.Solidity.Rows.Count <= 0)
         {
            MessageBox.Show(this, "Please define solidity before creating sprite states", "Add Sprite State", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); 
            return;
         }
         String sName;
         Int32 nIdx = 1;
         do
            sName = "New Sprite State " + (nIdx++).ToString();
         while (ProjectData.GetSpriteState(m_SpriteDef.Name, sName) != null);

         lstSpriteStates.SelectedItem = ProjectData.AddSpriteState(m_SpriteDef, sName,
            (ProjectDataset.FramesetRow)ProjectData.Frameset.Rows[0],
            (ProjectDataset.SolidityRow)ProjectData.Solidity.Rows[0]);
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
               ProjectData.AcceptChanges();
            }
            else
            {
               foreach (SGDK2.ProjectDataset.FrameRow fr in gb.GetSelectedFrames())
               {
                  ProjectDataset.SpriteFrameRow sfr = SGDK2.ProjectData.InsertFrame(
                     row, nCellIndex, fr.FrameValue, (short)updRepeatCount.Value);
                  StateFrames.FramesToDisplay.Insert(nCellIndex++, new SpriteFrame(sfr));
               }
               ProjectData.AcceptChanges();
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
            if (StateFrames.CurrentCellIndex < 0)
               return;
            for(int i=0; i < StateFrames.CellCount; i++)
               if ((StateFrames.FramesToDisplay[i].IsSelected) && (((SpriteFrame)StateFrames.FramesToDisplay[i]).Row.Duration != (short)updRepeatCount.Value))
                  ((SpriteFrame)StateFrames.FramesToDisplay[i]).Row.Duration = (short)updRepeatCount.Value;
            ProjectData.AcceptChanges();
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
               updRepeatCount.Value = ((SpriteFrame)(StateFrames.FramesToDisplay[StateFrames.CurrentCellIndex])).Row.Duration;
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
            lstSpriteStates.Items.Add(e.Row);
         }
      }

      private void DataMonitor_SpriteStateRowChanging(object sender, SGDK2.ProjectDataset.SpriteStateRowChangeEvent e)
      {
         if ((e.Action == System.Data.DataRowAction.Change) && (e.Row.SpriteDefinitionRow == m_SpriteDef))
         {
            for (int i=0; i<lstSpriteStates.Items.Count; i++)
            {
               if (lstSpriteStates.Items[i] == e.Row)
                  lstSpriteStates.Items[i] = e.Row;
            }
         }
      }

      private void DataMonitor_SpriteStateRowDeleted(object sender, SGDK2.ProjectDataset.SpriteStateRowChangeEvent e)
      {
         if (lstSpriteStates.Items.Contains(e.Row))
            lstSpriteStates.Items.Remove(e.Row);
      }

      private void txtStateName_Validated(object sender, System.EventArgs e)
      {
         GetCurrentState().Name = txtStateName.Text;
         ProjectData.AcceptChanges();
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

      private void mnuDeleteState_Click(object sender, System.EventArgs e)
      {
         if (GetCurrentState() == null)
         {
            MessageBox.Show(this, "Please select a state to delete first", "Delete Sprite State", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }

         GetCurrentState().Delete();
         ProjectData.AcceptChanges();
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
            StateFrames.FramesToDisplay.Add(new SpriteFrame(ProjectData.InsertFrame(GetCurrentState(), -1, fr.FrameValue, (short)updRepeatCount.Value)));
         }
         ProjectData.AcceptChanges();
      }

      private void mnuRemoveFrame_Click(object sender, System.EventArgs e)
      {
         ProjectDataset.SpriteStateRow sr = GetCurrentState();
         
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
         ProjectData.AcceptChanges();
      }

      private void DataMonitor_SolidityRowChanged(object sender, SGDK2.ProjectDataset.SolidityRowChangeEvent e)
      {
         if ((e.Action == System.Data.DataRowAction.Add) && (!cboSolidity.Items.Contains(e.Row)))
         {
            cboSolidity.Items.Add(e.Row);
         }
      }

      private void DataMonitor_SolidityRowChanging(object sender, SGDK2.ProjectDataset.SolidityRowChangeEvent e)
      {
         if (e.Action == System.Data.DataRowAction.Change)
         {
            for (int i=0; i<cboSolidity.Items.Count; i++)
            {
               if (cboSolidity.Items[i] == e.Row)
               {
                  cboSolidity.Items[i] = e.Row;
                  break;
               }
            }
         }
      }

      private void DataMonitor_SolidityRowDeleted(object sender, SGDK2.ProjectDataset.SolidityRowChangeEvent e)
      {
         if (cboSolidity.Items.Contains(e.Row))
            cboSolidity.Items.Remove(e.Row);
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
            cboFrameset.Items.Remove(e.Row);      
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
 
      private void tabSpriteDefinition_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (tabSpriteDefinition.SelectedIndex == 2)
            InitRules(cboRuleType.SelectedIndex != 0, true);
      }

      private void cboRuleType_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         InitRules(cboRuleType.SelectedIndex != 0, false);
      }

      private void cboFunction_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         PrepareFunction(cboFunction.SelectedItem.ToString());
      }
      #endregion
   }
}