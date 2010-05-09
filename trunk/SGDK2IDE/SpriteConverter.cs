/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections;

namespace SGDK2
{
	/// <summary>
	/// Exposes properties of a Sprite to a property browser
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
               new ReflectionPropertyDescriptor(typ.GetProperty("Name")),
               new ReflectionPropertyDescriptor(typ.GetProperty("X")),
               new ReflectionPropertyDescriptor(typ.GetProperty("Y")),
               new ReflectionPropertyDescriptor(typ.GetProperty("DX")),
               new ReflectionPropertyDescriptor(typ.GetProperty("DY")),
               new ReflectionPropertyDescriptor(typ.GetProperty("Priority")),
               new ReflectionPropertyDescriptor(typ.GetProperty("CurrentStateName"), this),
               new ReflectionPropertyDescriptor(typ.GetProperty("CurrentFrame")),
               new ReflectionPropertyDescriptor(typ.GetProperty("Active")),
               new ReflectionPropertyDescriptor(typ.GetProperty("DefinitionName")),
               new ReflectionPropertyDescriptor(typ.GetProperty("Solidity"), this),
               new ReflectionPropertyDescriptor(typ.GetProperty("ModulateRed")),
               new ReflectionPropertyDescriptor(typ.GetProperty("ModulateGreen")),
               new ReflectionPropertyDescriptor(typ.GetProperty("ModulateBlue")),
               new ReflectionPropertyDescriptor(typ.GetProperty("ModulateAlpha"))
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
         if ((context.PropertyDescriptor != null) && (string.Compare(context.PropertyDescriptor.Name, "Solidity", true) == 0))
         {
            string[] solidityList = new string[ProjectData.Solidity.DefaultView.Count];
            int idx = 0;
            foreach(System.Data.DataRowView drv in ProjectData.Solidity.DefaultView)
               solidityList[idx++] = ((ProjectDataset.SolidityRow)drv.Row).Name;
            return new System.ComponentModel.TypeConverter.StandardValuesCollection(
               solidityList);
         }

