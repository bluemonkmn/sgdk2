/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */
using System;

/// <summary>
/// Defines a common interface by which a sprite can receive input from a player
/// or some object simulating a player.
/// </summary>
public partial interface IPlayer
{
   /// <summary>
   /// Returns true when the player is pressing up, or false otherwise.
   /// </summary>
   bool Up
   {
      get;
   }

   /// <summary>
   /// Returns true when the player is pressing left, or false otherwise.
   /// </summary>
   bool Left
   {
      get;
   }

   /// <summary>
   /// Returns true when the player is pressing right, or false otherwise.
   /// </summary>
   bool Right
   {
      get;
   }

   /// <summary>
   /// Returns true when the player is pressing down, or false otherwise.
   /// </summary>
   bool Down
   {
      get;
   }

   /// <summary>
   /// Returns true when the player is pressing button 1, or false otherwise.
   /// </summary>
   /// <remarks>The term "button 1" simply refers to one of 4 customizable inputs
   /// on a sprite. There is no pre-defined meaning to the buttons.</remarks>
   bool Button1
   {
      get;
   }

   /// <summary>
   /// Returns true when the player is pressing button 2, or false otherwise.
   /// </summary>
   /// <remarks>The term "button 2" simply refers to one of 4 customizable inputs
   /// on a sprite. There is no pre-defined meaning to the buttons.</remarks>
   bool Button2
   {
      get;
   }

   /// <summary>
   /// Returns true when the player is pressing button 3, or false otherwise.
   /// </summary>
   /// <remarks>The term "button 3" simply refers to one of 4 customizable inputs
   /// on a sprite. There is no pre-defined meaning to the buttons.</remarks>
   bool Button3
   {
      get;
   }

   /// <summary>
   /// Returns true when the player is pressing button 4, or false otherwise.
   /// </summary>
   /// <remarks>The term "button 4" simply refers to one of 4 customizable inputs
   /// on a sprite. There is no pre-defined meaning to the buttons.</remarks>
   bool Button4
   {
      get;
   }
}

/// <summary>
/// Represents interactions between the game and a player via the keyboard
/// </summary>
[Serializable()]
public partial class KeyboardPlayer : IPlayer
{
   /// <summary>
   /// Specifies which keyboard key maps to the notion of pressing left.
   /// </summary>
   public Key key_left;
   /// <summary>
   /// Specifies which keyboard key maps to the notion of pressing up.
   /// </summary>
   public Key key_up;
   /// <summary>
   /// Specifies which keyboard key maps to the notion of pressing right.
   /// </summary>
   public Key key_right;
   /// <summary>
   /// Specifies which keyboard key maps to the notion of pressing down.
   /// </summary>
   public Key key_down;
   /// <summary>
   /// Specifies which keyboard key maps to button 1.
   /// </summary>
   /// <remarks>The term "button 1" simply refers to 1 of 4 customizable inputs.</remarks>
   public Key key_button1;
   /// <summary>
   /// Specifies which keyboard key maps to button 2.
   /// </summary>
   /// <remarks>The term "button 2" simply refers to 1 of 4 customizable inputs.</remarks>
   public Key key_button2;
   /// <summary>
   /// Specifies which keyboard key maps to button 3.
   /// </summary>
   /// <remarks>The term "button 3" simply refers to 1 of 4 customizable inputs.</remarks>
   public Key key_button3;
   /// <summary>
   /// Specifies which keyboard key maps to button 4.
   /// </summary>
   /// <remarks>The term "button 4" simply refers to 1 of 4 customizable inputs.</remarks>
   public Key key_button4;

   /// <summary>
   /// Constructs a new player given the key mappings it will use.
   /// </summary>
   /// <param name="up">Initial value for <see cref="key_up"/></param>
   /// <param name="left">Initial value for <see cref="key_left"/></param>
   /// <param name="right">Initial value for <see cref="key_right"/></param>
   /// <param name="down">Initial value for <see cref="key_down"/></param>
   /// <param name="button1">Initial value for <see cref="key_button1"/></param>
   /// <param name="button2">Initial value for <see cref="key_button2"/></param>
   /// <param name="button3">Initial value for <see cref="key_button3"/></param>
   /// <param name="button4">Initial value for <see cref="key_button4"/></param>
   private void InitializeKeys(Key up, Key left, Key right, Key down,
      Key button1, Key button2, Key button3, Key button4)
   {
      key_up = up;
      key_left = left;
      key_right = right;
      key_down = down;
      key_button1 = button1;
      key_button2 = button2;
      key_button3 = button3;
      key_button4 = button4;
   }

