using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;

namespace SGDK2
{
	/// <summary>
	/// Summary description for DataChangeNotifier.
	/// </summary>
   public class DataChangeNotifier : System.ComponentModel.Component
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.Container components = null;

      ProjectDataset.GraphicSheetRowChangeEventHandler m_GraphicsSheetChanged = null;
      ProjectDataset.GraphicSheetRowChangeEventHandler m_GraphicsSheetChanging = null;
      ProjectDataset.GraphicSheetRowChangeEventHandler m_GraphicsSheetDeleted = null;
      ProjectDataset.FramesetRowChangeEventHandler m_FramesetChanged = null;
      ProjectDataset.FramesetRowChangeEventHandler m_FramesetChanging = null;
      ProjectDataset.FramesetRowChangeEventHandler m_FramesetDeleted = null;
      ProjectDataset.FrameRowChangeEventHandler m_FrameChanged = null;
      ProjectDataset.FrameRowChangeEventHandler m_FrameChanging = null;
      ProjectDataset.FrameRowChangeEventHandler m_FrameDeleted = null;
      ProjectDataset.TilesetRowChangeEventHandler m_TilesetChanged = null;
      ProjectDataset.TilesetRowChangeEventHandler m_TilesetChanging = null;
      ProjectDataset.TilesetRowChangeEventHandler m_TilesetDeleted = null;
      ProjectDataset.TileRowChangeEventHandler m_TileChanged = null;
      ProjectDataset.TileRowChangeEventHandler m_TileChanging = null;
      ProjectDataset.TileRowChangeEventHandler m_TileDeleted = null;
      ProjectDataset.CounterRowChangeEventHandler m_CounterChanged = null;
      ProjectDataset.CounterRowChangeEventHandler m_CounterChanging = null;
      ProjectDataset.CounterRowChangeEventHandler m_CounterDeleted = null;
      ProjectDataset.MapRowChangeEventHandler m_MapChanged = null;
      ProjectDataset.MapRowChangeEventHandler m_MapChanging = null;
      ProjectDataset.MapRowChangeEventHandler m_MapDeleted = null;
      ProjectDataset.LayerRowChangeEventHandler m_LayerChanged = null;
      ProjectDataset.LayerRowChangeEventHandler m_LayerChanging = null;
      ProjectDataset.LayerRowChangeEventHandler m_LayerDeleted = null;
      ProjectDataset.TileFrameRowChangeEventHandler m_TileFrameChanged = null;
      ProjectDataset.TileFrameRowChangeEventHandler m_TileFrameChanging = null;
      ProjectDataset.TileFrameRowChangeEventHandler m_TileFrameDeleted = null;
      ProjectDataset.CategoryRowChangeEventHandler m_CategoryChanged = null;
      ProjectDataset.CategoryRowChangeEventHandler m_CategoryChanging = null;
      ProjectDataset.CategoryRowChangeEventHandler m_CategoryDeleted = null;
      ProjectDataset.CategoryTileRowChangeEventHandler m_CategoryTileChanged = null;
      ProjectDataset.CategoryTileRowChangeEventHandler m_CategoryTileChanging = null;
      ProjectDataset.CategoryTileRowChangeEventHandler m_CategoryTileDeleted = null;
      ProjectDataset.SpriteDefinitionRowChangeEventHandler m_SpriteDefinitionChanged = null;
      ProjectDataset.SpriteDefinitionRowChangeEventHandler m_SpriteDefinitionChanging = null;
      ProjectDataset.SpriteDefinitionRowChangeEventHandler m_SpriteDefinitionDeleted = null;
      ProjectDataset.SpriteStateRowChangeEventHandler m_SpriteStateChanged = null;
      ProjectDataset.SpriteStateRowChangeEventHandler m_SpriteStateChanging = null;
      ProjectDataset.SpriteStateRowChangeEventHandler m_SpriteStateDeleted = null;
      ProjectDataset.SpriteFrameRowChangeEventHandler m_SpriteFrameChanged = null;
      ProjectDataset.SpriteFrameRowChangeEventHandler m_SpriteFrameChanging = null;
      ProjectDataset.SpriteFrameRowChangeEventHandler m_SpriteFrameDeleted = null;
      ProjectDataset.SpriteRuleRowChangeEventHandler m_SpriteRuleChanged = null;
      ProjectDataset.SpriteRuleRowChangeEventHandler m_SpriteRuleChanging = null;
      ProjectDataset.SpriteRuleRowChangeEventHandler m_SpriteRuleDeleted = null;
      ProjectDataset.SpriteParameterRowChangeEventHandler m_SpriteParameterChanged = null;
      ProjectDataset.SpriteParameterRowChangeEventHandler m_SpriteParameterChanging = null;
      ProjectDataset.SpriteParameterRowChangeEventHandler m_SpriteParameterDeleted = null;
      ProjectDataset.SpriteCategoryRowChangeEventHandler m_SpriteCategoryChanged = null;
      ProjectDataset.SpriteCategoryRowChangeEventHandler m_SpriteCategoryChanging = null;
      ProjectDataset.SpriteCategoryRowChangeEventHandler m_SpriteCategoryDeleted = null;
      ProjectDataset.SpriteCategorySpriteRowChangeEventHandler m_SpriteCategorySpriteChanged = null;
      ProjectDataset.SpriteCategorySpriteRowChangeEventHandler m_SpriteCategorySpriteChanging = null;
      ProjectDataset.SpriteCategorySpriteRowChangeEventHandler m_SpriteCategorySpriteDeleted = null;
      ProjectDataset.SpriteRowChangeEventHandler m_SpriteChanged = null;
      ProjectDataset.SpriteRowChangeEventHandler m_SpriteChanging = null;
      ProjectDataset.SpriteRowChangeEventHandler m_SpriteDeleted = null;
      ProjectDataset.ParameterValueRowChangeEventHandler m_ParameterValueChanged = null;
      ProjectDataset.ParameterValueRowChangeEventHandler m_ParameterValueChanging = null;
      ProjectDataset.ParameterValueRowChangeEventHandler m_ParameterValueDeleted = null;
      ProjectDataset.SolidityRowChangeEventHandler m_SolidityChanged = null;
      ProjectDataset.SolidityRowChangeEventHandler m_SolidityChanging = null;
      ProjectDataset.SolidityRowChangeEventHandler m_SolidityDeleted = null;
      ProjectDataset.SolidityShapeRowChangeEventHandler m_SolidityShapeChanged = null;
      ProjectDataset.SolidityShapeRowChangeEventHandler m_SolidityShapeChanging = null;
      ProjectDataset.SolidityShapeRowChangeEventHandler m_SolidityShapeDeleted = null;
      ProjectDataset.TileShapeRowChangeEventHandler m_TileShapeChanged = null;
      ProjectDataset.TileShapeRowChangeEventHandler m_TileShapeChanging = null;
      ProjectDataset.TileShapeRowChangeEventHandler m_TileShapeDeleted = null;
      ProjectDataset.CategoryFrameRowChangeEventHandler m_CategoryFrameChanged = null;
      ProjectDataset.CategoryFrameRowChangeEventHandler m_CategoryFrameChanging = null;
      ProjectDataset.CategoryFrameRowChangeEventHandler m_CategoryFrameDeleted = null;
      ProjectDataset.SourceCodeRowChangeEventHandler m_SourceCodeChanged = null;
      ProjectDataset.SourceCodeRowChangeEventHandler m_SourceCodeChanging = null;
      ProjectDataset.SourceCodeRowChangeEventHandler m_SourceCodeDeleted = null;
      System.EventHandler m_Clearing = null;

