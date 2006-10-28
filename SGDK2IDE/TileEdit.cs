using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace SGDK2
{
	/// <summary>
	/// Form for managing tilesets -- tile mappings, animations and sizes.
	/// </summary>
	public class frmTileEdit : System.Windows.Forms.Form
	{
      #region Non-control members
      ProjectDataset.TilesetRow m_Tileset = null;
      ProjectDataset.TileRow m_Tile = null;
      #endregion

      #region Control Members
      private System.Windows.Forms.Label lblTilesetName;
      private System.Windows.Forms.TextBox txtTilesetName;
      private System.Windows.Forms.Label lblFrameset;
      private System.Windows.Forms.ComboBox cboFrameset;
      private System.Windows.Forms.Splitter splitter1;
      private System.Windows.Forms.MainMenu mnuTileset;
      private System.Windows.Forms.MenuItem mnuInsert;
      private System.Windows.Forms.Panel pnlTileHeader;
      private System.Windows.Forms.ToolTip ttTileset;
      private System.Windows.Forms.Label lblCustomizeTile;
      private System.Windows.Forms.ComboBox cboMappedTiles;
      private System.Windows.Forms.ImageList imlTileset;
      private System.Windows.Forms.ToolBar tbMappedTiles;
      private System.Windows.Forms.ToolBarButton tbbNewTile;
      private System.Windows.Forms.ToolBarButton tbbDeleteTile;
      private SGDK2.GraphicBrowser AvailableFrames;
      private System.Windows.Forms.GroupBox grpTileProperties;
      private System.Windows.Forms.GroupBox grpAvailableFrames;
      private System.Windows.Forms.Panel pnlTileProperties;
      private System.Windows.Forms.ComboBox cboFrameCounter;
      private System.Windows.Forms.Label lblFrameCounter;
      private System.Windows.Forms.Label lblFrames;
      private System.Windows.Forms.Panel pnlFrames;
      private SGDK2.GraphicBrowser TileFrames;
      private System.Windows.Forms.Panel pnlFrameProperties;
      private System.Windows.Forms.GroupBox grpFrameProperties;
      private System.Windows.Forms.NumericUpDown updRepeatCount;
      private System.Windows.Forms.Panel pnlAddFrame;
      private System.Windows.Forms.Button btnAddFrame;
      private System.Windows.Forms.Label lblRepeatCount;
      private System.Windows.Forms.MenuItem mnuInsertNewTile;
      private System.Windows.Forms.Button btnDeleteFrame;
      private System.Windows.Forms.MenuItem mnuDeleteTile;
      private System.ComponentModel.IContainer components;
      private System.Windows.Forms.Label lblTilewidth;
      private System.Windows.Forms.NumericUpDown nudTileWidth;
      private System.Windows.Forms.NumericUpDown nudTileHeight;
      private System.Windows.Forms.Label lblTileHeight;
      private System.Windows.Forms.StatusBar StatusBar;
      private DataChangeNotifier dataMonitor;
      #endregion

      #region Initialization and clean-up
		public frmTileEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         String sName;
         Int32 nIdx = 1;
         do
            sName = "New Tileset " + (nIdx++).ToString();
         while (ProjectData.GetTileSet(sName) != null);

         if (ProjectData.Frameset.DefaultView.Count <= 0)
            throw new ApplicationException("Please create a Frameset before creating a Tileset");
         ProjectDataset.FramesetRow fr = (ProjectDataset.FramesetRow)ProjectData.Frameset.DefaultView[0].Row;
         ProjectDataset.FrameRow[] f = ProjectData.GetSortedFrameRows(fr);
         if (f.Length > 0)
         {
            ProjectDataset.GraphicSheetRow g = ProjectData.GetGraphicSheet(f[0].GraphicSheet);
            m_Tileset = ProjectData.AddTilesetRow(sName, fr, g.CellWidth, g.CellHeight);
         }
         else
         {
            m_Tileset = ProjectData.AddTilesetRow(sName, fr, 32, 32);
         }
         txtTilesetName.Text = sName;
         nudTileWidth.Value = m_Tileset.TileWidth;
         nudTileHeight.Value = m_Tileset.TileHeight;
         FillFramesets();
         FillCounters();
      }

      public frmTileEdit(ProjectDataset.TilesetRow drTileset)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         m_Tileset = drTileset;
         txtTilesetName.Text = drTileset.Name;
         nudTileWidth.Value = m_Tileset.TileWidth;
         nudTileHeight.Value = m_Tileset.TileHeight;
         TileFrames.Frameset = drTileset.FramesetRow;
         FillFramesets();
         FillCounters();
      }

      #endregion

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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.components = new System.ComponentModel.Container();
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmTileEdit));
         this.lblTilesetName = new System.Windows.Forms.Label();
         this.txtTilesetName = new System.Windows.Forms.TextBox();
         this.lblFrameset = new System.Windows.Forms.Label();
         this.cboFrameset = new System.Windows.Forms.ComboBox();
         this.pnlTileHeader = new System.Windows.Forms.Panel();
         this.lblTileHeight = new System.Windows.Forms.Label();
         this.nudTileHeight = new System.Windows.Forms.NumericUpDown();
         this.nudTileWidth = new System.Windows.Forms.NumericUpDown();
         this.lblTilewidth = new System.Windows.Forms.Label();
         this.tbMappedTiles = new System.Windows.Forms.ToolBar();
         this.tbbNewTile = new System.Windows.Forms.ToolBarButton();
         this.tbbDeleteTile = new System.Windows.Forms.ToolBarButton();
         this.imlTileset = new System.Windows.Forms.ImageList(this.components);
         this.cboMappedTiles = new System.Windows.Forms.ComboBox();
         this.lblCustomizeTile = new System.Windows.Forms.Label();
         this.splitter1 = new System.Windows.Forms.Splitter();
         this.mnuTileset = new System.Windows.Forms.MainMenu();
         this.mnuInsert = new System.Windows.Forms.MenuItem();
         this.mnuInsertNewTile = new System.Windows.Forms.MenuItem();
         this.mnuDeleteTile = new System.Windows.Forms.MenuItem();
         this.ttTileset = new System.Windows.Forms.ToolTip(this.components);
         this.cboFrameCounter = new System.Windows.Forms.ComboBox();
         this.updRepeatCount = new System.Windows.Forms.NumericUpDown();
         this.btnAddFrame = new System.Windows.Forms.Button();
         this.AvailableFrames = new SGDK2.GraphicBrowser();
         this.grpTileProperties = new System.Windows.Forms.GroupBox();
         this.pnlFrames = new System.Windows.Forms.Panel();
         this.TileFrames = new SGDK2.GraphicBrowser();
         this.pnlFrameProperties = new System.Windows.Forms.Panel();
         this.grpFrameProperties = new System.Windows.Forms.GroupBox();
         this.lblRepeatCount = new System.Windows.Forms.Label();
         this.pnlAddFrame = new System.Windows.Forms.Panel();
         this.btnDeleteFrame = new System.Windows.Forms.Button();
         this.pnlTileProperties = new System.Windows.Forms.Panel();
         this.lblFrames = new System.Windows.Forms.Label();
         this.lblFrameCounter = new System.Windows.Forms.Label();
         this.grpAvailableFrames = new System.Windows.Forms.GroupBox();
         this.dataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.StatusBar = new System.Windows.Forms.StatusBar();
         this.pnlTileHeader.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.nudTileHeight)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudTileWidth)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.updRepeatCount)).BeginInit();
         this.grpTileProperties.SuspendLayout();
         this.pnlFrames.SuspendLayout();
         this.pnlFrameProperties.SuspendLayout();
         this.grpFrameProperties.SuspendLayout();
         this.pnlAddFrame.SuspendLayout();
         this.pnlTileProperties.SuspendLayout();
         this.grpAvailableFrames.SuspendLayout();
         this.SuspendLayout();
         // 
         // lblTilesetName
         // 
         this.lblTilesetName.Location = new System.Drawing.Point(8, 0);
         this.lblTilesetName.Name = "lblTilesetName";
         this.lblTilesetName.Size = new System.Drawing.Size(96, 20);
         this.lblTilesetName.TabIndex = 1;
         this.lblTilesetName.Text = "Tileset Name:";
         this.lblTilesetName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtTilesetName
         // 
         this.txtTilesetName.Location = new System.Drawing.Point(104, 0);
         this.txtTilesetName.Name = "txtTilesetName";
         this.txtTilesetName.Size = new System.Drawing.Size(152, 20);
         this.txtTilesetName.TabIndex = 2;
         this.txtTilesetName.Text = "";
         this.txtTilesetName.Validating += new System.ComponentModel.CancelEventHandler(this.txtTilesetName_Validating);
         this.txtTilesetName.Validated += new System.EventHandler(this.txtTilesetName_Validated);
         // 
         // lblFrameset
         // 
         this.lblFrameset.Location = new System.Drawing.Point(8, 24);
         this.lblFrameset.Name = "lblFrameset";
         this.lblFrameset.Size = new System.Drawing.Size(96, 20);
         this.lblFrameset.TabIndex = 3;
         this.lblFrameset.Text = "Frameset:";
         this.lblFrameset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // cboFrameset
         // 
         this.cboFrameset.DisplayMember = "Name";
         this.cboFrameset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboFrameset.Location = new System.Drawing.Point(104, 24);
         this.cboFrameset.Name = "cboFrameset";
         this.cboFrameset.Size = new System.Drawing.Size(152, 21);
         this.cboFrameset.TabIndex = 4;
         this.cboFrameset.ValueMember = "Name";
         this.cboFrameset.SelectedIndexChanged += new System.EventHandler(this.cboFrameset_SelectedIndexChanged);
         // 
         // pnlTileHeader
         // 
         this.pnlTileHeader.Controls.Add(this.lblTileHeight);
         this.pnlTileHeader.Controls.Add(this.nudTileHeight);
         this.pnlTileHeader.Controls.Add(this.nudTileWidth);
         this.pnlTileHeader.Controls.Add(this.lblTilewidth);
         this.pnlTileHeader.Controls.Add(this.tbMappedTiles);
         this.pnlTileHeader.Controls.Add(this.cboMappedTiles);
         this.pnlTileHeader.Controls.Add(this.lblCustomizeTile);
         this.pnlTileHeader.Controls.Add(this.cboFrameset);
         this.pnlTileHeader.Controls.Add(this.lblFrameset);
         this.pnlTileHeader.Controls.Add(this.txtTilesetName);
         this.pnlTileHeader.Controls.Add(this.lblTilesetName);
         this.pnlTileHeader.Dock = System.Windows.Forms.DockStyle.Top;
         this.pnlTileHeader.Location = new System.Drawing.Point(0, 0);
         this.pnlTileHeader.Name = "pnlTileHeader";
         this.pnlTileHeader.Size = new System.Drawing.Size(496, 72);
         this.pnlTileHeader.TabIndex = 1;
         // 
         // lblTileHeight
         // 
         this.lblTileHeight.Location = new System.Drawing.Point(264, 24);
         this.lblTileHeight.Name = "lblTileHeight";
         this.lblTileHeight.Size = new System.Drawing.Size(80, 20);
         this.lblTileHeight.TabIndex = 10;
         this.lblTileHeight.Text = "Tile Height:";
         this.lblTileHeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // nudTileHeight
         // 
         this.nudTileHeight.Location = new System.Drawing.Point(344, 24);
         this.nudTileHeight.Maximum = new System.Decimal(new int[] {
                                                                      999,
                                                                      0,
                                                                      0,
                                                                      0});
         this.nudTileHeight.Minimum = new System.Decimal(new int[] {
                                                                      1,
                                                                      0,
                                                                      0,
                                                                      0});
         this.nudTileHeight.Name = "nudTileHeight";
         this.nudTileHeight.Size = new System.Drawing.Size(56, 20);
         this.nudTileHeight.TabIndex = 11;
         this.nudTileHeight.Value = new System.Decimal(new int[] {
                                                                    32,
                                                                    0,
                                                                    0,
                                                                    0});
         this.nudTileHeight.Validated += new System.EventHandler(this.nudControl_Validated);
         this.nudTileHeight.ValueChanged += new System.EventHandler(this.nudTileHeight_ValueChanged);
         // 
         // nudTileWidth
         // 
         this.nudTileWidth.Location = new System.Drawing.Point(344, 0);
         this.nudTileWidth.Maximum = new System.Decimal(new int[] {
                                                                     999,
                                                                     0,
                                                                     0,
                                                                     0});
         this.nudTileWidth.Minimum = new System.Decimal(new int[] {
                                                                     1,
                                                                     0,
                                                                     0,
                                                                     0});
         this.nudTileWidth.Name = "nudTileWidth";
         this.nudTileWidth.Size = new System.Drawing.Size(56, 20);
         this.nudTileWidth.TabIndex = 9;
         this.nudTileWidth.Value = new System.Decimal(new int[] {
                                                                   32,
                                                                   0,
                                                                   0,
                                                                   0});
         this.nudTileWidth.Validated += new System.EventHandler(this.nudControl_Validated);
         this.nudTileWidth.ValueChanged += new System.EventHandler(this.nudTileWidth_ValueChanged);
         // 
         // lblTilewidth
         // 
         this.lblTilewidth.Location = new System.Drawing.Point(264, 0);
         this.lblTilewidth.Name = "lblTilewidth";
         this.lblTilewidth.Size = new System.Drawing.Size(80, 20);
         this.lblTilewidth.TabIndex = 8;
         this.lblTilewidth.Text = "Tile Width:";
         this.lblTilewidth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // tbMappedTiles
         // 
         this.tbMappedTiles.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
         this.tbMappedTiles.AutoSize = false;
         this.tbMappedTiles.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                                         this.tbbNewTile,
                                                                                         this.tbbDeleteTile});
         this.tbMappedTiles.Divider = false;
         this.tbMappedTiles.Dock = System.Windows.Forms.DockStyle.None;
         this.tbMappedTiles.DropDownArrows = true;
         this.tbMappedTiles.ImageList = this.imlTileset;
         this.tbMappedTiles.Location = new System.Drawing.Point(208, 48);
         this.tbMappedTiles.Name = "tbMappedTiles";
         this.tbMappedTiles.ShowToolTips = true;
         this.tbMappedTiles.Size = new System.Drawing.Size(56, 24);
         this.tbMappedTiles.TabIndex = 7;
         this.tbMappedTiles.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbMappedTiles_ButtonClick);
         // 
         // tbbNewTile
         // 
         this.tbbNewTile.ImageIndex = 0;
         this.tbbNewTile.ToolTipText = "Add a new custom tile mapping.";
         // 
         // tbbDeleteTile
         // 
         this.tbbDeleteTile.Enabled = false;
         this.tbbDeleteTile.ImageIndex = 1;
         this.tbbDeleteTile.ToolTipText = "Delete the current custom tile mapping.";
         // 
         // imlTileset
         // 
         this.imlTileset.ImageSize = new System.Drawing.Size(15, 15);
         this.imlTileset.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlTileset.ImageStream")));
         this.imlTileset.TransparentColor = System.Drawing.Color.Magenta;
         // 
         // cboMappedTiles
         // 
         this.cboMappedTiles.DisplayMember = "TileValue";
         this.cboMappedTiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboMappedTiles.Location = new System.Drawing.Point(104, 48);
         this.cboMappedTiles.Name = "cboMappedTiles";
         this.cboMappedTiles.Size = new System.Drawing.Size(104, 21);
         this.cboMappedTiles.TabIndex = 6;
         this.ttTileset.SetToolTip(this.cboMappedTiles, "Specifies which tile index is being re-mapped/animated");
         this.cboMappedTiles.ValueMember = "TileValue";
         this.cboMappedTiles.SelectedIndexChanged += new System.EventHandler(this.cboMappedTiles_SelectedIndexChanged);
         // 
         // lblCustomizeTile
         // 
         this.lblCustomizeTile.Location = new System.Drawing.Point(8, 48);
         this.lblCustomizeTile.Name = "lblCustomizeTile";
         this.lblCustomizeTile.Size = new System.Drawing.Size(96, 21);
         this.lblCustomizeTile.TabIndex = 5;
         this.lblCustomizeTile.Text = "Mapped Tiles:";
         this.lblCustomizeTile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // splitter1
         // 
         this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
         this.splitter1.Location = new System.Drawing.Point(0, 256);
         this.splitter1.Name = "splitter1";
         this.splitter1.Size = new System.Drawing.Size(496, 6);
         this.splitter1.TabIndex = 3;
         this.splitter1.TabStop = false;
         // 
         // mnuTileset
         // 
         this.mnuTileset.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                   this.mnuInsert});
         // 
         // mnuInsert
         // 
         this.mnuInsert.Index = 0;
         this.mnuInsert.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                  this.mnuInsertNewTile,
                                                                                  this.mnuDeleteTile});
         this.mnuInsert.MergeOrder = 2;
         this.mnuInsert.Text = "&Tileset";
         // 
         // mnuInsertNewTile
         // 
         this.mnuInsertNewTile.Index = 0;
         this.mnuInsertNewTile.Text = "&New Tile";
         this.mnuInsertNewTile.Click += new System.EventHandler(this.mnuNewTile_Click);
         // 
         // mnuDeleteTile
         // 
         this.mnuDeleteTile.Enabled = false;
         this.mnuDeleteTile.Index = 1;
         this.mnuDeleteTile.Text = "&Delete/Reset Tile";
         this.mnuDeleteTile.Click += new System.EventHandler(this.mnuDeleteTile_Click);
         // 
         // cboFrameCounter
         // 
         this.cboFrameCounter.DisplayMember = "Name";
         this.cboFrameCounter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboFrameCounter.Location = new System.Drawing.Point(104, 2);
         this.cboFrameCounter.Name = "cboFrameCounter";
         this.cboFrameCounter.Size = new System.Drawing.Size(144, 21);
         this.cboFrameCounter.TabIndex = 2;
         this.ttTileset.SetToolTip(this.cboFrameCounter, "Names a variable that determines which frame the tile is currently displaying");
         this.cboFrameCounter.SelectedIndexChanged += new System.EventHandler(this.cboFrameCounter_SelectedIndexChanged);
         // 
         // updRepeatCount
         // 
         this.updRepeatCount.Location = new System.Drawing.Point(112, 16);
         this.updRepeatCount.Maximum = new System.Decimal(new int[] {
                                                                       255,
                                                                       0,
                                                                       0,
                                                                       0});
         this.updRepeatCount.Name = "updRepeatCount";
         this.updRepeatCount.Size = new System.Drawing.Size(64, 20);
         this.updRepeatCount.TabIndex = 2;
         this.ttTileset.SetToolTip(this.updRepeatCount, "For how many timer increments will the selected frame be repeated? (0 to merge wi" +
            "th next frame)");
         this.updRepeatCount.Value = new System.Decimal(new int[] {
                                                                     1,
                                                                     0,
                                                                     0,
                                                                     0});
         this.updRepeatCount.Validated += new System.EventHandler(this.nudControl_Validated);
         this.updRepeatCount.ValueChanged += new System.EventHandler(this.updRepeatCount_ValueChanged);
         // 
         // btnAddFrame
         // 
         this.btnAddFrame.Location = new System.Drawing.Point(8, 8);
         this.btnAddFrame.Name = "btnAddFrame";
         this.btnAddFrame.Size = new System.Drawing.Size(96, 24);
         this.btnAddFrame.TabIndex = 1;
         this.btnAddFrame.Text = "^ &Add Frames ^";
         this.ttTileset.SetToolTip(this.btnAddFrame, "Add the selected frame from \"Available Frames\" to the current tile\'s animation");
         this.btnAddFrame.Click += new System.EventHandler(this.btnAddFrame_Click);
         // 
         // AvailableFrames
         // 
         this.AvailableFrames.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.AvailableFrames.CellPadding = new System.Drawing.Size(6, 6);
         this.AvailableFrames.CellSize = new System.Drawing.Size(0, 0);
         this.AvailableFrames.CurrentCellIndex = -1;
         this.AvailableFrames.Dock = System.Windows.Forms.DockStyle.Fill;
         this.AvailableFrames.Frameset = null;
         this.AvailableFrames.FramesToDisplay = null;
         this.AvailableFrames.GraphicSheet = null;
         this.AvailableFrames.Location = new System.Drawing.Point(3, 16);
         this.AvailableFrames.Name = "AvailableFrames";
         this.AvailableFrames.SheetImage = null;
         this.AvailableFrames.Size = new System.Drawing.Size(490, 122);
         this.AvailableFrames.TabIndex = 1;
         this.AvailableFrames.CurrentCellChanged += new System.EventHandler(this.AvailableFrames_CurrentCellChanged);
         // 
         // grpTileProperties
         // 
         this.grpTileProperties.Controls.Add(this.pnlFrames);
         this.grpTileProperties.Controls.Add(this.pnlTileProperties);
         this.grpTileProperties.Dock = System.Windows.Forms.DockStyle.Top;
         this.grpTileProperties.Location = new System.Drawing.Point(0, 72);
         this.grpTileProperties.Name = "grpTileProperties";
         this.grpTileProperties.Size = new System.Drawing.Size(496, 184);
         this.grpTileProperties.TabIndex = 2;
         this.grpTileProperties.TabStop = false;
         this.grpTileProperties.Text = "Tile Properties";
         // 
         // pnlFrames
         // 
         this.pnlFrames.Controls.Add(this.TileFrames);
         this.pnlFrames.Controls.Add(this.pnlFrameProperties);
         this.pnlFrames.Dock = System.Windows.Forms.DockStyle.Fill;
         this.pnlFrames.DockPadding.Left = 5;
         this.pnlFrames.DockPadding.Right = 5;
         this.pnlFrames.Location = new System.Drawing.Point(3, 56);
         this.pnlFrames.Name = "pnlFrames";
         this.pnlFrames.Size = new System.Drawing.Size(490, 125);
         this.pnlFrames.TabIndex = 2;
         // 
         // TileFrames
         // 
         this.TileFrames.AllowDrop = true;
         this.TileFrames.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.TileFrames.CellPadding = new System.Drawing.Size(6, 6);
         this.TileFrames.CellSize = new System.Drawing.Size(0, 0);
         this.TileFrames.CurrentCellIndex = -1;
         this.TileFrames.Dock = System.Windows.Forms.DockStyle.Fill;
         this.TileFrames.Frameset = null;
         this.TileFrames.FramesToDisplay = null;
         this.TileFrames.GraphicSheet = null;
         this.TileFrames.Location = new System.Drawing.Point(5, 0);
         this.TileFrames.Name = "TileFrames";
         this.TileFrames.SheetImage = null;
         this.TileFrames.Size = new System.Drawing.Size(480, 85);
         this.TileFrames.TabIndex = 1;
         this.TileFrames.CurrentCellChanged += new System.EventHandler(this.TileFrames_CurrentCellChanged);
         this.TileFrames.DragDrop += new System.Windows.Forms.DragEventHandler(this.TileFrames_DragDrop);
         this.TileFrames.DragOver += new System.Windows.Forms.DragEventHandler(this.TileFrames_DragOver);
         // 
         // pnlFrameProperties
         // 
         this.pnlFrameProperties.Controls.Add(this.grpFrameProperties);
         this.pnlFrameProperties.Controls.Add(this.pnlAddFrame);
         this.pnlFrameProperties.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.pnlFrameProperties.Location = new System.Drawing.Point(5, 85);
         this.pnlFrameProperties.Name = "pnlFrameProperties";
         this.pnlFrameProperties.Size = new System.Drawing.Size(480, 40);
         this.pnlFrameProperties.TabIndex = 2;
         // 
         // grpFrameProperties
         // 
         this.grpFrameProperties.Controls.Add(this.updRepeatCount);
         this.grpFrameProperties.Controls.Add(this.lblRepeatCount);
         this.grpFrameProperties.Dock = System.Windows.Forms.DockStyle.Fill;
         this.grpFrameProperties.Location = new System.Drawing.Point(0, 0);
         this.grpFrameProperties.Name = "grpFrameProperties";
         this.grpFrameProperties.Size = new System.Drawing.Size(264, 40);
         this.grpFrameProperties.TabIndex = 1;
         this.grpFrameProperties.TabStop = false;
         this.grpFrameProperties.Text = "Current Frame Properties";
         // 
         // lblRepeatCount
         // 
         this.lblRepeatCount.Location = new System.Drawing.Point(8, 16);
         this.lblRepeatCount.Name = "lblRepeatCount";
         this.lblRepeatCount.Size = new System.Drawing.Size(104, 20);
         this.lblRepeatCount.TabIndex = 0;
         this.lblRepeatCount.Text = "Repeat Count:";
         this.lblRepeatCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // pnlAddFrame
         // 
         this.pnlAddFrame.Controls.Add(this.btnDeleteFrame);
         this.pnlAddFrame.Controls.Add(this.btnAddFrame);
         this.pnlAddFrame.Dock = System.Windows.Forms.DockStyle.Right;
         this.pnlAddFrame.Location = new System.Drawing.Point(264, 0);
         this.pnlAddFrame.Name = "pnlAddFrame";
         this.pnlAddFrame.Size = new System.Drawing.Size(216, 40);
         this.pnlAddFrame.TabIndex = 2;
         // 
         // btnDeleteFrame
         // 
         this.btnDeleteFrame.Location = new System.Drawing.Point(112, 8);
         this.btnDeleteFrame.Name = "btnDeleteFrame";
         this.btnDeleteFrame.Size = new System.Drawing.Size(96, 24);
         this.btnDeleteFrame.TabIndex = 2;
         this.btnDeleteFrame.Text = "&Delete Frames";
         this.btnDeleteFrame.Click += new System.EventHandler(this.btnDeleteFrame_Click);
         // 
         // pnlTileProperties
         // 
         this.pnlTileProperties.Controls.Add(this.lblFrames);
         this.pnlTileProperties.Controls.Add(this.cboFrameCounter);
         this.pnlTileProperties.Controls.Add(this.lblFrameCounter);
         this.pnlTileProperties.Dock = System.Windows.Forms.DockStyle.Top;
         this.pnlTileProperties.Location = new System.Drawing.Point(3, 16);
         this.pnlTileProperties.Name = "pnlTileProperties";
         this.pnlTileProperties.Size = new System.Drawing.Size(490, 40);
         this.pnlTileProperties.TabIndex = 1;
         // 
         // lblFrames
         // 
         this.lblFrames.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.lblFrames.Location = new System.Drawing.Point(0, 24);
         this.lblFrames.Name = "lblFrames";
         this.lblFrames.Size = new System.Drawing.Size(490, 16);
         this.lblFrames.TabIndex = 3;
         this.lblFrames.Text = "Drag frames for tile from \"Available Frames\", or click \"Add Frame\":";
         // 
         // lblFrameCounter
         // 
         this.lblFrameCounter.Location = new System.Drawing.Point(8, 2);
         this.lblFrameCounter.Name = "lblFrameCounter";
         this.lblFrameCounter.Size = new System.Drawing.Size(96, 20);
         this.lblFrameCounter.TabIndex = 1;
         this.lblFrameCounter.Text = "Frame Counter:";
         this.lblFrameCounter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // grpAvailableFrames
         // 
         this.grpAvailableFrames.Controls.Add(this.AvailableFrames);
         this.grpAvailableFrames.Dock = System.Windows.Forms.DockStyle.Fill;
         this.grpAvailableFrames.Location = new System.Drawing.Point(0, 262);
         this.grpAvailableFrames.Name = "grpAvailableFrames";
         this.grpAvailableFrames.Size = new System.Drawing.Size(496, 141);
         this.grpAvailableFrames.TabIndex = 4;
         this.grpAvailableFrames.TabStop = false;
         this.grpAvailableFrames.Text = "Available Frames";
         // 
         // dataMonitor
         // 
         this.dataMonitor.TileRowDeleted += new SGDK2.ProjectDataset.TileRowChangeEventHandler(this.dataMonitor_TileRowDeleted);
         this.dataMonitor.FramesetRowChanged += new SGDK2.ProjectDataset.FramesetRowChangeEventHandler(this.dataMonitor_FramesetRowChanged);
         this.dataMonitor.CounterRowDeleted += new SGDK2.ProjectDataset.CounterRowChangeEventHandler(this.dataMonitor_CounterRowDeleted);
         this.dataMonitor.TilesetRowDeleted += new SGDK2.ProjectDataset.TilesetRowChangeEventHandler(this.dataMonitor_TilesetRowDeleted);
         this.dataMonitor.FramesetRowDeleted += new SGDK2.ProjectDataset.FramesetRowChangeEventHandler(this.dataMonitor_FramesetRowDeleted);
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         this.dataMonitor.CounterRowChanged += new SGDK2.ProjectDataset.CounterRowChangeEventHandler(this.dataMonitor_CounterRowChanged);
         // 
         // StatusBar
         // 
         this.StatusBar.Location = new System.Drawing.Point(0, 403);
         this.StatusBar.Name = "StatusBar";
         this.StatusBar.Size = new System.Drawing.Size(496, 22);
         this.StatusBar.TabIndex = 5;
         // 
         // frmTileEdit
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(496, 425);
         this.Controls.Add(this.grpAvailableFrames);
         this.Controls.Add(this.splitter1);
         this.Controls.Add(this.grpTileProperties);
         this.Controls.Add(this.pnlTileHeader);
         this.Controls.Add(this.StatusBar);
         this.Menu = this.mnuTileset;
         this.MinimumSize = new System.Drawing.Size(432, 0);
         this.Name = "frmTileEdit";
         this.Text = "Tileset Editor";
         this.pnlTileHeader.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.nudTileHeight)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudTileWidth)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.updRepeatCount)).EndInit();
         this.grpTileProperties.ResumeLayout(false);
         this.pnlFrames.ResumeLayout(false);
         this.pnlFrameProperties.ResumeLayout(false);
         this.grpFrameProperties.ResumeLayout(false);
         this.pnlAddFrame.ResumeLayout(false);
         this.pnlTileProperties.ResumeLayout(false);
         this.grpAvailableFrames.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion

      #region Private methods
      private void FillFramesets()
      {
         cboFrameset.Items.Clear();
         foreach (DataRowView drv in ProjectData.Frameset.DefaultView)
         {
            ProjectDataset.FramesetRow fr = (ProjectDataset.FramesetRow)drv.Row;
            int nNewIndex = cboFrameset.Items.Add(fr);
            if (fr == m_Tileset.FramesetRow)
               cboFrameset.SelectedIndex = nNewIndex;
         }
      }

      private void FillCounters()
      {
         cboFrameCounter.Items.Clear();
         foreach (ProjectDataset.CounterRow cr in ProjectData.Counter.Rows)
            cboFrameCounter.Items.Add(cr);
      }

      private ProjectDataset.TileRow GetCurrentTile()
      {
         if ((m_Tile == null) || (m_Tile != cboMappedTiles.SelectedItem))
            throw new ApplicationException("Please select a Tile first");

         return m_Tile;
      }
      #endregion

      #region Public Static Members
      public static void Edit(Form MdiParent, ProjectDataset.TilesetRow EditRow)
      {
         foreach(Form frm in MdiParent.MdiChildren)
         {
            frmTileEdit f = frm as frmTileEdit;
            if (f != null)
            {
               if (f.m_Tileset == EditRow)
               {
                  f.Activate();
                  return;
               }
            }
         }

         frmTileEdit frmNew = new frmTileEdit(EditRow);
         frmNew.MdiParent = MdiParent;
         frmNew.Show();
      }
      #endregion

      #region Event Handlers
      private void TileFrames_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
      {
         GraphicBrowser gb = e.Data.GetData(typeof(GraphicBrowser)) as GraphicBrowser;
         if (gb == null)
            return;

         if (gb == TileFrames)
         {
            if (0 != (e.KeyState & 8))
               e.Effect = System.Windows.Forms.DragDropEffects.Copy;
            else
               e.Effect = System.Windows.Forms.DragDropEffects.Move;
         }
         else if (gb.Frameset == this.AvailableFrames.Frameset)
            e.Effect = System.Windows.Forms.DragDropEffects.Copy;

      }

      private void TileFrames_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
      {
         GraphicBrowser gb = e.Data.GetData(typeof(GraphicBrowser)) as GraphicBrowser;

         Point ptDrop = TileFrames.PointToClient(new Point(e.X,e.Y));
         short nCellIndex = (short)TileFrames.GetCellAtXY(ptDrop.X, ptDrop.Y,
            HitFlags.AllowExtraCell | HitFlags.GetNearest);

         try
         {
            ProjectDataset.TileRow tr =  GetCurrentTile();

            if (e.Effect == DragDropEffects.Move)
            {
               for (int idx = 0; idx < TileFrames.FramesToDisplay.Count; idx++)
               {
                  if (TileFrames.FramesToDisplay[idx].IsSelected)
                  {
                     short nNewIndex = nCellIndex;
                     if (nCellIndex > idx)
                        nNewIndex--;
                     ProjectData.MoveTileFrame((TileFrames.FramesToDisplay[idx] as
                        TileFrame).Row, nNewIndex);
                     IProvideFrame fra = TileFrames.FramesToDisplay[idx];
                     TileFrames.FramesToDisplay.RemoveAt(idx);
                     TileFrames.FramesToDisplay.Insert(nNewIndex, fra);
                     TileFrames.CurrentCellIndex = nNewIndex;
                  }
               }
            }
            else
            {
               foreach (SGDK2.ProjectDataset.FrameRow fr in gb.GetSelectedFrames())
               {
                  ProjectDataset.TileFrameRow tfr = SGDK2.ProjectData.InsertFrame(
                     tr, nCellIndex, fr.FrameValue, (short)updRepeatCount.Value);
                  TileFrames.FramesToDisplay.Insert(nCellIndex++, new TileFrame(tfr));
               }
            }
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(ex.Message, "Drag Tile Frames", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
         TileFrames.Invalidate();
      }

      private void cboFrameset_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         ProjectDataset.FramesetRow fr = (ProjectDataset.FramesetRow)cboFrameset.SelectedItem;
         if ((fr != m_Tileset.FramesetRow) && (m_Tileset.GetTileRows().Length > 0))
         {
            switch (MessageBox.Show(this, "The Frameset has been changed.  Do you want to delete all frames for this tileset?", "Confirm Frameset Change", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3))
            {
               case DialogResult.Yes:
                  foreach (ProjectDataset.TileRow tr in ProjectData.GetSortedTileRows(m_Tileset))
                     tr.Delete();
                  cboMappedTiles.Items.Clear();
                  break;
               case DialogResult.Cancel:
                  cboFrameset.SelectedItem = m_Tileset.FramesetRow;
                  return;
            }
         }
         AvailableFrames.Frameset = TileFrames.Frameset = m_Tileset.FramesetRow = fr;
         cboMappedTiles.Items.Clear();
         TileFrames.FramesToDisplay = new FrameList();
         foreach(ProjectDataset.TileRow tr in ProjectData.GetSortedTileRows(m_Tileset))
         {
            cboMappedTiles.Items.Add(tr);
            if (tr == m_Tile)
               cboMappedTiles.SelectedItem = tr;
         }
         if (cboMappedTiles.SelectedItem != m_Tile)
            m_Tile = null;
      }

      private void tbMappedTiles_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
      {
         try
         {
            if (e.Button == tbMappedTiles.Buttons[0])
            {
               int nNewVal = frmNewTileValue.PromptForNewTileValue(this);
               if (nNewVal < 0)
                  return;
               ProjectDataset.TileRow trNew = ProjectData.AddTileRow(m_Tileset, nNewVal, null);
               if (nNewVal < m_Tileset.FramesetRow.GetFrameRows().Length)
                  ProjectData.InsertFrame(trNew, 0, nNewVal, 1);
               cboMappedTiles.Items.Add(trNew);
               cboMappedTiles.SelectedItem = trNew;
            }
            else if (e.Button == tbMappedTiles.Buttons[1])
            {
                  ProjectDataset.TileRow tr = GetCurrentTile();
                  if (MessageBox.Show(this, "Delete tile " + tr.TileValue.ToString() + "?", "Confirm Delete Tile", MessageBoxButtons.YesNo,
                     MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                  {
                     cboMappedTiles.SelectedIndex = -1;
                     cboMappedTiles.Items.Remove(tr);
                     tr.Delete();
                  }
            }
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Tileset Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            ProjectData.RejectChanges();
         }
      }

      private void mnuDeleteTile_Click(object sender, System.EventArgs e)
      {
         try
         {
            ProjectDataset.TileRow tr = GetCurrentTile();
            if (MessageBox.Show(this, "Delete tile " + tr.TileValue.ToString() + "?", "Confirm Delete Tile", MessageBoxButtons.YesNo,
               MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
               cboMappedTiles.SelectedIndex = -1;
               cboMappedTiles.Items.Remove(tr);
               tr.Delete();
            }
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(ex.Message, "Delete Tile", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
      }

      private void cboMappedTiles_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (cboMappedTiles.SelectedIndex < 0)
         {
            m_Tile = null;
            tbbDeleteTile.Enabled = mnuDeleteTile.Enabled = true;
            mnuDeleteTile.Enabled = tbbDeleteTile.Enabled = false;
         }
         else
         {
            m_Tile = (ProjectDataset.TileRow)cboMappedTiles.SelectedItem;
            tbbDeleteTile.Enabled = mnuDeleteTile.Enabled = true;
         }
         TileFrames.Frameset = m_Tileset.FramesetRow;
         TileFrames.FramesToDisplay = new FrameList();
         if (m_Tile != null)
         {
            foreach(ProjectDataset.TileFrameRow tfr in ProjectData.GetSortedTileFrames(m_Tile))
               TileFrames.FramesToDisplay.Add(new TileFrame(tfr));
            if (m_Tile.IsCounterNull())
               cboFrameCounter.SelectedIndex = -1;
            else
               cboFrameCounter.SelectedItem = ProjectData.GetCounter(m_Tile.Counter);
         }
         TileFrames.Invalidate();
      }

      private void dataMonitor_FramesetRowChanged(object sender, SGDK2.ProjectDataset.FramesetRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
            cboFrameset.Items.Add(e.Row);
         else if (e.Action == DataRowAction.Change)
         {
            for (int nIdx=0; nIdx < cboFrameset.Items.Count; nIdx++)
               if (cboFrameset.Items[nIdx] == e.Row)
               {
                  // Force combo box to refresh the item
                  cboFrameset.Items[nIdx] = e.Row;
                  break;
               }
         }
      }

      private void dataMonitor_FramesetRowDeleted(object sender, SGDK2.ProjectDataset.FramesetRowChangeEvent e)
      {
         if ((e.Action == DataRowAction.Delete) && (cboFrameset.Items.Contains(e.Row)))
            cboFrameset.Items.Remove(e.Row);      
      }

      private void dataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void dataMonitor_TilesetRowDeleted(object sender, SGDK2.ProjectDataset.TilesetRowChangeEvent e)
      {
         if (e.Row == m_Tileset)
            this.Close();      
      }

      private void dataMonitor_CounterRowChanged(object sender, SGDK2.ProjectDataset.CounterRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
            cboFrameCounter.Items.Add(e.Row);
         else if (e.Action == DataRowAction.Change)
         {
            for (int nIdx=0; nIdx < cboFrameCounter.Items.Count; nIdx++)
               if (cboFrameCounter.Items[nIdx] == e.Row)
               {
                  // Force combo box to refresh the item
                  cboFrameCounter.Items[nIdx] = e.Row;
                  break;
               }
         }
      }

      private void dataMonitor_CounterRowDeleted(object sender, SGDK2.ProjectDataset.CounterRowChangeEvent e)
      {
         if ((e.Action == DataRowAction.Delete) && (cboFrameCounter.Items.Contains(e.Row)))
            cboFrameCounter.Items.Remove(e.Row);
      }

      private void dataMonitor_TileRowDeleted(object sender, SGDK2.ProjectDataset.TileRowChangeEvent e)
      {
         if (m_Tile == e.Row)
            cboMappedTiles.SelectedIndex = -1;
         if (cboMappedTiles.Items.Contains(e.Row))
            cboMappedTiles.Items.Remove(e.Row);
      }

      private void txtTilesetName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         string sValid = ProjectData.ValidateName(txtTilesetName.Text);

         if (sValid != null)
         {
            if (DialogResult.Cancel == MessageBox.Show(this, sValid, "Tileset Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtTilesetName.Text = m_Tileset.Name;
            e.Cancel = true;
         }
         ProjectDataset.TilesetRow tr = ProjectData.GetTileSet(txtTilesetName.Text);
         if ((null != tr) && (m_Tileset != tr))
         {
            if (DialogResult.Cancel == MessageBox.Show(this, txtTilesetName.Text + " already exists", "Tileset Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtTilesetName.Text = m_Tileset.Name;
            e.Cancel = true;
         }         
      }

      private void txtTilesetName_Validated(object sender, System.EventArgs e)
      {
         m_Tileset.Name = txtTilesetName.Text;
      }

      private void mnuNewTile_Click(object sender, System.EventArgs e)
      {
         int nNewVal = frmNewTileValue.PromptForNewTileValue(this);
         if (nNewVal < 0)
            return;
         ProjectDataset.TileRow trNew = ProjectData.AddTileRow(m_Tileset, nNewVal, null);
         if (nNewVal < m_Tileset.FramesetRow.GetFrameRows().Length)
         {
            ProjectData.InsertFrame(trNew, 0, nNewVal, 1);
         }
         cboMappedTiles.Items.Add(trNew);
         cboMappedTiles.SelectedItem = trNew;      
      }

      private void btnAddFrame_Click(object sender, System.EventArgs e)
      {
         try
         {
            ProjectDataset.TileRow tr =  GetCurrentTile();
            ProjectDataset.FrameRow[] Frames = AvailableFrames.GetSelectedFrames();
            short InsertPosition = (short)TileFrames.CurrentCellIndex;

            foreach(ProjectDataset.FrameRow fr in Frames)
            {
               InsertPosition++;
               TileFrames.FramesToDisplay.Insert(InsertPosition, new TileFrame(
                  ProjectData.InsertFrame(tr, InsertPosition, fr.FrameValue, (short)updRepeatCount.Value)));
            }
            TileFrames.CurrentCellIndex++;
            TileFrames.Invalidate();
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(ex.Message, "Add Frames", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
      }

      private void btnDeleteFrame_Click(object sender, System.EventArgs e)
      {
         if (DialogResult.Yes != MessageBox.Show(
            "Are you sure you want to delete " +
            TileFrames.GetSelectedCellCount().ToString() + " frames?",
            "Delete Tile Frames", MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2))
            return;

         FrameList DeleteFrames = new FrameList();

         for (int nIdx = 0; nIdx < TileFrames.FramesToDisplay.Count; nIdx++)
         {
            if (TileFrames.Selected[nIdx])
               DeleteFrames.Add((TileFrame)(TileFrames.FramesToDisplay[nIdx]));
         }

         foreach (TileFrame tf in DeleteFrames)
         {
            ProjectData.DeleteTileFrame(tf.Row);
            TileFrames.FramesToDisplay.Remove(tf);
         }

         TileFrames.Invalidate();
      }

      private void TileFrames_CurrentCellChanged(object sender, System.EventArgs e)
      {
         try
         {
            if (TileFrames.CurrentCellIndex >= 0)
               updRepeatCount.Value = ((TileFrame)(TileFrames.FramesToDisplay[TileFrames.CurrentCellIndex])).Row.Duration;
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(ex.Message, "Load Tile Frame Delay", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
      }

      private void updRepeatCount_ValueChanged(object sender, System.EventArgs e)
      {
         try
         {
            if (TileFrames.CurrentCellIndex >= 0)
            {
               TileFrame tf = (TileFrame)(TileFrames.FramesToDisplay[TileFrames.CurrentCellIndex]);
               if (tf.Row.Duration != (short)updRepeatCount.Value)
               {
                  tf.Row.Duration = (short)updRepeatCount.Value;
               }
            }
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(ex.Message, "Change Tile Frame Delay", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
      }

      private void cboFrameCounter_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         try
         {
            if (GetCurrentTile().CounterRow == cboFrameCounter.SelectedItem) return;
            if (cboFrameCounter.SelectedItem is ProjectDataset.CounterRow)
               GetCurrentTile().Counter = ((ProjectDataset.CounterRow)cboFrameCounter.SelectedItem).Name;
            else
               GetCurrentTile().SetCounterNull();
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(ex.Message, "Change Tile Counter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
      }

      private void nudControl_Validated(object sender, System.EventArgs e)
      {
         // Force control to pick up new value
         decimal dummy = ((NumericUpDown)sender).Value;
      }

      private void nudTileWidth_ValueChanged(object sender, System.EventArgs e)
      {
         m_Tileset.TileWidth = System.Convert.ToInt16(nudTileWidth.Value);
      }

      private void nudTileHeight_ValueChanged(object sender, System.EventArgs e)
      {
         m_Tileset.TileHeight = System.Convert.ToInt16(nudTileHeight.Value);
      }

      private void AvailableFrames_CurrentCellChanged(object sender, System.EventArgs e)
      {
         if (AvailableFrames.GetSelectedCellCount() == 1)
            StatusBar.Text = "Selected Frame Index in Available Frames: " + AvailableFrames.GetFirstSelectedCell().ToString();
         else
            StatusBar.Text = String.Empty;
      }
      #endregion

      class TileFrame : IProvideFrame
      {
         #region IProvideFrame Members

         private ProjectDataset.TileFrameRow row;
         private bool selected;

         public TileFrame(ProjectDataset.TileFrameRow row)
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

         public ProjectDataset.TileFrameRow Row
         {
            get
            {
               return row;
            }
         }
      }
   }
}
