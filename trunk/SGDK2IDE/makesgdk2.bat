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
csc /o+ /target:library /out:Reflect.dll Reflect\RemotingServices.cs Reflect\AssemblyInfo.cs
echo >Input.txt /o+ /target:winexe /win32icon:App.ico
;echo >>Input.txt /lib:%windir%\assembly\gac\Microsoft.DirectX\1.0.2902.0__31bf3856ad364e35
;echo >>Input.txt /lib:%windir%\assembly\gac\Microsoft.DirectX.Direct3D\1.0.2902.0__31bf3856ad364e35
;echo >>Input.txt /lib:%windir%\assembly\gac\Microsoft.DirectX.Direct3DX\1.0.2911.0__31bf3856ad364e35
echo >>Input.txt "/lib:%windir%\microsoft.net\managed directx\v9.00.1126"
echo >>Input.txt /r:System.Data.dll
echo >>Input.txt /r:System.dll
echo >>Input.txt /r:System.Drawing.dll
echo >>Input.txt /r:System.Windows.Forms.Dll
echo >>Input.txt /r:System.XML.dll
echo >>Input.txt /r:Microsoft.DirectX.dll
echo >>Input.txt /r:Microsoft.DirectX.Direct3D.dll
echo >>Input.txt /r:Microsoft.DirectX.Direct3DX.dll
echo >>Input.txt /r:Microsoft.DirectX.DirectInput.dll
echo >>Input.txt /r:Reflect.dll
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
echo >>Input.txt /res:SGDK2.frmSpriteCategories.resources
echo >>Input.txt /res:SGDK2.frmCodeEditor.resources
echo >>Input.txt /res:SGDK2.frmPlanEdit.resources
echo >>Input.txt /res:SGDK2.frmUnsavedChanges.resources
echo >>Input.txt /res:SGDK2.frmImportGraphics.resources
echo >>Input.txt /res:SGDK2.frmProject.resources
echo >>Input.txt /res:SGDK2.frmLogView.resources
echo >>Input.txt /res:SGDK2.frmCollisionMask.resources
echo >>Input.txt /res:SGDK2.frmFindReplace.resources
echo >>Input.txt /res:SGDK2.frmInputBox.resources
echo >>Input.txt /res:SGDK2.frmTileCategoryName.resources
echo >>Input.txt /res:SGDK2.frmSpriteImportWizard.resources
echo >>Input.txt /res:SGDK2.frmAbout.resources
echo >>Input.txt /res:SGDK2.frmWizardBase.resources
echo >>Input.txt /res:SGDK2.frmSelectTileset.resources
echo >>Input.txt /res:SGDK2.frmMapScrollWizard.resources
echo >>Input.txt /res:SGDK2.frmLayerWizard.resources
echo >>Input.txt /res:SGDK2.frmCustomObjectData.resources
echo >>Input.txt /res:SGDK2.frmCodeImport.resources
echo >>Input.txt /res:SGDK2.frmSoundPlayer.resources
echo >>Input.txt /res:SGDK2.frmHueMapParams.resources
echo >>Input.txt /res:SGDK2.frmGfxSheetImport.resources
echo >>Input.txt /res:SGDK2.frmTilesetImport.resources
echo >>Input.txt /res:Template\Display.cs,SGDK2.Template.Display.cs
echo >>Input.txt /res:Template\Frame.cs,SGDK2.Template.Frame.cs
echo >>Input.txt /res:Template\LayerBase.cs,SGDK2.Template.LayerBase.cs
echo >>Input.txt /res:Template\SpriteBase.cs,SGDK2.Template.SpriteBase.cs
echo >>Input.txt /res:Template\SpriteCollection.cs,SGDK2.Template.SpriteCollection.cs
echo >>Input.txt /res:Template\Tile.cs,SGDK2.Template.Tile.cs
echo >>Input.txt /res:Template\GameForm.cs,SGDK2.Template.GameForm.cs
echo >>Input.txt /res:Template\MapBase.cs,SGDK2.Template.MapBase.cs
echo >>Input.txt /res:Template\PlanBase.cs,SGDK2.Template.PlanBase.cs
echo >>Input.txt /res:Template\SpriteState.cs,SGDK2.Template.SpriteState.cs
echo >>Input.txt /res:Template\TileCategoryMembership.cs,SGDK2.Template.TileCategoryMembership.cs
echo >>Input.txt /res:Template\TileShapes.cs,SGDK2.Template.TileShapes.cs
echo >>Input.txt /res:Template\GeneralRules.cs,SGDK2.Template.GeneralRules.cs
echo >>Input.txt /res:Template\CollisionMask.cs,SGDK2.Template.CollisionMask.cs
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
echo >>Input.txt DataChangeNotifier.cs
echo >>Input.txt NewTileValue.cs
echo >>Input.txt CounterEdit.cs
echo >>Input.txt MapManager.cs
echo >>Input.txt MapEditor.cs
echo >>Input.txt LayerManager.cs
echo >>Input.txt SpriteDefinition.cs
echo >>Input.txt Solidity.cs
echo >>Input.txt TileCategory.cs
echo >>Input.txt DataGridComboBox.cs
echo >>Input.txt SpriteCache.cs
echo >>Input.txt SpriteConverter.cs
echo >>Input.txt SpriteCategories.cs
echo >>Input.txt CodeEditor.cs
echo >>Input.txt CodeGenerator.cs
echo >>Input.txt PlanConverter.cs
echo >>Input.txt PlanEdit.cs
echo >>Input.txt UnsavedChanges.cs
echo >>Input.txt ImportGraphics.cs
echo >>Input.txt Project.cs
echo >>Input.txt LogView.cs
echo >>Input.txt CollisionMask.cs
echo >>Input.txt FindReplace.cs
echo >>Input.txt InputBox.cs
echo >>Input.txt TileCategoryName.cs
echo >>Input.txt SpriteWizard.cs
echo >>Input.txt About.cs
echo >>Input.txt WizardBase.cs
echo >>Input.txt SelectTileset.cs
echo >>Input.txt MapScrollWizard.cs
echo >>Input.txt LayerWizard.cs
echo >>Input.txt CustomObjectData.cs
echo >>Input.txt CodeImport.cs
echo >>Input.txt SoundPlayer.cs
echo >>Input.txt fmod.cs
echo >>Input.txt SelectSpriteParameter.cs
echo >>Input.txt HueMapParams.cs
echo >>Input.txt GfxSheetImport.cs
echo >>Input.txt TilesetImport.cs
csc /noconfig @Input.txt
del Input.txt
del *.resources
del CompileResx.exe
if not exist SGDK2IDE.exe echo Errors occurred while compiling SGDK2IDE.exe.
if exist SGDK2IDE.exe echo SGDK2IDE.exe compiled.