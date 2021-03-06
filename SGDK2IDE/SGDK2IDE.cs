/*
 * Scrolling Game Development Kit 2.0
 *
 * Copyright � 2000 - 2007 Benjamin Marty <bluemonkmn@users.sourceforge.net>
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
         public bool LoadAsTemplate = false;

         public CommandLineArgs()
         {
         }
      }
      #endregion

      #region Non-Control Members
      public static ResourceManager g_Resources = null;
      public static CommandLineArgs g_CommandLine = new CommandLineArgs();
      public static System.Windows.Forms.HelpProvider g_HelpProvider;
      private static frmMain mainWindow = null;
      private static System.Collections.Stack statusStack = null;
      public const char pathSeparator = '\\';
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
         if ((args.Length == 0) 
            && (System.AppDomain.CurrentDomain.SetupInformation != null)
            && (System.AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null)
            && (System.AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null))
            args = System.AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
         int result = GetCommandLine(args);
         if (result != 0)
            return result;
         g_Resources = new ResourceManager("SGDK2.SGDK2IDE", Assembly.GetExecutingAssembly());
         CreateFileAssociation();
         frmSplashForm frmSp = new frmSplashForm(GetSplashImage());
         frmSp.Closed += new EventHandler(frmSplash_Closed);
         frmSp.Show();
         try
         {
            FindHelpFile();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Run(mainWindow = new frmMain());
         }
         catch(System.Exception ex)
         {
            FatalErrorHandler(ex);
         }
         CodeGenerator.ResetTempAssembly();
         return 0;
      }

      private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
      {
         FatalErrorHandler(e.ExceptionObject as Exception);
      }

      private static void FatalErrorHandler(Exception ex)
      {
         if (DialogResult.Yes == MessageBox.Show("An unexpected error occurred and will cause the application to terminate. Details:\r\n" + ex.ToString() + "\r\n\r\nI can, however, attempt to save your project before exiting. If you save the project, you will be prompted for a location to save -- you should probably not overwrite your original project under these circumstances. Would you like to try to save?", "Scrolling Game Development Kit 2 Unexpected Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1))
         {
            while (true)
            {
               try
               {
                  SaveFileDialog fd = new SaveFileDialog();
                  fd.CheckPathExists = true;
                  fd.OverwritePrompt = true;
                  fd.DefaultExt = "sgdk2";
                  fd.Filter = "SGDK2 Projects (*.sgdk2)|*.sgdk2|All Files (*.*)|*.*";
                  fd.FilterIndex = 1;
                  fd.Title = "Save Project";
                  fd.ValidateNames = true;
                  fd.ShowDialog();
                  // ShowDialog returns the incorrect result (Cancel) when trying to
                  // overwrite a file, so we have to check the filename instead.
                  if (fd.FileName.Length > 0)
                  {
                     ProjectData.WriteXml(fd.FileName);
                     MessageBox.Show("Success! Your project was saved to " + fd.FileName + ". You should verify that the project has not been corrupted before working with that project further. You should also consider reporting this error to the Scrolling Game Development Kit support site:\r\n" + ex.ToString(), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                     break;
                  }
                  else
                  {
                     if (DialogResult.Yes == MessageBox.Show("You cancelled your opportunity to save your project during a fatal error.  If you do not save it now, any changes you made will be lost.  Are you sure you want to exit without saving anything?", "Last Chance", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                        break;
                  }
               }
               catch (System.Exception ex2)
               {
                  if (DialogResult.No == MessageBox.Show("Well, that didn't work:\r\n" + ex2.ToString() + "\r\n\r\nDo you want to try again?", "Double Failure", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1))
                     break;
               }
            }
         }
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
         String sCopyright = "Copyright �2000-2017\r\nBenjamin Marty";
         Rectangle rcFmt = new Rectangle(40, 164, 130, 25);
         gc.DrawString(sCopyright, fnt, Brushes.Sienna, rcFmt, fmt);
         rcFmt.Offset(-1,-1);
         gc.DrawString(sCopyright, fnt, Brushes.White, rcFmt, fmt);
         fnt.Dispose();
         fnt = new Font("Tahoma", 8);
         string ver = Application.ProductVersion;
         rcFmt = new Rectangle(86, 151, 30, 10);
         gc.DrawString("v" + ver, fnt, Brushes.White, rcFmt, fmt);
         fmt.Dispose();
         fnt.Dispose();
         gc.Dispose();
         return bmp;
      }

      private static void CreateFileAssociation()
      {
         try
         {
            bool alreadyRegistered = false;
            Microsoft.Win32.RegistryKey testKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Classes\.sgdk2", false);
            if (testKey != null)
            {
               alreadyRegistered = true;
               testKey.Close();
               testKey.Dispose();
            }
            if (!alreadyRegistered)
            {
               using (Microsoft.Win32.RegistryKey classesKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Classes", true))
               {
                  using (Microsoft.Win32.RegistryKey extKey = classesKey.CreateSubKey(".sgdk2"))
                  {
                     using (Microsoft.Win32.RegistryKey progIdKey = classesKey.CreateSubKey("sgdk2file"))
                     {
                        extKey.SetValue(null, "sgdk2file");
                        progIdKey.SetValue(null, "Scrolling Game Development Kit 2 Project");
                        string exeFile = Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
                        string exeDir = System.IO.Path.GetDirectoryName(exeFile);
                        string prjIconFile = System.IO.Path.Combine(exeDir, "Prj.ico");
                        using (Microsoft.Win32.RegistryKey iconKey = progIdKey.CreateSubKey("DefaultIcon"))
                        {
                           iconKey.SetValue(null, "\"" + prjIconFile + "\"");
                        }
                        using (Microsoft.Win32.RegistryKey shellKey = progIdKey.CreateSubKey("shell"))
                        {
                           using (Microsoft.Win32.RegistryKey editKey = shellKey.CreateSubKey("edit"))
                           {
                              using (Microsoft.Win32.RegistryKey commandKey = editKey.CreateSubKey("command"))
                              {
                                 commandKey.SetValue(null, "\"" + exeFile + "\" \"%1\"");
                                 commandKey.Close();
                              }
                              editKey.Close();
                           }
                           shellKey.Close();
                        }
                        progIdKey.Close();
                     }
                     extKey.Close();
                  }
                  classesKey.Close();
               }
            }
         }
         catch (System.Security.SecurityException)
         {
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(ex.Message + "\r\nThis error is not critical, and will only prevent you from starting SGDK2 by opening a *.sgdk2 file. You may be able to work around it by running as administrator once.",
               "Register File Association", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                  case "t":
                     g_CommandLine.LoadAsTemplate = true;
                     break;
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
      public static void FindHelpFile()
      {
         g_HelpProvider = new System.Windows.Forms.HelpProvider();
         g_HelpProvider.HelpNamespace = System.IO.Path.Combine(Application.StartupPath, "SGDK2Help.chm");
         if (!System.IO.File.Exists(g_HelpProvider.HelpNamespace))
         {
            string helpParentDir = Application.StartupPath;
            do
            {
               if (System.IO.File.Exists(System.IO.Path.Combine(helpParentDir, @"Help\SGDK2Help.chm")))
               {
                  g_HelpProvider.HelpNamespace = System.IO.Path.Combine(helpParentDir, @"Help\SGDK2Help.chm");
                  break;
               }
               else if (System.IO.File.Exists(System.IO.Path.Combine(helpParentDir, @"Help\Compiled\SGDK2Help.chm")))
               {
                  g_HelpProvider.HelpNamespace = System.IO.Path.Combine(helpParentDir, @"Help\Compiled\SGDK2Help.chm");
                  break;
               }
               else
               {
                  System.IO.DirectoryInfo diHelp = System.IO.Directory.GetParent(helpParentDir);
                  if (diHelp == null)
                     helpParentDir = null;
                  else
                     helpParentDir = diHelp.FullName;
               }
            } while (helpParentDir != null);
         }
      }

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
      /// Copy an image like CopyImage, but apply normal mapping from normals in the process, assuming direct lighting.
      /// </summary>
      /// <param name="original">The image to be copied</param>
      /// <param name="output">Where the final result should be generated</param>
      /// <param name="normals">Normal map image</param>
      public static void ApplyFrontLitNormals(Bitmap output, Bitmap original, Bitmap normals)
      {
         if ((original.Size != normals.Size) || (original.Size != output.Size))
            throw new ArgumentException("Original, output and normal images must all be the same size.");
         Rectangle rectAllPixels = new Rectangle(Point.Empty, original.Size);
         System.Drawing.Imaging.BitmapData datOriginal =
            original.LockBits(rectAllPixels, System.Drawing.Imaging.ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
         System.Drawing.Imaging.BitmapData datNormals =
            normals.LockBits(rectAllPixels, System.Drawing.Imaging.ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
         System.Drawing.Imaging.BitmapData datOutput =
            output.LockBits(rectAllPixels, System.Drawing.Imaging.ImageLockMode.WriteOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
         Int32[] texturePixels = new Int32[Math.Abs(datOriginal.Stride) * datOriginal.Height / 4];
         Int32[] normalPixels = new Int32[Math.Abs(datNormals.Stride) * datNormals.Height / 4];
         System.Runtime.InteropServices.Marshal.Copy(datOriginal.Scan0, texturePixels, 0, texturePixels.Length);
         System.Runtime.InteropServices.Marshal.Copy(datNormals.Scan0, normalPixels, 0, normalPixels.Length);
         int nPixelStride = Math.Abs(datOriginal.Stride) / 4;
         int nNormalStride = Math.Abs(datNormals.Stride) / 4;
         for (int y = 0; y < original.Height; y++)
         {
            for (int x = 0; x < original.Width; x++)
            {
               Color pixel = Color.FromArgb(texturePixels[y * nPixelStride + x]);
               Color normal = Color.FromArgb(normalPixels[y * nNormalStride + x]);
               int colorScale = (normal.A * normal.B + (255 - normal.A) * 255) / 255;
               texturePixels[y * nPixelStride + x] = Color.FromArgb(
                  pixel.A,
                  pixel.R * colorScale / 255,
                  pixel.G * colorScale / 255,
                  pixel.B * colorScale / 255).ToArgb();
            }
         }
         System.Runtime.InteropServices.Marshal.Copy(texturePixels, 0, datOutput.Scan0, texturePixels.Length);
         normals.UnlockBits(datNormals);
         original.UnlockBits(datOriginal);
         output.UnlockBits(datOutput);
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

      public static string CurrentProjectFile
      {
         get
         {
            return mainWindow.ProjectFile;
         }
      }
      public static void PushStatus(string status, bool waitCursor)
      {
         if (statusStack == null)
         {
            statusStack = new System.Collections.Stack();
         }
         mainWindow.sbMain.Text = status;
         statusStack.Push(status);
         if (waitCursor)
            mainWindow.Cursor = Cursors.WaitCursor;
      }

      public static void PopStatus()
      {
         if (statusStack.Count > 0)
            statusStack.Pop();
         if (statusStack.Count > 0)
            mainWindow.sbMain.Text = (string)statusStack.Peek();
         if (statusStack.Count <= 1)
            mainWindow.Cursor = Cursors.Default;
      }

      public static string TopStatusMessage
      {
         get
         {
            return (string)statusStack.Peek();
         }
      }

      public static System.Xml.XmlDocument LoadUserSettings()
      {
         System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
         string prefsFile = System.IO.Path.Combine(Application.UserAppDataPath, "Settings.xml");
         bool success = false;
         try
         {
            doc.Load(prefsFile);
            Int32.Parse(((System.Xml.XmlElement)doc.SelectSingleNode("/SGDK2Prefs/MRUList")).GetAttribute("maxCount"));
            if ((doc.SelectSingleNode("/SGDK2Prefs/Windows") is System.Xml.XmlElement) &&
               (doc.SelectSingleNode("/SGDK2Prefs/UserOptions") is System.Xml.XmlElement))
               success = true;               
            else
               success = false;
         }
         catch (System.Exception)
         {
            success = false;
         }
         if (!success)
         {
            doc = new System.Xml.XmlDocument();
            System.Xml.XmlElement sgdk2Prefs = doc.CreateElement("SGDK2Prefs");
            doc.AppendChild(sgdk2Prefs);
            System.Xml.XmlElement mruList = doc.CreateElement("MRUList");
            mruList.SetAttribute("maxCount", "5");
            sgdk2Prefs.AppendChild(mruList);
            sgdk2Prefs.AppendChild(doc.CreateElement("Windows"));
            sgdk2Prefs.AppendChild(doc.CreateElement("UserOptions"));
         }
         return doc;
      }

      public static void SaveUserSettings(System.Xml.XmlDocument doc)
      {
         string prefsFile = System.IO.Path.Combine(Application.UserAppDataPath, "Settings.xml");
         doc.Save(prefsFile);
      }

      public static void LoadFormSettings(Form form)
      {
         try
         {
            System.Xml.XmlDocument doc = LoadUserSettings();
            System.Xml.XmlElement settings = doc.SelectSingleNode("//Windows/" + form.GetType().Name) as System.Xml.XmlElement;
            if (settings == null)
               return;
            if (settings.HasAttribute("Left"))
               form.Location = new Point(Int32.Parse(settings.GetAttribute("Left")), Int32.Parse(settings.GetAttribute("Top")));
            if ((form.FormBorderStyle == FormBorderStyle.Sizable) && settings.HasAttribute("Width"))
               form.ClientSize = new Size(Int32.Parse(settings.GetAttribute("Width")), Int32.Parse(settings.GetAttribute("Height")));
            if (form.MaximizeBox && settings.HasAttribute("FormWindowState"))
               form.WindowState = (FormWindowState)System.Enum.Parse(typeof(FormWindowState), settings.GetAttribute("FormWindowState"));
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(System.Windows.Forms.Form.ActiveForm, ex.Message, "Error Reading User Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      public static void SaveFormSettings(Form form)
      {
         System.Xml.XmlDocument doc = LoadUserSettings();
         System.Xml.XmlElement settings = doc.SelectSingleNode("//Windows/" + form.GetType().Name) as System.Xml.XmlElement;
         if (settings == null)
         {
            settings = doc.CreateElement(form.GetType().Name);
            doc.SelectSingleNode("//Windows").AppendChild(settings);
         }
         if (form.MaximizeBox && (form.WindowState!=FormWindowState.Minimized))
         {
            settings.SetAttribute("FormWindowState", form.WindowState.ToString());
         }
         if (form.WindowState == FormWindowState.Normal)
         {
            settings.SetAttribute("Left", form.Left.ToString());
            settings.SetAttribute("Top", form.Top.ToString());
            if (form.FormBorderStyle == FormBorderStyle.Sizable)
            {
               settings.SetAttribute("Width", form.ClientSize.Width.ToString());
               settings.SetAttribute("Height", form.ClientSize.Height.ToString());
            }
         }
         SaveUserSettings(doc);
      }
      
      public static string GetUserOption(string name)
      {
         System.Xml.XmlDocument doc = LoadUserSettings();
         System.Xml.XmlElement option = doc.SelectSingleNode("//UserOptions/" + name) as System.Xml.XmlElement;
         if (option != null)
            return option.InnerText;
         return null;
      }

      public static void SetUserOption(string name, string value)
      {
         System.Xml.XmlDocument doc = LoadUserSettings();
         System.Xml.XmlElement option = doc.SelectSingleNode("//UserOptions/" + name) as System.Xml.XmlElement;
         if (option != null)
            option.InnerText = value;
         else
         {
            System.Xml.XmlElement elem = doc.CreateElement(name);
            elem.InnerText = value;
            doc.SelectSingleNode("//UserOptions").AppendChild(elem);
         }
         SaveUserSettings(doc);
      }

      public static void ResetUserOptions()
      {
         System.Xml.XmlDocument doc = LoadUserSettings();
         System.Xml.XmlElement option = doc.SelectSingleNode("//UserOptions") as System.Xml.XmlElement;
         option.RemoveAll();
         SaveUserSettings(doc);
      }

      public static System.Drawing.RectangleF GetRotatedBounds(int CellWidth, int CellHeight, System.Drawing.Drawing2D.Matrix m)
      {
         RectangleF bounds;
         SizeF CellSize = new SizeF(CellWidth, CellHeight);
         PointF[] ptsRect = new PointF[]
            {
               new PointF(0, 0),
               new PointF(CellSize.Width, 0),
               new PointF(CellSize.Width, CellSize.Height),
               new PointF(0, CellSize.Height)
            };
         m.TransformPoints(ptsRect);
         bounds = new RectangleF(ptsRect[0], new SizeF(0,0));
         foreach (PointF pt in ptsRect)
         {
            if(pt.X < bounds.X)
            {
               bounds.Width += bounds.X - pt.X;
               bounds.X = pt.X;
            }
            if(pt.Y < bounds.Y)
            {
               bounds.Height += bounds.Y - pt.Y;
               bounds.Y = pt.Y;
            }
            if (pt.X > bounds.Right)
               bounds.Width += pt.X - bounds.Right;
            if (pt.Y > bounds.Bottom)
               bounds.Height += pt.Y - bounds.Bottom;
         }
         return bounds;
      }
      #endregion

      private static void frmSplash_Closed(object sender, EventArgs e)
      {
         mainWindow.tmrInitComplete.Enabled = true;
      }
   }
}