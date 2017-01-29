using System;
using System.ComponentModel;

/// <summary>
/// Provides functionality and extra properties for sprites that represent dynamic light sources.
/// </summary>
public abstract partial class LightSpriteBase : SpriteBase
{
   /// <summary>
   /// Constant falloff works in conjunction with <see cref="linearFalloff"/> and <see cref="quadraticFalloff"/>
   /// to determine how this light source extends over distance.
   /// </summary>
   /// <remarks>
   /// The sum of these 3 values should be about 1.0. Lower values extend more light and higher values
   /// extend less light. Google Constant Linear Quadratic Lighting Falloff for details.
   /// </remarks>
   public float constantFalloff;
   /// <summary>
   /// Linear falloff works in conjunction with <see cref="constantFalloff"/> and <see cref="quadraticFalloff"/>
   /// to determine how this light source extends over distance.
   /// </summary>
   /// <remarks>
   /// The sum of these 3 values should be about 1.0. Lower values extend more light and higher values
   /// extend less light. Google Constant Linear Quadratic Lighting Falloff for details.
   /// </remarks>
   public float linearFalloff;
   /// <summary>
   /// Quadratic falloff works in conjunction with <see cref="constantFalloff"/> and <see cref="linearFalloff"/>
   /// to determine how this light source extends over distance.
   /// </summary>
   /// <remarks>
   /// The sum of these 3 values should be about 1.0. Lower values extend more light and higher values
   /// extend less light. Google Constant Linear Quadratic Lighting Falloff for details.
   /// </remarks>
   public float quadraticFalloff;
   /// <summary>
   /// Horizontal offset relative to this sprite's location at which this light source points.
   /// </summary>
   /// <remarks>
   /// Aim is mostly noticeable when <see cref="apertureFocus"/> is used to focus the light in one direction,
   /// however, if aim is not set, the light will not be visible. The size of the aim vector does not matter
   /// because it is normalized to a unit vector; only the direction matters.
   /// <seealso cref="aimY"/><seealso cref="aimZ"/>
   /// </remarks>
   public float aimX;
   /// <summary>
   /// Vertical offset relative to this sprite's location at which this light source points.
   /// </summary>
   /// <remarks>
   /// Positive values point down, and negative values point up.
   /// Aim is mostly noticeable when <see cref="apertureFocus"/> is used to focus the light in one direction,
   /// however, if aim is not set, the light will not be visible. The size of the aim vector does not matter
   /// because it is normalized to a unit vector; only the direction matters.
   /// <seealso cref="aimX"/><seealso cref="aimZ"/>
   /// </remarks>
   public float aimY;
   /// <summary>
   /// Depth offset relative to this sprite's location at which this light source points.
   /// </summary>
   /// <remarks>
   /// Positive values point from the layer toward the viewer, and negative numbers point from the viewer
   /// toward the layer, so negative numbers would be more common for this value because light sources are
   /// generally positioned (with <see cref="lightZ"/>) on the same side of the layer as the viewer.
   /// Aim is mostly noticeable when <see cref="apertureFocus"/> is used to focus the light in one direction,
   /// however, if aim is not set, the light will not be visible. The size of the aim vector does not matter
   /// because it is normalized to a unit vector; only the direction matters.
   /// <seealso cref="aimX"/><seealso cref="aimY"/>
   /// </remarks>
   public float aimZ;
   /// <summary>
   /// Depth position of this light source relative to the layer where the sprite resides.
   /// </summary>
   /// <remarks>
   /// Since scrolling game sprites generally don't have a depth coordinate, but depth is important for
   /// lighting, this property is helpful in determining how light shines on the map. Common values
   /// for this property fall approximately in the range of 0.1 to 10.0. Larger values position the
   /// light farther from the map, which is useful when illuminating a large area, for example, to
   /// provide ambient light. This value combined with the <see cref="SpriteBase"/> base class'
   /// <see cref="SpriteBase.x"/> and <see cref="SpriteBase.y"/> values form a 3D coordinate, which work with the <see
   /// cref="aimX"/>, <see cref="aimY"/> and <see cref="aimZ"/> to determine the light's location and
   /// direction.
   /// </remarks>
   public float lightZ;
   /// <summary>
   /// Determines whether and how light is focused in a particular direction.
   /// </summary>
   /// <remarks>A value of 0 causes light to spread over a 180-degree cone (in other words, from a plane)
   /// in the direction determined by <see cref="aimX"/>, <see cref="aimY"/> and <see cref="aimZ"/>.
   /// Values near 1 (but less) focus the light into a narrow beam. Negative values widen the spread.
   /// Negative 1 sheds light in all directions. <seealso cref="apertureSoftness"/>
   /// </remarks>
   public float apertureFocus;
   /// <summary>
   /// Determines how crisp the edges of the light cone are for lights directed by <see cref="apertureFocus"/>.
   /// </summary>
   /// <remarks>This value is essentially added to <see cref="apertureFocus"/> to generate two light
   /// cones that form a gradient from lit to unlit.</remarks>
   public float apertureSoftness;

