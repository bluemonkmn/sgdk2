namespace SGDK2
{
   partial class frmEditMessageChar
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
         System.Windows.Forms.Label lblUnicode;
         this.graphics = new SGDK2.GraphicBrowser();
         this.txtUnicode = new System.Windows.Forms.TextBox();
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         lblUnicode = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // graphics
         // 
         this.graphics.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.graphics.BorderStyle = SGDK2.DragPanelBorderStyle.None;
         this.graphics.CellBorders = false;
         this.graphics.CellPadding = new System.Drawing.Size(0, 0);
         this.graphics.CellSize = new System.Drawing.Size(0, 0);
         this.graphics.CurrentCellIndex = -1;
         this.graphics.Frameset = null;
         this.graphics.FramesToDisplay = null;
         this.graphics.GraphicSheet = null;
         this.graphics.Location = new System.Drawing.Point(0, 0);
         this.graphics.Name = "graphics";
         this.graphics.SheetImage = null;
         this.graphics.Size = new System.Drawing.Size(321, 211);
         this.graphics.TabIndex = 0;
         this.graphics.Text = "Available Tiles";
         this.graphics.DoubleClick += new System.EventHandler(this.graphics_DoubleClick);
         this.graphics.CurrentCellChanged += new System.EventHandler(this.graphics_CurrentCellChanged);
         // 
         // lblUnicode
         // 
         lblUnicode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         lblUnicode.AutoSize = true;
         lblUnicode.Location = new System.Drawing.Point(12, 220);
         lblUnicode.Name = "lblUnicode";
         lblUnicode.Size = new System.Drawing.Size(84, 13);
         lblUnicode.TabIndex = 1;
         lblUnicode.Text = "Character Code:";
         // 
         // txtUnicode
         // 
         this.txtUnicode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtUnicode.Location = new System.Drawing.Point(102, 217);
         this.txtUnicode.Name = "txtUnicode";
         this.txtUnicode.Size = new System.Drawing.Size(207, 20);
         this.txtUnicode.TabIndex = 2;
         // 
         // btnOK
         // 
         this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnOK.Location = new System.Drawing.Point(149, 243);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(77, 24);
         this.btnOK.TabIndex = 3;
         this.btnOK.Text = "Insert";
         this.btnOK.UseVisualStyleBackColor = true;
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(232, 243);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(77, 24);
         this.btnCancel.TabIndex = 4;
         this.btnCancel.Text = "Cancel";
         this.btnCancel.UseVisualStyleBackColor = true;
         // 
         // frmEditMessageChar
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(321, 279);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.txtUnicode);
         this.Controls.Add(lblUnicode);
         this.Controls.Add(this.graphics);
         this.Name = "frmEditMessageChar";
         this.Text = "Edit Message - Extended Characters";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private GraphicBrowser graphics;
      private System.Windows.Forms.TextBox txtUnicode;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
   }
}