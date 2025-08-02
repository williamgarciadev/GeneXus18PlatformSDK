using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Acme.Packages.Menu
{
    public partial class LogDebugResultForm : Form
    {
        public LogDebugResultForm(List<string> logLines)
        {
            InitializeComponent();
            DisplayLogLines(logLines);
        }

        private void InitializeComponent()
        {
            this.txtLogDebug = new System.Windows.Forms.TextBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtLogDebug
            // 
            this.txtLogDebug.Location = new System.Drawing.Point(12, 12);
            this.txtLogDebug.Multiline = true;
            this.txtLogDebug.Name = "txtLogDebug";
            this.txtLogDebug.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLogDebug.Size = new System.Drawing.Size(560, 400);
            this.txtLogDebug.TabIndex = 0;
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(416, 430);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 30);
            this.btnCopy.TabIndex = 1;
            this.btnCopy.Text = "Copiar";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(497, 430);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 30);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Guardar";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // LogDebugResultForm
            // 
            this.ClientSize = new System.Drawing.Size(584, 471);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.txtLogDebug);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LogDebugResultForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Líneas de Log.Debug Generadas";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private TextBox txtLogDebug;
        private Button btnCopy;
        private Button btnSave;

        private void DisplayLogLines(List<string> logLines)
        {
            if (logLines != null && logLines.Count > 0)
            {
                txtLogDebug.Text = string.Join(Environment.NewLine, logLines);
            }
            else
            {
                txtLogDebug.Text = "No se generaron líneas de Log.Debug.";
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLogDebug.Text))
            {
                Clipboard.SetText(txtLogDebug.Text);
                MessageBox.Show("Las líneas de Log.Debug han sido copiadas al portapapeles.", "Copiado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLogDebug.Text))
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Archivos de Texto (*.txt)|*.txt|Todos los Archivos (*.*)|*.*";
                    saveFileDialog.Title = "Guardar Líneas de Log.Debug";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            System.IO.File.WriteAllText(saveFileDialog.FileName, txtLogDebug.Text);
                            MessageBox.Show("Las líneas de Log.Debug han sido guardadas exitosamente.", "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ocurrió un error al guardar el archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
    }
}
