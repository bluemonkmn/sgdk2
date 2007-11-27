/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */
using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;
using System.Collections;

/// <summary>
/// Specifies a size and color depth for a display.
/// </summary>
/// <remarks>Color depth only applies when the display is in full screen mode.</remarks>
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
[Serializable()]
public class Display : ScrollableControl, System.Runtime.Serialization.ISerializable
{
   #region Win32 API Constants
   public const int WS_EX_CLIENTEDGE = unchecked((int)0x00000200);
   public const int WS_BORDER = unchecked((int)0x00800000);
   #endregion

   #region Events
   public event EventHandler WindowedChanged;
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

   /// <summary>
   /// Manages a reference to a graphic sheet ("texture") in the hardware.
   /// </summary>
   /// <remarks>This class tracks an instance of a Direct3D texture and provides
   /// a layer of indirection, allowing a frameset to refer to a texture (via this
   /// object) while the texture (and even the whole display) are destroyed and
   /// re-created, without losing track of which graphics it is associated with.
   /// </remarks>
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
   private Font m_Font = null;
   private Line m_Line = null;
   private Surface targetSurface = null;
   private string fontName = null;
   private int fontSize = 0;

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

      m_pp = new PresentParameters();
      m_pp.Windowed = windowed;
      m_pp.SwapEffect = SwapEffect.Copy; // Allows ScissorTestEnable to work in full screen
      // Allow GetGraphics
      m_pp.PresentFlag = PresentFlag.LockableBackBuffer;
      MakeValidPresentParameters(mode, m_pp);
      if (!windowed)
      {
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
         if (m_Font != null)
         {
            m_Font.Dispose();
            m_Font = null;
         }
         if (m_Line != null)
         {
            m_Line.Dispose();
            m_Line = null;
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
      m_pp.SwapEffect = SwapEffect.Copy; // Allows ScissorTestEnable to work in full screen

      try
      {
         if (m_d3d == null)
         {
            Recreate();
         }
      }
      catch(Exception ex)
      {
         MessageBox.Show(this, "Error creating display device: " + ex.ToString(), "Error Creating Device", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
         if (m_Font != null)
         {
            m_Font.Dispose();
            m_Font = null;
         }
         if (m_Line != null)
         {
            m_Line.Dispose();
            m_Line = null;
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
         if (m_Font != null)
         {
            m_Font.Dispose();
            m_Font = null;
         }
         if (m_Line != null)
         {
            m_Line.Dispose();
            m_Line = null;
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
         (System.Drawing.Bitmap)Project.Resources.GetObject(Name), 0, Pool.Managed);
   }

   #endregion

   #region Public members
   /// <summary>
   /// Defines how the edges of the display appear when embedded in a window.
   /// </summary>
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

   /// <summary>
   /// Retrieve a reference to a hardware-supported graphic sheet ("texture") given its name
   /// </summary>
   /// <param name="Name">The name of a graphic sheet defined by the game</param>
   /// <returns>Object that manages graphics on the hardware for this graphic sheet</returns>
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

   /// <summary>
   /// Release resources used by all hardware copies of graphic sheets
   /// </summary>
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

   /// <summary>
   /// Returns the Direct3D device supporting this display object
   /// </summary>
   public Device Device
   {
      get
      {
         return m_d3d;
      }
   }

   /// <summary>
   /// Gets or sets whether the display is running in windowed mode versus full screen mode.
   /// </summary>
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
            if (m_Font != null)
            {
               m_Font.Dispose();
               m_Font = null;
            }
            if (m_Line != null)
            {
               m_Line.Dispose();
               m_Line = null;
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
         m_pp.SwapEffect = SwapEffect.Copy; // Allows ScissorTestEnable to work in full screen

         if (value)
         {
            m_pp.DeviceWindow = this;
            MakeValidPresentParameters(GameDisplayMode, m_pp);
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
            MakeValidPresentParameters(GameDisplayMode, m_pp);
            Recreate();
         }
         if (WindowedChanged != null)
            WindowedChanged(this, null);
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
   /// Completes the presentation parameter structure by filling out a back buffer
   /// width, height, and format.
   /// </summary>
   /// <param name="mode">Game requested display mode</param>
   /// <param name="pp">Object to finish populating</param>
   public static void MakeValidPresentParameters(GameDisplayMode mode, PresentParameters pp)
   {
      if (pp.Windowed)
      {
         pp.BackBufferFormat = Format.Unknown;
         pp.BackBufferWidth = pp.BackBufferHeight = 0;
         pp.FullScreenRefreshRateInHz = 0;

         if (!Manager.CheckDeviceType(Manager.Adapters.Default.Adapter, DeviceType.Hardware, Manager.Adapters.Default.CurrentDisplayMode.Format, pp.BackBufferFormat, pp.Windowed))
            throw new ApplicationException("No hardware support for windowed mode on default display adapter");
      }
      else
      {
         foreach(DisplayMode dm in Manager.Adapters.Default.SupportedDisplayModes)
         {
            System.Drawing.Size s = GetScreenSize(mode);
            if ((dm.Width != s.Width) || (dm.Height != s.Height))
               continue;

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
                     // These are OK 16-bit formats. Break out of the switch and proceed
                     break;
                  default:
                     // Not a 16-bit format, continue to the next mode
                     continue;
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
                     // These are OK 32-bit formats. Break out of the switch and proceed
                     break;
                  default:
                     // Not a 32-bit format, continue to the next mode
                     continue;
               }
                  break;
            }

            if (Manager.CheckDeviceType(Manager.Adapters.Default.Adapter, DeviceType.Hardware, dm.Format, dm.Format, pp.Windowed))
            {
               // This can improve performance in some cases, but I have not
               // observed much difference in my testing.
               //m_pp.BackBufferCount = 2; 
               pp.BackBufferWidth = dm.Width;
               pp.BackBufferHeight = dm.Height;
               pp.BackBufferFormat = dm.Format;
               if (dm.RefreshRate > pp.FullScreenRefreshRateInHz)
                  pp.FullScreenRefreshRateInHz = dm.RefreshRate;
            }
         }
         if (pp.FullScreenRefreshRateInHz == 0)
            throw new ApplicationException("Current display does not support mode " + mode.ToString() +".");
      }

      string errMsg = ValidateAdapter(Manager.Adapters.Default);
      if (errMsg != null)
         throw new ApplicationException("Default display adapter is inadequate: " + errMsg);
   }

   /// <summary>
   /// Gets or sets the size/resolution and color depth of the display
   /// </summary>
   /// <remarks>If the display is windowed, this only affects the size. The color depth
   /// will match that of the user's desktop. In full screen mode this affects the
   /// resolution and color depth of the display.</remarks>
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
            MakeValidPresentParameters(m_GameDisplayMode, m_pp);
            if (m_Sprite != null)
            {
               m_Sprite.Dispose();
               m_Sprite = null;
            }
            if (m_Font != null)
            {
               m_Font.Dispose();
               m_Font = null;
            }
            if (m_Line != null)
            {
               m_Line.Dispose();
               m_Line = null;
            }
            m_d3d.Reset(m_pp);
         }
      }
   }

