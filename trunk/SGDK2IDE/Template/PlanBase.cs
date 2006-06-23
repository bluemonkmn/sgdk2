using System;
using System.ComponentModel;
using System.Drawing;

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
      public static implicit operator Point(Coordinate value)
      {
         return new Point(value.x, value.y);
      }
   }

   public abstract LayerBase ParentLayer
   {
      get;
   }

   public virtual Rectangle PlanRectangle
   {
      get
      {
         return Rectangle.Empty;
      }
   }

   [Description("Returns true if the specified sprite is touching this plan's rectangle")]
   public bool IsSpriteTouching(SpriteBase sprite)
   {
      Rectangle spriteRect = new Rectangle(sprite.PixelX, sprite.PixelY, sprite.SolidWidth, sprite.SolidHeight);
      Rectangle targetRect = PlanRectangle;
      if (!spriteRect.IntersectsWith(Rectangle.Inflate(targetRect,2,2)))
         return false;
      if (spriteRect.IntersectsWith(Rectangle.Inflate(targetRect,2,0)) ||
            spriteRect.IntersectsWith(Rectangle.Inflate(targetRect,0,2)))
         return true;
      else
         return false;
   }

   [Description("Returns true if the specified sprite was touching this plan's rectangle in the previous frame")]
   public bool WasSpriteTouching(SpriteBase sprite)
   {
      Rectangle spriteRect = new Rectangle(sprite.OldPixelX, sprite.OldPixelY, sprite.SolidWidth, sprite.SolidHeight);
      Rectangle targetRect = PlanRectangle;
      if (!spriteRect.IntersectsWith(Rectangle.Inflate(targetRect,2,2)))
         return false;
      if (spriteRect.IntersectsWith(Rectangle.Inflate(targetRect,2,0)) ||
         spriteRect.IntersectsWith(Rectangle.Inflate(targetRect,0,2)))
         return true;
      else
         return false;
   }

   [Description("Moves the specified sprite to the specified coordinate")]
   public void Transport(SpriteBase sprite, Point target)
   {
      sprite.oldX = sprite.x;
      sprite.oldY = sprite.y;
      sprite.x = target.X;
      sprite.y = target.Y;
   }

   [Description("Write a string to the debug output without moving to the next line")]
   public void LogDebugLabel(string Label)
   {
      Project.GameWindow.debugText.Write(Label);
   }

   [Description("Write a number to the debug output and move to the next line")]
   public void LogDebugValue(int DebugValue)
   {
      Project.GameWindow.debugText.WriteLine(DebugValue.ToString());
   }

   [Description("Draw the debug log on the display and reset it for the next frame")]
   public void DrawDebugLog()
   {
      Project.GameWindow.OutputDebugInfo();
   }

   [Description("Returns true if the specified key is currently pressed")]
   public bool IsKeyPressed(Microsoft.DirectX.DirectInput.Key key)
   {
      return Project.GameWindow.KeyboardState[key];
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

   [Description("Scroll all layers on this plan's layer's map so that the specified sprite is within the scoll margins of the map")]
   public void ScrollSpriteIntoView(SpriteBase sprite)
   {
      ParentLayer.ScrollSpriteIntoView(sprite);
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