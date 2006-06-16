/*
 * Scrolling Game Development Kit 2.0
 *
 * Copyright © 2000 - 2005 Benjamin Marty <BlueMonkMN@email.com>
 * 
 * Distributed under the GNU General Public License (GPL)
 *   - see included file COPYING.txt for details, or visit:
 *     http://www.fsf.org/copyleft/gpl.html
 */

using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;
using System.Collections;

namespace SGDK2
{
   public enum GameDisplayMode
   {
      m320x240x16,
      m640x480x16,
      m800x600x16,
      m1024x768x16,
      m1280x1024x16,
      m320x240x24,
      m640x480x24,
      m800x600x24,
      m1024x768x24,
      m1280x1024x24
   }

   /// <summary>
   /// Manages the display device on which real-time game graphics are drawn
   /// </summary>
   public class Display : ScrollableControl
   {
      #region Win32 API Constants
      public const int WS_EX_CLIENTEDGE = unchecked((int)0x00000200);
      public const int WS_BORDER = unchecked((int)0x00800000);
      #endregion

      #region Embedded Classes
      private class CoverWindow : Form
      {
         public Display m_LinkedControl;

         public CoverWindow(Display LinkedControl)
         {
            m_LinkedControl = LinkedControl;
            FormBorderStyle = FormBorderStyle.None;
            System.Drawing.Size sz = Display.GetScreenSize(LinkedControl.GameDisplayMode);
            SetBounds(0,0,sz.Width,sz.Height);
            ShowInTaskbar = false;
            Show();
         }

         protected override void OnMouseMove(MouseEventArgs e)
         {
            m_LinkedControl.OnMouseMove(e);
         }
         protected override void OnKeyDown(KeyEventArgs e)
         {
            m_LinkedControl.OnKeyDown(e);
         }
      }

      public class TextureRef : IDisposable
      {
         private string m_Name;
         private Texture m_Texture = null;
         private Display m_Display;
      
         public TextureRef(Display Disp, string Name)
         {
            m_Display = Disp;
            m_Name = Name;
         }

         public string Name
         {
            get
            {
               return m_Name;
            }
         }

         public void Reset()
         {
            m_Texture = null;
         }

         public Texture Texture
         {
            get
            {
               if (m_Texture == null)
                  m_Texture = m_Display.GetTexture(m_Name);
               return m_Texture;
            }
         }

         #region IDisposable Members
         public void Dispose()
         {
            if (m_Texture != null)
            {
               m_Texture.Dispose();
               m_Texture = null;
            }
         }
         #endregion
      }
      #endregion

      #region Fields
      private System.Collections.Specialized.HybridDictionary m_TextureRefs = null;
      private Device m_d3d = null;
      private PresentParameters m_pp;
      private GameDisplayMode m_GameDisplayMode;
      private BorderStyle m_BorderStyle;
      private CoverWindow m_CoverWindow = null;
      private Sprite m_Sprite = null;
      #endregion

      #region Initialization and clean-up
      public Display() : this(GameDisplayMode.m640x480x24, true)
      {
      }

      public Display(GameDisplayMode mode, bool windowed)
      {
         this.SetStyle(ControlStyles.ResizeRedraw, true);
         this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
         this.SetStyle(ControlStyles.UserPaint, true);
         this.SetStyle(ControlStyles.Opaque, true);

         DisplayMode dm = GetActualDisplayMode(mode);
         m_pp = new PresentParameters();
         m_pp.Windowed = windowed;
         m_pp.SwapEffect = SwapEffect.Discard;
         // Allow GetGraphics
         m_pp.PresentFlag = PresentFlag.LockableBackBuffer;
         if (windowed)
         {
            m_pp.BackBufferFormat = Format.Unknown;
            m_pp.BackBufferWidth = m_pp.BackBufferHeight = 0;
         }
         else
         {
            // This can improve performance in some cases, but I have not
            // observed much difference in my testing. (See other two instances of this code)
            //m_pp.BackBufferCount = 2; 
            m_pp.BackBufferFormat = dm.Format;
            m_pp.BackBufferWidth = dm.Width;
            m_pp.BackBufferHeight = dm.Height;
            m_CoverWindow = new CoverWindow(this);
            Recreate();
         }
         m_GameDisplayMode = mode;
      }