   /// <summary>
   /// Frees and re-creates all resources managed by the display
   /// </summary>
   public void Recreate()
   {
      DisposeAllTextures();
      if (m_Sprite != null)
      {
         m_Sprite.Dispose();
         m_Sprite = null;
      }
      if (m_Font != null)
      {
         m_Font.Dispose();
         m_Font = null;
      }
      if (m_Line != null)
      {
         m_Line.Dispose();
         m_Line = null;
      }
      if (m_d3d != null)
         m_d3d.Dispose();
      if (Windowed || (m_CoverWindow == null))
         m_d3d = new Device(Manager.Adapters.Default.Adapter, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, m_pp);
      else
         m_d3d = new Device(Manager.Adapters.Default.Adapter, DeviceType.Hardware, m_CoverWindow, CreateFlags.SoftwareVertexProcessing, m_pp);
      m_d3d.DeviceReset += new EventHandler(d3d_DeviceReset);
   }
   
   private static string ValidateAdapter(AdapterInformation adapter)
   {
      Caps caps = Manager.GetDeviceCaps(adapter.Adapter, DeviceType.Hardware);
      if (!caps.PrimitiveMiscCaps.SupportsBlendOperation)
         return "Inadequate hardware support for alpha blending";
      if (!caps.TextureCaps.SupportsAlpha)
         return "Hardware does not support textures with alpha";
      if (!caps.RasterCaps.SupportsScissorTest)
         return "No hardware support for clipping";
      if (!caps.TextureOperationCaps.SupportsModulate)
         return "No hardware support for color modulation";
      return null;
   }
   
