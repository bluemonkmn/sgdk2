using System;

/// <summary>
/// Maps a tile index to frameset frames based on counter values etc
/// </summary>
public abstract class TileBase
{
   /// <summary>
   /// Returns an array of frameset frames to draw for a tile during a particular frame
   /// </summary>
   public abstract int[] CurrentFrame
   {
      get;
   }
}

/// <summary>
/// Represents an animated tile
/// </summary>
public class AnimTile : TileBase
{
   private readonly TileFrame[] m_frames;
   private readonly Counter m_counter;
   private int m_nLastCounterValue = -1;
   private int m_nTotalDuration;
   private int[] m_lastFrame = null;

   /// <summary>
   /// Creates an animated tile definition
   /// </summary>
   /// <param name="frames">Array of frames sorted by chronological sequence</param>
   /// <param name="counter">Which counter affects this tile's animation</param>
   public AnimTile(Counter counter, params TileFrame[] frames)
   {
      if (frames.Length <= 0)
         throw new System.ApplicationException("Use EmptyTile to create empty tiles");
      this.m_frames = frames;
      this.m_counter = counter;
      m_nTotalDuration = m_frames[m_frames.Length - 1].m_nAccumulatedDuration;
   }

   public override int[] CurrentFrame
   {
      get
      {
         if (m_counter.CurrentValue != m_nLastCounterValue)
         {
            m_nLastCounterValue = m_counter.CurrentValue;
            int nFoundIdx = Array.BinarySearch(m_frames, m_nLastCounterValue % m_nTotalDuration + 1);
            if ((nFoundIdx < 0) && (~nFoundIdx < m_frames.Length))
               m_lastFrame = m_frames[~nFoundIdx].subFrames;
            else if (nFoundIdx >= 0)
               m_lastFrame = m_frames[nFoundIdx].subFrames;
            else
               throw new ApplicationException("Did not expect modded counter value beyond array bounds");
         }
         return m_lastFrame;
      }
   }
}

/// <summary>
/// Represents a non-animated composite or single-cell tile
/// </summary>
public class SimpleTile : TileBase
{
   private readonly int[] frame;

   public SimpleTile(int frame)
   {
      this.frame = new int[] {frame};
   }

   public SimpleTile(int[] frame)
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
}

/// <summary>
/// Represents a tile that doesn't draw anything onto the layer
/// </summary>
public class EmptyTile : TileBase
{
   public static readonly EmptyTile Value = new EmptyTile();
   private readonly int[] frame;

   private EmptyTile()
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