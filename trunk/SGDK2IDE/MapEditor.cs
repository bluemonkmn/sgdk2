/*
 * Scrolling Game Development Kit 2.0
 *
 * Copyright � 2000 - 2005 Benjamin Marty <BlueMonkMN@email.com>
 * 
 * Distributed under the GNU General Public License (GPL)
 *   - see included file COPYING.txt for details, or visit:
 *     http://www.fsf.org/copyleft/gpl.html
 */

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
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
      private bool m_ChangingName = false;
      private Point m_LayerMouseCoord = Point.Empty;
      private int m_DeletedKey = -1;
      #endregion

      #region Form Designer Members
      private System.Windows.Forms.Splitter MapSplitter;
      private SGDK2.Display MapDisplay;
      private System.Windows.Forms.MainMenu mnuMapEditor;
      private System.Windows.Forms.MenuItem menuItem1;
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
         SelectCoordinate
      }

      class TileProvider : IProvideFrame
      {
         private readonly TileCache m_TileCache;
         private readonly int nTileIndex;

         public TileProvider(TileCache tileCache, int i)
         {
            m_TileCache = tileCache;
            nTileIndex = i;
         }

         #region IProvideFrame Members

         public int FrameIndex
         {
            get
            {
               if (m_TileCache[nTileIndex].Length > 0)
                  return m_TileCache[nTileIndex][0];
               else
                  return 0;
            }
         }

         public int[] FrameIndexes
         {
            get
            {
               return m_TileCache[nTileIndex];
            }
         }

         public int TileIndex
         {
            get
            {
               return nTileIndex;
            }
         }

         public bool IsSelected
         {
            get
            {
               return false;
            }
            set
            {
            }
         }

         #endregion
      }
      #endregion

      #region Initialization and clean-up
		public frmMapEditor(ProjectDataset.LayerRow Layer)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         ProjectDataset.LayerRow[] lrs = ProjectData.GetSortedLayers(Layer.MapRow);
         m_Layers = new Layer[lrs.Length];
         for (int i=0; i<lrs.Length; i++)
         {
            m_Layers[i] = new Layer(lrs[i], MapDisplay);
            if (lrs[i] == Layer)
               m_nCurLayer = i;
         }
         m_SpriteCache = new SpriteCache(MapDisplay);
         LoadCategories();
         LoadSpriteCategories();
         RefreshLayerSprites();
         PopulateAvailableSprites();
         PopulatePlans();
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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmMapEditor));
         this.MapSplitter = new System.Windows.Forms.Splitter();
         this.MapDisplay = new SGDK2.Display();
         this.mnuMapEditor = new System.Windows.Forms.MainMenu();
         this.menuItem1 = new System.Windows.Forms.MenuItem();
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
         this.tabPlans = new System.Windows.Forms.TabPage();
         this.grpPlanList = new System.Windows.Forms.GroupBox();
         this.lstPlans = new System.Windows.Forms.ListBox();
         this.rdoShowAllPlans = new System.Windows.Forms.RadioButton();
         this.rdoShowSelectedPlans = new System.Windows.Forms.RadioButton();
         this.tbPlans = new System.Windows.Forms.ToolBar();
         this.tbbNewPlan = new System.Windows.Forms.ToolBarButton();
         this.tbbDeletePlan = new System.Windows.Forms.ToolBarButton();
         this.imlToolbar = new System.Windows.Forms.ImageList(this.components);
         this.CoordSplitter = new System.Windows.Forms.Splitter();
         this.grpPlanCoords = new System.Windows.Forms.GroupBox();
         this.lstPlanCoords = new System.Windows.Forms.ListBox();
         this.rdoAppendCoord = new System.Windows.Forms.RadioButton();
         this.rdoSelectCoord = new System.Windows.Forms.RadioButton();
         this.PlanSplitter = new System.Windows.Forms.Splitter();
         this.grdPlan = new System.Windows.Forms.PropertyGrid();
         this.StatusBar = new System.Windows.Forms.StatusBar();
         this.sbpStatus = new System.Windows.Forms.StatusBarPanel();
         this.sbpCtrl = new System.Windows.Forms.StatusBarPanel();
         this.tabSelector.SuspendLayout();
         this.tabTiles.SuspendLayout();
         this.pnlTiles.SuspendLayout();
         this.tabSprites.SuspendLayout();
         this.tabPlans.SuspendLayout();
         this.grpPlanList.SuspendLayout();
         this.grpPlanCoords.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.sbpStatus)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpCtrl)).BeginInit();
         this.SuspendLayout();
         // 
         // MapSplitter
         // 
         this.MapSplitter.Location = new System.Drawing.Point(144, 0);
         this.MapSplitter.Name = "MapSplitter";
         this.MapSplitter.Size = new System.Drawing.Size(5, 489);
         this.MapSplitter.TabIndex = 8;
         this.MapSplitter.TabStop = false;
         // 
         // MapDisplay
         // 
         this.MapDisplay.AutoScroll = true;
         this.MapDisplay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.MapDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
         this.MapDisplay.GameDisplayMode = SGDK2.GameDisplayMode.m640x480x24;
         this.MapDisplay.Location = new System.Drawing.Point(149, 0);
         this.MapDisplay.Name = "MapDisplay";
         this.MapDisplay.Size = new System.Drawing.Size(443, 473);
         this.MapDisplay.TabIndex = 10;
         this.MapDisplay.Windowed = true;
         this.MapDisplay.Paint += new System.Windows.Forms.PaintEventHandler(this.MapDisplay_Paint);
         this.MapDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DisplayMap_MouseMove);
         this.MapDisplay.MouseLeave += new System.EventHandler(this.MapDisplay_MouseLeave);
         this.MapDisplay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MapDisplay_MouseDown);
         // 
         // mnuMapEditor
         // 
         this.mnuMapEditor.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                     this.menuItem1});
         // 
         // menuItem1
         // 
         this.menuItem1.Index = 0;
         this.menuItem1.Text = "&Layers";
         // 
         // dataMonitor
         // 
         this.dataMonitor.TileFrameRowDeleting += new SGDK2.ProjectDataset.TileFrameRowChangeEventHandler(this.dataMonitor_TileFrameRowChanged);
         this.dataMonitor.TileFrameRowDeleted += new SGDK2.ProjectDataset.TileFrameRowChangeEventHandler(this.dataMonitor_TileFrameRowChanged);
         this.dataMonitor.SpritePlanRowDeleted += new SGDK2.ProjectDataset.SpritePlanRowChangeEventHandler(this.dataMonitor_SpritePlanRowDeleted);
         this.dataMonitor.SpritePlanRowChanged += new SGDK2.ProjectDataset.SpritePlanRowChangeEventHandler(this.dataMonitor_SpritePlanRowChanged);
         this.dataMonitor.SpritePlanRowChanging += new SGDK2.ProjectDataset.SpritePlanRowChangeEventHandler(this.dataMonitor_SpritePlanRowChanging);
         this.dataMonitor.SpriteRowChanged += new SGDK2.ProjectDataset.SpriteRowChangeEventHandler(this.dataMonitor_SpriteRowChanged);
         this.dataMonitor.SpriteRowChanging += new SGDK2.ProjectDataset.SpriteRowChangeEventHandler(this.dataMonitor_SpriteRowChanging);
         this.dataMonitor.MapRowDeleted += new SGDK2.ProjectDataset.MapRowChangeEventHandler(this.dataMonitor_MapRowDeleted);
         this.dataMonitor.LayerRowDeleted += new SGDK2.ProjectDataset.LayerRowChangeEventHandler(this.dataMonitor_LayerRowDeleted);
         this.dataMonitor.TileFrameRowChanged += new SGDK2.ProjectDataset.TileFrameRowChangeEventHandler(this.dataMonitor_TileFrameRowChanged);
         this.dataMonitor.TileRowChanged += new SGDK2.ProjectDataset.TileRowChangeEventHandler(this.dataMonitor_TileRowChanged);
         this.dataMonitor.TileRowDeleting += new SGDK2.ProjectDataset.TileRowChangeEventHandler(this.dataMonitor_TileRowChanged);
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         this.dataMonitor.SpriteRowDeleted += new SGDK2.ProjectDataset.SpriteRowChangeEventHandler(this.dataMonitor_SpriteRowDeleted);
         // 
         // tabSelector
         // 
         this.tabSelector.Controls.Add(this.tabTiles);
         this.tabSelector.Controls.Add(this.tabSprites);
         this.tabSelector.Controls.Add(this.tabPlans);
         this.tabSelector.Dock = System.Windows.Forms.DockStyle.Left;
         this.tabSelector.Location = new System.Drawing.Point(0, 0);
         this.tabSelector.Name = "tabSelector";
         this.tabSelector.SelectedIndex = 0;
         this.tabSelector.Size = new System.Drawing.Size(144, 489);
         this.tabSelector.TabIndex = 13;
         this.tabSelector.SelectedIndexChanged += new System.EventHandler(this.tabSelector_SelectedIndexChanged);
         // 
         // tabTiles
         // 
         this.tabTiles.Controls.Add(this.pnlTiles);
         this.tabTiles.Location = new System.Drawing.Point(4, 22);
         this.tabTiles.Name = "tabTiles";
         this.tabTiles.Size = new System.Drawing.Size(136, 463);
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
         this.pnlTiles.Size = new System.Drawing.Size(136, 463);
         this.pnlTiles.TabIndex = 14;
         // 
         // TileSelector
         // 
         this.TileSelector.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
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
         this.TileSelector.Size = new System.Drawing.Size(136, 442);
         this.TileSelector.TabIndex = 9;
         // 
         // cboCategory
         // 
         this.cboCategory.Dock = System.Windows.Forms.DockStyle.Top;
         this.cboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboCategory.Location = new System.Drawing.Point(0, 0);
         this.cboCategory.Name = "cboCategory";
         this.cboCategory.Size = new System.Drawing.Size(136, 21);
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
         this.tabSprites.Location = new System.Drawing.Point(4, 22);
         this.tabSprites.Name = "tabSprites";
         this.tabSprites.Size = new System.Drawing.Size(136, 463);
         this.tabSprites.TabIndex = 1;
         this.tabSprites.Text = "Sprites";
         // 
         // lstAvailableSprites
         // 
         this.lstAvailableSprites.DisplayMember = "Name";
         this.lstAvailableSprites.Dock = System.Windows.Forms.DockStyle.Fill;
         this.lstAvailableSprites.IntegralHeight = false;
         this.lstAvailableSprites.Location = new System.Drawing.Point(0, 52);
         this.lstAvailableSprites.Name = "lstAvailableSprites";
         this.lstAvailableSprites.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
         this.lstAvailableSprites.Size = new System.Drawing.Size(136, 262);
         this.lstAvailableSprites.TabIndex = 6;
         this.lstAvailableSprites.SelectedIndexChanged += new System.EventHandler(this.lstAvailableSprites_SelectedIndexChanged);
         // 
         // SpriteSelector
         // 
         this.SpriteSelector.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.SpriteSelector.CellPadding = new System.Drawing.Size(0, 0);
         this.SpriteSelector.CellSize = new System.Drawing.Size(0, 0);
         this.SpriteSelector.CurrentCellIndex = -1;
         this.SpriteSelector.Dock = System.Windows.Forms.DockStyle.Fill;
         this.SpriteSelector.Frameset = null;
         this.SpriteSelector.FramesToDisplay = null;
         this.SpriteSelector.GraphicSheet = null;
         this.SpriteSelector.Location = new System.Drawing.Point(0, 52);
         this.SpriteSelector.Name = "SpriteSelector";
         this.SpriteSelector.SheetImage = null;
         this.SpriteSelector.Size = new System.Drawing.Size(136, 262);
         this.SpriteSelector.TabIndex = 2;
         this.SpriteSelector.Visible = false;
         this.SpriteSelector.CurrentCellChanged += new System.EventHandler(this.SpriteSelector_CurrentCellChanged);
         // 
         // SpriteSplitter
         // 
         this.SpriteSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.SpriteSplitter.Location = new System.Drawing.Point(0, 314);
         this.SpriteSplitter.Name = "SpriteSplitter";
         this.SpriteSplitter.Size = new System.Drawing.Size(136, 5);
         this.SpriteSplitter.TabIndex = 3;
         this.SpriteSplitter.TabStop = false;
         // 
         // cboSpriteCategory
         // 
         this.cboSpriteCategory.Dock = System.Windows.Forms.DockStyle.Top;
         this.cboSpriteCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboSpriteCategory.Location = new System.Drawing.Point(0, 32);
         this.cboSpriteCategory.Name = "cboSpriteCategory";
         this.cboSpriteCategory.Size = new System.Drawing.Size(136, 20);
         this.cboSpriteCategory.TabIndex = 1;
         this.cboSpriteCategory.Visible = false;
         this.cboSpriteCategory.SelectedIndexChanged += new System.EventHandler(this.cboSpriteCategory_SelectedIndexChanged);
         // 
         // grdSprite
         // 
         this.grdSprite.CommandsVisibleIfAvailable = true;
         this.grdSprite.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.grdSprite.HelpVisible = false;
         this.grdSprite.LargeButtons = false;
         this.grdSprite.LineColor = System.Drawing.SystemColors.ScrollBar;
         this.grdSprite.Location = new System.Drawing.Point(0, 319);
         this.grdSprite.Name = "grdSprite";
         this.grdSprite.Size = new System.Drawing.Size(136, 144);
         this.grdSprite.TabIndex = 0;
         this.grdSprite.Text = "PropertyGrid";
         this.grdSprite.ToolbarVisible = false;
         this.grdSprite.ViewBackColor = System.Drawing.SystemColors.Window;
         this.grdSprite.ViewForeColor = System.Drawing.SystemColors.WindowText;
         // 
         // rdoAddSprites
         // 
         this.rdoAddSprites.Dock = System.Windows.Forms.DockStyle.Top;
         this.rdoAddSprites.Location = new System.Drawing.Point(0, 16);
         this.rdoAddSprites.Name = "rdoAddSprites";
         this.rdoAddSprites.Size = new System.Drawing.Size(136, 16);
         this.rdoAddSprites.TabIndex = 5;
         this.rdoAddSprites.Text = "Add Sprites";
         this.rdoAddSprites.CheckedChanged += new System.EventHandler(this.rdoSpriteMode_CheckedChanged);
         // 
         // rdoSelectSprites
         // 
         this.rdoSelectSprites.Checked = true;
         this.rdoSelectSprites.Dock = System.Windows.Forms.DockStyle.Top;
         this.rdoSelectSprites.Location = new System.Drawing.Point(0, 0);
         this.rdoSelectSprites.Name = "rdoSelectSprites";
         this.rdoSelectSprites.Size = new System.Drawing.Size(136, 16);
         this.rdoSelectSprites.TabIndex = 4;
         this.rdoSelectSprites.TabStop = true;
         this.rdoSelectSprites.Text = "Select Sprites";
         this.rdoSelectSprites.CheckedChanged += new System.EventHandler(this.rdoSpriteMode_CheckedChanged);
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
         this.tabPlans.Size = new System.Drawing.Size(136, 463);
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
         this.grpPlanList.Size = new System.Drawing.Size(136, 245);
         this.grpPlanList.TabIndex = 22;
         this.grpPlanList.TabStop = false;
         this.grpPlanList.Text = "Plans";
         // 
         // lstPlans
         // 
         this.lstPlans.DisplayMember = "Name";
         this.lstPlans.Dock = System.Windows.Forms.DockStyle.Fill;
         this.lstPlans.IntegralHeight = false;
         this.lstPlans.Location = new System.Drawing.Point(3, 56);
         this.lstPlans.Name = "lstPlans";
         this.lstPlans.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
         this.lstPlans.Size = new System.Drawing.Size(130, 154);
         this.lstPlans.TabIndex = 0;
         this.lstPlans.SelectedIndexChanged += new System.EventHandler(this.lstPlans_SelectedIndexChanged);
         // 
         // rdoShowAllPlans
         // 
         this.rdoShowAllPlans.Checked = true;
         this.rdoShowAllPlans.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.rdoShowAllPlans.Location = new System.Drawing.Point(3, 210);
         this.rdoShowAllPlans.Name = "rdoShowAllPlans";
         this.rdoShowAllPlans.Size = new System.Drawing.Size(130, 16);
         this.rdoShowAllPlans.TabIndex = 18;
         this.rdoShowAllPlans.TabStop = true;
         this.rdoShowAllPlans.Text = "Show All";
         this.rdoShowAllPlans.CheckedChanged += new System.EventHandler(this.rdoShowPlans_CheckedChanged);
         // 
         // rdoShowSelectedPlans
         // 
         this.rdoShowSelectedPlans.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.rdoShowSelectedPlans.Location = new System.Drawing.Point(3, 226);
         this.rdoShowSelectedPlans.Name = "rdoShowSelectedPlans";
         this.rdoShowSelectedPlans.Size = new System.Drawing.Size(130, 16);
         this.rdoShowSelectedPlans.TabIndex = 17;
         this.rdoShowSelectedPlans.Text = "Show Selected";
         this.rdoShowSelectedPlans.CheckedChanged += new System.EventHandler(this.rdoShowPlans_CheckedChanged);
         // 
         // tbPlans
         // 
         this.tbPlans.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                                   this.tbbNewPlan,
                                                                                   this.tbbDeletePlan});
         this.tbPlans.Divider = false;
         this.tbPlans.DropDownArrows = true;
         this.tbPlans.ImageList = this.imlToolbar;
         this.tbPlans.Location = new System.Drawing.Point(3, 16);
         this.tbPlans.Name = "tbPlans";
         this.tbPlans.ShowToolTips = true;
         this.tbPlans.Size = new System.Drawing.Size(130, 40);
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
         // imlToolbar
         // 
         this.imlToolbar.ImageSize = new System.Drawing.Size(15, 15);
         this.imlToolbar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlToolbar.ImageStream")));
         this.imlToolbar.TransparentColor = System.Drawing.Color.Magenta;
         // 
         // CoordSplitter
         // 
         this.CoordSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.CoordSplitter.Location = new System.Drawing.Point(0, 245);
         this.CoordSplitter.MinExtra = 93;
         this.CoordSplitter.MinSize = 72;
         this.CoordSplitter.Name = "CoordSplitter";
         this.CoordSplitter.Size = new System.Drawing.Size(136, 5);
         this.CoordSplitter.TabIndex = 20;
         this.CoordSplitter.TabStop = false;
         // 
         // grpPlanCoords
         // 
         this.grpPlanCoords.Controls.Add(this.lstPlanCoords);
         this.grpPlanCoords.Controls.Add(this.rdoAppendCoord);
         this.grpPlanCoords.Controls.Add(this.rdoSelectCoord);
         this.grpPlanCoords.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.grpPlanCoords.Location = new System.Drawing.Point(0, 250);
         this.grpPlanCoords.Name = "grpPlanCoords";
         this.grpPlanCoords.Size = new System.Drawing.Size(136, 104);
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
         this.lstPlanCoords.Size = new System.Drawing.Size(130, 53);
         this.lstPlanCoords.TabIndex = 17;
         this.lstPlanCoords.SelectedIndexChanged += new System.EventHandler(this.lstPlanCoords_SelectedIndexChanged);
         // 
         // rdoAppendCoord
         // 
         this.rdoAppendCoord.Checked = true;
         this.rdoAppendCoord.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.rdoAppendCoord.Location = new System.Drawing.Point(3, 69);
         this.rdoAppendCoord.Name = "rdoAppendCoord";
         this.rdoAppendCoord.Size = new System.Drawing.Size(130, 16);
         this.rdoAppendCoord.TabIndex = 7;
         this.rdoAppendCoord.TabStop = true;
         this.rdoAppendCoord.Text = "Append Coordinates";
         // 
         // rdoSelectCoord
         // 
         this.rdoSelectCoord.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.rdoSelectCoord.Location = new System.Drawing.Point(3, 85);
         this.rdoSelectCoord.Name = "rdoSelectCoord";
         this.rdoSelectCoord.Size = new System.Drawing.Size(130, 16);
         this.rdoSelectCoord.TabIndex = 8;
         this.rdoSelectCoord.Text = "Select Coordinates";
         // 
         // PlanSplitter
         // 
         this.PlanSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.PlanSplitter.Location = new System.Drawing.Point(0, 354);
         this.PlanSplitter.Name = "PlanSplitter";
         this.PlanSplitter.Size = new System.Drawing.Size(136, 5);
         this.PlanSplitter.TabIndex = 1;
         this.PlanSplitter.TabStop = false;
         // 
         // grdPlan
         // 
         this.grdPlan.CommandsVisibleIfAvailable = true;
         this.grdPlan.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.grdPlan.HelpVisible = false;
         this.grdPlan.LargeButtons = false;
         this.grdPlan.LineColor = System.Drawing.SystemColors.ScrollBar;
         this.grdPlan.Location = new System.Drawing.Point(0, 359);
         this.grdPlan.Name = "grdPlan";
         this.grdPlan.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
         this.grdPlan.Size = new System.Drawing.Size(136, 104);
         this.grdPlan.TabIndex = 6;
         this.grdPlan.Text = "PropertyGrid";
         this.grdPlan.ToolbarVisible = false;
         this.grdPlan.ViewBackColor = System.Drawing.SystemColors.Window;
         this.grdPlan.ViewForeColor = System.Drawing.SystemColors.WindowText;
         this.grdPlan.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.grdPlan_PropertyValueChanged);
         // 
         // StatusBar
         // 
         this.StatusBar.Location = new System.Drawing.Point(149, 473);
         this.StatusBar.Name = "StatusBar";
         this.StatusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
                                                                                     this.sbpStatus,
                                                                                     this.sbpCtrl});
         this.StatusBar.ShowPanels = true;
         this.StatusBar.Size = new System.Drawing.Size(443, 16);
         this.StatusBar.TabIndex = 14;
         this.StatusBar.DrawItem += new System.Windows.Forms.StatusBarDrawItemEventHandler(this.StatusBar_DrawItem);
         // 
         // sbpStatus
         // 
         this.sbpStatus.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
         this.sbpStatus.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
         this.sbpStatus.Width = 383;
         // 
         // sbpCtrl
         // 
         this.sbpCtrl.Alignment = System.Windows.Forms.HorizontalAlignment.Right;
         this.sbpCtrl.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
         this.sbpCtrl.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
         this.sbpCtrl.Text = "CTRL";
         this.sbpCtrl.Width = 44;
         // 
         // frmMapEditor
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(592, 489);
         this.Controls.Add(this.MapDisplay);
         this.Controls.Add(this.StatusBar);
         this.Controls.Add(this.MapSplitter);
         this.Controls.Add(this.tabSelector);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.KeyPreview = true;
         this.Menu = this.mnuMapEditor;
         this.Name = "frmMapEditor";
         this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMapEditor_KeyDown);
         this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmMapEditor_KeyUp);
         this.tabSelector.ResumeLayout(false);
         this.tabTiles.ResumeLayout(false);
         this.pnlTiles.ResumeLayout(false);
         this.tabSprites.ResumeLayout(false);
         this.tabPlans.ResumeLayout(false);
         this.grpPlanList.ResumeLayout(false);
         this.grpPlanCoords.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.sbpStatus)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpCtrl)).EndInit();
         this.ResumeLayout(false);

      }
		#endregion

      #region Private Members
      private Size GetScrollBounds()
      {
         ProjectDataset.TilesetRow tsr = m_Layers[m_nCurLayer].LayerRow.TilesetRow;
         return new Size(m_Layers[m_nCurLayer].Columns * tsr.TileWidth,
            m_Layers[m_nCurLayer].Rows * tsr.TileHeight);
      }

      private int CurrentTile
      {
         get
         {
            if ((TileSelector.CurrentCellIndex < 0) || (TileSelector.CurrentCellIndex >= TileSelector.FramesToDisplay.Count))
               return 0;
            if (cboCategory.SelectedItem is ProjectDataset.CategoryRow)
               return ((TileProvider)TileSelector.FramesToDisplay[TileSelector.CurrentCellIndex]).TileIndex;
            return TileSelector.CurrentCellIndex;
         }
      }
      private Point TileFromLayerPoint(Point ptLayer)
      {
         ProjectDataset.TilesetRow tsr = m_Layers[m_nCurLayer].LayerRow.TilesetRow;
         return new Point(ptLayer.X / tsr.TileWidth, ptLayer.Y / tsr.TileHeight);
      }

      private void LoadTileSelector(ProjectDataset.CategoryRow category)
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
            DefinitionsInCategory = ProjectData.SpriteDefinition;
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
                     SpriteProvider sp = new SpriteProvider(
                        m_SpriteCache[drDef.Name], drState[validState].Name, 0);
                     string name;
                     int index = 1;
                     do
                     {
                        name = sp.DefinitionName + " " + (index++).ToString();
                     } while (ProjectData.GetSprite(m_Layers[m_nCurLayer].LayerRow, name) != null);
                     sp.Name = name;
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
         foreach(ProjectDataset.CategoryRow row in m_Layers[m_nCurLayer].LayerRow.TilesetRow.GetCategoryRows())
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
         else
            return CursorMode.None;
      }

      private void DrawSpriteSelection(Graphics gfx, SpriteProvider sp)
      {
         Rectangle inner = sp.Bounds;
         Point ptOffset = m_Layers[m_nCurLayer].CurrentPosition;
         inner.Offset(ptOffset.X, ptOffset.Y);
         Rectangle outer = inner;
         outer.Inflate(4,4);
         using (System.Drawing.Region rgn = new Region(outer))
         {
            rgn.Exclude(inner);
            using(Brush brSel = new System.Drawing.Drawing2D.HatchBrush(
                     System.Drawing.Drawing2D.HatchStyle.Percent25, Color.DarkBlue, Color.Wheat))
            {
               gfx.FillRegion(brSel, rgn);
            }
         }
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

      private void DrawPath(ProjectDataset.SpritePlanRow PathPlan, System.Drawing.Graphics gfx, bool bIncludeMouse)
      {
         ProjectDataset.CoordinateRow[] coords = ProjectData.GetSortedCoordinates(PathPlan);
         System.Collections.ArrayList points = new ArrayList();
         System.Drawing.Point ptLyr = m_Layers[m_nCurLayer].CurrentPosition;
         System.Drawing.Point ptCur;
         gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
         for(int i=0; i<coords.Length; i++)
         {
            ptCur = new Point(coords[i].X, coords[i].Y);
            ptCur.Offset(ptLyr.X, ptLyr.Y);
            points.Add(ptCur);
         }
         if (bIncludeMouse && !m_LayerMouseCoord.IsEmpty)
         {
            ptCur = m_LayerMouseCoord;
            ptCur.Offset(ptLyr.X, ptLyr.Y);
            points.Add(ptCur);
         }
         if (points.Count > 1)
         {
            if (points.Count == 2)
            {
               System.Drawing.Point[] pts = (System.Drawing.Point[])points.ToArray(typeof(System.Drawing.Point));
               Rectangle DrawRect = new Rectangle(Math.Min(pts[0].X, pts[1].X), Math.Min(pts[0].Y, pts[1].Y),
                  Math.Abs(pts[0].X - pts[1].X) + 1, Math.Abs(pts[0].Y - pts[1].Y) + 1);
               System.Drawing.Brush RectBrush = new SolidBrush(System.Drawing.Color.FromArgb(96, 0, 0, 255));
               try
               {
                  gfx.FillRectangle(RectBrush, DrawRect);
                  gfx.DrawRectangle(System.Drawing.Pens.White, DrawRect);
               }
               finally
               {
                  RectBrush.Dispose();
               }
            }

            System.Drawing.Pen pen = new Pen(System.Drawing.Color.Black, 6);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
            pen.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            pen.CompoundArray = new float[] {0,.2f, .8f, 1f};
            try
            {
               gfx.DrawLines(pen, (System.Drawing.Point[])points.ToArray(typeof(System.Drawing.Point)));
               pen.Dispose();
               pen = new Pen(System.Drawing.Color.White, 6);
               pen.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
               pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
               pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
               pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
               pen.CompoundArray = new float[] {.2f, .8f};
               gfx.DrawLines(pen, (System.Drawing.Point[])points.ToArray(typeof(System.Drawing.Point)));
            }
            finally
            {
               pen.Dispose();
            }
         }
         else if (points.Count == 1)
         {
            ptCur = (System.Drawing.Point)points[0];
            gfx.FillEllipse(System.Drawing.Brushes.White, ptCur.X - 4, ptCur.Y - 4, 8, 8);
            gfx.DrawEllipse(System.Drawing.Pens.Black, ptCur.X - 4, ptCur.Y - 4, 8, 8);
         }
      }

      private void DrawSelectedCoord(Graphics gfx, CoordProvider coord)
      {
         RectangleF rcCoord = new RectangleF(coord.X - 3.5f, coord.Y - 3.5f, 7f, 7f);
         rcCoord.Offset(m_Layers[m_nCurLayer].CurrentPosition);
         gfx.FillEllipse(Brushes.Red, rcCoord);
         gfx.DrawEllipse(Pens.Black, rcCoord);
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
      #endregion

      #region Event Handlers
      private void DisplayMap_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         m_Layers[m_nCurLayer].ClearInjections();
         m_Layers[m_nCurLayer].InjectCachedSprites();
         m_LayerMouseCoord = new Point(e.X - m_Layers[m_nCurLayer].CurrentPosition.X, e.Y - m_Layers[m_nCurLayer].CurrentPosition.Y);
         switch(GetCurrentMode())
         {
            case CursorMode.PlaceTile:
               int nSel = CurrentTile;
               Point TilePos = TileFromLayerPoint(m_LayerMouseCoord);
               if ((TilePos.X < m_Layers[m_nCurLayer].Columns) && (TilePos.Y < m_Layers[m_nCurLayer].Rows))
               {
                  if (0 != (e.Button & MouseButtons.Left))
                     m_Layers[m_nCurLayer][TilePos.X, TilePos.Y] = nSel;
                  else
                     m_Layers[m_nCurLayer].InjectTile(TilePos.X, TilePos.Y, nSel);
               }
               break;
            case CursorMode.PlaceSprite:
               if (SpriteSelector.CurrentCellIndex < 0)
                  return;
               SpriteProvider sp = (SpriteProvider)m_SpriteProvider[SpriteSelector.CurrentCellIndex];
               int nCount = sp.GetSubFrameCount();
               if ((sp.X != m_LayerMouseCoord.X) || (sp.Y != m_LayerMouseCoord.Y))
               {
                  sp.X = m_LayerMouseCoord.X;
                  sp.Y = m_LayerMouseCoord.Y;
                  grdSprite.Refresh();
               }
               for(int i = 0; i<nCount; i++)
               {
                  m_Layers[m_nCurLayer].InjectFrame(sp.X, sp.Y, sp.Priority, sp.GetSubFrame(i));
               }
               break;
         }
         MapDisplay.Invalidate();
      }

      private void MapDisplay_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         try
         {
            m_LayerMouseCoord = new Point(e.X - m_Layers[m_nCurLayer].CurrentPosition.X, e.Y - m_Layers[m_nCurLayer].CurrentPosition.Y);

            switch(GetCurrentMode())
            {
               case CursorMode.PlaceTile:
                  int nSel = CurrentTile;
                  Point TilePos = TileFromLayerPoint(m_LayerMouseCoord);
                  if ((TilePos.X < m_Layers[m_nCurLayer].Columns) && (TilePos.Y < m_Layers[m_nCurLayer].Rows))
                     m_Layers[m_nCurLayer][TilePos.X, TilePos.Y] = nSel;
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
                     sp.Active, paramNames, sp.ParameterValues);
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
                  }
                  else
                  {
                     grdSprite.SelectedObject = null;
                     lstAvailableSprites.SelectedIndex = -1;
                  }
                  break;
               case CursorMode.AddCoordinate:
                  if (lstPlans.SelectedIndices.Count == 1)
                     ProjectData.AppendPlanCoordinate((ProjectDataset.SpritePlanRow)lstPlans.SelectedItem, m_LayerMouseCoord.X, m_LayerMouseCoord.Y, 0);
                  break;
               case CursorMode.SelectCoordinate:
                  ProjectDataset.CoordinateRow drCoord = GetCoordAtPoint(m_LayerMouseCoord);
                  lstPlanCoords.ClearSelected();
                  if (drCoord == null)
                     lstPlans.ClearSelected();
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
                  }
                  break;
            }
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Error Adding Sprite", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                  m_TileCache.ResetTile( nTileValue, nTileValue % m_Layers[m_nCurLayer].LayerRow.TilesetRow.FramesetRow.GetFrameRows().Length);
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
         Size ScrollBounds = GetScrollBounds();
         if (ScrollBounds != MapDisplay.AutoScrollMinSize)
            MapDisplay.AutoScrollMinSize = ScrollBounds;
         MapDisplay.Device.Clear(ClearFlags.Target, 0, 1.0f, 0);
         MapDisplay.Device.BeginScene();
         for (int i=0; i<m_Layers.Length; i++)
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
            m_Layers[i].Draw(MapDisplay.Device, MapDisplay.ClientSize);
         }
         MapDisplay.Device.EndScene();
         Surface sfc = MapDisplay.Device.GetRenderTarget(0);
         Graphics gfxDx = sfc.GetGraphics();
         try
         {
            switch(GetCurrentMode())
            {
               case CursorMode.SelectSprite:
                  if (grdSprite.SelectedObjects is SpriteProvider[])
                  {
                     foreach(SpriteProvider sp in grdSprite.SelectedObjects)
                     {
                        DrawSpriteSelection(gfxDx, sp);
                     }
                  } 
                  else if(grdSprite.SelectedObject is SpriteProvider)
                  {
                     SpriteProvider sp = (SpriteProvider)grdSprite.SelectedObject;
                     if (sp.IsDataRow)
                     {
                        DrawSpriteSelection(gfxDx, sp);
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
                     foreach(ProjectDataset.SpritePlanRow plan in ProjectData.GetSortedSpritePlans(m_Layers[m_nCurLayer].LayerRow))
                        DrawPath(plan, gfxDx, bIncludeMouse && (plan == lstPlans.SelectedItem));
                  else
                     foreach(ProjectDataset.SpritePlanRow plan in lstPlans.SelectedItems)
                        DrawPath(plan, gfxDx, bIncludeMouse);
                  if (grdPlan.SelectedObjects is CoordProvider[])
                     foreach(CoordProvider coord in grdPlan.SelectedObjects)
                        DrawSelectedCoord(gfxDx, coord);
                  else if (grdPlan.SelectedObject is CoordProvider)
                     DrawSelectedCoord(gfxDx, (CoordProvider)grdPlan.SelectedObject);
               }
                  break;
            }
         }
         finally
         {
            sfc.ReleaseGraphics();
         }
         MapDisplay.Device.Present();
      }

      private void cboCategory_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (cboCategory.SelectedItem is ProjectDataset.CategoryRow)
            LoadTileSelector((ProjectDataset.CategoryRow)cboCategory.SelectedItem);
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
                     m_ChangingName = true;
                     lstAvailableSprites.Items[i] = e.Row; // Force refresh;
                     m_ChangingName = false;
                  }
            }
         }
      }

      private void dataMonitor_SpriteRowDeleted(object sender, SGDK2.ProjectDataset.SpriteRowChangeEvent e)
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
         if (m_ReflectingSelection || m_ChangingName)
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
            if (e.Row.HasVersion(DataRowVersion.Current) && (e.Row[ProjectData.SpritePlan.NameColumn, DataRowVersion.Current].ToString().CompareTo(e.Row.Name) != 0))
            {
               m_ChangingName = true;
               lstPlans.Items[lstPlans.Items.IndexOf(e.Row)] = e.Row;
               m_ChangingName = false;
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
            int i=1;
            string PlanName;
            do
            {
               PlanName = m_Layers[m_nCurLayer].LayerRow.MapRow.Name + " Plan " + i++.ToString();
            } while (null != ProjectData.GetSpritePlan(m_Layers[m_nCurLayer].LayerRow, PlanName));
            ProjectData.AddSpritePlan(m_Layers[m_nCurLayer].LayerRow, PlanName, 1);
         }
         else if (e.Button == tbbDeletePlan)
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
               for (int i = 0; i < lstPlans.Items.Count;)
                  if (lstPlans.GetSelected(i))
                     ((ProjectDataset.SpritePlanRow)lstPlans.Items[i]).Delete();
                  else
                     i++;
            }
         }
      }

      private void lstPlans_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (m_ChangingName)
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
      }
      #endregion
   }
}
