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
      reflectType = System.Reflection.Assembly.GetExecutingAssembly().GetType(typeName, true);
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
         }
         result[i].Arguments = rpi;
      }
      return result;
   }

   public string[] GetEnumVals()
   {
      return System.Enum.GetNames(reflectType);
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