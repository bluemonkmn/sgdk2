class CompileResx
{
      public static void Main()
      {
         System.Resources.ResXResourceReader rd =
            new System.Resources.ResXResourceReader("SGDK2IDE.resx");
         WriteResources(rd, "SGDK2.SGDK2IDE.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("MainWindow.resx");
         WriteResources(rd,"SGDK2.frmMain.resources");
         rd.Close();         
         rd = new System.Resources.ResXResourceReader("GfxEdit.resx");
         WriteResources(rd,"SGDK2.frmGraphicsEditor.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("GfxPane.resx");
         WriteResources(rd,"SGDK2.frmGraphicPane.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("MainWindow.resx");
         WriteResources(rd,"SGDK2.frmMainWindow.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("ColorSel.resx");
         WriteResources(rd,"SGDK2.ColorSel.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("GfxSheet.resx");
         WriteResources(rd,"SGDK2.frmGfxSheet.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("FrameEdit.resx");
         WriteResources(rd,"SGDK2.frmFrameEdit.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("MapManager.resx");
         WriteResources(rd,"SGDK2.frmMapManager.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("MapEditor.resx");
         WriteResources(rd,"SGDK2.frmMapEditor.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("LayerManager.resx");
         WriteResources(rd,"SGDK2.frmLayerManager.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("TileEdit.resx");
         WriteResources(rd,"SGDK2.frmTileEdit.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("SpriteDefinition.resx");
         WriteResources(rd,"SGDK2.frmSpriteDefinition.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("Solidity.resx");
         WriteResources(rd,"SGDK2.frmSolidity.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("TileCategory.resx");
         WriteResources(rd,"SGDK2.frmTileCategory.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("CodeEditor.resx");
         WriteResources(rd,"SGDK2.frmCodeEditor.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("SpriteCategories.resx");
         WriteResources(rd,"SGDK2.frmSpriteCategories.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("PlanEdit.resx");
         WriteResources(rd,"SGDK2.frmPlanEdit.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("UnsavedChanges.resx");
         WriteResources(rd,"SGDK2.frmUnsavedChanges.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("ImportGraphics.resx");
         WriteResources(rd,"SGDK2.frmImportGraphics.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("Project.resx");
         WriteResources(rd,"SGDK2.frmProject.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("LogView.resx");
         WriteResources(rd,"SGDK2.frmLogView.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("CollisionMask.resx");
         WriteResources(rd,"SGDK2.frmCollisionMask.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("FindReplace.resx");
         WriteResources(rd,"SGDK2.frmFindReplace.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("InputBox.resx");
         WriteResources(rd,"SGDK2.frmInputBox.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("TileCategoryName.resx");
         WriteResources(rd,"SGDK2.frmTileCategoryName.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("SpriteWizard.resx");
         WriteResources(rd,"SGDK2.frmSpriteImportWizard.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("About.resx");
         WriteResources(rd,"SGDK2.frmAbout.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("WizardBase.resx");
         WriteResources(rd,"SGDK2.frmWizardBase.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("SelectTileset.resx");
         WriteResources(rd,"SGDK2.frmSelectTileset.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("MapScrollWizard.resx");
         WriteResources(rd,"SGDK2.frmMapScrollWizard.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("LayerWizard.resx");
         WriteResources(rd,"SGDK2.frmLayerWizard.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("CustomObjectData.resx");
         WriteResources(rd,"SGDK2.frmCustomObjectData.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("CodeImport.resx");
         WriteResources(rd,"SGDK2.frmCodeImport.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("SoundPlayer.resx");
         WriteResources(rd,"SGDK2.frmSoundPlayer.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("SelectSpriteParameter.resx");
         WriteResources(rd,"SGDK2.frmSelectSpriteParameter.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("HueMapParams.resx");
         WriteResources(rd,"SGDK2.frmHueMapParams.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("GfxSheetImport.resx");
         WriteResources(rd,"SGDK2.frmGfxSheetImport.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("TilesetImport.resx");
         WriteResources(rd,"SGDK2.frmTilesetImport.resources");
         rd.Close();
         rd = new System.Resources.ResXResourceReader("FramesetImport.resx");
         WriteResources(rd,"SGDK2.frmFramesetImport.resources");
         rd.Close();
      }
      public static void WriteResources(System.Resources.ResXResourceReader rd, string sFile)
      {
         System.Resources.ResourceWriter rw = new System.Resources.ResourceWriter(sFile);
         foreach (System.Collections.DictionaryEntry de in rd)
         {
            rw.AddResource(de.Key.ToString(), de.Value);
            //System.Console.WriteLine("Writing {0}",de.Key.ToString());
         }
         rw.Generate();
         rw.Close();
      }
}