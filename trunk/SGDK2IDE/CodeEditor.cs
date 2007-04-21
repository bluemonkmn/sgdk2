using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SGDK2
{
	/// <summary>
	/// Summary description for CodeEditor.
	/// </summary>
   public class frmCodeEditor : System.Windows.Forms.Form
   {
      #region Embedded Types
      private class UndoUnit
      {
         public POINT scrollPos = POINT.Empty;
         public int changeStart = 0;
         public int changeLen = 0; // Only for Inserts (undo=delete) or replace
         public string text = null; // Only for delete (undo=insert) or replace
         private UndoUnit next = null;
         public int GetSize()
         {
            if (text == null)
               return 24;
            else
               return 24 + text.Length * 2;
         }
         public static void Push(ref UndoUnit target, UndoUnit insert)
         {
            if (target != null)
               insert.next = target;
            else
               insert.next = null;
            target = insert;
         }
         public static UndoUnit Pop(ref UndoUnit target)
         {
            UndoUnit result = target;
            target = target.next;
            result.next = null;
            return result;
         }
         public static void TruncateAtSize(ref UndoUnit target, int maxSize)
         {
            int size = target.GetSize();
            if (size > maxSize)
            {
               target = null;
               return;
            }
            for (UndoUnit cur = target; cur.next != null; cur = cur.next)
            {
               size += cur.next.GetSize();
               if (size > maxSize)
               {
                  cur.next = null;
                  return;
               }
            }
         }
      }
      #endregion

      #region Non-control members
      ProjectDataset.SourceCodeRow m_SourceCode;
      frmFindReplace m_frmFindReplace = null;
      #endregion

      private SGDK2.DataChangeNotifier DataMonitor;
      private CodeEditor rtfCode;
      private System.ComponentModel.IContainer components;
      private UndoUnit undoStack;
      private UndoUnit redoStack;

      #region Win32 API
      private const int WM_USER = 0x0400;
      private const int EM_GETSCROLLPOS = WM_USER + 221;
      private System.Windows.Forms.Timer tmrReparse;
      private const int EM_SETSCROLLPOS = WM_USER + 222;
      private const int EM_SETUNDOLIMIT = WM_USER + 82;
      private System.Windows.Forms.StatusBar staCode;
      private System.Windows.Forms.StatusBarPanel sbpLineNum;
      private System.Windows.Forms.StatusBarPanel sbpChar;
      private System.Windows.Forms.StatusBarPanel sbpCAPS;
      private System.Windows.Forms.StatusBarPanel sbpStatus;
      private System.Windows.Forms.StatusBarPanel sbpInsert;
      private System.Windows.Forms.MenuItem mnuEdit;
      private System.Windows.Forms.MenuItem mnuEditFind;
      private System.Windows.Forms.MenuItem mnuEditReplace;
      private System.Windows.Forms.MenuItem mnuEditGoto;
      private System.Windows.Forms.MenuItem mnuFindNext;
      private System.Windows.Forms.MainMenu mnuCodeEditor;
      private System.Windows.Forms.Timer tmrInvalidateStatus;
      private System.Windows.Forms.MenuItem mnuEmbeddedData;
      private System.Windows.Forms.MenuItem mnuDataLoad;
      private System.Windows.Forms.MenuItem mnuDataEdit;
      private System.Windows.Forms.MenuItem mnuDataClear;
      private System.Windows.Forms.MenuItem mnuFile;
      private System.Windows.Forms.MenuItem mnuFileRename;
      private System.Windows.Forms.MenuItem mnuFileSeparator;
      private System.Windows.Forms.OpenFileDialog dlgEmbeddedFile;
      private System.Windows.Forms.MenuItem mnuEditUndo;
      private System.Windows.Forms.MenuItem mnuEditRedo;
      private System.Windows.Forms.MenuItem mnuEditSeparator;
      private System.Windows.Forms.MenuItem mnuDataPlay;
      private struct POINT
      {
         public int x;
         public int y;
         public static readonly POINT Empty;
         static POINT()
         {
            Empty = new POINT();
            Empty.x = 0;
            Empty.y = 0;
         }
      }

      [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint="SendMessage")]
      static extern IntPtr SendScrollPosMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref POINT lParam);
      [System.Runtime.InteropServices.DllImport("user32.dll")]
      static extern short GetKeyState(Keys nVirtKey);
      [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint="SendMessage")]
      static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
      #endregion

      #region Initialization and Clean-up
      public frmCodeEditor(string Name, string DependsOn)
      {
         // This call is required by the Windows Form Designer.
         InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         m_SourceCode = ProjectData.AddSourceCode(Name, CodeGenerator.GetCustomCodeTemplate(Name), DependsOn, true, null);
         rtfCode.Rtf = ConvertToRTF(m_SourceCode.Text);
         mnuFileSeparator.Visible = mnuFileRename.Visible = mnuEmbeddedData.Visible = true;
      }

      public frmCodeEditor(ProjectDataset.SourceCodeRow drSourceCode)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         SGDK2IDE.LoadFormSettings(this);

         m_SourceCode = drSourceCode;
         if (!m_SourceCode.IsTextNull())
            rtfCode.Rtf = ConvertToRTF(m_SourceCode.Text);

         if (m_SourceCode.IsCustomObject)
            mnuFileSeparator.Visible = mnuFileRename.Visible = mnuEmbeddedData.Visible = true;

         InitClearMenuItem();
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
            if (m_frmFindReplace != null)
               m_frmFindReplace.Dispose();
         }
         base.Dispose( disposing );
      }
      #endregion

      #region Windows Form Designer generated code
      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmCodeEditor));
         this.DataMonitor = new SGDK2.DataChangeNotifier(this.components);
         this.rtfCode = new SGDK2.CodeEditor();
         this.tmrReparse = new System.Windows.Forms.Timer(this.components);
         this.staCode = new System.Windows.Forms.StatusBar();
         this.sbpStatus = new System.Windows.Forms.StatusBarPanel();
         this.sbpCAPS = new System.Windows.Forms.StatusBarPanel();
         this.sbpInsert = new System.Windows.Forms.StatusBarPanel();
         this.sbpLineNum = new System.Windows.Forms.StatusBarPanel();
         this.sbpChar = new System.Windows.Forms.StatusBarPanel();
         this.mnuCodeEditor = new System.Windows.Forms.MainMenu();
         this.mnuFile = new System.Windows.Forms.MenuItem();
         this.mnuFileSeparator = new System.Windows.Forms.MenuItem();
         this.mnuFileRename = new System.Windows.Forms.MenuItem();
         this.mnuEdit = new System.Windows.Forms.MenuItem();
         this.mnuEditFind = new System.Windows.Forms.MenuItem();
         this.mnuFindNext = new System.Windows.Forms.MenuItem();
         this.mnuEditReplace = new System.Windows.Forms.MenuItem();
         this.mnuEditGoto = new System.Windows.Forms.MenuItem();
         this.mnuEmbeddedData = new System.Windows.Forms.MenuItem();
         this.mnuDataLoad = new System.Windows.Forms.MenuItem();
         this.mnuDataEdit = new System.Windows.Forms.MenuItem();
         this.mnuDataClear = new System.Windows.Forms.MenuItem();
         this.mnuDataPlay = new System.Windows.Forms.MenuItem();
         this.tmrInvalidateStatus = new System.Windows.Forms.Timer(this.components);
         this.dlgEmbeddedFile = new System.Windows.Forms.OpenFileDialog();
         this.mnuEditUndo = new System.Windows.Forms.MenuItem();
         this.mnuEditRedo = new System.Windows.Forms.MenuItem();
         this.mnuEditSeparator = new System.Windows.Forms.MenuItem();
         ((System.ComponentModel.ISupportInitialize)(this.sbpStatus)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpCAPS)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpInsert)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpLineNum)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpChar)).BeginInit();
         this.SuspendLayout();
         // 
         // DataMonitor
         // 
         this.DataMonitor.SourceCodeRowDeleted += new SGDK2.ProjectDataset.SourceCodeRowChangeEventHandler(this.DataMonitor_SourceCodeRowDeleted);
         this.DataMonitor.SourceCodeRowChanged += new SGDK2.ProjectDataset.SourceCodeRowChangeEventHandler(this.DataMonitor_SourceCodeRowChanged);
         this.DataMonitor.Clearing += new System.EventHandler(this.DataMonitor_Clearing);
         // 
         // rtfCode
         // 
         this.rtfCode.AcceptsTab = true;
         this.rtfCode.Dock = System.Windows.Forms.DockStyle.Fill;
         this.rtfCode.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
         this.rtfCode.HideSelection = false;
         this.rtfCode.Location = new System.Drawing.Point(0, 0);
         this.rtfCode.Name = "rtfCode";
         this.rtfCode.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
         this.rtfCode.ShowSelectionMargin = true;
         this.rtfCode.Size = new System.Drawing.Size(456, 327);
         this.rtfCode.TabIndex = 0;
         this.rtfCode.Text = "";
         this.rtfCode.WordWrap = false;
         this.rtfCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtfCode_KeyDown);
         this.rtfCode.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rtfCode_MouseUp);
         this.rtfCode.TextChanged += new System.EventHandler(this.rtfCode_TextChanged);
         // 
         // tmrReparse
         // 
         this.tmrReparse.Interval = 300;
         this.tmrReparse.Tick += new System.EventHandler(this.tmrReparse_Tick);
         // 
         // staCode
         // 
         this.staCode.Location = new System.Drawing.Point(0, 327);
         this.staCode.Name = "staCode";
         this.staCode.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
                                                                                   this.sbpStatus,
                                                                                   this.sbpCAPS,
                                                                                   this.sbpInsert,
                                                                                   this.sbpLineNum,
                                                                                   this.sbpChar});
         this.staCode.ShowPanels = true;
         this.staCode.Size = new System.Drawing.Size(456, 22);
         this.staCode.TabIndex = 1;
         this.staCode.DrawItem += new System.Windows.Forms.StatusBarDrawItemEventHandler(this.staCode_DrawItem);
         // 
         // sbpStatus
         // 
         this.sbpStatus.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
         this.sbpStatus.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
         this.sbpStatus.Width = 342;
         // 
         // sbpCAPS
         // 
         this.sbpCAPS.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
         this.sbpCAPS.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
         this.sbpCAPS.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
         this.sbpCAPS.Text = "CAPS";
         this.sbpCAPS.Width = 45;
         // 
         // sbpInsert
         // 
         this.sbpInsert.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
         this.sbpInsert.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
         this.sbpInsert.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
         this.sbpInsert.Text = "INS";
         this.sbpInsert.Width = 33;
         // 
         // sbpLineNum
         // 
         this.sbpLineNum.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
         this.sbpLineNum.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
         this.sbpLineNum.Width = 10;
         // 
         // sbpChar
         // 
         this.sbpChar.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
         this.sbpChar.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
         this.sbpChar.Width = 10;
         // 
         // mnuCodeEditor
         // 
         this.mnuCodeEditor.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.mnuFile,
                                                                                      this.mnuEdit,
                                                                                      this.mnuEmbeddedData});
         // 
         // mnuFile
         // 
         this.mnuFile.Index = 0;
         this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                this.mnuFileSeparator,
                                                                                this.mnuFileRename});
         this.mnuFile.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
         this.mnuFile.Text = "&File";
         // 
         // mnuFileSeparator
         // 
         this.mnuFileSeparator.Index = 0;
         this.mnuFileSeparator.MergeOrder = 8;
         this.mnuFileSeparator.Text = "-";
         this.mnuFileSeparator.Visible = false;
         // 
         // mnuFileRename
         // 
         this.mnuFileRename.Index = 1;
         this.mnuFileRename.MergeOrder = 8;
         this.mnuFileRename.Text = "Rename &Custom Code Object";
         this.mnuFileRename.Visible = false;
         this.mnuFileRename.Click += new System.EventHandler(this.mnuFileRename_Click);
         // 
         // mnuEdit
         // 
         this.mnuEdit.Index = 1;
         this.mnuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                this.mnuEditUndo,
                                                                                this.mnuEditRedo,
                                                                                this.mnuEditSeparator,
                                                                                this.mnuEditFind,
                                                                                this.mnuFindNext,
                                                                                this.mnuEditReplace,
                                                                                this.mnuEditGoto});
         this.mnuEdit.Text = "&Edit";
         // 
         // mnuEditFind
         // 
         this.mnuEditFind.Index = 3;
         this.mnuEditFind.Shortcut = System.Windows.Forms.Shortcut.CtrlF;
         this.mnuEditFind.Text = "&Find...";
         this.mnuEditFind.Click += new System.EventHandler(this.mnuEditFind_Click);
         // 
         // mnuFindNext
         // 
         this.mnuFindNext.Index = 4;
         this.mnuFindNext.Shortcut = System.Windows.Forms.Shortcut.F3;
         this.mnuFindNext.Text = "Find &Next";
         this.mnuFindNext.Click += new System.EventHandler(this.mnuFindNext_Click);
         // 
         // mnuEditReplace
         // 
         this.mnuEditReplace.Index = 5;
         this.mnuEditReplace.Shortcut = System.Windows.Forms.Shortcut.CtrlH;
         this.mnuEditReplace.Text = "&Replace...";
         this.mnuEditReplace.Click += new System.EventHandler(this.mnuEditReplace_Click);
         // 
         // mnuEditGoto
         // 
         this.mnuEditGoto.Index = 6;
         this.mnuEditGoto.Shortcut = System.Windows.Forms.Shortcut.CtrlG;
         this.mnuEditGoto.Text = "&Go To Line...";
         this.mnuEditGoto.Click += new System.EventHandler(this.mnuEditGoto_Click);
         // 
         // mnuEmbeddedData
         // 
         this.mnuEmbeddedData.Index = 2;
         this.mnuEmbeddedData.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                        this.mnuDataLoad,
                                                                                        this.mnuDataEdit,
                                                                                        this.mnuDataClear,
                                                                                        this.mnuDataPlay});
         this.mnuEmbeddedData.MergeOrder = 2;
         this.mnuEmbeddedData.Text = "Embedded &Data";
         this.mnuEmbeddedData.Visible = false;
         // 
         // mnuDataLoad
         // 
         this.mnuDataLoad.Index = 0;
         this.mnuDataLoad.Text = "&Load From File...";
         this.mnuDataLoad.Click += new System.EventHandler(this.mnuDataLoad_Click);
         // 
         // mnuDataEdit
         // 
         this.mnuDataEdit.Index = 1;
         this.mnuDataEdit.Text = "&Edit As Text...";
         this.mnuDataEdit.Click += new System.EventHandler(this.mnuDataEdit_Click);
         // 
         // mnuDataClear
         // 
         this.mnuDataClear.Enabled = false;
         this.mnuDataClear.Index = 2;
         this.mnuDataClear.Text = "&Clear";
         this.mnuDataClear.Click += new System.EventHandler(this.mnuDataClear_Click);
         // 
         // mnuDataPlay
         // 
         this.mnuDataPlay.Index = 3;
         this.mnuDataPlay.Text = "&Play with FMOD";
         this.mnuDataPlay.Click += new System.EventHandler(this.mnuDataPlay_Click);
         // 
         // tmrInvalidateStatus
         // 
         this.tmrInvalidateStatus.Tick += new System.EventHandler(this.tmrInvalidateStatus_Tick);
         // 
         // dlgEmbeddedFile
         // 
         this.dlgEmbeddedFile.AddExtension = false;
         this.dlgEmbeddedFile.Filter = "All Files (*.*)|*.*";
         this.dlgEmbeddedFile.Title = "Select File To Embed";
         // 
         // mnuEditUndo
         // 
         this.mnuEditUndo.Index = 0;
         this.mnuEditUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
         this.mnuEditUndo.Text = "&Undo";
         this.mnuEditUndo.Click += new System.EventHandler(this.rtfCode_OnUndo);
         // 
         // mnuEditRedo
         // 
         this.mnuEditRedo.Index = 1;
         this.mnuEditRedo.Shortcut = System.Windows.Forms.Shortcut.CtrlY;
         this.mnuEditRedo.Text = "Re&do";
         this.mnuEditRedo.Click += new System.EventHandler(this.rtfCode_OnRedo);
         // 
         // mnuEditSeparator
         // 
         this.mnuEditSeparator.Index = 2;
         this.mnuEditSeparator.Text = "-";
         // 
         // frmCodeEditor
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(456, 349);
         this.Controls.Add(this.rtfCode);
         this.Controls.Add(this.staCode);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.KeyPreview = true;
         this.Menu = this.mnuCodeEditor;
         this.Name = "frmCodeEditor";
         this.Text = "Source Code Editor";
         this.Closing += new System.ComponentModel.CancelEventHandler(this.frmCodeEditor_Closing);
         ((System.ComponentModel.ISupportInitialize)(this.sbpStatus)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpCAPS)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpInsert)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpLineNum)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpChar)).EndInit();
         this.ResumeLayout(false);

      }
      #endregion

      #region Overrides
      protected override void OnLoad(EventArgs e)
      {
         UpdateStatus();
         Text = "Source Code Editor - " + m_SourceCode.Name;
         SendMessage(rtfCode.Handle, EM_SETUNDOLIMIT, 0, 0);
         base.OnLoad (e);
      }

      protected override void WndProc(ref Message m)
      {
         switch(m.Msg)
         {
            case 0x14: // WM_ERASEBKGND
               m.Result = System.IntPtr.Zero;
               return;
         }
         base.WndProc (ref m);
      }

      protected override void OnClosing(CancelEventArgs e)
      {
         base.OnClosing (e);
         SGDK2IDE.SaveFormSettings(this);
      }
      #endregion

      #region Code Parser
      enum ParseState
      {
         Initial,
         White,
         String,
         EscapedString,
         OneSlash,
         DoubleSlash,
         CommentBlock,
         Asterisk,
         Token,
         Number,
         Plus,
         Minus,
         Zero,
         Hex,
         Period,
         NextDone,
         Char,
         EscapedChar,
         Done
      }

      enum TokenType
      {
         White,
         String,
         Comment,
         Token,
         Number,
         Other
      }

      enum LineState
      {
         Start,
         PreProcessor,
         Other
      }

      private static readonly char[] escape = new char[] {'{', '}', '\\', '\r', '\n'};

      void QueueReparse()
      {
         tmrReparse.Stop();
         tmrReparse.Start();
      }

      private string RtfEscape(string source, int start, int length)
      {
         if (source.IndexOfAny(escape) >= 0)
         {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int nextEscape = source.IndexOfAny(escape, start, length);
               (nextEscape >= 0) && (length > 0);
               nextEscape = source.IndexOfAny(escape, start, length))
            {
               if (nextEscape > start)
               {
                  sb.Append(source, start, nextEscape - start);
               }
               switch(source[nextEscape])
               {
                  case '{':
                  case '}':
                  case '\\':
                     sb.Append("\\" + source[nextEscape]);
                     break;
                  case '\r':
                     break;
                  case '\n':
                     sb.Append("\\par\r\n");
                     break;
               }
               length -= nextEscape + 1 - start;
               start = nextEscape + 1;
            }
            if (length > 0)
               sb.Append(source, start, length);
            return sb.ToString();
         }
         else
            return source.Substring(start, length);
      }

      private string ConvertToRTF(string code)
      {
         int curPos;
         int tokenLen;
         TokenType type;
         LineState lineState = LineState.Start;

         System.Text.StringBuilder rtf = new System.Text.StringBuilder(
            @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Courier New;}}" + "\r\n" +
            @"{\colortbl ;\red0\green0\blue255;\red0\green128\blue0;\red139\green0\blue139;\red128\green128\blue128;\red165\green42\blue42;}" + "\r\n" +
            @"\viewkind4\uc1\pard\tx315\tx630\tx945\tx1260\tx1575\tx1890\tx2205\tx2520\tx2835\tx3150\tx3465\cf1\f0\fs17 ");

         int currentColor = -1;

         for (curPos = 0; (tokenLen = ParseToken(code, curPos, out type)) > 0; curPos += tokenLen)
         {
            if (type == TokenType.White)
            {
               if (code.IndexOf("\n", curPos, tokenLen) >= 0)
                  lineState = LineState.Start;
            }

            if (lineState == LineState.PreProcessor)
            {
               if (currentColor != 4)
               {
                  rtf.Append(@"\cf4");
                  currentColor = 4;
               }
               rtf.Append(RtfEscape(code, curPos, tokenLen));
               continue;
            }

            switch(type)
            {
               case TokenType.Comment:
                  if (currentColor != 2)
                  {
                     rtf.Append(@"\cf2");
                     currentColor = 2;
                  }
                  rtf.Append(RtfEscape(code, curPos, tokenLen));
                  break;
               case TokenType.Number:
                  if (currentColor != 3)
                  {
                     rtf.Append(@"\cf3 ");
                     currentColor = 3;
                  }
                  rtf.Append(code, curPos, tokenLen);
                  break;
               case TokenType.String:
                  if (currentColor != 5)
                  {
                     rtf.Append(@"\cf5");
                     currentColor = 5;
                  }
                  rtf.Append(RtfEscape(code, curPos, tokenLen));
                  break;
               case TokenType.Token:
               switch(code.Substring(curPos, tokenLen))
               {
                  case "abstract":
                  case "as":
                  case "base":
                  case "bool":
                  case "break":
                  case "byte":
                  case "case":
                  case "catch":
                  case "char":
                  case "checked":
                  case "class":
                  case "const":
                  case "continue":
                  case "decimal":
                  case "default":
                  case "delegate":
                  case "do":
                  case "double":
                  case "else":
                  case "enum":
                  case "event":
                  case "explicit":
                  case "extern":
                  case "false":
                  case "finally":
                  case "fixed":
                  case "float":
                  case "for":
                  case "foreach":
                  case "goto":
                  case "get":
                  case "if":
                  case "implicit":
                  case "in":
                  case "int":
                  case "interface":
                  case "internal":
                  case "is":
                  case "lock":
                  case "long":
                  case "namespace":
                  case "new":
                  case "null":
                  case "object":
                  case "operator":
                  case "out":
                  case "override":
                  case "params":
                  case "private":
                  case "protected":
                  case "public":
                  case "readonly":
                  case "ref":
                  case "return":
                  case "sbyte":
                  case "sealed":
                  case "set":
                  case "short":
                  case "sizeof":
                  case "stackalloc":
                  case "static":
                  case "string":
                  case "struct":
                  case "switch":
                  case "this":
                  case "throw":
                  case "true":
                  case "try":
                  case "typeof":
                  case "uint":
                  case "ulong":
                  case "unchecked":
                  case "unsafe":
                  case "ushort":
                  case "using":
                  case "virtual":
                  case "volatile":
                  case "void":
                  case "while":
                     if (currentColor != 1)
                     {
                        rtf.Append(@"\cf1");
                        currentColor = 1;
                     }
                     rtf.Append(code, curPos, tokenLen);
                     break;
                  default:
                     if (currentColor != 0)
                     {
                        rtf.Append(@"\cf0");
                        currentColor = 0;
                     }
                     rtf.Append(code, curPos, tokenLen);
                     break;
               }
                  break;
               case TokenType.White:
                  rtf.Append(RtfEscape(code, curPos, tokenLen));
                  break;
               default:
                  if ((code[curPos] == '#') && (lineState == LineState.Start))
                  {
                     lineState = LineState.PreProcessor;
                     if (currentColor != 4)
                     {
                        rtf.Append(@"\cf4");
                        currentColor = 4;
                     }
                     rtf.Append(code, curPos, tokenLen);
                  }
                  else
                  {
                     if (currentColor != 0)
                     {
                        rtf.Append(@"\cf0");
                        currentColor = 0;
                     }
                     rtf.Append(RtfEscape(code, curPos, tokenLen));
                  }
                  break;
            }
            if ((type != TokenType.White) && (lineState != LineState.PreProcessor))
               lineState = LineState.Other;
         }
         rtf.Append("\\par}");
         return rtf.ToString();
      }

      /// <summary>
      /// Get the length of the next component of the source code string
      /// </summary>
      /// <param name="source">Source code string</param>
      /// <returns>Length of next token or whitespace block</returns>
      private int ParseToken(string source, int position, out TokenType type)
      {
         int nextPos;
         ParseState parseState = ParseState.Initial;

         type = TokenType.Other;

         for (nextPos = position;
            (nextPos < source.Length) && (parseState != ParseState.NextDone);
            nextPos++)
         {
            char curChar = source[nextPos];

            switch(parseState)
            {
               case ParseState.Initial:
               switch(curChar)
               {
                  case ' ':
                  case '\r':
                  case '\n':
                  case '\t':
                     parseState = ParseState.White;
                     type = TokenType.White;
                     break;
                  case '/':
                     parseState = ParseState.OneSlash;
                     break;
                  case '\"':
                     parseState = ParseState.String;
                     type = TokenType.String;
                     break;
                  case '\'':
                     parseState = ParseState.Char;
                     type = TokenType.String;
                     break;
                  case '+':
                     parseState = ParseState.Plus;
                     break;
                  case '-':
                     parseState = ParseState.Minus;
                     break;
                  case '.':
                     parseState = ParseState.Period;
                     break;
                  case '0':
                     parseState = ParseState.Zero;
                     type = TokenType.Number;
                     break;
                  default:
                     if (char.IsLetter(curChar))
                     {
                        parseState = ParseState.Token;
                        type = TokenType.Token;
                     }
                     else if (char.IsDigit(curChar))
                     {
                        parseState = ParseState.Number;
                        type = TokenType.Number;
                     }
                     else parseState = ParseState.NextDone;
                     break;
               }
                  break;
               case ParseState.White:
                  if (!char.IsWhiteSpace(curChar))
                     parseState = ParseState.Done;
                  break;
               case ParseState.OneSlash:
               switch(curChar)
               {
                  case '/':
                     parseState = ParseState.DoubleSlash;
                     type = TokenType.Comment;
                     break;
                  case '*':
                     parseState = ParseState.CommentBlock;
                     type = TokenType.Comment;
                     break;
                  default:
                     parseState = ParseState.Done;
                     break;
               }
                  break;
               case ParseState.DoubleSlash:
               switch(curChar)
               {
                  case '\n':
                     parseState = ParseState.NextDone;
                     break;
               }
                  break;
               case ParseState.CommentBlock:
               switch(curChar)
               {
                  case '*':
                     parseState = ParseState.Asterisk;
                     break;
               }
                  break;
               case ParseState.Asterisk:
               switch(curChar)
               {
                  case '/':
                     parseState = ParseState.NextDone;
                     break;
                  default:
                     parseState = ParseState.CommentBlock;
                     break;
               }
                  break;
               case ParseState.String:
               switch(curChar)
               {
                  case '\"':
                     parseState = ParseState.NextDone;
                     break;
                  case '\\':
                     parseState = ParseState.EscapedString;
                     break;
               }
                  break;
               case ParseState.Char:
               switch(curChar)
               {
                  case '\'':
                     parseState = ParseState.NextDone;
                     break;
                  case '\\':
                     parseState = ParseState.EscapedChar;
                     break;
               }
                  break;
               case ParseState.EscapedString:
                  parseState = ParseState.String;
                  break;
               case ParseState.EscapedChar:
                  parseState = ParseState.Char;
                  break;
               case ParseState.Minus:
               switch(curChar)
               {
                  case '-':
                  case '=':
                     parseState = ParseState.NextDone;
                     break;
                  default:
                     parseState = ParseState.Done;
                     break;
               }
                  break;
               case ParseState.Plus:
               switch(curChar)
               {
                  case '+':
                  case '=':
                     parseState = ParseState.NextDone;
                     break;
                  default:
                     parseState = ParseState.Done;
                     break;
               }
                  break;
               case ParseState.Token:
                  if(!char.IsLetterOrDigit(curChar) && (curChar != '_'))
                     parseState = ParseState.Done;
                  break;
               case ParseState.Number:
                  if (!char.IsDigit(curChar) && (curChar != '.'))
                     parseState = ParseState.Done;
                  break;
               case ParseState.Period:
                  if (char.IsDigit(curChar))
                  {
                     parseState = ParseState.Number;
                     type = TokenType.Number;
                  }
                  else
                     parseState = ParseState.Done;
                  break;                  
               case ParseState.Zero:
               switch(curChar)
               {
                  case 'x':
                  case 'X':
                     parseState = ParseState.Hex;
                     break;
                  default:
                     if (char.IsDigit(curChar) || (curChar == '.'))
                        parseState = ParseState.Number;
                     else
                        parseState = ParseState.Done;
                     break;
               }
                  break;
               case ParseState.Hex:
                  if (!("0123456789abcdefABCDEF".IndexOf(curChar) >= 0))
                     parseState = ParseState.Done;
                  break;
            }

            if (parseState == ParseState.Done)
               break;
         }

         return nextPos - position;
      }
      #endregion

      #region Private Methods
      private void UpdateStatus()
      {
         tmrInvalidateStatus.Stop();
         tmrInvalidateStatus.Start();
      }

      private void InitClearMenuItem()
      {
         if (m_SourceCode.IsCustomObjectDataNull())
         {
            mnuDataClear.Text = "&Clear (No Data)";
            mnuDataClear.Enabled = false;
         }
         else
         {
            mnuDataClear.Text = "&Clear (" + ProjectData.GetCustomObjectDataSize(m_SourceCode) + ")";
            mnuDataClear.Enabled = true;
         }
      }
      #endregion

      #region Public Static Members
      public static void Edit(Form MdiParent, ProjectDataset.SourceCodeRow EditRow)
      {
         foreach(Form frm in MdiParent.MdiChildren)
         {
            frmCodeEditor f = frm as frmCodeEditor;
            if (f != null)
            {
               if (f.m_SourceCode == EditRow)
               {
                  f.Activate();
                  return;
               }
            }
         }

         frmCodeEditor frmNew = new frmCodeEditor(EditRow);
         frmNew.MdiParent = MdiParent;
         frmNew.Show();
      }
      #endregion

      #region Public Methods
      public void DoFindNext()
      {
         if (m_frmFindReplace == null)
         {
            m_frmFindReplace = new frmFindReplace(this, false);
            m_frmFindReplace.Show();
            return;
         }

         int start, end;
         RichTextBoxFinds options = m_frmFindReplace.Options;
         if (0 != (options & RichTextBoxFinds.Reverse))
         {
            start = 0;
            if (rtfCode.SelectionStart > 0)
               end = rtfCode.SelectionStart - 1;
            else
               end = rtfCode.TextLength;
         }
         else
         {
            if (rtfCode.SelectionStart < rtfCode.TextLength)
               start = rtfCode.SelectionStart + 1;
            else
               start = 0;
            end = rtfCode.TextLength - 1;
         }
         int result = rtfCode.Find(m_frmFindReplace.FindString, start, end, m_frmFindReplace.Options);
         if (result < 0)
            result = rtfCode.Find(m_frmFindReplace.FindString, m_frmFindReplace.Options);
         if (result < 0)
            MessageBox.Show(this, "The specified text was found anywhere in " + m_SourceCode.Name, "Find Text", MessageBoxButtons.OK, MessageBoxIcon.Information);

         UpdateStatus();
      }

      public void DoReplace()
      {
         if (m_frmFindReplace == null)
         {
            m_frmFindReplace = new frmFindReplace(this, true);
            m_frmFindReplace.Show();
            return;
         }

         if (rtfCode.SelectedText != m_frmFindReplace.FindString)
            DoFindNext();
         else if (rtfCode.SelectedText == m_frmFindReplace.FindString)
         {
            rtfCode.SelectedText = m_frmFindReplace.ReplaceString;
            DoFindNext();
         }
      }

      public void DoReplaceAll()
      {
         if (m_frmFindReplace == null)
         {
            m_frmFindReplace = new frmFindReplace(this, true);
            m_frmFindReplace.Show();
            return;
         }

         int start;
         RichTextBoxFinds options = m_frmFindReplace.Options & ~RichTextBoxFinds.Reverse;
         int result = rtfCode.Find(m_frmFindReplace.FindString, m_frmFindReplace.Options);
         int replacements = 0;
         while (result >= 0)
         {
            start = rtfCode.SelectionStart + m_frmFindReplace.ReplaceString.Length;
            rtfCode.SelectedText = m_frmFindReplace.ReplaceString;
            replacements++;
            result = rtfCode.Find(m_frmFindReplace.FindString, start, m_frmFindReplace.Options);
         }
         if (replacements == 0)
            MessageBox.Show(this, "The specified text was found anywhere in " + m_SourceCode.Name, "Find Text", MessageBoxButtons.OK, MessageBoxIcon.Information);
         else
            MessageBox.Show(this, "Replaced " + replacements.ToString() + " occurrences.", "Replace All", MessageBoxButtons.OK, MessageBoxIcon.Information);

         UpdateStatus();
      }
      #endregion

      #region Event Handlers
      private void DataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void frmCodeEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         if ((m_SourceCode.RowState != System.Data.DataRowState.Detached) && (m_SourceCode.RowState != System.Data.DataRowState.Deleted))
         {
            string rtfCodeText = rtfCode.Text.Replace("\r", String.Empty);
            if ((m_SourceCode.IsTextNull() && (rtfCodeText.Length > 0)) ||
                (!m_SourceCode.IsTextNull()) && (m_SourceCode.Text.Replace("\r", String.Empty) != rtfCodeText))
            {
               m_SourceCode.Text = rtfCodeText.Replace("\n","\r\n");
               CodeGenerator.ResetTempAssembly();
            }
         }
      }
      
      private void rtfCode_TextChanged(object sender, System.EventArgs e)
      {
         QueueReparse();
      }
      
      private void tmrReparse_Tick(object sender, System.EventArgs e)
      {
         tmrReparse.Stop();
         try
         {
            POINT scrollPos = new POINT();
            SendScrollPosMessage(rtfCode.Handle, EM_GETSCROLLPOS, IntPtr.Zero, ref scrollPos);
            rtfCode.Visible = false;
            int selStart = rtfCode.SelectionStart;
            int selLen = rtfCode.SelectionLength;
            rtfCode.Rtf = ConvertToRTF(rtfCode.Text);
            rtfCode.SelectionStart = selStart;
            rtfCode.SelectionLength = selLen;
            SendScrollPosMessage(rtfCode.Handle, EM_SETSCROLLPOS, IntPtr.Zero, ref scrollPos);
            string rtfCodeText;
            if (rtfCode.Text.IndexOf('\r') >= 0)
               rtfCodeText = rtfCode.Text.Replace("\r", String.Empty);
            else
               rtfCodeText = rtfCode.Text;
            string rowText = null;
            if (!m_SourceCode.IsTextNull())
               rowText = m_SourceCode.Text.Replace("\r", String.Empty);
            if ((m_SourceCode.IsTextNull() && (rtfCodeText.Length > 0)) ||
               (!m_SourceCode.IsTextNull()) && (rowText != rtfCodeText))
            {
               UndoUnit undo = new UndoUnit();
               int startDiff, endDiffRow, endDiffRtf;
               int endPos = (rowText.Length < rtfCodeText.Length ? rowText.Length : rtfCodeText.Length);
               for (startDiff=-1; ++startDiff < endPos;)
               {
                  if (rowText[startDiff] != rtfCodeText[startDiff])
                     break;
               }
               for (endDiffRow=rowText.Length-1, endDiffRtf=rtfCodeText.Length-1;
                    (endDiffRow >= startDiff) && (endDiffRtf >= startDiff); endDiffRow--, endDiffRtf--)
               {
                  if (rowText[endDiffRow] != rtfCodeText[endDiffRtf])
                  {
                     endDiffRow++;
                     endDiffRtf++;
                     break;
                  }
               }
               if ((endDiffRow < startDiff) || (endDiffRtf < startDiff))
               {
                  endDiffRow++;
                  endDiffRtf++;
               }
               undo.scrollPos = scrollPos;
               undo.changeStart = startDiff;
               if (endDiffRtf > startDiff)
               {
                  // Text was inserted or replaced, remember to delete for undo
                  undo.changeLen = endDiffRtf - startDiff;
               }
               if (endDiffRow > startDiff)
               {
                  // Text was deleted or replaced, remember to insert for undo
                  undo.text = rowText.Substring(startDiff, endDiffRow-startDiff);
               }
               UndoUnit.Push(ref undoStack, undo);
               UndoUnit.TruncateAtSize(ref undoStack, 1000000);
               redoStack = null;
               m_SourceCode.Text = rtfCodeText.Replace("\n","\r\n");
               CodeGenerator.ResetTempAssembly();
            }
         }
         finally
         {
            rtfCode.Visible = true;
            tmrReparse.Stop();
         }         
      }

      private void rtfCode_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
      {
         UpdateStatus();
      }
      private void rtfCode_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         UpdateStatus();
      }
      private void staCode_DrawItem(object sender, System.Windows.Forms.StatusBarDrawItemEventArgs sbdevent)
      {
         if (sbdevent.Panel == sbpCAPS)
         {
            Brush b = SystemBrushes.ControlText;
            if (0 == (GetKeyState(Keys.CapsLock) & 1))
               b = SystemBrushes.ControlDark;
            StringFormat sf = new StringFormat(StringFormatFlags.NoWrap);
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            sbdevent.Graphics.DrawString(sbdevent.Panel.Text, ((StatusBar)sender).Font, b, sbdevent.Bounds,sf);
         } 
         else if (sbdevent.Panel == sbpInsert)
         {
            Brush b = SystemBrushes.ControlDark;
            if (rtfCode.InsertMode)
               b = SystemBrushes.ControlText;
            StringFormat sf = new StringFormat(StringFormatFlags.NoWrap);
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            sbdevent.Graphics.DrawString(sbdevent.Panel.Text, ((StatusBar)sender).Font, b, sbdevent.Bounds,sf);
         }
      }
      private void mnuFindNext_Click(object sender, System.EventArgs e)
      {
         DoFindNext();
      }

      private void tmrInvalidateStatus_Tick(object sender, System.EventArgs e)
      {
         tmrInvalidateStatus.Stop();
         sbpChar.Text = "Char " + (rtfCode.GetCurrentLineCharIndex()+1).ToString();
         sbpLineNum.Text = "Line " + (rtfCode.GetLineFromCharIndex(rtfCode.SelectionStart)+1).ToString();
         staCode.Invalidate();
      }
      private void mnuEditFind_Click(object sender, System.EventArgs e)
      {
         if (null == m_frmFindReplace)
            m_frmFindReplace = new frmFindReplace(this, false);
         else
            m_frmFindReplace.SetMode(false);

         m_frmFindReplace.Show();
         m_frmFindReplace.InitFocus();
      }

      private void mnuEditReplace_Click(object sender, System.EventArgs e)
      {
         if (null == m_frmFindReplace)
            m_frmFindReplace = new frmFindReplace(this, true);
         else
            m_frmFindReplace.SetMode(true);

         m_frmFindReplace.Show();
         m_frmFindReplace.InitFocus();      
      }

      private void mnuEditGoto_Click(object sender, System.EventArgs e)
      {
         string line = frmInputBox.GetInput(this, "Go To Line", "Enter line number", String.Empty);
         if (line != null)
         {
            double lineNum;
            if (!double.TryParse(line, System.Globalization.NumberStyles.Integer, System.Threading.Thread.CurrentThread.CurrentCulture, out lineNum) || lineNum < 0)
               MessageBox.Show(this, "Invalid line number", "Go To Line", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            rtfCode.Select(rtfCode.GetLineStartCharIndex((int)lineNum-1), 0);
            UpdateStatus();
         }

      }

      private void mnuFileRename_Click(object sender, System.EventArgs e)
      {
         string sName = frmInputBox.GetInput(this, "Rename Custom Code Object", "Specify a new name", m_SourceCode.Name);
         if ((sName == null) || (sName == m_SourceCode.Name))
            return;

         string sBareName;
         if (sName.EndsWith(".cs"))
            sBareName = sName.Substring(0, sName.Length - 3);
         else if (sName.EndsWith(".dll"))
            sBareName = sName.Substring(0, sName.Length - 4);
         else
         {
            sBareName = sName;
            sName += ".cs";
         }

         string msg = ProjectData.ValidateName(sBareName);
         if (msg != null)
         {
            MessageBox.Show(this, "Invalid name \"" + sBareName + "\": " + msg, "Rename Custom Code Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }

         if (ProjectData.GetSourceCode(sName) != null)
         {
            MessageBox.Show(this, "The specified custom object name already exists", "Rename Custom Code Object", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }

         m_SourceCode.Name = sName;
      }

      private void mnuDataClear_Click(object sender, System.EventArgs e)
      {
         if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete the data associated with this custom code object?", "Clear Custom Object Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
         {
            m_SourceCode.SetCustomObjectDataNull();
            InitClearMenuItem();
         }
      }

      private void mnuDataLoad_Click(object sender, System.EventArgs e)
      {
         if (!m_SourceCode.IsCustomObjectDataNull())
         {
            if (DialogResult.OK != MessageBox.Show(this, "Loading new data will replace existing data associated with this code object.", "Load Custom Object Data", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
               return;
         }
         if (DialogResult.OK == dlgEmbeddedFile.ShowDialog(this))
         {
            try
            {
               System.IO.BinaryReader br = new System.IO.BinaryReader(dlgEmbeddedFile.OpenFile());
               try
               {
                  m_SourceCode.CustomObjectData = br.ReadBytes((int)br.BaseStream.Length);
               }
               finally
               {
                  br.Close();
               }
               InitClearMenuItem();
            }
            catch(System.Exception ex)
            {
               MessageBox.Show(this, ex.Message, "Load Custom Object Data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
         }
      }

      private void mnuDataEdit_Click(object sender, System.EventArgs e)
      {
         if (!m_SourceCode.IsCustomObjectDataNull())
         {
            string editString = System.Text.Encoding.UTF8.GetString(m_SourceCode.CustomObjectData);
            byte[] identity = System.Text.Encoding.UTF8.GetBytes(editString);
            bool identical = true;
            if (identity.Length != m_SourceCode.CustomObjectData.Length)
               identical = false;
            else
            {
               for (int i=0; i<identity.Length; i++)
                  if (m_SourceCode.CustomObjectData[i] != identity[i])
                  {
                     identical = false;
                     System.Diagnostics.Debug.WriteLine(editString.Substring(i,100));
                     break;
                  }
            }
            if (!identical)
            {
               if (DialogResult.OK != MessageBox.Show(this, "The data embedded in this object appears to be binary and *WILL* be irrevocably corrupted if OK is pressed after viewing in the text editor.", "Edit Data As Text", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation))
                  return;
            }
            if (null != (editString = frmCustomObjectData.EditText(this, editString)))
               m_SourceCode.CustomObjectData = System.Text.Encoding.UTF8.GetBytes(editString);
         }
         else
         {
            string result = frmCustomObjectData.EditText(this, String.Empty);
            if (result != null)
               m_SourceCode.CustomObjectData = System.Text.Encoding.UTF8.GetBytes(result);
         }
         InitClearMenuItem();
      }

      private void DataMonitor_SourceCodeRowDeleted(object sender, SGDK2.ProjectDataset.SourceCodeRowChangeEvent e)
      {
         if (m_SourceCode == e.Row)
            this.Close();
      }

      private void DataMonitor_SourceCodeRowChanged(object sender, SGDK2.ProjectDataset.SourceCodeRowChangeEvent e)
      {
         if (m_SourceCode == e.Row)
         {
            Text = "Source Code Editor - " + e.Row.Name;
         }
      }

      private void mnuDataPlay_Click(object sender, System.EventArgs e)
      {
         if (m_SourceCode.IsCustomObjectDataNull())
         {
            MessageBox.Show(this, "You must load data into this object before attempting to play it.", "Play With FMOD", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
         }
         frmSoundPlayer.PlaySound(this, m_SourceCode.CustomObjectData);
      }

      private void rtfCode_OnUndo(object sender, EventArgs e)
      {
         if (undoStack == null)
            return;

         UndoUnit undo = UndoUnit.Pop(ref undoStack);
         UndoUnit redo = new UndoUnit();

         redo.scrollPos = undo.scrollPos;
         redo.changeStart = undo.changeStart;
         if (undo.text == null)
            redo.changeLen = 0;
         else
            redo.changeLen = undo.text.Length;
         if (undo.changeLen > 0)
            redo.text = rtfCode.Text.Substring(undo.changeStart, undo.changeLen);

         string left;
         string right;
         left = rtfCode.Text.Substring(0, undo.changeStart);
         if (left.IndexOf('\r') >= 0)
            left = left.Replace("\r", String.Empty);
         right = rtfCode.Text.Substring(undo.changeStart + undo.changeLen);
         if (right.IndexOf('\r') >= 0)
            right = right.Replace("\r", String.Empty);
         if ((undo.text != null) && (undo.text.IndexOf('\r') >= 0))
            undo.text = undo.text.Replace("\r", String.Empty);
         rtfCode.Visible = false;
         if (undo.text != null)
         {
            m_SourceCode.Text = (left + undo.text + right).Replace("\n", "\r\n");
            rtfCode.Rtf = ConvertToRTF(left + undo.text + right);
         }
         else
         {
            m_SourceCode.Text = (left + right).Replace("\n", "\r\n");
            rtfCode.Rtf = ConvertToRTF(left + right);
         }
         rtfCode.SelectionStart = undo.changeStart;
         if (undo.text != null)
            rtfCode.SelectionLength = undo.text.Length;
         else
            rtfCode.SelectionLength = 0;
         SendScrollPosMessage(rtfCode.Handle, EM_SETSCROLLPOS, IntPtr.Zero, ref undo.scrollPos);
         rtfCode.Visible = true;

         UndoUnit.Push(ref redoStack, redo);
      }

      private void rtfCode_OnRedo(object sender, EventArgs e)
      {
         if (redoStack == null)
            return;

         UndoUnit redo = UndoUnit.Pop(ref redoStack);
         UndoUnit undo = new UndoUnit();

         undo.scrollPos = redo.scrollPos;
         undo.changeStart = redo.changeStart;
         if (redo.text == null)
            undo.changeLen = 0;
         else
            undo.changeLen = redo.text.Length;
         if (redo.changeLen > 0)
            undo.text = rtfCode.Text.Substring(redo.changeStart, redo.changeLen);

         string left;
         string right;
         left = rtfCode.Text.Substring(0, redo.changeStart);
         if (left.IndexOf('\r') >= 0)
            left = left.Replace("\r", String.Empty);
         right = rtfCode.Text.Substring(redo.changeStart + redo.changeLen);
         if (right.IndexOf('\r') >= 0)
            right = right.Replace("\r", String.Empty);
         if ((redo.text != null) && (redo.text.IndexOf('\r') >= 0))
            redo.text = redo.text.Replace("\r", String.Empty);
         rtfCode.Visible = false;
         if (redo.text != null)
         {
            m_SourceCode.Text = (left + redo.text + right).Replace("\n", "\r\n");
            rtfCode.Rtf = ConvertToRTF(left + redo.text + right);
         }
         else
         {
            m_SourceCode.Text = (left + right).Replace("\n", "\r\n");
            rtfCode.Rtf = ConvertToRTF(left + right);
         }
         rtfCode.SelectionStart = redo.changeStart;
         if (redo.text != null)
            rtfCode.SelectionLength = redo.text.Length;
         else
            rtfCode.SelectionLength = 0;
         SendScrollPosMessage(rtfCode.Handle, EM_SETSCROLLPOS, IntPtr.Zero, ref redo.scrollPos);
         rtfCode.Visible = true;

         UndoUnit.Push(ref undoStack, undo);
      }
      #endregion
   }

   #region CodeEditor Control
   public class CodeEditor : RichTextBox
   {
      private const int WM_KEYDOWN = 0x100;
      private const int WM_CHAR = 0x102;
      [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint="SendMessage")]
      static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

      public event System.EventHandler OnUndo;
      public event System.EventHandler OnRedo;

      private bool m_isInsert = true;

      /// <summary>
      /// Retrieves the position of the beginning of the selection within the current line
      /// </summary>
      /// <returns>0-based integer offset in characters from the beginning of the line</returns>
      public int GetCurrentLineCharIndex()
      {
         int i;
         for(i=this.SelectionStart-1; (i>0) && (Text[i] != '\n'); i--)
            ;
         if (i<=0)
            return this.SelectionStart;
         return this.SelectionStart - i - 1;
      }

      public int GetLineStartCharIndex(int line)
      {
         int index = this.TextLength / 2;
         int step = (int)Math.Ceiling(index / 2d);
         while(step > 0)
         {
            int charLine = GetLineFromCharIndex(index);
            if (charLine > line)
               index -= step;
            else if (charLine < line)
               index += step;
            else
            {
               if (index <= 0)
                  return index;
               if (GetLineFromCharIndex(index-1) < line)
                  return index;
               index -= step;
            }
            if (index < 0)
               return 0;
            else if (index >= this.TextLength)
               return this.TextLength - 1;
            step = (int)Math.Ceiling(step / 2d);
         }
         return index;
      }

      protected override bool ProcessKeyMessage(ref Message m)
      {
         if ((m.Msg==WM_CHAR) && ((int)m.WParam == 9))
         {
            m.WParam = new IntPtr(32);
            m.LParam = new IntPtr(0x390001);
            int sendMore = 2 - (GetCurrentLineCharIndex() % 3);
            for (int i=0; i<sendMore; i++)
               SendMessage(m.HWnd, m.Msg, m.WParam, m.LParam);
            return false;
         }
         else if ((m.Msg==WM_KEYDOWN) && (m.WParam.ToInt32() == (int)Keys.Insert))
         {
            m_isInsert = !m_isInsert;
         }
         return base.ProcessKeyMessage (ref m);
      }

      public bool InsertMode
      {
         get
         {
            return m_isInsert;
         }
      }

      protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
      {
         if ((msg.Msg == WM_KEYDOWN) && (keyData == (Keys.Control | Keys.Z)))
            if (OnUndo != null)
               OnUndo(this, null);
         if ((msg.Msg == WM_KEYDOWN) && (keyData == (Keys.Control | Keys.Y)))
            if (OnRedo != null)
               OnRedo(this, null);
         return base.ProcessCmdKey (ref msg, keyData);
      }
   }
   #endregion
}
