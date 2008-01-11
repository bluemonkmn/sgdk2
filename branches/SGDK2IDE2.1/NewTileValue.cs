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
	/// Summary description for Form1.
	/// </summary>
	public class frmNewTileValue : System.Windows.Forms.Form
	{
      private static int previousSelection = -1;

      private System.Windows.Forms.Label lblPrompt;
      private System.Windows.Forms.Label lblTileValue;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.NumericUpDown updTileValue;
      private System.Windows.Forms.RadioButton rdoUnmapped;
      private System.Windows.Forms.Label lblUnmapped;
      private System.Windows.Forms.TextBox txtUnmapped;
      private System.Windows.Forms.RadioButton rdoUndefined;
      private System.Windows.Forms.Label lblUndefined;
      private System.Windows.Forms.TextBox txtUndefined;
      private System.Windows.Forms.RadioButton rdoCustom;
      private System.Windows.Forms.RadioButton rdoEnd;
      private System.Windows.Forms.TextBox txtEnd;
      private System.Windows.Forms.Label lblEnd;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmNewTileValue()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"TileEdit.html");
         SGDK2IDE.g_HelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
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
         this.lblPrompt = new System.Windows.Forms.Label();
         this.lblTileValue = new System.Windows.Forms.Label();
         this.updTileValue = new System.Windows.Forms.NumericUpDown();
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.rdoUnmapped = new System.Windows.Forms.RadioButton();
         this.lblUnmapped = new System.Windows.Forms.Label();
         this.txtUnmapped = new System.Windows.Forms.TextBox();
         this.rdoUndefined = new System.Windows.Forms.RadioButton();
         this.lblUndefined = new System.Windows.Forms.Label();
         this.txtUndefined = new System.Windows.Forms.TextBox();
         this.rdoCustom = new System.Windows.Forms.RadioButton();
         this.rdoEnd = new System.Windows.Forms.RadioButton();
         this.txtEnd = new System.Windows.Forms.TextBox();
         this.lblEnd = new System.Windows.Forms.Label();
         ((System.ComponentModel.ISupportInitialize)(this.updTileValue)).BeginInit();
         this.SuspendLayout();
         // 
         // lblPrompt
         // 
         this.lblPrompt.Location = new System.Drawing.Point(16, 8);
         this.lblPrompt.Name = "lblPrompt";
         this.lblPrompt.Size = new System.Drawing.Size(320, 56);
         this.lblPrompt.TabIndex = 0;
         this.lblPrompt.Text = "Enter a tile value to be defined in the Tileset editor.  This value may exceed th" +
            "e number of frames in the Frameset; if it doesn\'t, the value will no longer refe" +
            "r to the specified frame index.";
         // 
         // lblTileValue
         // 
         this.lblTileValue.Location = new System.Drawing.Point(32, 224);
         this.lblTileValue.Name = "lblTileValue";
         this.lblTileValue.Size = new System.Drawing.Size(80, 20);
         this.lblTileValue.TabIndex = 11;
         this.lblTileValue.Text = "Tile Value:";
         this.lblTileValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // updTileValue
         // 
         this.updTileValue.Location = new System.Drawing.Point(112, 224);
         this.updTileValue.Maximum = new System.Decimal(new int[] {
                                                                     2147483647,
                                                                     0,
                                                                     0,
                                                                     0});
         this.updTileValue.Name = "updTileValue";
         this.updTileValue.Size = new System.Drawing.Size(88, 20);
         this.updTileValue.TabIndex = 12;
         this.updTileValue.Enter += new System.EventHandler(this.updTileValue_Enter);
         // 
         // btnOK
         // 
         this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnOK.Enabled = false;
         this.btnOK.Location = new System.Drawing.Point(344, 8);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(72, 24);
         this.btnOK.TabIndex = 13;
         this.btnOK.Text = "OK";
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(344, 40);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(72, 24);
         this.btnCancel.TabIndex = 14;
         this.btnCancel.Text = "Cancel";
         // 
         // rdoUnmapped
         // 
         this.rdoUnmapped.Location = new System.Drawing.Point(16, 64);
         this.rdoUnmapped.Name = "rdoUnmapped";
         this.rdoUnmapped.Size = new System.Drawing.Size(320, 16);
         this.rdoUnmapped.TabIndex = 1;
         this.rdoUnmapped.Text = "Next unmapped (first value that is not overridden)";
         this.rdoUnmapped.CheckedChanged += new System.EventHandler(this.rdoValueSource_CheckedChanged);
         // 
         // lblUnmapped
         // 
         this.lblUnmapped.Location = new System.Drawing.Point(32, 80);
         this.lblUnmapped.Name = "lblUnmapped";
         this.lblUnmapped.Size = new System.Drawing.Size(80, 20);
         this.lblUnmapped.TabIndex = 2;
         this.lblUnmapped.Text = "Tile Value:";
         this.lblUnmapped.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtUnmapped
         // 
         this.txtUnmapped.Location = new System.Drawing.Point(112, 80);
         this.txtUnmapped.Name = "txtUnmapped";
         this.txtUnmapped.ReadOnly = true;
         this.txtUnmapped.Size = new System.Drawing.Size(88, 20);
         this.txtUnmapped.TabIndex = 3;
         this.txtUnmapped.Text = "";
         // 
         // rdoUndefined
         // 
         this.rdoUndefined.Location = new System.Drawing.Point(16, 112);
         this.rdoUndefined.Name = "rdoUndefined";
         this.rdoUndefined.Size = new System.Drawing.Size(312, 16);
         this.rdoUndefined.TabIndex = 4;
         this.rdoUndefined.Text = "Next undefined (first value without any other meaning)";
         this.rdoUndefined.CheckedChanged += new System.EventHandler(this.rdoValueSource_CheckedChanged);
         // 
         // lblUndefined
         // 
         this.lblUndefined.Location = new System.Drawing.Point(32, 128);
         this.lblUndefined.Name = "lblUndefined";
         this.lblUndefined.Size = new System.Drawing.Size(80, 20);
         this.lblUndefined.TabIndex = 5;
         this.lblUndefined.Text = "Tile Value:";
         this.lblUndefined.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtUndefined
         // 
         this.txtUndefined.Location = new System.Drawing.Point(112, 128);
         this.txtUndefined.Name = "txtUndefined";
         this.txtUndefined.ReadOnly = true;
         this.txtUndefined.Size = new System.Drawing.Size(88, 20);
         this.txtUndefined.TabIndex = 6;
         this.txtUndefined.Text = "";
         // 
         // rdoCustom
         // 
         this.rdoCustom.Location = new System.Drawing.Point(16, 208);
         this.rdoCustom.Name = "rdoCustom";
         this.rdoCustom.Size = new System.Drawing.Size(312, 16);
         this.rdoCustom.TabIndex = 10;
         this.rdoCustom.Text = "Custom";
         this.rdoCustom.CheckedChanged += new System.EventHandler(this.rdoValueSource_CheckedChanged);
         // 
         // rdoEnd
         // 
         this.rdoEnd.Location = new System.Drawing.Point(16, 160);
         this.rdoEnd.Name = "rdoEnd";
         this.rdoEnd.Size = new System.Drawing.Size(312, 16);
         this.rdoEnd.TabIndex = 7;
         this.rdoEnd.Text = "End (after the last tile or frame value)";
         this.rdoEnd.CheckedChanged += new System.EventHandler(this.rdoValueSource_CheckedChanged);
         // 
         // txtEnd
         // 
         this.txtEnd.Location = new System.Drawing.Point(112, 176);
         this.txtEnd.Name = "txtEnd";
         this.txtEnd.ReadOnly = true;
         this.txtEnd.Size = new System.Drawing.Size(88, 20);
         this.txtEnd.TabIndex = 9;
         this.txtEnd.Text = "";
         // 
         // lblEnd
         // 
         this.lblEnd.Location = new System.Drawing.Point(32, 176);
         this.lblEnd.Name = "lblEnd";
         this.lblEnd.Size = new System.Drawing.Size(80, 20);
         this.lblEnd.TabIndex = 8;
         this.lblEnd.Text = "Tile Value:";
         this.lblEnd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // frmNewTileValue
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(426, 255);
         this.ControlBox = false;
         this.Controls.Add(this.txtEnd);
         this.Controls.Add(this.lblEnd);
         this.Controls.Add(this.rdoEnd);
         this.Controls.Add(this.rdoCustom);
         this.Controls.Add(this.txtUndefined);
         this.Controls.Add(this.lblUndefined);
         this.Controls.Add(this.rdoUndefined);
         this.Controls.Add(this.txtUnmapped);
         this.Controls.Add(this.lblUnmapped);
         this.Controls.Add(this.rdoUnmapped);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.updTileValue);
         this.Controls.Add(this.lblTileValue);
         this.Controls.Add(this.lblPrompt);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Name = "frmNewTileValue";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Specify New Tile Value";
         ((System.ComponentModel.ISupportInitialize)(this.updTileValue)).EndInit();
         this.ResumeLayout(false);

      }
		#endregion

      public static int PromptForNewTileValue(IWin32Window owner, ProjectDataset.TilesetRow drTileset, int defaultIndex)
      {
         frmNewTileValue frm = new frmNewTileValue();

         ProjectDataset.TileRow[] tiles = ProjectData.GetSortedTileRows(drTileset);
         ProjectDataset.FrameRow[] frames = ProjectData.GetSortedFrameRows(drTileset.FramesetRow);
         if (frames.Length == 0)
         {
            MessageBox.Show(frm, "Add some frames to the frameset first.", "Specify New Tile Value", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return -1;
         }

         switch(previousSelection)
         {
            case 0:
               frm.rdoUnmapped.Checked = true;
               break;
            case 1:
               frm.rdoUndefined.Checked = true;
               break;
            case 2:
               frm.rdoEnd.Checked = true;
               break;
            case 3:
               frm.rdoCustom.Checked = true;
               break;
         }

         int lastFrame = frames[frames.Length-1].FrameValue;
         if ((tiles.Length == 0) || (tiles[0].TileValue > 0))
            frm.txtUnmapped.Text = "0";
         else
            for (int i=1; i<tiles.Length; i++)
            {
               if ((frm.txtUnmapped.Text.Length == 0) && (tiles[i].TileValue > tiles[i-1].TileValue + 1))
               {
                  frm.txtUnmapped.Text = (tiles[i-1].TileValue + 1).ToString();
                  break;
               }
            }

         int undefined = lastFrame + 1;
         for (int i=0; (i<tiles.Length) && (tiles[i].TileValue <= undefined); i++)
            if (tiles[i].TileValue >= undefined)
               undefined = tiles[i].TileValue + 1;

         for (int i=0; i<tiles.Length; i++)
            if (tiles[i].TileValue == defaultIndex)
            {
               defaultIndex = -1;
               break;
            }

         frm.txtUndefined.Text = undefined.ToString();
         
         if (frm.txtUnmapped.Text.Length == 0)
         {
            frm.txtUnmapped.Text = (tiles[tiles.Length-1].TileValue + 1).ToString();
         }

         if (defaultIndex >= 0)
         {
            frm.updTileValue.Value = defaultIndex;
            frm.rdoCustom.Checked = true;
         }
 
         if (tiles.Length > 0)
         {
            if (tiles[tiles.Length-1].TileValue > lastFrame)
               frm.txtEnd.Text = (tiles[tiles.Length-1].TileValue + 1).ToString();
            else
               frm.txtEnd.Text = (lastFrame + 1).ToString();
         }
         else
            frm.txtEnd.Text = (lastFrame + 1).ToString();

         while(true)
         {
            if (DialogResult.OK == frm.ShowDialog(owner))
            {
               if (frm.rdoUnmapped.Checked)
               {
                  previousSelection = 0;
                  return int.Parse(frm.txtUnmapped.Text);
               }
               else if (frm.rdoUndefined.Checked)
               {
                  previousSelection = 1;
                  return int.Parse(frm.txtUndefined.Text);
               }
               else if (frm.rdoEnd.Checked)
               {
                  previousSelection = 2;
                  return int.Parse(frm.txtEnd.Text);
               }
               else
               {
                  int result = (int)frm.updTileValue.Value;
                  int i;
                  for (i=0; i<tiles.Length; i++)
                     if (tiles[i].TileValue == result)
                        break;
                  if (i<tiles.Length)
                     MessageBox.Show(owner, "Tile index " + result.ToString() + " has already been mapped.  You must enter a tile index that has not been explicitly mapped by this Tileset.", "Specify New Tile Value", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  else
                  {
                     previousSelection = 3;
                     return result;
                  }
               }
            }
            else
               return -1;
         }
      }

      private void rdoValueSource_CheckedChanged(object sender, System.EventArgs e)
      {
         btnOK.Enabled = true;
      }

      private void updTileValue_Enter(object sender, System.EventArgs e)
      {
         rdoCustom.Checked = true;
      }
	}
}
