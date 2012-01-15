function GeneralRules() {
}

GeneralRules.buttonSpecifier = {first:1, second:2, third:4, fourth:8, freezeInputs:16};
GeneralRules.maxMessages = 4;
GeneralRules.messageBackground = "rgba(64, 0, 255, .5)";
GeneralRules.currentPlayer = 0;
GeneralRules.activeMessages = [];
GeneralRules.messageMargin = 6;

GeneralRules.prototype.saveGame = function(slot, temporary) {
   if (GeneralRules.saveUnit == null) {
      this.includeInSaveUnit("AllMaps");
      this.includeInSaveUnit("AllCounters");
      this.includeInSaveUnit("WhichMapIsCurrent");
      this.includeInSaveUnit("WhichMapIsOverlaid");
   }
   if (GeneralRules.saveUnit.allMaps) {
      for(var key in maps) {
         GeneralRules.saveUnit.maps[key] = maps[key].getState();
      }
   } else if (GeneralRules.saveUnit.maps !== undefined) {
      for(var key in GeneralRules.saveUnit.maps) {
         GeneralRules.saveUnit.maps[key] = maps[key].getState();
      }
   }
   if (GeneralRules.saveUnit.counters != null) {
      for(var key in GeneralRules.saveUnit.counters) {
         GeneralRules.saveUnit.counters[key] = counters[key];
      }
   }
   if (GeneralRules.saveUnit.currentMap !== undefined)
      GeneralRules.saveUnit.currentMap = getMapName(currentMap);
   if (GeneralRules.saveUnit.overlayMap !== undefined)
      GeneralRules.saveUnit.overlayMap = getMapName(overlayMap);
   if (temporary)
      GeneralRules["save" + slot] = JSON.stringify(GeneralRules.saveUnit);
   else
      localStorage.setItem("save" + slot, JSON.stringify(GeneralRules.saveUnit));
   GeneralRules.saveUnit = null;
};

GeneralRules.prototype.loadGame = function(slot, temporary) {
   var data;
   if (temporary)
      data = GeneralRules["save" + slot];
   else
      data = localStorage.getItem("save" + slot);
   if (data == null) return;
   data = JSON.parse(data);
   for(var key in data.maps)
   {
      if (maps[key] == null)
         mapInitializers[key]();
      maps[key].setState(data.maps[key]);
   }
   if (data.allMaps)
   {
      for(var key in maps)
         if (data.maps[key] == null)
            delete maps[key];
   }
   if (data.counters != null) {
      for(var key in data.counters)
         counters[key].value = data.counters[key].value; // Tile definitions are linked to the original counter instance
   }
   if (data.currentMap !== undefined)
      this.switchToMap(data.currentMap, true);
   if (data.overlayMap !== undefined)
      this.setOverlay(data.overlayMap);
};

GeneralRules.prototype.deleteSave = function(slot, temporary) {
   if (temporary)
      delete GeneralRules["save" + slot];
   else
      localStorage.removeItem("save" + slot);
}

GeneralRules.prototype.saveExists = function(slot, temporary) {
   if (temporary)
      return GeneralRules["save" + slot] != null;
   else
      return localStorage.getItem("save" + slot) != null;
};

GeneralRules.prototype.includeMapInSaveUnit = function(mapName) {
   if (GeneralRules.saveUnit == null)
      GeneralRules.saveUnit = {};
   if (GeneralRules.saveUnit.maps == null)
      GeneralRules.saveUnit.maps = {};
   GeneralRules.saveUnit.maps[mapName] = null;
};

GeneralRules.prototype.excludeMapFromSaveUnit = function(mapName) {
   if ((GeneralRules.saveUnit == null) || (GeneralRules.saveUnit.maps == null))
      return;
   if (GeneralRules.saveUnit.maps[mapName] !== undefined)
      delete GeneralRules.saveUnit.maps[mapName];
}

