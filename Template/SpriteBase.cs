/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */
using System;
using System.ComponentModel;
using System.Diagnostics;

/// <summary>
/// Base class for all sprite definitions.
/// </summary>
[Serializable()]
public abstract partial class SpriteBase : GeneralRules
{
   /// <summary>
   /// Horizontal coordinate of the sprite within its layer.
   /// </summary>
   /// <remarks>Because a sprite can move at non-integer rates, the position is tracked
   /// as a floating point number. <seealso cref="PixelX"/></remarks>
   public double x;
   /// <summary>
   /// Vertical coordinage of the sprite within its layer.
   /// </summary>
   /// <remarks>Because a sprite can move at non-integer rates, the position is tracked
   /// as a floating point number. <seealso cref="PixelY"/></remarks>
   public double y;
   /// <summary>
   /// Horizontal velocity of the sprite in pixels per frame
   /// </summary>
   /// <remarks>Negative numbers represent leftward movement while positive numbers
   /// represent rightward movement. This value only represents the final velocity
   /// of the sprite relative to the layer. If the sprite is riding a platform,
   /// see <see cref="LocalDX"/>.<seealso cref="dy"/></remarks>
   public double dx;
   /// <summary>
   /// Vertical velocity of the sprite in pixels per frame
   /// </summary>
   /// <remarks>Negative numbers represent upward movement while positive numbers
   /// represent downward movement. This value only represents the final velocity
   /// of the sprite relative to the layer. If the sprite is riding a platform,
   /// see <see cref="LocalDY"/>.<seealso cref="dx"/></remarks>
   public double dy;
   /// <summary>
   /// The value of this sprite's <see cref="x"/> coordinate on the previous frame.
   /// </summary>
   /// <remarks>This value is set during <see cref="MoveByVelocity"/>.
   /// <seealso cref="OldPixelX"/><seealso cref="oldY"/></remarks>
   public double oldX;
   /// <summary>
   /// The value of this sprite's <see cref="y"/> coordinate on the previous frame.
   /// </summary>
   /// <remarks>This value is set during <see cref="MoveByVelocity"/>.
   /// <seealso cref="OldPixelY"/><seealso cref="oldX"/></remarks>
   public double oldY;
   /// <summary>
   /// Numeric value corresponding to the sprite's current state.
   /// </summary>
   /// <remarks>A value of 0 represents the first state listed in this sprite's
   /// list of states. Each subsequent state is the value of the previous plus 1.
   /// You can refer to the sprites State enumeration to refer to states by name, but
   /// the result must be explicitly converted to an integer.
   /// <seealso cref="SwitchToState"/>
   /// </remarks>
   /// <example><c>state = (int)State.Left</c></example>
   public int state;
   /// <summary>
   /// Numeric value corresponding to the sprite's current frame.
   /// </summary>
   /// <remarks>The number of valid frames depend on the current state. Each state
   /// has its own sequence of frames. The frame that is displayed depends both on
   /// the number of frames and the repeat count of each frame. <seealso cref="Animate"/></remarks>
   public int frame;
   /// <summary>
   /// Stores the inputs that are currently being "pressed" on this sprite.
   /// </summary>
   /// <remarks>Normally the inputs of a sprite are set by a player, but could
   /// also by explicitly set by rules and other input sources such as saved
   /// input (replaying a previous input sequence). You can cause the sprite
   /// to accelerate in the directions dictated by the input bits with
   /// <see cref="AccelerateByInputs"/>.<seealso cref="AccelerateByInputs"/>
   /// <seealso cref="IsInputPressed"/><seealso cref="SetInputState"/>
   /// <seealso cref="ClearInputs"/><seealso cref="MapKeyToInput"/>
   /// <seealso cref="MapPlayerToInputs"/><seealso cref="oldinputs"/></remarks>
   public InputBits inputs;
   /// <summary>
   /// Remembers the inputs that were active on this sprite in the previous frame
   /// </summary>
   /// <remarks>This value is used to determine when a player (or other input source)
   /// has just started pressing an input versus continued pressing an input that was
   /// already pressed. It is copied from <see cref="inputs"/> when
   /// <see cref="MapPlayerToInputs"/> is called.<seealso cref="inputs"/>
   /// <seealso cref="MapPlayerToInputs"/><seealso cref="IsInputPressed"/></remarks>
   public InputBits oldinputs;
   /// <summary>
   /// Determines if this sprite is currently active.
   /// </summary>
   /// <remarks>Only active sprites will be drawn and process rules. Attempting to
   /// refer to an inactive sprite from an active rule (on an active sprite or plan)
   /// will result in an error message in debug mode. <seealso cref="Deactivate"/>
   /// <seealso cref="TileActivateSprite"/></remarks>
   public bool isActive;
   private LayerBase layer;
   private Solidity m_solidity;
   /// <summary>
   /// A combination of <see cref="ModulateRed"/>, <see cref="ModulateGreen"/>,
   /// <see cref="ModulateBlue"/> and <see cref="ModulateAlpha"/>.
   /// </summary>
   /// <remarks><para>This member stores the actual data for all 4 of the Modulate properties
   /// that affect the sprite's color. Using the Modulate properties is easier to
   /// read and understand, but setting this property directly is simple and faster
   /// in terms of the amount of code.</para>
   /// <para>This number consists of 4 bytes, which are, in order from least significant
   /// to most significant: Blue, Greed, Red, Alpha.  To to retrieve the alpha component of
   /// this value, you would divide it by 16777216 or shift the bits rightward 24 places.
   /// </para></remarks>
   public int color;

   /// <summary>
   /// Horizontal velocity relative to the sprite's environment (like a platform)
   /// </summary>
   /// <remarks>If the sprite is riding a platform, changes to the sprite's velocity
   /// should affect this instead of <see cref="dx"/>. When the sprite is not riding
   /// a platform, this value will be set to double.NaN.</remarks>
   public double LocalDX;
   /// <summary>
   /// Vertical velocity relative to the sprite's environment
   /// </summary>
   /// <remarks>If the sprite is riding an object within which it can move vertically
   /// (platforms only allow the sprite to move within the platform horizontally), changes
   /// to the sprite's velocity should affect this instead of <see cref="dy"/>.
   /// When the sprite is not riding such an object, this value will be set to double.NaN.</remarks>
   public double LocalDY;

   /// <summary>
   /// Contains all the possible bits that can be set in a sprite's <see cref="inputs"/> and
   /// <see cref="oldinputs"/> properties.
   /// </summary>
   /// <remarks>Multiple bits may be set at once if multiple inputs are being pressed on this
   /// sprite at the same time.</remarks>
   [FlagsAttribute()]
   public enum InputBits
   {
      /// <summary>
      /// Refers to an input that causes the sprite to move up or accelerate
      /// </summary>
      Up=1,
      /// <summary>
      /// Refers to an input that causes the sprite to move or turn right
      /// </summary>
      Right=2,
      /// <summary>
      /// Refers to an input that causes the sprite to move down or decelerate
      /// </summary>
      Down=4,
      /// <summary>
      /// Refers to an input that causes the sprite to move or turn left
      /// </summary>
      Left=8,
      /// <summary>
      /// Refers to 1 of 4 customizable inputs on the sprite.
      /// </summary>
      Button1=16,
      /// <summary>
      /// Refers to 1 of 4 customizable inputs on the sprite.
      /// </summary>
      Button2=32,
      /// <summary>
      /// Refers to 1 of 4 customizable inputs on the sprite.
      /// </summary>
      Button3=64,
      /// <summary>
      /// Refers to 1 of 4 customizable inputs on the sprite.
      /// </summary>
      Button4=128
   }

   /// <summary>
   /// Used to refer to 4 primary directions.
   /// </summary>
   public enum Direction
   {
      Up,
      Right,
      Down,
      Left
   }

   /// <summary>
   /// Defines the basis for a sprite's animation
   /// </summary>
   public enum SpriteAnimationType
   {
      /// <summary>
      /// Each frame that passes in the game will cause the sprite to advance its animation by one frame.
      /// </summary>
      ByFrame,
      /// <summary>
      /// The sprite's animation will advance according to how many pixels it will move horizontally each frame.
      /// </summary>
      ByHorizontalVelocity,
      /// <summary>
      /// The sprite's animation will advance according to how many pixels it will move vertically each frame.
      /// </summary>
      ByVerticalVelocity,
      /// <summary>
      /// The sprite's animation will advance according to how many pixels it moves each frame in any direction (using the distance formula).
      /// </summary>
      ByVectorVelocity
   }

   /// <summary>
   /// Constructs a new sprite instance given all its base properties.
   /// </summary>
   /// <param name="layer">Layer that contains the sprite.</param>
   /// <param name="x">Initial horizontal coordinate within the layer</param>
   /// <param name="y">Initial vertical coordinate within the layer</param>
   /// <param name="dx">Initial horizontal velocity</param>
   /// <param name="dy">Initial vertical velocity</param>
   /// <param name="state">Initial state</param>
   /// <param name="frame">Initial frame within the initial state</param>
   /// <param name="active">Determines if the sprite is initially active</param>
   /// <param name="solidity">Which solidity definition does the sprite initially react to</param>
   /// <param name="color">Initial color modulation settings</param>
   /// <remarks><para>This cannot be called directly because SpriteBase is an abstract class,
   /// but it is called by the derived constructors on each individual sprite definition.</para>
   /// <para>The sprite assumes that this is the layer to which it is added and will use it when
   /// looking for other objects or properties in the layer.</para></remarks>
   public SpriteBase(LayerBase layer, double x, double y, double dx, double dy, int state, int frame, bool active, Solidity solidity, int color)
   {
      this.layer = layer;
      this.x = this.oldX = x;
      this.y = this.oldY = y;
      this.dx = dx;
      this.dy = dy;
      this.state = state;
      this.frame = frame;
      this.isActive = active;
      this.m_solidity = solidity;
      this.color = color;
      this.LocalDX = double.NaN;
      this.LocalDY = double.NaN;
   }

   #region Properties
   /// <summary>
   /// Horizontal position of the sprite within the layer rounded toward zero to yield an integer.
   /// </summary>
   /// <remarks>This can only be changed by changing the sprite's <see cref="dx"/> property.</remarks>
   public int PixelX
   {
      get
      {
         Debug.Assert(this.isActive, "Attempted to access PixelX on an inactive sprite");
         // If you don't have the left edge of your map protected by a solid boundary,
         // and the different behavior of the left edge is bugging you, you can use this
         // return statement to work around the rounding difference of negative numbers,
         // but it involves just a bit of unnecessary overhead, and looks rather clumsy:
         // return (int)(x+16)-16;
         return (int)x;
      }
   }

   /// <summary>
   /// Vertical position of the sprite within the layer rounded toward zero to yield an integer.
   /// </summary>
   /// <remarks>This can only be changed by changing the sprite's <see cref="dy"/> property.</remarks>
   public int PixelY
   {
      get
      {
         Debug.Assert(this.isActive, "Attempted to access PixelY on an inactive sprite");
         return (int)y;
      }
   }

