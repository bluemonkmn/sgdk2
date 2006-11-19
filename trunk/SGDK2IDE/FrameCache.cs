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
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;

namespace SGDK2
{
	/// <summary>
	/// Read-only optimized access to frameset frames and associated graphics
	/// </summary>
	public class FrameCache
	{
      public struct Frame
      {
         public Display.TextureRef GraphicSheetTexture;
         public short CellIndex;
         public Matrix Transform;
         public Rectangle SourceRect;
         public ProjectDataset.GraphicSheetRow GraphicSheet;
         private System.Drawing.Rectangle bounds;
         public int Color;

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
                  Size CellSize = new Size(SourceRect.Width, SourceRect.Height);
                  using (System.Drawing.Drawing2D.Matrix m =
                     new System.Drawing.Drawing2D.Matrix(Transform.M11, Transform.M12, Transform.M21, Transform.M22, Transform.M41, Transform.M42))
                  {
                     System.Drawing.Point[] ptsRect = new Point[]
                     {
                        new Point(0, 0),
                        new Point(CellSize.Width, 0),
                        new Point(CellSize.Width, CellSize.Height),
                        new Point(0, CellSize.Height)
                     };
                     m.TransformPoints(ptsRect);
                     bounds = new Rectangle(ptsRect[0], new Size(1,1));
                     foreach (Point pt in ptsRect)
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
               }
               return bounds;
            }
         }
      }

      #region Fields
      private Frame[] m_arFrames = null;
      private Display m_Display = null;
      #endregion

      private static System.Collections.Hashtable activeCaches = new System.Collections.Hashtable();

      public static FrameCache GetFrameCache(string name, Display display)
      {
         if (activeCaches.Contains(name))
         {
            WeakReference cachedObject = (WeakReference)activeCaches[name];
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
         System.Collections.ArrayList lstRemove = new System.Collections.ArrayList();
         foreach(System.Collections.DictionaryEntry de in activeCaches)
         {
            WeakReference cachedObject = (WeakReference)de.Value;
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

      public FrameCache(ProjectDataset.FramesetRow Frameset, Display display)
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

            if (m_Display != null)
               m_arFrames[nIdx].GraphicSheetTexture = m_Display.GetTextureRef(g.Name);
            m_arFrames[nIdx].GraphicSheet = g;
            m_arFrames[nIdx].CellIndex = arfr[nIdx].CellIndex;
            m_arFrames[nIdx].Transform.M11 = arfr[nIdx].m11;
            m_arFrames[nIdx].Transform.M12 = arfr[nIdx].m12;
            m_arFrames[nIdx].Transform.M13 = 0;
            m_arFrames[nIdx].Transform.M14 = 0;
            m_arFrames[nIdx].Transform.M21 = arfr[nIdx].m21;
            m_arFrames[nIdx].Transform.M22 = arfr[nIdx].m22;
            m_arFrames[nIdx].Transform.M23 = 0;
            m_arFrames[nIdx].Transform.M24 = 0;
            m_arFrames[nIdx].Transform.M41 = arfr[nIdx].dx;
            m_arFrames[nIdx].Transform.M42 = arfr[nIdx].dy;
            m_arFrames[nIdx].Transform.M43 = 0;
            m_arFrames[nIdx].Transform.M44 = 1;
            m_arFrames[nIdx].Color = arfr[nIdx].color;

            m_arFrames[nIdx].SourceRect = new Rectangle(
               (arfr[nIdx].CellIndex % g.Columns) * g.CellWidth,
               (arfr[nIdx].CellIndex / g.Columns) * g.CellHeight,
               g.CellWidth, g.CellHeight);

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
