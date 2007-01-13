using System;
using System.Drawing;

/// <summary>
/// Base class for all maps.
/// </summary>
[Serializable()]
public abstract class MapBase
{
   protected Display m_Display;
   public Type m_CameFromMapType = null;
   private ViewLayout viewLayout = ViewLayout.Single;
   private static byte currentViewIndex = 0;
   public int MapFlags = 0;

   public MapBase(Display Disp)
   {
      m_Display = Disp;
   }

   protected abstract void Draw();

   public abstract void ExecuteRules();

   /// <summary>
   /// Scroll all layers to the specified coordinates after calculating relative scroll rates
   /// </summary>
   /// <param name="position">New coordinate. int.MinValue indicates no scrolling on this axis.</param>
   public abstract void Scroll(Point position);

   public abstract short ScrollMarginLeft
   {
      get;
   }
   public abstract short ScrollMarginTop
   {
      get;
   }
   public abstract short ScrollMarginRight
   {
      get;
   }
   public abstract short ScrollMarginBottom
   {
      get;
   }

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
   /// </summary>
   public Rectangle CurrentView
   {
      get
      {
         return GetView(CurrentViewIndex);
      }
   }

   /// <summary>
   /// Which view is currently being processed (0 to Project.MaxViews - 1)
   /// </summary>
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
   /// Draws all the views according to the map's current ViewLayout
   /// </summary>
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

   public Display Display
   {
      get
      {
         return m_Display;
      }
   }
}
