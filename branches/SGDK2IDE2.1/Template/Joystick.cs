using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

public partial class Joystick
{
   #region Embedded Types
   [Flags()]
   private enum JoystickFlags
   {
      JOY_RETURNX = 0x1,
      JOY_RETURNY = 0x2,
      JOY_RETURNZ = 0x4,
      JOY_RETURNR = 0x8,
      JOY_RETURNU = 0x10,
      JOY_RETURNV = 0x20,
      JOY_RETURNPOV = 0x40,
      JOY_RETURNBUTTONS = 0x80,
      JOY_RETURNALL = (JOY_RETURNX | JOY_RETURNY | JOY_RETURNZ | JOY_RETURNR | JOY_RETURNU | JOY_RETURNV | JOY_RETURNPOV | JOY_RETURNBUTTONS)
   }

   [StructLayout(LayoutKind.Sequential)]
   private struct JOYCAPS
   {
      public UInt16 wMid;
      public UInt16 wPid;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string szPname;
      public Int32 wXmin;
      public Int32 wXmax;
      public Int32 wYmin;
      public Int32 wYmax;
      public Int32 wZmin;
      public Int32 wZmax;
      public Int32 wNumButtons;
      public Int32 wPeriodMin;
      public Int32 wPeriodMax;
      public Int32 wRmin;
      public Int32 wRmax;
      public Int32 wUmin;
      public Int32 wUmax;
      public Int32 wVmin;
      public Int32 wVmax;
      public Int32 wCaps;
      public Int32 wMaxAxes;
      public Int32 wNumAxes;
      public Int32 wMaxButtons;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string szRegKey;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string szOEMVxD;
   }

   [StructLayout(LayoutKind.Sequential)]
   private struct JOYINFOEX
   {
      public UInt32 dwSize;
      [MarshalAs(UnmanagedType.I4)]
      public JoystickFlags dwFlags;
      public Int32 dwXpos;
      public Int32 dwYpos;
      public Int32 dwZpos;
      public Int32 dwRpos;
      public Int32 dwUpos;
      public Int32 dwVpos;
      public UInt32 dwButtons;
      public UInt32 dwButtonNumber;
      public Int32 dwPOV;
      public UInt32 dwReserved1;
      public UInt32 dwReserved2;
   }
   #endregion

   #region Private Members
   private int deviceNum;

   private JOYCAPS joyCaps;
   private JOYINFOEX joyInfo;
   #endregion

   [DllImport("Winmm.dll")]
   private static extern UInt32 joyGetDevCaps(Int32 uJoyID, out JOYCAPS pjc, Int32 cbjc);
   [DllImport("Winmm.dll")]
   private static extern UInt32 joyGetPosEx(Int32 uJoyID, out JOYINFOEX pji);

   /// <summary>
   /// Returns the number of joysticks available on the system.
   /// </summary>
   /// <returns>A number from 0 to 16</returns>
   public static int GetDeviceCount()
   {
      JOYCAPS joyCaps = new JOYCAPS();
      int count = 0;
      if ((Environment.OSVersion.Platform == PlatformID.Win32NT) ||
          (Environment.OSVersion.Platform == PlatformID.Win32Windows))
      {
         for (count = 0; count < 16; count++)
         {
            if (0 != joyGetDevCaps(count, out joyCaps, Marshal.SizeOf(joyCaps)))
               return count;
         }
      }
      return count;
   }

   /// <summary>
   /// Create an object which can be used to acces information about the specified
   /// joystick number.
   /// </summary>
   /// <param name="deviceNum">Number from 0 to 15 indicating a joystick number</param>
   public Joystick(int deviceNum)
   {
      this.deviceNum = deviceNum;
      if ((Environment.OSVersion.Platform == PlatformID.Win32NT) ||
          (Environment.OSVersion.Platform == PlatformID.Win32Windows))
      {
         if (0 != joyGetDevCaps(deviceNum, out joyCaps, Marshal.SizeOf(joyCaps)))
            throw new InvalidOperationException("Failed to access specified joystick");
      }
   }

   /// <summary>
   /// Read all data from the device associated with this Joystick into the
   /// Position, POVAngle and Button properties.
   /// </summary>
   /// <remarks>Buttons are retrieved by accessing the Joystick's indexer
   /// property</remarks>
   /// <example>
   /// if (myJoy[0])
   /// {
   ///    // button 0 is pressed
   /// }
   /// </example>
   public void Read()
   {
      joyInfo.dwSize = (UInt32)Marshal.SizeOf(joyInfo);
      joyInfo.dwFlags = JoystickFlags.JOY_RETURNALL;
      if ((Environment.OSVersion.Platform == PlatformID.Win32NT) ||
          (Environment.OSVersion.Platform == PlatformID.Win32Windows))
         joyGetPosEx(deviceNum, out joyInfo);
   }

