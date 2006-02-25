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
using System.Drawing.Drawing2D;

namespace SGDK2
{
	/// <summary>
	/// Implements a frame that performs drawing operations on a bitmap when clicked
	/// </summary>
	public class frmGraphicPane : System.Windows.Forms.Form
	{
      #region Public members
      /// <summary>
      /// Stores the current graphic in its persisted state
      /// </summary>
      public Bitmap Image;
      /// <summary>
      /// Shows the proposed result as the user drags a tool.
      /// </summary>
      public Bitmap TempImage;
      /// <summary>
      /// Determines whether this view should draw a grid when magnified.
      /// </summary>
      public Boolean ShowGrid;
      #endregion

      #region Events
      public delegate void ViewChangedEvent();
      public event ViewChangedEvent ViewChanged;
      public delegate void GraphicChangedEvent();
      public event GraphicChangedEvent GraphicChanged;
      #endregion

      #region Private members
      private PointF DragStart = new Point(-1,-1);
      frmGraphicsEditor ParentEditor
      {
         get
         {
            return (frmGraphicsEditor)Parent;
         }
      }
      private Cursor m_DrawCursor = null;
      private Cursor m_RotateCursor = null;
      private Cursor m_DropperCursor = null;
      private Cursor m_FloodFillCursor = null;
      private Cursor m_FloodSelCursor = null;
      private System.Random m_Random = new System.Random();
      #endregion

      #region Initialization and clean-up

      private frmGraphicPane()
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();
      }

      /// <summary>
      /// Creates a new instance of a graphic editing pane
      /// </summary>
      /// <param name="frmParent">Parent form that will contain this pane</param>
      /// <param name="imgEdit">Image to edit in this pane</param>
      /// <param name="nScaleFactor">1=actual size, 2=2x2 magnification, 3=3x3 magnification, etc.</param>
      public frmGraphicPane(frmGraphicsEditor frmParent, Bitmap imgEdit, int nScaleFactor) : this()
      {
         this.ResizeRedraw = true;
         this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
         this.SetStyle(ControlStyles.UserPaint, true);
         this.Image = imgEdit;
         this.TopLevel = false;
         this.Parent = frmParent;

         this.Cursor = m_DrawCursor = SGDK2IDE.LoadCursor("Draw.cur");
         m_RotateCursor = SGDK2IDE.LoadCursor("Rotate.cur");
         m_DropperCursor = SGDK2IDE.LoadCursor("Dropper.cur");
         m_FloodFillCursor = SGDK2IDE.LoadCursor("FloodFill.cur");
         m_FloodSelCursor = SGDK2IDE.LoadCursor("FloodSel.cur");
         
         this.Text = String.Format("{0}%", nScaleFactor * 100);

         this.ClientSize = new Size(imgEdit.Width * nScaleFactor, imgEdit.Height * nScaleFactor);
         if (this.Top < frmParent.UsableClientRect.Top)
            this.Top = frmParent.UsableClientRect.Top;
         if (this.Left < ParentEditor.UsableClientRect.Left)
            this.Left = ParentEditor.UsableClientRect.Left;
      }

