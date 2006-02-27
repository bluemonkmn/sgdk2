using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text;
using System.Collections;

namespace SGDK2
{
	/// <summary>
	/// Summary description for ProjectData.
	/// </summary>
   public class ProjectData
   {
      #region Fields
      private static ProjectDataset m_dsPrj = new ProjectDataset();
      private static System.Collections.Specialized.HybridDictionary m_GraphicSheets = null;
      #endregion

      // There should be no instances of this class
      private ProjectData()
      {
      }

      #region General/Dataset
      public static event System.EventHandler Clearing;

      public static void AcceptChanges()
      {
         m_dsPrj.AcceptChanges();
      }

      public static void RejectChanges()
      {
         m_dsPrj.RejectChanges();
      }
      public static void WriteXml(string fileName)
      {
         m_dsPrj.WriteXml(fileName, XmlWriteMode.WriteSchema);
      }
      public static void Clear()
      {
         if (Clearing != null)
            Clearing(null, null);
         m_dsPrj.Clear();
      }
      public static void Merge(DataSet dataSet)
      {
         m_dsPrj.Merge(dataSet);
      }
      public static PropertyCollection ExtendedProperties
      {
         get
         {
            return m_dsPrj.ExtendedProperties;
         }
      }
      public static string ValidateName(string Name)
      {
         if (Name.Length == 0)
            return "Name cannot be empty";

         if (char.IsDigit(Name,0))
         {
            return "Name must not start with a digit.";
         }

         for(int nIdx = 0; nIdx < Name.Length; nIdx++)
         {
            if (!(char.IsLetterOrDigit(Name, nIdx) || (Name[nIdx] == ' ')))
               return "Name must contain only letters, digits and spaces";
         }

         return null;
      }
      #endregion

      #region GraphicsSheet
      public static event ProjectDataset.GraphicSheetRowChangeEventHandler GraphicSheetRowChanged
      {
         add
         {
            m_dsPrj.GraphicSheet.GraphicSheetRowChanged += value;
         }
         remove
         {
            m_dsPrj.GraphicSheet.GraphicSheetRowChanged -= value;
         }
      }
      public static event ProjectDataset.GraphicSheetRowChangeEventHandler GraphicSheetRowChanging
      {
         add
         {
            m_dsPrj.GraphicSheet.GraphicSheetRowChanging += value;
         }
         remove
         {
            m_dsPrj.GraphicSheet.GraphicSheetRowChanging -= value;
         }
      }
      public static event ProjectDataset.GraphicSheetRowChangeEventHandler GraphicSheetRowDeleted
      {
         add
         {
            m_dsPrj.GraphicSheet.GraphicSheetRowDeleted += value;
         }
         remove
         {
            m_dsPrj.GraphicSheet.GraphicSheetRowDeleted -= value;
         }
      }
      
      public static void AddGraphicSheetRow(ProjectDataset.GraphicSheetRow row)
      {
         m_dsPrj.GraphicSheet.AddGraphicSheetRow(row);
      }
      public static ProjectDataset.GraphicSheetRow NewGraphicSheet()
      {
         return m_dsPrj.GraphicSheet.NewGraphicSheetRow();
      }

      public static ProjectDataset.GraphicSheetRow GetGraphicSheet(string Name)
      {
         return m_dsPrj.GraphicSheet.FindByName(Name);
      }
      public static ProjectDataset.GraphicSheetDataTable GraphicSheet
      {
         get
         {
            return m_dsPrj.GraphicSheet;
         }
      }

