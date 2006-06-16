using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Form that contains and managed the main display for the game.
/// </summary>
public class GameForm : Form
{
   public Display GameDisplay = null;
   private Microsoft.DirectX.DirectInput.Device keyboard;
   private Microsoft.DirectX.DirectInput.KeyboardState m_keyboardState;
   public MapBase CurrentMap;
   public System.Collections.Hashtable LoadedMaps = new System.Collections.Hashtable();
   private int m_fps = 0;
   private int m_frameCount = 0;
   private DateTime m_frameStart;
   public System.IO.StringWriter debugText = new System.IO.StringWriter();

   public GameForm(GameDisplayMode mode, bool windowed, string title, System.Type initMapType)
   {
#if DEBUG
      MessageBox.Show("You are running in debug mode.  Unexpected runtime conditions may cause the game to halt, and if an error occurs you will have the opportunity to debug into it if you have a debugger installed and the project's source code handy", "Debug Mode Active");
#endif
      GameDisplay = new Display(mode, windowed);
      GameDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
      GameDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
      GameDisplay.Location = new System.Drawing.Point(0, 0);
      GameDisplay.Name = "GameDisplay";
      GameDisplay.Size = Display.GetScreenSize(mode);
      ClientSize = Display.GetScreenSize(mode);
      Controls.Add(this.GameDisplay);
      Name = "GameForm";
      Text = title;
      KeyPreview = true;
      FormBorderStyle = FormBorderStyle.FixedSingle;
      CurrentMap = GetMap(initMapType, false);
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

      Microsoft.DirectX.DirectInput.DeviceList kbdList = Microsoft.DirectX.DirectInput.Manager.GetDevices(Microsoft.DirectX.DirectInput.DeviceType.Keyboard, Microsoft.DirectX.DirectInput.EnumDevicesFlags.AttachedOnly);
      if (kbdList.MoveNext())
      {
         keyboard = new Microsoft.DirectX.DirectInput.Device(((Microsoft.DirectX.DirectInput.DeviceInstance)kbdList.Current).InstanceGuid);
         keyboard.SetCooperativeLevel(this, Microsoft.DirectX.DirectInput.CooperativeLevelFlags.Background |
            Microsoft.DirectX.DirectInput.CooperativeLevelFlags.NonExclusive); 
         keyboard.SetDataFormat(Microsoft.DirectX.DirectInput.DeviceDataFormat.Keyboard);
         keyboard.Acquire();
      }
      else
         keyboard = null;

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
            CurrentMap.Draw();
            if (keyboard != null)
               m_keyboardState = keyboard.GetCurrentKeyboardState();
            CurrentMap.ExecuteRules();
            GameDisplay.Device.EndScene();
            GameDisplay.Device.Present();
         }
         Application.DoEvents();
      }
   }

   public void OutputDebugInfo()
   {
      Microsoft.DirectX.Direct3D.Surface sfc = GameDisplay.Device.GetRenderTarget(0);
      Graphics g = sfc.GetGraphics();
      g.DrawString(debugText.ToString(), this.Font, System.Drawing.Brushes.White, 0, 0);
      sfc.ReleaseGraphics();
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

   public MapBase GetMap(System.Type mapType, bool forceNew)
   {
      if (forceNew || !LoadedMaps.ContainsKey(mapType))
      {
         MapBase result = (MapBase)mapType.GetConstructor(new System.Type[] {typeof(Display)}).Invoke(new object[] {GameDisplay});
         LoadedMaps[mapType] = result;
         return result;
      }
      else
         return (MapBase)(LoadedMaps[mapType]);
   }

   public Microsoft.DirectX.DirectInput.KeyboardState KeyboardState
   {
      get
      {
         return m_keyboardState;
      }
   }
}