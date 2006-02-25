/*
 * Scrolling Game Development Kit 2.0
 *
 * Copyright © 2000 - 2006 Benjamin Marty <BlueMonkMN@email.com>
 * 
 * Distributed under the GNU General Public License (GPL)
 *   - see included file COPYING.txt for details, or visit:
 *     http://www.fsf.org/copyleft/gpl.html
 */

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Resources;
using System.Reflection;

namespace SGDK2
{
   /// <summary>
   /// Summary description for SGDK2.
   /// </summary>
   public class SGDK2IDE
   {
      #region Non-Control Members
      public static ResourceManager g_Resources = null;
      #endregion

      private SGDK2IDE()
      {
      }

      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main()
      {
         g_Resources = new ResourceManager("SGDK2.SGDK2IDE", Assembly.GetExecutingAssembly());
         new frmSplashForm(GetSplashImage()).Show();
         Application.Run(new frmMain());
      }

      #region Private Functions
      private static Bitmap GetSplashImage()
      {
         Bitmap bmp = (Bitmap)SGDK2IDE.g_Resources.GetObject("SplashImage");
         Graphics gc = Graphics.FromImage(bmp);
         gc.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
         Font fnt = new Font("Verdana", 9);
         String sCopyright = "Copyright ©2000-2004\n      Benjamin Marty";
         gc.DrawString(sCopyright, fnt, Brushes.Sienna, 101, 216);
         gc.DrawString(sCopyright, fnt, Brushes.White, 100, 215);
         fnt.Dispose();
         fnt = new Font("Tahoma", 8);
         gc.DrawString("v2.0.0", fnt, Brushes.White, 140, 200);
         fnt.Dispose();
         gc.Dispose();
         return bmp;
      }
      #endregion

      #region Public Functions
      /// <summary>
      /// GDI+ has a bug copying bitmaps.  Change this method
      /// when it is fixed. (near-opaque pixels lose 1 RGB during DrawImage)
      /// </summary>
      /// <param name="Dest">Target image</param>
      /// <param name="Src">Source image</param>
      public static void CopyImage (Bitmap Dest, Bitmap Src)
      {
         System.Drawing.Imaging.BitmapData datDest =
            Dest.LockBits(new Rectangle(0,0,Dest.Width,Dest.Height),
            System.Drawing.Imaging.ImageLockMode.WriteOnly,
            Src.PixelFormat);
         System.Drawing.Imaging.BitmapData datSrc =
            Src.LockBits(new Rectangle(0,0,Src.Width,Src.Height),
            System.Drawing.Imaging.ImageLockMode.ReadOnly, Src.PixelFormat);
         Int32 cbSize = Math.Abs(datSrc.Stride) * datSrc.Height;
         byte[] buffer = new byte[cbSize];
         System.Runtime.InteropServices.Marshal.Copy(datSrc.Scan0, buffer, 0, cbSize);
         Src.UnlockBits(datSrc);
         System.Runtime.InteropServices.Marshal.Copy(buffer, 0, datDest.Scan0, cbSize);
         Dest.UnlockBits(datDest);
      }

      /// <summary>
      /// Load a cursor from a resource that contains the base-64 encoded cursor file
      /// data.
      /// </summary>
      /// <param name="CursorName">Name of the resource containing the byte array</param>
      /// <returns>A Cursor object</returns>
      /// <remarks>The .NET framework version 1.1 has a bug that prevents cursors from
      /// properly constructing directly from a typed resource (they lose the hotspot)
      /// so it's necessary to just read it as a byte array and manually construct
      /// the cursor from a stream.</remarks>
      public static Cursor LoadCursor(string CursorName)
      {
         System.IO.MemoryStream stm = new System.IO.MemoryStream(
            (byte[])g_Resources.GetObject(CursorName), false);
         Cursor cur = new Cursor(stm);
         stm.Close();
         return cur;
      }
      #endregion
   }
}