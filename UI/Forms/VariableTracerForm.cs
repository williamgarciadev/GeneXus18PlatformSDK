using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Acme.Packages.Menu.Core.Domain.DTOs;

namespace Acme.Packages.Menu.UI.Forms
{
    public class VariableTracerForm : Form
    {
        private DataGridView dgvTraces;
        private Label lblInfo;
        private Button btnClose;
        
        public event Action<VariableOccurrenceDto> OnJumpToCode;

        public VariableTracerForm(string varName, List<VariableOccurrenceDto> traces)
        {
            InitializeComponent();
            this.Text = $"Rastreador de Variable: {varName}";
            lblInfo.Text = $"Se encontraron {traces.Count} ocurrencias de {varName}:";
            LoadData(traces);
        }

        private void InitializeComponent()
        {
            this.dgvTraces = new DataGridView();
            this.lblInfo = new Label();
            this.btnClose = new Button();

            this.SuspendLayout();

            // lblInfo
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblInfo.Location = new Point(12, 10);
            this.lblInfo.Text = "Resultados del rastreo:";

            // dgvTraces
            this.dgvTraces.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvTraces.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTraces.Location = new Point(12, 35);
            this.dgvTraces.Name = "dgvTraces";
            this.dgvTraces.Size = new Size(760, 350);
            this.dgvTraces.AllowUserToAddRows = false;
            this.dgvTraces.ReadOnly = true;
            this.dgvTraces.RowHeadersVisible = false;
            this.dgvTraces.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvTraces.DoubleClick += DgvTraces_DoubleClick;

            // btnClose
            this.btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnClose.Location = new Point(697, 395);
            this.btnClose.Size = new Size(75, 30);
            this.btnClose.Text = "Cerrar";
            this.btnClose.Click += (s, e) => this.Close();

            // Form
            this.ClientSize = new Size(784, 437);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.dgvTraces);
            this.Controls.Add(this.btnClose);
            this.MinimumSize = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadData(List<VariableOccurrenceDto> traces)
        {
            dgvTraces.Columns.Clear();
            dgvTraces.Columns.Add("Type", "Acción");
            dgvTraces.Columns.Add("Part", "Parte");
            dgvTraces.Columns.Add("Line", "Línea");
            dgvTraces.Columns.Add("Code", "Código");

            dgvTraces.Columns["Type"].Width = 80;
            dgvTraces.Columns["Part"].Width = 100;
            dgvTraces.Columns["Line"].Width = 60;
            dgvTraces.Columns["Code"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            foreach (var t in traces)
            {
                string actionIcon = t.Type == OccurrenceType.Write ? "📝 Escritura" : "📖 Lectura";
                int rowIndex = dgvTraces.Rows.Add(actionIcon, t.PartName, t.LineNumber, t.FullLine);
                dgvTraces.Rows[rowIndex].Tag = t;
                
                if (t.Type == OccurrenceType.Write)
                    dgvTraces.Rows[rowIndex].DefaultCellStyle.BackColor = Color.OldLace;
            }
        }

        private void DgvTraces_DoubleClick(object sender, EventArgs e)
        {
            if (dgvTraces.SelectedRows.Count > 0)
            {
                var trace = (VariableOccurrenceDto)dgvTraces.SelectedRows[0].Tag;
                OnJumpToCode?.Invoke(trace);
            }
        }
    }
}
