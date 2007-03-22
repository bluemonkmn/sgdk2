using System;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

/// <summary>
/// Summary description for LayerBase.
/// </summary>
[Serializable()]
public abstract class LayerBase : System.Collections.IEnumerable
{
   #region Embedded Classes
   private class InjectedFrame : IComparable
   {
      public int x;
      public int y;
      /* The runtime implementation only uses priority to determine
       * whether the frame is behind the layer (-1), on the layer (0)
       * or in front of the layer(1).  The rest of the priority
       * handling is pre-processed by having sprites sorted in the
       * original arrays.
       */
      public int priority;
      public Frame frame;
      public int color;
      public InjectedFrame(int x, int y, int priority, Frame frame, int color)
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
            this.color = Microsoft.DirectX.Direct3D.ColorOperator.Modulate(ColorValue.FromArgb(frame.Color), ColorValue.FromArgb(color)).ToArgb();
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
   protected readonly Tileset m_Tileset;
   private Frameset m_Frameset;
   [NonSerialized()]
   private System.Collections.ArrayList m_InjectedFrames = null;
   
   private readonly int m_nLeftBuffer;
   private readonly int m_nTopBuffer;
   private readonly int m_nRightBuffer;
   private readonly int m_nBottomBuffer;
   protected readonly int m_nColumns;
   protected readonly int m_nRows;
   private readonly int m_nVirtualColumns;
   private readonly int m_nVirtualRows;
   private System.Drawing.Point m_AbsolutePosition;
   protected SpriteCollection m_Sprites;
   private readonly System.Drawing.SizeF m_ScrollRate;
   private System.Drawing.Point[] m_CurrentPosition = new System.Drawing.Point[Project.MaxViews];
   private MapBase m_ParentMap;
   public LayerSpriteCategoriesBase m_SpriteCategories;
   #endregion

   protected LayerBase(Tileset Tileset, MapBase Parent, int nLeftBuffer, int nTopBuffer, int nRightBuffer, int nBottomBuffer,
      int nColumns, int nRows, int nVirtualColumns, int nVirtualRows, System.Drawing.Point Position, System.Drawing.SizeF ScrollRate)
   {
      this.m_ParentMap = Parent;
      this.m_Tileset = Tileset;
      this.m_Frameset = Tileset.GetFrameset(Parent.Display);
      this.m_nLeftBuffer = nLeftBuffer;
      this.m_nTopBuffer = nTopBuffer;
      this.m_nRightBuffer = nRightBuffer;
      this.m_nBottomBuffer = nBottomBuffer;
      this.m_nColumns = nColumns;
      this.m_nRows = nRows;
      if (nVirtualColumns == 0)
         this.m_nVirtualColumns = nColumns;
      else
         this.m_nVirtualColumns = nVirtualColumns;
      if (nVirtualRows == 0)
         this.m_nVirtualRows = nRows;
      else
         this.m_nVirtualRows = nVirtualRows;
      this.m_AbsolutePosition = Position;
      this.m_ScrollRate = ScrollRate;
      this.Move(new Point(0,0));
   }

   #region Abstract Members
   /// <summary>
   /// Retrieves the value of a tile at the specified tile coordinate
   /// </summary>
   public abstract int this[int x, int y]
   {
      get;
      set;
   }
   public abstract int[] GetTileFrame(int x, int y);
   public abstract TileBase GetTile(int x, int y);
   public abstract void InjectSprites();
   #endregion

   #region IEnumerable Members
   public System.Collections.IEnumerator GetEnumerator()
   {
      return new ActiveSpriteEnumerator(m_Sprites);
   }
   #endregion

   #region Properties
   /// <summary>
   /// Get the number of columns of tiles in the layer's tile data
   /// </summary>
   public int ActualColumns
   {
      get
      {
         return m_nColumns;
      }
   }
   /// <summary>
   /// Get the number of rows of tiles in the layer's tile data
   /// </summary>
   public int ActualRows
   {
      get
      {
         return m_nRows;
      }
   }

   /// <summary>
   /// Get the number of columns of tiles in the layer's displayed
   /// virtual scroll space (data is wrapped)
   /// </summary>
   public int VirtualColumns
   {
      get
      {
         return m_nVirtualColumns;
      }
   }
   /// <summary>
   /// Get the number of rows of tiles in the layer's displayed
   /// virtual scroll space (data is wrapped)
   /// </summary>
   public int VirtualRows
   {
      get
      {
         return m_nVirtualRows;
      }
   }

   /// <summary>
   /// Get or set the position of the layer within the map. 
   /// (Does not affect current position until <see cref="Move"/> is called)
   /// </summary>
   public System.Drawing.Point AbsolutePosition
   {
      get
      {
         return m_AbsolutePosition;
      }
      set
      {
         m_AbsolutePosition = value;
      }
   }

