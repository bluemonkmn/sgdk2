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
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for DragPanel.
	/// </summary>
	public class DragPanel : System.Windows.Forms.ScrollableControl
	{
      #region Win32 API Constants
      public const int WS_EX_CLIENTEDGE = unchecked((int)0x00000200);
      public const int WS_BORDER = unchecked((int)0x00800000);
      public const int WS_CAPTION = unchecked((int)0x00C00000);
      public const int WS_SIZEBOX = unchecked((int)0x00040000);
      public const int WS_DLGFRAME = unchecked((int)0x00400000L);
      public const int WS_EX_TOOLWINDOW = unchecked((int)0x00000080L);
      public const int WS_CHILD = unchecked((int)0x40000000L);
      public const int WS_POPUP = unchecked((int)0x80000000L);
      public const int WS_SYSMENU = unchecked((int)0x00080000L);
      public const int WS_VISIBLE = unchecked((int)0x10000000L);
      public const int WS_EX_CONTROLPARENT = unchecked((int)0x00010000L);
      public const int WS_EX_DLGMODALFRAME = unchecked((int)0x00000001L);
      public const int WS_EX_MDICHILD = unchecked((int)0x00000040L);
      public const int WS_EX_WINDOWEDGE = unchecked((int)0x00000100L);
      public const int WS_EX_TOPMOST = unchecked((int)0x00000008L);
      #endregion

      #region Fields
      private DragPanelBorderStyle m_BorderStyle;
      #endregion

      public DragPanelBorderStyle BorderStyle
      {
         get
         {
            return m_BorderStyle;
         }
         set
         {
            m_BorderStyle = value;
            UpdateStyles();
         }
      }
	
      #region Overrides

      protected override CreateParams CreateParams
      {
         get
         {
            CreateParams cp = base.CreateParams;
            cp.ExStyle &= ~WS_EX_CLIENTEDGE;
            cp.Style &= ~(WS_BORDER | WS_CAPTION | WS_SIZEBOX | WS_DLGFRAME);

            switch (m_BorderStyle)
            {
               case DragPanelBorderStyle.FixedInset:
                  cp.ExStyle |= WS_EX_CLIENTEDGE;
                  break;
               case DragPanelBorderStyle.FixedRaised:
                  cp.Style |= WS_DLGFRAME;
                  break;
               case DragPanelBorderStyle.Sizable:
                  cp.Style |= WS_SIZEBOX;
                  break;
               case DragPanelBorderStyle.FixedSingle:
                  cp.Style |= WS_BORDER;
                  break;
            }

            if ((Text.Length > 0) && (DragPanelBorderStyle.None != m_BorderStyle))
               cp.Style |= WS_CAPTION;

            return cp;
         }
      }
   
      public override string Text
      {
         get
         {
            return base.Text;
         }
         set
         {
            base.Text = value;
            UpdateStyles();
         }
      }
   }
   #endregion

   public enum DragPanelBorderStyle
   {
      None,
      FixedSingle,
      FixedRaised,
      FixedInset,
      Sizable
   }
}
