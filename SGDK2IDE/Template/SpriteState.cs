using System;

/// <summary>
/// Defines how a particular state within a sprite definition can animate.
/// </summary>
public class SpriteState
{
   private readonly TileFrame[] m_frames;
   private short[] m_frameIndexMap;
   private Frameset m_Frameset;
   private int m_nSolidWidth;
   private int m_nSolidHeight;
   private Solidity m_Solidity;
   private System.Drawing.Rectangle m_LocalBounds;

   public SpriteState(int width, int height, Solidity solidity, Display disp, string frameset, System.Drawing.Rectangle localBounds, params TileFrame[] frames)
   {
      m_nSolidWidth = width;
      m_nSolidHeight = height;
      m_Solidity = solidity;
      m_Frameset = Frameset.GetFrameset(frameset, disp);
      m_LocalBounds = localBounds;
      m_frames = frames;
      m_frameIndexMap = new short[m_frames[m_frames.Length - 1].m_nAccumulatedDuration];
      short frameIndex = 0;
      for (int frameValue=0; frameValue<m_frameIndexMap.Length; frameValue++)
      {
         if (m_frames[frameIndex].m_nAccumulatedDuration <= frameValue)
            frameIndex++;
         m_frameIndexMap[frameValue] = frameIndex;
      }
   }

   public Frameset Frameset
   {
      get
      {
         return m_Frameset;
      }
   }

   public int[] GetFrame(int frameIndex)
   {
      return m_frames[m_frameIndexMap[frameIndex % m_frameIndexMap.Length]].subFrames;
   }

   public int SolidWidth
   {
      get
      {
         return m_nSolidWidth;
      }
   }

   public int SolidHeight
   {
      get
      {
         return m_nSolidHeight;
      }
   }

   /// <summary>
   /// Returns a rectangle (relative to the origin of the sprite) that bounds
   /// all the graphics in this state.
   /// </summary>
   public System.Drawing.Rectangle LocalBounds
   {
      get
      {
         return m_LocalBounds;
      }
   }
}