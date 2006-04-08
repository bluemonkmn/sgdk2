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
         public Texture GraphicSheetTexture;
         public short CellIndex;
         public Matrix Transform;
         public Rectangle SourceRect;
         public ProjectDataset.GraphicSheetRow GraphicSheet;
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
            return result;
         }
         else
            return new FrameCache(ProjectData.GetFrameSet(name), display);
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

            m_arFrames[nIdx].GraphicSheetTexture = m_Display.GetTexture(g.Name, false);
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
