using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SGDK2
{
   public partial class frmSpriteFunctionSelector : frmWizardBase
   {
      private ProjectDataset.LayerRow m_Layer;
      private ProjectDataset.PlanRuleRow m_Rule;
      private frmPlanEdit m_planEditor;

      public frmSpriteFunctionSelector(ProjectDataset.LayerRow drLayer, ProjectDataset.PlanRuleRow drRule, frmPlanEdit planEditor)
      {
         m_Layer = drLayer;
         m_Rule = drRule;
         m_planEditor = planEditor;
         InitializeComponent();
      }

      private void stepSelectSprite_InitFunction(object sender, EventArgs e)
      {
         string selText = null;
         if (lvwSprites.SelectedItems.Count > 0)
            selText = lvwSprites.SelectedItems[0].Text;
         lvwSprites.Items.Clear();
         foreach (ProjectDataset.SpriteRow sprite in m_Layer.GetSpriteRows())
         {
            ListViewItem lvi = lvwSprites.Items.Add(new ListViewItem(new string[] { sprite.Name, sprite.DefinitionName }));
            lvi.Tag = sprite;
            if (selText == sprite.Name)
               lvi.Selected = true;
         }
         lvwSprites.Sorting = SortOrder.Ascending;
      }

      private bool stepSelectSprite_ValidateFunction(StepInfo sender)
      {
         if (lvwSprites.SelectedItems.Count != 1)
         {
            MessageBox.Show(this, "You must select a sprite", "Select Sprite", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         return true;
      }

      private void stepSelectFunction_InitFunction(object sender, EventArgs e)
      {
         string selText = null;

         if (lvwFunctions.SelectedItems.Count > 0)
            selText = lvwFunctions.SelectedItems[0].Text;

         lvwFunctions.Items.Clear();

         try
         {
            CodeGenerator gen = new CodeGenerator();
            string errs;
            gen.GenerateLevel = CodeGenerator.CodeLevel.ExcludeRules;
            errs = gen.CompileTempAssembly(false);
            if ((errs != null) && (errs.Length > 0))
            {
               MessageBox.Show(this, "Unable to list sprite functions due to compile errors: " + errs + "\r\nTry again after resolving these errors.", "Select Sprite Function", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               Close();
               return;
            }

            ProjectDataset.SpriteDefinitionRow spriteDef = ProjectData.GetSpriteDefinition(((ProjectDataset.SpriteRow)lvwSprites.SelectedItems[0].Tag).DefinitionName);

            RemotingServices.IRemoteTypeInfo reflector = CodeGenerator.CreateInstanceAndUnwrap("RemoteReflector",
               CodeGenerator.SpritesNamespace + "." + CodeGenerator.NameToVariable(spriteDef.Name))
               as RemotingServices.IRemoteTypeInfo;

            RemotingServices.RemoteMethodInfo[] localRuleList = reflector.GetMethods();

            foreach (RemotingServices.RemoteMethodInfo mi in localRuleList)
            {
               if ((mi.Description != null) && (mi.Description.Length > 0))
               {
                  foreach (string allowedType in new string[]
                        {
                           typeof(Boolean).Name, typeof(Int32).Name, typeof(Int16).Name,
                           typeof(Double).Name, typeof(Single).Name, typeof(void).Name
                        })
                  {
                     if (string.Compare(allowedType, mi.ReturnType.Name) == 0)
                     {
                        ListViewItem lvi = lvwFunctions.Items.Add(mi.MethodName);
                        lvi.Tag = mi;
                        if (selText == mi.MethodName)
                           lvi.Selected = true;
                        break;
                     }
                  }
               }
            }
            lvwFunctions.Sorting = SortOrder.Ascending;
         }
         catch (System.Exception ex)
         {
            MessageBox.Show(this, "Error getting list of sprite functions: " + ex.Message, "Select Sprite Function", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
            return;
         }
      }

      private bool stepSelectFunction_ValidateFunction(StepInfo sender)
      {
         if (lvwFunctions.SelectedItems.Count != 1)
         {
            MessageBox.Show(this, "You must select a function.", "Select Function", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }
         return true;
      }

      private bool stepParameters_IsApplicableFunction(StepInfo sender)
      {
         return ((RemotingServices.RemoteMethodInfo)lvwFunctions.SelectedItems[0].Tag).Arguments.Length > 0;
      }

      private void stepParameters_InitFunction(object sender, EventArgs e)
      {
         string[] paramVals = new string[3];

         paramVals[0] = cboParam1.Text;
         paramVals[1] = cboParam2.Text;
         paramVals[2] = cboParam3.Text;

         m_planEditor.m_SpriteContext = new frmPlanEdit.SpriteCodeRef((ProjectDataset.SpriteRow)lvwSprites.SelectedItems[0].Tag);
         RemotingServices.RemoteMethodInfo mi = (RemotingServices.RemoteMethodInfo)lvwFunctions.SelectedItems[0].Tag;
         lblParam3.Visible = cboParam3.Visible = mi.Arguments.Length >= 3;
         lblParam2.Visible = cboParam2.Visible = mi.Arguments.Length >= 2;
         m_planEditor.PopulateParameter(lblParam1, cboParam1, mi.Arguments[0], true);
         cboParam1.Text = paramVals[0];
         if (mi.Arguments.Length > 1)
         {
            m_planEditor.PopulateParameter(lblParam2, cboParam2, mi.Arguments[1], true);
            cboParam2.Text = paramVals[1];
         }
         if (mi.Arguments.Length > 2)
         {
            m_planEditor.PopulateParameter(lblParam3, cboParam3, mi.Arguments[2], true);
            cboParam3.Text = paramVals[2];
         }
      }

      private void cboParam_SelectedIndexChanged(object sender, EventArgs e)
      {
         ComboBox source = (ComboBox)sender;
         if (source.SelectedIndex < 0)
            return;
         frmSelectSpriteParameter frm;
         if ((source.Items[source.SelectedIndex].Equals(frmPlanEdit.SelectSpriteParameterItem)) ||
            (source.Items[source.SelectedIndex].Equals(frmPlanEdit.SelectWritableSpriteParameterItem)))
         {
            bool isRef = (source.Items[source.SelectedIndex].Equals(frmPlanEdit.SelectWritableSpriteParameterItem));
            using (frm = new frmSelectSpriteParameter(m_Layer))
            {
               if (DialogResult.OK == frm.ShowDialog(this))
               {
                  m_planEditor.m_SpriteContext = new frmPlanEdit.SpriteCodeRef(frm.SpriteRow);
                  string newSel = (isRef ? "ref " : "") + m_planEditor.m_SpriteContext.ToString() + "." + CodeGenerator.NameToVariable(frm.SpriteParameterRow.Name);
                  int selIdx = source.FindStringExact(newSel);
                  if (selIdx >= 0)
                     source.SelectedIndex = selIdx;
                  else
                     source.SelectedIndex = source.Items.Add(newSel);
               }
               else
                  source.SelectedIndex = -1;
            }
         }
      }

      private void cboParam_SelectionChangeCommitted(object sender, EventArgs e)
      {
         ComboBox source = (ComboBox)sender;
         if ((source.SelectedIndex >= 0) && (source.Items[source.SelectedIndex] is EnumOptionSelector))
         {
            string oldText = source.Text;
            string newValue = frmSpecifyFlags.GetOptions(this, (EnumOptionSelector)source.Items[source.SelectedIndex], oldText);
            if (newValue == null)
            {
               int selIdx = source.FindStringExact(oldText);
               if (selIdx >= 0)
                  source.SelectedIndex = selIdx;
               else
                  source.SelectedIndex = source.Items.Add(oldText);
            }
            else
            {
               int selIdx = source.FindStringExact(newValue);
               if (selIdx >= 0)
                  source.SelectedIndex = selIdx;
               else
                  source.SelectedIndex = source.Items.Add(newValue);
            }
         }
         else if ((source.SelectedIndex >= 0) && (source.Items[source.SelectedIndex] is string)
            && (string.Compare((string)source.Items[source.SelectedIndex], frmEditMessage.messageEditorItem) == 0))
         {
            string oldText = source.Text;
            string newText = frmEditMessage.EditMessage(source.Text, this);
            if (newText == null)
               newText = oldText;
            int selIdx = source.FindStringExact(newText);
            if (selIdx >= 0)
               source.SelectedIndex = selIdx;
            else
               source.SelectedIndex = source.Items.Add(newText);
         }
      }

      private void stepFinish_InitFunction(object sender, EventArgs e)
      {
         System.Text.StringBuilder sb = new StringBuilder();
         RemotingServices.RemoteMethodInfo mi = ((RemotingServices.RemoteMethodInfo)lvwFunctions.SelectedItems[0].Tag);
         sb.AppendLine(string.Format("Function: {0}.m_{1}.{2}", CodeGenerator.SpritePlanParentField, 
            CodeGenerator.NameToVariable(((ProjectDataset.SpriteRow)lvwSprites.SelectedItems[0].Tag).Name),
            mi.MethodName));
         if (mi.Arguments.Length > 0)
            sb.AppendLine(string.Format("{0}: {1}", mi.Arguments[0].Name, cboParam1.Text));
         if (mi.Arguments.Length > 1)
            sb.AppendLine(string.Format("{0}: {1}", mi.Arguments[1].Name, cboParam2.Text));
         if (mi.Arguments.Length > 2)
            sb.AppendLine(string.Format("{0}: {1}", mi.Arguments[2].Name, cboParam3.Text));
         txtFinish.Text = sb.ToString();
      }

      private bool stepFinish_ValidateFunction(StepInfo sender)
      {
         RemotingServices.RemoteMethodInfo mi = ((RemotingServices.RemoteMethodInfo)lvwFunctions.SelectedItems[0].Tag);
         m_Rule.Function = string.Format("{0}.m_{1}.{2}", CodeGenerator.SpritePlanParentField,
            CodeGenerator.NameToVariable(((ProjectDataset.SpriteRow)lvwSprites.SelectedItems[0].Tag).Name),
            mi.MethodName);
         if (mi.Arguments.Length > 0)
            m_Rule.Parameter1 = cboParam1.Text;
         else
            m_Rule.Parameter1 = string.Empty;
         if (mi.Arguments.Length > 1)
            m_Rule.Parameter2 = cboParam2.Text;
         else
            m_Rule.Parameter2 = string.Empty;
         if (mi.Arguments.Length > 2)
            m_Rule.Parameter3 = cboParam3.Text;
         else
            m_Rule.Parameter3 = string.Empty;
         return true;
      }
   }
}
