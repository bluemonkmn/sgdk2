/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace SGDK2
{
	/// <summary>
	/// Summary description for UnsavedChanges.
	/// </summary>
	public class frmUnsavedChanges : System.Windows.Forms.Form
	{
      private System.Windows.Forms.TextBox txtChanges;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmUnsavedChanges()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);
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
         this.txtChanges = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // txtChanges
         // 
         this.txtChanges.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtChanges.Location = new System.Drawing.Point(0, 0);
         this.txtChanges.Multiline = true;
         this.txtChanges.Name = "txtChanges";
         this.txtChanges.Size = new System.Drawing.Size(440, 333);
         this.txtChanges.TabIndex = 0;
         this.txtChanges.Text = "";
         // 
         // frmUnsavedChanges
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(440, 333);
         this.Controls.Add(this.txtChanges);
         this.Name = "frmUnsavedChanges";
         this.Text = "View Unsaved Changes";
         this.ResumeLayout(false);

      }
		#endregion

      #region Overrides
      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad (e);

         System.IO.StringWriter sw = new System.IO.StringWriter();

         foreach(DataTable dt in ProjectData.GetChangedTables())
         {
            sw.WriteLine(dt.TableName);
            foreach(DataRow dr in dt.Rows)
            {
               sw.Write("   " + dr.RowState.ToString() + " ");
               int i = 0;
               foreach(DataColumn dc in dt.PrimaryKey)
               {
                  if (i++>0) sw.Write(",");
                  if (dr.RowState == DataRowState.Deleted)
                     sw.Write(dc.ColumnName + "=" + dr[dc,DataRowVersion.Original].ToString());
                  else
                     sw.Write(dc.ColumnName + "=" + dr[dc].ToString());
               }
               sw.Write(Environment.NewLine);
            }
         }

         txtChanges.Text = sw.ToString();
         sw.Close();
      }
      
      protected override void OnClosing(CancelEventArgs e)
      {
         base.OnClosing (e);
         SGDK2IDE.SaveFormSettings(this);
      }
      #endregion
   }
}
