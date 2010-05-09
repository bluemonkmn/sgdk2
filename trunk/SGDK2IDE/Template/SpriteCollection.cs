/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */
using System;

/// <summary>
/// Categorizes / collects sprite instances
/// </summary>
/// <remarks>Instances of this class are used both to represent the entire collection
/// of sprites contained within a layer <see cref="LayerBase.m_Sprites"/> as well
/// as sub-collections of these sprites divided into individual categories, exposed by
/// <see cref="LayerBase.m_SpriteCategories"/>. Sprite collections can contain static
/// and dynamic sprites. For more information about this distinction, see remarks in
/// <see cref="staticSize"/>.</remarks>
[Serializable()]
public partial class SpriteCollection : System.Collections.CollectionBase
{
   /// <summary>
   /// Indicates the initial size of this collection, and the size below which
   /// sprites are never removed.
   /// </summary>
   /// <remarks><para>Sprites defined in the map editor are considered "static" and will always
   /// exist, although they may be inactive (invisible and excluded from processing rules).
   /// But sprites can also be added at runtime. These are referred to as "dynamic"
   /// sprites. When a dynamic sprite is added, it is appended to the end of the main
   /// sprite collection for the layer (<see cref="LayerBase.m_Sprites"/>) as well as
   /// all categories designated to include the specified sprite type (which are also
   /// <see cref="SpriteCollection"/> objects). So each sprite collection has a segment
   /// (at the beginning) of static sprites and a segment at the end (beginning at index
   /// determined by staticSize) containing zero or more dynamic sprites.</para>
   /// <para>This value, then, represents the number of static sprites in the collection
   /// (whether it be a layer's main collection or a category within the layer).
   /// It is used to ensure that no static sprite can ever be completely removed from
   /// the collection because these should always be available for activation. And sprites
   /// in the collection beyond this range are the only sprites that need to be considered
   /// for removal after a layer's rules are done executing. Only the range of dynamic
   /// sprites will be checked for inactive sprites. Inactive dynamic sprites are removed
   /// from all the categories in which they were a member.</para>
   /// <seealso cref="SpriteBase.TileAddSprite"/>
   /// <seealso cref="SpriteBase.AddSpriteHere"/>
   /// <seealso cref="PlanBase.AddSpriteAtPlan"/>
   /// <seealso cref="Clean"/></remarks>
   public readonly int staticSize;

   /// <summary>
   /// Defines a maximum sprite collection size to help ensure dynamic sprites are not
   /// "leaking" into the system without getting cleaned up.
   /// </summary>
   /// <remarks>For more information about dynamic sprites, see <see cref="staticSize"/>.</remarks>
   public const int maxCollectionSize = 100;

   /// <summary>
   /// Constructs a sprite collection given a list of all the sprites it should contain.
   /// </summary>
   /// <param name="sprites">Array of sprite instances contained in the collection.</param>
   public SpriteCollection(params SpriteBase[] sprites)
   {
      InnerList.AddRange(sprites);
      staticSize = sprites.Length;
   }

   /// <summary>
   /// Return a sprite instance from the collection given its 0-based index.
   /// </summary>
   public SpriteBase this[int index]
   {
      get
      {
         return (SpriteBase)InnerList[index];
      }
   }

   /// <summary>
   /// Add a dynamic sprite to the end of this sprite collection.
   /// </summary>
   /// <param name="sprite">Sprite to be added</param>
   /// <returns>Index of the new sprite in the collection</returns>
   /// <remarks>For more information about dynamic sprites, see <see cref="staticSize"/>.
   /// This function will fail if the number of sprites in the collection is already equal to
   /// or greater than <see cref="maxCollectionSize"/>.</remarks>
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

   /// <summary>
   /// Determine the index of the specified sprite in the collection.
   /// </summary>
   /// <param name="sprite">Sprite to search for</param>
   /// <returns>Zero-based index of the sprite if found, otherwise -1.</returns>
   public int IndexOf(SpriteBase sprite)
   {
      return List.IndexOf(sprite);
   }

   /// <summary>
   /// Remove the specified sprite from this collection.
   /// </summary>
   /// <param name="sprite">Sprite to be removed. This must be a dynamic sprite</param>
   /// <remarks>For more information about dynamic sprites, see <see cref="staticSize"/>.
   /// An error will occur if an attempt is made to remove a static sprite.</remarks>
   public void Remove(SpriteBase sprite)
   {
      List.Remove(sprite);
   }

   /// <summary>
   /// Remove inactive dynamic sprites from the collection
   /// </summary>
   /// <remarks>For more information about dynamic sprites, see <see cref="staticSize"/>.
   /// This function is called after all rules for a layer are processed
   /// (at the end of <see cref="LayerBase.ProcessSprites"/>).</remarks>
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

/// <summary>
/// Enumerates only the currently active sprites on a layer
/// </summary>
public class ActiveSpriteEnumerator : System.Collections.IEnumerator
{
   private System.Collections.IEnumerator SpriteEnumerator;

   public ActiveSpriteEnumerator(SpriteCollection sprites)
   {
      SpriteEnumerator = sprites.GetEnumerator();
   }

   #region IEnumerator Members

   /// <summary>
   /// Re-start enumerating active sprites.
   /// </summary>
   public void Reset()
   {
      SpriteEnumerator.Reset();
   }

   /// <summary>
   /// The current sprite being enumerated.
   /// </summary>
   public object Current
   {
      get
      {
         return SpriteEnumerator.Current;
      }
   }

   /// <summary>
   /// Move to the next active sprite in the collection.
   /// </summary>
   /// <returns>True if another active sprite exists, or false if there are no more.</returns>
   public bool MoveNext()
   {
      bool result;
      while ((result = SpriteEnumerator.MoveNext()) && (!((SpriteBase)Current).isActive))
         ;
      return result;
   }

   #endregion
}
