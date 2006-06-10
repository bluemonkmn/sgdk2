using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Form that contains and managed the main display for the game.
/// </summary>
public class GameForm : Form
{
   public Display GameDisplay = null;
   public Point MousePosition = Point.Empty;
   private System.Collections.Hashtable KeyboardState = new System.Collections.Hashtable();
   public MapBase CurrentMap;
   public System.Collections.Hashtable LoadedMaps = new System.Collections.Hashtable();

   public GameForm(GameDisplayMode mode, bool windowed, string title, System.Type initMapType)
   {
      GameDisplay = new Display(mode, windowed);
      GameDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
      GameDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
      GameDisplay.Location = new System.Drawing.Point(0, 0);
      GameDisplay.Name = "GameDisplay";
      GameDisplay.Size = Display.GetScreenSize(mode);
      GameDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GameDisplay_MouseMove);
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

   private void GameDisplay_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
   {
      if (GameDisplay.Device.Disposed)
      {
         Close();
         return;
      }
      MousePosition = new Point(e.X, e.Y);
   }

   public void Run()
   {
      while(true)
      {
         if ((GameDisplay == null) || (GameDisplay.Device.Disposed))
         {
            Close();
            return;
         }
         GameDisplay.Device.BeginScene();
         CurrentMap.Draw();
         CurrentMap.ExecuteRules();
         GameDisplay.Device.EndScene();
         GameDisplay.Device.Present();
         Application.DoEvents();
      }
   }

   private void Debug()
   {
      /*
         GameDisplay.Device.BeginScene();
         theMap.Draw();
         Microsoft.DirectX.Direct3D.Surface sfc = GameDisplay.Device.GetRenderTarget(0);
         Graphics g = sfc.GetGraphics();
         g.DrawString(debugString.ToString(), this.Font, System.Drawing.Brushes.White, 0, 0);
         sfc.ReleaseGraphics();
         debugString.Close();
         debugString = new System.IO.StringWriter();
         GameDisplay.Device.EndScene();
         GameDisplay.Device.Present();
      */
   }

   protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
   {
      GameDisplay.Dispose();
      GameDisplay = null;
      base.OnClosing(e);
   }

   protected override void OnKeyDown(KeyEventArgs e)
   {
      base.OnKeyDown(e);
      KeyboardState[e.KeyCode] = true;
   }

   protected override void OnKeyUp(KeyEventArgs e)
   {
      base.OnKeyUp(e);
      KeyboardState[e.KeyCode] = false;
   }

   public bool GetKeyState(System.Windows.Forms.Keys key)
   {
      object result = KeyboardState[key];
      if (result == null)
         return false;
      return (bool)result;
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
}