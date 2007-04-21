using System;

/// <summary>
/// Categorizes / collects sprite instances
/// </summary>
[Serializable()]
public class SpriteCollection : System.Collections.CollectionBase
{
   /// <summary>
   /// Indicates the initial size of this collection, and the size below which
   /// sprites are never removed.
   /// </summary>
   public readonly int staticSize;

   /// <summary>
   /// Defines a maximum sprite collection size to help ensure sprites are not
   /// "leaking" into the system without getting cleaned up.
   /// </summary>
   public const int maxCollectionSize = 100;

   public SpriteCollection(params SpriteBase[] sprites)
   {
      InnerList.AddRange(sprites);
      staticSize = sprites.Length;
   }

   public SpriteBase this[int index]
   {
      get
      {
         return (SpriteBase)InnerList[index];
      }
   }

   public int Add(SpriteBase sprite)
   {
      if (List.Count >= maxCollectionSize)
         throw new ApplicationException("A sprite collection has reached the maximum size of " + maxCollectionSize.ToString() + ". This may be a result of failing to properly clean up dynamic sprites, or it may be the result of a maximum that is too small. To increase it, change the value associated with maxCollectionSize in SpriteCollection.cs");
      return List.Add(sprite);
   }

   protected override void OnRemove( int index, Object value )  
   {
      if (index < staticSize)
         throw new ApplicationException("Attempted to remove from a collection a sprite that was not dynamically added");
   }

   public int IndexOf(SpriteBase sprite)
   {
      return List.IndexOf(sprite);
   }

   public void Remove(SpriteBase sprite)
   {
      List.Remove(sprite);
   }

   /// <summary>
   /// Remove inactive dynamic sprites from the collection
   /// </summary>
   public void Clean()
   {
      for (int index=staticSize; index < List.Count; index++)
      {
         SpriteBase sprite = this[index];
         if (!sprite.isActive)
         {
            RemoveAt(index);
            sprite.RemoveFromCategories();
         }
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
