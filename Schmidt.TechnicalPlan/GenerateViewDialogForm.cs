using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.IO;


namespace Schmidt.TechnicalPlan
{

    public partial class GenerateViewDialogForm : Form
    {
        public static string generikPaperSizeText = System.Printing.PageMediaSizeName.ISOA4.ToString();
        private Plugin _plugin;      
        Syncfusion.Pdf.Parsing.PdfLoadedDocument _blank = null;
        string _blankFilePath = String.Empty;
        private string _currentPdfFilePath;
        private string _archiveFileDir = String.Empty;
        private static string _currentScaleFactor;
        private static string _currentPageMediaSizeName;
        private static string _currentPageOrientation;

        private List<TechnicalDocument> _documentList = new List<TechnicalDocument>();
        private TechnicalPlanDocumentFileNameForm _technicalPlanDocumentFileNameForm = null;
        private KD.Plugin.Word.Plugin _pluginWord = null;
        private Dico _dico = null;
        private TechnicalDocument _technicalDocument = null;
        private Syncfusion.Pdf.Parsing.PdfLoadedDocument _loadedDocument = null;

        private ListViewItem lvItem = null;

        private const int InitialZoomValue = 99;

        private double _zoom = 99;

        private MyImageButton myOverViewButton = null;

        private int lviSelectedRowIndex = KD.Const.UnknownId;
        private int lviSelectedColumnIndex = (int)MyListView.Enum.ColumnIndex.UnKnown;

        string drawingSurface2D = String.Empty;

        public Plugin Plugin
        {
            get
            {
                return _plugin;
            }
            set
            {
                _plugin = value;
            }
        }        
        public Syncfusion.Pdf.Parsing.PdfLoadedDocument BlankPdf
        {
            get
            {
                return _blank;
            }
            set
            {
                _blank = value;
            }
        }
        public string BlankPdfPath
        {
            get
            {
                return _blankFilePath;
            }
            set
            {
                _blankFilePath = value;
            }
        }
        public string CurrentPdfFilePath
        {
            get
            {
                return _currentPdfFilePath;
            }
            set
            {
                _currentPdfFilePath = value;
            }
        }
        public string ArchiveFileDir
        {
            get
            {
                return _archiveFileDir;
            }
            set
            {
                _archiveFileDir = value;
            }
        }
        public static string CurrentScaleFactor
        {
            get { return _currentScaleFactor; }
            set { _currentScaleFactor = value; }
        }
        public static string CurrentPageMediaSizeName
        {
            get { return _currentPageMediaSizeName; }
            set { _currentPageMediaSizeName = value; }
        }
        public static string CurrentPageOrientation
        {
            get { return _currentPageOrientation; }
            set { _currentPageOrientation = value; }
        }
        


        //
        //Printer format setting
        double margeX = 20.0;
        double margeY = 55.0;

        private static System.Drawing.Printing.PaperKind ISOA4Portrait = System.Drawing.Printing.PaperKind.A4;
        private static System.Drawing.Printing.PaperKind ISOA4Landscape = System.Drawing.Printing.PaperKind.A4Rotated;
        private static System.Drawing.Printing.PaperKind ISOA3Portrait = System.Drawing.Printing.PaperKind.A3;
        private static System.Drawing.Printing.PaperKind ISOA3Landscape = System.Drawing.Printing.PaperKind.A3Rotated;

        //System.Drawing.Printing.PageSettings pageSettings = new PageSettings();
        //int toto = pageSettings.PaperSize.Width;

        System.Drawing.Printing.PaperSize paperSizeA4P = new System.Drawing.Printing.PaperSize(ISOA4Portrait.ToString(), 210, 297); // 1 pouce = 2.54 cm 420=1653, 297=1169
        System.Drawing.Printing.PaperSize paperSizeA4L = new System.Drawing.Printing.PaperSize(ISOA4Landscape.ToString(), 297, 210); // 1 pouce = 2.54 cm 420=1653, 297=1169
        System.Drawing.Printing.PaperSize paperSizeA3P = new System.Drawing.Printing.PaperSize(ISOA3Portrait.ToString(), 297, 420); // 1 pouce = 2.54 cm 420=1653, 297=1169
        System.Drawing.Printing.PaperSize paperSizeA3L = new System.Drawing.Printing.PaperSize(ISOA3Landscape.ToString(), 420, 297); // 1 pouce = 2.54 cm 420=1653, 297=1169

        //

        public GenerateViewDialogForm(Plugin plugin, KD.Plugin.Word.Plugin pluginWord, Dico dico, TechnicalDocument technicalDocument)//
        {
            InitializeComponent();

            _plugin = plugin;
            _pluginWord = pluginWord;
            _dico = dico;
            _technicalDocument = technicalDocument;
            
            this.InitializeForm();
            this.CreateConfigDirectories();           
        }

   
        private void InitializePreviewDirectory()
        {            
            _archiveFileDir = _pluginWord.Config.SceneDocDir;           

            if (System.IO.Directory.Exists(Plugin.PreviewDirectory))
            {
                IEnumerable<string> technicalPlanFiles = System.IO.Directory.EnumerateFiles(Plugin.PreviewDirectory);
                foreach (string technicalPlanFile in technicalPlanFiles)
                {
                    this.DeleteFileIfExist(technicalPlanFile);                   
                }
            }
        }
        private void InitializeListView()
        {
            this.myListView_MLV.Clear();
            // Add the pet to our listview
            //docListView.View = View.SmallIcon;
            this.myListView_MLV.View = View.Details;
            // Display check boxes.
            this.myListView_MLV.CheckBoxes = true;
            // Select the item and subitems when selection is made.
            this.myListView_MLV.FullRowSelect = true;
            // Display grid lines.
            this.myListView_MLV.GridLines = true;
            // Sort the items in the list in ascending order.
            this.myListView_MLV.Sorting = SortOrder.None;//.Ascending;

            Font stdfont = new Font("Microsoft Sans Serif", 10.0f, FontStyle.Regular);
            this.Font = stdfont;

            this.myListView_MLV.HideSelection = false;

            //
            string assemblyDirPath = KD.IO.Path.ExtractFileDir(Plugin.AssemblyFilePath);
            if (Directory.Exists(assemblyDirPath))
            {
                string imageDirPath = System.IO.Path.Combine(assemblyDirPath, "img");

                this.BlankPdfPath = System.IO.Path.Combine(imageDirPath, "blank.pdf");                
                if (this.BlankPdf == null && System.IO.File.Exists(this.BlankPdfPath))
                {
                    this.BlankPdf = new Syncfusion.Pdf.Parsing.PdfLoadedDocument(this.BlankPdfPath);
                }
            }

            //

            var header = new List<string>();

            header.Add(this._dico.GetTranslate(TranslateIdentifyId.ColumnHeaderSelect_ID));
            header.Add(this._dico.GetTranslate(TranslateIdentifyId.ColumnHeaderView_ID));
            header.Add(this._dico.GetTranslate(TranslateIdentifyId.ColumnHeaderScale_ID));
            header.Add(this._dico.GetTranslate(TranslateIdentifyId.ColumnHeaderPaper_ID));
            header.Add(this._dico.GetTranslate(TranslateIdentifyId.ColumnHeaderOrientation_ID));
            header.Add(this._dico.GetTranslate(TranslateIdentifyId.ColumnHeaderOverview_ID));
            header.ForEach(name => this.myListView_MLV.Columns.Add(name));

            // Loop through and size each column header to fit the column header text.
            foreach (ColumnHeader ch in this.myListView_MLV.Columns)
            {                
                ch.Width = -2;
                ch.TextAlign = HorizontalAlignment.Center;

                this.ColumnLeftAlign(ch);
                this.PaperColumnWidthToZero(ch);
            }
        }
        private void InitializeForm()
        {
            this.Text = _dico.GetTranslate(TranslateIdentifyId.UITitle_ID);
            this.Ok_BTN.Text = _dico.GetTranslate(TranslateIdentifyId.UIOkButton_ID);
            this.Cancel_BTN.Text = _dico.GetTranslate(TranslateIdentifyId.UICancelButton_ID);
            this.selectAll_CHB.Text = _dico.GetTranslate(TranslateIdentifyId.UISelectAllViewCheckBox_ID);
            this.paper_LAB.Text = _dico.GetTranslate(TranslateIdentifyId.ColumnHeaderPaper_ID);                      

            this.ToolStripButtonVisible(this.openFolder_BTN, Plugin.IsOpenFolderButtonVisible);
            this.ToolStripButtonVisible(this.print_BTN, Plugin.IsPrintButtonVisible); 
            
        }
        private void CreateConfigDirectories()
        {
            this.CreateDirIfNotExist(Plugin.SM2Directory);
            this.CreateDirIfNotExist(Plugin.PreviewDirectory);
        }

        private void ToolStripButtonVisible(ToolStripButton TSbutton, bool isVisible)
        {
            TSbutton.Tag = isVisible;      

            if (!(bool)TSbutton.Tag)
            {
                TSbutton.Visible = false;
            }
            else if ((bool)TSbutton.Tag)
            {
                TSbutton.Visible = true;
            }
        }
        private void PaperColumnWidthToZero(ColumnHeader ch)
        {
            if (ch.Index == (int)MyListView.Enum.ColumnIndex.Paper)
            {
                ch.Width = 0;
            }
        }
        private void ColumnLeftAlign(ColumnHeader ch)
        {
            if (ch.Index == (int)MyListView.Enum.ColumnIndex.View)
            {
                ch.TextAlign = HorizontalAlignment.Left;
            }
        }
        private void SizeAndPlaceControlInForm()
        {
            this.myListView_MLV.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            int columnHeaderSize = 0;
            foreach (ColumnHeader ch in this.myListView_MLV.Columns)
            {
                this.PaperColumnWidthToZero(ch);
                columnHeaderSize += ch.Width;
            }
            this.splitContainer_SPC.SplitterDistance = this.splitContainer_SPC.Location.X + columnHeaderSize + 2;

            int x = (this.splitContainer_SPC.SplitterDistance - this.generikPaperList_CBX.Width);
            int y = this.selectAll_CHB.Location.Y;
            this.generikPaperList_CBX.Location = new Point(x, y - 2);

            
            int x1 = (this.generikPaperList_CBX.Location.X - this.generikPaperList_CBX.Margin.Left - this.paper_LAB.Width - this.paper_LAB.Margin.Right); //this.splitContainer_SPC.Location.X
            this.paper_LAB.Location = new Point(x1, y + 1);

            this.myListView_MLV.Refresh();
        }

