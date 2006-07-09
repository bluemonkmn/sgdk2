using System;

/// <summary>
/// Utility class for interacting with objects accross AppDomains
/// </summary>
public class RemoteReflector : System.MarshalByRefObject, SGDK2.RemotingServices.IRemoteTypeInfo
{
   private Type reflectType;

   private RemoteReflector()
   {
   }

   public RemoteReflector(string typeName)
   {
      reflectType = typeof(RemoteReflector).Assembly.GetType(typeName, false);
      if (null != reflectType)
         return;
      reflectType = typeof(Microsoft.DirectX.DirectInput.KeyboardState).Assembly.GetType(typeName, false);
      if (null != reflectType)
         return;
      throw new System.ApplicationException("Failed to load type " + typeName);
   }
   #region IRemoteTypeInfo Members

   public SGDK2.RemotingServices.RemoteMethodInfo[] GetMethods()
   {
      System.Reflection.MethodInfo[] mi = reflectType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod);
      SGDK2.RemotingServices.RemoteMethodInfo[] result = new SGDK2.RemotingServices.RemoteMethodInfo[mi.Length];
      for (int i = 0; i < mi.Length; i++)
      {
         result[i].MethodName = mi[i].Name;
         result[i].ReturnType = mi[i].ReturnType.Name;
         System.ComponentModel.DescriptionAttribute da = System.Attribute.GetCustomAttribute(mi[i], typeof(System.ComponentModel.DescriptionAttribute), false) as System.ComponentModel.DescriptionAttribute;
         if (da == null)
            result[i].Description = String.Empty;
         else
            result[i].Description = da.Description;
         System.Reflection.ParameterInfo[] pi = mi[i].GetParameters();
         SGDK2.RemotingServices.RemoteParameterInfo[] rpi = new SGDK2.RemotingServices.RemoteParameterInfo[pi.Length];
         for (int j = 0; j < pi.Length; j++)
         {
            rpi[j].Name = pi[j].Name;
            rpi[j].IsEnum = pi[j].ParameterType.IsEnum;
            if (pi[j].ParameterType.IsEnum)
               rpi[j].TypeName = pi[j].ParameterType.FullName;
            else
               rpi[j].TypeName = pi[j].ParameterType.Name;
            System.Attribute[] editors = Attribute.GetCustomAttributes(pi[j], typeof(System.ComponentModel.EditorAttribute));
            if (editors.Length > 0)
            {
               rpi[j].Editors = new string[editors.Length];
               for (int k=0; k < editors.Length; k++)
                  rpi[j].Editors[k] = ((System.ComponentModel.EditorAttribute)editors[k]).EditorTypeName;
            }
         }
         result[i].Arguments = rpi;
      }
      return result;
   }

   public SGDK2.RemotingServices.RemotePropertyInfo[] GetProperties()
   {
      System.Reflection.BindingFlags binder;
      binder = System.Reflection.BindingFlags.Public |
         System.Reflection.BindingFlags.Instance | 
         System.Reflection.BindingFlags.SetProperty | 
         System.Reflection.BindingFlags.GetProperty |
         System.Reflection.BindingFlags.SetField | 
         System.Reflection.BindingFlags.GetField;
      System.Reflection.PropertyInfo[] pi = reflectType.GetProperties(binder);
      System.Reflection.FieldInfo[] fi = reflectType.GetFields(binder);
      SGDK2.RemotingServices.RemotePropertyInfo[] result = new SGDK2.RemotingServices.RemotePropertyInfo[pi.Length + fi.Length];
      for (int i = 0; i < pi.Length; i++)
      {
         result[i].Name = pi[i].Name;
         result[i].Type = pi[i].PropertyType.Name;
         result[i].CanRead = pi[i].CanRead;
         result[i].CanWrite = pi[i].CanWrite;
      }
      for (int i = 0; i < fi.Length; i++)
      {
         int idx = i+pi.Length;
         result[idx].Name = fi[i].Name;
         result[idx].Type = fi[i].FieldType.Name;
         result[idx].CanRead = true;
         result[idx].CanWrite = ((fi[i].Attributes & System.Reflection.FieldAttributes.InitOnly) == 0);
      }
      return result;
   }

   public string[] GetEnumVals()
   {
      string[] result = System.Enum.GetNames(reflectType);
      for(int idx = 0; idx < result.Length; idx++)
         result[idx] = reflectType.FullName.Replace('+','.') + "." + result[idx];
      return result;
   }

   public string[] GetSubclasses()
   {
      System.Collections.ArrayList result = new System.Collections.ArrayList();
      System.Type[] types = reflectType.Assembly.GetTypes();
      for (int i=0; i<types.Length; i++)
      {
         if (types[i].IsSubclassOf(reflectType))
            result.Add(types[i].Name);
      }
      return (string[])result.ToArray(typeof(string));
   }
   #endregion
}