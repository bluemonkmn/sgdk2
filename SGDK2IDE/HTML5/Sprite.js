function SpriteState(solidWidth, solidHeight, frameSet, bounds, frames) {
   this.solidWidth = solidWidth;
   this.solidHeight = solidHeight;
   this.frameSet = frameSet;
   this.bounds = bounds;
   this.frames = frames;
   this.totalDuration = frames ? frames[frames.length - 1].accumulatedDuration : 0;
}

function Sprite(layer, x, y, dx, dy, state, frame, active, priority, solidity) {
   this.layer = layer;
   this.x = x;
   this.y = y;
   this.dx = dx;
   this.dy = dy;
   this.state = state;
   this.frame = frame;
   this.active = active;
   this.priority = priority;
   this.solidity = solidity;
   this.ridingOn = null;
   this.localDX = null;
}

Sprite.prototype.getCurFrames = function() {
   var curState = this.states[this.state];
   if (curState.frames == null) return null;
   for(var i = 0; i < curState.frames.length; i++) {
      if((this.frame % curState.totalDuration) < curState.frames[i].accumulatedDuration) return curState.frames[i].subFrames;
   }
   return curState.frames[curState.frames.length - 1].subFrames;
};

Sprite.prototype.getSolidWidth = function() {
   return this.states[this.state].solidWidth;
};

Sprite.prototype.getSolidHeight = function() {
   return this.states[this.state].solidHeight;
};