        private string GetTechnicalDocumentFileNameFromListViewItem(ListViewItem lvi)//int lvItemIndex
        { 
            //lvi.Tag = myListView_MLV.Items[lvItemIndex].Tag;
            TechnicalDocument technicalDocument = (TechnicalDocument)lvi.Tag;

            string technicalDocumentViewMode = this.GetViewMode((int)technicalDocument.ViewMode) + KD.StringTools.Const.MinusSign;

            string scale = GetScaleStringForDocPartition(technicalDocument.ScaleFactorValue) + KD.StringTools.Const.MinusSign;//lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Tag.ToString()
            string paper = technicalDocument.PageMediaSizeName.ToString() + KD.StringTools.Const.MinusSign;//lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Text
            string orientation = technicalDocument.PageOrientation.ToString(); // GetOrientationStringForDocPartition(lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Text

            return (technicalDocumentViewMode + scale + paper + orientation);           
        }
        public void Build(bool preview, ListViewItem lvi)//int lvItemIndex
        {
            this.DesactivatePdfViewer();

            Cursor.Current = Cursors.WaitCursor;
      
            TechnicalDocument technicalDocument = (TechnicalDocument)lvi.Tag;
            
            //System.Guid.NewGuid().ToString();
            technicalDocument.FileName = this.TranslateTechnicalDocumentFileName(technicalDocument); 

            // Optimize the generate doc, look at preview if exist
            //string sceneNameDir = Path.Combine(_plugin.CurrentAppli.ScenesDir, _plugin.CurrentAppli.SceneName);
            string previewPdfFilePath = Path.Combine(Plugin.PreviewDirectory, technicalDocument.FileName + KD.IO.File.Extension.Pdf);//sceneNameDir,ConstRessourceNames.DocTechnicalPlanDir, ConstRessourceNames.TechnicalPlanPreviewDirName
            string previewDotFilePath = Path.Combine(Plugin.PreviewDirectory, technicalDocument.FileName + KD.IO.File.Extension.Dot);//sceneNameDir, ConstRessourceNames.DocTechnicalPlanDir, ConstRessourceNames.TechnicalPlanPreviewDirName

            string SM2PdfFilePath = Path.Combine(Plugin.SM2Directory, technicalDocument.FileName + KD.IO.File.Extension.Pdf);//sceneNameDir, ConstRessourceNames.DocTechnicalPlanDir
            string SM2DotFilePath = Path.Combine(Plugin.SM2Directory, technicalDocument.FileName + KD.IO.File.Extension.Dot);//sceneNameDir, ConstRessourceNames.DocTechnicalPlanDir
            if (System.IO.File.Exists(previewDotFilePath) && !preview)
            {
                System.IO.File.Copy(previewDotFilePath, SM2DotFilePath);
            }

            if (System.IO.File.Exists(previewPdfFilePath) && !preview)
            {
                System.IO.File.Copy(previewPdfFilePath, SM2PdfFilePath);
            }
            else
            {
                //
                _pluginWord.DocIndex2Use = FindTemplateRankFromListView(lvi);//lvItemIndex

                if (_pluginWord.DocIndex2Use != KD.Const.UnknownId)
                {
                    _pluginWord.GenerateDocument(false);

                    string docname = _pluginWord.GetLocalizedDocName(_pluginWord.DocIndex2Use);
                    if (!String.IsNullOrEmpty(docname))
                    {
                        string pdfFilePath = _pluginWord.GetArchiveFilePath(docname, KD.IO.File.Extension.Pdf);
                        string dotFilePath = _pluginWord.GetArchiveFilePath(docname, KD.IO.File.Extension.Dot);

                        string idPdfFileName = technicalDocument.FileName + KD.IO.File.Extension.Pdf;
                        string idPdfFilePath = System.IO.Path.Combine(Plugin.SM2Directory, idPdfFileName);//System.IO.Path.GetDirectoryName(pdfFilePath)
                        if (System.IO.File.Exists(pdfFilePath) && !System.IO.File.Exists(idPdfFilePath))
                        {
                            System.IO.File.Copy(pdfFilePath, idPdfFilePath, true);
                        }
                        if (System.IO.File.Exists(idPdfFilePath))
                        {
                            this.DeleteFileIfExist(pdfFilePath);
                        }

                        string idDotFileName = technicalDocument.FileName + KD.IO.File.Extension.Dot;
                        string idDotFilePath = System.IO.Path.Combine(Plugin.SM2Directory, idDotFileName);//System.IO.Path.GetDirectoryName(dotFilePath)
                        if (System.IO.File.Exists(dotFilePath) && !System.IO.File.Exists(idDotFilePath))
                        {
                            System.IO.File.Copy(dotFilePath, idDotFilePath, true);
                            
                        }
                        if (System.IO.File.Exists(idDotFilePath))
                        {
                            this.DeleteFileIfExist(dotFilePath);
                        }

                        string movePdfPath = this.MoveFileToDocPlanDir(lvi.Index, idPdfFilePath, idDotFilePath, 0);//lvItemIndex
                        string moveDotPath = movePdfPath.Replace(KD.IO.File.Extension.Pdf, KD.IO.File.Extension.Dot);

                        //string guidFileName = _documentList[lviIndex].FileName + KD.IO.File.Extension.Pdf; // this.GetViewFileName(lviIndex, System.IO.Path.GetFileName(pdfFilePath));                
                        //string guidFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(dirPdfPath), guidFileName);

                        //System.IO.File.Copy(dirPdfPath, guidFilePath);
                        //System.IO.File.Delete(dirPdfPath);

                        if (preview)
                        {
                            this.LoadPdfFile(this.MoveFileToPreviewDir(movePdfPath, moveDotPath, 0));
                        }
                    }
                }
            }
            Cursor.Current = Cursors.Arrow;
        }
        private string MoveFileToDocPlanDir(int lviIndex, string pdfFilePath, string dotFilePath, int time)
        {          
            string currentDir = System.IO.Path.GetDirectoryName(pdfFilePath);
            string lastSubDir = KD.StringTools.Helper.SubStringAfterLast(currentDir, KD.StringTools.Const.BackSlatch);

            if (!currentDir.Contains(lastSubDir))//ConstRessourceNames.DocTechnicalPlanDir
            {
                string currentPdfDir = System.IO.Path.Combine(currentDir, lastSubDir);//ConstRessourceNames.DocTechnicalPlanDir

                this.CreateDirIfNotExist(currentPdfDir);

                string viewFilePath = _documentList[lviIndex].FileName + KD.IO.File.Extension.Pdf; // this.GetViewFileName(lviIndex, System.IO.Path.GetFileName(pdfFilePath));                

                string newPdfFilePath = System.IO.Path.Combine(currentPdfDir, viewFilePath); // System.IO.Path.GetFileName(pdfFilePath));
                string newDotFilePath = System.IO.Path.Combine(currentPdfDir, viewFilePath.Replace(KD.IO.File.Extension.Pdf, KD.IO.File.Extension.Dot));

                try
                {                   
                    if (System.IO.File.Exists(pdfFilePath))
                    {
                        System.IO.File.Copy(pdfFilePath, newPdfFilePath, true);
                        this.DeleteFileIfExist(pdfFilePath);
                    }
                    else
                    {
                        if (time <= 5)
                        {
                            MoveFileToDocPlanDir(lviIndex, pdfFilePath, dotFilePath, time++);
                        }
                        else
                        {
                            time = 0;
                            return pdfFilePath;
                        }
                    }
                    
                    if (System.IO.File.Exists(dotFilePath))
                    {
                        System.IO.File.Copy(dotFilePath, newDotFilePath, true);
                        this.DeleteFileIfExist(dotFilePath);
                    }

                    return newPdfFilePath;
                }
                catch (Exception)
                {
                    return pdfFilePath;
                }
            }
            return pdfFilePath;
        }
        private string MoveFileToPreviewDir(string pdfFilePath, string dotFilePath, int time)
        {
            string currentDir = System.IO.Path.GetDirectoryName(pdfFilePath);
            this.CreateDirIfNotExist(Plugin.PreviewDirectory);//System.IO.Path.Combine(currentDir, ConstRessourceNames.TechnicalPlanPreviewDirName)

            string newDir = Plugin.PreviewDirectory; // System.IO.Path.Combine(currentDir, ConstRessourceNames.TechnicalPlanPreviewDirName);
            string previewPdfFilePath = System.IO.Path.Combine(newDir, System.IO.Path.GetFileName(pdfFilePath));
            string previewDotFilePath = System.IO.Path.Combine(newDir, System.IO.Path.GetFileName(dotFilePath));

            try
            {
                if (System.IO.File.Exists(pdfFilePath))
                {
                    System.IO.File.Copy(pdfFilePath, previewPdfFilePath, true);
                    this.DeleteFileIfExist(pdfFilePath);
                }
                else
                {
                    if (time <= 5)
                    {
                        time++;
                        MoveFileToPreviewDir(pdfFilePath, dotFilePath, time);
                    }
                    else
                    {
                        time = 0;
                        return pdfFilePath;
                    }
                }     

                if (System.IO.File.Exists(dotFilePath))
                {
                    System.IO.File.Copy(dotFilePath, previewDotFilePath, true);
                    this.DeleteFileIfExist(dotFilePath);
                }

                return previewPdfFilePath;
            }
            catch (Exception)
            {
                return pdfFilePath;
            }
        }
        private void DeleteFileIfExist(string path)
        {
            if (System.IO.File.Exists(path))
            {
                try
                {
                    System.IO.File.Delete(path);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }
                
            }
        }
        private void CreateDirIfNotExist(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }

