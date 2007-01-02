using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for InputBox.
	/// </summary>
	public class frmInputBox : System.Windows.Forms.Form
	{
      private System.Windows.Forms.TextBox txtInput;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Label lblPrompt;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private frmInputBox()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
         this.txtInput = new System.Windows.Forms.TextBox();
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.lblPrompt = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // txtInput
         // 
         this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtInput.Location = new System.Drawing.Point(8, 24);
         this.txtInput.Name = "txtInput";
         this.txtInput.Size = new System.Drawing.Size(298, 20);
         this.txtInput.TabIndex = 0;
         this.txtInput.Text = "";
         // 
         // btnOK
         // 
         this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnOK.Location = new System.Drawing.Point(152, 56);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(72, 24);
         this.btnOK.TabIndex = 1;
         this.btnOK.Text = "OK";
         // 
         // btnCancel
         // 
         this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(232, 56);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(72, 24);
         this.btnCancel.TabIndex = 2;
         this.btnCancel.Text = "Cancel";
         // 
         // lblPrompt
         // 
         this.lblPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.lblPrompt.Location = new System.Drawing.Point(8, 0);
         this.lblPrompt.Name = "lblPrompt";
         this.lblPrompt.Size = new System.Drawing.Size(296, 24);
         this.lblPrompt.TabIndex = 3;
         this.lblPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // frmInputBox
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(314, 87);
         this.Controls.Add(this.lblPrompt);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.txtInput);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.Name = "frmInputBox";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.ResumeLayout(false);

      }
		#endregion

      public static string GetInput(IWin32Window parent, string Title, string Prompt, string Default)
      {
         frmInputBox frm = new frmInputBox();
         try
         {
            frm.Text = Title;
            frm.lblPrompt.Text = Prompt;
            frm.txtInput.Text = Default;
            SizeF promptSize;
            using (Graphics gfx = Graphics.FromHwnd(frm.Handle))
               promptSize = gfx.MeasureString(Prompt, frm.lblPrompt.Font, frm.lblPrompt.ClientSize.Width);
            frm.Height = (int)(frm.Height - frm.lblPrompt.Height + promptSize.Height);
            if (DialogResult.OK == frm.ShowDialog(parent))
               return frm.txtInput.Text;
            return null;
         }
         finally
         {
            frm.Dispose();
         }
      }
   }
}
