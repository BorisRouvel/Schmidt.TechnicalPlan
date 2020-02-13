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
            Syncfusion.Windows.Forms.PdfViewer.MessageBoxSettings messageBoxSettings1 = new Syncfusion.Windows.Forms.PdfViewer.MessageBoxSettings();
            Syncfusion.Windows.PdfViewer.PdfViewerPrinterSettings pdfViewerPrinterSettings1 = new Syncfusion.Windows.PdfViewer.PdfViewerPrinterSettings();
            Syncfusion.Windows.Forms.PdfViewer.TextSearchSettings textSearchSettings1 = new Syncfusion.Windows.Forms.PdfViewer.TextSearchSettings();
            this.iconBar_TSP = new System.Windows.Forms.ToolStrip();
            this.transferSM2_BTN = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.print_BTN = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.openFolder_BTN = new System.Windows.Forms.ToolStripButton();
            this.orientationList_CBX = new System.Windows.Forms.ComboBox();
            this.paperList_CBX = new System.Windows.Forms.ComboBox();
            this.scaleList_CBX = new System.Windows.Forms.ComboBox();
            this.splitContainer_SPC = new System.Windows.Forms.SplitContainer();
            this.myImageButton = new Schmidt.TechnicalPlan.MyImageButton();
            this.myListView_MLV = new Schmidt.TechnicalPlan.MyListView();
            this.pdfDocumentView_PDFV = new Syncfusion.Windows.Forms.PdfViewer.PdfDocumentView();
            this.Ok_BTN = new System.Windows.Forms.Button();
            this.Cancel_BTN = new System.Windows.Forms.Button();
            this.selectAll_CHB = new System.Windows.Forms.CheckBox();
            this.generikPaperList_CBX = new System.Windows.Forms.ComboBox();
            this.paper_LAB = new System.Windows.Forms.Label();
            this.InitCustomInfo_BTN = new System.Windows.Forms.Button();
            this.iconBar_TSP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_SPC)).BeginInit();
            this.splitContainer_SPC.Panel1.SuspendLayout();
            this.splitContainer_SPC.Panel2.SuspendLayout();
            this.splitContainer_SPC.SuspendLayout();
            this.SuspendLayout();
            // 
            // iconBar_TSP
            // 
            this.iconBar_TSP.AutoSize = false;
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
            this.transferSM2_BTN.Image = global::Schmidt.TechnicalPlan.Properties.Resources.SM2;
            this.transferSM2_BTN.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.transferSM2_BTN.Name = "transferSM2_BTN";
            this.transferSM2_BTN.Size = new System.Drawing.Size(32, 32);
            this.transferSM2_BTN.Click += new System.EventHandler(this.transferSM2_BTN_Click);
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
            this.print_BTN.Tag = "1";
            this.print_BTN.Click += new System.EventHandler(this.print_BTN_Click);
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
            this.openFolder_BTN.Tag = "1";
            this.openFolder_BTN.Click += new System.EventHandler(this.openFolder_BTN_Click);
            // 
            // orientationList_CBX
            // 
            this.orientationList_CBX.FormattingEnabled = true;
            this.orientationList_CBX.Location = new System.Drawing.Point(42, 96);
            this.orientationList_CBX.Name = "orientationList_CBX";
            this.orientationList_CBX.Size = new System.Drawing.Size(121, 21);
            this.orientationList_CBX.TabIndex = 16;
            this.orientationList_CBX.Visible = false;
            this.orientationList_CBX.SelectedValueChanged += new System.EventHandler(this.orientationList_CBX_SelectedValueChanged);
            this.orientationList_CBX.Enter += new System.EventHandler(this.orientationList_CBX_Enter);
            this.orientationList_CBX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.orientationList_CBX_KeyPress);
            this.orientationList_CBX.Leave += new System.EventHandler(this.orientationList_CBX_Leave);
            // 
            // paperList_CBX
            // 
            this.paperList_CBX.FormattingEnabled = true;
            this.paperList_CBX.Location = new System.Drawing.Point(42, 69);
            this.paperList_CBX.Name = "paperList_CBX";
            this.paperList_CBX.Size = new System.Drawing.Size(121, 21);
            this.paperList_CBX.TabIndex = 15;
            this.paperList_CBX.Visible = false;
            this.paperList_CBX.SelectedValueChanged += new System.EventHandler(this.paperList_CBX_SelectedValueChanged);
            this.paperList_CBX.Enter += new System.EventHandler(this.paperList_CBX_Enter);
            this.paperList_CBX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.paperList_CBX_KeyPress);
            this.paperList_CBX.Leave += new System.EventHandler(this.paperList_CBX_Leave);
            // 
            // scaleList_CBX
            // 
            this.scaleList_CBX.FormattingEnabled = true;
            this.scaleList_CBX.ItemHeight = 13;
            this.scaleList_CBX.Location = new System.Drawing.Point(42, 42);
            this.scaleList_CBX.Name = "scaleList_CBX";
            this.scaleList_CBX.Size = new System.Drawing.Size(121, 21);
            this.scaleList_CBX.TabIndex = 14;
            this.scaleList_CBX.Visible = false;
            this.scaleList_CBX.SelectedValueChanged += new System.EventHandler(this.scaleList_CBX_SelectedValueChanged);
            this.scaleList_CBX.Enter += new System.EventHandler(this.scaleList_CBX_Enter);
            this.scaleList_CBX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.scaleList_CBX_KeyPress);
            this.scaleList_CBX.Leave += new System.EventHandler(this.scaleList_CBX_Leave);
            // 
            // splitContainer_SPC
            // 
            this.splitContainer_SPC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer_SPC.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer_SPC.Location = new System.Drawing.Point(12, 72);
            this.splitContainer_SPC.Name = "splitContainer_SPC";
            // 
            // splitContainer_SPC.Panel1
            // 
            this.splitContainer_SPC.Panel1.Controls.Add(this.scaleList_CBX);
            this.splitContainer_SPC.Panel1.Controls.Add(this.myImageButton);
            this.splitContainer_SPC.Panel1.Controls.Add(this.paperList_CBX);
            this.splitContainer_SPC.Panel1.Controls.Add(this.orientationList_CBX);
            this.splitContainer_SPC.Panel1.Controls.Add(this.myListView_MLV);
            // 
            // splitContainer_SPC.Panel2
            // 
            this.splitContainer_SPC.Panel2.Controls.Add(this.pdfDocumentView_PDFV);
            this.splitContainer_SPC.Size = new System.Drawing.Size(960, 637);
            this.splitContainer_SPC.SplitterDistance = 358;
            this.splitContainer_SPC.TabIndex = 18;
            // 
            // myImageButton
            // 
            this.myImageButton.BackColor = System.Drawing.SystemColors.WindowText;
            this.myImageButton.BackgroundImage = global::Schmidt.TechnicalPlan.Properties.Resources.twin3;
            this.myImageButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.myImageButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.myImageButton.Location = new System.Drawing.Point(202, 62);
            this.myImageButton.Margin = new System.Windows.Forms.Padding(0);
            this.myImageButton.Name = "myImageButton";
            this.myImageButton.Size = new System.Drawing.Size(52, 55);
            this.myImageButton.TabIndex = 17;
            this.myImageButton.Tag = "";
            this.myImageButton.UseVisualStyleBackColor = false;
            this.myImageButton.Visible = false;
            // 
            // myListView_MLV
            // 
            this.myListView_MLV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myListView_MLV.CheckBoxes = true;
            this.myListView_MLV.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.myListView_MLV.FullRowSelect = true;
            this.myListView_MLV.GridLines = true;
            this.myListView_MLV.HideSelection = false;
            this.myListView_MLV.Location = new System.Drawing.Point(3, 3);
            this.myListView_MLV.Name = "myListView_MLV";
            this.myListView_MLV.Size = new System.Drawing.Size(349, 631);
            this.myListView_MLV.TabIndex = 8;
            this.myListView_MLV.UseCompatibleStateImageBehavior = false;
            this.myListView_MLV.View = System.Windows.Forms.View.Details;
            this.myListView_MLV.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.myListView_MLV_ColumnWidthChanging);
            this.myListView_MLV.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.MyListView_MLV_ItemChecked);
            this.myListView_MLV.SelectedIndexChanged += new System.EventHandler(this.MyListView_MLV_SelectedIndexChanged);
            this.myListView_MLV.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MyListView_MLV_MouseDown);
            this.myListView_MLV.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MyListView_MLV_MouseUp);
            // 
            // pdfDocumentView_PDFV
            // 
            this.pdfDocumentView_PDFV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pdfDocumentView_PDFV.AutoScroll = true;
            this.pdfDocumentView_PDFV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
            this.pdfDocumentView_PDFV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pdfDocumentView_PDFV.CursorMode = Syncfusion.Windows.Forms.PdfViewer.PdfViewerCursorMode.SelectTool;
            this.pdfDocumentView_PDFV.EnableContextMenu = true;
            this.pdfDocumentView_PDFV.HorizontalScrollOffset = 0;
            this.pdfDocumentView_PDFV.IsTextSearchEnabled = true;
            this.pdfDocumentView_PDFV.IsTextSelectionEnabled = true;
            this.pdfDocumentView_PDFV.Location = new System.Drawing.Point(3, 3);
            messageBoxSettings1.EnableNotification = true;
            this.pdfDocumentView_PDFV.MessageBoxSettings = messageBoxSettings1;
            this.pdfDocumentView_PDFV.MinimumZoomPercentage = 50;
            this.pdfDocumentView_PDFV.Name = "pdfDocumentView_PDFV";
            this.pdfDocumentView_PDFV.PageBorderThickness = 1;
            pdfViewerPrinterSettings1.PageOrientation = Syncfusion.Windows.PdfViewer.PdfViewerPrintOrientation.Auto;
            pdfViewerPrinterSettings1.PageSize = Syncfusion.Windows.PdfViewer.PdfViewerPrintSize.ActualSize;
            pdfViewerPrinterSettings1.PrintLocation = ((System.Drawing.PointF)(resources.GetObject("pdfViewerPrinterSettings1.PrintLocation")));
            pdfViewerPrinterSettings1.ShowPrintStatusDialog = true;
            this.pdfDocumentView_PDFV.PrinterSettings = pdfViewerPrinterSettings1;
            this.pdfDocumentView_PDFV.ReferencePath = null;
            this.pdfDocumentView_PDFV.ScrollDisplacementValue = 0;
            this.pdfDocumentView_PDFV.ShowHorizontalScrollBar = true;
            this.pdfDocumentView_PDFV.ShowVerticalScrollBar = true;
            this.pdfDocumentView_PDFV.Size = new System.Drawing.Size(592, 631);
            this.pdfDocumentView_PDFV.SpaceBetweenPages = 8;
            this.pdfDocumentView_PDFV.TabIndex = 0;
            textSearchSettings1.CurrentInstanceColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(255)))), ((int)(((byte)(171)))), ((int)(((byte)(64)))));
            textSearchSettings1.HighlightAllInstance = true;
            textSearchSettings1.OtherInstanceColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.pdfDocumentView_PDFV.TextSearchSettings = textSearchSettings1;
            this.pdfDocumentView_PDFV.ThemeName = "Default";
            this.pdfDocumentView_PDFV.VerticalScrollOffset = 0;
            this.pdfDocumentView_PDFV.VisualStyle = Syncfusion.Windows.Forms.PdfViewer.VisualStyle.Default;
            this.pdfDocumentView_PDFV.ZoomMode = Syncfusion.Windows.Forms.PdfViewer.ZoomMode.FitPage;
            this.pdfDocumentView_PDFV.ZoomChanged += new Syncfusion.Windows.Forms.PdfViewer.PdfDocumentView.ZoomChangedEventHandler(this.pdfDocumentView_PDFV_ZoomChanged);
            // 
            // Ok_BTN
            // 
            this.Ok_BTN.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Ok_BTN.Location = new System.Drawing.Point(412, 726);
            this.Ok_BTN.Name = "Ok_BTN";
            this.Ok_BTN.Size = new System.Drawing.Size(75, 23);
            this.Ok_BTN.TabIndex = 19;
            this.Ok_BTN.Text = "Ok";
            this.Ok_BTN.UseVisualStyleBackColor = true;
            this.Ok_BTN.Click += new System.EventHandler(this.Ok_BTN_Click);
            // 
            // Cancel_BTN
            // 
            this.Cancel_BTN.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Cancel_BTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_BTN.Location = new System.Drawing.Point(493, 726);
            this.Cancel_BTN.Name = "Cancel_BTN";
            this.Cancel_BTN.Size = new System.Drawing.Size(75, 23);
            this.Cancel_BTN.TabIndex = 20;
            this.Cancel_BTN.Text = "Cancel";
            this.Cancel_BTN.UseVisualStyleBackColor = true;
            this.Cancel_BTN.Click += new System.EventHandler(this.Cancel_BTN_Click);
            // 
            // selectAll_CHB
            // 
            this.selectAll_CHB.AutoSize = true;
            this.selectAll_CHB.Location = new System.Drawing.Point(24, 49);
            this.selectAll_CHB.Name = "selectAll_CHB";
            this.selectAll_CHB.Size = new System.Drawing.Size(108, 17);
            this.selectAll_CHB.TabIndex = 21;
            this.selectAll_CHB.Text = "Tout sélectionner";
            this.selectAll_CHB.UseVisualStyleBackColor = true;
            this.selectAll_CHB.CheckedChanged += new System.EventHandler(this.selectAll_CHB_CheckedChanged);
            // 
            // generikPaperList_CBX
            // 
            this.generikPaperList_CBX.FormattingEnabled = true;
            this.generikPaperList_CBX.Location = new System.Drawing.Point(258, 47);
            this.generikPaperList_CBX.Name = "generikPaperList_CBX";
            this.generikPaperList_CBX.Size = new System.Drawing.Size(84, 21);
            this.generikPaperList_CBX.TabIndex = 22;
            this.generikPaperList_CBX.SelectedIndexChanged += new System.EventHandler(this.generikPaperList_CBX_SelectedIndexChanged);
            this.generikPaperList_CBX.TextChanged += new System.EventHandler(this.generikPaperList_CBX_TextChanged);
            // 
            // paper_LAB
            // 
            this.paper_LAB.AutoSize = true;
            this.paper_LAB.Location = new System.Drawing.Point(180, 50);
            this.paper_LAB.Name = "paper_LAB";
            this.paper_LAB.Size = new System.Drawing.Size(72, 13);
            this.paper_LAB.TabIndex = 23;
            this.paper_LAB.Text = "Format Papier";
            // 
            // InitCustomInfo_BTN
            // 
            this.InitCustomInfo_BTN.Enabled = false;
            this.InitCustomInfo_BTN.Location = new System.Drawing.Point(611, 38);
            this.InitCustomInfo_BTN.Name = "InitCustomInfo_BTN";
            this.InitCustomInfo_BTN.Size = new System.Drawing.Size(122, 23);
            this.InitCustomInfo_BTN.TabIndex = 24;
            this.InitCustomInfo_BTN.Text = "Init CustomInfo XML";
            this.InitCustomInfo_BTN.UseVisualStyleBackColor = true;
            this.InitCustomInfo_BTN.Visible = false;
            this.InitCustomInfo_BTN.Click += new System.EventHandler(this.InitCustomInfo_BTN_Click);
            // 
            // GenerateViewDialogForm
            // 
            this.AcceptButton = this.Ok_BTN;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.Cancel_BTN;
            this.ClientSize = new System.Drawing.Size(984, 761);
            this.Controls.Add(this.InitCustomInfo_BTN);
            this.Controls.Add(this.paper_LAB);
            this.Controls.Add(this.generikPaperList_CBX);
            this.Controls.Add(this.selectAll_CHB);
            this.Controls.Add(this.Cancel_BTN);
            this.Controls.Add(this.Ok_BTN);
            this.Controls.Add(this.splitContainer_SPC);
            this.Controls.Add(this.iconBar_TSP);
            this.KeyPreview = true;
            this.Name = "GenerateViewDialogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GenerateViewDialogForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GenerateViewDialogForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GenerateViewDialogForm_FormClosed);
            this.Load += new System.EventHandler(this.GenerateViewDialogForm_Load);
            this.Shown += new System.EventHandler(this.GenerateViewDialogForm_Shown);
            this.iconBar_TSP.ResumeLayout(false);
            this.iconBar_TSP.PerformLayout();
            this.splitContainer_SPC.Panel1.ResumeLayout(false);
            this.splitContainer_SPC.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_SPC)).EndInit();
            this.splitContainer_SPC.ResumeLayout(false);
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
        private MyListView myListView_MLV;
        private MyImageButton myImageButton;
        private System.Windows.Forms.ComboBox orientationList_CBX;
        private System.Windows.Forms.ComboBox paperList_CBX;
        public System.Windows.Forms.ComboBox scaleList_CBX;
        private System.Windows.Forms.SplitContainer splitContainer_SPC;
        private Syncfusion.Windows.Forms.PdfViewer.PdfDocumentView pdfDocumentView_PDFV;
        private System.Windows.Forms.Button Ok_BTN;
        private System.Windows.Forms.Button Cancel_BTN;
        private System.Windows.Forms.CheckBox selectAll_CHB;
        private System.Windows.Forms.ComboBox generikPaperList_CBX;
        private System.Windows.Forms.Label paper_LAB;
        private System.Windows.Forms.Button InitCustomInfo_BTN;
    }
}