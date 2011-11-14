function MapLayer(tileset, columns, rows, virtualColumns, virtualRows, offsetX, offsetY, priority, tileData) {
   this.tileset = tileset;
   this.columns = columns;
   this.rows = rows;
   this.offsetX = offsetX;
   this.offsetY = offsetY;
   this.currentX = offsetX;
   this.currentY = offsetY;
   if(tileData.length < columns * rows * 2)
      this.tiles = DecodeData1(tileData);
   else
      this.tiles = DecodeData2(tileData);
   this.virtualColumns = virtualColumns ? virtualColumns : columns;
   this.virtualRows = virtualRows ? virtualRows : rows;
}

MapLayer.prototype.getTile = function(x, y) {
   return this.tiles[(y % this.rows) * this.columns + (x % this.columns)];
};

MapLayer.prototype.draw = function(ctx) {
   var tileWidth = this.tileset.tileWidth;
   var tileHeight = this.tileset.tileHeight;
   var lastRow = Math.floor((viewHeight - this.currentY - 1) / tileHeight);
   if (lastRow >= this.virtualRows) lastRow = this.virtualRows - 1;
   var lastCol = Math.floor((viewWidth - this.currentX - 1) / tileWidth);
   if (lastCol >= this.virtualColumns) lastCol = this.virtualColumns - 1;
   for(y = Math.floor(-this.currentY / tileHeight), y = y < 0 ? 0 : y; y <= lastRow; y++) {
      for(x = Math.floor(-this.currentX / tileWidth), x = x < 0 ? 0 : x; x <= lastCol; x++) {
         var tile = this.tileset.tiles[this.getTile(x, y)];
         if (tile == null) continue;
         var drx = x * tileWidth + this.currentX;
         var dry = y * tileHeight + this.currentY;
         if (typeof tile == 'number')
            this.tileset.frameSet.frames[tile % this.tileset.frameSet.frames.length].draw(ctx, drx, dry);
         else {
            var frames;
            if (tile instanceof AnimTile)
               frames = tile.getCurFrames();
            else
               frames = tile;
            if (typeof frames == 'number')
               this.tileset.frameSet.frames[frames % this.tileset.frameSet.frames.length].draw(ctx, drx, dry);
            else
               for(var fi = 0; fi < frames.length; fi++)
                  this.tileset.frameSet.frames[frames[fi] % this.tileset.frameSet.frames.length].draw(ctx, drx, dry);
         }
      }
   }
   for(si = 0; si < this.sprites.length; si++) {
      var curSprite = this.sprites[si];
      if (!curSprite.active) continue;
      var frames = curSprite.getCurFrames();
      if (frames == null) continue;
      var frameSet = curSprite.states[curSprite.state].frameSet;
      if (typeof frames == 'number')
         frameSet.frames[frames % frameSet.frames.length].draw(ctx, curSprite.x + this.currentX, curSprite.y + this.currentY);
      else
         for(var fi = 0; fi < frames.length; fi++)
            frameSet.frames[frames[fi] % frameSet.frames.length].draw(ctx, curSprite.x + this.currentX, curSprite.y + this.currentY);
   }
};

MapLayer.noSolid = -2000000000;

