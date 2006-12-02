using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

/// <summary>
/// Summary description for About.
/// </summary>
public class frmAbout : System.Windows.Forms.Form
{
   private System.Windows.Forms.Button btnOK;
   private System.Windows.Forms.Label lblHeader;
   private System.Windows.Forms.Label lblSGDK2;
   private System.Windows.Forms.LinkLabel llbURL;
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
      this.btnOK = new System.Windows.Forms.Button();
      this.lblHeader = new System.Windows.Forms.Label();
      this.lblSGDK2 = new System.Windows.Forms.Label();
      this.llbURL = new System.Windows.Forms.LinkLabel();
      this.SuspendLayout();
      // 
      // btnOK
      // 
      this.btnOK.Location = new System.Drawing.Point(208, 72);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(72, 24);
      this.btnOK.TabIndex = 0;
      this.btnOK.Text = "OK";
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      // 
      // lblHeader
      // 
      this.lblHeader.Location = new System.Drawing.Point(8, 8);
      this.lblHeader.Name = "lblHeader";
      this.lblHeader.Size = new System.Drawing.Size(272, 16);
      this.lblHeader.TabIndex = 1;
      this.lblHeader.Text = "This game was created with:";
      // 
      // lblSGDK2
      // 
      this.lblSGDK2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.lblSGDK2.Location = new System.Drawing.Point(8, 24);
      this.lblSGDK2.Name = "lblSGDK2";
      this.lblSGDK2.Size = new System.Drawing.Size(272, 16);
      this.lblSGDK2.TabIndex = 2;
      this.lblSGDK2.Text = "Scrolling Game Development Kit 2";
      this.lblSGDK2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // llbURL
      // 
      this.llbURL.Location = new System.Drawing.Point(8, 56);
      this.llbURL.Name = "llbURL";
      this.llbURL.Size = new System.Drawing.Size(272, 16);
      this.llbURL.TabIndex = 3;
      this.llbURL.TabStop = true;
      this.llbURL.Text = "http://sgdk2.sf.net/";
      this.llbURL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Link_Clicked);
      // 
      // frmAbout
      // 
      this.AcceptButton = this.btnOK;
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(292, 111);
      this.Controls.Add(this.llbURL);
      this.Controls.Add(this.lblSGDK2);
      this.Controls.Add(this.lblHeader);
      this.Controls.Add(this.btnOK);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "frmAbout";
      this.Text = "About " + System.Windows.Forms.Application.ProductName;
      this.ResumeLayout(false);

   }
	#endregion

   private void Link_Clicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
   {
      System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
   }
}