   /// <summary>
   /// Gets the scroll rate that is applied to <see cref="Move"/> operations.
   /// </summary>
   public System.Drawing.SizeF ScrollRate
   {
      get
      {
         return m_ScrollRate;
      }
   }

   /// <summary>
   /// Gets/Sets the current pixel position of the layer relative to the screen.
   /// (Setting this directly ignores <see cref="ScrollRate"/> and <see cref="AbsolutePosition"/>.)
   /// </summary>
   public System.Drawing.Point CurrentPosition
   {
      get
      {
         return m_CurrentPosition[ParentMap.CurrentViewIndex];
      }
      set
      {
         m_CurrentPosition[ParentMap.CurrentViewIndex] = value;
      }
   }

   /// <summary>
   /// Returns the map that owns this layer
   /// </summary>
   public MapBase ParentMap
   {
      get
      {
         return m_ParentMap;
      }
   }
   
   public Tileset Tileset
   {
      get
      {
         return m_Tileset;
      }
   }
   #endregion

   #region Public methods
   /// <summary>
   /// Move/Scroll the layer to a new position based on the current map position.
   /// The layer's current position is offset by its position on the map and scaled
   /// by the layer's scroll rate.
   /// </summary>
   /// <param name="MapPosition">Position of the map.  If one component is int.minValue,
   /// that axis is not affected</param>
   /// <remarks>Map positions are usually negative because the map position indicates
   /// the position of the top-left corner of the map which is usually scrolled off
   /// the top-left corner of the screen to a negative position.</remarks>
   public void Move(Point MapPosition)
   {
      if (MapPosition.X != int.MinValue)
         if (MapPosition.Y != int.MinValue)
            CurrentPosition = new Point(m_AbsolutePosition.X + (int)(MapPosition.X * m_ScrollRate.Width), m_AbsolutePosition.Y + (int)(MapPosition.Y * m_ScrollRate.Height));
         else
            CurrentPosition = new Point(m_AbsolutePosition.X + (int)(MapPosition.X * m_ScrollRate.Width), CurrentPosition.Y);
      else if (MapPosition.Y != int.MinValue)
         CurrentPosition = new Point(CurrentPosition.X, m_AbsolutePosition.Y + (int)(MapPosition.Y * m_ScrollRate.Height));
   }

   public void Draw()
   {
      int nTileWidth = m_Tileset.TileWidth;
      int nTileHeight = m_Tileset.TileHeight;

      int nStartCol = (-m_nLeftBuffer - CurrentPosition.X) / nTileWidth;
      if (nStartCol < 0)
         nStartCol = 0;
      int nStartRow = (-m_nTopBuffer - CurrentPosition.Y) / nTileHeight;
      if (nStartRow < 0)
         nStartRow = 0;

      Rectangle ViewRect = m_ParentMap.CurrentView;
      m_ParentMap.Display.Device.RenderState.ScissorTestEnable = true;
      m_ParentMap.Display.Device.ScissorRectangle = ViewRect;

      int EndCol = (ViewRect.Width - 1 + m_nRightBuffer - CurrentPosition.X) / nTileWidth;
      if (EndCol >= VirtualColumns)
         EndCol = VirtualColumns - 1;
      int EndRow = (ViewRect.Height - 1 + m_nBottomBuffer - CurrentPosition.Y) / nTileHeight;
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

      Sprite spr = m_ParentMap.Display.Sprite;

      for (int y = nStartRow; y <= EndRow; y++)
      {
         if (Injected != null)
         {
            while ((((CurFrame = (InjectedFrame)Injected.Current).y < y * nTileHeight)) && (CurFrame.priority <= 0) ||
                   (CurFrame.priority < 0))
            {
               spr.Transform = Matrix.Multiply(CurFrame.frame.Transform, Matrix.Translation(
                  (float)CurFrame.x + CurrentPosition.X + ViewRect.X,
                  (float)CurFrame.y + CurrentPosition.Y + ViewRect.Y, 0));
               spr.Draw(CurFrame.frame.GraphicSheetTexture.Texture, CurFrame.frame.SourceRect,
                  Vector3.Empty, Vector3.Empty, CurFrame.color);
               if (!Injected.MoveNext())
               {
                  Injected = null;
                  break;
               }
            }
         }

         for (int x = nStartCol; x <= EndCol; x++)
         {
            int[] SubFrames = GetTileFrame(x,y);
            for (int nFrame = 0; nFrame < SubFrames.Length; nFrame++)
            {
               Frame f = m_Frameset[SubFrames[nFrame]];
               spr.Transform = Matrix.Multiply(f.Transform, Matrix.Translation(
                  x * nTileWidth + CurrentPosition.X + ViewRect.X,
                  y * nTileHeight + CurrentPosition.Y + ViewRect.Y, 0));
               spr.Draw(f.GraphicSheetTexture.Texture, f.SourceRect, Vector3.Empty, Vector3.Empty, f.Color);
            }
         }
      }

      while (Injected != null)
      {
         CurFrame = (InjectedFrame)Injected.Current;
         spr.Transform = Matrix.Multiply(CurFrame.frame.Transform, Matrix.Translation(
            (float)CurFrame.x + CurrentPosition.X + ViewRect.X,
            (float)CurFrame.y + CurrentPosition.Y + ViewRect.Y, 0));
         spr.Draw(CurFrame.frame.GraphicSheetTexture.Texture, CurFrame.frame.SourceRect,
            Vector3.Empty, Vector3.Empty, CurFrame.color);
         if (!Injected.MoveNext())
         {
            Injected = null;
            break;
         }
      }
   }

