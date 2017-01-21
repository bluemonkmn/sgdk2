/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */

using System;
using System.Collections;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

/// <summary>
/// Specifies a size, color depth and scaling factor for a display.
/// </summary>
/// <remarks>Color depth only applies when the display is in full screen mode.</remarks>
public enum GameDisplayMode
{
   /// <summary>
   /// 320x240-pixel display with 16-bit color
   /// </summary>
   m320x240x16,
   /// <summary>
   /// 640x480-pixel display with 16-bit color
   /// </summary>
   m640x480x16,
   /// <summary>
   /// 800x600-pixel display with 16-bit color
   /// </summary>
   m800x600x16,
   /// <summary>
   /// 1024x768-pixel display with 16-bit color
   /// </summary>
   m1024x768x16,
   /// <summary>
   /// 1280x1024-pixel display with 16-bit color
   /// </summary>
   m1280x1024x16,
   /// <summary>
   /// 1920x1080-pixel display with 16-bit color
   /// </summary>
   m1920x1080x16,
   /// <summary>
   /// 320x240-pixel display with 24-bit color
   /// </summary>
   m320x240x24,
   /// <summary>
   /// 640x480-pixel display with 24-bit color
   /// </summary>
   m640x480x24,
   /// <summary>
   /// 800x600-pixel display with 24-bit color
   /// </summary>
   m800x600x24,
   /// <summary>
   /// 1024x768-pixel display with 24-bit color
   /// </summary>
   m1024x768x24,
   /// <summary>
   /// 1280x1024-pixel display with 24-bit color
   /// </summary>
   m1280x1024x24,
   /// <summary>
   /// 1920x1080-pixel display with 24-bit color
   /// </summary>
   m1920x1080x24,
   /// <summary>
   /// 160x120-pixel view with 16-bit color, scaled up to 320x240-pixel display
   /// </summary>
   m160x120x16_2x,
   /// <summary>
   /// 320x240-pixel view with 16-bit color, scaled up to 640x480-pixel display
   /// </summary>
   m320x240x16_2x,
   /// <summary>
   /// 400x300-pixel view with 16-bit color, scaled up to 800x600-pixel display
   /// </summary>
   m400x300x16_2x,
   /// <summary>
   /// 512x384-pixel view with 16-bit color, scaled up to 1024x768-pixel display
   /// </summary>
   m512x384x16_2x,
   /// <summary>
   /// 640x512-pixel view with 16-bit color, scaled up to 1280x1024-pixel display
   /// </summary>
   m640x512x16_2x,
   /// <summary>
   /// 960x540-pixel view with 16-bit color, scaled up to 1920x1080-pixel display
   /// </summary>
   m960x540x16_2x,
   /// <summary>
   /// 160x120-pixel view with 24-bit color, scaled up to 320x240-pixel display
   /// </summary>
   m160x120x24_2x,
   /// <summary>
   /// 320x240-pixel view with 24-bit color, scaled up to 640x480-pixel display
   /// </summary>
   m320x240x24_2x,
   /// <summary>
   /// 400x300-pixel view with 24-bit color, scaled up to 800x600-pixel display
   /// </summary>
   m400x300x24_2x,
   /// <summary>
   /// 512x384-pixel view with 24-bit color, scaled up to 1024x768-pixel display
   /// </summary>
   m512x384x24_2x,
   /// <summary>
   /// 640x512-pixel view with 24-bit color, scaled up to 1280x1024-pixel display
   /// </summary>
   m640x512x24_2x,
   /// <summary>
   /// 960x540-pixel view with 24-bit color, scaled up to 1920x1080-pixel display
   /// </summary>
   m960x540x24_2x,
}

/// <summary>
/// Manages the display device on which real-time game graphics are drawn
/// </summary>
[Serializable()]
public partial class Display : GLControl, IDisposable, System.Runtime.Serialization.ISerializable
{
   #region Embedded Classes
   /// <summary>
   /// Object used to refer to a texture (graphic sheet) managed by a particular <see cref="Display" />.
   /// </summary>
   public partial class TextureRef : IDisposable
   {
      private string m_Name;
      private int m_Texture = 0;
      private int m_NormalMap = 0;
      private Display m_Display;
      private int m_Width = 0;
      private int m_Height = 0;
      private bool? m_HasNormalMap = null;

      public TextureRef(Display Disp, string Name)
      {
         m_Display = Disp;
         m_Name = Name;
      }

      /// <summary>
      /// Name of a graphic sheet
      /// </summary>
      public string Name
      {
         get
         {
            return m_Name;
         }
      }

      public void Use(ShaderProgram sp, bool enableLighting)
      {
         if (m_Texture == 0)
            m_Texture = m_Display.GetTexture(m_Name, TextureUnit.Texture0);
         if (m_Texture != 0)
         {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(texTarget, m_Texture);
            int texLoc = sp.GetUniformLocation("tex");
            GL.Uniform1(texLoc, 0);
            CheckError();
         }

         if ((m_NormalMap == 0) && enableLighting)
            m_NormalMap = m_Display.GetTexture(m_Name + " nm", TextureUnit.Texture1);
         if ((m_NormalMap != 0) && enableLighting)
         {
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(texTarget, m_NormalMap);
            int texLoc = sp.GetUniformLocation("norm");
            GL.Uniform1(texLoc, 1);
            CheckError();
         }
      }

      public bool HasNormalMap
      {
         get
         {
            if (!m_HasNormalMap.HasValue)
               m_HasNormalMap = Project.Resources.GetObject(m_Name + " nm") != null;
            return m_HasNormalMap.Value;
         }
      }

      /// <summary>
      /// Returns OpenGL handle to the texture referenced by this object
      /// </summary>
      public int Texture
      {
         get
         {
            if (m_Texture == 0)
            {
               m_Texture = m_Display.GetTexture(m_Name, TextureUnit.Texture0);
            }
            return m_Texture;
         }
      }

      public int Width
      {
         get
         {
            if (m_Width <= 0)
               m_Width = m_Display.m_TextureSizes[m_Name].Width;
            return m_Width;
         }
      }

      public int Height
      {
         get
         {
            if (m_Height <= 0)
               m_Height = m_Display.m_TextureSizes[m_Name].Height;
            return m_Height;
         }
      }

      #region IDisposable Members
      /// <summary>
      /// Releases all resources for the associated texture.
      /// </summary>
      public void Dispose()
      {
         if (m_Texture != 0)
         {
            if (m_Display.Context != null)
            {
               if (!m_Display.Context.IsCurrent)
                  m_Display.MakeCurrent();
               GL.DeleteTextures(1, ref m_Texture);
               if (m_NormalMap != 0)
                  GL.DeleteTextures(1, ref m_NormalMap);
               m_NormalMap = 0;
            }
            m_Texture = 0;
         }
      }
      #endregion
   }

