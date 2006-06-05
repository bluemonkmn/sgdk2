using System;

namespace SGDK2
{
	/// <summary>
	/// Summary description for RemotingServices.
	/// </summary>
	public class RemotingServices
	{
      [Serializable()]
      public struct RemoteParameterInfo
      {
         public string Name;
         public string TypeName;
         public bool IsEnum;
         public RemoteParameterInfo(string name, string typeName, bool isEnum)
         {
            this.Name = name;
            this.TypeName = typeName;
            this.IsEnum = isEnum;
         }
         public static RemoteParameterInfo Empty = new RemoteParameterInfo(null, null, false);
         public static RemoteParameterInfo Unknown = new RemoteParameterInfo(null, "unknown", false);
         public bool IsEmpty()
         {
            return (Name == null) && (TypeName == null);
         }
         public bool IsUnknown()
         {
            return String.Compare(TypeName, Unknown.TypeName) == 0;
         }
      }

      [Serializable()]
      public struct RemoteMethodInfo
      {
         public string MethodName;
         public RemoteParameterInfo[] Arguments;
         public string ReturnType;
         public string Description;
      }

      public interface IRemoteTypeInfo
      {
         RemoteMethodInfo[] GetMethods();
         string[] GetEnumVals();
         string[] GetSubclasses();
      }
   }

   class RuleTable : System.Collections.DictionaryBase
   {
      public RemotingServices.RemoteMethodInfo this[string name]
      {
         get
         {
            return (RemotingServices.RemoteMethodInfo)InnerHashtable[name];
         }
         set
         {
            InnerHashtable[name] = value;
         }
      }

      public System.Collections.ICollection Rules
      {
         get
         {
            return InnerHashtable.Values;
         }
      }

      public bool Contains(string name)
      {
         return InnerHashtable.ContainsKey(name);
      }
   }

   class EnumTable : System.Collections.DictionaryBase
   {
      public string[] this[string name]
      {
         get
         {
            return InnerHashtable[name] as string[];
         }
         set
         {
            InnerHashtable[name] = value;
         }
      }

      public bool Contains(string name)
      {
         return InnerHashtable.Contains(name);
      }
   }
}
