function KeyboardPlayer(defaultSet) {
   switch(defaultSet)
   {
      case 0:
         this.initializeKeys(
            keyboardState.key.Up,     // Up
            keyboardState.key.Left,   // Left
            keyboardState.key.Right,  // Right
            keyboardState.key.Down,   // Down
            keyboardState.key.Ctrl,   // Button 1
            keyboardState.key.Space,  // Button 2
            keyboardState.key.Enter,  // Button 3
            keyboardState.key.Shift); // Button 4
         break;
      case 1:
         this.initializeKeys(
            keyboardState.key.W,     // Up
            keyboardState.key.A,     // Left
            keyboardState.key.D,     // Right
            keyboardState.key.S,     // Down
            keyboardState.key.Z,     // Button 1
            keyboardState.key.C,     // Button 2
            keyboardState.key.Q,     // Button 3
            keyboardState.key.E);    // Button 4
         break;
      case 2:
         this.initializeKeys(
            keyboardState.key.NumPad8,      // Up
            keyboardState.key.NumPad4,      // Right
            keyboardState.key.NumPad6,      // Left
            keyboardState.key.NumPad2,      // Down
            keyboardState.key.NumPad5,      // Button 1
            keyboardState.key.NumPad0,      // Button 2
            keyboardState.key.NumPadEnter,  // Button 3
            keyboardState.key.NumPad7);     // Button 4
         break;
      default:
         this.initializeKeys(
            keyboardState.key.I,            // Up
            keyboardState.key.J,            // Right
            keyboardState.key.L,            // Left
            keyboardState.key.K,            // Down
            keyboardState.key.U,            // Button 1
            keyboardState.key.O,            // Button 2
            keyboardState.key.M,            // Button 3
            keyboardState.key.Comma);       // Button 4
         break;
   }
}

KeyboardPlayer.prototype.initializeKeys = function(up, left, right, down, button1, button2, button3, button4) {
   this.upKey = up;
   this.leftKey = left;
   this.rightKey = right;
   this.downKey = down;
   this.button1Key = button1;
   this.button2Key = button2;
   this.button3Key = button3;
   this.button4Key = button4;
};

KeyboardPlayer.prototype.up = function() { return keyboardState.keyState[this.upKey]; };
KeyboardPlayer.prototype.left = function() { return keyboardState.keyState[this.leftKey]; };
KeyboardPlayer.prototype.right = function() { return keyboardState.keyState[this.rightKey]; };
KeyboardPlayer.prototype.down = function() { return keyboardState.keyState[this.downKey]; };
KeyboardPlayer.prototype.button1 = function() { return keyboardState.keyState[this.button1Key]; };
KeyboardPlayer.prototype.button2 = function() { return keyboardState.keyState[this.button2Key]; };
KeyboardPlayer.prototype.button3 = function() { return keyboardState.keyState[this.button3Key]; };
KeyboardPlayer.prototype.button4 = function() { return keyboardState.keyState[this.button4Key]; };

var players = [ new KeyboardPlayer(0), new KeyboardPlayer(1), new KeyboardPlayer(2), new KeyboardPlayer(3) ];