   public Rectangle VisibleArea
   {
      get
      {
         return new Rectangle(new System.Drawing.Point(-CurrentPosition.X, -CurrentPosition.Y), m_ParentMap.CurrentView.Size);
      }
   }

   public bool IsSpriteVisible(SpriteBase sprite)
   {
      return sprite.isActive && sprite.GetBounds().IntersectsWith(VisibleArea);
   }

   public void InjectFrames(int x, int y, Frame[] frames)
   {
      InjectFrames(x, y, frames, -1);
   }

   public void InjectFrames(int x, int y, Frame[] frames, int color)
   {
      if (frames.Length <= 0)
         return;

      InjectedFrame[] additions = new InjectedFrame[frames.Length];
      for (int idx=0; idx<frames.Length; idx++)
         additions[idx] = new InjectedFrame(x, y, 0, frames[idx], color);

      int insIdx;
      if (m_InjectedFrames == null)
      {
         m_InjectedFrames = new System.Collections.ArrayList();
         insIdx = 0;
      }
      else
      {
         insIdx = m_InjectedFrames.BinarySearch(additions[0]);
         if (insIdx < 0)
            insIdx = ~insIdx;
      }
      m_InjectedFrames.InsertRange(insIdx, additions);
   }

   public void AppendFrames(int x, int y, Frame[] frames, int color, int priority)
   {
      InjectedFrame[] additions = new InjectedFrame[frames.Length];
      for (int idx=0; idx<frames.Length; idx++)
         additions[idx] = new InjectedFrame(x, y, priority, frames[idx], color);
      if (m_InjectedFrames == null)
      {
         m_InjectedFrames = new System.Collections.ArrayList(additions);
      }
      else
         m_InjectedFrames.AddRange(additions);
   }

   public void ClearInjections()
   {
      if (m_InjectedFrames != null)
         m_InjectedFrames.Clear();
   }

   public Point GetMousePosition()
   {
      Point dispPos;
      if (m_ParentMap.Display.Windowed)
         dispPos = m_ParentMap.Display.PointToClient(System.Windows.Forms.Control.MousePosition);
      else
         dispPos = System.Windows.Forms.Control.MousePosition;
      dispPos.Offset(-CurrentPosition.X, -CurrentPosition.Y);
      return dispPos;
   }

   public void ScrollSpriteIntoView(SpriteBase sprite, bool useScrollMargins)
   {
      Rectangle spriteBounds = sprite.GetBounds();
      int newX = int.MinValue;
      int newY = int.MinValue;
      int marginLeft;
      int marginTop;
      int marginRight;
      int marginBottom;
      if (useScrollMargins)
      {
         marginLeft = ParentMap.ScrollMarginLeft;
         marginTop = ParentMap.ScrollMarginTop;
         marginRight = ParentMap.ScrollMarginRight;
         marginBottom = ParentMap.ScrollMarginBottom;
      }
      else
      {
         marginLeft = 0;
         marginTop = 0;
         marginRight = 0;
         marginBottom = 0;
      }
      if (spriteBounds.Left + CurrentPosition.X < marginLeft)
      {
         if (ScrollRate.Width > 0)
            newX = (int)((-spriteBounds.Left + marginLeft - AbsolutePosition.X) / ScrollRate.Width);
         else
            CurrentPosition = new Point(-spriteBounds.Left + marginLeft, CurrentPosition.Y);
      }
      else if (spriteBounds.Right + CurrentPosition.X > VisibleArea.Width - marginRight)
      {
         if (ScrollRate.Width > 0)
            newX = (int)((-spriteBounds.Right + VisibleArea.Width - marginRight - AbsolutePosition.X) / ScrollRate.Width);
         else
            CurrentPosition = new Point(-spriteBounds.Right + VisibleArea.Width - marginRight, CurrentPosition.Y);
      }

      if (spriteBounds.Top + CurrentPosition.Y < marginTop)
      {
         if (ScrollRate.Height > 0)
            newY = (int)((-spriteBounds.Top + marginTop - AbsolutePosition.Y) / ScrollRate.Height);
         else
            CurrentPosition = new Point(CurrentPosition.X, -spriteBounds.Top + marginTop);
      }
      else if (spriteBounds.Bottom + CurrentPosition.Y > VisibleArea.Height - marginBottom)
      {
         if (ScrollRate.Height > 0)
            newY = (int)((-spriteBounds.Bottom + VisibleArea.Height - marginBottom - AbsolutePosition.Y) / ScrollRate.Height);
         else
            CurrentPosition = new Point(CurrentPosition.X, -spriteBounds.Bottom + VisibleArea.Height - marginBottom);
      }
      ParentMap.Scroll(new Point(newX, newY));
   }