        public void LoadPdfFile(string pdfFilePath)
        {
            this.ActivatePdfViewer();

            if (pdfFilePath != this.CurrentPdfFilePath) // optimization (not loading same doc twice)
            {
                // Set cursor as hourglass
                Cursor.Current = Cursors.WaitCursor;

                if (_loadedDocument != null)
                {
                    _loadedDocument.Close();
                }
                if (System.IO.File.Exists(pdfFilePath))
                {
                    this._loadedDocument = new Syncfusion.Pdf.Parsing.PdfLoadedDocument(pdfFilePath);
                    pdfDocumentView_PDFV.Load(_loadedDocument); // @"D:\Ic90dev\Scenes\5DE66AB8_0226_01\DocTechnicalPlan\TECHNICAL_PLAN_PREVIEW\Elévation du mur.pdf"); 
               
                

                // first load fit page                
                //if (_zoom == InitialZoomValue)
                //{
                    pdfDocumentView_PDFV.ZoomMode = Syncfusion.Windows.Forms.PdfViewer.ZoomMode.FitPage;
                    //}
                    //else // take last zoom value                 
                    //{
                    //    pdfDocumentView_PDFV.ZoomTo(Convert.ToInt32(_zoom));
                    //}
                    // can Update member only after load
                    _currentPdfFilePath = pdfFilePath;
                }
                else
                {
                    this.DesactivatePdfViewer();
                }
                // Set cursor as default arrow
                Cursor.Current = Cursors.Default;
            }

           

        }
        private void CreateAndLoadPdfLoadedDocument(String filepath)
        {
            if (System.IO.File.Exists(filepath))
            {
                // If there is any loaded doc here, let's release it
                if (_loadedDocument != null)
                {
                    _loadedDocument.Close();
                }

                this._loadedDocument = new Syncfusion.Pdf.Parsing.PdfLoadedDocument(filepath);
                this.pdfDocumentView_PDFV.Load(_loadedDocument);
                this.CurrentPdfFilePath = filepath;
            }            
        }

        private void DesactivatePdfViewer()
        {
            try
            {
                //this.pdfDocumentView1.Unload();
                // problem with unload, lets close loaded document
                // otherwise we can,'t Delete file with System.IO.Delete
                // Releasing data is compulsory
                if (_loadedDocument != null)
                {                   
                    _loadedDocument.Close();
                }
                this._loadedDocument = new Syncfusion.Pdf.Parsing.PdfLoadedDocument(this.BlankPdfPath);
                this.pdfDocumentView_PDFV.Load(_loadedDocument);
                this.CurrentPdfFilePath = this.BlankPdfPath;

                //// need to update CurrentPdfFilePath now for later use.
                //// BUt can't use LoadPdfFile coz it can call DesactivatePdfViewer and makes infinite loop
                //this.pdfDocumentView_PDFV.Load(String.Empty);
                //this.CurrentPdfFilePath = String.Empty;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

           this.pdfDocumentView_PDFV.Enabled = false;
            this.pdfDocumentView_PDFV.Visible = false;
            return;
        }
        private void ActivatePdfViewer()
        {
            this.splitContainer_SPC.Panel2.Show();
            this.pdfDocumentView_PDFV.Enabled = true;
            this.pdfDocumentView_PDFV.Visible = true;
            return;
        }
        private int FindTemplateRankFromListView(ListViewItem lvi)//int lvItemIndex
        {           
            string technicalDocumentName = this.GetTechnicalDocumentFileNameFromListViewItem(lvi);// technicalDocumentViewMode + scale + paper + orientation;                                                           

            int nbSubDoc = _pluginWord.CurrentAppli.GetDocsNb();

            for (int iSubDoc = 0; iSubDoc <= nbSubDoc; iSubDoc++)
            {                      
                string templateFileName = _pluginWord.CurrentAppli.DocGetInfo(iSubDoc, KD.SDK.AppliEnum.DocInfo.NAME);

                if (templateFileName.ToUpper().Equals(technicalDocumentName.ToUpper()))
                {                           
                    return iSubDoc;                           
                }
            }
            return KD.Const.UnknownId;
        }
        private string GetViewMode(int viewMode)
        {
            if (viewMode == (int)KD.SDK.SceneEnum.ViewMode.TOP)
            {
                return ConstRessourceNames.TopViewFileNameHeader;
            }
            else if (viewMode == (int)KD.SDK.SceneEnum.ViewMode.VECTELEVATION)
            {
                return ConstRessourceNames.ElevationViewFileNameHeader;
            }
            return KD.SDK.SceneEnum.ViewMode.UNKNOWN.ToString();
        }
        private string TranslateTechnicalDocumentFileName(TechnicalDocument technicalDocument)
        {                    
            return technicalDocument.ViewName.Replace(KD.StringTools.Const.WhiteSpace, String.Empty) + KD.StringTools.Const.MinusSign + 
                    GetScaleStringForDocPartition(technicalDocument.ScaleFactorValue) + KD.StringTools.Const.MinusSign +
                    technicalDocument.PageMediaSizeName.ToString() + KD.StringTools.Const.MinusSign +
                    PageOrientationSubItem.Dico[technicalDocument.PageOrientationID];
        }
        private string GetScaleStringForDocPartition(double scale)
        {
            if (scale == SubItemsConst.scaleFactor1_20) //"0.05"
            {
                return SubItemsConst.scaleFactorString1_20;
            }
            else if (scale == SubItemsConst.scaleFactor1_25) //"0.04"
            {
                return SubItemsConst.scaleFactorString1_25;
            }
            else if (scale == SubItemsConst.scaleFactor1_50) //"0.02"
            {
                return SubItemsConst.scaleFactorString1_50;
            }
            else if (scale == SubItemsConst.scaleFactorAuto) //"Auto"
            {
                return SubItemsConst.scaleFactorStringAuto;
            }               
           
            return String.Empty; ;
        }
        private string GetOrientationStringForDocPartition(string orientation)
        {
            PageOrientationSubItem pageOrientation = new PageOrientationSubItem(orientation);
            string result = pageOrientation.ToPageOrientation().ToString();
            return result;
        }

        private int GetSelectedRowIndex()
        {
            if (lvItem != null)
            {
                return lvItem.Index;
            }
            return KD.Const.UnknownId;
        }
        private int GetSelectColumnIndex(int x)
        {
            if (lviSelectedRowIndex != KD.Const.UnknownId)
            {
                foreach (ColumnHeader column in this.myListView_MLV.Columns)
                {
                    if (column.Index != (int)MyListView.Enum.ColumnIndex.Select && column.Index != (int)MyListView.Enum.ColumnIndex.Overview)
                    {
                        int columnPosition = this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[column.Index].Bounds.X;
                        int columnWidth = this.myListView_MLV.Columns[column.Index].Width;

                        if (columnPosition <= x && x <= (columnPosition + columnWidth))
                        {
                            return column.Index;
                        }
                    }
                }
            }
            return (int)MyListView.Enum.ColumnIndex.UnKnown;
        }

        private void KeyPressEvent(KeyPressEventArgs keyPressEvent, ComboBox comboBox)
        {
            switch (keyPressEvent.KeyChar)
            {
                case (char)(int)Keys.Escape:
                    {
                        // Reset the original text value, and then hide the ComboBox.
                        //= comboBox.Items[comboBox.SelectedIndex].ToString();                       
                        comboBox.Visible = false;
                        break;
                    }

                case (char)(int)Keys.Enter:
                    {
                        // Hide the ComboBox.
                        comboBox.Visible = false;
                        break;
                    }
            }
        }
        private void HideComboBox(ComboBox comboBox)
        {
            comboBox.Visible = false;
        }
        private void SetComboBoxTextInItem(ComboBox comboBox)
        {           
            this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[lviSelectedColumnIndex].Text = comboBox.Text;
        }
        private void SetItemTextInComboBox(ComboBox comboBox)
        {            
            comboBox.Text = this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[lviSelectedColumnIndex].Text;            
        }
        private void SetItemTagInComboBox(ComboBox comboBox)
        {
            comboBox.Tag = new PageOrientationSubItem(comboBox.SelectedItem.ToString()).ToPageOrientation(); // this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[lviSelectedColumnIndex].Tag;
        }
    
        private void LoadCustomInfoOrCalculateBestFormat()
        {      
            string xmlCustomInfo = _pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(TechnicalDocument.technicalDocumentsTag);
            _technicalDocument.ReadFromXml(xmlCustomInfo, out List<TechnicalDocument> customInfoDocumentList);

            if (customInfoDocumentList.Count > 0)
            {               
                generikPaperSizeText = customInfoDocumentList[0].PageMediaSizeName.ToString();

                foreach (ListViewItem lvi in myListView_MLV.Items)
                {
                    TechnicalDocument technicalDocument = (TechnicalDocument)lvi.Tag;
                    TechnicalDocument customInfoTechnicalDocument = customInfoDocumentList[0];                                                                

                    if (technicalDocument.ObjectID == customInfoTechnicalDocument.ObjectID)
                    {
                        this.UpdateListViewItemFromDocument(customInfoTechnicalDocument, lvi);                         
                        customInfoDocumentList.RemoveAt(0);
                    }                       
                    else
                    {
                        this.CalculateBestFormat(lvi, false);
                    }                  
                }
               
                this.SetGenerikPaperCombobox();
                myListView_MLV.Refresh();
                this.UpdateDocumentListFromListView();
            }
            else
            {
                foreach (ListViewItem lvi in myListView_MLV.Items)
                {
                    this.CalculateBestFormat(lvi, true);
                }
                this.SetGenerikPaperCombobox();
                myListView_MLV.Refresh();
                this.UpdateDocumentListFromListView();
            }
        }    
        private void SaveCustomInfo()
        {            
            _technicalDocument.WriteToXml(this._documentList, out XmlNode xmlNode);
            string xmlCustomInfo = xmlNode.OuterXml;
            
            _pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(xmlCustomInfo, TechnicalDocument.technicalDocumentsTag);        

            #region Info
            //< Vues >
            //< Vue >
            //< NomFichier > toto.pdf </ NomFichier > => {ViewMode}_{Number}_{ObjectId}.pdf
            //< Nom > Vue de dessus de toto</ Nom >
            //< Type > Dessus </ Type >
            //< Format > A3 </ Format >
            //< Date > 20191217100254 </ Date >
            //</ Vue >
            //</ Vues >

            //< Vues >
            //< Vue >
            //< NomFichier > toto.pdf </ NomFichier >
            //< Nom > Vue d'élévation du mur 1</Nom>
            //< Type > Elevation </ Type >
            //< Format > A4 </ Format >
            //< Date > 20191217100454 </ Date >
            //</ Vue >
            //</ Vues >

            //< Vues >
            //< Vue >
            //< NomFichier > toto.pdf </ NomFichier >
            //< Nom > Vue d'élévation du mur 2</Nom>
            //< Type > Elevation </ Type >
            //< Format > A4 </ Format >
            //< Date > 20191217100654 </ Date >
            //</ Vue >
            //</ Vues >
            //
            #endregion
        }
        private void CalculateBestFormat(ListViewItem lvi, bool isFormat)
        {
            double paperA4PWidth = paperSizeA4P.Width - margeX; //margin 10 + 10
            double paperA4PHeight = paperSizeA4P.Height - margeY; // first line is line for drawing (297 - 67 = 230)

            double paperA4LWidth = paperSizeA4L.Width - margeX; //margin 10 + 10
            double paperA4LHeight = paperSizeA4L.Height - margeY; // first line is line for drawing (297 - 67 = 230)

            double paperA3PWidth = paperSizeA3P.Width - margeX; //margin 10 + 10
            double paperA3PHeight = paperSizeA3P.Height - margeY; // first line is line for drawing (297 - 67 = 230)

            double paperA3LWidth = paperSizeA3L.Width - margeX; //margin 10 + 10
            double paperA3LHeight = paperSizeA3L.Height - margeY; // first line is line for drawing (297 - 67 = 230)
                      
            List<int> elevationIds = new List<int>();
            List<int> wallIds = this.GetWallIDs();
            List<int> symbolIds = this.GetElevSymbolIDs();

            //elevationIds.Add(KD.Const.UnknownId); //id for the top view (-1)
            elevationIds.AddRange(wallIds);
            elevationIds.AddRange(symbolIds);

            double sceneDimensionX = System.Convert.ToDouble(_plugin.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DIMX));
            double sceneDimensionY = System.Convert.ToDouble(_plugin.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DIMY));

