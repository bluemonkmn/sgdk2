/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */
using System;

/// <summary>
/// Maps a tile index to frameset frames based on counter values etc
/// </summary>
public abstract partial class TileBase
{
   protected TileCategoryMembershipBase m_membership;

   protected TileBase(TileCategoryMembershipBase membership)
   {
      m_membership = membership;
   }

   /// <summary>
   /// Returns an array of frameset frames to draw for a tile during a particular frame
   /// </summary>
   public abstract int[] CurrentFrame
   {
      get;
   }

   /// <summary>
   /// Determines if the tile is a member of the specified category.
   /// </summary>
   /// <param name="cat">Enumerated value that designates a tile category</param>
   /// <returns>True if the tile is a member of the specified category</returns>
   public abstract bool IsMember(TileCategoryName cat);
}

/// <summary>
/// Represents the definition for an animated tile in a tileset
/// </summary>
public partial class AnimTile : TileBase
{
   private readonly TileFrame[] m_frames;
   private readonly Counter m_counter;
   private short[] m_frameIndexMap;

   /// <summary>
   /// Creates an animated tile definition
   /// </summary>
   /// <param name="counter">Which counter affects this tile's animation</param>
   /// <param name="frames">Array of frames sorted by chronological sequence</param>
   public AnimTile(Counter counter, params TileFrame[] frames) : base(null)
   {
      if (frames.Length <= 0)
         throw new System.ApplicationException("Use EmptyTile to create empty tiles");
      this.m_frames = frames;
      this.m_counter = counter;
      GenerateFrameIndexMap();
   }

   /// <summary>
   /// Creates an animated tile definition and defines the tile's membership in tiel categories.
   /// </summary>
   /// <param name="counter">Which counter affects this tile's animation</param>
   /// <param name="membership">Defines the categories of which this tile is a member</param>
   /// <param name="frames">Array of frames sorted by chronological sequence</param>
   public AnimTile(Counter counter, TileCategoryMembershipBase membership, params TileFrame[] frames) : base(membership)
   {
      if (frames.Length <= 0)
         throw new System.ApplicationException("Use EmptyTile to create empty tiles");
      this.m_frames = frames;
      this.m_counter = counter;
      GenerateFrameIndexMap();
   }

   private void GenerateFrameIndexMap()
   {
      m_frameIndexMap = new short[m_frames[m_frames.Length - 1].m_nAccumulatedDuration];
      short frameIndex = 0;
      for (int frameValue=0; frameValue<m_frameIndexMap.Length; frameValue++)
      {
         if (m_frames[frameIndex].m_nAccumulatedDuration <= frameValue)
            frameIndex++;
         m_frameIndexMap[frameValue] = frameIndex;
      }
   }

   /// <summary>
   /// Return the index of the current distinct animation frame in the tile's animation sequence.
   /// (NOT a counter value)
   /// </summary>
   /// <remarks>If a tile's first frame has a repeat count of 2, then counter values of 1
   /// and 2 will yield the same animation frame for that tile, and a counter value of 3
   /// will yield the next animation frame. Notice that these were 3 counter values, but only 2
   /// distinct frames. This property will return 1 for counter values of 1 and 2, and 2 for a
   /// counter value of 3 in this example.</remarks>
   public int FrameSequenceIndex
   {
      get
      {
         return m_frameIndexMap[m_counter.CurrentValue % m_frameIndexMap.Length];
      }
   }

   /// <summary>
   /// Return the total number of distinct animation frames in the tile.
   /// </summary>
   /// <remarks>Although a tile may have (for example) 3 distinct animation frames, they may be
   /// spread out over 24 separate counter values be giving each frame a repeat count of 8.
   /// This value will return 3 in this example, not 24.</remarks>
   public int FrameSequenceLength
   {
      get
      {
         return m_frames.Length;
      }
   }

   /// <summary>
   /// Return an array of frame indexes representing the frames from this tile's
   /// frameset that should be drawn for the tile in its current state.
   /// </summary>
   /// <remarks>Often times this will be an array of 1 integer because tiles will
   /// often only have one frameset frame per animation frame. But multiple frameset
   /// frames can be combined into a single tile animation frame by specifying a
   /// repeat count of 0, in which case this will return all the "sub-frames" that are
   /// active in the current frame of this tile.</remarks>
   public override int[] CurrentFrame
   {
      get
      {
         return m_frames[FrameSequenceIndex].subFrames;
      }
   }

   /// <summary>
   /// See <see cref="TileBase.IsMember"/>.
   /// </summary>
   public override bool IsMember(TileCategoryName cat)
   {
      if (m_membership == null) return false;
      if (m_membership is TileCategoryFrameMembership)
         return ((TileCategoryFrameMembership)m_membership)[FrameSequenceIndex, cat];
      else
         return ((TileCategorySimpleMembership)m_membership)[cat];
   }
}

/// <summary>
/// Represents a non-animated composite or single-cell tile
/// </summary>
public partial class SimpleTile : TileBase
{
   private readonly int[] frame;

   /// <summary>
   /// Constructs a tile based on a single frame index from a frameset.
   /// </summary>
   /// <param name="frame">Zero-based index of the frame within the frameset.</param>
   public SimpleTile(int frame) : base(null)
   {
      this.frame = new int[] {frame};
   }

   /// <summary>
   /// Constructs a single-frame tile based on multiple frameset frames.
   /// </summary>
   /// <param name="frame">An array of frame indexes that make of this compisite tile.</param>
   /// <remarks>The frame indexes represent the multiple frames that this tile will always
   /// draw. The first frame is drawn behind, and the last frame is drawn in front.</remarks>
   public SimpleTile(int[] frame) : base(null)
   {
      this.frame = frame;
   }

