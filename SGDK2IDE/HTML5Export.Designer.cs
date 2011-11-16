namespace SGDK2
{
   partial class frmHTML5Export
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.lblFilename = new System.Windows.Forms.Label();
         this.txtFilename = new System.Windows.Forms.TextBox();
         this.btnBrowse = new System.Windows.Forms.Button();
         this.grpDisplayArea = new System.Windows.Forms.GroupBox();
         this.rdoFill = new System.Windows.Forms.RadioButton();
         this.rdoFixed = new System.Windows.Forms.RadioButton();
         this.grpOutputFiles = new System.Windows.Forms.GroupBox();
         this.rdoMultiFiles = new System.Windows.Forms.RadioButton();
         this.rdoSingleFile = new System.Windows.Forms.RadioButton();
         this.chkMapButtons = new System.Windows.Forms.CheckBox();
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.dlgFilename = new System.Windows.Forms.SaveFileDialog();
         this.grpDisplayArea.SuspendLayout();
         this.grpOutputFiles.SuspendLayout();
         this.SuspendLayout();
         // 
         // lblFilename
         // 
         this.lblFilename.AutoSize = true;
         this.lblFilename.Location = new System.Drawing.Point(12, 15);
         this.lblFilename.Name = "lblFilename";
         this.lblFilename.Size = new System.Drawing.Size(57, 13);
         this.lblFilename.TabIndex = 0;
         this.lblFilename.Text = "File Name:";
         // 
         // txtFilename
         // 
         this.txtFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtFilename.Location = new System.Drawing.Point(75, 12);
         this.txtFilename.Name = "txtFilename";
         this.txtFilename.Size = new System.Drawing.Size(222, 20);
         this.txtFilename.TabIndex = 1;
         // 
         // btnBrowse
         // 
         this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnBrowse.Location = new System.Drawing.Point(301, 12);
         this.btnBrowse.Name = "btnBrowse";
         this.btnBrowse.Size = new System.Drawing.Size(27, 20);
         this.btnBrowse.TabIndex = 2;
         this.btnBrowse.Text = "...";
         this.btnBrowse.UseVisualStyleBackColor = true;
         this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
         // 
         // grpDisplayArea
         // 
         this.grpDisplayArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.grpDisplayArea.Controls.Add(this.rdoFill);
         this.grpDisplayArea.Controls.Add(this.rdoFixed);
         this.grpDisplayArea.Location = new System.Drawing.Point(12, 38);
         this.grpDisplayArea.Name = "grpDisplayArea";
         this.grpDisplayArea.Size = new System.Drawing.Size(316, 65);
         this.grpDisplayArea.TabIndex = 3;
         this.grpDisplayArea.TabStop = false;
         this.grpDisplayArea.Text = "Display Area";
         // 
         // rdoFill
         // 
         this.rdoFill.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.rdoFill.Location = new System.Drawing.Point(6, 42);
         this.rdoFill.Name = "rdoFill";
         this.rdoFill.Size = new System.Drawing.Size(304, 17);
         this.rdoFill.TabIndex = 1;
         this.rdoFill.Text = "Fill browser window";
         this.rdoFill.UseVisualStyleBackColor = true;
         // 
         // rdoFixed
         // 
         this.rdoFixed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.rdoFixed.Checked = true;
         this.rdoFixed.Location = new System.Drawing.Point(6, 19);
         this.rdoFixed.Name = "rdoFixed";
         this.rdoFixed.Size = new System.Drawing.Size(304, 17);
         this.rdoFixed.TabIndex = 0;
         this.rdoFixed.TabStop = true;
         this.rdoFixed.Text = "Fixed size defined in project settings";
         this.rdoFixed.UseVisualStyleBackColor = true;
         // 
         // grpOutputFiles
         // 
         this.grpOutputFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.grpOutputFiles.Controls.Add(this.rdoMultiFiles);
         this.grpOutputFiles.Controls.Add(this.rdoSingleFile);
         this.grpOutputFiles.Location = new System.Drawing.Point(12, 109);
         this.grpOutputFiles.Name = "grpOutputFiles";
         this.grpOutputFiles.Size = new System.Drawing.Size(316, 65);
         this.grpOutputFiles.TabIndex = 4;
         this.grpOutputFiles.TabStop = false;
         this.grpOutputFiles.Text = "Output files";
         // 
         // rdoMultiFiles
         // 
         this.rdoMultiFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.rdoMultiFiles.Location = new System.Drawing.Point(6, 42);
         this.rdoMultiFiles.Name = "rdoMultiFiles";
         this.rdoMultiFiles.Size = new System.Drawing.Size(304, 17);
         this.rdoMultiFiles.TabIndex = 1;
         this.rdoMultiFiles.Text = "Separate HTML, PNG and JS files";
         this.rdoMultiFiles.UseVisualStyleBackColor = true;
         // 
         // rdoSingleFile
         // 
         this.rdoSingleFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.rdoSingleFile.Checked = true;
         this.rdoSingleFile.Location = new System.Drawing.Point(6, 19);
         this.rdoSingleFile.Name = "rdoSingleFile";
         this.rdoSingleFile.Size = new System.Drawing.Size(304, 17);
         this.rdoSingleFile.TabIndex = 0;
         this.rdoSingleFile.TabStop = true;
         this.rdoSingleFile.Text = "Single HTML file (recommended for local use)";
         this.rdoSingleFile.UseVisualStyleBackColor = true;
         // 
         // chkMapButtons
         // 
         this.chkMapButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.chkMapButtons.Checked = true;
         this.chkMapButtons.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkMapButtons.Location = new System.Drawing.Point(18, 180);
         this.chkMapButtons.Name = "chkMapButtons";
         this.chkMapButtons.Size = new System.Drawing.Size(310, 17);
         this.chkMapButtons.TabIndex = 5;
         this.chkMapButtons.Text = "Generate buttons to switch between maps";
         this.chkMapButtons.UseVisualStyleBackColor = true;
         // 
         // btnOK
         // 
         this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnOK.Location = new System.Drawing.Point(136, 222);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(93, 28);
         this.btnOK.TabIndex = 6;
         this.btnOK.Text = "OK";
         this.btnOK.UseVisualStyleBackColor = true;
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(235, 222);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(93, 28);
         this.btnCancel.TabIndex = 7;
         this.btnCancel.Text = "Cancel";
         this.btnCancel.UseVisualStyleBackColor = true;
         // 
         // dlgFilename
         // 
         this.dlgFilename.Filter = "HTML Files (*.html)|*.html|All Files|*.*";
         this.dlgFilename.OverwritePrompt = false;
         this.dlgFilename.Title = "Export HTML5 File Name";
         // 
         // frmHTML5Export
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(340, 262);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.chkMapButtons);
         this.Controls.Add(this.grpOutputFiles);
         this.Controls.Add(this.grpDisplayArea);
         this.Controls.Add(this.btnBrowse);
         this.Controls.Add(this.txtFilename);
         this.Controls.Add(this.lblFilename);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.Name = "frmHTML5Export";
         this.ShowInTaskbar = false;
         this.Text = "Export Project to HTML5";
         this.grpDisplayArea.ResumeLayout(false);
         this.grpOutputFiles.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label lblFilename;
      private System.Windows.Forms.TextBox txtFilename;
      private System.Windows.Forms.Button btnBrowse;
      private System.Windows.Forms.GroupBox grpDisplayArea;
      private System.Windows.Forms.RadioButton rdoFill;
      private System.Windows.Forms.RadioButton rdoFixed;
      private System.Windows.Forms.GroupBox grpOutputFiles;
      private System.Windows.Forms.RadioButton rdoMultiFiles;
      private System.Windows.Forms.RadioButton rdoSingleFile;
      private System.Windows.Forms.CheckBox chkMapButtons;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.SaveFileDialog dlgFilename;
   }
}