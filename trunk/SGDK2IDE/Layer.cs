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
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

namespace SGDK2
{
	/// <summary>
	/// Stores data for tiles and sprites positioned on the map.
	/// Design-time wrapper for dataset layer row.
	/// </summary>
   public class Layer
   {
      #region Embedded Types
      private class InjectedFrame : IComparable
      {
         public int x;
         public int y;
         public FrameCache.Frame frame;
         public InjectedFrame(int x, int y, FrameCache.Frame frame)
         {
            this.x = x;
            this.y = y;
            this.frame = frame;
         }
         #region IComparable Members

         public int CompareTo(object obj)
         {
            int result = y.CompareTo((obj as InjectedFrame).y);
            if (result != 0)
               return result;
            result = x.CompareTo((obj as InjectedFrame).x);
            if (result != 0)
               return result;
            return -1;
         }

         #endregion
      }
      #endregion

      #region Fields
      ProjectDataset.LayerRow m_Layer = null;
      Point m_CurrentPosition;
      int m_nLeftBuffer;
      int m_nTopBuffer;
      int m_nRightBuffer;
      int m_nBottomBuffer;
      TileCache m_TileCache;
      FrameCache m_FrameCache;
      System.Collections.ArrayList m_InjectedFrames = null;
      #endregion

      private Layer()
      {
      }

      public Layer(ProjectDataset.LayerRow layer, Display display)
      {
         m_Layer = layer;
         ProjectData.GetTilesetOverlaps(m_Layer.TilesetRow,
            out m_nRightBuffer, out m_nBottomBuffer, out m_nLeftBuffer, out m_nTopBuffer);
         m_TileCache = new TileCache(layer.TilesetRow);
         m_FrameCache = new FrameCache(layer.TilesetRow.FramesetRow, display);
      }

      #region Public properties
      /// <summary>
      /// Get the number of columns of tiles in the layer
      /// </summary>
      public int Columns
      {
         get
         {
            return m_Layer.Width;
         }
      }
      /// <summary>
      /// Get the number of rows of tiles in the layer
      /// </summary>
      public int Rows
      {
         get
         {
            return m_Layer.Height;
         }
      }

      /// <summary>
      /// Get or set the position of the layer within the map. 
      /// (Does not affect current position until <see cref="Move"/> is called)
      /// </summary>
      public Point AbsolutePosition
      {
         get
         {
            return new Point(m_Layer.OffsetX, m_Layer.OffsetY);
         }
         set
         {
            m_Layer.OffsetX = value.X;
            m_Layer.OffsetY = value.Y;
         }
      }

      /// <summary>
      /// Get/Set tile values in the layer
      /// </summary>
      public int this[int x, int y]
      {
         get
         {
            switch(m_Layer.BytesPerTile)
            {
               case 1:
                  return m_Layer.Tiles[y*m_Layer.Width + x];
               case 2:
                  return BitConverter.ToInt16(m_Layer.Tiles,(y*m_Layer.Width + x) * 2);
               case 4:
                  return BitConverter.ToInt32(m_Layer.Tiles,(y*m_Layer.Width + x) * 4);
            }
            throw new ApplicationException("Unexpected BytesPerTile value");
         }
         set
         {
            switch(m_Layer.BytesPerTile)
            {
               case 1:
                  m_Layer.Tiles[y*m_Layer.Width + x] = System.Convert.ToByte(value);
                  break;
               case 2:
                  BitConverter.GetBytes(System.Convert.ToInt16(value)).CopyTo(m_Layer.Tiles,(y*m_Layer.Width + x) * 2);
                  break;
               case 4:
                  BitConverter.GetBytes(value).CopyTo(m_Layer.Tiles,(y*m_Layer.Width + x) * 4);
                  break;
            }
         }
      }

      /// <summary>
      /// Gets/Sets the scroll rate that is applied to <see cref="Move"/> operations.
      /// </summary>
      public SizeF ScrollRate
      {
         get
         {
            return new SizeF(m_Layer.ScrollRateX, m_Layer.ScrollRateY);
         }
         set
         {
            m_Layer.ScrollRateX = value.Width;
            m_Layer.ScrollRateY = value.Height;
         }
      }

      /// <summary>
      /// Gets/Sets the current pixel position of the layer relative to the screen.
      /// (Setting this directly ignores <see cref="ScrollRate"/> and <see cref="AbsolutePosition"/>.)
      /// </summary>
      public Point CurrentPosition
      {
         get
         {
            return m_CurrentPosition;
         }
         set
         {
            m_CurrentPosition = value;
         }
      }

      public ProjectDataset.LayerRow LayerRow
      {
         get
         {
            return m_Layer;
         }
      }
      #endregion

      #region Public methods
      /// <summary>
      /// Move/Scroll the layer to a new position based on the current map position.
      /// The layer's current position is offset by its position on the map and scaled
      /// by the layer's scroll rate.
      /// </summary>
      /// <param name="x">Horizontal position of the map</param>
      /// <param name="y">Vertical position of the map</param>
      /// <remarks>Map positions are usually negative because the map position indicates
      /// the position of the top-left corner of the map which is usually scrolled off
      /// the top-left corner of the screen to a negative position.</remarks>
      public void Move(int x, int y)
      {
         m_CurrentPosition = new Point(m_Layer.OffsetX + (int)(x * m_Layer.ScrollRateX), m_Layer.OffsetY + (int)(y * m_Layer.ScrollRateY));
      }