      /// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
         m_DrawCursor.Dispose();
         m_RotateCursor.Dispose();
         if (this.TempImage != null)
            this.TempImage.Dispose();
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
         // 
         // frmGraphicPane
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(216, 176);
         this.ControlBox = false;
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
         this.Name = "frmGraphicPane";
         this.ShowInTaskbar = false;
         this.Text = "Graphics Editor";
         this.Resize += new System.EventHandler(this.frmGraphicPane_Resize);
         this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmGraphicsEditor_MouseDown);
         this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmGraphicsEditor_MouseUp);
         this.Move += new System.EventHandler(this.frmGraphicPane_Move);
         this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmGraphicPane_Paint);
         this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmGraphicsEditor_MouseMove);
         this.MouseLeave += new System.EventHandler(this.frmGraphicPane_MouseLeave);

      }
		#endregion

      #region Event Handlers
      private void frmGraphicsEditor_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         try
         {
            PointF DragEnd = ConvertMouseCoordsToPoint(e.X, e.Y);
            GraphicsPath pathTemp = null;

            switch(ParentEditor.CurrentTool)
            {
               case DrawingTool.Bezier:
                  if (e.Button == MouseButtons.Right)
                  {
                     if (ParentEditor.FreehandPoints != null)
                     {
                        ParentEditor.IsDragging = false;
                        AcceptDrawing();
                     }
                  }
                  else
                  {
                     DragStart = ConvertMouseCoordsToPoint(e.X, e.Y);
                     if (ParentEditor.FreehandPoints == null)
                        ParentEditor.FreehandPoints = new PointF[] {DragStart, DragStart};
                     else
                     {
                        DragStart = ParentEditor.FreehandPoints[((ParentEditor.FreehandPoints.Length-2) / 3) * 3 + 1];
                        AppendFreehandPoint(DragStart);
                     }
                  }
                  break;
               case DrawingTool.SelRect:
                  ParentEditor.ActivateTool(DrawingTool.Translate);
                  break;
               case DrawingTool.SelFree:
                  if (e.Button == MouseButtons.Right)
                  {
                     AppendSelectionPoint(ParentEditor.SelectionOutline[0]);
                     pathTemp = GetSelectionPath();
                     if (ParentEditor.SelectedRegion != null)
                        ParentEditor.SelectedRegion.Dispose();
                     ParentEditor.SelectedRegion = new Region(pathTemp);
                     pathTemp.Dispose();
                     ParentEditor.ActivateTool(DrawingTool.Translate);
                  }
                  break;
               case DrawingTool.Translate:
               case DrawingTool.Dropper:
               case DrawingTool.FloodSel:
                  ParentEditor.IsDragging = false;
                  break;
               case DrawingTool.FreeLine:
                  break;
               case DrawingTool.Rotate:
               case DrawingTool.Scale:
                  if (ParentEditor.TempTransform != null)
                  {
                     ParentEditor.SelectionTransform.Multiply(ParentEditor.TempTransform);
                     ParentEditor.TempTransform.Dispose();
                     ParentEditor.TempTransform = null;
                  }
                  ParentEditor.IsDragging = false;
                  break;
               case DrawingTool.FloodFill:
               {
                  Region rgnFlood;
                  rgnFlood = GetFloodRegion(Image, Point.Round(DragStart), Point.Round(DragEnd));
                  Graphics gTemp = Graphics.FromImage(TempImage);
                  gTemp.CompositingMode = CompositingMode.SourceCopy;
                  gTemp.FillRegion(ParentEditor.CurrentBrush, rgnFlood);
                  gTemp.Dispose();
                  rgnFlood.Dispose();
                  AcceptDrawing();
                  ParentEditor.IsDragging = false;
               }
                  break;
               case DrawingTool.Line:
               case DrawingTool.Rectangle:
               case DrawingTool.Ellipse:
               case DrawingTool.AirBrush:
               case DrawingTool.FreeDraw:
                  if (e.Button == MouseButtons.Right)
                  {
                     this.Cursor = m_DrawCursor;
                     if ((DragEnd.X < Image.Width) && (DragEnd.Y < Image.Height) &&
                        (DragEnd.X > 0) && (DragEnd.Y > 0))
                        ParentEditor.PickColor(Image.GetPixel((int)DragEnd.X, (int)DragEnd.Y));
                     break;
                  }
                  goto default;
               default:
                  ParentEditor.IsDragging = false;
                  AcceptDrawing();
                  break;
            }
         }
         catch(Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void frmGraphicsEditor_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         try
         {
            DragStart = ConvertMouseCoordsToPoint(e.X, e.Y);

            ParentEditor.IsDragging = true;

            switch (ParentEditor.CurrentTool)
            {
               case DrawingTool.Bezier:
                  if (ParentEditor.FreehandPoints != null)
                  {
                     if (ParentEditor.FreehandPoints.Length > 1)
                        DragStart = ParentEditor.FreehandPoints[((ParentEditor.FreehandPoints.Length-2) / 3) * 3 + 1];
                     else
                        DragStart = ParentEditor.FreehandPoints[0];
                  }
                  break;
               case DrawingTool.FreeLine:
                  if (e.Button == MouseButtons.Right)
                  {
                     if (ParentEditor.FreehandPoints != null)
                     {
                        ParentEditor.IsDragging = true;
                        AcceptDrawing();
                     }
                  }
                  else
                  {
                     if (ParentEditor.FreehandPoints == null)
                        ParentEditor.FreehandPoints = new PointF[] {DragStart, DragStart};
                     else
                     {
                        AppendFreehandPoint(DragStart);
                        DragStart = ParentEditor.FreehandPoints[ParentEditor.FreehandPoints.Length - 2];
                     }
                  }
                  break;
               case DrawingTool.SelFree:
                  if (e.Button != MouseButtons.Right)
                  {
                     if ((ParentEditor.SelectionOutline == null) || (ParentEditor.SelectedRegion != null))
                     {
                        if (ParentEditor.FloatingSelection != null)
                           ParentEditor.DropFloatingSelection();
                        if (ParentEditor.SelectionTransform != null)
                        {
                           ParentEditor.SelectionTransform.Dispose();
                           ParentEditor.SelectionTransform = null;
                        }
                        ParentEditor.SelectionOutline = new PointF[] {DragStart, DragStart};
                        if (ParentEditor.SelectedRegion != null)
                        {
                           ParentEditor.SelectedRegion.Dispose();
                           ParentEditor.SelectedRegion = null;
                        }
                     }
                     else
                        AppendSelectionPoint(DragStart);
                  }
                  break;
               case DrawingTool.SelRect:
                  if (ParentEditor.FloatingSelection != null)
                     ParentEditor.DropFloatingSelection();
                  if (ParentEditor.SelectionTransform != null)
                  {
                     ParentEditor.SelectionTransform.Dispose();
                     ParentEditor.SelectionTransform = null;
                  }
                  break;
               case DrawingTool.Dropper:
                  if ((DragStart.X < Image.Width) && (DragStart.Y < Image.Height) &&
                     (DragStart.X > 0) && (DragStart.Y > 0))
                     ParentEditor.PickColor(Image.GetPixel((int)DragStart.X, (int)DragStart.Y));
                  break;
               case DrawingTool.FloodFill:
               {
                  if (ParentEditor.HighlightBrush != null)
                  {
                     Region rgnFlood;
                     ResetTempImage();
                     Graphics gTemp = Graphics.FromImage(TempImage);
                     rgnFlood = GetFloodRegion(Image, Point.Round(DragStart), Point.Round(DragStart));
                     gTemp.CompositingMode = CompositingMode.SourceOver;
                     gTemp.FillRegion(ParentEditor.HighlightBrush, rgnFlood);
                     rgnFlood.Dispose();
                     DrawTransparentImage(TempImage, null);
                  }
               }
                  break;
               case DrawingTool.FloodSel:
                  if (ParentEditor.SelectedRegion != null)
                     ParentEditor.SelectedRegion.Dispose();
                  ParentEditor.SelectedRegion = GetFloodRegion(Image, Point.Round(DragStart), Point.Round(DragStart));
                  DrawTransparentImage(Image, null);
                  break;
               case DrawingTool.Line:
               case DrawingTool.Rectangle:
               case DrawingTool.Ellipse:
               case DrawingTool.AirBrush:
               case DrawingTool.FreeDraw:
                  if (e.Button == MouseButtons.Right)
                     this.Cursor = m_DropperCursor;
                  break;
            }
         }
         catch(Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void frmGraphicsEditor_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         try
         {
            ResetTempImage();
            PointF DragEnd = ConvertMouseCoordsToPoint(e.X, e.Y);

            Graphics g = Graphics.FromImage(TempImage);
            ParentEditor.InitGraphicsSettings(g);

            this.Text = GetScaleFactorString() + " " + DragEnd.X.ToString() + "," + DragEnd.Y.ToString();

            if ((e.Button > 0) || ((ParentEditor.FreehandPoints != null) &&
               ((ParentEditor.CurrentTool == DrawingTool.Bezier) ||
               (ParentEditor.CurrentTool == DrawingTool.FreeLine))) ||
               ((ParentEditor.SelectionOutline != null) && (ParentEditor.CurrentTool == DrawingTool.SelFree)))
            {
               PointF LT = new PointF(DragStart.X < DragEnd.X ? DragStart.X : DragEnd.X,
                  DragStart.Y < DragEnd.Y ? DragStart.Y : DragEnd.Y);
               PointF RB = new PointF(DragStart.X > DragEnd.X ? DragStart.X : DragEnd.X,
                  DragStart.Y > DragEnd.Y ? DragStart.Y : DragEnd.Y);
               RectangleF rcCur = RectangleF.FromLTRB( LT.X, LT.Y, RB.X, RB.Y);
               float dist = (float)Math.Sqrt((DragStart.X - DragEnd.X) * (DragStart.X - DragEnd.X) +
                  (DragStart.Y - DragEnd.Y) * (DragStart.Y - DragEnd.Y));

               switch (ParentEditor.CurrentTool)
               {
                  case DrawingTool.Bezier:
                  case DrawingTool.FreeLine:
                  case DrawingTool.Line:
                  case DrawingTool.GradientFill:
                     if (0 != (ParentEditor.CurrentOptions & ToolOptions.Lock))
                     {
                        if (Math.Abs(DragEnd.Y - DragStart.Y) < 1)
                        {
                           DragEnd.Y = DragStart.Y;
                        }
                        else
                        {
                           float fRatio = (DragEnd.X - DragStart.X) / (DragEnd.Y - DragStart.Y);
                           if (Math.Abs(fRatio) < 0.25f)
                              DragEnd.X = DragStart.X;
                           else if (Math.Abs(fRatio) < 0.75f)
                              DragEnd.X = DragStart.X + (DragEnd.Y - DragStart.Y) * Math.Sign(fRatio) / 2f;
                           else if (Math.Abs(fRatio) < 1.5f)
                              DragEnd.X = DragStart.X + (DragEnd.Y - DragStart.Y) * Math.Sign(fRatio);
                           else if (Math.Abs(fRatio) < 4f)
                              DragEnd.Y = DragStart.Y + (DragEnd.X - DragStart.X) * Math.Sign(fRatio) / 2f;
                           else
                              DragEnd.Y = DragStart.Y;
                        }
                     }
                     break;
               }

               switch(ParentEditor.CurrentTool)
               {
                  case DrawingTool.Line:
                     if (e.Button == MouseButtons.Right)
                     {
                        if ((DragEnd.X < Image.Width) && (DragEnd.Y < Image.Height) &&
                           (DragEnd.X > 0) && (DragEnd.Y > 0))
                           ParentEditor.PickColor(Image.GetPixel((int)DragEnd.X, (int)DragEnd.Y));
                        break;
                     }
                     g.DrawLine(ParentEditor.CurrentPen, DragStart, DragEnd);
                     break;
                  case DrawingTool.Rectangle:
                     if (e.Button == MouseButtons.Right)
                     {
                        if ((DragEnd.X < Image.Width) && (DragEnd.Y < Image.Height) &&
                           (DragEnd.X > 0) && (DragEnd.Y > 0))
                           ParentEditor.PickColor(Image.GetPixel((int)DragEnd.X, (int)DragEnd.Y));
                        break;
                     }
                     if (0 != (ParentEditor.CurrentOptions & ToolOptions.Lock))
                        if (rcCur.Width > rcCur.Height)
                           rcCur.Height = rcCur.Width;
                        else
                           rcCur.Width = rcCur.Height;
                     if (0 != (ParentEditor.CurrentOptions & ToolOptions.Fill))
                     {
                        if (0 != (ParentEditor.CurrentOptions & ToolOptions.GradientFill))
                        {
                           if ((rcCur.Width > 1) && (rcCur.Height > 1))
                           {
                              GraphicsPath gpRect = new GraphicsPath();
                              gpRect.AddRectangle(rcCur);
                              PathGradientBrush pgb = new PathGradientBrush(gpRect);
                              if (ParentEditor.CurrentBrush is SolidBrush)
                              {
                                 pgb.CenterColor = ((SolidBrush)ParentEditor.CurrentBrush).Color;
                                 pgb.SurroundColors = new Color[] {ParentEditor.CurrentPen.Color};
                                 g.FillPath(pgb, gpRect);
                              }
                              else
                                 g.FillRectangle(ParentEditor.CurrentBrush, rcCur);
                              gpRect.Dispose();
                              pgb.Dispose();
                           }
                        }
                        else
                           g.FillRectangle(ParentEditor.CurrentBrush, rcCur);
                     }
                     if (0 != (ParentEditor.CurrentOptions & ToolOptions.Outline))
                        g.DrawRectangle(ParentEditor.CurrentPen, rcCur.Left, rcCur.Top, rcCur.Width, rcCur.Height);
                     break;
                  case DrawingTool.Ellipse:
                     if (e.Button == MouseButtons.Right)
                     {
                        if ((DragEnd.X < Image.Width) && (DragEnd.Y < Image.Height) &&
                           (DragEnd.X > 0) && (DragEnd.Y > 0))
                           ParentEditor.PickColor(Image.GetPixel((int)DragEnd.X, (int)DragEnd.Y));
                        break;
                     }
                     if (0 != (ParentEditor.CurrentOptions & ToolOptions.Lock))
                        if (rcCur.Width > rcCur.Height)
                           rcCur.Height = rcCur.Width;
                        else
                           rcCur.Width = rcCur.Height;
                     if (0 != (ParentEditor.CurrentOptions & ToolOptions.Fill))
                     {
                        if (0 != (ParentEditor.CurrentOptions & ToolOptions.GradientFill))
                        {
                           if ((rcCur.Width > 1) && (rcCur.Height > 1))
                           {
                              GraphicsPath gpEllipse = new GraphicsPath();
                              gpEllipse.AddEllipse(rcCur);
                              PathGradientBrush pgb = new PathGradientBrush(gpEllipse);
                              if (ParentEditor.CurrentBrush is SolidBrush)
                              {
                                 pgb.CenterColor = ((SolidBrush)ParentEditor.CurrentBrush).Color;
                                 pgb.SurroundColors = new Color[] {ParentEditor.CurrentPen.Color};
                                 g.FillPath(pgb, gpEllipse);
                              }
                              else
                                 g.FillEllipse(ParentEditor.CurrentBrush, rcCur);
                              gpEllipse.Dispose();
                              pgb.Dispose();
                           }
                        }
                        else
                           g.FillEllipse(ParentEditor.CurrentBrush, rcCur);
                     }
                     if (0 != (ParentEditor.CurrentOptions & ToolOptions.Outline))
                        g.DrawEllipse(ParentEditor.CurrentPen, rcCur);
                     break;
                  case DrawingTool.FreeDraw:
                     if (e.Button == MouseButtons.Right)
                     {
                        if ((DragEnd.X < Image.Width) && (DragEnd.Y < Image.Height) &&
                           (DragEnd.X > 0) && (DragEnd.Y > 0))
                           ParentEditor.PickColor(Image.GetPixel((int)DragEnd.X, (int)DragEnd.Y));
                        break;
                     }
                     goto case DrawingTool.FreeLine;
                  case DrawingTool.FreeLine:
                  case DrawingTool.Erase:
                     Cursor = m_DrawCursor;
                     if ((e.Button == MouseButtons.Left) && ((dist >= 1) || (ParentEditor.FreehandPoints == null) ||
                        (ParentEditor.CurrentTool != DrawingTool.FreeLine)))
                     {
                        AppendFreehandPoint(DragEnd);
                        DragStart = DragEnd;
                     }
                     else if (ParentEditor.FreehandPoints != null)
                     {
                        ParentEditor.FreehandPoints[ParentEditor.FreehandPoints.Length - 1] = DragEnd;
                     }
                     if (ParentEditor.FreehandPoints != null)
                     {
                        if (DrawingTool.Erase == ParentEditor.CurrentTool)
                        {
                           Pen penErase = new Pen(Color.Transparent, ParentEditor.CurrentPen.Width);
                           penErase.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Flat);
                           penErase.LineJoin = LineJoin.Round;
                           g.CompositingMode = CompositingMode.SourceCopy;
                           g.SmoothingMode = SmoothingMode.None;
                           g.DrawLines(penErase, ParentEditor.FreehandPoints);
                           penErase.Dispose();
                        }
                        else
                        {
                           if (0 != (ParentEditor.CurrentOptions & ToolOptions.Fill))
                           {
                              if ((ParentEditor.CurrentBrush is SolidBrush) && (0 != (ParentEditor.CurrentOptions & ToolOptions.GradientFill)))
                              {
                                 if (IsSelectionOutlineLongEnoughToDraw(ParentEditor.FreehandPoints))
                                 {
                                    GraphicsPath gpTemp = new GraphicsPath();
                                    gpTemp.AddLines(ParentEditor.FreehandPoints);
                                    gpTemp.CloseAllFigures();
                                    PathGradientBrush pgb = new PathGradientBrush(gpTemp);
                                    pgb.CenterColor = ((SolidBrush)ParentEditor.CurrentBrush).Color;
                                    pgb.SurroundColors = new Color[] {ParentEditor.CurrentPen.Color};
                                    g.FillPath(pgb, gpTemp);
                                    pgb.Dispose();
                                    gpTemp.Dispose();
                                 }
                              }
                              else
                                 g.FillPolygon(ParentEditor.CurrentBrush, ParentEditor.FreehandPoints);
                           }
                           if (0 != (ParentEditor.CurrentOptions & ToolOptions.Outline))
                           {
                              if (0 != (ParentEditor.CurrentOptions & ToolOptions.Fill))
                                 g.DrawPolygon(ParentEditor.CurrentPen, ParentEditor.FreehandPoints);
                              else
                                 g.DrawLines(ParentEditor.CurrentPen, ParentEditor.FreehandPoints);
                           }
                        }
                     }
                     break;
                  case DrawingTool.Bezier:
                     Cursor = m_DrawCursor;
                     if (ParentEditor.FreehandPoints != null)
                     {
                        ParentEditor.FreehandPoints[ParentEditor.FreehandPoints.Length - 1] = DragEnd;
                        GraphicsPath gpTemp = GetBezierPath();
                        if (0 != (ParentEditor.CurrentOptions & ToolOptions.Fill))
                        {
                           gpTemp.CloseAllFigures();
                           if ((ParentEditor.CurrentBrush is SolidBrush) && (0 != (ParentEditor.CurrentOptions & ToolOptions.GradientFill)))
                           {
                              if (IsSelectionOutlineLongEnoughToDraw(ParentEditor.FreehandPoints))
                              {
                                 PathGradientBrush pgb = new PathGradientBrush(gpTemp);
                                 pgb.CenterColor = ((SolidBrush)ParentEditor.CurrentBrush).Color;
                                 pgb.SurroundColors = new Color[] {ParentEditor.CurrentPen.Color};
                                 g.FillPath(pgb, gpTemp);
                                 pgb.Dispose();
                              }
                           }
                           else
                              g.FillPath(ParentEditor.CurrentBrush, gpTemp);
                        }
                        if (0 != (ParentEditor.CurrentOptions & ToolOptions.Outline))
                           g.DrawPath(ParentEditor.CurrentPen, gpTemp);

                        gpTemp.Dispose();
                     }
                     break;
                  case DrawingTool.SelRect:
                     if (ParentEditor.SelectedRegion != null)
                        ParentEditor.SelectedRegion.Dispose();
                     ParentEditor.SelectedRegion = new Region(rcCur);
                     ParentEditor.SelectionOutline = new PointF[] { new PointF(rcCur.X, rcCur.Y),
                                                                     new PointF(rcCur.X+rcCur.Width, rcCur.Y),
                                                                     new PointF(rcCur.X+rcCur.Width, rcCur.Y + rcCur.Height),
                                                                     new PointF(rcCur.X, rcCur.Y + rcCur.Height), 
                                                                     new PointF(rcCur.X, rcCur.Y) };
                     break;
                  case DrawingTool.SelFree:
                     Cursor = m_DrawCursor;
                     if ((e.Button == MouseButtons.Left) && ((dist >= 1) || (ParentEditor.SelectionOutline == null)))
                     {
                        AppendSelectionPoint(DragEnd);
                        DragStart = DragEnd;
                     }
                     else if ((ParentEditor.SelectionOutline != null) && (ParentEditor.SelectedRegion == null))
                     {
                        ParentEditor.SelectionOutline[ParentEditor.SelectionOutline.Length - 1] = DragEnd;
                     }
                     break;
                  case DrawingTool.Translate:
                     if (ParentEditor.TempTransform != null)
                        ParentEditor.TempTransform.Dispose();
                     if (ParentEditor.FloatingSelection == null)
                        FloatSelection();
                     if (ParentEditor.SelectionTransform == null)
                        ParentEditor.SelectionTransform = new Matrix();
                     ParentEditor.SelectionTransform.Translate(DragEnd.X - DragStart.X,
                        DragEnd.Y - DragStart.Y, MatrixOrder.Append);
                     DragStart = DragEnd;
                     break;
                  case DrawingTool.Rotate:
                     if (ParentEditor.TempTransform != null)
                        ParentEditor.TempTransform.Dispose();
                     if (ParentEditor.FloatingSelection == null)
                        FloatSelection();
                     if (ParentEditor.SelectionTransform == null)
                        ParentEditor.SelectionTransform = new Matrix();
                     ParentEditor.TempTransform = new Matrix();
                     float fRotate = 2 * (DragEnd.Y - DragStart.Y + DragEnd.X - DragStart.X);
                     if (0 != (ParentEditor.CurrentOptions & ToolOptions.Lock))
                        fRotate = (float)(Math.Round(fRotate * 24f / 360f) * 360f / 24f);
                     ParentEditor.TempTransform.RotateAt(fRotate,
                        new PointF(ParentEditor.FloatingSelection.Width / 2f,
                        ParentEditor.FloatingSelection.Height / 2f), MatrixOrder.Prepend);
                     break;
                  case DrawingTool.Scale:
                     if (ParentEditor.TempTransform != null)
                        ParentEditor.TempTransform.Dispose();
                     if (ParentEditor.FloatingSelection == null)
                        FloatSelection();
                     if (ParentEditor.SelectionTransform == null)
                        ParentEditor.SelectionTransform = new Matrix();
                     ParentEditor.TempTransform = new Matrix();
                     float ScaleX = 1 + (DragEnd.X - DragStart.X) * 2 / Image.Width;
                     if (Math.Abs(ScaleX) < .1) ScaleX = .1f;
                     float ScaleY = 1 + (DragEnd.Y - DragStart.Y) * 2 / Image.Height;
                     if (Math.Abs(ScaleY) < .1) ScaleY = .1f;
                     if (0 != (ParentEditor.CurrentOptions & ToolOptions.Lock))
                        if (ScaleX > ScaleY)
                           ScaleY = ScaleX;
                        else
                           ScaleX = ScaleY;
                     ParentEditor.TempTransform.Scale(ScaleX, ScaleY);
                     break;
                  case DrawingTool.Dropper:
                     Cursor = m_DropperCursor;
                     if ((DragEnd.X < Image.Width) && (DragEnd.Y < Image.Height) &&
                        (DragEnd.X > 0) && (DragEnd.Y > 0))
                        ParentEditor.PickColor(Image.GetPixel((int)DragEnd.X, (int)DragEnd.Y));
                     break;
                  case DrawingTool.FloodFill:
                  {
                     if (ParentEditor.HighlightBrush != null)
                     {
                        Region rgnFlood;
                        rgnFlood = GetFloodRegion(Image, Point.Round(DragStart), Point.Round(DragEnd));
                        g.CompositingMode = CompositingMode.SourceOver;
                        g.FillRegion(ParentEditor.HighlightBrush, rgnFlood);
                        rgnFlood.Dispose();
                     }
                  }
                     break;
                  case DrawingTool.FloodSel:
                     if (ParentEditor.SelectedRegion != null)
                        ParentEditor.SelectedRegion.Dispose();
                     ParentEditor.SelectedRegion = GetFloodRegion(Image, Point.Round(DragStart), Point.Round(DragEnd));
                     break;
                  case DrawingTool.GradientFill:
                     if (dist > 1)
                     {
                        if (ParentEditor.CurrentBrush != null)
                           ParentEditor.CurrentBrush.Dispose();
                        ParentEditor.CurrentBrush = new LinearGradientBrush(DragStart, DragEnd, ParentEditor.ctlColorSel.m_BrushColor, ParentEditor.ctlColorSel.m_PenColor);
                        ((LinearGradientBrush)ParentEditor.CurrentBrush).WrapMode = WrapMode.TileFlipXY;
                        if (ParentEditor.SelectedRegion != null)
                           g.FillRegion(ParentEditor.CurrentBrush, ParentEditor.SelectedRegion);
                        else
                           g.FillRectangle(ParentEditor.CurrentBrush, -.5f, -.5f, Image.Width + 1, Image.Height + 1);
                     }
                     break;
                  case DrawingTool.AirBrush:
                     if (e.Button == MouseButtons.Right)
                     {
                        if ((DragEnd.X < Image.Width) && (DragEnd.Y < Image.Height) &&
                           (DragEnd.X > 0) && (DragEnd.Y > 0))
                           ParentEditor.PickColor(Image.GetPixel((int)DragEnd.X, (int)DragEnd.Y));
                        break;
                     }
                  {
                     float fAngle = (float)(m_Random.NextDouble() * Math.PI * 2);
                     float fDist = (float)(m_Random.NextDouble() * ParentEditor.CurrentPen.Width / 2);
                     PointF ptRand = new PointF((float)(DragEnd.X + Math.Cos(fAngle) * fDist), (float)(DragEnd.Y + Math.Sin(fAngle) * fDist));
                     g.FillRectangle(ParentEditor.CurrentBrush, ptRand.X - .49f, ptRand.Y - .49f, 1, 1);
                     SGDK2IDE.CopyImage(Image, TempImage);
                  }
                     break;
                  case DrawingTool.Smooth:
                     Blur(TempImage, new Rectangle((int)(DragEnd.X - ParentEditor.CurrentPen.Width / 2),
                        (int)(DragEnd.Y - ParentEditor.CurrentPen.Width / 2), (int)(ParentEditor.CurrentPen.Width),
                        (int)(ParentEditor.CurrentPen.Width)));
                     SGDK2IDE.CopyImage(Image,TempImage);
                     break;
                  case DrawingTool.Custom:
                     CustTool.DrawTool(ParentEditor.m_CustomTool, g, DragStart, DragEnd, ParentEditor.CurrentPen, ParentEditor.CurrentBrush, ParentEditor.CurrentOptions);
                     break;
               }
            }
            else
            {
               switch(ParentEditor.CurrentTool)
               {
                  case DrawingTool.SelFree:
                  case DrawingTool.SelRect:
                     Cursor = m_DrawCursor;
                     break;
                  case DrawingTool.Translate:
                     Cursor = Cursors.SizeAll;
                     break;
                  case DrawingTool.Rotate:
                     Cursor = m_RotateCursor;
                     break;
                  case DrawingTool.Scale:
                     Cursor = Cursors.SizeNWSE;
                     break;
                  case DrawingTool.Dropper:
                     Cursor = m_DropperCursor;
                     break;
                  case DrawingTool.FloodFill:
                     Cursor = m_FloodFillCursor;
                     g.FillRectangle(ParentEditor.CurrentBrush, DragEnd.X - .5f, DragEnd.Y - .5f, 1, 1);
                     break;
                  case DrawingTool.FloodSel:
                     Cursor = m_FloodSelCursor;
                     break;
                  case DrawingTool.AirBrush:
                     Cursor = m_DrawCursor;
                     g.FillEllipse(ParentEditor.CurrentBrush, DragEnd.X - ParentEditor.CurrentPen.Width / 2.0f,
                        DragEnd.Y - ParentEditor.CurrentPen.Width / 2.0f, ParentEditor.CurrentPen.Width,
                        ParentEditor.CurrentPen.Width);
                     break;
                  case DrawingTool.Smooth:
                     Cursor = m_DrawCursor;
                     for (int nIdx = 0; nIdx < 3; nIdx++)
                        Blur(TempImage, new Rectangle((int)(DragEnd.X - ParentEditor.CurrentPen.Width / 2),
                           (int)(DragEnd.Y - ParentEditor.CurrentPen.Width / 2), (int)(ParentEditor.CurrentPen.Width),
                           (int)(ParentEditor.CurrentPen.Width)));
                     break;
                  default:
                     Cursor = m_DrawCursor;
                     if ((ParentEditor.CurrentTool == DrawingTool.Line) ||
                        (ParentEditor.CurrentTool == DrawingTool.Erase) ||
                        (0 != (ParentEditor.CurrentOptions & ToolOptions.Outline)))
                     {
                        GraphicsPath gpTemp = new GraphicsPath( new PointF[]
                        {
                           new PointF(DragEnd.X, DragEnd.Y - .49f),
                           new PointF(DragEnd.X, DragEnd.Y + .5f),
                           new PointF(DragEnd.X - .49f, DragEnd.Y),
                           new PointF(DragEnd.X + .5f, DragEnd.Y)
                        }, new byte[]
                        {
                           (byte)PathPointType.Start,
                           (byte)PathPointType.Line,
                           (byte)PathPointType.Start,
                           (byte)PathPointType.Line
                        });

                        if (ParentEditor.CurrentTool == DrawingTool.Erase)
                        {
                           Pen penErase = new Pen(Color.Transparent, ParentEditor.CurrentPen.Width);
                           penErase.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Flat);
                           g.CompositingMode = CompositingMode.SourceCopy;
                           g.SmoothingMode = SmoothingMode.None;
                           g.DrawPath(penErase, gpTemp);
                           penErase.Dispose();
                        }
                        else
                           g.DrawPath(ParentEditor.CurrentPen, gpTemp);
                        gpTemp.Dispose();
                     }
                     else
                     {
                        g.FillRectangle(ParentEditor.CurrentBrush, DragEnd.X - .5f, DragEnd.Y - .5f, 1f, 1f);
                     }
                     break;
               }
            }
            g.Dispose();
            DrawTransparentImage(TempImage, null);
            if (ViewChanged != null)
               ViewChanged();
         }
         catch(Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void frmGraphicPane_MouseLeave(object sender, System.EventArgs e)
      {
         try
         {
            ResetTempImage();      
            DrawTransparentImage(TempImage, null);
            if (ViewChanged != null)
               ViewChanged();
         }
         catch(Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void frmGraphicPane_Move(object sender, System.EventArgs e)
      {
         int nLeft, nTop, nWidth, nHeight;
         try
         {
            nLeft = Left;
            nTop = Top;
            nWidth = Width;
            nHeight = Height;
            if (nLeft + nWidth > ParentEditor.UsableClientRect.Right)
               nLeft = ParentEditor.UsableClientRect.Right - nWidth;
            if (nTop + nHeight > ParentEditor.UsableClientRect.Bottom)
               nTop = ParentEditor.UsableClientRect.Bottom - nHeight;
            if (nLeft < ParentEditor.UsableClientRect.Left)
               nLeft = ParentEditor.UsableClientRect.Left;
            if (nTop < ParentEditor.UsableClientRect.Top)
               nTop = ParentEditor.UsableClientRect.Top;
            SetBounds(nLeft, nTop, nWidth, nHeight, BoundsSpecified.Location);
         }
         catch(Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void frmGraphicPane_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         try
         {
            DrawTransparentImage(Image,e.Graphics);
         }
         catch(Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void frmGraphicPane_Resize(object sender, System.EventArgs e)
      {
         try
         {
            this.Text = GetScaleFactorString();
         }
         catch(Exception ex)
         {
            MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      #endregion

      #region Private methods
      private string GetScaleFactorString()
      {
         int nHScale = this.ClientSize.Width * 100 / Image.Width;
         return String.Format("{0}%", nHScale);
      }

      private void ResetTempImage()
      {
         if (TempImage == null)
            TempImage = (Bitmap)Image.Clone();
         else
            SGDK2IDE.CopyImage(TempImage, Image);
      }

      private void AppendSelectionPoint(PointF pt)
      {
         if (ParentEditor.SelectionOutline== null)
         {
            ParentEditor.SelectionOutline = new PointF[2] {DragStart, pt};
         }
         else
         {
            PointF[] newcopy;
            newcopy = new PointF[ParentEditor.SelectionOutline.Length + 1];
            ParentEditor.SelectionOutline.CopyTo(newcopy,0);
            newcopy[ParentEditor.SelectionOutline.Length] = pt;
            ParentEditor.SelectionOutline = newcopy;
         }
      }

      private void AppendFreehandPoint(PointF pt)
      {
         if (ParentEditor.FreehandPoints == null)
         {
            ParentEditor.FreehandPoints = new PointF[2] {DragStart, pt};
         }
         else
         {
            PointF[] newcopy;
            newcopy = new PointF[ParentEditor.FreehandPoints.Length + 1];
            ParentEditor.FreehandPoints.CopyTo(newcopy,0);
            newcopy[ParentEditor.FreehandPoints.Length] = pt;
            ParentEditor.FreehandPoints = newcopy;
         }
      }

      private GraphicsPath GetBezierPath()
      {
         int nPointCount = ((ParentEditor.FreehandPoints.Length+1)/3) * 3 + 1;
         int nNewIdx;
         PointF[] arpt = new PointF[nPointCount];
         byte[] arby = new byte[nPointCount];

         for(int i = 0; i<nPointCount; i++)
         {
            arby[i] = (byte)PathPointType.Bezier;
            if (i>0)
            {
               nNewIdx = ((i % 3) == 0)?i-2:i+1;
               if ( nNewIdx < ParentEditor.FreehandPoints.Length)
               {
                  arpt[i] = ParentEditor.FreehandPoints[nNewIdx];
               }
               else
                  arpt[i] = ParentEditor.FreehandPoints[ParentEditor.FreehandPoints.Length - 1];
            }
            else
               arpt[i] = ParentEditor.FreehandPoints[i];
         }
         return new GraphicsPath(arpt, arby);
      }

      private GraphicsPath GetSelectionPath()
      {
         int nPointCount = ParentEditor.SelectionOutline.Length;
         byte[] arby = new byte[nPointCount];

         arby[0] = (byte)PathPointType.Start;

         for(int i = 1; i<nPointCount; i++)
         {
            arby[i] = (byte)PathPointType.Line;
         }
         return new GraphicsPath(ParentEditor.SelectionOutline, arby);
      }

      private PointF ConvertMouseCoordsToPoint(float x, float y)
      {
         PointF ptResult = new PointF(((float)x * Image.Width) / this.ClientSize.Width,
            ((float)y * Image.Height) / this.ClientSize.Height);

         if ((0 == (ParentEditor.CurrentOptions & ToolOptions.AntiAlias))
            || (ParentEditor.CurrentTool == DrawingTool.Erase)
            || (ParentEditor.CurrentTool == DrawingTool.SelRect)
            || (ParentEditor.CurrentTool == DrawingTool.SelFree))
         {
            ptResult.X = (float)Math.Round(ptResult.X);
            ptResult.Y = (float)Math.Round(ptResult.Y);
         } 

         return ptResult;
      }

      private Boolean IsPointFloodable(Color SourceColorStart, Color SourceColorEnd, Color DestColor)
      {
         return MaxDiff(SourceColorStart, SourceColorEnd) >= MaxDiff(SourceColorStart, DestColor);
      }

      private int MaxDiff(Color Color1, Color Color2)
      {
         int nDiff = Math.Abs((int)Color1.R - (int)Color2.R);
         int nTmp = Math.Abs((int)Color1.G - (int)Color2.G);
         if (nTmp > nDiff) nDiff = nTmp;
         nTmp = Math.Abs((int)Color1.B - (int)Color2.B);
         if (nTmp > nDiff) nDiff = nTmp;
         nTmp = Math.Abs((int)Color1.A - (int)Color2.A);
         if (nTmp > nDiff) nDiff = nTmp;
         if (Color1.A > Color2.A)
            nDiff *= Color1.A;
         else
            nDiff *= Color2.A;
         return nDiff;
      }

      public void Blur(Bitmap bmpTarget, Rectangle rcBlur)
      {
         Bitmap bmpBlur = new Bitmap((int)(rcBlur.Width+1), (int)(rcBlur.Height+1), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
         Graphics g = Graphics.FromImage(bmpBlur);
         g.CompositingMode = CompositingMode.SourceOver;
         g.DrawImage(bmpTarget, 0.5f, 0.5f, rcBlur, GraphicsUnit.Pixel);
         g.Dispose();
         float fRadius;
         if (bmpBlur.Width < bmpBlur.Height)
            fRadius = bmpBlur.Width / 2f;
         else
            fRadius = bmpBlur.Height / 2f;
         for (int y = 0; y < bmpBlur.Height; y++)
         {
            for (int x = 0; x < bmpBlur.Width; x++)
            {
               Color clrPix = bmpBlur.GetPixel(x,y);
               float fcx = x - bmpBlur.Width / 2f;
               float fcy = y - bmpBlur.Height / 2f;
               float fDist = (float)Math.Sqrt(fcx * fcx + fcy * fcy);
               if (fDist > fRadius)
                  clrPix = Color.Transparent;
               else
                  clrPix = Color.FromArgb((int)(clrPix.A * (fRadius - fDist) / fRadius), clrPix);
               bmpBlur.SetPixel(x, y, clrPix);
            }
         }
         g = Graphics.FromImage(bmpTarget);
         g.DrawImage(bmpBlur, rcBlur.X - .5f, rcBlur.Y - .5f);
         g.Dispose();
      }
      #endregion

      #region Public methods
      /// <summary>
      /// Draw an image on the form, but put a background on it to see where the transparency is.
      /// Any floating selection that may exist is drawn on top.
      /// </summary>
      /// <param name="img">Image to draw</param>
      public void DrawTransparentImage(Image img, Graphics PaintContext)
      {
         Bitmap bmp = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb);
         Bitmap bmpSmallBuffer = null;
         Graphics g = Graphics.FromImage(bmp);
         Graphics gSmall = null;
         Pen SelectionOutlinePen;

         g.SmoothingMode = SmoothingMode.None;
         g.FillRectangle(ParentEditor.Backdrop,0,0,ClientRectangle.Width,ClientRectangle.Height);
         g.InterpolationMode = InterpolationMode.NearestNeighbor;
         g.PixelOffsetMode = PixelOffsetMode.Half;
         g.DrawImage(img, 0, 0, bmp.Width, bmp.Height);

         if (ParentEditor.SelectionOutline != null)
         {
            SelectionOutlinePen = new Pen(Color.White);
            SelectionOutlinePen.DashStyle = DashStyle.Custom;
            SelectionOutlinePen.DashPattern = new float[] {3.0f, 3.0f};
            PointF[] ScaledOutline = (PointF[])ParentEditor.SelectionOutline.Clone();
            Matrix Transform = new Matrix();
            if (ParentEditor.SelectionTransform != null)
               Transform.Multiply(ParentEditor.SelectionTransform);
            if (ParentEditor.TempTransform != null)
               Transform.Multiply(ParentEditor.TempTransform);
            Transform.Translate(.5f, .5f, MatrixOrder.Append);
            Transform.Scale(bmp.Width / (float)img.Width, bmp.Height / (float)img.Height, MatrixOrder.Append);
            Transform.TransformPoints(ScaledOutline);
            Transform.Dispose();
            if (IsSelectionOutlineLongEnoughToDraw(ScaledOutline))
            {
               g.DrawLines(SelectionOutlinePen, ScaledOutline);
               SelectionOutlinePen.Color = Color.Black;
               SelectionOutlinePen.DashOffset = 3;
               g.DrawLines(SelectionOutlinePen, ScaledOutline);
            }
            SelectionOutlinePen.Dispose();
         }

         if (ParentEditor.FloatingSelection != null)
         {
            bmpSmallBuffer = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            gSmall = Graphics.FromImage(bmpSmallBuffer);
            ParentEditor.InitGraphicsSettings(gSmall);
            if (ParentEditor.SelectionTransform != null)
            {
               gSmall.Transform = ParentEditor.SelectionTransform;
               if (ParentEditor.TempTransform != null)
                  gSmall.MultiplyTransform(ParentEditor.TempTransform);
            }
            gSmall.DrawImage(ParentEditor.FloatingSelection, 0, 0);
            g.ResetTransform();
         }

         if ((ParentEditor.SelectedRegion != null) && (ParentEditor.HighlightBrush != null))
         {
            if (bmpSmallBuffer == null)
            {
               bmpSmallBuffer = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
               gSmall = Graphics.FromImage(bmpSmallBuffer);
               if (ParentEditor.SelectionTransform != null)
               {
                  gSmall.Transform = ParentEditor.SelectionTransform;
                  if (ParentEditor.TempTransform != null)
                     gSmall.MultiplyTransform(ParentEditor.TempTransform, MatrixOrder.Append);
               }
               ParentEditor.InitGraphicsSettings(gSmall);
            }
            gSmall.CompositingMode = CompositingMode.SourceOver;
            gSmall.FillRegion(ParentEditor.HighlightBrush , ParentEditor.SelectedRegion);
         }

         if (bmpSmallBuffer != null)
         {
            g.DrawImage(bmpSmallBuffer, 0, 0, bmp.Width, bmp.Height);
            bmpSmallBuffer.Dispose();
            gSmall.Dispose();
         }

         if (ShowGrid  && (bmp.Width / img.Width > 2))
         {
            Pen penGrid;

            if (ParentEditor.Backdrop is SolidBrush)
            {
               Color clr = ((SolidBrush)ParentEditor.Backdrop).Color;
               if (clr.R + clr.G + clr.B > 384)
                  penGrid = new Pen(Color.Black);
               else
                  penGrid = new Pen(Color.White);
            }
            else
            {
               Color clr1 = ((HatchBrush)ParentEditor.Backdrop).BackgroundColor;
               Color clr2 = ((HatchBrush)ParentEditor.Backdrop).ForegroundColor;
               if (clr1.R + clr1.G + clr1.B + clr2.R + clr2.G + clr2.B > 768)
                  penGrid = new Pen(Color.Black);
               else
                  penGrid = new Pen(Color.White);
            }

            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            for (int x = 0; x < bmp.Width; x += bmp.Width / img.Width)
               g.DrawLine(penGrid, x, 0, x, bmp.Height);

            for (int y = 0; y < bmp.Height; y += bmp.Height / img.Height)
               g.DrawLine(penGrid, 0, y, bmp.Width, y);

            penGrid.Dispose();
         }

         g.Dispose();
         if (PaintContext == null)
            g = Graphics.FromHwnd(this.Handle);
         else
            g = PaintContext;
         g.DrawImage(bmp, this.ClientRectangle);
         bmp.Dispose();
         g.Dispose();
      }

      public Boolean IsSelectionOutlineLongEnoughToDraw(PointF[] pts)
      {
         float nLen = 0;
         for (int i=1; i< pts.Length; i++)
         {
            nLen += Math.Abs(pts[i].X - pts[i-1].X) + 
               Math.Abs(pts[i].Y - pts[i-1].Y);
            if (nLen > 6) return true;
         }
         return false;
      }

      public Boolean SetMagnify(Int32 nScaleFactor)
      {
         Size szNew;
         szNew = new Size(Image.Width * nScaleFactor, Image.Height * nScaleFactor);
         if (this.ParentEditor.ClientRectangle.Height < szNew.Height)
         {
            MessageBox.Show(ParentEditor, "This magnification would make the form too tall to fit in the editor window.", "Invalid Request Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         if (this.ParentEditor.ClientRectangle.Width < szNew.Width)
         {
            MessageBox.Show(ParentEditor, "This magnification would make the form too wide to fit in the editor window.", "Invalid Request Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         this.ClientSize = szNew;
         frmGraphicPane_Move(this, null);
         if (this.Top < ParentEditor.UsableClientRect.Top)
            this.Top = ParentEditor.UsableClientRect.Top;
         if (this.Left < ParentEditor.UsableClientRect.Left)
            this.Left = ParentEditor.UsableClientRect.Left;
         return true;
      }

      public void AcceptDrawing()
      {
         // Don't dispose of the old image because other people have handles to it
         SGDK2IDE.CopyImage(this.Image, TempImage);
         if (ViewChanged != null)
            ViewChanged();
         if (GraphicChanged != null)
            GraphicChanged();
         ParentEditor.FreehandPoints = null;
      }

      public void RejectDrawing()
      {
         ParentEditor.IsDragging = false;
         ParentEditor.FreehandPoints = null;
      }

      public void FloatSelection()
      {
         if (ParentEditor.SelectedRegion == null)
            ParentEditor.SelectedRegion = new Region(new Rectangle(0, 0, Image.Width, Image.Height));

         Graphics gSrc = Graphics.FromImage(Image);
         Rectangle rcFloat = Rectangle.Round(ParentEditor.SelectedRegion.GetBounds(gSrc));
         gSrc.Clip = ParentEditor.SelectedRegion;
         if (ParentEditor.FloatingSelection != null)
            ParentEditor.DropFloatingSelection();
         ParentEditor.FloatingSelection = ParentEditor.CopySelection(gSrc);
         if (ParentEditor.SelectionTransform == null)
            ParentEditor.SelectionTransform = new Matrix();
         ParentEditor.SelectionTransform.Translate(rcFloat.X, rcFloat.Y, MatrixOrder.Append);
         ParentEditor.SelectedRegion.Translate(-rcFloat.X, -rcFloat.Y);
         if (ParentEditor.SelectionOutline != null)
         {
            Matrix OutlineTransform = new Matrix();
            OutlineTransform.Translate(-rcFloat.X, -rcFloat.Y);
            OutlineTransform.TransformPoints(ParentEditor.SelectionOutline);
            OutlineTransform.Dispose();
         }
         gSrc.CompositingMode = CompositingMode.SourceCopy;
         gSrc.FillRectangle(Brushes.Transparent, 0, 0, Image.Width, Image.Height);
         gSrc.Dispose();
         if (GraphicChanged != null)
            GraphicChanged();
      }

      public Region GetFloodRegion(Bitmap FloodImage, Point Start, Point End)
      {
         Queue FloodQueue = new Queue();
         Point ptCur;
         if ((Start.X < 0) || (Start.Y < 0) || (Start.X >= Image.Width) || (Start.Y >= Image.Height))
            return new Region();
         Color StartColor = FloodImage.GetPixel(Start.X, Start.Y);
         if ((End.X < 0) || (End.Y < 0) || (End.X >= Image.Width) || (End.Y >= Image.Height))
            return new Region();
         Color EndColor = FloodImage.GetPixel(End.X, End.Y);
         Boolean bSeparatorAbove, bSeparatorBelow;
         Region rgnResult = null;

         FloodQueue.Enqueue(Start);
         while(FloodQueue.Count > 0)
         {
            ptCur = (Point)FloodQueue.Dequeue();
            int nMinX, nMaxX;
            for (nMinX=ptCur.X; nMinX > 0; nMinX--)
            {
               if (!IsPointFloodable(StartColor, EndColor, FloodImage.GetPixel(nMinX-1, ptCur.Y)))
                  break;
            }

            bSeparatorAbove = bSeparatorBelow = true;
            for (nMaxX = nMinX; nMaxX < Image.Width - 1; nMaxX++)
            {
               if (ptCur.Y > 0)
               {
                  if (IsPointFloodable(StartColor, EndColor, FloodImage.GetPixel(nMaxX, ptCur.Y - 1)))
                  {
                     Point ptAdd = new Point(nMaxX, ptCur.Y - 1);
                     if (bSeparatorAbove)
                        if ((rgnResult == null) || !rgnResult.IsVisible(ptAdd))
                           if (!FloodQueue.Contains(ptAdd))
                              FloodQueue.Enqueue(ptAdd);
                     bSeparatorAbove = false;
                  }
                  else
                     bSeparatorAbove = true;
               }
               if (ptCur.Y < FloodImage.Height - 1)
               {
                  if (IsPointFloodable(StartColor, EndColor, FloodImage.GetPixel(nMaxX, ptCur.Y + 1)))
                  {
                     Point ptAdd = new Point(nMaxX, ptCur.Y + 1);
                     if (bSeparatorBelow)
                        if ((rgnResult == null) || !rgnResult.IsVisible(ptAdd))
                           if (!FloodQueue.Contains(ptAdd))
                              FloodQueue.Enqueue(ptAdd);
                     bSeparatorBelow = false;
                  }
                  else
                     bSeparatorBelow = true;
               }
               if (!IsPointFloodable(StartColor, EndColor, FloodImage.GetPixel(nMaxX+1, ptCur.Y)))
                  break;
            }

            if (rgnResult == null)
               rgnResult = new Region(new Rectangle(nMinX, ptCur.Y, nMaxX - nMinX + 1, 1));
            else
               rgnResult.Union(new Rectangle(nMinX, ptCur.Y, nMaxX - nMinX + 1, 1));
         }
         return rgnResult;
      }

      public void ClearTempImage()
      {
         if (TempImage != null)
         {
            TempImage.Dispose();
            TempImage = null;
         }
      }
      #endregion
   }
}
