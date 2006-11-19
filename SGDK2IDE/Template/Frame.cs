using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;

public struct Frame
{
   public Display.TextureRef GraphicSheetTexture;
   public short CellIndex;
   public Matrix Transform;
   public int Color;
   public Rectangle SourceRect;

   public Frame(Display.TextureRef texture, short cell, float M11, float M12, float M21, float M22, float M41, float M42, Rectangle srcRect, int color)
   {
      GraphicSheetTexture = texture;
      CellIndex = cell;
      Transform = Matrix.Identity;
      Transform.M11 = M11;
      Transform.M12 = M12;
      Transform.M21 = M21;
      Transform.M22 = M22;
      Transform.M41 = M41;
      Transform.M42 = M42;
      Transform.M44 = 1;
      SourceRect = srcRect;
      this.Color = color;
   }

   public Frame(Display.TextureRef texture, short cell, float M11, float M12, float M21, float M22, float M41, float M42, Rectangle srcRect) :
      this(texture, cell, M11, M12, M21, M22, M41, M42, srcRect, -1)
   {
   }

   public Frame(Display.TextureRef texture, short cell, Rectangle srcRect, int color)
   {
      GraphicSheetTexture = texture;
      CellIndex = cell;
      Transform = Matrix.Identity;
      SourceRect = srcRect;
      this.Color = color;
   }

   public Frame(Display.TextureRef texture, short cell, Rectangle srcRect) :
      this(texture, cell, srcRect, -1)
   {
   }
}
