using System.Windows.Forms;
using ZiZhuJY.ResX_Aggregator.Core;
using zizhujycom.ResX_Aggregator;

namespace ZiZhuJY.ResX_Aggregator
{
    public partial class GridEditor : UserControl
    {
        private string m_TextToRecord;
        private readonly VSMacroRecorder m_Recorder;
        private ResXAggregator m_resxAgg;

        public GridEditor()
        {
            InitializeComponent();

            m_Recorder = new VSMacroRecorder(GuidList.guidResX_AggregatorEditorFactory);
        }

        private DataGridView _dataGridControl;

        public DataGridView DataGridControl
        {
            get
            {
                return this._dataGridControl;
            }

            private set
            {
                this._dataGridControl = value;
            }
        }

        #region Macro Recording methods

        public void RecordDelete(bool backspace, bool word)
        {
            // If not backspace then it's a delete
            // If not word then it's a single character
            var macroType = backspace
                ? (word ? LastMacro.BackspaceWord : LastMacro.BackspaceChar)
                : (word ? LastMacro.DeleteWord : LastMacro.DeleteChar);

            // Get the number of times the macro type calculated above has been recorded already 
            // (if any) and then add one to get the current count
            var count = m_Recorder.GetTimesPreviouslyRecorded(macroType) + 1;

            var macroString = string.Empty;
            // If this parameter is negative, it indicates a backspace, rather than a delete
            macroString += "ActiveDoument.Object.Delete(" +
                           (int) (word ? tom.tomConstants.tomWord : tom.tomConstants.tomCharFormat) + ", " +
                           (backspace ? -1*count : count) + ")";

            m_Recorder.RecordBatchedLine(macroType, macroString);
        }

        public void RecordMove(LastMacro state, string direction, MoveScope scope, bool extend)
        {
            var macroString = string.Empty;

            macroString += "ActiveDocument.Object.Move";
            macroString += direction;

            // Get the number of times this macro type has been recorded already
            // (if any) and then add one to get the current count
            macroString += "(" + (int) scope + ", " + (m_Recorder.GetTimesPreviouslyRecorded(state) + 1) + ", " +
                           (int) (extend ? tom.tomConstants.tomExtend : tom.tomConstants.tomMove) + ")";

            m_Recorder.RecordBatchedLine(state, macroString);
        }

        public void RecordCommand(string command)
        {
            if (!m_Recorder.IsRecording()) return;

            var line = "ActiveDocument.Object.";

            line += command;

            m_Recorder.RecordLine(line);
        }

        public void StopRecorder()
        {
            m_Recorder.Stop();
        }

