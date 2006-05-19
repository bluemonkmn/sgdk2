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
using System.Data;

namespace SGDK2
{
	/// <summary>
	/// Summary description for GfxSheet.
	/// </summary>
	public class frmGfxSheet : System.Windows.Forms.Form
	{
      #region Embedded classes
      public class GfxSheetProperties
      {
         private GfxSheetProperties()
         {
         }
         public GfxSheetProperties(ProjectDataset.GraphicSheetRow drEdit)
         {
            m_drGfx = drEdit;
         }
         public ProjectDataset.GraphicSheetRow m_drGfx;
         private Color m_BackgroundColor;
         
         [DescriptionAttribute("Name by which this object will be referred to in the project"),
         Category("Design"),
         ParenthesizePropertyName(true)]
         public string Name
         {
            get
            {
               return m_drGfx.Name;
            }
            set
            {
               m_drGfx.Name = value;
            }
         }

         [DescriptionAttribute("Number of graphic cells on each row of the sheet"),
         Category("Layout")]
         public short Columns
         {
            get
            {
               return m_drGfx.Columns;
            }
            set
            {
               m_drGfx.Columns = value;
            }
         }
         [DescriptionAttribute("Number of rows of cells in the sheet"),
         Category("Layout")]
         public short Rows
         {
            get
            {
               return m_drGfx.Rows;
            }
            set
            {
               m_drGfx.Rows = value;
            }
         }
         [DescriptionAttribute("Width in pixels of each graphic in the sheet"),
         Category("Layout")]
         public short CellWidth
         {
            get
            {
               return m_drGfx.CellWidth;
            }
            set
            {
               m_drGfx.CellWidth = value;
            }
         }
         [DescriptionAttribute("Height in pixels of each graphic in the sheet"),
         Category("Layout")]
         public short CellHeight
         {
            get
            {
               return m_drGfx.CellHeight;
            }
            set
            {
               m_drGfx.CellHeight = value;
            }
         }

         [DescriptionAttribute("Color to which the background of the graphic sheet will be initialized"),
         Category("Appearance")]
         public Color BackgroundColor
         {
            get
            {
               return m_BackgroundColor;
            }
            set
            {
               m_BackgroundColor = value;
            }
         }
      }
      #endregion

      #region Non-control Members
      public GfxSheetProperties DataObject;
      private ProjectDataset.GraphicSheetRowChangeEventHandler m_RowChangeEvent;
      #endregion

      #region Form Designer Members

      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.PropertyGrid pgrGfxSheet;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
      #endregion

      #region Initialization and Clean-up
		public frmGfxSheet()
		{
			InitializeComponent();
         int iNew;
         for (iNew = 1; ProjectData.GetGraphicSheet("New Sheet " + iNew.ToString()) != null; iNew++)
            ;
         ProjectDataset.GraphicSheetRow EditRow = ProjectData.NewGraphicSheet();
         EditRow.Name = "New Sheet " + iNew.ToString();
         EditRow.Columns = (short)ProjectData.GraphicSheet.ColumnsColumn.DefaultValue;
         EditRow.Rows = (short)ProjectData.GraphicSheet.RowsColumn.DefaultValue;
         EditRow.CellWidth = (short)ProjectData.GraphicSheet.CellWidthColumn.DefaultValue;
         EditRow.CellHeight = (short)ProjectData.GraphicSheet.CellHeightColumn.DefaultValue;
         EditRow.BeginEdit();
         m_RowChangeEvent = new SGDK2.ProjectDataset.GraphicSheetRowChangeEventHandler(ProjectData_GraphicSheetRowDeleted);
         ProjectData.GraphicSheetRowDeleted += m_RowChangeEvent;
         pgrGfxSheet.SelectedObject = DataObject = new GfxSheetProperties(EditRow);
      }

