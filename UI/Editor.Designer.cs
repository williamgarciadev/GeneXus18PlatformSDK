using Artech.Architecture.UI.Framework.Packages;
using Artech.Common.Framework.Commands;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Forms;

namespace Acme.Packages.Menu
{
	partial class MyEditor
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

        // Registrar el comando
        public static CommandKey GenerateLogDebug { get; }

        static MyEditor()
        {
            GenerateLogDebug = new CommandKey(UIPackageGuid.Core, "GenerateLogDebug");
        }


        // Método que ejecuta el comando
        public static void ExecGenerateLogDebug(string selectedText)
        {
            if (string.IsNullOrEmpty(selectedText))
            {
                MessageBox.Show("No se ha seleccionado ningún código.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Extraer variables
            List<string> variables = ExtractVariables(selectedText);
            if (variables.Count == 0)
            {
                MessageBox.Show("No se encontraron variables.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Generar Log.Debug
            string logCode = GenerateLogDebugCode(variables);
            Clipboard.SetText(logCode); // Copiar al portapapeles
            MessageBox.Show("Código Log.Debug copiado al portapapeles.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Método para extraer variables
        private static List<string> ExtractVariables(string code)
        {
            var variables = new List<string>();
            string pattern = @"&(\w+)";
            var matches = Regex.Matches(code, pattern);

            foreach (Match match in matches)
            {
                variables.Add(match.Value);
            }

            return variables;
        }

        // Método para generar código Log.Debug
        private static string GenerateLogDebugCode(List<string> variables)
        {
            var logLines = new StringBuilder();

            foreach (var variable in variables)
            {
                logLines.AppendLine($"Log.Debug(Format(\"{variable}=%1\", {variable}), \"{variable}\");");
            }

            return logLines.ToString();
        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBox = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// richTextBox1
			// 
			this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox.EnableAutoDragDrop = true;
			this.textBox.Location = new System.Drawing.Point(0, 0);
			this.textBox.Name = "richTextBox1";
			this.textBox.ShowSelectionMargin = true;
			this.textBox.Size = new System.Drawing.Size(150, 150);
			this.textBox.TabIndex = 0;
			this.textBox.Text = "";
			// 
			// MyEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textBox);
			this.Name = "MyEditor";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox textBox;
	}
}
