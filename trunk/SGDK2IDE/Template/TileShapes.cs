using System;

[Serializable()]
public struct SolidityMapping : System.Runtime.Serialization.ISerializable
{
   public TileCategoryName category;
   public TileShape shape;

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
   // Returns a value between 0 and height-1, inclusive, or short.MaxValue if there is no solid
   public abstract short GetTopSolidPixel(short width, short height, short min, short max);
   // Returns a value between 0 and height-1, inclusive, or short.MinValue if there is no solid
   public abstract short GetBottomSolidPixel(short width, short height, short min, short max);
   // Returns a value between 0 and width-1, inclusive, or short.MaxValue if there is no solid
   public abstract short GetLeftSolidPixel(short width, short height, short min, short max);
   // Returns a value between 0 and width-1, inclusive, or short.MinValue if there is no solid
   public abstract short GetRightSolidPixel(short width, short height, short min, short max);
}

public class EmptyTileShape : TileShape
{
   private static EmptyTileShape m_Value = new EmptyTileShape();

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

   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return short.MaxValue;
   }

   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return short.MinValue;
   }

   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return short.MaxValue;
   }

   public override short GetRightSolidPixel(short width, short height, short min, short max)
   {
      return short.MinValue;
   }
}

public class SolidTileShape : TileShape
{
   private static SolidTileShape m_Value = new SolidTileShape();

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

   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return 0;
   }

   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height-1);
   }

   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return 0;
   }

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
public class UphillTileShape : TileShape
{
   private static UphillTileShape m_Value = new UphillTileShape();

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

   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height * (width-max-1) / width);
   }

   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height-1);
   }

   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return (short)(width * (height-max-1) / height);
   }

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
public class DownhillTileShape : TileShape
{
   private static DownhillTileShape m_Value = new DownhillTileShape();

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

   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return (short)(min * height / width);
   }

   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height-1);
   }

   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return 0;
   }

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
public class UpCeilingTileShape : TileShape
{
   private static UpCeilingTileShape m_Value = new UpCeilingTileShape();

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

   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return 0;
   }

   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(((width - min) * height - 1) / width);
   }

   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return 0;
   }

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
public class DownCeilingTileShape : TileShape
{
   private static DownCeilingTileShape m_Value = new DownCeilingTileShape();

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

   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return 0;
   }

   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height - (width - max - 1) * height / width - 1);
   }

   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return (short)(min * width / height);
   }

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
public class UphillRightTileShape : TileShape
{
   private static UphillRightTileShape m_Value = new UphillRightTileShape();

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

   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height * (width - max - 1) / width / 2);
   }

   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height - 1);
   }

   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return (short)((max * 2 >= height - 2) ? 0 : width * (height - max * 2 - 2) / height);
   }

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
public class UphillLeftTileShape : TileShape
{
   private static UphillLeftTileShape m_Value = new UphillLeftTileShape();

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

   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height * (width - max - 1) / width / 2 + height / 2);
   }

   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height - 1);
   }

   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return (short)(((max + 1) * 2 <= height)?short.MaxValue:width * (height - max - 1) * 2 / height);
   }

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
public class DownhillLeftTileShape : TileShape
{
   private static DownhillLeftTileShape m_Value = new DownhillLeftTileShape();

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

   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return (short)(min * height / width / 2);
   }

   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height-1);
   }

   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return 0;
   }

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
public class DownhillRightTileShape : TileShape
{
   private static DownhillRightTileShape m_Value = new DownhillRightTileShape();

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

   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return (short)((height + min * height / width) / 2);
   }

   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return (short)(height-1);
   }

   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return (short)(((min + 1) * 2 <= height) ? short.MaxValue : (short)0);
   }

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
public class TopSolidTileShape : TileShape
{
   private static TopSolidTileShape m_Value = new TopSolidTileShape();

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

   public override short GetTopSolidPixel(short width, short height, short min, short max)
   {
      return 0;
   }

   public override short GetBottomSolidPixel(short width, short height, short min, short max)
   {
      return short.MinValue;
   }

   public override short GetLeftSolidPixel(short width, short height, short min, short max)
   {
      return short.MaxValue;
   }

   public override short GetRightSolidPixel(short width, short height, short min, short max)
   {
      return short.MinValue;
   }
}

