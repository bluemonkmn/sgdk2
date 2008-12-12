using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

/// <summary>
/// Identifies a key on the keyboard.
/// </summary>
public enum Key
{
   None = 0,
   Backspace = 0x08,
   Tab = 0x09,
   Enter = 0x0D,
   Pause = 0x13,
   CapsLock = 0x14,
   Escape = 0x1B,
   Space = 0x20,
   PageUp = 0x21,
   PageDown = 0x22,
   End = 0x23,
   Home = 0x24,
   Left = 0x25,
   Up = 0x26,
   Right = 0x27,
   Down = 0x28,
   PrintScreen = 0x2C,
   Insert = 0x2D,
   Delete = 0x2E,
   Digit0 = 0x30,
   Digit1 = 0x31,
   Digit2 = 0x32,
   Digit3 = 0x33,
   Digit4 = 0x34,
   Digit5 = 0x35,
   Digit6 = 0x36,
   Digit7 = 0x37,
   Digit8 = 0x38,
   Digit9 = 0x39,
   A = 0x41,
   B = 0x42,
   C = 0x43,
   D = 0x44,
   E = 0x45,
   F = 0x46,
   G = 0x47,
   H = 0x48,
   I = 0x49,
   J = 0x4A,
   K = 0x4B,
   L = 0x4C,
   M = 0x4D,
   N = 0X4E,
   O = 0X4F,
   P = 0X50,
   Q = 0X51,
   R = 0X52,
   S = 0X53,
   T = 0X54,
   U = 0X55,
   V = 0X56,
   W = 0X57,
   X = 0X58,
   Y = 0X59,
   Z = 0X5A,
   LWindow = 0x5B,
   RWindow = 0x5C,
   ContextMenu = 0x5D,
   NumPad0 = 0x60,
   NumPad1 = 0x61,
   NumPad2 = 0x62,
   NumPad3 = 0x63,
   NumPad4 = 0x64,
   NumPad5 = 0x65,
   NumPad6 = 0x66,
   NumPad7 = 0x67,
   NumPad8 = 0x68,
   NumPad9 = 0x69,
   NumPadMultiply = 0x6A,
   NumPadAdd = 0x6B,
   NumPadEnter = 0x6C,
   NumPadSubtract = 0x6D,
   NumPadDecimal = 0x6E,
   NumPadDivide = 0x6F,
   F1 = 0x70,
   F2 = 0x71,
   F3 = 0x72,
   F4 = 0x73,
   F5 = 0x74,
   F6 = 0x75,
   F7 = 0x76,
   F8 = 0x77,
   F9 = 0x78,
   F10 = 0x79,
   F11 = 0x7A,
   F12 = 0x7B,
   NumLock = 0x90,
   ScrollLock = 0x91,
   LShift = 0xA0,
   RShift = 0xA1,
   LControl = 0xA2,
   RControl = 0xA3,
   LAlt = 0xA4,
   RAlt = 0xA5,
   SemiColon = 0xBA,
   Equal = 0xBB,
   Comma = 0xBC,
   Minus = 0xBD,
   Period = 0xBE,
   Slash = 0xBF,
   Backtick = 0xC0,
   LeftBracket = 0xDB,
   BackSlash = 0xDC,
   RightBracket = 0xDD,
   Quote = 0xDE,
}

/// <summary>
/// Represents the current state of all keys on the keyboard.
/// </summary>
public partial class KeyboardState
{
   [DllImport("user32.dll")]
   private static extern bool GetKeyboardState(byte[] lpKeyState);
   private const int WM_KEYDOWN = 0x100;
   private const int WM_KEYUP = 0x101;

   private System.Collections.BitArray keyStates = new System.Collections.BitArray(256,false);

   /// <summary>
   /// Gets or sets the state of the specified key on the keyboard.
   /// </summary>
   /// <param name="key">Which key is affected.</param>
   /// <returns>True if the key is pressed of false if it is not pressed.</returns>
   public bool this[Key key]
   {
      get
      {
         return keyStates[(int)key];
      }
      set
      {
         keyStates[(int)key] = value;
      }
   }

   /// <summary>
   /// This function is called by the environment and processes key events to track the state
   /// of all the keys on the keyboard.
   /// </summary>
   /// <param name="m">Windows API message</param>
   public void ProcessMessage(Message m)
   {
      switch (m.Msg)
      {
         case WM_KEYDOWN:
            keyStates[m.WParam.ToInt32()] = true;
            keyStates[GetExtraKey(m)] = true;
            break;
         case WM_KEYUP:
            keyStates[m.WParam.ToInt32()] = false;
            keyStates[GetExtraKey(m)] = false;
            break;
      }
   }

   private int GetExtraKey(Message m)
   {
      switch (m.WParam.ToInt32())
      {
         case 0x10:
            if ((m.LParam.ToInt32() & 0xFF0000) == 0x2A0000)
               return (int)Key.LShift;
            else
               return (int)Key.RShift;
            break;
         case 0x11:
            if ((m.LParam.ToInt32() & 0x1000000) != 0)
               return (int)Key.RControl;
            else
               return (int)Key.LControl;
            break;
         case 0x0d:
            if ((m.LParam.ToInt32() & 0x1000000) != 0)
               return (int)Key.NumPadEnter;
            else
               return (int)Key.Enter;
      }
      return m.WParam.ToInt32();
   }

   /// <summary>
   /// Get the currently pressed key.  If more than one key is pressed, get the first one.
   /// </summary>
   /// <returns><see cref="Key.None"/> is returned if no key is pressed.</returns>
   public Key GetFirstKey()
   {
      foreach (Key specificKey in new Key[] {
         Key.LControl, Key.RControl, Key.LShift, Key.RShift,
         Key.NumPadEnter})
         if (this[specificKey])
            return specificKey;

      foreach (Key k in System.Enum.GetValues(typeof(Key)))
         if (this[k]) return k;
      return Key.None;
   }
}