   /// <summary>
   /// Constructs a tile based on a single frame index from a frameset, and specifies
   /// the tile's membership in tile categories.
   /// </summary>
   /// <param name="frame">Zero-based index of the frame within the frameset.</param>
   /// <param name="membership">Provides an object that determines in which categories
   /// this tile is a member.</param>
   public SimpleTile(int frame, TileCategoryMembershipBase membership) : base(membership)
   {
      this.frame = new int[] {frame};
   }

   /// <summary>
   /// Constructs a single-frame tile based on multiple frameset frames, and specifies
   /// the tile's membership in tile categories.
   /// </summary>
   /// <param name="frame">An array of frame indexes that make of this compisite tile.</param>
   /// <param name="membership">Provides an object that determines in which categories
   /// this tile is a member.</param>
   /// <remarks>The frame indexes represent the multiple frames that this tile will always
   /// draw. The first frame is drawn behind, and the last frame is drawn in front.</remarks>
   public SimpleTile(int[] frame, TileCategoryMembershipBase membership) : base(membership)
   {
      this.frame = frame;
   }

   /// <summary>
   /// Returns an array of frameset frame indexes that represent the current appearance of the tile.
   /// </summary>
   /// <remarks>For a simple single-frame tile this will always be an array of 1.  For a
   /// compound tile, this will contain multiple frame indexes in sequence from back-most to
   /// fore-most on the display.</remarks>
   public override int[] CurrentFrame
   {
      get
      {
         return frame;
      }
   }

   /// <summary>
   /// See <see cref="TileBase.IsMember"/>.
   /// </summary>
   public override bool IsMember(TileCategoryName cat)
   {
      if (m_membership == null) return false;
      return ((TileCategorySimpleMembership)m_membership)[cat];
   }
}

/// <summary>
/// Represents a tile that doesn't draw anything onto the layer
/// </summary>
public partial class EmptyTile : TileBase
{
   /// <summary>
   /// Returns the single empty tile object used for all empty tiles.
   /// </summary>
   public static readonly EmptyTile Value = new EmptyTile();
   private readonly int[] frame;

   private EmptyTile() : base(null)
   {
      frame = new int[] {};
   }

   /// <summary>
   /// Returns an empty array
   /// </summary>
   public override int[] CurrentFrame
   {
      get
      {
         return frame;
      }
   }

   /// <summary>
   /// See <see cref="TileBase.IsMember"/>.
   /// </summary>
   public override bool IsMember(TileCategoryName cat)
   {
      return false;
   }
}

/// <summary>
/// Represents the appearance of a tile during one iteration of the game loop.
/// </summary>
/// <remarks>Composite tiles are tiles that draw multiple images at once for a single
/// iteration of the game loop. Such tiles will have sub-frames representing the multiple
/// images that are drawn overlapped.</remarks>
public partial class TileFrame : IComparable
{
   /// <summary>
   /// Represents counter value, and is used to optimize frame searching
   /// </summary>
   /// <remarks>This is the sum of all the repeat count values on this frame
   /// and before and is used to optimize the translation between a counter
   /// value and an index into the series of distinct tile frames/appearances.</remarks>
   public readonly int m_nAccumulatedDuration;
   /// <summary>
   /// A list of all the frameset frame indexes contained in this tile frame.
   /// </summary>
   /// <remarks>Composite tiles may have multiple frames drawn on top of each other.</remarks>
   public readonly int[] subFrames;

   /// <summary>
   /// Constructs a composite TileFrame given its <see cref="m_nAccumulatedDuration"/> and
   /// <see cref="subFrames"/>.
   /// </summary>
   /// <param name="nAccumulatedDuration"><see cref="m_nAccumulatedDuration"/> value</param>
   /// <param name="subFrames"><see cref="subFrames"/> value</param>
   public TileFrame(int nAccumulatedDuration, int[] subFrames)
   {
      this.m_nAccumulatedDuration = nAccumulatedDuration;
      this.subFrames = subFrames;
   }

   /// <summary>
   /// Constructs a simple (non-composite) TileFrame given its <see cref="m_nAccumulatedDuration"/>
   /// and the single frameset frame index.
   /// </summary>
   /// <param name="nAccumulatedDuration"><see cref="m_nAccumulatedDuration"/> value</param>
   /// <param name="frame">Frameset frame index drawn by this tile frame. This gets converted into
   /// a single-element array of integers in <see cref="subFrames"/>.</param>
   public TileFrame(int nAccumulatedDuration, int frame)
   {
      this.m_nAccumulatedDuration= nAccumulatedDuration;
      this.subFrames = new int[] {frame};
   }

   #region IComparable Members
   /// <summary>
   /// Compares frames based on their <see cref="m_nAccumulatedDuration"/> value
   /// </summary>
   /// <param name="obj">Another TileFrame object, or a counter value</param>
   /// <returns>
   /// <list type="table">
   /// <listheader><term>Return Value</term><item>Description</item></listheader>
   /// <item><term>Less than zero</term><description>This frame comes before <paramref name="obj"/>
   /// in the counter indexing sequence.</description></item>
   /// <item><term>Zero</term><description>This frame should be drawn when the specified counter value is current</description></item>
   /// <item><term>Greater than zero</term><description>This frame comes after
   /// <paramref name="obj"/> in the counter indexing sequence.</description></item>
   /// </list></returns>
   public int CompareTo(object obj)
   {
      if (obj is TileFrame)
         return m_nAccumulatedDuration.CompareTo(((TileFrame)obj).m_nAccumulatedDuration);
      else
         return m_nAccumulatedDuration.CompareTo(obj);
   }
   #endregion
}