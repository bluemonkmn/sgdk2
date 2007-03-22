using System;
using System.ComponentModel;
using System.Drawing;
using Microsoft.DirectX;

/// <summary>
/// Base class for "plans", which consist of map coordinates and rules
/// </summary>
[Serializable()]
public abstract class PlanBase : GeneralRules, System.Collections.IEnumerable
{
   public static int SharedTemp1;
   [Description("How close must a sprite be to a coordinate in this plan before heading to the next (default=5)")]
   public int TargetDistance = 5;

	protected PlanBase()
	{
	}

   [Serializable()]
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

   public virtual Rectangle PlanRectangle
   {
      get
      {
         return Rectangle.Empty;
      }
   }


   #region Sprites
   [Description("Make the specified sprite active.")]
   public void ActivateSprite(SpriteBase Target)
   {
      Target.isActive = true;
   }

   [Description("Make the specified sprite inactive.")]
   public void DeactivateSprite(SpriteBase Target)
   {
      Target.isActive = false;
   }

   [Description("Set the position of the source sprite to match that of the target sprite.")]
   public void MatchSpritePosition(SpriteBase Source, SpriteBase Target)
   {
      Source.x = Target.x;
      Source.y = Target.y;
   }

   [Description("Returns true if the specified sprite is touching this plan's rectangle")]
   public bool IsSpriteTouching(SpriteBase sprite)
   {
      if (!sprite.isActive)
         return false;

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

   [Description("Returns true if the specified part of the specified sprite is within the plan's rectangle")]
   public bool IsSpriteWithin(SpriteBase sprite, RelativePosition RelativePosition)
   {
      System.Drawing.Point rp = sprite.GetRelativePosition(RelativePosition);
      Rectangle targetRect = PlanRectangle;
      return targetRect.Contains(rp);
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


   [Description("Scroll all layers on this plan's layer's map so that the specified sprite is within the visible area of the map.  If UseScrollMargins is true, the layer will scroll the sprite into the scroll margins of the map.")]
   public void ScrollSpriteIntoView(SpriteBase Sprite, bool UseScrollMargins)
   {
      ParentLayer.ScrollSpriteIntoView(Sprite, UseScrollMargins);
   }

   [Description("Alter a sprite's velocity so that it remains within the map's visible area or within the scroll margins, according to this plan's layer's position within the map.")]
   public void PushSpriteIntoView(SpriteBase Sprite, bool StayInScrollMargins)
   {
      ParentLayer.PushSpriteIntoView(Sprite, StayInScrollMargins);
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

   [Description("Determine whether the sprite is within the TargetDistance of the specified coordinate, and has waited for the number of frames determined by the coordinate's weight based on the specified WaitCounter after reaching it.  If so, return the next CoordinateIndex, otherwise return the current CoordinateIndex.")]
   public int CheckNextCoordinate(SpriteBase Sprite, int CoordinateIndex, ref int WaitCounter)
   {
      if (WaitCounter > 0)
      {
         if (++WaitCounter > this[CoordinateIndex].weight)
         {
            WaitCounter = 0;
            return (CoordinateIndex + 1) % Count;
         }
         else
            return CoordinateIndex;
      }
      int dx = this[CoordinateIndex].x - Sprite.PixelX;
      int dy = this[CoordinateIndex].y - Sprite.PixelY;
      if (Math.Sqrt(dx * dx + dy * dy) < TargetDistance)
      {
         if (this[CoordinateIndex].weight > 0)
            WaitCounter++;
         else
            return (CoordinateIndex + 1) % Count;
      }
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

   [Description("Moves the specified sprite to the specified coordinate.")]
   public void TransportToPoint(SpriteBase sprite, Point target)
   {
      sprite.oldX = sprite.x;
      sprite.oldY = sprite.y;
      sprite.x = target.X;
      sprite.y = target.Y;
   }

   [Description("Moves the specified sprite to the specified plan's rectangle, aligned to the specified corner/edge.")]
   public void TransportToPlan(SpriteBase Sprite, PlanBase Plan, RelativePosition Alignment)
   {
      System.Diagnostics.Debug.Assert(!Plan.PlanRectangle.IsEmpty, "TransportToPlan was called on a plan that does not have a rectangle defined.");
      if (Plan.PlanRectangle.IsEmpty)
         return;
      switch(Alignment)
      {
         case RelativePosition.TopLeft:
         case RelativePosition.TopCenter:
         case RelativePosition.TopRight:
            Sprite.y = Plan.PlanRectangle.Y;
            break;
         case RelativePosition.LeftMiddle:
         case RelativePosition.CenterMiddle:
         case RelativePosition.RightMiddle:
            Sprite.y = Plan.PlanRectangle.Y + (int)((Plan.PlanRectangle.Height - Sprite.SolidHeight)/2);
            break;
         default:
            Sprite.y = Plan.PlanRectangle.Y + Plan.PlanRectangle.Height - Sprite.SolidHeight;
            break;
      }
      switch(Alignment)
      {
         case RelativePosition.TopLeft:
         case RelativePosition.LeftMiddle:
         case RelativePosition.BottomLeft:
            Sprite.x = Plan.PlanRectangle.X;
            break;
         case RelativePosition.TopCenter:
         case RelativePosition.CenterMiddle:
         case RelativePosition.BottomCenter:
            Sprite.x = Plan.PlanRectangle.X + (int)((Plan.PlanRectangle.Width - Sprite.SolidWidth)/2);
            break;
         default:
            Sprite.x = Plan.PlanRectangle.Y + Plan.PlanRectangle.Height - Sprite.SolidHeight;
            break;
      }
   }
   
   [Description("Associate the state of the input device for the specified player (1-4) with the inputs on the specified sprite.")]
   public void MapPlayerToInputs(int PlayerNumber, SpriteBase Target)
   {
      Target.MapPlayerToInputs(PlayerNumber);
   }

   #endregion

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

   public enum ColorChannel
   {
      Blue,
      Green,
      Red,
      Alpha
   }

   [Description("Modulate/scale the specified color channel of the specified sprite to the specified level (0-255)")]
   public void ModulateColor(SpriteBase Sprite, ColorChannel Channel, int Level)
   {
      switch(Channel)
      {
         case ColorChannel.Blue:
            Sprite.ModulateBlue = Level;
            break;
         case ColorChannel.Green:
            Sprite.ModulateGreen = Level;
            break;
         case ColorChannel.Red:
            Sprite.ModulateRed = Level;
            break;
         case ColorChannel.Alpha:
            Sprite.ModulateAlpha = Level;
            break;
      }
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

      Rectangle CurrentView = ParentLayer.ParentMap.CurrentView;

      switch(style)
      {
         case DrawStyle.ClipRightToCounter:
            disp.Device.RenderState.ScissorTestEnable = true;
            disp.Device.ScissorRectangle = new Rectangle(
               PlanRectangle.X + ParentLayer.CurrentPosition.X + CurrentView.X,
               PlanRectangle.Y + ParentLayer.CurrentPosition.Y + CurrentView.Y,
               PlanRectangle.Width * counter.CurrentValue / counter.MaxValue,
               PlanRectangle.Height);
            foreach(int frameIndex in ts[TileIndex].CurrentFrame)
            {
               disp.Sprite.Transform = Matrix.Multiply(Matrix.Multiply(
                  fr[frameIndex].Transform,
                  Matrix.Scaling(PlanRectangle.Width / (float)ts.TileWidth, 1, 1)),
                  Matrix.Translation(
                  PlanRectangle.X + ParentLayer.CurrentPosition.X + CurrentView.X,
                  PlanRectangle.Y + ParentLayer.CurrentPosition.Y + CurrentView.Y,
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
                  PlanRectangle.X + ParentLayer.CurrentPosition.X + CurrentView.X,
                  PlanRectangle.Y + ParentLayer.CurrentPosition.Y + CurrentView.Y,
                  0));
               disp.Sprite.Draw(fr[frameIndex].GraphicSheetTexture.Texture, fr[frameIndex].SourceRect,
                  Vector3.Empty, Vector3.Empty, -1);
            }
            break;
         case DrawStyle.RepeatRightToCounter:
            foreach(int frameIndex in ts[TileIndex].CurrentFrame)
            {
               int FillWidth = PlanRectangle.Width * counter.CurrentValue / counter.MaxValue;
               disp.Device.RenderState.ScissorTestEnable = false;
               disp.Device.ScissorRectangle = new Rectangle(
                  PlanRectangle.X + ParentLayer.CurrentPosition.X + CurrentView.X,
                  PlanRectangle.Y + ParentLayer.CurrentPosition.Y + CurrentView.Y,
                  FillWidth, PlanRectangle.Height);
               for (int repeat=0; repeat < (int)Math.Ceiling(FillWidth / (float)ts.TileWidth); repeat++)
               {
                  disp.Sprite.Transform = Matrix.Multiply(
                     fr[frameIndex].Transform,
                     Matrix.Translation(
                     PlanRectangle.X + ParentLayer.CurrentPosition.X + CurrentView.X + repeat * ts.TileWidth,
                     PlanRectangle.Y + ParentLayer.CurrentPosition.Y + CurrentView.Y,
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
               PlanRectangle.X + ParentLayer.CurrentPosition.X + CurrentView.X,
               PlanRectangle.Y + ParentLayer.CurrentPosition.Y + CurrentView.Y +
               PlanRectangle.Height - FillHeight, PlanRectangle.Width, FillHeight);
            foreach(int frameIndex in ts[TileIndex].CurrentFrame)
            {
               disp.Sprite.Transform = Matrix.Multiply(Matrix.Multiply(
                  fr[frameIndex].Transform,
                  Matrix.Scaling(1, PlanRectangle.Height / (float)ts.TileHeight, 1)),
                  Matrix.Translation(
                  PlanRectangle.X + ParentLayer.CurrentPosition.X + CurrentView.X,
                  PlanRectangle.Y + ParentLayer.CurrentPosition.Y + CurrentView.Y,
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
                  PlanRectangle.X + ParentLayer.CurrentPosition.X + CurrentView.X,
                  PlanRectangle.Y + ParentLayer.CurrentPosition.Y + CurrentView.Y + PlanRectangle.Height - FillHeight,
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
               disp.Device.RenderState.ScissorTestEnable = false;
               disp.Device.ScissorRectangle = new Rectangle(
                  PlanRectangle.X + ParentLayer.CurrentPosition.X + CurrentView.X,
                  PlanRectangle.Y + ParentLayer.CurrentPosition.Y + CurrentView.Y +
                  PlanRectangle.Height - FillHeight, PlanRectangle.Width, FillHeight);
               for (int repeat=0; repeat < (int)Math.Ceiling(FillHeight / (float)ts.TileHeight); repeat++)
               {
                  disp.Sprite.Transform = Matrix.Multiply(
                     fr[frameIndex].Transform,
                     Matrix.Translation(
                     PlanRectangle.X + ParentLayer.CurrentPosition.X + CurrentView.X,
                     PlanRectangle.Y + ParentLayer.CurrentPosition.Y + CurrentView.Y + PlanRectangle.Height - repeat * ts.TileHeight - ts.TileHeight,
                     0));
                  disp.Sprite.Draw(fr[frameIndex].GraphicSheetTexture.Texture, fr[frameIndex].SourceRect,
                     Vector3.Empty, Vector3.Empty, -1);
               }
            }
            break;
      }
      disp.Sprite.Flush();
   }

   [Description("Display a counter value as a number with a label in the current plan's rectangle")]
   public void DrawCounterWithLabel(string Label, Counter counter, System.Drawing.KnownColor color)
   {
      System.Diagnostics.Debug.Assert(!PlanRectangle.IsEmpty, "DrawCounterAsTile was called on a plan that does not have a rectangle defined");
      if (PlanRectangle.IsEmpty)
         return;
      
      Display disp = ParentLayer.ParentMap.Display;
      disp.Device.RenderState.ScissorTestEnable = false;
      disp.Sprite.Transform = Matrix.Identity;
      disp.D3DFont.DrawText(disp.Sprite, Label.ToString() + counter.CurrentValue.ToString(), PlanRectangle, Microsoft.DirectX.Direct3D.DrawTextFormat.Left, System.Drawing.Color.FromKnownColor(color));
      disp.Sprite.Flush();
   }
   #endregion

   [Description("Copy tiles from this plan's rectangle to another plan's rectangle.")]
   public void CopyTo(PlanBase Target, RelativePosition RelativePosition)
   {
      int src_left = (int)(PlanRectangle.X / ParentLayer.Tileset.TileWidth);
      int src_top = (int)(PlanRectangle.Y / ParentLayer.Tileset.TileHeight);
      int src_right = (int)((PlanRectangle.X + PlanRectangle.Width - 1) / ParentLayer.Tileset.TileWidth);
      int src_bottom = (int)((PlanRectangle.Y + PlanRectangle.Height - 1) / ParentLayer.Tileset.TileHeight);

      int dst_left = (int)(Target.PlanRectangle.X / Target.ParentLayer.Tileset.TileWidth);
      int dst_top = (int)(Target.PlanRectangle.Y / Target.ParentLayer.Tileset.TileHeight);
      int dst_right = (int)((Target.PlanRectangle.X + Target.PlanRectangle.Width - 1) / Target.ParentLayer.Tileset.TileWidth);
      int dst_bottom = (int)((Target.PlanRectangle.Y + Target.PlanRectangle.Height - 1) / Target.ParentLayer.Tileset.TileHeight);

      for (int y = src_top; y <= src_bottom; y++)
      {
         int targety;
         switch(RelativePosition)
         {
            case RelativePosition.TopLeft:
            case RelativePosition.TopCenter:
            case RelativePosition.TopRight:
               targety = dst_top + y - src_top;
               break;
            case RelativePosition.LeftMiddle:
            case RelativePosition.CenterMiddle:
            case RelativePosition.RightMiddle:
               targety = y + (int)(dst_top + dst_bottom - src_top - src_bottom) / 2;
               break;
            default:
               targety = dst_bottom + y - src_bottom;
               break;
         }
         if (targety < 0)
            continue;
         if (targety >= Target.ParentLayer.VirtualRows)
            break;
         for (int x = src_left; x <= src_right; x++)
         {
            int targetx;
            switch(RelativePosition)
            {
               case RelativePosition.TopLeft:
               case RelativePosition.LeftMiddle:
               case RelativePosition.BottomLeft:
                  targetx = dst_left + x - src_left;
                  break;
               case RelativePosition.TopCenter:
               case RelativePosition.CenterMiddle:
               case RelativePosition.BottomCenter:
                  targetx = x + (int)(dst_left + dst_right - src_left - src_right) / 2;
                  break;
               default:
                  targetx = dst_right + x - src_right;
                  break;
            }
            if (targetx < 0)
               continue;
            if (targetx >= Target.ParentLayer.VirtualColumns)
               break;
            
            Target.ParentLayer[targetx,targety] = ParentLayer[x,y];
         }
      }
   }

   [Description("Determine if the specified sprite's specified input is pressed.  InitialOnly causes this to return true only if the input has just been pressed and was not pressed before.")]
   public bool IsInputPressed(SpriteBase Sprite, SpriteBase.InputBits Input, bool InitialOnly)
   {
      return Sprite.IsInputPressed(Input, InitialOnly);
   }

   [Description("Ensure that all the inputs currently being pressed on the specified sprite are henceforth processed as already pressed.")]
   public void CopyInputsToOld(SpriteBase Sprite)
   {
      Sprite.oldinputs = Sprite.inputs;
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