            Dictionary<int, double> elevationDimensionXDict = new Dictionary<int, double>();
            Dictionary<int, double> elevationDimensionYDict = new Dictionary<int, double>();
            for (int ids = 0; ids < elevationIds.Count; ids++)
            {
                elevationDimensionXDict.Add(elevationIds[ids], System.Convert.ToDouble(_plugin.CurrentAppli.Scene.ObjectGetInfo(elevationIds[ids], KD.SDK.SceneEnum.ObjectInfo.DIMX)));
                elevationDimensionYDict.Add(elevationIds[ids], System.Convert.ToDouble(_plugin.CurrentAppli.Scene.ObjectGetInfo(elevationIds[ids], KD.SDK.SceneEnum.ObjectInfo.DIMZ)));
            }
                      
            double dimensionX = -1.0;
            double dimensionY = -1.0;

            double proportionX_1_20 = 0;
            double proportionY_1_20 = 0;
            double proportionX_1_25 = 0;
            double proportionY_1_25 = 0;
            double proportionX_1_50 = 0;
            double proportionY_1_50 = 0;

            if (lvi.Index == 0)
            {
                dimensionX = sceneDimensionX;
                dimensionY = sceneDimensionY;                   
            }
            else if (lvi.Index > 0)
            {
                TechnicalDocument infoTag = (TechnicalDocument)lvi.Tag;
                int id = infoTag.ObjectID;                    

                foreach (KeyValuePair<int, double> kvp in elevationDimensionXDict)
                {
                    if (kvp.Key == id)
                    {
                        dimensionX = kvp.Value;
                    }
                }
                foreach (KeyValuePair<int, double> kvp in elevationDimensionYDict)
                {
                    if (kvp.Key == id)
                    {
                        dimensionY = kvp.Value;
                    }
                }
                    
            }

            if (dimensionX != -1.0 && dimensionY != -1.0)
            {
                proportionX_1_20 = dimensionX * SubItemsConst.scaleFactor1_20; // 0.05
                proportionY_1_20 = dimensionY * SubItemsConst.scaleFactor1_20; // 0.05
                proportionX_1_25 = dimensionX * SubItemsConst.scaleFactor1_25; // 0.04
                proportionY_1_25 = dimensionY * SubItemsConst.scaleFactor1_25; // 0.04
                proportionX_1_50 = dimensionX * SubItemsConst.scaleFactor1_50; // 0.02
                proportionY_1_50 = dimensionY * SubItemsConst.scaleFactor1_50; // 0.02

                // 1_20
                if (paperA3LWidth >= proportionX_1_20 && paperA3LHeight >= proportionY_1_20)
                {
                    this.UpdateListViewItemFromCalculate(lvi, SubItemsConst.scaleFactor1_20, System.Printing.PageMediaSizeName.ISOA3, System.Printing.PageOrientation.Landscape, isFormat);
                }
                else if (paperA4LWidth >= proportionX_1_20 && paperA4LHeight >= proportionY_1_20)
                {
                    this.UpdateListViewItemFromCalculate(lvi, SubItemsConst.scaleFactor1_20, System.Printing.PageMediaSizeName.ISOA4, System.Printing.PageOrientation.Landscape, isFormat);
                }
                else if (paperA3PWidth >= proportionX_1_20 && paperA3PHeight >= proportionY_1_20)
                {
                    this.UpdateListViewItemFromCalculate(lvi, SubItemsConst.scaleFactor1_20, System.Printing.PageMediaSizeName.ISOA3, System.Printing.PageOrientation.Portrait, isFormat);
                }
                else if (paperA4PWidth >= proportionX_1_20 && paperA4PHeight >= proportionY_1_20)
                {
                    this.UpdateListViewItemFromCalculate(lvi, SubItemsConst.scaleFactor1_20, System.Printing.PageMediaSizeName.ISOA4, System.Printing.PageOrientation.Portrait, isFormat);
                }
                // 1_25
                else if (paperA3LWidth >= proportionX_1_25 && paperA3LHeight >= proportionY_1_25)
                {
                    this.UpdateListViewItemFromCalculate(lvi, SubItemsConst.scaleFactor1_25, System.Printing.PageMediaSizeName.ISOA3, System.Printing.PageOrientation.Landscape, isFormat);
                }
                else if (paperA4LWidth >= proportionX_1_25 && paperA4LHeight >= proportionY_1_25)
                {
                    this.UpdateListViewItemFromCalculate(lvi, SubItemsConst.scaleFactor1_25, System.Printing.PageMediaSizeName.ISOA4, System.Printing.PageOrientation.Landscape, isFormat);
                }
                else if (paperA3PWidth >= proportionX_1_25 && paperA3PHeight >= proportionY_1_25)
                {
                    this.UpdateListViewItemFromCalculate(lvi, SubItemsConst.scaleFactor1_25, System.Printing.PageMediaSizeName.ISOA3, System.Printing.PageOrientation.Portrait, isFormat);
                }
                else if (paperA4PWidth >= proportionX_1_25 && paperA4PHeight >= proportionY_1_25)
                {
                    this.UpdateListViewItemFromCalculate(lvi, SubItemsConst.scaleFactor1_25, System.Printing.PageMediaSizeName.ISOA4, System.Printing.PageOrientation.Portrait, isFormat);
                }
                // 1_50
                else if (paperA3LWidth >= proportionX_1_50 && paperA3LHeight >= proportionY_1_50)
                {
                    this.UpdateListViewItemFromCalculate(lvi, SubItemsConst.scaleFactor1_50, System.Printing.PageMediaSizeName.ISOA3, System.Printing.PageOrientation.Landscape, isFormat);
                }
                else if (paperA4LWidth >= proportionX_1_50 && paperA4LHeight >= proportionY_1_50)
                {
                    this.UpdateListViewItemFromCalculate(lvi, SubItemsConst.scaleFactor1_50, System.Printing.PageMediaSizeName.ISOA4, System.Printing.PageOrientation.Landscape, isFormat);
                }
                else if (paperA3PWidth >= proportionX_1_50 && paperA3PHeight >= proportionY_1_50)
                {
                    this.UpdateListViewItemFromCalculate(lvi, SubItemsConst.scaleFactor1_50, System.Printing.PageMediaSizeName.ISOA3, System.Printing.PageOrientation.Portrait, isFormat);
                }
                else if (paperA4PWidth >= proportionX_1_50 && paperA4PHeight >= proportionY_1_50)
                {
                    this.UpdateListViewItemFromCalculate(lvi, SubItemsConst.scaleFactor1_50, System.Printing.PageMediaSizeName.ISOA4, System.Printing.PageOrientation.Portrait, isFormat);
                }
                // Auto
                else
                {
                    this.UpdateListViewItemFromCalculate(lvi, SubItemsConst.scaleFactorAuto, System.Printing.PageMediaSizeName.ISOA3, System.Printing.PageOrientation.Landscape, isFormat);
                }
            }
        }
        private void UpdateListViewItemFromCalculate(ListViewItem lvi, double scale, System.Printing.PageMediaSizeName pageMediaSizeName, System.Printing.PageOrientation pageOrientation, bool isFormat)
        {
            lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Text = new ScaleFactorSubItem(scale).ToString();
            lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Tag = scale;            

            lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Text = PageOrientationSubItem.Dico[(int)pageOrientation]; //pageOrientation.ToString();
            lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Tag = pageOrientation;

            if (isFormat)
            {
                lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Text = pageMediaSizeName.ToString();
                lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Tag = pageMediaSizeName;

                generikPaperSizeText = pageMediaSizeName.ToString();
                this.SetGenerikPaperCombobox();
            }
        }

