/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */
using System;
using System.Drawing;

/// <summary>
/// Base class for all maps.
/// </summary>
[Serializable()]
public abstract partial class MapBase
{
   protected Display m_Display;
   /// <summary>
   /// Stores the type of the map that was previously the current map.
   /// </summary>
   /// <value>A <see cref="System.Type"/> referring to the type of the previously
   /// active map if any existed, or a null reference if there was none.</value>
   /// <remarks>This is set by <see cref="GeneralRules.SwitchToMap"/> and used by
   /// <see cref="GeneralRules.ReturnToPreviousMap"/>.</remarks>
   public Type m_CameFromMapType = null;
   private ViewLayout viewLayout = ViewLayout.Single;
   private static byte currentViewIndex = 0;
   /// <summary>
   /// Stores a set of arbitrary boolean/flag values that can be used for any purpose
   /// on each map.
   /// </summary>
   /// <remarks>Technically this value can be used to store any data that will
   /// fit in a 32-bit integer, but it's designed to be used with functions in
   /// <see cref="GeneralRules"/> to store individually accessible bits. These
   /// can be set with <see cref="GeneralRules.SetMapFlag"/> and checked with
   /// <see cref="GeneralRules.IsMapFlagOn"/>.</remarks>
   public int MapFlags = 0;

   /// <summary>
   /// Constructs a new map and links it to the display.
   /// </summary>
   /// <param name="Disp">Display object on which the map will be drawn</param>
   public MapBase(Display Disp)
   {
      m_Display = Disp;
   }

   /// <summary>
   /// Draws all the layers of this map in the current view.
   /// </summary>
   /// <remarks>To draw all layers for all views, see <see cref="DrawAllViews"/>.
   /// <seealso cref="CurrentView"/>
   /// <seealso cref="CurrentViewIndex"/></remarks>
   protected abstract void Draw();

   /// <summary>
   /// Execute the rules of each layer in the map.
   /// </summary>
   /// <remarks>This executes all the rules for all the plans and active sprites for all layers
   /// in the map. See <see cref="ExecuteRules"/> for information on overiding this.</remarks>
   public abstract void ExecuteRulesInternal();

   /// <summary>
   /// Allows customization of the way <see cref="ExecuteRulesInternal"/> is called.
   /// </summary>
   /// <remarks>The default implementation simply calls ExecuteRulesInternal,
   /// but a partial class on the class derived from this base class may
   /// override this behavior and call ExecuteRulesInternal conditionally.</remarks>
   public virtual void ExecuteRules() { ExecuteRulesInternal(); }

   /// <summary>
   /// Scroll all layers to the specified coordinates after calculating relative scroll rates
   /// </summary>
   /// <param name="position">New coordinate. int.MinValue indicates no scrolling on this axis.</param>
   public abstract void Scroll(Point position);

   /// <summary>
   /// Return the left scroll margin for this map
   /// </summary>
   /// <remarks>Scroll margins are used to maintain some distance between a sprite and the
   /// edge of the map. Functions that scroll the map generally call
   /// <see cref="LayerBase.ScrollSpriteIntoView"/>, which, when given the appropriate
   /// parameters, will use the scroll margin settings on the map to ensure that the sprite
   /// is scrolled far enough into the view that the scroll margins are maintained.</remarks>
   public abstract short ScrollMarginLeft
   {
      get;
   }
   /// <summary>
   /// Return the top scroll margin for this map
   /// </summary>
   /// <remarks>Scroll margins are used to maintain some distance between a sprite and the
   /// edge of the map. Functions that scroll the map generally call
   /// <see cref="LayerBase.ScrollSpriteIntoView"/>, which, when given the appropriate
   /// parameters, will use the scroll margin settings on the map to ensure that the sprite
   /// is scrolled far enough into the view that the scroll margins are maintained.</remarks>
   public abstract short ScrollMarginTop
   {
      get;
   }
   /// <summary>
   /// Return the right scroll margin for this map
   /// </summary>
   /// <remarks>Scroll margins are used to maintain some distance between a sprite and the
   /// edge of the map. Functions that scroll the map generally call
   /// <see cref="LayerBase.ScrollSpriteIntoView"/>, which, when given the appropriate
   /// parameters, will use the scroll margin settings on the map to ensure that the sprite
   /// is scrolled far enough into the view that the scroll margins are maintained.</remarks>
   public abstract short ScrollMarginRight
   {
      get;
   }
   /// <summary>
   /// Return the bottom scroll margin for this map
   /// </summary>
   /// <remarks>Scroll margins are used to maintain some distance between a sprite and the
   /// edge of the map. Functions that scroll the map generally call
   /// <see cref="LayerBase.ScrollSpriteIntoView"/>, which, when given the appropriate
   /// parameters, will use the scroll margin settings on the map to ensure that the sprite
   /// is scrolled far enough into the view that the scroll margins are maintained.</remarks>
   public abstract short ScrollMarginBottom
   {
      get;
   }

