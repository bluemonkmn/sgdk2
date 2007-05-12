/*
 * Created using Scrolling Game Development Kit 2.0
 * See Project.cs for copyright/licensing details
 */
using System;

/// <summary>
/// Associates a category of tiles with a solidity shape to which sprites can react.
/// </summary>
[Serializable()]
public struct SolidityMapping : System.Runtime.Serialization.ISerializable
{
   /// <summary>
   /// Designates the category of tiles to which a shape is applied.
   /// </summary>
   public TileCategoryName category;
   /// <summary>
   /// Designates the shape of the tiles in the associated category.
   /// </summary>
   public TileShape shape;

   /// <summary>
   /// Constructs a SolidityMapping given all its parameters
   /// </summary>
   /// <param name="category">Initial value for <see cref="category"/></param>
   /// <param name="shape">Initial value for <see cref="shape"/></param>
   public SolidityMapping(TileCategoryName category, TileShape shape)
   {
      this.category = category;
      this.shape = shape;
   }

   private SolidityMapping(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
   {
      category = (TileCategoryName)info.GetInt32("TileCategoryName");
      shape = (TileShape)System.Type.GetType(info.GetString("TileShapeName"), true, false).GetProperty("Value",
         System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Static |
         System.Reflection.BindingFlags.Public).GetValue(null, null);
   }

   /// <summary>
   /// This is provided to allow the object to be serialized for the purposes of saving and loading game data.
   /// </summary>
   public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
   {
      info.AddValue("TileCategoryName", (System.Int32)category);
      info.AddValue("TileShapeName", shape.GetType().Name);
   }
}

/// <summary>
/// Defines a particular shape that a tile can have at any/all sizes
/// </summary>
public abstract class TileShape
{
   public TileShape()
   {
   }
   /// <summary>
   /// Gets the vertical coordinate of the top-most solid pixel of a tile shape between two horizontal offsets.
   /// </summary>
   /// <param name="width">Width of the tile to which the shape is being applied</param>
   /// <param name="height">Height of the tile to which the shape is being applied</param>
   /// <param name="min">Horizontal offset of the left side of the range to test</param>
   /// <param name="max">Horizontal offset of the right side of the range to test</param>
   /// <returns>A value between 0 and height-1, inclusive, or short.MaxValue if there is no solid</returns>
   public abstract short GetTopSolidPixel(short width, short height, short min, short max);
   /// <summary>
   /// Gets the vertical coordinate of the bottom-most solid pixel of a tile shape between two horizontal offsets.
   /// </summary>
   /// <param name="width">Width of the tile to which the shape is being applied</param>
   /// <param name="height">Height of the tile to which the shape is being applied</param>
   /// <param name="min">Horizontal offset of the left side of the range to test</param>
   /// <param name="max">Horizontal offset of the right side of the range to test</param>
   /// <returns>A value between 0 and height-1, inclusive, or short.MinValue if there is no solid</returns>
   public abstract short GetBottomSolidPixel(short width, short height, short min, short max);
   /// <summary>
   /// Gets the horizontal coordinate of the left-most solid pixel of a tile shape between two vertical offsets
   /// </summary>
   /// <param name="width">Width of the tile to which the shape is being applied</param>
   /// <param name="height">Height of the tile to which the shape is being applied</param>
   /// <param name="min">Vertical offset of the top of the range to test</param>
   /// <param name="max">Vertical offset of the bottom of the range to test</param>
   /// <returns>A value between 0 and width-1, inclusive, or short.MaxValue if there is no solid</returns>
   public abstract short GetLeftSolidPixel(short width, short height, short min, short max);
   /// <summary>
   /// Gets the horizontal coordinate of the right-most solid pixel of a tile shape between two vertical offsets
   /// </summary>
   /// <param name="width">Width of the tile to which the shape is being applied</param>
   /// <param name="height">Height of the tile to which the shape is being applied</param>
   /// <param name="min">Vertical offset of the top of the range to test</param>
   /// <param name="max">Vertical offset of the bottom of the range to test</param>
   /// <returns>A value between 0 and width-1, inclusive, or short.MinValue if there is no solid</returns>
   public abstract short GetRightSolidPixel(short width, short height, short min, short max);
}

/// <summary>
/// Represents an empty tile (nothing solid)
/// </summary>
public class EmptyTileShape : TileShape
{
   private static EmptyTileShape m_Value = new EmptyTileShape();