GeneralRules.prototype.includeInSaveUnit = function(include) {
   if (GeneralRules.saveUnit == null)
      GeneralRules.saveUnit = {};

   switch (include) {
      case "AllMaps":
         GeneralRules.saveUnit.allMaps = true;
         break;
      case "AllCounters":
         GeneralRules.saveUnit.counters = {};
         for(key in counters)
            GeneralRules.saveUnit.counters[key] = null;
         break;
      case "WhichMapIsCurrent":
         GeneralRules.saveUnit.currentMap = null;
         break;
      case "WhichMapIsOverlaid":
         GeneralRules.saveUnit.overlayMap = null;
         break;
      case "PlayerOptions":
         // Not implemented
         break;
   }
};

GeneralRules.prototype.includeCounterInSaveUnit = function(counter) {
   if (GeneralRules.saveUnit == null)
      GeneralRules.saveUnit = {};
   if (GeneralTules.saveUnit.counters == null)
      GeneralRules.saveUnit.counters = {};
   GeneralRules.saveUnit.counters[key] = null;
}

GeneralRules.prototype.excludeCounterFromSaveUnit = function(counter) {
   if ((GeneralRules.saveUnit == null) || (GeneralRules.saveUnit.counters == null))
      return;
   for (key in GeneralRules.saveUnit.counters) {
      if (counters[key] === counter)
         delete GeneralRules.saveUnit.counters[key];
   }
};

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
         if (counter.value > counter.min) {
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
};

GeneralRules.prototype.setMapFlag = function(flagIndex, value) {
   if (this.layer.map.mapFlags == null)
      this.layer.map.mapFlags = 0;
   if (value)
      this.layer.map.mapFlags |= 1 << flagIndex;
   else
      this.layer.map.mapFlags &= ~(1 << flagIndex);
};

GeneralRules.prototype.isMapFlagOn = function(flagIndex) {
   if (this.layer.map.mapFlags == null)
      this.layer.map.mapFlags = 0;
   return ((this.layer.map.mapFlags & (1 << flagIndex)) != 0);
};

GeneralRules.prototype.setTargetMapFlag = function(mapName, flagIndex, value) {
   if (value)
      maps[mapName].mapFlags |= 1 << flagIndex;
   else
      maps[mapName].mapFlags &= ~(1 << flagIndex);
}

GeneralRules.prototype.clearOverlay = function() {
   overlayMap = null;
};

GeneralRules.prototype.clearAllMessages = function() {
   GeneralRules.activeMessages.length = 0;
};

GeneralRules.prototype.canReturnToPreviousMap = function() {
   return currentMap.cameFromMapName != null;
};

GeneralRules.prototype.returnToPreviousMap = function(unloadCurrent) {
   var source = currentMap.cameFromMapName;
   if (source == null)
      source = getMapName(currentMap);
   if (unloadCurrent)
      for(var key in maps)
         if (maps[key] == currentMap)
            delete maps[key];
   if (maps[source] === undefined)
      mapInitializers[source]();
   currentMap = maps[source];
};

GeneralRules.prototype.switchToMap = function(mapName, unloadCurrent) {
   var oldMapName = null;
   if (currentMap != null) {
      for(key in maps) {
         if (maps[key] === currentMap) {
            if (unloadCurrent)
               delete maps[key];
            oldMapName = key;
         }
      }
   }
   if (maps[mapName] === undefined)
      mapInitializers[mapName]();
   currentMap = maps[mapName];
   currentMap.cameFromMapName = oldMapName;
};

GeneralRules.prototype.isKeyPressed = function(key) {
   return keyboardState.isKeyPressed(key);
};

GeneralRules.prototype.setOverlay = function(mapName) {
   if (maps[mapName] === undefined)
      mapInitializers[mapName]();
   overlayMap = maps[mapName];
};

GeneralRules.prototype.unloadBackgroundMaps = function() {
   for(key in maps) {
      if ((maps[key] !== currentMap) && (maps[key] !== overlayMap))
         delete maps[key];
   }
};

