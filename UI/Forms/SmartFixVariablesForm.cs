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
            dgvVariables.Columns.Add("Type", "Tipo Sugerido");
            dgvVariables.Columns.Add("Reference", "Basado en KB");

            foreach (var v in variables)
            {
                int rowIndex = dgvVariables.Rows.Add(
                    true, 
                    v.CleanName, 
                    v.Type.ToString() + (v.Length > 0 ? $"({v.Length})" : ""),
                    v.IsBasedOnAttributeOrDomain ? "✅ " + v.BaseReference : "-"
                );
                dgvVariables.Rows[rowIndex].Tag = v;
            }

            dgvVariables.Columns["Name"].ReadOnly = true;
            dgvVariables.Columns["Type"].ReadOnly = true;
            dgvVariables.Columns["Reference"].ReadOnly = true;
            dgvVariables.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            SelectedVariables = new List<VariableInfo>();
            foreach (DataGridViewRow row in dgvVariables.Rows)
            {
                if ((bool)row.Cells["Create"].Value)
                {
                    SelectedVariables.Add((VariableInfo)row.Tag);
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
    }
}