      public frmGfxSheet(ProjectDataset.GraphicSheetRow EditRow)
      {
         InitializeComponent();
         btnOK.Text = "Update";
         EditRow.BeginEdit();
         m_RowChangeEvent = new SGDK2.ProjectDataset.GraphicSheetRowChangeEventHandler(ProjectData_GraphicSheetRowDeleted);
         ProjectData.GraphicSheetRowDeleted += m_RowChangeEvent;
         pgrGfxSheet.SelectedObject = DataObject = new GfxSheetProperties(EditRow);
      }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
            ProjectData.GraphicSheetRowDeleted -= m_RowChangeEvent;
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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmGfxSheet));
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.pgrGfxSheet = new System.Windows.Forms.PropertyGrid();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnOK.Location = new System.Drawing.Point(232, 8);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(64, 24);
         this.btnOK.TabIndex = 14;
         this.btnOK.Text = "Add";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(232, 40);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(64, 24);
         this.btnCancel.TabIndex = 15;
         this.btnCancel.Text = "Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // pgrGfxSheet
         // 
         this.pgrGfxSheet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.pgrGfxSheet.CommandsVisibleIfAvailable = true;
         this.pgrGfxSheet.LargeButtons = false;
         this.pgrGfxSheet.LineColor = System.Drawing.SystemColors.ScrollBar;
         this.pgrGfxSheet.Location = new System.Drawing.Point(0, 0);
         this.pgrGfxSheet.Name = "pgrGfxSheet";
         this.pgrGfxSheet.Size = new System.Drawing.Size(224, 256);
         this.pgrGfxSheet.TabIndex = 16;
         this.pgrGfxSheet.Text = "PropertyGrid";
         this.pgrGfxSheet.ViewBackColor = System.Drawing.SystemColors.Window;
         this.pgrGfxSheet.ViewForeColor = System.Drawing.SystemColors.WindowText;
         // 
         // frmGfxSheet
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(304, 261);
         this.Controls.Add(this.pgrGfxSheet);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "frmGfxSheet";
         this.ShowInTaskbar = false;
         this.Text = "Graphics Sheet Properties";
         this.ResumeLayout(false);

      }
		#endregion

      #region Private Members
      private Boolean SaveRecord()
      {
         try
         {
            string sValid = ProjectData.ValidateName(DataObject.m_drGfx.Name);
            if (sValid != null)
            {
               MessageBox.Show(this, sValid, "Graphic Sheet Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }

            if ((DataObject.BackgroundColor != Color.Empty) &&
               (DataObject.m_drGfx.RowState != DataRowState.Detached))
            {
               if (DialogResult.OK != MessageBox.Show(this, "Setting the BackColor property will clear the entire sheet to the specified color, erasing all graphics.", "Update Graphics Sheet", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
                  return false;
            }
            DataObject.m_drGfx.EndEdit();
            if (DataObject.m_drGfx.RowState == DataRowState.Detached)
            {
               DataObject.m_drGfx.Image = GetImageForCurrentParams();
               try
               {
                  ProjectData.AddGraphicSheetRow(DataObject.m_drGfx);
                  btnOK.Text = "Update";
               }
               catch (ConstraintException)
               {
                  MessageBox.Show(this, "Unable to add the graphic sheet due to invalid data.  Please specify a unique name.", "Add Graphic Sheet", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return false;
               }
            }
            else if (!GetImageSize().Equals(new Size(
               DataObject.m_drGfx.Columns * DataObject.m_drGfx.CellWidth,
               DataObject.m_drGfx.Rows * DataObject.m_drGfx.CellHeight)) ||
               (DataObject.BackgroundColor != Color.Empty))
            {
               DataObject.m_drGfx.Image = GetImageForCurrentParams();
            }
            btnCancel.Text = "Close";
            ((frmMain)MdiParent).SelectByContext("GS" + DataObject.m_drGfx.Name);
            return true;
         }
         catch (ConstraintException)
         {
            MessageBox.Show(this, "Unable to modify the graphic sheet due to invalid data.  Please specify a unique name.", "Add Graphic Sheet", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
      }

      private Size GetImageSize()
      {
         System.IO.MemoryStream stmBmp = new System.IO.MemoryStream(
            DataObject.m_drGfx.Image, false);
         Bitmap bmpTmp = new Bitmap(stmBmp);
         try
         {
            stmBmp.Close();
            return bmpTmp.Size;
         }
         finally
         {
            bmpTmp.Dispose();
         }
      }

      private byte[] GetImageForCurrentParams()
      {
         Bitmap bmpTemp;
         if (DataObject.m_drGfx.IsImageNull() || (DataObject.BackgroundColor != Color.Empty))
         {
            bmpTemp = new Bitmap(
               DataObject.m_drGfx.Columns * DataObject.m_drGfx.CellWidth,
               DataObject.m_drGfx.Rows * DataObject.m_drGfx.CellHeight,
               System.Drawing.Imaging.PixelFormat.Format32bppArgb);
         }
         else
         {
            System.IO.MemoryStream stmOld = new System.IO.MemoryStream(
               DataObject.m_drGfx.Image, false);
            Bitmap bmpOld = new Bitmap(stmOld);
            stmOld.Close();
            bmpTemp = new Bitmap(
               DataObject.m_drGfx.Columns * DataObject.m_drGfx.CellWidth,
               DataObject.m_drGfx.Rows * DataObject.m_drGfx.CellHeight,
               System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            CopyImage(bmpTemp, bmpOld, 0, 0, 0, 0, bmpOld.Width, bmpOld.Height);
            bmpOld.Dispose();
         }
         if (DataObject.BackgroundColor != Color.Empty)
         {
            Graphics g = Graphics.FromImage(bmpTemp);
            g.Clear(DataObject.BackgroundColor);
            g.Dispose();
         }
         System.IO.MemoryStream ms = new System.IO.MemoryStream();
         bmpTemp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
         bmpTemp.Dispose();
         ms.Close();
         return ms.GetBuffer();
      }

      /// <summary>
      /// GDI+ has a bug copying bitmaps.  Change this method
      /// when it is fixed. (near-opaque pixels lose 1 RGB during DrawImage)
      /// </summary>
      /// <param name="Dest">Target image</param>
      /// <param name="Src">Source image</param>
      public void CopyImage(Bitmap Dest, Bitmap Src, int nSrcX, int nSrcY, int nDstX, int nDstY, int nWid, int nHgt)
      {
         // This is not intended for high performance applications.
         // Use lockbits if you need that.
         if (nWid > Src.Width - nSrcX)
            nWid = Src.Width - nSrcX;
         if (nWid > Dest.Width - nDstX)
            nWid = Dest.Width - nDstX;
         if (nHgt > Src.Height - nSrcY)
            nHgt = Src.Height - nSrcY;
         if (nHgt > Dest.Height - nDstY)
            nHgt = Dest.Height - nDstY;
         for (int nY = 0; nY < nHgt; nY++)
            for (int nX = 0; nX < nWid; nX++)
               Dest.SetPixel(nX + nDstX, nY + nDstY, Src.GetPixel(nX + nSrcX, nY + nSrcY));
      }
      #endregion

      #region Event Handlers
      private void btnOK_Click(object sender, System.EventArgs e)
      {
         if (SaveRecord())
            this.Close();
      }

      private void btnCancel_Click(object sender, System.EventArgs e)
      {
         DataObject.m_drGfx.CancelEdit();
         this.Close();
      }

      private void ProjectData_GraphicSheetRowDeleted(object sender, SGDK2.ProjectDataset.GraphicSheetRowChangeEvent e)
      {
         if (DataObject.m_drGfx == e.Row)
         {
            DataObject.m_drGfx.CancelEdit();
            this.Close();
         }
      }
      #endregion
   }
}