   /// <summary>
   /// Construct a new light source sprite providing all the properties of the light source and the base sprite.
   /// </summary>
   /// <param name="layer">Layer that contains the sprite.</param>
   /// <param name="x">Initial horizontal coordinate within the layer</param>
   /// <param name="y">Initial vertical coordinate within the layer</param>
   /// <param name="dx">Initial horizontal velocity</param>
   /// <param name="dy">Initial vertical velocity</param>
   /// <param name="state">Initial state</param>
   /// <param name="frame">Initial frame within the initial state</param>
   /// <param name="active">Determines if the sprite is initially active</param>
   /// <param name="solidity">Which solidity definition does the sprite initially react to</param>
   /// <param name="color">Initial color modulation settings</param>
   /// <param name="constantFalloff">Initial value for <see cref="constantFalloff"/></param>
   /// <param name="linearFalloff">Initial value for <see cref="linearFalloff"/></param>
   /// <param name="quadraticFalloff">Initial value for <see cref="quadraticFalloff"/></param>
   /// <param name="aimX">Initial value for <see cref="aimX"/></param>
   /// <param name="aimY">Initial value for <see cref="aimY"/></param>
   /// <param name="aimZ">Initial value for <see cref="aimZ"/></param>
   /// <param name="lightZ">Initial value for <see cref="lightZ"/></param>
   /// <param name="apertureFocus">Initial value for <see cref="apertureFocus"/></param>
   /// <param name="apertureSoftness">Initial value for <see cref="apertureSoftness"/></param>
   public LightSpriteBase(LayerBase layer, double x, double y, double dx, double dy, int state, int frame,
      bool active, Solidity solidity, int color, float constantFalloff, float linearFalloff, float quadraticFalloff,
      float aimX, float aimY, float aimZ, float lightZ, float apertureFocus, float apertureSoftness)
      : base(layer, x, y, dx, dy, state, frame, active, solidity, color)
   {
      this.constantFalloff = constantFalloff;
      this.linearFalloff = linearFalloff;
      this.quadraticFalloff = quadraticFalloff;
      this.aimX = aimX;
      this.aimY = aimY;
      this.aimZ = aimZ;
      this.lightZ = lightZ;
      this.apertureFocus = apertureFocus;
      this.apertureSoftness = apertureSoftness;
   }

   /// <summary>
   /// Rotates the AimX and AimY of this light clockwise (+) or counterclockwise (-).
   /// </summary>
   /// <param name="Offset">Positive values represent clockwise rotation in 100ths of a degree;
   /// negative values represent counterclockwise rotation.</param>
   [Description("Rotates the AimX and AimY of this light clockwise (+) or counterclockwise (-). Offset is in 100ths of a degree.")]
   public void RotateLight(int Offset)
   {
      OpenTK.Matrix3 m3 = OpenTK.Matrix3.CreateRotationZ(Offset * (float)Math.PI / 18000f);
      OpenTK.Vector3 v3 = new OpenTK.Vector3(aimX, aimY, aimZ);
      v3 *= m3;
      aimX = v3.X;
      aimY = v3.Y;
      aimZ = v3.Z;
   }

