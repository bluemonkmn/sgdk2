var gameViewContext;
var dragX, dragY;
var currentMap;
var overlayMap;
var mainLoop = {interval:null, milliseconds:20};
function startGame() {
   initGraphicSheets();
   initFramesets();
   initTilesets();
   initTileCategories();
   firstMap();
   var gameView = document.getElementById('gameView');
   gameViewContext = gameView.getContext('2d');
   mainLoop.interval = setInterval("pulse()", mainLoop.milliseconds);
}
function pulse() {
   if (currentMap != null)
   {
      currentMap.draw(gameViewContext);
      currentMap.executeRules();
   }
   GeneralRules.drawMessages();
   if (overlayMap != null)
   {
      overlayMap.draw(gameViewContext);
      overlayMap.executeRules();
   }
}

function resizeView() {
   viewWidth = window.innerWidth;
   viewHeight = window.innerHeight;
   var gameView = document.getElementById('gameView');
   gameView.width = viewWidth;
   gameView.height = viewHeight;
   if ((gameViewContext != null) && (currentMap != null))
      currentMap.draw(gameViewContext);
}
function beginDrag(e) {
   dragX = e.clientX;
   dragY = e.clientY;
   var srcEl = e.srcElement ? e.srcElement : e.target;
   srcEl.onmousemove = processDrag;
   return false;
}
function endDrag(e) {
   var srcEl = e.srcElement ? e.srcElement : e.target;
   srcEl.onmousemove = null;
}
function processDrag(e) {
   e = e || window.event;
   drag(e.clientX, e.clientY);
   return false;
}
function drag(newX, newY) {
   currentMap.scroll(currentMap.scrollX + newX - dragX, currentMap.scrollY + newY - dragY);
   dragX = newX;
   dragY = newY;
   currentMap.draw(gameViewContext);
}
function beginTouchDrag(e) {
   var touches = e.touches;
   var touch = touches.item(0);
   dragX = touch.clientX;
   dragY = touch.clientY;
   return false;
}
function processTouchDrag(e) {
   var touches = e.touches;
   var touch = touches.item(0);
   drag(touch.clientX, touch.clientY);
   return false;
}
function truncate(n) {
   return n | 0;
}