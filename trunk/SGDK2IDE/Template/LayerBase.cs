/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */
using System;
using System.Drawing;

/// <summary>
/// Defines the basic operation of a layer of tiles and sprites within a map.
/// </summary>
[Serializable()]
public abstract partial class LayerBase : System.Collections.IEnumerable
{
   #region Embedded Classes
   /// <summary>
   /// Maintains information about a <see cref="Frame"/> that has been inserted
   /// into the layer for the current loop.
   /// </summary>
   /// <remarks>Frames may be inserted behind the layer (drawn before the tiles),
   /// Appended in front of the layer (drawn after all the tiles) or inserted between
   /// rows of tiles in the layer (drawn after the rows above it, and before the rows
   /// below it). Frames are generally injected into a layer by the existence of a
   /// sprite on the layer.</remarks>
   private class InjectedFrame : IComparable
   {
      /// <summary>
      /// Horizontal pixel coordinate of the frame within the layer
      /// </summary>
      public int x;
      /// <summary>
      /// Vertical pixel coordinate of the frame within the layer
      /// </summary>
      public int y;
      /// <summary>
      /// Determines whether the frame is drawn behind the layer, in front of it,
      /// ir interleaved with the tiles.
      /// </summary>
      /// <value><list type="table">
      /// <listheader><term>Value</term><description>Drawing Order</description></listheader>
      /// <item><term>-1</term><description>Behind tiles</description></item>
      /// <item><term>0</term><description>Interleaved with tiles</description></item>
      /// <item><term>1</term><description>In front of tiles</description></item>
      /// </list></value>
      /// <remarks>Although at design time, priorities are generally specified with
      /// a wide range of numbers on the sprite instances, all we need to know at
      /// runtime is whether a frame is behind, interleaved or in front, so this
      /// priority value only has 3 possible values.</remarks>
      public int priority;
      /// <summary>
      /// Refers to the frame that is drawn "injected" into this layer
      /// </summary>
      public Frame frame;
      /// <summary>
      /// Speifies any color modulation that is applied to the frame when it is drawn
      /// </summary>
      public int color;
      /// <summary>
      /// Constructs a new frame provided with all the values
      /// </summary>
      /// <param name="x">Initial value for <see cref="x"/></param>
      /// <param name="y">Initial value for <see cref="y"/></param>
      /// <param name="priority">Initial value for <see cref="priority"/></param>
      /// <param name="frame">Initial value for <see cref="frame"/></param>
      /// <param name="color">Initial value for <see cref="color"/></param>
      /// <remarks>A frame may have its own color modulation value internally.
      /// If the color provided to this constructor (presumably from the sprite's
      /// color modulation value) and the frame's color are both specified (not the
      /// default of -1) then they are combined.</remarks>
      public InjectedFrame(int x, int y, int priority, Frame frame, int color)
      {
         this.x = x;
         this.y = y;
         this.frame = frame;
         this.priority = priority;
         if (color == -1)
            this.color = frame.Color;
         else //if (frame.Color == -1)
            this.color = color;
         /*else
            this.color = Microsoft.DirectX.Direct3D.ColorOperator.Modulate(ColorValue.FromArgb(frame.Color), ColorValue.FromArgb(color)).ToArgb();*/
      }
      #region IComparable Members

      /// <summary>
      /// Dertermine whether a specified frame should be drawn before or after this frame.
      /// </summary>
      /// <param name="obj">Another <see cref="InjectedFrame"/> object.</param>
      /// <returns>A signed integer that indicates the sequence of the frames
      /// <list type="table">
      /// <listheader><term>Return Value</term><item>Description</item></listheader>
      /// <item><term>Less than zero</term><description>This injected frame should be drawn
      /// before <paramref name="obj"/>.</description></item>
      /// <item><term>Zero</term><description>Frames overlap exactly, drawing order indeterminant.</description></item>
      /// <item><term>Greater than zero</term><description>This injected frame should be drawn
      /// after <paramref name="obj"/>.</description></item>
      /// </list></returns>
      /// <remarks><see cref="InjectFrames"/> relies on this interface to make sure that newly
      /// injected frames will be drawn in the right order. Frames higher up in the layer are
      /// drawn before frames that are lower down in the layer regardless of the order in which
      /// they are injected. Frames at the same vertical position will use the horizontal position
      /// to determine drawing order, frames on the left will draw first.</remarks>
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
   private bool? m_enableLighting;

