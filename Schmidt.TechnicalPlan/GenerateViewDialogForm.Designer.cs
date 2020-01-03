namespace Schmidt.TechnicalPlan
{
    partial class GenerateViewDialogForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenerateViewDialogForm));
            this.iconBar_TSP = new System.Windows.Forms.ToolStrip();
            this.transferSM2_BTN = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.print_BTN = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.openFolder_BTN = new System.Windows.Forms.ToolStripButton();
            this.OrientationList_CBX = new System.Windows.Forms.ComboBox();
            this.PaperList_CBX = new System.Windows.Forms.ComboBox();
            this.ScaleList_CBX = new System.Windows.Forms.ComboBox();
            this.PdfDocumentView_SyncPDFV = new Syncfusion.Windows.Forms.PdfViewer.PdfDocumentView();
            this.myImageButton = new Schmidt.TechnicalPlan.MyImageButton();
            this.MyListView_MLV = new Schmidt.TechnicalPlan.MyListView();
            this.iconBar_TSP.SuspendLayout();
            this.SuspendLayout();
            // 
            // iconBar_TSP
            // 
            this.iconBar_TSP.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.iconBar_TSP.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.transferSM2_BTN,
            this.toolStripSeparator1,
            this.print_BTN,
            this.toolStripSeparator2,
            this.openFolder_BTN});
            this.iconBar_TSP.Location = new System.Drawing.Point(0, 0);
            this.iconBar_TSP.Name = "iconBar_TSP";
            this.iconBar_TSP.Size = new System.Drawing.Size(984, 35);
            this.iconBar_TSP.TabIndex = 0;
            // 
            // transferSM2_BTN
            // 
            this.transferSM2_BTN.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.transferSM2_BTN.Image = ((System.Drawing.Image)(resources.GetObject("transferSM2_BTN.Image")));
            this.transferSM2_BTN.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.transferSM2_BTN.Name = "transferSM2_BTN";
            this.transferSM2_BTN.Size = new System.Drawing.Size(32, 32);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // print_BTN
            // 
            this.print_BTN.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.print_BTN.Image = ((System.Drawing.Image)(resources.GetObject("print_BTN.Image")));
            this.print_BTN.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.print_BTN.Name = "print_BTN";
            this.print_BTN.Size = new System.Drawing.Size(32, 32);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 35);
            // 
            // openFolder_BTN
            // 
            this.openFolder_BTN.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openFolder_BTN.Image = ((System.Drawing.Image)(resources.GetObject("openFolder_BTN.Image")));
            this.openFolder_BTN.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openFolder_BTN.Name = "openFolder_BTN";
            this.openFolder_BTN.Size = new System.Drawing.Size(32, 32);
            // 
            // OrientationList_CBX
            // 
            this.OrientationList_CBX.FormattingEnabled = true;
            this.OrientationList_CBX.Location = new System.Drawing.Point(56, 164);
            this.OrientationList_CBX.Name = "OrientationList_CBX";
            this.OrientationList_CBX.Size = new System.Drawing.Size(121, 21);
            this.OrientationList_CBX.TabIndex = 16;
            this.OrientationList_CBX.Visible = false;
            this.OrientationList_CBX.SelectedValueChanged += new System.EventHandler(this.OrientationList_CBX_SelectedValueChanged);
            this.OrientationList_CBX.Enter += new System.EventHandler(this.OrientationList_CBX_Enter);
            this.OrientationList_CBX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OrientationList_CBX_KeyPress);
            this.OrientationList_CBX.Leave += new System.EventHandler(this.OrientationList_CBX_Leave);
            // 
            // PaperList_CBX
            // 
            this.PaperList_CBX.FormattingEnabled = true;
            this.PaperList_CBX.Location = new System.Drawing.Point(56, 137);
            this.PaperList_CBX.Name = "PaperList_CBX";
            this.PaperList_CBX.Size = new System.Drawing.Size(121, 21);
            this.PaperList_CBX.TabIndex = 15;
            this.PaperList_CBX.Visible = false;
            this.PaperList_CBX.SelectedValueChanged += new System.EventHandler(this.PaperList_CBX_SelectedValueChanged);
            this.PaperList_CBX.Enter += new System.EventHandler(this.PaperList_CBX_Enter);
            this.PaperList_CBX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PaperList_CBX_KeyPress);
            this.PaperList_CBX.Leave += new System.EventHandler(this.PaperList_CBX_Leave);
            // 
            // ScaleList_CBX
            // 
            this.ScaleList_CBX.FormattingEnabled = true;
            this.ScaleList_CBX.ItemHeight = 13;
            this.ScaleList_CBX.Location = new System.Drawing.Point(56, 110);
            this.ScaleList_CBX.Name = "ScaleList_CBX";
            this.ScaleList_CBX.Size = new System.Drawing.Size(121, 21);
            this.ScaleList_CBX.TabIndex = 14;
            this.ScaleList_CBX.Visible = false;
            this.ScaleList_CBX.SelectedValueChanged += new System.EventHandler(this.ScaleList_CBX_SelectedValueChanged);
            this.ScaleList_CBX.Enter += new System.EventHandler(this.ScaleList_CBX_Enter);
            this.ScaleList_CBX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ScaleList_CBX_KeyPress);
            this.ScaleList_CBX.Leave += new System.EventHandler(this.ScaleList_CBX_Leave);
            // 
            // PdfDocumentView_SyncPDFV
            // 
            this.PdfDocumentView_SyncPDFV.AutoScroll = true;
            this.PdfDocumentView_SyncPDFV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
            this.PdfDocumentView_SyncPDFV.Location = new System.Drawing.Point(438, 101);
            this.PdfDocumentView_SyncPDFV.Name = "PdfDocumentView_SyncPDFV";
            this.PdfDocumentView_SyncPDFV.ScrollDisplacementValue = 0;
            this.PdfDocumentView_SyncPDFV.Size = new System.Drawing.Size(484, 482);
            this.PdfDocumentView_SyncPDFV.TabIndex = 0;
            this.PdfDocumentView_SyncPDFV.ZoomMode = Syncfusion.Windows.Forms.PdfViewer.ZoomMode.Default;
            // 
            // myImageButton
            // 
            this.myImageButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("myImageButton.BackgroundImage")));
            this.myImageButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.myImageButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.myImageButton.Location = new System.Drawing.Point(216, 130);
            this.myImageButton.Margin = new System.Windows.Forms.Padding(0);
            this.myImageButton.Name = "myImageButton";
            this.myImageButton.Size = new System.Drawing.Size(55, 55);
            this.myImageButton.TabIndex = 17;
            this.myImageButton.Tag = "";
            this.myImageButton.UseVisualStyleBackColor = true;
            this.myImageButton.Visible = false;
            // 
            // MyListView_MLV
            // 
            this.MyListView_MLV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MyListView_MLV.CheckBoxes = true;
            this.MyListView_MLV.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.MyListView_MLV.FullRowSelect = true;
            this.MyListView_MLV.GridLines = true;
            this.MyListView_MLV.HideSelection = false;
            this.MyListView_MLV.Location = new System.Drawing.Point(12, 62);
            this.MyListView_MLV.Name = "MyListView_MLV";
            this.MyListView_MLV.Size = new System.Drawing.Size(387, 521);
            this.MyListView_MLV.TabIndex = 8;
            this.MyListView_MLV.UseCompatibleStateImageBehavior = false;
            this.MyListView_MLV.View = System.Windows.Forms.View.Details;
            this.MyListView_MLV.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.MyListView_MLV_ItemChecked);
            this.MyListView_MLV.SelectedIndexChanged += new System.EventHandler(this.MyListView_MLV_SelectedIndexChanged);
            this.MyListView_MLV.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MyListView_MLV_MouseDown);
            this.MyListView_MLV.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MyListView_MLV_MouseUp);
            // 
            // GenerateViewDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(984, 761);
            this.Controls.Add(this.ScaleList_CBX);
            this.Controls.Add(this.PdfDocumentView_SyncPDFV);
            this.Controls.Add(this.myImageButton);
            this.Controls.Add(this.OrientationList_CBX);
            this.Controls.Add(this.iconBar_TSP);
            this.Controls.Add(this.PaperList_CBX);
            this.Controls.Add(this.MyListView_MLV);
            this.KeyPreview = true;
            this.Name = "GenerateViewDialogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GenerateViewDialogForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GenerateViewDialogForm_FormClosed);
            this.Load += new System.EventHandler(this.GenerateViewDialogForm_Load);
            this.Shown += new System.EventHandler(this.GenerateViewDialogForm_Shown);
            this.iconBar_TSP.ResumeLayout(false);
            this.iconBar_TSP.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip iconBar_TSP;
        private System.Windows.Forms.ToolStripButton print_BTN;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton transferSM2_BTN;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton openFolder_BTN;
        private MyListView MyListView_MLV;
        private MyImageButton myImageButton;
        private System.Windows.Forms.ComboBox OrientationList_CBX;
        private System.Windows.Forms.ComboBox PaperList_CBX;
        public System.Windows.Forms.ComboBox ScaleList_CBX;
        private Syncfusion.Windows.Forms.PdfViewer.PdfDocumentView PdfDocumentView_SyncPDFV;
    }
}