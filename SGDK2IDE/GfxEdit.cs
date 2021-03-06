/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SGDK2
{
   /// <summary>
	/// Summary description for GfxEdit.
	/// </summary>
   public class frmGraphicsEditor : System.Windows.Forms.Form
   {
      #region Embedded types
      struct ToolInfo
      {
         public ToolInfo(ToolBarButton newButton, MenuItem newMenu, DrawingTool newTool)
         {
            this.Button = newButton;
            this.Menu = newMenu;
            this.Tool = newTool;
         }
         public ToolBarButton Button;
         public MenuItem Menu;
         public DrawingTool Tool;
      };

      struct CustomToolInfo
      {
         public CustomToolInfo(MenuItem newMenu, MenuItem newContextItem, int newIndex, int newImageIndex)
         {
            this.Menu = newMenu;
            this.ContextMenu = newContextItem;
            this.ToolIndex = newIndex;
            this.ImageIndex = newImageIndex;
         }
         public MenuItem Menu;
         public MenuItem ContextMenu;
         public int ToolIndex;
         public int ImageIndex;
      }

      struct PenInfo
      {
         public PenInfo(ToolBarButton newButton, MenuItem newMenu, int nPenSize)
         {
            this.Button = newButton;
            this.Menu = newMenu;
            this.PenSize = nPenSize;
         }
         public ToolBarButton Button;
         public MenuItem Menu;
         public int PenSize;
      };

      struct SelHighlightInfo
      {
         public SelHighlightInfo(MenuItem newMenu, Color clrHighlight)
         {
            Menu = newMenu;
            Highlight = clrHighlight;
         }
         public MenuItem Menu;
         public Color Highlight;
      }

      struct BackdropInfo
      {
         public BackdropInfo(MenuItem newMenu, Color clrForeColor, Color clrBackColor, HatchStyle hsStyle, Boolean bHatch)
         {
            Menu = newMenu;
            ForeColor = clrForeColor;
            BackColor = clrBackColor;
            Style = hsStyle;
            IsHatched = bHatch;
         }
         public MenuItem Menu;
         public Color ForeColor;
         public Color BackColor;
         public HatchStyle Style;
         public Boolean IsHatched;
      }

      public class frmTilePreview : Form
      {
         MenuItem mnuIncreaseTileZoom;
         MenuItem mnuDecreaseTileZoom;
         int ZoomFactor = 1;
         Brush bruBackground;
         Bitmap bmpBackground;

         public frmTilePreview(Brush bruBackground, Bitmap bmpBackground)
         {
            this.Text = "Preview Tiling (1x)";
            this.bruBackground = bruBackground;
            this.bmpBackground = bmpBackground;
            GenerateImage();
            mnuIncreaseTileZoom = new MenuItem("&Increase Zoom", mnuTileZoom_Click);
            mnuDecreaseTileZoom = new MenuItem("&Decrease Zoom", mnuTileZoom_Click);
            this.Menu = new MainMenu(new MenuItem[] { mnuIncreaseTileZoom, mnuDecreaseTileZoom });
         }

         private void mnuTileZoom_Click(object sender, EventArgs e)
         {
            if ((sender == mnuIncreaseTileZoom) && (ZoomFactor < 16))
            {
               ZoomFactor *= 2;
            }
            else if ((sender == mnuDecreaseTileZoom) && (ZoomFactor > 1))
            {
               ZoomFactor /= 2;
            }
            GenerateImage();
            Text = string.Format("Preview Tiling ({0}x)", ZoomFactor);
         }

         private void GenerateImage()
         {
            Bitmap bmpBack = new Bitmap(bmpBackground.Width * ZoomFactor,
               bmpBackground.Height * ZoomFactor, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bmpBack);
            g.FillRectangle(bruBackground, 0, 0, bmpBack.Width, bmpBack.Height);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.DrawImage(bmpBackground, 0, 0, bmpBack.Width, bmpBack.Height);
            g.Dispose();
            if (this.BackgroundImage != null)
            {
               Image old = this.BackgroundImage;
               BackgroundImage = bmpBack;
               old.Dispose();
            }
            else
               BackgroundImage = bmpBack;
         }

         protected override void Dispose( bool disposing )
         {
            if( disposing )
               this.BackgroundImage.Dispose();
            this.BackgroundImage = null;
            base.Dispose(disposing);
         }
      }
      #endregion

      #region Non-control members
      private frmGraphicPane m_frmMagnify = null;
      private frmGraphicPane m_frmActual = null;
      private frmCellMgr m_frmCells = null;
      public Bitmap m_imgCurrentGraphic = null;
      public ProjectDataset.GraphicSheetRow m_DataSource = null;
      private ToolInfo[] m_arTools = null;
      private PenInfo[] m_arPens = null;
      private ArrayList m_CustomTools = new ArrayList();
      public Pen CurrentPen = new Pen(Color.Black);
      public Brush CurrentBrush = new SolidBrush(Color.Black);
      public CompositingMode CompositingMode = CompositingMode.SourceOver;
      public DrawingTool CurrentTool = DrawingTool.FreeDraw;
      public ToolOptions CurrentOptions = ToolOptions.AntiAlias | ToolOptions.Outline;
      private Bitmap[] m_arimgUndoStates = new Bitmap[10];
      public Matrix SelectionTransform = null;
      public PointF[] FreehandPoints = null;
      public PointF[] SelectionOutline = null;
      public Region SelectedRegion = null;
      public Bitmap FloatingSelection = null;
      public Matrix TempTransform = null;
      public Boolean IsDragging = false;
      public Brush HighlightBrush = new SolidBrush(Color.FromArgb(128, 0,0,255));
      private SelHighlightInfo[] m_arSelHighlightInfo = null;
      private BackdropInfo[] m_arBackDropInfo = null;
      public Brush Backdrop = new HatchBrush(HatchStyle.SolidDiamond, Color.Gray, Color.White);
      public int m_CustomTool = 0;
      #endregion

      #region Form Designer Members
      private System.ComponentModel.IContainer components;
      private System.Windows.Forms.ToolBar tbrGraphicsEditor;
      private System.Windows.Forms.ToolBarButton tbbLine;
      private System.Windows.Forms.ImageList imlGraphicsEditor;
      private System.Windows.Forms.MainMenu mnuGraphicsEditor;
      private System.Windows.Forms.MenuItem mnuTools;
      private System.Windows.Forms.MenuItem mnuToolLine;
      private System.Windows.Forms.MenuItem mnuToolAntiAlias;
      private System.Windows.Forms.MenuItem mnuFile;
      private System.Windows.Forms.MenuItem mnuExportGraphic;
      private System.Windows.Forms.ToolBarButton tbsSep1;
      private System.Windows.Forms.MenuItem mnuPixelPen;
      private System.Windows.Forms.MenuItem mnuTinyPen;
      private System.Windows.Forms.MenuItem mnuSmallPen;
      private System.Windows.Forms.MenuItem mnuMediumPen;
      private System.Windows.Forms.MenuItem mnuLargePen;
      private System.Windows.Forms.MenuItem mnu64Pen;
      private System.Windows.Forms.MenuItem mnuRoundPen;
      private System.Windows.Forms.MenuItem mnuSquarePen;
      private System.Windows.Forms.MenuItem mnuToolSep1;
      private System.Windows.Forms.MenuItem mnuToolSep2;
      private System.Windows.Forms.MenuItem mnuPen;
      private System.Windows.Forms.ContextMenu mnuCZoom;
      private System.Windows.Forms.MenuItem mnuActualSize;
      private System.Windows.Forms.MenuItem mnuZoom2;
      private System.Windows.Forms.MenuItem mnuZoom4;
      private System.Windows.Forms.MenuItem mnuZoom6;
      private System.Windows.Forms.MenuItem mnuZoom8;
      private MenuItem mnuZoom16;
      private System.Windows.Forms.ToolBarButton tbbBezier;
      private System.Windows.Forms.MenuItem mnuToolFreeDraw;
      private System.Windows.Forms.MenuItem mnuToolFreeLine;
      private System.Windows.Forms.MenuItem mnuToolBezier;
      private System.Windows.Forms.ToolBarButton tbbFreeLine;
      private System.Windows.Forms.ToolBarButton tbbFreeDraw;
      private System.Windows.Forms.ToolBarButton tbbRect;
      private System.Windows.Forms.ToolBarButton tbbEllipse;
      private System.Windows.Forms.MenuItem mnuToolRectangle;
      private System.Windows.Forms.MenuItem mnuToolEllipse;
      private System.Windows.Forms.MenuItem mnuToolFill;
      private System.Windows.Forms.MenuItem mnuToolOutline;
      private System.Windows.Forms.ToolBarButton tbbEraser;
      private System.Windows.Forms.MenuItem mnuToolErase;
      private System.Windows.Forms.MenuItem mnuEdit;
      private System.Windows.Forms.MenuItem mnuEditUndo;
      private System.Windows.Forms.MenuItem mnuEditRedo;
      private System.Windows.Forms.ToolBarButton tbbSelRect;
      private System.Windows.Forms.ToolBarButton tbbSelFree;
      private System.Windows.Forms.MenuItem mnuToolSelRect;
      private System.Windows.Forms.MenuItem mnuToolSelFree;
      private System.Windows.Forms.MenuItem mnuToolTranslate;
      private System.Windows.Forms.MenuItem mnuToolRotate;
      private System.Windows.Forms.MenuItem mnuToolScale;
      private System.Windows.Forms.ToolBarButton tbbTranslate;
      private System.Windows.Forms.ToolBarButton tbbRotate;
      private System.Windows.Forms.ToolBarButton tbbScale;
      private System.Windows.Forms.MenuItem mnuEditSep1;
      private System.Windows.Forms.MenuItem mnuEditResetPos;
      private System.Windows.Forms.MenuItem mnuEditDeselect;
      private System.Windows.Forms.MenuItem mnuToolLock;
      private System.Windows.Forms.MenuItem mnuEditSep2;
      private System.Windows.Forms.MenuItem mnuEditDelete;
      private System.Windows.Forms.MenuItem mnuEditCopy;
      private System.Windows.Forms.MenuItem mnuEditCut;
      private System.Windows.Forms.MenuItem mnuEditPaste;
      private System.Windows.Forms.MenuItem mnuEditSelectionHighlight;
      private System.Windows.Forms.MenuItem mnuEditSelBlue;
      private System.Windows.Forms.MenuItem mnuEditSelWhite;
      private System.Windows.Forms.MenuItem mnuEditSelBlack;
      private System.Windows.Forms.MenuItem mnuEditSelRed;
      private System.Windows.Forms.MenuItem mnuEditSelGreen;
      private System.Windows.Forms.MenuItem mnuEditSelBlueF;
      private System.Windows.Forms.MenuItem mnuEditSelGreenF;
      private System.Windows.Forms.MenuItem mnuEditSelRedF;
      private System.Windows.Forms.MenuItem mnuEditShowHighlight;
      private System.Windows.Forms.MenuItem mnuEditSelWhiteF;
      private System.Windows.Forms.MenuItem mnuEditSelBlackF;
      private System.Windows.Forms.MenuItem mnuEditHighlightSep1;
      private System.Windows.Forms.MenuItem mnuEditBackdrop;
      private System.Windows.Forms.MenuItem mnuEditBackWhiteDiamond;
      private System.Windows.Forms.MenuItem mnuEditBackBlack;
      private System.Windows.Forms.MenuItem mnuEditBackWhite;
      private System.Windows.Forms.MenuItem mnuEditBackGray;
      private System.Windows.Forms.MenuItem mnuEditBackWhiteCross;
      private System.Windows.Forms.MenuItem mnuEditBackBlackCross;
      private System.Windows.Forms.MenuItem mnuEditBackBlackDiamond;
      private System.Windows.Forms.ToolBarButton tbbDropper;
      private System.Windows.Forms.MenuItem mnuToolDropper;
      private System.Windows.Forms.ToolBarButton tbbFloodFill;
      private System.Windows.Forms.ToolBarButton tbbFloodSel;
      private System.Windows.Forms.MenuItem mnuToolFloodFill;
      private System.Windows.Forms.MenuItem mnuToolFloodSel;
      private System.Windows.Forms.ToolBarButton tbbGradientFill;
      private System.Windows.Forms.MenuItem mnuToolGradientFill;
      private System.Windows.Forms.MenuItem mnuEditSep4;
      private System.Windows.Forms.ToolBarButton tbbAirbrush;
      private System.Windows.Forms.ToolBarButton tbbHFlip;
      private System.Windows.Forms.ToolBarButton tbbVFlip;
      private System.Windows.Forms.MenuItem mnuEditHFlip;
      private System.Windows.Forms.MenuItem mnuEditVFlip;
      private System.Windows.Forms.MenuItem mnuEditSep3;
      private System.Windows.Forms.MenuItem mnuToolAirbrush;
      private System.Windows.Forms.MenuItem mnuEditSelectAll;
      private System.Windows.Forms.MenuItem mnuToolGradientFills;
      private System.Windows.Forms.ToolBarButton tbbHOffset;
      private System.Windows.Forms.ToolBarButton tbbVOffset;
      private System.Windows.Forms.MenuItem mnuEditHOffset;
      private System.Windows.Forms.MenuItem mnuEditVOffset;
      private System.Windows.Forms.ToolBarButton tbbTilePreview;
      private System.Windows.Forms.MenuItem mnuEditTilePreview;
      private System.Windows.Forms.MenuItem mnuEditShowGrid;
      private System.Windows.Forms.MenuItem mnuEditSep5;
      private System.Windows.Forms.MenuItem mnuToolSmooth;
      private System.Windows.Forms.ToolBarButton tbbSmooth;
      private System.Windows.Forms.ToolBarButton tbdCustomTool;
      private System.Windows.Forms.MenuItem mnuToolCustom;
      private System.Windows.Forms.ContextMenu mnuCCustomTool;
      private System.Windows.Forms.ToolBar tbrOptions;
      private System.Windows.Forms.ToolBarButton tbbAntiAlias;
      private System.Windows.Forms.ToolBarButton tbbOutline;
      private System.Windows.Forms.ToolBarButton tbbFill;
      private System.Windows.Forms.ToolBarButton tbbGradientFills;
      private System.Windows.Forms.ToolBarButton tbbLock;
      private System.Windows.Forms.ToolBarButton tbsOptSep1;
      private System.Windows.Forms.Splitter ToolSplitter;
      private System.Windows.Forms.ToolBarButton tbbPixelPen;
      private System.Windows.Forms.ToolBarButton tbbTinyPen;
      private System.Windows.Forms.ToolBarButton tbbSmallPen;
      private System.Windows.Forms.ToolBarButton tbbMediumPen;
      private System.Windows.Forms.ToolBarButton tbbLargePen;
      private System.Windows.Forms.ToolBarButton tbbHugePen;
      private System.Windows.Forms.ToolBarButton tbb64Pen;
      private System.Windows.Forms.ToolBarButton tbsOptSep2;
      private System.Windows.Forms.ToolBarButton tbbRound;
      private System.Windows.Forms.ToolBarButton tbbSquare;
      private System.Windows.Forms.ToolBarButton tbdZoom;
      private System.Windows.Forms.ToolBarButton tbsOptSep3;
      private System.Windows.Forms.ToolBarButton tbbShowGrid;
      public SGDK2.ColorSel ctlColorSel;
      private System.Windows.Forms.MenuItem mnuImportGraphic;
      private System.Windows.Forms.MenuItem mnuView;
      private System.Windows.Forms.MenuItem mnuZoom;
      private System.Windows.Forms.MenuItem mnuZoom1x1;
      private System.Windows.Forms.MenuItem mnuZoom2x2;
      private System.Windows.Forms.MenuItem mnuZoom4x4;
      private System.Windows.Forms.MenuItem mnuZoom8x8;
      private System.Windows.Forms.MenuItem mnuZoom6x6;
      private MenuItem mnuZoom16x16;
      private System.Windows.Forms.MenuItem mnuFileSep;
      private System.Windows.Forms.MenuItem mnuDisjointedColors;
      private System.Windows.Forms.ToolBarButton tbbDisjointedColors;
      private SGDK2.DataChangeNotifier dataMonitor;
      private System.Windows.Forms.ToolBarButton tbbHueMap;
      private System.Windows.Forms.ToolBarButton tbbNoise;
      private System.Windows.Forms.MenuItem mnuHueMap;
      private System.Windows.Forms.MenuItem mnuNoise;
      private System.Windows.Forms.MenuItem mnuZoomIncrease;
      private System.Windows.Forms.MenuItem mnuZoomDecrease;
      private System.Windows.Forms.MenuItem mnuZoomSeparator;
      private System.Windows.Forms.MenuItem mnuPenSep2;
      private System.Windows.Forms.MenuItem mnuPenIncrease;
      private System.Windows.Forms.MenuItem mnuPenDecrease;
      private System.Windows.Forms.MenuItem mnuPenSep1;
      private System.Windows.Forms.MenuItem mnuExportSheet;
      private System.Windows.Forms.MenuItem mnuImportSheet;
      private System.Windows.Forms.MenuItem mnuHugePen;
      #endregion

      #region Initialization and Clean-up
      private frmGraphicsEditor()
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         m_arTools = new ToolInfo[]
            {
               new ToolInfo(tbbFreeDraw, mnuToolFreeDraw, DrawingTool.FreeDraw),
               new ToolInfo(tbbFreeLine, mnuToolFreeLine, DrawingTool.FreeLine),
               new ToolInfo(tbbBezier, mnuToolBezier, DrawingTool.Bezier),
               new ToolInfo(tbbLine, mnuToolLine, DrawingTool.Line),
               new ToolInfo(tbbRect, mnuToolRectangle, DrawingTool.Rectangle),
               new ToolInfo(tbbEllipse, mnuToolEllipse, DrawingTool.Ellipse),
               new ToolInfo(tbbEraser, mnuToolErase, DrawingTool.Erase),
               new ToolInfo(tbbSelRect, mnuToolSelRect, DrawingTool.SelRect),
               new ToolInfo(tbbSelFree, mnuToolSelFree, DrawingTool.SelFree),
               new ToolInfo(tbbTranslate, mnuToolTranslate, DrawingTool.Translate),
               new ToolInfo(tbbRotate, mnuToolRotate, DrawingTool.Rotate),
               new ToolInfo(tbbScale, mnuToolScale, DrawingTool.Scale),
               new ToolInfo(tbbDropper, mnuToolDropper, DrawingTool.Dropper),
               new ToolInfo(tbbFloodFill, mnuToolFloodFill, DrawingTool.FloodFill),
               new ToolInfo(tbbFloodSel, mnuToolFloodSel, DrawingTool.FloodSel),
               new ToolInfo(tbbGradientFill, mnuToolGradientFill, DrawingTool.GradientFill),
               new ToolInfo(tbbAirbrush, mnuToolAirbrush, DrawingTool.AirBrush),
               new ToolInfo(tbbSmooth, mnuToolSmooth, DrawingTool.Smooth)
            };
         m_arPens = new PenInfo[] 
            {
               new PenInfo(tbbPixelPen, mnuPixelPen, 1),
               new PenInfo(tbbTinyPen, mnuTinyPen, 3),
               new PenInfo(tbbSmallPen, mnuSmallPen, 5),
               new PenInfo(tbbMediumPen, mnuMediumPen, 7),
               new PenInfo(tbbLargePen, mnuLargePen, 9),
               new PenInfo(tbbHugePen, mnuHugePen, 11),
               new PenInfo(tbb64Pen, mnu64Pen, 64)
            };
         m_arSelHighlightInfo = new SelHighlightInfo[]
            {
               new SelHighlightInfo(mnuEditSelRed, Color.FromArgb(128, Color.Red)),
               new SelHighlightInfo(mnuEditSelGreen, Color.FromArgb(128, Color.Green)),
               new SelHighlightInfo(mnuEditSelBlue, Color.FromArgb(128, Color.Blue)),
               new SelHighlightInfo(mnuEditSelWhite, Color.FromArgb(128, Color.White)),
               new SelHighlightInfo(mnuEditSelBlack, Color.FromArgb(128, Color.Black)),
               new SelHighlightInfo(mnuEditSelRedF, Color.FromArgb(32, Color.Red)),
               new SelHighlightInfo(mnuEditSelGreenF, Color.FromArgb(32, Color.Green)),
               new SelHighlightInfo(mnuEditSelBlueF, Color.FromArgb(32, Color.Blue)),
               new SelHighlightInfo(mnuEditSelWhiteF, Color.FromArgb(32, Color.White)),
               new SelHighlightInfo(mnuEditSelBlackF, Color.FromArgb(32, Color.Black))
            };
         m_arBackDropInfo = new BackdropInfo[]
            {
               new BackdropInfo(mnuEditBackWhiteDiamond, Color.Gray, Color.White, HatchStyle.SolidDiamond, true),
               new BackdropInfo(mnuEditBackBlackDiamond, Color.Gray, Color.Black, HatchStyle.SolidDiamond, true),
               new BackdropInfo(mnuEditBackBlack, Color.Black, Color.Black, HatchStyle.BackwardDiagonal, false),
               new BackdropInfo(mnuEditBackWhite, Color.White, Color.White, HatchStyle.BackwardDiagonal, false),
               new BackdropInfo(mnuEditBackGray, Color.Gray, Color.Gray, HatchStyle.BackwardDiagonal, false),
               new BackdropInfo(mnuEditBackWhiteCross, Color.LightGray, Color.White, HatchStyle.DiagonalCross, true),
               new BackdropInfo(mnuEditBackBlackCross, Color.DarkGray, Color.Black, HatchStyle.DiagonalCross, true)
            };
      }

      public frmGraphicsEditor(ProjectDataset.GraphicSheetRow drDataSource) : this()
      {
         m_DataSource = drDataSource;
         Text = "Graphics Editor - " + drDataSource.Name;
      }

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose( bool disposing )
      {
         if( disposing )
         {
            for (int i = 0; i < m_arimgUndoStates.Length; i++)
            {
               if (m_arimgUndoStates[i] != null)
               {
                  m_arimgUndoStates[i].Dispose();
                  m_arimgUndoStates[i] = null;
               }
            }
            if (SelectionTransform != null)
            {
               SelectionTransform.Dispose();
               SelectionTransform = null;
            }
            if (SelectedRegion != null)
            {
               SelectedRegion.Dispose();
               SelectedRegion = null;
            }
            if (TempTransform != null)
            {
               TempTransform.Dispose();
               TempTransform = null;
            }
            if (HighlightBrush != null)
            {
               HighlightBrush.Dispose();
               HighlightBrush = null;
            }
            if (Backdrop != null)
            {
               Backdrop.Dispose();
               Backdrop = null;
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGraphicsEditor));
         this.tbrGraphicsEditor = new System.Windows.Forms.ToolBar();
         this.tbbFreeDraw = new System.Windows.Forms.ToolBarButton();
         this.tbbFreeLine = new System.Windows.Forms.ToolBarButton();
         this.tbbBezier = new System.Windows.Forms.ToolBarButton();
         this.tbbLine = new System.Windows.Forms.ToolBarButton();
         this.tbbRect = new System.Windows.Forms.ToolBarButton();
         this.tbbEllipse = new System.Windows.Forms.ToolBarButton();
         this.tbbAirbrush = new System.Windows.Forms.ToolBarButton();
         this.tbbSmooth = new System.Windows.Forms.ToolBarButton();
         this.tbbEraser = new System.Windows.Forms.ToolBarButton();
         this.tbbGradientFill = new System.Windows.Forms.ToolBarButton();
         this.tbbFloodFill = new System.Windows.Forms.ToolBarButton();
         this.tbbFloodSel = new System.Windows.Forms.ToolBarButton();
         this.tbbSelRect = new System.Windows.Forms.ToolBarButton();
         this.tbbSelFree = new System.Windows.Forms.ToolBarButton();
         this.tbbTranslate = new System.Windows.Forms.ToolBarButton();
         this.tbbRotate = new System.Windows.Forms.ToolBarButton();
         this.tbbScale = new System.Windows.Forms.ToolBarButton();
         this.tbbDropper = new System.Windows.Forms.ToolBarButton();
         this.tbdCustomTool = new System.Windows.Forms.ToolBarButton();
         this.mnuCCustomTool = new System.Windows.Forms.ContextMenu();
         this.tbsSep1 = new System.Windows.Forms.ToolBarButton();
         this.tbbHFlip = new System.Windows.Forms.ToolBarButton();
         this.tbbVFlip = new System.Windows.Forms.ToolBarButton();
         this.tbbHOffset = new System.Windows.Forms.ToolBarButton();
         this.tbbVOffset = new System.Windows.Forms.ToolBarButton();
         this.tbbTilePreview = new System.Windows.Forms.ToolBarButton();
         this.tbbHueMap = new System.Windows.Forms.ToolBarButton();
         this.tbbNoise = new System.Windows.Forms.ToolBarButton();
         this.imlGraphicsEditor = new System.Windows.Forms.ImageList(this.components);
         this.mnuCZoom = new System.Windows.Forms.ContextMenu();
         this.mnuActualSize = new System.Windows.Forms.MenuItem();
         this.mnuZoom2 = new System.Windows.Forms.MenuItem();
         this.mnuZoom4 = new System.Windows.Forms.MenuItem();
         this.mnuZoom6 = new System.Windows.Forms.MenuItem();
         this.mnuZoom8 = new System.Windows.Forms.MenuItem();
         this.mnuZoom16 = new System.Windows.Forms.MenuItem();
         this.mnuGraphicsEditor = new System.Windows.Forms.MainMenu(this.components);
         this.mnuFile = new System.Windows.Forms.MenuItem();
         this.mnuFileSep = new System.Windows.Forms.MenuItem();
         this.mnuExportGraphic = new System.Windows.Forms.MenuItem();
         this.mnuImportGraphic = new System.Windows.Forms.MenuItem();
         this.mnuExportSheet = new System.Windows.Forms.MenuItem();
         this.mnuImportSheet = new System.Windows.Forms.MenuItem();
         this.mnuView = new System.Windows.Forms.MenuItem();
         this.mnuZoom = new System.Windows.Forms.MenuItem();
         this.mnuZoom1x1 = new System.Windows.Forms.MenuItem();
         this.mnuZoom2x2 = new System.Windows.Forms.MenuItem();
         this.mnuZoom4x4 = new System.Windows.Forms.MenuItem();
         this.mnuZoom6x6 = new System.Windows.Forms.MenuItem();
         this.mnuZoom8x8 = new System.Windows.Forms.MenuItem();
         this.mnuZoom16x16 = new System.Windows.Forms.MenuItem();
         this.mnuZoomSeparator = new System.Windows.Forms.MenuItem();
         this.mnuZoomDecrease = new System.Windows.Forms.MenuItem();
         this.mnuZoomIncrease = new System.Windows.Forms.MenuItem();
         this.mnuEdit = new System.Windows.Forms.MenuItem();
         this.mnuEditUndo = new System.Windows.Forms.MenuItem();
         this.mnuEditRedo = new System.Windows.Forms.MenuItem();
         this.mnuEditSep1 = new System.Windows.Forms.MenuItem();
         this.mnuEditCut = new System.Windows.Forms.MenuItem();
         this.mnuEditCopy = new System.Windows.Forms.MenuItem();
         this.mnuEditPaste = new System.Windows.Forms.MenuItem();
         this.mnuEditDelete = new System.Windows.Forms.MenuItem();
         this.mnuEditSep2 = new System.Windows.Forms.MenuItem();
         this.mnuEditResetPos = new System.Windows.Forms.MenuItem();
         this.mnuEditSelectAll = new System.Windows.Forms.MenuItem();
         this.mnuEditDeselect = new System.Windows.Forms.MenuItem();
         this.mnuEditSep3 = new System.Windows.Forms.MenuItem();
         this.mnuEditHFlip = new System.Windows.Forms.MenuItem();
         this.mnuEditVFlip = new System.Windows.Forms.MenuItem();
         this.mnuEditHOffset = new System.Windows.Forms.MenuItem();
         this.mnuEditVOffset = new System.Windows.Forms.MenuItem();
         this.mnuEditTilePreview = new System.Windows.Forms.MenuItem();
         this.mnuHueMap = new System.Windows.Forms.MenuItem();
         this.mnuNoise = new System.Windows.Forms.MenuItem();
         this.mnuEditSep4 = new System.Windows.Forms.MenuItem();
         this.mnuEditSelectionHighlight = new System.Windows.Forms.MenuItem();
         this.mnuEditShowHighlight = new System.Windows.Forms.MenuItem();
         this.mnuEditHighlightSep1 = new System.Windows.Forms.MenuItem();
         this.mnuEditSelRed = new System.Windows.Forms.MenuItem();
         this.mnuEditSelGreen = new System.Windows.Forms.MenuItem();
         this.mnuEditSelBlue = new System.Windows.Forms.MenuItem();
         this.mnuEditSelWhite = new System.Windows.Forms.MenuItem();
         this.mnuEditSelBlack = new System.Windows.Forms.MenuItem();
         this.mnuEditSelRedF = new System.Windows.Forms.MenuItem();
         this.mnuEditSelGreenF = new System.Windows.Forms.MenuItem();
         this.mnuEditSelBlueF = new System.Windows.Forms.MenuItem();
         this.mnuEditSelWhiteF = new System.Windows.Forms.MenuItem();
         this.mnuEditSelBlackF = new System.Windows.Forms.MenuItem();
         this.mnuEditBackdrop = new System.Windows.Forms.MenuItem();
         this.mnuEditBackWhiteDiamond = new System.Windows.Forms.MenuItem();
         this.mnuEditBackBlackDiamond = new System.Windows.Forms.MenuItem();
         this.mnuEditBackWhiteCross = new System.Windows.Forms.MenuItem();
         this.mnuEditBackBlackCross = new System.Windows.Forms.MenuItem();
         this.mnuEditBackWhite = new System.Windows.Forms.MenuItem();
         this.mnuEditBackBlack = new System.Windows.Forms.MenuItem();
         this.mnuEditBackGray = new System.Windows.Forms.MenuItem();
         this.mnuEditSep5 = new System.Windows.Forms.MenuItem();
         this.mnuEditShowGrid = new System.Windows.Forms.MenuItem();
         this.mnuTools = new System.Windows.Forms.MenuItem();
         this.mnuToolFreeDraw = new System.Windows.Forms.MenuItem();
         this.mnuToolFreeLine = new System.Windows.Forms.MenuItem();
         this.mnuToolBezier = new System.Windows.Forms.MenuItem();
         this.mnuToolLine = new System.Windows.Forms.MenuItem();
         this.mnuToolRectangle = new System.Windows.Forms.MenuItem();
         this.mnuToolEllipse = new System.Windows.Forms.MenuItem();
         this.mnuToolAirbrush = new System.Windows.Forms.MenuItem();
         this.mnuToolSmooth = new System.Windows.Forms.MenuItem();
         this.mnuToolErase = new System.Windows.Forms.MenuItem();
         this.mnuToolGradientFill = new System.Windows.Forms.MenuItem();
         this.mnuToolFloodFill = new System.Windows.Forms.MenuItem();
         this.mnuToolFloodSel = new System.Windows.Forms.MenuItem();
         this.mnuToolSelRect = new System.Windows.Forms.MenuItem();
         this.mnuToolSelFree = new System.Windows.Forms.MenuItem();
         this.mnuToolTranslate = new System.Windows.Forms.MenuItem();
         this.mnuToolRotate = new System.Windows.Forms.MenuItem();
         this.mnuToolScale = new System.Windows.Forms.MenuItem();
         this.mnuToolDropper = new System.Windows.Forms.MenuItem();
         this.mnuToolCustom = new System.Windows.Forms.MenuItem();
         this.mnuToolSep1 = new System.Windows.Forms.MenuItem();
         this.mnuToolAntiAlias = new System.Windows.Forms.MenuItem();
         this.mnuToolOutline = new System.Windows.Forms.MenuItem();
         this.mnuToolFill = new System.Windows.Forms.MenuItem();
         this.mnuToolGradientFills = new System.Windows.Forms.MenuItem();
         this.mnuToolLock = new System.Windows.Forms.MenuItem();
         this.mnuDisjointedColors = new System.Windows.Forms.MenuItem();
         this.mnuToolSep2 = new System.Windows.Forms.MenuItem();
         this.mnuPen = new System.Windows.Forms.MenuItem();
         this.mnuPixelPen = new System.Windows.Forms.MenuItem();
         this.mnuTinyPen = new System.Windows.Forms.MenuItem();
         this.mnuSmallPen = new System.Windows.Forms.MenuItem();
         this.mnuMediumPen = new System.Windows.Forms.MenuItem();
         this.mnuLargePen = new System.Windows.Forms.MenuItem();
         this.mnuHugePen = new System.Windows.Forms.MenuItem();
         this.mnu64Pen = new System.Windows.Forms.MenuItem();
         this.mnuPenSep1 = new System.Windows.Forms.MenuItem();
         this.mnuPenDecrease = new System.Windows.Forms.MenuItem();
         this.mnuPenIncrease = new System.Windows.Forms.MenuItem();
         this.mnuPenSep2 = new System.Windows.Forms.MenuItem();
         this.mnuRoundPen = new System.Windows.Forms.MenuItem();
         this.mnuSquarePen = new System.Windows.Forms.MenuItem();
         this.tbrOptions = new System.Windows.Forms.ToolBar();
         this.tbbAntiAlias = new System.Windows.Forms.ToolBarButton();
         this.tbbOutline = new System.Windows.Forms.ToolBarButton();
         this.tbbFill = new System.Windows.Forms.ToolBarButton();
         this.tbbGradientFills = new System.Windows.Forms.ToolBarButton();
         this.tbbLock = new System.Windows.Forms.ToolBarButton();
         this.tbbDisjointedColors = new System.Windows.Forms.ToolBarButton();
         this.tbsOptSep1 = new System.Windows.Forms.ToolBarButton();
         this.tbbPixelPen = new System.Windows.Forms.ToolBarButton();
         this.tbbTinyPen = new System.Windows.Forms.ToolBarButton();
         this.tbbSmallPen = new System.Windows.Forms.ToolBarButton();
         this.tbbMediumPen = new System.Windows.Forms.ToolBarButton();
         this.tbbLargePen = new System.Windows.Forms.ToolBarButton();
         this.tbbHugePen = new System.Windows.Forms.ToolBarButton();
         this.tbb64Pen = new System.Windows.Forms.ToolBarButton();
         this.tbsOptSep2 = new System.Windows.Forms.ToolBarButton();
         this.tbbRound = new System.Windows.Forms.ToolBarButton();
         this.tbbSquare = new System.Windows.Forms.ToolBarButton();
         this.tbsOptSep3 = new System.Windows.Forms.ToolBarButton();
         this.tbdZoom = new System.Windows.Forms.ToolBarButton();
         this.tbbShowGrid = new System.Windows.Forms.ToolBarButton();
         this.ToolSplitter = new System.Windows.Forms.Splitter();
         this.ctlColorSel = new SGDK2.ColorSel();
         this.dataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.SuspendLayout();
         // 
         // tbrGraphicsEditor
         // 
         this.tbrGraphicsEditor.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
         this.tbrGraphicsEditor.AutoSize = false;
         this.tbrGraphicsEditor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.tbrGraphicsEditor.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.tbbFreeDraw,
            this.tbbFreeLine,
            this.tbbBezier,
            this.tbbLine,
            this.tbbRect,
            this.tbbEllipse,
            this.tbbAirbrush,
            this.tbbSmooth,
            this.tbbEraser,
            this.tbbGradientFill,
            this.tbbFloodFill,
            this.tbbFloodSel,
            this.tbbSelRect,
            this.tbbSelFree,
            this.tbbTranslate,
            this.tbbRotate,
            this.tbbScale,
            this.tbbDropper,
            this.tbdCustomTool,
            this.tbsSep1,
            this.tbbHFlip,
            this.tbbVFlip,
            this.tbbHOffset,
            this.tbbVOffset,
            this.tbbTilePreview,
            this.tbbHueMap,
            this.tbbNoise});
         this.tbrGraphicsEditor.ButtonSize = new System.Drawing.Size(23, 22);
         this.tbrGraphicsEditor.Divider = false;
         this.tbrGraphicsEditor.Dock = System.Windows.Forms.DockStyle.Left;
         this.tbrGraphicsEditor.DropDownArrows = true;
         this.tbrGraphicsEditor.ImageList = this.imlGraphicsEditor;
         this.tbrGraphicsEditor.Location = new System.Drawing.Point(0, 0);
         this.tbrGraphicsEditor.Name = "tbrGraphicsEditor";
         this.tbrGraphicsEditor.ShowToolTips = true;
         this.tbrGraphicsEditor.Size = new System.Drawing.Size(48, 363);
         this.tbrGraphicsEditor.TabIndex = 0;
         this.tbrGraphicsEditor.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbrGraphicsEditor_ButtonClick);
         // 
         // tbbFreeDraw
         // 
         this.tbbFreeDraw.ImageIndex = 0;
         this.tbbFreeDraw.Name = "tbbFreeDraw";
         this.tbbFreeDraw.Pushed = true;
         this.tbbFreeDraw.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbFreeDraw.ToolTipText = "Paint brush (draw freehand strokes ended with a mouse button release)";
         // 
         // tbbFreeLine
         // 
         this.tbbFreeLine.ImageIndex = 1;
         this.tbbFreeLine.Name = "tbbFreeLine";
         this.tbbFreeLine.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbFreeLine.ToolTipText = "Draw connected freehand lines and curves ended with right mouse button";
         // 
         // tbbBezier
         // 
         this.tbbBezier.ImageIndex = 2;
         this.tbbBezier.Name = "tbbBezier";
         this.tbbBezier.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbBezier.ToolTipText = "Draw connected smooth curves ended with right mouse button";
         // 
         // tbbLine
         // 
         this.tbbLine.ImageIndex = 3;
         this.tbbLine.Name = "tbbLine";
         this.tbbLine.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbLine.Tag = "";
         this.tbbLine.ToolTipText = "Draw singular straight lines";
         // 
         // tbbRect
         // 
         this.tbbRect.ImageIndex = 4;
         this.tbbRect.Name = "tbbRect";
         this.tbbRect.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbRect.Tag = "";
         this.tbbRect.ToolTipText = "Draw hollow rectangles";
         // 
         // tbbEllipse
         // 
         this.tbbEllipse.ImageIndex = 5;
         this.tbbEllipse.Name = "tbbEllipse";
         this.tbbEllipse.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbEllipse.Tag = "";
         this.tbbEllipse.ToolTipText = "Draw hollow ellipses";
         // 
         // tbbAirbrush
         // 
         this.tbbAirbrush.ImageIndex = 37;
         this.tbbAirbrush.Name = "tbbAirbrush";
         this.tbbAirbrush.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbAirbrush.ToolTipText = "Airbrush";
         // 
         // tbbSmooth
         // 
         this.tbbSmooth.ImageIndex = 45;
         this.tbbSmooth.Name = "tbbSmooth";
         this.tbbSmooth.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbSmooth.ToolTipText = "Blur/smooth";
         // 
         // tbbEraser
         // 
         this.tbbEraser.ImageIndex = 26;
         this.tbbEraser.Name = "tbbEraser";
         this.tbbEraser.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbEraser.ToolTipText = "Erase areas to transparent color.";
         // 
         // tbbGradientFill
         // 
         this.tbbGradientFill.ImageIndex = 36;
         this.tbbGradientFill.Name = "tbbGradientFill";
         this.tbbGradientFill.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbGradientFill.ToolTipText = "Fill selected region with color gradient";
         // 
         // tbbFloodFill
         // 
         this.tbbFloodFill.ImageIndex = 7;
         this.tbbFloodFill.Name = "tbbFloodFill";
         this.tbbFloodFill.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbFloodFill.ToolTipText = "Flood fill regions of similar colors (drag over color range)";
         // 
         // tbbFloodSel
         // 
         this.tbbFloodSel.ImageIndex = 35;
         this.tbbFloodSel.Name = "tbbFloodSel";
         this.tbbFloodSel.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbFloodSel.ToolTipText = "Color Wand (select connected areas of similar color)";
         // 
         // tbbSelRect
         // 
         this.tbbSelRect.ImageIndex = 27;
         this.tbbSelRect.Name = "tbbSelRect";
         this.tbbSelRect.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbSelRect.ToolTipText = "Select rectangular regions";
         // 
         // tbbSelFree
         // 
         this.tbbSelFree.ImageIndex = 28;
         this.tbbSelFree.Name = "tbbSelFree";
         this.tbbSelFree.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbSelFree.ToolTipText = "Select irregular regions (terminate with right button)";
         // 
         // tbbTranslate
         // 
         this.tbbTranslate.ImageIndex = 29;
         this.tbbTranslate.Name = "tbbTranslate";
         this.tbbTranslate.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbTranslate.ToolTipText = "Move the currently selected portion of the graphic";
         // 
         // tbbRotate
         // 
         this.tbbRotate.ImageIndex = 30;
         this.tbbRotate.Name = "tbbRotate";
         this.tbbRotate.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbRotate.ToolTipText = "Freely rotate the selected portion of the graphic";
         // 
         // tbbScale
         // 
         this.tbbScale.ImageIndex = 31;
         this.tbbScale.Name = "tbbScale";
         this.tbbScale.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbScale.ToolTipText = "Scale the selected portion of the graphic to a new size";
         // 
         // tbbDropper
         // 
         this.tbbDropper.ImageIndex = 33;
         this.tbbDropper.Name = "tbbDropper";
         this.tbbDropper.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbDropper.ToolTipText = "Pick a color from the image";
         // 
         // tbdCustomTool
         // 
         this.tbdCustomTool.DropDownMenu = this.mnuCCustomTool;
         this.tbdCustomTool.Name = "tbdCustomTool";
         this.tbdCustomTool.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
         this.tbdCustomTool.ToolTipText = "Select custom tool";
         // 
         // tbsSep1
         // 
         this.tbsSep1.Name = "tbsSep1";
         this.tbsSep1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
         this.tbsSep1.Tag = "";
         // 
         // tbbHFlip
         // 
         this.tbbHFlip.ImageIndex = 38;
         this.tbbHFlip.Name = "tbbHFlip";
         this.tbbHFlip.ToolTipText = "Flip selection horizontally";
         // 
         // tbbVFlip
         // 
         this.tbbVFlip.ImageIndex = 39;
         this.tbbVFlip.Name = "tbbVFlip";
         this.tbbVFlip.ToolTipText = "Flip selection vertically";
         // 
         // tbbHOffset
         // 
         this.tbbHOffset.ImageIndex = 41;
         this.tbbHOffset.Name = "tbbHOffset";
         this.tbbHOffset.ToolTipText = "Offset and wrap image horizontally to edit seam";
         // 
         // tbbVOffset
         // 
         this.tbbVOffset.ImageIndex = 42;
         this.tbbVOffset.Name = "tbbVOffset";
         this.tbbVOffset.ToolTipText = "Offset and wrap image vertically to edit seam";
         // 
         // tbbTilePreview
         // 
         this.tbbTilePreview.ImageIndex = 43;
         this.tbbTilePreview.Name = "tbbTilePreview";
         this.tbbTilePreview.ToolTipText = "Preview how graphic will appear when tiled";
         // 
         // tbbHueMap
         // 
         this.tbbHueMap.ImageIndex = 47;
         this.tbbHueMap.Name = "tbbHueMap";
         this.tbbHueMap.ToolTipText = "Remap colors";
         // 
         // tbbNoise
         // 
         this.tbbNoise.ImageIndex = 48;
         this.tbbNoise.Name = "tbbNoise";
         this.tbbNoise.ToolTipText = "Add noise";
         // 
         // imlGraphicsEditor
         // 
         this.imlGraphicsEditor.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlGraphicsEditor.ImageStream")));
         this.imlGraphicsEditor.TransparentColor = System.Drawing.Color.Magenta;
         this.imlGraphicsEditor.Images.SetKeyName(0, "");
         this.imlGraphicsEditor.Images.SetKeyName(1, "");
         this.imlGraphicsEditor.Images.SetKeyName(2, "");
         this.imlGraphicsEditor.Images.SetKeyName(3, "");
         this.imlGraphicsEditor.Images.SetKeyName(4, "");
         this.imlGraphicsEditor.Images.SetKeyName(5, "");
         this.imlGraphicsEditor.Images.SetKeyName(6, "");
         this.imlGraphicsEditor.Images.SetKeyName(7, "");
         this.imlGraphicsEditor.Images.SetKeyName(8, "");
         this.imlGraphicsEditor.Images.SetKeyName(9, "");
         this.imlGraphicsEditor.Images.SetKeyName(10, "");
         this.imlGraphicsEditor.Images.SetKeyName(11, "");
         this.imlGraphicsEditor.Images.SetKeyName(12, "");
         this.imlGraphicsEditor.Images.SetKeyName(13, "");
         this.imlGraphicsEditor.Images.SetKeyName(14, "");
         this.imlGraphicsEditor.Images.SetKeyName(15, "");
         this.imlGraphicsEditor.Images.SetKeyName(16, "");
         this.imlGraphicsEditor.Images.SetKeyName(17, "");
         this.imlGraphicsEditor.Images.SetKeyName(18, "");
         this.imlGraphicsEditor.Images.SetKeyName(19, "");
         this.imlGraphicsEditor.Images.SetKeyName(20, "");
         this.imlGraphicsEditor.Images.SetKeyName(21, "");
         this.imlGraphicsEditor.Images.SetKeyName(22, "");
         this.imlGraphicsEditor.Images.SetKeyName(23, "");
         this.imlGraphicsEditor.Images.SetKeyName(24, "");
         this.imlGraphicsEditor.Images.SetKeyName(25, "");
         this.imlGraphicsEditor.Images.SetKeyName(26, "");
         this.imlGraphicsEditor.Images.SetKeyName(27, "");
         this.imlGraphicsEditor.Images.SetKeyName(28, "");
         this.imlGraphicsEditor.Images.SetKeyName(29, "");
         this.imlGraphicsEditor.Images.SetKeyName(30, "");
         this.imlGraphicsEditor.Images.SetKeyName(31, "");
         this.imlGraphicsEditor.Images.SetKeyName(32, "");
         this.imlGraphicsEditor.Images.SetKeyName(33, "");
         this.imlGraphicsEditor.Images.SetKeyName(34, "");
         this.imlGraphicsEditor.Images.SetKeyName(35, "");
         this.imlGraphicsEditor.Images.SetKeyName(36, "");
         this.imlGraphicsEditor.Images.SetKeyName(37, "");
         this.imlGraphicsEditor.Images.SetKeyName(38, "");
         this.imlGraphicsEditor.Images.SetKeyName(39, "");
         this.imlGraphicsEditor.Images.SetKeyName(40, "");
         this.imlGraphicsEditor.Images.SetKeyName(41, "");
         this.imlGraphicsEditor.Images.SetKeyName(42, "");
         this.imlGraphicsEditor.Images.SetKeyName(43, "");
         this.imlGraphicsEditor.Images.SetKeyName(44, "");
         this.imlGraphicsEditor.Images.SetKeyName(45, "");
         this.imlGraphicsEditor.Images.SetKeyName(46, "");
         this.imlGraphicsEditor.Images.SetKeyName(47, "");
         this.imlGraphicsEditor.Images.SetKeyName(48, "");
         this.imlGraphicsEditor.Images.SetKeyName(49, "16x16.bmp");
         // 
         // mnuCZoom
         // 
         this.mnuCZoom.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuActualSize,
            this.mnuZoom2,
            this.mnuZoom4,
            this.mnuZoom6,
            this.mnuZoom8,
            this.mnuZoom16});
         // 
         // mnuActualSize
         // 
         this.mnuActualSize.Index = 0;
         this.mnuActualSize.Text = "Actual Size";
         this.mnuActualSize.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuZoom2
         // 
         this.mnuZoom2.Index = 1;
         this.mnuZoom2.Text = "2x2 Zoom";
         this.mnuZoom2.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuZoom4
         // 
         this.mnuZoom4.Checked = true;
         this.mnuZoom4.Index = 2;
         this.mnuZoom4.Text = "4x4 Zoom";
         this.mnuZoom4.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuZoom6
         // 
         this.mnuZoom6.Index = 3;
         this.mnuZoom6.Text = "6x6 Zoom";
         this.mnuZoom6.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuZoom8
         // 
         this.mnuZoom8.Index = 4;
         this.mnuZoom8.Text = "8x8 Zoom";
         this.mnuZoom8.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuZoom16
         // 
         this.mnuZoom16.Index = 5;
         this.mnuZoom16.Text = "16x16 Zoom";
         this.mnuZoom16.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuGraphicsEditor
         // 
         this.mnuGraphicsEditor.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFile,
            this.mnuView,
            this.mnuEdit,
            this.mnuTools});
         // 
         // mnuFile
         // 
         this.mnuFile.Index = 0;
         this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFileSep,
            this.mnuExportGraphic,
            this.mnuImportGraphic,
            this.mnuExportSheet,
            this.mnuImportSheet});
         this.mnuFile.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
         this.mnuFile.Text = "&File";
         // 
         // mnuFileSep
         // 
         this.mnuFileSep.Index = 0;
         this.mnuFileSep.MergeOrder = 1;
         this.mnuFileSep.Text = "-";
         // 
         // mnuExportGraphic
         // 
         this.mnuExportGraphic.Index = 1;
         this.mnuExportGraphic.MergeOrder = 2;
         this.mnuExportGraphic.Text = "Export Grap&hic...";
         this.mnuExportGraphic.Click += new System.EventHandler(this.mnuExportGraphic_Click);
         // 
         // mnuImportGraphic
         // 
         this.mnuImportGraphic.Index = 2;
         this.mnuImportGraphic.MergeOrder = 2;
         this.mnuImportGraphic.Text = "Import Graphi&c...";
         this.mnuImportGraphic.Click += new System.EventHandler(this.mnuImportGraphic_Click);
         // 
         // mnuExportSheet
         // 
         this.mnuExportSheet.Index = 3;
         this.mnuExportSheet.MergeOrder = 2;
         this.mnuExportSheet.Text = "Export Sheet as Image...";
         this.mnuExportSheet.Click += new System.EventHandler(this.mnuExportSheet_Click);
         // 
         // mnuImportSheet
         // 
         this.mnuImportSheet.Index = 4;
         this.mnuImportSheet.MergeOrder = 2;
         this.mnuImportSheet.Text = "Import Image as Sheet...";
         this.mnuImportSheet.Click += new System.EventHandler(this.mnuImportSheet_Click);
         // 
         // mnuView
         // 
         this.mnuView.Index = 1;
         this.mnuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuZoom});
         this.mnuView.MergeOrder = 1;
         this.mnuView.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
         this.mnuView.Text = "&View";
         // 
         // mnuZoom
         // 
         this.mnuZoom.Index = 0;
         this.mnuZoom.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuZoom1x1,
            this.mnuZoom2x2,
            this.mnuZoom4x4,
            this.mnuZoom6x6,
            this.mnuZoom8x8,
            this.mnuZoom16x16,
            this.mnuZoomSeparator,
            this.mnuZoomDecrease,
            this.mnuZoomIncrease});
         this.mnuZoom.Text = "&Zoom";
         // 
         // mnuZoom1x1
         // 
         this.mnuZoom1x1.Index = 0;
         this.mnuZoom1x1.Text = "&Actual Size";
         this.mnuZoom1x1.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuZoom2x2
         // 
         this.mnuZoom2x2.Index = 1;
         this.mnuZoom2x2.Text = "&2x2 Magnification";
         this.mnuZoom2x2.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuZoom4x4
         // 
         this.mnuZoom4x4.Checked = true;
         this.mnuZoom4x4.Index = 2;
         this.mnuZoom4x4.Text = "&4x4 Magnification";
         this.mnuZoom4x4.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuZoom6x6
         // 
         this.mnuZoom6x6.Index = 3;
         this.mnuZoom6x6.Text = "&6x6 Magnification";
         this.mnuZoom6x6.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuZoom8x8
         // 
         this.mnuZoom8x8.Index = 4;
         this.mnuZoom8x8.Text = "&8x8 Magnification";
         this.mnuZoom8x8.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuZoom16x16
         // 
         this.mnuZoom16x16.Index = 5;
         this.mnuZoom16x16.Text = "16&x16 Magnification";
         this.mnuZoom16x16.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuZoomSeparator
         // 
         this.mnuZoomSeparator.Index = 6;
         this.mnuZoomSeparator.Text = "-";
         // 
         // mnuZoomDecrease
         // 
         this.mnuZoomDecrease.Index = 7;
         this.mnuZoomDecrease.Text = "&Decrease Magnification\t-";
         this.mnuZoomDecrease.Click += new System.EventHandler(this.mnuZoomDecrease_Click);
         // 
         // mnuZoomIncrease
         // 
         this.mnuZoomIncrease.Index = 8;
         this.mnuZoomIncrease.Text = "&Increase Magnification\t+";
         this.mnuZoomIncrease.Click += new System.EventHandler(this.mnuZoomIncrease_Click);
         // 
         // mnuEdit
         // 
         this.mnuEdit.Index = 2;
         this.mnuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuEditUndo,
            this.mnuEditRedo,
            this.mnuEditSep1,
            this.mnuEditCut,
            this.mnuEditCopy,
            this.mnuEditPaste,
            this.mnuEditDelete,
            this.mnuEditSep2,
            this.mnuEditResetPos,
            this.mnuEditSelectAll,
            this.mnuEditDeselect,
            this.mnuEditSep3,
            this.mnuEditHFlip,
            this.mnuEditVFlip,
            this.mnuEditHOffset,
            this.mnuEditVOffset,
            this.mnuEditTilePreview,
            this.mnuHueMap,
            this.mnuNoise,
            this.mnuEditSep4,
            this.mnuEditSelectionHighlight,
            this.mnuEditBackdrop,
            this.mnuEditSep5,
            this.mnuEditShowGrid});
         this.mnuEdit.MergeOrder = 2;
         this.mnuEdit.Text = "&Edit";
         // 
         // mnuEditUndo
         // 
         this.mnuEditUndo.Index = 0;
         this.mnuEditUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
         this.mnuEditUndo.Text = "&Undo";
         this.mnuEditUndo.Click += new System.EventHandler(this.mnuEditUndo_Click);
         // 
         // mnuEditRedo
         // 
         this.mnuEditRedo.Index = 1;
         this.mnuEditRedo.Shortcut = System.Windows.Forms.Shortcut.CtrlY;
         this.mnuEditRedo.Text = "&Redo";
         this.mnuEditRedo.Click += new System.EventHandler(this.mnuEditRedo_Click);
         // 
         // mnuEditSep1
         // 
         this.mnuEditSep1.Index = 2;
         this.mnuEditSep1.Text = "-";
         // 
         // mnuEditCut
         // 
         this.mnuEditCut.Index = 3;
         this.mnuEditCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
         this.mnuEditCut.Text = "Cu&t";
         this.mnuEditCut.Click += new System.EventHandler(this.mnuClipAction_Click);
         // 
         // mnuEditCopy
         // 
         this.mnuEditCopy.Index = 4;
         this.mnuEditCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
         this.mnuEditCopy.Text = "&Copy";
         this.mnuEditCopy.Click += new System.EventHandler(this.mnuClipAction_Click);
         // 
         // mnuEditPaste
         // 
         this.mnuEditPaste.Index = 5;
         this.mnuEditPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
         this.mnuEditPaste.Text = "&Paste";
         this.mnuEditPaste.Click += new System.EventHandler(this.mnuClipAction_Click);
         // 
         // mnuEditDelete
         // 
         this.mnuEditDelete.Index = 6;
         this.mnuEditDelete.Shortcut = System.Windows.Forms.Shortcut.Del;
         this.mnuEditDelete.Text = "&Delete Selection";
         this.mnuEditDelete.Click += new System.EventHandler(this.mnuClipAction_Click);
         // 
         // mnuEditSep2
         // 
         this.mnuEditSep2.Index = 7;
         this.mnuEditSep2.Text = "-";
         // 
         // mnuEditResetPos
         // 
         this.mnuEditResetPos.Index = 8;
         this.mnuEditResetPos.Text = "Re&set Position";
         this.mnuEditResetPos.Click += new System.EventHandler(this.mnuEditResetPos_Click);
         // 
         // mnuEditSelectAll
         // 
         this.mnuEditSelectAll.Index = 9;
         this.mnuEditSelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
         this.mnuEditSelectAll.Text = "Select &All";
         this.mnuEditSelectAll.Click += new System.EventHandler(this.mnuEditSelectAll_Click);
         // 
         // mnuEditDeselect
         // 
         this.mnuEditDeselect.Index = 10;
         this.mnuEditDeselect.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftA;
         this.mnuEditDeselect.Text = "D&eselect All";
         this.mnuEditDeselect.Click += new System.EventHandler(this.mnuEditDeselect_Click);
         // 
         // mnuEditSep3
         // 
         this.mnuEditSep3.Index = 11;
         this.mnuEditSep3.Text = "-";
         // 
         // mnuEditHFlip
         // 
         this.mnuEditHFlip.Index = 12;
         this.mnuEditHFlip.Text = "Flip &Horizontally";
         this.mnuEditHFlip.Click += new System.EventHandler(this.mnuEditFlip_Click);
         // 
         // mnuEditVFlip
         // 
         this.mnuEditVFlip.Index = 13;
         this.mnuEditVFlip.Text = "Flip &Vertically";
         this.mnuEditVFlip.Click += new System.EventHandler(this.mnuEditFlip_Click);
         // 
         // mnuEditHOffset
         // 
         this.mnuEditHOffset.Index = 14;
         this.mnuEditHOffset.Text = "Hori&zontal Offset && Wrap";
         this.mnuEditHOffset.Click += new System.EventHandler(this.mnuEditOffset_Click);
         // 
         // mnuEditVOffset
         // 
         this.mnuEditVOffset.Index = 15;
         this.mnuEditVOffset.Text = "Vertical &Offset && Wrap";
         this.mnuEditVOffset.Click += new System.EventHandler(this.mnuEditOffset_Click);
         // 
         // mnuEditTilePreview
         // 
         this.mnuEditTilePreview.Index = 16;
         this.mnuEditTilePreview.Text = "Previe&w Tiling";
         this.mnuEditTilePreview.Click += new System.EventHandler(this.mnuEditTilePreview_Click);
         // 
         // mnuHueMap
         // 
         this.mnuHueMap.Index = 17;
         this.mnuHueMap.Text = "Re&map Hues";
         this.mnuHueMap.Click += new System.EventHandler(this.mnuHueMap_Click);
         // 
         // mnuNoise
         // 
         this.mnuNoise.Index = 18;
         this.mnuNoise.Text = "Add &Noise";
         this.mnuNoise.Click += new System.EventHandler(this.mnuNoise_Click);
         // 
         // mnuEditSep4
         // 
         this.mnuEditSep4.Index = 19;
         this.mnuEditSep4.Text = "-";
         // 
         // mnuEditSelectionHighlight
         // 
         this.mnuEditSelectionHighlight.Index = 20;
         this.mnuEditSelectionHighlight.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuEditShowHighlight,
            this.mnuEditHighlightSep1,
            this.mnuEditSelRed,
            this.mnuEditSelGreen,
            this.mnuEditSelBlue,
            this.mnuEditSelWhite,
            this.mnuEditSelBlack,
            this.mnuEditSelRedF,
            this.mnuEditSelGreenF,
            this.mnuEditSelBlueF,
            this.mnuEditSelWhiteF,
            this.mnuEditSelBlackF});
         this.mnuEditSelectionHighlight.Text = "Selection Highlight";
         // 
         // mnuEditShowHighlight
         // 
         this.mnuEditShowHighlight.Checked = true;
         this.mnuEditShowHighlight.Index = 0;
         this.mnuEditShowHighlight.Shortcut = System.Windows.Forms.Shortcut.CtrlT;
         this.mnuEditShowHighlight.Text = "&Show Highlight";
         this.mnuEditShowHighlight.Click += new System.EventHandler(this.mnuEditSelColor_Click);
         // 
         // mnuEditHighlightSep1
         // 
         this.mnuEditHighlightSep1.Index = 1;
         this.mnuEditHighlightSep1.Text = "-";
         // 
         // mnuEditSelRed
         // 
         this.mnuEditSelRed.Index = 2;
         this.mnuEditSelRed.RadioCheck = true;
         this.mnuEditSelRed.Text = "Red";
         this.mnuEditSelRed.Click += new System.EventHandler(this.mnuEditSelColor_Click);
         // 
         // mnuEditSelGreen
         // 
         this.mnuEditSelGreen.Index = 3;
         this.mnuEditSelGreen.RadioCheck = true;
         this.mnuEditSelGreen.Text = "Green";
         this.mnuEditSelGreen.Click += new System.EventHandler(this.mnuEditSelColor_Click);
         // 
         // mnuEditSelBlue
         // 
         this.mnuEditSelBlue.Checked = true;
         this.mnuEditSelBlue.Index = 4;
         this.mnuEditSelBlue.RadioCheck = true;
         this.mnuEditSelBlue.Text = "Blue";
         this.mnuEditSelBlue.Click += new System.EventHandler(this.mnuEditSelColor_Click);
         // 
         // mnuEditSelWhite
         // 
         this.mnuEditSelWhite.Index = 5;
         this.mnuEditSelWhite.RadioCheck = true;
         this.mnuEditSelWhite.Text = "White";
         this.mnuEditSelWhite.Click += new System.EventHandler(this.mnuEditSelColor_Click);
         // 
         // mnuEditSelBlack
         // 
         this.mnuEditSelBlack.Index = 6;
         this.mnuEditSelBlack.RadioCheck = true;
         this.mnuEditSelBlack.Text = "Black";
         this.mnuEditSelBlack.Click += new System.EventHandler(this.mnuEditSelColor_Click);
         // 
         // mnuEditSelRedF
         // 
         this.mnuEditSelRedF.Index = 7;
         this.mnuEditSelRedF.RadioCheck = true;
         this.mnuEditSelRedF.Text = "Faint Red";
         this.mnuEditSelRedF.Click += new System.EventHandler(this.mnuEditSelColor_Click);
         // 
         // mnuEditSelGreenF
         // 
         this.mnuEditSelGreenF.Index = 8;
         this.mnuEditSelGreenF.RadioCheck = true;
         this.mnuEditSelGreenF.Text = "Faint Green";
         this.mnuEditSelGreenF.Click += new System.EventHandler(this.mnuEditSelColor_Click);
         // 
         // mnuEditSelBlueF
         // 
         this.mnuEditSelBlueF.Index = 9;
         this.mnuEditSelBlueF.RadioCheck = true;
         this.mnuEditSelBlueF.Text = "Faint Blue";
         this.mnuEditSelBlueF.Click += new System.EventHandler(this.mnuEditSelColor_Click);
         // 
         // mnuEditSelWhiteF
         // 
         this.mnuEditSelWhiteF.Index = 10;
         this.mnuEditSelWhiteF.RadioCheck = true;
         this.mnuEditSelWhiteF.Text = "Faint White";
         this.mnuEditSelWhiteF.Click += new System.EventHandler(this.mnuEditSelColor_Click);
         // 
         // mnuEditSelBlackF
         // 
         this.mnuEditSelBlackF.Index = 11;
         this.mnuEditSelBlackF.RadioCheck = true;
         this.mnuEditSelBlackF.Text = "Faint Black";
         this.mnuEditSelBlackF.Click += new System.EventHandler(this.mnuEditSelColor_Click);
         // 
         // mnuEditBackdrop
         // 
         this.mnuEditBackdrop.Index = 21;
         this.mnuEditBackdrop.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuEditBackWhiteDiamond,
            this.mnuEditBackBlackDiamond,
            this.mnuEditBackWhiteCross,
            this.mnuEditBackBlackCross,
            this.mnuEditBackWhite,
            this.mnuEditBackBlack,
            this.mnuEditBackGray});
         this.mnuEditBackdrop.Text = "Backdrop";
         // 
         // mnuEditBackWhiteDiamond
         // 
         this.mnuEditBackWhiteDiamond.Checked = true;
         this.mnuEditBackWhiteDiamond.Index = 0;
         this.mnuEditBackWhiteDiamond.RadioCheck = true;
         this.mnuEditBackWhiteDiamond.Shortcut = System.Windows.Forms.Shortcut.Ctrl1;
         this.mnuEditBackWhiteDiamond.Text = "Gray and white diamonds";
         this.mnuEditBackWhiteDiamond.Click += new System.EventHandler(this.mnuBackdrop_Click);
         // 
         // mnuEditBackBlackDiamond
         // 
         this.mnuEditBackBlackDiamond.Index = 1;
         this.mnuEditBackBlackDiamond.RadioCheck = true;
         this.mnuEditBackBlackDiamond.Shortcut = System.Windows.Forms.Shortcut.Ctrl2;
         this.mnuEditBackBlackDiamond.Text = "Gray and black diamonds";
         this.mnuEditBackBlackDiamond.Click += new System.EventHandler(this.mnuBackdrop_Click);
         // 
         // mnuEditBackWhiteCross
         // 
         this.mnuEditBackWhiteCross.Index = 2;
         this.mnuEditBackWhiteCross.RadioCheck = true;
         this.mnuEditBackWhiteCross.Shortcut = System.Windows.Forms.Shortcut.Ctrl3;
         this.mnuEditBackWhiteCross.Text = "Light Crosshatch";
         this.mnuEditBackWhiteCross.Click += new System.EventHandler(this.mnuBackdrop_Click);
         // 
         // mnuEditBackBlackCross
         // 
         this.mnuEditBackBlackCross.Index = 3;
         this.mnuEditBackBlackCross.RadioCheck = true;
         this.mnuEditBackBlackCross.Shortcut = System.Windows.Forms.Shortcut.Ctrl4;
         this.mnuEditBackBlackCross.Text = "Dark Crosshatch";
         this.mnuEditBackBlackCross.Click += new System.EventHandler(this.mnuBackdrop_Click);
         // 
         // mnuEditBackWhite
         // 
         this.mnuEditBackWhite.Index = 4;
         this.mnuEditBackWhite.RadioCheck = true;
         this.mnuEditBackWhite.Shortcut = System.Windows.Forms.Shortcut.Ctrl5;
         this.mnuEditBackWhite.Text = "Solid White";
         this.mnuEditBackWhite.Click += new System.EventHandler(this.mnuBackdrop_Click);
         // 
         // mnuEditBackBlack
         // 
         this.mnuEditBackBlack.Index = 5;
         this.mnuEditBackBlack.RadioCheck = true;
         this.mnuEditBackBlack.Shortcut = System.Windows.Forms.Shortcut.Ctrl6;
         this.mnuEditBackBlack.Text = "Solid Black";
         this.mnuEditBackBlack.Click += new System.EventHandler(this.mnuBackdrop_Click);
         // 
         // mnuEditBackGray
         // 
         this.mnuEditBackGray.Index = 6;
         this.mnuEditBackGray.RadioCheck = true;
         this.mnuEditBackGray.Shortcut = System.Windows.Forms.Shortcut.Ctrl7;
         this.mnuEditBackGray.Text = "Solid Gray";
         this.mnuEditBackGray.Click += new System.EventHandler(this.mnuBackdrop_Click);
         // 
         // mnuEditSep5
         // 
         this.mnuEditSep5.Index = 22;
         this.mnuEditSep5.Text = "-";
         // 
         // mnuEditShowGrid
         // 
         this.mnuEditShowGrid.Index = 23;
         this.mnuEditShowGrid.Text = "&Grid";
         this.mnuEditShowGrid.Click += new System.EventHandler(this.mnuEditShowGrid_Click);
         // 
         // mnuTools
         // 
         this.mnuTools.Index = 3;
         this.mnuTools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuToolFreeDraw,
            this.mnuToolFreeLine,
            this.mnuToolBezier,
            this.mnuToolLine,
            this.mnuToolRectangle,
            this.mnuToolEllipse,
            this.mnuToolAirbrush,
            this.mnuToolSmooth,
            this.mnuToolErase,
            this.mnuToolGradientFill,
            this.mnuToolFloodFill,
            this.mnuToolFloodSel,
            this.mnuToolSelRect,
            this.mnuToolSelFree,
            this.mnuToolTranslate,
            this.mnuToolRotate,
            this.mnuToolScale,
            this.mnuToolDropper,
            this.mnuToolCustom,
            this.mnuToolSep1,
            this.mnuToolAntiAlias,
            this.mnuToolOutline,
            this.mnuToolFill,
            this.mnuToolGradientFills,
            this.mnuToolLock,
            this.mnuDisjointedColors,
            this.mnuToolSep2,
            this.mnuPen});
         this.mnuTools.MergeOrder = 3;
         this.mnuTools.MergeType = System.Windows.Forms.MenuMerge.Replace;
         this.mnuTools.Text = "&Tools";
         // 
         // mnuToolFreeDraw
         // 
         this.mnuToolFreeDraw.Checked = true;
         this.mnuToolFreeDraw.Index = 0;
         this.mnuToolFreeDraw.RadioCheck = true;
         this.mnuToolFreeDraw.Text = "&Freehand Drawing";
         this.mnuToolFreeDraw.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolFreeLine
         // 
         this.mnuToolFreeLine.Index = 1;
         this.mnuToolFreeLine.RadioCheck = true;
         this.mnuToolFreeLine.Text = "Freeha&nd Lines";
         this.mnuToolFreeLine.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolBezier
         // 
         this.mnuToolBezier.Index = 2;
         this.mnuToolBezier.Text = "&Bezier Curves";
         this.mnuToolBezier.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolLine
         // 
         this.mnuToolLine.Index = 3;
         this.mnuToolLine.RadioCheck = true;
         this.mnuToolLine.Text = "&Line";
         this.mnuToolLine.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolRectangle
         // 
         this.mnuToolRectangle.Index = 4;
         this.mnuToolRectangle.RadioCheck = true;
         this.mnuToolRectangle.Text = "&Rectangle";
         this.mnuToolRectangle.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolEllipse
         // 
         this.mnuToolEllipse.Index = 5;
         this.mnuToolEllipse.RadioCheck = true;
         this.mnuToolEllipse.Text = "Elli&pse";
         this.mnuToolEllipse.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolAirbrush
         // 
         this.mnuToolAirbrush.Index = 6;
         this.mnuToolAirbrush.RadioCheck = true;
         this.mnuToolAirbrush.Text = "Airbr&ush";
         this.mnuToolAirbrush.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolSmooth
         // 
         this.mnuToolSmooth.Index = 7;
         this.mnuToolSmooth.Text = "Smoot&h";
         this.mnuToolSmooth.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolErase
         // 
         this.mnuToolErase.Index = 8;
         this.mnuToolErase.RadioCheck = true;
         this.mnuToolErase.Text = "&Erase";
         this.mnuToolErase.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolGradientFill
         // 
         this.mnuToolGradientFill.Index = 9;
         this.mnuToolGradientFill.RadioCheck = true;
         this.mnuToolGradientFill.Text = "Gradient Fill";
         this.mnuToolGradientFill.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolFloodFill
         // 
         this.mnuToolFloodFill.Index = 10;
         this.mnuToolFloodFill.RadioCheck = true;
         this.mnuToolFloodFill.Text = "Flood Fill";
         this.mnuToolFloodFill.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolFloodSel
         // 
         this.mnuToolFloodSel.Index = 11;
         this.mnuToolFloodSel.RadioCheck = true;
         this.mnuToolFloodSel.Text = "Color &Wand";
         this.mnuToolFloodSel.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolSelRect
         // 
         this.mnuToolSelRect.Index = 12;
         this.mnuToolSelRect.RadioCheck = true;
         this.mnuToolSelRect.Text = "Rectan&gular Selection";
         this.mnuToolSelRect.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolSelFree
         // 
         this.mnuToolSelFree.Index = 13;
         this.mnuToolSelFree.RadioCheck = true;
         this.mnuToolSelFree.Text = "&Irregular Selection";
         this.mnuToolSelFree.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolTranslate
         // 
         this.mnuToolTranslate.Index = 14;
         this.mnuToolTranslate.RadioCheck = true;
         this.mnuToolTranslate.Text = "&Move Selection";
         this.mnuToolTranslate.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolRotate
         // 
         this.mnuToolRotate.Index = 15;
         this.mnuToolRotate.RadioCheck = true;
         this.mnuToolRotate.Text = "Ro&tate Selection";
         this.mnuToolRotate.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolScale
         // 
         this.mnuToolScale.Index = 16;
         this.mnuToolScale.RadioCheck = true;
         this.mnuToolScale.Text = "Si&ze Selection";
         this.mnuToolScale.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolDropper
         // 
         this.mnuToolDropper.Index = 17;
         this.mnuToolDropper.RadioCheck = true;
         this.mnuToolDropper.Text = "&Color Pick Dropper";
         this.mnuToolDropper.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolCustom
         // 
         this.mnuToolCustom.Index = 18;
         this.mnuToolCustom.Text = "Custom Tool";
         // 
         // mnuToolSep1
         // 
         this.mnuToolSep1.Index = 19;
         this.mnuToolSep1.Text = "-";
         // 
         // mnuToolAntiAlias
         // 
         this.mnuToolAntiAlias.Checked = true;
         this.mnuToolAntiAlias.Index = 20;
         this.mnuToolAntiAlias.Text = "&Anti-Alias";
         this.mnuToolAntiAlias.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolOutline
         // 
         this.mnuToolOutline.Checked = true;
         this.mnuToolOutline.Index = 21;
         this.mnuToolOutline.Text = "Draw &Outlined Shapes";
         this.mnuToolOutline.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolFill
         // 
         this.mnuToolFill.Index = 22;
         this.mnuToolFill.Text = "&Draw Filled Shapes";
         this.mnuToolFill.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolGradientFills
         // 
         this.mnuToolGradientFills.Index = 23;
         this.mnuToolGradientFills.Text = "Gradient Fills";
         this.mnuToolGradientFills.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolLock
         // 
         this.mnuToolLock.Index = 24;
         this.mnuToolLock.Text = "Loc&k Tools Proportions";
         this.mnuToolLock.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuDisjointedColors
         // 
         this.mnuDisjointedColors.Index = 25;
         this.mnuDisjointedColors.Text = "Affect Un&joined Color Regions";
         this.mnuDisjointedColors.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuToolSep2
         // 
         this.mnuToolSep2.Index = 26;
         this.mnuToolSep2.Text = "-";
         // 
         // mnuPen
         // 
         this.mnuPen.Index = 27;
         this.mnuPen.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuPixelPen,
            this.mnuTinyPen,
            this.mnuSmallPen,
            this.mnuMediumPen,
            this.mnuLargePen,
            this.mnuHugePen,
            this.mnu64Pen,
            this.mnuPenSep1,
            this.mnuPenDecrease,
            this.mnuPenIncrease,
            this.mnuPenSep2,
            this.mnuRoundPen,
            this.mnuSquarePen});
         this.mnuPen.Text = "Pen";
         // 
         // mnuPixelPen
         // 
         this.mnuPixelPen.Checked = true;
         this.mnuPixelPen.Index = 0;
         this.mnuPixelPen.RadioCheck = true;
         this.mnuPixelPen.Text = "&Pixel (1x1)";
         this.mnuPixelPen.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuTinyPen
         // 
         this.mnuTinyPen.Index = 1;
         this.mnuTinyPen.RadioCheck = true;
         this.mnuTinyPen.Text = "&Tiny (3x3)";
         this.mnuTinyPen.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuSmallPen
         // 
         this.mnuSmallPen.Index = 2;
         this.mnuSmallPen.RadioCheck = true;
         this.mnuSmallPen.Text = "&Small (5x5)";
         this.mnuSmallPen.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuMediumPen
         // 
         this.mnuMediumPen.Index = 3;
         this.mnuMediumPen.RadioCheck = true;
         this.mnuMediumPen.Text = "&Medium (7x7)";
         this.mnuMediumPen.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuLargePen
         // 
         this.mnuLargePen.Index = 4;
         this.mnuLargePen.RadioCheck = true;
         this.mnuLargePen.Text = "&Large (9x9)";
         this.mnuLargePen.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuHugePen
         // 
         this.mnuHugePen.Index = 5;
         this.mnuHugePen.RadioCheck = true;
         this.mnuHugePen.Text = "&Huge (11x11)";
         this.mnuHugePen.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnu64Pen
         // 
         this.mnu64Pen.Index = 6;
         this.mnu64Pen.Text = "&64x64";
         this.mnu64Pen.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuPenSep1
         // 
         this.mnuPenSep1.Index = 7;
         this.mnuPenSep1.Text = "-";
         // 
         // mnuPenDecrease
         // 
         this.mnuPenDecrease.Index = 8;
         this.mnuPenDecrease.Text = "&Decrease Size\t[";
         this.mnuPenDecrease.Click += new System.EventHandler(this.mnuPenDecrease_Click);
         // 
         // mnuPenIncrease
         // 
         this.mnuPenIncrease.Index = 9;
         this.mnuPenIncrease.Text = "&Increase Size\t]";
         this.mnuPenIncrease.Click += new System.EventHandler(this.mnuPenIncrease_Click);
         // 
         // mnuPenSep2
         // 
         this.mnuPenSep2.Index = 10;
         this.mnuPenSep2.Text = "-";
         // 
         // mnuRoundPen
         // 
         this.mnuRoundPen.Checked = true;
         this.mnuRoundPen.Index = 11;
         this.mnuRoundPen.RadioCheck = true;
         this.mnuRoundPen.Shortcut = System.Windows.Forms.Shortcut.F9;
         this.mnuRoundPen.Text = "&Round";
         this.mnuRoundPen.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // mnuSquarePen
         // 
         this.mnuSquarePen.Index = 12;
         this.mnuSquarePen.RadioCheck = true;
         this.mnuSquarePen.Shortcut = System.Windows.Forms.Shortcut.F10;
         this.mnuSquarePen.Text = "S&quare";
         this.mnuSquarePen.Click += new System.EventHandler(this.mnuTool_Click);
         // 
         // tbrOptions
         // 
         this.tbrOptions.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
         this.tbrOptions.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.tbrOptions.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.tbbAntiAlias,
            this.tbbOutline,
            this.tbbFill,
            this.tbbGradientFills,
            this.tbbLock,
            this.tbbDisjointedColors,
            this.tbsOptSep1,
            this.tbbPixelPen,
            this.tbbTinyPen,
            this.tbbSmallPen,
            this.tbbMediumPen,
            this.tbbLargePen,
            this.tbbHugePen,
            this.tbb64Pen,
            this.tbsOptSep2,
            this.tbbRound,
            this.tbbSquare,
            this.tbsOptSep3,
            this.tbdZoom,
            this.tbbShowGrid});
         this.tbrOptions.Divider = false;
         this.tbrOptions.DropDownArrows = true;
         this.tbrOptions.ImageList = this.imlGraphicsEditor;
         this.tbrOptions.Location = new System.Drawing.Point(54, 0);
         this.tbrOptions.Name = "tbrOptions";
         this.tbrOptions.ShowToolTips = true;
         this.tbrOptions.Size = new System.Drawing.Size(570, 27);
         this.tbrOptions.TabIndex = 1;
         this.tbrOptions.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbrGraphicsEditor_ButtonClick);
         // 
         // tbbAntiAlias
         // 
         this.tbbAntiAlias.ImageIndex = 6;
         this.tbbAntiAlias.Name = "tbbAntiAlias";
         this.tbbAntiAlias.Pushed = true;
         this.tbbAntiAlias.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbAntiAlias.ToolTipText = "Toggle anti-aliasing";
         // 
         // tbbOutline
         // 
         this.tbbOutline.ImageIndex = 8;
         this.tbbOutline.Name = "tbbOutline";
         this.tbbOutline.Pushed = true;
         this.tbbOutline.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbOutline.ToolTipText = "Toggle drawing of outlines around shapes";
         // 
         // tbbFill
         // 
         this.tbbFill.ImageIndex = 34;
         this.tbbFill.Name = "tbbFill";
         this.tbbFill.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbFill.ToolTipText = "Toggle filling of shapes";
         // 
         // tbbGradientFills
         // 
         this.tbbGradientFills.ImageIndex = 40;
         this.tbbGradientFills.Name = "tbbGradientFills";
         this.tbbGradientFills.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbGradientFills.ToolTipText = "Toggle use of gradients on filled shapes";
         // 
         // tbbLock
         // 
         this.tbbLock.ImageIndex = 32;
         this.tbbLock.Name = "tbbLock";
         this.tbbLock.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbLock.ToolTipText = "Toggle locking of proportions and rotation to nice ratios";
         // 
         // tbbDisjointedColors
         // 
         this.tbbDisjointedColors.ImageIndex = 46;
         this.tbbDisjointedColors.Name = "tbbDisjointedColors";
         this.tbbDisjointedColors.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbDisjointedColors.ToolTipText = "Fill and Color Wand affect all similar colors instead of joined regions";
         // 
         // tbsOptSep1
         // 
         this.tbsOptSep1.Name = "tbsOptSep1";
         this.tbsOptSep1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
         // 
         // tbbPixelPen
         // 
         this.tbbPixelPen.ImageIndex = 9;
         this.tbbPixelPen.Name = "tbbPixelPen";
         this.tbbPixelPen.Pushed = true;
         this.tbbPixelPen.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbPixelPen.ToolTipText = "1x1 pen";
         // 
         // tbbTinyPen
         // 
         this.tbbTinyPen.ImageIndex = 10;
         this.tbbTinyPen.Name = "tbbTinyPen";
         this.tbbTinyPen.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbTinyPen.ToolTipText = "3x3 Pen";
         // 
         // tbbSmallPen
         // 
         this.tbbSmallPen.ImageIndex = 11;
         this.tbbSmallPen.Name = "tbbSmallPen";
         this.tbbSmallPen.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbSmallPen.ToolTipText = "5x5 pen";
         // 
         // tbbMediumPen
         // 
         this.tbbMediumPen.ImageIndex = 12;
         this.tbbMediumPen.Name = "tbbMediumPen";
         this.tbbMediumPen.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbMediumPen.ToolTipText = "7x7 pen";
         // 
         // tbbLargePen
         // 
         this.tbbLargePen.ImageIndex = 13;
         this.tbbLargePen.Name = "tbbLargePen";
         this.tbbLargePen.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbLargePen.ToolTipText = "9x9 pen";
         // 
         // tbbHugePen
         // 
         this.tbbHugePen.ImageIndex = 14;
         this.tbbHugePen.Name = "tbbHugePen";
         this.tbbHugePen.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbHugePen.ToolTipText = "11x11 pen";
         // 
         // tbb64Pen
         // 
         this.tbb64Pen.ImageIndex = 15;
         this.tbb64Pen.Name = "tbb64Pen";
         this.tbb64Pen.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbb64Pen.ToolTipText = "64x64 pen";
         // 
         // tbsOptSep2
         // 
         this.tbsOptSep2.Name = "tbsOptSep2";
         this.tbsOptSep2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
         // 
         // tbbRound
         // 
         this.tbbRound.ImageIndex = 14;
         this.tbbRound.Name = "tbbRound";
         this.tbbRound.Pushed = true;
         this.tbbRound.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbRound.ToolTipText = "Round Pen";
         // 
         // tbbSquare
         // 
         this.tbbSquare.ImageIndex = 20;
         this.tbbSquare.Name = "tbbSquare";
         this.tbbSquare.ToolTipText = "Square/flat pen";
         // 
         // tbsOptSep3
         // 
         this.tbsOptSep3.Name = "tbsOptSep3";
         this.tbsOptSep3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
         // 
         // tbdZoom
         // 
         this.tbdZoom.DropDownMenu = this.mnuCZoom;
         this.tbdZoom.ImageIndex = 23;
         this.tbdZoom.Name = "tbdZoom";
         this.tbdZoom.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
         this.tbdZoom.ToolTipText = "Set magnification level";
         // 
         // tbbShowGrid
         // 
         this.tbbShowGrid.ImageIndex = 44;
         this.tbbShowGrid.Name = "tbbShowGrid";
         this.tbbShowGrid.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.tbbShowGrid.ToolTipText = "Toggle grid";
         // 
         // ToolSplitter
         // 
         this.ToolSplitter.Location = new System.Drawing.Point(48, 0);
         this.ToolSplitter.Name = "ToolSplitter";
         this.ToolSplitter.Size = new System.Drawing.Size(6, 363);
         this.ToolSplitter.TabIndex = 2;
         this.ToolSplitter.TabStop = false;
         // 
         // ctlColorSel
         // 
         this.ctlColorSel.CurrentColorType = SGDK2.SelectColorType.Pen;
         this.ctlColorSel.Cursor = System.Windows.Forms.Cursors.Default;
         this.ctlColorSel.Location = new System.Drawing.Point(328, 40);
         this.ctlColorSel.Name = "ctlColorSel";
         this.ctlColorSel.Size = new System.Drawing.Size(288, 296);
         this.ctlColorSel.TabIndex = 3;
         this.ctlColorSel.Text = "Color Selector";
         // 
         // dataMonitor
         // 
         this.dataMonitor.GraphicSheetRowChanged += new SGDK2.ProjectDataset.GraphicSheetRowChangeEventHandler(this.dataMonitor_GraphicSheetRowChanged);
         this.dataMonitor.GraphicSheetRowDeleted += new SGDK2.ProjectDataset.GraphicSheetRowChangeEventHandler(this.dataMonitor_GraphicSheetRowDeleted);
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         // 
         // frmGraphicsEditor
         // 
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
         this.AutoScroll = true;
         this.ClientSize = new System.Drawing.Size(624, 363);
         this.Controls.Add(this.ctlColorSel);
         this.Controls.Add(this.tbrOptions);
         this.Controls.Add(this.ToolSplitter);
         this.Controls.Add(this.tbrGraphicsEditor);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.KeyPreview = true;
         this.Menu = this.mnuGraphicsEditor;
         this.Name = "frmGraphicsEditor";
         this.Text = "Graphics Editor";
         this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
         this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frmGraphicsEditor_KeyPress);
         this.ResumeLayout(false);
         this.PerformLayout();

      }
      #endregion

      #region Private Methods
      private void SelectTool(Component cTool)
      {
         DrawingTool selTool = DrawingTool.None;
         int nPenSize = 0;

         foreach (ToolInfo ti in m_arTools)
         {
            if ((ti.Button == cTool) || (ti.Menu == cTool))
            {
               selTool = ti.Tool;
               break;
            }
         }

         if (selTool != DrawingTool.None)
         {
            if (selTool != DrawingTool.Custom)
            {
               tbdCustomTool.Pushed = false;
               foreach (CustomToolInfo ci in m_CustomTools)
                  ci.Menu.Checked = ci.ContextMenu.Checked = false;
            }

            foreach (ToolInfo ti in m_arTools)
               ti.Menu.Checked = ti.Button.Pushed = (ti.Tool == selTool);

            this.CurrentTool = selTool;

            if ((selTool == DrawingTool.FreeDraw) || (selTool == DrawingTool.Line))
            {
               tbbFill.Pushed = mnuToolFill.Checked = false;
               tbbOutline.Pushed = mnuToolOutline.Checked = true;

               if (selTool == DrawingTool.FreeDraw)
                  SelectTool(tbbRound);
            }
            if (selTool == DrawingTool.Erase)
               SelectTool(tbbLargePen);
            
            if ((selTool == DrawingTool.FloodFill) || (selTool == DrawingTool.AirBrush))
            {
               tbbFill.Pushed = mnuToolFill.Checked = true;
               tbbOutline.Pushed = mnuToolOutline.Checked = false;
            }
         }

         if ((cTool == tbbAntiAlias) || (cTool == mnuToolAntiAlias) ||
            (cTool == tbbFill) || (cTool == mnuToolFill) ||
            (cTool == tbbOutline) || (cTool == mnuToolOutline) ||
            (cTool == tbbGradientFills) || (cTool == mnuToolGradientFills) ||
            (cTool == tbbLock) || (cTool == mnuToolLock) || (cTool == mnuDisjointedColors) ||
            (cTool == tbbDisjointedColors))
         {
            if (cTool is ToolBarButton)
            {
               if (!tbbFill.Pushed && !tbbOutline.Pushed)
                  tbbOutline.Pushed = true;
               mnuToolAntiAlias.Checked = tbbAntiAlias.Pushed;
               mnuToolFill.Checked = tbbFill.Pushed;
               mnuToolOutline.Checked = tbbOutline.Pushed;
               mnuToolGradientFills.Checked = tbbGradientFills.Pushed;
               mnuToolLock.Checked = tbbLock.Pushed;
               mnuDisjointedColors.Checked = tbbDisjointedColors.Pushed;
            }
            else
            {
               ((MenuItem)cTool).Checked = !((MenuItem)cTool).Checked;
               if (!mnuToolFill.Checked && !mnuToolOutline.Checked)
                  mnuToolOutline.Checked = true;
               tbbAntiAlias.Pushed = mnuToolAntiAlias.Checked;
               tbbFill.Pushed = mnuToolFill.Checked;
               tbbOutline.Pushed = mnuToolOutline.Checked;
               tbbGradientFills.Pushed = mnuToolGradientFills.Checked;
               tbbLock.Pushed = mnuToolLock.Checked;
               tbbDisjointedColors.Pushed = mnuDisjointedColors.Checked;
            }
         }

         CurrentOptions = (tbbAntiAlias.Pushed?ToolOptions.AntiAlias:0) |
            (tbbFill.Pushed?ToolOptions.Fill:0) | (tbbOutline.Pushed?ToolOptions.Outline:0) |
            (tbbGradientFills.Pushed?ToolOptions.GradientFill:0) | (tbbLock.Pushed?ToolOptions.Lock:0) |
            (tbbDisjointedColors.Pushed?ToolOptions.DisjointedColors:0);

         if (selTool != DrawingTool.GradientFill)
         {
            if (0 == (CurrentOptions & ToolOptions.Outline))
               ctlColorSel.CurrentColorType = SelectColorType.Brush;
            else if (0 == (CurrentOptions & ToolOptions.Fill))
               ctlColorSel.CurrentColorType = SelectColorType.Pen;
         }

         foreach (PenInfo pi in m_arPens)
         {
            if ((pi.Button == cTool) || (pi.Menu == cTool))
               nPenSize = pi.PenSize;
         }
         if (nPenSize > 0)
         {
            foreach (PenInfo pi in m_arPens)
            {
               pi.Button.Pushed = pi.Menu.Checked = (pi.PenSize == nPenSize);
            }
            this.CurrentPen.Width = nPenSize;
         }

         if ((cTool == tbbRound) || (cTool == mnuRoundPen))
         {
            tbbRound.Pushed = mnuRoundPen.Checked = true;
            tbbSquare.Pushed = mnuSquarePen.Checked = false;
            this.CurrentPen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
            this.CurrentPen.LineJoin = LineJoin.Round;
            foreach (PenInfo pi in m_arPens)
               if ((pi.Button.ImageIndex >= 16) && (pi.Button.ImageIndex <= 20))
                  pi.Button.ImageIndex = pi.Button.ImageIndex - 6;
         }
         else if ((cTool == tbbSquare) || (cTool == mnuSquarePen)) 
         {
            tbbRound.Pushed = mnuRoundPen.Checked = false;
            tbbSquare.Pushed = mnuSquarePen.Checked = true;
            this.CurrentPen.SetLineCap(LineCap.Flat, LineCap.Flat, DashCap.Flat);
            this.CurrentPen.LineJoin = LineJoin.MiterClipped;
            this.CurrentPen.MiterLimit = 4f;
            foreach (PenInfo pi in m_arPens)
               if ((pi.Button.ImageIndex >=10) && (pi.Button.ImageIndex <= 14))
                  pi.Button.ImageIndex = pi.Button.ImageIndex + 6;
         }

         if ((cTool == mnuActualSize) ||
            (cTool == mnuZoom2) ||
            (cTool == mnuZoom4) ||
            (cTool == mnuZoom6) ||
            (cTool == mnuZoom8) ||
            (cTool == mnuZoom16) ||
            (cTool == mnuZoom1x1) ||
            (cTool == mnuZoom2x2) ||
            (cTool == mnuZoom4x4) ||
            (cTool == mnuZoom6x6) ||
            (cTool == mnuZoom8x8) ||
            (cTool == mnuZoom16x16))
         {
            Boolean bSuccess = false;
            mnuZoom1x1.Checked = mnuZoom2x2.Checked = mnuZoom4x4.Checked = mnuZoom6x6.Checked = mnuZoom8x8.Checked = mnuZoom16x16.Checked = false;
            if ((cTool == mnuActualSize) || (cTool == mnuZoom1x1))
            {
               if (bSuccess = m_frmMagnify.SetMagnify(1))
               {
                  tbdZoom.ImageIndex = 21;
                  mnuZoom1x1.Checked = true;
               }
            } 
            else if ((cTool == mnuZoom2) || (cTool == mnuZoom2x2))
            {
               if (bSuccess = m_frmMagnify.SetMagnify(2))
               {
                  tbdZoom.ImageIndex = 22;
                  mnuZoom2x2.Checked = true;
               }
            }
            else if ((cTool == mnuZoom4) || (cTool == mnuZoom4x4))
            {
               if (bSuccess = m_frmMagnify.SetMagnify(4))
               {
                  tbdZoom.ImageIndex = 23;
                  mnuZoom4x4.Checked = true;
               }
            }
            else if ((cTool == mnuZoom6) || (cTool == mnuZoom6x6))
            {
               if (bSuccess = m_frmMagnify.SetMagnify(6))
               {
                  tbdZoom.ImageIndex = 24;
                  mnuZoom6x6.Checked = true;
               }
            }
            else if ((cTool == mnuZoom8) || (cTool == mnuZoom8x8))
            {
               if (bSuccess = m_frmMagnify.SetMagnify(8))
               {
                  tbdZoom.ImageIndex = 25;
                  mnuZoom8x8.Checked = true;
               }
            }
            else if ((cTool == mnuZoom16) || (cTool == mnuZoom16x16))
            {
               if (bSuccess = m_frmMagnify.SetMagnify(16))
               {
                  tbdZoom.ImageIndex = 49;
                  mnuZoom16x16.Checked = true;
               }
            }
            if (bSuccess)
               foreach (MenuItem mi in new MenuItem[] {mnuActualSize, mnuZoom2, mnuZoom4, mnuZoom6, mnuZoom8})
                  mi.Checked = (mi == cTool);
            ArrangePanes();
         }
      }

      private void RemapHues(frmHueMapParams frm, Bitmap target)
      {
         int maxX, maxY;
         maxX = target.Width;
         maxY = target.Height;

         for (int y=0; y<maxY; y++)
            for (int x=0; x<maxX; x++)
            {
               System.Drawing.Color clr;
               clr = target.GetPixel(x,y);
               if ((FloatingSelection == null) && (SelectedRegion != null) &&
                  (!SelectedRegion.IsVisible((float)x, (float)y)))
                        continue;
               if (clr.A > 0)
               {
                  float oldHue;
                  float oldSat;
                  float oldVal;
                  ColorSel.RGBtoHSV(clr, out oldHue, out oldSat, out oldVal);
                  oldHue *= 180.0f / (float)Math.PI;

                  // number 0-1 representing distance of hue into source range
                  float rangeDistance = -1;
                  if (frm.sourceHueRange > 0)
                  {
                     if ((oldHue >= frm.sourceStartHue) &&
                        (oldHue <= frm.sourceStartHue + frm.sourceHueRange))
                        rangeDistance = (oldHue - frm.sourceStartHue) / frm.sourceHueRange;
                     else if ((oldHue + 360.0f >= frm.sourceStartHue) &&
                        (oldHue + 360.0f <= frm.sourceStartHue + frm.sourceHueRange))
                        rangeDistance = (oldHue + 360.0f - frm.sourceStartHue) / frm.sourceHueRange;
                  }
                  else
                  {
                     if ((oldHue <= frm.sourceStartHue) &&
                        (oldHue >= frm.sourceStartHue + frm.sourceHueRange))
                        rangeDistance = (oldHue - frm.sourceStartHue) / frm.sourceHueRange;
                     else if ((oldHue - 360.0f <= frm.sourceStartHue) &&
                        (oldHue - 360.0f >= frm.sourceStartHue + frm.sourceHueRange))
                        rangeDistance = (oldHue - 360.0f - frm.sourceStartHue) / frm.sourceHueRange;
                  }

                  if (rangeDistance < 0)
                     continue;

                  float newHue = frm.targetStartHue + frm.targetHueRange * rangeDistance;
                  while (newHue < 0)
                     newHue += 360.0f;
                  while (newHue >= 360.0f)
                     newHue -= 360.0f;

                  clr = ColorSel.HSVtoRGB(newHue * Math.PI / 180.0f, oldSat, oldVal);
                  target.SetPixel(x, y, clr);
               }
            }
      }
      #endregion

      #region Public Static Members
      public static frmGraphicsEditor Edit(Form MdiParent, ProjectDataset.GraphicSheetRow EditRow)
      {
         foreach(Form frm in MdiParent.MdiChildren)
         {
            frmGraphicsEditor f = frm as frmGraphicsEditor;
            if (f != null)
            {
               if (f.m_DataSource == EditRow)
               {
                  f.Activate();
                  return f;
               }
            }
         }

         frmGraphicsEditor frmNew = new frmGraphicsEditor(EditRow);
         frmNew.MdiParent = MdiParent;
         frmNew.Show();
         return frmNew;
      }

      public static void Edit(Form MdiParent, ProjectDataset.GraphicSheetRow EditRow, int cellIndex)
      {
         frmGraphicsEditor frmNew = Edit(MdiParent, EditRow);
         frmNew.m_frmCells.LoadCellIntoEditor(cellIndex);
      }
      #endregion

      #region Public Methods
      public void DeleteFloatingSelection()
      {
         if (FloatingSelection != null)
         {
            FloatingSelection.Dispose();
            FloatingSelection = null;
         }
         if (SelectedRegion != null)
         {
            SelectedRegion.Dispose();
            SelectedRegion = null;
         }
         SelectionOutline = null;
         if (SelectionTransform != null)
         {
            SelectionTransform.Dispose();
            SelectionTransform = null;
         }
         m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
         m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
      }

      public void InitGraphicsSettings(Graphics g)
      {
         g.SmoothingMode = (0 != (CurrentOptions & ToolOptions.AntiAlias))?
            SmoothingMode.HighQuality:SmoothingMode.None;
         g.InterpolationMode = (0 != (CurrentOptions & ToolOptions.AntiAlias))?
            InterpolationMode.High:InterpolationMode.NearestNeighbor;
         g.CompositingMode = CompositingMode;
      }

      public void DropFloatingSelection()
      {
         if (FloatingSelection != null)
         {
            Graphics gDrop = Graphics.FromImage(m_imgCurrentGraphic);
            InitGraphicsSettings(gDrop);
            if (SelectionTransform != null)
               gDrop.Transform = SelectionTransform;
            gDrop.DrawImage(FloatingSelection, 0, 0);
            gDrop.Dispose();
         }
         m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
         m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
         DeleteFloatingSelection();
         ChildPane_GraphicChanged();
      }

      public void ActivateTool(DrawingTool tool)
      {
         foreach (ToolInfo ti in m_arTools)
         {
            if (ti.Tool == tool)
            {
               ti.Button.Pushed = true;
               SelectTool(ti.Button);
            }
         }
      }

      public Bitmap CopySelection(Graphics g)
      {
         if (SelectedRegion == null)
            return (Bitmap)m_imgCurrentGraphic.Clone();

         Graphics gSrc = g;
         Bitmap imgResult;
         if (gSrc == null)
            gSrc = Graphics.FromImage(m_imgCurrentGraphic);
         Rectangle rcFloat = Rectangle.Round(SelectedRegion.GetBounds(gSrc));
         gSrc.Clip = SelectedRegion;
         imgResult = new Bitmap(rcFloat.Width, rcFloat.Height,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
         Graphics gDest = Graphics.FromImage(imgResult);
         gDest.Clip = SelectedRegion;
         gDest.TranslateClip(-rcFloat.X, -rcFloat.Y);
         gDest.DrawImage(m_imgCurrentGraphic, 0, 0, rcFloat, GraphicsUnit.Pixel);
         gDest.Dispose();
         if (g == null)
            gSrc.Dispose();
         return imgResult;
      }
      public void PickColor(Color clr)
      {
         ctlColorSel.ActivateColor(clr);
      }

      public Rectangle UsableClientRect
      {
         get
         {
            Rectangle rcClient = this.ClientRectangle;
            rcClient.Width -= ToolSplitter.Right;
            rcClient.X = ToolSplitter.Right;
            rcClient.Height -= tbrOptions.Bottom;
            rcClient.Y = tbrOptions.Bottom;
            return rcClient;
         }
      }
      /// <summary>
      /// Loads a graphic into the editor panes for editing, if possible
      /// </summary>
      /// <param name="bmpEdit">The image to edit</param>
      /// <returns>True if the image could be loaded, false if it is too big.</returns>
      public Boolean LoadImage(Bitmap bmpEdit)
      {
         if ((bmpEdit.Height * m_frmMagnify.ClientSize.Height / m_imgCurrentGraphic.Height > this.ClientSize.Height) ||
            (bmpEdit.Width * m_frmMagnify.ClientSize.Width / m_imgCurrentGraphic.Width > this.ClientSize.Width))
         {
            MessageBox.Show(this, "The image is to big to edit at the current magnification", "Load Graphic Cell", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         m_frmMagnify.ClientSize = new Size(bmpEdit.Width * m_frmMagnify.ClientSize.Width / m_imgCurrentGraphic.Width,
            bmpEdit.Height * m_frmMagnify.ClientSize.Height / m_imgCurrentGraphic.Height);
         m_frmActual.ClientSize = bmpEdit.Size;
         if (m_imgCurrentGraphic != null)
            m_imgCurrentGraphic.Dispose();
         for (int i = 0; i<m_arimgUndoStates.Length; i++)
            if (m_arimgUndoStates[i] != null)
            {
               m_arimgUndoStates[i].Dispose();
               m_arimgUndoStates[i] = null;
            }
         m_frmMagnify.ClearTempImage();
         m_frmActual.ClearTempImage();
         m_frmMagnify.Image = m_frmActual.Image = m_imgCurrentGraphic = bmpEdit;
         m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
         m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
         ChildPane_GraphicChanged();
         ArrangePanes();
         return true;
      }

      public void ArrangePanes()
      {
         m_frmMagnify.SetBounds(0,0,0,0,BoundsSpecified.Location);
         ctlColorSel.SetBounds(m_frmMagnify.Right, m_frmMagnify.Top, 0, 0, BoundsSpecified.Location);
         if (m_frmMagnify.Height > ctlColorSel.Height)
         {
            m_frmActual.SetBounds(m_frmMagnify.Right, m_frmMagnify.Top + ctlColorSel.Height, 0, 0, BoundsSpecified.Location);
            m_frmCells.SetBounds(m_frmMagnify.Right, m_frmActual.Bottom, 0, 0, BoundsSpecified.Location);
         }
         else
         {
            m_frmActual.SetBounds(0, m_frmMagnify.Bottom, 0, 0, BoundsSpecified.Location);
            m_frmCells.SetBounds(m_frmMagnify.Right, m_frmMagnify.Top + ctlColorSel.Height, 0, 0, BoundsSpecified.Location);
         }
      }
      #endregion

      #region Overrides
      protected override void OnLoad(EventArgs e)
      {
         m_imgCurrentGraphic = new Bitmap(m_DataSource.CellWidth, m_DataSource.CellHeight, PixelFormat.Format32bppArgb);
         m_frmMagnify = new frmGraphicPane(this, m_imgCurrentGraphic, 4);
         this.Controls.Add(m_frmMagnify);
         m_frmMagnify.Show();
         m_frmActual = new frmGraphicPane(this, m_imgCurrentGraphic, 1);
         this.Controls.Add(m_frmActual);
         m_frmActual.Show();
         m_frmCells = new frmCellMgr(this, m_DataSource);
         m_frmCells.Show();
         CurrentPen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
         CurrentPen.LineJoin = LineJoin.Round;
         ChildPane_GraphicChanged();
         ArrangePanes();

         for (int i = 0; i < CustTool.CustomToolCount; i++)
         {
            MenuItem mnu = mnuToolCustom.MenuItems.Add(CustTool.GetToolName(i), new System.EventHandler(mnuCustomTool_Click));
            MenuItem mnuContext = mnuCCustomTool.MenuItems.Add(CustTool.GetToolName(i), new System.EventHandler(mnuCustomTool_Click));
            mnuContext.OwnerDraw = true;
            mnuContext.MeasureItem += new MeasureItemEventHandler(mnuCustomTool_MeasureItem);
            mnuContext.DrawItem += new DrawItemEventHandler(mnuCustomTool_DrawItem);
            int imgIdx;
            //do
            //{
               imgIdx = imlGraphicsEditor.Images.Add(CustTool.GetToolImage(i), Color.Transparent);
            //} while (imgIdx == 52); // Bug in image list has bad image #52? Enable this code if problem remains
            m_CustomTools.Add(new CustomToolInfo(mnu, mnuContext, i, imgIdx));
         }
         tbdCustomTool.ImageIndex = ((CustomToolInfo)m_CustomTools[0]).ImageIndex;

         m_frmMagnify.ViewChanged += new frmGraphicPane.ViewChangedEvent(Magnify_ViewChanged);
         m_frmActual.ViewChanged += new frmGraphicPane.ViewChangedEvent(Actual_ViewChanged);
         m_frmMagnify.GraphicChanged += new frmGraphicPane.GraphicChangedEvent(ChildPane_GraphicChanged);
         m_frmActual.GraphicChanged += new frmGraphicPane.GraphicChangedEvent(ChildPane_GraphicChanged);

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"html/2d575c3e-dff1-484e-8f48-6109b0f79028.htm");
         SGDK2IDE.g_HelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);

         base.OnLoad (e);
      }
   
      protected override void OnKeyDown(KeyEventArgs e)
      {
         if ((e.KeyCode == Keys.ShiftKey) && IsDragging)
            if (0 == (CurrentOptions & ToolOptions.Lock))
            {
               tbbLock.Pushed = true;
               SelectTool(tbbLock);
            }
         if ((e.KeyCode == Keys.ControlKey) && ((CurrentTool == DrawingTool.FloodFill) ||
            (CurrentTool == DrawingTool.FloodSel)))
            if (0 == (CurrentOptions & ToolOptions.DisjointedColors))
            {
               tbbDisjointedColors.Pushed = true;
               SelectTool(tbbDisjointedColors);
            }
         base.OnKeyDown (e);
      }
   
      protected override void OnKeyUp(KeyEventArgs e)
      {
         if (e.KeyCode == Keys.ShiftKey)
            if (0 != (CurrentOptions & ToolOptions.Lock))
            {
               tbbLock.Pushed = false;
               SelectTool(tbbLock);
            }
         if ((e.KeyCode == Keys.ControlKey) && ((CurrentTool == DrawingTool.FloodFill) ||
            (CurrentTool == DrawingTool.FloodSel)))
            if (0 != (CurrentOptions & ToolOptions.DisjointedColors))
            {
               tbbDisjointedColors.Pushed = false;
               SelectTool(tbbDisjointedColors);
            }
         base.OnKeyUp (e);
      }
   
      protected override void OnKeyPress(KeyPressEventArgs e)
      {
         if (e.KeyChar == (char)27) // Escape
         {
            m_frmActual.RejectDrawing();
            m_frmMagnify.RejectDrawing();
         }
         base.OnKeyPress (e);
      }

      protected override void OnClosing(CancelEventArgs e)
      {
         base.OnClosing (e);
         SGDK2IDE.SaveFormSettings(this);
      }

      #endregion

      #region Event Handlers
      private void tbrGraphicsEditor_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
      {
         if (e.Button == tbbHFlip)
         {
            mnuEditFlip_Click(mnuEditHFlip, null);
            return;
         }

         if (e.Button == tbbVFlip)
         {
            mnuEditFlip_Click(mnuEditVFlip, null);
            return;
         }

         if (e.Button == tbbHOffset)
         {
            mnuEditOffset_Click(mnuEditHOffset, null);
            return;
         }

         if (e.Button == tbbVOffset)
         {
            mnuEditOffset_Click(mnuEditVOffset, null);
            return;
         }

         if (e.Button == tbbTilePreview)
         {
            mnuEditTilePreview_Click(mnuEditTilePreview, null);
            return;
         }

         if (e.Button == tbbShowGrid)
         {
            mnuEditShowGrid_Click(e.Button, null);
            return;
         }

         if (e.Button == tbdCustomTool)
         {
            mnuCustomTool_Click(tbdCustomTool, null);
            return;
         }

         if (e.Button == tbbHueMap)
         {
            mnuHueMap_Click(tbbHueMap, null);
            return;
         }

         if (e.Button == tbbNoise)
         {
            mnuNoise_Click(tbbNoise, null);
            return;
         }

         SelectTool(e.Button);
      }

      private void mnuExportGraphic_Click(object sender, System.EventArgs e)
      {
         SaveFileDialog dlgSave = new SaveFileDialog();
         dlgSave.DefaultExt = "png";
         dlgSave.Filter = "PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg)|*.jpg|TIFF (*.TIF)|*.tif";
         dlgSave.FilterIndex = 1;
         dlgSave.CheckPathExists = true;
         dlgSave.AddExtension = true;
         dlgSave.OverwritePrompt = true;
         dlgSave.RestoreDirectory = true;
         dlgSave.ValidateNames = true;
         if (DialogResult.Cancel != dlgSave.ShowDialog(this))
         {
            System.Drawing.Imaging.ImageFormat fmt = System.Drawing.Imaging.ImageFormat.Png;
            switch (dlgSave.FilterIndex)
            {
               case 1:
                  fmt = System.Drawing.Imaging.ImageFormat.Png;
                  break;
               case 2:
                  fmt = System.Drawing.Imaging.ImageFormat.Bmp;
                  break;
               case 3:
                  fmt = System.Drawing.Imaging.ImageFormat.Gif;
                  break;
               case 4:
                  fmt = System.Drawing.Imaging.ImageFormat.Jpeg;
                  break;
               case 5:
                  fmt = System.Drawing.Imaging.ImageFormat.Tiff;
                  break;
            }
            m_frmActual.Image.Save(dlgSave.FileName, fmt);
         }
      }

      private void Magnify_ViewChanged()
      {
         m_frmActual.DrawTransparentImage(m_frmMagnify.TempImage, null);
         m_frmActual.DragEnd = m_frmMagnify.DragEnd;
      }

      private void Actual_ViewChanged()
      {
         m_frmMagnify.DrawTransparentImage(m_frmActual.TempImage, null);
         m_frmMagnify.DragEnd = m_frmActual.DragEnd;
      }

      private void ChildPane_GraphicChanged()
      {
         ctlColorSel.RememberColors();
         if (m_arimgUndoStates[m_arimgUndoStates.Length - 1] != null)
            m_arimgUndoStates[m_arimgUndoStates.Length - 1].Dispose();
         for (int i = m_arimgUndoStates.Length - 1; i > 0; i--)
         {
            m_arimgUndoStates[i] = m_arimgUndoStates[i-1];
         }
         m_arimgUndoStates[0] = (Bitmap)m_imgCurrentGraphic.Clone();
      }

      private void mnuTool_Click(object sender, System.EventArgs e)
      {
         SelectTool((MenuItem)sender);
      }

      private void mnuEditUndo_Click(object sender, System.EventArgs e)
      {
         Bitmap imgTemp = m_arimgUndoStates[0];

         if (m_arimgUndoStates[1] == null)
            return;

         int i;
         for (i = 0; (i < m_arimgUndoStates.Length - 1) && (m_arimgUndoStates[i+1] != null); i++)
         {
            m_arimgUndoStates[i] = m_arimgUndoStates[i+1];
         }
         m_arimgUndoStates[i] = imgTemp;
         m_imgCurrentGraphic.Dispose();
         m_imgCurrentGraphic = (Bitmap)m_arimgUndoStates[0].Clone();
         m_frmActual.Image = m_frmMagnify.Image = m_imgCurrentGraphic;
         m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
         m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
      }

      private void mnuEditRedo_Click(object sender, System.EventArgs e)
      {
         Bitmap imgTemp;
         
         if (m_arimgUndoStates[1] == null)
            return;

         int i;
         for (i = m_arimgUndoStates.Length - 1; m_arimgUndoStates[i] == null; i--)
            ;

         imgTemp = m_arimgUndoStates[i];

         for (; (i > 0) && (m_arimgUndoStates[i-1] != null); i--)
         {
            m_arimgUndoStates[i] = m_arimgUndoStates[i-1];
         }
         m_arimgUndoStates[i] = imgTemp;
         m_imgCurrentGraphic.Dispose();
         m_imgCurrentGraphic = (Bitmap)m_arimgUndoStates[0].Clone();
         m_frmActual.Image = m_frmMagnify.Image = m_imgCurrentGraphic;      
         m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
         m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
      }

      private void mnuEditResetPos_Click(object sender, System.EventArgs e)
      {
         if (TempTransform != null)
            TempTransform.Dispose();
         if (SelectionTransform != null)
            SelectionTransform.Reset();
         m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
         m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
      }

      private void mnuEditDeselect_Click(object sender, System.EventArgs e)
      {
         DropFloatingSelection();
      }

      private void mnuClipAction_Click(object sender, System.EventArgs e)
      {
         if ((sender == mnuEditCut) || (sender == mnuEditCopy))
         {
            if (FloatingSelection == null)
               Clipboard.SetDataObject(CopySelection(null), true);
            else
               Clipboard.SetDataObject(FloatingSelection, true);
         }  
         if ((sender == mnuEditDelete) || (sender == mnuEditCut))
         {
            if ((FloatingSelection == null) && (SelectedRegion != null))
               m_frmMagnify.FloatSelection();
            DeleteFloatingSelection();
         }
         if (sender == mnuEditPaste)
         {
            if (FloatingSelection != null)
               DropFloatingSelection();
            DeleteFloatingSelection();
            IDataObject datClip = Clipboard.GetDataObject();
            Bitmap imgClip = (Bitmap)datClip.GetData(typeof(Bitmap));
            if (imgClip != null)
            {
               FloatingSelection = imgClip;
               SelectionTransform = new Matrix();
               SelectedRegion = new Region(new Rectangle(0, 0, imgClip.Width, imgClip.Height));
               m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
               m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
            }
         }
      }

      private void mnuEditSelColor_Click(object sender, System.EventArgs e)
      {
         if (HighlightBrush != null)
         {
            HighlightBrush.Dispose();
            HighlightBrush = null;
         }

         if (sender == mnuEditShowHighlight)
            mnuEditShowHighlight.Checked = !mnuEditShowHighlight.Checked;

         SelHighlightInfo hiSel = new SelHighlightInfo(null, Color.Empty);
         foreach (SelHighlightInfo hi in m_arSelHighlightInfo)
         {
            if (hi.Menu == sender)
               hiSel = hi;
         }

         if (hiSel.Menu != null)
         {
            foreach (SelHighlightInfo hi in m_arSelHighlightInfo)
               hi.Menu.Checked = (sender == hi.Menu);
         }

         if (mnuEditShowHighlight.Checked)
         {
            foreach (SelHighlightInfo hi in m_arSelHighlightInfo)
               if (hi.Menu.Checked)
                  HighlightBrush = new SolidBrush(hi.Highlight);
         }

         m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
         m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
         m_frmCells.InvalidateSheet();
      }

      private void mnuBackdrop_Click(object sender, System.EventArgs e)
      {
         foreach (BackdropInfo bi in m_arBackDropInfo)
         {
            bi.Menu.Checked = (bi.Menu == sender);
            if (bi.Menu == sender)
            {
               Backdrop.Dispose();
               if (bi.IsHatched)
               {
                  Backdrop = new HatchBrush(bi.Style, bi.ForeColor, bi.BackColor);
               }
               else
               {
                  Backdrop = new SolidBrush(bi.ForeColor);
               }
            }
         }
         m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
         m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
         m_frmCells.Backdrop = Backdrop;
      }

      private void mnuEditFlip_Click(object sender, System.EventArgs e)
      {
         Graphics g = Graphics.FromImage(m_imgCurrentGraphic);
         if (sender == mnuEditHFlip)
         {
            if ((SelectedRegion != null) && (SelectionTransform == null))
               m_frmMagnify.FloatSelection();

            if (SelectionTransform != null)
            {
               SelectionTransform.Multiply(new Matrix(-1,0,0,1,SelectedRegion.GetBounds(g).Width,0));
            }
            else
            {
               g.Transform = new Matrix(-1,0,0,1,m_imgCurrentGraphic.Width,0);
               g.CompositingMode = CompositingMode.SourceCopy;
               g.DrawImage(m_imgCurrentGraphic,0,0);
            }
         }
         else
         {
            if ((SelectedRegion != null) && (SelectionTransform == null))
               m_frmMagnify.FloatSelection();

            if (SelectionTransform != null)
            {
               SelectionTransform.Multiply(new Matrix(1,0,0,-1,0,SelectedRegion.GetBounds(g).Height));
            }
            else
            {
               g.Transform = new Matrix(1,0,0,-1,0,m_imgCurrentGraphic.Height);
               g.CompositingMode = CompositingMode.SourceCopy;
               g.DrawImage(m_imgCurrentGraphic,0,0);
            }
         }
         m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
         m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
         g.Dispose();
         ChildPane_GraphicChanged();
      }

      private void mnuEditSelectAll_Click(object sender, System.EventArgs e)
      {
         if (FloatingSelection != null)
            DropFloatingSelection();
         if (SelectionTransform != null)
         {
            SelectionTransform.Dispose();
            SelectionTransform = null;
         }
         if (SelectedRegion != null)
            SelectedRegion.Dispose();
         SelectedRegion = new Region(new Rectangle(0,0,m_imgCurrentGraphic.Width, m_imgCurrentGraphic.Height));
         SelectionOutline = new PointF[] {new PointF(0,0), new PointF(m_imgCurrentGraphic.Width, 0),
                                            new PointF(m_imgCurrentGraphic.Width, m_imgCurrentGraphic.Height),
                                            new PointF(0,m_imgCurrentGraphic.Height), new PointF(0,0)};
         m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
         m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
      }

      private void mnuEditOffset_Click(object sender, System.EventArgs e)
      {
         System.Drawing.Imaging.BitmapData datImage = m_imgCurrentGraphic.LockBits(
            new Rectangle(0,0,m_imgCurrentGraphic.Width, m_imgCurrentGraphic.Height),
            ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

         int cbBufSize = Math.Abs(datImage.Stride) * datImage.Height / 4;
         Int32[] buffer = new Int32[cbBufSize];
         System.Runtime.InteropServices.Marshal.Copy(datImage.Scan0, buffer, 0, cbBufSize);

         int nPixelStride = Math.Abs(datImage.Stride) / 4;

         try
         {
            if (sender == mnuEditHOffset)
            {
               if (0 != (m_imgCurrentGraphic.Width % 2))
                  if (DialogResult.OK != MessageBox.Show(this, "This action is not supported on an image with an odd width; the rightmost column of pixels (which should go in the middle) will not be moved.  Proceed anyway?", "Horizontal Offset & Wrap", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
                     return;
               int offset = m_imgCurrentGraphic.Width / 2;
               for (int y = 0; y < m_imgCurrentGraphic.Height; y++)
               {
                  for (int x = 0; x < offset; x++)
                  {
                     Int32 nTemp = buffer[x + y * nPixelStride];
                     buffer[x + y * nPixelStride] = buffer[x + y * nPixelStride + offset];
                     buffer[x + y * nPixelStride + offset] = nTemp;
                  }
               }
            }
            else
            {
               if (0 != (m_imgCurrentGraphic.Height % 2))
                  if (DialogResult.OK != MessageBox.Show(this, "This action is not supported on an image with an odd height; the bottom row of pixels (which should go in the middle) will not be moved.  Proceed anyway?", "Vertical Offset & Wrap", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
                     return;
               int offset = m_imgCurrentGraphic.Height / 2;
               for (int y = 0; y < offset; y++)
               {
                  for (int x = 0; x < m_imgCurrentGraphic.Width; x++)
                  {
                     Int32 nTemp = buffer[x + y * nPixelStride];
                     buffer[x + y * nPixelStride] = buffer[x + (y + offset) * nPixelStride];
                     buffer[x + (y + offset) * nPixelStride] = nTemp;
                  }
               }
            }
         }
         finally
         {
            try
            {
               System.Runtime.InteropServices.Marshal.Copy(buffer, 0, datImage.Scan0, cbBufSize);
               m_imgCurrentGraphic.UnlockBits(datImage);
               m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
               m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
               ChildPane_GraphicChanged();
            }
            catch(Exception ex)
            {
               MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
         }
      }

      private void mnuEditTilePreview_Click(object sender, System.EventArgs e)
      {
         frmTilePreview frm = new frmTilePreview(Backdrop, m_imgCurrentGraphic);
         frm.ShowDialog(this);
      }

      private void mnuEditShowGrid_Click(object sender, System.EventArgs e)
      {
         if (sender == mnuEditShowGrid)
            m_frmMagnify.ShowGrid = mnuEditShowGrid.Checked = tbbShowGrid.Pushed = !mnuEditShowGrid.Checked;
         else
            m_frmMagnify.ShowGrid = mnuEditShowGrid.Checked = tbbShowGrid.Pushed;
         m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
      }

      private void mnuCustomTool_Click(object sender, System.EventArgs e)
      {
         foreach (ToolInfo ti in m_arTools)
            ti.Menu.Checked = ti.Button.Pushed = false;
         this.CurrentTool = DrawingTool.Custom;
         foreach (CustomToolInfo ct in m_CustomTools)
            if ((ct.Menu == sender) || (ct.ContextMenu == sender) ||
                ((sender == tbdCustomTool) && (ct.ToolIndex == m_CustomTool)))
            {
               m_CustomTool = ct.ToolIndex;
               ct.Menu.Checked = ct.ContextMenu.Checked = true;
               tbdCustomTool.ImageIndex = ct.ImageIndex;
            }
            else
               ct.Menu.Checked = ct.ContextMenu.Checked = false;
         if (sender == tbdCustomTool)
            tbdCustomTool.Pushed = true;
         else
            ((MenuItem)sender).Checked = tbdCustomTool.Pushed = true;
      }

      private void mnuCustomTool_MeasureItem(object sender, MeasureItemEventArgs e)
      {
         CustomToolInfo cti = (CustomToolInfo)m_CustomTools[e.Index];
         e.ItemHeight = 16;
         SizeF StringSize = e.Graphics.MeasureString(cti.ContextMenu.Text, this.Font);
         e.ItemWidth = (int)StringSize.Width + 16;
         if (StringSize.Height > 16)
            e.ItemHeight = (int)StringSize.Height;
      }

      private void mnuCustomTool_DrawItem(object sender, DrawItemEventArgs e)
      {
         CustomToolInfo cti = (CustomToolInfo)m_CustomTools[e.Index];
         if (0 != (e.State & DrawItemState.Selected))
         {
            e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
            e.Graphics.DrawString(cti.ContextMenu.Text, this.Font, SystemBrushes.HighlightText,
               e.Bounds.X + 16, e.Bounds.Y);
         }
         else
         {
            e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds);
            e.Graphics.DrawString(cti.ContextMenu.Text, this.Font, SystemBrushes.ControlText, e.Bounds.X + 16,
               e.Bounds.Y);
         }
         e.Graphics.DrawImageUnscaled(imlGraphicsEditor.Images[cti.ImageIndex], e.Bounds.X,
            e.Bounds.Y);
      }

      private void mnuImportGraphic_Click(object sender, System.EventArgs e)
      {
         Bitmap importImage = frmImportGraphics.ImportGraphic(this.MdiParent, m_DataSource.CellWidth, m_DataSource.CellHeight);
         Retry:
         if (importImage != null)
         {
            if (!LoadImage(importImage))
            {
               if (MessageBox.Show(this.MdiParent, "Would you like to re-import from a subset of the imported image?", "Re-import", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
               {
                  Bitmap importSubimage = frmImportGraphics.ImportGraphic(this.MdiParent, m_DataSource.CellWidth, m_DataSource.CellHeight, importImage);
                  importImage.Dispose();
                  importImage = importSubimage;
                  goto Retry;
               }
               importImage.Dispose();
            }
         }
      }

      private void dataMonitor_GraphicSheetRowDeleted(object sender, SGDK2.ProjectDataset.GraphicSheetRowChangeEvent e)
      {
         if (e.Row == m_DataSource)
            this.Close();
      }

      private void dataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void mnuHueMap_Click(object sender, System.EventArgs e)
      {
         using (frmHueMapParams frm = new frmHueMapParams())
         {
            frm.Preview += new EventHandler(HueRemap_Preview);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
               if (FloatingSelection == null)
                  RemapHues(frm, m_imgCurrentGraphic);
               else
                  RemapHues(frm, FloatingSelection);
               m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
               m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
               ChildPane_GraphicChanged();      
            }
            else
            {
               m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
               m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
            }
         }
      }

      private void mnuNoise_Click(object sender, System.EventArgs e)
      {
         Random rnd = new Random();
         int maxX, maxY;
         if (FloatingSelection != null)
         {
            maxX = FloatingSelection.Width;
            maxY = FloatingSelection.Height;
         }
         else
         {
            maxX = m_imgCurrentGraphic.Width;
            maxY = m_imgCurrentGraphic.Height;
         }

         string userEntry = frmInputBox.GetInput(this, "Add Noise", "Noise Level (1-255)", "24");
         if (userEntry == null)
            return;
         int noiseLevel;
         try
         {
            noiseLevel = int.Parse(userEntry);
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Error Parsing Noise Level", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }

         for (int y=0; y<maxY; y++)
            for (int x=0; x<maxX; x++)
            {
               System.Drawing.Color clr;
               if (FloatingSelection != null)
                  clr = FloatingSelection.GetPixel(x,y);
               else
               {
                  if (SelectedRegion != null)
                  {
                     if (!SelectedRegion.IsVisible((float)x, (float)y))
                        continue;
                  }
                  clr = m_imgCurrentGraphic.GetPixel(x,y);
               }
               if (clr.A > 0)
               {
                  byte newR = clr.R, newG = clr.G, newB = clr.B;
                  int delta = rnd.Next(-noiseLevel,noiseLevel);
                  if (newR + delta <= byte.MaxValue)
                     if (newR + delta >= 0)
                        newR += (byte)delta;
                     else
                        newR = 0;
                  else
                     newR = byte.MaxValue;
                  if (newG + delta <= byte.MaxValue)
                     if (newG + delta >= 0)
                        newG += (byte)delta;
                     else
                        newG = 0;
                  else
                     newG = byte.MaxValue;
                  if (newB + delta <= byte.MaxValue)
                     if (newB + delta >= 0)
                        newB += (byte)delta;
                     else
                        newB = 0;
                  else
                     newB = byte.MaxValue;
                  clr = Color.FromArgb(clr.A, newR, newG, newB);
                  if (FloatingSelection != null)
                     FloatingSelection.SetPixel(x, y, clr);
                  else
                     m_imgCurrentGraphic.SetPixel(x, y, clr);
               }
            }
         m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
         m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
         ChildPane_GraphicChanged();      
      }

      private void HueRemap_Preview(object sender, EventArgs e)
      {
         Bitmap target, tmp;
         if (FloatingSelection != null)
         {
            target = (Bitmap)FloatingSelection.Clone();
            RemapHues((frmHueMapParams)sender, target);
            tmp = FloatingSelection;
            FloatingSelection = target;
            m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
            m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
            FloatingSelection = tmp;
            target.Dispose();
         }
         else
         {
            target = (Bitmap)m_imgCurrentGraphic.Clone();
            RemapHues((frmHueMapParams)sender, target);
            tmp = m_imgCurrentGraphic;
            m_imgCurrentGraphic = target;
            m_frmMagnify.DrawTransparentImage(m_imgCurrentGraphic, null);
            m_frmActual.DrawTransparentImage(m_imgCurrentGraphic, null);
            m_imgCurrentGraphic = tmp;
            target.Dispose();
         }
      }

      private void mnuZoomDecrease_Click(object sender, System.EventArgs e)
      {
         if (mnuZoom2x2.Checked || mnuZoom2.Checked)
            SelectTool(mnuActualSize);
         else if (mnuZoom4x4.Checked || mnuZoom4.Checked)
            SelectTool(mnuZoom2);
         else if (mnuZoom6x6.Checked || mnuZoom6.Checked)
            SelectTool(mnuZoom4);
         else if (mnuZoom8x8.Checked || mnuZoom8.Checked)
            SelectTool(mnuZoom6);
         else if (mnuZoom16x16.Checked || mnuZoom16.Checked)
            SelectTool(mnuZoom8);
      }

      private void mnuZoomIncrease_Click(object sender, System.EventArgs e)
      {
         if (mnuZoom1x1.Checked || mnuActualSize.Checked)
            SelectTool(mnuZoom2);
         else if (mnuZoom2x2.Checked || mnuZoom2.Checked)
            SelectTool(mnuZoom4);
         else if (mnuZoom4x4.Checked || mnuZoom4.Checked)
            SelectTool(mnuZoom6);
         else if (mnuZoom6x6.Checked || mnuZoom6.Checked)
            SelectTool(mnuZoom8);
         else if (mnuZoom8x8.Checked || mnuZoom8.Checked)
            SelectTool(mnuZoom16);
      }

      private void frmGraphicsEditor_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
      {
         switch(e.KeyChar)
         {
            case '-':
               mnuZoomDecrease_Click(sender, e);
               break;
            case '+':
               mnuZoomIncrease_Click(sender, e);
               break;
            case '[':
               mnuPenDecrease_Click(sender, e);
               break;
            case ']':
               mnuPenIncrease_Click(sender, e);
               break;
         }
      }

      private void mnuPenDecrease_Click(object sender, System.EventArgs e)
      {
         int curSize = 0;
         PenInfo newSel = new PenInfo(null, null, 0);
         foreach (PenInfo pi in m_arPens)
         {
            if (pi.Menu.Checked)
            {
               curSize = pi.PenSize;
               break;
            }
         }
         foreach (PenInfo pi in m_arPens)
         {
            if ((pi.PenSize < curSize) && ((newSel.Menu == null) || (pi.PenSize > newSel.PenSize)))
               newSel = pi;
         }
         SelectTool(newSel.Menu);
         m_frmActual.DrawCurrentToolState();
         m_frmMagnify.DrawCurrentToolState();
      }

      private void mnuPenIncrease_Click(object sender, System.EventArgs e)
      {
         int curSize = 0;
         PenInfo newSel = new PenInfo(null, null, 0);
         foreach (PenInfo pi in m_arPens)
         {
            if (pi.Menu.Checked)
            {
               curSize = pi.PenSize;
               break;
            }
         }
         foreach (PenInfo pi in m_arPens)
         {
            if ((pi.PenSize > curSize) && ((newSel.Menu == null) || (pi.PenSize < newSel.PenSize)))
               newSel = pi;
         }
         SelectTool(newSel.Menu);
         m_frmActual.DrawCurrentToolState();
         m_frmMagnify.DrawCurrentToolState();
      }

      private void mnuExportSheet_Click(object sender, System.EventArgs e)
      {
         using (System.Windows.Forms.SaveFileDialog dlgSave = new SaveFileDialog())
         {
            try
            {
               dlgSave.AddExtension = true;
               dlgSave.DefaultExt = "png";
               dlgSave.Filter = "*.PNG|*.PNG|All Files (*.*)|*.*";
               dlgSave.FilterIndex = 1;
               dlgSave.OverwritePrompt = true;
               dlgSave.ValidateNames = true;
               dlgSave.Title = "Export Graphic Sheet as Image";
               if (dlgSave.ShowDialog(this) == DialogResult.OK)
                  ProjectData.GetGraphicSheetImage(m_DataSource.Name, true).Save(dlgSave.FileName);
            }
            catch(System.Exception ex)
            {
               MessageBox.Show(this, ex.Message, "Export Graphic Sheet as Image", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
         }
      }

      private void mnuImportSheet_Click(object sender, System.EventArgs e)
      {
         using (System.Windows.Forms.OpenFileDialog dlgImport = new OpenFileDialog())
         {
            try
            {
               dlgImport.CheckFileExists=true;
               dlgImport.Filter = "*.PNG|*.PNG|All Files (*.*)|*.*";
               dlgImport.FilterIndex = 1;
               dlgImport.Multiselect = false;
               dlgImport.Title = "Import Graphic Sheet from Image";
               if (dlgImport.ShowDialog(this) == DialogResult.OK)
               {
                  Bitmap bmpImport = (Bitmap)Bitmap.FromFile(dlgImport.FileName);
                  m_frmCells.ReplaceSheet(bmpImport);
               }
            }
            catch(System.Exception ex)
            {
               MessageBox.Show(this, ex.Message, "Import Graphic Sheet from Image", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
         }
      }

      private void dataMonitor_GraphicSheetRowChanged(object sender, SGDK2.ProjectDataset.GraphicSheetRowChangeEvent e)
      {
         if ((e.Action == System.Data.DataRowAction.Change) && (e.Row == m_DataSource))
            Text = "Graphics Editor - " + e.Row.Name;
      }
      #endregion
   }
   public enum DrawingTool
   {
      None = 0,
      FreeDraw,
      FreeLine,
      Bezier,
      Line,
      Rectangle,
      Ellipse,
      Erase,
      SelRect,
      SelFree,
      Translate,
      Rotate,
      Scale,
      Dropper,
      FloodFill,
      FloodSel,
      GradientFill,
      AirBrush,
      Smooth,
      Custom
   }

   [System.Flags()]
   public enum ToolOptions
   {
      AntiAlias=1,
      Outline=2,
      Fill=4,
      Lock=8,
      GradientFill=16,
      DisjointedColors=32
   }
}