   private enum DisplayOperation
   {
      None = 0,
      DrawFrames,
      DrawLines,
      DrawPoints
   }
   #endregion

   #region Fields
   private System.Collections.Generic.Dictionary<string, WeakReference> m_TextureRefs = null;
   private System.Collections.Generic.Dictionary<string, System.Drawing.Size> m_TextureSizes = null;
   private GameDisplayMode m_GameDisplayMode;
   private DisplayOperation m_currentOp;
   private TextureRef m_currentTexture = null;
   private readonly int m_scaleFactor;
   private const TextureTarget texTarget = TextureTarget.Texture2D;
   private const EnableCap texCap = EnableCap.Texture2D;
   private static TextureRef m_DefaultFont = null;
   private static byte[] shadedStipple = new byte[] {
      0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA,
      0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA,
      0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA,
      0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA,
      0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA,
      0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA,
      0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA,
      0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA,
      0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA,
      0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA,
      0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA,
      0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA,
      0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA,
      0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA,
      0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA,
      0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA};
   private System.Drawing.Point endPoint = System.Drawing.Point.Empty;
   private int frameBuffer = -1;
   private int texUnscaledOutput = -1;
   /// <summary>
   /// Determines if requirements have already been checked or need to be
   /// (re-)checked during the next call to <see cref="DrawFrame"/>.
   /// </summary>
   /// <value>If true, requirements have already been checked.
   /// If false, requirements will be checked next time DrawFrame executes.</value>
   public bool isInitialized = false;
   VertexBuffer<TileVertex> vertexBuffer;
   VertexBuffer<ColoredVertex> solidVertexBuffer;
   Matrix4 projectionMatrix;
   private LightSources[] lights;
   private VertexArray<TileVertex> normalVertexArray;
   private VertexArray<TileVertex> flatVertexArray;
   private VertexArray<ColoredVertex> solidVertexArray;
   private VertexArray<TileVertex> nolightVertexArray;
   private Color4 currentColor;
   public int currentView;
   public bool enableLighting;
   #endregion

   #region Initialization and clean-up
   public Display()
      : this(GameDisplayMode.m640x480x24, true)
   {
   }

   public Display(GameDisplayMode mode, bool windowed)
      : base(CreateGraphicsMode(mode))
   {
      m_GameDisplayMode = mode;
      m_scaleFactor = (mode >= GameDisplayMode.m160x120x16_2x ? 2 : 1);
   }

   private void Initialize()
   {
      if (isInitialized)
         return;

      // Check Requirements
      GL.Finish();
      string[] versionParts = GL.GetString(StringName.Version).Split(new char[] { '.' }, 3);
      int majorVer = int.Parse(versionParts[0]);
      int minorVer = int.Parse(versionParts[1]);
      if (majorVer < 3)
      {
         string errString = "OpenGL version 3.0 is required";
         try
         {
            errString += "; your version is: " + GL.GetString(StringName.Version);
         }
         catch
         {
         }
         if (System.Windows.Forms.DialogResult.Cancel == System.Windows.Forms.MessageBox.Show(this, errString + "\r\nTry updating your video drivers.", "Requirement Check Warning", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Exclamation, System.Windows.Forms.MessageBoxDefaultButton.Button2))
            throw new ApplicationException(errString);
      }

      // Create vertex buffer
      vertexBuffer = new VertexBuffer<TileVertex>(TileVertex.Size);
      solidVertexBuffer = new VertexBuffer<ColoredVertex>(ColoredVertex.Size);

      // Set up VertexArray objects
      VertexAttribute vaposition = new VertexAttribute("vPosition", 2, VertexAttribPointerType.Float, TileVertex.Size, 0);
      VertexAttribute vasrc = new VertexAttribute("vSrc", 2, VertexAttribPointerType.Float, TileVertex.Size, 2 * 4);
      VertexAttribute vacolor = new VertexAttribute("vColor", 4, VertexAttribPointerType.Float, TileVertex.Size, 4 * 4);
      normalVertexArray = new VertexArray<TileVertex>(vertexBuffer, ShaderProgram.NormalMappedShader, vaposition, vasrc, vacolor);
      flatVertexArray = new VertexArray<TileVertex>(vertexBuffer, ShaderProgram.FlatShader, vaposition, vasrc, vacolor);
      nolightVertexArray = new VertexArray<TileVertex>(vertexBuffer, ShaderProgram.NoLightShader, vaposition, vasrc, vacolor);
      VertexAttribute vaSolidPosition = new VertexAttribute("vPosition", 2, VertexAttribPointerType.Float, ColoredVertex.Size, 0);
      VertexAttribute vaSolidColor = new VertexAttribute("vColor", 4, VertexAttribPointerType.Float, ColoredVertex.Size, 2 * 4);
      solidVertexArray = new VertexArray<ColoredVertex>(solidVertexBuffer, ShaderProgram.SolidShader, vaSolidPosition, vaSolidColor);

      // One set of light sources for each possible view
      if (Project.MaxViews == 4)
         lights = new LightSources[] { new LightSources(), new LightSources(), new LightSources(), new LightSources() };
      else if (Project.MaxViews == 2)
         lights = new LightSources[] { new LightSources(), new LightSources() };
      else
         lights = new LightSources[] { new LightSources() };

      // Align to display
      System.Drawing.Size nativeSize = GetScreenSize(m_GameDisplayMode, false);
      projectionMatrix = Matrix4.CreateOrthographicOffCenter(0, nativeSize.Width, nativeSize.Height, 0, .1f, 10f);

      if (m_scaleFactor > 1)
         Buffer(false, true);

      isInitialized = true;
   }
   #endregion

   #region Overrides
   protected override bool IsInputKey(System.Windows.Forms.Keys keyData)
   {
      return true;
   }
   protected override void OnResize(EventArgs e)
   {
      base.OnResize(e);
      if (GraphicsContext.CurrentContext == null)
         return;
      System.Drawing.Size nativeSize = GetScreenSize(m_GameDisplayMode, false);
      GL.Viewport(0, 0, nativeSize.Width, nativeSize.Height);
      projectionMatrix = Matrix4.CreateOrthographicOffCenter(0, nativeSize.Width, nativeSize.Height, 0, .1f, 10f);
   }

   protected override void WndProc(ref System.Windows.Forms.Message m)
   {
      switch (m.Msg)
      {
         case 0x2: // WM_DESTROY
            CheckError();
            Dispose();
            break;
      }
      base.WndProc(ref m);
   }

