/*
 * Scrolling Game Development Kit 2.0
 *
 * Copyright © 2000 - 2004 Benjamin Marty <BlueMonkMN@email.com>
 * 
 * Distributed under the GNU General Public License (GPL)
 *   - see included file COPYING.txt for details, or visit:
 *     http://www.fsf.org/copyleft/gpl.html
 */

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace SGDK2
{
	/// <summary>
	/// Summary description for ColorSel.
	/// </summary>
   public class ColorSel : UserControl
   {
      #region Non-control members
      private Bitmap m_bmpColorWheel = null;
      private SelectColorType SelectColorType = SelectColorType.Pen;
      private Bitmap m_bmpValueGradient = null;
      private Bitmap m_bmpAlphaGradient = null;
      public Color m_PenColor = Color.Black;
      public Color m_BrushColor = Color.Black;
      float m_fHue = 0.0f;
      float m_fSat = 0.0f;
      private int m_ActivatingColor;
      Color[] m_RememberedColors = new Color[]
      {
         Color.Black, Color.White, Color.Red, Color.Lime, Color.Blue,
         Color.Yellow, Color.Magenta, Color.Cyan, Color.Orange, Color.SkyBlue,
         Color.DarkRed, Color.Green, Color.DarkBlue, Color.Olive, Color.DarkMagenta,
         Color.DarkCyan, Color.LightGray, Color.DarkGray, Color.DimGray, Color.DarkGreen,
         Color.Brown, Color.Beige, Color.Pink, Color.Transparent
      };
      Point m_DragStart = Point.Empty;
      private static Point RecentColorLocation = new Point(112, 256);
      private const int RecentColorCols = 12;
      private const int RecentColorSize = 14;
      #endregion

      #region Windows Form Designer Components
      private System.Windows.Forms.TrackBar trbValue;
      private System.Windows.Forms.Label lblRed;
      private System.Windows.Forms.TextBox txtRed;
      private System.Windows.Forms.TextBox txtGreen;
      private System.Windows.Forms.Label lblGreen;
      private System.Windows.Forms.TextBox txtBlue;
      private System.Windows.Forms.Label lblBlue;
      private System.Windows.Forms.RadioButton rdoColorPen;
      private System.Windows.Forms.RadioButton rdoColorBrush;
      private System.Windows.Forms.TextBox txtAlpha;
      private System.Windows.Forms.Label lblAlpha;
      private System.Windows.Forms.TrackBar trbAlpha;
      public System.Windows.Forms.RadioButton rdoOverlay;
      public System.Windows.Forms.RadioButton rdoCopy;
      private System.Windows.Forms.Button btnSwapColors;
      private System.Windows.Forms.GroupBox grpColoringMode;
      private System.Windows.Forms.Label lblRecent;

      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.Container components = null;
      #endregion

      #region Initialization and Clean-up
      public ColorSel()
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
         this.SetStyle(ControlStyles.UserPaint, true);

         m_bmpColorWheel = MakeColorWheel(160);
      }

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose( bool disposing )
      {
         if( disposing )
         {
            m_bmpColorWheel.Dispose();
            if (m_bmpValueGradient != null)
            {
               m_bmpValueGradient.Dispose();
               m_bmpValueGradient = null;
            }
            if (m_bmpAlphaGradient != null)
            {
               m_bmpAlphaGradient.Dispose();
               m_bmpAlphaGradient = null;
            }
            if(components != null)
            {
               components.Dispose();
            }
         }
         base.Dispose( disposing );
      }
      #endregion

      #region Windows Form Designer generated code
      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ColorSel));
         this.trbValue = new System.Windows.Forms.TrackBar();
         this.lblRed = new System.Windows.Forms.Label();
         this.txtRed = new System.Windows.Forms.TextBox();
         this.txtGreen = new System.Windows.Forms.TextBox();
         this.lblGreen = new System.Windows.Forms.Label();
         this.txtBlue = new System.Windows.Forms.TextBox();
         this.lblBlue = new System.Windows.Forms.Label();
         this.rdoColorPen = new System.Windows.Forms.RadioButton();
         this.rdoColorBrush = new System.Windows.Forms.RadioButton();
         this.txtAlpha = new System.Windows.Forms.TextBox();
         this.lblAlpha = new System.Windows.Forms.Label();
         this.trbAlpha = new System.Windows.Forms.TrackBar();
         this.rdoOverlay = new System.Windows.Forms.RadioButton();
         this.rdoCopy = new System.Windows.Forms.RadioButton();
         this.grpColoringMode = new System.Windows.Forms.GroupBox();
         this.btnSwapColors = new System.Windows.Forms.Button();
         this.lblRecent = new System.Windows.Forms.Label();
         ((System.ComponentModel.ISupportInitialize)(this.trbValue)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.trbAlpha)).BeginInit();
         this.grpColoringMode.SuspendLayout();
         this.SuspendLayout();
         // 
         // trbValue
         // 
         this.trbValue.Cursor = System.Windows.Forms.Cursors.Arrow;
         this.trbValue.LargeChange = 16;
         this.trbValue.Location = new System.Drawing.Point(176, 0);
         this.trbValue.Maximum = 255;
         this.trbValue.Name = "trbValue";
         this.trbValue.Orientation = System.Windows.Forms.Orientation.Vertical;
         this.trbValue.Size = new System.Drawing.Size(34, 160);
         this.trbValue.TabIndex = 0;
         this.trbValue.TickFrequency = 16;
         this.trbValue.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
         this.trbValue.Scroll += new System.EventHandler(this.trbValue_Scroll);
         // 
         // lblRed
         // 
         this.lblRed.Cursor = System.Windows.Forms.Cursors.Arrow;
         this.lblRed.Location = new System.Drawing.Point(8, 168);
         this.lblRed.Name = "lblRed";
         this.lblRed.Size = new System.Drawing.Size(48, 20);
         this.lblRed.TabIndex = 1;
         this.lblRed.Text = "Red:";
         this.lblRed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtRed
         // 
         this.txtRed.Location = new System.Drawing.Point(56, 168);
         this.txtRed.Name = "txtRed";
         this.txtRed.Size = new System.Drawing.Size(40, 20);
         this.txtRed.TabIndex = 2;
         this.txtRed.Text = "";
         this.txtRed.TextChanged += new System.EventHandler(this.ColorComponent_TextChanged);
         // 
         // txtGreen
         // 
         this.txtGreen.Location = new System.Drawing.Point(56, 192);
         this.txtGreen.Name = "txtGreen";
         this.txtGreen.Size = new System.Drawing.Size(40, 20);
         this.txtGreen.TabIndex = 4;
         this.txtGreen.Text = "";
         this.txtGreen.TextChanged += new System.EventHandler(this.ColorComponent_TextChanged);
         // 
         // lblGreen
         // 
         this.lblGreen.Cursor = System.Windows.Forms.Cursors.Arrow;
         this.lblGreen.Location = new System.Drawing.Point(8, 192);
         this.lblGreen.Name = "lblGreen";
         this.lblGreen.Size = new System.Drawing.Size(48, 20);
         this.lblGreen.TabIndex = 3;
         this.lblGreen.Text = "Green:";
         this.lblGreen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtBlue
         // 
         this.txtBlue.Location = new System.Drawing.Point(56, 216);
         this.txtBlue.Name = "txtBlue";
         this.txtBlue.Size = new System.Drawing.Size(40, 20);
         this.txtBlue.TabIndex = 6;
         this.txtBlue.Text = "";
         this.txtBlue.TextChanged += new System.EventHandler(this.ColorComponent_TextChanged);
         // 
         // lblBlue
         // 
         this.lblBlue.Cursor = System.Windows.Forms.Cursors.Arrow;
         this.lblBlue.Location = new System.Drawing.Point(8, 216);
         this.lblBlue.Name = "lblBlue";
         this.lblBlue.Size = new System.Drawing.Size(48, 20);
         this.lblBlue.TabIndex = 5;
         this.lblBlue.Text = "Blue:";
         this.lblBlue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // rdoColorPen
         // 
         this.rdoColorPen.Appearance = System.Windows.Forms.Appearance.Button;
         this.rdoColorPen.Checked = true;
         this.rdoColorPen.Cursor = System.Windows.Forms.Cursors.Arrow;
         this.rdoColorPen.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.rdoColorPen.Location = new System.Drawing.Point(112, 168);
         this.rdoColorPen.Name = "rdoColorPen";
         this.rdoColorPen.Size = new System.Drawing.Size(48, 24);
         this.rdoColorPen.TabIndex = 9;
         this.rdoColorPen.TabStop = true;
         this.rdoColorPen.Text = "Lines";
         this.rdoColorPen.CheckedChanged += new System.EventHandler(this.rdoColorType_CheckedChanged);
         // 
         // rdoColorBrush
         // 
         this.rdoColorBrush.Appearance = System.Windows.Forms.Appearance.Button;
         this.rdoColorBrush.Cursor = System.Windows.Forms.Cursors.Arrow;
         this.rdoColorBrush.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.rdoColorBrush.Location = new System.Drawing.Point(200, 168);
         this.rdoColorBrush.Name = "rdoColorBrush";
         this.rdoColorBrush.Size = new System.Drawing.Size(48, 24);
         this.rdoColorBrush.TabIndex = 10;
         this.rdoColorBrush.Text = "Solids";
         this.rdoColorBrush.CheckedChanged += new System.EventHandler(this.rdoColorType_CheckedChanged);
         // 
         // txtAlpha
         // 
         this.txtAlpha.Location = new System.Drawing.Point(56, 240);
         this.txtAlpha.Name = "txtAlpha";
         this.txtAlpha.Size = new System.Drawing.Size(40, 20);
         this.txtAlpha.TabIndex = 8;
         this.txtAlpha.Text = "";
         this.txtAlpha.TextChanged += new System.EventHandler(this.ColorComponent_TextChanged);
         // 
         // lblAlpha
         // 
         this.lblAlpha.Cursor = System.Windows.Forms.Cursors.Arrow;
         this.lblAlpha.Location = new System.Drawing.Point(8, 240);
         this.lblAlpha.Name = "lblAlpha";
         this.lblAlpha.Size = new System.Drawing.Size(48, 20);
         this.lblAlpha.TabIndex = 7;
         this.lblAlpha.Text = "Alpha:";
         this.lblAlpha.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // trbAlpha
         // 
         this.trbAlpha.Cursor = System.Windows.Forms.Cursors.Arrow;
         this.trbAlpha.LargeChange = 16;
         this.trbAlpha.Location = new System.Drawing.Point(224, 0);
         this.trbAlpha.Maximum = 255;
         this.trbAlpha.Name = "trbAlpha";
         this.trbAlpha.Orientation = System.Windows.Forms.Orientation.Vertical;
         this.trbAlpha.Size = new System.Drawing.Size(34, 160);
         this.trbAlpha.TabIndex = 11;
         this.trbAlpha.TickFrequency = 16;
         this.trbAlpha.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
         this.trbAlpha.Value = 255;
         this.trbAlpha.ValueChanged += new System.EventHandler(this.trbAlpha_ValueChanged);
         // 
         // rdoOverlay
         // 
         this.rdoOverlay.Cursor = System.Windows.Forms.Cursors.Arrow;
         this.rdoOverlay.Location = new System.Drawing.Point(8, 16);
         this.rdoOverlay.Name = "rdoOverlay";
         this.rdoOverlay.Size = new System.Drawing.Size(80, 16);
         this.rdoOverlay.TabIndex = 12;
         this.rdoOverlay.Text = "Overlay";
         this.rdoOverlay.CheckedChanged += new System.EventHandler(this.rdoCompositeMode_CheckedChanged);
         // 
         // rdoCopy
         // 
         this.rdoCopy.Cursor = System.Windows.Forms.Cursors.Arrow;
         this.rdoCopy.Location = new System.Drawing.Point(88, 16);
         this.rdoCopy.Name = "rdoCopy";
         this.rdoCopy.Size = new System.Drawing.Size(72, 16);
         this.rdoCopy.TabIndex = 13;
         this.rdoCopy.Text = "Copy";
         this.rdoCopy.CheckedChanged += new System.EventHandler(this.rdoCompositeMode_CheckedChanged);
         // 
         // grpColoringMode
         // 
         this.grpColoringMode.Controls.Add(this.rdoOverlay);
         this.grpColoringMode.Controls.Add(this.rdoCopy);
         this.grpColoringMode.Cursor = System.Windows.Forms.Cursors.Arrow;
         this.grpColoringMode.Location = new System.Drawing.Point(112, 200);
         this.grpColoringMode.Name = "grpColoringMode";
         this.grpColoringMode.Size = new System.Drawing.Size(168, 40);
         this.grpColoringMode.TabIndex = 14;
         this.grpColoringMode.TabStop = false;
         this.grpColoringMode.Text = "Coloring Mode";
         // 
         // btnSwapColors
         // 
         this.btnSwapColors.Cursor = System.Windows.Forms.Cursors.Arrow;
         this.btnSwapColors.Image = ((System.Drawing.Image)(resources.GetObject("btnSwapColors.Image")));
         this.btnSwapColors.Location = new System.Drawing.Point(184, 168);
         this.btnSwapColors.Name = "btnSwapColors";
         this.btnSwapColors.Size = new System.Drawing.Size(16, 24);
         this.btnSwapColors.TabIndex = 15;
         this.btnSwapColors.Click += new System.EventHandler(this.btnSwapColors_Click);
         // 
         // lblRecent
         // 
         this.lblRecent.Location = new System.Drawing.Point(112, 240);
         this.lblRecent.Name = "lblRecent";
         this.lblRecent.Size = new System.Drawing.Size(112, 16);
         this.lblRecent.TabIndex = 16;
         this.lblRecent.Text = "Recent Color List:";
         // 
         // ColorSel
         // 
         this.Controls.Add(this.lblRecent);
         this.Controls.Add(this.btnSwapColors);
         this.Controls.Add(this.grpColoringMode);
         this.Controls.Add(this.trbAlpha);
         this.Controls.Add(this.txtAlpha);
         this.Controls.Add(this.txtBlue);
         this.Controls.Add(this.txtGreen);
         this.Controls.Add(this.txtRed);
         this.Controls.Add(this.lblAlpha);
         this.Controls.Add(this.rdoColorBrush);
         this.Controls.Add(this.rdoColorPen);
         this.Controls.Add(this.lblBlue);
         this.Controls.Add(this.lblGreen);
         this.Controls.Add(this.lblRed);
         this.Controls.Add(this.trbValue);
         this.Cursor = System.Windows.Forms.Cursors.Default;
         this.Name = "ColorSel";
         this.Size = new System.Drawing.Size(282, 272);
         ((System.ComponentModel.ISupportInitialize)(this.trbValue)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.trbAlpha)).EndInit();
         this.grpColoringMode.ResumeLayout(false);
         this.ResumeLayout(false);

      }
      #endregion

      #region Public Methods
      public void ActivateColor(Color clr)
      {
         m_ActivatingColor++;

         if (ParentEditor == null)
            return;

         if (SelectColorType == SelectColorType.Pen)
         {
            ParentEditor.CurrentPen.Color = clr;
            m_PenColor = clr;
         }
         else if (SelectColorType == SelectColorType.Brush)
         {
            if (ParentEditor.CurrentBrush != null)
               ParentEditor.CurrentBrush.Dispose();
            ParentEditor.CurrentBrush = new SolidBrush(clr);
            m_BrushColor = clr;
         }
         Graphics g = Graphics.FromHwnd(Handle);
         DrawCurrentColors(g);
         g.Dispose();

         if (txtRed.Text != clr.R.ToString())
         {
            txtAlpha.Text = "...";
            txtRed.Text = clr.R.ToString();
         }
         if (txtGreen.Text != clr.G.ToString())
         {
            txtAlpha.Text = "...";
            txtGreen.Text = clr.G.ToString();
         }
         if (txtBlue.Text != clr.B.ToString())
         {
            txtAlpha.Text = "...";
            txtBlue.Text = clr.B.ToString();
         }
         if (txtAlpha.Text != clr.A.ToString())
            txtAlpha.Text = clr.A.ToString();

         if (Color.FromArgb(trbAlpha.Value, HSVtoRGB(m_fHue, m_fSat, trbValue.Value / 255.0f)) != clr)
         {
            float fVal;
            RGBtoHSV(clr, out m_fHue, out m_fSat, out fVal);
            trbValue.Value = (int)Math.Round(fVal * 255.0);
            trbAlpha.Value = clr.A;
            RedrawValueGradient();
         }
         
         int nMax = clr.R;
         if (clr.G > nMax)
            nMax = clr.G;
         if (clr.B > nMax)
            nMax = clr.B;

         if (nMax != trbValue.Value)
         {
            trbValue.Value = nMax;
            ReadCurrentColor();
         }
         m_ActivatingColor--;
      }

      public void RememberColor(Color clr)
      {
         int nOldIdx = m_RememberedColors.Length - 1;

         for (int i=0; i<m_RememberedColors.Length; i++)
         {
            if (m_RememberedColors[i].ToArgb() == clr.ToArgb())
            {
               nOldIdx = i;
               break;
            }
         }

         for (int i=nOldIdx; i > 0; i--)
            m_RememberedColors[i] = m_RememberedColors[i-1];
         m_RememberedColors[0] = clr;         
      }

      public void RememberColors()
      {
         if (ParentEditor == null)
            return;

         if ((0 != (ParentEditor.CurrentOptions & ToolOptions.Outline))
            && (ParentEditor.CurrentTool != DrawingTool.Erase))
            RememberColor(m_PenColor);

         if ((0 != (ParentEditor.CurrentOptions & ToolOptions.Fill))
            && (ParentEditor.CurrentTool != DrawingTool.Erase)
            && (ParentEditor.CurrentTool != DrawingTool.Line))
            RememberColor(m_BrushColor);

         DrawRememberedColors(null);
      }
      #endregion

      #region Private Methods
      /// <summary>
      /// Get an RGB color with 100% brightness from a hue and saturation
      /// </summary>
      /// <param name="dblHue">Number from 0 to 2*pi</param>
      /// <param name="dblSat">Number from 0 to 1 where 1 is maximum saturation and 0 is gray</param>
      private Color HStoRGB(double dblHue, double dblSat)
      {
         double RangedAngle = dblHue * 3.0 / Math.PI;
         int red, green, blue;

         if (RangedAngle < 1.0)
         {
            red = 255;
            green = (int)(Math.Round(255.0 * (1.0 - dblSat) + RangedAngle * 255.0 * dblSat));
            blue = (int)(Math.Round(255.0 * (1.0 - dblSat)));
         } 
         else if (RangedAngle < 2.0)
         {
            RangedAngle = 2.0 - RangedAngle;
            red = (int)(Math.Round(255.0 * (1.0 - dblSat) + RangedAngle * 255.0 * dblSat));
            green = 255;
            blue = (int)(Math.Round(255.0 * (1.0 - dblSat)));
         }
         else if (RangedAngle < 3.0)
         {
            RangedAngle -= 2.0;
            red = (int)(Math.Round(255.0 * (1.0 - dblSat)));
            green = 255;
            blue = (int)(Math.Round(255.0 * (1.0 - dblSat) + RangedAngle * 255.0 * dblSat));
         }
         else if (RangedAngle < 4.0)
         {
            RangedAngle = 4.0 - RangedAngle;
            red = (int)(Math.Round(255.0 * (1.0 - dblSat)));
            green = (int)(Math.Round(255.0 * (1.0 - dblSat) + RangedAngle * 255.0 * dblSat));
            blue = 255;
         }
         else if (RangedAngle < 5.0)
         {
            RangedAngle -= 4.0;
            red = (int)(Math.Round(255.0 * (1.0 - dblSat) + RangedAngle * 255.0 * dblSat));
            green = (int)(Math.Round(255.0 * (1.0 - dblSat)));
            blue = 255;
         }
         else
         {
            RangedAngle = 6.0 - RangedAngle;
            red = 255;
            green = (int)(Math.Round(255.0 * (1.0 - dblSat)));
            blue = (int)(Math.Round(255.0 * (1.0 - dblSat) + RangedAngle * 255.0 * dblSat));
         }
         return Color.FromArgb(red, green, blue);
      }

      private Color HSVtoRGB(double dblHue, double dblSat, double dblVal)
      {
         Color SelColor = HStoRGB(m_fHue, m_fSat);
         SelColor = Color.FromArgb(
            (int)Math.Round(SelColor.R * dblVal),
            (int)Math.Round(SelColor.G * dblVal),
            (int)Math.Round(SelColor.B * dblVal));
         return SelColor;
      }

      private float ScaleComponents(int high, int mid, int low)
      {
         float fHigh, fMid, fLow;
         if (high > 0)
         {
            fHigh = 1.0f;
            fMid = mid / (float)high;
            fLow = low / (float)high;
         }
         else
         {
            fHigh = fMid = fLow = 0.0f;
         }

         if ((fLow > 0.0f) && (fLow < 1.0f))
         {
            fMid = 1.0f - (1.0f - fMid) / (1.0f - fLow);
            fLow = 0.0f;
         }

         return fMid;
      }

      private void RGBtoHSV(Color clr, out float hue, out float sat, out float val)
      {
         hue = sat = val = 0.0f;

         if ((clr.R == 0) && (clr.G == 0) && (clr.B == 0))
            return;

         if ((clr.R >= clr.G) && (clr.G >= clr.B)) // RGB
         {
            hue = (float)(ScaleComponents(clr.R, clr.G, clr.B) * Math.PI / 3.0);
            sat = (clr.R - clr.B) / (float)clr.R;
            val = clr.R / 255.0f;
         }
         else if ((clr.G >= clr.R) && (clr.R >= clr.B)) // GRB
         {
            hue =(float)(Math.PI * (2.0 - ScaleComponents(clr.G, clr.R, clr.B)) / 3.0);
            sat = (clr.G - clr.B) / (float)clr.G;
            val = clr.G / 255.0f;
         }
         else if ((clr.G >= clr.B) && (clr.B >= clr.R)) // GBR
         {
            hue = (float)((2.0 + ScaleComponents(clr.G, clr.B, clr.R)) * Math.PI / 3.0);
            sat = (clr.G - clr.R) / (float)clr.G;
            val = clr.G / 255.0f;
         }
         else if ((clr.B >= clr.G) && (clr.G >= clr.R)) // BGR
         {
            hue = (float)(Math.PI * (4.0 - ScaleComponents(clr.B, clr.G, clr.R)) / 3.0);
            sat = (clr.B - clr.R) / (float)clr.B;
            val = clr.B / 255.0f;
         }
         else if ((clr.B >= clr.R) && (clr.R >= clr.G)) // BRG
         {
            hue = (float)((4.0 + ScaleComponents(clr.B, clr.R, clr.G)) * Math.PI / 3.0);
            sat = (clr.B - clr.G) / (float)clr.B;
            val = clr.B / 255.0f;
         }
         else if ((clr.R >= clr.B) && (clr.B >= clr.G)) // RBG
         {
            hue = (float)(Math.PI * (6.0 - ScaleComponents(clr.R, clr.B, clr.G)) / 3.0);
            sat = (clr.R - clr.G) / (float)clr.R;
            val = clr.R / 255.0f;
         }
      }

      private Bitmap MakeColorWheel(int Diameter)
      {
         int Ctr = Diameter / 2;

         Bitmap bmpWheel = new Bitmap(Diameter, Diameter, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

         Graphics g = Graphics.FromImage(bmpWheel);
         g.FillRectangle(Brushes.White, 0, 0, Diameter, Diameter);
         g.Dispose();

         for (int Y = 0; Y < Diameter; Y++)
         {
            for (int X = 0; X < Diameter; X++)
            {
               double angle = Math.Atan2(Ctr - Y, X - Ctr);
               if (angle < 0)
                  angle += Math.PI * 2;
               double dist = Math.Sqrt( (Ctr - Y) * (Ctr - Y) + (X - Ctr) * (X-Ctr));
               if (dist < Ctr * .9)
                  bmpWheel.SetPixel(X, Y, HStoRGB(angle, dist * 2.22 / Diameter));
               else if (dist < Ctr)
                  bmpWheel.SetPixel(X, Y, HStoRGB(angle, 1));
            }
         }
         return bmpWheel;
      }

      private Bitmap MakeValueGradient(int nWidth, int nHeight)
      {
         Brush bGrad = new System.Drawing.Drawing2D.LinearGradientBrush(
            new Rectangle(0,0,nWidth,nHeight), HStoRGB(m_fHue, m_fSat), Color.Black, 90);
         Bitmap bmpGrad = new Bitmap(nWidth, nHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
         Graphics g = Graphics.FromImage(bmpGrad);
         g.FillRectangle(bGrad,0,0,nWidth,nHeight);
         bGrad.Dispose();
         g.Dispose();
         return bmpGrad;
      }
      
      private Bitmap MakeAlphaGradient(int nWidth, int nHeight)
      {
         Bitmap bmpAlpha = new Bitmap(nWidth, nHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
         Brush brFill = new HatchBrush(HatchStyle.SolidDiamond, Color.Gray, Color.White);
         Graphics g = Graphics.FromImage(bmpAlpha);
         g.FillRectangle(brFill, 0, 0, nWidth, nHeight);
         brFill.Dispose();
         brFill = new LinearGradientBrush(new Rectangle(0, 0, nWidth, nHeight),
            HSVtoRGB(m_fHue, m_fSat, trbValue.Value / 255.0), Color.Transparent, LinearGradientMode.Vertical);
         g.FillRectangle(brFill, 0, 0, nWidth, nHeight);
         brFill.Dispose();
         g.Dispose();
         return bmpAlpha;
      }

      private frmGraphicsEditor ParentEditor
      {
         get
         {
            return Parent as frmGraphicsEditor;
         }
      }

      private void DrawCurrentColors(Graphics g)
      {
         Brush bPat = new HatchBrush(HatchStyle.SolidDiamond, Color.Gray, Color.White);
         Brush bClr = new SolidBrush(m_BrushColor);
         g.FillRectangle(bPat, rdoColorBrush.Right, rdoColorBrush.Top, 20, rdoColorBrush.Height);
         g.FillRectangle(bClr,rdoColorBrush.Right, rdoColorBrush.Top, 20, rdoColorBrush.Height);
         bClr.Dispose();
         bClr = new SolidBrush(m_PenColor);
         g.FillRectangle(bPat, rdoColorPen.Right, rdoColorPen.Top, 20, rdoColorPen.Height);
         g.FillRectangle(bClr, rdoColorPen.Right, rdoColorPen.Top, 20, rdoColorPen.Height);
         bClr.Dispose();
         bPat.Dispose();
      }

      private void DrawRememberedColors(Graphics g)
      {
         Bitmap bmpBuffer = new Bitmap(168, ((m_RememberedColors.Length + RecentColorCols - 1) / RecentColorCols) * RecentColorSize, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
         Graphics gBuf = Graphics.FromImage(bmpBuffer);

         Brush bPat = new HatchBrush(HatchStyle.SolidDiamond, Color.Gray, Color.White);
         gBuf.FillRectangle(bPat, 0, 0, bmpBuffer.Width, bmpBuffer.Height);
         bPat.Dispose();
         for (int i=0; i<m_RememberedColors.Length; i++)
         {
            Brush bClr = new SolidBrush(m_RememberedColors[i]);
            gBuf.FillRectangle(bClr, (i % RecentColorCols) * RecentColorSize, (i / RecentColorCols) * RecentColorSize, RecentColorSize, RecentColorSize);
            bClr.Dispose();
         }
         gBuf.Dispose();

         if (g == null)
         {
            g = Graphics.FromHwnd(this.Handle);
            g.DrawImageUnscaled(bmpBuffer, RecentColorLocation.X, RecentColorLocation.Y);
            g.Dispose();
         }
         else
            g.DrawImageUnscaled(bmpBuffer, RecentColorLocation.X, RecentColorLocation.Y);
         bmpBuffer.Dispose();
      }

      private void RedrawValueGradient()
      {
         if (m_bmpValueGradient != null)
            m_bmpValueGradient.Dispose();
         m_bmpValueGradient = MakeValueGradient(8, 144);
         if (m_bmpAlphaGradient != null)
            m_bmpAlphaGradient.Dispose();
         m_bmpAlphaGradient = MakeAlphaGradient(8, 144);
         Graphics g = Graphics.FromHwnd(this.Handle);
         g.DrawImageUnscaled(m_bmpValueGradient, trbValue.Left - 8, 8);
         g.DrawImageUnscaled(m_bmpAlphaGradient, trbAlpha.Left - 8, 8);
         g.Dispose();
      }

      private void ReadCurrentColor()
      {
         if (m_ActivatingColor > 0)
            return;
         RedrawValueGradient();
         ActivateColor(Color.FromArgb(trbAlpha.Value, HSVtoRGB(m_fHue, m_fSat, trbValue.Value / 255f)));
      }
      #endregion

      #region Public Properties
      public SelectColorType CurrentColorType
      {
         get
         {
            return SelectColorType;
         }
         set
         {
            SelectColorType = value;
            if (value == SelectColorType.Brush)
               rdoColorBrush.Checked = true;
            else
               rdoColorPen.Checked = true;
         }
      }
      #endregion

      #region Overrides
      protected override CreateParams CreateParams
      {
         get
         {
            CreateParams cp = base.CreateParams;
            cp.Style |= DragPanel.WS_DLGFRAME;
            cp.ExStyle |= DragPanel.WS_EX_TOOLWINDOW;
            return cp;
         }
      }

      [Browsable(true),
       DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      public override string Text
      {
         get
         {
            return base.Text;
         }
         set
         {
            base.Text = value;
         }
      }

      protected override void OnLoad(EventArgs e)
      {
         ReadCurrentColor();
         rdoOverlay.Checked = true;      
         base.OnLoad (e);
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint (e);
         e.Graphics.DrawImageUnscaled(m_bmpColorWheel,0,0);
         if (m_bmpValueGradient != null)
            e.Graphics.DrawImageUnscaled(m_bmpValueGradient, trbValue.Left - 8, 8);
         if (m_bmpAlphaGradient != null)
            e.Graphics.DrawImageUnscaled(m_bmpAlphaGradient, trbAlpha.Left - 8, 8);
         DrawCurrentColors(e.Graphics);
         DrawRememberedColors(e.Graphics);
      }

      protected override void OnMouseMove(MouseEventArgs e)
      {
         Boolean bOverColorWheel = ((e.X < 160) && (e.Y < 160) && (e.X >= 0) && (e.Y >= 0));
         Boolean bOverRecentColors = OverRecentColors(new Point(e.X, e.Y));

         if (m_DragStart != Point.Empty)
         {
            Point ptNewLoc = Location;
            ptNewLoc.Offset(e.X - m_DragStart.X, e.Y - m_DragStart.Y);
            Location = ptNewLoc;
         }

         if ((bOverColorWheel) || (bOverRecentColors))
            this.Cursor = Cursors.Cross;
         else
            this.Cursor = Cursors.SizeAll;

         if ((e.Button == MouseButtons.Left) || (e.Button == MouseButtons.Right))
         {
            if (bOverColorWheel)
            {
               m_fHue = (float)Math.Atan2(m_bmpColorWheel.Height / 2 - e.Y, e.X - m_bmpColorWheel.Width / 2);
               if (m_fHue < 0) m_fHue += (float)Math.PI * 2.0f;
               m_fSat = (float)Math.Sqrt((e.X - m_bmpColorWheel.Width / 2) * (e.X - m_bmpColorWheel.Width / 2) +
                  (e.Y - m_bmpColorWheel.Height / 2) * (e.Y - m_bmpColorWheel.Height / 2))
                  * 2.22f / (float)m_bmpColorWheel.Width;
               if (m_fSat > 1.11)
               {
                  m_fSat = 0.0f;
                  m_fHue = 0.0f;
               }
               else if (m_fSat > 1.0)
               {
                  m_fSat = 1.0f;
               }

               ReadCurrentColor();
            }
            else if (bOverRecentColors)
               ActivateColor(m_RememberedColors[GetRecentColorIndex(new Point(e.X, e.Y))]);
         }
         base.OnMouseMove (e);
      }

      protected override void OnMouseDown(MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Right)
         {
            if (SelectColorType == SelectColorType.Brush)
               CurrentColorType = SelectColorType.Pen;
            else
               CurrentColorType = SelectColorType.Brush;
         }

         if ((e.X < 160) && (e.Y < 160) && (e.X >= 0) && (e.Y >= 0))
         {
            m_fHue = (float)Math.Atan2(m_bmpColorWheel.Height / 2 - e.Y, e.X - m_bmpColorWheel.Width / 2);
            if (m_fHue < 0) m_fHue += (float)Math.PI * 2.0f;
            m_fSat = (float)Math.Sqrt((e.X - m_bmpColorWheel.Width / 2) * (e.X - m_bmpColorWheel.Width / 2) +
               (e.Y - m_bmpColorWheel.Height / 2) * (e.Y - m_bmpColorWheel.Height / 2))
               * 2.22f / (float)m_bmpColorWheel.Width;
            if (m_fSat > 1.11)
            {
               m_fSat = 0.0f;
               m_fHue = 0.0f;
            }
            else if (m_fSat > 1.0)
            {
               m_fSat = 1.0f;
            }

            ReadCurrentColor();
         }
         else if (OverRecentColors(new Point(e.X, e.Y)))
         {
            ActivateColor(m_RememberedColors[GetRecentColorIndex(new Point(e.X, e.Y))]);
         }
         else
         {
            m_DragStart = new Point(e.X, e.Y);
            this.BringToFront();
         }

         base.OnMouseDown (e);
      }

      protected override void OnMouseUp(MouseEventArgs e)
      {
         m_DragStart = Point.Empty;

         if (e.Button == MouseButtons.Right)
         {
            if (SelectColorType == SelectColorType.Brush)
               CurrentColorType = SelectColorType.Pen;
            else
               CurrentColorType = SelectColorType.Brush;
         }
         base.OnMouseUp (e);
      }
      #endregion

      #region Event Handlers
      private void trbValue_Scroll(object sender, System.EventArgs e)
      {
         ReadCurrentColor();      
      }
      private void trbAlpha_ValueChanged(object sender, System.EventArgs e)
      {
         ReadCurrentColor();
      }
      private void rdoColorType_CheckedChanged(object sender, System.EventArgs e)
      {
         if (((RadioButton)sender).Checked)
         {
            if (sender == rdoColorBrush)
            {
               SelectColorType = SelectColorType.Brush;
               ActivateColor(m_BrushColor);
            }
            else
            {
               SelectColorType = SelectColorType.Pen;
               ActivateColor(m_PenColor);
            }
         }
      }

      private void ColorComponent_TextChanged(object sender, System.EventArgs e)
      {
         double dblRed, dblGreen, dblBlue, dblAlpha;
         if (double.TryParse(txtRed.Text, System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.CurrentCulture, out dblRed) &&
            double.TryParse(txtGreen.Text, System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.CurrentCulture, out dblGreen) &&
            double.TryParse(txtBlue.Text, System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.CurrentCulture, out dblBlue) &&
            double.TryParse(txtAlpha.Text, System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.CurrentCulture, out dblAlpha))
         {
            if ((dblRed <= 255) && (dblRed >= 0) &&
                (dblGreen <= 255) && (dblGreen >= 0) &&
                (dblGreen <= 255) && (dblGreen >= 0) &&
                (dblAlpha <= 255) && (dblAlpha >= 0))
               ActivateColor(Color.FromArgb((int)dblAlpha, (int)dblRed, (int)dblGreen, (int)dblBlue));
         }
      }

      private void rdoCompositeMode_CheckedChanged(object sender, System.EventArgs e)
      {
         if (ParentEditor == null)
            return;

         if (((RadioButton)sender).Checked)
         {
            if (sender == rdoCopy)
               ParentEditor.CompositingMode = CompositingMode.SourceCopy;
            else
               ParentEditor.CompositingMode = CompositingMode.SourceOver;
         }
      }

      private void btnSwapColors_Click(object sender, System.EventArgs e)
      {
         if (ParentEditor == null)
            return;

         Color clrTemp = m_BrushColor;
         m_BrushColor = m_PenColor;
         m_PenColor = clrTemp;
         ParentEditor.CurrentPen.Color = m_PenColor;
         if (ParentEditor.CurrentBrush != null)
            ParentEditor.CurrentBrush.Dispose();
         ParentEditor.CurrentBrush = new SolidBrush(m_BrushColor);
         if (CurrentColorType == SelectColorType.Brush)
            ActivateColor(m_BrushColor);
         else
            ActivateColor(m_PenColor);
      }

      private bool OverRecentColors(Point e)
      {
         return ((e.X >= RecentColorLocation.X) &&
            (e.X < RecentColorLocation.X + RecentColorCols * RecentColorSize) &&
            (e.Y >= RecentColorLocation.Y) &&
            (e.Y < RecentColorLocation.Y + ((m_RememberedColors.Length + RecentColorCols - 1) / RecentColorCols) * RecentColorSize));
      }

      private int GetRecentColorIndex(Point e)
      {
         return ((e.Y - RecentColorLocation.Y) / RecentColorSize) * RecentColorCols +
            (e.X - RecentColorLocation.X) / RecentColorSize;
      }
      #endregion   
   }

   public enum SelectColorType
   {
      Pen,
      Brush
   }
}
