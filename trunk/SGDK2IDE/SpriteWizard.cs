using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SGDK2
{
	public class frmSpriteImportWizard : SGDK2.frmWizardBase
	{
      #region Non-control Members
      ProjectDataset dsSource = null;
      ProjectDataset.FramesetRow[] m_ImportFramesets = null;
      ProjectDataset.GraphicSheetRow[] m_ImportGfx = null;
      #endregion

      #region Form Designer Members
      private System.Windows.Forms.Panel pnlSpecifyFile;
      private SGDK2.frmWizardBase.StepInfo SpecifyFile;
      private System.Windows.Forms.Label lblSpecifyFile;
      private System.Windows.Forms.Label lblSourceFile;
      private System.Windows.Forms.TextBox txtSourceFile;
      private System.Windows.Forms.Button btnBrowse;
      private System.Windows.Forms.OpenFileDialog dlgSourceFile;
      private System.Windows.Forms.Panel pnlSpecifySprite;
      private SGDK2.frmWizardBase.StepInfo SpecifySprite;
      private System.Windows.Forms.Label lblSpecifySprite;
      private System.Windows.Forms.CheckedListBox chlSelectSprites;
      private System.Windows.Forms.Button btnSelectAllSprites;
      private System.Windows.Forms.Button btnDeselectAllSprites;
      private System.Windows.Forms.Label lblFileComment;
      private System.Windows.Forms.TextBox txtComment;
      private System.Windows.Forms.Panel pnlUniqueNames;
      private SGDK2.frmWizardBase.StepInfo UniqueNames;
      private System.Windows.Forms.Label lblUniqueNames;
      private System.Windows.Forms.DataGrid grdNameMap;
      private System.Windows.Forms.DataGridTableStyle gridStyle;
      private System.Windows.Forms.DataGridTextBoxColumn colStyleOldName;
      private System.Windows.Forms.DataGridTextBoxColumn colStyleNewName;
      private System.Data.DataSet dsMapping;
      private System.Data.DataTable dtSpriteNames;
      private System.Data.DataColumn dcOldName;
      private System.Data.DataColumn dcNewName;
      private System.Data.DataView dvSpriteNames;
      private System.Windows.Forms.Panel pnlMergeFramesets;
      private SGDK2.frmWizardBase.StepInfo MergeFramesets;
      private System.Windows.Forms.Label lblMergeFramesets;
      private System.Windows.Forms.DataGrid grdFramesets;
      private System.Data.DataTable dtFramesetNames;
      private System.Data.DataColumn dcOldFSName;
      private System.Data.DataColumn dcNewFSName;
      private System.Data.DataView dvFramesetNames;
      private System.Windows.Forms.DataGridTableStyle framesetTableStyle;
      private System.Windows.Forms.DataGridTextBoxColumn colOldFSName;
      private System.Windows.Forms.DataGridTextBoxColumn colNewFSName;
      private System.Windows.Forms.Panel pnlMergeGraphics;
      private SGDK2.frmWizardBase.StepInfo MergeGraphics;
      private System.Windows.Forms.Label lblMergeGraphics;
      private System.Windows.Forms.DataGrid grdGraphicNames;
      private System.Data.DataTable dtGraphicNames;
      private System.Data.DataColumn dcOldGSName;
      private System.Data.DataColumn dcNewGSName;
      private System.Data.DataView dvGraphicNames;
      private System.Windows.Forms.DataGridTableStyle graphicNameStyle;
      private System.Windows.Forms.DataGridTextBoxColumn colOldGSName;
      private System.Windows.Forms.DataGridTextBoxColumn colNewGSName;
      private System.Windows.Forms.Panel pnlReview;
      private SGDK2.frmWizardBase.StepInfo Review;
      private System.Windows.Forms.Label lblReview;
      private System.Windows.Forms.TextBox txtReview;
		private System.ComponentModel.IContainer components = null;
      #endregion

      #region Initialization and Clean-up
		public frmSpriteImportWizard()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
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
      #endregion

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.pnlSpecifyFile = new System.Windows.Forms.Panel();
         this.txtComment = new System.Windows.Forms.TextBox();
         this.lblFileComment = new System.Windows.Forms.Label();
         this.btnBrowse = new System.Windows.Forms.Button();
         this.txtSourceFile = new System.Windows.Forms.TextBox();
         this.lblSourceFile = new System.Windows.Forms.Label();
         this.lblSpecifyFile = new System.Windows.Forms.Label();
         this.SpecifyFile = new SGDK2.frmWizardBase.StepInfo();
         this.dlgSourceFile = new System.Windows.Forms.OpenFileDialog();
         this.pnlSpecifySprite = new System.Windows.Forms.Panel();
         this.btnDeselectAllSprites = new System.Windows.Forms.Button();
         this.btnSelectAllSprites = new System.Windows.Forms.Button();
         this.chlSelectSprites = new System.Windows.Forms.CheckedListBox();
         this.lblSpecifySprite = new System.Windows.Forms.Label();
         this.SpecifySprite = new SGDK2.frmWizardBase.StepInfo();
         this.pnlUniqueNames = new System.Windows.Forms.Panel();
         this.grdNameMap = new System.Windows.Forms.DataGrid();
         this.dvSpriteNames = new System.Data.DataView();
         this.dtSpriteNames = new System.Data.DataTable();
         this.dcOldName = new System.Data.DataColumn();
         this.dcNewName = new System.Data.DataColumn();
         this.gridStyle = new System.Windows.Forms.DataGridTableStyle();
         this.colStyleOldName = new System.Windows.Forms.DataGridTextBoxColumn();
         this.colStyleNewName = new System.Windows.Forms.DataGridTextBoxColumn();
         this.lblUniqueNames = new System.Windows.Forms.Label();
         this.UniqueNames = new SGDK2.frmWizardBase.StepInfo();
         this.dsMapping = new System.Data.DataSet();
         this.dtFramesetNames = new System.Data.DataTable();
         this.dcOldFSName = new System.Data.DataColumn();
         this.dcNewFSName = new System.Data.DataColumn();
         this.dtGraphicNames = new System.Data.DataTable();
         this.dcOldGSName = new System.Data.DataColumn();
         this.dcNewGSName = new System.Data.DataColumn();
         this.pnlMergeFramesets = new System.Windows.Forms.Panel();
         this.grdFramesets = new System.Windows.Forms.DataGrid();
         this.dvFramesetNames = new System.Data.DataView();
         this.framesetTableStyle = new System.Windows.Forms.DataGridTableStyle();
         this.colOldFSName = new System.Windows.Forms.DataGridTextBoxColumn();
         this.colNewFSName = new System.Windows.Forms.DataGridTextBoxColumn();
         this.lblMergeFramesets = new System.Windows.Forms.Label();
         this.MergeFramesets = new SGDK2.frmWizardBase.StepInfo();
         this.pnlMergeGraphics = new System.Windows.Forms.Panel();
         this.grdGraphicNames = new System.Windows.Forms.DataGrid();
         this.dvGraphicNames = new System.Data.DataView();
         this.graphicNameStyle = new System.Windows.Forms.DataGridTableStyle();
         this.colOldGSName = new System.Windows.Forms.DataGridTextBoxColumn();
         this.colNewGSName = new System.Windows.Forms.DataGridTextBoxColumn();
         this.lblMergeGraphics = new System.Windows.Forms.Label();
         this.MergeGraphics = new SGDK2.frmWizardBase.StepInfo();
         this.pnlReview = new System.Windows.Forms.Panel();
         this.txtReview = new System.Windows.Forms.TextBox();
         this.lblReview = new System.Windows.Forms.Label();
         this.Review = new SGDK2.frmWizardBase.StepInfo();
         this.pnlSpecifyFile.SuspendLayout();
         this.pnlSpecifySprite.SuspendLayout();
         this.pnlUniqueNames.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.grdNameMap)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.dvSpriteNames)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.dtSpriteNames)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.dsMapping)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.dtFramesetNames)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.dtGraphicNames)).BeginInit();
         this.pnlMergeFramesets.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.grdFramesets)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.dvFramesetNames)).BeginInit();
         this.pnlMergeGraphics.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.grdGraphicNames)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.dvGraphicNames)).BeginInit();
         this.pnlReview.SuspendLayout();
         this.SuspendLayout();
         // 
         // pnlSpecifyFile
         // 
         this.pnlSpecifyFile.Controls.Add(this.txtComment);
         this.pnlSpecifyFile.Controls.Add(this.lblFileComment);
         this.pnlSpecifyFile.Controls.Add(this.btnBrowse);
         this.pnlSpecifyFile.Controls.Add(this.txtSourceFile);
         this.pnlSpecifyFile.Controls.Add(this.lblSourceFile);
         this.pnlSpecifyFile.Controls.Add(this.lblSpecifyFile);
         this.pnlSpecifyFile.Location = new System.Drawing.Point(168, 42);
         this.pnlSpecifyFile.Name = "pnlSpecifyFile";
         this.pnlSpecifyFile.Size = new System.Drawing.Size(282, 231);
         this.pnlSpecifyFile.TabIndex = 6;
         // 
         // txtComment
         // 
         this.txtComment.Location = new System.Drawing.Point(80, 120);
         this.txtComment.Multiline = true;
         this.txtComment.Name = "txtComment";
         this.txtComment.ReadOnly = true;
         this.txtComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtComment.Size = new System.Drawing.Size(200, 88);
         this.txtComment.TabIndex = 5;
         this.txtComment.Text = "";
         // 
         // lblFileComment
         // 
         this.lblFileComment.Location = new System.Drawing.Point(0, 120);
         this.lblFileComment.Name = "lblFileComment";
         this.lblFileComment.Size = new System.Drawing.Size(80, 16);
         this.lblFileComment.TabIndex = 4;
         this.lblFileComment.Text = "File Comment:";
         // 
         // btnBrowse
         // 
         this.btnBrowse.Location = new System.Drawing.Point(248, 80);
         this.btnBrowse.Name = "btnBrowse";
         this.btnBrowse.Size = new System.Drawing.Size(24, 20);
         this.btnBrowse.TabIndex = 3;
         this.btnBrowse.Text = "...";
         this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
         // 
         // txtSourceFile
         // 
         this.txtSourceFile.Location = new System.Drawing.Point(80, 80);
         this.txtSourceFile.Name = "txtSourceFile";
         this.txtSourceFile.Size = new System.Drawing.Size(168, 20);
         this.txtSourceFile.TabIndex = 2;
         this.txtSourceFile.Text = "";
         this.txtSourceFile.TextChanged += new System.EventHandler(this.txtSourceFile_TextChanged);
         // 
         // lblSourceFile
         // 
         this.lblSourceFile.Location = new System.Drawing.Point(0, 80);
         this.lblSourceFile.Name = "lblSourceFile";
         this.lblSourceFile.Size = new System.Drawing.Size(80, 20);
         this.lblSourceFile.TabIndex = 1;
         this.lblSourceFile.Text = "Source File:";
         this.lblSourceFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblSpecifyFile
         // 
         this.lblSpecifyFile.Dock = System.Windows.Forms.DockStyle.Top;
         this.lblSpecifyFile.Location = new System.Drawing.Point(0, 0);
         this.lblSpecifyFile.Name = "lblSpecifyFile";
         this.lblSpecifyFile.Size = new System.Drawing.Size(282, 64);
         this.lblSpecifyFile.TabIndex = 0;
         this.lblSpecifyFile.Text = "Sprites can be imported from pre-packaged template files, exported sprite definit" +
            "ion files, or SGDK2 project files.  All these files are actually in the same for" +
            "mat as a standard SGDK2 project file.";
         // 
         // SpecifyFile
         // 
         this.SpecifyFile.StepControl = this.pnlSpecifyFile;
         this.SpecifyFile.TitleText = "Specify Import Source";
         this.SpecifyFile.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.SpecifyFile_ValidateFunction);
         // 
         // dlgSourceFile
         // 
         this.dlgSourceFile.DefaultExt = "sgdk2";
         this.dlgSourceFile.Filter = "SGDK2 Project (*.sgdk2)|*.SGDK2|All Files (*.*)|*.*";
         this.dlgSourceFile.Title = "Specify Import Source";
         // 
         // pnlSpecifySprite
         // 
         this.pnlSpecifySprite.Controls.Add(this.btnDeselectAllSprites);
         this.pnlSpecifySprite.Controls.Add(this.btnSelectAllSprites);
         this.pnlSpecifySprite.Controls.Add(this.chlSelectSprites);
         this.pnlSpecifySprite.Controls.Add(this.lblSpecifySprite);
         this.pnlSpecifySprite.Location = new System.Drawing.Point(-10168, 42);
         this.pnlSpecifySprite.Name = "pnlSpecifySprite";
         this.pnlSpecifySprite.Size = new System.Drawing.Size(288, 231);
         this.pnlSpecifySprite.TabIndex = 7;
         // 
         // btnDeselectAllSprites
         // 
         this.btnDeselectAllSprites.Location = new System.Drawing.Point(184, 96);
         this.btnDeselectAllSprites.Name = "btnDeselectAllSprites";
         this.btnDeselectAllSprites.Size = new System.Drawing.Size(88, 24);
         this.btnDeselectAllSprites.TabIndex = 3;
         this.btnDeselectAllSprites.Text = "Select N&one";
         this.btnDeselectAllSprites.Click += new System.EventHandler(this.btnDeselectAllSprites_Click);
         // 
         // btnSelectAllSprites
         // 
         this.btnSelectAllSprites.Location = new System.Drawing.Point(184, 64);
         this.btnSelectAllSprites.Name = "btnSelectAllSprites";
         this.btnSelectAllSprites.Size = new System.Drawing.Size(88, 24);
         this.btnSelectAllSprites.TabIndex = 2;
         this.btnSelectAllSprites.Text = "Select &All";
         this.btnSelectAllSprites.Click += new System.EventHandler(this.btnSelectAllSprites_Click);
         // 
         // chlSelectSprites
         // 
         this.chlSelectSprites.CheckOnClick = true;
         this.chlSelectSprites.Location = new System.Drawing.Point(8, 64);
         this.chlSelectSprites.Name = "chlSelectSprites";
         this.chlSelectSprites.Size = new System.Drawing.Size(168, 154);
         this.chlSelectSprites.TabIndex = 1;
         // 
         // lblSpecifySprite
         // 
         this.lblSpecifySprite.Dock = System.Windows.Forms.DockStyle.Top;
         this.lblSpecifySprite.Location = new System.Drawing.Point(0, 0);
         this.lblSpecifySprite.Name = "lblSpecifySprite";
         this.lblSpecifySprite.Size = new System.Drawing.Size(288, 48);
         this.lblSpecifySprite.TabIndex = 0;
         this.lblSpecifySprite.Text = "The specified import source contains multiple sprites.  Select one or more sprite" +
            "s that you want to import.";
         // 
         // SpecifySprite
         // 
         this.SpecifySprite.StepControl = this.pnlSpecifySprite;
         this.SpecifySprite.TitleText = "Specify Sprite";
         this.SpecifySprite.InitFunction += new System.EventHandler(this.SpecifySprite_InitFunction);
         this.SpecifySprite.IsApplicableFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.SpecifySprite_IsApplicableFunction);
         this.SpecifySprite.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.SpecifySprite_ValidateFunction);
         // 
         // pnlUniqueNames
         // 
         this.pnlUniqueNames.Controls.Add(this.grdNameMap);
         this.pnlUniqueNames.Controls.Add(this.lblUniqueNames);
         this.pnlUniqueNames.Location = new System.Drawing.Point(-10168, 42);
         this.pnlUniqueNames.Name = "pnlUniqueNames";
         this.pnlUniqueNames.Size = new System.Drawing.Size(285, 231);
         this.pnlUniqueNames.TabIndex = 8;
         // 
         // grdNameMap
         // 
         this.grdNameMap.CaptionVisible = false;
         this.grdNameMap.DataMember = "";
         this.grdNameMap.DataSource = this.dvSpriteNames;
         this.grdNameMap.Dock = System.Windows.Forms.DockStyle.Fill;
         this.grdNameMap.HeaderForeColor = System.Drawing.SystemColors.ControlText;
         this.grdNameMap.Location = new System.Drawing.Point(0, 56);
         this.grdNameMap.Name = "grdNameMap";
         this.grdNameMap.RowHeadersVisible = false;
         this.grdNameMap.Size = new System.Drawing.Size(285, 175);
         this.grdNameMap.TabIndex = 1;
         this.grdNameMap.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
                                                                                               this.gridStyle});
         // 
         // dvSpriteNames
         // 
         this.dvSpriteNames.AllowDelete = false;
         this.dvSpriteNames.AllowNew = false;
         this.dvSpriteNames.Table = this.dtSpriteNames;
         // 
         // dtSpriteNames
         // 
         this.dtSpriteNames.Columns.AddRange(new System.Data.DataColumn[] {
                                                                             this.dcOldName,
                                                                             this.dcNewName});
         this.dtSpriteNames.Constraints.AddRange(new System.Data.Constraint[] {
                                                                                 new System.Data.UniqueConstraint("Constraint1", new string[] {
                                                                                                                                                 "Old Name"}, true)});
         this.dtSpriteNames.PrimaryKey = new System.Data.DataColumn[] {
                                                                         this.dcOldName};
         this.dtSpriteNames.TableName = "SpriteNames";
         // 
         // dcOldName
         // 
         this.dcOldName.AllowDBNull = false;
         this.dcOldName.ColumnName = "Old Name";
         this.dcOldName.ReadOnly = true;
         // 
         // dcNewName
         // 
         this.dcNewName.ColumnName = "New Name";
         // 
         // gridStyle
         // 
         this.gridStyle.DataGrid = this.grdNameMap;
         this.gridStyle.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
                                                                                                    this.colStyleOldName,
                                                                                                    this.colStyleNewName});
         this.gridStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText;
         this.gridStyle.MappingName = "SpriteNames";
         this.gridStyle.RowHeadersVisible = false;
         // 
         // colStyleOldName
         // 
         this.colStyleOldName.Format = "";
         this.colStyleOldName.FormatInfo = null;
         this.colStyleOldName.HeaderText = "Old (Source) Name";
         this.colStyleOldName.MappingName = "Old Name";
         this.colStyleOldName.NullText = "";
         this.colStyleOldName.ReadOnly = true;
         this.colStyleOldName.Width = 136;
         // 
         // colStyleNewName
         // 
         this.colStyleNewName.Format = "";
         this.colStyleNewName.FormatInfo = null;
         this.colStyleNewName.HeaderText = "New (Target) Name";
         this.colStyleNewName.MappingName = "New Name";
         this.colStyleNewName.NullText = "";
         this.colStyleNewName.Width = 136;
         // 
         // lblUniqueNames
         // 
         this.lblUniqueNames.Dock = System.Windows.Forms.DockStyle.Top;
         this.lblUniqueNames.Location = new System.Drawing.Point(0, 0);
         this.lblUniqueNames.Name = "lblUniqueNames";
         this.lblUniqueNames.Size = new System.Drawing.Size(285, 56);
         this.lblUniqueNames.TabIndex = 0;
         this.lblUniqueNames.Text = "Imported sprite definition names are duplicates of sprite definition names that a" +
            "lready exist in the project.  Specify unique names for the imported sprite defin" +
            "itions.";
         // 
         // UniqueNames
         // 
         this.UniqueNames.StepControl = this.pnlUniqueNames;
         this.UniqueNames.TitleText = "Specify Unique Names";
         this.UniqueNames.InitFunction += new System.EventHandler(this.UniqueNames_InitFunction);
         this.UniqueNames.IsApplicableFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.UniqueNames_IsApplicableFunction);
         this.UniqueNames.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.UniqueNames_ValidateFunction);
         // 
         // dsMapping
         // 
         this.dsMapping.DataSetName = "Name Mapping";
         this.dsMapping.Locale = new System.Globalization.CultureInfo("en-US");
         this.dsMapping.Tables.AddRange(new System.Data.DataTable[] {
                                                                       this.dtSpriteNames,
                                                                       this.dtFramesetNames,
                                                                       this.dtGraphicNames});
         // 
         // dtFramesetNames
         // 
         this.dtFramesetNames.Columns.AddRange(new System.Data.DataColumn[] {
                                                                               this.dcOldFSName,
                                                                               this.dcNewFSName});
         this.dtFramesetNames.Constraints.AddRange(new System.Data.Constraint[] {
                                                                                   new System.Data.UniqueConstraint("Constraint1", new string[] {
                                                                                                                                                   "Old Name"}, true)});
         this.dtFramesetNames.PrimaryKey = new System.Data.DataColumn[] {
                                                                           this.dcOldFSName};
         this.dtFramesetNames.TableName = "FramesetNames";
         // 
         // dcOldFSName
         // 
         this.dcOldFSName.AllowDBNull = false;
         this.dcOldFSName.ColumnName = "Old Name";
         this.dcOldFSName.ReadOnly = true;
         // 
         // dcNewFSName
         // 
         this.dcNewFSName.ColumnName = "New Name";
         // 
         // dtGraphicNames
         // 
         this.dtGraphicNames.Columns.AddRange(new System.Data.DataColumn[] {
                                                                              this.dcOldGSName,
                                                                              this.dcNewGSName});
         this.dtGraphicNames.Constraints.AddRange(new System.Data.Constraint[] {
                                                                                  new System.Data.UniqueConstraint("Constraint1", new string[] {
                                                                                                                                                  "Old Name"}, true)});
         this.dtGraphicNames.PrimaryKey = new System.Data.DataColumn[] {
                                                                          this.dcOldGSName};
         this.dtGraphicNames.TableName = "GraphicNames";
         // 
         // dcOldGSName
         // 
         this.dcOldGSName.AllowDBNull = false;
         this.dcOldGSName.ColumnName = "Old Name";
         this.dcOldGSName.ReadOnly = true;
         // 
         // dcNewGSName
         // 
         this.dcNewGSName.ColumnName = "New Name";
         // 
         // pnlMergeFramesets
         // 
         this.pnlMergeFramesets.Controls.Add(this.grdFramesets);
         this.pnlMergeFramesets.Controls.Add(this.lblMergeFramesets);
         this.pnlMergeFramesets.Location = new System.Drawing.Point(-10168, 42);
         this.pnlMergeFramesets.Name = "pnlMergeFramesets";
         this.pnlMergeFramesets.Size = new System.Drawing.Size(281, 231);
         this.pnlMergeFramesets.TabIndex = 9;
         // 
         // grdFramesets
         // 
         this.grdFramesets.CaptionVisible = false;
         this.grdFramesets.DataMember = "";
         this.grdFramesets.DataSource = this.dvFramesetNames;
         this.grdFramesets.Dock = System.Windows.Forms.DockStyle.Fill;
         this.grdFramesets.HeaderForeColor = System.Drawing.SystemColors.ControlText;
         this.grdFramesets.Location = new System.Drawing.Point(0, 104);
         this.grdFramesets.Name = "grdFramesets";
         this.grdFramesets.RowHeadersVisible = false;
         this.grdFramesets.Size = new System.Drawing.Size(281, 127);
         this.grdFramesets.TabIndex = 1;
         this.grdFramesets.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
                                                                                                 this.framesetTableStyle});
         // 
         // dvFramesetNames
         // 
         this.dvFramesetNames.AllowDelete = false;
         this.dvFramesetNames.AllowNew = false;
         this.dvFramesetNames.Table = this.dtFramesetNames;
         // 
         // framesetTableStyle
         // 
         this.framesetTableStyle.DataGrid = this.grdFramesets;
         this.framesetTableStyle.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
                                                                                                             this.colOldFSName,
                                                                                                             this.colNewFSName});
         this.framesetTableStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText;
         this.framesetTableStyle.MappingName = "FramesetNames";
         this.framesetTableStyle.RowHeadersVisible = false;
         // 
         // colOldFSName
         // 
         this.colOldFSName.Format = "";
         this.colOldFSName.FormatInfo = null;
         this.colOldFSName.HeaderText = "Old (Source) Name";
         this.colOldFSName.MappingName = "Old Name";
         this.colOldFSName.NullText = "";
         this.colOldFSName.ReadOnly = true;
         this.colOldFSName.Width = 136;
         // 
         // colNewFSName
         // 
         this.colNewFSName.Format = "";
         this.colNewFSName.FormatInfo = null;
         this.colNewFSName.HeaderText = "New (Target) Name";
         this.colNewFSName.MappingName = "New Name";
         this.colNewFSName.NullText = "";
         this.colNewFSName.Width = 136;
         // 
         // lblMergeFramesets
         // 
         this.lblMergeFramesets.Dock = System.Windows.Forms.DockStyle.Top;
         this.lblMergeFramesets.Location = new System.Drawing.Point(0, 0);
         this.lblMergeFramesets.Name = "lblMergeFramesets";
         this.lblMergeFramesets.Size = new System.Drawing.Size(281, 104);
         this.lblMergeFramesets.TabIndex = 0;
         this.lblMergeFramesets.Text = @"Some imported frameset names match those of existing framesets in the project.  When this occurs, and an adequate number of frames exist in the existing frameset, you can choose whether to use the existing frameset or import the sprite definition's frameset as a new object.  To use the existing frameset, leave the new name the same as the original.";
         // 
         // MergeFramesets
         // 
         this.MergeFramesets.StepControl = this.pnlMergeFramesets;
         this.MergeFramesets.TitleText = "Merge Framesets";
         this.MergeFramesets.InitFunction += new System.EventHandler(this.MergeFramesets_InitFunction);
         this.MergeFramesets.IsApplicableFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.MergeFramesets_IsApplicableFunction);
         this.MergeFramesets.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.MergeFramesets_ValidateFunction);
         // 
         // pnlMergeGraphics
         // 
         this.pnlMergeGraphics.Controls.Add(this.grdGraphicNames);
         this.pnlMergeGraphics.Controls.Add(this.lblMergeGraphics);
         this.pnlMergeGraphics.Location = new System.Drawing.Point(-10168, 42);
         this.pnlMergeGraphics.Name = "pnlMergeGraphics";
         this.pnlMergeGraphics.Size = new System.Drawing.Size(281, 231);
         this.pnlMergeGraphics.TabIndex = 10;
         // 
         // grdGraphicNames
         // 
         this.grdGraphicNames.CaptionVisible = false;
         this.grdGraphicNames.DataMember = "";
         this.grdGraphicNames.DataSource = this.dvGraphicNames;
         this.grdGraphicNames.Dock = System.Windows.Forms.DockStyle.Fill;
         this.grdGraphicNames.HeaderForeColor = System.Drawing.SystemColors.ControlText;
         this.grdGraphicNames.Location = new System.Drawing.Point(0, 88);
         this.grdGraphicNames.Name = "grdGraphicNames";
         this.grdGraphicNames.Size = new System.Drawing.Size(281, 143);
         this.grdGraphicNames.TabIndex = 1;
         this.grdGraphicNames.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
                                                                                                    this.graphicNameStyle});
         // 
         // dvGraphicNames
         // 
         this.dvGraphicNames.AllowDelete = false;
         this.dvGraphicNames.AllowNew = false;
         this.dvGraphicNames.Table = this.dtGraphicNames;
         // 
         // graphicNameStyle
         // 
         this.graphicNameStyle.DataGrid = this.grdGraphicNames;
         this.graphicNameStyle.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
                                                                                                           this.colOldGSName,
                                                                                                           this.colNewGSName});
         this.graphicNameStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText;
         this.graphicNameStyle.MappingName = "GraphicNames";
         this.graphicNameStyle.RowHeadersVisible = false;
         // 
         // colOldGSName
         // 
         this.colOldGSName.Format = "";
         this.colOldGSName.FormatInfo = null;
         this.colOldGSName.HeaderText = "Old (Source) Name";
         this.colOldGSName.MappingName = "Old Name";
         this.colOldGSName.NullText = "";
         this.colOldGSName.ReadOnly = true;
         this.colOldGSName.Width = 136;
         // 
         // colNewGSName
         // 
         this.colNewGSName.Format = "";
         this.colNewGSName.FormatInfo = null;
         this.colNewGSName.HeaderText = "New (Target) Name";
         this.colNewGSName.MappingName = "New Name";
         this.colNewGSName.NullText = "";
         this.colNewGSName.Width = 136;
         // 
         // lblMergeGraphics
         // 
         this.lblMergeGraphics.Dock = System.Windows.Forms.DockStyle.Top;
         this.lblMergeGraphics.Location = new System.Drawing.Point(0, 0);
         this.lblMergeGraphics.Name = "lblMergeGraphics";
         this.lblMergeGraphics.Size = new System.Drawing.Size(281, 88);
         this.lblMergeGraphics.TabIndex = 0;
         this.lblMergeGraphics.Text = @"Some Graphic Sheets in the source file use the same name as existing Graphic Sheets in the project.  You may choose to import the graphics from the source file under a new name, or, if the existing Graphic Sheet has enough cells, use the existing sheet by leaving the name identical.";
         // 
         // MergeGraphics
         // 
         this.MergeGraphics.StepControl = this.pnlMergeGraphics;
         this.MergeGraphics.TitleText = "Merge Graphics";
         this.MergeGraphics.InitFunction += new System.EventHandler(this.MergeGraphics_InitFunction);
         this.MergeGraphics.IsApplicableFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.MergeGraphics_IsApplicableFunction);
         this.MergeGraphics.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.MergeGraphics_ValidateFunction);
         // 
         // pnlReview
         // 
         this.pnlReview.Controls.Add(this.txtReview);
         this.pnlReview.Controls.Add(this.lblReview);
         this.pnlReview.Location = new System.Drawing.Point(-10168, 42);
         this.pnlReview.Name = "pnlReview";
         this.pnlReview.Size = new System.Drawing.Size(281, 231);
         this.pnlReview.TabIndex = 11;
         // 
         // txtReview
         // 
         this.txtReview.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtReview.Location = new System.Drawing.Point(0, 56);
         this.txtReview.Multiline = true;
         this.txtReview.Name = "txtReview";
         this.txtReview.ReadOnly = true;
         this.txtReview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtReview.Size = new System.Drawing.Size(281, 175);
         this.txtReview.TabIndex = 1;
         this.txtReview.Text = "";
         // 
         // lblReview
         // 
         this.lblReview.Dock = System.Windows.Forms.DockStyle.Top;
         this.lblReview.Location = new System.Drawing.Point(0, 0);
         this.lblReview.Name = "lblReview";
         this.lblReview.Size = new System.Drawing.Size(281, 56);
         this.lblReview.TabIndex = 0;
         this.lblReview.Text = "The Sprite Import Wizard has all the information needed to import the requested s" +
            "prites.  The following actions will occur when you click Finish...";
         // 
         // Review
         // 
         this.Review.StepControl = this.pnlReview;
         this.Review.TitleText = "Review";
         this.Review.InitFunction += new System.EventHandler(this.Review_InitFunction);
         this.Review.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.Review_ValidateFunction);
         // 
         // frmSpriteImportWizard
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(450, 313);
         this.Controls.Add(this.pnlReview);
         this.Controls.Add(this.pnlMergeGraphics);
         this.Controls.Add(this.pnlMergeFramesets);
         this.Controls.Add(this.pnlUniqueNames);
         this.Controls.Add(this.pnlSpecifySprite);
         this.Controls.Add(this.pnlSpecifyFile);
         this.Name = "frmSpriteImportWizard";
         this.Steps.Add(this.SpecifyFile);
         this.Steps.Add(this.SpecifySprite);
         this.Steps.Add(this.UniqueNames);
         this.Steps.Add(this.MergeFramesets);
         this.Steps.Add(this.MergeGraphics);
         this.Steps.Add(this.Review);
         this.Text = "Sprite Import Wizard";
         this.Controls.SetChildIndex(this.pnlSpecifyFile, 0);
         this.Controls.SetChildIndex(this.pnlSpecifySprite, 0);
         this.Controls.SetChildIndex(this.pnlUniqueNames, 0);
         this.Controls.SetChildIndex(this.pnlMergeFramesets, 0);
         this.Controls.SetChildIndex(this.pnlMergeGraphics, 0);
         this.Controls.SetChildIndex(this.pnlReview, 0);
         this.pnlSpecifyFile.ResumeLayout(false);
         this.pnlSpecifySprite.ResumeLayout(false);
         this.pnlUniqueNames.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.grdNameMap)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.dvSpriteNames)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.dtSpriteNames)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.dsMapping)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.dtFramesetNames)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.dtGraphicNames)).EndInit();
         this.pnlMergeFramesets.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.grdFramesets)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.dvFramesetNames)).EndInit();
         this.pnlMergeGraphics.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.grdGraphicNames)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.dvGraphicNames)).EndInit();
         this.pnlReview.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion

      #region Private Members
      private ProjectDataset.FramesetRow[] GetImportFramesets()
      {
         System.Collections.ArrayList result = new ArrayList();
         string inClause = String.Empty;
         foreach (string defName in GetImportSpriteList())
         {
            if (inClause.Length == 0)
               inClause = "('";
            else
               inClause += ",'";
            inClause += defName + "'";
         }
         inClause += ")";

         foreach(ProjectDataset.FramesetRow frameset in dsSource.Frameset)
         {
            if (dsSource.SpriteState.Select(dsSource.SpriteState.FramesetNameColumn.ColumnName + " = '" + frameset.Name +
               "' AND " + dsSource.SpriteState.DefinitionNameColumn.ColumnName + " IN " + inClause).Length > 0)
               result.Add(frameset);
         }

         return (ProjectDataset.FramesetRow[])result.ToArray(typeof(ProjectDataset.FramesetRow));
      }

      private ProjectDataset.GraphicSheetRow[] GetImportGfx()
      {
         System.Collections.ArrayList result = new ArrayList();
         string inClause = String.Empty;
         foreach(ProjectDataset.FramesetRow frameset in m_ImportFramesets)
         {
            System.Data.DataRow drMap = dtFramesetNames.Rows.Find(frameset.Name);
            if (drMap != null)
            {
               if (drMap[dcOldFSName].ToString() == drMap[dcNewFSName].ToString())
                  continue;
            }
            if (inClause.Length == 0)
               inClause = "('";
            else
               inClause += ",'";
            inClause += frameset.Name + "'";
         }
         inClause += ")";

         if (inClause.Length > 1)
         {
            foreach(ProjectDataset.GraphicSheetRow gfx in dsSource.GraphicSheet)
            {
               if (dsSource.Frame.Select(dsSource.Frame.GraphicSheetColumn.ColumnName + " = '" + gfx.Name +
                  "' AND " + dsSource.Frame.NameColumn.ColumnName + " IN " + inClause).Length > 0)
                  result.Add(gfx);
            }
         }

         return (ProjectDataset.GraphicSheetRow[])result.ToArray(typeof(ProjectDataset.GraphicSheetRow));
      }

      private string[] GetImportSpriteList()
      {
         if (dsSource.SpriteDefinition.Count != 1)
         {
            string[] result = new string[chlSelectSprites.CheckedItems.Count];
            chlSelectSprites.CheckedItems.CopyTo(result, 0);
            return result;
         }
         return new string[] {dsSource.SpriteDefinition[0].Name};
      }
      #endregion

      #region Event Handlers
      private void btnBrowse_Click(object sender, System.EventArgs e)
      {
         if (txtSourceFile.Text.Length > 0)
            dlgSourceFile.FileName = txtSourceFile.Text;
         if (DialogResult.OK == dlgSourceFile.ShowDialog(this))
         {
            txtSourceFile.Text = dlgSourceFile.FileName;
         }
      }

      private bool SpecifyFile_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         if (!System.IO.File.Exists(txtSourceFile.Text))
         {
            MessageBox.Show(this, "Specified file does not exist.", "Specify Source File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }

         dsSource = new ProjectDataset();
         try
         {
            dsSource.ReadXml(txtSourceFile.Text, System.Data.XmlReadMode.IgnoreSchema);
         }
         catch(System.Exception)
         {
            MessageBox.Show(this, "The specified file does not contain valid SGDK2 data", "Specify Import Source", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }

         dsMapping.Clear();
         chlSelectSprites.Items.Clear();

         return true;
      }

      private void SpecifySprite_InitFunction(object sender, System.EventArgs e)
      {
         if (chlSelectSprites.Items.Count == 0)
         {
            foreach(ProjectDataset.SpriteDefinitionRow sprite in dsSource.SpriteDefinition)
               chlSelectSprites.Items.Add(sprite.Name, true);
         }
      }

      private void btnSelectAllSprites_Click(object sender, System.EventArgs e)
      {
         for (int i=0; i<chlSelectSprites.Items.Count; i++)
            chlSelectSprites.SetItemChecked(i, true);
      }

      private void btnDeselectAllSprites_Click(object sender, System.EventArgs e)
      {
         foreach(int i in chlSelectSprites.CheckedIndices)
            chlSelectSprites.SetItemChecked(i, false);
      }

      private bool SpecifySprite_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         if (chlSelectSprites.CheckedIndices.Count <= 0)
         {
            MessageBox.Show(this, "At least one item must be checked", "Specify Sprites", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         dsMapping.Clear();
         return true;
      }

      private bool SpecifySprite_IsApplicableFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         return (dsSource.SpriteDefinition.Count != 1);
      }

      private void txtSourceFile_TextChanged(object sender, System.EventArgs e)
      {
         if (System.IO.File.Exists(txtSourceFile.Text))
         {
            try
            {
               System.IO.TextReader tr = new System.IO.StreamReader(txtSourceFile.Text);
               try
               {
                  System.Xml.XmlReader xml = new System.Xml.XmlTextReader(tr);
                  try
                  {
                     while(xml.Read())
                     {
                        if (xml.Name == "Project")
                        {
                           txtComment.Text = xml.GetAttribute("TitleText");
                           break;
                        }
                     }
                  }
                  finally
                  {
                     xml.Close();
                  }
               }
               finally
               {
                  tr.Close();
               }
            }
            catch(System.Exception ex)
            {
               txtComment.Text = ex.Message;
            }
         }
         else
         {
            txtComment.Text = String.Empty;
         }
      }

      private bool UniqueNames_IsApplicableFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         foreach(string name in GetImportSpriteList())
         {
            if(ProjectData.GetSpriteDefinition(name) != null)
               return true;
         }
         return false;
      }

      private void UniqueNames_InitFunction(object sender, System.EventArgs e)
      {
         if (dtSpriteNames.Rows.Count > 0)
            return;
         foreach(string name in GetImportSpriteList())
         {
            if(ProjectData.GetSpriteDefinition(name) == null)
               dtSpriteNames.Rows.Add(new object[] {name, name});
            else
               dtSpriteNames.Rows.Add(new object[] {name, null});
         }
      }

      private bool UniqueNames_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         foreach(System.Data.DataRow dr in dtSpriteNames.Rows)
         {
            if(!(dr[dcNewName] is string))
            {
               MessageBox.Show(this, "Please specify a new name for \"" + dr[dcOldName].ToString() + "\".", "Specify Unique Names", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }
            string msg = ProjectData.ValidateName(dr[dcNewName].ToString());
            if (msg != null)
            {
               MessageBox.Show(this, "\"" + dr[dcNewName].ToString() + "\" is invalid.  " + msg, "Specify Unique Names", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }
            if (ProjectData.GetSpriteDefinition(dr[dcNewName].ToString()) != null)
            {
               MessageBox.Show(this, "Sprite Definition \"" + dr[dcNewName].ToString() + "\" already exists in the loaded project.  Please specify a different name.", "Specify Unique Names", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }
         }
         return true;
      }

      private bool MergeFramesets_IsApplicableFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         m_ImportFramesets = GetImportFramesets();
         foreach(ProjectDataset.FramesetRow frameset in m_ImportFramesets)
         {
            if (ProjectData.GetFrameSet(frameset.Name) != null)
               return true;
         }
         return false;
      }

      private void MergeFramesets_InitFunction(object sender, System.EventArgs e)
      {
         if (dtFramesetNames.Rows.Count > 0)
            return;
         foreach(ProjectDataset.FramesetRow newFrameset in m_ImportFramesets)
         {
            ProjectDataset.FramesetRow oldFrameset = ProjectData.GetFrameSet(newFrameset.Name);
            if (oldFrameset != null)
            {
               if (newFrameset.GetFrameRows().Length <= oldFrameset.GetFrameRows().Length)
                  dtFramesetNames.Rows.Add(new object[] {newFrameset.Name, newFrameset.Name});
               else
                  dtFramesetNames.Rows.Add(new object[] {newFrameset.Name, null});
            }
         }
      }

      private bool MergeFramesets_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         foreach(System.Data.DataRow dr in dtFramesetNames.Rows)
         {
            if (!(dr[dcNewFSName] is string))
            {
               MessageBox.Show(this, "Please enter a new name for \"" + dr[dcOldGSName].ToString() + "\".", "Frameset Names", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }
            string msg = ProjectData.ValidateName(dr[dcNewFSName].ToString());
            if (msg != null)
            {
               MessageBox.Show(this, "Invalid new name entered for \"" + dr[dcNewFSName].ToString() + "\".  " + msg, "Frameset Names", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }
            ProjectDataset.FramesetRow oldFrameset = ProjectData.GetFrameSet(dr[dcNewFSName].ToString());
            if (oldFrameset != null)
            {
               ProjectDataset.FramesetRow newFrameset = dsSource.Frameset.FindByName(dr[dcNewFSName].ToString());
               if(newFrameset.GetFrameRows().Length < oldFrameset.GetFrameRows().Length)
               {
                  MessageBox.Show(this, "The new Frameset name specified for \"" + dr[dcOldGSName].ToString() + "\" refers to an existing Frameset that has fewer frames.  Please specify a new name or the name of an existing Frameset with at least as many frames as the imported Frameset (" + newFrameset.GetFrameRows().Length.ToString() + " frames).", "Frameset Names", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return false;
               }
            }
         }

         dtGraphicNames.Rows.Clear();
         return true;
      }

      private bool MergeGraphics_IsApplicableFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         m_ImportGfx = GetImportGfx();
         foreach(ProjectDataset.GraphicSheetRow gfx in m_ImportGfx)
         {
            if(ProjectData.GetGraphicSheet(gfx.Name) != null)
               return true;
         }
         return false;
      }

      private void MergeGraphics_InitFunction(object sender, System.EventArgs e)
      {
         if (dtGraphicNames.Rows.Count > 0)
            return;

         foreach(ProjectDataset.GraphicSheetRow gfx in m_ImportGfx)
         {
            ProjectDataset.GraphicSheetRow existingRow = ProjectData.GetGraphicSheet(gfx.Name);
            if (existingRow != null)
            {
               if (existingRow.Rows * existingRow.Columns >= gfx.Rows * gfx.Columns)
                  dtGraphicNames.Rows.Add(new object[] {gfx.Name, gfx.Name});
               else
                  dtGraphicNames.Rows.Add(new object[] {gfx.Name, null});
            }
         }
      }

      private bool MergeGraphics_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         foreach(System.Data.DataRow dr in dtGraphicNames.Rows)
         {
            if (!(dr[dcNewGSName] is string))
            {
               MessageBox.Show(this, "Please enter a new name for \"" + dr[dcOldGSName].ToString() + "\".", "Graphic Sheet Names", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }
            string msg = ProjectData.ValidateName(dr[dcNewGSName].ToString());
            if (msg != null)
            {
               MessageBox.Show(this, "Invalid new name entered for \"" + dr[dcNewGSName].ToString() + "\".  " + msg, "Graphic Sheet Names", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }
            ProjectDataset.GraphicSheetRow gfxOld = ProjectData.GetGraphicSheet(dr[dcNewGSName].ToString());
            if (gfxOld != null)
            {
               ProjectDataset.GraphicSheetRow gfxNew = dsSource.GraphicSheet.FindByName(dr[dcNewGSName].ToString());
               if(gfxNew.Rows * gfxNew.Columns < gfxOld.Rows * gfxOld.Columns)
               {
                  MessageBox.Show(this, "The new Graphic Sheet name specified for \"" + dr[dcOldGSName].ToString() + "\" refers to an existing graphic sheet that has fewer cells.  Please specify a new name or the name of an existing graphic sheet with at least as many cells as the imported graphic sheet (" + (gfxNew.Columns * gfxNew.Rows).ToString() + " cells).", "Graphic Sheet Names", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return false;
               }
            }
         }
         return true;
      }

      private void Review_InitFunction(object sender, System.EventArgs e)
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();
         sb.Append("Use source data from " + txtSourceFile.Text + ".\r\n");
         foreach(string sprite in GetImportSpriteList())
         {
            System.Data.DataRow dr = dtSpriteNames.Rows.Find(sprite);
            if (dr != null)
               sb.Append("Import Sprite Definition \"" + sprite + "\" as \"" + dr[dcNewName].ToString() + "\".\r\n");
            else
               sb.Append("Import Sprite Definition \"" + sprite + "\"\r\n");
         }
         foreach(ProjectDataset.FramesetRow frameset in m_ImportFramesets)
         {
            System.Data.DataRow dr = dtFramesetNames.Rows.Find(frameset.Name);
            if (dr != null)
               if (ProjectData.GetFrameSet(dr[dcNewFSName].ToString()) != null)
                  sb.Append("Re-map all imported frames that refer to Frameset \"" + frameset.Name + "\" to use the existing \"" + dr[dcNewFSName] + "\" instead.\r\n");
               else
                  sb.Append("Import Frameset \"" + frameset.Name + "\" as \"" + dr[dcNewFSName].ToString() + "\".\r\n");
            else
               sb.Append("Import Frameset \"" + frameset.Name + "\".\r\n");
         }
         foreach(ProjectDataset.GraphicSheetRow gfx in m_ImportGfx)
         {
            System.Data.DataRow dr = dtGraphicNames.Rows.Find(gfx.Name);
            if (dr != null)
               if (ProjectData.GetGraphicSheet(dr[dcNewGSName].ToString()) != null)
                  sb.Append("Re-map all imported frames that refer to Graphic Sheet \"" + gfx.Name + "\" to use the existing \"" + dr[dcNewGSName] + "\" instead.\r\n");
               else
                  sb.Append("Import Graphic Sheet \"" + gfx.Name + "\" as \"" + dr[dcNewGSName].ToString() + "\".\r\n");
            else
               sb.Append("Import Graphic Sheet \"" + gfx.Name + "\".\r\n");
         }
         sb.Append(ProjectData.GetCreditAdditions(dsSource));
         txtReview.Text = sb.ToString();
      }

      private bool Review_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         try
         {
            ProjectData.EnforceConstraints = false;
            foreach(ProjectDataset.GraphicSheetRow gfx in m_ImportGfx)
            {
               ProjectDataset.GraphicSheetRow src = dsSource.GraphicSheet.FindByName(gfx.Name);
               System.Data.DataRow dr = dtGraphicNames.Rows.Find(gfx.Name);
               if (dr != null)
               {
                  dsSource.EnforceConstraints = false;
                  src.Name = dr[dcNewGSName].ToString();
                  dsSource.AcceptChanges();
                  dsSource.EnforceConstraints = true;

                  if (ProjectData.GetGraphicSheet(dr[dcNewGSName].ToString()) == null)
                     ProjectData.GraphicSheet.Rows.Add(src.ItemArray);
               }
               else
                  ProjectData.GraphicSheet.Rows.Add(src.ItemArray);
            }
            foreach(ProjectDataset.FramesetRow frameset in m_ImportFramesets)
            {
               ProjectDataset.FramesetRow src = dsSource.Frameset.FindByName(frameset.Name);
               System.Data.DataRow dr = dtFramesetNames.Rows.Find(frameset.Name);
               bool bImport = false;
               
               if (dr != null)
               {
                  dsSource.EnforceConstraints = false;
                  src.Name = dr[dcNewFSName].ToString();
                  dsSource.AcceptChanges();
                  dsSource.EnforceConstraints = true;

                  if (ProjectData.GetFrameSet(dr[dcNewFSName].ToString()) == null)
                     bImport = true;
               }
               else
                  bImport = true;

               if (bImport)
               {
                  ProjectData.Frameset.Rows.Add(src.ItemArray);
                  foreach(ProjectDataset.FrameRow frame in src.GetFrameRows())
                     ProjectData.Frame.Rows.Add(frame.ItemArray);
               }
            }
            foreach(string sprite in GetImportSpriteList())
            {
               ProjectDataset.SpriteDefinitionRow src = dsSource.SpriteDefinition.FindByName(sprite);
               System.Data.DataRow dr = dtSpriteNames.Rows.Find(sprite);

               if (dr != null)
               {
                  dsSource.EnforceConstraints = false;
                  src.Name = dr[dcNewName].ToString();
                  dsSource.AcceptChanges();
                  dsSource.EnforceConstraints = true;
               }
               ProjectData.SpriteDefinition.Rows.Add(src.ItemArray);
               foreach(ProjectDataset.SpriteStateRow state in src.GetSpriteStateRows())
               {
                  ProjectData.SpriteState.Rows.Add(state.ItemArray);
                  foreach(ProjectDataset.SpriteFrameRow frame in state.GetSpriteFrameRows())
                     ProjectData.SpriteFrame.Rows.Add(frame.ItemArray);
               }
               foreach(ProjectDataset.SpriteParameterRow param in src.GetSpriteParameterRows())
                  ProjectData.SpriteParameter.Rows.Add(param.ItemArray);
               foreach(ProjectDataset.SpriteRuleRow rule in src.GetSpriteRuleRows())
                  ProjectData.SpriteRule.Rows.Add(rule.ItemArray);
            }

            ProjectData.EnforceConstraints = true;

            ProjectData.MergeCredits(dsSource);
            return true;
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Import Sprite Definition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
      }
      #endregion
   }
}

