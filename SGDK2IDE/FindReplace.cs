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
	/// Summary description for FindReplace.
	/// </summary>
	public class frmFindReplace : System.Windows.Forms.Form
	{
      private System.Windows.Forms.Label lblSearchFor;
      private System.Windows.Forms.TextBox txtSearchFor;
      private System.Windows.Forms.TextBox txtReplaceWith;
      private System.Windows.Forms.Label lblReplaceWith;
      private System.Windows.Forms.Button btnFindNext;
      private System.Windows.Forms.Button btnReplace;
      private System.Windows.Forms.Button btnReplaceAll;
      private System.Windows.Forms.CheckBox chkMatchCase;
      private System.Windows.Forms.CheckBox chkWholeWord;
      private System.Windows.Forms.CheckBox chkSearchUp;
      private System.Windows.Forms.Button btnClose;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmFindReplace(frmCodeEditor owner, bool replace)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         this.Owner = owner;
         SetMode(replace);
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
         this.lblSearchFor = new System.Windows.Forms.Label();
         this.txtSearchFor = new System.Windows.Forms.TextBox();
         this.txtReplaceWith = new System.Windows.Forms.TextBox();
         this.lblReplaceWith = new System.Windows.Forms.Label();
         this.btnFindNext = new System.Windows.Forms.Button();
         this.btnReplace = new System.Windows.Forms.Button();
         this.btnReplaceAll = new System.Windows.Forms.Button();
         this.chkMatchCase = new System.Windows.Forms.CheckBox();
         this.chkWholeWord = new System.Windows.Forms.CheckBox();
         this.chkSearchUp = new System.Windows.Forms.CheckBox();
         this.btnClose = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // lblSearchFor
         // 
         this.lblSearchFor.Location = new System.Drawing.Point(8, 8);
         this.lblSearchFor.Name = "lblSearchFor";
         this.lblSearchFor.Size = new System.Drawing.Size(88, 20);
         this.lblSearchFor.TabIndex = 0;
         this.lblSearchFor.Text = "&Search for:";
         this.lblSearchFor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtSearchFor
         // 
         this.txtSearchFor.Location = new System.Drawing.Point(96, 8);
         this.txtSearchFor.Name = "txtSearchFor";
         this.txtSearchFor.Size = new System.Drawing.Size(192, 20);
         this.txtSearchFor.TabIndex = 1;
         this.txtSearchFor.Text = "";
         // 
         // txtReplaceWith
         // 
         this.txtReplaceWith.Location = new System.Drawing.Point(96, 56);
         this.txtReplaceWith.Name = "txtReplaceWith";
         this.txtReplaceWith.Size = new System.Drawing.Size(192, 20);
         this.txtReplaceWith.TabIndex = 6;
         this.txtReplaceWith.Text = "";
         this.txtReplaceWith.Visible = false;
         // 
         // lblReplaceWith
         // 
         this.lblReplaceWith.Location = new System.Drawing.Point(8, 56);
         this.lblReplaceWith.Name = "lblReplaceWith";
         this.lblReplaceWith.Size = new System.Drawing.Size(88, 20);
         this.lblReplaceWith.TabIndex = 5;
         this.lblReplaceWith.Text = "Re&place with:";
         this.lblReplaceWith.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         this.lblReplaceWith.Visible = false;
         // 
         // btnFindNext
         // 
         this.btnFindNext.Location = new System.Drawing.Point(296, 8);
         this.btnFindNext.Name = "btnFindNext";
         this.btnFindNext.Size = new System.Drawing.Size(80, 24);
         this.btnFindNext.TabIndex = 7;
         this.btnFindNext.Text = "&Find Next";
         this.btnFindNext.Click += new System.EventHandler(this.btnFindNext_Click);
         // 
         // btnReplace
         // 
         this.btnReplace.Location = new System.Drawing.Point(296, 40);
         this.btnReplace.Name = "btnReplace";
         this.btnReplace.Size = new System.Drawing.Size(80, 24);
         this.btnReplace.TabIndex = 8;
         this.btnReplace.Text = "&Replace";
         this.btnReplace.Visible = false;
         this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
         // 
         // btnReplaceAll
         // 
         this.btnReplaceAll.Location = new System.Drawing.Point(296, 72);
         this.btnReplaceAll.Name = "btnReplaceAll";
         this.btnReplaceAll.Size = new System.Drawing.Size(80, 24);
         this.btnReplaceAll.TabIndex = 9;
         this.btnReplaceAll.Text = "Replace &All";
         this.btnReplaceAll.Visible = false;
         this.btnReplaceAll.Click += new System.EventHandler(this.btnReplaceAll_Click);
         // 
         // chkMatchCase
         // 
         this.chkMatchCase.Location = new System.Drawing.Point(8, 32);
         this.chkMatchCase.Name = "chkMatchCase";
         this.chkMatchCase.Size = new System.Drawing.Size(88, 16);
         this.chkMatchCase.TabIndex = 2;
         this.chkMatchCase.Text = "&Match case";
         // 
         // chkWholeWord
         // 
         this.chkWholeWord.Location = new System.Drawing.Point(96, 32);
         this.chkWholeWord.Name = "chkWholeWord";
         this.chkWholeWord.Size = new System.Drawing.Size(96, 16);
         this.chkWholeWord.TabIndex = 3;
         this.chkWholeWord.Text = "&Whole word";
         // 
         // chkSearchUp
         // 
         this.chkSearchUp.Location = new System.Drawing.Point(192, 32);
         this.chkSearchUp.Name = "chkSearchUp";
         this.chkSearchUp.Size = new System.Drawing.Size(96, 16);
         this.chkSearchUp.TabIndex = 4;
         this.chkSearchUp.Text = "Search &up";
         // 
         // btnClose
         // 
         this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnClose.Location = new System.Drawing.Point(296, 104);
         this.btnClose.Name = "btnClose";
         this.btnClose.Size = new System.Drawing.Size(80, 24);
         this.btnClose.TabIndex = 10;
         this.btnClose.Text = "Close";
         this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
         // 
         // frmFindReplace
         // 
         this.AcceptButton = this.btnFindNext;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnClose;
         this.ClientSize = new System.Drawing.Size(386, 138);
         this.Controls.Add(this.btnClose);
         this.Controls.Add(this.chkSearchUp);
         this.Controls.Add(this.chkWholeWord);
         this.Controls.Add(this.chkMatchCase);
         this.Controls.Add(this.btnReplaceAll);
         this.Controls.Add(this.btnReplace);
         this.Controls.Add(this.btnFindNext);
         this.Controls.Add(this.txtReplaceWith);
         this.Controls.Add(this.lblReplaceWith);
         this.Controls.Add(this.txtSearchFor);
         this.Controls.Add(this.lblSearchFor);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
         this.Name = "frmFindReplace";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.ResumeLayout(false);

      }
		#endregion

      protected override void OnClosing(CancelEventArgs e)
      {
         e.Cancel = true;
         this.Hide();
      }

      public string FindString
      {
         get
         {
            return txtSearchFor.Text;
         }
      }

      public string ReplaceString
      {
         get
         {
            return txtReplaceWith.Text;
         }
      }

      public void SetMode(bool replace)
      {
         lblReplaceWith.Visible = txtReplaceWith.Visible = btnReplace.Visible = btnReplaceAll.Visible = replace;
         ClientSize = new Size(ClientSize.Width, replace?136:72);
         btnClose.Top=replace?104:40;
         Text = replace?"Replace Text":"Find Text";
      }

      public void InitFocus()
      {
         txtSearchFor.Focus();
         txtSearchFor.SelectAll();
      }

      private void btnFindNext_Click(object sender, System.EventArgs e)
      {
         ((frmCodeEditor)Owner).DoFindNext();
      }

      private void btnClose_Click(object sender, System.EventArgs e)
      {
         this.Hide();
      }

      private void btnReplace_Click(object sender, System.EventArgs e)
      {
         ((frmCodeEditor)Owner).DoReplace();
      }

      private void btnReplaceAll_Click(object sender, System.EventArgs e)
      {
         ((frmCodeEditor)Owner).DoReplaceAll();
         this.Activate();
      }

      public RichTextBoxFinds Options
      {
         get
         {
            RichTextBoxFinds result = RichTextBoxFinds.None;
            if (chkMatchCase.Checked)
               result |= RichTextBoxFinds.MatchCase;
            if (chkWholeWord.Checked)
               result |= RichTextBoxFinds.WholeWord;
            if (chkSearchUp.Checked)
               result |= RichTextBoxFinds.Reverse;
            return result;
         }
      }
   }
}
