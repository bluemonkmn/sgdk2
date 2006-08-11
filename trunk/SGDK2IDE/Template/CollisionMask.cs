using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

/// <summary>
/// Defines the shape of a sprite for the purpose of collision detection with other sprites
/// </summary>
public class CollisionMask
{
   private int[,] m_Mask;
   private int m_Width;
   private int m_Height;
   private Point m_Origin;
   private static System.Collections.Hashtable m_RectangularMasks = new System.Collections.Hashtable(10);
	
   public CollisionMask(Rectangle localBounds, Frameset frameset, int[] subFrames, byte[] alphas)
   {
      m_Mask = GenerateMaskFromFrame(localBounds, frameset, subFrames, alphas);
      m_Width = localBounds.Width;
      m_Height = localBounds.Height;
      m_Origin = new Point(-localBounds.X, -localBounds.Y);
   }

   private CollisionMask(Size size)
   {
      m_Mask = new int[size.Height, (int)Math.Ceiling(size.Width / 32f)];
      for (int x = 0; x < (int)(size.Width / 32); x++)
      {
         for (int y = 0; y < size.Height; y++)
         {
            m_Mask[y,x] = unchecked((int)0xFFFFFFFF);
         }
      }
      if (size.Width % 32 > 0)
      {
         int lastColMask = 0;
         lastColMask |= (-1) << (32 - (size.Width % 32));
         int lastColIdx = (int)Math.Ceiling(size.Width / 32f);
         for (int y = 0; y < size.Height; y++)
         {
            m_Mask[y, lastColIdx] = lastColMask;
         }
      }
      m_Width = size.Width;
      m_Height = size.Height;
      m_Origin = new Point(0,0);
   }

   public static CollisionMask GetRectangularMask(Size size)
   {
      CollisionMask result = m_RectangularMasks[size] as CollisionMask;
      if (result != null)
         return result;
      m_RectangularMasks[size] = result = new CollisionMask(size);
      return result;
   }

   /// <summary>
   /// Determine if this mask is colliding with the specified target mask
   /// </summary>
   /// <param name="target">Mask to test against</param>
   /// <param name="offset">Offset from this mask's position to the target mask's position</param>
   /// <returns>True if solid bits in the masks collide when positioned at the specified offset</returns>
   public bool TestCollisionWith(CollisionMask target, int offsetX, int offsetY)
   {
      offsetX += m_Origin.X - target.m_Origin.X;
      offsetY += m_Origin.Y - target.m_Origin.Y;

      if ((offsetY >= m_Height) || (offsetY <= -target.m_Height) ||
         (offsetX >= m_Width) || (offsetX <= -target.m_Width))
         return false;

      int maxY;
      int myMinY, targetMinY;
      if (offsetY > 0)
      {
         myMinY = offsetY;
         targetMinY = 0;
         if (target.m_Height >= m_Height - offsetY)
            maxY = m_Height - offsetY;
         else
            maxY = target.m_Height;
      }
      else
      {
         myMinY = 0;
         targetMinY = -offsetY;
         if (m_Height >= target.m_Height + offsetY)
            maxY = target.m_Height + offsetY;
         else
            maxY = m_Height;
      }

      int maxX;
      int myMinX, targetMinX;
      if (offsetX > 0)
      {
         myMinX = offsetX;
         targetMinX = 0;
         if (target.m_Width >= m_Width - offsetX)
            maxX = m_Width - offsetX;
         else
            maxX = target.m_Width;
      }
      else
      {
         myMinX = 0;
         targetMinX = -offsetX;
         if (m_Width >= target.m_Width + offsetX)
            maxX = target.m_Width + offsetX;
         else
            maxX = m_Width;
      }

      for(int y=0; y < maxY; y++)
      {
         for(int x=0; x < maxX; x+=32)
         {
            int myColIdx = (int)((x+myMinX)/32);
            int myColOff = myMinX % 32;
            int targetColIdx = (int)((x+targetMinX)/32);
            int targetColOff = targetMinX % 32;
            int myMask = m_Mask[y+myMinY,myColIdx] << myColOff;
            int targetMask = target.m_Mask[y+targetMinY,targetColIdx] << targetColOff;
            if (myColOff != 0)
            {
               if (myColIdx + 1 < m_Mask.GetUpperBound(1))
                  myMask |= (m_Mask[y+myMinY,myColIdx+1] >> (32-myColOff)) &
                     ~(unchecked((int)0x80000000) >> (31-myColOff));
            }
            else if (targetColOff != 0)
            {
               if (targetColIdx + 1 < target.m_Mask.GetUpperBound(1))
                  targetMask |= (target.m_Mask[y+targetMinY,targetColIdx+1] >> (32-targetColOff)) &
                     ~(unchecked((int)0x80000000) >> (31-targetColOff));
            }
            if ((myMask & targetMask) != 0)
               return true;
         }
      }
      return false;
   }

   private static int[,] GenerateMaskFromFrame(Rectangle rcBound, Frameset frameset, int[] subFrames, byte[] alphas)
   {
      BitmapData bmpData;
      int[] pixels;

      using (Bitmap bmpSingle = new Bitmap(rcBound.Width, rcBound.Height, PixelFormat.Format32bppArgb))
      {
         int maskColumns = (int)Math.Ceiling(bmpSingle.Width / 32f);
         int[,] arbt = new int[bmpSingle.Height, maskColumns];
         using (Graphics gfxSingle = Graphics.FromImage(bmpSingle))
         {
            gfxSingle.CompositingMode = CompositingMode.SourceCopy;
            gfxSingle.PixelOffsetMode = PixelOffsetMode.Half;

            for(int subFrameIdx = 0; subFrameIdx < subFrames.Length; subFrameIdx++)
            {
               int subFrame = subFrames[subFrameIdx];
               gfxSingle.Clear(Color.Transparent);
               Frame SFrame = frameset[subFrame];
               using(Matrix mtx = new Matrix(SFrame.Transform.M11, SFrame.Transform.M12, SFrame.Transform.M21, SFrame.Transform.M22, SFrame.Transform.M41, SFrame.Transform.M42))
               {
                  gfxSingle.Transform = mtx;
                  Bitmap bmpGfxSheet = (Bitmap)Project.Resources.GetObject(SFrame.GraphicSheetTexture.Name);
                  gfxSingle.TranslateTransform(-rcBound.X, -rcBound.Y, MatrixOrder.Append);
                  gfxSingle.DrawImage(bmpGfxSheet, 0, 0, SFrame.SourceRect, GraphicsUnit.Pixel);
                  bmpData = bmpSingle.LockBits(new Rectangle(Point.Empty, rcBound.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                  pixels = new int[bmpSingle.Height * Math.Abs(bmpData.Stride) / 4];
                  System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixels, 0, bmpSingle.Height * Math.Abs(bmpData.Stride) / 4);
                  bmpSingle.UnlockBits(bmpData);
                  for (int rowIdx = 0; rowIdx < bmpSingle.Height; rowIdx++)
                  {
                     for (int pixIdx = 0; pixIdx < bmpSingle.Width; pixIdx++)
                     {
                        if (Color.FromArgb(pixels[rowIdx * bmpData.Stride / 4 + pixIdx]).A > alphas[subFrameIdx])
                           arbt[rowIdx, pixIdx / 32] |= 1 << (31 - (pixIdx % 32));
                     }
                  }
               }
            }
         }
         return arbt;
      }
   }
}
