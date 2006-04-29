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
/// Stores pre-computed information about a tile shape at a particular size
/// </summary>
public class SizedTileShape
{
   // m_TopSolids[0] returns topmost pixel in column 0
   // m_TopSolids[1] returns topmost pixel in columns 0-1
   // m_TopSolids[Width] returns topmost pixel in column 1
   // m_TopSolids[Width+1] returns topmost pixel in columns 1-2
   private byte[] m_TopSolids = null;
   private byte[] m_BottomSolids = null;
   private byte[] m_LeftSolids = null;
   private byte[] m_RightSolids = null;

   public readonly byte Width;
   public readonly byte Height;

   public SizedTileShape(byte width, byte height, TileShape shape)
   {
      this.Width = width;
      this.Height = height;

      m_TopSolids = new byte[(width * width + width) / 2];
      m_BottomSolids = new byte[(width * width + width) / 2];
      m_LeftSolids = new byte[(height * height + height) / 2];
      m_RightSolids = new byte[(height * height + height) / 2];
      
      for (byte x=0; x<Width; x++)
      {
         byte y;
         for (y=0; y<Height; y++)
         {
            if (shape.TestPixel(x, y, Width, Height))
            {
               SetTopSolidPixel(x, x, y);
               break;
            }
         }

         if (y >= Height)
            SetTopSolidPixel(x, x, byte.MaxValue);

         for (y=(byte)(Height-1); y>=0; y--)
         {
            if (shape.TestPixel(x, y, Width, Height))
            {
               SetBottomSolidPixel(x, x, y);
               break;
            }
         }

         if (y < 0)
            SetBottomSolidPixel(x, x, byte.MaxValue);
      }

      for (byte y=0; y<Height; y++)
      {
         byte x;
         for (x=0; x<Width; x++)
         {
            if (shape.TestPixel(x, y, Width, Height))
            {
               SetLeftSolidPixel(y, y, x);
               break;
            }
         }

         if (x >= Width)
            SetLeftSolidPixel(y, y, byte.MaxValue);

         for (x=(byte)(Width-1); x>=0; x--)
         {
            if (shape.TestPixel(x, y, Width, Height))
            {
               SetRightSolidPixel(y, y, x);
               break;
            }
         }

         if (x < 0)
            SetRightSolidPixel(y, y, byte.MaxValue);
      }

      for (byte x1=0; x1 < width - 1; x1++)
         for (byte x2=(byte)(x1+1); x2 < width; x2++)
         {
            byte nVal1 = GetTopSolidPixel(x1, (byte)(x2-1));
            byte nVal2 = GetTopSolidPixel(x2, x2);
            byte nFinal;
            if (nVal1 == byte.MaxValue)
               nFinal = nVal2;
            else if (nVal2 == byte.MaxValue)
               nFinal = nVal1;
            else if (nVal1 < nVal2)
               nFinal = nVal1;
            else
               nFinal = nVal2;
            SetTopSolidPixel(x1, x2, nFinal);

            nVal1 = GetBottomSolidPixel(x1, (byte)(x2-1));
            nVal2 = GetBottomSolidPixel(x2, x2);
            if (nVal1 == byte.MaxValue)
               nFinal = nVal2;
            else if (nVal2 == byte.MaxValue)
               nFinal = nVal1;
            else if (nVal1 > nVal2)
               nFinal = nVal1;
            else
               nFinal = nVal2;
            SetBottomSolidPixel(x1, x2, nFinal);
         }

      for (byte y1=0; y1 < height - 1; y1++)
         for (byte y2=(byte)(y1+1); y2 < height; y2++)
         {
            byte nVal1 = GetLeftSolidPixel(y1, (byte)(y2-1));
            byte nVal2 = GetLeftSolidPixel(y2, y2);
            byte nFinal;
            if (nVal1 == byte.MaxValue)
               nFinal = nVal2;
            else if (nVal2 == byte.MaxValue)
               nFinal = nVal1;
            else if (nVal1 < nVal2)
               nFinal = nVal1;
            else
               nFinal = nVal2;
            SetLeftSolidPixel(y1, y2, nFinal);

            nVal1 = GetRightSolidPixel(y1, (byte)(y2-1));
            nVal2 = GetRightSolidPixel(y2, y2);
            if (nVal1 == byte.MaxValue)
               nFinal = nVal2;
            else if (nVal2 == byte.MaxValue)
               nFinal = nVal1;
            else if (nVal1 > nVal2)
               nFinal = nVal1;
            else
               nFinal = nVal2;
            SetRightSolidPixel(y1, y2, nFinal);
         }
   }

