using System;
using System.ComponentModel;

/// <summary>
/// Base class for "plans", which consist of map coordinates and rules
/// </summary>
public abstract class PlanBase : System.Collections.IEnumerable
{
	protected PlanBase()
	{
	}

   public struct Coordinate
   {
      public int x;
      public int y;
      public int weight;
      public Coordinate(int x, int y, int weight)
      {
         this.x = x;
         this.y = y;
         this.weight = weight;
      }
      public static implicit operator System.Drawing.Point(Coordinate value)
      {
         return new System.Drawing.Point(value.x, value.y);
      }
   }

   public virtual System.Drawing.Rectangle PlanRectangle
   {
      get
      {
         return System.Drawing.Rectangle.Empty;
      }
   }

   [Description("Returns true if the specified sprite is touching this plan's rectangle")]
   public bool IsSpriteTouching(SpriteBase sprite)
   {
      System.Drawing.Rectangle spriteRect = new System.Drawing.Rectangle(sprite.PixelX, sprite.PixelY, sprite.SolidWidth, sprite.SolidHeight);
      System.Drawing.Rectangle targetRect = PlanRectangle;
      if (!spriteRect.IntersectsWith(System.Drawing.Rectangle.Inflate(targetRect,2,2)))
         return false;
      if (spriteRect.IntersectsWith(System.Drawing.Rectangle.Inflate(targetRect,2,0)) ||
            spriteRect.IntersectsWith(System.Drawing.Rectangle.Inflate(targetRect,0,2)))
         return true;
      else
         return false;
   }

   [Description("Returns true if the specified sprite was touching this plan's rectangle in the previous frame")]
   public bool WasSpriteTouching(SpriteBase sprite)
   {
      System.Drawing.Rectangle spriteRect = new System.Drawing.Rectangle(sprite.OldPixelX, sprite.OldPixelY, sprite.SolidWidth, sprite.SolidHeight);
      System.Drawing.Rectangle targetRect = PlanRectangle;
      if (!spriteRect.IntersectsWith(System.Drawing.Rectangle.Inflate(targetRect,2,2)))
         return false;
      if (spriteRect.IntersectsWith(System.Drawing.Rectangle.Inflate(targetRect,2,0)) ||
         spriteRect.IntersectsWith(System.Drawing.Rectangle.Inflate(targetRect,0,2)))
         return true;
      else
         return false;
   }

   [Description("Moves the specified sprite to the specified coordinate")]
   public void Transport(SpriteBase sprite, System.Drawing.Point target)
   {
      sprite.oldX = sprite.x;
      sprite.oldY = sprite.y;
      sprite.x = target.X;
      sprite.y = target.Y;
   }

   protected virtual Coordinate[] Coordinates
   {
      get
      {
         return null;
      }
   }

   public Coordinate this[int index]
   {
      get
      {
         return Coordinates[index];
      }
   }

   public int Count
   {
      get
      {
         if (Coordinates == null)
            return 0;
         return Coordinates.Length;
      }
   }

   public virtual void ExecuteRules()
   {
      throw new NotImplementedException("Attempted to execute rules on plan " + this.GetType().Name + " without any rules");
   }

   #region IEnumerable Members

   public System.Collections.IEnumerator GetEnumerator()
   {
      if (Coordinates == null)
         new System.Collections.ArrayList().GetEnumerator();
      return Coordinates.GetEnumerator();
   }

   #endregion
}