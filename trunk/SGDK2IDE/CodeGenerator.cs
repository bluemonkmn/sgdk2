using System;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace SGDK2
{
	/// <summary>
	/// Static methods for creating code from project data
	/// </summary>
   public class CodeGenerator
   {
      public CodeGenerator()
      {
      }

      private const string ResourcesField = "m_res";
      private const string ResourcesProperty = "m_res";
      private const string ProjectClass = "Project";
      private const string FramesetClass = "Frameset";
      private const string FrameClass = "Frame";
      private const string CounterClass = "Counter";
      private const string CounterValFld = "m_nValue";
      private const string CounterMaxFld = "m_nMax";
      private const string CounterValProp = "CurrentValue";
      private const string CounterMaxProp = "MaxValue";
      private const string TilesetClass = "Tileset";
      private const string TileListVar = "TileList";
      private const string TileBaseClass = "TileBase";
      private const string AnimTileClass = "AnimTile";
      private const string SimpleTileClass = "SimpleTile";
      private const string EmptyTileClass = "EmptyTile";
      private const string TilesField = "m_Tiles";
      private const string TileWidthField = "m_nTileWidth";
      private const string TileHeightField = "m_nTileHeight";
      private const string TilesetFramesetField = "m_strFrameset";
      private const string MapDisplayField = "m_Display";

      public System.CodeDom.Compiler.ICodeGenerator Generator = new Microsoft.CSharp.CSharpCodeProvider().CreateGenerator();
      public CodeGeneratorOptions GeneratorOptions = new CodeGeneratorOptions();

      public void GenerateAllCode(string FolderName)
      {
         if (!System.IO.Path.IsPathRooted(FolderName))
         {
            FolderName = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, FolderName);
         }
         if (!System.IO.Directory.Exists(FolderName))
            System.IO.Directory.CreateDirectory(FolderName);

         System.IO.TextWriter err = new System.IO.StringWriter();

         System.IO.TextWriter txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, ProjectClass + ".cs"));
         GenerateMainCode(txt);
         txt.Close();

         txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, ProjectClass + ".resx"));
         GenerateResx(txt);
         txt.Close();

         txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, CounterClass + ".cs"));
         GenerateCounters(txt);
         txt.Close();

         txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, FramesetClass + ".cs"));
         GenerateFramesets(txt);
         txt.Close();

         txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, TilesetClass + ".cs"));
         GenerateTilesets(txt, err);
         txt.Close();

         foreach (ProjectDataset.MapRow drMap in ProjectData.Map)
         {
            txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, "Map" + drMap.Name + ".resx"));
            GenerateMapResx(drMap, txt);
            txt.Close();

            txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, "Map" + drMap.Name + ".cs"));
            GenerateMap(drMap, txt);
            txt.Close();
         }

         GenerateProjectSourceCode(FolderName);

         string errs = err.ToString();err.Close();

         if (errs.Length > 0)
            System.Windows.Forms.MessageBox.Show(errs, "Error");

      }

      public void GenerateMainCode(System.IO.TextWriter txt)
      {
         CodeTypeDeclaration typ = new CodeTypeDeclaration(ProjectClass);
         CodeMemberField resDecl = new CodeMemberField(typeof(System.Resources.ResourceManager), ResourcesField);
         resDecl.Attributes |= MemberAttributes.Static;
         typ.Members.Add(resDecl);
         CodeEntryPointMethod main = new CodeEntryPointMethod();
         typ.Members.Add(main);
         CodeFieldReferenceExpression resRef = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(ProjectClass), ResourcesField);
         CodeObjectCreateExpression createResources = new CodeObjectCreateExpression(typeof(System.Resources.ResourceManager), new CodeTypeOfExpression(ProjectClass));
         CodeAssignStatement assign = new CodeAssignStatement(resRef, createResources);
         main.Statements.Add(assign);

         CodeMemberProperty prpRes = new CodeMemberProperty();
         prpRes.HasSet = false;
         prpRes.HasGet = true;
         prpRes.Name = "Resources";
         prpRes.Attributes = MemberAttributes.Final | MemberAttributes.Public | MemberAttributes.Static;
         prpRes.Type = new CodeTypeReference(typeof(System.Resources.ResourceManager));
         prpRes.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(ResourcesField)));
         typ.Members.Add(prpRes);

         Generator.GenerateCodeFromType(typ, txt, GeneratorOptions);
      }

      public void GenerateResx(System.IO.TextWriter txt)
      {
         System.Resources.ResXResourceWriter w = new System.Resources.ResXResourceWriter(txt);
         try
         {
            foreach(ProjectDataset.GraphicSheetRow drGfx in ProjectData.GraphicSheet)
            {
               w.AddResource(drGfx.Name, ProjectData.GetGraphicSheetImage(drGfx.Name, false));
            }
         }
         finally
         {
            w.Close();
         }
      }

      public void GenerateFramesets(System.IO.TextWriter txt)
      {
         CodeTypeDeclaration framesetClassDecl = new CodeTypeDeclaration(FramesetClass);
         framesetClassDecl.Members.Add(new CodeMemberField(new CodeTypeReference(FrameClass, 1), "m_arFrames"));
         framesetClassDecl.Members.Add(new CodeMemberField("Display", "m_Display"));
         CodeConstructor framesetConstructor = new CodeConstructor();
         framesetConstructor.Attributes = MemberAttributes.Final | MemberAttributes.Public;
         framesetConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "Name"));
         framesetConstructor.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference("Display"), "disp"));
         framesetClassDecl.Members.Add(framesetConstructor);

         CodeConditionStatement topCondition = new CodeConditionStatement();
         CodeConditionStatement curCondition = topCondition;

         foreach(ProjectDataset.FramesetRow drFrameset in ProjectData.Frameset)
         {
            if (curCondition.TrueStatements.Count > 0) curCondition.FalseStatements.Add(curCondition = new CodeConditionStatement());
            CodeVariableReferenceExpression nameParam = new CodeVariableReferenceExpression("Name");
            CodePrimitiveExpression nameCompare = new CodePrimitiveExpression(drFrameset.Name);
            CodeBinaryOperatorExpression DoCompare = new CodeBinaryOperatorExpression(nameParam, CodeBinaryOperatorType.ValueEquality, nameCompare);
            curCondition.Condition = DoCompare;
            CodeVariableReferenceExpression dispParam = new CodeVariableReferenceExpression("disp");
            System.Collections.ArrayList frameParams = new System.Collections.ArrayList();
            foreach(ProjectDataset.FrameRow drFrame in ProjectData.GetSortedFrameRows(drFrameset))
            {
               CodeMethodInvokeExpression getTextureExp = new CodeMethodInvokeExpression(dispParam, "GetTexture", new CodePrimitiveExpression(drFrame.GraphicSheet));
               CodePrimitiveExpression cellExp = new CodePrimitiveExpression(drFrame.CellIndex);
               ProjectDataset.GraphicSheetRow g = ProjectData.GetGraphicSheet(drFrame.GraphicSheet);
               Microsoft.DirectX.Matrix im = Microsoft.DirectX.Matrix.Identity;
               CodeObjectCreateExpression rectExp = new CodeObjectCreateExpression(
                  typeof(System.Drawing.Rectangle),
                  new CodePrimitiveExpression((drFrame.CellIndex % g.Columns) * g.CellWidth),
                  new CodePrimitiveExpression(((int)(drFrame.CellIndex / g.Columns)) * g.CellHeight),
                  new CodePrimitiveExpression(g.CellWidth),
                  new CodePrimitiveExpression(g.CellHeight));
               if ((drFrame.m11 == im.M11) &&
                  (drFrame.m12 == im.M12) &&
                  (drFrame.m21 == im.M21) &&
                  (drFrame.m22 == im.M22) &&
                  (drFrame.dx == im.M41) &&
                  (drFrame.dy == im.M42))
               {
                  frameParams.Add(new CodeObjectCreateExpression(FrameClass, getTextureExp, cellExp, rectExp));
               }
               else
               {
                  CodePrimitiveExpression m11Exp = new CodePrimitiveExpression(drFrame.m11);
                  CodePrimitiveExpression m12Exp = new CodePrimitiveExpression(drFrame.m12);
                  CodePrimitiveExpression m21Exp = new CodePrimitiveExpression(drFrame.m21);
                  CodePrimitiveExpression m22Exp = new CodePrimitiveExpression(drFrame.m22);
                  CodePrimitiveExpression m41Exp = new CodePrimitiveExpression(drFrame.dx);
                  CodePrimitiveExpression m42Exp = new CodePrimitiveExpression(drFrame.dy);
                  frameParams.Add(new CodeObjectCreateExpression(FrameClass, getTextureExp, cellExp, m11Exp, m12Exp, m21Exp, m22Exp, m41Exp, m42Exp, rectExp));
               }
            }
            CodeArrayCreateExpression createArray = new CodeArrayCreateExpression(FrameClass,(CodeExpression[])frameParams.ToArray(typeof(CodeExpression)));
            curCondition.TrueStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "m_arFrames"), createArray));
         }
         curCondition.FalseStatements.Clear();
         framesetConstructor.Statements.Add(topCondition);
         framesetConstructor.Statements.Add(new CodeAssignStatement(
            new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "m_Display"),
            new CodeVariableReferenceExpression("disp")));

         CodeMemberProperty indexer = new CodeMemberProperty();
         indexer.Name = "Item";
         indexer.Type= new CodeTypeReference(FrameClass);
         indexer.Attributes = MemberAttributes.Public | MemberAttributes.Final;
         indexer.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "index"));
         indexer.HasSet=false;
         indexer.HasGet=true;
         indexer.GetStatements.Add(new CodeMethodReturnStatement(new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "m_arFrames"), new CodeVariableReferenceExpression("index"))));
         framesetClassDecl.Members.Add(indexer);

         CodeMemberProperty countProp = new CodeMemberProperty();
         countProp.Name = "Count";
         countProp.Type = new CodeTypeReference(typeof(int));
         countProp.Attributes = MemberAttributes.Public | MemberAttributes.Final;
         countProp.HasSet = false;
         countProp.HasGet = true;
         countProp.GetStatements.Add(new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "m_arFrames"), "Length")));
         framesetClassDecl.Members.Add(countProp);

         Generator.GenerateCodeFromType(framesetClassDecl, txt, GeneratorOptions);
      }

      private string NameToVariable(string name)
      {
         return name.Replace(" ","_");
      }

      public void GenerateCounters(System.IO.TextWriter txt)
      {
         CodeTypeDeclaration counterClassDecl = new CodeTypeDeclaration(CounterClass);
         CodeConstructor construct = new CodeConstructor();
         construct.Attributes = MemberAttributes.Public;
         construct.Parameters.AddRange(new CodeParameterDeclarationExpression[] {new CodeParameterDeclarationExpression(typeof(int), "nValue"), new CodeParameterDeclarationExpression(typeof(int), "nMax")});
         construct.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CounterValFld), new CodeVariableReferenceExpression("nValue")));
         construct.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CounterMaxFld), new CodeVariableReferenceExpression("nMax")));
         counterClassDecl.Members.Add(construct);

         CodeMemberField fld = new CodeMemberField(typeof(int), CounterValFld);
         fld.Attributes = MemberAttributes.Private | MemberAttributes.Final;
         counterClassDecl.Members.Add(fld);
         
         fld = new CodeMemberField(typeof(int), CounterMaxFld);
         fld.Attributes = MemberAttributes.Private | MemberAttributes.Final;
         counterClassDecl.Members.Add(fld);

         CodeMemberProperty propDecl = new CodeMemberProperty();
         propDecl.Attributes = MemberAttributes.Public | MemberAttributes.Final;
         propDecl.HasGet= true;
         propDecl.HasSet = true;
         propDecl.Name = CounterValProp;
         propDecl.Type = new CodeTypeReference(typeof(int));
         CodeFieldReferenceExpression fldRef = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CounterValFld);
         propDecl.GetStatements.Add(new CodeMethodReturnStatement(fldRef));
         CodePropertySetValueReferenceExpression propVal = new CodePropertySetValueReferenceExpression();
         CodeFieldReferenceExpression maxValue = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CounterMaxFld);
         CodePrimitiveExpression minValue = new CodePrimitiveExpression((int)0);
         CodeAssignStatement assignToProp = new CodeAssignStatement(fldRef, propVal);
         CodeConditionStatement testMin = new CodeConditionStatement(
            new CodeBinaryOperatorExpression(fldRef, CodeBinaryOperatorType.GreaterThanOrEqual,
            minValue), new CodeStatement[] {assignToProp}, new CodeStatement[]
               {new CodeAssignStatement(fldRef, minValue)});
         CodeConditionStatement testMax = new CodeConditionStatement(
            new CodeBinaryOperatorExpression(fldRef, CodeBinaryOperatorType.LessThanOrEqual,
            maxValue), new CodeStatement[] {testMin}, new CodeStatement[]
               {new CodeAssignStatement(fldRef, maxValue)});
         propDecl.SetStatements.Add(testMax);
         counterClassDecl.Members.Add(propDecl);

         propDecl = new CodeMemberProperty();
         propDecl.Attributes = MemberAttributes.Public | MemberAttributes.Final;
         propDecl.HasGet= true;
         propDecl.HasSet = false;
         propDecl.Name = CounterMaxProp;
         propDecl.Type = new CodeTypeReference(typeof(int));
         propDecl.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CounterMaxFld)));
         counterClassDecl.Members.Add(propDecl);


         foreach(ProjectDataset.CounterRow drCounter in ProjectData.Counter)
         {
            CodeMemberField fldCounter = new CodeMemberField("Counter", "m_" + NameToVariable(drCounter.Name));
            fldCounter.InitExpression = new CodeObjectCreateExpression("Counter", new CodePrimitiveExpression(drCounter.Value), new CodePrimitiveExpression(drCounter.Max));
            fldCounter.Attributes = MemberAttributes.Final | MemberAttributes.Private | MemberAttributes.Static;
            counterClassDecl.Members.Add(fldCounter);

            propDecl = new CodeMemberProperty();
            propDecl.Attributes = MemberAttributes.Final | MemberAttributes.Public | MemberAttributes.Static;
            propDecl.HasGet = true;
            propDecl.HasSet = false;
            propDecl.Name = NameToVariable(drCounter.Name);
            propDecl.Type = new CodeTypeReference("Counter");
            propDecl.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(fldCounter.Name)));
            counterClassDecl.Members.Add(propDecl);
         }

         Generator.GenerateCodeFromType(counterClassDecl, txt, GeneratorOptions);
      }

      public void GenerateTilesets(System.IO.TextWriter txt, System.IO.TextWriter err)
      {
         CodeTypeDeclaration classTileset = new CodeTypeDeclaration(TilesetClass);
         CodeTypeConstructor staticConstructor = new CodeTypeConstructor();
         classTileset.Members.Add(staticConstructor);
         staticConstructor.Attributes |= MemberAttributes.Static;

         CodeMemberField fldTiles = new CodeMemberField(new CodeTypeReference(TileBaseClass, 1), TilesField);
         fldTiles.Attributes = MemberAttributes.Final | MemberAttributes.Private;
         classTileset.Members.Add(fldTiles);

         CodeMemberField fldTileWidth = new CodeMemberField(typeof(short), TileWidthField);
         fldTileWidth.Attributes = MemberAttributes.Final | MemberAttributes.Private;
         classTileset.Members.Add(fldTileWidth);

         CodeMemberField fldTileHeight = new CodeMemberField(typeof(short), TileHeightField);
         fldTileHeight.Attributes = MemberAttributes.Final | MemberAttributes.Private;
         classTileset.Members.Add(fldTileHeight);

         CodeMemberField fldTilesetFrameset = new CodeMemberField(typeof(string), TilesetFramesetField);
         fldTilesetFrameset.Attributes = MemberAttributes.Final | MemberAttributes.Private;
         classTileset.Members.Add(fldTilesetFrameset);

         CodeConstructor constructor = new CodeConstructor();
         classTileset.Members.Add(constructor);
         constructor.Attributes = MemberAttributes.Private;
         constructor.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(TileBaseClass, 1), "tiles"));
         constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(short), "nTileWidth"));
         constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(short), "nTileHeight"));
         constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "strFrameset"));
         constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldTiles.Name), new CodeVariableReferenceExpression("tiles")));
         constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldTileWidth.Name), new CodeVariableReferenceExpression("nTileWidth")));
         constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldTileHeight.Name), new CodeVariableReferenceExpression("nTileHeight")));
         constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldTilesetFrameset.Name), new CodeVariableReferenceExpression("strFrameset")));
         staticConstructor.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.Collections.ArrayList), TileListVar));

         CodeVariableReferenceExpression varTileList = new CodeVariableReferenceExpression(TileListVar);

         foreach(ProjectDataset.TilesetRow drTileset in ProjectData.Tileset)
         {
            ProjectDataset.TileRow[] arTiles = ProjectData.GetSortedTileRows(drTileset);
            int nCurIdx = 0;
            CodeArrayCreateExpression TileArrayExp = new CodeArrayCreateExpression("TileBase", new CodePrimitiveExpression(arTiles.Length));
            CodeMemberField fldTileset = new CodeMemberField(TilesetClass, "m_" + NameToVariable(drTileset.Name));
            fldTileset.Attributes = MemberAttributes.Private | MemberAttributes.Final | MemberAttributes.Static;
            classTileset.Members.Add(fldTileset);

            CodeMemberProperty prpTileset = new CodeMemberProperty();
            prpTileset.Name = NameToVariable(drTileset.Name);
            prpTileset.HasSet = false;
            prpTileset.HasGet = true;
            prpTileset.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
            prpTileset.Type = new CodeTypeReference(TilesetClass);
            CodeFieldReferenceExpression fldTilesetRef = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(TilesetClass), fldTileset.Name);
            prpTileset.GetStatements.Add(new CodeMethodReturnStatement(fldTilesetRef));
            classTileset.Members.Add(prpTileset);

            staticConstructor.Statements.Add(new CodeCommentStatement(drTileset.Name));
            staticConstructor.Statements.Add(new CodeAssignStatement(varTileList, new CodeObjectCreateExpression(typeof(System.Collections.ArrayList))));
            int nMax = arTiles[arTiles.Length-1].TileValue;
            if (drTileset.FramesetRow.GetFrameRows().Length > nMax)
               nMax = drTileset.FramesetRow.GetFrameRows().Length - 1;
            for(int i=0; i<=nMax; i++)
            {
               CodeExpression TileExpr;
               if ((nCurIdx < arTiles.Length) && (arTiles[nCurIdx].TileValue == i))
               {
                  ProjectDataset.TileRow drTile = arTiles[nCurIdx];
                  nCurIdx++;
                  ProjectDataset.TileFrameRow[] arFrames = ProjectData.GetSortedTileFrames(drTile);
                  int nAccumulatedDuration = 0;
                  System.Collections.ArrayList frames = new System.Collections.ArrayList();
                  System.Collections.ArrayList subFrames = null;
                  foreach(ProjectDataset.TileFrameRow drTileFrame in arFrames)
                  {
                     if (subFrames == null)
                        subFrames = new System.Collections.ArrayList();
                     subFrames.Add(new CodePrimitiveExpression(drTileFrame.FrameValue));
                     if (drTileFrame.Duration > 0)
                     {
                        nAccumulatedDuration += drTileFrame.Duration;
                        if (subFrames.Count == 1)
                        {
                           frames.Add(new CodeObjectCreateExpression("TileFrame",
                              new CodePrimitiveExpression(nAccumulatedDuration),
                              (CodePrimitiveExpression)subFrames[0]));
                        }
                        else
                        {
                           frames.Add(new CodeObjectCreateExpression("TileFrame",
                              new CodePrimitiveExpression(nAccumulatedDuration),
                              new CodeArrayCreateExpression(typeof(int), (CodeExpression[])subFrames.ToArray(typeof(CodeExpression)))));
                        }
                        subFrames.Clear();
                     }
                  }
                  if (frames.Count == 0)
                     TileExpr = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(EmptyTileClass), "Value");
                  else if (frames.Count == 1)
                     TileExpr = new CodeObjectCreateExpression(SimpleTileClass, (CodeArrayCreateExpression)(frames[0] as CodeObjectCreateExpression).Parameters[1]);
                  else
                  {
                     System.Collections.ArrayList animArgs = new System.Collections.ArrayList();
                     if (drTile.CounterRow == null)
                     {
                        err.WriteLine("Tileset " + drTileset.Name + " tile " + drTile.TileValue + " requires a counter");
                        animArgs.Add(new CodePrimitiveExpression(null));
                     }
                     else
                     {
                        animArgs.Add(
                           new CodePropertyReferenceExpression(
                           new CodeTypeReferenceExpression(CounterClass),
                           NameToVariable(drTile.CounterRow.Name)));
                     }
                     animArgs.AddRange(frames.ToArray(typeof(CodeExpression)));

                     TileExpr = new CodeObjectCreateExpression(AnimTileClass, (CodeExpression[])animArgs.ToArray(typeof(CodeExpression)));
                  }
               }
               else
               {
                  TileExpr = new CodeObjectCreateExpression(SimpleTileClass, new CodePrimitiveExpression(i));
               }
               staticConstructor.Statements.Add(new CodeMethodInvokeExpression(varTileList, "Add", TileExpr));
            }
            staticConstructor.Statements.Add(
               new CodeAssignStatement(fldTilesetRef,
               new CodeObjectCreateExpression(TilesetClass,
               new CodeCastExpression(
               new CodeTypeReference(TileBaseClass, 1),
               new CodeMethodInvokeExpression(
               new CodeVariableReferenceExpression(TileListVar), "ToArray",
               new CodeTypeOfExpression(TileBaseClass))),
               new CodePrimitiveExpression(drTileset.TileWidth),
               new CodePrimitiveExpression(drTileset.TileHeight),
               new CodePrimitiveExpression(drTileset.Frameset))));
         }

         CodeMemberProperty prpTile = new CodeMemberProperty();
         prpTile.Name = "Item";
         CodeParameterDeclarationExpression indexParam = new CodeParameterDeclarationExpression(typeof(int), "index");
         prpTile.Parameters.Add(indexParam);
         prpTile.HasGet = true;
         prpTile.HasSet = false;
         prpTile.Attributes = MemberAttributes.Final | MemberAttributes.Public;
         prpTile.Type = new CodeTypeReference(TileBaseClass);
         prpTile.GetStatements.Add(
            new CodeMethodReturnStatement(
            new CodeArrayIndexerExpression(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), TilesField),
            new CodeVariableReferenceExpression(indexParam.Name))));
         classTileset.Members.Add(prpTile);

         CodeMemberProperty prpTileWidth = new CodeMemberProperty();
         prpTileWidth.Name = "TileWidth";
         prpTileWidth.HasGet = true;
         prpTileWidth.HasSet = false;
         prpTileWidth.Attributes = MemberAttributes.Final | MemberAttributes.Public;
         prpTileWidth.Type = new CodeTypeReference(typeof(short));
         prpTileWidth.GetStatements.Add(
            new CodeMethodReturnStatement(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), TileWidthField)));
         classTileset.Members.Add(prpTileWidth);

         CodeMemberProperty prpTileHeight = new CodeMemberProperty();
         prpTileHeight.Name = "TileHeight";
         prpTileHeight.HasGet = true;
         prpTileHeight.HasSet = false;
         prpTileHeight.Attributes = MemberAttributes.Final | MemberAttributes.Public;
         prpTileHeight.Type = new CodeTypeReference(typeof(short));
         prpTileHeight.GetStatements.Add(
            new CodeMethodReturnStatement(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), TileHeightField)));
         classTileset.Members.Add(prpTileHeight);

         CodeMemberMethod mthCreateFrameset = new CodeMemberMethod();
         mthCreateFrameset.Name = "CreateFrameset";
         mthCreateFrameset.Parameters.Add(new CodeParameterDeclarationExpression("Display", "disp"));
         mthCreateFrameset.Attributes = MemberAttributes.Final | MemberAttributes.Public;
         mthCreateFrameset.ReturnType = new CodeTypeReference(FramesetClass);
         mthCreateFrameset.Statements.Add(
            new CodeMethodReturnStatement(
            new CodeObjectCreateExpression(
            new CodeTypeReference(FramesetClass),
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), TilesetFramesetField),
            new CodeVariableReferenceExpression("disp"))));
         classTileset.Members.Add(mthCreateFrameset);

         CodeMemberProperty prpTileCount = new CodeMemberProperty();
         prpTileCount.Type = new CodeTypeReference(typeof(int));
         prpTileCount.HasSet = false;
         prpTileCount.HasGet = true;
         prpTileCount.Attributes = MemberAttributes.Final | MemberAttributes.Public;
         prpTileCount.Name = "TileCount";
         prpTileCount.GetStatements.Add(new CodeMethodReturnStatement(
            new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), TilesField), "Length")));
         classTileset.Members.Add(prpTileCount);
         
         Generator.GenerateCodeFromType(classTileset, txt, GeneratorOptions);
      }


      public void GenerateMapResx(ProjectDataset.MapRow drMap, System.IO.TextWriter txt)
      {
         System.Resources.ResXResourceWriter writer = new System.Resources.ResXResourceWriter(txt);

         foreach (ProjectDataset.LayerRow drLayer in ProjectData.GetSortedLayers(drMap))
         {
            switch(drLayer.BytesPerTile)
            {
               case 1:
               {
                  byte[,] tiles = new byte[drLayer.Width, drLayer.Height];
                  for (int y=0; y<drLayer.Height; y++)
                  {
                     for (int x=0; x<drLayer.Width; x++)
                     {
                        tiles[x,y] = drLayer.Tiles[y*drLayer.Width + x];
                     }
                  }
                  writer.AddResource(drLayer.Name, tiles);
               }
                  break;
               case 2:
               {
                  short[,] tiles = new short[drLayer.Width, drLayer.Height];
                  for (int y=0; y<drLayer.Height; y++)
                  {
                     for (int x=0; x<drLayer.Width; x++)
                     {
                        tiles[x,y] = BitConverter.ToInt16(drLayer.Tiles, (y*drLayer.Width + x)*2);
                     }
                  }
                  writer.AddResource(drLayer.Name, tiles);
               }
                  break;
               default:
               {
                  int[,] tiles = new int[drLayer.Width, drLayer.Height];
                  for (int y=0; y<drLayer.Height; y++)
                  {
                     for (int x=0; x<drLayer.Width; x++)
                     {
                        tiles[x,y] = BitConverter.ToInt16(drLayer.Tiles, (y*drLayer.Width + x)*4);
                     }
                  }
                  writer.AddResource(drLayer.Name, tiles);
               }
                  break;
            }
         }
         writer.Close();
      }

      public void GenerateMap(ProjectDataset.MapRow drMap, System.IO.TextWriter txt)
      {
         CodeTypeDeclaration clsMap = new CodeTypeDeclaration("Map" + NameToVariable(drMap.Name));
         CodeConstructor constructor = new CodeConstructor();
         constructor.Attributes = MemberAttributes.Final | MemberAttributes.Public;
         constructor.Parameters.Add(new CodeParameterDeclarationExpression("Display", "Disp"));
         CodeVariableReferenceExpression refDisp = new CodeVariableReferenceExpression("Disp");
         clsMap.Members.Add(constructor);
         clsMap.Members.Add(new CodeMemberField("Display", MapDisplayField));
         CodeFieldReferenceExpression fldDisplayRef = new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), MapDisplayField);

         CodeMemberMethod mthDraw = new CodeMemberMethod();
         mthDraw.Attributes = MemberAttributes.Public | MemberAttributes.Final;
         mthDraw.Name = "Draw";
         clsMap.Members.Add(mthDraw);

         mthDraw.Statements.Add(new CodeVariableDeclarationStatement(typeof(Microsoft.DirectX.Direct3D.Sprite), "spr",
            new CodeObjectCreateExpression(typeof(Microsoft.DirectX.Direct3D.Sprite),
            new CodeFieldReferenceExpression(fldDisplayRef, "Device"))));
         CodeVariableReferenceExpression refSpr = new CodeVariableReferenceExpression("spr");
         mthDraw.Statements.Add(new CodeMethodInvokeExpression(refSpr, "Begin",
            new CodeFieldReferenceExpression(
            new CodeTypeReferenceExpression(typeof(Microsoft.DirectX.Direct3D.SpriteFlags)),"AlphaBlend")));

         foreach (ProjectDataset.LayerRow drLayer in ProjectData.GetSortedLayers(drMap))
         {
            CodeMemberField fldLayer;
            int nTop, nBottom, nLeft, nRight;
            ProjectData.GetTilesetOverlaps(drLayer.TilesetRow, out nRight, out nBottom, out nLeft, out nTop);
            CodeTypeReference lyrTyp;
            switch(drLayer.BytesPerTile)
            {
               case 1:
                  lyrTyp = new CodeTypeReference("ByteLayer");
                  break;
               case 2:
                  lyrTyp = new CodeTypeReference("ShortLayer");
                  break;
               default:
                  lyrTyp = new CodeTypeReference("IntLayer");
                  break;
            }
            fldLayer = new CodeMemberField(lyrTyp, "m_" + NameToVariable(drLayer.Name));
            clsMap.Members.Add(fldLayer);

            constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(
               new CodeThisReferenceExpression(), fldLayer.Name),
               new CodeObjectCreateExpression(lyrTyp,
               new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("Tileset"),
               NameToVariable(drLayer.Tileset)), refDisp,
               new CodePrimitiveExpression(nLeft),
               new CodePrimitiveExpression(nTop),
               new CodePrimitiveExpression(nRight),
               new CodePrimitiveExpression(nBottom),
               new CodePrimitiveExpression(drLayer.Width),
               new CodePrimitiveExpression(drLayer.Height),
               new CodeObjectCreateExpression(typeof(System.Drawing.Point), 
               new CodePrimitiveExpression(drLayer.OffsetX),
               new CodePrimitiveExpression(drLayer.OffsetY)),
               new CodePrimitiveExpression(null), // TODO: Create sprites
               new CodeObjectCreateExpression(typeof(System.Drawing.SizeF),
               new CodePrimitiveExpression(drLayer.ScrollRateX),
               new CodePrimitiveExpression(drLayer.ScrollRateY)),
               new CodePrimitiveExpression(drLayer.Name))));

            CodeMemberProperty prpLayer = new CodeMemberProperty();
            prpLayer.Name = NameToVariable(drLayer.Name);
            prpLayer.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            prpLayer.HasGet = true;
            prpLayer.HasSet = false;
            prpLayer.Type = lyrTyp;
            prpLayer.GetStatements.Add(new CodeMethodReturnStatement(
               new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldLayer.Name)));
            clsMap.Members.Add(prpLayer);

            mthDraw.Statements.Add(new CodeMethodInvokeExpression(
               new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldLayer.Name),
               "Draw", new CodePropertyReferenceExpression(fldDisplayRef, "DisplayRectangle"),
               refSpr));
         }
         mthDraw.Statements.Add(new CodeMethodInvokeExpression(refSpr, "End"));
         mthDraw.Statements.Add(new CodeMethodInvokeExpression(refSpr, "Dispose"));
         constructor.Statements.Add(new CodeAssignStatement(fldDisplayRef, refDisp));
         Generator.GenerateCodeFromType(clsMap, txt, GeneratorOptions);
      }
   
      public void GenerateProjectSourceCode(string FolderName)
      {
         foreach (ProjectDataset.SourceCodeRow drCode in ProjectData.SourceCode)
         {
            System.IO.TextWriter txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, drCode.Name));
            txt.Write(drCode.Text);
            txt.Close();
         }
      }
	}
}
