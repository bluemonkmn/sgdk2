/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */
using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// UI that contains and manages the main display for the game.
/// </summary>
public class GameForm : Form
{
   /// <summary>
   /// Hardware-backed display object embedded in the main window.
   /// </summary>
   public Display GameDisplay = null;
   private KeyboardState m_keyboardState = null;
   private Joystick[] controllers = null;
   private System.Collections.BitArray controllerEnabled;
   /// <summary>
   /// Refers to the currently active primary map that is being drawn on the display
   /// </summary>
   public MapBase CurrentMap;
   /// <summary>
   /// Refers to the currently active overlay map being drawn in front of the primary map
   /// </summary>
   /// <value>This is a null reference if no overlay is active</value>
   public MapBase OverlayMap;
   /// <summary>
   /// Contains a collection of all currently loaded maps.
   /// </summary>
   /// <remarks>The key of this collection is the type of the map class. The value is an
   /// instance of the map.
   /// <seealso cref="UnloadMap"/>
   /// <seealso cref="GeneralRules.SwitchToMap"/></remarks>
   public System.Collections.Hashtable LoadedMaps = new System.Collections.Hashtable();
   private int m_fps = 0;
   private int m_frameCount = 0;
   private DateTime m_frameStart;
   /// <summary>
   /// Used to write debug text that will be displayed for the current frame.
   /// </summary>
   public System.IO.StringWriter debugText = new System.IO.StringWriter();
   private bool m_quit = false;
   /// <summary>
   /// Provides access to the input currently coming from the players' input devices.
   /// </summary>
   /// <remarks>Each of these objects can refer to a <see cref="KeyboardPlayer"/>
   /// or a <see cref="ControllerPlayer"/>, or you can create your own player input.</remarks>
   public IPlayer[] Players = new IPlayer[Project.MaxPlayers];
   bool isFullScreen = false;
   private string title;

   #region Events
   /// <summary>
   /// Defines a mechanism by which simple notifications without any data can be triggered.
   /// </summary>
   public delegate void SimpleNotification();
   /// <summary>
   /// Event fires every frame even when the game is not advancing.
   /// </summary>
   /// <remarks>The game may not be moving because the window may be minimized, but
   /// this event will still be raised. This might be useful for monitoring sounds that
   /// might continue to play while the window is minimized.</remarks>
   public event SimpleNotification OnFrameStart;
   /// <summary>
   /// Event fires every frame that the game is advancing right before the scene is started.
   /// </summary>
   /// <remarks>This event does not occur if the game is minimized/paused.</remarks>
   public event SimpleNotification OnBeforeBeginScene;
   /// <summary>
   /// Event fires every frame that the game is advancing right before executing rules.
   /// </summary>
   /// <remarks>This event does not occur if the game is minimized/paused.</remarks>
   public event SimpleNotification OnBeforeExecuteRules;
   /// <summary>
   /// Event fires every frame that the game is advancing while the scene is being
   /// generated, right before the overlay map is drawn.
   /// </summary>
   /// <remarks>This event does not occur if the game is minimized/paused.</remarks>
   public event SimpleNotification OnBeforeDrawOverlay;
   /// <summary>
   /// Event fires every frame that the game is advancing after the overlay map is drawn.
   /// </summary>
   /// <remarks>This event does not occur if the game is minimized/paused.</remarks>
   public event SimpleNotification OnAfterDrawOverlay;
   #endregion


   #region Windows Forms Components
   private System.Windows.Forms.MainMenu mnuGame;
   private System.Windows.Forms.MenuItem mnuFile;
   private System.Windows.Forms.MenuItem mnuFileExit;
   private System.Windows.Forms.MenuItem mnuTools;
   private System.Windows.Forms.MenuItem mnuToolsOptions;
   private System.Windows.Forms.MenuItem mnuHelp;
   private System.ComponentModel.IContainer components;
   private System.Windows.Forms.MenuItem mnuHelpAbout;
   #endregion

