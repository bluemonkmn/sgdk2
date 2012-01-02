function TileShape()
{
}

TileShape.maxValue = 32767;
TileShape.minValue = -32768;

TileShape.empty = new TileShape();
TileShape.empty.getTopSolidPixel = function(width, height, min, max) { return TileShape.maxValue; };
TileShape.empty.getLeftSolidPixel = function(width, height, min, max) { return TileShape.maxValue; };
TileShape.empty.getRightSolidPixel = function(width, height, min, max) { return TileShape.minValue; };
TileShape.empty.getBottomSolidPixel = function(width, height, min, max) { return TileShape.minValue; };

TileShape.solid = new TileShape();
TileShape.prototype.getTopSolidPixel = function(width, height, min, max) { return 0; };
TileShape.prototype.getLeftSolidPixel = function(width, height, min, max) { return 0; };
TileShape.prototype.getRightSolidPixel = function(width, height, min, max) { return width - 1; };
TileShape.prototype.getBottomSolidPixel = function(width, height, min, max) { return height - 1; };

TileShape.uphill = new TileShape();
TileShape.uphill.getTopSolidPixel = function(width, height, min, max) { return Math.floor(height * (width-max-1) / width); };
TileShape.uphill.getLeftSolidPixel = function(width, height, min, max) { return Math.floor(width * (height-max-1) / height); };

TileShape.downhill = new TileShape();
TileShape.downhill.getTopSolidPixel = function(width, height, min, max) { return Math.floor(min * height / width); };
TileShape.downhill.getRightSolidPixel = function(width, height, min, max) { return Math.floor(width - (height - max - 1) * width / height - 1); };

TileShape.upCeiling = new TileShape();
TileShape.upCeiling.getRightSolidPixel = function(width, height, min, max) { return Math.floor(((height - min) * width - 1) / height); };
TileShape.upCeiling.getBottomSolidPixel = function(width, height, min, max) { return Math.floor(((width - min) * height - 1) / width); };

TileShape.downCeiling = new TileShape();
TileShape.downCeiling.getLeftSolidPixel = function(width, height, min, max) { return Math.floor(min * width / height); };
TileShape.downCeiling.getBottomSolidPixel = function(width, height, min, max) { return Math.floor(height - (width - max - 1) * height / width - 1); };

TileShape.uphillRight = new TileShape();
TileShape.uphillRight.getTopSolidPixel = function(width, height, min, max) { return Math.floor(height * (width - max - 1) / width / 2); };
TileShape.uphillRight.getLeftSolidPixel = function(width, height, min, max) { return Math.floor((max * 2 >= height - 2) ? 0 : width * (height - max * 2 - 2) / height); };

TileShape.uphillLeft = new TileShape();
TileShape.uphillLeft.getTopSolidPixel = function(width, height, min, max) { return Math.floor(height * (width - max - 1) / width / 2 + height / 2); };
TileShape.uphillLeft.getLeftSolidPixel = function(width, height, min, max) { return Math.floor(((max + 1) * 2 <= height)?TileShape.maxValue:width * (height - max - 1) * 2 / height); };
TileShape.uphillLeft.getRightSolidPixel = function(width, height, min, max) { return Math.floor(((max + 1) * 2 <= height)?TileShape.minValue:width - 1); };

TileShape.downhillLeft = new TileShape();
TileShape.downhillLeft.getTopSolidPixel = function(width, height, min, max) { return Math.floor(min * height / width / 2); };
TileShape.downhillLeft.getRightSolidPixel = function(width, height, min, max) { return Math.floor(((max + 1) * 2 > height) ? width - 1 : width * 2 - (height - max - 1) * width * 2 / height - 1); };

TileShape.downhillRight = new TileShape();
TileShape.downhillRight.getTopSolidPixel = function(width, height, min, max) { return Math.floor((height + min * height / width) / 2); };
TileShape.downhillRight.getLeftSolidPixel = function(width, height, min, max) { return Math.floor(((min + 1) * 2 <= height) ? TileShape.maxValue : 0); };
TileShape.downhillRight.getRightSolidPixel = function(width, height, min, max) { return Math.floor(((max + 1) * 2 <= height) ? TileShape.minValue : width - (height - max - 1) * 2 * width / height - 1); };

TileShape.topSolid = new TileShape();
TileShape.topSolid.getLeftSolidPixel = TileShape.empty.getLeftSolidPixel;
TileShape.topSolid.getRightSolidPixel = TileShape.empty.getRightSolidPixel;
TileShape.topSolid.getBottomSolidPixel = TileShape.empty.getBottomSolidPixel;

function TileCategory(tilesetMembership) {
   this.membership = new Object();
   for(var tsIndex = 0; tsIndex < tilesetMembership.length; tsIndex++) {
      var tsMemberLookup = new Array();
      var tsMemberList = tilesetMembership[tsIndex].membership;
      this.membership[tilesetMembership[tsIndex].tileset.name] = tsMemberLookup;
      for(var tileIndex = 0; tileIndex < tsMemberList.length; tileIndex++) {
         if (typeof tsMemberList[tileIndex] == 'number')
            tsMemberLookup[tsMemberList[tileIndex]] = true;
         else
            tsMemberLookup[tsMemberList[tileIndex].tileIndex] = tsMemberList[tileIndex].frames;
      }
   }
}

TileCategory.prototype.isTileMember = function(tileset, tileIndex) {
   var membership = this.membership[tileset.name];
   if (membership == null)
      return false;
   var member = membership[tileIndex];
   if (member == true) return true;
   if (member == null) return false;
   return member.indexOf(tileset.tiles[tileIndex].getCurFrameIndex()) > -1;
};

function Solidity(mapping) {
   this.mapping = mapping;
};

Solidity.prototype.getCurrentTileShape = function(tileset, tileIndex) {
   for(var i = 0; i < this.mapping.length; i++) {
      if (this.mapping[i].tileCategory.isTileMember(tileset, tileIndex))
         return this.mapping[i].tileShape;
   }
   return TileShape.empty;
};
