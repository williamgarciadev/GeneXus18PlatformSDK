using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using Artech.Architecture.UI.Framework.Services;
using Artech.Architecture.Common.Objects;
using Artech.Genexus.Common.Objects;
using Acme.Packages.Menu.Services.Analysis;
using Acme.Packages.Menu.Models;
using Acme.Packages.Menu.Utilities;

namespace Acme.Packages.Menu.UI.Forms
{
    /// <summary>
    /// Formulario para mostrar conteo de líneas de código por objeto
    /// </summary>
    public partial class CodeLinesCountForm : Form
    {
        private DataGridView dgvResults;
        private Panel pnlTop;
        private Panel pnlBottom;
        private Button btnAnalyze;
        private Button btnExportCSV;
        private Button btnClose;
        private Label lblStatus;
        private ProgressBar progressBar;
        private CheckBox chkProcedures;
        private CheckBox chkWebPanels;
        private GroupBox grpOptions;
        private Label lblSummary;

        private List<CodeLineInfo> _results;

        public CodeLinesCountForm()
        {
            InitializeComponent();
            SetupDataGridViewColumns();
            InitializeData();
            
            // Agregar evento Load para configuración final
            this.Load += CodeLinesCountForm_Load;
        }
        
        private void CodeLinesCountForm_Load(object sender, EventArgs e)
        {
            // Configuración final de headers después de que el formulario se muestre
            dgvResults.EnableHeadersVisualStyles = false;
            dgvResults.ColumnHeadersVisible = true;
            dgvResults.ColumnHeadersHeight = 40;
            
            // Verificar que el estilo se aplique correctamente
            if (dgvResults.ColumnHeadersDefaultCellStyle.BackColor == Color.Empty)
            {
                dgvResults.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
                dgvResults.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvResults.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10F, FontStyle.Bold);
            }
            
            dgvResults.Refresh();
        }

        private void InitializeComponent()
        {
            // Form configuration
            this.Text = "📊 Contador de Líneas de Código - GeneXus Objects";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(800, 500);

            // Top Panel
            pnlTop = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = SystemColors.Control
            };

            // Options GroupBox
            grpOptions = new GroupBox()
            {
                Text = "Tipos de Objetos a Analizar",
                Location = new Point(10, 10),
                Size = new Size(200, 80)
            };

            chkProcedures = new CheckBox()
            {
                Text = "📋 Procedures",
                Location = new Point(10, 20),
                Checked = true
            };

            chkWebPanels = new CheckBox()
            {
                Text = "🌐 WebPanels", 
                Location = new Point(10, 45),
                Checked = true
            };

            grpOptions.Controls.AddRange(new Control[] { chkProcedures, chkWebPanels });

            // Analyze Button
            btnAnalyze = new Button()
            {
                Text = "🔍 Analizar Objetos",
                Location = new Point(230, 30),
                Size = new Size(150, 35),
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAnalyze.Click += BtnAnalyze_Click;

            // Export CSV Button
            btnExportCSV = new Button()
            {
                Text = "📁 Exportar CSV",
                Location = new Point(400, 30),
                Size = new Size(120, 35),
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnExportCSV.Click += BtnExportCSV_Click;

            // Status Label
            lblStatus = new Label()
            {
                Text = "Listo para analizar objetos",
                Location = new Point(550, 35),
                Size = new Size(300, 25),
                ForeColor = Color.DarkBlue
            };

            // Progress Bar
            progressBar = new ProgressBar()
            {
                Location = new Point(10, 95),
                Size = new Size(860, 20),
                Visible = false
            };

            pnlTop.Controls.AddRange(new Control[] 
            { 
                grpOptions, btnAnalyze, btnExportCSV, lblStatus, progressBar 
            });

            // DataGridView con configuración forzada para headers
            dgvResults = new DataGridView()
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                ColumnHeadersVisible = true,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 40,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing,
                GridColor = Color.LightGray,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                Font = new Font("Segoe UI", 9F),
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised
            };
            
            // Forzar estilo de headers inmediatamente
            dgvResults.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle()
            {
                BackColor = Color.DarkBlue,
                ForeColor = Color.White,
                Font = new Font("Arial", 10F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                SelectionBackColor = Color.DarkBlue,
                SelectionForeColor = Color.White
            };

            // Bottom Panel
            pnlBottom = new Panel()
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = SystemColors.Control
            };

            // Summary Label
            lblSummary = new Label()
            {
                Text = "",
                Location = new Point(10, 10),
                Size = new Size(600, 50),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkGreen
            };

            // Close Button
            btnClose = new Button()
            {
                Text = "Cerrar",
                Location = new Point(800, 25),
                Size = new Size(80, 30),
                DialogResult = DialogResult.OK
            };

            pnlBottom.Controls.AddRange(new Control[] { lblSummary, btnClose });

            // Add panels to form
            this.Controls.AddRange(new Control[] { pnlTop, dgvResults, pnlBottom });
        }

