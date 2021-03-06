/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
using System;
using System.ComponentModel;

namespace SGDK2
{
   public class PlanConverter : TypeConverter
   {
      System.Collections.Hashtable m_PlanProperties = null;
      System.Collections.Hashtable m_RemoteTypeCache = null;
      string[] m_PlanBaseClasses = null;
      string BaseClass;

      public PlanConverter()
      {
         this.BaseClass = CodeGenerator.PlanBaseClassName;
      }

      public PlanConverter(string BaseClass)
      {
         this.BaseClass = BaseClass;
      }

      public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
      {
         if (context.Instance is PlanProvider)
         {
            System.Collections.ArrayList properties = new System.Collections.ArrayList();
            System.Type typ = typeof(PlanProvider);
            properties.AddRange(new ReflectionPropertyDescriptor[]
            {
               new ReflectionPropertyDescriptor(typ.GetProperty("Name")),
               new ReflectionPropertyDescriptor(typ.GetProperty("Priority")),
               new ReflectionPropertyDescriptor(typ.GetProperty("BaseClass"), this)
            });

            InitCustomProperties(context);
            foreach(System.Collections.DictionaryEntry de in m_PlanProperties)
            {
               RemotingServices.RemotePropertyInfo pi = (RemotingServices.RemotePropertyInfo)de.Value;
               if ((pi.Flags & (RemotingServices.MemberFlags.Static | RemotingServices.MemberFlags.Browsable)) == RemotingServices.MemberFlags.Browsable)
                  properties.Add(new PlanParamDescriptor(pi, BaseClass));
            }
            return new PropertyDescriptorCollection((PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor)));
         }
         return base.GetProperties (context, value, attributes);
      }

      private void InitCustomProperties(ITypeDescriptorContext context)
      {
         if (m_PlanProperties == null)
         {
            m_PlanProperties = new System.Collections.Hashtable();

            CodeGenerator gen = new CodeGenerator();
            string errs;
            gen.GenerateLevel = CodeGenerator.CodeLevel.ExcludeRules;
            errs = gen.CompileTempAssembly(false);
            if ((errs != null) && (errs.Length > 0))
               return;

            RemotingServices.IRemoteTypeInfo reflector = CodeGenerator.CreateInstanceAndUnwrap(
               "RemoteReflector", BaseClass) as RemotingServices.IRemoteTypeInfo;

            foreach(RemotingServices.RemotePropertyInfo pi in reflector.GetProperties())
               if ((pi.Flags & RemotingServices.MemberFlags.CanWrite) != 0)
                  m_PlanProperties[pi.Name] = pi;
         }
      }

      public override bool GetPropertiesSupported(ITypeDescriptorContext context)
      {
         if ((context.Instance is PlanProvider) && (context.PropertyDescriptor == null))
            return true;
         return base.GetPropertiesSupported (context);
      }

      private string[] GetSpritesOfType(SGDK2.RemotingServices.RemoteTypeName type, ITypeDescriptorContext context, SGDK2.RemotingServices.IRemoteTypeInfo reflector)
      {
         if (!(context.Instance is PlanProvider))
            return null;

         RemotingServices.RemoteTypeName[] types = reflector.GetDerivedClasses(false);
         ProjectDataset.SpriteRow[] sprites = ProjectData.GetSortedSpriteRows(((PlanProvider)context.Instance).Plan.LayerRowParent, true);
         System.Collections.ArrayList result = new System.Collections.ArrayList();
         foreach(ProjectDataset.SpriteRow sprite in sprites)
         {
            foreach (RemotingServices.RemoteTypeName typ in types)
            {
               if (CodeGenerator.SpritesNamespace + "." + sprite[ProjectData.Sprite.DefinitionNameColumn].ToString() == typ.FullName)
                  result.Add("m_" + CodeGenerator.NameToVariable(sprite.Name));
            }
         }
         if (result.Count == 0)
            return null;
         return (string[])result.ToArray(typeof(string));
      }