   public void PushSpriteIntoView(SpriteBase sprite, bool stayInScrollMargins)
   {
      Rectangle spriteBounds = sprite.GetBounds();
      int marginLeft;
      int marginTop;
      int marginRight;
      int marginBottom;
      if (stayInScrollMargins)
      {
         marginLeft = ParentMap.ScrollMarginLeft;
         marginTop = ParentMap.ScrollMarginTop;
         marginRight = ParentMap.ScrollMarginRight;
         marginBottom = ParentMap.ScrollMarginBottom;
      }
      else
      {
         marginLeft = 0;
         marginTop = 0;
         marginRight = 0;
         marginBottom = 0;
      }

      if (spriteBounds.Left + CurrentPosition.X < marginLeft)
      {
         if (double.IsNaN(sprite.LocalDX))
            sprite.dx = marginLeft - CurrentPosition.X - spriteBounds.Left;
         else
            sprite.LocalDX = marginLeft - CurrentPosition.X - spriteBounds.Left - sprite.RidingOn.dx;
      }
      else if (spriteBounds.Right + CurrentPosition.X > VisibleArea.Width - marginRight)
      {
         if (double.IsNaN(sprite.LocalDX))
            sprite.dx = VisibleArea.Width - marginRight - CurrentPosition.X - spriteBounds.Right;
         else
            sprite.LocalDX = VisibleArea.Width - marginRight - CurrentPosition.X - spriteBounds.Right - sprite.RidingOn.dx;
      }

      if (spriteBounds.Top + CurrentPosition.Y < marginTop)
      {
         if (double.IsNaN(sprite.LocalDY))
            sprite.dy = marginTop - spriteBounds.Top - CurrentPosition.Y;
         else
            sprite.LocalDY = marginTop - spriteBounds.Top - CurrentPosition.Y - sprite.RidingOn.dy;
      }
      else if (spriteBounds.Bottom + CurrentPosition.Y > VisibleArea.Height - marginBottom)
      {
         if (double.IsNaN(sprite.LocalDY))
            sprite.dy = VisibleArea.Height - marginBottom - spriteBounds.Bottom - CurrentPosition.Y;
         else
            sprite.LocalDY = VisibleArea.Height - marginBottom - spriteBounds.Bottom - CurrentPosition.Y - sprite.RidingOn.dy;
      }
   }