   /// <summary>
   /// A "Category" or collection of all sprites contained by the layer.
   /// </summary>
   /// <remarks>"Static" sprites (sprite instances added at design time in the
   /// map editor) occupy the first portion of this collection. Any dynamically
   /// added sprites (added with functions like <see cref="SpriteBase.TileAddSprite"/>)
   /// are appended to the end. When a dynamically added sprite is deactivated, it is
   /// removed from this collection.</remarks>
   public SpriteCollection m_Sprites;
   private readonly System.Drawing.SizeF m_ScrollRate;
   private System.Drawing.Point[] m_CurrentPosition = new System.Drawing.Point[Project.MaxViews];
   private MapBase m_ParentMap;
   /// <summary>
   /// Provides access to all sprite categories as they relate to this layer.
   /// </summary>
   /// <remarks>The objects provided by this member will return a collection of all sprites
   /// in the requested category that are on this layer.</remarks>
   public LayerSpriteCategoriesBase m_SpriteCategories;
   protected readonly int m_nInjectStartIndex;
   protected readonly int m_nAppendStartIndex;
   #endregion

   protected LayerBase(Tileset Tileset, MapBase Parent, int nLeftBuffer, int nTopBuffer, int nRightBuffer, int nBottomBuffer,
      int nColumns, int nRows, int nVirtualColumns, int nVirtualRows, System.Drawing.Point Position,
      System.Drawing.SizeF ScrollRate, int nInjectStartIndex, int nAppendStartIndex)
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
      byte origView = Parent.CurrentViewIndex;
      for (byte v = 0; v < Project.MaxViews; v++)
      {
         Parent.CurrentViewIndex = v;
         this.Move(new Point(0, 0));
      }
      Parent.CurrentViewIndex = origView;
      this.m_nInjectStartIndex = nInjectStartIndex;
      this.m_nAppendStartIndex = nAppendStartIndex;
   }

   #region Abstract Members
   /// <summary>
   /// Retrieves or sets the value of a tile at the specified tile coordinate
   /// </summary>
   public abstract int this[int x, int y]
   {
      get;
      set;
   }
   protected abstract int[] GetTileFrame(int x, int y);
   /// <summary>
   /// Retrieves information about the tile at the specified tile coordinate
   /// </summary>
   /// <param name="x">Horizontal tile coordinate of the tile to retrieve</param>
   /// <param name="y">Vertical tile coordinate of the tile to retrieve</param>
   /// <returns>Object describing the tile at the specified position in the layer.</returns>
   /// <remarks>If the coordinate is beyond the edge of the layer's data, it wraps
   /// to the other side of the layer in order to support layers whose virtual size
   /// is larger than the data size.</remarks>
   public abstract TileBase GetTile(int x, int y);
   #endregion

   #region IEnumerable Members
   /// <summary>
   /// Enumerates active sprites on the layer.
   /// </summary>
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
   /// <param name="MapPosition">Position of the map. If one component is int.minValue,
   /// that axis is not affected.</param>
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

   /// <summary>
   /// Determines whether lighing effect are enabled for this layer.
   /// </summary>
   /// <remarks>By default, lighting effects are only enabled when sprites derived from LightSpriteBase
   /// are on it when first drawn. This value is ignored when graphics with normal maps are drawn, in
   /// which case lighting is implicitly enabled.</remarks>
   public bool EnableLighting
   {
      get
      {
         if (!m_enableLighting.HasValue)
         {
            return false;
            m_enableLighting = false;
            if (m_Sprites != null)
               for (int i = 0; i < m_Sprites.Count; i++)
                  if (m_Sprites[i] is LightSpriteBase)
                     m_enableLighting = true;
         }
         return m_enableLighting.Value;
      }
      set
      {
         m_enableLighting = value;
      }
   }

