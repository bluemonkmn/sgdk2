using System;
using System.ComponentModel;

public abstract partial class LightSpriteBase : SpriteBase
{
   public float constantFalloff;
   public float linearFalloff;
   public float quadraticFalloff;
   public float aimX;
   public float aimY;
   public float aimZ;
   public float lightZ;
   public float apertureFocus;
   public float apertureSoftness;

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

   public static OpenTK.Vector3[] WallCoordinates
   {
      get
      {
         return wallCoords;
      }
   }

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