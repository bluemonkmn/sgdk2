using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections;

namespace SGDK2
{
	/// <summary>
	/// Summary description for SpriteConverter.
	/// </summary>
	public class SpriteConverter : TypeConverter
	{
		public SpriteConverter()
		{
		}

      public override System.ComponentModel.PropertyDescriptorCollection GetProperties(System.ComponentModel.ITypeDescriptorContext context, object value, Attribute[] attributes)
      {
         if (context.Instance is SpriteProvider)
         {
            ArrayList properties = new ArrayList();
            System.Type typ = typeof(SpriteProvider);
            properties.AddRange(new ReflectionPropertyDescriptor[]
            {
               new ReflectionPropertyDescriptor(typ.GetProperty("X")),
               new ReflectionPropertyDescriptor(typ.GetProperty("Y")),
               new ReflectionPropertyDescriptor(typ.GetProperty("DX")),
               new ReflectionPropertyDescriptor(typ.GetProperty("DY")),
               new ReflectionPropertyDescriptor(typ.GetProperty("Priority")),
               new ReflectionPropertyDescriptor(typ.GetProperty("CurrentStateName")),
               new ReflectionPropertyDescriptor(typ.GetProperty("CurrentFrame"))
            });

            foreach(string paramName in ((SpriteProvider)context.Instance).ParameterNames)
            {
               properties.Add(new SpriteParamDescriptor(paramName));
            }
            return new PropertyDescriptorCollection((PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor)));
         }
         return base.GetProperties (context, value, attributes);
      }
   
      public override bool GetPropertiesSupported(System.ComponentModel.ITypeDescriptorContext context)
      {
         if ((context.Instance is SpriteProvider) && (context.PropertyDescriptor == null))
            return true;
         return base.GetPropertiesSupported (context);
      }
   