        public void RecordPrintableChar(char currentValue)
        {
            var macroString = "";

            if (!m_Recorder.IsLastRecordedMacro(LastMacro.Text))
            {
                m_TextToRecord = string.Empty;
            }

            // Only deal with text characters. Everything, space and above is a text character
            // except DEL (0x7f). Include carriage return (enter key) and tab, which are below space, since those are also text characters.
            if (char.IsLetterOrDigit(currentValue) || char.IsPunctuation(currentValue) || char.IsSeparator(currentValue) ||
                char.IsSymbol(currentValue) || char.IsWhiteSpace(currentValue) || '\r' == currentValue ||
                '\t' == currentValue)
            {
                if ('\r' == currentValue)
                {
                    // Emit "\r\n" as the standard line terminator
                    m_TextToRecord += "\" & vbCr & \"";
                }else if ('\t' == currentValue)
                {
                    // Emit "\t" as the standard tab 
                    m_TextToRecord += "\" & vbTab & \"";
                }
                else
                {
                    m_TextToRecord += currentValue;
                }

                macroString += "ActiveDocument.Object.TypeText(\"";
                macroString += m_TextToRecord;
                macroString += "\")";

                if (m_Recorder.RecordBatchedLine(LastMacro.Text, macroString, 100)) // arbitrary max length
                {
                    // Clear out the buffer if the line hit max length, since
                    // it will not continue to be appended to 
                    m_TextToRecord = string.Empty;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")
        ]
        public void RecordNonprintableChar(Keys currentKey)
        {
            var macroString = string.Empty;


            // Obtain the CTRL and SHIFT as they modify a number of the virtual keys. 
            bool shiftDown = System.Windows.Forms.Keys.Shift ==
                             (System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Shift);
                //Keyboard::IsKeyDown(VK_SHIFT);
            bool controlDown = System.Windows.Forms.Keys.Control ==
                               (System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Control);
                //Keyboard::IsKeyDown(VK_CONTROL);

            // msg.WParam indicates the virtual key.
            switch (currentKey)
            {
                case Keys.Back: // BackSpace key
                    // Note that SHIFT does not affect this command
                    RecordDelete(true, controlDown);
                    break;

                case Keys.Delete:
                    // Note that SHIFT completely disables this command
                    if (!shiftDown)
                    {
                        RecordDelete(false, controlDown);
                    }
                    break;

                case Keys.Left: // Left Arrow
                    // SHIFT indicates selection, CTRL indicates words instead of characters
                {
                    LastMacro macroType = controlDown
                        ? (shiftDown ? LastMacro.LeftArrowWordSelection : LastMacro.LeftArrowWord)
                        : (shiftDown ? LastMacro.LeftArrowCharSelection : LastMacro.LeftArrowChar);

                    RecordMove(macroType, "Left", controlDown ? MoveScope.Word : MoveScope.Character, shiftDown);
                }
                    break;

                case Keys.Right: // Right Arrow
                    // SHIFT indicates selection, CTRL indicates words instead of characters
                {
                    LastMacro macroType = controlDown
                        ? (shiftDown ? LastMacro.RightArrowWordSelection : LastMacro.RightArrowWord)
                        : (shiftDown ? LastMacro.RightArrowCharSelection : LastMacro.RightArrowChar);

                    RecordMove(macroType, "Right", controlDown ? MoveScope.Word : MoveScope.Character, shiftDown);
                }
                    break;

                case Keys.Up: // Up Arrow
                    // SHIFT indicates selection, CTRL indicates paragraphs instead of lines
                {
                    LastMacro macroType = controlDown
                        ? (shiftDown ? LastMacro.UpArrowParaSelection : LastMacro.UpArrowPara)
                        : (shiftDown ? LastMacro.UpArrowLineSelection : LastMacro.UpArrowLine);

                    RecordMove(macroType, "Up", controlDown ? MoveScope.Paragraph : MoveScope.Line, shiftDown);
                }
                    break;

                case Keys.Down: // Down Arrow
                    // SHIFT indicates selection, CTRL indicates paragraphs instead of lines
                {
                    LastMacro macroType = controlDown
                        ? (shiftDown ? LastMacro.DownArrowParaSelection : LastMacro.DownArrowPara)
                        : (shiftDown ? LastMacro.DownArrowLineSelection : LastMacro.DownArrowLine);

                    RecordMove(macroType, "Down", controlDown ? MoveScope.Paragraph : MoveScope.Line, shiftDown);
                }
                    break;

                case Keys.Prior: // Page Up
                case Keys.Next: // Page Down
                    macroString += "ActiveDocument.Object.Move";

                    if (System.Windows.Forms.Keys.Prior == currentKey)
                    {
                        macroString += "Up";
                    }
                    else
                    {
                        macroString += "Down";
                    }

                    macroString += "(" + (int) (controlDown ? tom.tomConstants.tomWindow : tom.tomConstants.tomScreen) +
                                   ", 1, " + (int) (shiftDown ? tom.tomConstants.tomExtend : tom.tomConstants.tomMove) +
                                   ")";

                    m_Recorder.RecordLine(macroString);
                    break;

                case Keys.End:
                case Keys.Home:
                    macroString += "ActiveDocument.Object.";

                    if (System.Windows.Forms.Keys.End == currentKey)
                    {
                        macroString += "EndKey";
                    }
                    else
                    {
                        macroString += "HomeKey";
                    }

                    macroString += "(" + (int) (controlDown ? tom.tomConstants.tomStory : tom.tomConstants.tomLine) +
                                   ", " + (int) (shiftDown ? tom.tomConstants.tomExtend : tom.tomConstants.tomMove) +
                                   ")";

                    m_Recorder.RecordLine(macroString);
                    break;

                case Keys.Insert:
                    // Note that the CTRL completely disables this command.  Also the SHIFT+INSERT
                    // actually generates a WM_PASTE message rather than a WM_KEYDOWN
                    if (!controlDown)
                    {
                        macroString = "ActiveDocument.Object.Flags = ActiveDocument.Object.Flags Xor ";
                        macroString += (int) tom.tomConstants.tomSelOvertype;
                        m_Recorder.RecordLine(macroString);
                    }
                    break;
            }
        }

        // This event returns the literal key that was pressed and does not account for
        // case of characters.  KeyPress is used to handled printable characters.
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_Recorder.IsRecording())
            {
                RecordNonprintableChar(e.KeyCode);
            }
        }

        // The arguments of this event will give us the char value of the key press taking into
        // account other characters press such as shift or caps lock for proper casing.
        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (m_Recorder.IsRecording())
            {
                RecordPrintableChar(e.KeyChar);
            }
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (m_Recorder.IsRecording())
            {

            }
            else
            {

            }
        }
        #endregion

        #region Business methods

        public void SaveFile(string fileName = null)
        {
            if (m_resxAgg == null) return;

            m_resxAgg.Save();
        }

        public void LoadFile(string fileName)
        {
            m_resxAgg = new ResXAggregator(fileName);

            DataGridControl.DataSource = null;
            DataGridControl.DataSource = m_resxAgg.DataTable;
            DataGridControl.Columns[0].Frozen = true;

            var maxColumnWidth = GridEditorSettings.ColumnMaxWidth;

            for (var i = 0; i < DataGridControl.Columns.Count; i++)
            {
                var column = DataGridControl.Columns[i];

                if (column.Width <= maxColumnWidth) continue;

                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                column.Width = maxColumnWidth;
            }

        }

        #endregion

        private void dataGridView1_BindingContextChanged(object sender, System.EventArgs e)
        {
            //MessageBox.Show("Binding Context Changed.");
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show("Cell value changed.");
        }
    }
}
