/*
 * Scrolling Game Development Kit 2.0
 * Generated Project Template Code
 * 
 * Copyright © 2000 - 2007 Benjamin Marty <bluemonkmn@users.sourceforge.net>
 * 
 * Distributed under the GNU General Public License (GPL)
 *   - see included file COPYING.txt for details, or visit:
 *     http://www.fsf.org/copyleft/gpl.html
 */
using System;

/// <summary>
/// This class provides global, static data. No instances are created.
/// </summary>
/// <remarks>This class was created to serve as an interface between
/// generated code and project custom code.  It exists so that the
/// Highest level code (including the entry point) can be customized
/// while still allowing the code generator to provide some high level
/// global properties in the Project.resx file.</remarks>
public partial class Project
{
   private static System.Resources.ResourceManager m_res;
   private static GameForm game;

   /// <summary>
   /// Defines the maximum number of players that can be active.
   /// </summary>
   /// <remarks>This determines primarily how many players the user can customize
   /// in the options dialog at runtime. If there is only 1 player, there will be
   /// no dowpdown list to select which player to customize. Note that the actual
   /// number of active players is determined by the rules defined in the project.
   /// <seealso cref="PlanBase.MapPlayerToInputs"/></remarks>
   public static readonly System.Byte MaxPlayers;
   /// <summary>
   /// Defines the maxmimum number of views that can be active.
   /// </summary>
   /// <remarks>This determines which view layouts can be selected with
   /// <see cref="MapBase.ViewLayout"/> and related functions.</remarks>
   public static readonly System.Byte MaxViews;
   /// <summary>
   /// Contains a string to display in the game's about dialog.
   /// </summary>
   /// <remarks>The IDE will automatically merge credits from imported objects
   /// into this string.</remarks>
   public static readonly string GameCredits;

   /// <summary>
   /// Initializes the project's read-only properties from the resource file
   /// provided by the code generator.
   /// </summary>
   static Project()
   {
      Project.m_res = new System.Resources.ResourceManager(typeof(Project));
      MaxPlayers = byte.Parse(m_res.GetString("_MaxPlayers"));
      MaxViews = byte.Parse(m_res.GetString("_MaxViews"));
      GameCredits = m_res.GetString("_GameCredits");
   }

   /// <summary>
   /// Returns the object that contains all the graphic sheet resources for the project
   /// (among other things)
   /// </summary>
   /// <example>A graphic sheet's image can be retrieved from this object using code
   /// like this:
   /// <code>System.Drawing.Bitmap bmpGfx = (System.Drawing.Bitmap)Project.Resources.GetObject("My Sheet")</code>
   /// </example>
   public static System.Resources.ResourceManager Resources
   {
      get
      {
         return m_res;
      }
   }

   /// <summary>
   /// Returns the main window object.
   /// </summary>
   public static GameForm GameWindow
   {
      get
      {
         return Project.game;
      }
   }

   /// <summary>
   /// Represents the main entry point of the program.
   /// </summary>
   /// <remarks>This creates the main window, initializing it with the game's startup
   /// map, overlay map, title, and default window mode (windowed or full screen), then
   /// calls <see cref="GameForm.Run"/> and lets it run until the game quits.</remarks>
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
