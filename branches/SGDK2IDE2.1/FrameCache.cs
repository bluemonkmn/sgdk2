/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */

using System;
using System.Drawing;

namespace SGDK2
{
	/// <summary>
	/// Read-only optimized access to frameset frames and associated graphics
	/// </summary>
	public class FrameCache
	{
      public class Frame
      {
         public Display.TextureRef GraphicSheetTexture = null;
         public short CellIndex = 0;
         public Point[] corners = new Point[4];
         public Rectangle SourceRect = Rectangle.Empty;
         public ProjectDataset.GraphicSheetRow GraphicSheet = null;
         private System.Drawing.Rectangle bounds = Rectangle.Empty;
         public int Color = 0;

         public Frame(short cellIndex, Rectangle source, ProjectDataset.GraphicSheetRow graphicSheet, int color, Point[] corners)
         {
            this.CellIndex = cellIndex;
            this.SourceRect = source;
            this.GraphicSheet = graphicSheet;
            this.Color = color;
            System.Diagnostics.Debug.Assert(corners.Length == 4);
            this.corners = corners;
         }

         /// <summary>
         /// Determine the bounding rectangle of the graphics in this frame
         /// </summary>
         /// <returns>Rectangle relative to the origin of this frame
         /// that contains the frame's graphics</returns>
         public System.Drawing.Rectangle Bounds
         {
            get
            {
               if (bounds.IsEmpty)
               {
                  bounds = new Rectangle(corners[0], new Size(0,0));
                  foreach (Point pt in corners)
                  {
                     if(pt.X < bounds.X)
                     {
                        bounds.Width += bounds.X - pt.X;
                        bounds.X = pt.X;
                     }
                     if(pt.Y < bounds.Y)
                     {
                        bounds.Height += bounds.Y - pt.Y;
                        bounds.Y = pt.Y;
                     }
                     if (pt.X > bounds.Right)
                        bounds.Width += pt.X - bounds.Right;
                     if (pt.Y > bounds.Bottom)
                        bounds.Height += pt.Y - bounds.Bottom;
                  }
               }
               return bounds;
            }
         }
      }

      #region Fields
      private Frame[] m_arFrames = null;
      private Display m_Display = null;
      #endregion

      private static System.Collections.Generic.Dictionary<string, WeakReference> activeCaches = new System.Collections.Generic.Dictionary<string, WeakReference>();

      public static FrameCache GetFrameCache(string name, Display display)
      {
         if (activeCaches.ContainsKey(name))
         {
            WeakReference cachedObject = activeCaches[name];
            FrameCache result;
            if (!cachedObject.IsAlive || (null == (result = (FrameCache)cachedObject.Target)))
               return new FrameCache(ProjectData.GetFrameSet(name), display);
            if (result.m_Display != display)
               return new FrameCache(ProjectData.GetFrameSet(name), display);
            return result;
         }
         else
            return new FrameCache(ProjectData.GetFrameSet(name), display);
      }

      public static void ClearDisplayCache(Display display)
      {
         System.Collections.Generic.List<string> lstRemove = new System.Collections.Generic.List<string>();
         foreach(var de in activeCaches)
         {
            WeakReference cachedObject = de.Value;
            if (!cachedObject.IsAlive)
               lstRemove.Add(de.Key);
            else if (((FrameCache)cachedObject.Target).m_Display == display)
               lstRemove.Add(de.Key);
         }
         foreach(string name in lstRemove)
            activeCaches.Remove(name);
      }

      #region Initialization and Clean-up
      private FrameCache()
      {
      }

      private FrameCache(ProjectDataset.FramesetRow Frameset, Display display)
      {
         ProjectDataset.FrameRow[] arfr = ProjectData.GetSortedFrameRows(Frameset);
         m_Display = display;
         m_arFrames = new Frame[arfr.Length];
         for (int nIdx = 0; nIdx < arfr.Length; nIdx++)
         {
            if (arfr[nIdx].FrameValue != nIdx)
            {
               throw new ApplicationException(String.Format("The \"" + Frameset.Name + "\" Frameset has frames out of sequence.  FrameValue for frame {0} is {1}.",
                  nIdx, arfr[nIdx].FrameValue));
            }
            ProjectDataset.GraphicSheetRow g = ProjectData.GetGraphicSheet(arfr[nIdx].GraphicSheet);

            Rectangle source = new Rectangle(
               (arfr[nIdx].CellIndex % g.Columns) * g.CellWidth,
               (arfr[nIdx].CellIndex / g.Columns) * g.CellHeight,
               g.CellWidth, g.CellHeight);
            System.Drawing.Point[] ptsRect = new Point[]
            {
               new Point(0, 0),
               new Point(0, source.Height),
               new Point(source.Width, source.Height),
               new Point(source.Width, 0)
            };
            using (System.Drawing.Drawing2D.Matrix m =
               new System.Drawing.Drawing2D.Matrix(arfr[nIdx].m11, arfr[nIdx].m12, arfr[nIdx].m21, arfr[nIdx].m22, arfr[nIdx].dx, arfr[nIdx].dy))
            {
               m.TransformPoints(ptsRect);
            }
            m_arFrames[nIdx] = new Frame(arfr[nIdx].CellIndex, source, g, arfr[nIdx].color, ptsRect);
            if (m_Display != null)
               m_arFrames[nIdx].GraphicSheetTexture = m_Display.GetTextureRef(g.Name);

            activeCaches[Frameset.Name] = new WeakReference(this);
         }
      }
      #endregion

      #region Public Members
      public Frame this[int index]
      {
         get
         {
            return m_arFrames[index];
         }
      }

      public int Count
      {
         get
         {
            return m_arFrames.Length;
         }
      }
      #endregion
   }
}
