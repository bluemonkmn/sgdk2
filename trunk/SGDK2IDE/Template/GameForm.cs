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
   private byte currentPlayers = 1;
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

      Players[0] = new KeyboardPlayer(
         Microsoft.DirectX.DirectInput.Key.UpArrow,
         Microsoft.DirectX.DirectInput.Key.LeftArrow,
         Microsoft.DirectX.DirectInput.Key.RightArrow,
         Microsoft.DirectX.DirectInput.Key.DownArrow,
         Microsoft.DirectX.DirectInput.Key.RightControl,
         Microsoft.DirectX.DirectInput.Key.Space,
         Microsoft.DirectX.DirectInput.Key.Return,
         Microsoft.DirectX.DirectInput.Key.RightShift);

      if (controllers.Length > 0)
      {
         for (int playerIdx = 1; playerIdx<Project.MaxPlayers; playerIdx++)
         {
            Players[playerIdx] = new ControllerPlayer((playerIdx-1) % controllers.Length);
         }
      }
      else
      {
         if (Project.MaxPlayers > 1)
            Players[1] = new KeyboardPlayer(
               Microsoft.DirectX.DirectInput.Key.W,
               Microsoft.DirectX.DirectInput.Key.A,
               Microsoft.DirectX.DirectInput.Key.D,
               Microsoft.DirectX.DirectInput.Key.S,
               Microsoft.DirectX.DirectInput.Key.LeftAlt,
               Microsoft.DirectX.DirectInput.Key.LeftShift,
               Microsoft.DirectX.DirectInput.Key.LeftControl,
               Microsoft.DirectX.DirectInput.Key.E);
         if (Project.MaxPlayers > 2)
            Players[2] = new KeyboardPlayer(
               Microsoft.DirectX.DirectInput.Key.NumPad8,
               Microsoft.DirectX.DirectInput.Key.NumPad4,
               Microsoft.DirectX.DirectInput.Key.NumPad6,
               Microsoft.DirectX.DirectInput.Key.NumPad2,
               Microsoft.DirectX.DirectInput.Key.NumPad5,
               Microsoft.DirectX.DirectInput.Key.NumPad0,
               Microsoft.DirectX.DirectInput.Key.NumPadEnter,
               Microsoft.DirectX.DirectInput.Key.NumPad7);
         if (Project.MaxPlayers > 3)
            Players[3] = new KeyboardPlayer(
               Microsoft.DirectX.DirectInput.Key.I,
               Microsoft.DirectX.DirectInput.Key.J,
               Microsoft.DirectX.DirectInput.Key.L,
               Microsoft.DirectX.DirectInput.Key.K,
               Microsoft.DirectX.DirectInput.Key.U,
               Microsoft.DirectX.DirectInput.Key.O,
               Microsoft.DirectX.DirectInput.Key.M,
               Microsoft.DirectX.DirectInput.Key.Comma);
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
            CurrentMap.Draw();
            if (keyboard != null)
               m_keyboardState = keyboard.GetCurrentKeyboardState();
            ReadControllers();
            CurrentMap.ExecuteRules();
            if (OverlayMap != null)
            {
               OverlayMap.Draw();
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
      // 
      // mnuGame
      // 
      this.mnuGame.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                             this.mnuFile,
                                                                             this.mnuTools});
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

   public byte CurrentPlayers
   {
      get
      {
         return currentPlayers;
      }
      set
      {
         if ((value >= 1) && (value <= 4))
         {
            currentPlayers = value;
            ReadControllers();
         }
         else
            System.Diagnostics.Debug.Fail("Bad CurrentPlayers value ignored");
      }
   }

   public void RefreshControllers()
   {
      foreach(IPlayer plr in Players)
      {
         if (plr is ControllerPlayer)
            controllerEnabled[((ControllerPlayer)plr).deviceNumber] = true;
         ReadControllers();
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
}