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
	/// Summary description for ImportGraphics.
	/// </summary>
	public class frmImportGraphics : System.Windows.Forms.Form
	{
      private System.Drawing.Bitmap ImportedImage = null;
      private Point m_ptDragStart = Point.Empty;
      private Size m_szDragSize = Size.Empty;
      private bool m_bIsDragging = false;
      private int m_cellWidth;
      private int m_cellHeight;
      private static string m_lastFileName = null;

      private System.Windows.Forms.PictureBox picImport;
      private System.Windows.Forms.StatusBar status;
      private System.Windows.Forms.Panel pnlScroll;
      private System.Windows.Forms.StatusBarPanel pnlTopLeft;
      private System.Windows.Forms.StatusBarPanel pnlWidthHeight;
      private System.Windows.Forms.StatusBarPanel pnlMain;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

      public static Bitmap ImportGraphic(IWin32Window parent, int cellWidth, int cellHeight)
      {
         Retry:
         System.Windows.Forms.OpenFileDialog dlgImportFrom = new System.Windows.Forms.OpenFileDialog();
         dlgImportFrom.Filter = "All graphics|*.bmp;*.png;*.jpg;*.gif|Bitmaps (*.bmp)|*.bmp|" +
            "Portable Network Graphics (*.png)|*.png|Jpeg (*.jpg)|*.jpg|Gif (*.gif)|*.gif";
         dlgImportFrom.Title = "Specify Source Image";

         if (m_lastFileName != null)
            dlgImportFrom.FileName = m_lastFileName;

         if (DialogResult.OK != dlgImportFrom.ShowDialog(parent))
         {
            dlgImportFrom.Dispose();
            return null;
         }

         m_lastFileName = dlgImportFrom.FileName;

         Bitmap sourceImage = (Bitmap)System.Drawing.Bitmap.FromFile(dlgImportFrom.FileName);
         try
         {
            dlgImportFrom.Dispose();

            if ((sourceImage.Width < cellWidth) || (sourceImage.Height < cellHeight))
            {
               if (DialogResult.Retry == MessageBox.Show(parent, "Image is too small to import -- it would not fill a cell", "Import Graphics", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
                  goto Retry;
               return null;
            }

            frmImportGraphics frm = new frmImportGraphics(cellWidth, cellHeight, sourceImage);
            if (frm.ShowDialog(parent) == DialogResult.OK)
            {
               Bitmap result = frm.ImportedImage;
               frm.Close();
               return frm.ImportedImage;
            }
            else
            {
               frm.Close();
               return null;
            }
         }
         finally
         {
            sourceImage.Dispose();
         }
      }

      public static Bitmap ImportGraphic(IWin32Window parent, int cellWidth, int cellHeight, Bitmap sourceImage)
      {
         if ((sourceImage.Width < cellWidth) || (sourceImage.Height < cellHeight))
         {
            MessageBox.Show(parent, "Image is too small to import -- it would not fill a cell", "Import Graphics", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return null;
         }

         frmImportGraphics frm = new frmImportGraphics(cellWidth, cellHeight, sourceImage);
         if (frm.ShowDialog(parent) == DialogResult.OK)
         {
            Bitmap result = frm.ImportedImage;
            frm.Close();
            return frm.ImportedImage;
         }
         else
         {
            frm.Close();
            return null;
         }
      }

      private frmImportGraphics(int cellWidth, int cellHeight, Bitmap sourceImage)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         picImport.Cursor = SGDK2IDE.LoadCursor("Draw.cur");
         m_cellWidth = cellWidth;
         m_cellHeight = cellHeight;
         picImport.Image = sourceImage;
         picImport.ClientSize = picImport.Image.Size;
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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmImportGraphics));
         this.picImport = new System.Windows.Forms.PictureBox();
         this.status = new System.Windows.Forms.StatusBar();
         this.pnlMain = new System.Windows.Forms.StatusBarPanel();
         this.pnlTopLeft = new System.Windows.Forms.StatusBarPanel();
         this.pnlWidthHeight = new System.Windows.Forms.StatusBarPanel();
         this.pnlScroll = new System.Windows.Forms.Panel();
         ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.pnlTopLeft)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.pnlWidthHeight)).BeginInit();
         this.pnlScroll.SuspendLayout();
         this.SuspendLayout();
         // 
         // picImport
         // 
         this.picImport.Location = new System.Drawing.Point(0, 0);
         this.picImport.Name = "picImport";
         this.picImport.Size = new System.Drawing.Size(376, 232);
         this.picImport.TabIndex = 0;
         this.picImport.TabStop = false;
         this.picImport.Paint += new System.Windows.Forms.PaintEventHandler(this.picImport_Paint);
         this.picImport.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picImport_MouseUp);
         this.picImport.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picImport_MouseMove);
         this.picImport.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picImport_MouseDown);
         // 
         // status
         // 
         this.status.Location = new System.Drawing.Point(0, 321);
         this.status.Name = "status";
         this.status.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
                                                                                  this.pnlMain,
                                                                                  this.pnlTopLeft,
                                                                                  this.pnlWidthHeight});
         this.status.ShowPanels = true;
         this.status.Size = new System.Drawing.Size(480, 20);
         this.status.TabIndex = 2;
         // 
         // pnlMain
         // 
         this.pnlMain.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
         this.pnlMain.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
         this.pnlMain.Width = 362;
         // 
         // pnlTopLeft
         // 
         this.pnlTopLeft.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
         this.pnlTopLeft.Icon = ((System.Drawing.Icon)(resources.GetObject("pnlTopLeft.Icon")));
         this.pnlTopLeft.Text = "0,0";
         this.pnlTopLeft.Width = 51;
         // 
         // pnlWidthHeight
         // 
         this.pnlWidthHeight.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
         this.pnlWidthHeight.Icon = ((System.Drawing.Icon)(resources.GetObject("pnlWidthHeight.Icon")));
         this.pnlWidthHeight.Text = "0,0";
         this.pnlWidthHeight.Width = 51;
         // 
         // pnlScroll
         // 
         this.pnlScroll.AutoScroll = true;
         this.pnlScroll.Controls.Add(this.picImport);
         this.pnlScroll.Dock = System.Windows.Forms.DockStyle.Fill;
         this.pnlScroll.Location = new System.Drawing.Point(0, 0);
         this.pnlScroll.Name = "pnlScroll";
         this.pnlScroll.Size = new System.Drawing.Size(480, 321);
         this.pnlScroll.TabIndex = 3;
         // 
         // frmImportGraphics
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(480, 341);
         this.Controls.Add(this.pnlScroll);
         this.Controls.Add(this.status);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.KeyPreview = true;
         this.Name = "frmImportGraphics";
         this.Text = "Import Graphics";
         ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.pnlTopLeft)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.pnlWidthHeight)).EndInit();
         this.pnlScroll.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion

      #region Overrides
      protected override void OnKeyDown(KeyEventArgs e)
      {
         base.OnKeyDown (e);
         if (e.KeyCode == Keys.Escape)
         {
            m_bIsDragging = false;
         }
      }

      protected override void OnClosing(CancelEventArgs e)
      {
         base.OnClosing (e);
         if (!e.Cancel)
         {
            if (this.DialogResult != DialogResult.OK)
               this.DialogResult = DialogResult.Cancel;
            Bitmap bmpSource = (Bitmap)(picImport.Image);
            picImport.Image = null;
            bmpSource.Dispose();

            Cursor csr = picImport.Cursor;
            picImport.Cursor = Cursors.Default;
            csr.Dispose();
         }
      }

      #endregion

      #region Event Handlers
      private void picImport_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         int bmpWidth = ((Bitmap)picImport.Image).Width;
         int bmpHeight = ((Bitmap)picImport.Image).Height;
         if (!m_bIsDragging)
         {
            pnlWidthHeight.Text = "0, 0";
            pnlMain.Text = "Click and drag from the top left corner of the import area";
            int maxLeft = bmpWidth - m_cellWidth;
            int maxTop = bmpHeight - m_cellHeight;
            m_ptDragStart = new Point(e.X, e.Y);
            if (m_ptDragStart.X >=  maxLeft)
               m_ptDragStart.X = maxLeft;
            if (m_ptDragStart.Y > maxTop)
               m_ptDragStart.Y = maxTop;
            pnlTopLeft.Text = m_ptDragStart.X + ", " + m_ptDragStart.Y.ToString();
            picImport.Invalidate();
         }
         else
         {
            int width = e.X - m_ptDragStart.X;
            int height = e.Y - m_ptDragStart.Y;
            if (width <= 0)
               width = 1;
            if (height <= 0)
               height = 1;
            width += m_cellWidth;
            height += m_cellHeight;
            int columns = (int)(width / m_cellWidth);
            int rows = (int)(height / m_cellHeight); 
            
            if ((m_ptDragStart.X + columns * m_cellWidth) >= bmpWidth)
               columns = (int)((bmpWidth - m_ptDragStart.X) / m_cellWidth);
            if ((m_ptDragStart.Y + rows * m_cellHeight) >= bmpHeight)
               rows = (int)((bmpHeight - m_ptDragStart.Y) / m_cellHeight);

            width = columns * m_cellWidth;
            height = rows * m_cellHeight;

            pnlWidthHeight.Text = width.ToString() + ", "  + height.ToString();
            pnlMain.Text = "Selecting " + columns.ToString() + "x" + rows.ToString() + " cells (Esc to cancel)";
            m_szDragSize = new Size(width, height);
            picImport.Invalidate();
         }
      }

      private void picImport_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         if ((m_bIsDragging) && (m_szDragSize.Width > 0) && (m_szDragSize.Height > 0))
         {
            Bitmap bmpClip = new Bitmap(m_szDragSize.Width, m_szDragSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int y = m_ptDragStart.Y; y < m_ptDragStart.Y + m_szDragSize.Height; y++)
            {
               for (int x = m_ptDragStart.X; x < m_ptDragStart.X + m_szDragSize.Width; x++)
               {
                  bmpClip.SetPixel(x - m_ptDragStart.X, y - m_ptDragStart.Y,
                     ((Bitmap)picImport.Image).GetPixel(x, y));
               }
            }
            ImportedImage = bmpClip;
            this.DialogResult = DialogResult.OK;
            this.Close();
         }
      }

      private void picImport_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         if (picImport.Image == null)
            return;

         if (m_bIsDragging)
         {
            Pen p = new Pen(Color.White);
            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            p.DashOffset = 1;
            e.Graphics.DrawRectangle(p, new Rectangle(m_ptDragStart, m_szDragSize));
            p.Color = Color.Black;
            p.DashOffset = 0;
            e.Graphics.DrawRectangle(p, new Rectangle(m_ptDragStart, m_szDragSize));
            p.Dispose();
         }
         else
         {
            using (Pen gridPen = new Pen(Color.White))
            {
               gridPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
               gridPen.DashPattern = new float[] {1,3};
               for (int y=m_ptDragStart.Y; y<picImport.ClientRectangle.Height; y += m_cellHeight)
               {
                  gridPen.DashOffset=0;
                  gridPen.Color = Color.White;
                  e.Graphics.DrawLine(gridPen, m_ptDragStart.X, y, picImport.ClientSize.Width-1, y);
                  gridPen.DashOffset=2;
                  gridPen.Color = Color.Black;
                  e.Graphics.DrawLine(gridPen, m_ptDragStart.X, y, picImport.ClientSize.Width-1, y);
               }
               for (int x=m_ptDragStart.X; x<picImport.ClientRectangle.Width; x += m_cellWidth)
               {
                  gridPen.DashOffset=0;
                  gridPen.Color = Color.White;
                  e.Graphics.DrawLine(gridPen, x, m_ptDragStart.Y, x, picImport.ClientSize.Height-1);
                  gridPen.DashOffset=2;
                  gridPen.Color = Color.Black;
                  e.Graphics.DrawLine(gridPen, x, m_ptDragStart.Y, x, picImport.ClientSize.Height-1);
               }
            }
         }
      }

      private void picImport_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         m_bIsDragging = true;
      }
      #endregion
   }
}
