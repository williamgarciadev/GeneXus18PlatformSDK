namespace Acme.Packages.Menu.UI.Forms
{
    partial class FormCodigo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnBrowse = new System.Windows.Forms.Button();
            this.chkProcedures = new System.Windows.Forms.CheckBox();
            this.chkWebPanels = new System.Windows.Forms.CheckBox();
            this.txtFilterName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(447, 354);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "button1";
            this.btnBrowse.UseVisualStyleBackColor = true;
            // 
            // chkProcedures
            // 
            this.chkProcedures.AutoSize = true;
            this.chkProcedures.Checked = true;
            this.chkProcedures.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkProcedures.Location = new System.Drawing.Point(210, 156);
            this.chkProcedures.Name = "chkProcedures";
            this.chkProcedures.Size = new System.Drawing.Size(80, 17);
            this.chkProcedures.TabIndex = 1;
            this.chkProcedures.Text = "checkBox1";
            this.chkProcedures.UseVisualStyleBackColor = true;
            // 
            // chkWebPanels
            // 
            this.chkWebPanels.AutoSize = true;
            this.chkWebPanels.Checked = true;
            this.chkWebPanels.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWebPanels.Location = new System.Drawing.Point(210, 216);
            this.chkWebPanels.Name = "chkWebPanels";
            this.chkWebPanels.Size = new System.Drawing.Size(80, 17);
            this.chkWebPanels.TabIndex = 2;
            this.chkWebPanels.Text = "checkBox1";
            this.chkWebPanels.UseVisualStyleBackColor = true;
            // 
            // txtFilterName
            // 
            this.txtFilterName.Location = new System.Drawing.Point(210, 272);
            this.txtFilterName.Name = "txtFilterName";
            this.txtFilterName.Size = new System.Drawing.Size(100, 20);
            this.txtFilterName.TabIndex = 3;
            // 
            // FormCodigo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtFilterName);
            this.Controls.Add(this.chkWebPanels);
            this.Controls.Add(this.chkProcedures);
            this.Controls.Add(this.btnBrowse);
            this.Name = "FormCodigo";
            this.Text = "FormCodigo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.CheckBox chkProcedures;
        private System.Windows.Forms.CheckBox chkWebPanels;
        private System.Windows.Forms.TextBox txtFilterName;
    }
}