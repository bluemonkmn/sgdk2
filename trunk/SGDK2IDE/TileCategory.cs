using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace SGDK2
{
	/// <summary>
	/// Summary description for TileCategory.
	/// </summary>
   public class frmTileCategory : System.Windows.Forms.Form
   {
      #region Embedded classes
      class TileProvider : IProvideFrame
      {
         private readonly TileCache m_TileCache;
         private readonly int m_nTileIndex;
         // Which animation frame of the tile is referenced?
         private readonly short m_nFrameIndex;
         private bool bSelected;

         public TileProvider(TileCache tileCache, int nTileIndex) : this(tileCache, nTileIndex, -1)
         {
         }

         public TileProvider(TileCache tileCache, int nTileIndex, short nFrameIndex)
         {
            m_TileCache = tileCache;
            m_nTileIndex = nTileIndex;
            m_nFrameIndex = nFrameIndex;
         }

         #region IProvideFrame Members

         public int FrameIndex
         {
            get
            {
               if (m_TileCache[m_nTileIndex].Length > 0)
                  return m_TileCache[m_nTileIndex][0];
               else
                  return 0;
            }
         }

         // Return an array of sub-frames within the current tile animation frame
         public int[] FrameIndexes
         {
            get
            {
               if (IsFrame)
                  return m_TileCache[m_nTileIndex, m_nFrameIndex];
               else
                  return m_TileCache[m_nTileIndex];
            }
         }

         public bool IsSelected
         {
            get
            {
               return bSelected;
            }
            set
            {
               bSelected = value;
            }
         }
         #endregion

         public int TileIndex
         {
            get
            {
               return m_nTileIndex;
            }
         }

         public short TileFrameIndex
         {
            get
            {
               return m_nFrameIndex;
            }
         }

         public bool IsFrame
         {
            get
            {
               return m_nFrameIndex >= 0;
            }
         }
      }
      #endregion

      #region Non-control members
      private ProjectDataset.CategoryRow m_Category;
      private TileCache m_Tiles;
      // Provides frameset info to the TilesetTiles GraphicBrowser
      private FrameList m_TileProvider;
      // Provides frameset info to the CategoryFrames GraphicBrowser
      private FrameList m_CategoryProvider;
      // Provides frameset info to the TileFrames GraphicBrowser
      private FrameList m_TileFrameProvider;
      #endregion

      #region Form Designer Members
      private SGDK2.GraphicBrowser TilesetTiles;
      private System.Windows.Forms.Panel pnlParams;
      private System.Windows.Forms.TextBox txtName;
      private System.Windows.Forms.Label lblName;
      private System.Windows.Forms.Splitter splitterCategory;
      private System.Windows.Forms.MainMenu mnuTileCategory;
      private System.Windows.Forms.MenuItem mnuTiles;
      private System.Windows.Forms.MenuItem mnuRemove;
      private System.Windows.Forms.MenuItem mnuAdd;
      private SGDK2.DataChangeNotifier dataMonitor;
      private System.Windows.Forms.CheckBox chkLimitIncludedFrames;
      private SGDK2.GraphicBrowser CategoryFrames;
      private SGDK2.GraphicBrowser TileFrames;
      private System.Windows.Forms.Splitter splitterTileset;
      private System.Windows.Forms.Label lblAvailableTiles;
      private System.Windows.Forms.StatusBar status;
      private System.Windows.Forms.StatusBarPanel stpTileIndexLabel;
      private System.Windows.Forms.StatusBarPanel stpFrameIndexLabel;
      private System.Windows.Forms.StatusBarPanel stpTileIndex;
      private System.Windows.Forms.StatusBarPanel stpFrameIndex;
      private System.ComponentModel.IContainer components;
      #endregion

      #region Initialization and clean-up
      public frmTileCategory(ProjectDataset.TilesetRow parent)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         int iNew;
         for (iNew = 1; ProjectData.GetCategory(parent.Name, "New Category " + iNew.ToString()) != null; iNew++)
            ;
         m_Category = ProjectData.AddCategoryRow(parent, "New Category " + iNew.ToString());
         txtName.Text = m_Category.Name;
         ProjectData.AcceptChanges();
         InitializeTiles();
      }

      public frmTileCategory(ProjectDataset.CategoryRow row)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         m_Category = row;
         txtName.Text = row.Name;
         InitializeTiles();
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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmTileCategory));
         this.TileFrames = new SGDK2.GraphicBrowser();
         this.TilesetTiles = new SGDK2.GraphicBrowser();
         this.pnlParams = new System.Windows.Forms.Panel();
         this.txtName = new System.Windows.Forms.TextBox();
         this.lblName = new System.Windows.Forms.Label();
         this.splitterCategory = new System.Windows.Forms.Splitter();
         this.mnuTileCategory = new System.Windows.Forms.MainMenu();
         this.mnuTiles = new System.Windows.Forms.MenuItem();
         this.mnuRemove = new System.Windows.Forms.MenuItem();
         this.mnuAdd = new System.Windows.Forms.MenuItem();
         this.dataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.chkLimitIncludedFrames = new System.Windows.Forms.CheckBox();
         this.splitterTileset = new System.Windows.Forms.Splitter();
         this.CategoryFrames = new SGDK2.GraphicBrowser();
         this.lblAvailableTiles = new System.Windows.Forms.Label();
         this.status = new System.Windows.Forms.StatusBar();
         this.stpTileIndexLabel = new System.Windows.Forms.StatusBarPanel();
         this.stpTileIndex = new System.Windows.Forms.StatusBarPanel();
         this.stpFrameIndexLabel = new System.Windows.Forms.StatusBarPanel();
         this.stpFrameIndex = new System.Windows.Forms.StatusBarPanel();
         this.pnlParams.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.stpTileIndexLabel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.stpTileIndex)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.stpFrameIndexLabel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.stpFrameIndex)).BeginInit();
         this.SuspendLayout();
         // 
         // TileFrames
         // 
         this.TileFrames.AllowDrop = true;
         this.TileFrames.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.TileFrames.CellPadding = new System.Drawing.Size(2, 2);
         this.TileFrames.CellSize = new System.Drawing.Size(0, 0);
         this.TileFrames.CurrentCellIndex = -1;
         this.TileFrames.Dock = System.Windows.Forms.DockStyle.Top;
         this.TileFrames.Frameset = null;
         this.TileFrames.FramesToDisplay = null;
         this.TileFrames.GraphicSheet = null;
         this.TileFrames.IsOrdered = false;
         this.TileFrames.Location = new System.Drawing.Point(0, 165);
         this.TileFrames.Name = "TileFrames";
         this.TileFrames.SheetImage = null;
         this.TileFrames.Size = new System.Drawing.Size(392, 96);
         this.TileFrames.TabIndex = 1;
         this.TileFrames.Visible = false;
         this.TileFrames.CurrentCellChanged += new System.EventHandler(this.TileFrames_CurrentCellChanged);
         // 
         // TilesetTiles
         // 
         this.TilesetTiles.AllowDrop = true;
         this.TilesetTiles.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.TilesetTiles.CellPadding = new System.Drawing.Size(2, 2);
         this.TilesetTiles.CellSize = new System.Drawing.Size(0, 0);
         this.TilesetTiles.CurrentCellIndex = -1;
         this.TilesetTiles.Dock = System.Windows.Forms.DockStyle.Top;
         this.TilesetTiles.Frameset = null;
         this.TilesetTiles.FramesToDisplay = null;
         this.TilesetTiles.GraphicSheet = null;
         this.TilesetTiles.IsOrdered = false;
         this.TilesetTiles.Location = new System.Drawing.Point(0, 48);
         this.TilesetTiles.Name = "TilesetTiles";
         this.TilesetTiles.SheetImage = null;
         this.TilesetTiles.Size = new System.Drawing.Size(392, 96);
         this.TilesetTiles.TabIndex = 3;
         this.TilesetTiles.CurrentCellChanged += new System.EventHandler(this.TilesetTiles_CurrentCellChanged);
         this.TilesetTiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.TilesetTiles_DragDrop);
         this.TilesetTiles.DragOver += new System.Windows.Forms.DragEventHandler(this.TilesetTiles_DragOver);
         // 
         // pnlParams
         // 
         this.pnlParams.Controls.Add(this.txtName);
         this.pnlParams.Controls.Add(this.lblName);
         this.pnlParams.Dock = System.Windows.Forms.DockStyle.Top;
         this.pnlParams.Location = new System.Drawing.Point(0, 0);
         this.pnlParams.Name = "pnlParams";
         this.pnlParams.Size = new System.Drawing.Size(392, 32);
         this.pnlParams.TabIndex = 0;
         // 
         // txtName
         // 
         this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtName.Location = new System.Drawing.Point(56, 8);
         this.txtName.Name = "txtName";
         this.txtName.Size = new System.Drawing.Size(328, 20);
         this.txtName.TabIndex = 2;
         this.txtName.Text = "";
         this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
         this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
         // 
         // lblName
         // 
         this.lblName.Location = new System.Drawing.Point(8, 8);
         this.lblName.Name = "lblName";
         this.lblName.Size = new System.Drawing.Size(48, 20);
         this.lblName.TabIndex = 1;
         this.lblName.Text = "Name:";
         this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // splitterCategory
         // 
         this.splitterCategory.Dock = System.Windows.Forms.DockStyle.Top;
         this.splitterCategory.Location = new System.Drawing.Point(0, 261);
         this.splitterCategory.Name = "splitterCategory";
         this.splitterCategory.Size = new System.Drawing.Size(392, 5);
         this.splitterCategory.TabIndex = 2;
         this.splitterCategory.TabStop = false;
         this.splitterCategory.Visible = false;
         // 
         // mnuTileCategory
         // 
         this.mnuTileCategory.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                        this.mnuTiles});
         // 
         // mnuTiles
         // 
         this.mnuTiles.Index = 0;
         this.mnuTiles.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                 this.mnuRemove,
                                                                                 this.mnuAdd});
         this.mnuTiles.Text = "&Tiles";
         // 
         // mnuRemove
         // 
         this.mnuRemove.Index = 0;
         this.mnuRemove.Text = "&Remove from category";
         this.mnuRemove.Click += new System.EventHandler(this.mnuRemove_Click);
         // 
         // mnuAdd
         // 
         this.mnuAdd.Index = 1;
         this.mnuAdd.Text = "&Add to category";
         this.mnuAdd.Click += new System.EventHandler(this.mnuAdd_Click);
         // 
         // dataMonitor
         // 
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         this.dataMonitor.TileFrameRowDeleted += new SGDK2.ProjectDataset.TileFrameRowChangeEventHandler(this.dataMonitor_TileFrameRowChanged);
         this.dataMonitor.CategoryRowDeleted += new SGDK2.ProjectDataset.CategoryRowChangeEventHandler(this.dataMonitor_CategoryRowDeleted);
         this.dataMonitor.TileRowChanged += new SGDK2.ProjectDataset.TileRowChangeEventHandler(this.dataMonitor_TileRowChanged);
         this.dataMonitor.TileFrameRowChanged += new SGDK2.ProjectDataset.TileFrameRowChangeEventHandler(this.dataMonitor_TileFrameRowChanged);
         this.dataMonitor.CategoryTileRowChanged += new SGDK2.ProjectDataset.CategoryTileRowChangeEventHandler(this.dataMonitor_CategoryTileRowChanged);
         this.dataMonitor.CategoryFrameRowChanged += new SGDK2.ProjectDataset.CategoryFrameRowChangeEventHandler(this.dataMonitor_CategoryFrameRowChanged);
         this.dataMonitor.CategoryTileRowDeleted += new SGDK2.ProjectDataset.CategoryTileRowChangeEventHandler(this.dataMonitor_CategoryTileRowDeleted);
         this.dataMonitor.CategoryFrameRowDeleted += new SGDK2.ProjectDataset.CategoryFrameRowChangeEventHandler(this.dataMonitor_CategoryFrameRowDeleted);
         this.dataMonitor.TileRowDeleted += new SGDK2.ProjectDataset.TileRowChangeEventHandler(this.dataMonitor_TileRowChanged);
         // 
         // chkLimitIncludedFrames
         // 
         this.chkLimitIncludedFrames.Dock = System.Windows.Forms.DockStyle.Top;
         this.chkLimitIncludedFrames.Enabled = false;
         this.chkLimitIncludedFrames.Location = new System.Drawing.Point(0, 149);
         this.chkLimitIncludedFrames.Name = "chkLimitIncludedFrames";
         this.chkLimitIncludedFrames.Size = new System.Drawing.Size(392, 16);
         this.chkLimitIncludedFrames.TabIndex = 0;
         this.chkLimitIncludedFrames.Text = "I&nclude only some frames from selected tile";
         this.chkLimitIncludedFrames.CheckedChanged += new System.EventHandler(this.chkLimitIncludedFrames_CheckedChanged);
         // 
         // splitterTileset
         // 
         this.splitterTileset.Dock = System.Windows.Forms.DockStyle.Top;
         this.splitterTileset.Location = new System.Drawing.Point(0, 144);
         this.splitterTileset.Name = "splitterTileset";
         this.splitterTileset.Size = new System.Drawing.Size(392, 5);
         this.splitterTileset.TabIndex = 4;
         this.splitterTileset.TabStop = false;
         // 
         // CategoryFrames
         // 
         this.CategoryFrames.AllowDrop = true;
         this.CategoryFrames.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.CategoryFrames.CellPadding = new System.Drawing.Size(0, 0);
         this.CategoryFrames.CellSize = new System.Drawing.Size(0, 0);
         this.CategoryFrames.CurrentCellIndex = -1;
         this.CategoryFrames.Dock = System.Windows.Forms.DockStyle.Fill;
         this.CategoryFrames.Frameset = null;
         this.CategoryFrames.FramesToDisplay = null;
         this.CategoryFrames.GraphicSheet = null;
         this.CategoryFrames.IsOrdered = false;
         this.CategoryFrames.Location = new System.Drawing.Point(0, 266);
         this.CategoryFrames.Name = "CategoryFrames";
         this.CategoryFrames.SheetImage = null;
         this.CategoryFrames.Size = new System.Drawing.Size(392, 119);
         this.CategoryFrames.TabIndex = 5;
         this.CategoryFrames.CurrentCellChanged += new System.EventHandler(this.CategoryFrames_CurrentCellChanged);
         this.CategoryFrames.DragDrop += new System.Windows.Forms.DragEventHandler(this.CategoryFrames_DragDrop);
         this.CategoryFrames.DragOver += new System.Windows.Forms.DragEventHandler(this.CategoryFrames_DragOver);
         // 
         // lblAvailableTiles
         // 
         this.lblAvailableTiles.Dock = System.Windows.Forms.DockStyle.Top;
         this.lblAvailableTiles.Location = new System.Drawing.Point(0, 32);
         this.lblAvailableTiles.Name = "lblAvailableTiles";
         this.lblAvailableTiles.Size = new System.Drawing.Size(392, 16);
         this.lblAvailableTiles.TabIndex = 6;
         this.lblAvailableTiles.Text = "Tiles Available in Tileset";
         // 
         // status
         // 
         this.status.Location = new System.Drawing.Point(0, 385);
         this.status.Name = "status";
         this.status.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
                                                                                  this.stpTileIndexLabel,
                                                                                  this.stpTileIndex,
                                                                                  this.stpFrameIndexLabel,
                                                                                  this.stpFrameIndex});
         this.status.ShowPanels = true;
         this.status.Size = new System.Drawing.Size(392, 16);
         this.status.TabIndex = 7;
         // 
         // stpTileIndexLabel
         // 
         this.stpTileIndexLabel.Alignment = System.Windows.Forms.HorizontalAlignment.Right;
         this.stpTileIndexLabel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
         this.stpTileIndexLabel.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
         this.stpTileIndexLabel.Text = "Tile Index:";
         this.stpTileIndexLabel.Width = 66;
         // 
         // stpTileIndex
         // 
         this.stpTileIndex.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
         this.stpTileIndex.Text = "N/A";
         this.stpTileIndex.Width = 33;
         // 
         // stpFrameIndexLabel
         // 
         this.stpFrameIndexLabel.Alignment = System.Windows.Forms.HorizontalAlignment.Right;
         this.stpFrameIndexLabel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
         this.stpFrameIndexLabel.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
         this.stpFrameIndexLabel.Text = "Frame Index:";
         this.stpFrameIndexLabel.Width = 81;
         // 
         // stpFrameIndex
         // 
         this.stpFrameIndex.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
         this.stpFrameIndex.Text = "N/A";
         this.stpFrameIndex.Width = 33;
         // 
         // frmTileCategory
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(392, 401);
         this.Controls.Add(this.CategoryFrames);
         this.Controls.Add(this.status);
         this.Controls.Add(this.splitterCategory);
         this.Controls.Add(this.TileFrames);
         this.Controls.Add(this.chkLimitIncludedFrames);
         this.Controls.Add(this.splitterTileset);
         this.Controls.Add(this.TilesetTiles);
         this.Controls.Add(this.lblAvailableTiles);
         this.Controls.Add(this.pnlParams);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Menu = this.mnuTileCategory;
         this.Name = "frmTileCategory";
         this.Text = "Tile Category";
         this.pnlParams.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.stpTileIndexLabel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.stpTileIndex)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.stpFrameIndexLabel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.stpFrameIndex)).EndInit();
         this.ResumeLayout(false);

      }
      #endregion

      #region Private Methods
      private void InitializeTiles()
      {
         m_Tiles = new TileCache(m_Category.TilesetRow);
         m_TileProvider = new FrameList();
         for (int i = 0; i < m_Tiles.Count; i++)
            m_TileProvider.Add(new TileProvider(m_Tiles, i));
         TilesetTiles.Frameset = m_Category.TilesetRow.FramesetRow;
         TilesetTiles.FramesToDisplay = m_TileProvider;

         m_CategoryProvider = new FrameList();
         foreach(ProjectDataset.CategoryTileRow cr in m_Category.GetCategoryTileRows())
         {
            ProjectDataset.CategoryFrameRow[] frameList = cr.GetCategoryFrameRows();
            if (frameList.Length > 0)
            {
               foreach (ProjectDataset.CategoryFrameRow cfr in frameList)
                  m_CategoryProvider.Add(new TileProvider(m_Tiles, cr.TileValue, cfr.Frame));
            }
            else
               m_CategoryProvider.Add(new TileProvider(m_Tiles, cr.TileValue));
         }
         CategoryFrames.Frameset = m_Category.TilesetRow.FramesetRow;
         CategoryFrames.FramesToDisplay = m_CategoryProvider;
      }

      private void AddSelectionToCategory()
      {
         if (TilesetTiles.CurrentCellIndex < 0)
         {
            MessageBox.Show(this, "Select a tile before performing this action", "Add to Category", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         if (chkLimitIncludedFrames.Checked)
         {
            if (TileFrames.GetSelectedCellCount() == 0)
            {
               MessageBox.Show(this, "Select some frames from the tile before performing this action", "Add Frames to Category", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return;
            }
            for (short i=0; i < TileFrames.CellCount; i++)
            {
               if ((TileFrames.Selected[i]) && (null==ProjectData.GetCategoryFrameRow(
                  m_Category.TilesetRow.Name, m_Category.Name, TilesetTiles.CurrentCellIndex, i)))
               {
                  ProjectData.AddCategoryFrameRow(m_Category.TilesetRow.Name, m_Category.Name,
                     ((TileProvider)m_TileFrameProvider[i]).TileIndex,
                     ((TileProvider)m_TileFrameProvider[i]).TileFrameIndex);
               }
            }
         }
         else
         {
            for (int i=0; i < TilesetTiles.CellCount; i++)
            {
               if (TilesetTiles.Selected[i])
               {
                  if (null==ProjectData.GetCategoryTileRow(m_Category.TilesetRow.Name, m_Category.Name, i))
                     ProjectData.AddCategoryTileRow(m_Category.TilesetRow.Name, m_Category.Name, ((TileProvider)m_TileProvider[i]).TileIndex);
                  else
                     ProjectData.ResetCategoryFrames(m_Category.TilesetRow.Name, m_Category.Name, ((TileProvider)m_TileProvider[i]).TileIndex);
               }
            }
         }
         ProjectData.AcceptChanges();
      }

      private void RemoveSelectionFromCategory()
      {
         for (int i = 0; i < CategoryFrames.CellCount; i++)
         {
            if (CategoryFrames.Selected[i])
            {
               if (((TileProvider)m_CategoryProvider[i]).IsFrame)
               {
                  ProjectData.DeleteCategoryFrameRow(m_Category.TilesetRow.Name, m_Category.Name,
                     ((TileProvider)m_CategoryProvider[i]).TileIndex, 
                     ((TileProvider)m_CategoryProvider[i]).TileFrameIndex);
               }
               else
               {
                  ProjectData.GetCategoryTileRow(m_Category.TilesetRow.Name, m_Category.Name,
                     ((TileProvider)m_CategoryProvider[i]).TileIndex).Delete();
               }
            }
         }
         ProjectData.AcceptChanges();
      }

      private void LoadTileFrames(int nTileIndex)
      {
         m_TileFrameProvider = new FrameList();
         for (short i = 0; i < m_Tiles.GetFrameCount(nTileIndex); i++)
         {
            m_TileFrameProvider.Add(new TileProvider(m_Tiles, nTileIndex, i));
         }
         TileFrames.Frameset = m_Category.TilesetRow.FramesetRow;
         TileFrames.FramesToDisplay = m_TileFrameProvider;
      }
      #endregion

      #region Event Handlers
      private void txtName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         string sValid = ProjectData.ValidateName(txtName.Text);

         if (sValid != null)
         {
            if (DialogResult.Cancel == MessageBox.Show(this, sValid, "Category Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtName.Text = m_Category.Name;
            e.Cancel = true;
         }
         ProjectDataset.CategoryRow cr = ProjectData.GetCategory(m_Category.TilesetRow.Name, txtName.Text);
         if ((null != cr) && (m_Category != cr))
         {
            if (DialogResult.Cancel == MessageBox.Show(this, txtName.Text + " already exists in this tileset", "Category Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtName.Text = m_Category.Name;
            e.Cancel = true;
         }         
      }

      private void txtName_Validated(object sender, System.EventArgs e)
      {
         if (m_Category.Name != txtName.Text)
         {
            m_Category.Name = txtName.Text;
            ProjectData.AcceptChanges();
         }
      }

      private void TilesetTiles_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
      {
         RemoveSelectionFromCategory();
      }

      private void TilesetTiles_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
      {
         GraphicBrowser gb = e.Data.GetData(typeof(GraphicBrowser)) as GraphicBrowser;
         if ((gb != null) && (gb == CategoryFrames))
            e.Effect = DragDropEffects.Copy;
         else
            e.Effect = DragDropEffects.None;      
      }

      private void mnuRemove_Click(object sender, System.EventArgs e)
      {
         if (CategoryFrames.GetSelectedCellCount() == 0)
         {
            MessageBox.Show(this, "Category tiles must be selected to remove tiles from the category", "Remove Tiles From Category", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         RemoveSelectionFromCategory();
      }

      private void mnuAdd_Click(object sender, System.EventArgs e)
      {
         if (TilesetTiles.GetSelectedCellCount() == 0)
         {
            MessageBox.Show(this, "Tileset tiles must be selected to add tiles to the category", "Add Tiles To Category", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         AddSelectionToCategory();
      }

      private void dataMonitor_CategoryTileRowChanged(object sender, SGDK2.ProjectDataset.CategoryTileRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            if (e.Row.CategoryRowParent == m_Category)
            {
               m_CategoryProvider.Add(new TileProvider(m_Tiles, e.Row.TileValue));
               CategoryFrames.Invalidate();
            }
         }
      }

      private void dataMonitor_CategoryTileRowDeleted(object sender, SGDK2.ProjectDataset.CategoryTileRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Delete)
         {
            if ((String.Compare(e.Row[ProjectData.CategoryTile.CategoryColumn, DataRowVersion.Original].ToString(),
               m_Category.Name)==0) && (String.Compare(e.Row[ProjectData.CategoryTile.TilesetColumn,
               DataRowVersion.Original].ToString(), m_Category.TilesetRow.Name) == 0))
            {
               int nDeleteValue = (int)(e.Row[ProjectData.CategoryTile.TileValueColumn, DataRowVersion.Original]);
               for (int i = 0; i < m_CategoryProvider.Count; i++)
                  if (((TileProvider)m_CategoryProvider[i]).TileIndex == nDeleteValue)
                  {
                     m_CategoryProvider.RemoveAt(i);
                     CategoryFrames.Invalidate();
                     return;
                  }
            }
         }      
      }

      private void dataMonitor_TileRowChanged(object sender, SGDK2.ProjectDataset.TileRowChangeEvent e)
      {
         ProjectDataset.TileRow tr =  (ProjectDataset.TileRow)e.Row;
         switch(e.Action)
         {
            case DataRowAction.Add:
            case DataRowAction.Change:
               if (tr.TilesetRow == this.m_Category.TilesetRow)
               {
                  if (tr.TileValue >= m_Tiles.Count)
                     m_Tiles = new TileCache(tr.TilesetRow);
                  else
                     m_Tiles.RefreshTile(tr);
                  TilesetTiles.Invalidate();
                  CategoryFrames.Invalidate();
               }
               break;
            case DataRowAction.Delete:
               if (string.Compare((string)e.Row["Name", DataRowVersion.Original], m_Category.TilesetRow.Name) == 0)
               {
                  int nTileValue = (int)tr["TileValue", DataRowVersion.Original];
                  m_Tiles.ResetTile(nTileValue, nTileValue % m_Category.TilesetRow.FramesetRow.GetFrameRows().Length);
                  TilesetTiles.Invalidate();
                  CategoryFrames.Invalidate();
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
               if (tfr.TileRowParent.TilesetRow == m_Category.TilesetRow)
               {
                  m_Tiles.RefreshTile(tfr.TileRowParent);
                  TilesetTiles.Invalidate();
                  CategoryFrames.Invalidate();
               }
               break;
            case DataRowAction.Delete:
               if (string.Compare((string)tfr["Name", DataRowVersion.Original], m_Category.TilesetRow.Name) == 0)
               {
                  ProjectDataset.TileRow tr = ProjectData.Tile.FindByNameTileValue(
                     m_Category.TilesetRow.Name, (int)tfr["TileValue", DataRowVersion.Original]);
                  m_Tiles.RefreshTile(tr);
                  TilesetTiles.Invalidate();
                  CategoryFrames.Invalidate();
               }
               break;
         }
      }

      private void dataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void dataMonitor_CategoryRowDeleted(object sender, SGDK2.ProjectDataset.CategoryRowChangeEvent e)
      {
         if (e.Row == m_Category)
            this.Close();
      }

      private void chkLimitIncludedFrames_CheckedChanged(object sender, System.EventArgs e)
      {
         TileFrames.Visible = splitterCategory.Visible = chkLimitIncludedFrames.Checked;
         if (TileFrames.Visible)
            LoadTileFrames(TilesetTiles.CurrentCellIndex);
      }

      private void TilesetTiles_CurrentCellChanged(object sender, System.EventArgs e)
      {
         stpTileIndex.Text = TilesetTiles.CurrentCellIndex.ToString();
         stpFrameIndex.Text = "N/A";
         if ((TilesetTiles.CurrentCellIndex < 0) || (m_Tiles.GetFrameCount(TilesetTiles.CurrentCellIndex) <=1))
            chkLimitIncludedFrames.Enabled = chkLimitIncludedFrames.Checked = false;
         else
            chkLimitIncludedFrames.Enabled = true;
         if (chkLimitIncludedFrames.Checked)
         {
            LoadTileFrames(TilesetTiles.CurrentCellIndex);
         }
      }

      private void TileFrames_CurrentCellChanged(object sender, System.EventArgs e)
      {
         if (TileFrames.CurrentCellIndex >= 0)
         {
            stpTileIndex.Text = ((TileProvider)m_TileFrameProvider[TileFrames.CurrentCellIndex]).TileIndex.ToString();
            stpFrameIndex.Text = ((TileProvider)m_TileFrameProvider[TileFrames.CurrentCellIndex]).TileFrameIndex.ToString();
         }
      }

      private void CategoryFrames_CurrentCellChanged(object sender, System.EventArgs e)
      {
         if (CategoryFrames.CurrentCellIndex >= 0)
         {
            stpTileIndex.Text = ((TileProvider)m_CategoryProvider[CategoryFrames.CurrentCellIndex]).TileIndex.ToString();
            if (((TileProvider)m_CategoryProvider[CategoryFrames.CurrentCellIndex]).IsFrame)
               stpFrameIndex.Text = ((TileProvider)m_CategoryProvider[CategoryFrames.CurrentCellIndex]).TileFrameIndex.ToString();
            else
               stpFrameIndex.Text = "Any";
         }
      }

      private void dataMonitor_CategoryFrameRowChanged(object sender, SGDK2.ProjectDataset.CategoryFrameRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            if (e.Row.CategoryTileRowParent.CategoryRowParent == m_Category)
            {
               for (int i = 0; i < m_CategoryProvider.Count; i++)
               {
                  if ((!((TileProvider)m_CategoryProvider[i]).IsFrame) &&
                     (((TileProvider)m_CategoryProvider[i]).TileIndex == e.Row.CategoryTileRowParent.TileValue))
                  {
                     m_CategoryProvider.RemoveAt(i);
                     break;
                  }
               }
               m_CategoryProvider.Add(new TileProvider(m_Tiles, e.Row.CategoryTileRowParent.TileValue, e.Row.Frame));
               CategoryFrames.Invalidate();
            }
         }      
      }

      private void dataMonitor_CategoryFrameRowDeleted(object sender, SGDK2.ProjectDataset.CategoryFrameRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Delete)
         {
            string Tileset = e.Row[ProjectData.CategoryFrame.TilesetColumn, DataRowVersion.Original].ToString();
            string Category = e.Row[ProjectData.CategoryFrame.CategoryColumn, DataRowVersion.Original].ToString();

            if ((String.Compare(Category, m_Category.Name)==0) &&
                (String.Compare(Tileset, m_Category.TilesetRow.Name) == 0))
            {
               int nDeleteTile = (int)(e.Row[ProjectData.CategoryFrame.TileValueColumn, DataRowVersion.Original]);
               short nDeleteFrame = (short)(e.Row[ProjectData.CategoryFrame.FrameColumn, DataRowVersion.Original]);
               for (int i = 0; i < m_CategoryProvider.Count; i++)
               {
                  if ((((TileProvider)m_CategoryProvider[i]).TileIndex == nDeleteTile) &&
                     (((TileProvider)m_CategoryProvider[i]).TileFrameIndex == nDeleteFrame))
                  {
                     m_CategoryProvider.RemoveAt(i);
                     // When all of a tile's frames are removed without removing the tile itself,
                     // must put the tile back in with all frames.
                     ProjectDataset.CategoryTileRow parent = ProjectData.GetCategoryTileRow(Tileset, Category, nDeleteTile);
                     if (parent.GetCategoryFrameRows().Length == 0)
                     {
                        m_CategoryProvider.Add(new TileProvider(m_Tiles, parent.TileValue));
                     }
                     CategoryFrames.Invalidate();
                     return;
                  }
               }
            }
         }            
      }

      private void CategoryFrames_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
      {
         if ((e.Data.GetData(typeof(GraphicBrowser)) as GraphicBrowser == TilesetTiles) &&
            chkLimitIncludedFrames.Checked)
            chkLimitIncludedFrames.Checked = false;
         AddSelectionToCategory();
      }

      private void CategoryFrames_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
      {
         GraphicBrowser gb = e.Data.GetData(typeof(GraphicBrowser)) as GraphicBrowser;
         if ((gb != null) && ((gb == TilesetTiles) || (gb == TileFrames)))
            e.Effect = DragDropEffects.Copy;
         else
            e.Effect = DragDropEffects.None;      
      }
      #endregion
   }
}
