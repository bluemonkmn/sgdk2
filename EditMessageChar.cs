using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SGDK2
{
   public partial class frmEditMessageChar : Form
   {
      private TileCache m_TileCache = null;
      public frmEditMessageChar(ProjectDataset.TilesetRow drTileset)
      {
         InitializeComponent();
         FrameList tileProvider = new FrameList();
         m_TileCache = new TileCache(drTileset);
         for (int i = 0; (i < m_TileCache.Count) && (i < 256); i++)
            if ((i != 10) && (i != 13))
               tileProvider.Add(new TileProvider(m_TileCache, i));
         graphics.Frameset = drTileset.FramesetRow;
         graphics.FramesToDisplay = tileProvider;
      }

      private void graphics_DoubleClick(object sender, EventArgs e)
      {
         if (graphics.CurrentCellIndex >= 0)
         {
            Clipboard.SetText(string.Format("\\u{0:x4}", ((TileProvider)graphics.FramesToDisplay[graphics.CurrentCellIndex]).TileIndex));
            DialogResult = DialogResult.OK;
            Close();
         }
      }

      private void graphics_CurrentCellChanged(object sender, EventArgs e)
      {
         txtUnicode.Text = string.Format("\\u{0:x4}", ((TileProvider)graphics.FramesToDisplay[graphics.CurrentCellIndex]).TileIndex);
      }

      public static string GetExtendedCharacter(ProjectDataset.TilesetRow drTileset, IWin32Window owner)
      {
         frmEditMessageChar frm = new frmEditMessageChar(drTileset);
         if (frm.ShowDialog(owner) == DialogResult.OK)
            return frm.txtUnicode.Text;
         return null;
      }
   }
}
