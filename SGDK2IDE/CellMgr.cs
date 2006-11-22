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

namespace SGDK2
{
	/// <summary>
	/// Summary description for CellMgr.
	/// </summary>
	public class frmCellMgr : System.Windows.Forms.Form
	{
      #region Non-Control Members
      private ProjectDataset.GraphicSheetRow m_DataSource;
      private int m_nStartSelectCell = 0;
      private int m_nEndSelectCell = 0;
      private ProjectDataset.GraphicSheetRowChangeEventHandler m_ChangeEvent = null;
      #endregion

      #region Form Designer Members
      private System.Windows.Forms.Panel pnlSheet;
      private System.Windows.Forms.Button btnLoadCell;
      private System.Windows.Forms.Button btnStoreCell;
      private System.Windows.Forms.PictureBox picSheet;
      #endregion

      #region Initialization and Clean-up
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private frmCellMgr()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
      }

      public frmCellMgr(frmGraphicsEditor frmParent, ProjectDataset.GraphicSheetRow drDataSource) : this()
      {
         this.TopLevel = false;
         this.Parent = frmParent;
         Backdrop = ParentEditor.Backdrop;
         m_DataSource = drDataSource;
         picSheet.Image  = ProjectData.GetGraphicSheetImage(drDataSource.Name, true);
         picSheet.Size = picSheet.Image.Size;
         m_ChangeEvent = new SGDK2.ProjectDataset.GraphicSheetRowChangeEventHandler(DataSource_GraphicSheetRowChanged);
         ((ProjectDataset.GraphicSheetDataTable)drDataSource.Table).GraphicSheetRowChanged += m_ChangeEvent;
      }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
            if (m_ChangeEvent != null)
            {
               ((ProjectDataset.GraphicSheetDataTable)m_DataSource.Table).GraphicSheetRowChanged -= m_ChangeEvent;
               m_ChangeEvent = null;
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
         this.pnlSheet = new System.Windows.Forms.Panel();
         this.picSheet = new System.Windows.Forms.PictureBox();
         this.btnLoadCell = new System.Windows.Forms.Button();
         this.btnStoreCell = new System.Windows.Forms.Button();
         this.pnlSheet.SuspendLayout();
         this.SuspendLayout();
         // 
         // pnlSheet
         // 
         this.pnlSheet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.pnlSheet.AutoScroll = true;
         this.pnlSheet.Controls.Add(this.picSheet);
         this.pnlSheet.Location = new System.Drawing.Point(0, 24);
         this.pnlSheet.Name = "pnlSheet";
         this.pnlSheet.Size = new System.Drawing.Size(272, 88);
         this.pnlSheet.TabIndex = 0;
         // 
         // picSheet
         // 
         this.picSheet.Location = new System.Drawing.Point(0, 0);
         this.picSheet.Name = "picSheet";
         this.picSheet.Size = new System.Drawing.Size(264, 80);
         this.picSheet.TabIndex = 0;
         this.picSheet.TabStop = false;
         this.picSheet.Paint += new System.Windows.Forms.PaintEventHandler(this.picSheet_Paint);
         this.picSheet.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picSheet_MouseMove);
         this.picSheet.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picSheet_MouseDown);
         // 
         // btnLoadCell
         // 
         this.btnLoadCell.Location = new System.Drawing.Point(0, 0);
         this.btnLoadCell.Name = "btnLoadCell";
         this.btnLoadCell.Size = new System.Drawing.Size(136, 24);
         this.btnLoadCell.TabIndex = 1;
         this.btnLoadCell.Text = "Load Selected Cell(s)";
         this.btnLoadCell.Click += new System.EventHandler(this.btnLoadCell_Click);
         // 
         // btnStoreCell
         // 
         this.btnStoreCell.Location = new System.Drawing.Point(136, 0);
         this.btnStoreCell.Name = "btnStoreCell";
         this.btnStoreCell.Size = new System.Drawing.Size(136, 24);
         this.btnStoreCell.TabIndex = 2;
         this.btnStoreCell.Text = "Store to Selected Cell(s)";
         this.btnStoreCell.Click += new System.EventHandler(this.btnStoreCell_Click);
         // 
         // frmCellMgr
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(272, 112);
         this.ControlBox = false;
         this.Controls.Add(this.btnStoreCell);
         this.Controls.Add(this.btnLoadCell);
         this.Controls.Add(this.pnlSheet);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "frmCellMgr";
         this.ShowInTaskbar = false;
         this.Text = "Cell Manager";
         this.pnlSheet.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion

      #region Private Properties
      frmGraphicsEditor ParentEditor
      {
         get
         {
            return (frmGraphicsEditor)Parent;
         }
      }

      Rectangle SelectedRectangle
      {
         get
         {
            return Rectangle.FromLTRB(
               m_DataSource.CellWidth * (m_nStartSelectCell % m_DataSource.Columns),
               m_DataSource.CellHeight * (m_nStartSelectCell / m_DataSource.Columns),
               m_DataSource.CellWidth * ((m_nEndSelectCell % m_DataSource.Columns) + 1),
               m_DataSource.CellHeight * ((m_nEndSelectCell / m_DataSource.Columns) + 1));
         }
      }

      private void StoreImageToProject()
      {
         System.IO.MemoryStream stm = new System.IO.MemoryStream();
         ((Bitmap)picSheet.Image).Save(stm, System.Drawing.Imaging.ImageFormat.Png);
         stm.Close();
         m_DataSource.Image = stm.GetBuffer();
      }
      #endregion

      #region Public Properties
      public Brush Backdrop
      {
         set
         {
            if (picSheet.BackgroundImage != null)
               picSheet.BackgroundImage.Dispose();
            picSheet.BackgroundImage = new Bitmap(8, 8, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(picSheet.BackgroundImage);
            g.FillRectangle(value, 0, 0, 8, 8);
            g.Dispose();
         }
      }
      #endregion

      #region Public Methods
      public void InvalidateSheet()
      {
         picSheet.Invalidate();
      }
      public void ReplaceSheet(Bitmap newImage)
      {
         if ((newImage.Width != picSheet.Image.Width) ||
            (newImage.Height != picSheet.Image.Height))
         {
            newImage.Dispose();
            throw new ApplicationException("Replacement image must be " + picSheet.Image.Width.ToString() +
               "x" + picSheet.Image.Height.ToString() + " pixels");
         }
         if (newImage.PixelFormat != picSheet.Image.PixelFormat)
         {
            Bitmap clonedImage = new Bitmap(picSheet.Image.Width, picSheet.Image.Height, picSheet.Image.PixelFormat);
            using (Graphics gfx = Graphics.FromImage(clonedImage))
               gfx.DrawImageUnscaled(newImage, 0, 0);
            newImage.Dispose();
            picSheet.Image.Dispose();
            picSheet.Image = clonedImage;
         }
         else
         {
            picSheet.Image.Dispose();
            picSheet.Image = newImage;
         }
         StoreImageToProject();
      }
      #endregion

      #region Event Handlers
      private void picSheet_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         if (ParentEditor.HighlightBrush != null)
            e.Graphics.FillRectangle(ParentEditor.HighlightBrush, SelectedRectangle);
      }

      private void btnStoreCell_Click(object sender, System.EventArgs e)
      {
         Rectangle selRect = SelectedRectangle;
         if ((ParentEditor.m_imgCurrentGraphic.Width != selRect.Width) ||
            (ParentEditor.m_imgCurrentGraphic.Height != selRect.Height))
         {
            MessageBox.Show(this, "The selected target area is a different size than the image currently loaded into the editor.", "Store Image", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         ParentEditor.DropFloatingSelection();
         for (int y = 0; y < selRect.Height; y++)
         {
            for (int x = 0; x < selRect.Width; x++)
            {
               ((Bitmap)picSheet.Image).SetPixel(x + selRect.X, y + selRect.Y,
                  ParentEditor.m_imgCurrentGraphic.GetPixel(x,y));
            }
         }
         picSheet.Invalidate();
         StoreImageToProject();
      }

      private void picSheet_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         int nCol = e.X / m_DataSource.CellWidth;
         int nRow = e.Y / m_DataSource.CellHeight;
         if ((nCol < m_DataSource.Columns) && (nRow < m_DataSource.Rows) &&
            (nCol >= 0) && (nRow >= 0))
            m_nStartSelectCell = m_nEndSelectCell = nRow * m_DataSource.Columns + nCol;
         picSheet.Invalidate();
      }

      private void picSheet_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Left)
         {
            int nStartCol = m_nStartSelectCell % m_DataSource.Columns;
            int nCol = e.X / m_DataSource.CellWidth;
            int nRow = e.Y / m_DataSource.CellHeight;
            int nClickedCell = nRow * m_DataSource.Columns + nCol;
            if ((nCol < m_DataSource.Columns) && (nRow < m_DataSource.Rows) &&
               (nCol >= nStartCol) && (nClickedCell >= m_nStartSelectCell))
               m_nEndSelectCell = nClickedCell;
            picSheet.Invalidate();
         }
      }

      private void btnLoadCell_Click(object sender, System.EventArgs e)
      {
         int nCols = (m_nEndSelectCell % m_DataSource.Columns) - (m_nStartSelectCell % m_DataSource.Columns) + 1;
         int nRows = (m_nEndSelectCell / m_DataSource.Columns) - (m_nStartSelectCell / m_DataSource.Columns) + 1;
         int nX = (m_nStartSelectCell % m_DataSource.Columns) * m_DataSource.CellWidth;
         int nY = (m_nStartSelectCell / m_DataSource.Columns) * m_DataSource.CellHeight;
         Bitmap bmpLoad = new Bitmap(picSheet.Image, m_DataSource.CellWidth * nCols, m_DataSource.CellHeight * nRows);
         for (int y = 0; y < nRows * m_DataSource.CellHeight; y++)
            for (int x = 0; x < nCols * m_DataSource.CellWidth; x++)
               bmpLoad.SetPixel(x,y,((Bitmap)picSheet.Image).GetPixel(x+nX, y+nY));
         ParentEditor.LoadImage(bmpLoad);
      }

      private void DataSource_GraphicSheetRowChanged(object sender, SGDK2.ProjectDataset.GraphicSheetRowChangeEvent e)
      {
         if ((e.Action != System.Data.DataRowAction.Change) || (m_DataSource != e.Row))
            return;

         if (m_nStartSelectCell >= e.Row.Columns * e.Row.Rows)
         {
            m_nStartSelectCell = 0;
            m_nEndSelectCell = 0;
         }
         else if (m_nEndSelectCell > e.Row.Columns * e.Row.Rows)
         {
            m_nEndSelectCell = m_nStartSelectCell;
         }

         picSheet.Image = ProjectData.GetGraphicSheetImage(e.Row.Name, true);
         picSheet.Size = picSheet.Image.Size;
      }
      #endregion
   }
}
