function Map(scrollWidth, scrollHeight, scrollMarginLeft, scrollMarginTop, scrollMarginRight, scrollMarginBottom) {
   this.scrollX = 0;
   this.scrollY = 0;
   this.scrollWidth = scrollWidth;
   this.scrollHeight = scrollHeight;
   this.scrollMarginLeft = scrollMarginLeft;
   this.scrollMarginTop = scrollMarginTop;
   this.scrollMarginRight = scrollMarginRight;
   this.scrollMarginBottom = scrollMarginBottom;
   this.layers = {};
}

function getMapName(map) {
   for(var key in maps) {
      if (maps[key] === map)
         return key;
   }
   return null;
}

Map.prototype.scroll = function(x, y) {
   if(x < viewWidth - this.scrollWidth) x = viewWidth - this.scrollWidth;
   if(x > 0) x = 0;
   if(y < viewHeight - this.scrollHeight) y = viewHeight - this.scrollHeight;
   if(y > 0) y = 0;
   this.scrollX = x;
   this.scrollY = y;
   for(var key in this.layers) {
      this.layers[key].currentX = this.layers[key].offsetX + Math.floor(x * this.layers[key].scrollRateX);
      this.layers[key].currentY = this.layers[key].offsetY + Math.floor(y * this.layers[key].scrollRateY);
   }
};

Map.prototype.draw = function(ctx) {
   for(var key in this.layers)
      this.layers[key].draw(ctx);
};

Map.prototype.getState = function() {
   var result = {layers:{},cameFromMapName:this.cameFromMapName,mapFlags:this.mapFlags};
   for(var key in this.layers)
      result.layers[key] = this.layers[key].getState();
   return result;
};

Map.prototype.setState = function(data) {
   for(var key in this.layers)
      this.layers[key].setState(data.layers[key]);
   this.cameFromMapName = data.cameFromMapName;
   this.mapFlags = data.mapFlags;
};

Map.prototype.executeRules = function() {
   for(var key in this.layers)
      this.layers[key].executeRules();
};