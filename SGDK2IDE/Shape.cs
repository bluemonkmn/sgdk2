using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
   /// <summary>
   /// Summary description for Shape.
   /// </summary>
   public class frmShape : System.Windows.Forms.Form
   {
      private ProjectDataset.TileShapeRow m_Shape;

      #region Windows Form Designer Components
      private System.Windows.Forms.Label lblName;
      private System.Windows.Forms.TextBox txtName;
      private System.Windows.Forms.Label lblExpression;
      private System.Windows.Forms.TextBox txtExpression;
      private System.Windows.Forms.GroupBox fraReference;
      private System.Windows.Forms.ImageList imlShapes;
      private System.Windows.Forms.Label lblUphill;
      private System.Windows.Forms.Label lblDownhill;
      private System.Windows.Forms.TextBox txtUphill;
      private System.Windows.Forms.TextBox txtDownhill;
      private System.Windows.Forms.TextBox txtDownCeil;
      private System.Windows.Forms.TextBox txtUpCeil;
      private System.Windows.Forms.Label lblDownCeil;
      private System.Windows.Forms.Label lblUpCeil;
      private System.Windows.Forms.TextBox txtSolid;
      private System.Windows.Forms.TextBox txtEmpty;
      private System.Windows.Forms.Label lblSolid;
      private System.Windows.Forms.TextBox txtDownhill2;
      private System.Windows.Forms.TextBox txtUphill2;
      private System.Windows.Forms.Label lblDownhill2;
      private System.Windows.Forms.Label lblUphill2;
      private System.Windows.Forms.PictureBox picPreview;
      private System.Windows.Forms.Button btnPreview;
      private System.Windows.Forms.TextBox txtWidth;
      private System.Windows.Forms.TextBox txtHeight;
      private System.Windows.Forms.Label lblX;
      private System.Windows.Forms.Label lblUphill3;
      private System.Windows.Forms.Label lblDownhill3;
      private System.Windows.Forms.TextBox txtUphill3;
      private System.Windows.Forms.TextBox txtDownhill3;
      private SGDK2.DataChangeNotifier dataMonitor;
      private System.ComponentModel.IContainer components;
      #endregion

      public frmShape()
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         String sName;
         Int32 nIdx = 1;
         do
            sName = "New Shape " + (nIdx++).ToString();
         while (ProjectData.GetTileShape(sName) != null);

         m_Shape = ProjectData.AddTileShape(sName, "true");
         txtName.Text = sName;
         txtExpression.Text = m_Shape.Shape;
      }

      public frmShape(ProjectDataset.TileShapeRow shape)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         m_Shape = shape;
         txtName.Text = m_Shape.Name;
         txtExpression.Text = m_Shape.Shape;
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
         }
         base.Dispose( disposing );
      }

      #region Windows Form Designer generated code
      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmShape));
         this.lblName = new System.Windows.Forms.Label();
         this.txtName = new System.Windows.Forms.TextBox();
         this.lblExpression = new System.Windows.Forms.Label();
         this.txtExpression = new System.Windows.Forms.TextBox();
         this.fraReference = new System.Windows.Forms.GroupBox();
         this.txtUphill3 = new System.Windows.Forms.TextBox();
         this.lblDownhill3 = new System.Windows.Forms.Label();
         this.imlShapes = new System.Windows.Forms.ImageList(this.components);
         this.lblUphill3 = new System.Windows.Forms.Label();
         this.txtDownhill3 = new System.Windows.Forms.TextBox();
         this.txtDownhill2 = new System.Windows.Forms.TextBox();
         this.txtUphill2 = new System.Windows.Forms.TextBox();
         this.lblDownhill2 = new System.Windows.Forms.Label();
         this.lblUphill2 = new System.Windows.Forms.Label();
         this.txtSolid = new System.Windows.Forms.TextBox();
         this.txtEmpty = new System.Windows.Forms.TextBox();
         this.lblSolid = new System.Windows.Forms.Label();
         this.txtDownCeil = new System.Windows.Forms.TextBox();
         this.txtUpCeil = new System.Windows.Forms.TextBox();
         this.lblDownCeil = new System.Windows.Forms.Label();
         this.lblUpCeil = new System.Windows.Forms.Label();
         this.txtDownhill = new System.Windows.Forms.TextBox();
         this.txtUphill = new System.Windows.Forms.TextBox();
         this.lblDownhill = new System.Windows.Forms.Label();
         this.lblUphill = new System.Windows.Forms.Label();
         this.picPreview = new System.Windows.Forms.PictureBox();
         this.btnPreview = new System.Windows.Forms.Button();
         this.txtWidth = new System.Windows.Forms.TextBox();
         this.txtHeight = new System.Windows.Forms.TextBox();
         this.lblX = new System.Windows.Forms.Label();
         this.dataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.fraReference.SuspendLayout();
         this.SuspendLayout();
         // 
         // lblName
         // 
         this.lblName.Location = new System.Drawing.Point(8, 8);
         this.lblName.Name = "lblName";
         this.lblName.Size = new System.Drawing.Size(72, 20);
         this.lblName.TabIndex = 0;
         this.lblName.Text = "Name:";
         this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtName
         // 
         this.txtName.Location = new System.Drawing.Point(80, 8);
         this.txtName.Name = "txtName";
         this.txtName.Size = new System.Drawing.Size(112, 20);
         this.txtName.TabIndex = 1;
         this.txtName.Text = "";
         this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
         this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
         // 
         // lblExpression
         // 
         this.lblExpression.Location = new System.Drawing.Point(8, 40);
         this.lblExpression.Name = "lblExpression";
         this.lblExpression.Size = new System.Drawing.Size(72, 20);
         this.lblExpression.TabIndex = 2;
         this.lblExpression.Text = "Expression:";
         this.lblExpression.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // txtExpression
         // 
         this.txtExpression.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtExpression.Location = new System.Drawing.Point(80, 40);
         this.txtExpression.Multiline = true;
         this.txtExpression.Name = "txtExpression";
         this.txtExpression.Size = new System.Drawing.Size(320, 40);
         this.txtExpression.TabIndex = 3;
         this.txtExpression.Text = "";
         this.txtExpression.Validated += new System.EventHandler(this.txtExpression_Validated);
         // 
         // fraReference
         // 
         this.fraReference.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.fraReference.Controls.Add(this.txtUphill3);
         this.fraReference.Controls.Add(this.lblDownhill3);
         this.fraReference.Controls.Add(this.lblUphill3);
         this.fraReference.Controls.Add(this.txtDownhill3);
         this.fraReference.Controls.Add(this.txtDownhill2);
         this.fraReference.Controls.Add(this.txtUphill2);
         this.fraReference.Controls.Add(this.lblDownhill2);
         this.fraReference.Controls.Add(this.lblUphill2);
         this.fraReference.Controls.Add(this.txtSolid);
         this.fraReference.Controls.Add(this.txtEmpty);
         this.fraReference.Controls.Add(this.lblSolid);
         this.fraReference.Controls.Add(this.txtDownCeil);
         this.fraReference.Controls.Add(this.txtUpCeil);
         this.fraReference.Controls.Add(this.lblDownCeil);
         this.fraReference.Controls.Add(this.lblUpCeil);
         this.fraReference.Controls.Add(this.txtDownhill);
         this.fraReference.Controls.Add(this.txtUphill);
         this.fraReference.Controls.Add(this.lblDownhill);
         this.fraReference.Controls.Add(this.lblUphill);
         this.fraReference.Location = new System.Drawing.Point(8, 192);
         this.fraReference.Name = "fraReference";
         this.fraReference.Size = new System.Drawing.Size(392, 152);
         this.fraReference.TabIndex = 4;
         this.fraReference.TabStop = false;
         this.fraReference.Text = "Expression Reference";
         // 
         // txtUphill3
         // 
         this.txtUphill3.Location = new System.Drawing.Point(40, 120);
         this.txtUphill3.Name = "txtUphill3";
         this.txtUphill3.ReadOnly = true;
         this.txtUphill3.Size = new System.Drawing.Size(152, 20);
         this.txtUphill3.TabIndex = 18;
         this.txtUphill3.Text = "x + y * 2 + 2 >= TileHeight";
         // 
         // lblDownhill3
         // 
         this.lblDownhill3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
         this.lblDownhill3.ImageIndex = 8;
         this.lblDownhill3.ImageList = this.imlShapes;
         this.lblDownhill3.Location = new System.Drawing.Point(200, 120);
         this.lblDownhill3.Name = "lblDownhill3";
         this.lblDownhill3.Size = new System.Drawing.Size(24, 20);
         this.lblDownhill3.TabIndex = 17;
         // 
         // imlShapes
         // 
         this.imlShapes.ImageSize = new System.Drawing.Size(15, 15);
         this.imlShapes.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlShapes.ImageStream")));
         this.imlShapes.TransparentColor = System.Drawing.Color.Magenta;
         // 
         // lblUphill3
         // 
         this.lblUphill3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
         this.lblUphill3.ImageIndex = 7;
         this.lblUphill3.ImageList = this.imlShapes;
         this.lblUphill3.Location = new System.Drawing.Point(8, 120);
         this.lblUphill3.Name = "lblUphill3";
         this.lblUphill3.Size = new System.Drawing.Size(24, 20);
         this.lblUphill3.TabIndex = 16;
         // 
         // txtDownhill3
         // 
         this.txtDownhill3.Location = new System.Drawing.Point(232, 120);
         this.txtDownhill3.Name = "txtDownhill3";
         this.txtDownhill3.ReadOnly = true;
         this.txtDownhill3.Size = new System.Drawing.Size(152, 20);
         this.txtDownhill3.TabIndex = 0;
         this.txtDownhill3.Text = "x * 2 <= y + TileWidth";
         // 
         // txtDownhill2
         // 
         this.txtDownhill2.Location = new System.Drawing.Point(232, 96);
         this.txtDownhill2.Name = "txtDownhill2";
         this.txtDownhill2.ReadOnly = true;
         this.txtDownhill2.Size = new System.Drawing.Size(152, 20);
         this.txtDownhill2.TabIndex = 15;
         this.txtDownhill2.Text = "x * 2 <= y";
         // 
         // txtUphill2
         // 
         this.txtUphill2.Location = new System.Drawing.Point(40, 96);
         this.txtUphill2.Name = "txtUphill2";
         this.txtUphill2.ReadOnly = true;
         this.txtUphill2.Size = new System.Drawing.Size(152, 20);
         this.txtUphill2.TabIndex = 14;
         this.txtUphill2.Text = "x + y * 2 + 2 >= TileHeight * 2";
         // 
         // lblDownhill2
         // 
         this.lblDownhill2.ImageIndex = 5;
         this.lblDownhill2.ImageList = this.imlShapes;
         this.lblDownhill2.Location = new System.Drawing.Point(200, 96);
         this.lblDownhill2.Name = "lblDownhill2";
         this.lblDownhill2.Size = new System.Drawing.Size(24, 20);
         this.lblDownhill2.TabIndex = 13;
         // 
         // lblUphill2
         // 
         this.lblUphill2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
         this.lblUphill2.ImageIndex = 4;
         this.lblUphill2.ImageList = this.imlShapes;
         this.lblUphill2.Location = new System.Drawing.Point(8, 96);
         this.lblUphill2.Name = "lblUphill2";
         this.lblUphill2.Size = new System.Drawing.Size(24, 20);
         this.lblUphill2.TabIndex = 12;
         // 
         // txtSolid
         // 
         this.txtSolid.Location = new System.Drawing.Point(232, 72);
         this.txtSolid.Name = "txtSolid";
         this.txtSolid.ReadOnly = true;
         this.txtSolid.Size = new System.Drawing.Size(152, 20);
         this.txtSolid.TabIndex = 11;
         this.txtSolid.Text = "true";
         // 
         // txtEmpty
         // 
         this.txtEmpty.Location = new System.Drawing.Point(40, 72);
         this.txtEmpty.Name = "txtEmpty";
         this.txtEmpty.ReadOnly = true;
         this.txtEmpty.Size = new System.Drawing.Size(152, 20);
         this.txtEmpty.TabIndex = 10;
         this.txtEmpty.Text = "false";
         // 
         // lblSolid
         // 
         this.lblSolid.ImageIndex = 6;
         this.lblSolid.ImageList = this.imlShapes;
         this.lblSolid.Location = new System.Drawing.Point(200, 72);
         this.lblSolid.Name = "lblSolid";
         this.lblSolid.Size = new System.Drawing.Size(24, 20);
         this.lblSolid.TabIndex = 9;
         // 
         // txtDownCeil
         // 
         this.txtDownCeil.Location = new System.Drawing.Point(232, 48);
         this.txtDownCeil.Name = "txtDownCeil";
         this.txtDownCeil.ReadOnly = true;
         this.txtDownCeil.Size = new System.Drawing.Size(152, 20);
         this.txtDownCeil.TabIndex = 7;
         this.txtDownCeil.Text = "x >= y";
         // 
         // txtUpCeil
         // 
         this.txtUpCeil.Location = new System.Drawing.Point(40, 48);
         this.txtUpCeil.Name = "txtUpCeil";
         this.txtUpCeil.ReadOnly = true;
         this.txtUpCeil.Size = new System.Drawing.Size(152, 20);
         this.txtUpCeil.TabIndex = 6;
         this.txtUpCeil.Text = "x+y < TileWidth";
         // 
         // lblDownCeil
         // 
         this.lblDownCeil.ImageIndex = 3;
         this.lblDownCeil.ImageList = this.imlShapes;
         this.lblDownCeil.Location = new System.Drawing.Point(200, 48);
         this.lblDownCeil.Name = "lblDownCeil";
         this.lblDownCeil.Size = new System.Drawing.Size(24, 20);
         this.lblDownCeil.TabIndex = 5;
         // 
         // lblUpCeil
         // 
         this.lblUpCeil.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
         this.lblUpCeil.ImageIndex = 2;
         this.lblUpCeil.ImageList = this.imlShapes;
         this.lblUpCeil.Location = new System.Drawing.Point(8, 48);
         this.lblUpCeil.Name = "lblUpCeil";
         this.lblUpCeil.Size = new System.Drawing.Size(24, 20);
         this.lblUpCeil.TabIndex = 4;
         // 
         // txtDownhill
         // 
         this.txtDownhill.Location = new System.Drawing.Point(232, 24);
         this.txtDownhill.Name = "txtDownhill";
         this.txtDownhill.ReadOnly = true;
         this.txtDownhill.Size = new System.Drawing.Size(152, 20);
         this.txtDownhill.TabIndex = 3;
         this.txtDownhill.Text = "x <= y";
         // 
         // txtUphill
         // 
         this.txtUphill.Location = new System.Drawing.Point(40, 24);
         this.txtUphill.Name = "txtUphill";
         this.txtUphill.ReadOnly = true;
         this.txtUphill.Size = new System.Drawing.Size(152, 20);
         this.txtUphill.TabIndex = 2;
         this.txtUphill.Text = "x+y+1 >= TileWidth";
         // 
         // lblDownhill
         // 
         this.lblDownhill.ImageIndex = 1;
         this.lblDownhill.ImageList = this.imlShapes;
         this.lblDownhill.Location = new System.Drawing.Point(200, 24);
         this.lblDownhill.Name = "lblDownhill";
         this.lblDownhill.Size = new System.Drawing.Size(24, 20);
         this.lblDownhill.TabIndex = 1;
         // 
         // lblUphill
         // 
         this.lblUphill.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
         this.lblUphill.ImageIndex = 0;
         this.lblUphill.ImageList = this.imlShapes;
         this.lblUphill.Location = new System.Drawing.Point(8, 24);
         this.lblUphill.Name = "lblUphill";
         this.lblUphill.Size = new System.Drawing.Size(24, 20);
         this.lblUphill.TabIndex = 0;
         // 
         // picPreview
         // 
         this.picPreview.Location = new System.Drawing.Point(80, 88);
         this.picPreview.Name = "picPreview";
         this.picPreview.Size = new System.Drawing.Size(97, 97);
         this.picPreview.TabIndex = 6;
         this.picPreview.TabStop = false;
         // 
         // btnPreview
         // 
         this.btnPreview.Location = new System.Drawing.Point(8, 88);
         this.btnPreview.Name = "btnPreview";
         this.btnPreview.Size = new System.Drawing.Size(64, 24);
         this.btnPreview.TabIndex = 7;
         this.btnPreview.Text = "&Preview";
         this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
         // 
         // txtWidth
         // 
         this.txtWidth.Location = new System.Drawing.Point(8, 120);
         this.txtWidth.Name = "txtWidth";
         this.txtWidth.Size = new System.Drawing.Size(24, 20);
         this.txtWidth.TabIndex = 8;
         this.txtWidth.Text = "16";
         // 
         // txtHeight
         // 
         this.txtHeight.Location = new System.Drawing.Point(48, 120);
         this.txtHeight.Name = "txtHeight";
         this.txtHeight.Size = new System.Drawing.Size(24, 20);
         this.txtHeight.TabIndex = 9;
         this.txtHeight.Text = "16";
         // 
         // lblX
         // 
         this.lblX.Location = new System.Drawing.Point(32, 120);
         this.lblX.Name = "lblX";
         this.lblX.Size = new System.Drawing.Size(16, 20);
         this.lblX.TabIndex = 10;
         this.lblX.Text = "x";
         this.lblX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // dataMonitor
         // 
         this.dataMonitor.Clearing += new System.EventHandler(this.dataMonitor_Clearing);
         this.dataMonitor.TileShapeRowDeleted += new SGDK2.ProjectDataset.TileShapeRowChangeEventHandler(this.dataMonitor_TileShapeRowDeleted);
         // 
         // frmShape
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(410, 359);
         this.Controls.Add(this.lblX);
         this.Controls.Add(this.txtHeight);
         this.Controls.Add(this.txtWidth);
         this.Controls.Add(this.txtExpression);
         this.Controls.Add(this.txtName);
         this.Controls.Add(this.btnPreview);
         this.Controls.Add(this.picPreview);
         this.Controls.Add(this.fraReference);
         this.Controls.Add(this.lblExpression);
         this.Controls.Add(this.lblName);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "frmShape";
         this.Text = "Define Shape";
         this.fraReference.ResumeLayout(false);
         this.ResumeLayout(false);

      }
      #endregion

      private void btnPreview_Click(object sender, System.EventArgs e)
      {
         Microsoft.CSharp.CSharpCodeProvider cscp = new Microsoft.CSharp.CSharpCodeProvider();
         System.CodeDom.Compiler.ICodeCompiler cc = cscp.CreateCompiler();
         System.CodeDom.Compiler.CompilerParameters cp = new System.CodeDom.Compiler.CompilerParameters();
         cp.ReferencedAssemblies.Add(System.IO.Path.Combine(Application.StartupPath,"SGDK2IDE.exe"));
         cp.OutputAssembly = System.IO.Path.Combine(Application.StartupPath, "TempEval.dll");
         cp.GenerateInMemory = false;
         cp.GenerateExecutable = false;
         int nWidth, nHeight;
         try
         {
            nWidth = int.Parse(txtWidth.Text);
            nHeight = int.Parse(txtHeight.Text);
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(this, "Invalid preview size specified: " + ex.Message, "Preview Shape");
            return;
         }
         string srcCode = "class TempEval : System.MarshalByRefObject, SGDK2.RemotingServices.IEvaluatePoint { " +
            "public ulong[] evalMatrix() { ulong[] result = new ulong[" + nHeight.ToString() +"]; " +
            "System.Array.Clear(result,0," + nHeight.ToString() + "); " +
            "for (int y=0; y<" + nHeight.ToString() + "; y++) { for (int x=0; x<" + nWidth.ToString() + "; x++) " +
            "if (evalExpr(x,y)) result[y] |= (ulong)1<<x; } return result;} " +
            "private bool evalExpr(int x, int y) " +
            "{ const int TileWidth = " + nWidth.ToString() + "; const int TileHeight = " + nHeight.ToString() + "; return " + txtExpression.Text + "; } }";
         System.CodeDom.Compiler.CompilerResults cr = cc.CompileAssemblyFromSource(cp, srcCode);
         System.Text.StringBuilder sb = new System.Text.StringBuilder();
         foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
            sb.Append(ce.ToString() + Environment.NewLine);
         if (cr.Errors.Count > 0)
         {
            MessageBox.Show(this, sb.ToString(), "Preview Shape Expression", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
         if (cr.NativeCompilerReturnValue == 0)
         {
            try
            {
               AppDomain evalDomain = AppDomain.CreateDomain("EvalDomain");

               RemotingServices.IEvaluatePoint hEvalObj = evalDomain.CreateInstanceAndUnwrap("TempEval", "TempEval", false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, new object[] {}, null, null, null) as RemotingServices.IEvaluatePoint;

               System.UInt64[] results = hEvalObj.evalMatrix();
               hEvalObj = null;
               AppDomain.Unload(evalDomain);
               System.IO.File.Delete(cp.OutputAssembly);

               Bitmap b = new Bitmap(97, 97, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
               Graphics g = Graphics.FromImage(b);
               g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
               int nPixelSize = 96 / Math.Max(nWidth, nHeight);
               for (int y = 0; y < nHeight; y++)
               {
                  for (int x = 0; x < nWidth; x++)
                     if (0 != (results[y] & ((System.UInt64)1<<x)))
                        g.FillRectangle(System.Drawing.Brushes.Black, x*nPixelSize,y*nPixelSize, nPixelSize + 1, nPixelSize + 1);
                     else
                     {
                        g.FillRectangle(System.Drawing.Brushes.Transparent, x*nPixelSize,y*nPixelSize, nPixelSize, nPixelSize);
                        g.DrawRectangle(System.Drawing.Pens.Gray, x*nPixelSize, y*nPixelSize, nPixelSize, nPixelSize);
                     }
               }
               g.Dispose();
               System.Drawing.Image i = picPreview.Image;
               picPreview.Image = b;
               if (i != null)
                  i.Dispose();

            }
            catch (System.Exception ex)
            {
               MessageBox.Show(this, ex.ToString(), "Preview Shape Expression", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
         }
      }

      private void txtName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         string sValid = ProjectData.ValidateName(txtName.Text);

         if (sValid != null)
         {
            if (DialogResult.Cancel == MessageBox.Show(this, sValid, "Shape Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtName.Text = m_Shape.Name;
            e.Cancel = true;
         }
         ProjectDataset.TileShapeRow tr = ProjectData.GetTileShape(txtName.Text);
         if ((null != tr) && (m_Shape != tr))
         {
            if (DialogResult.Cancel == MessageBox.Show(this, txtName.Text + " already exists", "Shape Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
               txtName.Text = m_Shape.Name;
            e.Cancel = true;
         }               
      }

      private void dataMonitor_TileShapeRowDeleted(object sender, SGDK2.ProjectDataset.TileShapeRowChangeEvent e)
      {
         if (e.Row == m_Shape)
            this.Close();      
      }

      private void dataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void txtName_Validated(object sender, System.EventArgs e)
      {
         m_Shape.Name = txtName.Text;
      }

      private void txtExpression_Validated(object sender, System.EventArgs e)
      {
         m_Shape.Shape = txtExpression.Text;
      }
   }
}