GeneralRules.prototype.unloadMap = function(mapName) {
   delete maps[mapName];
}

GeneralRules.prototype.setMessageFont = function(tileset) {
   GeneralRules.fontTileset = tileset;
};

GeneralRules.colorNameToRgba = function(color, alpha) {
    var colors = {"aliceblue":"#f0f8ff","antiquewhite":"#faebd7","aqua":"#00ffff","aquamarine":"#7fffd4","azure":"#f0ffff",
    "beige":"#f5f5dc","bisque":"#ffe4c4","black":"#000000","blanchedalmond":"#ffebcd","blue":"#0000ff","blueviolet":"#8a2be2","brown":"#a52a2a","burlywood":"#deb887",
    "cadetblue":"#5f9ea0","chartreuse":"#7fff00","chocolate":"#d2691e","coral":"#ff7f50","cornflowerblue":"#6495ed","cornsilk":"#fff8dc","crimson":"#dc143c","cyan":"#00ffff",
    "darkblue":"#00008b","darkcyan":"#008b8b","darkgoldenrod":"#b8860b","darkgray":"#a9a9a9","darkgreen":"#006400","darkkhaki":"#bdb76b","darkmagenta":"#8b008b","darkolivegreen":"#556b2f",
    "darkorange":"#ff8c00","darkorchid":"#9932cc","darkred":"#8b0000","darksalmon":"#e9967a","darkseagreen":"#8fbc8f","darkslateblue":"#483d8b","darkslategray":"#2f4f4f","darkturquoise":"#00ced1",
    "darkviolet":"#9400d3","deeppink":"#ff1493","deepskyblue":"#00bfff","dimgray":"#696969","dodgerblue":"#1e90ff",
    "firebrick":"#b22222","floralwhite":"#fffaf0","forestgreen":"#228b22","fuchsia":"#ff00ff",
    "gainsboro":"#dcdcdc","ghostwhite":"#f8f8ff","gold":"#ffd700","goldenrod":"#daa520","gray":"#808080","green":"#008000","greenyellow":"#adff2f",
    "honeydew":"#f0fff0","hotpink":"#ff69b4",
    "indianred ":"#cd5c5c","indigo ":"#4b0082","ivory":"#fffff0","khaki":"#f0e68c",
    "lavender":"#e6e6fa","lavenderblush":"#fff0f5","lawngreen":"#7cfc00","lemonchiffon":"#fffacd","lightblue":"#add8e6","lightcoral":"#f08080","lightcyan":"#e0ffff","lightgoldenrodyellow":"#fafad2",
    "lightgrey":"#d3d3d3","lightgreen":"#90ee90","lightpink":"#ffb6c1","lightsalmon":"#ffa07a","lightseagreen":"#20b2aa","lightskyblue":"#87cefa","lightslategray":"#778899","lightsteelblue":"#b0c4de",
    "lightyellow":"#ffffe0","lime":"#00ff00","limegreen":"#32cd32","linen":"#faf0e6",
    "magenta":"#ff00ff","maroon":"#800000","mediumaquamarine":"#66cdaa","mediumblue":"#0000cd","mediumorchid":"#ba55d3","mediumpurple":"#9370d8","mediumseagreen":"#3cb371","mediumslateblue":"#7b68ee",
    "mediumspringgreen":"#00fa9a","mediumturquoise":"#48d1cc","mediumvioletred":"#c71585","midnightblue":"#191970","mintcream":"#f5fffa","mistyrose":"#ffe4e1","moccasin":"#ffe4b5",
    "navajowhite":"#ffdead","navy":"#000080",
    "oldlace":"#fdf5e6","olive":"#808000","olivedrab":"#6b8e23","orange":"#ffa500","orangered":"#ff4500","orchid":"#da70d6",
    "palegoldenrod":"#eee8aa","palegreen":"#98fb98","paleturquoise":"#afeeee","palevioletred":"#d87093","papayawhip":"#ffefd5","peachpuff":"#ffdab9","peru":"#cd853f","pink":"#ffc0cb","plum":"#dda0dd","powderblue":"#b0e0e6","purple":"#800080",
    "red":"#ff0000","rosybrown":"#bc8f8f","royalblue":"#4169e1",
    "saddlebrown":"#8b4513","salmon":"#fa8072","sandybrown":"#f4a460","seagreen":"#2e8b57","seashell":"#fff5ee","sienna":"#a0522d","silver":"#c0c0c0","skyblue":"#87ceeb","slateblue":"#6a5acd","slategray":"#708090","snow":"#fffafa","springgreen":"#00ff7f","steelblue":"#4682b4",
    "tan":"#d2b48c","teal":"#008080","thistle":"#d8bfd8","tomato":"#ff6347","turquoise":"#40e0d0",
    "violet":"#ee82ee",
    "wheat":"#f5deb3","white":"#ffffff","whitesmoke":"#f5f5f5",
    "yellow":"#ffff00","yellowgreen":"#9acd32"};

    return "rgba(" + parseInt(colors[color].substr(1,2), 16) + "," + parseInt(colors[color].substr(3,2), 16) + "," + parseInt(colors[color].substr(5,2), 16) + "," + alpha/255 + ")";
};

