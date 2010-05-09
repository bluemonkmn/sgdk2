/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */

using System;
using System.Collections;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SGDK2
{
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

   public enum DisplayOperation
   {
      None=0,
      DrawFrames,
      DrawLines,
      DrawPoints
   }

   /// <summary>
   /// Manages the display device on which real-time game graphics are drawn
   /// </summary>
   [Serializable()]
   public class Display : GLControl, IDisposable
   {
      #region Embedded Classes
      public class TextureRef : IDisposable
      {
         private string m_Name;
         private int m_Texture = 0;
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

         public int Texture
         {
            get
            {
               if (m_Texture == 0)
               {
                  m_Display.MakeCurrent();
                  m_Texture = m_Display.GetTexture(m_Name);
               }
               return m_Texture;
            }
         }

         #region IDisposable Members
         public void Dispose()
         {
            if (m_Texture != 0)
            {
               if (m_Display.Context != null)
               {
                  if (!m_Display.Context.IsCurrent)
                     m_Display.MakeCurrent();
                  GL.DeleteTextures(1, ref m_Texture);
               }
               m_Texture = 0;
            }
         }
         #endregion
      }
      #endregion

      #region Fields
      private System.Collections.Generic.Dictionary<string, WeakReference> m_TextureRefs = null;
      private GameDisplayMode m_GameDisplayMode;
      private DisplayOperation m_currentOp;
      private TextureRef m_currentTexture = null;
      private bool scaleNativeSize = false;
      public const TextureTarget texTarget = TextureTarget.TextureRectangleArb;
      public const EnableCap texCap = (EnableCap)ArbTextureRectangle.TextureRectangleArb;
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
      public bool requirementsChecked = false;
      #endregion

      #region Initialization and clean-up
      public Display() : this(GameDisplayMode.m640x480x24, true)
      {
      }

      public Display(GameDisplayMode mode, bool windowed) : base(CreateGraphicsMode(mode))
      {
         m_GameDisplayMode = mode;
      }

      protected override void Dispose(bool disposing)
      {
         if (disposing)
         {
            DisposeAllTextures();
         }
         base.Dispose (disposing);
      }
      #endregion

      #region Overrides
      protected override void OnResize(EventArgs e)
      {
         base.OnResize(e);
         if ((GraphicsContext.CurrentContext == null) || (!this.IsHandleCreated))
            return;
         MakeCurrent();
         GL.Finish();
         GL.Viewport(0, 0, ClientSize.Width, ClientSize.Height);
         GL.MatrixMode(MatrixMode.Projection);
         GL.LoadIdentity();
         if (scaleNativeSize)
         {
            var nativeSize = GetScreenSize(m_GameDisplayMode);
            GL.Ortho(0, nativeSize.Width, nativeSize.Height, 0, -1, 1);
         }
         else
         {
            GL.Ortho(0, ClientSize.Width, ClientSize.Height, 0, -1, 1);
         }
      }
      #endregion

      #region Private members
      private int GetTexture(string Name)
      {
         int texture;
         GL.GenTextures(1, out texture);
         GL.BindTexture(texTarget, texture);
         GL.TexParameter(texTarget, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
         GL.TexParameter(texTarget, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
         System.Drawing.Bitmap bmpTexture = (System.Drawing.Bitmap)ProjectData.GetGraphicSheetImage(Name, false);

         int texSize;
         GL.GetInteger(GetPName.MaxTextureSize, out texSize);
         if ((texSize < bmpTexture.Width) ||
             (texSize < bmpTexture.Height))
            throw new System.ApplicationException("Texture " + Name + " is size " + bmpTexture.Width + "x" + bmpTexture.Height +
               ", but the current OpenGL video drivers only support textures up to " + texSize.ToString());

         var bits = bmpTexture.LockBits(new System.Drawing.Rectangle(0, 0, bmpTexture.Width, bmpTexture.Height),
            System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
         try
         {
            GL.TexImage2D(texTarget, 0, PixelInternalFormat.Rgba8, bmpTexture.Width, bmpTexture.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bits.Scan0);
         }
         finally
         {
            bmpTexture.UnlockBits(bits);
         }
         return texture;
      }

      private static void CheckError()
      {
         ErrorCode ec = GL.GetError();
         if (ec != 0)
         {
            throw new System.Exception(ec.ToString());
         }
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
            foreach (var de in m_TextureRefs)
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

      public void CheckRequirements()
      {
         if (!requirementsChecked)
         {
            requirementsChecked = true;
            GL.Finish();
            string[] versionParts = GL.GetString(StringName.Version).Split(new char[] {'.'}, 3);
            int majorVer = int.Parse(versionParts[0]);
            int minorVer = int.Parse(versionParts[1]);
            if ((majorVer < 1 ) || ((majorVer == 1) && (minorVer < 2)))
            {
               string errString = "OpenGL version 1.2 is required";
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
            if (!GL.GetString(StringName.Extensions).Contains("GL_ARB_texture_rectangle"))
            {
               System.Windows.Forms.MessageBox.Show(this, "GL_ARB_texture_rectangle may be required for proper operation. The current video driver does not support this feature. Try updating your video drivers.", "Requirement Check Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            }
         }
      }

      /// <summary>
      /// Completes the presentation parameter structure by filling out a back buffer
      /// width, height, and format.
      /// </summary>
      /// <param name="mode">Game requested display mode</param>
      /// <param name="pp">Object to finish populating</param>
      public static GraphicsMode CreateGraphicsMode(GameDisplayMode mode) 
      {
         System.Drawing.Size screenSize = GetScreenSize(mode);
         int depth;

         switch (mode)
         {
            case GameDisplayMode.m320x240x16:
            case GameDisplayMode.m640x480x16:
            case GameDisplayMode.m800x600x16:
            case GameDisplayMode.m1024x768x16:
            case GameDisplayMode.m1280x1024x16:
               depth = 16;
               break;
            default:
               depth = 24;
               break;
         }
         return new GraphicsMode(new ColorFormat(depth));
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
            // this.Context forces context to be created
            if ((this.IsHandleCreated) && (this.Context != null))
            {
               GL.MatrixMode(MatrixMode.Projection);
               GL.LoadIdentity();
               var naturalSize = GetScreenSize(value);
               GL.Ortho(0, naturalSize.Width, naturalSize.Height, 0, -1, 1);
            }
         }
      }

      /// <summary>
      /// Draw a rectangle from a texture on the display
      /// </summary>
      /// <param name="texture">Texture from which graphics are copied</param>
      /// <param name="sourceRect">Specifies the source area of the copy</param>
      /// <param name="corners">Specifies the corners (counter-clockwise) of the output quadrilateral</param>
      /// <param name="offsetX">Specifies the horizontal (rightward) offset of the corners</param>
      /// <param name="offsetY">Specifies the vertical (downward) offset of the corners</param>
      public void DrawFrame(TextureRef texture, System.Drawing.Rectangle sourceRect, System.Drawing.Point[] corners, int offsetX, int offsetY)
      {
         if ((m_currentOp != DisplayOperation.DrawFrames) ||
             (m_currentTexture != texture))
         {
            if (m_currentOp != DisplayOperation.None)
               GL.End();
            CheckError();

            CheckRequirements();
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(texCap);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Dither);

            CheckError();
            GL.BindTexture(texTarget, texture.Texture);
            GL.Begin(BeginMode.Quads);
            m_currentOp = DisplayOperation.DrawFrames;
            m_currentTexture = texture;
         }
         GL.TexCoord2(sourceRect.Left, sourceRect.Top);
         GL.Vertex2(corners[0].X + offsetX, corners[0].Y + offsetY);
         GL.TexCoord2(sourceRect.Left, sourceRect.Top + sourceRect.Height);
         GL.Vertex2(corners[1].X + offsetX, corners[1].Y + offsetY);
         GL.TexCoord2(sourceRect.Left + sourceRect.Width, sourceRect.Top + sourceRect.Height);
         GL.Vertex2(corners[2].X + offsetX, corners[2].Y + offsetY);
         GL.TexCoord2(sourceRect.Left + sourceRect.Width, sourceRect.Top);
         GL.Vertex2(corners[3].X + offsetX, corners[3].Y + offsetY);
      }

      /// <summary>
      /// Finishes any pending drawing operation on the display.
      /// </summary>
      public void Flush()
      {
         if (m_currentOp != DisplayOperation.None)
         {
            GL.End();
            m_currentOp = DisplayOperation.None;
         }
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
         if (m_currentOp != DisplayOperation.None)
            GL.End();
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
         GL.Begin(BeginMode.LineStrip);
         for (int i=0; i<points.Length - 1; i++)
            GL.Vertex2(points[i].X, points[i].Y);
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
            GL.Vertex2(x1, y1);
            ndx = dx * arrowSize / len;
            ndy = dy * arrowSize / len;
            GL.End();

            m_currentOp = DisplayOperation.None;

            GL.Enable(EnableCap.PolygonSmooth);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
            GL.Begin(BeginMode.Triangles);
            GL.Vertex2(x1 - ndy / 2, y1 + ndx / 2);
            GL.Vertex2(x1 + ndx, y1 + ndy);
            GL.Vertex2(x1 + ndy / 2, y1 - ndx / 2);
            GL.End();
         }
         else
         {
            GL.Vertex2(points[points.Length - 1].X, points[points.Length - 1].Y);
            GL.End();
         }
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
         if (m_currentOp != DisplayOperation.None)
            GL.End();
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
         GL.Begin(BeginMode.LineStrip);
         m_currentOp = DisplayOperation.DrawLines;
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
            if (m_currentOp != DisplayOperation.None)
               GL.End();
            BeginLine(1, 0, false);
         }
         GL.Vertex2(x, y);
         endPoint = new System.Drawing.Point(x,y);
      }

      /// <summary>
      /// End a line begun with <see cref="BeginLine"/> with an arrowhead.
      /// </summary>
      /// <param name="x">Horizontal coordinate of the tip of the arrow head</param>
      /// <param name="y">Vertical coordinate of the tip of the arrowhead</param>
      /// <param name="ArrowSize">Length of the arrowhead</param>
      public void ArrowTo(int x, int y, int ArrowSize)
      {
         if (m_currentOp != DisplayOperation.DrawLines)
         {
            if (m_currentOp != DisplayOperation.None)
               GL.End();
            BeginLine(1, 0, true);
            GL.Vertex2(endPoint.X, endPoint.Y);
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
            GL.Vertex2(x1, y1);
            GL.End();

            m_currentOp = DisplayOperation.None;

            GL.Enable(EnableCap.PolygonSmooth);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
            GL.Begin(BeginMode.Triangles);
            GL.Vertex2(x1 - ndy / 2, y1 + ndx / 2);
            GL.Vertex2(x1 + ndx, y1 + ndy);
            GL.Vertex2(x1 + ndy / 2, y1 - ndx / 2);
            GL.End();
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
         if (m_currentOp != DisplayOperation.None)
         {
            GL.End();
            m_currentOp = DisplayOperation.None;
         }
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
         GL.Begin(BeginMode.LineLoop);
         GL.Vertex2(rectf.X, rectf.Y);
         GL.Vertex2(rectf.X, rectf.Y + rectf.Height - 1);
         GL.Vertex2(rectf.X + rectf.Width - 1, rectf.Y + rectf.Height - 1);
         GL.Vertex2(rectf.X + rectf.Width - 1, rectf.Y);
         GL.End();
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
         if (m_currentOp != DisplayOperation.None)
            GL.End();
         m_currentOp = DisplayOperation.None;
         GL.Disable(texCap);
         GL.Begin(BeginMode.Quads);
         GL.Vertex2(rect.X, rect.Y);
         GL.Vertex2(rect.X, rect.Y + rect.Height);
         GL.Vertex2(rect.X + rect.Width, rect.Y + rect.Height);
         GL.Vertex2(rect.X + rect.Width, rect.Y);
         GL.End();
      }

      /// <summary>
      /// Specifies the size of points drawn with <see cref="DrawPoint"/>.
      /// </summary>
      public int PointSize
      {
         set
         {
            if (m_currentOp != DisplayOperation.None)
               GL.End();
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
            if (m_currentOp != DisplayOperation.None)
               GL.End();
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            GL.Enable(EnableCap.PointSmooth);
            GL.Disable(texCap);
            GL.Begin(BeginMode.Points);
            m_currentOp = DisplayOperation.DrawPoints;
         }
         GL.Vertex2(location.X, location.Y);
      }

      /// <summary>
      /// Set the current color for drawing operations.
      /// </summary>
      /// <param name="color">Color to select.</param>
      public void SetColor(System.Drawing.Color color)
      {
         GL.Color4(color.R, color.G, color.B, color.A);
      }

      /// <summary>
      /// Set the current color for drawing operations.
      /// </summary>
      /// <param name="color">Color as an integer with bytes in ARGB order.</param>
      public void SetColor(int color)
      {
         var c = System.Drawing.Color.FromArgb(color);
         SetColor(c);
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
         if (m_currentOp != DisplayOperation.None)
            GL.End();
         m_currentOp = DisplayOperation.None;
         GL.Disable(texCap);
         GL.Disable(EnableCap.PolygonStipple);
         SetColor(color1);
         GL.Begin(BeginMode.QuadStrip);
         SendRectFramePoints(inner, thickness);
         GL.End();
         GL.Enable(EnableCap.PolygonStipple);
         GL.PolygonStipple(shadedStipple);
         SetColor(color2);
         GL.Begin(BeginMode.QuadStrip);
         SendRectFramePoints(inner, thickness);
         GL.End();
         GL.Disable(EnableCap.PolygonStipple);
      }

      private void SendRectFramePoints(System.Drawing.Rectangle inner, int thickness)
      {
         GL.Vertex2(inner.X - thickness, inner.Y - thickness);
         GL.Vertex2(inner.X - 1, inner.Y - 1);
         GL.Vertex2(inner.X - thickness, inner.Y + inner.Height + thickness - 1);
         GL.Vertex2(inner.X - 1, inner.Y + inner.Height);
         GL.Vertex2(inner.X + inner.Width + thickness - 1, inner.Y + inner.Height + thickness - 1);
         GL.Vertex2(inner.X + inner.Width, inner.Y + inner.Height);
         GL.Vertex2(inner.X + inner.Width + thickness - 1, inner.Y - thickness);
         GL.Vertex2(inner.X + inner.Width, inner.Y - 1);
         GL.Vertex2(inner.X - thickness, inner.Y - thickness);
         GL.Vertex2(inner.X - 1, inner.Y - 1);
      }

      /// <summary>
      /// Clear the display of all graphics.
      /// </summary>
      public void Clear()
      {
         GL.Clear(ClearBufferMask.AccumBufferBit | ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
      }

      #endregion
   }
}
