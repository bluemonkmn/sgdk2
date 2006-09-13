using System;
using System.Drawing;

/// <summary>
/// Base class for all maps.
/// </summary>
public abstract class MapBase
{
   protected Display m_Display;
    
   public MapBase(Display Disp)
   {
      m_Display = Disp;
   }

   public abstract void Draw();

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

   public virtual Rectangle View
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
