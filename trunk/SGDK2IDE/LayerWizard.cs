using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

namespace SGDK2
{
   public class frmLayerWizard : SGDK2.frmWizardBase
   {
      #region Non-control members
      private ProjectDataset.LayerRow m_Layer;
      private int m_nTileCount = 0;
      private double m_nScrollRateX;
      private double m_nScrollRateY;
      private Layer[] m_Layers = null;
      private int m_nCurLayer;
      private Point m_ProposedOffset;
      private Point m_RememberedOffset = Point.Empty;
      private Size m_ViewSize = Size.Empty;
      private Size m_PropsedSize = Size.Empty;
      private Size m_PropsedVirtualSize = Size.Empty;
      private frmLayerManager.LayerProperties m_LayerProperties;
      #endregion

      #region Form Designer Members
      private System.Windows.Forms.Panel pnlTileset;
      private System.Windows.Forms.Label lblTilesetInfo;
      private System.Windows.Forms.ComboBox cboTileset;
      private System.Windows.Forms.Label lblTileset;
      private SGDK2.frmWizardBase.StepInfo Tileset;
      private System.Windows.Forms.Panel pnlBytesPerTile;
      private SGDK2.frmWizardBase.StepInfo BytesPerTile;
      private System.Windows.Forms.Label lblBytesPerTileInfo;
      private System.Windows.Forms.Label lblBytesPerTile;
      private System.Windows.Forms.ComboBox cboBytesPerTile;
      private System.Windows.Forms.Panel pnlBackgroundTile;
      private SGDK2.frmWizardBase.StepInfo BackgroundTile;
      private System.Windows.Forms.Label lblBackgroundTileInfo;
      private System.Windows.Forms.CheckBox chkChangeBackground;
      private SGDK2.GraphicBrowser BackgroundTileSelector;
      private System.Windows.Forms.Panel pnlOffset;
      private SGDK2.frmWizardBase.StepInfo Offset;
      private SGDK2.Display offsetDisplay;
      private System.Windows.Forms.Label lblOffsetInfo;
      private System.Windows.Forms.Label lblOffset;
      private System.Windows.Forms.Panel pnlScrollRate;
      private SGDK2.frmWizardBase.StepInfo ScrollRate;
      private System.Windows.Forms.Label lblScrollRateInfo;
      private System.Windows.Forms.Label lblScrollRateX;
      private System.Windows.Forms.TextBox txtScrollRateX;
      private System.Windows.Forms.TextBox txtScrollRateY;
      private System.Windows.Forms.Label lblScrollRateY;
      private System.Windows.Forms.Panel pnlPriority;
      private SGDK2.frmWizardBase.StepInfo Priority;
      private System.Windows.Forms.Label lblPriorityInfo;
      private System.Windows.Forms.ListBox lstLayers;
      private System.Windows.Forms.Label lblZIndex;
      private System.Windows.Forms.Label lblPriority;
      private System.Windows.Forms.NumericUpDown nudZIndex;
      private System.Windows.Forms.NumericUpDown nudPriority;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.Panel pnlSize;
      private SGDK2.frmWizardBase.StepInfo LayerSize;
      private System.Windows.Forms.Label lblSizeInfo;
      private System.Windows.Forms.Label lblTileRows;
      private System.Windows.Forms.TextBox txtTileRows;
      private System.Windows.Forms.Label lblTileCols;
      private System.Windows.Forms.Label lblSizeWarning;
      private SGDK2.GraphicBrowser PreviewTiles;
      private System.Windows.Forms.Label lblPreviewTileset;
      private System.Windows.Forms.TextBox txtTileCols;
      private System.Windows.Forms.CheckBox chkFitToMap;
      private System.Windows.Forms.Panel pnlReview;
      private SGDK2.frmWizardBase.StepInfo Review;
      private System.Windows.Forms.Label lblReview;
      private System.Windows.Forms.TextBox txtReview;
      private SGDK2.frmWizardBase.StepInfo VirtualSize;
      private System.Windows.Forms.Panel pnlVirtualSize;
      private System.Windows.Forms.TextBox txtVirtualColumns;
      private System.Windows.Forms.Label lblVirtualColumns;
      private System.Windows.Forms.TextBox txtVirtualRows;
      private System.Windows.Forms.Label lblVirtualRows;
      private System.Windows.Forms.CheckBox chkFitVirtualToMap;
      private System.Windows.Forms.Label lblVirtualSizePrompt;
      private System.ComponentModel.IContainer components = null;
      #endregion