        private void SetupDataGridViewColumns()
        {
            // Crear columnas con headers explícitos
            dgvResults.Columns.Add("Objeto", "OBJETO");
            dgvResults.Columns.Add("Tipo", "TIPO");  
            dgvResults.Columns.Add("TotalLineas", "TOTAL");
            dgvResults.Columns.Add("LineasSinComentarios", "SIN COMENTARIOS");
            dgvResults.Columns.Add("LineasOperativas", "OPERATIVAS");
            dgvResults.Columns.Add("Complejidad", "COMPLEJIDAD");
            
            // Configurar anchos
            dgvResults.Columns[0].Width = 150;  // Objeto
            dgvResults.Columns[1].Width = 100;  // Tipo
            dgvResults.Columns[2].Width = 80;   // Total
            dgvResults.Columns[3].Width = 120;  // Sin comentarios
            dgvResults.Columns[4].Width = 100;  // Operativas
            dgvResults.Columns[5].Width = 120;  // Complejidad
            
            // Alineación para columnas numéricas
            dgvResults.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvResults.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvResults.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvResults.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            // Verificar que headers sean visibles
            dgvResults.ColumnHeadersVisible = true;
            dgvResults.ColumnHeadersHeight = 40;
            
            // Forzar repaint
            dgvResults.Invalidate();
            dgvResults.Update();
        }

        private void InitializeData()
        {
            _results = new List<CodeLineInfo>();
        }