   public int GetTopSolidPixel(Rectangle testArea, Solidity solid)
   {
      int topTile = (testArea.Top + m_Tileset.TileHeight) / m_Tileset.TileHeight - 1;
      int bottomTile = (int)((testArea.Top + testArea.Height - 1) / m_Tileset.TileHeight);
      int leftTile = (testArea.Left + m_Tileset.TileHeight) / m_Tileset.TileWidth - 1;
      int rightTile = (int)((testArea.Left + testArea.Width - 1) / m_Tileset.TileWidth);
      bool outOfBounds = false;
      if ((topTile < 0) || (topTile >= VirtualRows) || (bottomTile < 0) || (bottomTile >= VirtualRows)
         || (leftTile < 0) || (leftTile >= VirtualColumns) || (rightTile < 0) || (rightTile >= VirtualColumns))
         outOfBounds = true;
      short minTileTop = (short)(testArea.Top % m_Tileset.TileHeight);
      int tileLeft = leftTile * m_Tileset.TileWidth;
      for (int y = topTile; y <= bottomTile; y++)
      {
         if (rightTile == leftTile)
         {
            short topMost;
            if (outOfBounds && ((leftTile < 0) || (leftTile >= VirtualColumns) || (y < 0) || (y >= VirtualRows)))
               topMost = 0;
            else
               topMost = solid.GetCurrentTileShape(m_Tileset[this[leftTile,y]]).GetTopSolidPixel(
                  m_Tileset.TileWidth, m_Tileset.TileHeight, (short)(testArea.Left - tileLeft),
                  (short)(testArea.Left + testArea.Width - 1 - tileLeft));
            if ((topMost != short.MaxValue) && ((y > topTile) || (topMost >= minTileTop)))
            {
               int result = topMost + y * m_Tileset.TileHeight;
               if (result < testArea.Top + testArea.Height)
                  return result;
               else
                  return int.MinValue;
            }
         }
         else
         {
            short topMost;
            if (outOfBounds && ((leftTile < 0) || (leftTile >= VirtualColumns) || (y < 0) || (y >= VirtualRows)))
               topMost = 0;
            else
               topMost = solid.GetCurrentTileShape(m_Tileset[this[leftTile,y]]).GetTopSolidPixel(
                  m_Tileset.TileWidth, m_Tileset.TileHeight, (short)(testArea.Left - tileLeft), (short)(m_Tileset.TileWidth - 1));
            if ((y == topTile) && (topMost < minTileTop))
               topMost = short.MaxValue;
            short top;
            for (int x = leftTile + 1; x < rightTile; x++)
            {
               if (outOfBounds && ((x < 0) || (x >= VirtualColumns) || (y < 0) || (y >= VirtualRows)))
                  top = 0;
               else
                  top = solid.GetCurrentTileShape(m_Tileset[this[x,y]]).GetTopSolidPixel(
                     m_Tileset.TileWidth, m_Tileset.TileHeight, 0, (short)(m_Tileset.TileWidth - 1));
               if ((top < topMost) && ((y > topTile) || (top >= minTileTop)))
                  topMost = top;
            }
            if (outOfBounds && ((rightTile < 0) || (rightTile >= VirtualColumns) || (y < 0) || (y >= VirtualRows)))
               top = 0;
            else
               top = solid.GetCurrentTileShape(m_Tileset[this[rightTile,y]]).GetTopSolidPixel(
                  m_Tileset.TileWidth, m_Tileset.TileHeight, 0, (short)((testArea.Left + testArea.Width - 1) % m_Tileset.TileWidth));
            if ((top < topMost) && ((y > topTile) || (top >= minTileTop)))
               topMost = top;
            if (topMost != short.MaxValue)
            {
               int result = topMost + y * m_Tileset.TileHeight;
               if (result < testArea.Top + testArea.Height)
                  return result;
               else
                  return int.MinValue;
            }
         }
      }
      return int.MinValue;
   }

   public int GetBottomSolidPixel(Rectangle testArea, Solidity solid)
   {
      int topTile = (testArea.Top + m_Tileset.TileHeight) / m_Tileset.TileHeight - 1;
      int bottomTile = (int)((testArea.Top + testArea.Height - 1) / m_Tileset.TileHeight);
      int leftTile = (testArea.Left + m_Tileset.TileHeight) / m_Tileset.TileWidth - 1;
      int rightTile = (int)((testArea.Left + testArea.Width - 1) / m_Tileset.TileWidth);
      bool outOfBounds = false;
      if ((topTile < 0) || (topTile >= VirtualRows) || (bottomTile < 0) || (bottomTile >= VirtualRows)
         || (leftTile < 0) || (leftTile >= VirtualColumns) || (rightTile < 0) || (rightTile >= VirtualColumns))
         outOfBounds = true;
      short maxTileBottom = (short)((testArea.Top+testArea.Height-1) % m_Tileset.TileHeight);
      int tileLeft = leftTile * m_Tileset.TileWidth;
      for (int y = bottomTile; y >= topTile; y--)
      {
         if (rightTile == leftTile)
         {
            short bottomMost;
            if (outOfBounds && ((leftTile < 0) || (leftTile >= VirtualColumns) || (y < 0) || (y >= VirtualRows)))
               bottomMost = (short)(m_Tileset.TileHeight - 1);
            else
               bottomMost = solid.GetCurrentTileShape(m_Tileset[this[leftTile,y]]).GetBottomSolidPixel(
                  m_Tileset.TileWidth, m_Tileset.TileHeight, (short)(testArea.Left - tileLeft),
                  (short)(testArea.Left + testArea.Width - 1 - tileLeft));
            if ((bottomMost != short.MinValue) && ((y < bottomTile) || (bottomMost <= maxTileBottom)))
            {
               int result = bottomMost + y * m_Tileset.TileHeight;
               if (result >= testArea.Top)
                  return result;
               else
                  return int.MinValue;
            }
         }
         else
         {
            short bottomMost;
            if (outOfBounds && ((leftTile < 0) || (leftTile >= VirtualColumns) || (y < 0) || (y >= VirtualRows)))
               bottomMost = (short)(m_Tileset.TileHeight - 1);
            else
               bottomMost = solid.GetCurrentTileShape(m_Tileset[this[leftTile,y]]).GetBottomSolidPixel(
                  m_Tileset.TileWidth, m_Tileset.TileHeight, (short)(testArea.Left - tileLeft), (short)(m_Tileset.TileWidth - 1));
            if ((y == bottomTile) && (bottomMost > maxTileBottom))
               bottomMost = short.MinValue;
            short bottom;
            for (int x = leftTile + 1; x < rightTile; x++)
            {
               if (outOfBounds && ((x < 0) || (x >= VirtualColumns) || (y < 0) || (y >= VirtualRows)))
                  bottom = (short)(m_Tileset.TileHeight - 1);
               else
                  bottom = solid.GetCurrentTileShape(m_Tileset[this[x,y]]).GetBottomSolidPixel(
                     m_Tileset.TileWidth, m_Tileset.TileHeight, 0, (short)(m_Tileset.TileWidth - 1));
               if ((bottom > bottomMost) && ((y < bottomTile) || (bottom <= maxTileBottom)))
                  bottomMost = bottom;
            }
            if (outOfBounds && ((rightTile < 0) || (rightTile >= VirtualColumns) || (y < 0) || (y >= VirtualRows)))
               bottom = (short)(m_Tileset.TileHeight - 1);
            else
               bottom = solid.GetCurrentTileShape(m_Tileset[this[rightTile,y]]).GetBottomSolidPixel(
                  m_Tileset.TileWidth, m_Tileset.TileHeight, 0, (short)((testArea.Left + testArea.Width - 1) % m_Tileset.TileWidth));
            if ((bottom > bottomMost) && ((y < bottomTile) || (bottom <= maxTileBottom)))
               bottomMost = bottom;
            if (bottomMost != short.MinValue)
            {
               int result = bottomMost + y * m_Tileset.TileHeight;
               if (result >= testArea.Top)
                  return result;
               else
                  return int.MinValue;
            }
         }
      }
      return int.MinValue;
   }

