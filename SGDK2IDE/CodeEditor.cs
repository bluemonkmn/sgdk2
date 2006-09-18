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
      #region Non-control members
      ProjectDataset.SourceCodeRow m_SourceCode;
      #endregion

      private SGDK2.DataChangeNotifier DataMonitor;
      private CodeEditor rtfCode;
      private System.ComponentModel.IContainer components;

      #region Win32 API
      private const int WM_USER = 0x0400;
      private const int EM_GETSCROLLPOS = WM_USER + 221;
      private System.Windows.Forms.Timer tmrReparse;
      private const int EM_SETSCROLLPOS = WM_USER + 222;
      private const int EM_SETTEXTEX = WM_USER + 97;
      private System.Windows.Forms.StatusBar staCode;
      private System.Windows.Forms.StatusBarPanel sbpLineNum;
      private System.Windows.Forms.StatusBarPanel sbpChar;
      private System.Windows.Forms.StatusBarPanel sbpCAPS;
      private System.Windows.Forms.StatusBarPanel sbpStatus;
      private System.Windows.Forms.StatusBarPanel sbpInsert;
      private const int ST_KEEPUNDO = 1;
      private struct POINT
      {
         public int x;
         public int y;
      }
      private struct SETTEXTEX
      {
         public uint flags;
         public uint codepage;
      }

      [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint="SendMessage")]
      static extern IntPtr SendScrollPosMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref POINT lParam);
      [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint="SendMessage")]
      static extern IntPtr SendSetTextMessage(IntPtr hWnd, uint Msg, ref SETTEXTEX wParam, string lParam);
      [System.Runtime.InteropServices.DllImport("user32.dll")]
      static extern short GetKeyState(Keys nVirtKey);
      #endregion

      #region Windows Form Designer Components

      #endregion

      #region Initialization and Clean-up
      public frmCodeEditor(ProjectDataset.SourceCodeRow drSourceCode)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         m_SourceCode = drSourceCode;
         rtfCode.Rtf = ConvertToRTF(m_SourceCode.Text);
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
         ((System.ComponentModel.ISupportInitialize)(this.sbpStatus)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpCAPS)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpInsert)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpLineNum)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.sbpChar)).BeginInit();
         this.SuspendLayout();
         // 
         // DataMonitor
         // 
         this.DataMonitor.Clearing += new System.EventHandler(this.DataMonitor_Clearing);
         // 
         // rtfCode
         // 
         this.rtfCode.AcceptsTab = true;
         this.rtfCode.Dock = System.Windows.Forms.DockStyle.Fill;
         this.rtfCode.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
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
         // frmCodeEditor
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(456, 349);
         this.Controls.Add(this.rtfCode);
         this.Controls.Add(this.staCode);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.KeyPreview = true;
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

      #region Event Handlers
      private void DataMonitor_Clearing(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void frmCodeEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         if (m_SourceCode.Text != rtfCode.Text)
            m_SourceCode.Text = rtfCode.Text;
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
            // Keep the undo buffer while updating the RTF content
            SETTEXTEX st = new SETTEXTEX();
            st.flags = ST_KEEPUNDO;
            st.codepage = (uint)System.Globalization.CultureInfo.CurrentUICulture.TextInfo.ANSICodePage;
            SendSetTextMessage(rtfCode.Handle, EM_SETTEXTEX, ref st, ConvertToRTF(rtfCode.Text));
            rtfCode.SelectionStart = selStart;
            rtfCode.SelectionLength = selLen;
            SendScrollPosMessage(rtfCode.Handle, EM_SETSCROLLPOS, IntPtr.Zero, ref scrollPos);
            m_SourceCode.Text = rtfCode.Text;
         }
         finally
         {
            rtfCode.Visible = true;
            tmrReparse.Stop();
         }         
      }
      private void rtfCode_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
      {
         staCode.Invalidate();
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

            UpdateStatus();
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
      #endregion

      #region Overrides
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
            return source;
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

         int currentColor = 0;

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
         rtf.Append("}");
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
         sbpChar.Text = "Char " + (rtfCode.GetCurrentLineCharIndex()+1).ToString();
         sbpLineNum.Text = "Line " + (rtfCode.GetLineFromCharIndex(rtfCode.SelectionStart)+1).ToString();
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

      private bool m_isUndo;
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
            m_isUndo = true;
         return base.ProcessCmdKey (ref msg, keyData);
      }

      protected override void OnTextChanged(EventArgs e)
      {
         if (m_isUndo)
         {
            m_isUndo = false;
            return;
         }
         base.OnTextChanged (e);
      }
   }
   #endregion
}
