using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for About.
	/// </summary>
	public class frmAbout : System.Windows.Forms.Form
	{
      private System.Windows.Forms.Label lblVersion;
      private System.Windows.Forms.Button btnClose;
      private System.Windows.Forms.LinkLabel llbHomepage;
      private System.Windows.Forms.Label lblHomepage;
      private System.Windows.Forms.Label lblLicense;
      private System.Windows.Forms.LinkLabel llbLicense;
      private System.Windows.Forms.Label lblTitle;
      private System.Windows.Forms.Label lblCopyright;
      private System.Windows.Forms.TextBox txtCredits;
      private System.Windows.Forms.GroupBox grpCredits;
      private System.Windows.Forms.PictureBox picLogo;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.TextBox txtModules;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmAbout()
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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmAbout));
         this.lblTitle = new System.Windows.Forms.Label();
         this.lblVersion = new System.Windows.Forms.Label();
         this.btnClose = new System.Windows.Forms.Button();
         this.lblCopyright = new System.Windows.Forms.Label();
         this.llbHomepage = new System.Windows.Forms.LinkLabel();
         this.lblHomepage = new System.Windows.Forms.Label();
         this.lblLicense = new System.Windows.Forms.Label();
         this.llbLicense = new System.Windows.Forms.LinkLabel();
         this.txtCredits = new System.Windows.Forms.TextBox();
         this.grpCredits = new System.Windows.Forms.GroupBox();
         this.picLogo = new System.Windows.Forms.PictureBox();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.txtModules = new System.Windows.Forms.TextBox();
         this.grpCredits.SuspendLayout();
         this.groupBox1.SuspendLayout();
         this.SuspendLayout();
         // 
         // lblTitle
         // 
         this.lblTitle.BackColor = System.Drawing.Color.Transparent;
         this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
         this.lblTitle.Location = new System.Drawing.Point(72, 8);
         this.lblTitle.Name = "lblTitle";
         this.lblTitle.Size = new System.Drawing.Size(344, 32);
         this.lblTitle.TabIndex = 0;
         this.lblTitle.Text = "Scrolling Game Development Kit";
         // 
         // lblVersion
         // 
         this.lblVersion.BackColor = System.Drawing.Color.Transparent;
         this.lblVersion.Location = new System.Drawing.Point(72, 40);
         this.lblVersion.Name = "lblVersion";
         this.lblVersion.Size = new System.Drawing.Size(344, 16);
         this.lblVersion.TabIndex = 1;
         // 
         // btnClose
         // 
         this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnClose.Location = new System.Drawing.Point(336, 288);
         this.btnClose.Name = "btnClose";
         this.btnClose.Size = new System.Drawing.Size(72, 24);
         this.btnClose.TabIndex = 2;
         this.btnClose.Text = "Close";
         // 
         // lblCopyright
         // 
         this.lblCopyright.BackColor = System.Drawing.Color.Transparent;
         this.lblCopyright.Location = new System.Drawing.Point(72, 56);
         this.lblCopyright.Name = "lblCopyright";
         this.lblCopyright.Size = new System.Drawing.Size(344, 24);
         this.lblCopyright.TabIndex = 3;
         this.lblCopyright.Text = "Copyright © 2000-2006 Benjamin Marty (GPL)";
         // 
         // llbHomepage
         // 
         this.llbHomepage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.llbHomepage.BackColor = System.Drawing.Color.Transparent;
         this.llbHomepage.Location = new System.Drawing.Point(192, 248);
         this.llbHomepage.Name = "llbHomepage";
         this.llbHomepage.Size = new System.Drawing.Size(224, 16);
         this.llbHomepage.TabIndex = 4;
         this.llbHomepage.TabStop = true;
         this.llbHomepage.Text = "http://sgdk2.sf.net/";
         this.llbHomepage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Link_Clicked);
         // 
         // lblHomepage
         // 
         this.lblHomepage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblHomepage.BackColor = System.Drawing.Color.Transparent;
         this.lblHomepage.Location = new System.Drawing.Point(72, 248);
         this.lblHomepage.Name = "lblHomepage";
         this.lblHomepage.Size = new System.Drawing.Size(120, 16);
         this.lblHomepage.TabIndex = 5;
         this.lblHomepage.Text = "SGDK2 Homepage:";
         // 
         // lblLicense
         // 
         this.lblLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblLicense.BackColor = System.Drawing.Color.Transparent;
         this.lblLicense.Location = new System.Drawing.Point(72, 264);
         this.lblLicense.Name = "lblLicense";
         this.lblLicense.Size = new System.Drawing.Size(120, 16);
         this.lblLicense.TabIndex = 6;
         this.lblLicense.Text = "GPL License Details:";
         // 
         // llbLicense
         // 
         this.llbLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.llbLicense.BackColor = System.Drawing.Color.Transparent;
         this.llbLicense.Location = new System.Drawing.Point(192, 264);
         this.llbLicense.Name = "llbLicense";
         this.llbLicense.Size = new System.Drawing.Size(224, 16);
         this.llbLicense.TabIndex = 7;
         this.llbLicense.TabStop = true;
         this.llbLicense.Text = "http://www.gnu.org/licenses/gpl.html";
         this.llbLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Link_Clicked);
         // 
         // txtCredits
         // 
         this.txtCredits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtCredits.Location = new System.Drawing.Point(8, 16);
         this.txtCredits.Multiline = true;
         this.txtCredits.Name = "txtCredits";
         this.txtCredits.ReadOnly = true;
         this.txtCredits.Size = new System.Drawing.Size(328, 48);
         this.txtCredits.TabIndex = 8;
         this.txtCredits.Text = "Primary design and development: Benjamin Marty\r\nGraphics: Jeff Cruz\r\nAlpha testin" +
            "g and design input: Seth Marty";
         // 
         // grpCredits
         // 
         this.grpCredits.Controls.Add(this.txtCredits);
         this.grpCredits.Location = new System.Drawing.Point(72, 80);
         this.grpCredits.Name = "grpCredits";
         this.grpCredits.Size = new System.Drawing.Size(344, 72);
         this.grpCredits.TabIndex = 9;
         this.grpCredits.TabStop = false;
         this.grpCredits.Text = "Credits";
         // 
         // picLogo
         // 
         this.picLogo.Image = ((System.Drawing.Image)(resources.GetObject("picLogo.Image")));
         this.picLogo.Location = new System.Drawing.Point(0, 0);
         this.picLogo.Name = "picLogo";
         this.picLogo.Size = new System.Drawing.Size(72, 72);
         this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
         this.picLogo.TabIndex = 10;
         this.picLogo.TabStop = false;
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.txtModules);
         this.groupBox1.Location = new System.Drawing.Point(72, 160);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(344, 72);
         this.groupBox1.TabIndex = 11;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Loaded Modules";
         // 
         // txtModules
         // 
         this.txtModules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtModules.Location = new System.Drawing.Point(8, 16);
         this.txtModules.Multiline = true;
         this.txtModules.Name = "txtModules";
         this.txtModules.ReadOnly = true;
         this.txtModules.ScrollBars = System.Windows.Forms.ScrollBars.Both;
         this.txtModules.Size = new System.Drawing.Size(328, 48);
         this.txtModules.TabIndex = 0;
         this.txtModules.Text = "";
         // 
         // frmAbout
         // 
         this.AcceptButton = this.btnClose;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnClose;
         this.ClientSize = new System.Drawing.Size(426, 319);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.picLogo);
         this.Controls.Add(this.grpCredits);
         this.Controls.Add(this.llbLicense);
         this.Controls.Add(this.lblLicense);
         this.Controls.Add(this.lblHomepage);
         this.Controls.Add(this.llbHomepage);
         this.Controls.Add(this.lblCopyright);
         this.Controls.Add(this.btnClose);
         this.Controls.Add(this.lblVersion);
         this.Controls.Add(this.lblTitle);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.Name = "frmAbout";
         this.Text = "About...";
         this.grpCredits.ResumeLayout(false);
         this.groupBox1.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion

      protected override void OnLoad(EventArgs e)
      {
         lblVersion.Text = "Version " + Application.ProductVersion + " alpha release 2";
         base.OnLoad (e);

         System.Text.StringBuilder sb = new System.Text.StringBuilder();
         foreach(System.Diagnostics.ProcessModule mod in System.Diagnostics.Process.GetCurrentProcess().Modules)
         {
            sb.Append(string.Format("{0} ({1})\r\n", mod.FileName, mod.FileVersionInfo.FileVersion));
         }
         txtModules.Text = sb.ToString();
      }

      private void Link_Clicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
      {
         System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
      }

   }
}