        private void ClearComboBoxListView()
        {
            foreach (Control control in splitContainer_SPC.Panel1.Controls)
            {
                if (control.GetType().Equals(typeof(ComboBox)))
                {
                    ComboBox comboBox = (ComboBox)control;
                    comboBox.Items.Clear();
                }
            }

            generikPaperList_CBX.Items.Clear();
        }
        private void AssignItemsInComboBox()
        {
            foreach (Control control in splitContainer_SPC.Panel1.Controls)
            {
                if (control.GetType().Equals(typeof(ComboBox)))
                {
                    ComboBox comboBox = (ComboBox)control;
                    if (comboBox.Name.ToUpper().Contains(ConstComboBox.PartialScaleName))
                    {
                        this.ScaleItems(comboBox);
                    }
                    if (comboBox.Name.ToUpper().Contains(ConstComboBox.PartialPaperName))
                    {
                        this.PaperItems(comboBox);
                    }
                    if (comboBox.Name.ToUpper().Contains(ConstComboBox.PartialOrientationName))
                    {
                        this.OrientationItems(comboBox);
                    }
                }               
            }
            generikPaperList_CBX.Items.Add(System.Printing.PageMediaSizeName.ISOA4); //(PageMediaSizeNameSubItem.Dico[(int)System.Printing.PageMediaSizeName.ISOA4]); //
            generikPaperList_CBX.Items.Add(System.Printing.PageMediaSizeName.ISOA3); //(PageMediaSizeNameSubItem.Dico[(int)System.Printing.PageMediaSizeName.ISOA3]); // 

            generikPaperList_CBX.Text = System.Printing.PageMediaSizeName.ISOA3.ToString();          
        }
        private void ScaleItems(ComboBox comboBox)
        {
            comboBox.Items.Add(ScaleFactorSubItem.Dico[SubItemsConst.scaleFactor1_20]);
            comboBox.Items.Add(ScaleFactorSubItem.Dico[SubItemsConst.scaleFactor1_25]);
            comboBox.Items.Add(ScaleFactorSubItem.Dico[SubItemsConst.scaleFactor1_50]);
            comboBox.Items.Add(ScaleFactorSubItem.Dico[SubItemsConst.scaleFactorAuto]);
        }
        private void PaperItems(ComboBox comboBox)
        {
            comboBox.Items.Add(PageMediaSizeNameSubItem.Dico[(int)System.Printing.PageMediaSizeName.ISOA4]); // System.Printing.PageMediaSizeName.ISOA4);
            comboBox.Items.Add(PageMediaSizeNameSubItem.Dico[(int)System.Printing.PageMediaSizeName.ISOA3]); // System.Printing.PageMediaSizeName.ISOA3);
        }
        private void OrientationItems(ComboBox comboBox)
        {
            //(new PageOrientationSubItem(PageOrientationSubItem.Dico[(int)System.Printing.PageOrientation.Portrait]).ToPageOrientation()); //
            comboBox.Items.Add(PageOrientationSubItem.Dico[(int)System.Printing.PageOrientation.Portrait]); //(System.Printing.PageOrientation.Portrait);
            //new PageOrientationSubItem(PageOrientationSubItem.Dico[(int)System.Printing.PageOrientation.Landscape]).ToPageOrientation()); //
            comboBox.Items.Add(PageOrientationSubItem.Dico[(int)System.Printing.PageOrientation.Landscape]); //(System.Printing.PageOrientation.Landscape);  
        }
        private void SetGenerikPaperCombobox()
        {
            this.generikPaperList_CBX.Text = generikPaperSizeText;
            this.generikPaperList_CBX.SelectedItem = generikPaperSizeText;
        }

        private ListViewItem FindListViewItemFromDocName(string docName)
        {
            ListViewItem lvItem = null;

            for (int iDoc = 0; iDoc < myListView_MLV.Items.Count; iDoc++)
            {
                lvItem = myListView_MLV.Items[iDoc];
                if (this.GetDocNameFromListViewItem(lvItem) == docName)
                {
                    return lvItem;
                }
            }
            return null;
        }
        private string GetDocNameFromListViewItem(ListViewItem lvi)
        {
            if (lvi == null)
            {
                return String.Empty;
            }
            return lvi.Tag.ToString();
        }
      