GeneralRules.prototype.setMessageBackground = function(color, alpha) {
   GeneralRules.messageBackground = GeneralRules.colorNameToRgba(color, alpha);
};

GeneralRules.prototype.setMessageDismissal = function(dismissButton, player) {
   GeneralRules.dismissButton = dismissButton;
   GeneralRules.currentPlayer = player - 1;
};

GeneralRules.prototype.showMessage = function(message) {
   if (GeneralRules.activeMessages.length >= GeneralRules.maxMessages)
      throw "Maximum number of displayed messages exceeded";
   else
      GeneralRules.activeMessages.push(this.createMessage(message));
};

function MessageLayer(tileset, map, columns, rows, offsetX, offsetY, background, player, dismissButton) {
   MapLayer.call(this, map, tileset, columns, rows, 0, 0, offsetX, offsetY, 0, 0, 0, null);
   this.background = background;
   this.dismissButton = dismissButton;
   this.player = player;
}

MessageLayer.prototype = new MapLayer();
MessageLayer.prototype.constructor = MessageLayer;

GeneralRules.playerPressButton = function(playerNumber) {
   for (var i = 0; i < GeneralRules.activeMessages.length; i++) {
      var msg = GeneralRules.activeMessages[i];
      if (msg.player == playerNumber - 1) {
         var player = players[playerNumber - 1];
         var dismissPressed = false;
         if ((msg.dismissButton & GeneralRules.buttonSpecifier.first) && player.button1())
            dismissPressed = true;
         if ((msg.dismissButton & GeneralRules.buttonSpecifier.second) && player.button2())
            dismissPressed = true;
         if ((msg.dismissButton & GeneralRules.buttonSpecifier.third) && player.button3())
            dismissPressed = true;
         if ((msg.dismissButton & GeneralRules.buttonSpecifier.fourth) && player.button4())
            dismissPressed = true;

         // dismissPhase[x]:
         // 0 = No frames have passed yet
         // 1 = Frames have passed and the dismiss button was initially pressed
         // 2 = Frames have passed and the dismiss button is not pressed
         // 3 = Dismiss button was not pressed, but now it is.

         if (GeneralRules.dismissPhase == null)
            GeneralRules.dismissPhase = [0,0,0,0];

         if (dismissPressed) {
            if ((GeneralRules.dismissPhase[msg.player] == 0) || (GeneralRules.dismissPhase[msg.player] == 2))
               GeneralRules.dismissPhase[msg.player]++;
         } else {
            if (GeneralRules.dismissPhase[msg.player] < 2)
               GeneralRules.dismissPhase[msg.player] = 2;
            else if (GeneralRules.dismissPhase[msg.player] > 2) {
               GeneralRules.dismissMessage(i);
               GeneralRules.dismissPhase[msg.player] = 0;
            }
         }

         if (msg.dismissButton & GeneralRules.buttonSpecifier.freezeInputs) {
            return false;
         }
      }
   }
   return true;
};

