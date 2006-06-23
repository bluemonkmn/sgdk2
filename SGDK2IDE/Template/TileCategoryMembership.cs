using System;

/// <summary>
/// Stores information about which categories a tile is in.
/// </summary>
public abstract class TileCategoryMembershipBase
{
}

public class TileCategorySimpleMembership : TileCategoryMembershipBase
{
   private System.Collections.BitArray m_membership;

   public TileCategorySimpleMembership(params TileCategoryName[] membership)
   {
      m_membership = new System.Collections.BitArray((int)TileCategoryName.Count);
      foreach(TileCategoryName cat in membership)
         m_membership[(int)cat] = true;
   }

   public bool this[TileCategoryName category]
   {
      get
      {
         return m_membership[(int)category];
      }
   }
}

public class TileCategoryFrameMembership : TileCategoryMembershipBase
{
   private System.Collections.BitArray[] m_frames;
   public TileCategoryFrameMembership(int nFrameSequenceCount, params TileFrameMembership[] membership)
   {
      m_frames = new System.Collections.BitArray[nFrameSequenceCount];
      for (int i=0; i<(int)nFrameSequenceCount; i++)
         m_frames[i] = new System.Collections.BitArray((int)TileCategoryName.Count);

      foreach(TileFrameMembership mbr in membership)
         for (int frameIdx = 0; frameIdx < mbr.frames.Length; frameIdx++)
            m_frames[mbr.frames[frameIdx]][(int)mbr.category] = true;
   }

   public bool this[int nFrameSequenceIndex, TileCategoryName category]
   {
      get
      {
         return m_frames[nFrameSequenceIndex][(int)category];
      }
   }
}

public struct TileFrameMembership
{
   public TileCategoryName category;
   public int[] frames;

   public TileFrameMembership(TileCategoryName category, int[] frames)
   {
      this.frames = frames;
      this.category = category;
   }
}
