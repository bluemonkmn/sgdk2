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
   Point m_CurrentPosition;
   TileCache m_TileCache;
   Frameset m_Frameset;
   System.Collections.ArrayList m_InjectedFrames = null;
   #endregion

   protected LayerBase()
   {
   }

   #region Abstract Members
   /// <summary>
   /// Retrieves the value of a tile at the specified tile coordinate
   /// </summary>
   public abstract int this[int x, int y]
   {
      get;
   }

   /// <summary>
   /// Retrieves the collection of all sprites that can exist on this layer
   /// </summary>
   public abstract SpriteCollection Sprites
   {
      get;
   }

   protected abstract int LeftBuffer
   {
      get;
   }

   protected abstract int TopBuffer
   {
      get;
   }

   protected abstract int RightBuffer
   {
      get;
   }

   protected abstract int BottomBuffer
   {
      get;
   }
   #endregion

   #region IEnumerable Members
   public System.Collections.IEnumerator GetEnumerator()
   {
      return new ActiveSpriteEnumerator(Sprites);
   }
   #endregion
}