      public DataChangeNotifier(System.ComponentModel.IContainer container)
      {
         ///
         /// Required for Windows.Forms Class Composition Designer support
         ///
         container.Add(this);
         InitializeComponent();
      }

      public DataChangeNotifier()
      {
         ///
         /// Required for Windows.Forms Class Composition Designer support
         ///
         InitializeComponent();
      }

      /// <summary> 
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose( bool disposing )
      {
         if( disposing )
         {
            if(components != null)
            {
               components.Dispose();
            }
            if (m_GraphicsSheetChanged != null)
               GraphicSheetRowChanged -= m_GraphicsSheetChanged;
            if (m_GraphicsSheetChanging != null)
               GraphicSheetRowChanging -= m_GraphicsSheetChanging;
            if (m_GraphicsSheetDeleted != null)
               GraphicSheetRowDeleted -= m_GraphicsSheetDeleted;
            if (m_FramesetChanged != null)
               FramesetRowChanged -= m_FramesetChanged;
            if (m_FramesetChanging != null)
               FramesetRowChanging -= m_FramesetChanging;
            if (m_FramesetDeleted != null)
               FramesetRowDeleted -= m_FramesetDeleted;
            if (m_FrameChanged != null)
               FrameRowChanged -= m_FrameChanged;
            if (m_FrameChanging != null)
               FrameRowChanging -= m_FrameChanging;
            if (m_FrameDeleted != null)
               FrameRowDeleted -= m_FrameDeleted;
            if (m_TilesetChanged != null)
               TilesetRowChanged -= m_TilesetChanged;
            if (m_TilesetChanging != null)
               TilesetRowChanging -= m_TilesetChanging;
            if (m_TilesetDeleted != null)
               TilesetRowDeleted -= m_TilesetDeleted;
            if (m_TileChanged != null)
               TileRowChanged -= m_TileChanged;
            if (m_TileChanging != null)
               TileRowChanging -= m_TileChanging;
            if (m_TileDeleted != null)
               TileRowDeleted -= m_TileDeleted;
            if (m_CounterChanged != null)
               CounterRowChanged -= m_CounterChanged;
            if (m_CounterChanging != null)
               CounterRowChanging -= m_CounterChanging;
            if (m_CounterDeleted != null)
               CounterRowDeleted -= m_CounterDeleted;
            if (m_MapChanged != null)
               MapRowChanged -= m_MapChanged;
            if (m_MapChanging != null)
               MapRowChanging -= m_MapChanging;
            if (m_MapDeleted != null)
               MapRowDeleted -= m_MapDeleted;
            if (m_LayerChanged != null)
               LayerRowChanged -= m_LayerChanged;
            if (m_LayerChanging != null)
               LayerRowChanging -= m_LayerChanging;
            if (m_LayerDeleted != null)
               LayerRowDeleted -= m_LayerDeleted;
            if (m_TileFrameChanged != null)
               TileFrameRowChanged -= m_TileFrameChanged;
            if (m_TileFrameChanging != null)
               TileFrameRowChanging -= m_TileFrameChanging;
            if (m_TileFrameDeleted != null)
               TileFrameRowDeleted -= m_TileFrameDeleted;
            if (m_CategoryChanged != null)
               CategoryRowChanged -= m_CategoryChanged;
            if (m_CategoryChanging != null)
               CategoryRowChanging -= m_CategoryChanging;
            if (m_CategoryDeleted != null)
               CategoryRowDeleted -= m_CategoryDeleted;
            if (m_CategoryTileChanged != null)
               CategoryTileRowChanged -= m_CategoryTileChanged;
            if (m_CategoryTileChanging != null)
               CategoryTileRowChanging -= m_CategoryTileChanging;
            if (m_CategoryTileDeleted != null)
               CategoryTileRowDeleted -= m_CategoryTileDeleted;
            if (m_SpriteDefinitionChanged != null)
               SpriteDefinitionRowChanged -= m_SpriteDefinitionChanged;
            if (m_SpriteDefinitionChanging != null)
               SpriteDefinitionRowChanging -= m_SpriteDefinitionChanging;
            if (m_SpriteDefinitionDeleted != null)
               SpriteDefinitionRowDeleted -= m_SpriteDefinitionDeleted;
            if (m_SpriteStateChanged != null)
               SpriteStateRowChanged -= m_SpriteStateChanged;
            if (m_SpriteStateChanging != null)
               SpriteStateRowChanging -= m_SpriteStateChanging;
            if (m_SpriteStateDeleted != null)
               SpriteStateRowDeleted -= m_SpriteStateDeleted;
            if (m_SpriteFrameChanged != null)
               SpriteFrameRowChanged -= m_SpriteFrameChanged;
            if (m_SpriteFrameChanging != null)
               SpriteFrameRowChanging -= m_SpriteFrameChanging;
            if (m_SpriteFrameDeleted != null)
               SpriteFrameRowDeleted -= m_SpriteFrameDeleted;
            if (m_SpriteRuleChanged != null)
               SpriteRuleRowChanged -= m_SpriteRuleChanged;
            if (m_SpriteRuleChanging != null)
               SpriteRuleRowChanging -= m_SpriteRuleChanging;
            if (m_SpriteRuleDeleted != null)
               SpriteRuleRowDeleted -= m_SpriteRuleDeleted;
            if (m_SpriteParameterChanged != null)
               SpriteParameterRowChanged -= m_SpriteParameterChanged;
            if (m_SpriteParameterChanging != null)
               SpriteParameterRowChanging -= m_SpriteParameterChanging;
            if (m_SpriteParameterDeleted != null)
               SpriteParameterRowDeleted -= m_SpriteParameterDeleted;
            if (m_SpriteCategoryChanged != null)
               SpriteCategoryRowChanged -= m_SpriteCategoryChanged;
            if (m_SpriteCategoryChanging != null)
               SpriteCategoryRowChanging -= m_SpriteCategoryChanging;
            if (m_SpriteCategoryDeleted != null)
               SpriteCategoryRowDeleted -= m_SpriteCategoryDeleted;
            if (m_SpriteCategorySpriteChanged != null)
               SpriteCategorySpriteRowChanged -= m_SpriteCategorySpriteChanged;
            if (m_SpriteCategorySpriteChanging != null)
               SpriteCategorySpriteRowChanging -= m_SpriteCategorySpriteChanging;
            if (m_SpriteCategorySpriteDeleted != null)
               SpriteCategorySpriteRowDeleted -= m_SpriteCategorySpriteDeleted;
            if (m_SpriteChanged != null)
               SpriteRowChanged -= m_SpriteChanged;
            if (m_SpriteChanging != null)
               SpriteRowChanging -= m_SpriteChanging;
            if (m_SpriteDeleted != null)
               SpriteRowDeleted -= m_SpriteDeleted;
            if (m_ParameterValueChanged != null)
               ParameterValueRowChanged -= m_ParameterValueChanged;
            if (m_ParameterValueChanging != null)
               ParameterValueRowChanging -= m_ParameterValueChanging;
            if (m_ParameterValueDeleted != null)
               ParameterValueRowDeleted -= m_ParameterValueDeleted;
            if (m_SolidityChanged != null)
               SolidityRowChanged -= m_SolidityChanged;
            if (m_SolidityChanging != null)
               SolidityRowChanging -= m_SolidityChanging;
            if (m_SolidityDeleted != null)
               SolidityRowDeleted -= m_SolidityDeleted;
            if (m_SolidityShapeChanged != null)
               SolidityShapeRowChanged -= m_SolidityShapeChanged;
            if (m_SolidityShapeChanging != null)
               SolidityShapeRowChanging -= m_SolidityShapeChanging;
            if (m_SolidityShapeDeleted != null)
               SolidityShapeRowDeleted -= m_SolidityShapeDeleted;
            if (m_TileShapeChanged != null)
               TileShapeRowChanged -= m_TileShapeChanged;
            if (m_TileShapeChanging != null)
               TileShapeRowChanging -= m_TileShapeChanging;
            if (m_TileShapeDeleted != null)
               TileShapeRowDeleted -= m_TileShapeDeleted;
            if (m_CategoryFrameChanged != null)
               CategoryFrameRowChanged -= m_CategoryFrameChanged;
            if (m_CategoryFrameChanging != null)
               CategoryFrameRowChanging -= m_CategoryFrameChanging;
            if (m_CategoryFrameDeleted != null)
               CategoryFrameRowDeleted -= m_CategoryFrameDeleted;
            if (m_SourceCodeChanged != null)
               SourceCodeRowChanged -= m_SourceCodeChanged;
            if (m_SourceCodeChanging != null)
               SourceCodeRowChanging -= m_SourceCodeChanging;
            if (m_SourceCodeDeleted != null)
               SourceCodeRowDeleted -= m_SourceCodeDeleted;
            if (m_Clearing != null)
               Clearing -= m_Clearing;
         }
         base.Dispose( disposing );
      }


