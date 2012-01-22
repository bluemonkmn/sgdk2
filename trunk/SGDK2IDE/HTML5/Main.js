﻿var gameViewContext;
var mouseInfo = {x: 0,y:0,pressed:false,oldX:0,oldY:0,clicked:false};
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

   gameView.onmousedown = function(e) {
      e = e || window.event;
      mouseInfo.x = e.clientX;
      mouseInfo.y = e.clientY;
      mouseInfo.pressed = true;
      mouseInfo.clicked = true;
   };

   gameView.onmousemove = function(e) {
      e = e || window.event;
      mouseInfo.x = e.clientX;
      mouseInfo.y = e.clientY;
   };

   gameView.onmouseup = function(e) {
      mouseInfo.pressed = false;
   };

   gameView.onmouseout = function(e) {
      mouseInfo.pressed = false;
   };

   gameView.touchstart = function(e) {
      e = e || window.event;
      var touch = e.touches.item(0);
      mouseInfo.x = touch.clientX;
      mouseInfo.y = touch.clientY;
      mouseInfo.pressed = true;
      mouseInfo.clicked = true;
   };

   gameView.touchmove = function(e) {
      e = e || window.event;
      var touch = e.touches.item(0);
      mouseInfo.x = touch.clientX;
      mouseInfo.y = touch.clientY;
      mouseInfo.pressed = true;
   };
   
   gameView.touchend = function(e) {
      mouseInfo.pressed = false;
   }

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
   cycleMouseInfo();
}

function cycleMouseInfo() {
   mouseInfo.oldX = mouseInfo.x;
   mouseInfo.oldY = mouseInfo.y;
   mouseInfo.clicked = false;
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

function truncate(n) {
   return n | 0;
}