Sprite.prototype.reactToSolid = function() {
   var hit = false;
   var dyOrig = this.dy;
   var dxOrig = this.dx;

   var proposedPixelY2 = Math.ceil(this.y + this.dy);
   var pixelX = Math.floor(this.x);
   var pixelY = Math.floor(this.y);
   var solidWidth = this.getSolidWidth();
   var solidHeight = this.getSolidHeight();
   var proposedPixelX = Math.floor(this.x + this.dx);
   var proposedPixelY = Math.floor(this.y + this.dy);
   var solidPixelWidth = solidWidth + Math.ceil(this.x) - pixelX;
   if (this.dy > 0)
   {
      var ground = this.layer.getTopSolidPixel(pixelX, pixelY + solidHeight, solidPixelWidth, proposedPixelY2 - pixelY, this.solidity);
      if (ground != MapLayer.noSolid)
      {
         this.dy = ground - solidHeight - this.y;
         hit = true;
      }
   }
   else if (this.dy < 0)
   {
      var ceiling = this.layer.getBottomSolidPixel(pixelX, proposedPixelY, solidPixelWidth, pixelY - proposedPixelY, this.solidity);
      if (ceiling != MapLayer.noSolid)
      {
         this.dy = ceiling + 1 - this.y;
         hit = true;
      }
   }

   proposedPixelY = Math.floor(this.y + this.dy);

   if (this.dx > 0)
   {
      var proposedPixelX2 = Math.ceil(this.x + this.dx);
      var pixelX2 = Math.ceil(this.x);
      var rightwall = this.layer.getLeftSolidPixel(pixelX2 + solidWidth, proposedPixelY, proposedPixelX2 - pixelX2, solidHeight, this.solidity);
      var hitWall = false;
      if (rightwall != MapLayer.noSolid)
      {
         var maxSlopeProposedY = Math.floor(this.y + this.dy - this.dx);
         var slopedFloor = this.layer.getTopSolidPixel(pixelX2 + solidWidth, maxSlopeProposedY + solidHeight, proposedPixelX2 - pixelX2, proposedPixelY - maxSlopeProposedY, this.solidity);
         if (slopedFloor != MapLayer.noSolid)
         {
            var ceiling = this.layer.getBottomSolidPixel(pixelX2, slopedFloor - solidHeight, solidWidth, proposedPixelY + solidHeight - slopedFloor, this.solidity);
            if ((ceiling == MapLayer.noSolid) && (this.ridingOn == null))
            {
               var rightwall2 = this.layer.getLeftSolidPixel(pixelX2 + solidWidth, slopedFloor - solidHeight, proposedPixelX2 - pixelX2, solidHeight, this.solidity);
               if (rightwall2 == MapLayer.noSolid)
                  this.dy = dyOrig = slopedFloor - solidHeight - 1 - this.y;
               else
                  hitWall = true;
            }
            else
               hitWall = true;
         }
         else
         {
            maxSlopeProposedY = Math.floor(this.y + this.dy + this.dx);
            var slopedCeiling = this.layer.getBottomSolidPixel(pixelX2 + solidWidth, proposedPixelY, proposedPixelX2 - pixelX2, maxSlopeProposedY - proposedPixelY, this.solidity);
            if (slopedCeiling != MapLayer.noSolid)
            {
               slopedCeiling++;
               var floor = this.layer.getTopSolidPixel(pixelX2, proposedPixelY + solidHeight, solidWidth, slopedCeiling - proposedPixelY, this.solidity);
               if ((floor == MapLayer.noSolid) && (this.ridingOn == null))
               {
                  var rightwall2 = this.layer.getLeftSolidPixel(pixelX2 + solidWidth, slopedCeiling, proposedPixelX2 - pixelX2, solidHeight, this.solidity);
                  if (rightwall2 == MapLayer.noSolid)
                     this.dy = dyOrig = slopedCeiling - this.y;
                  else
                     hitWall = true;
               }
               else
                  hitWall = true;
            }
            else
               hitWall = true;
         }
         if (hitWall)
         {
            this.dx = rightwall - solidWidth - this.x;
         }
         hit = true;
      }
   }
   else if (this.dx < 0)
   {
      var leftwall = this.layer.getRightSolidPixel(proposedPixelX, proposedPixelY, pixelX - proposedPixelX, solidHeight, this.solidity);
      var hitWall = false;
      if (leftwall != MapLayer.noSolid)
      {
         var maxSlopeProposedY = Math.floor(this.y + this.dy + this.dx);
         var slopedFloor = this.layer.getTopSolidPixel(proposedPixelX, maxSlopeProposedY + solidHeight, pixelX - proposedPixelX, proposedPixelY - maxSlopeProposedY, this.solidity);
         if (slopedFloor != MapLayer.noSolid)
         {
            var ceiling = this.layer.getBottomSolidPixel(pixelX, slopedFloor - solidHeight, solidWidth, proposedPixelY + solidHeight - slopedFloor, this.solidity);
            if ((ceiling == MapLayer.noSolid) && (this.ridingOn == null))
            {
               var leftwall2 = this.layer.getRightSolidPixel(proposedPixelX, slopedFloor - solidHeight, pixelX - proposedPixelX, solidHeight, this.solidity);
               if (leftwall2 == MapLayer.noSolid)
                  this.dy = dyOrig = slopedFloor - solidHeight - 1 - this.y;
               else
                  hitWall = true;
            }
            else
               hitWall = true;
         }
         else
         {
            maxSlopeProposedY = Math.floor(this.y + this.dy - this.dx);
            var slopedCeiling = this.layer.getBottomSolidPixel(proposedPixelX, proposedPixelY, pixelX - proposedPixelX, maxSlopeProposedY - proposedPixelY, this.solidity);
            if (slopedCeiling != MapLayer.noSolid)
            {
               slopedCeiling++;
               var floor = this.layer.getTopSolidPixel(pixelX, proposedPixelY + solidHeight, solidWidth, slopedCeiling - proposedPixelY, this.solidity);
               if ((floor == MapLayer.noSolid) && (this.ridingOn == null))
               {
                  var leftwall2 = this.layer.getRightSolidPixel(proposedPixelX, slopedCeiling, pixelX - proposedPixelX, solidHeight, this.solidity);
                  if (leftwall2 == MapLayer.noSolid)
                     this.dy = dyOrig = slopedCeiling - this.y;
                  else
                     hitWall = true;
               }
               else
                  hitWall = true;
            }
            else
               hitWall = true;
         }
         if (hitWall)
         {
            // Do integer arithmetic before double otherwise strange rounding seems to happen
            this.dx = leftwall + 1 - this.x;
         }
         hit = true;
      }
   }

   this.dy = dyOrig;
   proposedPixelX = Math.floor(this.x + this.dx);
   proposedPixelY = Math.floor(this.y + this.dy);
   var proposedSolidPixelWidth = solidWidth + Math.ceil(this.x + this.dx) - proposedPixelX;
   if (this.dy > 0)
   {
      proposedPixelY2 = Math.ceil(this.y + this.dy);
      var ground = this.layer.getTopSolidPixel(proposedPixelX, pixelY + solidHeight, proposedSolidPixelWidth, proposedPixelY2 - pixelY, this.solidity);
      if (ground != MapLayer.noSolid)
      {
         this.dy = ground - solidHeight - this.y;
         hit = true;
      }
   }
   else if (this.dy < 0)
   {
      var ceiling = this.layer.getBottomSolidPixel(proposedPixelX, proposedPixelY, proposedSolidPixelWidth, pixelY - proposedPixelY, this.solidity);
      if (ceiling != MapLayer.noSolid)
      {
         this.dy = ceiling + 1 - this.y;
         hit = true;
      }
   }

   if (hit && (this.localDX != null))
      this.localDX += this.dx - dxOrig;

   return hit;
};