      private string[] GetPlans(ITypeDescriptorContext context)
      {
         if (!(context.Instance is PlanProvider))
            return null;

         ProjectDataset.SpritePlanRow[] plans = ProjectData.GetSortedSpritePlans(((PlanProvider)context.Instance).Plan.LayerRowParent, true);
         string[] result = new string[plans.Length];
         for(int i=0; i<plans.Length; i++)
            result[i] = "m_" + CodeGenerator.NameToVariable(plans[i].Name);
         return result;
      }

      private string[] GetCustomObjects(RemotingServices.RemoteTypeName type, ITypeDescriptorContext context)
      {
         if (!(context.Instance is PlanProvider))
            return null;

         CodeGenerator gen = new CodeGenerator();
         string errs;
         gen.GenerateLevel = CodeGenerator.CodeLevel.ExcludeRules;
         errs = gen.CompileTempAssembly(false);
         if ((errs != null) && (errs.Length > 0))
            return null;

         RemotingServices.IRemoteTypeInfo reflector = CodeGenerator.CreateInstanceAndUnwrap(
            "RemoteReflector", type.FullName) as RemotingServices.IRemoteTypeInfo;

         RemotingServices.RemoteGlobalAccessorInfo[] providers = reflector.GetGlobalProvidersOfSelf();
         if (providers.Length == 0)
            return null;

         string[] results = new string[providers.Length];
         for(int i=0; i<providers.Length; i++)
            results[i] = providers[i].Type.FullName + "." + providers[i].MemberName;
         return results;
      }

      private string[] GetSpriteCollections(ITypeDescriptorContext context)
      {
         if (!(context.Instance is PlanProvider))
            return null;

         string[] result = new string[ProjectData.SpriteCategory.DefaultView.Count];
         int i = 0;
         foreach(System.Data.DataRowView drv in ProjectData.SpriteCategory.DefaultView)
         {
            result[i++] = CodeGenerator.SpriteCategoriesFieldName + "." +
               CodeGenerator.NameToVariable(((ProjectDataset.SpriteCategoryRow)drv.Row).Name);
         }
         return result;
      }

