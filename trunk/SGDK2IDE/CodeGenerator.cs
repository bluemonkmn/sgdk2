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
         GeneratorOptions.IndentString = "   ";
      }

      #region Constants
      public const string SpritesNamespace = "Sprites";
      public const string SpriteBaseClass = "SpriteBase";
      private const string ResourcesField = "m_res";
      private const string ResourcesProperty = "m_res";
      private const string ProjectClass = "Project";
      private const string FramesetClass = "Frameset";
      private const string FrameClass = "Frame";
      public const string CounterClass = "Counter";
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
      public const string SolidityClassName = "Solidity";
      private const string SpriteStateClassName = "SpriteState";
      private const string SpriteStateField = "m_SpriteStates";
      public const string SpriteStateEnumName = "State";
      private const string GetFramesetMethodName = "GetFrameset";
      private const string CoordinateTypeName = "PlanBase.Coordinate";
      private const string LayerParentField = "m_ParentMap";
      private const string LayerParentArg = "ParentMap";
      public const string SpritePlanParentField = "m_ParentLayer";
      public const string SpritePlanParentProperty = "ParentLayer";
      private const string SpritePlanParentArg = "ParentLayer";
      private const string GameFormInstance = "GameWindow";
      private const string GameFormType = "GameForm";
      private const string SpriteIsActiveRef = "isActive";
      private const string UndefinedSolidityProperty = "UndefinedSolidity";
      private const string SpriteProcessedRulesField = "Processed";
      private const string LayerSpriteCategoriesBaseClassName = "LayerSpriteCategoriesBase";
      public const string LayerSpriteCategoriesClassName = "LayerSpriteCategories";
      public const string SpriteCollectionClassName = "SpriteCollection";
      private const string LayerBaseClassName = "LayerBase";
      public const string PlanBaseClassName = "PlanBase";
      public const string SpriteCategoriesFieldName = "m_SpriteCategories";
      private const string TilesetRefClassName = "TilesetRef";
      private const string TilesetIndexSerializeName = "TilesetIndex";
      private const string FramesetRefClassName = "FramesetRef";
      private const string FramesetSerializeName = "FramesetName";
      private const string GameDisplayField = "GameDisplay";
      #endregion

      #region Embedded Types
      private class RuleContent
      {
         public string Name;
         public string Type;
         public string Function;
         public string Parameter1;
         public string Parameter2;
         public string Parameter3;
         public string ResultParameter;
         public bool EndIf;

         public RuleContent(ProjectDataset.SpriteRuleRow row)
         {
            Name = row.Name;
            Type = row.Type;
            Function = row.Function;
            if (row.IsParameter1Null())
               Parameter1 = null;
            else
               Parameter1 = row.Parameter1;
            if (row.IsParameter2Null())
               Parameter2 = null;
            else
               Parameter2 = row.Parameter2;
            if (row.IsParameter3Null())
               Parameter3 = null;
            else
               Parameter3 = row.Parameter3;
            if (row.IsResultParameterNull())
               ResultParameter = null;
            else
               ResultParameter = row.ResultParameter;
            EndIf = row.EndIf;
         }
         public RuleContent(ProjectDataset.PlanRuleRow row)
         {
            Name = row.Name;
            Type = row.Type;
            Function = row.Function;
            if (row.IsParameter1Null())
               Parameter1 = null;
            else
               Parameter1 = row.Parameter1;
            if (row.IsParameter2Null())
               Parameter2 = null;
            else
               Parameter2 = row.Parameter2;
            if (row.IsParameter3Null())
               Parameter3 = null;
            else
               Parameter3 = row.Parameter3;
            if (row.IsResultParameterNull())
               ResultParameter = null;
            else
               ResultParameter = row.ResultParameter;
            EndIf = row.EndIf;
         }
      }
      private class ConditionStackElement
      {
         private int mode; // 0 = If, 1 = Else, 2 = While
         private CodeStatement statement;
         private RuleContent rule;
         public ConditionStackElement(CodeStatement statement, RuleContent rule)
         {
            if (statement is CodeConditionStatement)
               this.mode = 0;
            else if (statement is CodeIterationStatement)
               this.mode = 2;
            else
               throw new ApplicationException("Unexpected condition type");

            this.statement = statement;
            this.rule = rule;
         }
         public CodeExpression Condition
         {
            get
            {
               if (statement is CodeIterationStatement)
                  return ((CodeIterationStatement)statement).TestExpression;
               else
                  return ((CodeConditionStatement)statement).Condition;
            }
            set
            {
               if (statement is CodeIterationStatement)
                  ((CodeIterationStatement)statement).TestExpression = value;
               else
                  ((CodeConditionStatement)statement).Condition = value;
            }
         }
         public void NextPhase()
         {
            if (mode == 0)
               mode = 1;
            else
               throw new System.ApplicationException("Rule \"" + RuleName + "\" has too many else conditions.");
         }
         public CodeStatement Statement
         {
            get
            {
               return statement;
            }
         }
         public CodeConditionStatement ConditionStatement
         {
            get
            {
               return (CodeConditionStatement)statement;
            }
         }
         public CodeIterationStatement IteratorStatement
         {
            get
            {
               return (CodeIterationStatement)statement;
            }
         }
         public CodeStatementCollection ChildStatements
         {
            get
            {
               switch(mode)
               {
                  case 0:
                     return ConditionStatement.TrueStatements;
                  case 1:
                     return ConditionStatement.FalseStatements;
                  case 2:
                     return IteratorStatement.Statements;
                  default:
                     throw new ApplicationException("Unexpected mode in ChildStatements");
               }
            }
         }
         public string RuleName
         {
            get
            {
               return rule.Name;
            }
         }
      }

      public enum CodeLevel
      {
         ExcludeRules,
         IncludeAll
      }

      private class TempAssembly : MarshalByRefObject, IDisposable
      {
         private string m_assemblyFile;
         private AppDomain m_tempDomain;

         public TempAssembly(string assemblyFile)
         {
            m_tempDomain = AppDomain.CreateDomain("TempDomain");
            m_assemblyFile = assemblyFile;
            if (!System.IO.File.Exists(assemblyFile))
               throw new System.IO.FileNotFoundException("Specified assembly file was not found", assemblyFile);
         }

         public object CreateInstanceAndUnwrap(string typeName, object[] constructorParams)
         {
            string asmName = System.IO.Path.GetFileNameWithoutExtension(m_assemblyFile);
            return m_tempDomain.CreateInstanceAndUnwrap(asmName, typeName, false,
               System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
               null, constructorParams, null, null, null);
         }

         #region IDisposable Members
         public void Dispose()
         {
            AppDomain.Unload(m_tempDomain);
            System.IO.File.Delete(m_assemblyFile);
         }
         #endregion
      }
      #endregion

      public System.CodeDom.Compiler.ICodeGenerator Generator = new Microsoft.CSharp.CSharpCodeProvider().CreateGenerator();
      public CodeGeneratorOptions GeneratorOptions = new CodeGeneratorOptions();
      public bool Debug = false;
      public CodeLevel GenerateLevel = CodeLevel.IncludeAll;
      private static TempAssembly m_TempAssembly = null;

      #region Project Level Code Generation
      public void GenerateAllCode(string FolderName, out string errs)
      {
         System.IO.TextWriter err = new System.IO.StringWriter();
         System.IO.TextWriter txt = null;

         try
         {
            if (!System.IO.Path.IsPathRooted(FolderName))
            {
               FolderName = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, FolderName);
            }
            if (!System.IO.Directory.Exists(FolderName))
               System.IO.Directory.CreateDirectory(FolderName);

            txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, ProjectClass + ".cs"));
            GenerateMainCode(txt, err);
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

            foreach (System.Data.DataRowView drv in ProjectData.Map.DefaultView)
            {
               ProjectDataset.MapRow drMap = (ProjectDataset.MapRow)drv.Row;
               txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, NameToVariable(drMap.Name) + "_Map.resx"));
               GenerateMapResx(drMap, txt);
               txt.Close();

               txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, NameToVariable(drMap.Name) + "_Map.cs"));
               GenerateMap(drMap, txt, err);
               txt.Close();
            }

            string SpritesFolder = System.IO.Path.Combine(FolderName, "Sprites");
            if (!System.IO.Directory.Exists(SpritesFolder))
               System.IO.Directory.CreateDirectory(SpritesFolder);
            foreach (System.Data.DataRowView drv in ProjectData.SpriteDefinition.DefaultView)
            {
               ProjectDataset.SpriteDefinitionRow drSpriteDef = (ProjectDataset.SpriteDefinitionRow)drv.Row;
               txt = new System.IO.StreamWriter(System.IO.Path.Combine(SpritesFolder, NameToVariable(drSpriteDef.Name) + ".cs"));
               GenerateSpriteDef(drSpriteDef, txt, err);
               txt.Close();
            }

            txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, SolidityClassName + ".cs"));
            GenerateTileCategories(txt);
            GenerateSolidity(txt);
            txt.Close();

            txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, LayerSpriteCategoriesBaseClassName + ".cs"));
            GenerateLayerSpriteCategoriesBase(txt);
            txt.Close();

            txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, "AssemblyInfo.cs"));
            GenerateAssemblyInfo(txt);
            txt.Close();

            GenerateProjectSourceCode(FolderName, err);
            GenerateEmbeddedResources(FolderName);
         }
         catch(System.Exception ex)
         {
            if (txt != null)
               txt.Close();
            System.Windows.Forms.MessageBox.Show(ex.Message, "Error Generating Project", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
         }

         errs = err.ToString();
         err.Close();
      }

      public string[] GetCodeFileList(string FolderName)
      {
         System.Collections.ArrayList fileList = new System.Collections.ArrayList();

         if (!System.IO.Path.IsPathRooted(FolderName))
         {
            FolderName = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, FolderName);
         }

         fileList.AddRange(new string[]
            {
               System.IO.Path.Combine(FolderName, ProjectClass + ".cs"),
               System.IO.Path.Combine(FolderName, CounterClass + ".cs"),
               System.IO.Path.Combine(FolderName, FramesetClass + ".cs"),
               System.IO.Path.Combine(FolderName, TilesetClass + ".cs"),
               System.IO.Path.Combine(FolderName, SolidityClassName + ".cs"),
               System.IO.Path.Combine(FolderName, LayerSpriteCategoriesBaseClassName + ".cs"),
               System.IO.Path.Combine(FolderName, "AssemblyInfo.cs")
            });

         foreach (System.Data.DataRowView drv in ProjectData.Map.DefaultView)
            fileList.Add(System.IO.Path.Combine(FolderName, NameToVariable(((ProjectDataset.MapRow)drv.Row).Name) + "_Map.cs"));

         string SpritesFolder = System.IO.Path.Combine(FolderName, "Sprites");
         foreach (System.Data.DataRowView drv in ProjectData.SpriteDefinition.DefaultView)
            fileList.Add(System.IO.Path.Combine(SpritesFolder, NameToVariable(((ProjectDataset.SpriteDefinitionRow)drv.Row).Name) + ".cs"));

         foreach (System.Data.DataRowView drv in ProjectData.SourceCode.DefaultView)
         {
            ProjectDataset.SourceCodeRow drCode = (ProjectDataset.SourceCodeRow)drv.Row;
            if (!drCode.IsTextNull() && (drCode.Text.Trim().Length > 0) && drCode.Name.EndsWith(".cs"))
               fileList.Add(System.IO.Path.Combine(FolderName, drCode.Name));
         }

         return (string[])(fileList.ToArray(typeof(string)));
      }

      public string[] GetResxFileList(string FolderName)
      {
         System.Collections.ArrayList fileList = new System.Collections.ArrayList();
         if (!System.IO.Path.IsPathRooted(FolderName))
            FolderName = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, FolderName);
         fileList.Add(System.IO.Path.Combine(FolderName, ProjectClass + ".resx"));
         foreach (System.Data.DataRowView drv in ProjectData.Map.DefaultView)
            fileList.Add(System.IO.Path.Combine(FolderName, NameToVariable(((ProjectDataset.MapRow)drv.Row).Name) + "_Map.resx"));
         return (string[])(fileList.ToArray(typeof(string)));
      }

      public string[] GetResourcesFileList(string FolderName)
      {
         string[] result = GetResxFileList(FolderName);
         for (int i=0; i<result.Length; i++)
            result[i] = result[i].Substring(0,result[i].Length -
               System.IO.Path.GetExtension(result[i]).Length) + ".resources";
         return result;
      }

      public string[] GetEmbeddedResourceList(string FolderName)
      {
         System.Collections.ArrayList fileList = new System.Collections.ArrayList();
         if (!System.IO.Path.IsPathRooted(FolderName))
            FolderName = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, FolderName);
         foreach(System.Data.DataRowView drv in ProjectData.SourceCode.DefaultView)
         {
            ProjectDataset.SourceCodeRow drCode = (ProjectDataset.SourceCodeRow)drv.Row;
            if ((!drCode.IsCustomObjectDataNull()) && drCode.Name.EndsWith(".cs"))
               fileList.Add(System.IO.Path.Combine(FolderName, System.IO.Path.GetFileNameWithoutExtension(drCode.Name)) + ".bin");
         }
         return (string[])(fileList.ToArray(typeof(string)));
      }

      public string[] GetLocalReferenceFileList(string FolderName)
      {
         System.Collections.ArrayList fileList = new System.Collections.ArrayList();
         if (!System.IO.Path.IsPathRooted(FolderName))
         {
            FolderName = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, FolderName);
         }
         System.Reflection.Assembly asmRef = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.DirectX.Matrix));
         fileList.Add(System.IO.Path.Combine(FolderName, System.IO.Path.GetFileName(asmRef.GetFiles()[0].Name)));
         asmRef = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.DirectX.Direct3D.Device));
         fileList.Add(System.IO.Path.Combine(FolderName, System.IO.Path.GetFileName(asmRef.GetFiles()[0].Name)));
         asmRef = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.DirectX.Direct3D.Sprite));
         fileList.Add(System.IO.Path.Combine(FolderName, System.IO.Path.GetFileName(asmRef.GetFiles()[0].Name)));
         asmRef = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.DirectX.DirectInput.KeyboardState));
         fileList.Add(System.IO.Path.Combine(FolderName, System.IO.Path.GetFileName(asmRef.GetFiles()[0].Name)));
         foreach(System.Data.DataRowView drv in ProjectData.SourceCode.DefaultView)
         {
            ProjectDataset.SourceCodeRow drCode = (ProjectDataset.SourceCodeRow)drv.Row;
            if (drCode.Name.EndsWith(".dll"))
               fileList.Add(System.IO.Path.Combine(FolderName, System.IO.Path.GetFileName(drCode.Name)));
         }
         return (string[])(fileList.ToArray(typeof(string)));
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

         GenerateMainCode(txt, err);
         txt.Close();
         result.Add(txt.ToString());

         foreach(System.Data.DataRowView drv in ProjectData.SourceCode.DefaultView)
         {
            ProjectDataset.SourceCodeRow drCode = (ProjectDataset.SourceCodeRow)drv.Row;
            if (!drCode.IsTextNull() && (drCode.Text.Trim().Length > 0) && drCode.Name.EndsWith(".cs"))
               try
               {
                  result.Add(ProjectData.GetSourceCodeText(drCode));
               }
               catch(ApplicationException ex)
               {
                  err.WriteLine(ex.Message);
               }
         }

         txt = new System.IO.StringWriter();
         GenerateCounters(txt);
         txt.Close();
         result.Add(txt.ToString());

         /*txt = new System.IO.StringWriter(System.IO.Path.Combine(FolderName, ProjectClass + ".resx"));
            GenerateResx(txt);
            txt.Close();*/

         foreach (System.Data.DataRowView drv in ProjectData.Map.DefaultView)
         {
            ProjectDataset.MapRow drMap = (ProjectDataset.MapRow)drv.Row;
            /* Don't need this in the string-only version of the code
                * txt = new System.IO.StringWriter(System.IO.Path.Combine(FolderName, "Map" + drMap.Name + ".resx"));
               GenerateMapResx(drMap, txt);
               txt.Close();*/

            txt = new System.IO.StringWriter();
            GenerateMap(drMap, txt, err);
            txt.Close();
            result.Add(txt.ToString());
         }

         txt = new System.IO.StringWriter();
         GenerateFramesets(txt);
         txt.Close();
         result.Add(txt.ToString());

         txt = new System.IO.StringWriter();
         GenerateTilesets(txt, err);
         txt.Close();
         result.Add(txt.ToString());

         txt = new System.IO.StringWriter();
         GenerateTileCategories(txt);
         GenerateSolidity(txt);
         txt.Close();
         result.Add(txt.ToString());

         txt = new System.IO.StringWriter();
         GenerateLayerSpriteCategoriesBase(txt);
         txt.Close();
         result.Add(txt.ToString());

         txt = new System.IO.StringWriter();
         GenerateAssemblyInfo(txt);
         txt.Close();
         result.Add(txt.ToString());

         foreach (System.Data.DataRowView drv in ProjectData.SpriteDefinition.DefaultView)
         {
            ProjectDataset.SpriteDefinitionRow drSpriteDef = (ProjectDataset.SpriteDefinitionRow)drv.Row;
            txt = new System.IO.StringWriter();
            GenerateSpriteDef(drSpriteDef, txt, err);
            txt.Close();
            result.Add(txt.ToString());
         }

         errs = err.ToString();
         err.Close();

         return (string[])result.ToArray(typeof(string));
      }
      
      public void GenerateProjectSourceCode(string FolderName, System.IO.TextWriter err)
      {
         foreach (System.Data.DataRowView drv in ProjectData.SourceCode.DefaultView)
         {
            ProjectDataset.SourceCodeRow drCode = (ProjectDataset.SourceCodeRow)drv.Row;
            if (!drCode.IsTextNull() && (drCode.Text.Trim().Length > 0) && drCode.Name.EndsWith(".cs"))
            {
               System.IO.TextWriter txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, drCode.Name));
               try
               {
                  txt.Write(ProjectData.GetSourceCodeText(drCode));
               }
               catch(System.Exception ex)
               {
                  err.WriteLine(ex.Message);
               }
               txt.Close();
            }
         }
      }

      public void GenerateEmbeddedResources(string FolderName)
      {
         foreach (System.Data.DataRowView drv in ProjectData.SourceCode.DefaultView)
         {
            ProjectDataset.SourceCodeRow drCode = (ProjectDataset.SourceCodeRow)drv.Row;
            if (drCode.Name.EndsWith(".cs") && !drCode.IsCustomObjectDataNull())
            {
               System.IO.FileStream fs = new System.IO.FileStream(System.IO.Path.Combine(FolderName, System.IO.Path.GetFileNameWithoutExtension(drCode.Name) + ".bin"), System.IO.FileMode.Create);
               fs.Write(drCode.CustomObjectData, 0, drCode.CustomObjectData.Length);
               fs.Close();
            }
            else if (drCode.Name.EndsWith(".dll") && !drCode.IsCustomObjectDataNull())
            {
               System.IO.FileStream fs = new System.IO.FileStream(System.IO.Path.Combine(FolderName, System.IO.Path.GetFileName(drCode.Name)), System.IO.FileMode.Create);
               fs.Write(drCode.CustomObjectData, 0, drCode.CustomObjectData.Length);
               fs.Close();
            }
         }
      }
      #endregion

      #region File Level Code Generation
      public void GenerateMainCode(System.IO.TextWriter txt, System.IO.TextWriter err)
      {
         CodeTypeDeclaration typ = new CodeTypeDeclaration(ProjectClass);
         CodeMemberField resDecl = new CodeMemberField(typeof(System.Resources.ResourceManager), ResourcesField);
         resDecl.Attributes |= MemberAttributes.Static;
         typ.Members.Add(resDecl);
         CodeEntryPointMethod main = new CodeEntryPointMethod();
         typ.Members.Add(main);

         CodeTryCatchFinallyStatement tryBlock = new CodeTryCatchFinallyStatement();

         CodeFieldReferenceExpression resRef = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(ProjectClass), ResourcesField);
         CodeObjectCreateExpression createResources = new CodeObjectCreateExpression(typeof(System.Resources.ResourceManager), new CodeTypeOfExpression(ProjectClass));
         CodeAssignStatement assign = new CodeAssignStatement(resRef, createResources);
         tryBlock.TryStatements.Add(assign);

         ProjectDataset.ProjectRow prj = ProjectData.ProjectRow;
         CodeMemberField declareGame = new CodeMemberField(GameFormType, "game");
         declareGame.Attributes = MemberAttributes.Private | MemberAttributes.Final | MemberAttributes.Static;
         typ.Members.Add(declareGame);
         CodeExpression overlay = new CodePrimitiveExpression(null);
         if ((prj.OverlayMap != null) && !System.Convert.IsDBNull(prj.OverlayMap) && prj.OverlayMap.Length > 0)
            overlay = new CodeTypeOfExpression(NameToMapClass(prj.OverlayMap));
         if ((prj.StartMap == null) || (prj.StartMap.Length == 0))
            err.WriteLine("Startup map has not been specified in the project settings");
         tryBlock.TryStatements.Add(new CodeAssignStatement(
            new CodeFieldReferenceExpression(
            new CodeTypeReferenceExpression(typ.Name), declareGame.Name),
            new CodeObjectCreateExpression(GameFormType,
            new CodeFieldReferenceExpression(
            new CodeTypeReferenceExpression("GameDisplayMode"), prj.DisplayMode),
            new CodePrimitiveExpression(prj.Windowed),
            new CodePrimitiveExpression(prj.TitleText),
            ((prj.StartMap != null) && (prj.StartMap.Length > 0)) ?
            (CodeExpression)new CodeTypeOfExpression(NameToMapClass(prj.StartMap)):(CodeExpression)new CodePrimitiveExpression(null),
            overlay)));
         tryBlock.TryStatements.Add(new CodeMethodInvokeExpression(
            new CodeVariableReferenceExpression(declareGame.Name), "Show"));
         tryBlock.TryStatements.Add(new CodeMethodInvokeExpression(
            new CodeVariableReferenceExpression(declareGame.Name), "Run"));

         tryBlock.CatchClauses.Add(new CodeCatchClause(
            "ex", new CodeTypeReference(typeof(System.Exception)),
            new CodeExpressionStatement(
            new CodeMethodInvokeExpression(
            new CodeTypeReferenceExpression(GameFormType),
            "HandleException",
            new CodeVariableReferenceExpression("ex")))));

         main.Statements.Add(tryBlock);

         CodeMemberProperty prpRes = new CodeMemberProperty();
         prpRes.HasSet = false;
         prpRes.HasGet = true;
         prpRes.Name = "Resources";
         prpRes.Attributes = MemberAttributes.Final | MemberAttributes.Public | MemberAttributes.Static;
         prpRes.Type = new CodeTypeReference(typeof(System.Resources.ResourceManager));
         prpRes.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(ResourcesField)));
         typ.Members.Add(prpRes);

         CodeMemberProperty prpGame = new CodeMemberProperty();
         prpGame.HasSet = false;
         prpGame.HasGet = true;
         prpGame.Name = GameFormInstance;
         prpGame.Attributes = MemberAttributes.Final | MemberAttributes.Public | MemberAttributes.Static;
         prpGame.Type = new CodeTypeReference(GameFormType);
         prpGame.GetStatements.Add(new CodeMethodReturnStatement(
            new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typ.Name), "game")));
         typ.Members.Add(prpGame);

         CodeMemberField constMaxPlayers = new CodeMemberField(typeof(byte), "MaxPlayers");
         constMaxPlayers.Attributes = MemberAttributes.Const | MemberAttributes.Public;
         constMaxPlayers.InitExpression = new CodePrimitiveExpression(ProjectData.ProjectRow.MaxPlayers);
         typ.Members.Add(constMaxPlayers);

         CodeMemberField constMaxViews = new CodeMemberField(typeof(byte), "MaxViews");
         constMaxViews.Attributes = MemberAttributes.Const | MemberAttributes.Public;
         constMaxViews.InitExpression = new CodePrimitiveExpression(ProjectData.ProjectRow.MaxViews);
         typ.Members.Add(constMaxViews);

         CodeMemberField constCredits = new CodeMemberField(typeof(string), "GameCredits");
         constCredits.Attributes = MemberAttributes.Const | MemberAttributes.Public;
         if (!ProjectData.ProjectRow.IsCreditsNull())
            constCredits.InitExpression = new CodePrimitiveExpression(ProjectData.ProjectRow.Credits);
         else
            constCredits.InitExpression = new CodePrimitiveExpression(String.Empty);
         typ.Members.Add(constCredits);

         Generator.GenerateCodeFromType(typ, txt, GeneratorOptions);
      }

      public void GenerateResx(System.IO.TextWriter txt)
      {
         System.Resources.ResXResourceWriter w = new System.Resources.ResXResourceWriter(txt);
         try
         {
            foreach(System.Data.DataRowView drv in ProjectData.GraphicSheet.DefaultView)
            {
               ProjectDataset.GraphicSheetRow drGfx = (ProjectDataset.GraphicSheetRow)drv.Row;
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
         framesetClassDecl.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
         framesetClassDecl.BaseTypes.Add(typeof(System.Runtime.Serialization.ISerializable));
         framesetClassDecl.Members.Add(new CodeMemberField(new CodeTypeReference(FrameClass, 1), "m_arFrames"));
         framesetClassDecl.Members.Add(new CodeMemberField("Display", "m_Display"));
         CodeMemberField fldFramesetName = new CodeMemberField(typeof(string), "Name");
         framesetClassDecl.Members.Add(fldFramesetName);
         CodeConstructor framesetConstructor = new CodeConstructor();
         framesetConstructor.Attributes = MemberAttributes.Final | MemberAttributes.Private;
         framesetConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "Name"));
         framesetConstructor.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference("Display"), "disp"));
         framesetConstructor.Statements.Add(
            new CodeAssignStatement(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), fldFramesetName.Name),
            new CodeArgumentReferenceExpression("Name")));
         framesetClassDecl.Members.Add(framesetConstructor);

         // Implement ISerializable
         CodeMemberMethod mthGetObjectData = CreateGetObjectDataMethod();
         framesetClassDecl.Members.Add(mthGetObjectData);
         mthGetObjectData.Statements.Add(
            new CodeMethodInvokeExpression(
            new CodeArgumentReferenceExpression("info"),
            "SetType", new CodeExpression[]
            {new CodeTypeOfExpression(FramesetRefClassName)}));
         mthGetObjectData.Statements.Add(
            new CodeMethodInvokeExpression(
            new CodeArgumentReferenceExpression("info"),
            "AddValue", new CodeExpression[]
               {
                  new CodePrimitiveExpression(FramesetSerializeName),
                  new CodeFieldReferenceExpression(
                  new CodeThisReferenceExpression(), fldFramesetName.Name)
               }));


         CodeTypeDeclaration classFramesetRef = new CodeTypeDeclaration(FramesetRefClassName);
         classFramesetRef.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
         classFramesetRef.BaseTypes.Add(typeof(System.Runtime.Serialization.IObjectReference));
         classFramesetRef.BaseTypes.Add(typeof(System.Runtime.Serialization.ISerializable));
         CodeMemberField fldFramesetRefName = new CodeMemberField(typeof(string), "m_FramesetName");
         classFramesetRef.Members.Add(fldFramesetRefName);
         CodeConstructor framesetRefConstructor = new CodeConstructor();
         classFramesetRef.Members.Add(framesetRefConstructor);
         framesetRefConstructor.Parameters.Add(
            new CodeParameterDeclarationExpression(
            typeof(System.Runtime.Serialization.SerializationInfo), "info"));
         framesetRefConstructor.Parameters.Add(
            new CodeParameterDeclarationExpression(
            typeof(System.Runtime.Serialization.StreamingContext), "context"));
         framesetRefConstructor.Statements.Add(
            new CodeAssignStatement(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), fldFramesetRefName.Name),
            new CodeMethodInvokeExpression(
            new CodeArgumentReferenceExpression("info"), "GetString",
            new CodeExpression[]
            {new CodePrimitiveExpression(FramesetSerializeName)})));

         CodeMemberMethod mthRefGetObjectData = CreateGetObjectDataMethod();
         classFramesetRef.Members.Add(mthRefGetObjectData);
         mthRefGetObjectData.Statements.Add(
            new CodeThrowExceptionStatement(
            new CodeObjectCreateExpression(typeof(NotImplementedException),
            new CodeExpression[]
            { new CodePrimitiveExpression("Unexpected serialization call") })));

         CodeMemberMethod mthGetRealObject = new CodeMemberMethod();
         classFramesetRef.Members.Add(mthGetRealObject);
         mthGetRealObject.Attributes = MemberAttributes.Public | MemberAttributes.Final;
         mthGetRealObject.Name = "GetRealObject";
         mthGetRealObject.Parameters.Add(
            new CodeParameterDeclarationExpression(
            typeof(System.Runtime.Serialization.StreamingContext), "context"));
         mthGetRealObject.ReturnType = new CodeTypeReference(typeof(object));

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

         mthGetRealObject.Statements.Add(
            new CodeMethodReturnStatement(
            new CodeMethodInvokeExpression(
            new CodeTypeReferenceExpression(FramesetClass),
            mthGetFrameset.Name, new CodeExpression[]
            {
               new CodeFieldReferenceExpression(
               new CodeThisReferenceExpression(), fldFramesetRefName.Name),
               new CodeFieldReferenceExpression(
               new CodePropertyReferenceExpression(
               new CodeTypeReferenceExpression(ProjectClass), GameFormInstance),
               GameDisplayField)
            })));

         foreach(System.Data.DataRowView drv in ProjectData.Frameset.DefaultView)
         {
            ProjectDataset.FramesetRow drFrameset = (ProjectDataset.FramesetRow)drv.Row;
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
                  if (drFrame.color == -1)
                     frameParams.Add(new CodeObjectCreateExpression(FrameClass, getTextureExp, cellExp, rectExp));
                  else
                     frameParams.Add(new CodeObjectCreateExpression(FrameClass, getTextureExp, cellExp, rectExp, new CodePrimitiveExpression(drFrame.color)));
               }
               else
               {
                  CodePrimitiveExpression m11Exp = new CodePrimitiveExpression(drFrame.m11);
                  CodePrimitiveExpression m12Exp = new CodePrimitiveExpression(drFrame.m12);
                  CodePrimitiveExpression m21Exp = new CodePrimitiveExpression(drFrame.m21);
                  CodePrimitiveExpression m22Exp = new CodePrimitiveExpression(drFrame.m22);
                  CodePrimitiveExpression m41Exp = new CodePrimitiveExpression(drFrame.dx);
                  CodePrimitiveExpression m42Exp = new CodePrimitiveExpression(drFrame.dy);
                  if (drFrame.color == -1)
                     frameParams.Add(new CodeObjectCreateExpression(FrameClass, getTextureExp, cellExp, m11Exp, m12Exp, m21Exp, m22Exp, m41Exp, m42Exp, rectExp));
                  else
                     frameParams.Add(new CodeObjectCreateExpression(FrameClass, getTextureExp, cellExp, m11Exp, m12Exp, m21Exp, m22Exp, m41Exp, m42Exp, rectExp, new CodePrimitiveExpression(drFrame.color)));
               }
            }
            CodeArrayCreateExpression createArray = new CodeArrayCreateExpression(FrameClass,(CodeExpression[])frameParams.ToArray(typeof(CodeExpression)));
            curCondition.TrueStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "m_arFrames"), createArray));
         }
         curCondition.FalseStatements.Clear();
         if (topCondition.Condition != null)
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
         Generator.GenerateCodeFromType(classFramesetRef, txt, GeneratorOptions);
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
            new CodeBinaryOperatorExpression(new CodePropertySetValueReferenceExpression(),
            CodeBinaryOperatorType.GreaterThanOrEqual,
            minValue), new CodeStatement[] {assignToProp}, new CodeStatement[]
               {new CodeAssignStatement(fldRef, minValue)});
         CodeConditionStatement testMax = new CodeConditionStatement(
            new CodeBinaryOperatorExpression(new CodePropertySetValueReferenceExpression(),
            CodeBinaryOperatorType.LessThanOrEqual,
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


         foreach(System.Data.DataRowView drv in ProjectData.Counter.DefaultView)
         {
            ProjectDataset.CounterRow drCounter = (ProjectDataset.CounterRow)drv.Row;
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
         classTileset.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
         classTileset.BaseTypes.Add(typeof(System.Runtime.Serialization.ISerializable));
         CodeTypeConstructor staticConstructor = new CodeTypeConstructor();
         classTileset.Members.Add(staticConstructor);
         staticConstructor.Attributes |= MemberAttributes.Static;

         // Implement ISerializable
         CodeMemberMethod mthGetObjectData = CreateGetObjectDataMethod();
         classTileset.Members.Add(mthGetObjectData);
         mthGetObjectData.Statements.Add(
            new CodeMethodInvokeExpression(
            new CodeArgumentReferenceExpression("info"),
            "SetType", new CodeExpression[]
            {new CodeTypeOfExpression(TilesetRefClassName)}));            

         CodeTypeDeclaration classTilesetRef = new CodeTypeDeclaration(TilesetRefClassName);
         classTilesetRef.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
         classTilesetRef.BaseTypes.Add(typeof(System.Runtime.Serialization.IObjectReference));
         classTilesetRef.BaseTypes.Add(typeof(System.Runtime.Serialization.ISerializable));
         CodeMemberField fldTilesetRefInst = new CodeMemberField(TilesetClass, "m_Tileset");
         classTilesetRef.Members.Add(fldTilesetRefInst);
         CodeConstructor tilesetRefConstructor = new CodeConstructor();
         classTilesetRef.Members.Add(tilesetRefConstructor);
         tilesetRefConstructor.Parameters.Add(
            new CodeParameterDeclarationExpression(
            typeof(System.Runtime.Serialization.SerializationInfo), "info"));
         tilesetRefConstructor.Parameters.Add(
            new CodeParameterDeclarationExpression(
            typeof(System.Runtime.Serialization.StreamingContext), "context"));
         CodeVariableDeclarationStatement varTilesetIndex =
            new CodeVariableDeclarationStatement(typeof(int), "tilesetIndex",
            new CodeMethodInvokeExpression(
            new CodeArgumentReferenceExpression("info"), "GetInt32",
            new CodeExpression[]
            { new CodePrimitiveExpression(TilesetIndexSerializeName) }));
         tilesetRefConstructor.Statements.Add(varTilesetIndex);

         CodeMemberMethod mthRefGetObjectData = CreateGetObjectDataMethod();
         classTilesetRef.Members.Add(mthRefGetObjectData);
         mthRefGetObjectData.Statements.Add(
            new CodeThrowExceptionStatement(
            new CodeObjectCreateExpression(typeof(NotImplementedException),
            new CodeExpression[]
            { new CodePrimitiveExpression("Unexpected serialization call") })));

         CodeMemberMethod mthGetRealObject = new CodeMemberMethod();
         classTilesetRef.Members.Add(mthGetRealObject);
         mthGetRealObject.Attributes = MemberAttributes.Public | MemberAttributes.Final;
         mthGetRealObject.Name = "GetRealObject";
         mthGetRealObject.Parameters.Add(
            new CodeParameterDeclarationExpression(
            typeof(System.Runtime.Serialization.StreamingContext), "context"));
         mthGetRealObject.ReturnType = new CodeTypeReference(typeof(object));
         mthGetRealObject.Statements.Add(
            new CodeMethodReturnStatement(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), fldTilesetRefInst.Name)));

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

         int nTilesetIndex = 0;
         foreach(System.Data.DataRowView drv in ProjectData.Tileset.DefaultView)
         {
            ProjectDataset.TilesetRow drTileset = (ProjectDataset.TilesetRow)drv.Row;
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
            int nMax = 0;
            if (arTiles.Length > 0)
               nMax = arTiles[arTiles.Length-1].TileValue;
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
                     TileExpr = new CodeObjectCreateExpression(SimpleTileClass, ((CodeObjectCreateExpression)frames[0]).Parameters[1]);
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
                        new CodeTypeReferenceExpression("TileCategoryName"), NameToVariable(drCat.CategorizedTilesetRowParent.Name));
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

            mthGetObjectData.Statements.Add(
               new CodeConditionStatement(
               new CodeBinaryOperatorExpression(
               new CodePropertyReferenceExpression(
               new CodeTypeReferenceExpression(TilesetClass), prpTileset.Name),
               CodeBinaryOperatorType.IdentityEquality,
               new CodeThisReferenceExpression()),
               new CodeStatement[]
               {
                  new CodeExpressionStatement(
                  new CodeMethodInvokeExpression(
                  new CodeArgumentReferenceExpression("info"),
                  "AddValue", new CodeExpression[]
               {
                  new CodePrimitiveExpression(TilesetIndexSerializeName),
                  new CodePrimitiveExpression(nTilesetIndex)
               }))
               }));

            tilesetRefConstructor.Statements.Add(
               new CodeConditionStatement(
               new CodeBinaryOperatorExpression(
               new CodeVariableReferenceExpression(varTilesetIndex.Name),
               CodeBinaryOperatorType.ValueEquality,
               new CodePrimitiveExpression(nTilesetIndex++)),
               new CodeStatement[]
               {
                  new CodeAssignStatement(
                  new CodeFieldReferenceExpression(
                  new CodeThisReferenceExpression(), fldTilesetRefInst.Name),
                  new CodeFieldReferenceExpression(
                  new CodeTypeReferenceExpression(TilesetClass),
                  prpTileset.Name)),
                  new CodeMethodReturnStatement()
               }));
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
         Generator.GenerateCodeFromType(classTilesetRef, txt, GeneratorOptions);
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

      public void GenerateMap(ProjectDataset.MapRow drMap, System.IO.TextWriter txt, System.IO.TextWriter err)
      {
         CodeTypeDeclaration clsMap = new CodeTypeDeclaration(NameToVariable(drMap.Name) + "_Map");
         clsMap.BaseTypes.Add("MapBase");
         clsMap.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
         CodeConstructor constructor = new CodeConstructor();
         constructor.Attributes = MemberAttributes.Final | MemberAttributes.Public;
         constructor.Parameters.Add(new CodeParameterDeclarationExpression("Display", "Disp"));
         CodeArgumentReferenceExpression refDisp = new CodeArgumentReferenceExpression("Disp");
         clsMap.Members.Add(constructor);
         constructor.BaseConstructorArgs.Add(refDisp);
         CodeFieldReferenceExpression fldDisplayRef = new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), MapDisplayField);

         CodeMemberMethod mthDraw = new CodeMemberMethod();
         mthDraw.Attributes = MemberAttributes.Family | MemberAttributes.Override ;
         mthDraw.Name = "Draw";
         clsMap.Members.Add(mthDraw);

         CodeMemberMethod mthExecuteMapRules = new CodeMemberMethod();
         mthExecuteMapRules.Name = "ExecuteRules";
         mthExecuteMapRules.Attributes = MemberAttributes.Override | MemberAttributes.Public;
         clsMap.Members.Add(mthExecuteMapRules);

         CodeMemberMethod mthOnScroll = new CodeMemberMethod();
         mthOnScroll.Name = "Scroll";
         mthOnScroll.Attributes = MemberAttributes.Public | MemberAttributes.Override;
         CodeParameterDeclarationExpression parmPosition = 
            new CodeParameterDeclarationExpression(typeof(System.Drawing.Point), "position");
         mthOnScroll.Parameters.Add(parmPosition);
         CodeArgumentReferenceExpression refPosition =
            new CodeArgumentReferenceExpression(parmPosition.Name);
         CodeExpression maxScroll = new CodeBinaryOperatorExpression(
            new CodePropertyReferenceExpression(
            new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),
            "CurrentView"),"Width"), CodeBinaryOperatorType.Subtract,
            new CodePrimitiveExpression(drMap.ScrollWidth));
         mthOnScroll.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(
            new CodeBinaryOperatorExpression(
            new CodePropertyReferenceExpression(refPosition, "X"), CodeBinaryOperatorType.IdentityInequality,
            new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MinValue")),
            CodeBinaryOperatorType.BooleanAnd,
            new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(
            refPosition, "X"), CodeBinaryOperatorType.LessThan, maxScroll)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(
            refPosition, "X"), maxScroll)));
         mthOnScroll.Statements.Add(new CodeConditionStatement(
            new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(
            refPosition, "X"), CodeBinaryOperatorType.GreaterThan, new CodePrimitiveExpression(0)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(
            refPosition, "X"), new CodePrimitiveExpression(0))));
         maxScroll = new CodeBinaryOperatorExpression(
            new CodePropertyReferenceExpression(
            new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),
            "CurrentView"),"Height"), CodeBinaryOperatorType.Subtract,
            new CodePrimitiveExpression(drMap.ScrollHeight));
         mthOnScroll.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(
            new CodeBinaryOperatorExpression(
            new CodePropertyReferenceExpression(refPosition, "Y"), CodeBinaryOperatorType.IdentityInequality,
            new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MinValue")),
            CodeBinaryOperatorType.BooleanAnd,
            new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(
            refPosition, "Y"), CodeBinaryOperatorType.LessThan, maxScroll)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(
            refPosition, "Y"), maxScroll)));
         mthOnScroll.Statements.Add(new CodeConditionStatement(
            new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(
            refPosition, "Y"), CodeBinaryOperatorType.GreaterThan, new CodePrimitiveExpression(0)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(
            refPosition, "Y"), new CodePrimitiveExpression(0))));
         clsMap.Members.Add(mthOnScroll);

         CodeMemberField fldScrollMarginLeft = new CodeMemberField(typeof(short), "m_ScrollMarginLeft");
         fldScrollMarginLeft.Attributes = MemberAttributes.Private | MemberAttributes.Final;
         fldScrollMarginLeft.InitExpression = new CodePrimitiveExpression(drMap.ScrollMarginLeft);
         clsMap.Members.Add(fldScrollMarginLeft);
         CodeMemberProperty prpScrollMarginLeft = new CodeMemberProperty();
         prpScrollMarginLeft.Name = "ScrollMarginLeft";
         prpScrollMarginLeft.Type = new CodeTypeReference(typeof(short));
         prpScrollMarginLeft.Attributes = MemberAttributes.Public | MemberAttributes.Override;
         prpScrollMarginLeft.HasGet = true;
         prpScrollMarginLeft.HasSet = false;
         prpScrollMarginLeft.GetStatements.Add(new CodeMethodReturnStatement(
            new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldScrollMarginLeft.Name)));
         clsMap.Members.Add(prpScrollMarginLeft);

         CodeMemberField fldScrollMarginTop = new CodeMemberField(typeof(short), "m_ScrollMarginTop");
         fldScrollMarginTop.Attributes = MemberAttributes.Private | MemberAttributes.Final;
         fldScrollMarginTop.InitExpression = new CodePrimitiveExpression(drMap.ScrollMarginTop);
         clsMap.Members.Add(fldScrollMarginTop);
         CodeMemberProperty prpScrollMarginTop = new CodeMemberProperty();
         prpScrollMarginTop.Name = "ScrollMarginTop";
         prpScrollMarginTop.Type = new CodeTypeReference(typeof(short));
         prpScrollMarginTop.Attributes = MemberAttributes.Public | MemberAttributes.Override;
         prpScrollMarginTop.HasGet = true;
         prpScrollMarginTop.HasSet = false;
         prpScrollMarginTop.GetStatements.Add(new CodeMethodReturnStatement(
            new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldScrollMarginTop.Name)));
         clsMap.Members.Add(prpScrollMarginTop);

         CodeMemberField fldScrollMarginRight = new CodeMemberField(typeof(short), "m_ScrollMarginRight");
         fldScrollMarginRight.Attributes = MemberAttributes.Private | MemberAttributes.Final;
         fldScrollMarginRight.InitExpression = new CodePrimitiveExpression(drMap.ScrollMarginRight);
         clsMap.Members.Add(fldScrollMarginRight);
         CodeMemberProperty prpScrollMarginRight = new CodeMemberProperty();
         prpScrollMarginRight.Name = "ScrollMarginRight";
         prpScrollMarginRight.Type = new CodeTypeReference(typeof(short));
         prpScrollMarginRight.Attributes = MemberAttributes.Public | MemberAttributes.Override;
         prpScrollMarginRight.HasGet = true;
         prpScrollMarginRight.HasSet = false;
         prpScrollMarginRight.GetStatements.Add(new CodeMethodReturnStatement(
            new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldScrollMarginRight.Name)));
         clsMap.Members.Add(prpScrollMarginRight);

         CodeMemberField fldScrollMarginBottom = new CodeMemberField(typeof(short), "m_ScrollMarginBottom");
         fldScrollMarginBottom.Attributes = MemberAttributes.Private | MemberAttributes.Final;
         fldScrollMarginBottom.InitExpression = new CodePrimitiveExpression(drMap.ScrollMarginBottom);
         clsMap.Members.Add(fldScrollMarginBottom);
         CodeMemberProperty prpScrollMarginBottom = new CodeMemberProperty();
         prpScrollMarginBottom.Name = "ScrollMarginBottom";
         prpScrollMarginBottom.Type = new CodeTypeReference(typeof(short));
         prpScrollMarginBottom.Attributes = MemberAttributes.Public | MemberAttributes.Override;
         prpScrollMarginBottom.HasGet = true;
         prpScrollMarginBottom.HasSet = false;
         prpScrollMarginBottom.GetStatements.Add(new CodeMethodReturnStatement(
            new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldScrollMarginBottom.Name)));
         clsMap.Members.Add(prpScrollMarginBottom);

         if ((drMap.ViewWidth > 0) && (drMap.ViewHeight > 0))
         {
            CodeMemberField fldView = new CodeMemberField(typeof(System.Drawing.Rectangle), "m_View");
            fldView.Attributes = MemberAttributes.Private | MemberAttributes.Final;
            fldView.InitExpression = new CodeObjectCreateExpression(typeof(System.Drawing.Rectangle),
               new CodePrimitiveExpression(drMap.ViewLeft),
               new CodePrimitiveExpression(drMap.ViewTop),
               new CodePrimitiveExpression(drMap.ViewWidth),
               new CodePrimitiveExpression(drMap.ViewHeight));
            clsMap.Members.Add(fldView);
            CodeMemberProperty prpView = new CodeMemberProperty();
            prpView.Name = "TotalView";
            prpView.Type = new CodeTypeReference(typeof(System.Drawing.Rectangle));
            prpView.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            prpView.HasGet = true;
            prpView.HasSet = true;
            prpView.GetStatements.Add(new CodeMethodReturnStatement(
               new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldView.Name)));
            prpView.SetStatements.Add(new CodeCommentStatement("First, assert valid rectangle in debug mode by calling base implementation"));
            prpView.SetStatements.Add(new CodeAssignStatement(
               new CodeFieldReferenceExpression(
               new CodeBaseReferenceExpression(), prpView.Name),
               new CodePropertySetValueReferenceExpression()));
            prpView.SetStatements.Add(new CodeAssignStatement(
               new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldView.Name),
               new CodeMethodInvokeExpression(
               new CodeTypeReferenceExpression(typeof(System.Drawing.Rectangle)), "Intersect",
               new CodeFieldReferenceExpression(
               new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
               MapDisplayField), "DisplayRectangle"),
               new CodePropertySetValueReferenceExpression())));
            clsMap.Members.Add(prpView);
         }

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

            CodeTypeDeclaration clsLayer = new CodeTypeDeclaration(NameToVariable(drLayer.Name)+ "_Lyr");
            clsLayer.BaseTypes.Add(lyrTyp);
            clsLayer.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
            lyrTyp = new CodeTypeReference(clsLayer.Name);
            CodeConstructor lyrConstructor = new CodeConstructor();
            clsLayer.Members.Add(lyrConstructor);
            lyrConstructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            lyrConstructor.Parameters.AddRange(new CodeParameterDeclarationExpression[]
               {
                  new CodeParameterDeclarationExpression("Tileset", "Tileset"),
                  new CodeParameterDeclarationExpression(clsMap.Name, LayerParentArg),
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
            CodeObjectCreateExpression createLayerSprites = new CodeObjectCreateExpression(SpriteCollectionClassName);
            lyrConstructor.BaseConstructorArgs.AddRange(new CodeExpression[]
               {
                  new CodeArgumentReferenceExpression("Tileset"),
                  new CodeArgumentReferenceExpression(LayerParentArg),
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
            fldLayer.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            clsMap.Members.Add(fldLayer);

            constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(
               new CodeThisReferenceExpression(), fldLayer.Name),
               new CodeObjectCreateExpression(lyrTyp,
               new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("Tileset"),
               NameToVariable(drLayer.Tileset)), new CodeThisReferenceExpression(),
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

            if ((drLayer.ScrollRateX==0) && (drLayer.ScrollRateY==0))
               mthOnScroll.Statements.Add(new CodeCommentStatement(drLayer.Name + " does not participate in automatic scrolling"));
            else
               mthOnScroll.Statements.Add(new CodeMethodInvokeExpression(
                  new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                  fldLayer.Name), "Move", new CodeArgumentReferenceExpression(
                  "position")));

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
               "Draw"));
            mthDraw.Statements.Add(new CodeMethodInvokeExpression(
               new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldLayer.Name),
               "ClearInjections"));

            CodeMemberMethod mthInject = new CodeMemberMethod();
            mthInject.Name = "InjectSprites";
            mthInject.Attributes = MemberAttributes.Override | MemberAttributes.Public;
            mthInject.ReturnType = new CodeTypeReference(typeof(void));
            clsLayer.Members.Add(mthInject);

            CodeMemberField fldLayerParent = new CodeMemberField(clsMap.Name, LayerParentField);
            fldLayerParent.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            clsLayer.Members.Add(fldLayerParent);

            lyrConstructor.Statements.Add(new CodeAssignStatement(
               new CodeFieldReferenceExpression(
               new CodeThisReferenceExpression(), fldLayerParent.Name),
               new CodeArgumentReferenceExpression(LayerParentArg)));

            ProjectDataset.SpritePlanRow[] plans = ProjectData.GetSortedSpritePlans(drLayer);
            ProjectDataset.SpriteRow[] sprites = ProjectData.GetSortedSpriteRows(drLayer);
            CodeMemberMethod mthExecuteLayerRules = new CodeMemberMethod();

            if ((plans.Length > 0) || (sprites.Length > 0))
            {
               mthExecuteLayerRules.Attributes = MemberAttributes.Public | MemberAttributes.Final;
               mthExecuteLayerRules.Name = "ExecuteRules";
               clsLayer.Members.Add(mthExecuteLayerRules);
               mthExecuteMapRules.Statements.Add(
                  new CodeMethodInvokeExpression(
                  new CodeFieldReferenceExpression(
                  new CodeThisReferenceExpression(), fldLayer.Name), "ExecuteRules"));
            }

            foreach(ProjectDataset.SpritePlanRow drPlan in plans)
            {
               CodeTypeDeclaration clsPlan = new CodeTypeDeclaration(NameToVariable(drPlan.Name));
               clsLayer.Members.Add(clsPlan);
               clsPlan.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
               clsPlan.BaseTypes.Add(PlanBaseClassName);

               clsPlan.Members.Add(new CodeMemberField(clsLayer.Name, SpritePlanParentField));

               CodeConstructor planConstructor = new CodeConstructor();
               clsPlan.Members.Add(planConstructor);
               planConstructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
               planConstructor.Parameters.Add(new CodeParameterDeclarationExpression(
                  clsLayer.Name, SpritePlanParentArg));
               planConstructor.Statements.Add(new CodeAssignStatement(
                  new CodeFieldReferenceExpression(
                  new CodeThisReferenceExpression(), SpritePlanParentField),
                  new CodeArgumentReferenceExpression(SpritePlanParentArg)));

               CodeMemberProperty prpPlanParent = new CodeMemberProperty();
               prpPlanParent.Name = SpritePlanParentProperty;
               prpPlanParent.Attributes = MemberAttributes.Public | MemberAttributes.Override;
               prpPlanParent.HasGet = true;
               prpPlanParent.HasSet = false;
               prpPlanParent.Type = new CodeTypeReference(LayerBaseClassName);
               prpPlanParent.GetStatements.Add(new CodeMethodReturnStatement(
                  new CodeFieldReferenceExpression(
                  new CodeThisReferenceExpression(), SpritePlanParentField)));
               clsPlan.Members.Add(prpPlanParent);

               CodeMemberField fldPlan = new CodeMemberField(clsPlan.Name, "m_" + NameToVariable(drPlan.Name));
               fldPlan.Attributes = MemberAttributes.Public | MemberAttributes.Final;
               clsLayer.Members.Add(fldPlan);
               lyrConstructor.Statements.Add(new CodeAssignStatement(
                  new CodeFieldReferenceExpression(
                  new CodeThisReferenceExpression(), fldPlan.Name),
                  new CodeObjectCreateExpression(clsPlan.Name,
                  new CodeThisReferenceExpression())));
               
               ProjectDataset.CoordinateRow[] drCoords = ProjectData.GetSortedCoordinates(drPlan);
               if (drCoords.Length == 2)
               {
                  CodeMemberField fldRect = new CodeMemberField(typeof(System.Drawing.Rectangle), "m_PlanRectangle");
                  fldRect.Attributes = MemberAttributes.Private | MemberAttributes.Final;
                  int minX = drCoords[0].X;
                  int maxX;
                  if (drCoords[1].X >= minX)
                     maxX = drCoords[1].X;
                  else
                  {
                     maxX = minX;
                     minX = drCoords[1].X;
                  }
                  int minY = drCoords[0].Y;
                  int maxY;
                  if (drCoords[1].Y >= minY)
                     maxY = drCoords[1].Y;
                  else
                  {
                     maxY = minY;
                     minY = drCoords[1].Y;
                  }
                  // Rectangle size is one pixel smaller than second coordinate
                  // In order to allow snap-to-tile feature to create rectangles
                  // that align to tile boundaries.
                  fldRect.InitExpression = new CodeObjectCreateExpression(typeof(System.Drawing.Rectangle),
                     new CodePrimitiveExpression(minX),
                     new CodePrimitiveExpression(minY),
                     new CodePrimitiveExpression(maxX - minX),
                     new CodePrimitiveExpression(maxY - minY));
                  clsPlan.Members.Add(fldRect);

                  CodeMemberProperty prpRect = new CodeMemberProperty();
                  prpRect.Type = new CodeTypeReference(typeof(System.Drawing.Rectangle));
                  prpRect.Name = "PlanRectangle";
                  prpRect.Attributes = MemberAttributes.Public | MemberAttributes.Override;
                  prpRect.HasGet = true;
                  prpRect.HasSet = false;
                  prpRect.GetStatements.Add(new CodeMethodReturnStatement(
                     new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fldRect.Name)));
                  clsPlan.Members.Add(prpRect);
               }
               CodeMemberField fldCoords = new CodeMemberField(new CodeTypeReference(CoordinateTypeName, 1), "m_Coords");
               fldCoords.Attributes = MemberAttributes.Private | MemberAttributes.Final;
               fldCoords.InitExpression = new CodeArrayCreateExpression();
               ((CodeArrayCreateExpression)fldCoords.InitExpression).CreateType = new CodeTypeReference(CoordinateTypeName);
               foreach (ProjectDataset.CoordinateRow drCoord in drCoords)
               {  
                  ((CodeArrayCreateExpression)fldCoords.InitExpression).Initializers.Add(
                     new CodeObjectCreateExpression(CoordinateTypeName,
                     new CodePrimitiveExpression(drCoord.X),
                     new CodePrimitiveExpression(drCoord.Y),
                     new CodePrimitiveExpression(drCoord.Weight)));
               }
               clsPlan.Members.Add(fldCoords);

               CodeMemberProperty prpCoords = new CodeMemberProperty();
               prpCoords.Name = "Coordinates";
               prpCoords.Attributes = MemberAttributes.Family | MemberAttributes.Override;
               prpCoords.Type = new CodeTypeReference(CoordinateTypeName,1);
               prpCoords.HasGet = true;
               prpCoords.HasSet = false;
               prpCoords.GetStatements.Add(new CodeMethodReturnStatement(
                  new CodeFieldReferenceExpression(
                  new CodeThisReferenceExpression(), fldCoords.Name)));
               clsPlan.Members.Add(prpCoords);

               if (GenerateLevel > CodeLevel.ExcludeRules)
               {
                  ProjectDataset.PlanRuleRow[] rules = ProjectData.GetSortedPlanRules(drPlan, false);
                  if (rules.Length > 0)
                  {
                     mthExecuteLayerRules.Statements.Add(
                        new CodeMethodInvokeExpression(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), fldPlan.Name),
                        "ExecuteRules"));

                     RuleContent[] ruleArray = new RuleContent[rules.Length];
                     for (int i = 0; i < rules.Length; i++)
                        ruleArray[i] = new RuleContent(rules[i]);
                     CodeMemberMethod mthExecuteRules = new CodeMemberMethod();
                     mthExecuteRules.Name = "ExecuteRules";
                     mthExecuteRules.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                     try
                     {
                        GenerateRules(ruleArray, mthExecuteRules);
                        clsPlan.Members.Add(mthExecuteRules);
                     }
                     catch (System.ApplicationException ex)
                     {
                        err.WriteLine("Error generating plan \"" + drPlan.Name +"\" on layer \"" + drLayer.Name + "\" of map \"" + drMap.Name + "\": " +  ex.Message);
                     }
                  }
               }
            }

            CodeTypeDeclaration clsLayerSpriteCategories = new CodeTypeDeclaration(LayerSpriteCategoriesClassName);
            clsLayer.Members.Add(clsLayerSpriteCategories);
            clsLayerSpriteCategories.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));

            CodeMemberField fldSprCatLayer = new CodeMemberField(clsLayer.Name, "m_layer");
            fldSprCatLayer.Attributes = MemberAttributes.Private;
            clsLayerSpriteCategories.Members.Add(fldSprCatLayer);

            clsLayerSpriteCategories.BaseTypes.Add(LayerSpriteCategoriesBaseClassName);

            CodeConstructor constructLayerSpriteCategories = new CodeConstructor();
            clsLayerSpriteCategories.Members.Add(constructLayerSpriteCategories);
            constructLayerSpriteCategories.Attributes = MemberAttributes.Public;
            constructLayerSpriteCategories.Parameters.Add(new CodeParameterDeclarationExpression(clsLayer.Name, "layer"));
            constructLayerSpriteCategories.Statements.Add(
               new CodeAssignStatement(
               new CodeFieldReferenceExpression(
               new CodeThisReferenceExpression(), "m_layer"),
               new CodeArgumentReferenceExpression("layer")));

            System.Collections.Hashtable htCategories = new System.Collections.Hashtable();
            if (sprites.Length > 0)
            {
               CodeMemberMethod mthClearProcessedFlag = new CodeMemberMethod();
               mthClearProcessedFlag.Attributes = MemberAttributes.Private;
               mthClearProcessedFlag.Name = "ClearProcessedFlag";
               clsLayer.Members.Add(mthClearProcessedFlag);
               mthExecuteLayerRules.Statements.Add(new CodeMethodInvokeExpression(
                  new CodeThisReferenceExpression(), mthClearProcessedFlag.Name));

               foreach(ProjectDataset.SpriteRow sprite in sprites)
               {
                  System.Collections.ArrayList SpriteCreateParams = new System.Collections.ArrayList();
                  ProjectDataset.SpriteStateRow drState = sprite.SpriteStateRowParent;
                  ProjectDataset.SpriteDefinitionRow drDef = drState.SpriteDefinitionRow;
                  SpriteCreateParams.Add(new CodeThisReferenceExpression());
                  SpriteCreateParams.Add(new CodePrimitiveExpression(sprite.X));
                  SpriteCreateParams.Add(new CodePrimitiveExpression(sprite.Y));
                  SpriteCreateParams.Add(new CodePrimitiveExpression(sprite.DX));
                  SpriteCreateParams.Add(new CodePrimitiveExpression(sprite.DY));
                  SpriteCreateParams.Add(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("Sprites." + NameToVariable(drDef.Name) + ".State"), NameToVariable(sprite.StateName)));
                  SpriteCreateParams.Add(new CodePrimitiveExpression(sprite.CurrentFrame));
                  SpriteCreateParams.Add(new CodePrimitiveExpression(sprite.Active));
                  SpriteCreateParams.Add(new CodeFieldReferenceExpression(
                     new CodeArgumentReferenceExpression(LayerParentArg), MapDisplayField));
                  if (ProjectData.GetSolidity(sprite.Solidity) != null)
                     SpriteCreateParams.Add(new CodeFieldReferenceExpression(
                        new CodeTypeReferenceExpression("Solidity"), NameToVariable(sprite.Solidity)));
                  else
                     SpriteCreateParams.Add(new CodeFieldReferenceExpression(
                        new CodeTypeReferenceExpression("Solidity"), UndefinedSolidityProperty));
                  SpriteCreateParams.Add(new CodePrimitiveExpression(sprite.Color));
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
                     ((System.Collections.ArrayList)htCategories[drCat.Name]).Add(
                        new CodeFieldReferenceExpression(
                        new CodeArgumentReferenceExpression(constructLayerSpriteCategories.Parameters[0].Name),
                        fldrefSpr.FieldName));
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
                        new CodeMethodInvokeExpression(fldrefSpr, "GetCurrentFramesetFrames"),
                        new CodePropertyReferenceExpression(fldrefSpr, "color")
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

                  mthClearProcessedFlag.Statements.Add(new CodeAssignStatement(
                     new CodeFieldReferenceExpression(
                     new CodeFieldReferenceExpression(
                     new CodeThisReferenceExpression(), "m_" + NameToVariable(sprite.Name)),
                     SpriteProcessedRulesField),
                     new CodePrimitiveExpression(false)));

                  mthExecuteLayerRules.Statements.Add(new CodeConditionStatement(new CodeFieldReferenceExpression(
                     new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                     "m_" + NameToVariable(sprite.Name)), SpriteIsActiveRef),
                     new CodeStatement[]
                     {
                        new CodeExpressionStatement(
                        new CodeMethodInvokeExpression(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(),
                        "m_" + NameToVariable(sprite.Name)), "ProcessRules"))
                     }));
               }

               if (htCategories.Count > 0)
               {
                  foreach(System.Collections.DictionaryEntry de in htCategories)
                  {
                     clsLayerSpriteCategories.Members.Add(new CodeMemberField(
                        SpriteCollectionClassName, "m_" + NameToVariable(de.Key.ToString())));
                     constructLayerSpriteCategories.Statements.Add(new CodeAssignStatement(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), "m_" + NameToVariable(de.Key.ToString())),
                        new CodeObjectCreateExpression(SpriteCollectionClassName,
                        (CodeFieldReferenceExpression[])((System.Collections.ArrayList)de.Value).ToArray(
                        typeof(CodeFieldReferenceExpression)))));

                     CodeMemberProperty prpLayerSpriteCategory = new CodeMemberProperty();
                     prpLayerSpriteCategory.Type = new CodeTypeReference(SpriteCollectionClassName);
                     prpLayerSpriteCategory.Attributes = MemberAttributes.Public | MemberAttributes.Override;
                     prpLayerSpriteCategory.HasGet = true;
                     prpLayerSpriteCategory.HasSet = false;
                     prpLayerSpriteCategory.Name = NameToVariable(de.Key.ToString());
                     prpLayerSpriteCategory.GetStatements.Add(
                        new CodeMethodReturnStatement(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(),
                        "m_" + NameToVariable(de.Key.ToString()))));
                     clsLayerSpriteCategories.Members.Add(prpLayerSpriteCategory);
                  }
               }
            }

            // Initialize plan parameters after other objects are initialized because
            // the plan parameters might refer to them.
            foreach(ProjectDataset.SpritePlanRow drPlan in plans)
            {
               ProjectDataset.PlanParameterValueRow[] planParams = drPlan.GetPlanParameterValueRows();
               if (planParams.Length > 0)
               {
                  foreach(ProjectDataset.PlanParameterValueRow planParam in planParams)
                  {
                     lyrConstructor.Statements.Add(new CodeAssignStatement(
                        new CodeFieldReferenceExpression(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(),
                        "m_" + NameToVariable(drPlan.Name)),
                        planParam.Name), new CodeSnippetExpression(planParam.Value)));
                  }
               }
            }

            lyrConstructor.Statements.Add(new CodeAssignStatement(
               new CodeFieldReferenceExpression(
               new CodeThisReferenceExpression(), "m_Sprites"),
               createLayerSprites));

            lyrConstructor.Statements.Add(
               new CodeAssignStatement(
               new CodeFieldReferenceExpression(
               new CodeThisReferenceExpression(), SpriteCategoriesFieldName),
               new CodeObjectCreateExpression(clsLayerSpriteCategories.Name, new CodeThisReferenceExpression())));
         }
         // Flush forces ScissorRectangle to apply to this batch only.
         mthDraw.Statements.Add(new CodeMethodInvokeExpression(
            new CodePropertyReferenceExpression(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), MapDisplayField),
            "Sprite"), "Flush"));
         Generator.GenerateCodeFromType(clsMap, txt, GeneratorOptions);
      }

      private void GenerateRules(RuleContent[] rules, CodeMemberMethod mthExecuteRules)
      {
         System.Collections.Stack stkNestedConditions = new System.Collections.Stack();
         foreach(RuleContent rule in rules)
         {
            if (rule.Function.Length == 0)
               continue;
            if (String.Compare(rule.Type , "End", true) != 0)
            {
               CodeExpression invokeResult;
               CodeBinaryOperatorType op = CodeBinaryOperatorType.Modulus;
               switch(rule.Function)
               {
                  case "+":
                     op = CodeBinaryOperatorType.Add;
                     break;
                  case "-":
                     op = CodeBinaryOperatorType.Subtract;
                     break;
                  case "<":
                  case "!>=":
                     op = CodeBinaryOperatorType.LessThan;
                     break;
                  case "<=":
                  case "!>":
                     op = CodeBinaryOperatorType.LessThanOrEqual;
                     break;
                  case "==":
                  case "!!=":
                     op = CodeBinaryOperatorType.IdentityEquality;
                     break;
                  case ">=":
                  case "!<":
                     op = CodeBinaryOperatorType.GreaterThanOrEqual;
                     break;
                  case ">":
                  case "!<=":
                     op = CodeBinaryOperatorType.GreaterThan;
                     break;
                  case "!=":
                  case "!==":
                     op = CodeBinaryOperatorType.IdentityInequality;
                     break;
               }
               // If function is an operator
               if (op != CodeBinaryOperatorType.Modulus)
               {
                  invokeResult = new CodeBinaryOperatorExpression(
                     new CodeSnippetExpression(rule.Parameter1),
                     op, new CodeSnippetExpression(rule.Parameter2));
               }
               else if (String.Compare(rule.Function, "=", true) == 0)
                  invokeResult = new CodeSnippetExpression(rule.Parameter1);
               else
               {
                  CodeMethodInvokeExpression invoke;
                  CodeExpression target;
                  string functionName;
                  if (rule.Function.IndexOf(".") >= 0)
                  {
                     int dotPos = rule.Function.LastIndexOf('.');
                     string typeName = rule.Function.Substring(0,dotPos);
                     if (typeName.StartsWith("!"))
                        typeName = typeName.Substring(1);
                     target = new CodeTypeReferenceExpression(typeName);
                     functionName = rule.Function.Substring(dotPos+1);
                  }
                  else
                  {
                     target = new CodeThisReferenceExpression();
                     if (rule.Function.StartsWith("!"))
                        functionName = rule.Function.Substring(1);
                     else
                        functionName = rule.Function;
                  }
                  if (rule.Function.StartsWith("!"))
                  {
                     invokeResult = new CodeBinaryOperatorExpression(
                        invoke = new CodeMethodInvokeExpression(
                        target, functionName),
                        CodeBinaryOperatorType.ValueEquality,
                        new CodePrimitiveExpression(false));
                  }
                  else
                     invokeResult = invoke = new CodeMethodInvokeExpression(target, functionName);

                  if (!((rule.Parameter1 == null) || (rule.Parameter1.Length == 0)))
                  {
                     invoke.Parameters.Add(new CodeSnippetExpression(rule.Parameter1));
                     if (!((rule.Parameter2 == null) || (rule.Parameter2.Length == 0)))
                     {
                        invoke.Parameters.Add(new CodeSnippetExpression(rule.Parameter2));
                        if (!((rule.Parameter3 == null) || (rule.Parameter3.Length == 0)))
                        {
                           invoke.Parameters.Add(new CodeSnippetExpression(rule.Parameter3));
                        }
                     }
                  }
               }

               if (String.Compare(rule.Type,"if",true) == 0)
               {
                  CodeConditionStatement cond = new CodeConditionStatement();
                  cond.Condition = invokeResult;
                  stkNestedConditions.Push(new ConditionStackElement(cond, rule));
               }
               else if (String.Compare(rule.Type,"and",true) == 0)
               {
                  if (stkNestedConditions.Count > 0)
                  {
                     ConditionStackElement prev = (ConditionStackElement)stkNestedConditions.Peek();
                     prev.Condition = new CodeBinaryOperatorExpression(
                        prev.Condition, CodeBinaryOperatorType.BooleanAnd, invokeResult);
                  }
               }
               else if (String.Compare(rule.Type,"or",true) == 0)
               {
                  if (stkNestedConditions.Count > 0)
                  {
                     ConditionStackElement prev = (ConditionStackElement)stkNestedConditions.Peek();
                     prev.Condition = new CodeBinaryOperatorExpression(
                        prev.Condition, CodeBinaryOperatorType.BooleanOr, invokeResult);
                  }
               }
               else if (String.Compare(rule.Type,"ElseIf", true) == 0)
               {
                  if (stkNestedConditions.Count > 0)
                  {
                     ConditionStackElement elem = (ConditionStackElement)stkNestedConditions.Peek();
                     if (elem.Statement is CodeConditionStatement)
                     {
                        elem.NextPhase();
                        CodeConditionStatement cond = new CodeConditionStatement();
                        cond.Condition = invokeResult;
                        stkNestedConditions.Push(new ConditionStackElement(cond, rule));
                     }
                  }
               }
               else if (String.Compare(rule.Type, "while", true) == 0)
               {
                  CodeIterationStatement cond = new CodeIterationStatement(new CodeSnippetStatement(""), invokeResult, new CodeSnippetStatement(""));
                  stkNestedConditions.Push(new ConditionStackElement(cond, rule));
               }
               else
               {
                  CodeStatement stmtRule = null;
                  if (!((rule.ResultParameter == null) || (rule.ResultParameter.Length == 0)))
                  {
                     stmtRule = new CodeAssignStatement(
                        new CodeSnippetExpression(rule.ResultParameter),
                        invokeResult);
                  }
                  else
                  {
                     stmtRule = new CodeExpressionStatement(invokeResult);
                  }
                  if (stkNestedConditions.Count > 0)
                  {
                     ConditionStackElement prev = (ConditionStackElement)stkNestedConditions.Peek();
                     if ((String.Compare(rule.Type, "Else", true) == 0) && (prev.Statement is CodeConditionStatement))
                        prev.NextPhase();
                     prev.ChildStatements.Add(new CodeCommentStatement(rule.Name));
                     prev.ChildStatements.Add(stmtRule);

                     if (rule.EndIf)
                     {
                        ConditionStackElement popVal = ((ConditionStackElement)stkNestedConditions.Pop());
                        if (stkNestedConditions.Count > 0)
                        {
                           ((ConditionStackElement)stkNestedConditions.Peek()).ChildStatements.Add(new CodeCommentStatement(popVal.RuleName));
                           ((ConditionStackElement)stkNestedConditions.Peek()).ChildStatements.Add(popVal.Statement);
                        }
                        else
                        {
                           mthExecuteRules.Statements.Add(new CodeCommentStatement(popVal.RuleName));
                           mthExecuteRules.Statements.Add(popVal.Statement);
                        }
                     }
                  }
                  else
                  {
                     mthExecuteRules.Statements.Add(new CodeCommentStatement(rule.Name));
                     mthExecuteRules.Statements.Add(stmtRule);
                  }
               }
            }
            else
            {
               ConditionStackElement popVal = ((ConditionStackElement)stkNestedConditions.Pop());
               if (stkNestedConditions.Count > 0)
               {
                  ((ConditionStackElement)stkNestedConditions.Peek()).ChildStatements.Add(new CodeCommentStatement(popVal.RuleName));
                  ((ConditionStackElement)stkNestedConditions.Peek()).ChildStatements.Add(popVal.Statement);
               }
               else
               {
                  mthExecuteRules.Statements.Add(new CodeCommentStatement(popVal.RuleName));
                  mthExecuteRules.Statements.Add(popVal.Statement);
               }
            }
         }
         while(stkNestedConditions.Count > 0)
         {
            ConditionStackElement popVal = ((ConditionStackElement)stkNestedConditions.Pop());
            if (stkNestedConditions.Count > 0)
            {
               ((ConditionStackElement)stkNestedConditions.Peek()).ChildStatements.Add(new CodeCommentStatement(popVal.RuleName));
               ((ConditionStackElement)stkNestedConditions.Peek()).ChildStatements.Add(popVal.Statement);
            }
            else
            {
               mthExecuteRules.Statements.Add(new CodeCommentStatement(popVal.RuleName));
               mthExecuteRules.Statements.Add(popVal.Statement);
            }
         }
      }

      public void GenerateSpriteDef(ProjectDataset.SpriteDefinitionRow drSpriteDef, System.IO.TextWriter txt, System.IO.TextWriter err)
      {
         CodeNamespace nsSprites = new CodeNamespace(SpritesNamespace);
         CodeTypeDeclaration clsSpriteDef = new CodeTypeDeclaration(NameToVariable(drSpriteDef.Name));
         nsSprites.Types.Add(clsSpriteDef);
         clsSpriteDef.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));

         clsSpriteDef.BaseTypes.Add(SpriteBaseClass);
         CodeConstructor constructor = new CodeConstructor();
         constructor.Attributes = MemberAttributes.Public;
         constructor.Parameters.AddRange(new CodeParameterDeclarationExpression[]
            {
               new CodeParameterDeclarationExpression(LayerBaseClassName, "layer"),
               new CodeParameterDeclarationExpression(typeof(double), "x"),
               new CodeParameterDeclarationExpression(typeof(double), "y"),
               new CodeParameterDeclarationExpression(typeof(double), "dx"),
               new CodeParameterDeclarationExpression(typeof(double), "dy"),
               new CodeParameterDeclarationExpression(
               new CodeTypeReference("Sprites." + NameToVariable(drSpriteDef.Name) + ".State"), "state"),
               new CodeParameterDeclarationExpression(typeof(int), "frame"),
               new CodeParameterDeclarationExpression(typeof(bool), "active"),
               new CodeParameterDeclarationExpression("Display", "disp"),
               new CodeParameterDeclarationExpression("Solidity", "solidity"),
               new CodeParameterDeclarationExpression(typeof(int), "color")
            });
         constructor.BaseConstructorArgs.AddRange(new CodeExpression[]
            {
               new CodeArgumentReferenceExpression("layer"),
               new CodeArgumentReferenceExpression("x"),
               new CodeArgumentReferenceExpression("y"),
               new CodeArgumentReferenceExpression("dx"),
               new CodeArgumentReferenceExpression("dy"),
               new CodeCastExpression(typeof(int),
               new CodeArgumentReferenceExpression("state")),
               new CodeArgumentReferenceExpression("frame"),
               new CodeArgumentReferenceExpression("active"),
               new CodeArgumentReferenceExpression("solidity"),
               new CodeArgumentReferenceExpression("color")
            });

         CodeMemberMethod mthClearParams = new CodeMemberMethod();
         mthClearParams.Name = "ClearParameters";
         mthClearParams.Attributes = MemberAttributes.Override | MemberAttributes.Public;
         clsSpriteDef.Members.Add(mthClearParams);

         foreach(ProjectDataset.SpriteParameterRow drParam in ProjectData.GetSortedSpriteParameters(drSpriteDef))
         {
            CodeMemberField fldParam = new CodeMemberField(typeof(int), NameToVariable(drParam.Name));
            fldParam.Attributes = MemberAttributes.Public;
            clsSpriteDef.Members.Add(fldParam);
            CodeFieldReferenceExpression refParam = new CodeFieldReferenceExpression(
               new CodeThisReferenceExpression(), NameToVariable(drParam.Name));
            constructor.Parameters.Add(new CodeParameterDeclarationExpression(
               typeof(int), NameToVariable(drParam.Name)));
            constructor.Statements.Add(new CodeAssignStatement(refParam,               
               new CodeArgumentReferenceExpression(NameToVariable(drParam.Name))));
            mthClearParams.Statements.Add(new CodeAssignStatement(refParam, new CodePrimitiveExpression(0)));
         }
         clsSpriteDef.Members.Add(constructor);

         CodeMemberMethod initStates = new CodeMemberMethod();
         initStates.Name = "InitializeStates";
         initStates.Attributes = MemberAttributes.Static | MemberAttributes.Private;
         initStates.Parameters.Add(new CodeParameterDeclarationExpression("Display", "disp"));
         clsSpriteDef.Members.Add(initStates);

         constructor.Statements.Add(new CodeConditionStatement(
            new CodeBinaryOperatorExpression(
            new CodeFieldReferenceExpression(
            new CodeTypeReferenceExpression(clsSpriteDef.Name), SpriteStateField),
            CodeBinaryOperatorType.IdentityEquality,
            new CodePrimitiveExpression(null)), new CodeStatement[]
            {
               new CodeExpressionStatement(
               new CodeMethodInvokeExpression(
               new CodeTypeReferenceExpression(clsSpriteDef.Name),
               initStates.Name, new CodeArgumentReferenceExpression("disp")))
            }));

         ProjectDataset.SpriteStateRow[] stateRows = ProjectData.GetSortedSpriteStates(drSpriteDef);

         CodeTypeDeclaration enumStates = new CodeTypeDeclaration(SpriteStateEnumName);
         initStates.Statements.Add(new CodeAssignStatement(
            new CodeFieldReferenceExpression(
            new CodeTypeReferenceExpression(clsSpriteDef.Name), SpriteStateField),
            new CodeArrayCreateExpression(
            new CodeTypeReference(SpriteStateClassName, 1),
            new CodePrimitiveExpression(stateRows.Length))));

         clsSpriteDef.Members.Add(enumStates);
         enumStates.IsEnum = true;
         
         CachedSpriteDef sprCached = new CachedSpriteDef(drSpriteDef, null);

         CodeVariableDeclarationStatement varFrameset = new CodeVariableDeclarationStatement(
            FramesetClass, "framesetRef");
         initStates.Statements.Add(varFrameset);

         CodeVariableDeclarationStatement varBounds = new CodeVariableDeclarationStatement(
            typeof(System.Drawing.Rectangle), "localBounds");
         initStates.Statements.Add(varBounds);

         CodeObjectCreateExpression createRect = null;
         CodeMethodInvokeExpression getFrameset = null;

         foreach(ProjectDataset.SpriteStateRow drState in stateRows)
         {
            if ((getFrameset == null) ||
               (!((CodePrimitiveExpression)getFrameset.Parameters[0]).Value.Equals(drState.FramesetName)))
            {
               getFrameset = new CodeMethodInvokeExpression(
                  new CodeTypeReferenceExpression(FramesetClass),
                  GetFramesetMethodName, new CodePrimitiveExpression(drState.FramesetName),
                  new CodeArgumentReferenceExpression("disp"));
               initStates.Statements.Add(new CodeAssignStatement(
                  new CodeVariableReferenceExpression(varFrameset.Name), getFrameset));
            }

            StateInfo sinf = sprCached[drState.Name];

            if ((createRect == null) ||
               (!((CodePrimitiveExpression)createRect.Parameters[0]).Value.Equals(sinf.Bounds.X)) ||
               (!((CodePrimitiveExpression)createRect.Parameters[1]).Value.Equals(sinf.Bounds.Y)) ||
               (!((CodePrimitiveExpression)createRect.Parameters[2]).Value.Equals(sinf.Bounds.Width)) ||
               (!((CodePrimitiveExpression)createRect.Parameters[3]).Value.Equals(sinf.Bounds.Height)))
            {
               createRect = new CodeObjectCreateExpression(typeof(System.Drawing.Rectangle),
                  new CodePrimitiveExpression(sinf.Bounds.X),
                  new CodePrimitiveExpression(sinf.Bounds.Y),
                  new CodePrimitiveExpression(sinf.Bounds.Width),
                  new CodePrimitiveExpression(sinf.Bounds.Height));
               
               initStates.Statements.Add(new CodeAssignStatement(
                  new CodeVariableReferenceExpression(varBounds.Name), createRect));
            }

            enumStates.Members.Add(new CodeMemberField(typeof(int), NameToVariable(drState.Name)));
            int nAccumulatedDuration = 0;
            System.Collections.ArrayList stateParams = new System.Collections.ArrayList();
            stateParams.Add(new CodePrimitiveExpression(drState.SolidWidth));
            stateParams.Add(new CodePrimitiveExpression(drState.SolidHeight));
            stateParams.Add(new CodeVariableReferenceExpression(varFrameset.Name));
            stateParams.Add(new CodeVariableReferenceExpression(varBounds.Name));
            System.Collections.ArrayList subFrames = null;
            System.Collections.ArrayList alphas = null;
            bool hasAlphas = false;
            foreach (ProjectDataset.SpriteFrameRow drFrame in ProjectData.GetSortedSpriteFrames(drState))
            {
               if (subFrames == null)
               {
                  subFrames = new System.Collections.ArrayList();
                  alphas = new System.Collections.ArrayList();
               }
               subFrames.Add(new CodePrimitiveExpression(drFrame.FrameValue));
               alphas.Add(new CodePrimitiveExpression(drFrame.MaskAlphaLevel));
               if (drFrame.MaskAlphaLevel > 0)
                  hasAlphas = true;
               if (drFrame.Duration > 0)
               {
                  nAccumulatedDuration += drFrame.Duration;
                  if (subFrames.Count == 1)
                  {
                     if (hasAlphas)
                        stateParams.Add(new CodeObjectCreateExpression("SpriteFrame",
                           new CodeVariableReferenceExpression(varBounds.Name),
                           new CodeVariableReferenceExpression(varFrameset.Name),
                           new CodePrimitiveExpression(nAccumulatedDuration),
                           (CodePrimitiveExpression)subFrames[0],
                           (CodePrimitiveExpression)alphas[0]));
                     else
                        stateParams.Add(new CodeObjectCreateExpression("TileFrame",
                           new CodePrimitiveExpression(nAccumulatedDuration),
                           (CodePrimitiveExpression)subFrames[0]));
                  }
                  else
                  {
                     if (hasAlphas)
                        stateParams.Add(new CodeObjectCreateExpression("SpriteFrame",
                           new CodeVariableReferenceExpression(varBounds.Name),
                           new CodeVariableReferenceExpression(varFrameset.Name),
                           new CodePrimitiveExpression(nAccumulatedDuration),
                           new CodeArrayCreateExpression(typeof(int),
                           (CodeExpression[])subFrames.ToArray(typeof(CodeExpression))),
                           new CodeArrayCreateExpression(typeof(byte),
                           (CodeExpression[])alphas.ToArray(typeof(CodeExpression)))));
                     else
                        stateParams.Add(new CodeObjectCreateExpression("TileFrame",
                           new CodePrimitiveExpression(nAccumulatedDuration),
                           new CodeArrayCreateExpression(typeof(int),
                           (CodeExpression[])subFrames.ToArray(typeof(CodeExpression)))));
                  }
                  subFrames.Clear();
                  alphas.Clear();
                  hasAlphas = false;
               }
            }

            initStates.Statements.Add(new CodeAssignStatement(
               new CodeArrayIndexerExpression(
               new CodeFieldReferenceExpression(
               new CodeTypeReferenceExpression(clsSpriteDef.Name), SpriteStateField),
               new CodeCastExpression(typeof(int),
               new CodeFieldReferenceExpression(
               new CodeTypeReferenceExpression(enumStates.Name),
               NameToVariable(drState.Name)))),
               new CodeObjectCreateExpression(SpriteStateClassName,
               (CodeExpression[])stateParams.ToArray(typeof(CodeExpression)))));
         }

         if (enumStates.Members.Count > 0)
            ((CodeMemberField)enumStates.Members[0]).InitExpression = new CodePrimitiveExpression(0);

         FrameCache.ClearDisplayCache(null);

         CodeMemberField fldSpriteStates = new CodeMemberField(
            new CodeTypeReference(SpriteStateClassName, 1), SpriteStateField);
         fldSpriteStates.Attributes = MemberAttributes.Static | MemberAttributes.Private;
         fldSpriteStates.InitExpression = new CodePrimitiveExpression(null);
         clsSpriteDef.Members.Add(fldSpriteStates);

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
            new CodeTypeReferenceExpression(clsSpriteDef.Name), SpriteStateField),
            new CodeArgumentReferenceExpression("state"))));
         clsSpriteDef.Members.Add(prpIndexer);

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
            new CodeTypeReferenceExpression(clsSpriteDef.Name), SpriteStateField),
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
            new CodeTypeReferenceExpression(clsSpriteDef.Name), SpriteStateField),
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), "state")),
            SpriteStateSolidHeight)));
         clsSpriteDef.Members.Add(propStateInfo);

         // Sprite Rules
         ProjectDataset.SpriteRuleRow[] rules = ProjectData.GetSortedSpriteRules(drSpriteDef,false);
         RuleContent[] ruleArray = new RuleContent[rules.Length];
         CodeMemberMethod mthExecuteRules = new CodeMemberMethod();
         mthExecuteRules.Name = "ExecuteRules";
         mthExecuteRules.Attributes = MemberAttributes.Override | MemberAttributes.Family;
         try
         {
            if (GenerateLevel > CodeLevel.ExcludeRules)
            {
               for (int i = 0; i < rules.Length; i++)
                  ruleArray[i] = new RuleContent(rules[i]);
               GenerateRules(ruleArray, mthExecuteRules);
            }
            clsSpriteDef.Members.Add(mthExecuteRules);
         }
         catch (System.ApplicationException ex)
         {
            err.WriteLine("Error generating sprite definition \"" + drSpriteDef.Name +"\": " +  ex.Message);
         }

         Generator.GenerateCodeFromNamespace(nsSprites, txt, GeneratorOptions);
      }

      public void GenerateTileCategories(System.IO.TextWriter txt)
      {
         CodeTypeDeclaration enumCategories = new CodeTypeDeclaration("TileCategoryName");
         enumCategories.IsEnum = true;
         foreach(System.Data.DataRowView drv in ProjectData.TileCategory.DefaultView)
         {
            ProjectDataset.TileCategoryRow drCat = (ProjectDataset.TileCategoryRow)drv.Row;
            enumCategories.Members.Add(new CodeMemberField(typeof(int), NameToVariable(drCat.Name)));
         }
         if (enumCategories.Members.Count > 0)
            ((CodeMemberField)enumCategories.Members[0]).InitExpression = new CodePrimitiveExpression(0);
         enumCategories.Members.Add(new CodeMemberField(typeof(int), "Count"));

         Generator.GenerateCodeFromType(enumCategories, txt, GeneratorOptions);
      }

      public void GenerateLayerSpriteCategoriesBase(System.IO.TextWriter txt)
      {
         CodeTypeDeclaration clsLayerSpriteCategoriesBase = new CodeTypeDeclaration(LayerSpriteCategoriesBaseClassName);
         clsLayerSpriteCategoriesBase.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
         CodeMemberField fldEmpty = new CodeMemberField(SpriteCollectionClassName, "m_EmptyCollection");
         fldEmpty.Attributes = MemberAttributes.Private | MemberAttributes.Static;
         fldEmpty.InitExpression = new CodeObjectCreateExpression(SpriteCollectionClassName);
         clsLayerSpriteCategoriesBase.Members.Add(fldEmpty);

         foreach(System.Data.DataRowView drv in ProjectData.SpriteCategory.DefaultView)
         {
            ProjectDataset.SpriteCategoryRow drCat = (ProjectDataset.SpriteCategoryRow)drv.Row;
            CodeMemberProperty prpCat = new CodeMemberProperty();
            prpCat.Name = NameToVariable(drCat.Name);
            prpCat.Type = new CodeTypeReference(SpriteCollectionClassName);
            prpCat.Attributes = MemberAttributes.Public;
            prpCat.HasGet = true;
            prpCat.HasSet = false;
            prpCat.GetStatements.Add(new CodeMethodReturnStatement(
               new CodeFieldReferenceExpression(
               new CodeTypeReferenceExpression(clsLayerSpriteCategoriesBase.Name), fldEmpty.Name)));
            clsLayerSpriteCategoriesBase.Members.Add(prpCat);
         }
         Generator.GenerateCodeFromType(clsLayerSpriteCategoriesBase, txt, GeneratorOptions);
      }

      public void GenerateSolidity(System.IO.TextWriter txt)
      {
         CodeTypeDeclaration clsSolidity = new CodeTypeDeclaration(SolidityClassName);
         clsSolidity.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
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
            new CodeFieldReferenceExpression(
            new CodeTypeReferenceExpression("EmptyTileShape"), "Value")));

         CodeMemberField fldUndefined = new CodeMemberField(SolidityClassName, "m_UndefinedSolidity");
         fldUndefined.Attributes = MemberAttributes.Private | MemberAttributes.Static | MemberAttributes.Final;
         fldUndefined.InitExpression =
            new CodeObjectCreateExpression("Solidity", 
            new CodeArrayCreateExpression("SolidityMapping", new CodeObjectCreateExpression[] {}));
         clsSolidity.Members.Add(fldUndefined);

         CodeMemberProperty prpUndefined = new CodeMemberProperty();
         prpUndefined.Name = UndefinedSolidityProperty;
         prpUndefined.Type = new CodeTypeReference(SolidityClassName);
         prpUndefined.Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final;
         prpUndefined.HasSet = false;
         prpUndefined.HasGet = true;
         prpUndefined.GetStatements.Add(
            new CodeMethodReturnStatement(
            new CodeFieldReferenceExpression(
            new CodeTypeReferenceExpression(clsSolidity.Name), fldUndefined.Name)));
         clsSolidity.Members.Add(prpUndefined);

         foreach(System.Data.DataRowView drv in ProjectData.Solidity.DefaultView)
         {
            ProjectDataset.SolidityRow drSolid = (ProjectDataset.SolidityRow)drv.Row;
            CodeMemberField fldSolid = new CodeMemberField(SolidityClassName, "m_" +
               NameToVariable(drSolid.Name));
            fldSolid.Attributes = MemberAttributes.Private | MemberAttributes.Static | MemberAttributes.Final;
            System.Collections.ArrayList mappings = new System.Collections.ArrayList();
            foreach (ProjectDataset.SolidityShapeRow drShape in drSolid.GetSolidityShapeRows())
            {
               if (!drShape.IsShapeNameNull())
               {
                  mappings.Add(new CodeObjectCreateExpression("SolidityMapping",
                     new CodeFieldReferenceExpression(
                     new CodeTypeReferenceExpression("TileCategoryName"),
                     drShape.TileCategoryRow.Name),
                     new CodeFieldReferenceExpression(
                     new CodeTypeReferenceExpression(NameToVariable(drShape.ShapeName)), "Value")));
               }
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

      public void GenerateAssemblyInfo(System.IO.TextWriter txt)
      {
         CodeCompileUnit attributes = new CodeCompileUnit();
         attributes.AssemblyCustomAttributes.Add(
            new CodeAttributeDeclaration("System.Reflection.AssemblyVersion", new CodeAttributeArgument[]
            { new CodeAttributeArgument(new CodePrimitiveExpression("1.0.0.0")) }));
         
         attributes.AssemblyCustomAttributes.Add(
            new CodeAttributeDeclaration("System.Reflection.AssemblyCompany", new CodeAttributeArgument[]
            { new CodeAttributeArgument(new CodePrimitiveExpression("Scrolling Game Development Kit")) }));

         if (SGDK2IDE.CurrentProjectFile != null)
         {
            attributes.AssemblyCustomAttributes.Add(
               new CodeAttributeDeclaration("System.Reflection.AssemblyProduct", new CodeAttributeArgument[]
              { new CodeAttributeArgument(new CodePrimitiveExpression(System.IO.Path.GetFileNameWithoutExtension(SGDK2IDE.CurrentProjectFile))) }));
         }

         Generator.GenerateCodeFromCompileUnit(attributes, txt, GeneratorOptions);
      }
      #endregion

      #region Utility Functions
      public static string NameToVariable(string name)
      {
         return name.Replace(" ","_");
      }
      public static string NameToMapClass(string name)
      {
         return NameToVariable(name) + "_Map";
      }
      public static string GetCustomCodeTemplate(string name)
      {
         if (name.EndsWith(".cs"))
            name = name.Substring(0, name.Length - 3);
         return "namespace CustomObjects\r\n{\r\n   public class " + NameToVariable(name) +
            "\r\n   {\r\n\r\n   }\r\n}\r\n";
      }
      public static string FindFullPath(string fileName)
      {
         string result;

         if (!System.IO.Path.IsPathRooted(fileName))
         {
            if (SGDK2IDE.CurrentProjectFile != null)
            {
               result = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SGDK2IDE.CurrentProjectFile), fileName);
               if (System.IO.File.Exists(result))
                  return result;
            }
         }
         else if (System.IO.File.Exists(fileName))
            return fileName;
         result = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, System.IO.Path.GetFileName(fileName));
         if (System.IO.File.Exists(result))
            return result;
         return fileName;
      }
      public static bool IsOldDll(string fileName)
      {
         if (!System.IO.File.Exists(fileName))
            return false;
         System.IO.FileStream file = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
         try
         {
            byte[] tmp = new byte[file.Length];
            file.Read(tmp, 0, (int)file.Length);
            byte[] seekText = System.Text.Encoding.ASCII.GetBytes("_CorDllMain");
            for (int pos = Array.IndexOf(tmp, seekText[0]); pos >= 0; pos = Array.IndexOf(tmp, seekText[0], pos))
            {
               int count;
               for (count=1; count < seekText.Length; count++)
               {
                  if (seekText[count] != tmp[pos+count])
                     break;
               }
               if (count >= seekText.Length)
                  return false;
               pos += count;
            }
            return true;
         }
         finally
         {
            file.Close();
         }
      }
      private CodeMemberMethod CreateGetObjectDataMethod()
      {
         CodeMemberMethod mthGetObjectData = new CodeMemberMethod();
         mthGetObjectData.Name = "GetObjectData";
         mthGetObjectData.Attributes = MemberAttributes.Public | MemberAttributes.Final;
         CodeParameterDeclarationExpression paramSerInfo = 
            new CodeParameterDeclarationExpression(
            typeof(System.Runtime.Serialization.SerializationInfo), "info");
         mthGetObjectData.Parameters.Add(paramSerInfo);
         mthGetObjectData.Parameters.Add(
            new CodeParameterDeclarationExpression(
            typeof(System.Runtime.Serialization.StreamingContext), "context"));
         return mthGetObjectData;
      }
      #endregion

      #region Compilation
      /// <summary>
      /// Compile a the temporary instance of the project to use for reflection
      /// (retrieving function names and parameter info, etc).
      /// </summary>
      /// <param name="borceRecompile">Recompile even if project was already compiled</param>
      /// <returns>Error list as a single string; null when no errors occurred</returns>
      public string CompileTempAssembly(bool forceRecompile)
      {
         if ((m_TempAssembly != null) && (!forceRecompile))
            return null;

         if (m_TempAssembly != null)
            m_TempAssembly.Dispose();
         m_TempAssembly = null;

         try
         {
            SGDK2IDE.PushStatus("Compiling temporary project for runtime property inspection", true);

            string errs;
            string[] code = GenerateCodeStrings(out errs);
            if ((errs != null) && (errs.Length > 0))
               return errs;

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
            asmRef = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.DirectX.DirectInput.KeyboardState));
            compilerParams.ReferencedAssemblies.Add(asmRef.GetFiles()[0].Name);
            compilerParams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Drawing.dll");
            compilerParams.ReferencedAssemblies.Add("System.Design.dll");
            compilerParams.ReferencedAssemblies.Add(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Reflect.dll"));
            foreach(System.Data.DataRowView drv in ProjectData.SourceCode.DefaultView)
            {
               ProjectDataset.SourceCodeRow drCode = (ProjectDataset.SourceCodeRow)drv.Row;
               if (drCode.Name.EndsWith(".dll") && !IsOldDll(FindFullPath(drCode.Name)))
               {
                  System.Reflection.Assembly refAssy = System.Reflection.Assembly.LoadWithPartialName(System.IO.Path.GetFileNameWithoutExtension(drCode.Name));
                  if (refAssy != null)
                  {
                     string assyPath = refAssy.GetModules(false)[0].FullyQualifiedName;
                     compilerParams.ReferencedAssemblies.Add(assyPath);
                  }
               }
            }
            compilerParams.GenerateExecutable = false;
            System.CodeDom.Compiler.CompilerResults results = compiler.CompileAssemblyFromSourceBatch(compilerParams, code);
            if (results.Errors.Count > 0)
            {
               System.Text.StringBuilder sb = new System.Text.StringBuilder();
               for (int i = 0; i < results.Errors.Count; i++)
               {
                  sb.Append(results.Errors[i].ToString() + Environment.NewLine);
               }
               return sb.ToString();
            }
            m_TempAssembly = new TempAssembly(compilerParams.OutputAssembly);
         }
         finally
         {
            SGDK2IDE.PopStatus();
         }
         return null;
      }

      public static object CreateInstanceAndUnwrap(string typeName, params object[] constructorParameters)
      {
         if (m_TempAssembly == null)
            throw new ApplicationException("Temporary assembly is not compiled");

         return m_TempAssembly.CreateInstanceAndUnwrap(typeName, constructorParameters);
      }

      public static void ResetTempAssembly()
      {
         if (m_TempAssembly != null)
         {
            m_TempAssembly.Dispose();
            m_TempAssembly = null;
         }
      }

      public void CompileResources(string FolderName)
      {
         string[] resxList = GetResxFileList(FolderName);

         foreach(string resxFile in resxList)
         {
            System.Resources.ResXResourceReader rd =
               new System.Resources.ResXResourceReader(resxFile);
            string resName = resxFile.Substring(0,resxFile.Length -
               System.IO.Path.GetExtension(resxFile).Length) + ".resources";
            System.Resources.ResourceWriter rw = new System.Resources.ResourceWriter(resName);
            foreach(System.Collections.DictionaryEntry de in rd)
               rw.AddResource(de.Key.ToString(), de.Value);
            rw.Generate();
            rw.Close();
            rd.Close();
         }
      }

      public string CompileProject(string ProjectName, string FolderName, out string errs)
      {
         SGDK2IDE.PushStatus("Compiling " + ProjectName + " to " + FolderName, true);
         try
         {
            if (!System.IO.Path.IsPathRooted(FolderName))
               FolderName = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, FolderName);

            string[] fileList = GetCodeFileList(FolderName);
            string[] resourcesList = GetResourcesFileList(FolderName);
            System.Collections.ArrayList OldDlls = new System.Collections.ArrayList();
            GenerateAllCode(FolderName, out errs);
            if (errs.Length > 0)
               return null;

            string resourceSwitches = string.Empty;
            foreach (string resFile in resourcesList)
            {
               resourceSwitches += " /res:\"" + resFile +"\"";
            }

            CompileResources(FolderName);

            Microsoft.CSharp.CSharpCodeProvider codeProvider = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.ICodeCompiler compiler = codeProvider.CreateCompiler();
            System.CodeDom.Compiler.CompilerParameters compilerParams = new System.CodeDom.Compiler.CompilerParameters(new string[] {}, System.IO.Path.Combine(FolderName, ProjectName + ".exe"));
            System.Reflection.Assembly asmRef = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.DirectX.Matrix));
            System.IO.File.Copy(asmRef.GetFiles()[0].Name, System.IO.Path.Combine(FolderName, System.IO.Path.GetFileName(asmRef.GetFiles()[0].Name)), true);
            compilerParams.ReferencedAssemblies.Add(asmRef.GetFiles()[0].Name);
            asmRef = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.DirectX.Direct3D.Device));
            System.IO.File.Copy(asmRef.GetFiles()[0].Name, System.IO.Path.Combine(FolderName, System.IO.Path.GetFileName(asmRef.GetFiles()[0].Name)), true);
            compilerParams.ReferencedAssemblies.Add(asmRef.GetFiles()[0].Name);
            asmRef = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.DirectX.Direct3D.Sprite));
            System.IO.File.Copy(asmRef.GetFiles()[0].Name, System.IO.Path.Combine(FolderName, System.IO.Path.GetFileName(asmRef.GetFiles()[0].Name)), true);
            compilerParams.ReferencedAssemblies.Add(asmRef.GetFiles()[0].Name);
            asmRef = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.DirectX.DirectInput.KeyboardState));
            System.IO.File.Copy(asmRef.GetFiles()[0].Name, System.IO.Path.Combine(FolderName, System.IO.Path.GetFileName(asmRef.GetFiles()[0].Name)), true);
            compilerParams.ReferencedAssemblies.Add(asmRef.GetFiles()[0].Name);
            compilerParams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Drawing.dll");
            compilerParams.ReferencedAssemblies.Add("System.Design.dll");
            foreach(System.Data.DataRowView drv in ProjectData.SourceCode.DefaultView)
            {
               ProjectDataset.SourceCodeRow drCode = (ProjectDataset.SourceCodeRow)drv.Row;
               if (drCode.Name.EndsWith(".dll"))
               {
                  if (IsOldDll(FindFullPath(drCode.Name)))
                  {
                     OldDlls.Add(FindFullPath(drCode.Name));
                  }
                  else
                  {
                     try
                     {
                        System.Reflection.Assembly assy = System.Reflection.Assembly.LoadWithPartialName(System.IO.Path.GetFileNameWithoutExtension(drCode.Name));
                        string assyPath = assy.GetModules(false)[0].FullyQualifiedName;
                        string targetPath = System.IO.Path.Combine(FolderName, System.IO.Path.GetFileName(drCode.Name));
                        System.IO.File.Copy(assyPath, targetPath, true);
                        compilerParams.ReferencedAssemblies.Add(targetPath);
                     }
                     catch(System.Exception)
                     {
                     }
                  }
               }
               else if (drCode.Name.EndsWith(".cs") && !drCode.IsCustomObjectDataNull())
                  resourceSwitches += " /res:\"" + System.IO.Path.Combine(FolderName, System.IO.Path.GetFileNameWithoutExtension(drCode.Name) + ".bin") + "\"";
            }
            compilerParams.GenerateExecutable = true;
            compilerParams.GenerateInMemory = false;
            compilerParams.IncludeDebugInformation = Debug;
            if (Debug)
               compilerParams.CompilerOptions = "/define:DEBUG /target:winexe" + resourceSwitches;
            else
               compilerParams.CompilerOptions = " /target:winexe" + resourceSwitches;
            System.CodeDom.Compiler.CompilerResults results = compiler.CompileAssemblyFromFileBatch(compilerParams, fileList);
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
            else
            {
               foreach(string dllFile in OldDlls)
               {
                  System.IO.File.Copy(dllFile,
                     System.IO.Path.Combine(System.IO.Path.GetDirectoryName(results.PathToAssembly),
                     System.IO.Path.GetFileName(dllFile)), true);
               }
            }
            return compilerParams.OutputAssembly;
         }
         finally
         {
            SGDK2IDE.PopStatus();
         }
      }
      #endregion
   }
}
