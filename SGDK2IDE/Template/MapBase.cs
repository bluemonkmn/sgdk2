using System;

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

   public Display Display
   {
      get
      {
         return m_Display;
      }
   }
}