      #region Component Designer generated code
      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         components = new System.ComponentModel.Container();
      }
      #endregion

      public event ProjectDataset.GraphicSheetRowChangeEventHandler GraphicSheetRowChanged
      {
         add
         {
            Debug.Assert(m_GraphicsSheetChanged == null);
            if (m_GraphicsSheetChanged != null)
               ProjectData.GraphicSheetRowChanged -= m_GraphicsSheetChanged;
            ProjectData.GraphicSheetRowChanged += m_GraphicsSheetChanged = value;
         }
         remove
         {
            Debug.Assert(m_GraphicsSheetChanged == value);
            ProjectData.GraphicSheetRowChanged -= value;
            m_GraphicsSheetChanged = null;
         }
      }
      public event ProjectDataset.GraphicSheetRowChangeEventHandler GraphicSheetRowChanging
      {
         add
         {
            Debug.Assert(m_GraphicsSheetChanging == null);
            if (m_GraphicsSheetChanging != null)
               ProjectData.GraphicSheetRowChanging -= m_GraphicsSheetChanging;
            ProjectData.GraphicSheetRowChanging += m_GraphicsSheetChanging = value;
         }
         remove
         {
            Debug.Assert(m_GraphicsSheetChanging == value);
            ProjectData.GraphicSheetRowChanging -= value;
            m_GraphicsSheetChanging = null;
         }
      }
      public event ProjectDataset.GraphicSheetRowChangeEventHandler GraphicSheetRowDeleted
      {
         add
         {
            Debug.Assert(m_GraphicsSheetDeleted == null);
            if (m_GraphicsSheetDeleted != null)
               ProjectData.GraphicSheetRowDeleted -= m_GraphicsSheetDeleted;
            ProjectData.GraphicSheetRowDeleted += m_GraphicsSheetDeleted = value;
         }
         remove
         {
            Debug.Assert(m_GraphicsSheetDeleted == value);
            ProjectData.GraphicSheetRowDeleted -= value;
            m_GraphicsSheetDeleted = null;
         }
      }
      public event ProjectDataset.FramesetRowChangeEventHandler FramesetRowChanged
      {
         add
         {
            Debug.Assert(m_FramesetChanged == null);
            if (m_FramesetChanged != null)
               ProjectData.FramesetRowChanged -= m_FramesetChanged;
            ProjectData.FramesetRowChanged += m_FramesetChanged = value;
         }
         remove
         {
            Debug.Assert(m_FramesetChanged == value);
            ProjectData.FramesetRowChanged -= value;
            m_FramesetChanged = null;
         }
      }
      public event ProjectDataset.FramesetRowChangeEventHandler FramesetRowChanging
      {
         add
         {
            Debug.Assert(m_FramesetChanging == null);
            if (m_FramesetChanging != null)
               ProjectData.FramesetRowChanging -= m_FramesetChanging;
            ProjectData.FramesetRowChanging += m_FramesetChanging = value;
         }
         remove
         {
            Debug.Assert(m_FramesetChanging == value);
            ProjectData.FramesetRowChanging -= value;
            m_FramesetChanging = null;
         }
      }
      public event ProjectDataset.FramesetRowChangeEventHandler FramesetRowDeleted
      {
         add
         {
            Debug.Assert(m_FramesetDeleted == null);
            if (m_FramesetDeleted != null)
               ProjectData.FramesetRowDeleted -= m_FramesetDeleted;
            ProjectData.FramesetRowDeleted += m_FramesetDeleted = value;
         }
         remove
         {
            Debug.Assert(m_FramesetDeleted == value);
            ProjectData.FramesetRowDeleted -= value;
            m_FramesetDeleted = null;
         }
      }
      public event ProjectDataset.FrameRowChangeEventHandler FrameRowChanged
      {
         add
         {
            Debug.Assert(m_FrameChanged == null);
            if (m_FrameChanged != null)
               ProjectData.FrameRowChanged -= m_FrameChanged;
            ProjectData.FrameRowChanged += m_FrameChanged = value;
         }
         remove
         {
            Debug.Assert(m_FrameChanged == value);
            ProjectData.FrameRowChanged -= value;
            m_FrameChanged = null;
         }
      }
      public event ProjectDataset.FrameRowChangeEventHandler FrameRowChanging
      {
         add
         {
            Debug.Assert(m_FrameChanging == null);
            if (m_FrameChanging != null)
               ProjectData.FrameRowChanging -= m_FrameChanging;
            ProjectData.FrameRowChanging += m_FrameChanging = value;
         }
         remove
         {
            Debug.Assert(m_FrameChanging == value);
            ProjectData.FrameRowChanging -= value;
            m_FrameChanging = null;
         }
      }
      public event ProjectDataset.FrameRowChangeEventHandler FrameRowDeleted
      {
         add
         {
            Debug.Assert(m_FrameDeleted == null);
            if (m_FrameDeleted != null)
               ProjectData.FrameRowDeleted -= m_FrameDeleted;
            ProjectData.FrameRowDeleted += m_FrameDeleted = value;
         }
         remove
         {
            Debug.Assert(m_FrameDeleted == value);
            ProjectData.FrameRowDeleted -= value;
            m_FrameDeleted = null;
         }
      }   
      public event ProjectDataset.TilesetRowChangeEventHandler TilesetRowChanged
      {
         add
         {
            Debug.Assert(m_TilesetChanged == null);
            if (m_TilesetChanged != null)
               ProjectData.TilesetRowChanged -= m_TilesetChanged;
            ProjectData.TilesetRowChanged += m_TilesetChanged = value;
         }
         remove
         {
            Debug.Assert(m_TilesetChanged == value);
            ProjectData.TilesetRowChanged -= value;
            m_TilesetChanged = null;
         }
      }
      public event ProjectDataset.TilesetRowChangeEventHandler TilesetRowChanging
      {
         add
         {
            Debug.Assert(m_TilesetChanging == null);
            if (m_TilesetChanging != null)
               ProjectData.TilesetRowChanging -= m_TilesetChanging;
            ProjectData.TilesetRowChanging += m_TilesetChanging = value;
         }
         remove
         {
            Debug.Assert(m_TilesetChanging == value);
            ProjectData.TilesetRowChanging -= value;
            m_TilesetChanging = null;
         }
      }
      public event ProjectDataset.TilesetRowChangeEventHandler TilesetRowDeleted
      {
         add
         {
            Debug.Assert(m_TilesetDeleted == null);
            if (m_TilesetDeleted != null)
               ProjectData.TilesetRowDeleted -= m_TilesetDeleted;
            ProjectData.TilesetRowDeleted += m_TilesetDeleted = value;
         }
         remove
         {
            Debug.Assert(m_TilesetDeleted == value);
            ProjectData.TilesetRowDeleted -= value;
            m_TilesetDeleted = null;
         }
      }
      public event ProjectDataset.TileRowChangeEventHandler TileRowChanged
      {
         add
         {
            Debug.Assert(m_TileChanged == null);
            if (m_TileChanged != null)
               ProjectData.TileRowChanged -= m_TileChanged;
            ProjectData.TileRowChanged += m_TileChanged = value;
         }
         remove
         {
            Debug.Assert(m_TileChanged == value);
            ProjectData.TileRowChanged -= value;
            m_TileChanged = null;
         }
      }
      public event ProjectDataset.TileRowChangeEventHandler TileRowChanging
      {
         add
         {
            Debug.Assert(m_TileChanging == null);
            if (m_TileChanging != null)
               ProjectData.TileRowChanging -= m_TileChanging;
            ProjectData.TileRowChanging += m_TileChanging = value;
         }
         remove
         {
            Debug.Assert(m_TileChanging == value);
            ProjectData.TileRowChanging -= value;
            m_TileChanging = null;
         }
      }
      public event ProjectDataset.TileRowChangeEventHandler TileRowDeleted
      {
         add
         {
            Debug.Assert(m_TileDeleted == null);
            if (m_TileDeleted != null)
               ProjectData.TileRowDeleted -= m_TileDeleted;
            ProjectData.TileRowDeleted += m_TileDeleted = value;
         }
         remove
         {
            Debug.Assert(m_TileDeleted == value);
            ProjectData.TileRowDeleted -= value;
            m_TileDeleted = null;
         }
      }
      public event ProjectDataset.CounterRowChangeEventHandler CounterRowChanged
      {
         add
         {
            Debug.Assert(m_CounterChanged == null);
            if (m_CounterChanged != null)
               ProjectData.CounterRowChanged -= m_CounterChanged;
            ProjectData.CounterRowChanged += m_CounterChanged = value;
         }
         remove
         {
            Debug.Assert(m_CounterChanged == value);
            ProjectData.CounterRowChanged -= value;
            m_CounterChanged = null;
         }
      }
      public event ProjectDataset.CounterRowChangeEventHandler CounterRowChanging
      {
         add
         {
            Debug.Assert(m_CounterChanging == null);
            if (m_CounterChanging != null)
               ProjectData.CounterRowChanging -= m_CounterChanging;
            ProjectData.CounterRowChanging += m_CounterChanging = value;
         }
         remove
         {
            Debug.Assert(m_CounterChanging == value);
            ProjectData.CounterRowChanging -= value;
            m_CounterChanging = null;
         }
      }
      public event ProjectDataset.CounterRowChangeEventHandler CounterRowDeleted
      {
         add
         {
            Debug.Assert(m_CounterDeleted == null);
            if (m_CounterDeleted != null)
               ProjectData.CounterRowDeleted -= m_CounterDeleted;
            ProjectData.CounterRowDeleted += m_CounterDeleted = value;
         }
         remove
         {
            Debug.Assert(m_CounterDeleted == value);
            ProjectData.CounterRowDeleted -= value;
            m_CounterDeleted = null;
         }
      }
      public event ProjectDataset.MapRowChangeEventHandler MapRowChanged
      {
         add
         {
            Debug.Assert(m_MapChanged == null);
            if (m_MapChanged != null)
               ProjectData.MapRowChanged -= m_MapChanged;
            ProjectData.MapRowChanged += m_MapChanged = value;
         }
         remove
         {
            Debug.Assert(m_MapChanged == value);
            ProjectData.MapRowChanged -= value;
            m_MapChanged = null;
         }
      }
      public event ProjectDataset.MapRowChangeEventHandler MapRowChanging
      {
         add
         {
            Debug.Assert(m_MapChanging == null);
            if (m_MapChanging != null)
               ProjectData.MapRowChanging -= m_MapChanging;
            ProjectData.MapRowChanging += m_MapChanging = value;
         }
         remove
         {
            Debug.Assert(m_MapChanging == value);
            ProjectData.MapRowChanging -= value;
            m_MapChanging = null;
         }
      }
      public event ProjectDataset.MapRowChangeEventHandler MapRowDeleted
      {
         add
         {
            Debug.Assert(m_MapDeleted == null);
            if (m_MapDeleted != null)
               ProjectData.MapRowDeleted -= m_MapDeleted;
            ProjectData.MapRowDeleted += m_MapDeleted = value;
         }
         remove
         {
            Debug.Assert(m_MapDeleted == value);
            ProjectData.MapRowDeleted -= value;
            m_MapDeleted = null;
         }
      }
      public event ProjectDataset.LayerRowChangeEventHandler LayerRowChanged
      {
         add
         {
            Debug.Assert(m_LayerChanged == null);
            if (m_LayerChanged != null)
               ProjectData.LayerRowChanged -= m_LayerChanged;
            ProjectData.LayerRowChanged += m_LayerChanged = value;
         }
         remove
         {
            Debug.Assert(m_LayerChanged == value);
            ProjectData.LayerRowChanged -= value;
            m_LayerChanged = null;
         }
      }
      public event ProjectDataset.LayerRowChangeEventHandler LayerRowChanging
      {
         add
         {
            Debug.Assert(m_LayerChanging == null);
            if (m_LayerChanging != null)
               ProjectData.LayerRowChanging -= m_LayerChanging;
            ProjectData.LayerRowChanging += m_LayerChanging = value;
         }
         remove
         {
            Debug.Assert(m_LayerChanging == value);
            ProjectData.LayerRowChanging -= value;
            m_LayerChanging = null;
         }
      }
      public event ProjectDataset.LayerRowChangeEventHandler LayerRowDeleted
      {
         add
         {
            Debug.Assert(m_LayerDeleted == null);
            if (m_LayerDeleted != null)
               ProjectData.LayerRowDeleted -= m_LayerDeleted;
            ProjectData.LayerRowDeleted += m_LayerDeleted = value;
         }
         remove
         {
            Debug.Assert(m_LayerDeleted == value);
            ProjectData.LayerRowDeleted -= value;
            m_LayerDeleted = null;
         }
      }
      public event ProjectDataset.TileFrameRowChangeEventHandler TileFrameRowChanged
      {
         add
         {
            Debug.Assert(m_TileFrameChanged == null);
            if (m_TileFrameChanged != null)
               ProjectData.TileFrameRowChanged -= m_TileFrameChanged;
            ProjectData.TileFrameRowChanged += m_TileFrameChanged = value;
         }
         remove
         {
            Debug.Assert(m_TileFrameChanged == value);
            ProjectData.TileFrameRowChanged -= value;
            m_TileFrameChanged = null;
         }
      }
      public event ProjectDataset.TileFrameRowChangeEventHandler TileFrameRowChanging
      {
         add
         {
            Debug.Assert(m_TileFrameChanging == null);
            if (m_TileFrameChanging != null)
               ProjectData.TileFrameRowChanging -= m_TileFrameChanging;
            ProjectData.TileFrameRowChanging += m_TileFrameChanging = value;
         }
         remove
         {
            Debug.Assert(m_TileFrameChanging == value);
            ProjectData.TileFrameRowChanging -= value;
            m_TileFrameChanging = null;
         }
      }
      public event ProjectDataset.TileFrameRowChangeEventHandler TileFrameRowDeleted
      {
         add
         {
            Debug.Assert(m_TileFrameDeleted == null);
            if (m_TileFrameDeleted != null)
               ProjectData.TileFrameRowDeleted -= m_TileFrameDeleted;
            ProjectData.TileFrameRowDeleted += m_TileFrameDeleted = value;
         }
         remove
         {
            Debug.Assert(m_TileFrameDeleted == value);
            ProjectData.TileFrameRowDeleted -= value;
            m_TileFrameDeleted = null;
         }
      }
      public event ProjectDataset.CategoryRowChangeEventHandler CategoryRowChanged
      {
         add
         {
            Debug.Assert(m_CategoryChanged == null);
            if (m_CategoryChanged != null)
               ProjectData.CategoryRowChanged -= m_CategoryChanged;
            ProjectData.CategoryRowChanged += m_CategoryChanged = value;
         }
         remove
         {
            Debug.Assert(m_CategoryChanged == value);
            ProjectData.CategoryRowChanged -= value;
            m_CategoryChanged = null;
         }
      }
      public event ProjectDataset.CategoryRowChangeEventHandler CategoryRowChanging
      {
         add
         {
            Debug.Assert(m_CategoryChanging == null);
            if (m_CategoryChanging != null)
               ProjectData.CategoryRowChanging -= m_CategoryChanging;
            ProjectData.CategoryRowChanging += m_CategoryChanging = value;
         }
         remove
         {
            Debug.Assert(m_CategoryChanging == value);
            ProjectData.CategoryRowChanging -= value;
            m_CategoryChanging = null;
         }
      }
      public event ProjectDataset.CategoryRowChangeEventHandler CategoryRowDeleted
      {
         add
         {
            Debug.Assert(m_CategoryDeleted == null);
            if (m_CategoryDeleted != null)
               ProjectData.CategoryRowDeleted -= m_CategoryDeleted;
            ProjectData.CategoryRowDeleted += m_CategoryDeleted = value;
         }
         remove
         {
            Debug.Assert(m_CategoryDeleted == value);
            ProjectData.CategoryRowDeleted -= value;
            m_CategoryDeleted = null;
         }
      }
      public event ProjectDataset.CategoryTileRowChangeEventHandler CategoryTileRowChanged
      {
         add
         {
            Debug.Assert(m_CategoryTileChanged == null);
            if (m_CategoryTileChanged != null)
               ProjectData.CategoryTileRowChanged -= m_CategoryTileChanged;
            ProjectData.CategoryTileRowChanged += m_CategoryTileChanged = value;
         }
         remove
         {
            Debug.Assert(m_CategoryTileChanged == value);
            ProjectData.CategoryTileRowChanged -= value;
            m_CategoryTileChanged = null;
         }
      }
      public event ProjectDataset.CategoryTileRowChangeEventHandler CategoryTileRowChanging
      {
         add
         {
            Debug.Assert(m_CategoryTileChanging == null);
            if (m_CategoryTileChanging != null)
               ProjectData.CategoryTileRowChanging -= m_CategoryTileChanging;
            ProjectData.CategoryTileRowChanging += m_CategoryTileChanging = value;
         }
         remove
         {
            Debug.Assert(m_CategoryTileChanging == value);
            ProjectData.CategoryTileRowChanging -= value;
            m_CategoryTileChanging = null;
         }
      }
      public event ProjectDataset.CategoryTileRowChangeEventHandler CategoryTileRowDeleted
      {
         add
         {
            Debug.Assert(m_CategoryTileDeleted == null);
            if (m_CategoryTileDeleted != null)
               ProjectData.CategoryTileRowDeleted -= m_CategoryTileDeleted;
            ProjectData.CategoryTileRowDeleted += m_CategoryTileDeleted = value;
         }
         remove
         {
            Debug.Assert(m_CategoryTileDeleted == value);
            ProjectData.CategoryTileRowDeleted -= value;
            m_CategoryTileDeleted = null;
         }
      }
      public event ProjectDataset.SpriteDefinitionRowChangeEventHandler SpriteDefinitionRowChanged
      {
         add
         {
            Debug.Assert(m_SpriteDefinitionChanged == null);
            if (m_SpriteDefinitionChanged != null)
               ProjectData.SpriteDefinitionRowChanged -= m_SpriteDefinitionChanged;
            ProjectData.SpriteDefinitionRowChanged += m_SpriteDefinitionChanged = value;
         }
         remove
         {
            Debug.Assert(m_SpriteDefinitionChanged == value);
            ProjectData.SpriteDefinitionRowChanged -= value;
            m_SpriteDefinitionChanged = null;
         }
      }
      public event ProjectDataset.SpriteDefinitionRowChangeEventHandler SpriteDefinitionRowChanging
      {
         add
         {
            Debug.Assert(m_SpriteDefinitionChanging == null);
            if (m_SpriteDefinitionChanging != null)
               ProjectData.SpriteDefinitionRowChanging -= m_SpriteDefinitionChanging;
            ProjectData.SpriteDefinitionRowChanging += m_SpriteDefinitionChanging = value;
         }
         remove
         {
            Debug.Assert(m_SpriteDefinitionChanging == value);
            ProjectData.SpriteDefinitionRowChanging -= value;
            m_SpriteDefinitionChanging = null;
         }
      }
      public event ProjectDataset.SpriteDefinitionRowChangeEventHandler SpriteDefinitionRowDeleted
      {
         add
         {
            Debug.Assert(m_SpriteDefinitionDeleted == null);
            if (m_SpriteDefinitionDeleted != null)
               ProjectData.SpriteDefinitionRowDeleted -= m_SpriteDefinitionDeleted;
            ProjectData.SpriteDefinitionRowDeleted += m_SpriteDefinitionDeleted = value;
         }
         remove
         {
            Debug.Assert(m_SpriteDefinitionDeleted == value);
            ProjectData.SpriteDefinitionRowDeleted -= value;
            m_SpriteDefinitionDeleted = null;
         }
      }
      public event ProjectDataset.SpriteStateRowChangeEventHandler SpriteStateRowChanged
      {
         add
         {
            Debug.Assert(m_SpriteStateChanged == null);
            if (m_SpriteStateChanged != null)
               ProjectData.SpriteStateRowChanged -= m_SpriteStateChanged;
            ProjectData.SpriteStateRowChanged += m_SpriteStateChanged = value;
         }
         remove
         {
            Debug.Assert(m_SpriteStateChanged == value);
            ProjectData.SpriteStateRowChanged -= value;
            m_SpriteStateChanged = null;
         }
      }
      public event ProjectDataset.SpriteStateRowChangeEventHandler SpriteStateRowChanging
      {
         add
         {
            Debug.Assert(m_SpriteStateChanging == null);
            if (m_SpriteStateChanging != null)
               ProjectData.SpriteStateRowChanging -= m_SpriteStateChanging;
            ProjectData.SpriteStateRowChanging += m_SpriteStateChanging = value;
         }
         remove
         {
            Debug.Assert(m_SpriteStateChanging == value);
            ProjectData.SpriteStateRowChanging -= value;
            m_SpriteStateChanging = null;
         }
      }
      public event ProjectDataset.SpriteStateRowChangeEventHandler SpriteStateRowDeleted
      {
         add
         {
            Debug.Assert(m_SpriteStateDeleted == null);
            if (m_SpriteStateDeleted != null)
               ProjectData.SpriteStateRowDeleted -= m_SpriteStateDeleted;
            ProjectData.SpriteStateRowDeleted += m_SpriteStateDeleted = value;
         }
         remove
         {
            Debug.Assert(m_SpriteStateDeleted == value);
            ProjectData.SpriteStateRowDeleted -= value;
            m_SpriteStateDeleted = null;
         }
      }
      public event ProjectDataset.SpriteFrameRowChangeEventHandler SpriteFrameRowChanged
      {
         add
         {
            Debug.Assert(m_SpriteFrameChanged == null);
            if (m_SpriteFrameChanged != null)
               ProjectData.SpriteFrameRowChanged -= m_SpriteFrameChanged;
            ProjectData.SpriteFrameRowChanged += m_SpriteFrameChanged = value;
         }
         remove
         {
            Debug.Assert(m_SpriteFrameChanged == value);
            ProjectData.SpriteFrameRowChanged -= value;
            m_SpriteFrameChanged = null;
         }
      }
      public event ProjectDataset.SpriteFrameRowChangeEventHandler SpriteFrameRowChanging
      {
         add
         {
            Debug.Assert(m_SpriteFrameChanging == null);
            if (m_SpriteFrameChanging != null)
               ProjectData.SpriteFrameRowChanging -= m_SpriteFrameChanging;
            ProjectData.SpriteFrameRowChanging += m_SpriteFrameChanging = value;
         }
         remove
         {
            Debug.Assert(m_SpriteFrameChanging == value);
            ProjectData.SpriteFrameRowChanging -= value;
            m_SpriteFrameChanging = null;
         }
      }
      public event ProjectDataset.SpriteFrameRowChangeEventHandler SpriteFrameRowDeleted
      {
         add
         {
            Debug.Assert(m_SpriteFrameDeleted == null);
            if (m_SpriteFrameDeleted != null)
               ProjectData.SpriteFrameRowDeleted -= m_SpriteFrameDeleted;
            ProjectData.SpriteFrameRowDeleted += m_SpriteFrameDeleted = value;
         }
         remove
         {
            Debug.Assert(m_SpriteFrameDeleted == value);
            ProjectData.SpriteFrameRowDeleted -= value;
            m_SpriteFrameDeleted = null;
         }
      }
      public event ProjectDataset.SpriteRuleRowChangeEventHandler SpriteRuleRowChanged
      {
         add
         {
            Debug.Assert(m_SpriteRuleChanged == null);
            if (m_SpriteRuleChanged != null)
               ProjectData.SpriteRuleRowChanged -= m_SpriteRuleChanged;
            ProjectData.SpriteRuleRowChanged += m_SpriteRuleChanged = value;
         }
         remove
         {
            Debug.Assert(m_SpriteRuleChanged == value);
            ProjectData.SpriteRuleRowChanged -= value;
            m_SpriteRuleChanged = null;
         }
      }
      public event ProjectDataset.SpriteRuleRowChangeEventHandler SpriteRuleRowChanging
      {
         add
         {
            Debug.Assert(m_SpriteRuleChanging == null);
            if (m_SpriteRuleChanging != null)
               ProjectData.SpriteRuleRowChanging -= m_SpriteRuleChanging;
            ProjectData.SpriteRuleRowChanging += m_SpriteRuleChanging = value;
         }
         remove
         {
            Debug.Assert(m_SpriteRuleChanging == value);
            ProjectData.SpriteRuleRowChanging -= value;
            m_SpriteRuleChanging = null;
         }
      }
      public event ProjectDataset.SpriteRuleRowChangeEventHandler SpriteRuleRowDeleted
      {
         add
         {
            Debug.Assert(m_SpriteRuleDeleted == null);
            if (m_SpriteRuleDeleted != null)
               ProjectData.SpriteRuleRowDeleted -= m_SpriteRuleDeleted;
            ProjectData.SpriteRuleRowDeleted += m_SpriteRuleDeleted = value;
         }
         remove
         {
            Debug.Assert(m_SpriteRuleDeleted == value);
            ProjectData.SpriteRuleRowDeleted -= value;
            m_SpriteRuleDeleted = null;
         }
      }
      public event ProjectDataset.SpriteParameterRowChangeEventHandler SpriteParameterRowChanged
      {
         add
         {
            Debug.Assert(m_SpriteParameterChanged == null);
            if (m_SpriteParameterChanged != null)
               ProjectData.SpriteParameterRowChanged -= m_SpriteParameterChanged;
            ProjectData.SpriteParameterRowChanged += m_SpriteParameterChanged = value;
         }
         remove
         {
            Debug.Assert(m_SpriteParameterChanged == value);
            ProjectData.SpriteParameterRowChanged -= value;
            m_SpriteParameterChanged = null;
         }
      }
      public event ProjectDataset.SpriteParameterRowChangeEventHandler SpriteParameterRowChanging
      {
         add
         {
            Debug.Assert(m_SpriteParameterChanging == null);
            if (m_SpriteParameterChanging != null)
               ProjectData.SpriteParameterRowChanging -= m_SpriteParameterChanging;
            ProjectData.SpriteParameterRowChanging += m_SpriteParameterChanging = value;
         }
         remove
         {
            Debug.Assert(m_SpriteParameterChanging == value);
            ProjectData.SpriteParameterRowChanging -= value;
            m_SpriteParameterChanging = null;
         }
      }
      public event ProjectDataset.SpriteParameterRowChangeEventHandler SpriteParameterRowDeleted
      {
         add
         {
            Debug.Assert(m_SpriteParameterDeleted == null);
            if (m_SpriteParameterDeleted != null)
               ProjectData.SpriteParameterRowDeleted -= m_SpriteParameterDeleted;
            ProjectData.SpriteParameterRowDeleted += m_SpriteParameterDeleted = value;
         }
         remove
         {
            Debug.Assert(m_SpriteParameterDeleted == value);
            ProjectData.SpriteParameterRowDeleted -= value;
            m_SpriteParameterDeleted = null;
         }
      }
      public event ProjectDataset.SpriteCategoryRowChangeEventHandler SpriteCategoryRowChanged
      {
         add
         {
            Debug.Assert(m_SpriteCategoryChanged == null);
            if (m_SpriteCategoryChanged != null)
               ProjectData.SpriteCategoryRowChanged -= m_SpriteCategoryChanged;
            ProjectData.SpriteCategoryRowChanged += m_SpriteCategoryChanged = value;
         }
         remove
         {
            Debug.Assert(m_SpriteCategoryChanged == value);
            ProjectData.SpriteCategoryRowChanged -= value;
            m_SpriteCategoryChanged = null;
         }
      }
      public event ProjectDataset.SpriteCategoryRowChangeEventHandler SpriteCategoryRowChanging
      {
         add
         {
            Debug.Assert(m_SpriteCategoryChanging == null);
            if (m_SpriteCategoryChanging != null)
               ProjectData.SpriteCategoryRowChanging -= m_SpriteCategoryChanging;
            ProjectData.SpriteCategoryRowChanging += m_SpriteCategoryChanging = value;
         }
         remove
         {
            Debug.Assert(m_SpriteCategoryChanging == value);
            ProjectData.SpriteCategoryRowChanging -= value;
            m_SpriteCategoryChanging = null;
         }
      }
      public event ProjectDataset.SpriteCategoryRowChangeEventHandler SpriteCategoryRowDeleted
      {
         add
         {
            Debug.Assert(m_SpriteCategoryDeleted == null);
            if (m_SpriteCategoryDeleted != null)
               ProjectData.SpriteCategoryRowDeleted -= m_SpriteCategoryDeleted;
            ProjectData.SpriteCategoryRowDeleted += m_SpriteCategoryDeleted = value;
         }
         remove
         {
            Debug.Assert(m_SpriteCategoryDeleted == value);
            ProjectData.SpriteCategoryRowDeleted -= value;
            m_SpriteCategoryDeleted = null;
         }
      }
      public event ProjectDataset.SpriteCategorySpriteRowChangeEventHandler SpriteCategorySpriteRowChanged
      {
         add
         {
            Debug.Assert(m_SpriteCategorySpriteChanged == null);
            if (m_SpriteCategorySpriteChanged != null)
               ProjectData.SpriteCategorySpriteRowChanged -= m_SpriteCategorySpriteChanged;
            ProjectData.SpriteCategorySpriteRowChanged += m_SpriteCategorySpriteChanged = value;
         }
         remove
         {
            Debug.Assert(m_SpriteCategorySpriteChanged == value);
            ProjectData.SpriteCategorySpriteRowChanged -= value;
            m_SpriteCategorySpriteChanged = null;
         }
      }
      public event ProjectDataset.SpriteCategorySpriteRowChangeEventHandler SpriteCategorySpriteRowChanging
      {
         add
         {
            Debug.Assert(m_SpriteCategorySpriteChanging == null);
            if (m_SpriteCategorySpriteChanging != null)
               ProjectData.SpriteCategorySpriteRowChanging -= m_SpriteCategorySpriteChanging;
            ProjectData.SpriteCategorySpriteRowChanging += m_SpriteCategorySpriteChanging = value;
         }
         remove
         {
            Debug.Assert(m_SpriteCategorySpriteChanging == value);
            ProjectData.SpriteCategorySpriteRowChanging -= value;
            m_SpriteCategorySpriteChanging = null;
         }
      }
      public event ProjectDataset.SpriteCategorySpriteRowChangeEventHandler SpriteCategorySpriteRowDeleted
      {
         add
         {
            Debug.Assert(m_SpriteCategorySpriteDeleted == null);
            if (m_SpriteCategorySpriteDeleted != null)
               ProjectData.SpriteCategorySpriteRowDeleted -= m_SpriteCategorySpriteDeleted;
            ProjectData.SpriteCategorySpriteRowDeleted += m_SpriteCategorySpriteDeleted = value;
         }
         remove
         {
            Debug.Assert(m_SpriteCategorySpriteDeleted == value);
            ProjectData.SpriteCategorySpriteRowDeleted -= value;
            m_SpriteCategorySpriteDeleted = null;
         }
      }
      public event ProjectDataset.SpriteRowChangeEventHandler SpriteRowChanged
      {
         add
         {
            Debug.Assert(m_SpriteChanged == null);
            if (m_SpriteChanged != null)
               ProjectData.SpriteRowChanged -= m_SpriteChanged;
            ProjectData.SpriteRowChanged += m_SpriteChanged = value;
         }
         remove
         {
            Debug.Assert(m_SpriteChanged == value);
            ProjectData.SpriteRowChanged -= value;
            m_SpriteChanged = null;
         }
      }
      public event ProjectDataset.SpriteRowChangeEventHandler SpriteRowChanging
      {
         add
         {
            Debug.Assert(m_SpriteChanging == null);
            if (m_SpriteChanging != null)
               ProjectData.SpriteRowChanging -= m_SpriteChanging;
            ProjectData.SpriteRowChanging += m_SpriteChanging = value;
         }
         remove
         {
            Debug.Assert(m_SpriteChanging == value);
            ProjectData.SpriteRowChanging -= value;
            m_SpriteChanging = null;
         }
      }
      public event ProjectDataset.SpriteRowChangeEventHandler SpriteRowDeleted
      {
         add
         {
            Debug.Assert(m_SpriteDeleted == null);
            if (m_SpriteDeleted != null)
               ProjectData.SpriteRowDeleted -= m_SpriteDeleted;
            ProjectData.SpriteRowDeleted += m_SpriteDeleted = value;
         }
         remove
         {
            Debug.Assert(m_SpriteDeleted == value);
            ProjectData.SpriteRowDeleted -= value;
            m_SpriteDeleted = null;
         }
      }
      public event ProjectDataset.ParameterValueRowChangeEventHandler ParameterValueRowChanged
      {
         add
         {
            Debug.Assert(m_ParameterValueChanged == null);
            if (m_ParameterValueChanged != null)
               ProjectData.ParameterValueRowChanged -= m_ParameterValueChanged;
            ProjectData.ParameterValueRowChanged += m_ParameterValueChanged = value;
         }
         remove
         {
            Debug.Assert(m_ParameterValueChanged == value);
            ProjectData.ParameterValueRowChanged -= value;
            m_ParameterValueChanged = null;
         }
      }
      public event ProjectDataset.ParameterValueRowChangeEventHandler ParameterValueRowChanging
      {
         add
         {
            Debug.Assert(m_ParameterValueChanging == null);
            if (m_ParameterValueChanging != null)
               ProjectData.ParameterValueRowChanging -= m_ParameterValueChanging;
            ProjectData.ParameterValueRowChanging += m_ParameterValueChanging = value;
         }
         remove
         {
            Debug.Assert(m_ParameterValueChanging == value);
            ProjectData.ParameterValueRowChanging -= value;
            m_ParameterValueChanging = null;
         }
      }
      public event ProjectDataset.ParameterValueRowChangeEventHandler ParameterValueRowDeleted
      {
         add
         {
            Debug.Assert(m_ParameterValueDeleted == null);
            if (m_ParameterValueDeleted != null)
               ProjectData.ParameterValueRowDeleted -= m_ParameterValueDeleted;
            ProjectData.ParameterValueRowDeleted += m_ParameterValueDeleted = value;
         }
         remove
         {
            Debug.Assert(m_ParameterValueDeleted == value);
            ProjectData.ParameterValueRowDeleted -= value;
            m_ParameterValueDeleted = null;
         }
      }
      public event ProjectDataset.SolidityRowChangeEventHandler SolidityRowChanged
      {
         add
         {
            Debug.Assert(m_SolidityChanged == null);
            if (m_SolidityChanged != null)
               ProjectData.SolidityRowChanged -= m_SolidityChanged;
            ProjectData.SolidityRowChanged += m_SolidityChanged = value;
         }
         remove
         {
            Debug.Assert(m_SolidityChanged == value);
            ProjectData.SolidityRowChanged -= value;
            m_SolidityChanged = null;
         }
      }
      public event ProjectDataset.SolidityRowChangeEventHandler SolidityRowChanging
      {
         add
         {
            Debug.Assert(m_SolidityChanging == null);
            if (m_SolidityChanging != null)
               ProjectData.SolidityRowChanging -= m_SolidityChanging;
            ProjectData.SolidityRowChanging += m_SolidityChanging = value;
         }
         remove
         {
            Debug.Assert(m_SolidityChanging == value);
            ProjectData.SolidityRowChanging -= value;
            m_SolidityChanging = null;
         }
      }
      public event ProjectDataset.SolidityRowChangeEventHandler SolidityRowDeleted
      {
         add
         {
            Debug.Assert(m_SolidityDeleted == null);
            if (m_SolidityDeleted != null)
               ProjectData.SolidityRowDeleted -= m_SolidityDeleted;
            ProjectData.SolidityRowDeleted += m_SolidityDeleted = value;
         }
         remove
         {
            Debug.Assert(m_SolidityDeleted == value);
            ProjectData.SolidityRowDeleted -= value;
            m_SolidityDeleted = null;
         }
      }
      public event ProjectDataset.SolidityShapeRowChangeEventHandler SolidityShapeRowChanged
      {
         add
         {
            Debug.Assert(m_SolidityShapeChanged == null);
            if (m_SolidityShapeChanged != null)
               ProjectData.SolidityShapeRowChanged -= m_SolidityShapeChanged;
            ProjectData.SolidityShapeRowChanged += m_SolidityShapeChanged = value;
         }
         remove
         {
            Debug.Assert(m_SolidityShapeChanged == value);
            ProjectData.SolidityShapeRowChanged -= value;
            m_SolidityShapeChanged = null;
         }
      }
      public event ProjectDataset.SolidityShapeRowChangeEventHandler SolidityShapeRowChanging
      {
         add
         {
            Debug.Assert(m_SolidityShapeChanging == null);
            if (m_SolidityShapeChanging != null)
               ProjectData.SolidityShapeRowChanging -= m_SolidityShapeChanging;
            ProjectData.SolidityShapeRowChanging += m_SolidityShapeChanging = value;
         }
         remove
         {
            Debug.Assert(m_SolidityShapeChanging == value);
            ProjectData.SolidityShapeRowChanging -= value;
            m_SolidityShapeChanging = null;
         }
      }
      public event ProjectDataset.SolidityShapeRowChangeEventHandler SolidityShapeRowDeleted
      {
         add
         {
            Debug.Assert(m_SolidityShapeDeleted == null);
            if (m_SolidityShapeDeleted != null)
               ProjectData.SolidityShapeRowDeleted -= m_SolidityShapeDeleted;
            ProjectData.SolidityShapeRowDeleted += m_SolidityShapeDeleted = value;
         }
         remove
         {
            Debug.Assert(m_SolidityShapeDeleted == value);
            ProjectData.SolidityShapeRowDeleted -= value;
            m_SolidityShapeDeleted = null;
         }
      }
      public event ProjectDataset.TileShapeRowChangeEventHandler TileShapeRowChanged
      {
         add
         {
            Debug.Assert(m_TileShapeChanged == null);
            if (m_TileShapeChanged != null)
               ProjectData.TileShapeRowChanged -= m_TileShapeChanged;
            ProjectData.TileShapeRowChanged += m_TileShapeChanged = value;
         }
         remove
         {
            Debug.Assert(m_TileShapeChanged == value);
            ProjectData.TileShapeRowChanged -= value;
            m_TileShapeChanged = null;
         }
      }
      public event ProjectDataset.TileShapeRowChangeEventHandler TileShapeRowChanging
      {
         add
         {
            Debug.Assert(m_TileShapeChanging == null);
            if (m_TileShapeChanging != null)
               ProjectData.TileShapeRowChanging -= m_TileShapeChanging;
            ProjectData.TileShapeRowChanging += m_TileShapeChanging = value;
         }
         remove
         {
            Debug.Assert(m_TileShapeChanging == value);
            ProjectData.TileShapeRowChanging -= value;
            m_TileShapeChanging = null;
         }
      }
      public event ProjectDataset.TileShapeRowChangeEventHandler TileShapeRowDeleted
      {
         add
         {
            Debug.Assert(m_TileShapeDeleted == null);
            if (m_TileShapeDeleted != null)
               ProjectData.TileShapeRowDeleted -= m_TileShapeDeleted;
            ProjectData.TileShapeRowDeleted += m_TileShapeDeleted = value;
         }
         remove
         {
            Debug.Assert(m_TileShapeDeleted == value);
            ProjectData.TileShapeRowDeleted -= value;
            m_TileShapeDeleted = null;
         }
      }
      public event ProjectDataset.CategoryFrameRowChangeEventHandler CategoryFrameRowChanged
      {
         add
         {
            Debug.Assert(m_CategoryFrameChanged == null);
            if (m_CategoryFrameChanged != null)
               ProjectData.CategoryFrameRowChanged -= m_CategoryFrameChanged;
            ProjectData.CategoryFrameRowChanged += m_CategoryFrameChanged = value;
         }
         remove
         {
            Debug.Assert(m_CategoryFrameChanged == value);
            ProjectData.CategoryFrameRowChanged -= value;
            m_CategoryFrameChanged = null;
         }
      }
      public event ProjectDataset.CategoryFrameRowChangeEventHandler CategoryFrameRowChanging
      {
         add
         {
            Debug.Assert(m_CategoryFrameChanging == null);
            if (m_CategoryFrameChanging != null)
               ProjectData.CategoryFrameRowChanging -= m_CategoryFrameChanging;
            ProjectData.CategoryFrameRowChanging += m_CategoryFrameChanging = value;
         }
         remove
         {
            Debug.Assert(m_CategoryFrameChanging == value);
            ProjectData.CategoryFrameRowChanging -= value;
            m_CategoryFrameChanging = null;
         }
      }
      public event ProjectDataset.CategoryFrameRowChangeEventHandler CategoryFrameRowDeleted
      {
         add
         {
            Debug.Assert(m_CategoryFrameDeleted == null);
            if (m_CategoryFrameDeleted != null)
               ProjectData.CategoryFrameRowDeleted -= m_CategoryFrameDeleted;
            ProjectData.CategoryFrameRowDeleted += m_CategoryFrameDeleted = value;
         }
         remove
         {
            Debug.Assert(m_CategoryFrameDeleted == value);
            ProjectData.CategoryFrameRowDeleted -= value;
            m_CategoryFrameDeleted = null;
         }
      }
      public event ProjectDataset.SourceCodeRowChangeEventHandler SourceCodeRowChanged
      {
         add
         {
            Debug.Assert(m_SourceCodeChanged == null);
            if (m_SourceCodeChanged != null)
               ProjectData.SourceCodeRowChanged -= m_SourceCodeChanged;
            ProjectData.SourceCodeRowChanged += m_SourceCodeChanged = value;
         }
         remove
         {
            Debug.Assert(m_SourceCodeChanged == value);
            ProjectData.SourceCodeRowChanged -= value;
            m_SourceCodeChanged = null;
         }
      }
      public event ProjectDataset.SourceCodeRowChangeEventHandler SourceCodeRowChanging
      {
         add
         {
            Debug.Assert(m_SourceCodeChanging == null);
            if (m_SourceCodeChanging != null)
               ProjectData.SourceCodeRowChanging -= m_SourceCodeChanging;
            ProjectData.SourceCodeRowChanging += m_SourceCodeChanging = value;
         }
         remove
         {
            Debug.Assert(m_SourceCodeChanging == value);
            ProjectData.SourceCodeRowChanging -= value;
            m_SourceCodeChanging = null;
         }
      }
      public event ProjectDataset.SourceCodeRowChangeEventHandler SourceCodeRowDeleted
      {
         add
         {
            Debug.Assert(m_SourceCodeDeleted == null);
            if (m_SourceCodeDeleted != null)
               ProjectData.SourceCodeRowDeleted -= m_SourceCodeDeleted;
            ProjectData.SourceCodeRowDeleted += m_SourceCodeDeleted = value;
         }
         remove
         {
            Debug.Assert(m_SourceCodeDeleted == value);
            ProjectData.SourceCodeRowDeleted -= value;
            m_SourceCodeDeleted = null;
         }
      }
      public event System.EventHandler Clearing
      {
         add
         {
            Debug.Assert(m_Clearing == null);
            if (m_Clearing != null)
               ProjectData.Clearing -= m_Clearing;
            ProjectData.Clearing += m_Clearing = value;
         }
         remove
         {
            Debug.Assert(m_Clearing == value);
            ProjectData.Clearing -= value;
            m_Clearing = null;
         }
      }
   }
}
