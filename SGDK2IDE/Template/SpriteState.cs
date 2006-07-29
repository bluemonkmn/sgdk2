using System;

/// <summary>
/// This specialization of TileFrame is used specifically for sprite frames
/// with collision masks (when alpha is non-zero).
/// </summary>
public class SpriteFrame : TileFrame
{
   private CollisionMask m_Mask;

   public SpriteFrame(System.Drawing.Rectangle localBounds, Frameset frameset, int nAccumulatedDuration, int[] subFrames, byte[] alphas) : base(nAccumulatedDuration, subFrames)
   {
      m_Mask = new CollisionMask(localBounds, frameset, subFrames, alphas);
   }

   public SpriteFrame(System.Drawing.Rectangle localBounds, Frameset frameset, int nAccumulatedDuration, int frame, byte alpha) : base(nAccumulatedDuration, frame)
   {
      m_Mask = new CollisionMask(localBounds, frameset, new int[] {frame}, new byte[] {alpha});
   }

   public CollisionMask Mask
   {
      get
      {
         return m_Mask;
      }
   }
}

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

   public SpriteState(int width, int height, Solidity solidity, Display disp, Frameset frameset, System.Drawing.Rectangle localBounds, params TileFrame[] frames)
   {
      m_nSolidWidth = width;
      m_nSolidHeight = height;
      m_Solidity = solidity;
      m_Frameset = frameset;
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

   public CollisionMask GetMask(int frameIndex)
   {
      SpriteFrame frame = m_frames[m_frameIndexMap[frameIndex % m_frameIndexMap.Length]] as SpriteFrame;
      if (frame == null)
         return null;
      return frame.Mask;
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