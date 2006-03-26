using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;

public struct Frame
{
   public Display.TextureRef GraphicSheetTexture;
   public short CellIndex;
   public Matrix Transform;
   public Rectangle SourceRect;

   public Frame(Display.TextureRef texture, short cell, float M11, float M12, float M21, float M22, float M41, float M42, Rectangle srcRect)
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
   }

   public Frame(Display.TextureRef texture, short cell, Rectangle srcRect)
   {
      GraphicSheetTexture = texture;
      CellIndex = cell;
      Transform = Matrix.Identity;
      SourceRect = srcRect;
   }
}
