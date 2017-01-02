/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace SGDK2
{
   public interface IBooleanIndexer
   {
      bool this[int nIndex]
      {
         get;
         set;
      }
   }

   [Flags()]
   public enum HitFlags
   {
      Default = 0,
      GetNearest = 1,
      AllowExtraCell = 2
   }

   /// <summary>
   /// Summary description for CellBrowser.
   /// </summary>
   public class GraphicBrowser : DragPanel, IBooleanIndexer
   {
      #region Fields
      private ProjectDataset.GraphicSheetRow m_GraphicSheet = null;
      private ProjectDataset.FramesetRow m_Frameset = null;
      private Bitmap m_imgGraphicSheet = null;
      private Size m_CellPadding;
      private Rectangle m_LargestCell;
      private SelectionMode m_SelectionMode = SelectionMode.MultiExtended;
      private BitArray m_SelectedCells = null;
      private Point m_DragStart = Point.Empty;
      private Size m_FrameDrawSize = Size.Empty;
      private bool m_bDragReady = false;
      private BitArray m_DragSel;
      private int m_FocusIndex = -1;
      private bool m_bSelTemp;
      private Timer RecalcTimer = null;
      private DateTime DragScrollTime = DateTime.MinValue;
      private FrameList m_FramesToDisplay = null;
      private bool m_bIsOrdered;
      private bool m_CellBorders = false;

      ProjectDataset.GraphicSheetRowChangeEventHandler m_RowChangeEvent = null;
      ProjectDataset.FrameRowChangeEventHandler m_FrameChangingEvent = null;
      private ListChangedEventHandler m_FramesToDisplayChangedEvent = null;
      #endregion

      #region Events
      public event System.EventHandler CurrentCellChanged;
      #endregion

      #region Initialization and clean-up
      public GraphicBrowser()
      {
         this.VScroll = true;
         this.SetStyle(ControlStyles.ResizeRedraw, true);
         this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
         this.SetStyle(ControlStyles.UserPaint, true);
         this.SetStyle(ControlStyles.DoubleBuffer, true);
         this.SetStyle(ControlStyles.Selectable, true);
         this.m_bIsOrdered = true;
      }

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose( bool disposing )
      {
         if (disposing)
         {
            if (RecalcTimer != null)
            {
               RecalcTimer.Dispose();
               RecalcTimer = null;
            }
            if (m_RowChangeEvent != null)
            {
               
               if (m_GraphicSheet != null)
               {
                  ProjectData.GraphicSheetRowChanged -= m_RowChangeEvent;
                  m_RowChangeEvent = null;
               }
               else if (m_Frameset != null)
               {
                  ProjectData.GraphicSheetRowChanged -= m_RowChangeEvent;
                  ProjectData.FrameRowChanging -= m_FrameChangingEvent;
                  ProjectData.FrameRowDeleting -= m_FrameChangingEvent;
                  m_FrameChangingEvent = null;
                  m_RowChangeEvent = null;
               }
            }

            if (m_imgGraphicSheet != null)
            {
               m_imgGraphicSheet.Dispose();
               m_imgGraphicSheet = null;
            }

            if (m_FramesToDisplayChangedEvent != null)
            {
               m_FramesToDisplay.ListChanged -= m_FramesToDisplayChangedEvent;
               m_FramesToDisplayChangedEvent = null;
            }
         }
         base.Dispose( disposing );
      }
      #endregion

      #region Properties
      public ProjectDataset.GraphicSheetRow GraphicSheet
      {
         get
         {
            return m_GraphicSheet;
         }
         set
         {
            try
            {
               // NOTE: Loading a PNG file with a different resolution than the screen exhibits
               // strange behavior.  Not sure how to force a matching resolution.
               if (m_Frameset != null)
                  Frameset = null;
               if (m_imgGraphicSheet != null)
               {
                  m_imgGraphicSheet.Dispose();
                  m_imgGraphicSheet = null;
               }
               if ((m_RowChangeEvent != null) && (value == null))
               {
                  ((ProjectDataset.GraphicSheetDataTable)m_GraphicSheet.Table).GraphicSheetRowChanged -= m_RowChangeEvent;
                  m_RowChangeEvent = null;
               }
               m_GraphicSheet = value;

               if (value != null)
               {
                  if (m_RowChangeEvent == null)
                  {
                     m_RowChangeEvent = new SGDK2.ProjectDataset.GraphicSheetRowChangeEventHandler(GraphicSheet_RowChanged);
                     ((ProjectDataset.GraphicSheetDataTable)m_GraphicSheet.Table).GraphicSheetRowChanged += m_RowChangeEvent;
                  }
                  QueueRecalc();
               }
            }
            catch(System.Exception ex)
            {
               MessageBox.Show("Set Property GraphicSheet: " + ex.Message);
               throw;
            }
         }
      }

      public ProjectDataset.FramesetRow Frameset
      {
         get
         {
            return m_Frameset;
         }
         set
         {
            try
            {
               if (m_GraphicSheet != null)
                  GraphicSheet = null;
               if (m_imgGraphicSheet != null)
               {
                  m_imgGraphicSheet.Dispose();
                  m_imgGraphicSheet = null;
               }
               if ((m_RowChangeEvent != null) && (value == null))
                  ProjectData.GraphicSheetRowChanged -= m_RowChangeEvent;
               if ((m_FrameChangingEvent != null) && (value == null))
               {
                  ProjectData.FrameRowChanging -= m_FrameChangingEvent;
                  ProjectData.FrameRowDeleting -= m_FrameChangingEvent;
               }
               m_Frameset = value;
               if (value != null)
               {
                  if (m_RowChangeEvent == null)
                  {
                     m_RowChangeEvent = new SGDK2.ProjectDataset.GraphicSheetRowChangeEventHandler(GraphicSheet_RowChanged);
                     ProjectData.GraphicSheetRowChanged += m_RowChangeEvent;
                  }
                  if (m_FrameChangingEvent == null)
                  {
                     m_FrameChangingEvent = new SGDK2.ProjectDataset.FrameRowChangeEventHandler(Frame_FrameRowChanging);
                     ProjectData.FrameRowChanging += m_FrameChangingEvent;
                     ProjectData.FrameRowDeleting += m_FrameChangingEvent;
                  }
                  QueueRecalc();
               }
            }
            catch(System.Exception ex)
            {
               MessageBox.Show("Set Property FrameSet: " + ex.Message);
               throw;
            }
         }
      }

      public FrameList FramesToDisplay
      {
         get
         {
            return m_FramesToDisplay;
         }
         set
         {
            if (m_FramesToDisplayChangedEvent != null)
            {
               m_FramesToDisplay.ListChanged -= m_FramesToDisplayChangedEvent;
               m_FramesToDisplayChangedEvent = null;
            }
            m_FramesToDisplay = value;
            if (value != null)
            {
               m_FramesToDisplayChangedEvent = new ListChangedEventHandler(FramesToDisplay_ListChanged);
               m_FramesToDisplay.ListChanged += m_FramesToDisplayChangedEvent;
            }
            QueueRecalc();
         }
      }

      public Bitmap SheetImage
      {
         get
         {
            return m_imgGraphicSheet;
         }
         set
         {
            if (m_Frameset != null)
               Frameset = null;
            if (m_GraphicSheet != null)
               GraphicSheet = null;
            m_imgGraphicSheet = value;
            QueueRecalc();
         }
      }

      public Size CellPadding
      {
         get
         {
            return m_CellPadding;
         }
         set
         {
            m_CellPadding = value;
            Invalidate();
         }
      }

      public Size CellSize
      {
         get
         {
            return new Size(m_LargestCell.Width, m_LargestCell.Height);
         }
         set
         {
            m_LargestCell = new Rectangle(0, 0, value.Width, value.Height);
            Invalidate();
         }
      }

      public bool CellBorders
      {
         get
         {
            return m_CellBorders;
         }
         set
         {
            m_CellBorders = value;
            Invalidate();
         }
      }

      public IBooleanIndexer Selected
      {
         get
         {
            return this;
         }
      }

      bool IBooleanIndexer.this[int nIndex]
      {
         get
         {
            return GetSelectedInternal(nIndex) || (m_FocusIndex == nIndex);
         }
         set
         {
            if (nIndex >= CellCount)
               throw new IndexOutOfRangeException("Index " + nIndex.ToString() + " exceeds the cell count of " + CellCount.ToString() + ".");
            if (nIndex < 0)
               throw new IndexOutOfRangeException("Index for Selected cannot be less than 0.");
            if (FramesToDisplay != null)
            {
               FramesToDisplay[nIndex].IsSelected = value;
            }
            else
            {
               if (m_SelectedCells == null)
                  m_SelectedCells = new BitArray(CellCount, false);
               if (nIndex >= m_SelectedCells.Length)
               {
                  BitArray tmp = new BitArray(CellCount, false);
                  for (Int32 nIdx = 0; nIdx < m_SelectedCells.Length; nIdx++)
                     tmp[nIdx] = m_SelectedCells[nIdx];
                  m_SelectedCells = tmp;
               }
               m_SelectedCells[nIndex] = value;
            }
            Invalidate(GetCellRect(nIndex, true));
         }
      }

      public int CellCount
      {
         get
         {
            if (m_imgGraphicSheet != null)
               return ((m_imgGraphicSheet.Width / CellSize.Width) *
                  (m_imgGraphicSheet.Height / CellSize.Height));
            
            if (m_GraphicSheet != null)
            {
               Bitmap img = ProjectData.GetGraphicSheetImage(m_GraphicSheet.Name, false);
               return ((img.Width / m_GraphicSheet.CellWidth) * (img.Height / m_GraphicSheet.CellHeight));
            }

            if (m_Frameset != null)
            {
               if (m_FramesToDisplay==null)
                  return m_Frameset.GetFrameRows().Length;
               else
                  return m_FramesToDisplay.Count;
            }

            if ((m_FramesToDisplay != null) && (m_FramesToDisplay.ProvidesGraphics))
               return m_FramesToDisplay.Count;

            return 0;
         }
      }

      public int CurrentCellIndex
      {
         get
         {
            return m_FocusIndex;
         }
         set
         {
            if (value < -1) throw new IndexOutOfRangeException("CurrentCellIndex must be -1 or non-negative");
            if (value >= CellCount) throw new IndexOutOfRangeException("CurrentCellIndex must be less than CellCount");
            if ((m_FocusIndex >= 0) && (m_FocusIndex < CellCount))
               Invalidate(GetCellRect(m_FocusIndex, true));
            m_FocusIndex = value;
            if ((m_FocusIndex >= 0) && (m_FocusIndex < CellCount))
               Invalidate(GetCellRect(m_FocusIndex, true));
            if (CurrentCellChanged != null)
               CurrentCellChanged(this, null);
         }
      }

      [DefaultValue(true),
      AmbientValue(true),
      Description("Determines whether the control should draw a dotted frame at the tile position where a drop would occur during a drag operation")]
      public bool IsOrdered
      {
         get
         {
            return m_bIsOrdered;
         }
         set
         {
            m_bIsOrdered = false;
         }
      }
      #endregion

      #region Private methods
      private Size CalculateVirtualSize()
      {
         if (CellCount == 0)
            return new Size(0,0);
         int nTilesPerRow = (ClientSize.Width - m_CellPadding.Width) /
            (m_LargestCell.Width + m_CellPadding.Width);
         if (nTilesPerRow <= 0)
            nTilesPerRow = 1;
         int nTileCount = CellCount;
         int nRows = (nTileCount + nTilesPerRow - 1) / nTilesPerRow;
         return new Size(
            nTilesPerRow * (CellSize.Width + m_CellPadding.Width) + m_CellPadding.Width,
            nRows * (CellSize.Height + m_CellPadding.Height) + m_CellPadding.Height);
      }

      private Rectangle GetLargestCell()
      {
         if (m_imgGraphicSheet != null)
         {
            Rectangle r = new Rectangle(0, 0, m_LargestCell.Width, m_LargestCell.Height);
            return r;
         }

         if (m_GraphicSheet != null)
            return new Rectangle(0, 0, m_GraphicSheet.CellWidth, m_GraphicSheet.CellHeight);

         if (m_Frameset != null)
         {
            Rectangle rcMax = new Rectangle(0,0,0,0);
            ProjectDataset ds = (ProjectDataset)m_Frameset.Table.DataSet;
            int nCellCount = CellCount;
            ProjectDataset.FrameRow[] frames = GetSortedFrameRows();
            for(int nCellIndex = 0; nCellIndex < nCellCount; nCellIndex++)                                                                              
            {
               ProjectDataset.FrameRow dr;
               for (int nSubFrame = 0; (nSubFrame==0) || ((m_FramesToDisplay != null) && (nSubFrame < m_FramesToDisplay[nCellIndex].FrameIndexes.Length)); nSubFrame++)
               {
                  if (m_FramesToDisplay==null)
                     dr = frames[nCellIndex];
                  else
                  {
                     if (m_FramesToDisplay[nCellIndex].FrameIndexes.Length > 0)
                        dr = frames[m_FramesToDisplay[nCellIndex].FrameIndexes[nSubFrame]%frames.Length];
                     else
                        continue;
                  }

                  ProjectDataset.GraphicSheetRow drG = ds.GraphicSheet.FindByName(dr.GraphicSheet);
                  Point[] ptCorners = new Point[]
                  {
                     new Point(0,0),
                     new Point(drG.CellWidth, 0),
                     new Point(0, drG.CellHeight),
                     new Point(drG.CellWidth, drG.CellHeight)
                  };
                  Matrix m = new System.Drawing.Drawing2D.Matrix(dr.m11, dr.m12, dr.m21, dr.m22, dr.dx, dr.dy);
                  m.TransformPoints(ptCorners);
                  m.Dispose();
                  foreach (Point pt in ptCorners)
                  {
                     if (pt.X < rcMax.X)
                     {
                        rcMax.Width = rcMax.Left + rcMax.Width - pt.X;
                        rcMax.X = pt.X;
                     }
                     if (pt.Y < rcMax.Y)
                     {
                        rcMax.Height = rcMax.Top + rcMax.Height - pt.Y;
                        rcMax.Y = pt.Y;
                     }
                     if (pt.X - rcMax.Left > rcMax.Width)
                        rcMax.Width = pt.X - rcMax.Left;
                     if (pt.Y - rcMax.Top > rcMax.Height)
                        rcMax.Height = pt.Y - rcMax.Top;
                  }
               }
            }
            return rcMax;
         }

         if  ((m_FramesToDisplay != null) && (m_FramesToDisplay.ProvidesGraphics))
         {
            Rectangle rcMax = new Rectangle(0,0,0,0);
            int nCellCount = CellCount;
            for(int nCellIndex = 0; nCellIndex < nCellCount; nCellIndex++)
            {
               for (int nSubFrame = 0; nSubFrame < m_FramesToDisplay[nCellIndex].FrameIndexes.Length; nSubFrame++)
               {
                  IProvideGraphics gfx;
                  if (m_FramesToDisplay[nCellIndex].FrameIndexes.Length > 0)
                     gfx = ((IProvideGraphics)m_FramesToDisplay[nCellIndex]);
                  else
                     continue;

                  ProjectDataset.GraphicSheetRow drG = gfx.GetGraphicSheet(nSubFrame);
                  Point[] ptCorners = gfx.GetTransformedCorners(nSubFrame);
                  foreach (Point pt in ptCorners)
                  {
                     if (pt.X < rcMax.X)
                     {
                        rcMax.Width = rcMax.Left + rcMax.Width - pt.X;
                        rcMax.X = pt.X;
                     }
                     if (pt.Y < rcMax.Y)
                     {
                        rcMax.Height = rcMax.Top + rcMax.Height - pt.Y;
                        rcMax.Y = pt.Y;
                     }
                     if (pt.X - rcMax.Left > rcMax.Width)
                        rcMax.Width = pt.X - rcMax.Left;
                     if (pt.Y - rcMax.Top > rcMax.Height)
                        rcMax.Height = pt.Y - rcMax.Top;
                  }
               }
            }
            return rcMax;
         }

         return Rectangle.Empty;
      }

      private void DoSelectionRectangle()
      {
         if ((m_FrameDrawSize != Size.Empty) && (m_DragStart != Point.Empty))
            ControlPaint.DrawReversibleFrame(
               new Rectangle(PointToScreen(m_DragStart), m_FrameDrawSize),
               Color.LightGray, FrameStyle.Dashed);
      }
      private void QueueRecalc()
      {
         if (RecalcTimer == null)
         {
            RecalcTimer = new Timer();
            RecalcTimer.Tick += new EventHandler(RecalcTimer_Tick);
         }
         else
            RecalcTimer.Stop();
         RecalcTimer.Interval = 100;
         RecalcTimer.Start();
      }

      private bool GetSelectedInternal(int nIndex)
      {
         if (nIndex >= CellCount)
            throw new IndexOutOfRangeException("Index " + nIndex.ToString() + " exceeds the cell count of " + CellCount.ToString() + ".");
         if (nIndex < 0)
            throw new IndexOutOfRangeException("Index for Selected cannot be less than 0.");
         if (FramesToDisplay != null)
            return m_FramesToDisplay[nIndex].IsSelected;
         return (m_SelectedCells != null) && (nIndex < m_SelectedCells.Length) && m_SelectedCells[nIndex];
      }
      #endregion

      #region Public Methods
      public BitArray GetCellsInRect(Rectangle rect)
      {
         if ((m_imgGraphicSheet == null) && (m_Frameset == null) && (m_GraphicSheet == null) &&
            ((m_FramesToDisplay == null) || !m_FramesToDisplay.ProvidesGraphics))
            return null;

         if ((CellSize.Width <= 0) || (CellSize.Height <= 0))
            return null;

         int nX = m_CellPadding.Width;
         int nY = m_CellPadding.Height;

         int nCellCount = CellCount;

         BitArray Result = null;

         if (rect.Width < 0)
         {
            rect.Width = -rect.Width;
            rect.X -= rect.Width;
         }

         if (rect.Height < 0)
         {
            rect.Height = -rect.Height;
            rect.Y -= rect.Height;
         }

         for (int nCell = 0; nCell < nCellCount; nCell++,
            nX += CellSize.Width + m_CellPadding.Width)
         {
            if ((nX > m_CellPadding.Width) &&
               (nX + CellSize.Width + m_CellPadding.Width > ClientSize.Width))
            {
               nX = m_CellPadding.Width;
               nY += CellSize.Height + m_CellPadding.Height;
            }
            if (rect.Contains(nX + AutoScrollPosition.X + CellSize.Width / 2,
               nY + AutoScrollPosition.Y + CellSize.Height / 2))
            {
               if (Result == null)
                  Result = new BitArray(CellCount, false);
               Result[nCell] = true;
            }
         }
         return Result;
      }

      /// <summary>
      /// Get a cell index given a pixel coordinate
      /// </summary>
      /// <param name="X">Horizontal pixel offset from control corner</param>
      /// <param name="Y">Vertical pixel offset from control corner</param>
      /// <param name="HitType">Determines whether new cell is allowed,
      /// and whether the nearest cell is returned when X,Y is not over
      /// a cell</param>
      /// <returns>0-based position of the cell under the coordinate
      /// relative to the series of cells contained in the control</returns>
      public int GetCellAtXY(int X, int Y, HitFlags HitType)
      {
         if ((m_imgGraphicSheet == null) && (m_Frameset == null) && (m_GraphicSheet == null) &&
            ((m_FramesToDisplay == null) || !m_FramesToDisplay.ProvidesGraphics))
         {
            if (0 != (int)(HitType & HitFlags.AllowExtraCell))
               return CellCount;
            else
               return -1;
         }

         if ((CellSize.Width <= 0) || (CellSize.Height <= 0))
         {
            if (0 != (int)(HitType & HitFlags.AllowExtraCell))
               return CellCount;
            else
               return -1;
         }

         int nX = m_CellPadding.Width;
         int nY = m_CellPadding.Height;

         int nCellCount = CellCount;
         int nCell;

         for (nCell = 0; nCell < nCellCount; nCell++,
            nX += CellSize.Width + m_CellPadding.Width)
         {
            if ((nX > m_CellPadding.Width) &&
               (nX + CellSize.Width + m_CellPadding.Width > ClientSize.Width))
            {
               nX = m_CellPadding.Width;
               nY += CellSize.Height + m_CellPadding.Height;
            }
            if (nY + AutoScrollPosition.Y > ClientSize.Height)
               break;
            if ((nX + CellSize.Width + AutoScrollPosition.X > 0) &&
               (nY + CellSize.Height + AutoScrollPosition.Y > 0))
            {
               if (0 != (Int32)(HitType & HitFlags.GetNearest))
               {
                  if ((nX + AutoScrollPosition.X + CellSize.Width + CellPadding.Width > X) &&
                     (nY + AutoScrollPosition.Y + CellSize.Height + CellPadding.Height > Y))
                     return nCell;
               }
               else if (new Rectangle(nX + AutoScrollPosition.X, nY + AutoScrollPosition.Y,
                  CellSize.Width, CellSize.Height).Contains(X, Y))
                  return nCell;
            }
         }
         this.AutoScrollMinSize = CalculateVirtualSize();

         if ((HitFlags.GetNearest | HitFlags.AllowExtraCell) ==
            (HitType & (HitFlags.GetNearest | HitFlags.AllowExtraCell)))
            return nCell;
         else
            return -1;
      }

      public int GetFirstSelectedCell()
      {
         if ((FramesToDisplay==null) && (m_SelectedCells == null))
            return m_FocusIndex;

         if  (m_FocusIndex >= 0)
            return m_FocusIndex;

         for (int nIdx = 0; nIdx < CellCount; nIdx++)
            if (Selected[nIdx])
               return nIdx;
         return -1;
      }

      public void ClearSelection()
      {
         for (int i=0; i<CellCount; i++)
         {
            if (Selected[i])
               Invalidate(GetCellRect(i, true));
         }
         m_SelectedCells = null;
         if (FramesToDisplay != null)
            foreach(IProvideFrame f in FramesToDisplay)
               f.IsSelected = false;
      }

      public int CellsPerRow
      {
         get
         {
            if ((m_imgGraphicSheet == null) && (m_Frameset == null) && (m_GraphicSheet == null) &&
               ((m_FramesToDisplay == null) || !m_FramesToDisplay.ProvidesGraphics))
               return 1;

            if (CellSize.Width <= 0)
               return 1;

            int nResult = (ClientSize.Width - m_CellPadding.Width) / (CellSize.Width + m_CellPadding.Width);
            return (nResult <= 0) ? 1 : nResult;
         }
      }

      public Rectangle GetCellRect(int CellIndex, bool bInflateByPadding)
      {
         Rectangle rcCell = GetCellRect(CellIndex);
         if (bInflateByPadding)
            rcCell.Inflate(CellPadding);
         return rcCell;
      }
      
      public Rectangle GetCellRect(int CellIndex)
      {
         Rectangle rcAbsolute = GetCellAbsoluteRect(CellIndex);
         rcAbsolute.Offset(AutoScrollPosition.X, AutoScrollPosition.Y);
         return rcAbsolute;
      }

      public Rectangle GetCellAbsoluteRect(int CellIndex)
      {
         if ((m_imgGraphicSheet == null) && (m_Frameset == null) && (m_GraphicSheet == null) && 
            ((m_FramesToDisplay == null) || !m_FramesToDisplay.ProvidesGraphics))
            return Rectangle.Empty;

         if ((CellSize.Width <= 0) || (CellSize.Height <= 0))
            return Rectangle.Empty;

         int nCellsPerRow = CellsPerRow;
         int nX = m_CellPadding.Width + (CellIndex % nCellsPerRow) * (CellSize.Width + m_CellPadding.Width);
         int nY = m_CellPadding.Height + (CellIndex / nCellsPerRow) * (CellSize.Height + m_CellPadding.Height);

         return new Rectangle(nX, nY, CellSize.Width, CellSize.Height);
      }

      public int GetSelectedCellCount()
      {
         if ((FramesToDisplay == null) && (m_SelectedCells == null))
            return (m_FocusIndex >= 0) ? 1 : 0;

         int nCount = 0;

         for (int nIdx = 0; nIdx < CellCount; nIdx++)
            if (Selected[nIdx])
               nCount++;

         return nCount;
      }

      public ProjectDataset.FrameRow[] GetSortedFrameRows()
      {
         if (m_Frameset == null)
            return null;
         return ProjectData.GetSortedFrameRows(m_Frameset);
      }

      public ProjectDataset.FrameRow[] GetSelectedFrames()
      {
         if (m_Frameset == null)
            return null;

         if ((FramesToDisplay == null) && (m_SelectedCells == null) && (m_FocusIndex < 0))
            return null;
         
         int nSelIdx = 0;
         ProjectDataset.FrameRow[] result = new SGDK2.ProjectDataset.FrameRow[GetSelectedCellCount()];
         ProjectDataset.FrameRow[] allframes = GetSortedFrameRows();
         for (int nIdx = 0; nIdx < CellCount; nIdx++)
         {
            if (Selected[nIdx])
            {
               if (FramesToDisplay == null)
                  result[nSelIdx++] = allframes[nIdx];
               else
                  result[nSelIdx++] = allframes[FramesToDisplay[nIdx].FrameIndex];
            }
         }
         return result;
      }

      public Bitmap GetCellImageData(int CellIndex)
      {
         int nCols = 1;
         Bitmap bmpImage = m_imgGraphicSheet;

         if (FramesToDisplay != null)
            CellIndex = FramesToDisplay[CellIndex].FrameIndex;

         if (m_Frameset == null)
         {
            if (m_GraphicSheet != null)
               bmpImage = ProjectData.GetGraphicSheetImage(m_GraphicSheet.Name, false, true);
            if (bmpImage == null)
               return null;
            else
               nCols = bmpImage.Width / CellSize.Width;

            return bmpImage.Clone(new Rectangle(
               (CellIndex % nCols) * CellSize.Width, (CellIndex / nCols) * CellSize.Height,
               CellSize.Width, CellSize.Height), bmpImage.PixelFormat);
         }
         else
         {
            ProjectDataset.FrameRow fr = ProjectData.GetFrame(m_Frameset.Name, CellIndex);
            if (fr == null)
               return null;
            ProjectDataset.GraphicSheetRow gr = ProjectData.GetGraphicSheet(fr.GraphicSheet);
            bmpImage = ProjectData.GetGraphicSheetImage(fr.GraphicSheet , false, true);
            nCols = bmpImage.Width / gr.CellWidth;
            Rectangle rcCell = new Rectangle(
               (fr.CellIndex % nCols) * gr.CellWidth,
               (fr.CellIndex / nCols) * gr.CellHeight,
               gr.CellWidth, gr.CellHeight);
            return bmpImage.Clone(rcCell, bmpImage.PixelFormat);
         }
      }

      public bool ScrollCellIntoView(int CellIndex)
      {
         Rectangle rcCell = GetCellAbsoluteRect(CellIndex);
         rcCell.Inflate(0,CellPadding.Height / 2);

         Point ptNew = new Point(-AutoScrollPosition.X, -AutoScrollPosition.Y);

         if (rcCell.Bottom > ClientSize.Height - AutoScrollPosition.Y)
            ptNew = new Point(0, rcCell.Bottom - ClientSize.Height);
         if (rcCell.Top < -AutoScrollPosition.Y)
            ptNew = new Point(0, rcCell.Top);

         if ((ptNew.X != -AutoScrollPosition.X) || (ptNew.Y != -AutoScrollPosition.Y))
         {
            if (!m_DragStart.IsEmpty)
               m_DragStart.Offset(-ptNew.X - AutoScrollPosition.X, -ptNew.Y - AutoScrollPosition.Y);
            AutoScrollPosition = ptNew;
            return true;
         }
         return false;
      }

      /// <summary>
      /// Recompute the size of the largest cell and re-draw the contents with the new size
      /// </summary>
      public void RecalcSizes()
      {
         if (RecalcTimer != null)
         {
            RecalcTimer.Dispose();
            RecalcTimer = null;
         }
         m_LargestCell = GetLargestCell();
         AutoScrollMinSize = CalculateVirtualSize();
         if (m_FocusIndex >= 0)
            ScrollCellIntoView(m_FocusIndex);
         Invalidate();
      }
      #endregion

      #region Overrides
      protected override void OnPaint(PaintEventArgs pe)
      {
         if ((m_imgGraphicSheet == null) && (m_Frameset == null) && (m_GraphicSheet == null) &&
             ((m_FramesToDisplay==null) || !(m_FramesToDisplay.ProvidesGraphics)))
            return;

         if ((CellSize.Width <= 0) || (CellSize.Height <= 0))
         {
            QueueRecalc();
            return;
         }

         Graphics gfxOut = pe.Graphics;
         try
         {
            if (m_FocusIndex >= CellCount)
               CurrentCellIndex = -1;

            pe.Graphics.ResetTransform();

            Brush SelectedBrush = this.Focused ? SystemBrushes.Highlight : SystemBrushes.InactiveCaption;

            m_FrameDrawSize = Size.Empty;

            int nCols = 0;
            Bitmap bmpImage = m_imgGraphicSheet;
            if (m_GraphicSheet != null)
               bmpImage = ProjectData.GetGraphicSheetImage(m_GraphicSheet.Name, false, true);

            if (bmpImage != null)
               nCols = bmpImage.Width / CellSize.Width;

            ProjectDataset.FrameRow[] Frames = null;
            ProjectDataset.GraphicSheetDataTable dtGraphics = null;
            ProjectDataset.GraphicSheetRow drGfx = null;
         
            if ((m_Frameset != null) || ((m_FramesToDisplay != null) && (m_FramesToDisplay.ProvidesGraphics)))
            {
               Frames = GetSortedFrameRows();
               dtGraphics = ProjectData.GraphicSheet;
            }

            int nX = m_CellPadding.Width;
            int nY = m_CellPadding.Height;

            int nCellCount = CellCount;

            for (int nCell = 0; nCell < nCellCount; nCell++,
               nX += CellSize.Width + m_CellPadding.Width)
            {
               if ((nX > m_CellPadding.Width) &&
                  (nX + CellSize.Width + m_CellPadding.Width > ClientSize.Width))
               {
                  nX = m_CellPadding.Width;
                  nY += CellSize.Height + m_CellPadding.Height;
               }
               if (nY + AutoScrollPosition.Y > ClientSize.Height)
                  break;
               if ((nX + CellSize.Width + AutoScrollPosition.X > 0) &&
                  (nY + CellSize.Height + AutoScrollPosition.Y > 0))
               {
                  Rectangle rcSelRect = new Rectangle(nX + AutoScrollPosition.X,
                     nY + AutoScrollPosition.Y, CellSize.Width, CellSize.Height);
                  rcSelRect.Inflate(CellPadding.Width / 2, CellPadding.Height / 2);
                  if (!rcSelRect.IntersectsWith(pe.ClipRectangle))
                     continue;
                  if (Selected[nCell] || ((m_DragSel != null) && (m_DragSel.Length > nCell) && m_DragSel[nCell]))
                  {
                     gfxOut.FillRectangle(SelectedBrush, rcSelRect);
                  }

                  if (m_CellBorders)
                  {
                     using (Pen outlinePen = new Pen(SystemColors.WindowText))
                     {
                        outlinePen.DashStyle = DashStyle.Dash;
                        gfxOut.PixelOffsetMode = PixelOffsetMode.Default;
                        gfxOut.SmoothingMode = SmoothingMode.HighQuality;
                        gfxOut.DrawRectangle(outlinePen, nX + AutoScrollPosition.X - 1,
                           nY + AutoScrollPosition.Y - 1, CellSize.Width + 1, CellSize.Height + 1);
                     }
                  }

                  if ((m_Frameset == null) && ((m_FramesToDisplay == null) || !m_FramesToDisplay.ProvidesGraphics))
                  {
                     Rectangle rcCell = new Rectangle((nCell % nCols) * CellSize.Width,
                        (nCell / nCols) * CellSize.Height, CellSize.Width, CellSize.Height);
                     gfxOut.DrawImage(bmpImage, nX + AutoScrollPosition.X,
                        nY + AutoScrollPosition.Y, rcCell, GraphicsUnit.Pixel);
                  }
                  else
                  {
                     int nFrameToDisplay;
                     int nCompositeFrameStep = 0;

                     for (nCompositeFrameStep = 0;
                        (FramesToDisplay == null) && (nCompositeFrameStep == 0) ||
                        (FramesToDisplay != null) && (FramesToDisplay[nCell].FrameIndexes != null) && 
                        (nCompositeFrameStep < FramesToDisplay[nCell].FrameIndexes.Length);
                        nCompositeFrameStep++)
                     {
                        if (FramesToDisplay == null)
                           nFrameToDisplay = nCell;
                        else
                           nFrameToDisplay = FramesToDisplay[nCell].FrameIndexes[nCompositeFrameStep];

                        Rectangle rcCell;
                        Point[] corners;
                        int color;
                        if ((FramesToDisplay != null) && (FramesToDisplay.ProvidesGraphics))
                        {
                           IProvideGraphics p = (IProvideGraphics)FramesToDisplay[nCell];
                           ProjectDataset.GraphicSheetRow subGraphic = p.GetGraphicSheet(nCompositeFrameStep);
                           if (drGfx != subGraphic)
                           {
                              drGfx = subGraphic;
                              bmpImage = ProjectData.GetGraphicSheetImage(drGfx.Name, false, true);
                              nCols = bmpImage.Width / drGfx.CellWidth;
                           }
                           rcCell = p.GetRectangle(nCompositeFrameStep);
                           corners = new Point[3];
                           Point[] ccwCorners = p.GetTransformedCorners(nCompositeFrameStep);
                           corners[0] = ccwCorners[0];
                           corners[1] = ccwCorners[3];
                           corners[2] = ccwCorners[1];
                           color = p.Color;
                        }
                        else
                        {
                           if ((drGfx == null) || (drGfx.Name != Frames[nFrameToDisplay].GraphicSheet))
                           {
                              drGfx = dtGraphics.FindByName(Frames[nFrameToDisplay].GraphicSheet);
                              bmpImage = ProjectData.GetGraphicSheetImage(drGfx.Name, false, true);
                              nCols = bmpImage.Width / drGfx.CellWidth;
                           }
                           rcCell = new Rectangle((Frames[nFrameToDisplay].CellIndex % nCols) * drGfx.CellWidth,
                              (Frames[nFrameToDisplay].CellIndex / nCols) * drGfx.CellHeight, drGfx.CellWidth, drGfx.CellHeight);
                           using (System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix(
                              Frames[nFrameToDisplay].m11, Frames[nFrameToDisplay].m12, Frames[nFrameToDisplay].m21,
                              Frames[nFrameToDisplay].m22, Frames[nFrameToDisplay].dx, Frames[nFrameToDisplay].dy))
                           {
                              // Assumes that the image is a parallelogram
                              corners = new Point[] {
                                 new Point(0, 0),
                                 new Point(rcCell.Width, 0),
                                 new Point(0, rcCell.Height)};
                              m.TransformPoints(corners);
                           }
                           color = Frames[nFrameToDisplay].color;
                        }
                        for (int cornerIdx = 0; cornerIdx < corners.Length; cornerIdx++)
                           corners[cornerIdx].Offset(
                              nX + AutoScrollPosition.X - m_LargestCell.X,
                              nY + AutoScrollPosition.Y - m_LargestCell.Y);
                        gfxOut.PixelOffsetMode = PixelOffsetMode.Half;
                        if (color != -1)
                        {
                           byte[] clr = BitConverter.GetBytes(color);
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
                              gfxOut.DrawImage(bmpImage, corners, rcCell,
                                 System.Drawing.GraphicsUnit.Pixel, attr);
                           }
                        }
                        else
                           gfxOut.DrawImage(bmpImage, corners, rcCell, GraphicsUnit.Pixel);
                     }
                  }
                  if ((Selected[nCell]) || ((m_DragSel != null) &&
                     (m_DragSel.Length > nCell) && m_DragSel[nCell]))
                  {
                     SolidBrush brTemp = new SolidBrush(Color.FromArgb(96, SystemColors.Highlight));
                     gfxOut.FillRectangle(brTemp, rcSelRect);
                     brTemp.Dispose();
                  }
               }
            }
            this.AutoScrollMinSize = CalculateVirtualSize();

            if ((m_FocusIndex >= 0) && (Focused))
            {
               gfxOut.PixelOffsetMode = PixelOffsetMode.Default;
               Rectangle rcFocus = GetCellRect(m_FocusIndex);
               rcFocus.Inflate(CellPadding.Width / 2, CellPadding.Height / 2);
               ControlPaint.DrawFocusRectangle(gfxOut, rcFocus, SystemColors.HighlightText, SystemColors.Highlight);
            }
         }
         catch (System.Exception ex)
         {
            SolidBrush brTemp = new SolidBrush(this.ForeColor);
            gfxOut.DrawString(ex.Message, this.Font, brTemp, 0, 0);
            brTemp.Dispose();
         }

         // Calling the base class OnPaint
         base.OnPaint(pe);
      }

      protected override void OnMouseDown(MouseEventArgs e)
      {
         int nCell = GetCellAtXY(e.X, e.Y, HitFlags.Default);
         int nSelCell = GetFirstSelectedCell();

         this.Focus();
         if (nCell >=0)
         {
            m_DragStart = new Point(e.X, e.Y);
            if (m_SelectionMode != SelectionMode.MultiSimple)
               m_bDragReady = true;
         }

         if (0 == (int)(Control.ModifierKeys & Keys.Control))
         {
            if ((nCell < 0) || (!GetSelectedInternal(nCell)))
            {
               ClearSelection();
            }
         }

         if (0 != (int)(Control.ModifierKeys & Keys.Shift))
         {
            if (nCell >= 0)
            {
               if (nCell < nSelCell)
                  for (int nIdx = nCell; nIdx <= nSelCell; nIdx++)
                     Selected[nIdx] = true;
               else
                  for (int nIdx = nSelCell; nIdx <= nCell; nIdx++)
                     Selected[nIdx] = true;
               CurrentCellIndex = nCell;
            }
         }
         else
         {
            if (nCell >= 0)
            {
               if (0 != (int)(Control.ModifierKeys & Keys.Control))
               {
                  if (m_FocusIndex == nCell)
                  {
                     CurrentCellIndex = -1;
                     Selected[nCell] = false;
                  }
                  else
                  {
                     Selected[nCell] = !GetSelectedInternal(nCell);
                     CurrentCellIndex = nCell;
                  }
               }
               else
               {
                  Selected[nCell] = true;
                  CurrentCellIndex = nCell;
               }
            }
            else
            {
               m_DragStart = new Point(e.X, e.Y);
               CurrentCellIndex = -1;
            }
         }

         if (m_FocusIndex >= 0)
            ScrollCellIntoView(m_FocusIndex);

         base.OnMouseDown (e);
      }
   
      protected override void OnMouseMove(MouseEventArgs e)
      {
         if ((int)e.Button == 0)
            this.Cursor = (GetCellAtXY(e.X, e.Y, HitFlags.Default) < 0) ? Cursors.Cross : Cursors.Hand;

         if (m_bDragReady && (0 != (int)(e.Button & MouseButtons.Left)))
         {
            if ((Math.Abs(m_DragStart.X-e.X) >= 5) || (Math.Abs(m_DragStart.Y-e.Y) >= 5))
            {
               DoDragDrop(this, DragDropEffects.Copy | DragDropEffects.Move);
               m_bDragReady = false;
               m_DragStart = Point.Empty;
            }
         }
         else
         {
            if (m_DragStart != Point.Empty)
            {
               DoSelectionRectangle();
               BitArray baTemp = GetCellsInRect(new Rectangle(m_DragStart, new Size(e.X - m_DragStart.X, e.Y - m_DragStart.Y)));
               if ((m_DragSel != null) && (baTemp != null))
               {
                  if (m_DragSel.Length != baTemp.Length)
                  {
                     m_DragSel = baTemp;
                     Refresh();
                  }
                  else
                  {
                     for (int nIdx = 0; nIdx < baTemp.Length; nIdx++)
                     {
                        if (baTemp[nIdx] != m_DragSel[nIdx])
                        {
                           m_DragSel = baTemp;
                           Refresh();
                           break;
                        }
                     }
                  }
                  for (int nIdx = m_DragSel.Length - 1; nIdx >= 0; nIdx--)
                  {
                     if (m_DragSel[nIdx])
                     {
                        if (ScrollCellIntoView(nIdx))
                           Refresh();
                        break;
                     }
                  }
               }
               else
               {
                  if ((m_DragSel == null) ^ (baTemp == null))
                  {
                     m_DragSel = baTemp;
                     Refresh();
                  }
               }
               m_FrameDrawSize = new Size(e.X - m_DragStart.X, e.Y - m_DragStart.Y);
               DoSelectionRectangle();
            }
         }
         base.OnMouseMove (e);
      }

      protected override void OnMouseUp(MouseEventArgs e)
      {
         if (m_DragStart != Point.Empty)
         {
            DoSelectionRectangle();
            if (m_DragSel != null)
            {
               if (FramesToDisplay == null)
               {
                  if (m_SelectedCells == null)
                     m_SelectedCells = m_DragSel;
                  else if (m_DragSel.Length == m_SelectedCells.Length)
                     m_SelectedCells = m_DragSel.Or(m_SelectedCells);
               }
               else
               {
                  int nCount = CellCount;
                  for (int nIdx = 0; nIdx < CellCount; nIdx++)
                  {
                     FramesToDisplay[nIdx].IsSelected = m_DragSel[nIdx];
                  }
               }
               Invalidate();
               m_DragSel = null;
            }
         }
         m_DragStart = Point.Empty;
         m_bDragReady = false;
         base.OnMouseUp (e);
      }

      protected override void OnDragOver(DragEventArgs drgevent)
      {
         DoSelectionRectangle();

         base.OnDragOver(drgevent);

         if ((drgevent.Effect != DragDropEffects.None) && (m_bIsOrdered))
         {
            Point ptDrag = PointToClient(new Point(drgevent.X, drgevent.Y));
            int nCell = GetCellAtXY(ptDrag.X, ptDrag.Y, HitFlags.GetNearest | HitFlags.AllowExtraCell);
            Rectangle rcInsert = GetCellRect(nCell);
            Point ptNew = AutoScrollPosition;
            m_FrameDrawSize = Size.Empty;
            bool updateRequired = false;
            if ((rcInsert.Bottom > ClientRectangle.Bottom) && (rcInsert.Top > ClientRectangle.Top))
            {
               ptNew.Y = -ptNew.Y + rcInsert.Bottom - ClientRectangle.Bottom;
               updateRequired = true;
            }
            else if (rcInsert.Top < ClientRectangle.Top)
            {
               ptNew.Y = -ptNew.Y + rcInsert.Top;
               updateRequired = true;
            }
            else if ((ptDrag.Y > ClientSize.Height - 16) && (-AutoScrollPosition.Y + ClientSize.Height < AutoScrollMinSize.Height) && (DateTime.Now.Subtract(DragScrollTime).TotalMilliseconds > 180))
            {
               ptNew.Y = -ptNew.Y + CellSize.Height + CellPadding.Height;
               if (ptNew.Y > AutoScrollMinSize.Height - ClientSize.Height)
                  ptNew.Y = AutoScrollMinSize.Height - ClientSize.Height;
               updateRequired = true;
            }
            else if ((ptDrag.Y < 16) && (AutoScrollPosition.Y < 0) && (DateTime.Now.Subtract(DragScrollTime).TotalMilliseconds > 180))
            {
               ptNew.Y = -ptNew.Y - CellSize.Height - CellPadding.Height;
               if (ptNew.Y < 0)
                  ptNew.Y = 0;
               updateRequired = true;
            }
            if (updateRequired)
            {
               AutoScrollPosition = ptNew;
               rcInsert = GetCellRect(nCell);
               Update();
               DragScrollTime = DateTime.Now;
            }

            if (nCell < CellCount)
            {
               rcInsert.Width = 2;
               rcInsert.Offset(-2,0);
            }
            
            m_DragStart = rcInsert.Location;
            m_FrameDrawSize = rcInsert.Size;

            DoSelectionRectangle();            
         }
      }

      protected override void OnDragDrop(DragEventArgs drgevent)
      {
         DoSelectionRectangle();
         m_DragStart = Point.Empty;
         base.OnDragDrop (drgevent);
      }

      protected override void OnDragLeave(EventArgs e)
      {
         DoSelectionRectangle();
         m_DragStart = Point.Empty;
         m_FrameDrawSize = Size.Empty;
         base.OnDragLeave (e);
      }

      protected override void OnGotFocus(EventArgs e)
      {
         if (m_FocusIndex >= 0)
            Invalidate(GetCellRect(m_FocusIndex, true));
         base.OnGotFocus (e);
      }
   
      protected override void OnLostFocus(EventArgs e)
      {
         if (m_FocusIndex >= 0)
            Invalidate(GetCellRect(m_FocusIndex, true));
         base.OnLostFocus (e);
      }

      protected override void OnKeyDown(KeyEventArgs e)
      {
         if (CellCount <= 0)
            return;
         if (m_FocusIndex < 0) CurrentCellIndex = 0;

         int nNewIdx;
         switch (e.KeyCode)
         {
            case Keys.Left:
               if (!(e.Shift || e.Control))
                  ClearSelection();
               if (e.Shift)
                  Selected[m_FocusIndex] = true;
               if (m_FocusIndex > 0)
                  CurrentCellIndex--;
               if (m_FocusIndex >= 0)
                  if (ScrollCellIntoView(m_FocusIndex))
                     Invalidate();
               break;
            case Keys.Right:
               if (!(e.Shift || e.Control))
                  ClearSelection();
               if (e.Shift)
                  Selected[m_FocusIndex] = true;
               if (m_FocusIndex < CellCount - 1)
                  CurrentCellIndex++;
               if (m_FocusIndex >= 0)
                  if (ScrollCellIntoView(m_FocusIndex))
                     Invalidate();
               break;
            case Keys.Up:
               if (!(e.Shift || e.Control))
                  ClearSelection();
               nNewIdx = m_FocusIndex - CellsPerRow;
               if (nNewIdx < 0)
                  nNewIdx = 0;
               if (e.Shift)
                  for (int nIdx = nNewIdx; nIdx <= m_FocusIndex; nIdx++)
                     Selected[nIdx] = true;
               CurrentCellIndex = nNewIdx;
               if (m_FocusIndex >= 0)
                  if (ScrollCellIntoView(m_FocusIndex))
                     Invalidate();
               break;
            case Keys.Down:
               if (!(e.Shift || e.Control))
                  ClearSelection();
               nNewIdx = m_FocusIndex + CellsPerRow;
               if (nNewIdx >= CellCount)
                  nNewIdx = CellCount - 1;
               if (e.Shift)
                  for (int nIdx = m_FocusIndex; nIdx <= nNewIdx; nIdx++)
                     Selected[nIdx] = true;
               CurrentCellIndex = nNewIdx;
               if (m_FocusIndex >= 0)
                  if (ScrollCellIntoView(m_FocusIndex))
                     Invalidate();
               break;
            case Keys.Space:
               Selected[m_FocusIndex] = !GetSelectedInternal(m_FocusIndex);
               if (m_FocusIndex >= 0)
                  ScrollCellIntoView(m_FocusIndex);
               break;
         }

         base.OnKeyDown (e);
      }

      protected override bool IsInputKey(Keys keyData)
      {
         switch (keyData & Keys.KeyCode)
         {
            case Keys.Up:
            case Keys.Left:
            case Keys.Right:
            case Keys.Down:
               return true;
            default:
               return base.IsInputKey (keyData);
         }
      }
      #endregion

      #region Event Handlers
      public void GraphicSheet_RowChanged(object sender, ProjectDataset.GraphicSheetRowChangeEvent e)
      {
         if (e.Action != DataRowAction.Change)
            return;
         if ((m_GraphicSheet != null) && (e.Row != m_GraphicSheet))
            return;

         QueueRecalc();
      }

      private void Frame_FrameRowChanging(object sender, SGDK2.ProjectDataset.FrameRowChangeEvent e)
      {
         if (Frameset == null)
            return;

         if ((Frameset.RowState == DataRowState.Deleted) || (Frameset.RowState == DataRowState.Detached))
         {
            Frameset = null;
            return;
         }

         if (e.Row.FramesetRow == Frameset)
         {
            if (FramesToDisplay == null)
            {
               if (e.Action == DataRowAction.Change)
               {
                  Int32 nOrigValue = (Int32)e.Row[ProjectData.Frame.FrameValueColumn, DataRowVersion.Current];
                  if (nOrigValue != e.Row.FrameValue)
                  {
                     if (nOrigValue == int.MaxValue)
                        Selected[e.Row.FrameValue] = m_bSelTemp;
                     else if (e.Row.FrameValue == int.MaxValue)
                        m_bSelTemp = Selected[nOrigValue];
                     else
                     {
                        if (nOrigValue >= CellCount)
                           Selected[e.Row.FrameValue] = m_bSelTemp;
                        else
                           Selected[e.Row.FrameValue] = Selected[nOrigValue];
                     }
                     if (m_FocusIndex == nOrigValue)
                     {
                        if (e.Row.FrameValue == int.MaxValue)
                           CurrentCellIndex = -1;
                        else
                           CurrentCellIndex = e.Row.FrameValue;
                     }
                     if ((CurrentCellIndex == -1) && (nOrigValue == int.MaxValue))
                        CurrentCellIndex = e.Row.FrameValue;
                  }
                  QueueRecalc();
               }
               else if (e.Action == DataRowAction.Delete)
               {
                  if (CellCount > 0)
                     m_bSelTemp = Selected[CellCount-1];
                  QueueRecalc();
               }
               else if (e.Action == DataRowAction.Add)
               {
                  QueueRecalc();
               }
            }
         }
      }

      private void RecalcTimer_Tick(object sender, EventArgs e)
      {
         RecalcSizes();
      }

      private void FramesToDisplay_ListChanged(object sender, ListChangedEventArgs e)
      {
         QueueRecalc();
      }
      #endregion
   }

   public interface IProvideFrame
   {
      /// <summary>
      /// Provide the "main" frame to display
      /// </summary>
      int FrameIndex
      {
         get;
      }

      /// <summary>
      /// Provide an array of frames that compose a composite
      /// tile (only one element for simple tiles)
      /// </summary> 
      int[] FrameIndexes
      {
         get;
      }

      bool IsSelected
      {
         get;
         set;
      }
   }

   public class FrameList : System.Collections.CollectionBase
   {
      public event ListChangedEventHandler ListChanged;

      public bool ProvidesGraphics = false;

      public IProvideFrame this[int index]
      {
         get
         {
            return (IProvideFrame)base.List[index];
         }
         set
         {
            base.List[index] = value;
         }
      }

      public int Add(IProvideFrame value)
      {
         int nResult = base.List.Add(value);
         return nResult; 
      }

      public void Insert(int index, IProvideFrame value)
      {
         base.List.Insert(index, value);
      }

      public void Remove(IProvideFrame value)
      {
         base.List.Remove(value);
      }
   
      protected override void OnClearComplete()
      {
         if (ListChanged != null)
            ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
         base.OnClearComplete ();
      }
   
      protected override void OnInsertComplete(int index, object value)
      {
         if (ListChanged != null)
            ListChanged(this, new ListChangedEventArgs(ListChangedType.ItemAdded, index));
         base.OnInsertComplete (index, value);
      }
   
      protected override void OnRemoveComplete(int index, object value)
      {
         if (ListChanged != null)
            ListChanged(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, -1, index));
         base.OnRemoveComplete (index, value);
      }
   
      protected override void OnSetComplete(int index, object oldValue, object newValue)
      {
         if (ListChanged != null)
            ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
         base.OnSetComplete (index, oldValue, newValue);
      }
   }

   public interface IProvideGraphics
   {
      ProjectDataset.GraphicSheetRow GetGraphicSheet(int subFrame);
      Rectangle GetRectangle(int subFrame);
      Point[] GetTransformedCorners(int subFrame);
      int Color
      {
         get;
      }
   }
}
