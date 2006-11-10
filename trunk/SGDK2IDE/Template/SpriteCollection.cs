using System;

/// <summary>
/// Categorizes / collects sprite instances
/// </summary>
[Serializable()]
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

public class ActiveSpriteEnumerator : System.Collections.IEnumerator
{
   private System.Collections.IEnumerator SpriteEnumerator;

   public ActiveSpriteEnumerator(SpriteCollection sprites)
   {
      SpriteEnumerator = sprites.GetEnumerator();
   }

   #region IEnumerator Members

   public void Reset()
   {
      SpriteEnumerator.Reset();
   }

   public object Current
   {
      get
      {
         return SpriteEnumerator.Current;
      }
   }

   public bool MoveNext()
   {
      bool result;
      while ((result = SpriteEnumerator.MoveNext()) && (!((SpriteBase)Current).isActive))
         ;
      return result;
   }

   #endregion
}
