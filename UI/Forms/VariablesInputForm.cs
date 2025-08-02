using Artech.Architecture.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Acme.Packages.Menu
{
    public partial class VariablesInputForm : Form
    {
        public List<string> InputVariables { get; private set; }
        public string SelOutputType { get; private set; }

        public VariablesInputForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtVariables = new System.Windows.Forms.TextBox();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            this.SuspendLayout();
            // 
            // txtVariables
            // 
            this.txtVariables.Location = new System.Drawing.Point(12, 35);
            this.txtVariables.Multiline = true;
            this.txtVariables.Name = "txtVariables";
            this.txtVariables.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtVariables.Size = new System.Drawing.Size(360, 200);
            this.txtVariables.TabIndex = 0;
            // 
            // lblInstructions
            // 
            this.lblInstructions.AutoSize = true;
            this.lblInstructions.Location = new System.Drawing.Point(12, 15);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(298, 13);
            this.lblInstructions.TabIndex = 1;
            this.lblInstructions.Text = "Pega o escribe las variables, separadas por comas o saltos de línea:";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(216, 250);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 30);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "Generar";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(297, 250);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // 
            // cmbOutputType
            // 
            this.cmbOutputType = new System.Windows.Forms.ComboBox();
            this.cmbOutputType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOutputType.Items.AddRange(new object[] {
            "Generar msg",
            "Generar Log.Debug"});
            this.cmbOutputType.Location = new System.Drawing.Point(12, 250);
            this.cmbOutputType.Name = "cmbOutputType";
            this.cmbOutputType.Size = new System.Drawing.Size(198, 21);
            this.cmbOutputType.TabIndex = 4;
            this.cmbOutputType.SelectedIndex = 0;

            // 
            // VariablesInputForm
            // 
            this.ClientSize = new System.Drawing.Size(384, 301);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.txtVariables);
            this.Controls.Add(this.cmbOutputType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "VariablesInputForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Entrada de Variables";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private TextBox txtVariables;
        private Label lblInstructions;
        private Button btnGenerate;
        private Button btnCancel;
        private ComboBox cmbOutputType;

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            var inputText = txtVariables.Text.Trim();
            if (string.IsNullOrEmpty(inputText))
            {
                MessageBox.Show("Por favor, ingresa al menos una variable.", "Entrada Vacía", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Definir los separadores: comas y saltos de línea
            char[] separators = new char[] { ',', '\n', '\r' };

            // Separar las variables por comas y/o saltos de línea, eliminar espacios y filtrar entradas vacías
            var rawVariables = inputText.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(v => v.Trim())
                                         .Where(v => !string.IsNullOrEmpty(v))
                                         .ToList();

            if (rawVariables.Count == 0)
            {
                MessageBox.Show("No se encontraron variables válidas.", "Entrada Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CommonServices.Output.AddLine("Raw Variables: " + string.Join(", ", rawVariables));


            //// Asignar las variables ingresadas a la propiedad InputVariables
            //InputVariables = rawVariables;

            //this.DialogResult = DialogResult.OK;
            //this.Close();

            // Determinar el tipo de salida seleccionado
            string selectedOutputType = cmbOutputType.SelectedItem.ToString();
            List<string> outputLines = new List<string>();

            if (selectedOutputType == "Generar msg")
            {
                // Generar líneas para msg
                SelOutputType = "msg";
            }
            else if (selectedOutputType == "Generar Log.Debug")
            {
                // Generar líneas para Log.Debug
                SelOutputType = "log";
            }


            // Asignar las variables ingresadas a la propiedad InputVariables
            InputVariables = rawVariables;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            InputVariables = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