MapLayer.prototype.getTopSolidPixel = function(areaX, areaY, areaWidth, areaHeight, solidity) {
   var topTile = Math.floor(areaY / this.tileset.tileHeight);
   var bottomTile = Math.floor((areaY + areaHeight - 1) / this.tileset.tileHeight);
   var leftTile = Math.floor(areaX / this.tileset.tileWidth);
   var rightTile = Math.floor((areaX + areaWidth - 1) / this.tileset.tileWidth);
   var outOfBounds = false;
   if ((topTile < 0) || (topTile >= this.virtualRows) || (bottomTile < 0) || (bottomTile >= this.virtualRows)
      || (leftTile < 0) || (leftTile >= this.virtualColumns) || (rightTile < 0) || (rightTile >= this.virtualColumns))
      outOfBounds = true;
   var minTileTop = areaY % this.tileset.tileHeight;
   var tileLeft = leftTile * this.tileset.tileWidth;
   for (var y = topTile; y <= bottomTile; y++) {
      if (rightTile == leftTile) {
         var topMost;
         if (outOfBounds && ((leftTile < 0) || (leftTile >= this.virtualColumns) || (y < 0) || (y >= this.virtualRows)))
            topMost = 0;
         else
            topMost = solidity.getCurrentTileShape(this.tileset, this.getTile(leftTile,y)).getTopSolidPixel(
               this.tileset.tileWidth, this.tileset.tileHeight, areaX - tileLeft,
               areaX + areaWidth - 1 - tileLeft);
         if ((topMost != TileShape.maxValue) && ((y > topTile) || (topMost >= minTileTop))) {
            var result = topMost + y * this.tileset.tileHeight;
            if (result < areaY + areaHeight)
               return result;
            else
               return MapLayer.noSolid;
         }
      } else {
         var topMost;
         if (outOfBounds && ((leftTile < 0) || (leftTile >= this.virtualColumns) || (y < 0) || (y >= this.virtualRows)))
            topMost = 0;
         else
            topMost = solidity.getCurrentTileShape(this.tileset, this.getTile(leftTile, y)).getTopSolidPixel(
                this.tileset.tileWidth, this.tileset.tileHeight, areaX - tileLeft, this.tileset.tileWidth - 1);
         if ((y == topTile) && (topMost < minTileTop))
            topMost = TileShape.maxValue;
         var top;
         for (var x = leftTile + 1; x < rightTile; x++) {
            if (outOfBounds && ((x < 0) || (x >= this.virtualColumns) || (y < 0) || (y >= this.virtualRows)))
               top = 0;
            else
               top = solidity.getCurrentTileShape(this.tileset, this.getTile(x,y)).getTopSolidPixel(
                  this.tileset.tileWidth, this.tileset.tileHeight, 0, this.tileset.tileWidth - 1);
            if ((top < topMost) && ((y > topTile) || (top >= minTileTop)))
               topMost = top;
         }
         if (outOfBounds && ((rightTile < 0) || (rightTile >= this.virtualColumns) || (y < 0) || (y >= this.virtualRows)))
            top = 0;
         else
            top = solidity.getCurrentTileShape(this.tileset, this.getTile(rightTile,y)).getTopSolidPixel(
               this.tileset.tileWidth, this.tileset.tileHeight, 0, (areaX + areaWidth - 1) % this.tileset.tileWidth);
         if ((top < topMost) && ((y > topTile) || (top >= minTileTop)))
            topMost = top;
         if (topMost != TileShape.maxValue) {
            var result = topMost + y * this.tileset.tileHeight;
            if (result < areaY + areaHeight)
               return result;
            else
               return MapLayer.noSolid;
         }
      }
   }
   return MapLayer.noSolid;
};

MapLayer.prototype.getBottomSolidPixel = function(areaX, areaY, areaWidth, areaHeight, solidity) {
   var topTile = Math.floor(areaY / this.tileset.tileHeight);
   var bottomTile = Math.floor((areaY + areaHeight - 1) / this.tileset.tileHeight);
   var leftTile = Math.floor(areaX / this.tileset.tileWidth);
   var rightTile = Math.floor((areaX + areaWidth - 1) / this.tileset.tileWidth);
   var outOfBounds = false;
   if ((topTile < 0) || (topTile >= this.virtualRows) || (bottomTile < 0) || (bottomTile >= this.virtualRows)
      || (leftTile < 0) || (leftTile >= this.virtualColumns) || (rightTile < 0) || (rightTile >= this.virtualColumns))
      outOfBounds = true;
   var maxTileBottom = (areaY + areaHeight - 1) % this.tileset.tileHeight;
   var tileLeft = leftTile * this.tileset.tileWidth;
   for (var y = bottomTile; y >= topTile; y--) {
      if (rightTile == leftTile) {
         var bottomMost;
         if (outOfBounds && ((leftTile < 0) || (leftTile >= this.virtualColumns) || (y < 0) || (y >= this.virtualRows)))
            bottomMost = this.tileset.tileHeight - 1;
         else
            bottomMost = solidity.getCurrentTileShape(this.tileset, this.getTile(leftTile,y)).getBottomSolidPixel(
               this.tileset.tileWidth, this.tileset.tileHeight, areaX - tileLeft,
               areaX + areaWidth - 1 - tileLeft);
         if ((bottomMost != TileShape.minValue) && ((y < bottomTile) || (bottomMost <= maxTileBottom))) {
            var result = bottomMost + y * this.tileset.tileHeight;
            if (result >= areaY)
               return result;
            else
               return MapLayer.noSolid;
         }
      } else {
         var bottomMost;
         if (outOfBounds && ((leftTile < 0) || (leftTile >= this.virtualColumns) || (y < 0) || (y >= this.virtualRows)))
            bottomMost = this.tileset.tileHeight - 1;
         else
            bottomMost = solidity.getCurrentTileShape(this.tileset, this.getTile(leftTile, y)).getBottomSolidPixel(
               this.tileset.tileWidth, this.tileset.tileHeight, areaX - tileLeft, this.tileset.tileWidth - 1);
         if ((y == bottomTile) && (bottomMost > maxTileBottom))
            bottomMost = TileShape.minValue;
         var bottom;
         for (var x = leftTile + 1; x < rightTile; x++) {
            if (outOfBounds && ((x < 0) || (x >= this.virtualColumns) || (y < 0) || (y >= this.virtualRows)))
               bottom = this.tileset.tileHeight - 1;
            else
               bottom = solidity.getCurrentTileShape(this.tileset, this.getTile(x,y)).getBottomSolidPixel(
                  this.tileset.tileWidth, this.tileset.tileHeight, 0, this.tileset.tileWidth - 1);
            if ((bottom > bottomMost) && ((y < bottomTile) || (bottom <= maxTileBottom)))
               bottomMost = bottom;
         }
         if (outOfBounds && ((rightTile < 0) || (rightTile >= this.virtualColumns) || (y < 0) || (y >= this.virtualRows)))
            bottom = this.tileset.tileHeight - 1;
         else
            bottom = solidity.getCurrentTileShape(this.tileset, this.getTile(rightTile,y)).getBottomSolidPixel(
               this.tileset.tileWidth, this.tileset.tileHeight, 0, (areaX + areaWidth - 1) % this.tileset.tileWidth);
         if ((bottom > bottomMost) && ((y < bottomTile) || (bottom <= maxTileBottom)))
            bottomMost = bottom;
         if (bottomMost != TileShape.minValue) {
            var result = bottomMost + y * this.tileset.tileHeight;
            if (result >= areaY)
               return result;
            else
               return MapLayer.noSolid;
         }
      }
   }
   return MapLayer.noSolid;
};