   /// <summary>
   /// The horizontal coordinate that the sprite is expected to be at on the next frame based on its
   /// current position and velocity, rounded toward zero to yield an integer pixel coordinate.
   /// <seealso cref="x"/><seealso cref="dx"/>
   /// </summary>
   public int ProposedPixelX
   {
      get
      {
         Debug.Assert(this.isActive, "Attempted to access ProposedPixelX on an inactive sprite");
         // If you don't have the left edge of your map protected by a solid boundary,
         // and the different behavior of the left edge is bugging you, you can use this
         // return statement to work around the rounding difference of negative numbers,
         // but it involves just a bit of unnecessary overhead, and looks rather clumsy:
         // return (int)(x+dx+16)-16
         return (int)(x+dx);
      }
   }

   /// <summary>
   /// The vertical coordinate that the sprite is expected to be at on the next frame based on its
   /// current position and velocity, rounded toward zero to yield an integer pixel coordinate.
   /// <seealso cref="y"/><seealso cref="dy"/>
   /// </summary>
   public int ProposedPixelY
   {
      get
      {
         Debug.Assert(this.isActive, "Attempted to access ProposedPixelY on an inactive sprite");
         return (int)(y+dy);
      }
   }

   /// <summary>
   /// The horizontal pixel coordinate that the sprite was at on the previous frame
   /// </summary>
   /// <remarks>A pixel coordinate is the actual coordinate (<see cref="oldX"/> in this case)
   /// rounded toward zero to yield an integer.</remarks>
   public int OldPixelX
   {
      get
      {
         Debug.Assert(this.isActive, "Attempted to access OldPixelX on an inactive sprite");
         return (int)oldX;
      }
   }

   /// <summary>
   /// The vertical pixel coordinate that the sprite was at on the previous frame
   /// </summary>
   /// <remarks>A pixel coordinate is the actual coordinate (<see cref="oldY"/> in this case)
   /// rounded toward zero to yield an integer.</remarks>
   public int OldPixelY
   {
      get
      {
         Debug.Assert(this.isActive, "Attempted to access OldPixelY on an inactive sprite");
         return (int)oldY;
      }
   }

   /// <summary>
   /// Retrieves information about the state that the sprite is currently in.
   /// </summary>
   public SpriteState CurrentState
   {
      get
      {
         Debug.Assert(this.isActive, "Attempted to access CurrentState on an inactive sprite");
         return this[state];
      }
   }

   /// <summary>
   /// Retrieves information about the layer in which the sprite resides.
   /// </summary>
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
   /// How many pixels wide is the solid area of this sprite.
   /// </summary>
   /// <remarks>The solid area of the sprite is the area that avoids overlapping
   /// solid areas of the map. The width is measured from the origin and
   /// extends rightward. <seealso cref="SolidHeight"/></remarks>
   public abstract int SolidWidth
   {
      [Description("Returns the width of the sprite's solid area")]
      get;
   }

   /// <summary>
   /// How many pixels high is the solid area of this sprite. 
   /// </summary>
   /// <remarks>The solid area of the sprite is the area that avoids overlapping
   /// solid areas of the map. The height is measured from the origin and
   /// extends downward. <seealso cref="SolidWidth"/></remarks>
   public abstract int SolidHeight
   {
      [Description("Returns the height of the sprite's solid area")]
      get;
   }

   /// <summary>
   /// Returns information about a specified state of this sprite
   /// </summary>
   /// <example>The following code shows how you might retrieve the height
   /// of a sprite's crouching state assuming it has one:
   /// <code>crouchHeight = this[State.Crouch].LocalBounds.Height;</code></example>
   public abstract SpriteState this[int state]
   {
      get;
   }

   /// <summary>
   /// Executes all the rules associated with this sprite.
   /// </summary>
   /// <remarks><see cref="ProcessRules"/> is the recommended alternative to execute the sprite's
   /// rules only once per frame by only calling ExecuteRules when the sprites rules haven't
   /// already been executed this frame. <seealso cref="Processed"/></remarks>
   protected abstract void ExecuteRules();

   /// <summary>
   /// Resets all parameter values defined on the specific sprite definition to 0.
   /// </summary>
   /// <remarks>This is helpful in resetting a sprite's state without knowing specifically
   /// what its parameters are.  Then the sprite can internally re-initialize itself when
   /// it sees that it's parameters have been reset to 0.</remarks>
   public abstract void ClearParameters();

   /// <summary>
   /// Remove the sprite from its designated categories.
   /// USE ONLY on dynamically added sprites.
   /// </summary>
   /// <remarks>This is used by functions involved with the creation and deactivation of
   /// dynamically created sprites to ensure that a deactivated sprite no longer exists
   /// in any categories/collections. It's called by <see cref="SpriteCollection.Clean"/>,
   /// which is called by <see cref="LayerBase.ProcessSprites"/>.</remarks>
   public abstract void RemoveFromCategories();

   #endregion

   #region Public Methods
   /// <summary>
   /// Returns information about the visual boundaries of the sprite's currently displayed frame(s).
   /// </summary>
   /// <returns>Rectangle object containing layer-relative coordinates that encompass the
   /// sprite's current image.</returns>
   public System.Drawing.Rectangle GetBounds()
   {
      Debug.Assert(this.isActive, "Attempted to execute GetBounds on an inactive sprite");
      System.Drawing.Rectangle result = CurrentState.LocalBounds;
      result.Offset(PixelX, PixelY);
      return result;
   }

   /// <summary>
   /// Get a list of frameset frames that the sprite is currently displaying
   /// </summary>
   /// <returns>
   /// Array of Frame objects representing the currently displayed frames.
   /// </returns>
   /// <remarks>The array will only have more than one element if the sprite
   /// is currently displaying a composite frame.  The frames are ordered from
   /// background to foreground.</remarks>
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
   /// <remarks>This checks <see cref="Processed"/> and calls <see cref="ExecuteRules"/>
   /// if it's not set, setting <see cref="Processed"/> to true first.</remarks>
   public void ProcessRules()
   {
      if (!Processed)
      {
         // Help prevent infinite recursion
         Processed = true;
         ExecuteRules();
      }
   }

   #endregion

   #region Sprite Interaction
   #region Rider Feature
   /// <summary>
   /// Stores the platform sprite (the sprite that this sprite rides on).
   /// If not set, then the sprite is not riding anything.
   /// </summary>
   public SpriteBase RidingOn;
   /// <summary>
   /// Determines if this sprite's definition's rules have been processed yet this frame
   /// </summary>
   [NonSerialized()]
   public bool Processed;

