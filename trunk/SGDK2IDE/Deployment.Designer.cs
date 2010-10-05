namespace SGDK2
{
   partial class frmDeployment
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDeployment));
         System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Online Help",
            "Press F1 for help about the current window, or use Help menu.",
            "Not Installed"}, -1);
         System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "Template Library",
            "Sample projects, sprites, sounds and more to use in your projects.",
            "Not Installed"}, -1);
         this.lblDeploymentInfo = new System.Windows.Forms.Label();
         this.lvwDeployment = new System.Windows.Forms.ListView();
         this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.colStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.btnDownload = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.lblToolMenuInfo = new System.Windows.Forms.Label();
         this.chkDontShow = new System.Windows.Forms.CheckBox();
         this.progress = new System.Windows.Forms.ProgressBar();
         this.lblProgress = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // lblDeploymentInfo
         // 
         this.lblDeploymentInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.lblDeploymentInfo.Location = new System.Drawing.Point(12, 12);
         this.lblDeploymentInfo.Name = "lblDeploymentInfo";
         this.lblDeploymentInfo.Size = new System.Drawing.Size(536, 49);
         this.lblDeploymentInfo.TabIndex = 0;
         this.lblDeploymentInfo.Text = resources.GetString("lblDeploymentInfo.Text");
         // 
         // lvwDeployment
         // 
         this.lvwDeployment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.lvwDeployment.CheckBoxes = true;
         this.lvwDeployment.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colDescription,
            this.colStatus});
         listViewItem1.Checked = true;
         listViewItem1.StateImageIndex = 1;
         listViewItem2.Checked = true;
         listViewItem2.StateImageIndex = 1;
         this.lvwDeployment.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
         this.lvwDeployment.Location = new System.Drawing.Point(12, 99);
         this.lvwDeployment.Name = "lvwDeployment";
         this.lvwDeployment.Size = new System.Drawing.Size(536, 72);
         this.lvwDeployment.TabIndex = 1;
         this.lvwDeployment.UseCompatibleStateImageBehavior = false;
         this.lvwDeployment.View = System.Windows.Forms.View.Details;
         // 
         // colName
         // 
         this.colName.Text = "Feature";
         this.colName.Width = 113;
         // 
         // colDescription
         // 
         this.colDescription.Text = "Description";
         this.colDescription.Width = 321;
         // 
         // colStatus
         // 
         this.colStatus.Text = "Status";
         this.colStatus.Width = 79;
         // 
         // btnDownload
         // 
         this.btnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnDownload.Location = new System.Drawing.Point(320, 215);
         this.btnDownload.Name = "btnDownload";
         this.btnDownload.Size = new System.Drawing.Size(111, 28);
         this.btnDownload.TabIndex = 2;
         this.btnDownload.Text = "Download";
         this.btnDownload.UseVisualStyleBackColor = true;
         this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(437, 215);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(111, 28);
         this.btnCancel.TabIndex = 3;
         this.btnCancel.Text = "Cancel";
         this.btnCancel.UseVisualStyleBackColor = true;
         // 
         // lblToolMenuInfo
         // 
         this.lblToolMenuInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.lblToolMenuInfo.Location = new System.Drawing.Point(12, 61);
         this.lblToolMenuInfo.Name = "lblToolMenuInfo";
         this.lblToolMenuInfo.Size = new System.Drawing.Size(536, 35);
         this.lblToolMenuInfo.TabIndex = 4;
         this.lblToolMenuInfo.Text = "If you want to revisit this option later, select \"Download Updates...\" from the T" +
             "ools menu at any time.";
         // 
         // chkDontShow
         // 
         this.chkDontShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.chkDontShow.AutoSize = true;
         this.chkDontShow.Location = new System.Drawing.Point(12, 214);
         this.chkDontShow.Name = "chkDontShow";
         this.chkDontShow.Size = new System.Drawing.Size(172, 17);
         this.chkDontShow.TabIndex = 5;
         this.chkDontShow.Text = "Don\'t display this automatically.";
         this.chkDontShow.UseVisualStyleBackColor = true;
         this.chkDontShow.CheckedChanged += new System.EventHandler(this.chkDontShow_CheckedChanged);
         // 
         // progress
         // 
         this.progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.progress.Location = new System.Drawing.Point(12, 193);
         this.progress.Name = "progress";
         this.progress.Size = new System.Drawing.Size(536, 16);
         this.progress.TabIndex = 6;
         // 
         // lblProgress
         // 
         this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.lblProgress.Location = new System.Drawing.Point(12, 174);
         this.lblProgress.Name = "lblProgress";
         this.lblProgress.Size = new System.Drawing.Size(536, 16);
         this.lblProgress.TabIndex = 7;
         this.lblProgress.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
         // 
         // frmDeployment
         // 
         this.AcceptButton = this.btnDownload;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(560, 255);
         this.Controls.Add(this.lblProgress);
         this.Controls.Add(this.progress);
         this.Controls.Add(this.chkDontShow);
         this.Controls.Add(this.lblToolMenuInfo);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnDownload);
         this.Controls.Add(this.lvwDeployment);
         this.Controls.Add(this.lblDeploymentInfo);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.Name = "frmDeployment";
         this.Text = "Deployment Options";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label lblDeploymentInfo;
      private System.Windows.Forms.ListView lvwDeployment;
      private System.Windows.Forms.ColumnHeader colName;
      private System.Windows.Forms.ColumnHeader colDescription;
      private System.Windows.Forms.ColumnHeader colStatus;
      private System.Windows.Forms.Button btnDownload;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Label lblToolMenuInfo;
      private System.Windows.Forms.CheckBox chkDontShow;
      private System.Windows.Forms.ProgressBar progress;
      private System.Windows.Forms.Label lblProgress;
   }
}