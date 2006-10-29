using System;
using System.ComponentModel;

/// <summary>
/// Implements rules common to sprites and plans
/// </summary>
public abstract class GeneralRules
{
   public abstract LayerBase ParentLayer
   {
      get;
   }

   [Description("Write a string to the debug output without moving to the next line"),
   System.Diagnostics.Conditional("DEBUG")]
   public void LogDebugLabel(string Label)
   {
      Project.GameWindow.debugText.Write(Label);
   }

   [Description("Write a number to the debug output and move to the next line"),
   System.Diagnostics.Conditional("DEBUG")]
   public void LogDebugValue(int DebugValue)
   {
      Project.GameWindow.debugText.WriteLine(DebugValue.ToString());
   }

   [Description("Sets a different map as the one to be drawn on the game display.  If UnloadCurrent is true, the current map will be unloaded first (which causes it to be recreated/reset when returning to it).")]
   public void SwitchToMap([Editor("MapType", "UITypeEditor")] Type MapType, bool UnloadCurrent)
   {
      if (UnloadCurrent)
         Project.GameWindow.UnloadMap(Project.GameWindow.CurrentMap.GetType());
      Project.GameWindow.CurrentMap = Project.GameWindow.GetMap(MapType);
   }

   [Description("Unloads the specified map, which will force it to be recreated/reset next time it is used.")]
   public void UnloadMap([Editor("MapType", "UITypeEditor")] Type MapType)
   {
      Project.GameWindow.UnloadMap(MapType);
   }
}