   /// <summary>
   /// Adjust this sprite's velocity according to the motion of the platform it is riding
   /// </summary>
   /// <remarks>Apply this rule before <see cref="ReactToSolid"/> in order to prevent
   /// the platform from allowing the sprite to move through solids.</remarks>
   [Description("Moves this sprite according to the motion of the platform it is riding.")]
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
         (y+SolidHeight < RidingOn.oldY - 1) || (y+SolidHeight >= RidingOn.oldY+RidingOn.SolidHeight))
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
   /// Tests to see if this sprite is landing on a platform (from above), and if so,
   /// make the sprite ride the platform.
   /// </summary>
   /// <param name="PlatformList">List of platform sprites to check</param>
   /// <returns>True if the sprite landed on a platform, False if it is already riding a platform or doesn't need to</returns>
   /// <remarks>
   /// This should be called after sprites are moved, but before
   /// they are drawn.</remarks>
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
   
   /// <summary>
   /// Determine whether the sprite's collision mask is overlapping part of any sprite in the specified category.
   /// </summary>
   /// <param name="Targets">A sprite category containing sprites that will be checked for collision
   /// with this sprite.</param>
   /// <returns>The index of the sprite within the category if a collision is occurring, otherwise -1.</returns>
   /// <remarks>The collision mask is derived from the sprite's Mask Alpha Level setting.
   /// If both sprites have a collision mask, they are checked for overlapping solid bits.
   /// If one sprite has Mask Alpha Level set to 0, then a rectangular mask for that sprite
   /// is synthesized from the solid width and solid height using
   /// <see cref="CollisionMask.GetRectangularMask"/>.
   /// If both sprites have Mask Alpha Level set to 0, then a simple rectangular collision
   /// detection is performed (for improved performance).</remarks>
   [Description("Determine whether the sprite's collision mask is overlapping part of any sprite in the specified category. Return the index of the sprite within the category if a collision is occurring, otherwise return -1.")]
   public int TestCollisionMask(SpriteCollection Targets)
   {
      if (!isActive)
         return -1;
      CollisionMask sourceMask = CurrentState.GetMask(frame);
      bool bSourceIsRectangle = false;
      System.Drawing.Rectangle sourceRect = new System.Drawing.Rectangle(PixelX, PixelY, SolidWidth, SolidHeight);
      if (sourceMask == null)
      {
         sourceMask = CollisionMask.GetRectangularMask(new System.Drawing.Size(SolidWidth, SolidHeight));
         bSourceIsRectangle = true;
      }
      for (int idx = 0; idx < Targets.Count; idx++)
      {
         SpriteBase TargetSprite = Targets[idx];
         if (TargetSprite == this)
            continue;
         if (TargetSprite.isActive)
         {
            CollisionMask targetMask = TargetSprite.CurrentState.GetMask(TargetSprite.frame);
            if (targetMask == null)
            {
               if (bSourceIsRectangle)
               {
                  if (sourceRect.IntersectsWith(new System.Drawing.Rectangle(
                     TargetSprite.PixelX, TargetSprite.PixelY, TargetSprite.SolidWidth, TargetSprite.SolidHeight)))
                     return idx;
                  continue;
               }
               else
                  targetMask = CollisionMask.GetRectangularMask(new System.Drawing.Size(TargetSprite.SolidWidth, TargetSprite.SolidHeight));
            }
            if (sourceMask.TestCollisionWith(targetMask, TargetSprite.PixelX - PixelX, TargetSprite.PixelY - PixelY))
               return idx;
         }         
      }
      return -1;
   }

   /// <summary>
   /// Determine whether the solidity rectangle of the sprite overlaps that of any sprite in the specified category.
   /// </summary>
   /// <param name="Targets">A sprite category containing sprites that will be checked for collision
   /// with this sprite.</param>
   /// <returns>The index of the sprite within the category if a collision is occurring, otherwise -1.</returns>
   /// <remarks>This can be used to force a simple rectangular collision test even if one or both
   /// sprites involved have a Mask Alpha level greater than 0.  This method is recommended
   /// for improved performance when pixel-perfect collision detection is not required.
   /// <seealso cref="TestCollisionMask"/></remarks>
   [Description("Determine whether the solidity rectangle of the sprite overlaps that of any sprite in the specified category. Return the index of the sprite within the category if a collision is occurring, otherwise return -1.")]
   public int TestCollisionRect(SpriteCollection Targets)
   {
      if (!isActive)
         return -1;
      for (int idx = 0; idx < Targets.Count; idx++)
      {
         SpriteBase TargetSprite = Targets[idx];
         if ((TargetSprite == this) || (!TargetSprite.isActive))
            continue;
         int x1 = PixelX;
         int w1 = SolidWidth;
         int x2 = TargetSprite.PixelX;
         int w2 = TargetSprite.SolidWidth;
         int y1 = PixelY;
         int h1 = SolidHeight;
         int y2 = TargetSprite.PixelY;
         int h2 = TargetSprite.SolidHeight;

         if ((x1+w1 > x2) && (x2+w2 > x1) && (y1+h1 > y2) && (y2+h2 > y1))
            return idx;
      }
      return -1;
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
   /// <remarks>The <see cref="oldX"/> and <see cref="oldY"/> properties are
   /// set from the current position and a new position is calculated into
   /// <see cref="x"/> and <see cref="y"/> by adding <see cref="dx"/> and
   /// <see cref="dy"/> to them respectively.</remarks>
   [Description("Move this sprite according to its current velocity")]
   public void MoveByVelocity()
   {
      Debug.Assert(this.isActive, "Attempted to execute MoveByVelocity on an inactive sprite");
      oldX = x;
      oldY = y;
      x += dx;
      y += dy;
   }

   /// <summary>
   /// If the velocity of the sprite is more than the specified maximum, normalize it so that
   /// it's going the same direction, but at no more that the specified maximum speed.
   /// </summary>
   /// <param name="Maximum">Specifies the maximum speed in pixels per frame</param>
   /// <remarks><para>The speed of the sprite is calculated with the distance formula, so a sprite
   /// moving 3 pixels horizontally and 4 pixels vertically, for example, is considered to be
   /// moving 5 pixels.</para>
   /// <para>If the sprite is riding a platform, the maximum velocity is applied relative to
   /// the platform. For example, if the platform is moving rightward at 3 pixels per frame,
   /// and the sprite is moving rightward at 5 pixels per frame relative to the platform (8
   /// pixels per frame in absolute terms), and the maximum is set to 4 pixels per frame,
   /// the sprite's <see cref="LocalDX"/> will be reduced to 4 rather than to 1</para>.</remarks>
   [Description("Limit the velocity of the sprite to the specified maximum pixels per frame (affects only local velocity when applicable)")]
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

   /// <summary>
   /// Reduces the sprite's velocity to simulate friction.
   /// </summary>
   /// <param name="RetainPercentVertical">What percent (0 to 100) of the sprite's horizontal velocity
   /// (<see cref="dx"/>) is retained.</param>
   /// <param name="RetainPercentHorizontal">What percent (0 to 100) of the sprite's vertical
   /// velocity (<see cref="dy"/>) is retained.</param>
   /// <remarks>Note that inertia may have unexpected side-effects on gravity.  For example,
   /// if RetainPercentVertical is set to zero, this will cause the sprite to start from a
   /// vertical velocity of 0 on every frame, which will result in gravity being unable to
   /// accumulate any effect.  It's best to set RetainPercentVertical to a high number or 100
   /// when gravity is in effect. A gravity effect is simply an automatic adjustment to the
   /// Y velocity with <see cref="AlterYVelocity"/>.</remarks>
   [Description("Reduces the sprite's velocity to simulate friction.  RetainPercent is a number 0 to 100 indicating how much inertia is retained.")]
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

   /// <summary>
   /// Determines if the sprite is moving in the specified direction at all.
   /// </summary>
   /// <param name="Direction">Determines which direction to check</param>
   /// <returns>True if the sprite has any movement in the specified direction, otherwise false.</returns>
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

   /// <summary>
   /// Accelerate a rotating sprite in the direction that it is aiming.
   /// </summary>
   /// <param name="Acceleration">Specifies acceleration amount in tenths of a pixel per frame per frame.</param>
   /// <param name="FirstState">Provide the first state of this sprite's group of states (representing the sprite pointing rightward).</param>
   /// <param name="StateCount">Provide the number of states this sprite takes to represent one full rotation.</param>
   /// <remarks>The current angle of this sprite is calculated by assuming that the provided first state
   /// points rightward, and that each subsequent state rotates the sprite counter-clockwise in equal
   /// increments until StateCount states have resulted in one full rotation. The sprite's current state
   /// is somewhere in that progression, and determines the current angle. The actual formula is
   /// (state - FirstState) * pi * 2 / StateCount, which calculates the angle in radians.
   /// <seealso cref="RotateVelocity"/></remarks>
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
   
   /// <summary>
   /// Redirect this sprite's velocity to be 100% in the direction that it is facing.
   /// </summary>
   /// <param name="FirstState">Provide the first state of this sprite's group of states (representing the sprite pointing rightward).</param>
   /// <param name="StateCount">Provide the number of states this sprite takes to represent one full rotation.</param>
   /// <remarks><paramref name="FirstState" /> must represent the sprite in a rightward-aiming state, and each subsequent
   /// state must represent an equal counter-clockwise rotation until <paramref name="StateCount" /> states result in
   /// the sprite pointing almost rightward again. The sprite's current state must be one of the
   /// states in that range. This function is useful for car-type sprites where turning the sprite should automatically
   /// cause the sprite to be moving in that direction only (no drifting in the previous direction
   /// like a space ship would). <seealso cref="PolarAccelerate"/></remarks>
   [Description("Redirect this sprite's velocity to be 100% in the direction that it is facing, based on its state, where FirstState points rightward and the number of states rotate counterclockwise.")]
   public void RotateVelocity([Editor("SpriteState", "UITypeEditor")] int FirstState, int StateCount)
   {
      Debug.Assert(this.isActive, "Attepmted to execute RotateVelocity on an inactive sprite");
      float oldDx, oldDy;
      if (double.IsNaN(LocalDX))
         oldDx = (float)dx;
      else
         oldDx = (float)LocalDX;
      if (double.IsNaN(LocalDY))
         oldDy = (float)dy;
      else
         oldDy = (float)LocalDY;

      float angle = (float)((state - FirstState) * Math.PI * 2 / (float)StateCount);

      float facingX, facingY;
      facingX = (float)Math.Cos(angle);
      facingY = -(float)Math.Sin(angle);
      float dotProduct = oldDx * facingX + oldDy * facingY;
      facingX *= dotProduct;
      facingY *= dotProduct;

      if (double.IsNaN(LocalDX))
         dx = facingX;
      else
         LocalDX = facingX;

      if (double.IsNaN(LocalDY))
         dy = facingY;
      else
         LocalDY = facingY;
   }

   /// <summary>
   /// Scroll all layers on this sprite's layer's map so that the sprite is within visible area of the map.
   /// </summary>
   /// <param name="UseScrollMargins">If true, scroll the sprite into the scroll margins of the map. Otherwise just scroll the layers var enough so the sprite is within the edges of the display.</param>
   /// <remarks>For a multi-player game where both players are in the same view, you may want to
   /// set UseScrollMargins to false in order to allow the players to get closer to the edge of the
   /// display when moving apart from one another. This function will not affect layers with a scroll
   /// rate of zero. <seealso cref="PushSpriteIntoView"/>
   /// </remarks>
   [Description("Scroll all layers on this sprite's layer's map so that the sprite is within visible area of the map.  If UseScrollMargins is true, scroll the sprite into the scroll margins of the map.")]
   public void ScrollSpriteIntoView(bool UseScrollMargins)
   {
      ParentLayer.ScrollSpriteIntoView(this, UseScrollMargins);
   }

   /// <summary>
   /// Alter this sprite's velocity so that it remains within the map's visible area or
   /// within the scroll margins, according to this sprite's layer's position within the map.
   /// </summary>
   /// <param name="StayInScrollMargins">If true, the sprite will be pushed toward the center as
   /// soon as it comes within a minimum distance of the edge of the display, determined by the
   /// map's scroll margins (<see cref="MapBase.ScrollMarginLeft"/>, <see cref="MapBase.ScrollMarginTop"/>,
   /// <see cref="MapBase.ScrollMarginRight"/>, <see cref="MapBase.ScrollMarginBottom"/>). Otherwise
   /// the sprite will only be pushed when it reaches the edge of the display.</param>
   /// <remarks>This takes into account the sprites <see cref="LocalDX"/> and <see cref="LocalDY"/>
   /// properties in case it is riding another sprite. If the sprite has somehow gone far off the
   /// edge of the map, it's velocity may be set to unreasonable values because it will attempt
   /// to enter the display in a single frame. This function will only set the velocity, not
   /// actually move the sprite (<see cref="MoveByVelocity"/> is used to move sprites). This
   /// allows you to make the sprite react to many forces before moving it. In a multi-player
   /// game with two players in the same view this is useful for ensuring that both players
   /// remain on screen.
   /// <seealso cref="ScrollSpriteIntoView"/></remarks>
   [Description("Alter this sprite's velocity so that it remains within the map's visible area or within the scroll margins, according to this sprite's layer's position within the map.")]
   public void PushSpriteIntoView(bool StayInScrollMargins)
   {
      ParentLayer.PushSpriteIntoView(this, StayInScrollMargins);
   }

   /// <summary>
   /// Compute the index of the nearest active sprite from the specified category and return it.
   /// </summary>
   /// <param name="Target">Sprite category whose sprites will be searched for sprites near this sprite.</param>
   /// <returns>An integer representing the 0-based index of the nearest active sprite, or -1 if
   /// there are no active sprites in the specified category.</returns>
   /// <remarks>The output of this function would commonly be stored in a sprite parameter for
   /// passing to <see cref="PushTowardCategory "/>.</remarks>
   [Description("Compute the index of the nearest active sprite from the specified category and return it.")]
   public int GetNearestSpriteIndex(SpriteCollection Target)
   {
      int minDist = int.MaxValue;
      int result = -1;
      for(int i = 0; i < Target.Count; i++)
      {
         if (!Target[i].isActive)
            continue;
         int xOff = Target[i].PixelX - PixelX;
         int yOff = Target[i].PixelY - PixelY;
         int dist = xOff * xOff + yOff * yOff;
         if (dist < minDist)
         {
            minDist = dist;
            result = i;
         }
      }
      return result;
   }

   /// <summary>
   /// Push this sprite toward a sprite in the specified category.
   /// </summary>
   /// <param name="Target">Specifies a category containint the target
   /// sprite toward which this sprite will be pushed.</param>
   /// <param name="Index">Specifies the 0-based index of a sprite in the specified category.
   /// Use <see cref="GetNearestSpriteIndex"/> to compute the index of the nearest sprite, which
   /// can then be passed to this parameter.  Pass -1 in this parameter to push the sprite toward
   /// the current nearest sprite rather than a pre-computed index.</param>
   /// <param name="Force">Force in tenths of a pixel per frame per frame that will be applied.</param>
   /// <returns>True if the sprite was pushed, or false if there are no active sprites in the
   /// target category or the sprite is already overlapping the target.</returns>
   /// <remarks>If there is no active sprite in the target category, or if the sprite is overlapping
   /// the target exactly, this function will have no effect.
   /// <seealso cref="SetInputsTowardCategory"/>
   /// <seealso cref="PushTowardSprite"/>
   /// <seealso cref="SetInputsTowardSprite"/></remarks>
   [Description("Push this sprite toward a sprite in the specified category. Use GetNearestSpriteIndex to compute the index of the nearest sprite and pass that to Index, or pass -1 to push toward the current nearest sprite. Force is in tenths of a pixel per frame per frame.")]
   public bool PushTowardCategory(SpriteCollection Target, int Index, int Force)
   {
      Debug.Assert(this.isActive, "Attepmted to execute PushTowardCategory on an inactive sprite");
      Debug.Assert(Index < Target.Count, "Attempted to PushTowardCategory on an index beyond the bounds of a collection");

      if (Index < 0)
         Index = GetNearestSpriteIndex(Target);
      if (Index < 0)
         return false;

      return PushTowardSprite(Target[Index], Force);
   }

   /// <summary>
   /// Push this sprite toward a specified sprite.
   /// </summary>
   /// <param name="Target">Sprite toward which this sprite is pushed.</param>
   /// <param name="Force">Acceleration force in tenths of a pixel per frame per frame.</param>
   /// <returns>True if the sprite is pushed or false if the sprite is already overlapping
   /// the target.
   /// <seealso cref="PushTowardCategory"/>
   /// <seealso cref="SetInputsTowardSprite"/>
   /// <seealso cref="SetInputsTowardCategory"/></returns>
   public bool PushTowardSprite(SpriteBase Target, int Force)
   {
      double vx = Target.PixelX - PixelX + (Target.SolidWidth - SolidWidth) / 2;
      double vy = Target.PixelY - PixelY + (Target.SolidHeight - SolidHeight) / 2;
      double dist = Math.Sqrt(vx * vx + vy + vy);
      if (dist >= 1)
      {
         dx += vx * Force / dist / 10.0;
         dy += vy * Force / dist / 10.0;
         return true;
      }
      return false;
   }
   #endregion

   #region States and animation

   /// <summary>
   /// Advance the animation frame of this sprite according to its velocity or a constant rate.
   /// </summary>
   /// <param name="Correlation">Determines how and if the sprite's animation speed should be
   /// based on its movement.</param>
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

   /// <summary>
   /// Calculate the state that a rotating sprite should use in order to point in the direction it is currently traveling
   /// </summary>
   /// <param name="FirstState">Provide the first state of this sprite's group of states (representing the sprite pointing rightward).</param>
   /// <param name="StateCount">Provide the number of states this sprite takes to represent one full rotation.</param>
   /// <returns><paramref name="FirstState" /> must represent the sprite in a rightward-aiming state, and each subsequent
   /// state must represent an equal counter-clockwise rotation until <paramref name="StateCount" /> states result in
   /// the sprite pointing almost rightward again. The returned state number will be in that range.
   /// The result of this can be stored directly into <see cref="state"/> if you want the sprite
   /// to switch to that state.</returns>
   [Description("Return the state that a rotating sprite should use in order to point in the direction it is currently traveling, assuming that FirstState points rightward and each subsequent state is one step counter-clockwise")]
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

   /// <summary>
   /// Return a state that is in the same group of states as the sprite's current state,
   /// but possibly pointing a different direction.
   /// </summary>
   /// <param name="FirstState">Represents the beginning of this sprite's rotating states.
   /// This will usually be the sprite's first state, but can be something else if the sprite
   /// has other states before the first rotating state.</param>
   /// <param name="StateCount">Number of states in each rotation</param>
   /// <param name="NewState">Specifies the new state. The state group which this value specifies
   /// is ignored; only the offset into the group is used.</param>
   /// <returns>New state value  that represents the state in the same group as the sprite's
   /// current state, but rotated according to NewState.</returns>
   /// <remarks>This is commonly used in calculating a new state when left or right is
   /// pressed on a rotating sprite, to ensure that pressing left and right don't
   /// cause the sprite to change to a different group of states.</remarks>
   [Description("Return a state that is in the same group of states as the sprite's current state, but possibly pointing a different direction.")]
   public int CalculateRotatedState([Editor("SpriteState", "UITypeEditor")] int FirstState, int StateCount, int NewState)
   {
      Debug.Assert(this.isActive, "Attempted to execute CalculateRotatedState on an inactive sprite");
      return (NewState + StateCount - FirstState) % StateCount + FirstState + (int)((state - FirstState) / StateCount) * StateCount;
   }

   /// <summary>
   /// Switch the sprite to the the specified state, ensuring that the sprite doesn't hit a solid.
   /// </summary>
   /// <param name="State">State to which the sprite will be switched</param>
   /// <param name="Alignment">Specifies a point within the sprite which will remain constant.  For example, RelativePosition.BottomCenter will ensure that the bottom center point of the new state aligns with the bottom center point of this state.</param>
   /// <returns>True if the state could be switched, false if the new state's differing size would
   /// have caused the sprite the sprite to pass through a solid tile.</returns>
   /// <remarks>This function is handy for performing action like switching from a crouching
   /// state to a standing state because it will prevent you from standing up if there is a low
   /// ceiling in the way. It also helps you align the two states properly. For example, if you
   /// switched from the crouching state to the standing state without moving the sprite at
   /// all, the origin (usually the top left corner of the sprite) would remain in the same place,
   /// which would cause the head to remain in the same place and the feet to stick through the
   /// floor. But by using the Alignment paremeter, you can make sure that the botton of the
   /// new state aligns with the bottom of the current state.</remarks>
   [Description("Switch the sprite to the the specified state, ensuring that the specified alignment point in the new state lines up with the same point in the current state.  Returns false if the state could not switch due to solidity.")]
   public bool SwitchToState([Editor("SpriteState", "UITypeEditor")] int State, RelativePosition Alignment)
   {
      Debug.Assert(this.isActive, "Attempted to execute SwitchToState on an inactive sprite");
      System.Drawing.Rectangle oldRect = new System.Drawing.Rectangle(PixelX, PixelY, SolidWidth, SolidHeight);
      int newWidth = this[State].SolidWidth;
      int newHeight = this[State].SolidHeight;
      double newX, newY;
      switch(Alignment)
      {
         case RelativePosition.TopCenter:
         case RelativePosition.CenterMiddle:
         case RelativePosition.BottomCenter:
            newX = x + (oldRect.Width - newWidth) / 2f;
            break;
         case RelativePosition.TopRight:
         case RelativePosition.RightMiddle:
         case RelativePosition.BottomRight:
            newX = x + oldRect.Width - newWidth;
            break;
         default:
            newX = x;
            break;
      }
      switch(Alignment)
      {
         case RelativePosition.LeftMiddle:
         case RelativePosition.CenterMiddle:
         case RelativePosition.RightMiddle:
            newY = y + (oldRect.Height - newHeight) / 2f;
            break;
         case RelativePosition.BottomLeft:
         case RelativePosition.BottomCenter:
         case RelativePosition.BottomRight:
            newY = y + oldRect.Height - newHeight;
            break;
         default:
            newY = y;
            break;
      }

      if (((int)Math.Ceiling(newY + newHeight) > oldRect.Bottom) && (layer.GetTopSolidPixel(new System.Drawing.Rectangle(
         (int)newX, oldRect.Bottom, newWidth, (int)Math.Ceiling(newY) + newHeight - oldRect.Bottom), m_solidity) != int.MinValue))
         return false;

      if (((int)newY < oldRect.Top) && (layer.GetBottomSolidPixel(new System.Drawing.Rectangle(
         (int)newX, (int)newY, newWidth, oldRect.Top - (int)newY), m_solidity) != int.MinValue))
         return false;

      if (((int)newX < oldRect.Left) && (layer.GetRightSolidPixel(new System.Drawing.Rectangle(
         (int)newX, (int)newY, oldRect.Left - (int)newX, newHeight), m_solidity) != int.MinValue))
         return false;

      if (((int)Math.Ceiling(newX + newWidth) > oldRect.Right) && (layer.GetLeftSolidPixel(new System.Drawing.Rectangle(
         oldRect.Right, (int)newY, (int)Math.Ceiling(newX) + newWidth - oldRect.Right, newHeight), m_solidity) != int.MinValue))
         return false;

      x = newX;
      y = newY;
      state = State;
      return true;
   }

   /// <summary>
   /// Determines if the sprite is in the specified range of states.
   /// </summary>
   /// <param name="FirstState">Minimum state number</param>
   /// <param name="LastState">Maximum state number</param>
   /// <returns>Returns true if the sprite's current state is equal to <paramref name="FirstState" /> or
   /// <paramref name="LastState" /> or any state in between. False otherwise.</returns>
   [Description("Determines if the sprite is in the specified range of states")]
   public bool IsInState([Editor("SpriteState", "UITypeEditor")] int FirstState, [Editor("SpriteState", "UITypeEditor")] int LastState)
   {
      Debug.Assert(this.isActive, "Attempted to execute IsInState on an inactive sprite");
      return (state >= FirstState) && (state <= LastState);
   }

   /// <summary>
   /// Affects the sprite's visibility.
   /// </summary>
   /// <remarks>By setting this to 128, the sprite will be drawn half-transparent.
   /// By setting it to 255, it draws in the usual way (as defined). This value is
   /// merged with the ModulateAlpha value in the frames that make up the sprite.
   /// So if the frame is set to ModulateAlpha=128 and the sprite ModulateAlpha=128,
   /// then the final visibility of the frame will only be 25% (64).
   /// <seealso cref="color"/></remarks>
   public int ModulateAlpha
   {
      get
      {
         return 0xFF & color >> 24;
      }
      set
      {
         color = color & 0x00FFFFFF | (byte)(value%256) << 24;
      }
   }

   /// <summary>
   /// Affects the output of red when this sprite is drawn.
   /// </summary>
   /// <remarks>By setting this to 128, only half the red will be output, causing the
   /// sprite to appear more cyan (blue/green) than normal. This value is merged with
   /// the ModulateRed value in the frames that make up the sprite. So if the frame is
   /// set to ModulateRed=128 and the sprite ModulateRed=128, then the final image will
   /// only contain 25% of the original red.
   /// <seealso cref="color"/></remarks>
   public int ModulateRed
   {
      get
      {
         return (color & 0x00FF0000) >> 16;
      }
      set
      {
         color = (int)(color & 0xFF00FFFF) | (byte)(value%256) << 16;
      }
   }

   /// <summary>
   /// Affects the output of green when this sprite is drawn.
   /// </summary>
   /// <remarks>By setting this to 128, only half the green will be output, causing the
   /// sprite to appear more magenta (red+blue) than normal. This value is merged with
   /// the ModulateGreen value in the frames that make up the sprite. So if the frame is
   /// set to ModulateGreen=128 and the sprite ModulateGreen=128, then the final image will
   /// only contain 25% of the original red.
   /// <seealso cref="color"/></remarks>
   public int ModulateGreen
   {
      get
      {
         return (color & 0x0000FF00) >> 8;
      }
      set
      {
         color = (int)(color & 0xFFFF00FF) | (byte)(value%256) << 8;
      }
   }

   /// <summary>
   /// Affects the output of blue when this sprite is drawn.
   /// </summary>
   /// <remarks>By setting this to 128, only half the blue will be output, causing the
   /// sprite to appear more yellow (red+green) than normal. This value is merged with
   /// the ModulateBlue value in the frames that make up the sprite. So if the frame is
   /// set to ModulateBlue=128 and the sprite ModulateBlue=128, then the final image will
   /// only contain 25% of the original blue.
   /// <seealso cref="color"/></remarks>
   public int ModulateBlue
   {
      get
      {
         return color & 0xFF;
      }
      set
      {
         color = (int)(color & 0xFFFFFF00) | (byte)(value%256);
      }
   }
   #endregion

   #region Input Processing
   /// <summary>
   /// Determine if the specified input is being pressed for this sprite.
   /// </summary>
   /// <param name="Input">Which of thes sprite's inputs should be checked</param>
   /// <param name="InitialOnly">If this is true, the result will only be true if the input has just been pressed and was not pressed before.</param>
   /// <returns>If <paramref name="InitialOnly" /> is set, true only when the specified input on this
   /// sprite has just been turned on or pressed, otherwise true if the input is currently on or
   /// "pressed" regardless of the previous state of the input.</returns>
   /// <remarks>This function uses <see cref="oldinputs"/> to determine whether an input
   /// was pressed before or not (when InitialOnly is true). <see cref="oldinputs"/>
   /// is automatically managed by the <see cref="MapPlayerToInputs"/> function.
   /// <seealso cref="oldinputs"/></remarks>
   [Description("Determine if the specified input is being pressed for this sprite.  InitialOnly causes this to return true only if the input has just been pressed and was not pressed before.")]
   public bool IsInputPressed(InputBits Input, bool InitialOnly)
   {
      Debug.Assert(this.isActive, "Attempted to execute IsInputPressed on an inactive sprite");
      return (0 != (inputs & Input)) && 
         (!InitialOnly || (0 == (oldinputs & Input)));
   }

   /// <summary>
   /// Turns on or off the specified input on this sprite.
   /// </summary>
   /// <param name="Input">Indicates a particular input to be affected.</param>
   /// <param name="Press">True if the input should be turned on or "pressed", False if the
   /// input should be turned off or "released".</param>
   /// <remarks>Although this function can be used to set a sprite's inputs based on rules,
   /// <see cref="MapPlayerToInputs"/> is the recommended means for setting inputs on a
   /// sprite. <see cref="SetInputState"/> does not affect <see cref="oldinputs"/>, so if
   /// you want to use this function with the InitialOnly feature of
   /// <see cref="IsInputPressed"/>, you will have to manage <see cref="oldinputs"/>
   /// manually.</remarks>
   [Description("Turns on or off the specified input on this sprite.")]
   public void SetInputState(InputBits Input, bool Press)
   {
      Debug.Assert(this.isActive, "Attempted to execute SetInputState on an inactive sprite");
      if (Press)
         inputs |= Input;
      else
         inputs &= ~Input;
   }

   /// <summary>
   /// Turns off all current inputs on this sprite.
   /// </summary>
   /// <param name="SetOldInputs">True if you want to remember the current set of inputs
   /// as the inputs for the previous frame (<see cref="oldinputs"/>).
   /// False if you just want to clear the inputs.</param>
   [Description("Turns off all current inputs on this sprite.")]
   public void ClearInputs(bool SetOldInputs)
   {
      Debug.Assert(this.isActive, "Attempted to execute ClearInputs on an inactive sprite");
      if (SetOldInputs)
         oldinputs = inputs;
      inputs = 0;
   }

   /// <summary>
   /// Associates the state of the specified keyboard key with the specified input on this sprite.
   /// </summary>
   /// <param name="key">Which key should be tested</param>
   /// <param name="Input">Which sprite input should be affected</param>
   /// <remarks>This function does not affect <see cref="oldinputs"/>.</remarks>
   [Description("Associates the state of the specified keyboard key with the specified input on this sprite.")]
   public void MapKeyToInput(Key key, InputBits Input)
   {
      Debug.Assert(this.isActive, "Attempted to execute MapKeyToInput on an inactive sprite");
      SetInputState(Input, Project.GameWindow.KeyboardState[key]);
   }

   /// <summary>
   /// Associate the state of the input device for the specified player with the inputs on this sprite.
   /// </summary>
   /// <param name="PlayerNumber">Player number 1 through 4. The number must not exceed
   /// the maximum number of players specified in the project properties and stored in
   /// <see cref="Project.MaxPlayers"/>.</param>
   /// <remarks><para>Before the inputs are mapped from the player's input device to the sprite,
   /// the existing inputs are copied from <see cref="inputs"/> to <see cref="oldinputs"/>
   /// so other rules will be able to determine which inputs were pressed before.</para>
   /// <para>The input device is defined by the player at runtime, and may come from a
   /// joystick, gamepad or keyboard.</para></remarks>
   [Description("Associate the state of the input device for the specified player (1-4) with the inputs on this sprite.")]
   public void MapPlayerToInputs(int PlayerNumber)
   {
      Debug.Assert(this.isActive, "Attempted to execute MapPlayerToInput on an inactive sprite");
      if (PlayerNumber > Project.MaxPlayers)
      {
         Debug.Fail("Attempted to map inactive player input");
         return;
      }
      oldinputs = inputs;
      IPlayer player = Project.GameWindow.Players[PlayerNumber-1];
      inputs = 0;
      if (PlayerPressButton(PlayerNumber, player))
      {
         if (player.Up) inputs |= InputBits.Up;
         if (player.Left) inputs |= InputBits.Left;
         if (player.Right) inputs |= InputBits.Right;
         if (player.Down) inputs |= InputBits.Down;
         if (player.Button1) inputs |= InputBits.Button1;
         if (player.Button2) inputs |= InputBits.Button2;
         if (player.Button3) inputs |= InputBits.Button3;
         if (player.Button4) inputs |= InputBits.Button4;
      }
   }

   /// <summary>
   /// Accelerate this sprite according to which directional inputs are on/pressed.
   /// </summary>
   /// <param name="Acceleration">Specifies how much effect any input can have on acceleration in tenths of a pixel per frame per frame</param>
   /// <param name="Max">Specifies the maximum speed to which the sprite can accelerate along any particular axis in pixels per frame</param>
   /// <param name="HorizontalOnly">If this is true, only horizontal acceleration will be applied. (Often times in platform games,
   /// a sprite can move horizontally, but in order to move vertically, must jump.)</param>
   /// <remarks>If the sprite's <see cref="LocalDX"/> or <see cref="LocalDY"/> properties are
   /// set (if the sprite is riding another sprite) this will affect the local velocity and
   /// affect <see cref="dx"/> and/or <see cref="dy"/> only indirectly.</remarks>
   /// <example>
   /// The following example would accelerate a sprite horizontally at 1 pixel per frame per frame
   /// up to a maximum of 5 pixels per frame.
   /// <code>AccelerateByInputs(10, 5, true)</code></example>
   [Description("Accelerate this sprite according to which directional inputs are on.  Acceleration is in tenths of a pixel per frame squared.  Max is in pixels per frame.")]
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

   /// <summary>
   /// Set the state of the directional inputs on this sprite to move toward the specified sprite in a category.
   /// </summary>
   /// <param name="Target">Specifies the sprite collection containing the sprite toward which this sprite will try to move.</param>
   /// <param name="Index">Use <see cref="GetNearestSpriteIndex"/> to compute this value or pass -1 to compute the current nearest sprite each time this is called.</param>
   /// <remarks>This function assumes that the directional inputs on the sprite
   /// cause the sprite to move directly in the direction associated with the input.  It will
   /// not work if, for example, left arrow causes the sprite to turn left instead of move left.
   /// <seealso cref="SetInputsTowardSprite"/>
   /// <seealso cref="PushTowardCategory"/>
   /// <seealso cref="GetNearestSpriteIndex"/></remarks>
   [Description("Set the state of the directional inputs on this sprite to move toward the specified sprite in a category, assuming the input causes the sprite to move directly in its direction. Use GetNearestSpriteIndex to compute an Index or pass -1 to use the current nearest sprite.")]
   public void SetInputsTowardCategory(SpriteCollection Target, int Index)
   {
      Debug.Assert(this.isActive, "Attepmted to execute SetInputsTowardCategory on an inactive sprite");
      Debug.Assert(Index < Target.Count, "Attempted to SetInputsTowardCategory on an index beyond the bounds of a collection");

      if (Index < 0)
         Index = GetNearestSpriteIndex(Target);
      if (Index < 0)
         return;

      SetInputsTowardSprite(Target[Index]);
   }

   /// <summary>
   /// Set the state of the directional inputs on this sprite to move toward the specified sprite.
   /// </summary>
   /// <param name="Target">Specifies the sprite collection containing the sprite toward which
   /// this sprite will try to move.</param>
   /// <remarks>This function is not exposed as a rule function because it is intended to be
   /// called by a plan or other code that can provide a target sprite based on specific
   /// context whereas a sprite definition rule function is supposed to be generic.
   /// This is called by <see cref="SetInputsTowardCategory"/>.</remarks>
   public void SetInputsTowardSprite(SpriteBase Target)
   {
      int targetCenter = Target.PixelX + Target.SolidWidth / 2;
      int myCenter = PixelX + SolidWidth / 2;

      if (targetCenter < myCenter)
         inputs |= InputBits.Left;
      else if (targetCenter > myCenter)
         inputs |= InputBits.Right;
      else
         inputs &= ~(InputBits.Left | InputBits.Right);

      targetCenter = Target.PixelY + Target.SolidHeight / 2;
      myCenter = PixelY + SolidHeight / 2;
      if (targetCenter < myCenter)
         inputs |= InputBits.Up;
      else if (targetCenter > myCenter)
         inputs |= InputBits.Down;
      else
         inputs &= ~(InputBits.Up | InputBits.Down);
   }

   /// <summary>
   /// Move the sprite to the position of the mouse cursor and set the sprite's button inputs based on mouse button states.
   /// </summary>
   /// <param name="InstantMove">If true, the sprite will be moved immediately without regard to
   /// the existing position or solidity or anything else.  If false, the sprite's position will
   /// not be immediately changed, but its velocity will be set so that the sprite will end up at
   /// the mouse cursor's location after <see cref="MoveByVelocity"/> executes. Note that moving
   /// the sprite instantly will ignore solidity and will not work well with sprites riding on
   /// this sprite, while allowing just the velocity to be set will allow this, but limit the
   /// sprite's movement based on solidity.</param>
   /// <remarks>Before the button inputs are mapped from the mouse to the sprite,
   /// the existing inputs are copied from <see cref="inputs"/> to <see cref="oldinputs"/>
   /// so other rules will be able to determine which buttons were pressed before.
   /// </remarks>
   [Description("Move the sprite to the position of the mouse cursor and set the sprite's button inputs based on mouse button states. If InstantMove is true, the sprite will be moved immediately, otherwise it the velocity will be set to move when MoveByVelocity runs.")]
   public void MapMouseToSprite(bool InstantMove)
   {
      System.Drawing.Point pos = ParentLayer.GetMousePosition();
      if (InstantMove)
      {
         oldX = x;
         oldY = y;
         x = pos.X;
         y = pos.Y;
      }
      else
      {
         dx = pos.X - x;
         dy = pos.Y - y;
      }
      oldinputs = inputs;
      inputs = 0;
      if (0 != (System.Windows.Forms.Control.MouseButtons & System.Windows.Forms.MouseButtons.Left))
         inputs |= InputBits.Button1;
      if (0 != (System.Windows.Forms.Control.MouseButtons & System.Windows.Forms.MouseButtons.Right))
         inputs |= InputBits.Button2;
      if (0 != (System.Windows.Forms.Control.MouseButtons & System.Windows.Forms.MouseButtons.Middle))
         inputs |= InputBits.Button3;
   }
   #endregion

   #region Solidity
   /// <summary>
   /// Set the solidity rules to which the sprite is currently reacting.
   /// </summary>
   /// <param name="Solidity">Specified which solidity rules should adopt.</param>
   /// <remarks>Since solidity is based on tile categories which in turn can contain
   /// tiles from multiple tilesets, the same solidity definition can apply to any
   /// number of maps with different tilesets. So this doesn't need to be called
   /// just to handle tiles from a different tileset, but could be called to make
   /// the sprite act differently to all tiles (for example, to suddenly be able to
   /// traverse any water).</remarks>
   [Description("Set the solidity rules to which the sprite is currently reacting.")]
   public void SetSolidity(Solidity Solidity)
   {
      m_solidity = Solidity;
   }

   /// <summary>
   /// Alter the sprite's velocity to react to solid areas on the map.
   /// </summary>
   /// <returns>True if the sprite's velocity was altered; in other words, if it pushed
   /// against something solid. That doesn't necessarily mean the sprite was stopped by
   /// something solid because it may just be pushed uphill.</returns>
   /// <remarks>It is recommended that this be the last rule applied to the sprite's velocity
   /// before <see cref="MoveByVelocity"/> is applied. This will help ensure that the sprite
   /// never goes through solids even if scrolling and platforms try to make it.</remarks>
   [Description("Alter the sprite's velocity to react to solid areas on the map.  Returns true if velocity is affected by solid.")]
   public bool ReactToSolid()
   {
      Debug.Assert(this.isActive, "Attempted to execute ReactToSolid on an inactive sprite");
      if (m_solidity == null)
         throw new System.ApplicationException("Attempted to execute ReactToSolid on sprite without solidity defined");
      bool hit = false;
      double dyOrig = dy;
      double dxOrig = dx;

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
               int ceiling = layer.GetBottomSolidPixel(new System.Drawing.Rectangle(PixelX2, slopedFloor - SolidHeight, SolidWidth, ProposedPixelY + SolidHeight - slopedFloor), m_solidity);
               if ((ceiling == int.MinValue) && (RidingOn == null))
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
                  int floor = layer.GetTopSolidPixel(new System.Drawing.Rectangle(PixelX2, ProposedPixelY + SolidHeight, SolidWidth, slopedCeiling - ProposedPixelY), m_solidity);
                  if ((floor == int.MinValue) && (RidingOn == null))
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
               int ceiling = layer.GetBottomSolidPixel(new System.Drawing.Rectangle(PixelX, slopedFloor - SolidHeight, SolidWidth, ProposedPixelY + SolidHeight - slopedFloor), m_solidity);
               if ((ceiling == int.MinValue) && (RidingOn == null))
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
                  int floor = layer.GetTopSolidPixel(new System.Drawing.Rectangle(PixelX, ProposedPixelY + SolidHeight, SolidWidth, slopedCeiling - ProposedPixelY), m_solidity);
                  if ((floor == int.MinValue) && (RidingOn == null))
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

      if (hit && !double.IsNaN(LocalDX))
         LocalDX += dx - dxOrig;

      return hit;
   }

   /// <summary>
   /// Ensure the sprite stays in contact with the ground by altering its velocity
   /// to snap down onto the ground when it is close to the ground.
   /// </summary>
   /// <param name="Threshhold">The number of pixels that mey separate the sprite from the ground.
   /// If the sprite is within this threshhold distance, it will be "snapped".</param>
   /// <returns>True if the sprite was snapped to the ground.</returns>
   /// <remarks>Note that this does not actually move the sprite, but rather just alters its
   /// velocity so that when <see cref="MoveByVelocity"/> is applied, it will be touching the
   /// ground, if the appropriate conditions are met. The purpose of this rule is to help sprites
   /// behave as desired when going downhill in case the force of gravity isn't enough to keep
   /// them "grounded" (which it often isn't with the simple physics model used for sprites).
   /// </remarks>
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

   /// <summary>
   /// Determines if the sprite is blocked from moving freely in a particular direction by solidity on the layer.
   /// </summary>
   /// <param name="Direction">Which direction should be tested</param>
   /// <returns>True if the sprite's velocity will be affected or stopped when attempting
   /// to move in the specified direction.  False if the sprite can move freely in that
   /// direction.</returns>
   /// <remarks>Note that this function only tests if there is any solidity in the specified
   /// direction, but does not indicate whether the sprite will be blocked by it or simply
   /// deflected (up or down a hill) by it.</remarks>
   [Description("Determines if the sprite is blocked from moving freely in a particular direction by solidity on the layer.")]
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

   /// <summary>
   /// Determines if the specified point within a sprite is blocked from moving in a particular direction by the specified number of pixels.
   /// </summary>
   /// <param name="TestPoint">Point within the sprite from which the test is performed</param>
   /// <param name="Direction">Direction relative to TestPoint which will be tested for solidity</param>
   /// <param name="Distance">How many pixels to check for solidity from the test point</param>
   /// <returns>True if there is solidity within Distance pixels of TestPoint, false otherwise.</returns>
   [Description("Determines if the specified point within a sprite is blocked from moving in a particular direction by the specified number of pixels.")]
   public bool IsPointBlocked(RelativePosition TestPoint, Direction Direction, int Distance)
   {
      Debug.Assert(this.isActive, "Attempted to execute IsPointBlocked on an inactive sprite");
      System.Drawing.Point ptRelative = GetRelativePosition(TestPoint);
      switch(Direction)
      {
         case Direction.Up:
            return layer.GetBottomSolidPixel(new System.Drawing.Rectangle(ptRelative.X, ptRelative.Y - Distance, 1, Distance), m_solidity) != int.MinValue;
         case Direction.Right:
            return layer.GetLeftSolidPixel(new System.Drawing.Rectangle(ptRelative.X+1, ptRelative.Y, Distance, 1), m_solidity) != int.MinValue;
         case Direction.Down:
            return layer.GetTopSolidPixel(new System.Drawing.Rectangle(ptRelative.X, ptRelative.Y+1, 1, Distance), m_solidity) != int.MinValue;
         case Direction.Left:
            return layer.GetRightSolidPixel(new System.Drawing.Rectangle(ptRelative.X - Distance, ptRelative.Y, Distance, 1), m_solidity) != int.MinValue;
      }
      return false;
   }

   #endregion

   #region Tile Interaction
   /// <summary>
   /// Tracks information about a tile for the purposes of processing interactions
   /// between a sprite and a tile that it is touching.
   /// </summary>
   /// <remarks>Instances of this class are created by <see cref="TouchTiles"/>.
   /// and added to <see cref="TouchedTiles"/>.</remarks>
   public class TouchedTile
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
   /// <summary>
   /// A collection of tiles being processed by <see cref="TouchTiles"/> and other rules
   /// related to tile interactions.
   /// </summary>
   /// <remarks><see cref="TouchTiles"/> populates this object, and other touch-functions
   /// then process the data about tiles that the sprite is touching stored in this
   /// object. It's not usually necessary to refer to this object directly.</remarks>
   [NonSerialized()]
   public System.Collections.ArrayList TouchedTiles = null;

   /// <summary>
   /// Collects information about tiles the sprite is currently touching.
   /// </summary>
   /// <param name="Category">This should supply a tile category that includes all tiles that the sprite interacts with.</param>
   /// <returns>True if the sprite is touching any tiles in the supplied category.</returns>
   /// <remarks>This must be called before performing most forms of tile interaction.
   /// By supplying a single category containing all tiles with which the sprite may need
   /// to interact, performance is optimized so that each tile touching the sprite only
   /// needs to be inspected once to check if it's relevant.  This also helps simplify the
   /// process of processing each tile exactly once by queuing a list of tiles to be
   /// processed and tracking which of those tiles have been processed.
   /// <seealso cref="TileTake"/><seealso cref="TileUseUp"/><seealso cref="TileTouchingIndex"/>
   /// <seealso cref="TileActivateSprite"/><seealso cref="TileAddSprite"/>
   /// <seealso cref="TileChange"/><seealso cref="TileChangeTouched"/></remarks>
   /// <example>This example will check to see if the current sprite is touching
   /// any tiles in a category named "Touchable" and then (assuming tile numbers 10
   /// and 11 are members of this category) take any tiles whose number is 10 as an
   /// inventory item represented by a counter named "Coins", and take any tiles whose
   /// number is 11 as an inventory item represented by a counter named "Keys".
   /// <code lang="C#">
   /// if (TouchTiles(TileCategoryName.Touchable))
   /// {
   ///    TileTake(10,Counter.Coins))
   ///    TileTake(11,Counter.Keys))
   /// }</code></example>
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
      if (maxY >= layer.VirtualRows)
         maxY = layer.VirtualRows -1;
      int maxYEdge = (PixelY + SolidHeight - 1) / th;
      int minX = (PixelX - 1)/ tw;
      int minXEdge = PixelX / tw;
      int maxX = (PixelX + SolidWidth) / tw;
      if (maxX >= layer.VirtualColumns)
         maxX = layer.VirtualColumns - 1;
      int maxXEdge = (PixelX + SolidWidth - 1) / tw;
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

   /// <summary>
   /// Determine if the sprite is touching the specified tile, and if so,
   /// "take" it and increment a counter (requires <see cref="TouchTiles"/>).
   /// </summary>
   /// <param name="TileValue">Index of the tile to look for.</param>
   /// <param name="Counter">Counter to be checked and incremented when the specified tile is found.
   /// If the counter's maximum value has been reached, the tile will not be taken.</param>
   /// <param name="NewValue">Specified the tile value with which the touched tile will
   /// be replaced if the counter has not hit its maximum.</param>
   /// <returns>The number of tiles affected.</returns>
   /// <remarks><para>This function will search through all tiles that have been touched (collected
   /// by <see cref="TouchTiles"/>, and for each tile that it finds that matches the specified
   /// <paramref name="TileValue" />, it will check <paramref name="Counter" />, and, if it
   /// has not yet reached the maximum value, increment the counter and replace the tile with
   /// the tile number specified by NewValue.</para><para>
   /// Only unprocessed tiles are considered. Once this function (or similar functions)
   /// affects the tile, it is marked as processed. It is only marked as processed if
   /// it is affected (if the counter changes).</para></remarks>
   [Description("When the sprite is touching the specified tile, and the specified counter is not maxed, change/clear the tile value to NewValue and increment the specified counter/parameter. Returns the number of tiles affected. (Must run TouchTiles first.)")]
   public int TileTake(int TileValue,  Counter Counter, int NewValue)
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
               layer[tt.x, tt.y] = tt.tileValue = NewValue;
               tt.processed = true;
               result++;
            }
            else
               break;
         }
      }
      return result;
   }

   /// <summary>
   /// Make the specified tile behave like it is "using up" one of the items tracked by the specified counter (requires <see cref="TouchTiles"/>).
   /// </summary>
   /// <param name="TileValue">Index of the tile to look for</param>
   /// <param name="Counter">Specifies a counter that will be affected. If this counter's value is
   /// greater than 0, it will be decremented and the tile removed.</param>
   /// <param name="NewValue">Specifies a new tile value that will replace the touched tile when
   /// it is removed. This value is commonly 0, which usually represents a tileset's empty tile.</param>
   /// <returns>The number of tiles affected</returns>
   /// <remarks><para>This function will search through all tiles that have been touched (collected
   /// by <see cref="TouchTiles"/>, and for each tile that it finds that matches the specified
   /// <paramref name="TileValue" />, it will check <paramref name="Counter" />, and, if it
   /// is greater than 0, decrement the counter and replace the tile with tile number
   /// <paramref name="NewValue"/>.</para>
   /// <para>Only unprocessed tiles are considered. Once this function (or similar functions)
   /// affects the tile, it is marked as processed. It is only marked as processed if
   /// it is affected (if the counter changes).</para></remarks>
   [Description("When the sprite is touching the specified tile, and the specified counter is greater than 0, decrement the counter and clear the tile value to NewValue. Returns the number of tiles affected. (Must run TouchTiles first.)")]
   public int TileUseUp(int TileValue,  Counter Counter, int NewValue)
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
               layer[tt.x, tt.y] = tt.tileValue = NewValue;
               tt.processed = true;
               result++;
            }
            else
               break;
         }
      }
      return result;
   }


   /// <summary>
   /// Determine if the sprite is touching a tile in the specified category (requires <see cref="TouchTiles"/>).
   /// </summary>
   /// <param name="Category">Category of tiles to be tested</param>
   /// <param name="TouchingIndex">Receives the index of the first qualifying tile
   /// if the sprite is touching a tile in the specified category.  This is an index
   /// into the <see cref="TouchedTiles"/> array returned by <see cref="TouchTiles"/>.</param>
   /// <param name="InitialOnly">If true, a tile can only qualify if the sprite was not
   /// already touching the tile in the previous frame.</param>
   /// <returns>True if the sprite is touching a tile in the specified category, false otherwise.</returns>
   [Description("Determine if the sprite is touching the specified tile. TouchingIndex is updated to refer to the index of the first qualifying tile if true. Returns true if the sprite is touching a tile in the specified category. (Must run TouchTiles first.)")]
   public bool TileCategoryTouched(TileCategoryName Category, ref int TouchingIndex, bool InitialOnly)
   {
      Debug.Assert(this.isActive, "Attempted to execute TileCategoryTouched on an inactive sprite");

      if (TouchedTiles == null)
         return false;

      int result = 0;

      for (int i = 0; i < TouchedTiles.Count; i++)
      {
         TouchedTile tt = (TouchedTile)TouchedTiles[i];
         if (!tt.processed && layer.GetTile(tt.x, tt.y).IsMember(Category) 
            && (!InitialOnly || tt.initial))
         {
            TouchingIndex = i;
            tt.processed = true;
            return true;
         }
      }
      return false;
   }

   /// <summary>
   /// Find the next unprocessed tile of the specified type (requires <see cref="TouchTiles"/>).
   /// </summary>
   /// <param name="TileValue">Tile index to search for</param>
   /// <param name="InitialOnly">If this is true, the tile will only be considered if the sprite
   /// just started touching it (was not touching it before).</param>
   /// <param name="MarkAsProcessed">If this is true, the tile that is returned is immediately
   /// marked as processed, otherwise it is left as unprocessed and may still be affected by
   /// other tile interaction functions.</param>
   /// <returns>The index of the next unprocessed tile in <see cref="TouchedTiles"/> if one
   /// exists with the specified <paramref name="TileValue" />, or -1 if no such tile exists.
   /// </returns>
   /// <remarks>While <see cref="TileUseUp"/> and <see cref="TileTake"/> provide simple
   /// access to common behaviors related to tile interactions, they aren't expected to cover
   /// all behaviors you might want to implement related to tile interactions.  This function
   /// provides a piece of functionality that will be useful in more detailed control over
   /// tile interactions.<seealso cref="TouchTiles"/><seealso cref="TileActivateSprite"/>
   /// <seealso cref="TileAddSprite"/></remarks>
   /// <example>
   /// The following example demonstrates how you could activate the next inactive instance of
   /// a sprite in the "Points" category at the location of any tile whose tile number is 10
   /// when the sprite touches the tile, only when the sprite first touches the tile. Performing
   /// this in a while loop ensures that all such tiles that the sprite is initially touching get
   /// processed at once, which is important because it won't be initially touching them any more
   /// in the next frame. TempNum is any temporary numeric variable, such as a sprite parameter.
   /// <code>
   /// if (TouchTiles(TileCategoryName.Touchable))
   /// {
   ///    TempNum = TileTouchingIndex(10, true, true);
   ///    while(TempNum > 0)
   ///    {
   ///       TileActivateSprite(TempNum, ParentLayer.m_SpriteCategories.Points, true);
   ///       TempNum = TileTouchingIndex(10, true, true);
   ///    }
   /// }
   /// </code>
   /// </example>
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

   /// <summary>
   /// Activate the next inactive sprite from a category at the coordinates of a tile being touched by the sprite.
   /// </summary>
   /// <param name="TouchingIndex">Index of the tile in the array of tiles to process (<see cref="TouchedTiles"/>).
   /// This can be acquired using <see cref="TileTouchingIndex"/>.</param>
   /// <param name="Category">Category containing sprites that can be activated.</param>
   /// <param name="ClearParameters">True if the newly activated sprite's parameters should
   /// all be set to zero.</param>
   /// <returns>The index of the newly activated sprite, if a sprite was activated (this
   /// value will be greater than or equal to zero) or -1 if all sprites in the specified
   /// category are already activate.</returns>
   /// <remarks><para>This function allows you to treat a number of sprites within a category
   /// as kind of a dynamic collection of sprites which can be activated one after the other.
   /// Each time this is called, it will find and activate the next inactive sprite within
   /// the category. This limits the number of sprites that can be activated by this function
   /// to the number of actual sprite instances in the specified category on the layer where
   /// the function is executed. For a truly dynamic collection of sprites, see
   /// <see cref="TileAddSprite"/>.</para>
   /// <para>Clearing the parameters of a newly activated sprite can be very useful in initializing
   /// the sprite as soon as it is activated because the activated sprite can check to see
   /// when a particular parameter is zero. If it sees that it's zero, it can immediately set
   /// it to some other value and perform whatever initialization it needs to (such as offsetting
   /// its position from the tile by some pre-set distance).</para></remarks>
   [Description("Activate the next inactive sprite from a category at the coordinates of a tile being touched by the sprite.  Use TileTouchingIndex to acquire TouchingIndex.  Returns the index into the category of the sprite that was activated, or -1 if all sprites in the category were already active.")]
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
            {
               Category[i].frame = 0;
               Category[i].state = 0;
               Category[i].ClearParameters();
            }
            Category[i].ProcessRules();
            return i;
         }
      }
      return -1;
   }

   /// <summary>
   /// Create a new (dynamic) instance of the specified sprite type at the coordinates
   /// of a tile being touched by the player.
   /// </summary>
   /// <param name="TouchingIndex">Refers to a tile in <see cref="TouchedTiles"/> by index.
   /// <see cref="TileTouchingIndex"/> can be used to acquire this value.</param>
   /// <param name="SpriteDefinition">Specifies the type of sprite that will be created.</param>
   /// <remarks><para>As opposed to <see cref="TileActivateSprite"/>, which activates a pre-defined
   /// instance of a sprite, this function will actually create a new sprite instance on the
   /// fly (referred to as a "dynamic" sprite instance). Dynamic sprites cannot be referred
   /// to directly by other rules because they don't have names associated with them, therefore
   /// it's recommended that <see cref="TileActivateSprite"/> be used when more control is
   /// desired. However, for common sprites that may be created on any map, this is ideal
   /// because it doesn't require you to pre-define each possible instance.</para>
   /// <para>Dynamic sprites are added to the end of each category that they belong to
   /// (categories are defined in terms of sprite definitions, so a dynamic sprite knows
   /// the categories in which it should participate). This happens as the sprite is created.
   /// When the sprite is deactivated, it will automatically be removed from all categories
   /// as part of <see cref="LayerBase.ProcessSprites"/>.</para>
   /// <para>The maximum number of sprites that can be active on a layer at once is
   /// defined by <see cref="SpriteCollection.maxCollectionSize"/>. This includes static
   /// sprites as well as dynamic sprites. The purpose of this maximum is not a technical
   /// limitation (feel free to change the code in SpriteCollection.cs to increase the maximum
   /// all you like). It only exists to help developers realize when sprite instances are
   /// "leaking" (not being cleaned up or deactivated properly) or when more sprites than
   /// necessary are being created. In many cases, the engine can actually handle quite a
   /// bit more than 100 sprites without significant performance impact.</para>
   /// <para>Because the sprite instances being created are not activations of already
   /// existing instances (as is the case with <see cref="TileActivateSprite"/>), there
   /// is no need for a parameter to reset all the sprite's parameters. All parameters of
   /// a newly created sprite instance are always initialized to zero. The solidity is copied
   /// from the sprite definition that created the sprite.</para>
   /// <para>New sprite instances will not refer to any solidity, and will begin in the first
   /// frame of the first state, but you can use rules to affect <see cref="GeneralRules.lastCreatedSprite"/>
   /// to set values of the new sprite, or define rules on the created sprite type to make it
   /// initialize itself appropriately.</para>
   /// <seealso cref="PlanBase.AddSpriteAtPlan"/>
   /// <seealso cref="AddSpriteHere"/></remarks>
   [Description("Create a new (dynamic) instance of the specified sprite type at the coordinates of a tile being touched by the player.  Use TileTouchingIndex to acquire TouchingIndex.")]
   public void TileAddSprite(int TouchingIndex, [Editor("SpriteDefinition", "UITypeEditor")] System.Type SpriteDefinition)
   {
      Debug.Assert(this.isActive, "Attempted to execute TileAddSprite on an inactive sprite");

      System.Reflection.ConstructorInfo constructor = SpriteDefinition.GetConstructor(new System.Type[]
      {
         typeof(LayerBase), typeof(double), typeof(double), typeof(double), typeof(double), typeof(int), typeof(int), typeof(bool), typeof(Display), typeof(Solidity), typeof(int), typeof(bool)
      });
      TouchedTile tt = (TouchedTile)TouchedTiles[TouchingIndex];
      lastCreatedSprite = (SpriteBase)constructor.Invoke(new object[]
      {
         layer, tt.x * layer.Tileset.TileWidth, tt.y * layer.Tileset.TileHeight, 0, 0, 0, 0, true, layer.ParentMap.Display, m_solidity, -1, true
      });
   }

   /// <summary>
   /// Create a new (dynamic) instance of the specified sprite positioned such that HotSpot on the created sprite overlaps Location on this sprite.
   /// </summary>
   /// <param name="SpriteDefinition">Specifies the type of sprite to create</param>
   /// <param name="Location">Specifies a point in the current sprite, relative to which the new
   /// sprite will be created.</param>
   /// <param name="HotSpot">Specifies a point within the created sprite that will be used to
   /// position the sprite (the location of the "handle" by which it is positioned).</param>
   /// <remarks>See <see cref="TileAddSprite"/> for more information about dynamically added sprites.
   /// <seealso cref="TileAddSprite"/><seealso cref="PlanBase.AddSpriteAtPlan"/>
   /// <seealso cref="GeneralRules.lastCreatedSprite"/></remarks>
   /// <example>
   /// The following code will create an instance of a sprite named "Bullet" such that the left side
   /// of the bullet matches up with the right side of the current sprite. (The bullet will be
   /// immediately to the right of this sprite, overlapping by 1 pixel.)
   /// <code>AddSpriteHere(typeof(Sprites.Bullet), RelativePosition.RightMiddle, RelativePosition.LeftMiddle);</code>
   /// </example>
   [Description("Create a new (dynamic) instance of the specified sprite positioned such that HotSpot on the created sprite overlaps Location on this sprite.")]
   public void AddSpriteHere([Editor("SpriteDefinition", "UITypeEditor")] System.Type SpriteDefinition, RelativePosition Location, RelativePosition HotSpot)
   {
      Debug.Assert(this.isActive, "Attempted to execute AddSpriteHere on an inactive sprite");

      System.Reflection.ConstructorInfo constructor = SpriteDefinition.GetConstructor(new System.Type[]
      {
         typeof(LayerBase), typeof(double), typeof(double), typeof(double), typeof(double), typeof(int), typeof(int), typeof(bool), typeof(Display), typeof(Solidity), typeof(int), typeof(bool)
      });

      lastCreatedSprite = (SpriteBase)constructor.Invoke(new object[]
      {
         layer, 0, 0, 0, 0, 0, 0, true, layer.ParentMap.Display, m_solidity, -1, true
      });

      System.Drawing.Point ptLocation = GetRelativePosition(Location);
      System.Drawing.Point ptHotSpot = lastCreatedSprite.GetRelativePosition(HotSpot);
      lastCreatedSprite.x = lastCreatedSprite.oldX = ptLocation.X - ptHotSpot.X;
      lastCreatedSprite.y = lastCreatedSprite.oldY = ptLocation.Y - ptHotSpot.Y;
   }

   /// <summary>
   /// Change tiles of the specified type that the sprite is touching to another tile (requires <see cref="TouchTiles"/>).
   /// </summary>
   /// <param name="OldTileValue">Index of the tile type to search for</param>
   /// <param name="NewTileValue">Which tile should these tiles be changed to</param>
   /// <param name="InitialOnly">If true, only affect tiles that the player just started touching.</param>
   /// <returns>The number of tiles affected.</returns>
   /// <remarks>This function changes all specified tiles at once.
   /// Use <see cref="TileChangeTouched"/> to change only one tile.
   /// <seealso cref="TileChangeTouched"/></remarks>
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

   /// <summary>
   /// Change the tile specified by TouchingIndex that is being touched by the sprite to another tile (requires <see cref="TouchTiles"/>).
   /// </summary>
   /// <param name="TouchingIndex">Refers to a tile in <see cref="TouchedTiles"/> by index.
   /// <see cref="TileTouchingIndex"/> can be used to acquire this value.</param>
   /// <param name="NewTileValue">Specifies the tileset tile index of the new tile that will appear in place of the specified tile.</param>
   [Description("Change the tile specified by TouchingIndex that is being touched by the sprite to another tile. (Must run TouchTiles first.)")]
   public void TileChangeTouched(int TouchingIndex, int NewTileValue)
   {
      Debug.Assert(this.isActive, "Attempted to execute TileChangeTouched on an inactive sprite");
      Debug.Assert((TouchedTiles != null) && (TouchedTiles.Count > TouchingIndex),
         "Attempted to execute TileChangeTouched with invalid touched tiles");

      if ((TouchedTiles == null) || (TouchedTiles.Count <= TouchingIndex))
         return;

      TouchedTile tt = (TouchedTile)TouchedTiles[TouchingIndex];
      layer[tt.x, tt.y] = tt.tileValue = NewTileValue;
   }

   /// <summary>
   /// Calculate the absolute position of a specified <see cref="RelativePosition"/> value with respect to this sprite.
   /// </summary>
   /// <param name="RelativePosition">Which position within this sprite should be retrieved.</param>
   /// <returns>A point relative to the sprite's layer that represents the requested position in the sprite.</returns>
   /// <remarks>This cannot be applied as a rule function because it returns a point object,
   /// which is not supported by the SGDK2 IDE as an output type.</remarks>
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

   /// <summary>
   /// Determines if a tile at the sprite's current position is a member of the specified category.
   /// </summary>
   /// <param name="Category">Tile category against which the tile will be checked.</param>
   /// <param name="RelativePosition">The sprite may be on multiple tiles at once. This parameter
   /// indicates which part of the sprite to look at, and gets the tile from the layer at
   /// the specified position.</param>
   /// <returns>True if the specified point in the sprite is on a tile in the specified category, false otherwise.</returns>
   [Description("Examines the tile on the layer at the sprite's current position and determines if it is a member of the specified category. The RelativePosition parameter determines which part of the sprite to use when identifying a location on the layer. (TouchTiles is not necessary for this function.)")]
   public bool IsOnTile(TileCategoryName Category, RelativePosition RelativePosition)
   {
      Debug.Assert(this.isActive, "Attempted to execute IsOnTile on an inactive sprite");

      System.Drawing.Point rp = GetRelativePosition(RelativePosition);
      return layer.GetTile((int)(rp.X / layer.Tileset.TileWidth), (int)(rp.Y / layer.Tileset.TileHeight)).IsMember(Category);
   }
   #endregion

   #region Activation
   /// <summary>
   /// Deactivate this sprite.
   /// </summary>
   /// <remarks>Deactivated sprites are no longer be drawn and their rules are not processed.
   /// In debug mode, attempting to refer to inactive sprites from active rules (or otherwise
   /// attempting to execute functions on inactive sprites) will display errors. If the
   /// sprite was added as a dynamic sprite by <see cref="TileAddSprite"/>, deactivating it
   /// will cause <see cref="LayerBase.ProcessSprites"/> to remove the sprite from all
   /// categories so it cannot even be considered for re-activation.
   /// <seealso cref="TileAddSprite"/><seealso cref="TileActivateSprite"/></remarks>
   [Description("Deactivate this sprite.  It will no longer be drawn, and in debug mode, will display errors if rules try to execute on it.")]
   public void Deactivate()
   {
      isActive = false;
   }
   #endregion

   #region Pushing
   /// <summary>
   /// Alter the velocity of TargetSprite to plan to move out of the way of this sprite
   /// if TargetSprite is obstructing this sprite.
   /// </summary>
   /// <param name="TargetSprite">Sprite whose velocity may be affected.</param>
   /// <returns>True if the sprite was pushed, false otherwise.</return>
   [Description("Alter the velocity of TargetSprite to plan to move out of the way of this sprite.")]
   public bool PushSprite(SpriteBase TargetSprite)
   {
      Debug.Assert(this.isActive, "Attempted to execute PushSprite on an inactive sprite");

      int x1 = ProposedPixelX;
      int w1 = SolidWidth;
      int x2 = TargetSprite.ProposedPixelX;
      int w2 = TargetSprite.SolidWidth;
      int y1 = ProposedPixelY;
      int h1 = SolidHeight;
      int y2 = TargetSprite.ProposedPixelY;
      int h2 = TargetSprite.SolidHeight;

      int pushright = x1 + w1 - x2;
      int pushleft = x2 + w2 - x1;
      if ((pushright > 0) && (pushleft > 0))
      {
         int pushx;
         pushx = (pushright < pushleft) ? pushright : -pushleft;
         int pushdown = y1 + h1 - y2;
         int pushup = y2 + h2 - y1;
         if ((pushup > 0) && (pushdown > 0))
         {
            int pushy = (pushdown < pushup) ? pushdown : -pushup;
            if (System.Math.Abs(pushx) > System.Math.Abs(pushy))
            {
               if (!double.IsNaN(TargetSprite.LocalDY))
                  TargetSprite.LocalDY += pushy;
               TargetSprite.dy += pushy;
            }
            else
            {
               if (!double.IsNaN(TargetSprite.LocalDX))
                  TargetSprite.LocalDX += pushx;
               TargetSprite.dx += pushx;
            }
            return true;
         }
      }
      return false;
   }

   /// <summary>
   /// Alter the velocity of this sprite to plan to move out of the way of sprites in Pushers
   /// if this sprite is obstructing sprites in Pushers.
   /// </summary>
   /// <param name="Pushers">Sprites that can push this sprite.</param>
   /// <returns>True if the sprite was pushed, false otherwise.</return>
   [Description("Alter the velocity of this sprite to plan to move out of the way of sprites in Pushers.")]
   public bool ReactToPush(SpriteCollection Pushers)
   {
      if (!isActive)
         return false;
      bool result = false;
      for (int idx = 0; idx < Pushers.Count; idx++)
      {
         SpriteBase TargetSprite = Pushers[idx];
         if ((TargetSprite == this) || (!TargetSprite.isActive))
            continue;
         if (TargetSprite.Processed)
            result |= TargetSprite.PushSprite(this);
      }
      return result;
   }

   /// <summary>
   /// Alter the velocity of this sprite to plan to move out of the way of sprites in
   /// Pushers but if this sprite is planning to overlap a sprite in Pushers, only
   /// after those sprites have processed their rules (usually to react to this
   /// sprite's pushing first).
   /// </summary>
   /// <param name="Pushers">Sprites that can push back on this sprite.</param>
   /// <returns>True if the sprite was pushed, false otherwise.</return>
   /// <remarks>The process of making moving sprites push and avoid each other is
   /// relatively complex and not 100% reliable. If only one of the sprites is moving,
   /// it's recommended to use the simpler <see cref="ReactToPush"/> function on the
   /// moving sprite instead. This function is designed to be used on sprites that can
   /// both move and push other sprites, and the other sprites need to push back if
   /// they cannot move any farther.</remarks>
   [Description("Alter the velocity of this sprite to plan to move out of the way of sprites in Pushers, but only after they have processed their rules where necessary.")]
   public bool ReactToPushback(SpriteCollection Pushers)
   {
      if (!isActive)
         return false;

      bool result = false;
      for (int idx = 0; idx < Pushers.Count; idx++)
      {
         SpriteBase TargetSprite = Pushers[idx];
         if (!TargetSprite.isActive || TargetSprite.Processed || TargetSprite == this)
            continue;
         if (TestCollisionRect(TargetSprite))
         {
            TargetSprite.ProcessRules();
            result |= TargetSprite.PushSprite(this);
         }
      }

      return result;
   }

   /// <summary>
   /// Determine if this sprite is planning to overlap the target sprite.
   /// </summary>
   /// <param name="TargetSprite">Sprite against which planned overlap is checked.</param>
   /// <returns>True if the sprite will overlap TargetSprite.</return>
   [Description("Determine if this sprite is planning to overlap the target sprite.")]
   public bool TestCollisionRect(SpriteBase TargetSprite)
   {
      Debug.Assert(this.isActive, "Attempted to execute TestCollision on an inactive sprite");

      int x1 = ProposedPixelX;
      int w1 = SolidWidth;
      int x2 = TargetSprite.ProposedPixelX;
      int w2 = TargetSprite.SolidWidth;
      int y1 = ProposedPixelY;
      int h1 = SolidHeight;
      int y2 = TargetSprite.ProposedPixelY;
      int h2 = TargetSprite.SolidHeight;

      int pushright = x1 + w1 - x2;
      int pushleft = x2 + w2 - x1;
      if ((pushright > 0) && (pushleft > 0))
      {
         int pushdown = y1 + h1 - y2;
         int pushup = y2 + h2 - y1;
         if ((pushup > 0) && (pushdown > 0))
            return true;
      }
      return false;
   }
   #endregion
}