using System;
using System.Windows.Forms;
using System.Data;

namespace SGDK2
{
   public class DataGridComboBoxColumn : DataGridColumnStyle
   {
      private DataGridComboBox m_control = null;
      private EventHandler m_SelectionChangeEvent = null;
      private int m_rowIndex = -1;

      public DataGridComboBoxColumn(System.ComponentModel.PropertyDescriptor desc, DataRow[] values, string foreignMember) : base(desc)
      {
         m_control = new DataGridComboBox();
         m_control.Visible = false;
         m_control.DropDownStyle = ComboBoxStyle.DropDownList;
         m_control.DisplayMember = foreignMember;
         m_control.ValueMember = foreignMember;
         m_control.Items.AddRange(values);
         m_SelectionChangeEvent = new EventHandler(Control_SelectionChangeCommitted);
         m_control.SelectionChangeCommitted += m_SelectionChangeEvent;
         m_control.Leave += new EventHandler(Control_Leave);
      }

      public DataGridComboBoxColumn(System.ComponentModel.PropertyDescriptor desc, string[] values) : base(desc)
      {
         m_control = new DataGridComboBox();
         m_control.Visible = false;
         m_control.DropDownStyle = ComboBoxStyle.DropDownList;
         m_control.Items.AddRange(values);
         m_SelectionChangeEvent = new EventHandler(Control_SelectionChangeCommitted);
         m_control.SelectionChangeCommitted += m_SelectionChangeEvent;
         m_control.Leave += new EventHandler(Control_Leave);
      }

      protected override void Abort(int rowNum)
      {
         m_control.Hide();
      }
   
      protected override bool Commit(CurrencyManager dataSource, int rowNum)
      {
         if (rowNum < dataSource.List.Count)
         {
            if (m_rowIndex == rowNum)
            {
               if (m_control.SelectedItem is DataRow)
                  SetColumnValueAtRow(dataSource, rowNum,
                     ((DataRow)m_control.SelectedItem)[m_control.ValueMember]);
               else if (m_control.SelectedItem is string)
                  SetColumnValueAtRow(dataSource, rowNum, m_control.SelectedItem.ToString());
               else
                  SetColumnValueAtRow(dataSource, rowNum, Convert.DBNull);
               Invalidate();
            }
            return true;
         }
         return true;
      }
   
      protected override void Edit(CurrencyManager source, int rowNum, System.Drawing.Rectangle bounds, bool readOnly, string instantText, bool cellIsVisible)
      {
         m_control.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);

         if (m_rowIndex != rowNum)
         {
            m_control.SelectionChangeCommitted -= m_SelectionChangeEvent;
            m_control.SelectedIndex = -1;
            m_rowIndex = -1; // current value does not represent value on any grid row

            if (rowNum < source.List.Count)
            {
               string currentValue = GetColumnValueAtRow(source, rowNum).ToString();
               for (int i = 0; i < m_control.Items.Count; i++)
                  if (m_control.GetItemText(m_control.Items[i]) == currentValue)
                  {
                     m_control.SelectedIndex = i;
                     break;
                  }
               m_rowIndex = rowNum;
            } 
            m_control.SelectionChangeCommitted += m_SelectionChangeEvent;
         }
         m_control.Show();
         m_control.Focus();
      }
   
      protected override int GetMinimumHeight()
      {
         return m_control.PreferredHeight;
      }
   
      protected override int GetPreferredHeight(System.Drawing.Graphics g, object value)
      {
         return m_control.PreferredHeight;
      }
   
      protected override System.Drawing.Size GetPreferredSize(System.Drawing.Graphics g, object value)
      {
         return new System.Drawing.Size(
            (int)g.MeasureString(value.ToString(), DataGridTableStyle.DataGrid.Font).Width,
            m_control.PreferredHeight);
      }
   
      protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle bounds, CurrencyManager source, int rowNum)
      {
         Paint(g, bounds, source, rowNum, false);
      }
   
      protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight)
      {
         System.Drawing.Brush textBrush = new System.Drawing.SolidBrush(DataGridTableStyle.ForeColor);
         System.Drawing.Brush backBrush = new System.Drawing.SolidBrush(DataGridTableStyle.BackColor);
         System.Drawing.StringFormat sf = new System.Drawing.StringFormat();

         try
         {
            g.FillRectangle(backBrush, bounds);
            bounds.Inflate(-1, -2);
            bounds.Offset(1, 2);
            if (alignToRight)
               sf.Alignment = System.Drawing.StringAlignment.Far;
            else
               sf.Alignment = System.Drawing.StringAlignment.Near;
            string displayString = GetColumnValueAtRow(source, rowNum) as string;
            if (displayString == null)
               displayString = NullText;
            g.DrawString(displayString , DataGridTableStyle.DataGrid.Font, textBrush, bounds, sf);
         }
         finally
         {
            sf.Dispose();
            textBrush.Dispose();
            backBrush.Dispose();
         }
      }      
   
      protected override void SetDataGridInColumn(DataGrid value)
      {
         if (!value.Controls.Contains(m_control))
         {
            value.Controls.Add(m_control);
            this.WidthChanged += new EventHandler(DataGridComboBoxColumn_WidthChanged);
         }
         base.SetDataGridInColumn (value);
      }

      private void DataGridComboBoxColumn_WidthChanged(object sender, EventArgs e)
      {
         m_control.Width = Width;
      }

      private void Control_Leave(object sender, EventArgs e)
      {
         m_control.Hide();
         Invalidate();
      }

      private void Control_SelectionChangeCommitted(object sender, EventArgs e)
      {
         ColumnStartedEditing(m_control);
      }

      /// <summary>
      /// Returns the combo box control used to edit column values
      /// </summary>
      public DataGridComboBox Control
      {
         get
         {
            return m_control;
         }
      }
   }

   public class DataGridComboBox : ComboBox
   {
      private const int WM_KEYUP = 0x101;
      private const int WM_KEYDOWN = 0x100;

      protected override void WndProc(ref System.Windows.Forms.Message theMessage)
      {
         // Ignore KeyUp event to avoid double-tabbing through dropdown.
         if ((theMessage.Msg == WM_KEYUP) && (theMessage.WParam.ToInt32() == (int)Keys.Tab))
         {
            return;
         }
         else
         {
            base.WndProc(ref theMessage);
         }
      }
   }
}