   /// <summary>
   /// Constructs the main form for containing the game display.
   /// </summary>
   /// <param name="mode">Defines the size of the form in windowed mode and the resolution and
   /// color depth of the display in full screen mode.</param>
   /// <param name="windowed">Determines whether the display is initially windowed or full screen.</param>
   /// <param name="title">Supplies a title for the window when the game is in windowed mode.</param>
   /// <param name="initMapType">Defines the map that is initially active when the game starts.</param>
   /// <param name="overlayMapType">Defines the map that in initially set as the
   /// <seealso cref="OverlayMap"/> or null if there is no overlay initially.</param>
   public GameForm(GameDisplayMode mode, bool windowed, string title, System.Type initMapType, System.Type overlayMapType)
   {
      InitializeComponent();
#if DEBUG
      MessageBox.Show("You are running in debug mode.  Unexpected runtime conditions may cause the game to halt, and if an error occurs you will have the opportunity to debug into it if you have a debugger installed and the project's source code handy", "Debug Mode Active");
#endif
      AutoScale = false;
      ClientSize = Display.GetScreenSize(mode);
      GameDisplay = new Display(mode, windowed);
      GameDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
      GameDisplay.Location = new System.Drawing.Point(0, 0);
      GameDisplay.Name = "GameDisplay";
      GameDisplay.Size = Display.GetScreenSize(mode);
      Controls.Add(this.GameDisplay);
      Name = "GameForm";
      Text = this.title = title;
      KeyPreview = true;
      FormBorderStyle = FormBorderStyle.FixedSingle;
      CurrentMap = GetMap(initMapType);
      if (overlayMapType != null)
         OverlayMap = GetMap(overlayMapType);
      else
         OverlayMap = null;
      if (!windowed)
         FullScreen = true;
   }

   /// <summary>
   /// Clean up any resources being used.
   /// </summary>
   protected override void Dispose( bool disposing )
   {
      if( disposing )
      {
         if (GameDisplay != null)
         {
            GameDisplay.Dispose();
            GameDisplay = null;
         }
      }
      base.Dispose( disposing );
   }

   /// <summary>
   /// Main game loop initializes input devices and runs the game.
   /// </summary>
   /// <remarks>The general sequence of steps performed in this loop are:
   /// <list type="number">
   /// <item><description>Trigger <see cref="OnFrameStart"/>.</description></item>
   /// <item><description>Check if game has been closed/quit, and exit if necessary.</description></item>
   /// <item><description>If game is inactive draw the screen as it was when it
   /// was last active, and skip the rest of the loop.</description></item>
   /// <item><description>Trigger <see cref="OnBeforeBeginScene"/>.</description></item>
   /// <item><description>Draw the main map.</description></item>
   /// <item><description>Read players' input devices.</description></item>
   /// <item><description>Trigger <see cref="OnBeforeExecuteRules"/>.</description></item>
   /// <item><description>Execute the main map's rules.</description></item>
   /// <item><description>Trigger <see cref="OnBeforeDrawOverlay"/>.</description></item>
   /// <item><description>If an overlay map is active, draw it and execute its rules.</description></item>
   /// <item><description>Draw the current debug output if debug mode is active, and clear the debug text buffer.</description></item>
   /// <item><description>Trigger <see cref="OnAfterDrawOverlay"/>.</description></item>
   /// </list>
   /// </remarks>
   public void Run()
   {
      int coopCode;
      int controllerCount = Joystick.GetDeviceCount();
      if (controllerCount > 0)
      {
         controllers = new Joystick[controllerCount];
         controllerEnabled = new System.Collections.BitArray(controllers.Length, false);
      }
      else
      {
         controllers = null;
         controllerEnabled = null;
      }

      // Player 0 always uses keyboard by default
      Players[0] = new KeyboardPlayer(0);

      // Players 1 through (M-N) use keyboard while players (M-N+1) through M use controllers
      // where M is max player number and N is number of controllers.
      for (int playerIdx = 1; playerIdx<Project.MaxPlayers; playerIdx++)
      {
         if ((controllers != null) && (Project.MaxPlayers - playerIdx <= controllers.Length))
            Players[playerIdx] = new ControllerPlayer(playerIdx - (Project.MaxPlayers - controllers.Length));
         else
            Players[playerIdx] = new KeyboardPlayer(playerIdx);
      }

      m_keyboardState = new KeyboardState();

      while(true)
      {
         if (OnFrameStart != null)
            OnFrameStart();
         if ((GameDisplay == null) || GameDisplay.IsDisposed || m_quit)
         {
            Close();
            return;
         }
         bool isActive;
         isActive = (System.Windows.Forms.Form.ActiveForm == this);
         if (!isActive)
         {
            // Display is minimized or inactive, wait until it is restored
            Application.DoEvents();
            if (GameDisplay != null)
               GameDisplay.SwapBuffers();
            System.Threading.Thread.Sleep(0);
            continue;
         }
         if (OnBeforeBeginScene != null)
            OnBeforeBeginScene();
         CurrentMap.DrawAllViews();
         ReadControllers();
         if (OnBeforeExecuteRules != null)
            OnBeforeExecuteRules();
         CurrentMap.ExecuteRules();
         if (OnBeforeDrawOverlay != null)
            OnBeforeDrawOverlay();
         GeneralRules.DrawMessages();
         if (OverlayMap != null)
         {
            OverlayMap.DrawAllViews();
            OverlayMap.ExecuteRules();
         }
         OutputDebugInfo();
         if (OnAfterDrawOverlay != null)
            OnAfterDrawOverlay();
         GameDisplay.Flush();
         GameDisplay.SwapBuffers();
         Application.DoEvents();
      }
   }