   /// <summary>
   /// Returns the Direct3D Sprite object implementing this display's ability to draw graphics.
   /// </summary>
   public Sprite Sprite
   {
      get
      {
         if ((m_Sprite == null) && (m_d3d != null))
            m_Sprite = new Sprite(m_d3d);
         return m_Sprite;
      }
   }

   /// <summary>
   /// Returns the Direct3D Font object implementing this display's ability to draw text.
   /// </summary>
   public Font D3DFont
   {
      get
      {
         if ((m_Font == null) && (m_d3d != null))
         {
            if (fontName == null)
            {
               m_Font = new Microsoft.DirectX.Direct3D.Font(m_d3d, Font);
            }
            else
            {
               Microsoft.DirectX.Direct3D.FontDescription desc = new Microsoft.DirectX.Direct3D.FontDescription();
               desc.FaceName = fontName;
               desc.Height = fontSize;
               m_Font = new Microsoft.DirectX.Direct3D.Font(m_d3d, desc);
            }
         }
         return m_Font;
      }
   }

   /// <summary>
   /// Change the font used for drawing text on the display.
   /// </summary>
   /// <param name="font">Specifies a font to use</param>
   public void SetFont(string name, int size)
   {
      if (m_Font != null)
      {
         m_Font.Dispose();
         m_Font = null;
      }
      fontName = name;
      fontSize = size;      
   }
   
   /// <summary>
   /// Returns the Direct3D Line object implementing this displays ability to perform simple line
   /// drawing operations.
   /// </summary>
   public Line D3DLine
   {
      get
      {
         if ((m_Line == null) && (m_d3d != null))
            m_Line = new Microsoft.DirectX.Direct3D.Line(m_d3d);
         return m_Line;
      }
   }

   /// <summary>
   /// Return DirectX rendering target surface
   /// </summary>
   /// <remarks>Apparent memory leak in managed D3D requires minimizing number of
   /// references to GetRenderTarget or program hangs on close.</remarks>
   public Surface TargetSurface
   {
      get
      {
         if (targetSurface == null)
            targetSurface = m_d3d.GetRenderTarget(0);
         return targetSurface;
      }
   }
   #endregion

   #region Event Handlers
   private void d3d_DeviceReset(object sender, EventArgs e)
   {
      DisposeAllTextures();
   }
   #endregion

   #region ISerializable Members

   public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
   {
      info.SetType(typeof(DisplayRef));
   }

   #endregion
}

/// <summary>
/// Provides serialization "services" for the <see cref="Display"/> object, preventing
/// attempts to save or load data for the display object when the game is saved/loaded.
/// </summary>
[Serializable()]
public class DisplayRef : System.Runtime.Serialization.IObjectReference, System.Runtime.Serialization.ISerializable
{

   private DisplayRef(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
   {
   }

   public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
   {
      throw new System.NotImplementedException("Unexpected serialization call");
   }

   #region IObjectReference Members
   public object GetRealObject(System.Runtime.Serialization.StreamingContext context)
   {
      return Project.GameWindow.GameDisplay;
   }
   #endregion
}