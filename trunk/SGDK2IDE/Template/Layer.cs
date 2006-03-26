using System;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

/// <summary>
/// Summary description for LayerBase.
/// </summary>
public abstract class LayerBase : System.Collections.IEnumerable
{
   #region Embedded Classes
   public class ActiveSpriteEnumerator : System.Collections.IEnumerator
   {
      private System.Collections.IEnumerator SpriteEnumerator;

      public ActiveSpriteEnumerator(SpriteCollection sprites)
      {
         SpriteEnumerator = sprites.GetEnumerator();
      }

      #region IEnumerator Members

      public void Reset()
      {
         SpriteEnumerator.Reset();
      }

      public object Current
      {
         get
         {
            return SpriteEnumerator.Current;
         }
      }

      public bool MoveNext()
      {
         bool result;
         while ((result = SpriteEnumerator.MoveNext()) && (!((SpriteBase)Current).isActive))
            ;
         return result;
      }

      #endregion
   }

   private class InjectedFrame : IComparable
   {
      public int x;
      public int y;
      public Frame frame;
      public InjectedFrame(int x, int y, Frame frame)
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
   protected readonly Tileset m_Tileset;
   private Frameset m_Frameset;
   private System.Collections.ArrayList m_InjectedFrames = null;
   
   private readonly int m_nLeftBuffer;
   private readonly int m_nTopBuffer;
   private readonly int m_nRightBuffer;
   private readonly int m_nBottomBuffer;
   private readonly int m_nColumns;
   private readonly int m_nRows;
   private System.Drawing.Point m_AbsolutePosition;
   private readonly SpriteCollection m_Sprites;
   private readonly System.Drawing.SizeF m_ScrollRate;
   private System.Drawing.Point m_CurrentPosition;
   private Display m_Display;
   #endregion

   protected LayerBase(Tileset Tileset, Display Disp, int nLeftBuffer, int nTopBuffer, int nRightBuffer, int nBottomBuffer,
      int nColumns, int nRows, System.Drawing.Point Position, SpriteCollection Sprites,
      System.Drawing.SizeF ScrollRate)
   {
      this.m_Tileset = Tileset;
      this.m_Frameset = Tileset.CreateFrameset(Disp);
      this.m_nLeftBuffer = nLeftBuffer;
      this.m_nTopBuffer = nTopBuffer;
      this.m_nRightBuffer = nRightBuffer;
      this.m_nBottomBuffer = nBottomBuffer;
      this.m_nColumns = nColumns;
      this.m_nRows = nRows;
      this.m_AbsolutePosition = Position;
      this.m_Sprites = Sprites;
      this.m_ScrollRate = ScrollRate;
      this.m_Display = Disp;
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
   #endregion

   #region IEnumerable Members
   public System.Collections.IEnumerator GetEnumerator()
   {
      return new ActiveSpriteEnumerator(m_Sprites);
   }
   #endregion

   #region Properties
   /// <summary>
   /// Get the number of columns of tiles in the layer
   /// </summary>
   public int Columns
   {
      get
      {
         return m_nColumns;
      }
   }
   /// <summary>
   /// Get the number of rows of tiles in the layer
   /// </summary>
   public int Rows
   {
      get
      {
         return m_nRows;
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
         return m_CurrentPosition;
      }
      set
      {
         m_CurrentPosition = value;
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
      m_CurrentPosition = new Point(m_AbsolutePosition.X + (int)(x * m_ScrollRate.Width), m_AbsolutePosition.Y + (int)(y * m_ScrollRate.Height));
   }

   public void Draw(Rectangle ViewRect, Sprite spr)
   {
      int nTileWidth = m_Tileset.TileWidth;
      int nTileHeight = m_Tileset.TileHeight;

      int nStartCol = (-m_nLeftBuffer - m_CurrentPosition.X) / nTileWidth;
      if (nStartCol < 0)
         nStartCol = 0;
      int nStartRow = (-m_nTopBuffer - m_CurrentPosition.Y) / nTileHeight;
      if (nStartRow < 0)
         nStartRow = 0;

      int EndCol = (ViewRect.Width - 1 + m_nRightBuffer - m_CurrentPosition.X) / nTileWidth;
      if (EndCol >= Columns)
         EndCol = Columns - 1;
      int EndRow = (ViewRect.Height - 1 + m_nBottomBuffer - m_CurrentPosition.Y) / nTileHeight;
      if (EndRow >= Rows)
         EndRow = Rows - 1;

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
               spr.Transform = Matrix.Multiply(CurFrame.frame.Transform, Matrix.Translation(
                  CurFrame.x + m_CurrentPosition.X + ViewRect.X,
                  CurFrame.y + m_CurrentPosition.Y + ViewRect.Y, 0));
               spr.Draw(CurFrame.frame.GraphicSheetTexture.Texture, CurFrame.frame.SourceRect,
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
            int[] SubFrames = GetTileFrame(x,y);
            for (int nFrame = 0; nFrame < SubFrames.Length; nFrame++)
            {
               Frame f = m_Frameset[SubFrames[nFrame]];
               spr.Transform = Matrix.Multiply(f.Transform, Matrix.Translation(
                  x * nTileWidth + m_CurrentPosition.X + ViewRect.X,
                  y * nTileHeight + m_CurrentPosition.Y + ViewRect.Y, 0));
               spr.Draw(f.GraphicSheetTexture.Texture, f.SourceRect, Vector3.Empty, Vector3.Empty, -1);
            }
         }
      }

      while (Injected != null)
      {
         CurFrame = (InjectedFrame)Injected.Current;
         spr.Transform = Matrix.Multiply(CurFrame.frame.Transform, Matrix.Translation(
            CurFrame.x + m_CurrentPosition.X + ViewRect.X,
            CurFrame.y + m_CurrentPosition.Y + ViewRect.Y, 0));
         spr.Draw(CurFrame.frame.GraphicSheetTexture.Texture, CurFrame.frame.SourceRect,
            Vector3.Empty, Vector3.Empty, -1);
         if (!Injected.MoveNext())
         {
            Injected = null;
            break;
         }
      }
   }

   public void InjectFrame(int x, int y, Frame frame)
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
      short nWidth = m_Tileset.TileWidth;
      short nHeight = m_Tileset.TileHeight;
      int[] SubFrames = m_Tileset[nTileValue].CurrentFrame;
      for (int nFrame = 0; nFrame < SubFrames.Length; nFrame++)
         InjectFrame(nCol * nWidth, nRow * nHeight, m_Frameset[SubFrames[nFrame]]);
   }

