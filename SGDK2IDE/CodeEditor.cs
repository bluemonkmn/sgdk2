using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for CodeEditor.
	/// </summary>
	public class frmCodeEditor : System.Windows.Forms.Form
	{
      #region Non-control members
      ProjectDataset.SourceCodeRow m_SourceCode;
      bool m_isDirty;
      #endregion

      private System.Windows.Forms.TextBox txtCode;
      private System.Windows.Forms.MainMenu mnuCodeEditor;
      private System.Windows.Forms.MenuItem mnuCode;
      private System.Windows.Forms.MenuItem mnuSave;
      private SGDK2.DataChangeNotifier DataMonitor;
      private System.ComponentModel.IContainer components;

      #region Windows Form Designer Components

      #endregion

      #region Initialization and Clean-up
		public frmCodeEditor(ProjectDataset.SourceCodeRow drSourceCode)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         m_SourceCode = drSourceCode;
         txtCode.Text = m_SourceCode.Text;
         m_isDirty = false;
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
      #endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.components = new System.ComponentModel.Container();
         this.txtCode = new System.Windows.Forms.TextBox();
         this.mnuCodeEditor = new System.Windows.Forms.MainMenu();
         this.mnuCode = new System.Windows.Forms.MenuItem();
         this.mnuSave = new System.Windows.Forms.MenuItem();
         this.DataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.SuspendLayout();
         // 
         // txtCode
         // 
         this.txtCode.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtCode.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
         this.txtCode.Location = new System.Drawing.Point(0, 0);
         this.txtCode.Multiline = true;
         this.txtCode.Name = "txtCode";
         this.txtCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
         this.txtCode.Size = new System.Drawing.Size(456, 349);
         this.txtCode.TabIndex = 0;
         this.txtCode.Text = "";
         this.txtCode.WordWrap = false;
         this.txtCode.TextChanged += new System.EventHandler(this.txtCode_TextChanged);
         // 
         // mnuCodeEditor
         // 
         this.mnuCodeEditor.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.mnuCode});
         // 
         // mnuCode
         // 
         this.mnuCode.Index = 0;
         this.mnuCode.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                this.mnuSave});
         this.mnuCode.Text = "&Code";
         // 
         // mnuSave
         // 
         this.mnuSave.Index = 0;
         this.mnuSave.Shortcut = System.Windows.Forms.Shortcut.CtrlF8;
         this.mnuSave.Text = "&Save to Project";
         this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
         // 
         // DataMonitor
         // 
         this.DataMonitor.Clearing += new System.EventHandler(this.DataMonitor_Clearing);
         // 
         // frmCodeEditor
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(456, 349);
         this.Controls.Add(this.txtCode);
         this.Menu = this.mnuCodeEditor;
         this.Name = "frmCodeEditor";
         this.Text = "CodeEditor";
         this.Closing += new System.ComponentModel.CancelEventHandler(this.frmCodeEditor_Closing);
         this.ResumeLayout(false);

      }
		#endregion

      #region Event Handlers
      private void mnuSave_Click(object sender, System.EventArgs e)
      {
         m_SourceCode.Text = txtCode.Text;
         m_isDirty = false;
      }

      private void txtCode_TextChanged(object sender, System.EventArgs e)
      {
         m_isDirty = true;
      }

      private void DataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void frmCodeEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         if (m_isDirty)
         {
            switch (MessageBox.Show(this, "Do you want to save the changes that have been made to this code to the project?", "Unsaved Changes Exist", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
            {
               case DialogResult.Yes:
                  m_SourceCode.Text = txtCode.Text;
                  m_isDirty=false;
                  break;
               case DialogResult.No:
                  break;
               case DialogResult.Cancel:
                  e.Cancel = true;
                  break;
            }
         }
      }
      #endregion
   }
}
