function PlanBase() {
   this.targetDistance = 5;
}

PlanBase.prototype = new GeneralRules();
PlanBase.prototype.constructor = PlanBase;

PlanBase.prototype.isSpriteActive = function(sprite) {
   return sprite.isActive;
};

PlanBase.prototype.mapPlayerToInputs = function(playerNumber, target)
{
   target.mapPlayerToInputs(playerNumber);
};

PlanBase.prototype.followPath = function(sprite, coordinateIndexMember, waitCounterMember) {
   if (sprite.isActive) {
      if (sprite[waitCounterMember] == 0)
         this.pushSpriteTowardCoordinate(sprite, sprite[coordinateIndexMember], 10);
      else
         this.stopSprite(sprite);
      sprite[coordinateIndexMember] = this.checkNextCoordinate(sprite, sprite[coordinateIndexMember], waitCounterMember);
   }
};

PlanBase.prototype.pushSpriteTowardCoordinate = function(sprite, coordinateIndex, force) {
   this.pushSpriteTowardPoint(sprite, this[coordinateIndex], force);
};

PlanBase.prototype.pushSpriteTowardPoint = function(sprite, target, force) {
   var dx = target.x - sprite.x;
   var dy = target.y - sprite.y;

   // Normalize target vector to magnitude of Force parameter
   var dist = Math.sqrt(dx * dx + dy * dy);
   if (dist > 0) {
      dx = dx * force / dist / 10;
      dy = dy * force / dist / 10;

      // Push sprite
      sprite.dx += dx;
      sprite.dy += dy;
   }
};

PlanBase.prototype.checkNextCoordinate = function(sprite, coordinateIndex, waitCounterMember) {
   if (sprite[waitCounterMember] > 0)
   {
      if (++sprite[waitCounterMember] > this[coordinateIndex].weight)
      {
         sprite[waitCounterMember] = 0;
         return (coordinateIndex + 1) % this.m_Coords.length;
      }
      else
         return coordinateIndex;
   }
   var dx = this[coordinateIndex].x - sprite.x;
   var dy = this[coordinateIndex].y - sprite.y;
   if (Math.sqrt(dx * dx + dy * dy) <= this.targetDistance)
   {
      if (this[coordinateIndex].weight > 0)
         sprite[waitCounterMember]++;
      else
         return (coordinateIndex + 1) % this.m_Coords.length;
   }
   return coordinateIndex;
};

PlanBase.prototype.isSpriteTouching = function(sprite) {
   if (!sprite.isActive)
      return false;

   if ((Math.floor(sprite.x) <= this.left + this.width) && (Math.floor(sprite.x) + sprite.getSolidWidth() >= this.left) &&
      (Math.floor(sprite.y) < this.top + this.height) && (Math.floor(sprite.y) + sprite.getSolidHeight() > this.top))
      return true;
   if ((Math.floor(sprite.x) < this.left + this.width) && (Math.floor(sprite.x) + sprite.getSolidWidth() > this.left) &&
      (Math.floor(sprite.y) <= this.top + this.height) && (Math.floor(sprite.y) + sprite.getSolidHeight() >= this.top))
      return true;
   return false;
};

PlanBase.prototype.wasSpriteTouching = function(sprite) {
   if ((Math.floor(sprite.oldX) <= this.left + this.width) && (Math.floor(sprite.oldX) + sprite.getSolidWidth() >= this.left) &&
      (Math.floor(sprite.oldY) < this.top + this.height) && (Math.floor(sprite.oldY) + sprite.getSolidHeight() > this.top))
      return true;
   if ((Math.floor(sprite.oldX) < this.left + this.width) && (Math.floor(sprite.oldX) + sprite.getSolidWidth() > this.left) &&
      (Math.floor(sprite.oldY) <= this.top + this.height) && (Math.floor(sprite.oldY) + sprite.getSolidHeight() >= this.top))
      return true;
   return false;
};

PlanBase.prototype.stopSprite = function(sprite) {
   sprite.dx = sprite.dy = 0;
};

PlanBase.prototype.isInputPressed = function(sprite, input, initialOnly)
{
   return sprite.isInputPressed(input, initialOnly);
};

