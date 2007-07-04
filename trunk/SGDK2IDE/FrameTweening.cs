using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SGDK2
{
	public class frmFrameTweening : SGDK2.frmWizardBase
	{
      #region Non-Control Members
      private ProjectDataset.FramesetRow m_frameset;
      private double m_SrcOffsetX;
      private double m_SrcOffsetY;
      private double m_SrcScaleX;
      private double m_SrcScaleY;
      private double m_EndOffsetX;
      private double m_EndOffsetY;
      private double m_EndScaleX;
      private double m_EndScaleY;
      #endregion

      #region Form Designer Members
      private System.Windows.Forms.Panel pnlMode;
      private System.Windows.Forms.Label lblIntro;
      private SGDK2.frmWizardBase.StepInfo Mode;
      private System.Windows.Forms.RadioButton rdoAppend;
      private System.Windows.Forms.RadioButton rdoUpdate;
      private System.Windows.Forms.Panel pnlFrameSource;
      private SGDK2.frmWizardBase.StepInfo FrameSource;
      private System.Windows.Forms.Label lblFrameSource;
      private System.Windows.Forms.Label lblGraphicSheet;
      private System.Windows.Forms.ComboBox cboGraphicSheet;
      private GraphicBrowser SourceCell;
      private System.Windows.Forms.Panel pnlFrameRange;
      private SGDK2.frmWizardBase.StepInfo FrameRange;
      private GraphicBrowser FrameSelection;
      private System.Windows.Forms.Label lblFrameRange;
      private System.Windows.Forms.Panel pnlStartingParams;
      private SGDK2.frmWizardBase.StepInfo StartingParameters;
      private System.Windows.Forms.Label lblStatingParameters;
      private System.Windows.Forms.Label lblSrcRotation;
      private System.Windows.Forms.NumericUpDown nudSrcRotation;
      private System.Windows.Forms.Label lblSrcScale;
      private System.Windows.Forms.Label lblSrcScaleX;
      private System.Windows.Forms.TextBox txtSrcScaleX;
      private System.Windows.Forms.Label lblSrcScaleY;
      private System.Windows.Forms.TextBox txtSrcScaleY;
      private System.Windows.Forms.TextBox txtSrcOffsetY;
      private System.Windows.Forms.Label lblSrcOffsetY;
      private System.Windows.Forms.TextBox txtSrcOffsetX;
      private System.Windows.Forms.Label lblSrcOffsetX;
      private System.Windows.Forms.Label lblSrcOffset;
      private System.Windows.Forms.Panel pnlEndingParams;
      private System.Windows.Forms.Label lblEndRotation;
      private System.Windows.Forms.Label lblEndingParams;
      private SGDK2.frmWizardBase.StepInfo EndingParameters;
      private System.Windows.Forms.TextBox txtEndOffsetY;
      private System.Windows.Forms.Label lblEndOffsetY;
      private System.Windows.Forms.TextBox txtEndOffsetX;
      private System.Windows.Forms.Label lblEndOffsetX;
      private System.Windows.Forms.Label lblEndOffset;
      private System.Windows.Forms.TextBox txtEndScaleY;
      private System.Windows.Forms.Label lblEndScaleY;
      private System.Windows.Forms.TextBox txtEndScaleX;
      private System.Windows.Forms.Label lblEndScaleX;
      private System.Windows.Forms.Label lblEndScale;
      private System.Windows.Forms.NumericUpDown nudEndRotation;
      private System.Windows.Forms.Panel pnlFrameCount;
      private SGDK2.frmWizardBase.StepInfo FrameCount;
      private System.Windows.Forms.Label lblFrameCount;
      private System.Windows.Forms.Label lblCount;
      private SGDK2.frmWizardBase.StepInfo Review;
      private System.Windows.Forms.Panel pnlReview;
      private System.Windows.Forms.Label lblReview;
      private System.Windows.Forms.TextBox txtReview;
      private System.Windows.Forms.NumericUpDown nudFrameCount;
		private System.ComponentModel.IContainer components = null;
      #endregion

		public frmFrameTweening(ProjectDataset.FramesetRow frameset)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			m_frameset = frameset;
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
         this.pnlMode = new System.Windows.Forms.Panel();
         this.rdoUpdate = new System.Windows.Forms.RadioButton();
         this.rdoAppend = new System.Windows.Forms.RadioButton();
         this.lblIntro = new System.Windows.Forms.Label();
         this.Mode = new SGDK2.frmWizardBase.StepInfo();
         this.pnlFrameSource = new System.Windows.Forms.Panel();
         this.SourceCell = new SGDK2.GraphicBrowser();
         this.cboGraphicSheet = new System.Windows.Forms.ComboBox();
         this.lblGraphicSheet = new System.Windows.Forms.Label();
         this.lblFrameSource = new System.Windows.Forms.Label();
         this.FrameSource = new SGDK2.frmWizardBase.StepInfo();
         this.pnlFrameRange = new System.Windows.Forms.Panel();
         this.lblFrameRange = new System.Windows.Forms.Label();
         this.FrameSelection = new SGDK2.GraphicBrowser();
         this.FrameRange = new SGDK2.frmWizardBase.StepInfo();
         this.pnlStartingParams = new System.Windows.Forms.Panel();
         this.txtSrcOffsetY = new System.Windows.Forms.TextBox();
         this.lblSrcOffsetY = new System.Windows.Forms.Label();
         this.txtSrcOffsetX = new System.Windows.Forms.TextBox();
         this.lblSrcOffsetX = new System.Windows.Forms.Label();
         this.lblSrcOffset = new System.Windows.Forms.Label();
         this.txtSrcScaleY = new System.Windows.Forms.TextBox();
         this.lblSrcScaleY = new System.Windows.Forms.Label();
         this.txtSrcScaleX = new System.Windows.Forms.TextBox();
         this.lblSrcScaleX = new System.Windows.Forms.Label();
         this.lblSrcScale = new System.Windows.Forms.Label();
         this.nudSrcRotation = new System.Windows.Forms.NumericUpDown();
         this.lblSrcRotation = new System.Windows.Forms.Label();
         this.lblStatingParameters = new System.Windows.Forms.Label();
         this.StartingParameters = new SGDK2.frmWizardBase.StepInfo();
         this.pnlEndingParams = new System.Windows.Forms.Panel();
         this.txtEndOffsetY = new System.Windows.Forms.TextBox();
         this.lblEndOffsetY = new System.Windows.Forms.Label();
         this.txtEndOffsetX = new System.Windows.Forms.TextBox();
         this.lblEndOffsetX = new System.Windows.Forms.Label();
         this.lblEndOffset = new System.Windows.Forms.Label();
         this.txtEndScaleY = new System.Windows.Forms.TextBox();
         this.lblEndScaleY = new System.Windows.Forms.Label();
         this.txtEndScaleX = new System.Windows.Forms.TextBox();
         this.lblEndScaleX = new System.Windows.Forms.Label();
         this.lblEndScale = new System.Windows.Forms.Label();
         this.nudEndRotation = new System.Windows.Forms.NumericUpDown();
         this.lblEndRotation = new System.Windows.Forms.Label();
         this.lblEndingParams = new System.Windows.Forms.Label();
         this.EndingParameters = new SGDK2.frmWizardBase.StepInfo();
         this.pnlFrameCount = new System.Windows.Forms.Panel();
         this.nudFrameCount = new System.Windows.Forms.NumericUpDown();
         this.lblCount = new System.Windows.Forms.Label();
         this.lblFrameCount = new System.Windows.Forms.Label();
         this.FrameCount = new SGDK2.frmWizardBase.StepInfo();
         this.Review = new SGDK2.frmWizardBase.StepInfo();
         this.pnlReview = new System.Windows.Forms.Panel();
         this.txtReview = new System.Windows.Forms.TextBox();
         this.lblReview = new System.Windows.Forms.Label();
         this.pnlMode.SuspendLayout();
         this.pnlFrameSource.SuspendLayout();
         this.pnlFrameRange.SuspendLayout();
         this.pnlStartingParams.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.nudSrcRotation)).BeginInit();
         this.pnlEndingParams.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.nudEndRotation)).BeginInit();
         this.pnlFrameCount.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.nudFrameCount)).BeginInit();
         this.pnlReview.SuspendLayout();
         this.SuspendLayout();
         // 
         // pnlMode
         // 
         this.pnlMode.Controls.Add(this.rdoUpdate);
         this.pnlMode.Controls.Add(this.rdoAppend);
         this.pnlMode.Controls.Add(this.lblIntro);
         this.pnlMode.Location = new System.Drawing.Point(168, 42);
         this.pnlMode.Name = "pnlMode";
         this.pnlMode.Size = new System.Drawing.Size(282, 231);
         this.pnlMode.TabIndex = 6;
         // 
         // rdoUpdate
         // 
         this.rdoUpdate.Location = new System.Drawing.Point(16, 144);
         this.rdoUpdate.Name = "rdoUpdate";
         this.rdoUpdate.Size = new System.Drawing.Size(256, 40);
         this.rdoUpdate.TabIndex = 2;
         this.rdoUpdate.Text = "Update existing frames between two selected frames according to the intermediate " +
            "transformations.";
         // 
         // rdoAppend
         // 
         this.rdoAppend.Location = new System.Drawing.Point(16, 96);
         this.rdoAppend.Name = "rdoAppend";
         this.rdoAppend.Size = new System.Drawing.Size(256, 32);
         this.rdoAppend.TabIndex = 1;
         this.rdoAppend.Text = "Append a specified number of new frames to the end of this frameset.";
         // 
         // lblIntro
         // 
         this.lblIntro.Location = new System.Drawing.Point(8, 8);
         this.lblIntro.Name = "lblIntro";
         this.lblIntro.Size = new System.Drawing.Size(264, 80);
         this.lblIntro.TabIndex = 0;
         this.lblIntro.Text = "This wizard will allow you to specify starting and ending values for rotation, sc" +
            "aling and positioning of a frame, and then apply intermediate transformations as" +
            " specified below.";
         // 
         // Mode
         // 
         this.Mode.StepControl = this.pnlMode;
         this.Mode.TitleText = "Specify Mode";
         // 
         // pnlFrameSource
         // 
         this.pnlFrameSource.Controls.Add(this.SourceCell);
         this.pnlFrameSource.Controls.Add(this.cboGraphicSheet);
         this.pnlFrameSource.Controls.Add(this.lblGraphicSheet);
         this.pnlFrameSource.Controls.Add(this.lblFrameSource);
         this.pnlFrameSource.Location = new System.Drawing.Point(-10168, 42);
         this.pnlFrameSource.Name = "pnlFrameSource";
         this.pnlFrameSource.Size = new System.Drawing.Size(285, 231);
         this.pnlFrameSource.TabIndex = 7;
         // 
         // SourceCell
         // 
         this.SourceCell.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.SourceCell.CellBorders = false;
         this.SourceCell.CellPadding = new System.Drawing.Size(2, 2);
         this.SourceCell.CellSize = new System.Drawing.Size(0, 0);
         this.SourceCell.CurrentCellIndex = -1;
         this.SourceCell.Frameset = null;
         this.SourceCell.FramesToDisplay = null;
         this.SourceCell.GraphicSheet = null;
         this.SourceCell.Location = new System.Drawing.Point(8, 96);
         this.SourceCell.Name = "SourceCell";
         this.SourceCell.SheetImage = null;
         this.SourceCell.Size = new System.Drawing.Size(264, 128);
         this.SourceCell.TabIndex = 3;
         // 
         // cboGraphicSheet
         // 
         this.cboGraphicSheet.DisplayMember = "Name";
         this.cboGraphicSheet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboGraphicSheet.Location = new System.Drawing.Point(112, 64);
         this.cboGraphicSheet.Name = "cboGraphicSheet";
         this.cboGraphicSheet.Size = new System.Drawing.Size(160, 21);
         this.cboGraphicSheet.TabIndex = 2;
         this.cboGraphicSheet.SelectedIndexChanged += new System.EventHandler(this.cboGraphicSheet_SelectedIndexChanged);
         // 
         // lblGraphicSheet
         // 
         this.lblGraphicSheet.Location = new System.Drawing.Point(8, 64);
         this.lblGraphicSheet.Name = "lblGraphicSheet";
         this.lblGraphicSheet.Size = new System.Drawing.Size(104, 21);
         this.lblGraphicSheet.TabIndex = 1;
         this.lblGraphicSheet.Text = "Graphic Sheet:";
         this.lblGraphicSheet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblFrameSource
         // 
         this.lblFrameSource.Location = new System.Drawing.Point(8, 8);
         this.lblFrameSource.Name = "lblFrameSource";
         this.lblFrameSource.Size = new System.Drawing.Size(264, 48);
         this.lblFrameSource.TabIndex = 0;
         this.lblFrameSource.Text = "Specify the graphic sheet and cell on which the new frames will be based.";
         // 
         // FrameSource
         // 
         this.FrameSource.StepControl = this.pnlFrameSource;
         this.FrameSource.TitleText = "";
         this.FrameSource.InitFunction += new System.EventHandler(this.FrameSource_InitFunction);
         this.FrameSource.IsApplicableFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.FrameSource_IsApplicableFunction);
         this.FrameSource.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.FrameSource_ValidateFunction);
         // 
         // pnlFrameRange
         // 
         this.pnlFrameRange.Controls.Add(this.lblFrameRange);
         this.pnlFrameRange.Controls.Add(this.FrameSelection);
         this.pnlFrameRange.Location = new System.Drawing.Point(-10168, 42);
         this.pnlFrameRange.Name = "pnlFrameRange";
         this.pnlFrameRange.Size = new System.Drawing.Size(285, 231);
         this.pnlFrameRange.TabIndex = 8;
         // 
         // lblFrameRange
         // 
         this.lblFrameRange.Location = new System.Drawing.Point(8, 8);
         this.lblFrameRange.Name = "lblFrameRange";
         this.lblFrameRange.Size = new System.Drawing.Size(264, 32);
         this.lblFrameRange.TabIndex = 1;
         this.lblFrameRange.Text = "Select the frames to be affected (hold Shift or Ctrl to select multiple frames).";
         // 
         // FrameSelection
         // 
         this.FrameSelection.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.FrameSelection.CellBorders = false;
         this.FrameSelection.CellPadding = new System.Drawing.Size(0, 0);
         this.FrameSelection.CellSize = new System.Drawing.Size(0, 0);
         this.FrameSelection.CurrentCellIndex = -1;
         this.FrameSelection.Frameset = null;
         this.FrameSelection.FramesToDisplay = null;
         this.FrameSelection.GraphicSheet = null;
         this.FrameSelection.Location = new System.Drawing.Point(8, 40);
         this.FrameSelection.Name = "FrameSelection";
         this.FrameSelection.SheetImage = null;
         this.FrameSelection.Size = new System.Drawing.Size(264, 184);
         this.FrameSelection.TabIndex = 0;
         // 
         // FrameRange
         // 
         this.FrameRange.StepControl = this.pnlFrameRange;
         this.FrameRange.TitleText = "Select Frames";
         this.FrameRange.InitFunction += new System.EventHandler(this.FrameRange_InitFunction);
         this.FrameRange.IsApplicableFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.FrameRange_IsApplicableFunction);
         this.FrameRange.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.FrameRange_ValidateFunction);
         // 
         // pnlStartingParams
         // 
         this.pnlStartingParams.Controls.Add(this.txtSrcOffsetY);
         this.pnlStartingParams.Controls.Add(this.lblSrcOffsetY);
         this.pnlStartingParams.Controls.Add(this.txtSrcOffsetX);
         this.pnlStartingParams.Controls.Add(this.lblSrcOffsetX);
         this.pnlStartingParams.Controls.Add(this.lblSrcOffset);
         this.pnlStartingParams.Controls.Add(this.txtSrcScaleY);
         this.pnlStartingParams.Controls.Add(this.lblSrcScaleY);
         this.pnlStartingParams.Controls.Add(this.txtSrcScaleX);
         this.pnlStartingParams.Controls.Add(this.lblSrcScaleX);
         this.pnlStartingParams.Controls.Add(this.lblSrcScale);
         this.pnlStartingParams.Controls.Add(this.nudSrcRotation);
         this.pnlStartingParams.Controls.Add(this.lblSrcRotation);
         this.pnlStartingParams.Controls.Add(this.lblStatingParameters);
         this.pnlStartingParams.Location = new System.Drawing.Point(-10168, 42);
         this.pnlStartingParams.Name = "pnlStartingParams";
         this.pnlStartingParams.Size = new System.Drawing.Size(284, 231);
         this.pnlStartingParams.TabIndex = 9;
         // 
         // txtSrcOffsetY
         // 
         this.txtSrcOffsetY.Location = new System.Drawing.Point(200, 112);
         this.txtSrcOffsetY.Name = "txtSrcOffsetY";
         this.txtSrcOffsetY.Size = new System.Drawing.Size(72, 20);
         this.txtSrcOffsetY.TabIndex = 12;
         this.txtSrcOffsetY.Text = "0.0";
         // 
         // lblSrcOffsetY
         // 
         this.lblSrcOffsetY.Location = new System.Drawing.Point(176, 112);
         this.lblSrcOffsetY.Name = "lblSrcOffsetY";
         this.lblSrcOffsetY.Size = new System.Drawing.Size(24, 20);
         this.lblSrcOffsetY.TabIndex = 11;
         this.lblSrcOffsetY.Text = "Y:";
         this.lblSrcOffsetY.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtSrcOffsetX
         // 
         this.txtSrcOffsetX.Location = new System.Drawing.Point(96, 112);
         this.txtSrcOffsetX.Name = "txtSrcOffsetX";
         this.txtSrcOffsetX.Size = new System.Drawing.Size(72, 20);
         this.txtSrcOffsetX.TabIndex = 10;
         this.txtSrcOffsetX.Text = "0.0";
         // 
         // lblSrcOffsetX
         // 
         this.lblSrcOffsetX.Location = new System.Drawing.Point(72, 112);
         this.lblSrcOffsetX.Name = "lblSrcOffsetX";
         this.lblSrcOffsetX.Size = new System.Drawing.Size(24, 20);
         this.lblSrcOffsetX.TabIndex = 9;
         this.lblSrcOffsetX.Text = "X:";
         this.lblSrcOffsetX.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblSrcOffset
         // 
         this.lblSrcOffset.Location = new System.Drawing.Point(8, 112);
         this.lblSrcOffset.Name = "lblSrcOffset";
         this.lblSrcOffset.Size = new System.Drawing.Size(56, 20);
         this.lblSrcOffset.TabIndex = 8;
         this.lblSrcOffset.Text = "Offset:";
         this.lblSrcOffset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtSrcScaleY
         // 
         this.txtSrcScaleY.Location = new System.Drawing.Point(200, 88);
         this.txtSrcScaleY.Name = "txtSrcScaleY";
         this.txtSrcScaleY.Size = new System.Drawing.Size(72, 20);
         this.txtSrcScaleY.TabIndex = 7;
         this.txtSrcScaleY.Text = "1.0";
         // 
         // lblSrcScaleY
         // 
         this.lblSrcScaleY.Location = new System.Drawing.Point(176, 88);
         this.lblSrcScaleY.Name = "lblSrcScaleY";
         this.lblSrcScaleY.Size = new System.Drawing.Size(24, 20);
         this.lblSrcScaleY.TabIndex = 6;
         this.lblSrcScaleY.Text = "Y:";
         this.lblSrcScaleY.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtSrcScaleX
         // 
         this.txtSrcScaleX.Location = new System.Drawing.Point(96, 88);
         this.txtSrcScaleX.Name = "txtSrcScaleX";
         this.txtSrcScaleX.Size = new System.Drawing.Size(72, 20);
         this.txtSrcScaleX.TabIndex = 5;
         this.txtSrcScaleX.Text = "1.0";
         // 
         // lblSrcScaleX
         // 
         this.lblSrcScaleX.Location = new System.Drawing.Point(72, 88);
         this.lblSrcScaleX.Name = "lblSrcScaleX";
         this.lblSrcScaleX.Size = new System.Drawing.Size(24, 20);
         this.lblSrcScaleX.TabIndex = 4;
         this.lblSrcScaleX.Text = "X:";
         this.lblSrcScaleX.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblSrcScale
         // 
         this.lblSrcScale.Location = new System.Drawing.Point(8, 88);
         this.lblSrcScale.Name = "lblSrcScale";
         this.lblSrcScale.Size = new System.Drawing.Size(56, 20);
         this.lblSrcScale.TabIndex = 3;
         this.lblSrcScale.Text = "Scale:";
         this.lblSrcScale.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // nudSrcRotation
         // 
         this.nudSrcRotation.Location = new System.Drawing.Point(96, 56);
         this.nudSrcRotation.Maximum = new System.Decimal(new int[] {
                                                                       720,
                                                                       0,
                                                                       0,
                                                                       0});
         this.nudSrcRotation.Minimum = new System.Decimal(new int[] {
                                                                       720,
                                                                       0,
                                                                       0,
                                                                       -2147483648});
         this.nudSrcRotation.Name = "nudSrcRotation";
         this.nudSrcRotation.Size = new System.Drawing.Size(72, 20);
         this.nudSrcRotation.TabIndex = 2;
         // 
         // lblSrcRotation
         // 
         this.lblSrcRotation.Location = new System.Drawing.Point(8, 56);
         this.lblSrcRotation.Name = "lblSrcRotation";
         this.lblSrcRotation.Size = new System.Drawing.Size(88, 20);
         this.lblSrcRotation.TabIndex = 1;
         this.lblSrcRotation.Text = "Rotation:";
         this.lblSrcRotation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblStatingParameters
         // 
         this.lblStatingParameters.Location = new System.Drawing.Point(8, 8);
         this.lblStatingParameters.Name = "lblStatingParameters";
         this.lblStatingParameters.Size = new System.Drawing.Size(264, 40);
         this.lblStatingParameters.TabIndex = 0;
         this.lblStatingParameters.Text = "Specify the transformation to be applied to the first frame in the series.";
         // 
         // StartingParameters
         // 
         this.StartingParameters.StepControl = this.pnlStartingParams;
         this.StartingParameters.TitleText = "Starting Parameters";
         this.StartingParameters.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.StartingParameters_ValidateFunction);
         // 
         // pnlEndingParams
         // 
         this.pnlEndingParams.Controls.Add(this.txtEndOffsetY);
         this.pnlEndingParams.Controls.Add(this.lblEndOffsetY);
         this.pnlEndingParams.Controls.Add(this.txtEndOffsetX);
         this.pnlEndingParams.Controls.Add(this.lblEndOffsetX);
         this.pnlEndingParams.Controls.Add(this.lblEndOffset);
         this.pnlEndingParams.Controls.Add(this.txtEndScaleY);
         this.pnlEndingParams.Controls.Add(this.lblEndScaleY);
         this.pnlEndingParams.Controls.Add(this.txtEndScaleX);
         this.pnlEndingParams.Controls.Add(this.lblEndScaleX);
         this.pnlEndingParams.Controls.Add(this.lblEndScale);
         this.pnlEndingParams.Controls.Add(this.nudEndRotation);
         this.pnlEndingParams.Controls.Add(this.lblEndRotation);
         this.pnlEndingParams.Controls.Add(this.lblEndingParams);
         this.pnlEndingParams.Location = new System.Drawing.Point(-10168, 42);
         this.pnlEndingParams.Name = "pnlEndingParams";
         this.pnlEndingParams.Size = new System.Drawing.Size(283, 231);
         this.pnlEndingParams.TabIndex = 10;
         // 
         // txtEndOffsetY
         // 
         this.txtEndOffsetY.Location = new System.Drawing.Point(200, 112);
         this.txtEndOffsetY.Name = "txtEndOffsetY";
         this.txtEndOffsetY.Size = new System.Drawing.Size(72, 20);
         this.txtEndOffsetY.TabIndex = 12;
         this.txtEndOffsetY.Text = "0";
         // 
         // lblEndOffsetY
         // 
         this.lblEndOffsetY.Location = new System.Drawing.Point(176, 112);
         this.lblEndOffsetY.Name = "lblEndOffsetY";
         this.lblEndOffsetY.Size = new System.Drawing.Size(24, 20);
         this.lblEndOffsetY.TabIndex = 11;
         this.lblEndOffsetY.Text = "Y:";
         this.lblEndOffsetY.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtEndOffsetX
         // 
         this.txtEndOffsetX.Location = new System.Drawing.Point(96, 112);
         this.txtEndOffsetX.Name = "txtEndOffsetX";
         this.txtEndOffsetX.Size = new System.Drawing.Size(72, 20);
         this.txtEndOffsetX.TabIndex = 10;
         this.txtEndOffsetX.Text = "0";
         // 
         // lblEndOffsetX
         // 
         this.lblEndOffsetX.Location = new System.Drawing.Point(72, 112);
         this.lblEndOffsetX.Name = "lblEndOffsetX";
         this.lblEndOffsetX.Size = new System.Drawing.Size(24, 20);
         this.lblEndOffsetX.TabIndex = 9;
         this.lblEndOffsetX.Text = "X:";
         this.lblEndOffsetX.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblEndOffset
         // 
         this.lblEndOffset.Location = new System.Drawing.Point(8, 112);
         this.lblEndOffset.Name = "lblEndOffset";
         this.lblEndOffset.Size = new System.Drawing.Size(56, 20);
         this.lblEndOffset.TabIndex = 8;
         this.lblEndOffset.Text = "Offset:";
         this.lblEndOffset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtEndScaleY
         // 
         this.txtEndScaleY.Location = new System.Drawing.Point(200, 88);
         this.txtEndScaleY.Name = "txtEndScaleY";
         this.txtEndScaleY.Size = new System.Drawing.Size(72, 20);
         this.txtEndScaleY.TabIndex = 7;
         this.txtEndScaleY.Text = "1.0";
         // 
         // lblEndScaleY
         // 
         this.lblEndScaleY.Location = new System.Drawing.Point(176, 88);
         this.lblEndScaleY.Name = "lblEndScaleY";
         this.lblEndScaleY.Size = new System.Drawing.Size(24, 20);
         this.lblEndScaleY.TabIndex = 6;
         this.lblEndScaleY.Text = "Y:";
         this.lblEndScaleY.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtEndScaleX
         // 
         this.txtEndScaleX.Location = new System.Drawing.Point(96, 88);
         this.txtEndScaleX.Name = "txtEndScaleX";
         this.txtEndScaleX.Size = new System.Drawing.Size(72, 20);
         this.txtEndScaleX.TabIndex = 5;
         this.txtEndScaleX.Text = "1.0";
         // 
         // lblEndScaleX
         // 
         this.lblEndScaleX.Location = new System.Drawing.Point(72, 88);
         this.lblEndScaleX.Name = "lblEndScaleX";
         this.lblEndScaleX.Size = new System.Drawing.Size(24, 20);
         this.lblEndScaleX.TabIndex = 4;
         this.lblEndScaleX.Text = "X:";
         this.lblEndScaleX.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblEndScale
         // 
         this.lblEndScale.Location = new System.Drawing.Point(8, 88);
         this.lblEndScale.Name = "lblEndScale";
         this.lblEndScale.Size = new System.Drawing.Size(56, 20);
         this.lblEndScale.TabIndex = 3;
         this.lblEndScale.Text = "Scale:";
         this.lblEndScale.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // nudEndRotation
         // 
         this.nudEndRotation.Location = new System.Drawing.Point(96, 56);
         this.nudEndRotation.Maximum = new System.Decimal(new int[] {
                                                                       720,
                                                                       0,
                                                                       0,
                                                                       0});
         this.nudEndRotation.Minimum = new System.Decimal(new int[] {
                                                                       720,
                                                                       0,
                                                                       0,
                                                                       -2147483648});
         this.nudEndRotation.Name = "nudEndRotation";
         this.nudEndRotation.Size = new System.Drawing.Size(72, 20);
         this.nudEndRotation.TabIndex = 2;
         // 
         // lblEndRotation
         // 
         this.lblEndRotation.Location = new System.Drawing.Point(8, 56);
         this.lblEndRotation.Name = "lblEndRotation";
         this.lblEndRotation.Size = new System.Drawing.Size(88, 20);
         this.lblEndRotation.TabIndex = 1;
         this.lblEndRotation.Text = "Rotation:";
         this.lblEndRotation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblEndingParams
         // 
         this.lblEndingParams.Location = new System.Drawing.Point(8, 8);
         this.lblEndingParams.Name = "lblEndingParams";
         this.lblEndingParams.Size = new System.Drawing.Size(264, 40);
         this.lblEndingParams.TabIndex = 0;
         this.lblEndingParams.Text = "Specify the transformation to be applied to the last frame in the series.";
         // 
         // EndingParameters
         // 
         this.EndingParameters.StepControl = this.pnlEndingParams;
         this.EndingParameters.TitleText = "Ending Parameters";
         this.EndingParameters.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.EndingParameters_ValidateFunction);
         // 
         // pnlFrameCount
         // 
         this.pnlFrameCount.Controls.Add(this.nudFrameCount);
         this.pnlFrameCount.Controls.Add(this.lblCount);
         this.pnlFrameCount.Controls.Add(this.lblFrameCount);
         this.pnlFrameCount.Location = new System.Drawing.Point(-10168, 42);
         this.pnlFrameCount.Name = "pnlFrameCount";
         this.pnlFrameCount.Size = new System.Drawing.Size(282, 231);
         this.pnlFrameCount.TabIndex = 11;
         // 
         // nudFrameCount
         // 
         this.nudFrameCount.Location = new System.Drawing.Point(120, 72);
         this.nudFrameCount.Maximum = new System.Decimal(new int[] {
                                                                      999,
                                                                      0,
                                                                      0,
                                                                      0});
         this.nudFrameCount.Minimum = new System.Decimal(new int[] {
                                                                      3,
                                                                      0,
                                                                      0,
                                                                      0});
         this.nudFrameCount.Name = "nudFrameCount";
         this.nudFrameCount.Size = new System.Drawing.Size(80, 20);
         this.nudFrameCount.TabIndex = 2;
         this.nudFrameCount.Value = new System.Decimal(new int[] {
                                                                    3,
                                                                    0,
                                                                    0,
                                                                    0});
         // 
         // lblCount
         // 
         this.lblCount.Location = new System.Drawing.Point(56, 72);
         this.lblCount.Name = "lblCount";
         this.lblCount.Size = new System.Drawing.Size(64, 20);
         this.lblCount.TabIndex = 1;
         this.lblCount.Text = "Count:";
         this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblFrameCount
         // 
         this.lblFrameCount.Location = new System.Drawing.Point(8, 8);
         this.lblFrameCount.Name = "lblFrameCount";
         this.lblFrameCount.Size = new System.Drawing.Size(264, 48);
         this.lblFrameCount.TabIndex = 0;
         this.lblFrameCount.Text = "Specify the number of frames to generate, including the starting and ending frame" +
            ".";
         // 
         // FrameCount
         // 
         this.FrameCount.StepControl = this.pnlFrameCount;
         this.FrameCount.TitleText = "Frame Count";
         this.FrameCount.IsApplicableFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.FrameCount_IsApplicableFunction);
         // 
         // Review
         // 
         this.Review.StepControl = this.pnlReview;
         this.Review.TitleText = "Review";
         this.Review.InitFunction += new System.EventHandler(this.Review_InitFunction);
         this.Review.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.Review_ValidateFunction);
         // 
         // pnlReview
         // 
         this.pnlReview.Controls.Add(this.txtReview);
         this.pnlReview.Controls.Add(this.lblReview);
         this.pnlReview.Location = new System.Drawing.Point(-10168, 42);
         this.pnlReview.Name = "pnlReview";
         this.pnlReview.Size = new System.Drawing.Size(283, 231);
         this.pnlReview.TabIndex = 12;
         // 
         // txtReview
         // 
         this.txtReview.Location = new System.Drawing.Point(8, 40);
         this.txtReview.Multiline = true;
         this.txtReview.Name = "txtReview";
         this.txtReview.ReadOnly = true;
         this.txtReview.ScrollBars = System.Windows.Forms.ScrollBars.Both;
         this.txtReview.Size = new System.Drawing.Size(264, 184);
         this.txtReview.TabIndex = 1;
         this.txtReview.Text = "";
         // 
         // lblReview
         // 
         this.lblReview.Location = new System.Drawing.Point(8, 8);
         this.lblReview.Name = "lblReview";
         this.lblReview.Size = new System.Drawing.Size(264, 32);
         this.lblReview.TabIndex = 0;
         this.lblReview.Text = "The wizard if ready to perform the following actions:";
         // 
         // frmFrameTweening
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.BackColor = System.Drawing.SystemColors.Control;
         this.ClientSize = new System.Drawing.Size(450, 313);
         this.Controls.Add(this.pnlReview);
         this.Controls.Add(this.pnlFrameCount);
         this.Controls.Add(this.pnlEndingParams);
         this.Controls.Add(this.pnlStartingParams);
         this.Controls.Add(this.pnlFrameRange);
         this.Controls.Add(this.pnlFrameSource);
         this.Controls.Add(this.pnlMode);
         this.Name = "frmFrameTweening";
         this.Steps.Add(this.Mode);
         this.Steps.Add(this.FrameSource);
         this.Steps.Add(this.FrameRange);
         this.Steps.Add(this.StartingParameters);
         this.Steps.Add(this.EndingParameters);
         this.Steps.Add(this.FrameCount);
         this.Steps.Add(this.Review);
         this.Controls.SetChildIndex(this.pnlMode, 0);
         this.Controls.SetChildIndex(this.pnlFrameSource, 0);
         this.Controls.SetChildIndex(this.pnlFrameRange, 0);
         this.Controls.SetChildIndex(this.pnlStartingParams, 0);
         this.Controls.SetChildIndex(this.pnlEndingParams, 0);
         this.Controls.SetChildIndex(this.pnlFrameCount, 0);
         this.Controls.SetChildIndex(this.pnlReview, 0);
         this.pnlMode.ResumeLayout(false);
         this.pnlFrameSource.ResumeLayout(false);
         this.pnlFrameRange.ResumeLayout(false);
         this.pnlStartingParams.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.nudSrcRotation)).EndInit();
         this.pnlEndingParams.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.nudEndRotation)).EndInit();
         this.pnlFrameCount.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.nudFrameCount)).EndInit();
         this.pnlReview.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion

      private bool FrameSource_IsApplicableFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         return rdoAppend.Checked;
      }

      private bool FrameRange_IsApplicableFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         return rdoUpdate.Checked;
      }

      private bool FrameRange_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         if (FrameSelection.GetSelectedCellCount() > 2)
            return true;
         MessageBox.Show(this, "At least 3 frames must be selected", CurrentStep.TitleText, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         return false;
      }

      private bool FrameSource_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         if (SourceCell.GetSelectedCellCount() > 0)
            return true;
         MessageBox.Show(this, "A graphic cell must be selected", CurrentStep.TitleText, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         return false;
      }

      private void FrameSource_InitFunction(object sender, System.EventArgs e)
      {
         foreach(System.Data.DataRowView drv in ProjectData.GraphicSheet.DefaultView)
         {
            cboGraphicSheet.Items.Add(drv.Row);
         }
      }

      private void cboGraphicSheet_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         SourceCell.GraphicSheet = (ProjectDataset.GraphicSheetRow)cboGraphicSheet.SelectedItem;
      }

      private void FrameRange_InitFunction(object sender, System.EventArgs e)
      {
         FrameSelection.Frameset = m_frameset;
      }

      private bool FrameCount_IsApplicableFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         return rdoAppend.Checked;
      }

      private bool StartingParameters_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         if (!double.TryParse(txtSrcOffsetX.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out m_SrcOffsetX))
         {
            MessageBox.Show(this, "Invalid X offset specified", CurrentStep.TitleText, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            txtSrcOffsetX.Focus();
            return false;
         }
         if (!double.TryParse(txtSrcOffsetY.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out m_SrcOffsetY))
         {
            MessageBox.Show(this, "Invalid Y offset specified", CurrentStep.TitleText, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            txtSrcOffsetY.Focus();
            return false;
         }
         if (!double.TryParse(txtSrcScaleX.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out m_SrcScaleX))
         {
            MessageBox.Show(this, "Invalid X Scale specified", CurrentStep.TitleText, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            txtSrcScaleX.Focus();
            return false;
         }
         if (!double.TryParse(txtSrcScaleY.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out m_SrcScaleY))
         {
            MessageBox.Show(this, "Invalid Y Scale specified", CurrentStep.TitleText, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            txtSrcScaleY.Focus();
            return false;
         }
         return true;
      }
      
      private bool EndingParameters_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         if (!double.TryParse(txtEndOffsetX.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out m_EndOffsetX))
         {
            MessageBox.Show(this, "Invalid X offset specified", CurrentStep.TitleText, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            txtEndOffsetX.Focus();
            return false;
         }
         if (!double.TryParse(txtEndOffsetY.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out m_EndOffsetY))
         {
            MessageBox.Show(this, "Invalid Y offset specified", CurrentStep.TitleText, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            txtEndOffsetY.Focus();
            return false;
         }
         if (!double.TryParse(txtEndScaleX.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out m_EndScaleX))
         {
            MessageBox.Show(this, "Invalid X Scale specified", CurrentStep.TitleText, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            txtEndScaleX.Focus();
            return false;
         }
         if (!double.TryParse(txtEndScaleY.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out m_EndScaleY))
         {
            MessageBox.Show(this, "Invalid Y Scale specified", CurrentStep.TitleText, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            txtEndScaleY.Focus();
            return false;
         }
         return true;
      }

      private void Review_InitFunction(object sender, System.EventArgs e)
      {
         int count;
         ProjectDataset.FrameRow[] selFrames = null;

         if (rdoAppend.Checked)
            count = (int)nudFrameCount.Value;
         else
         {
            selFrames = FrameSelection.GetSelectedFrames();
            count = selFrames.Length;
         }

         System.Text.StringBuilder sb = new System.Text.StringBuilder();
         
         ProjectDataset.FrameRow[] totalFrames = ProjectData.GetSortedFrameRows(m_frameset);
         int lastFrame = totalFrames[totalFrames.Length-1].FrameValue;

         for (int i=0; i < count; i++)
         {
            int rotation = (int)(nudSrcRotation.Value * (count - i - 1) + nudEndRotation.Value * i) / (count - 1);
            float scaleX = (float)(m_SrcScaleX * (count - i - 1) + m_EndScaleX * i) / (count - 1);
            float scaleY = (float)(m_SrcScaleY * (count - i - 1) + m_EndScaleY * i) / (count - 1);
            float offsetX = (float)(m_SrcOffsetX * (count - i - 1) + m_EndOffsetX * i) / (count - 1);
            float offsetY = (float)(m_SrcOffsetY * (count - i - 1) + m_EndOffsetY * i) / (count - 1);

            if (rdoAppend.Checked)
            {
               sb.AppendFormat("Append frame {0} with:\r\n", i+lastFrame+1);
            }
            else
            {
               sb.AppendFormat("Update frame {0} with:\r\n", selFrames[i].FrameValue);
            }
            if (rotation != 0)
               sb.AppendFormat("   Rotation: {0}\r\n", rotation);
            if ((scaleX != 1) || (scaleY != 1))
               sb.AppendFormat("   Scaling X,Y: {0},{1}\r\n", scaleX, scaleY);
            if ((offsetX != 0) || (offsetY != 0))
               sb.AppendFormat("   Offset X,Y: {0}, {1}\r\n", offsetX, offsetY);
         }

         txtReview.Text = sb.ToString();
      }

      private bool Review_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         int count;
         ProjectDataset.FrameRow[] selFrames = null;

         if (rdoAppend.Checked)
            count = (int)nudFrameCount.Value;
         else
         {
            selFrames = FrameSelection.GetSelectedFrames();
            count = selFrames.Length;
         }

         ProjectDataset.FrameRow[] totalFrames = ProjectData.GetSortedFrameRows(m_frameset);
         int lastFrame = totalFrames[totalFrames.Length-1].FrameValue;

         ProjectDataset.GraphicSheetRow gfx = (ProjectDataset.GraphicSheetRow)cboGraphicSheet.SelectedItem;

         for (int i=0; i < count; i++)
         {
            int rotation = (int)(nudSrcRotation.Value * (count - i - 1) + nudEndRotation.Value * i) / (count - 1);
            float scaleX = (float)(m_SrcScaleX * (count - i - 1) + m_EndScaleX * i) / (count - 1);
            float scaleY = (float)(m_SrcScaleY * (count - i - 1) + m_EndScaleY * i) / (count - 1);
            float offsetX = (float)(m_SrcOffsetX * (count - i - 1) + m_EndOffsetX * i) / (count - 1);
            float offsetY = (float)(m_SrcOffsetY * (count - i - 1) + m_EndOffsetY * i) / (count - 1);

            if (rdoAppend.Checked)
            {
               using (System.Drawing.Drawing2D.Matrix mtx = new System.Drawing.Drawing2D.Matrix())
               {
                  if (rotation != 0)
                     mtx.RotateAt(rotation, new PointF(gfx.CellWidth / 2.0f, gfx.CellHeight / 2.0f));
                  if ((scaleX != 1) || (scaleY != 1))
                     mtx.Scale(scaleX, scaleY);
                  if ((offsetX != 0) || (offsetY != 0))
                     mtx.Translate(offsetX, offsetY);
                  ProjectData.AddFrameRow(m_frameset, i+lastFrame+1, gfx.Name, (short)SourceCell.GetFirstSelectedCell(),
                     mtx.Elements[0], mtx.Elements[1], mtx.Elements[2],
                     mtx.Elements[3], mtx.Elements[4], mtx.Elements[5], -1);
               }
            }
            else
            {
               using (System.Drawing.Drawing2D.Matrix mtx = new System.Drawing.Drawing2D.Matrix(
                         selFrames[i].m11, selFrames[i].m12, selFrames[i].m21,
                         selFrames[i].m22, selFrames[i].dx, selFrames[i].dy))
               {
                  if (rotation != 0)
                  {
                     gfx = ProjectData.GetGraphicSheet(selFrames[i].GraphicSheet);
                     RectangleF rotatedBounds = SGDK2IDE.GetRotatedBounds(gfx.CellWidth, gfx.CellHeight, mtx);
                     mtx.RotateAt(rotation, new PointF(rotatedBounds.Left + rotatedBounds.Width / 2,
                        rotatedBounds.Top + rotatedBounds.Height / 2), System.Drawing.Drawing2D.MatrixOrder.Append);
                  }
                  if ((scaleX != 1) || (scaleY != 1))
                     mtx.Scale(scaleX, scaleY, System.Drawing.Drawing2D.MatrixOrder.Append);
                  if ((offsetX != 0) || (offsetY != 0))
                     mtx.Translate(offsetX, offsetY, System.Drawing.Drawing2D.MatrixOrder.Append);
                  selFrames[i].m11 = mtx.Elements[0];
                  selFrames[i].m12 = mtx.Elements[1];
                  selFrames[i].m21 = mtx.Elements[2];
                  selFrames[i].m22 = mtx.Elements[3];
                  selFrames[i].dx = mtx.Elements[4];
                  selFrames[i].dy = mtx.Elements[5];
               }
            }
         }
         return true;
      }
   }
}