   // Return the element in an array containing information for the
   // specified range of columns/rows
   private int Locate(byte min, byte max, byte total)
   {
      return min * total - (min * (min-1))>>1 + max - min ;
   }

   void SetTopSolidPixel(byte min, byte max, byte topVal)
   {
      m_TopSolids[Locate(min, max, Width)] = topVal;
   }

   public byte GetTopSolidPixel(byte min, byte max)
   {
      return m_TopSolids[Locate(min, max, Width)];
   }

   void SetBottomSolidPixel(byte min, byte max, byte BottomVal)
   {
      m_BottomSolids[Locate(min, max, Width)] = BottomVal;
   }

   public byte GetBottomSolidPixel(byte min, byte max)
   {
      return m_BottomSolids[Locate(min, max, Width)];
   }

   void SetLeftSolidPixel(byte min, byte max, byte LeftVal)
   {
      m_LeftSolids[Locate(min, max, Height)] = LeftVal;
   }

   public byte GetLeftSolidPixel(byte min, byte max)
   {
      return m_LeftSolids[Locate(min, max, Height)];
   }

   void SetRightSolidPixel(byte min, byte max, byte RightVal)
   {
      m_RightSolids[Locate(min, max, Height)] = RightVal;
   }

   public byte GetRightSolidPixel(byte min, byte max)
   {
      return m_RightSolids[Locate(min, max, Height)];
   }   
}

/// <summary>
/// Defines a particular shape that a tile can have at any/all sizes
/// </summary>
public abstract class TileShape
{
   protected System.Collections.Specialized.HybridDictionary m_SizedShapes;

   public TileShape()
   {
      m_SizedShapes = new System.Collections.Specialized.HybridDictionary();
   }

   protected SizedTileShape GetSizedTileShape(byte width, byte height)
   {
      byte[] hashbytes = new byte[2] {width, height};
      int hash = System.BitConverter.ToInt16(hashbytes, 0);
      SizedTileShape result = (SizedTileShape)m_SizedShapes[hashbytes];
      if (result == null)
         m_SizedShapes[hashbytes] = result = new SizedTileShape(width, height, this);         
      return result;
   }

   public virtual byte GetTopSolidPixel(byte width, byte height, byte min, byte max)
   {
      return GetSizedTileShape(width, height).GetTopSolidPixel(min, max);
   }
   public virtual byte GetBottomSolidPixel(byte width, byte height, byte min, byte max)
   {
      return GetSizedTileShape(width, height).GetBottomSolidPixel(min, max);
   }
   public virtual byte GetLeftSolidPixel(byte width, byte height, byte min, byte max)
   {
      return GetSizedTileShape(width, height).GetLeftSolidPixel(min, max);
   }
   public virtual byte GetRightSolidPixel(byte width, byte height, byte min, byte max)
   {
      return GetSizedTileShape(width, height).GetRightSolidPixel(min, max);
   }

   public abstract bool TestPixel(byte TileWidth, byte TileHeight, byte x, byte y);
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

   public override bool TestPixel(byte TileWidth, byte TileHeight, byte x, byte y)
   {
      return false;
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
      return height;
   }

   public override byte GetLeftSolidPixel(byte width, byte height, byte min, byte max)
   {
      return 0;
   }

   public override byte GetRightSolidPixel(byte width, byte height, byte min, byte max)
   {
      return width;
   }

   public override bool TestPixel(byte TileWidth, byte TileHeight, byte x, byte y)
   {
      return true;
   }
}