   protected override void Dispose(bool disposing)
   {
      // If controls are disposed in the wrong order, GL somehow gets into an error
      // state during non-application code.
      CheckError();
      if (disposing)
      {
         if (vertexBuffer != null)
         {
            vertexBuffer.Dispose();
            vertexBuffer = null;
         }
         if (solidVertexBuffer != null)
         {
            solidVertexBuffer.Dispose();
            solidVertexBuffer = null;
         }
         if (normalVertexArray != null)
         {
            normalVertexArray.Dispose();
            normalVertexArray = null;
         }
         if (flatVertexArray != null)
         {
            flatVertexArray.Dispose();
            flatVertexArray = null;
         }
         if (solidVertexArray != null)
         {
            solidVertexArray.Dispose();
            solidVertexArray = null;
         }
         if (nolightVertexArray != null)
         {
            nolightVertexArray.Dispose();
            nolightVertexArray = null;
         }
         ShaderProgram.DisposeShaderPrograms();
         OpenTK.DisplayDevice.Default.RestoreResolution();
         DisposeAllTextures();
      }
      if (frameBuffer != -1)
      {
         GL.DeleteFramebuffer(frameBuffer);
         frameBuffer = -1;
      }
      if (texUnscaledOutput != -1)
      {
         GL.DeleteTexture(texUnscaledOutput);
         texUnscaledOutput = -1;
      }
      base.Dispose(disposing);
   }
   #endregion

