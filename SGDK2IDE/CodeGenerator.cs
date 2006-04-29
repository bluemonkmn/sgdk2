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

      #region Constants
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
      private const string SpriteStateSolidWidth = "SolidWidth";
      private const string SpriteStateSolidHeight = "SolidHeight";
      private const string CategoryCount = "Count";
      private const string SolidityClassName = "Solidity";
      private const string SpriteStateClassName = "SpriteState";
      private const string SpriteStateField = "m_SpriteStates";
      private const string SpriteStateEnumName = "State";
      private const string GetFramesetMethodName = "GetFrameset";
      #endregion

      public System.CodeDom.Compiler.ICodeGenerator Generator = new Microsoft.CSharp.CSharpCodeProvider().CreateGenerator();
      public CodeGeneratorOptions GeneratorOptions = new CodeGeneratorOptions();

      #region Project Level Code Generation
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

         string SpritesFolder = System.IO.Path.Combine(FolderName, "Sprites");
         if (!System.IO.Directory.Exists(SpritesFolder))
            System.IO.Directory.CreateDirectory(SpritesFolder);
         foreach (ProjectDataset.SpriteDefinitionRow drSpriteDef in ProjectData.SpriteDefinition)
         {
            txt = new System.IO.StreamWriter(System.IO.Path.Combine(SpritesFolder, NameToVariable(drSpriteDef.Name) + ".cs"));
            GenerateSpriteDef(drSpriteDef, txt);
            txt.Close();
         }

         txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, SolidityClassName + ".cs"));
         GenerateTileCategories(txt);
         GenerateSolidity(txt);
         txt.Close();

         GenerateProjectSourceCode(FolderName);

         string errs = err.ToString();err.Close();

         if (errs.Length > 0)
            System.Windows.Forms.MessageBox.Show(errs, "Error");

      }
      
      public string[] GenerateCodeStrings()
      {
         string errs;
         return GenerateCodeStrings(out errs);
      }

      public string[] GenerateCodeStrings(out string errs)
      {
         System.Collections.ArrayList result = new System.Collections.ArrayList();

         System.IO.TextWriter err = new System.IO.StringWriter();
         System.IO.StringWriter txt = new System.IO.StringWriter();

         GenerateMainCode(txt);
         txt.Close();
         result.Add(txt.ToString());

         /*txt = new System.IO.StringWriter(System.IO.Path.Combine(FolderName, ProjectClass + ".resx"));
         GenerateResx(txt);
         txt.Close();*/

         txt = new System.IO.StringWriter();
         GenerateCounters(txt);
         txt.Close();
         result.Add(txt.ToString());

         txt = new System.IO.StringWriter();
         GenerateFramesets(txt);
         txt.Close();
         result.Add(txt.ToString());

         txt = new System.IO.StringWriter();
         GenerateTilesets(txt, err);
         txt.Close();
         result.Add(txt.ToString());

         foreach (ProjectDataset.MapRow drMap in ProjectData.Map)
         {
            /*txt = new System.IO.StringWriter(System.IO.Path.Combine(FolderName, "Map" + drMap.Name + ".resx"));
            GenerateMapResx(drMap, txt);
            txt.Close();*/

            txt = new System.IO.StringWriter();
            GenerateMap(drMap, txt);
            txt.Close();
            result.Add(txt.ToString());
         }

         foreach (ProjectDataset.SpriteDefinitionRow drSpriteDef in ProjectData.SpriteDefinition)
         {
            txt = new System.IO.StringWriter();
            GenerateSpriteDef(drSpriteDef, txt);
            txt.Close();
            result.Add(txt.ToString());
         }

         txt = new System.IO.StringWriter();
         GenerateTileCategories(txt);
         GenerateSolidity(txt);
         txt.Close();
         result.Add(txt.ToString());

         foreach(ProjectDataset.SourceCodeRow drCode in ProjectData.SourceCode)
            result.Add(drCode.Text);

         errs = err.ToString();
         err.Close();

         return (string[])result.ToArray(typeof(string));
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
      #endregion

      #region File Level Code Generation
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
         framesetConstructor.Attributes = MemberAttributes.Final | MemberAttributes.Private;
         framesetConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "Name"));
         framesetConstructor.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference("Display"), "disp"));
         framesetClassDecl.Members.Add(framesetConstructor);

         CodeMemberField fldFramesetCache = new CodeMemberField(typeof(System.Collections.Hashtable), "m_CachedFramesets");
         framesetClassDecl.Members.Add(fldFramesetCache);
         fldFramesetCache.Attributes = MemberAttributes.Private | MemberAttributes.Static | MemberAttributes.Final;
         fldFramesetCache.InitExpression = new CodeObjectCreateExpression(typeof(System.Collections.Hashtable));

         CodeMemberMethod mthGetFrameset = new CodeMemberMethod();
         framesetClassDecl.Members.Add(mthGetFrameset);
         mthGetFrameset.Name = GetFramesetMethodName;
         mthGetFrameset.Attributes = MemberAttributes.Final | MemberAttributes.Public | MemberAttributes.Static;
         mthGetFrameset.ReturnType = new CodeTypeReference(FramesetClass);
         mthGetFrameset.Parameters.Add(new CodeParameterDeclarationExpression(
            typeof(string), "Name"));
         mthGetFrameset.Parameters.Add(new CodeParameterDeclarationExpression(
            "Display", "disp"));
         
         CodeVariableDeclarationStatement varResult = new CodeVariableDeclarationStatement(FramesetClass, "result");
         
         varResult.InitExpression = new CodeCastExpression(FramesetClass,
            new CodeIndexerExpression(
            new CodeFieldReferenceExpression(
            new CodeTypeReferenceExpression(FramesetClass), "m_CachedFramesets"),
            new CodeArgumentReferenceExpression("Name")));

         mthGetFrameset.Statements.Add(varResult);
         mthGetFrameset.Statements.Add(new CodeConditionStatement(
            new CodeBinaryOperatorExpression(
            new CodeVariableReferenceExpression(varResult.Name),
            CodeBinaryOperatorType.IdentityEquality,
            new CodePrimitiveExpression(null)),
            new CodeStatement[]
            {
               new CodeAssignStatement(
               new CodeVariableReferenceExpression(varResult.Name),
               new CodeObjectCreateExpression(FramesetClass,
               new CodeArgumentReferenceExpression("Name"),
               new CodeArgumentReferenceExpression("disp"))),
               new CodeAssignStatement(
               new CodeIndexerExpression(
               new CodeFieldReferenceExpression(
               new CodeTypeReferenceExpression(FramesetClass), "m_CachedFramesets"),
               new CodeArgumentReferenceExpression("Name")),
               new CodeVariableReferenceExpression(varResult.Name))
            }));
         mthGetFrameset.Statements.Add(new CodeMethodReturnStatement(
            new CodeVariableReferenceExpression(varResult.Name)));

         CodeConditionStatement topCondition = new CodeConditionStatement();
         CodeConditionStatement curCondition = topCondition;

         foreach(ProjectDataset.FramesetRow drFrameset in ProjectData.Frameset)
         {
            if (curCondition.TrueStatements.Count > 0) curCondition.FalseStatements.Add(curCondition = new CodeConditionStatement());
            CodeArgumentReferenceExpression nameParam = new CodeArgumentReferenceExpression("Name");
            CodePrimitiveExpression nameCompare = new CodePrimitiveExpression(drFrameset.Name);
            CodeBinaryOperatorExpression DoCompare = new CodeBinaryOperatorExpression(nameParam, CodeBinaryOperatorType.ValueEquality, nameCompare);
            curCondition.Condition = DoCompare;
            CodeArgumentReferenceExpression dispParam = new CodeArgumentReferenceExpression("disp");
            System.Collections.ArrayList frameParams = new System.Collections.ArrayList();
            foreach(ProjectDataset.FrameRow drFrame in ProjectData.GetSortedFrameRows(drFrameset))
            {
               CodeMethodInvokeExpression getTextureExp = new CodeMethodInvokeExpression(dispParam, "GetTextureRef", new CodePrimitiveExpression(drFrame.GraphicSheet));
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
            new CodeArgumentReferenceExpression("disp")));

         CodeMemberProperty indexer = new CodeMemberProperty();
         indexer.Name = "Item";
         indexer.Type= new CodeTypeReference(FrameClass);
         indexer.Attributes = MemberAttributes.Public | MemberAttributes.Final;
         indexer.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "index"));
         indexer.HasSet=false;
         indexer.HasGet=true;
         indexer.GetStatements.Add(new CodeMethodReturnStatement(new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "m_arFrames"), new CodeArgumentReferenceExpression("index"))));
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

      public void GenerateCounters(System.IO.TextWriter txt)
      {
         CodeTypeDeclaration counterClassDecl = new CodeTypeDeclaration(CounterClass);
         CodeConstructor construct = new CodeConstructor();
         construct.Attributes = MemberAttributes.Public;
         construct.Parameters.AddRange(new CodeParameterDeclarationExpression[] {new CodeParameterDeclarationExpression(typeof(int), "nValue"), new CodeParameterDeclarationExpression(typeof(int), "nMax")});
         construct.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CounterValFld), new CodeArgumentReferenceExpression("nValue")));
         construct.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CounterMaxFld), new CodeArgumentReferenceExpression("nMax")));
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
         constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldTiles.Name), new CodeArgumentReferenceExpression("tiles")));
         constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldTileWidth.Name), new CodeArgumentReferenceExpression("nTileWidth")));
         constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldTileHeight.Name), new CodeArgumentReferenceExpression("nTileHeight")));
         constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldTilesetFrameset.Name), new CodeArgumentReferenceExpression("strFrameset")));
         staticConstructor.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.Collections.ArrayList), TileListVar));

         CodeVariableReferenceExpression varTileList = new CodeVariableReferenceExpression(TileListVar);

         foreach(ProjectDataset.TilesetRow drTileset in ProjectData.Tileset)
         {
            ProjectDataset.TileRow[] arTiles = ProjectData.GetSortedTileRows(drTileset);
            int nCurIdx = 0;
            CodeArrayCreateExpression TileArrayExp = new CodeArrayCreateExpression(TileBaseClass, new CodePrimitiveExpression(arTiles.Length));
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
               int nFrameSequenceCount = 0;
               CodeExpression TileExpr;
               if ((nCurIdx < arTiles.Length) && (arTiles[nCurIdx].TileValue == i))
               {
                  ProjectDataset.TileRow drTile = arTiles[nCurIdx];
                  nCurIdx++;
                  ProjectDataset.TileFrameRow[] arFrames = ProjectData.GetSortedTileFrames(drTile);
                  nFrameSequenceCount = arFrames.Length;
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
                  TileExpr = new CodeObjectCreateExpression(SimpleTileClass, new CodePrimitiveExpression(i));

               ProjectDataset.CategoryTileRow[] drCats = ProjectData.GetTileCategories(drTileset.Name, i);
               if (drCats.Length > 0)
               {
                  bool hasFrames = false;
                  CodeObjectCreateExpression createCategory = new CodeObjectCreateExpression(
                     "TileCategoryFrameMembership", new CodePrimitiveExpression(nFrameSequenceCount));
                  CodeObjectCreateExpression createCategorySimple = new CodeObjectCreateExpression(
                     "TileCategorySimpleMembership");
                  foreach(ProjectDataset.CategoryTileRow drCat in drCats)
                  {
                     CodeArrayCreateExpression createFrameList = new CodeArrayCreateExpression(
                        typeof(int), new CodeExpression[] {});
                     CodeFieldReferenceExpression refCategory = new CodeFieldReferenceExpression(
                           new CodeTypeReferenceExpression("TileCategoryName"), drCat.CategoryRowParent.Name);
                     CodeObjectCreateExpression createTileFrameMembership = new CodeObjectCreateExpression(
                        "TileFrameMembership", new CodeExpression[]
                        { refCategory, createFrameList });

                     ProjectDataset.CategoryFrameRow[] drFrms = drCat.GetCategoryFrameRows();
                     if (drFrms.Length > 0)
                     {
                        foreach(ProjectDataset.CategoryFrameRow drFrm in drFrms)
                           createFrameList.Initializers.Add(new CodePrimitiveExpression(drFrm.Frame));
                        hasFrames = true;
                     }
                     createCategory.Parameters.Add(createTileFrameMembership);
                     createCategorySimple.Parameters.Add(refCategory);
                  }

                  if (hasFrames)
                     ((CodeObjectCreateExpression)TileExpr).Parameters.Insert(1, createCategory);
                  else
                  {
                     if (TileExpr is CodeObjectCreateExpression)
                        ((CodeObjectCreateExpression)TileExpr).Parameters.Insert(1,createCategorySimple);
                     else
                        err.WriteLine("Cannot categorize empty tile " + i.ToString() + " in tileset " + drTileset.Name);
                  }
               }
               staticConstructor.Statements.Add(new CodeMethodInvokeExpression(varTileList, "Add", TileExpr));
            }
            staticConstructor.Statements.Add(
               new CodeAssignStatement(fldTilesetRef,
               new CodeObjectCreateExpression(TilesetClass,
               new CodeCastExpression(
               new CodeTypeReference(TileBaseClass, 1),
               new CodeMethodInvokeExpression(
               varTileList, "ToArray",
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
            new CodeArgumentReferenceExpression(indexParam.Name))));
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
         mthCreateFrameset.Name = GetFramesetMethodName;
         mthCreateFrameset.Parameters.Add(new CodeParameterDeclarationExpression("Display", "disp"));
         mthCreateFrameset.Attributes = MemberAttributes.Final | MemberAttributes.Public;
         mthCreateFrameset.ReturnType = new CodeTypeReference(FramesetClass);
         mthCreateFrameset.Statements.Add(
            new CodeMethodReturnStatement(
            new CodeMethodInvokeExpression(
            new CodeTypeReferenceExpression(FramesetClass),
            GetFramesetMethodName,
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), TilesetFramesetField),
            new CodeArgumentReferenceExpression("disp"))));
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
         CodeArgumentReferenceExpression refDisp = new CodeArgumentReferenceExpression("Disp");
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

            CodeTypeDeclaration clsLayer = new CodeTypeDeclaration(NameToVariable(drLayer.Name)+ "_t");
            clsLayer.BaseTypes.Add(lyrTyp);
            lyrTyp = new CodeTypeReference(clsLayer.Name);
            CodeConstructor lyrConstructor = new CodeConstructor();
            clsLayer.Members.Add(lyrConstructor);
            lyrConstructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            lyrConstructor.Parameters.AddRange(new CodeParameterDeclarationExpression[]
               {
                  new CodeParameterDeclarationExpression("Tileset", "Tileset"),
                  new CodeParameterDeclarationExpression("Display", "Disp"),
                  new CodeParameterDeclarationExpression(typeof(int), "nLeftBuffer"),
                  new CodeParameterDeclarationExpression(typeof(int), "nTopBuffer"),
                  new CodeParameterDeclarationExpression(typeof(int), "nRightBuffer"),
                  new CodeParameterDeclarationExpression(typeof(int), "nBottomBuffer"),
                  new CodeParameterDeclarationExpression(typeof(int), "nColumns"),
                  new CodeParameterDeclarationExpression(typeof(int), "nRows"),
                  new CodeParameterDeclarationExpression(typeof(System.Drawing.Point), "Position"),
                  new CodeParameterDeclarationExpression(typeof(System.Drawing.SizeF), "ScrollRate"),
                  new CodeParameterDeclarationExpression(typeof(string), "Name")
               });
            CodeObjectCreateExpression createLayerSprites = new CodeObjectCreateExpression("SpriteCollection");
            lyrConstructor.BaseConstructorArgs.AddRange(new CodeExpression[]
               {
                  new CodeArgumentReferenceExpression("Tileset"),
                  new CodeArgumentReferenceExpression("Disp"),
                  new CodeArgumentReferenceExpression("nLeftBuffer"),
                  new CodeArgumentReferenceExpression("nTopBuffer"),
                  new CodeArgumentReferenceExpression("nRightBuffer"),
                  new CodeArgumentReferenceExpression("nBottomBuffer"),
                  new CodeArgumentReferenceExpression("nColumns"),
                  new CodeArgumentReferenceExpression("nRows"),
                  new CodeArgumentReferenceExpression("Position"),
                  new CodeArgumentReferenceExpression("ScrollRate"),
                  new CodeArgumentReferenceExpression("Name")
               });
            clsMap.Members.Add(clsLayer);

            fldLayer = new CodeMemberField(clsLayer.Name, "m_" + NameToVariable(drLayer.Name));
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
               "InjectSprites"));
            mthDraw.Statements.Add(new CodeMethodInvokeExpression(
               new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldLayer.Name),
               "Draw", refSpr));
            mthDraw.Statements.Add(new CodeMethodInvokeExpression(
               new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldLayer.Name),
               "ClearInjections"));

            CodeMemberMethod mthInject = new CodeMemberMethod();
            mthInject.Name = "InjectSprites";
            mthInject.Attributes = MemberAttributes.Override | MemberAttributes.Public;
            mthInject.ReturnType = new CodeTypeReference(typeof(void));
            clsLayer.Members.Add(mthInject);

            ProjectDataset.SpriteRow[] sprites = ProjectData.GetSortedSpriteRows(drLayer);
            System.Collections.Hashtable htCategories = new System.Collections.Hashtable();
            if (sprites.Length > 0)
            {
               foreach(ProjectDataset.SpriteRow sprite in sprites)
               {
                  System.Collections.ArrayList SpriteCreateParams = new System.Collections.ArrayList();
                  ProjectDataset.SpriteStateRow drState = sprite.SpriteStateRowParent;
                  ProjectDataset.SpriteDefinitionRow drDef = drState.SpriteDefinitionRow;
                  SpriteCreateParams.Add(new CodePrimitiveExpression(sprite.X));
                  SpriteCreateParams.Add(new CodePrimitiveExpression(sprite.Y));
                  SpriteCreateParams.Add(new CodePrimitiveExpression(sprite.DX));
                  SpriteCreateParams.Add(new CodePrimitiveExpression(sprite.DY));
                  SpriteCreateParams.Add(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("Sprites." + NameToVariable(drDef.Name) + ".State"), NameToVariable(sprite.StateName)));
                  SpriteCreateParams.Add(new CodePrimitiveExpression(sprite.CurrentFrame));
                  SpriteCreateParams.Add(new CodePrimitiveExpression(sprite.Active));
                  SpriteCreateParams.Add(new CodeArgumentReferenceExpression("Disp"));
                  ProjectDataset.SpriteParameterRow[] sprParams = ProjectData.GetSortedSpriteParameters(drDef);
                  if (sprParams.Length > 0)
                  {
                     foreach (ProjectDataset.SpriteParameterRow drParam in sprParams)
                     {
                        ProjectDataset.ParameterValueRow sprParam = ProjectData.GetSpriteParameterValueRow(sprite, drParam.Name);
                        if (sprParam == null)
                           SpriteCreateParams.Add(new CodePrimitiveExpression(0));
                        else
                           SpriteCreateParams.Add(new CodePrimitiveExpression(sprParam.Value));
                     }
                  }
                  CodeObjectCreateExpression createSprite = new CodeObjectCreateExpression(
                     "Sprites." + NameToVariable(drDef.Name), (CodeExpression[])
                     SpriteCreateParams.ToArray(typeof(CodeExpression)));
                  clsLayer.Members.Add(new CodeMemberField("Sprites." + NameToVariable(drDef.Name), "m_" + NameToVariable(sprite.Name)));
                  lyrConstructor.Statements.Add(new CodeAssignStatement(
                     new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "m_" + NameToVariable(sprite.Name)),
                     createSprite));
                  CodeFieldReferenceExpression fldrefSpr = new CodeFieldReferenceExpression(
                     new CodeThisReferenceExpression(), "m_" + NameToVariable(sprite.Name));
                  createLayerSprites.Parameters.Add(fldrefSpr);

                  foreach(ProjectDataset.SpriteCategorySpriteRow drCatSpr in drDef.GetSpriteCategorySpriteRows())
                  {
                     ProjectDataset.SpriteCategoryRow drCat = drCatSpr.SpriteCategoryRow;
                     if (!htCategories.ContainsKey(drCat.Name))
                        htCategories[drCat.Name] = new System.Collections.ArrayList();
                     ((System.Collections.ArrayList)htCategories[drCat.Name]).Add(fldrefSpr);                        
                  }

                  int priority;
                  if (sprite.Priority < drLayer.Priority)
                     priority = -1;
                  else if (sprite.Priority > drLayer.Priority)
                     priority = 1;
                  else
                     priority = 0;

                  // Generate code that injects ths sprite into the layer when visible
                  System.Collections.ArrayList InjectParams = new System.Collections.ArrayList();
                  InjectParams.AddRange(new CodeExpression[]
                     {
                        new CodePropertyReferenceExpression(fldrefSpr, "PixelX"),
                        new CodePropertyReferenceExpression(fldrefSpr, "PixelY"),
                        new CodeMethodInvokeExpression(fldrefSpr, "GetCurrentFramesetFrames")
                     });
                  if (priority != 0)
                     InjectParams.Add(new CodePrimitiveExpression(priority));

                  mthInject.Statements.Add(new CodeConditionStatement(
                     new CodeMethodInvokeExpression(
                     new CodeThisReferenceExpression(), "IsSpriteVisible",
                     fldrefSpr),
                     new CodeExpressionStatement(
                     new CodeMethodInvokeExpression(
                     new CodeThisReferenceExpression(),
                     (priority == 0)?"InjectFrames":"AppendFrames",
                     (CodeExpression[])InjectParams.ToArray(typeof(CodeExpression))))));
               }

               if (htCategories.Count > 0)
               {
                  foreach(System.Collections.DictionaryEntry de in htCategories)
                  {
                     clsLayer.Members.Add(new CodeMemberField(
                        "SpriteCollection", "m_" + NameToVariable(de.Key.ToString())));
                     lyrConstructor.Statements.Add(new CodeAssignStatement(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), "m_" + NameToVariable(de.Key.ToString())),
                        new CodeObjectCreateExpression("SpriteCollection",
                        (CodeFieldReferenceExpression[])((System.Collections.ArrayList)de.Value).ToArray(
                        typeof(CodeFieldReferenceExpression)))));
                     CodeMemberProperty prpSpriteCat = new CodeMemberProperty();
                     prpSpriteCat.Type = new CodeTypeReference("SpriteCollection");
                     prpSpriteCat.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                     prpSpriteCat.HasGet = true;
                     prpSpriteCat.HasSet = false;
                     prpSpriteCat.Name = NameToVariable(de.Key.ToString());
                     prpSpriteCat.GetStatements.Add(
                        new CodeMethodReturnStatement(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), "m_" + NameToVariable(de.Key.ToString()))));
                     clsLayer.Members.Add(prpSpriteCat);
                  }
               }
            }
            lyrConstructor.Statements.Add(new CodeAssignStatement(
               new CodeFieldReferenceExpression(
               new CodeThisReferenceExpression(), "m_Sprites"),
               createLayerSprites));
         }
         mthDraw.Statements.Add(new CodeMethodInvokeExpression(refSpr, "End"));
         mthDraw.Statements.Add(new CodeMethodInvokeExpression(refSpr, "Dispose"));
         constructor.Statements.Add(new CodeAssignStatement(fldDisplayRef, refDisp));
         Generator.GenerateCodeFromType(clsMap, txt, GeneratorOptions);
      }

      public void GenerateSpriteDef(ProjectDataset.SpriteDefinitionRow drSpriteDef, System.IO.TextWriter txt)
      {
         CodeNamespace nsSprites = new CodeNamespace("Sprites");
         CodeTypeDeclaration clsSpriteDef = new CodeTypeDeclaration(NameToVariable(drSpriteDef.Name));
         nsSprites.Types.Add(clsSpriteDef);

         clsSpriteDef.BaseTypes.Add("SpriteBase");
         CodeConstructor constructor = new CodeConstructor();
         constructor.Attributes = MemberAttributes.Public;
         constructor.Parameters.AddRange(new CodeParameterDeclarationExpression[]
            {
               new CodeParameterDeclarationExpression(typeof(double), "x"),
               new CodeParameterDeclarationExpression(typeof(double), "y"),
               new CodeParameterDeclarationExpression(typeof(double), "dx"),
               new CodeParameterDeclarationExpression(typeof(double), "dy"),
               new CodeParameterDeclarationExpression(
               new CodeTypeReference("Sprites." + drSpriteDef.Name + ".State"), "state"),
               new CodeParameterDeclarationExpression(typeof(int), "frame"),
               new CodeParameterDeclarationExpression(typeof(bool), "active"),
               new CodeParameterDeclarationExpression("Display", "disp")
            });
         constructor.BaseConstructorArgs.AddRange(new CodeExpression[]
            {
               new CodeArgumentReferenceExpression("x"),
               new CodeArgumentReferenceExpression("y"),
               new CodeArgumentReferenceExpression("dx"),
               new CodeArgumentReferenceExpression("dy"),
               new CodeCastExpression(typeof(int),
               new CodeArgumentReferenceExpression("state")),
               new CodeArgumentReferenceExpression("frame"),
               new CodeArgumentReferenceExpression("active")
            });
         foreach(ProjectDataset.SpriteParameterRow drParam in ProjectData.GetSortedSpriteParameters(drSpriteDef))
         {
            clsSpriteDef.Members.Add(new CodeMemberField(typeof(int), "m_" + NameToVariable(drParam.Name)));
            CodeFieldReferenceExpression refParam = new CodeFieldReferenceExpression(
               new CodeThisReferenceExpression(), "m_" + NameToVariable(drParam.Name));
            constructor.Parameters.Add(new CodeParameterDeclarationExpression(
               typeof(int), NameToVariable(drParam.Name)));
            constructor.Statements.Add(new CodeAssignStatement(refParam,               
               new CodeArgumentReferenceExpression(NameToVariable(drParam.Name))));
            CodeMemberProperty prp = new CodeMemberProperty();
            prp.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            prp.Name = NameToVariable(drParam.Name);
            prp.HasGet = true;
            prp.HasSet = true;
            prp.Type = new CodeTypeReference(typeof(int));
            prp.GetStatements.Add(new CodeMethodReturnStatement(refParam));
            prp.SetStatements.Add(new CodeAssignStatement(refParam, new CodePropertySetValueReferenceExpression()));
            clsSpriteDef.Members.Add(prp);
         }
         clsSpriteDef.Members.Add(constructor);

         CodeTypeDeclaration enumStates = new CodeTypeDeclaration(SpriteStateEnumName);
         System.Collections.ArrayList StateCreators = new System.Collections.ArrayList();

         clsSpriteDef.Members.Add(enumStates);
         
         CachedSpriteDef sprCached = new CachedSpriteDef(drSpriteDef, null);

         foreach(ProjectDataset.SpriteStateRow drState in drSpriteDef.GetSpriteStateRows())
         {
            enumStates.IsEnum = true;
            enumStates.Members.Add(new CodeMemberField(typeof(int), drState.Name));
            int nAccumulatedDuration = 0;
            System.Collections.ArrayList stateParams = new System.Collections.ArrayList();
            stateParams.Add(new CodePrimitiveExpression(drState.SolidWidth));
            stateParams.Add(new CodePrimitiveExpression(drState.SolidHeight));
            stateParams.Add(new CodeFieldReferenceExpression(
               new CodeTypeReferenceExpression(SolidityClassName),
               NameToVariable(drState.SolidityName)));
            stateParams.Add(new CodeArgumentReferenceExpression("disp"));
            stateParams.Add(new CodePrimitiveExpression(drState.FramesetName));
            StateInfo sinf = sprCached[drState.Name];

            stateParams.Add(new CodeObjectCreateExpression(typeof(System.Drawing.Rectangle),
               new CodePrimitiveExpression(sinf.Bounds.X),
               new CodePrimitiveExpression(sinf.Bounds.Y),
               new CodePrimitiveExpression(sinf.Bounds.Width),
               new CodePrimitiveExpression(sinf.Bounds.Height)));
            System.Collections.ArrayList subFrames = null;
            foreach (ProjectDataset.SpriteFrameRow drFrame in ProjectData.GetSortedSpriteFrames(drState))
            {
               if (subFrames == null)
                  subFrames = new System.Collections.ArrayList();
               subFrames.Add(new CodePrimitiveExpression(drFrame.FrameValue));
               if (drFrame.Duration > 0)
               {
                  nAccumulatedDuration += drFrame.Duration;
                  if (subFrames.Count == 1)
                  {
                     stateParams.Add(new CodeObjectCreateExpression("TileFrame",
                        new CodePrimitiveExpression(nAccumulatedDuration),
                        (CodePrimitiveExpression)subFrames[0]));
                  }
                  else
                  {
                     stateParams.Add(new CodeObjectCreateExpression("TileFrame",
                        new CodePrimitiveExpression(nAccumulatedDuration),
                        new CodeArrayCreateExpression(typeof(int), (CodeExpression[])subFrames.ToArray(typeof(CodeExpression)))));
                  }
                  subFrames.Clear();
               }
            }

            StateCreators.Add(new CodeObjectCreateExpression(
               SpriteStateClassName, (CodeExpression[])stateParams.ToArray(typeof(CodeExpression))));
         }

         FrameCache.ClearDisplayCache(null);

         clsSpriteDef.Members.Add(new CodeMemberField(
            new CodeTypeReference(SpriteStateClassName, 1), SpriteStateField));

         CodeMemberProperty prpIndexer = new CodeMemberProperty();
         prpIndexer.Name = "Item";
         prpIndexer.Type = new CodeTypeReference(SpriteStateClassName);
         prpIndexer.Attributes = MemberAttributes.Public | MemberAttributes.Override;
         prpIndexer.HasSet = false;
         prpIndexer.HasGet = true;
         prpIndexer.Parameters.Add(new CodeParameterDeclarationExpression(
            typeof(int), "state"));
         prpIndexer.GetStatements.Add(new CodeMethodReturnStatement(
            new CodeIndexerExpression(new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), SpriteStateField),
            new CodeArgumentReferenceExpression("state"))));
         clsSpriteDef.Members.Add(prpIndexer);

         constructor.Statements.Add(new CodeAssignStatement(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), SpriteStateField),
            new CodeArrayCreateExpression(
            new CodeTypeReference(SpriteStateClassName, 1),
            (CodeObjectCreateExpression[])StateCreators.ToArray(typeof(CodeObjectCreateExpression)))));

         CodeMemberProperty propStateInfo = new CodeMemberProperty();
         propStateInfo.Name = "SolidWidth";
         propStateInfo.Type = new CodeTypeReference(typeof(int));
         propStateInfo.Attributes = MemberAttributes.Override | MemberAttributes.Public;
         propStateInfo.HasSet = false;
         propStateInfo.HasGet = true;
         propStateInfo.GetStatements.Add(new CodeMethodReturnStatement(
            new CodePropertyReferenceExpression(
            new CodeArrayIndexerExpression(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), SpriteStateField),
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), "state")),
            SpriteStateSolidWidth)));
         clsSpriteDef.Members.Add(propStateInfo);

         propStateInfo = new CodeMemberProperty();
         propStateInfo.Name = "SolidHeight";
         propStateInfo.Type = new CodeTypeReference(typeof(int));
         propStateInfo.Attributes = MemberAttributes.Override | MemberAttributes.Public;
         propStateInfo.HasSet = false;
         propStateInfo.HasGet = true;
         propStateInfo.GetStatements.Add(new CodeMethodReturnStatement(
            new CodePropertyReferenceExpression(
            new CodeArrayIndexerExpression(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), SpriteStateField),
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), "state")),
            SpriteStateSolidHeight)));
         clsSpriteDef.Members.Add(propStateInfo);

         Generator.GenerateCodeFromNamespace(nsSprites, txt, GeneratorOptions);
      }

      public void GenerateTileCategories(System.IO.TextWriter txt)
      {
         CodeTypeDeclaration enumCategories = new CodeTypeDeclaration("TileCategoryName");
         enumCategories.IsEnum = true;
         foreach(ProjectDataset.CategoryRow drCat in ProjectData.Category)
         {
            enumCategories.Members.Add(new CodeMemberField(typeof(int), drCat.Name));
         }
         enumCategories.Members.Add(new CodeMemberField(typeof(int), "Count"));

         Generator.GenerateCodeFromType(enumCategories, txt, GeneratorOptions);
      }

      public void GenerateSolidity(System.IO.TextWriter txt)
      {
         CodeTypeDeclaration clsSolidity = new CodeTypeDeclaration(SolidityClassName);
         CodeConstructor constructor = new CodeConstructor();
         clsSolidity.Members.Add(constructor);
         CodeMemberField fldMappings = new CodeMemberField(
            new CodeTypeReference(new CodeTypeReference("SolidityMapping"), 1), "m_mappings");
         fldMappings.Attributes = MemberAttributes.Private | MemberAttributes.Final;
         clsSolidity.Members.Add(fldMappings);
         constructor.Parameters.Add(new CodeParameterDeclarationExpression(
            new CodeTypeReference("SolidityMapping", 1), "mappings"));
         constructor.Statements.Add(new CodeAssignStatement(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), fldMappings.Name),
            new CodeArgumentReferenceExpression("mappings")));

         CodeMemberMethod mthGetTileShape = new CodeMemberMethod();
         mthGetTileShape.Name = "GetCurrentTileShape";
         mthGetTileShape.ReturnType = new CodeTypeReference("TileShape");
         mthGetTileShape.Attributes = MemberAttributes.Public | MemberAttributes.Final;
         mthGetTileShape.Parameters.Add(new CodeParameterDeclarationExpression(
            TileBaseClass, "tile"));
         clsSolidity.Members.Add(mthGetTileShape);

         CodeVariableDeclarationStatement loopvar = new CodeVariableDeclarationStatement(
            typeof(int), "idx", new CodePrimitiveExpression(0));
         CodeArrayIndexerExpression curElem = new CodeArrayIndexerExpression(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), "m_mappings"),
            new CodeVariableReferenceExpression("idx"));

         mthGetTileShape.Statements.Add(new CodeIterationStatement(loopvar,
            new CodeBinaryOperatorExpression(
            new CodeVariableReferenceExpression("idx"), CodeBinaryOperatorType.LessThan,
            new CodePropertyReferenceExpression(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), "m_mappings"), "Length")),
            new CodeAssignStatement(
            new CodeVariableReferenceExpression("idx"),
            new CodeBinaryOperatorExpression(
            new CodeVariableReferenceExpression("idx"), CodeBinaryOperatorType.Add,
            new CodePrimitiveExpression(1))),
            new CodeConditionStatement(
            new CodeMethodInvokeExpression(
            new CodeArgumentReferenceExpression("tile"), "IsMember",
            new CodePropertyReferenceExpression(curElem, "category")),
            new CodeMethodReturnStatement(
            new CodePropertyReferenceExpression(curElem, "shape")))));

         mthGetTileShape.Statements.Add(new CodeMethodReturnStatement(
            new CodePrimitiveExpression(null)));

         foreach(ProjectDataset.TileShapeRow drShape in ProjectData.TileShape)
         {
            CodeTypeDeclaration clsShape = new CodeTypeDeclaration(NameToVariable(drShape.Name));
            CodeConstructor shapeConstructor = new CodeConstructor();
            clsShape.Members.Add(shapeConstructor);
            shapeConstructor.Attributes = MemberAttributes.Public;

            if (string.Compare(drShape.Shape.Trim(' ', ';', '\r', '\n'), bool.TrueString, true) == 0)
            {
               clsShape.BaseTypes.Add("SolidTileShape");
            }
            else if (string.Compare(drShape.Shape.Trim(' ', ';', '\r', '\n'), bool.FalseString, true) == 0)
            {
               clsShape.BaseTypes.Add("EmptyTileShape");
            }
            else
            {
               clsShape.BaseTypes.Add("TileShape");

               CodeMemberMethod mthTestPixel = new CodeMemberMethod();
               mthTestPixel.Name = "TestPixel";
               mthTestPixel.Attributes = MemberAttributes.Override;
               mthTestPixel.Parameters.Add(new CodeParameterDeclarationExpression(typeof(short), "TileWidth"));
               mthTestPixel.Parameters.Add(new CodeParameterDeclarationExpression(typeof(short), "TileHeight"));
               mthTestPixel.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "x"));
               mthTestPixel.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "y"));
               mthTestPixel.ReturnType = new CodeTypeReference(typeof(bool));
               mthTestPixel.Statements.Add(new CodeMethodReturnStatement(
                  new CodeSnippetExpression(drShape.Shape)));
               clsShape.Members.Add(mthTestPixel);

               CodeMemberField fldShape = new CodeMemberField(clsShape.Name, "m_Value");
               fldShape.Attributes = MemberAttributes.Static | MemberAttributes.Private | MemberAttributes.Final;
               fldShape.InitExpression = new CodeObjectCreateExpression(clsShape.Name);
               clsShape.Members.Add(fldShape);

               CodeMemberProperty prpShape = new CodeMemberProperty();
               prpShape.Attributes = MemberAttributes.Static | MemberAttributes.Public | MemberAttributes.Final;
               prpShape.Name = "Value";
               prpShape.HasGet = true;
               prpShape.HasSet = false;
               prpShape.Type = new CodeTypeReference(clsShape.Name);
               prpShape.GetStatements.Add(new CodeMethodReturnStatement(
                  new CodeFieldReferenceExpression(
                  new CodeThisReferenceExpression(), fldShape.Name)));
               clsShape.Members.Add(prpShape);
            }

            Generator.GenerateCodeFromType(clsShape, txt, GeneratorOptions);
         }

         foreach(ProjectDataset.SolidityRow drSolid in ProjectData.Solidity)
         {
            CodeMemberField fldSolid = new CodeMemberField(SolidityClassName, "m_" +
               NameToVariable(drSolid.Name));
            fldSolid.Attributes = MemberAttributes.Private | MemberAttributes.Static | MemberAttributes.Final;
            System.Collections.ArrayList mappings = new System.Collections.ArrayList();
            foreach (ProjectDataset.SolidityShapeRow drShape in drSolid.GetSolidityShapeRows())
            {
               mappings.Add(new CodeObjectCreateExpression("SolidityMapping",
                  new CodeFieldReferenceExpression(
                  new CodeTypeReferenceExpression("TileCategoryName"),
                  drShape.CategoryRowParent.Name),
                  new CodeFieldReferenceExpression(
                  new CodeTypeReferenceExpression(NameToVariable(drShape.ShapeName)), "Value")));
            }
            fldSolid.InitExpression =
               new CodeObjectCreateExpression("Solidity", 
               new CodeArrayCreateExpression("SolidityMapping",
               (CodeObjectCreateExpression[])
               mappings.ToArray(typeof(CodeObjectCreateExpression))));
            clsSolidity.Members.Add(fldSolid);

            CodeMemberProperty prpSolid = new CodeMemberProperty();
            prpSolid.Name = NameToVariable(drSolid.Name);
            prpSolid.Type = new CodeTypeReference(SolidityClassName);
            prpSolid.Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final;
            prpSolid.HasSet = false;
            prpSolid.HasGet = true;
            prpSolid.GetStatements.Add(
               new CodeMethodReturnStatement(
               new CodeFieldReferenceExpression(
               new CodeTypeReferenceExpression(clsSolidity.Name), fldSolid.Name)));
            clsSolidity.Members.Add(prpSolid);
         }

         Generator.GenerateCodeFromType(clsSolidity, txt, GeneratorOptions);
      }
      #endregion

      #region Utility Functions
      private string NameToVariable(string name)
      {
         return name.Replace(" ","_");
      }
      #endregion

      #region Compilation
      public string CompileTempAssembly(out string errs)
      {
         string[] code = GenerateCodeStrings(out errs);
         if (errs.Length > 0)
            return null;

         System.IO.Stream remoteReflectorStream  = System.Reflection.Assembly.GetAssembly(typeof(SGDK2IDE)).GetManifestResourceStream("SGDK2.RemoteReflector.cs");
         System.IO.StreamReader readReflector = new System.IO.StreamReader(remoteReflectorStream);
         string[] tmpcode = new string[code.Length + 1];
         code.CopyTo(tmpcode, 0);
         tmpcode[code.Length] = readReflector.ReadToEnd();
         code = tmpcode;
         readReflector.Close();
         
         Microsoft.CSharp.CSharpCodeProvider codeProvider = new Microsoft.CSharp.CSharpCodeProvider();
         System.CodeDom.Compiler.ICodeCompiler compiler = codeProvider.CreateCompiler();
         System.CodeDom.Compiler.CompilerParameters compilerParams = new System.CodeDom.Compiler.CompilerParameters(new string[] {}, System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "SGDK2Tmp.dll"));
         System.Reflection.Assembly asmRef = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.DirectX.Matrix));
         compilerParams.ReferencedAssemblies.Add(asmRef.GetFiles()[0].Name);
         asmRef = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.DirectX.Direct3D.Device));
         compilerParams.ReferencedAssemblies.Add(asmRef.GetFiles()[0].Name);
         asmRef = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.DirectX.Direct3D.Sprite));
         compilerParams.ReferencedAssemblies.Add(asmRef.GetFiles()[0].Name);
         compilerParams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
         compilerParams.ReferencedAssemblies.Add("System.dll");
         compilerParams.ReferencedAssemblies.Add("System.Drawing.dll");
         compilerParams.ReferencedAssemblies.Add(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "SGDK2IDE.exe"));
         compilerParams.GenerateExecutable = false;
         System.CodeDom.Compiler.CompilerResults results = compiler.CompileAssemblyFromSourceBatch(compilerParams, code);
         if (results.Errors.Count > 0)
         {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < results.Errors.Count; i++)
            {
               sb.Append(results.Errors[i].ToString() + Environment.NewLine);
            }
            errs = sb.ToString();
            return null;
         }
         return compilerParams.OutputAssembly;
      }
      #endregion
   }
}