PlanBase.prototype.drawCounterAsTile = function(tileIndex, counter, style) {
   if (this.left == null)
      return;
   if (counter.currentValue == 0)
      return;
   var map = this.layer.map;
   ts = this.layer.tileset;
   fr = ts.frameSet;
   var disp = gameViewContext;

   var frames = ts.tiles[tileIndex];
   if (typeof frames == "number") {
      frames = [frames];
   } else {
      if (frames instanceof AnimTile)
         frames = frames.getCurFrames();
      if (typeof frames == "number") {
         frames = [frames];
      }
   }

   switch(style)
   {
      case "ClipRightToCounter":
         disp.save();
         disp.beginPath();
         disp.rect(this.left + this.layer.currentX,
            this.top + this.layer.currentY,
            this.width * counter.value / counter.max,
            this.height);
         disp.clip();
         for(var frameIndex in frames)
            fr.frames[frameIndex % fr.frames.length].draw(disp, this.left, this.top);
         disp.restore();
         break;
      case "StretchRightToCounter":
         throw "Not Implemented";
         break;
      case "RepeatRightToCounter":
         for(var i in frames) {
            var frameIndex = frames[i];
            var fillWidth = this.width * counter.value / counter.max;
            for (var repeat = 0; repeat < Math.ceil(fillWidth / ts.tileWidth); repeat++)
               fr.frames[frameIndex % fr.frames.length].draw(disp, this.left + repeat * ts.tileWidth, this.top);
         }
         break;
      case "ClipTopToCounter":
         throw "Not Implemented";
         break;
      case "StretchTopToCounter":
         throw "Not Implemented";
         break;
      case "RepeatUpToCounter":
         for(var i in frames) {
            var frameIndex = frames[i];
            var fillHeight = this.height * counter.value / counter.max;
            for (var repeat = 0; repeat < Math.ceil(fillHeight / ts.tileHeight); repeat++)
               fr.frames[frameIndex % fr.frames.length].draw(disp, this.left + repeat * ts.tileWidth, this.top - repeat * ts.tileHeight - ts.tileHeight);
         }
         break;
   }
};

function drawText(text, x, y) {
   var charWidth = 13;
   var charHeight = 18;
   var font = graphicSheets.CoolFont;
   if (font == null)
      throw "In order to use DrawText, the project must have a Graphic Sheet named \"CoolFont\"";
   var origX = x;
   for (var charIdx = 0; charIdx < text.length; charIdx++) {
      var curChar = text.charCodeAt(charIdx);
      if (curChar > 32) {
         var col = (curChar - 33) % 24;
         var row = Math.floor((curChar - 33) / 24);
         gameViewContext.drawImage(font.image, col * font.cellWidth, row * font.cellHeight,
            font.cellWidth, font.cellHeight, x, y, font.cellWidth, font.cellHeight);
         x += charWidth;
      }
      else if (curChar == 10)
      {
         x = origX;
         y += charHeight;
      }
   }
}

PlanBase.prototype.drawCounterWithLabel = function(label, counter) {
   if (this.left == null)
      return;   
   drawText(label.toString() + counter.value.toString(), this.left, this.top);
};

PlanBase.prototype.isSpriteWithin = function(sprite, relativePosition) {
   var rp = sprite.getRelativePosition(relativePosition);
   return ((rp.x >= this.left) && (rp.y >= this.top) && (rp.x < this.left + this.width) && (rp.y < this.top + this.height));
};

PlanBase.prototype.copyInputsToOld = function(sprite) {
   sprite.oldInputs = sprite.inputs;
};

PlanBase.prototype.transportToPlan = function(sprite, plan, alignment) {
   if (plan.left == null)
      return;

   switch(alignment) {
      case "TopLeft":
      case "TopCenter":
      case "TopRight":
         sprite.y = plan.top;
         break;
      case "LeftMiddle":
      case "CenterMiddle":
      case "RightMiddle":
         sprite.y = plan.top + Math.floor((plan.height - sprite.getSolidHeight())/2);
         break;
      default:
         sprite.y = plan.top + plan.height - sprite.getSolidHeight();
         break;
   }
   switch(alignment)
   {
      case "TopLeft":
      case "LeftMiddle":
      case "BottomLeft":
         sprite.x = plan.left;
         break;
      case "TopCenter":
      case "CenterMiddle":
      case "BottomCenter":
         sprite.x = plan.left + Math.floor((plan.width - sprite.getSolidWidth())/2);
         break;
      default:
         sprite.x = plan.left + plan.width - sprite.getSolidWidth();
         break;
   }
};

PlanBase.prototype.door = function(target, sprites, trigger) {
   var result = -1;
   for (var i=0; i<sprites.length; i++) {
      if (sprites[i].isActive) {
         var outDoor;
         if (this.isSpriteWithin(sprites[i], "CenterMiddle"))
            outDoor = target;
         else if (target.isSpriteWithin(sprites[i], "CenterMiddle"))
            outDoor = this;
         else
            continue;
         if (((trigger & sprites[i].inputs) == trigger) &&
            ((sprites[i].inputs & trigger) != (sprites[i].oldInputs & trigger)))
         {
            result = i;
            this.transportToPlan(sprites[i], outDoor, "BottomCenter");
         }
      }
   }
   return result;
};

PlanBase.prototype.activateSprite = function(target) {
   target.isActive = true;
};