         if (context.Instance is SpriteProvider)
         {
            if (string.Compare(context.PropertyDescriptor.Name, "CurrentStateName", true) == 0)
            {
               return new System.ComponentModel.TypeConverter.StandardValuesCollection(
                  ((SpriteProvider)context.Instance).GetAvailableStates());
            }
         }
         System.Collections.Specialized.StringCollection vals = null;
         foreach(SpriteProvider sp in (SpriteProvider[])context.Instance)
         {
            if(vals == null)
            {
               vals = new System.Collections.Specialized.StringCollection();
               vals.AddRange(sp.GetAvailableStates());
            }
            else
            {
               string[] valcopy = new string[vals.Count];
               vals.CopyTo(valcopy, 0);
               foreach(string s in valcopy)
                  if (Array.IndexOf(sp.GetAvailableStates(), s) < 0)
                     vals.Remove(s);               
            }
         }
         string[] result = new string[vals.Count];
         vals.CopyTo(result,0);
         return new System.ComponentModel.TypeConverter.StandardValuesCollection(result);
      }
   
      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
         if ((context.PropertyDescriptor != null) && (string.Compare(context.PropertyDescriptor.Name, "Solidity", true) == 0))
            return true;
         if ((context.Instance is SpriteProvider) && (context.PropertyDescriptor != null) &&
            (context.PropertyDescriptor.Name.Equals("CurrentStateName")))
            return true;
         if ((context.Instance is SpriteProvider[]) && (context.PropertyDescriptor != null) &&
             (context.PropertyDescriptor.Name.Equals("CurrentStateName")))
            return true;
         return base.GetStandardValuesSupported (context);
      }      
   }

   [TypeConverter(typeof(SpriteConverter))]
   public class SpriteProvider : IProvideFrame, IProvideGraphics, ICustomTypeDescriptor, ITypeDescriptorContext
   {
      private readonly CachedSpriteDef m_Sprite;
      private ProjectDataset.SpriteRow m_SpriteRow = null;
      private ProjectDataset.ParameterValueRow[] m_ParameterRows = null;
      private string m_State;
      private short m_Frame;
      private int[] m_ParameterValues;
      private string m_name;
      private int m_x;
      private int m_y;
      private float m_dx;
      private float m_dy;
      private int m_priority;
      private bool m_active;
      private string m_Solidity;
      private int m_color;
      private static int defaultPriority = 1;

      private static SpriteConverter m_converter = new SpriteConverter();
      private static Hashtable defaultInstances = new Hashtable();

      private SpriteProvider(CachedSpriteDef sprite, string state, short frame, int color)
      {
         m_Sprite = sprite;
         m_State = state;
         m_priority = int.MinValue;
         m_Frame = frame;
         m_ParameterValues = new int[m_Sprite.ParamNames.Count];
         m_color = color;
      }

      public SpriteProvider(CachedSpriteDef sprite, ProjectDataset.SpriteRow row)
      {
         m_Sprite = sprite;
         m_SpriteRow = row;
         ProjectDataset.SpriteParameterRow[] parmrows = ProjectData.GetSortedSpriteParameters(row.SpriteStateRowParent.SpriteDefinitionRow);
         m_ParameterRows = new ProjectDataset.ParameterValueRow[parmrows.Length];
         for(int i = 0; i < parmrows.Length; i++)
            m_ParameterRows[i] = ProjectData.GetSpriteParameterValueRow(row, parmrows[i].Name);
      }

      public static SpriteProvider GetDefaultInstance(CachedSpriteDef sprite, string defaultState, short defaultFrame, int defaultColor)
      {
         if (!defaultInstances.ContainsKey(sprite.Name))
            defaultInstances[sprite.Name] = new SpriteProvider(sprite, defaultState, defaultFrame, defaultColor);
         SpriteProvider result = (SpriteProvider)defaultInstances[sprite.Name];
         if (result.m_Sprite != sprite)
         {
            SpriteProvider newResult = new SpriteProvider(sprite, defaultState, defaultFrame, defaultColor);
            newResult.m_active = result.Active;
            newResult.m_color = result.Color;
            newResult.m_dx = result.m_dx;
            newResult.m_dy = result.m_dy;
            newResult.m_Frame = result.m_Frame;
            for(int i=0; i<newResult.ParameterNames.Count; i++)
            {
               int paramIdx = result.ParameterNames.IndexOf(newResult.ParameterNames[i]);
               if (paramIdx >= 0)
                  newResult.ParameterValues[i] = result.m_ParameterValues[paramIdx];
            }
            newResult.m_priority = result.m_priority;
            newResult.m_Solidity = result.m_Solidity;
            if (sprite.ContainsState(result.m_State) &&
                sprite[result.m_State].frames.Length > 0)
               newResult.m_State = result.m_State;
            defaultInstances[sprite.Name] = result = newResult;
         }
         return result;
      }

      [Description("Name of the sprite definition on which this sprite is based")]
      public string DefinitionName
      {
         get
         {
            return m_Sprite.Name;
         }
      }

      [Description("Name of this sprite instance unique in its layer")]
      public string Name
      {
         get
         {
            if (m_SpriteRow != null)
               return m_SpriteRow.Name;
            else
               return m_name;
         }
         set
         {
            if (m_SpriteRow != null)
            {
               if (ProjectData.GetSpritePlan(m_SpriteRow.LayerRowParent, value) != null)
                  throw new ApplicationException("Sprite name \"" + value + "\" conflicts with the name of an existing plan.  Choose a name that does not conflict with that of a plan or another sprite.");
               string error = ProjectData.ValidateName(value);
               if (!string.IsNullOrEmpty(error))
                  throw new ApplicationException(error);
               m_SpriteRow.Name = value;
            }
            else
               m_name = value;
         }
      }

      [Description("Horizontal pixel coordinate of the sprite within its layer")]
      public int X
      {
         get
         {
            if (m_SpriteRow != null)
               return m_SpriteRow.X;
            else
               return m_x;
         }
         set
         {
            if (m_SpriteRow != null)
               m_SpriteRow.X = value;
            else
               m_x = value;
         }
      }

      [Description("Vertical pixel coordinate of the sprite within its layer")]
      public int Y
      {
         get
         {
            if (m_SpriteRow != null)
               return m_SpriteRow.Y;
            else
               return m_y;
         }
         set
         {
            if (m_SpriteRow != null)
               m_SpriteRow.Y = value;
            else
               m_y = value;
         }
      }

      [Description("Horizontal velocity of the sprite in pixels per frame")]
      public float DX
      {
         get
         {
            if (m_SpriteRow != null)
               return m_SpriteRow.DX;
            else
               return m_dx;
         }
         set
         {
            if (m_SpriteRow != null)
               m_SpriteRow.DX = value;
            else
               m_dx = value;
         }
      }

      [Description("Vertical velocity of the sprite in pixels per frame")]
      public float DY
      {
         get
         {
            if (m_SpriteRow != null)
               return m_SpriteRow.DY;
            else
               return m_dy;
         }
         set
         {
            if (m_SpriteRow != null)
               m_SpriteRow.DY = value;
            else
               m_dy = value;
         }
      }

      [Description("When the sprite is drawn relative to tiles and other sprites. Higher priority numbers draw in front. Tiles are drawn according to layer's priority.")]
      public int Priority
      {
         get
         {
            if (m_SpriteRow != null)
               return m_SpriteRow.Priority;
            else
            {
               if (m_priority == int.MinValue)
                  return defaultPriority;
               else
                  return m_priority;
            }
         }
         set
         {
            if (m_SpriteRow != null)
               m_SpriteRow.Priority = value;
            else
            {
               m_priority = value;
               defaultPriority = value;
            }
         }
      }

      public int Color
      {
         get
         {
            if (m_SpriteRow != null)
               return m_SpriteRow.Color;
            else
               return m_color;
         }
         set
         {
            if (m_SpriteRow != null)
               m_SpriteRow.Color = value;
            else
               m_color = value;
         }
      }

      [Description("How much red is retained when this sprite instance is drawn. 0 = 0%, 255 = 100%")]
      public byte ModulateRed
      {
         get
         {
            if (m_SpriteRow != null)
               return BitConverter.GetBytes(m_SpriteRow.Color)[2];
            else
               return BitConverter.GetBytes(m_color)[2];
         }
         set
         {
            if (m_SpriteRow != null)
               m_SpriteRow.Color = (int)(m_SpriteRow.Color & 0xFF00FFFFU) | value << 16;
            else
               m_color = (int)(m_color & 0xFF00FFFFU) | value << 16;
         }
      }

      [Description("How much green is retained when this sprite instance is drawn. 0 = 0%, 255 = 100%")]
      public byte ModulateGreen
      {
         get
         {
            if (m_SpriteRow != null)
               return BitConverter.GetBytes(m_SpriteRow.Color)[1];
            else
               return BitConverter.GetBytes(m_color)[1];
         }
         set
         {
            if (m_SpriteRow != null)
               m_SpriteRow.Color = (int)(m_SpriteRow.Color & 0xFFFF00FFU) | value << 8;
            else
               m_color = (int)(m_color & 0xFFFF00FFU) | value << 8;
         }
      }

      [Description("How much blue is retained when this sprite instance is drawn. 0 = 0%, 255 = 100%")]
      public byte ModulateBlue
      {
         get
         {
            if (m_SpriteRow != null)
               return BitConverter.GetBytes(m_SpriteRow.Color)[0];
            else
               return BitConverter.GetBytes(m_color)[0];
         }
         set
         {
            if (m_SpriteRow != null)
               m_SpriteRow.Color = (int)(m_SpriteRow.Color & 0xFFFFFF00U) | value;
            else
               m_color = (int)(m_color & 0xFFFFFF00U) | value;
         }
      }
      
      [Description("Opacity of the sprite: 0 = invisible, 128 = translucent, 255 = opaque")]
      public byte ModulateAlpha
      {
         get
         {
            if (m_SpriteRow != null)
               return BitConverter.GetBytes(m_SpriteRow.Color)[3];
            else
               return BitConverter.GetBytes(m_color)[3];
         }
         set
         {
            if (m_SpriteRow != null)
               m_SpriteRow.Color = (int)(m_SpriteRow.Color & 0x00FFFFFFU) | value << 24;
            else
               m_color = (int)(m_color & 0x00FFFFFFU) | value << 24;
         }
      }
      
      [Description("Determines if this sprite instance is currently being drawn and its rules processed")]
      public bool Active
      {
         get
         {
            if (m_SpriteRow != null)
               return m_SpriteRow.Active;
            else
               return m_active;
         }
         set
         {
            if (m_SpriteRow != null)
               m_SpriteRow.Active = value;
            else
               m_active = value;
         }
      }

      [Description("Specifies which state the sprite instance is currently in")]
      public string CurrentStateName
      {
         get
         {
            if (m_SpriteRow != null)
               return m_SpriteRow.StateName;
            else
               return m_State;
         }
         set
         {
            if (m_Sprite.ContainsState(value))
            {
               if (m_SpriteRow != null)
                  m_SpriteRow.StateName = value;
               else
                  m_State = value;
            }
            else
            {
               throw new InvalidOperationException("State " + value + " does not exist");
            }
         }
      }

      [Description("Determines which tiles and parts of tiles this sprite sees as solid")]
      public string Solidity
      {
         get
         {
            if (m_SpriteRow != null)
               return m_SpriteRow.Solidity;
            else
               return m_Solidity;
         }
         set
         {
            if (ProjectData.GetSolidity(value) != null)
            {
               if (m_SpriteRow != null)
                  m_SpriteRow.Solidity = value;
               else
                  m_Solidity = value;
            }
            else
            {
               throw new InvalidOperationException("Solidity " + value + " does not exist");
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

      [Description("Determines which animation frame from the sprite's current state is displayed. Affected by Repeat Count of the frames")]
      public short CurrentFrame
      {
         get
         {
            if (m_SpriteRow != null)
               return m_SpriteRow.CurrentFrame;
            else
               return m_Frame;
         }
         set
         {
            if (m_SpriteRow != null)
               m_SpriteRow.CurrentFrame = value;
            else
               m_Frame = value;
         }
      }

      public int this[string ParamName]
      {
         get
         {
            if (m_ParameterRows != null)
            {
               for(int i = 0; i < m_ParameterRows.Length; i++)
                  if (m_ParameterRows[i].SpriteParameterRowParent.Name.Equals(ParamName))
                     return m_ParameterRows[i].Value;
               throw new IndexOutOfRangeException("Parameter " + ParamName + " not found");
            }
            else
            {
               int pidx = m_Sprite.ParamNames.IndexOf(ParamName);
               if (pidx < 0)
                  throw new IndexOutOfRangeException("Parameter " + ParamName + " not found");
               return m_ParameterValues[pidx];
            }
         }
         set
         {
            if (m_ParameterRows != null)
            {
               for(int i = 0; i < m_ParameterRows.Length; i++)
                  if (m_ParameterRows[i].SpriteParameterRowParent.Name.Equals(ParamName))
                  {
                     m_ParameterRows[i].Value = value;
                     return;
                  }
               throw new IndexOutOfRangeException("Parameter " + ParamName + " not found");
            }
            else
            {
               int pidx = m_Sprite.ParamNames.IndexOf(ParamName);
               if (pidx < 0)
                  throw new IndexOutOfRangeException("Parameter " + ParamName + " not found");
               m_ParameterValues[pidx] = value;
            }
         }
      }

      public int[] ParameterValues
      {
         get
         {
            if (m_ParameterRows != null)
            {
               int[] result = new int[m_ParameterRows.Length];
               for(int i = 0; i < result.Length; i++)
                  result[i] = m_ParameterRows[i].Value;
               return result;
            }
            else
               return m_ParameterValues;
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

      public int GetSubFrameCount()
      {
         return m_Sprite[CurrentStateName, CurrentFrame].Length;
      }

      public FrameCache.Frame GetSubFrame(int nSubFrame)
      {
         return m_Sprite[CurrentStateName].frameset[m_Sprite[CurrentStateName, CurrentFrame][nSubFrame]];
      }

      public Rectangle Bounds
      {
         get
         {
            Rectangle result = m_Sprite[CurrentStateName].Bounds;
            result.Offset(X, Y);
            return result;
         }
      }

      public bool IsDataRow
      {
         get
         {
            return m_SpriteRow != null;
         }
      }

      public void DeleteRow()
      {
         if (!IsDataRow)
            throw new InvalidOperationException("Cannot delete un-persisted sprite");
         m_SpriteRow.Delete();
      }

      #region IProvideFrame Members

      public int FrameIndex
      {
         get
         {
            if (m_Sprite[CurrentStateName,CurrentFrame].Length >0)
               return m_Sprite[CurrentStateName,CurrentFrame][0];
            else
               return 0;
         }
      }

      public int[] FrameIndexes
      {
         get
         {
            return m_Sprite[CurrentStateName,CurrentFrame];
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
         return m_Sprite[CurrentStateName].frameset[FrameIndexes[subFrame]].GraphicSheet;
      }

      public Rectangle GetRectangle(int subFrame)
      {
         return m_Sprite[CurrentStateName].frameset[FrameIndexes[subFrame]].SourceRect;
      }

      public Point[] GetTransformedCorners(int subFrame)
      {
         return m_Sprite[CurrentStateName].frameset[FrameIndexes[subFrame]].corners;
      }
      #endregion

      #region ICustomTypeDescriptor Members

      public TypeConverter GetConverter()
      {
         return m_converter;
      }

      public EventDescriptorCollection GetEvents(Attribute[] attributes)
      {
         return new EventDescriptorCollection(new EventDescriptor[] {});
      }

      EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
      {
         return new EventDescriptorCollection(new EventDescriptor[] {});
      }

      public string GetComponentName()
      {
         return this.Name;
      }

      public object GetPropertyOwner(PropertyDescriptor pd)
      {
         return this;
      }

      public AttributeCollection GetAttributes()
      {
         return new AttributeCollection(new Attribute[] {new TypeConverterAttribute(typeof(SpriteConverter))});
      }

      public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
      {
         return m_converter.GetProperties(this, this, attributes);
      }

      PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
      {
         return m_converter.GetProperties(this, this, null);
      }

      public object GetEditor(Type editorBaseType)
      {
         return null;
      }

      public PropertyDescriptor GetDefaultProperty()
      {
         return null;
      }

      public EventDescriptor GetDefaultEvent()
      {
         return null;
      }

      public string GetClassName()
      {
         return "Sprite";
      }

      #endregion

      #region ITypeDescriptorContext Members

      public void OnComponentChanged()
      {
      }

      public IContainer Container
      {
         get
         {
            return null;
         }
      }

      public bool OnComponentChanging()
      {
         return false;
      }

      public object Instance
      {
         get
         {            
            return this;
         }
      }

      public PropertyDescriptor PropertyDescriptor
      {
         get
         {
            return null;
         }
      }

      #endregion

      #region IServiceProvider Members

      public object GetService(Type serviceType)
      {
         return null;
      }

      #endregion
   }

   class ReflectionPropertyDescriptor : System.ComponentModel.PropertyDescriptor
   {
      System.Reflection.PropertyInfo m_PropertyInfo;
      
      public ReflectionPropertyDescriptor(System.Reflection.PropertyInfo pi) : base(pi.Name, new Attribute[] {new CategoryAttribute("Intrinsic"), new BrowsableAttribute(true)})
      {
         m_PropertyInfo = pi;
      }

      public ReflectionPropertyDescriptor(System.Reflection.PropertyInfo pi, TypeConverter tc) : base(pi.Name, new Attribute[] {new TypeConverterAttribute(tc.GetType()), new CategoryAttribute("Intrinsic"), new BrowsableAttribute(true)})
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

      public override string Description
      {
         get
         {
            DescriptionAttribute[] attr = m_PropertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), true) as DescriptionAttribute[];
            if ((attr != null) && (attr.Length > 0))
               return attr[0].Description;
            return string.Empty;
         }
      }

   }

   class SpriteParamDescriptor : System.ComponentModel.PropertyDescriptor
   {
      public SpriteParamDescriptor(string name) : base(name, new Attribute[] {new CategoryAttribute("Parameters")})
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
