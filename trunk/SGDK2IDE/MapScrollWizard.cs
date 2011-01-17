/*
 * Scrolling Game Development Kit 2.0
 * See AssemblyInfo.cs for copyright/licensing details
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SGDK2
{
	public class frmMapScrollWizard : SGDK2.frmWizardBase
	{
      private System.Windows.Forms.Panel pnlChooseLayers;
      private SGDK2.frmWizardBase.StepInfo ChooseLayer;
      private SGDK2.frmWizardBase.StepInfo Intro;
      private System.Windows.Forms.Label lblInfo;
      private System.Windows.Forms.CheckedListBox chlLayers;
      private System.Windows.Forms.Label lblLayerInstructions;
		private System.ComponentModel.IContainer components = null;
      private System.Windows.Forms.Panel pnlResults;
      private SGDK2.frmWizardBase.StepInfo Results;
      private System.Windows.Forms.Label lblResults;
      private System.Windows.Forms.TextBox txtResults;
      private ProjectDataset.MapRow m_Map = null;
      private int m_nScrollWidth = 0;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.RadioButton rdoConstDepth;
      private System.Windows.Forms.RadioButton rdoMaxView;
      private System.Windows.Forms.Label lblNote;
      private int m_nScrollHeight = 0;

		public frmMapScrollWizard(ProjectDataset.MapRow mapRow)  
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			m_Map = mapRow;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.pnlChooseLayers = new System.Windows.Forms.Panel();
         this.lblLayerInstructions = new System.Windows.Forms.Label();
         this.chlLayers = new System.Windows.Forms.CheckedListBox();
         this.ChooseLayer = new SGDK2.frmWizardBase.StepInfo();
         this.lblInfo = new System.Windows.Forms.Label();
         this.Intro = new SGDK2.frmWizardBase.StepInfo();
         this.panel1 = new System.Windows.Forms.Panel();
         this.lblNote = new System.Windows.Forms.Label();
         this.rdoMaxView = new System.Windows.Forms.RadioButton();
         this.rdoConstDepth = new System.Windows.Forms.RadioButton();
         this.pnlResults = new System.Windows.Forms.Panel();
         this.txtResults = new System.Windows.Forms.TextBox();
         this.lblResults = new System.Windows.Forms.Label();
         this.Results = new SGDK2.frmWizardBase.StepInfo();
         this.pnlChooseLayers.SuspendLayout();
         this.panel1.SuspendLayout();
         this.pnlResults.SuspendLayout();
         this.SuspendLayout();
         // 
         // pnlChooseLayers
         // 
         this.pnlChooseLayers.Controls.Add(this.lblLayerInstructions);
         this.pnlChooseLayers.Controls.Add(this.chlLayers);
         this.pnlChooseLayers.Location = new System.Drawing.Point(-10168, 42);
         this.pnlChooseLayers.Name = "pnlChooseLayers";
         this.pnlChooseLayers.Size = new System.Drawing.Size(282, 231);
         this.pnlChooseLayers.TabIndex = 4;
         // 
         // lblLayerInstructions
         // 
         this.lblLayerInstructions.Location = new System.Drawing.Point(24, 16);
         this.lblLayerInstructions.Name = "lblLayerInstructions";
         this.lblLayerInstructions.Size = new System.Drawing.Size(232, 48);
         this.lblLayerInstructions.TabIndex = 1;
         this.lblLayerInstructions.Text = "Select which layers should be able to scroll entirely into the map view.";
         // 
         // chlLayers
         // 
         this.chlLayers.CheckOnClick = true;
         this.chlLayers.Location = new System.Drawing.Point(24, 64);
         this.chlLayers.Name = "chlLayers";
         this.chlLayers.Size = new System.Drawing.Size(232, 154);
         this.chlLayers.TabIndex = 0;
         // 
         // ChooseLayer
         // 
         this.ChooseLayer.StepControl = this.pnlChooseLayers;
         this.ChooseLayer.TitleText = "Choose Layers";
         this.ChooseLayer.InitFunction += new System.EventHandler(this.ChooseLayer_InitFunction);
         // 
         // lblInfo
         // 
         this.lblInfo.Location = new System.Drawing.Point(8, 8);
         this.lblInfo.Name = "lblInfo";
         this.lblInfo.Size = new System.Drawing.Size(264, 48);
         this.lblInfo.TabIndex = 0;
         this.lblInfo.Text = "This wizard will help determine an appropriate ScrollSize for a map based on the " +
            "map\'s layers and view size.  Multiple modes are available:";
         // 
         // Intro
         // 
         this.Intro.StepControl = this.panel1;
         this.Intro.TitleText = "Introduction";
         // 
         // panel1
         // 
         this.panel1.Controls.Add(this.lblNote);
         this.panel1.Controls.Add(this.rdoMaxView);
         this.panel1.Controls.Add(this.rdoConstDepth);
         this.panel1.Controls.Add(this.lblInfo);
         this.panel1.Location = new System.Drawing.Point(168, 42);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(280, 231);
         this.panel1.TabIndex = 6;
         // 
         // lblNote
         // 
         this.lblNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
         this.lblNote.Location = new System.Drawing.Point(8, 160);
         this.lblNote.Name = "lblNote";
         this.lblNote.Size = new System.Drawing.Size(264, 64);
         this.lblNote.TabIndex = 3;
         this.lblNote.Text = "Note: This wizard may yield unexpected results for layers with a scroll rate othe" +
            "r than 1 when multiple views are active (more than 1 sub-view displayed in the m" +
            "ap\'s view).";
         // 
         // rdoMaxView
         // 
         this.rdoMaxView.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.rdoMaxView.Location = new System.Drawing.Point(8, 104);
         this.rdoMaxView.Name = "rdoMaxView";
         this.rdoMaxView.Size = new System.Drawing.Size(264, 48);
         this.rdoMaxView.TabIndex = 2;
         this.rdoMaxView.Text = "Maximum View - Ensure that all areas of all layers can be scrolled into the map\'s" +
            " view.";
         this.rdoMaxView.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // rdoConstDepth
         // 
         this.rdoConstDepth.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.rdoConstDepth.Checked = true;
         this.rdoConstDepth.Location = new System.Drawing.Point(8, 56);
         this.rdoConstDepth.Name = "rdoConstDepth";
         this.rdoConstDepth.Size = new System.Drawing.Size(264, 48);
         this.rdoConstDepth.TabIndex = 1;
         this.rdoConstDepth.TabStop = true;
         this.rdoConstDepth.Text = "Constant Depth - Ensure that all selected layers always fill the map view area an" +
            "d cannot be scrolled out of view.";
         this.rdoConstDepth.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // pnlResults
         // 
         this.pnlResults.Controls.Add(this.txtResults);
         this.pnlResults.Controls.Add(this.lblResults);
         this.pnlResults.Location = new System.Drawing.Point(-10168, 42);
         this.pnlResults.Name = "pnlResults";
         this.pnlResults.Size = new System.Drawing.Size(282, 231);
         this.pnlResults.TabIndex = 5;
         // 
         // txtResults
         // 
         this.txtResults.Location = new System.Drawing.Point(8, 56);
         this.txtResults.Multiline = true;
         this.txtResults.Name = "txtResults";
         this.txtResults.ReadOnly = true;
         this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtResults.Size = new System.Drawing.Size(264, 168);
         this.txtResults.TabIndex = 1;
         this.txtResults.Text = "";
         // 
         // lblResults
         // 
         this.lblResults.Location = new System.Drawing.Point(8, 16);
         this.lblResults.Name = "lblResults";
         this.lblResults.Size = new System.Drawing.Size(264, 40);
         this.lblResults.TabIndex = 0;
         this.lblResults.Text = "Given the information you have provided, the map\'s scroll size will be set as fol" +
            "lows:";
         // 
         // Results
         // 
         this.Results.StepControl = this.pnlResults;
         this.Results.TitleText = "Review";
         this.Results.InitFunction += new System.EventHandler(this.Results_InitFunction);
         this.Results.ValidateFunction += new SGDK2.frmWizardBase.ValidateFunctionEvent(this.Results_ValidateFunction);
         // 
         // frmMapScrollWizard
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(450, 313);
         this.Controls.Add(this.panel1);
         this.Controls.Add(this.pnlResults);
         this.Controls.Add(this.pnlChooseLayers);
         this.Name = "frmMapScrollWizard";
         this.Steps.Add(this.Intro);
         this.Steps.Add(this.ChooseLayer);
         this.Steps.Add(this.Results);
         this.Text = "Map Scrolling Wizard";
         this.Controls.SetChildIndex(this.pnlChooseLayers, 0);
         this.Controls.SetChildIndex(this.pnlResults, 0);
         this.Controls.SetChildIndex(this.panel1, 0);
         this.pnlChooseLayers.ResumeLayout(false);
         this.panel1.ResumeLayout(false);
         this.pnlResults.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion

      private void ChooseLayer_InitFunction(object sender, System.EventArgs e)
      {
         if (chlLayers.Items.Count != m_Map.GetLayerRows().Length)
         {
            chlLayers.Items.Clear();
            foreach(ProjectDataset.LayerRow layer in ProjectData.GetSortedLayers(m_Map))
            {
               chlLayers.Items.Add(layer.Name);
            }
         }
      }

      private void Results_InitFunction(object sender, System.EventArgs e)
      {
         m_nScrollWidth = -1;
         m_nScrollHeight = -1;
         int nWastedWidth = 0;
         string strWastedWidthLayer = null;
         string strWastedHeightLayer = null;
         int nWastedHeight = 0;

         foreach(string layerName in chlLayers.CheckedItems)
         {
            ProjectDataset.LayerRow layer = ProjectData.GetLayer(m_Map.Name, layerName);
            int layerMaxX = 0;
            int layerMaxY = 0;
            if (layer.ScrollRateX > 0)
               layerMaxX = (int)((layer.OffsetX +
                  ((layer.VirtualWidth == 0) ? layer.Width : layer.VirtualWidth) *
                  layer.TilesetRow.TileWidth - m_Map.ViewWidth) / layer.ScrollRateX) + m_Map.ViewWidth;
            if (layer.ScrollRateY > 0)
               layerMaxY = (int)((layer.OffsetY +
                  ((layer.VirtualHeight == 0) ? layer.Height : layer.VirtualHeight) *
                  layer.TilesetRow.TileHeight - m_Map.ViewHeight) / layer.ScrollRateY) + m_Map.ViewHeight;
            if (rdoConstDepth.Checked)
            {
               if ((m_nScrollWidth < 0) || (layerMaxX < m_nScrollWidth))
                  m_nScrollWidth = layerMaxX;
               if ((m_nScrollHeight < 0) || (layerMaxY < m_nScrollHeight))
                  m_nScrollHeight = layerMaxY;

               if (layerMaxX > nWastedWidth)
               {
                  strWastedWidthLayer = layerName;
                  nWastedWidth = layerMaxX;
               }
               if (layerMaxY > nWastedHeight)
               {
                  strWastedHeightLayer = layerName;
                  nWastedHeight = layerMaxY;
               }
            }
            else
            {
               if (layerMaxX > m_nScrollWidth)
                  m_nScrollWidth = layerMaxX;
               if (layerMaxY > m_nScrollHeight)
                  m_nScrollHeight = layerMaxY;
            }
         }
         if (m_nScrollWidth < 0)
            m_nScrollWidth = 0;
         if (m_nScrollHeight < 0)
            m_nScrollHeight = 0;

         txtResults.Text = "ScrollWidth: " + m_nScrollWidth.ToString() + "\r\n" +
                           "ScrollHeight: " + m_nScrollHeight.ToString() + "\r\n";

         if (rdoConstDepth.Checked)
         {
            if (nWastedWidth-m_nScrollWidth > 0)
               txtResults.Text += "Layer \"" + strWastedWidthLayer + "\" wastes the most horizontal scroll area at " + (nWastedWidth - m_nScrollWidth).ToString() + " units\r\n";
            if (nWastedHeight-m_nScrollHeight > 0)
               txtResults.Text += "Layer \"" + strWastedHeightLayer + "\" wastes the most vertical scroll area at " + (nWastedHeight - m_nScrollHeight).ToString() + " units\r\n";
            if ((nWastedWidth-m_nScrollWidth > 0) || (nWastedHeight-m_nScrollHeight>0))
               txtResults.Text += "(Wasted scroll area is area within a layer that can not be scrolled into view)\r\n";
         }
      }

      private bool Results_ValidateFunction(SGDK2.frmWizardBase.StepInfo sender)
      {
         m_Map.ScrollWidth = m_nScrollWidth;
         m_Map.ScrollHeight = m_nScrollHeight;
         return true;
      }
	}
}