   /// <summary>
   /// Returns the default instance of this class.
   /// </summary>
   /// <remarks>Objects derived from <see cref="TileShape"/> generally have a default
   /// instance that all code can refer to because they are just a set of functions
   /// and don't have any instance-specific data.  So all code can share one instance
   /// rather than each case dealing with creating its own.</remarks>
   public static EmptyTileShape Value
   {
      get
      {
         return m_Value;
      }
   }

   public EmptyTileShape()
   {
   }

   /// <summary>
   /// See <see cref="TileShape.GetTopSolidPixel"/>.
   /// </summary>
   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return short.MaxValue;
   }

   /// <summary>
   /// See <see cref="TileShape.GetBottomSolidPixel"/>.
   /// </summary>
   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return short.MinValue;
   }

   /// <summary>
   /// See <see cref="TileShape.GetLeftSolidPixel"/>.
   /// </summary>
   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return short.MaxValue;
   }

   /// <summary>
   /// See <see cref="TileShape.GetRightSolidPixel"/>.
   /// </summary>
   public override short GetRightSolidPixel(short width, short height, short min, short max)
   {
      return short.MinValue;
   }
}

/// <summary>
/// Represents a solid tile (flat solid on all 4 sides)
/// </summary>
public class SolidTileShape : TileShape
{
   private static SolidTileShape m_Value = new SolidTileShape();

   /// <summary>
   /// Returns the default instance of this class.
   /// </summary>
   /// <remarks>Objects derived from <see cref="TileShape"/> generally have a default
   /// instance that all code can refer to because they are just a set of functions
   /// and don't have any instance-specific data.  So all code can share one instance
   /// rather than each case dealing with creating its own.</remarks>
   public static SolidTileShape Value
   {
      get
      {
         return m_Value;
      }
   }

   public SolidTileShape()
   {
   }

   /// <summary>
   /// See <see cref="TileShape.GetTopSolidPixel"/>.
   /// </summary>
   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return 0;
   }

   /// <summary>
   /// See <see cref="TileShape.GetBottomSolidPixel"/>.
   /// </summary>
   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height-1);
   }

   /// <summary>
   /// See <see cref="TileShape.GetLeftSolidPixel"/>.
   /// </summary>
   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return 0;
   }

   /// <summary>
   /// See <see cref="TileShape.GetRightSolidPixel"/>.
   /// </summary>
   public override short GetRightSolidPixel(short width, short height, short min, short max)
   {
      return (short)(width-1);
   }
}

/*
   +--------+
   |       X|
   |      XX|
   |     XXX|
   |    XXXX|
   |   XXXXX|
   |  XXXXXX|
   | XXXXXXX|
   |XXXXXXXX|
   +--------+
*/
/// <summary>
/// Represents an "uphill" shape with a slope leading from the lower left corner
/// to the upper right corner with the lower right half of the tile being solid.
/// </summary>
public class UphillTileShape : TileShape
{
   private static UphillTileShape m_Value = new UphillTileShape();

   /// <summary>
   /// Returns the default instance of this class.
   /// </summary>
   /// <remarks>Objects derived from <see cref="TileShape"/> generally have a default
   /// instance that all code can refer to because they are just a set of functions
   /// and don't have any instance-specific data.  So all code can share one instance
   /// rather than each case dealing with creating its own.</remarks>
   public static UphillTileShape Value
   {
      get
      {
         return m_Value;
      }
   }

   public UphillTileShape()
   {
   }

   /// <summary>
   /// See <see cref="TileShape.GetTopSolidPixel"/>.
   /// </summary>
   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height * (width-max-1) / width);
   }

   /// <summary>
   /// See <see cref="TileShape.GetBottomSolidPixel"/>.
   /// </summary>
   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height-1);
   }

   /// <summary>
   /// See <see cref="TileShape.GetLeftSolidPixel"/>.
   /// </summary>
   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return (short)(width * (height-max-1) / height);
   }

   /// <summary>
   /// See <see cref="TileShape.GetRightSolidPixel"/>.
   /// </summary>
   public override short GetRightSolidPixel(short width, short height, short min, short max)
   {
      return (short)(width-1);
   }
}

/*
   +--------+
   |X       |
   |XX      |
   |XXX     |
   |XXXX    |
   |XXXXX   |
   |XXXXXX  |
   |XXXXXXX |
   |XXXXXXXX|
   +--------+
*/
/// <summary>
/// Represents a "downhill" shape with a slope leading from the upper left corner
/// to the lower right corner with the lower left half of the tile being solid.
/// </summary>
public class DownhillTileShape : TileShape
{
   private static DownhillTileShape m_Value = new DownhillTileShape();

