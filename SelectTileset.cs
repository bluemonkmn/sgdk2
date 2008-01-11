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
	/// Summary description for SelectTileset.
	/// </summary>
	public class frmSelectTileset : System.Windows.Forms.Form
	{
      private System.Windows.Forms.ComboBox cboTileset;
      private System.Windows.Forms.Label lblTileset;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button brnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmSelectTileset()
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
         this.cboTileset = new System.Windows.Forms.ComboBox();
         this.lblTileset = new System.Windows.Forms.Label();
         this.btnOK = new System.Windows.Forms.Button();
         this.brnCancel = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // cboTileset
         // 
         this.cboTileset.DisplayMember = "Name";
         this.cboTileset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboTileset.Location = new System.Drawing.Point(80, 8);
         this.cboTileset.Name = "cboTileset";
         this.cboTileset.Size = new System.Drawing.Size(208, 21);
         this.cboTileset.TabIndex = 0;
         // 
         // lblTileset
         // 
         this.lblTileset.Location = new System.Drawing.Point(8, 8);
         this.lblTileset.Name = "lblTileset";
         this.lblTileset.Size = new System.Drawing.Size(72, 21);
         this.lblTileset.TabIndex = 1;
         this.lblTileset.Text = "Tileset:";
         this.lblTileset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // btnOK
         // 
         this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnOK.Location = new System.Drawing.Point(152, 40);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(64, 24);
         this.btnOK.TabIndex = 2;
         this.btnOK.Text = "OK";
         // 
         // brnCancel
         // 
         this.brnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.brnCancel.Location = new System.Drawing.Point(224, 40);
         this.brnCancel.Name = "brnCancel";
         this.brnCancel.Size = new System.Drawing.Size(64, 24);
         this.brnCancel.TabIndex = 3;
         this.brnCancel.Text = "Cancel";
         // 
         // frmSelectTileset
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.brnCancel;
         this.ClientSize = new System.Drawing.Size(298, 71);
         this.Controls.Add(this.brnCancel);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.lblTileset);
         this.Controls.Add(this.cboTileset);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "frmSelectTileset";
         this.Text = "Select Tileset";
         this.ResumeLayout(false);

      }
		#endregion

      protected override void OnLoad(EventArgs e)
      {
         foreach(System.Data.DataRowView drv in ProjectData.Tileset.DefaultView)
            cboTileset.Items.Add((ProjectDataset.TilesetRow)drv.Row);
         base.OnLoad (e);
      }

      public static ProjectDataset.TilesetRow SelectTileset()
      {
         frmSelectTileset frm = new frmSelectTileset();
         if (DialogResult.OK == frm.ShowDialog())
         {
            ProjectDataset.TilesetRow result = (ProjectDataset.TilesetRow)frm.cboTileset.SelectedItem;
            frm.Dispose();
            return result;
         }
         frm.Dispose();
         return null;
      }
	}
}
