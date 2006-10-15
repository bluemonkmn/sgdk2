/*
 * Scrolling Game Development Kit 2.0
 *
 * Copyright © 2000 - 2006 Benjamin Marty <BlueMonkMN@email.com>
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
using System.Data;

namespace SGDK2
{
   /// <summary>
   /// Main window for SGDK2 design time environment
   /// </summary>
   public class frmMain : System.Windows.Forms.Form
   {
      #region Non-Control Members
      private System.Collections.Specialized.HybridDictionary m_TreeNodes;
      Int32 m_nPinStatus; // 0 == Pinned, 1 == Unpinned Collapsed, 2 = Unpinned Expanded
      Boolean m_bOverPin = false;
      TreeNode m_ContextNode = null;
      private string m_strProjectPath;
      private Hashtable m_AffectedNodeKeys = new Hashtable();
      #endregion

      #region Windows Form Designer Memebers
      public System.Windows.Forms.ToolBar tbrMain;
      private System.Windows.Forms.TreeView tvwMain;
      private System.Windows.Forms.Splitter splitterMDI;
      private System.Windows.Forms.ImageList imlMain;
      private System.Windows.Forms.MenuItem mnuWindows;
      private System.Windows.Forms.MainMenu mnuMain;
      private System.Windows.Forms.Panel pnlProjectTree;
      private System.Windows.Forms.Label lblProjectTree;
      private System.Windows.Forms.ToolBarButton tbbShowTree;
      private System.Windows.Forms.ToolBarButton tbbOpen;
      private System.Windows.Forms.ToolBarButton tbsTree;
      private System.Windows.Forms.ToolBarButton tbbNew;
      private System.Windows.Forms.MenuItem mnuFile;
      private System.Windows.Forms.ToolBarButton tbbProp;
      private System.Windows.Forms.ToolBarButton tbbFileSep;
      private System.Windows.Forms.MenuItem mnuFileOpenPrj;
      private System.Windows.Forms.MenuItem mnuFileSavePrj;
      private System.Windows.Forms.MenuItem mnuFileExit;
      private System.Windows.Forms.MenuItem mnuView;
      private System.Windows.Forms.MenuItem mnuEditProperties;
      private System.Windows.Forms.MenuItem mnuFileNewObj;
      private System.Windows.Forms.MenuItem mnuFileSep1;
      private System.Windows.Forms.ContextMenu mnuTreeView;
      private System.Windows.Forms.MenuItem mnuTreeNew;
      private System.Windows.Forms.MenuItem mnuTreeEdit;
      private System.Windows.Forms.ToolBarButton tbbDelete;
      private System.Windows.Forms.MenuItem mnuFileDeleteObj;
      private SGDK2.DataChangeNotifier dataMonitor;
      private System.Windows.Forms.MenuItem mnuFileNewPrj;
      private System.Windows.Forms.MenuItem mnuFileResetCode;
      private System.Windows.Forms.MenuItem mnuFileGenerate;
      private System.Windows.Forms.MenuItem mnuFileSavePrjAs;
      private System.Windows.Forms.MenuItem mnuViewChanges;
      private System.Windows.Forms.MenuItem mnuFileRunProject;
      private System.Windows.Forms.MenuItem mnuFileDeleteOutputFiles;
      private System.Windows.Forms.MenuItem mnuFileRunProjectInDebugMode;
      private System.Windows.Forms.MenuItem mnuHelp;
      private System.Windows.Forms.MenuItem mnuHelpAbout;
      private System.Windows.Forms.MenuItem mnuFileSep2;
      private System.Windows.Forms.MenuItem mnuFileSep3;
      private System.Windows.Forms.MenuItem mnuTreeDelete;
      private System.Windows.Forms.MenuItem mnuTreeImport;
      private System.Windows.Forms.MenuItem mnuFileImportObj;
      private System.ComponentModel.IContainer components;
      #endregion

      #region Initialization and Clean-up
		public frmMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmMain));
         this.tbrMain = new System.Windows.Forms.ToolBar();
         this.tbbShowTree = new System.Windows.Forms.ToolBarButton();
         this.tbsTree = new System.Windows.Forms.ToolBarButton();
         this.tbbOpen = new System.Windows.Forms.ToolBarButton();
         this.tbbFileSep = new System.Windows.Forms.ToolBarButton();
         this.tbbNew = new System.Windows.Forms.ToolBarButton();
         this.tbbDelete = new System.Windows.Forms.ToolBarButton();
         this.tbbProp = new System.Windows.Forms.ToolBarButton();
         this.imlMain = new System.Windows.Forms.ImageList(this.components);
         this.tvwMain = new System.Windows.Forms.TreeView();
         this.mnuTreeView = new System.Windows.Forms.ContextMenu();
         this.mnuTreeNew = new System.Windows.Forms.MenuItem();
         this.mnuTreeEdit = new System.Windows.Forms.MenuItem();
         this.mnuTreeDelete = new System.Windows.Forms.MenuItem();
         this.mnuTreeImport = new System.Windows.Forms.MenuItem();
         this.splitterMDI = new System.Windows.Forms.Splitter();
         this.mnuMain = new System.Windows.Forms.MainMenu();
         this.mnuFile = new System.Windows.Forms.MenuItem();
         this.mnuFileNewPrj = new System.Windows.Forms.MenuItem();
         this.mnuFileOpenPrj = new System.Windows.Forms.MenuItem();
         this.mnuFileSavePrj = new System.Windows.Forms.MenuItem();
         this.mnuFileSavePrjAs = new System.Windows.Forms.MenuItem();
         this.mnuFileSep1 = new System.Windows.Forms.MenuItem();
         this.mnuFileNewObj = new System.Windows.Forms.MenuItem();
         this.mnuFileDeleteObj = new System.Windows.Forms.MenuItem();
         this.mnuFileImportObj = new System.Windows.Forms.MenuItem();
         this.mnuFileSep2 = new System.Windows.Forms.MenuItem();
         this.mnuFileRunProject = new System.Windows.Forms.MenuItem();
         this.mnuFileRunProjectInDebugMode = new System.Windows.Forms.MenuItem();
         this.mnuFileResetCode = new System.Windows.Forms.MenuItem();
         this.mnuFileGenerate = new System.Windows.Forms.MenuItem();
         this.mnuFileDeleteOutputFiles = new System.Windows.Forms.MenuItem();
         this.mnuFileSep3 = new System.Windows.Forms.MenuItem();
         this.mnuFileExit = new System.Windows.Forms.MenuItem();
         this.mnuView = new System.Windows.Forms.MenuItem();
         this.mnuEditProperties = new System.Windows.Forms.MenuItem();
         this.mnuViewChanges = new System.Windows.Forms.MenuItem();
         this.mnuWindows = new System.Windows.Forms.MenuItem();
         this.mnuHelp = new System.Windows.Forms.MenuItem();
         this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
         this.pnlProjectTree = new System.Windows.Forms.Panel();
         this.lblProjectTree = new System.Windows.Forms.Label();
         this.dataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.pnlProjectTree.SuspendLayout();
         this.SuspendLayout();
         // 
         // tbrMain
         // 
         this.tbrMain.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                                   this.tbbShowTree,
                                                                                   this.tbsTree,
                                                                                   this.tbbOpen,
                                                                                   this.tbbFileSep,
                                                                                   this.tbbNew,
                                                                                   this.tbbDelete,
                                                                                   this.tbbProp});
         this.tbrMain.DropDownArrows = true;
         this.tbrMain.ImageList = this.imlMain;
         this.tbrMain.Location = new System.Drawing.Point(0, 0);
         this.tbrMain.Name = "tbrMain";
         this.tbrMain.ShowToolTips = true;
         this.tbrMain.Size = new System.Drawing.Size(800, 27);
         this.tbrMain.TabIndex = 1;
         this.tbrMain.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbrMain_ButtonClick);
         // 
         // tbbShowTree
         // 
         this.tbbShowTree.ImageIndex = 0;
         this.tbbShowTree.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbShowTree.ToolTipText = "Show/hide the project tree";
         this.tbbShowTree.Visible = false;
         // 
         // tbsTree
         // 
         this.tbsTree.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
         // 
         // tbbOpen
         // 
         this.tbbOpen.ImageIndex = 6;
         this.tbbOpen.ToolTipText = "Open an existing project";
         // 
         // tbbFileSep
         // 
         this.tbbFileSep.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
         // 
         // tbbNew
         // 
         this.tbbNew.ImageIndex = 8;
         this.tbbNew.ToolTipText = "Create a new object in the currently selected category";
         // 
         // tbbDelete
         // 
         this.tbbDelete.ImageIndex = 11;
         this.tbbDelete.ToolTipText = "Delete the selected object";
         // 
         // tbbProp
         // 
         this.tbbProp.ImageIndex = 9;
         this.tbbProp.ToolTipText = "Edit the currently selected object";
         // 
         // imlMain
         // 
         this.imlMain.ImageSize = new System.Drawing.Size(15, 15);
         this.imlMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlMain.ImageStream")));
         this.imlMain.TransparentColor = System.Drawing.Color.Magenta;
         // 
         // tvwMain
         // 
         this.tvwMain.ContextMenu = this.mnuTreeView;
         this.tvwMain.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tvwMain.HideSelection = false;
         this.tvwMain.ImageList = this.imlMain;
         this.tvwMain.Indent = 18;
         this.tvwMain.ItemHeight = 16;
         this.tvwMain.Location = new System.Drawing.Point(0, 16);
         this.tvwMain.Name = "tvwMain";
         this.tvwMain.Size = new System.Drawing.Size(176, 531);
         this.tvwMain.TabIndex = 3;
         this.tvwMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvwMain_MouseUp);
         this.tvwMain.DoubleClick += new System.EventHandler(this.tvwMain_DoubleClick);
         this.tvwMain.Leave += new System.EventHandler(this.tvwMain_Leave);
         // 
         // mnuTreeView
         // 
         this.mnuTreeView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                    this.mnuTreeNew,
                                                                                    this.mnuTreeEdit,
                                                                                    this.mnuTreeDelete,
                                                                                    this.mnuTreeImport});
         // 
         // mnuTreeNew
         // 
         this.mnuTreeNew.Index = 0;
         this.mnuTreeNew.Text = "&New";
         this.mnuTreeNew.Click += new System.EventHandler(this.mnuTreeNew_Click);
         // 
         // mnuTreeEdit
         // 
         this.mnuTreeEdit.Index = 1;
         this.mnuTreeEdit.Text = "&Edit";
         this.mnuTreeEdit.Click += new System.EventHandler(this.mnuTreeEdit_Click);
         // 
         // mnuTreeDelete
         // 
         this.mnuTreeDelete.Index = 2;
         this.mnuTreeDelete.Text = "&Delete";
         this.mnuTreeDelete.Click += new System.EventHandler(this.mnuTreeDelete_Click);
         // 
         // mnuTreeImport
         // 
         this.mnuTreeImport.Index = 3;
         this.mnuTreeImport.Text = "&Import From...";
         this.mnuTreeImport.Click += new System.EventHandler(this.mnuTreeImport_Click);
         // 
         // splitterMDI
         // 
         this.splitterMDI.Location = new System.Drawing.Point(176, 27);
         this.splitterMDI.MinSize = 20;
         this.splitterMDI.Name = "splitterMDI";
         this.splitterMDI.Size = new System.Drawing.Size(6, 547);
         this.splitterMDI.TabIndex = 4;
         this.splitterMDI.TabStop = false;
         // 
         // mnuMain
         // 
         this.mnuMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                this.mnuFile,
                                                                                this.mnuView,
                                                                                this.mnuWindows,
                                                                                this.mnuHelp});
         // 
         // mnuFile
         // 
         this.mnuFile.Index = 0;
         this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                this.mnuFileNewPrj,
                                                                                this.mnuFileOpenPrj,
                                                                                this.mnuFileSavePrj,
                                                                                this.mnuFileSavePrjAs,
                                                                                this.mnuFileSep1,
                                                                                this.mnuFileNewObj,
                                                                                this.mnuFileDeleteObj,
                                                                                this.mnuFileImportObj,
                                                                                this.mnuFileSep2,
                                                                                this.mnuFileRunProject,
                                                                                this.mnuFileRunProjectInDebugMode,
                                                                                this.mnuFileResetCode,
                                                                                this.mnuFileGenerate,
                                                                                this.mnuFileDeleteOutputFiles,
                                                                                this.mnuFileSep3,
                                                                                this.mnuFileExit});
         this.mnuFile.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
         this.mnuFile.Text = "&File";
         // 
         // mnuFileNewPrj
         // 
         this.mnuFileNewPrj.Index = 0;
         this.mnuFileNewPrj.MergeOrder = 1;
         this.mnuFileNewPrj.Text = "&New Project";
         this.mnuFileNewPrj.Click += new System.EventHandler(this.mnuFileNewPrj_Click);
         // 
         // mnuFileOpenPrj
         // 
         this.mnuFileOpenPrj.Index = 1;
         this.mnuFileOpenPrj.MergeOrder = 2;
         this.mnuFileOpenPrj.Text = "&Open Project";
         this.mnuFileOpenPrj.Click += new System.EventHandler(this.mnuFileOpenPrj_Click);
         // 
         // mnuFileSavePrj
         // 
         this.mnuFileSavePrj.Index = 2;
         this.mnuFileSavePrj.MergeOrder = 3;
         this.mnuFileSavePrj.Text = "&Save Project";
         this.mnuFileSavePrj.Click += new System.EventHandler(this.mnuFileSavePrj_Click);
         // 
         // mnuFileSavePrjAs
         // 
         this.mnuFileSavePrjAs.Index = 3;
         this.mnuFileSavePrjAs.MergeOrder = 4;
         this.mnuFileSavePrjAs.Text = "Save Project &As...";
         this.mnuFileSavePrjAs.Click += new System.EventHandler(this.mnuFileSavePrjAs_Click);
         // 
         // mnuFileSep1
         // 
         this.mnuFileSep1.Index = 4;
         this.mnuFileSep1.MergeOrder = 10;
         this.mnuFileSep1.Text = "-";
         // 
         // mnuFileNewObj
         // 
         this.mnuFileNewObj.Index = 5;
         this.mnuFileNewObj.MergeOrder = 11;
         this.mnuFileNewObj.Text = "New O&bject";
         this.mnuFileNewObj.Click += new System.EventHandler(this.mnuFileNewObj_Click);
         // 
         // mnuFileDeleteObj
         // 
         this.mnuFileDeleteObj.Index = 6;
         this.mnuFileDeleteObj.MergeOrder = 12;
         this.mnuFileDeleteObj.Text = "&Delete Object";
         this.mnuFileDeleteObj.Click += new System.EventHandler(this.mnuFileDeleteObj_Click);
         // 
         // mnuFileImportObj
         // 
         this.mnuFileImportObj.Index = 7;
         this.mnuFileImportObj.MergeOrder = 13;
         this.mnuFileImportObj.Text = "&Import From...";
         this.mnuFileImportObj.Click += new System.EventHandler(this.mnuFileImportObj_Click);
         // 
         // mnuFileSep2
         // 
         this.mnuFileSep2.Index = 8;
         this.mnuFileSep2.MergeOrder = 20;
         this.mnuFileSep2.Text = "-";
         // 
         // mnuFileRunProject
         // 
         this.mnuFileRunProject.Index = 9;
         this.mnuFileRunProject.MergeOrder = 21;
         this.mnuFileRunProject.Shortcut = System.Windows.Forms.Shortcut.CtrlF5;
         this.mnuFileRunProject.Text = "&Run Project";
         this.mnuFileRunProject.Click += new System.EventHandler(this.mnuFileRunProject_Click);
         // 
         // mnuFileRunProjectInDebugMode
         // 
         this.mnuFileRunProjectInDebugMode.Index = 10;
         this.mnuFileRunProjectInDebugMode.MergeOrder = 22;
         this.mnuFileRunProjectInDebugMode.Shortcut = System.Windows.Forms.Shortcut.F5;
         this.mnuFileRunProjectInDebugMode.Text = "Run Project in Debug &Mode";
         this.mnuFileRunProjectInDebugMode.Click += new System.EventHandler(this.mnuFileRunProjectInDebugMode_Click);
         // 
         // mnuFileResetCode
         // 
         this.mnuFileResetCode.Index = 11;
         this.mnuFileResetCode.MergeOrder = 23;
         this.mnuFileResetCode.Text = "R&eset Source Code";
         this.mnuFileResetCode.Click += new System.EventHandler(this.mnuFileResetCode_Click);
         // 
         // mnuFileGenerate
         // 
         this.mnuFileGenerate.Index = 12;
         this.mnuFileGenerate.MergeOrder = 24;
         this.mnuFileGenerate.Shortcut = System.Windows.Forms.Shortcut.F7;
         this.mnuFileGenerate.Text = "&Generate Project";
         this.mnuFileGenerate.Click += new System.EventHandler(this.mnuFileGenerate_Click);
         // 
         // mnuFileDeleteOutputFiles
         // 
         this.mnuFileDeleteOutputFiles.Enabled = false;
         this.mnuFileDeleteOutputFiles.Index = 13;
         this.mnuFileDeleteOutputFiles.MergeOrder = 25;
         this.mnuFileDeleteOutputFiles.Text = "Dele&te Output Files";
         this.mnuFileDeleteOutputFiles.Click += new System.EventHandler(this.mnuFileDeleteOutputFiles_Click);
         // 
         // mnuFileSep3
         // 
         this.mnuFileSep3.Index = 14;
         this.mnuFileSep3.MergeOrder = 98;
         this.mnuFileSep3.Text = "-";
         // 
         // mnuFileExit
         // 
         this.mnuFileExit.Index = 15;
         this.mnuFileExit.MergeOrder = 99;
         this.mnuFileExit.Text = "E&xit";
         this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
         // 
         // mnuView
         // 
         this.mnuView.Index = 1;
         this.mnuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                this.mnuEditProperties,
                                                                                this.mnuViewChanges});
         this.mnuView.MergeOrder = 1;
         this.mnuView.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
         this.mnuView.Text = "&View";
         // 
         // mnuEditProperties
         // 
         this.mnuEditProperties.Index = 0;
         this.mnuEditProperties.Shortcut = System.Windows.Forms.Shortcut.F4;
         this.mnuEditProperties.Text = "&Properties";
         this.mnuEditProperties.Click += new System.EventHandler(this.mnuEditProperties_Click);
         // 
         // mnuViewChanges
         // 
         this.mnuViewChanges.Index = 1;
         this.mnuViewChanges.Text = "&Unsaved Changes";
         this.mnuViewChanges.Click += new System.EventHandler(this.mnuViewChanges_Click);
         // 
         // mnuWindows
         // 
         this.mnuWindows.Index = 2;
         this.mnuWindows.MdiList = true;
         this.mnuWindows.MergeOrder = 4;
         this.mnuWindows.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
         this.mnuWindows.Text = "&Windows";
         // 
         // mnuHelp
         // 
         this.mnuHelp.Index = 3;
         this.mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                this.mnuHelpAbout});
         this.mnuHelp.MergeOrder = 7;
         this.mnuHelp.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
         this.mnuHelp.Text = "&Help";
         // 
         // mnuHelpAbout
         // 
         this.mnuHelpAbout.Index = 0;
         this.mnuHelpAbout.Text = "&About";
         this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
         // 
         // pnlProjectTree
         // 
         this.pnlProjectTree.Controls.Add(this.tvwMain);
         this.pnlProjectTree.Controls.Add(this.lblProjectTree);
         this.pnlProjectTree.Dock = System.Windows.Forms.DockStyle.Left;
         this.pnlProjectTree.Location = new System.Drawing.Point(0, 27);
         this.pnlProjectTree.Name = "pnlProjectTree";
         this.pnlProjectTree.Size = new System.Drawing.Size(176, 547);
         this.pnlProjectTree.TabIndex = 6;
         // 
         // lblProjectTree
         // 
         this.lblProjectTree.BackColor = System.Drawing.SystemColors.ActiveCaption;
         this.lblProjectTree.Dock = System.Windows.Forms.DockStyle.Top;
         this.lblProjectTree.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
         this.lblProjectTree.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
         this.lblProjectTree.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
         this.lblProjectTree.ImageIndex = 2;
         this.lblProjectTree.ImageList = this.imlMain;
         this.lblProjectTree.Location = new System.Drawing.Point(0, 0);
         this.lblProjectTree.Name = "lblProjectTree";
         this.lblProjectTree.Size = new System.Drawing.Size(176, 16);
         this.lblProjectTree.TabIndex = 4;
         this.lblProjectTree.Text = "Project Tree";
         this.lblProjectTree.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         this.lblProjectTree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblProjectTree_MouseUp);
         this.lblProjectTree.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblProjectTree_MouseMove);
         this.lblProjectTree.MouseLeave += new System.EventHandler(this.MainPanel_MouseLeave);
         this.lblProjectTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblProjectTree_MouseMove);
         // 
         // dataMonitor
         // 
         this.dataMonitor.CategorizedTilesetRowChanging += new SGDK2.ProjectDataset.CategorizedTilesetRowChangeEventHandler(this.dataMonitor_CategorizedTilesetRowChanging);
         this.dataMonitor.SourceCodeRowDeleted += new SGDK2.ProjectDataset.SourceCodeRowChangeEventHandler(this.dataMonitor_SourceCodeRowDeleted);
         this.dataMonitor.SpriteCategoryRowChanging += new SGDK2.ProjectDataset.SpriteCategoryRowChangeEventHandler(this.dataMonitor_SpriteCategoryRowChanging);
         this.dataMonitor.SpritePlanRowDeleted += new SGDK2.ProjectDataset.SpritePlanRowChangeEventHandler(this.dataMonitor_SpritePlanRowDeleted);
         this.dataMonitor.LayerRowChanged += new SGDK2.ProjectDataset.LayerRowChangeEventHandler(this.dataMonitor_LayerRowChanged);
         this.dataMonitor.SpritePlanRowChanged += new SGDK2.ProjectDataset.SpritePlanRowChangeEventHandler(this.dataMonitor_SpritePlanRowChanged);
         this.dataMonitor.CategorizedTilesetRowDeleting += new SGDK2.ProjectDataset.CategorizedTilesetRowChangeEventHandler(this.dataMonitor_CategorizedTilesetRowDeleting);
         this.dataMonitor.CategorizedTilesetRowChanged += new SGDK2.ProjectDataset.CategorizedTilesetRowChangeEventHandler(this.dataMonitor_CategorizedTilesetRowChanged);
         this.dataMonitor.SourceCodeRowChanging += new SGDK2.ProjectDataset.SourceCodeRowChangeEventHandler(this.dataMonitor_SourceCodeRowChanging);
         this.dataMonitor.MapRowDeleting += new SGDK2.ProjectDataset.MapRowChangeEventHandler(this.dataMonitor_MapRowDeleting);
         this.dataMonitor.FramesetRowChanging += new SGDK2.ProjectDataset.FramesetRowChangeEventHandler(this.dataMonitor_FramesetRowChanging);
         this.dataMonitor.SpriteDefinitionRowDeleting += new SGDK2.ProjectDataset.SpriteDefinitionRowChangeEventHandler(this.dataMonitor_SpriteDefinitionRowDeleting);
         this.dataMonitor.SourceCodeRowChanged += new SGDK2.ProjectDataset.SourceCodeRowChangeEventHandler(this.dataMonitor_SourceCodeRowChanged);
         this.dataMonitor.GraphicSheetRowChanged += new SGDK2.ProjectDataset.GraphicSheetRowChangeEventHandler(this.dataMonitor_GraphicSheetRowChanged);
         this.dataMonitor.LayerRowDeleting += new SGDK2.ProjectDataset.LayerRowChangeEventHandler(this.dataMonitor_LayerRowDeleting);
         this.dataMonitor.FramesetRowChanged += new SGDK2.ProjectDataset.FramesetRowChangeEventHandler(this.dataMonitor_FramesetRowChanged);
         this.dataMonitor.CounterRowDeleted += new SGDK2.ProjectDataset.CounterRowChangeEventHandler(this.dataMonitor_CounterRowDeleted);
         this.dataMonitor.SpriteCategoryRowDeleting += new SGDK2.ProjectDataset.SpriteCategoryRowChangeEventHandler(this.dataMonitor_SpriteCategoryRowDeleting);
         this.dataMonitor.SolidityRowDeleting += new SGDK2.ProjectDataset.SolidityRowChangeEventHandler(this.dataMonitor_SolidityRowDeleting);
         this.dataMonitor.CounterRowDeleting += new SGDK2.ProjectDataset.CounterRowChangeEventHandler(this.dataMonitor_CounterRowDeleting);
         this.dataMonitor.SourceCodeRowDeleting += new SGDK2.ProjectDataset.SourceCodeRowChangeEventHandler(this.dataMonitor_SourceCodeRowDeleting);
         this.dataMonitor.SpritePlanRowChanging += new SGDK2.ProjectDataset.SpritePlanRowChangeEventHandler(this.dataMonitor_SpritePlanRowChanging);
         this.dataMonitor.SolidityRowDeleted += new SGDK2.ProjectDataset.SolidityRowChangeEventHandler(this.dataMonitor_SolidityRowDeleted);
         this.dataMonitor.TilesetRowDeleted += new SGDK2.ProjectDataset.TilesetRowChangeEventHandler(this.dataMonitor_TilesetRowDeleted);
         this.dataMonitor.TileCategoryRowDeleting += new SGDK2.ProjectDataset.TileCategoryRowChangeEventHandler(this.dataMonitor_TileCategoryRowDeleting);
         this.dataMonitor.TileCategoryRowChanging += new SGDK2.ProjectDataset.TileCategoryRowChangeEventHandler(this.dataMonitor_TileCategoryRowChanging);
         this.dataMonitor.CounterRowChanging += new SGDK2.ProjectDataset.CounterRowChangeEventHandler(this.dataMonitor_CounterRowChanging);
         this.dataMonitor.LayerRowDeleted += new SGDK2.ProjectDataset.LayerRowChangeEventHandler(this.dataMonitor_LayerRowDeleted);
         this.dataMonitor.TilesetRowDeleting += new SGDK2.ProjectDataset.TilesetRowChangeEventHandler(this.dataMonitor_TilesetRowDeleting);
         this.dataMonitor.SpriteDefinitionRowDeleted += new SGDK2.ProjectDataset.SpriteDefinitionRowChangeEventHandler(this.dataMonitor_SpriteDefinitionRowDeleted);
         this.dataMonitor.GraphicSheetRowDeleted += new SGDK2.ProjectDataset.GraphicSheetRowChangeEventHandler(this.dataMonitor_GraphicSheetRowDeleted);
         this.dataMonitor.SolidityRowChanging += new SGDK2.ProjectDataset.SolidityRowChangeEventHandler(this.dataMonitor_SolidityRowChanging);
         this.dataMonitor.LayerRowChanging += new SGDK2.ProjectDataset.LayerRowChangeEventHandler(this.dataMonitor_LayerRowChanging);
         this.dataMonitor.GraphicSheetRowChanging += new SGDK2.ProjectDataset.GraphicSheetRowChangeEventHandler(this.dataMonitor_GraphicSheetRowChanging);
         this.dataMonitor.SolidityRowChanged += new SGDK2.ProjectDataset.SolidityRowChangeEventHandler(this.dataMonitor_SolidityRowChanged);
         this.dataMonitor.TilesetRowChanging += new SGDK2.ProjectDataset.TilesetRowChangeEventHandler(this.dataMonitor_TilesetRowChanging);
         this.dataMonitor.TilesetRowChanged += new SGDK2.ProjectDataset.TilesetRowChangeEventHandler(this.dataMonitor_TilesetRowChanged);
         this.dataMonitor.FramesetRowDeleted += new SGDK2.ProjectDataset.FramesetRowChangeEventHandler(this.dataMonitor_FramesetRowDeleted);
         this.dataMonitor.MapRowChanged += new SGDK2.ProjectDataset.MapRowChangeEventHandler(this.dataMonitor_MapRowChanged);
         this.dataMonitor.CategorizedTilesetRowDeleted += new SGDK2.ProjectDataset.CategorizedTilesetRowChangeEventHandler(this.dataMonitor_CategorizedTilesetRowDeleted);
         this.dataMonitor.FramesetRowDeleting += new SGDK2.ProjectDataset.FramesetRowChangeEventHandler(this.dataMonitor_FramesetRowDeleting);
         this.dataMonitor.SpritePlanRowDeleting += new SGDK2.ProjectDataset.SpritePlanRowChangeEventHandler(this.dataMonitor_SpritePlanRowDeleting);
         this.dataMonitor.MapRowDeleted += new SGDK2.ProjectDataset.MapRowChangeEventHandler(this.dataMonitor_MapRowDeleted);
         this.dataMonitor.SpriteDefinitionRowChanging += new SGDK2.ProjectDataset.SpriteDefinitionRowChangeEventHandler(this.dataMonitor_SpriteDefinitionRowChanging);
         this.dataMonitor.TileCategoryRowChanged += new SGDK2.ProjectDataset.TileCategoryRowChangeEventHandler(this.dataMonitor_TileCategoryRowChanged);
         this.dataMonitor.SpriteCategoryRowChanged += new SGDK2.ProjectDataset.SpriteCategoryRowChangeEventHandler(this.dataMonitor_SpriteCategoryRowChanged);
         this.dataMonitor.TileCategoryRowDeleted += new SGDK2.ProjectDataset.TileCategoryRowChangeEventHandler(this.dataMonitor_TileCategoryRowDeleted);
         this.dataMonitor.GraphicSheetRowDeleting += new SGDK2.ProjectDataset.GraphicSheetRowChangeEventHandler(this.dataMonitor_GraphicSheetRowDeleting);
         this.dataMonitor.SpriteDefinitionRowChanged += new SGDK2.ProjectDataset.SpriteDefinitionRowChangeEventHandler(this.dataMonitor_SpriteDefinitionRowChanged);
         this.dataMonitor.MapRowChanging += new SGDK2.ProjectDataset.MapRowChangeEventHandler(this.dataMonitor_MapRowChanging);
         this.dataMonitor.CounterRowChanged += new SGDK2.ProjectDataset.CounterRowChangeEventHandler(this.dataMonitor_CounterRowChanged);
         this.dataMonitor.SpriteCategoryRowDeleted += new SGDK2.ProjectDataset.SpriteCategoryRowChangeEventHandler(this.dataMonitor_SpriteCategoryRowDeleted);
         // 
         // frmMain
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(800, 574);
         this.Controls.Add(this.splitterMDI);
         this.Controls.Add(this.pnlProjectTree);
         this.Controls.Add(this.tbrMain);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.IsMdiContainer = true;
         this.Menu = this.mnuMain;
         this.Name = "frmMain";
         this.Text = "Scrolling Game Development Kit";
         this.pnlProjectTree.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion

      #region Private methods
      private void InitializeTree()
      {
         tvwMain.Nodes.Clear();
         TreeNode ndRoot = new TreeNode("Project", 0, 0);
         ndRoot.Tag = "Project";
         m_TreeNodes = new System.Collections.Specialized.HybridDictionary();
         tvwMain.Nodes.Add(ndRoot);
         TreeNode ndFolder = new TreeNode("Graphic Sheets", 1, 1);
         ndFolder.Tag = "GS";
         m_TreeNodes.Add("GS", ndFolder);
         ndRoot.Nodes.Add(ndFolder);

         ndFolder = new TreeNode("Framesets", 1, 1);
         ndFolder.Tag = "FS";
         m_TreeNodes.Add("FS", ndFolder);
         ndRoot.Nodes.Add(ndFolder);

         ndFolder = new TreeNode("Tilesets", 1, 1);
         ndFolder.Tag = "TS";
         m_TreeNodes.Add("TS", ndFolder);
         ndRoot.Nodes.Add(ndFolder);

         ndFolder = new TreeNode("Tile Categories", 1, 1);
         ndFolder.Tag = "TC";
         m_TreeNodes.Add("TC", ndFolder);
         ndRoot.Nodes.Add(ndFolder);

         ndFolder = new TreeNode("Counters", 1, 1);
         ndFolder.Tag = "CR";
         m_TreeNodes.Add("CR", ndFolder);
         ndRoot.Nodes.Add(ndFolder);

         ndFolder = new TreeNode("Maps", 1, 1);
         ndFolder.Tag = "MP";
         m_TreeNodes.Add("MP", ndFolder);
         ndRoot.Nodes.Add(ndFolder);

         ndFolder = new TreeNode("Solidity", 1, 1);
         ndFolder.Tag = "SY";
         m_TreeNodes.Add("SY", ndFolder);
         ndRoot.Nodes.Add(ndFolder);

         ndFolder = new TreeNode("Sprite Definitions", 1, 1);
         ndFolder.Tag = "SD";
         m_TreeNodes.Add("SD", ndFolder);
         ndRoot.Nodes.Add(ndFolder);

         ndFolder = new TreeNode("Sprite Categories", 1, 1);
         ndFolder.Tag = "SC";
         m_TreeNodes.Add("SC", ndFolder);
         ndRoot.Nodes.Add(ndFolder);

         ndFolder = new TreeNode("SourceCode", 1, 1);
         ndFolder.Tag = "CD";
         m_TreeNodes.Add("CD", ndFolder);
         ndRoot.Nodes.Add(ndFolder);
      }

      private void DoNewProject()
      {
         mnuFileDeleteOutputFiles.Enabled = false;
         ProjectData.Clear();
         InitializeTree();
         ProjectData.ExtendedProperties["SchemaVersion"] = "1";
         tvwMain.CollapseAll();
         tvwMain.Nodes[0].Expand();
         m_strProjectPath = null;
      }

      private void DoOpenProject(string projectFile)
      {
         DataSet dsLoad = new DataSet();
         dsLoad.ReadXml(projectFile);
         ProjectData.Clear();
         InitializeTree();
         ProjectData.Merge(dsLoad);
         ProjectData.AcceptChanges();
         m_strProjectPath = projectFile;
         tvwMain.CollapseAll();
         tvwMain.Nodes[0].Expand();
         mnuFileDeleteOutputFiles.Enabled = true;
      }

      private void ShowMDIChild(System.Type typForm)
      {
         foreach(Form frm in this.MdiChildren)
            if (typForm.IsInstanceOfType(frm))
            {
               frm.Activate();
               return;
            }

         Form frmNew = (Form)typForm.GetConstructor(System.Type.EmptyTypes).Invoke(null);
         frmNew.MdiParent = this;
         frmNew.Show();
      }

      private void NewObject()
      {
         for (bool bCreatedObject = false; !bCreatedObject; bCreatedObject = !bCreatedObject)
         {
            if (m_ContextNode == tvwMain.Nodes[0])
            {
               MessageBox.Show(this, "Cannot create a new object of the selected type", "Create New Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return;
            }
            Form frmNew;
            String Key = m_ContextNode.Tag.ToString();
            String[] KeyParts = Key.Substring(2).Split('~');
            switch(Key.Substring(0,2))
            {
               case "GS":
                  frmNew = new frmGfxSheet();
                  frmNew.MdiParent = this;
                  frmNew.Show();                           
                  break;
               case "FS":
                  frmNew = new frmFrameEdit();
                  frmNew.MdiParent = this;
                  frmNew.Show();
                  break;
               case "TS":
                  if (ProjectData.Frameset.DefaultView.Count <= 0)
                  {
                     MessageBox.Show(this, "Please create a Frameset before creating a Tileset", "New Tileset", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     return;
                  }
                  frmNew = new frmTileEdit();
                  frmNew.MdiParent = this;
                  frmNew.Show();
                  break;
               case "TC":
                  if ((KeyParts.Length == 0) || (KeyParts[0].Length == 0))
                  {
                     frmNew = new frmTileCategoryName();
                     frmNew.MdiParent = this;
                     frmNew.Show();
                  }
                  else
                  {
                     ProjectDataset.TilesetRow tsr = frmSelectTileset.SelectTileset();
                     if (tsr == null)
                        break;

                     if (ProjectData.GetCategorizedTileset(tsr.Name, KeyParts[0]) != null)
                     {
                        if (DialogResult.OK == MessageBox.Show(this, "Tileset \"" + tsr.Name + "\" already has tiles categorized for category \"" + KeyParts[0] + "\".  The existing set of categorized tiles will load instead of adding a new one.", "Add Tileset to Tile Category", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation))
                           frmNew = new frmTileCategory(ProjectData.GetCategorizedTileset(tsr.Name, KeyParts[0]));
                        else
                           break;
                     }
                     else
                        frmNew = new frmTileCategory(ProjectData.AddCategorizedTilesetRow(
                           tsr, ProjectData.GetTileCategory(KeyParts[0])));
                     frmNew.MdiParent = this;
                     frmNew.Show();
                  }
                  break;
               case "CR":
                  frmNew = new frmCounterEdit();
                  frmNew.MdiParent = this;
                  frmNew.Show();
                  break;
               case "SD":
                  frmNew = new frmSpriteDefinition();
                  frmNew.MdiParent = this;
                  frmNew.Show();
                  break;
               case "SY":
                  frmNew = new frmSolidity();
                  frmNew.MdiParent = this;
                  frmNew.Show();
                  break;
               case "MP":
                  frmNew = new frmMapManager();
                  frmNew.MdiParent = this;
                  frmNew.Show();
                  break;
               case "LR":
               case "LE":
                  if (ProjectData.Tileset.Count <= 0)
                  {
                     MessageBox.Show(this, "Please create a tileset before creating a layer", "New Layer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     return;
                  }
                  frmNew = new frmLayerManager(ProjectData.GetMap(KeyParts[0]));
                  frmNew.MdiParent = this;
                  frmNew.Show();
                  break;
               case "PL":
                  frmNew = new frmPlanEdit(ProjectData.GetLayer(KeyParts[0], KeyParts[1]));
                  frmNew.MdiParent = this;
                  frmNew.Show();
                  break;
               case "SC":
                  frmNew = new frmSpriteCategory();
                  frmNew.MdiParent = this;
                  frmNew.Show();
                  break;
               case "CD":
                  string sName = frmInputBox.GetInput(MdiParent, "New Custom Code Object", "Enter a name for the new object", String.Empty);
                  if (sName == null)
                     return;
                  string sBareName;
                  if (sName.EndsWith(".cs"))
                     sBareName = sName.Substring(0, sName.Length - 3);
                  else if (sName.EndsWith(".dll"))
                     sBareName = sName.Substring(0, sName.Length - 4);
                  else
                  {
                     sBareName = sName;
                     sName += ".cs";
                  }
                  if (!sName.EndsWith(".dll"))
                  {
                     string msg = ProjectData.ValidateName(sBareName);
                     if (msg != null)
                     {
                        MessageBox.Show(this, "Invalid name \"" + sBareName + "\": " + msg, "New Custom Code Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                     }
                  }
                  if (null != ProjectData.GetSourceCode(sName))
                  {
                     MessageBox.Show(this, "The specified custom object name already exists", "New Custom Code Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     return;
                  }
                  if (sName.EndsWith(".cs"))
                  {
                     frmNew = new frmCodeEditor(sName, KeyParts[KeyParts.Length-1]);
                     frmNew.MdiParent = this;
                     frmNew.Show();
                  }
                  else
                     ProjectData.AddSourceCode(sName, null, null, true, null);
                  break;
               default:
                  m_ContextNode = m_ContextNode.Parent;
                  bCreatedObject = !bCreatedObject;
                  continue;
            }
         }
      }

      private void EditObject()
      {
         Form frmNew;
         String Key = m_ContextNode.Tag.ToString();
         string[] KeyParts = Key.Substring(2).Split('~');
         switch(Key.Substring(0,2))
         {
            case "Pr":
               frmNew = new frmProject();
               frmNew.MdiParent = this;
               frmNew.Show();
               break;
            case "GS":
               if (Key == "GS")
               {
                  MessageBox.Show(this, "A specific graphic sheet must be selected to edit.", "Edit Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
               }
               frmNew = new frmGfxSheet(ProjectData.GetGraphicSheet(KeyParts[0]));
               frmNew.MdiParent = this;
               frmNew.Show();
               break;
            case "GE":
               frmNew = new frmGraphicsEditor(ProjectData.GetGraphicSheet(KeyParts[0]));
               frmNew.MdiParent = this;
               frmNew.Show();
               break;
            case "FS":
               if (Key == "FS")
               {
                  MessageBox.Show(this, "A specific Frameset must be selected to edit.", "Edit Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
               }
               frmNew = new frmFrameEdit(ProjectData.GetFrameSet(KeyParts[0]));
               frmNew.MdiParent = this;
               frmNew.Show();
               break;
            case "TS":
               if (Key == "TS")
               {
                  MessageBox.Show(this, "A specific Tileset must be selected to edit.", "Edit Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
               }
               frmNew = new frmTileEdit(ProjectData.GetTileSet(KeyParts[0]));
               frmNew.MdiParent = this;
               frmNew.Show();
               break;
            case "TC":
               if (Key == "TC")
               {
                  MessageBox.Show(this, "A specific Tile Category must be selected to edit.", "Edit Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
               }
               if (KeyParts.Length <= 1)
               {
                  frmNew = new frmTileCategoryName(ProjectData.GetTileCategory(KeyParts[0]));
                  frmNew.MdiParent = this;
                  frmNew.Show();
               }
               else
               {
                  frmNew = new frmTileCategory(ProjectData.GetCategorizedTileset(KeyParts[1], KeyParts[0]));
                  frmNew.MdiParent = this;
                  frmNew.Show();
               }
               break;
            case "CR":
               if (Key == "CR")
               {
                  MessageBox.Show(this, "A specific Counter must be selected to edit.", "Edit Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
               }
               frmNew = new frmCounterEdit(ProjectData.GetCounter(KeyParts[0]));
               frmNew.MdiParent = this;
               frmNew.Show();
               break;
            case "SD":
               if (Key == "SD")
               {
                  MessageBox.Show(this, "A specific Sprite Definition must be selected to edit.", "Edit Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
               }
               frmNew = new frmSpriteDefinition(ProjectData.GetSpriteDefinition(KeyParts[0]));
               frmNew.MdiParent = this;
               frmNew.Show();
               break;
            case "SY":
               if (Key == "SY")
               {
                  MessageBox.Show(this, "A specific Solidity must be selected to edit.", "Edit Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
               }
               frmNew = new frmSolidity(ProjectData.GetSolidity(KeyParts[0]));
               frmNew.MdiParent = this;
               frmNew.Show();
               break;
            case "MP":
               if (Key == "MP")
               {
                  MessageBox.Show(this, "A specific Map must be selected to edit.", "Edit Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
               }
               frmNew = new frmMapManager(ProjectData.GetMap(KeyParts[0]));
               frmNew.MdiParent = this;
               frmNew.Show();
               break;
            case "LR":
               if (KeyParts.Length <= 1)
               {
                  MessageBox.Show(this, "A specific Layer must be selected to edit.", "Edit Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
               }
               frmNew = new frmLayerManager(ProjectData.GetLayer(KeyParts[0], KeyParts[1]));
               frmNew.MdiParent = this;
               frmNew.Show();
               break;
            case "LE":
               frmNew = new frmMapEditor(ProjectData.GetLayer(KeyParts[0], KeyParts[1]));
               frmNew.MdiParent = this;
               frmNew.Show();
               break;
            case "PL":
               if (KeyParts.Length <= 2)
               {
                  MessageBox.Show(this, "A specific Plan must be selected to edit.", "Edit Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
               }
               frmNew = new frmPlanEdit(ProjectData.GetSpritePlan(KeyParts[0], KeyParts[1], KeyParts[2]));
               frmNew.MdiParent = this;
               frmNew.Show();
               break;
            case "SC":
               if (Key == "SC")
               {
                  MessageBox.Show(this, "A specific Sprite Category must be selected to edit.", "Edit Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
               }
               frmNew = new frmSpriteCategory(ProjectData.GetSpriteCategory(KeyParts[0]));
               frmNew.MdiParent = this;
               frmNew.Show();
               break;
            case "CD":
               if (Key == "CD")
               {
                  MessageBox.Show(this, "A specific Code object must be selected to edit.", "Edit Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
               }
               if (KeyParts[KeyParts.Length - 1].EndsWith(".dll"))
               {
                  MessageBox.Show(this, "Cannot edit DLL reference items.", "Edit Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
               }
               ProjectDataset.SourceCodeRow row = ProjectData.GetSourceCode(KeyParts[KeyParts.Length - 1]);
               if (row == null)
                  row = ProjectData.AddSourceCode(KeyParts[0], String.Empty, String.Empty, true, null);
               frmNew = new frmCodeEditor(row);
               frmNew.MdiParent = this;
               frmNew.Show();
               break;
            default:
               MessageBox.Show(this, "Cannot display editor for the selected type", "Edit Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return;
         }
      }

      private void DeleteObject()
      {
         string Key = m_ContextNode.Tag.ToString();
         string[] KeyParts = Key.Substring(2).Split('~');
         try
         {
            switch(Key.Substring(0,2))
            {
               case "GS":
               case "GE":
                  if (Key == "GS")
                  {
                     MessageBox.Show(this, "A specific graphic sheet must be selected to delete.", "Delete Graphic Sheet", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     return;
                  }
                  System.Text.StringBuilder sbReferences = new System.Text.StringBuilder();
                  foreach (System.Data.DataRowView drv in ProjectData.Frameset.DefaultView)
                  {
                     ProjectDataset.FramesetRow fsr = (ProjectDataset.FramesetRow)drv.Row;
                     foreach(ProjectDataset.FrameRow fr in fsr.GetFrameRows())
                     {
                        if (fr.GraphicSheet == KeyParts[0])
                        {
                           sbReferences.Append("   " + fsr.Name + "\r\n");
                           break;
                        }
                     }
                  }
                  if (sbReferences.Length > 0)
                  {
                     MessageBox.Show("This graphics sheet cannot be deleted because it is referenced by the following Framesets:\r\n" +
                        sbReferences.ToString(), "Delete Graphic Sheet", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     return;
                  }
                  if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete graphic sheet \"" + KeyParts[0] + "\"?", "Delete Graphic Sheet", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                  {
                     try
                     {
                        ProjectData.GetGraphicSheet(KeyParts[0]).Delete();
                     }
                     catch(System.Exception ex)
                     {
                        MessageBox.Show("An error occurred while trying to delete the graphic sheet: " + ex.Message, "Delete Graphic Sheet", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     }
                  }
                  break;
               case "FS":
                  if (Key == "FS")
                  {
                     MessageBox.Show(this, "A specific Frameset must be selected to delete.", "Delete Frameset", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     return;
                  }
                  if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete frameset \"" + KeyParts[0] + "\"?", "Delete Frameset", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                  {
                     ProjectData.GetFrameSet(KeyParts[0]).Delete();
                  }
                  break;
               case "TS":
                  if (Key == "TS")
                  {
                     MessageBox.Show(this, "A specific Tileset must be selected to delete.", "Delete Tileset", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     return;
                  }
                  if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete Tileset \"" + KeyParts[0] + "\"?", "Delete Tileset", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                  {
                     ProjectData.GetTileSet(KeyParts[0]).Delete();
                  }
                  break;
               case "TC":
                  if (Key == "TC")
                  {
                     MessageBox.Show(this, "A specific Tile Category must be selected to delete.", "Delete Tileset", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     return;
                  }
                  if (KeyParts.Length <= 1)
                  {
                     if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete Tile Category \"" + KeyParts[0] + "\"?", "Delete Tile Category", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                     {
                        ProjectData.GetTileCategory(KeyParts[0]).Delete();
                     }
                  }
                  else
                  {
                     if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete all tiles in Tileset \"" + KeyParts[1] + "\" from Tile Category \"" + KeyParts[0] + "\"?", "Delete Categorized Tiles", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                     {
                        ProjectData.GetCategorizedTileset(KeyParts[1], KeyParts[0]).Delete();
                     }
                  }
                  break;
               case "CR":
                  if (Key == "CR")
                  {
                     MessageBox.Show(this, "A specific Counter must be selected to delete.", "Delete Counter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     return;
                  }
                  if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete Counter \"" + KeyParts[0] + "\"?", "Delete Counter", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                  {
                     ProjectData.GetCounter(KeyParts[0]).Delete();
                  }
                  break;
               case "SD":
                  if (Key == "SD")
                  {
                     MessageBox.Show(this, "A specific Sprite Definition must be selected to delete.", "Delete Sprite Definition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     return;
                  }
                  if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete Sprite Definition \"" + KeyParts[0] + "\"?", "Delete Tile Shape", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                  {
                     ProjectData.GetSpriteDefinition(KeyParts[0]).Delete();
                  }
                  break;
               case "SY":
                  if (Key == "SY")
                  {
                     MessageBox.Show(this, "A specific Solidity must be selected to delete.", "Delete Solidity", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     return;
                  }
                  if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete Solidity \"" + KeyParts[0] + "\"?", "Delete Solidity", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                  {
                     ProjectData.GetSolidity(KeyParts[0]).Delete();
                  }
                  break;
               case "MP":
                  if (Key == "MP")
                  {
                     MessageBox.Show(this, "A specific Map must be selected to delete.", "Delete Map", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     return;
                  }
                  if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete Map \"" + KeyParts[0] + "\"?", "Delete Map", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                  {
                     ProjectData.GetMap(KeyParts[0]).Delete();
                  }
                  break;
               case "LR":
                  if (KeyParts.Length <= 1)
                  {
                     MessageBox.Show(this, "A specific Layer must be selected to delete.", "Delete Layer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     return;
                  }
                  goto case "LE";
               case "LE":
                  if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete Layer \"" + KeyParts[1] + "\" from Map \"" + KeyParts[0] + "\"?", "Delete Layer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                  {
                     ProjectData.GetLayer(KeyParts[0], KeyParts[1]).Delete();
                  }
                  break;
               case "PL":
                  if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete Plan \"" + KeyParts[2] + "\" from Layer \"" + KeyParts[1] +"\" in Map \"" + KeyParts[0] + "\"?", "Delete Plan", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                  {
                     ProjectData.GetSpritePlan(KeyParts[0], KeyParts[1], KeyParts[2]).Delete();
                  }
                  break;
               case "SC":
                  if (Key == "SC")
                  {
                     MessageBox.Show(this, "A specific Sprite Category must be selected to delete.", "Delete Sprite Category", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     return;
                  }
                  if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete Sprite Category \"" + KeyParts[0] + "\"?", "Delete Sprite Category", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                  {
                     ProjectData.GetSpriteCategory(KeyParts[0]).Delete();
                  }
                  break;
               case "CD":
                  ProjectDataset.SourceCodeRow row = ProjectData.GetSourceCode(KeyParts[KeyParts.Length-1]);
                  if (row != null)
                  {
                     if (!row.IsCustomObject)
                     {
                        MessageBox.Show(this, "Only custom code objects can be deleted.", "Delete Code", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                     }
                     if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete custom code object \"" + KeyParts[0] + "\"?", "Delete Source Code", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                     {
                        ProjectData.DeleteSourceCode(row);
                     }
                  }
                  break;
               default:
                  MessageBox.Show(this, "Cannot delete the selected type", "Delete Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
            }
         }
         catch(InvalidConstraintException ex)
         {
            ProjectData.RejectChanges();
            MessageBox.Show(this, ex.Message, "Delete Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
      }

      private void ImportObject()
      {
         if (m_ContextNode == tvwMain.Nodes[0])
         {
            MessageBox.Show(this, "Cannot import an object of the selected type", "Import From...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }

         string Key = m_ContextNode.Tag.ToString();
         // If a layer import wizard is implemented, might need this...
         //string[] KeyParts = Key.Substring(2).Split('~');
         switch(Key.Substring(0,2))
         {
            case "SD":
               frmSpriteImportWizard frm = new frmSpriteImportWizard();
               frm.ShowDialog(this);
               frm.Dispose();
               break;
            default:
               MessageBox.Show(this, "Only Sprite Definitions can be imported.", "Import From...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               break;
         }
      }
      private string GetSourceCodeKey(ProjectDataset.SourceCodeRow drCode, DataRowVersion version)
      {
         string name;
         if (drCode.HasVersion(version))
            name = drCode[ProjectData.SourceCode.NameColumn, version].ToString();
         else
            name = drCode.Name;
         string dep;
         if (Convert.IsDBNull(drCode[ProjectData.SourceCode.DependsOnColumn, DataRowVersion.Current]))
            dep = null;
         else
            dep = drCode[ProjectData.SourceCode.DependsOnColumn, DataRowVersion.Current].ToString();
         if ((dep == null) || (dep.Length <= 0))
            return "CD" + name;
         return GetSourceCodeKey(ProjectData.GetSourceCode(dep), version) + "~" + name;
      }
      #endregion

      #region Public Methods
      public void SelectByContext(string Context)
      {
         TreeNode ndSel = m_TreeNodes[Context] as TreeNode;
         if (ndSel != null)
         {
            ndSel.EnsureVisible();
            ndSel.Expand();
            tvwMain.SelectedNode = ndSel;
         }
      }
      #endregion

      #region Public Properties
      public string ProjectFile
      {
         get
         {
            return m_strProjectPath;
         }
      }
      #endregion

      #region Overrides
      protected override void OnClosing(CancelEventArgs e)
      {
         if (ProjectData.GetChangedTables().Length > 0)
         {
            switch (MessageBox.Show(this, "Changes to the project have not been saved.  Do you want to save them before exiting?", "Save Changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
            {
               case DialogResult.Yes:
                  if (m_strProjectPath == null)
                     mnuFileSavePrjAs_Click(this, e);
                  else
                  {
                     ProjectData.WriteXml(m_strProjectPath);
                     ProjectData.AcceptChanges();
                  }
                  if (m_strProjectPath == null)
                     e.Cancel = true;
                  break;
               case DialogResult.Cancel:
                  e.Cancel = true;
                  break;
            }
         }
         base.OnClosing (e);
      }

      protected override void OnLoad(EventArgs e)
      {
         if (SGDK2IDE.g_CommandLine.ProjectFile == null)
            DoNewProject();
         else
            try
            {
               DoOpenProject(SGDK2IDE.g_CommandLine.ProjectFile);
            }
            catch (System.Exception ex)
            {
               MessageBox.Show(this, ex.Message, "Open Project", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               DoNewProject();
            }
         base.OnLoad (e);
      }

      #endregion

      #region Event Handlers
      private void dataMonitor_GraphicSheetRowChanged(object sender, SGDK2.ProjectDataset.GraphicSheetRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            TreeNode ndNew = ((TreeNode)m_TreeNodes["GS"]).Nodes.Add(e.Row.Name);
            ndNew.Tag = "GS" + e.Row.Name;
            ndNew.SelectedImageIndex = ndNew.ImageIndex = 4;
            // Add the node to the local index
            m_TreeNodes.Add(ndNew.Tag, ndNew);

            ndNew = ndNew.Nodes.Add("Edit Images");
            ndNew.Tag = "GE" + e.Row.Name;
            ndNew.SelectedImageIndex = ndNew.ImageIndex = 7;
            ndNew.EnsureVisible();

            // Add the node to the local index
            m_TreeNodes.Add(ndNew.Tag, ndNew);
         }
      }

      private void dataMonitor_GraphicSheetRowChanging(object sender, SGDK2.ProjectDataset.GraphicSheetRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Change)
         {
            String sOldKey = e.Row[ProjectData.GraphicSheet.NameColumn, DataRowVersion.Current].ToString();
            TreeNode ndOld = (TreeNode)m_TreeNodes["GS" + sOldKey];
            TreeNode ndOldChild = (TreeNode)m_TreeNodes["GE" + sOldKey];
            m_TreeNodes.Remove("GS" + sOldKey);
            m_TreeNodes.Remove("GE" + sOldKey);
            ndOld.Tag = "GS" + (ndOld.Text = e.Row.Name);
            ndOldChild.Tag = "GE" + e.Row.Name;
            m_TreeNodes.Add(ndOld.Tag, ndOld);
            m_TreeNodes.Add(ndOldChild.Tag, ndOldChild);
         }
      }

      private void dataMonitor_GraphicSheetRowDeleting(object sender, SGDK2.ProjectDataset.GraphicSheetRowChangeEvent e)
      {
         if ((e.Action == DataRowAction.Delete) && (e.Row.HasVersion(DataRowVersion.Current)))
            m_AffectedNodeKeys["GS"] = e.Row[ProjectData.GraphicSheet.NameColumn, DataRowVersion.Current].ToString();
      }

      private void dataMonitor_GraphicSheetRowDeleted(object sender, SGDK2.ProjectDataset.GraphicSheetRowChangeEvent e)
      {
         if (m_AffectedNodeKeys.ContainsKey("GS"))
         {            
            TreeNode tnSheet = (TreeNode)m_TreeNodes["GS" + m_AffectedNodeKeys["GS"]];
            TreeNode tnEditor = (TreeNode)m_TreeNodes["GE" + m_AffectedNodeKeys["GS"]];
            m_TreeNodes.Remove("GS" + m_AffectedNodeKeys["GS"]);
            m_TreeNodes.Remove("GE" + m_AffectedNodeKeys["GS"]);
            tnSheet.Parent.Nodes.Remove(tnSheet);
            m_AffectedNodeKeys.Remove("GS");
         }
      }

      private void dataMonitor_FramesetRowChanged(object sender, SGDK2.ProjectDataset.FramesetRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            TreeNode ndNew = ((TreeNode)m_TreeNodes["FS"]).Nodes.Add(e.Row.Name);
            ndNew.Tag = "FS" + e.Row.Name;
            ndNew.SelectedImageIndex = ndNew.ImageIndex = 10;
            ndNew.EnsureVisible();

            // Add the node to the local index
            m_TreeNodes.Add(ndNew.Tag, ndNew);
         }
      }

      private void dataMonitor_FramesetRowChanging(object sender, SGDK2.ProjectDataset.FramesetRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Change)
         {
            String sOldKey = e.Row[ProjectData.Frameset.NameColumn, DataRowVersion.Current].ToString();
            TreeNode ndOld = (TreeNode)m_TreeNodes["FS" + sOldKey];
            m_TreeNodes.Remove("FS" + sOldKey);
            ndOld.Tag = "FS" + (ndOld.Text = e.Row.Name);
            m_TreeNodes.Add(ndOld.Tag, ndOld);
         }
      }

      private void dataMonitor_FramesetRowDeleting(object sender, SGDK2.ProjectDataset.FramesetRowChangeEvent e)
      {
         if ((e.Action == DataRowAction.Delete) && (e.Row.HasVersion(DataRowVersion.Current)))
            m_AffectedNodeKeys["FS"] = "FS" + e.Row[ProjectData.Frameset.NameColumn, DataRowVersion.Current];
      }

      private void dataMonitor_FramesetRowDeleted(object sender, SGDK2.ProjectDataset.FramesetRowChangeEvent e)
      {
         if (m_AffectedNodeKeys.ContainsKey("FS"))
         {
            TreeNode tnDel = (TreeNode)m_TreeNodes[m_AffectedNodeKeys["FS"]];
            m_TreeNodes.Remove(m_AffectedNodeKeys["FS"]);
            tnDel.Parent.Nodes.Remove(tnDel);
            m_AffectedNodeKeys.Remove("FS");
         }
      }

      private void dataMonitor_TilesetRowChanging(object sender, SGDK2.ProjectDataset.TilesetRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Change)
         {
            String sOldKey = e.Row[ProjectData.Tileset.NameColumn, DataRowVersion.Current].ToString();
            TreeNode ndOld = (TreeNode)m_TreeNodes["TS" + sOldKey];
            m_TreeNodes.Remove("TS" + sOldKey);
            ndOld.Tag = "TS" + (ndOld.Text = e.Row.Name);
            m_TreeNodes.Add(ndOld.Tag, ndOld);
         }
      }

      private void dataMonitor_TileCategoryRowChanging(object sender, SGDK2.ProjectDataset.TileCategoryRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Change)
         {
            String sOldKey = e.Row[ProjectData.TileCategory.NameColumn, DataRowVersion.Current].ToString();
            TreeNode ndOld = (TreeNode)m_TreeNodes["TC" + sOldKey];
            m_TreeNodes.Remove("TC" + sOldKey);
            ndOld.Tag = "TC" + (ndOld.Text = e.Row.Name);
            m_TreeNodes.Add(ndOld.Tag, ndOld);
         }
      }

      private void dataMonitor_TilesetRowDeleting(object sender, SGDK2.ProjectDataset.TilesetRowChangeEvent e)
      {
         if ((e.Action == DataRowAction.Delete) && (e.Row.HasVersion(DataRowVersion.Current)))
            m_AffectedNodeKeys["TS"] = e.Row[ProjectData.Tileset.NameColumn, DataRowVersion.Current];
      }

      private void dataMonitor_TilesetRowDeleted(object sender, SGDK2.ProjectDataset.TilesetRowChangeEvent e)
      {
         if (m_AffectedNodeKeys.ContainsKey("TS"))
         {
            TreeNode tnDel = (TreeNode)m_TreeNodes["TS" + m_AffectedNodeKeys["TS"]];
            m_TreeNodes.Remove("TS" + m_AffectedNodeKeys["TS"]);
            tnDel.Parent.Nodes.Remove(tnDel);
            m_AffectedNodeKeys.Remove("TS");
         }
      }

      private void dataMonitor_TilesetRowChanged(object sender, SGDK2.ProjectDataset.TilesetRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            TreeNode ndNew = ((TreeNode)m_TreeNodes["TS"]).Nodes.Add(e.Row.Name);
            ndNew.Tag = "TS" + e.Row.Name;
            ndNew.SelectedImageIndex = ndNew.ImageIndex = 12;
            // Add the node to the local index
            m_TreeNodes.Add(ndNew.Tag, ndNew);
         }
      }

      private void dataMonitor_TileCategoryRowChanged(object sender, SGDK2.ProjectDataset.TileCategoryRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            TreeNode ndNew = ((TreeNode)m_TreeNodes["TC"]).Nodes.Add(e.Row.Name);
            ndNew.Tag = "TC" + e.Row.Name;
            ndNew.SelectedImageIndex = ndNew.ImageIndex = 17;
            // Add the node to the local index
            m_TreeNodes.Add(ndNew.Tag, ndNew);
         }
      }

      private void dataMonitor_CounterRowChanged(object sender, SGDK2.ProjectDataset.CounterRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            TreeNode ndNew = ((TreeNode)m_TreeNodes["CR"]).Nodes.Add(e.Row.Name);
            ndNew.Tag = "CR" + e.Row.Name;
            ndNew.SelectedImageIndex = ndNew.ImageIndex = 13;
            ndNew.EnsureVisible();
            // Add the node to the local index
            m_TreeNodes.Add(ndNew.Tag, ndNew);
         }
      }

      private void dataMonitor_CounterRowChanging(object sender, SGDK2.ProjectDataset.CounterRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Change)
         {
            String sOldKey = e.Row[ProjectData.Counter.NameColumn, DataRowVersion.Current].ToString();
            TreeNode ndOld = (TreeNode)m_TreeNodes["CR" + sOldKey];
            m_TreeNodes.Remove("CR" + sOldKey);
            ndOld.Tag = "CR" + (ndOld.Text = e.Row.Name);
            m_TreeNodes.Add(ndOld.Tag, ndOld);
         }      
      }

      private void dataMonitor_CounterRowDeleting(object sender, SGDK2.ProjectDataset.CounterRowChangeEvent e)
      {
         if ((e.Action == DataRowAction.Delete) && (e.Row.HasVersion(DataRowVersion.Current)))
            m_AffectedNodeKeys["CR"] = "CR" + e.Row[ProjectData.Counter.NameColumn, DataRowVersion.Current];
      }

      private void dataMonitor_CounterRowDeleted(object sender, SGDK2.ProjectDataset.CounterRowChangeEvent e)
      {
         if (m_AffectedNodeKeys.ContainsKey("CR"))
         {
            TreeNode tnDel = (TreeNode)m_TreeNodes[m_AffectedNodeKeys["CR"]];
            m_TreeNodes.Remove(m_AffectedNodeKeys["CR"]);
            tnDel.Parent.Nodes.Remove(tnDel);
            m_AffectedNodeKeys.Remove("CR");
         }      
      }

      private void dataMonitor_SolidityRowChanged(object sender, SGDK2.ProjectDataset.SolidityRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            TreeNode ndNew = ((TreeNode)m_TreeNodes["SY"]).Nodes.Add(e.Row.Name);
            ndNew.Tag = "SY" + e.Row.Name;
            ndNew.SelectedImageIndex = ndNew.ImageIndex = 19;
            ndNew.EnsureVisible();
            // Add the node to the local index
            m_TreeNodes.Add(ndNew.Tag, ndNew);
         }      
      }

      private void dataMonitor_SolidityRowChanging(object sender, SGDK2.ProjectDataset.SolidityRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Change)
         {
            String sOldKey = e.Row[ProjectData.Solidity.NameColumn, DataRowVersion.Current].ToString();
            TreeNode ndOld = (TreeNode)m_TreeNodes["SY" + sOldKey];
            m_TreeNodes.Remove("SY" + sOldKey);
            ndOld.Tag = "SY" + (ndOld.Text = e.Row.Name);
            m_TreeNodes.Add(ndOld.Tag, ndOld);
         }            
      }

      private void dataMonitor_SolidityRowDeleting(object sender, SGDK2.ProjectDataset.SolidityRowChangeEvent e)
      {
         if ((e.Action == DataRowAction.Delete) && (e.Row.HasVersion(DataRowVersion.Current)))
            m_AffectedNodeKeys["SY"] = "SY" + e.Row[ProjectData.Solidity.NameColumn, DataRowVersion.Current];
      }

      private void dataMonitor_SolidityRowDeleted(object sender, SGDK2.ProjectDataset.SolidityRowChangeEvent e)
      {
         if (m_AffectedNodeKeys.ContainsKey("SY"))
         {
            TreeNode tnDel = (TreeNode)m_TreeNodes[m_AffectedNodeKeys["SY"]];
            m_TreeNodes.Remove(m_AffectedNodeKeys["SY"]);
            tnDel.Parent.Nodes.Remove(tnDel);
            m_AffectedNodeKeys.Remove("SY");
         }      
      }

      private void dataMonitor_MapRowChanged(object sender, SGDK2.ProjectDataset.MapRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            TreeNode ndNew = ((TreeNode)m_TreeNodes["MP"]).Nodes.Add(e.Row.Name);
            ndNew.Tag = "MP" + e.Row.Name;
            ndNew.SelectedImageIndex = ndNew.ImageIndex = 14;
            // Add the node to the local index
            m_TreeNodes.Add(ndNew.Tag, ndNew);

            ndNew = ndNew.Nodes.Add("Layers");
            ndNew.Tag = "LR" + e.Row.Name;
            ndNew.SelectedImageIndex = ndNew.ImageIndex = 1;
            ndNew.EnsureVisible();
            m_TreeNodes.Add(ndNew.Tag, ndNew);
         }      
      }

      private void dataMonitor_MapRowChanging(object sender, SGDK2.ProjectDataset.MapRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Change)
         {
            String sOldKey = e.Row[ProjectData.Map.NameColumn, DataRowVersion.Current].ToString();
            TreeNode ndOld = (TreeNode)m_TreeNodes["MP" + sOldKey];
            m_TreeNodes.Remove("MP" + sOldKey);
            ndOld.Tag = "MP" + (ndOld.Text = e.Row.Name);
            m_TreeNodes.Add(ndOld.Tag, ndOld);

            ndOld = (TreeNode)m_TreeNodes["LR" + sOldKey];
            m_TreeNodes.Remove("LR" + sOldKey);
            ndOld.Tag = "LR" + e.Row.Name;
            m_TreeNodes.Add(ndOld.Tag, ndOld);
         }            
      }

      private void dataMonitor_MapRowDeleting(object sender, SGDK2.ProjectDataset.MapRowChangeEvent e)
      {
         if ((e.Action == DataRowAction.Delete) && (e.Row.HasVersion(DataRowVersion.Current)))
            m_AffectedNodeKeys["MP"] = e.Row[ProjectData.Map.NameColumn, DataRowVersion.Current];
      }

      private void dataMonitor_MapRowDeleted(object sender, SGDK2.ProjectDataset.MapRowChangeEvent e)
      {
         if (m_AffectedNodeKeys.ContainsKey("MP"))
         {
            TreeNode tnDel = (TreeNode)m_TreeNodes["MP" + m_AffectedNodeKeys["MP"]];
            m_TreeNodes.Remove("MP" + m_AffectedNodeKeys["MP"]);
            tnDel.Parent.Nodes.Remove(tnDel);
            m_TreeNodes.Remove("LR" + m_AffectedNodeKeys["MP"]);
            m_AffectedNodeKeys.Remove("MP");
         }            
      }

      private void dataMonitor_SpriteDefinitionRowChanged(object sender, SGDK2.ProjectDataset.SpriteDefinitionRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            TreeNode ndNew = ((TreeNode)m_TreeNodes["SD"]).Nodes.Add(e.Row.Name);
            ndNew.Tag = "SD" + e.Row.Name;
            ndNew.SelectedImageIndex = ndNew.ImageIndex = 20;
            ndNew.EnsureVisible();
            // Add the node to the local index
            m_TreeNodes.Add(ndNew.Tag, ndNew);
         }      
      }

      private void dataMonitor_SpriteDefinitionRowChanging(object sender, SGDK2.ProjectDataset.SpriteDefinitionRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Change)
         {
            String sOldKey = e.Row[ProjectData.SpriteDefinition.NameColumn, DataRowVersion.Current].ToString();
            TreeNode ndOld = (TreeNode)m_TreeNodes["SD" + sOldKey];
            m_TreeNodes.Remove("SD" + sOldKey);
            ndOld.Tag = "SD" + (ndOld.Text = e.Row.Name);
            m_TreeNodes.Add(ndOld.Tag, ndOld);
         }
      }

      private void dataMonitor_SpriteDefinitionRowDeleting(object sender, SGDK2.ProjectDataset.SpriteDefinitionRowChangeEvent e)
      {
         if ((e.Action == DataRowAction.Delete) && (e.Row.HasVersion(DataRowVersion.Current)))
            m_AffectedNodeKeys["SD"] = "SD" + e.Row[ProjectData.SpriteDefinition.NameColumn, DataRowVersion.Current];
      }

      private void dataMonitor_SpriteDefinitionRowDeleted(object sender, SGDK2.ProjectDataset.SpriteDefinitionRowChangeEvent e)
      {
         if (m_AffectedNodeKeys.ContainsKey("SD"))
         {
            TreeNode tnDel = (TreeNode)m_TreeNodes[m_AffectedNodeKeys["SD"]];
            m_TreeNodes.Remove(m_AffectedNodeKeys["SD"]);
            tnDel.Parent.Nodes.Remove(tnDel);
            m_AffectedNodeKeys.Remove("SD");
         }      
      }

      private void dataMonitor_LayerRowChanged(object sender, SGDK2.ProjectDataset.LayerRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            TreeNode ndNew = ((TreeNode)m_TreeNodes["LR" + e.Row.MapRow.Name]).Nodes.Add(e.Row.Name);
            ndNew.Tag = "LR" + e.Row.MapRow.Name + "~" + e.Row.Name;
            ndNew.SelectedImageIndex = ndNew.ImageIndex = 15;
            // Add the node to the local index
            m_TreeNodes.Add(ndNew.Tag, ndNew);

            TreeNode ndEditor = ndNew.Nodes.Add("Editor");
            ndEditor.Tag = "LE" + e.Row.MapRow.Name + "~" + e.Row.Name;
            ndEditor.SelectedImageIndex = ndEditor.ImageIndex = 16;
            ndEditor.EnsureVisible();
            // Add the node to the local index
            m_TreeNodes.Add(ndEditor.Tag, ndEditor);

            TreeNode ndPlans = ndNew.Nodes.Add("Plans");
            ndPlans.Tag = "PL" + e.Row.MapRow.Name + "~" + e.Row.Name;
            ndPlans.SelectedImageIndex = ndPlans.ImageIndex = 1;
            ndPlans.EnsureVisible();
            m_TreeNodes.Add(ndPlans.Tag, ndPlans);
         }
      }

      private void dataMonitor_LayerRowChanging(object sender, SGDK2.ProjectDataset.LayerRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Change)
         {
            String sOldKey = e.Row[ProjectData.Layer.MapNameColumn, DataRowVersion.Current].ToString() + "~" + e.Row[ProjectData.Layer.NameColumn, DataRowVersion.Current].ToString();
            TreeNode ndOld = (TreeNode)m_TreeNodes["LR" + sOldKey];
            TreeNode ndOldChild = (TreeNode)m_TreeNodes["LE" + sOldKey];
            m_TreeNodes.Remove("LR" + sOldKey);
            m_TreeNodes.Remove("LE" + sOldKey);
            ndOld.Tag = "LR" + e.Row[ProjectData.Layer.MapNameColumn,DataRowVersion.Proposed].ToString() +
               "~" + e.Row.Name;
            ndOld.Text = e.Row.Name;
            ndOldChild.Tag = "LE" + e.Row[ProjectData.Layer.MapNameColumn,DataRowVersion.Proposed].ToString() +
               "~" + e.Row.Name;
            m_TreeNodes.Add(ndOld.Tag, ndOld);
            m_TreeNodes.Add(ndOldChild.Tag, ndOldChild);
         }
      }

      private void dataMonitor_LayerRowDeleting(object sender, SGDK2.ProjectDataset.LayerRowChangeEvent e)
      {
         if ((e.Action == DataRowAction.Delete) && (e.Row.HasVersion(DataRowVersion.Current)))
            m_AffectedNodeKeys["LR"] = e.Row[ProjectData.Layer.MapNameColumn, DataRowVersion.Current].ToString() + "~" + e.Row[ProjectData.Layer.NameColumn, DataRowVersion.Current].ToString();
      }

      private void dataMonitor_LayerRowDeleted(object sender, SGDK2.ProjectDataset.LayerRowChangeEvent e)
      {
         if (m_AffectedNodeKeys.ContainsKey("LR") &&  (e.Action == DataRowAction.Delete))
         {
            TreeNode tnSheet = (TreeNode)m_TreeNodes["LR" + m_AffectedNodeKeys["LR"]];
            TreeNode tnEditor = (TreeNode)m_TreeNodes["LE" + m_AffectedNodeKeys["LR"]];
            m_TreeNodes.Remove("LR" + m_AffectedNodeKeys["LR"]);
            m_TreeNodes.Remove("LE" + m_AffectedNodeKeys["LR"]);
            m_TreeNodes.Remove("PL" + m_AffectedNodeKeys["LR"]);
            tnSheet.Parent.Nodes.Remove(tnSheet);
            m_AffectedNodeKeys.Remove("LR");
         }
      }

      private void dataMonitor_SpritePlanRowChanged(object sender, SGDK2.ProjectDataset.SpritePlanRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            string parentKey = "PL" + e.Row.LayerRowParent.MapRow.Name + "~" + e.Row.LayerRowParent.Name;
            TreeNode ndNew = ((TreeNode)m_TreeNodes[parentKey]).Nodes.Add(e.Row.Name);
            ndNew.Tag = parentKey + "~" + e.Row.Name;
            ndNew.SelectedImageIndex = ndNew.ImageIndex = 22;
            // Add the node to the local index
            m_TreeNodes.Add(ndNew.Tag, ndNew);
         }
      }

      private void dataMonitor_SpritePlanRowChanging(object sender, SGDK2.ProjectDataset.SpritePlanRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Change)
         {
            String sOldKey = "PL" + e.Row[ProjectData.SpritePlan.MapNameColumn, DataRowVersion.Current].ToString()
               + "~" + e.Row[ProjectData.SpritePlan.LayerNameColumn, DataRowVersion.Current].ToString()
               + "~" + e.Row[ProjectData.SpritePlan.NameColumn, DataRowVersion.Current].ToString();
            TreeNode ndOld = (TreeNode)m_TreeNodes[sOldKey];
            m_TreeNodes.Remove(sOldKey);
            ndOld.Tag = "PL" + e.Row[ProjectData.SpritePlan.MapNameColumn, DataRowVersion.Proposed].ToString() + 
               "~" + e.Row[ProjectData.SpritePlan.LayerNameColumn,DataRowVersion.Proposed].ToString() +
               "~" + e.Row.Name;
            ndOld.Text = e.Row.Name;
            m_TreeNodes.Add(ndOld.Tag, ndOld);
         }
      }

      private void dataMonitor_SpritePlanRowDeleting(object sender, SGDK2.ProjectDataset.SpritePlanRowChangeEvent e)
      {
         if ((e.Action == DataRowAction.Delete) && (e.Row.HasVersion(DataRowVersion.Current)))
            m_AffectedNodeKeys["PL"] = "PL" + e.Row[ProjectData.SpritePlan.MapNameColumn, DataRowVersion.Current].ToString()
               + "~" + e.Row[ProjectData.SpritePlan.LayerNameColumn, DataRowVersion.Current].ToString()
               + "~" + e.Row[ProjectData.SpritePlan.NameColumn, DataRowVersion.Current].ToString();
      }

      private void dataMonitor_SpritePlanRowDeleted(object sender, SGDK2.ProjectDataset.SpritePlanRowChangeEvent e)
      {
         if (m_AffectedNodeKeys.ContainsKey("PL") && (e.Action == DataRowAction.Delete))
         {
            TreeNode tnPlan = (TreeNode)m_TreeNodes[m_AffectedNodeKeys["PL"]];
            m_TreeNodes.Remove(m_AffectedNodeKeys["PL"]);
            tnPlan.Parent.Nodes.Remove(tnPlan);
            m_AffectedNodeKeys.Remove("PL");
         }      
      }

      private void dataMonitor_CategorizedTilesetRowChanged(object sender, SGDK2.ProjectDataset.CategorizedTilesetRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            TreeNode ndNew = ((TreeNode)m_TreeNodes["TC" + e.Row.Name]).Nodes.Add(e.Row.Tileset);
            ndNew.Tag = "TC" + e.Row.Name + "~" + e.Row.TilesetRow.Name;
            ndNew.SelectedImageIndex = ndNew.ImageIndex = 12;
            ndNew.EnsureVisible();
            // Add the node to the local index
            m_TreeNodes.Add(ndNew.Tag, ndNew);
         }      
      }

      private void dataMonitor_CategorizedTilesetRowChanging(object sender, SGDK2.ProjectDataset.CategorizedTilesetRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Change)
         {
            String sOldKey = e.Row[ProjectData.CategorizedTileset.NameColumn, DataRowVersion.Current].ToString() + "~" + e.Row[ProjectData.CategorizedTileset.TilesetColumn, DataRowVersion.Current].ToString();
            TreeNode ndOld = (TreeNode)m_TreeNodes["TC" + sOldKey];
            m_TreeNodes.Remove("TC" + sOldKey);
            ndOld.Tag = "TC" + e.Row[ProjectData.CategorizedTileset.NameColumn, DataRowVersion.Proposed].ToString() + "~" + e.Row.Tileset;
            ndOld.Text = e.Row.Name;
            m_TreeNodes.Add(ndOld.Tag, ndOld);
         }      
      }

      private void dataMonitor_CategorizedTilesetRowDeleting(object sender, SGDK2.ProjectDataset.CategorizedTilesetRowChangeEvent e)
      {
         if ((e.Action == DataRowAction.Delete) && (e.Row.HasVersion(DataRowVersion.Current)))
            m_AffectedNodeKeys["TC2"] = "TC" + e.Row[ProjectData.CategorizedTileset.NameColumn, DataRowVersion.Current].ToString() + "~" + e.Row[ProjectData.CategorizedTileset.TilesetColumn, DataRowVersion.Current].ToString();
      }

      private void dataMonitor_CategorizedTilesetRowDeleted(object sender, SGDK2.ProjectDataset.CategorizedTilesetRowChangeEvent e)
      {
         if (m_AffectedNodeKeys.ContainsKey("TC2"))
         {
            TreeNode tnCategory = (TreeNode)m_TreeNodes[m_AffectedNodeKeys["TC2"]];
            m_TreeNodes.Remove(m_AffectedNodeKeys["TC2"]);
            tnCategory.Parent.Nodes.Remove(tnCategory);
            m_AffectedNodeKeys.Remove("TC2");
         }            
      }

      private void dataMonitor_TileCategoryRowDeleting(object sender, SGDK2.ProjectDataset.TileCategoryRowChangeEvent e)
      {
         if ((e.Action == DataRowAction.Delete) && (e.Row.HasVersion(DataRowVersion.Current)))
            m_AffectedNodeKeys["TC1"] = e.Row[ProjectData.TileCategory.NameColumn, DataRowVersion.Current];         
      }

      private void dataMonitor_TileCategoryRowDeleted(object sender, SGDK2.ProjectDataset.TileCategoryRowChangeEvent e)
      {
         if (m_AffectedNodeKeys.ContainsKey("TC1"))
         {
            TreeNode tnDel = (TreeNode)m_TreeNodes["TC" + m_AffectedNodeKeys["TC1"]];
            m_TreeNodes.Remove("TC" + m_AffectedNodeKeys["TC1"]);
            tnDel.Parent.Nodes.Remove(tnDel);
            m_AffectedNodeKeys.Remove("TC1");
         }
      }

      private void dataMonitor_SpriteCategoryRowChanged(object sender, SGDK2.ProjectDataset.SpriteCategoryRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            TreeNode ndNew = ((TreeNode)m_TreeNodes["SC"]).Nodes.Add(e.Row.Name);
            ndNew.Tag = "SC" + e.Row.Name;
            ndNew.SelectedImageIndex = ndNew.ImageIndex = 21;
            ndNew.EnsureVisible();
            // Add the node to the local index
            m_TreeNodes.Add(ndNew.Tag, ndNew);
         }            
      }

      private void dataMonitor_SpriteCategoryRowChanging(object sender, SGDK2.ProjectDataset.SpriteCategoryRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Change)
         {
            String sOldKey = e.Row[ProjectData.SpriteCategory.NameColumn, DataRowVersion.Current].ToString();
            TreeNode ndOld = (TreeNode)m_TreeNodes["SC" + sOldKey];
            m_TreeNodes.Remove("SC" + sOldKey);
            ndOld.Tag = "SC" + e.Row.Name;
            ndOld.Text = e.Row.Name;
            m_TreeNodes.Add(ndOld.Tag, ndOld);
         }            
      }

      private void dataMonitor_SpriteCategoryRowDeleting(object sender, SGDK2.ProjectDataset.SpriteCategoryRowChangeEvent e)
      {
         if ((e.Action == DataRowAction.Delete) && (e.Row.HasVersion(DataRowVersion.Current)))
            m_AffectedNodeKeys["SC"] = "SC" + e.Row[ProjectData.SpriteCategory.NameColumn, DataRowVersion.Current].ToString();
      }

      private void dataMonitor_SpriteCategoryRowDeleted(object sender, SGDK2.ProjectDataset.SpriteCategoryRowChangeEvent e)
      {
         if (m_AffectedNodeKeys.ContainsKey("SC"))
         {
            TreeNode tnCategory = (TreeNode)m_TreeNodes[m_AffectedNodeKeys["SC"]];
            m_TreeNodes.Remove(m_AffectedNodeKeys["SC"]);
            tnCategory.Parent.Nodes.Remove(tnCategory);
            m_AffectedNodeKeys.Remove("SC");
         }                  
      }

      private void dataMonitor_SourceCodeRowChanged(object sender, SGDK2.ProjectDataset.SourceCodeRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
         {
            string parent = String.Empty;
            ProjectDataset.SourceCodeRow treeRow = e.Row;
            while (!treeRow.IsDependsOnNull() && treeRow.DependsOn.Length > 0)
            {
               if (parent.Length > 0)
                  parent = "~" + parent;
               parent = treeRow.DependsOn + parent;
               treeRow = ProjectData.GetSourceCode(treeRow.DependsOn);
            }
            TreeNode ndNew = ((TreeNode)m_TreeNodes["CD" + parent]).Nodes.Add(e.Row.Name);
            if (parent.Length == 0)
               ndNew.Tag = "CD" + e.Row.Name;
            else
               ndNew.Tag = "CD" + parent + "~" + e.Row.Name;
            if (e.Row.IsCustomObject)
               ndNew.SelectedImageIndex = ndNew.ImageIndex = 24;
            else
               ndNew.SelectedImageIndex = ndNew.ImageIndex = 23;
            ndNew.EnsureVisible();
            // Add the node to the local index
            m_TreeNodes.Add(ndNew.Tag, ndNew);
            CodeGenerator.ResetTempAssembly();
         }
      }

      private void dataMonitor_SourceCodeRowChanging(object sender, SGDK2.ProjectDataset.SourceCodeRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Change)
         {
            String sOldKey = GetSourceCodeKey(e.Row, DataRowVersion.Current);
            TreeNode ndOld = (TreeNode)m_TreeNodes[sOldKey];
            m_TreeNodes.Remove(sOldKey);
            ndOld.Tag = GetSourceCodeKey(e.Row, DataRowVersion.Proposed);
            ndOld.Text = e.Row.Name;
            m_TreeNodes.Add(ndOld.Tag, ndOld);
            foreach(ProjectDataset.SourceCodeRow child in ProjectData.GetDependentSourceCode(e.Row))
               child.DependsOn = e.Row.Name;
            CodeGenerator.ResetTempAssembly();
         }                  
      }

      private void dataMonitor_SourceCodeRowDeleting(object sender, SGDK2.ProjectDataset.SourceCodeRowChangeEvent e)
      {
         if ((e.Action == DataRowAction.Delete) && (e.Row.HasVersion(DataRowVersion.Current)))
            m_AffectedNodeKeys["CD"] = GetSourceCodeKey(e.Row, DataRowVersion.Current);
      }

      private void dataMonitor_SourceCodeRowDeleted(object sender, SGDK2.ProjectDataset.SourceCodeRowChangeEvent e)
      {
         if (m_AffectedNodeKeys.ContainsKey("CD"))
         {
            TreeNode tnCode = (TreeNode)m_TreeNodes[m_AffectedNodeKeys["CD"]];
            m_TreeNodes.Remove(m_AffectedNodeKeys["CD"]);
            tnCode.Parent.Nodes.Remove(tnCode);
            m_AffectedNodeKeys.Remove("CD");
            CodeGenerator.ResetTempAssembly();
         }                        
      }

      private void MainPanel_MouseLeave(object sender, System.EventArgs e)
      {
         if (sender == lblProjectTree)
            ((Control)sender).Invalidate();
      }

      private void lblProjectTree_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         Int32 PinWidth = lblProjectTree.ImageList.Images[lblProjectTree.ImageIndex].Width;
         Rectangle rcPin = new Rectangle(lblProjectTree.Width - PinWidth, 0, PinWidth, lblProjectTree.Height);
         rcPin.Offset(-4,0);

         if (rcPin.Contains(new Point(e.X, e.Y)))
         {
            Graphics g = Graphics.FromHwnd(lblProjectTree.Handle);
            if (e.Button == MouseButtons.Left)
               ControlPaint.DrawBorder3D(g, rcPin, Border3DStyle.Sunken);
            else
               ControlPaint.DrawBorder3D(g, rcPin, Border3DStyle.Raised);
            g.Dispose();
            m_bOverPin = true;
         }
         else
         {
            lblProjectTree.Invalidate();
            m_bOverPin = false;
         }
      }

      private void lblProjectTree_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         if (m_bOverPin)
         {
            if (m_nPinStatus == 0)
            {
               pnlProjectTree.Visible = splitterMDI.Visible = false;
               lblProjectTree.ImageIndex = 3;
               m_nPinStatus = 1;
               tbbShowTree.Pushed = false;
               tbbShowTree.Visible = true;
            }
            else
            {
               lblProjectTree.ImageIndex = 2;
               m_nPinStatus = 0;
               tbbShowTree.Visible = false;
            }
         }
      }

      private void tvwMain_Leave(object sender, System.EventArgs e)
      {
         if (m_nPinStatus == 2)
         {
            tbbShowTree.Pushed = false;
            m_nPinStatus = 1;
            pnlProjectTree.Visible = splitterMDI.Visible = false;
         }
      }

      private void tbrMain_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
      {
         if (e.Button == tbbShowTree)
         {
            if (pnlProjectTree.Visible)
            {
               m_nPinStatus = 1;
               tbbShowTree.Pushed = false;
               pnlProjectTree.Visible = splitterMDI.Visible = false;
            }
            else
            {
               m_nPinStatus = 2;
               tbbShowTree.Pushed = true;
               pnlProjectTree.Visible = splitterMDI.Visible = true;
               tvwMain.Focus();
            }
         }
         else if (e.Button == tbbNew)
            mnuFileNewObj_Click(tbbNew, e);
         else if (e.Button == tbbProp)
            mnuEditProperties_Click(tbbProp, e);
         else if (e.Button == tbbOpen)
            mnuFileOpenPrj_Click(tbbOpen, e);
         else if (e.Button == tbbDelete)
            mnuFileDeleteObj_Click(tbbDelete, e);
      }

      private void mnuFileExit_Click(object sender, System.EventArgs e)
      {
         // Close each form to trigger its closing event.
         // If there are errors (cancels), don't exit.
         while (MdiChildren.Length > 0)
         {
            Form frmClose = MdiChildren[0];
            frmClose.Close();
            if ((MdiChildren.Length > 0) && (MdiChildren[0] == frmClose))
               return;
         }
         Application.Exit();
      }

      private void mnuFileNewObj_Click(object sender, System.EventArgs e)
      {
         if (tvwMain.SelectedNode == null)
         {
            MessageBox.Show(this, "Please make a selection from the Project Tree before attempting to create a new object.", "Create New Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         if (!(tvwMain.SelectedNode.Tag is string))
         {
            MessageBox.Show(this, "Cannot create a new object of the selected type", "Create New Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         m_ContextNode = tvwMain.SelectedNode;
         NewObject();
      }

      private void mnuEditProperties_Click(object sender, System.EventArgs e)
      {
         if (tvwMain.SelectedNode == null)
         {
            MessageBox.Show(this, "Please make a selection from the Project Tree before attempting to edit objects.", "Edit Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         if (!(tvwMain.SelectedNode.Tag is string))
         {
            MessageBox.Show(this, "Cannot display editor for the selected type", "Edit Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         m_ContextNode = tvwMain.SelectedNode;
         EditObject();
      }

      private void mnuTreeNew_Click(object sender, System.EventArgs e)
      {
         if (m_ContextNode == null)
            mnuFileNewObj_Click(mnuTreeNew, e);
         else
            NewObject();
      }

      private void mnuTreeEdit_Click(object sender, System.EventArgs e)
      {
         if (m_ContextNode == null)
            mnuEditProperties_Click(mnuTreeEdit, e);
         else
            EditObject();
      }

      private void tvwMain_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         m_ContextNode = tvwMain.GetNodeAt(e.X, e.Y);
      }

      private void tvwMain_DoubleClick(object sender, System.EventArgs e)
      {
         mnuEditProperties_Click(tvwMain, e);
      }

      private void mnuFileNewPrj_Click(object sender, System.EventArgs e)
      {
         try
         {
            DoNewProject();
         }
         catch (Exception ex)
         {
            MessageBox.Show(this, ex.Message, "New Project", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void mnuFileSavePrj_Click(object sender, System.EventArgs e)
      {
         try
         {
            if (m_strProjectPath == null)
            {
               mnuFileSavePrjAs_Click(sender, e);
               return;
            }
            ProjectData.WriteXml(m_strProjectPath);
            ProjectData.AcceptChanges();
         }
         catch(Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Save Project", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void mnuFileSavePrjAs_Click(object sender, System.EventArgs e)
      {
         try
         {
            SaveFileDialog fd = new SaveFileDialog();
            fd.CheckPathExists = true;
            fd.OverwritePrompt = true;
            fd.DefaultExt = "sgdk2";
            fd.Filter = "SGDK2 Projects (*.sgdk2)|*.sgdk2|All Files (*.*)|*.*";
            fd.FilterIndex = 1;
            fd.Title = "Save Project";
            fd.ValidateNames = true;
            if (DialogResult.OK == fd.ShowDialog(this))
            {
               ProjectData.WriteXml(fd.FileName);
               m_strProjectPath = fd.FileName;
               mnuFileDeleteOutputFiles.Enabled = true;
            }
         }
         catch(Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Save Project As...", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void mnuFileOpenPrj_Click(object sender, System.EventArgs e)
      {
         try
         {
            OpenFileDialog fd = new OpenFileDialog();
            fd.CheckFileExists = true;
            fd.DefaultExt = "sgdk2";
            fd.Filter = "SGDK2 Projects (*.sgdk2)|*.sgdk2|All Files (*.*)|*.*";
            fd.FilterIndex = 1;
            fd.Multiselect = false;
            fd.Title = "Open Project";
            if (DialogResult.OK == fd.ShowDialog(this))
            {
               DoOpenProject(fd.FileName);
            }
         }
         catch(Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Open Project", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void mnuFileDeleteObj_Click(object sender, System.EventArgs e)
      {
         if (tvwMain.SelectedNode == null)
         {
            MessageBox.Show(this, "Please make a selection from the Project Tree before attempting to delete an object.", "Delete Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         if (!(tvwMain.SelectedNode.Tag is string))
         {
            MessageBox.Show(this, "Cannot delete the selected object", "Delete Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         m_ContextNode = tvwMain.SelectedNode;
         DeleteObject();
      }

      private void mnuFileImportObj_Click(object sender, System.EventArgs e)
      {
         if (tvwMain.SelectedNode == null)
         {
            MessageBox.Show(this, "Please make a selection from the Project Tree (to indicate what kind of object to import) before attempting to import an object.", "Import From...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }

         if (!(tvwMain.SelectedNode.Tag is string))
         {
            MessageBox.Show(this, "Cannot import the selected type of object", "Import From...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         m_ContextNode = tvwMain.SelectedNode;
         ImportObject();
      }

      private void mnuFileResetCode_Click(object sender, System.EventArgs e)
      {
         DataView dv = new DataView(ProjectData.SourceCode, "IsCustomObject=false", String.Empty, DataViewRowState.CurrentRows);

         string[] embeddedCodeFiles = System.Reflection.Assembly.GetAssembly(typeof(SGDK2IDE)).GetManifestResourceNames();
         for (int rowIdx = 0; rowIdx < dv.Count; rowIdx++)
         {
            int rscIdx;
            for (rscIdx = 0; rscIdx < embeddedCodeFiles.Length; rscIdx++)
            {
               if (embeddedCodeFiles[rscIdx].StartsWith("SGDK2.Template."))
               {
                  if (string.Compare(((ProjectDataset.SourceCodeRow)dv[rowIdx].Row).Name, embeddedCodeFiles[rscIdx].Substring(15), true) == 0)
                     break;
               }
            }
            if (rscIdx >= embeddedCodeFiles.Length)
               dv[rowIdx--].Row.Delete();
         }

         foreach (string resourceName in embeddedCodeFiles)
         {
            if (resourceName.StartsWith("SGDK2.Template."))
            {
               System.IO.TextReader stm = new System.IO.StreamReader(System.Reflection.Assembly.GetAssembly(typeof(SGDK2IDE)).GetManifestResourceStream(resourceName));

               int rowIdx;
               for (rowIdx = 0; rowIdx < dv.Count; rowIdx++)
               {
                  if (string.Compare(((ProjectDataset.SourceCodeRow)dv[rowIdx].Row).Name, resourceName.Substring(15), true) == 0)
                  {
                     string resetText = stm.ReadToEnd();
                     if (((ProjectDataset.SourceCodeRow)dv[rowIdx].Row).Text != resetText)
                        ((ProjectDataset.SourceCodeRow)dv[rowIdx].Row).Text = resetText;
                     break;
                  }
               }
               if (rowIdx >= dv.Count)
                  ProjectData.AddSourceCode(resourceName.Substring(15), stm.ReadToEnd(), null, false, null);
               stm.Close();
            }
         }
      }

      private void mnuFileGenerate_Click(object sender, System.EventArgs e)
      {
         if (m_strProjectPath == null)
         {
            mnuFileSavePrj_Click(sender, e);
            if (m_strProjectPath == null)
               return;
         }

         CodeGenerator.ResetTempAssembly();

         string strFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(m_strProjectPath), System.IO.Path.GetFileNameWithoutExtension(m_strProjectPath));
         CodeGenerator g = new CodeGenerator();
         g.GeneratorOptions.BracingStyle = "C";
         string errs;
         string outFile = g.CompileProject(System.IO.Path.GetFileNameWithoutExtension(m_strProjectPath), strFolder, out errs);
         if (errs.Length > 0)
         {
            frmLogView frm = new frmLogView(errs);
            frm.MdiParent = this;
            frm.Show();
            return;
         }
         MessageBox.Show(this, outFile + " Compiled", "Generate Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }

      private void mnuViewChanges_Click(object sender, System.EventArgs e)
      {
         frmUnsavedChanges frmNew = new frmUnsavedChanges();
         frmNew.MdiParent = this;
         frmNew.Show();
      }

      private void mnuFileRunProject_Click(object sender, System.EventArgs e)
      {
         if (m_strProjectPath == null)
         {
            mnuFileSavePrj_Click(sender, e);
            if (m_strProjectPath == null)
               return;
         }

         CodeGenerator.ResetTempAssembly();

         string strFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(m_strProjectPath), System.IO.Path.GetFileNameWithoutExtension(m_strProjectPath));
         CodeGenerator g = new CodeGenerator();
         g.GeneratorOptions.BracingStyle = "C";
         string errs;
         string outFile = g.CompileProject(System.IO.Path.GetFileNameWithoutExtension(m_strProjectPath), strFolder, out errs);
         if (errs.Length > 0)
         {
            frmLogView frm = new frmLogView(errs);
            frm.MdiParent = this;
            frm.Show();
            return;
         }
         System.Diagnostics.Process.Start(outFile);
      }

      private void mnuFileDeleteOutputFiles_Click(object sender, System.EventArgs e)
      {
         CodeGenerator g = new CodeGenerator();
         string strFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(m_strProjectPath), System.IO.Path.GetFileNameWithoutExtension(m_strProjectPath));
         System.Collections.Specialized.StringCollection deleteFiles = new System.Collections.Specialized.StringCollection();
         deleteFiles.AddRange(g.GetCodeFileList(strFolder));
         deleteFiles.AddRange(g.GetResxFileList(strFolder));
         deleteFiles.AddRange(g.GetResourcesFileList(strFolder));
         deleteFiles.AddRange(g.GetLocalReferenceFileList(strFolder));
         deleteFiles.AddRange(g.GetEmbeddedResourceList(strFolder));
         deleteFiles.Add(System.IO.Path.Combine(strFolder, System.IO.Path.GetFileNameWithoutExtension(m_strProjectPath) + ".exe"));
         deleteFiles.Add(System.IO.Path.Combine(strFolder, System.IO.Path.GetFileNameWithoutExtension(m_strProjectPath) + ".pdb"));
         foreach(string deleteFile in deleteFiles)
         {
            if (System.IO.File.Exists(deleteFile))
               System.IO.File.Delete(deleteFile);
         }
      }

      private void mnuFileRunProjectInDebugMode_Click(object sender, System.EventArgs e)
      {
         if (m_strProjectPath == null)
         {
            mnuFileSavePrj_Click(sender, e);
            if (m_strProjectPath == null)
               return;
         }

         CodeGenerator.ResetTempAssembly();

         string strFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(m_strProjectPath), System.IO.Path.GetFileNameWithoutExtension(m_strProjectPath));
         CodeGenerator g = new CodeGenerator();
         g.GeneratorOptions.BracingStyle = "C";
         g.Debug = true;
         string errs;
         string outFile = g.CompileProject(System.IO.Path.GetFileNameWithoutExtension(m_strProjectPath), strFolder, out errs);
         if (errs.Length > 0)
         {
            frmLogView frm = new frmLogView(errs);
            frm.MdiParent = this;
            frm.Show();
            return;
         }
         System.Diagnostics.Process.Start(outFile);      
      }

      private void mnuHelpAbout_Click(object sender, System.EventArgs e)
      {
         new frmAbout().ShowDialog(this);
      }

      private void mnuTreeDelete_Click(object sender, System.EventArgs e)
      {
         if (m_ContextNode == null)
            mnuFileDeleteObj_Click(mnuTreeDelete, null);
         else
            DeleteObject();
      }

      private void mnuTreeImport_Click(object sender, System.EventArgs e)
      {
         if (m_ContextNode == null)
            mnuFileImportObj_Click(mnuTreeImport, null);
         else
            ImportObject();
      }
      #endregion
   }
}
