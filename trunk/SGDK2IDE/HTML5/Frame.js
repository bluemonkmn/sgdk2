function Frameset(name, frames) {
   this.name = name;
   this.frames = frames;
}
function XFrame(m11, m12, m21, m22, dx, dy, graphicSheet, imageSource, cellIndex) {
   this.m11 = m11;
   this.m12 = m12;
   this.m21 = m21;
   this.m22 = m22;
   this.dx = dx;
   this.dy = dy;
   this.graphicSheet = graphicSheet;
   this.imageSource = imageSource;
   this.cellIndex = cellIndex;
}
function Frame(graphicSheet, imageSource, cellIndex) {
   this.graphicSheet = graphicSheet;
   this.imageSource = imageSource;
   this.cellIndex = cellIndex;
}
Frame.prototype.draw = function(ctx, x, y) {
   if (this.imageSource == null) return;
   ctx.drawImage(this.imageSource, (this.cellIndex % this.graphicSheet.columns) * this.graphicSheet.cellWidth,
   Math.floor(this.cellIndex / this.graphicSheet.columns) * this.graphicSheet.cellHeight,
   this.graphicSheet.cellWidth, this.graphicSheet.cellHeight, x, y, this.graphicSheet.cellWidth, this.graphicSheet.cellHeight);
};
XFrame.prototype.draw = function(ctx, x, y) {
   ctx.save();
   ctx.transform(this.m11, this.m12, this.m21, this.m22, this.dx+x, this.dy+y);
   ctx.drawImage(this.imageSource, (this.cellIndex % this.graphicSheet.columns) * this.graphicSheet.cellWidth,
      Math.floor(this.cellIndex / this.graphicSheet.columns) * this.graphicSheet.cellHeight,
      this.graphicSheet.cellWidth, this.graphicSheet.cellHeight, 0, 0, this.graphicSheet.cellWidth, this.graphicSheet.cellHeight);
   ctx.restore();
};
function ModulateCelColor(target, x, y, width, height, r, g, b, a) {
   var cel;
   try { cel = target.getImageData(x, y, width, height); }
   catch(e) {
      document.write('Failed to process images. This may occur when running from local files; see <a href="http://stackoverflow.com/questions/2704929/uncaught-error-security-err-dom-exception-18">see details</a>');
      throw(e);
   }
   var celData = cel.data;
   for (yi = 0; yi < height; yi++) {
      for (xi = 0; xi < width; xi++) {
         var byteIdx = (yi * width + xi) * 4;
         celData[byteIdx] = Math.floor(celData[byteIdx] * r / 255);
         celData[byteIdx+1] = Math.floor(celData[byteIdx+1] * g / 255);
         celData[byteIdx+2] = Math.floor(celData[byteIdx+2] * b / 255);
         celData[byteIdx+3] = Math.floor(celData[byteIdx+3] * a / 255);
      }
   }
   target.putImageData(cel, x, y);
}