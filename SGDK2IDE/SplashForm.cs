/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SGDK2
{
	public class frmSplashForm : System.Windows.Forms.Form
	{
      #region API declarations
      [DllImport("user32.dll")]
      private static extern int UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point ptTopLeft, ref Size pSize, IntPtr hdcSrc, ref Point ptSrcRef, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

      [StructLayout(LayoutKind.Sequential, Pack=1)]
      private struct BLENDFUNCTION
      {
         public byte BlendOp;
         public byte BlendFlags;
         public byte SourceConstantAlpha;
         public byte AlphaFormat;
      }

      [DllImport("gdi32.dll")]
      private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

      [DllImport("gdi32.dll")]
      private static extern int DeleteDC(IntPtr hDC);

      [DllImport("gdi32.dll")]
      private static extern int DeleteObject(IntPtr hObject);

      [DllImport("User32.dll")] 
      private extern static System.IntPtr GetDC(System.IntPtr hWnd); 

      [DllImport("User32.dll")] 
      private extern static int ReleaseDC(System.IntPtr hWnd, System.IntPtr hDC);

      private void InitializeComponent()
      {
         // 
         // frmSplashForm
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(292, 273);
         this.Name = "frmSplashForm";

      }

      [DllImport("gdi32.dll", ExactSpelling=true)]
      private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
      #endregion

      #region Fields
      private Point m_ptOffset = Point.Empty;
      private Bitmap m_SplashImage = null;
      private Timer m_tmrExpire = null;
      #endregion

      #region Initialization
      private frmSplashForm()
      {
         this.StartPosition = FormStartPosition.CenterScreen;
         this.TopMost = true;
         m_tmrExpire = new Timer();
         m_tmrExpire.Tick += new EventHandler(tmrExpire_Tick);
         m_tmrExpire.Interval = 4000;
         m_tmrExpire.Start();
         this.ShowInTaskbar = false;
      }

      public frmSplashForm(Bitmap image) : this()
      {
         m_SplashImage = image;
         this.ClientSize = image.Size;
      }
      #endregion

      #region Overrides
      protected override void OnMouseDown(MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Left)
            m_ptOffset = new Point(e.X, e.Y);
      }

      protected override void OnMouseMove(MouseEventArgs e)
      {
         if (0 == (Int32)(e.Button & MouseButtons.Left))
            m_ptOffset = Point.Empty;
         if (m_ptOffset != Point.Empty)
            this.Location = new Point(Location.X + e.X - m_ptOffset.X, Location.Y + e.Y - m_ptOffset.Y);
      }

      protected override void OnMouseUp(MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Left)
            m_ptOffset = Point.Empty;
      }

      protected override void OnLoad(EventArgs e)
      {
         try
         {
            BLENDFUNCTION bf;

            String strAppDir = Application.ExecutablePath;
            strAppDir = System.IO.Path.GetDirectoryName(strAppDir);

            bf.BlendOp = 0; // AC_SRC_OVER
            bf.BlendFlags = 0;
            bf.SourceConstantAlpha = 255;
            bf.AlphaFormat = 1; // AC_SRC_ALPHA

            IntPtr hdcScreen = GetDC(IntPtr.Zero);
            IntPtr hdcImage = CreateCompatibleDC(hdcScreen);
            IntPtr hBmp = m_SplashImage.GetHbitmap(Color.FromArgb(0));
            IntPtr bmpOld = SelectObject(hdcImage, hBmp);

            Point ptSrc = new Point(0, 0);
            Size szImg = m_SplashImage.Size;
            Point ptTopLeft = this.Location;

            UpdateLayeredWindow(this.Handle, hdcScreen, ref ptTopLeft, ref szImg, hdcImage, ref ptSrc, 0, ref bf, 2 /* ULW_ALPHA */);

            SelectObject(hdcImage, bmpOld);
            DeleteObject(hBmp);
            DeleteDC(hdcImage);
            m_SplashImage.Dispose();
            m_SplashImage = null;
            ReleaseDC(IntPtr.Zero, hdcScreen);
         }
         catch
         {
         }
      }

      protected override CreateParams CreateParams
      {
         get 
         {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x00080000; // WS_EX_LAYERED
            return cp;
         }
      }

      protected override void OnDoubleClick(EventArgs e)
      {
         this.Close();
         base.OnDoubleClick (e);
      }
      #endregion

      #region Event Handlers
      private void tmrExpire_Tick(object sender, EventArgs e)
      {
         m_tmrExpire.Dispose();
         m_tmrExpire = null;
         this.Close();
      }
      #endregion
   }
}
