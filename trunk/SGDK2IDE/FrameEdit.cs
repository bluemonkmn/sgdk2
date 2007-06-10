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
using System.Drawing.Drawing2D;

namespace SGDK2
{
	/// <summary>
	/// Summary description for FrameEdit.
	/// </summary>
   public class frmFrameEdit : System.Windows.Forms.Form
   {
      #region Private Enums
      enum GrabHandlePosition
      {
         TopLeft,
         TopCenter,
         TopRight,
         MiddleLeft,
         MiddleRight,
         BottomLeft,
         BottomCenter,
         BottomRight
      }

      enum MouseZone
      {
         Outside,
         TopLeftHandle,
         TopCenterHandle,
         TopRightHandle,
         MiddleLeftHandle,
         MiddleRightHandle,
         BottomLeftHandle,
         BottomCenterHandle,
         BottomRightHandle,
         Frame,
         Inside
      }
      #endregion

      #region Non-Control members
      Matrix m_CurrentTransform = new Matrix();
      int m_CurrentColor = -1;
      Bitmap m_CurrentImage = null;
      DateTime dtLastPaintError = DateTime.MinValue;
      MouseZone m_DragType = MouseZone.Outside;
      Point m_DragStart = Point.Empty;
      short m_nCurrentCellIndex = -1;
      Point m_ptCenter = Point.Empty;
      System.Data.DataRow m_FrameEditorSource = null;
      bool editingFrame = false;
      #endregion

      #region Windows Form Designer Components

      private System.Windows.Forms.Panel pnlFrames;
      private System.Windows.Forms.Panel pnlFrameAction;
      private System.Windows.Forms.Panel pnlTransform;
      private System.Windows.Forms.Label lblRotate;
      private System.Windows.Forms.NumericUpDown nudXOffset;
      private System.Windows.Forms.NumericUpDown nudYOffset;
      private System.Windows.Forms.Label lblYOffset;
      private System.Windows.Forms.Label lblYScale;
      private System.Windows.Forms.Label lblXScale;
      private System.Windows.Forms.Label lblXOffset;
      private System.Windows.Forms.TextBox txtRotate;
      private System.Windows.Forms.Button btnReset;
      private System.Windows.Forms.Panel pnlGraphicSheet;
      private System.Windows.Forms.ToolTip ttFrameset;
      private SGDK2.GraphicBrowser CellBrowser;
      private System.Windows.Forms.Label lblFramesetName;
      private System.Windows.Forms.TextBox txtFramesetName;
      private System.Windows.Forms.ComboBox cboGraphicSheet;
      private System.Windows.Forms.TrackBar trbRotate;
      private System.Windows.Forms.CheckBox chkRotateAroundCenter;
      private System.Windows.Forms.ContextMenu mnuContext;
      private System.Windows.Forms.MenuItem mnuHFlip;
      private System.Windows.Forms.MenuItem mnuHFlipOrigin;
      private System.Windows.Forms.MenuItem mnuHFlipCenter;
      private System.Windows.Forms.MenuItem mnuVFlip;
      private System.Windows.Forms.MenuItem mnuVFlipOrigin;
      private System.Windows.Forms.MenuItem mnuVFlipCenter;
      private System.Windows.Forms.MenuItem mnuDoubleWidth;
      private System.Windows.Forms.MenuItem mnuDoubleHeight;
      private System.Windows.Forms.MenuItem mnuHalveWidth;
      private System.Windows.Forms.MenuItem mnuHalveHeight;
      private System.Windows.Forms.MenuItem mnuLeftHalf;
      private System.Windows.Forms.MenuItem mnuRightHalf;
      private System.Windows.Forms.MenuItem mnuUpHalf;
      private System.Windows.Forms.MenuItem mnuDownHalf;
      private System.Windows.Forms.MenuItem mnuClockwise90;
      private System.Windows.Forms.MenuItem mnuClockwise90Origin;
      private System.Windows.Forms.MenuItem mnuClockwise90Center;
      private System.Windows.Forms.MenuItem mnuCounter90;
      private System.Windows.Forms.MenuItem mnuCounter90Origin;
      private System.Windows.Forms.MenuItem mnuCounter90Center;
      private System.Windows.Forms.NumericUpDown nudXScale;
      private System.Windows.Forms.NumericUpDown nudYScale;
      private SGDK2.GraphicBrowser FrameBrowser;
      private SGDK2.DataChangeNotifier dataMonitor;
      private System.Windows.Forms.MainMenu mnuFrameset;
      private System.Windows.Forms.MenuItem mnuFrameRemappingWizard;
      private System.Windows.Forms.MenuItem mnuFramesetPop;
      private System.Windows.Forms.MenuItem mnuDeleteFrames;
      private System.Windows.Forms.MenuItem mnuAddCell;
      private System.Windows.Forms.MenuItem mnuFramesetSeparator;
      private System.Windows.Forms.Splitter FrameSplitter;
      private System.Windows.Forms.PropertyGrid FrameProperties;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Label lblGraphicSheet;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Splitter splitterGraphics;
      private System.Windows.Forms.Label lblDegrees;
      private System.Windows.Forms.Panel pnlGraphicSheetName;
      private System.Windows.Forms.ToolBar tbrGraphicSheet;
      private System.Windows.Forms.ImageList imlFrameset;
      private System.Windows.Forms.ToolBarButton tbbAddToFrameset;
      private System.Windows.Forms.TabControl tabFrameset;
      private System.Windows.Forms.TabPage tpgFrameset;
      private System.Windows.Forms.TabPage tpgFrameEditor;
      private System.Windows.Forms.MenuItem mnuFramesetTransform;
      private System.Windows.Forms.MenuItem mnuXHFlip;
      private System.Windows.Forms.MenuItem mnuXHFlipOrigin;
      private System.Windows.Forms.MenuItem mnuXHFlipCenter;
      private System.Windows.Forms.MenuItem mnuXVFlip;
      private System.Windows.Forms.MenuItem mnuXVFlipOrigin;
      private System.Windows.Forms.MenuItem mnuXVFlipCenter;
      private System.Windows.Forms.MenuItem mnuXDoubleWidth;
      private System.Windows.Forms.MenuItem mnuXDoubleHeight;
      private System.Windows.Forms.MenuItem mnuXHalveWidth;
      private System.Windows.Forms.MenuItem mnuXHalveHeight;
      private System.Windows.Forms.MenuItem mnuXLeftHalf;
      private System.Windows.Forms.MenuItem mnuXRightHalf;
      private System.Windows.Forms.MenuItem mnuXUpHalf;
      private System.Windows.Forms.MenuItem mnuXDownHalf;
      private System.Windows.Forms.MenuItem mnuXClockwise90;
      private System.Windows.Forms.MenuItem mnuXClockwise90Origin;
      private System.Windows.Forms.MenuItem mnuXClockwise90Center;
      private System.Windows.Forms.MenuItem mnuXCounter90;
      private System.Windows.Forms.MenuItem mnuXCounter90Origin;
      private System.Windows.Forms.MenuItem mnuXCounter90Center;
      private System.Windows.Forms.MenuItem mnuTransformCustom;
      private System.Windows.Forms.MenuItem mnuXReset;
      private System.Windows.Forms.MenuItem mnuReset;
      private System.Windows.Forms.MenuItem mnuEditGraphicCell;
      private System.Windows.Forms.MenuItem mnuFsEditGraphicCell;
      private System.ComponentModel.IContainer components;
      #endregion

      #region Initialization and Clean-up

		public frmFrameEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         String sName;
         Int32 nIdx = 1;
         do
            sName = "New Frameset " + (nIdx++).ToString();
         while(ProjectData.GetFrameSet(sName) != null);

         FrameBrowser.Frameset = ProjectData.AddFramesetRow(sName);
         txtFramesetName.Text = sName;
      }

