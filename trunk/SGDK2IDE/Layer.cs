/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */

using System;
using System.Drawing;

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
         public int priority;
         public FrameCache.Frame frame;
         public int color;

         public InjectedFrame(int x, int y, int priority, FrameCache.Frame frame, int color)
         {
            this.x = x;
            this.y = y;
            this.frame = frame;
            this.priority = priority;
            if (color == -1)
               this.color = frame.Color;
            else if (frame.Color == -1)
               this.color = color;
            else
               this.color = color; //Microsoft.DirectX.Direct3D.ColorOperator.Modulate(frame.Color, color);
         }
         #region IComparable Members

         public int CompareTo(object obj)
         {
            int result = priority.CompareTo((obj as InjectedFrame).priority);
            if (result != 0)
               return result;
            result = y.CompareTo((obj as InjectedFrame).y);
            if (result != 0)
               return result;
            result = x.CompareTo((obj as InjectedFrame).x);
            if (result != 0)
               return result;
            return -1;
         }

         #endregion
      }

      private class PointComparer : System.Collections.IComparer
      {
         public readonly static PointComparer Value = new PointComparer();

         #region IComparer Members
         public int Compare(object x, object y)
         {
            if (((Point)y).Y > ((Point)x).Y)
               return 1;
            else if (((Point)y).Y < ((Point)x).Y)
               return -1;
            else if (((Point)y).X > ((Point)x).X)
               return 1;
            else if (((Point)y).X < ((Point)x).X)
               return -1;
            else
               return 0;
         }
         #endregion
      }

      public enum LightingMode
      {
         Disabled,
         Normal
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
      System.Collections.ArrayList m_CachedSprites = null;
      System.Collections.ArrayList m_SuspendedTiles = null;
      #endregion

      private Layer()
      {
      }

      public Layer(ProjectDataset.LayerRow layer, Display display)
      {
         m_Layer = layer;
         if (!String.IsNullOrEmpty(layer.Tileset))
         {
            ProjectData.GetTilesetOverlaps(m_Layer.TilesetRow,
            out m_nRightBuffer, out m_nBottomBuffer, out m_nLeftBuffer, out m_nTopBuffer);
            m_TileCache = new TileCache(layer.TilesetRow);
            m_FrameCache = FrameCache.GetFrameCache(layer.TilesetRow.Frameset, display);
         }
         else
         {
            m_TileCache = null;
            m_FrameCache = FrameCache.GetFrameCache(null, display);
         }
      }

      #region Public properties
      /// <summary>
      /// Get the number of columns of tiles in the layer
      /// </summary>
      public int ActualColumns
      {
         get
         {
            return m_Layer.Width;
         }
      }
      /// <summary>
      /// Get the number of rows of tiles in the layer
      /// </summary>
      public int ActualRows
      {
         get
         {
            return m_Layer.Height;
         }
      }

      /// <summary>
      /// Get the number of columns of tiles in the layer
      /// </summary>
      public int VirtualColumns
      {
         get
         {
            return (m_Layer.VirtualWidth == 0) ? m_Layer.Width : m_Layer.VirtualWidth;
         }
      }
      /// <summary>
      /// Get the number of rows of tiles in the layer
      /// </summary>
      public int VirtualRows
      {
         get
         {
            return (m_Layer.VirtualHeight == 0) ? m_Layer.Height : m_Layer.VirtualHeight;
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

      public LightingMode Lighting
      {
         get
         {
            if (m_Layer.IsLightingNull())
               return LightingMode.Disabled;
            if (m_Layer.Lighting == "Normal")
               return LightingMode.Normal;
            return LightingMode.Disabled;
         }
         set
         {
            if (value == LightingMode.Normal)
               m_Layer.Lighting = "Normal";
            else // Lighting "Disabled"
               m_Layer.SetLightingNull();
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
                  return m_Layer.Tiles[(y % ActualRows)*m_Layer.Width + (x % ActualColumns)];
               case 2:
                  return BitConverter.ToInt16(m_Layer.Tiles,((y%ActualRows)*m_Layer.Width + (x%ActualColumns)) * 2);
               case 4:
                  return BitConverter.ToInt32(m_Layer.Tiles,((y%ActualRows)*m_Layer.Width + (x%ActualColumns)) * 4);
            }
            throw new ApplicationException("Unexpected BytesPerTile value");
         }
         set
         {
            switch(m_Layer.BytesPerTile)
            {
               case 1:
                  m_Layer.Tiles[(y%ActualRows)*m_Layer.Width + (x%ActualColumns)] = System.Convert.ToByte(value);
                  break;
               case 2:
                  BitConverter.GetBytes(System.Convert.ToInt16(value)).CopyTo(m_Layer.Tiles,((y%ActualRows)*m_Layer.Width + (x%ActualColumns)) * 2);
                  break;
               case 4:
                  BitConverter.GetBytes(value).CopyTo(m_Layer.Tiles,((y%ActualRows)*m_Layer.Width + (x%ActualColumns)) * 4);
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

      public int Priority
      {
         get
         {
            return m_Layer.Priority;
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

      public void Draw(Display Display, Size ViewSize)
      {
         if ((m_TileCache != null) &&  (m_TileCache.Count <= 0))
            return;
         int[] bgTileFrame = {0};

         ProjectDataset.TilesetRow tsr;
         int nTileWidth;
         int nTileHeight;
         if (String.IsNullOrEmpty(m_Layer.Tileset))
         {
            tsr = null;
            nTileWidth = 32;
            nTileHeight = 32;
         }
         else
         {
            tsr = m_Layer.TilesetRow;
            nTileWidth = tsr.TileWidth;
            nTileHeight = tsr.TileHeight;
         }

         int nStartCol = (-m_nLeftBuffer-m_CurrentPosition.X) / nTileWidth;
         if (nStartCol < 0)
            nStartCol = 0;
         int nStartRow = (-m_nTopBuffer-m_CurrentPosition.Y) / nTileHeight;
         if (nStartRow < 0)
            nStartRow = 0;

         int EndCol = (ViewSize.Width - 1 + m_nRightBuffer - m_CurrentPosition.X) / nTileWidth;
         if (EndCol >= VirtualColumns)
            EndCol = VirtualColumns - 1;
         int EndRow = (ViewSize.Height - 1 + m_nBottomBuffer - m_CurrentPosition.Y) / nTileHeight;
         if (EndRow >= VirtualRows)
            EndRow = VirtualRows - 1;

         System.Collections.IEnumerator Injected = null;
         InjectedFrame CurFrame;
         if (m_InjectedFrames != null)
         {
            Injected = m_InjectedFrames.GetEnumerator();
            if (!Injected.MoveNext())
               Injected = null;
         }

         int lastColor = 0;

         for (int y = nStartRow; y <= EndRow; y++)
         {
            if (Injected != null)
            {
               while (((CurFrame = (InjectedFrame)Injected.Current).priority < Priority) ||
                  ((CurFrame.priority == Priority) && (CurFrame.y < y * nTileHeight)))
               {
                  if (CurFrame.color != lastColor)
                  {
                     Display.SetColor(CurFrame.color);
                     lastColor = CurFrame.color;
                  }
                  Display.DrawFrame(CurFrame.frame.GraphicSheetTexture, CurFrame.frame.SourceRect, CurFrame.frame.corners,
                     CurFrame.x + m_CurrentPosition.X, CurFrame.y + m_CurrentPosition.Y);
                  if (!Injected.MoveNext())
                  {
                     Injected = null;
                     break;
                  }
               }
            }

            Point[] suspendedTiles = null;

            if (m_SuspendedTiles != null)
            {
               m_SuspendedTiles.Sort(PointComparer.Value);
               suspendedTiles = (Point[])(m_SuspendedTiles.ToArray(typeof(Point)));
            }

            for (int x = nStartCol; x <= EndCol; x++)
            {
               int[] SubFrames;
               if (m_TileCache == null)
                  SubFrames = bgTileFrame;
               else
                  SubFrames = m_TileCache[this[x, y]];

               for (int nFrame = 0; nFrame < SubFrames.Length; nFrame++)
               {
                  if ((suspendedTiles != null) && (Array.BinarySearch(suspendedTiles, new Point(x,y), PointComparer.Value) >= 0))
                     continue;
                  FrameCache.Frame f = m_FrameCache[SubFrames[nFrame]%m_FrameCache.Count];
                  if (f.Color != lastColor)
                  {
                     Display.SetColor(f.Color);
                     lastColor = f.Color;
                  }

                  Display.DrawFrame(f.GraphicSheetTexture, f.SourceRect, f.corners,
                     x * nTileWidth + m_CurrentPosition.X, y * nTileHeight + m_CurrentPosition.Y);
               }
            }
         }

         while (Injected != null)
         {
            CurFrame = (InjectedFrame)Injected.Current;
            if (CurFrame.color != lastColor)
            {
               Display.SetColor(CurFrame.color);
               lastColor = CurFrame.color;
            }
            Display.DrawFrame(CurFrame.frame.GraphicSheetTexture, CurFrame.frame.SourceRect, CurFrame.frame.corners,
               CurFrame.x + m_CurrentPosition.X, CurFrame.y + m_CurrentPosition.Y);
            if (!Injected.MoveNext())
            {
               Injected = null;
               break;
            }
         }
      }

      public void InjectFrame(int x, int y, int priority, FrameCache.Frame frame)
      {
         InjectFrame(x, y, priority, frame, -1);
      }

      public void InjectFrame(int x, int y, int priority, FrameCache.Frame frame, int color)
      {
         int idx;
         InjectedFrame f = new InjectedFrame(x, y, priority, frame, color);
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
         if (m_TileCache.Count <= 0)
            return;
         ProjectDataset.TilesetRow tsr = m_Layer.TilesetRow;
         short nWidth = tsr.TileWidth;
         short nHeight = tsr.TileHeight;
         int[] SubFrames = m_TileCache[nTileValue];
         for (int nFrame = 0; nFrame < SubFrames.Length; nFrame++)
            InjectFrame(nCol * nWidth, nRow * nHeight, Priority, m_FrameCache[SubFrames[nFrame]]);
         if (m_SuspendedTiles == null)
         {
            m_SuspendedTiles = new System.Collections.ArrayList();
         }
         m_SuspendedTiles.Add(new Point(nCol, nRow));
      }

      public void ClearInjections()
      {
         m_InjectedFrames = null;
         m_SuspendedTiles = null;
      }

      public void InjectCachedSprites()
      {
         foreach(SpriteProvider sp in m_CachedSprites)
         {
            int nCount = sp.GetSubFrameCount();
            for(int i=0; i<nCount; i++)
            {
               InjectFrame(sp.X, sp.Y, sp.Priority, sp.GetSubFrame(i), sp.Color);
            }
         }
      }

      public void RefreshLayerSprites(SpriteCache cache)
      {
         ProjectDataset.SpriteRow[] SpriteRows = ProjectData.GetSortedSpriteRows(m_Layer, false);
         m_CachedSprites = new System.Collections.ArrayList(SpriteRows.Length);
         foreach(ProjectDataset.SpriteRow drSprite in SpriteRows)
         {
            ProjectDataset.SpriteDefinitionRow drSpriteDef = ProjectData.GetSpriteDefinition(drSprite.DefinitionName);
            SpriteProvider sp;
            if (ProjectData.IsLightSource(drSpriteDef))
               sp = new LightSpriteProvider(cache[drSprite.DefinitionName], drSprite);
            else
               sp = new SpriteProvider(cache[drSprite.DefinitionName], drSprite);
            m_CachedSprites.Add(sp);
         }
      }

      public SpriteProvider[] GetSpritesInRect(Rectangle rect)
      {
         System.Collections.ArrayList result = new System.Collections.ArrayList();
         foreach (SpriteProvider sp in m_CachedSprites)
         {
            if (rect.IntersectsWith(sp.Bounds))
               result.Add(sp);
         }
         return (SpriteProvider[])result.ToArray(typeof(SpriteProvider));
      }

      public SpriteProvider[] GetSpritesAtPoint(Point pt)
      {
         System.Collections.ArrayList result = new System.Collections.ArrayList();
         foreach (SpriteProvider sp in m_CachedSprites)
         {
            if (sp.Bounds.Contains(pt))
               result.Add(sp);
         }
         return (SpriteProvider[])result.ToArray(typeof(SpriteProvider));
      }
      #endregion
   }
}