   public int GetLeftSolidPixel(Rectangle testArea, Solidity solid)
   {
      int topTile = (testArea.Top + m_Tileset.TileHeight) / m_Tileset.TileHeight - 1;
      int bottomTile = (int)((testArea.Top + testArea.Height - 1) / m_Tileset.TileHeight);
      int leftTile = (testArea.Left + m_Tileset.TileHeight) / m_Tileset.TileWidth - 1;
      int rightTile = (int)((testArea.Left + testArea.Width - 1) / m_Tileset.TileWidth);
      bool outOfBounds = false;
      if ((topTile < 0) || (topTile >= VirtualRows) || (bottomTile < 0) || (bottomTile >= VirtualRows)
         || (leftTile < 0) || (leftTile >= VirtualColumns) || (rightTile < 0) || (rightTile >= VirtualColumns))
         outOfBounds = true;
      short minTileLeft = (short)(testArea.Left % m_Tileset.TileWidth);
      int tileTop = topTile * m_Tileset.TileHeight;
      for (int x = leftTile; x <= rightTile; x++)
      {
         if (bottomTile == topTile)
         {
            short leftMost;
            if (outOfBounds && ((topTile < 0) || (topTile >= VirtualRows) || (x < 0) || (x >= VirtualColumns)))
               leftMost = 0;
            else
               leftMost = solid.GetCurrentTileShape(m_Tileset[this[x,topTile]]).GetLeftSolidPixel(
                  m_Tileset.TileWidth, m_Tileset.TileHeight, (short)(testArea.Top - tileTop),
                  (short)(testArea.Top + testArea.Height - 1 - tileTop));
            if ((leftMost != short.MaxValue) && ((x > leftTile) || (leftMost >= minTileLeft)))
            {
               int result = leftMost + x * m_Tileset.TileWidth;
               if (result < testArea.Left + testArea.Width)
                  return result;
               else
                  return int.MinValue;
            }
         }
         else
         {
            short leftMost;
            if (outOfBounds && ((topTile < 0) || (topTile >= VirtualRows) || (x < 0) || (x >= VirtualColumns)))
               leftMost = 0;
            else
               leftMost = solid.GetCurrentTileShape(m_Tileset[this[x, topTile]]).GetLeftSolidPixel(
                  m_Tileset.TileWidth, m_Tileset.TileHeight, (short)(testArea.Top - tileTop), (short)(m_Tileset.TileHeight - 1));
            if ((x == leftTile) && (leftMost < minTileLeft))
               leftMost = short.MaxValue;
            short left;
            for (int y = topTile + 1; y < bottomTile; y++)
            {
               if (outOfBounds && ((x < 0) || (x >= VirtualColumns) || (y < 0) || (y >= VirtualRows)))
                  left = 0;
               else
                  left = solid.GetCurrentTileShape(m_Tileset[this[x,y]]).GetLeftSolidPixel(
                     m_Tileset.TileWidth, m_Tileset.TileHeight, 0, (short)(m_Tileset.TileHeight - 1));
               if ((left < leftMost) && ((x > leftTile) || (left >= minTileLeft)))
                  leftMost = left;
            }
            if (outOfBounds && ((bottomTile < 0) || (bottomTile >= VirtualRows) || (x < 0) || (x >= VirtualColumns)))
               left = 0;
            else
               left = solid.GetCurrentTileShape(m_Tileset[this[x, bottomTile]]).GetLeftSolidPixel(
                  m_Tileset.TileWidth, m_Tileset.TileHeight, 0, (short)((testArea.Top + testArea.Height - 1) % m_Tileset.TileHeight));
            if ((left < leftMost) && ((x > leftTile) || (left >= minTileLeft)))
               leftMost = left;
            if (leftMost != short.MaxValue)
            {
               int result = leftMost + x * m_Tileset.TileWidth;
               if (result < testArea.Left + testArea.Width)
                  return result;
               else
                  return int.MinValue;
            }
         }
      }
      return int.MinValue;
   }

