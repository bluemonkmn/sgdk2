using System;
using Microsoft.DirectX.DirectInput;

public interface IPlayer
{
   bool Up
   {
      get;
   }

   bool Left
   {
      get;
   }

   bool Right
   {
      get;
   }

   bool Down
   {
      get;
   }

   bool Button1
   {
      get;
   }

   bool Button2
   {
      get;
   }

   bool Button3
   {
      get;
   }

   bool Button4
   {
      get;
   }
}

/// <summary>
/// Represents interactions between the game and a player via the keyboard
/// </summary>
public class KeyboardPlayer : IPlayer
{
   public Key key_left;
   public Key key_up;
   public Key key_right;
   public Key key_down;
   public Key key_button1;
   public Key key_button2;
   public Key key_button3;
   public Key key_button4;      

   public KeyboardPlayer(Key up, Key left, Key right, Key down,
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
   #region IPlayer Members

   public bool Up
   {
      get
      {
         return Project.GameWindow.KeyboardState[key_up];
      }
   }

   public bool Left
   {
      get
      {
         return Project.GameWindow.KeyboardState[key_left];
      }
   }

   public bool Right
   {
      get
      {
         return Project.GameWindow.KeyboardState[key_right];
      }
   }

   public bool Down
   {
      get
      {
         return Project.GameWindow.KeyboardState[key_down];
      }
   }

   public bool Button1
   {
      get
      {
         return Project.GameWindow.KeyboardState[key_button1];
      }
   }

   public bool Button2
   {
      get
      {
         return Project.GameWindow.KeyboardState[key_button2];
      }
   }

   public bool Button3
   {
      get
      {
         return Project.GameWindow.KeyboardState[key_button3];
      }
   }

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
public class ControllerPlayer : IPlayer
{
   public int deviceNumber;

   public ControllerPlayer(int deviceNumber)
   {
      this.deviceNumber = deviceNumber;
   }

   #region IPlayer Members

   public bool Up
   {
      get
      {
         return Project.GameWindow.GetControllerState(deviceNumber).Y < 0x4000;
      }
   }

   public bool Left
   {
      get
      {
         return Project.GameWindow.GetControllerState(deviceNumber).X < 0x4000;
      }
   }

   public bool Right
   {
      get
      {
         return Project.GameWindow.GetControllerState(deviceNumber).X > 0xC000;
      }
   }

   public bool Down
   {
      get
      {
         return Project.GameWindow.GetControllerState(deviceNumber).Y > 0xC000;
      }
   }

   public bool Button1
   {
      get
      {
         return Project.GameWindow.GetControllerState(deviceNumber).GetButtons()[0] != 0;
      }
   }

   public bool Button2
   {
      get
      {
         return Project.GameWindow.GetControllerState(deviceNumber).GetButtons()[1] != 0;
      }
   }

   public bool Button3
   {
      get
      {
         return Project.GameWindow.GetControllerState(deviceNumber).GetButtons()[2] != 0;
      }
   }

   public bool Button4
   {
      get
      {
         return Project.GameWindow.GetControllerState(deviceNumber).GetButtons()[3] != 0;
      }
   }

   #endregion
}
