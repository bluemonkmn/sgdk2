using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SGDK2
{
   public class frmRotateWizard : SGDK2.frmWizardBase
   {
      #region Non-control members
      private ProjectDataset.SpriteDefinitionRow m_SpriteDefinition;
      #endregion

      #region Form Designer Members
      private System.Windows.Forms.Panel pnlSourceGraphic;
      private SGDK2.frmWizardBase.StepInfo SourceGraphic;
      private System.Windows.Forms.Label lblIntro;
      private System.Windows.Forms.Label lblGraphicSheet;
      private System.Windows.Forms.ComboBox cboGraphicSheet;
      private SGDK2.GraphicBrowser SourceCell;
      private System.Windows.Forms.Panel pnlFrameset;
      private System.Windows.Forms.Label lblFramesetHeader;
      private System.Windows.Forms.Label lblFrameset;
      private System.Windows.Forms.ComboBox cboFrameset;
      private System.Windows.Forms.Panel pnlSizingStyle;
      private SGDK2.frmWizardBase.StepInfo SizingStyle;
      private System.Windows.Forms.Label lblSizingStyleHeader;
      private System.Windows.Forms.RadioButton rdoFixedSize;
      private System.Windows.Forms.Label lblSizingStyleNote;
      private System.Windows.Forms.RadioButton rdoVariableSize;
      private System.Windows.Forms.Panel pnlPrefix;
      private SGDK2.frmWizardBase.StepInfo Prefix;
      private System.Windows.Forms.Label lblPrefixHeader;
      private System.Windows.Forms.Label lblPrefix;
      private System.Windows.Forms.TextBox txtPrefix;
      private System.Windows.Forms.Panel pnlStateCount;
      private SGDK2.frmWizardBase.StepInfo StateCount;
      private System.Windows.Forms.Label lblStateCountHeader;
      private System.Windows.Forms.Label lblStateCount;
      private System.Windows.Forms.NumericUpDown nudStateCount;
      private SGDK2.frmWizardBase.StepInfo TargetFrameset;
      private System.Windows.Forms.Panel pnlReview;
      private SGDK2.frmWizardBase.StepInfo Review;
      private System.Windows.Forms.Label lblReview;
      private System.Windows.Forms.TextBox txtReview;
      private System.Windows.Forms.Label lblFrameRepeatHeader;
      private System.Windows.Forms.Label lblRepeatCount;
      private System.Windows.Forms.NumericUpDown nudRepeatCount;
      private System.Windows.Forms.Panel pnlFrameSettings;
      private SGDK2.frmWizardBase.StepInfo FrameSettings;
      private System.Windows.Forms.Label lblAlpha;
      private System.Windows.Forms.NumericUpDown nudAlpha;
      private System.Windows.Forms.Panel pnlBaseSize;
      private SGDK2.frmWizardBase.StepInfo BaseSize;
      private System.Windows.Forms.NumericUpDown nudSizingHeight;
      private System.Windows.Forms.Label lblSizingHeight;
      private System.Windows.Forms.NumericUpDown nudSizingWidth;
      private System.Windows.Forms.Label lblSizingWidth;
      private System.Windows.Forms.Label lblBaseSizeHeader;
      private System.Windows.Forms.Panel pnlPreviewSolid;
      private System.ComponentModel.IContainer components = null;
      #endregion

      public frmRotateWizard(ProjectDataset.SpriteDefinitionRow spriteDefinition)
      {
         // This call is required by the Windows Form Designer.
         InitializeComponent();

         m_SpriteDefinition = spriteDefinition;
      }

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose( bool disposing )
      {
         if( disposing )
         {
            if (components != null) 
            {
               components.Dispose();
            }
         }
         base.Dispose( disposing );
      }

      #region Designer generated code
      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.pnlSourceGraphic = new System.Windows.Forms.Panel();
         this.SourceCell = new SGDK2.GraphicBrowser();
         this.cboGraphicSheet = new System.Windows.Forms.ComboBox();
         this.lblGraphicSheet = new System.Windows.Forms.Label();
         this.lblIntro = new System.Windows.Forms.Label();
         this.SourceGraphic = new SGDK2.frmWizardBase.StepInfo();
         this.pnlFrameset = new System.Windows.Forms.Panel();
         this.cboFrameset = new System.Windows.Forms.ComboBox();
         this.lblFrameset = new System.Windows.Forms.Label();
         this.lblFramesetHeader = new System.Windows.Forms.Label();
         this.TargetFrameset = new SGDK2.frmWizardBase.StepInfo();
         this.pnlSizingStyle = new System.Windows.Forms.Panel();
         this.rdoVariableSize = new System.Windows.Forms.RadioButton();
         this.lblSizingStyleNote = new System.Windows.Forms.Label();
         this.rdoFixedSize = new System.Windows.Forms.RadioButton();
         this.lblSizingStyleHeader = new System.Windows.Forms.Label();
         this.SizingStyle = new SGDK2.frmWizardBase.StepInfo();
         this.pnlPrefix = new System.Windows.Forms.Panel();
         this.txtPrefix = new System.Windows.Forms.TextBox();
         this.lblPrefix = new System.Windows.Forms.Label();
         this.lblPrefixHeader = new System.Windows.Forms.Label();
         this.Prefix = new SGDK2.frmWizardBase.StepInfo();
         this.pnlStateCount = new System.Windows.Forms.Panel();
         this.nudStateCount = new System.Windows.Forms.NumericUpDown();
         this.lblStateCount = new System.Windows.Forms.Label();
         this.lblStateCountHeader = new System.Windows.Forms.Label();
         this.StateCount = new SGDK2.frmWizardBase.StepInfo();
         this.pnlReview = new System.Windows.Forms.Panel();
         this.txtReview = new System.Windows.Forms.TextBox();
         this.lblReview = new System.Windows.Forms.Label();
         this.Review = new SGDK2.frmWizardBase.StepInfo();
         this.pnlFrameSettings = new System.Windows.Forms.Panel();
         this.nudAlpha = new System.Windows.Forms.NumericUpDown();
         this.lblAlpha = new System.Windows.Forms.Label();
         this.nudRepeatCount = new System.Windows.Forms.NumericUpDown();
         this.lblRepeatCount = new System.Windows.Forms.Label();
         this.lblFrameRepeatHeader = new System.Windows.Forms.Label();
         this.FrameSettings = new SGDK2.frmWizardBase.StepInfo();
         this.pnlBaseSize = new System.Windows.Forms.Panel();
         this.pnlPreviewSolid = new System.Windows.Forms.Panel();
         this.lblBaseSizeHeader = new System.Windows.Forms.Label();
         this.nudSizingHeight = new System.Windows.Forms.NumericUpDown();
         this.lblSizingHeight = new System.Windows.Forms.Label();
         this.nudSizingWidth = new System.Windows.Forms.NumericUpDown();
         this.lblSizingWidth = new System.Windows.Forms.Label();
         this.BaseSize = new SGDK2.frmWizardBase.StepInfo();
         this.pnlSourceGraphic.SuspendLayout();
         this.pnlFrameset.SuspendLayout();
         this.pnlSizingStyle.SuspendLayout();
         this.pnlPrefix.SuspendLayout();
         this.pnlStateCount.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.nudStateCount)).BeginInit();
         this.pnlReview.SuspendLayout();
         this.pnlFrameSettings.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudRepeatCount)).BeginInit();
         this.pnlBaseSize.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.nudSizingHeight)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudSizingWidth)).BeginInit();
         this.SuspendLayout();
         // 
         // pnlSourceGraphic
         // 
         this.pnlSourceGraphic.Controls.Add(this.SourceCell);
         this.pnlSourceGraphic.Controls.Add(this.cboGraphicSheet);
         this.pnlSourceGraphic.Controls.Add(this.lblGraphicSheet);
         this.pnlSourceGraphic.Controls.Add(this.lblIntro);
         this.pnlSourceGraphic.Location = new System.Drawing.Point(168, 42);
         this.pnlSourceGraphic.Name = "pnlSourceGraphic";
         this.pnlSourceGraphic.Size = new System.Drawing.Size(281, 231);
         this.pnlSourceGraphic.TabIndex = 6;
         // 
         // SourceCell
         // 
         this.SourceCell.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.SourceCell.CellPadding = new System.Drawing.Size(0, 0);
         this.SourceCell.CellSize = new System.Drawing.Size(0, 0);
         this.SourceCell.CurrentCellIndex = -1;
         this.SourceCell.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.SourceCell.Frameset = null;
         this.SourceCell.FramesToDisplay = null;
         this.SourceCell.GraphicSheet = null;
         this.SourceCell.Location = new System.Drawing.Point(0, 127);
         this.SourceCell.Name = "SourceCell";
         this.SourceCell.SheetImage = null;
         this.SourceCell.Size = new System.Drawing.Size(281, 104);
         this.SourceCell.TabIndex = 3;
         // 
         // cboGraphicSheet
         // 
         this.cboGraphicSheet.DisplayMember = "Name";
         this.cboGraphicSheet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboGraphicSheet.Location = new System.Drawing.Point(96, 96);
         this.cboGraphicSheet.Name = "cboGraphicSheet";
         this.cboGraphicSheet.Size = new System.Drawing.Size(176, 21);
         this.cboGraphicSheet.TabIndex = 2;
         this.cboGraphicSheet.SelectedIndexChanged += new System.EventHandler(this.cboGraphicSheet_SelectedIndexChanged);
         // 
         // lblGraphicSheet
         // 
         this.lblGraphicSheet.Location = new System.Drawing.Point(8, 96);
         this.lblGraphicSheet.Name = "lblGraphicSheet";
         this.lblGraphicSheet.Size = new System.Drawing.Size(88, 21);
         this.lblGraphicSheet.TabIndex = 1;
         this.lblGraphicSheet.Text = "Graphic Sheet:";
         this.lblGraphicSheet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblIntro
         // 
         this.lblIntro.Dock = System.Windows.Forms.DockStyle.Top;
         this.lblIntro.Location = new System.Drawing.Point(0, 0);
         this.lblIntro.Name = "lblIntro";
         this.lblIntro.Size = new System.Drawing.Size(281, 88);
         this.lblIntro.TabIndex = 0;
         this.lblIntro.Text = @"The rotating sprite state wizard will help you create frameset frames and sprite states to represent elements of a rotating sprite.  Each pass through the wizard can deal with one graphic cell, but multiple passes can add more graphics to existing rotating sprites.  Specify the cell to use below.";
         // 
         // SourceGraphic
         // 
         this.SourceGraphic.StepControl = this.pnlSourceGraphic;
         this.SourceGraphic.TitleText = "Source Graphic";
         this.SourceGraphic.InitFunction += new System.EventHandler(this.SourceGraphic_InitFunction);
         this.SourceGraphic.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.SourceGraphic_ValidateFunction);
         // 
         // pnlFrameset
         // 
         this.pnlFrameset.Controls.Add(this.cboFrameset);
         this.pnlFrameset.Controls.Add(this.lblFrameset);
         this.pnlFrameset.Controls.Add(this.lblFramesetHeader);
         this.pnlFrameset.Location = new System.Drawing.Point(-10168, 42);
         this.pnlFrameset.Name = "pnlFrameset";
         this.pnlFrameset.Size = new System.Drawing.Size(286, 231);
         this.pnlFrameset.TabIndex = 7;
         // 
         // cboFrameset
         // 
         this.cboFrameset.DisplayMember = "Name";
         this.cboFrameset.Location = new System.Drawing.Point(88, 112);
         this.cboFrameset.Name = "cboFrameset";
         this.cboFrameset.Size = new System.Drawing.Size(184, 21);
         this.cboFrameset.TabIndex = 2;
         // 
         // lblFrameset
         // 
         this.lblFrameset.Location = new System.Drawing.Point(8, 112);
         this.lblFrameset.Name = "lblFrameset";
         this.lblFrameset.Size = new System.Drawing.Size(80, 21);
         this.lblFrameset.TabIndex = 1;
         this.lblFrameset.Text = "Frameset:";
         this.lblFrameset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblFramesetHeader
         // 
         this.lblFramesetHeader.Location = new System.Drawing.Point(8, 8);
         this.lblFramesetHeader.Name = "lblFramesetHeader";
         this.lblFramesetHeader.Size = new System.Drawing.Size(264, 104);
         this.lblFramesetHeader.TabIndex = 0;
         this.lblFramesetHeader.Text = @"The wizard has determined that new states will be created.  You may specify the frameset to be associated with these new states.  Targeting an existing frameset will append the frames for the generated rotations to an existing frameset. Entering a new name will create a new frameset for the new states.";
         // 
         // TargetFrameset
         // 
         this.TargetFrameset.StepControl = this.pnlFrameset;
         this.TargetFrameset.TitleText = "Target Frameset";
         this.TargetFrameset.InitFunction += new System.EventHandler(this.frmFrameset_InitFunction);
         this.TargetFrameset.IsApplicableFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.TargetFrameset_IsApplicableFunction);
         this.TargetFrameset.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.frmFrameset_ValidateFunction);
         // 
         // pnlSizingStyle
         // 
         this.pnlSizingStyle.Controls.Add(this.rdoVariableSize);
         this.pnlSizingStyle.Controls.Add(this.lblSizingStyleNote);
         this.pnlSizingStyle.Controls.Add(this.rdoFixedSize);
         this.pnlSizingStyle.Controls.Add(this.lblSizingStyleHeader);
         this.pnlSizingStyle.Location = new System.Drawing.Point(-10168, 42);
         this.pnlSizingStyle.Name = "pnlSizingStyle";
         this.pnlSizingStyle.Size = new System.Drawing.Size(286, 231);
         this.pnlSizingStyle.TabIndex = 8;
         // 
         // rdoVariableSize
         // 
         this.rdoVariableSize.Checked = true;
         this.rdoVariableSize.Location = new System.Drawing.Point(8, 112);
         this.rdoVariableSize.Name = "rdoVariableSize";
         this.rdoVariableSize.Size = new System.Drawing.Size(264, 16);
         this.rdoVariableSize.TabIndex = 7;
         this.rdoVariableSize.TabStop = true;
         this.rdoVariableSize.Text = "Variable size based on cell size and rotation";
         // 
         // lblSizingStyleNote
         // 
         this.lblSizingStyleNote.Location = new System.Drawing.Point(8, 152);
         this.lblSizingStyleNote.Name = "lblSizingStyleNote";
         this.lblSizingStyleNote.Size = new System.Drawing.Size(264, 64);
         this.lblSizingStyleNote.TabIndex = 2;
         this.lblSizingStyleNote.Text = "Note: Sizing style only affects solidity size of states created by the wizard, an" +
            "d will not affect existing states to which the wizard appends frames.  It will, " +
            "however, affect the positioning of all frames created by the wizard.";
         // 
         // rdoFixedSize
         // 
         this.rdoFixedSize.Location = new System.Drawing.Point(8, 88);
         this.rdoFixedSize.Name = "rdoFixedSize";
         this.rdoFixedSize.Size = new System.Drawing.Size(264, 16);
         this.rdoFixedSize.TabIndex = 1;
         this.rdoFixedSize.Text = "Use a fixed size for all states";
         // 
         // lblSizingStyleHeader
         // 
         this.lblSizingStyleHeader.Location = new System.Drawing.Point(8, 8);
         this.lblSizingStyleHeader.Name = "lblSizingStyleHeader";
         this.lblSizingStyleHeader.Size = new System.Drawing.Size(264, 80);
         this.lblSizingStyleHeader.TabIndex = 0;
         this.lblSizingStyleHeader.Text = "A rotating sprite can have a fixed size, which means the perimiter is the same no" +
            " matter what rotation the sprite is at, or it may allow the size to change as it" +
            "s rotation changes, which may prevent the sprite from rotating when stuck in tig" +
            "ht spots.";
         // 
         // SizingStyle
         // 
         this.SizingStyle.StepControl = this.pnlSizingStyle;
         this.SizingStyle.TitleText = "Sizing Style";
         // 
         // pnlPrefix
         // 
         this.pnlPrefix.Controls.Add(this.txtPrefix);
         this.pnlPrefix.Controls.Add(this.lblPrefix);
         this.pnlPrefix.Controls.Add(this.lblPrefixHeader);
         this.pnlPrefix.Location = new System.Drawing.Point(-10168, 42);
         this.pnlPrefix.Name = "pnlPrefix";
         this.pnlPrefix.Size = new System.Drawing.Size(286, 231);
         this.pnlPrefix.TabIndex = 9;
         // 
         // txtPrefix
         // 
         this.txtPrefix.Location = new System.Drawing.Point(72, 128);
         this.txtPrefix.MaxLength = 40;
         this.txtPrefix.Name = "txtPrefix";
         this.txtPrefix.Size = new System.Drawing.Size(152, 20);
         this.txtPrefix.TabIndex = 2;
         this.txtPrefix.Text = "";
         // 
         // lblPrefix
         // 
         this.lblPrefix.Location = new System.Drawing.Point(8, 128);
         this.lblPrefix.Name = "lblPrefix";
         this.lblPrefix.Size = new System.Drawing.Size(64, 20);
         this.lblPrefix.TabIndex = 1;
         this.lblPrefix.Text = "Prefix:";
         this.lblPrefix.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblPrefixHeader
         // 
         this.lblPrefixHeader.Location = new System.Drawing.Point(8, 8);
         this.lblPrefixHeader.Name = "lblPrefixHeader";
         this.lblPrefixHeader.Size = new System.Drawing.Size(264, 112);
         this.lblPrefixHeader.TabIndex = 0;
         this.lblPrefixHeader.Text = @"The generated sprite states will be named according to how many degrees of rotation they represent.  But each state must also be prefixed with a string.  This allows you to have multiple sets of rotating states to represent, for example, accelerating and drifting states for each rotation.  Specifying an existing prefix will cause the wizard to append frames to existing states.";
         // 
         // Prefix
         // 
         this.Prefix.StepControl = this.pnlPrefix;
         this.Prefix.TitleText = "Specify Prefix";
         this.Prefix.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.Prefix_ValidateFunction);
         // 
         // pnlStateCount
         // 
         this.pnlStateCount.Controls.Add(this.nudStateCount);
         this.pnlStateCount.Controls.Add(this.lblStateCount);
         this.pnlStateCount.Controls.Add(this.lblStateCountHeader);
         this.pnlStateCount.Location = new System.Drawing.Point(-10168, 42);
         this.pnlStateCount.Name = "pnlStateCount";
         this.pnlStateCount.Size = new System.Drawing.Size(285, 231);
         this.pnlStateCount.TabIndex = 10;
         // 
         // nudStateCount
         // 
         this.nudStateCount.Location = new System.Drawing.Point(104, 88);
         this.nudStateCount.Maximum = new System.Decimal(new int[] {
                                                                      1080,
                                                                      0,
                                                                      0,
                                                                      0});
         this.nudStateCount.Minimum = new System.Decimal(new int[] {
                                                                      4,
                                                                      0,
                                                                      0,
                                                                      0});
         this.nudStateCount.Name = "nudStateCount";
         this.nudStateCount.Size = new System.Drawing.Size(64, 20);
         this.nudStateCount.TabIndex = 2;
         this.nudStateCount.Value = new System.Decimal(new int[] {
                                                                    72,
                                                                    0,
                                                                    0,
                                                                    0});
         // 
         // lblStateCount
         // 
         this.lblStateCount.Location = new System.Drawing.Point(8, 88);
         this.lblStateCount.Name = "lblStateCount";
         this.lblStateCount.Size = new System.Drawing.Size(96, 20);
         this.lblStateCount.TabIndex = 1;
         this.lblStateCount.Text = "State Count:";
         this.lblStateCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblStateCountHeader
         // 
         this.lblStateCountHeader.Location = new System.Drawing.Point(8, 8);
         this.lblStateCountHeader.Name = "lblStateCountHeader";
         this.lblStateCountHeader.Size = new System.Drawing.Size(264, 80);
         this.lblStateCountHeader.TabIndex = 0;
         this.lblStateCountHeader.Text = "The number of states determines the increments in which the sprite rotates.  A st" +
            "ate count of 72 would result in a sprite with 72 rotation states, each represent" +
            "ing a 5 degree increment from the previous state.";
         // 
         // StateCount
         // 
         this.StateCount.StepControl = this.pnlStateCount;
         this.StateCount.TitleText = "State Count";
         // 
         // pnlReview
         // 
         this.pnlReview.Controls.Add(this.txtReview);
         this.pnlReview.Controls.Add(this.lblReview);
         this.pnlReview.Location = new System.Drawing.Point(-10168, 42);
         this.pnlReview.Name = "pnlReview";
         this.pnlReview.Size = new System.Drawing.Size(285, 231);
         this.pnlReview.TabIndex = 11;
         // 
         // txtReview
         // 
         this.txtReview.Location = new System.Drawing.Point(8, 48);
         this.txtReview.Multiline = true;
         this.txtReview.Name = "txtReview";
         this.txtReview.ReadOnly = true;
         this.txtReview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtReview.Size = new System.Drawing.Size(264, 176);
         this.txtReview.TabIndex = 1;
         this.txtReview.Text = "";
         // 
         // lblReview
         // 
         this.lblReview.Location = new System.Drawing.Point(8, 8);
         this.lblReview.Name = "lblReview";
         this.lblReview.Size = new System.Drawing.Size(264, 40);
         this.lblReview.TabIndex = 0;
         this.lblReview.Text = "The following actions will be performed when you click finish.";
         // 
         // Review
         // 
         this.Review.StepControl = this.pnlReview;
         this.Review.TitleText = "Review";
         this.Review.InitFunction += new System.EventHandler(this.Review_InitFunction);
         this.Review.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.Review_ValidateFunction);
         // 
         // pnlFrameSettings
         // 
         this.pnlFrameSettings.Controls.Add(this.nudAlpha);
         this.pnlFrameSettings.Controls.Add(this.lblAlpha);
         this.pnlFrameSettings.Controls.Add(this.nudRepeatCount);
         this.pnlFrameSettings.Controls.Add(this.lblRepeatCount);
         this.pnlFrameSettings.Controls.Add(this.lblFrameRepeatHeader);
         this.pnlFrameSettings.Location = new System.Drawing.Point(-10168, 42);
         this.pnlFrameSettings.Name = "pnlFrameSettings";
         this.pnlFrameSettings.Size = new System.Drawing.Size(284, 231);
         this.pnlFrameSettings.TabIndex = 12;
         // 
         // nudAlpha
         // 
         this.nudAlpha.Location = new System.Drawing.Point(120, 184);
         this.nudAlpha.Maximum = new System.Decimal(new int[] {
                                                                 255,
                                                                 0,
                                                                 0,
                                                                 0});
         this.nudAlpha.Name = "nudAlpha";
         this.nudAlpha.Size = new System.Drawing.Size(64, 20);
         this.nudAlpha.TabIndex = 4;
         // 
         // lblAlpha
         // 
         this.lblAlpha.Location = new System.Drawing.Point(8, 184);
         this.lblAlpha.Name = "lblAlpha";
         this.lblAlpha.Size = new System.Drawing.Size(112, 20);
         this.lblAlpha.TabIndex = 3;
         this.lblAlpha.Text = "Mask Alpha Level:";
         this.lblAlpha.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // nudRepeatCount
         // 
         this.nudRepeatCount.Location = new System.Drawing.Point(120, 152);
         this.nudRepeatCount.Maximum = new System.Decimal(new int[] {
                                                                       512,
                                                                       0,
                                                                       0,
                                                                       0});
         this.nudRepeatCount.Name = "nudRepeatCount";
         this.nudRepeatCount.Size = new System.Drawing.Size(64, 20);
         this.nudRepeatCount.TabIndex = 2;
         this.nudRepeatCount.Value = new System.Decimal(new int[] {
                                                                     1,
                                                                     0,
                                                                     0,
                                                                     0});
         // 
         // lblRepeatCount
         // 
         this.lblRepeatCount.Location = new System.Drawing.Point(8, 152);
         this.lblRepeatCount.Name = "lblRepeatCount";
         this.lblRepeatCount.Size = new System.Drawing.Size(112, 20);
         this.lblRepeatCount.TabIndex = 1;
         this.lblRepeatCount.Text = "Repeat Count:";
         this.lblRepeatCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblFrameRepeatHeader
         // 
         this.lblFrameRepeatHeader.Location = new System.Drawing.Point(8, 8);
         this.lblFrameRepeatHeader.Name = "lblFrameRepeatHeader";
         this.lblFrameRepeatHeader.Size = new System.Drawing.Size(264, 144);
         this.lblFrameRepeatHeader.TabIndex = 0;
         this.lblFrameRepeatHeader.Text = @"The frame repeat determines how long an animation frame is displayed before proceeding to the next animation frame within the state.  A setting of 0 will cause the frame to merge with the following frame.  The mask alpha level determines how sprite collisions in each frame are handled.  A high value requires pixels to be more opaque in order for collision to occur.  A value of 0 uses the solidity width and height.  The values specified below will be used for every frame appended to the sprite.";
         // 
         // FrameSettings
         // 
         this.FrameSettings.StepControl = this.pnlFrameSettings;
         this.FrameSettings.TitleText = "Frame Settings";
         // 
         // pnlBaseSize
         // 
         this.pnlBaseSize.Controls.Add(this.pnlPreviewSolid);
         this.pnlBaseSize.Controls.Add(this.lblBaseSizeHeader);
         this.pnlBaseSize.Controls.Add(this.nudSizingHeight);
         this.pnlBaseSize.Controls.Add(this.lblSizingHeight);
         this.pnlBaseSize.Controls.Add(this.nudSizingWidth);
         this.pnlBaseSize.Controls.Add(this.lblSizingWidth);
         this.pnlBaseSize.Location = new System.Drawing.Point(-10168, 42);
         this.pnlBaseSize.Name = "pnlBaseSize";
         this.pnlBaseSize.Size = new System.Drawing.Size(282, 231);
         this.pnlBaseSize.TabIndex = 13;
         // 
         // pnlPreviewSolid
         // 
         this.pnlPreviewSolid.Location = new System.Drawing.Point(160, 112);
         this.pnlPreviewSolid.Name = "pnlPreviewSolid";
         this.pnlPreviewSolid.Size = new System.Drawing.Size(112, 112);
         this.pnlPreviewSolid.TabIndex = 12;
         this.pnlPreviewSolid.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlPreviewSolid_Paint);
         // 
         // lblBaseSizeHeader
         // 
         this.lblBaseSizeHeader.Location = new System.Drawing.Point(8, 8);
         this.lblBaseSizeHeader.Name = "lblBaseSizeHeader";
         this.lblBaseSizeHeader.Size = new System.Drawing.Size(264, 120);
         this.lblBaseSizeHeader.TabIndex = 11;
         this.lblBaseSizeHeader.Text = @"You might want to change the rectangle that defines the boundaries of the solid area of the sprite if the graphic cell is larger than you want you solid area to be, or if some of the rotated states have an otherwise too-large solid area.  The size specified here will define the solid area of the un-rotated graphic cell, and will be centered in/over the cell's graphic.";
         // 
         // nudSizingHeight
         // 
         this.nudSizingHeight.Location = new System.Drawing.Point(96, 152);
         this.nudSizingHeight.Maximum = new System.Decimal(new int[] {
                                                                        512,
                                                                        0,
                                                                        0,
                                                                        0});
         this.nudSizingHeight.Minimum = new System.Decimal(new int[] {
                                                                        1,
                                                                        0,
                                                                        0,
                                                                        0});
         this.nudSizingHeight.Name = "nudSizingHeight";
         this.nudSizingHeight.Size = new System.Drawing.Size(56, 20);
         this.nudSizingHeight.TabIndex = 10;
         this.nudSizingHeight.Value = new System.Decimal(new int[] {
                                                                      1,
                                                                      0,
                                                                      0,
                                                                      0});
         this.nudSizingHeight.ValueChanged += new System.EventHandler(this.SolidArea_Changed);
         // 
         // lblSizingHeight
         // 
         this.lblSizingHeight.Location = new System.Drawing.Point(16, 152);
         this.lblSizingHeight.Name = "lblSizingHeight";
         this.lblSizingHeight.Size = new System.Drawing.Size(80, 20);
         this.lblSizingHeight.TabIndex = 9;
         this.lblSizingHeight.Text = "Solid Height:";
         this.lblSizingHeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // nudSizingWidth
         // 
         this.nudSizingWidth.Location = new System.Drawing.Point(96, 128);
         this.nudSizingWidth.Maximum = new System.Decimal(new int[] {
                                                                       512,
                                                                       0,
                                                                       0,
                                                                       0});
         this.nudSizingWidth.Minimum = new System.Decimal(new int[] {
                                                                       1,
                                                                       0,
                                                                       0,
                                                                       0});
         this.nudSizingWidth.Name = "nudSizingWidth";
         this.nudSizingWidth.Size = new System.Drawing.Size(56, 20);
         this.nudSizingWidth.TabIndex = 8;
         this.nudSizingWidth.Value = new System.Decimal(new int[] {
                                                                     1,
                                                                     0,
                                                                     0,
                                                                     0});
         this.nudSizingWidth.ValueChanged += new System.EventHandler(this.SolidArea_Changed);
         // 
         // lblSizingWidth
         // 
         this.lblSizingWidth.Location = new System.Drawing.Point(16, 128);
         this.lblSizingWidth.Name = "lblSizingWidth";
         this.lblSizingWidth.Size = new System.Drawing.Size(80, 20);
         this.lblSizingWidth.TabIndex = 7;
         this.lblSizingWidth.Text = "Solid Width:";
         this.lblSizingWidth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // BaseSize
         // 
         this.BaseSize.StepControl = this.pnlBaseSize;
         this.BaseSize.TitleText = "Base Size";
         // 
         // frmRotateWizard
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(450, 313);
         this.Controls.Add(this.pnlBaseSize);
         this.Controls.Add(this.pnlFrameSettings);
         this.Controls.Add(this.pnlReview);
         this.Controls.Add(this.pnlStateCount);
         this.Controls.Add(this.pnlPrefix);
         this.Controls.Add(this.pnlSizingStyle);
         this.Controls.Add(this.pnlFrameset);
         this.Controls.Add(this.pnlSourceGraphic);
         this.Name = "frmRotateWizard";
         this.Steps.Add(this.SourceGraphic);
         this.Steps.Add(this.StateCount);
         this.Steps.Add(this.FrameSettings);
         this.Steps.Add(this.Prefix);
         this.Steps.Add(this.TargetFrameset);
         this.Steps.Add(this.SizingStyle);
         this.Steps.Add(this.BaseSize);
         this.Steps.Add(this.Review);
         this.Text = "Rotating Sprite State Wizard";
         this.Controls.SetChildIndex(this.pnlSourceGraphic, 0);
         this.Controls.SetChildIndex(this.pnlFrameset, 0);
         this.Controls.SetChildIndex(this.pnlSizingStyle, 0);
         this.Controls.SetChildIndex(this.pnlPrefix, 0);
         this.Controls.SetChildIndex(this.pnlStateCount, 0);
         this.Controls.SetChildIndex(this.pnlReview, 0);
         this.Controls.SetChildIndex(this.pnlFrameSettings, 0);
         this.Controls.SetChildIndex(this.pnlBaseSize, 0);
         this.pnlSourceGraphic.ResumeLayout(false);
         this.pnlFrameset.ResumeLayout(false);
         this.pnlSizingStyle.ResumeLayout(false);
         this.pnlPrefix.ResumeLayout(false);
         this.pnlStateCount.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.nudStateCount)).EndInit();
         this.pnlReview.ResumeLayout(false);
         this.pnlFrameSettings.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudRepeatCount)).EndInit();
         this.pnlBaseSize.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.nudSizingHeight)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudSizingWidth)).EndInit();
         this.ResumeLayout(false);

      }
      #endregion

      private void cboGraphicSheet_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         SourceCell.GraphicSheet = (ProjectDataset.GraphicSheetRow)cboGraphicSheet.Items[cboGraphicSheet.SelectedIndex];
         SourceCell.CurrentCellIndex = 0;
         nudSizingWidth.Value = SourceCell.GraphicSheet.CellWidth;
         nudSizingHeight.Value = SourceCell.GraphicSheet.CellHeight;
      }

      private void SourceGraphic_InitFunction(object sender, System.EventArgs e)
      {
         if (cboGraphicSheet.Items.Count == 0)
            foreach(System.Data.DataRowView drv in ProjectData.GraphicSheet.DefaultView)
               cboGraphicSheet.Items.Add((ProjectDataset.GraphicSheetRow)drv.Row);
      }

      private bool SourceGraphic_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         if (SourceCell.CurrentCellIndex < 0)
         {
            MessageBox.Show(this, "A graphic sheet and cell must be selected.", "Specify Source Graphic", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         return true;
      }

      private void frmFrameset_InitFunction(object sender, System.EventArgs e)
      {
         if (cboFrameset.Items.Count == 0)
            foreach(System.Data.DataRowView drv in ProjectData.Frameset.DefaultView)
               cboFrameset.Items.Add((ProjectDataset.FramesetRow)drv.Row);
      }

      private bool frmFrameset_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         if (ProjectData.GetFrameSet(cboFrameset.Text) != null)
            return true;
         string errMsg = ProjectData.ValidateName(cboFrameset.Text);
         if (errMsg == null)
            return true;
         MessageBox.Show(this, errMsg, "Target Frameset", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         return false;
      }

      private bool Prefix_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         string errMsg = ProjectData.ValidateName(txtPrefix.Text);
         if (errMsg == null)
            return true;
         MessageBox.Show(this, errMsg, "Specify Prefix", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         return false;
      }

      private void Review_InitFunction(object sender, System.EventArgs e)
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();

         if (cboFrameset.Text.Length > 0)
         {
            if (ProjectData.GetFrameSet(cboFrameset.Text) == null)
               sb.Append("Create a new frameset named \"" + cboFrameset.Text + "\" and append " +
                  nudStateCount.Value.ToString() + " frames to it based on graphic sheet \"" +
                  cboGraphicSheet.Text + "\" cell number " + SourceCell.CurrentCellIndex.ToString() + ".\r\n");
            else
               sb.Append("Append " + nudStateCount.Value.ToString() + " frames to existing frameset \"" +
                  cboFrameset.Text + "\" based on graphic sheet \"" + cboGraphicSheet.Text +
                  "\" cell number " + SourceCell.CurrentCellIndex.ToString() + ".\r\n");
         }

         int newCount = 0;
         int insertPos = sb.Length;
         for (int i=0; i<(int)nudStateCount.Value; i++)
         {
            float angle = 360f * i / (float)nudStateCount.Value;
            string stateName = txtPrefix.Text + Math.Round(angle, 3).ToString().Replace(".","d");
            if (ProjectData.GetSpriteState(m_SpriteDefinition.Name, stateName) == null)
            {
               newCount++;
               sb.Append(stateName + " (new");
               if (rdoVariableSize.Checked)
               {
                  ProjectDataset.GraphicSheetRow drGfx = ((ProjectDataset.GraphicSheetRow)cboGraphicSheet.Items[cboGraphicSheet.SelectedIndex]);
                  using (System.Drawing.Drawing2D.Matrix m = CreateRotateMatrix(angle, drGfx))
                  {
                     Rectangle bounds = Rectangle.Round(GetRotatedBounds((int)nudSizingWidth.Value, (int)nudSizingHeight.Value, m));
                     sb.Append(" with solidity size " + bounds.Width.ToString() + "x" + bounds.Height.ToString());
                  }
               }
               sb.Append(")\r\n");
            }
            else
               sb.Append(stateName + " (append)\r\n");
         }
         if (newCount > 0)
            if (newCount < nudStateCount.Value)
               sb.Insert(insertPos, "Create " + newCount.ToString() + " new sprite states, and append to " +
                  (nudStateCount.Value - newCount).ToString() +
                  " existing sprite states having the prefix \"" + txtPrefix.Text + "\":\r\n");
            else
               sb.Insert(insertPos, "Create " + newCount.ToString() + " new sprite states with the prefix \"" +
                  txtPrefix.Text + "\":\r\n");
         else
            sb.Insert(insertPos, "Append frames to " + nudStateCount.Value +
               " existing sprite states having the prefix \"" + txtPrefix.Text + "\":\r\n");

         if (rdoFixedSize.Checked)
            sb.Append("Boundaries for added frames will be fixed around a centered rectangle sized at " +
               nudSizingWidth.Value.ToString() + "x" + nudSizingHeight.Value.ToString() + " pixels.\r\n");
         else
            sb.Append("Boundaries for added frames will vary to represent the limits of a rectangle of size " +
               nudSizingWidth.Value.ToString() + "x" + nudSizingHeight.Value.ToString() +
               " pixels rotated to the frame's angle.\r\n");

         sb.Append("For each new frame, the frame repeat count will be set to " + nudRepeatCount.Value.ToString() + ", and the alpha mask level will be set to " + nudAlpha.Value.ToString() + ".\r\n");

         txtReview.Text = sb.ToString();
      }
    
      private System.Drawing.RectangleF GetRotatedBounds(int CellWidth, int CellHeight, System.Drawing.Drawing2D.Matrix m)
      {
         RectangleF bounds;
         SizeF CellSize = new SizeF(CellWidth, CellHeight);
         PointF[] ptsRect = new PointF[]
            {
               new PointF(0, 0),
               new PointF(CellSize.Width, 0),
               new PointF(CellSize.Width, CellSize.Height),
               new PointF(0, CellSize.Height)
            };
         m.TransformPoints(ptsRect);
         bounds = new RectangleF(ptsRect[0], new SizeF(0,0));
         foreach (PointF pt in ptsRect)
         {
            if(pt.X < bounds.X)
            {
               bounds.Width += bounds.X - pt.X;
               bounds.X = pt.X;
            }
            if(pt.Y < bounds.Y)
            {
               bounds.Height += bounds.Y - pt.Y;
               bounds.Y = pt.Y;
            }
            if (pt.X > bounds.Right)
               bounds.Width += pt.X - bounds.Right;
            if (pt.Y > bounds.Bottom)
               bounds.Height += pt.Y - bounds.Bottom;
         }
         return bounds;
      }

      private bool Review_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         try
         {
            ProjectDataset.FramesetRow drNewFrameset = null;
            ProjectDataset.GraphicSheetRow drGfx = (ProjectDataset.GraphicSheetRow)(cboGraphicSheet.Items[cboGraphicSheet.SelectedIndex]);

            System.Collections.Hashtable htFrameIndexes = new Hashtable();

            for (int i=0; i<(int)nudStateCount.Value; i++)
            {
               float angle = 360f * i / (float)nudStateCount.Value;
               string stateName = txtPrefix.Text + Math.Round(angle, 3).ToString().Replace(".","d");
               using (System.Drawing.Drawing2D.Matrix m = CreateRotateMatrix(90f - angle, drGfx))
               {
                  RectangleF bounds = GetRotatedBounds((int)nudSizingWidth.Value, (int)nudSizingHeight.Value, m);
                  m.Translate(
                     (float)(nudSizingWidth.Value - drGfx.CellWidth)/2f,
                     (float)(nudSizingHeight.Value - drGfx.CellHeight)/2f,
                     System.Drawing.Drawing2D.MatrixOrder.Prepend);
                  if (rdoVariableSize.Checked)
                     m.Translate(-bounds.X, -bounds.Y, System.Drawing.Drawing2D.MatrixOrder.Append);

                  ProjectDataset.SpriteStateRow drState = ProjectData.GetSpriteState(m_SpriteDefinition.Name, stateName);
                  ProjectDataset.FramesetRow drFrameset;
                  int lastFrameValue;
                  if (drState == null)
                  {
                     if (drNewFrameset == null)
                        if ((drNewFrameset = ProjectData.GetFrameSet(cboFrameset.Text)) == null)
                           drNewFrameset = ProjectData.AddFramesetRow(cboFrameset.Text);

                     drFrameset = drNewFrameset;
                  }
                  else
                     drFrameset = drState.FramesetRow;
                  if (!htFrameIndexes.ContainsKey(drFrameset))
                  {
                     ProjectDataset.FrameRow[] existingFrames = ProjectData.GetSortedFrameRows(drFrameset);
                     if (existingFrames.Length > 0)
                        htFrameIndexes[drFrameset] = existingFrames[existingFrames.Length-1].FrameValue;
                     else
                        htFrameIndexes[drFrameset] = -1;
                  }
                  lastFrameValue = (int)(htFrameIndexes[drFrameset] = (int)(htFrameIndexes[drFrameset])+1);

                  ProjectData.AddFrameRow(drFrameset, lastFrameValue, drGfx.Name, (short)SourceCell.CurrentCellIndex,
                     m.Elements[0], m.Elements[1], m.Elements[2], m.Elements[3], m.Elements[4], m.Elements[5], -1);
                  if (drState == null)
                  {
                     short solidWidth, solidHeight;
                     if (rdoVariableSize.Checked)
                     {
                        solidWidth = (short)Math.Round(bounds.Width);
                        solidHeight = (short)Math.Round(bounds.Height);
                     }
                     else
                     {
                        solidWidth = (short)nudSizingWidth.Value;
                        solidHeight = (short)nudSizingHeight.Value;
                     }
                     drState = ProjectData.AddSpriteState(m_SpriteDefinition, stateName, drFrameset, solidWidth, solidHeight, -1);
                  }
                  ProjectData.InsertFrame(drState, -1, lastFrameValue, (short)nudRepeatCount.Value, (byte)nudAlpha.Value);
               }
            }
            return true;
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Rotating Sprite State Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
         }
      }

      private System.Drawing.Drawing2D.Matrix CreateRotateMatrix(float angle, ProjectDataset.GraphicSheetRow drGfx)
      {
         System.Drawing.Drawing2D.Matrix result = new System.Drawing.Drawing2D.Matrix();

         result.RotateAt(angle, new System.Drawing.PointF(
            (float)nudSizingWidth.Value/2f,
            (float)nudSizingHeight.Value/2f),
            System.Drawing.Drawing2D.MatrixOrder.Append);
         float[] elements = result.Elements;
         for (int i = 0; i<6; i++)
            elements[i] = (float)Math.Round(elements[i], 4);
         result.Dispose();
         result = new System.Drawing.Drawing2D.Matrix(
            elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);
         return result;
      }

      private void SolidArea_Changed(object sender, System.EventArgs e)
      {
         pnlPreviewSolid.Invalidate();
      }

      private void pnlPreviewSolid_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         ProjectDataset.GraphicSheetRow drGfx = (ProjectDataset.GraphicSheetRow)cboGraphicSheet.Items[cboGraphicSheet.SelectedIndex];
         Bitmap bmpGfx = ProjectData.GetGraphicSheetImage(drGfx.Name, false);
         int row = (int)(SourceCell.CurrentCellIndex / drGfx.Columns);
         int col = SourceCell.CurrentCellIndex % drGfx.Columns;
         Rectangle srcRect = new Rectangle(col * drGfx.CellWidth, row * drGfx.CellHeight, drGfx.CellWidth, drGfx.CellHeight);
         e.Graphics.DrawImage(bmpGfx, (pnlPreviewSolid.Width - drGfx.CellWidth) / 2,
            (pnlPreviewSolid.Height - drGfx.CellHeight) / 2, srcRect, GraphicsUnit.Pixel);
         Rectangle solidRect = new Rectangle((int)((pnlPreviewSolid.Width - nudSizingWidth.Value) / 2),
            (int)((pnlPreviewSolid.Height - (int)nudSizingHeight.Value) / 2),
            (int)nudSizingWidth.Value, (int)nudSizingHeight.Value);
         using (Brush solidBrush = new SolidBrush(Color.FromArgb(200,0,80,255)))
            e.Graphics.FillRectangle(solidBrush, solidRect);
      }

      private bool TargetFrameset_IsApplicableFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         for (int i=0; i<(int)nudStateCount.Value; i++)
         {
            float angle = 360f * i / (float)nudStateCount.Value;
            string stateName = txtPrefix.Text + Math.Round(angle, 3).ToString().Replace(".","d");
            if (null == ProjectData.GetSpriteState(m_SpriteDefinition.Name, stateName))
               return true;
         }
         cboFrameset.Text = string.Empty;
         return false;
      }
   }
}

