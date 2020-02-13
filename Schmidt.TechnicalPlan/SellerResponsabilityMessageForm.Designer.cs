namespace Schmidt.TechnicalPlan
{
    partial class SellerResponsabilityMessageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SellerResponsabilityMessageForm));
            this.SellerResponsabilityMessage_GBX = new System.Windows.Forms.GroupBox();
            this.SellerResponsabilityText_LAB = new System.Windows.Forms.Label();
            this.Cancel_BTN = new System.Windows.Forms.Button();
            this.Ok_BTN = new System.Windows.Forms.Button();
            this.SellerResponsabilityMessage_GBX.SuspendLayout();
            this.SuspendLayout();
            // 
            // SellerResponsabilityMessage_GBX
            // 
            this.SellerResponsabilityMessage_GBX.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SellerResponsabilityMessage_GBX.Controls.Add(this.SellerResponsabilityText_LAB);
            this.SellerResponsabilityMessage_GBX.Location = new System.Drawing.Point(12, 12);
            this.SellerResponsabilityMessage_GBX.Name = "SellerResponsabilityMessage_GBX";
            this.SellerResponsabilityMessage_GBX.Size = new System.Drawing.Size(534, 290);
            this.SellerResponsabilityMessage_GBX.TabIndex = 0;
            this.SellerResponsabilityMessage_GBX.TabStop = false;
            // 
            // SellerResponsabilityText_LAB
            // 
            this.SellerResponsabilityText_LAB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SellerResponsabilityText_LAB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SellerResponsabilityText_LAB.Location = new System.Drawing.Point(6, 16);
            this.SellerResponsabilityText_LAB.Name = "SellerResponsabilityText_LAB";
            this.SellerResponsabilityText_LAB.Size = new System.Drawing.Size(522, 271);
            this.SellerResponsabilityText_LAB.TabIndex = 0;
            this.SellerResponsabilityText_LAB.Text = resources.GetString("SellerResponsabilityText_LAB.Text");
            this.SellerResponsabilityText_LAB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Cancel_BTN
            // 
            this.Cancel_BTN.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Cancel_BTN.BackColor = System.Drawing.SystemColors.HotTrack;
            this.Cancel_BTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_BTN.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.Cancel_BTN.Location = new System.Drawing.Point(184, 308);
            this.Cancel_BTN.Name = "Cancel_BTN";
            this.Cancel_BTN.Size = new System.Drawing.Size(93, 38);
            this.Cancel_BTN.TabIndex = 1;
            this.Cancel_BTN.UseVisualStyleBackColor = false;
            this.Cancel_BTN.Click += new System.EventHandler(this.Cancel_BTN_Click);
            // 
            // Ok_BTN
            // 
            this.Ok_BTN.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Ok_BTN.BackColor = System.Drawing.SystemColors.HotTrack;
            this.Ok_BTN.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.Ok_BTN.Location = new System.Drawing.Point(283, 308);
            this.Ok_BTN.Name = "Ok_BTN";
            this.Ok_BTN.Size = new System.Drawing.Size(93, 38);
            this.Ok_BTN.TabIndex = 0;
            this.Ok_BTN.UseVisualStyleBackColor = false;
            this.Ok_BTN.Click += new System.EventHandler(this.Ok_BTN_Click);
            // 
            // SellerResponsabilityMessageForm
            // 
            this.AcceptButton = this.Ok_BTN;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_BTN;
            this.ClientSize = new System.Drawing.Size(558, 358);
            this.ControlBox = false;
            this.Controls.Add(this.Ok_BTN);
            this.Controls.Add(this.Cancel_BTN);
            this.Controls.Add(this.SellerResponsabilityMessage_GBX);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SellerResponsabilityMessageForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SellerResponsabilityMessageForm";
            this.Load += new System.EventHandler(this.SellerResponsabilityMessageForm_Load);
            this.SellerResponsabilityMessage_GBX.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox SellerResponsabilityMessage_GBX;
        private System.Windows.Forms.Label SellerResponsabilityText_LAB;
        private System.Windows.Forms.Button Cancel_BTN;
        private System.Windows.Forms.Button Ok_BTN;
    }
}