   /// <summary>
   /// Draw the text currently in the <see cref="debugText"/> buffer and clear the buffer.
   /// </summary>
   /// <remarks>This will only execute in debug mode. It requires that the "CoolFont" graphic
   /// sheet be embedded in the project.</remarks>
   [System.Diagnostics.Conditional("DEBUG")]
   public void OutputDebugInfo()
   {
      GameDisplay.ScissorOff();
      GameDisplay.SetColor(Color.White);
      GameDisplay.DrawText(debugText.ToString(), 0, 0);

      debugText.GetStringBuilder().Length = 0;
      debugText.WriteLine("fps=" + m_fps.ToString());
      m_frameCount++;
      if (DateTime.Now.Subtract(m_frameStart).TotalSeconds >= 1f)
      {
         m_fps = m_frameCount;
         m_frameCount=0;
         m_frameStart = DateTime.Now;
      }
   }

   protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
   {
      GameDisplay.Dispose();
      GameDisplay = null;
      base.OnClosing(e);
   }

   /// <summary>
   /// Retrieves an instance of the specified map.
   /// </summary>
   /// <param name="mapType">Specifies which map to retrieve by its type</param>
   /// <returns>A newly initialized map if the map was not loaded, or the existing
   /// map if it was already loaded.</returns>
   /// <remarks>Loaded maps are stored in <see cref="LoadedMaps"/>.
   /// <seealso cref="GeneralRules.UnloadMap"/>
   /// <seealso cref="GeneralRules.SwitchToMap"/></remarks>
   public MapBase GetMap(System.Type mapType)
   {
      if (!LoadedMaps.ContainsKey(mapType))
      {
         MapBase result = (MapBase)mapType.GetConstructor(new System.Type[] {typeof(Display)}).Invoke(new object[] {GameDisplay});
         LoadedMaps[mapType] = result;
         return result;
      }
      else
         return (MapBase)(LoadedMaps[mapType]);
   }

   /// <summary>
   /// Unloads the currently loaded instance of the specified map type if it is loaded.
   /// </summary>
   /// <param name="mapType">Specifies which map to unload.</param>
   /// <remarks>This is called by <see cref="GeneralRules.UnloadMap"/>.</remarks>
   public void UnloadMap(System.Type mapType)
   {
      LoadedMaps.Remove(mapType);
   }

   /// <summary>
   /// Unload all maps that are not the current primary map or overlay map.
   /// </summary>
   /// <remarks>This is called by <see cref="GeneralRules.UnloadBackgroundMaps"/>.
   /// <seealso cref="UnloadMap"/></remarks>
   public void UnloadBackgroundMaps()
   {
      System.Collections.ArrayList toRemove = new System.Collections.ArrayList();
      foreach(System.Collections.DictionaryEntry de in LoadedMaps)
      {
         if ((de.Value != CurrentMap) && (de.Value != OverlayMap))
            toRemove.Add(de.Key);
      }
      foreach(System.Type removeKey in toRemove)
      {
         LoadedMaps.Remove(removeKey);
      }
   }

