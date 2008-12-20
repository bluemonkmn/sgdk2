using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SGDK2
{
   public partial class frmConvertToFunction : frmWizardBase
   {
      private ProjectDataset.SpriteRuleRow[] m_spriteRules = null;
      private ProjectDataset.PlanRuleRow[] m_planRules = null;
      private System.Collections.Specialized.StringCollection m_reserved;

      public frmConvertToFunction(ProjectDataset.SpriteRuleRow[] spriteRules, System.Collections.Specialized.StringCollection reserved)
      {
         InitializeComponent();
         m_spriteRules = spriteRules;
         m_reserved = reserved;
      }

      public frmConvertToFunction(ProjectDataset.PlanRuleRow[] planRules, System.Collections.Specialized.StringCollection reserved)
      {
         InitializeComponent();
         m_planRules = planRules;
         m_reserved = reserved;
      }

      private bool stepOutputOption_ValidateFunction(StepInfo sender)
      {
         if (rdoOutputClipboard.Checked || rdoOutputExistingFile.Checked || rdoOutputNewFile.Checked)
            return true;
         MessageBox.Show(this, "You must select an output option.", "Specify Output Option", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         return false;
      }

      private bool stepTargetName_IsApplicableFunction(StepInfo sender)
      {
         return rdoOutputExistingFile.Checked || rdoOutputNewFile.Checked;
      }

      private void stepTargetName_InitFunction(object sender, EventArgs e)
      {
         bool existing = rdoOutputExistingFile.Checked;
         cboCodeName.Visible = existing;
         txtCodeName.Visible = rdoOutputNewFile.Checked;
         if (existing)
         {
            string oldValue = cboCodeName.Text;
            int oldItem = -1;
            cboCodeName.Items.Clear();
            foreach (DataRowView drv in ProjectData.SourceCode.DefaultView)
            {
               int newIndex = cboCodeName.Items.Add(drv.Row);
               if (((ProjectDataset.SourceCodeRow)drv.Row).Name == oldValue)
                  oldItem = newIndex;
            }
            if (oldItem >= 0)
               cboCodeName.SelectedIndex = oldItem;
         }
      }

      private void chkAddCall_CheckedChanged(object sender, EventArgs e)
      {
         lblNewRuleName.Enabled = txtNewRuleName.Enabled = chkAddCall.Checked;
      }

      private void stepRuleOptions_InitFunction(object sender, EventArgs e)
      {
         chkAddCall.Enabled = (rdoOutputExistingFile.Checked || rdoOutputNewFile.Checked);
         txtNewRuleName.Enabled = chkAddCall.Enabled && chkAddCall.Checked;
      }

      private bool stepTargetName_ValidateFunction(StepInfo sender)
      {
         if (txtCodeName.Visible)
         {
            string validateText;
            if (txtCodeName.Text.EndsWith(".cs"))
            {
               validateText = txtCodeName.Text.Substring(0, txtCodeName.Text.Length - 3);
            }
            else
            {
               validateText = txtCodeName.Text;
               txtCodeName.Text = validateText + ".cs";
            }

            for (int i = 0; i < validateText.Length; i++)
            {
               if (!(char.IsLetterOrDigit(validateText, i) || (validateText[i] == '_')))
               {
                  MessageBox.Show(this, "Code object name may only contain digits, letters, underscores, and \".cs\" extension.", "Code Object Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return false;
               }
            }

            if (ProjectData.GetSourceCode(txtCodeName.Text) != null)
            {
               MessageBox.Show(this, txtCodeName.Text + " already exists", "Name Target", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }

            if (validateText.Length == 0)
            {
               MessageBox.Show(this, "Code object name must be specified.", "Name Target", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }
         }

         if (txtFunctionName.Text.Length <= 0)
         {
            MessageBox.Show(this, "Function name must be specified.", "Name Target", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }

         if (m_reserved.Contains(txtFunctionName.Text))
         {
            MessageBox.Show(this, "\"" + txtFunctionName.Text + "\" is already used.", "Name Target", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }

         if (char.IsDigit(txtFunctionName.Text[0]))
         {
            MessageBox.Show(this, "Function cannot begin with a digit.", "Name Target", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }

         for (int i = 0; i < txtFunctionName.Text.Length; i++)
            if (!(char.IsLetterOrDigit(txtFunctionName.Text,i) || (txtFunctionName.Text[i] == '_')))
            {
               MessageBox.Show(this, "Function name can only contain letters, digits, and underscores.", "Name Target", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }

         return true;
      }

      private bool stepRuleOptions_ValidateFunction(StepInfo sender)
      {
         if (chkAddCall.Checked)
         {
            if (txtNewRuleName.Text.Trim().Length == 0)
            {
               MessageBox.Show(this, "New rule name must be specified", "Rule Options", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }

            if (m_planRules != null)
            {
               if (ProjectData.GetPlanRule(m_planRules[0].SpritePlanRowParent, txtNewRuleName.Text) != null)
               {
                  MessageBox.Show(this, "The specified rule name already exists on this plan. Specify a unique name.", "Rule Options", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return false;
               }
            }
            else if (m_spriteRules != null)
            {
               if (ProjectData.GetSpriteRule(m_spriteRules[0].SpriteDefinitionRow, txtNewRuleName.Text) != null)
               {
                  MessageBox.Show(this, "The specified rule name already exists on this sprite. Specify a unique name.", "Rule Options", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return false;
               }
            }

         }
         return true;
      }

      private void stepFinish_InitFunction(object sender, EventArgs e)
      {
         System.Text.StringBuilder sb = new StringBuilder("The content of the following rules will be converted to source code:\r\n");

         if (m_spriteRules != null)
         {
            foreach (ProjectDataset.SpriteRuleRow rule in m_spriteRules)
            {
               sb.AppendLine(rule.Name);
            }
         }
         else if (m_planRules != null)
         {
            foreach (ProjectDataset.PlanRuleRow rule in m_planRules)
            {
               sb.AppendLine(rule.Name);
            }
         }

         if (rdoOutputClipboard.Checked)
         {
            sb.AppendLine("\r\nThe resulting code will be copied to the system clipboard.\r\n");
         }
         else if (rdoOutputExistingFile.Checked)
         {
            sb.AppendLine("\r\nThe resulting code will be added to a new function named \"" + txtFunctionName.Text +
               "\" in the existing \"" + ((ProjectDataset.SourceCodeRow)cboCodeName.SelectedItem).Name + "\" source code object.");
         }
         else if (rdoOutputNewFile.Checked)
         {
            sb.AppendLine("\r\nThe resulting code will be added to a new function named \"" + txtFunctionName.Text +
               "\" in a new source code object named \"" + txtCodeName.Text + "\".");
         }

         if (chkDeleteOld.Checked)
         {
            sb.AppendLine("The converted rules will be deleted.");
         }

         if (!rdoOutputClipboard.Checked && chkAddCall.Checked)
         {
            if (m_planRules != null)
               sb.AppendLine("A rule named \"" + txtNewRuleName.Text + "\" will be added to the plan to call the created function.");
            else
               sb.AppendLine("A rule named \"" + txtNewRuleName.Text + "\" will be added to the sprite to call the created function.");
         }

         txtFinish.Text = sb.ToString();
      }

      private bool stepFinish_ValidateFunction(StepInfo sender)
      {
         CodeGenerator cg = new CodeGenerator();
         cg.GeneratorOptions = new System.CodeDom.Compiler.CodeGeneratorOptions();
         cg.GeneratorOptions.IndentString = "   ";

         string code = string.Empty;
         string funcName = null;

         if (!rdoOutputClipboard.Checked)
            funcName = txtFunctionName.Text;

         if (m_spriteRules != null)
         {
            code = cg.ConvertRuleToCode(m_spriteRules, funcName);
         }
         else if (m_planRules != null)
         {
            code = cg.ConvertRuleToCode(m_planRules, funcName);
         }

         if (rdoOutputClipboard.Checked)
         {
            Clipboard.SetText(code);
         }
         else if (rdoOutputExistingFile.Checked)
         {
            ProjectDataset.SourceCodeRow src = (ProjectDataset.SourceCodeRow)cboCodeName.SelectedItem;
            if (src == null)
            {
               MessageBox.Show(this, "Failed to access code named " + cboCodeName.SelectedText, "Convert Rules to Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
               return false;
            }
            src.Text += "\r\n" + code;
         }
         else if (rdoOutputNewFile.Checked)
         {
            ProjectData.AddSourceCode(txtCodeName.Text, code, null, true, null);
         }

         if (!rdoOutputClipboard.Checked && chkAddCall.Checked)
         {
            if (m_spriteRules != null)
               ProjectData.InsertSpriteRule(m_spriteRules[0].SpriteDefinitionRow, txtNewRuleName.Text, "Do", -1, txtFunctionName.Text, null, null, null, null, false, false);
            else if (m_planRules != null)
               ProjectData.InsertPlanRule(m_planRules[0].SpritePlanRowParent, txtNewRuleName.Text, "Do", -1, txtFunctionName.Text, null, null, null, null, false, false);
         }

         if (chkDeleteOld.Checked)
         {
            if (m_spriteRules != null)
               foreach (ProjectDataset.SpriteRuleRow row in m_spriteRules)
                  ProjectData.DeleteSpriteRule(row);
            else if (m_planRules != null)
               foreach (ProjectDataset.PlanRuleRow row in m_planRules)
                  ProjectData.DeletePlanRule(row);
         }
         return true;
      }
   }
}