      public void Draw(Device Device, Size ViewSize)
      {
         ProjectDataset.TilesetRow tsr = m_Layer.TilesetRow;
         int nTileWidth = tsr.TileWidth;
         int nTileHeight = tsr.TileHeight;

         int nStartCol = (-m_nLeftBuffer-m_CurrentPosition.X) / nTileWidth;
         if (nStartCol < 0)
            nStartCol = 0;
         int nStartRow = (-m_nTopBuffer-m_CurrentPosition.Y) / nTileHeight;
         if (nStartRow < 0)
            nStartRow = 0;

         int EndCol = (ViewSize.Width - 1 + m_nRightBuffer - m_CurrentPosition.X) / nTileWidth;
         if (EndCol >= Columns)
            EndCol = Columns - 1;
         int EndRow = (ViewSize.Height - 1 + m_nBottomBuffer - m_CurrentPosition.Y) / nTileHeight;
         if (EndRow >= Rows)
            EndRow = Rows - 1;

         Sprite spr = new Sprite(Device);
         try
         {
            spr.Begin(SpriteFlags.AlphaBlend);

            Texture t = null;

            System.Collections.IEnumerator Injected = null;
            InjectedFrame CurFrame;
            if (m_InjectedFrames != null)
            {
               Injected = m_InjectedFrames.GetEnumerator();
               if (!Injected.MoveNext())
                  Injected = null;
            }

            for (int y = nStartRow; y <= EndRow; y++)
            {
               if (Injected != null)
               {
                  while ((CurFrame = (InjectedFrame)Injected.Current).y < y * nTileHeight)
                  {
                     if ((t == null) || (t != CurFrame.frame.GraphicSheetTexture))
                        Device.SetTexture(0, t = CurFrame.frame.GraphicSheetTexture);
                     spr.Transform = Matrix.Multiply(CurFrame.frame.Transform, Matrix.Translation(
                        CurFrame.x + m_CurrentPosition.X, CurFrame.y + m_CurrentPosition.Y, 0));
                     spr.Draw(CurFrame.frame.GraphicSheetTexture, CurFrame.frame.SourceRect,
                        Vector3.Empty, Vector3.Empty, -1);
                     if (!Injected.MoveNext())
                     {
                        Injected = null;
                        break;
                     }
                  }
               }

               for (int x = nStartCol; x <= EndCol; x++)
               {
                  int[] SubFrames = m_TileCache[this[x,y]];
                  for (int nFrame = 0; nFrame < SubFrames.Length; nFrame++)
                  {
                     FrameCache.Frame f = m_FrameCache[SubFrames[nFrame]];
                     if ((t == null) || (t != f.GraphicSheetTexture))
                        Device.SetTexture(0, t = f.GraphicSheetTexture);

                     spr.Transform = Matrix.Multiply(f.Transform, Matrix.Translation(
                        x * nTileWidth + m_CurrentPosition.X,
                        y * nTileHeight + m_CurrentPosition.Y, 0));
                     spr.Draw(f.GraphicSheetTexture, f.SourceRect, Vector3.Empty, Vector3.Empty, -1);
                  }
               }
            }

            while (Injected != null)
            {
               CurFrame = (InjectedFrame)Injected.Current;
               if (t != CurFrame.frame.GraphicSheetTexture)
                  Device.SetTexture(0, t = CurFrame.frame.GraphicSheetTexture);
               spr.Transform = Matrix.Multiply(CurFrame.frame.Transform, Matrix.Translation(
                  CurFrame.x + m_CurrentPosition.X, CurFrame.y + m_CurrentPosition.Y, 0));
               spr.Draw(CurFrame.frame.GraphicSheetTexture, CurFrame.frame.SourceRect,
                  Vector3.Empty, Vector3.Empty, -1);
               if (!Injected.MoveNext())
               {
                  Injected = null;
                  break;
               }
            }
            spr.End();
         }
         finally
         {
            spr.Dispose();
         }
      }

      public void InjectFrame(int x, int y, FrameCache.Frame frame)
      {
         int idx;
         InjectedFrame f = new InjectedFrame(x, y, frame);
         if (m_InjectedFrames == null)
         {
            m_InjectedFrames = new System.Collections.ArrayList();
            idx = 0;
         }
         else
         {
            idx = m_InjectedFrames.BinarySearch(f);
            if (idx < 0)
               idx = ~idx;
         }
         m_InjectedFrames.Insert(idx, f);
      }

      public void InjectTile(int nCol, int nRow, int nTileValue)
      {
         ProjectDataset.TilesetRow tsr = m_Layer.TilesetRow;
         short nWidth = tsr.TileWidth;
         short nHeight = tsr.TileHeight;
         int[] SubFrames = m_TileCache[nTileValue];
         for (int nFrame = 0; nFrame < SubFrames.Length; nFrame++)
            InjectFrame(nCol * nWidth, nRow * nHeight, m_FrameCache[SubFrames[nFrame]]);
      }

      public void ClearInjections()
      {
         m_InjectedFrames = null;
      }
      #endregion
   }
}
