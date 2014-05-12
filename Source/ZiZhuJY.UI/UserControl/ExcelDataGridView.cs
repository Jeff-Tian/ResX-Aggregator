namespace ZiZhuJY.UI.UserControl
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// A customized DataGridView. To support tab key and enter key.
    /// </summary>
    public class ExcelDataGridView : DataGridView
    {
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C))
            {
                if (this.CurrentCell != null)
                {
                    //MessageBox.Show("Ctrl+C: " + this.CurrentCell.Value);
                    //return true;
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else if (keyData == (Keys.Control | Keys.V))
            {
                //MessageBox.Show("Ctrl+V:" + this.CurrentCell.Value);
                //return true;
                return false;
            }

            if (msg.WParam.ToInt32().Equals(46)) 
            {
                // Delete key was pressed
                return false;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            return base.ProcessKeyEventArgs(ref m);
        }

        protected override bool ProcessKeyMessage(ref Message m)
        {
            return base.ProcessKeyMessage(ref m);
        }

        protected override bool ProcessKeyPreview(ref Message m)
        {
            return base.ProcessKeyPreview(ref m);
        }

        /// <summary>
        /// Processes keys used for navigating in the <see cref="T:System.Windows.Forms.DataGridView" />.
        /// </summary>
        /// <param name="e">Contains information about the key that was pressed.</param>
        /// <returns>
        /// true if the key was processed; otherwise, false.
        /// </returns>
        protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Tab:
                    if (this.EditingControl != null)
                    {
                        if (this.EditingControl is TextBox)
                        {
                            /*
                                                        TextBox txt = this.EditingControl as TextBox;
                                                        int tmp = txt.SelectionStart;
                                                        txt.Text = txt.Text.Insert(txt.SelectionStart, "\t");
                                                        txt.SelectionStart = tmp + "\t".Length;*/
                            return true;
                        }
                    }
                    return false;
                case Keys.Enter:
                    if (this.EditingControl != null)
                    {
                        if (this.EditingControl is TextBox)
                        {
                            TextBox txt = this.EditingControl as TextBox;
                            int tmp = txt.SelectionStart;
                            txt.Text = txt.Text.Insert(txt.SelectionStart, Environment.NewLine);
                            txt.SelectionStart = tmp + Environment.NewLine.Length;
                            return true;
                        }
                    }
                    return false;
                case Keys.Up:
                    if (this.EditingControl != null)
                    {
                        if (this.EditingControl is TextBox)
                        {
                            return true;
                        }
                    }
                    return false;
                case Keys.Down:
                    if (this.EditingControl != null)
                    {
                        if (this.EditingControl is TextBox)
                        {
                            return true;
                        }
                    }
                    return false;
                case Keys.Delete:
                    if (this.EditingControl != null)
                    {
                        if (this.EditingControl is TextBox)
                        {
                            return true;
                        }
                    }
                    return false;
            }

            return base.ProcessDataGridViewKey(e);
        }
    }
}
