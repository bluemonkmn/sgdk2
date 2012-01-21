function SpriteState(solidWidth, solidHeight, frameSetName, bounds, frames) {
   this.solidWidth = solidWidth;
   this.solidHeight = solidHeight;
   this.frameSetName = frameSetName;
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
   this.isActive = active;
   this.priority = priority;
   this.solidity = solidity;
   this.ridingOn = null;
   this.localDX = null;
   this.inputs = 0;
   this.oldInputs = 0;
}

Sprite.prototype = new GeneralRules();
Sprite.prototype.constructor = Sprite;

Sprite.categorize = function(sprites) {
   var categories = {};
   for(var sprKey in sprites) {
      var spr = sprites[sprKey];
      if (spr.categories == null) continue;
      for(var sprCatKey in spr.categories) {
         var cat = spr.categories[sprCatKey];
         if (categories[cat] == null)
            categories[cat] = [spr];
         else
            categories[cat].push(spr);
      }
   }
   return categories;
}

Sprite.deserialize = function(layer,data) {
   var source = JSON.parse(data);
   return spriteDefinitions[source["~1"]].deserialize(layer, data);
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
   if (this.solidity == null)
      return;
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

Sprite.inputBits = { up:1, right:2, down:4, left:8, button1:16, button2:32, button3:64, button4:128 };
Sprite.prototype.mapPlayerToInputs = function(playerNum) {
   var p = players[playerNum - 1];
   this.oldInputs = this.inputs;
   this.inputs = 0;
   if (GeneralRules.playerPressButton(playerNum)) {
      if (p.up()) this.inputs |= Sprite.inputBits.up;
      if (p.left()) this.inputs |= Sprite.inputBits.left;
      if (p.right()) this.inputs |= Sprite.inputBits.right;
      if (p.down()) this.inputs |= Sprite.inputBits.down;
      if (p.button1()) this.inputs |= Sprite.inputBits.button1;
      if (p.button2()) this.inputs |= Sprite.inputBits.button2;
      if (p.button3()) this.inputs |= Sprite.inputBits.button3;
      if (p.button4()) this.inputs |= Sprite.inputBits.button4;
   }
};

Sprite.prototype.accelerateByInputs = function(acceleration, max, horizontalOnly) {
   if (!horizontalOnly) {
      if (0 != (this.inputs & Sprite.inputBits.up))
         this.dy -= acceleration / 10;
      if (this.dy < -max)
         this.dy = -max;
      if (0 != (this.inputs & Sprite.inputBits.down))
         this.dy += acceleration / 10;
      if (this.dy > max)
         this.dy = max;
   }
   if (this.localDX == null) {
      if (0 != (this.inputs & Sprite.inputBits.left))
         this.dx -= acceleration / 10;
      if (this.dx < -max)
         this.dx = -max;
      if (0 != (this.inputs & Sprite.inputBits.right))
         this.dx += acceleration / 10;
      if (this.dx > max)
         this.dx = max;
   } else {
      if (0 != (this.inputs & Sprite.inputBits.left))
         this.localDX -= acceleration / 10;
      if (this.localDX < -max)
         this.localDX = -max;
      if (0 != (this.inputs & Sprite.inputBits.right))
         this.localDX += acceleration / 10;
      if (this.localDX > max)
         this.localDX = max;
   }
};

Sprite.prototype.isInState = function(firstState, lastState) {
   return (this.state >= firstState) && (this.state <= lastState);
};

Sprite.prototype.moveByVelocity = function() {
   this.oldX = this.x;
   this.oldY = this.y;
   this.x += this.dx;
   this.y += this.dy;
};

Sprite.prototype.scrollSpriteIntoView = function(useScrollMargins) {
   this.layer.scrollSpriteIntoView(this, useScrollMargins);
};

Sprite.prototype.limitVelocity = function(maximum) {
   var useDX;
   if (this.localDX == null)
      useDX = this.dx;
   else
      useDX = this.localDX;
   var dist = useDX * useDX + this.dy * this.dy;
   if (dist > maximum * maximum) {
      dist = Math.sqrt(dist);
      useDX = useDX * maximum / dist;
      this.dy = this.dy * maximum / dist;
      if (this.localDX == null)
         this.dx = useDX;
      else
         this.localDX = useDX;
   }
}

Sprite.prototype.isOnTile = function(category, relativePosition) {
   var rp = this.getRelativePosition(relativePosition);
   var tile = this.layer.getTile(Math.floor(rp.x / this.layer.tileset.tileWidth), Math.floor(rp.y / this.layer.tileset.tileHeight));
   return category.isTileMember(this.layer.tileset, tile);
}

Sprite.prototype.getRelativePosition = function(relativePosition) {
   var rp = {x:Math.floor(this.x),y:Math.floor(this.y)};

   switch (relativePosition) {
      case "TopCenter":
         rp.x = Math.floor(this.x + this.getSolidWidth() / 2);
         break;
      case "TopRight":
         rp.x = Math.floor(this.x) + this.getSolidWidth() - 1;
         break;
      case "LeftMiddle":
         rp.y = Math.floor(this.y + this.getSolidHeight() / 2);
         break;
      case "CenterMiddle":
         rp.x = Math.floor(this.x + this.getSolidWidth() / 2);
         rp.y = Math.floor(this.y + this.getSolidHeight() / 2);
         break;
      case "RightMiddle":
         rp.x = Math.floor(this.x) + this.getSolidWidth() - 1;
         rp.y = Math.floor(this.y + this.getSolidHeight() / 2);
         break;
      case "BottomLeft":
         rp.y = Math.floor(this.y + this.getSolidHeight() - 1);
         break;
      case "BottomCenter":
         rp.x = Math.floor(this.x + this.getSolidWidth() / 2);
         rp.y = Math.floor(this.y + this.getSolidHeight() - 1);
         break;
      case "BottomRight":
         rp.x = Math.floor(this.x) + this.getSolidWidth() - 1;
         rp.y = Math.floor(this.y + this.getSolidHeight() - 1);
         break;
   }
   return rp;
}

Sprite.prototype.blocked = function(direction) {
   var solidPixelWidth;
   var solidPixelHeight;
   switch (direction)
   {
      case "Up":
         solidPixelWidth = this.getSolidWidth() + Math.ceil(this.x) - Math.floor(this.x);
         return this.layer.getBottomSolidPixel(Math.floor(this.x), Math.floor(this.y) - 1, solidPixelWidth, 1, this.solidity) != MapLayer.noSolid;
      case "Right":
         solidPixelHeight = this.getSolidHeight() + Math.ceil(this.y) - Math.floor(this.y);
         return this.layer.getLeftSolidPixel(Math.floor(this.x) + this.getSolidWidth(), Math.floor(this.y), 1, solidPixelHeight, this.solidity) != MapLayer.noSolid;
      case "Down":
         solidPixelWidth = this.getSolidWidth() + Math.ceil(this.x) - Math.floor(this.x);
         return this.layer.getTopSolidPixel(Math.floor(this.x), Math.floor(this.y) + this.getSolidHeight(), solidPixelWidth, 1, this.solidity) != MapLayer.noSolid;
      case "Left":
         solidPixelHeight = this.getSolidHeight() + Math.ceiling(this.y) - Math.floor(this.y);
         return this.layer.getRightSolidPixel(Math.floor(this.x) - 1, Math.floor(this.y), 1, solidPixelHeight, this.solidity) != MapLayer.noSolid;
   }
   return false;
}

Sprite.prototype.isMoving = function(direction) {
   var useDX;
   if (this.localDX == null)
      useDX = this.dx;
   else
      useDX = this.localDX;

   switch (direction) {
      case "Left":
         return useDX < 0;
      case "Right":
         return useDX > 0;
      case "Up":
         return this.dy < 0;
      case "Down":
         return this.dy > 0;
   }
   return false;
}

Sprite.prototype.isInputPressed = function(input, initialOnly) {
   return (this.inputs & input) &&
      (!initialOnly || (0 == (this.oldInputs & input)));
}

Sprite.prototype.alterXVelocity = function(delta) {
   this.dx += delta;
}

Sprite.prototype.alterYVelocity = function(delta) {
   this.dy += delta;
}

Sprite.prototype.reactToInertia = function(retainPercentVertical, retainPercentHorizontal) {
   if (this.localDX == null) {
      if (Math.abs(this.dx) < .01)
         this.dx = 0;
      else
         this.dx *= retainPercentHorizontal / 100.0;
   } else {
      if (Math.abs(this.localDX) < .01)
         this.localDX = 0;
      else
         this.localDX *= retainPercentHorizontal / 100.0;
   }
   if (Math.abs(this.dy) < .01)
      this.dy = 0;
   else
      this.dy *= retainPercentVertical / 100.0;
}

Sprite.prototype.animate = function(correlation) {
   switch (correlation)
   {
      case "ByFrame":
         this.frame++;
         break;
      case "ByHorizontalVelocity":
         if (this.localDX == null)
            this.frame += Math.abs(Math.floor(this.x + this.dx) - Math.floor(this.x));
         else
            this.frame += Math.abs(Math.floor(this.localDX));
         break;
      case "ByVerticalVelocity":
         this.frame += Math.abs(Math.floor(this.y + this.dy) - Math.floor(this.y));
         break;
      case "ByVectorVelocity":
         var tmpDx = Math.abs(Math.floor(this.x + this.dx) - Math.floor(this.x));
         var tmpDy = Math.abs(Math.floor(this.y + this.dy) - Math.floor(this.y));
         this.frame += Math.floor(Math.sqrt(tmpDx * tmpDx + tmpDy * tmpDy));
         break;
   }
}

Sprite.prototype.isRidingPlatform = function() {
   return this.ridingOn != null;
}

Sprite.prototype.processRules = function() {
   if ((!this.processed) && (this.isActive)) {
      this.processed = true;
      this.executeRules();
   }
}

Sprite.prototype.reactToPlatform = function() {
   if (this.ridingOn == null)
      return;

   if (!this.ridingOn.processed)
      this.ridingOn.processRules();

   if ((this.ridingOn.isActive == false) || (this.x + this.getSolidWidth() < this.ridingOn.oldX) || (this.x > this.ridingOn.oldX + this.ridingOn.getSolidWidth()) ||
      (this.y + this.getSolidHeight() < this.ridingOn.oldY - 1) || (this.y + this.getSolidHeight() >= this.ridingOn.oldY + this.ridingOn.getSolidHeight()))
   {
      this.stopRiding();
      return;
   }

   if (this.localDX != null)
      this.dx = this.localDX + this.ridingOn.dx;
   this.dy = this.ridingOn.y - this.getSolidHeight() - this.y;
}

Sprite.prototype.landDownOnPlatform = function(platformList) {
   if (this.ridingOn != null)
      return false;
   for(var sprKey in platformList) {
      var spr = platformList[sprKey];
      if (!spr.isActive)
         continue;
      if ((this.oldY + this.getSolidHeight() <= spr.oldY) &&
         (this.y + this.getSolidHeight() > spr.y) &&
         (this.x + this.getSolidWidth() > spr.x) &&
         (this.x < spr.x + spr.getSolidWidth()))
      {
         this.ridingOn = spr;
         spr.processRules();
         this.localDX = this.dx - spr.dx;
         this.dy = spr.y - this.getSolidHeight() - this.y;
         return true;
      }
   }
   return false;
}

Sprite.prototype.snapToGround = function(threshhold) {
   var proposedPixelX = Math.floor(this.x + this.dx);
   var proposedPixelY = Math.floor(this.y + this.dy);
   var proposedSolidPixelWidth = this.getSolidWidth() + Math.ceil(this.x + this.dx) - proposedPixelX;
   var ground = this.layer.getTopSolidPixel(proposedPixelX, proposedPixelY + this.getSolidHeight(), proposedSolidPixelWidth, threshhold, this.solidity);
   if (ground != MapLayer.noSolid) {
      newDy = ground - this.getSolidHeight() - this.y;
      if (newDy > this.dy)
         this.dy = newDy;
      return true;
   }
   return false;
}

Sprite.prototype.stopRiding = function() {
   this.localDX = null;
   this.ridingOn = null;
}

Sprite.prototype.switchToState = function(state, alignment) {
   var oldRect = {x:Math.floor(this.x), y:Math.floor(this.y), width:this.getSolidWidth(), height:this.getSolidHeight()};
   oldRect.bottom = oldRect.y + oldRect.height;
   oldRect.right = oldRect.x + oldRect.width;
   var newWidth = this.states[state].solidWidth;
   var newHeight = this.states[state].solidHeight;
   var newX, newY;
   switch (alignment) {
      case "TopCenter":
      case "CenterMiddle":
      case "BottomCenter":
         newX = this.x + (oldRect.width - newWidth) / 2;
         break;
      case "TopRight":
      case "RightMiddle":
      case "BottomRight":
         newX = this.x + oldRect.width - newWidth;
         break;
      default:
         newX = this.x;
         break;
   }
   switch (alignment) {
      case "LeftMiddle":
      case "CenterMiddle":
      case "RightMiddle":
         newY = this.y + (oldRect.height - newHeight) / 2;
         break;
      case "BottomLeft":
      case "BottomCenter":
      case "BottomRight":
         newY = this.y + oldRect.height - newHeight;
         break;
      default:
         newY = this.y;
         break;
   }

   if ((Math.ceil(newY + newHeight) > oldRect.bottom) && (this.layer.getTopSolidPixel(
      Math.floor(newX), oldRect.bottom, newWidth, Math.ceil(newY) + newHeight - oldRect.bottom, this.solidity) != MapLayer.noSolid))
      return false;

   if ((Math.floor(newY) < oldRect.y) && (this.layer.getBottomSolidPixel(
      Math.floor(newX), Math.floor(newY), newWidth, oldRect.y - Math.floor(newY), this.solidity) != MapLayer.noSolid))
      return false;

   if ((Math.floor(newX) < oldRect.x) && (this.layer.getRightSolidPixel(
      Math.floor(newX), Math.floor(newY), oldRect.x - Math.floor(newX), newHeight, this.solidity) != MapLayer.noSolid))
      return false;

   if ((Math.ceil(newX + newWidth) > oldRect.right) && (this.layer.getLeftSolidPixel(
      oldRect.right, Math.floor(newY), Math.ceil(newX) + newWidth - oldRect.right, newHeight, this.solidity) != MapLayer.noSolid))
      return false;

   this.x = newX;
   this.y = newY;
   this.state = state;
   return true;
}

Sprite.prototype.deactivate = function() {
   this.isActive = false;
}

Sprite.prototype.touchTiles = function(category) {
   if (this.touchedTiles != null)
      this.touchedTiles.length = 0;

   var tw = this.layer.tileset.tileWidth;
   var th = this.layer.tileset.tileHeight;
   var minYEdge = Math.floor(Math.floor(this.y) / th);
   var maxY = Math.floor((Math.floor(this.y) + this.getSolidHeight()) / th);
   if (maxY >= this.layer.virtualRows)
      maxY = this.layer.virtualRows - 1;
   var maxYEdge = Math.floor((Math.floor(this.y) + this.getSolidHeight() - 1) / th);
   var minX = Math.floor(Math.floor(this.x - 1) / tw);
   var minXEdge = Math.floor(Math.floor(this.x) / tw);
   var maxX = Math.floor((Math.floor(this.x) + this.getSolidWidth()) / tw);
   if (maxX >= this.layer.virtualColumns)
      maxX = this.layer.virtualColumns - 1;
   var maxXEdge = Math.floor((Math.floor(this.x) + this.getSolidWidth() - 1) / tw);
   for (var yidx = Math.floor((Math.floor(this.y) - 1) / th); yidx <= maxY; yidx++) {
      var isYEdge = !((yidx >= minYEdge) && (yidx <= maxYEdge));
      for (var xidx = (isYEdge ? minXEdge : minX);
         xidx <= (isYEdge ? maxXEdge : maxX);
         xidx++)
      {
         if (category.isTileMember(this.layer.tileset, this.layer.getTile(xidx, yidx))) {
            var wasTouching;
            var oldPixelX = Math.floor(this.oldX);
            var oldPixelY = Math.floor(this.oldY);

            if ((oldPixelX <= xidx * tw + tw) &&
               (oldPixelX + this.getSolidWidth() >= xidx * tw) &&
               (oldPixelY <= yidx * th + th) &&
               (oldPixelY + this.getSolidHeight() >= yidx * th))
            {
               var edgeX = (oldPixelX + this.getSolidWidth() == xidx * tw) ||
                  (this.oldPixelX == xidx * tw + tw);
               var edgeY = (oldPixelY + this.getSolidHeight() == yidx * th) ||
                  (this.oldPixelY == yidx * th + th);
               if (edgeX && edgeY)
                  wasTouching = false;
               else
                  wasTouching = true;
            }
            else
               wasTouching = false;
            
            if (this.touchedTiles == null)
               this.touchedTiles = [];
            this.touchedTiles.push({x:xidx, y:yidx, tileValue:this.layer.getTile(xidx, yidx), initial:!wasTouching, processed:false});
         }
      }
   }
   if (this.touchedTiles == null)
      return false;
   return this.touchedTiles.length > 0;
};

Sprite.prototype.tileTake = function(tileValue, counter, newValue) {
   if (this.touchedTiles == null)
      return 0;

   var result = 0;

   for (var i = 0; i < this.touchedTiles.length; i++) {
      var tt = this.touchedTiles[i];
      if ((tt.tileValue == tileValue) && (!tt.processed)) {
         if (counter.value < counter.max) {
            counter.value++;
            this.layer.setTile(tt.x, tt.y, tt.tileValue = newValue);
            tt.processed = true;
            result++;
         }
         else
            break;
      }
   }
   return result;
};

Sprite.prototype.tileAddSprite = function (touchingIndex, spriteDefinition) {
   var tt = this.touchedTiles[touchingIndex];
   var spriteParams = "{\"~1\":\"" + spriteDefinition + "\", \"x\":" +
   tt.x * this.layer.tileset.tileWidth + ",\"y\":" + tt.y * this.layer.tileset.tileHeight +
   ",\"dx\":0,\"dy\":0,\"state\":0,\"frame\":0,\"active\":true,\"priority\":0,\"solidityName\":\"" +
   solidity.getSolidityName(this.solidity) + "\"}";
   GeneralRules.lastCreatedSprite = Sprite.deserialize(this.layer, spriteParams);
   GeneralRules.lastCreatedSprite.isDynamic = true;
   GeneralRules.lastCreatedSprite.clearParameters();

   this.layer.sprites.push(GeneralRules.lastCreatedSprite);
   for(var categoryKey in spriteDefinitions[spriteDefinition].prototype.categories) {
      var category = spriteDefinitions[spriteDefinition].prototype.categories[categoryKey];
      this.layer.spriteCategories[category].push(GeneralRules.lastCreatedSprite);
   }
};

Sprite.prototype.tileActivateSprite = function(touchingIndex, category, clearParameters) {
   for (var i = 0; i < category.length; i++) {
      if (!category[i].isActive) {
         category[i].isActive = true;
         var tt = this.touchedTiles[touchingIndex];
         category[i].x = tt.x * this.layer.tileset.tileWidth;
         category[i].y = tt.y * this.layer.tileset.tileHeight;
         if (clearParameters) {
            category[i].frame = 0;
            category[i].state = 0;
            category[i].clearParameters();
         }
         category[i].processRules();
         return i;
      }
   }
   return -1;
};

Sprite.prototype.clearParameters = function() {
   if (this.userParams == null) return;
   for(i in userParams) {
      this[this.userParams[i]] = 0;
   }
};

Sprite.prototype.setSolidity = function(solidity) {
   this.solidity = solidity;
};

Sprite.prototype.testCollisionRect = function(targets) {
   if (!this.isActive)
      return -1;
   for(var idx = 0; idx < targets.length; idx++) {
      var targetSprite = targets[idx];
      if ((targetSprite == this) || (!targetSprite.isActive))
         continue;
      var x1 = Math.floor(this.x);
      var w1 = this.getSolidWidth();
      var x2 = Math.floor(targetSprite.x);
      var w2 = targetSprite.getSolidWidth();
      var y1 = Math.floor(this.y);
      var h1 = this.getSolidHeight();
      var y2 = Math.floor(targetSprite.y);
      var h2 = targetSprite.getSolidHeight();

      if ((x1 + w1 > x2) && (x2 + w2 > x1) && (y1 + h1 > y2) && (y2 + h2 > y1))
         return idx;
   }
   return -1;
};

Sprite.prototype.getNearestSpriteIndex = function(target) {
   var minDist = 999999999;
   var result = -1;
   for (var i = 0; i < target.length; i++) {
      if ((!target[i].isActive) || (target[i] == this))
         continue;
      var xOff = target[i].x - this.x;
      var yOff = target[i].y - this.y;
      var dist = xOff * xOff + yOff * yOff;
      if (dist < minDist) {
         minDist = dist;
         result = i;
      }
   }
   return result;
};

Sprite.prototype.pushTowardCategory = function(target, index, force) {
   if (index < 0)
      index = this.getNearestSpriteIndex(target);
   if (index < 0)
      return false;

   return this.pushTowardSprite(target[index], force);
};

Sprite.prototype.pushTowardSprite = function (target, force) {
   var vx = target.x - this.x + (target.getSolidWidth() - this.getSolidWidth()) / 2;
   var vy = target.y - this.y + (target.getSolidHeight() - this.getSolidHeight()) / 2;
   var dist = Math.sqrt(vx * vx + vy * vy);
   if (dist >= 1) {
      this.dx += vx * force / dist / 10.0;
      this.dy += vy * force / dist / 10.0;
      return true;
   }
   return false;
};

Sprite.prototype.setInputState = function(input, press) {
   if (press)
      this.inputs |= input;
   else
      this.inputs &= ~input;
};

Sprite.prototype.clearInputs = function(setOldInputs) {
   if (setOldInputs)
      this.oldInputs = this.inputs;
   inputs = 0;
};

Sprite.prototype.tileUseUp = function(tileValue, counter, newValue) {
   if (this.touchedTiles == null)
      return 0;

   var result = 0;

   for (var i = 0; i < this.touchedTiles.length; i++) {
      var tt = this.touchedTiles[i];
      if ((tt.tileValue == tileValue) && (!tt.processed)) {
         if (counter.value > 0) {
            counter.value--;
            this.layer.setTile(tt.x, tt.y, tt.tileValue = newValue);
            tt.processed = true;
            result++;
         }
         else
            break;
      }
   }
   return result;
};

Sprite.prototype.addSpriteHere = function(spriteDefinition, location, hotSpot) {
   var spriteParams = "{\"~1\":\"" + spriteDefinition + "\", \"x\":0,\"y\":0" +
   ",\"dx\":0,\"dy\":0,\"state\":0,\"frame\":0,\"active\":true,\"priority\":0,\"solidityName\":\"" +
   solidity.getSolidityName(this.solidity) + "\"}";
   GeneralRules.lastCreatedSprite = Sprite.deserialize(this.layer, spriteParams);

   ptLocation = this.getRelativePosition(location);
   ptHotSpot = GeneralRules.lastCreatedSprite.getRelativePosition(hotSpot);
   GeneralRules.lastCreatedSprite.x = GeneralRules.lastCreatedSprite.oldX = ptLocation.x - ptHotSpot.x;
   GeneralRules.lastCreatedSprite.y = GeneralRules.lastCreatedSprite.oldY = ptLocation.y - ptHotSpot.y;

   GeneralRules.lastCreatedSprite.isDynamic = true;
   GeneralRules.lastCreatedSprite.clearParameters();

   this.layer.sprites.push(GeneralRules.lastCreatedSprite);
   for(var categoryKey in spriteDefinitions[spriteDefinition].prototype.categories) {
      var category = spriteDefinitions[spriteDefinition].prototype.categories[categoryKey];
      this.layer.spriteCategories[category].push(GeneralRules.lastCreatedSprite);
   }
};

Sprite.prototype.tileChange = function(oldTileValue, newTileValue, initialOnly) {
   if (this.touchedTiles == null)
      return 0;

   var result = 0;

   for (var i = 0; i < this.touchedTiles.length; i++) {
      var tt = this.touchedTiles[i];
      if ((tt.tileValue == oldTileValue) && (!tt.processed) && (!initialOnly || tt.initial)) {
         tt.processed = true;
         this.layer.setTile(tt.x, tt.y, tt.tileValue = newTileValue);
         result++;
      }
   }
   return result;
};

Sprite.prototype.tileChangeTouched = function(touchingIndex, newTileValue) {
   if ((this.touchedTiles == null) || (this.touchedTiles.length <= touchingIndex))
      return;

   var tt = this.touchedTiles[touchingIndex];
   this.layer[tt.x, tt.y] = tt.tileValue = newTileValue;
};