MapLayer.prototype.getLeftSolidPixel = function(areaX, areaY, areaWidth, areaHeight, solidity) {
   var topTile = Math.floor(areaY / this.tileset.tileHeight);
   var bottomTile = Math.floor((areaY + areaHeight - 1) / this.tileset.tileHeight);
   var leftTile = Math.floor(areaX / this.tileset.tileWidth);
   var rightTile = Math.floor((areaX + areaWidth - 1) / this.tileset.tileWidth);
   var outOfBounds = false;
   if ((topTile < 0) || (topTile >= this.virtualRows) || (bottomTile < 0) || (bottomTile >= this.virtualRows)
      || (leftTile < 0) || (leftTile >= this.virtualColumns) || (rightTile < 0) || (rightTile >= this.virtualColumns))
      outOfBounds = true;
   var minTileLeft = areaX % this.tileset.tileWidth;
   var tileTop = topTile * this.tileset.tileHeight;
   for (var x = leftTile; x <= rightTile; x++) {
      if (bottomTile == topTile){
         var leftMost;
         if (outOfBounds && ((topTile < 0) || (topTile >= this.virtualRows) || (x < 0) || (x >= this.virtualColumns)))
            leftMost = 0;
         else
            leftMost = solidity.getCurrentTileShape(this.tileset, this.getTile(x, topTile)).getLeftSolidPixel(
               this.tileset.tileWidth, this.tileset.tileHeight, areaY - tileTop,
               areaY + areaHeight - 1 - tileTop);
         if ((leftMost != TileShape.maxValue) && ((x > leftTile) || (leftMost >= minTileLeft))) {
            var result = leftMost + x * this.tileset.tileWidth;
            if (result < areaX + areaWidth)
               return result;
            else
               return MapLayer.noSolid;
         }
      } else {
         var leftMost;
         if (outOfBounds && ((topTile < 0) || (topTile >= this.virtualRows) || (x < 0) || (x >= this.virtualColumns)))
            leftMost = 0;
         else
            leftMost = solidity.getCurrentTileShape(this.tileset, this.getTile(x, topTile)).getLeftSolidPixel(
                this.tileset.tileWidth, this.tileset.tileHeight, areaY - tileTop, this.tileset.tileHeight - 1);
         if ((x == leftTile) && (leftMost < minTileLeft))
            leftMost = TileShape.maxValue;
         var left;
         for (var y = topTile + 1; y < bottomTile; y++) {
            if (outOfBounds && ((x < 0) || (x >= this.virtualColumns) || (y < 0) || (y >= this.virtualRows)))
               left = 0;
            else
               left = solidity.getCurrentTileShape(this.tileset, this.getTile(x,y)).getLeftSolidPixel(
                  this.tileset.tileWidth, this.tileset.tileHeight, 0, this.tileset.tileHeight - 1);
            if ((left < leftMost) && ((x > leftTile) || (left >= minTileLeft)))
               leftMost = left;
         }
         if (outOfBounds && ((bottomTile < 0) || (bottomTile >= this.virtualRows) || (x < 0) || (x >= this.virtualColumns)))
            left = 0;
         else
            left = solidity.getCurrentTileShape(this.tileset, this.getTile(x, bottomTile)).getLeftSolidPixel(
               this.tileset.tileWidth, this.tileset.tileHeight, 0, (areaY + areaHeight - 1) % this.tileset.tileHeight);
         if ((left < leftMost) && ((x > leftTile) || (left >= minTileLeft)))
            leftMost = left;
         if (leftMost != TileShape.maxValue) {
            var result = leftMost + x * this.tileset.tileWidth;
            if (result < areaX + areaWidth)
               return result;
            else
               return MapLayer.noSolid;
         }
      }
   }
   return MapLayer.noSolid;
};

