using System;

/// <summary>
/// This class provides global, static data. No instances are created.
/// </summary>
/// <remarks>This class was created to serve as an interface between
/// generated code and project custom code.  It exists so that the
/// Highest level code (including the entry point) can be customized
/// while still allowing the code generator to provide some high level
/// global properties in the Project.resx file.</remarks>
public class Project
{
   private static System.Resources.ResourceManager m_res;
   private static GameForm game;
   
   public static readonly System.Byte MaxPlayers;   
   public static readonly System.Byte MaxViews;
   public static readonly string GameCredits;

   static Project()
   {
      Project.m_res = new System.Resources.ResourceManager(typeof(Project));
      MaxPlayers = byte.Parse(m_res.GetString("_MaxPlayers"));
      MaxViews = byte.Parse(m_res.GetString("_MaxViews"));
      GameCredits = m_res.GetString("_GameCredits");
   }

   public static System.Resources.ResourceManager Resources
   {
      get
      {
         return m_res;
      }
   }

   public static GameForm GameWindow
   {
      get
      {
         return Project.game;
      }
   }

   public static void Main()
   {
      try
      {
         GameDisplayMode mode = (GameDisplayMode)System.Enum.Parse(typeof(GameDisplayMode), m_res.GetString("_DisplayMode"));
         bool windowed = bool.Parse(m_res.GetString("_Windowed"));
         string windowTitle = m_res.GetString("_WindowTitle");
         System.Type startupMapType = System.Reflection.Assembly.GetExecutingAssembly().GetType(m_res.GetString("_StartupMapType").Replace(" ","_") + "_Map", true);
         System.Type overlayMapType = null;
         if (m_res.GetString("_OverlayMapType") != null)
             overlayMapType = System.Reflection.Assembly.GetExecutingAssembly().GetType(m_res.GetString("_OverlayMapType").Replace(" ","_") + "_Map", true);

         Project.game = new GameForm(mode, windowed, windowTitle, startupMapType, overlayMapType);
         game.Show();
         game.Run();
      }
      catch (System.Exception ex)
      {
         GameForm.HandleException(ex);
      }
   }
}
