/*
 * Scrolling Game Development Kit 2.0
 *
 * Copyright © 2000 - 2005 Benjamin Marty <BlueMonkMN@email.com>
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
      private System.Windows.Forms.Splitter SpriteSplitter;
      #endregion

      #region Embedded classes
      private enum CursorMode
      {
         None,
         PlaceTile,
         PlaceSprite,
         SelectSprite
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
         this.SpriteSelector = new SGDK2.GraphicBrowser();
         this.SpriteSplitter = new System.Windows.Forms.Splitter();
         this.cboSpriteCategory = new System.Windows.Forms.ComboBox();
         this.grdSprite = new System.Windows.Forms.PropertyGrid();
         this.rdoAddSprites = new System.Windows.Forms.RadioButton();
         this.rdoSelectSprites = new System.Windows.Forms.RadioButton();
         this.StatusBar = new System.Windows.Forms.StatusBar();
         this.sbpStatus = new System.Windows.Forms.StatusBarPanel();
         this.sbpCtrl = new System.Windows.Forms.StatusBarPanel();
         this.tabSelector.SuspendLayout();
         this.tabTiles.SuspendLayout();
         this.pnlTiles.SuspendLayout();
         this.tabSprites.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.sbpStatus)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpCtrl)).BeginInit();
         this.SuspendLayout();
         // 
         // MapSplitter
         // 
         this.MapSplitter.Location = new System.Drawing.Point(144, 0);
         this.MapSplitter.Name = "MapSplitter";
         this.MapSplitter.Size = new System.Drawing.Size(5, 473);
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
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         this.dataMonitor.TileFrameRowDeleted += new SGDK2.ProjectDataset.TileFrameRowChangeEventHandler(this.dataMonitor_TileFrameRowChanged);
         this.dataMonitor.TileRowChanged += new SGDK2.ProjectDataset.TileRowChangeEventHandler(this.dataMonitor_TileRowChanged);
         this.dataMonitor.SpriteRowChanged += new SGDK2.ProjectDataset.SpriteRowChangeEventHandler(this.dataMonitor_SpriteRowChanged);
         this.dataMonitor.TileFrameRowChanged += new SGDK2.ProjectDataset.TileFrameRowChangeEventHandler(this.dataMonitor_TileFrameRowChanged);
         this.dataMonitor.LayerRowDeleted += new SGDK2.ProjectDataset.LayerRowChangeEventHandler(this.dataMonitor_LayerRowDeleted);
         this.dataMonitor.SpriteRowDeleted += new SGDK2.ProjectDataset.SpriteRowChangeEventHandler(this.dataMonitor_SpriteRowDeleted);
         this.dataMonitor.MapRowDeleted += new SGDK2.ProjectDataset.MapRowChangeEventHandler(this.dataMonitor_MapRowDeleted);
         this.dataMonitor.TileRowDeleted += new SGDK2.ProjectDataset.TileRowChangeEventHandler(this.dataMonitor_TileRowChanged);
         // 
         // tabSelector
         // 
         this.tabSelector.Controls.Add(this.tabTiles);
         this.tabSelector.Controls.Add(this.tabSprites);
         this.tabSelector.Dock = System.Windows.Forms.DockStyle.Left;
         this.tabSelector.Location = new System.Drawing.Point(0, 0);
         this.tabSelector.Name = "tabSelector";
         this.tabSelector.SelectedIndex = 0;
         this.tabSelector.Size = new System.Drawing.Size(144, 473);
         this.tabSelector.TabIndex = 13;
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
         this.tabSprites.Controls.Add(this.SpriteSelector);
         this.tabSprites.Controls.Add(this.SpriteSplitter);
         this.tabSprites.Controls.Add(this.cboSpriteCategory);
         this.tabSprites.Controls.Add(this.grdSprite);
         this.tabSprites.Controls.Add(this.rdoAddSprites);
         this.tabSprites.Controls.Add(this.rdoSelectSprites);
         this.tabSprites.Location = new System.Drawing.Point(4, 22);
         this.tabSprites.Name = "tabSprites";
         this.tabSprites.Size = new System.Drawing.Size(136, 447);
         this.tabSprites.TabIndex = 1;
         this.tabSprites.Text = "Sprites";
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
         this.SpriteSelector.Location = new System.Drawing.Point(0, 69);
         this.SpriteSelector.Name = "SpriteSelector";
         this.SpriteSelector.SheetImage = null;
         this.SpriteSelector.Size = new System.Drawing.Size(136, 229);
         this.SpriteSelector.TabIndex = 2;
         this.SpriteSelector.CurrentCellChanged += new System.EventHandler(this.SpriteSelector_CurrentCellChanged);
         // 
         // SpriteSplitter
         // 
         this.SpriteSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.SpriteSplitter.Location = new System.Drawing.Point(0, 298);
         this.SpriteSplitter.Name = "SpriteSplitter";
         this.SpriteSplitter.Size = new System.Drawing.Size(136, 5);
         this.SpriteSplitter.TabIndex = 3;
         this.SpriteSplitter.TabStop = false;
         // 
         // cboSpriteCategory
         // 
         this.cboSpriteCategory.Dock = System.Windows.Forms.DockStyle.Top;
         this.cboSpriteCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboSpriteCategory.Location = new System.Drawing.Point(0, 48);
         this.cboSpriteCategory.Name = "cboSpriteCategory";
         this.cboSpriteCategory.Size = new System.Drawing.Size(136, 21);
         this.cboSpriteCategory.TabIndex = 1;
         this.cboSpriteCategory.SelectedIndexChanged += new System.EventHandler(this.cboSpriteCategory_SelectedIndexChanged);
         // 
         // grdSprite
         // 
         this.grdSprite.CommandsVisibleIfAvailable = true;
         this.grdSprite.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.grdSprite.HelpVisible = false;
         this.grdSprite.LargeButtons = false;
         this.grdSprite.LineColor = System.Drawing.SystemColors.ScrollBar;
         this.grdSprite.Location = new System.Drawing.Point(0, 303);
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
         this.rdoAddSprites.Appearance = System.Windows.Forms.Appearance.Button;
         this.rdoAddSprites.Dock = System.Windows.Forms.DockStyle.Top;
         this.rdoAddSprites.Location = new System.Drawing.Point(0, 24);
         this.rdoAddSprites.Name = "rdoAddSprites";
         this.rdoAddSprites.Size = new System.Drawing.Size(136, 24);
         this.rdoAddSprites.TabIndex = 5;
         this.rdoAddSprites.Text = "Add Sprites";
         this.rdoAddSprites.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // rdoSelectSprites
         // 
         this.rdoSelectSprites.Appearance = System.Windows.Forms.Appearance.Button;
         this.rdoSelectSprites.Checked = true;
         this.rdoSelectSprites.Dock = System.Windows.Forms.DockStyle.Top;
         this.rdoSelectSprites.Location = new System.Drawing.Point(0, 0);
         this.rdoSelectSprites.Name = "rdoSelectSprites";
         this.rdoSelectSprites.Size = new System.Drawing.Size(136, 24);
         this.rdoSelectSprites.TabIndex = 4;
         this.rdoSelectSprites.TabStop = true;
         this.rdoSelectSprites.Text = "Select Sprites";
         this.rdoSelectSprites.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // StatusBar
         // 
         this.StatusBar.Location = new System.Drawing.Point(0, 473);
         this.StatusBar.Name = "StatusBar";
         this.StatusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
                                                                                     this.sbpStatus,
                                                                                     this.sbpCtrl});
         this.StatusBar.ShowPanels = true;
         this.StatusBar.Size = new System.Drawing.Size(592, 16);
         this.StatusBar.TabIndex = 14;
         this.StatusBar.DrawItem += new System.Windows.Forms.StatusBarDrawItemEventHandler(this.StatusBar_DrawItem);
         // 
         // sbpStatus
         // 
         this.sbpStatus.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
         this.sbpStatus.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
         this.sbpStatus.Width = 532;
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
         this.Controls.Add(this.MapSplitter);
         this.Controls.Add(this.tabSelector);
         this.Controls.Add(this.StatusBar);
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
         foreach(ProjectDataset.SpriteCategoryRow row in ProjectData.SpriteCategory)
         {
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
      #endregion

      #region Event Handlers
      private void DisplayMap_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         m_Layers[m_nCurLayer].ClearInjections();
         m_Layers[m_nCurLayer].InjectCachedSprites();
         Point ptInLayer = new Point(e.X - m_Layers[m_nCurLayer].CurrentPosition.X, e.Y - m_Layers[m_nCurLayer].CurrentPosition.Y);
         switch(GetCurrentMode())
         {
            case CursorMode.PlaceTile:
               int nSel = CurrentTile;
               Point TilePos = TileFromLayerPoint(ptInLayer);
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
               if ((sp.X != ptInLayer.X) || (sp.Y != ptInLayer.Y))
               {
                  sp.X = ptInLayer.X;
                  sp.Y = ptInLayer.Y;
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
            Point ptInLayer = new Point(e.X - m_Layers[m_nCurLayer].CurrentPosition.X, e.Y - m_Layers[m_nCurLayer].CurrentPosition.Y);

            switch(GetCurrentMode())
            {
               case CursorMode.PlaceTile:
                  int nSel = CurrentTile;
                  Point TilePos = TileFromLayerPoint(ptInLayer);
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
                     paramNames, sp.ParameterValues);
                  ProjectData.AcceptChanges();
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
                  SpriteProvider[] arsp = m_Layers[m_nCurLayer].GetSpritesAtPoint(ptInLayer);
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
                        }
                        else if (grdSprite.SelectedObject != null)
                        {
                           SpriteProvider[] all = new SpriteProvider[arsp.Length + 1];
                           all[0] = (SpriteProvider)grdSprite.SelectedObject;
                           arsp.CopyTo(all, 1);
                           grdSprite.SelectedObjects = all;
                        }                     
                     }
                     else
                     {
                        if (arsp.Length > 1)
                           grdSprite.SelectedObjects = arsp;
                        else
                           grdSprite.SelectedObject = arsp[0];
                     }
                  }
                  else
                     grdSprite.SelectedObject = null;
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
               if (string.Compare((string)e.Row["Name", DataRowVersion.Original], m_Layers[m_nCurLayer].LayerRow.TilesetRow.Name) == 0)
               {
                  int nTileValue = (int)tr["TileValue", DataRowVersion.Original];
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
               if (string.Compare((string)tfr["Name", DataRowVersion.Original], m_Layers[m_nCurLayer].LayerRow.TilesetRow.Name) == 0)
               {
                  ProjectDataset.TileRow tr = ProjectData.Tile.FindByNameTileValue(
                     (string)m_Layers[m_nCurLayer].LayerRow.TilesetRow.Name, (int)tfr["TileValue", DataRowVersion.Original]);
                  m_TileCache.RefreshTile(tr);
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
         }
         else if (e.Action == DataRowAction.Change)
         {
            m_Layers[m_nCurLayer].ClearInjections();
            m_Layers[m_nCurLayer].InjectCachedSprites();
            MapDisplay.Invalidate();
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
      #endregion
   }
}