      /// <summary>
      /// Return a cached copy of a graphic sheet image in bitmap form,
      /// creating it from source data if necessary.
      /// </summary>
      /// <param name="row">Row containing the source data</param>
      /// <param name="bForceReload">Force reload from source data</param>
      /// <returns>Bitmap object</returns>
      public static Bitmap GetGraphicSheetImage(string Name, bool bForceReload)
      {
         if (m_GraphicSheets == null)
            m_GraphicSheets = new System.Collections.Specialized.HybridDictionary();

         if (m_GraphicSheets.Contains(Name))
         {
            WeakReference wr = (WeakReference)m_GraphicSheets[Name];
            if(bForceReload)
            {
               // If reload is forced, may have to dispose of old bitmap
               if (wr.IsAlive)
                  ((Bitmap)wr.Target).Dispose();
            }
            else
            {
               if (wr.IsAlive)
                  return (Bitmap)wr.Target;
            }
         }

         System.IO.MemoryStream ms = new System.IO.MemoryStream(
            ProjectData.GetGraphicSheet(Name).Image, false);
         Bitmap bmpStreamImage = (Bitmap)Bitmap.FromStream(ms);
         Bitmap bmpResult = new Bitmap(bmpStreamImage.Width, bmpStreamImage.Height,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
         SGDK2IDE.CopyImage(bmpResult, bmpStreamImage);
         bmpStreamImage.Dispose();
         ms.Close();
         m_GraphicSheets[Name] = new WeakReference(bmpResult);
         return bmpResult;
      }
      #endregion

      #region Frameset
      public static event ProjectDataset.FramesetRowChangeEventHandler FramesetRowChanged
      {
         add
         {
            m_dsPrj.Frameset.FramesetRowChanged += value;
         }
         remove
         {
            m_dsPrj.Frameset.FramesetRowChanged -= value;
         }
      }
      public static event ProjectDataset.FramesetRowChangeEventHandler FramesetRowChanging
      {
         add
         {
            m_dsPrj.Frameset.FramesetRowChanging += value;
         }
         remove
         {
            m_dsPrj.Frameset.FramesetRowChanging -= value;
         }
      }
      public static event ProjectDataset.FramesetRowChangeEventHandler FramesetRowDeleted
      {
         add
         {
            m_dsPrj.Frameset.FramesetRowDeleted += value;
         }
         remove
         {
            m_dsPrj.Frameset.FramesetRowDeleted -= value;
         }
      }
      public static ProjectDataset.FramesetRow AddFramesetRow(string Name)
      {
         return m_dsPrj.Frameset.AddFramesetRow(Name);
      }

      public static ProjectDataset.FramesetRow GetFrameSet(string Name)
      {
         return m_dsPrj.Frameset.FindByName(Name);
      }

      private class DataRowComparer : IComparer
      {
         #region IComparer Members
         public int Compare(object x, object y)
         {
            if (x is ProjectDataset.FrameRow)
               return ((ProjectDataset.FrameRow)x).FrameValue.CompareTo(((ProjectDataset.FrameRow)y).FrameValue);
            else if (x is ProjectDataset.TileFrameRow)
               return ((ProjectDataset.TileFrameRow)x).Sequence.CompareTo(((ProjectDataset.TileFrameRow)y).Sequence);
            else if (x is ProjectDataset.TileRow)
               return ((ProjectDataset.TileRow)x).TileValue.CompareTo(((ProjectDataset.TileRow)y).TileValue);
            else if (x is ProjectDataset.LayerRow)
               return ((ProjectDataset.LayerRow)x).ZIndex.CompareTo(((ProjectDataset.LayerRow)y).ZIndex);
            else if (x is ProjectDataset.SpriteFrameRow)
               return ((ProjectDataset.SpriteFrameRow)x).Sequence.CompareTo(((ProjectDataset.SpriteFrameRow)y).Sequence);
            else
               throw new ApplicationException("Unknown data row type for comparing");
         }
         #endregion
      }

      public static ProjectDataset.FrameRow[] GetSortedFrameRows(ProjectDataset.FramesetRow row)
      {
         ProjectDataset.FrameRow[] result = row.GetFrameRows();
         Array.Sort(result, new DataRowComparer());
         return result;
      }
      public static ProjectDataset.FramesetDataTable Frameset
      {
         get
         {
            return m_dsPrj.Frameset;
         }
      }
      #endregion

      #region Frame
      public static event ProjectDataset.FrameRowChangeEventHandler FrameRowChanged
      {
         add
         {
            m_dsPrj.Frame.FrameRowChanged += value;
         }
         remove
         {
            m_dsPrj.Frame.FrameRowChanged -= value;
         }
      }
      public static event ProjectDataset.FrameRowChangeEventHandler FrameRowChanging
      {
         add
         {
            m_dsPrj.Frame.FrameRowChanging += value;
         }
         remove
         {
            m_dsPrj.Frame.FrameRowChanging -= value;
         }
      }
      public static event ProjectDataset.FrameRowChangeEventHandler FrameRowDeleted
      {
         add
         {
            m_dsPrj.Frame.FrameRowDeleted += value;
         }
         remove
         {
            m_dsPrj.Frame.FrameRowDeleted -= value;
         }
      }
      public static ProjectDataset.FrameDataTable Frame
      {
         get
         {
            return m_dsPrj.Frame;
         }
      }

      /// <summary>
      /// Insert a frame with the specified FrameValue index into the Frameset
      /// </summary>
      /// <param name="parent">Frameset into which the frame will be added</param>
      /// <param name="FrameValue">Index of the frame in the frameset (existing frames shifted upward)</param>
      /// <param name="GraphicSheet">Name of the graphic sheet owning the graphic for the frame</param>
      /// <param name="CellIndex">Index of the cell in the graphic sheet</param>
      /// <param name="m11">Matrix element</param>
      /// <param name="m12">Matrix element</param>
      /// <param name="m21">Matrix element</param>
      /// <param name="m22">Matrix element</param>
      /// <param name="dx">Matrix element</param>
      /// <param name="dy">Matrix element</param>
      /// <returns></returns>
      public static ProjectDataset.FrameRow InsertFrame(ProjectDataset.FramesetRow parent,
         int FrameValue, string GraphicSheet, short CellIndex,
         float m11, float m12, float m21, float m22, float dx, float dy)
      {
         ProjectDataset.FrameRow drNew = m_dsPrj.Frame.NewFrameRow();
         drNew.FramesetRow = parent;
         // int.MaxValue is the temporary slot
         // Must put frame in temp slot first because FrameValue is not allowed
         // to exceed (or equal) cell count in graphic browser control.
         drNew.FrameValue = int.MaxValue;
         drNew.GraphicSheet = GraphicSheet;
         drNew.CellIndex = CellIndex;
         drNew.m11 = m11;
         drNew.m12 = m12;
         drNew.m21 = m21;
         drNew.m22 = m22;
         drNew.dx = dx;
         drNew.dy = dy;
         DataRow[] ardr = m_dsPrj.Frame.Select("Name='" + parent.Name + "' AND FrameValue>=" + FrameValue.ToString(), "FrameValue DESC");
         m_dsPrj.Frame.Rows.InsertAt(drNew, FrameValue);
         foreach(ProjectDataset.FrameRow dr in ardr)
            dr.FrameValue++;
         m_dsPrj.Frame.AcceptChanges();
         drNew.FrameValue = FrameValue;
         drNew.AcceptChanges();
         return drNew;
      }

      public static int MoveFrame(ProjectDataset.FrameRow row, int NewFrameValue)
      {
         int nSelVal = (row.FrameValue);
         if (NewFrameValue != nSelVal)
         {
            row.FrameValue = int.MaxValue;
            if (NewFrameValue < nSelVal)
            {
               DataRow[] ardr = m_dsPrj.Frame.Select(
                  "Name='" + row.FramesetRow.Name + "' AND FrameValue >= " + NewFrameValue.ToString() +
                  " AND FrameValue < " + nSelVal.ToString(), "FrameValue DESC");
               foreach(ProjectDataset.FrameRow fr in ardr)
                  fr.FrameValue++;
               m_dsPrj.AcceptChanges();
               row.FrameValue = NewFrameValue++;
               row.AcceptChanges();
            }
            else if (NewFrameValue > nSelVal)
            {
               DataRow[] ardr = m_dsPrj.Frame.Select(
                  "Name='" + row.FramesetRow.Name + "' AND FrameValue > " + nSelVal.ToString() +
                  " AND FrameValue < " + NewFrameValue.ToString(), "FrameValue");
               foreach(ProjectDataset.FrameRow fr in ardr)
                  fr.FrameValue--;
               m_dsPrj.Frame.AcceptChanges();
               row.FrameValue = NewFrameValue - 1;
               row.EndEdit();
               row.AcceptChanges();
            }
         }
         return NewFrameValue;
      }
      public static void DeleteFrame(ProjectDataset.FrameRow row)
      {
         int nFrameValue = row.FrameValue;
         string sDelete = row.FramesetRow.Name;
         row.Delete();
         foreach(ProjectDataset.FrameRow fr in
            m_dsPrj.Frame.Select("Name='" + sDelete + "' AND FrameValue > " + nFrameValue.ToString(), "FrameValue"))
            fr.FrameValue--;
         m_dsPrj.Frame.AcceptChanges();
      }
      public static ProjectDataset.FrameRow AddFrameRow(
         ProjectDataset.FramesetRow Frameset, int FrameValue, string GraphicSheet, short CellIndex,
         float m11, float m12, float m21, float m22, float dx, float dy)
      {
         return m_dsPrj.Frame.AddFrameRow(FrameValue, GraphicSheet, CellIndex, m11, m12, m21, m22, dx, dy, Frameset);
      }
      public static ProjectDataset.FrameRow GetFrame(string Frameset, int FrameValue)
      {
         return m_dsPrj.Frame.Rows.Find(new object[] {Frameset, FrameValue}) as ProjectDataset.FrameRow;
      }
      #endregion

      #region Tileset
      public static event ProjectDataset.TilesetRowChangeEventHandler TilesetRowChanged
      {
         add
         {
            m_dsPrj.Tileset.TilesetRowChanged += value;
         }
         remove
         {
            m_dsPrj.Tileset.TilesetRowChanged -= value;
         }
      }
      public static event ProjectDataset.TilesetRowChangeEventHandler TilesetRowChanging
      {
         add
         {
            m_dsPrj.Tileset.TilesetRowChanging += value;
         }
         remove
         {
            m_dsPrj.Tileset.TilesetRowChanging -= value;
         }
      }
      public static event ProjectDataset.TilesetRowChangeEventHandler TilesetRowDeleted
      {
         add
         {
            m_dsPrj.Tileset.TilesetRowDeleted += value;
         }
         remove
         {
            m_dsPrj.Tileset.TilesetRowDeleted -= value;
         }
      }
      public static ProjectDataset.TilesetRow AddTilesetRow(string Name, ProjectDataset.FramesetRow Frameset, short TileWidth, short TileHeight)
      {
         return m_dsPrj.Tileset.AddTilesetRow(Name, Frameset, TileWidth, TileHeight);
      }

      public static ProjectDataset.TilesetRow GetTileSet(string Name)
      {
         return m_dsPrj.Tileset.FindByName(Name);
      }

      public static ProjectDataset.TileRow[] GetSortedTileRows(ProjectDataset.TilesetRow row)
      {
         ProjectDataset.TileRow[] result = row.GetTileRows();
         Array.Sort(result, new DataRowComparer());
         return result;
      }

      public static ProjectDataset.TilesetDataTable Tileset
      {
         get
         {
            return m_dsPrj.Tileset;
         }
      }

      /// <summary>
      /// For each of the 4 sides of a tile retrieve the largest overlap that any frame
      /// in the tileset's frameset will exhibit on that side.  In other words, calculate
      /// how far beyond a standard tile area a tile's graphics extend.
      /// </summary>
      /// <param name="row">Supplies the Tileset to calculate overlaps with</param>
      /// <param name="nLeft">Returns the largest overlap area on the left of a frame</param>
      /// <param name="nTop">Returns the largest overlap on the top of a frame</param>
      /// <param name="nRight">Returns the largest overlap on the right of a frame</param>
      /// <param name="nBottom">Returns the largest overlap on the bottom of a frame</param>
      public static void GetTilesetOverlaps(ProjectDataset.TilesetRow row,
         out int nLeft, out int nTop, out int nRight, out int nBottom)
      {
         ProjectDataset.FrameRow[] frames = GetSortedFrameRows(row.FramesetRow);
         nLeft = nTop = nRight = nBottom = 0;
         for(int i = 0; i < frames.Length; i++)
         {
            Size CellSize = new Size(row.TileWidth, row.TileHeight);
            using (Matrix m = new Matrix(frames[i].m11, frames[i].m12, frames[i].m21, frames[i].m22, frames[i].dx, frames[i].dy))
            {
               System.Drawing.Point[] ptsRect = new Point[]
                  {
                     new Point(0, 0),
                     new Point(CellSize.Width, 0),
                     new Point(CellSize.Width, CellSize.Height),
                     new Point(0, CellSize.Height)
                  };
               m.TransformPoints(ptsRect);
               foreach (Point pt in ptsRect)
               {
                  if(pt.X < -nLeft)
                     nLeft = -pt.X;
                  if(pt.Y < -nTop)
                     nTop = -pt.Y;
                  if (pt.X > CellSize.Width + nRight)
                     nRight = pt.X - CellSize.Width;
                  if (pt.Y > CellSize.Height + nBottom)
                     nBottom = pt.Y - CellSize.Height;
               }
            }
         }
      }
      #endregion

      #region Tile
      public static event ProjectDataset.TileRowChangeEventHandler TileRowChanged
      {
         add
         {
            m_dsPrj.Tile.TileRowChanged += value;
         }
         remove
         {
            m_dsPrj.Tile.TileRowChanged -= value;
         }
      }
      public static event ProjectDataset.TileRowChangeEventHandler TileRowChanging
      {
         add
         {
            m_dsPrj.Tile.TileRowChanging += value;
         }
         remove
         {
            m_dsPrj.Tile.TileRowChanging -= value;
         }
      }
      public static event ProjectDataset.TileRowChangeEventHandler TileRowDeleted
      {
         add
         {
            m_dsPrj.Tile.TileRowDeleted += value;
         }
         remove
         {
            m_dsPrj.Tile.TileRowDeleted -= value;
         }
      }
      /// <summary>
      /// Add a new "managed" tile (not automatically mapped to the frameset) to a tileset.
      /// </summary>
      /// <param name="Tileset">Provides the parent object to which the tile will be added</param>
      /// <param name="TileValue">Specifies the index of the tile that will become a managed tile</param>
      /// <param name="Counter">If specified, indicates the counter object that determines which frame to display</param>
      /// <returns>New tile row object</returns>
      public static ProjectDataset.TileRow AddTileRow(ProjectDataset.TilesetRow Tileset, int TileValue, ProjectDataset.CounterRow Counter)
      {
         ProjectDataset.TileRow tr = m_dsPrj.Tile.NewTileRow();
         tr.TileValue = TileValue;
         tr.CounterRow = Counter;
         tr.TilesetRow = Tileset;
         m_dsPrj.Tile.AddTileRow(tr);
         return tr;
      }

      public static ProjectDataset.TileFrameRow[] GetSortedTileFrames(ProjectDataset.TileRow row)
      {
         ProjectDataset.TileFrameRow[] result = row.GetTileFrameRows();
         Array.Sort(result, new DataRowComparer());
         return result;
      }

      public static ProjectDataset.TileDataTable Tile
      {
         get
         {
            return m_dsPrj.Tile;
         }
      }
      #endregion

      #region TileFrame
      public static event ProjectDataset.TileFrameRowChangeEventHandler TileFrameRowChanged
      {
         add
         {
            m_dsPrj.TileFrame.TileFrameRowChanged += value;
         }
         remove
         {
            m_dsPrj.TileFrame.TileFrameRowChanged -= value;
         }
      }
      public static event ProjectDataset.TileFrameRowChangeEventHandler TileFrameRowChanging
      {
         add
         {
            m_dsPrj.TileFrame.TileFrameRowChanging += value;
         }
         remove
         {
            m_dsPrj.TileFrame.TileFrameRowChanging -= value;
         }
      }
      public static event ProjectDataset.TileFrameRowChangeEventHandler TileFrameRowDeleted
      {
         add
         {
            m_dsPrj.TileFrame.TileFrameRowDeleted += value;
         }
         remove
         {
            m_dsPrj.TileFrame.TileFrameRowDeleted -= value;
         }
      }

      public static ProjectDataset.TileFrameRow InsertFrame(ProjectDataset.TileRow parent, short Sequence, int FrameValue, short Duration)
      {
         try
         {
            ProjectDataset.TileFrameRow[] rows = GetSortedTileFrames(parent);
            
            if (Sequence > rows.Length)
               throw new ArgumentOutOfRangeException("Sequence", Sequence, "Sequence cannot be greater than row count (" + rows.Length.ToString() + ")");

            for (short nIdx = (short)(rows.Length - 1); nIdx >= Sequence; nIdx--)
            {
               System.Diagnostics.Debug.Assert(rows[nIdx].Sequence == nIdx);
               rows[nIdx].Sequence =(short)(nIdx + 1);
            }
            
            return m_dsPrj.TileFrame.AddTileFrameRow(Sequence, FrameValue, Duration, parent.TilesetRow.Name, parent.TileValue);
         }
         catch (System.Exception)
         {
            RejectChanges();
            throw;
         }
      }

      public static void DeleteTileFrame(ProjectDataset.TileFrameRow row)
      {
         try
         {
            short nSequence = row.Sequence;
            ProjectDataset.TileRow trParent = row.TileRowParent;
            row.Delete();
            ProjectDataset.TileFrameRow[] rows = GetSortedTileFrames(trParent);

            for (short nIdx = nSequence; nIdx < rows.Length; nIdx++)
            {
               System.Diagnostics.Debug.Assert(rows[nIdx].Sequence - 1 == nIdx);
               rows[nIdx].Sequence = (short)(nIdx);
            }
         }
         catch (System.Exception)
         {
            RejectChanges();
            throw;
         }
      }

      public static void MoveTileFrame(ProjectDataset.TileFrameRow row, short NewSequence)
      {
         try
         {
            ProjectDataset.TileFrameRow[] rows = GetSortedTileFrames(row.TileRowParent);

            if (NewSequence >= rows.Length)
               throw new ArgumentOutOfRangeException("NewSequence", NewSequence, "Paremeter must be less than collection size (" + rows.Length + ")");

            if (NewSequence == row.Sequence)
               return;

            short nOffsetDir = (short)((row.Sequence < NewSequence) ? -1 : 1);
            short nIdx = (short)(row.Sequence-nOffsetDir);

            row.Sequence = (short)rows.Length;

            for (; nIdx != NewSequence-nOffsetDir; nIdx-=nOffsetDir)
            {
               System.Diagnostics.Debug.Assert(rows[nIdx].Sequence==nIdx);
               rows[nIdx].Sequence = (short)(nIdx + nOffsetDir);
            }
            m_dsPrj.TileFrame.AcceptChanges();
            
            row.Sequence = NewSequence;
         }
         catch (System.Exception)
         {
            RejectChanges();
            throw;
         }
      }
      #endregion

      #region Counter
      public static event ProjectDataset.CounterRowChangeEventHandler CounterRowChanged
      {
         add
         {
            m_dsPrj.Counter.CounterRowChanged += value;
         }
         remove
         {
            m_dsPrj.Counter.CounterRowChanged -= value;
         }
      }
      public static event ProjectDataset.CounterRowChangeEventHandler CounterRowChanging
      {
         add
         {
            m_dsPrj.Counter.CounterRowChanging += value;
         }
         remove
         {
            m_dsPrj.Counter.CounterRowChanging -= value;
         }
      }
      public static event ProjectDataset.CounterRowChangeEventHandler CounterRowDeleted
      {
         add
         {
            m_dsPrj.Counter.CounterRowDeleted += value;
         }
         remove
         {
            m_dsPrj.Counter.CounterRowDeleted -= value;
         }
      }

      public static ProjectDataset.CounterDataTable Counter
      {
         get
         {
            return m_dsPrj.Counter;
         }
      }
      public static ProjectDataset.CounterRow GetCounter(string Name)
      {
         return m_dsPrj.Counter.FindByName(Name);
      }
      public static ProjectDataset.CounterRow AddCounter(string Name, int Value, int Max)
      {
         return m_dsPrj.Counter.AddCounterRow(Name, Value, Max);
      }
      #endregion

      #region Map
      public static event ProjectDataset.MapRowChangeEventHandler MapRowChanged
      {
         add
         {
            m_dsPrj.Map.MapRowChanged += value;
         }
         remove
         {
            m_dsPrj.Map.MapRowChanged -= value;
         }
      }
      public static event ProjectDataset.MapRowChangeEventHandler MapRowChanging
      {
         add
         {
            m_dsPrj.Map.MapRowChanging += value;
         }
         remove
         {
            m_dsPrj.Map.MapRowChanging -= value;
         }
      }
      public static event ProjectDataset.MapRowChangeEventHandler MapRowDeleted
      {
         add
         {
            m_dsPrj.Map.MapRowDeleted += value;
         }
         remove
         {
            m_dsPrj.Map.MapRowDeleted -= value;
         }
      }
      public static void AddMapRow(ProjectDataset.MapRow row)
      {
         m_dsPrj.Map.AddMapRow(row);
      }
      public static ProjectDataset.MapRow NewMap()
      {
         return m_dsPrj.Map.NewMapRow();
      }
      public static ProjectDataset.MapRow GetMap(string Name)
      {
         return m_dsPrj.Map.FindByName(Name);
      }
      public static ProjectDataset.MapDataTable Map
      {
         get
         {
            return m_dsPrj.Map;
         }
      }

      public static ProjectDataset.LayerRow[] GetSortedLayers(ProjectDataset.MapRow row)
      {
         ProjectDataset.LayerRow[] result = row.GetLayerRows();
         Array.Sort(result, new DataRowComparer());
         return result;
      }        

      #endregion

      #region Layer
      public static event ProjectDataset.LayerRowChangeEventHandler LayerRowChanged
      {
         add
         {
            m_dsPrj.Layer.LayerRowChanged += value;
         }
         remove
         {
            m_dsPrj.Layer.LayerRowChanged -= value;
         }
      }
      public static event ProjectDataset.LayerRowChangeEventHandler LayerRowChanging
      {
         add
         {
            m_dsPrj.Layer.LayerRowChanging += value;
         }
         remove
         {
            m_dsPrj.Layer.LayerRowChanging -= value;
         }
      }
      public static event ProjectDataset.LayerRowChangeEventHandler LayerRowDeleted
      {
         add
         {
            m_dsPrj.Layer.LayerRowDeleted += value;
         }
         remove
         {
            m_dsPrj.Layer.LayerRowDeleted -= value;
         }
      }
      public static void AddLayerRow(ProjectDataset.LayerRow row)
      {
         m_dsPrj.Layer.AddLayerRow(row);
      }
      public static ProjectDataset.LayerRow NewLayer()
      {
         return m_dsPrj.Layer.NewLayerRow();
      }
      public static ProjectDataset.LayerRow GetLayer(string Map, string Name)
      {
         return m_dsPrj.Layer.Rows.Find(new object[] {Map, Name}) as ProjectDataset.LayerRow;
      }
      public static ProjectDataset.LayerDataTable Layer
      {
         get
         {
            return m_dsPrj.Layer;
         }
      }
      #endregion

      #region Category
      public static event ProjectDataset.CategoryRowChangeEventHandler CategoryRowChanged
      {
         add
         {
            m_dsPrj.Category.CategoryRowChanged += value;
         }
         remove
         {
            m_dsPrj.Category.CategoryRowChanged -= value;
         }
      }
      public static event ProjectDataset.CategoryRowChangeEventHandler CategoryRowChanging
      {
         add
         {
            m_dsPrj.Category.CategoryRowChanging += value;
         }
         remove
         {
            m_dsPrj.Category.CategoryRowChanging -= value;
         }
      }
      public static event ProjectDataset.CategoryRowChangeEventHandler CategoryRowDeleted
      {
         add
         {
            m_dsPrj.Category.CategoryRowDeleted += value;
         }
         remove
         {
            m_dsPrj.Category.CategoryRowDeleted -= value;
         }
      }
      public static ProjectDataset.CategoryRow AddCategoryRow(ProjectDataset.TilesetRow Parent, string Name)
      {
         return m_dsPrj.Category.AddCategoryRow(Parent, Name);
      }
      public static ProjectDataset.CategoryRow NewCategory()
      {
         return m_dsPrj.Category.NewCategoryRow();
      }
      public static ProjectDataset.CategoryRow GetCategory(string Tileset, string Name)
      {
         return m_dsPrj.Category.FindByTilesetName(Tileset, Name);
      }
      public static ProjectDataset.CategoryDataTable Category
      {
         get
         {
            return m_dsPrj.Category;
         }
      }
      #endregion

      #region CategoryTile
      public static event ProjectDataset.CategoryTileRowChangeEventHandler CategoryTileRowChanged
      {
         add
         {
            m_dsPrj.CategoryTile.CategoryTileRowChanged += value;
         }
         remove
         {
            m_dsPrj.CategoryTile.CategoryTileRowChanged -= value;
         }
      }
      public static event ProjectDataset.CategoryTileRowChangeEventHandler CategoryTileRowChanging
      {
         add
         {
            m_dsPrj.CategoryTile.CategoryTileRowChanging += value;
         }
         remove
         {
            m_dsPrj.CategoryTile.CategoryTileRowChanging -= value;
         }
      }
      public static event ProjectDataset.CategoryTileRowChangeEventHandler CategoryTileRowDeleted
      {
         add
         {
            m_dsPrj.CategoryTile.CategoryTileRowDeleted += value;
         }
         remove
         {
            m_dsPrj.CategoryTile.CategoryTileRowDeleted -= value;
         }
      }
      public static ProjectDataset.CategoryTileRow AddCategoryTileRow(string Tileset, string Category, int TileValue)
      {
         return m_dsPrj.CategoryTile.AddCategoryTileRow(Tileset, Category, TileValue);
      }
      public static ProjectDataset.CategoryTileRow GetCategoryTileRow(string Tileset, string Category, int TileValue)
      {
         return m_dsPrj.CategoryTile.FindByTilesetCategoryTileValue(Tileset, Category, TileValue);
      }
      public static ProjectDataset.CategoryTileRow NewCategoryTile()
      {
         return m_dsPrj.CategoryTile.NewCategoryTileRow();
      }
      public static ProjectDataset.CategoryTileDataTable CategoryTile
      {
         get
         {
            return m_dsPrj.CategoryTile;
         }
      }
      #endregion

      #region SpriteDefinition
      public static event ProjectDataset.SpriteDefinitionRowChangeEventHandler SpriteDefinitionRowChanged
      {
         add
         {
            m_dsPrj.SpriteDefinition.SpriteDefinitionRowChanged += value;
         }
         remove
         {
            m_dsPrj.SpriteDefinition.SpriteDefinitionRowChanged -= value;
         }
      }
      public static event ProjectDataset.SpriteDefinitionRowChangeEventHandler SpriteDefinitionRowChanging
      {
         add
         {
            m_dsPrj.SpriteDefinition.SpriteDefinitionRowChanging += value;
         }
         remove
         {
            m_dsPrj.SpriteDefinition.SpriteDefinitionRowChanging -= value;
         }
      }
      public static event ProjectDataset.SpriteDefinitionRowChangeEventHandler SpriteDefinitionRowDeleted
      {
         add
         {
            m_dsPrj.SpriteDefinition.SpriteDefinitionRowDeleted += value;
         }
         remove
         {
            m_dsPrj.SpriteDefinition.SpriteDefinitionRowDeleted -= value;
         }
      }
      public static ProjectDataset.SpriteDefinitionDataTable SpriteDefinition
      {
         get
         {
            return m_dsPrj.SpriteDefinition;
         }
      }
      public static ProjectDataset.SpriteDefinitionRow GetSpriteDefinition(string Name)
      {
         return m_dsPrj.SpriteDefinition.FindByName(Name);
      }
      public static ProjectDataset.SpriteDefinitionRow AddSpriteDefinition(string Name)
      {
         return m_dsPrj.SpriteDefinition.AddSpriteDefinitionRow(Name);
      }
      #endregion

      #region SpriteState
      public static event ProjectDataset.SpriteStateRowChangeEventHandler SpriteStateRowChanged
      {
         add
         {
            m_dsPrj.SpriteState.SpriteStateRowChanged += value;
         }
         remove
         {
            m_dsPrj.SpriteState.SpriteStateRowChanged -= value;
         }
      }
      public static event ProjectDataset.SpriteStateRowChangeEventHandler SpriteStateRowChanging
      {
         add
         {
            m_dsPrj.SpriteState.SpriteStateRowChanging += value;
         }
         remove
         {
            m_dsPrj.SpriteState.SpriteStateRowChanging -= value;
         }
      }
      public static event ProjectDataset.SpriteStateRowChangeEventHandler SpriteStateRowDeleted
      {
         add
         {
            m_dsPrj.SpriteState.SpriteStateRowDeleted += value;
         }
         remove
         {
            m_dsPrj.SpriteState.SpriteStateRowDeleted -= value;
         }
      }
      public static ProjectDataset.SpriteStateDataTable SpriteState
      {
         get
         {
            return m_dsPrj.SpriteState;
         }
      }
      public static ProjectDataset.SpriteStateRow GetSpriteState(string SpriteDefinition, string State)
      {
         return m_dsPrj.SpriteState.FindByDefinitionNameName(SpriteDefinition, State);
      }
      public static ProjectDataset.SpriteStateRow AddSpriteState(ProjectDataset.SpriteDefinitionRow parent, string Name, ProjectDataset.FramesetRow Frameset, ProjectDataset.SolidityRow Solidity)
      {
         return m_dsPrj.SpriteState.AddSpriteStateRow(parent, Name, Frameset, Solidity);
      }
      public static ProjectDataset.SpriteFrameRow[] GetSortedSpriteFrames(ProjectDataset.SpriteStateRow row)
      {
         ProjectDataset.SpriteFrameRow[] result = row.GetSpriteFrameRows();
         Array.Sort(result, new DataRowComparer());
         return result;
      }
      #endregion

      #region SpriteFrame
      public static event ProjectDataset.SpriteFrameRowChangeEventHandler SpriteFrameRowChanged
      {
         add
         {
            m_dsPrj.SpriteFrame.SpriteFrameRowChanged += value;
         }
         remove
         {
            m_dsPrj.SpriteFrame.SpriteFrameRowChanged -= value;
         }
      }
      public static event ProjectDataset.SpriteFrameRowChangeEventHandler SpriteFrameRowChanging
      {
         add
         {
            m_dsPrj.SpriteFrame.SpriteFrameRowChanging += value;
         }
         remove
         {
            m_dsPrj.SpriteFrame.SpriteFrameRowChanging -= value;
         }
      }
      public static event ProjectDataset.SpriteFrameRowChangeEventHandler SpriteFrameRowDeleted
      {
         add
         {
            m_dsPrj.SpriteFrame.SpriteFrameRowDeleted += value;
         }
         remove
         {
            m_dsPrj.SpriteFrame.SpriteFrameRowDeleted -= value;
         }
      }
      public static ProjectDataset.SpriteFrameDataTable SpriteFrame
      {
         get
         {
            return m_dsPrj.SpriteFrame;
         }
      }
      public static void MoveSpriteFrame(ProjectDataset.SpriteFrameRow row, short NewSequence)
      {
         try
         {
            ProjectDataset.SpriteFrameRow[] rows = GetSortedSpriteFrames(row.SpriteStateRowParent);

            if (NewSequence >= rows.Length)
               throw new ArgumentOutOfRangeException("NewSequence", NewSequence, "Paremeter must be less than collection size (" + rows.Length + ")");

            if (NewSequence == row.Sequence)
               return;

            short nOffsetDir = (short)((row.Sequence < NewSequence) ? -1 : 1);
            short nIdx = (short)(row.Sequence-nOffsetDir);

            row.Sequence = (short)rows.Length;

            for (; nIdx != NewSequence-nOffsetDir; nIdx-=nOffsetDir)
            {
               System.Diagnostics.Debug.Assert(rows[nIdx].Sequence==nIdx);
               rows[nIdx].Sequence = (short)(nIdx + nOffsetDir);
            }
            m_dsPrj.SpriteFrame.AcceptChanges();
            
            row.Sequence = NewSequence;
         }
         catch (System.Exception)
         {
            RejectChanges();
            throw;
         }
      }
      public static ProjectDataset.SpriteFrameRow InsertFrame(ProjectDataset.SpriteStateRow parent, short Sequence, int FrameValue, short Duration)
      {
         try
         {
            ProjectDataset.SpriteFrameRow[] rows = GetSortedSpriteFrames(parent);

            if (Sequence < 0)
            {
               if (rows.Length == 0)
                  Sequence = 0;
               else
                  Sequence = (short)(rows[rows.Length-1].Sequence + 1);
            }

            if (Sequence > rows.Length)
               throw new ArgumentOutOfRangeException("Sequence", Sequence, "Sequence cannot be greater than row count (" + rows.Length.ToString() + ")");

            for (short nIdx = (short)(rows.Length - 1); nIdx >= Sequence; nIdx--)
            {
               System.Diagnostics.Debug.Assert(rows[nIdx].Sequence == nIdx);
               rows[nIdx].Sequence =(short)(nIdx + 1);
            }
            
            return m_dsPrj.SpriteFrame.AddSpriteFrameRow(parent.SpriteDefinitionRow.Name, parent.Name, Sequence, FrameValue, Duration);
         }
         catch (System.Exception)
         {
            RejectChanges();
            throw;
         }
      }

      public static void DeleteSpriteFrame(ProjectDataset.SpriteFrameRow row)
      {
         try
         {
            short nSequence = row.Sequence;
            ProjectDataset.SpriteStateRow srParent = row.SpriteStateRowParent;
            row.Delete();
            ProjectDataset.SpriteFrameRow[] rows = GetSortedSpriteFrames(srParent);

            for (short nIdx = nSequence; nIdx < rows.Length; nIdx++)
            {
               System.Diagnostics.Debug.Assert(rows[nIdx].Sequence - 1 == nIdx);
               rows[nIdx].Sequence = (short)(nIdx);
            }
         }
         catch (System.Exception)
         {
            RejectChanges();
            throw;
         }
      }

      #endregion
      
      #region SpriteRule
      public static event ProjectDataset.SpriteRuleRowChangeEventHandler SpriteRuleRowChanged
      {
         add
         {
            m_dsPrj.SpriteRule.SpriteRuleRowChanged += value;
         }
         remove
         {
            m_dsPrj.SpriteRule.SpriteRuleRowChanged -= value;
         }
      }
      public static event ProjectDataset.SpriteRuleRowChangeEventHandler SpriteRuleRowChanging
      {
         add
         {
            m_dsPrj.SpriteRule.SpriteRuleRowChanging += value;
         }
         remove
         {
            m_dsPrj.SpriteRule.SpriteRuleRowChanging -= value;
         }
      }
      public static event ProjectDataset.SpriteRuleRowChangeEventHandler SpriteRuleRowDeleted
      {
         add
         {
            m_dsPrj.SpriteRule.SpriteRuleRowDeleted += value;
         }
         remove
         {
            m_dsPrj.SpriteRule.SpriteRuleRowDeleted -= value;
         }
      }
      public static ProjectDataset.SpriteRuleDataTable SpriteRule
      {
         get
         {
            return m_dsPrj.SpriteRule;
         }
      }
      #endregion

      #region SpriteParameter
      public static event ProjectDataset.SpriteParameterRowChangeEventHandler SpriteParameterRowChanged
      {
         add
         {
            m_dsPrj.SpriteParameter.SpriteParameterRowChanged += value;
         }
         remove
         {
            m_dsPrj.SpriteParameter.SpriteParameterRowChanged -= value;
         }
      }
      public static event ProjectDataset.SpriteParameterRowChangeEventHandler SpriteParameterRowChanging
      {
         add
         {
            m_dsPrj.SpriteParameter.SpriteParameterRowChanging += value;
         }
         remove
         {
            m_dsPrj.SpriteParameter.SpriteParameterRowChanging -= value;
         }
      }
      public static event ProjectDataset.SpriteParameterRowChangeEventHandler SpriteParameterRowDeleted
      {
         add
         {
            m_dsPrj.SpriteParameter.SpriteParameterRowDeleted += value;
         }
         remove
         {
            m_dsPrj.SpriteParameter.SpriteParameterRowDeleted -= value;
         }
      }
      public static ProjectDataset.SpriteParameterDataTable SpriteParameter
      {
         get
         {
            return m_dsPrj.SpriteParameter;
         }
      }
      #endregion
      
      #region SpriteCategory
      public static event ProjectDataset.SpriteCategoryRowChangeEventHandler SpriteCategoryRowChanged
      {
         add
         {
            m_dsPrj.SpriteCategory.SpriteCategoryRowChanged += value;
         }
         remove
         {
            m_dsPrj.SpriteCategory.SpriteCategoryRowChanged -= value;
         }
      }
      public static event ProjectDataset.SpriteCategoryRowChangeEventHandler SpriteCategoryRowChanging
      {
         add
         {
            m_dsPrj.SpriteCategory.SpriteCategoryRowChanging += value;
         }
         remove
         {
            m_dsPrj.SpriteCategory.SpriteCategoryRowChanging -= value;
         }
      }
      public static event ProjectDataset.SpriteCategoryRowChangeEventHandler SpriteCategoryRowDeleted
      {
         add
         {
            m_dsPrj.SpriteCategory.SpriteCategoryRowDeleted += value;
         }
         remove
         {
            m_dsPrj.SpriteCategory.SpriteCategoryRowDeleted -= value;
         }
      }
      public static ProjectDataset.SpriteCategoryDataTable SpriteCategory
      {
         get
         {
            return m_dsPrj.SpriteCategory;
         }
      }
      public static ProjectDataset.SpriteCategoryRow AddSpriteCategory(string Name)
      {
         return m_dsPrj.SpriteCategory.AddSpriteCategoryRow(Name);
      }
      public static ProjectDataset.SpriteCategoryRow GetSpriteCategory(string Name)
      {
         return m_dsPrj.SpriteCategory.FindByName(Name);
      }
      #endregion

      #region SpriteCategorySprite
      public static event ProjectDataset.SpriteCategorySpriteRowChangeEventHandler SpriteCategorySpriteRowChanged
      {
         add
         {
            m_dsPrj.SpriteCategorySprite.SpriteCategorySpriteRowChanged += value;
         }
         remove
         {
            m_dsPrj.SpriteCategorySprite.SpriteCategorySpriteRowChanged -= value;
         }
      }
      public static event ProjectDataset.SpriteCategorySpriteRowChangeEventHandler SpriteCategorySpriteRowChanging
      {
         add
         {
            m_dsPrj.SpriteCategorySprite.SpriteCategorySpriteRowChanging += value;
         }
         remove
         {
            m_dsPrj.SpriteCategorySprite.SpriteCategorySpriteRowChanging -= value;
         }
      }
      public static event ProjectDataset.SpriteCategorySpriteRowChangeEventHandler SpriteCategorySpriteRowDeleted
      {
         add
         {
            m_dsPrj.SpriteCategorySprite.SpriteCategorySpriteRowDeleted += value;
         }
         remove
         {
            m_dsPrj.SpriteCategorySprite.SpriteCategorySpriteRowDeleted -= value;
         }
      }
      public static ProjectDataset.SpriteCategorySpriteDataTable SpriteCategorySprite
      {
         get
         {
            return m_dsPrj.SpriteCategorySprite;
         }
      }
      public static bool IsSpriteDefinitionInCategory(string CategoryName, string SpriteDefName)
      {
         return m_dsPrj.SpriteCategorySprite.FindByCategoryNameDefinitionName(CategoryName, SpriteDefName) != null;
      }
      public static ProjectDataset.SpriteCategorySpriteRow AddSpriteDefinitionToCategory(ProjectDataset.SpriteCategoryRow SpriteCategory, ProjectDataset.SpriteDefinitionRow SpriteDefinition)
      {
         ProjectDataset.SpriteCategorySpriteRow dr = m_dsPrj.SpriteCategorySprite.FindByCategoryNameDefinitionName(SpriteCategory.Name, SpriteDefinition.Name);
         if (dr == null)
            return m_dsPrj.SpriteCategorySprite.AddSpriteCategorySpriteRow(SpriteCategory, SpriteDefinition);
         return dr;
      }
      public static void RemoveSpriteDefinitionFromCategory(string CategoryName, string SpriteDefName)
      {
         ProjectDataset.SpriteCategorySpriteRow dr = m_dsPrj.SpriteCategorySprite.FindByCategoryNameDefinitionName(CategoryName, SpriteDefName);
         if (dr != null)
            dr.Delete();
      }
      #endregion
      
      #region Sprite
      public static event ProjectDataset.SpriteRowChangeEventHandler SpriteRowChanged
      {
         add
         {
            m_dsPrj.Sprite.SpriteRowChanged += value;
         }
         remove
         {
            m_dsPrj.Sprite.SpriteRowChanged -= value;
         }
      }
      public static event ProjectDataset.SpriteRowChangeEventHandler SpriteRowChanging
      {
         add
         {
            m_dsPrj.Sprite.SpriteRowChanging += value;
         }
         remove
         {
            m_dsPrj.Sprite.SpriteRowChanging -= value;
         }
      }
      public static event ProjectDataset.SpriteRowChangeEventHandler SpriteRowDeleted
      {
         add
         {
            m_dsPrj.Sprite.SpriteRowDeleted += value;
         }
         remove
         {
            m_dsPrj.Sprite.SpriteRowDeleted -= value;
         }
      }
      public static ProjectDataset.SpriteDataTable Sprite
      {
         get
         {
            return m_dsPrj.Sprite;
         }
      }
      #endregion

      #region ParameterValue
      public static event ProjectDataset.ParameterValueRowChangeEventHandler ParameterValueRowChanged
      {
         add
         {
            m_dsPrj.ParameterValue.ParameterValueRowChanged += value;
         }
         remove
         {
            m_dsPrj.ParameterValue.ParameterValueRowChanged -= value;
         }
      }
      public static event ProjectDataset.ParameterValueRowChangeEventHandler ParameterValueRowChanging
      {
         add
         {
            m_dsPrj.ParameterValue.ParameterValueRowChanging += value;
         }
         remove
         {
            m_dsPrj.ParameterValue.ParameterValueRowChanging -= value;
         }
      }
      public static event ProjectDataset.ParameterValueRowChangeEventHandler ParameterValueRowDeleted
      {
         add
         {
            m_dsPrj.ParameterValue.ParameterValueRowDeleted += value;
         }
         remove
         {
            m_dsPrj.ParameterValue.ParameterValueRowDeleted -= value;
         }
      }
      public static ProjectDataset.ParameterValueDataTable ParameterValue
      {
         get
         {
            return m_dsPrj.ParameterValue;
         }
      }
      #endregion

      #region SpritePlan
      public static event ProjectDataset.SpritePlanRowChangeEventHandler SpritePlanRowChanged
      {
         add
         {
            m_dsPrj.SpritePlan.SpritePlanRowChanged += value;
         }
         remove
         {
            m_dsPrj.SpritePlan.SpritePlanRowChanged -= value;
         }
      }
      public static event ProjectDataset.SpritePlanRowChangeEventHandler SpritePlanRowChanging
      {
         add
         {
            m_dsPrj.SpritePlan.SpritePlanRowChanging += value;
         }
         remove
         {
            m_dsPrj.SpritePlan.SpritePlanRowChanging -= value;
         }
      }
      public static event ProjectDataset.SpritePlanRowChangeEventHandler SpritePlanRowDeleted
      {
         add
         {
            m_dsPrj.SpritePlan.SpritePlanRowDeleted += value;
         }
         remove
         {
            m_dsPrj.SpritePlan.SpritePlanRowDeleted -= value;
         }
      }
      public static ProjectDataset.SpritePlanDataTable SpritePlan
      {
         get
         {
            return m_dsPrj.SpritePlan;
         }
      }
      #endregion

      #region Coordinate
      public static event ProjectDataset.CoordinateRowChangeEventHandler CoordinateRowChanged
      {
         add
         {
            m_dsPrj.Coordinate.CoordinateRowChanged += value;
         }
         remove
         {
            m_dsPrj.Coordinate.CoordinateRowChanged -= value;
         }
      }
      public static event ProjectDataset.CoordinateRowChangeEventHandler CoordinateRowChanging
      {
         add
         {
            m_dsPrj.Coordinate.CoordinateRowChanging += value;
         }
         remove
         {
            m_dsPrj.Coordinate.CoordinateRowChanging -= value;
         }
      }
      public static event ProjectDataset.CoordinateRowChangeEventHandler CoordinateRowDeleted
      {
         add
         {
            m_dsPrj.Coordinate.CoordinateRowDeleted += value;
         }
         remove
         {
            m_dsPrj.Coordinate.CoordinateRowDeleted -= value;
         }
      }
      public static ProjectDataset.CoordinateDataTable Coordinate
      {
         get
         {
            return m_dsPrj.Coordinate;
         }
      }
      #endregion
      
      #region Solidity
      public static event ProjectDataset.SolidityRowChangeEventHandler SolidityRowChanged
      {
         add
         {
            m_dsPrj.Solidity.SolidityRowChanged += value;
         }
         remove
         {
            m_dsPrj.Solidity.SolidityRowChanged -= value;
         }
      }
      public static event ProjectDataset.SolidityRowChangeEventHandler SolidityRowChanging
      {
         add
         {
            m_dsPrj.Solidity.SolidityRowChanging += value;
         }
         remove
         {
            m_dsPrj.Solidity.SolidityRowChanging -= value;
         }
      }
      public static event ProjectDataset.SolidityRowChangeEventHandler SolidityRowDeleted
      {
         add
         {
            m_dsPrj.Solidity.SolidityRowDeleted += value;
         }
         remove
         {
            m_dsPrj.Solidity.SolidityRowDeleted -= value;
         }
      }
      public static ProjectDataset.SolidityDataTable Solidity
      {
         get
         {
            return m_dsPrj.Solidity;
         }
      }
      public static ProjectDataset.SolidityRow AddSolidity(string Name)
      {
         return m_dsPrj.Solidity.AddSolidityRow(Name);
      }
      public static ProjectDataset.SolidityRow GetSolidity(string Name)
      {
         return m_dsPrj.Solidity.FindByName(Name);
      }
      #endregion

      #region SolidityShape
      public static event ProjectDataset.SolidityShapeRowChangeEventHandler SolidityShapeRowChanged
      {
         add
         {
            m_dsPrj.SolidityShape.SolidityShapeRowChanged += value;
         }
         remove
         {
            m_dsPrj.SolidityShape.SolidityShapeRowChanged -= value;
         }
      }
      public static event ProjectDataset.SolidityShapeRowChangeEventHandler SolidityShapeRowChanging
      {
         add
         {
            m_dsPrj.SolidityShape.SolidityShapeRowChanging += value;
         }
         remove
         {
            m_dsPrj.SolidityShape.SolidityShapeRowChanging -= value;
         }
      }
      public static event ProjectDataset.SolidityShapeRowChangeEventHandler SolidityShapeRowDeleted
      {
         add
         {
            m_dsPrj.SolidityShape.SolidityShapeRowDeleted += value;
         }
         remove
         {
            m_dsPrj.SolidityShape.SolidityShapeRowDeleted -= value;
         }
      }
      public static ProjectDataset.SolidityShapeDataTable SolidityShape
      {
         get
         {
            return m_dsPrj.SolidityShape;
         }
      }
      #endregion

      #region TileShape
      public static event ProjectDataset.TileShapeRowChangeEventHandler TileShapeRowChanged
      {
         add
         {
            m_dsPrj.TileShape.TileShapeRowChanged += value;
         }
         remove
         {
            m_dsPrj.TileShape.TileShapeRowChanged -= value;
         }
      }
      public static event ProjectDataset.TileShapeRowChangeEventHandler TileShapeRowChanging
      {
         add
         {
            m_dsPrj.TileShape.TileShapeRowChanging += value;
         }
         remove
         {
            m_dsPrj.TileShape.TileShapeRowChanging -= value;
         }
      }
      public static event ProjectDataset.TileShapeRowChangeEventHandler TileShapeRowDeleted
      {
         add
         {
            m_dsPrj.TileShape.TileShapeRowDeleted += value;
         }
         remove
         {
            m_dsPrj.TileShape.TileShapeRowDeleted -= value;
         }
      }
      public static ProjectDataset.TileShapeDataTable TileShape
      {
         get
         {
            return m_dsPrj.TileShape;
         }
      }
      public static ProjectDataset.TileShapeRow GetTileShape(string Name)
      {
         return m_dsPrj.TileShape.FindByName(Name);
      }
      public static ProjectDataset.TileShapeRow AddTileShape(string Name, string Shape)
      {
         return m_dsPrj.TileShape.AddTileShapeRow(Name, Shape);
      }
      #endregion

      #region CategoryFrame
      public static event ProjectDataset.CategoryFrameRowChangeEventHandler CategoryFrameRowChanged
      {
         add
         {
            m_dsPrj.CategoryFrame.CategoryFrameRowChanged += value;
         }
         remove
         {
            m_dsPrj.CategoryFrame.CategoryFrameRowChanged -= value;
         }
      }
      public static event ProjectDataset.CategoryFrameRowChangeEventHandler CategoryFrameRowChanging
      {
         add
         {
            m_dsPrj.CategoryFrame.CategoryFrameRowChanging += value;
         }
         remove
         {
            m_dsPrj.CategoryFrame.CategoryFrameRowChanging -= value;
         }
      }
      public static event ProjectDataset.CategoryFrameRowChangeEventHandler CategoryFrameRowDeleted
      {
         add
         {
            m_dsPrj.CategoryFrame.CategoryFrameRowDeleted += value;
         }
         remove
         {
            m_dsPrj.CategoryFrame.CategoryFrameRowDeleted -= value;
         }
      }
      public static ProjectDataset.CategoryFrameDataTable CategoryFrame
      {
         get
         {
            return m_dsPrj.CategoryFrame;
         }
      }
      public static void ResetCategoryFrames(string Tileset, string Category, int TileValue)
      {
         DataRow[] drFrames = m_dsPrj.CategoryFrame.Select("Tileset='" + Tileset + "' and Category='" + Category + "' and TileValue=" + TileValue.ToString());
         if (drFrames.Length > 0)
         {
            foreach(ProjectDataset.CategoryFrameRow delRow in drFrames)
               delRow.Delete();
         }
      }
      public static ProjectDataset.CategoryFrameRow AddCategoryFrameRow(string Tileset, string Category, int TileValue, short FrameValue)
      {
         ProjectDataset.CategoryTileRow parent = m_dsPrj.CategoryTile.FindByTilesetCategoryTileValue(Tileset, Category, TileValue);
         if (parent == null)
            parent = AddCategoryTileRow(Tileset, Category, TileValue);
         return m_dsPrj.CategoryFrame.AddCategoryFrameRow(Tileset, Category, TileValue, FrameValue);
      }
      public static ProjectDataset.CategoryFrameRow GetCategoryFrameRow(string Tileset, string Category, int TileValue, short FrameValue)
      {
         return m_dsPrj.CategoryFrame.FindByTilesetCategoryTileValueFrame(Tileset, Category, TileValue, FrameValue);
      }
      public static void DeleteCategoryFrameRow(string Tileset, string Category, int TileValue, short FrameValue)
      {
         ProjectDataset.CategoryFrameRow deleteRow = m_dsPrj.CategoryFrame.FindByTilesetCategoryTileValueFrame(Tileset, Category, TileValue, FrameValue);
         if (deleteRow.CategoryTileRowParent.GetCategoryFrameRows().Length <= 1)
            deleteRow.CategoryTileRowParent.Delete();
         else
            deleteRow.Delete();
      }
      #endregion

      #region SourceCode
      public static event ProjectDataset.SourceCodeRowChangeEventHandler SourceCodeRowChanged
      {
         add
         {
            m_dsPrj.SourceCode.SourceCodeRowChanged += value;
         }
         remove
         {
            m_dsPrj.SourceCode.SourceCodeRowChanged -= value;
         }
      }
      public static event ProjectDataset.SourceCodeRowChangeEventHandler SourceCodeRowChanging
      {
         add
         {
            m_dsPrj.SourceCode.SourceCodeRowChanging += value;
         }
         remove
         {
            m_dsPrj.SourceCode.SourceCodeRowChanging -= value;
         }
      }
      public static event ProjectDataset.SourceCodeRowChangeEventHandler SourceCodeRowDeleted
      {
         add
         {
            m_dsPrj.SourceCode.SourceCodeRowDeleted += value;
         }
         remove
         {
            m_dsPrj.SourceCode.SourceCodeRowDeleted -= value;
         }
      }
      public static ProjectDataset.SourceCodeDataTable SourceCode
      {
         get
         {
            return m_dsPrj.SourceCode;
         }
      }
      public static ProjectDataset.SourceCodeRow GetSourceCode(string Name)
      {
         return m_dsPrj.SourceCode.FindByName(Name);
      }
      public static ProjectDataset.SourceCodeRow AddSourceCode(string Name, string Text)
      {
         return m_dsPrj.SourceCode.AddSourceCodeRow(Name, Text);
      }
      #endregion
   }
}
