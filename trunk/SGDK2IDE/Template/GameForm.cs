using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Form that contains and managed the main display for the game.
/// </summary>
public class GameForm : Form
{
   public Display GameDisplay = null;
   private Microsoft.DirectX.DirectInput.Device keyboard = null;
   private Microsoft.DirectX.DirectInput.KeyboardState m_keyboardState;
   private Microsoft.DirectX.DirectInput.Device[] controllers = null;
   private Microsoft.DirectX.DirectInput.JoystickState[] m_controllerState;
   private System.Collections.BitArray controllerEnabled;
   private System.Collections.BitArray controllerAcquired;
   public MapBase CurrentMap;
   public MapBase OverlayMap;
   public System.Collections.Hashtable LoadedMaps = new System.Collections.Hashtable();
   private int m_fps = 0;
   private int m_frameCount = 0;
   private DateTime m_frameStart;
   private System.Windows.Forms.MainMenu mnuGame;
   private System.Windows.Forms.MenuItem mnuFile;
   private System.Windows.Forms.MenuItem mnuFileExit;
   private System.Windows.Forms.MenuItem mnuTools;
   private System.Windows.Forms.MenuItem mnuToolsOptions;
   public IPlayer[] Players = new IPlayer[Project.MaxPlayers];
   private System.Windows.Forms.MenuItem mnuHelp;
   private System.Windows.Forms.MenuItem mnuHelpAbout;
   public System.IO.StringWriter debugText = new System.IO.StringWriter();

