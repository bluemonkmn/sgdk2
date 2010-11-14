using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace SGDK2
{
   public partial class frmEditMessage : Form
   {
      public const string messageEditorItem = "<Message Editor...>";
      private ProjectDataset.TilesetRow m_tileset = null;

      public static string EditMessage(string input, IWin32Window owner)
      {
         frmEditMessage frm = new frmEditMessage();
         frm.txtMessage.Text = input;
         if (DialogResult.OK == frm.ShowDialog(owner))
            return frm.txtMessage.Text;
         else
            return null;
      }

      private static string ParseStringLiteral(string literal)
      {
         int state=0; /* 0=outside quotes
                         1=outside quotes after first char
                         2=in quotes
                         3=escaped in quote
                         4=parsing unicode character number (digit 1)
                         5=parsing unicode character number (digit 2)
                         6=parsing unicode character number (digit 3)
                         7=parsing unicode character number (digit 4)
                       */
         ushort unicodeVal = 0;
         System.Text.StringBuilder sb = new StringBuilder();

         for (int i = 0; i < literal.Length; i++)
         {
            switch (state)
            {
               case 0:
               case 1:
                  if (literal[i] == '"')
                     state = 2;
                  else
                  {
                     if (state == 0)
                     {
                        sb.Append("[Variable]");
                        state = 1;
                     }
                  }
                  break;
               case 2:
                  switch (literal[i])
                  {
                     case '"':
                        state = 0;
                        break;
                     case '\\':
                        state = 3;
                        break;
                     default:
                        sb.Append(literal[i]);
                        break;
                  }
                  break;
               case 3:
                  state = 2;
                  switch (literal[i])
                  {
                     case 'u':
                        state = 4;
                        unicodeVal = 0;
                        break;
                     case 'r':
                        sb.Append('\r');
                        break;
                     case 'n':
                        sb.Append('\n');
                        break;
                     case 't':
                        sb.Append('\t');
                        break;
                     default:
                        sb.Append('?');
                        break;
                  }
                  break;
               case 4:
               case 5:
               case 6:
               case 7:
                  ushort hex = HexDigitValue(literal, i);
                  if (hex < 16)
                  {
                     unicodeVal = (ushort)(unicodeVal * 16 + hex);
                     if (state == 7)
                     {
                        sb.Append(System.Text.Encoding.Unicode.GetChars(
                           System.BitConverter.GetBytes(unicodeVal)));
                        state = 2;
                     }
                     else
                        state++;
                  }
                  else
                     state = 2;
                  break;
            }
         }
         return sb.ToString();
      }

      private static byte HexDigitValue(string source, int index)
      {
         byte[] asciiCodes = System.Text.Encoding.ASCII.GetBytes("09afAF");
         byte[] outByte = new byte[2];
         System.Text.Encoding.ASCII.GetBytes(source, index, 1, outByte, 0);
         byte ascii = outByte[0];
         if ((ascii >= asciiCodes[0]) && (ascii <= asciiCodes[1]))
            return (byte)(ascii - asciiCodes[0]);
         if ((ascii >= asciiCodes[2]) && (ascii <= asciiCodes[3]))
            return (byte)(ascii - asciiCodes[2] + 10);
         if ((ascii >= asciiCodes[4]) && (ascii <= asciiCodes[5]))
            return (byte)(ascii - asciiCodes[4] + 10);
         return (byte)127;
      }

      public frmEditMessage()
      {
         InitializeComponent();
         SGDK2IDE.g_HelpProvider.SetHelpKeyword(this, @"html/cd49bb62-e269-483c-b7e3-527c99770b47.htm");
         SGDK2IDE.g_HelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
         foreach (DataRowView drv in ProjectData.Tileset.DefaultView)
         {
            ProjectDataset.TilesetRow drTileset = (ProjectDataset.TilesetRow)drv.Row;
            cboTileset.Items.Add(drTileset);
         }
      }

      private void DrawMessage(Graphics gfx, Rectangle bounds)
      {
         if (m_tileset == null)
            return;
         TileCache tileIndexer = new TileCache(m_tileset);
         ProjectDataset.FrameRow[] frames = ProjectData.GetSortedFrameRows(m_tileset.FramesetRow);

         byte[,] tiles = MessageToArray(ParseStringLiteral(txtMessage.Text));
         string sheetName = string.Empty;
         Bitmap bmpFrameset = null;
         ProjectDataset.GraphicSheetRow drGfx = null;
         Size messageSize = new Size(
            (tiles.GetUpperBound(0) + 1) * m_tileset.TileWidth,
            (tiles.GetUpperBound(1) + 1) * m_tileset.TileHeight);
         Point pos = Point.Empty;
         pos.X = (bounds.X + bounds.Width - messageSize.Width) / 2;
         pos.Y = (bounds.Y + bounds.Height - messageSize.Height) / 2;
         Rectangle rcMessage = new Rectangle(pos, messageSize);
         using (Brush bkg = new SolidBrush(Color.FromArgb(196, 0, 0, 255)))
         {
            Rectangle rcFrame = rcMessage;
            rcFrame.Inflate(6, 6);
            gfx.FillRectangle(bkg, rcFrame);
            gfx.DrawRectangle(Pens.White, rcFrame);
         }

         for (int y = 0; y <= tiles.GetUpperBound(1); y++)
         {
            for (int x = 0; x <= tiles.GetUpperBound(0); x++)
            {
               int[] subFrames = tileIndexer[tiles[x, y]];
               for (int subFrame = 0; subFrame < subFrames.Length; subFrame++)
               {
                  ProjectDataset.FrameRow drFrame = frames[subFrames[subFrame]];
                  if (string.Compare(sheetName, drFrame.GraphicSheet) != 0)
                  {
                     bmpFrameset = ProjectData.GetGraphicSheetImage(drFrame.GraphicSheet, false);
                     sheetName = drFrame.GraphicSheet;
                     drGfx = ProjectData.GetGraphicSheet(drFrame.GraphicSheet);
                  }
                  Point[] corners;
                  int xOff = rcMessage.X + x * m_tileset.TileWidth;
                  int yOff = rcMessage.Y + y * m_tileset.TileHeight;
                  using (Matrix mtx = new Matrix(drFrame.m11, drFrame.m12, drFrame.m21, drFrame.m22, drFrame.dx, drFrame.dy))
                  {
                     // Assumes that the image is a parallelogram
                     corners = new Point[] {
                                 new Point(0, 0),
                                 new Point(drGfx.CellWidth, 0),
                                 new Point(0, drGfx.CellHeight)};
                     mtx.TransformPoints(corners);
                     for (int i=0; i<corners.Length; i++)
                        corners[i].Offset(xOff, yOff);
                  }
                  int row = drFrame.CellIndex / drGfx.Columns;
                  int col = drFrame.CellIndex % drGfx.Columns;
                  Rectangle rcSource = new Rectangle(drGfx.CellWidth * col, drGfx.CellHeight * row, drGfx.CellWidth, drGfx.CellHeight);
                  if (drFrame.color != -1)
                  {
                     byte[] clr = BitConverter.GetBytes(drFrame.color);
                     System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix(
                        new float[][]
                              {
                                 new float[] {(float)clr[2]/255.0f, 0, 0, 0, 0},
                                 new float[] {0, (float)clr[1]/255.0f, 0, 0, 0},
                                 new float[] {0, 0, (float)clr[0]/255.0f, 0, 0},
                                 new float[] {0, 0, 0, (float)clr[3]/255.0f, 0},
                                 new float[] {0, 0, 0, 0, 1}
                              });
                     using (System.Drawing.Imaging.ImageAttributes attr = new System.Drawing.Imaging.ImageAttributes())
                     {
                        attr.SetColorMatrices(cm, cm);
                        gfx.DrawImage(bmpFrameset, corners, rcSource,
                           System.Drawing.GraphicsUnit.Pixel, attr);
                     }
                  }
                  else
                     gfx.DrawImage(bmpFrameset, corners, rcSource, GraphicsUnit.Pixel);
               }
            }
         }
      }

      private void DrawGuides(Graphics gfx, Rectangle bounds)
      {
         using (StringFormat sf = new StringFormat(StringFormatFlags.DirectionVertical))
         {
            using (Pen guidePen = new Pen(SystemColors.ControlText))
            {
               guidePen.DashStyle = DashStyle.Dot;

               foreach (int x in new int[] { 320, 400, 640, 800 })
               {
                  for (int lr = -1; lr <= 1; lr += 2)
                  {
                     int linePos = (bounds.X + bounds.Width + x * lr) / 2;
                     gfx.DrawLine(guidePen, linePos, bounds.Top, linePos, bounds.Bottom);
                     gfx.DrawString(x.ToString(), Font, SystemBrushes.ControlText, linePos, bounds.Top, sf);
                  }
               }
            }
         }
      }

      private byte[,] MessageToArray(string message)
      {
         int x=0, width=0, row=1;

         byte[] outByte = new byte[2];

         for (int i = 0; i < message.Length; i++)
         {
            switch (message[i])
            {
               case '\n':
                  row++;
                  x=0;
                  break;
               case '\r':
                  break;
               default:
                  if (++x > width) width = x;
                  break;
            }
         }
         if (width == 0)
            return new byte[1, 1];

         byte[,] result = new byte[width, row];
         row = 0;
         x = 0;

         for (int i = 0; i < message.Length; i++)
         {
            switch (message[i])
            {
               case '\n':
                  row++;
                  x = 0;
                  break;
               case '\r':
                  break;
               default:
                  System.Text.Encoding.Unicode.GetBytes(message, i, 1, outByte, 0);
                  result[x++, row] = outByte[0];
                  break;
            }
         }
         return result;
      }

      private void cboTileset_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (cboTileset.SelectedIndex >= 0)
         {
            m_tileset = (ProjectDataset.TilesetRow)cboTileset.SelectedItem;
            grpPreview.Invalidate();
         }
      }

      private void grpPreview_Paint(object sender, PaintEventArgs e)
      {
         Rectangle bounds = ((GroupBox)sender).ClientRectangle;
         bounds.Inflate(-4, -10);
         bounds.Y += 6;
         DrawGuides(e.Graphics, bounds);
         DrawMessage(e.Graphics, bounds);
      }

      private void txtMessage_TextChanged(object sender, EventArgs e)
      {
         grpPreview.Invalidate();
      }

      private void btnExtended_Click(object sender, EventArgs e)
      {
         if (m_tileset == null)
         {
            MessageBox.Show(this, "Select a tileset first", "Insert Extended Character", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         string extChar = frmEditMessageChar.GetExtendedCharacter(m_tileset, this);
         if (extChar != null)
            txtMessage.SelectedText = extChar;
      }
   }
}