   static System.Collections.BitArray processedTiles;
   static System.Collections.Generic.Queue<Tuple<int, int>> tileCoords = new System.Collections.Generic.Queue<Tuple<int, int>>();
   static Wall[] walls = new Wall[LightSource.wallsPerLight];
   static OpenTK.Vector3[] wallCoords = new OpenTK.Vector3[LightSource.wallsPerLight * 2];
   static int wallCount;

   [Flags()]
   enum WallType
   {
      Top,
      Bottom,
      Left,
      Right,
      Downhill,
      Uphill,
      DownCeiling,
      UpCeiling
   }

   private class Wall
   {
      public WallType type;
      public int startX;
      public int startY;
      public int lengthInTiles;

      public static void Create(int lightCol, int lightRow, int column, int row, TileShape ts)
      {
         int edgeCoord1 = ts.GetTopSolidPixel(4, 4, 1, 1);
         int edgeCoord2 = ts.GetBottomSolidPixel(4, 4, 1, 1);

         if ((edgeCoord1 == 0) && (edgeCoord2 == 3))
         {
            if (wallCount < walls.Length)
            {
               if (column > lightCol)
                  walls[wallCount++] = new Wall() { type = WallType.Left, startX = column, startY = row, lengthInTiles = 1 };
               else if (column < lightCol)
                  walls[wallCount++] = new Wall() { type = WallType.Right, startX = column, startY = row, lengthInTiles = 1 };
            }

            if (wallCount < walls.Length)
            {
               if (row > lightRow)
                  walls[wallCount++] = new Wall() { type = WallType.Top, startX = column, startY = row, lengthInTiles = 1 };
               else if (row < lightRow)
                  walls[wallCount++] = new Wall() { type = WallType.Bottom, startX = column, startY = row, lengthInTiles = 1 };
            }
         }
         else if (edgeCoord1 == 1) // Downhill
         {
            if ((wallCount < walls.Length) && (column > lightCol))
               walls[wallCount++] = new Wall() { type = WallType.Left, startX = column, startY = row, lengthInTiles = 1 };

            if ((wallCount < walls.Length) && (row < lightRow))
               walls[wallCount++] = new Wall() { type = WallType.Bottom, startX = column, startY = row, lengthInTiles = 1 };

            if ((wallCount < walls.Length) && (lightCol - column >= lightRow - row))
               walls[wallCount++] = new Wall() { type = WallType.Downhill, startX = column, startY = row, lengthInTiles = 1 };
         }
         else if (edgeCoord1 == 2) // Uphill
         {
            if ((wallCount < walls.Length) && (column < lightCol))
               walls[wallCount++] = new Wall() { type = WallType.Right, startX = column, startY = row, lengthInTiles = 1 };

            if ((wallCount < walls.Length) && (row < lightRow))
               walls[wallCount++] = new Wall() { type = WallType.Bottom, startX = column, startY = row, lengthInTiles = 1 };

            if ((wallCount < walls.Length) && (lightCol - column <= row - lightRow))
               walls[wallCount++] = new Wall() { type = WallType.Uphill, startX = column, startY = row, lengthInTiles = 1 };
         }
         else if (edgeCoord2 == 2) // UpCeiling
         {
            if ((wallCount < walls.Length) && (column > lightCol))
               walls[wallCount++] = new Wall() { type = WallType.Left, startX = column, startY = row, lengthInTiles = 1 };

            if ((wallCount < walls.Length) && (row > lightRow))
               walls[wallCount++] = new Wall() { type = WallType.Top, startX = column, startY = row, lengthInTiles = 1 };

            if ((wallCount < walls.Length) && (lightCol - column >= row - lightRow))
               walls[wallCount++] = new Wall() { type = WallType.UpCeiling, startX = column, startY = row, lengthInTiles = 1 };
         }
         else if (edgeCoord2 == 1) // DownCeiling
         {
            if ((wallCount < walls.Length) && (column < lightCol))
               walls[wallCount++] = new Wall() { type = WallType.Right, startX = column, startY = row, lengthInTiles = 1 };

            if ((wallCount < walls.Length) && (row > lightRow))
               walls[wallCount++] = new Wall() { type = WallType.Top, startX = column, startY = row, lengthInTiles = 1 };

            if ((wallCount < walls.Length) && (lightCol - column <= lightRow - row))
               walls[wallCount++] = new Wall() { type = WallType.DownCeiling, startX = column, startY = row, lengthInTiles = 1 };
         }
      }

