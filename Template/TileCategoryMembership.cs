/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */
using System;

/// <summary>
/// Stores information about which categories a tile is in.
/// </summary>
public abstract class TileCategoryMembershipBase
{
}

/// <summary>
/// Represents membership information in which the membership of a tile does not
/// change based on its state.
/// </summary>
public class TileCategorySimpleMembership : TileCategoryMembershipBase
{
   private System.Collections.BitArray m_membership;

   /// <summary>
   /// Constructs membership information given a list of category designators.
   /// </summary>
   /// <param name="membership">Designates the categories in which a tile is a member.</param>
   public TileCategorySimpleMembership(params TileCategoryName[] membership)
   {
      m_membership = new System.Collections.BitArray((int)TileCategoryName.Count);
      foreach(TileCategoryName cat in membership)
         m_membership[(int)cat] = true;
   }

   /// <summary>
   /// Returns true if the tile is a member of the specified category
   /// </summary>
   /// <remarks>This algorithm is simple and executes in O(1) time (it is not
   /// proportional to the number of tiles, number of categories, or number
   /// of memberships).</remarks>
   public bool this[TileCategoryName category]
   {
      get
      {
         return m_membership[(int)category];
      }
   }
}

/// <summary>
/// Represents membership information for a tile whose membership changes based on
/// which frame is active.
/// </summary>
public class TileCategoryFrameMembership : TileCategoryMembershipBase
{
   private System.Collections.BitArray[] m_frames;

   /// <summary>
   /// Constructs membership information for tile given all the information for the membership
   /// </summary>
   /// <param name="nFrameSequenceCount">The number of distinct frames the tile has</param>
   /// <param name="membership">A list of memberships for some or all of the tile's frames.
   /// For each category that contains one or more of the tile's frames, there should be
   /// one element in this list.</param>
   /// <remarks>The <paramref name="membership"/> parameter is only a temporary object used
   /// in the process of constructing this object and is discarded when construction is
   /// complete. This is done to abbreviate the code that constructs these memberships since
   /// the number of distinct categories in which the tile's various frames are members is
   /// usually low, but there may be many distinct frames that are included in each category.
   /// This information is converted to a more optimal format during construction, which allows
   /// the code to determine if a frame is in a category without doing any searching. So the
   /// original membership data is no longer required.</remarks>
   public TileCategoryFrameMembership(int nFrameSequenceCount, params TileFrameMembership[] membership)
   {
      m_frames = new System.Collections.BitArray[nFrameSequenceCount];
      for (int i=0; i<(int)nFrameSequenceCount; i++)
         m_frames[i] = new System.Collections.BitArray((int)TileCategoryName.Count);

      foreach(TileFrameMembership mbr in membership)
         for (int frameIdx = 0; frameIdx < mbr.frames.Length; frameIdx++)
            m_frames[mbr.frames[frameIdx]][(int)mbr.category] = true;
   }

   /// <summary>
   /// Returns true if the specified zero-based frame (not counter value) of the tile
   /// is a member of the specified category.
   /// </summary>
   /// <remarks>nFrameSequenceIndex refers to the index within the list of distinct
   /// frames in the tile's animation sequence, not the counter value that corresponds
   /// to that frame. This operation is simple and will return in O(1) time (it does not
   /// depend on the number of frames, categories or memberships involved).
   /// <see cref="AnimTile.FrameSequenceIndex"/></remarks>
   public bool this[int nFrameSequenceIndex, TileCategoryName category]
   {
      get
      {
         return m_frames[nFrameSequenceIndex][(int)category];
      }
   }
}

/// <summary>
/// Defines an object that can be used to provide information to the construction of a
/// <see cref="TileCategoryFrameMembership"/>.
/// </summary>
/// <remarks>Instances of this class are not retained, but rather converted into a more optimal
/// form during the construction of the <see cref="TileCategoryFrameMembership"/>.</remarks>
public struct TileFrameMembership
{
   /// <summary>
   /// Represents a category designated to contain one or more specific frames from an animated tile.
   /// </summary>
   public TileCategoryName category;
   /// <summary>
   /// Represents a list of frame indexes from a tile that are to be included in a category.
   /// </summary>
   /// <remarks>Note that this refers to frame sequence index as described
   /// in <see cref="AnimTile. FrameSequenceIndex"/>, and not a counter value.</remarks>
   public int[] frames;

   /// <summary>
   /// Constructs membership information given all the parameters
   /// </summary>
   /// <param name="category">Specifies a category that will contain one or more
   /// frames from an animated tile.</param>
   /// <param name="frames">Specifies the indexes of the tile's frames that will be
   /// included in this category. Note that this refers to frame sequence index as described
   /// in <see cref="AnimTile. FrameSequenceIndex"/>, and not a counter value.</param>
   public TileFrameMembership(TileCategoryName category, int[] frames)
   {
      this.frames = frames;
      this.category = category;
   }
}
