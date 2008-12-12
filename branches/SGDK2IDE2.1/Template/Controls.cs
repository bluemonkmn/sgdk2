/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

/// <summary>
/// UI to support the player's customization of input device.
/// </summary>
public partial class frmControls : System.Windows.Forms.Form
{
   #region Embedded Types
   class KeyTextBox : TextBox
   {
      protected override void WndProc(ref Message m)
      {
         base.WndProc(ref m);
         if (Parent is frmControls)
            ((frmControls)Parent).ProcessMessage(this, m);
      }
   }
   #endregion

   #region Non-control members
   private System.Threading.Thread readControllerThread = null;
   private bool bExitControllerThread = false;
   private delegate void ControllerButtonPressedDelegate(byte button);
   private int pressedButtons = 0;
   private int currentPlayer;
   private System.Windows.Forms.TextBox txtCurJButton = null;
   private bool bLoading = false;
   private const int WM_KEYDOWN = 0x100;
   #endregion

   #region Windows Form Designer Members
   private System.Windows.Forms.Label lblController;
   private System.Windows.Forms.ComboBox cboController;
   private System.Windows.Forms.RadioButton rdoController;
   private System.Windows.Forms.RadioButton rdoKeyboard;
   private KeyTextBox txtButton4;
   private System.Windows.Forms.Label lblButton4;
   private KeyTextBox txtButton3;
   private System.Windows.Forms.Label lblButton3;
   private KeyTextBox txtButton2;
   private System.Windows.Forms.Label lblButton2;
   private KeyTextBox txtButton1;
   private System.Windows.Forms.Label lblButton1;
   private KeyTextBox txtDown;
   private System.Windows.Forms.Label lblDown;
   private KeyTextBox txtRight;
   private System.Windows.Forms.Label lblRight;
   private KeyTextBox txtLeft;
   private System.Windows.Forms.Label lblLeft;
   private KeyTextBox txtUp;
   private System.Windows.Forms.Label lblUp;
   private System.Windows.Forms.ComboBox cboPlayer;
   private System.Windows.Forms.Label lblPlayer;
   private System.Windows.Forms.TextBox txtJButton4;
   private System.Windows.Forms.TextBox txtJButton3;
   private System.Windows.Forms.TextBox txtJButton2;
   private System.Windows.Forms.TextBox txtJButton1;
   private System.Windows.Forms.Label lblJButton4;
   private System.Windows.Forms.Label lblJButton3;
   private System.Windows.Forms.Label lblJButton2;
   private System.Windows.Forms.Label lblJButton1;
   private System.Windows.Forms.Label label1;

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
         lblPlayer.Hide();
         cboPlayer.Hide();
         this.ClientSize = new System.Drawing.Size(298, 270);
      }

      for(int i = 0; i < Project.GameWindow.ControllerCount; i++)
         cboController.Items.Add(Project.GameWindow.GetControllerName(i));
      rdoController.Enabled = (cboController.Items.Count > 0);

      for (int i = 0; i<Project.MaxPlayers; i++)
         cboPlayer.Items.Add("Player " + (i+1).ToString());
      cboPlayer.SelectedIndex = 0;
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
      EndControllerThread();
   }

   #region Windows Form Designer generated code
   private void InitializeComponent()
   {
      this.lblController = new System.Windows.Forms.Label();
      this.cboController = new System.Windows.Forms.ComboBox();
      this.rdoController = new System.Windows.Forms.RadioButton();
      this.rdoKeyboard = new System.Windows.Forms.RadioButton();
      this.txtButton4 = new KeyTextBox();
      this.lblButton4 = new System.Windows.Forms.Label();
      this.txtButton3 = new KeyTextBox();
      this.lblButton3 = new System.Windows.Forms.Label();
      this.txtButton2 = new KeyTextBox();
      this.lblButton2 = new System.Windows.Forms.Label();
      this.txtButton1 = new KeyTextBox();
      this.lblButton1 = new System.Windows.Forms.Label();
      this.txtDown = new KeyTextBox();
      this.lblDown = new System.Windows.Forms.Label();
      this.txtRight = new KeyTextBox();
      this.lblRight = new System.Windows.Forms.Label();
      this.txtLeft = new KeyTextBox();
      this.lblLeft = new System.Windows.Forms.Label();
      this.txtUp = new KeyTextBox();
      this.lblUp = new System.Windows.Forms.Label();
      this.cboPlayer = new System.Windows.Forms.ComboBox();
      this.lblPlayer = new System.Windows.Forms.Label();
      this.txtJButton4 = new System.Windows.Forms.TextBox();
      this.txtJButton3 = new System.Windows.Forms.TextBox();
      this.txtJButton2 = new System.Windows.Forms.TextBox();
      this.txtJButton1 = new System.Windows.Forms.TextBox();
      this.lblJButton4 = new System.Windows.Forms.Label();
      this.lblJButton3 = new System.Windows.Forms.Label();
      this.lblJButton2 = new System.Windows.Forms.Label();
      this.lblJButton1 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // lblController
      // 
      this.lblController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblController.Enabled = false;
      this.lblController.Location = new System.Drawing.Point(24, 184);
      this.lblController.Name = "lblController";
      this.lblController.Size = new System.Drawing.Size(64, 16);
      this.lblController.TabIndex = 43;
      this.lblController.Text = "Controller:";
      // 
      // cboController
      // 
      this.cboController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.cboController.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboController.Enabled = false;
      this.cboController.Location = new System.Drawing.Point(88, 184);
      this.cboController.Name = "cboController";
      this.cboController.Size = new System.Drawing.Size(200, 21);
      this.cboController.TabIndex = 42;
      this.cboController.SelectedIndexChanged += new System.EventHandler(this.cboController_SelectedIndexChanged);
      // 
      // rdoController
      // 
      this.rdoController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.rdoController.Location = new System.Drawing.Point(8, 160);
      this.rdoController.Name = "rdoController";
      this.rdoController.Size = new System.Drawing.Size(272, 16);
      this.rdoController.TabIndex = 41;
      this.rdoController.Text = "Game Controller";
      this.rdoController.CheckedChanged += new System.EventHandler(this.InputType_Changed);
      // 
      // rdoKeyboard
      // 
      this.rdoKeyboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.rdoKeyboard.Location = new System.Drawing.Point(8, 40);
      this.rdoKeyboard.Name = "rdoKeyboard";
      this.rdoKeyboard.Size = new System.Drawing.Size(272, 16);
      this.rdoKeyboard.TabIndex = 40;
      this.rdoKeyboard.Text = "Keyboard";
      this.rdoKeyboard.CheckedChanged += new System.EventHandler(this.InputType_Changed);
      // 
      // txtButton4
      // 
      this.txtButton4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtButton4.Enabled = false;
      this.txtButton4.Location = new System.Drawing.Point(224, 128);
      this.txtButton4.Name = "txtButton4";
      this.txtButton4.ReadOnly = true;
      this.txtButton4.Size = new System.Drawing.Size(64, 20);
      this.txtButton4.TabIndex = 39;
      this.txtButton4.Text = "";
      // 
      // lblButton4
      // 
      this.lblButton4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblButton4.Enabled = false;
      this.lblButton4.Location = new System.Drawing.Point(160, 128);
      this.lblButton4.Name = "lblButton4";
      this.lblButton4.Size = new System.Drawing.Size(64, 20);
      this.lblButton4.TabIndex = 38;
      this.lblButton4.Text = "Button 4:";
      this.lblButton4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtButton3
      // 
      this.txtButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtButton3.Enabled = false;
      this.txtButton3.Location = new System.Drawing.Point(224, 104);
      this.txtButton3.Name = "txtButton3";
      this.txtButton3.ReadOnly = true;
      this.txtButton3.Size = new System.Drawing.Size(64, 20);
      this.txtButton3.TabIndex = 37;
      this.txtButton3.Text = "";
      // 
      // lblButton3
      // 
      this.lblButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblButton3.Enabled = false;
      this.lblButton3.Location = new System.Drawing.Point(160, 104);
      this.lblButton3.Name = "lblButton3";
      this.lblButton3.Size = new System.Drawing.Size(64, 20);
      this.lblButton3.TabIndex = 36;
      this.lblButton3.Text = "Button 3:";
      this.lblButton3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtButton2
      // 
      this.txtButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtButton2.Enabled = false;
      this.txtButton2.Location = new System.Drawing.Point(224, 80);
      this.txtButton2.Name = "txtButton2";
      this.txtButton2.ReadOnly = true;
      this.txtButton2.Size = new System.Drawing.Size(64, 20);
      this.txtButton2.TabIndex = 35;
      this.txtButton2.Text = "";
      // 
      // lblButton2
      // 
      this.lblButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblButton2.Enabled = false;
      this.lblButton2.Location = new System.Drawing.Point(160, 80);
      this.lblButton2.Name = "lblButton2";
      this.lblButton2.Size = new System.Drawing.Size(64, 20);
      this.lblButton2.TabIndex = 34;
      this.lblButton2.Text = "Button 2:";
      this.lblButton2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtButton1
      // 
      this.txtButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtButton1.Enabled = false;
      this.txtButton1.Location = new System.Drawing.Point(224, 56);
      this.txtButton1.Name = "txtButton1";
      this.txtButton1.ReadOnly = true;
      this.txtButton1.Size = new System.Drawing.Size(64, 20);
      this.txtButton1.TabIndex = 33;
      this.txtButton1.Text = "";
      // 
      // lblButton1
      // 
      this.lblButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblButton1.Enabled = false;
      this.lblButton1.Location = new System.Drawing.Point(160, 56);
      this.lblButton1.Name = "lblButton1";
      this.lblButton1.Size = new System.Drawing.Size(64, 20);
      this.lblButton1.TabIndex = 32;
      this.lblButton1.Text = "Button 1:";
      this.lblButton1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtDown
      // 
      this.txtDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtDown.Enabled = false;
      this.txtDown.Location = new System.Drawing.Point(88, 128);
      this.txtDown.Name = "txtDown";
      this.txtDown.ReadOnly = true;
      this.txtDown.Size = new System.Drawing.Size(64, 20);
      this.txtDown.TabIndex = 31;
      this.txtDown.Text = "";
      // 
      // lblDown
      // 
      this.lblDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblDown.Enabled = false;
      this.lblDown.Location = new System.Drawing.Point(24, 128);
      this.lblDown.Name = "lblDown";
      this.lblDown.Size = new System.Drawing.Size(64, 20);
      this.lblDown.TabIndex = 30;
      this.lblDown.Text = "Down:";
      this.lblDown.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtRight
      // 
      this.txtRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtRight.Enabled = false;
      this.txtRight.Location = new System.Drawing.Point(88, 104);
      this.txtRight.Name = "txtRight";
      this.txtRight.ReadOnly = true;
      this.txtRight.Size = new System.Drawing.Size(64, 20);
      this.txtRight.TabIndex = 29;
      this.txtRight.Text = "";
      // 
      // lblRight
      // 
      this.lblRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblRight.Enabled = false;
      this.lblRight.Location = new System.Drawing.Point(24, 104);
      this.lblRight.Name = "lblRight";
      this.lblRight.Size = new System.Drawing.Size(64, 20);
      this.lblRight.TabIndex = 28;
      this.lblRight.Text = "Right:";
      this.lblRight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtLeft
      // 
      this.txtLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtLeft.Enabled = false;
      this.txtLeft.Location = new System.Drawing.Point(88, 80);
      this.txtLeft.Name = "txtLeft";
      this.txtLeft.ReadOnly = true;
      this.txtLeft.Size = new System.Drawing.Size(64, 20);
      this.txtLeft.TabIndex = 27;
      this.txtLeft.Text = "";
      // 
      // lblLeft
      // 
      this.lblLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblLeft.Enabled = false;
      this.lblLeft.Location = new System.Drawing.Point(24, 80);
      this.lblLeft.Name = "lblLeft";
      this.lblLeft.Size = new System.Drawing.Size(64, 20);
      this.lblLeft.TabIndex = 26;
      this.lblLeft.Text = "Left:";
      this.lblLeft.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtUp
      // 
      this.txtUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtUp.Enabled = false;
      this.txtUp.Location = new System.Drawing.Point(88, 56);
      this.txtUp.Name = "txtUp";
      this.txtUp.ReadOnly = true;
      this.txtUp.Size = new System.Drawing.Size(64, 20);
      this.txtUp.TabIndex = 25;
      this.txtUp.Text = "";
      // 
      // lblUp
      // 
      this.lblUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblUp.Enabled = false;
      this.lblUp.Location = new System.Drawing.Point(24, 56);
      this.lblUp.Name = "lblUp";
      this.lblUp.Size = new System.Drawing.Size(64, 20);
      this.lblUp.TabIndex = 24;
      this.lblUp.Text = "Up:";
      this.lblUp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cboPlayer
      // 
      this.cboPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.cboPlayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboPlayer.Location = new System.Drawing.Point(88, 8);
      this.cboPlayer.Name = "cboPlayer";
      this.cboPlayer.Size = new System.Drawing.Size(200, 21);
      this.cboPlayer.TabIndex = 23;
      this.cboPlayer.SelectedIndexChanged += new System.EventHandler(this.cboPlayer_SelectedIndexChanged);
      // 
      // lblPlayer
      // 
      this.lblPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblPlayer.Location = new System.Drawing.Point(8, 8);
      this.lblPlayer.Name = "lblPlayer";
      this.lblPlayer.Size = new System.Drawing.Size(80, 21);
      this.lblPlayer.TabIndex = 22;
      this.lblPlayer.Text = "Player:";
      this.lblPlayer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtJButton4
      // 
      this.txtJButton4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtJButton4.Enabled = false;
      this.txtJButton4.Location = new System.Drawing.Point(224, 232);
      this.txtJButton4.Name = "txtJButton4";
      this.txtJButton4.ReadOnly = true;
      this.txtJButton4.Size = new System.Drawing.Size(64, 20);
      this.txtJButton4.TabIndex = 51;
      this.txtJButton4.Text = "";
      this.txtJButton4.Leave += new System.EventHandler(this.txtJButton_Leave);
      this.txtJButton4.Enter += new System.EventHandler(this.txtJButton_Enter);
      // 
      // txtJButton3
      // 
      this.txtJButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtJButton3.Enabled = false;
      this.txtJButton3.Location = new System.Drawing.Point(224, 208);
      this.txtJButton3.Name = "txtJButton3";
      this.txtJButton3.ReadOnly = true;
      this.txtJButton3.Size = new System.Drawing.Size(64, 20);
      this.txtJButton3.TabIndex = 49;
      this.txtJButton3.Text = "";
      this.txtJButton3.Leave += new System.EventHandler(this.txtJButton_Leave);
      this.txtJButton3.Enter += new System.EventHandler(this.txtJButton_Enter);
      // 
      // txtJButton2
      // 
      this.txtJButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtJButton2.Enabled = false;
      this.txtJButton2.Location = new System.Drawing.Point(88, 232);
      this.txtJButton2.Name = "txtJButton2";
      this.txtJButton2.ReadOnly = true;
      this.txtJButton2.Size = new System.Drawing.Size(64, 20);
      this.txtJButton2.TabIndex = 47;
      this.txtJButton2.Text = "";
      this.txtJButton2.Leave += new System.EventHandler(this.txtJButton_Leave);
      this.txtJButton2.Enter += new System.EventHandler(this.txtJButton_Enter);
      // 
      // txtJButton1
      // 
      this.txtJButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtJButton1.Enabled = false;
      this.txtJButton1.Location = new System.Drawing.Point(88, 208);
      this.txtJButton1.Name = "txtJButton1";
      this.txtJButton1.ReadOnly = true;
      this.txtJButton1.Size = new System.Drawing.Size(64, 20);
      this.txtJButton1.TabIndex = 45;
      this.txtJButton1.Text = "";
      this.txtJButton1.Leave += new System.EventHandler(this.txtJButton_Leave);
      this.txtJButton1.Enter += new System.EventHandler(this.txtJButton_Enter);
      // 
      // lblJButton4
      // 
      this.lblJButton4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblJButton4.Enabled = false;
      this.lblJButton4.Location = new System.Drawing.Point(160, 232);
      this.lblJButton4.Name = "lblJButton4";
      this.lblJButton4.Size = new System.Drawing.Size(64, 20);
      this.lblJButton4.TabIndex = 50;
      this.lblJButton4.Text = "Button 4:";
      this.lblJButton4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblJButton3
      // 
      this.lblJButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblJButton3.Enabled = false;
      this.lblJButton3.Location = new System.Drawing.Point(160, 208);
      this.lblJButton3.Name = "lblJButton3";
      this.lblJButton3.Size = new System.Drawing.Size(64, 20);
      this.lblJButton3.TabIndex = 48;
      this.lblJButton3.Text = "Button 3:";
      this.lblJButton3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblJButton2
      // 
      this.lblJButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblJButton2.Enabled = false;
      this.lblJButton2.Location = new System.Drawing.Point(24, 232);
      this.lblJButton2.Name = "lblJButton2";
      this.lblJButton2.Size = new System.Drawing.Size(64, 20);
      this.lblJButton2.TabIndex = 46;
      this.lblJButton2.Text = "Button 2:";
      this.lblJButton2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblJButton1
      // 
      this.lblJButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblJButton1.Enabled = false;
      this.lblJButton1.Location = new System.Drawing.Point(24, 208);
      this.lblJButton1.Name = "lblJButton1";
      this.lblJButton1.Size = new System.Drawing.Size(64, 20);
      this.lblJButton1.TabIndex = 44;
      this.lblJButton1.Text = "Button 1:";
      this.lblJButton1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
         | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.Location = new System.Drawing.Point(8, 264);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(288, 32);
      this.label1.TabIndex = 52;
      this.label1.Text = "To change button configuration, click on a box and press the key/button to map to" +
         " that button.";
      // 
      // frmControls
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(298, 295);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txtJButton4);
      this.Controls.Add(this.txtJButton3);
      this.Controls.Add(this.txtJButton2);
      this.Controls.Add(this.txtJButton1);
      this.Controls.Add(this.txtButton4);
      this.Controls.Add(this.txtButton3);
      this.Controls.Add(this.txtButton2);
      this.Controls.Add(this.txtButton1);
      this.Controls.Add(this.txtDown);
      this.Controls.Add(this.txtRight);
      this.Controls.Add(this.txtLeft);
      this.Controls.Add(this.txtUp);
      this.Controls.Add(this.lblJButton4);
      this.Controls.Add(this.lblJButton3);
      this.Controls.Add(this.lblJButton2);
      this.Controls.Add(this.lblJButton1);
      this.Controls.Add(this.lblController);
      this.Controls.Add(this.cboController);
      this.Controls.Add(this.rdoController);
      this.Controls.Add(this.rdoKeyboard);
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
            lblJButton1.Enabled = txtJButton1.Enabled =
            lblJButton2.Enabled = txtJButton2.Enabled =
            lblJButton3.Enabled = txtJButton3.Enabled =
            lblJButton4.Enabled = txtJButton4.Enabled =
            (sender != rdoKeyboard);

         if (!bLoading)
         {
            if (sender == rdoKeyboard)
               Project.GameWindow.Players[SelectedPlayer] = new KeyboardPlayer(SelectedPlayer);
            else
               Project.GameWindow.Players[SelectedPlayer] = new ControllerPlayer(SelectedPlayer % cboController.Items.Count);
         }

         LoadCurrentControls();
      }
   }

   private int SelectedPlayer
   {
      get
      {
         return cboPlayer.SelectedIndex;
      }
   }

   private void ProcessMessage(KeyTextBox sender, Message m)
   {
      Project.GameWindow.KeyboardState.ProcessMessage(m);
      if (m.Msg == WM_KEYDOWN)
      {
         KeyboardState kbs = Project.GameWindow.KeyboardState;
         sender.Text = System.Enum.Format(typeof(Key), kbs.GetFirstKey(), "g");
         KeyboardPlayer player = (KeyboardPlayer)Project.GameWindow.Players[SelectedPlayer];
         if (sender == txtUp)
            player.key_up = kbs.GetFirstKey();
         else if (sender == txtLeft)
            player.key_left = kbs.GetFirstKey();
         else if (sender == txtRight)
            player.key_right = kbs.GetFirstKey();
         else if (sender == txtDown)
            player.key_down = kbs.GetFirstKey();
         else if (sender == txtButton1)
            player.key_button1 = kbs.GetFirstKey();
         else if (sender == txtButton2)
            player.key_button2 = kbs.GetFirstKey();
         else if (sender == txtButton3)
            player.key_button3 = kbs.GetFirstKey();
         else if (sender == txtButton4)
            player.key_button4 = kbs.GetFirstKey();
      }
   }

   private void cboController_SelectedIndexChanged(object sender, System.EventArgs e)
   {
      if (cboController.SelectedIndex >= 0)
         ((ControllerPlayer)Project.GameWindow.Players[SelectedPlayer]).deviceNumber = cboController.SelectedIndex;
   }

   private void cboPlayer_SelectedIndexChanged(object sender, System.EventArgs e)
   {
      // Required for cross-thread access
      currentPlayer = SelectedPlayer;
      LoadCurrentControls();
   }

   private void LoadCurrentControls()
   {
      if (Project.GameWindow.Players[SelectedPlayer] is KeyboardPlayer)
      {
         bLoading = true;
         KeyboardPlayer player = (KeyboardPlayer)Project.GameWindow.Players[SelectedPlayer];
         rdoKeyboard.Checked = true;
         txtUp.Text = System.Enum.Format(typeof(Key), player.key_up, "g");
         txtLeft.Text = System.Enum.Format(typeof(Key), player.key_left, "g");
         txtRight.Text = System.Enum.Format(typeof(Key), player.key_right, "g");
         txtDown.Text = System.Enum.Format(typeof(Key), player.key_down, "g");
         txtButton1.Text = System.Enum.Format(typeof(Key), player.key_button1, "g");
         txtButton2.Text = System.Enum.Format(typeof(Key), player.key_button2, "g");
         txtButton3.Text = System.Enum.Format(typeof(Key), player.key_button3, "g");
         txtButton4.Text = System.Enum.Format(typeof(Key), player.key_button4, "g");
         bLoading = false;
      }
      else
      {
         bLoading = true;
         rdoController.Checked = true;
         ControllerPlayer player = ((ControllerPlayer)Project.GameWindow.Players[SelectedPlayer]);
         int devNum = player.deviceNumber;
         if (cboController.Items.Count > devNum)
         {
            cboController.SelectedIndex = devNum;
            txtJButton1.Text = player.buttonMap[0].ToString();
            txtJButton2.Text = player.buttonMap[1].ToString();
            txtJButton3.Text = player.buttonMap[2].ToString();
            txtJButton4.Text = player.buttonMap[3].ToString();
         }
         else
            cboController.SelectedIndex = -1;
         bLoading = false;
      }
   }

   private void ControllerButtonPressed(byte button)
   {
      ControllerPlayer plr = (ControllerPlayer)Project.GameWindow.Players[SelectedPlayer];
      if (txtCurJButton == txtJButton1)
         plr.buttonMap[0] = button;
      else if (txtCurJButton == txtJButton2)
         plr.buttonMap[1] = button;
      else if (txtCurJButton == txtJButton3)
         plr.buttonMap[2] = button;
      else if (txtCurJButton == txtJButton4)
         plr.buttonMap[3] = button;
      if (txtCurJButton != null)
         txtCurJButton.Text = button.ToString();
   }

   private void ReadControllerLoop()
   {
      ControllerButtonPressedDelegate cbp = new ControllerButtonPressedDelegate(ControllerButtonPressed);
      while (!bExitControllerThread)
      {
         Project.GameWindow.ReadControllers();
         for (byte button = 0; button < 32; button++)
         {
            if (((pressedButtons & (1 << button)) == 0) &&
               (Project.GameWindow.GetControllerState(((ControllerPlayer)Project.GameWindow.Players[
            currentPlayer]).deviceNumber)[button]))
            {
               this.Invoke(cbp, new object[] { button });
               pressedButtons |= (1 << button);
            }
            else
               pressedButtons &= ~(1 << button);
         }
         System.Threading.Thread.Sleep(0);
      }
   }

   private void BeginControllerThread()
   {
      EndControllerThread();
      System.Threading.ThreadStart ts = new System.Threading.ThreadStart(ReadControllerLoop);
      readControllerThread = new System.Threading.Thread(ts);
      readControllerThread.Start();
   }

   private void EndControllerThread()
   {
      if (readControllerThread != null)
      {
         bExitControllerThread = true;
         readControllerThread.Join();
      }
      readControllerThread = null;
      bExitControllerThread = false;
   }

   private void txtJButton_Enter(object sender, System.EventArgs e)
   {
      txtCurJButton = (System.Windows.Forms.TextBox)sender;
      BeginControllerThread();
   }

   private void txtJButton_Leave(object sender, System.EventArgs e)
   {
      txtCurJButton = null;
      EndControllerThread();
   }
}

