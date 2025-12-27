using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Acme.Packages.Menu.Models;

namespace Acme.Packages.Menu.UI.Forms
{
    public class SmartFixVariablesForm : Form
    {
        private DataGridView dgvVariables;
        private Button btnConfirm;
        private Button btnCancel;
        private Label lblHeader;
        public List<VariableInfo> SelectedVariables { get; private set; }

        public SmartFixVariablesForm(List<VariableInfo> suggestedVariables)
        {
            InitializeComponent();
            LoadData(suggestedVariables);
        }

        private void InitializeComponent()
        {
            this.dgvVariables = new DataGridView();
            this.btnConfirm = new Button();
            this.btnCancel = new Button();
            this.lblHeader = new Label();

            this.SuspendLayout();

            // lblHeader
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblHeader.Location = new Point(12, 10);
            this.lblHeader.Text = "Variables detectadas sin definir en el objeto:";

            // dgvVariables
            this.dgvVariables.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvVariables.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVariables.Location = new Point(12, 35);
            this.dgvVariables.Name = "dgvVariables";
            this.dgvVariables.Size = new Size(560, 300);
            this.dgvVariables.AllowUserToAddRows = false;
            this.dgvVariables.RowHeadersVisible = false;
            this.dgvVariables.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // btnConfirm
            this.btnConfirm.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnConfirm.Location = new Point(416, 345);
            this.btnConfirm.Size = new Size(75, 30);
            this.btnConfirm.Text = "Definir";
            this.btnConfirm.Click += btnConfirm_Click;

            // btnCancel
            this.btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnCancel.Location = new Point(497, 345);
            this.btnCancel.Size = new Size(75, 30);
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.Click += (s, e) => this.Close();

            // Form
            this.ClientSize = new Size(584, 387);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.dgvVariables);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.MinimumSize = new Size(400, 300);
            this.Name = "SmartFixVariablesForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Smart Fix - Auto-Definición de Variables";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadData(List<VariableInfo> variables)
        {
            dgvVariables.Columns.Clear();
            
            var colCheck = new DataGridViewCheckBoxColumn { HeaderText = "Crear", Name = "Create", Width = 50 };
            dgvVariables.Columns.Add(colCheck);
            dgvVariables.Columns.Add("Name", "Nombre");
            dgvVariables.Columns.Add("Type", "Tipo Sugerido (Editable)");
            dgvVariables.Columns.Add("Reference", "Basado en KB");

            foreach (var v in variables)
            {
                string typeStr = FormatType(v);
                int rowIndex = dgvVariables.Rows.Add(
                    true, 
                    v.CleanName, 
                    typeStr,
                    v.IsBasedOnAttributeOrDomain ? "✅ " + v.BaseReference : "-"
                );
                dgvVariables.Rows[rowIndex].Tag = v;
            }

            dgvVariables.Columns["Name"].ReadOnly = true;
            dgvVariables.Columns["Type"].ReadOnly = false; // ¡AHORA EDITABLE!
            dgvVariables.Columns["Reference"].ReadOnly = true;
            dgvVariables.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private string FormatType(VariableInfo v)
        {
            if (v.Type == Artech.Genexus.Common.eDBType.NUMERIC)
                return $"N({v.Length}.{v.Decimals})";
            if (v.Type == Artech.Genexus.Common.eDBType.VARCHAR)
                return $"V({v.Length})";
            if (v.Type == Artech.Genexus.Common.eDBType.CHARACTER)
                return $"C({v.Length})";
            if (v.Type == Artech.Genexus.Common.eDBType.Boolean)
                return "Boolean";
            if (v.Type == Artech.Genexus.Common.eDBType.DATE)
                return "Date";
            if (v.Type == Artech.Genexus.Common.eDBType.DATETIME)
                return "DateTime";
            
            return v.Type.ToString() + (v.Length > 0 ? $"({v.Length})" : "");
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            SelectedVariables = new List<VariableInfo>();
            foreach (DataGridViewRow row in dgvVariables.Rows)
            {
                if ((bool)row.Cells["Create"].Value)
                {
                    var v = (VariableInfo)row.Tag;
                    string manualType = row.Cells["Type"].Value.ToString();
                    
                    // Si el usuario editó manualmente el tipo, intentamos parsearlo
                    ApplyManualType(v, manualType);
                    
                    SelectedVariables.Add(v);
                }
            }

            if (!SelectedVariables.Any())
            {
                MessageBox.Show("Por favor, seleccione al menos una variable.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ApplyManualType(VariableInfo v, string text)
        {
            if (v.IsBasedOnAttributeOrDomain && text.Contains("✅")) return;

            text = text.ToUpper().Trim();

            // Regex para capturar Tipo(Largo.Dec) ej: N(10.2) o V(128)
            var match = System.Text.RegularExpressions.Regex.Match(text, @"^([NVC])\((\d+)(?:\.(\d+))?\)$");
            if (match.Success)
            {
                char t = match.Groups[1].Value[0];
                int len = int.Parse(match.Groups[2].Value);
                int dec = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 0;

                v.Length = len;
                v.Decimals = dec;
                if (t == 'N') v.Type = Artech.Genexus.Common.eDBType.NUMERIC;
                if (t == 'V') v.Type = Artech.Genexus.Common.eDBType.VARCHAR;
                if (t == 'C') v.Type = Artech.Genexus.Common.eDBType.CHARACTER;
                return;
            }

            // Fallback para tipos simples
            if (text.StartsWith("BOOL")) v.Type = Artech.Genexus.Common.eDBType.Boolean;
            else if (text.StartsWith("DATE")) v.Type = Artech.Genexus.Common.eDBType.DATE;
            else if (text.StartsWith("DATETI")) v.Type = Artech.Genexus.Common.eDBType.DATETIME;
        }
    }
}
