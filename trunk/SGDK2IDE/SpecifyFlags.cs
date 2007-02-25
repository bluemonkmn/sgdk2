using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for SpecifyFlags.
	/// </summary>
	public class frmSpecifyFlags : System.Windows.Forms.Form
	{
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.ListView lvwFlags;
      private System.Windows.Forms.ColumnHeader colFlag;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmSpecifyFlags(EnumOptionSelector options, string oldValue)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         string[] parts;
         if (oldValue != null)
            parts = oldValue.Split('|');
         else
            parts = new string[] {};

         int maxWidth = 60;
         foreach(string item in options.details.names)
         {
            using (Graphics gfx = Graphics.FromHwnd(lvwFlags.Handle))
            {
               ListViewItem lvi = lvwFlags.Items.Add(item);
               int itemWidth = (int)gfx.MeasureString(item, lvwFlags.Font).Width;
               if (itemWidth > maxWidth)
                  maxWidth = itemWidth;
               foreach(string part in parts)
               {
                  if (part.Trim() == item)
                     lvi.Checked=true;
               }
            }
         }
         colFlag.Width = maxWidth;
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
         this.btnCancel = new System.Windows.Forms.Button();
         this.lvwFlags = new System.Windows.Forms.ListView();
         this.colFlag = new System.Windows.Forms.ColumnHeader();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnOK.Location = new System.Drawing.Point(142, 232);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(72, 24);
         this.btnOK.TabIndex = 1;
         this.btnOK.Text = "OK";
         // 
         // btnCancel
         // 
         this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(222, 232);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(72, 24);
         this.btnCancel.TabIndex = 2;
         this.btnCancel.Text = "Cancel";
         // 
         // lvwFlags
         // 
         this.lvwFlags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.lvwFlags.CheckBoxes = true;
         this.lvwFlags.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                   this.colFlag});
         this.lvwFlags.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
         this.lvwFlags.Location = new System.Drawing.Point(8, 8);
         this.lvwFlags.Name = "lvwFlags";
         this.lvwFlags.Size = new System.Drawing.Size(286, 216);
         this.lvwFlags.TabIndex = 3;
         this.lvwFlags.View = System.Windows.Forms.View.Details;
         // 
         // colFlag
         // 
         this.colFlag.Text = "Option Name";
         this.colFlag.Width = 282;
         // 
         // frmSpecifyFlags
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(304, 263);
         this.Controls.Add(this.lvwFlags);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
         this.MaximizeBox = false;
         this.Name = "frmSpecifyFlags";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "SpecifyFlags";
         this.ResumeLayout(false);

      }
		#endregion

      public static string GetOptions(IWin32Window owner, EnumOptionSelector options, string oldValue)
      {
         using(frmSpecifyFlags frm = new frmSpecifyFlags(options, oldValue))
         {
            if (frm.ShowDialog(owner) == DialogResult.OK)
            {
               System.Text.StringBuilder sb = new System.Text.StringBuilder();
               for (int idx=0; idx<frm.lvwFlags.Items.Count; idx++)
               {
                  if (frm.lvwFlags.Items[idx].Checked)
                  {
                     if (sb.Length > 0)
                        sb.Append("|");
                     sb.Append(frm.lvwFlags.Items[idx].Text);
                  }
               }
               return sb.ToString();
            }
            else
               return null;
         }
      }
   }

   #region Embedded classes
   public class EnumOptionSelector
   {
      public readonly string name;
      public readonly EnumTable.EnumDetails details;
      public EnumOptionSelector(string name, EnumTable.EnumDetails details)
      {
         this.name = name;
         this.details = details;
      }
      public override string ToString()
      {
         return "<Specify multiple " + name + " values...>";
      }
   }
   #endregion
}
