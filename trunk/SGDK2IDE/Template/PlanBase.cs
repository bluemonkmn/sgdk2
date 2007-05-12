/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */
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
   /// <summary>
   /// This is provided as a temporary storage area for numbers while processing
   /// plan rules.
   /// </summary>
   public static int SharedTemp1;

   /// <summary>
   /// This value is used by <see cref="CheckNextCoordinate"/> to determine how close a
   /// sprite must be to a coordinate before heading to the next coordinate.
   /// </summary>
   [Description("How close must a sprite be to a coordinate in this plan before heading to the next (default=5)")]
   public int TargetDistance = 5;

	protected PlanBase()
	{
	}

   /// <summary>
   /// Stores information about a coordinate within a <see cref="PlanBase"/>.
   /// </summary>
   [Serializable()]
   public struct Coordinate
   {
      /// <summary>
      /// The horizontal aspect of this coordinate
      /// </summary>
      public int x;
      /// <summary>
      /// The vercial aspect of this coordinate 
      /// </summary>
      public int y;
      /// <summary>
      /// Can be used to store any additional piece of information about this coordinate,
      /// but <see cref="PlanBase.CheckNextCoordinate"/> will use it to define a wait period
      /// for a sprite arriving at this coordinate before proceeding to the next.
      /// </summary>
      public int weight;
      /// <summary>
      /// Creates a new coordinate given all its properties
      /// </summary>
      /// <param name="x">Provides the <see cref="x"/> value for this coordinate</param>
      /// <param name="y">Provides the <see cref="y"/> value for this coordinate</param>
      /// <param name="weight">Provides the <see cref="weight"/> calue for this coordinate</param>
      public Coordinate(int x, int y, int weight)
      {
         this.x = x;
         this.y = y;
         this.weight = weight;
      }
      /// <summary>
      /// Allows a path coordinate to be automatically used as a Point
      /// </summary>
      public static implicit operator Point(Coordinate value)
      {
         return new Point(value.x, value.y);
      }
   }

   /// <summary>
   /// Returns the rectangle defined by a plan
   /// </summary>
   /// <remarks>Plans with 2 points define a rectanglular area that can be used
   /// for a variety of purposes. This property will return the plan defined by
   /// the plan's two points for such plans.  It will return an empty regtangle
   /// if the plan does not have exactly 2 points.</remarks>
   public virtual Rectangle PlanRectangle
   {
      get
      {
         return Rectangle.Empty;
      }
   }


   #region Sprites
   /// <summary>
   /// Make the specified sprite active.
   /// </summary>
   /// <param name="Target">Specifies which sprite instance to activate</param>
   /// <remarks>If the specified sprite instance is already active, this has
   /// no effect.</remarks>
   [Description("Make the specified sprite active.")]
   public void ActivateSprite(SpriteBase Target)
   {
      Target.isActive = true;
   }

   /// <summary>
   /// Make the specified sprite inactive.
   /// </summary>
   /// <param name="Target">Specifies which sprite instance to deactivate</param>
   /// <remarks>If the specified sprite instance is already inactive, this
   /// has no effect.</remarks>
   [Description("Make the specified sprite inactive.")]
   public void DeactivateSprite(SpriteBase Target)
   {
      Target.isActive = false;
   }

   /// <summary>
   /// Set the position of the target sprite to match that of the source sprite.
   /// </summary>
   /// <param name="Target">Specifies the sprite whose position will change.</param>
   /// <param name="Source">Specifies the sprite whose position is copied.</param>
   /// <remarks>This very simply copies the position from the source sprite to that
   /// of the target sprite. No tests are performed for solidity and no velocity is
   /// changed. The old position of the target sprite, however, is tracked, so it's
   /// still possible to determine if the sprite was touching a tile or plan before
   /// it moved with a function like <see cref="WasSpriteTouching"/>.
   /// <seealso cref="TransportToPoint"/>
   /// <seealso cref="TransportToPlan"/></remarks>
   [Description("Set the position of the target sprite to match that of the source sprite.")]
   public void MatchSpritePosition(SpriteBase Target, SpriteBase Source)
   {
      Target.oldX = Target.x;
      Target.oldY = Target.y;
      Target.x = Source.x;
      Target.y = Source.y;
   }

   /// <summary>
   /// Determines if the specified sprite is touching this plan's rectangle.
   /// </summary>
   /// <param name="sprite">Sprite to test</param>
   /// <returns>True if the sprite's solidity rectangle is touching this plan's rectangle,
   /// otherwise false.</returns>
   /// <remarks>"Touching" means one rectangle is overlapping the other, or the borders
   /// are immediately adjacent. The rectangles are not considered touching if the corners
   /// are only diagonally adjacent (kitty-corner). There must be some length of adjecent
   /// edge. This ensures that a plan that is blocked off by two diagonally-arranged
   /// solid blocks can't be activated through the crack.</remarks>
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

   /// <summary>
   /// Determines if the specified part of the specified sprite is within the plan's rectangle.
   /// </summary>
   /// <param name="sprite">Sprite whose position will be tested</param>
   /// <param name="RelativePosition">Specifies a point within the sprite to test</param>
   /// <returns>True if the specified point within the sprcified sprite's solidity rectangle is
   /// within the plan's rectangle.</returns>
   /// <remarks>Unlike <see cref="IsSpriteTouching"/>, this can only return true when the sprite
   /// and the plan's rectangle actually overlap because the point is inside the sprite's
   /// rectangle, and must also be inside the plan's rectangle to return true.</remarks>
   [Description("Returns true if the specified part of the specified sprite is within the plan's rectangle")]
   public bool IsSpriteWithin(SpriteBase sprite, RelativePosition RelativePosition)
   {
      System.Drawing.Point rp = sprite.GetRelativePosition(RelativePosition);
      Rectangle targetRect = PlanRectangle;
      return targetRect.Contains(rp);
   }

   /// <summary>
   /// Determines if the specified sprite was touching this plan's rectangle in the previous frame.
   /// </summary>
   /// <param name="sprite">Sprite to test</param>
   /// <returns>True if the specified sprite was touching this plan's rectangle previously, or false
   /// if it wasn't.</returns>
   /// <remarks>This function is identical to <see cref="IsSpriteTouching"/> except that it operates
   /// on the sprite's previous position instead of its current position. This is useful for
   /// determining if the sprite just started touching a plan's rectangle or was already touching
   /// it. Often times it's desirable to only perform an automated action like a message only
   /// when a sprite initially touches a plan's rectangle. It's not as desirable for plan rules
   /// that also require other conditions to activate.
   /// Consider, for example, a plan that displays
   /// a message when the sprite touches it. This plan can automatically display the message only
   /// when the sprite first touches the plan, and everything will be fine. But what happens if
   /// it's also required that a button be pressed to activate the rule?  The player may start
   /// touching the plan without touching the button.  Then when the player presses the button,
   /// the rule won't activate because this is no longer the frame when the sprite initially
   /// started touching the plan, so the plan may never get activated.
   /// </remarks>
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


   /// <summary>
   /// Scroll all layers on this plan's layer's map so that the specified sprite is within the visible area of the map.
   /// </summary>
   /// <param name="Sprite">Specified which sprite will be scrolled into view.</param>
   /// <param name="UseScrollMargins">If true, the layer will scroll the sprite into the scroll margins
   /// of the map, otherwise it will only be scrolled just far enough for the sprite to be fully visible
   /// on the display.</param>
   /// <remarks>For multi-player games, is may be desirable to set UseScrollMargins to false
   /// to allow at least one player to get closer to the edge of the screen without trying to
   /// keep it scrolled so strictly within the scroll margin area.
   /// <seealso cref="PushSpriteIntoView"/></remarks>
   [Description("Scroll all layers on this plan's layer's map so that the specified sprite is within the visible area of the map.  If UseScrollMargins is true, the layer will scroll the sprite into the scroll margins of the map.")]
   public void ScrollSpriteIntoView(SpriteBase Sprite, bool UseScrollMargins)
   {
      ParentLayer.ScrollSpriteIntoView(Sprite, UseScrollMargins);
   }

   /// <summary>
   /// Alter a sprite's velocity so that it remains within the map's visible area.
   /// </summary>
   /// <param name="Sprite">Sprite that should be pushed</param>
   /// <param name="StayInScrollMargins">True to push the sprite until it is within the scroll
   /// margins of the map or false to push it only until it is fully visible.</param>
   /// <remarks>This can be useful in multi-player games to not only ensure that the view
   /// remains focused on a particular sprite, but also to ensure that another sprite
   /// (another player) can't leave the view (in cases where both players are shown in
   /// the same view). <seealso cref="ScrollSpriteIntoView"/></remarks>
   [Description("Alter a sprite's velocity so that it remains within the map's visible area or within the scroll margins.")]
   public void PushSpriteIntoView(SpriteBase Sprite, bool StayInScrollMargins)
   {
      ParentLayer.PushSpriteIntoView(Sprite, StayInScrollMargins);
   }

   /// <summary>
   /// Alter the velocity of the specified sprite to go toward a coordinate associated with the current plan.
   /// </summary>
   /// <param name="Sprite">Sprite whose velocity will be affected</param>
   /// <param name="CoordinateIndex">Indicates the 0-based index of the coordinate in the current plan toward which the sprite will be pushed</param>
   /// <param name="Force">How hard to push the sprite in tenths of a pixel per frame per frame</param>
   /// <remarks>Use this in combination with <see cref="CheckNextCoordinate"/> to make a
   /// sprite follow a series of coordinates in a path. This function is identical to
   /// <see cref="PushSpriteTowardPoint"/> except that it is designed only to operate on
   /// coordinates within the current plan (designated by coordinate index).</remarks>
   /// <example>
   /// See <see cref="StopSprite"/> for an example.
   /// </example>
   [Description("Alter the velocity of the specified sprite to go toward a coordinate associated with the current plan.  CoordinateIndex indicates which coordinate in the plan to head toward, and Force is how hard to push the sprite in tenths of a pixel per frame per frame")]
   public void PushSpriteTowardCoordinate(SpriteBase Sprite, int CoordinateIndex, int Force)
   {
      PushSpriteTowardPoint(Sprite, this[CoordinateIndex], Force);
   }

   /// <summary>
   /// Alter the velocity of the specified sprite to go toward a specified location.
   /// </summary>
   /// <param name="Sprite">The sprite whose velocity will be affected</param>
   /// <param name="Target">Specifies a point toward which the sprite will be pushed</param>
   /// <param name="Force">How hard to push the sprite in tenths of a pixel per frame per frame</param>
   /// <remarks>This function is a more generalized form of <see cref="PushSpriteTowardCoordinate"/>.
   /// Rather than pushing a sprite toward a coordinate in the current plan, it can push it toward
   /// any location that can be represented as a point.</remarks>
   /// <example>
   /// This example pushes the sprite toward the mouse location.
   /// <code>PushSpriteTowardPoint(m_ParentLayer.m_Plasma_1, ParentLayer.GetMousePosition(), 40);</code>
   /// </example>
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

   /// <summary>
   /// Make a sprite move toward the next coordinate in the plan when appropriate.
   /// </summary>
   /// <param name="Sprite">Sprite that is following coordinates in this plan</param>
   /// <param name="CoordinateIndex">Zero-based index of the coordinate within the plan toward which the sprite is currently heading.</param>
   /// <param name="WaitCounter">A variable that is used to count frames while this sprite is waiting at a coordinate for a time period specified by the coordinate's <see cref="Coordinate.weight"/> to elapse.</param>
   /// <returns>The index of the coordinate toward which the sprite should be heading.</returns>
   /// <remarks><para>This function determines if the sprite should head toward the next coordinate by
   /// checking if the sprite is within this plan's <see cref="TargetDistance"/> of the specified
   /// coordinate. If it is, then it checks the coordinate's <see cref="Coordinate.weight"/>,
   /// to see if the sprite is supposed to wait at this coordinate.  If it's not supposed to wait,
   /// the function returns the next coordinate index right away. If it is supposed to wait,
   /// the function will only return thye next coordinate index if the sprite has waited the
   /// specified number of frames at the current coordinate.</para>
   /// <para>Normally two sprite parameters are used in conjunction with a sprite that follows
   /// a series of coordinates in a plan, and they are both passed into this function. One
   /// parameter tracks the index of the coordinate toward which the the sprite is currently
   /// heading. The other tracks how long the sprite has waited at the current coordinate.
   /// The coordinate parameter is passed in as the the input for <paramref name="CoordinateIndex"/>
   /// and also specified to receive the output of the function. The wait counter is passed for
   /// the last parameter and is automatically updated when needed because it is passed by reference.
   /// </para><seealso cref="PushSpriteTowardCoordinate"/></remarks>
   /// <example>
   /// This example demonstrates the common usage of this function on a sprite named "Plasma 1".
   /// <code>m_ParentLayer.m_Plasma_1.CoordIndex = CheckNextCoordinate(m_ParentLayer.m_Plasma_1, m_ParentLayer.m_Plasma_1.CoordIndex, ref m_ParentLayer.m_Plasma_1.WaitCounter);</code>
   /// For a more complete example, see <see cref="StopSprite"/>.
   /// </example>
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

   /// <summary>
   /// Stops the sprite's current motion
   /// </summary>
   /// <param name="Sprite">Sprite to be stopped</param>
   /// <remarks>This stops the current sprite from moving by setting its
   /// <see cref="SpriteBase.dx"/> and <see cref="SpriteBase.dy"/> values
   /// to zero. This may be desired when a sprite reaches a coordinate in
   /// a path where it's supposed to wait, otherwise the sprite may continue
   /// to drift while it waits. One easy way to determine when a sprite is
   /// waiting at a coordinate is to check if the sprite's wait counter parameter
   /// is zero. The wait counter will only be non-zero when the sprite is waiting.
   /// <seealso cref="CheckNextCoordinate"/>
   /// </remarks>
   /// <example>
   /// The following example shows the code used for a sprite named "Plasma 1" that
   /// follows a path and stops and waits at coordinates that have a non-zero weight.
   /// <code lang="C#">
   /// // If active
   /// if (<see cref="IsSpriteActive"/>(m_ParentLayer.m_Plasma_1))
   /// {
   ///    // If not waiting
   ///    if ((m_ParentLayer.m_Plasma_1.WaitCounter == 0))
   ///    {
   ///       // Move sprite towards coordinate
   ///       <see cref="PushSpriteTowardCoordinate"/>(m_ParentLayer.m_Plasma_1, m_ParentLayer.m_Plasma_1.CoordIndex, 40);
   ///    }
   ///    else
   ///    {
   ///       // Else stop sprite
   ///       <see cref="StopSprite"/>(m_ParentLayer.m_Plasma_1);
   ///    }
   ///    // Move to next coordinate
   ///   m_ParentLayer.m_Plasma_1.CoordIndex = <see cref="CheckNextCoordinate"/>(m_ParentLayer.m_Plasma_1, m_ParentLayer.m_Plasma_1.CoordIndex, ref m_ParentLayer.m_Plasma_1.WaitCounter);
   /// }
   /// </code>
   /// </example>
   [Description("Set the velocity of the specified sprite to zero")]
   public void StopSprite(SpriteBase Sprite)
   {
      Sprite.dx = Sprite.dy = 0;
   }

   /// <summary>
   /// Determine whether the specified sprite's collision mask is overlapping part of any sprite in the specified category.
   /// </summary>
   /// <param name="SourceSprite">A sprite that will be checked for collisions</param>
   /// <param name="Targets">A category of sprites against which collisions will be tested</param>
   /// <returns>The 0-based index of the sprite within <paramref name="Targets"/> if a collision is occurring, otherwise -1.</returns>
   /// <remarks>The collision mask is derived from the sprite's Mask Alpha Level setting.
   /// If both sprites being tested have a collision mask, they are checked for overlapping solid bits.
   /// If one sprite has Mask Alpha Level set to 0, then a rectangular mask for that sprite
   /// is synthesized from the solid width and solid height using
   /// <see cref="CollisionMask.GetRectangularMask"/>.
   /// If both sprites have Mask Alpha Level set to 0, then a simple rectangular collision
   /// detection is performed (for improved performance).</remarks>
   [Description("Determine whether the specified sprite's collision mask is overlapping part of any sprite in the specified category. Return the index of the sprite within the category if a collision is occurring, otherwise return -1.")]
   public int TestCollisionMask(SpriteBase SourceSprite, SpriteCollection Targets)
   {
      return SourceSprite.TestCollisionMask(Targets);
   }

   /// <summary>
   /// Determine whether the solidity rectangle of the specified sprite overlaps that of any sprite in the specified category.
   /// </summary>
   /// <param name="SourceSprite">A sprite that will be checked for collisions</param>
   /// <param name="Targets">A category of sprites against which collisions will be tested</param>
   /// <returns>The 0-based index of the sprite within <paramref name="Targets"/> if a collision is occurring, otherwise -1.</returns>
   /// <remarks>This can be used to force a simple rectangular collision test even if one or both
   /// sprites involved have a Mask Alpha level greater than 0.  This method is recommended
   /// for improved performance when pixel-perfect collision detection is not required.
   /// <seealso cref="TestCollisionMask"/></remarks>
   [Description("Determine whether the solidity rectangle of the specified sprite overlaps that of any sprite in the specified category. Return the index of the sprite within the category if a collision is occurring, otherwise return -1.")]
   public int TestCollisionRect(SpriteBase SourceSprite, SpriteCollection Targets)
   {
      return SourceSprite.TestCollisionRect(Targets);
   }

   /// <summary>
   /// Deactivate a sprite within a category given the sprite's index within the category.
   /// </summary>
   /// <param name="Category">Category containing the sprite to deactivate</param>
   /// <param name="Index">Zero-based index within the category of the sprite to be deactivated</param>
   /// <remarks>This can be used in conjunction with the result of a function like
   /// <see cref="TestCollisionRect"/> or <see cref="TestCollisionMask"/> to deactivate the
   /// target sprite when a collision occurs.
   /// <seealso cref="DeactivateSprite"/></remarks>
   [Description("Deactivate a sprite within a category given the sprite's index within the category")]
   public void DeactivateCategorySprite(SpriteCollection Category, int Index)
   {
      Category[Index].isActive = false;
   }

   /// <summary>
   /// Determines if the specified sprite instace is active.
   /// </summary>
   /// <param name="Sprite">Sprite instance to test</param>
   /// <returns>True if the sprite is active, false otherwise.</returns>
   /// <remarks>The main differences between an active sprite and an inactive
   /// sprite are than only active sprites are drawn when they are in the visible
   /// part of the map, and only active sprites' rules are processed. Using
   /// IsActiveSprite is a good way to determine if a sprite is currently valid
   /// for use because inactive sprites should not have any functions running on
   /// them except to activate them. Most functions will trigger an error message
   /// in debug mode if they find that they are operating on an inactive sprite.
   /// </remarks>
   /// <example>See <see cref="StopSprite"/> for an example of
   /// IsSpriteActive.</example>
   [Description("Determines if the specified sprite instace is active.")]
   public bool IsSpriteActive(SpriteBase Sprite)
   {
      return Sprite.isActive;
   }

   /// <summary>
   /// Moves the specified sprite to the specified coordinate.
   /// </summary>
   /// <param name="sprite">Sprite instance to be moved</param>
   /// <param name="target">Location to which the sprite will be moved</param>
   /// <remarks>This very simply sets the position of the sprite to a sprcified
   /// coordinate. No tests are performed for solidity and no velocity is
   /// changed. The old position of the target sprite, however, is tracked, so it's
   /// still possible to determine if the sprite was touching a tile or plan before
   /// it moved with a function like <see cref="WasSpriteTouching"/>.
   /// <seealso cref="MatchSpritePosition"/>
   /// <seealso cref="TransportToPlan"/></remarks>
   /// <example>
   /// The following example could be used to move a sprite to the mouse's location
   /// to make it behave like a mouse cursor.
   /// <code>TransportToPoint(m_ParentLayer.m_Plasma_1, ParentLayer.GetMousePosition());</code>
   /// </example>
   [Description("Moves the specified sprite to the specified coordinate.")]
   public void TransportToPoint(SpriteBase sprite, Point target)
   {
      sprite.oldX = sprite.x;
      sprite.oldY = sprite.y;
      sprite.x = target.X;
      sprite.y = target.Y;
   }

   /// <summary>
   /// Moves the specified sprite to the specified plan's rectangle, aligned to the specified corner/edge.
   /// </summary>
   /// <param name="Sprite">Sprite to be moved</param>
   /// <param name="Plan">Specifies a target location</param>
   /// <param name="Alignment">Specifies how the sprite's solidity rectangle will be aligned with the target plan's rectangle</param>
   /// <remarks>The specified <paramref name="Plan"/> must be a 2-point plan (specifying a rectangle)
   /// or the function will ignore the request (and display an error in debug mode).
   /// The sprite will be aligned inside the rectangle according to <paramref name="Alignment"/>,
   /// so, for example, if RelativePosition.RightMiddle is specified, the midpoint of the
   /// right side of the sprite's solidity rectangle will be aligned to the midpoint of the
   /// right side of the plan's rectangle.</remarks>
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
            Sprite.x = Plan.PlanRectangle.X + Plan.PlanRectangle.Width - Sprite.SolidWidth;
            break;
      }
   }
   
   /// <summary>
   /// Associate the state of the input device for the specified player with the inputs on the specified sprite.
   /// </summary>
   /// <param name="PlayerNumber">A number from 1 to <see cref="Project.MaxPlayers"/> indicating which
   /// player's input settings to use.</param>
   /// <param name="Target">Specifies which sprite the player should control</param>
   /// <remarks>This is the basic means by which the input device, as customized for a
   /// particular player, is associated with a sprite, so that the input will control
   /// the sprite for that player. In many cases, it may be easier to use the sprite
   /// definition's version of <see cref="SpriteBase.MapPlayerToInputs"/> so that this
   /// doesn't have to be repeated on every map, however in a multi-player game, it may be
   /// easier or necessary to distinguish the individual players on each map if each player
   /// uses the same sprite definition.</remarks>
   [Description("Associate the state of the input device for the specified player (1-4) with the inputs on the specified sprite.")]
   public void MapPlayerToInputs(int PlayerNumber, SpriteBase Target)
   {
      Target.MapPlayerToInputs(PlayerNumber);
   }

   #endregion

   /// <summary>
   /// Retrieves a list of coordinates contained in this plan
   /// </summary>
   protected virtual Coordinate[] Coordinates
   {
      get
      {
         return null;
      }
   }

   /// <summary>
   /// Retrieves the coordinate specified by a 0-based index for this plan
   /// </summary>
   public Coordinate this[int index]
   {
      get
      {
         return Coordinates[index];
      }
   }

   /// <summary>
   /// Retrieves the number of coordinates that this plan contains
   /// </summary>
   public int Count
   {
      get
      {
         if (Coordinates == null)
            return 0;
         return Coordinates.Length;
      }
   }

   /// <summary>
   /// Executes this plan's rules if any exist.
   /// </summary>
   /// <remarks>An error is raised if no rules exist on this plan.</remarks>
   public virtual void ExecuteRules()
   {
      throw new NotImplementedException("Attempted to execute rules on plan " + this.GetType().Name + " without any rules");
   }

   /// <summary>
   /// Specifies one of the 4 color channels: alpha, red, green or blue.
   /// </summary>
   public enum ColorChannel
   {
      Blue,
      Green,
      Red,
      Alpha
   }

   /// <summary>
   /// Modulate/scale the specified color channel of the specified sprite to the specified level.
   /// </summary>
   /// <param name="Sprite">Sprite whose appearance will be altered/restored</param>
   /// <param name="Channel">Which color channel will be affected</param>
   /// <param name="Level">Specifies the level to which the channel will be modulated</param>
   /// <remarks>Modulating a color channel means that its output will be scaled down to the
   /// specified level</remarks>
   /// <example>The following example scales the blue channel to 128 (half) which will cause the
   /// sprite to appear more yellow, green or red than normal:
   /// <code>ModulateColor(m_ParentLayer.m_Player_1, ColorChannel.Blue, 128);</code>
   /// The following example scales the alpha channel to 128 (half) which will cause the
   /// sprite to appear semi-transparent:
   /// <code>ModulateColor(m_ParentLayer.m_Player_1, ColorChannel.Alpha, 128);</code>
   /// </example>
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
   /// <summary>
   /// Specifies a drawing style for inventory/status-type bars drawn with <see cref="DrawCounterAsTile"/>.
   /// </summary>
   public enum DrawStyle
   {
      /// <summary>
      /// The specified tile is drawn stretched to fill the width of plan, and the right side
      /// is cropped according to the counter value.
      /// </summary>
      /// <remarks>When the counter reaches the maximum, the full tile is drawn, otherwise
      /// the clipping size is scaled according to the ratio of the counter's current value to
      /// its maximum value. The tile image is only stretched horizontally. The vertical size
      /// remains at the tile's original height.</remarks>
      ClipRightToCounter,
      /// <summary>
      /// The specified tile is drawn stretched rightward according to the counter value.
      /// </summary>
      /// <remarks>When the counter reaches its maximum, the tile will fill the plan rectangle's width.
      /// Otherwise the image is scaled down horizontally to fill the left portion of the plan
      /// rectangle as determined by the ratio of the counter's current value to its
      /// maximum value.</remarks>
      StretchRightToCounter,
      /// <summary>
      /// The specified tile is draw unscaled from left to right, repeated according
      /// to the counter value and plan size.
      /// </summary>
      /// <remarks>When the counter reaches its maximum, the tile will be repeated from the
      /// plan's left side to its right side, as many whole copies as can fit in the plan
      /// rectangle.  Otherwise, the number of copies of the tile that are drawn is based
      /// on the counter value and the maximum number than can be drawn in the plan rectangle.
      /// The result is rounded to a whole number (partial tiles are not drawn). Because of
      /// the way this style works, you will probably want to make sure the number of copies
      /// of the tile that can fit in the plan rectangle exactly matches the counter's maximum
      /// value.</remarks>
      RepeatRightToCounter,
      /// <summary>
      /// The specified tile is drawn stretched to fill the height of the plan, and the top is
      /// cropped according to the counter value.
      /// </summary>
      /// <remarks>When the counter reaches the maximum, the full tile is drawn, otherwise
      /// the clipping size is scaled according to the ratio of the counter's current value to
      /// its maximum value. The tile image is only stretched vertically. The horizontal size
      /// remains at the tile's original width.</remarks>
      ClipTopToCounter,
      /// <summary>
      /// The specified tile is drawn stretched upward from the bottom of the plan according
      /// to the counter value.
      /// </summary>
      /// <remarks>When the counter reaches its maximum, the tile will fill the plan rectangle's height.
      /// Otherwise the image is scaled down vertically to fill the bottom portion of the plan
      /// rectangle as determined by the ratio of the counter's current value to its
      /// maximum value.</remarks>
      StretchTopToCounter,
      /// <summary>
      /// The specified tile is draw unscaled from bottom to top, repeated according
      /// to the counter value and plan size.
      /// </summary>
      /// <remarks>When the counter reaches its maximum, the tile will be repeated from the
      /// plan's bottom to its top, as many whole copies as can fit in the plan rectangle.
      /// Otherwise, the number of copies of the tile that are drawn is based on the counter
      /// value and the maximum number than can be drawn in the plan rectangle.
      /// The result is rounded to a whole number (partial tiles are not drawn). Because of
      /// the way this style works, you will probably want to make sure the number of copies
      /// of the tile that can fit in the plan rectangle exactly matches the counter's maximum
      /// value.</remarks>
      RepeatUpToCounter
   }

   /// <summary>
   /// Draw the value of a counter as a bar filled with a tile's image.
   /// </summary>
   /// <param name="TileIndex">Specifies the index of a tile from this plan's layer</param>
   /// <param name="counter">Specifies which counter's value is being drawn</param>
   /// <param name="style">Specifies a style with which the tile will fill the bar</param>
   /// <remarks>The size of the plan determines the maximum proportions of the bar.
   /// The counter value determines the drawn/current size of the bar.
   /// See <see cref="DrawStyle"/> for details about the different ways in which the bar
   /// can be filled.</remarks>
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

   /// <summary>
   /// Display a counter value as a number with a label in the current plan's rectangle.
   /// </summary>
   /// <param name="Label">String containing the text of the label</param>
   /// <param name="counter">Counter whose value will be displayed</param>
   /// <param name="color">Color of the label and quantity text</param>
   /// <remarks>The label and quantity are merged into a single string of
   /// text and drawn at the top left corner of the plan's rectangle.</remarks>
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

   private void CopyTiles(PlanBase Source, PlanBase Target, RelativePosition RelativePosition)
   {
      int src_left = (int)(Source.PlanRectangle.X / Source.ParentLayer.Tileset.TileWidth);
      int src_top = (int)(Source.PlanRectangle.Y / Source.ParentLayer.Tileset.TileHeight);
      int src_right = (int)((Source.PlanRectangle.X + Source.PlanRectangle.Width - 1) / Source.ParentLayer.Tileset.TileWidth);
      int src_bottom = (int)((Source.PlanRectangle.Y + Source.PlanRectangle.Height - 1) / Source.ParentLayer.Tileset.TileHeight);

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
            
            Target.ParentLayer[targetx,targety] = Source.ParentLayer[x,y];
         }
      }
   }

   /// <summary>
   /// Copy tiles from this plan's rectangle to another plan's rectangle.
   /// </summary>
   /// <param name="Target">Specifies another plan specifying a location to which tiles will be copied</param>
   /// <param name="RelativePosition">Specifies the alignment of the tiles in the target rectangle if
   /// this plan's rectangle is a different size</param>
   /// <remarks>If the source rectangle is larger than the target rectangle, the copy
   /// locations will be aligned according to RelativePosition, and the copied tiles
   /// will be allowed to overflow the target rectangle.  For example, if the alignment
   /// is <see cref="RelativePosition.RightMiddle"/>, the right middle tile of the source
   /// rectangle will be copied into the right middle tile of the target rectangle, and
   /// build around there regardless of the target rectangle's size.</remarks>
   [Description("Copy tiles from this plan's rectangle to another plan's rectangle.")]
   public void CopyTo(PlanBase Target, RelativePosition RelativePosition)
   {
      CopyTiles(this, Target, RelativePosition);
   }

   /// <summary>
   /// Copy tiles from the specified plan's rectangle to this plan's rectangle.
   /// </summary>
   /// <param name="Source">Specifies another plan specifying a location from which tiles will be copied</param>
   /// <param name="RelativePosition">Specifies the alignment of the tiles in this plan's rectangle if
   /// the source plan's rectangle is a different size</param>
   /// <remarks>If the source rectangle is larger than the target rectangle, the copy
   /// locations will be aligned according to RelativePosition, and the copied tiles
   /// will be allowed to overflow the target rectangle.  For example, if the alignment
   /// is <see cref="RelativePosition.RightMiddle"/>, the right middle tile of the source
   /// rectangle will be copied into the right middle tile of the target rectangle, and
   /// build around there regardless of the target rectangle's size.</remarks>
   [Description("Copy tiles from the specified plan's rectangle to this plan's rectangle.")]
   public void CopyFrom(PlanBase Source, RelativePosition RelativePosition)
   {
      CopyTiles(Source, this, RelativePosition);
   }

   /// <summary>
   /// Determine if the specified sprite's specified input is pressed.
   /// </summary>
   /// <param name="Sprite">Sprite whose inputs will be examined</param>
   /// <param name="Input">Specifies which input will be examined</param>
   /// <param name="InitialOnly">When true only return true if the input has just been pressed and was not pressed before</param>
   /// <returns>True if the input is pressed, false otherwise.</returns>
   /// <remarks>The <see cref="SpriteBase.IsInputPressed"/> function for sprite definitions is
   /// more commonly used, but this allows you to test a specific sprite's inputs on a specific
   /// layer.</remarks>
   [Description("Determine if the specified sprite's specified input is pressed.  InitialOnly causes this to return true only if the input has just been pressed and was not pressed before.")]
   public bool IsInputPressed(SpriteBase Sprite, SpriteBase.InputBits Input, bool InitialOnly)
   {
      return Sprite.IsInputPressed(Input, InitialOnly);
   }

   /// <summary>
   /// Ensure that all the inputs currently being pressed on the specified sprite are henceforth processed as already pressed.
   /// </summary>
   /// <param name="Sprite">Sprite whose inputs will be shifted.</param>
   /// <remarks>This is usually handled by <see cref="MapPlayerToInputs"/>, but if you
   /// have customized the factors that affect the inputs, you may need to manually
   /// shift the current inputs into the old inputs to allow correct handling for
   /// "InitialOnly" parameters.</remarks>
   [Description("Ensure that all the inputs currently being pressed on the specified sprite are henceforth processed as already pressed.")]
   public void CopyInputsToOld(SpriteBase Sprite)
   {
      Sprite.oldinputs = Sprite.inputs;
   }

   /// <summary>
   /// Create a new (dynamic) instance of the specified sprite type at the first coordinate in this plan.
   /// </summary>
   /// <param name="SpriteDefinition">Specifies the type of sprite that will be created.</param>
   /// <param name="RelativePosition">Specified a coordinate within the sprite that should be
   /// matched up with the first coordinate of the plan.</param>
   /// <remarks><para>As opposed to <see cref="ActivateSprite"/>, which activates a pre-defined
   /// instance of a sprite, this function will actually create a new sprite instance on the
   /// fly (referred to as a "dynamic" sprite instance). Dynamic sprites cannot be referred
   /// to directly by other rules because they don't have names associated with them, therefore
   /// it's recommended that <see cref="ActivateSprite"/> be used when more control is
   /// desired. However, <see cref="AddSpriteAtPlan"/> doesn't require you to pre-define each
   /// possible instance.</para>
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
   /// <para>All parameters of a newly created sprite instance are always initialized
   /// to zero. Furthermore, new sprite instances will not refer to any solidity,
   /// and will begin in the first
   /// frame of the first state, but you can use rules to affect <see cref="GeneralRules.lastCreatedSprite"/>
   /// to set values of the new sprite, or define rules on the created sprite type to make it
   /// initialize itself appropriately.</para>
   /// <para>If this plan has no coordinates, the sprite is created at the top left corner of
   /// the layer, and not adjusted according to <paramref name="RelativePosition"/>.
   /// </para><seealso cref="SpriteBase.AddSpriteHere"/><seealso cref="SpriteBase.TileAddSprite"/></remarks>
   [Description("Add a new instance of the specified sprite type to this plan's layer such that the specified position within the sprite corresponds to the first coordinate in this plan")]
   public void AddSpriteAtPlan([Editor("SpriteDefinition", "UITypeEditor")] System.Type SpriteDefinition, RelativePosition RelativePosition)
   {
      System.Reflection.ConstructorInfo constructor = SpriteDefinition.GetConstructor(new System.Type[]
      {
         typeof(LayerBase), typeof(double), typeof(double), typeof(double), typeof(double), typeof(int), typeof(int), typeof(bool), typeof(Display), typeof(Solidity), typeof(int), typeof(bool)
      });
      lastCreatedSprite = (SpriteBase)constructor.Invoke(new object[]
      {
         ParentLayer, 0, 0, 0, 0, 0, 0, true, ParentLayer.ParentMap.Display, null, -1, true
      });
      if (Count > 0)
      {
         System.Drawing.Point offset = lastCreatedSprite.GetRelativePosition(RelativePosition);
         lastCreatedSprite.x = Coordinates[0].x - offset.X;
         lastCreatedSprite.y = Coordinates[0].y - offset.Y ;
      }
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