/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
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
      reflectType = typeof(System.Drawing.Color).Assembly.GetType(typeName, false);
      if (null != reflectType)
         return;
      throw new System.ApplicationException("Failed to load type " + typeName);
   }
   #region IRemoteTypeInfo Members

   public SGDK2.RemotingServices.RemoteGlobalAccessorInfo[] GetGlobalProvidersOfSelf()
   {
      System.Reflection.BindingFlags binder;
      binder = System.Reflection.BindingFlags.Public |
         System.Reflection.BindingFlags.Static | 
         System.Reflection.BindingFlags.GetProperty |
         System.Reflection.BindingFlags.GetField;

      System.Collections.ArrayList result = new System.Collections.ArrayList();

      foreach (System.Type type in reflectType.Assembly.GetTypes())
      {
         System.Reflection.PropertyInfo[] pi = type.GetProperties(binder);
         System.Reflection.FieldInfo[] fi = type.GetFields(binder);
         for (int i = 0; i < pi.Length; i++)
         {
            if (reflectType.IsAssignableFrom(pi[i].PropertyType))
               result.Add(new SGDK2.RemotingServices.RemoteGlobalAccessorInfo(
                  new SGDK2.RemotingServices.RemoteTypeName(type), pi[i].Name));
         }
         for (int i = 0; i < fi.Length; i++)
         {
            if (reflectType.IsAssignableFrom(fi[i].FieldType))
               result.Add(new SGDK2.RemotingServices.RemoteGlobalAccessorInfo(
                  new SGDK2.RemotingServices.RemoteTypeName(type), fi[i].Name));
         }
      }

      return (SGDK2.RemotingServices.RemoteGlobalAccessorInfo[])result.ToArray(typeof(SGDK2.RemotingServices.RemoteGlobalAccessorInfo));
   }

   public SGDK2.RemotingServices.RemoteMethodInfo[] GetGlobalFunctions()
   {
      System.Collections.ArrayList result = new System.Collections.ArrayList();

      foreach (System.Type type in reflectType.Assembly.GetTypes())
      {
         System.Reflection.MethodInfo[] mi = type.GetMethods(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Static | 
            System.Reflection.BindingFlags.InvokeMethod);
         for (int i = 0; i < mi.Length; i++)
         {
            System.ComponentModel.DescriptionAttribute da = System.Attribute.GetCustomAttribute(mi[i], typeof(System.ComponentModel.DescriptionAttribute), false) as System.ComponentModel.DescriptionAttribute;
            if ((da == null) || (da.Description == null))
               continue;

            SGDK2.RemotingServices.RemoteMethodInfo current;
            current.MethodName = type.FullName + "." + mi[i].Name;
            current.Description = da.Description;
            current.ReturnType = new SGDK2.RemotingServices.RemoteTypeName(mi[i].ReturnType);
            System.Reflection.ParameterInfo[] pi = mi[i].GetParameters();
            SGDK2.RemotingServices.RemoteParameterInfo[] rpi = new SGDK2.RemotingServices.RemoteParameterInfo[pi.Length];
            for (int j = 0; j < pi.Length; j++)
            {
               rpi[j].Name = pi[j].Name;
               rpi[j].IsEnum = pi[j].ParameterType.IsEnum;
               rpi[j].Type = new SGDK2.RemotingServices.RemoteTypeName(pi[j].ParameterType);
               System.Attribute[] editors = Attribute.GetCustomAttributes(pi[j], typeof(System.ComponentModel.EditorAttribute));
               if (editors.Length > 0)
               {
                  rpi[j].Editors = new string[editors.Length];
                  for (int k=0; k < editors.Length; k++)
                     rpi[j].Editors[k] = ((System.ComponentModel.EditorAttribute)editors[k]).EditorTypeName;
               }
            }
            current.Arguments = rpi;
            result.Add(current);
         }
      }

      return (SGDK2.RemotingServices.RemoteMethodInfo[])result.ToArray(typeof(SGDK2.RemotingServices.RemoteMethodInfo));
   }

   public SGDK2.RemotingServices.RemoteMethodInfo[] GetMethods()
   {
      System.Reflection.MethodInfo[] mi = reflectType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod);
      SGDK2.RemotingServices.RemoteMethodInfo[] result = new SGDK2.RemotingServices.RemoteMethodInfo[mi.Length];
      for (int i = 0; i < mi.Length; i++)
      {
         result[i].MethodName = mi[i].Name;
         result[i].ReturnType = new SGDK2.RemotingServices.RemoteTypeName(mi[i].ReturnType);
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
            rpi[j].Type = new SGDK2.RemotingServices.RemoteTypeName(pi[j].ParameterType);
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
         System.Reflection.BindingFlags.Static | 
         System.Reflection.BindingFlags.SetProperty | 
         System.Reflection.BindingFlags.GetProperty |
         System.Reflection.BindingFlags.SetField | 
         System.Reflection.BindingFlags.GetField;
      System.Reflection.PropertyInfo[] pi = reflectType.GetProperties(binder);
      System.Reflection.FieldInfo[] fi = reflectType.GetFields(binder);
      SGDK2.RemotingServices.RemotePropertyInfo[] result = new SGDK2.RemotingServices.RemotePropertyInfo[pi.Length + fi.Length];
      bool browsable;
      for (int i = 0; i < pi.Length; i++)
      {
         result[i].Name = pi[i].Name;
         result[i].Type = new SGDK2.RemotingServices.RemoteTypeName(pi[i].PropertyType);
         System.ComponentModel.BrowsableAttribute ba = System.Attribute.GetCustomAttribute(pi[i], typeof(System.ComponentModel.BrowsableAttribute)) as System.ComponentModel.BrowsableAttribute;
         if (ba == null)
            browsable = true;
         else
            browsable = ba.Browsable;
         result[i].Flags = (pi[i].CanRead ? SGDK2.RemotingServices.MemberFlags.CanRead:0) |
            (pi[i].CanWrite ? SGDK2.RemotingServices.MemberFlags.CanWrite:0) |
            (browsable ? SGDK2.RemotingServices.MemberFlags.Browsable:0);
         if (pi[i].IsDefined(typeof(System.ComponentModel.DescriptionAttribute), true))
            result[i].Description = ((System.ComponentModel.DescriptionAttribute)System.Attribute.GetCustomAttribute(pi[i], typeof(System.ComponentModel.DescriptionAttribute))).Description;
         else
            result[i].Description = String.Empty;
      }
      for (int i = 0; i < fi.Length; i++)
      {
         int idx = i+pi.Length;
         result[idx].Name = fi[i].Name;
         result[idx].Type = new SGDK2.RemotingServices.RemoteTypeName(fi[i].FieldType);
         System.ComponentModel.BrowsableAttribute ba = System.Attribute.GetCustomAttribute(fi[i], typeof(System.ComponentModel.BrowsableAttribute)) as System.ComponentModel.BrowsableAttribute;
         if (ba == null)
            browsable = true;
         else
            browsable = ba.Browsable;
         result[idx].Flags = SGDK2.RemotingServices.MemberFlags.CanRead |
            (((fi[i].Attributes & System.Reflection.FieldAttributes.InitOnly) == 0)?SGDK2.RemotingServices.MemberFlags.CanWrite:0) |
            ((0 != (int)(fi[i].Attributes & System.Reflection.FieldAttributes.Static))?SGDK2.RemotingServices.MemberFlags.Static:0) |
            (browsable ? SGDK2.RemotingServices.MemberFlags.Browsable:0);
         if (fi[i].IsDefined(typeof(System.ComponentModel.DescriptionAttribute), true))
            result[idx].Description = ((System.ComponentModel.DescriptionAttribute)System.Attribute.GetCustomAttribute(fi[i], typeof(System.ComponentModel.DescriptionAttribute))).Description;
         else
            result[idx].Description = String.Empty;
      }
      return result;
   }

   public bool IsEnum
   {
      get
      {
         return reflectType.IsEnum;
      }
   }

   public bool IsFlags
   {
      get
      {
         return reflectType.GetCustomAttributes(typeof(System.FlagsAttribute), false).Length > 0;
      }
   }

   public string[] GetEnumVals()
   {
      string[] result = System.Enum.GetNames(reflectType);
      for(int idx = 0; idx < result.Length; idx++)
         result[idx] = reflectType.FullName.Replace('+','.') + "." + result[idx];
      return result;
   }

   public SGDK2.RemotingServices.RemoteTypeName[] GetSubclasses()
   {
      System.Collections.ArrayList result = new System.Collections.ArrayList();
      System.Type[] types = reflectType.Assembly.GetTypes();
      for (int i=0; i<types.Length; i++)
      {
         if (types[i].IsSubclassOf(reflectType))
            result.Add(new SGDK2.RemotingServices.RemoteTypeName(types[i]));
      }
      return (SGDK2.RemotingServices.RemoteTypeName[])result.ToArray(typeof(SGDK2.RemotingServices.RemoteTypeName));
   }
   #endregion
}