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
/// About dialog displayed from game's help menu.
/// </summary>
public partial class frmAbout : System.Windows.Forms.Form
{
   private System.Windows.Forms.Button btnOK;
   private System.Windows.Forms.Label lblHeader;
   private System.Windows.Forms.Label lblSGDK2;
   private System.Windows.Forms.LinkLabel llbURL;
   private System.Windows.Forms.Label lblCredits;
   private System.Windows.Forms.TextBox txtCredits;
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

      txtCredits.Text = Project.GameCredits;
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
      this.btnOK = new System.Windows.Forms.Button();
      this.lblHeader = new System.Windows.Forms.Label();
      this.lblSGDK2 = new System.Windows.Forms.Label();
      this.llbURL = new System.Windows.Forms.LinkLabel();
      this.lblCredits = new System.Windows.Forms.Label();
      this.txtCredits = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // btnOK
      // 
      this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOK.Location = new System.Drawing.Point(206, 168);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(72, 24);
      this.btnOK.TabIndex = 0;
      this.btnOK.Text = "OK";
      // 
      // lblHeader
      // 
      this.lblHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
         | System.Windows.Forms.AnchorStyles.Right)));
      this.lblHeader.Location = new System.Drawing.Point(8, 8);
      this.lblHeader.Name = "lblHeader";
      this.lblHeader.Size = new System.Drawing.Size(272, 16);
      this.lblHeader.TabIndex = 1;
      this.lblHeader.Text = "This game was created with:";
      // 
      // lblSGDK2
      // 
      this.lblSGDK2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
         | System.Windows.Forms.AnchorStyles.Right)));
      this.lblSGDK2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.lblSGDK2.Location = new System.Drawing.Point(8, 24);
      this.lblSGDK2.Name = "lblSGDK2";
      this.lblSGDK2.Size = new System.Drawing.Size(272, 16);
      this.lblSGDK2.TabIndex = 2;
      this.lblSGDK2.Text = "Scrolling Game Development Kit 2";
      this.lblSGDK2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // llbURL
      // 
      this.llbURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
         | System.Windows.Forms.AnchorStyles.Right)));
      this.llbURL.Location = new System.Drawing.Point(8, 40);
      this.llbURL.Name = "llbURL";
      this.llbURL.Size = new System.Drawing.Size(272, 16);
      this.llbURL.TabIndex = 3;
      this.llbURL.TabStop = true;
      this.llbURL.Text = "http://sgdk2.sf.net/";
      this.llbURL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Link_Clicked);
      // 
      // lblCredits
      // 
      this.lblCredits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
         | System.Windows.Forms.AnchorStyles.Right)));
      this.lblCredits.Location = new System.Drawing.Point(8, 64);
      this.lblCredits.Name = "lblCredits";
      this.lblCredits.Size = new System.Drawing.Size(272, 16);
      this.lblCredits.TabIndex = 4;
      this.lblCredits.Text = "Game Credits:";
      // 
      // txtCredits
      // 
      this.txtCredits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
         | System.Windows.Forms.AnchorStyles.Left) 
         | System.Windows.Forms.AnchorStyles.Right)));
      this.txtCredits.Location = new System.Drawing.Point(8, 80);
      this.txtCredits.Multiline = true;
      this.txtCredits.Name = "txtCredits";
      this.txtCredits.ReadOnly = true;
      this.txtCredits.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtCredits.Size = new System.Drawing.Size(270, 80);
      this.txtCredits.TabIndex = 5;
      this.txtCredits.Text = "";
      // 
      // frmAbout
      // 
      this.AcceptButton = this.btnOK;
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(290, 199);
      this.Controls.Add(this.txtCredits);
      this.Controls.Add(this.lblCredits);
      this.Controls.Add(this.llbURL);
      this.Controls.Add(this.lblSGDK2);
      this.Controls.Add(this.lblHeader);
      this.Controls.Add(this.btnOK);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "frmAbout";
      this.Text = "About ";
      this.ResumeLayout(false);

   }
	#endregion

   private void Link_Clicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
   {
      System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
   }
}
