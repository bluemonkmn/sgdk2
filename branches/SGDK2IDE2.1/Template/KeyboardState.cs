using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public interface IKeyboard
{
   void Poll();
   bool IsKeyDown(Key key);
   Key GetFirstKey();
}

/// <summary>
/// Identifies a key on the keyboard.
/// </summary>
public enum Key
{
   None = 0,
   Backspace,
   Tab,
   Enter,
   Pause,
   CapsLock,
   Escape,
   Space,
   PageUp,
   PageDown,
   End,
   Home,
   Left,
   Up,
   Right,
   Down,
   PrintScreen,
   Insert,
   Delete,
   Digit0,
   Digit1,
   Digit2,
   Digit3,
   Digit4,
   Digit5,
   Digit6,
   Digit7,
   Digit8,
   Digit9,
   A,
   B,
   C,
   D,
   E,
   F,
   G,
   H,
   I,
   J,
   K,
   L,
   M,
   N,
   O,
   P,
   Q,
   R,
   S,
   T,
   U,
   V,
   W,
   X,
   Y,
   Z,
   LWindow,
   RWindow,
   ContextMenu,
   NumPad0,
   NumPad1,
   NumPad2,
   NumPad3,
   NumPad4,
   NumPad5,
   NumPad6,
   NumPad7,
   NumPad8,
   NumPad9,
   NumPadMultiply,
   NumPadAdd,
   NumPadEnter,
   NumPadSubtract,
   NumPadDecimal,
   NumPadDivide,
   F1,
   F2,
   F3,
   F4,
   F5,
   F6,
   F7,
   F8,
   F9,
   F10,
   F11,
   F12,
   NumLock,
   ScrollLock,
   LShift,
   RShift,
   LControl,
   RControl,
   LAlt,
   RAlt,
   SemiColon,
   Equal,
   Comma,
   Minus,
   Period,
   Slash,
   Backtick,
   LeftBracket,
   BackSlash,
   RightBracket,
   Quote
}

public partial class KeyboardState : IKeyboard
{
   IKeyboard kbImpl;
   
   public KeyboardState()
   {
      if (Environment.OSVersion.Platform == PlatformID.Unix)
         kbImpl = new XKeyboard();
      else
         kbImpl = new WinKeyboard();
   }
   
   public bool IsKeyDown(Key key)
   {
      return kbImpl.IsKeyDown(key);
   }
   
   public Key GetFirstKey()
   {
      return kbImpl.GetFirstKey();
   }
   
   public void Poll()
   {
      kbImpl.Poll();
   }
   
   public bool this[Key key]
   {
      get
      {
         return IsKeyDown(key);
      }
   }
}

/// <summary>
/// Represents the current state of all keys on the keyboard.
/// </summary>
public partial class WinKeyboard : IKeyboard
{
   private byte[] keyMap = new byte[] {
      /*None =*/ 0,
      /*Backspace =*/ 0x08,
      /*Tab = */0x09,
      /*Enter = */0x0D,
      /*Pause = */0x13,
      /*CapsLock = */0x14,
      /*Escape = */0x1B,
      /*Space = */0x20,
      /*PageUp = */0x21,
      /*PageDown = */0x22,
      /*End = */0x23,
      /*Home = */0x24,
      /*Left = */0x25,
      /*Up = */0x26,
      /*Right = */0x27,
      /*Down = */0x28,
      /*PrintScreen = */0x2C,
      /*Insert = */0x2D,
      /*Delete = */0x2E,
      /*Digit0 = */0x30,
      /*Digit1 = */0x31,
      /*Digit2 = */0x32,
      /*Digit3 = */0x33,
      /*Digit4 = */0x34,
      /*Digit5 = */0x35,
      /*Digit6 = */0x36,
      /*Digit7 = */0x37,
      /*Digit8 = */0x38,
      /*Digit9 = */0x39,
      /*A = */0x41,
      /*B = */0x42,
      /*C = */0x43,
      /*D = */0x44,
      /*E = */0x45,
      /*F = */0x46,
      /*G = */0x47,
      /*H = */0x48,
      /*I = */0x49,
      /*J = */0x4A,
      /*K = */0x4B,
      /*L = */0x4C,
      /*M = */0x4D,
      /*N = */0X4E,
      /*O = */0X4F,
      /*P = */0X50,
      /*Q = */0X51,
      /*R = */0X52,
      /*S = */0X53,
      /*T = */0X54,
      /*U = */0X55,
      /*V = */0X56,
      /*W = */0X57,
      /*X = */0X58,
      /*Y = */0X59,
      /*Z = */0X5A,
      /*LWindow = */0x5B,
      /*RWindow = */0x5C,
      /*ContextMenu = */0x5D,
      /*NumPad0 = */0x60,
      /*NumPad1 = */0x61,
      /*NumPad2 = */0x62,
      /*NumPad3 = */0x63,
      /*NumPad4 = */0x64,
      /*NumPad5 = */0x65,
      /*NumPad6 = */0x66,
      /*NumPad7 = */0x67,
      /*NumPad8 = */0x68,
      /*NumPad9 = */0x69,
      /*NumPadMultiply = */0x6A,
      /*NumPadAdd = */0x6B,
      /*NumPadEnter = */0x6C,
      /*NumPadSubtract = */0x6D,
      /*NumPadDecimal = */0x6E,
      /*NumPadDivide = */0x6F,
      /*F1 = */0x70,
      /*F2 = */0x71,
      /*F3 = */0x72,
      /*F4 = */0x73,
      /*F5 = */0x74,
      /*F6 = */0x75,
      /*F7 = */0x76,
      /*F8 = */0x77,
      /*F9 = */0x78,
      /*F10 = */0x79,
      /*F11 = */0x7A,
      /*F12 = */0x7B,
      /*NumLock = */0x90,
      /*ScrollLock = */0x91,
      /*LShift = */0xA0,
      /*RShift = */0xA1,
      /*LControl = */0xA2,
      /*RControl = */0xA3,
      /*LAlt = */0xA4,
      /*RAlt = */0xA5,
      /*SemiColon = */0xBA,
      /*Equal = */0xBB,
      /*Comma = */0xBC,
      /*Minus = */0xBD,
      /*Period = */0xBE,
      /*Slash = */0xBF,
      /*Backtick = */0xC0,
      /*LeftBracket = */0xDB,
      /*BackSlash = */0xDC,
      /*RightBracket = */0xDD,
      /*Quote = */0xDE
   };

