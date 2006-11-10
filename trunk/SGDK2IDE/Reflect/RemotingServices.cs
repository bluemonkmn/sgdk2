using System;

namespace SGDK2
{
	/// <summary>
	/// Defines interfaces for passing type information accross application domains
	/// without loading the remote assembly into the main domain.
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
         public RemoteTypeName Type;
         public string[] Editors;
         public bool IsEnum;
         public RemoteParameterInfo(string name, RemoteTypeName type, bool isEnum)
         {
            this.Name = name;
            this.Type = type;
            this.IsEnum = isEnum;
            this.Editors = null;
         }
         public static RemoteParameterInfo Empty = new RemoteParameterInfo(null, RemoteTypeName.Empty, false);
         public static RemoteParameterInfo Unknown = new RemoteParameterInfo(null, RemoteTypeName.Unknown , false);
         public bool IsEmpty()
         {
            return (Name == null) && (Type.FullName == RemoteTypeName.Empty.FullName);
         }
         public bool IsUnknown()
         {
            return String.Compare(Type.FullName, RemoteTypeName.Unknown.FullName) == 0;
         }
      }

      [Serializable()]
      public struct RemoteMethodInfo
      {
         public string MethodName;
         public RemoteParameterInfo[] Arguments;
         public RemoteTypeName ReturnType;
         public string Description;
      }

      [Serializable()]
      public struct RemoteGlobalAccessorInfo
      {
         public RemoteTypeName Type;
         public string MemberName;
         public RemoteGlobalAccessorInfo(RemoteTypeName Type, string MemberName)
         {
            this.Type = Type;
            this.MemberName = MemberName;
         }
      }

      [Serializable()]
      public struct RemoteTypeName
      {
         public string Name;
         public string FullName;
         public RemoteTypeName(string fullName)
         {
            this.FullName = fullName;
            if (FullName == null)
            {
               Name = null;
               return;
            }
            int dotPos = fullName.LastIndexOf('.');
            if (dotPos >= 0)
               this.Name = fullName.Substring(dotPos + 1);
            else
               this.Name = fullName;
         }
         public RemoteTypeName(Type type)
         {
            this.FullName = type.FullName;
            this.Name = type.Name;
         }
         public static RemoteTypeName Unknown
         {
            get
            {
               return new RemoteTypeName("unknown");
            }
         }
         public static RemoteTypeName Empty
         {
            get
            {
               return new RemoteTypeName((string)null);
            }
         }
      }

      public interface IRemoteTypeInfo
      {
         RemoteMethodInfo[] GetMethods();
         RemotePropertyInfo[] GetProperties();
         RemoteGlobalAccessorInfo[] GetGlobalProvidersOfSelf();
         RemoteMethodInfo[] GetGlobalFunctions();
         bool IsEnum
         {
            get;
         }
         string[] GetEnumVals();
         RemoteTypeName[] GetSubclasses();
      }

      [Serializable()]
      public struct RemotePropertyInfo
      {
         public string Name;
         public RemoteTypeName Type;
         public bool CanRead;
         public bool CanWrite;
         public string Description;
         public bool Static;
      }
   }

   public class RuleTable : System.Collections.DictionaryBase
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
         RemotingServices.RemoteTypeName intName = new RemotingServices.RemoteTypeName(typeof(int));
         op.Arguments = new RemotingServices.RemoteParameterInfo[]
                  {
                     new RemotingServices.RemoteParameterInfo("left operand", intName, false),
                     new RemotingServices.RemoteParameterInfo("right operand", intName, false)
                  };
         op.Description = "Return the result of adding two numbers";
         op.ReturnType = intName;
         this["+"] = op;

         op.MethodName = "-";
         op.Description = "Return the result of subtracting the right operand from the left operand";
         this["-"] = op;

         op.MethodName = "<";
         op.Description = "Determine if the left operand is less than the right operand";
         op.ReturnType = new SGDK2.RemotingServices.RemoteTypeName(typeof(bool));
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
                     new RemotingServices.RemoteParameterInfo("Value", intName, false),
                  };
         op.ReturnType = intName;
         this["="] = op;
      }
   }

   public class EnumTable : System.Collections.DictionaryBase
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