      protected override void Dispose(bool disposing)
      {
         if (disposing)
         {
            DisposeAllTextures();
            if (m_Sprite != null)
            {
               m_Sprite.Dispose();
               m_Sprite = null;
            }
            if (m_d3d != null)
            {
               m_d3d.Dispose();
               m_d3d = null;
            }
            if (m_CoverWindow != null)
            {
               m_CoverWindow.m_LinkedControl = null;
               m_CoverWindow.Close();
               m_CoverWindow.Dispose();
               m_CoverWindow = null;
            }
         }
         base.Dispose (disposing);
      }
      #endregion

      #region Overrides
      protected override void OnCreateControl()
      {
         m_pp.DeviceWindow = this;
         m_pp.SwapEffect = SwapEffect.Discard;

         try
         {
            if (m_d3d == null)
            {
               Recreate();
            }
         }
         catch(Exception ex)
         {
            MessageBox.Show(this, "Error while opening the display: " + ex.Message, "Error Creating Device", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }

         base.OnCreateControl ();
      }

      protected override void OnKeyDown(KeyEventArgs e)
      {
         if ((e.KeyCode == Keys.Enter) && e.Alt)
            Windowed = !Windowed;
         base.OnKeyDown (e);
      }

      protected override CreateParams CreateParams
      {
         get
         {
            CreateParams cp = base.CreateParams;
            cp.ExStyle &= ~WS_EX_CLIENTEDGE;
            cp.Style &= ~WS_BORDER;

            if (!m_pp.Windowed)
               return cp;

            switch (m_BorderStyle)
            {
               case BorderStyle.Fixed3D:
                  cp.ExStyle |= WS_EX_CLIENTEDGE;
                  break;
               case BorderStyle.FixedSingle:
                  cp.Style |= WS_BORDER;
                  break;
            }

            return cp;
         }
      }

      protected override void OnResize(EventArgs e)
      {
         if (this.Size.IsEmpty)
         {
            DisposeAllTextures();
            if (m_Sprite != null)
            {
               m_Sprite.Dispose();
               m_Sprite = null;
            }
            if (m_d3d != null)
            {
               m_d3d.Dispose();
               m_d3d = null;
            }
         }
         else if ((m_d3d == null) && Created)
         {
            Recreate();
         }
         else if ((m_d3d != null) && (m_pp != null) && (m_pp.Windowed))
         {
            m_pp.BackBufferHeight = m_pp.BackBufferWidth = 0;
            if (m_Sprite != null)
            {
               m_Sprite.Dispose();
               m_Sprite = null;
            }
            m_d3d.Reset(m_pp);
         }
         base.OnResize (e);
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         if ((m_d3d == null) || (m_d3d.Disposed))
            return;
         int coop;
         if (!m_d3d.CheckCooperativeLevel(out coop))
         {
            Microsoft.DirectX.Direct3D.ResultCode coopStatus = (Microsoft.DirectX.Direct3D.ResultCode)System.Enum.Parse(typeof(Microsoft.DirectX.Direct3D.ResultCode), coop.ToString());
            if (coopStatus == Microsoft.DirectX.Direct3D.ResultCode.DeviceNotReset)
               Recreate();
            else
               return;
         }
         base.OnPaint (e);
      }

      #endregion

      #region Private members
      private Texture GetTexture(string Name)
      {
         return Texture.FromBitmap(m_d3d,
            (System.Drawing.Bitmap)ProjectData.GetGraphicSheetImage(Name, false), 0, Pool.Managed);
      }

      #endregion

      #region Public members
      public BorderStyle BorderStyle
      {
         get
         {
            return m_BorderStyle;
         }
         set
         {
            m_BorderStyle = value;
            UpdateStyles();
         }
      }

      public TextureRef GetTextureRef(string Name)
      {
         if (m_TextureRefs == null)
            m_TextureRefs = new System.Collections.Specialized.HybridDictionary();

         if (m_TextureRefs.Contains(Name))
         {
            WeakReference wr = (WeakReference)m_TextureRefs[Name];
            if (wr.IsAlive)
               return (TextureRef)wr.Target;
         }

         TextureRef tex = new TextureRef(this, Name);
         m_TextureRefs[Name] = new WeakReference(tex);
         return tex;
      }

      public void DisposeAllTextures()
      {
         if (m_TextureRefs != null)
         {
            foreach (DictionaryEntry de in m_TextureRefs)
            {
               if (((WeakReference)de.Value).IsAlive)
                  ((TextureRef)((WeakReference)de.Value).Target).Dispose();
            }
         }
      }

      public Device Device
      {
         get
         {
            return m_d3d;
         }
      }

      public bool Windowed
      {
         set
         {
            if ((DesignMode) && (!value))
               throw new InvalidOperationException("Cannot use full screen in design mode");

            if (value != m_pp.Windowed)
            {
               DisposeAllTextures();
               if (m_Sprite != null)
               {
                  m_Sprite.Dispose();
                  m_Sprite = null;
               }
               if (m_d3d != null)
               {
                  m_d3d.Dispose();
                  m_d3d = null;
               }
            }
            else
               return;

            m_pp.Windowed = value;
            m_pp.SwapEffect = SwapEffect.Discard;

            if (value)
            {
               m_pp.DeviceWindow = this;
               m_pp.BackBufferWidth = m_pp.BackBufferHeight = 0;
               m_pp.BackBufferFormat = Format.Unknown;
               if (m_CoverWindow != null)
               {
                  m_CoverWindow.m_LinkedControl = null;
                  m_CoverWindow.Close();
                  m_CoverWindow.Dispose();
                  m_CoverWindow = null;
               }
               Recreate();
            }
            else
            {
               m_CoverWindow = new CoverWindow(this);
               m_pp.DeviceWindow = m_CoverWindow;
               DisplayMode dm = GetActualDisplayMode(m_GameDisplayMode);
               if (dm.Format == Format.Unknown)
                  throw new ApplicationException("Current display does not support mode " + m_GameDisplayMode.ToString() +".");
               // This can improve performance in some cases, but I have not
               // observed much difference in my testing. (See other two instances of this code)
               //m_pp.BackBufferCount = 2;
               m_pp.BackBufferWidth = dm.Width;
               m_pp.BackBufferHeight = dm.Height;
               m_pp.BackBufferFormat = dm.Format;
               Recreate();
            }
         }
         get
         {
            return m_pp.Windowed;
         }
      }

      /// <summary>
      /// Get the size of a display based on the specified mode
      /// </summary>
      /// <param name="mode">Game display mode used for the display</param>
      /// <returns>Width and height in pixels</returns>
      public static System.Drawing.Size GetScreenSize(GameDisplayMode mode)
      {
         switch(mode)
         {
            case GameDisplayMode.m320x240x16:
            case GameDisplayMode.m320x240x24:
               return new System.Drawing.Size(320,240);
            case GameDisplayMode.m640x480x16:
            case GameDisplayMode.m640x480x24:
               return new System.Drawing.Size(640,480);
            case GameDisplayMode.m800x600x16:
            case GameDisplayMode.m800x600x24:
               return new System.Drawing.Size(800,600);
            case GameDisplayMode.m1024x768x16:
            case GameDisplayMode.m1024x768x24:
               return new System.Drawing.Size(1024,768);
            case GameDisplayMode.m1280x1024x16:
            case GameDisplayMode.m1280x1024x24:
               return new System.Drawing.Size(1280,1024);
         }
         return new System.Drawing.Size(0,0);
      }

      /// <summary>
      /// Gets the actual full screen display mode that will be used for the specified
      /// game display mode when the game is in full screen mode.
      /// </summary>
      /// <param name="mode">Game requested display mode</param>
      /// <returns>Supported DirectX display mode that conforms to the requested resolution and color depth.</returns>
      public static DisplayMode GetActualDisplayMode(GameDisplayMode mode)
      {
         foreach(DisplayMode dm in Manager.Adapters.Default.SupportedDisplayModes)
         {
            System.Drawing.Size s = GetScreenSize(mode);

            if ((dm.Width == s.Width) && (dm.Height == s.Height))
            {
               switch (mode)
               {
                  case GameDisplayMode.m320x240x16:
                  case GameDisplayMode.m640x480x16:
                  case GameDisplayMode.m800x600x16:
                  case GameDisplayMode.m1024x768x16:
                  case GameDisplayMode.m1280x1024x16:
                  switch (dm.Format)
                  {
                     case Format.A4R4G4B4:
                     case Format.A1R5G5B5:
                     case Format.R5G6B5:
                     case Format.X1R5G5B5:
                     case Format.X4R4G4B4:
                        return dm;
                  }
                     break;
                  default:
                  switch (dm.Format)
                  {
                     case Format.A2B10G10R10:
                     case Format.A2R10G10B10:
                     case Format.A8B8G8R8:
                     case Format.A8R8G8B8:
                     case Format.R8G8B8:
                     case Format.X8B8G8R8:
                     case Format.X8R8G8B8:
                        return dm;
                  }
                     break;
               }
            }
         }
         return new DisplayMode();
      }

      public GameDisplayMode GameDisplayMode
      {
         get
         {
            return m_GameDisplayMode;
         }
         set
         {
            m_GameDisplayMode = value;
            ClientSize = GetScreenSize(value);
            if (!m_pp.Windowed)
            {
               DisplayMode dm = GetActualDisplayMode(m_GameDisplayMode);
               if (dm.Format == Format.Unknown)
                  throw new ApplicationException("Current display does not support mode " + m_GameDisplayMode.ToString() +".");
               // This can improve performance in some cases, but I have not
               // observed much difference in my testing. (See other two instances of this code)
               //m_pp.BackBufferCount = 2;
               m_pp.BackBufferWidth = dm.Width;
               m_pp.BackBufferHeight = dm.Height;
               m_pp.BackBufferFormat = dm.Format;
               if (m_Sprite != null)
               {
                  m_Sprite.Dispose();
                  m_Sprite = null;
               }
               m_d3d.Reset(m_pp);
            }
         }
      }

      public void Recreate()
      {
         DisposeAllTextures();
         if (m_Sprite != null)
         {
            m_Sprite.Dispose();
            m_Sprite = null;
         }
         if (m_d3d != null)
            m_d3d.Dispose();
         if (Windowed || (m_CoverWindow == null))
            m_d3d = new Device(Manager.Adapters.Default.Adapter, DeviceType.Hardware, this, CreateFlags.HardwareVertexProcessing, m_pp);
         else
            m_d3d = new Device(Manager.Adapters.Default.Adapter, DeviceType.Hardware, m_CoverWindow, CreateFlags.HardwareVertexProcessing, m_pp);
         m_d3d.DeviceReset += new EventHandler(d3d_DeviceReset);
      }

      public Sprite Sprite
      {
         get
         {
            if ((m_Sprite == null) && (m_d3d != null))
               m_Sprite = new Sprite(m_d3d);
            return m_Sprite;
         }
      }
      #endregion

      #region Event Handlers
      private void d3d_DeviceReset(object sender, EventArgs e)
      {
         DisposeAllTextures();
      }
      #endregion
   }
}
