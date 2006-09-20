using System;
using System.ComponentModel;
using System.Drawing;
using Microsoft.DirectX;

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

   [Description("Write a string to the debug output without moving to the next line"),
   System.Diagnostics.Conditional("DEBUG")]
   public void LogDebugLabel(string Label)
   {
      Project.GameWindow.debugText.Write(Label);
   }

   [Description("Write a number to the debug output and move to the next line"),
   System.Diagnostics.Conditional("DEBUG")]
   public void LogDebugValue(int DebugValue)
   {
      Project.GameWindow.debugText.WriteLine(DebugValue.ToString());
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
      if (dist > 0)
      {
         dx = dx * Force / dist / 10;
         dy = dy * Force / dist / 10;

         // Push sprite
         Sprite.dx += dx;
         Sprite.dy += dy;
      }
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
         if (TargetSprite == SourceSprite)
            continue;
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
         if (TargetSprite == SourceSprite)
            continue;
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

   [Description("Determines if the specified sprite instace is active.")]
   public bool IsSpriteActive(SpriteBase Sprite)
   {
      return Sprite.isActive;
   }

   [Description("Sets a different map as the one to be drawn on the game display.  If UnloadCurrent is true, the current map will be unloaded first (which causes it to be recreated/reset when returning to it).")]
   public void SwitchToMap([Editor("MapType", "UITypeEditor")] Type MapType, bool UnloadCurrent)
   {
      if (UnloadCurrent)
         Project.GameWindow.UnloadMap(Project.GameWindow.CurrentMap.GetType());
      Project.GameWindow.CurrentMap = Project.GameWindow.GetMap(MapType);
   }

   [Description("Unloads the specified map, which will force it to be recreated/reset next time it is used.")]
   public void UnloadMap([Editor("MapType", "UITypeEditor")] Type MapType)
   {
      Project.GameWindow.UnloadMap(MapType);
   }

   #region Inventory / Overlay
   public enum DrawStyle
   {
      ClipRightToCounter,
      StretchRightToCounter,
      RepeatRightToCounter,
      ClipTopToCounter,
      StretchTopToCounter,
      RepeatUpToCounter
   }

   [Description("Draw the specified tile from the layer's tileset in this plan's rectangle according to the specified counter value")]
   public void DrawCounterAsTile(int TileIndex, Counter counter, DrawStyle style)
   {
      System.Diagnostics.Debug.Assert(!PlanRectangle.IsEmpty, "DrawCounterAsTile was called on a plan that does not have a rectangle defined");
      if (PlanRectangle.IsEmpty)
         return;
      if (counter.CurrentValue == 0)
         return;
      MapBase map = ParentLayer.ParentMap;
      Display disp = map.Display;
      Tileset ts = ParentLayer.Tileset;
      Frameset fr = ts.GetFrameset(ParentLayer.ParentMap.Display);

      switch(style)
      {
         case DrawStyle.ClipRightToCounter:
            disp.Device.RenderState.ScissorTestEnable = true;
            disp.Device.ScissorRectangle = new Rectangle(
               PlanRectangle.X + ParentLayer.CurrentPosition.X + ParentLayer.ParentMap.View.X,
               PlanRectangle.Y + ParentLayer.CurrentPosition.Y + ParentLayer.ParentMap.View.Y,
               PlanRectangle.Width * counter.CurrentValue / counter.MaxValue,
               PlanRectangle.Height);
            foreach(int frameIndex in ts[TileIndex].CurrentFrame)
            {
               disp.Sprite.Transform = Matrix.Multiply(Matrix.Multiply(
                  fr[frameIndex].Transform,
                  Matrix.Scaling(PlanRectangle.Width / (float)ts.TileWidth, 1, 1)),
                  Matrix.Translation(
                  PlanRectangle.X + ParentLayer.CurrentPosition.X + ParentLayer.ParentMap.View.X,
                  PlanRectangle.Y + ParentLayer.CurrentPosition.Y + ParentLayer.ParentMap.View.Y,
                  0));
               disp.Sprite.Draw(fr[frameIndex].GraphicSheetTexture.Texture, fr[frameIndex].SourceRect,
                  Vector3.Empty, Vector3.Empty, -1);
            }
            break;
         case DrawStyle.StretchRightToCounter:
            disp.Device.RenderState.ScissorTestEnable = false;
            foreach(int frameIndex in ts[TileIndex].CurrentFrame)
            {
               disp.Sprite.Transform = Matrix.Multiply(Matrix.Multiply(
                  fr[frameIndex].Transform,
                  Matrix.Scaling(PlanRectangle.Width * counter.CurrentValue / (float)counter.MaxValue / (float)ts.TileWidth, 1, 1)),
                  Matrix.Translation(
                  PlanRectangle.X + ParentLayer.CurrentPosition.X + ParentLayer.ParentMap.View.X,
                  PlanRectangle.Y + ParentLayer.CurrentPosition.Y + ParentLayer.ParentMap.View.Y,
                  0));
               disp.Sprite.Draw(fr[frameIndex].GraphicSheetTexture.Texture, fr[frameIndex].SourceRect,
                  Vector3.Empty, Vector3.Empty, -1);
            }
            break;
         case DrawStyle.RepeatRightToCounter:
            foreach(int frameIndex in ts[TileIndex].CurrentFrame)
            {
               int FillWidth = PlanRectangle.Width * counter.CurrentValue / counter.MaxValue;
               disp.Device.RenderState.ScissorTestEnable = true;
               disp.Device.ScissorRectangle = new Rectangle(
                  PlanRectangle.X + ParentLayer.CurrentPosition.X + ParentLayer.ParentMap.View.X,
                  PlanRectangle.Y + ParentLayer.CurrentPosition.Y + ParentLayer.ParentMap.View.Y,
                  FillWidth, PlanRectangle.Height);
               for (int repeat=0; repeat < (int)Math.Ceiling(FillWidth / (float)ts.TileWidth); repeat++)
               {
                  disp.Sprite.Transform = Matrix.Multiply(
                     fr[frameIndex].Transform,
                     Matrix.Translation(
                     PlanRectangle.X + ParentLayer.CurrentPosition.X + ParentLayer.ParentMap.View.X + repeat * ts.TileWidth,
                     PlanRectangle.Y + ParentLayer.CurrentPosition.Y + ParentLayer.ParentMap.View.Y,
                     0));
                  disp.Sprite.Draw(fr[frameIndex].GraphicSheetTexture.Texture, fr[frameIndex].SourceRect,
                     Vector3.Empty, Vector3.Empty, -1);
               }
            }
            break;
         case DrawStyle.ClipTopToCounter:
         {
            disp.Device.RenderState.ScissorTestEnable = true;
            int FillHeight = PlanRectangle.Height * counter.CurrentValue / counter.MaxValue;
            disp.Device.ScissorRectangle = new Rectangle(
               PlanRectangle.X + ParentLayer.CurrentPosition.X + ParentLayer.ParentMap.View.X,
               PlanRectangle.Y + ParentLayer.CurrentPosition.Y + ParentLayer.ParentMap.View.Y +
               PlanRectangle.Height - FillHeight, PlanRectangle.Width, FillHeight);
            foreach(int frameIndex in ts[TileIndex].CurrentFrame)
            {
               disp.Sprite.Transform = Matrix.Multiply(Matrix.Multiply(
                  fr[frameIndex].Transform,
                  Matrix.Scaling(1, PlanRectangle.Height / (float)ts.TileHeight, 1)),
                  Matrix.Translation(
                  PlanRectangle.X + ParentLayer.CurrentPosition.X + ParentLayer.ParentMap.View.X,
                  PlanRectangle.Y + ParentLayer.CurrentPosition.Y + ParentLayer.ParentMap.View.Y,
                  0));
               disp.Sprite.Draw(fr[frameIndex].GraphicSheetTexture.Texture, fr[frameIndex].SourceRect,
                  Vector3.Empty, Vector3.Empty, -1);
            }
         }
            break;
         case DrawStyle.StretchTopToCounter:
         {
            disp.Device.RenderState.ScissorTestEnable = false;
            int FillHeight = PlanRectangle.Height * counter.CurrentValue / counter.MaxValue;
            foreach(int frameIndex in ts[TileIndex].CurrentFrame)
            {
               disp.Sprite.Transform = Matrix.Multiply(Matrix.Multiply(
                  fr[frameIndex].Transform,
                  Matrix.Scaling(1, FillHeight / (float)ts.TileHeight, 1)),
                  Matrix.Translation(
                  PlanRectangle.X + ParentLayer.CurrentPosition.X + ParentLayer.ParentMap.View.X,
                  PlanRectangle.Y + ParentLayer.CurrentPosition.Y + ParentLayer.ParentMap.View.Y + PlanRectangle.Height - FillHeight,
                  0));
               disp.Sprite.Draw(fr[frameIndex].GraphicSheetTexture.Texture, fr[frameIndex].SourceRect,
                  Vector3.Empty, Vector3.Empty, -1);
            }
         }
            break;
         case DrawStyle.RepeatUpToCounter:
            foreach(int frameIndex in ts[TileIndex].CurrentFrame)
            {
               int FillHeight = PlanRectangle.Height * counter.CurrentValue / counter.MaxValue;
               disp.Device.RenderState.ScissorTestEnable = true;
               disp.Device.ScissorRectangle = new Rectangle(
                  PlanRectangle.X + ParentLayer.CurrentPosition.X + ParentLayer.ParentMap.View.X,
                  PlanRectangle.Y + ParentLayer.CurrentPosition.Y + ParentLayer.ParentMap.View.Y +
                  PlanRectangle.Height - FillHeight, PlanRectangle.Width, FillHeight);
               for (int repeat=0; repeat < (int)Math.Ceiling(FillHeight / (float)ts.TileHeight); repeat++)
               {
                  disp.Sprite.Transform = Matrix.Multiply(
                     fr[frameIndex].Transform,
                     Matrix.Translation(
                     PlanRectangle.X + ParentLayer.CurrentPosition.X + ParentLayer.ParentMap.View.X,
                     PlanRectangle.Y + ParentLayer.CurrentPosition.Y + ParentLayer.ParentMap.View.Y + PlanRectangle.Height - repeat * ts.TileHeight - ts.TileHeight,
                     0));
                  disp.Sprite.Draw(fr[frameIndex].GraphicSheetTexture.Texture, fr[frameIndex].SourceRect,
                     Vector3.Empty, Vector3.Empty, -1);
               }
            }
            break;
      }
      disp.Sprite.Flush();
   }
   #endregion

   #region IEnumerable Members

   public System.Collections.IEnumerator GetEnumerator()
   {
      if (Coordinates == null)
         new System.Collections.ArrayList().GetEnumerator();
      return Coordinates.GetEnumerator();
   }

   #endregion
}