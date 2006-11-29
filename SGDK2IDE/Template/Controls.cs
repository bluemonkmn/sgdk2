using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

/// <summary>
/// Summary description for frmControls.
/// </summary>
public class frmControls : System.Windows.Forms.Form
{
   #region Windows Form Designer Members
   private System.Windows.Forms.Label lblController;
   private System.Windows.Forms.ComboBox cboController;
   private System.Windows.Forms.RadioButton rdoController;
   private System.Windows.Forms.RadioButton rdoKeyboard;
   private System.Windows.Forms.TextBox txtButton4;
   private System.Windows.Forms.Label lblButton4;
   private System.Windows.Forms.TextBox txtButton3;
   private System.Windows.Forms.Label lblButton3;
   private System.Windows.Forms.TextBox txtButton2;
   private System.Windows.Forms.Label lblButton2;
   private System.Windows.Forms.TextBox txtButton1;
   private System.Windows.Forms.Label lblButton1;
   private System.Windows.Forms.TextBox txtDown;
   private System.Windows.Forms.Label lblDown;
   private System.Windows.Forms.TextBox txtRight;
   private System.Windows.Forms.Label lblRight;
   private System.Windows.Forms.TextBox txtLeft;
   private System.Windows.Forms.Label lblLeft;
   private System.Windows.Forms.TextBox txtUp;
   private System.Windows.Forms.Label lblUp;
   private System.Windows.Forms.ComboBox cboPlayer;
   private System.Windows.Forms.Label lblPlayer;
   private System.Windows.Forms.GroupBox grpPlayers;
   private System.Windows.Forms.RadioButton[] rdoPlayers;

   /// <summary>
   /// Required designer variable.
   /// </summary>
   private System.ComponentModel.Container components = null;
   #endregion
   
   public frmControls()
   {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      if (Project.MaxPlayers <= 1)
      {
         grpPlayers.Hide();
         this.ClientSize = new System.Drawing.Size(298, 185);
         lblPlayer.Hide();
         cboPlayer.Hide();
      }

      for(int i = 0; i < Project.GameWindow.ControllerCount; i++)
         cboController.Items.Add(Project.GameWindow.GetControllerName(i));
      rdoController.Enabled = (cboController.Items.Count > 0);
      for (int i = Project.MaxPlayers; i < rdoPlayers.Length; i++)
         rdoPlayers[i].Visible = false;
      rdoPlayers[0].Checked = true;
   }

   /// <summary>
   /// Clean up any resources being used.
   /// </summary>
   protected override void Dispose( bool disposing )
   {
      if( disposing )
      {
         if(components != null)
         {
            components.Dispose();
         }
      }
      base.Dispose( disposing );
   }

