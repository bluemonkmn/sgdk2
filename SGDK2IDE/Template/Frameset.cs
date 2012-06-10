/// <summary>Provides objects that encapsulate the functionality of the framesets defined at design time.</summary>
/// <remarks>The class is entirely generated based on the framesets defined in the project. Static members exist to create/access instances of each frameset, and each instance represents one specific frameset. Only one instance (maximum) of each frameset will ever exist per display.</remarks>
[System.Serializable()]
public partial class Frameset : System.Runtime.Serialization.ISerializable
{

   private Frame[] m_arFrames;

   private Display m_Display;

   private string Name;

   private static System.Collections.Hashtable m_CachedFramesets = new System.Collections.Hashtable();

   private Frameset(string Name, Display disp)
   {
      this.Name = Name;
      using (System.IO.Stream framesetStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Framesets." + Name + ".bin"))
      {
         using (System.IO.BinaryReader framesetReader = new System.IO.BinaryReader(framesetStream))
         {
            int sheetCount = framesetReader.ReadInt32();
            string[] sheetNames = new string[sheetCount];
            for (int sheetIndex = 0; sheetIndex < sheetCount; sheetIndex++)
            {
               sheetNames[sheetIndex] = framesetReader.ReadString();
            }
            int frameCount = framesetReader.ReadInt32();
            this.m_arFrames = new Frame[frameCount];
            for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
            {
               switch (framesetReader.ReadByte())
               {
                  case 0:
                     m_arFrames[frameIndex] = new Frame(disp.GetTextureRef(sheetNames[framesetReader.ReadInt16()]),
                        framesetReader.ReadInt16(), new System.Drawing.Rectangle(
                           framesetReader.ReadInt32(), framesetReader.ReadInt32(), framesetReader.ReadInt16(), framesetReader.ReadInt16()));
                     break;
                  case 1:
                     m_arFrames[frameIndex] = new Frame(disp.GetTextureRef(sheetNames[framesetReader.ReadInt16()]),
                        framesetReader.ReadInt16(), new System.Drawing.Rectangle(
                           framesetReader.ReadInt32(), framesetReader.ReadInt32(), framesetReader.ReadInt16(), framesetReader.ReadInt16()),
                           framesetReader.ReadInt32());
                     break;
                  case 2:
                     m_arFrames[frameIndex] = new Frame(disp.GetTextureRef(sheetNames[framesetReader.ReadInt16()]),
                        framesetReader.ReadInt16(), new System.Drawing.PointF[] {
                           new System.Drawing.PointF(framesetReader.ReadSingle(), framesetReader.ReadSingle()), 
                           new System.Drawing.PointF(framesetReader.ReadSingle(), framesetReader.ReadSingle()), 
                           new System.Drawing.PointF(framesetReader.ReadSingle(), framesetReader.ReadSingle()), 
                           new System.Drawing.PointF(framesetReader.ReadSingle(), framesetReader.ReadSingle())}, new System.Drawing.Rectangle(
                           framesetReader.ReadInt32(), framesetReader.ReadInt32(), framesetReader.ReadInt16(), framesetReader.ReadInt16()));
                     break;
                  case 3:
                     m_arFrames[frameIndex] = new Frame(disp.GetTextureRef(sheetNames[framesetReader.ReadInt16()]),
                        framesetReader.ReadInt16(), new System.Drawing.PointF[] {
                           new System.Drawing.PointF(framesetReader.ReadSingle(), framesetReader.ReadSingle()), 
                           new System.Drawing.PointF(framesetReader.ReadSingle(), framesetReader.ReadSingle()), 
                           new System.Drawing.PointF(framesetReader.ReadSingle(), framesetReader.ReadSingle()), 
                           new System.Drawing.PointF(framesetReader.ReadSingle(), framesetReader.ReadSingle())}, new System.Drawing.Rectangle(
                           framesetReader.ReadInt32(), framesetReader.ReadInt32(), framesetReader.ReadInt16(), framesetReader.ReadInt16()),
                           framesetReader.ReadInt32());
                     break;
               }
            }
         }
      }
      this.m_Display = disp;
   }

   /// <summary>Return the <see cref="Frame"/> object defining the frame at the specified 0-based index within this frameset</summary>
   public Frame this[int index]
   {
      get
      {
         return this.m_arFrames[(index % this.m_arFrames.Length)];
      }
   }

   /// <summary>Return the number of frames in this frameset.</summary>
   public int Count
   {
      get
      {
         return this.m_arFrames.Length;
      }
   }

   public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
   {
      info.SetType(typeof(FramesetRef));
      info.AddValue("FramesetName", this.Name);
   }

   /// <summary>Retrieves an object representing the frameset by name</summary>
   /// <param name="Name">Specifies the name of the frameset as defined in the project at design time.</param>
   /// <param name="disp">Specifies the display to which the frameset is linked. This is used to construct the hardware objects that support the frameset if the graphics for the frameset have not been loaded into the hardware</param>
   /// <returns>An instance of the <see cref="Frameset"/> class.</returns>
   /// <remarks>If the specified frameset has already been constructed for the specified display, it will be returned from the cache, otherwise a new instance will be constructed and added to the cache before returning.</remarks>
   public static Frameset GetFrameset(string Name, Display disp)
   {
      Frameset result = ((Frameset)(Frameset.m_CachedFramesets[Name]));
      if ((result == null))
      {
         result = new Frameset(Name, disp);
         Frameset.m_CachedFramesets[Name] = result;
      }
      return result;
   }
}
/// <summary>Provides serialization services for <see cref="Frameset" /> to allow objects that reference framesets to be saved without saving everything that is referenced by the frameset.</summary>
[System.Serializable()]
public partial class FramesetRef : System.Runtime.Serialization.IObjectReference, System.Runtime.Serialization.ISerializable
{

   private string m_FramesetName;

   private FramesetRef(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
   {
      this.m_FramesetName = info.GetString("FramesetName");
   }

   public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
   {
      throw new System.NotImplementedException("Unexpected serialization call");
   }

   public object GetRealObject(System.Runtime.Serialization.StreamingContext context)
   {
      return Frameset.GetFrameset(this.m_FramesetName, Project.GameWindow.GameDisplay);
   }
}
