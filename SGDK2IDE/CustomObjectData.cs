using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for CustomObjectData.
	/// </summary>
	public class frmCustomObjectData : System.Windows.Forms.Form
	{
      private System.Windows.Forms.TextBox txtData;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private frmCustomObjectData(string source)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         txtData.Text = source;
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
         this.txtData = new System.Windows.Forms.TextBox();
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // txtData
         // 
         this.txtData.AcceptsReturn = true;
         this.txtData.Dock = System.Windows.Forms.DockStyle.Top;
         this.txtData.Location = new System.Drawing.Point(0, 0);
         this.txtData.MaxLength = 99999999;
         this.txtData.Multiline = true;
         this.txtData.Name = "txtData";
         this.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtData.Size = new System.Drawing.Size(474, 192);
         this.txtData.TabIndex = 0;
         this.txtData.Text = "";
         // 
         // btnOK
         // 
         this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnOK.Location = new System.Drawing.Point(312, 200);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(72, 24);
         this.btnOK.TabIndex = 1;
         this.btnOK.Text = "&OK";
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(392, 200);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(72, 24);
         this.btnCancel.TabIndex = 2;
         this.btnCancel.Text = "Cancel";
         // 
         // frmCustomObjectData
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(474, 231);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.txtData);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.Name = "frmCustomObjectData";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Edit Custom Object Data As Text";
         this.ResumeLayout(false);

      }
		#endregion
	
      public static string EditText(IWin32Window owner, string source)
      {
         frmCustomObjectData frm = new frmCustomObjectData(source);
         try
         {
            if (DialogResult.OK == frm.ShowDialog(owner))
               return frm.txtData.Text;
            else
               return null;
         }
         finally
         {
            frm.Dispose();
         }
      }
   }
}
