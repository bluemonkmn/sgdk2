/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
using System;
using System.ComponentModel;

namespace SGDK2
{
   /// <summary>
   /// Exposes properties of a Sprite to a property browser
   /// </summary>
   public class FrameConverter : TypeConverter
	{
		public FrameConverter()
		{
		}

      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
         if ((context.PropertyDescriptor != null) && (context.PropertyDescriptor.Name == "GraphicSheet"))
         {
            return true;
         }
         return base.GetStandardValuesSupported (context);
      }

      public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
         if ((context.PropertyDescriptor != null) && (context.PropertyDescriptor.Name == "GraphicSheet"))
         {
            System.Collections.ArrayList result = new System.Collections.ArrayList();
            foreach(System.Data.DataRowView drv in ProjectData.GraphicSheet.DefaultView)
            {
               result.Add(((ProjectDataset.GraphicSheetRow)drv.Row).Name);
            }
            return new TypeConverter.StandardValuesCollection(result);
         }
         return base.GetStandardValues (context);
      }

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
      {
         return true;
      }
	}

   [TypeConverter(typeof(FrameConverter))]
   public class FrameProvider
   {      
      private ProjectDataset.FrameRow m_FrameRow = null;

      public FrameProvider(ProjectDataset.FrameRow frame)
      {
         m_FrameRow = frame;
      }

      [Category("Matrix")]
      public float m11
      {
         get
         {
            return m_FrameRow.m11;
         }
         set
         {
            m_FrameRow.m11 = value;
         }
      }
      [Category("Matrix")]
      public float m12
      {
         get
         {
            return m_FrameRow.m12;
         }
         set
         {
            m_FrameRow.m12 = value;
         }
      }

      [Category("Matrix")]
      public float m21
      {
         get
         {
            return m_FrameRow.m21;
         }
         set
         {
            m_FrameRow.m21 = value;
         }
      }

      [Category("Matrix")]
      public float m22
      {
         get
         {
            return m_FrameRow.m22;
         }
         set
         {
            m_FrameRow.m22 = value;
         }
      }

      [Category("Matrix")]
      public float dx
      {
         get
         {
            return m_FrameRow.dx;
         }
         set
         {
            m_FrameRow.dx = value;
         }
      }

      [Category("Matrix")]
      public float dy
      {
         get
         {
            return m_FrameRow.dy;
         }
         set
         {
            m_FrameRow.dy = value;
         }
      }

      [System.ComponentModel.Description("Index of the graphic sheet cell referenced by this frame"), Category("Source")]
      public short CellIndex
      {
         get
         {
            return m_FrameRow.CellIndex;
         }
         set
         {
            ProjectDataset.GraphicSheetRow drGfx = ProjectData.GetGraphicSheet(m_FrameRow.GraphicSheet);
            if (drGfx == null)
               throw new ApplicationException("Graphic Sheet is invalid");
            if (value >= drGfx.Rows * drGfx.Columns)
               throw new ApplicationException("CellIndex must be a value less than " + (drGfx.Rows * drGfx.Columns).ToString() + ".");
            if (value <0)
               throw new ApplicationException("CellIndex must be 0 or greater.");
            m_FrameRow.CellIndex = value;
         }
      }

      [Category("Color"), System.ComponentModel.Description("255 = retain 100% of the red, 0 = eliminate all red")]
      public byte ModulateRed
      {
         get
         {
            return BitConverter.GetBytes(m_FrameRow.color)[2];
         }
         set
         {
            m_FrameRow.color = (int)(m_FrameRow.color & 0xFF00FFFFU) | value << 16;
         }
      }

      [Category("Color"), System.ComponentModel.Description("255 = retain 100% of the green, 0 = eliminate all green")]
      public byte ModulateGreen
      {
         get
         {
            return BitConverter.GetBytes(m_FrameRow.color)[1];
         }
         set
         {
            m_FrameRow.color = (int)(m_FrameRow.color & 0xFFFF00FFU) | value << 8;
         }
      }

      [Category("Color"), System.ComponentModel.Description("255 = retain 100% of the blue, 0 = eliminate all blue")]
      public byte ModulateBlue
      {
         get
         {
            return BitConverter.GetBytes(m_FrameRow.color)[0];
         }
         set
         {
            m_FrameRow.color = (int)(m_FrameRow.color & 0xFFFFFF00U) | value;
         }
      }
      
      [Category("Color"), System.ComponentModel.Description("255 = opaque, 128 = 50% transparent, 0 = invisible")]
      public byte ModulateAlpha
      {
         get
         {
            return BitConverter.GetBytes(m_FrameRow.color)[3];
         }
         set
         {
            m_FrameRow.color = (int)(m_FrameRow.color & 0x00FFFFFFU) | value << 24;
         }
      }

      [System.ComponentModel.Description("Index of the frame within the frameset"), Category("Key")]
      public int FrameValue
      {
         get
         {
            return m_FrameRow.FrameValue;
         }
      }

      [TypeConverter(typeof(FrameConverter)), Category("Source"), System.ComponentModel.Description("Determines which graphic sheet's cells this frame's image is selected from.")]
      public string GraphicSheet
      {
         get
         {
            return m_FrameRow.GraphicSheet;
         }
         set
         {
            ProjectDataset.GraphicSheetRow drGfx = ProjectData.GetGraphicSheet(value);
            if (null == drGfx)
               throw new ApplicationException("Specified Graphic Sheet not found");
            if (drGfx.RowState == System.Data.DataRowState.Deleted)
               throw new ApplicationException("Cannot use deleted Graphic Sheets");
            if (m_FrameRow.CellIndex >= drGfx.Rows * drGfx.Columns)
               throw new ApplicationException("CellIndex would exceed the number of cells in the proposed Graphic Sheet");
            m_FrameRow.GraphicSheet = value;
         }
      }
   }
}