        private void BtnAnalyze_Click(object sender, EventArgs e)
        {
            if (!chkProcedures.Checked && !chkWebPanels.Checked)
            {
                MessageBox.Show("⚠️ Debe seleccionar al menos un tipo de objeto para analizar.", 
                    "Selección Requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                AnalyzeObjects();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error durante el análisis:\n{ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Utils.Log($"Error en análisis de código: {ex}");
            }
        }

        private void AnalyzeObjects()
        {
            btnAnalyze.Enabled = false;
            btnExportCSV.Enabled = false;
            progressBar.Visible = true;
            _results.Clear();
            dgvResults.Rows.Clear();

            var model = UIServices.KB.CurrentModel;
            if (model?.KB == null)
            {
                throw new InvalidOperationException("No hay una Knowledge Base activa.");
            }

            var codeAnalyzer = new CodeAnalyzer();
            var objectsToAnalyze = GetObjectsToAnalyze(model);

            progressBar.Maximum = objectsToAnalyze.Count;
            progressBar.Value = 0;

            int processedCount = 0;
            foreach (var objectInfo in objectsToAnalyze)
            {
                try
                {
                    var obj = objectInfo.Item1;
                    var typeName = objectInfo.Item2;
                    
                    lblStatus.Text = $"Analizando {typeName}: {obj.Name}...";
                    Application.DoEvents();

                    var codeStats = codeAnalyzer.AnalyzeObjectCode(obj);
                    var complexity = GetComplexityLevel(codeStats.OperativeLines);
                    
                    var result = new CodeLineInfo
                    {
                        ObjectName = obj.Name,
                        ObjectType = typeName,
                        TotalLines = codeStats.TotalLines,
                        NonCommentLines = codeStats.NonCommentLines,
                        OperativeLines = codeStats.OperativeLines,
                        Complexity = complexity
                    };

                    _results.Add(result);
                    AddRowToGrid(result);

                    processedCount++;
                    progressBar.Value = processedCount;
                }
                catch (Exception ex)
                {
                    var obj = objectInfo.Item1;
                    var typeName = objectInfo.Item2;
                    
                    Utils.Log($"⚠️ Error procesando {typeName} '{obj.Name}': {ex.Message}");
                    
                    var errorResult = new CodeLineInfo
                    {
                        ObjectName = obj.Name,
                        ObjectType = typeName,
                        TotalLines = 0,
                        NonCommentLines = 0,
                        OperativeLines = 0,
                        Complexity = "Error"
                    };
                    
                    _results.Add(errorResult);
                    AddRowToGrid(errorResult);
                }
            }

            UpdateSummary();
            btnAnalyze.Enabled = true;
            btnExportCSV.Enabled = _results.Any();
            progressBar.Visible = false;
            lblStatus.Text = $"✅ Análisis completado: {processedCount} objetos procesados";
        }

        private List<(KBObject obj, string typeName)> GetObjectsToAnalyze(KBModel model)
        {
            var objects = new List<(KBObject, string)>();

            if (chkProcedures.Checked)
            {
                var procedures = model.GetObjects<Procedure>().ToList();
                objects.AddRange(procedures.Select(p => ((KBObject)p, "Procedure")));
            }

            if (chkWebPanels.Checked)
            {
                var webPanels = model.GetObjects<WebPanel>().ToList();
                objects.AddRange(webPanels.Select(w => ((KBObject)w, "WebPanel")));
            }

            return objects.OrderBy(x => x.Item2).ThenBy(x => x.Item1.Name).ToList();
        }

        private string GetComplexityLevel(int operativeLines)
        {
            if (operativeLines <= 10) return "🟢 Baja";
            if (operativeLines <= 50) return "🟡 Media";
            if (operativeLines <= 100) return "🟠 Alta";
            return "🔴 Muy Alta";
        }

        private void AddRowToGrid(CodeLineInfo info)
        {
            dgvResults.Rows.Add(
                info.ObjectName,
                info.ObjectType,
                info.TotalLines,
                info.NonCommentLines,
                info.OperativeLines,
                info.Complexity
            );

            // Color coding by complexity
            var lastRow = dgvResults.Rows[dgvResults.Rows.Count - 1];
            if (info.Complexity.Contains("Muy Alta"))
                lastRow.DefaultCellStyle.BackColor = Color.MistyRose;
            else if (info.Complexity.Contains("Alta"))
                lastRow.DefaultCellStyle.BackColor = Color.LemonChiffon;
        }

        private void UpdateSummary()
        {
            if (!_results.Any()) return;

            var totalObjects = _results.Count;
            var totalLines = _results.Sum(r => r.TotalLines);
            var totalOperative = _results.Sum(r => r.OperativeLines);
            var avgLinesPerObject = totalObjects > 0 ? totalLines / totalObjects : 0;

            var procedureCount = _results.Count(r => r.ObjectType == "Procedure");
            var webPanelCount = _results.Count(r => r.ObjectType == "WebPanel");

            lblSummary.Text = $"📊 Resumen: {totalObjects} objetos | {totalLines:N0} líneas totales | " +
                             $"{totalOperative:N0} operativas | Promedio: {avgLinesPerObject:N0} líneas/objeto\n" +
                             $"📋 Procedures: {procedureCount} | 🌐 WebPanels: {webPanelCount}";
        }

        private void BtnExportCSV_Click(object sender, EventArgs e)
        {
            if (!_results.Any())
            {
                MessageBox.Show("⚠️ No hay datos para exportar. Ejecute el análisis primero.", 
                    "Sin Datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var csvContent = GenerateCSVContent();
                var fileName = $"ConteoLineasCodigo_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var filePath = Path.Combine(desktopPath, fileName);

                File.WriteAllText(filePath, csvContent);

                var result = MessageBox.Show($"✅ Archivo CSV exportado exitosamente:\n{filePath}\n\n¿Desea abrir la carpeta?", 
                    "Exportación Exitosa", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("explorer.exe", "/select," + filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error al exportar CSV:\n{ex.Message}", 
                    "Error de Exportación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateCSVContent()
        {
            var lines = new List<string>
            {
                "Objeto,Tipo,TotalLineas,LineasSinComentarios,LineasOperativas,Complejidad"
            };

            foreach (var result in _results.OrderBy(r => r.ObjectType).ThenBy(r => r.ObjectName))
            {
                var complexityText = result.Complexity.Replace("🟢 ", "").Replace("🟡 ", "")
                                                    .Replace("🟠 ", "").Replace("🔴 ", "");
                
                lines.Add($"{result.ObjectName},{result.ObjectType},{result.TotalLines}," +
                         $"{result.NonCommentLines},{result.OperativeLines},{complexityText}");
            }

            return string.Join(Environment.NewLine, lines);
        }
    }

    /// <summary>
    /// Modelo de datos para información de líneas de código
    /// </summary>
    public class CodeLineInfo
    {
        public string ObjectName { get; set; }
        public string ObjectType { get; set; }
        public int TotalLines { get; set; }
        public int NonCommentLines { get; set; }
        public int OperativeLines { get; set; }
        public string Complexity { get; set; }
    }
}