   [DllImport("user32.dll")]
   private static extern bool GetKeyboardState(byte[] lpKeyState);

   private byte[] keyStates = new byte[256];

   /// <summary>
   /// Gets or sets the state of the specified key on the keyboard.
   /// </summary>
   /// <param name="key">Which key is affected.</param>
   /// <returns>True if the key is pressed of false if it is not pressed.</returns>
   public bool this[Key key]
   {
      get
      {
         return IsKeyDown(key);
      }
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
   
   public bool IsKeyDown(Key key)
   {
      return 0 != (keyStates[keyMap[(int)key]] & 0x80);
   }
   
   public void Poll()
   {
      GetKeyboardState(keyStates);
   }
}

class XKeyboard : IKeyboard
{
   private enum KeySym
   {
      None = 0,
      Space = 0x20,
      Quote = 0x27,
      Comma = 0x2c,
      Minus = 0x2d,
      Period = 0x2e,
      Slash = 0x2f,
      Digit0 = 0x30,
      Digit1,
      Digit2,
      Digit3,
      Digit4,
      Digit5,
      Digit6,
      Digit7,
      Digit8,
      Digit9,
      SemiColon = 0x3b,
      Equal = 0x3d,
      A = 0x41,
      B,
      C,
      D,
      E,
      F,
      G,
      H,
      I,
      J,
      K,
      L,
      M,
      N,
      O,
      P,
      Q,
      R,
      S,
      T,
      U,
      V,
      W,
      X,
      Y,
      Z,
      LeftBracket = 0x5b,
      Backslash,
      RightBracket,
      Grave = 0x60,
      PrintScreen = 0xfd1d,
      Backspace = 0xff08,
      Tab = 0xff09,
      Enter = 0xff0d,
      Pause = 0xff13,
      ScrollLock = 0xff14,
      SysRq = 0xff15,
      Escape = 0xff1b,
      Home = 0xff50,
      Left,
      Up,
      Right,
      Down,
      PageUp,
      PageDown,
      End,
      Insert = 0xff63,
      Menu = 0xff67,
      Break = 0xff6b,
      NumLock = 0xff7f,
      KeyPadEnter = 0xff8d,
      KeyPadAsterisk = 0xffaa,
      KeyPadPlus = 0xffab,
      KeyPadMinus = 0xffad,
      KeyPadDot = 0xffae,
      KeyPadSlash = 0xffaf,
      KeyPad0 = 0xffb0,
      KeyPad1,
      KeyPad2,
      KeyPad3,
      KeyPad4,
      KeyPad5,
      KeyPad6,
      KeyPad7,
      KeyPad8,
      KeyPad9,
      F1 = 0xffbe,
      F2,
      F3,
      F4,
      F5,
      F6,
      F7,
      F8,
      F9,
      F10,
      F11,
      F12,
      LeftShift = 0xffe1,
      RightShift = 0xffe2,
      LeftCtrl = 0xffe3,
      RightCtrl = 0xffe4,
      CapsLock = 0xffe5,
      LeftAlt = 0xffe9,
      RightAlt = 0xffea,
      LeftSuper = 0xffeb, // aka Left Windows Key
      RightSuper = 0xffec, // aka Right Windows Key
      Delete = 0xffff
   }

