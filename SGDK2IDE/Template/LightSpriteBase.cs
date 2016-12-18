public abstract partial class LightSpriteBase : SpriteBase
{
   public float constantFalloff;
   public float linearFalloff;
   public float quadraticFalloff;

   public LightSpriteBase(LayerBase layer, double x, double y, double dx, double dy, int state, int frame,
      bool active, Solidity solidity, int color, float constantFalloff, float linearFalloff, float quadraticFalloff)
      : base(layer, x, y, dx, dy, state, frame, active, solidity, color)
   {
      this.constantFalloff = constantFalloff;
      this.linearFalloff = linearFalloff;
      this.quadraticFalloff = quadraticFalloff;
   }
}