   /// <summary>
   /// Gets or sets the current arrangement of views within the display.
   /// </summary>
   /// <remarks>By changing this you can affect the number and arrangement of views
   /// that are being managed by the map and its layers. Activating a multi-view layout
   /// will cause the layers to maintain multiple scroll positions, one for each view.
   /// And when a layer is scrolled, it will only be scrolled for the current view (as
   /// indicated by <see cref="CurrentViewIndex"/>.  The number of views must not exceed
   /// the maximum set in the project and stored in <see cref="Project.MaxViews"/>.
   /// The view layout can be set from a rule function by calling
   /// <see cref="GeneralRules.SetViewLayout"/>, and the current view can be changed with
   /// <see cref="GeneralRules.CurrentView"/>.
   /// <seealso cref="CurrentView"/>
   /// <seealso cref="CurrentViewIndex"/>
   /// <seealso cref="GetView"/>
   /// </remarks>
   public ViewLayout ViewLayout
   {
      get
      {
         return viewLayout;
      }
      set
      {
         if (value == ViewLayout.FourCorners)
            if (Project.MaxViews < 4)
            {
               System.Diagnostics.Debug.Fail("Attempted to use 4-corner view with max views less than 4");
               return;
            }
         if ((value == ViewLayout.TopBottom) || (value == ViewLayout.LeftRight))
            if (Project.MaxViews < 2)
            {
               System.Diagnostics.Debug.Fail("Attempted to use double view with max views less than 2");
               return;
            }
         viewLayout = value;
      }
   }

   /// <summary>
   /// Retrieve the area within the display that the current view occupies.
   /// </summary>
   /// <param name="viewNumber">Number from 0 to <see cref="Project.MaxViews"/>-1 specifying
   /// which view to retrieve.</param>
   /// <returns>Rectangle containing the area on the display object occupied by the view.</returns>
   /// <remarks>Views are numbered in reading order, left to right and top to bottom.
   /// <seealso cref="ViewLayout"/>
   /// <seealso cref="CurrentView"/>
   /// <seealso cref="CurrentViewIndex"/>
   /// </remarks>
   public Rectangle GetView(int viewNumber)
   {
      Rectangle result = TotalView;
      switch (viewLayout)
      {
         case ViewLayout.Single:
            return result;
         case ViewLayout.TopBottom:
            result.Height /= 2;
         switch (viewNumber)
         {
            case 0:
               return result;
            default:
               result.Y += TotalView.Height / 2;
               return result;
         }
         case ViewLayout.LeftRight:
            result.Width /= 2;
         switch(viewNumber)
         {
            case 0:
               return result;
            default:
               result.X += TotalView.Width / 2;
               return result;
         }
         default:
            result.Width /= 2;
            result.Height /= 2;
         switch(viewNumber)
         {
            case 0:
               return result;
            case 1:
               result.X += TotalView.Width / 2;
               return result;
            case 2:
               result.Y += TotalView.Height / 2;
               return result;
            default:
               result.X += TotalView.Width / 2;
               result.Y += TotalView.Height / 2;
               return result;
         }
      }
   }

   /// <summary>
   /// Returns the rectangle for the currently active view within the map's total view area
   /// <seealso cref="GetView"/>
   /// </summary>
   public Rectangle CurrentView
   {
      get
      {
         return GetView(CurrentViewIndex);
      }
   }

   /// <summary>
   /// Which view is currently being processed (0 to <see cref="Project.MaxViews"/> - 1)
   /// </summary>
   /// <remarks>
   /// This can be retrieved and set from rule functions by using
   /// <see cref="GeneralRules.CurrentView"/>.
   /// <seealso cref="ViewLayout"/>
   /// </remarks>
   public byte CurrentViewIndex
   {
      get
      {
         return currentViewIndex;
      }
      set
      {
         if ((value >= 0) && (value < Project.MaxViews))
            currentViewIndex = value;
         else
            System.Diagnostics.Debug.Fail("Bad CurrentViewIndex value ignored");
      }
   }

   /// <summary>
   /// Draws all layers in the map on all views according to the map's current ViewLayout
   /// </summary>
   /// <remarks>Each layer is drawn according to its respective scroll position in the view.
   /// currentViewIndex is reset to 0 after this function called.
   /// <seealso cref="MapBase.ViewLayout"/></remarks>
   public void DrawAllViews()
   {
      switch(viewLayout)
      {
         case ViewLayout.Single:
            currentViewIndex = 0;
            Draw();
            break;
         case ViewLayout.TopBottom:
         case ViewLayout.LeftRight:
            for (currentViewIndex = 0; currentViewIndex < 2; currentViewIndex++)
               Draw();
            currentViewIndex = 0;
            break;
         case ViewLayout.FourCorners:
            for (currentViewIndex = 0; currentViewIndex < 4; currentViewIndex++)
               Draw();
            currentViewIndex = 0;
            break;
      }
   }

   /// <summary>
   /// Returns the entire view area for this map (including all sub-views if multiple views exist)
   /// </summary>
   /// <remarks>The total view area is based on the size of the display, which depends on the
   /// display mode setting of the project. Changing this is not supported.</remarks>
   public virtual Rectangle TotalView
   {
      get
      {
         return m_Display.DisplayRectangle;
      }
      set
      {
         System.Diagnostics.Debug.Assert(System.Drawing.Rectangle.Intersect(m_Display.DisplayRectangle, value).Equals(value));
      }
   }

   /// <summary>
   /// Returns the display on which this map is drawn.
   /// </summary>
   public Display Display
   {
      get
      {
         return m_Display;
      }
   }
}