   public int GetRightSolidPixel(Rectangle testArea, Solidity solid)
   {
      int topTile = (testArea.Top + m_Tileset.TileHeight) / m_Tileset.TileHeight - 1;
      int bottomTile = (int)((testArea.Top + testArea.Height - 1) / m_Tileset.TileHeight);
      int leftTile = (testArea.Left + m_Tileset.TileHeight) / m_Tileset.TileWidth - 1;
      int rightTile = (int)((testArea.Left + testArea.Width - 1) / m_Tileset.TileWidth);
      bool outOfBounds = false;
      if ((topTile < 0) || (topTile >= VirtualRows) || (bottomTile < 0) || (bottomTile >= VirtualRows)
         || (leftTile < 0) || (leftTile >= VirtualColumns) || (rightTile < 0) || (rightTile >= VirtualColumns))
         outOfBounds = true;
      short maxTileRight = (short)((testArea.Left+testArea.Width-1) % m_Tileset.TileWidth);
      int tileTop = topTile * m_Tileset.TileHeight;
      for (int x = rightTile; x >= leftTile; x--)
      {
         if (bottomTile == topTile)
         {
            short rightMost;
            if (outOfBounds && ((topTile < 0) || (topTile >= VirtualRows) || (x < 0) || (x >= VirtualColumns)))
               rightMost = (short)(m_Tileset.TileWidth - 1);
            else
               rightMost = solid.GetCurrentTileShape(m_Tileset[this[x,topTile]]).GetRightSolidPixel(
                  m_Tileset.TileWidth, m_Tileset.TileHeight, (short)(testArea.Top - tileTop),
                  (short)(testArea.Top + testArea.Height - 1 - tileTop));
            if ((rightMost != short.MinValue) && ((x < rightTile) || (rightMost <= maxTileRight)))
            {
               int result = rightMost + x * m_Tileset.TileWidth;
               if (result >= testArea.Left)
                  return result;
               else
                  return int.MinValue;
            }
         }
         else
         {
            short rightMost;
            if (outOfBounds && ((topTile < 0) || (topTile >= VirtualRows) || (x < 0) || (x >= VirtualColumns)))
               rightMost = (short)(m_Tileset.TileWidth - 1);
            else
               rightMost = solid.GetCurrentTileShape(m_Tileset[this[x, topTile]]).GetRightSolidPixel(
                  m_Tileset.TileWidth, m_Tileset.TileHeight, (short)(testArea.Top - tileTop), (short)(m_Tileset.TileHeight - 1));
            if ((x == rightTile) && (rightMost > maxTileRight))
               rightMost = short.MinValue;
            short right;
            for (int y = topTile + 1; y < bottomTile; y++)
            {
               if (outOfBounds && ((x < 0) || (x >= VirtualColumns) || (y < 0) || (y >= VirtualRows)))
                  right = (short)(m_Tileset.TileWidth - 1);
               else
                  right = solid.GetCurrentTileShape(m_Tileset[this[x,y]]).GetRightSolidPixel(
                     m_Tileset.TileWidth, m_Tileset.TileHeight, 0, (short)(m_Tileset.TileHeight - 1));
               if ((right > rightMost) && ((x < rightTile) || (right <= maxTileRight)))
                  rightMost = right;
            }
            if (outOfBounds && ((bottomTile < 0) || (bottomTile >= VirtualRows) || (x < 0) || (x >= VirtualColumns)))
               right = (short)(m_Tileset.TileWidth - 1);
            else
               right = solid.GetCurrentTileShape(m_Tileset[this[x, bottomTile]]).GetRightSolidPixel(
                  m_Tileset.TileWidth, m_Tileset.TileHeight, 0, (short)((testArea.Top + testArea.Height - 1) % m_Tileset.TileHeight));
            if ((right > rightMost) && ((x < rightTile) || (right <= maxTileRight)))
               rightMost = right;
            if (rightMost != short.MinValue)
            {
               int result = rightMost + x * m_Tileset.TileWidth;
               if (result >= testArea.Left)
                  return result;
               else
                  return int.MinValue;
            }
         }
      }
      return int.MinValue;
   }
   #endregion
}

[Serializable()]
public abstract class IntLayer : LayerBase
{
   private int[,] m_Tiles;

