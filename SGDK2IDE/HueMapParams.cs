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
	/// Summary description for HueMapParams.
	/// </summary>
	public class frmHueMapParams : System.Windows.Forms.Form
	{
      private Bitmap m_ColorWheel = null;
      private System.Windows.Forms.Panel pnlSourceRange;
      private System.Windows.Forms.Panel pnlTargetRange;
      private int wheelSize = 100;

      public event System.EventHandler Preview = null;

      public float sourceStartHue = float.NaN;
      public float sourceHueRange = float.NaN;
      public float targetStartHue = float.NaN;
      public float targetHueRange = float.NaN;
      private System.Windows.Forms.Label lblSourceRange;
      private System.Windows.Forms.Label lblTargetRange;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Label lblInstructions;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmHueMapParams()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         m_ColorWheel = ColorSel.MakeColorWheel(wheelSize);
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
         if (m_ColorWheel != null)
         {
            m_ColorWheel.Dispose();
            m_ColorWheel = null;
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
         this.pnlSourceRange = new System.Windows.Forms.Panel();
         this.pnlTargetRange = new System.Windows.Forms.Panel();
         this.lblSourceRange = new System.Windows.Forms.Label();
         this.lblTargetRange = new System.Windows.Forms.Label();
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.lblInstructions = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // pnlSourceRange
         // 
         this.pnlSourceRange.BackColor = System.Drawing.Color.White;
         this.pnlSourceRange.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.pnlSourceRange.Location = new System.Drawing.Point(8, 56);
         this.pnlSourceRange.Name = "pnlSourceRange";
         this.pnlSourceRange.Size = new System.Drawing.Size(120, 120);
         this.pnlSourceRange.TabIndex = 0;
         this.pnlSourceRange.Paint += new System.Windows.Forms.PaintEventHandler(this.WheelPanel_Paint);
         this.pnlSourceRange.MouseMove += new System.Windows.Forms.MouseEventHandler(this.WheelPanel_MouseMove);
         this.pnlSourceRange.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WheelPanel_MouseDown);
         // 
         // pnlTargetRange
         // 
         this.pnlTargetRange.BackColor = System.Drawing.Color.White;
         this.pnlTargetRange.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.pnlTargetRange.Location = new System.Drawing.Point(152, 56);
         this.pnlTargetRange.Name = "pnlTargetRange";
         this.pnlTargetRange.Size = new System.Drawing.Size(120, 120);
         this.pnlTargetRange.TabIndex = 1;
         this.pnlTargetRange.Paint += new System.Windows.Forms.PaintEventHandler(this.WheelPanel_Paint);
         this.pnlTargetRange.MouseMove += new System.Windows.Forms.MouseEventHandler(this.WheelPanel_MouseMove);
         this.pnlTargetRange.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WheelPanel_MouseDown);
         // 
         // lblSourceRange
         // 
         this.lblSourceRange.Location = new System.Drawing.Point(8, 40);
         this.lblSourceRange.Name = "lblSourceRange";
         this.lblSourceRange.Size = new System.Drawing.Size(120, 16);
         this.lblSourceRange.TabIndex = 2;
         this.lblSourceRange.Text = "Source Hue Range:";
         // 
         // lblTargetRange
         // 
         this.lblTargetRange.Location = new System.Drawing.Point(152, 40);
         this.lblTargetRange.Name = "lblTargetRange";
         this.lblTargetRange.Size = new System.Drawing.Size(120, 16);
         this.lblTargetRange.TabIndex = 3;
         this.lblTargetRange.Text = "Target Hue Range:";
         // 
         // btnOK
         // 
         this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnOK.Location = new System.Drawing.Point(120, 192);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(72, 24);
         this.btnOK.TabIndex = 4;
         this.btnOK.Text = "OK";
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(200, 192);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(72, 24);
         this.btnCancel.TabIndex = 5;
         this.btnCancel.Text = "Cancel";
         // 
         // lblInstructions
         // 
         this.lblInstructions.Location = new System.Drawing.Point(8, 0);
         this.lblInstructions.Name = "lblInstructions";
         this.lblInstructions.Size = new System.Drawing.Size(264, 40);
         this.lblInstructions.TabIndex = 6;
         this.lblInstructions.Text = "Drag a color range in the source and target.  Preview the results in the graphic " +
            "editor window.";
         this.lblInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // frmHueMapParams
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(282, 223);
         this.Controls.Add(this.lblInstructions);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.lblTargetRange);
         this.Controls.Add(this.lblSourceRange);
         this.Controls.Add(this.pnlTargetRange);
         this.Controls.Add(this.pnlSourceRange);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.Name = "frmHueMapParams";
         this.Text = "Remap Hue Parameters";
         this.ResumeLayout(false);

      }
		#endregion

      private void WheelPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         e.Graphics.DrawImageUnscaled(m_ColorWheel,
            (((Panel)sender).ClientSize.Width - wheelSize) / 2,
            (((Panel)sender).ClientSize.Height - wheelSize) / 2);

         float start;
         float range;
         if (sender == pnlSourceRange)
         {
            start = -sourceStartHue;
            range = -sourceHueRange;
         }
         else
         {
            start = -targetStartHue;
            range = -targetHueRange;
         }

         if (float.IsNaN(start) || float.IsNaN(range))
            return;

         using (Pen pen = new Pen(Color.Black, 4))
         {
            pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Rectangle arcRect = Rectangle.Inflate(((Panel)sender).ClientRectangle, -5, -5);
            e.Graphics.DrawArc(pen, arcRect, start, range);
            using (Brush pie = new SolidBrush(Color.FromArgb(64, 0, 0, 192)))
            {
               e.Graphics.FillPie(pie, arcRect, start, range);
            }
         }
      }

      private void WheelPanel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         float hue = (float)(Math.Atan2(((Panel)sender).Width / 2 - e.Y, e.X - ((Panel)sender).Height / 2) * 180.0 / Math.PI);
         if (hue < 0) hue += 360.0f;

         if (sender == pnlSourceRange)
         {
            sourceStartHue = hue;
            sourceHueRange = float.NaN;
         }
         else
         {
            targetStartHue = hue;
            targetHueRange = float.NaN;
         }

         ((Panel)sender).Invalidate();
      }

      private void WheelPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         if (0 != (e.Button & MouseButtons.Left))
         {
            float hue = (float)(Math.Atan2(((Panel)sender).Width / 2 - e.Y, e.X - ((Panel)sender).Height / 2) * 180.0 / Math.PI);
            if (hue < 0) hue += 360.0f;
            float startHue;
            float hueRange;
            if (sender == pnlSourceRange)
            {
               startHue = sourceStartHue;
               hueRange = sourceHueRange;
            }
            else
            {
               startHue = targetStartHue;
               hueRange = targetHueRange;
            }

            if (float.IsNaN(startHue))
               return;

            if (float.IsNaN(hueRange))
            {
               hueRange = hue - startHue;
               if (hueRange < -180.0f)
                  hueRange += 360.0f;
               else if (hueRange > 180.0f)
                  hueRange -= 360.0f;
            }
            else
            {
               float newRange = hue - startHue;
               if (Math.Abs(newRange - hueRange) > Math.Abs(newRange + 360.0f - hueRange))
                  hueRange = newRange + 360.0f;
               else if (Math.Abs(newRange - hueRange) > Math.Abs(newRange - 360.0f - hueRange))
                  hueRange = newRange - 360.0f;
               else
                  hueRange = newRange;
            }
            while(hueRange <= -360.0f)
               hueRange += 360.0f;
            while (hueRange >= 360.0f)
               hueRange -= 360.0f;

            if (sender == pnlSourceRange)
               sourceHueRange = hueRange;
            else
               targetHueRange = hueRange;
            ((Panel)sender).Invalidate();

            if (!float.IsNaN(sourceStartHue) && !float.IsNaN(sourceHueRange) &&
               !float.IsNaN(targetStartHue) && !float.IsNaN(targetHueRange) &&
               Preview != null)
               Preview(this, null);
         }
      }
	}
}