   /// <summary>
   /// Returns the default instance of this class.
   /// </summary>
   /// <remarks>Objects derived from <see cref="TileShape"/> generally have a default
   /// instance that all code can refer to because they are just a set of functions
   /// and don't have any instance-specific data.  So all code can share one instance
   /// rather than each case dealing with creating its own.</remarks>
   public static DownhillTileShape Value
   {
      get
      {
         return m_Value;
      }
   }

   public DownhillTileShape()
   {
   }

   /// <summary>
   /// See <see cref="TileShape.GetTopSolidPixel"/>.
   /// </summary>
   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return (short)(min * height / width);
   }

   /// <summary>
   /// See <see cref="TileShape.GetBottomSolidPixel"/>.
   /// </summary>
   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height-1);
   }

   /// <summary>
   /// See <see cref="TileShape.GetLeftSolidPixel"/>.
   /// </summary>
   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return 0;
   }

   /// <summary>
   /// See <see cref="TileShape.GetRightSolidPixel"/>.
   /// </summary>
   public override short GetRightSolidPixel(short width, short height, short min, short max)
   {
      return (short)(width - (height - max - 1) * width / height - 1);
   }
}

/*
   +--------+
   |XXXXXXXX|
   |XXXXXXX |
   |XXXXXX  |
   |XXXXX   |
   |XXXX    |
   |XXX     |
   |XX      |
   |X       |
   +--------+
*/
/// <summary>
/// Represents an "upward ceiling" shape with a slope leading from the lower left corner
/// to the upper right corner with the upper left half of the tile being solid.
/// </summary>
public class UpCeilingTileShape : TileShape
{
   private static UpCeilingTileShape m_Value = new UpCeilingTileShape();

   /// <summary>
   /// Returns the default instance of this class.
   /// </summary>
   /// <remarks>Objects derived from <see cref="TileShape"/> generally have a default
   /// instance that all code can refer to because they are just a set of functions
   /// and don't have any instance-specific data.  So all code can share one instance
   /// rather than each case dealing with creating its own.</remarks>
   public static UpCeilingTileShape Value
   {
      get
      {
         return m_Value;
      }
   }

   public UpCeilingTileShape()
   {
   }

   /// <summary>
   /// See <see cref="TileShape.GetTopSolidPixel"/>.
   /// </summary>
   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return 0;
   }

   /// <summary>
   /// See <see cref="TileShape.GetBottomSolidPixel"/>.
   /// </summary>
   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(((width - min) * height - 1) / width);
   }

   /// <summary>
   /// See <see cref="TileShape.GetLeftSolidPixel"/>.
   /// </summary>
   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return 0;
   }

   /// <summary>
   /// See <see cref="TileShape.GetRightSolidPixel"/>.
   /// </summary>
   public override short GetRightSolidPixel(short width, short height, short min, short max)
   {
      return (short)(((height - min) * width - 1) / height);
   }
}

/*
   +--------+
   |XXXXXXXX|
   | XXXXXXX|
   |  XXXXXX|
   |   XXXXX|
   |    XXXX|
   |     XXX|
   |      XX|
   |       X|
   +--------+
*/
/// <summary>
/// Represents a "downward ceiling" shape with a slope leading from the upper left corner
/// to the lower right corner with the upper right half of the tile being solid.
/// </summary>
public class DownCeilingTileShape : TileShape
{
   private static DownCeilingTileShape m_Value = new DownCeilingTileShape();

   /// <summary>
   /// Returns the default instance of this class.
   /// </summary>
   /// <remarks>Objects derived from <see cref="TileShape"/> generally have a default
   /// instance that all code can refer to because they are just a set of functions
   /// and don't have any instance-specific data.  So all code can share one instance
   /// rather than each case dealing with creating its own.</remarks>
   public static DownCeilingTileShape Value
   {
      get
      {
         return m_Value;
      }
   }

   public DownCeilingTileShape()
   {
   }

   /// <summary>
   /// See <see cref="TileShape.GetTopSolidPixel"/>.
   /// </summary>
   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return 0;
   }

   /// <summary>
   /// See <see cref="TileShape.GetBottomSolidPixel"/>.
   /// </summary>
   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height - (width - max - 1) * height / width - 1);
   }

   /// <summary>
   /// See <see cref="TileShape.GetLeftSolidPixel"/>.
   /// </summary>
   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return (short)(min * width / height);
   }

   /// <summary>
   /// See <see cref="TileShape.GetRightSolidPixel"/>.
   /// </summary>
   public override short GetRightSolidPixel(short width, short height, short min, short max)
   {
      return (short)(width - 1);
   }
}

