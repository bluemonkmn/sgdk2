/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for CollisionMask.
	/// </summary>
	public class frmCollisionMask : System.Windows.Forms.Form
	{
      private Bitmap m_bmpTarget;
      private ProjectDataset.SpriteStateRow m_SpriteState;
      private int[] m_SelectedSequences;
      private System.Windows.Forms.TrackBar trbAlphaLevel;
      private System.Windows.Forms.ComboBox cboMode;
      private System.Windows.Forms.Panel pnlPreview;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmCollisionMask(ProjectDataset.SpriteStateRow drSpriteState, int[] SelectedSequences)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         m_SpriteState = drSpriteState;
         m_SelectedSequences = SelectedSequences;
         trbAlphaLevel.Value = ProjectData.GetSortedSpriteFrames(drSpriteState)[SelectedSequences[0]].MaskAlphaLevel;
         cboMode.SelectedIndex = 0;

         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"html/1b29c16a-1be0-4a46-b0b2-1f023b465b15.htm");
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
         if (m_bmpTarget != null)
            m_bmpTarget.Dispose();
         m_bmpTarget = null;
         base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.trbAlphaLevel = new System.Windows.Forms.TrackBar();
         this.cboMode = new System.Windows.Forms.ComboBox();
         this.pnlPreview = new System.Windows.Forms.Panel();
         ((System.ComponentModel.ISupportInitialize)(this.trbAlphaLevel)).BeginInit();
         this.SuspendLayout();
         // 
         // trbAlphaLevel
         // 
         this.trbAlphaLevel.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.trbAlphaLevel.LargeChange = 32;
         this.trbAlphaLevel.Location = new System.Drawing.Point(0, 246);
         this.trbAlphaLevel.Maximum = 255;
         this.trbAlphaLevel.Name = "trbAlphaLevel";
         this.trbAlphaLevel.Size = new System.Drawing.Size(240, 34);
         this.trbAlphaLevel.TabIndex = 1;
         this.trbAlphaLevel.TickFrequency = 32;
         this.trbAlphaLevel.Scroll += new System.EventHandler(this.trbAlphaLevel_Scroll);
         // 
         // cboMode
         // 
         this.cboMode.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.cboMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboMode.Items.AddRange(new object[] {
                                                     "Solid Black on Image",
                                                     "Solid White on Image",
                                                     "Solid Image on Black",
                                                     "Solid Image on White",
                                                     "Solid Black on White",
                                                     "Solid White on Black"});
         this.cboMode.Location = new System.Drawing.Point(0, 225);
         this.cboMode.Name = "cboMode";
         this.cboMode.Size = new System.Drawing.Size(240, 21);
         this.cboMode.TabIndex = 2;
         this.cboMode.SelectedIndexChanged += new System.EventHandler(this.cboMode_SelectedIndexChanged);
         // 
         // pnlPreview
         // 
         this.pnlPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.pnlPreview.Dock = System.Windows.Forms.DockStyle.Fill;
         this.pnlPreview.Location = new System.Drawing.Point(0, 0);
         this.pnlPreview.Name = "pnlPreview";
         this.pnlPreview.Size = new System.Drawing.Size(240, 225);
         this.pnlPreview.TabIndex = 3;
         this.pnlPreview.Resize += new System.EventHandler(this.pnlPreview_Resize);
         this.pnlPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlPreview_Paint);
         // 
         // frmCollisionMask
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(240, 280);
         this.Controls.Add(this.pnlPreview);
         this.Controls.Add(this.cboMode);
         this.Controls.Add(this.trbAlphaLevel);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
         this.MaximizeBox = false;
         this.Name = "frmCollisionMask";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Adjust Collision Mask";
         ((System.ComponentModel.ISupportInitialize)(this.trbAlphaLevel)).EndInit();
         this.ResumeLayout(false);

      }
		#endregion

      private RectangleF GetFrameBounds(ProjectDataset.FrameRow drFrame)
      {
         RectangleF result;
         ProjectDataset.GraphicSheetRow drGfx = ProjectData.GetGraphicSheet(drFrame.GraphicSheet);
         PointF[] arptCorners = new PointF[]
         {
            new PointF(0f, 0f), new PointF(drGfx.CellWidth, 0),
            new PointF(drGfx.CellWidth, drGfx.CellHeight),
            new PointF(0, drGfx.CellHeight)
         };

         using(Matrix mtx = new Matrix(drFrame.m11, drFrame.m12,
                  drFrame.m21, drFrame.m22, drFrame.dx, drFrame.dy))
         {
            mtx.TransformPoints(arptCorners);
         }

         result = new RectangleF(arptCorners[0], new SizeF(0f, 0f));
         for (int i=1; i<arptCorners.Length; i++)
         {
            if (arptCorners[i].X < result.X)
            {
               result.Width += result.X - arptCorners[i].X;
               result.X = arptCorners[i].X;
            }
            else if (arptCorners[i].X - result.X > result.Width)
               result.Width = arptCorners[i].X - result.X;

            if (arptCorners[i].Y < result.Y)
            {
               result.Height += result.Y - arptCorners[i].Y;
               result.Y = arptCorners[i].Y;
            }
            else if (arptCorners[i].Y - result.Y > result.Height)
               result.Height = arptCorners[i].Y - result.Y;
         }
         return result;
      }

      public Bitmap GetCompositeFrame(ProjectDataset.SpriteStateRow drSprite, int Sequence)
      {
         ProjectDataset.SpriteFrameRow[] drFrames = ProjectData.GetSortedSpriteFrames(drSprite);
         RectangleF rcFrame = RectangleF.Empty;
         int idx = Sequence;
         do
         {
            ProjectDataset.FrameRow drFrame = ProjectData.GetFrame(drFrames[idx].SpriteStateRowParent.FramesetName, drFrames[idx].FrameValue);
            if (rcFrame.IsEmpty)
               rcFrame = GetFrameBounds(drFrame);
            else
               rcFrame = RectangleF.Union(rcFrame, GetFrameBounds(drFrame));
         } while ((idx<drFrames.Length-1) && (drFrames[idx++].Duration == 0));
         Rectangle rcBound = Rectangle.Round(rcFrame);

         BitmapData bmpData;
         int[] pixels;
         Bitmap bmpResult = new Bitmap(rcBound.Width, rcBound.Height, PixelFormat.Format32bppArgb);
         BitArray arbt = new BitArray(bmpResult.Width * bmpResult.Height);

         using (Bitmap bmpSingle = new Bitmap(bmpResult))
         {
            using (Graphics gfx = Graphics.FromImage(bmpResult))
            {
               using (Graphics gfxSingle = Graphics.FromImage(bmpSingle))
               {
                  gfxSingle.CompositingMode = CompositingMode.SourceCopy;
                  gfxSingle.PixelOffsetMode = PixelOffsetMode.Half;
                  gfx.PixelOffsetMode = PixelOffsetMode.Half;

                  idx = Sequence;
                  bool hasAlpha = false;
                  do
                  {
                     if (drFrames[idx].MaskAlphaLevel != 0)
                        hasAlpha = true;
                  } while ((idx<drFrames.Length-1) && (drFrames[idx++].Duration == 0));
                  idx = Sequence;
                  do
                  {
                     gfxSingle.Clear(Color.Transparent);
                     ProjectDataset.SpriteStateRow drSpriteState = drFrames[idx].SpriteStateRowParent;
                     ProjectDataset.FrameRow drFrame = ProjectData.GetFrame(drSpriteState.FramesetName, drFrames[idx].FrameValue);
                     using(Matrix mtx = new Matrix(drFrame.m11, drFrame.m12, drFrame.m21, drFrame.m22, drFrame.dx, drFrame.dy))
                     {
                        gfx.Transform = mtx;
                        gfxSingle.Transform = mtx;
                        Bitmap bmpFrameset = ProjectData.GetGraphicSheetImage(drFrame.GraphicSheet, false);
                        ProjectDataset.GraphicSheetRow drGfx = ProjectData.GetGraphicSheet(drFrame.GraphicSheet);
                        int row = drFrame.CellIndex / drGfx.Columns;
                        int col = drFrame.CellIndex % drGfx.Columns;
                        Rectangle rcSource = new Rectangle(drGfx.CellWidth * col, drGfx.CellHeight * row, drGfx.CellWidth, drGfx.CellHeight);
                        gfx.TranslateTransform(-rcBound.X, -rcBound.Y, MatrixOrder.Append);
                        gfxSingle.TranslateTransform(-rcBound.X, -rcBound.Y, MatrixOrder.Append);
                        gfx.DrawImage(bmpFrameset, 0, 0, rcSource, GraphicsUnit.Pixel);
                        gfxSingle.DrawImage(bmpFrameset, 0, 0, rcSource, GraphicsUnit.Pixel);
                        bmpData = bmpSingle.LockBits(new Rectangle(Point.Empty, rcBound.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                        pixels = new int[bmpSingle.Height * Math.Abs(bmpData.Stride) / 4];
                        System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixels, 0, bmpSingle.Height * Math.Abs(bmpData.Stride) / 4);
                        bmpSingle.UnlockBits(bmpData);
                        for (int rowIdx = 0; rowIdx < bmpSingle.Height; rowIdx++)
                        {
                           for (int pixIdx = 0; pixIdx < bmpSingle.Width; pixIdx++)
                           {
                              if (hasAlpha)
                              {
                                 arbt[rowIdx * bmpSingle.Width + pixIdx] |=
                                    (Color.FromArgb(pixels[rowIdx * bmpData.Stride / 4 + pixIdx]).A > drFrames[idx].MaskAlphaLevel);
                              }
                              else
                              {
                                 arbt[rowIdx * bmpSingle.Width + pixIdx] |=
                                    (rowIdx >= -rcBound.Y) && (rowIdx < -rcBound.Y + drSpriteState.SolidHeight) &&
                                    (pixIdx >= -rcBound.X) && (pixIdx < -rcBound.X + drSpriteState.SolidWidth);
                              }
                           }
                        }
                     }
                  } while ((idx<drFrames.Length-1) && (drFrames[idx++].Duration == 0));
               }
            }
         }

         bmpData = bmpResult.LockBits(new Rectangle(Point.Empty, rcBound.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
         pixels = new int[bmpResult.Width * Math.Abs(bmpData.Stride)];
         System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixels, 0, bmpResult.Height * Math.Abs(bmpData.Stride) / 4);
         for (int rowIdx = 0; rowIdx < bmpResult.Height; rowIdx++)
         {
            for (int pixIdx = 0; pixIdx < bmpResult.Width; pixIdx++)
            {
               switch(cboMode.SelectedIndex)
               {
                  case 0:
                     if (arbt[rowIdx * bmpResult.Width + pixIdx])
                        pixels[rowIdx * bmpData.Stride / 4 + pixIdx] = Color.Black.ToArgb();
                     break;
                  case 1:
                     if (arbt[rowIdx * bmpResult.Width + pixIdx])
                        pixels[rowIdx * bmpData.Stride / 4 + pixIdx] = Color.White.ToArgb();
                     break;
                  case 2:
                     if (!arbt[rowIdx * bmpResult.Width + pixIdx])
                        pixels[rowIdx * bmpData.Stride / 4 + pixIdx] = Color.Black.ToArgb();
                     break;
                  case 3:
                     if (!arbt[rowIdx * bmpResult.Width + pixIdx])
                        pixels[rowIdx * bmpData.Stride / 4 + pixIdx] = Color.White.ToArgb();
                     break;
                  case 4:
                     if (arbt[rowIdx * bmpResult.Width + pixIdx])
                        pixels[rowIdx * bmpData.Stride / 4 + pixIdx] = Color.Black.ToArgb();
                     else
                        pixels[rowIdx * bmpData.Stride / 4 + pixIdx] = Color.White.ToArgb();
                     break;
                  default:
                     if (arbt[rowIdx * bmpResult.Width + pixIdx])
                        pixels[rowIdx * bmpData.Stride / 4 + pixIdx] = Color.White.ToArgb();
                     else
                        pixels[rowIdx * bmpData.Stride / 4 + pixIdx] = Color.Black.ToArgb();
                     break;
               }
            }
         }
         System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bmpData.Scan0, bmpResult.Height * Math.Abs(bmpData.Stride) / 4);
         bmpResult.UnlockBits(bmpData);
         return bmpResult;
      }
      
      #region Event Handlers
      private void pnlPreview_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         using (Brush brBackground = new HatchBrush(HatchStyle.DiagonalCross, Color.Black, Color.White))
         {
            e.Graphics.FillRectangle(brBackground, pnlPreview.ClientRectangle);
         }
         e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
         e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
         e.Graphics.DrawImage(m_bmpTarget, pnlPreview.ClientRectangle);
      }

      private void cboMode_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (m_bmpTarget != null)
            m_bmpTarget.Dispose();
         m_bmpTarget = GetCompositeFrame(m_SpriteState, m_SelectedSequences[0]);
         pnlPreview.Invalidate();
      }

      private void trbAlphaLevel_Scroll(object sender, System.EventArgs e)
      {
         if (cboMode.SelectedIndex >= 0)
         {
            if (m_bmpTarget != null)
               m_bmpTarget.Dispose();
            for (int i=0; i<m_SelectedSequences.Length; i++)
               ProjectData.GetSortedSpriteFrames(m_SpriteState)[m_SelectedSequences[i]].MaskAlphaLevel = (byte)trbAlphaLevel.Value;
            m_bmpTarget = GetCompositeFrame(m_SpriteState, m_SelectedSequences[0]);
            pnlPreview.Invalidate();
         }
      }

      private void pnlPreview_Resize(object sender, System.EventArgs e)
      {
         pnlPreview.Invalidate();
      }
      #endregion
   }
}