   #region Private members
   private int GetTexture(string Name, TextureUnit unit)
   {
      int texture;
      System.Drawing.Bitmap bmpTexture = (System.Drawing.Bitmap)Project.Resources.GetObject(Name);
      if (bmpTexture == null)
         return 0;
      GL.GenTextures(1, out texture);
      GL.ActiveTexture(unit);
      GL.BindTexture(texTarget, texture);
      CheckError();
      GL.TexParameter(texTarget, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
      GL.TexParameter(texTarget, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
      CheckError();
      GL.TexParameter(texTarget, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
      GL.TexParameter(texTarget, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
      CheckError();

      int useWidth = OpenTK.MathHelper.NextPowerOfTwo(bmpTexture.Width - 1);
      int useHeight = OpenTK.MathHelper.NextPowerOfTwo(bmpTexture.Height - 1);

      bool useSubTexture = (useWidth != bmpTexture.Width) || (useHeight != bmpTexture.Height);

      int texSize;
      GL.GetInteger(GetPName.MaxTextureSize, out texSize);
      CheckError();
      if ((texSize < useWidth) ||
          (texSize < useHeight))
         throw new System.ApplicationException("Texture " + Name + " is size " + useWidth + "x" + useHeight +
            " (after rounding up to power of 2), but the current OpenGL video drivers only support textures up to " + texSize.ToString());

      if (m_TextureSizes == null)
         m_TextureSizes = new System.Collections.Generic.Dictionary<string, System.Drawing.Size>();
      if (!m_TextureSizes.ContainsKey(Name))
         m_TextureSizes[Name] = new System.Drawing.Size(useWidth, useHeight);

      System.Drawing.Imaging.BitmapData bits = bmpTexture.LockBits(new System.Drawing.Rectangle(0, 0, bmpTexture.Width, bmpTexture.Height),
         System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      try
      {
         if (useSubTexture)
         {
            GL.TexImage2D(texTarget, 0, PixelInternalFormat.Rgba8, useWidth, useHeight, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexSubImage2D(texTarget, 0, 0, 0, bmpTexture.Width, bmpTexture.Height, PixelFormat.Bgra, PixelType.UnsignedByte, bits.Scan0);
            CheckError();
         }
         else
         {
            GL.TexImage2D(texTarget, 0, PixelInternalFormat.Rgba8, useWidth, useHeight, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bits.Scan0);
            CheckError();
         }
      }
      finally
      {
         bmpTexture.UnlockBits(bits);
      }
      return texture;
   }
   #endregion

   #region Public members
   /// <summary>
   /// Retrieve a reference to a hardware-supported graphic sheet ("texture") given its name
   /// </summary>
   /// <param name="Name">The name of a graphic sheet defined by the game</param>
   /// <returns>Object that manages graphics on the hardware for this graphic sheet</returns>
   public TextureRef GetTextureRef(string Name)
   {
      if (m_TextureRefs == null)
         m_TextureRefs = new System.Collections.Generic.Dictionary<string, WeakReference>();

      if (m_TextureRefs.ContainsKey(Name))
      {
         WeakReference wr = m_TextureRefs[Name];
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
         foreach (System.Collections.Generic.KeyValuePair<string, WeakReference> de in m_TextureRefs)
         {
            if (de.Value.IsAlive)
               ((TextureRef)(de.Value).Target).Dispose();
         }
      }
   }

   /// <summary>
   /// Get the size of a display based on the specified mode
   /// </summary>
   /// <param name="mode">Game display mode used for the display</param>
   /// <param name="scaled">true to return the scaled size (multiplied by scaleFactor), false to return the native size</param>
   /// <returns>Width and height in pixels</returns>
   public static System.Drawing.Size GetScreenSize(GameDisplayMode mode, bool scaled)
   {
      int scale = scaled ? 2 : 1;
      switch (mode)
      {
         case GameDisplayMode.m320x240x16:
         case GameDisplayMode.m320x240x24:
            return new System.Drawing.Size(320, 240);
         case GameDisplayMode.m640x480x16:
         case GameDisplayMode.m640x480x24:
            return new System.Drawing.Size(640, 480);
         case GameDisplayMode.m800x600x16:
         case GameDisplayMode.m800x600x24:
            return new System.Drawing.Size(800, 600);
         case GameDisplayMode.m1024x768x16:
         case GameDisplayMode.m1024x768x24:
            return new System.Drawing.Size(1024, 768);
         case GameDisplayMode.m1280x1024x16:
         case GameDisplayMode.m1280x1024x24:
            return new System.Drawing.Size(1280, 1024);
         case GameDisplayMode.m1920x1080x16:
         case GameDisplayMode.m1920x1080x24:
            return new System.Drawing.Size(1920, 1080);
         case GameDisplayMode.m160x120x16_2x:
         case GameDisplayMode.m160x120x24_2x:
            return new System.Drawing.Size(160 * scale, 120 * scale);
         case GameDisplayMode.m320x240x16_2x:
         case GameDisplayMode.m320x240x24_2x:
            return new System.Drawing.Size(320 * scale, 240 * scale);
         case GameDisplayMode.m400x300x16_2x:
         case GameDisplayMode.m400x300x24_2x:
            return new System.Drawing.Size(400 * scale, 300 * scale);
         case GameDisplayMode.m512x384x16_2x:
         case GameDisplayMode.m512x384x24_2x:
            return new System.Drawing.Size(512 * scale, 384 * scale);
         case GameDisplayMode.m640x512x16_2x:
         case GameDisplayMode.m640x512x24_2x:
            return new System.Drawing.Size(640 * scale, 512 * scale);
         case GameDisplayMode.m960x540x16_2x:
         case GameDisplayMode.m960x540x24_2x:
            return new System.Drawing.Size(960 * scale, 540 * scale);
      }
      return new System.Drawing.Size(0, 0);
   }

   /// <summary>
   /// Return the bit depth of the specified mode
   /// </summary>
   /// <param name="mode">GameDisplayMode value whose depth will be returned</param>
   /// <returns>Integer value of 16 or 24</returns>
   public static int GetModeDepth(GameDisplayMode mode)
   {
      switch (mode)
      {
         case GameDisplayMode.m320x240x16:
         case GameDisplayMode.m640x480x16:
         case GameDisplayMode.m800x600x16:
         case GameDisplayMode.m1024x768x16:
         case GameDisplayMode.m1280x1024x16:
         case GameDisplayMode.m1920x1080x16:
         case GameDisplayMode.m160x120x16_2x:
         case GameDisplayMode.m320x240x16_2x:
         case GameDisplayMode.m400x300x16_2x:
         case GameDisplayMode.m512x384x16_2x:
         case GameDisplayMode.m640x512x16_2x:
         case GameDisplayMode.m960x540x16_2x:
            return 16;
         default:
            return 24;
      }
   }

   /// <summary>
   /// Returns a GraphicsMode structure corresponding to the requested GameDisplayMode
   /// </summary>
   /// <param name="mode">Game requested display mode</param>
   public static GraphicsMode CreateGraphicsMode(GameDisplayMode mode)
   {
      return new GraphicsMode(new ColorFormat(GetModeDepth(mode)));
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
         System.Drawing.Size nativeSize = GetScreenSize(m_GameDisplayMode, false);
         projectionMatrix = Matrix4.CreateOrthographicOffCenter(0, nativeSize.Width, nativeSize.Height, 0, .1f, 10f);
      }
   }

   /// <summary>
   /// Restore the resolution of the display after a call to <see cref="SwitchToResolution" />
   /// </summary>
   public void SwitchToResolution()
   {
      System.Drawing.Size size = GetScreenSize(m_GameDisplayMode, true);
      int depth = GetModeDepth(m_GameDisplayMode);
      OpenTK.DisplayResolution best = null;
      foreach (OpenTK.DisplayResolution dr in OpenTK.DisplayDevice.Default.AvailableResolutions)
      {
         if ((dr.Width == size.Width) && (dr.Height == size.Height))
         {
            if ((dr.BitsPerPixel == 32 ? 24 : 32) == depth)
            {
               if ((best == null) || (best.RefreshRate < dr.RefreshRate))
                  best = dr;
            }
            else
            {
               if ((best == null) || (best.BitsPerPixel != depth) &&
                   (best.BitsPerPixel < dr.BitsPerPixel))
                  best = dr;
            }
         }
      }
      if (best != null)
      {
         OpenTK.DisplayDevice.Default.ChangeResolution(best);
         return;
      }
      throw new ApplicationException("Cannot match display mode " + m_GameDisplayMode.ToString());
   }

   /// <summary>
   /// Restores the original (desktop) resolution, for example after switching to full
   /// screen mode in another resolution.
   /// </summary>
   public static void RestoreResolution()
   {
      OpenTK.DisplayDevice.Default.RestoreResolution();
   }

   /// <summary>
   /// Draw a rectangle from a texture on the display
   /// </summary>
   /// <param name="texture">Texture from which graphics are copied</param>
   /// <param name="sourceRect">Specifies the source area of the copy</param>
   /// <param name="corners">Specifies the corners (counter-clockwise) of the output quadrilateral</param>
   /// <param name="offsetX">Specifies the horizontal (rightward) offset of the corners</param>
   /// <param name="offsetY">Specifies the vertical (downward) offset of the corners</param>
   public void DrawFrame(TextureRef texture, System.Drawing.Rectangle sourceRect, System.Drawing.PointF[] corners, int offsetX, int offsetY)
   {
      if ((m_currentOp != DisplayOperation.DrawFrames) ||
          (m_currentTexture != texture))
      {
         Initialize();
         Flush();

         GL.Enable(EnableCap.Blend);
         GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
         GL.Disable(EnableCap.PolygonSmooth);
         GL.Enable(texCap);
         GL.Disable(EnableCap.DepthTest);
         GL.Disable(EnableCap.Lighting);
         GL.Disable(EnableCap.Dither);

         CheckError();
         if (enableLighting)
         {
            if (texture.HasNormalMap)
            {
               ShaderProgram.NormalMappedShader.Use(projectionMatrix);
               texture.Use(ShaderProgram.NormalMappedShader, enableLighting);
               normalVertexArray.Bind();
               lights[currentView].UseProgram(ShaderProgram.NormalMappedShader, "lights");
               lights[currentView].Set();
            }
            else
            {
               ShaderProgram.FlatShader.Use(projectionMatrix);
               texture.Use(ShaderProgram.FlatShader, enableLighting);
               flatVertexArray.Bind();
               lights[currentView].UseProgram(ShaderProgram.FlatShader, "lights");
               lights[currentView].Set();
            }
         }
         else
         {
            ShaderProgram.NoLightShader.Use(projectionMatrix);
            texture.Use(ShaderProgram.NoLightShader, enableLighting);
            nolightVertexArray.Bind();
         }
         m_currentOp = DisplayOperation.DrawFrames;
         m_currentTexture = texture;
      }

      vertexBuffer.AddVertex(new TileVertex(corners[0].X + offsetX, corners[0].Y + offsetY, sourceRect.X, sourceRect.Y, currentColor));
      vertexBuffer.AddVertex(new TileVertex(corners[1].X + offsetX, corners[1].Y + offsetY, sourceRect.X, sourceRect.Bottom, currentColor));
      vertexBuffer.AddVertex(new TileVertex(corners[2].X + offsetX, corners[2].Y + offsetY, sourceRect.Right, sourceRect.Bottom, currentColor));
      vertexBuffer.AddVertex(new TileVertex(corners[3].X + offsetX, corners[3].Y + offsetY, sourceRect.Right, sourceRect.Y, currentColor));
   }

   /// <summary>
   /// Finishes any pending drawing operation on the display.
   /// </summary>
   public void Flush()
   {
      switch (m_currentOp)
      {
         case DisplayOperation.DrawFrames:
            vertexBuffer.Bind();
            vertexBuffer.BufferData();
            vertexBuffer.Draw(PrimitiveType.Quads);
            break;
         case DisplayOperation.DrawLines:
            solidVertexBuffer.Bind();
            solidVertexBuffer.BufferData();
            solidVertexBuffer.Draw(PrimitiveType.LineStrip);
            break;
         case DisplayOperation.DrawPoints:
            solidVertexBuffer.Bind();
            solidVertexBuffer.BufferData();
            solidVertexBuffer.Draw(PrimitiveType.Points);
            break;
      }
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.UseProgram(0);
      GL.BindVertexArray(0);
      if (solidVertexBuffer != null)
         solidVertexBuffer.Clear();
      if (vertexBuffer != null)
      vertexBuffer.Clear();
      m_currentOp = DisplayOperation.None;
      CheckError();
   }

   /// <summary>
   /// Draw series of connected lines connecting points ending in an arrow head.
   /// </summary>
   /// <param name="points">Points between which lines are drawn</param>
   /// <param name="width">Width of the lines</param>
   /// <param name="pattern">Bit pattern defining the line's dash style</param>
   /// <param name="antiAlias">Determines whether or not the line should be anti-aliased</param>
   /// <param name="arrowSize">The length of the arrow head in pixels</param>
   /// <param name="arrowShorten">The number of pixels (beyond arrowSize) by which the last line is shortened, and the arrowhead pulled back.</param>
   public void DrawArrow(System.Drawing.Point[] points, int width, short pattern, bool antiAlias, int arrowSize, int arrowShorten)
   {
      Initialize();
      Flush();
      ShaderProgram.SolidShader.Use(projectionMatrix);
      solidVertexArray.Bind();
      solidVertexBuffer.Bind();
      solidVertexBuffer.Clear();

      GL.Disable(texCap);
      if (antiAlias)
      {
         GL.Enable(EnableCap.LineSmooth);
         GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
         GL.Enable(EnableCap.PolygonSmooth);
         GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
      }
      else
      {
         GL.Disable(EnableCap.LineSmooth);
         GL.Disable(EnableCap.PolygonSmooth);
      }
      GL.LineWidth(width);

      if ((pattern != unchecked((short)(0xffff))) && (pattern != 0))
      {
         GL.Enable(EnableCap.LineStipple);
         GL.LineStipple(1, pattern);
      }
      else
      {
         GL.Disable(EnableCap.LineStipple);
      }

      for (int i = 0; i < points.Length - 1; i++)
         solidVertexBuffer.AddVertex(new ColoredVertex(points[i].X, points[i].Y, currentColor));
      int x = points[points.Length - 1].X;
      int y = points[points.Length - 1].Y;
      int dx = x - points[points.Length - 2].X;
      int dy = y - points[points.Length - 2].Y;
      float len = (float)Math.Sqrt(dx * dx + dy * dy);
      if (len > 1)
      {
         float ndx = dx * (arrowSize + arrowShorten) / len;
         float ndy = dy * (arrowSize + arrowShorten) / len;
         float x1 = x - ndx;
         float y1 = y - ndy;
         solidVertexBuffer.AddVertex(new ColoredVertex(x1, y1, currentColor));
         ndx = dx * arrowSize / len;
         ndy = dy * arrowSize / len;
         solidVertexBuffer.BufferData();
         solidVertexBuffer.Draw(PrimitiveType.LineStrip);
         solidVertexBuffer.Clear();

         GL.Enable(EnableCap.PolygonSmooth);
         GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
         solidVertexBuffer.AddVertex(new ColoredVertex(x1 - ndy / 2, y1 + ndx / 2, currentColor));
         solidVertexBuffer.AddVertex(new ColoredVertex(x1 + ndx, y1 + ndy, currentColor));
         solidVertexBuffer.AddVertex(new ColoredVertex(x1 + ndy / 2, y1 - ndx / 2, currentColor));
         solidVertexBuffer.BufferData();
         solidVertexBuffer.Draw(PrimitiveType.Triangles);
      }
      else
      {
         solidVertexBuffer.AddVertex(new ColoredVertex(points[points.Length - 1].X, points[points.Length - 1].Y, currentColor));
         solidVertexBuffer.BufferData();
         solidVertexBuffer.Draw(PrimitiveType.LineStrip);
      }
      solidVertexBuffer.Clear();
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.UseProgram(0);
      GL.BindVertexArray(0);
      m_currentOp = DisplayOperation.None;
   }

   /// <summary>
   /// Begins drawing a series of connected lines.
   /// </summary>
   /// <param name="width">Width of the lines.</param>
   /// <param name="pattern">Bit pattern determining how/if the line is dashed.</param>
   /// <param name="antiAlias">Determines if the lines are anti-aliased.</param>
   public void BeginLine(float width, short pattern, bool antiAlias)
   {
      Initialize();
      Flush();

      GL.Disable(texCap);
      if (antiAlias)
      {
         GL.Enable(EnableCap.LineSmooth);
         GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
      }
      else
      {
         GL.Disable(EnableCap.LineSmooth);
      }
      GL.LineWidth(width);

      if ((pattern != unchecked((short)(0xffff))) && (pattern != 0))
      {
         GL.Enable(EnableCap.LineStipple);
         GL.LineStipple(1, pattern);
      }
      else
      {
         GL.Disable(EnableCap.LineStipple);
      }
      m_currentOp = DisplayOperation.DrawLines;
      solidVertexBuffer.Clear();
      solidVertexBuffer.Bind();
      solidVertexArray.Bind();
      ShaderProgram.SolidShader.Use(projectionMatrix);
   }

   /// <summary>
   /// Continues a line started with <see cref="BeginLine"/>.
   /// </summary>
   /// <param name="x">Horizontal coordinate within the display</param>
   /// <param name="y">Vertical coordinate within the display</param>
   /// <remarks>First first call to LineTo sets the beginning point
   /// of the line. Coordinates are relative to the top left corner of
   /// the display, not the layer or map.</remarks>
   public void LineTo(int x, int y)
   {
      if (m_currentOp != DisplayOperation.DrawLines)
      {
         Initialize();
         Flush();
         BeginLine(1, 0, false);
      }
      ColoredVertex cv = new ColoredVertex(x, y, currentColor);
      solidVertexBuffer.AddVertex(new ColoredVertex(x, y, currentColor));
      endPoint = new System.Drawing.Point(x, y);
   }

   /// <summary>
   /// End a line begun with <see cref="BeginLine"/> with an arrowhead.
   /// </summary>
   /// <param name="x">Horizontal coordinate of the tip of the arrowhead</param>
   /// <param name="y">Vertical coordinate of the tip of the arrowhead</param>
   /// <param name="ArrowSize">Length of the arrowhead</param>
   public void ArrowTo(int x, int y, int ArrowSize)
   {
      if (m_currentOp != DisplayOperation.DrawLines)
      {
         Initialize();
         Flush();
         BeginLine(1, 0, true);
         solidVertexBuffer.AddVertex(new ColoredVertex(endPoint.X, endPoint.Y, currentColor));
      }
      int dx = (x - endPoint.X);
      int dy = (y - endPoint.Y);
      float len = (float)Math.Sqrt(dx * dx + dy * dy);
      if (len > 1)
      {
         float ndx = dx * ArrowSize / len;
         float ndy = dy * ArrowSize / len;
         float x1 = x - ndx;
         float y1 = y - ndy;
         solidVertexBuffer.AddVertex(new ColoredVertex(x1, y1, currentColor));
         Flush();

         m_currentOp = DisplayOperation.None;

         GL.Enable(EnableCap.PolygonSmooth);
         GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
         solidVertexBuffer.AddVertex(new ColoredVertex(x1 - ndy / 2, y1 + ndx / 2, currentColor));
         solidVertexBuffer.AddVertex(new ColoredVertex(x1 + ndx, y1 + ndy, currentColor));
         solidVertexBuffer.AddVertex(new ColoredVertex(x1 + ndy / 2, y1 - ndx / 2, currentColor));
         solidVertexBuffer.BufferData();
         solidVertexBuffer.Draw(PrimitiveType.Triangles);
         solidVertexBuffer.Clear();
      }
      else
         LineTo(x, y);
   }

   /// <summary>
   /// Draw a rectangular outline on the display.
   /// </summary>
   /// <param name="rect">Rectangle relative to the top left corner of the display.</param>
   /// <param name="pattern">Dash pattern applied to the lines forming the outline.</param>
   public void DrawRectangle(System.Drawing.Rectangle rect, short pattern)
   {
      Initialize();
      Flush();
      if ((pattern == 0) || (pattern == unchecked((short)(0xffff))))
         GL.Disable(EnableCap.LineStipple);
      else
      {
         GL.Enable(EnableCap.LineStipple);
         GL.LineStipple(1, pattern);
      }
      System.Drawing.RectangleF rectf = rect;
      rectf.Offset(0.5f, 0.5f); // Align to pixel grid
      GL.Disable(EnableCap.LineSmooth);
      GL.LineWidth(1);
      GL.Disable(texCap);
      solidVertexBuffer.Bind();
      solidVertexArray.Bind();
      ShaderProgram.SolidShader.Use(projectionMatrix);
      solidVertexBuffer.AddVertex(new ColoredVertex(rectf.X, rectf.Y, currentColor));
      solidVertexBuffer.AddVertex(new ColoredVertex(rectf.X, rectf.Y + rectf.Height - 1, currentColor));
      solidVertexBuffer.AddVertex(new ColoredVertex(rectf.X + rectf.Width - 1, rectf.Y + rectf.Height - 1, currentColor));
      solidVertexBuffer.AddVertex(new ColoredVertex(rectf.X + rectf.Width - 1, rectf.Y, currentColor));
      solidVertexBuffer.BufferData();
      solidVertexBuffer.Draw(PrimitiveType.LineLoop);
      solidVertexBuffer.Clear();
      GL.UseProgram(0);
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
   }

   /// <summary>
   /// Fills a rectangular area with a solid color
   /// </summary>
   /// <param name="rect">Rectangular area to fill.</param>
   /// <remarks>
   /// <see cref="SetColor"/> determines the color with which to fill
   /// the rectangle, and may be semi-translucent.
   /// <seealso cref="SetColor"/></remarks>
   public void FillRectangle(System.Drawing.Rectangle rect)
   {
      Initialize();
      Flush();
      m_currentOp = DisplayOperation.None;
      GL.Disable(texCap);
      solidVertexBuffer.Bind();
      ShaderProgram.SolidShader.Use(projectionMatrix);
      solidVertexArray.Bind();
      solidVertexBuffer.AddVertex(new ColoredVertex(rect.X, rect.Y, currentColor));
      solidVertexBuffer.AddVertex(new ColoredVertex(rect.X, rect.Y + rect.Height, currentColor));
      solidVertexBuffer.AddVertex(new ColoredVertex(rect.X + rect.Width, rect.Y + rect.Height, currentColor));
      solidVertexBuffer.AddVertex(new ColoredVertex(rect.X + rect.Width, rect.Y, currentColor));
      solidVertexBuffer.BufferData();
      solidVertexBuffer.Draw(PrimitiveType.Quads);
      solidVertexBuffer.Clear();
      GL.UseProgram(0);
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
   }

   /// <summary>
   /// Specifies the size of points drawn with <see cref="DrawPoint"/>.
   /// </summary>
   public int PointSize
   {
      set
      {
         Flush();
         m_currentOp = DisplayOperation.None;
         GL.PointSize(value);
      }
   }

   /// <summary>
   /// Draw a point in the display
   /// </summary>
   /// <param name="location">Coordinate relative to the top left corner of the display.</param>
   public void DrawPoint(System.Drawing.Point location)
   {
      if (m_currentOp != DisplayOperation.DrawPoints)
      {
         Initialize();
         Flush();
         GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
         GL.Enable(EnableCap.PointSmooth);
         GL.Disable(texCap);
         solidVertexBuffer.Bind();
         solidVertexArray.Bind();
         ShaderProgram.SolidShader.Use(projectionMatrix);
         m_currentOp = DisplayOperation.DrawPoints;
      }
      solidVertexBuffer.AddVertex(new ColoredVertex(location.X, location.Y, currentColor));
   }

   /// <summary>
   /// Set the current color for drawing operations.
   /// </summary>
   /// <param name="color">Color to select.</param>
   public void SetColor(System.Drawing.Color color)
   {
      currentColor = new Color4(color.R, color.G, color.B, color.A);
   }

   /// <summary>
   /// Set the current color for drawing operations.
   /// </summary>
   /// <param name="color">Color as an integer with bytes in ARGB order.</param>
   public void SetColor(int color)
   {
      SetColor(System.Drawing.Color.FromArgb(color));
   }

   /// <summary>
   /// Draw a shaded rectangular frame.
   /// </summary>
   /// <param name="inner">Inner, empty portion of the rectangle</param>
   /// <param name="thickness">Thickness of the frame in pixels.</param>
   /// <param name="color1">Background color</param>
   /// <param name="color2">Foreground dither color</param>
   public void DrawShadedRectFrame(System.Drawing.Rectangle inner, int thickness, System.Drawing.Color color1, System.Drawing.Color color2)
   {
      Initialize();
      Flush();
      GL.Disable(texCap);
      GL.Disable(EnableCap.PolygonStipple);
      SetColor(color1);
      solidVertexBuffer.Bind();
      solidVertexArray.Bind();
      ShaderProgram.SolidShader.Use(projectionMatrix);
      SendRectFramePoints(inner, thickness);
      solidVertexBuffer.BufferData();
      solidVertexBuffer.Draw(PrimitiveType.QuadStrip);
      solidVertexBuffer.Clear();
      GL.Enable(EnableCap.PolygonStipple);
      GL.PolygonStipple(shadedStipple);
      SetColor(color2);
      SendRectFramePoints(inner, thickness);
      solidVertexBuffer.BufferData();
      solidVertexBuffer.Draw(PrimitiveType.QuadStrip);
      solidVertexBuffer.Clear();
      GL.Disable(EnableCap.PolygonStipple);
   }

   private void SendRectFramePoints(System.Drawing.Rectangle inner, int thickness)
   {
      solidVertexBuffer.AddVertex(new ColoredVertex(inner.X - thickness, inner.Y - thickness, currentColor));
      solidVertexBuffer.AddVertex(new ColoredVertex(inner.X - 1, inner.Y - 1, currentColor));
      solidVertexBuffer.AddVertex(new ColoredVertex(inner.X - thickness, inner.Y + inner.Height + thickness - 1, currentColor));
      solidVertexBuffer.AddVertex(new ColoredVertex(inner.X - 1, inner.Y + inner.Height, currentColor));
      solidVertexBuffer.AddVertex(new ColoredVertex(inner.X + inner.Width + thickness - 1, inner.Y + inner.Height + thickness - 1, currentColor));
      solidVertexBuffer.AddVertex(new ColoredVertex(inner.X + inner.Width, inner.Y + inner.Height, currentColor));
      solidVertexBuffer.AddVertex(new ColoredVertex(inner.X + inner.Width + thickness - 1, inner.Y - thickness, currentColor));
      solidVertexBuffer.AddVertex(new ColoredVertex(inner.X + inner.Width, inner.Y - 1, currentColor));
      solidVertexBuffer.AddVertex(new ColoredVertex(inner.X - thickness, inner.Y - thickness, currentColor));
      solidVertexBuffer.AddVertex(new ColoredVertex(inner.X - 1, inner.Y - 1, currentColor));
   }

   /// <summary>
   /// Clear the display of all graphics.
   /// </summary>
   public void Clear()
   {
      Flush();
      GL.Clear(ClearBufferMask.AccumBufferBit | ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
   }

   /// <summary>
   /// Set the clipping rectangle that determines the area of the display which will be
   /// affected by all drawing operations.
   /// </summary>
   /// <param name="rect">Rectangle relative to the top-left corner of the display in pixel coordinates.</param>
   public void Scissor(System.Drawing.Rectangle rect)
   {
      Flush();
      GL.Enable(EnableCap.ScissorTest);
      System.Drawing.Size nativeSize = GetScreenSize(m_GameDisplayMode, false);
      GL.Scissor(rect.X, nativeSize.Height - (rect.Y + rect.Height), rect.Width, rect.Height);
   }

   /// <summary>
   /// Disables clipping defined with <see cref="Scissor"/>.
   /// </summary>
   public void ScissorOff()
   {
      Flush();
      GL.Disable(EnableCap.ScissorTest);
   }

   public static void CheckError()
   {
      if (GraphicsContext.CurrentContext == null)
         return;
      ErrorCode ec = GL.GetError();
      if (ec != 0)
      {
         throw new System.Exception(ec.ToString());
      }
   }

   /// <summary>
   /// Draws a string of text on the display using the "CoolFont" graphic sheet
   /// </summary>
   /// <param name="text">String to be drawn. Embedded line feeds (\n) are the only cause for line breaks.</param>
   /// <param name="x">Horizontal coordinate relative to the top left corner of the display where drawing begins.</param>
   /// <param name="y">Vertical coordinate relative to the top left corner of the display where drawing begins.</param>
   /// <remarks>An error will occur if there is no "CoolFont" graphic sheet in the project.
   /// This function is only a very basic implementation of text drawing intended for debugging purposes.
   /// For more full-featured text support, see the <see cref="GeneralRules.ShowMessage"/> rule function.</remarks>
   public void DrawText(string text, int x, int y)
   {
      const int charWidth = 13;
      const int charHeight = 18;
      if (m_DefaultFont == null)
      {
         object testExist = Project.Resources.GetObject("CoolFont");
         if (testExist == null)
            throw new ApplicationException("In order to use Display.DrawText, the project must have a Graphic Sheet named \"CoolFont\"");
         m_DefaultFont = GetTextureRef("CoolFont");
      }
      byte[] charBytes = System.Text.Encoding.ASCII.GetBytes(text);
      System.Drawing.PointF[] corners = new System.Drawing.PointF[]
         { new System.Drawing.PointF(0, 0),
           new System.Drawing.PointF(0, charHeight),
           new System.Drawing.PointF(charWidth, charHeight),
           new System.Drawing.PointF(charWidth, 0)};
      int origX = x;
      for (int charIdx = 0; charIdx < charBytes.Length; charIdx++)
      {
         byte curChar = charBytes[charIdx];
         if (curChar > 32)
         {
            int col = (curChar - 33) % 24;
            int row = (curChar - 33) / 24;
            System.Drawing.Rectangle sourceRect = new System.Drawing.Rectangle(
               col * charWidth, row * charHeight, charWidth, charHeight);
            DrawFrame(m_DefaultFont, sourceRect, corners, x, y);
            x += charWidth;
         }
         else if (curChar == 10)
         {
            x = origX;
            y += charHeight;
         }
      }
   }

   /// <summary>
   /// Set the properties of one of the display's light sources for real-time lighting effects.
   /// </summary>
   /// <param name="index">Indicates which light source to set. Must be between 0 and MAX_LIGHTS - 1, inclusive</param>
   /// <param name="windowCoordinate">Coordinate within the display at which the light should be positioned with the origin at the top left corner</param>
   /// <param name="falloff">Constant, linear and quadratic falloff of the light intensity. Google linear light falloff for details.</param>
   /// <param name="color">Color and intensity of the light source. Alpha channel indicates intensity.</param>
   /// <param name="aimX">Relative horizontal offset of direction in which the light points.</param>
   /// <param name="aimY">Relative vertical offset of direction in which the light points.</param>
   /// <param name="aimZ">Relative depth offset of direction in which the light points.</param>
   /// <param name="lightZ">Z position of light source relative to map layer. Positive moves from layer toward viewer.</param>
   /// <param name="apertureFocus">Determines how focused the light source is. 1 is an invisibly narrow beam and 0 covers a 180-degree arc.</param>
   /// <param name="apertureSoftness">Determines how soft the edges of the light beam are.</param>
   /// <param name="walls">Array of Vector3 structures specifying the endpoints of walls (in pairs)</param>
   /// <param name="wallCoordCount">Number of applicable (non-zero) elements in walls. This should be a multiple of 2.</param>
   public void SetLightSource(int index, Vector2 windowCoordinate, Vector3 falloff, System.Drawing.Color color,
      float aimX, float aimY, float aimZ, float lightZ, float apertureFocus, float apertureSoftness, Vector3[] walls, int wallCoordCount)
   {
      if (index >= LightSources.MAX_LIGHTS)
         throw new IndexOutOfRangeException("SetLightSource index must be less than MAX_LIGHTS");

      System.Drawing.Size nativeSize = GetScreenSize(m_GameDisplayMode, false);
      lights[currentView][index].Falloff = falloff;
      lights[currentView][index].Position = new Vector3(
         windowCoordinate.X, nativeSize.Height - windowCoordinate.Y, lightZ);
      lights[currentView][index].Color = color;
      lights[currentView][index].Aim = new Vector3(aimX, -aimY, aimZ);
      lights[currentView][index].ApertureFocus = apertureFocus;
      lights[currentView][index].ApertureSoftness = apertureSoftness;
      int wallIndex;
      for (wallIndex = 0; wallIndex < wallCoordCount; wallIndex++)
         lights[currentView][index][wallIndex] = new Vector3(walls[wallIndex].X, nativeSize.Height - walls[wallIndex].Y, walls[wallIndex].Z);
      while (wallIndex < LightSource.wallsPerLight * 2)
         lights[currentView][index][wallIndex++] = new Vector3(0, 0, 0);
   }

   /// <summary>
   /// Reset all light sources to initial default behavior
   /// </summary>
   public void ResetLights()
   {
      lights[currentView].Reset();
   }

   public const int MAX_LIGHTS = LightSources.MAX_LIGHTS;

   /// <summary>
   /// Return the rectangle into which code can draw using game-native coordinates
   /// </summary>
   /// <remarks>This rectangle's coordinates will be half of the physical coordinates if scaleFactor is 2.</remarks>
   public System.Drawing.Rectangle NativeDisplayRect
   {
      get
      {
         return new System.Drawing.Rectangle(DisplayRectangle.X, DisplayRectangle.Y, DisplayRectangle.Width / m_scaleFactor, DisplayRectangle.Height / m_scaleFactor);
      }
   }

   /// <summary>
   /// Controls how graphics output is buffered and copied to the display.
   /// </summary>
   /// <param name="copy">When true, the content of the buffer will be copied to the display, scaling
   /// the buffer to the size of the display if necessary.</param>
   /// <param name="enable">When true, subsequent drawing operations will go to a buffer whose size is
   /// determined by the native game display size instead od directly to the screen. When false, this
   /// buffer, if it exists will be bypassed.</param>
   /// <remarks>The purpose of this command is to optimize scaling because lighting effects
   /// can severly burden the GPU if they happen at the scaled up size. Therefore, when scaling up,
   /// everything is rendered to the smaller buffer, and then copied and scaled to the larger size.</remarks>
   private void Buffer(bool copy, bool enable)
   {
      if (copy)
      {
         Flush();
         GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
         CheckError();
         System.Drawing.Size nativeSize = GetScreenSize(m_GameDisplayMode, false);
         System.Drawing.Size displaySize = GetScreenSize(m_GameDisplayMode, true);
         Matrix4 copyMatrix = Matrix4.CreateOrthographicOffCenter(0, displaySize.Width, 0, displaySize.Height, .1f, 10f);
         GL.Viewport(ClientRectangle);
         ShaderProgram.NoLightShader.Use(copyMatrix);
         GL.ActiveTexture(TextureUnit.Texture0);
         GL.BindTexture(texTarget, texUnscaledOutput);
         int texLoc = ShaderProgram.NoLightShader.GetUniformLocation("tex");
         GL.Uniform1(texLoc, 0);
         CheckError();
         nolightVertexArray.Bind();
         GL.Disable(EnableCap.DepthTest);
         vertexBuffer.AddVertex(new TileVertex(0, 0, 0, 0));
         vertexBuffer.AddVertex(new TileVertex(0, displaySize.Height, 0, nativeSize.Height));
         vertexBuffer.AddVertex(new TileVertex(displaySize.Width, displaySize.Height, nativeSize.Width, nativeSize.Height));
         vertexBuffer.AddVertex(new TileVertex(displaySize.Width, 0, nativeSize.Width, 0));
         vertexBuffer.Bind();
         vertexBuffer.BufferData();
         vertexBuffer.Draw(PrimitiveType.Quads);
         Flush();
         GL.Viewport(0, 0, nativeSize.Width, nativeSize.Height);
      }

      if (enable)
      {
         if (frameBuffer == -1)
         {
            // Create frame buffer for scaling after rendering
            frameBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            CheckError();
         }
         if (texUnscaledOutput == -1)
         {
            texUnscaledOutput = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texUnscaledOutput);
            CheckError();
            System.Drawing.Size nativeSize = GetScreenSize(m_GameDisplayMode, false);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, nativeSize.Width, nativeSize.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            CheckError();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
            CheckError();
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texUnscaledOutput, 0);
            CheckError();
         }
         GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
         CheckError();
      }
      else if (!copy)
      {
         GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
         CheckError();
      }
   }

   /// <summary>
   /// Present the buffered display to the visible window.
   /// </summary>
   public void FinishFrame()
   {
      if (frameBuffer != -1)
         Buffer(true, false);
      SwapBuffers();
      if (frameBuffer != -1)
         Buffer(false, true);
   }
   #endregion

   #region ISerializable Members
   /// <summary>
   /// This is for internal use only and is needed to control behavior of the
   /// Display with respect to the Save Game functions.
   /// </summary>
   /// <param name="info">Internal use only</param>
   /// <param name="context">Not used</param>
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
public partial class DisplayRef : System.Runtime.Serialization.IObjectReference, System.Runtime.Serialization.ISerializable
{

   private DisplayRef(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
   {
   }

   /// <summary>
   /// This is for internal use only and is needed to control behavior of the
   /// Display with respect to the Save Game functions.
   /// </summary>
   public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
   {
      throw new System.NotImplementedException("Unexpected serialization call");
   }

   #region IObjectReference Members
   /// <summary>
   /// This is for internal use only and is needed to control behavior of the
   /// Display with respect to the Load Game functions.
   /// </summary>
   public object GetRealObject(System.Runtime.Serialization.StreamingContext context)
   {
      return Project.GameWindow.GameDisplay;
   }
   #endregion
}