      /// <summary>
      /// Extend this wall to include the specified tile with the specified shape if possible
      /// </summary>
      /// <param name="tileX">Column in which the tile appears</param>
      /// <param name="tileY">Row in which the tile appears</param>
      /// <param name="ts">Shape of the tile</param>
      /// <returns>0 if this wall cannot extent into the tile without improperly disrupting lighting,
      /// 1 if this wall can extend into the tile, but does not block light to all appropriate areas of the tile,
      /// 2 if this wall exactly obstructs the light it needs to on this tile.</returns>
      public int Extend(int tileX, int tileY, TileShape ts)
      {
         int retVal = 2;
         int edgeCoord;

         switch (type)
         {
            case WallType.Top:
               if ((tileY != startY) || (ts.GetTopSolidPixel(4, 4, 2, 2) > 0))
                  return 0;
               break;
            case WallType.Bottom:
               if ((tileY != startY) || (ts.GetBottomSolidPixel(4, 4, 2, 2) < 3))
                  return 0;
               break;
            case WallType.Left:
               if ((tileX != startX) || (ts.GetLeftSolidPixel(4, 4, 2, 2) > 0))
                  return 0;
               break;
            case WallType.Right:
               if ((tileX != startX) || (ts.GetRightSolidPixel(4, 4, 2, 2) < 3))
                  return 0;
               break;
            case WallType.Downhill:
               edgeCoord = ts.GetTopSolidPixel(4, 4, 1, 1);
               if (((tileX - startX) != (tileY - startY)) || (edgeCoord > 1))
                  return 0;
               if (edgeCoord < 1)
                  retVal = 1;
               break;
            case WallType.Uphill:
               edgeCoord = ts.GetTopSolidPixel(4, 4, 2, 2);
               if (((tileX - startX) != (startY - tileY)) || (edgeCoord > 1))
                  return 0;
               if (edgeCoord < 1)
                  retVal = 1;
               break;
            case WallType.DownCeiling:
               edgeCoord = ts.GetBottomSolidPixel(4, 4, 2, 2);
               if (((tileX - startX) != (tileY - startY)) || (edgeCoord < 2))
                  return 0;
               if (edgeCoord > 2)
                  retVal = 1;
               break;
            case WallType.UpCeiling:
               edgeCoord = ts.GetBottomSolidPixel(4, 4, 1, 1);
               if (((tileX - startX) != (startY - tileY)) || (edgeCoord < 2))
                  return 0;
               if (edgeCoord > 2)
                  retVal = 1;
               break;
         }

         if ((type == WallType.Left) || (type == WallType.Right))
         {
            if (tileY == startY - 1)
            {
               startY--;
               lengthInTiles++;
               return retVal;
            }
            if (tileY == startY + lengthInTiles)
            {
               lengthInTiles++;
               return retVal;
            }
            return 0;
         }
         else
         {
            if (tileX == startX - 1)
            {
               startX--;
               switch (type)
               {
                  case WallType.Downhill:
                  case WallType.DownCeiling:
                     startY--;
                     break;
                  case WallType.Uphill:
                  case WallType.UpCeiling:
                     startY++;
                     break;
               }
               lengthInTiles++;
               return retVal;
            }
            if (tileX == startX + lengthInTiles)
            {
               lengthInTiles++;
               return retVal;
            }
            return 0;
         }
      }
   }

