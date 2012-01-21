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
         this.rdoEmbedJS = new System.Windows.Forms.RadioButton();
         this.chkMapButtons = new System.Windows.Forms.CheckBox();
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.chkCamelCase = new System.Windows.Forms.CheckBox();
         this.chkEmbedPng = new System.Windows.Forms.CheckBox();
         this.rdoSeparateJS = new System.Windows.Forms.RadioButton();
         this.rdoFilePerObject = new System.Windows.Forms.RadioButton();
         this.dlgOutFolder = new System.Windows.Forms.FolderBrowserDialog();
         this.lblDirectory = new System.Windows.Forms.Label();
         this.txtDirectory = new System.Windows.Forms.TextBox();
         this.grpDisplayArea.SuspendLayout();
         this.grpOutputFiles.SuspendLayout();
         this.SuspendLayout();
         // 
         // lblFilename
         // 
         this.lblFilename.AutoSize = true;
         this.lblFilename.Location = new System.Drawing.Point(15, 229);
         this.lblFilename.Name = "lblFilename";
         this.lblFilename.Size = new System.Drawing.Size(57, 13);
         this.lblFilename.TabIndex = 5;
         this.lblFilename.Text = "File Name:";
         // 
         // txtFilename
         // 
         this.txtFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtFilename.Location = new System.Drawing.Point(78, 226);
         this.txtFilename.Name = "txtFilename";
         this.txtFilename.Size = new System.Drawing.Size(280, 20);
         this.txtFilename.TabIndex = 6;
         // 
         // btnBrowse
         // 
         this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnBrowse.Location = new System.Drawing.Point(359, 200);
         this.btnBrowse.Name = "btnBrowse";
         this.btnBrowse.Size = new System.Drawing.Size(27, 20);
         this.btnBrowse.TabIndex = 4;
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
         this.grpDisplayArea.Location = new System.Drawing.Point(12, 12);
         this.grpDisplayArea.Name = "grpDisplayArea";
         this.grpDisplayArea.Size = new System.Drawing.Size(374, 65);
         this.grpDisplayArea.TabIndex = 0;
         this.grpDisplayArea.TabStop = false;
         this.grpDisplayArea.Text = "Display Area";
         // 
         // rdoFill
         // 
         this.rdoFill.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.rdoFill.Location = new System.Drawing.Point(6, 42);
         this.rdoFill.Name = "rdoFill";
         this.rdoFill.Size = new System.Drawing.Size(362, 17);
         this.rdoFill.TabIndex = 1;
         this.rdoFill.Text = "Fill &browser window";
         this.rdoFill.UseVisualStyleBackColor = true;
         // 
         // rdoFixed
         // 
         this.rdoFixed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.rdoFixed.Checked = true;
         this.rdoFixed.Location = new System.Drawing.Point(6, 19);
         this.rdoFixed.Name = "rdoFixed";
         this.rdoFixed.Size = new System.Drawing.Size(362, 17);
         this.rdoFixed.TabIndex = 0;
         this.rdoFixed.TabStop = true;
         this.rdoFixed.Text = "&Fixed size defined in project settings";
         this.rdoFixed.UseVisualStyleBackColor = true;
         // 
         // grpOutputFiles
         // 
         this.grpOutputFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.grpOutputFiles.Controls.Add(this.rdoFilePerObject);
         this.grpOutputFiles.Controls.Add(this.rdoSeparateJS);
         this.grpOutputFiles.Controls.Add(this.chkEmbedPng);
         this.grpOutputFiles.Controls.Add(this.rdoEmbedJS);
         this.grpOutputFiles.Location = new System.Drawing.Point(12, 83);
         this.grpOutputFiles.Name = "grpOutputFiles";
         this.grpOutputFiles.Size = new System.Drawing.Size(374, 111);
         this.grpOutputFiles.TabIndex = 1;
         this.grpOutputFiles.TabStop = false;
         this.grpOutputFiles.Text = "Output Format";
         // 
         // rdoEmbedJS
         // 
         this.rdoEmbedJS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.rdoEmbedJS.Checked = true;
         this.rdoEmbedJS.Location = new System.Drawing.Point(6, 42);
         this.rdoEmbedJS.Name = "rdoEmbedJS";
         this.rdoEmbedJS.Size = new System.Drawing.Size(362, 17);
         this.rdoEmbedJS.TabIndex = 1;
         this.rdoEmbedJS.TabStop = true;
         this.rdoEmbedJS.Text = "Embed &JavaScript in HTML";
         this.rdoEmbedJS.UseVisualStyleBackColor = true;
         this.rdoEmbedJS.CheckedChanged += new System.EventHandler(this.Output_CheckedChanged);
         // 
         // chkMapButtons
         // 
         this.chkMapButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.chkMapButtons.Location = new System.Drawing.Point(18, 252);
         this.chkMapButtons.Name = "chkMapButtons";
         this.chkMapButtons.Size = new System.Drawing.Size(368, 17);
         this.chkMapButtons.TabIndex = 7;
         this.chkMapButtons.Text = "&Generate buttons to switch between maps";
         this.chkMapButtons.UseVisualStyleBackColor = true;
         // 
         // btnOK
         // 
         this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnOK.Location = new System.Drawing.Point(194, 299);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(93, 28);
         this.btnOK.TabIndex = 9;
         this.btnOK.Text = "OK";
         this.btnOK.UseVisualStyleBackColor = true;
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(293, 299);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(93, 28);
         this.btnCancel.TabIndex = 10;
         this.btnCancel.Text = "Cancel";
         this.btnCancel.UseVisualStyleBackColor = true;
         // 
         // chkCamelCase
         // 
         this.chkCamelCase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.chkCamelCase.Checked = true;
         this.chkCamelCase.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkCamelCase.Location = new System.Drawing.Point(18, 275);
         this.chkCamelCase.Name = "chkCamelCase";
         this.chkCamelCase.Size = new System.Drawing.Size(368, 18);
         this.chkCamelCase.TabIndex = 8;
         this.chkCamelCase.Text = "Force &rule function calls to camelCase";
         this.chkCamelCase.UseVisualStyleBackColor = true;
         // 
         // chkEmbedPng
         // 
         this.chkEmbedPng.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.chkEmbedPng.Checked = true;
         this.chkEmbedPng.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkEmbedPng.Location = new System.Drawing.Point(6, 19);
         this.chkEmbedPng.Name = "chkEmbedPng";
         this.chkEmbedPng.Size = new System.Drawing.Size(362, 17);
         this.chkEmbedPng.TabIndex = 0;
         this.chkEmbedPng.Text = "&Embed images in HTML file (recommended for local use)";
         this.chkEmbedPng.UseVisualStyleBackColor = true;
         this.chkEmbedPng.CheckedChanged += new System.EventHandler(this.Output_CheckedChanged);
         // 
         // rdoSeparateJS
         // 
         this.rdoSeparateJS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.rdoSeparateJS.Location = new System.Drawing.Point(6, 65);
         this.rdoSeparateJS.Name = "rdoSeparateJS";
         this.rdoSeparateJS.Size = new System.Drawing.Size(362, 17);
         this.rdoSeparateJS.TabIndex = 2;
         this.rdoSeparateJS.Text = "&Single external JavaScript file";
         this.rdoSeparateJS.UseVisualStyleBackColor = true;
         this.rdoSeparateJS.CheckedChanged += new System.EventHandler(this.Output_CheckedChanged);
         // 
         // rdoFilePerObject
         // 
         this.rdoFilePerObject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.rdoFilePerObject.Location = new System.Drawing.Point(6, 88);
         this.rdoFilePerObject.Name = "rdoFilePerObject";
         this.rdoFilePerObject.Size = new System.Drawing.Size(362, 17);
         this.rdoFilePerObject.TabIndex = 3;
         this.rdoFilePerObject.Text = "Separate JavaScript file &per object";
         this.rdoFilePerObject.UseVisualStyleBackColor = true;
         this.rdoFilePerObject.CheckedChanged += new System.EventHandler(this.Output_CheckedChanged);
         // 
         // lblDirectory
         // 
         this.lblDirectory.AutoSize = true;
         this.lblDirectory.Location = new System.Drawing.Point(15, 203);
         this.lblDirectory.Name = "lblDirectory";
         this.lblDirectory.Size = new System.Drawing.Size(52, 13);
         this.lblDirectory.TabIndex = 2;
         this.lblDirectory.Text = "Directory:";
         // 
         // txtDirectory
         // 
         this.txtDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtDirectory.Location = new System.Drawing.Point(78, 200);
         this.txtDirectory.Name = "txtDirectory";
         this.txtDirectory.Size = new System.Drawing.Size(280, 20);
         this.txtDirectory.TabIndex = 3;
         // 
         // frmHTML5Export
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(398, 339);
         this.Controls.Add(this.txtDirectory);
         this.Controls.Add(this.lblDirectory);
         this.Controls.Add(this.grpOutputFiles);
         this.Controls.Add(this.chkCamelCase);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.chkMapButtons);
         this.Controls.Add(this.grpDisplayArea);
         this.Controls.Add(this.btnBrowse);
         this.Controls.Add(this.txtFilename);
         this.Controls.Add(this.lblFilename);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.Name = "frmHTML5Export";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
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
      private System.Windows.Forms.RadioButton rdoEmbedJS;
      private System.Windows.Forms.CheckBox chkMapButtons;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.CheckBox chkCamelCase;
      private System.Windows.Forms.CheckBox chkEmbedPng;
      private System.Windows.Forms.RadioButton rdoFilePerObject;
      private System.Windows.Forms.RadioButton rdoSeparateJS;
      private System.Windows.Forms.FolderBrowserDialog dlgOutFolder;
      private System.Windows.Forms.Label lblDirectory;
      private System.Windows.Forms.TextBox txtDirectory;
   }
}