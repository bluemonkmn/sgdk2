using System;
using System.ComponentModel;
using System.Drawing;

/// <summary>
/// Base class for "plans", which consist of map coordinates and rules
/// </summary>
public abstract class PlanBase : System.Collections.IEnumerable
{
   public static int SharedTemp1;

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

   [Description("Alter the velocity of the specified sprite to go toward a coordinate associated with the current plan.  CoordinateIndex indicates the which coordinate in the plan to head toward, and Force is how hard to push the sprite in tenths of a pixel per frame per frame")]
   public void PushSpriteTowardCoordinate(SpriteBase Sprite, int CoordinateIndex, int Force)
   {
      PushSpriteTowardPoint(Sprite, this[CoordinateIndex], Force);
   }

   [Description("Alter the velocity of the specified sprite to go toward a specified location.  Force is how hard to push the sprite in tenths of a pixel per frame per frame")]
   public void PushSpriteTowardPoint(SpriteBase Sprite, Point Target, int Force)
   {
      double dx = Target.X - Sprite.PixelX;
      double dy = Target.Y - Sprite.PixelY;

      // Normalize target vector to magnitude of Force parameter
      double dist = Math.Sqrt(dx * dx + dy * dy);
      dx = dx * Force / dist / 10;
      dy = dy * Force / dist / 10;

      // Push sprite
      Sprite.dx += dx;
      Sprite.dy += dy;
   }

   [Description("Determine whether the sprite is within the TargetDistance of the specified coordinate.  If so, return the next CoordinateIndex, otherwise return the current CoordinateIndex.")]
   public int CheckNextCoordinate(SpriteBase Sprite, int CoordinateIndex, int TargetDistance)
   {
      int dx = this[CoordinateIndex].x - Sprite.PixelX;
      int dy = this[CoordinateIndex].y - Sprite.PixelY;
      if (Math.Sqrt(dx * dx + dy * dy) < TargetDistance)
         return (CoordinateIndex + 1) % Count;
      return CoordinateIndex;
   }

   [Description("Set the velocity of the specified sprite to zero")]
   public void StopSprite(SpriteBase Sprite)
   {
      Sprite.dx = Sprite.dy = 0;
   }

   [Description("Determine whether the specified sprite's collision mask is overlapping part of any sprite in the specified category. Return the index of the sprite within the category if a collision is occurring, otherwise return -1.")]
   public int TestCollisionMask(SpriteBase SourceSprite, SpriteCollection Targets)
   {
      if (!SourceSprite.isActive)
         return -1;
      CollisionMask sourceMask = SourceSprite.CurrentState.GetMask(SourceSprite.frame);
      if (sourceMask == null)
         sourceMask = CollisionMask.GetRectangularMask(new Size(SourceSprite.SolidWidth, SourceSprite.SolidHeight));
      for (int idx = 0; idx < Targets.Count; idx++)
      {
         SpriteBase TargetSprite = Targets[idx];
         if (TargetSprite.isActive)
         {
            CollisionMask targetMask = TargetSprite.CurrentState.GetMask(TargetSprite.frame);
            if (targetMask == null)
               targetMask = CollisionMask.GetRectangularMask(new Size(TargetSprite.SolidWidth, TargetSprite.SolidHeight));
            if (sourceMask.TestCollisionWith(targetMask, TargetSprite.PixelX - SourceSprite.PixelX, TargetSprite.PixelY - SourceSprite.PixelY))
               return idx;
         }         
      }
      return -1;
   }

   [Description("Determine whether the solidity rectangle of the specified sprite overlaps that of any sprite in the specified category. Return the index of the sprite within the category if a collision is occurring, otherwise return -1.")]
   public int TestCollisionRect(SpriteBase SourceSprite, SpriteCollection Targets)
   {
      if (!SourceSprite.isActive)
         return -1;
      for (int idx = 0; idx < Targets.Count; idx++)
      {
         SpriteBase TargetSprite = Targets[idx];
         int x1 = SourceSprite.PixelX;
         int w1 = SourceSprite.SolidWidth;
         int x2 = TargetSprite.PixelX;
         int w2 = TargetSprite.SolidWidth;
         int y1 = SourceSprite.PixelY;
         int h1 = SourceSprite.SolidHeight;
         int y2 = TargetSprite.PixelY;
         int h2 = TargetSprite.SolidHeight;

         if ((x1+w1 > x2) && (x2+w2 > x1) && (y1+h1 > y2) && (y2+h2 > y1))
            return idx;
      }
      return -1;
   }

   [Description("Deactivate a sprite within a category given the sprite's index within the category")]
   public void DeactivateCategorySprite(SpriteCollection Category, int Index)
   {
      Category[Index].isActive = false;
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