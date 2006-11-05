using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for SelectSpriteParameter.
	/// </summary>
	public class frmSelectSpriteParameter : System.Windows.Forms.Form
	{
      private System.Windows.Forms.Label lblSprite;
      private System.Windows.Forms.ComboBox cboSprite;
      private System.Windows.Forms.Label lblParameter;
      private System.Windows.Forms.ComboBox cboParameter;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmSelectSpriteParameter(ProjectDataset.LayerRow parentLayer)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         foreach(ProjectDataset.SpriteRow sprite in parentLayer.GetSpriteRows())
            cboSprite.Items.Add(sprite);
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
         this.lblSprite = new System.Windows.Forms.Label();
         this.cboSprite = new System.Windows.Forms.ComboBox();
         this.lblParameter = new System.Windows.Forms.Label();
         this.cboParameter = new System.Windows.Forms.ComboBox();
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // lblSprite
         // 
         this.lblSprite.Location = new System.Drawing.Point(8, 8);
         this.lblSprite.Name = "lblSprite";
         this.lblSprite.Size = new System.Drawing.Size(96, 21);
         this.lblSprite.TabIndex = 0;
         this.lblSprite.Text = "Sprite:";
         this.lblSprite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // cboSprite
         // 
         this.cboSprite.DisplayMember = "Name";
         this.cboSprite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboSprite.Location = new System.Drawing.Point(104, 8);
         this.cboSprite.Name = "cboSprite";
         this.cboSprite.Size = new System.Drawing.Size(168, 21);
         this.cboSprite.TabIndex = 1;
         this.cboSprite.SelectedValueChanged += new System.EventHandler(this.cboSprite_SelectedValueChanged);
         // 
         // lblParameter
         // 
         this.lblParameter.Location = new System.Drawing.Point(8, 40);
         this.lblParameter.Name = "lblParameter";
         this.lblParameter.Size = new System.Drawing.Size(96, 21);
         this.lblParameter.TabIndex = 2;
         this.lblParameter.Text = "Parameter:";
         this.lblParameter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // cboParameter
         // 
         this.cboParameter.DisplayMember = "Name";
         this.cboParameter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboParameter.Location = new System.Drawing.Point(104, 40);
         this.cboParameter.Name = "cboParameter";
         this.cboParameter.Size = new System.Drawing.Size(168, 21);
         this.cboParameter.TabIndex = 3;
         this.cboParameter.SelectedValueChanged += new System.EventHandler(this.cboParameter_SelectedValueChanged);
         // 
         // btnOK
         // 
         this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnOK.Enabled = false;
         this.btnOK.Location = new System.Drawing.Point(104, 72);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(80, 24);
         this.btnOK.TabIndex = 4;
         this.btnOK.Text = "OK";
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(192, 72);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(80, 24);
         this.btnCancel.TabIndex = 5;
         this.btnCancel.Text = "Cancel";
         // 
         // frmSelectSpriteParameter
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(282, 103);
         this.ControlBox = false;
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.cboParameter);
         this.Controls.Add(this.lblParameter);
         this.Controls.Add(this.cboSprite);
         this.Controls.Add(this.lblSprite);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "frmSelectSpriteParameter";
         this.Text = "Select Sprite Parameter";
         this.ResumeLayout(false);

      }
		#endregion

      private void cboSprite_SelectedValueChanged(object sender, System.EventArgs e)
      {
         if (cboSprite.SelectedIndex < 0)
            return;
         cboParameter.Items.Clear();
         foreach(ProjectDataset.SpriteParameterRow param in ((ProjectDataset.SpriteRow)cboSprite.Items[cboSprite.SelectedIndex]).SpriteStateRowParent.SpriteDefinitionRow.GetSpriteParameterRows())
            cboParameter.Items.Add(param);
         btnOK.Enabled = false;
      }

      private void cboParameter_SelectedValueChanged(object sender, System.EventArgs e)
      {
         btnOK.Enabled = (cboParameter.SelectedIndex >= 0);
      }

      public ProjectDataset.SpriteRow SpriteRow
      {
         get
         {
            return (ProjectDataset.SpriteRow)cboSprite.Items[cboSprite.SelectedIndex];
         }
      }

      public ProjectDataset.SpriteParameterRow SpriteParameterRow
      {
         get
         {
            return (ProjectDataset.SpriteParameterRow)cboParameter.Items[cboParameter.SelectedIndex];
         }
      }
	}
}
