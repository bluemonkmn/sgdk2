using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Implements a dialog class to prompt the user with information, and which
	/// can be disabled.
	/// </summary>
	public class frmSwitchableMessage : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
      private System.Windows.Forms.Label lblMessage;
      private System.Windows.Forms.CheckBox chkSwitch;
      private System.Windows.Forms.Button btn1;
      private System.Windows.Forms.Button btn2;
      private System.Windows.Forms.Button btn3;

		public frmSwitchableMessage()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
         this.lblMessage = new System.Windows.Forms.Label();
         this.chkSwitch = new System.Windows.Forms.CheckBox();
         this.btn1 = new System.Windows.Forms.Button();
         this.btn2 = new System.Windows.Forms.Button();
         this.btn3 = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // lblMessage
         // 
         this.lblMessage.Location = new System.Drawing.Point(8, 8);
         this.lblMessage.Size = new System.Drawing.Size(400, 64);
         this.lblMessage.TabIndex = 0;
         // 
         // chkSwitch
         // 
         this.chkSwitch.Location = new System.Drawing.Point(8, 72);
         this.chkSwitch.Size = new System.Drawing.Size(400, 16);
         this.chkSwitch.TabIndex = 1;
         this.chkSwitch.Text = "Don\'t display this message again";
         // 
         // btn1
         // 
         this.btn1.Location = new System.Drawing.Point(176, 96);
         this.btn1.Size = new System.Drawing.Size(72, 24);
         this.btn1.TabIndex = 2;
         this.btn1.Visible = false;
         // 
         // btn2
         // 
         this.btn2.Location = new System.Drawing.Point(256, 96);
         this.btn2.Size = new System.Drawing.Size(72, 24);
         this.btn2.TabIndex = 3;
         this.btn2.Visible = false;
         // 
         // btn3
         // 
         this.btn3.Location = new System.Drawing.Point(336, 96);
         this.btn3.Size = new System.Drawing.Size(72, 24);
         this.btn3.TabIndex = 4;
         // 
         // frmSwitchableMessage
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(416, 125);
         this.ControlBox = false;
         this.Controls.Add(this.btn3);
         this.Controls.Add(this.btn2);
         this.Controls.Add(this.btn1);
         this.Controls.Add(this.chkSwitch);
         this.Controls.Add(this.lblMessage);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.Name = "frmSwitchableMessage";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.ResumeLayout(false);
      }
		#endregion

      public static System.Windows.Forms.DialogResult GetResult(string switchName, DialogResult defaultResult, string message, string title, MessageBoxButtons buttons, IWin32Window owner)
      {
         string sval = SGDK2IDE.GetUserOption(switchName);
         if ((sval == null) || (sval != "0"))
         {
            frmSwitchableMessage frm = new frmSwitchableMessage();
            frm.lblMessage.Text = message;
            frm.Text = title;
            switch(buttons)
            {
               case MessageBoxButtons.OK:
                  frm.btn3.DialogResult = DialogResult.OK;
                  frm.AcceptButton = frm.btn3;
                  frm.btn3.Text = "OK";
                  break;
               case MessageBoxButtons.OKCancel:
                  frm.btn2.DialogResult = DialogResult.OK;
                  frm.AcceptButton = frm.btn2;
                  frm.btn2.Text = "OK";
                  frm.btn2.Visible = true;
                  frm.btn3.DialogResult = DialogResult.Cancel;
                  frm.btn3.Text = "Cancel";
                  frm.CancelButton = frm.btn3;
                  break;
               case MessageBoxButtons.RetryCancel:
                  frm.btn2.DialogResult = DialogResult.Retry;
                  frm.AcceptButton = frm.btn2;
                  frm.btn2.Text = "Retry";
                  frm.btn2.Visible = true;
                  frm.btn3.DialogResult = DialogResult.Cancel;
                  frm.btn3.Text = "Cancel";
                  frm.CancelButton = frm.btn3;
                  break;
               case MessageBoxButtons.YesNo:
                  frm.btn2.DialogResult = DialogResult.Yes;
                  frm.btn2.Text = "Yes";
                  frm.btn2.Visible = true;
                  frm.btn3.DialogResult = DialogResult.No;
                  frm.btn3.Text = "No";
                  if (defaultResult == DialogResult.Yes)
                     frm.AcceptButton = frm.btn2;
                  else if (defaultResult == DialogResult.No)
                     frm.AcceptButton = frm.btn3;
                  break;
               case MessageBoxButtons.YesNoCancel:
                  frm.btn1.DialogResult = DialogResult.Yes;
                  frm.btn1.Text = "Yes";
                  frm.btn1.Visible = true;
                  frm.btn2.DialogResult = DialogResult.No;
                  frm.btn2.Text = "No";
                  frm.btn2.Visible = true;
                  frm.btn3.DialogResult = DialogResult.Cancel;
                  frm.btn3.Text = "Cancel";
                  frm.CancelButton = frm.btn3;
                  if (defaultResult == DialogResult.Yes)
                     frm.AcceptButton = frm.btn1;
                  else if (defaultResult == DialogResult.No)
                     frm.AcceptButton = frm.btn2;
                  break;
               case MessageBoxButtons.AbortRetryIgnore:
                  frm.btn1.DialogResult = DialogResult.Abort;
                  frm.btn1.Text = "Abort";
                  frm.btn1.Visible = true;
                  frm.btn2.DialogResult = DialogResult.Retry;
                  frm.btn2.Text = "Retry";
                  frm.btn2.Visible = true;
                  frm.btn3.DialogResult = DialogResult.Ignore;
                  frm.btn3.Text = "Ignore";
                  frm.CancelButton = frm.btn1;
                  if (defaultResult == DialogResult.Retry)
                     frm.AcceptButton = frm.btn2;
                  else if (defaultResult == DialogResult.Ignore)
                     frm.AcceptButton = frm.btn3;
                  break;
            }
            DialogResult result = frm.ShowDialog(owner);
            if (frm.chkSwitch.Checked)
               SGDK2IDE.SetUserOption(switchName, "0");
            return result;
         }
         else
            return defaultResult;
      }
   }
}