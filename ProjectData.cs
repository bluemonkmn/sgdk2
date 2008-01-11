/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
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

      [Serializable()]
      public struct CopiedRule
      {
         public string Name;
         public string Type;
         public string Function;
         public string Parameter1;
         public string Parameter2;
         public string Parameter3;
         public string ResultParameter;
         public bool EndIf;
         public bool Suspended;
         int source;

         public CopiedRule(ProjectDataset.PlanRuleRow rule)
         {
            Name = rule.Name;
            Type = rule.Type;
            Function = rule.Function;
            if (rule.IsParameter1Null())
               Parameter1 = null;
            else
               Parameter1 = rule.Parameter1;
            if (rule.IsParameter2Null())
               Parameter2 = null;
            else
               Parameter2 = rule.Parameter2;
            if (rule.IsParameter3Null())
               Parameter3 = null;
            else
               Parameter3 = rule.Parameter3;
            if (rule.IsResultParameterNull())
               ResultParameter = null;
            else
               ResultParameter = rule.ResultParameter;
            EndIf = rule.EndIf;
            Suspended = rule.Suspended;
            source = 0;
         }

         public CopiedRule(ProjectDataset.SpriteRuleRow rule)
         {
            Name = rule.Name;
            Type = rule.Type;
            Function = rule.Function;
            if (rule.IsParameter1Null())
               Parameter1 = null;
            else
               Parameter1 = rule.Parameter1;
            if (rule.IsParameter2Null())
               Parameter2 = null;
            else
               Parameter2 = rule.Parameter2;
            if (rule.IsParameter3Null())
               Parameter3 = null;
            else
               Parameter3 = rule.Parameter3;
            if (rule.IsResultParameterNull())
               ResultParameter = null;
            else
               ResultParameter = rule.ResultParameter;
            EndIf = rule.EndIf;
            Suspended = rule.Suspended;
            source = 1;
         }

         public bool IsSpriteDefinitionRule
         {
            get
            {
               return (source == 1);
            }
         }

         public bool IsPlanRule
         {
            get
            {
               return (source == 0);
            }
         }
      }

      // There should be no instances of this class
      private ProjectData()
      {
      }

      static ProjectData()
      {
         m_dsPrj.SpriteDefinition.DefaultView.Sort = ProjectData.SpriteDefinition.NameColumn.ColumnName;
         m_dsPrj.Solidity.DefaultView.Sort = ProjectData.Solidity.NameColumn.ColumnName;
      }

      #region General/Dataset
      public static event System.EventHandler Clearing;

      public static void AcceptChanges()
      {
         m_dsPrj.AcceptChanges();
      }

      public static DataTable[] GetChangedTables()
      {
         System.Collections.ArrayList tables = new ArrayList();
         foreach (DataTable dt in m_dsPrj.Tables)
         {
            DataTable chg = dt.GetChanges();
            if ((chg != null) && (chg.Rows.Count > 0))
               tables.Add(chg);
         }

         return (DataTable[])tables.ToArray(typeof(DataTable));
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
      /// <summary>
      /// Check the validity of a name for an object
      /// </summary>
      /// <param name="Name">Proposed name</param>
      /// <returns>An error message or null if the name is acceptable.</returns>
      public static string ValidateName(string Name)
      {
         if (Name.Length == 0)
            return "Name cannot be empty";

         if (Name.EndsWith(" t"))
            return "Name cannot end with \" t\"";

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
      public static bool EnforceConstraints
      {
         get
         {
            return m_dsPrj.EnforceConstraints;
         }
         set
         {
            m_dsPrj.EnforceConstraints = value;
         }
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
            else if (x is ProjectDataset.SpriteRow)
               return ((ProjectDataset.SpriteRow)x).Priority.CompareTo(((ProjectDataset.SpriteRow)y).Priority);
            else if (x is ProjectDataset.SpriteParameterRow)
               return ((ProjectDataset.SpriteParameterRow)x).Name.CompareTo(((ProjectDataset.SpriteParameterRow)y).Name);
            else if (x is ProjectDataset.SpritePlanRow)
            {
               int result = ((ProjectDataset.SpritePlanRow)x).Priority.CompareTo(((ProjectDataset.SpritePlanRow)y).Priority);
               if (result == 0)
                  result = ((ProjectDataset.SpritePlanRow)x).Name.CompareTo(((ProjectDataset.SpritePlanRow)y).Name);
               return result;
            }
            else if (x is ProjectDataset.CoordinateRow)
               return ((ProjectDataset.CoordinateRow)x).Sequence.CompareTo(((ProjectDataset.CoordinateRow)y).Sequence);
            else if (x is ProjectDataset.PlanRuleRow)
               return ((ProjectDataset.PlanRuleRow)x).Sequence.CompareTo(((ProjectDataset.PlanRuleRow)y).Sequence);
            else if (x is ProjectDataset.SpriteRuleRow)
               return ((ProjectDataset.SpriteRuleRow)x).Sequence.CompareTo(((ProjectDataset.SpriteRuleRow)y).Sequence);
            else if (x is ProjectDataset.SpriteStateRow)
               return ((ProjectDataset.SpriteStateRow)x).Sequence.CompareTo(((ProjectDataset.SpriteStateRow)y).Sequence);
            else
               throw new ApplicationException("Unknown data row type for comparing");
         }
         #endregion
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
      public static event ProjectDataset.GraphicSheetRowChangeEventHandler GraphicSheetRowDeleting
      {
         add
         {
            m_dsPrj.GraphicSheet.GraphicSheetRowDeleting += value;
         }
         remove
         {
            m_dsPrj.GraphicSheet.GraphicSheetRowDeleting -= value;
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

      public static bool IsGraphicSheetDecapsulated(ProjectDataset.GraphicSheetRow row)
      {
         if (row.IsImageNull())
            return false;
         return System.Text.Encoding.UTF8.GetString(row.Image, 0, 7) == "~import";
      }

      public static string GetGraphicSheetCapsuleName(ProjectDataset.GraphicSheetRow row)
      {
         return System.Text.Encoding.UTF8.GetString(row.Image).Substring(7);
      }

      public static string GetGraphicSheetCapsulePath(string relativeTo, ProjectDataset.GraphicSheetRow row)
      {
         return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(relativeTo),
            GetGraphicSheetCapsuleName(row));
      }

      public static void ReencapsulateGraphicSheet(string relativeTo, ProjectDataset.GraphicSheetRow row)
      {
         if (IsGraphicSheetDecapsulated(row))
         {
            string capsulePath = GetGraphicSheetCapsulePath(relativeTo, row);
            try
            {
               using(System.IO.BinaryReader capsuleReader =
                        new System.IO.BinaryReader(System.IO.File.OpenRead(capsulePath)))
                  row.Image = capsuleReader.ReadBytes((int)capsuleReader.BaseStream.Length);
            }
            catch(System.Exception ex)
            {
               throw new ApplicationException("An error occurred trying to load the decapsulated image from \"" + capsulePath + "\": " + ex.Message, ex);
            }
         }
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

         Bitmap bmpStreamImage;
         ProjectDataset.GraphicSheetRow drGfx = ProjectData.GetGraphicSheet(Name);
         System.IO.MemoryStream ms = null;
         if (IsGraphicSheetDecapsulated(drGfx))
            try
            {
               bmpStreamImage = new Bitmap(ProjectData.GetGraphicSheetCapsulePath(SGDK2IDE.CurrentProjectFile, drGfx));
            }
            catch(System.Exception ex)
            {
               throw new ApplicationException("An error occurred attempting to load a decapsulated graphic sheet from \"" + ProjectData.GetGraphicSheetCapsulePath(SGDK2IDE.CurrentProjectFile, drGfx) + "\": " + ex.ToString(), ex);
            }
         else
         {
            ms = new System.IO.MemoryStream(ProjectData.GetGraphicSheet(Name).Image, false);
            bmpStreamImage = (Bitmap)Bitmap.FromStream(ms);
         }
         Bitmap bmpResult = new Bitmap(bmpStreamImage.Width, bmpStreamImage.Height,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
         SGDK2IDE.CopyImage(bmpResult, bmpStreamImage);
         bmpStreamImage.Dispose();
         if (ms != null)
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
      public static event ProjectDataset.FramesetRowChangeEventHandler FramesetRowDeleting
      {
         add
         {
            m_dsPrj.Frameset.FramesetRowDeleting += value;
         }
         remove
         {
            m_dsPrj.Frameset.FramesetRowDeleting -= value;
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
      public static event ProjectDataset.FrameRowChangeEventHandler FrameRowDeleting
      {
         add
         {
            m_dsPrj.Frame.FrameRowDeleting += value;
         }
         remove
         {
            m_dsPrj.Frame.FrameRowDeleting -= value;
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
         float m11, float m12, float m21, float m22, float dx, float dy, int color)
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
         drNew.color = color;
         DataRow[] ardr = m_dsPrj.Frame.Select("Name='" + parent.Name + "' AND FrameValue>=" + FrameValue.ToString(), "FrameValue DESC");
         m_dsPrj.Frame.Rows.InsertAt(drNew, FrameValue);
         foreach(ProjectDataset.FrameRow dr in ardr)
            dr.FrameValue++;
         drNew.FrameValue = FrameValue;
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
               row.FrameValue = NewFrameValue++;
            }
            else if (NewFrameValue > nSelVal)
            {
               DataRow[] ardr = m_dsPrj.Frame.Select(
                  "Name='" + row.FramesetRow.Name + "' AND FrameValue > " + nSelVal.ToString() +
                  " AND FrameValue < " + NewFrameValue.ToString(), "FrameValue");
               foreach(ProjectDataset.FrameRow fr in ardr)
                  fr.FrameValue--;
               row.FrameValue = NewFrameValue - 1;
               row.EndEdit();
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
      }
      public static ProjectDataset.FrameRow AddFrameRow(
         ProjectDataset.FramesetRow Frameset, int FrameValue, string GraphicSheet, short CellIndex,
         float m11, float m12, float m21, float m22, float dx, float dy, int color)
      {
         return m_dsPrj.Frame.AddFrameRow(FrameValue, GraphicSheet, CellIndex, m11, m12, m21, m22, dx, dy, Frameset, color);
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
      public static event ProjectDataset.TilesetRowChangeEventHandler TilesetRowDeleting
      {
         add
         {
            m_dsPrj.Tileset.TilesetRowDeleting += value;
         }
         remove
         {
            m_dsPrj.Tileset.TilesetRowDeleting -= value;
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
      public static event ProjectDataset.TileRowChangeEventHandler TileRowDeleting
      {
         add
         {
            m_dsPrj.Tile.TileRowDeleting += value;
         }
         remove
         {
            m_dsPrj.Tile.TileRowDeleting -= value;
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
      public static event ProjectDataset.TileFrameRowChangeEventHandler TileFrameRowDeleting
      {
         add
         {
            m_dsPrj.TileFrame.TileFrameRowDeleting += value;
         }
         remove
         {
            m_dsPrj.TileFrame.TileFrameRowDeleting -= value;
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
            
            row.Sequence = NewSequence;
         }
         catch (System.Exception)
         {
            RejectChanges();
            throw;
         }
      }

      public static ProjectDataset.TileFrameDataTable TileFrame
      {
         get
         {
            return m_dsPrj.TileFrame;
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
      public static event ProjectDataset.CounterRowChangeEventHandler CounterRowDeleting
      {
         add
         {
            m_dsPrj.Counter.CounterRowDeleting += value;
         }
         remove
         {
            m_dsPrj.Counter.CounterRowDeleting -= value;
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
      public static event ProjectDataset.MapRowChangeEventHandler MapRowDeleting
      {
         add
         {
            m_dsPrj.Map.MapRowDeleting += value;
         }
         remove
         {
            m_dsPrj.Map.MapRowDeleting -= value;
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
      public static event ProjectDataset.LayerRowChangeEventHandler LayerRowDeleting
      {
         add
         {
            m_dsPrj.Layer.LayerRowDeleting += value;
         }
         remove
         {
            m_dsPrj.Layer.LayerRowDeleting -= value;
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
      public static ProjectDataset.SpriteRow[] GetSortedSpriteRows(ProjectDataset.LayerRow row)
      {
         ProjectDataset.SpriteRow[] result = row.GetSpriteRows();
         Array.Sort(result, new DataRowComparer());
         return result;
      }
      #endregion

      #region TileCategory
      public static event ProjectDataset.TileCategoryRowChangeEventHandler TileCategoryRowChanged
      {
         add
         {
            m_dsPrj.TileCategory.TileCategoryRowChanged += value;
         }
         remove
         {
            m_dsPrj.TileCategory.TileCategoryRowChanged -= value;
         }
      }
      public static event ProjectDataset.TileCategoryRowChangeEventHandler TileCategoryRowChanging
      {
         add
         {
            m_dsPrj.TileCategory.TileCategoryRowChanging += value;
         }
         remove
         {
            m_dsPrj.TileCategory.TileCategoryRowChanging -= value;
         }
      }
      public static event ProjectDataset.TileCategoryRowChangeEventHandler TileCategoryRowDeleted
      {
         add
         {
            m_dsPrj.TileCategory.TileCategoryRowDeleted += value;
         }
         remove
         {
            m_dsPrj.TileCategory.TileCategoryRowDeleted -= value;
         }
      }
      public static event ProjectDataset.TileCategoryRowChangeEventHandler TileCategoryRowDeleting
      {
         add
         {
            m_dsPrj.TileCategory.TileCategoryRowDeleting += value;
         }
         remove
         {
            m_dsPrj.TileCategory.TileCategoryRowDeleting -= value;
         }
      }
      public static ProjectDataset.TileCategoryDataTable TileCategory
      {
         get
         {
            return m_dsPrj.TileCategory;
         }
      }
      public static ProjectDataset.TileCategoryRow AddTileCategoryRow(string Name)
      {
         return m_dsPrj.TileCategory.AddTileCategoryRow(Name);
      }
      public static ProjectDataset.TileCategoryRow NewTileCategory()
      {
         return m_dsPrj.TileCategory.NewTileCategoryRow();
      }
      public static ProjectDataset.TileCategoryRow GetTileCategory(string Name)
      {
         return m_dsPrj.TileCategory.FindByName(Name);
      }
      #endregion

      #region CategorizedTileset
      public static event ProjectDataset.CategorizedTilesetRowChangeEventHandler CategorizedTilesetRowChanged
      {
         add
         {
            m_dsPrj.CategorizedTileset.CategorizedTilesetRowChanged += value;
         }
         remove
         {
            m_dsPrj.CategorizedTileset.CategorizedTilesetRowChanged -= value;
         }
      }
      public static event ProjectDataset.CategorizedTilesetRowChangeEventHandler CategorizedTilesetRowChanging
      {
         add
         {
            m_dsPrj.CategorizedTileset.CategorizedTilesetRowChanging += value;
         }
         remove
         {
            m_dsPrj.CategorizedTileset.CategorizedTilesetRowChanging -= value;
         }
      }
      public static event ProjectDataset.CategorizedTilesetRowChangeEventHandler CategorizedTilesetRowDeleted
      {
         add
         {
            m_dsPrj.CategorizedTileset.CategorizedTilesetRowDeleted += value;
         }
         remove
         {
            m_dsPrj.CategorizedTileset.CategorizedTilesetRowDeleted -= value;
         }
      }
      public static event ProjectDataset.CategorizedTilesetRowChangeEventHandler CategorizedTilesetRowDeleting
      {
         add
         {
            m_dsPrj.CategorizedTileset.CategorizedTilesetRowDeleting += value;
         }
         remove
         {
            m_dsPrj.CategorizedTileset.CategorizedTilesetRowDeleting -= value;
         }
      }
      public static ProjectDataset.CategorizedTilesetDataTable CategorizedTileset
      {
         get
         {
            return m_dsPrj.CategorizedTileset;
         }
      }
      public static ProjectDataset.CategorizedTilesetRow AddCategorizedTilesetRow(ProjectDataset.TilesetRow Tileset, ProjectDataset.TileCategoryRow Category)
      {
         return m_dsPrj.CategorizedTileset.AddCategorizedTilesetRow(Tileset, Category);
      }
      public static ProjectDataset.CategorizedTilesetRow NewCategorizedTileset()
      {
         return m_dsPrj.CategorizedTileset.NewCategorizedTilesetRow();
      }
      public static ProjectDataset.CategorizedTilesetRow GetCategorizedTileset(string Tileset, string Name)
      {
         return m_dsPrj.CategorizedTileset.FindByTilesetName(Tileset, Name);
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
      public static event ProjectDataset.CategoryTileRowChangeEventHandler CategoryTileRowDeleting
      {
         add
         {
            m_dsPrj.CategoryTile.CategoryTileRowDeleting += value;
         }
         remove
         {
            m_dsPrj.CategoryTile.CategoryTileRowDeleting -= value;
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
      public static ProjectDataset.CategoryTileRow[] GetTileCategories(string Tileset, int TileValue)
      {
         DataRow[] drUntyped = m_dsPrj.CategoryTile.Select("TileValue=" + TileValue.ToString() + " and Tileset='" + Tileset.ToString() + "'");
         ProjectDataset.CategoryTileRow[] drResult = new SGDK2.ProjectDataset.CategoryTileRow[drUntyped.Length];
         drUntyped.CopyTo(drResult, 0);
         return drResult;
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
      public static event ProjectDataset.SpriteDefinitionRowChangeEventHandler SpriteDefinitionRowDeleting
      {
         add
         {
            m_dsPrj.SpriteDefinition.SpriteDefinitionRowDeleting += value;
         }
         remove
         {
            m_dsPrj.SpriteDefinition.SpriteDefinitionRowDeleting -= value;
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
      public static ProjectDataset.SpriteParameterRow[] GetSortedSpriteParameters(ProjectDataset.SpriteDefinitionRow row)
      {
         ProjectDataset.SpriteParameterRow[] result = row.GetSpriteParameterRows();
         Array.Sort(result, new DataRowComparer());
         return result;
      }
      public static ProjectDataset.SpriteStateRow[] GetSortedSpriteStates(ProjectDataset.SpriteDefinitionRow row)
      {
         ProjectDataset.SpriteStateRow[] result = row.GetSpriteStateRows();
         Array.Sort(result, new DataRowComparer());
         return result;
      }
      public static void ResyncParameters(ProjectDataset.SpriteDefinitionRow Definition)
      {
         foreach(ProjectDataset.SpriteRow drSprite in m_dsPrj.Sprite.Select(m_dsPrj.Sprite.DefinitionNameColumn.ColumnName + "='" + Definition.Name + "'", String.Empty, DataViewRowState.CurrentRows))
         {
            foreach(ProjectDataset.SpriteParameterRow drParam in Definition.GetSpriteParameterRows())
            {
               if (null == GetSpriteParameterValueRow(drSprite, drParam.Name))
               {
                  m_dsPrj.ParameterValue.AddParameterValueRow(drSprite[m_dsPrj.Sprite.LayerNameColumn].ToString(), drSprite.Name, drParam.Name, 0, Definition.Name, drSprite[m_dsPrj.Sprite.MapNameColumn].ToString());
               }
            }
         }
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
      public static event ProjectDataset.SpriteStateRowChangeEventHandler SpriteStateRowDeleting
      {
         add
         {
            m_dsPrj.SpriteState.SpriteStateRowDeleting += value;
         }
         remove
         {
            m_dsPrj.SpriteState.SpriteStateRowDeleting -= value;
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
      public static short GetMaxSpriteStateSequence(ProjectDataset.SpriteDefinitionRow parent)
      {
         short max = 0;
         foreach (ProjectDataset.SpriteStateRow row in parent.GetSpriteStateRows())
         {
            if (row.Sequence > max)
               max = row.Sequence;
         }
         return max;
      }
      public static ProjectDataset.SpriteStateRow AddSpriteState(ProjectDataset.SpriteDefinitionRow parent, string Name, ProjectDataset.FramesetRow Frameset, short SolidWidth, short SolidHeight, short sequence)
      {
         if (sequence < 0)
            sequence = (short)(GetMaxSpriteStateSequence(parent) + 1);
         else
            foreach(ProjectDataset.SpriteStateRow row in GetSortedSpriteStates(parent))
               if (row.Sequence >= sequence)
                  row.Sequence += 1;
         return m_dsPrj.SpriteState.AddSpriteStateRow(parent, Name, Frameset, SolidWidth, SolidHeight, sequence);
      }
      public static void DeleteSpriteState(ProjectDataset.SpriteStateRow row)
      {
         int oldSeq = row.Sequence;
         ProjectDataset.SpriteDefinitionRow parent = row.SpriteDefinitionRow;
         row.Delete();
         foreach(ProjectDataset.SpriteStateRow drChange in GetSortedSpriteStates(parent))
            if (drChange.Sequence >= oldSeq)
               drChange.Sequence -= 1;
      }
      public static ProjectDataset.SpriteFrameRow[] GetSortedSpriteFrames(ProjectDataset.SpriteStateRow row)
      {
         ProjectDataset.SpriteFrameRow[] result = row.GetSpriteFrameRows();
         Array.Sort(result, new DataRowComparer());
         return result;
      }
      public static bool MoveSpriteState(ProjectDataset.SpriteStateRow row, bool moveDown)
      {
         string definitionName = row[m_dsPrj.SpriteState.DefinitionNameColumn].ToString();
         string filter = "DefinitionName='" + definitionName + "' ";

         DataRow[] nextRows;
         if (moveDown)
            nextRows = m_dsPrj.SpriteState.Select(filter + "and Sequence >= " + row.Sequence.ToString(), "Sequence ASC");
         else
            nextRows = m_dsPrj.SpriteState.Select(filter + "and Sequence <= " + row.Sequence.ToString(), "Sequence DESC");
         System.Diagnostics.Debug.Assert((nextRows.Length > 0) && (nextRows[0] == row), "Unexpected sprite state sequencing error");
         if (nextRows.Length == 1)
            return false;
         ProjectDataset.SpriteStateRow nextRow = (ProjectDataset.SpriteStateRow)nextRows[1];
         short nextSeq = nextRow.Sequence;
         if (moveDown)
            System.Diagnostics.Debug.Assert(nextRow.Sequence == row.Sequence + 1, "Sprite state rows are not consecutively sequenced");
         else
            System.Diagnostics.Debug.Assert(nextRow.Sequence == row.Sequence - 1, "Sprite state rows are not consecutively sequenced");
         nextRow.Sequence = row.Sequence;
         row.Sequence = nextSeq;
         return true;
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
      public static event ProjectDataset.SpriteFrameRowChangeEventHandler SpriteFrameRowDeleting
      {
         add
         {
            m_dsPrj.SpriteFrame.SpriteFrameRowDeleting += value;
         }
         remove
         {
            m_dsPrj.SpriteFrame.SpriteFrameRowDeleting -= value;
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
            
            row.Sequence = NewSequence;
         }
         catch (System.Exception)
         {
            RejectChanges();
            throw;
         }
      }
      public static ProjectDataset.SpriteFrameRow InsertFrame(ProjectDataset.SpriteStateRow parent, short Sequence, int FrameValue, short Duration, byte MaskAlpha)
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
            
            return m_dsPrj.SpriteFrame.AddSpriteFrameRow(parent.SpriteDefinitionRow.Name, parent.Name, Sequence, FrameValue, Duration, MaskAlpha);
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
      public static event ProjectDataset.SpriteRuleRowChangeEventHandler SpriteRuleRowDeleting
      {
         add
         {
            m_dsPrj.SpriteRule.SpriteRuleRowDeleting += value;
         }
         remove
         {
            m_dsPrj.SpriteRule.SpriteRuleRowDeleting -= value;
         }
      }
      public static ProjectDataset.SpriteRuleDataTable SpriteRule
      {
         get
         {
            return m_dsPrj.SpriteRule;
         }
      }
      public static ProjectDataset.SpriteRuleRow[] GetSortedSpriteRules(ProjectDataset.SpriteDefinitionRow parent, bool includeSuspended)
      {
         if (!includeSuspended)
         {
            string filter = m_dsPrj.SpriteRule.DefinitionNameColumn.ColumnName + "='" + parent.Name +
               "' and Suspended=false";
            return (ProjectDataset.SpriteRuleRow[])m_dsPrj.SpriteRule.Select(filter, "Sequence");
         }
         ProjectDataset.SpriteRuleRow[] result = parent.GetSpriteRuleRows();
         Array.Sort(result, new DataRowComparer());
         return result;
      }
      public static ProjectDataset.SpriteRuleRow InsertSpriteRule(ProjectDataset.SpriteDefinitionRow parent, string name,
         string type, int sequence, string function, string parameter1, string parameter2, string parameter3, string resultParameter,
         bool endIf, bool suspended)
      {
         if (sequence < 0)
            sequence = GetMaxSpritePlanSequence(parent) + 1;
         else
            foreach(ProjectDataset.SpriteRuleRow row in GetSortedSpriteRules(parent,true))
               if (row.Sequence >= sequence)
                  row.Sequence += 1;
         return m_dsPrj.SpriteRule.AddSpriteRuleRow(parent, name, sequence, type, function, parameter1, parameter2, parameter3, resultParameter, endIf, suspended);
      }
      
      public static void DeleteSpriteRule(ProjectDataset.SpriteRuleRow row)
      {
         int oldSeq = row.Sequence;
         ProjectDataset.SpriteDefinitionRow parent = row.SpriteDefinitionRow;
         row.Delete();
         foreach(ProjectDataset.SpriteRuleRow drChange in GetSortedSpriteRules(parent,true))
            if (drChange.Sequence >= oldSeq)
               drChange.Sequence -= 1;
      }
      
      public static bool MoveSpriteRule(ProjectDataset.SpriteRuleRow row, bool moveDown)
      {
         string definitionName = row[m_dsPrj.SpriteRule.DefinitionNameColumn].ToString();
         string filter = "DefinitionName='" + definitionName + "' ";

         DataRow[] nextRows;
         if (moveDown)
            nextRows = m_dsPrj.SpriteRule.Select(filter + "and Sequence >= " + row.Sequence.ToString(), "Sequence ASC");
         else
            nextRows = m_dsPrj.SpriteRule.Select(filter + "and Sequence <= " + row.Sequence.ToString(), "Sequence DESC");
         System.Diagnostics.Debug.Assert((nextRows.Length > 0) && (nextRows[0] == row), "Unexpected sprite rule sequencing error");
         if (nextRows.Length == 1)
            return false;
         ProjectDataset.SpriteRuleRow nextRow = (ProjectDataset.SpriteRuleRow)nextRows[1];
         int nextSeq = nextRow.Sequence;
         if (moveDown)
            System.Diagnostics.Debug.Assert(nextRow.Sequence == row.Sequence + 1, "Sprite rows are not consecutively sequenced");
         else
            System.Diagnostics.Debug.Assert(nextRow.Sequence == row.Sequence - 1, "Sprite rows are not consecutively sequenced");
         nextRow.Sequence = row.Sequence;
         row.Sequence = nextSeq;
         return true;
      }

      public static ProjectDataset.SpriteRuleRow GetSpriteRule(ProjectDataset.SpriteDefinitionRow parent, string name)
      {
         return m_dsPrj.SpriteRule.FindByDefinitionNameName(parent.Name, name);
      }
      public static int GetMaxSpritePlanSequence(ProjectDataset.SpriteDefinitionRow parent)
      {
         int max = 0;
         foreach (ProjectDataset.SpriteRuleRow row in parent.GetSpriteRuleRows())
         {
            if (row.Sequence > max)
               max = row.Sequence;
         }
         return max;
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
      public static event ProjectDataset.SpriteParameterRowChangeEventHandler SpriteParameterRowDeleting
      {
         add
         {
            m_dsPrj.SpriteParameter.SpriteParameterRowDeleting += value;
         }
         remove
         {
            m_dsPrj.SpriteParameter.SpriteParameterRowDeleting -= value;
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
      public static event ProjectDataset.SpriteCategoryRowChangeEventHandler SpriteCategoryRowDeleting
      {
         add
         {
            m_dsPrj.SpriteCategory.SpriteCategoryRowDeleting += value;
         }
         remove
         {
            m_dsPrj.SpriteCategory.SpriteCategoryRowDeleting -= value;
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
      public static event ProjectDataset.SpriteCategorySpriteRowChangeEventHandler SpriteCategorySpriteRowDeleting
      {
         add
         {
            m_dsPrj.SpriteCategorySprite.SpriteCategorySpriteRowDeleting += value;
         }
         remove
         {
            m_dsPrj.SpriteCategorySprite.SpriteCategorySpriteRowDeleting -= value;
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
      public static event ProjectDataset.SpriteRowChangeEventHandler SpriteRowDeleting
      {
         add
         {
            m_dsPrj.Sprite.SpriteRowDeleting += value;
         }
         remove
         {
            m_dsPrj.Sprite.SpriteRowDeleting -= value;
         }
      }
      public static ProjectDataset.SpriteDataTable Sprite
      {
         get
         {
            return m_dsPrj.Sprite;
         }
      }
      public static ProjectDataset.ParameterValueRow GetSpriteParameterValueRow(ProjectDataset.SpriteRow row, string paramName)
      {
         string LayerName = (string)row[m_dsPrj.Sprite.LayerNameColumn];
         string MapName = (string)row[m_dsPrj.Sprite.MapNameColumn];
         return m_dsPrj.ParameterValue.FindByLayerNameSpriteNameParameterNameMapName(LayerName, row.Name, paramName, MapName);
      }
      public static ProjectDataset.SpriteRow AddSprite(string LayerName, string SpriteName,
         string DefinitionName, string StateName, short CurrentFrame, int X, int Y,
         float DX, float DY, string MapName, int Priority, bool Active, string Solidity, int Color, string[] ParamNames, int[] ParamValues)
      {
         if (GetSpritePlan(GetLayer(MapName, LayerName), SpriteName) != null)
            throw new ApplicationException("Sprite name \"" + SpriteName + "\" conflicts with an existing plan name.  Choose a name that does not conflict with that of a plan or another sprite.");
         ProjectDataset.SpriteRow drSprite = m_dsPrj.Sprite.AddSpriteRow(
            LayerName, SpriteName, DefinitionName, StateName, CurrentFrame,
            X, Y, DX, DY, MapName, Priority, Active, Solidity, Color);
         for (int i=0; i<ParamNames.Length; i++)
            m_dsPrj.ParameterValue.AddParameterValueRow(LayerName, SpriteName, ParamNames[i], 
               ParamValues[i], DefinitionName, MapName);
         return drSprite;
      }
      public static ProjectDataset.SpriteRow GetSprite(ProjectDataset.LayerRow layer, string name)
      {
         return m_dsPrj.Sprite.FindByLayerNameNameMapName(layer.Name, name, layer[m_dsPrj.Layer.MapNameColumn].ToString());
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
      public static event ProjectDataset.ParameterValueRowChangeEventHandler ParameterValueRowDeleting
      {
         add
         {
            m_dsPrj.ParameterValue.ParameterValueRowDeleting += value;
         }
         remove
         {
            m_dsPrj.ParameterValue.ParameterValueRowDeleting -= value;
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
      public static event ProjectDataset.SpritePlanRowChangeEventHandler SpritePlanRowDeleting
      {
         add
         {
            m_dsPrj.SpritePlan.SpritePlanRowDeleting += value;
         }
         remove
         {
            m_dsPrj.SpritePlan.SpritePlanRowDeleting -= value;
         }
      }
      public static ProjectDataset.SpritePlanDataTable SpritePlan
      {
         get
         {
            return m_dsPrj.SpritePlan;
         }
      }
      public static ProjectDataset.SpritePlanRow AddSpritePlan(ProjectDataset.LayerRow ParentLayer, string Name, int Priority)
      {
         if (ProjectData.GetSprite(ParentLayer, Name) != null)
            throw new ApplicationException("Plan name \"" + Name + "\" conflicts with that of a sprite.  Choose a name that doesn't conflict with that of a sprite or another plan.");
         return m_dsPrj.SpritePlan.AddSpritePlanRow(ParentLayer.MapRow.Name, ParentLayer.Name, Name, Priority);
      }
      public static ProjectDataset.SpritePlanRow GetSpritePlan(ProjectDataset.LayerRow ParentLayer, string Name)
      {
         return m_dsPrj.SpritePlan.FindByMapNameLayerNameName(ParentLayer.MapRow.Name, ParentLayer.Name, Name);
      }
      public static ProjectDataset.SpritePlanRow GetSpritePlan(string MapName, string LayerName, string Name)
      {
         return m_dsPrj.SpritePlan.FindByMapNameLayerNameName(MapName, LayerName, Name);
      }
      public static ProjectDataset.SpritePlanRow[] GetSortedSpritePlans(ProjectDataset.LayerRow ParentLayer)
      {
         ProjectDataset.SpritePlanRow[] result = ParentLayer.GetSpritePlanRows();
         Array.Sort(result, new DataRowComparer());
         return result;
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
      public static event ProjectDataset.CoordinateRowChangeEventHandler CoordinateRowDeleting
      {
         add
         {
            m_dsPrj.Coordinate.CoordinateRowDeleting += value;
         }
         remove
         {
            m_dsPrj.Coordinate.CoordinateRowDeleting -= value;
         }
      }
      public static ProjectDataset.CoordinateDataTable Coordinate
      {
         get
         {
            return m_dsPrj.Coordinate;
         }
      }
      public static ProjectDataset.CoordinateRow[] GetSortedCoordinates(ProjectDataset.SpritePlanRow ParentPlan)
      {
         ProjectDataset.CoordinateRow[] result = ParentPlan.GetCoordinateRows();
         Array.Sort(result, new DataRowComparer());
         return result;
      }
      public static ProjectDataset.CoordinateRow AppendPlanCoordinate(ProjectDataset.SpritePlanRow ParentPlan, int x, int y, int weight)
      {
         ProjectDataset.CoordinateRow[] others = ParentPlan.GetCoordinateRows();
         short MaxSeq = 0;
         for (int i=0; i<others.Length; i++)
         {
            if (others[i].Sequence > MaxSeq)
               MaxSeq = others[i].Sequence;
         }
         return m_dsPrj.Coordinate.AddCoordinateRow(ParentPlan.LayerRowParent.MapRow.Name, ParentPlan.LayerRowParent.Name, ParentPlan.Name, (short)(MaxSeq + 1), x, y, weight);
      }
      public static void DeleteCoordinate(ProjectDataset.CoordinateRow row)
      {
         ProjectDataset.SpritePlanRow parent = row.SpritePlanRowParent;
         int delIdx = row.Sequence;
         row.Delete();
         foreach(ProjectDataset.CoordinateRow coord in GetSortedCoordinates(parent))
         {
            if (coord.Sequence > delIdx)
               coord.Sequence--;
         }
      }
      #endregion
      
      #region PlanRule
      public static event ProjectDataset.PlanRuleRowChangeEventHandler PlanRuleRowChanged
      {
         add
         {
            m_dsPrj.PlanRule.PlanRuleRowChanged += value;
         }
         remove
         {
            m_dsPrj.PlanRule.PlanRuleRowChanged -= value;
         }
      }
      public static event ProjectDataset.PlanRuleRowChangeEventHandler PlanRuleRowChanging
      {
         add
         {
            m_dsPrj.PlanRule.PlanRuleRowChanging += value;
         }
         remove
         {
            m_dsPrj.PlanRule.PlanRuleRowChanging -= value;
         }
      }
      public static event ProjectDataset.PlanRuleRowChangeEventHandler PlanRuleRowDeleted
      {
         add
         {
            m_dsPrj.PlanRule.PlanRuleRowDeleted += value;
         }
         remove
         {
            m_dsPrj.PlanRule.PlanRuleRowDeleted -= value;
         }
      }
      public static event ProjectDataset.PlanRuleRowChangeEventHandler PlanRuleRowDeleting
      {
         add
         {
            m_dsPrj.PlanRule.PlanRuleRowDeleting += value;
         }
         remove
         {
            m_dsPrj.PlanRule.PlanRuleRowDeleting -= value;
         }
      }
      public static ProjectDataset.PlanRuleDataTable PlanRule
      {
         get
         {
            return m_dsPrj.PlanRule;
         }
      }
      public static ProjectDataset.PlanRuleRow[] GetSortedPlanRules(ProjectDataset.SpritePlanRow parent, bool includeSuspended)
      {
         if (!includeSuspended)
         {
            string filter = m_dsPrj.PlanRule.MapNameColumn.ColumnName + "='" + parent[m_dsPrj.SpritePlan.MapNameColumn].ToString() +
               "' and " + m_dsPrj.PlanRule.LayerNameColumn.ColumnName + "='" + parent[m_dsPrj.SpritePlan.LayerNameColumn].ToString() +
               "' and " + m_dsPrj.PlanRule.PlanNameColumn.ColumnName + "='" + parent.Name + 
               "' and Suspended=false";
            return (ProjectDataset.PlanRuleRow[])m_dsPrj.PlanRule.Select(filter, "Sequence");
         }
         ProjectDataset.PlanRuleRow[] result = parent.GetPlanRuleRows();
         Array.Sort(result, new DataRowComparer());
         return result;
      }
      public static ProjectDataset.PlanRuleRow InsertPlanRule(ProjectDataset.SpritePlanRow parent, string name,
         string type, int sequence, string function, string parameter1, string parameter2, string parameter3,
         string resultParameter, bool endIf, bool suspended)
      {
         string layerName = parent[m_dsPrj.SpritePlan.LayerNameColumn].ToString();
         string mapName = parent[m_dsPrj.SpritePlan.MapNameColumn].ToString();
         if (sequence < 0)
            sequence = GetMaxPlanSequence(parent) + 1;
         else
            foreach(ProjectDataset.PlanRuleRow row in GetSortedPlanRules(parent, true))
               if (row.Sequence >= sequence)
                  row.Sequence += 1;
         return m_dsPrj.PlanRule.AddPlanRuleRow(mapName, layerName, parent.Name,
            name, sequence, type, function, parameter1, parameter2, parameter3, resultParameter, endIf, suspended);
      }
      
      public static void DeletePlanRule(ProjectDataset.PlanRuleRow row)
      {
         int oldSeq = row.Sequence;
         ProjectDataset.SpritePlanRow parent = row.SpritePlanRowParent;
         row.Delete();
         foreach(ProjectDataset.PlanRuleRow drChange in GetSortedPlanRules(parent, true))
            if (drChange.Sequence >= oldSeq)
               drChange.Sequence -= 1;
      }
      
      public static bool MovePlanRule(ProjectDataset.PlanRuleRow row, bool moveDown)
      {
         string planName = row[m_dsPrj.PlanRule.PlanNameColumn].ToString();
         string layerName = row[m_dsPrj.PlanRule.LayerNameColumn].ToString();
         string mapName = row[m_dsPrj.PlanRule.MapNameColumn].ToString();
         string filter = "MapName='" + mapName + "' and LayerName='" + layerName + "' and PlanName='" + planName + "' ";

         DataRow[] nextRows;
         if (moveDown)
            nextRows = m_dsPrj.PlanRule.Select(filter + "and Sequence >= " + row.Sequence.ToString(), "Sequence ASC");
         else
            nextRows = m_dsPrj.PlanRule.Select(filter + "and Sequence <= " + row.Sequence.ToString(), "Sequence DESC");
         System.Diagnostics.Debug.Assert((nextRows.Length > 0) && (nextRows[0] == row), "Unexpected plan rule sequencing error");
         if (nextRows.Length == 1)
            return false;
         ProjectDataset.PlanRuleRow nextRow = (ProjectDataset.PlanRuleRow)nextRows[1];
         int nextSeq = nextRow.Sequence;
         if (moveDown)
            System.Diagnostics.Debug.Assert(nextRow.Sequence == row.Sequence + 1, "Plan rows are not consecutively sequenced");
         else
            System.Diagnostics.Debug.Assert(nextRow.Sequence == row.Sequence - 1, "Plan rows are not consecutively sequenced");
         nextRow.Sequence = row.Sequence;
         row.Sequence = nextSeq;
         return true;
      }
      public static ProjectDataset.PlanRuleRow GetPlanRule(ProjectDataset.SpritePlanRow parent, string name)
      {
         string layerName = parent[m_dsPrj.SpritePlan.LayerNameColumn].ToString();
         string mapName = parent[m_dsPrj.SpritePlan.MapNameColumn].ToString();
         return m_dsPrj.PlanRule.FindByMapNameLayerNamePlanNameName(mapName, layerName, parent.Name, name);
      }
      public static int GetMaxPlanSequence(ProjectDataset.SpritePlanRow parent)
      {
         int max = 0;
         foreach (ProjectDataset.PlanRuleRow row in parent.GetPlanRuleRows())
         {
            if (row.Sequence > max)
               max = row.Sequence;
         }
         return max;
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
      public static event ProjectDataset.SolidityRowChangeEventHandler SolidityRowDeleting
      {
         add
         {
            m_dsPrj.Solidity.SolidityRowDeleting += value;
         }
         remove
         {
            m_dsPrj.Solidity.SolidityRowDeleting -= value;
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
      public static event ProjectDataset.SolidityShapeRowChangeEventHandler SolidityShapeRowDeleting
      {
         add
         {
            m_dsPrj.SolidityShape.SolidityShapeRowDeleting += value;
         }
         remove
         {
            m_dsPrj.SolidityShape.SolidityShapeRowDeleting -= value;
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
      public static event ProjectDataset.CategoryFrameRowChangeEventHandler CategoryFrameRowDeleting
      {
         add
         {
            m_dsPrj.CategoryFrame.CategoryFrameRowDeleting += value;
         }
         remove
         {
            m_dsPrj.CategoryFrame.CategoryFrameRowDeleting -= value;
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
      public static bool DeleteCategoryFrameRow(string Tileset, string Category, int TileValue, short FrameValue)
      {
         ProjectDataset.CategoryFrameRow deleteRow = m_dsPrj.CategoryFrame.FindByTilesetCategoryTileValueFrame(Tileset, Category, TileValue, FrameValue);
         if (deleteRow.CategoryTileRowParent.GetCategoryFrameRows().Length <= 1)
         {
            deleteRow.CategoryTileRowParent.Delete();
            return true;
         }
         else
         {
            deleteRow.Delete();
            return false;
         }
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
      public static event ProjectDataset.SourceCodeRowChangeEventHandler SourceCodeRowDeleting
      {
         add
         {
            m_dsPrj.SourceCode.SourceCodeRowDeleting += value;
         }
         remove
         {
            m_dsPrj.SourceCode.SourceCodeRowDeleting -= value;
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
      public static ProjectDataset.SourceCodeRow AddSourceCode(string Name, string Text, string DependsOn, bool IsCustomObject, byte[] CustomObjectData)
      {
         return m_dsPrj.SourceCode.AddSourceCodeRow(Name, IsCustomObject, DependsOn, Text, CustomObjectData);
      }
      public static void DeleteSourceCode(ProjectDataset.SourceCodeRow row)
      {
         foreach(ProjectDataset.SourceCodeRow dep in GetDependentSourceCode(row))
            DeleteSourceCode(dep);
         row.Delete();
      }
      public static bool IsSourceCodeDecapsulated(ProjectDataset.SourceCodeRow row)
      {
         if (row.IsTextNull())
            return false;
         return (row.Text.StartsWith("~import"));
      }
      public static string GetSourceCodeCapsulePath(string relativeTo, ProjectDataset.SourceCodeRow row)
      {
         string fileName = row.Text.Substring(7);
         fileName = fileName.Trim();
         fileName = fileName.Trim(new char[] {'\"'});
         return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(relativeTo), fileName);
      }
      public static void ReencapsulateSourceCode(string relativeTo, ProjectDataset.SourceCodeRow row)
      {
         if (IsSourceCodeDecapsulated(row))
            row.Text = GetSourceCodeText(relativeTo, row);
      }
      /// <summary>
      /// Returns the text for the specified source code object, following the decapsulated link
      /// if necessary.
      /// </summary>
      /// <param name="relativeTo">Path of the SGDK file to which the capsule would be relative.</param>
      /// <param name="row">SourceCode row containing the code or capsule to retrieve.</param>
      /// <returns>The contents of the capsule file if code is decapsulated, or the
      /// contents of the Text property of the SourceCodeRow otherwise.</returns>
      public static string GetSourceCodeText(string relativeTo, ProjectDataset.SourceCodeRow row)
      {
         if (IsSourceCodeDecapsulated(row))
         {
            string capsulePath = GetSourceCodeCapsulePath(relativeTo, row);
            try
            {
               using(System.IO.StreamReader capsuleReader = new StreamReader(capsulePath))
                  return capsuleReader.ReadToEnd();
            }
            catch(System.Exception ex)
            {
               throw new ApplicationException("Failed to read decapsulated source code object \"" + row.Name + "\" from \"" + capsulePath + "\": " + ex.ToString(), ex);
            }
         }
         else
            return row.Text;
      }

      public static string GetSourceCodeText(ProjectDataset.SourceCodeRow row)
      {
         if (IsSourceCodeDecapsulated(row) && (SGDK2IDE.CurrentProjectFile == null))
            throw new ApplicationException("Source code object \"" + row.Name + "\" attempted to decapsulate code relative to an un-saved project.  This is not supported.  Save the project and try again.");
         return GetSourceCodeText(SGDK2IDE.CurrentProjectFile, row);
      }

      public static ProjectDataset.SourceCodeRow[] GetDependentSourceCode(ProjectDataset.SourceCodeRow parent)
      {
         DataView dvCurrent = new DataView(SourceCode, SourceCode.DependsOnColumn + "='" + parent[SourceCode.NameColumn, DataRowVersion.Current].ToString() + "'", String.Empty, DataViewRowState.CurrentRows);
         ProjectDataset.SourceCodeRow[] result = new SGDK2.ProjectDataset.SourceCodeRow[dvCurrent.Count];
         for(int i = 0; i < result.Length; i++)
            result[i] = (ProjectDataset.SourceCodeRow)dvCurrent[i].Row;
         return result;
      }
      public static string GetCustomObjectDataSize(ProjectDataset.SourceCodeRow row)
      {
         if (row.CustomObjectData.Length < 1024)
            return row.CustomObjectData.Length.ToString() + " bytes";
         else if (row.CustomObjectData.Length < 1024*1024)
            return ((int)(row.CustomObjectData.Length / 1024)).ToString() + " KB";
         else if (row.CustomObjectData.Length < 1024 * 1024 * 1024)
            return ((int)(row.CustomObjectData.Length / (1024 * 1024))).ToString() + " MB";
         else
            return ((int)(row.CustomObjectData.Length / (1024 * 1024 * 1024))).ToString() + " GB";
      }
      #endregion

      #region Project
      public static ProjectDataset.ProjectRow ProjectRow
      {
         get
         {
            if (m_dsPrj.Project.Count <= 0)
               m_dsPrj.Project.AddProjectRow(GameDisplayMode.m640x480x24.ToString(), true, "Powered by Scrolling Game Development Kit 2 (http://sgdk2.sf.net)", null, null, 1, 1, "SGDK2 Engine: http://sgdk2.sf.net");
            return m_dsPrj.Project[0];
         }
      }

      public static string[] GetCreditLines()
      {
         if (ProjectRow.IsCreditsNull() || (ProjectRow.Credits.Length <= 0))
            return new string[] {};
         return ProjectRow.Credits.Replace("\r\n","\n").Split('\n');
      }

      public static string GetCreditAdditions(ProjectDataset importData)
      {
         StringBuilder sb = new StringBuilder();

         if ((importData.Project.Count > 0) && !importData.Project.Rows[0].IsNull(importData.Project.CreditsColumn))
         {
            System.Collections.Specialized.StringCollection existingCredits =
               new System.Collections.Specialized.StringCollection();
            existingCredits.AddRange(GetCreditLines());

            foreach(string credit in importData.Project[0].Credits.Replace("\r\n","\n").Split('\n'))
            {
               if (!existingCredits.Contains(credit))
                  sb.Append("Add \"" + credit + "\" to the credits.\r\n");
            }
         }

         return sb.ToString();
      }

      public static void MergeCredits(ProjectDataset importData)
      {
         StringBuilder sb = new StringBuilder();

         if ((importData.Project.Count > 0) && !importData.Project.Rows[0].IsNull(importData.Project.CreditsColumn))
         {
            System.Collections.Specialized.StringCollection existingCredits =
               new System.Collections.Specialized.StringCollection();
            existingCredits.AddRange(GetCreditLines());

            foreach(string credit in importData.Project[0].Credits.Replace("\r\n","\n").Split('\n'))
            {
               if (!existingCredits.Contains(credit))
               {
                  if (sb.Length > 0)
                     sb.Append("\r\n");
                  sb.Append(credit);
               }
            }
         }

         if (ProjectRow.IsCreditsNull())
            ProjectRow.Credits = sb.ToString();
         else if (ProjectRow.Credits.EndsWith("\r\n"))
            ProjectRow.Credits += sb.ToString();
         else
            ProjectRow.Credits += "\r\n" + sb.ToString();
      }

      #endregion     

      #region PlanParameterValue
      public static event ProjectDataset.PlanParameterValueRowChangeEventHandler PlanParameterValueRowChanged
      {
         add
         {
            m_dsPrj.PlanParameterValue.PlanParameterValueRowChanged += value;
         }
         remove
         {
            m_dsPrj.PlanParameterValue.PlanParameterValueRowChanged -= value;
         }
      }
      public static event ProjectDataset.PlanParameterValueRowChangeEventHandler PlanParameterValueRowChanging
      {
         add
         {
            m_dsPrj.PlanParameterValue.PlanParameterValueRowChanging += value;
         }
         remove
         {
            m_dsPrj.PlanParameterValue.PlanParameterValueRowChanging -= value;
         }
      }
      public static event ProjectDataset.PlanParameterValueRowChangeEventHandler PlanParameterValueRowDeleted
      {
         add
         {
            m_dsPrj.PlanParameterValue.PlanParameterValueRowDeleted += value;
         }
         remove
         {
            m_dsPrj.PlanParameterValue.PlanParameterValueRowDeleted -= value;
         }
      }
      public static event ProjectDataset.PlanParameterValueRowChangeEventHandler PlanParameterValueRowDeleting
      {
         add
         {
            m_dsPrj.PlanParameterValue.PlanParameterValueRowDeleting += value;
         }
         remove
         {
            m_dsPrj.PlanParameterValue.PlanParameterValueRowDeleting -= value;
         }
      }
      public static ProjectDataset.PlanParameterValueDataTable PlanParameterValue
      {
         get
         {
            return m_dsPrj.PlanParameterValue;
         }
      }
      public static ProjectDataset.PlanParameterValueRow AddPlanParameterValue(ProjectDataset.SpritePlanRow parent, string parameterName, string parameterValue)
      {
         return m_dsPrj.PlanParameterValue.AddPlanParameterValueRow(
            parent[m_dsPrj.SpritePlan.MapNameColumn].ToString(),
            parent[m_dsPrj.SpritePlan.LayerNameColumn].ToString(),
            parent.Name, parameterName, parameterValue);
      }
      #endregion
   }
}