   public GameForm(GameDisplayMode mode, bool windowed, string title, System.Type initMapType, System.Type overlayMapType)
   {
      InitializeComponent();
#if DEBUG
      MessageBox.Show("You are running in debug mode.  Unexpected runtime conditions may cause the game to halt, and if an error occurs you will have the opportunity to debug into it if you have a debugger installed and the project's source code handy", "Debug Mode Active");
#endif
      ClientSize = Display.GetScreenSize(mode);
      GameDisplay = new Display(mode, windowed);
      GameDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
      GameDisplay.Location = new System.Drawing.Point(0, 0);
      GameDisplay.Name = "GameDisplay";
      GameDisplay.Size = Display.GetScreenSize(mode);
      Controls.Add(this.GameDisplay);
      Name = "GameForm";
      Text = title;
      KeyPreview = true;
      FormBorderStyle = FormBorderStyle.FixedSingle;
      CurrentMap = GetMap(initMapType);
      if (overlayMapType != null)
         OverlayMap = GetMap(overlayMapType);
      else
         OverlayMap = null;
      GameDisplay.WindowedChanged += new EventHandler(GameDisplay_WindowedChanged);
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

   public void Run()
   {
      int coopCode;

      System.Collections.ArrayList controllersBuilder = new System.Collections.ArrayList();
      foreach(Microsoft.DirectX.DirectInput.DeviceInstance dev in Microsoft.DirectX.DirectInput.Manager.Devices)
      {
         switch (dev.DeviceType)
         {
            case Microsoft.DirectX.DirectInput.DeviceType.Keyboard:
               if (keyboard == null)
               {
                  keyboard = new Microsoft.DirectX.DirectInput.Device(dev.InstanceGuid);
                  keyboard.SetCooperativeLevel(this, Microsoft.DirectX.DirectInput.CooperativeLevelFlags.Background |
                     Microsoft.DirectX.DirectInput.CooperativeLevelFlags.NonExclusive); 
                  keyboard.SetDataFormat(Microsoft.DirectX.DirectInput.DeviceDataFormat.Keyboard);
                  keyboard.Acquire();
               }
               break;
            case Microsoft.DirectX.DirectInput.DeviceType.Gamepad:
            case Microsoft.DirectX.DirectInput.DeviceType.Joystick:
            {
               Microsoft.DirectX.DirectInput.Device controller =
                  new Microsoft.DirectX.DirectInput.Device(dev.InstanceGuid);
               controller.SetCooperativeLevel(this, Microsoft.DirectX.DirectInput.CooperativeLevelFlags.Background |
                  Microsoft.DirectX.DirectInput.CooperativeLevelFlags.NonExclusive);
               controller.SetDataFormat(Microsoft.DirectX.DirectInput.DeviceDataFormat.Joystick);
               controllersBuilder.Add(controller);
               break;
            }
         }
      }
      if (controllersBuilder.Count > 0)
      {
         controllers = (Microsoft.DirectX.DirectInput.Device[])controllersBuilder.ToArray(typeof(Microsoft.DirectX.DirectInput.Device));
         controllerEnabled = new System.Collections.BitArray(controllers.Length, false);
         controllerAcquired = new System.Collections.BitArray(controllers.Length, false);
         m_controllerState = new Microsoft.DirectX.DirectInput.JoystickState[controllers.Length];
      }
      else
      {
         controllers = null;
         controllerEnabled = controllerAcquired = null;
      }

      // Player 0 always uses keyboard by default
      Players[0] = new KeyboardPlayer(0);

      // Players 1 through (M-N) use keyboard while players (M-N+1) through M use controllers
      // where M is max player number and N is number of controllers.
      for (int playerIdx = 1; playerIdx<Project.MaxPlayers; playerIdx++)
      {
         if (Project.MaxPlayers - playerIdx <= controllers.Length)
            Players[playerIdx] = new ControllerPlayer(playerIdx - (Project.MaxPlayers - controllers.Length));
         else
            Players[playerIdx] = new KeyboardPlayer(playerIdx);
      }

      while(true)
      {
         if ((GameDisplay != null) && (GameDisplay.Device == null))
         {
            // Display is minimized, wait until it is restored
            Application.DoEvents();
            System.Threading.Thread.Sleep(0);
            continue;
         }
         if ((GameDisplay == null) || (GameDisplay.Device.Disposed))
         {
            Close();
            return;
         }
         if (!GameDisplay.Device.CheckCooperativeLevel(out coopCode))
         {
            Microsoft.DirectX.Direct3D.ResultCode coop = (Microsoft.DirectX.Direct3D.ResultCode)System.Enum.Parse(typeof(Microsoft.DirectX.Direct3D.ResultCode), coopCode.ToString());
            if (coop == Microsoft.DirectX.Direct3D.ResultCode.DeviceNotReset)
               GameDisplay.Recreate();
            else
               System.Threading.Thread.Sleep(0);
         }
         else
         {
            GameDisplay.Device.BeginScene();
            GameDisplay.Sprite.Begin(Microsoft.DirectX.Direct3D.SpriteFlags.AlphaBlend);
            CurrentMap.DrawAllViews();
            if (keyboard != null)
               m_keyboardState = keyboard.GetCurrentKeyboardState();
            ReadControllers();
            CurrentMap.ExecuteRules();
            if (OverlayMap != null)
            {
               OverlayMap.DrawAllViews();
               OverlayMap.ExecuteRules();
            }
            OutputDebugInfo();
            GameDisplay.Sprite.End();
            GameDisplay.Device.EndScene();
            GameDisplay.Device.Present();
         }
         Application.DoEvents();
      }
   }

   [System.Diagnostics.Conditional("DEBUG")]
   public void OutputDebugInfo()
   {
      GameDisplay.Sprite.Transform = Microsoft.DirectX.Matrix.Identity;
      GameDisplay.Device.RenderState.ScissorTestEnable = false;
      GameDisplay.D3DFont.DrawText(GameDisplay.Sprite, debugText.ToString(), GameDisplay.DisplayRectangle, Microsoft.DirectX.Direct3D.DrawTextFormat.Left, Color.White);

      debugText.Close();
      debugText = new System.IO.StringWriter();
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

   public void UnloadMap(System.Type mapType)
   {
      LoadedMaps.Remove(mapType);
   }

   private void InitializeComponent()
   {
      this.mnuGame = new System.Windows.Forms.MainMenu();
      this.mnuFile = new System.Windows.Forms.MenuItem();
      this.mnuFileExit = new System.Windows.Forms.MenuItem();
      this.mnuTools = new System.Windows.Forms.MenuItem();
      this.mnuToolsOptions = new System.Windows.Forms.MenuItem();
      this.mnuHelp = new System.Windows.Forms.MenuItem();
      this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
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
      this.Menu = this.mnuGame;
      this.Name = "GameForm";

   }

   public Microsoft.DirectX.DirectInput.KeyboardState KeyboardState
   {
      get
      {
         return m_keyboardState;
      }
   }

   public Microsoft.DirectX.DirectInput.Key[] GetPressedKeys()
   {
      return keyboard.GetPressedKeys();
   }

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
            if (!controllerAcquired[i])
            {
               controllers[i].Acquire();
               controllerAcquired[i] = true;
            }
            m_controllerState[i] = controllers[i].CurrentJoystickState;
         }
         else if (controllerAcquired[i])
         {
            controllers[i].Unacquire();
            controllerAcquired[i] = false;
         }
      }
   }

   public int ControllerCount
   {
      get
      {
         if (controllers == null)
            return 0;
         return controllers.Length;
      }
   }

   public string GetControllerName(int deviceNumber)
   {
      return controllers[deviceNumber].DeviceInformation.InstanceName;
   }

   public Microsoft.DirectX.DirectInput.JoystickState GetControllerState(int deviceNumber)
   {
      return m_controllerState[deviceNumber];
   }

   public System.Collections.BitArray ControllerEnabled
   {
      get
      {
         return controllerEnabled;
      }
   }

   private void GameDisplay_WindowedChanged(object sender, EventArgs e)
   {
      if (GameDisplay.Windowed)
         ClientSize = Display.GetScreenSize(GameDisplay.GameDisplayMode);
   }

   private void mnuToolsOptions_Click(object sender, System.EventArgs e)
   {
      frmControls frm = new frmControls();
      frm.ShowDialog();
   }

   private void mnuHelpAbout_Click(object sender, System.EventArgs e)
   {
      using (frmAbout frm = new frmAbout())
         frm.ShowDialog();
   }
}