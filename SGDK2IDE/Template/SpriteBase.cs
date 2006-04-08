using System;

/// <summary>
/// Base class for all sprite definitions.
/// </summary>
public abstract class SpriteBase
{
   public double x;
   public double y;
   public double dx;
   public double dy;
   public double oldX;
   public double oldY;
   public int state;
   public int frame;
   public InputBits inputs;
   public InputBits oldinputs;
   public bool isActive;

   [FlagsAttribute()]
   public enum InputBits
   {
      Up=1,
      Right=2,
      Down=4,
      Left=8,
      Button1=16,
      Button2=32,
      Button3=64,
      Button4=128
   }

   public SpriteBase(double x, double y, double dx, double dy, int state, int frame)
	{
      this.x = x;
      this.y = y;
      this.dx = dx;
      this.dy = dy;
      this.state = state;
      this.frame = frame;
	}

   #region Properties
   public int PixelX
   {
      get
      {
         return (int)x;
      }
   }

   public int PixelY
   {
      get
      {
         return (int)y;
      }
   }
   #endregion

   #region Virtual members
   /// <summary>
   /// How many pixels wide is the area of this sprite that avoids overlapping
   /// solid areas of the map.  The width is measured from the origin and
   /// extends rightward.
   /// </summary>
   public abstract int solidWidth
   {
      get;
   }

   /// <summary>
   /// How many pixels high is the area of this sprite that avoids overlapping
   /// solid areas of the map.  The height is measured from the origin and
   /// extends downward.
   /// </summary>
   public abstract int solidHeight
   {
      get;
   }
   #endregion

   #region Rider Feature
   /// <summary>
   /// Stores the platform sprite (the sprite that this sprite rides on).
   /// If not set, then the sprite is not riding anything.
   /// </summary>
   public SpriteBase RidingOn;
   /// <summary>
   /// Stores the horizontal position of this sprite relative to its platform sprite
   /// </summary>
   public double RideRelativeX;

   /// <summary>
   /// Moves this sprite according to the motion of the platform it is riding
   /// </summary>
   /// <param name="Slipperiness">A value from 0 to 100 where 0
   /// causes the sprite to immediately assume the velocity of the platform and
   /// 100 causes the sprite to retain its own velocity relative to the map.</param>
   public void ReactToPlatform(int Slipperiness)
   {
      if (RidingOn == null)
         return;
      x = RidingOn.x + RideRelativeX + dx;
      dx = (Slipperiness * dx + (100-RidingOn.dx) * RidingOn.dx) / 100.0d;
      y = RidingOn.PixelY - solidHeight;
      dy = 0;
   }

   /// <summary>
   /// Determine if the sprite is riding another sprite
   /// </summary>
   /// <returns>True if this sprite is currently riding on another sprite</returns>
   public bool IsRidingPlatform()
   {
      return RidingOn != null;
   }

   /// <summary>
   /// Stop riding the sprite that this sprite is currently riding, if any.
   /// </summary>
   public void StopRiding()
   {
      dx = dx + RidingOn.dx;
      dy = RidingOn.dy;
      RidingOn = null;
   }

   /// <summary>
   /// Tests to see if this sprite is landing on a platform (from above).
   /// If it is, the sprite will begin riding the platform.
   /// This should be called after sprites are moved, but before
   /// they are drawn.
   /// </summary>
   /// <param name="PlatformList">List of platform sprites to check</param>
   /// <returns>True if the sprite landed on a platform.</returns>
   public bool LandDownOnPlatform(SpriteCollection PlatformList)
   {
      foreach(SpriteBase spr in PlatformList)
      {
         if((oldY + solidHeight <= spr.oldY) &&
            (y + solidHeight > spr.y) &&
            (x + solidWidth > spr.x) &&
            (x < spr.x + spr.solidWidth))
         {
            RidingOn = spr;
            RideRelativeX = x - spr.x;
            dx = dx - spr.dx;
            y = spr.y - solidHeight;
            return true;
         }
      }
      return false;
   }
   #endregion

   #region Movement
   /// <summary>
   /// Increment or decrement horizontal velocity 
   /// </summary>
   /// <param name="delta">Amount by which to change velocity in pixels per frame per frame</param>
   public void AlterXVelocity(double delta)
   {
      dx += delta;
   }

   /// <summary>
   /// Increment or decrement horizontal velocity 
   /// </summary>
   /// <param name="delta">Amount by which to change velocity in pixels per frame per frame</param>
   public void AlterYVelocity(double delta)
   {
      dy += delta;
   }

   /// <summary>
   /// Move this sprite according to its current velocity
   /// </summary>
   public void MoveByVelocity()
   {
      x += dx;
      y += dy;
   }
   #endregion

   #region Input Processing
   public bool IsInputPressed(InputBits Input, bool InitialOnly)
   {
      return (0 != (inputs & Input)) && 
         (!InitialOnly || (0 == (oldinputs & Input)));
   }

   public void CopyInputsToOld()
   {
      oldinputs = inputs;
   }

   public void SetInputState(InputBits Input, bool Press)
   {
      if (Press)
         inputs |= Input;
      else
         inputs &= ~Input;
   }

   public void ClearInputs()
   {
      inputs = 0;
   }
   #endregion
}