      string[] GetStandardValues(RemotingServices.RemoteTypeName type, ITypeDescriptorContext context)
      {
         if (m_RemoteTypeCache == null)
            m_RemoteTypeCache = new System.Collections.Hashtable();

         if (m_RemoteTypeCache.ContainsKey(type.FullName))
            return (string[])m_RemoteTypeCache[type.FullName];

         System.Type localType = Type.GetType(type.FullName, false, false);
         if (localType == null)
         {
            CodeGenerator gen = new CodeGenerator();
            string errs;
            gen.GenerateLevel = CodeGenerator.CodeLevel.ExcludeRules;
            errs = gen.CompileTempAssembly(false);
            if ((errs != null) && (errs.Length > 0))
               return (string[])(m_RemoteTypeCache[type.FullName] = null);

            RemotingServices.IRemoteTypeInfo reflector = CodeGenerator.CreateInstanceAndUnwrap(
               "RemoteReflector", type.FullName) as RemotingServices.IRemoteTypeInfo;

            if (reflector.IsEnum)
            {
               return (string[])(m_RemoteTypeCache[type.FullName] = reflector.GetEnumVals());
            }
            else
            {
               string[] result = GetSpritesOfType(type, context, reflector);
               if (result != null)
                  return (string[])(m_RemoteTypeCache[type.FullName] = result);
               else if (type.FullName == CodeGenerator.PlanBaseClassName)
                  return (string[])(m_RemoteTypeCache[type.FullName] = GetPlans(context));
               else if (type.FullName == CodeGenerator.SpriteCollectionClassName)
                  return (string[])(m_RemoteTypeCache[type.FullName] = GetSpriteCollections(context));
               else if ((result = GetCustomObjects(type, context)) != null)
                  return (string[])(m_RemoteTypeCache[type.FullName] = result);
               else
                  return (string[])(m_RemoteTypeCache[type.FullName] = null);
            }
         }
         else if (localType.IsEnum)
         {
            string[] result = System.Enum.GetNames(localType);
            for(int idx = 0; idx < result.Length; idx++)
               result[idx] = localType.FullName.Replace('+','.') + "." + result[idx];
            return result;
         }
         else
            return (string[])(m_RemoteTypeCache[type.FullName] = null);
      }

      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
         if ((context.Instance is PlanProvider) && (context.PropertyDescriptor != null))
         {
            if (context.PropertyDescriptor is PlanParamDescriptor)
            {
               return GetStandardValues(((PlanParamDescriptor)context.PropertyDescriptor).RemotePropertyInfo.Type, context) != null;
            }
            if ((context.PropertyDescriptor is ReflectionPropertyDescriptor) &&
               (string.Compare(context.PropertyDescriptor.Name, "BaseClass", true) == 0))
            {
               return true;
            }
         }
         return base.GetStandardValuesSupported(context);
      }

      public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
         if ((context.PropertyDescriptor is ReflectionPropertyDescriptor) &&
             (string.Compare(context.PropertyDescriptor.Name, "BaseClass", true) == 0))
         {
            if (m_PlanBaseClasses != null)
               return new StandardValuesCollection(m_PlanBaseClasses);

            CodeGenerator gen = new CodeGenerator();
            string errs;
            gen.GenerateLevel = CodeGenerator.CodeLevel.ExcludeRules;
            errs = gen.CompileTempAssembly(false);
            if ((errs != null) && (errs.Length > 0))
               return new StandardValuesCollection((string[])(m_PlanBaseClasses = null));

            RemotingServices.IRemoteTypeInfo reflector = CodeGenerator.CreateInstanceAndUnwrap(
               "RemoteReflector", CodeGenerator.PlanBaseClassName) as RemotingServices.IRemoteTypeInfo;

            RemotingServices.RemoteTypeName[] subclasses = reflector.GetDerivedClasses(true);
            m_PlanBaseClasses = new string[subclasses.Length + 1];
            m_PlanBaseClasses[0] = CodeGenerator.PlanBaseClassName;
            for(int i = 1; i <= subclasses.Length; i++)
               m_PlanBaseClasses[i] = subclasses[i-1].FullName;
            return new StandardValuesCollection(m_PlanBaseClasses);
         }
         return new StandardValuesCollection(
            GetStandardValues(((PlanParamDescriptor)context.PropertyDescriptor).RemotePropertyInfo.Type, context));
      }

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
      {
         return false;
      }

      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
      {
         if ((context.Instance is PlanProvider) && (context.PropertyDescriptor != null) &&
            (context.PropertyDescriptor is PlanParamDescriptor) &&
            (sourceType == typeof(string)))
         {
            return true;
         }

         InitCustomProperties(context);
         if ((context.Instance is PlanProvider[]) && (context.PropertyDescriptor != null) &&
            (m_PlanProperties.ContainsKey(context.PropertyDescriptor.Name)) &&
            (sourceType == typeof(string)))
         {
            return true;
         }
         return base.CanConvertFrom (context, sourceType);
      }

      public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
      {
         if ((context.Instance is PlanProvider) && (context.PropertyDescriptor != null) &&
            (context.PropertyDescriptor is PlanParamDescriptor) &&
            (value is string) || (value == null))
         {
            return value.ToString();
         }
         InitCustomProperties(context);
         if ((context.Instance is PlanProvider[]) && (context.PropertyDescriptor != null) &&
            (m_PlanProperties.ContainsKey(context.PropertyDescriptor.Name)) &&
            (value is string) || (value == null))
         {
            return value.ToString();
         }
         return base.ConvertFrom (context, culture, value);
      }

      public int ParamStatus(string paramName)
      {
         if (m_PlanProperties == null) return 0;
         if (m_PlanProperties.ContainsKey(paramName)) return 1;
         return -1;
      }
   }

   class PlanParamDescriptor : PropertyDescriptor
   {
      private RemotingServices.RemotePropertyInfo propertyInfo;
      private static System.Collections.Generic.Dictionary<string, PlanConverter> m_converters = null;
      private string BaseClass;
      
      public PlanParamDescriptor(RemotingServices.RemotePropertyInfo propertyInfo, string BaseClass) : base(propertyInfo.Name, new Attribute[] {new CategoryAttribute("Parameters")})
      {
         this.propertyInfo = propertyInfo;
         this.BaseClass = BaseClass;
      }

      public static void ResetCustomProperties()
      {
         m_converters = null;
      }

      public RemotingServices.RemotePropertyInfo RemotePropertyInfo
      {
         get
         {
            return propertyInfo;
         }
      }

      public override bool IsReadOnly
      {
         get
         {
            return (propertyInfo.Flags & RemotingServices.MemberFlags.CanWrite) == 0;
         }
      }
   
      public override bool CanResetValue(object component)
      {
         return ((PlanProvider)component)[base.Name] != null;
      }
   
      public override Type ComponentType
      {
         get
         {
            return typeof(PlanProvider);
         }
      }
   
      public override Type PropertyType
      {
         get
         {
            return typeof(string);
         }
      }
         
      public override TypeConverter Converter
      {
         get
         {
            if (m_converters == null)
               m_converters = new System.Collections.Generic.Dictionary<string, PlanConverter>();
            if (!m_converters.ContainsKey(BaseClass))
               m_converters[BaseClass] = new PlanConverter(BaseClass);
            return m_converters[BaseClass];
         }
      }

      public override object GetValue(object component)
      {
         return ((PlanProvider)component)[base.Name];
      }
   
      public override void ResetValue(object component)
      {
         ((PlanProvider)component)[base.Name] = string.Empty;
      }
   
      public override void SetValue(object component, object value)
      {
         ((PlanProvider)component)[base.Name] = value.ToString();
      }
   
      public override bool ShouldSerializeValue(object component)
      {
         return true;
      }
      
      public override string Description
      {
         get
         {
            return propertyInfo.Description;
         }
      }

   }

	/// <summary>
	/// Exposes properties of a SpritePlanRow to a property browser
	/// </summary>
	[TypeConverter(typeof(PlanConverter))]
   public class PlanProvider : ICustomTypeDescriptor, ITypeDescriptorContext
   {
      private ProjectDataset.SpritePlanRow m_Plan;
      private System.Collections.Hashtable m_Params = null;
      
      private static System.Collections.Generic.Dictionary<string, PlanConverter> m_converters = null;

      public PlanProvider(ProjectDataset.SpritePlanRow plan)
      {
         m_Plan = plan;
      }

      public static void ResetCustomProperties()
      {
         m_converters = null;
      }

      public string Name
      {
         get
         {
            return m_Plan.Name;
         }
         set
         {
            if (ProjectData.GetSprite(m_Plan.LayerRowParent, value) != null)
               throw new ApplicationException("Plan name \"" + value + "\" conflicts with the name of a sprite.  Choose a name that does not conflict with that of a sprite or another plan.");
            string error = ProjectData.ValidateName(value);
            if (!string.IsNullOrEmpty(error))
               throw new ApplicationException(error);
            m_Plan.Name = value;
         }
      }

      [Description("Determines the order in which plans with rules are executed (lowest first)")]
      public int Priority
      {
         get
         {
            return m_Plan.Priority;
         }
         set
         {
            m_Plan.Priority = value;
         }
      }

      private void InitParams()
      {
         if (m_Params == null)
         {
            m_Params = new System.Collections.Hashtable();
            foreach (ProjectDataset.PlanParameterValueRow dr in m_Plan.GetPlanParameterValueRows())
            {
               m_Params[dr.Name] = dr;
            }
         }
         
         if ((m_converters != null) && (m_converters.ContainsKey(BaseClass)))
         {
            PlanConverter conv = m_converters[BaseClass];
            if (conv == null)
               return;
            System.Collections.Specialized.StringCollection toRemove = new System.Collections.Specialized.StringCollection();
            foreach (System.Collections.DictionaryEntry de in m_Params)
            {
               if (conv.ParamStatus((string)de.Key) == -1)
                  toRemove.Add((string)de.Key);
            }
            foreach (string key in toRemove)
            {
               ((ProjectDataset.PlanParameterValueRow)m_Params[key]).Delete();
               m_Params.Remove(key);
            }
         }
      }

      public string this[string propertyName]
      {
         get
         {
            if (propertyName == "Name")
               return Name;
            else if (propertyName == "Priority")
               return Priority.ToString();

            InitParams();
            if (m_Params.ContainsKey(propertyName))
               return ((ProjectDataset.PlanParameterValueRow)(m_Params[propertyName])).Value;
            else
               return null;
         }
         set
         {
            if (propertyName == "Name")
            {
               Name = value;
               return;
            }
            else if (propertyName == "Priority")
            {
               Priority = int.Parse(value);
               return;
            }
            InitParams();
            if ((value == String.Empty) && (m_Params.ContainsKey(propertyName)))
            {
               ((ProjectDataset.PlanParameterValueRow)m_Params[propertyName]).Delete();
               m_Params.Remove(propertyName);
            }
            else if ((value != String.Empty) && !m_Params.ContainsKey(propertyName))
               m_Params[propertyName] = ProjectData.AddPlanParameterValue(m_Plan, propertyName, value);
            else if (value != String.Empty)
               ((ProjectDataset.PlanParameterValueRow)(m_Params[propertyName])).Value = value;
         }
      }

      public ProjectDataset.SpritePlanRow Plan
      {
         get
         {
            return m_Plan;
         }
      }

      [Description("Source code object that defines the basic behavior of this plan (default=PlanBase)."),
      RefreshProperties(RefreshProperties.All)]
      public string BaseClass
      {
         get
         {
            return m_Plan.BaseClass;
         }
         set
         {
            m_Plan.BaseClass = value;
         }
      }

      #region ICustomTypeDescriptor Members

      public TypeConverter GetConverter()
      {
         if (m_converters == null)
            m_converters = new System.Collections.Generic.Dictionary<string,PlanConverter>();
         if (!m_converters.ContainsKey(BaseClass))
            m_converters[BaseClass] = new PlanConverter(BaseClass);
         return m_converters[BaseClass];
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
         return Name;
      }

      public object GetPropertyOwner(PropertyDescriptor pd)
      {
         return this;
      }

      public AttributeCollection GetAttributes()
      {
         return new AttributeCollection(new Attribute[] {new TypeConverterAttribute(typeof(PlanConverter))});
      }

      public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
      {
         return GetConverter().GetProperties(this, this, attributes);
      }

      PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
      {
         return GetConverter().GetProperties(this, this, null);
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
         return "Plan";
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

   public class CoordProvider
   {
      private ProjectDataset.CoordinateRow m_Coord;

      public CoordProvider(ProjectDataset.CoordinateRow coord)
      {
         m_Coord = coord;
      }

      public int X
      {
         get
         {
            return m_Coord.X;
         }
         set
         {
            m_Coord.X = value;
         }
      }

      public int Y
      {
         get
         {
            return m_Coord.Y;
         }
         set
         {
            m_Coord.Y = value;
         }
      }

      [Description("Can be used to specify how long a sprite will wait at this coordinate, or for other custom uses")]
      public int Weight
      {
         get
         {
            return m_Coord.Weight;
         }
         set
         {
            m_Coord.Weight = value;
         }
      }

      public int Sequence
      {
         get
         {
            return m_Coord.Sequence;
         }
      }

      public string Plan
      {
         get
         {
            return m_Coord.SpritePlanRowParent.Name;
         }
      }

      public override string ToString()
      {
         return X.ToString() + ", " + Y.ToString();
      }

      [Browsable(false)]
      public ProjectDataset.CoordinateRow CoordinateRow
      {
         get
         {
            return m_Coord;
         }
      }
   }
}
