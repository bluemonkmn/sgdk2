using System;
using System.ComponentModel;
using System.Diagnostics;

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
   public readonly LayerBase layer;
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

      if ((x+SolidWidth < RidingOn.oldX) || (x > RidingOn.oldX+RidingOn.SolidWidth) ||
          (y+SolidHeight < RidingOn.oldY - 1) || (y+SolidHeight >= RidingOn.oldY+SolidHeight))
      {
         StopRiding();
         return;
      }

      // Don't try to process the platform's rules if it's already moved.
      // Even though this is already being checked in ProcessRules, circular
      // references (which shouldn't exist) would lead to deadlock.
      if (!RidingOn.Processed)
         // Ensure that the sprite that this sprite is riding moves first
         RidingOn.ProcessRules();

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
         if((oldY + SolidHeight <= spr.oldY) &&
            (y + SolidHeight > spr.y) &&
            (x + SolidWidth > spr.x) &&
            (x < spr.x + spr.SolidWidth))
         {
            RidingOn = spr;
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
      Debug.Assert(this.isActive, "Attempted to execute MaxVelocity on an inactive sprite");
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
   #endregion

   #region States and animation
   [Description("Advance the animation frame of this sprite according to its velocity or a constant rate")]
   public void Animate(SpriteAnimationType Correlation)
   {
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
      if (double.IsNaN(LocalDX))
         useDX = dx;
      else
         useDX = LocalDX;
      if (double.IsNaN(LocalDY))
         useDY = dy;
      else
         useDY = LocalDY;
      return FirstState + ((StateCount + (int)Math.Round(System.Math.Atan2(-useDY,useDX) * StateCount / Math.PI)) % StateCount);
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
            dy = ground - y - SolidHeight;
            hit = true;
         }
      } 
      else if (dy < 0)
      {
         int ceiling = layer.GetBottomSolidPixel(new System.Drawing.Rectangle(PixelX, ProposedPixelY, SolidPixelWidth, PixelY - ProposedPixelY), m_solidity);
         if (ceiling != int.MinValue)
         {
            dy = ceiling - y + 1;
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
                     dy = dyOrig = slopedFloor - 1 - y - SolidHeight;
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
               dx = rightwall - x - SolidWidth;
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
                     dy = dyOrig = slopedFloor - 1 - y - SolidHeight;
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
               dx = leftwall - x + 1;
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
            dy = ground - y - SolidHeight;
            hit = true;
         }
      }
      else if (dy < 0)
      {
         int ceiling = layer.GetBottomSolidPixel(new System.Drawing.Rectangle(ProposedPixelX, ProposedPixelY, ProposedSolidPixelWidth, PixelY - ProposedPixelY), m_solidity);
         if (ceiling != int.MinValue)
         {
            dy = ceiling - y + 1;
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
         double newDy = ground - y - SolidHeight;
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
}