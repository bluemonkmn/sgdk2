using System;

public struct SolidityMapping
{
   public TileCategoryName category;
   public TileShape shape;

   public SolidityMapping(TileCategoryName category, TileShape shape)
   {
      this.category = category;
      this.shape = shape;
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
   public abstract byte GetTopSolidPixel(byte width, byte height, byte min, byte max);
   public abstract byte GetBottomSolidPixel(byte width, byte height, byte min, byte max);
   public abstract byte GetLeftSolidPixel(byte width, byte height, byte min, byte max);
   public abstract byte GetRightSolidPixel(byte width, byte height, byte min, byte max);
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

   public override byte GetTopSolidPixel(byte width, byte height, byte min, byte max)
   {
      return byte.MaxValue;
   }

   public override byte GetBottomSolidPixel(byte width, byte height, byte min, byte max)
   {
      return byte.MaxValue;
   }

   public override byte GetLeftSolidPixel(byte width, byte height, byte min, byte max)
   {
      return byte.MaxValue;
   }

   public override byte GetRightSolidPixel(byte width, byte height, byte min, byte max)
   {
      return byte.MaxValue;
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

   public override byte GetTopSolidPixel(byte width, byte height, byte min, byte max)
   {
      return 0;
   }

   public override byte GetBottomSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(height-1);
   }

   public override byte GetLeftSolidPixel(byte width, byte height, byte min, byte max)
   {
      return 0;
   }

   public override byte GetRightSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(width-1);
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

   public override byte GetTopSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(height * (width-max-1) / width);
   }

   public override byte GetBottomSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(height-1);
   }

   public override byte GetLeftSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(width * (height-max-1) / height);
   }

   public override byte GetRightSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(width-1);
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

   public override byte GetTopSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(min * height / width);
   }

   public override byte GetBottomSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(height-1);
   }

   public override byte GetLeftSolidPixel(byte width, byte height, byte min, byte max)
   {
      return 0;
   }

   public override byte GetRightSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(width - (height - min - 1) * width / height - 1);
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

   public override byte GetTopSolidPixel(byte width, byte height, byte min, byte max)
   {
      return 0;
   }

   public override byte GetBottomSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)((width - min) * height / width);
   }

   public override byte GetLeftSolidPixel(byte width, byte height, byte min, byte max)
   {
      return 0;
   }

   public override byte GetRightSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(((height - min) * width - 1) / height);
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

   public override byte GetTopSolidPixel(byte width, byte height, byte min, byte max)
   {
      return 0;
   }

   public override byte GetBottomSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(height - (width - max - 1) * height / width - 1);
   }

   public override byte GetLeftSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(min * width / height);
   }

   public override byte GetRightSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(width - 1);
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

   public override byte GetTopSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(height * (width - max - 1) / width / 2);
   }

   public override byte GetBottomSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(height - 1);
   }

   public override byte GetLeftSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)((max * 2 >= height - 2) ? 0 : width * (height - max * 2 - 2) / height);
   }

   public override byte GetRightSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(width - 1);
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

   public override byte GetTopSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(height * (width - max - 1) / width / 2 + height / 2);
   }

   public override byte GetBottomSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(height - 1);
   }

   public override byte GetLeftSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(((max + 1) * 2 <= height)?byte.MaxValue:width * (height - max - 1) * 2 / height);
   }

   public override byte GetRightSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(((max + 1) * 2 <= height)?byte.MaxValue:width - 1);
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

   public override byte GetTopSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(min * height / width / 2);
   }

   public override byte GetBottomSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(height-1);
   }

   public override byte GetLeftSolidPixel(byte width, byte height, byte min, byte max)
   {
      return 0;
   }

   public override byte GetRightSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(((min + 1) * 2 > height) ? width - 1 : width * 2 - (height - min - 1) * width * 2 / height - 1);
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

   public override byte GetTopSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)((height + min * height / width) / 2);
   }

   public override byte GetBottomSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(height-1);
   }

   public override byte GetLeftSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(((min + 1) * 2 <= height) ? byte.MaxValue : (byte)0);
   }

   public override byte GetRightSolidPixel(byte width, byte height, byte min, byte max)
   {
      return (byte)(((min + 1) * 2 <= height) ? byte.MaxValue : width - (height - min - 1) * 2 * width / height - 1);
   }
}
