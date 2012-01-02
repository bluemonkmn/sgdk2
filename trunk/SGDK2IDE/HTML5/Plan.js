function PlanBase() {
   this.targetDistance = 5;
}

PlanBase.prototype = new GeneralRules();
PlanBase.prototype.constructor = PlanBase;

PlanBase.prototype.isSpriteActive = function(sprite) {
   return sprite.isActive;
}

PlanBase.prototype.mapPlayerToInputs = function(playerNumber, target)
{
   target.mapPlayerToInputs(playerNumber);
}

PlanBase.prototype.followPath = function(sprite, coordinateIndexMember, waitCounterMember) {
   if (sprite.isActive) {
      if (sprite[waitCounterMember] == 0)
         this.pushSpriteTowardCoordinate(sprite, sprite[coordinateIndexMember], 10);
      else
         this.stopSprite(sprite);
      sprite[coordinateIndexMember] = this.checkNextCoordinate(sprite, sprite[coordinateIndexMember], waitCounterMember);
   }
}

PlanBase.prototype.pushSpriteTowardCoordinate = function(sprite, coordinateIndex, force) {
   this.pushSpriteTowardPoint(sprite, this[coordinateIndex], force);
}

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
}

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
}

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
}

PlanBase.prototype.stopSprite = function(sprite) {
   sprite.dx = sprite.dy = 0;
}
