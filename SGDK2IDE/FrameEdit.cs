/*
 * Scrolling Game Development Kit 2.0
 *
 * Copyright © 2000 - 2004 Benjamin Marty <BlueMonkMN@email.com>
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
      string m_sCurrentSheetName = null;
      short m_nCurrentCellIndex = -1;
      Point m_ptCenter = Point.Empty;
      #endregion

      #region Windows Form Designer Components
      private System.Windows.Forms.Panel pnlFrameSet;
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
      private System.Windows.Forms.Button btnApply;
      private System.Windows.Forms.Button btnReset;
      private System.Windows.Forms.Panel pnlGraphicSheet;
      private System.Windows.Forms.Splitter splitterGraphicSheet;
      private System.Windows.Forms.Button btnLoadFrame;
      private System.Windows.Forms.Button btnSaveFrame;
      private System.Windows.Forms.Button btnDeleteFrame;
      private System.Windows.Forms.Label lblFrameInfo;
      private System.Windows.Forms.ToolTip ttFrameset;
      private SGDK2.GraphicBrowser CellBrowser;
      private System.Windows.Forms.Label lblFramesetName;
      private System.Windows.Forms.TextBox txtFramesetName;
      private System.Windows.Forms.ComboBox cboGraphicSheet;
      private System.Windows.Forms.Button btnAddFrame;
      private System.Windows.Forms.TrackBar trbRotate;
      private System.Windows.Forms.CheckBox chkRotateAroundCenter;
      private System.Windows.Forms.Button btnMore;
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
      private System.Windows.Forms.Label lblMatrix;
      private System.Windows.Forms.TextBox txtM11;
      private System.Windows.Forms.TextBox txtM12;
      private System.Windows.Forms.TextBox txtM21;
      private System.Windows.Forms.TextBox txtM22;
      private System.Windows.Forms.TextBox txtDY;
      private System.Windows.Forms.TextBox txtDX;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Button btnLoadGCell;
      private SGDK2.DataChangeNotifier dataMonitor;
      private System.Windows.Forms.Label lblColorAdjust;
      private System.Windows.Forms.Label lblRed;
      private System.Windows.Forms.TextBox txtRed;
      private System.Windows.Forms.TextBox txtGreen;
      private System.Windows.Forms.Label lblGreen;
      private System.Windows.Forms.TextBox txtAlpha;
      private System.Windows.Forms.Label lblAlpha;
      private System.Windows.Forms.TextBox txtBlue;
      private System.Windows.Forms.Label lblBlue;
      private System.Windows.Forms.StatusBar sbFrame;
      private System.Windows.Forms.StatusBarPanel sbpCellIndex;
      private System.Windows.Forms.StatusBarPanel sbpFrameIndex;
      private System.Windows.Forms.MainMenu mnuFrameset;
      private System.Windows.Forms.MenuItem mnuFrameRemappingWizard;
      private System.Windows.Forms.MenuItem mnuFramesetPop;
      private System.Windows.Forms.MenuItem mnuDeleteFrames;
      private System.Windows.Forms.MenuItem mnuLoadCell;
      private System.Windows.Forms.MenuItem mnuAddCell;
      private System.Windows.Forms.MenuItem mnuFramesetSeparator;
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
         this.pnlFrameSet = new System.Windows.Forms.Panel();
         this.pnlFrames = new System.Windows.Forms.Panel();
         this.FrameBrowser = new SGDK2.GraphicBrowser();
         this.pnlFrameAction = new System.Windows.Forms.Panel();
         this.lblFrameInfo = new System.Windows.Forms.Label();
         this.btnDeleteFrame = new System.Windows.Forms.Button();
         this.btnSaveFrame = new System.Windows.Forms.Button();
         this.btnLoadFrame = new System.Windows.Forms.Button();
         this.txtFramesetName = new System.Windows.Forms.TextBox();
         this.lblFramesetName = new System.Windows.Forms.Label();
         this.sbFrame = new System.Windows.Forms.StatusBar();
         this.sbpFrameIndex = new System.Windows.Forms.StatusBarPanel();
         this.sbpCellIndex = new System.Windows.Forms.StatusBarPanel();
         this.pnlTransform = new System.Windows.Forms.Panel();
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
         this.txtAlpha = new System.Windows.Forms.TextBox();
         this.lblAlpha = new System.Windows.Forms.Label();
         this.txtBlue = new System.Windows.Forms.TextBox();
         this.lblBlue = new System.Windows.Forms.Label();
         this.txtGreen = new System.Windows.Forms.TextBox();
         this.lblGreen = new System.Windows.Forms.Label();
         this.txtRed = new System.Windows.Forms.TextBox();
         this.lblRed = new System.Windows.Forms.Label();
         this.lblColorAdjust = new System.Windows.Forms.Label();
         this.txtDY = new System.Windows.Forms.TextBox();
         this.txtDX = new System.Windows.Forms.TextBox();
         this.txtM22 = new System.Windows.Forms.TextBox();
         this.txtM21 = new System.Windows.Forms.TextBox();
         this.txtM12 = new System.Windows.Forms.TextBox();
         this.txtM11 = new System.Windows.Forms.TextBox();
         this.lblMatrix = new System.Windows.Forms.Label();
         this.nudYScale = new System.Windows.Forms.NumericUpDown();
         this.nudXScale = new System.Windows.Forms.NumericUpDown();
         this.btnMore = new System.Windows.Forms.Button();
         this.nudXOffset = new System.Windows.Forms.NumericUpDown();
         this.nudYOffset = new System.Windows.Forms.NumericUpDown();
         this.lblXOffset = new System.Windows.Forms.Label();
         this.btnApply = new System.Windows.Forms.Button();
         this.btnReset = new System.Windows.Forms.Button();
         this.lblYOffset = new System.Windows.Forms.Label();
         this.lblYScale = new System.Windows.Forms.Label();
         this.lblXScale = new System.Windows.Forms.Label();
         this.chkRotateAroundCenter = new System.Windows.Forms.CheckBox();
         this.lblRotate = new System.Windows.Forms.Label();
         this.txtRotate = new System.Windows.Forms.TextBox();
         this.trbRotate = new System.Windows.Forms.TrackBar();
         this.pnlGraphicSheet = new System.Windows.Forms.Panel();
         this.btnLoadGCell = new System.Windows.Forms.Button();
         this.label1 = new System.Windows.Forms.Label();
         this.btnAddFrame = new System.Windows.Forms.Button();
         this.cboGraphicSheet = new System.Windows.Forms.ComboBox();
         this.CellBrowser = new SGDK2.GraphicBrowser();
         this.splitterGraphicSheet = new System.Windows.Forms.Splitter();
         this.ttFrameset = new System.Windows.Forms.ToolTip(this.components);
         this.dataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.mnuFrameset = new System.Windows.Forms.MainMenu();
         this.mnuFramesetPop = new System.Windows.Forms.MenuItem();
         this.mnuDeleteFrames = new System.Windows.Forms.MenuItem();
         this.mnuFrameRemappingWizard = new System.Windows.Forms.MenuItem();
         this.mnuLoadCell = new System.Windows.Forms.MenuItem();
         this.mnuAddCell = new System.Windows.Forms.MenuItem();
         this.mnuFramesetSeparator = new System.Windows.Forms.MenuItem();
         this.pnlFrameSet.SuspendLayout();
         this.pnlFrames.SuspendLayout();
         this.pnlFrameAction.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.sbpFrameIndex)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpCellIndex)).BeginInit();
         this.pnlTransform.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.nudYScale)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudXScale)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudXOffset)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudYOffset)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.trbRotate)).BeginInit();
         this.pnlGraphicSheet.SuspendLayout();
         this.SuspendLayout();
         // 
         // pnlFrameSet
         // 
         this.pnlFrameSet.Controls.Add(this.pnlFrames);
         this.pnlFrameSet.Controls.Add(this.pnlTransform);
         this.pnlFrameSet.Dock = System.Windows.Forms.DockStyle.Fill;
         this.pnlFrameSet.Location = new System.Drawing.Point(0, 0);
         this.pnlFrameSet.Name = "pnlFrameSet";
         this.pnlFrameSet.Size = new System.Drawing.Size(464, 423);
         this.pnlFrameSet.TabIndex = 21;
         // 
         // pnlFrames
         // 
         this.pnlFrames.Controls.Add(this.FrameBrowser);
         this.pnlFrames.Controls.Add(this.pnlFrameAction);
         this.pnlFrames.Controls.Add(this.sbFrame);
         this.pnlFrames.Dock = System.Windows.Forms.DockStyle.Fill;
         this.pnlFrames.Location = new System.Drawing.Point(272, 0);
         this.pnlFrames.Name = "pnlFrames";
         this.pnlFrames.Size = new System.Drawing.Size(192, 423);
         this.pnlFrames.TabIndex = 25;
         // 
         // FrameBrowser
         // 
         this.FrameBrowser.AllowDrop = true;
         this.FrameBrowser.AutoScroll = true;
         this.FrameBrowser.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.FrameBrowser.CellPadding = new System.Drawing.Size(3, 3);
         this.FrameBrowser.CellSize = new System.Drawing.Size(0, 0);
         this.FrameBrowser.CurrentCellIndex = -1;
         this.FrameBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
         this.FrameBrowser.Frameset = null;
         this.FrameBrowser.FramesToDisplay = null;
         this.FrameBrowser.GraphicSheet = null;
         this.FrameBrowser.Location = new System.Drawing.Point(0, 80);
         this.FrameBrowser.Name = "FrameBrowser";
         this.FrameBrowser.SheetImage = null;
         this.FrameBrowser.Size = new System.Drawing.Size(192, 321);
         this.FrameBrowser.TabIndex = 6;
         this.FrameBrowser.CurrentCellChanged += new System.EventHandler(this.FrameBrowser_CurrentCellChanged);
         this.FrameBrowser.DragDrop += new System.Windows.Forms.DragEventHandler(this.FrameBrowser_DragDrop);
         this.FrameBrowser.DragOver += new System.Windows.Forms.DragEventHandler(this.FrameBrowser_DragOver);
         // 
         // pnlFrameAction
         // 
         this.pnlFrameAction.Controls.Add(this.lblFrameInfo);
         this.pnlFrameAction.Controls.Add(this.btnDeleteFrame);
         this.pnlFrameAction.Controls.Add(this.btnSaveFrame);
         this.pnlFrameAction.Controls.Add(this.btnLoadFrame);
         this.pnlFrameAction.Controls.Add(this.txtFramesetName);
         this.pnlFrameAction.Controls.Add(this.lblFramesetName);
         this.pnlFrameAction.Dock = System.Windows.Forms.DockStyle.Top;
         this.pnlFrameAction.Location = new System.Drawing.Point(0, 0);
         this.pnlFrameAction.Name = "pnlFrameAction";
         this.pnlFrameAction.Size = new System.Drawing.Size(192, 80);
         this.pnlFrameAction.TabIndex = 1;
         // 
         // lblFrameInfo
         // 
         this.lblFrameInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.lblFrameInfo.Location = new System.Drawing.Point(8, 24);
         this.lblFrameInfo.Name = "lblFrameInfo";
         this.lblFrameInfo.Size = new System.Drawing.Size(178, 32);
         this.lblFrameInfo.TabIndex = 2;
         this.lblFrameInfo.Text = "Load and Save frames between the editor and the Frameset:";
         // 
         // btnDeleteFrame
         // 
         this.btnDeleteFrame.Location = new System.Drawing.Point(128, 56);
         this.btnDeleteFrame.Name = "btnDeleteFrame";
         this.btnDeleteFrame.Size = new System.Drawing.Size(64, 24);
         this.btnDeleteFrame.TabIndex = 5;
         this.btnDeleteFrame.Text = "&Delete";
         this.ttFrameset.SetToolTip(this.btnDeleteFrame, "Delete the selected frame");
         this.btnDeleteFrame.Click += new System.EventHandler(this.btnDeleteFrame_Click);
         // 
         // btnSaveFrame
         // 
         this.btnSaveFrame.Location = new System.Drawing.Point(64, 56);
         this.btnSaveFrame.Name = "btnSaveFrame";
         this.btnSaveFrame.Size = new System.Drawing.Size(64, 24);
         this.btnSaveFrame.TabIndex = 4;
         this.btnSaveFrame.Text = "&Save >>";
         this.ttFrameset.SetToolTip(this.btnSaveFrame, "Save the frame from the editor over the selected frame");
         this.btnSaveFrame.Click += new System.EventHandler(this.btnSaveFrame_Click);
         // 
         // btnLoadFrame
         // 
         this.btnLoadFrame.Location = new System.Drawing.Point(0, 56);
         this.btnLoadFrame.Name = "btnLoadFrame";
         this.btnLoadFrame.Size = new System.Drawing.Size(64, 24);
         this.btnLoadFrame.TabIndex = 3;
         this.btnLoadFrame.Text = "<< &Load";
         this.ttFrameset.SetToolTip(this.btnLoadFrame, "Load the selected frame into the editor");
         this.btnLoadFrame.Click += new System.EventHandler(this.btnLoadFrame_Click);
         // 
         // txtFramesetName
         // 
         this.txtFramesetName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtFramesetName.Location = new System.Drawing.Point(112, 0);
         this.txtFramesetName.Name = "txtFramesetName";
         this.txtFramesetName.Size = new System.Drawing.Size(80, 20);
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
         // sbFrame
         // 
         this.sbFrame.Location = new System.Drawing.Point(0, 401);
         this.sbFrame.Name = "sbFrame";
         this.sbFrame.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
                                                                                   this.sbpFrameIndex,
                                                                                   this.sbpCellIndex});
         this.sbFrame.ShowPanels = true;
         this.sbFrame.Size = new System.Drawing.Size(192, 22);
         this.sbFrame.SizingGrip = false;
         this.sbFrame.TabIndex = 7;
         this.sbFrame.Visible = false;
         this.sbFrame.PanelClick += new System.Windows.Forms.StatusBarPanelClickEventHandler(this.sbFrame_PanelClick);
         // 
         // sbpFrameIndex
         // 
         this.sbpFrameIndex.Icon = ((System.Drawing.Icon)(resources.GetObject("sbpFrameIndex.Icon")));
         this.sbpFrameIndex.Width = 75;
         // 
         // sbpCellIndex
         // 
         this.sbpCellIndex.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
         this.sbpCellIndex.Icon = ((System.Drawing.Icon)(resources.GetObject("sbpCellIndex.Icon")));
         this.sbpCellIndex.ToolTipText = "Graphic Sheet and Cell Index (click for wizard)";
         this.sbpCellIndex.Width = 117;
         // 
         // pnlTransform
         // 
         this.pnlTransform.AllowDrop = true;
         this.pnlTransform.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.pnlTransform.ContextMenu = this.mnuContext;
         this.pnlTransform.Controls.Add(this.txtAlpha);
         this.pnlTransform.Controls.Add(this.lblAlpha);
         this.pnlTransform.Controls.Add(this.txtBlue);
         this.pnlTransform.Controls.Add(this.lblBlue);
         this.pnlTransform.Controls.Add(this.txtGreen);
         this.pnlTransform.Controls.Add(this.lblGreen);
         this.pnlTransform.Controls.Add(this.txtRed);
         this.pnlTransform.Controls.Add(this.lblRed);
         this.pnlTransform.Controls.Add(this.lblColorAdjust);
         this.pnlTransform.Controls.Add(this.txtDY);
         this.pnlTransform.Controls.Add(this.txtDX);
         this.pnlTransform.Controls.Add(this.txtM22);
         this.pnlTransform.Controls.Add(this.txtM21);
         this.pnlTransform.Controls.Add(this.txtM12);
         this.pnlTransform.Controls.Add(this.txtM11);
         this.pnlTransform.Controls.Add(this.lblMatrix);
         this.pnlTransform.Controls.Add(this.nudYScale);
         this.pnlTransform.Controls.Add(this.nudXScale);
         this.pnlTransform.Controls.Add(this.btnMore);
         this.pnlTransform.Controls.Add(this.nudXOffset);
         this.pnlTransform.Controls.Add(this.nudYOffset);
         this.pnlTransform.Controls.Add(this.lblXOffset);
         this.pnlTransform.Controls.Add(this.btnApply);
         this.pnlTransform.Controls.Add(this.btnReset);
         this.pnlTransform.Controls.Add(this.lblYOffset);
         this.pnlTransform.Controls.Add(this.lblYScale);
         this.pnlTransform.Controls.Add(this.lblXScale);
         this.pnlTransform.Controls.Add(this.chkRotateAroundCenter);
         this.pnlTransform.Controls.Add(this.lblRotate);
         this.pnlTransform.Controls.Add(this.txtRotate);
         this.pnlTransform.Controls.Add(this.trbRotate);
         this.pnlTransform.Dock = System.Windows.Forms.DockStyle.Left;
         this.pnlTransform.Location = new System.Drawing.Point(0, 0);
         this.pnlTransform.Name = "pnlTransform";
         this.pnlTransform.Size = new System.Drawing.Size(272, 423);
         this.pnlTransform.TabIndex = 0;
         this.pnlTransform.Resize += new System.EventHandler(this.pnlTransform_Resize);
         this.pnlTransform.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlTransform_MouseUp);
         this.pnlTransform.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlTransform_Paint);
         this.pnlTransform.DragDrop += new System.Windows.Forms.DragEventHandler(this.pnlTransform_DragDrop);
         this.pnlTransform.DragOver += new System.Windows.Forms.DragEventHandler(this.pnlTransform_DragOver);
         this.pnlTransform.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlTransform_MouseMove);
         this.pnlTransform.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlTransform_MouseDown);
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
                                                                                   this.mnuCounter90});
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
         // txtAlpha
         // 
         this.txtAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.txtAlpha.Location = new System.Drawing.Point(224, 400);
         this.txtAlpha.Name = "txtAlpha";
         this.txtAlpha.Size = new System.Drawing.Size(32, 20);
         this.txtAlpha.TabIndex = 31;
         this.txtAlpha.Text = "";
         this.txtAlpha.TextChanged += new System.EventHandler(this.ColorControl_Changed);
         // 
         // lblAlpha
         // 
         this.lblAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblAlpha.Location = new System.Drawing.Point(200, 400);
         this.lblAlpha.Name = "lblAlpha";
         this.lblAlpha.Size = new System.Drawing.Size(24, 20);
         this.lblAlpha.TabIndex = 30;
         this.lblAlpha.Text = "A:";
         this.lblAlpha.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // txtBlue
         // 
         this.txtBlue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.txtBlue.Location = new System.Drawing.Point(160, 400);
         this.txtBlue.Name = "txtBlue";
         this.txtBlue.Size = new System.Drawing.Size(32, 20);
         this.txtBlue.TabIndex = 29;
         this.txtBlue.Text = "";
         this.txtBlue.TextChanged += new System.EventHandler(this.ColorControl_Changed);
         // 
         // lblBlue
         // 
         this.lblBlue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblBlue.Location = new System.Drawing.Point(136, 400);
         this.lblBlue.Name = "lblBlue";
         this.lblBlue.Size = new System.Drawing.Size(24, 20);
         this.lblBlue.TabIndex = 28;
         this.lblBlue.Text = "B:";
         this.lblBlue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // txtGreen
         // 
         this.txtGreen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.txtGreen.Location = new System.Drawing.Point(96, 400);
         this.txtGreen.Name = "txtGreen";
         this.txtGreen.Size = new System.Drawing.Size(32, 20);
         this.txtGreen.TabIndex = 27;
         this.txtGreen.Text = "";
         this.txtGreen.TextChanged += new System.EventHandler(this.ColorControl_Changed);
         // 
         // lblGreen
         // 
         this.lblGreen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblGreen.Location = new System.Drawing.Point(72, 400);
         this.lblGreen.Name = "lblGreen";
         this.lblGreen.Size = new System.Drawing.Size(24, 20);
         this.lblGreen.TabIndex = 26;
         this.lblGreen.Text = "G:";
         this.lblGreen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // txtRed
         // 
         this.txtRed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.txtRed.Location = new System.Drawing.Point(32, 400);
         this.txtRed.Name = "txtRed";
         this.txtRed.Size = new System.Drawing.Size(32, 20);
         this.txtRed.TabIndex = 24;
         this.txtRed.Text = "";
         this.txtRed.TextChanged += new System.EventHandler(this.ColorControl_Changed);
         // 
         // lblRed
         // 
         this.lblRed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblRed.Location = new System.Drawing.Point(8, 400);
         this.lblRed.Name = "lblRed";
         this.lblRed.Size = new System.Drawing.Size(24, 20);
         this.lblRed.TabIndex = 23;
         this.lblRed.Text = "R:";
         this.lblRed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // lblColorAdjust
         // 
         this.lblColorAdjust.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblColorAdjust.Location = new System.Drawing.Point(8, 384);
         this.lblColorAdjust.Name = "lblColorAdjust";
         this.lblColorAdjust.Size = new System.Drawing.Size(264, 16);
         this.lblColorAdjust.TabIndex = 22;
         this.lblColorAdjust.Text = "Color adjustment (0=eliminate, 255=retain 100%):";
         // 
         // txtDY
         // 
         this.txtDY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.txtDY.Location = new System.Drawing.Point(184, 360);
         this.txtDY.Name = "txtDY";
         this.txtDY.Size = new System.Drawing.Size(56, 20);
         this.txtDY.TabIndex = 21;
         this.txtDY.Text = "";
         this.txtDY.TextChanged += new System.EventHandler(this.MatrixText_Changed);
         // 
         // txtDX
         // 
         this.txtDX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.txtDX.Location = new System.Drawing.Point(184, 336);
         this.txtDX.Name = "txtDX";
         this.txtDX.Size = new System.Drawing.Size(56, 20);
         this.txtDX.TabIndex = 18;
         this.txtDX.Text = "";
         this.txtDX.TextChanged += new System.EventHandler(this.MatrixText_Changed);
         // 
         // txtM22
         // 
         this.txtM22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.txtM22.Location = new System.Drawing.Point(120, 360);
         this.txtM22.Name = "txtM22";
         this.txtM22.Size = new System.Drawing.Size(56, 20);
         this.txtM22.TabIndex = 20;
         this.txtM22.Text = "";
         this.txtM22.TextChanged += new System.EventHandler(this.MatrixText_Changed);
         // 
         // txtM21
         // 
         this.txtM21.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.txtM21.Location = new System.Drawing.Point(56, 360);
         this.txtM21.Name = "txtM21";
         this.txtM21.Size = new System.Drawing.Size(56, 20);
         this.txtM21.TabIndex = 19;
         this.txtM21.Text = "";
         this.txtM21.TextChanged += new System.EventHandler(this.MatrixText_Changed);
         // 
         // txtM12
         // 
         this.txtM12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.txtM12.Location = new System.Drawing.Point(120, 336);
         this.txtM12.Name = "txtM12";
         this.txtM12.Size = new System.Drawing.Size(56, 20);
         this.txtM12.TabIndex = 17;
         this.txtM12.Text = "";
         this.txtM12.TextChanged += new System.EventHandler(this.MatrixText_Changed);
         // 
         // txtM11
         // 
         this.txtM11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.txtM11.Location = new System.Drawing.Point(56, 336);
         this.txtM11.Name = "txtM11";
         this.txtM11.Size = new System.Drawing.Size(56, 20);
         this.txtM11.TabIndex = 16;
         this.txtM11.Text = "";
         this.txtM11.TextChanged += new System.EventHandler(this.MatrixText_Changed);
         // 
         // lblMatrix
         // 
         this.lblMatrix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblMatrix.Location = new System.Drawing.Point(8, 336);
         this.lblMatrix.Name = "lblMatrix";
         this.lblMatrix.Size = new System.Drawing.Size(48, 32);
         this.lblMatrix.TabIndex = 15;
         this.lblMatrix.Text = "Matrix:";
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
         this.nudYScale.Location = new System.Drawing.Point(184, 256);
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
         this.nudYScale.Size = new System.Drawing.Size(56, 20);
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
         this.nudXScale.Location = new System.Drawing.Point(64, 256);
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
         this.nudXScale.Size = new System.Drawing.Size(56, 20);
         this.nudXScale.TabIndex = 5;
         this.nudXScale.Value = new System.Decimal(new int[] {
                                                                1,
                                                                0,
                                                                0,
                                                                0});
         this.nudXScale.Validated += new System.EventHandler(this.TransformControl_Validated);
         this.nudXScale.ValueChanged += new System.EventHandler(this.TransformControl_ValueChanged);
         // 
         // btnMore
         // 
         this.btnMore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.btnMore.Location = new System.Drawing.Point(168, 304);
         this.btnMore.Name = "btnMore";
         this.btnMore.Size = new System.Drawing.Size(72, 24);
         this.btnMore.TabIndex = 14;
         this.btnMore.Text = "More >>";
         this.btnMore.Click += new System.EventHandler(this.btnMore_Click);
         // 
         // nudXOffset
         // 
         this.nudXOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.nudXOffset.DecimalPlaces = 1;
         this.nudXOffset.Location = new System.Drawing.Point(64, 280);
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
         this.nudXOffset.Size = new System.Drawing.Size(56, 20);
         this.nudXOffset.TabIndex = 9;
         this.nudXOffset.Validated += new System.EventHandler(this.TransformControl_Validated);
         this.nudXOffset.ValueChanged += new System.EventHandler(this.TransformControl_ValueChanged);
         // 
         // nudYOffset
         // 
         this.nudYOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.nudYOffset.DecimalPlaces = 1;
         this.nudYOffset.Location = new System.Drawing.Point(184, 280);
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
         this.nudYOffset.Size = new System.Drawing.Size(56, 20);
         this.nudYOffset.TabIndex = 11;
         this.nudYOffset.Validated += new System.EventHandler(this.TransformControl_Validated);
         this.nudYOffset.ValueChanged += new System.EventHandler(this.TransformControl_ValueChanged);
         // 
         // lblXOffset
         // 
         this.lblXOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblXOffset.Location = new System.Drawing.Point(8, 280);
         this.lblXOffset.Name = "lblXOffset";
         this.lblXOffset.Size = new System.Drawing.Size(56, 20);
         this.lblXOffset.TabIndex = 8;
         this.lblXOffset.Text = "X Offset:";
         this.lblXOffset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // btnApply
         // 
         this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.btnApply.Location = new System.Drawing.Point(88, 304);
         this.btnApply.Name = "btnApply";
         this.btnApply.Size = new System.Drawing.Size(72, 24);
         this.btnApply.TabIndex = 13;
         this.btnApply.Text = "Apply";
         this.ttFrameset.SetToolTip(this.btnApply, "Apply the current transformation to the graphic");
         this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
         // 
         // btnReset
         // 
         this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.btnReset.Location = new System.Drawing.Point(8, 304);
         this.btnReset.Name = "btnReset";
         this.btnReset.Size = new System.Drawing.Size(72, 24);
         this.btnReset.TabIndex = 12;
         this.btnReset.Text = "Reset";
         this.ttFrameset.SetToolTip(this.btnReset, "Reset the current frame to initial state (untransformed graphic cell)");
         this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
         // 
         // lblYOffset
         // 
         this.lblYOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblYOffset.Location = new System.Drawing.Point(128, 280);
         this.lblYOffset.Name = "lblYOffset";
         this.lblYOffset.Size = new System.Drawing.Size(56, 20);
         this.lblYOffset.TabIndex = 10;
         this.lblYOffset.Text = "Y Offset:";
         this.lblYOffset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblYScale
         // 
         this.lblYScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblYScale.Location = new System.Drawing.Point(128, 256);
         this.lblYScale.Name = "lblYScale";
         this.lblYScale.Size = new System.Drawing.Size(56, 20);
         this.lblYScale.TabIndex = 6;
         this.lblYScale.Text = "Y Scale:";
         this.lblYScale.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // lblXScale
         // 
         this.lblXScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblXScale.Location = new System.Drawing.Point(8, 256);
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
         this.chkRotateAroundCenter.Location = new System.Drawing.Point(128, 232);
         this.chkRotateAroundCenter.Name = "chkRotateAroundCenter";
         this.chkRotateAroundCenter.Size = new System.Drawing.Size(112, 16);
         this.chkRotateAroundCenter.TabIndex = 3;
         this.chkRotateAroundCenter.Text = "Around center";
         this.chkRotateAroundCenter.CheckedChanged += new System.EventHandler(this.TransformControl_ValueChanged);
         // 
         // lblRotate
         // 
         this.lblRotate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblRotate.Location = new System.Drawing.Point(8, 232);
         this.lblRotate.Name = "lblRotate";
         this.lblRotate.Size = new System.Drawing.Size(56, 20);
         this.lblRotate.TabIndex = 1;
         this.lblRotate.Text = "Rotate:";
         this.lblRotate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtRotate
         // 
         this.txtRotate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.txtRotate.Location = new System.Drawing.Point(64, 232);
         this.txtRotate.Name = "txtRotate";
         this.txtRotate.Size = new System.Drawing.Size(56, 20);
         this.txtRotate.TabIndex = 2;
         this.txtRotate.Text = "";
         this.txtRotate.TextChanged += new System.EventHandler(this.txtRotate_TextChanged);
         // 
         // trbRotate
         // 
         this.trbRotate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.trbRotate.LargeChange = 15;
         this.trbRotate.Location = new System.Drawing.Point(0, 192);
         this.trbRotate.Maximum = 180;
         this.trbRotate.Minimum = -179;
         this.trbRotate.Name = "trbRotate";
         this.trbRotate.Size = new System.Drawing.Size(270, 34);
         this.trbRotate.TabIndex = 0;
         this.trbRotate.TickFrequency = 15;
         this.trbRotate.Scroll += new System.EventHandler(this.trbRotate_Scroll);
         // 
         // pnlGraphicSheet
         // 
         this.pnlGraphicSheet.Controls.Add(this.btnLoadGCell);
         this.pnlGraphicSheet.Controls.Add(this.label1);
         this.pnlGraphicSheet.Controls.Add(this.btnAddFrame);
         this.pnlGraphicSheet.Controls.Add(this.cboGraphicSheet);
         this.pnlGraphicSheet.Controls.Add(this.CellBrowser);
         this.pnlGraphicSheet.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.pnlGraphicSheet.Location = new System.Drawing.Point(0, 429);
         this.pnlGraphicSheet.Name = "pnlGraphicSheet";
         this.pnlGraphicSheet.Size = new System.Drawing.Size(464, 120);
         this.pnlGraphicSheet.TabIndex = 2;
         // 
         // btnLoadGCell
         // 
         this.btnLoadGCell.Location = new System.Drawing.Point(0, 0);
         this.btnLoadGCell.Name = "btnLoadGCell";
         this.btnLoadGCell.Size = new System.Drawing.Size(64, 24);
         this.btnLoadGCell.TabIndex = 4;
         this.btnLoadGCell.Text = "^ Load ^";
         this.ttFrameset.SetToolTip(this.btnLoadGCell, "Load the selected cell into the transformer");
         this.btnLoadGCell.Click += new System.EventHandler(this.btnLoadGCell_Click);
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(88, 0);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(80, 21);
         this.label1.TabIndex = 3;
         this.label1.Text = "Graphic Sheet:";
         this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // btnAddFrame
         // 
         this.btnAddFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnAddFrame.Location = new System.Drawing.Point(392, 0);
         this.btnAddFrame.Name = "btnAddFrame";
         this.btnAddFrame.Size = new System.Drawing.Size(72, 24);
         this.btnAddFrame.TabIndex = 1;
         this.btnAddFrame.Text = "^ Add ^";
         this.ttFrameset.SetToolTip(this.btnAddFrame, "Add new frame(s) to the Frameset based on the selected graphic cell(s)");
         this.btnAddFrame.Click += new System.EventHandler(this.btnAddFrame_Click);
         // 
         // cboGraphicSheet
         // 
         this.cboGraphicSheet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cboGraphicSheet.DisplayMember = "Name";
         this.cboGraphicSheet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboGraphicSheet.Location = new System.Drawing.Point(168, 0);
         this.cboGraphicSheet.Name = "cboGraphicSheet";
         this.cboGraphicSheet.Size = new System.Drawing.Size(200, 21);
         this.cboGraphicSheet.TabIndex = 0;
         this.ttFrameset.SetToolTip(this.cboGraphicSheet, "Select a graphic sheet to use as a source for frame graphics");
         this.cboGraphicSheet.ValueMember = "Name";
         this.cboGraphicSheet.SelectedIndexChanged += new System.EventHandler(this.cboGraphicSheet_SelectedIndexChanged);
         // 
         // CellBrowser
         // 
         this.CellBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.CellBrowser.BorderStyle = SGDK2.DragPanelBorderStyle.FixedInset;
         this.CellBrowser.CellPadding = new System.Drawing.Size(3, 3);
         this.CellBrowser.CellSize = new System.Drawing.Size(0, 0);
         this.CellBrowser.CurrentCellIndex = -1;
         this.CellBrowser.Frameset = null;
         this.CellBrowser.FramesToDisplay = null;
         this.CellBrowser.GraphicSheet = null;
         this.CellBrowser.Location = new System.Drawing.Point(0, 24);
         this.CellBrowser.Name = "CellBrowser";
         this.CellBrowser.SheetImage = null;
         this.CellBrowser.Size = new System.Drawing.Size(464, 96);
         this.CellBrowser.TabIndex = 2;
         // 
         // splitterGraphicSheet
         // 
         this.splitterGraphicSheet.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.splitterGraphicSheet.Location = new System.Drawing.Point(0, 423);
         this.splitterGraphicSheet.Name = "splitterGraphicSheet";
         this.splitterGraphicSheet.Size = new System.Drawing.Size(464, 6);
         this.splitterGraphicSheet.TabIndex = 23;
         this.splitterGraphicSheet.TabStop = false;
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
                                                                                       this.mnuLoadCell,
                                                                                       this.mnuAddCell,
                                                                                       this.mnuDeleteFrames,
                                                                                       this.mnuFramesetSeparator,
                                                                                       this.mnuFrameRemappingWizard});
         this.mnuFramesetPop.MergeOrder = 2;
         this.mnuFramesetPop.Text = "F&rameset";
         // 
         // mnuDeleteFrames
         // 
         this.mnuDeleteFrames.Index = 2;
         this.mnuDeleteFrames.Shortcut = System.Windows.Forms.Shortcut.Del;
         this.mnuDeleteFrames.Text = "&Delete Selected Frames";
         this.mnuDeleteFrames.Click += new System.EventHandler(this.mnuDeleteFrames_Click);
         // 
         // mnuFrameRemappingWizard
         // 
         this.mnuFrameRemappingWizard.Index = 4;
         this.mnuFrameRemappingWizard.Text = "&Frame Remapping Wizard...";
         this.mnuFrameRemappingWizard.Click += new System.EventHandler(this.mnuFrameRemappingWizard_Click);
         // 
         // mnuLoadCell
         // 
         this.mnuLoadCell.Index = 0;
         this.mnuLoadCell.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
         this.mnuLoadCell.Text = "&Load Selected Cell into Editor";
         this.mnuLoadCell.Click += new System.EventHandler(this.btnLoadGCell_Click);
         // 
         // mnuAddCell
         // 
         this.mnuAddCell.Index = 1;
         this.mnuAddCell.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
         this.mnuAddCell.Text = "&Add Selected Cell to Frameset";
         this.mnuAddCell.Click += new System.EventHandler(this.btnAddFrame_Click);
         // 
         // mnuFramesetSeparator
         // 
         this.mnuFramesetSeparator.Index = 3;
         this.mnuFramesetSeparator.Text = "-";
         // 
         // frmFrameEdit
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(464, 549);
         this.Controls.Add(this.pnlFrameSet);
         this.Controls.Add(this.splitterGraphicSheet);
         this.Controls.Add(this.pnlGraphicSheet);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Menu = this.mnuFrameset;
         this.Name = "frmFrameEdit";
         this.Text = "Frameset Editor";
         this.pnlFrameSet.ResumeLayout(false);
         this.pnlFrames.ResumeLayout(false);
         this.pnlFrameAction.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.sbpFrameIndex)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpCellIndex)).EndInit();
         this.pnlTransform.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.nudYScale)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudXScale)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudXOffset)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.nudYOffset)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.trbRotate)).EndInit();
         this.pnlGraphicSheet.ResumeLayout(false);
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
         Rectangle rcBounds = Rectangle.Round(GetTransformedBounds(CurTransform));
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
         Rectangle rcBounds = Rectangle.Round(GetTransformedBounds(Transform));
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

      private RectangleF GetTransformedBounds(Matrix Transform)
      {
         PointF[] arptCorners = new PointF[]
         {
            new PointF(0f,0f), new PointF((float)m_CurrentImage.Width, 0f),
            new PointF((float)m_CurrentImage.Width, (float)m_CurrentImage.Height),
            new PointF(0f, (float)m_CurrentImage.Height)
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
         return GetTransformedBounds(m_CurrentTransform);
      }

      private PointF GetTransformedCenter()
      {
         RectangleF rcTransformed = GetTransformedBounds();
         return new PointF(
            rcTransformed.X + rcTransformed.Width / 2f,
            rcTransformed.Y + rcTransformed.Height / 2f);
      }

      private Boolean ParseDouble(string s, out double result)
      {
         return Double.TryParse(s, System.Globalization.NumberStyles.Number, System.Globalization.NumberFormatInfo.CurrentInfo, out result);
      }

      private Boolean ParseByte(string s, out byte result)
      {
         double data;
         if (!Double.TryParse(s, System.Globalization.NumberStyles.AllowLeadingWhite | System.Globalization.NumberStyles.AllowTrailingWhite, System.Globalization.CultureInfo.InvariantCulture, out data))
         {
            result = 255;
            return false;
         }
         if (data>255)
         {
            result = 255;
            return false;
         }
         result = (byte)data;
         return true;
      }

      private Matrix GetTransform()
      {
         Double dblParse;
         Matrix Result = new Matrix();

         if (ParseDouble(txtRotate.Text, out dblParse))
         {
            if (chkRotateAroundCenter.Checked)
               Result.RotateAt((float)dblParse, GetTransformedCenter());
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

      private void LoadMatrix()
      {
         // make sure change event doesn't store matrix until everything is in
         txtDY.Text = "";
         txtM11.Text = m_CurrentTransform.Elements[0].ToString();
         txtM12.Text = m_CurrentTransform.Elements[1].ToString();
         txtM21.Text = m_CurrentTransform.Elements[2].ToString();
         txtM22.Text = m_CurrentTransform.Elements[3].ToString();
         txtDX.Text = m_CurrentTransform.Elements[4].ToString();
         txtDY.Text = m_CurrentTransform.Elements[5].ToString();
      }

      private void LoadColorAdjust()
      {
         byte[] clr = BitConverter.GetBytes(m_CurrentColor);
         txtRed.Text = clr[2].ToString();
         txtGreen.Text = clr[1].ToString();
         txtBlue.Text = clr[0].ToString();
         txtAlpha.Text = clr[3].ToString();
      }

      private bool StoreMatrix()
      {
         Double dblM11, dblM12, dblM21, dblM22, dblDX, dblDY;
         if ((ParseDouble(txtM11.Text, out dblM11)) &&
            (ParseDouble(txtM12.Text, out dblM12)) &&
            (ParseDouble(txtM21.Text, out dblM21)) &&
            (ParseDouble(txtM22.Text, out dblM22)) &&
            (ParseDouble(txtDX.Text, out dblDX)) &&
            (ParseDouble(txtDY.Text, out dblDY)))
         {
            if (m_CurrentTransform != null)
               m_CurrentTransform.Dispose();
            m_CurrentTransform = new Matrix(
               (float)dblM11, (float)dblM12,
               (float)dblM21, (float)dblM22,
               (float)dblDX, (float)dblDY);
            return true;
         }
         return false;
      }

      private bool StoreColorAdjust()
      {
         byte R, G, B, A;
         if (ParseByte(txtRed.Text, out R) &&
            ParseByte(txtGreen.Text, out G) &&
            ParseByte(txtBlue.Text, out B) &&
            ParseByte(txtAlpha.Text, out A))
         {
            m_CurrentColor = BitConverter.ToInt32(new byte[] {B,G,R,A}, 0);
            return true;
         }
         return false;
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
         m_sCurrentSheetName = fr.GraphicSheet;
         m_nCurrentCellIndex = fr.CellIndex;
         if (m_CurrentTransform != null)
            m_CurrentTransform.Dispose();
         m_CurrentTransform = new Matrix(fr.m11, fr.m12, fr.m21, fr.m22, fr.dx, fr.dy);
         m_CurrentColor = fr.color;
         ResetControls();
         LoadMatrix();
         LoadColorAdjust();
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
         else if (e.Data.GetData(typeof(Panel)) == pnlTransform)
            e.Effect = DragDropEffects.Copy;
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

            if (gb == null)
            {
               if (m_nCurrentCellIndex < 0)
                  return;

               ProjectData.InsertFrame(FrameBrowser.Frameset,
                  nFrameValue, m_sCurrentSheetName, m_nCurrentCellIndex,
                  m_CurrentTransform.Elements[0], m_CurrentTransform.Elements[1], m_CurrentTransform.Elements[2],
                  m_CurrentTransform.Elements[3], m_CurrentTransform.Elements[4], m_CurrentTransform.Elements[5], m_CurrentColor);
            }
            else
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

      private void btnLoadFrame_Click(object sender, System.EventArgs e)
      {
         int nSel = FrameBrowser.GetFirstSelectedCell();
         if (nSel >= 0)
            LoadFrameRow(FrameBrowser.GetSelectedFrames()[0]);
      }

      private void trbRotate_Scroll(object sender, System.EventArgs e)
      {
         txtRotate.Text = trbRotate.Value.ToString();
      }

      private void pnlTransform_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         if (m_CurrentImage == null)
            return;

         m_ptCenter = new Point(pnlTransform.ClientSize.Width / 2, trbRotate.Top / 2);
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
            Rectangle rcBounds = Rectangle.Round(GetTransformedBounds(PaintTransform));
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
         Point ptRight = new Point(trbRotate.ClientSize.Width - 1, m_ptCenter.Y);
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
         LoadMatrix();
         LoadColorAdjust();
      }

      private void btnReset_Click(object sender, System.EventArgs e)
      {
         if (m_CurrentTransform != null)
            m_CurrentTransform.Dispose();
         m_CurrentTransform = new Matrix();
         m_CurrentColor = -1;
         ResetControls();
         LoadMatrix();
         LoadColorAdjust();
         pnlTransform.Invalidate();
      }

      private void btnMore_Click(object sender, System.EventArgs e)
      {
         mnuContext.Show(pnlTransform, new Point(btnMore.Bounds.Right, btnMore.Bounds.Top));
      }

      private void mnuContextItem_Click(object sender, System.EventArgs e)
      {
         btnApply_Click(null, null);
         PointF ptCenter = GetTransformedCenter();

         if (sender == mnuClockwise90Center)
            m_CurrentTransform.RotateAt(90f, ptCenter, MatrixOrder.Append);
         else if (sender == mnuClockwise90Origin)
            m_CurrentTransform.Rotate(90f, MatrixOrder.Append);
         else if (sender == mnuCounter90Center)
            m_CurrentTransform.RotateAt(-90f, ptCenter, MatrixOrder.Append);
         else if (sender == mnuCounter90Origin)
            m_CurrentTransform.Rotate(-90f, MatrixOrder.Append);
         else if (sender == mnuDoubleHeight)
            m_CurrentTransform.Scale(1f, 2f, MatrixOrder.Append);
         else if (sender == mnuDoubleWidth)
            m_CurrentTransform.Scale(2f, 1f, MatrixOrder.Append);
         else if (sender == mnuDownHalf)
            m_CurrentTransform.Translate(0, GetTransformedBounds().Height / 2f, MatrixOrder.Append);
         else if (sender == mnuUpHalf)
            m_CurrentTransform.Translate(0, -GetTransformedBounds().Height / 2f, MatrixOrder.Append);
         else if (sender == mnuLeftHalf)
            m_CurrentTransform.Translate(-GetTransformedBounds().Width / 2f, 0, MatrixOrder.Append);
         else if (sender == mnuRightHalf)
            m_CurrentTransform.Translate(GetTransformedBounds().Width / 2f, 0, MatrixOrder.Append);
         else if (sender == mnuHalveHeight)
            m_CurrentTransform.Scale(1f, 0.5f, MatrixOrder.Append);
         else if (sender == mnuHalveWidth)
            m_CurrentTransform.Scale(0.5f, 1f, MatrixOrder.Append);
         else if (sender == mnuHFlipCenter)
         {
            m_CurrentTransform.Translate(-ptCenter.X, -ptCenter.Y, MatrixOrder.Append);
            m_CurrentTransform.Scale(-1f, 1f, MatrixOrder.Append);
            m_CurrentTransform.Translate(ptCenter.X, ptCenter.Y, MatrixOrder.Append);
         }
         else if (sender == mnuHFlipOrigin)
            m_CurrentTransform.Scale(-1f, 1f, MatrixOrder.Append);
         else if (sender == mnuVFlipCenter)
         {
            m_CurrentTransform.Translate(-ptCenter.X, -ptCenter.Y, MatrixOrder.Append);
            m_CurrentTransform.Scale(1f, -1f, MatrixOrder.Append);
            m_CurrentTransform.Translate(ptCenter.X, ptCenter.Y, MatrixOrder.Append);
         }
         else if (sender == mnuVFlipOrigin)
            m_CurrentTransform.Scale(1f, -1f, MatrixOrder.Append);
         else
            MessageBox.Show(this, "Not Implemented");

         RoundMatrix(ref m_CurrentTransform);
         LoadMatrix();
         pnlTransform.Invalidate();
      }

      private void TransformControl_ValueChanged(object sender, System.EventArgs e)
      {
         pnlTransform.Invalidate();      
      }

      private void TransformControl_Validated(object sender, System.EventArgs e)
      {
         decimal dummy = ((NumericUpDown)sender).Value;
      }

      private void MatrixText_Changed(object sender, System.EventArgs e)
      {
         StoreMatrix();
         ResetControls();
         pnlTransform.Invalidate();
      }

      private void ColorControl_Changed(object sender, System.EventArgs e)
      {
         StoreColorAdjust();
         pnlTransform.Invalidate();
      }

      private void btnSaveFrame_Click(object sender, System.EventArgs e)
      {
         ProjectDataset.FrameRow[] arfr = FrameBrowser.GetSelectedFrames();

         if ((arfr == null) || (arfr.Length == 0))
            return;

         btnApply_Click(null, null);
         arfr[0].CellIndex = m_nCurrentCellIndex;
         arfr[0].GraphicSheet = m_sCurrentSheetName;
         arfr[0].m11 = m_CurrentTransform.Elements[0];
         arfr[0].m12 = m_CurrentTransform.Elements[1];
         arfr[0].m21 = m_CurrentTransform.Elements[2];
         arfr[0].m22 = m_CurrentTransform.Elements[3];
         arfr[0].dx = m_CurrentTransform.Elements[4];
         arfr[0].dy = m_CurrentTransform.Elements[5];
         arfr[0].color = m_CurrentColor;
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
         if (m_DragType == MouseZone.Inside)
         {
            m_DragType = MouseZone.Outside;
            btnApply_Click(null, null);
            DoDragDrop(pnlTransform, DragDropEffects.Copy);
         }
         else
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
                  Cursor.Current = Cursors.SizeAll;
                  break;
            }

            if (bTempMode)
               m_DragType = MouseZone.Outside;
            else
            {
               if (txtRotate.Text != "0")
                  btnApply_Click(null, null);
               RectangleF rcOriginal = GetTransformedBounds();
               Matrix CurTransform = m_CurrentTransform.Clone();
               Matrix CtlTransform = GetTransform();
               CurTransform.Multiply(CtlTransform, MatrixOrder.Append);
               CtlTransform.Dispose();
               RectangleF rcNew = GetTransformedBounds(CurTransform);
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
                     rcNew.Offset(ptMouse.X - m_DragStart.X, ptMouse.Y - m_DragStart.Y);
                     m_DragStart = ptMouse;
                     break;
               }
               
               XScale = (decimal)(rcNew.Width / rcOriginal.Width);
               YScale = (decimal)(rcNew.Height / rcOriginal.Height);
               CurTransform = m_CurrentTransform.Clone();
               CurTransform.Scale((float)XScale, (float)YScale, MatrixOrder.Append);
               rcOriginal = GetTransformedBounds(CurTransform);
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
      }

      private void pnlTransform_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
      {
         if (e.Data.GetData(typeof(GraphicBrowser)) != null)
            e.Effect = DragDropEffects.Copy;
      }

      private void pnlTransform_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
      {
         GraphicBrowser gb = ((GraphicBrowser)e.Data.GetData(typeof(GraphicBrowser)));
         int nSel = gb.GetFirstSelectedCell();
         if (nSel < 0)
            return;

         if (gb.GraphicSheet == null)
            LoadFrameRow(gb.GetSelectedFrames()[0]);
         else
         {
            if (m_CurrentImage != null)
               m_CurrentImage.Dispose();
            m_CurrentImage = gb.GetCellImageData(nSel);
            m_sCurrentSheetName = gb.GraphicSheet.Name;
            m_nCurrentCellIndex = (short)nSel;
            if (m_CurrentTransform != null)
               m_CurrentTransform.Dispose();
            m_CurrentTransform = new Matrix();
            ResetControls();
            LoadMatrix();
            pnlTransform.Invalidate();
         }
      }

      private void btnAddFrame_Click(object sender, System.EventArgs e)
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

      private void btnLoadGCell_Click(object sender, System.EventArgs e)
      {
         short nSel = (short)CellBrowser.GetFirstSelectedCell();
         if (nSel < 0)
            return;

         if (m_CurrentImage != null)
            m_CurrentImage.Dispose();
         m_CurrentImage = CellBrowser.GetCellImageData(nSel);
         m_sCurrentSheetName = CellBrowser.GraphicSheet.Name;
         m_nCurrentCellIndex = nSel;
         if (m_CurrentTransform != null)
            m_CurrentTransform.Dispose();
         m_CurrentTransform = new Matrix();
         ResetControls();
         LoadMatrix();
         pnlTransform.Invalidate();
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

      private void btnDeleteFrame_Click(object sender, System.EventArgs e)
      {
         DeleteSelectedFrames();
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
         if (sbFrame.Visible = (FrameBrowser.GetSelectedCellCount() == 1))
         {
            ProjectDataset.FrameRow row = ProjectData.GetFrame(FrameBrowser.Frameset.Name, FrameBrowser.GetFirstSelectedCell());
            if (row == null)
            {
               sbFrame.Visible = false;
               return;
            }
            sbpFrameIndex.Text = "#" + row.FrameValue.ToString();
            sbpCellIndex.Text = "#" + row.CellIndex.ToString() + " (" + row.GraphicSheet + ")";
         }
      }

      private void sbFrame_PanelClick(object sender, System.Windows.Forms.StatusBarPanelClickEventArgs e)
      {
         LaunchFrameRemappingWizard();
      }

      private void mnuFrameRemappingWizard_Click(object sender, System.EventArgs e)
      {
         LaunchFrameRemappingWizard();
      }

      private void mnuDeleteFrames_Click(object sender, System.EventArgs e)
      {
         DeleteSelectedFrames();
      }
      #endregion
   }
}
