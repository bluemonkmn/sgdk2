using System;

/// <summary>
/// Maps a tile index to frameset frames based on counter values etc
/// </summary>
public abstract class TileBase
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

   public abstract bool IsMember(TileCategoryName cat);
}

/// <summary>
/// Represents an animated tile
/// </summary>
public class AnimTile : TileBase
{
   private readonly TileFrame[] m_frames;
   private readonly Counter m_counter;
   private short[] m_frameIndexMap;

   /// <summary>
   /// Creates an animated tile definition
   /// </summary>
   /// <param name="frames">Array of frames sorted by chronological sequence</param>
   /// <param name="counter">Which counter affects this tile's animation</param>
   public AnimTile(Counter counter, params TileFrame[] frames) : base(null)
   {
      if (frames.Length <= 0)
         throw new System.ApplicationException("Use EmptyTile to create empty tiles");
      this.m_frames = frames;
      this.m_counter = counter;
      GenerateFrameIndexMap();
   }

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

   public int FrameSequenceIndex
   {
      get
      {
         return m_frameIndexMap[m_counter.CurrentValue % m_frameIndexMap.Length];
      }
   }

   public int FrameSequenceLength
   {
      get
      {
         return m_frames.Length;
      }
   }

   public override int[] CurrentFrame
   {
      get
      {
         return m_frames[FrameSequenceIndex].subFrames;
      }
   }

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
public class SimpleTile : TileBase
{
   private readonly int[] frame;

   public SimpleTile(int frame) : base(null)
   {
      this.frame = new int[] {frame};
   }

   public SimpleTile(int[] frame) : base(null)
   {
      this.frame = frame;
   }

   public SimpleTile(int frame, TileCategoryMembershipBase membership) : base(membership)
   {
      this.frame = new int[] {frame};
   }

   public SimpleTile(int[] frame, TileCategoryMembershipBase membership) : base(membership)
   {
      this.frame = frame;
   }

   public override int[] CurrentFrame
   {
      get
      {
         return frame;
      }
   }

   public override bool IsMember(TileCategoryName cat)
   {
      if (m_membership == null) return false;
      return ((TileCategorySimpleMembership)m_membership)[cat];
   }
}

/// <summary>
/// Represents a tile that doesn't draw anything onto the layer
/// </summary>
public class EmptyTile : TileBase
{
   public static readonly EmptyTile Value = new EmptyTile();
   private readonly int[] frame;

   private EmptyTile() : base(null)
   {
      frame = new int[] {};
   }

   public override int[] CurrentFrame
   {
      get
      {
         return frame;
      }
   }

   public override bool IsMember(TileCategoryName cat)
   {
      return false;
   }
}

public class TileFrame : IComparable
{
   /// <summary>
   /// Represents counter value, and is used to optimize frame searching
   /// </summary>
   public readonly int m_nAccumulatedDuration;
   /// <summary>
   /// Composite tiles may have multiple frames drawn on top of each other
   /// </summary>
   public readonly int[] subFrames;

   public TileFrame(int nAccumulatedDuration, int[] subFrames)
   {
      this.m_nAccumulatedDuration = nAccumulatedDuration;
      this.subFrames = subFrames;
   }

   public TileFrame(int nAccumulatedDuration, int frame)
   {
      this.m_nAccumulatedDuration= nAccumulatedDuration;
      this.subFrames = new int[] {frame};
   }

   #region IComparable Members

   public int CompareTo(object obj)
   {
      if (obj is TileFrame)
         return m_nAccumulatedDuration.CompareTo(((TileFrame)obj).m_nAccumulatedDuration);
      else
         return m_nAccumulatedDuration.CompareTo(obj);
   }
   #endregion
}