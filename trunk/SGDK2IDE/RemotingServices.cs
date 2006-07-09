using System;

namespace SGDK2
{
	/// <summary>
	/// Summary description for RemotingServices.
	/// </summary>
	public class RemotingServices
	{
      public class RemoteMethodComparer : System.Collections.IComparer
      {
         #region IComparer Members
         public int Compare(object x, object y)
         {
            if (x is RemoteMethodInfo)
               return ((RemoteMethodInfo)x).MethodName.CompareTo(((RemoteMethodInfo)y).MethodName);
            else
               throw new ApplicationException("Unknown data row type for comparing");
         }
         #endregion
      }

      [Serializable()]
      public struct RemoteParameterInfo
      {
         public string Name;
         public string TypeName;
         public string[] Editors;
         public bool IsEnum;
         public RemoteParameterInfo(string name, string typeName, bool isEnum)
         {
            this.Name = name;
            this.TypeName = typeName;
            this.IsEnum = isEnum;
            this.Editors = null;
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
         RemotePropertyInfo[] GetProperties();
         string[] GetEnumVals();
         string[] GetSubclasses();
      }

      [Serializable()]
      public struct RemotePropertyInfo
      {
         public string Name;
         public string Type;
         public bool CanRead;
         public bool CanWrite;
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

      public void InsertOperators()
      {
         RemotingServices.RemoteMethodInfo op;
         op.MethodName = "+";
         op.Arguments = new RemotingServices.RemoteParameterInfo[]
                  {
                     new RemotingServices.RemoteParameterInfo("left operand", typeof(int).Name, false),
                     new RemotingServices.RemoteParameterInfo("right operand", typeof(int).Name, false)
                  };
         op.Description = "Return the result of adding two numbers";
         op.ReturnType = typeof(int).Name;
         this["+"] = op;

         op.MethodName = "-";
         op.Description = "Return the result of subtracting the right operand from the left operand";
         this["-"] = op;

         op.MethodName = "<";
         op.Description = "Determine if the left operand is less than the right operand";
         op.ReturnType = typeof(bool).Name;
         this["<"] = op;

         op.MethodName = "<=";
         op.Description = "Determine if the left operand is less than or equal to the right operand";
         this["<="] = op;

         op.MethodName = "==";
         op.Description = "Determine if the left operand is equal to the right operand";
         this["=="] = op;

         op.MethodName = ">=";
         op.Description = "Determine if the left operand is greater than or equal to the right operand";
         this[">="] = op;

         op.MethodName = ">";
         op.Description = "Determine if the left operand is greater than the right operand";
         this[">"] = op;

         op.MethodName = "!=";
         op.Description = "Determine if the left operand is not equal to the right operand";
         this["!="] = op;

         op.MethodName = "=";
         op.Description = "Copy a value to a variable";
         op.Arguments = new RemotingServices.RemoteParameterInfo[]
                  {
                     new RemotingServices.RemoteParameterInfo("Value", typeof(int).Name, false),
                  };
         op.ReturnType = typeof(int).Name;
         this["="] = op;

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