GeneralRules.dismissMessage = function (messageIndex) {
   GeneralRules.activeMessages.splice(messageIndex, 1);
};

GeneralRules.prototype.createMessage = function(message) {
   if (GeneralRules.fontTileset == null) {
      var tilesetKey;
      for (tilesetKey in tilesets)
         break;
      GeneralRules.fontTileset = tilesets[tilesetKey];
   }

   var x = 0, y = 1;
   var maxWidth = 1;
   for (var charIdx = 0; charIdx < message.length; charIdx++) {
      if (message[charIdx] == '\n') {
         x = 0;
         y++;
      } else if (message[charIdx] != '\r') {
         if (++x > maxWidth)
            maxWidth = x;
      }
   }

   var messageSize = {width: maxWidth * GeneralRules.fontTileset.tileWidth, height: y * GeneralRules.fontTileset.tileHeight};
   var messageX = (viewWidth - messageSize.width) / 2;
   var messageY = (viewHeight - messageSize.height) / 2;

   var result = new MessageLayer(
      GeneralRules.fontTileset, this.layer.map, maxWidth, y, messageX, messageY,
      GeneralRules.messageBackground, GeneralRules.currentPlayer, GeneralRules.dismissButton);

   x = 0;
   y = 0;
   for (var charIdx = 0; charIdx < message.length; charIdx ++) {
      if (message.charAt(charIdx) == '\n') {
         x = 0;
         y++;
      } else if (message.charAt(charIdx) != '\r') {
         result.setTile(x++, y, message.charCodeAt(charIdx));
      }
   }

   return result;
}

GeneralRules.drawMessage = function(msg) {
   var messageRect = {
      x: msg.currentX - GeneralRules.messageMargin,
      y: msg.currentY - GeneralRules.messageMargin,
      width: msg.virtualColumns * msg.tileset.tileWidth + GeneralRules.messageMargin * 2,
      height: msg.virtualRows * msg.tileset.tileHeight + GeneralRules.messageMargin * 2};
   gameViewContext.fillStyle = msg.background;
   gameViewContext.fillRect(messageRect.x, messageRect.y, messageRect.width, messageRect.height);
   gameViewContext.strokeStyle = "#ffffff";
   gameViewContext.lineWidth = 2;
   gameViewContext.strokeRect(messageRect.x, messageRect.y, messageRect.width, messageRect.height);
   msg.draw(gameViewContext);
};

GeneralRules.drawMessages = function() {
   for (var i = 0; i < GeneralRules.activeMessages.length; i++) {
      var msg = GeneralRules.activeMessages[i];
      GeneralRules.drawMessage(msg);
   }
};

GeneralRules.prototype.limitFrameRate = function(fps) {
   if (fps == 0) {
      mainLoop.milliseconds = 0;
      if (mainLoop.interval != null)
         clearInterval(mainLoop.interval);
      mainLoop.interval = null;
      return;
   }

   var milliseconds = Math.ceil(1000 / fps);
   if (milliseconds != mainLoop.milliseconds) {
      if (mainLoop.interval != null)
         clearInterval(mainLoop.interval);
      mainLoop.milliseconds = milliseconds;
      mainLoop.interval = setInterval("pulse()", mainLoop.milliseconds);
   }
};

GeneralRules.prototype.setCategorySpriteState = function(category, spriteIndex, state) {
   category[spriteIndex].state = state;
};

GeneralRules.prototype.quitGame = function() {
   window.close();
};

GeneralRules.prototype.getRandomNumber = function(minimum, maximum) {
   return Math.floor(Math.random() * (maximum - minimum)) + minimum;
};