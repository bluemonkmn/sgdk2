using System;
using System.ComponentModel;

public abstract partial class LightSpriteBase : SpriteBase
{
   public float constantFalloff;
   public float linearFalloff;
   public float quadraticFalloff;
   public float aimX;
   public float aimY;
   public float apertureFocus;
   public float apertureSoftness;

   public LightSpriteBase(LayerBase layer, double x, double y, double dx, double dy, int state, int frame,
      bool active, Solidity solidity, int color, float constantFalloff, float linearFalloff, float quadraticFalloff,
      float aimX, float aimY, float apertureFocus, float apertureSoftness)
      : base(layer, x, y, dx, dy, state, frame, active, solidity, color)
   {
      this.constantFalloff = constantFalloff;
      this.linearFalloff = linearFalloff;
      this.quadraticFalloff = quadraticFalloff;
      this.aimX = aimX;
      this.aimY = aimY;
      this.apertureFocus = apertureFocus;
      this.apertureSoftness = apertureSoftness;
   }

   [Description("Rotates the AimX and AimY of this light clockwise (+) or counterclockwise (-).")]
   public void RotateLight(int degreeOffset)
   {
      float radians;
      if ((aimX == 0) && (aimY == 0))
         radians = 0;
      else
         radians = (float)Math.Atan2(aimY, aimX);
      radians += (float)(degreeOffset * Math.PI / 180.0);
      aimX = (float)Math.Cos(radians);
      aimY = (float)Math.Sin(radians);
   }
}