MapLayer.prototype.getRightSolidPixel = function(areaX, areaY, areaWidth, areaHeight, solidity) {
   var topTile = Math.floor(areaY / this.tileset.tileHeight);
   var bottomTile = Math.floor((areaY + areaHeight - 1) / this.tileset.tileHeight);
   var leftTile = Math.floor(areaX / this.tileset.tileWidth);
   var rightTile = Math.floor((areaX + areaWidth - 1) / this.tileset.tileWidth);
   var outOfBounds = false;
   if ((topTile < 0) || (topTile >= this.virtualRows) || (bottomTile < 0) || (bottomTile >= this.virtualRows)
      || (leftTile < 0) || (leftTile >= this.virtualColumns) || (rightTile < 0) || (rightTile >= this.virtualColumns))
      outOfBounds = true;
   var maxTileRight = (areaX + areaWidth - 1) % this.tileset.tileWidth;
   var tileTop = topTile * this.tileset.tileHeight;
   for (var x = rightTile; x >= leftTile; x--) {
      if (bottomTile == topTile){
         var rightMost;
         if (outOfBounds && ((topTile < 0) || (topTile >= this.virtualRows) || (x < 0) || (x >= this.virtualColumns)))
            rightMost = this.tileset.tileWidth - 1;
         else
            rightMost = solidity.getCurrentTileShape(this.tileset, this.getTile(x, topTile)).getRightSolidPixel(
               this.tileset.tileWidth, this.tileset.tileHeight, areaY - tileTop,
               areaY + areaHeight - 1 - tileTop);
         if ((rightMost != TileShape.minValue) && ((x < rightTile) || (rightMost <= maxTileRight))) {
            var result = rightMost + x * this.tileset.tileWidth;
            if (result >= areaX)
               return result;
            else
               return MapLayer.noSolid;
         }
      } else {
         var rightMost;
         if (outOfBounds && ((topTile < 0) || (topTile >= this.virtualRows) || (x < 0) || (x >= this.virtualColumns)))
            rightMost = this.tileset.tileWidth - 1;
         else
            rightMost = solidity.getCurrentTileShape(this.tileset, this.getTile(x, topTile)).getRightSolidPixel(
                this.tileset.tileWidth, this.tileset.tileHeight, areaY - tileTop, this.tileset.tileHeight - 1);
         if ((x == rightTile) && (rightMost > maxTileRight))
            rightMost = TileShape.minValue;
         var right;
         for (var y = topTile + 1; y < bottomTile; y++) {
            if (outOfBounds && ((x < 0) || (x >= this.virtualColumns) || (y < 0) || (y >= this.virtualRows)))
               right = this.tileset.tileWidth - 1;
            else
               right = solidity.getCurrentTileShape(this.tileset, this.getTile(x,y)).getRightSolidPixel(
                  this.tileset.tileWidth, this.tileset.tileHeight, 0, this.tileset.tileHeight - 1);
            if ((right > rightMost) && ((x < rightTile) || (right <= maxTileRight)))
               rightMost = right;
         }
         if (outOfBounds && ((bottomTile < 0) || (bottomTile >= this.virtualRows) || (x < 0) || (x >= this.virtualColumns)))
            right = this.tileset.tileWidth - 1;
         else
            right = solidity.getCurrentTileShape(this.tileset, this.getTile(x, bottomTile)).getRightSolidPixel(
               this.tileset.tileWidth, this.tileset.tileHeight, 0, (areaY + areaHeight - 1) % this.tileset.tileHeight);
         if ((right > rightMost) && ((x < rightTile) || (right <= maxTileRight)))
            rightMost = right;
         if (rightMost != TileShape.minValue) {
            var result = rightMost + x * this.tileset.tileWidth;
            if (result >= areaX)
               return result;
            else
               return MapLayer.noSolid;
         }
      }
   }
   return MapLayer.noSolid;
};
