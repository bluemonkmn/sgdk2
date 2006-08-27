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
      #region Embedded Types
      public class CommandLineArgs
      {
         public string ProjectFile = null;

         public CommandLineArgs()
         {
         }
      }
      #endregion

      #region Non-Control Members
      public static ResourceManager g_Resources = null;
      public static CommandLineArgs g_CommandLine = new CommandLineArgs();
      #endregion

      private SGDK2IDE()
      {
      }

      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static int Main(params string[] args)
      {
         int result = GetCommandLine(args);
         if (result != 0)
            return result;
         g_Resources = new ResourceManager("SGDK2.SGDK2IDE", Assembly.GetExecutingAssembly());
         CreateFileAssociation();
         new frmSplashForm(GetSplashImage()).Show();
         Application.Run(new frmMain());
         CodeGenerator.ResetTempAssembly();
         return 0;
      }

      #region Private Functions
      private static Bitmap GetSplashImage()
      {
         Bitmap bmp = (Bitmap)SGDK2IDE.g_Resources.GetObject("SplashImage");
         Graphics gc = Graphics.FromImage(bmp);
         gc.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
         Font fnt = new Font("Verdana", 9);

         System.Drawing.StringFormat fmt = new System.Drawing.StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip);
         fmt.Alignment = StringAlignment.Center;
         String sCopyright = "Copyright ©2000-2006\nBenjamin Marty";
         Rectangle rcFmt = new Rectangle(40, 164, 130, 25);
         gc.DrawString(sCopyright, fnt, Brushes.Sienna, rcFmt, fmt);
         rcFmt.Offset(-1,-1);
         gc.DrawString(sCopyright, fnt, Brushes.White, rcFmt, fmt);
         fnt.Dispose();
         fnt = new Font("Tahoma", 8);
         AssemblyFileVersionAttribute ver = (AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyFileVersionAttribute));
         rcFmt = new Rectangle(88, 151, 27, 10);
         gc.DrawString("v" + ver.Version, fnt, Brushes.White, rcFmt, fmt);
         fmt.Dispose();
         fnt.Dispose();
         gc.Dispose();
         return bmp;
      }

      private static void CreateFileAssociation()
      {
         try
         {
            Microsoft.Win32.RegistryKey testKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(".sgdk2", false);
            if (testKey == null)
            {
               Microsoft.Win32.RegistryKey extKey = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(".sgdk2");
               try
               {
                  Microsoft.Win32.RegistryKey progIdKey = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey("sgdk2file");
                  try
                  {
                     extKey.SetValue(null, "sgdk2file");
                     progIdKey.SetValue(null, "Scrolling Game Development Kit 2 Project");
                     Microsoft.Win32.RegistryKey shellKey = progIdKey.CreateSubKey("shell");
                     try
                     {
                        Microsoft.Win32.RegistryKey editKey = shellKey.CreateSubKey("edit");
                        try
                        {
                           Microsoft.Win32.RegistryKey commandKey = editKey.CreateSubKey("command");
                           try
                           {
                              string exeFile = Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
                              commandKey.SetValue(null, "\"" + exeFile + "\" \"%1\"");
                           }
                           finally
                           {
                              commandKey.Close();
                           }
                        }
                        finally
                        {
                           editKey.Close();
                        }
                     }
                     finally
                     {
                        shellKey.Close();
                     }
                  }
                  finally
                  {
                     progIdKey.Close();
                  }
               }
               finally
               {
                  extKey.Close();
               }
            }
            else
               testKey.Close();
         }
         catch(System.Security.SecurityException)
         {
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(ex.Message, "Register File Association", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
      }

      private static int GetCommandLine(string[] args)
      {
         for (int idx=0; idx < args.Length; idx++)
         {
            if("-/".IndexOf(args[idx][0]) >= 0)
            {
               switch(args[idx].Substring(1))
               {
                  default:
                     CommandLineError(args[idx] + " is not a recognized command line switch");
                     return 1;
               }
            }
            else
            {
               if (g_CommandLine.ProjectFile != null)
               {
                  CommandLineError("Project file was specified more than once on command line");
                  return 2;
               }
               g_CommandLine.ProjectFile = args[idx];
            }
         }
         return 0;
      }

      private static void CommandLineError(string message)
      {
         MessageBox.Show(message, "Scrolling Game Development Kit 2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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