   public void ClearInjections()
   {
      m_InjectedFrames = null;
   }
   #endregion
}

public class IntLayer : LayerBase
{
   public int[,] m_Tiles;

   public IntLayer(Tileset Tileset, Display Disp, int nLeftBuffer, int nTopBuffer, int nRightBuffer,
      int nBottomBuffer, int nColumns, int nRows, System.Drawing.Point Position,
      SpriteCollection Sprites, System.Drawing.SizeF ScrollRate, string Name) : 
      base(Tileset, Disp, nLeftBuffer, nTopBuffer, nRightBuffer,
      nBottomBuffer, nColumns, nRows, Position, Sprites, ScrollRate)
   {
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MapLevel1));
      m_Tiles = (int[,])(resources.GetObject(Name));
   }

   public override int this[int x, int y]
   {
      get
      {
         return m_Tiles[x,y];
      }
      set
      {
         m_Tiles[x,y] = value;
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

public class ShortLayer : LayerBase
{
   public short[,] m_Tiles;

   public ShortLayer(Tileset Tileset, Display Disp, int nLeftBuffer, int nTopBuffer, int nRightBuffer,
      int nBottomBuffer, int nColumns, int nRows, System.Drawing.Point Position,
      SpriteCollection Sprites, System.Drawing.SizeF ScrollRate, string Name) : 
      base(Tileset, Disp, nLeftBuffer, nTopBuffer, nRightBuffer,
      nBottomBuffer, nColumns, nRows, Position, Sprites, ScrollRate)
   {
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MapLevel1));
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

public class ByteLayer : LayerBase
{
   public byte[,] m_Tiles;

   public ByteLayer(Tileset Tileset, Display Disp, int nLeftBuffer, int nTopBuffer, int nRightBuffer,
      int nBottomBuffer, int nColumns, int nRows, System.Drawing.Point Position,
      SpriteCollection Sprites, System.Drawing.SizeF ScrollRate, string Name) : 
      base(Tileset, Disp, nLeftBuffer, nTopBuffer, nRightBuffer,
      nBottomBuffer, nColumns, nRows, Position, Sprites, ScrollRate)
   {
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MapLevel1));
      m_Tiles = (byte[,])(resources.GetObject(Name));
   }

   public override int this[int x, int y]
   {
      get
      {
         return (int)(m_Tiles[x,y]);
      }
      set
      {
         m_Tiles[x,y] = (byte)value;
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