   /// <summary>
   /// Draw the layer according to the currently active view defined by <see cref="MapBase.CurrentView"/>.
   /// </summary>
   /// <remarks>Drawing the layer includes drawing of all the tiles and the sprites in the layer.</remarks>
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
      Display disp = m_ParentMap.Display;
      disp.Scissor(ViewRect);
      disp.currentView = m_ParentMap.CurrentViewIndex;
      disp.enableLighting = EnableLighting;

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

      int lastColor = 0;

      for (int y = nStartRow; y <= EndRow; y++)
      {
         if (Injected != null)
         {
            while ((((CurFrame = (InjectedFrame)Injected.Current).y < y * nTileHeight)) && (CurFrame.priority <= 0) ||
                   (CurFrame.priority < 0))
            {
               if (CurFrame.color != lastColor)
               {
                  disp.SetColor(CurFrame.color);
                  lastColor = CurFrame.color;
               }
               disp.DrawFrame(CurFrame.frame.GraphicSheetTexture,
                  CurFrame.frame.SourceRect, CurFrame.frame.Corners,
                  CurFrame.x + CurrentPosition.X + ViewRect.X,
                  CurFrame.y + CurrentPosition.Y + ViewRect.Y);
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
               if (f.Color != lastColor)
               {
                  disp.SetColor(f.Color);
                  lastColor = f.Color;
               }
               disp.DrawFrame(f.GraphicSheetTexture, f.SourceRect, f.Corners,
                  x * nTileWidth + CurrentPosition.X + ViewRect.X,
                  y * nTileHeight + CurrentPosition.Y + ViewRect.Y);
            }
         }
      }