   /// <summary>
   /// Calculate based on this sprite's solidity where light walls should appear within a specified radius.
   /// </summary>
   /// <param name="tileRadius">How many tiles away a solid tile can cause a wall to be generated.
   /// This is a square radius, so not, strictly speaking, a radius.</param>
   /// <remarks>After this function completes, <see cref="WallCoordinates"/> will contain information
   /// about all the walls for this light source. Note that this information is shared among all
   /// light sources, and so should be transferred to the <see cref="Display"/> object before calculating
   /// walls for other light sources. Large radiuses may harm performance without improving results
   /// because there are a limited number of walls that the display can handle. Once this limit is
   /// reached, no more walls are processed regardless of whether the information for them has been
   /// calculated. However, it is possible with larger radius values to extend existing walls farther
   /// because each wall is simply a line that can be extended for any length of colinear tiles.</remarks>
   public void GenerateWalls(int tileRadius)
   {
      int tileX, tileY, startX, startY;
      int tw = layer.Tileset.TileWidth;
      int th = layer.Tileset.TileHeight;
      tileX = startX = (int)((x + SolidWidth / 2) / tw);
      tileY = startY = (int)((y + SolidHeight / 2) / th);

      int tileCount = layer.VirtualColumns * layer.VirtualRows;
      if ((processedTiles == null) || (processedTiles.Length < tileCount))
         processedTiles = new System.Collections.BitArray(tileCount);
      else
         processedTiles.SetAll(false);
      tileCoords.Clear();

      processedTiles[tileY * layer.VirtualColumns + tileX] = true;

      wallCount = 0;

      do
      {
         int offsetV = Math.Sign(tileY - startY);
         int offsetH = Math.Sign(tileX - startX);

         if ((Math.Abs(tileX - startX) < tileRadius) && (Math.Abs(tileY - startY) < tileRadius))
         {
            bool horizontalFirst = false;

            if (offsetH > offsetV)
               horizontalFirst = true;

            if (offsetH == 0) offsetH = -1;
            if (offsetV == 0) offsetV = -1;

            if (horizontalFirst)
            {
               EnqueueCoord(tileX + offsetH, tileY);
               EnqueueCoord(tileX - offsetH, tileY);
            }

            EnqueueCoord(tileX, tileY + offsetV);
            EnqueueCoord(tileX, tileY - offsetV);

            if (!horizontalFirst)
            {
               EnqueueCoord(tileX + offsetH, tileY);
               EnqueueCoord(tileX - offsetH, tileY);
            }
         }

         Tuple<int, int> currentCoord = tileCoords.Dequeue();
         tileX = currentCoord.Item1;
         tileY = currentCoord.Item2;

         TileShape ts = GetTileShapeAt(tileX, tileY);

         int wallMatch = 0;

         for (int i = 0; i < wallCount; i++)
            if (walls[i].Extend(tileX, tileY, ts) == 2)
            {
               wallMatch = 2;
               break;
            }

         if ((wallMatch != 2) && (wallCount < walls.Length))
            Wall.Create(startX, startY, tileX, tileY, ts);
      } while (tileCoords.Count > 0);

      for (int i = 0; i < wallCount; i++)
      {
         switch (walls[i].type)
         {
            case WallType.DownCeiling:
            case WallType.Downhill:
            case WallType.Left:
            case WallType.Top:
               wallCoords[i * 2] = new OpenTK.Vector3(walls[i].startX * tw, walls[i].startY * th, 1);
               break;
            case WallType.Bottom:
               wallCoords[i * 2] = new OpenTK.Vector3(walls[i].startX * tw, walls[i].startY * th + th, 1);
               break;
            case WallType.UpCeiling:
            case WallType.Uphill:
               wallCoords[i * 2] = new OpenTK.Vector3(walls[i].startX * tw, walls[i].startY * th + th, 1);
               break;
            case WallType.Right:
               wallCoords[i * 2] = new OpenTK.Vector3(walls[i].startX * tw + tw, walls[i].startY * th, 1);
               break;
         }
         switch (walls[i].type)
         {
            case WallType.Top:
               wallCoords[i * 2 + 1] = new OpenTK.Vector3((walls[i].startX + walls[i].lengthInTiles) * tw, walls[i].startY * th, 1);
               break;
            case WallType.UpCeiling:
            case WallType.Uphill:
               wallCoords[i * 2 + 1] = new OpenTK.Vector3((walls[i].startX + walls[i].lengthInTiles) * tw, (walls[i].startY - walls[i].lengthInTiles + 1) * th, 1);
               break;
            case WallType.Bottom:
               wallCoords[i * 2 + 1] = new OpenTK.Vector3((walls[i].startX + walls[i].lengthInTiles) * tw, walls[i].startY * th + th, 1);
               break;
            case WallType.Downhill:
            case WallType.DownCeiling:
               wallCoords[i * 2 + 1] = new OpenTK.Vector3((walls[i].startX + walls[i].lengthInTiles) * tw, (walls[i].startY + walls[i].lengthInTiles) * th, 1);
               break;
            case WallType.Left:
               wallCoords[i * 2 + 1] = new OpenTK.Vector3(walls[i].startX * tw, (walls[i].startY + walls[i].lengthInTiles) * th, 1);
               break;
            case WallType.Right:
               wallCoords[i * 2 + 1] = new OpenTK.Vector3(walls[i].startX * tw + tw, (walls[i].startY + walls[i].lengthInTiles) * th, 1);
               break;
         }
      }
   }