/*
   +--------+
   |      XX|
   |    XXXX|
   |  XXXXXX|
   |XXXXXXXX|
   |XXXXXXXX|
   |XXXXXXXX|
   |XXXXXXXX|
   |XXXXXXXX|
   +--------+
*/
/// <summary>
/// Represents the right half of a gradual "uphill" tile with a slope leading from the
/// middle left to the upper right corner with the lower portion of the tile being solid.
/// </summary>
public class UphillRightTileShape : TileShape
{
   private static UphillRightTileShape m_Value = new UphillRightTileShape();

   /// <summary>
   /// Returns the default instance of this class.
   /// </summary>
   /// <remarks>Objects derived from <see cref="TileShape"/> generally have a default
   /// instance that all code can refer to because they are just a set of functions
   /// and don't have any instance-specific data.  So all code can share one instance
   /// rather than each case dealing with creating its own.</remarks>
   public static UphillRightTileShape Value
   {
      get
      {
         return m_Value;
      }
   }

   public UphillRightTileShape()
   {
   }

   /// <summary>
   /// See <see cref="TileShape.GetTopSolidPixel"/>.
   /// </summary>
   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height * (width - max - 1) / width / 2);
   }

   /// <summary>
   /// See <see cref="TileShape.GetBottomSolidPixel"/>.
   /// </summary>
   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height - 1);
   }

   /// <summary>
   /// See <see cref="TileShape.GetLeftSolidPixel"/>.
   /// </summary>
   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return (short)((max * 2 >= height - 2) ? 0 : width * (height - max * 2 - 2) / height);
   }

   /// <summary>
   /// See <see cref="TileShape.GetRightSolidPixel"/>.
   /// </summary>
   public override short GetRightSolidPixel(short width, short height, short min, short max)
   {
      return (short)(width - 1);
   }
}

/*
   +--------+
   |        |
   |        |
   |        |
   |        |
   |      XX|
   |    XXXX|
   |  XXXXXX|
   |XXXXXXXX|
   +--------+
*/
/// <summary>
/// Represents the left half of a gradual "uphill" tile with a slope leading from the
/// lower left corner to the middle right with the lower portion of the tile being solid.
/// </summary>
public class UphillLeftTileShape : TileShape
{
   private static UphillLeftTileShape m_Value = new UphillLeftTileShape();

   /// <summary>
   /// Returns the default instance of this class.
   /// </summary>
   /// <remarks>Objects derived from <see cref="TileShape"/> generally have a default
   /// instance that all code can refer to because they are just a set of functions
   /// and don't have any instance-specific data.  So all code can share one instance
   /// rather than each case dealing with creating its own.</remarks>
   public static UphillLeftTileShape Value
   {
      get
      {
         return m_Value;
      }
   }

   public UphillLeftTileShape()
   {
   }

   /// <summary>
   /// See <see cref="TileShape.GetTopSolidPixel"/>.
   /// </summary>
   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height * (width - max - 1) / width / 2 + height / 2);
   }

   /// <summary>
   /// See <see cref="TileShape.GetBottomSolidPixel"/>.
   /// </summary>
   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height - 1);
   }

   /// <summary>
   /// See <see cref="TileShape.GetLeftSolidPixel"/>.
   /// </summary>
   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return (short)(((max + 1) * 2 <= height)?short.MaxValue:width * (height - max - 1) * 2 / height);
   }

   /// <summary>
   /// See <see cref="TileShape.GetRightSolidPixel"/>.
   /// </summary>
   public override short GetRightSolidPixel(short width, short height, short min, short max)
   {
      return (short)(((max + 1) * 2 <= height)?short.MinValue:width - 1);
   }
}

/*
   +--------+
   |XX      |
   |XXXX    |
   |XXXXXX  |
   |XXXXXXXX|
   |XXXXXXXX|
   |XXXXXXXX|
   |XXXXXXXX|
   |XXXXXXXX|
   +--------+
*/
/// <summary>
/// Represents the left half of a gradual "downhill" tile with a slope leading from the
/// upper left corner to the middle right with the lower portion of the tile being solid.
/// </summary>
public class DownhillLeftTileShape : TileShape
{
   private static DownhillLeftTileShape m_Value = new DownhillLeftTileShape();

   /// <summary>
   /// Returns the default instance of this class.
   /// </summary>
   /// <remarks>Objects derived from <see cref="TileShape"/> generally have a default
   /// instance that all code can refer to because they are just a set of functions
   /// and don't have any instance-specific data.  So all code can share one instance
   /// rather than each case dealing with creating its own.</remarks>
   public static DownhillLeftTileShape Value
   {
      get
      {
         return m_Value;
      }
   }