   /// <summary>
   /// Constructs a new player based on one of 4 possible sets of default key mappings
   /// </summary>
   /// <param name="defaultSet">Number 0 through 3 indicating which set of defaults to use.
   /// Default set number 0 is mapped as follows:
   /// <list type="table">
   /// <listheader><term>Key</term><description>Mapped to</description></listheader>
   /// <item><term>Up Arrow</term><description><see cref="key_up"/></description></item>
   /// <item><term>Left Arrow</term><description><see cref="key_left"/></description></item>
   /// <item><term>Right Arrow</term><description><see cref="key_right"/></description></item>
   /// <item><term>Down Arrow</term><description><see cref="key_down"/></description></item>
   /// <item><term>Right Ctrl</term><description><see cref="key_button1"/></description></item>
   /// <item><term>Space</term><description><see cref="key_button2"/></description></item>
   /// <item><term>Return</term><description><see cref="key_button3"/></description></item>
   /// <item><term>Right Shift</term><description><see cref="key_button4"/></description></item>
   /// </list>
   /// Default set number 1 is mapped as follows:
   /// <list type="table">
   /// <listheader><term>Key</term><description>Mapped to</description></listheader>
   /// <item><term>W</term><description><see cref="key_up"/></description></item>
   /// <item><term>A</term><description><see cref="key_left"/></description></item>
   /// <item><term>D</term><description><see cref="key_right"/></description></item>
   /// <item><term>S</term><description><see cref="key_down"/></description></item>
   /// <item><term>Left Shift</term><description><see cref="key_button1"/></description></item>
   /// <item><term>Left Ctrl</term><description><see cref="key_button2"/></description></item>
   /// <item><term>Q</term><description><see cref="key_button3"/></description></item>
   /// <item><term>E</term><description><see cref="key_button4"/></description></item>
   /// </list>
   /// Default set number 2 is mapped as follows:
   /// <list type="table">
   /// <listheader><term>Key</term><description>Mapped to</description></listheader>
   /// <item><term>Numeric Keypad Up/8</term><description><see cref="key_up"/></description></item>
   /// <item><term>Numeric Keypad Left/4</term><description><see cref="key_left"/></description></item>
   /// <item><term>Numeric Keypad Right/6</term><description><see cref="key_right"/></description></item>
   /// <item><term>Numeric Keypad Down/2</term><description><see cref="key_down"/></description></item>
   /// <item><term>Numeric Keypad 5</term><description><see cref="key_button1"/></description></item>
   /// <item><term>Numeric Keypad 0</term><description><see cref="key_button2"/></description></item>
   /// <item><term>Numeric Keypad Enter</term><description><see cref="key_button3"/></description></item>
   /// <item><term>Numeric Keypad 7</term><description><see cref="key_button4"/></description></item>
   /// </list>
   /// Default set number 3 is mapped as follows:
   /// <list type="table">
   /// <listheader><term>Key</term><description>Mapped to</description></listheader>
   /// <item><term>I</term><description><see cref="key_up"/></description></item>
   /// <item><term>J</term><description><see cref="key_left"/></description></item>
   /// <item><term>L</term><description><see cref="key_right"/></description></item>
   /// <item><term>K</term><description><see cref="key_down"/></description></item>
   /// <item><term>U</term><description><see cref="key_button1"/></description></item>
   /// <item><term>O</term><description><see cref="key_button2"/></description></item>
   /// <item><term>M</term><description><see cref="key_button3"/></description></item>
   /// <item><term>,</term><description><see cref="key_button4"/></description></item>
   /// </list>
   /// </param>
   public KeyboardPlayer(int defaultSet)
   {
      switch(defaultSet)
      {
         case 0:
            InitializeKeys(
               Key.Up,           // Up
               Key.Left,         // Left
               Key.Right,        // Right
               Key.Down,         // Down
               Key.RControl,     // Button 1
               Key.Space,        // Button 2
               Key.Enter,        // Button 3
               Key.RShift);      // Button 4
            break;
         case 1:
            InitializeKeys(
               Key.W,            // Up
               Key.A,            // Left
               Key.D,            // Right
               Key.S,            // Down
               Key.LShift,       // Button 1
               Key.LControl,     // Button 2
               Key.Q,            // Button 3
               Key.E);           // Button 4
            break;
         case 2:
            InitializeKeys(
               Key.NumPad8,      // Up
               Key.NumPad4,      // Right
               Key.NumPad6,      // Left
               Key.NumPad2,      // Down
               Key.NumPad5,      // Button 1
               Key.NumPad0,      // Button 2
               Key.NumPadEnter,  // Button 3
               Key.NumPad7);     // Button 4
            break;
         default:
            InitializeKeys(
               Key.I,            // Up
               Key.J,            // Right
               Key.L,            // Left
               Key.K,            // Down
               Key.U,            // Button 1
               Key.O,            // Button 2
               Key.M,            // Button 3
               Key.Comma);       // Button 4
            break;
      }
   }