   #region Windows Form Designer generated code
   private void InitializeComponent()
   {
      this.lblController = new System.Windows.Forms.Label();
      this.cboController = new System.Windows.Forms.ComboBox();
      this.rdoController = new System.Windows.Forms.RadioButton();
      this.rdoKeyboard = new System.Windows.Forms.RadioButton();
      this.txtButton4 = new System.Windows.Forms.TextBox();
      this.lblButton4 = new System.Windows.Forms.Label();
      this.txtButton3 = new System.Windows.Forms.TextBox();
      this.lblButton3 = new System.Windows.Forms.Label();
      this.txtButton2 = new System.Windows.Forms.TextBox();
      this.lblButton2 = new System.Windows.Forms.Label();
      this.txtButton1 = new System.Windows.Forms.TextBox();
      this.lblButton1 = new System.Windows.Forms.Label();
      this.txtDown = new System.Windows.Forms.TextBox();
      this.lblDown = new System.Windows.Forms.Label();
      this.txtRight = new System.Windows.Forms.TextBox();
      this.lblRight = new System.Windows.Forms.Label();
      this.txtLeft = new System.Windows.Forms.TextBox();
      this.lblLeft = new System.Windows.Forms.Label();
      this.txtUp = new System.Windows.Forms.TextBox();
      this.lblUp = new System.Windows.Forms.Label();
      this.cboPlayer = new System.Windows.Forms.ComboBox();
      this.lblPlayer = new System.Windows.Forms.Label();
      this.grpPlayers = new System.Windows.Forms.GroupBox();
      this.rdoPlayers = new System.Windows.Forms.RadioButton[4]
      {
         new System.Windows.Forms.RadioButton(),
         new System.Windows.Forms.RadioButton(),
         new System.Windows.Forms.RadioButton(),
         new System.Windows.Forms.RadioButton()
      };
      this.grpPlayers.SuspendLayout();
      this.SuspendLayout();
      // 
      // lblController
      // 
      this.lblController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblController.Location = new System.Drawing.Point(24, 224);
      this.lblController.Name = "lblController";
      this.lblController.Size = new System.Drawing.Size(64, 16);
      this.lblController.TabIndex = 43;
      this.lblController.Enabled = false;
      this.lblController.Text = "Controller:";
      // 
      // cboController
      // 
      this.cboController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.cboController.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboController.Location = new System.Drawing.Point(88, 224);
      this.cboController.Name = "cboController";
      this.cboController.Size = new System.Drawing.Size(200, 21);
      this.cboController.Enabled = false;
      this.cboController.TabIndex = 42;
      this.cboController.SelectedIndexChanged += new System.EventHandler(this.cboController_SelectedIndexChanged);
      // 
      // rdoController
      // 
      this.rdoController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.rdoController.Location = new System.Drawing.Point(8, 200);
      this.rdoController.Name = "rdoController";
      this.rdoController.Size = new System.Drawing.Size(272, 16);
      this.rdoController.TabIndex = 41;
      this.rdoController.Text = "Game Controller";
      this.rdoController.CheckedChanged += new System.EventHandler(this.InputType_Changed);
      // 
      // rdoKeyboard
      // 
      this.rdoKeyboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.rdoKeyboard.Location = new System.Drawing.Point(8, 80);
      this.rdoKeyboard.Name = "rdoKeyboard";
      this.rdoKeyboard.Size = new System.Drawing.Size(272, 16);
      this.rdoKeyboard.TabIndex = 40;
      this.rdoKeyboard.Text = "Keyboard";
      this.rdoKeyboard.CheckedChanged += new System.EventHandler(this.InputType_Changed);
      // 
      // txtButton4
      // 
      this.txtButton4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtButton4.Location = new System.Drawing.Point(224, 168);
      this.txtButton4.Name = "txtButton4";
      this.txtButton4.Size = new System.Drawing.Size(64, 20);
      this.txtButton4.TabIndex = 39;
      this.txtButton4.Text = "";
      this.txtButton4.Enabled = false;
      this.txtButton4.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyBox_KeyDown);
      // 
      // lblButton4
      // 
      this.lblButton4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblButton4.Location = new System.Drawing.Point(160, 168);
      this.lblButton4.Name = "lblButton4";
      this.lblButton4.Size = new System.Drawing.Size(64, 20);
      this.lblButton4.TabIndex = 38;
      this.lblButton4.Text = "Button 4:";
      this.lblButton4.Enabled = false;
      this.txtButton4.ReadOnly = true;
      this.lblButton4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtButton3
      // 
      this.txtButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtButton3.Location = new System.Drawing.Point(224, 144);
      this.txtButton3.Name = "txtButton3";
      this.txtButton3.Size = new System.Drawing.Size(64, 20);
      this.txtButton3.TabIndex = 37;
      this.txtButton3.Text = "";
      this.txtButton3.Enabled = false;
      this.txtButton3.ReadOnly = true;
      this.txtButton3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyBox_KeyDown);
      // 
      // lblButton3
      // 
      this.lblButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblButton3.Location = new System.Drawing.Point(160, 144);
      this.lblButton3.Name = "lblButton3";
      this.lblButton3.Size = new System.Drawing.Size(64, 20);
      this.lblButton3.TabIndex = 36;
      this.lblButton3.Text = "Button 3:";
      this.lblButton3.Enabled = false;
      this.lblButton3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtButton2
      // 
      this.txtButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtButton2.Location = new System.Drawing.Point(224, 120);
      this.txtButton2.Name = "txtButton2";
      this.txtButton2.Size = new System.Drawing.Size(64, 20);
      this.txtButton2.TabIndex = 35;
      this.txtButton2.Text = "";
      this.txtButton2.Enabled = false;
      this.txtButton2.ReadOnly = true;
      this.txtButton2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyBox_KeyDown);
      // 
      // lblButton2
      // 
      this.lblButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblButton2.Location = new System.Drawing.Point(160, 120);
      this.lblButton2.Name = "lblButton2";
      this.lblButton2.Size = new System.Drawing.Size(64, 20);
      this.lblButton2.TabIndex = 34;
      this.lblButton2.Text = "Button 2:";
      this.lblButton2.Enabled = false;
      this.lblButton2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtButton1
      // 
      this.txtButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtButton1.Location = new System.Drawing.Point(224, 96);
      this.txtButton1.Name = "txtButton1";
      this.txtButton1.Size = new System.Drawing.Size(64, 20);
      this.txtButton1.TabIndex = 33;
      this.txtButton1.Text = "";
      this.txtButton1.Enabled = false;
      this.txtButton1.ReadOnly = true;
      this.txtButton1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyBox_KeyDown);
      // 
      // lblButton1
      // 
      this.lblButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblButton1.Location = new System.Drawing.Point(160, 96);
      this.lblButton1.Name = "lblButton1";
      this.lblButton1.Size = new System.Drawing.Size(64, 20);
      this.lblButton1.TabIndex = 32;
      this.lblButton1.Text = "Button 1:";
      this.lblButton1.Enabled = false;
      this.lblButton1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtDown
      // 
      this.txtDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtDown.Location = new System.Drawing.Point(88, 168);
      this.txtDown.Name = "txtDown";
      this.txtDown.Size = new System.Drawing.Size(64, 20);
      this.txtDown.TabIndex = 31;
      this.txtDown.Text = "";
      this.txtDown.Enabled = false;
      this.txtDown.ReadOnly = true;
      this.txtDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyBox_KeyDown);
      // 
      // lblDown
      // 
      this.lblDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblDown.Location = new System.Drawing.Point(24, 168);
      this.lblDown.Name = "lblDown";
      this.lblDown.Size = new System.Drawing.Size(64, 20);
      this.lblDown.TabIndex = 30;
      this.lblDown.Text = "Down:";
      this.lblDown.Enabled = false;
      this.lblDown.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtRight
      // 
      this.txtRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtRight.Location = new System.Drawing.Point(88, 144);
      this.txtRight.Name = "txtRight";
      this.txtRight.Size = new System.Drawing.Size(64, 20);
      this.txtRight.TabIndex = 29;
      this.txtRight.Text = "";
      this.txtRight.Enabled = false;
      this.txtRight.ReadOnly = true;
      this.txtRight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyBox_KeyDown);
      // 
      // lblRight
      // 
      this.lblRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblRight.Location = new System.Drawing.Point(24, 144);
      this.lblRight.Name = "lblRight";
      this.lblRight.Size = new System.Drawing.Size(64, 20);
      this.lblRight.TabIndex = 28;
      this.lblRight.Text = "Right:";
      this.lblRight.Enabled = false;
      this.lblRight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtLeft
      // 
      this.txtLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtLeft.Location = new System.Drawing.Point(88, 120);
      this.txtLeft.Name = "txtLeft";
      this.txtLeft.Size = new System.Drawing.Size(64, 20);
      this.txtLeft.TabIndex = 27;
      this.txtLeft.Text = "";
      this.txtLeft.Enabled = false;
      this.txtLeft.ReadOnly = true;
      this.txtLeft.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyBox_KeyDown);
      // 
      // lblLeft
      // 
      this.lblLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblLeft.Location = new System.Drawing.Point(24, 120);
      this.lblLeft.Name = "lblLeft";
      this.lblLeft.Size = new System.Drawing.Size(64, 20);
      this.lblLeft.TabIndex = 26;
      this.lblLeft.Text = "Left:";
      this.lblLeft.Enabled = false;
      this.lblLeft.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtUp
      // 
      this.txtUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtUp.Location = new System.Drawing.Point(88, 96);
      this.txtUp.Name = "txtUp";
      this.txtUp.Size = new System.Drawing.Size(64, 20);
      this.txtUp.TabIndex = 25;
      this.txtUp.Text = "";
      this.txtUp.Enabled = false;
      this.txtUp.ReadOnly = true;
      this.txtUp.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyBox_KeyDown);
      // 
      // lblUp
      // 
      this.lblUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblUp.Location = new System.Drawing.Point(24, 96);
      this.lblUp.Name = "lblUp";
      this.lblUp.Size = new System.Drawing.Size(64, 20);
      this.lblUp.TabIndex = 24;
      this.lblUp.Text = "Up:";
      this.lblUp.Enabled = false;
      this.lblUp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cboPlayer
      // 
      this.cboPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.cboPlayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboPlayer.Location = new System.Drawing.Point(88, 48);
      this.cboPlayer.Name = "cboPlayer";
      this.cboPlayer.Size = new System.Drawing.Size(200, 21);
      this.cboPlayer.TabIndex = 23;
      this.cboPlayer.SelectedIndexChanged += new System.EventHandler(this.cboPlayer_SelectedIndexChanged);
      // 
      // lblPlayer
      // 
      this.lblPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblPlayer.Location = new System.Drawing.Point(8, 48);
      this.lblPlayer.Name = "lblPlayer";
      this.lblPlayer.Size = new System.Drawing.Size(80, 21);
      this.lblPlayer.TabIndex = 22;
      this.lblPlayer.Text = "Player:";
      this.lblPlayer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // grpPlayers
      // 
      this.grpPlayers.Controls.Add(this.rdoPlayers[3]);
      this.grpPlayers.Controls.Add(this.rdoPlayers[2]);
      this.grpPlayers.Controls.Add(this.rdoPlayers[1]);
      this.grpPlayers.Controls.Add(this.rdoPlayers[0]);
      this.grpPlayers.Location = new System.Drawing.Point(8, 0);
      this.grpPlayers.Name = "grpPlayers";
      this.grpPlayers.Size = new System.Drawing.Size(280, 40);
      this.grpPlayers.TabIndex = 44;
      this.grpPlayers.TabStop = false;
      this.grpPlayers.Text = "Players";
      // 
      // rdoOnePlayer
      // 
      this.rdoPlayers[0].Location = new System.Drawing.Point(8, 16);
      this.rdoPlayers[0].Name = "rdoOnePlayer";
      this.rdoPlayers[0].Size = new System.Drawing.Size(64, 16);
      this.rdoPlayers[0].TabIndex = 0;
      this.rdoPlayers[0].Text = "&One";
      this.rdoPlayers[0].CheckedChanged += new System.EventHandler(this.PlayerCount_Changed);
      // 
      // rdoTwoPlayers
      // 
      this.rdoPlayers[1].Location = new System.Drawing.Point(72, 16);
      this.rdoPlayers[1].Name = "rdoTwoPlayers";
      this.rdoPlayers[1].Size = new System.Drawing.Size(64, 16);
      this.rdoPlayers[1].TabIndex = 1;
      this.rdoPlayers[1].Text = "&Two";
      this.rdoPlayers[1].CheckedChanged += new System.EventHandler(this.PlayerCount_Changed);
      // 
      // rdoThreePlayers
      // 
      this.rdoPlayers[2].Location = new System.Drawing.Point(136, 16);
      this.rdoPlayers[2].Name = "rdoThreePlayers";
      this.rdoPlayers[2].Size = new System.Drawing.Size(64, 16);
      this.rdoPlayers[2].TabIndex = 2;
      this.rdoPlayers[2].Text = "T&hree";
      this.rdoPlayers[2].CheckedChanged += new System.EventHandler(this.PlayerCount_Changed);
      // 
      // rdoFourPlayers
      // 
      this.rdoPlayers[3].Location = new System.Drawing.Point(200, 16);
      this.rdoPlayers[3].Name = "rdoFourPlayers";
      this.rdoPlayers[3].Size = new System.Drawing.Size(64, 16);
      this.rdoPlayers[3].TabIndex = 3;
      this.rdoPlayers[3].Text = "&Four";
      this.rdoPlayers[3].CheckedChanged += new System.EventHandler(this.PlayerCount_Changed);
      // 
      // frmControls
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(298, 255);
      this.Controls.Add(this.grpPlayers);
      this.Controls.Add(this.lblController);
      this.Controls.Add(this.cboController);
      this.Controls.Add(this.rdoController);
      this.Controls.Add(this.rdoKeyboard);
      this.Controls.Add(this.txtButton4);
      this.Controls.Add(this.txtButton3);
      this.Controls.Add(this.txtButton2);
      this.Controls.Add(this.txtButton1);
      this.Controls.Add(this.txtDown);
      this.Controls.Add(this.txtRight);
      this.Controls.Add(this.txtLeft);
      this.Controls.Add(this.txtUp);
      this.Controls.Add(this.lblButton4);
      this.Controls.Add(this.lblButton3);
      this.Controls.Add(this.lblButton2);
      this.Controls.Add(this.lblButton1);
      this.Controls.Add(this.lblDown);
      this.Controls.Add(this.lblRight);
      this.Controls.Add(this.lblLeft);
      this.Controls.Add(this.lblUp);
      this.Controls.Add(this.cboPlayer);
      this.Controls.Add(this.lblPlayer);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.Name = "frmControls";
      this.Text = "Customize Controls";
      this.grpPlayers.ResumeLayout(false);
      this.ResumeLayout(false);

   }
   #endregion

   private void InputType_Changed(object sender, System.EventArgs e)
   {
      if (((RadioButton)sender).Checked)
      {
         lblUp.Enabled = txtUp.Enabled =
            lblLeft.Enabled = txtLeft.Enabled =
            lblRight.Enabled = txtRight.Enabled =
            lblDown.Enabled = txtDown.Enabled =
            lblButton1.Enabled = txtButton1.Enabled = 
            lblButton2.Enabled = txtButton2.Enabled = 
            lblButton3.Enabled = txtButton3.Enabled = 
            lblButton4.Enabled = txtButton4.Enabled =
            sender == rdoKeyboard;

         lblController.Enabled = cboController.Enabled = 
            (sender != rdoKeyboard);

         if (sender == rdoKeyboard)
            Project.GameWindow.Players[SelectedPlayer] = new KeyboardPlayer(
               Microsoft.DirectX.DirectInput.Key.UpArrow,
               Microsoft.DirectX.DirectInput.Key.LeftArrow,
               Microsoft.DirectX.DirectInput.Key.RightArrow,
               Microsoft.DirectX.DirectInput.Key.DownArrow,
               Microsoft.DirectX.DirectInput.Key.RightControl,
               Microsoft.DirectX.DirectInput.Key.Space,
               Microsoft.DirectX.DirectInput.Key.Return,
               Microsoft.DirectX.DirectInput.Key.RightShift);
         else
            Project.GameWindow.Players[SelectedPlayer] = new ControllerPlayer(SelectedPlayer % cboController.Items.Count);

         LoadCurrentControls();
      }
   }

   private void PlayerCount_Changed(object sender, System.EventArgs e)
   {
      if (((RadioButton)sender).Checked)
      {
         for (int playerIndex=0; playerIndex<4; playerIndex++)
         {
            if (rdoPlayers[playerIndex] == sender)
            {
               cboPlayer.Items.Clear();
               for (int i = 0; i<=playerIndex; i++)
               {
                  cboPlayer.Items.Add("Player " + (i+1).ToString());
               }

               Project.GameWindow.CurrentPlayers = (byte)(playerIndex + 1);
            }
         }
         cboPlayer.SelectedIndex = 0;
      }
   }

   private int SelectedPlayer
   {
      get
      {
         return cboPlayer.SelectedIndex;
      }
   }

   private void KeyBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
   {
      Microsoft.DirectX.DirectInput.Key[] pressed;
      do
      {
         pressed = Project.GameWindow.GetPressedKeys();
      } while(pressed.Length == 0);

      ((TextBox)sender).Text = System.Enum.Format(typeof(
         Microsoft.DirectX.DirectInput.Key), pressed[0], "g");

      KeyboardPlayer player = (KeyboardPlayer)Project.GameWindow.Players[SelectedPlayer];
      if (sender == txtUp)
         player.key_up = pressed[0];
      else if (sender == txtLeft)
         player.key_left = pressed[0];
      else if (sender == txtRight)
         player.key_right = pressed[0];
      else if (sender == txtDown)
         player.key_down = pressed[0];
      else if (sender == txtButton1)
         player.key_button1 = pressed[0];
      else if (sender == txtButton2)
         player.key_button2 = pressed[0];
      else if (sender == txtButton3)
         player.key_button3 = pressed[0];
      else if (sender == txtButton4)
         player.key_button4 = pressed[0];
   }

   private void cboController_SelectedIndexChanged(object sender, System.EventArgs e)
   {
      if (cboController.SelectedIndex >= 0)
         ((ControllerPlayer)Project.GameWindow.Players[SelectedPlayer]).deviceNumber = cboController.SelectedIndex;
   }

   private void cboPlayer_SelectedIndexChanged(object sender, System.EventArgs e)
   {
      LoadCurrentControls();
   }

   private void LoadCurrentControls()
   {
      if (Project.GameWindow.Players[SelectedPlayer] is KeyboardPlayer)
      {
         KeyboardPlayer player = (KeyboardPlayer)Project.GameWindow.Players[SelectedPlayer];
         rdoKeyboard.Checked = true;
         txtUp.Text = System.Enum.Format(typeof(Microsoft.DirectX.DirectInput.Key), player.key_up, "g");
         txtLeft.Text = System.Enum.Format(typeof(Microsoft.DirectX.DirectInput.Key), player.key_left, "g");
         txtRight.Text = System.Enum.Format(typeof(Microsoft.DirectX.DirectInput.Key), player.key_right, "g");
         txtDown.Text = System.Enum.Format(typeof(Microsoft.DirectX.DirectInput.Key), player.key_down, "g");
         txtButton1.Text = System.Enum.Format(typeof(Microsoft.DirectX.DirectInput.Key), player.key_button1, "g");
         txtButton2.Text = System.Enum.Format(typeof(Microsoft.DirectX.DirectInput.Key), player.key_button2, "g");
         txtButton3.Text = System.Enum.Format(typeof(Microsoft.DirectX.DirectInput.Key), player.key_button3, "g");
         txtButton4.Text = System.Enum.Format(typeof(Microsoft.DirectX.DirectInput.Key), player.key_button4, "g");
      }
      else
      {
         rdoController.Checked = true;
         int devNum = ((ControllerPlayer)Project.GameWindow.Players[SelectedPlayer]).deviceNumber;
         if (cboController.Items.Count > devNum)
            cboController.SelectedIndex = devNum;
         else
            cboController.SelectedIndex = -1;
      }
   }

   protected override void OnClosed(EventArgs e)
   {
      base.OnClosed (e);
      Project.GameWindow.RefreshControllers();
   }
}

