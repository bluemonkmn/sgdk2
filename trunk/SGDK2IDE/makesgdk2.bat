@echo off
echo [ Scrolling Game Development Kit 2.0                            ]
echo [                                                               ]
echo [ Copyright (c) 2000-2005 Benjamin Marty                        ]
echo [ (BlueMonkMN@users.sourceforge.net)                            ]
echo [                                                               ]
echo [ Distributed under the GNU General Public License (GPL)        ]
echo [  - see included file COPYING.txt for details, or visit:       ]
echo [    http://www.fsf.org/copyleft/gpl.html                       ]

path %windir%;%windir%\System32;%windir%\Microsoft.NET\Framework\v1.1.4322
csc CompileResx.cs
CompileResx.exe
del SGDK2IDE.exe
echo >Input.txt /o+ /target:winexe /win32icon:App.ico
;echo >>Input.txt /lib:%windir%\assembly\gac\Microsoft.DirectX\1.0.2902.0__31bf3856ad364e35
;echo >>Input.txt /lib:%windir%\assembly\gac\Microsoft.DirectX.Direct3D\1.0.2902.0__31bf3856ad364e35
;echo >>Input.txt /lib:%windir%\assembly\gac\Microsoft.DirectX.Direct3DX\1.0.2902.0__31bf3856ad364e35
echo >>Input.txt "/lib:%windir%\microsoft.net\managed directx\v9.00.1126"
echo >>Input.txt /r:System.Data.dll
echo >>Input.txt /r:System.dll
echo >>Input.txt /r:System.Drawing.dll
echo >>Input.txt /r:System.Windows.Forms.Dll
echo >>Input.txt /r:System.XML.dll
echo >>Input.txt /r:Microsoft.DirectX.dll
echo >>Input.txt /r:Microsoft.DirectX.Direct3D.dll
echo >>Input.txt /r:Microsoft.DirectX.Direct3DX.dll
echo >>Input.txt /res:SGDK2.SGDK2IDE.resources
echo >>Input.txt /res:SGDK2.frmMain.resources
echo >>Input.txt /res:SGDK2.frmGraphicsEditor.resources
echo >>Input.txt /res:SGDK2.frmGraphicPane.resources
echo >>Input.txt /res:SGDK2.frmMainWindow.resources
echo >>Input.txt /res:SGDK2.frmFrameEdit.resources
echo >>Input.txt /res:SGDK2.ColorSel.resources
echo >>Input.txt /res:SGDK2.frmGfxSheet.resources
echo >>Input.txt /res:SGDK2.frmMapManager.resources
echo >>Input.txt /res:SGDK2.frmMapEditor.resources
echo >>Input.txt /res:SGDK2.frmLayerManager.resources
echo >>Input.txt /res:SGDK2.frmTileEdit.resources
echo >>Input.txt /res:SGDK2.frmSpriteDefinition.resources
echo >>Input.txt /res:SGDK2.frmSolidity.resources
echo >>Input.txt /res:SGDK2.frmTileCategory.resources
echo >>Input.txt /res:SGDK2.frmShape.resources
echo >>Input.txt /res:SGDK2.frmSpriteCategories.resources
echo >>Input.txt /res:SGDK2.frmCodeEditor.resources
echo >>Input.txt /res:Template\Display.cs,SGDK2.Template.Display.cs
echo >>Input.txt /res:Template\Frame.cs,SGDK2.Template.Frame.cs
echo >>Input.txt /res:Template\LayerBase.cs,SGDK2.Template.LayerBase.cs
echo >>Input.txt /res:Template\SpriteBase.cs,SGDK2.Template.SpriteBase.cs
echo >>Input.txt /res:Template\SpriteCollection.cs,SGDK2.Template.SpriteCollection.cs
echo >>Input.txt /res:Template\Tile.cs,SGDK2.Template.Tile.cs
echo >>Input.txt /res:RemoteReflector.cs,SGDK2.RemoteReflector.cs
echo >>Input.txt AssemblyInfo.cs
echo >>Input.txt GfxEdit.cs
echo >>Input.txt GfxPane.cs
echo >>Input.txt MainWindow.cs
echo >>Input.txt SGDK2IDE.cs
echo >>Input.txt ColorSel.cs
echo >>Input.txt CustTool.cs
echo >>Input.txt GfxSheet.cs
echo >>Input.txt ProjectDataset.cs
echo >>Input.txt CellMgr.cs
echo >>Input.txt FrameEdit.cs
echo >>Input.txt GraphicBrowser.cs
echo >>Input.txt DragPanel.cs
echo >>Input.txt SplashForm.cs
echo >>Input.txt ProjectData.cs
echo >>Input.txt Display.cs
echo >>Input.txt Layer.cs
echo >>Input.txt TileEdit.cs
echo >>Input.txt FrameCache.cs
echo >>Input.txt TileCache.cs
echo >>Input.txt Map.cs
echo >>Input.txt DataChangeNotifier.cs
echo >>Input.txt NewTileValue.cs
echo >>Input.txt CounterEdit.cs
echo >>Input.txt MapManager.cs
echo >>Input.txt MapEditor.cs
echo >>Input.txt LayerManager.cs
echo >>Input.txt SpriteDefinition.cs
echo >>Input.txt Solidity.cs
echo >>Input.txt Shape.cs
echo >>Input.txt TileCategory.cs
echo >>Input.txt DataGridComboBox.cs
echo >>Input.txt RemotingServices.cs
echo >>Input.txt SpriteCache.cs
echo >>Input.txt SpriteConverter.cs
echo >>Input.txt SpriteCategories.cs
echo >>Input.txt CodeEditor.cs
echo >>Input.txt CodeGenerator.cs
csc /noconfig @Input.txt
del Input.txt
del *.resources
del CompileResx.exe
if not exist SGDK2IDE.exe echo Errors occurred while compiling SGDK2IDE.exe.
if exist SGDK2IDE.exe echo SGDK2IDE.exe compiled.