   public IntLayer(Tileset Tileset, MapBase Parent, int nLeftBuffer, int nTopBuffer, int nRightBuffer,
      int nBottomBuffer, int nColumns, int nRows, int nVirtualColumns, int nVirtualRows,
      System.Drawing.Point Position, System.Drawing.SizeF ScrollRate, string Name) : 
      base(Tileset, Parent, nLeftBuffer, nTopBuffer, nRightBuffer,
      nBottomBuffer, nColumns, nRows, nVirtualColumns, nVirtualRows, Position, ScrollRate)
   {
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(Parent.GetType());
      m_Tiles = (int[,])(resources.GetObject(Name));
   }

   public override int this[int x, int y]
   {
      get
      {
         return m_Tiles[x % m_nColumns, y % m_nRows];
      }
      set
      {
         m_Tiles[x % m_nColumns, y % m_nRows] = value;
      }
   }

   public override int[] GetTileFrame(int x, int y)
   {
      return m_Tileset[m_Tiles[x % m_nColumns, y % m_nRows]].CurrentFrame;
   }

   public override TileBase GetTile(int x, int y)
   {
      return m_Tileset[m_Tiles[x % m_nColumns,y % m_nRows]];
   }
}

[Serializable()]
public abstract class ShortLayer : LayerBase
{
   private short[,] m_Tiles;

   public ShortLayer(Tileset Tileset, MapBase Parent, int nLeftBuffer, int nTopBuffer, int nRightBuffer,
      int nBottomBuffer, int nColumns, int nRows, int nVirtualColumns, int nVirtualRows, System.Drawing.Point Position,
      System.Drawing.SizeF ScrollRate, string Name) : 
      base(Tileset, Parent, nLeftBuffer, nTopBuffer, nRightBuffer,
      nBottomBuffer, nColumns, nRows, nVirtualColumns, nVirtualRows, Position, ScrollRate)
   {
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(Parent.GetType());
      m_Tiles = (short[,])(resources.GetObject(Name));
   }

   public override int this[int x, int y]
   {
      get
      {
         return (int)(m_Tiles[x,y]);
      }
      set
      {
         m_Tiles[x,y] = (short)value;
      }
   }

   public override int[] GetTileFrame(int x, int y)
   {
      return m_Tileset[m_Tiles[x,y]].CurrentFrame;
   }

   public override TileBase GetTile(int x, int y)
   {
      return m_Tileset[m_Tiles[x,y]];
   }
}

[Serializable()]
public abstract class ByteLayer : LayerBase
{
   private byte[,] m_Tiles;

   public ByteLayer(Tileset Tileset, MapBase Parent, int nLeftBuffer, int nTopBuffer, int nRightBuffer,
      int nBottomBuffer, int nColumns, int nRows, int nVirtualColumns, int nVirtualRows, System.Drawing.Point Position,
      System.Drawing.SizeF ScrollRate, string Name) : 
      base(Tileset, Parent, nLeftBuffer, nTopBuffer, nRightBuffer,
      nBottomBuffer, nColumns, nRows, nVirtualColumns, nVirtualRows, Position, ScrollRate)
   {
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(Parent.GetType());
      m_Tiles = (byte[,])(resources.GetObject(Name));
   }

   public override int this[int x, int y]
   {
      get
      {
         return (int)(m_Tiles[x % m_nColumns, y % m_nRows]);
      }
      set
      {
         m_Tiles[x % m_nColumns, y % m_nRows] = (byte)value;
      }
   }

   public override int[] GetTileFrame(int x, int y)
   {
      return m_Tileset[m_Tiles[x % m_nColumns, y % m_nRows]].CurrentFrame;
   }

   public override TileBase GetTile(int x, int y)
   {
      return m_Tileset[m_Tiles[x % m_nColumns, y % m_nRows]];
   }
}