   private void InitializeComponent()
   {
      this.components = new System.ComponentModel.Container();
      this.mnuGame = new System.Windows.Forms.MainMenu(this.components);
      this.mnuFile = new System.Windows.Forms.MenuItem();
      this.mnuFileExit = new System.Windows.Forms.MenuItem();
      this.mnuTools = new System.Windows.Forms.MenuItem();
      this.mnuToolsOptions = new System.Windows.Forms.MenuItem();
      this.mnuHelp = new System.Windows.Forms.MenuItem();
      this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
      this.SuspendLayout();
      // 
      // mnuGame
      // 
      this.mnuGame.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFile,
            this.mnuTools,
            this.mnuHelp});
      // 
      // mnuFile
      // 
      this.mnuFile.Index = 0;
      this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFileExit});
      this.mnuFile.Text = "&File";
      // 
      // mnuFileExit
      // 
      this.mnuFileExit.Index = 0;
      this.mnuFileExit.Shortcut = System.Windows.Forms.Shortcut.AltF4;
      this.mnuFileExit.Text = "E&xit";
      this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
      // 
      // mnuTools
      // 
      this.mnuTools.Index = 1;
      this.mnuTools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuToolsOptions});
      this.mnuTools.Text = "&Tools";
      // 
      // mnuToolsOptions
      // 
      this.mnuToolsOptions.Index = 0;
      this.mnuToolsOptions.Text = "&Options";
      this.mnuToolsOptions.Click += new System.EventHandler(this.mnuToolsOptions_Click);
      // 
      // mnuHelp
      // 
      this.mnuHelp.Index = 2;
      this.mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuHelpAbout});
      this.mnuHelp.Text = "&Help";
      // 
      // mnuHelpAbout
      // 
      this.mnuHelpAbout.Index = 0;
      this.mnuHelpAbout.Text = "&About...";
      this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
      // 
      // GameForm
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(292, 273);
      this.MaximizeBox = false;
      this.Menu = this.mnuGame;
      this.Name = "GameForm";
      this.ResumeLayout(false);

   }

   /// <summary>
   /// Represents the current state of the keyboard.
   /// </summary>
   public KeyboardState KeyboardState
   {
      get
      {
         return m_keyboardState;
      }
   }

   /// <summary>
   /// Reads the state of all relevant game controller devices into the respective
   /// objects in <see cref="Players"/>.
   /// </summary>
   public void ReadControllers()
   {
      if (controllerEnabled == null)
         return;

      foreach(IPlayer plr in Players)
      {
         if (plr is ControllerPlayer)
            controllerEnabled[((ControllerPlayer)plr).deviceNumber] = true;
      }

      for (int i=0; i<controllerEnabled.Count; i++)
      {
         if (controllerEnabled[i])
         {
            if (controllers[i] == null)
               controllers[i] = new Joystick(i);
            controllers[i].Read();
         }
         else if (controllers[i] != null)
            controllers[i] = null;
      }
   }

   /// <summary>
   /// Return the number of available controllers connected to the system
   /// </summary>
   public int ControllerCount
   {
      get
      {
         if (controllers == null)
            return 0;
         return controllers.Length;
      }
   }

   /// <summary>
   /// Return the name of a specific game controller as displayed in the options window.
   /// </summary>
   /// <param name="deviceNumber">Zero-based index of the game controller</param>
   /// <returns>String containing the display name for the device</returns>
   public string GetControllerName(int deviceNumber)
   {
      if (controllers[deviceNumber] == null)
         controllers[deviceNumber] = new Joystick(deviceNumber);
      return controllers[deviceNumber].Name;
   }

   /// <summary>
   /// Reads the current state of a game controller
   /// </summary>
   /// <param name="deviceNumber">Zero-based index of the game controller</param>
   public Joystick GetControllerState(int deviceNumber)
   {
      return controllers[deviceNumber];
   }

   /// <summary>
   /// Returns true if the controller is currently enabled for input.
   /// </summary>
   /// <remarks>A controller is enabled when a player is using it.</remarks>
   public System.Collections.BitArray ControllerEnabled
   {
      get
      {
         return controllerEnabled;
      }
   }

   private void mnuFileExit_Click(object sender, System.EventArgs e)
   {
      Quit();
   }
   
   private void mnuToolsOptions_Click(object sender, System.EventArgs e)
   {
      frmControls frm = new frmControls();
      frm.ShowDialog();
      frm.Dispose();
   }

   private void mnuHelpAbout_Click(object sender, System.EventArgs e)
   {
      using (frmAbout frm = new frmAbout())
         frm.ShowDialog();
   }

   /// <summary>
   /// Sets an indicator that causes the game to quit at the beginning of the
   /// next game loop.
   /// </summary>
   /// <remarks>This value is checked during <see cref="Run"/>.</remarks>
   public void Quit()
   {
      m_quit = true;
   }

   /// <summary>
   /// This function is called by the SGDK2 generated code when a top-level
   /// exception occurs.
   /// </summary>
   /// <param name="ex">Refers to the exception that was received at the top level</param>
   public static void HandleException(System.Exception ex)
   {
      if (Project.GameWindow != null)
         Project.GameWindow.Close();
      MessageBox.Show("A fatal error occurred initializing or running the game:\r\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
   }

   protected override void OnKeyDown(KeyEventArgs e)
   {
      if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Alt)
      {
         e.Handled = true;
         e.SuppressKeyPress = true;
         FullScreen = !FullScreen;
      }
   }

   /// <summary>
   /// Determines whether the display will occupy the full screen rather than being contained in a window.
   /// </summary>
   public bool FullScreen
   {
      get
      {
         return isFullScreen;
      }
      set
      {
         if (value != isFullScreen)
         {
            isFullScreen = value;
            if (isFullScreen)
            {
               Text = String.Empty;
               FormBorderStyle = FormBorderStyle.None;
               ControlBox = false;
               MinimizeBox = false;
               Menu = null;
               GameDisplay.SwitchToResolution();
               WindowState = FormWindowState.Maximized;
            }
            else
            {
               Display.RestoreResolution();
               WindowState = FormWindowState.Normal;
               Text = title;
               FormBorderStyle = FormBorderStyle.FixedSingle;
               ControlBox = true;
               MinimizeBox = true;
               Menu = mnuGame;
               ClientSize = Display.GetScreenSize(GameDisplay.GameDisplayMode);
            }
         }
      }
   }
}