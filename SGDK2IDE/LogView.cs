/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for LogView.
	/// </summary>
	public class frmLogView : System.Windows.Forms.Form
	{
      private System.Windows.Forms.TextBox txtLogView;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmLogView(string text)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

			txtLogView.Text = text;
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
         this.txtLogView = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // txtLogView
         // 
         this.txtLogView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtLogView.Location = new System.Drawing.Point(0, 0);
         this.txtLogView.Multiline = true;
         this.txtLogView.Name = "txtLogView";
         this.txtLogView.ReadOnly = true;
         this.txtLogView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtLogView.Size = new System.Drawing.Size(440, 261);
         this.txtLogView.TabIndex = 0;
         this.txtLogView.Text = "";
         // 
         // frmLogView
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(440, 261);
         this.Controls.Add(this.txtLogView);
         this.Name = "frmLogView";
         this.Text = "Log Viewer";
         this.ResumeLayout(false);

      }
		#endregion
	
      #region Overrides
      protected override void OnClosing(CancelEventArgs e)
      {
         base.OnClosing (e);
         SGDK2IDE.SaveFormSettings(this);
      }

      #endregion
   }
}
