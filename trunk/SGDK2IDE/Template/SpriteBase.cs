using System;
using System.ComponentModel;

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

   public SpriteBase(double x, double y, double dx, double dy, int state, int frame, bool active)
	{
      this.x = x;
      this.y = y;
      this.dx = dx;
      this.dy = dy;
      this.state = state;
      this.frame = frame;
      this.isActive = active;
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

   public int OldPixelX
   {
      get
      {
         return (int)oldX;
      }
   }

   public int OldPixelY
   {
      get
      {
         return (int)oldY;
      }
   }

   public SpriteState CurrentState
   {
      get
      {
         return this[state];
      }
   }

   #endregion

   #region Virtual members
   /// <summary>
   /// How many pixels wide is the area of this sprite that avoids overlapping
   /// solid areas of the map.  The width is measured from the origin and
   /// extends rightward.
   /// </summary>
   public abstract int SolidWidth
   {
      [Description("Returns the width of the sprite's solid area")]
      get;
   }

   /// <summary>
   /// How many pixels high is the area of this sprite that avoids overlapping
   /// solid areas of the map.  The height is measured from the origin and
   /// extends downward.
   /// </summary>
   public abstract int SolidHeight
   {
      [Description("Returns the height of the sprite's solid area")]
      get;
   }

   public abstract SpriteState this[int state]
   {
      get;
   }

   public abstract void ExecuteRules();
   #endregion

   #region Public Methods
   public System.Drawing.Rectangle GetBounds()
   {
      System.Drawing.Rectangle result = CurrentState.LocalBounds;
      result.Offset(PixelX, PixelY);
      return result;
   }

   public Frame[] GetCurrentFramesetFrames()
   {
      SpriteState curstate = CurrentState;
      Frameset stateframes = curstate.Frameset;
      int[] subframes = curstate.GetFrame(frame);
      Frame[] result = new Frame[subframes.Length];
      for(int idx = 0; idx < result.Length; idx++)
         result[idx] = stateframes[subframes[idx]];
      return result;
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
   [Description("Moves this sprite according to the motion of the platform it is riding. Slipperiness is a value from 0 to 100 where 0 causes the sprite to immediately assume the velocity of the platform and 100 causes the sprite to retain its own velocity relative to the map.")]
   public void ReactToPlatform(int Slipperiness)
   {
      if (RidingOn == null)
         return;
      x = RidingOn.x + RideRelativeX + dx;
      dx = (Slipperiness * dx + (100-RidingOn.dx) * RidingOn.dx) / 100.0d;
      y = RidingOn.PixelY - SolidHeight;
      dy = 0;
   }

   /// <summary>
   /// Determine if the sprite is riding another sprite
   /// </summary>
   /// <returns>True if this sprite is currently riding on another sprite</returns>
   [Description("Determine if the sprite is riding another sprite")]
   public bool IsRidingPlatform()
   {
      return RidingOn != null;
   }

   /// <summary>
   /// Stop riding the sprite that this sprite is currently riding, if any.
   /// </summary>
   [Description("Stop riding the sprite that this sprite is currently riding, if any.")]
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
   [Description("Tests to see if this sprite is landing on a platform (from above). If it is, the sprite will begin riding the platform.")]
   public bool LandDownOnPlatform(SpriteCollection PlatformList)
   {
      foreach(SpriteBase spr in PlatformList)
      {
         if((oldY + SolidHeight <= spr.oldY) &&
            (y + SolidHeight > spr.y) &&
            (x + SolidWidth > spr.x) &&
            (x < spr.x + spr.SolidWidth))
         {
            RidingOn = spr;
            RideRelativeX = x - spr.x;
            dx = dx - spr.dx;
            y = spr.y - SolidHeight;
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
   [Description("Increment or decrement horizontal velocity")]
   public void AlterXVelocity(double delta)
   {
      dx += delta;
   }

   /// <summary>
   /// Increment or decrement vertical velocity 
   /// </summary>
   /// <param name="delta">Amount by which to change velocity in pixels per frame per frame</param>
   [Description("Increment or decrement vertical velocity")]
   public void AlterYVelocity(double delta)
   {
      dy += delta;
   }

   /// <summary>
   /// Move this sprite according to its current velocity
   /// </summary>
   [Description("Move this sprite according to its current velocity")]
   public void MoveByVelocity()
   {
      oldX = x;
      oldY = y;
      x += dx;
      y += dy;
   }
   #endregion

   #region Input Processing
   [Description("Determine if the specified input is being pressed for this sprite.  InitialOnly causes this to return true only if the input has just been presssed and was not pressed before.")]
   public bool IsInputPressed(InputBits Input, bool InitialOnly)
   {
      return (0 != (inputs & Input)) && 
         (!InitialOnly || (0 == (oldinputs & Input)));
   }

   [Description("Move the current set of inputs on this sprite to the old set of inputs, making room for a new set.")]
   public void CopyInputsToOld()
   {
      oldinputs = inputs;
   }

   [Description("Turns on or off the specified input on this sprite.")]
   public void SetInputState(InputBits Input, bool Press)
   {
      if (Press)
         inputs |= Input;
      else
         inputs &= ~Input;
   }

   [Description("Turns off all current inputs on this sprite.")]
   public void ClearInputs()
   {
      inputs = 0;
   }
   #endregion
}