   /// <summary>
   /// Provides access to the current set of light walls generated by <see cref="GenerateWalls(int)"/>.
   /// </summary>
   /// <remarks>Note that these coordinates are shared by all light sources, and shoudl therefore
   /// be transferred or processed before generating coordinates for another light source. For
   /// performance reasons, the size of this collection is constant, and the number of actual
   /// light sources calculated is determined with <see cref="WallCoordinateCount"/>. The
   /// coordinates in this list are always paired with the first coordinate representing one end
   /// of a wall and the second coordinate representing the other end of a wall.
   /// <seealso cref="Display.SetLightSource(int, OpenTK.Vector2, OpenTK.Vector3, System.Drawing.Color, float, float, float, float, float, float, OpenTK.Vector3[], int)"/>
   /// </remarks>
   public static OpenTK.Vector3[] WallCoordinates
   {
      get
      {
         return wallCoords;
      }
   }

   /// <summary>
   /// Returns how many coordinates were generated by the last call to <see cref="GenerateWalls(int)"/>.
   /// </summary>
   /// <remarks>The number of walls is always half the number of coordinates.
   /// <seealso cref="Display.SetLightSource(int, OpenTK.Vector2, OpenTK.Vector3, System.Drawing.Color, float, float, float, float, float, float, OpenTK.Vector3[], int)"/>
   /// </remarks>
   public static int WallCoordinateCount
   {
      get
      {
         return wallCount * 2;
      }
   }

   private void EnqueueCoord(int x, int y)
   {
      if ((x >= 0) && (y >= 0) && (x < layer.VirtualColumns) && (y < layer.VirtualRows) && !processedTiles[y * layer.VirtualColumns + x])
      {
         tileCoords.Enqueue(new Tuple<int, int>(x, y));
         processedTiles[y * layer.VirtualColumns + x] = true;
      }
   }

   private TileShape GetTileShapeAt(int x, int y)
   {
      if ((x < 0) || (y < 0) || (x >= layer.VirtualColumns) || (y >= layer.VirtualColumns))
         return EmptyTileShape.Value;
      return m_solidity.GetCurrentTileShape(layer.GetTile(x, y));
   }
}