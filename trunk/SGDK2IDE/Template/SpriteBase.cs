using System;
using System.ComponentModel;
using System.Diagnostics;

/// <summary>
/// Base class for all sprite definitions.
/// </summary>
public abstract class SpriteBase : GeneralRules
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
   private LayerBase layer;
   private Solidity m_solidity;

   /// <summary>
   /// Horizontal velocity relative to the sprite's environment (like a platform)
   /// </summary>
   public double LocalDX;
   /// <summary>
   /// Vertical velocity relative to the sprite's environment
   /// </summary>
   public double LocalDY;

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

   public enum Direction
   {
      Up,
      Right,
      Down,
      Left
   }

   public enum SpriteAnimationType
   {
      ByFrame,
      ByHorizontalVelocity,
      ByVerticalVelocity,
      ByVectorVelocity
   }

   public enum Axis
   {
      Horizontal,
      Vertical
   }

   public SpriteBase(LayerBase layer, double x, double y, double dx, double dy, int state, int frame, bool active, Solidity solidity)
   {
      this.layer = layer;
      this.x = x;
      this.y = y;
      this.dx = dx;
      this.dy = dy;
      this.state = state;
      this.frame = frame;
      this.isActive = active;
      this.m_solidity = solidity;
      this.LocalDX = double.NaN;
      this.LocalDY = double.NaN;
   }

   #region Properties
   public int PixelX
   {
      get
      {
         Debug.Assert(this.isActive, "Attempted to access PixelX on an inactive sprite");
         return (int)x;
      }
   }

   public int PixelY
   {
      get
      {
         Debug.Assert(this.isActive, "Attempted to access PixelY on an inactive sprite");
         return (int)y;
      }
   }

   public int ProposedPixelX
   {
      get
      {
         Debug.Assert(this.isActive, "Attempted to access ProposedPixelX on an inactive sprite");
         return (int)(x+dx);
      }
   }

   public int ProposedPixelY
   {
      get
      {
         Debug.Assert(this.isActive, "Attempted to access ProposedPixelY on an inactive sprite");
         return (int)(y+dy);
      }
   }

   public int OldPixelX
   {
      get
      {
         Debug.Assert(this.isActive, "Attempted to access OldPixelX on an inactive sprite");
         return (int)oldX;
      }
   }

   public int OldPixelY
   {
      get
      {
         Debug.Assert(this.isActive, "Attempted to access OldPixelY on an inactive sprite");
         return (int)oldY;
      }
   }

   public SpriteState CurrentState
   {
      get
      {
         Debug.Assert(this.isActive, "Attempted to access CurrentState on an inactive sprite");
         return this[state];
      }
   }

   public override LayerBase ParentLayer
   {
      get
      {
         return layer;
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

   protected abstract void ExecuteRules();

   public abstract void ClearParameters();

   #endregion

   #region Public Methods
   public System.Drawing.Rectangle GetBounds()
   {
      Debug.Assert(this.isActive, "Attempted to execute GetBounds on an inactive sprite");
      System.Drawing.Rectangle result = CurrentState.LocalBounds;
      result.Offset(PixelX, PixelY);
      return result;
   }

   public Frame[] GetCurrentFramesetFrames()
   {
      Debug.Assert(this.isActive, "Attempted to execute GetCurrentFramesetFrames on an inactive sprite");
      SpriteState curstate = CurrentState;
      Frameset stateframes = curstate.Frameset;
      int[] subframes = curstate.GetFrame(frame);
      Frame[] result = new Frame[subframes.Length];
      for(int idx = 0; idx < result.Length; idx++)
         result[idx] = stateframes[subframes[idx]];
      return result;
   }

   /// <summary>
   /// Execute the rules for this sprite if they have not already been executed this frame
   /// </summary>
   public void ProcessRules()
   {
      if (!Processed)
         ExecuteRules();
      Processed = true;
   }

   #endregion

   #region Rider Feature
   /// <summary>
   /// Stores the platform sprite (the sprite that this sprite rides on).
   /// If not set, then the sprite is not riding anything.
   /// </summary>
   public SpriteBase RidingOn;
   /// <summary>
   /// Determines if this sprite's definition's rules have been processed yet this frame
   /// </summary>
   public bool Processed;

   /// <summary>
   /// Adjust this sprite's velocity according to the motion of the platform it is riding
   /// </summary>
   [Description("Moves this sprite according to the motion of the platform it is riding. Slipperiness is a value from 0 to 100 where 0 causes the sprite to immediately assume the velocity of the platform and 100 causes the sprite to retain its own velocity relative to the map.")]
   public void ReactToPlatform()
   {
      Debug.Assert(this.isActive, "Attempted to execute ReactToPlatform on an inactive sprite");
      if (RidingOn == null)
         return;

      // Don't try to process the platform's rules if it's already moved.
      // Even though this is already being checked in ProcessRules, circular
      // references (which shouldn't exist) would lead to deadlock.
      if (!RidingOn.Processed)
         // Ensure that the sprite that this sprite is riding moves first
         RidingOn.ProcessRules();

      if ((x+SolidWidth < RidingOn.oldX) || (x > RidingOn.oldX+RidingOn.SolidWidth) ||
         (y+SolidHeight < RidingOn.oldY - 1) || (y+SolidHeight >= RidingOn.oldY+SolidHeight))
      {
         StopRiding();
         return;
      }

      if (double.IsNaN(LocalDX))
         Debug.Fail("LocalDX is not a number");
      else
         dx = LocalDX + RidingOn.dx;
      dy = RidingOn.y - SolidHeight - y;
   }

   /// <summary>
   /// Determine if the sprite is riding another sprite
   /// </summary>
   /// <returns>True if this sprite is currently riding on another sprite</returns>
   [Description("Determine if the sprite is riding another sprite")]
   public bool IsRidingPlatform()
   {
      Debug.Assert(this.isActive, "Attempted to execute IsRidingPlatform on an inactive sprite");
      return RidingOn != null;
   }

   /// <summary>
   /// Stop riding the sprite that this sprite is currently riding, if any.
   /// </summary>
   [Description("Stop riding the sprite that this sprite is currently riding, if any.")]
   public void StopRiding()
   {
      Debug.Assert(this.isActive, "Attempted to execute StopRiding on an inactive sprite");
      LocalDX = double.NaN;
      RidingOn = null;
   }

   /// <summary>
   /// Tests to see if this sprite is landing on a platform (from above).
   /// If it is, the sprite will begin riding the platform.
   /// This should be called after sprites are moved, but before
   /// they are drawn.
   /// </summary>
   /// <param name="PlatformList">List of platform sprites to check</param>
   /// <returns>True if the sprite landed on a platform, False if it is already riding a platform or doesn't need to</returns>
   [Description("Tests to see if this sprite is landing on a platform (from above). If it is, the sprite will begin riding the platform.")]
   public bool LandDownOnPlatform(SpriteCollection PlatformList)
   {
      Debug.Assert(this.isActive, "Attempted to execute LandDownOnPlatform on an inactive sprite");
      if (RidingOn != null)
         return false;
      foreach(SpriteBase spr in PlatformList)
      {
         if (!spr.isActive)
            continue;
         if((oldY + SolidHeight <= spr.oldY) &&
            (y + SolidHeight > spr.y) &&
            (x + SolidWidth > spr.x) &&
            (x < spr.x + spr.SolidWidth))
         {
            RidingOn = spr;
            spr.ProcessRules();
            LocalDX = dx - spr.dx;
            dy = spr.y - SolidHeight - y;
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
      Debug.Assert(this.isActive, "Attempted to execute AlterXVelocity on an inactive sprite");
      dx += delta;
   }

   /// <summary>
   /// Increment or decrement vertical velocity 
   /// </summary>
   /// <param name="delta">Amount by which to change velocity in pixels per frame per frame</param>
   [Description("Increment or decrement vertical velocity")]
   public void AlterYVelocity(double delta)
   {
      Debug.Assert(this.isActive, "Attempted to execute AlterYVelocity on an inactive sprite");
      dy += delta;
   }

   /// <summary>
   /// Move this sprite according to its current velocity
   /// </summary>
   [Description("Move this sprite according to its current velocity")]
   public void MoveByVelocity()
   {
      Debug.Assert(this.isActive, "Attempted to execute MoveByVelocity on an inactive sprite");
      oldX = x;
      oldY = y;
      x += dx;
      y += dy;
   }

   [Description("Limit the velocity of the sprite to the specified maximum pixels per frame (affects only to local velocity when applicable)")]
   public void LimitVelocity(int Maximum)
   {
      Debug.Assert(this.isActive, "Attempted to execute LimitVelocity on an inactive sprite");
      double useDX, useDY;
      if (double.IsNaN(LocalDX))
         useDX = dx;
      else
         useDX = LocalDX;
      if (double.IsNaN(LocalDY))
         useDY = dy;
      else
         useDY = LocalDY;
      double dist = useDX * useDX + useDY * useDY;
      if (dist > Maximum * Maximum)
      {
         dist = Math.Sqrt(dist);
         useDX = useDX * Maximum / dist;
         useDY = useDY * Maximum / dist;
         if (double.IsNaN(LocalDX))
            dx = useDX;
         else
            LocalDX = useDX;
         if (double.IsNaN(LocalDY))
            dy = useDY;
         else
            LocalDY = useDY;
      }
   }

   [Description("Reduces the sprites velocity to simulate friction.  RetainPercent is a number 0 to 100 indicating how much inertia is retained.")]
   public void ReactToInertia(int RetainPercentVertical, int RetainPercentHorizontal)
   {
      Debug.Assert(this.isActive, "Attempted to execute ReactToInertia on an inactive sprite");
      if (double.IsNaN(LocalDX))
      {
         if (Math.Abs(dx) < .01)
            dx = 0;
         else
            dx *= RetainPercentHorizontal / 100.0f;
      }
      else
      {
         if (Math.Abs(LocalDX) < .01)
            LocalDX = 0;
         else
            LocalDX *= RetainPercentHorizontal / 100.0f;
      }
      if (double.IsNaN(LocalDY))
      {
         if (Math.Abs(dy) < .01)
            dy = 0;
         else
            dy *= RetainPercentVertical / 100.0f;
      }
      else
      {
         if (Math.Abs(LocalDY) < .01)
            LocalDY = 0;
         else
            LocalDY *= RetainPercentVertical / 100.0f;
      }
   }

   [Description("Returns true if the sprite is moving in the specified direction")]
   public bool IsMoving(Direction Direction)
   {
      Debug.Assert(this.isActive, "Attempted to execute IsMoving on an inactive sprite");
      double useDX, useDY;
      if (double.IsNaN(LocalDX))
         useDX = dx;
      else
         useDX = LocalDX;
      if (double.IsNaN(LocalDY))
         useDY = dy;
      else
         useDY = LocalDY;

      switch(Direction)
      {
         case Direction.Left:
            return useDX < 0;
         case Direction.Right:
            return useDX > 0;
         case Direction.Up:
            return useDY < 0;
         case Direction.Down:
            return useDY > 0;
      }
      return false;
   }

   [Description("Accelerate the sprite in a direction determined by its state, assuming the first state points rightward and the number of states rotate counterclockwise 360 degrees. Acceleration is in tenths of a pixel per frame per frame.")]
   public void PolarAccelerate(int Acceleration, [Editor("SpriteState", "UITypeEditor")] int FirstState, int StateCount)
   {
      Debug.Assert(this.isActive, "Attempted to execute PolarAccelerate on an inactive sprite");
      double angle = (state - FirstState) * Math.PI * 2 / (double)StateCount;
      double ddx = Math.Cos(angle) * Acceleration / 10.0d;
      double ddy = -Math.Sin(angle) * Acceleration / 10.0d;
      if (double.IsNaN(LocalDY))
         dy += ddy;
      else
         LocalDY += ddy;

      if (double.IsNaN(LocalDX))
         dx += ddx;
      else
         LocalDX += ddx;
   }
   
   [Description("Scroll all layers on this sprite's layer's map so that the sprite is within the scroll margins of the map")]
   public void ScrollSpriteIntoView()
   {
      ParentLayer.ScrollSpriteIntoView(this);
   }

   [Description("Alter this sprite's velocity so that it remains within the map's visible area or within the scroll margins, according to this sprite's layer's position within the map.")]
   public void PushSpriteIntoView(bool StayInScrollMargins)
   {
      ParentLayer.PushSpriteIntoView(this, StayInScrollMargins);
   }
   #endregion

   #region States and animation
   [Description("Advance the animation frame of this sprite according to its velocity or a constant rate")]
   public void Animate(SpriteAnimationType Correlation)
   {
      Debug.Assert(this.isActive, "Attempted to execute Animate on an inactive sprite");
      switch(Correlation)
      {
         case SpriteAnimationType.ByFrame:
            frame++;
            break;
         case SpriteAnimationType.ByHorizontalVelocity:
            if (double.IsNaN(LocalDX))
               frame += System.Math.Abs(ProposedPixelX - PixelX);
            else
               frame += System.Math.Abs((int)LocalDX);
            break;
         case SpriteAnimationType.ByVerticalVelocity:
            if (double.IsNaN(LocalDY))
               frame += System.Math.Abs(ProposedPixelY - PixelY);
            else
               frame += System.Math.Abs((int)LocalDY);
            break;
         case SpriteAnimationType.ByVectorVelocity:
         {
            int tmpDx = ProposedPixelX - PixelX;
            int tmpDy = ProposedPixelY - PixelY;
            frame += (int)(System.Math.Sqrt(tmpDx * tmpDx + tmpDy * tmpDy));
         }
            break;
      }
   }

   [Description("Return the state that a rotating sprite should use in order to point in the direction it is currently travelling, assuming that FirstState points rightward and each subsequent state is one step counter-clockwise")]
   public int GetPolarStateByVector([Editor("SpriteState", "UITypeEditor")] int FirstState, int StateCount)
   {
      double useDX, useDY;
      Debug.Assert(this.isActive, "Attempted to execute GetPolarStateByVector on an inactive sprite");
      if (double.IsNaN(LocalDX))
         useDX = dx;
      else
         useDX = LocalDX;
      if (double.IsNaN(LocalDY))
         useDY = dy;
      else
         useDY = LocalDY;
      return FirstState + ((StateCount + (int)Math.Round(System.Math.Atan2(-useDY,useDX) * StateCount / Math.PI / 2f)) % StateCount);
   }

   [Description("Switch the sprite to the the specified state")]
   public void SwitchToState([Editor("SpriteState", "UITypeEditor")] int State)
   {
      Debug.Assert(this.isActive, "Attempted to execute SwitchToState on an inactive sprite");
      state = State;
   }

   [Description("Determines if the sprite is in the specified range of states")]
   public bool IsInState([Editor("SpriteState", "UITypeEditor")] int FirstState, [Editor("SpriteState", "UITypeEditor")] int LastState)
   {
      Debug.Assert(this.isActive, "Attempted to execute IsInState on an inactive sprite");
      return (state <= FirstState) && (state >= LastState);
   }
   #endregion

   #region Input Processing
   [Description("Determine if the specified input is being pressed for this sprite.  InitialOnly causes this to return true only if the input has just been presssed and was not pressed before.")]
   public bool IsInputPressed(InputBits Input, bool InitialOnly)
   {
      Debug.Assert(this.isActive, "Attempted to execute IsInputPressed on an inactive sprite");
      return (0 != (inputs & Input)) && 
         (!InitialOnly || (0 == (oldinputs & Input)));
   }

   [Description("Move the current set of inputs on this sprite to the old set of inputs, making room for a new set.")]
   public void CopyInputsToOld()
   {
      Debug.Assert(this.isActive, "Attempted to execute CopyInputsToOld on an inactive sprite");
      oldinputs = inputs;
   }

   [Description("Turns on or off the specified input on this sprite.")]
   public void SetInputState(InputBits Input, bool Press)
   {
      Debug.Assert(this.isActive, "Attempted to execute SetInputState on an inactive sprite");
      if (Press)
         inputs |= Input;
      else
         inputs &= ~Input;
   }

   [Description("Turns off all current inputs on this sprite.")]
   public void ClearInputs()
   {
      Debug.Assert(this.isActive, "Attempted to execute ClearInputs on an inactive sprite");
      inputs = 0;
   }

   [Description("Associates the state of the specified keyboard key with the specified input on this sprite.")]
   public void MapKeyToInput(Microsoft.DirectX.DirectInput.Key key, InputBits Input)
   {
      Debug.Assert(this.isActive, "Attempted to execute MapKeyToInput on an inactive sprite");
      SetInputState(Input, Project.GameWindow.KeyboardState[key]);
   }

   [Description("Accelerate this sprite according to which directional inputs are on.  Acceleration is in tenths of a pixel per second squared.  Max is in pixels per second.")]
   public void AccelerateByInputs(int Acceleration, int Max, bool HorizontalOnly)
   {
      Debug.Assert(this.isActive, "Attempted to execute AccelerateByInputs on an inactive sprite");
      if (!HorizontalOnly)
      {
         if (double.IsNaN(LocalDY))
         {
            if (0 != (inputs & InputBits.Up))
               dy -= ((double)Acceleration)/10.0d;
            if (dy < -(double)Max)
               dy = -(double)Max;
            if (0 != (inputs & InputBits.Down))
               dy += ((double)Acceleration)/10.0d;
            if (dy > (double)Max)
               dy = (double)Max;
         }
         else
         {
            if (0 != (inputs & InputBits.Up))
               LocalDY -= ((double)Acceleration)/10.0d;
            if (LocalDY < -(double)Max)
               LocalDY = -(double)Max;
            if (0 != (inputs & InputBits.Down))
               LocalDY += ((double)Acceleration)/10.0d;
            if (LocalDY > (double)Max)
               LocalDY = (double)Max;
         }
      }
      if (double.IsNaN(LocalDX))
      {
         if (0 != (inputs & InputBits.Left))
            dx -= ((double)Acceleration)/10.0d;
         if (dx < -(double)Max)
            dx = -(double)Max;
         if (0 != (inputs & InputBits.Right))
            dx += ((double)Acceleration)/10.0d;
         if (dx > (double)Max)
            dx = (double)Max;
      }
      else
      {
         if (0 != (inputs & InputBits.Left))
            LocalDX -= ((double)Acceleration)/10.0d;
         if (LocalDX < -(double)Max)
            LocalDX = -(double)Max;
         if (0 != (inputs & InputBits.Right))
            LocalDX += ((double)Acceleration)/10.0d;
         if (LocalDX > (double)Max)
            LocalDX = (double)Max;
      }
   }
   #endregion

   #region Solidity
   public Solidity CurrentSolidity
   {
      [Description("Set the solidity rules to which the sprite is currently reacting")]
      set
      {
         m_solidity = value;
      }
   }

   [Description("Alter the sprite's velocity to react to solid areas on the map.  Returns true if sprite touches solid.")]
   public bool ReactToSolid()
   {
      Debug.Assert(this.isActive, "Attempted to execute ReactToSolid on an inactive sprite");
      bool hit = false;
      double dyOrig = dy;

      int ProposedPixelY2 = (int)Math.Ceiling(y+dy);
      int SolidPixelWidth = SolidWidth + (int)Math.Ceiling(x) - PixelX;
      if (dy > 0)
      {
         int ground = layer.GetTopSolidPixel(new System.Drawing.Rectangle(PixelX, PixelY+SolidHeight, SolidPixelWidth, ProposedPixelY2 - PixelY), m_solidity);
         if (ground != int.MinValue)
         {
            // Do integer arithmetic before double otherwise strange rounding seems to happen
            dy = ground - SolidHeight - y;
            hit = true;
         }
      } 
      else if (dy < 0)
      {
         int ceiling = layer.GetBottomSolidPixel(new System.Drawing.Rectangle(PixelX, ProposedPixelY, SolidPixelWidth, PixelY - ProposedPixelY), m_solidity);
         if (ceiling != int.MinValue)
         {
            // Do integer arithmetic before double otherwise strange rounding seems to happen
            dy = ceiling + 1 - y;
            hit = true;
         }
      }

      if (dx > 0)
      {
         int ProposedPixelX2 = (int)Math.Ceiling(x+dx);
         int PixelX2 = (int)Math.Ceiling(x);
         int rightwall = layer.GetLeftSolidPixel(new System.Drawing.Rectangle(PixelX2 + SolidWidth, ProposedPixelY, ProposedPixelX2 - PixelX2, SolidHeight), m_solidity);
         bool hitWall = false;
         if (rightwall != int.MinValue)
         {
            int maxSlopeProposedY = (int)(y + dy - dx);
            int slopedFloor = layer.GetTopSolidPixel(new System.Drawing.Rectangle(PixelX2 + SolidWidth, maxSlopeProposedY + SolidHeight, ProposedPixelX2 - PixelX2, ProposedPixelY - maxSlopeProposedY), m_solidity);
            if (slopedFloor != int.MinValue)
            {
               int ceiling = layer.GetBottomSolidPixel(new System.Drawing.Rectangle(ProposedPixelX2, slopedFloor - SolidHeight, SolidWidth, ProposedPixelY + SolidHeight - slopedFloor), m_solidity);
               if (ceiling == int.MinValue)
               {
                  int rightwall2 = layer.GetLeftSolidPixel(new System.Drawing.Rectangle(PixelX2 + SolidWidth, slopedFloor - SolidHeight, ProposedPixelX2 - PixelX2, SolidHeight), m_solidity);
                  if (rightwall2 == int.MinValue)
                     // Do integer arithmetic before double otherwise strange rounding seems to happen
                     dy = dyOrig = slopedFloor - SolidHeight - 1 - y;
                  else
                     hitWall = true;
               }
               else
                  hitWall = true;
            }
            else
            {
               maxSlopeProposedY = (int)(y + dy + dx);
               int slopedCeiling = layer.GetBottomSolidPixel(new System.Drawing.Rectangle(PixelX2 + SolidWidth, ProposedPixelY, ProposedPixelX2 - PixelX2, maxSlopeProposedY - ProposedPixelY), m_solidity);
               if (slopedCeiling != int.MinValue)
               {
                  slopedCeiling++;
                  int floor = layer.GetTopSolidPixel(new System.Drawing.Rectangle(ProposedPixelX2, ProposedPixelY + SolidHeight, SolidWidth, slopedCeiling - ProposedPixelY), m_solidity);
                  if (floor == int.MinValue)
                  {
                     int rightwall2 = layer.GetLeftSolidPixel(new System.Drawing.Rectangle(PixelX2 + SolidWidth, slopedCeiling, ProposedPixelX2 - PixelX2, SolidHeight), m_solidity);
                     if (rightwall2 == int.MinValue)
                        dy = dyOrig = slopedCeiling - y;
                     else
                        hitWall = true;
                  }
                  else
                     hitWall = true;
               }
               else
                  hitWall = true;
            }
            if (hitWall)
            {
               // Do integer arithmetic before double otherwise strange rounding seems to happen
               dx = rightwall - SolidWidth - x;
            }
            hit = true;
         }
      }
      else if (dx < 0)
      {
         int leftwall = layer.GetRightSolidPixel(new System.Drawing.Rectangle(ProposedPixelX, ProposedPixelY, PixelX - ProposedPixelX, SolidHeight), m_solidity);
         bool hitWall = false;
         if (leftwall != int.MinValue)
         {
            int maxSlopeProposedY = (int)(y + dy + dx);
            int slopedFloor = layer.GetTopSolidPixel(new System.Drawing.Rectangle(ProposedPixelX, maxSlopeProposedY + SolidHeight, PixelX - ProposedPixelX, ProposedPixelY - maxSlopeProposedY), m_solidity);
            if (slopedFloor != int.MinValue)
            {
               int ceiling = layer.GetBottomSolidPixel(new System.Drawing.Rectangle(ProposedPixelX, slopedFloor - SolidHeight, SolidWidth, ProposedPixelY + SolidHeight - slopedFloor), m_solidity);
               if (ceiling == int.MinValue)
               {
                  int leftwall2 = layer.GetRightSolidPixel(new System.Drawing.Rectangle(ProposedPixelX, slopedFloor - SolidHeight, PixelX - ProposedPixelX, SolidHeight), m_solidity);
                  if (leftwall2 == int.MinValue)
                     // Do integer arithmetic before double otherwise strange rounding seems to happen
                     dy = dyOrig = slopedFloor - SolidHeight - 1 - y;
                  else
                     hitWall = true;
               }
               else
                  hitWall = true;
            }
            else
            {
               maxSlopeProposedY = (int)(y + dy - dx);
               int slopedCeiling = layer.GetBottomSolidPixel(new System.Drawing.Rectangle(ProposedPixelX, ProposedPixelY, PixelX - ProposedPixelX, maxSlopeProposedY - ProposedPixelY), m_solidity);
               if (slopedCeiling != int.MinValue)
               {
                  slopedCeiling++;
                  int floor = layer.GetTopSolidPixel(new System.Drawing.Rectangle(ProposedPixelX, ProposedPixelY + SolidHeight, SolidWidth, slopedCeiling - ProposedPixelY), m_solidity);
                  if (floor == int.MinValue)
                  {
                     int leftwall2 = layer.GetRightSolidPixel(new System.Drawing.Rectangle(ProposedPixelX, slopedCeiling, PixelX - ProposedPixelX, SolidHeight), m_solidity);
                     if (leftwall2 == int.MinValue)
                        dy = dyOrig = slopedCeiling - y;
                     else
                        hitWall = true;
                  }
                  else
                     hitWall = true;
               }
               else
                  hitWall = true;
            }
            if (hitWall)
            {
               // Do integer arithmetic before double otherwise strange rounding seems to happen
               dx = leftwall + 1 - x;
            }
            hit = true;
         }
      }

      dy = dyOrig;

      int ProposedSolidPixelWidth = SolidWidth + (int)Math.Ceiling(x+dx) - ProposedPixelX;
      if (dy > 0)
      {
         ProposedPixelY2 = (int)Math.Ceiling(y+dy);
         int ground = layer.GetTopSolidPixel(new System.Drawing.Rectangle(ProposedPixelX, PixelY+SolidHeight, ProposedSolidPixelWidth, ProposedPixelY2 - PixelY), m_solidity);
         if (ground != int.MinValue)
         {
            // Do integer arithmetic before double otherwise strange rounding seems to happen
            dy = ground - SolidHeight - y;
            hit = true;
         }
      }
      else if (dy < 0)
      {
         int ceiling = layer.GetBottomSolidPixel(new System.Drawing.Rectangle(ProposedPixelX, ProposedPixelY, ProposedSolidPixelWidth, PixelY - ProposedPixelY), m_solidity);
         if (ceiling != int.MinValue)
         {
            // Do integer arithmetic before double otherwise strange rounding seems to happen
            dy = ceiling + 1 - y;
            hit = true;
         }
      }

      return hit;
   }

   [Description("If the sprite's proposed position is within <Threshhold> pixels of the ground, alter its velocity so it will touch the ground.  Returns true if snap occurred.")]
   public bool SnapToGround(int Threshhold)
   {
      Debug.Assert(this.isActive, "Attempted to execute SnapToGround on an inactive sprite");

      int ProposedSolidPixelWidth = SolidWidth + (int)Math.Ceiling(x+dx) - ProposedPixelX;
      int ground = layer.GetTopSolidPixel(new System.Drawing.Rectangle(ProposedPixelX, ProposedPixelY+SolidHeight, ProposedSolidPixelWidth, Threshhold), m_solidity);
      if (ground != int.MinValue)
      {
         // Do integer arithmetic before double otherwise strange rounding seems to happen
         double newDy = ground - SolidHeight - y;
         if (newDy > dy)
            dy = newDy;
         return true;
      }
      return false;
   }

   [Description("Determines if the sprite is blocked from moving in a particular direction by solidity on the layer.")]
   public bool Blocked(Direction Direction)
   {
      Debug.Assert(this.isActive, "Attempted to execute Blocked on an inactive sprite");

      int SolidPixelWidth;
      int SolidPixelHeight;
      switch(Direction)
      {
         case Direction.Up:
            SolidPixelWidth = SolidWidth + (int)Math.Ceiling(x) - PixelX;
            return layer.GetBottomSolidPixel(new System.Drawing.Rectangle(PixelX, PixelY-1, SolidPixelWidth, 1), m_solidity) != int.MinValue;
         case Direction.Right:
            SolidPixelHeight = SolidHeight + (int)Math.Ceiling(y) - PixelY;
            return layer.GetLeftSolidPixel(new System.Drawing.Rectangle(PixelX+SolidWidth, PixelY, 1, SolidPixelHeight), m_solidity) != int.MinValue;
         case Direction.Down:
            SolidPixelWidth = SolidWidth + (int)Math.Ceiling(x) - PixelX;
            return layer.GetTopSolidPixel(new System.Drawing.Rectangle(PixelX, PixelY+SolidHeight, SolidPixelWidth, 1), m_solidity) != int.MinValue;
         case Direction.Left:
            SolidPixelHeight = SolidHeight + (int)Math.Ceiling(y) - PixelY;
            return layer.GetRightSolidPixel(new System.Drawing.Rectangle(PixelX - 1, PixelY, 1, SolidPixelHeight), m_solidity) != int.MinValue;
      }
      return false;
   }

   #endregion

   #region Tile Interaction
   class TouchedTile
   {
      public int x;
      public int y;
      public int tileValue;
      public bool initial;
      public bool processed;

      public TouchedTile(int x, int y, int tileValue, bool initial)
      {
         this.x = x;
         this.y = y;
         this.tileValue = tileValue;
         this.initial = initial;
         processed = false;
      }
   }

   // Do not allocate appreciable memory unless this sprite
   // participates in tile interaction.
   System.Collections.ArrayList TouchedTiles = null;

   [Description("Collects information about tiles the sprite is currently touching.  Category should include all tiles that the sprite interacts with.  Must be called before performing any tile interaction.")]
   public bool TouchTiles(TileCategoryName Category)
   {
      Debug.Assert(this.isActive, "Attempted to execute TouchTiles on an inactive sprite");
      
      if (TouchedTiles != null)
         TouchedTiles.Clear();

      int tw = layer.Tileset.TileWidth;
      int th = layer.Tileset.TileHeight;
      int minYEdge = (PixelY / th);
      int maxY = (PixelY + SolidHeight) / th;
      int maxYEdge = (PixelY + SolidHeight - 1) / th;
      int minX = (PixelX - 1)/ tw;
      int minXEdge = PixelX / tw;
      int maxX = (PixelX + SolidHeight) / tw;
      int maxXEdge = (PixelX + SolidHeight - 1) / tw;
      for (int yidx = (PixelY - 1) / th; yidx <= maxY; yidx++)
      {
         bool isYEdge = !((yidx >= minYEdge) && (yidx <= maxYEdge));
         for (int xidx = (isYEdge ? minXEdge : minX);
            xidx <= (isYEdge ? maxXEdge : maxX);
            xidx++)
         {
            if (layer.GetTile(xidx, yidx).IsMember(Category))
            {
               bool wasTouching;

               if ((OldPixelX <= xidx * tw + tw) &&
                  (OldPixelX + SolidWidth >= xidx * tw) &&
                  (OldPixelY <= yidx * th + th) &&
                  (OldPixelY + SolidHeight >= yidx * th))
               {
                  bool edgeX = (OldPixelX+SolidWidth == xidx * tw) || 
                     (OldPixelX == xidx * tw + tw);
                  bool edgeY = (OldPixelY+SolidHeight == yidx * th) ||
                     (OldPixelY == yidx * th + th);
                  if (edgeX && edgeY)
                     wasTouching = false;
                  else
                     wasTouching = true;
               }
               else
                  wasTouching = false;
               
               if (TouchedTiles == null)
                  TouchedTiles = new System.Collections.ArrayList(10);
               TouchedTiles.Add(new TouchedTile(xidx, yidx, layer[xidx, yidx], !wasTouching));
            }
         }
      }
      if (TouchedTiles == null)
         return false;
      return TouchedTiles.Count > 0;
   }

   [Description("When the sprite is touching the specified tile, and the specified counter is not maxed, clear the tile value to 0 and increment the specified counter/parameter. Returns the number of tiles affected. (Must run TouchTiles first.)")]
   public int TileTake(int TileValue,  Counter Counter)
   {
      Debug.Assert(this.isActive, "Attempted to execute TileTake on an inactive sprite");

      if (TouchedTiles == null)
         return 0 ;

      int result = 0;

      for (int i=0; i < TouchedTiles.Count; i++)
      {
         TouchedTile tt = (TouchedTile)TouchedTiles[i];
         if ((tt.tileValue == TileValue) && (!tt.processed))
         {
            if (Counter.CurrentValue < Counter.MaxValue)
            {
               Counter.CurrentValue++;
               layer[tt.x, tt.y] = tt.tileValue = 0;
               tt.processed = true;
               result++;
            }
         }
      }
      return result;
   }

   [Description("When the sprite is touching the specified tile, and the specified counter is greater than 0, decrement the counter and clear the tile value to 0. Returns the number of tiles affected. (Must run TouchTiles first.)")]
   public int TileUseUp(int TileValue,  Counter Counter)
   {
      Debug.Assert(this.isActive, "Attempted to execute TileUseUp on an inactive sprite");

      if (TouchedTiles == null)
         return 0;

      int result = 0;

      for (int i=0; i < TouchedTiles.Count; i++)
      {
         TouchedTile tt = (TouchedTile)TouchedTiles[i];
         if ((tt.tileValue == TileValue) && (!tt.processed))
         {
            if (Counter.CurrentValue > 0)
            {
               Counter.CurrentValue--;
               layer[tt.x, tt.y] = tt.tileValue = 0;
               tt.processed = true;
               result++;
            }
         }
      }
      return result;
   }

   [Description("Return the index of the next unprocessed tile with the specified value from the list of tiles the sprite is touching. If InitialOnly is set, only return tiles that the sprite wasn't already touching. Return -1 if no tiles are being touched. (Must run TouchTiles first.)")]
   public int TileTouchingIndex(int TileValue, bool InitialOnly, bool MarkAsProcessed)
   {
      Debug.Assert(this.isActive, "Attempted to execute TileTouchingIndex on an inactive sprite");

      if (TouchedTiles == null)
         return -1;

      for (int i=0; i < TouchedTiles.Count; i++)
      {
         TouchedTile tt = (TouchedTile)TouchedTiles[i];
         if ((tt.tileValue == TileValue) && (!tt.processed) && (!InitialOnly || tt.initial))
         {
            tt.processed = MarkAsProcessed;
            return i;
         }
      }
      
      return -1;
   }

   [Description("Activate the next inactive sprite from a category at the coordinates of a tile being touched by the player.  Use TileTouchingIndex to acquire TouchingIndex.")]
   public int TileActivateSprite(int TouchingIndex, SpriteCollection Category, bool ClearParameters)
   {
      Debug.Assert(this.isActive, "Attempted to execute TileActivateSprite on an inactive sprite");

      for(int i=0; i<Category.Count; i++)
      {
         if (!Category[i].isActive)
         {
            Category[i].isActive = true;
            TouchedTile tt = (TouchedTile)TouchedTiles[TouchingIndex];
            Category[i].x = tt.x * layer.Tileset.TileWidth;
            Category[i].y = tt.y * layer.Tileset.TileHeight;
            if (ClearParameters)
               Category[i].ClearParameters();
            Category[i].ProcessRules();
            break;
         }
      }
      return -1;
   }

   [Description("Change the specified tile that the sprite is touching to another tile. Return the number of tiles affected. (Must run TouchTiles first.)")]
   public int TileChange(int OldTileValue, int NewTileValue, bool InitialOnly)
   {
      Debug.Assert(this.isActive, "Attempted to execute TileChange on an inactive sprite");

      if (TouchedTiles == null)
         return 0;

      int result = 0;

      for (int i=0; i < TouchedTiles.Count; i++)
      {
         TouchedTile tt = (TouchedTile)TouchedTiles[i];
         if ((tt.tileValue == OldTileValue) && (!tt.processed) && (!InitialOnly || tt.initial))
         {
            tt.processed = true;
            layer[tt.x, tt.y] = tt.tileValue = NewTileValue;
            result++;
         }
      }
      return result;
   }

 
   public enum RelativePosition
   {
      TopLeft,
      TopCenter,
      TopRight,
      LeftMiddle,
      CenterMiddle,
      RightMiddle,
      BottomLeft,
      BottomCenter,
      BottomRight
   }

   public System.Drawing.Point GetRelativePosition(RelativePosition RelativePosition)
   {
      System.Drawing.Point rp = new System.Drawing.Point(PixelX, PixelY);

      switch (RelativePosition)
      {
         case RelativePosition.TopCenter:
            rp.X = (int)(PixelX + SolidWidth / 2);
            break;
         case RelativePosition.TopRight:
            rp.X = PixelX + SolidWidth - 1;
            break;
         case RelativePosition.LeftMiddle:
            rp.Y = PixelY + (int)(SolidHeight / 2);
            break;
         case RelativePosition.CenterMiddle:
            rp.X = PixelX + (int)(SolidWidth / 2);
            rp.Y = PixelY + (int)(SolidHeight / 2);
            break;
         case RelativePosition.RightMiddle:
            rp.X = PixelX + SolidWidth - 1;
            rp.Y = PixelY + (int)(SolidHeight / 2);
            break;
         case RelativePosition.BottomLeft:
            rp.Y = PixelY + SolidHeight -1;
            break;
         case RelativePosition.BottomCenter:
            rp.X = PixelX + (int)(SolidWidth / 2);
            rp.Y = PixelY + SolidHeight - 1;
            break;
         case RelativePosition.BottomRight:
            rp.X = PixelX + SolidWidth - 1;
            rp.Y = PixelY + SolidHeight - 1;
            break;
      }
      return rp;
   }

   [Description("Examines the tile on the layer at the sprite's current position and determines if it is a member of the specified category. The RelativePosition parameter determines which part of the sprite to use when identifying a location on the layer. (TouchTiles is not necessary for this function.)")]
   public bool IsOnTile(TileCategoryName Category, RelativePosition RelativePosition)
   {
      System.Drawing.Point rp = GetRelativePosition(RelativePosition);
      return layer.GetTile((int)(rp.X / layer.Tileset.TileWidth), (int)(rp.Y / layer.Tileset.TileHeight)).IsMember(Category);
   }
   #endregion

   #region Activation
   [Description("Deactivate this sprite.  It will no longer be drawn, and in debug mode, will display errors if rules try to execute on it.")]
   public void Deactivate()
   {
      isActive = false;
   }
   #endregion
}