PlanBase.prototype.copyTiles = function(source, target, relativePosition) {
   var src_left = Math.floor(source.left / source.layer.tileset.tileWidth);
   var src_top = Math.floor(source.top / source.layer.tileset.tileHeight);
   var src_right = Math.floor((source.left + source.width - 1) / source.layer.tileset.tileWidth);
   var src_bottom = Math.floor((source.top + source.height - 1) / source.layer.tileset.tileHeight);

   var dst_left = Math.floor(target.left / target.layer.tileset.tileWidth);
   var dst_top = Math.floor(target.top / target.layer.tileset.tileHeight);
   var dst_right = Math.floor((target.left + target.width - 1) / target.layer.tileset.tileWidth);
   var dst_bottom = Math.floor((target.top + target.height - 1) / target.layer.tileset.tileHeight);

   for (var y = src_top; y <= src_bottom; y++) {
      var targety;
      switch(relativePosition) {
         case "TopLeft":
         case "TopCenter":
         case "TopRight":
            targety = dst_top + y - src_top;
            break;
         case "LeftMiddle":
         case "CenterMiddle":
         case "RightMiddle":
            targety = y + Math.floor((dst_top + dst_bottom - src_top - src_bottom) / 2);
            break;
         default:
            targety = dst_bottom + y - src_bottom;
            break;
      }
      if (targety < 0)
         continue;
      if (targety >= target.layer.virtualRows)
         break;
      for (var x = src_left; x <= src_right; x++) {
         var targetx;
         switch(relativePosition) {
            case "TopLeft":
            case "LeftMiddle":
            case "BottomLeft":
               targetx = dst_left + x - src_left;
               break;
            case "TopCenter":
            case "CenterMiddle":
            case "BottomCenter":
               targetx = x + Math.floor((dst_left + dst_right - src_left - src_right) / 2);
               break;
            default:
               targetx = dst_right + x - src_right;
               break;
         }
         if (targetx < 0)
            continue;
         if (targetx >= target.layer.virtualColumns)
            break;
            
         target.layer.setTile(targetx,targety,source.layer.getTile(x,y));
      }
   }
};

PlanBase.prototype.copyTo = function(target, relativePosition) {
   this.copyTiles(this, target, relativePosition);
};

PlanBase.prototype.copyFrom = function(source, relativePosition) {
   this.copyTiles(source, this, relativePosition);
};

PlanBase.prototype.deactivateSprite = function(target) {
   target.isActive = false;
};

PlanBase.prototype.matchSpritePosition = function(target, source) {
   target.oldX = target.x;
   target.oldY = target.y;
   target.x = source.x;
   target.y = source.y;
};

PlanBase.prototype.isSpriteWithin = function(sprite, relativePosition) {
   var rp = sprite.getRelativePosition(relativePosition);
   if ((rp.x >= this.left) && (rp.y >= this.top) && (rp.x < this.left + this.width) && (rp.y < this.top + this.height)) {
      return true;
   }
   return false;
};

PlanBase.prototype.scrollSpriteIntoView = function(sprite, useScrollMargins) {
   this.layer.scrollSpriteIntoView(sprite, useScrollMargins);
};

PlanBase.prototype.testCollisionRect = function(sourceSprite, targets) {
   sourceSprite.testCollisionRect(targets);
};

PlanBase.prototype.addSpriteAtPlan = function(spriteDefinition, relativePosition) {
   var spriteParams = "{\"~1\":\"" + spriteDefinition + "\", \"x\":0,\"y\":0" +
   ",\"dx\":0,\"dy\":0,\"state\":0,\"frame\":0,\"active\":true,\"priority\":0,\"solidityName\":\"\"}";
   
   GeneralRules.lastCreatedSprite = Sprite.deserialize(this.layer, spriteParams);

   if ((this.m_Coords != null) && (this.m_Coords.length > 0))
   {
      offset = lastCreatedSprite.getRelativePosition(relativePosition);
      GeneralRules.lastCreatedSprite.x = this[0].x - offset.x;
      GeneralRules.lastCreatedSprite.y = this[0].y - offset.y ;
   }

   this.layer.sprites.push(GeneralRules.lastCreatedSprite);
   for(var categoryKey in spriteDefinitions[spriteDefinition].prototype.categories) {
      var category = spriteDefinitions[spriteDefinition].prototype.categories[categoryKey];
      if (this.layer.spriteCategories[category] == null)
         this.layer.spriteCategories[category] = [];
      this.layer.spriteCategories[category].push(GeneralRules.lastCreatedSprite);
   }
}

PlanBase.prototype.mapMouseToSprite = function(target, instantMove, hotSpot) {
   target.mapMouseToSprite(instantMove, hotSpot);
}