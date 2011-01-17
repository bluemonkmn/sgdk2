namespace SGDK2
{
   partial class frmEditMessage
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
         System.Windows.Forms.Label lblTileset;
         System.Windows.Forms.Label lblMessage;
         System.Windows.Forms.Button btnOK;
         System.Windows.Forms.Button btnCancel;
         System.Windows.Forms.Button btnExtended;
         this.grpPreview = new System.Windows.Forms.GroupBox();
         this.cboTileset = new System.Windows.Forms.ComboBox();
         this.txtMessage = new System.Windows.Forms.TextBox();
         lblTileset = new System.Windows.Forms.Label();
         lblMessage = new System.Windows.Forms.Label();
         btnOK = new System.Windows.Forms.Button();
         btnCancel = new System.Windows.Forms.Button();
         btnExtended = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // lblTileset
         // 
         lblTileset.AutoSize = true;
         lblTileset.Location = new System.Drawing.Point(12, 15);
         lblTileset.Name = "lblTileset";
         lblTileset.Size = new System.Drawing.Size(41, 13);
         lblTileset.TabIndex = 0;
         lblTileset.Text = "Tileset:";
         // 
         // lblMessage
         // 
         lblMessage.AutoSize = true;
         lblMessage.Location = new System.Drawing.Point(12, 42);
         lblMessage.Name = "lblMessage";
         lblMessage.Size = new System.Drawing.Size(53, 13);
         lblMessage.TabIndex = 2;
         lblMessage.Text = "Message:";
         // 
         // btnOK
         // 
         btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         btnOK.Location = new System.Drawing.Point(439, 298);
         btnOK.Name = "btnOK";
         btnOK.Size = new System.Drawing.Size(82, 23);
         btnOK.TabIndex = 8;
         btnOK.Text = "OK";
         btnOK.UseVisualStyleBackColor = true;
         // 
         // btnCancel
         // 
         btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         btnCancel.Location = new System.Drawing.Point(527, 298);
         btnCancel.Name = "btnCancel";
         btnCancel.Size = new System.Drawing.Size(82, 23);
         btnCancel.TabIndex = 9;
         btnCancel.Text = "Cancel";
         btnCancel.UseVisualStyleBackColor = true;
         // 
         // btnExtended
         // 
         btnExtended.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         btnExtended.Location = new System.Drawing.Point(582, 39);
         btnExtended.Name = "btnExtended";
         btnExtended.Size = new System.Drawing.Size(27, 20);
         btnExtended.TabIndex = 10;
         btnExtended.Text = "...";
         btnExtended.UseVisualStyleBackColor = true;
         btnExtended.Click += new System.EventHandler(this.btnExtended_Click);
         // 
         // grpPreview
         // 
         this.grpPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.grpPreview.Location = new System.Drawing.Point(12, 65);
         this.grpPreview.Name = "grpPreview";
         this.grpPreview.Size = new System.Drawing.Size(597, 227);
         this.grpPreview.TabIndex = 5;
         this.grpPreview.TabStop = false;
         this.grpPreview.Text = "Preview";
         this.grpPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.grpPreview_Paint);
         // 
         // cboTileset
         // 
         this.cboTileset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.cboTileset.DisplayMember = "Name";
         this.cboTileset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboTileset.FormattingEnabled = true;
         this.cboTileset.Location = new System.Drawing.Point(71, 12);
         this.cboTileset.Name = "cboTileset";
         this.cboTileset.Size = new System.Drawing.Size(538, 21);
         this.cboTileset.TabIndex = 3;
         this.cboTileset.SelectedIndexChanged += new System.EventHandler(this.cboTileset_SelectedIndexChanged);
         // 
         // txtMessage
         // 
         this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtMessage.Location = new System.Drawing.Point(71, 39);
         this.txtMessage.Name = "txtMessage";
         this.txtMessage.Size = new System.Drawing.Size(505, 20);
         this.txtMessage.TabIndex = 4;
         this.txtMessage.TextChanged += new System.EventHandler(this.txtMessage_TextChanged);
         // 
         // frmEditMessage
         // 
         this.AcceptButton = btnOK;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = btnCancel;
         this.ClientSize = new System.Drawing.Size(621, 333);
         this.Controls.Add(btnExtended);
         this.Controls.Add(btnCancel);
         this.Controls.Add(btnOK);
         this.Controls.Add(this.grpPreview);
         this.Controls.Add(this.txtMessage);
         this.Controls.Add(this.cboTileset);
         this.Controls.Add(lblMessage);
         this.Controls.Add(lblTileset);
         this.Name = "frmEditMessage";
         this.Text = "Edit Message";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.ComboBox cboTileset;
      private System.Windows.Forms.TextBox txtMessage;
      private System.Windows.Forms.GroupBox grpPreview;

   }
}