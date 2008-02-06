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
	/// Summary description for MapEditor.
	/// </summary>
	public class frmMapEditor : System.Windows.Forms.Form
	{
      #region Non-Control Members
      private Layer[] m_Layers;
      private int m_nCurLayer;
      private TileCache m_TileCache;
      private SpriteCache m_SpriteCache;
      private FrameList m_TileProvider;
      private FrameList m_SpriteProvider;
      private Timer m_RefreshSpriteTimer = null;
      private Keys m_CurrentModifiers = Keys.None;
      private bool m_ReflectingSelection = false;
      private bool m_UpdatingList = false;
      private Point m_LayerMouseCoord = Point.Empty;
      private Point m_SnappedMouseCoord = Point.Empty;
      private Point m_DragStart = Point.Empty;
      private int m_DeletedKey = -1;
      private bool m_DangerWillRobinson = false;
      #endregion

      #region Form Designer Members
      private System.Windows.Forms.Splitter MapSplitter;
      private SGDK2.Display MapDisplay;
      private System.Windows.Forms.MainMenu mnuMapEditor;
      private System.ComponentModel.IContainer components;
      private SGDK2.DataChangeNotifier dataMonitor;
      private System.Windows.Forms.TabControl tabSelector;
      private System.Windows.Forms.TabPage tabTiles;
      private System.Windows.Forms.TabPage tabSprites;
      private System.Windows.Forms.Panel pnlTiles;
      private SGDK2.GraphicBrowser TileSelector;
      private System.Windows.Forms.ComboBox cboCategory;
      private System.Windows.Forms.PropertyGrid grdSprite;
      private System.Windows.Forms.ComboBox cboSpriteCategory;
      private SGDK2.GraphicBrowser SpriteSelector;
      private System.Windows.Forms.RadioButton rdoSelectSprites;
      private System.Windows.Forms.RadioButton rdoAddSprites;
      private System.Windows.Forms.StatusBar StatusBar;
      private System.Windows.Forms.StatusBarPanel sbpCtrl;
      private System.Windows.Forms.StatusBarPanel sbpStatus;
      private System.Windows.Forms.TabPage tabPlans;
      private System.Windows.Forms.ListBox lstAvailableSprites;
      private System.Windows.Forms.ListBox lstPlans;
      private System.Windows.Forms.Splitter PlanSplitter;
      private System.Windows.Forms.RadioButton rdoAppendCoord;
      private System.Windows.Forms.RadioButton rdoSelectCoord;
      private System.Windows.Forms.ImageList imlToolbar;
      private System.Windows.Forms.ToolBarButton tbbNewPlan;
      private System.Windows.Forms.ToolBarButton tbbDeletePlan;
      private System.Windows.Forms.Splitter CoordSplitter;
      private System.Windows.Forms.ListBox lstPlanCoords;
      private System.Windows.Forms.ToolBar tbPlans;
      private System.Windows.Forms.GroupBox grpPlanCoords;
      private System.Windows.Forms.GroupBox grpPlanList;
      private System.Windows.Forms.RadioButton rdoShowSelectedPlans;
      private System.Windows.Forms.RadioButton rdoShowAllPlans;
      private System.Windows.Forms.PropertyGrid grdPlan;
      private System.Windows.Forms.MenuItem mnuLayers;
      private System.Windows.Forms.MenuItem mnuEdit;
      private System.Windows.Forms.MenuItem mnuEditDelete;
      private System.Windows.Forms.StatusBarPanel sbpTileCoord;
      private System.Windows.Forms.StatusBarPanel sbpPixelCoord;
      private System.Windows.Forms.StatusBarPanel sbpTileAtCursor;
      private System.Windows.Forms.StatusBarPanel sbpSelTile;
      private System.Windows.Forms.MenuItem mnuSnapToTiles;
      private System.Windows.Forms.TabPage tabTools;
      private System.Windows.Forms.RadioButton rdoToolCopy;
      private System.Windows.Forms.RadioButton rdoToolPaste;
      private System.Windows.Forms.RadioButton rdoToolPasteTransparent;
      private System.Windows.Forms.ToolBarButton tbbPlanProperties;
      private System.Windows.Forms.ToolBarButton tbbSort;
      private System.Windows.Forms.ToolBarButton tbbGotoCoord;
      private System.Windows.Forms.MenuItem mnuEditDetails;
      private System.Windows.Forms.MenuItem mnuLocateCoordinate;
      private System.Windows.Forms.MenuItem mnuSortPlans;
      private System.Windows.Forms.ToolBar tbSprites;
      private System.Windows.Forms.ToolBarButton tbbDeleteSprite;
      private System.Windows.Forms.ToolBarButton tbbGotoSprite;
      private System.Windows.Forms.MenuItem mnuAddPlan;
      private System.Windows.Forms.MenuItem mnuView;
      private System.Windows.Forms.MenuItem mnuViewLayerEdges;
      private System.Windows.Forms.Splitter SpriteSplitter;
      #endregion

      #region Embedded classes
      private enum CursorMode
      {
         None,
         PlaceTile,
         PlaceSprite,
         SelectSprite,
         AddCoordinate,
         SelectCoordinate,
         Copy,
         Paste,
         PasteTransparent
      }
      #endregion

      #region Initialization and clean-up
		public frmMapEditor(ProjectDataset.LayerRow Layer)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         this.Text = "Edit Map Layer - " + Layer.Name;

         ProjectDataset.LayerRow[] lrs = ProjectData.GetSortedLayers(Layer.MapRow);
         m_Layers = new Layer[lrs.Length];
         for (int i=0; i<lrs.Length; i++)
         {
            m_Layers[i] = new Layer(lrs[i], MapDisplay);
            if (lrs[i] == Layer)
               m_nCurLayer = i;
            MenuItem mnuLyr = new MenuItem(lrs[i].Name, new EventHandler(LayerMenu_Click));
            mnuLyr.Checked = true;
            mnuLayers.MenuItems.Add(mnuLyr);
         }
         m_SpriteCache = new SpriteCache(MapDisplay);
         LoadCategories();
         LoadSpriteCategories();
         RefreshLayerSprites();
         PopulateAvailableSprites();
         PopulatePlans();

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"MapEditor.html");
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
         if (m_RefreshSpriteTimer != null)
         {
            m_RefreshSpriteTimer.Dispose();
            m_RefreshSpriteTimer = null;
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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmMapEditor));
         this.MapSplitter = new System.Windows.Forms.Splitter();
         this.MapDisplay = new SGDK2.Display();
         this.mnuMapEditor = new System.Windows.Forms.MainMenu();
         this.mnuEdit = new System.Windows.Forms.MenuItem();
         this.mnuEditDelete = new System.Windows.Forms.MenuItem();
         this.mnuSnapToTiles = new System.Windows.Forms.MenuItem();
         this.mnuAddPlan = new System.Windows.Forms.MenuItem();
         this.mnuEditDetails = new System.Windows.Forms.MenuItem();
         this.mnuLocateCoordinate = new System.Windows.Forms.MenuItem();
         this.mnuSortPlans = new System.Windows.Forms.MenuItem();
         this.mnuView = new System.Windows.Forms.MenuItem();
         this.mnuViewLayerEdges = new System.Windows.Forms.MenuItem();
         this.mnuLayers = new System.Windows.Forms.MenuItem();
         this.dataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.tabSelector = new System.Windows.Forms.TabControl();
         this.tabTiles = new System.Windows.Forms.TabPage();
         this.pnlTiles = new System.Windows.Forms.Panel();
         this.TileSelector = new SGDK2.GraphicBrowser();
         this.cboCategory = new System.Windows.Forms.ComboBox();
         this.tabSprites = new System.Windows.Forms.TabPage();
         this.lstAvailableSprites = new System.Windows.Forms.ListBox();
         this.SpriteSelector = new SGDK2.GraphicBrowser();
         this.SpriteSplitter = new System.Windows.Forms.Splitter();
         this.cboSpriteCategory = new System.Windows.Forms.ComboBox();
         this.grdSprite = new System.Windows.Forms.PropertyGrid();
         this.rdoAddSprites = new System.Windows.Forms.RadioButton();
         this.rdoSelectSprites = new System.Windows.Forms.RadioButton();
         this.tbSprites = new System.Windows.Forms.ToolBar();
         this.tbbDeleteSprite = new System.Windows.Forms.ToolBarButton();
         this.tbbGotoSprite = new System.Windows.Forms.ToolBarButton();
         this.imlToolbar = new System.Windows.Forms.ImageList(this.components);
         this.tabPlans = new System.Windows.Forms.TabPage();
         this.grpPlanList = new System.Windows.Forms.GroupBox();
         this.lstPlans = new System.Windows.Forms.ListBox();
         this.rdoShowAllPlans = new System.Windows.Forms.RadioButton();
         this.rdoShowSelectedPlans = new System.Windows.Forms.RadioButton();
         this.tbPlans = new System.Windows.Forms.ToolBar();
         this.tbbNewPlan = new System.Windows.Forms.ToolBarButton();
         this.tbbDeletePlan = new System.Windows.Forms.ToolBarButton();
         this.tbbPlanProperties = new System.Windows.Forms.ToolBarButton();
         this.tbbSort = new System.Windows.Forms.ToolBarButton();
         this.tbbGotoCoord = new System.Windows.Forms.ToolBarButton();
         this.CoordSplitter = new System.Windows.Forms.Splitter();
         this.grpPlanCoords = new System.Windows.Forms.GroupBox();
         this.lstPlanCoords = new System.Windows.Forms.ListBox();
         this.rdoAppendCoord = new System.Windows.Forms.RadioButton();
         this.rdoSelectCoord = new System.Windows.Forms.RadioButton();
         this.PlanSplitter = new System.Windows.Forms.Splitter();
         this.grdPlan = new System.Windows.Forms.PropertyGrid();
         this.tabTools = new System.Windows.Forms.TabPage();
         this.rdoToolPasteTransparent = new System.Windows.Forms.RadioButton();
         this.rdoToolPaste = new System.Windows.Forms.RadioButton();
         this.rdoToolCopy = new System.Windows.Forms.RadioButton();
         this.StatusBar = new System.Windows.Forms.StatusBar();
         this.sbpStatus = new System.Windows.Forms.StatusBarPanel();
         this.sbpCtrl = new System.Windows.Forms.StatusBarPanel();
         this.sbpTileCoord = new System.Windows.Forms.StatusBarPanel();
         this.sbpPixelCoord = new System.Windows.Forms.StatusBarPanel();
         this.sbpTileAtCursor = new System.Windows.Forms.StatusBarPanel();
         this.sbpSelTile = new System.Windows.Forms.StatusBarPanel();
         this.tabSelector.SuspendLayout();
         this.tabTiles.SuspendLayout();
         this.pnlTiles.SuspendLayout();
         this.tabSprites.SuspendLayout();
         this.tabPlans.SuspendLayout();
         this.grpPlanList.SuspendLayout();
         this.grpPlanCoords.SuspendLayout();
         this.tabTools.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.sbpStatus)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpCtrl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpTileCoord)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpPixelCoord)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpTileAtCursor)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpSelTile)).BeginInit();
         this.SuspendLayout();
         // 
         // MapSplitter
         // 
         this.MapSplitter.Location = new System.Drawing.Point(176, 0);
         this.MapSplitter.Name = "MapSplitter";
         this.MapSplitter.Size = new System.Drawing.Size(5, 505);
         this.MapSplitter.TabIndex = 8;
         this.MapSplitter.TabStop = false;
         // 
         // MapDisplay
         // 
         this.MapDisplay.AutoScroll = true;
         this.MapDisplay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.MapDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
         this.MapDisplay.GameDisplayMode = SGDK2.GameDisplayMode.m640x480x24;
         this.MapDisplay.Location = new System.Drawing.Point(181, 0);
         this.MapDisplay.Name = "MapDisplay";
         this.MapDisplay.Size = new System.Drawing.Size(483, 505);
         this.MapDisplay.TabIndex = 10;
         this.MapDisplay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MapDisplay_MouseUp);
         this.MapDisplay.Paint += new System.Windows.Forms.PaintEventHandler(this.MapDisplay_Paint);
         this.MapDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MapDisplay_MouseMove);
         this.MapDisplay.MouseLeave += new System.EventHandler(this.MapDisplay_MouseLeave);
         this.MapDisplay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MapDisplay_MouseDown);
         // 
         // mnuMapEditor
         // 
         this.mnuMapEditor.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                     this.mnuEdit,
                                                                                     this.mnuView,
                                                                                     this.mnuLayers});
         // 
         // mnuEdit
         // 
         this.mnuEdit.Index = 0;
         this.mnuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                this.mnuEditDelete,
                                                                                this.mnuSnapToTiles,
                                                                                this.mnuAddPlan,
                                                                                this.mnuEditDetails,
                                                                                this.mnuLocateCoordinate,
                                                                                this.mnuSortPlans});
         this.mnuEdit.Text = "&Edit";
         // 
         // mnuEditDelete
         // 
         this.mnuEditDelete.Index = 0;
         this.mnuEditDelete.Shortcut = System.Windows.Forms.Shortcut.Del;
         this.mnuEditDelete.Text = "&Delete Selected Object";
         this.mnuEditDelete.Click += new System.EventHandler(this.mnuEditDelete_Click);
         // 
         // mnuSnapToTiles
         // 
         this.mnuSnapToTiles.Index = 1;
         this.mnuSnapToTiles.Shortcut = System.Windows.Forms.Shortcut.CtrlT;
         this.mnuSnapToTiles.Text = "&Snap to Tiles";
         this.mnuSnapToTiles.Click += new System.EventHandler(this.mnuSnapToTiles_Click);
         // 
         // mnuAddPlan
         // 
         this.mnuAddPlan.Index = 2;
         this.mnuAddPlan.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
         this.mnuAddPlan.Text = "&Add Plan";
         this.mnuAddPlan.Click += new System.EventHandler(this.mnuAddPlan_Click);
         // 
         // mnuEditDetails
         // 
         this.mnuEditDetails.Index = 3;
         this.mnuEditDetails.Text = "Selected &Plan Details";
         this.mnuEditDetails.Click += new System.EventHandler(this.mnuEditDetails_Click);
         // 
         // mnuLocateCoordinate
         // 
         this.mnuLocateCoordinate.Index = 4;
         this.mnuLocateCoordinate.Shortcut = System.Windows.Forms.Shortcut.CtrlG;
         this.mnuLocateCoordinate.Text = "&Locate Selected Object";
         this.mnuLocateCoordinate.Click += new System.EventHandler(this.mnuLocateCoordinate_Click);
         // 
         // mnuSortPlans
         // 
         this.mnuSortPlans.Index = 5;
         this.mnuSortPlans.Text = "So&rt Plans";
         this.mnuSortPlans.Click += new System.EventHandler(this.mnuSortPlans_Click);
         // 
         // mnuView
         // 
         this.mnuView.Index = 1;
         this.mnuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                this.mnuViewLayerEdges});
         this.mnuView.MergeOrder = 1;
         this.mnuView.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
         this.mnuView.Text = "&View";
         // 
         // mnuViewLayerEdges
         // 
         this.mnuViewLayerEdges.Index = 0;
         this.mnuViewLayerEdges.Text = "&Layer Edges";
         this.mnuViewLayerEdges.Click += new System.EventHandler(this.mnuViewLayerEdges_Click);
         // 
         // mnuLayers
         // 
         this.mnuLayers.Index = 2;
         this.mnuLayers.MergeOrder = 2;
         this.mnuLayers.Text = "&Layers";
         // 
         // dataMonitor
         // 
         this.dataMonitor.TileFrameRowDeleting += new SGDK2.ProjectDataset.TileFrameRowChangeEventHandler(this.dataMonitor_TileFrameRowChanged);
         this.dataMonitor.SpritePlanRowDeleted += new SGDK2.ProjectDataset.SpritePlanRowChangeEventHandler(this.dataMonitor_SpritePlanRowDeleted);
         this.dataMonitor.LayerRowChanged += new SGDK2.ProjectDataset.LayerRowChangeEventHandler(this.dataMonitor_LayerRowChanged);
         this.dataMonitor.SpritePlanRowChanged += new SGDK2.ProjectDataset.SpritePlanRowChangeEventHandler(this.dataMonitor_SpritePlanRowChanged);
         this.dataMonitor.TileFrameRowChanged += new SGDK2.ProjectDataset.TileFrameRowChangeEventHandler(this.dataMonitor_TileFrameRowChanged);
         this.dataMonitor.SpritePlanRowChanging += new SGDK2.ProjectDataset.SpritePlanRowChangeEventHandler(this.dataMonitor_SpritePlanRowChanging);
         this.dataMonitor.LayerRowDeleted += new SGDK2.ProjectDataset.LayerRowChangeEventHandler(this.dataMonitor_LayerRowDeleted);
         this.dataMonitor.TileRowChanged += new SGDK2.ProjectDataset.TileRowChangeEventHandler(this.dataMonitor_TileRowChanged);
         this.dataMonitor.TileRowDeleting += new SGDK2.ProjectDataset.TileRowChangeEventHandler(this.dataMonitor_TileRowChanged);
         this.dataMonitor.SpriteRowChanging += new SGDK2.ProjectDataset.SpriteRowChangeEventHandler(this.dataMonitor_SpriteRowChanging);
         this.dataMonitor.TileFrameRowDeleted += new SGDK2.ProjectDataset.TileFrameRowChangeEventHandler(this.dataMonitor_TileFrameRowChanged);
         this.dataMonitor.MapRowDeleted += new SGDK2.ProjectDataset.MapRowChangeEventHandler(this.dataMonitor_MapRowDeleted);
         this.dataMonitor.SpriteRowChanged += new SGDK2.ProjectDataset.SpriteRowChangeEventHandler(this.dataMonitor_SpriteRowChanged);
         this.dataMonitor.SpriteRowDeleting += new SGDK2.ProjectDataset.SpriteRowChangeEventHandler(this.dataMonitor_SpriteRowDeleting);
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         // 
         // tabSelector
         // 
         this.tabSelector.Controls.Add(this.tabTiles);
         this.tabSelector.Controls.Add(this.tabSprites);
         this.tabSelector.Controls.Add(this.tabPlans);
         this.tabSelector.Controls.Add(this.tabTools);
         this.tabSelector.Dock = System.Windows.Forms.DockStyle.Left;
         this.tabSelector.Location = new System.Drawing.Point(0, 0);
         this.tabSelector.Name = "tabSelector";
         this.tabSelector.SelectedIndex = 0;
         this.tabSelector.Size = new System.Drawing.Size(176, 505);
         this.tabSelector.TabIndex = 13;
         this.tabSelector.SelectedIndexChanged += new System.EventHandler(this.tabSelector_SelectedIndexChanged);
         // 
         // tabTiles
         // 
         this.tabTiles.Controls.Add(this.pnlTiles);
         this.tabTiles.Location = new System.Drawing.Point(4, 22);
         this.tabTiles.Name = "tabTiles";
         this.tabTiles.Size = new System.Drawing.Size(168, 479);
         this.tabTiles.TabIndex = 0;
         this.tabTiles.Text = "Tiles";
         // 
         // pnlTiles
         // 
         this.pnlTiles.Controls.Add(this.TileSelector);
         this.pnlTiles.Controls.Add(this.cboCategory);
         this.pnlTiles.Dock = System.Windows.Forms.DockStyle.Fill;
         this.pnlTiles.Location = new System.Drawing.Point(0, 0);
         this.pnlTiles.Name = "pnlTiles";
         this.pnlTiles.Size = new System.Drawing.Size(168, 479);
         this.pnlTiles.TabIndex = 14;
         // 
         // TileSelector
         // 
         this.TileSelector.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.TileSelector.CellBorders = false;
         this.TileSelector.CellPadding = new System.Drawing.Size(2, 2);
         this.TileSelector.CellSize = new System.Drawing.Size(0, 0);
         this.TileSelector.CurrentCellIndex = -1;
         this.TileSelector.Dock = System.Windows.Forms.DockStyle.Fill;
         this.TileSelector.Frameset = null;
         this.TileSelector.FramesToDisplay = null;
         this.TileSelector.GraphicSheet = null;
         this.TileSelector.Location = new System.Drawing.Point(0, 21);
         this.TileSelector.Name = "TileSelector";
         this.TileSelector.SheetImage = null;
         this.TileSelector.Size = new System.Drawing.Size(168, 458);
         this.TileSelector.TabIndex = 9;
         this.TileSelector.CurrentCellChanged += new System.EventHandler(this.TileSelector_CurrentCellChanged);
         // 
         // cboCategory
         // 
         this.cboCategory.Dock = System.Windows.Forms.DockStyle.Top;
         this.cboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboCategory.Location = new System.Drawing.Point(0, 0);
         this.cboCategory.Name = "cboCategory";
         this.cboCategory.Size = new System.Drawing.Size(168, 21);
         this.cboCategory.TabIndex = 11;
         this.cboCategory.SelectedIndexChanged += new System.EventHandler(this.cboCategory_SelectedIndexChanged);
         // 
         // tabSprites
         // 
         this.tabSprites.Controls.Add(this.lstAvailableSprites);
         this.tabSprites.Controls.Add(this.SpriteSelector);
         this.tabSprites.Controls.Add(this.SpriteSplitter);
         this.tabSprites.Controls.Add(this.cboSpriteCategory);
         this.tabSprites.Controls.Add(this.grdSprite);
         this.tabSprites.Controls.Add(this.rdoAddSprites);
         this.tabSprites.Controls.Add(this.rdoSelectSprites);
         this.tabSprites.Controls.Add(this.tbSprites);
         this.tabSprites.Location = new System.Drawing.Point(4, 22);
         this.tabSprites.Name = "tabSprites";
         this.tabSprites.Size = new System.Drawing.Size(168, 479);
         this.tabSprites.TabIndex = 1;
         this.tabSprites.Text = "Sprites";
         // 
         // lstAvailableSprites
         // 
         this.lstAvailableSprites.DisplayMember = "Name";
         this.lstAvailableSprites.Dock = System.Windows.Forms.DockStyle.Fill;
         this.lstAvailableSprites.IntegralHeight = false;
         this.lstAvailableSprites.Location = new System.Drawing.Point(0, 78);
         this.lstAvailableSprites.Name = "lstAvailableSprites";
         this.lstAvailableSprites.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
         this.lstAvailableSprites.Size = new System.Drawing.Size(168, 252);
         this.lstAvailableSprites.TabIndex = 6;
         this.lstAvailableSprites.DoubleClick += new System.EventHandler(this.lstAvailableSprites_DoubleClick);
         this.lstAvailableSprites.Enter += new System.EventHandler(this.lstAvailableSprites_Enter);
         this.lstAvailableSprites.SelectedIndexChanged += new System.EventHandler(this.lstAvailableSprites_SelectedIndexChanged);
         // 
         // SpriteSelector
         // 
         this.SpriteSelector.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.SpriteSelector.CellBorders = false;
         this.SpriteSelector.CellPadding = new System.Drawing.Size(0, 0);
         this.SpriteSelector.CellSize = new System.Drawing.Size(0, 0);
         this.SpriteSelector.CurrentCellIndex = -1;
         this.SpriteSelector.Dock = System.Windows.Forms.DockStyle.Fill;
         this.SpriteSelector.Frameset = null;
         this.SpriteSelector.FramesToDisplay = null;
         this.SpriteSelector.GraphicSheet = null;
         this.SpriteSelector.Location = new System.Drawing.Point(0, 78);
         this.SpriteSelector.Name = "SpriteSelector";
         this.SpriteSelector.SheetImage = null;
         this.SpriteSelector.Size = new System.Drawing.Size(168, 252);
         this.SpriteSelector.TabIndex = 2;
         this.SpriteSelector.Visible = false;
         this.SpriteSelector.CurrentCellChanged += new System.EventHandler(this.SpriteSelector_CurrentCellChanged);
         // 
         // SpriteSplitter
         // 
         this.SpriteSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.SpriteSplitter.Location = new System.Drawing.Point(0, 330);
         this.SpriteSplitter.Name = "SpriteSplitter";
         this.SpriteSplitter.Size = new System.Drawing.Size(168, 5);
         this.SpriteSplitter.TabIndex = 3;
         this.SpriteSplitter.TabStop = false;
         // 
         // cboSpriteCategory
         // 
         this.cboSpriteCategory.Dock = System.Windows.Forms.DockStyle.Top;
         this.cboSpriteCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboSpriteCategory.Location = new System.Drawing.Point(0, 57);
         this.cboSpriteCategory.Name = "cboSpriteCategory";
         this.cboSpriteCategory.Size = new System.Drawing.Size(168, 21);
         this.cboSpriteCategory.TabIndex = 1;
         this.cboSpriteCategory.Visible = false;
         this.cboSpriteCategory.SelectedIndexChanged += new System.EventHandler(this.cboSpriteCategory_SelectedIndexChanged);
         // 
         // grdSprite
         // 
         this.grdSprite.CommandsVisibleIfAvailable = true;
         this.grdSprite.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.grdSprite.LargeButtons = false;
         this.grdSprite.LineColor = System.Drawing.SystemColors.ScrollBar;
         this.grdSprite.Location = new System.Drawing.Point(0, 335);
         this.grdSprite.Name = "grdSprite";
         this.grdSprite.Size = new System.Drawing.Size(168, 200);
         this.grdSprite.TabIndex = 0;
         this.grdSprite.Text = "PropertyGrid";
         this.grdSprite.ToolbarVisible = false;
         this.grdSprite.ViewBackColor = System.Drawing.SystemColors.Window;
         this.grdSprite.ViewForeColor = System.Drawing.SystemColors.WindowText;
         this.grdSprite.Enter += new System.EventHandler(this.grdSprite_Enter);
         this.grdSprite.Leave += new System.EventHandler(this.grdSprite_Leave);
         // 
         // rdoAddSprites
         // 
         this.rdoAddSprites.Dock = System.Windows.Forms.DockStyle.Top;
         this.rdoAddSprites.Location = new System.Drawing.Point(0, 41);
         this.rdoAddSprites.Name = "rdoAddSprites";
         this.rdoAddSprites.Size = new System.Drawing.Size(168, 16);
         this.rdoAddSprites.TabIndex = 5;
         this.rdoAddSprites.Text = "Add Sprites";
         this.rdoAddSprites.CheckedChanged += new System.EventHandler(this.rdoSpriteMode_CheckedChanged);
         // 
         // rdoSelectSprites
         // 
         this.rdoSelectSprites.Checked = true;
         this.rdoSelectSprites.Dock = System.Windows.Forms.DockStyle.Top;
         this.rdoSelectSprites.Location = new System.Drawing.Point(0, 25);
         this.rdoSelectSprites.Name = "rdoSelectSprites";
         this.rdoSelectSprites.Size = new System.Drawing.Size(168, 16);
         this.rdoSelectSprites.TabIndex = 4;
         this.rdoSelectSprites.TabStop = true;
         this.rdoSelectSprites.Text = "Select Sprites";
         this.rdoSelectSprites.CheckedChanged += new System.EventHandler(this.rdoSpriteMode_CheckedChanged);
         // 
         // tbSprites
         // 
         this.tbSprites.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                                     this.tbbDeleteSprite,
                                                                                     this.tbbGotoSprite});
         this.tbSprites.ButtonSize = new System.Drawing.Size(22, 21);
         this.tbSprites.Divider = false;
         this.tbSprites.DropDownArrows = true;
         this.tbSprites.ImageList = this.imlToolbar;
         this.tbSprites.Location = new System.Drawing.Point(0, 0);
         this.tbSprites.Name = "tbSprites";
         this.tbSprites.ShowToolTips = true;
         this.tbSprites.Size = new System.Drawing.Size(168, 25);
         this.tbSprites.TabIndex = 7;
         this.tbSprites.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbSprites_ButtonClick);
         // 
         // tbbDeleteSprite
         // 
         this.tbbDeleteSprite.ImageIndex = 1;
         this.tbbDeleteSprite.ToolTipText = "Delete selected sprite";
         // 
         // tbbGotoSprite
         // 
         this.tbbGotoSprite.ImageIndex = 4;
         this.tbbGotoSprite.ToolTipText = "Locate the selected sprite in the map";
         // 
         // imlToolbar
         // 
         this.imlToolbar.ImageSize = new System.Drawing.Size(15, 15);
         this.imlToolbar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlToolbar.ImageStream")));
         this.imlToolbar.TransparentColor = System.Drawing.Color.Magenta;
         // 
         // tabPlans
         // 
         this.tabPlans.Controls.Add(this.grpPlanList);
         this.tabPlans.Controls.Add(this.CoordSplitter);
         this.tabPlans.Controls.Add(this.grpPlanCoords);
         this.tabPlans.Controls.Add(this.PlanSplitter);
         this.tabPlans.Controls.Add(this.grdPlan);
         this.tabPlans.Location = new System.Drawing.Point(4, 22);
         this.tabPlans.Name = "tabPlans";
         this.tabPlans.Size = new System.Drawing.Size(168, 479);
         this.tabPlans.TabIndex = 2;
         this.tabPlans.Text = "Plans";
         // 
         // grpPlanList
         // 
         this.grpPlanList.Controls.Add(this.lstPlans);
         this.grpPlanList.Controls.Add(this.rdoShowAllPlans);
         this.grpPlanList.Controls.Add(this.rdoShowSelectedPlans);
         this.grpPlanList.Controls.Add(this.tbPlans);
         this.grpPlanList.Dock = System.Windows.Forms.DockStyle.Fill;
         this.grpPlanList.Location = new System.Drawing.Point(0, 0);
         this.grpPlanList.Name = "grpPlanList";
         this.grpPlanList.Size = new System.Drawing.Size(168, 197);
         this.grpPlanList.TabIndex = 22;
         this.grpPlanList.TabStop = false;
         this.grpPlanList.Text = "Plans";
         // 
         // lstPlans
         // 
         this.lstPlans.DisplayMember = "Name";
         this.lstPlans.Dock = System.Windows.Forms.DockStyle.Fill;
         this.lstPlans.IntegralHeight = false;
         this.lstPlans.Location = new System.Drawing.Point(3, 41);
         this.lstPlans.Name = "lstPlans";
         this.lstPlans.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
         this.lstPlans.Size = new System.Drawing.Size(162, 121);
         this.lstPlans.TabIndex = 0;
         this.lstPlans.DoubleClick += new System.EventHandler(this.lstPlans_DoubleClick);
         this.lstPlans.Leave += new System.EventHandler(this.lstPlans_Leave);
         this.lstPlans.Enter += new System.EventHandler(this.lstPlans_Enter);
         this.lstPlans.SelectedIndexChanged += new System.EventHandler(this.lstPlans_SelectedIndexChanged);
         // 
         // rdoShowAllPlans
         // 
         this.rdoShowAllPlans.Checked = true;
         this.rdoShowAllPlans.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.rdoShowAllPlans.Location = new System.Drawing.Point(3, 162);
         this.rdoShowAllPlans.Name = "rdoShowAllPlans";
         this.rdoShowAllPlans.Size = new System.Drawing.Size(162, 16);
         this.rdoShowAllPlans.TabIndex = 18;
         this.rdoShowAllPlans.TabStop = true;
         this.rdoShowAllPlans.Text = "Show All";
         this.rdoShowAllPlans.CheckedChanged += new System.EventHandler(this.rdoShowPlans_CheckedChanged);
         // 
         // rdoShowSelectedPlans
         // 
         this.rdoShowSelectedPlans.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.rdoShowSelectedPlans.Location = new System.Drawing.Point(3, 178);
         this.rdoShowSelectedPlans.Name = "rdoShowSelectedPlans";
         this.rdoShowSelectedPlans.Size = new System.Drawing.Size(162, 16);
         this.rdoShowSelectedPlans.TabIndex = 17;
         this.rdoShowSelectedPlans.Text = "Show Selected";
         this.rdoShowSelectedPlans.CheckedChanged += new System.EventHandler(this.rdoShowPlans_CheckedChanged);
         // 
         // tbPlans
         // 
         this.tbPlans.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                                   this.tbbNewPlan,
                                                                                   this.tbbDeletePlan,
                                                                                   this.tbbPlanProperties,
                                                                                   this.tbbSort,
                                                                                   this.tbbGotoCoord});
         this.tbPlans.Divider = false;
         this.tbPlans.DropDownArrows = true;
         this.tbPlans.ImageList = this.imlToolbar;
         this.tbPlans.Location = new System.Drawing.Point(3, 16);
         this.tbPlans.Name = "tbPlans";
         this.tbPlans.ShowToolTips = true;
         this.tbPlans.Size = new System.Drawing.Size(162, 25);
         this.tbPlans.TabIndex = 16;
         this.tbPlans.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbPlans_ButtonClick);
         // 
         // tbbNewPlan
         // 
         this.tbbNewPlan.ImageIndex = 0;
         this.tbbNewPlan.ToolTipText = "Add a New Plan";
         // 
         // tbbDeletePlan
         // 
         this.tbbDeletePlan.ImageIndex = 1;
         this.tbbDeletePlan.ToolTipText = "Delete Selected Plan";
         // 
         // tbbPlanProperties
         // 
         this.tbbPlanProperties.ImageIndex = 2;
         this.tbbPlanProperties.ToolTipText = "Edit Selected Plan";
         // 
         // tbbSort
         // 
         this.tbbSort.ImageIndex = 3;
         this.tbbSort.ToolTipText = "Sort plans alphabetically";
         // 
         // tbbGotoCoord
         // 
         this.tbbGotoCoord.ImageIndex = 4;
         this.tbbGotoCoord.ToolTipText = "Locate the selected plan coordinate on the map";
         // 
         // CoordSplitter
         // 
         this.CoordSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.CoordSplitter.Location = new System.Drawing.Point(0, 197);
         this.CoordSplitter.MinExtra = 93;
         this.CoordSplitter.MinSize = 72;
         this.CoordSplitter.Name = "CoordSplitter";
         this.CoordSplitter.Size = new System.Drawing.Size(168, 5);
         this.CoordSplitter.TabIndex = 20;
         this.CoordSplitter.TabStop = false;
         // 
         // grpPlanCoords
         // 
         this.grpPlanCoords.Controls.Add(this.lstPlanCoords);
         this.grpPlanCoords.Controls.Add(this.rdoAppendCoord);
         this.grpPlanCoords.Controls.Add(this.rdoSelectCoord);
         this.grpPlanCoords.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.grpPlanCoords.Location = new System.Drawing.Point(0, 202);
         this.grpPlanCoords.Name = "grpPlanCoords";
         this.grpPlanCoords.Size = new System.Drawing.Size(168, 120);
         this.grpPlanCoords.TabIndex = 21;
         this.grpPlanCoords.TabStop = false;
         this.grpPlanCoords.Text = "Plan Coordinates";
         // 
         // lstPlanCoords
         // 
         this.lstPlanCoords.Dock = System.Windows.Forms.DockStyle.Fill;
         this.lstPlanCoords.IntegralHeight = false;
         this.lstPlanCoords.Location = new System.Drawing.Point(3, 16);
         this.lstPlanCoords.Name = "lstPlanCoords";
         this.lstPlanCoords.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
         this.lstPlanCoords.Size = new System.Drawing.Size(162, 69);
         this.lstPlanCoords.TabIndex = 17;
         this.lstPlanCoords.DoubleClick += new System.EventHandler(this.lstPlanCoords_DoubleClick);
         this.lstPlanCoords.Leave += new System.EventHandler(this.lstPlanCoords_Leave);
         this.lstPlanCoords.Enter += new System.EventHandler(this.lstPlanCoords_Enter);
         this.lstPlanCoords.SelectedIndexChanged += new System.EventHandler(this.lstPlanCoords_SelectedIndexChanged);
         // 
         // rdoAppendCoord
         // 
         this.rdoAppendCoord.Checked = true;
         this.rdoAppendCoord.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.rdoAppendCoord.Location = new System.Drawing.Point(3, 85);
         this.rdoAppendCoord.Name = "rdoAppendCoord";
         this.rdoAppendCoord.Size = new System.Drawing.Size(162, 16);
         this.rdoAppendCoord.TabIndex = 7;
         this.rdoAppendCoord.TabStop = true;
         this.rdoAppendCoord.Text = "Append Coordinates";
         // 
         // rdoSelectCoord
         // 
         this.rdoSelectCoord.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.rdoSelectCoord.Location = new System.Drawing.Point(3, 101);
         this.rdoSelectCoord.Name = "rdoSelectCoord";
         this.rdoSelectCoord.Size = new System.Drawing.Size(162, 16);
         this.rdoSelectCoord.TabIndex = 8;
         this.rdoSelectCoord.Text = "Select Coordinates";
         // 
         // PlanSplitter
         // 
         this.PlanSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.PlanSplitter.Location = new System.Drawing.Point(0, 322);
         this.PlanSplitter.Name = "PlanSplitter";
         this.PlanSplitter.Size = new System.Drawing.Size(168, 5);
         this.PlanSplitter.TabIndex = 1;
         this.PlanSplitter.TabStop = false;
         // 
         // grdPlan
         // 
         this.grdPlan.CommandsVisibleIfAvailable = true;
         this.grdPlan.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.grdPlan.LargeButtons = false;
         this.grdPlan.LineColor = System.Drawing.SystemColors.ScrollBar;
         this.grdPlan.Location = new System.Drawing.Point(0, 327);
         this.grdPlan.Name = "grdPlan";
         this.grdPlan.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
         this.grdPlan.Size = new System.Drawing.Size(168, 152);
         this.grdPlan.TabIndex = 6;
         this.grdPlan.Text = "PropertyGrid";
         this.grdPlan.ToolbarVisible = false;
         this.grdPlan.ViewBackColor = System.Drawing.SystemColors.Window;
         this.grdPlan.ViewForeColor = System.Drawing.SystemColors.WindowText;
         this.grdPlan.Enter += new System.EventHandler(this.grdPlan_Enter);
         this.grdPlan.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.grdPlan_PropertyValueChanged);
         // 
         // tabTools
         // 
         this.tabTools.Controls.Add(this.rdoToolPasteTransparent);
         this.tabTools.Controls.Add(this.rdoToolPaste);
         this.tabTools.Controls.Add(this.rdoToolCopy);
         this.tabTools.Location = new System.Drawing.Point(4, 22);
         this.tabTools.Name = "tabTools";
         this.tabTools.Size = new System.Drawing.Size(168, 479);
         this.tabTools.TabIndex = 3;
         this.tabTools.Text = "Tools";
         // 
         // rdoToolPasteTransparent
         // 
         this.rdoToolPasteTransparent.Appearance = System.Windows.Forms.Appearance.Button;
         this.rdoToolPasteTransparent.Dock = System.Windows.Forms.DockStyle.Top;
         this.rdoToolPasteTransparent.Location = new System.Drawing.Point(0, 48);
         this.rdoToolPasteTransparent.Name = "rdoToolPasteTransparent";
         this.rdoToolPasteTransparent.Size = new System.Drawing.Size(168, 24);
         this.rdoToolPasteTransparent.TabIndex = 2;
         this.rdoToolPasteTransparent.Text = "Paste Transparent";
         this.rdoToolPasteTransparent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // rdoToolPaste
         // 
         this.rdoToolPaste.Appearance = System.Windows.Forms.Appearance.Button;
         this.rdoToolPaste.Dock = System.Windows.Forms.DockStyle.Top;
         this.rdoToolPaste.Location = new System.Drawing.Point(0, 24);
         this.rdoToolPaste.Name = "rdoToolPaste";
         this.rdoToolPaste.Size = new System.Drawing.Size(168, 24);
         this.rdoToolPaste.TabIndex = 1;
         this.rdoToolPaste.Text = "Paste";
         this.rdoToolPaste.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // rdoToolCopy
         // 
         this.rdoToolCopy.Appearance = System.Windows.Forms.Appearance.Button;
         this.rdoToolCopy.Dock = System.Windows.Forms.DockStyle.Top;
         this.rdoToolCopy.Location = new System.Drawing.Point(0, 0);
         this.rdoToolCopy.Name = "rdoToolCopy";
         this.rdoToolCopy.Size = new System.Drawing.Size(168, 24);
         this.rdoToolCopy.TabIndex = 0;
         this.rdoToolCopy.Text = "Copy";
         this.rdoToolCopy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // StatusBar
         // 
         this.StatusBar.Location = new System.Drawing.Point(0, 505);
         this.StatusBar.Name = "StatusBar";
         this.StatusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
                                                                                     this.sbpStatus,
                                                                                     this.sbpCtrl,
                                                                                     this.sbpTileCoord,
                                                                                     this.sbpPixelCoord,
                                                                                     this.sbpTileAtCursor,
                                                                                     this.sbpSelTile});
         this.StatusBar.ShowPanels = true;
         this.StatusBar.Size = new System.Drawing.Size(664, 16);
         this.StatusBar.TabIndex = 14;
         this.StatusBar.DrawItem += new System.Windows.Forms.StatusBarDrawItemEventHandler(this.StatusBar_DrawItem);
         // 
         // sbpStatus
         // 
         this.sbpStatus.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
         this.sbpStatus.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
         this.sbpStatus.Width = 564;
         // 
         // sbpCtrl
         // 
         this.sbpCtrl.Alignment = System.Windows.Forms.HorizontalAlignment.Right;
         this.sbpCtrl.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
         this.sbpCtrl.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
         this.sbpCtrl.Text = "CTRL";
         this.sbpCtrl.Width = 44;
         // 
         // sbpTileCoord
         // 
         this.sbpTileCoord.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
         this.sbpTileCoord.Width = 10;
         // 
         // sbpPixelCoord
         // 
         this.sbpPixelCoord.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
         this.sbpPixelCoord.Width = 10;
         // 
         // sbpTileAtCursor
         // 
         this.sbpTileAtCursor.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
         this.sbpTileAtCursor.Width = 10;
         // 
         // sbpSelTile
         // 
         this.sbpSelTile.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
         this.sbpSelTile.Width = 10;
         // 
         // frmMapEditor
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(664, 521);
         this.Controls.Add(this.MapDisplay);
         this.Controls.Add(this.MapSplitter);
         this.Controls.Add(this.tabSelector);
         this.Controls.Add(this.StatusBar);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.KeyPreview = true;
         this.Menu = this.mnuMapEditor;
         this.Name = "frmMapEditor";
         this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
         this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMapEditor_KeyDown);
         this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmMapEditor_KeyUp);
         this.tabSelector.ResumeLayout(false);
         this.tabTiles.ResumeLayout(false);
         this.pnlTiles.ResumeLayout(false);
         this.tabSprites.ResumeLayout(false);
         this.tabPlans.ResumeLayout(false);
         this.grpPlanList.ResumeLayout(false);
         this.grpPlanCoords.ResumeLayout(false);
         this.tabTools.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.sbpStatus)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpCtrl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpTileCoord)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpPixelCoord)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpTileAtCursor)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpSelTile)).EndInit();
         this.ResumeLayout(false);

      }
		#endregion

      #region Private Members
      private Size GetScrollBounds()
      {
         ProjectDataset.TilesetRow tsr = m_Layers[m_nCurLayer].LayerRow.TilesetRow;
         return new Size(m_Layers[m_nCurLayer].VirtualColumns * tsr.TileWidth,
            m_Layers[m_nCurLayer].VirtualRows * tsr.TileHeight);
      }

      private int CurrentTile
      {
         get
         {
            if ((TileSelector.CurrentCellIndex < 0) || (TileSelector.CurrentCellIndex >= TileSelector.FramesToDisplay.Count))
               return 0;
            if (cboCategory.SelectedItem is ProjectDataset.CategorizedTilesetRow)
               return ((TileProvider)TileSelector.FramesToDisplay[TileSelector.CurrentCellIndex]).TileIndex;
            return TileSelector.CurrentCellIndex;
         }
         set
         {
            for (int i=0; i<TileSelector.FramesToDisplay.Count; i++)
            {
               if (((TileProvider)TileSelector.FramesToDisplay[i]).TileIndex == value)
               {
                  if (TileSelector.CurrentCellIndex != i)
                  {
                     TileSelector.CurrentCellIndex = i;
                     TileSelector.ScrollCellIntoView(i);
                  }
                  return;
               }
            }

            cboCategory.SelectedIndex = 0;
            if ((TileSelector.CellCount > value) && (TileSelector.CurrentCellIndex != value))
            {
               TileSelector.CurrentCellIndex = value;
               TileSelector.ScrollCellIntoView(value);
            }
         }
      }

      private Point TileFromLayerPoint(Point ptLayer)
      {
         ProjectDataset.TilesetRow tsr = m_Layers[m_nCurLayer].LayerRow.TilesetRow;
         return new Point(ptLayer.X / tsr.TileWidth, ptLayer.Y / tsr.TileHeight);
      }

      private void LoadTileSelector(ProjectDataset.CategorizedTilesetRow category)
      {
         m_TileProvider = new FrameList();
         ProjectDataset.TilesetRow tsr = m_Layers[m_nCurLayer].LayerRow.TilesetRow;
         if (tsr == null)
            return;
         m_TileCache = new TileCache(tsr);
         if (category == null)
         {
            for (int i = 0; i < m_TileCache.Count; i++)
               m_TileProvider.Add(new TileProvider(m_TileCache, i));
         }
         else
         {
            foreach (ProjectDataset.CategoryTileRow tr in category.GetCategoryTileRows())
               m_TileProvider.Add(new TileProvider(m_TileCache, tr.TileValue));
         }
         TileSelector.Frameset = tsr.FramesetRow;
         TileSelector.FramesToDisplay = m_TileProvider;
      }

      private void LoadSpriteSelector(ProjectDataset.SpriteCategoryRow category)
      {
         m_SpriteProvider = new FrameList();
         m_SpriteProvider.ProvidesGraphics = true;
         IEnumerable DefinitionsInCategory;

         if (category == null)
         {
            DefinitionsInCategory = ProjectData.SpriteDefinition.Select(string.Empty, string.Empty, DataViewRowState.CurrentRows);
         }
         else
         {
            ArrayList SpriteDefList = new ArrayList();
            foreach(ProjectDataset.SpriteCategorySpriteRow drCatSpr in category.GetSpriteCategorySpriteRows())
            {
               SpriteDefList.Add(drCatSpr.SpriteDefinitionRow);
            }
            DefinitionsInCategory = SpriteDefList;
         }
         foreach(ProjectDataset.SpriteDefinitionRow drDef in DefinitionsInCategory)
         {
            ProjectDataset.SpriteStateRow[] drState = drDef.GetSpriteStateRows();
            if (drState.Length > 0)
            {
               ProjectDataset.SpriteFrameRow[] drFrame = ProjectData.GetSortedSpriteFrames(drState[0]);
               for (int validState = 0; validState < drState.Length; validState++)
               {
                  if (drState[validState].GetSpriteFrameRows().Length > 0)
                  {
                     SpriteProvider sp = SpriteProvider.GetDefaultInstance(
                        m_SpriteCache[drDef.Name], drState[validState].Name, 0, -1);
                     string name;
                     int index = 1;
                     do
                     {
                        name = sp.DefinitionName + " " + (index++).ToString();
                     } while (ProjectData.GetSprite(m_Layers[m_nCurLayer].LayerRow, name) != null);
                     sp.Name = name;
                     sp.Active = true;
                     m_SpriteProvider.Add(sp);
                     break;
                  }
               }
            }
         }
         SpriteSelector.FramesToDisplay = m_SpriteProvider;
      }

      private void LoadCategories()
      {
         cboCategory.Items.Clear();
         cboCategory.Items.Add("<All>");
         cboCategory.DisplayMember = "Name";
         foreach(ProjectDataset.CategorizedTilesetRow row in m_Layers[m_nCurLayer].LayerRow.TilesetRow.GetCategorizedTilesetRows())
         {
            cboCategory.Items.Add(row);
         }
         cboCategory.SelectedIndex = 0;
      }

      private void LoadSpriteCategories()
      {
         cboSpriteCategory.Items.Clear();
         cboSpriteCategory.Items.Add("<All>");
         cboSpriteCategory.DisplayMember = "Name";
         foreach(System.Data.DataRowView drv in ProjectData.SpriteCategory.DefaultView)
         {
            ProjectDataset.SpriteCategoryRow row = (ProjectDataset.SpriteCategoryRow)drv.Row;
            cboSpriteCategory.Items.Add(row);
         }
      }

      private void QueueRefreshLayerSprites()
      {
         if (m_RefreshSpriteTimer == null)
         {
            m_RefreshSpriteTimer = new Timer();
            m_RefreshSpriteTimer.Tick += new EventHandler(RefreshSpriteTimer_Tick);
         }
         else
            m_RefreshSpriteTimer.Stop();
         m_RefreshSpriteTimer.Interval = 100;
         m_RefreshSpriteTimer.Start();
      }
      private void QueueRefreshProperties()
      {
         if (m_RefreshSpriteTimer == null)
         {
            m_RefreshSpriteTimer = new Timer();
            m_RefreshSpriteTimer.Tick += new EventHandler(RefreshSpriteProperties_Tick);
         }
         else
            m_RefreshSpriteTimer.Stop();
         m_RefreshSpriteTimer.Interval = 100;
         m_RefreshSpriteTimer.Start();
      }
      private void RefreshLayerSprites()
      {
         foreach(Layer layer in m_Layers)
         {
            layer.RefreshLayerSprites(m_SpriteCache);
            layer.InjectCachedSprites();
         }
         MapDisplay.Invalidate();
      }

      private void PopulateAvailableSprites()
      {
         lstAvailableSprites.Items.Clear();
         foreach(ProjectDataset.SpriteRow drSprite in ProjectData.GetSortedSpriteRows(m_Layers[m_nCurLayer].LayerRow))
            lstAvailableSprites.Items.Add(drSprite);
      }
      private void PopulatePlans()
      {
         lstPlans.Items.Clear();
         foreach(ProjectDataset.SpritePlanRow drPlan in ProjectData.GetSortedSpritePlans(m_Layers[m_nCurLayer].LayerRow))
            lstPlans.Items.Add(drPlan);
      }

      private CursorMode GetCurrentMode()
      {
         if (tabSelector.SelectedTab == tabTiles)
            return CursorMode.PlaceTile;
         else if (tabSelector.SelectedTab == tabSprites)
         {
            if (rdoSelectSprites.Checked)
               return CursorMode.SelectSprite;
            else
               return CursorMode.PlaceSprite;
         }
         else if (tabSelector.SelectedTab == tabPlans)
         {
            if (rdoAppendCoord.Checked)
               return CursorMode.AddCoordinate;
            else
               return CursorMode.SelectCoordinate;
         }
         else if (tabSelector.SelectedTab == tabTools)
         {
            if (rdoToolCopy.Checked)
               return CursorMode.Copy;
            else if (rdoToolPaste.Checked)
               return CursorMode.Paste;
            else if (rdoToolPasteTransparent.Checked)
               return CursorMode.PasteTransparent;
         }
            return CursorMode.None;
      }

      private void DrawSpriteSelection(SpriteProvider sp)
      {
         Rectangle inner = sp.Bounds;
         MapDisplay.DrawShadedRectFrame(inner, 4, Color.Black, Color.White);
      }

      private void ReflectSelection(SpriteProvider[] selSprites)
      {
         m_ReflectingSelection = true;
         for(int lstIdx = 0; lstIdx < lstAvailableSprites.Items.Count; lstIdx++)
         {
            int selIdx;
            for(selIdx = 0; selIdx < selSprites.Length; selIdx++)
               if (selSprites[selIdx].Name == ((ProjectDataset.SpriteRow)lstAvailableSprites.Items[lstIdx]).Name)
               {
                  lstAvailableSprites.SetSelected(lstIdx, true);
                  break;
               }
            if (selIdx >= selSprites.Length)
               lstAvailableSprites.SetSelected(lstIdx, false);
         }
         m_ReflectingSelection = false;
      }

      private void ReflectSelection(SpriteProvider selSprite)
      {
         m_ReflectingSelection = true;
         for(int lstIdx = 0; lstIdx < lstAvailableSprites.Items.Count; lstIdx++)
            lstAvailableSprites.SetSelected(lstIdx, selSprite.Name == ((ProjectDataset.SpriteRow)lstAvailableSprites.Items[lstIdx]).Name);
         m_ReflectingSelection = false;
      }

      private void DrawPath(ProjectDataset.SpritePlanRow PathPlan, bool bIncludeMouse)
      {
         ProjectDataset.CoordinateRow[] coords = ProjectData.GetSortedCoordinates(PathPlan);
         System.Collections.ArrayList points = new ArrayList();
         System.Drawing.Point ptLyr = m_Layers[m_nCurLayer].CurrentPosition;
         System.Drawing.Point ptCur;
         for(int i=0; i<coords.Length; i++)
         {
            ptCur = new Point(coords[i].X, coords[i].Y);
            ptCur.Offset(ptLyr.X, ptLyr.Y);
            points.Add(ptCur);
         }
         if (bIncludeMouse && !m_SnappedMouseCoord.IsEmpty)
         {
            ptCur = m_SnappedMouseCoord;
            ptCur.Offset(ptLyr.X, ptLyr.Y);
            points.Add(ptCur);
         }
         if (points.Count > 1)
         {
            if (points.Count == 2)
            {
               // Regtangle is 1 pixel smaller than second coordinate dictates in order
               // to allow snap-to-tile feature to create rectangles that align to tile boundaries.
               System.Drawing.Point[] pts = (System.Drawing.Point[])points.ToArray(typeof(System.Drawing.Point));
               Rectangle DrawRect = new Rectangle(Math.Min(pts[0].X, pts[1].X), Math.Min(pts[0].Y, pts[1].Y),
                  Math.Abs(pts[0].X - pts[1].X), Math.Abs(pts[0].Y - pts[1].Y));
               MapDisplay.SetColor(Color.FromArgb(96, 0, 0, 255));
               MapDisplay.FillRectangle(DrawRect);
               MapDisplay.SetColor(Color.White);
               MapDisplay.DrawRectangle(DrawRect, 0);
            }

            MapDisplay.BeginLine(5f, unchecked((short)(0xff00)), true);
            for (int i = 0; i < points.Count-1; i++)
               MapDisplay.LineTo(((Point)(points[i])).X, ((Point)(points[i])).Y);
            MapDisplay.ArrowTo(((Point)(points[points.Count - 1])).X, ((Point)(points[points.Count - 1])).Y);
         }
         else if (points.Count == 1)
         {
            MapDisplay.PointSize = 6;
            MapDisplay.SetColor(Color.Black);
            MapDisplay.DrawPoint((System.Drawing.Point)points[0]);
            MapDisplay.PointSize = 4;
            MapDisplay.SetColor(Color.White);
            MapDisplay.DrawPoint((System.Drawing.Point)points[0]);
         }
      }

      private void DrawCopyRect()
      {
         ProjectDataset.TilesetRow tsr = m_Layers[m_nCurLayer].LayerRow.TilesetRow;
         Point TilePos;
         if (m_DragStart.IsEmpty)
            TilePos = TileFromLayerPoint(m_LayerMouseCoord);
         else
            TilePos = TileFromLayerPoint(m_DragStart);
         Point CopyStart = new Point(TilePos.X * tsr.TileWidth, TilePos.Y * tsr.TileHeight);
         TilePos = TileFromLayerPoint(m_LayerMouseCoord);
         Size CopySize = new Size(TilePos.X * tsr.TileWidth - CopyStart.X + tsr.TileWidth - 1, TilePos.Y * tsr.TileHeight - CopyStart.Y + tsr.TileHeight - 1);
         if ((CopySize.Width <= 0) || (CopySize.Height <= 0))
            return;
         Rectangle DrawRect = new Rectangle(CopyStart,  CopySize);
         DrawRect.Offset(m_Layers[m_nCurLayer].CurrentPosition.X, m_Layers[m_nCurLayer].CurrentPosition.Y);
         MapDisplay.SetColor(Color.FromArgb(96, 0, 0, 255));
         MapDisplay.FillRectangle(DrawRect);
         MapDisplay.SetColor(Color.White);
         MapDisplay.DrawRectangle(DrawRect, 0);
      }

      private void DrawPasteRect(CursorMode mode, bool persist)
      {
         int[,] tiles = (int[,])(Clipboard.GetDataObject().GetData(typeof(int[,])));
         Point StartPos = TileFromLayerPoint(m_LayerMouseCoord);
         int max;
         switch (m_Layers[m_nCurLayer].LayerRow.BytesPerTile)
         {
            case 1:
               max = byte.MaxValue;
               break;
            case 2:
               max = short.MaxValue;
               break;
            default:
               max = int.MaxValue;
               break;
         }

         // Dataset doesn't recognize that data changed without BeginEdit, apparently.
         if (mode != CursorMode.None)
            m_Layers[m_nCurLayer].LayerRow.BeginEdit();

         for(int y=0; y<=tiles.GetUpperBound(1); y++)
            for(int x=0; x<=tiles.GetUpperBound(0); x++)
            {
               Point TilePos = StartPos;
               TilePos.Offset(x,y);
               int nSel = tiles[x,y] % max;
               if ((TilePos.X >= 0) && (TilePos.Y >= 0) && (TilePos.X < m_Layers[m_nCurLayer].VirtualColumns) && (TilePos.Y < m_Layers[m_nCurLayer].VirtualRows))
               {
                  switch(mode)
                  {
                     case CursorMode.PasteTransparent:
                        if (nSel > 0)
                           goto case CursorMode.Paste;
                        break;
                     case CursorMode.Paste:
                        if (persist)
                           m_Layers[m_nCurLayer][TilePos.X, TilePos.Y] = nSel;
                        else
                           m_Layers[m_nCurLayer].InjectTile(TilePos.X, TilePos.Y, nSel);
                        break;
                  }
               }
            }
         if (mode != CursorMode.None)
            m_Layers[m_nCurLayer].LayerRow.EndEdit();
      }

      private void DrawSelectedCoord(CoordProvider coord)
      {
         MapDisplay.SetColor(Color.Black);
         MapDisplay.PointSize = 6;
         Point ptCur = new Point(coord.X, coord.Y);
         ptCur.Offset(m_Layers[m_nCurLayer].CurrentPosition);
         MapDisplay.DrawPoint(ptCur);
         MapDisplay.SetColor(Color.Red);
         MapDisplay.PointSize = 4;
         MapDisplay.DrawPoint(ptCur);
      }

      private ProjectDataset.CoordinateRow GetCoordAtPoint(Point ptNear)
      {
         object[] VisiblePlanList;

         if (rdoShowAllPlans.Checked)
            VisiblePlanList = ProjectData.GetSortedSpritePlans(m_Layers[m_nCurLayer].LayerRow);
         else
         {
            VisiblePlanList = new object[lstPlans.SelectedItems.Count];
            lstPlans.SelectedItems.CopyTo(VisiblePlanList, 0);
         }

         int MinDistSqr = 25;

         ProjectDataset.CoordinateRow result = null;

         foreach(ProjectDataset.SpritePlanRow plan in VisiblePlanList)
            foreach(ProjectDataset.CoordinateRow coord in plan.GetCoordinateRows())
            {
               int dx = ptNear.X - coord.X;
               int dy = ptNear.Y - coord.Y;

               if (dx * dx + dy * dy < MinDistSqr)
               {
                  MinDistSqr = dx * dx + dy * dy;
                  result = coord;
                  if (MinDistSqr <= 0)
                     return result;
               }
            }
         return result;
      }

      private void DeleteSelectedObjects()
      {
         switch(GetCurrentMode())
         {
            case CursorMode.SelectSprite:
               SpriteProvider[] sprites = null;
               if (grdSprite.SelectedObjects is SpriteProvider[])
               {
                  sprites = new SpriteProvider[grdSprite.SelectedObjects.Length];
                  grdSprite.SelectedObjects.CopyTo(sprites, 0);
                  if (DialogResult.Yes != MessageBox.Show(this, "Are you sure you want to delete " + sprites.Length.ToString() + " sprites?", "Delete Sprites", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                     return;
               } 
               else if(grdSprite.SelectedObject is SpriteProvider)
               {
                  sprites = new SpriteProvider[1];
                  sprites[0] = (SpriteProvider)grdSprite.SelectedObject;
                  if (DialogResult.Yes != MessageBox.Show(this, "Are you sure you want to delete sprite \"" + sprites[0].Name + "\"?", "Delete Sprite", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                     return;
               }
               else
               {
                  MessageBox.Show(this, "Select sprites first.", "Delete Selection", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
               }
               foreach(SpriteProvider sp in sprites)
                  sp.DeleteRow();
               // Don't call QueueRefreshLayerSprites otherwise code can execute on
               // deleted sprite rows before they are refreshed.
               RefreshLayerSprites();
               break;
            case CursorMode.SelectCoordinate:
            case CursorMode.AddCoordinate:
               if (lstPlanCoords.SelectedIndex < 0)
               {
                  if (lstPlans.SelectedIndex < 0)
                  {
                     MessageBox.Show(this, "Select coordinates or a plan first.", "Delete Selection", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     return;
                  }
                  DeleteSelectedPlans();
                  break;
               }
               CoordProvider[] deleteCoords = new CoordProvider[lstPlanCoords.SelectedItems.Count];
               lstPlanCoords.SelectedItems.CopyTo(deleteCoords, 0);
               if (DialogResult.Yes != MessageBox.Show(this, "Are you sure you want to delete " + deleteCoords.Length.ToString() + " coordinate(s) from " + deleteCoords[0].CoordinateRow.SpritePlanRowParent.Name + "?", "Delete Coordinates", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                  return;
               lstPlanCoords.SelectedIndex = -1;
               foreach(CoordProvider coord in deleteCoords)
               {
                  ProjectData.DeleteCoordinate(coord.CoordinateRow);
                  lstPlanCoords.Items.Remove(coord);
               }
               break;
            default:
               MessageBox.Show(this, "Please select a sprite or a coordinate to delete first", "Delete Selected Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               break;
         }
      }

      private void DisableDelete()
      {
         mnuEditDelete.Text = "Delete Selected Objects";
         mnuEditDelete.Enabled = false;      
      }

      private void DeleteCoordinatesMode()
      {
         mnuEditDelete.Text = "Delete Selected Coordinates";
         mnuEditDelete.Enabled = true;
      }

      private void DeleteSpriteMode()
      {
         mnuEditDelete.Text = "Delete Selected Sprites";
         mnuEditDelete.Enabled = true;
      }

      private void DeleteSelectedPlans()
      {
         string msg;
         if (lstPlans.SelectedIndices.Count > 1)
         {
            msg = "Are you sure you want to delete the selected " + lstPlans.SelectedIndices.Count.ToString() + " plans?";
         }
         else if (lstPlans.SelectedIndices.Count == 1)
         {
            msg = "Are you sure you want to delete the selected plan?";
         }
         else
         {
            MessageBox.Show(this, "Must select a plan before deleting plans.", "Delete Plans", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }

         if (MessageBox.Show(this, msg, "Delete Plans", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
         {
            grdPlan.SelectedObject = null;
            m_UpdatingList = true;
            try
            {
               lstPlanCoords.Items.Clear();
               for (int i = 0; i < lstPlans.Items.Count;)
                  if (lstPlans.GetSelected(i))
                     ((ProjectDataset.SpritePlanRow)lstPlans.Items[i]).Delete();
                  else
                     i++;
            }
            finally
            {
               m_UpdatingList = false;
            }
         }
      }

      private void EditSelectedPlan()
      {
         string msg = null;
         if (lstPlans.SelectedIndices.Count > 1)
         {
            msg = "Please select just a single plan first.";
         }
         else if (lstPlans.SelectedIndices.Count == 0)
         {
            msg = "Please select a plan first.";
         }
         if (msg != null)
         {
            MessageBox.Show(this, msg, "Edit Plan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }

         frmPlanEdit.Edit(this.MdiParent, (ProjectDataset.SpritePlanRow)lstPlans.SelectedItem);
      }

      private void LocateSelectedObject()
      {
         string msg = null;
         switch(GetCurrentMode())
         {
            case CursorMode.SelectSprite:
               if (lstAvailableSprites.SelectedIndices.Count > 1)
                  msg = "Please select just a single sprite first.";
               else if (lstAvailableSprites.SelectedIndices.Count == 0)
                  msg = "Please select a sprite first.";
               break;
            case CursorMode.SelectCoordinate:
            case CursorMode.AddCoordinate:
               if (lstPlans.SelectedIndices.Count > 1)
               {
                  msg = "Please select just a single plan first.";
               }
               else if (lstPlans.SelectedIndices.Count == 0)
               {
                  msg = "Please select a plan first.";
               }
               else if (lstPlanCoords.Items.Count == 0)
               {
                  msg = "No coordinate to locate.";
               }
               break;
            default:
               msg = "No object to locate.";
               break;
         }

         if (msg != null)
         {
            MessageBox.Show(this, msg, "Locate Selected Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }

         int posX, posY;

         switch(GetCurrentMode())
         {
            case CursorMode.SelectSprite:
               SpriteProvider sprite = (SpriteProvider)grdSprite.SelectedObject;
               posX = sprite.X;
               posY = sprite.Y;
               break;
            case CursorMode.AddCoordinate:
            case CursorMode.SelectCoordinate:
               CoordProvider coord;

               if (lstPlanCoords.SelectedIndex > 0)
                  coord = (CoordProvider)lstPlanCoords.SelectedItem;
               else
                  coord = (CoordProvider)lstPlanCoords.Items[0];

               posX = coord.X;
               posY = coord.Y;
               break;
            default:
               System.Diagnostics.Debug.Fail("Unreachable code reached");
               return;
         }

         posX -= MapDisplay.ClientSize.Width/2;
         posY -= MapDisplay.ClientSize.Height/2;

         MapDisplay.AutoScrollPosition = new Point(posX, posY);
      }

      private void AddNewPlan()
      {
         int i=1;
         string PlanName;
         do
         {
            PlanName = m_Layers[m_nCurLayer].LayerRow.MapRow.Name + " Plan " + i++.ToString();
         } while (null != ProjectData.GetSpritePlan(m_Layers[m_nCurLayer].LayerRow, PlanName));
         ProjectData.AddSpritePlan(m_Layers[m_nCurLayer].LayerRow, PlanName, 1);
      }
      #endregion

      #region Public Static Members
      public static void Edit(Form MdiParent, ProjectDataset.LayerRow EditRow)
      {
         foreach(Form frm in MdiParent.MdiChildren)
         {
            frmMapEditor f = frm as frmMapEditor;
            if (f != null)
            {
               if (f.m_Layers[f.m_nCurLayer].LayerRow == EditRow)
               {
                  f.Activate();
                  return;
               }
            }
         }

         frmMapEditor frmNew = new frmMapEditor(EditRow);
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
      #endregion

      #region Event Handlers
      private void MapDisplay_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         m_Layers[m_nCurLayer].ClearInjections();
         if ((m_RefreshSpriteTimer == null) || (!m_RefreshSpriteTimer.Enabled))
            m_Layers[m_nCurLayer].InjectCachedSprites();
         Point oldCoord = m_LayerMouseCoord;

         // Auto-Scroll
         if (0 != (int)(e.Button & MouseButtons.Left))
         {
            Point newPos = new Point(-MapDisplay.AutoScrollPosition.X, -MapDisplay.AutoScrollPosition.Y);
            bool bMove = false;
            if (e.X < 32)
            {
               newPos.X -= 32;
               bMove = true;
            }
            if (e.X > MapDisplay.ClientSize.Width - 32)
            {
               newPos.X += 32;
               bMove = true;
            }
            if (e.Y < 32)
            {
               newPos.Y -= 32;
               bMove = true;
            }
            if (e.Y > MapDisplay.ClientSize.Height - 32)
            {
               newPos.Y += 32;
               bMove = true;
            }
            if (bMove)
               MapDisplay.AutoScrollPosition = newPos;
         }

         m_LayerMouseCoord = new Point(e.X - m_Layers[m_nCurLayer].CurrentPosition.X, e.Y - m_Layers[m_nCurLayer].CurrentPosition.Y);
         Point TilePos = TileFromLayerPoint(m_LayerMouseCoord);
         if (mnuSnapToTiles.Checked)
         {
            ProjectDataset.TilesetRow tsr = m_Layers[m_nCurLayer].LayerRow.TilesetRow;
            m_SnappedMouseCoord.X = TilePos.X * tsr.TileWidth;
            m_SnappedMouseCoord.Y = TilePos.Y * tsr.TileHeight;
         }
         else
            m_SnappedMouseCoord = m_LayerMouseCoord;
         sbpPixelCoord.Text = "Pixel X,Y: " + m_LayerMouseCoord.X.ToString() + "," + m_LayerMouseCoord.Y.ToString();
         if ((TilePos.X >= 0) && (TilePos.Y >= 0) && (TilePos.X < m_Layers[m_nCurLayer].VirtualColumns) && (TilePos.Y < m_Layers[m_nCurLayer].VirtualRows))
         {
            sbpTileCoord.Text = "Tile X,Y: " + TilePos.X.ToString() + "," + TilePos.Y.ToString();
            sbpTileAtCursor.Text = "Tile @X,Y: " + m_Layers[m_nCurLayer][TilePos.X, TilePos.Y].ToString();
         }
         else
         {
            sbpTileCoord.Text = "Tile X,Y: N/A";
            sbpTileAtCursor.Text = "Tile @X,Y: N/A";
         }
         if (GetCurrentMode() == CursorMode.PlaceTile)
            sbpSelTile.Text = "Selected Tile: " + CurrentTile.ToString();
         else
            sbpSelTile.Text = "Selected Tile: N/A";
         
         switch(GetCurrentMode())
         {
            case CursorMode.PlaceTile:
               int nSel = CurrentTile;
               if ((TilePos.X >= 0) && (TilePos.Y >= 0) && (TilePos.X < m_Layers[m_nCurLayer].VirtualColumns) && (TilePos.Y < m_Layers[m_nCurLayer].VirtualRows))
               {
                  if (0 != (int)(e.Button & MouseButtons.Right))
                  {
                     CurrentTile = m_Layers[m_nCurLayer][TilePos.X, TilePos.Y];
                  }
                  else if (0 != (e.Button & MouseButtons.Left))
                  {
                     // Dataset doesn't recognize that data changed without BeginEdit, apparently.
                     m_Layers[m_nCurLayer].LayerRow.BeginEdit();
                     m_Layers[m_nCurLayer][TilePos.X, TilePos.Y] = nSel;
                     m_Layers[m_nCurLayer].LayerRow.EndEdit();
                  }
                  else
                     m_Layers[m_nCurLayer].InjectTile(TilePos.X, TilePos.Y, nSel);
               }
               break;
            case CursorMode.PlaceSprite:
               if (SpriteSelector.CurrentCellIndex < 0)
                  return;
            {
               SpriteProvider sp = (SpriteProvider)m_SpriteProvider[SpriteSelector.CurrentCellIndex];
               int nCount = sp.GetSubFrameCount();
               if ((sp.X != m_SnappedMouseCoord.X) || (sp.Y != m_SnappedMouseCoord.Y))
               {
                  sp.X = m_SnappedMouseCoord.X;
                  sp.Y = m_SnappedMouseCoord.Y;
                  grdSprite.Refresh();
               }
               for(int i = 0; i<nCount; i++)
               {
                  m_Layers[m_nCurLayer].InjectFrame(sp.X, sp.Y, sp.Priority, sp.GetSubFrame(i), sp.Color);
               }
            }
               break;
            case CursorMode.SelectSprite:
            {
               SpriteProvider[] sprites = null;
               if (grdSprite.SelectedObjects is SpriteProvider[])
               {
                  sprites = new SpriteProvider[grdSprite.SelectedObjects.Length];
                  grdSprite.SelectedObjects.CopyTo(sprites, 0);
               } 
               else if(grdSprite.SelectedObject is SpriteProvider)
               {
                  sprites = new SpriteProvider[1];
                  sprites[0] = (SpriteProvider)grdSprite.SelectedObject;
               }
               else
                  break;
               if (0 != (int)(e.Button & MouseButtons.Left))
               {
                  foreach (SpriteProvider sp in sprites)
                  {
                     if ((oldCoord.X != m_LayerMouseCoord.X) || (oldCoord.Y != m_LayerMouseCoord.Y))
                     {
                        sp.X += m_LayerMouseCoord.X - oldCoord.X;
                        sp.Y += m_LayerMouseCoord.Y - oldCoord.Y;
                     }
                  }
                  QueueRefreshProperties();
               }
               foreach (SpriteProvider sp in sprites)
               {
                  if (sp.Bounds.Contains(m_LayerMouseCoord))
                  {
                     Cursor.Current = Cursors.SizeAll;
                     break;
                  }
               }
            }
               break;
            case CursorMode.PasteTransparent:
            case CursorMode.Paste:
               if (Clipboard.GetDataObject().GetDataPresent(typeof(int[,])))
                  DrawPasteRect(GetCurrentMode(), (0 != (e.Button & MouseButtons.Left)));
               break;
         }
         MapDisplay.Invalidate();
      }

      private void MapDisplay_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         try
         {
            m_LayerMouseCoord = new Point(e.X - m_Layers[m_nCurLayer].CurrentPosition.X, e.Y - m_Layers[m_nCurLayer].CurrentPosition.Y);
            Point TilePos = TileFromLayerPoint(m_LayerMouseCoord);
            if (mnuSnapToTiles.Checked)
            {
               ProjectDataset.TilesetRow tsr = m_Layers[m_nCurLayer].LayerRow.TilesetRow;
               m_SnappedMouseCoord.X = TilePos.X * tsr.TileWidth;
               m_SnappedMouseCoord.Y = TilePos.Y * tsr.TileHeight;
            }
            else
               m_SnappedMouseCoord = m_LayerMouseCoord;

            MapDisplay.Focus();

            switch(GetCurrentMode())
            {
               case CursorMode.PlaceTile:
                  int nSel = CurrentTile;
                  if ((TilePos.X < m_Layers[m_nCurLayer].VirtualColumns) && (TilePos.Y < m_Layers[m_nCurLayer].VirtualRows))
                  {
                     if (e.Button == MouseButtons.Left)
                     {
                        // Dataset doesn't recognize that data changed without BeginEdit, apparently.
                        m_Layers[m_nCurLayer].LayerRow.BeginEdit();
                        m_Layers[m_nCurLayer][TilePos.X, TilePos.Y] = nSel;
                        m_Layers[m_nCurLayer].LayerRow.EndEdit();
                     }
                     else if (e.Button == MouseButtons.Right)
                     {
                        CurrentTile = m_Layers[m_nCurLayer][TilePos.X, TilePos.Y];
                     }
                  }
                  break;
               case CursorMode.PlaceSprite:
                  if (SpriteSelector.CurrentCellIndex < 0)
                     return;
                  SpriteProvider sp = (SpriteProvider)m_SpriteProvider[SpriteSelector.CurrentCellIndex];
                  string[] paramNames = new string[sp.ParameterNames.Count];
                  sp.ParameterNames.CopyTo(paramNames, 0);
                  ProjectDataset.LayerRow drLayer = m_Layers[m_nCurLayer].LayerRow;
                  ProjectData.AddSprite(drLayer.Name, sp.Name, sp.DefinitionName, sp.CurrentStateName,
                     sp.CurrentFrame, sp.X, sp.Y, sp.DX, sp.DY, drLayer.MapRow.Name, sp.Priority,
                     sp.Active, sp.Solidity, sp.Color, paramNames, sp.ParameterValues);
                  string name;
                  int index = 1;
                  do
                  {
                     name = sp.DefinitionName + " " + (index++).ToString();
                  } while (ProjectData.GetSprite(m_Layers[m_nCurLayer].LayerRow, name) != null);
                  sp.Name = name;
                  QueueRefreshLayerSprites();
                  break;
               case CursorMode.SelectSprite:
                  Point ptOffset = m_Layers[m_nCurLayer].CurrentPosition;
                  SpriteProvider[] arsp = m_Layers[m_nCurLayer].GetSpritesAtPoint(m_LayerMouseCoord);
                  if (arsp.Length > 0)
                  {
                     if (0 != (m_CurrentModifiers & Keys.Control))
                     {
                        if (grdSprite.SelectedObjects != null)
                        {
                           SpriteProvider[] all = new SpriteProvider[arsp.Length + grdSprite.SelectedObjects.Length];
                           grdSprite.SelectedObjects.CopyTo(all, 0);
                           arsp.CopyTo(all, grdSprite.SelectedObjects.Length);
                           grdSprite.SelectedObjects = all;
                           ReflectSelection(all);
                        }
                        else if (grdSprite.SelectedObject != null)
                        {
                           SpriteProvider[] all = new SpriteProvider[arsp.Length + 1];
                           all[0] = (SpriteProvider)grdSprite.SelectedObject;
                           arsp.CopyTo(all, 1);
                           grdSprite.SelectedObjects = all;
                           ReflectSelection(all);
                        }
                     }
                     else
                     {
                        if (arsp.Length > 1)
                        {
                           grdSprite.SelectedObjects = arsp;
                           ReflectSelection(arsp);
                        }
                        else
                        {
                           grdSprite.SelectedObject = arsp[0];
                           ReflectSelection(arsp[0]);
                        }
                     }
                     DeleteSpriteMode();
                  }
                  else
                  {
                     grdSprite.SelectedObject = null;
                     lstAvailableSprites.SelectedIndex = -1;
                     DisableDelete();
                  }
                  break;
               case CursorMode.AddCoordinate:
                  if (lstPlans.SelectedIndices.Count == 1)
                     lstPlanCoords.Items.Add(new CoordProvider(
                        ProjectData.AppendPlanCoordinate((ProjectDataset.SpritePlanRow)lstPlans.SelectedItem,
                        m_SnappedMouseCoord.X, m_SnappedMouseCoord.Y, 0)));
                  break;
               case CursorMode.SelectCoordinate:
                  ProjectDataset.CoordinateRow drCoord = GetCoordAtPoint(m_LayerMouseCoord);
                  lstPlanCoords.ClearSelected();
                  if (drCoord == null)
                  {
                     lstPlans.ClearSelected();
                     DisableDelete();
                  }
                  else
                  {
                     if(!lstPlans.GetSelected(lstPlans.Items.IndexOf(drCoord.SpritePlanRowParent)))
                     {
                        lstPlans.ClearSelected();
                        lstPlans.SelectedItem = drCoord.SpritePlanRowParent;
                     }
                     for (int i=0; i<lstPlanCoords.Items.Count; i++)
                     {
                        if (((CoordProvider)lstPlanCoords.Items[i]).CoordinateRow == drCoord)
                           lstPlanCoords.SelectedIndex = i;
                     }
                     DeleteCoordinatesMode();
                  }
                  break;
               case CursorMode.Copy:
                  m_DragStart = m_LayerMouseCoord;
                  break;
               case CursorMode.PasteTransparent:
               case CursorMode.Paste:
                  if (Clipboard.GetDataObject().GetDataPresent(typeof(int[,])))
                     DrawPasteRect(GetCurrentMode(), true);
                  break;
            }
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Mep Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
      }

      private void MapDisplay_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         if ((GetCurrentMode() == CursorMode.Copy) && !m_DragStart.IsEmpty)
         {
            Point StartTile = TileFromLayerPoint(m_DragStart);
            Point EndTile = TileFromLayerPoint(m_LayerMouseCoord);
            Size CopySize = new Size(EndTile.X - StartTile.X + 1, EndTile.Y - StartTile.Y + 1);
            if ((CopySize.Width > 0) && (CopySize.Height > 0))
            {
               int[,] tiles = new int[CopySize.Width, CopySize.Height];
               for(int y=0; y<CopySize.Height; y++)
                  for(int x=0; x<CopySize.Width; x++)
                     tiles[x,y] = m_Layers[m_nCurLayer][x+StartTile.X, y+StartTile.Y];
               Clipboard.SetDataObject(tiles, true);
               rdoToolPaste.Checked = true;
            }
            m_DragStart = Point.Empty;
         }
      }

      private void dataMonitor_TileRowChanged(object sender, SGDK2.ProjectDataset.TileRowChangeEvent e)
      {
         ProjectDataset.TileRow tr =  (ProjectDataset.TileRow)e.Row;
         switch(e.Action)
         {
            case DataRowAction.Add:
            case DataRowAction.Change:
               if (tr.TilesetRow == this.m_Layers[m_nCurLayer].LayerRow.TilesetRow)
               {
                  if (tr.TileValue >= m_TileCache.Count)
                     m_TileCache = new TileCache(tr.TilesetRow);
                  else
                     m_TileCache.RefreshTile(tr);
                  TileSelector.Invalidate();
               }
               break;
            case DataRowAction.Delete:
               if (string.Compare((string)e.Row["Name", DataRowVersion.Current], m_Layers[m_nCurLayer].LayerRow.TilesetRow.Name) == 0)
               {
                  int nTileValue = (int)tr["TileValue", DataRowVersion.Current];
                  int nFrameCount = m_Layers[m_nCurLayer].LayerRow.TilesetRow.FramesetRow.GetFrameRows().Length;
                  if (nFrameCount > 0)
                     m_TileCache.ResetTile(nTileValue, nTileValue % nFrameCount);
                  else
                     m_TileCache.EmptyTile(nTileValue);
                  TileSelector.Invalidate();
               }
               break;
         }
      }

      private void dataMonitor_TileFrameRowChanged(object sender, SGDK2.ProjectDataset.TileFrameRowChangeEvent e)
      {
         ProjectDataset.TileFrameRow tfr = (ProjectDataset.TileFrameRow)e.Row;
         switch(e.Action)
         {
            case DataRowAction.Add:
            case DataRowAction.Change:
               if (tfr.TileRowParent.TilesetRow == m_Layers[m_nCurLayer].LayerRow.TilesetRow)
               {
                  m_TileCache.RefreshTile(tfr.TileRowParent);
                  TileSelector.Invalidate();
               }
               break;
            case DataRowAction.Delete:
               if (tfr.HasVersion(DataRowVersion.Current) // Deleting, but not deleted
                  && (string.Compare((string)tfr["Name", DataRowVersion.Current], m_Layers[m_nCurLayer].LayerRow.TilesetRow.Name) == 0))
               {
                  m_DeletedKey = (int)tfr["TileValue", DataRowVersion.Current];
               }
               else if (m_DeletedKey >= 0) // Deleted -- key value no longer available in row
               {
                  ProjectDataset.TileRow tr = ProjectData.Tile.FindByNameTileValue(
                     (string)m_Layers[m_nCurLayer].LayerRow.TilesetRow.Name, m_DeletedKey);
                  m_TileCache.RefreshTile(tr);
                  TileSelector.Invalidate();
                  m_DeletedKey = -1;
               }
               break;
         }
      }

      private void dataMonitor_LayerRowDeleted(object sender, SGDK2.ProjectDataset.LayerRowChangeEvent e)
      {
         if (e.Row == m_Layers[m_nCurLayer].LayerRow)
            this.Close();
      }

      private void dataMonitor_MapRowDeleted(object sender, SGDK2.ProjectDataset.MapRowChangeEvent e)
      {
         if (e.Row == m_Layers[0].LayerRow.MapRow)
            this.Close();
      }

      private void dataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void MapDisplay_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         if (m_DangerWillRobinson)
            return;

         try
         {
            MapDisplay.MakeCurrent();
            Size ScrollBounds = GetScrollBounds();
            if (ScrollBounds != MapDisplay.AutoScrollMinSize)
               MapDisplay.AutoScrollMinSize = ScrollBounds;
            for (int i = 0; i < m_Layers.Length; i++)
            {
               if (i == m_nCurLayer)
                  m_Layers[i].CurrentPosition = MapDisplay.AutoScrollPosition;
               else
               {
                  Point ptPos = Point.Empty;
                  Layer CurLayer = m_Layers[m_nCurLayer];
                  Layer DrawLayer = m_Layers[i];
                  // Manually scroll other layers because current layer is always
                  // 1:1 scroll ratio with AutoScrollPosition (not like map).
                  if (CurLayer.ScrollRate.Width > 0)
                     ptPos.X = (int)((MapDisplay.AutoScrollPosition.X - CurLayer.AbsolutePosition.X) / CurLayer.ScrollRate.Width * DrawLayer.ScrollRate.Width + DrawLayer.AbsolutePosition.X);
                  else
                     ptPos.X = MapDisplay.AutoScrollPosition.X - CurLayer.AbsolutePosition.X + DrawLayer.AbsolutePosition.X;
                  if (CurLayer.ScrollRate.Height > 0)
                     ptPos.Y = (int)((MapDisplay.AutoScrollPosition.Y - CurLayer.AbsolutePosition.Y) / CurLayer.ScrollRate.Height * DrawLayer.ScrollRate.Height + DrawLayer.AbsolutePosition.Y);
                  else
                     ptPos.Y = MapDisplay.AutoScrollPosition.Y - CurLayer.AbsolutePosition.Y + DrawLayer.AbsolutePosition.Y;
                  DrawLayer.CurrentPosition = ptPos;
               }
               if (mnuLayers.MenuItems[i].Checked)
                  m_Layers[i].Draw(MapDisplay, MapDisplay.ClientSize);
            }
            if (mnuViewLayerEdges.Checked)
            {
               for (int i = 0; i < m_Layers.Length; i++)
               {
                  if (mnuLayers.MenuItems[i].Checked)
                  {
                     Layer lyr = m_Layers[i];
                     ProjectDataset.TilesetRow tsr = lyr.LayerRow.TilesetRow;
                     Rectangle rcLayer = new Rectangle(lyr.CurrentPosition,
                        new Size(lyr.VirtualColumns * tsr.TileWidth,
                        lyr.VirtualRows * tsr.TileHeight));
                     MapDisplay.SetColor(Color.White);
                     MapDisplay.DrawRectangle(rcLayer, unchecked((short)0xFF00));
                     MapDisplay.SetColor(Color.Black);
                     MapDisplay.DrawRectangle(rcLayer, unchecked((short)0x00FF));
                  }
               }
            }
            switch (GetCurrentMode())
            {
               case CursorMode.SelectSprite:
                  if (grdSprite.SelectedObjects is SpriteProvider[])
                  {
                     foreach (SpriteProvider sp in grdSprite.SelectedObjects)
                     {
                        DrawSpriteSelection(sp);
                     }
                  }
                  else if (grdSprite.SelectedObject is SpriteProvider)
                  {
                     SpriteProvider sp = (SpriteProvider)grdSprite.SelectedObject;
                     if (sp.IsDataRow)
                     {
                        DrawSpriteSelection(sp);
                     }
                  }
                  break;
               case CursorMode.AddCoordinate:
               case CursorMode.SelectCoordinate:
                  {
                     bool bIncludeMouse = false;
                     if ((lstPlans.SelectedIndices.Count == 1) && (GetCurrentMode() == CursorMode.AddCoordinate))
                        bIncludeMouse = true;

                     if (rdoShowAllPlans.Checked)
                        foreach (ProjectDataset.SpritePlanRow plan in ProjectData.GetSortedSpritePlans(m_Layers[m_nCurLayer].LayerRow))
                           DrawPath(plan, bIncludeMouse && (plan == lstPlans.SelectedItem));
                     else
                        foreach (ProjectDataset.SpritePlanRow plan in lstPlans.SelectedItems)
                           DrawPath(plan, bIncludeMouse);
                     if (grdPlan.SelectedObjects is CoordProvider[])
                        foreach (CoordProvider coord in grdPlan.SelectedObjects)
                           DrawSelectedCoord(coord);
                     else if (grdPlan.SelectedObject is CoordProvider)
                        DrawSelectedCoord((CoordProvider)grdPlan.SelectedObject);
                  }
                  break;
               case CursorMode.Copy:
                  if (!m_LayerMouseCoord.IsEmpty)
                     DrawCopyRect();
                  break;
            }

            MapDisplay.Flush();
            MapDisplay.SwapBuffers();
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(MdiParent, "An error occurred while drawing the display in the map editor. This might happen if too many displays are active. In order to attempt to avoid fatal errors and data loss, the display handling in this map editor window will be disabled and you should close it yourself. Details:\r\n" + ex.ToString(), "Map Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            m_DangerWillRobinson = true;
         }
      }

      private void cboCategory_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (cboCategory.SelectedItem is ProjectDataset.CategorizedTilesetRow)
            LoadTileSelector((ProjectDataset.CategorizedTilesetRow)cboCategory.SelectedItem);
         else
            LoadTileSelector(null);
      }

      private void cboSpriteCategory_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (cboSpriteCategory.SelectedItem is ProjectDataset.SpriteCategoryRow)
            LoadSpriteSelector((ProjectDataset.SpriteCategoryRow)cboSpriteCategory.SelectedItem);
         else
            LoadSpriteSelector(null);
      }
      private void SpriteSelector_CurrentCellChanged(object sender, System.EventArgs e)
      {
         lstAvailableSprites.SelectedIndex = -1;
         if (SpriteSelector.CurrentCellIndex >= 0)
            grdSprite.SelectedObject = (SpriteProvider)m_SpriteProvider[SpriteSelector.CurrentCellIndex];
         else
            grdSprite.SelectedObject = null;
      }

      private void dataMonitor_SpriteRowChanged(object sender, SGDK2.ProjectDataset.SpriteRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            ProjectDataset.SpriteRow drSprite = (ProjectDataset.SpriteRow)e.Row;
            if (drSprite.LayerRowParent.MapRow == m_Layers[m_nCurLayer].LayerRow.MapRow)
            {
               QueueRefreshLayerSprites();
            }
            lstAvailableSprites.Items.Add(e.Row);
         }
         else if (e.Action == DataRowAction.Change)
         {
            m_Layers[m_nCurLayer].ClearInjections();
            m_Layers[m_nCurLayer].InjectCachedSprites();
            MapDisplay.Invalidate();
         }
      }

      private void dataMonitor_SpriteRowChanging(object sender, SGDK2.ProjectDataset.SpriteRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Change)
         {
            if(String.Compare(e.Row.Name, e.Row[ProjectData.Sprite.NameColumn, DataRowVersion.Current].ToString()) != 0)
            {
               for (int i = 0; i<lstAvailableSprites.Items.Count; i++)
                  if (lstAvailableSprites.Items[i] == e.Row)
                  {
                     m_UpdatingList = true;
                     lstAvailableSprites.Items[i] = e.Row; // Force refresh;
                     m_UpdatingList = false;
                  }
            }
         }
      }

      private void dataMonitor_SpriteRowDeleting(object sender, SGDK2.ProjectDataset.SpriteRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Delete)
         {
            ProjectDataset.SpriteRow drSprite = (ProjectDataset.SpriteRow)e.Row;
            if (drSprite.LayerRowParent.MapRow == m_Layers[m_nCurLayer].LayerRow.MapRow)
            {
               QueueRefreshLayerSprites();
               lstAvailableSprites.Items.Remove(e.Row);
            }
         }            
      }

      private void RefreshSpriteTimer_Tick(object sender, EventArgs e)
      {
         m_RefreshSpriteTimer.Dispose();
         m_RefreshSpriteTimer = null;
         RefreshLayerSprites();
      }

      private void MapDisplay_MouseLeave(object sender, System.EventArgs e)
      {
         m_Layers[m_nCurLayer].ClearInjections();
         m_Layers[m_nCurLayer].InjectCachedSprites();
         m_LayerMouseCoord = Point.Empty;
         m_SnappedMouseCoord = Point.Empty;
         MapDisplay.Invalidate();
      }

      private void frmMapEditor_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
      {
         m_CurrentModifiers = e.Modifiers;
         StatusBar.Invalidate();
      }

      private void frmMapEditor_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
      {
         m_CurrentModifiers = e.Modifiers;
         StatusBar.Invalidate();
      }

      private void StatusBar_DrawItem(object sender, System.Windows.Forms.StatusBarDrawItemEventArgs sbdevent)
      {
         if (sbdevent.Panel == sbpCtrl)
         {
            Brush b = SystemBrushes.ControlText;
            if (0 == (m_CurrentModifiers & Keys.Control))
               b = SystemBrushes.ControlDark;
            StringFormat sf = new StringFormat(StringFormatFlags.NoWrap);
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            sbdevent.Graphics.DrawString(sbdevent.Panel.Text, ((StatusBar)sender).Font, b, sbdevent.Bounds,sf);
         }
      }

      private void rdoSpriteMode_CheckedChanged(object sender, System.EventArgs e)
      {
         switch(GetCurrentMode())
         {
            case CursorMode.SelectSprite:
               lstAvailableSprites.Visible = true;
               cboSpriteCategory.Visible = SpriteSelector.Visible = false;
               break;
            case CursorMode.PlaceSprite:
               lstAvailableSprites.Visible = false;
               cboSpriteCategory.Visible = SpriteSelector.Visible = true;
               break;
         }
      }

      private void lstAvailableSprites_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (m_ReflectingSelection || m_UpdatingList)
            return;
         if (lstAvailableSprites.SelectedIndex < 0)
         {
            grdSprite.SelectedObject = null;
            return;
         }

         System.Windows.Forms.ListBox.SelectedIndexCollection selIdcs = lstAvailableSprites.SelectedIndices;

         SpriteProvider[] selSprites = new SpriteProvider[selIdcs.Count];
         for (int i = 0; i < selIdcs.Count; i++)
         {
            ProjectDataset.SpriteRow drSprite = (ProjectDataset.SpriteRow)lstAvailableSprites.Items[selIdcs[i]];
            selSprites[i] = new SpriteProvider(m_SpriteCache[drSprite.DefinitionName], drSprite);
         }
         if (selIdcs.Count == 1)
            grdSprite.SelectedObject = selSprites[0];
         else
            grdSprite.SelectedObjects = selSprites;
         MapDisplay.Invalidate();
      }

      private void dataMonitor_SpritePlanRowChanged(object sender, SGDK2.ProjectDataset.SpritePlanRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            if ((e.Row.LayerRowParent == m_Layers[m_nCurLayer].LayerRow) && (!lstPlans.Items.Contains(e.Row)))
               lstPlans.Items.Add(e.Row);
         }
      }

      private void dataMonitor_SpritePlanRowChanging(object sender, SGDK2.ProjectDataset.SpritePlanRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Change)
         {
            if ((e.Row.LayerRowParent == m_Layers[m_nCurLayer].LayerRow) && e.Row.HasVersion(DataRowVersion.Current) && (e.Row[ProjectData.SpritePlan.NameColumn, DataRowVersion.Current].ToString().CompareTo(e.Row.Name) != 0))
            {
               m_UpdatingList = true;
               lstPlans.Items[lstPlans.Items.IndexOf(e.Row)] = e.Row;
               m_UpdatingList = false;
            }
         }
      }

      private void dataMonitor_SpritePlanRowDeleted(object sender, SGDK2.ProjectDataset.SpritePlanRowChangeEvent e)
      {
         if (lstPlans.Items.Contains(e.Row))
            lstPlans.Items.Remove(e.Row);
      }

      private void tbPlans_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
      {
         if (e.Button == tbbNewPlan)
         {
            AddNewPlan();
         }
         else if (e.Button == tbbDeletePlan)
         {
            DeleteSelectedPlans();
         }
         else if (e.Button == tbbPlanProperties)
         {
            EditSelectedPlan();
         }
         else if (e.Button == tbbSort)
         {
            lstPlans.Sorted = true;
            lstPlans.Sorted = false;
         }
         else if (e.Button == tbbGotoCoord)
         {
            LocateSelectedObject();
         }
      }

      private void mnuAddPlan_Click(object sender, System.EventArgs e)
      {
         AddNewPlan();
      }

      private void lstPlans_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (m_UpdatingList)
            return;
         lstPlanCoords.Items.Clear();
         if (lstPlans.SelectedIndices.Count > 1)
         {
            ListBox.SelectedObjectCollection SelObjs = lstPlans.SelectedItems;
            PlanProvider[] SelProviders = new PlanProvider[SelObjs.Count];
            for (int i = 0; i < SelObjs.Count; i++)
               SelProviders[i] = new PlanProvider((ProjectDataset.SpritePlanRow)SelObjs[i]);
            grdPlan.SelectedObjects = SelProviders;
         }
         else if (lstPlans.SelectedIndices.Count == 1)
         {
            grdPlan.SelectedObject = new PlanProvider((ProjectDataset.SpritePlanRow)lstPlans.SelectedItem);

            foreach(ProjectDataset.CoordinateRow coord in ProjectData.GetSortedCoordinates((ProjectDataset.SpritePlanRow)lstPlans.SelectedItem))
               lstPlanCoords.Items.Add(new CoordProvider(coord));
         }
         else
            grdPlan.SelectedObject = null;
         MapDisplay.Invalidate();
      }

      private void rdoShowPlans_CheckedChanged(object sender, System.EventArgs e)
      {
         MapDisplay.Invalidate();
      }

      private void lstPlanCoords_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         ListBox.SelectedObjectCollection SelObjs = lstPlanCoords.SelectedItems;
         if (SelObjs.Count == 0)
         {
            grdPlan.SelectedObject = null;
            MapDisplay.Invalidate();
            return;
         }
         if (SelObjs.Count == 1)
         {
            grdPlan.SelectedObject = SelObjs[0];
            MapDisplay.Invalidate();
            return;
         }
         CoordProvider[] NewSel = new CoordProvider[lstPlanCoords.SelectedIndices.Count];
         SelObjs.CopyTo(NewSel, 0);
         grdPlan.SelectedObjects = NewSel;
         MapDisplay.Invalidate();
      }

      private void grdPlan_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
      {
         MapDisplay.Invalidate();
      }

      private void tabSelector_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         MapDisplay.Invalidate();
         if (tabSelector.SelectedTab == tabSprites)
         {
            DeleteSpriteMode();
         }
         else
         {
            DisableDelete();
         }

         if (tabSelector.SelectedTab == tabPlans)
         {
            PlanProvider.ResetCustomProperties();
            PlanParamDescriptor.ResetCustomProperties();
         }
      }

      private void dataMonitor_LayerRowChanged(object sender, SGDK2.ProjectDataset.LayerRowChangeEvent e)
      {
         if (e.Row == m_Layers[m_nCurLayer].LayerRow)
         {
            this.Text = "Edit Map Layer - " + e.Row.Name;
         }
      }

      private void mnuEditDelete_Click(object sender, System.EventArgs e)
      {
         DeleteSelectedObjects();
      }
      private void RefreshSpriteProperties_Tick(object sender, EventArgs e)
      {
         m_RefreshSpriteTimer.Dispose();
         m_RefreshSpriteTimer = null;
         if ((grdSprite.SelectedObjects != null) && (grdSprite.SelectedObjects.Length > 1))
            grdSprite.SelectedObjects = grdSprite.SelectedObjects;
         else
            grdSprite.SelectedObject = grdSprite.SelectedObject;
      }

      private void TileSelector_CurrentCellChanged(object sender, System.EventArgs e)
      {
         sbpSelTile.Text = "Selected Tile: " + CurrentTile.ToString();
      }

      private void mnuSnapToTiles_Click(object sender, System.EventArgs e)
      {
         mnuSnapToTiles.Checked = !mnuSnapToTiles.Checked;
      }

      private void LayerMenu_Click(object sender, System.EventArgs e)
      {
         ((MenuItem)sender).Checked = !((MenuItem)sender).Checked;
         MapDisplay.Invalidate();
      }

      private void lstPlans_Enter(object sender, System.EventArgs e)
      {
         mnuEditDelete.Text = "Delete Selected Plan";
         mnuEditDelete.Enabled = true;
      }

      private void lstPlans_Leave(object sender, System.EventArgs e)
      {
         DisableDelete();
      }

      private void lstPlanCoords_Enter(object sender, System.EventArgs e)
      {
         DeleteCoordinatesMode();
      }

      private void lstPlanCoords_Leave(object sender, System.EventArgs e)
      {
         DisableDelete();
      }

      private void grdPlan_Enter(object sender, System.EventArgs e)
      {
         DisableDelete();
      }

      private void grdSprite_Enter(object sender, System.EventArgs e)
      {
         DisableDelete();
      }

      private void grdSprite_Leave(object sender, System.EventArgs e)
      {
         DeleteSpriteMode();
      }

      private void lstAvailableSprites_Enter(object sender, System.EventArgs e)
      {
         DeleteSpriteMode();
      }

      private void mnuEditDetails_Click(object sender, System.EventArgs e)
      {
         EditSelectedPlan();
      }

      private void mnuLocateCoordinate_Click(object sender, System.EventArgs e)
      {
         LocateSelectedObject();
      }

      private void mnuSortPlans_Click(object sender, System.EventArgs e)
      {
         lstPlans.Sorted = true;
         lstPlans.Sorted = false;
      }

      private void tbSprites_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
      {
         if (e.Button == tbbDeleteSprite)
            DeleteSelectedObjects();
         else if (e.Button == tbbGotoSprite)
            LocateSelectedObject();
      }

      private void mnuViewLayerEdges_Click(object sender, System.EventArgs e)
      {
         mnuViewLayerEdges.Checked = !mnuViewLayerEdges.Checked;
         MapDisplay.Invalidate();
      }

      private void lstAvailableSprites_DoubleClick(object sender, System.EventArgs e)
      {
         LocateSelectedObject();
      }

      private void lstPlans_DoubleClick(object sender, EventArgs e)
      {
         LocateSelectedObject();
      }

      private void lstPlanCoords_DoubleClick(object sender, EventArgs e)
      {
         LocateSelectedObject();
      }
   }
   #endregion
}