   #region IPlayer Members

   /// <summary>
   /// See <see cref="IPlayer.Up"/>.
   /// </summary>
   public bool Up
   {
      get
      {
         return Project.GameWindow.KeyboardState[key_up];
      }
   }

   /// <summary>
   /// See <see cref="IPlayer.Left"/>.
   /// </summary>
   public bool Left
   {
      get
      {
         return Project.GameWindow.KeyboardState[key_left];
      }
   }

   /// <summary>
   /// See <see cref="IPlayer.Right"/>.
   /// </summary>
   public bool Right
   {
      get
      {
         return Project.GameWindow.KeyboardState[key_right];
      }
   }

   /// <summary>
   /// See <see cref="IPlayer.Down"/>.
   /// </summary>
   public bool Down
   {
      get
      {
         return Project.GameWindow.KeyboardState[key_down];
      }
   }

   /// <summary>
   /// See <see cref="IPlayer.Button1"/>.
   /// </summary>
   public bool Button1
   {
      get
      {
         return Project.GameWindow.KeyboardState[key_button1];
      }
   }

   /// <summary>
   /// See <see cref="IPlayer.Button2"/>.
   /// </summary>
   public bool Button2
   {
      get
      {
         return Project.GameWindow.KeyboardState[key_button2];
      }
   }

   /// <summary>
   /// See <see cref="IPlayer.Button3"/>.
   /// </summary>
   public bool Button3
   {
      get
      {
         return Project.GameWindow.KeyboardState[key_button3];
      }
   }

   /// <summary>
   /// See <see cref="IPlayer.Button4"/>.
   /// </summary>
   public bool Button4
   {
      get
      {
         return Project.GameWindow.KeyboardState[key_button4];
      }
   }

   #endregion
}

/// <summary>
/// Represents interactions between the game and a player via a joystick/gamepad
/// </summary>
[Serializable()]
public partial class ControllerPlayer : IPlayer
{
   public int deviceNumber;
   /// <summary>
   /// Array of 4 integers mapping controller button numbers to player button inputs.
   /// </summary>
   /// <remarks>The array element at index 0 contains index of the button on the controller
   /// that is mapped to player button number 1. Element 1 is the index for button 2 and
   /// so forth.</remarks>
   public byte[] buttonMap;

   /// <summary>
   /// Constructs a player object that links input from the specified game controller device
   /// to game input.
   /// </summary>
   /// <param name="deviceNumber">Device number as defined in user's Control Panel</param>
   public ControllerPlayer(int deviceNumber)
   {
      this.deviceNumber = deviceNumber;
      buttonMap = new byte[4] {0, 1, 2, 3};
   }

   public Joystick MyController
   {
      get
      {
         return Project.GameWindow.GetControllerState(deviceNumber);
      }
   }
   #region IPlayer Members

   /// <summary>
   /// See <see cref="IPlayer.Up"/>.
   /// </summary>
   public bool Up
   {
      get
      {
         return MyController.YPosition < (MyController.MinimumY * 3 + MyController.MaximumY) / 4;
      }
   }

   /// <summary>
   /// See <see cref="IPlayer.Left"/>.
   /// </summary>
   public bool Left
   {
      get
      {
         return MyController.XPosition < (MyController.MinimumX * 3 + MyController.MaximumX) / 4;
      }
   }

   /// <summary>
   /// See <see cref="IPlayer.Right"/>.
   /// </summary>
   public bool Right
   {
      get
      {
         return MyController.XPosition > (MyController.MinimumX + MyController.MaximumX * 3) / 4;
      }
   }

   /// <summary>
   /// See <see cref="IPlayer.Down"/>.
   /// </summary>
   public bool Down
   {
      get
      {
         return MyController.YPosition > (MyController.MinimumY + MyController.MaximumY * 3) / 4;
      }
   }

   /// <summary>
   /// See <see cref="IPlayer.Button1"/>.
   /// </summary>
   public bool Button1
   {
      get
      {
         return MyController[buttonMap[0]];
      }
   }

   /// <summary>
   /// See <see cref="IPlayer.Button2"/>.
   /// </summary>
   public bool Button2
   {
      get
      {
         return MyController[buttonMap[1]];
      }
   }

   /// <summary>
   /// See <see cref="IPlayer.Button3"/>.
   /// </summary>
   public bool Button3
   {
      get
      {
         return MyController[buttonMap[2]];
      }
   }

   /// <summary>
   /// See <see cref="IPlayer.Button4"/>.
   /// </summary>
   public bool Button4
   {
      get
      {
         return MyController[buttonMap[3]];
      }
   }

   #endregion
}