      while (Injected != null)
      {
         CurFrame = (InjectedFrame)Injected.Current;
         if (CurFrame.color != lastColor)
         {
            disp.SetColor(CurFrame.color);
            lastColor = CurFrame.color;
         }
         disp.DrawFrame(CurFrame.frame.GraphicSheetTexture,
            CurFrame.frame.SourceRect, CurFrame.frame.Corners,
            CurFrame.x + CurrentPosition.X + ViewRect.X,
            CurFrame.y + CurrentPosition.Y + ViewRect.Y);
         if (!Injected.MoveNext())
         {
            Injected = null;
            break;
         }
      }
      disp.SetColor(-1);
   }

   /// <summary>
   /// Returns a rectangle within this layer that represents the currently visible portion.
   /// </summary>
   public Rectangle VisibleArea
   {
      get
      {
         return new Rectangle(new System.Drawing.Point(-CurrentPosition.X, -CurrentPosition.Y), m_ParentMap.CurrentView.Size);
      }
   }

   /// <summary>
   /// Determines if any part of the specified sprite is visible in the current view.
   /// </summary>
   /// <param name="sprite">Sprite to test.</param>
   /// <returns>True if any part of the specified sprite is visible in the map's
   /// <see cref="MapBase.CurrentView"/>.</returns>
   public bool IsSpriteVisible(SpriteBase sprite)
   {
      return sprite.isActive && sprite.GetBounds().IntersectsWith(VisibleArea);
   }

   /// <summary>
   /// Injects a series of <see cref="Frame"/> objects into this layer, to be interleaved
   /// with the tiles on the layer.
   /// </summary>
   /// <param name="x">Horizontal pixel coordinate of the location to inject the frames</param>
   /// <param name="y">Vertical pixel coordinate of the location to inject the frames</param>
   /// <param name="frames">Array of frames to be injected at the specified coordinate</param>
   /// <remarks>Often times only a single frame is injected, but sprites with compound frames
   /// may inject multiple frames at once.</remarks>
   public void InjectFrames(int x, int y, Frame[] frames)
   {
      InjectFrames(x, y, frames, -1);
   }

   /// <summary>
   /// Injects a series of <see cref="Frame"/> objects into this layer, to be interleaved
   /// with the tiles on the layer, specifying a color modulation value.
   /// </summary>
   /// <param name="x">Horizontal pixel coordinate of the location to inject the frames</param>
   /// <param name="y">Vertical pixel coordinate of the location to inject the frames</param>
   /// <param name="frames">Array of frames to be injected at the specified coordinate</param>
   /// <param name="color">Specifies how the color channels of the injected frames will be affected.
   /// If the frames include their own color modulation, they will be merged with this.</param>
   /// <remarks>Often times only a single frame is injected, but sprites with compound frames
   /// may inject multiple frames at once.</remarks>
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

   /// <summary>
   /// Injects a series of <see cref="Frame"/> objects into this layer, to be drawn behind the
   /// layer or in front of it, specifying a color modulation value.
   /// </summary>
   /// <param name="x">Horizontal pixel coordinate of the location to inject the frames</param>
   /// <param name="y">Vertical pixel coordinate of the location to inject the frames</param>
   /// <param name="frames">Array of frames to be injected at the specified coordinate</param>
   /// <param name="color">Specifies how the color channels of the injected frames will be affected.
   /// If the frames include their own color modulation, they will be merged with this.</param>
   /// <param name="priority"><list type="table">
   /// <listheader><term>Value</term><description>Drawing Order</description></listheader>
   /// <item><term>-1</term><description>Behind tiles</description></item>
   /// <item><term>1</term><description>In front of tiles</description></item>
   /// </list></param>
   /// <remarks>Often times only a single frame is injected, but sprites with compound frames
   /// may inject multiple frames at once.</remarks>
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

   /// <summary>
   /// Remove all injected frames from the layer.
   /// </summary>
   /// <remarks>This is performed after each time the layer is drawn to
   /// prepare for the next iteration.</remarks>
   public void ClearInjections()
   {
      if (m_InjectedFrames != null)
         m_InjectedFrames.Clear();
   }

   /// <summary>
   /// Inject frames into the layer to represent the current images and positions of
   /// the sprites contained in this layer for this loop iteration.
   /// </summary>
   public void InjectSprites()
   {
      for (int i = 0; (i < m_nInjectStartIndex) && (i < m_Sprites.Count); i++)
      {
         SpriteBase sprite = m_Sprites[i];
         if (IsSpriteVisible(sprite))
            AppendFrames(sprite.PixelX, sprite.PixelY, sprite.GetCurrentFramesetFrames(), sprite.color, -1);
      }
      for (int i = m_nInjectStartIndex; (i < m_nAppendStartIndex) && (i < m_Sprites.Count); i++)
      {
         SpriteBase sprite = m_Sprites[i];
         if (IsSpriteVisible(sprite))
            InjectFrames(sprite.PixelX, sprite.PixelY, sprite.GetCurrentFramesetFrames(), sprite.color);
      }
      for (int i = m_nAppendStartIndex; (i < m_Sprites.Count); i++)
      {
         SpriteBase sprite = m_Sprites[i];
         if (IsSpriteVisible(sprite))
            AppendFrames(sprite.PixelX, sprite.PixelY, sprite.GetCurrentFramesetFrames(), sprite.color, 1);
      }
   }

   /// <summary>
   /// Execute the rules of all active sprites on this layer.
   /// </summary>
   /// <remarks>After the rules are executed, the function checks to see if
   /// any dynamic sprites have been de-activated and removes them.
   /// This function can be overridden in the derived layer to customize how
   /// and when ProcessRules is called on each sprite.</remarks>
   public virtual void ProcessSprites()
   {
      foreach(SpriteBase sprite in m_Sprites)
         // Assuming it's more efficient to just set them all to false rather than
         // try and only enumerate the active ones.
         sprite.Processed = false;
      for(int i=0; i < m_Sprites.Count; i++)
         if (m_Sprites[i].isActive)
            m_Sprites[i].ProcessRules();
      m_Sprites.Clean();
   }

   /// <summary>
   /// Executes the rules for all the plans on this layer
   /// </summary>
   /// <remarks>See <see cref="ExecuteRules"/> for information on overriding this.</remarks>
   public virtual void ExecuteRulesInternal()
   {
      throw new NotImplementedException("ExecuteRules called on a layer without rules");
   }

   public virtual void ExecuteRules() { ExecuteRulesInternal(); }
  
   /// <summary>
   /// Retrieve the current mouse position
   /// </summary>
   /// <returns>Layer-relative coordinate representing the current position of the mouse.</returns>
   /// <remarks>This can be used to set a sprite's position at the mouse cursor and make it behave
   /// like a mouse pointer.
   /// <seealso cref="PlanBase.TransportToPoint"/></remarks>
   public Point GetMousePosition()
   {
      Point dispPos;
      dispPos = m_ParentMap.Display.PointToClient(GameForm.curMousePosition);
      dispPos.Offset(-CurrentPosition.X, -CurrentPosition.Y);
      return dispPos;
   }

   /// <summary>
   /// Scroll the currently active view on the map so the specified sprite is visible.
   /// </summary>
   /// <param name="sprite">Sprite to scroll into view</param>
   /// <param name="useScrollMargins">True to scroll the sprite so that it is also within
   /// the scroll margins, false to only scroll it so it is fully visible in the view.</param>
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

   /// <summary>
   /// Push the specified sprite into the currently active view.
   /// </summary>
   /// <param name="sprite">Sprite to be pushed</param>
   /// <param name="stayInScrollMargins">True to push the sprite until it's within the scroll
   /// margins, or false to only push it until it's fully visible in the view.</param>
   /// <remarks>This only affects the sprites intended velocity and does not actually move it.
   /// Solidity ot other factors could still impede the sprite's ability to stay in the view.</remarks>
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

   /// <summary>
   /// Determine the vertical offset of the top-most solid pixel within the specified rectangle.
   /// </summary>
   /// <param name="testArea">Rectangle to test</param>
   /// <param name="solid">Solidity definition defining tile shapes</param>
   /// <returns>Layer-relative vertical coordinate (y) of the top-most solid pixel contained
   /// in the rectangle, or <see cref="int.MinValue"/> if none exists.</returns>
   /// <remarks>This function (and solidity in general) only deals with solid boundaries of tiles.
   /// Therefore if the rectangle is fully embedded in a solid tile and is not crossing any solid
   /// boundaries, the return value will indicate that no solidity was found. This allows such
   /// features as tiles through which the player can jump upward but land down on solidly.</remarks>
   public int GetTopSolidPixel(Rectangle testArea, Solidity solid)
   {
      int topTile = (testArea.Top + m_Tileset.TileHeight) / m_Tileset.TileHeight - 1;
      int bottomTile = (int)((testArea.Top + testArea.Height - 1) / m_Tileset.TileHeight);
      int leftTile = (testArea.Left + m_Tileset.TileWidth) / m_Tileset.TileWidth - 1;
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

   /// <summary>
   /// Determine the vertical offset of the bottom-most solid pixel within the specified rectangle.
   /// </summary>
   /// <param name="testArea">Rectangle to test</param>
   /// <param name="solid">Solidity definition defining tile shapes</param>
   /// <returns>Layer-relative vertical coordinate (y) of the bottom-most solid pixel contained
   /// in the rectangle, or <see cref="int.MinValue"/> if none exists.</returns>
   /// <remarks>This function (and solidity in general) only deals with solid boundaries of tiles.
   /// Therefore if the rectangle is fully embedded in a solid tile and is not crossing any solid
   /// boundaries, the return value will indicate that no solidity was found. This allows such
   /// features as tiles through which the player can jump upward but land down on solidly.</remarks>
   public int GetBottomSolidPixel(Rectangle testArea, Solidity solid)
   {
      int topTile = (testArea.Top + m_Tileset.TileHeight) / m_Tileset.TileHeight - 1;
      int bottomTile = (int)((testArea.Top + testArea.Height - 1) / m_Tileset.TileHeight);
      int leftTile = (testArea.Left + m_Tileset.TileWidth) / m_Tileset.TileWidth - 1;
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

   /// <summary>
   /// Determine the horizontal offset of the left-most solid pixel within the specified rectangle.
   /// </summary>
   /// <param name="testArea">Rectangle to test</param>
   /// <param name="solid">Solidity definition defining tile shapes</param>
   /// <returns>Layer-relative horizontal coordinate (x) of the left-most solid pixel contained
   /// in the rectangle, or <see cref="int.MinValue"/> if none exists.</returns>
   /// <remarks>This function (and solidity in general) only deals with solid boundaries of tiles.
   /// Therefore if the rectangle is fully embedded in a solid tile and is not crossing any solid
   /// boundaries, the return value will indicate that no solidity was found. This allows such
   /// features as tiles through which the player can jump upward but land down on solidly.</remarks>
   public int GetLeftSolidPixel(Rectangle testArea, Solidity solid)
   {
      int topTile = (testArea.Top + m_Tileset.TileHeight) / m_Tileset.TileHeight - 1;
      int bottomTile = (int)((testArea.Top + testArea.Height - 1) / m_Tileset.TileHeight);
      int leftTile = (testArea.Left + m_Tileset.TileWidth) / m_Tileset.TileWidth - 1;
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

   /// <summary>
   /// Determine the horizontal offset of the right-most solid pixel within the specified rectangle.
   /// </summary>
   /// <param name="testArea">Rectangle to test</param>
   /// <param name="solid">Solidity definition defining tile shapes</param>
   /// <returns>Layer-relative horizontal coordinate (x) of the right-most solid pixel contained
   /// in the rectangle, or <see cref="int.MinValue"/> if none exists.</returns>
   /// <remarks>This function (and solidity in general) only deals with solid boundaries of tiles.
   /// Therefore if the rectangle is fully embedded in a solid tile and is not crossing any solid
   /// boundaries, the return value will indicate that no solidity was found. This allows such
   /// features as tiles through which the player can jump upward but land down on solidly.</remarks>
   public int GetRightSolidPixel(Rectangle testArea, Solidity solid)
   {
      int topTile = (testArea.Top + m_Tileset.TileHeight) / m_Tileset.TileHeight - 1;
      int bottomTile = (int)((testArea.Top + testArea.Height - 1) / m_Tileset.TileHeight);
      int leftTile = (testArea.Left + m_Tileset.TileWidth) / m_Tileset.TileWidth - 1;
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

/// <summary>
/// Represents a layer where each tile is represented as a 32-bit integer.
/// </summary>
[Serializable()]
public abstract partial class IntLayer : LayerBase
{
   private int[,] m_Tiles;

   public IntLayer(Tileset Tileset, MapBase Parent, int nLeftBuffer, int nTopBuffer, int nRightBuffer,
      int nBottomBuffer, int nColumns, int nRows, int nVirtualColumns, int nVirtualRows,
      System.Drawing.Point Position, System.Drawing.SizeF ScrollRate,
      int nInjectStartIndex, int nAppendStartIndex, string Name) : 
      base(Tileset, Parent, nLeftBuffer, nTopBuffer, nRightBuffer,
      nBottomBuffer, nColumns, nRows, nVirtualColumns, nVirtualRows, Position,
      ScrollRate, nInjectStartIndex, nAppendStartIndex)
   {
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(Parent.GetType());
      if (Name != null)
         m_Tiles = (int[,])(resources.GetObject(Name));
      else
         m_Tiles = new int[nColumns, nRows];
   }

   /// <summary>
   /// Retrieves or sets the value of a tile at the specified tile coordinate
   /// </summary>
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

   protected override int[] GetTileFrame(int x, int y)
   {
      return m_Tileset[m_Tiles[x % m_nColumns, y % m_nRows]].CurrentFrame;
   }

   /// <summary>
   /// See <see cref="LayerBase.GetTile"/>.
   /// </summary>
   public override TileBase GetTile(int x, int y)
   {
      return m_Tileset[m_Tiles[x % m_nColumns,y % m_nRows]];
   }
}

/// <summary>
/// Represents a layer where each tile is represented as a 16-bit integer.
/// </summary>
[Serializable()]
public abstract partial class ShortLayer : LayerBase
{
   private short[,] m_Tiles;

   public ShortLayer(Tileset Tileset, MapBase Parent, int nLeftBuffer, int nTopBuffer, int nRightBuffer,
      int nBottomBuffer, int nColumns, int nRows, int nVirtualColumns, int nVirtualRows, System.Drawing.Point Position,
      System.Drawing.SizeF ScrollRate, int nInjectStartIndex, int nAppendStartIndex, string Name) : 
      base(Tileset, Parent, nLeftBuffer, nTopBuffer, nRightBuffer,
      nBottomBuffer, nColumns, nRows, nVirtualColumns, nVirtualRows, Position,
      ScrollRate, nInjectStartIndex, nAppendStartIndex)
   {
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(Parent.GetType());
      if (Name != null)
         m_Tiles = (short[,])(resources.GetObject(Name));
      else
         m_Tiles = new short[nColumns, nRows];
   }

   /// <summary>
   /// Retrieves or sets the value of a tile at the specified tile coordinate
   /// </summary>
   /// <value>A number from 0 to 32767</value>
   /// <remarks>Although this member accepts an integer as the value (because that
   /// is the type required by the base class), a layer of this type can only use
   /// 16-bit values.</remarks>
   public override int this[int x, int y]
   {
      get
      {
         return (int)(m_Tiles[x % m_nColumns, y % m_nRows]);
      }
      set
      {
         m_Tiles[x % m_nColumns, y % m_nRows] = (short)value;
      }
   }

   protected override int[] GetTileFrame(int x, int y)
   {
      return m_Tileset[m_Tiles[x % m_nColumns, y % m_nRows]].CurrentFrame;
   }

   /// <summary>
   /// See <see cref="LayerBase.GetTile"/>.
   /// </summary>
   public override TileBase GetTile(int x, int y)
   {
      return m_Tileset[m_Tiles[x % m_nColumns, y % m_nRows]];
   }
}

/// <summary>
/// Represents a layer where each tile is represented as a single byte.
/// </summary>
[Serializable()]
public abstract partial class ByteLayer : LayerBase
{
   private byte[,] m_Tiles;

   public ByteLayer(Tileset Tileset, MapBase Parent, int nLeftBuffer, int nTopBuffer, int nRightBuffer,
      int nBottomBuffer, int nColumns, int nRows, int nVirtualColumns, int nVirtualRows, System.Drawing.Point Position,
      System.Drawing.SizeF ScrollRate, int nInjectStartIndex, int nAppendStartIndex, string Name) : 
      base(Tileset, Parent, nLeftBuffer, nTopBuffer, nRightBuffer,
      nBottomBuffer, nColumns, nRows, nVirtualColumns, nVirtualRows, Position,
      ScrollRate, nInjectStartIndex, nAppendStartIndex)
   {
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(Parent.GetType());
      if (Name != null)
         m_Tiles = (byte[,])(resources.GetObject(Name));
      else
         m_Tiles = new byte[nColumns, nRows];
   }

   /// <summary>
   /// Retrieves or sets the value of a tile at the specified tile coordinate
   /// </summary>
   /// <value>A number from 0 to 255</value>
   /// <remarks>Although this member accepts an integer as the value (because that
   /// is the type required by the base class), a layer of this type can only use
   /// 8-bit values.</remarks>
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

   protected override int[] GetTileFrame(int x, int y)
   {
      return m_Tileset[m_Tiles[x % m_nColumns, y % m_nRows]].CurrentFrame;
   }

   /// <summary>
   /// See <see cref="LayerBase.GetTile"/>.
   /// </summary>
   public override TileBase GetTile(int x, int y)
   {
      return m_Tileset[m_Tiles[x % m_nColumns, y % m_nRows]];
   }
}