        private void SaveDrawingConfig()
        {
            drawingSurface2D = _plugin.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DRAWING_SURFACE_2D);
        }
        private void SetDrawingConfig()
        {
            _plugin.CurrentAppli.Scene.SceneSetInfo("0", KD.SDK.SceneEnum.SceneInfo.DRAWING_SURFACE_2D);
            _plugin.CurrentAppli.Scene.ZoomAdjusted();
        }
        private void RestoreDrawingConfig()
        {
            if (!String.IsNullOrEmpty(drawingSurface2D))
            {
                _plugin.CurrentAppli.Scene.SceneSetInfo(drawingSurface2D, KD.SDK.SceneEnum.SceneInfo.DRAWING_SURFACE_2D);
            }
        }
        private void SetCurrentFormat()
        {
            _currentScaleFactor = _documentList[lviSelectedRowIndex].ScaleFactorAsString;
            _currentPageMediaSizeName = _documentList[lvItem.Index].PageMediaSizeName.ToString();
            _currentPageOrientation = _documentList[lvItem.Index].PageOrientation.ToString();
        }

        private void InitializeDocumentList()
        {
            this._documentList.Clear();

            this.AddDocTopViewToDocumentList();
            this.AddDocElevationViewToDocumentList(GetWallIDs());
            this.AddDocElevationViewToDocumentList(GetElevSymbolIDs());
        }
        private List<int> GetWallIDs()
        {
            int nbObject = _plugin.CurrentAppli.Scene.SceneGetObjectsNb();           
            List<int> wallIds = new List<int>();           
            for (int iobj = 0; iobj < nbObject; iobj++)
            {
                int objectId = _plugin.CurrentAppli.Scene.SceneGetObjectId(iobj);
                KD.Model.Article article = new KD.Model.Article(_plugin.CurrentAppli, objectId);
                if (article.Type == (int)KD.SDK.SceneEnum.ObjectType.WALL)
                {                   
                    wallIds.Add(article.ObjectId);                   
                }
            }
            return wallIds;
        }
        private List<int> GetElevSymbolIDs()
        {
            int nbObject = _plugin.CurrentAppli.Scene.SceneGetObjectsNb();
            List<int> elevationIds = new List<int>();
            for (int iobj = 0; iobj < nbObject; iobj++)
            {
                int objectId = _plugin.CurrentAppli.Scene.SceneGetObjectId(iobj);
                KD.Model.Article article = new KD.Model.Article(_plugin.CurrentAppli, objectId);
                if (article.Type == (int)KD.SDK.SceneEnum.ObjectType.ELEVATIONSYMBOL)
                {
                    elevationIds.Add(article.ObjectId);
                }
            }
            return elevationIds;
        }
        private Dictionary<int, int> GetTypeDict()
        {
            int nbObject = _plugin.CurrentAppli.Scene.SceneGetObjectsNb();
            Dictionary<int, int> typeDict = new Dictionary<int, int>();

            for (int iobj = 0; iobj < nbObject; iobj++)
            {
                int objectId = _plugin.CurrentAppli.Scene.SceneGetObjectId(iobj);
                KD.Model.Article article = new KD.Model.Article(_plugin.CurrentAppli, objectId);
                if (article.Type == (int)KD.SDK.SceneEnum.ObjectType.WALL || article.Type == (int)KD.SDK.SceneEnum.ObjectType.ELEVATIONSYMBOL)
                {
                    typeDict.Add(article.ObjectId, article.Type);
                }
            }
            return typeDict;
        }
        private void AddDocTopViewToDocumentList()
        {
            TechnicalDocument docTopView = new TechnicalDocument();
            docTopView.ObjectID = KD.Const.UnknownId;
            docTopView.Type = TechnicalDocument.Dico[(int)KD.SDK.SceneEnum.ViewMode.TOP];
            docTopView.TypeID = (int)TechnicalDocumentEnum.viewTypeId.Top;
            docTopView.ScaleFactorAsString = new ScaleFactorSubItem(SubItemsConst.scaleFactor1_20).ToString();
            docTopView.ScaleFactorValue = SubItemsConst.scaleFactor1_20;            
            docTopView.PageMediaSizeName = System.Printing.PageMediaSizeName.ISOA3;
            docTopView.PageMediaSizeNameID = (int)System.Printing.PageMediaSizeName.ISOA3;
            docTopView.PageOrientation = new PageOrientationSubItem(PageOrientationSubItem.Dico[(int)System.Printing.PageOrientation.Landscape]).ToPageOrientation(); // System.Printing.PageOrientation.Portrait;
            docTopView.PageOrientationID = (int)System.Printing.PageOrientation.Landscape;
            docTopView.ViewMode = KD.SDK.SceneEnum.ViewMode.TOP;            
            docTopView.Number = KD.Const.UnknownId.ToString(); 
            _documentList.Add(docTopView);
        }
        private void AddDocElevationViewToDocumentList(List<int> elevationListIds)
        {
            for (int iElevation = 0; iElevation < elevationListIds.Count; iElevation++)
            {
                TechnicalDocument docElevationView = new TechnicalDocument();
                docElevationView.ObjectID = elevationListIds[iElevation];
                docElevationView.Type = TechnicalDocument.Dico[(int)KD.SDK.SceneEnum.ViewMode.VECTELEVATION];

                KD.Model.Article article = new KD.Model.Article(_plugin.CurrentAppli, docElevationView.ObjectID);
                if (article.Type == (int)KD.SDK.SceneEnum.ObjectType.WALL)
                {
                    docElevationView.TypeID = (int)TechnicalDocumentEnum.viewTypeId.WallElevation;
                }
                else if (article.Type == (int)KD.SDK.SceneEnum.ObjectType.ELEVATIONSYMBOL)
                {
                    docElevationView.TypeID = (int)TechnicalDocumentEnum.viewTypeId.SymbolElevation;
                }
                
                docElevationView.ScaleFactorAsString = new ScaleFactorSubItem(SubItemsConst.scaleFactor1_20).ToString();
                docElevationView.ScaleFactorValue = SubItemsConst.scaleFactor1_20;
                docElevationView.PageMediaSizeName = System.Printing.PageMediaSizeName.ISOA3;
                docElevationView.PageMediaSizeNameID = (int)System.Printing.PageMediaSizeName.ISOA3;
                docElevationView.PageOrientation = new PageOrientationSubItem(PageOrientationSubItem.Dico[(int)System.Printing.PageOrientation.Landscape]).ToPageOrientation(); // System.Printing.PageOrientation.Portrait;
                docElevationView.PageOrientationID = (int)System.Printing.PageOrientation.Landscape;
                docElevationView.ViewMode = KD.SDK.SceneEnum.ViewMode.VECTELEVATION;                
                docElevationView.Number = _plugin.CurrentAppli.Scene.ObjectGetInfo(docElevationView.ObjectID, KD.SDK.SceneEnum.ObjectInfo.NUMBER);
                _documentList.Add(docElevationView);
            }
        }
    
        private string GetElevationViewName(TechnicalDocument doc)//Select between 'Top view', 'Wall view' and 'Symbol view'
        {
            foreach (KeyValuePair<int, int> kvp in this.GetTypeDict())
            {
                if (kvp.Key == doc.ObjectID)
                {
                    if (kvp.Value == (int)KD.SDK.SceneEnum.ObjectType.WALL)
                    {
                        return doc.ToString();
                    }
                    else if (kvp.Value == (int)KD.SDK.SceneEnum.ObjectType.ELEVATIONSYMBOL)
                    {
                        return doc.ToString(); // ToSymbol();
                    }
                }
            }
            return doc.ToString();
        }

        private void UpdateListViewFromDocumentList()
        {
            int imageIndex = 0;
            myListView_MLV.Items.Clear();

            foreach (TechnicalDocument ithDoc in this._documentList)
            {
                string viewName = this.GetElevationViewName(ithDoc);  //Select between 'Top view', 'Wall view' and 'Symbol view'                   

                lvItem = new ListViewItem(viewName, imageIndex++);                
                lvItem.Tag = ithDoc;
                lvItem.Text = MyListView.Enum.ColumnIndex.View.ToString(); //String.Empty
                lvItem.SubItems.Add(viewName); //

                lvItem.SubItems.Add(ithDoc.ScaleFactorAsString); //new ScaleFactorSubItem(ithDoc.ScaleFactor)
                lvItem.SubItems.Add(ithDoc.PageMediaSizeName.ToString());//A4
                lvItem.SubItems.Add(ithDoc.PageOrientation.ToString());//portrait
                lvItem.SubItems.Add(MyListView.Enum.ColumnIndex.Overview.ToString()); // Preview button. It replaced by a button

                this.UpdateListViewItemFromDocument(ithDoc, lvItem);
                this.myListView_MLV.Items.Add(lvItem);
            }

            //
            this.SizeAndPlaceControlInForm();
        }
        private void UpdateListViewItemFromDocument(TechnicalDocument doc, ListViewItem lvi)
        {
            string viewName = this.GetElevationViewName(doc);

            lvi.Tag = doc;

            lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Select].Text = String.Empty; // selected case with any string
            lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Select].Tag = String.Empty; // selected case with any string

            lvi.SubItems[(int)MyListView.Enum.ColumnIndex.View].Text = viewName; //Select between 'Top view', 'Wall view' and 'Symbol view'
            lvi.SubItems[(int)MyListView.Enum.ColumnIndex.View].Tag = doc;

            lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Text = doc.ScaleFactorAsString;
            lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Tag = doc.ScaleFactorValue;//new ScaleFactorSubItem(doc.ScaleFactor)

            lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Text = doc.PageMediaSizeName.ToString();
            lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Tag = doc.PageMediaSizeName;

            lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Text = new PageOrientationSubItem(doc.PageOrientation).ToString(); // doc.PageOrientation.ToString();
            lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Tag = doc.PageOrientation;

            lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Overview].Text = String.Empty; // Preview button. It replaced by a button
            lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Overview].Tag = String.Empty; // Preview button. It replaced by a button
        }

        private void UpdateDocumentListFromListView()
        {           
            this._documentList.Clear();
            this.myListView_MLV.Update();
            foreach (ListViewItem lvi in this.myListView_MLV.Items)
            {                
                this.UpdateDocumentFromListViewSubItem(lvi);
                this._documentList.Add((TechnicalDocument)lvi.Tag);                
            }
        }     
        private void UpdateDocumentFromListViewSubItem(ListViewItem lvi)
        {           
            TechnicalDocument doc = (TechnicalDocument)lvi.Tag;

            KD.Model.Article article = new KD.Model.Article(_plugin.CurrentAppli, doc.ObjectID);
            
            if (article.Type == (int)KD.SDK.SceneEnum.ObjectType.WALL)
            {
                doc.TypeID = (int)TechnicalDocumentEnum.viewTypeId.WallElevation;
            }
            else if (article.Type == (int)KD.SDK.SceneEnum.ObjectType.ELEVATIONSYMBOL)
            {
                doc.TypeID = (int)TechnicalDocumentEnum.viewTypeId.SymbolElevation;
            }
            else
            {
                doc.TypeID = (int)TechnicalDocumentEnum.viewTypeId.Top;
            }

            doc.ScaleFactorAsString = lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Text;
            doc.ScaleFactorValue = (double)lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Tag; 

            doc.PageMediaSizeName = (System.Printing.PageMediaSizeName)lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Tag;
            PageMediaSizeNameSubItem pmsnInt = new PageMediaSizeNameSubItem(doc.PageMediaSizeName);
            doc.PageMediaSizeNameID = pmsnInt.ToInt();

            doc.PageOrientation = (System.Printing.PageOrientation)lvi.SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Tag;
            PageOrientationSubItem poInt = new PageOrientationSubItem(doc.PageOrientation);
            doc.PageOrientationID = poInt.ToInt();           
                       
            lvi.Tag = doc;

            //this.FindDocNameFromChoice(lvItem.Index); // here there is translate to contol

        }

        private void AssignComboBox(Rectangle clickedItem)
        {
            ComboBox comboBox = null;
            Button button = null;

            switch (lviSelectedColumnIndex)
            {
                case (int)MyListView.Enum.ColumnIndex.Scale:
                    comboBox = this.scaleList_CBX;
                    break;
                case (int)MyListView.Enum.ColumnIndex.Paper:
                    comboBox = this.paperList_CBX;
                    break;
                case (int)MyListView.Enum.ColumnIndex.Orientation:
                    comboBox = this.orientationList_CBX;
                    break;
                case (int)MyListView.Enum.ColumnIndex.Overview:
                    button = this.myImageButton;
                    break;
                default:
                    return;
            }

            if (comboBox != null)
            {
                // Assign calculated bounds to the ComboBox.
                comboBox.Bounds = clickedItem;
                // Set default text for ComboBox to match the item that is clicked.
                comboBox.Text = lvItem.Text;
                // Display the ComboBox, and make sure that it is on top with focus.
                comboBox.Visible = true;
                comboBox.BringToFront();
                comboBox.Focus();
            }
            if (button != null)
            {
                button.Visible = true;
                button.BringToFront();
                button.Focus();
            }
        }
        private void AddTwinButton()
        {
            for (int lineIndex = 0; lineIndex < this.myListView_MLV.Items.Count; lineIndex++)
            {
                Rectangle subItemImageButton = this.myListView_MLV.Items[lineIndex].SubItems[(int)MyListView.Enum.ColumnIndex.Overview].Bounds;
                subItemImageButton.Y = this.myListView_MLV.Items[lineIndex].SubItems[(int)MyListView.Enum.ColumnIndex.Overview].Bounds.Y;// + this.myListView.Top;
                subItemImageButton.X = this.myListView_MLV.Items[lineIndex].SubItems[(int)MyListView.Enum.ColumnIndex.Overview].Bounds.X + this.myListView_MLV.Left;
                subItemImageButton.Height = this.myListView_MLV.Items[lineIndex].SubItems[(int)MyListView.Enum.ColumnIndex.Overview].Bounds.Height + 4;
                subItemImageButton.Width = this.myListView_MLV.Items[lineIndex].SubItems[(int)MyListView.Enum.ColumnIndex.Overview].Bounds.Height + 4;
                // Assign calculated bounds to the ComboBox.
                myOverViewButton = new MyImageButton();
                myListView_MLV.Controls.Add(myOverViewButton);
                myOverViewButton.Bounds = subItemImageButton;
                myOverViewButton.Location = new Point(subItemImageButton.X + 10, subItemImageButton.Y - 2);

                // Set default text for ComboBox to match the item that is clicked.                 
                myOverViewButton.Visible = true;
                myOverViewButton.BringToFront();
                myOverViewButton.Name = "myOverViewButton" + lineIndex;
                myOverViewButton.Tag = lineIndex;//.ToString();
                myOverViewButton.Click += new System.EventHandler(this.myOverViewButton_Click);
            }

        }
        private void TwinButtonLocation()
        {
            foreach (ListViewItem lvItem in this.myListView_MLV.Items)
            {
                int x1 = this.myListView_MLV.Items[lvItem.Index].SubItems[(int)MyListView.Enum.ColumnIndex.Overview].Bounds.X + this.myListView_MLV.Left + 10;
                int y1 = this.myListView_MLV.Items[lvItem.Index].SubItems[(int)MyListView.Enum.ColumnIndex.Overview].Bounds.Y - 2;

                Control[] controls = myListView_MLV.Controls.Find("myOverViewButton" + lvItem.Index, true);
                foreach (Control control in controls)
                {
                    control.Location = new Point(x1, y1);
                }
            }
        }

        private void TransferTechnicalPlanToSM2()
        {
            foreach (ListViewItem lvi in myListView_MLV.Items)
            {
                if (lvi.Checked)
                {
                    this.Build(false, lvi);//lvItem.Index
                }
            }
        }
        private void SelectOrDeselectAllListViewItem()
        {
            lviSelectedColumnIndex = 0;
            foreach (ListViewItem lvItem in myListView_MLV.Items)
            {
                if (selectAll_CHB.Checked)
                {
                    lvItem.Checked = true;
                }
                else if (!selectAll_CHB.Checked)
                {
                    lvItem.Checked = false;
                }
            }
            myListView_MLV.Refresh();
            myListView_MLV.Update();
        }
        private void InitializeTechnicalPlanFileNameForm()
        {
            if (_technicalPlanDocumentFileNameForm == null)
            {
                _technicalPlanDocumentFileNameForm = new TechnicalPlanDocumentFileNameForm(_pluginWord, _dico);                
            }
        }
        private void ShowTechnicalPlanFileNameForm()
        {
            _technicalPlanDocumentFileNameForm.ShowDialog();
        }
        private void DialogAnswerToSM2()
        {
            if (_technicalPlanDocumentFileNameForm.DialogResult == DialogResult.OK)
            {
                this.SaveCustomInfo();
                this.TransferTechnicalPlanToSM2();
                this.Ok_BTN.PerformClick();
            }
        }
        private void DisableTechnicalPlanFileNameForm()
        {
            _technicalPlanDocumentFileNameForm = null;
        }

        #region // Form
        private void GenerateViewDialogForm_Load(object sender, EventArgs e)
        {
            this.InitializeListView();
            this.DesactivatePdfViewer();
            this.InitializePreviewDirectory();           
            this.ClearComboBoxListView();
            this.AssignItemsInComboBox();
            this.InitializeDocumentList();
            
            this.UpdateListViewFromDocumentList();           
        }
        private void GenerateViewDialogForm_Shown(object sender, EventArgs e)
        {
            this.AddTwinButton();           
            this.LoadCustomInfoOrCalculateBestFormat();           
        }
        private void GenerateViewDialogForm_FormClosing(object sender, FormClosingEventArgs e)
        {                
            this.Plugin.viewDialogForm = null;
        }
        private void GenerateViewDialogForm_FormClosed(object sender, FormClosedEventArgs e)
        {            
            this.Plugin.viewDialogForm = null;
        }

        private void MyListView_MLV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvItem != null)
            {
                lviSelectedRowIndex = lvItem.Index;
            }
        }
        private void MyListView_MLV_MouseUp(object sender, MouseEventArgs e)
        {
            // Get the item on the row that is clicked.
            lvItem = this.myListView_MLV.GetItemAt(e.X, e.Y);

            lviSelectedRowIndex = this.GetSelectedRowIndex();
            lviSelectedColumnIndex = this.GetSelectColumnIndex(e.X);

            // Make sure that an item is clicked.
            if (lvItem != null && lviSelectedRowIndex != KD.Const.UnknownId && lviSelectedColumnIndex != KD.Const.UnknownId)
            {
                // Get the bounds of the item that is clicked.
                Rectangle ClickedItem = lvItem.Bounds;

                // Verify that the column is completely scrolled off to the left.
                if ((ClickedItem.Left + this.myListView_MLV.Columns[lviSelectedColumnIndex].Width) < 0)
                {
                    // If the cell is out of view to the left, do nothing.
                    return;
                }

                // Verify that the column is partially scrolled off to the left.
                else if (ClickedItem.Left < 0)
                {
                    // Determine if column extends beyond right side of ListView.
                    if ((ClickedItem.Left + this.myListView_MLV.Columns[lviSelectedColumnIndex].Width) > this.myListView_MLV.Width)
                    {
                        // Set width of column to match width of ListView.
                        ClickedItem.Width = this.myListView_MLV.Width;
                        ClickedItem.X = 0;
                    }
                    else
                    {
                        // Right side of cell is in view.
                        ClickedItem.Width = this.myListView_MLV.Columns[lviSelectedColumnIndex].Width + ClickedItem.Left;
                        ClickedItem.X = 2;
                    }
                }
                else if (this.myListView_MLV.Columns[lviSelectedColumnIndex].Width > this.myListView_MLV.Width)
                {
                    ClickedItem.Width = this.myListView_MLV.Width;
                }
                else
                {
                    ClickedItem.Width = this.myListView_MLV.Columns[lviSelectedColumnIndex].Width;
                    ClickedItem.X = 2;
                }

                // Adjust the top to account for the location of the ListView.
                ClickedItem.Y = this.myListView_MLV.Items[lviSelectedRowIndex].Bounds.Y + this.myListView_MLV.Top;
                ClickedItem.X = this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[lviSelectedColumnIndex].Bounds.X + this.myListView_MLV.Left;
                ClickedItem.Height = lvItem.Bounds.Height;

                this.AssignComboBox(ClickedItem);
            }

        }
        private void MyListView_MLV_MouseDown(object sender, MouseEventArgs e)
        {
            lvItem = this.myListView_MLV.GetItemAt(e.X, e.Y);
        }       
        private void MyListView_MLV_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (lvItem != null)
            {
                if (lviSelectedColumnIndex > (int)MyListView.Enum.ColumnIndex.View)
                {
                    lvItem.Selected = false;
                    lvItem.Checked = false;
                    return;
                }

                if (e.Item.Index != KD.Const.UnknownId && lvItem.Index != KD.Const.UnknownId)
                {
                    if (lvItem.Checked)
                    {
                        lviSelectedRowIndex = lvItem.Index;
                    }
                }
            }
        }    
        private void myListView_MLV_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            this.TwinButtonLocation();            
        }

        private void scaleList_CBX_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the user presses ESC.
            this.KeyPressEvent(e, this.scaleList_CBX);

        }
        private void scaleList_CBX_Leave(object sender, EventArgs e)
        {            
            this.SetComboBoxTextInItem(this.scaleList_CBX);
            this.HideComboBox(this.scaleList_CBX);
        }
        private void scaleList_CBX_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            this.HideComboBox(this.scaleList_CBX);
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Tag = new ScaleFactorSubItem((string)comboBox.SelectedItem).ToDouble();

            //this.UpdateDocumentFromListViewSubItem(lvItem); //this one work but not implement the _documentList
            this.UpdateDocumentListFromListView(); //this one implement the _documentList but look if it work ?
        }
        private void scaleList_CBX_Enter(object sender, EventArgs e)
        {
            this.SetItemTextInComboBox(this.scaleList_CBX);
        }

        private void paperList_CBX_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.KeyPressEvent(e, this.paperList_CBX);
        }
        private void paperList_CBX_Leave(object sender, EventArgs e)
        {
            this.SetComboBoxTextInItem(this.paperList_CBX);
            this.HideComboBox(this.paperList_CBX);
        }
        private void paperList_CBX_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            this.HideComboBox(this.paperList_CBX);
            this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Tag = (System.Printing.PageMediaSizeName)comboBox.SelectedItem;

            //this.UpdateDocumentFromListViewSubItem(lvItem); //this one work but not implement the _documentList
            this.UpdateDocumentListFromListView(); //this one implement the _documentList but look if it work ?
        }
        private void paperList_CBX_Enter(object sender, EventArgs e)
        {
            this.SetItemTextInComboBox(this.paperList_CBX);
        }

        private void orientationList_CBX_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.KeyPressEvent(e, this.orientationList_CBX);
        }
        private void orientationList_CBX_Leave(object sender, EventArgs e)
        {
            this.SetComboBoxTextInItem(this.orientationList_CBX);
            this.HideComboBox(this.orientationList_CBX);
        }
        private void orientationList_CBX_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            this.SetItemTagInComboBox(comboBox);
            this.HideComboBox(this.orientationList_CBX);
            this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Tag = (System.Printing.PageOrientation)comboBox.Tag; // SelectedItem;

            //this.UpdateDocumentFromListViewSubItem(lvItem); //this one work but not implement the _documentList
            this.UpdateDocumentListFromListView(); //this one implement the _documentList but look if it work ?
        }
        private void orientationList_CBX_Enter(object sender, EventArgs e)
        {
            this.SetItemTextInComboBox(this.orientationList_CBX);
        }

        private void myOverViewButton_Click(object sender, EventArgs e)
        {
            Button overViewButton = (Button)sender;
            lvItem = this.myListView_MLV.Items[(int)overViewButton.Tag];

            if (lvItem != null)
            {
                lviSelectedRowIndex = lvItem.Index;
                //maybe save zoom or green frame. Actually i take the green frame
                //this.SaveDrawingConfig();
                //this.SetDrawingConfig();
                this.SetCurrentFormat();
                this.Build(true, lvItem);//lviSelectedRowIndex
                this.RestoreDrawingConfig();
            }

        }

        private void pdfDocumentView_PDFV_ZoomChanged(object sender, int zoomFactor)
        {
            _zoom = zoomFactor;
        }

        private void Cancel_BTN_Click(object sender, EventArgs e)
        {
            this.DesactivatePdfViewer();
            this.InitializePreviewDirectory();
            this.Close();
        }
        private void Ok_BTN_Click(object sender, EventArgs e)
        {
            this.SaveCustomInfo();
            this.DesactivatePdfViewer();
            this.InitializePreviewDirectory();
            this.Close();
        }

        private void transferSM2_BTN_Click(object sender, EventArgs e)
        {
            this.InitializeTechnicalPlanFileNameForm();
            this.ShowTechnicalPlanFileNameForm();
            this.DialogAnswerToSM2();
            this.DisableTechnicalPlanFileNameForm();
        }

        private void selectAll_CHB_CheckedChanged(object sender, EventArgs e)
        {
            this.SelectOrDeselectAllListViewItem();            
        }

        private void generikPaperList_CBX_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem lvItem in myListView_MLV.Items)
            {
                lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Text = generikPaperList_CBX.Text;
                ComboBox comboBox = (ComboBox)sender;
                lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Tag = (System.Printing.PageMediaSizeName)comboBox.SelectedItem;

                generikPaperSizeText = comboBox.Text;

                //this.UpdateDocumentFromListViewSubItem(lvItem); //this one work but not implement the _documentList
                           
            }
            this.UpdateDocumentListFromListView(); //this one implement the _documentList but look if it work ? yes
        }
        private void generikPaperList_CBX_TextChanged(object sender, EventArgs e)
        {
            generikPaperSizeText = generikPaperList_CBX.Text;
            this.SetGenerikPaperCombobox();
        }

        private void openFolder_BTN_Click(object sender, EventArgs e)
        {           

            string dir = this.ArchiveFileDir;
            if (System.IO.Directory.Exists(Plugin.SM2Directory))//System.IO.Path.Combine(this.ArchiveFileDir, ConstRessourceNames.DocTechnicalPlanDir
            {
                dir = Plugin.SM2Directory; // (System.IO.Path.Combine(this.ArchiveFileDir, ConstRessourceNames.DocTechnicalPlanDir));
            }
            KD.IO.Explorer.ShowDir(dir);                    
        }

        private void print_BTN_Click(object sender, EventArgs e)
        {
            if (_loadedDocument != null && pdfDocumentView_PDFV.Visible)
            {
                PrintDialog printDialog = new PrintDialog();
                printDialog.AllowPrintToFile = true;
                printDialog.AllowSomePages = true;
                printDialog.AllowCurrentPage = true;

                printDialog.Document = pdfDocumentView_PDFV.PrintDocument;

                int angleLandScape = printDialog.PrinterSettings.LandscapeAngle;
                System.Drawing.Printing.PrinterSettings.PaperSizeCollection paperSizeCollection = printDialog.PrinterSettings.PaperSizes;

                printDialog.Document.PrinterSettings.DefaultPageSettings.Landscape = true;
                printDialog.Document.PrinterSettings.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("First custom size", 100, 200);

                if (printDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //if (printDialog.Document != null)
                    //{ 
                    if (this.myListView_MLV.CheckedItems.Count > 1)
                    {
                        //PrintDocument[] documents = new PrintDocument[this.docListView.CheckedItems.Count];
                        foreach (ListViewItem lvItem in this.myListView_MLV.CheckedItems)
                        {
                            string docname = _pluginWord.GetLocalizedDocName(_pluginWord.DocIndex2Use);//GetDocNameFromListViewItem(lvItem);
                            string pdfFilePath = _pluginWord.GetArchiveFilePath(docname, KD.IO.File.Extension.Pdf);

                            // Set cursor as hourglass
                            Cursor.Current = Cursors.WaitCursor;
                            pdfDocumentView_PDFV.Load(pdfFilePath);
                            // Set cursor as default arrow
                            Cursor.Current = Cursors.Default;
                            if (pdfDocumentView_PDFV.PrintDocument != null)
                            {
                                pdfDocumentView_PDFV.PrintDocument.Print();
                            }
                            //documents.SetValue(new PrintDocument(), iDoc++);
                        }
                        // reload last loaded by user (not by program, like above to update PrintDocument)
                        this.CreateAndLoadPdfLoadedDocument(this.CurrentPdfFilePath);
                        // MultiPrintDocument mp = new MultiPrintDocument(documents);
                        // printDialog.Document = mp;
                    }
                    else
                    {
                        printDialog.Document = pdfDocumentView_PDFV.PrintDocument;
                        printDialog.Document.Print();
                    }
                    //XpsDocument xpsDocument = new System.Windows.Xps.Packaging.XpsDocument("C:\\FixedDocumentSequence.xps", FileAccess.ReadWrite);
                    //FixedDocumentSequence fixedDocSeq = xpsDocument.GetFixedDocumentSequence();
                    //printDialog.PrintDocument(fixedDocSeq.DocumentPaginator, "Test print job");
                }
            }
        }

        private void InitCustomInfo_BTN_Click(object sender, EventArgs e)
        {           
            _technicalDocument.DeleteXml();
            _pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(null, TechnicalDocument.technicalDocumentsTag);
        }
        #endregion
    }

    public class SubItemsConst
    {
        public const double scaleFactorUnknown = -1.0;
        public const double scaleFactor1_20 = 0.05;
        public const double scaleFactor1_25 = 0.04;
        public const double scaleFactor1_50 = 0.02;
        public const double scaleFactorAuto = 0.0;

        public const string scaleFactorString1_20 = "120";
        public const string scaleFactorString1_25 = "125";
        public const string scaleFactorString1_50 = "150";
        public const string scaleFactorStringAuto = "Auto";
    }


    #region //UI
    public class ScaleFactorSubItem : ListViewItem.ListViewSubItem
    {
        public static Dictionary<double, string> Dico = new Dictionary<double, string>();

        private double _scaleFactor;
        private string _scaleFactorString;

        public double ScaleFactor
        {
            get { return _scaleFactor; }
            set { _scaleFactor = value; }
        }

        public string ScaleFactorString
        {
            get { return _scaleFactorString; }
            set { _scaleFactorString = value; }
        }

        public ScaleFactorSubItem(double scaleFactor)
        {
            this._scaleFactor = scaleFactor;
            this.Text = this.ToString();
        }
        public ScaleFactorSubItem(string scaleFactorString)
        {
            _scaleFactorString = scaleFactorString;
        }


        public override string ToString()
        {
            if (_scaleFactor == SubItemsConst.scaleFactor1_20)
            {
                return (ScaleFactorSubItem.Dico[SubItemsConst.scaleFactor1_20]); //(SubItemsConst.scaleFactorString1_20);
            }
            else if (_scaleFactor == SubItemsConst.scaleFactor1_25)
            {
                return (ScaleFactorSubItem.Dico[SubItemsConst.scaleFactor1_25]); // (SubItemsConst.scaleFactorString1_25);
            }
            else if (_scaleFactor == SubItemsConst.scaleFactor1_50)
            {
                return (ScaleFactorSubItem.Dico[SubItemsConst.scaleFactor1_50]); // (SubItemsConst.scaleFactorString1_50);
            }
            else if (_scaleFactor == SubItemsConst.scaleFactorAuto)
            {
                return ScaleFactorSubItem.Dico[SubItemsConst.scaleFactorAuto];
                //return ("Auto");
            }
            else
            {
                return ScaleFactorSubItem.Dico[SubItemsConst.scaleFactorAuto];
            }
        }

        public double ToDouble()
        {
            if (_scaleFactorString == ScaleFactorSubItem.Dico[SubItemsConst.scaleFactor1_20]) // SubItemsConst.scaleFactorString1_20)
            {
                return (SubItemsConst.scaleFactor1_20);
            }
            else if (_scaleFactorString == ScaleFactorSubItem.Dico[SubItemsConst.scaleFactor1_25]) //SubItemsConst.scaleFactorString1_25)
            {
                return (SubItemsConst.scaleFactor1_25);
            }
            else if (_scaleFactorString == ScaleFactorSubItem.Dico[SubItemsConst.scaleFactor1_50]) //SubItemsConst.scaleFactorString1_50)
            {
                return (SubItemsConst.scaleFactor1_50);
            }
            else if (_scaleFactorString == ScaleFactorSubItem.Dico[SubItemsConst.scaleFactorAuto]) //SubItemsConst.scaleFactorStringAuto)
            {
                return (SubItemsConst.scaleFactorAuto);
            }
            else
            {
                return (SubItemsConst.scaleFactorAuto);
            }
        }
    }

    public class PageMediaSizeNameSubItem : ListViewItem.ListViewSubItem
    {
        public static Dictionary<int, string> Dico = new Dictionary<int, string>();

        private string _pageMediaSizeNameString;
        private System.Printing.PageMediaSizeName _pageMediaSizeName;

        public System.Printing.PageMediaSizeName PageMediaSizeName
        {
            get { return _pageMediaSizeName; }
            set { _pageMediaSizeName = value; }
        }
        public string PageMediaSizeNameString
        {
            get { return _pageMediaSizeNameString; }
            set { _pageMediaSizeNameString = value; }
        }

        public PageMediaSizeNameSubItem(System.Printing.PageMediaSizeName pageMediaSizeName)
        {
            this._pageMediaSizeName = pageMediaSizeName;
        }
        public PageMediaSizeNameSubItem(string pageMediaSizeNameString)
        {
            this._pageMediaSizeNameString = pageMediaSizeNameString;
        }


        public override string ToString()
        {
            return PageMediaSizeNameSubItem.Dico[(int)this._pageMediaSizeName];
        }
        public int ToInt()
        {
            return (int)this._pageMediaSizeName;
        }
        public System.Printing.PageMediaSizeName ToPageMediaSizeName()
        {
            if (_pageMediaSizeNameString == System.Printing.PageMediaSizeName.ISOA4.ToString()) // PageMediaSizeNameSubItem.Dico[(int)System.Printing.PageMediaSizeName.ISOA4])  //SubItemsConst.pageMediaSizeNameStringISOA4)
            {
                return System.Printing.PageMediaSizeName.ISOA4;
            }
            else if (_pageMediaSizeNameString == System.Printing.PageMediaSizeName.ISOA3.ToString()) // PageMediaSizeNameSubItem.Dico[(int)System.Printing.PageMediaSizeName.ISOA3]) //SubItemsConst.pageMediaSizeNameStringISOA3)
            {
                return System.Printing.PageMediaSizeName.ISOA3;
            }

            return System.Printing.PageMediaSizeName.Unknown;
        }
    }

    public class PageOrientationSubItem :  ListViewItem.ListViewSubItem
    {
        public static Dictionary<int, string> Dico = new Dictionary<int, string>();

        private string _pageOrientationString;
        private System.Printing.PageOrientation _pageOrientation;

        public System.Printing.PageOrientation PageOrientation
        {
            get { return _pageOrientation; }
            set { _pageOrientation = value; }
        }
        public string PageOrientationString
        {
            get { return _pageOrientationString; }
            set { _pageOrientationString = value; }
        }


        public PageOrientationSubItem(System.Printing.PageOrientation pageOrientation)
        {
            this._pageOrientation = pageOrientation;
        }
        public PageOrientationSubItem(string pageOrientationString)
        {
            this._pageOrientationString = pageOrientationString;
        }

        public override string ToString()
        {
            return PageOrientationSubItem.Dico[(int)this._pageOrientation];
        }
        public int ToInt()
        {
            return (int)this._pageOrientation;
        }
        public System.Printing.PageOrientation ToPageOrientation()
        {
            if (_pageOrientationString == PageOrientationSubItem.Dico[(int)System.Printing.PageOrientation.Portrait]) // SubItemsConst.pageOrientationStringPortrait)
            {
                return System.Printing.PageOrientation.Portrait;
            }
            else if (_pageOrientationString == PageOrientationSubItem.Dico[(int)System.Printing.PageOrientation.Landscape])  //SubItemsConst.pageOrientationStringLandScape)
            {
                return System.Printing.PageOrientation.Landscape;
            }

            return System.Printing.PageOrientation.Unknown;
        }

       // public 
    }
    #endregion
}
