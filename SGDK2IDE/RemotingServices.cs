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
         public bool IsEmpty()
         {
            return Name == null;
         }
      }

      [Serializable()]
      public struct RemoteMethodInfo
      {
         public string MethodName;
         public RemoteParameterInfo[] Arguments;
         public string ReturnType;
      }

      public interface IRemoteTypeInfo
      {
         RemoteMethodInfo[] GetMethods();
         string[] GetEnumVals();
      }

      public interface IEvaluatePoint
      {
         UInt64[] evalMatrix();
      }
   }
}