   /// <summary>
   /// The position of the joystick's x-axis input during the last call to <see cref="Read"/>.
   /// </summary>
   public int XPosition
   {
      get
      {
         return joyInfo.dwXpos;
      }
   }

   /// <summary>
   /// The position of the joystick's y-axis input during the last call to <see cref="Read"/>.
   /// </summary>
   public int YPosition
   {
      get
      {
         return joyInfo.dwYpos;
      }
   }

   /// <summary>
   /// The position of the joystick's z-axis input during the last call to <see cref="Read"/>.
   /// </summary>
   public int ZPosition
   {
      get
      {
         return joyInfo.dwZpos;
      }
   }

   /// <summary>
   /// The position of the joystick's rudder input during the last call to <see cref="Read"/>.
   /// </summary>
   public int RPosition
   {
      get
      {
         return joyInfo.dwRpos;
      }
   }

   /// <summary>
   /// The position of the joystick's u-axis input during the last call to <see cref="Read"/>.
   /// </summary>
   public int UPosition
   {
      get
      {
         return joyInfo.dwUpos;
      }
   }

   /// <summary>
   /// The position of the joystick's v-axis input during the last call to <see cref="Read"/>.
   /// </summary>
   public int VPosition
   {
      get
      {
         return joyInfo.dwVpos;
      }
   }

   /// <summary>
   /// The position of the joystick's POV control during the last call to <see cref="Read"/>,
   /// represented as a number between 0 and 35900 in hundredths of degrees
   /// </summary>
   public int POVAngle
   {
      get
      {
         return joyInfo.dwPOV;
      }
   }

   /// <summary>
   /// Determines which buttons were pressed during the last call to <see cref="Read"/>.
   /// </summary>
   /// <param name="buttonNum">A number from 0 to 31 specifying a button number</param>
   /// <returns>True if the button is pressed, or False otherwise.</returns>
   public bool this[byte buttonNum]
   {
      get
      {
         if (buttonNum >= 32)
            throw new ArgumentException("Invalid button number", "buttonNum");
         return 0 != (joyInfo.dwButtons & (1 << buttonNum));
      }
   }

   /// <summary>
   /// Returns a string identifying the joystick in plain text
   /// </summary>
   public string Name
   {
      get
      {
         return joyCaps.szPname;
      }
   }

   /// <summary>
   /// Returns the minimum value of <see cref="XPosition"/>.
   /// </summary>
   public int MinimumX
   {
      get
      {
         return joyCaps.wXmin;
      }
   }

   /// <summary>
   /// Returns the maximum value of <see cref="XPosition"/>.
   /// </summary>
   public int MaximumX
   {
      get
      {
         return joyCaps.wXmax;
      }
   }

   /// <summary>
   /// Returns the minimum value of <see cref="YPosition"/>.
   /// </summary>
   public int MinimumY
   {
      get
      {
         return joyCaps.wYmin;
      }
   }

   /// <summary>
   /// Returns the maximum value of <see cref="YPosition"/>.
   /// </summary>
   public int MaximumY
   {
      get
      {
         return joyCaps.wYmax;
      }
   }

   /// <summary>
   /// Returns the minimum value of <see cref="ZPosition"/>.
   /// </summary>
   public int MinimumZ
   {
      get
      {
         return joyCaps.wZmin;
      }
   }

   /// <summary>
   /// Returns the maximum value of <see cref="ZPosition"/>.
   /// </summary>
   public int MaximumZ
   {
      get
      {
         return joyCaps.wZmax;
      }
   }

   /// <summary>
   /// Returns the minimum value of <see cref="RPosition"/>.
   /// </summary>
   public int MinimumR
   {
      get
      {
         return joyCaps.wRmin;
      }
   }

   /// <summary>
   /// Returns the maximum value of <see cref="RPosition"/>.
   /// </summary>
   public int MaximumR
   {
      get
      {
         return joyCaps.wRmax;
      }
   }

   /// <summary>
   /// Returns the minimum value of <see cref="UPosition"/>.
   /// </summary>
   public int MinimumU
   {
      get
      {
         return joyCaps.wUmin;
      }
   }

   /// <summary>
   /// Returns the maximum value of <see cref="UPosition"/>.
   /// </summary>
   public int MaximumU
   {
      get
      {
         return joyCaps.wUmax;
      }
   }

   /// <summary>
   /// Returns the minimum value of <see cref="VPosition"/>.
   /// </summary>
   public int MinimumV
   {
      get
      {
         return joyCaps.wVmin;
      }
   }

   /// <summary>
   /// Returns the maximum value of <see cref="VPosition"/>.
   /// </summary>
   public int MaximumV
   {
      get
      {
         return joyCaps.wVmax;
      }
   }
}