   private readonly KeySym[] keyMap = new KeySym[] {
   /*None =*/ KeySym.None,
   /*Backspace =*/ KeySym.Backspace,
   /*Tab = */KeySym.Tab,
   /*Enter = */KeySym.Enter,
   /*Pause = */KeySym.Pause,
   /*CapsLock = */KeySym.CapsLock,
   /*Escape = */KeySym.Escape,
   /*Space = */KeySym.Space,
   /*PageUp = */KeySym.PageUp,
   /*PageDown = */KeySym.PageDown,
   /*End = */KeySym.End,
   /*Home = */KeySym.Home,
   /*Left = */KeySym.Left,
   /*Up = */KeySym.Up,
   /*Right = */KeySym.Right,
   /*Down = */KeySym.Down,
   /*PrintScreen = */KeySym.PrintScreen,
   /*Insert = */KeySym.Insert,
   /*Delete = */KeySym.Delete,
   /*Digit0 = */KeySym.Digit0,
   /*Digit1 = */KeySym.Digit1,
   /*Digit2 = */KeySym.Digit2,
   /*Digit3 = */KeySym.Digit3,
   /*Digit4 = */KeySym.Digit4,
   /*Digit5 = */KeySym.Digit5,
   /*Digit6 = */KeySym.Digit6,
   /*Digit7 = */KeySym.Digit7,
   /*Digit8 = */KeySym.Digit8,
   /*Digit9 = */KeySym.Digit9,
   /*A = */KeySym.A,
   /*B = */KeySym.B,
   /*C = */KeySym.C,
   /*D = */KeySym.D,
   /*E = */KeySym.E,
   /*F = */KeySym.F,
   /*G = */KeySym.G,
   /*H = */KeySym.H,
   /*I = */KeySym.I,
   /*J = */KeySym.J,
   /*K = */KeySym.K,
   /*L = */KeySym.L,
   /*M = */KeySym.M,
   /*N = */KeySym.N,
   /*O = */KeySym.O,
   /*P = */KeySym.P,
   /*Q = */KeySym.Q,
   /*R = */KeySym.R,
   /*S = */KeySym.S,
   /*T = */KeySym.T,
   /*U = */KeySym.U,
   /*V = */KeySym.V,
   /*W = */KeySym.W,
   /*X = */KeySym.X,
   /*Y = */KeySym.Y,
   /*Z = */KeySym.Z,
   /*LWindow = */KeySym.LeftSuper,
   /*RWindow = */KeySym.RightSuper,
   /*ContextMenu = */KeySym.Menu,
   /*NumPad0 = */KeySym.KeyPad0,
   /*NumPad1 = */KeySym.KeyPad1,
   /*NumPad2 = */KeySym.KeyPad2,
   /*NumPad3 = */KeySym.KeyPad3,
   /*NumPad4 = */KeySym.KeyPad4,
   /*NumPad5 = */KeySym.KeyPad5,
   /*NumPad6 = */KeySym.KeyPad6,
   /*NumPad7 = */KeySym.KeyPad7,
   /*NumPad8 = */KeySym.KeyPad8,
   /*NumPad9 = */KeySym.KeyPad9,
   /*NumPadMultiply = */KeySym.KeyPadAsterisk,
   /*NumPadAdd = */KeySym.KeyPadPlus,
   /*NumPadEnter = */KeySym.KeyPadEnter,
   /*NumPadSubtract = */KeySym.KeyPadMinus,
   /*NumPadDecimal = */KeySym.KeyPadDot,
   /*NumPadDivide = */KeySym.KeyPadSlash,
   /*F1 = */KeySym.F1,
   /*F2 = */KeySym.F2,
   /*F3 = */KeySym.F3,
   /*F4 = */KeySym.F4,
   /*F5 = */KeySym.F5,
   /*F6 = */KeySym.F6,
   /*F7 = */KeySym.F7,
   /*F8 = */KeySym.F8,
   /*F9 = */KeySym.F9,
   /*F10 = */KeySym.F10,
   /*F11 = */KeySym.F11,
   /*F12 = */KeySym.F12,
   /*NumLock = */KeySym.NumLock,
   /*ScrollLock = */KeySym.ScrollLock,
   /*LShift = */KeySym.LeftShift,
   /*RShift = */KeySym.RightShift,
   /*LControl = */KeySym.LeftCtrl,
   /*RControl = */KeySym.RightCtrl,
   /*LAlt = */KeySym.LeftAlt,
   /*RAlt = */KeySym.RightAlt,
   /*SemiColon = */KeySym.SemiColon,
   /*Equal = */KeySym.Equal,
   /*Comma = */KeySym.Comma,
   /*Minus = */KeySym.Minus,
   /*Period = */KeySym.Period,
   /*Slash = */KeySym.Slash,
   /*Backtick = */KeySym.Grave,
   /*LeftBracket = */KeySym.LeftBracket,
   /*BackSlash = */KeySym.Backslash,
   /*RightBracket = */KeySym.RightBracket,
   /*Quote = */KeySym.Quote
   };
   [DllImport("libX11")]
   private static extern IntPtr XOpenDisplay(string display_name);
   [DllImport("libX11")]
   private static extern void XQueryKeymap(IntPtr display, System.UInt32[] keys);
   [DllImport("libX11")]
   private static extern IntPtr XGetKeyboardMapping(IntPtr display, byte first_keycode, int keycode_count, out int keysyms_per_keycode);
   [DllImport("libX11")]
   private static extern void XDisplayKeycodes(IntPtr display, out int min_keycodes, out int max_keycodes);
   [DllImport("libX11")]
   private static extern IntPtr XFree(IntPtr data);