   public DownhillLeftTileShape()
   {
   }

   /// <summary>
   /// See <see cref="TileShape.GetTopSolidPixel"/>.
   /// </summary>
   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return (short)(min * height / width / 2);
   }

   /// <summary>
   /// See <see cref="TileShape.GetBottomSolidPixel"/>.
   /// </summary>
   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height-1);
   }

   /// <summary>
   /// See <see cref="TileShape.GetLeftSolidPixel"/>.
   /// </summary>
   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return 0;
   }

   /// <summary>
   /// See <see cref="TileShape.GetRightSolidPixel"/>.
   /// </summary>
   public override short GetRightSolidPixel(short width, short height, short min, short max)
   {
      return (short)(((max + 1) * 2 > height) ? width - 1 : width * 2 - (height - max - 1) * width * 2 / height - 1);
   }
}

/*
   +--------+
   |        |
   |        |
   |        |
   |        |
   |XX      |
   |XXXX    |
   |XXXXXX  |
   |XXXXXXXX|
   +--------+
*/
/// <summary>
/// Represents the right half of a gradual "downhill" tile with a slope leading from the
/// middle left to the lower right corner with the lower portion of the tile being solid.
/// </summary>
public class DownhillRightTileShape : TileShape
{
   private static DownhillRightTileShape m_Value = new DownhillRightTileShape();

   /// <summary>
   /// Returns the default instance of this class.
   /// </summary>
   /// <remarks>Objects derived from <see cref="TileShape"/> generally have a default
   /// instance that all code can refer to because they are just a set of functions
   /// and don't have any instance-specific data.  So all code can share one instance
   /// rather than each case dealing with creating its own.</remarks>
   public static DownhillRightTileShape Value
   {
      get
      {
         return m_Value;
      }
   }

   public DownhillRightTileShape()
   {
   }

   /// <summary>
   /// See <see cref="TileShape.GetTopSolidPixel"/>.
   /// </summary>
   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return (short)((height + min * height / width) / 2);
   }

   /// <summary>
   /// See <see cref="TileShape.GetBottomSolidPixel"/>.
   /// </summary>
   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height-1);
   }

   /// <summary>
   /// See <see cref="TileShape.GetLeftSolidPixel"/>.
   /// </summary>
   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return (short)(((min + 1) * 2 <= height) ? short.MaxValue : (short)0);
   }

   /// <summary>
   /// See <see cref="TileShape.GetRightSolidPixel"/>.
   /// </summary>
   public override short GetRightSolidPixel(short width, short height, short min, short max)
   {
      return (short)(((max + 1) * 2 <= height) ? short.MinValue : width - (height - max - 1) * 2 * width / height - 1);
   }
}


/*
   +--------+
   |^^^^^^^^|
   |        |
   |        |
   |        |
   |        |
   |        |
   |        |
   |        |
   +--------+
   (Top is solid only from above)
*/
/// <summary>
/// Represents a tile that is only solid from above. A sprite can move freely through this tile
/// in any direction unless the sprite is fully above the tile in which case it cannot penetrate
/// the top.
/// </summary>
public class TopSolidTileShape : TileShape
{
   private static TopSolidTileShape m_Value = new TopSolidTileShape();

   /// <summary>
   /// Returns the default instance of this class.
   /// </summary>
   /// <remarks>Objects derived from <see cref="TileShape"/> generally have a default
   /// instance that all code can refer to because they are just a set of functions
   /// and don't have any instance-specific data.  So all code can share one instance
   /// rather than each case dealing with creating its own.</remarks>
   public static TopSolidTileShape Value
   {
      get
      {
         return m_Value;
      }
   }

   public TopSolidTileShape()
   {
   }

   /// <summary>
   /// See <see cref="TileShape.GetTopSolidPixel"/>.
   /// </summary>
   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return 0;
   }

   /// <summary>
   /// See <see cref="TileShape.GetBottomSolidPixel"/>.
   /// </summary>
   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return short.MinValue;
   }

   /// <summary>
   /// See <see cref="TileShape.GetLeftSolidPixel"/>.
   /// </summary>
   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return short.MaxValue;
   }

   /// <summary>
   /// See <see cref="TileShape.GetRightSolidPixel"/>.
   /// </summary>
   public override short GetRightSolidPixel(short width, short height, short min, short max)
   {
      return short.MinValue;
   }
}

