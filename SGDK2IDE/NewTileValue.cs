using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmNewTileValue : System.Windows.Forms.Form
	{
      private System.Windows.Forms.Label lblPrompt;
      private System.Windows.Forms.Label lblTileValue;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.NumericUpDown updTileValue;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmNewTileValue()
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
			this.lblPrompt = new System.Windows.Forms.Label();
			this.lblTileValue = new System.Windows.Forms.Label();
			this.updTileValue = new System.Windows.Forms.NumericUpDown();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.updTileValue)).BeginInit();
			this.SuspendLayout();
			// 
			// lblPrompt
			// 
			this.lblPrompt.Location = new System.Drawing.Point(16, 8);
			this.lblPrompt.Name = "lblPrompt";
			this.lblPrompt.Size = new System.Drawing.Size(320, 48);
			this.lblPrompt.TabIndex = 0;
			this.lblPrompt.Text = "Enter a tile value to be defined in the Tileset editor.  This value may exceed th" +
				"e number of frames in the Frameset; if it doesn\'t, the value will no longer refe" +
				"r to the specified frame index.";
			// 
			// lblTileValue
			// 
			this.lblTileValue.Location = new System.Drawing.Point(16, 64);
			this.lblTileValue.Name = "lblTileValue";
			this.lblTileValue.Size = new System.Drawing.Size(80, 20);
			this.lblTileValue.TabIndex = 1;
			this.lblTileValue.Text = "Tile Value:";
			// 
			// updTileValue
			// 
			this.updTileValue.Location = new System.Drawing.Point(96, 64);
			this.updTileValue.Maximum = new System.Decimal(new int[] {
																		 2147483647,
																		 0,
																		 0,
																		 0});
			this.updTileValue.Name = "updTileValue";
			this.updTileValue.Size = new System.Drawing.Size(88, 20);
			this.updTileValue.TabIndex = 2;
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(344, 8);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(72, 24);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "OK";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(344, 40);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(72, 24);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			// 
			// frmNewTileValue
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(426, 95);
			this.ControlBox = false;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.updTileValue);
			this.Controls.Add(this.lblTileValue);
			this.Controls.Add(this.lblPrompt);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "frmNewTileValue";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Enter New Tile Value";
			((System.ComponentModel.ISupportInitialize)(this.updTileValue)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

      public static int PromptForNewTileValue(IWin32Window owner)
      {
         frmNewTileValue frm = new frmNewTileValue();
         if (DialogResult.OK == frm.ShowDialog(owner))
            return (int)frm.updTileValue.Value;
         else
            return -1;
      }
	}
}
