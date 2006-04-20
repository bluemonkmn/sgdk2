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
      class CoverWindow : Form
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
      #endregion

      #region Fields
      private System.Collections.Specialized.HybridDictionary m_GraphicSheets = null;
      private Device m_d3d;
      private PresentParameters m_pp;
      private GameDisplayMode m_GameDisplayMode;
      private BorderStyle m_BorderStyle;
      private CoverWindow m_CoverWindow = null;
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
         // Allow GetGraphics
         m_pp.PresentFlag = PresentFlag.LockableBackBuffer;
         if (windowed)
         {
            m_pp.BackBufferFormat = Format.Unknown;
            m_pp.BackBufferWidth = m_pp.BackBufferHeight = 0;
         }
         else
         {
            m_pp.BackBufferFormat = dm.Format;
            m_pp.BackBufferWidth = dm.Width;
            m_pp.BackBufferHeight = dm.Height;
         }
         m_GameDisplayMode = mode;
      }

      protected override void Dispose(bool disposing)
      {
         if (disposing)
         {
            DisposeAllTextures();
            if (m_d3d != null)
            {
               m_d3d.Dispose();
               m_d3d = null;
            }
            if (m_CoverWindow != null)
            {
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
            m_d3d = new Device(Manager.Adapters.Default.Adapter, DeviceType.Hardware, this, CreateFlags.HardwareVertexProcessing, m_pp);
         }
         catch(Exception ex)
         {
            MessageBox.Show(this, "Error while opening the display: " + ex.Message, "Error Creating Device", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }

         base.OnCreateControl ();
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint(e);
         //m_d3d.Present();
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
         if ((m_d3d != null) && (m_pp != null) && (m_pp.Windowed))
         {
            m_pp.BackBufferHeight = m_pp.BackBufferWidth = 0;
            m_d3d.Reset(m_pp);
         }
         base.OnResize (e);
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
      public Texture GetTexture(string Name, bool bForceReload)
      {
         if (m_GraphicSheets == null)
            m_GraphicSheets = new System.Collections.Specialized.HybridDictionary();

         if (m_GraphicSheets.Contains(Name))
         {
            WeakReference wr = (WeakReference)m_GraphicSheets[Name];
            if(bForceReload)
            {
               // If reload is forced, may have to dispose of old textrure
               if (wr.IsAlive)
                  ((Texture)wr.Target).Dispose();
            }
            else
            {
               if ((wr.IsAlive) && !((Texture)wr.Target).Disposed)
                  return (Texture)wr.Target;
            }
         }

         Texture tex = Texture.FromBitmap(m_d3d,
            ProjectData.GetGraphicSheetImage(Name, bForceReload), 0, Pool.Managed);
         m_GraphicSheets[Name] = new WeakReference(tex);
         return tex;
      }

      public void DisposeAllTextures()
      {
         if (m_GraphicSheets != null)
         {
            foreach (DictionaryEntry de in m_GraphicSheets)
            {
               if (((WeakReference)de.Value).IsAlive)
                  ((Texture)((WeakReference)de.Value).Target).Dispose();
            }
            m_GraphicSheets = null;
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

            m_pp.Windowed = value;

            if (value)
            {
               m_pp.DeviceWindow = this;
               m_pp.BackBufferWidth = m_pp.BackBufferHeight = 0;
               m_pp.BackBufferFormat = Format.Unknown;
               if (m_CoverWindow != null)
               {
                  m_CoverWindow.Close();
                  m_CoverWindow.Dispose();
                  m_CoverWindow = null;
               }
            }
            else
            {
               m_CoverWindow = new CoverWindow(this);
               m_pp.DeviceWindow = m_CoverWindow;
               DisplayMode dm = GetActualDisplayMode(m_GameDisplayMode);
               if (dm.Format == Format.Unknown)
                  throw new ApplicationException("Current display does not support mode " + m_GameDisplayMode.ToString() +".");
               m_pp.BackBufferWidth = dm.Width;
               m_pp.BackBufferHeight = dm.Height;
               m_pp.BackBufferFormat = dm.Format;
            }
            if (m_d3d != null)
               m_d3d.Reset(m_pp);
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
               m_pp.BackBufferWidth = dm.Width;
               m_pp.BackBufferHeight = dm.Height;
               m_pp.BackBufferFormat = dm.Format;
               m_d3d.Reset(m_pp);
            }
         }
      }

      #endregion   
   }
}
