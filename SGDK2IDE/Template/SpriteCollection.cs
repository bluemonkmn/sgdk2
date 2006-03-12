using System;

/// <summary>
/// Categorizes / collects sprite instances
/// </summary>
public class SpriteCollection : System.Collections.ReadOnlyCollectionBase
{
   public SpriteCollection(params SpriteBase[] sprites)
   {
      InnerList.AddRange(sprites);
   }

   public SpriteBase this[int index]
   {
      get
      {
         return (SpriteBase)InnerList[index];
      }
   }
}