      public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
         return new System.ComponentModel.TypeConverter.StandardValuesCollection(
            ((SpriteProvider)context.Instance).GetAvailableStates());
      }
   
      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
         if ((context.Instance is SpriteProvider) && (context.PropertyDescriptor != null) &&
            (context.PropertyDescriptor.Name.Equals("CurrentStateName")))
            return true;
         return base.GetStandardValuesSupported (context);
      }
      
      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
      {
         if ((context.Instance is SpriteProvider) && (context.PropertyDescriptor != null) &&
            (sourceType == typeof(string)))
         {
            if ((context.PropertyDescriptor.PropertyType == typeof(int)) || 
               (context.PropertyDescriptor.PropertyType == typeof(float)))
               return true;
         }
         return base.CanConvertFrom (context, sourceType);
      }
   
      public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
      {
         if ((context.Instance is SpriteProvider) && (context.PropertyDescriptor != null) && (value is string))
         {
            if (context.PropertyDescriptor.PropertyType == typeof(int))
               return int.Parse((string)value);
            if (context.PropertyDescriptor.PropertyType == typeof(float))
               return float.Parse((string)value);
         }
         return base.ConvertFrom (context, culture, value);
      }
   }

   [TypeConverter(typeof(SpriteConverter))]
   class SpriteProvider : IProvideFrame, IProvideGraphics
   {
      private readonly CachedSprite m_Sprite;
      private string m_State;
      private int m_Frame;
      private int[] m_ParameterValues;
      private int m_x;
      private int m_y;
      private float m_dx;
      private float m_dy;
      private int m_priority;

      public SpriteProvider(CachedSprite sprite, string state, int frame)
      {
         m_Sprite = sprite;
         m_State = state;
         m_Frame = frame;
         m_ParameterValues = new int[m_Sprite.ParamNames.Count];
      }

      public int X
      {
         get
         {
            return m_x;
         }
         set
         {
            m_x = value;
         }
      }

      public int Y
      {
         get
         {
            return m_y;
         }
         set
         {
            m_y = value;
         }
      }

      public float DX
      {
         get
         {
            return m_dx;
         }
         set
         {
            m_dx = value;
         }
      }

      public float DY
      {
         get
         {
            return m_dy;
         }
         set
         {
            m_dy = value;
         }
      }

      public int Priority
      {
         get
         {
            return m_priority;
         }
         set
         {
            m_priority = value;
         }
      }

      public string CurrentStateName
      {
         get
         {
            return m_State;
         }
         set
         {
            if (m_Sprite.ContainsState(value))
            {
               m_State = value;
            }
            else
            {
               throw new InvalidOperationException("State " + value + " does not exist");
            }
         }
      }

      public System.Collections.Specialized.StringCollection ParameterNames
      {
         get
         {
            return m_Sprite.ParamNames;
         }
      }

      public int CurrentFrame
      {
         get
         {
            return m_Frame;
         }
         set
         {
            m_Frame = value;
         }
      }

      public int this[string ParamName]
      {
         get
         {
            int pidx = m_Sprite.ParamNames.IndexOf(ParamName);
            if (pidx < 0)
               throw new IndexOutOfRangeException("Parameter " + ParamName + " not found");
            return m_ParameterValues[pidx];
         }
         set
         {
            int pidx = m_Sprite.ParamNames.IndexOf(ParamName);
            if (pidx < 0)
               throw new IndexOutOfRangeException("Parameter " + ParamName + " not found");
            m_ParameterValues[pidx] = value;
         }
      }

      public string[] GetAvailableStates()
      {
         string[] result = new string[m_Sprite.Count];

         int i=0;
         foreach(System.Collections.DictionaryEntry de in m_Sprite)
         {
            result[i++] = de.Key.ToString();
         }
         return result;
      }

      #region IProvideFrame Members

      public int FrameIndex
      {
         get
         {
            if (m_Sprite[m_State,m_Frame].Length >0)
               return m_Sprite[m_State,m_Frame][0];
            else
               return 0;
         }
      }

      public int[] FrameIndexes
      {
         get
         {
            return m_Sprite[m_State,m_Frame];
         }
      }

      public bool IsSelected
      {
         get
         {
            return false;
         }
         set
         {
         }
      }
      #endregion

      #region IProvideGraphics Members

      public SGDK2.ProjectDataset.GraphicSheetRow GetGraphicSheet(int subFrame)
      {
         return m_Sprite[m_State].frameset[FrameIndexes[subFrame]].GraphicSheet;
      }

      public Rectangle GetRectangle(int subFrame)
      {
         return m_Sprite[m_State].frameset[FrameIndexes[subFrame]].SourceRect;
      }

      /// <summary>
      /// Create a new GDI+ matrix object
      /// </summary>
      /// <param name="subFrame">Component of the frame whose matrix to create</param>
      /// <returns>new matrix instance</returns>
      /// <remarks>Caller must dispose of the returned object</remarks>
      public System.Drawing.Drawing2D.Matrix CreateMatrix(int subFrame)
      {
         Microsoft.DirectX.Matrix m3d = m_Sprite[m_State].frameset[FrameIndexes[subFrame]].Transform;
         return new System.Drawing.Drawing2D.Matrix(m3d.M11, m3d.M12, m3d.M21, m3d.M22, m3d.M41, m3d.M42);
      }

      #endregion
   }

   class ReflectionPropertyDescriptor : System.ComponentModel.PropertyDescriptor
   {
      System.Reflection.PropertyInfo m_PropertyInfo;
      
      public ReflectionPropertyDescriptor(System.Reflection.PropertyInfo pi) : base(pi.Name, new Attribute[] {new TypeConverterAttribute(typeof(SpriteConverter))})
      {
         m_PropertyInfo = pi;
      }

      public override bool IsReadOnly
      {
         get
         {
            return !m_PropertyInfo.CanWrite;
         }
      }
   
      public override bool CanResetValue(object component)
      {
         return false;
      }
   
      public override Type ComponentType
      {
         get
         {
            return m_PropertyInfo.DeclaringType;
         }
      }
   
      public override Type PropertyType
      {
         get
         {
            return m_PropertyInfo.PropertyType;
         }
      }
   
      public override object GetValue(object component)
      {
         return m_PropertyInfo.GetValue(component, new object[] {});
      }
   
      public override void ResetValue(object component)
      {
         return;
      }
   
      public override void SetValue(object component, object value)
      {
         m_PropertyInfo.SetValue(component, value, new object[] {});
      }
   
      public override bool ShouldSerializeValue(object component)
      {
         return true;
      }
   }

   class SpriteParamDescriptor : System.ComponentModel.PropertyDescriptor
   {
      public SpriteParamDescriptor(string name) : base(name, new Attribute[] {})
      {
      }

      public override bool IsReadOnly
      {
         get
         {
            return false;
         }
      }
   
      public override bool CanResetValue(object component)
      {
         return false;
      }
   
      public override Type ComponentType
      {
         get
         {
            return typeof(SpriteProvider);
         }
      }
   
      public override Type PropertyType
      {
         get
         {
            return typeof(int);
         }
      }
   
      public override object GetValue(object component)
      {
         return ((SpriteProvider)component)[base.Name];
      }
   
      public override void ResetValue(object component)
      {
         return;
      }
   
      public override void SetValue(object component, object value)
      {
         ((SpriteProvider)component)[base.Name] = (int)value;
      }
   
      public override bool ShouldSerializeValue(object component)
      {
         return true;
      }
   }
}
