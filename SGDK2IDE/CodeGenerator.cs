/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Xml;

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
      private const string CounterMinFld = "m_nMin";
      private const string CounterValProp = "CurrentValue";
      private const string CounterMinProp = "MinValue";
      private const string CounterMaxProp = "MaxValue";
      public const string TilesetClass = "Tileset";
      private const string TileListVar = "TileList";
      private const string TileBaseClass = "TileBase";
      private const string AnimTileClass = "AnimTile";
      private const string SimpleTileClass = "SimpleTile";
      private const string EmptyTileClass = "EmptyTile";
      private const string TilesField = "m_arTiles";
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
      private const string LayerSpriteCollectionField = "m_Sprites";
      public const string SpriteStateEnumName = "State";
      private const string GetFramesetMethodName = "GetFrameset";
      private const string CoordinateTypeName = "PlanBase.Coordinate";
      private const string LayerParentField = "m_ParentMap";
      private const string LayerParentArg = "ParentMap";
      public const string SpritePlanParentField = "m_ParentLayer";
      private const string SpritePlanParentArg = "ParentLayer";
      public const string ParentLayerProperty = "ParentLayer";
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
      private const string InjectStartIndexField = "m_nInjectStartIndex";
      private const string AppendStartIndexField = "m_nAppendStartIndex";
      private const string TileCategoryNameClass = "TileCategoryName";
      private const string SpritesDir = "Sprites";
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
            return m_tempDomain.CreateInstanceFromAndUnwrap(m_assemblyFile, typeName, false,
               System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
               null, constructorParams, null, null);
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

      public Microsoft.CSharp.CSharpCodeProvider Generator = new Microsoft.CSharp.CSharpCodeProvider();
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

            txt = new System.IO.StreamWriter(System.IO.Path.Combine(FolderName, ProjectClass + ".resx"));
            GenerateProjectResx(txt, err);
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

            string SpritesFolder = System.IO.Path.Combine(FolderName, SpritesDir);
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

            txt = new System.IO.StreamWriter(GetVSProjectFile(FolderName));
            GenerateVSProject(txt);
            txt.Close();

            txt = new System.IO.StreamWriter(GetMDProjectFile(FolderName));
            GenerateMDProject(txt);
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
               System.IO.Path.Combine(FolderName, CounterClass + ".cs"),
               System.IO.Path.Combine(FolderName, FramesetClass + ".cs"),
               System.IO.Path.Combine(FolderName, TilesetClass + ".cs"),
               System.IO.Path.Combine(FolderName, SolidityClassName + ".cs"),
               System.IO.Path.Combine(FolderName, LayerSpriteCategoriesBaseClassName + ".cs"),
               System.IO.Path.Combine(FolderName, "AssemblyInfo.cs")
            });

         foreach (System.Data.DataRowView drv in ProjectData.Map.DefaultView)
            fileList.Add(GetIntermediateCodeFilename(FolderName, drv.Row));

         foreach (System.Data.DataRowView drv in ProjectData.SpriteDefinition.DefaultView)
            fileList.Add(GetIntermediateCodeFilename(FolderName, drv.Row));

         foreach (System.Data.DataRowView drv in ProjectData.SourceCode.DefaultView)
         {
            string fileName = GetIntermediateCodeFilename(FolderName, drv.Row);
            if (fileName != null)
               fileList.Add(fileName);
         }

         return (string[])(fileList.ToArray(typeof(string)));
      }

      public string GetIntermediateConfigFile(string FolderName)
      {
         foreach (System.Data.DataRowView drv in ProjectData.SourceCode.DefaultView)
         {
            string name = ((ProjectDataset.SourceCodeRow)drv.Row).Name;
            if (name.EndsWith(".config"))
               return System.IO.Path.Combine(FolderName, name);
         }
         return null;
      }

      public static string GetIntermediateCodeFilename(string FolderName, System.Data.DataRow row)
      {
         if (!System.IO.Path.IsPathRooted(FolderName))
            FolderName = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, FolderName);

         if (row is ProjectDataset.SpriteDefinitionRow)
         {
            return System.IO.Path.Combine(System.IO.Path.Combine(FolderName, SpritesDir),
               NameToVariable(((ProjectDataset.SpriteDefinitionRow)row).Name) + ".cs");
         }
         else if (row is ProjectDataset.MapRow)
         {
            return System.IO.Path.Combine(FolderName, NameToVariable(((ProjectDataset.MapRow)row).Name) + "_Map.cs");
         }
         else if (row is ProjectDataset.SourceCodeRow)
         {
            ProjectDataset.SourceCodeRow drCode = (ProjectDataset.SourceCodeRow)row;
            if (!drCode.IsTextNull() && (drCode.Text.Trim().Length > 0)
               && drCode.Name.EndsWith(".cs"))
               return System.IO.Path.Combine(FolderName, drCode.Name);
         }
         return null;
      }

      public string[] GetResxFileList(string FolderName)
      {
         System.Collections.ArrayList fileList = new System.Collections.ArrayList();
         if (!System.IO.Path.IsPathRooted(FolderName))
            FolderName = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, FolderName);
         fileList.Add(System.IO.Path.Combine(FolderName, ProjectClass + ".resx"));
         foreach (System.Data.DataRowView drv in ProjectData.Map.DefaultView)
            fileList.Add(GetIntermediateResxFilename(FolderName, drv.Row));
         return (string[])(fileList.ToArray(typeof(string)));
      }

      public static string GetIntermediateResxFilename(string FolderName, System.Data.DataRow row)
      {
         if (!System.IO.Path.IsPathRooted(FolderName))
            FolderName = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, FolderName);
         if (row is ProjectDataset.MapRow)
         {
            return System.IO.Path.Combine(FolderName, NameToVariable(((ProjectDataset.MapRow)row).Name) + "_Map.resx");
         }
         return null;
      }

      public string[] GetResourcesFileList(string FolderName)
      {
         string[] result = GetResxFileList(FolderName);
         for (int i=0; i<result.Length; i++)
            result[i] = GetIntermediateResourcesFilename(result[i]);
         return result;
      }

      public static string GetIntermediateResourcesFilename(string ResxFilename)
      {
         return ResxFilename.Substring(0, ResxFilename.Length -
            System.IO.Path.GetExtension(ResxFilename).Length) + ".resources";
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
               fileList.Add(GetIntermediateEmbeddedFilename(FolderName, drv.Row));
         }
         return (string[])(fileList.ToArray(typeof(string)));
      }

      public static string GetIntermediateEmbeddedFilename(string FolderName, System.Data.DataRow row)
      {
         if (!System.IO.Path.IsPathRooted(FolderName))
            FolderName = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, FolderName);
         if (row is ProjectDataset.SourceCodeRow)
         {
            ProjectDataset.SourceCodeRow drCode = (ProjectDataset.SourceCodeRow)row;
            if ((!drCode.IsCustomObjectDataNull()) && drCode.Name.EndsWith(".cs"))
               return System.IO.Path.Combine(FolderName, System.IO.Path.GetFileNameWithoutExtension(drCode.Name)) + ".bin";
         }
         return null;
      }

      public string[] GetLocalReferenceFileList(string FolderName)
      {
         System.Collections.ArrayList fileList = new System.Collections.ArrayList();
         if (!System.IO.Path.IsPathRooted(FolderName))
         {
            FolderName = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, FolderName);
         }

         foreach(System.Data.DataRowView drv in ProjectData.SourceCode.DefaultView)
         {
            ProjectDataset.SourceCodeRow drCode = (ProjectDataset.SourceCodeRow)drv.Row;
            if (drCode.Name.EndsWith(".dll") || drCode.Name.EndsWith(".so"))
               fileList.Add(System.IO.Path.Combine(FolderName, System.IO.Path.GetFileName(drCode.Name)));
         }
         return (string[])(fileList.ToArray(typeof(string)));
      }

      public string GetVSProjectFile(string FolderName)
      {
         return System.IO.Path.Combine(FolderName, System.IO.Path.GetFileNameWithoutExtension(SGDK2IDE.CurrentProjectFile) + ".csproj");
      }

      public string GetMDProjectFile(string FolderName)
      {
         return System.IO.Path.Combine(FolderName, System.IO.Path.GetFileNameWithoutExtension(SGDK2IDE.CurrentProjectFile) + ".mdp");
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
            if (!drCode.IsTextNull() && (drCode.Text.Trim().Length > 0) && 
               (drCode.Name.EndsWith(".cs") || drCode.Name.EndsWith(".config")))
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
            else if ((drCode.Name.EndsWith(".dll") || drCode.Name.EndsWith(".so"))
               && !drCode.IsCustomObjectDataNull())
            {
               System.IO.FileStream fs = new System.IO.FileStream(System.IO.Path.Combine(FolderName, System.IO.Path.GetFileName(drCode.Name)), System.IO.FileMode.Create);
               fs.Write(drCode.CustomObjectData, 0, drCode.CustomObjectData.Length);
               fs.Close();
            }
         }
      }
      #endregion

      #region File Level Code Generation
      public void GenerateProjectResx(System.IO.TextWriter txt, System.IO.TextWriter err)
      {
         System.Resources.ResXResourceWriter w = new System.Resources.ResXResourceWriter(txt);
         try
         {
            ProjectDataset.ProjectRow prj = ProjectData.ProjectRow;
            w.AddResource("_MaxPlayers", prj.MaxPlayers.ToString());
            w.AddResource("_MaxViews", prj.MaxViews.ToString());
            w.AddResource("_GameCredits", prj.Credits);
            w.AddResource("_DisplayMode", prj.DisplayMode);
            w.AddResource("_Windowed", prj.Windowed.ToString());
            w.AddResource("_WindowTitle", prj.TitleText.ToString());
            if ((prj.StartMap == null) || (prj.StartMap.Length == 0))
               err.WriteLine("Startup map has not been specified in the project settings");
            else
               w.AddResource("_StartupMapType", prj.StartMap);
            if ((prj.OverlayMap != null) && (prj.OverlayMap.Length > 0))
               w.AddResource("_OverlayMapType", prj.OverlayMap);

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

         framesetClassDecl.Comments.AddRange(new CodeCommentStatement[]
            {
               new CodeCommentStatement("<summary>Provides objects that encapsulate the functionality of the framesets defined at design time.</summary>", true),
               new CodeCommentStatement("<remarks>The class is entirely generated based on the framesets defined in the project. Static members exist to create/access instances of each frameset, and each instance represents one specific frameset. Only one instance (maximum) of each frameset will ever exist per display.</remarks>", true)
            });

         framesetClassDecl.IsPartial = true;
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
         classFramesetRef.IsPartial = true;
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

         classFramesetRef.Comments.Add(new CodeCommentStatement(
            "<summary>Provides serialization services for <see cref=\"" + FramesetClass + "\" /> to allow objects that reference framesets to be saved without saving everything that is referenced by the frameset.</summary>", true));

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
         mthGetFrameset.Comments.AddRange(new CodeCommentStatement[]
            {
               new CodeCommentStatement("<summary>Retrieves an object representing the frameset by name</summary>", true),
               new CodeCommentStatement("<param name=\"Name\">Specifies the name of the frameset as defined in the project at design time.</param>", true),
               new CodeCommentStatement("<param name=\"disp\">Specifies the display to which the frameset is linked. This is used to construct the hardware objects that support the frameset if the graphics for the frameset have not been loaded into the hardware</param>", true),
               new CodeCommentStatement("<returns>An instance of the <see cref=\"" + FramesetClass + "\"/> class.</returns>", true),
               new CodeCommentStatement("<remarks>If the specified frameset has already been constructed for the specified display, it will be returned from the cache, otherwise a new instance will be constructed and added to the cache before returning.</remarks>", true)
            });
         
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
               CodeObjectCreateExpression rectExp = new CodeObjectCreateExpression(
                  typeof(System.Drawing.Rectangle),
                  new CodePrimitiveExpression((drFrame.CellIndex % g.Columns) * g.CellWidth),
                  new CodePrimitiveExpression(((int)(drFrame.CellIndex / g.Columns)) * g.CellHeight),
                  new CodePrimitiveExpression(g.CellWidth),
                  new CodePrimitiveExpression(g.CellHeight));
               if ((drFrame.m11 == 1) &&
                  (drFrame.m12 == 0) &&
                  (drFrame.m21 == 0) &&
                  (drFrame.m22 == 1) &&
                  (drFrame.dx == 0) &&
                  (drFrame.dy == 0))
               {
                  if (drFrame.color == -1)
                     frameParams.Add(new CodeObjectCreateExpression(FrameClass, getTextureExp, cellExp, rectExp));
                  else
                     frameParams.Add(new CodeObjectCreateExpression(FrameClass, getTextureExp, cellExp, rectExp, new CodePrimitiveExpression(drFrame.color)));
               }
               else
               {
                  System.Drawing.PointF[] corners = new System.Drawing.PointF[] {
                     new System.Drawing.PointF(0, 0),
                     new System.Drawing.PointF(0, g.CellHeight),
                     new System.Drawing.PointF(g.CellWidth, g.CellHeight),
                     new System.Drawing.PointF(g.CellWidth, 0)};
                  using (System.Drawing.Drawing2D.Matrix mtx = new System.Drawing.Drawing2D.Matrix(
                     drFrame.m11, drFrame.m12, drFrame.m21, drFrame.m22, drFrame.dx, drFrame.dy))
                  {
                     mtx.TransformPoints(corners);
                  }
                  CodeObjectCreateExpression[] cornerPts = new CodeObjectCreateExpression[4];
                  for (int ptIdx = 0; ptIdx < 4; ptIdx++)
                     cornerPts[ptIdx] = new CodeObjectCreateExpression(typeof(System.Drawing.PointF),
                        new CodePrimitiveExpression(corners[ptIdx].X),
                        new CodePrimitiveExpression(corners[ptIdx].Y));
                  CodeArrayCreateExpression cornerArray = new CodeArrayCreateExpression(
                     typeof(System.Drawing.PointF), cornerPts[0], cornerPts[1], cornerPts[2], cornerPts[3]);
                  if (drFrame.color == -1)
                     frameParams.Add(new CodeObjectCreateExpression(FrameClass, getTextureExp, cellExp, cornerArray, rectExp));
                  else
                     frameParams.Add(new CodeObjectCreateExpression(FrameClass, getTextureExp, cellExp, cornerArray, rectExp, new CodePrimitiveExpression(drFrame.color)));
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
         indexer.GetStatements.Add(new CodeMethodReturnStatement(
            new CodeArrayIndexerExpression(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), "m_arFrames"),
            new CodeBinaryOperatorExpression(
            new CodeArgumentReferenceExpression("index"),
            CodeBinaryOperatorType.Modulus,
            new CodePropertyReferenceExpression(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), "m_arFrames"),
            "Length")))));
         framesetClassDecl.Members.Add(indexer);
         indexer.Comments.Add(
            new CodeCommentStatement("<summary>Return the <see cref=\"" + FrameClass + "\"/> object defining the frame at the specified 0-based index within this frameset</summary>", true));

         CodeMemberProperty countProp = new CodeMemberProperty();
         countProp.Name = "Count";
         countProp.Type = new CodeTypeReference(typeof(int));
         countProp.Attributes = MemberAttributes.Public | MemberAttributes.Final;
         countProp.HasSet = false;
         countProp.HasGet = true;
         countProp.GetStatements.Add(new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "m_arFrames"), "Length")));
         framesetClassDecl.Members.Add(countProp);
         countProp.Comments.Add(
            new CodeCommentStatement("<summary>Return the number of frames in this frameset.</summary>", true));

         Generator.GenerateCodeFromType(framesetClassDecl, txt, GeneratorOptions);
         Generator.GenerateCodeFromType(classFramesetRef, txt, GeneratorOptions);
      }

      public void GenerateCounters(System.IO.TextWriter txt)
      {
         CodeTypeDeclaration counterClassDecl = new CodeTypeDeclaration(CounterClass);
         counterClassDecl.IsPartial = true;
         CodeConstructor construct = new CodeConstructor();
         construct.Attributes = MemberAttributes.Public;
         construct.Parameters.AddRange(new CodeParameterDeclarationExpression[] { new CodeParameterDeclarationExpression(typeof(int), "nValue"), new CodeParameterDeclarationExpression(typeof(int), "nMin"), new CodeParameterDeclarationExpression(typeof(int), "nMax") });
         construct.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CounterValFld), new CodeArgumentReferenceExpression("nValue")));
         construct.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CounterMinFld), new CodeArgumentReferenceExpression("nMin")));
         construct.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CounterMaxFld), new CodeArgumentReferenceExpression("nMax")));
         counterClassDecl.Members.Add(construct);
         counterClassDecl.Comments.AddRange(new CodeCommentStatement[]
            {
               new CodeCommentStatement("<summary>Represents a numeric counter defined in the project</summary>", true),
               new CodeCommentStatement("<remarks>This class is generated based on the counters defined in the project. Individual counter instances are represented as static members of this class whose names are based on the names defined for counters in the project.</remarks>", true)
            });
         construct.Comments.AddRange(new CodeCommentStatement[]
            {
               new CodeCommentStatement("<summary>Constructs a new counter instance given all its parameters</summary>", true),
               new CodeCommentStatement("<param name=\"nValue\">Initial value of this counter</param>", true),
               new CodeCommentStatement("<param name=\"nMin\">Minimum value of this counter</param>", true),
               new CodeCommentStatement("<param name=\"nMax\">Maximum value of this counter</param>", true),
               new CodeCommentStatement("<remarks>This is called by the generated code that creates all the counter instances to initialize all the projects' counters</remarks>", true)
            });

         CodeMemberField fld = new CodeMemberField(typeof(int), CounterValFld);
         fld.Attributes = MemberAttributes.Private | MemberAttributes.Final;
         counterClassDecl.Members.Add(fld);

         fld = new CodeMemberField(typeof(int), CounterMinFld);
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
         CodeFieldReferenceExpression minValue = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CounterMinFld);
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
         propDecl.Comments.AddRange(new CodeCommentStatement[]
            {
               new CodeCommentStatement("<summary>Gets or sets the current value of the counter to a specified integer</summary>", true),
               new CodeCommentStatement("<remarks>If the value is less than <see cref=\"" + CounterMinProp + "\"/>, it is set to <see cref=\"" + CounterMinProp + "\"/>.  If it is greater than <see cref=\"" + CounterMaxProp + "\"/>, it is set to <see cref=\"" + CounterMaxProp + "\"/></remarks>.", true)
            });

         propDecl = new CodeMemberProperty();
         propDecl.Attributes = MemberAttributes.Public | MemberAttributes.Final;
         propDecl.HasGet = true;
         propDecl.HasSet = false;
         propDecl.Name = CounterMinProp;
         propDecl.Type = new CodeTypeReference(typeof(int));
         propDecl.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CounterMinFld)));
         counterClassDecl.Members.Add(propDecl);
         propDecl.Comments.Add(new CodeCommentStatement("<summary>Returns the minimum value of this counter <seealso cref=\"" + CounterValProp + "\"/><seealso cref=\"" + CounterMaxProp + "\"/></summary>", true));

         propDecl = new CodeMemberProperty();
         propDecl.Attributes = MemberAttributes.Public | MemberAttributes.Final;
         propDecl.HasGet= true;
         propDecl.HasSet = false;
         propDecl.Name = CounterMaxProp;
         propDecl.Type = new CodeTypeReference(typeof(int));
         propDecl.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CounterMaxFld)));
         counterClassDecl.Members.Add(propDecl);
         propDecl.Comments.Add(new CodeCommentStatement("<summary>Returns the maximum value of this counter <seealso cref=\"" + CounterValProp + "\"/><seealso cref=\"" + CounterMinProp + "\"/></summary>", true));


         foreach(System.Data.DataRowView drv in ProjectData.Counter.DefaultView)
         {
            ProjectDataset.CounterRow drCounter = (ProjectDataset.CounterRow)drv.Row;
            CodeMemberField fldCounter = new CodeMemberField("Counter", "m_" + NameToVariable(drCounter.Name));
            fldCounter.InitExpression = new CodeObjectCreateExpression("Counter", new CodePrimitiveExpression(drCounter.Value), new CodePrimitiveExpression(drCounter.Min), new CodePrimitiveExpression(drCounter.Max));
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
         classTileset.IsPartial = true;
         classTileset.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
         classTileset.BaseTypes.Add(typeof(System.Runtime.Serialization.ISerializable));
         CodeTypeConstructor staticConstructor = new CodeTypeConstructor();
         classTileset.Members.Add(staticConstructor);
         staticConstructor.Attributes |= MemberAttributes.Static;
         classTileset.Comments.AddRange( new CodeCommentStatement[]
            {
               new CodeCommentStatement("<summary>Represents tilesets defined in the project</summary>", true),
               new CodeCommentStatement("<remarks>This class is generated based on tilesets defined in the project, and contains members to access all tilesets as <see cref=\"" + TilesetClass + "\"/> instances.</remarks>", true)
            });
         
         // Implement ISerializable
         CodeMemberMethod mthGetObjectData = CreateGetObjectDataMethod();
         classTileset.Members.Add(mthGetObjectData);
         mthGetObjectData.Statements.Add(
            new CodeMethodInvokeExpression(
            new CodeArgumentReferenceExpression("info"),
            "SetType", new CodeExpression[]
            {new CodeTypeOfExpression(TilesetRefClassName)}));            

         CodeTypeDeclaration classTilesetRef = new CodeTypeDeclaration(TilesetRefClassName);
         classTilesetRef.IsPartial = true;
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
         classTilesetRef.Comments.AddRange(new CodeCommentStatement[]
            {
               new CodeCommentStatement("<summary>Provides serialization services for <see cref=\"" + TilesetClass + "\"/> to allow classes that reference a tileset to be saved to a file without writing all the data internal to the tileset</summary>", true),
               new CodeCommentStatement("<remarks>This class is generated when the project is compiled</remarks>", true)
            });

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
                        new CodeTypeReferenceExpression(TileCategoryNameClass), NameToVariable(drCat.CategorizedTilesetRowParent.Name));
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
         prpTile.Comments.Add(new CodeCommentStatement("<summary>Return a <see cref=\"" + TileBaseClass + "\"/> object describing the tile at the specified tile index in the tileset.</summary>", true));

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
         prpTileWidth.Comments.AddRange(new CodeCommentStatement[]
         {
            new CodeCommentStatement("<summary>Return the width (in pixels) of tiles in this tileset</summary>", true),
            new CodeCommentStatement("<remarks>This width determines the distance from one tile to the next when drawn on a layer, and does not necessarily correspond to the size of the graphics within the tile.</remarks>", true)
         });

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
         prpTileHeight.Comments.AddRange(new CodeCommentStatement[]
         {
            new CodeCommentStatement("<summary>Return the height (in pixels) of tiles in this tileset</summary>", true),
            new CodeCommentStatement("<remarks>This height determines the distance from one tile to the next when drawn on a layer, and does not necessarily correspond to the size of the graphics within the tile.</remarks>", true)
         });

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
         prpTileCount.Comments.AddRange(new CodeCommentStatement[]
         {
            new CodeCommentStatement("<summary>Return the number of tiles in this tileset.</summary>", true),
            new CodeCommentStatement("<remarks>This value will correspond to the largest mapped tile value or the largest frame index in the associated frameset, whichever is larger.</remarks>", true)
         });

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
         CodeTypeDeclaration clsMap = new CodeTypeDeclaration(NameToMapClass(drMap.Name));
         clsMap.BaseTypes.Add("MapBase");
         clsMap.IsPartial = true;
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
         mthExecuteMapRules.Name = "ExecuteRulesInternal";
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
            if (String.IsNullOrEmpty(drLayer.Tileset))
               continue;
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
            clsLayer.IsPartial = true;
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
                  new CodeParameterDeclarationExpression(typeof(int), "nVirtualColumns"),
                  new CodeParameterDeclarationExpression(typeof(int), "nVirtualRows"),
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
                  new CodeArgumentReferenceExpression("nVirtualColumns"),
                  new CodeArgumentReferenceExpression("nVirtualRows"),
                  new CodeArgumentReferenceExpression("Position"),
                  new CodeArgumentReferenceExpression("ScrollRate"),
                  new CodePrimitiveExpression(-1), // Placeholder for nInjectStartIndex
                  new CodePrimitiveExpression(-1), // Placeholders for nAppendStartIndex
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
               new CodePrimitiveExpression(drLayer.VirtualWidth),
               new CodePrimitiveExpression(drLayer.VirtualHeight),
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

            CodeMemberField fldLayerParent = new CodeMemberField(clsMap.Name, LayerParentField);
            fldLayerParent.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            clsLayer.Members.Add(fldLayerParent);

            lyrConstructor.Statements.Add(new CodeAssignStatement(
               new CodeFieldReferenceExpression(
               new CodeThisReferenceExpression(), fldLayerParent.Name),
               new CodeArgumentReferenceExpression(LayerParentArg)));

            ProjectDataset.SpritePlanRow[] plans = ProjectData.GetSortedSpritePlans(drLayer, false);
            ProjectDataset.SpriteRow[] sprites = ProjectData.GetSortedSpriteRows(drLayer, false);
            CodeMemberMethod mthExecuteLayerRules = new CodeMemberMethod();

            if ((plans.Length > 0) || (sprites.Length > 0))
            {
               mthExecuteLayerRules.Attributes = MemberAttributes.Public | MemberAttributes.Override;
               mthExecuteLayerRules.Name = "ExecuteRulesInternal";
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
               clsPlan.IsPartial = true;
               clsPlan.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
               clsPlan.BaseTypes.Add(drPlan.BaseClass);

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
               prpPlanParent.Name = ParentLayerProperty;
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
                     mthExecuteRules.Name = "ExecuteRulesInternal";
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
            clsLayerSpriteCategories.IsPartial = true;
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

            int injectStartIndex = -1;
            int appendStartIndex = -1;
            for (int spriteIndex = 0; spriteIndex < sprites.Length; spriteIndex++)
            {
               if ((injectStartIndex < 0) &&  (sprites[spriteIndex].Priority >= drLayer.Priority))
                  injectStartIndex = spriteIndex;
               if ((appendStartIndex < 0) && (sprites[spriteIndex].Priority > drLayer.Priority))
                  appendStartIndex = spriteIndex;
            }

            if (injectStartIndex >= 0)
               lyrConstructor.BaseConstructorArgs[12] = new CodePrimitiveExpression(injectStartIndex);
            else
               lyrConstructor.BaseConstructorArgs[12] = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MaxValue");

            if (appendStartIndex >= 0)
               lyrConstructor.BaseConstructorArgs[13] = new CodePrimitiveExpression(appendStartIndex);
            else
               lyrConstructor.BaseConstructorArgs[13] = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MaxValue");

            System.Collections.Hashtable htCategories = new System.Collections.Hashtable();
            
            // Removed optimization to ignore sprites for layers that don't have
            // sprites because they might get dynamically added sprites.
            //if (sprites.Length > 0)
            //{
               mthExecuteLayerRules.Statements.Add(new CodeMethodInvokeExpression(
                  new CodeThisReferenceExpression(), "ProcessSprites"));

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
                  ProjectDataset.SpriteParameterRow[] sprParams = ProjectData.GetSortedSpriteParameters(drDef, false);
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
                     SpritesNamespace + "." + NameToVariable(drDef.Name), (CodeExpression[])
                     SpriteCreateParams.ToArray(typeof(CodeExpression)));
                  clsLayer.Members.Add(new CodeMemberField(SpritesNamespace + "." + NameToVariable(drDef.Name), "m_" + NameToVariable(sprite.Name)));
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
            //} (Removed optimization to ignore sprites)

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
               new CodeThisReferenceExpression(), LayerSpriteCollectionField),
               createLayerSprites));

            lyrConstructor.Statements.Add(
               new CodeAssignStatement(
               new CodeFieldReferenceExpression(
               new CodeThisReferenceExpression(), SpriteCategoriesFieldName),
               new CodeObjectCreateExpression(clsLayerSpriteCategories.Name, new CodeThisReferenceExpression())));
         }
         // Flush forces ScissorRectangle to apply to this batch only.
         mthDraw.Statements.Add(new CodeMethodInvokeExpression(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), MapDisplayField), "Flush"));
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
                  if (rule.Parameter1 != null &&
                     (rule.Parameter1.StartsWith("(") || !rule.Parameter1.EndsWith(")")))
                     rule.Parameter1 = "(" + rule.Parameter1 + ")";
                  if (rule.Parameter2 != null &&
                     (!rule.Parameter2.StartsWith("(") || !rule.Parameter2.EndsWith(")")))
                     rule.Parameter2 = "(" + rule.Parameter2 + ")";
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

      public string ConvertRuleToCode(ProjectDataset.SpriteRuleRow[] drRules, string name)
      {
         CodeNamespace nsSprites = new CodeNamespace(SpritesNamespace);
         CodeTypeDeclaration clsSpriteDef = new CodeTypeDeclaration(NameToVariable(drRules[0].SpriteDefinitionRow.Name));
         clsSpriteDef.IsPartial = true;
         nsSprites.Types.Add(clsSpriteDef);

         CodeMemberMethod mthRule = new CodeMemberMethod();
         mthRule.Name = name;
         mthRule.Attributes = MemberAttributes.Public | MemberAttributes.Final;
         mthRule.CustomAttributes.Add(
            new CodeAttributeDeclaration(
            new CodeTypeReference(typeof(System.ComponentModel.DescriptionAttribute)),
            new CodeAttributeArgument(
            new CodePrimitiveExpression("Executes rules converted to code from sprite " + drRules[0].SpriteDefinitionRow.Name))));
         clsSpriteDef.Members.Add(mthRule);
         RuleContent[] ruleArray = new RuleContent[drRules.Length];
         for (int i = 0; i < drRules.Length; i++)
            ruleArray[i] = new RuleContent(drRules[i]);
         GenerateRules(ruleArray, mthRule);
         using (System.IO.StringWriter sw = new System.IO.StringWriter())
         {
            if (string.IsNullOrEmpty(name))
            {
               foreach (CodeStatement stmt in mthRule.Statements)
                  Generator.GenerateCodeFromStatement(stmt, sw, GeneratorOptions);
            }
            else
            {
               Generator.GenerateCodeFromNamespace(nsSprites, sw, GeneratorOptions);
            }
            return sw.ToString();
         }
      }

      public string ConvertRuleToCode(ProjectDataset.PlanRuleRow[] drRules, string name)
      {
         CodeTypeDeclaration clsMap = new CodeTypeDeclaration(NameToMapClass(drRules[0].MapName));
         clsMap.IsPartial = true;
         CodeTypeDeclaration clsLayer = new CodeTypeDeclaration(NameToVariable(drRules[0].LayerName) + "_Lyr");
         clsLayer.IsPartial = true;
         clsMap.Members.Add(clsLayer);
         CodeTypeDeclaration clsPlan = new CodeTypeDeclaration(NameToVariable(drRules[0].PlanName));
         clsPlan.IsPartial = true;
         clsLayer.Members.Add(clsPlan);

         CodeMemberMethod mthRule = new CodeMemberMethod();
         mthRule.Name = name;
         mthRule.Attributes = MemberAttributes.Public | MemberAttributes.Final;
         mthRule.CustomAttributes.Add(
            new CodeAttributeDeclaration(
            new CodeTypeReference(typeof(System.ComponentModel.DescriptionAttribute)),
            new CodeAttributeArgument(
            new CodePrimitiveExpression("Executes rules converted to code from plan " + drRules[0].SpritePlanRowParent.Name))));
         clsPlan.Members.Add(mthRule);
         RuleContent[] ruleArray = new RuleContent[drRules.Length];
         for (int i = 0; i < drRules.Length; i++)
            ruleArray[i] = new RuleContent(drRules[i]);
         GenerateRules(ruleArray, mthRule);
         using (System.IO.StringWriter sw = new System.IO.StringWriter())
         {
            if (string.IsNullOrEmpty(name))
            {
               foreach (CodeStatement stmt in mthRule.Statements)
                  Generator.GenerateCodeFromStatement(stmt, sw, GeneratorOptions);
            }
            else
            {
               Generator.GenerateCodeFromType(clsMap, sw, GeneratorOptions);
            }
            return sw.ToString();
         }
      }

      public void GenerateSpriteDef(ProjectDataset.SpriteDefinitionRow drSpriteDef, System.IO.TextWriter txt, System.IO.TextWriter err)
      {
         CodeNamespace nsSprites = new CodeNamespace(SpritesNamespace);
         CodeTypeDeclaration clsSpriteDef = new CodeTypeDeclaration(NameToVariable(drSpriteDef.Name));
         nsSprites.Types.Add(clsSpriteDef);
         clsSpriteDef.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));

         clsSpriteDef.BaseTypes.Add(drSpriteDef.BaseClass);
         clsSpriteDef.IsPartial = true;
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

         CodeConstructor dynamicConstructor = new CodeConstructor();
         dynamicConstructor.Attributes = MemberAttributes.Public;
         dynamicConstructor.Parameters.AddRange(new CodeParameterDeclarationExpression[]
            {
               new CodeParameterDeclarationExpression(LayerBaseClassName, "layer"),
               new CodeParameterDeclarationExpression(typeof(double), "x"),
               new CodeParameterDeclarationExpression(typeof(double), "y"),
               new CodeParameterDeclarationExpression(typeof(double), "dx"),
               new CodeParameterDeclarationExpression(typeof(double), "dy"),
               new CodeParameterDeclarationExpression(typeof(int), "state"),
               new CodeParameterDeclarationExpression(typeof(int), "frame"),
               new CodeParameterDeclarationExpression(typeof(bool), "active"),
               new CodeParameterDeclarationExpression("Display", "disp"),
               new CodeParameterDeclarationExpression("Solidity", "solidity"),
               new CodeParameterDeclarationExpression(typeof(int), "color"),
               new CodeParameterDeclarationExpression(typeof(bool), "dynamic")
            });
         dynamicConstructor.ChainedConstructorArgs.AddRange(new CodeExpression[]
            {
               new CodeArgumentReferenceExpression("layer"),
               new CodeArgumentReferenceExpression("x"),
               new CodeArgumentReferenceExpression("y"),
               new CodeArgumentReferenceExpression("dx"),
               new CodeArgumentReferenceExpression("dy"),
               new CodeCastExpression("Sprites." + NameToVariable(drSpriteDef.Name) + ".State",
               new CodeArgumentReferenceExpression("state")),
               new CodeArgumentReferenceExpression("frame"),
               new CodeArgumentReferenceExpression("active"),
               new CodeArgumentReferenceExpression("disp"),
               new CodeArgumentReferenceExpression("solidity"),
               new CodeArgumentReferenceExpression("color")
            });

         CodeMemberMethod mthClearParams = new CodeMemberMethod();
         mthClearParams.Name = "ClearParameters";
         mthClearParams.Attributes = MemberAttributes.Override | MemberAttributes.Public;
         clsSpriteDef.Members.Add(mthClearParams);

         foreach(ProjectDataset.SpriteParameterRow drParam in ProjectData.GetSortedSpriteParameters(drSpriteDef, false))
         {
            CodeMemberField fldParam = new CodeMemberField(typeof(int), NameToVariable(drParam.Name));
            fldParam.Attributes = MemberAttributes.Public;
            clsSpriteDef.Members.Add(fldParam);
            CodeFieldReferenceExpression refParam = new CodeFieldReferenceExpression(
               new CodeThisReferenceExpression(), NameToVariable(drParam.Name));
            constructor.Parameters.Add(new CodeParameterDeclarationExpression(
               typeof(int), NameToVariable(drParam.Name)));
            dynamicConstructor.ChainedConstructorArgs.Add(new CodePrimitiveExpression(0));
            constructor.Statements.Add(new CodeAssignStatement(refParam,               
               new CodeArgumentReferenceExpression(NameToVariable(drParam.Name))));
            mthClearParams.Statements.Add(new CodeAssignStatement(refParam, new CodePrimitiveExpression(0)));
         }

         CodeConditionStatement dynamicCondition = new CodeConditionStatement();
         dynamicCondition.Condition = new CodeArgumentReferenceExpression("dynamic");
         dynamicConstructor.Statements.Add(dynamicCondition);

         dynamicCondition.TrueStatements.Add(
            new CodeMethodInvokeExpression(
            new CodeFieldReferenceExpression(
            new CodePropertyReferenceExpression(
            new CodeThisReferenceExpression(), ParentLayerProperty),
            LayerSpriteCollectionField),
            "Add", new CodeThisReferenceExpression()));

         CodeMemberMethod mthRemoveFromCategories = new CodeMemberMethod();
         mthRemoveFromCategories.Name = "RemoveFromCategories";
         mthRemoveFromCategories.Attributes = MemberAttributes.Public | MemberAttributes.Override;
         clsSpriteDef.Members.Add(mthRemoveFromCategories);

         foreach(ProjectDataset.SpriteCategorySpriteRow drCategory in drSpriteDef.GetSpriteCategorySpriteRows())
         {
            CodePropertyReferenceExpression refCategory =
            new CodePropertyReferenceExpression(
               new CodeFieldReferenceExpression(
               new CodePropertyReferenceExpression(
               new CodeThisReferenceExpression(), ParentLayerProperty),
               SpriteCategoriesFieldName), NameToVariable(drCategory.SpriteCategoryRow.Name));

            dynamicCondition.TrueStatements.Add(
               new CodeMethodInvokeExpression(refCategory,
               "Add", new CodeThisReferenceExpression()));

            mthRemoveFromCategories.Statements.Add(
               new CodeMethodInvokeExpression(refCategory,
               "Remove", new CodeThisReferenceExpression()));
         }

         clsSpriteDef.Members.Add(constructor);
         clsSpriteDef.Members.Add(dynamicConstructor);

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
         CodeTypeDeclaration enumCategories = new CodeTypeDeclaration(TileCategoryNameClass);
         enumCategories.IsEnum = true;
         foreach(System.Data.DataRowView drv in ProjectData.TileCategory.DefaultView)
         {
            ProjectDataset.TileCategoryRow drCat = (ProjectDataset.TileCategoryRow)drv.Row;
            enumCategories.Members.Add(new CodeMemberField(typeof(int), NameToVariable(drCat.Name)));
         }
         if (enumCategories.Members.Count > 0)
            ((CodeMemberField)enumCategories.Members[0]).InitExpression = new CodePrimitiveExpression(0);
         enumCategories.Members.Add(new CodeMemberField(typeof(int), "Count"));

         enumCategories.Comments.AddRange(new CodeCommentStatement[]
            {
               new CodeCommentStatement("<summary>Provides an enumerated list of every tile category defined in the project.</summary>", true),
               new CodeCommentStatement("<remarks>This class is generated based on the tile categories defined in the project. Each category name is converted to an enumerated value based on the category name. These enumerated values can be used to refer to tile categories more efficiently than a string name would.</remarks>", true)
            });

         Generator.GenerateCodeFromType(enumCategories, txt, GeneratorOptions);
      }

      public void GenerateLayerSpriteCategoriesBase(System.IO.TextWriter txt)
      {
         CodeTypeDeclaration clsLayerSpriteCategoriesBase = new CodeTypeDeclaration(LayerSpriteCategoriesBaseClassName);
         clsLayerSpriteCategoriesBase.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
         clsLayerSpriteCategoriesBase.Comments.AddRange(new CodeCommentStatement[]
            {
               new CodeCommentStatement("<summary>Provides categorized access to sprite within a layer for all categories defined in the project</summary>", true),
               new CodeCommentStatement("<remarks>Each <see cref=\"" + LayerBaseClassName + "." + SpriteCategoriesFieldName + "\"/> member of each layer contains an instance of a class derived from this that knows how to return sprites by category for the individual layer. Each sprite category is represented as a member whose name is based on a sprite category defined in the project, and returns a <see cref=\"" + SpriteCollectionClassName + "\"/> containing the sprites in that layer that are in the named category. Even categories without any members are represented in order to allow this common base class to provide the same set of categories universally regardless of the specific layer to which it applies.</remarks>", true)
            });
         clsLayerSpriteCategoriesBase.IsPartial = true;

         foreach(System.Data.DataRowView drv in ProjectData.SpriteCategory.DefaultView)
         {
            ProjectDataset.SpriteCategoryRow drCat = (ProjectDataset.SpriteCategoryRow)drv.Row;
            CodeMemberField fldCat = new CodeMemberField(SpriteCollectionClassName, "m_" + NameToVariable(drCat.Name));
            fldCat.Attributes = MemberAttributes.Private;
            fldCat.InitExpression = new CodePrimitiveExpression(null);
            clsLayerSpriteCategoriesBase.Members.Add(fldCat);

            CodeMemberProperty prpCat = new CodeMemberProperty();
            prpCat.Name = NameToVariable(drCat.Name);
            prpCat.Type = new CodeTypeReference(SpriteCollectionClassName);
            prpCat.Attributes = MemberAttributes.Public;
            prpCat.HasGet = true;
            prpCat.HasSet = false;
            CodeFieldReferenceExpression refCat = new CodeFieldReferenceExpression(
               new CodeThisReferenceExpression(), "m_" + NameToVariable(drCat.Name));
            prpCat.GetStatements.Add(new CodeConditionStatement(
               new CodeBinaryOperatorExpression(refCat,
               CodeBinaryOperatorType.IdentityEquality,
               new CodePrimitiveExpression(null)),
               new CodeAssignStatement(refCat,
               new CodeObjectCreateExpression(
               SpriteCollectionClassName))));
            prpCat.GetStatements.Add(new CodeMethodReturnStatement(refCat));
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
         clsSolidity.Comments.AddRange(new CodeCommentStatement[]
            {
               new CodeCommentStatement("<summary>Represents solidity definitions in the project</summary>", true),
               new CodeCommentStatement("<remarks>This class is generated based on solidity definitions defined in the project. A solidity definition defines a set of \"rules\" that associates tiles with the shapes that they should assume when these rules are active. The \"shape\" of a tile determines how sprites react to it. Each individual solidity definition is represented as a static member of this class whose name is based on the name assigned at design-time, which returns an instance of this class</remarks>", true)
            });
         clsSolidity.IsPartial = true;

         CodeMemberMethod mthGetTileShape = new CodeMemberMethod();
         mthGetTileShape.Name = "GetCurrentTileShape";
         mthGetTileShape.ReturnType = new CodeTypeReference("TileShape");
         mthGetTileShape.Attributes = MemberAttributes.Public | MemberAttributes.Final;
         mthGetTileShape.Parameters.Add(new CodeParameterDeclarationExpression(
            TileBaseClass, "tile"));
         clsSolidity.Members.Add(mthGetTileShape);
         mthGetTileShape.Comments.AddRange(new CodeCommentStatement[]
            {
               new CodeCommentStatement("<summary>Retrieve the solid shape of the specified tile according to the rules of this solidity definition</summary>", true),
               new CodeCommentStatement("<param name=\"tile\"><see cref=\"" + TileBaseClass + "\"/> object providing the necessary information to look it up in this solidity definition based on which category the tile is in.</param>", true),
               new CodeCommentStatement("<remarks>If the tile is in multiple categories that cause it to be associated with multiple tile shapes, the first shape that matches one of the specified tile's categories will be returned. The reason the name of this method includes \"Current\" is because a tile's shape may change over time if its categorization is based on frames contained in the tile, which are linked to a counter.</remarks>", true)
            });

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
         prpUndefined.Comments.Add(new CodeCommentStatement("<summary>Used for all sprites that don't specify some other solidity, and yields an empty shape for all tiles.</summary>", true));

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
                     new CodeTypeReferenceExpression(TileCategoryNameClass),
                     NameToVariable(drShape.TileCategoryRow.Name)),
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

      public void GenerateVSProject(System.IO.TextWriter txt)
      {
         XmlDocument xml = new XmlDocument();
         XmlElement root = xml.CreateElement("Project");
         xml.AppendChild(root);
         root.SetAttribute("DefaultTargets", "Build");
         root.SetAttribute("ToolsVersion", "3.5");
         root.SetAttribute("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003");
         XmlElement mainGroup = xml.CreateElement("PropertyGroup");
         root.AppendChild(mainGroup);
         AppendElem(mainGroup, "ProjectType", "Local");
         AppendElem(mainGroup, "ProductVersion", "9.0.21022");
         AppendElem(mainGroup, "SchemaVersion", "2.0");
         AppendElem(mainGroup, "Configuration", "Debug",
            new System.Collections.Generic.KeyValuePair<string, string>("Condition", " '$(Configuration)' == '' "));
         AppendElem(mainGroup, "Platform", "x86",
            new System.Collections.Generic.KeyValuePair<string, string>("Condition", " '$(Platform)' == '' "));
         AppendElem(mainGroup, "AssemblyName", System.IO.Path.GetFileNameWithoutExtension(SGDK2IDE.CurrentProjectFile));
         AppendElem(mainGroup, "OutputType", "WinExe");
         AppendElem(mainGroup, "TargetFrameworkVersion", "2.0");

         XmlElement debugGroup = xml.CreateElement("PropertyGroup");
         XmlElement releaseGroup = xml.CreateElement("PropertyGroup");
         root.AppendChild(debugGroup);
         root.AppendChild(releaseGroup);
         debugGroup.SetAttribute("Condition", " '$(Configuration)|$(Platform)' == 'Debug|x86' ");
         releaseGroup.SetAttribute("Condition", " '$(Configuration)|$(Platform)' == 'Release|x86' ");
         AppendElem(debugGroup, "PlatformTarget", "x86");
         AppendElem(releaseGroup, "PlatformTarget", "x86");
         AppendElem(debugGroup, "OutputPath", @".\");
         AppendElem(releaseGroup, "OutputPath", @".\");
         AppendElem(debugGroup, "AllowUnsafeBlocks", "false");
         AppendElem(releaseGroup, "AllowUnsafeBlocks", "false");
         AppendElem(debugGroup, "CheckForOverflowUnderflow", "false");
         AppendElem(releaseGroup, "CheckforOverflowUnderflow", "false");
         if (SGDK2IDE.CurrentProjectFile != null)
         {
            AppendElem(debugGroup, "DocumentationFile", System.IO.Path.GetFileNameWithoutExtension(SGDK2IDE.CurrentProjectFile) + ".xml");
            AppendElem(releaseGroup, "DocumentationFile", System.IO.Path.GetFileNameWithoutExtension(SGDK2IDE.CurrentProjectFile) + ".xml");
         }
         AppendElem(debugGroup, "DebugSymbols", "true");
         AppendElem(releaseGroup, "DebugSymbols", "false");
         AppendElem(debugGroup, "DefineConstants", "DEBUG");
         AppendElem(debugGroup, "FileAlignment", "4096");
         AppendElem(releaseGroup, "FileAlignment", "4096");
         AppendElem(debugGroup, "NoStdLib", "false");
         AppendElem(releaseGroup, "NoStdLib", "false");
         AppendElem(debugGroup, "Optimize", "false");
         AppendElem(releaseGroup, "Optimize", "true");
         AppendElem(debugGroup, "RegisterForComInterop", "false");
         AppendElem(releaseGroup, "RegisterForComInterop", "false");
         AppendElem(debugGroup, "RemoveIntegerChecks", "false");
         AppendElem(releaseGroup, "RemoveIntegerChecks", "false");
         AppendElem(debugGroup, "TreatWarningsAsErrors", "false");
         AppendElem(releaseGroup, "TreatWarningsAsErrors", "false");
         AppendElem(debugGroup, "WarningLevel", "1");
         AppendElem(releaseGroup, "WarningLevel", "1");
         AppendElem(debugGroup, "DebugType", "full");
         AppendElem(releaseGroup, "DebugType", "none");
         AppendElem(debugGroup, "ErrorReport", "prompt");
         AppendElem(releaseGroup, "ErrorReport", "prompt");

         XmlElement references = xml.CreateElement("ItemGroup");
         root.AppendChild(references);
         foreach(string refName in new string[]
            {
               "System.Drawing",
               "System.Windows.Forms",
               "System"
            })
         {
            XmlElement assyRef = xml.CreateElement("Reference");
            references.AppendChild(assyRef);
            assyRef.SetAttribute("Include", refName);
            AppendElem(assyRef, "Name", refName);
         }
         foreach(System.Data.DataRowView drv in ProjectData.SourceCode.DefaultView)
         {
            ProjectDataset.SourceCodeRow drCode = (ProjectDataset.SourceCodeRow)drv.Row;
            string fullPath = FindFullPath(drCode.Name);
            if (drCode.Name.EndsWith(".dll") && !IsOldDll(fullPath))
            {
               string assyName = null;
               if (System.IO.File.Exists(fullPath))
                  assyName = System.Reflection.AssemblyName.GetAssemblyName(fullPath).FullName;
               else
                  assyName = System.IO.Path.GetFileNameWithoutExtension(fullPath);

               XmlElement assyRef = xml.CreateElement("Reference");
               references.AppendChild(assyRef);
               assyRef.SetAttribute("Include", assyName);
               AppendElem(assyRef, "Name", assyName);
               AppendElem(assyRef, "SpecificVersion", "False");
               AppendElem(assyRef, "HintPath", fullPath);
            }
         }

         System.Xml.XmlElement files = xml.CreateElement("ItemGroup");
         root.AppendChild(files);

         int filePos = SGDK2IDE.CurrentProjectFile.IndexOf(System.IO.Path.GetFileName(SGDK2IDE.CurrentProjectFile));
         foreach(string filename in GetCodeFileList(System.IO.Path.GetDirectoryName(SGDK2IDE.CurrentProjectFile)))
         {
            XmlElement compileUnit = xml.CreateElement("Compile");
            compileUnit.SetAttribute("Include", filename.Substring(filePos));
            files.AppendChild(compileUnit);
         }

         string cfgFile = GetIntermediateConfigFile(System.IO.Path.GetDirectoryName(SGDK2IDE.CurrentProjectFile));
         if (cfgFile != null)
         {
            XmlElement compileUnit = xml.CreateElement("None");
            compileUnit.SetAttribute("Include", cfgFile.Substring(filePos));
            files.AppendChild(compileUnit);
         }

         foreach(string filename in GetEmbeddedResourceList(System.IO.Path.GetDirectoryName(SGDK2IDE.CurrentProjectFile)))
         {
            System.Xml.XmlElement file = xml.CreateElement("EmbeddedResource");
            files.AppendChild(file);
            file.SetAttribute("Include", filename.Substring(filePos));
         }

         foreach(string resx in GetResxFileList(System.IO.Path.GetDirectoryName(SGDK2IDE.CurrentProjectFile)))
         {
            System.Xml.XmlElement file = xml.CreateElement("EmbeddedResource");
            files.AppendChild(file);
            System.Xml.XmlElement dep = xml.CreateElement("DependentUpon");
            file.AppendChild(dep);
            file.SetAttribute("Include", resx.Substring(filePos));
            dep.InnerText = System.IO.Path.GetFileNameWithoutExtension(resx) + ".cs";
         }

         XmlElement bootStrap = xml.CreateElement("ItemGroup");
         root.AppendChild(bootStrap);
         XmlElement pkg = xml.CreateElement("BootstrapperPackage");
         bootStrap.AppendChild(pkg);
         pkg.SetAttribute("Include", "Microsoft.Net.Framework.2.0");
         AppendElem(pkg, "Visible", "False");
         AppendElem(pkg, "ProductName", ".NET Framework 2.0 %28x86%29");
         AppendElem(pkg, "Install", "false");
         XmlElement importProj = xml.CreateElement("Import");
         importProj.SetAttribute("Project", @"$(MSBuildBinPath)\Microsoft.CSharp.targets");
         root.AppendChild(importProj);
         System.Xml.XmlTextWriter xw = new System.Xml.XmlTextWriter(txt);
         xw.Indentation = 2;
         xw.IndentChar = ' ';
         xw.Formatting = System.Xml.Formatting.Indented;
         xml.WriteContentTo(xw);
      }

      public void GenerateMDProject(System.IO.TextWriter txt)
      {
         XmlDocument xml = new XmlDocument();
         XmlElement root = xml.CreateElement("Project");
         xml.AppendChild(root);
         string prjName = System.IO.Path.GetFileNameWithoutExtension(SGDK2IDE.CurrentProjectFile);
         root.SetAttribute("name", prjName);
         root.SetAttribute("fileversion", "2.0");
         root.SetAttribute("language", "C#");
         root.SetAttribute("clr-version", "Net_2_0");
         root.SetAttribute("ctype", "DotNetProject");
         XmlElement configurations = xml.CreateElement("Configurations");
         root.AppendChild(configurations);
         configurations.SetAttribute("active", "Debug");

         XmlElement debugCfg = xml.CreateElement("Configuration");
         debugCfg.SetAttribute("name", "Debug");
         debugCfg.SetAttribute("ctype", "DotNetProjectConfiguration");
         XmlElement releaseCfg = xml.CreateElement("Configuration");
         releaseCfg.SetAttribute("name", "Release");
         releaseCfg.SetAttribute("ctype", "DotNetProjectConfiguration");
         configurations.AppendChild(debugCfg);
         configurations.AppendChild(releaseCfg);

         XmlElement cfgOut = xml.CreateElement("Output");
         debugCfg.AppendChild(cfgOut);
         cfgOut.SetAttribute("directory", ".");
         cfgOut.SetAttribute("assembly", prjName);
         XmlElement cfgBuild = xml.CreateElement("Build");
         debugCfg.AppendChild(cfgBuild);
         cfgBuild.SetAttribute("debugmode", "True");
         cfgBuild.SetAttribute("target", "Exe");
         XmlElement cfgExec = xml.CreateElement("Execution");
         debugCfg.AppendChild(cfgExec);
         cfgExec.SetAttribute("runwithwarnings", "True");
         cfgExec.SetAttribute("consolepause", "True");
         cfgExec.SetAttribute("runtime", "MsNet");
         cfgExec.SetAttribute("clr-version", "Net_2_0");
         XmlElement cfgCodeGen = xml.CreateElement("CodeGeneration");
         debugCfg.AppendChild(cfgCodeGen);
         cfgCodeGen.SetAttribute("compiler", "Mcs");
         cfgCodeGen.SetAttribute("warninglevel", "4");
         cfgCodeGen.SetAttribute("optimize", "True");
         cfgCodeGen.SetAttribute("unsafecodeallowed", "False");
         cfgCodeGen.SetAttribute("generateoverflowchecks", "True");
         cfgCodeGen.SetAttribute("mainclass", string.Empty);
         cfgCodeGen.SetAttribute("definesymbols", "DEBUG");
         cfgCodeGen.SetAttribute("generatexmldocumentation", "False");
         cfgCodeGen.SetAttribute("win32Icon", ".");
         cfgCodeGen.SetAttribute("ctype", "CSharpCompilerParameters");

         releaseCfg.AppendChild(cfgOut.Clone());
         cfgOut.SetAttribute("directory", ".");
         cfgOut.SetAttribute("assembly", prjName);
         cfgBuild = xml.CreateElement("Build");
         releaseCfg.AppendChild(cfgBuild);
         cfgBuild.SetAttribute("debugmode", "False");
         cfgBuild.SetAttribute("target", "Exe");
         releaseCfg.AppendChild(cfgExec);
         cfgCodeGen = (XmlElement)cfgCodeGen.Clone();
         releaseCfg.AppendChild(cfgCodeGen);
         cfgCodeGen.SetAttribute("definesymbols", string.Empty);

         System.Xml.XmlElement files = xml.CreateElement("Contents");
         root.AppendChild(files);

         int filePos = SGDK2IDE.CurrentProjectFile.IndexOf(System.IO.Path.GetFileName(SGDK2IDE.CurrentProjectFile));
         foreach(string filename in GetCodeFileList(System.IO.Path.GetDirectoryName(SGDK2IDE.CurrentProjectFile)))
         {
            XmlElement compileUnit = xml.CreateElement("File");
            compileUnit.SetAttribute("name", filename.Substring(filePos).Replace(System.IO.Path.DirectorySeparatorChar, '/'));
            compileUnit.SetAttribute("subtype", "Code");
            compileUnit.SetAttribute("buildaction", "Compile");
            files.AppendChild(compileUnit);
         }

         string cfgFile = GetIntermediateConfigFile(System.IO.Path.GetDirectoryName(SGDK2IDE.CurrentProjectFile));
         if (cfgFile != null)
         {
            XmlElement compileUnit = xml.CreateElement("File");
            compileUnit.SetAttribute("name", cfgFile.Substring(filePos));
            compileUnit.SetAttribute("subtype", "Code");
            compileUnit.SetAttribute("buildaction", "FileCopy");
            files.AppendChild(compileUnit);
         }

         foreach(string filename in GetEmbeddedResourceList(System.IO.Path.GetDirectoryName(SGDK2IDE.CurrentProjectFile)))
         {
            System.Xml.XmlElement file = xml.CreateElement("File");
            files.AppendChild(file);
            file.SetAttribute("name", filename.Substring(filePos));
            file.SetAttribute("subtype", "Code");
            file.SetAttribute("buildaction", "EmbedAsResource");
         }

         foreach(string resx in GetResxFileList(System.IO.Path.GetDirectoryName(SGDK2IDE.CurrentProjectFile)))
         {
            System.Xml.XmlElement file = xml.CreateElement("File");
            files.AppendChild(file);
            file.SetAttribute("name", resx.Substring(filePos));
            file.SetAttribute("subtype", "Code");
            file.SetAttribute("buildaction", "EmbedAsResource");
            file.SetAttribute("resource_id", System.IO.Path.GetFileNameWithoutExtension(resx) + ".resources");
         }

         XmlElement references = xml.CreateElement("References");
         root.AppendChild(references);
         foreach (string refName in new string[]
            {
               "System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
               "System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
               "System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
            })
         {
            XmlElement assyRef = xml.CreateElement("ProjectReference");
            references.AppendChild(assyRef);
            assyRef.SetAttribute("type", "Gac");
            assyRef.SetAttribute("localcopy", "False");
            assyRef.SetAttribute("refto", refName);
         }

         foreach (System.Data.DataRowView drv in ProjectData.SourceCode.DefaultView)
         {
            ProjectDataset.SourceCodeRow drCode = (ProjectDataset.SourceCodeRow)drv.Row;
            string fullPath = FindFullPath(drCode.Name);
            if (drCode.Name.EndsWith(".dll") && !IsOldDll(fullPath))
            {
               XmlElement assyRef = xml.CreateElement("ProjectReference");
               references.AppendChild(assyRef);
               assyRef.SetAttribute("type", "Assembly");
               assyRef.SetAttribute("localcopy", "True");
               assyRef.SetAttribute("refto", System.IO.Path.GetFileName(fullPath));
            }
         }

         System.Xml.XmlTextWriter xw = new System.Xml.XmlTextWriter(txt);
         xw.Indentation = 2;
         xw.IndentChar = ' ';
         xw.Formatting = System.Xml.Formatting.Indented;
         xml.WriteContentTo(xw);
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
         string parentDir = System.Windows.Forms.Application.StartupPath;
         do
         {
            result = System.IO.Path.Combine(parentDir, System.IO.Path.GetFileName(fileName));
         } while (!System.IO.File.Exists(result) && (null != (System.IO.Directory.GetParent(parentDir))) && (null != (parentDir=System.IO.Directory.GetParent(parentDir).FullName)));

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
      private XmlElement AppendElem(XmlElement parent, string name, string value, params System.Collections.Generic.KeyValuePair<string, string>[] attribs)
      {
         XmlElement result = parent.OwnerDocument.CreateElement(name);
         result.InnerText = value;
         foreach (var attrib in attribs)
            result.SetAttribute(attrib.Key, attrib.Value);
         parent.AppendChild(result);
         return result;
      }
      #endregion

      #region Compilation
      public string GetTempAssemblyPath()
      {
         return System.Windows.Forms.Application.UserAppDataPath;
      }

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
            System.CodeDom.Compiler.CompilerParameters compilerParams = new System.CodeDom.Compiler.CompilerParameters(new string[] {}, System.IO.Path.Combine(GetTempAssemblyPath(), "SGDK2Tmp.dll"));
            int idx = 0;
            string reason = string.Empty;
            do
            {
               try
               {
                  using (System.IO.File.OpenWrite(compilerParams.OutputAssembly))
                  {}
                  break;
               }
               catch(System.Exception ex)
               {
                  compilerParams.OutputAssembly = System.IO.Path.Combine(GetTempAssemblyPath(), "SGDK2Tm" + (++idx).ToString() + ".dll");
                  reason = ex.Message;
               }
            } while(idx < 5);

            if (idx >= 5)
               return "Unable to write temporary assembly to " + System.Windows.Forms.Application.StartupPath + "\r\n" + reason;

            compilerParams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Drawing.dll");
            compilerParams.ReferencedAssemblies.Add(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Reflect.dll"));
            foreach(System.Data.DataRowView drv in ProjectData.SourceCode.DefaultView)
            {
               ProjectDataset.SourceCodeRow drCode = (ProjectDataset.SourceCodeRow)drv.Row;
               string fullPath = FindFullPath(drCode.Name);
               if (drCode.Name.EndsWith(".dll") && !IsOldDll(fullPath))
                  compilerParams.ReferencedAssemblies.Add(fullPath);
            }
            compilerParams.GenerateExecutable = false;
            System.CodeDom.Compiler.CompilerResults results = codeProvider.CompileAssemblyFromSource(compilerParams, code);
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

      public class ObjectErrorInfo
      {
         public System.Data.DataRow source { get; private set; }
         public CompilerError error { get; private set; }

         public ObjectErrorInfo(System.Data.DataRow source, CompilerError error)
         {
            this.source = source;
            this.error = error;
         }

         public string GetSourceType()
         {
            if (source is ProjectDataset.PlanRuleRow)
               return "Plan Rule";
            else if (source is ProjectDataset.SpriteRuleRow)
               return "Sprite Rule";
            else if (source is ProjectDataset.SourceCodeRow)
               return "Source Code";
            else
               return source.GetType().Name;
         }

         public string GetSourceName()
         {
            if (source is ProjectDataset.PlanRuleRow)
            {
               ProjectDataset.PlanRuleRow drPlanRule = (ProjectDataset.PlanRuleRow)source;
               return drPlanRule.MapName + "/" + drPlanRule.LayerName + "/" + drPlanRule.PlanName + "/" + drPlanRule.Name;
            }
            else if (source is ProjectDataset.SpriteRuleRow)
            {
               ProjectDataset.SpriteRuleRow drSpriteRule = (ProjectDataset.SpriteRuleRow)source;
               return drSpriteRule.DefinitionName + "/" + drSpriteRule.Name;
            }
            else if (source is ProjectDataset.SourceCodeRow)
            {
               return ((ProjectDataset.SourceCodeRow)source).Name;
            }
            else
               return null;
         }

         public string Message
         {
            get
            {
               return error.ErrorText;
            }
         }
      }

      public string CompileProject(string ProjectName, string FolderName, out string errs, out System.Collections.Generic.IEnumerable<ObjectErrorInfo> errorRules)
      {
         SGDK2IDE.PushStatus("Compiling " + ProjectName + " to " + FolderName, true);
         errorRules = null;
         var errorRows = new System.Collections.Generic.List<ObjectErrorInfo>();
         try
         {
            if (!System.IO.Path.IsPathRooted(FolderName))
               FolderName = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, FolderName);

            string[] fileList = GetCodeFileList(FolderName);
            string appCfgIntermediate = GetIntermediateConfigFile(FolderName);
            string appCfgOutput = System.IO.Path.Combine(FolderName, ProjectName + ".exe.config");
            string[] resourcesList = GetResourcesFileList(FolderName);
            System.Collections.ArrayList OldDlls = new System.Collections.ArrayList();
            GenerateAllCode(FolderName, out errs);
            if (errs.Length > 0)
               return null;

            string resourceSwitches = string.Empty;
            foreach (string resFile in resourcesList)
            {
               resourceSwitches += " /res:\"" + resFile + "\"";
            }

            CompileResources(FolderName);

            Microsoft.CSharp.CSharpCodeProvider codeProvider = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.CompilerParameters compilerParams = new System.CodeDom.Compiler.CompilerParameters(new string[] { }, System.IO.Path.Combine(FolderName, ProjectName + ".exe"));
            compilerParams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Drawing.dll");
            foreach (System.Data.DataRowView drv in ProjectData.SourceCode.DefaultView)
            {
               ProjectDataset.SourceCodeRow drCode = (ProjectDataset.SourceCodeRow)drv.Row;
               if (drCode.Name.EndsWith(".dll") || drCode.Name.EndsWith(".so"))
               {
                  string fullPath = FindFullPath(drCode.Name);
                  if (drCode.Name.EndsWith(".so") || IsOldDll(fullPath))
                  {
                     OldDlls.Add(fullPath);
                  }
                  else
                  {
                     if (System.IO.File.Exists(fullPath))
                     {
                        string targetPath = System.IO.Path.Combine(FolderName, System.IO.Path.GetFileName(drCode.Name));
                        System.IO.File.Copy(fullPath, targetPath, true);
                        if (System.IO.File.Exists(fullPath + ".config"))
                           System.IO.File.Copy(fullPath + ".config", targetPath + ".config", true);
                        compilerParams.ReferencedAssemblies.Add(targetPath);
                     }
                     else
                     {
                        compilerParams.ReferencedAssemblies.Add(drCode.Name);
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
               compilerParams.CompilerOptions = "/define:DEBUG /platform:x86 /target:winexe" + resourceSwitches;
            else
               compilerParams.CompilerOptions = "/platform:x86 /target:winexe" + resourceSwitches;

            string iconFile = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Prj.ico");
            while (true)
            {
               if (System.IO.File.Exists(iconFile))
               {
                  compilerParams.CompilerOptions += " /win32icon:\"" + iconFile + "\"";
                  break;
               }
               else
               {
                  System.IO.DirectoryInfo parentDir = System.IO.Directory.GetParent(System.IO.Path.GetDirectoryName(iconFile));
                  if (parentDir == null)
                     break;
                  iconFile = System.IO.Path.Combine(parentDir.FullName, System.IO.Path.GetFileName(iconFile));
               }
            }
            System.CodeDom.Compiler.CompilerResults results = codeProvider.CompileAssemblyFromFile(compilerParams, fileList);
            // MSBuild and Visual Studio automatically copy the config file, but
            // CSharpCodeProvider operates below that and we must copy it manually.
            if ((appCfgIntermediate != null) && System.IO.File.Exists(appCfgIntermediate))
               System.IO.File.Copy(appCfgIntermediate, appCfgOutput, true);
            if (results.Errors.Count > 0)
            {
               System.Text.StringBuilder sb = new System.Text.StringBuilder();
               for (int i = 0; i < results.Errors.Count; i++)
               {
                  sb.Append(results.Errors[i].ToString() + Environment.NewLine);
                  ProjectDataset.SpriteRuleRow errSpriteRule = GetSpriteRuleErr(results.Errors[i]);
                  if (errSpriteRule != null)
                  {
                     errorRows.Add(new ObjectErrorInfo(errSpriteRule, results.Errors[i]));
                  }
                  else
                  {
                     ProjectDataset.PlanRuleRow errPlanRule = GetPlanRuleErr(results.Errors[i]);
                     if (errPlanRule != null)
                     {
                        errorRows.Add(new ObjectErrorInfo(errPlanRule, results.Errors[i]));
                     }
                     else
                     {
                        ProjectDataset.SourceCodeRow errSrcCode = ProjectData.GetSourceCode(System.IO.Path.GetFileName(results.Errors[i].FileName));
                        if (errSrcCode != null)
                        {
                           errorRows.Add(new ObjectErrorInfo(errSrcCode, results.Errors[i]));
                        }
                     }
                  }
               }
               errorRules = errorRows.ToArray();
               errs = sb.ToString();
               return null;
            }
            else
            {
               foreach (string dllFile in OldDlls)
               {
                  System.IO.File.Copy(dllFile,
                     System.IO.Path.Combine(System.IO.Path.GetDirectoryName(results.PathToAssembly),
                     System.IO.Path.GetFileName(dllFile)), true);
               }
            }
            return compilerParams.OutputAssembly;
         }
         catch (System.IO.IOException ex)
         {
            errs = "A failure accessing files occurred; make sure your project is not already running." + Environment.NewLine +
               ex.Message;
            return null;
         }
         finally
         {
            SGDK2IDE.PopStatus();
         }
      }

      private ProjectDataset.SpriteRuleRow GetSpriteRuleErr(CompilerError err)
      {
         string errFile = System.IO.Path.GetFileName(err.FileName);
         string errDir = System.IO.Path.GetDirectoryName(err.FileName);
         if (!errDir.EndsWith(SpritesDir))
            return null;
         foreach (System.Data.DataRowView drv in ProjectData.SpriteDefinition.DefaultView)
         {
            ProjectDataset.SpriteDefinitionRow drSpriteDef = (ProjectDataset.SpriteDefinitionRow)drv.Row;
            string spriteFileName = NameToVariable(drSpriteDef.Name) + ".cs";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(err.FileName))
            {
               System.Collections.Generic.List<string> fileLines = new System.Collections.Generic.List<string>();
               if (errFile == spriteFileName)
               {
                  ProjectDataset.SpriteRuleRow[] spriteRules = ProjectData.GetSortedSpriteRules(drSpriteDef, false);
                  var ruleMap = System.Linq.Enumerable.ToDictionary(spriteRules, k => k.Name, v => v);
                  do
                  {
                     fileLines.Add(sr.ReadLine().TrimStart());
                  } while ((sr.EndOfStream == false) && (fileLines.Count < err.Line));
                  for (int i = fileLines.Count - 1; i > 0; i--)
                  {
                     string errLine = fileLines[fileLines.Count - 1];
                     if (fileLines[i].StartsWith("//"))
                     {
                        ProjectDataset.SpriteRuleRow spriteRule;
                        if (ruleMap.TryGetValue(fileLines[i].Substring(2).TrimStart(), out spriteRule))
                        {
                           System.Collections.Generic.List<string> funcParts = new System.Collections.Generic.List<string>();
                           int ruleIndex = spriteRule.Sequence;
                           while ((ruleIndex >= spriteRules.Length) || ((ruleIndex > 0) && (spriteRules[ruleIndex].Sequence > spriteRule.Sequence)))
                              ruleIndex--;
                           for (; ruleIndex < spriteRules.Length; ruleIndex++)
                           {
                              funcParts.Add(spriteRules[ruleIndex].Function);
                              if (!spriteRules[ruleIndex].IsParameter1Null()) funcParts.Add(spriteRules[ruleIndex].Parameter1);
                              if (!spriteRules[ruleIndex].IsParameter2Null()) funcParts.Add(spriteRules[ruleIndex].Parameter2);
                              if (!spriteRules[ruleIndex].IsParameter3Null()) funcParts.Add(spriteRules[ruleIndex].Parameter3);
                              if (!spriteRules[ruleIndex].IsResultParameterNull()) funcParts.Add(spriteRules[ruleIndex].ResultParameter);
                              bool found = true;
                              foreach (string fp in funcParts)
                                 if (!errLine.Contains(fp))
                                 {
                                    found = false;
                                    break;
                                 }
                              if (found)
                                 return spriteRules[ruleIndex];
                              funcParts.Clear();
                           }
                           return spriteRule;
                        }
                     }
                  }
               }
            }
         }
         return null;
      }
      private ProjectDataset.PlanRuleRow GetPlanRuleErr(CompilerError err)
      {
         string errFile = System.IO.Path.GetFileName(err.FileName);
         string errDir = System.IO.Path.GetDirectoryName(err.FileName);
         foreach (System.Data.DataRowView drv in ProjectData.Map.DefaultView)
         {
            ProjectDataset.MapRow drMap = (ProjectDataset.MapRow)drv.Row;
            string fileName = NameToVariable(((ProjectDataset.MapRow)drMap).Name) + "_Map.cs";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(err.FileName))
            {
               System.Collections.Generic.List<string> fileLines = new System.Collections.Generic.List<string>();
               if (errFile == fileName)
               {
                  System.Text.RegularExpressions.Regex reLayerDecl = new System.Text.RegularExpressions.Regex(@"class (\S+)_Lyr\s*\:\s*\S*Layer$");
                  System.Text.RegularExpressions.Regex rePlanDecl = new System.Text.RegularExpressions.Regex(@"class (\S+)\s*\:\s*PlanBase$");
                  string curLayer = null;
                  string curPlan = null;
                  int lineNum = 0;
                  do
                  {
                     string lineText = sr.ReadLine().TrimStart();
                     lineNum++;
                     fileLines.Add(lineText);

                     System.Text.RegularExpressions.Match match = reLayerDecl.Match(lineText);
                     if (match.Success)
                     {
                        fileLines.Clear();
                        curLayer = match.Groups[1].Captures[0].Value;
                     }
                     else
                     {
                        match = rePlanDecl.Match(lineText);
                        if (match.Success)
                        {
                           fileLines.Clear();
                           curPlan = match.Groups[1].Captures[0].Value;
                        }
                     }
                  } while ((sr.EndOfStream == false) && (lineNum < err.Line));

                  ProjectDataset.SpritePlanRow drSpritePlan = null;
                  foreach (ProjectDataset.LayerRow lyr in drMap.GetLayerRows())
                     if ((drSpritePlan == null) && (NameToVariable(lyr.Name) == curLayer))
                        foreach (ProjectDataset.SpritePlanRow pln in lyr.GetSpritePlanRows())
                           if ((drSpritePlan == null) && (NameToVariable(pln.Name) == curPlan))
                              drSpritePlan = pln;
                  string errLine = fileLines[fileLines.Count - 1];
                  ProjectDataset.PlanRuleRow[] planRules = ProjectData.GetSortedPlanRules(drSpritePlan, false);
                  var ruleMap = System.Linq.Enumerable.ToDictionary(planRules, k => k.Name, v => v);
                  for (int i = fileLines.Count - 1; i > 0; i--)
                  {
                     if (fileLines[i].TrimStart().StartsWith("//"))
                     {
                        ProjectDataset.PlanRuleRow planRule;
                        if (ruleMap.TryGetValue(fileLines[i].Substring(2).TrimStart(), out planRule))
                        {
                           System.Collections.Generic.List<string> funcParts = new System.Collections.Generic.List<string>();
                           int ruleIndex = planRule.Sequence;
                           while ((ruleIndex >= planRules.Length) || ((ruleIndex > 0) && (planRules[ruleIndex].Sequence > planRule.Sequence)))
                              ruleIndex--;
                           for (; ruleIndex < planRules.Length; ruleIndex++)
                           {
                              funcParts.Add(planRules[ruleIndex].Function);
                              if (!planRules[ruleIndex].IsParameter1Null()) funcParts.Add(planRules[ruleIndex].Parameter1);
                              if (!planRules[ruleIndex].IsParameter2Null()) funcParts.Add(planRules[ruleIndex].Parameter2);
                              if (!planRules[ruleIndex].IsParameter3Null()) funcParts.Add(planRules[ruleIndex].Parameter3);
                              if (!planRules[ruleIndex].IsResultParameterNull()) funcParts.Add(planRules[ruleIndex].ResultParameter);
                              bool found = true;
                              foreach (string fp in funcParts)
                                 if (!errLine.Contains(fp))
                                 {
                                    found = false;
                                    break;
                                 }
                              if (found)
                                 return planRules[ruleIndex];
                              funcParts.Clear();
                           }
                           return planRule;
                        }
                     }
                  }
               }
            }
         }
         return null;
      }
      #endregion

      #region HTML5 Code Generation
      public string[] GetHtmlFileList(string htmlFileName)
      {
         string FolderName = System.IO.Path.GetDirectoryName(htmlFileName);
         string ProjectName = System.IO.Path.GetFileNameWithoutExtension(htmlFileName);
         if (!System.IO.Path.IsPathRooted(FolderName))
         {
            FolderName = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, FolderName);
         }
         System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>();
         result.Add(System.IO.Path.Combine(FolderName, ProjectName + ".html"));
         result.Add(System.IO.Path.Combine(FolderName, ProjectName + ".js"));
         foreach (System.Data.DataRowView drv in ProjectData.GraphicSheet.DefaultView)
         {
            ProjectDataset.GraphicSheetRow drGfx = (ProjectDataset.GraphicSheetRow)drv.Row;
            string gsFileName = System.IO.Path.Combine(FolderName, drGfx.Name + ".png");
            result.Add(gsFileName);
         }
         return result.ToArray();
      }

      [Flags()]
      public enum HtmlGeneratorOptions
      {
         SingleFile=1,
         FillBrowser=2,
         GenerateMapButtons=4,
         CamelCase=8
      }
      private HtmlGeneratorOptions options;

      public string GenerateHtml5(string htmlFileName, HtmlGeneratorOptions options, out string errs, out System.Collections.Generic.IEnumerable<ObjectErrorInfo> errorRules)
      {
         System.IO.TextWriter err = new System.IO.StringWriter();
         errorRules = null;
         var errorRows = new System.Collections.Generic.List<ObjectErrorInfo>();
         try
         {
            string FolderName = System.IO.Path.GetDirectoryName(htmlFileName);
            if (!System.IO.Path.IsPathRooted(FolderName))
            {
               FolderName = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, FolderName);
            }
            if (!System.IO.Directory.Exists(FolderName))
               System.IO.Directory.CreateDirectory(FolderName);
            string ProjectName = System.IO.Path.GetFileNameWithoutExtension(htmlFileName);
            this.options = options;
            using (System.IO.StreamWriter txt = new System.IO.StreamWriter(htmlFileName))
            {
               GenerateHtmlProject(ProjectName, txt, err);
               txt.Close();
            }
            if (0 == (options & HtmlGeneratorOptions.SingleFile))
            {
               string jsFileName = System.IO.Path.Combine(FolderName, ProjectName + ".js");
               using (System.IO.StreamWriter txt = new System.IO.StreamWriter(jsFileName))
               {
                  GenerateHtmlJavascript(txt, err);
               }
               foreach (System.Data.DataRowView drv in ProjectData.GraphicSheet.DefaultView)
               {
                  ProjectDataset.GraphicSheetRow drGfx = (ProjectDataset.GraphicSheetRow)drv.Row;
                  string gsFileName = System.IO.Path.Combine(FolderName, drGfx.Name + ".png");
                  ProjectData.GetGraphicSheetImage(drGfx.Name, false).Save(gsFileName);
               }
            }
            errs = err.ToString();
            err.Close();
            if (errorRows.Count > 0)
               errorRules = errorRows.ToArray();
            return htmlFileName;
         }
         catch (System.Exception ex)
         {
            errs = ex.Message + "\r\n" + err.ToString();
            err.Close();
            if (errorRows.Count > 0)
               errorRules = errorRows.ToArray();
            return null;
         }
      }

      private string GetJavascriptFile(string name)
      {
         ProjectDataset.SourceCodeRow sc = ProjectData.GetSourceCode(name);
         if (sc != null) return sc.Text;
         string code;
         using (System.IO.StreamReader sr = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SGDK2.HTML5." + name)))
         {
            code = sr.ReadToEnd();
         }
         ProjectData.AddSourceCode(name, code, null, false, null);
         return code;
      }

      private void GenerateHtmlProject(string ProjectName, System.IO.TextWriter txt, System.IO.TextWriter err)
      {
         System.Drawing.Size dispSize = Display.GetScreenSize((GameDisplayMode)Enum.Parse(typeof(GameDisplayMode), ProjectData.ProjectRow.DisplayMode));
         txt.WriteLine("<!DOCTYPE html>\r\n" +
            "<html>\r\n" +
            "<head>\r\n" +
            "<meta charset=\"utf-8\" />\r\n" +
            "<title>" + ProjectName + "</title>\r\n" +
            "<style>\r\n" +
            "   .unselectable {\r\n" +
            "      user-select:none;\r\n" +
            "      -moz-user-select:-moz-none;\r\n" +
            "      -khtml-user-select:none;\r\n" +
            "      -webkit-user-select:none;\r\n" +
            "      -o-user-select:none;\r\n" +
            "   }\r\n" +
            "   canvas { display: block }\r\n" +
            (0 != (options & HtmlGeneratorOptions.FillBrowser) ? "   body { margin:0 }\r\n" : string.Empty) +
            "</style>\r\n");
         if (0 != (options & HtmlGeneratorOptions.SingleFile))
         {
            txt.WriteLine("<script type=\"text/javascript\">\r\n");
            GenerateHtmlJavascript(txt, err);
            txt.WriteLine("</script>\r\n");
         }
         else
         {
            txt.WriteLine("<script type=\"text/javascript\" src=\"" + ProjectName + ".js\"></script>");
         }
         txt.WriteLine("</head>\r\n" +
            "<body class=\"unselectable\" unselectable=\"on\"" + ((0 != (options & HtmlGeneratorOptions.FillBrowser))
            ? " onresize=\"resizeView()\"" : string.Empty) + "  onkeydown=\"keyboardState.handleKeyDown(event)\" onkeyup=\"keyboardState.handleKeyUp(event)\">\r\n" + 
            "<canvas id=\"gameView\" width=\"" + dispSize.Width.ToString() + "\" height=\"" + dispSize.Height.ToString() + "\" " +
            "onmousedown=\"beginDrag(event)\" ontouchstart=\"beginTouchDrag(event)\" onmouseup=\"endDrag(event)\" " +
            "onmouseout=\"endDrag(event)\" ontouchmove=\"processTouchDrag(event)\" " +
            "unselectable=\"on\" class=\"unselectable\">\r\n" +
            "   Your browser does not support HTML5 canvases.\r\n" +
            "</canvas>");
         foreach (System.Data.DataRowView drv in ProjectData.GraphicSheet.DefaultView)
         {
            ProjectDataset.GraphicSheetRow drGfx = (ProjectDataset.GraphicSheetRow)drv.Row;
            if (0 != (options & HtmlGeneratorOptions.SingleFile))
            {
               txt.Write("<img id=\"" + NameToVariable(drGfx.Name) + "\" style=\"display:none\" src=\"data:image/png;base64,");
               txt.Write(Convert.ToBase64String(ProjectData.GetGraphicSheet(drGfx.Name).Image, Base64FormattingOptions.InsertLineBreaks));
            }
            else
               txt.Write("<img id=\"" + NameToVariable(drGfx.Name) + "\" style=\"display:none\" src=\"" + drGfx.Name + ".png");
            txt.WriteLine("\" />");
         }
         if (0 != (options & HtmlGeneratorOptions.GenerateMapButtons))
            GenerateHtmlMapButtons(txt);
         txt.WriteLine("</body>\r\n" +
            "</html>");
      }

      private void GenerateHtmlJavascript(System.IO.TextWriter txt, System.IO.TextWriter err)
      {
         System.Drawing.Size dispSize = Display.GetScreenSize((GameDisplayMode)Enum.Parse(typeof(GameDisplayMode), ProjectData.ProjectRow.DisplayMode));
         txt.WriteLine(
            "var viewWidth = " + dispSize.Width.ToString() + ";\r\n" +
            "var viewHeight = " + dispSize.Height.ToString() + ";\r\n" +
            "function firstMap() {\r\n" +
            "   currentMap = " + NameToMapClass(ProjectData.ProjectRow.StartMap) + ";\r\n" +
            (string.IsNullOrEmpty(ProjectData.ProjectRow.OverlayMap)?string.Empty:"   overlayMap = " + NameToMapClass(ProjectData.ProjectRow.OverlayMap) + ";\r\n") +
            (0 != (options & HtmlGeneratorOptions.FillBrowser) ? "   resizeView();\r\n" : string.Empty) +
            "}");

         txt.WriteLine(GetJavascriptFile("Main.js"));

         GenerateHtmlGraphicSheets(txt);
         GenerateHtmlFramesets(txt);
         GenerateHtmlCounters(txt);
         GenerateHtmlTilesets(txt, err);
         txt.WriteLine(GetJavascriptFile("KeyboardState.js"));
         txt.WriteLine(GetJavascriptFile("Player.js"));
         txt.WriteLine(GetJavascriptFile("GeneralRules.js"));
         GenerateHtmlSprites(txt, err);
         GenerateHtmlTileCategories(txt);
         GenerateHtmlMaps(txt, err);
         txt.WriteLine("window.onload = startGame;");
      }

      private void GenerateHtmlGraphicSheets(System.IO.TextWriter txt)
      {
         txt.WriteLine("function GraphicSheet(image, cellWidth, cellHeight, columns, rows) {\r\n" +
            "   this.image = image;\r\n" +
            "   this.cellWidth = cellWidth;\r\n" +
            "   this.cellHeight = cellHeight;\r\n" +
            "   this.columns = columns;\r\n" +
            "   this.rows = rows;\r\n" +
            "}");
         txt.WriteLine("var graphicSheets;");
         txt.WriteLine("function initGraphicSheets() {\r\n" +
            "   graphicSheets = {");
         bool first = true;
         foreach (System.Data.DataRowView drv in ProjectData.GraphicSheet.DefaultView)
         {
            ProjectDataset.GraphicSheetRow drGfx = (ProjectDataset.GraphicSheetRow)drv.Row;
            if (first)
               txt.Write("      ");
            else
               txt.Write(",\r\n      ");
            txt.Write(NameToVariable(drGfx.Name) +
               ": new GraphicSheet(document.getElementById('" + NameToVariable(drGfx.Name) + "'), " +
               drGfx.CellWidth.ToString() + "," + drGfx.CellHeight.ToString() + "," +
               drGfx.Columns.ToString() + "," + drGfx.Rows.ToString() + ")");
            first = false;
         }
         txt.WriteLine("\r\n   };\r\n}");
      }

      private void GenerateHtmlFramesets(System.IO.TextWriter txt)
      {
         txt.WriteLine(GetJavascriptFile("Frame.js"));
         var coloredFrameMap = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<Tuple<int, short>, short>>();
         foreach (System.Data.DataRowView drv in ProjectData.Frameset.DefaultView)
         {
            ProjectDataset.FramesetRow drFrameset = (ProjectDataset.FramesetRow)drv.Row;
            foreach (ProjectDataset.FrameRow fr in ProjectData.GetSortedFrameRows(drFrameset))
            {
               if (fr.IscolorNull() || (fr.color == -1) || ((fr.color & 0xFF000000) == 0))
                  continue;
               System.Collections.Generic.Dictionary<Tuple<int, short>, short> mapping;
               if (!coloredFrameMap.TryGetValue(fr.GraphicSheet, out mapping))
               {
                  mapping = new System.Collections.Generic.Dictionary<Tuple<int, short>, short>();
                  coloredFrameMap[fr.GraphicSheet] = mapping;
               }
               var key = new Tuple<int, short>(fr.color, fr.CellIndex);
               if (!mapping.ContainsKey(key))
                  mapping[key] = (short)mapping.Count;
            }
         }

         txt.WriteLine("var frameSets = new Object();");
         txt.WriteLine("function initFramesets() {\r\n" +
            "   var ctx;\r\n" +
            "   var gfx;");
         foreach (var gsExt in coloredFrameMap)
         {
            var gs = ProjectData.GetGraphicSheet(gsExt.Key);
            txt.WriteLine("   gfx = graphicSheets." + NameToVariable(gs.Name) + ";");
            txt.WriteLine("   gfx.extra = document.createElement('canvas');\r\n" +
               "   gfx.extra.width = " + (gs.CellWidth * gs.Columns).ToString() + ";\r\n" +
               "   gfx.extra.height = " + ((int)Math.Ceiling((decimal)gsExt.Value.Count / gs.Columns) * gs.CellHeight).ToString() + ";\r\n" +
               "   ctx = gfx.extra.getContext('2d');\r\n");
            foreach (var ci in gsExt.Value)
            {
               int sx = (ci.Key.Item2 % gs.Columns) * gs.CellWidth;
               int sy = ((int)(ci.Key.Item2 / gs.Columns)) * gs.CellHeight;
               int tx = (ci.Value % gs.Columns) * gs.CellWidth;
               int ty = ((int)(ci.Value / gs.Columns)) * gs.CellHeight;
               byte[] cb = BitConverter.GetBytes(ci.Key.Item1);

               txt.WriteLine("   ctx.drawImage(gfx.image," + sx.ToString() + "," + sy.ToString() +
                  "," + gs.CellWidth.ToString() + "," + gs.CellHeight.ToString() + "," + 
                  tx.ToString() + "," + ty.ToString() + "," +
                  gs.CellWidth.ToString() + "," + gs.CellHeight.ToString() + ");");
               txt.WriteLine("   ModulateCelColor(ctx," + tx.ToString() + "," + ty.ToString() + "," +
                  gs.CellWidth.ToString() + "," + gs.CellHeight.ToString() + "," +
                  cb[2].ToString() + "," + cb[1].ToString() + "," + cb[0].ToString() + "," + cb[3].ToString() + ");");
            }
         }

         foreach (System.Data.DataRowView drv in ProjectData.Frameset.DefaultView)
         {
            ProjectDataset.FramesetRow drFrameset = (ProjectDataset.FramesetRow)drv.Row;
            txt.WriteLine("   frameSets." + NameToVariable(drFrameset.Name) + " = new Frameset('" + drFrameset.Name + "', [");
            bool first = true;
            foreach (ProjectDataset.FrameRow fr in ProjectData.GetSortedFrameRows(drFrameset))
            {
               System.Collections.Generic.Dictionary<Tuple<int, short>, short> mapping;
               string graphicSheetRef = "graphicSheets." + NameToVariable(fr.GraphicSheet);
               string imageSrcRef = graphicSheetRef;
               short cellIndex = fr.CellIndex;
               if (coloredFrameMap.TryGetValue(fr.GraphicSheet, out mapping))
               {
                  if (!fr.IscolorNull() && (fr.color != -1))
                  {
                     if ((fr.color & 0xFF000000) == 0)
                     {
                        imageSrcRef = "null";
                     } else {
                        imageSrcRef += ".extra";
                        cellIndex = mapping[new Tuple<int, short>(fr.color, fr.CellIndex)];
                     }
                  }
                  else
                  {
                     imageSrcRef += ".image";
                  }
               }
               else if (!fr.IscolorNull() && ((fr.color & 0xFF000000) == 0))
               {
                  // 0 alpha means don't draw anything
                  imageSrcRef = "null";
               }
               else
               {
                  imageSrcRef += ".image";
               }

               if (first)
                  txt.Write("      ");
               else
                  txt.Write(",\r\n      ");

               if (imageSrcRef == "null" ||
                  ((fr.m11 == 1) && (fr.m12 == 0) &&
                   (fr.m21 == 0) && (fr.m22 == 1) &&
                   (fr.dx == 0) && (fr.dy == 0)))
               {
                  txt.Write("new Frame(" + graphicSheetRef + "," + imageSrcRef + "," + cellIndex.ToString() + ")");
               }
               else
               {
                  txt.Write("new XFrame(" + fr.m11.ToString() + "," + fr.m12.ToString() + "," +
                     fr.m21.ToString() + "," + fr.m22.ToString() + "," + fr.dx.ToString() + "," +
                     fr.dy.ToString() + "," + graphicSheetRef + "," + imageSrcRef + "," + cellIndex.ToString() + ")");
               }
               first = false;
            }
            txt.WriteLine("]);");
         }
         txt.WriteLine("}");
      }

      private void GenerateHtmlCounters(System.IO.TextWriter txt)
      {
         txt.WriteLine("function Counter(value, min, max) {\r\n" +
            "   this.value = value;\r\n" +
            "   this.min = min;\r\n" +
            "   this.max = max;\r\n" +
            "}");
         txt.WriteLine("var counters = {");
         bool first = true;
         foreach (System.Data.DataRowView drv in ProjectData.Counter.DefaultView)
         {
            ProjectDataset.CounterRow drCounter = (ProjectDataset.CounterRow)drv.Row;
            if (first)
               txt.Write("   ");
            else
               txt.Write(",\r\n   ");

            txt.Write(NameToVariable(drCounter.Name) + ": new Counter(" + drCounter.Value.ToString() +
               ", " + drCounter.Min.ToString() + ", " + drCounter.Max.ToString() + ")");
            first = false;
         }
         txt.WriteLine("\r\n};");
      }

      private void GenerateHtmlTilesets(System.IO.TextWriter txt, System.IO.TextWriter err)
      {
         txt.WriteLine("function Tileset(name, tileWidth, tileHeight, frameSet, tiles) {\r\n" +
            "   this.name = name;\r\n" +
            "   this.tileWidth = tileWidth;\r\n" +
            "   this.tileHeight = tileHeight;\r\n" +
            "   this.frameSet = frameSet;\r\n" +
            "   this.tiles = tiles;\r\n" +
            "}");
         txt.WriteLine("function TileFrame(accumulatedDuration, subFrames) {\r\n" +
            "   this.accumulatedDuration = accumulatedDuration;\r\n" +
            "   this.subFrames = subFrames;\r\n" +
            "}");
         txt.WriteLine("function AnimTile(counter, frames) {\r\n" +
            "   this.counter = counter;\r\n" +
            "   this.frames = frames;\r\n" +
            "   this.totalDuration = frames[frames.length - 1].accumulatedDuration;\r\n" +
            "}");
         txt.WriteLine("AnimTile.prototype.getCurFrameIndex = function() {\r\n" +
            "   for(var i = 0; i < this.frames.length; i++) {\r\n" +
            "      if((this.counter.value % this.totalDuration) < this.frames[i].accumulatedDuration) return i;\r\n" +
            "   }\r\n" +
            "   return this.frames.length - 1;\r\n" +
            "};");
         txt.WriteLine("AnimTile.prototype.getCurFrames = function() {\r\n" +
            "   return this.frames[this.getCurFrameIndex()].subFrames;\r\n" +
            "};");
         txt.WriteLine("var tilesets = new Object();");
         txt.WriteLine("function initTilesets() {");
         foreach (System.Data.DataRowView drv in ProjectData.Tileset.DefaultView)
         {
            ProjectDataset.TilesetRow drTileset = (ProjectDataset.TilesetRow)drv.Row;
            ProjectDataset.TileRow[] arTiles = ProjectData.GetSortedTileRows(drTileset);
            int nCurIdx = 0;
            int nMax = 0;
            if (arTiles.Length > 0)
               nMax = arTiles[arTiles.Length - 1].TileValue;
            if (drTileset.FramesetRow.GetFrameRows().Length > nMax)
               nMax = drTileset.FramesetRow.GetFrameRows().Length - 1;
            txt.Write("   tilesets." + NameToVariable(drTileset.Name) + " = new Tileset('" +
               NameToVariable(drTileset.Name) + "'," +
               drTileset.TileWidth.ToString() + "," + drTileset.TileHeight.ToString() +
               ", frameSets." + NameToVariable(drTileset.Frameset) + ",[");
            int outputState = 0; // 0 = No previous output, 1 = Previous output wants to share line, 2 = previous output wants its own line
            for (int i = 0; i <= nMax; i++)
            {
               bool ownLine = false;
               int nFrameSequenceCount = 0;
               string TileExpr;
               if ((nCurIdx < arTiles.Length) && (arTiles[nCurIdx].TileValue == i))
               {
                  ProjectDataset.TileRow drTile = arTiles[nCurIdx];
                  nCurIdx++;
                  ProjectDataset.TileFrameRow[] arFrames = ProjectData.GetSortedTileFrames(drTile);
                  nFrameSequenceCount = arFrames.Length;
                  int nAccumulatedDuration = 0;
                  var frames = new System.Collections.Generic.List<Tuple<string, string>>();
                  System.Collections.ArrayList subFrames = null;
                  foreach (ProjectDataset.TileFrameRow drTileFrame in arFrames)
                  {
                     if (subFrames == null)
                        subFrames = new System.Collections.ArrayList();
                     subFrames.Add(drTileFrame.FrameValue.ToString());
                     if (drTileFrame.Duration > 0)
                     {
                        nAccumulatedDuration += drTileFrame.Duration;
                        if (subFrames.Count == 1)
                        {
                           frames.Add(new Tuple<string, string>(nAccumulatedDuration.ToString(), subFrames[0].ToString()));
                        }
                        else
                        {
                           frames.Add(new Tuple<string, string>(nAccumulatedDuration.ToString(), "[" + String.Join(",", subFrames.ToArray()) + "]"));
                        }
                        subFrames.Clear();
                     }
                  }
                  if (frames.Count == 0)
                     TileExpr = "null";
                  else if (frames.Count == 1)
                     TileExpr = frames[0].Item2;
                  else
                  {
                     string counterArg;
                     System.Collections.ArrayList animArgs = new System.Collections.ArrayList();
                     if (drTile.CounterRow == null)
                     {
                        err.WriteLine("Tileset " + drTileset.Name + " tile " + drTile.TileValue + " requires a counter");
                        counterArg = string.Empty;
                     }
                     else
                     {
                        counterArg = "counters." + NameToVariable(drTile.Counter);
                     }
                     foreach (var f in frames)
                        animArgs.Add("new TileFrame(" + f.Item1 + "," + f.Item2 + ")");

                     TileExpr = "new AnimTile(" + counterArg + ",[" + String.Join(",", animArgs.ToArray()) + "])";
                     ownLine = true;
                  }
               }
               else
                  TileExpr = i.ToString();

               switch (outputState)
               {
                  case 1:
                     if (ownLine)
                        txt.Write(",\r\n      ");
                     else
                        txt.Write(",");
                     break;
                  case 2:
                     txt.Write(",\r\n      ");
                     break;
               }
               txt.Write(TileExpr);
               outputState = ownLine ? 2 : 1;
            }
            txt.WriteLine("]);");
         }
         txt.WriteLine("};");
      }

      const string jsEncodedDigits = " 1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^*()_+-=[]{}|:;,./?`~";
      private void GenerateHtmlMaps(System.IO.TextWriter txt, System.IO.TextWriter err)
      {
         txt.WriteLine("// DecodeData1 = Cardinality 1-" + jsEncodedDigits.Length.ToString() + "\r\n" +
            "// DecodeData2 = Cardinality " + (jsEncodedDigits.Length + 1).ToString() + "-" + (jsEncodedDigits.Length * jsEncodedDigits.Length).ToString()+ "\r\n" +
            "var dataDigits = '" + jsEncodedDigits + "';\r\n");

         txt.WriteLine(GetJavascriptFile("MapLayer.js"));
         txt.WriteLine(GetJavascriptFile("Plan.js"));

         System.Text.StringBuilder sbLayer = new System.Text.StringBuilder();
         System.Text.StringBuilder sbScroll = new System.Text.StringBuilder();
         System.Text.StringBuilder sbDraw = new System.Text.StringBuilder();
         System.Text.StringBuilder sbSprites = new System.Text.StringBuilder();
         System.Text.StringBuilder sbPlans = new System.Text.StringBuilder();
         System.Text.StringBuilder sbSave = new System.Text.StringBuilder();
         System.Text.StringBuilder sbLoad = new System.Text.StringBuilder();
         System.Collections.Generic.List<String> mapList = new System.Collections.Generic.List<String>();
         foreach (System.Data.DataRowView drv in ProjectData.Map.DefaultView)
         {
            ProjectDataset.MapRow drMap = (ProjectDataset.MapRow)drv.Row;
            sbScroll.AppendLine(
               "   " + NameToMapClass(drMap.Name) + ".scroll = function(x, y) {\r\n" +
               "      if(x < viewWidth - this.scrollWidth) x = viewWidth - this.scrollWidth;\r\n" +
               "      if(x > 0) x = 0;\r\n" +
               "      if(y < viewHeight - this.scrollHeight) y = viewHeight - this.scrollHeight;\r\n" +
               "      if(y > 0) y = 0;\r\n" +
               "      this.scrollX = x;\r\n" +
               "      this.scrollY = y;");
            sbDraw.AppendLine(
               "   " + NameToMapClass(drMap.Name) + ".draw = function(ctx) {");
            mapList.Add(NameToMapClass(drMap.Name) + ":" + NameToMapClass(drMap.Name));
            txt.WriteLine("var " + NameToMapClass(drMap.Name) + " = {scrollX: 0, scrollY: 0, scrollWidth: " +
               drMap.ScrollWidth.ToString() + ", scrollHeight: " + drMap.ScrollHeight.ToString() + ", scrollMarginLeft: " +
               drMap.ScrollMarginLeft.ToString() + ", scrollMarginTop: " + drMap.ScrollMarginTop.ToString() + ", scrollMarginRight: " +
               drMap.ScrollMarginRight.ToString() + ", scrollMarginBottom: " + drMap.ScrollMarginBottom.ToString() + "};");
            txt.WriteLine(NameToMapClass(drMap.Name) + ".executeRules = function() {");
            sbSave.AppendLine("   " + NameToMapClass(drMap.Name) + ".getState = function() {\r\n" +
               "      var result = {};");
            sbLoad.AppendLine("   " + NameToMapClass(drMap.Name) + ".setState = function(data) {");

            foreach (ProjectDataset.LayerRow lyr in ProjectData.GetSortedLayers(drMap))
            {
               if (lyr.Tileset == null)
                  continue;
               string lyrRef = NameToMapClass(drMap.Name) + "." + NameToVariable(lyr.Name) + "_Lyr";

               txt.WriteLine("   " + lyrRef + ".executeRules();");
               sbLayer.AppendLine("   " + lyrRef + " = new MapLayer(\r\n" +
                  "      " + NameToMapClass(drMap.Name) +
                  ", tilesets." + NameToVariable(lyr.Tileset) + "," +
                  lyr.Width.ToString() + "," + lyr.Height.ToString() + "," +
                  lyr.VirtualWidth.ToString() + "," + lyr.VirtualHeight.ToString() + "," +
                  lyr.OffsetX.ToString() + "," + lyr.OffsetY.ToString() + "," +
                  lyr.ScrollRateX.ToString() + "," + lyr.ScrollRateY.ToString() + "," +
                  lyr.Priority.ToString() + ",\r\n" +
                  "      " + EncodeTilesToJavascript(lyr, "      ") + ");");
               if ((lyr.ScrollRateX != 0) || (lyr.ScrollRateY != 0))
               {
                  sbScroll.AppendLine("      " + lyrRef + ".currentX = " + ((lyr.OffsetX != 0) ? lyrRef + ".offsetX + " : string.Empty) +
                     ((lyr.ScrollRateX == 1) ? "x" : "Math.floor(x * " + lyr.ScrollRateX.ToString() + ")") + ";");
                  sbScroll.AppendLine("      " + lyrRef + ".currentY = " + ((lyr.OffsetY != 0) ? lyrRef + ".offsetY + " : string.Empty) +
                     ((lyr.ScrollRateY == 1) ? "y" : "Math.floor(y * " + lyr.ScrollRateY.ToString() + ")") + ";");
               }
               sbDraw.AppendLine("      " + lyrRef + ".draw(ctx);");
               sbSave.AppendLine("      result." + NameToVariable(lyr.Name) + "_Lyr = " + lyrRef + ".getState();");
               sbLoad.AppendLine("      " + lyrRef + ".setState(data." + NameToVariable(lyr.Name) + "_Lyr);");
               ProjectDataset.SpritePlanRow[] plans = ProjectData.GetSortedSpritePlans(lyr, false);

               sbLayer.AppendLine("   " + lyrRef + ".executeRules = function() {");

               foreach (ProjectDataset.SpritePlanRow drPlan in plans)
               {
                  string planRef = lyrRef + "." + NameToVariable(drPlan.Name);
                  sbPlans.AppendLine("   " + planRef + " = new PlanBase();");
                  sbPlans.AppendLine("   " + planRef + ".layer = " + lyrRef + ";");

                  ProjectDataset.CoordinateRow[] drCoords = ProjectData.GetSortedCoordinates(drPlan);
                  if (drCoords.Length == 2)
                  {
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
                     sbPlans.AppendLine("   " + planRef + ".left = " + minX + ";");
                     sbPlans.AppendLine("   " + planRef + ".top = " + minY + ";");
                     sbPlans.AppendLine("   " + planRef + ".width = " + (maxX - minX).ToString() + ";");
                     sbPlans.AppendLine("   " + planRef + ".height = " + (maxY - minY).ToString() + ";");
                  }
                  if (drCoords.Length > 0)
                  {
                     sbPlans.Append("   (" + planRef + ".m_Coords=[");
                     bool firstCoord = true;
                     foreach (ProjectDataset.CoordinateRow drCoord in drCoords)
                     {
                        if (firstCoord)
                           firstCoord = false;
                        else
                           sbPlans.Append(",");
                        sbPlans.Append("{x:" + drCoord.X.ToString() + ", y:" + drCoord.Y.ToString() + ((drCoord.Weight == 0)?"" : ", weight:" + drCoord.Weight.ToString()) + "}");
                     }
                     sbPlans.AppendLine("]).forEach(function(element, index, array) {" + planRef + "[index] = element});");
                  }

                  ProjectDataset.PlanParameterValueRow[] planParams = drPlan.GetPlanParameterValueRows();
                  if (planParams.Length > 0)
                  {
                     foreach (ProjectDataset.PlanParameterValueRow planParam in planParams)
                     {
                        sbPlans.AppendLine("   " + planRef + "." + planParam.Name + " = " + planParam.Value);
                     }
                  }

                  ProjectDataset.PlanRuleRow[] rules = ProjectData.GetSortedPlanRules(drPlan, false);
                  if (rules.Length > 0)
                  {
                     sbPlans.AppendLine("   " + planRef + ".executeRules = function() {");
                     sbLayer.AppendLine("      " + planRef + ".executeRules();");
                     RuleContent[] ruleArray = new RuleContent[rules.Length];
                     for (int i = 0; i < rules.Length; i++)
                        ruleArray[i] = new RuleContent(rules[i]);
                     CodeMemberMethod mthExecuteRules = new CodeMemberMethod();
                     try
                     {
                        PreProcessHtmlRules(ruleArray, null); 
                        GenerateRules(ruleArray, mthExecuteRules);
                        sbPlans.AppendLine(GenerateHtmlRules(mthExecuteRules.Statements, 6));
                     }
                     catch (System.ApplicationException ex)
                     {
                        err.WriteLine("Error generating plan \"" + drPlan.Name + "\" on layer \"" + lyr.Name + "\" of map \"" + drMap.Name + "\": " + ex.Message);
                     }
                     sbPlans.AppendLine("   };");
                  }
               }

               sbSprites.AppendLine("   " + lyrRef + ".initSprites = function() {");
               System.Collections.Generic.List<string> spriteNames = new System.Collections.Generic.List<string>();
               System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<ProjectDataset.SpriteDefinitionRow>> categorySprites = new System.Collections.Generic.Dictionary<string,System.Collections.Generic.List<ProjectDataset.SpriteDefinitionRow>>();
               foreach (ProjectDataset.SpriteRow spr in ProjectData.GetSortedSpriteRows(lyr, false))
               {
                  ProjectDataset.SpriteDefinitionRow sprDef = ProjectData.GetSpriteDefinition(spr.DefinitionName);
                  sbSprites.Append("      this.m_" + NameToVariable(spr.Name) + " = new spriteDefinitions." + NameToVariable(sprDef.Name) + "(this," +
                     spr.X.ToString() + "," + spr.Y.ToString() + "," + spr.DX.ToString() + "," + spr.DY.ToString() + "," +
                     "spriteDefinitions." + NameToVariable(sprDef.Name) + ".statesEnum." + NameToVariable(spr.StateName) + "," +
                     spr.CurrentFrame.ToString() + "," + (spr.Active ? "true" : "false") + "," + spr.Priority.ToString() + "," +
                     ((spr.IsSolidityNull() || string.IsNullOrEmpty(spr.Solidity)) ? "null" : "solidity." + NameToVariable(spr.Solidity)));
                  foreach (ProjectDataset.SpriteParameterRow spp in ProjectData.GetSortedSpriteParameters(sprDef, false))
                  {
                     ProjectDataset.ParameterValueRow drValue = ProjectData.GetSpriteParameterValueRow(spr, spp.Name);
                     if (drValue == null)
                        sbSprites.Append(",0");
                     else
                        sbSprites.Append("," + drValue.Value.ToString());
                  }
                  sbSprites.AppendLine(");");
                  spriteNames.Add("this.m_" + NameToVariable(spr.Name));
               }
               sbSprites.AppendLine("      this.sprites = [" + string.Join(",", spriteNames.ToArray()) + "];");
               sbSprites.AppendLine("      this.spriteCategories = Sprite.categorize(this.sprites);");
               sbSprites.AppendLine("   };\r\n" +
                  "   " + lyrRef + ".initSprites();");
               sbLayer.AppendLine("      " + lyrRef + ".processSprites();");
               sbLayer.AppendLine("   };");
            }
            txt.WriteLine("};"); // End Map's executeRules function
            sbScroll.AppendLine("   };");
            sbDraw.AppendLine("   };");
            sbSave.AppendLine("      return result;\r\n" +
               "   };");
            sbLoad.AppendLine("   };");
         }
         txt.WriteLine("function initMaps() {\r\n" + sbLayer.ToString() + sbScroll.ToString() + sbDraw.ToString() + sbPlans.ToString() + sbSprites.ToString() + 
            sbSave.ToString() + sbLoad.ToString() + "}");
         txt.WriteLine("var maps = {" + string.Join(",", mapList) + "};");
      }

      void PreProcessHtmlRules(RuleContent[] rules, ProjectDataset.SpriteDefinitionRow sprite)
      {
         System.Collections.Generic.HashSet<string> spriteParams = null;
         if (sprite != null)
         {
            spriteParams = new System.Collections.Generic.HashSet<string>();
            foreach (ProjectDataset.SpriteParameterRow p in sprite.GetSpriteParameterRows())
               spriteParams.Add(p.Name);
            RemotingServices.RemotePropertyInfo[] spriteProperties = GetSpriteProperties(sprite);
            if (spriteProperties != null)
            {
               foreach (var sp in spriteProperties)
                  spriteParams.Add(sp.Name);
            }
         }
         foreach(RuleContent rule in rules)
         {
            if (0 != (options & HtmlGeneratorOptions.CamelCase))
            {
               if (rule.Function.StartsWith("!"))
                  rule.Function = "!" + CamelCase(rule.Function.Substring(1));
               else
                  rule.Function = CamelCase(rule.Function);
            }
            PreProcessHtmlParameter(ref rule.Parameter1, spriteParams);
            PreProcessHtmlParameter(ref rule.Parameter2, spriteParams);
            PreProcessHtmlParameter(ref rule.Parameter3, spriteParams);
            PreProcessHtmlParameter(ref rule.ResultParameter, spriteParams);
         }
      }

      private System.Collections.Generic.List<System.Text.RegularExpressions.Regex> javaParamSearches;
      private System.Collections.Generic.List<String> javaParamReplacements;
      void PreProcessHtmlParameter(ref string param, System.Collections.Generic.HashSet<string> spriteParams)
      {
         if (javaParamReplacements == null)
         {
            System.Text.RegularExpressions.RegexOptions reOpt = System.Text.RegularExpressions.RegexOptions.Compiled;

            javaParamSearches = new System.Collections.Generic.List<System.Text.RegularExpressions.Regex>(
               new System.Text.RegularExpressions.Regex[] {
               new System.Text.RegularExpressions.Regex("([^.A-Za-z0-9]|^)(" + SpritePlanParentField + ")", reOpt),
               new System.Text.RegularExpressions.Regex(@"typeof *\(([^)]+)\)", reOpt),
               new System.Text.RegularExpressions.Regex(@"^\s*ref\s+\S*\.(\w+)\s*$", reOpt),
               new System.Text.RegularExpressions.Regex(TileCategoryNameClass + @"\.", reOpt), 
               new System.Text.RegularExpressions.Regex(@"RelativePosition\.(\w+)$", reOpt),
               new System.Text.RegularExpressions.Regex(@"^(SpriteBase\.)?\bSpriteAnimationType\.(\w+)$", reOpt),
               new System.Text.RegularExpressions.Regex(@"^(SpriteBase\.)?\bDirection\.(\w+)$", reOpt),
               new System.Text.RegularExpressions.Regex(@"\bParentLayer.m_SpriteCategories", reOpt),
               new System.Text.RegularExpressions.Regex(@"^CounterOperation\.(\w+)$", reOpt),
               new System.Text.RegularExpressions.Regex(@"^Counter\.(\w+)$", reOpt)});

            javaParamReplacements = new System.Collections.Generic.List<string>(new string[] {
               "$1this.layer", "$1", "\"$1\"", "tileCategories.", "\"$1\"", "\"$2\"", "\"$2\"", "this.layer.spriteCategories",
               "\"$1\"", "counters.$1"});

            foreach (System.Data.DataRowView drv in ProjectData.SpriteDefinition.DefaultView)
            {
               ProjectDataset.SpriteDefinitionRow sdr = (ProjectDataset.SpriteDefinitionRow)drv.Row;
               javaParamSearches.Add(new System.Text.RegularExpressions.Regex(
                  @"\b" + SpritesNamespace + @"\." + NameToVariable(sdr.Name) + @"\." + SpriteStateEnumName + @"\b",
                  reOpt));
               javaParamReplacements.Add("spriteDefinitions." + NameToVariable(sdr.Name) + ".statesEnum");
            }
         }
         if (string.IsNullOrEmpty(param)) return;
         param = param.Replace("(int)", string.Empty);
         for (int i = 0; i < javaParamSearches.Count; i++)
         {
            param = javaParamSearches[i].Replace(param, javaParamReplacements[i]);
         }
         if ((spriteParams != null) && spriteParams.Contains(param))
            param = "this." + param;
         while (param.IndexOf("SpriteBase.InputBits.") >= 0)
         {
            int repl = param.IndexOf("SpriteBase.InputBits.");
            param = param.Substring(0, repl) + "Sprite.inputBits." + CamelCase(param.Substring(repl + 21));
         }
      }

      string GenerateHtmlRules(CodeStatementCollection statements, int indentCols)
      {
         using (System.IO.StringWriter sw = new System.IO.StringWriter())
         {
            foreach (CodeStatement st in statements)
            {
               Generator.GenerateCodeFromStatement(st, sw, GeneratorOptions);
            }
            string indent = new string(' ', indentCols);
            return indent + sw.ToString().Replace(Environment.NewLine, Environment.NewLine + indent);
         }
      }
      void GenerateHtmlMapButtons(System.IO.TextWriter txt)
      {
         int counter = 0;
         txt.WriteLine("<script type=\"text/javascript\">\r\n" +
            "function switchToMap(m) { currentMap=m; m.draw(gameViewContext); }\r\n" +
            "</script>");
         foreach (System.Data.DataRowView drv in ProjectData.Map.DefaultView)
         {
            ProjectDataset.MapRow drMap = (ProjectDataset.MapRow)drv.Row;
            if ((counter++ % 5) == 0)
               txt.WriteLine("<br />");
            txt.WriteLine("<input id=\"btn" + NameToMapClass(drMap.Name) + "\" type=\"button\" onclick=\"switchToMap(" +
               NameToMapClass(drMap.Name) + ")\" value=\"" + drMap.Name + "\"/>");
         }
      }

      private int GetLayerCardinality(ProjectDataset.LayerRow lr)
      {
         int cardinality = 1;
         for (int i = 0; i < lr.Tiles.Length; i += lr.BytesPerTile)
         {
            int tileVal;
            switch (lr.BytesPerTile)
            {
               case 1:
                  tileVal = lr.Tiles[i];
                  break;
               case 2:
                  tileVal = BitConverter.ToInt16(lr.Tiles, i);
                  break;
               case 4:
                  tileVal = BitConverter.ToInt32(lr.Tiles, i);
                  break;
               default:
                  throw new NotImplementedException();
            }
            if (tileVal + 1 > cardinality) cardinality = tileVal + 1;
         }
         return cardinality;
      }

      private string EncodeTilesToJavascript(ProjectDataset.LayerRow lr, string indent)
      {
         int cardinality = GetLayerCardinality(lr);
         int tileCount = lr.Width * lr.Height;
         System.Text.StringBuilder sb = new System.Text.StringBuilder("'");
         for (int i = 0; i < tileCount; i++)
         {
            int tileVal;
            switch (lr.BytesPerTile)
            {
               case 1:
                  tileVal = lr.Tiles[i];
                  break;
               case 2:
                  tileVal = BitConverter.ToInt16(lr.Tiles, i * 2);
                  break;
               case 4:
                  tileVal = BitConverter.ToInt32(lr.Tiles, i * 4);
                  break;
               default:
                  throw new NotImplementedException();
            }
            if (cardinality <= jsEncodedDigits.Length)
               sb.Append(jsEncodedDigits[tileVal]);
            else
               sb.Append(jsEncodedDigits[(int)(tileVal / jsEncodedDigits.Length)].ToString() + jsEncodedDigits[tileVal % jsEncodedDigits.Length].ToString());
            if (i % lr.Width == lr.Width - 1)
            {
               sb.Append("'");
               if (i != tileCount - 1)
                  sb.Append(" + \r\n" + indent + "'");
            }
         }
         return sb.ToString();
      }

      private RemotingServices.RemotePropertyInfo[] GetSpriteProperties(ProjectDataset.SpriteDefinitionRow spriteDef)
      {
         if (!string.IsNullOrEmpty(CompileTempAssembly(false)))
            return null;
         RemotingServices.IRemoteTypeInfo reflector;
         try
         {
            reflector = CreateInstanceAndUnwrap("RemoteReflector", SpritesNamespace + "." + NameToVariable(spriteDef.Name)) as RemotingServices.IRemoteTypeInfo;
         }
         catch (System.Exception)
         {
            return null;
         }
         return reflector.GetProperties();
      }

      private void GenerateHtmlSprites(System.IO.TextWriter txt, System.IO.TextWriter err)
      {
         txt.WriteLine(GetJavascriptFile("Sprite.js"));
         txt.WriteLine("var spriteDefinitions = new Object();");
         foreach (System.Data.DataRowView drv in ProjectData.SpriteDefinition.DefaultView)
         {
            ProjectDataset.SpriteDefinitionRow drSpriteDef = (ProjectDataset.SpriteDefinitionRow)drv.Row;
            txt.Write("spriteDefinitions." + NameToVariable(drSpriteDef.Name) + " = function(layer, x, y, dx, dy, state, frame, active, priority, solidity");
            string paramDeserialize = string.Empty;
            string paramSerialize = string.Empty;
            foreach (ProjectDataset.SpriteParameterRow drParam in ProjectData.GetSortedSpriteParameters(drSpriteDef, false))
            {
               txt.Write(", " + NameToVariable(drParam.Name));
               paramDeserialize += ",source." + NameToVariable(drParam.Name);
               paramSerialize += "," + NameToVariable(drParam.Name) + ":this." + NameToVariable(drParam.Name);
            }
            txt.WriteLine(") {");
            txt.WriteLine("   Sprite.call(this, layer, x, y, dx, dy, state, frame, active, priority, solidity);");
            foreach(ProjectDataset.SpriteParameterRow drParam in ProjectData.GetSortedSpriteParameters(drSpriteDef, false))
            {
               txt.WriteLine("   this." + NameToVariable(drParam.Name) + " = " + NameToVariable(drParam.Name));
            }
            txt.WriteLine("};");
            txt.WriteLine("spriteDefinitions." + NameToVariable(drSpriteDef.Name) + ".prototype = new Sprite();\r\n" +
               "spriteDefinitions." + NameToVariable(drSpriteDef.Name) + ".prototype.constructor = spriteDefinitions." + NameToVariable(drSpriteDef.Name) + ";");
            txt.WriteLine("spriteDefinitions." + NameToVariable(drSpriteDef.Name) + ".deserialize = function(layer, data) {\r\n" +
               "   var source = JSON.parse(data);\r\n" +
               "   return result = new spriteDefinitions." + NameToVariable(drSpriteDef.Name) +
               "(layer, source.x, source.y, source.dx, source.dy, source.state, source.frame, source.active, source.priority, solidity[source.solidityName]" +
               paramDeserialize + ");\r\n" +
               "}");
            txt.WriteLine("spriteDefinitions." + NameToVariable(drSpriteDef.Name) + ".prototype.serialize = function() {\r\n" +
               "   return JSON.stringify(this);\r\n" +
               "}");
            txt.WriteLine("spriteDefinitions." + NameToVariable(drSpriteDef.Name) + ".prototype.toJSON = function() {\r\n" +
               "   return {\"~1\":\"" + NameToVariable(drSpriteDef.Name) + "\"" +
               ",x:this.x,y:this.y,dx:this.dx,dy:this.dy,state:this.state,frame:this.frame,active:this.isActive,priority:this.priority" +
               ",solidityName:solidity.getSolidityName(this.solidity)" + paramSerialize + "};\r\n" +
               "}");
         }
         txt.WriteLine("function initSprites() {\r\n" +
            "   var bounds;");

         foreach (System.Data.DataRowView drv in ProjectData.SpriteDefinition.DefaultView)
         {
            ProjectDataset.SpriteDefinitionRow drSpriteDef = (ProjectDataset.SpriteDefinitionRow)drv.Row;
            txt.WriteLine("   spriteDefinitions." + NameToVariable(drSpriteDef.Name) + ".prototype.states = new Array();");

            ProjectDataset.SpriteCategorySpriteRow[] scsrs = drSpriteDef.GetSpriteCategorySpriteRows();
            if (scsrs.Length > 0)
            {
               string[] cats = new string[scsrs.Length];
               for (int ci = 0; ci < scsrs.Length; ci++)
                  cats[ci] = "\"" + NameToVariable(scsrs[ci].CategoryName) + "\"";
               txt.WriteLine("   spriteDefinitions." + NameToVariable(drSpriteDef.Name) + ".prototype.categories = [" + string.Join(",", cats) + "];");
            }
            CachedSpriteDef sprCached = new CachedSpriteDef(drSpriteDef, null);
            System.Drawing.Rectangle bounds = System.Drawing.Rectangle.Empty;
            int stateIndex = 0;
            System.Collections.Generic.List<string> statesEnum = new System.Collections.Generic.List<string>();

            foreach (ProjectDataset.SpriteStateRow drState in ProjectData.GetSortedSpriteStates(drSpriteDef))
            {
               StateInfo sinf = sprCached[drState.Name];

               if ((bounds.IsEmpty) ||
                  (bounds.X != sinf.Bounds.X) || (bounds.Y != sinf.Bounds.Y) ||
                  (bounds.Width != sinf.Bounds.Width) || (bounds.Height != sinf.Bounds.Height))
               {
                  bounds = new System.Drawing.Rectangle(sinf.Bounds.X, sinf.Bounds.Y, sinf.Bounds.Width, sinf.Bounds.Height);
                  txt.WriteLine("   bounds = {x: " + bounds.X.ToString() + ", y: " + bounds.Y.ToString() +
                     ", width: " + bounds.Width.ToString() + ", height: " + bounds.Height.ToString() + "};");
               }

               statesEnum.Add(NameToVariable(drState.Name) + ": " + stateIndex.ToString());
               int nAccumulatedDuration = 0;
               System.Collections.ArrayList stateParams = new System.Collections.ArrayList();
               txt.Write("   spriteDefinitions." + NameToVariable(drSpriteDef.Name) + ".prototype.states[" + stateIndex + "] = new SpriteState(" +
                  drState.SolidWidth.ToString() + "," + drState.SolidHeight.ToString() + "," +
                  "frameSets." + NameToVariable(drState.FramesetName) + ",bounds,");
 
               System.Collections.Generic.List<string> subFrames = null;
               bool firstFrame = true;
               ProjectDataset.SpriteFrameRow[] frames = ProjectData.GetSortedSpriteFrames(drState);
               if (frames.Length == 0)
                  txt.Write("null");
               else
               {
                  txt.Write("[");
                  foreach (ProjectDataset.SpriteFrameRow drFrame in frames)
                  {
                     if (subFrames == null)
                        subFrames = new System.Collections.Generic.List<string>();
                     subFrames.Add(drFrame.FrameValue.ToString());
                     if (drFrame.Duration > 0)
                     {
                        nAccumulatedDuration += drFrame.Duration;
                        if (!firstFrame)
                           txt.Write(",");
                        if (subFrames.Count == 1)
                        {
                           txt.Write("new TileFrame(" + nAccumulatedDuration.ToString() + "," + subFrames[0] + ")");
                        }
                        else
                        {
                           txt.Write("new TileFrame(" + nAccumulatedDuration.ToString() + ",[" +
                              string.Join(",", subFrames.ToArray()) + "])");
                        }
                        subFrames.Clear();
                        firstFrame = false;
                     }
                  }
                  txt.Write("]");
               }
               stateIndex++;
               txt.WriteLine(");");
            }
            txt.WriteLine("   spriteDefinitions." + NameToVariable(drSpriteDef.Name) + ".statesEnum = {" + string.Join(",", statesEnum.ToArray()) + "};");

            ProjectDataset.SpriteRuleRow[] rules = ProjectData.GetSortedSpriteRules(drSpriteDef, false);
            if (rules.Length > 0)
            {
               txt.WriteLine("   spriteDefinitions." + NameToVariable(drSpriteDef.Name) + ".prototype.executeRules = function() {");
               RuleContent[] ruleArray = new RuleContent[rules.Length];
               CodeMemberMethod mthExecuteRules = new CodeMemberMethod();
               try
               {
                  for (int i = 0; i < rules.Length; i++)
                     ruleArray[i] = new RuleContent(rules[i]);
                  PreProcessHtmlRules(ruleArray, drSpriteDef);
                  GenerateRules(ruleArray, mthExecuteRules);
                  txt.WriteLine(GenerateHtmlRules(mthExecuteRules.Statements, 6));
               }
               catch (System.ApplicationException ex)
               {
                  err.WriteLine("Error generating sprite definition \"" + drSpriteDef.Name + "\": " + ex.Message);
               }
               txt.WriteLine("   };");
            }
         }
         txt.WriteLine("}");
      }

      private void GenerateHtmlTileCategories(System.IO.TextWriter txt)
      {
         txt.WriteLine(GetJavascriptFile("Tile.js"));

         txt.WriteLine("var tileCategories = new Object();");
         txt.WriteLine("var solidity = new Object();");
         txt.WriteLine("solidity.getSolidityName = function(solid) {\r\n" +
            "   for(var key in solidity) {\r\n" +
            "      if (solidity[key] === solid) return key;\r\n" +
            "   }\r\n" +
            "   return null;\r\n" +
            "}");
         txt.WriteLine("function initTileCategories() {");
         foreach(System.Data.DataRowView drv in ProjectData.TileCategory.DefaultView)
         {
            ProjectDataset.TileCategoryRow drCat = (ProjectDataset.TileCategoryRow)drv.Row;
            txt.WriteLine("   tileCategories." + NameToVariable(drCat.Name) + " = new TileCategory([");
            bool firstTileset = true;
            foreach(ProjectDataset.CategorizedTilesetRow drCatTs in drCat.GetCategorizedTilesetRows())
            {
               if (firstTileset)
                  txt.Write("      ");
               else
                  txt.Write(",\r\n      ");
               txt.Write("{tileset:tilesets." + NameToVariable(drCatTs.Tileset) + ",membership:[");
               bool firstTile = true;
               foreach (ProjectDataset.CategoryTileRow drCatTile in drCatTs.GetCategoryTileRows())
               {
                  if (!firstTile)
                     txt.Write(",");
                  ProjectDataset.CategoryFrameRow[] frameList = drCatTile.GetCategoryFrameRows();
                  if (frameList.Length > 0)
                  {
                     txt.Write("{tileIndex:" + drCatTile.TileValue.ToString() + ",frames:[");
                     bool firstFrame = true;
                     foreach (ProjectDataset.CategoryFrameRow cfr in frameList)
                     {
                        if (!firstFrame)
                           txt.Write(",");
                        txt.Write(cfr.Frame.ToString());
                        firstFrame = false;
                     }
                     txt.Write("]}");
                  }
                  else
                     txt.Write(drCatTile.TileValue.ToString());
                  firstTile = false;
               }
               txt.Write("]}");
               firstTileset = false;
            }
            txt.WriteLine("]);");
         }
         foreach (System.Data.DataRowView drv in ProjectData.Solidity.DefaultView)
         {
            ProjectDataset.SolidityRow drSolid = (ProjectDataset.SolidityRow)drv.Row;
            bool firstShape = true; 
            txt.WriteLine("   solidity." + NameToVariable(drSolid.Name) + " = new Solidity([");
            foreach (ProjectDataset.SolidityShapeRow drShape in drSolid.GetSolidityShapeRows())
            {
               if (firstShape)
                  txt.Write("      ");
               else
                  txt.Write(",\r\n      ");
               txt.Write("{tileCategory:tileCategories." + NameToVariable(drShape.CategoryName) + ", tileShape:TileShape." + TileShapeName(drShape.ShapeName) + "}");
               firstShape = false;
            }
            txt.WriteLine("]);");
         }
         txt.WriteLine("}");
      }

      private string TileShapeName(string name)
      {
         string result = CamelCase(NameToVariable(name));
         if (result.EndsWith("TileShape")) result = result.Substring(0, result.Length - 9);
         return result;
      }

      private static string CamelCase(string name)
      {
         return char.ToLower(name[0]) + name.Substring(1);
      }
      #endregion
   }
}
