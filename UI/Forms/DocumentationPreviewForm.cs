using System;
using System.Drawing;
using System.Windows.Forms;
using Acme.Packages.Menu.Core.Domain.DTOs;

namespace Acme.Packages.Menu.UI.Forms
{
    public class DocumentationPreviewForm : Form
    {
        private TextBox txtMarkdown;
        private Button btnCopy;
        private Button btnSave;
        private Button btnClose;
        private Label lblTitle;
        private string _content;
        private string _fileName;

        public DocumentationPreviewForm(string content, string objectName)
        {
            _content = content;
            _fileName = objectName + ".md";
            InitializeComponent();
            txtMarkdown.Text = _content;
            lblTitle.Text = "Documentación generada para: " + objectName;
        }

        private void InitializeComponent()
        {
            this.txtMarkdown = new TextBox();
            this.btnCopy = new Button();
            this.btnSave = new Button();
            this.btnClose = new Button();
            this.lblTitle = new Label();

            this.SuspendLayout();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblTitle.Location = new Point(12, 10);
            this.lblTitle.Size = new Size(300, 19);
            this.lblTitle.Text = "Documentación generada";

            // txtMarkdown
            this.txtMarkdown.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.txtMarkdown.Font = new Font("Consolas", 10F);
            this.txtMarkdown.Location = new Point(12, 40);
            this.txtMarkdown.Multiline = true;
            this.txtMarkdown.Name = "txtMarkdown";
            this.txtMarkdown.ReadOnly = true;
            this.txtMarkdown.ScrollBars = ScrollBars.Vertical;
            this.txtMarkdown.Size = new Size(560, 380);
            this.txtMarkdown.TabIndex = 0;

            // btnCopy
            this.btnCopy.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnCopy.Location = new Point(12, 430);
            this.btnCopy.Size = new Size(100, 30);
            this.btnCopy.Text = "Copiar";
            this.btnCopy.Click += btnCopy_Click;

            // btnSave
            this.btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnSave.Location = new Point(118, 430);
            this.btnSave.Size = new Size(100, 30);
            this.btnSave.Text = "Guardar como...";
            this.btnSave.Click += btnSave_Click;

            // btnClose
            this.btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnClose.Location = new Point(472, 430);
            this.btnClose.Size = new Size(100, 30);
            this.btnClose.Text = "Cerrar";
            this.btnClose.Click += (s, e) => this.Close();

            // DocumentationPreviewForm
            this.ClientSize = new Size(584, 472);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.txtMarkdown);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClose);
            this.MinimumSize = new Size(400, 300);
            this.Name = "DocumentationPreviewForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Vista Previa de Documentación (Markdown)";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMarkdown.Text))
            {
                Clipboard.SetText(txtMarkdown.Text);
                MessageBox.Show("Copiado al portapapeles.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.FileName = _fileName;
                sfd.Filter = "Markdown files (*.md)|*.md|All files (*.*)|*.*";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(sfd.FileName, txtMarkdown.Text);
                    MessageBox.Show("Archivo guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
