/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */

using System;
using System.Windows.Forms;
using System.Collections;
using OpenTK;
using OpenTK.OpenGL;
using OpenTK.OpenGL.Enums;

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
   public class Display : GLControl
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
      #endregion

      #region Initialization and clean-up
      public Display() : this(GameDisplayMode.m640x480x24, true)
      {
      }

      public Display(GameDisplayMode mode, bool windowed) : base(CreateDisplayMode(mode, windowed))
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
         GL.BindTexture(TextureTarget.TextureRectangleNv, texture);
         GL.TexParameter(TextureTarget.TextureRectangleNv, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
         GL.TexParameter(TextureTarget.TextureRectangleNv, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
         System.Drawing.Bitmap bmpTexture = (System.Drawing.Bitmap)ProjectData.GetGraphicSheetImage(Name, false);
         var bits = bmpTexture.LockBits(new System.Drawing.Rectangle(0, 0, bmpTexture.Width, bmpTexture.Height),
            System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
         try
         {
            GL.TexImage2D(TextureTarget.TextureRectangleNv, 0, PixelInternalFormat.Rgba, bmpTexture.Width, bmpTexture.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bits.Scan0);
         }
         finally
         {
            bmpTexture.UnlockBits(bits);
         }
         return texture;
      }
      #endregion

      #region Public members
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

      /// <summary>
      /// Completes the presentation parameter structure by filling out a back buffer
      /// width, height, and format.
      /// </summary>
      /// <param name="mode">Game requested display mode</param>
      /// <param name="pp">Object to finish populating</param>
      public static DisplayMode CreateDisplayMode(GameDisplayMode mode, bool windowed) 
      {
         var screenSize = GetScreenSize(mode);
         int depth;

         // TODO: Check for hardware support
/*         if (!Manager.CheckDeviceType(Manager.Adapters.Default.Adapter, DeviceType.Hardware, Manager.Adapters.Default.CurrentDisplayMode.Format, pp.BackBufferFormat, pp.Windowed))
            throw new ApplicationException("No hardware support for windowed mode on default display adapter");*/
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
         return new DisplayMode(screenSize.Width, screenSize.Height, 0, depth, !windowed);
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
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            var naturalSize = GetScreenSize(value);
            GL.Ortho(0, naturalSize.Width, naturalSize.Height, 0, -1, 1);
         }
      }

      public void DrawFrame(TextureRef texture, System.Drawing.Rectangle sourceRect, System.Drawing.Point[] corners, int offsetX, int offsetY)
      {
         if ((m_currentOp != DisplayOperation.DrawFrames) ||
             (m_currentTexture != texture))
         {
            if (m_currentOp != DisplayOperation.None)
               GL.End();
            else
            {
               GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
               GL.Enable(EnableCap.Blend);
               GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
               GL.Enable((EnableCap)NvTextureRectangle.TextureRectangleNv);
               GL.Disable(EnableCap.DepthTest);
               GL.Disable(EnableCap.Lighting);
               GL.Disable(EnableCap.Dither);
            }
            GL.BindTexture(TextureTarget.TextureRectangleNv, texture.Texture);
            GL.Begin(BeginMode.Quads);
            m_currentOp = DisplayOperation.DrawFrames;
            m_currentTexture = texture;
         }
         GL.TexCoord2(sourceRect.Left, sourceRect.Top);
         GL.Vertex2(corners[0].X + offsetX, corners[0].Y + offsetY);
         GL.TexCoord2(sourceRect.Left, sourceRect.Top + sourceRect.Height - 1);
         GL.Vertex2(corners[1].X + offsetX, corners[1].Y + offsetY);
         GL.TexCoord2(sourceRect.Left + sourceRect.Width - 1, sourceRect.Top + sourceRect.Height - 1);
         GL.Vertex2(corners[2].X + offsetX, corners[2].Y + offsetY);
         GL.TexCoord2(sourceRect.Left + sourceRect.Width - 1, sourceRect.Top);
         GL.Vertex2(corners[3].X + offsetX, corners[3].Y + offsetY);
      }

      public void Flush()
      {
         if (m_currentOp != DisplayOperation.None)
         {
            GL.End();
            m_currentOp = DisplayOperation.None;
         }
      }

      public void BeginLine(float width, short pattern, bool antiAlias)
      {
         if (m_currentTexture != null)
            GL.BindTexture(TextureTarget.TextureRectangleNv, 0);
         if (m_currentOp != DisplayOperation.None)
            GL.End();
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
         GL.Begin(BeginMode.Lines);
         m_currentOp = DisplayOperation.DrawLines;
      }

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

      public void ArrowTo(int x, int y)
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
            float ndx = dx * 7 / len;
            float ndy = dy * 7 / len;
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
         GL.Disable(EnableCap.LineSmooth);
         GL.LineWidth(1);
         if (m_currentTexture != null)
         {
            m_currentTexture = null;
            GL.BindTexture(TextureTarget.TextureRectangleNv, 0);
         }
         GL.Begin(BeginMode.LineLoop);
         GL.Vertex2(rect.X, rect.Y);
         GL.Vertex2(rect.X, rect.Y + rect.Height - 1);
         GL.Vertex2(rect.X + rect.Width - 1, rect.Y + rect.Height - 1);
         GL.Vertex2(rect.X + rect.Width - 1, rect.Y);
         GL.End();
      }

      public void FillRectangle(System.Drawing.Rectangle rect)
      {
         if (m_currentOp != DisplayOperation.None)
            GL.End();
         m_currentOp = DisplayOperation.None;
         if (m_currentTexture != null)
         {
            m_currentTexture = null;
            GL.BindTexture(TextureTarget.TextureRectangleNv, 0);
         }
         GL.Begin(BeginMode.Quads);
         GL.Vertex2(rect.X, rect.Y);
         GL.Vertex2(rect.X, rect.Y + rect.Height - 1);
         GL.Vertex2(rect.X + rect.Width - 1, rect.Y + rect.Height - 1);
         GL.Vertex2(rect.X + rect.Width - 1, rect.Y);
         GL.End();
      }

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

      public void DrawPoint(System.Drawing.Point location)
      {
         if (m_currentOp != DisplayOperation.DrawPoints)
         {
            if (m_currentOp != DisplayOperation.None)
               GL.End();
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            GL.Enable(EnableCap.PointSmooth);
            GL.Begin(BeginMode.Points);
            m_currentOp = DisplayOperation.DrawPoints;
         }
         if (m_currentTexture != null)
         {
            m_currentTexture = null;
            GL.BindTexture(TextureTarget.TextureRectangleNv, 0);
         }
      }

      public void SetColor(System.Drawing.Color color)
      {
         //GL.Color4(color.R, color.G, color.B, color.A);
      }

      public void SetColor(int color)
      {
         //GL.Color4(ref color);
      }

      public void DrawShadedRectFrame(System.Drawing.Rectangle inner, int thickness, System.Drawing.Color color1, System.Drawing.Color color2)
      {
         if (m_currentOp != DisplayOperation.None)
            GL.End();
         m_currentOp = DisplayOperation.None;
         if (m_currentTexture != null)
         {
            m_currentTexture = null;
            GL.BindTexture(TextureTarget.TextureRectangleNv, 0);
         }
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
         GL.Vertex2(inner.X - 1, inner.Y + Height);
         GL.Vertex2(inner.X + inner.Width + thickness - 1, inner.Y + inner.Height + thickness - 1);
         GL.Vertex2(inner.X + inner.Width, inner.Y + inner.Height);
         GL.Vertex2(inner.X + inner.Width + thickness - 1, inner.Y - thickness);
         GL.Vertex2(inner.X + inner.Width, inner.Y - 1);
         GL.Vertex2(inner.X - thickness, inner.Y - thickness);
         GL.Vertex2(inner.X - 1, inner.Y - 1);
      }

      public void Clear()
      {
         GL.Clear(ClearBufferMask.AccumBufferBit | ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
      }
      #endregion
   }
}