   private System.Collections.Generic.Dictionary<KeySym, byte> symbolMap;
   private KeySym[] revSymbolMap;
   private System.Collections.Generic.Dictionary<KeySym, Key> revKeyMap;
   private IntPtr display;
   private System.UInt32[] currentKeys = new System.UInt32[8];

   public XKeyboard()
   {
      display = XOpenDisplay(null);
      GetKeyboardMap();
   }

   private void GetKeyboardMap()
   {
      int minkey, maxkey;
      int keysyms_per_keycode;
      XDisplayKeycodes(display, out minkey, out maxkey);
      int count = maxkey - minkey + 1;
      IntPtr kmap_ptr = XGetKeyboardMapping(display, (byte)minkey, count, out keysyms_per_keycode);
      int[] kmap = new int[keysyms_per_keycode * count];
      System.Runtime.InteropServices.Marshal.Copy(kmap_ptr, kmap, 0, count * keysyms_per_keycode);
      XFree(kmap_ptr);

      symbolMap = new System.Collections.Generic.Dictionary<KeySym, byte>();
      revSymbolMap = new KeySym[256];
      for (int i = 0; i < count * keysyms_per_keycode; i++)
      {
         byte keyCode = (byte)(i / keysyms_per_keycode + minkey);
         int keySym = kmap[i];
         if (keySym == 0)
            continue;
         if ((System.Enum.IsDefined(typeof(KeySym), keySym)))
         {
            KeySym symbol = (KeySym)keySym;
            if (!symbolMap.ContainsKey(symbol))
               symbolMap[symbol] = keyCode;
            if (revSymbolMap[keyCode] == 0)
               revSymbolMap[keyCode] = symbol;
         }
      }
      foreach (KeySym sym in Enum.GetValues(typeof(KeySym)))
         if (!symbolMap.ContainsKey(sym))
            symbolMap[sym] = 0;
      revKeyMap = new System.Collections.Generic.Dictionary<KeySym, Key>();
      foreach (Key k in Enum.GetValues(typeof(Key)))
      {
         revKeyMap[keyMap[(int)k]] = k;
      }
   }

   public void Poll()
   {
      XQueryKeymap(display, currentKeys);
   }

   public bool IsKeyDown(Key key)
   {
      return this[keyMap[(int)key]];
   }

   public bool this[Key key]
   {
      get
      {
         return IsKeyDown(key);
      }
   }

   private bool this[KeySym key]
   {
      get
      {
         byte keyCode = symbolMap[key];
         return 0 != ((currentKeys[(keyCode >> 5)] >> (keyCode % 32)) & 1);
      }
   }

   public Key GetFirstKey()
   {
      for (int i = 0; i < currentKeys.Length; i++)
      {
         if (currentKeys[i] != 0)
         {
            uint val = currentKeys[i];
            int bit;
            for (bit = 0; 0 == ((val >> bit) & 1); bit++)
               ;
            return revKeyMap[revSymbolMap[(byte)((i << 5) | bit)]];
         }
      }
      return Key.None;
   }
}