      public frmFrameEdit(ProjectDataset.FramesetRow dr)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         txtFramesetName.Text = dr.Name;
         FrameBrowser.Frameset = dr;
      }

      /// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
            if (m_CurrentTransform != null)
            {
               m_CurrentTransform.Dispose();
               m_CurrentTransform = null;
            }
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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmFrameEdit));
         this.pnlFrames = new System.Windows.Forms.Panel();
         this.FrameBrowser = new SGDK2.GraphicBrowser();
         this.mnuContext = new System.Windows.Forms.ContextMenu();
         this.mnuHFlip = new System.Windows.Forms.MenuItem();
         this.mnuHFlipOrigin = new System.Windows.Forms.MenuItem();
         this.mnuHFlipCenter = new System.Windows.Forms.MenuItem();
         this.mnuVFlip = new System.Windows.Forms.MenuItem();
         this.mnuVFlipOrigin = new System.Windows.Forms.MenuItem();
         this.mnuVFlipCenter = new System.Windows.Forms.MenuItem();
         this.mnuDoubleWidth = new System.Windows.Forms.MenuItem();
         this.mnuDoubleHeight = new System.Windows.Forms.MenuItem();
         this.mnuHalveWidth = new System.Windows.Forms.MenuItem();
         this.mnuHalveHeight = new System.Windows.Forms.MenuItem();
         this.mnuLeftHalf = new System.Windows.Forms.MenuItem();
         this.mnuRightHalf = new System.Windows.Forms.MenuItem();
         this.mnuUpHalf = new System.Windows.Forms.MenuItem();
         this.mnuDownHalf = new System.Windows.Forms.MenuItem();
         this.mnuClockwise90 = new System.Windows.Forms.MenuItem();
         this.mnuClockwise90Origin = new System.Windows.Forms.MenuItem();
         this.mnuClockwise90Center = new System.Windows.Forms.MenuItem();
         this.mnuCounter90 = new System.Windows.Forms.MenuItem();
         this.mnuCounter90Origin = new System.Windows.Forms.MenuItem();
         this.mnuCounter90Center = new System.Windows.Forms.MenuItem();
         this.mnuReset = new System.Windows.Forms.MenuItem();
         this.mnuEditGraphicCell = new System.Windows.Forms.MenuItem();
         this.FrameSplitter = new System.Windows.Forms.Splitter();
         this.pnlFrameAction = new System.Windows.Forms.Panel();
         this.txtFramesetName = new System.Windows.Forms.TextBox();
         this.lblFramesetName = new System.Windows.Forms.Label();
         this.FrameProperties = new System.Windows.Forms.PropertyGrid();
         this.pnlTransform = new System.Windows.Forms.Panel();
         this.lblDegrees = new System.Windows.Forms.Label();
         this.btnCancel = new System.Windows.Forms.Button();
         this.nudYScale = new System.Windows.Forms.NumericUpDown();
         this.nudXScale = new System.Windows.Forms.NumericUpDown();
         this.nudXOffset = new System.Windows.Forms.NumericUpDown();
         this.nudYOffset = new System.Windows.Forms.NumericUpDown();
         this.lblXOffset = new System.Windows.Forms.Label();
         this.btnOK = new System.Windows.Forms.Button();
         this.btnReset = new System.Windows.Forms.Button();
         this.lblYOffset = new System.Windows.Forms.Label();
         this.lblYScale = new System.Windows.Forms.Label();
         this.lblXScale = new System.Windows.Forms.Label();
         this.chkRotateAroundCenter = new System.Windows.Forms.CheckBox();
         this.lblRotate = new System.Windows.Forms.Label();
         this.txtRotate = new System.Windows.Forms.TextBox();
         this.trbRotate = new System.Windows.Forms.TrackBar();
         this.pnlGraphicSheet = new System.Windows.Forms.Panel();
         this.CellBrowser = new SGDK2.GraphicBrowser();
         this.tbrGraphicSheet = new System.Windows.Forms.ToolBar();
         this.tbbAddToFrameset = new System.Windows.Forms.ToolBarButton();
         this.imlFrameset = new System.Windows.Forms.ImageList(this.components);
         this.pnlGraphicSheetName = new System.Windows.Forms.Panel();
         this.lblGraphicSheet = new System.Windows.Forms.Label();
         this.cboGraphicSheet = new System.Windows.Forms.ComboBox();
         this.ttFrameset = new System.Windows.Forms.ToolTip(this.components);
         this.dataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.mnuFrameset = new System.Windows.Forms.MainMenu();
         this.mnuFramesetPop = new System.Windows.Forms.MenuItem();
         this.mnuAddCell = new System.Windows.Forms.MenuItem();
         this.mnuDeleteFrames = new System.Windows.Forms.MenuItem();
         this.mnuFramesetTransform = new System.Windows.Forms.MenuItem();
         this.mnuXHFlip = new System.Windows.Forms.MenuItem();
         this.mnuXHFlipOrigin = new System.Windows.Forms.MenuItem();
         this.mnuXHFlipCenter = new System.Windows.Forms.MenuItem();
         this.mnuXVFlip = new System.Windows.Forms.MenuItem();
         this.mnuXVFlipOrigin = new System.Windows.Forms.MenuItem();
         this.mnuXVFlipCenter = new System.Windows.Forms.MenuItem();
         this.mnuXDoubleWidth = new System.Windows.Forms.MenuItem();
         this.mnuXDoubleHeight = new System.Windows.Forms.MenuItem();
         this.mnuXHalveWidth = new System.Windows.Forms.MenuItem();
         this.mnuXHalveHeight = new System.Windows.Forms.MenuItem();
         this.mnuXLeftHalf = new System.Windows.Forms.MenuItem();
         this.mnuXRightHalf = new System.Windows.Forms.MenuItem();
         this.mnuXUpHalf = new System.Windows.Forms.MenuItem();
         this.mnuXDownHalf = new System.Windows.Forms.MenuItem();
         this.mnuXClockwise90 = new System.Windows.Forms.MenuItem();
         this.mnuXClockwise90Origin = new System.Windows.Forms.MenuItem();
         this.mnuXClockwise90Center = new System.Windows.Forms.MenuItem();
         this.mnuXCounter90 = new System.Windows.Forms.MenuItem();
         this.mnuXCounter90Origin = new System.Windows.Forms.MenuItem();
         this.mnuXCounter90Center = new System.Windows.Forms.MenuItem();
         this.mnuTransformCustom = new System.Windows.Forms.MenuItem();
         this.mnuXReset = new System.Windows.Forms.MenuItem();
         this.mnuFramesetSeparator = new System.Windows.Forms.MenuItem();
         this.mnuFrameRemappingWizard = new System.Windows.Forms.MenuItem();
         this.mnuFsEditGraphicCell = new System.Windows.Forms.MenuItem();
         this.tabFrameset = new System.Windows.Forms.TabControl();
         this.tpgFrameset = new System.Windows.Forms.TabPage();
         this.splitterGraphics = new System.Windows.Forms.Splitter();
         this.tpgFrameEditor = new System.Windows.Forms.TabPage();
         this.pnlFrames.SuspendLayout();
         this.pnlFrameAction.SuspendLayout();
         this.pnlTransform.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.nudYScale)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudXScale)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudXOffset)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudYOffset)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.trbRotate)).BeginInit();
         this.pnlGraphicSheet.SuspendLayout();
         this.pnlGraphicSheetName.SuspendLayout();
         this.tabFrameset.SuspendLayout();
         this.tpgFrameset.SuspendLayout();
         this.tpgFrameEditor.SuspendLayout();
         this.SuspendLayout();
         // 
         // pnlFrames
         // 
         this.pnlFrames.Controls.Add(this.FrameBrowser);
         this.pnlFrames.Controls.Add(this.FrameSplitter);
         this.pnlFrames.Controls.Add(this.pnlFrameAction);
         this.pnlFrames.Controls.Add(this.FrameProperties);
         this.pnlFrames.Dock = System.Windows.Forms.DockStyle.Fill;
         this.pnlFrames.Location = new System.Drawing.Point(221, 0);
         this.pnlFrames.Name = "pnlFrames";
         this.pnlFrames.Size = new System.Drawing.Size(235, 447);
         this.pnlFrames.TabIndex = 25;
         // 
         // FrameBrowser
         // 
         this.FrameBrowser.AllowDrop = true;
         this.FrameBrowser.AutoScroll = true;
         this.FrameBrowser.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.FrameBrowser.CellPadding = new System.Drawing.Size(3, 3);
         this.FrameBrowser.CellSize = new System.Drawing.Size(0, 0);
         this.FrameBrowser.ContextMenu = this.mnuContext;
         this.FrameBrowser.CurrentCellIndex = -1;
         this.FrameBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
         this.FrameBrowser.Frameset = null;
         this.FrameBrowser.FramesToDisplay = null;
         this.FrameBrowser.GraphicSheet = null;
         this.FrameBrowser.Location = new System.Drawing.Point(0, 24);
         this.FrameBrowser.Name = "FrameBrowser";
         this.FrameBrowser.SheetImage = null;
         this.FrameBrowser.Size = new System.Drawing.Size(235, 218);
         this.FrameBrowser.TabIndex = 6;
         this.FrameBrowser.Resize += new System.EventHandler(this.FrameBrowser_Resize);
         this.FrameBrowser.CurrentCellChanged += new System.EventHandler(this.FrameBrowser_CurrentCellChanged);
         this.FrameBrowser.DragDrop += new System.Windows.Forms.DragEventHandler(this.FrameBrowser_DragDrop);
         this.FrameBrowser.DoubleClick += new System.EventHandler(this.FrameBrowser_DoubleClick);
         this.FrameBrowser.DragOver += new System.Windows.Forms.DragEventHandler(this.FrameBrowser_DragOver);
         // 
         // mnuContext
         // 
         this.mnuContext.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                   this.mnuHFlip,
                                                                                   this.mnuVFlip,
                                                                                   this.mnuDoubleWidth,
                                                                                   this.mnuDoubleHeight,
                                                                                   this.mnuHalveWidth,
                                                                                   this.mnuHalveHeight,
                                                                                   this.mnuLeftHalf,
                                                                                   this.mnuRightHalf,
                                                                                   this.mnuUpHalf,
                                                                                   this.mnuDownHalf,
                                                                                   this.mnuClockwise90,
                                                                                   this.mnuCounter90,
                                                                                   this.mnuReset,
                                                                                   this.mnuEditGraphicCell});
         // 
         // mnuHFlip
         // 
         this.mnuHFlip.Index = 0;
         this.mnuHFlip.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                 this.mnuHFlipOrigin,
                                                                                 this.mnuHFlipCenter});
         this.mnuHFlip.Text = "Horizontal Flip";
         // 
         // mnuHFlipOrigin
         // 
         this.mnuHFlipOrigin.Index = 0;
         this.mnuHFlipOrigin.Text = "Across origin";
         this.mnuHFlipOrigin.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuHFlipCenter
         // 
         this.mnuHFlipCenter.Index = 1;
         this.mnuHFlipCenter.Text = "Across center";
         this.mnuHFlipCenter.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuVFlip
         // 
         this.mnuVFlip.Index = 1;
         this.mnuVFlip.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                 this.mnuVFlipOrigin,
                                                                                 this.mnuVFlipCenter});
         this.mnuVFlip.Text = "Vertical Flip";
         // 
         // mnuVFlipOrigin
         // 
         this.mnuVFlipOrigin.Index = 0;
         this.mnuVFlipOrigin.Text = "Across origin";
         this.mnuVFlipOrigin.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuVFlipCenter
         // 
         this.mnuVFlipCenter.Index = 1;
         this.mnuVFlipCenter.Text = "Across center";
         this.mnuVFlipCenter.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuDoubleWidth
         // 
         this.mnuDoubleWidth.Index = 2;
         this.mnuDoubleWidth.Text = "Double Width";
         this.mnuDoubleWidth.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuDoubleHeight
         // 
         this.mnuDoubleHeight.Index = 3;
         this.mnuDoubleHeight.Text = "Double Height";
         this.mnuDoubleHeight.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuHalveWidth
         // 
         this.mnuHalveWidth.Index = 4;
         this.mnuHalveWidth.Text = "Halve Width";
         this.mnuHalveWidth.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuHalveHeight
         // 
         this.mnuHalveHeight.Index = 5;
         this.mnuHalveHeight.Text = "Halve Height";
         this.mnuHalveHeight.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuLeftHalf
         // 
         this.mnuLeftHalf.Index = 6;
         this.mnuLeftHalf.Text = "Move left 1/2 width";
         this.mnuLeftHalf.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuRightHalf
         // 
         this.mnuRightHalf.Index = 7;
         this.mnuRightHalf.Text = "Move right 1/2 width";
         this.mnuRightHalf.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuUpHalf
         // 
         this.mnuUpHalf.Index = 8;
         this.mnuUpHalf.Text = "Move up 1/2 height";
         this.mnuUpHalf.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuDownHalf
         // 
         this.mnuDownHalf.Index = 9;
         this.mnuDownHalf.Text = "Move down 1/2 height";
         this.mnuDownHalf.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuClockwise90
         // 
         this.mnuClockwise90.Index = 10;
         this.mnuClockwise90.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                       this.mnuClockwise90Origin,
                                                                                       this.mnuClockwise90Center});
         this.mnuClockwise90.Text = "Clockwise 90 degrees";
         // 
         // mnuClockwise90Origin
         // 
         this.mnuClockwise90Origin.Index = 0;
         this.mnuClockwise90Origin.Text = "Around origin";
         this.mnuClockwise90Origin.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuClockwise90Center
         // 
         this.mnuClockwise90Center.Index = 1;
         this.mnuClockwise90Center.Text = "Around center";
         this.mnuClockwise90Center.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuCounter90
         // 
         this.mnuCounter90.Index = 11;
         this.mnuCounter90.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                     this.mnuCounter90Origin,
                                                                                     this.mnuCounter90Center});
         this.mnuCounter90.Text = "Counter-clockwise 90 degrees";
         // 
         // mnuCounter90Origin
         // 
         this.mnuCounter90Origin.Index = 0;
         this.mnuCounter90Origin.Text = "Around origin";
         this.mnuCounter90Origin.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuCounter90Center
         // 
         this.mnuCounter90Center.Index = 1;
         this.mnuCounter90Center.Text = "Around center";
         this.mnuCounter90Center.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuReset
         // 
         this.mnuReset.Index = 12;
         this.mnuReset.Text = "Reset";
         this.mnuReset.Click += new System.EventHandler(this.mnuContextItem_Click);
         // 
         // mnuEditGraphicCell
         // 
         this.mnuEditGraphicCell.Index = 13;
         this.mnuEditGraphicCell.Text = "Edit &Graphic Cell...";
         this.mnuEditGraphicCell.Click += new System.EventHandler(this.mnuGraphicEditor_Click);
         // 
         // FrameSplitter
         // 
         this.FrameSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.FrameSplitter.Location = new System.Drawing.Point(0, 242);
         this.FrameSplitter.Name = "FrameSplitter";
         this.FrameSplitter.Size = new System.Drawing.Size(235, 5);
         this.FrameSplitter.TabIndex = 8;
         this.FrameSplitter.TabStop = false;
         // 
         // pnlFrameAction
         // 
         this.pnlFrameAction.Controls.Add(this.txtFramesetName);
         this.pnlFrameAction.Controls.Add(this.lblFramesetName);
         this.pnlFrameAction.Dock = System.Windows.Forms.DockStyle.Top;
         this.pnlFrameAction.Location = new System.Drawing.Point(0, 0);
         this.pnlFrameAction.Name = "pnlFrameAction";
         this.pnlFrameAction.Size = new System.Drawing.Size(235, 24);
         this.pnlFrameAction.TabIndex = 1;
         // 
         // txtFramesetName
         // 
         this.txtFramesetName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtFramesetName.Location = new System.Drawing.Point(120, 0);
         this.txtFramesetName.Name = "txtFramesetName";
         this.txtFramesetName.Size = new System.Drawing.Size(107, 20);
         this.txtFramesetName.TabIndex = 1;
         this.txtFramesetName.Text = "";
         this.txtFramesetName.Validating += new System.ComponentModel.CancelEventHandler(this.txtFramesetName_Validating);
         this.txtFramesetName.Validated += new System.EventHandler(this.txtFramesetName_Validated);
         // 
         // lblFramesetName
         // 
         this.lblFramesetName.Location = new System.Drawing.Point(8, 0);
         this.lblFramesetName.Name = "lblFramesetName";
         this.lblFramesetName.Size = new System.Drawing.Size(112, 20);
         this.lblFramesetName.TabIndex = 0;
         this.lblFramesetName.Text = "Frameset Name:";
         this.lblFramesetName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // FrameProperties
         // 
         this.FrameProperties.CommandsVisibleIfAvailable = true;
         this.FrameProperties.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.FrameProperties.LargeButtons = false;
         this.FrameProperties.LineColor = System.Drawing.SystemColors.ScrollBar;
         this.FrameProperties.Location = new System.Drawing.Point(0, 247);
         this.FrameProperties.Name = "FrameProperties";
         this.FrameProperties.Size = new System.Drawing.Size(235, 200);
         this.FrameProperties.TabIndex = 0;
         this.FrameProperties.Text = "PropertyGrid";
         this.FrameProperties.ToolbarVisible = false;
         this.FrameProperties.ViewBackColor = System.Drawing.SystemColors.Window;
         this.FrameProperties.ViewForeColor = System.Drawing.SystemColors.WindowText;
         this.FrameProperties.Enter += new System.EventHandler(this.FrameProperties_Enter);
         this.FrameProperties.Leave += new System.EventHandler(this.FrameProperties_Leave);
         // 
         // pnlTransform
         // 
         this.pnlTransform.AllowDrop = true;
         this.pnlTransform.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.pnlTransform.ContextMenu = this.mnuContext;
         this.pnlTransform.Controls.Add(this.lblDegrees);
         this.pnlTransform.Controls.Add(this.btnCancel);
         this.pnlTransform.Controls.Add(this.nudYScale);
         this.pnlTransform.Controls.Add(this.nudXScale);
         this.pnlTransform.Controls.Add(this.nudXOffset);
         this.pnlTransform.Controls.Add(this.nudYOffset);
         this.pnlTransform.Controls.Add(this.lblXOffset);
         this.pnlTransform.Controls.Add(this.btnOK);
         this.pnlTransform.Controls.Add(this.btnReset);
         this.pnlTransform.Controls.Add(this.lblYOffset);
         this.pnlTransform.Controls.Add(this.lblYScale);
         this.pnlTransform.Controls.Add(this.lblXScale);
         this.pnlTransform.Controls.Add(this.chkRotateAroundCenter);
         this.pnlTransform.Controls.Add(this.lblRotate);
         this.pnlTransform.Controls.Add(this.txtRotate);
         this.pnlTransform.Controls.Add(this.trbRotate);
         this.pnlTransform.Dock = System.Windows.Forms.DockStyle.Fill;
         this.pnlTransform.Location = new System.Drawing.Point(0, 0);
         this.pnlTransform.Name = "pnlTransform";
         this.pnlTransform.Size = new System.Drawing.Size(456, 447);
         this.pnlTransform.TabIndex = 0;
         this.pnlTransform.Resize += new System.EventHandler(this.pnlTransform_Resize);
         this.pnlTransform.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlTransform_MouseUp);
         this.pnlTransform.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlTransform_Paint);
         this.pnlTransform.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlTransform_MouseMove);
         this.pnlTransform.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlTransform_MouseDown);
         // 
         // lblDegrees
         // 
         this.lblDegrees.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblDegrees.Location = new System.Drawing.Point(128, 368);
         this.lblDegrees.Name = "lblDegrees";
         this.lblDegrees.Size = new System.Drawing.Size(56, 20);
         this.lblDegrees.TabIndex = 15;
         this.lblDegrees.Text = " degrees";
         this.lblDegrees.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // btnCancel
         // 
         this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(368, 40);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(72, 24);
         this.btnCancel.TabIndex = 13;
         this.btnCancel.Text = "Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // nudYScale
         // 
         this.nudYScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.nudYScale.DecimalPlaces = 2;
         this.nudYScale.Increment = new System.Decimal(new int[] {
                                                                    1,
                                                                    0,
                                                                    0,
                                                                    65536});
         this.nudYScale.Location = new System.Drawing.Point(232, 392);
         this.nudYScale.Maximum = new System.Decimal(new int[] {
                                                                  128,
                                                                  0,
                                                                  0,
                                                                  0});
         this.nudYScale.Minimum = new System.Decimal(new int[] {
                                                                  128,
                                                                  0,
                                                                  0,
                                                                  -2147483648});
         this.nudYScale.Name = "nudYScale";
         this.nudYScale.Size = new System.Drawing.Size(64, 20);
         this.nudYScale.TabIndex = 7;
         this.nudYScale.Value = new System.Decimal(new int[] {
                                                                1,
                                                                0,
                                                                0,
                                                                0});
         this.nudYScale.Validated += new System.EventHandler(this.TransformControl_Validated);
         this.nudYScale.ValueChanged += new System.EventHandler(this.TransformControl_ValueChanged);
         // 
         // nudXScale
         // 
         this.nudXScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.nudXScale.DecimalPlaces = 2;
         this.nudXScale.Increment = new System.Decimal(new int[] {
                                                                    1,
                                                                    0,
                                                                    0,
                                                                    65536});
         this.nudXScale.Location = new System.Drawing.Point(64, 392);
         this.nudXScale.Maximum = new System.Decimal(new int[] {
                                                                  128,
                                                                  0,
                                                                  0,
                                                                  0});
         this.nudXScale.Minimum = new System.Decimal(new int[] {
                                                                  128,
                                                                  0,
                                                                  0,
                                                                  -2147483648});
         this.nudXScale.Name = "nudXScale";
         this.nudXScale.Size = new System.Drawing.Size(64, 20);
         this.nudXScale.TabIndex = 5;
         this.nudXScale.Value = new System.Decimal(new int[] {
                                                                1,
                                                                0,
                                                                0,
                                                                0});
         this.nudXScale.Validated += new System.EventHandler(this.TransformControl_Validated);
         this.nudXScale.ValueChanged += new System.EventHandler(this.TransformControl_ValueChanged);
         // 
         // nudXOffset
         // 
         this.nudXOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.nudXOffset.DecimalPlaces = 1;
         this.nudXOffset.Location = new System.Drawing.Point(64, 416);
         this.nudXOffset.Maximum = new System.Decimal(new int[] {
                                                                   128,
                                                                   0,
                                                                   0,
                                                                   0});
         this.nudXOffset.Minimum = new System.Decimal(new int[] {
                                                                   128,
                                                                   0,
                                                                   0,
                                                                   -2147483648});
         this.nudXOffset.Name = "nudXOffset";
         this.nudXOffset.Size = new System.Drawing.Size(64, 20);
         this.nudXOffset.TabIndex = 9;
         this.nudXOffset.Validated += new System.EventHandler(this.TransformControl_Validated);
         this.nudXOffset.ValueChanged += new System.EventHandler(this.TransformControl_ValueChanged);
         // 
         // nudYOffset
         // 
         this.nudYOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.nudYOffset.DecimalPlaces = 1;
         this.nudYOffset.Location = new System.Drawing.Point(232, 416);
         this.nudYOffset.Maximum = new System.Decimal(new int[] {
                                                                   128,
                                                                   0,
                                                                   0,
                                                                   0});
         this.nudYOffset.Minimum = new System.Decimal(new int[] {
                                                                   128,
                                                                   0,
                                                                   0,
                                                                   -2147483648});
         this.nudYOffset.Name = "nudYOffset";
         this.nudYOffset.Size = new System.Drawing.Size(64, 20);
         this.nudYOffset.TabIndex = 11;
         this.nudYOffset.Validated += new System.EventHandler(this.TransformControl_Validated);
         this.nudYOffset.ValueChanged += new System.EventHandler(this.TransformControl_ValueChanged);
         // 
         // lblXOffset
         // 
         this.lblXOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblXOffset.Location = new System.Drawing.Point(8, 416);
         this.lblXOffset.Name = "lblXOffset";
         this.lblXOffset.Size = new System.Drawing.Size(56, 20);
         this.lblXOffset.TabIndex = 8;
         this.lblXOffset.Text = "X Offset:";
         this.lblXOffset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // btnOK
         // 
         this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnOK.Location = new System.Drawing.Point(368, 8);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(72, 24);
         this.btnOK.TabIndex = 12;
         this.btnOK.Text = "OK";
         this.ttFrameset.SetToolTip(this.btnOK, "Close the frame editor pane and apply these changes to all selected frames.");
         this.btnOK.Click += new System.EventHandler(this.btnApply_Click);
         // 
         // btnReset
         // 
         this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnReset.Location = new System.Drawing.Point(368, 72);
         this.btnReset.Name = "btnReset";
         this.btnReset.Size = new System.Drawing.Size(72, 24);
         this.btnReset.TabIndex = 14;
         this.btnReset.Text = "Reset";
         this.ttFrameset.SetToolTip(this.btnReset, "Reset the current frame to initial state (untransformed graphic cell)");
         this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
         // 
         // lblYOffset
         // 
         this.lblYOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblYOffset.Location = new System.Drawing.Point(176, 416);
         this.lblYOffset.Name = "lblYOffset";
         this.lblYOffset.Size = new System.Drawing.Size(56, 20);
         this.lblYOffset.TabIndex = 10;
         this.lblYOffset.Text = "Y Offset:";
         this.lblYOffset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblYScale
         // 
         this.lblYScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblYScale.Location = new System.Drawing.Point(176, 392);
         this.lblYScale.Name = "lblYScale";
         this.lblYScale.Size = new System.Drawing.Size(56, 20);
         this.lblYScale.TabIndex = 6;
         this.lblYScale.Text = "Y Scale:";
         this.lblYScale.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblXScale
         // 
         this.lblXScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblXScale.Location = new System.Drawing.Point(8, 392);
         this.lblXScale.Name = "lblXScale";
         this.lblXScale.Size = new System.Drawing.Size(56, 20);
         this.lblXScale.TabIndex = 4;
         this.lblXScale.Text = "X Scale:";
         this.lblXScale.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // chkRotateAroundCenter
         // 
         this.chkRotateAroundCenter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.chkRotateAroundCenter.Checked = true;
         this.chkRotateAroundCenter.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkRotateAroundCenter.Location = new System.Drawing.Point(184, 368);
         this.chkRotateAroundCenter.Name = "chkRotateAroundCenter";
         this.chkRotateAroundCenter.Size = new System.Drawing.Size(104, 20);
         this.chkRotateAroundCenter.TabIndex = 3;
         this.chkRotateAroundCenter.Text = "Around center";
         this.chkRotateAroundCenter.CheckedChanged += new System.EventHandler(this.TransformControl_ValueChanged);
         // 
         // lblRotate
         // 
         this.lblRotate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblRotate.Location = new System.Drawing.Point(8, 331);
         this.lblRotate.Name = "lblRotate";
         this.lblRotate.Size = new System.Drawing.Size(56, 20);
         this.lblRotate.TabIndex = 1;
         this.lblRotate.Text = "Rotate:";
         this.lblRotate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtRotate
         // 
         this.txtRotate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.txtRotate.Location = new System.Drawing.Point(64, 368);
         this.txtRotate.Name = "txtRotate";
         this.txtRotate.Size = new System.Drawing.Size(64, 20);
         this.txtRotate.TabIndex = 2;
         this.txtRotate.Text = "";
         this.txtRotate.TextChanged += new System.EventHandler(this.txtRotate_TextChanged);
         // 
         // trbRotate
         // 
         this.trbRotate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.trbRotate.LargeChange = 15;
         this.trbRotate.Location = new System.Drawing.Point(64, 323);
         this.trbRotate.Maximum = 180;
         this.trbRotate.Minimum = -179;
         this.trbRotate.Name = "trbRotate";
         this.trbRotate.Size = new System.Drawing.Size(290, 34);
         this.trbRotate.TabIndex = 0;
         this.trbRotate.TickFrequency = 15;
         this.trbRotate.Scroll += new System.EventHandler(this.trbRotate_Scroll);
         // 
         // pnlGraphicSheet
         // 
         this.pnlGraphicSheet.Controls.Add(this.CellBrowser);
         this.pnlGraphicSheet.Controls.Add(this.tbrGraphicSheet);
         this.pnlGraphicSheet.Controls.Add(this.pnlGraphicSheetName);
         this.pnlGraphicSheet.Dock = System.Windows.Forms.DockStyle.Left;
         this.pnlGraphicSheet.Location = new System.Drawing.Point(0, 0);
         this.pnlGraphicSheet.Name = "pnlGraphicSheet";
         this.pnlGraphicSheet.Size = new System.Drawing.Size(216, 447);
         this.pnlGraphicSheet.TabIndex = 2;
         // 
         // CellBrowser
         // 
         this.CellBrowser.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.CellBrowser.CellPadding = new System.Drawing.Size(3, 3);
         this.CellBrowser.CellSize = new System.Drawing.Size(0, 0);
         this.CellBrowser.CurrentCellIndex = -1;
         this.CellBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
         this.CellBrowser.Frameset = null;
         this.CellBrowser.FramesToDisplay = null;
         this.CellBrowser.GraphicSheet = null;
         this.CellBrowser.Location = new System.Drawing.Point(0, 51);
         this.CellBrowser.Name = "CellBrowser";
         this.CellBrowser.SheetImage = null;
         this.CellBrowser.Size = new System.Drawing.Size(216, 396);
         this.CellBrowser.TabIndex = 2;
         this.CellBrowser.CurrentCellChanged += new System.EventHandler(this.CellBrowser_CurrentCellChanged);
         this.CellBrowser.DoubleClick += new System.EventHandler(this.CellBrowser_DoubleClick);
         // 
         // tbrGraphicSheet
         // 
         this.tbrGraphicSheet.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                                           this.tbbAddToFrameset});
         this.tbrGraphicSheet.ButtonSize = new System.Drawing.Size(22, 21);
         this.tbrGraphicSheet.DropDownArrows = true;
         this.tbrGraphicSheet.ImageList = this.imlFrameset;
         this.tbrGraphicSheet.Location = new System.Drawing.Point(0, 24);
         this.tbrGraphicSheet.Name = "tbrGraphicSheet";
         this.tbrGraphicSheet.ShowToolTips = true;
         this.tbrGraphicSheet.Size = new System.Drawing.Size(216, 27);
         this.tbrGraphicSheet.TabIndex = 5;
         this.tbrGraphicSheet.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbrGraphicSheet_ButtonClick);
         // 
         // tbbAddToFrameset
         // 
         this.tbbAddToFrameset.ImageIndex = 0;
         this.tbbAddToFrameset.Text = "Add to Frameset";
         this.tbbAddToFrameset.ToolTipText = "Add selected cells to frameset";
         // 
         // imlFrameset
         // 
         this.imlFrameset.ImageSize = new System.Drawing.Size(15, 15);
         this.imlFrameset.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlFrameset.ImageStream")));
         this.imlFrameset.TransparentColor = System.Drawing.Color.Magenta;
         // 
         // pnlGraphicSheetName
         // 
         this.pnlGraphicSheetName.Controls.Add(this.lblGraphicSheet);
         this.pnlGraphicSheetName.Controls.Add(this.cboGraphicSheet);
         this.pnlGraphicSheetName.Dock = System.Windows.Forms.DockStyle.Top;
         this.pnlGraphicSheetName.Location = new System.Drawing.Point(0, 0);
         this.pnlGraphicSheetName.Name = "pnlGraphicSheetName";
         this.pnlGraphicSheetName.Size = new System.Drawing.Size(216, 24);
         this.pnlGraphicSheetName.TabIndex = 4;
         // 
         // lblGraphicSheet
         // 
         this.lblGraphicSheet.Location = new System.Drawing.Point(8, 0);
         this.lblGraphicSheet.Name = "lblGraphicSheet";
         this.lblGraphicSheet.Size = new System.Drawing.Size(88, 21);
         this.lblGraphicSheet.TabIndex = 3;
         this.lblGraphicSheet.Text = "Graphic Sheet:";
         this.lblGraphicSheet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // cboGraphicSheet
         // 
         this.cboGraphicSheet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cboGraphicSheet.DisplayMember = "Name";
         this.cboGraphicSheet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboGraphicSheet.Location = new System.Drawing.Point(96, 0);
         this.cboGraphicSheet.Name = "cboGraphicSheet";
         this.cboGraphicSheet.Size = new System.Drawing.Size(112, 21);
         this.cboGraphicSheet.TabIndex = 0;
         this.ttFrameset.SetToolTip(this.cboGraphicSheet, "Select a graphic sheet to use as a source for frame graphics");
         this.cboGraphicSheet.ValueMember = "Name";
         this.cboGraphicSheet.SelectedIndexChanged += new System.EventHandler(this.cboGraphicSheet_SelectedIndexChanged);
         // 
         // dataMonitor
         // 
         this.dataMonitor.GraphicSheetRowChanged += new SGDK2.ProjectDataset.GraphicSheetRowChangeEventHandler(this.GraphicSheet_GraphicSheetRowChange);
         this.dataMonitor.GraphicSheetRowDeleted += new SGDK2.ProjectDataset.GraphicSheetRowChangeEventHandler(this.GraphicSheet_GraphicSheetRowChange);
         this.dataMonitor.FramesetRowDeleted += new SGDK2.ProjectDataset.FramesetRowChangeEventHandler(this.Frameset_FramesetRowDeleted);
         this.dataMonitor.Clearing += new System.EventHandler(this.ProjectData_Clearing);
         // 
         // mnuFrameset
         // 
         this.mnuFrameset.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                    this.mnuFramesetPop});
         // 
         // mnuFramesetPop
         // 
         this.mnuFramesetPop.Index = 0;
         this.mnuFramesetPop.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                       this.mnuAddCell,
                                                                                       this.mnuDeleteFrames,
                                                                                       this.mnuFramesetTransform,
                                                                                       this.mnuFramesetSeparator,
                                                                                       this.mnuFrameRemappingWizard,
                                                                                       this.mnuFsEditGraphicCell});
         this.mnuFramesetPop.MergeOrder = 2;
         this.mnuFramesetPop.Text = "F&rameset";
         // 
         // mnuAddCell
         // 
         this.mnuAddCell.Index = 0;
         this.mnuAddCell.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
         this.mnuAddCell.Text = "&Add Selected Cells to Frameset";
         this.mnuAddCell.Click += new System.EventHandler(this.mnuAddCell_Click);
         // 
         // mnuDeleteFrames
         // 
         this.mnuDeleteFrames.Index = 1;
         this.mnuDeleteFrames.Shortcut = System.Windows.Forms.Shortcut.Del;
         this.mnuDeleteFrames.Text = "&Delete Selected Frames";
         this.mnuDeleteFrames.Click += new System.EventHandler(this.mnuDeleteFrames_Click);
         // 
         // mnuFramesetTransform
         // 
         this.mnuFramesetTransform.Index = 2;
         this.mnuFramesetTransform.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                             this.mnuXHFlip,
                                                                                             this.mnuXVFlip,
                                                                                             this.mnuXDoubleWidth,
                                                                                             this.mnuXDoubleHeight,
                                                                                             this.mnuXHalveWidth,
                                                                                             this.mnuXHalveHeight,
                                                                                             this.mnuXLeftHalf,
                                                                                             this.mnuXRightHalf,
                                                                                             this.mnuXUpHalf,
                                                                                             this.mnuXDownHalf,
                                                                                             this.mnuXClockwise90,
                                                                                             this.mnuXCounter90,
                                                                                             this.mnuTransformCustom,
                                                                                             this.mnuXReset});
         this.mnuFramesetTransform.Text = "&Transform Selected Frames";
         // 
         // mnuXHFlip
         // 
         this.mnuXHFlip.Index = 0;
         this.mnuXHFlip.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                  this.mnuXHFlipOrigin,
                                                                                  this.mnuXHFlipCenter});
         this.mnuXHFlip.Text = "Horizontal Flip";
         // 
         // mnuXHFlipOrigin
         // 
         this.mnuXHFlipOrigin.Index = 0;
         this.mnuXHFlipOrigin.Text = "Across origin";
         this.mnuXHFlipOrigin.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuXHFlipCenter
         // 
         this.mnuXHFlipCenter.Index = 1;
         this.mnuXHFlipCenter.Text = "Across center";
         this.mnuXHFlipCenter.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuXVFlip
         // 
         this.mnuXVFlip.Index = 1;
         this.mnuXVFlip.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                  this.mnuXVFlipOrigin,
                                                                                  this.mnuXVFlipCenter});
         this.mnuXVFlip.Text = "Vertical Flip";
         // 
         // mnuXVFlipOrigin
         // 
         this.mnuXVFlipOrigin.Index = 0;
         this.mnuXVFlipOrigin.Text = "Across origin";
         this.mnuXVFlipOrigin.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuXVFlipCenter
         // 
         this.mnuXVFlipCenter.Index = 1;
         this.mnuXVFlipCenter.Text = "Across center";
         this.mnuXVFlipCenter.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuXDoubleWidth
         // 
         this.mnuXDoubleWidth.Index = 2;
         this.mnuXDoubleWidth.Text = "Double Width";
         this.mnuXDoubleWidth.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuXDoubleHeight
         // 
         this.mnuXDoubleHeight.Index = 3;
         this.mnuXDoubleHeight.Text = "Double Height";
         this.mnuXDoubleHeight.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuXHalveWidth
         // 
         this.mnuXHalveWidth.Index = 4;
         this.mnuXHalveWidth.Text = "Halve Width";
         this.mnuXHalveWidth.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuXHalveHeight
         // 
         this.mnuXHalveHeight.Index = 5;
         this.mnuXHalveHeight.Text = "Halve Height";
         this.mnuXHalveHeight.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuXLeftHalf
         // 
         this.mnuXLeftHalf.Index = 6;
         this.mnuXLeftHalf.Text = "Move left 1/2 width";
         this.mnuXLeftHalf.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuXRightHalf
         // 
         this.mnuXRightHalf.Index = 7;
         this.mnuXRightHalf.Text = "Move right 1/2 width";
         this.mnuXRightHalf.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuXUpHalf
         // 
         this.mnuXUpHalf.Index = 8;
         this.mnuXUpHalf.Text = "Move up 1/2 height";
         this.mnuXUpHalf.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuXDownHalf
         // 
         this.mnuXDownHalf.Index = 9;
         this.mnuXDownHalf.Text = "Move down 1/2 height";
         this.mnuXDownHalf.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuXClockwise90
         // 
         this.mnuXClockwise90.Index = 10;
         this.mnuXClockwise90.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                        this.mnuXClockwise90Origin,
                                                                                        this.mnuXClockwise90Center});
         this.mnuXClockwise90.Text = "Clockwise 90 degrees";
         // 
         // mnuXClockwise90Origin
         // 
         this.mnuXClockwise90Origin.Index = 0;
         this.mnuXClockwise90Origin.Text = "Around origin";
         this.mnuXClockwise90Origin.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuXClockwise90Center
         // 
         this.mnuXClockwise90Center.Index = 1;
         this.mnuXClockwise90Center.Text = "Around center";
         this.mnuXClockwise90Center.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuXCounter90
         // 
         this.mnuXCounter90.Index = 11;
         this.mnuXCounter90.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.mnuXCounter90Origin,
                                                                                      this.mnuXCounter90Center});
         this.mnuXCounter90.Text = "Counter-clockwise 90 degrees";
         // 
         // mnuXCounter90Origin
         // 
         this.mnuXCounter90Origin.Index = 0;
         this.mnuXCounter90Origin.Text = "Around origin";
         this.mnuXCounter90Origin.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuXCounter90Center
         // 
         this.mnuXCounter90Center.Index = 1;
         this.mnuXCounter90Center.Text = "Around center";
         this.mnuXCounter90Center.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuTransformCustom
         // 
         this.mnuTransformCustom.Index = 12;
         this.mnuTransformCustom.Shortcut = System.Windows.Forms.Shortcut.CtrlT;
         this.mnuTransformCustom.Text = "Custom...";
         this.mnuTransformCustom.Click += new System.EventHandler(this.mnuTransformCustom_Click);
         // 
         // mnuXReset
         // 
         this.mnuXReset.Index = 13;
         this.mnuXReset.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
         this.mnuXReset.Text = "Reset";
         this.mnuXReset.Click += new System.EventHandler(this.mnuTransformItem_Click);
         // 
         // mnuFramesetSeparator
         // 
         this.mnuFramesetSeparator.Index = 3;
         this.mnuFramesetSeparator.Text = "-";
         // 
         // mnuFrameRemappingWizard
         // 
         this.mnuFrameRemappingWizard.Index = 4;
         this.mnuFrameRemappingWizard.Text = "&Frame Remapping Wizard...";
         this.mnuFrameRemappingWizard.Click += new System.EventHandler(this.mnuFrameRemappingWizard_Click);
         // 
         // mnuFsEditGraphicCell
         // 
         this.mnuFsEditGraphicCell.Index = 5;
         this.mnuFsEditGraphicCell.Text = "Edit &Graphic Cell...";
         this.mnuFsEditGraphicCell.Click += new System.EventHandler(this.mnuGraphicEditor_Click);
         // 
         // tabFrameset
         // 
         this.tabFrameset.Controls.Add(this.tpgFrameset);
         this.tabFrameset.Controls.Add(this.tpgFrameEditor);
         this.tabFrameset.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tabFrameset.Location = new System.Drawing.Point(0, 0);
         this.tabFrameset.Name = "tabFrameset";
         this.tabFrameset.SelectedIndex = 0;
         this.tabFrameset.Size = new System.Drawing.Size(464, 473);
         this.tabFrameset.TabIndex = 15;
         this.tabFrameset.SelectedIndexChanged += new System.EventHandler(this.tabFrameset_SelectedIndexChanged);
         // 
         // tpgFrameset
         // 
         this.tpgFrameset.Controls.Add(this.pnlFrames);
         this.tpgFrameset.Controls.Add(this.splitterGraphics);
         this.tpgFrameset.Controls.Add(this.pnlGraphicSheet);
         this.tpgFrameset.Location = new System.Drawing.Point(4, 22);
         this.tpgFrameset.Name = "tpgFrameset";
         this.tpgFrameset.Size = new System.Drawing.Size(456, 447);
         this.tpgFrameset.TabIndex = 0;
         this.tpgFrameset.Text = "Frameset";
         // 
         // splitterGraphics
         // 
         this.splitterGraphics.Location = new System.Drawing.Point(216, 0);
         this.splitterGraphics.Name = "splitterGraphics";
         this.splitterGraphics.Size = new System.Drawing.Size(5, 447);
         this.splitterGraphics.TabIndex = 26;
         this.splitterGraphics.TabStop = false;
         // 
         // tpgFrameEditor
         // 
         this.tpgFrameEditor.Controls.Add(this.pnlTransform);
         this.tpgFrameEditor.Location = new System.Drawing.Point(4, 22);
         this.tpgFrameEditor.Name = "tpgFrameEditor";
         this.tpgFrameEditor.Size = new System.Drawing.Size(456, 447);
         this.tpgFrameEditor.TabIndex = 1;
         this.tpgFrameEditor.Text = "Frame Editor";
         // 
         // frmFrameEdit
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(464, 473);
         this.Controls.Add(this.tabFrameset);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Menu = this.mnuFrameset;
         this.Name = "frmFrameEdit";
         this.Text = "Frameset Editor";
         this.pnlFrames.ResumeLayout(false);
         this.pnlFrameAction.ResumeLayout(false);
         this.pnlTransform.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.nudYScale)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudXScale)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudXOffset)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudYOffset)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.trbRotate)).EndInit();
         this.pnlGraphicSheet.ResumeLayout(false);
         this.pnlGraphicSheetName.ResumeLayout(false);
         this.tabFrameset.ResumeLayout(false);
         this.tpgFrameset.ResumeLayout(false);
         this.tpgFrameEditor.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion

      #region Private members
      private MouseZone GetMousePointZone(int x, int y)
      {
         Point ptMouse = new Point(x - m_ptCenter.X, y - m_ptCenter.Y);

         if ((m_CurrentTransform == null) || (m_CurrentImage == null))
            return MouseZone.Outside;

         Matrix CurTransform = m_CurrentTransform.Clone();
         Matrix CtlTransform = GetTransform();
         CurTransform.Multiply(CtlTransform, MatrixOrder.Append);
         CtlTransform.Dispose();
         Rectangle rcBounds = Rectangle.Round(GetTransformedBounds(CurTransform, m_CurrentImage.Size));
         CurTransform.Dispose();
         Rectangle rcBoundsOuter= rcBounds;
         rcBoundsOuter.Inflate(5,5);

         if (GetGrabHandleRect(GrabHandlePosition.TopLeft).Contains(ptMouse))
            return MouseZone.TopLeftHandle;
         else if (GetGrabHandleRect(GrabHandlePosition.BottomRight).Contains(ptMouse))
            return MouseZone.BottomRightHandle;
         else if (GetGrabHandleRect(GrabHandlePosition.TopCenter).Contains(ptMouse))
            return MouseZone.TopCenterHandle;
         else if (GetGrabHandleRect(GrabHandlePosition.BottomCenter).Contains(ptMouse))
            return MouseZone.BottomCenterHandle;
         else if (GetGrabHandleRect(GrabHandlePosition.TopRight).Contains(ptMouse))
            return MouseZone.TopRightHandle;
         else if (GetGrabHandleRect(GrabHandlePosition.BottomLeft).Contains(ptMouse))
            return MouseZone.BottomLeftHandle;
         else if (GetGrabHandleRect(GrabHandlePosition.MiddleLeft).Contains(ptMouse))
            return MouseZone.MiddleLeftHandle;
         else if (GetGrabHandleRect(GrabHandlePosition.MiddleRight).Contains(ptMouse))
            return MouseZone.MiddleRightHandle;
         else if (GetGrabHandleRect(GrabHandlePosition.TopCenter).Contains(ptMouse))
            return MouseZone.TopCenterHandle;
         else if (GetGrabHandleRect(GrabHandlePosition.BottomCenter).Contains(ptMouse))
            return MouseZone.BottomCenterHandle;
         else if (rcBoundsOuter.Contains(ptMouse))
         {
            if (rcBounds.Contains(ptMouse))
               return MouseZone.Inside;
            else
               return MouseZone.Frame;
         }
         else
            return MouseZone.Outside;
      }
      private Rectangle GetGrabHandleRect(GrabHandlePosition pos)
      {
         if (m_CurrentImage == null)
            return Rectangle.Empty;
         Matrix Transform = m_CurrentTransform.Clone();
         Matrix CtlTransform = GetTransform();
         Transform.Multiply(CtlTransform,MatrixOrder.Append);
         CtlTransform.Dispose();
         Rectangle rcBounds = Rectangle.Round(GetTransformedBounds(Transform, m_CurrentImage.Size));
         Transform.Dispose();
         Rectangle rcResult = Rectangle.Empty;
         if ((pos == GrabHandlePosition.TopLeft) ||
            (pos == GrabHandlePosition.TopCenter) ||
            (pos == GrabHandlePosition.TopRight))
            rcResult.Y = rcBounds.Top - 5;
         if ((pos == GrabHandlePosition.MiddleLeft) ||
            (pos == GrabHandlePosition.MiddleRight))
            rcResult.Y = rcBounds.Top + rcBounds.Height / 2 - 2;
         if ((pos == GrabHandlePosition.BottomLeft) ||
            (pos == GrabHandlePosition.BottomCenter) ||
            (pos == GrabHandlePosition.BottomRight))
            rcResult.Y = rcBounds.Bottom;
         if ((pos == GrabHandlePosition.TopLeft) ||
            (pos == GrabHandlePosition.MiddleLeft) ||
            (pos == GrabHandlePosition.BottomLeft))
            rcResult.X = rcBounds.X - 5;
         if ((pos == GrabHandlePosition.TopCenter) ||
             (pos == GrabHandlePosition.BottomCenter))
            rcResult.X = rcBounds.X + rcBounds.Width / 2 - 2;
         if ((pos == GrabHandlePosition.TopRight) ||
            (pos == GrabHandlePosition.MiddleRight) ||
            (pos == GrabHandlePosition.BottomRight))
            rcResult.X = rcBounds.Right;
         rcResult.Width = rcResult.Height = 5;
         return rcResult;
      }

      private void ConnectData()
      {
         cboGraphicSheet.Items.Clear();
         foreach (ProjectDataset.GraphicSheetRow dr in ProjectData.GraphicSheet)
            cboGraphicSheet.Items.Add(dr);
      }

      private RectangleF GetTransformedBounds(Matrix Transform, Size cellSize)
      {
         PointF[] arptCorners = new PointF[]
         {
            new PointF(0f,0f), new PointF((float)cellSize.Width, 0f),
            new PointF((float)cellSize.Width, (float)cellSize.Height),
            new PointF(0f, (float)cellSize.Height)
         };

         Transform.TransformPoints(arptCorners);
         RectangleF rcTransformedBounds = new RectangleF(arptCorners[0], new SizeF(0f, 0f));
         for (int nIdx = 1; nIdx < arptCorners.Length; nIdx++)
         {
            if (arptCorners[nIdx].X < rcTransformedBounds.X)
            {
               rcTransformedBounds.Width += rcTransformedBounds.X - arptCorners[nIdx].X;
               rcTransformedBounds.X = arptCorners[nIdx].X;
            }
            else if (arptCorners[nIdx].X - rcTransformedBounds.X > rcTransformedBounds.Width)
               rcTransformedBounds.Width = arptCorners[nIdx].X - rcTransformedBounds.X;

            if (arptCorners[nIdx].Y < rcTransformedBounds.Y)
            {
               rcTransformedBounds.Height += rcTransformedBounds.Y - arptCorners[nIdx].Y;
               rcTransformedBounds.Y = arptCorners[nIdx].Y;
            }
            else if (arptCorners[nIdx].Y - rcTransformedBounds.Y > rcTransformedBounds.Height)
               rcTransformedBounds.Height = arptCorners[nIdx].Y - rcTransformedBounds.Y;
         }
         return rcTransformedBounds;
      }

      private RectangleF GetTransformedBounds()
      {
         return GetTransformedBounds(m_CurrentTransform, m_CurrentImage.Size);
      }

      private Boolean ParseDouble(string s, out double result)
      {
         return Double.TryParse(s, System.Globalization.NumberStyles.Number, System.Globalization.NumberFormatInfo.CurrentInfo, out result);
      }

      private Matrix GetTransform()
      {
         Double dblParse;
         Matrix Result = new Matrix();

         if (ParseDouble(txtRotate.Text, out dblParse))
         {
            RectangleF rcTransformed = GetTransformedBounds();

            if (chkRotateAroundCenter.Checked)
               Result.RotateAt((float)dblParse, new PointF(
                  rcTransformed.X + rcTransformed.Width / 2f,
                  rcTransformed.Y + rcTransformed.Height / 2f));
            else
               Result.Rotate((float)dblParse);
         }
         Result.Scale((float)nudXScale.Value, (float)nudYScale.Value, MatrixOrder.Append);
         Result.Translate((float)nudXOffset.Value, (float)nudYOffset.Value, MatrixOrder.Append);
         return Result;
      }

      private void ResetControls()
      {
         trbRotate.Value = 0;
         txtRotate.Text = "0";
         nudXOffset.Value = 0;
         nudYOffset.Value = 0;
         nudXScale.Value = 1;
         nudYScale.Value = 1;
      }

      private void RoundMatrix(ref Matrix Source)
      {
         Matrix NewMatrix = new Matrix((float)Math.Round(Source.Elements[0], 4),
            (float)Math.Round(Source.Elements[1], 4),
            (float)Math.Round(Source.Elements[2], 4),
            (float)Math.Round(Source.Elements[3], 4),
            (float)Math.Round(Source.Elements[4], 4),
            (float)Math.Round(Source.Elements[5], 4));
         Source.Dispose();
         Source = NewMatrix;
      }
      private void LoadFrameRow(ProjectDataset.FrameRow fr)
      {
         if (m_CurrentImage != null)
            m_CurrentImage.Dispose();
         m_CurrentImage = FrameBrowser.GetCellImageData(fr.FrameValue);
         m_nCurrentCellIndex = fr.CellIndex;
         if (m_CurrentTransform != null)
            m_CurrentTransform.Dispose();
         m_CurrentTransform = new Matrix(fr.m11, fr.m12, fr.m21, fr.m22, fr.dx, fr.dy);
         m_CurrentColor = fr.color;
         ResetControls();
         pnlTransform.Invalidate();
      }
      
      private void LaunchFrameRemappingWizard()
      {
         using (frmFrameRemappingWizard frm = new frmFrameRemappingWizard(FrameBrowser.Frameset))
            frm.ShowDialog();
      }
      private void DeleteSelectedFrames()
      {
         ProjectDataset.FrameRow[] deleteFrames = FrameBrowser.GetSelectedFrames();

         if (deleteFrames.Length == 0)
         {
            MessageBox.Show(this, "Select frames to delete first.", "Delete Selected Frames", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }

         int newCount = (FrameBrowser.Frameset.GetFrameRows().Length - deleteFrames.Length);

         System.Data.DataRow[] errors = ProjectData.SpriteFrame.Select(
            "Parent.FramesetName='" + FrameBrowser.Frameset.Name + "' and FrameValue>="
            + newCount.ToString(), string.Empty, DataViewRowState.CurrentRows);
         if (errors.Length > 0)
         {
            ProjectDataset.SpriteFrameRow errMsgRow = (ProjectDataset.SpriteFrameRow)errors[0];
            MessageBox.Show(this, "Deleting the selected frames would cause one or more sprites to reference frame indexes beyond the bounds of this frameset.  Remove frames from state \"" + errMsgRow.SpriteStateRowParent.Name + "\" of sprite definition \"" + errMsgRow.SpriteStateRowParent.SpriteDefinitionRow.Name + "\" and any other sprite states that may be affected before deleting these frames.", "Delete Frameset Frames", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }

         errors = ProjectData.Tile.Select(
            "Parent(TilesetTile).Frameset='" + FrameBrowser.Frameset.Name + "' and Max(Child(TileTileFrame).FrameValue)>="
            + newCount.ToString(), string.Empty, DataViewRowState.CurrentRows);
         if (errors.Length > 0)
         {
            ProjectDataset.TileRow errMsgRow = (ProjectDataset.TileRow)errors[0];
            MessageBox.Show(this, "Deleting the selected frames would cause one or more tile mappings to reference frame indexes beyond the bounds of this frameset.  Remove frames from tile number " + errMsgRow.TileValue.ToString() + " of tileset \"" + errMsgRow.TilesetRow.Name + "\" and any other tiles that may be affeted before deleting these frames.", "Delete Frameset Frames", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }

         if (DialogResult.Yes != MessageBox.Show(this, "Are you sure you want to delete " + deleteFrames.Length.ToString() + " frames?", "Delete Selected Frames", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            return;

         foreach (ProjectDataset.FrameRow fr in deleteFrames)
            ProjectData.DeleteFrame(fr);
      }

      private void AddSelectedCells()
      {
         for (short nIdx = 0; nIdx < CellBrowser.CellCount; nIdx++)
         {
            if (CellBrowser.Selected[nIdx])
            {
               ProjectData.AddFrameRow(FrameBrowser.Frameset, FrameBrowser.CellCount,
                  CellBrowser.GraphicSheet.Name, nIdx, 1f, 0f, 0f, 1f, 0f, 0f, -1);
            }
         }
         FrameBrowser.Invalidate();
      }

      private void CopyTransformToFrameRow(ProjectDataset.FrameRow fr, Matrix transform)
      {
         fr.m11 = transform.Elements[0];
         fr.m12 = transform.Elements[1];
         fr.m21 = transform.Elements[2];
         fr.m22 = transform.Elements[3];
         fr.dx = transform.Elements[4];
         fr.dy = transform.Elements[5];
      }

      private void PerformTransform(object sender, RectangleF bounds)
      {
         PointF ptCenter = new PointF(
            bounds.X + bounds.Width / 2f,
            bounds.Y + bounds.Height / 2f);
         if ((sender == mnuClockwise90Center) || (sender == mnuXClockwise90Center))
            m_CurrentTransform.RotateAt(90f, ptCenter, MatrixOrder.Append);
         else if ((sender == mnuClockwise90Origin) || (sender == mnuXClockwise90Origin))
            m_CurrentTransform.Rotate(90f, MatrixOrder.Append);
         else if ((sender == mnuCounter90Center) || (sender == mnuXCounter90Center))
            m_CurrentTransform.RotateAt(-90f, ptCenter, MatrixOrder.Append);
         else if ((sender == mnuCounter90Origin) || (sender == mnuXCounter90Origin))
            m_CurrentTransform.Rotate(-90f, MatrixOrder.Append);
         else if ((sender == mnuDoubleHeight) || (sender == mnuXDoubleHeight))
            m_CurrentTransform.Scale(1f, 2f, MatrixOrder.Append);
         else if ((sender == mnuDoubleWidth) || (sender == mnuXDoubleWidth))
            m_CurrentTransform.Scale(2f, 1f, MatrixOrder.Append);
         else if ((sender == mnuDownHalf) || (sender == mnuXDownHalf))
            m_CurrentTransform.Translate(0, bounds.Height / 2f, MatrixOrder.Append);
         else if ((sender == mnuUpHalf) || (sender == mnuXUpHalf))
            m_CurrentTransform.Translate(0, -bounds.Height / 2f, MatrixOrder.Append);
         else if ((sender == mnuLeftHalf) || (sender == mnuXLeftHalf))
            m_CurrentTransform.Translate(-bounds.Width / 2f, 0, MatrixOrder.Append);
         else if ((sender == mnuRightHalf) || (sender == mnuXRightHalf))
            m_CurrentTransform.Translate(bounds.Width / 2f, 0, MatrixOrder.Append);
         else if ((sender == mnuHalveHeight) || (sender == mnuXHalveHeight))
            m_CurrentTransform.Scale(1f, 0.5f, MatrixOrder.Append);
         else if ((sender == mnuHalveWidth) || (sender == mnuXHalveWidth))
            m_CurrentTransform.Scale(0.5f, 1f, MatrixOrder.Append);
         else if ((sender == mnuHFlipCenter) || (sender == mnuXHFlipCenter))
         {
            m_CurrentTransform.Translate(-ptCenter.X, -ptCenter.Y, MatrixOrder.Append);
            m_CurrentTransform.Scale(-1f, 1f, MatrixOrder.Append);
            m_CurrentTransform.Translate(ptCenter.X, ptCenter.Y, MatrixOrder.Append);
         }
         else if ((sender == mnuHFlipOrigin) || (sender == mnuXHFlipOrigin))
            m_CurrentTransform.Scale(-1f, 1f, MatrixOrder.Append);
         else if ((sender == mnuVFlipCenter) || (sender == mnuXVFlipCenter))
         {
            m_CurrentTransform.Translate(-ptCenter.X, -ptCenter.Y, MatrixOrder.Append);
            m_CurrentTransform.Scale(1f, -1f, MatrixOrder.Append);
            m_CurrentTransform.Translate(ptCenter.X, ptCenter.Y, MatrixOrder.Append);
         }
         else if ((sender == mnuVFlipOrigin) || (sender == mnuXVFlipOrigin))
            m_CurrentTransform.Scale(1f, -1f, MatrixOrder.Append);
         else if ((sender == mnuReset) || (sender == mnuXReset))
            m_CurrentTransform.Reset();
         else
            MessageBox.Show(this, "Not Implemented");

         RoundMatrix(ref m_CurrentTransform);
      }

      void ApplyCurrentTransform()
      {
         if (m_CurrentTransform == null)
            return;
         Matrix CtlTransform = GetTransform();
         m_CurrentTransform.Multiply(CtlTransform, MatrixOrder.Append);
         CtlTransform.Dispose();
         RoundMatrix(ref m_CurrentTransform);
         ResetControls();
      }
      #endregion

      #region Public Static Members
      public static void Edit(Form MdiParent, ProjectDataset.FramesetRow EditRow)
      {
         foreach(Form frm in MdiParent.MdiChildren)
         {
            frmFrameEdit f = frm as frmFrameEdit;
            if (f != null)
            {
               if (f.FrameBrowser.Frameset == EditRow)
               {
                  f.Activate();
                  return;
               }
            }
         }

         frmFrameEdit frmNew = new frmFrameEdit(EditRow);
         frmNew.MdiParent = MdiParent;
         frmNew.Show();
      }
      #endregion

      #region Overrides
      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad (e);
         ConnectData();
         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"Frameset.html");
         SGDK2IDE.g_HelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
      }

      protected override void OnClosing(CancelEventArgs e)
      {
         base.OnClosing (e);
         SGDK2IDE.SaveFormSettings(this);
      }
      #endregion

      #region Event Handlers
      private void GraphicSheet_GraphicSheetRowChange(object sender, SGDK2.ProjectDataset.GraphicSheetRowChangeEvent e)
      {
         if (e.Action == DataRowAction.Add)
            cboGraphicSheet.Items.Add(e.Row);
         else if (e.Action == DataRowAction.Delete)
            cboGraphicSheet.Items.Remove(e.Row);
      }

      private void cboGraphicSheet_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         CellBrowser.GraphicSheet = (ProjectDataset.GraphicSheetRow)cboGraphicSheet.SelectedItem;
         CellBrowser.ClearSelection();
      }

      private void FrameBrowser_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
      {
         GraphicBrowser gb = e.Data.GetData(typeof(GraphicBrowser)) as GraphicBrowser;
         if (gb != null)
         {
            if (gb.GraphicSheet == null)
            {
               if (0 != (e.KeyState & 8))
                  e.Effect = DragDropEffects.Copy;
               else
                  e.Effect = DragDropEffects.Move;
            }
            else
               e.Effect = DragDropEffects.Copy;
         }
      }

      private void FrameBrowser_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
      {
         try
         {
            GraphicBrowser gb = ((GraphicBrowser)e.Data.GetData(typeof(GraphicBrowser)));

            Point ptDrag = FrameBrowser.PointToClient(new Point(e.X, e.Y));
            int nFrameValue = FrameBrowser.GetCellAtXY(ptDrag.X, ptDrag.Y, HitFlags.GetNearest | HitFlags.AllowExtraCell);
            int nSelCount = 1;

            if (gb != null)
               nSelCount = gb.GetSelectedCellCount();

            Boolean bMove = (e.Effect == DragDropEffects.Move);

            if (gb != null)
            {
               if (gb.GraphicSheet == null)
               {
                  ProjectDataset.FrameRow[] selframes = gb.GetSelectedFrames();

                  for (int nIdx = 0; nIdx < selframes.Length; nIdx++)
                  {
                     if (bMove)
                        nFrameValue = ProjectData.MoveFrame(selframes[nIdx], nFrameValue);
                     else
                     {
                        ProjectData.InsertFrame(FrameBrowser.Frameset, nFrameValue++,
                           selframes[nIdx].GraphicSheet, selframes[nIdx].CellIndex,
                           selframes[nIdx].m11, selframes[nIdx].m12, selframes[nIdx].m21,
                           selframes[nIdx].m22, selframes[nIdx].dx, selframes[nIdx].dy,
                           selframes[nIdx].color);
                     }
                  }
               }
               else
               {
                  for(short nIdx = 0; nIdx < gb.CellCount; nIdx++)
                  {
                     if (gb.Selected[nIdx])
                     {
                        ProjectData.InsertFrame(FrameBrowser.Frameset,
                           nFrameValue++, gb.GraphicSheet.Name, nIdx,
                           1, 0, 0, 1, 0, 0,-1);
                     }
                  }
               }
            }
         }
         catch(Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Error Dropping Frame", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            ProjectData.RejectChanges();
         }
      }

      private void trbRotate_Scroll(object sender, System.EventArgs e)
      {
         txtRotate.Text = trbRotate.Value.ToString();
      }

      private void pnlTransform_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         if ((m_CurrentImage == null) || (m_CurrentTransform == null))
            return;

         m_ptCenter = new Point(btnOK.Left / 2, trbRotate.Top / 2);
         try
         {
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            Matrix PaintTransform = m_CurrentTransform.Clone();
            Matrix CtlTransform = GetTransform();
            PaintTransform.Multiply(CtlTransform, MatrixOrder.Append);
            CtlTransform.Dispose();
            e.Graphics.Transform = PaintTransform;
            e.Graphics.TranslateTransform(m_ptCenter.X, m_ptCenter.Y, MatrixOrder.Append);
            if (m_CurrentColor != -1)
            {
               byte[] clr = BitConverter.GetBytes(m_CurrentColor);
               System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix(
                  new float[][]
               {
                  new float[] {(float)clr[2]/255.0f, 0, 0, 0, 0},
                  new float[] {0, (float)clr[1]/255.0f, 0, 0, 0},
                  new float[] {0, 0, (float)clr[0]/255.0f, 0, 0},
                  new float[] {0, 0, 0, (float)clr[3]/255.0f, 0},
                  new float[] {0, 0, 0, 0, 1}
               });
               using (System.Drawing.Imaging.ImageAttributes attr = new System.Drawing.Imaging.ImageAttributes())
               {
                  attr.SetColorMatrices(cm, cm);
                  e.Graphics.DrawImage(m_CurrentImage,
                     new Rectangle(0,0,m_CurrentImage.Width,m_CurrentImage.Height),
                     0,0,m_CurrentImage.Width,m_CurrentImage.Height,
                     System.Drawing.GraphicsUnit.Pixel, attr);
               }
            }
            else
            {
               e.Graphics.DrawImage(m_CurrentImage, 0, 0);
            }
            
            e.Graphics.ResetTransform();
            e.Graphics.TranslateTransform(m_ptCenter.X, m_ptCenter.Y, MatrixOrder.Append);
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Default;
            Rectangle rcBounds = Rectangle.Round(GetTransformedBounds(PaintTransform, m_CurrentImage.Size));
            Rectangle rcBoundsOuter= rcBounds;
            rcBoundsOuter.Inflate(5,5);
            ControlPaint.DrawSelectionFrame(e.Graphics, false, rcBoundsOuter, rcBounds, pnlTransform.BackColor);
            ControlPaint.DrawGrabHandle(e.Graphics, GetGrabHandleRect(GrabHandlePosition.TopLeft), true, true);
            ControlPaint.DrawGrabHandle(e.Graphics, GetGrabHandleRect(GrabHandlePosition.TopCenter), false, true);
            ControlPaint.DrawGrabHandle(e.Graphics, GetGrabHandleRect(GrabHandlePosition.TopRight), false, true);
            ControlPaint.DrawGrabHandle(e.Graphics, GetGrabHandleRect(GrabHandlePosition.MiddleLeft), false, true);
            ControlPaint.DrawGrabHandle(e.Graphics, GetGrabHandleRect(GrabHandlePosition.MiddleRight), false, true);
            ControlPaint.DrawGrabHandle(e.Graphics, GetGrabHandleRect(GrabHandlePosition.BottomLeft), false, true);
            ControlPaint.DrawGrabHandle(e.Graphics, GetGrabHandleRect(GrabHandlePosition.BottomCenter), false, true);
            ControlPaint.DrawGrabHandle(e.Graphics, GetGrabHandleRect(GrabHandlePosition.BottomRight), false, true);
            PaintTransform.Dispose();
         }
         catch (Exception ex)
         {
            if (((TimeSpan)DateTime.Now.Subtract(dtLastPaintError)).TotalSeconds > 5)
            {
               MessageBox.Show(this, "Error drawing transformed graphic -- probably an invalid matrix.\r\n(" + ex.Message + ")",
                  this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               dtLastPaintError = DateTime.Now;
            }
         }

         Point ptBottom = new Point(m_ptCenter.X, trbRotate.Top - 1);
         Point ptRight = new Point(btnOK.Left, m_ptCenter.Y);
         Point ptLeft = new Point(0, m_ptCenter.Y);
         Point ptTop = new Point(m_ptCenter.X, 0);
         e.Graphics.ResetTransform();
         e.Graphics.DrawLine(Pens.DarkBlue, ptTop, ptBottom);
         e.Graphics.DrawLine(Pens.DarkBlue, ptLeft, ptRight);
      }

      private void txtRotate_TextChanged(object sender, System.EventArgs e)
      {
         double dblVal;
         if (ParseDouble(txtRotate.Text, out dblVal))
         {
            int nVal = (int)dblVal;
            if ((nVal >= trbRotate.Minimum) && (nVal <= trbRotate.Maximum) && (nVal != trbRotate.Value))
               trbRotate.Value = nVal;
         }
         pnlTransform.Invalidate();
      }

      private void btnApply_Click(object sender, System.EventArgs e)
      {
         Matrix CtlTransform = GetTransform();
         m_CurrentTransform.Multiply(CtlTransform, MatrixOrder.Append);
         CtlTransform.Dispose();
         RoundMatrix(ref m_CurrentTransform);
         ResetControls();
         int editCell = 0;
         if (m_FrameEditorSource is ProjectDataset.FrameRow)
         {
            CopyTransformToFrameRow((ProjectDataset.FrameRow)m_FrameEditorSource, m_CurrentTransform);
            editCell = ((ProjectDataset.FrameRow)m_FrameEditorSource).FrameValue;
         }
         else if (m_FrameEditorSource is ProjectDataset.GraphicSheetRow)
         {
            ProjectData.AddFrameRow(FrameBrowser.Frameset, FrameBrowser.CellCount, ((ProjectDataset.GraphicSheetRow)m_FrameEditorSource).Name, m_nCurrentCellIndex,
               m_CurrentTransform.Elements[0], m_CurrentTransform.Elements[1],
               m_CurrentTransform.Elements[2], m_CurrentTransform.Elements[3],
               m_CurrentTransform.Elements[4], m_CurrentTransform.Elements[5], m_CurrentColor);
            editCell = FrameBrowser.CellCount - 1;
         }
         editingFrame = false;
         tabFrameset.SelectedTab = tpgFrameset;
         FrameBrowser.CurrentCellIndex = editCell;
      }

      private void btnReset_Click(object sender, System.EventArgs e)
      {
         if (m_CurrentTransform != null)
            m_CurrentTransform.Dispose();
         m_CurrentTransform = new Matrix();
         ResetControls();
         pnlTransform.Invalidate();
      }

      private void mnuContextItem_Click(object sender, System.EventArgs e)
      {
         if (mnuContext.SourceControl == pnlTransform)
         {
            ApplyCurrentTransform();
            PerformTransform(sender, GetTransformedBounds());
            pnlTransform.Invalidate();
         }
         else if (mnuContext.SourceControl == FrameBrowser)
         {
            ProjectDataset.FrameRow[] frameList = FrameBrowser.GetSelectedFrames();
            if (frameList == null)
               return;
            foreach(ProjectDataset.FrameRow fr in frameList)
            {
               if (m_CurrentTransform != null)
                  m_CurrentTransform.Dispose();
               m_CurrentTransform = new Matrix(fr.m11, fr.m12, fr.m21, fr.m22, fr.dx, fr.dy);
               ProjectDataset.GraphicSheetRow drGfx = ProjectData.GetGraphicSheet(fr.GraphicSheet);
               Size cellSize = new Size(drGfx.CellWidth, drGfx.CellHeight);
               PerformTransform(sender, GetTransformedBounds(m_CurrentTransform, cellSize));
               CopyTransformToFrameRow(fr, m_CurrentTransform);
            }
            if (m_CurrentTransform != null)
               m_CurrentTransform.Dispose();
         }
      }

      private void mnuTransformItem_Click(object sender, System.EventArgs e)
      {
         if (tabFrameset.SelectedTab == tpgFrameEditor)
         {
            ApplyCurrentTransform();
            PerformTransform(sender, GetTransformedBounds());
            pnlTransform.Invalidate();
         }
         else
         {
            foreach(ProjectDataset.FrameRow fr in FrameBrowser.GetSelectedFrames())
            {
               if (m_CurrentTransform != null)
                  m_CurrentTransform.Dispose();
               m_CurrentTransform = new Matrix(fr.m11, fr.m12, fr.m21, fr.m22, fr.dx, fr.dy);
               ProjectDataset.GraphicSheetRow drGfx = ProjectData.GetGraphicSheet(fr.GraphicSheet);
               Size cellSize = new Size(drGfx.CellWidth, drGfx.CellHeight);
               PerformTransform(sender, GetTransformedBounds(m_CurrentTransform, cellSize));
               CopyTransformToFrameRow(fr, m_CurrentTransform);
            }
            if (m_CurrentTransform != null)
               m_CurrentTransform.Dispose();
         }
      }

      private void TransformControl_ValueChanged(object sender, System.EventArgs e)
      {
         pnlTransform.Invalidate();      
      }

      private void TransformControl_Validated(object sender, System.EventArgs e)
      {
         decimal dummy = ((NumericUpDown)sender).Value;
      }

      private void pnlTransform_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         if (0 != (int)(e.Button & MouseButtons.Left))
         {
            m_DragType = GetMousePointZone(e.X, e.Y);
            m_DragStart = new Point(e.X - m_ptCenter.X, e.Y - m_ptCenter.Y);
         }
      }

      private void pnlTransform_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         m_DragType = MouseZone.Outside;
      }

      private void pnlTransform_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         bool bTempMode = false;
         if (m_DragType == MouseZone.Outside)
         {
            bTempMode = true;
            m_DragType = GetMousePointZone(e.X, e.Y);
         }

         switch (m_DragType)
         {
            case MouseZone.TopLeftHandle:
            case MouseZone.BottomRightHandle:
               Cursor.Current = Cursors.SizeNWSE;
               break;
            case MouseZone.TopCenterHandle:
            case MouseZone.BottomCenterHandle:
               Cursor.Current = Cursors.SizeNS;
               break;
            case MouseZone.TopRightHandle:
            case MouseZone.BottomLeftHandle:
               Cursor.Current = Cursors.SizeNESW;
               break;
            case MouseZone.MiddleLeftHandle:
            case MouseZone.MiddleRightHandle:
               Cursor.Current = Cursors.SizeWE;
               break;
            case MouseZone.Frame:
            case MouseZone.Inside:
               Cursor.Current = Cursors.SizeAll;
               break;
         }

         if (bTempMode)
            m_DragType = MouseZone.Outside;
         else
         {
            if (txtRotate.Text != "0")
               ApplyCurrentTransform();
            RectangleF rcOriginal = GetTransformedBounds();
            Matrix CurTransform = m_CurrentTransform.Clone();
            Matrix CtlTransform = GetTransform();
            CurTransform.Multiply(CtlTransform, MatrixOrder.Append);
            CtlTransform.Dispose();
            RectangleF rcNew = GetTransformedBounds(CurTransform, m_CurrentImage.Size);
            CurTransform.Dispose();
            Point ptMouse = new Point(e.X - m_ptCenter.X, e.Y - m_ptCenter.Y);
               
            decimal XScale, YScale, XOffset, YOffset;
               
            switch (m_DragType)
            {
               case MouseZone.TopLeftHandle:
                  rcNew = RectangleF.FromLTRB(ptMouse.X, ptMouse.Y, rcNew.Right, rcNew.Bottom);
                  break;
               case MouseZone.TopCenterHandle:
                  rcNew = RectangleF.FromLTRB(rcNew.X, ptMouse.Y, rcNew.Right, rcNew.Bottom);
                  break;
               case MouseZone.TopRightHandle:
                  rcNew = RectangleF.FromLTRB(rcNew.X, ptMouse.Y, ptMouse.X, rcNew.Bottom);
                  break;
               case MouseZone.MiddleLeftHandle:
                  rcNew = RectangleF.FromLTRB(ptMouse.X, rcNew.Y, rcNew.Right, rcNew.Bottom);
                  break;
               case MouseZone.MiddleRightHandle:
                  rcNew = RectangleF.FromLTRB(rcNew.X, rcNew.Y, ptMouse.X, rcNew.Bottom);
                  break;
               case MouseZone.BottomLeftHandle:
                  rcNew = RectangleF.FromLTRB(ptMouse.X, rcNew.Y, rcNew.Right, ptMouse.Y);
                  break;
               case MouseZone.BottomCenterHandle:
                  rcNew = RectangleF.FromLTRB(rcNew.X, rcNew.Y, rcNew.Right, ptMouse.Y);
                  break;
               case MouseZone.BottomRightHandle:
                  rcNew = RectangleF.FromLTRB(rcNew.X, rcNew.Y, ptMouse.X, ptMouse.Y);
                  break;
               case MouseZone.Frame:
               case MouseZone.Inside:
                  rcNew.Offset(ptMouse.X - m_DragStart.X, ptMouse.Y - m_DragStart.Y);
                  m_DragStart = ptMouse;
                  break;
            }
               
            XScale = (decimal)(rcNew.Width / rcOriginal.Width);
            YScale = (decimal)(rcNew.Height / rcOriginal.Height);
            CurTransform = m_CurrentTransform.Clone();
            CurTransform.Scale((float)XScale, (float)YScale, MatrixOrder.Append);
            rcOriginal = GetTransformedBounds(CurTransform, m_CurrentImage.Size);
            CurTransform.Dispose();
            XOffset = (decimal)(rcNew.X - rcOriginal.X);
            YOffset = (decimal)(rcNew.Y - rcOriginal.Y);

            if (Math.Abs(XScale) > .01m)
            {
               if (XScale < nudXScale.Minimum)
                  nudXScale.Value = nudXScale.Minimum;
               else if (XScale > nudXScale.Maximum)
                  nudXScale.Value = nudXScale.Maximum;
               else
                  nudXScale.Value = Math.Round(XScale, 4);
            }
            if (Math.Abs(YScale) > .01m)
            {
               if (YScale < nudYScale.Minimum)
                  nudYScale.Value = nudYScale.Minimum;
               else if (YScale > nudYScale.Maximum)
                  nudYScale.Value = nudYScale.Maximum;
               else
                  nudYScale.Value = Math.Round(YScale, 4);
            }
            if (XOffset < nudXOffset.Minimum)
               nudXOffset.Value = nudXOffset.Minimum;
            else if (XOffset > nudXOffset.Maximum)
               nudXOffset.Value = nudXOffset.Maximum;
            else
               nudXOffset.Value = Math.Round(XOffset, 4);
            if (YOffset < nudYOffset.Minimum)
               nudYOffset.Value = nudYOffset.Minimum;
            else if (YOffset > nudYOffset.Maximum)
               nudYOffset.Value = nudYOffset.Maximum;
            else
               nudYOffset.Value = Math.Round(YOffset, 4);
         }
      }

      private void pnlTransform_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
      {
         if (e.Data.GetData(typeof(GraphicBrowser)) != null)
            e.Effect = DragDropEffects.Copy;
      }

      private void mnuAddCell_Click(object sender, System.EventArgs e)
      {
         AddSelectedCells();
      }

      private void Frameset_FramesetRowDeleted(object sender, SGDK2.ProjectDataset.FramesetRowChangeEvent e)
      {
         if (e.Row == FrameBrowser.Frameset)
            this.Close();
      }

      private void ProjectData_Clearing(object sender, EventArgs e)
      {
         this.Close();
      }

      private void pnlTransform_Resize(object sender, System.EventArgs e)
      {
         pnlTransform.Invalidate();
      }

      private void txtFramesetName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         string sValid = ProjectData.ValidateName(txtFramesetName.Text);

         if (sValid != null)
         {
            if (DialogResult.Cancel == MessageBox.Show(this, sValid, "Frameset Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtFramesetName.Text = FrameBrowser.Frameset.Name;
            e.Cancel = true;
         }
         ProjectDataset.FramesetRow fr = ProjectData.GetFrameSet(txtFramesetName.Text);
         if ((null != fr) && (FrameBrowser.Frameset != fr))
         {
            if (DialogResult.Cancel == MessageBox.Show(this, txtFramesetName.Text + " already exists", "Frameset Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtFramesetName.Text = FrameBrowser.Frameset.Name;
            e.Cancel = true;
         }
      }

      private void txtFramesetName_Validated(object sender, System.EventArgs e)
      {
         FrameBrowser.Frameset.Name = txtFramesetName.Text;
      }

      private void FrameBrowser_CurrentCellChanged(object sender, System.EventArgs e)
      {
         int selCount = FrameBrowser.GetSelectedCellCount();
         ProjectDataset.FrameRow[] selectedRows = FrameBrowser.GetSelectedFrames();
         switch (selCount)
         {
            case 0:
               FrameProperties.SelectedObject = null;
               break;
            case 1:
               FrameProperties.SelectedObject = new FrameProvider(selectedRows[0]);
               m_FrameEditorSource = selectedRows[0];
               break;
            default:
            {
               FrameProvider[] selection = new FrameProvider[selCount];
               for (int idx=0; idx < selCount; idx++)
                  selection[idx] = new FrameProvider(selectedRows[idx]);
               FrameProperties.SelectedObjects = selection;
               m_FrameEditorSource = selectedRows[0];
               break;
            }
         }
      }

      private void mnuFrameRemappingWizard_Click(object sender, System.EventArgs e)
      {
         LaunchFrameRemappingWizard();
      }

      private void mnuDeleteFrames_Click(object sender, System.EventArgs e)
      {
         DeleteSelectedFrames();
      }

      private void tbrGraphicSheet_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
      {
         if (e.Button == tbbAddToFrameset)
            AddSelectedCells();
      }

      private void tabFrameset_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (tabFrameset.SelectedTab == tpgFrameEditor)
         {
            mnuDeleteFrames.Enabled = false;
            if (!editingFrame)
            {
               if (m_FrameEditorSource is ProjectDataset.FrameRow)
               {
                  LoadFrameRow((ProjectDataset.FrameRow)m_FrameEditorSource);
               }
               else if ((CellBrowser.CurrentCellIndex >= 0) && (CellBrowser.GraphicSheet != null))
               {
                  if (m_CurrentImage != null)
                     m_CurrentImage.Dispose();
                  m_CurrentImage = CellBrowser.GetCellImageData(CellBrowser.CurrentCellIndex);
                  m_nCurrentCellIndex = (short)CellBrowser.CurrentCellIndex;
                  if (m_CurrentTransform != null)
                     m_CurrentTransform.Dispose();
                  m_CurrentTransform = new Matrix();
                  ResetControls();
                  pnlTransform.Invalidate();
               }
               editingFrame = true;
            }
         }
         else
         {
            if (ActiveControl != FrameProperties)
               mnuDeleteFrames.Enabled = true;
            if (editingFrame)
            {
               if (m_FrameEditorSource is ProjectDataset.GraphicSheetRow)
               {
                  switch (MessageBox.Show(this, "Do you want to add the transformed graphic to the frameset?", "Unsaved Changes Exist", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                  {
                     case DialogResult.Yes:
                        btnApply_Click(tabFrameset, null);
                        editingFrame = false;
                        break;
                     case DialogResult.No:
                        editingFrame = false;
                        break;
                     case DialogResult.Cancel:
                        tabFrameset.SelectedTab = tpgFrameEditor;
                        break;
                  }
               }
               else if (m_FrameEditorSource is ProjectDataset.FrameRow)
               {
                  switch (MessageBox.Show(this, "Do you want to update the frame?", "Unsaved Changes Exist", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                  {
                     case DialogResult.Yes:
                        btnApply_Click(tabFrameset, null);
                        editingFrame = false;
                        break;
                     case DialogResult.No:
                        editingFrame = false;
                        break;
                     case DialogResult.Cancel:
                        tabFrameset.SelectedTab = tpgFrameEditor;
                        break;
                  }
               }
               else
                  editingFrame = false;
            }
         }
      }

      private void CellBrowser_CurrentCellChanged(object sender, System.EventArgs e)
      {
         if (CellBrowser.GetSelectedCellCount() > 0)
            m_FrameEditorSource = CellBrowser.GraphicSheet;
      }

      private void btnCancel_Click(object sender, System.EventArgs e)
      {
         if (m_CurrentTransform != null)
            m_CurrentTransform.Dispose();
         m_CurrentTransform = null;
         ResetControls();
         editingFrame = false;
         tabFrameset.SelectedTab = tpgFrameset;
      }

      private void FrameProperties_Enter(object sender, System.EventArgs e)
      {
         mnuDeleteFrames.Enabled = false;
      }

      private void FrameProperties_Leave(object sender, System.EventArgs e)
      {
         mnuDeleteFrames.Enabled = true;
      }

      private void mnuTransformCustom_Click(object sender, System.EventArgs e)
      {
         tabFrameset.SelectedTab = tpgFrameEditor;
      }

      private void FrameBrowser_DoubleClick(object sender, System.EventArgs e)
      {
         if (m_FrameEditorSource is ProjectDataset.FrameRow)
            tabFrameset.SelectedTab = tpgFrameEditor;
      }

      private void CellBrowser_DoubleClick(object sender, System.EventArgs e)
      {
         if ((m_FrameEditorSource is ProjectDataset.GraphicSheetRow) && (CellBrowser.CurrentCellIndex >= 0))
            tabFrameset.SelectedTab = tpgFrameEditor;
      }

      private void mnuGraphicEditor_Click(object sender, System.EventArgs e)
      {
         if (m_FrameEditorSource is ProjectDataset.FrameRow)
         {
            ProjectDataset.FrameRow fr = ((ProjectDataset.FrameRow)m_FrameEditorSource);
            frmGraphicsEditor.Edit(MdiParent, ProjectData.GetGraphicSheet(fr.GraphicSheet), fr.CellIndex);
         }
         else if (m_FrameEditorSource is ProjectDataset.GraphicSheetRow)
         {
            if (CellBrowser.CurrentCellIndex >= 0)
            {
               ProjectDataset.GraphicSheetRow gr = CellBrowser.GraphicSheet;
               frmGraphicsEditor.Edit(MdiParent, CellBrowser.GraphicSheet, CellBrowser.CurrentCellIndex);
            }
         }
      }

      private void FrameBrowser_Resize(object sender, System.EventArgs e)
      {
         if (FrameBrowser.CurrentCellIndex >= 0)
            FrameBrowser.ScrollCellIntoView(FrameBrowser.CurrentCellIndex);
      }
      #endregion
   }
}
