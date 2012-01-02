function GeneralRules() {
}

GeneralRules.prototype.saveUnit = null;

GeneralRules.prototype.saveGame = function(slot, temporary) {
   var data = {};
   GeneralRules["save" + slot] = data;
   if (this.saveUnit == null)
      this.saveUnit = maps;
   for(var key in this.saveUnit) {
      data[key] = maps[key].getState();
   }
   this.saveUnit = null;
};

GeneralRules.prototype.loadGame = function(slot, temporary) {
   var data = GeneralRules["save" + slot];
   if (data == null) return;
   for(var key in data) {
      maps[key].setState(data[key]);
   }
};

GeneralRules.prototype.saveExists = function(slot, temporary) {
   return GeneralRules["save" + slot] != null;
};

function getMapName(map) {
   for(var key in maps) {
      if (maps[key] === map)
         return key;
   }
   return null;
}

GeneralRules.prototype.includeMapInSaveUnit = function(map) {
   if (this.saveUnit == null)
      this.saveUnit = {};
   this.saveUnit[getMapName(map)] = map;
}

GeneralRules.prototype.changeCounter = function(counter, operation) {
   switch (operation) {
      case "IncrementAndStop":
         if (counter.value < counter.max)
            counter.value += 1;
         else
            return true;
         return false;
      case "DecrementAndStop":
         if (counter.value > counter.min)
            counter.value -= 1;
         else
            return true;
         return false;
      case "IncrementAndLoop":
         if (counter.value < counter.max)
         {
            counter.value += 1;
            return false;
         }
         counter.value = counter.min;
         return true;
      case "DecrementAndLoop":
         if (counter.value > counter.min)
         {
            counter.value -= 1;
            return false;
         }
         counter.value = counter.max;
         return true;
      case "SetToMinimum":
         if (counter.value == counter.min)
            return true;
         counter.value = counter.min;
         return false;
      case "SetToMaximum":
         if (counter.value == counter.max)
            return true;
         counter.value = counter.max;
         return false;
   }
   return false;
}

GeneralRules.prototype.setMapFlag = function(flagIndex, value) {
   if (this.layer.map.mapFlags == null)
      this.layer.map.mapFlags = 0;
   if (value)
      this.layer.map.mapFlags |= 1 << flagIndex;
   else
      this.layer.map.mapFlags &= ~(1 << flagIndex);
}

GeneralRules.prototype.isMapFlagOn = function(flagIndex) {
   if (this.layer.map.mapFlags == null)
      this.layer.map.mapFlags = 0;
   return ((this.layer.map.mapFlags & (1 << flagIndex)) != 0);
}

GeneralRules.prototype.clearOverlay = function() {
   overlayMap = null;
}