      public frmLayerWizard(frmLayerManager.LayerProperties layerProps)
      {
         // This call is required by the Windows Form Designer.
         InitializeComponent();

         m_Layer = layerProps.m_drLayer;
         m_LayerProperties = layerProps;
         if ((m_Layer.MapRow.ViewWidth <= 0) || (m_Layer.MapRow.ViewHeight <= 0))
            m_ViewSize = Display.GetScreenSize((GameDisplayMode)Enum.Parse(typeof(GameDisplayMode), ProjectData.ProjectRow.DisplayMode));
         else
            m_ViewSize = new Size(m_Layer.MapRow.ViewWidth, m_Layer.MapRow.ViewHeight);
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
         this.pnlTileset = new System.Windows.Forms.Panel();
         this.lblPreviewTileset = new System.Windows.Forms.Label();
         this.PreviewTiles = new SGDK2.GraphicBrowser();
         this.lblTileset = new System.Windows.Forms.Label();
         this.cboTileset = new System.Windows.Forms.ComboBox();
         this.lblTilesetInfo = new System.Windows.Forms.Label();
         this.Tileset = new SGDK2.frmWizardBase.StepInfo();
         this.pnlBytesPerTile = new System.Windows.Forms.Panel();
         this.cboBytesPerTile = new System.Windows.Forms.ComboBox();
         this.lblBytesPerTile = new System.Windows.Forms.Label();
         this.lblBytesPerTileInfo = new System.Windows.Forms.Label();
         this.BytesPerTile = new SGDK2.frmWizardBase.StepInfo();
         this.pnlBackgroundTile = new System.Windows.Forms.Panel();
         this.BackgroundTileSelector = new SGDK2.GraphicBrowser();
         this.chkChangeBackground = new System.Windows.Forms.CheckBox();
         this.lblBackgroundTileInfo = new System.Windows.Forms.Label();
         this.BackgroundTile = new SGDK2.frmWizardBase.StepInfo();
         this.pnlOffset = new System.Windows.Forms.Panel();
         this.lblOffsetInfo = new System.Windows.Forms.Label();
         this.offsetDisplay = new SGDK2.Display();
         this.lblOffset = new System.Windows.Forms.Label();
         this.Offset = new SGDK2.frmWizardBase.StepInfo();
         this.pnlScrollRate = new System.Windows.Forms.Panel();
         this.txtScrollRateY = new System.Windows.Forms.TextBox();
         this.lblScrollRateY = new System.Windows.Forms.Label();
         this.txtScrollRateX = new System.Windows.Forms.TextBox();
         this.lblScrollRateX = new System.Windows.Forms.Label();
         this.lblScrollRateInfo = new System.Windows.Forms.Label();
         this.ScrollRate = new SGDK2.frmWizardBase.StepInfo();
         this.pnlPriority = new System.Windows.Forms.Panel();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.lstLayers = new System.Windows.Forms.ListBox();
         this.nudPriority = new System.Windows.Forms.NumericUpDown();
         this.nudZIndex = new System.Windows.Forms.NumericUpDown();
         this.lblPriority = new System.Windows.Forms.Label();
         this.lblZIndex = new System.Windows.Forms.Label();
         this.lblPriorityInfo = new System.Windows.Forms.Label();
         this.Priority = new SGDK2.frmWizardBase.StepInfo();
         this.pnlSize = new System.Windows.Forms.Panel();
         this.lblSizeWarning = new System.Windows.Forms.Label();
         this.txtTileCols = new System.Windows.Forms.TextBox();
         this.lblTileCols = new System.Windows.Forms.Label();
         this.txtTileRows = new System.Windows.Forms.TextBox();
         this.lblTileRows = new System.Windows.Forms.Label();
         this.chkFitToMap = new System.Windows.Forms.CheckBox();
         this.lblSizeInfo = new System.Windows.Forms.Label();
         this.LayerSize = new SGDK2.frmWizardBase.StepInfo();
         this.pnlReview = new System.Windows.Forms.Panel();
         this.txtReview = new System.Windows.Forms.TextBox();
         this.lblReview = new System.Windows.Forms.Label();
         this.Review = new SGDK2.frmWizardBase.StepInfo();
         this.VirtualSize = new SGDK2.frmWizardBase.StepInfo();
         this.pnlVirtualSize = new System.Windows.Forms.Panel();
         this.txtVirtualColumns = new System.Windows.Forms.TextBox();
         this.lblVirtualColumns = new System.Windows.Forms.Label();
         this.txtVirtualRows = new System.Windows.Forms.TextBox();
         this.lblVirtualRows = new System.Windows.Forms.Label();
         this.chkFitVirtualToMap = new System.Windows.Forms.CheckBox();
         this.lblVirtualSizePrompt = new System.Windows.Forms.Label();
         this.pnlTileset.SuspendLayout();
         this.pnlBytesPerTile.SuspendLayout();
         this.pnlBackgroundTile.SuspendLayout();
         this.pnlOffset.SuspendLayout();
         this.pnlScrollRate.SuspendLayout();
         this.pnlPriority.SuspendLayout();
         this.groupBox1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.nudPriority)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudZIndex)).BeginInit();
         this.pnlSize.SuspendLayout();
         this.pnlReview.SuspendLayout();
         this.pnlVirtualSize.SuspendLayout();
         this.SuspendLayout();
         // 
         // pnlTileset
         // 
         this.pnlTileset.Controls.Add(this.lblPreviewTileset);
         this.pnlTileset.Controls.Add(this.PreviewTiles);
         this.pnlTileset.Controls.Add(this.lblTileset);
         this.pnlTileset.Controls.Add(this.cboTileset);
         this.pnlTileset.Controls.Add(this.lblTilesetInfo);
         this.pnlTileset.Location = new System.Drawing.Point(168, 42);
         this.pnlTileset.Name = "pnlTileset";
         this.pnlTileset.Size = new System.Drawing.Size(288, 231);
         this.pnlTileset.TabIndex = 6;
         this.pnlTileset.Visible = false;
         // 
         // lblPreviewTileset
         // 
         this.lblPreviewTileset.Location = new System.Drawing.Point(8, 120);
         this.lblPreviewTileset.Name = "lblPreviewTileset";
         this.lblPreviewTileset.Size = new System.Drawing.Size(88, 16);
         this.lblPreviewTileset.TabIndex = 4;
         this.lblPreviewTileset.Text = "Preview Tileset:";
         // 
         // PreviewTiles
         // 
         this.PreviewTiles.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.PreviewTiles.CellPadding = new System.Drawing.Size(1, 1);
         this.PreviewTiles.CellSize = new System.Drawing.Size(0, 0);
         this.PreviewTiles.CurrentCellIndex = -1;
         this.PreviewTiles.Frameset = null;
         this.PreviewTiles.FramesToDisplay = null;
         this.PreviewTiles.GraphicSheet = null;
         this.PreviewTiles.Location = new System.Drawing.Point(8, 136);
         this.PreviewTiles.Name = "PreviewTiles";
         this.PreviewTiles.SheetImage = null;
         this.PreviewTiles.Size = new System.Drawing.Size(264, 88);
         this.PreviewTiles.TabIndex = 3;
         // 
         // lblTileset
         // 
         this.lblTileset.Location = new System.Drawing.Point(8, 88);
         this.lblTileset.Name = "lblTileset";
         this.lblTileset.Size = new System.Drawing.Size(64, 21);
         this.lblTileset.TabIndex = 2;
         this.lblTileset.Text = "Tileset:";
         this.lblTileset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // cboTileset
         // 
         this.cboTileset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboTileset.Location = new System.Drawing.Point(72, 88);
         this.cboTileset.Name = "cboTileset";
         this.cboTileset.Size = new System.Drawing.Size(184, 21);
         this.cboTileset.TabIndex = 1;
         this.cboTileset.SelectedIndexChanged += new System.EventHandler(this.cboTileset_SelectedIndexChanged);
         // 
         // lblTilesetInfo
         // 
         this.lblTilesetInfo.Location = new System.Drawing.Point(8, 8);
         this.lblTilesetInfo.Name = "lblTilesetInfo";
         this.lblTilesetInfo.Size = new System.Drawing.Size(264, 80);
         this.lblTilesetInfo.TabIndex = 0;
         this.lblTilesetInfo.Text = "The tileset determines how graphics are stored on the layer.  It maps numeric val" +
            "ues from the layer\'s data to graphic frames defined in a frameset.  The tileset " +
            "also determines how tall and wide the tiles in the layer are, and how they anima" +
            "te.";
         // 
         // Tileset
         // 
         this.Tileset.StepControl = this.pnlTileset;
         this.Tileset.TitleText = "Select Tileset";
         this.Tileset.InitFunction += new System.EventHandler(this.Tileset_InitFunction);
         this.Tileset.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.Tileset_ValidateFunction);
         // 
         // pnlBytesPerTile
         // 
         this.pnlBytesPerTile.Controls.Add(this.cboBytesPerTile);
         this.pnlBytesPerTile.Controls.Add(this.lblBytesPerTile);
         this.pnlBytesPerTile.Controls.Add(this.lblBytesPerTileInfo);
         this.pnlBytesPerTile.Location = new System.Drawing.Point(-10168, 42);
         this.pnlBytesPerTile.Name = "pnlBytesPerTile";
         this.pnlBytesPerTile.Size = new System.Drawing.Size(292, 231);
         this.pnlBytesPerTile.TabIndex = 7;
         this.pnlBytesPerTile.Visible = false;
         // 
         // cboBytesPerTile
         // 
         this.cboBytesPerTile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboBytesPerTile.Items.AddRange(new object[] {
                                                             "1 (0-255)",
                                                             "2 (0-32676)",
                                                             "4 (0-2147483647)"});
         this.cboBytesPerTile.Location = new System.Drawing.Point(112, 144);
         this.cboBytesPerTile.Name = "cboBytesPerTile";
         this.cboBytesPerTile.Size = new System.Drawing.Size(160, 21);
         this.cboBytesPerTile.TabIndex = 2;
         // 
         // lblBytesPerTile
         // 
         this.lblBytesPerTile.Location = new System.Drawing.Point(8, 144);
         this.lblBytesPerTile.Name = "lblBytesPerTile";
         this.lblBytesPerTile.Size = new System.Drawing.Size(104, 21);
         this.lblBytesPerTile.TabIndex = 1;
         this.lblBytesPerTile.Text = "Bytes Per Tile:";
         this.lblBytesPerTile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblBytesPerTileInfo
         // 
         this.lblBytesPerTileInfo.Location = new System.Drawing.Point(8, 8);
         this.lblBytesPerTileInfo.Name = "lblBytesPerTileInfo";
         this.lblBytesPerTileInfo.Size = new System.Drawing.Size(264, 136);
         this.lblBytesPerTileInfo.TabIndex = 0;
         this.lblBytesPerTileInfo.Text = @"The Bytes Per Tile setting determines how many types of tiles can be represented in a layer.  This is generally determined by the tileset.  The smallest value of 1 allows up to 256 different tiles to be used on a layer.  Tilesets with more than 256 tiles require more bytes in order to allow unique values for each tile that can be represented in the layer.  The value shown below is suggested based on the tileset chosen in step 1.";
         // 
         // BytesPerTile
         // 
         this.BytesPerTile.StepControl = this.pnlBytesPerTile;
         this.BytesPerTile.TitleText = "Bytes Per Tile";
         this.BytesPerTile.InitFunction += new System.EventHandler(this.BytesPerTile_InitFunction);
         // 
         // pnlBackgroundTile
         // 
         this.pnlBackgroundTile.Controls.Add(this.BackgroundTileSelector);
         this.pnlBackgroundTile.Controls.Add(this.chkChangeBackground);
         this.pnlBackgroundTile.Controls.Add(this.lblBackgroundTileInfo);
         this.pnlBackgroundTile.Location = new System.Drawing.Point(-10168, 42);
         this.pnlBackgroundTile.Name = "pnlBackgroundTile";
         this.pnlBackgroundTile.Size = new System.Drawing.Size(292, 231);
         this.pnlBackgroundTile.TabIndex = 8;
         this.pnlBackgroundTile.Visible = false;
         // 
         // BackgroundTileSelector
         // 
         this.BackgroundTileSelector.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.BackgroundTileSelector.CellPadding = new System.Drawing.Size(1, 1);
         this.BackgroundTileSelector.CellSize = new System.Drawing.Size(0, 0);
         this.BackgroundTileSelector.CurrentCellIndex = -1;
         this.BackgroundTileSelector.Enabled = false;
         this.BackgroundTileSelector.Frameset = null;
         this.BackgroundTileSelector.FramesToDisplay = null;
         this.BackgroundTileSelector.GraphicSheet = null;
         this.BackgroundTileSelector.Location = new System.Drawing.Point(8, 112);
         this.BackgroundTileSelector.Name = "BackgroundTileSelector";
         this.BackgroundTileSelector.SheetImage = null;
         this.BackgroundTileSelector.Size = new System.Drawing.Size(264, 112);
         this.BackgroundTileSelector.TabIndex = 2;
         // 
         // chkChangeBackground
         // 
         this.chkChangeBackground.Location = new System.Drawing.Point(8, 88);
         this.chkChangeBackground.Name = "chkChangeBackground";
         this.chkChangeBackground.Size = new System.Drawing.Size(264, 16);
         this.chkChangeBackground.TabIndex = 1;
         this.chkChangeBackground.Text = "Delete all tiles on layer use a new background.";
         this.chkChangeBackground.CheckedChanged += new System.EventHandler(this.BackgroundTile_InitFunction);
         // 
         // lblBackgroundTileInfo
         // 
         this.lblBackgroundTileInfo.Location = new System.Drawing.Point(8, 8);
         this.lblBackgroundTileInfo.Name = "lblBackgroundTileInfo";
         this.lblBackgroundTileInfo.Size = new System.Drawing.Size(264, 72);
         this.lblBackgroundTileInfo.TabIndex = 0;
         this.lblBackgroundTileInfo.Text = "Background layers should use an opaque tile to set the background for the rest of" +
            " the layers.  Other layers should use tiles with at least some transparency in o" +
            "rder to see through to the background layer.";
         // 
         // BackgroundTile
         // 
         this.BackgroundTile.StepControl = this.pnlBackgroundTile;
         this.BackgroundTile.TitleText = "Background Tile";
         this.BackgroundTile.InitFunction += new System.EventHandler(this.BackgroundTile_InitFunction);
         this.BackgroundTile.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.BackgroundTile_ValidateFunction);
         // 
         // pnlOffset
         // 
         this.pnlOffset.Controls.Add(this.lblOffsetInfo);
         this.pnlOffset.Controls.Add(this.offsetDisplay);
         this.pnlOffset.Controls.Add(this.lblOffset);
         this.pnlOffset.Location = new System.Drawing.Point(-10168, 42);
         this.pnlOffset.Name = "pnlOffset";
         this.pnlOffset.Size = new System.Drawing.Size(286, 231);
         this.pnlOffset.TabIndex = 9;
         this.pnlOffset.Visible = false;
         // 
         // lblOffsetInfo
         // 
         this.lblOffsetInfo.Location = new System.Drawing.Point(8, 8);
         this.lblOffsetInfo.Name = "lblOffsetInfo";
         this.lblOffsetInfo.Size = new System.Drawing.Size(264, 16);
         this.lblOffsetInfo.TabIndex = 1;
         this.lblOffsetInfo.Text = "The layer can be positioned within the map.";
         // 
         // offsetDisplay
         // 
         this.offsetDisplay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.offsetDisplay.GameDisplayMode = SGDK2.GameDisplayMode.m320x240x16;
         this.offsetDisplay.Location = new System.Drawing.Point(0, 32);
         this.offsetDisplay.Name = "offsetDisplay";
         this.offsetDisplay.Size = new System.Drawing.Size(280, 176);
         this.offsetDisplay.TabIndex = 0;
         this.offsetDisplay.Windowed = true;
         this.offsetDisplay.Paint += new System.Windows.Forms.PaintEventHandler(this.offsetDisplay_Paint);
         this.offsetDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.offsetDisplay_MouseMove);
         this.offsetDisplay.MouseLeave += new System.EventHandler(this.offsetDisplay_MouseLeave);
         this.offsetDisplay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.offsetDisplay_MouseDown);
         // 
         // lblOffset
         // 
         this.lblOffset.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.lblOffset.Location = new System.Drawing.Point(0, 207);
         this.lblOffset.Name = "lblOffset";
         this.lblOffset.Size = new System.Drawing.Size(286, 24);
         this.lblOffset.TabIndex = 2;
         this.lblOffset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // Offset
         // 
         this.Offset.StepControl = this.pnlOffset;
         this.Offset.TitleText = "Offset";
         this.Offset.InitFunction += new System.EventHandler(this.Offset_InitFunction);
         // 
         // pnlScrollRate
         // 
         this.pnlScrollRate.Controls.Add(this.txtScrollRateY);
         this.pnlScrollRate.Controls.Add(this.lblScrollRateY);
         this.pnlScrollRate.Controls.Add(this.txtScrollRateX);
         this.pnlScrollRate.Controls.Add(this.lblScrollRateX);
         this.pnlScrollRate.Controls.Add(this.lblScrollRateInfo);
         this.pnlScrollRate.Location = new System.Drawing.Point(-10168, 42);
         this.pnlScrollRate.Name = "pnlScrollRate";
         this.pnlScrollRate.Size = new System.Drawing.Size(292, 231);
         this.pnlScrollRate.TabIndex = 10;
         this.pnlScrollRate.Visible = false;
         // 
         // txtScrollRateY
         // 
         this.txtScrollRateY.Location = new System.Drawing.Point(144, 136);
         this.txtScrollRateY.Name = "txtScrollRateY";
         this.txtScrollRateY.Size = new System.Drawing.Size(56, 20);
         this.txtScrollRateY.TabIndex = 4;
         this.txtScrollRateY.Text = "";
         // 
         // lblScrollRateY
         // 
         this.lblScrollRateY.Location = new System.Drawing.Point(8, 136);
         this.lblScrollRateY.Name = "lblScrollRateY";
         this.lblScrollRateY.Size = new System.Drawing.Size(128, 20);
         this.lblScrollRateY.TabIndex = 3;
         this.lblScrollRateY.Text = "Vertical Scroll Rate:";
         this.lblScrollRateY.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtScrollRateX
         // 
         this.txtScrollRateX.Location = new System.Drawing.Point(144, 112);
         this.txtScrollRateX.Name = "txtScrollRateX";
         this.txtScrollRateX.Size = new System.Drawing.Size(56, 20);
         this.txtScrollRateX.TabIndex = 2;
         this.txtScrollRateX.Text = "";
         // 
         // lblScrollRateX
         // 
         this.lblScrollRateX.Location = new System.Drawing.Point(8, 112);
         this.lblScrollRateX.Name = "lblScrollRateX";
         this.lblScrollRateX.Size = new System.Drawing.Size(128, 20);
         this.lblScrollRateX.TabIndex = 1;
         this.lblScrollRateX.Text = "Horizontal Scroll Rate:";
         this.lblScrollRateX.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblScrollRateInfo
         // 
         this.lblScrollRateInfo.Location = new System.Drawing.Point(8, 8);
         this.lblScrollRateInfo.Name = "lblScrollRateInfo";
         this.lblScrollRateInfo.Size = new System.Drawing.Size(264, 96);
         this.lblScrollRateInfo.TabIndex = 0;
         this.lblScrollRateInfo.Text = @"Specifying a scroll rate of 1 will make the layer scroll 1 pixel for each unit that the map scrolls.  Any other number will cause the layer to scroll at the specified multiple of the map scroll rate.  A scroll rate of 0 indicates the layer will not scroll automatically with the rest of the map.";
         // 
         // ScrollRate
         // 
         this.ScrollRate.StepControl = this.pnlScrollRate;
         this.ScrollRate.TitleText = "Scroll Rate";
         this.ScrollRate.InitFunction += new System.EventHandler(this.ScrollRate_InitFunction);
         this.ScrollRate.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.ScrollRate_ValidateFunction);
         // 
         // pnlPriority
         // 
         this.pnlPriority.Controls.Add(this.groupBox1);
         this.pnlPriority.Controls.Add(this.nudPriority);
         this.pnlPriority.Controls.Add(this.nudZIndex);
         this.pnlPriority.Controls.Add(this.lblPriority);
         this.pnlPriority.Controls.Add(this.lblZIndex);
         this.pnlPriority.Controls.Add(this.lblPriorityInfo);
         this.pnlPriority.Location = new System.Drawing.Point(-10168, 42);
         this.pnlPriority.Name = "pnlPriority";
         this.pnlPriority.Size = new System.Drawing.Size(290, 231);
         this.pnlPriority.TabIndex = 11;
         this.pnlPriority.Visible = false;
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.lstLayers);
         this.groupBox1.Location = new System.Drawing.Point(112, 128);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(168, 96);
         this.groupBox1.TabIndex = 9;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Layer order (top=foreground)";
         // 
         // lstLayers
         // 
         this.lstLayers.Location = new System.Drawing.Point(8, 16);
         this.lstLayers.Name = "lstLayers";
         this.lstLayers.SelectionMode = System.Windows.Forms.SelectionMode.None;
         this.lstLayers.Size = new System.Drawing.Size(152, 69);
         this.lstLayers.TabIndex = 1;
         // 
         // nudPriority
         // 
         this.nudPriority.Location = new System.Drawing.Point(56, 168);
         this.nudPriority.Name = "nudPriority";
         this.nudPriority.Size = new System.Drawing.Size(48, 20);
         this.nudPriority.TabIndex = 8;
         // 
         // nudZIndex
         // 
         this.nudZIndex.Location = new System.Drawing.Point(56, 144);
         this.nudZIndex.Name = "nudZIndex";
         this.nudZIndex.Size = new System.Drawing.Size(48, 20);
         this.nudZIndex.TabIndex = 7;
         this.nudZIndex.ValueChanged += new System.EventHandler(this.nudZIndex_ValueChanged);
         // 
         // lblPriority
         // 
         this.lblPriority.Location = new System.Drawing.Point(8, 168);
         this.lblPriority.Name = "lblPriority";
         this.lblPriority.Size = new System.Drawing.Size(48, 20);
         this.lblPriority.TabIndex = 5;
         this.lblPriority.Text = "Priority:";
         this.lblPriority.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblZIndex
         // 
         this.lblZIndex.Location = new System.Drawing.Point(8, 144);
         this.lblZIndex.Name = "lblZIndex";
         this.lblZIndex.Size = new System.Drawing.Size(48, 20);
         this.lblZIndex.TabIndex = 3;
         this.lblZIndex.Text = "ZIndex:";
         this.lblZIndex.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblPriorityInfo
         // 
         this.lblPriorityInfo.Location = new System.Drawing.Point(8, 8);
         this.lblPriorityInfo.Name = "lblPriorityInfo";
         this.lblPriorityInfo.Size = new System.Drawing.Size(264, 120);
         this.lblPriorityInfo.TabIndex = 0;
         this.lblPriorityInfo.Text = @"The ZIndex property of a layer determines how it is drawn relative to other layers while the Priority property determines how the layer's tiles are drawn relative to the sprites within the layer.  Lower values cause the layer to draw farther in the background.  If the priority of the layer is equal to the priority of a sprite, then the sprite is drawn after all the tiles above it and before the tiles below it.";
         // 
         // Priority
         // 
         this.Priority.StepControl = this.pnlPriority;
         this.Priority.TitleText = "Priority";
         this.Priority.InitFunction += new System.EventHandler(this.Priority_InitFunction);
         // 
         // pnlSize
         // 
         this.pnlSize.Controls.Add(this.lblSizeWarning);
         this.pnlSize.Controls.Add(this.txtTileCols);
         this.pnlSize.Controls.Add(this.lblTileCols);
         this.pnlSize.Controls.Add(this.txtTileRows);
         this.pnlSize.Controls.Add(this.lblTileRows);
         this.pnlSize.Controls.Add(this.chkFitToMap);
         this.pnlSize.Controls.Add(this.lblSizeInfo);
         this.pnlSize.Location = new System.Drawing.Point(-10168, 42);
         this.pnlSize.Name = "pnlSize";
         this.pnlSize.Size = new System.Drawing.Size(289, 231);
         this.pnlSize.TabIndex = 12;
         this.pnlSize.Visible = false;
         // 
         // lblSizeWarning
         // 
         this.lblSizeWarning.Location = new System.Drawing.Point(8, 200);
         this.lblSizeWarning.Name = "lblSizeWarning";
         this.lblSizeWarning.Size = new System.Drawing.Size(264, 32);
         this.lblSizeWarning.TabIndex = 6;
         this.lblSizeWarning.Text = "WARNING: Reducing a layer\'s size causes tiles to be deleted from the layer.";
         // 
         // txtTileCols
         // 
         this.txtTileCols.Location = new System.Drawing.Point(88, 144);
         this.txtTileCols.MaxLength = 7;
         this.txtTileCols.Name = "txtTileCols";
         this.txtTileCols.Size = new System.Drawing.Size(72, 20);
         this.txtTileCols.TabIndex = 5;
         this.txtTileCols.Text = "";
         // 
         // lblTileCols
         // 
         this.lblTileCols.Location = new System.Drawing.Point(8, 144);
         this.lblTileCols.Name = "lblTileCols";
         this.lblTileCols.Size = new System.Drawing.Size(80, 20);
         this.lblTileCols.TabIndex = 4;
         this.lblTileCols.Text = "Tile Columns:";
         this.lblTileCols.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtTileRows
         // 
         this.txtTileRows.Location = new System.Drawing.Point(88, 168);
         this.txtTileRows.MaxLength = 7;
         this.txtTileRows.Name = "txtTileRows";
         this.txtTileRows.Size = new System.Drawing.Size(72, 20);
         this.txtTileRows.TabIndex = 3;
         this.txtTileRows.Text = "";
         // 
         // lblTileRows
         // 
         this.lblTileRows.Location = new System.Drawing.Point(8, 168);
         this.lblTileRows.Name = "lblTileRows";
         this.lblTileRows.Size = new System.Drawing.Size(80, 20);
         this.lblTileRows.TabIndex = 2;
         this.lblTileRows.Text = "Tile Rows:";
         this.lblTileRows.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // chkFitToMap
         // 
         this.chkFitToMap.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.chkFitToMap.Location = new System.Drawing.Point(8, 104);
         this.chkFitToMap.Name = "chkFitToMap";
         this.chkFitToMap.Size = new System.Drawing.Size(264, 32);
         this.chkFitToMap.TabIndex = 1;
         this.chkFitToMap.Text = "Size the layer to fill the map\'s scrollable area. (Assumes single-view mode or sc" +
            "roll rate=1.)";
         this.chkFitToMap.CheckedChanged += new System.EventHandler(this.chkFitToMap_CheckedChanged);
         // 
         // lblSizeInfo
         // 
         this.lblSizeInfo.Location = new System.Drawing.Point(8, 8);
         this.lblSizeInfo.Name = "lblSizeInfo";
         this.lblSizeInfo.Size = new System.Drawing.Size(264, 96);
         this.lblSizeInfo.TabIndex = 0;
         this.lblSizeInfo.Text = @"The size of the layer relates to a number of other factors: Higher scroll rates need larger layers to provide more tiles since the layer scrolls through more area; An offset means that the layer doesn't need tiles for the area left or above the offset distance; Large tiles don't require as many columns or rows of tiles to fill the same amount of space.";
         // 
         // LayerSize
         // 
         this.LayerSize.StepControl = this.pnlSize;
         this.LayerSize.TitleText = "Layer Size";
         this.LayerSize.InitFunction += new System.EventHandler(this.LayerSize_InitFunction);
         this.LayerSize.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.LayerSize_ValidateFunction);
         // 
         // pnlReview
         // 
         this.pnlReview.Controls.Add(this.txtReview);
         this.pnlReview.Controls.Add(this.lblReview);
         this.pnlReview.Location = new System.Drawing.Point(-10168, 42);
         this.pnlReview.Name = "pnlReview";
         this.pnlReview.Size = new System.Drawing.Size(285, 231);
         this.pnlReview.TabIndex = 13;
         // 
         // txtReview
         // 
         this.txtReview.Location = new System.Drawing.Point(8, 72);
         this.txtReview.Multiline = true;
         this.txtReview.Name = "txtReview";
         this.txtReview.ReadOnly = true;
         this.txtReview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtReview.Size = new System.Drawing.Size(264, 152);
         this.txtReview.TabIndex = 1;
         this.txtReview.Text = "";
         // 
         // lblReview
         // 
         this.lblReview.Location = new System.Drawing.Point(8, 8);
         this.lblReview.Name = "lblReview";
         this.lblReview.Size = new System.Drawing.Size(264, 64);
         this.lblReview.TabIndex = 0;
         this.lblReview.Text = "Below is a summary of the changes you have selected.  These changes will not take" +
            " effect until the layer is updated in Layer Manager with the Update or Add butto" +
            "n.";
         // 
         // Review
         // 
         this.Review.StepControl = this.pnlReview;
         this.Review.TitleText = "Review";
         this.Review.InitFunction += new System.EventHandler(this.Review_InitFunction);
         this.Review.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.Review_ValidateFunction);
         // 
         // VirtualSize
         // 
         this.VirtualSize.StepControl = this.pnlVirtualSize;
         this.VirtualSize.TitleText = "Virtual Size";
         this.VirtualSize.InitFunction += new System.EventHandler(this.VirtualSize_InitFunction);
         this.VirtualSize.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.VirtualSize_ValidateFunction);
         // 
         // pnlVirtualSize
         // 
         this.pnlVirtualSize.Controls.Add(this.txtVirtualColumns);
         this.pnlVirtualSize.Controls.Add(this.lblVirtualColumns);
         this.pnlVirtualSize.Controls.Add(this.txtVirtualRows);
         this.pnlVirtualSize.Controls.Add(this.lblVirtualRows);
         this.pnlVirtualSize.Controls.Add(this.chkFitVirtualToMap);
         this.pnlVirtualSize.Controls.Add(this.lblVirtualSizePrompt);
         this.pnlVirtualSize.Location = new System.Drawing.Point(-10168, 42);
         this.pnlVirtualSize.Name = "pnlVirtualSize";
         this.pnlVirtualSize.Size = new System.Drawing.Size(280, 231);
         this.pnlVirtualSize.TabIndex = 14;
         // 
         // txtVirtualColumns
         // 
         this.txtVirtualColumns.Location = new System.Drawing.Point(88, 144);
         this.txtVirtualColumns.MaxLength = 7;
         this.txtVirtualColumns.Name = "txtVirtualColumns";
         this.txtVirtualColumns.Size = new System.Drawing.Size(72, 20);
         this.txtVirtualColumns.TabIndex = 11;
         this.txtVirtualColumns.Text = "";
         // 
         // lblVirtualColumns
         // 
         this.lblVirtualColumns.Location = new System.Drawing.Point(8, 144);
         this.lblVirtualColumns.Name = "lblVirtualColumns";
         this.lblVirtualColumns.Size = new System.Drawing.Size(80, 20);
         this.lblVirtualColumns.TabIndex = 10;
         this.lblVirtualColumns.Text = "Tile Columns:";
         this.lblVirtualColumns.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtVirtualRows
         // 
         this.txtVirtualRows.Location = new System.Drawing.Point(88, 168);
         this.txtVirtualRows.MaxLength = 7;
         this.txtVirtualRows.Name = "txtVirtualRows";
         this.txtVirtualRows.Size = new System.Drawing.Size(72, 20);
         this.txtVirtualRows.TabIndex = 9;
         this.txtVirtualRows.Text = "";
         // 
         // lblVirtualRows
         // 
         this.lblVirtualRows.Location = new System.Drawing.Point(8, 168);
         this.lblVirtualRows.Name = "lblVirtualRows";
         this.lblVirtualRows.Size = new System.Drawing.Size(80, 20);
         this.lblVirtualRows.TabIndex = 8;
         this.lblVirtualRows.Text = "Tile Rows:";
         this.lblVirtualRows.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // chkFitVirtualToMap
         // 
         this.chkFitVirtualToMap.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.chkFitVirtualToMap.Location = new System.Drawing.Point(8, 104);
         this.chkFitVirtualToMap.Name = "chkFitVirtualToMap";
         this.chkFitVirtualToMap.Size = new System.Drawing.Size(264, 32);
         this.chkFitVirtualToMap.TabIndex = 7;
         this.chkFitVirtualToMap.Text = "Size the layer to fill the map\'s scrollable area. (Assumes single-view mode or sc" +
            "roll rate=1.)";
         this.chkFitVirtualToMap.CheckedChanged += new System.EventHandler(this.chkFitVirtualToMap_CheckedChanged);
         // 
         // lblVirtualSizePrompt
         // 
         this.lblVirtualSizePrompt.Location = new System.Drawing.Point(8, 8);
         this.lblVirtualSizePrompt.Name = "lblVirtualSizePrompt";
         this.lblVirtualSizePrompt.Size = new System.Drawing.Size(264, 96);
         this.lblVirtualSizePrompt.TabIndex = 6;
         this.lblVirtualSizePrompt.Text = @"The virtual size of the layer determines the display size of the layer independently of how many tiles are actually defined in it.  By creating a layer whose virtual size is larger than the layer, you can achieve a wrapping effect in which moving beyond the edge of the layer wraps back to the beginning.  Setting these to 0 uses the layer's actual size.";
         // 
         // frmLayerWizard
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(450, 313);
         this.Controls.Add(this.pnlVirtualSize);
         this.Controls.Add(this.pnlReview);
         this.Controls.Add(this.pnlSize);
         this.Controls.Add(this.pnlPriority);
         this.Controls.Add(this.pnlScrollRate);
         this.Controls.Add(this.pnlOffset);
         this.Controls.Add(this.pnlBackgroundTile);
         this.Controls.Add(this.pnlBytesPerTile);
         this.Controls.Add(this.pnlTileset);
         this.Name = "frmLayerWizard";
         this.Steps.Add(this.Tileset);
         this.Steps.Add(this.BytesPerTile);
         this.Steps.Add(this.BackgroundTile);
         this.Steps.Add(this.ScrollRate);
         this.Steps.Add(this.Offset);
         this.Steps.Add(this.Priority);
         this.Steps.Add(this.LayerSize);
         this.Steps.Add(this.VirtualSize);
         this.Steps.Add(this.Review);
         this.Text = "Layer Wizard";
         this.Controls.SetChildIndex(this.pnlTileset, 0);
         this.Controls.SetChildIndex(this.pnlBytesPerTile, 0);
         this.Controls.SetChildIndex(this.pnlBackgroundTile, 0);
         this.Controls.SetChildIndex(this.pnlOffset, 0);
         this.Controls.SetChildIndex(this.pnlScrollRate, 0);
         this.Controls.SetChildIndex(this.pnlPriority, 0);
         this.Controls.SetChildIndex(this.pnlSize, 0);
         this.Controls.SetChildIndex(this.pnlReview, 0);
         this.Controls.SetChildIndex(this.pnlVirtualSize, 0);
         this.pnlTileset.ResumeLayout(false);
         this.pnlBytesPerTile.ResumeLayout(false);
         this.pnlBackgroundTile.ResumeLayout(false);
         this.pnlOffset.ResumeLayout(false);
         this.pnlScrollRate.ResumeLayout(false);
         this.pnlPriority.ResumeLayout(false);
         this.groupBox1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.nudPriority)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudZIndex)).EndInit();
         this.pnlSize.ResumeLayout(false);
         this.pnlReview.ResumeLayout(false);
         this.pnlVirtualSize.ResumeLayout(false);
         this.ResumeLayout(false);

      }
      #endregion

      #region Private Members
      private void PopulateLayerList()
      {
         lstLayers.Items.Clear();
         ProjectDataset.LayerRow[] lrs = ProjectData.GetSortedLayers(m_Layer.MapRow);
         bool bAdded = false;
         for (int i=0; i<lrs.Length; i++)
         {
            if (lrs[i] != m_Layer)
            {
               if ((nudZIndex.Value <= lrs[i].ZIndex) && (!bAdded))
               {
                  lstLayers.Items.Insert(0, ">> " + m_Layer.Name + " <<");
                  bAdded = true;
               }
               lstLayers.Items.Insert(0, lrs[i].Name);
            }
         }
         if (!bAdded)
            lstLayers.Items.Insert(0, ">> " + m_Layer.Name + " <<");
      }

      private bool SaveAll()
      {
         ProjectDataset.LayerRow[] lrs = ProjectData.GetSortedLayers(m_Layer.MapRow);
         bool ZReindexRequired = false;
         if (nudZIndex.Value != m_Layer.ZIndex)
         {
            for (int i=0; i<lrs.Length; i++)
            {
               if (lrs[i] == m_Layer)
                  continue;
               if (nudZIndex.Value == lrs[i].ZIndex)
               {
                  if (DialogResult.Cancel == MessageBox.Show(this, "The specified Z-Index for this layer conflicts with that of another layer.  If you contine, the layers of this map will be re-indexed with unique ZIndex values?  This action cannot be undone by clicking cancel on the Layer Manager window.", "ZIndex Conflict", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                     return false;
                  ZReindexRequired = true;
                  break;
               }
            }
         }

         if (ZReindexRequired)
         {
            int targetIdx;
            for (targetIdx=Array.IndexOf(lrs, m_Layer); (targetIdx>0) &&
               (lrs[targetIdx-1].ZIndex >= nudZIndex.Value); targetIdx--)
               lrs[targetIdx] = lrs[targetIdx-1];
            lrs[targetIdx] = m_Layer;
            for (int i=0; i<lrs.Length; i++)
               lrs[i].ZIndex = i;
         }
         else
            m_Layer.ZIndex = (int)nudZIndex.Value;

         m_Layer.Priority = (int)nudPriority.Value;

         m_Layer.Tileset = cboTileset.Text;
         if (chkChangeBackground.Checked)
            m_LayerProperties.BackgroundTile = BackgroundTileSelector.CurrentCellIndex;
         else
            m_LayerProperties.BackgroundTile = -1;

         switch(cboBytesPerTile.SelectedIndex)
         {
            case 0:
               m_Layer.BytesPerTile = 1;
               break;
            case 1:
               m_Layer.BytesPerTile = 2;
               break;
            case 2:
               m_Layer.BytesPerTile = 4;
               break;
         }

         m_Layer.OffsetX = m_ProposedOffset.X;
         m_Layer.OffsetY = m_ProposedOffset.Y;
         m_Layer.ScrollRateX = (float)m_nScrollRateX;
         m_Layer.ScrollRateY = (float)m_nScrollRateY;
         m_Layer.Width = m_PropsedSize.Width;
         m_Layer.Height = m_PropsedSize.Height;
         m_Layer.VirtualWidth = m_PropsedVirtualSize.Width;
         m_Layer.VirtualHeight = m_PropsedVirtualSize.Height;

         return true;
      }
      #endregion

      #region Event Handlers
      private void Tileset_InitFunction(object sender, System.EventArgs e)
      {
         if (cboTileset.Items.Count != ProjectData.Tileset.DefaultView.Count)
         {
            cboTileset.Items.Clear();
            foreach(DataRowView drv in ProjectData.Tileset.DefaultView)
            {
               ProjectDataset.TilesetRow tileset = (ProjectDataset.TilesetRow)drv.Row;
               int idx = cboTileset.Items.Add(tileset.Name);
               if (tileset.Name == m_Layer.Tileset)
                  cboTileset.SelectedIndex = idx;
            }
         }
      }

      private void cboTileset_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (cboTileset.SelectedIndex < 0)
            return;
         PreviewTiles.FramesToDisplay = null;
         ProjectDataset.TilesetRow tileset = ProjectData.GetTileSet(cboTileset.Text);
         TileCache cache = new TileCache(tileset);
         FrameList tileProvider = new FrameList();
         for (int i=0; i<cache.Count; i++)
         {
            tileProvider.Add(new TileProvider(cache, i));
         }
         PreviewTiles.Frameset = tileset.FramesetRow;
         PreviewTiles.FramesToDisplay = tileProvider;
         m_nTileCount = cache.Count;
      }

      private void BytesPerTile_InitFunction(object sender, System.EventArgs e)
      {
         if (m_nTileCount <= 256)
            cboBytesPerTile.SelectedIndex = 0;
         else if (m_nTileCount <= 32768)
            cboBytesPerTile.SelectedIndex = 1;
         else
            cboBytesPerTile.SelectedIndex = 2;
      }

      private bool Tileset_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         if (cboTileset.SelectedIndex < 0)
            return false;
         else
            return true;
      }

      private void BackgroundTile_InitFunction(object sender, System.EventArgs e)
      {
         if (chkChangeBackground.Checked)
         {
            BackgroundTileSelector.Frameset = PreviewTiles.Frameset;
            BackgroundTileSelector.FramesToDisplay = PreviewTiles.FramesToDisplay;
            BackgroundTileSelector.Enabled = true;
         }
         else
         {
            BackgroundTileSelector.FramesToDisplay = null;
            BackgroundTileSelector.Frameset = null;
            BackgroundTileSelector.Enabled = false;
         }      
      }

      private void ScrollRate_InitFunction(object sender, System.EventArgs e)
      {
         if (txtScrollRateX.Text.Length == 0)
         {
            txtScrollRateX.Text = m_Layer.ScrollRateX.ToString();
            txtScrollRateY.Text = m_Layer.ScrollRateY.ToString();
         }
      }

      private bool BackgroundTile_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         if ((chkChangeBackground.Checked) && (BackgroundTileSelector.CurrentCellIndex < 0))
         {
            MessageBox.Show(this, "Must select a tile if setting the background.", "Background Tile", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         else
            return true;
      }

      private bool ScrollRate_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         if (!double.TryParse(txtScrollRateX.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out m_nScrollRateX) || m_nScrollRateX < 0)
         {
            MessageBox.Show(this, "Invalid entry for Horizontal Scroll Rate", "Scroll Rate", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         if (!double.TryParse(txtScrollRateY.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out m_nScrollRateY) || m_nScrollRateY < 0)
         {
            MessageBox.Show(this, "Invalid entry for Vertical Scroll Rate", "Scroll Rate", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         return true;
      }

      private void Offset_InitFunction(object sender, System.EventArgs e)
      {
         ProjectDataset.LayerRow[] lrs = ProjectData.GetSortedLayers(m_Layer.MapRow);
         m_Layers = new Layer[lrs.Length];
         for(int i=0; i<lrs.Length; i++)
         {
            m_Layers[i] = new Layer(lrs[i], offsetDisplay);
            if (lrs[i] == m_Layer)
               m_nCurLayer = i;
         }
         if (m_RememberedOffset == Point.Empty)
            m_RememberedOffset = m_ProposedOffset = new Point(m_Layer.OffsetX, m_Layer.OffsetY);
      }

      private void offsetDisplay_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         if (m_Layers == null)
            return;
         Size ScrollBounds = new Size(m_Layer.MapRow.ScrollWidth, m_Layer.MapRow.ScrollHeight);
         if (ScrollBounds != offsetDisplay.AutoScrollMinSize)
            offsetDisplay.AutoScrollMinSize = ScrollBounds;
         offsetDisplay.Device.Clear(Microsoft.DirectX.Direct3D.ClearFlags.Target, 0, 1.0f, 0);
         offsetDisplay.Device.BeginScene();
         for (int i=0; i<m_Layers.Length; i++)
         {
            Layer DrawLayer = m_Layers[i];
            if (i == m_nCurLayer)
               DrawLayer.CurrentPosition = new Point(m_ProposedOffset.X + (int)(offsetDisplay.AutoScrollPosition.X * m_nScrollRateX), m_ProposedOffset.Y + (int)(offsetDisplay.AutoScrollPosition.Y * m_nScrollRateY));
            else
               DrawLayer.Move(offsetDisplay.AutoScrollPosition.X, offsetDisplay.AutoScrollPosition.Y);
            m_Layers[i].Draw(offsetDisplay, offsetDisplay.ClientSize);
         }
         offsetDisplay.Device.EndScene();
         offsetDisplay.Device.Present();
      }

      private void offsetDisplay_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         m_ProposedOffset = new Point(e.X - (int)(offsetDisplay.AutoScrollPosition.X * m_nScrollRateX),
            e.Y - (int)(offsetDisplay.AutoScrollPosition.Y * m_nScrollRateY));
         lblOffset.Text = "OffsetX: " + m_ProposedOffset.X.ToString() + "; OffsetY: " + m_ProposedOffset.Y.ToString();
         offsetDisplay.Invalidate();
      }

      private void offsetDisplay_MouseLeave(object sender, System.EventArgs e)
      {
         m_ProposedOffset = m_RememberedOffset;
         lblOffset.Text = "OffsetX: " + m_ProposedOffset.X.ToString() + "; OffsetY: " + m_ProposedOffset.Y.ToString();
         offsetDisplay.Invalidate();
      }

      private void offsetDisplay_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         m_RememberedOffset = m_ProposedOffset;
      }

      private void Priority_InitFunction(object sender, System.EventArgs e)
      {
         if (lstLayers.Items.Count != ProjectData.Layer.DefaultView.Count)
         {
            PopulateLayerList();
            nudZIndex.Value = m_Layer.ZIndex;
            nudPriority.Value = m_Layer.Priority;
         }
      }

      private void nudZIndex_ValueChanged(object sender, System.EventArgs e)
      {
         PopulateLayerList();
      }

      private void LayerSize_InitFunction(object sender, System.EventArgs e)
      {
         if (txtTileRows.Text.Length == 0)
         {
            txtTileRows.Text = m_Layer.Height.ToString();
            txtTileCols.Text = m_Layer.Width.ToString();
         }
      }

      private void chkFitToMap_CheckedChanged(object sender, System.EventArgs e)
      {
         if (chkFitToMap.Checked)
         {
            int scrollDist;
            if (m_ViewSize.Width > m_Layer.MapRow.ScrollWidth)
               scrollDist = 0;
            else
               scrollDist = m_Layer.MapRow.ScrollWidth - m_ViewSize.Width;
            txtTileCols.Text = ((int)Math.Ceiling((scrollDist * m_nScrollRateX + m_ViewSize.Width - m_ProposedOffset.X) / m_Layer.TilesetRow.TileWidth)).ToString();
            if (m_ViewSize.Height > m_Layer.MapRow.ScrollHeight)
               scrollDist = 0;
            else
               scrollDist = m_Layer.MapRow.ScrollHeight - m_ViewSize.Height;
            txtTileRows.Text = ((int)Math.Ceiling((scrollDist * m_nScrollRateY + m_ViewSize.Height - m_ProposedOffset.Y) / m_Layer.TilesetRow.TileHeight)).ToString();
         }
         else
         {
            txtTileCols.Text = m_Layer.Width.ToString();
            txtTileRows.Text = m_Layer.Height.ToString();
         }
      }

      private bool LayerSize_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         double dblTemp;
         if (!double.TryParse(txtTileCols.Text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out dblTemp))
         {
            MessageBox.Show(this, "Invalid value specified for Tile Columns", "Layer Size", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         if ((dblTemp < 1) || (dblTemp > 9999999))
         {
            MessageBox.Show(this, "Number out of valid range for Tile Columns", "Layer Size", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         m_PropsedSize.Width = (int)dblTemp;

         if (!double.TryParse(txtTileRows.Text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out dblTemp))
         {
            MessageBox.Show(this, "Invalid value specified for Tile Rows", "Layer Size", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         if ((dblTemp < 1) || (dblTemp > 9999999))
         {
            MessageBox.Show(this, "Number out of valid range for Tile Rows", "Layer Size", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         m_PropsedSize.Height = (int)dblTemp;

         return true;
      }

      private void VirtualSize_InitFunction(object sender, System.EventArgs e)
      {
         if (txtVirtualRows.Text.Length == 0)
         {
            txtVirtualRows.Text = m_Layer.VirtualHeight.ToString();
            txtVirtualColumns.Text = m_Layer.VirtualWidth.ToString();
         }      
      }

      private void chkFitVirtualToMap_CheckedChanged(object sender, System.EventArgs e)
      {
         if (chkFitVirtualToMap.Checked)
         {
            int scrollDist;
            if (m_ViewSize.Width > m_Layer.MapRow.ScrollWidth)
               scrollDist = 0;
            else
               scrollDist = m_Layer.MapRow.ScrollWidth - m_ViewSize.Width;
            txtVirtualColumns.Text = ((int)Math.Ceiling((scrollDist * m_nScrollRateX + m_ViewSize.Width - m_ProposedOffset.X) / m_Layer.TilesetRow.TileWidth)).ToString();
            if (m_ViewSize.Height > m_Layer.MapRow.ScrollHeight)
               scrollDist = 0;
            else
               scrollDist = m_Layer.MapRow.ScrollHeight - m_ViewSize.Height;
            txtVirtualRows.Text = ((int)Math.Ceiling((scrollDist * m_nScrollRateY + m_ViewSize.Height - m_ProposedOffset.Y) / m_Layer.TilesetRow.TileHeight)).ToString();
         }
         else
         {
            txtVirtualColumns.Text = m_Layer.VirtualWidth.ToString();
            txtVirtualRows.Text = m_Layer.VirtualHeight.ToString();
         }      
      }

      private bool VirtualSize_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         double dblTemp;
         if (!double.TryParse(txtVirtualColumns.Text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out dblTemp))
         {
            MessageBox.Show(this, "Invalid value specified for Tile Columns", "Layer Virtual Size", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         if ((dblTemp < 0) || (dblTemp > 9999999))
         {
            MessageBox.Show(this, "Number out of valid range for Tile Columns", "Layer Virtual Size", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         m_PropsedVirtualSize.Width = (int)dblTemp;

         if (!double.TryParse(txtVirtualRows.Text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out dblTemp))
         {
            MessageBox.Show(this, "Invalid value specified for Tile Rows", "Layer Virtual Size", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         if ((dblTemp < 0) || (dblTemp > 9999999))
         {
            MessageBox.Show(this, "Number out of valid range for Tile Rows", "Layer Virtual Size", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         m_PropsedVirtualSize.Height = (int)dblTemp;

         return true;
      }

      private void Review_InitFunction(object sender, System.EventArgs e)
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();

         if (cboTileset.Text != m_Layer.Tileset)
            sb.Append("Change Tileset from \"" + m_Layer.Tileset + "\" to \"" + cboTileset.Text + "\".\r\n");

         if (chkChangeBackground.Checked)
            sb.Append("Clear layer to background of tile number " + BackgroundTileSelector.CurrentCellIndex.ToString() + ".\r\n");

         int bpt = 0;
         switch(cboBytesPerTile.SelectedIndex)
         {
            case 0:
               bpt = 1;
               break;
            case 1:
               bpt = 2;
               break;
            case 2:
               bpt = 4;
               break;
         }
         if (bpt != m_Layer.BytesPerTile)
            sb.Append("Change BytesPerTile from " + m_Layer.BytesPerTile.ToString() + " to " + bpt.ToString() + ".\r\n");

         if (m_Layer.ScrollRateX != (float)m_nScrollRateX)
            sb.Append("Change scroll rate from " + m_Layer.ScrollRateX.ToString() + ", " + m_Layer.ScrollRateY.ToString() + " to " +
               m_nScrollRateX.ToString() + ", " + m_nScrollRateY.ToString() + ".\r\n");

         if ((m_ProposedOffset.X != m_Layer.OffsetX) || (m_ProposedOffset.Y != m_Layer.OffsetY))
            sb.Append("Change offset from " + m_Layer.OffsetX.ToString() + ", " + m_Layer.OffsetY.ToString() + " to " + 
               m_ProposedOffset.X + ", " + m_ProposedOffset.Y + ".\r\n");

         if (nudZIndex.Value != m_Layer.ZIndex)
            sb.Append("Change ZIndex from " + m_Layer.ZIndex.ToString() + " to " + nudZIndex.Value.ToString() + ".\r\n");
         if (nudPriority.Value != m_Layer.Priority)
            sb.Append("Change Priority from " + m_Layer.Priority.ToString() + " to " + nudPriority.Value.ToString() + ".\r\n");

         if ((m_Layer.Width != m_PropsedSize.Width) || (m_Layer.Height != m_PropsedSize.Height))
         {
            sb.Append("Change layer size from " + m_Layer.Width.ToString() + ", " + m_Layer.Height.ToString() + " to " +
               m_PropsedSize.Width.ToString() + ", " + m_PropsedSize.Height.ToString() + ".\r\n");
         }

         if ((m_Layer.VirtualWidth != m_PropsedVirtualSize.Width) || (m_Layer.VirtualHeight != m_PropsedVirtualSize.Height))
         {
            sb.Append("Change layer virtual size from " + m_Layer.VirtualWidth.ToString() + ", " + m_Layer.VirtualHeight.ToString() + " to " +
               m_PropsedVirtualSize.Width.ToString() + ", " + m_PropsedVirtualSize.Height.ToString() + ".\r\n");
         }

         txtReview.Text = sb.ToString();
      }

      private bool Review_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         return SaveAll();
      }
   }
   #endregion
}

