/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */
using System;

/// <summary>
/// This specialization of TileFrame is used specifically for sprite frames
/// with collision masks (when mask alpha level is non-zero).
/// </summary>
/// <remarks>Some sprites are simple enough that they can be treated very similar to tiles, and
/// will simply use <see cref="TileFrame"/> directly. Others will use this class.</remarks>
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

   /// <summary>
   /// Returns the mask for this sprite frame.
   /// </summary>
   public CollisionMask Mask
   {
      get
      {
         return m_Mask;
      }
   }
}

/// <summary>
/// Represents one state within a sprite definition, primarily defining how a it can animate.
/// </summary>
public class SpriteState
{
   private readonly TileFrame[] m_frames;
   private short[] m_frameIndexMap;
   private Frameset m_Frameset;
   private int m_nSolidWidth;
   private int m_nSolidHeight;
   private System.Drawing.Rectangle m_LocalBounds;

   /// <summary>
   /// Constructs a new state given all the information for the state.
   /// </summary>
   /// <param name="width">Solid width of this state. Each state within a sprite can have its own solid width.</param>
   /// <param name="height">Solid height of this state. Each state within a sprite can have its own solid height.</param>
   /// <param name="frameset"><see cref="Frameset"/> containing all the graphics for this state.</param>
   /// <param name="localBounds">Rectangle that visually enclompases the graphics of all the frames of this state.
   /// This is used to determine when the sprite is fully or partially visible on the layer and needs to be drawn.</param>
   /// <param name="frames">A sequential list of all frames from <paramref name="frameset"/> that are included in this state.</param>
   public SpriteState(int width, int height, Frameset frameset, System.Drawing.Rectangle localBounds, params TileFrame[] frames)
   {
      m_nSolidWidth = width;
      m_nSolidHeight = height;
      m_Frameset = frameset;
      m_LocalBounds = localBounds;
      m_frames = frames;
      if (m_frames.Length > 0)
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
   }

   /// <summary>
   /// Returns the frameset on which all graphics in this state are based.
   /// </summary>
   public Frameset Frameset
   {
      get
      {
         return m_Frameset;
      }
   }

   /// <summary>
   /// Return a list of all frame indexes that represent a particular animation frame of a sprite.
   /// </summary>
   /// <param name="frameIndex">Frame counter value.</param>
   /// <returns>Array if integers that represent indexes into <see cref="Frameset"/> for the
   /// frames that should be drawn for the specified frame.</returns>
   /// <remarks>Often times this will return an array of one integer because a sprite will only
   /// display one frame at a time. Compound frames defined by using a repeat count of "0" on some
   /// frames, however, will cause multiple frameset frames to be comined into a single sprite
   /// frame. So a single <paramref name="frameIndex"/> value can yield multiple frameset frame
   /// indexes to be drawn at once. But also note that multiple <paramref name="frameIndex"/>
   /// values can yield the same frames because a frame with a repeat count greater than 1 will
   /// cause the sprite to remain on the same frame for multiple iterations.</remarks>
   public int[] GetFrame(int frameIndex)
   {
      return m_frames[m_frameIndexMap[frameIndex % m_frameIndexMap.Length]].subFrames;
   }

   /// <summary>
   /// Get the collision mask for a specified sprite frame.
   /// </summary>
   /// <param name="frameIndex">Frame counter value.</param>
   /// <returns>Collision mask for the specified frame of the sprite (simply or compound)
   /// if applicable, otherwise a null reference.
   /// See <see cref="GetFrame"/> for more information about <paramref name="frameIndex"/> and
   /// frame counters.</returns>
   public CollisionMask GetMask(int frameIndex)
   {
      SpriteFrame frame = m_frames[m_frameIndexMap[frameIndex % m_frameIndexMap.Length]] as SpriteFrame;
      if (frame == null)
         return null;
      return frame.Mask;
   }

   /// <summary>
   /// Retrieve the solid width of this state.
   /// </summary>
   /// <remarks>Each sprite state can have its own solid width. For example a leaping sprite might
   /// be wider than a standing sprite, and might react to solidity on the map as such.</remarks>
   public int SolidWidth
   {
      get
      {
         return m_nSolidWidth;
      }
   }

   /// <summary>
   /// Retrieve the solid height of this state.
   /// </summary>
   /// <remarks>Each sprite state can have its own solid height. For example a crouching sprite might
   /// be shorter than a standing sprite, and might react to solidity on the map as such.</remarks>
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
   /// <remarks>This is used to determine when part or all of the sprite is visible on the current
   /// view of the sprite's layer, and consequently, whether it should be drawn.</remarks>
   public System.Drawing.Rectangle LocalBounds
   {
      get
      {
         return m_LocalBounds;
      }
   }
}