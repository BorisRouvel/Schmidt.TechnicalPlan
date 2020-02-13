namespace Schmidt.TechnicalPlan
{
    partial class TechnicalPlanDocumentFileNameForm
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
            this.ok_TechPlanUI_BTN = new System.Windows.Forms.Button();
            this.cancel_TechPlanUI_BTN = new System.Windows.Forms.Button();
            this.technicalPlanFiileName_TBX = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ok_TechPlanUI_BTN
            // 
            this.ok_TechPlanUI_BTN.Location = new System.Drawing.Point(12, 38);
            this.ok_TechPlanUI_BTN.Name = "ok_TechPlanUI_BTN";
            this.ok_TechPlanUI_BTN.Size = new System.Drawing.Size(75, 23);
            this.ok_TechPlanUI_BTN.TabIndex = 1;
            this.ok_TechPlanUI_BTN.Text = "button1";
            this.ok_TechPlanUI_BTN.UseVisualStyleBackColor = true;
            this.ok_TechPlanUI_BTN.Click += new System.EventHandler(this.ok_TechPlanUI_BTN_Click);
            // 
            // cancel_TechPlanUI_BTN
            // 
            this.cancel_TechPlanUI_BTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel_TechPlanUI_BTN.Location = new System.Drawing.Point(98, 38);
            this.cancel_TechPlanUI_BTN.Name = "cancel_TechPlanUI_BTN";
            this.cancel_TechPlanUI_BTN.Size = new System.Drawing.Size(75, 23);
            this.cancel_TechPlanUI_BTN.TabIndex = 2;
            this.cancel_TechPlanUI_BTN.Text = "button2";
            this.cancel_TechPlanUI_BTN.UseVisualStyleBackColor = true;
            this.cancel_TechPlanUI_BTN.Click += new System.EventHandler(this.cancel_TechPlanUI_BTN_Click);
            // 
            // technicalPlanFiileName_TBX
            // 
            this.technicalPlanFiileName_TBX.Location = new System.Drawing.Point(12, 12);
            this.technicalPlanFiileName_TBX.Name = "technicalPlanFiileName_TBX";
            this.technicalPlanFiileName_TBX.Size = new System.Drawing.Size(161, 20);
            this.technicalPlanFiileName_TBX.TabIndex = 0;
            this.technicalPlanFiileName_TBX.TextChanged += new System.EventHandler(this.technicalPlanFiileName_TBX_TextChanged);
            // 
            // TechnicalPlanDocumentFileNameForm
            // 
            this.AcceptButton = this.ok_TechPlanUI_BTN;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel_TechPlanUI_BTN;
            this.ClientSize = new System.Drawing.Size(185, 72);
            this.ControlBox = false;
            this.Controls.Add(this.technicalPlanFiileName_TBX);
            this.Controls.Add(this.cancel_TechPlanUI_BTN);
            this.Controls.Add(this.ok_TechPlanUI_BTN);
            this.Name = "TechnicalPlanDocumentFileNameForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Title";
            this.Load += new System.EventHandler(this.TechnicalPlanDocumentFileNameForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ok_TechPlanUI_BTN;
        private System.Windows.Forms.Button cancel_TechPlanUI_BTN;
        public System.Windows.Forms.TextBox technicalPlanFiileName_TBX;
    }
}