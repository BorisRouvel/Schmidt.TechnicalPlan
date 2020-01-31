using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;


namespace Schmidt.TechnicalPlan
{

    public partial class GenerateViewDialogForm : Form
    {
        public static string generikPaperSizeText = SubItemsConst.pageMediaSizeNameStringISOA4;
        private Plugin _plugin;
        private string _currentPdfFilePath;
        private string _archiveFileDir = String.Empty;

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

        private int lviSelectedRowIndex = -1;
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

        //
        //Printer format setting
        double margeX = 20.0;
        double margeY = 67.0;

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
        }

        private void InitializePreviewDirectory()
        {
            string technicalPlanDir = String.Empty;
            this._archiveFileDir = this._pluginWord.GetRootDir();
            if (!this.ArchiveFileDir.Contains(ConstFile.DocTechnicalPlanDir))
            {
                technicalPlanDir = System.IO.Path.Combine(this.ArchiveFileDir, ConstFile.DocTechnicalPlanDir, ConstFile.TechnicalPlanPreviewDirName);
            }
            else
            {
                technicalPlanDir = System.IO.Path.Combine(this.ArchiveFileDir, ConstFile.TechnicalPlanPreviewDirName);
            }
          

            if (System.IO.Directory.Exists(technicalPlanDir))
            {
                IEnumerable<string> technicalPlanFiles = System.IO.Directory.EnumerateFiles(technicalPlanDir);
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

            var header = new List<string>();

            header.Add(this._dico.GetTranslate(TranslateIdentifyId.ColumnHeaderSelectID));
            header.Add(this._dico.GetTranslate(TranslateIdentifyId.ColumnHeaderViewID));
            header.Add(this._dico.GetTranslate(TranslateIdentifyId.ColumnHeaderScaleID));
            header.Add(this._dico.GetTranslate(TranslateIdentifyId.ColumnHeaderPaperID));
            header.Add(this._dico.GetTranslate(TranslateIdentifyId.ColumnHeaderOrientationID));
            header.Add(this._dico.GetTranslate(TranslateIdentifyId.ColumnHeaderOverviewID));
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
            this.Ok_BTN.Text = _dico.GetTranslate(TranslateIdentifyId.OkButtonUIID);
            this.Cancel_BTN.Text = _dico.GetTranslate(TranslateIdentifyId.CancelButtonUIID);
            this.selectAll_CHB.Text = _dico.GetTranslate(TranslateIdentifyId.SelectAllViewCheckBoxUIID);
            this.paper_LAB.Text = _dico.GetTranslate(TranslateIdentifyId.ColumnHeaderPaperID);

            this.ToolStripButtonVisible(this.openFolder_BTN);
            this.ToolStripButtonVisible(this.print_BTN);
        }

        private void ToolStripButtonVisible(ToolStripButton TSbutton)
        {
            TSbutton.Tag = 1; // take future info (here 0) in custom config xml

            if ((int)TSbutton.Tag == 0)
            {
                TSbutton.Visible = false;
            }
            else if ((int)TSbutton.Tag == 1)
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
     
        public void Build(bool preview, int lviIndex)
        {
            this.DesactivatePdfViewer();

            Cursor.Current = Cursors.WaitCursor;

            this._pluginWord.DocIndex2Use = FindDocNameFromChoice(lviIndex);
            if (this._pluginWord.DocIndex2Use != KD.Const.UnknownId)
            {
                this._pluginWord.GenerateDocument(false);

                string docname = this._pluginWord.GetLocalizedDocName(this._pluginWord.DocIndex2Use);
                if (!String.IsNullOrEmpty(docname))
                {
                    string pdfFilePath = this._pluginWord.GetArchiveFilePath(docname, KD.IO.File.Extension.Pdf);
                    string dotFilePath = this._pluginWord.GetArchiveFilePath(docname, KD.IO.File.Extension.Dot);

                    string dirPdfPath = this.MoveFileToDocPlanDir(lviIndex, pdfFilePath, dotFilePath, 0);
                    string dirDotPath = dirPdfPath.Replace(KD.IO.File.Extension.Pdf, KD.IO.File.Extension.Dot);

                    if (preview)
                    {
                        this.LoadPdfFile(this.MoveFileToPreviewDir(dirPdfPath, dirDotPath, 0));
                    }                   
                }
            }
            Cursor.Current = Cursors.Arrow;
        }
        private string MoveFileToDocPlanDir(int lviIndex, string pdfFilePath, string dotFilePath, int time)
        {
            System.Windows.Forms.Application.DoEvents();

            string currentDir = System.IO.Path.GetDirectoryName(pdfFilePath);
            if (!currentDir.Contains(ConstFile.DocTechnicalPlanDir))
            {
                string currentPdfDir = System.IO.Path.Combine(currentDir, ConstFile.DocTechnicalPlanDir);

                this.CreateDirIfNotExist(currentPdfDir);

                string viewFileName = this.GetViewFileName(lviIndex, System.IO.Path.GetFileName(pdfFilePath));                

                string newPdfFilePath = System.IO.Path.Combine(currentPdfDir, viewFileName); // System.IO.Path.GetFileName(pdfFilePath));
                string newDotFilePath = System.IO.Path.Combine(currentPdfDir, viewFileName.Replace(KD.IO.File.Extension.Pdf, KD.IO.File.Extension.Dot));

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
                catch (Exception ex)
                {
                    return pdfFilePath;
                }
            }
            return pdfFilePath;
        }
        private string MoveFileToPreviewDir(string pdfFilePath, string dotFilePath, int time)
        {
            System.Windows.Forms.Application.DoEvents();

            string currentDir = System.IO.Path.GetDirectoryName(pdfFilePath);
            this.CreateDirIfNotExist(System.IO.Path.Combine(currentDir, ConstFile.TechnicalPlanPreviewDirName));

            string newDir = System.IO.Path.Combine(currentDir, ConstFile.TechnicalPlanPreviewDirName);
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
                        MoveFileToPreviewDir(pdfFilePath, dotFilePath, time++);
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
                this._loadedDocument = new Syncfusion.Pdf.Parsing.PdfLoadedDocument(pdfFilePath);
                pdfDocumentView_PDFV.Load(_loadedDocument); // @"D:\Ic90dev\Scenes\5DE66AB8_0226_01\DocTechnicalPlan\TECHNICAL_PLAN_PREVIEW\Elévation du mur.pdf"); 

                // Set cursor as default arrow
                Cursor.Current = Cursors.Default;

                // first load fit page                
                if (_zoom == InitialZoomValue)
                {
                    pdfDocumentView_PDFV.ZoomMode = Syncfusion.Windows.Forms.PdfViewer.ZoomMode.FitPage;
                }
                else // take last zoom value                 
                {
                    pdfDocumentView_PDFV.ZoomTo(Convert.ToInt32(_zoom));
                }

            }

            // can Update member only after load
            _currentPdfFilePath = pdfFilePath;

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
                //// need to update CurrentPdfFilePath now for later use.
                //// BUt can't use LoadPdfFile coz it can call DesactivatePdfViewer and makes infinite loop
                //this.pdfDocumentView_PDFV.Load(String.Empty);
                //this.CurrentPdfFilePath = String.Empty;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            //this.pdfDocumentView_PDFV.Enabled = false;
            this.pdfDocumentView_PDFV.Visible = false;
            return;
        }
        private void ActivatePdfViewer()
        {
            this.splitContainer_SPC.Panel2.Show();
            //this.pdfDocumentView_PDFV.Enabled = true;
            this.pdfDocumentView_PDFV.Visible = true;
            return;
        }
        private int FindDocNameFromChoice(int iDoc)
        {
            int rank = -1;

            foreach (ListViewItem lvItem in myListView_MLV.Items)
            {
                if (lvItem != null && lvItem.Index == iDoc)
                {
                    KD.SDK.SceneEnum.ViewMode viewMode = _documentList[iDoc].ViewMode;
                    string viewType = String.Empty;

                    if (viewMode == KD.SDK.SceneEnum.ViewMode.TOP)
                    {
                        viewType = "TOP" + KD.StringTools.Const.MinusSign;
                    }
                    else if (viewMode == KD.SDK.SceneEnum.ViewMode.VECTELEVATION)
                    {
                        viewType = "ELEV" + KD.StringTools.Const.MinusSign;
                    }

                    string scale = lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Text.Replace(KD.StringTools.Const.Slatch, String.Empty) + KD.StringTools.Const.MinusSign;
                    string subDocName = viewType + scale + lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Text + KD.StringTools.Const.MinusSign +
                                                            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Text;

                    int nbSubDoc = this._pluginWord.CurrentAppli.GetDocsNb();

                    for (int iSubDoc = 0; iSubDoc <= nbSubDoc; iSubDoc++)
                    {
                        rank = iSubDoc;
                        string name = this._pluginWord.CurrentAppli.DocGetInfo(rank, KD.SDK.AppliEnum.DocInfo.NAME);

                        if (name.ToUpper().Equals(subDocName.ToUpper()))
                        {                             
                            _documentList[iDoc].FileName = this.GetViewFileName(iDoc, subDocName); ;                          
                            break;
                        }
                    }
                }
            }
            return rank;
        }
        private string GetViewFileName(int lviIndex, string fileName)
        {
            string[] stringSeparators = new string[] { KD.StringTools.Const.MinusSign };
            string[] splitFileName = fileName.Split(stringSeparators, StringSplitOptions.None);

            string fileNameEntete = _dico.GetTranslate(splitFileName[0]);

            if (lviIndex > 0)
            {
                string wallName = fileNameEntete + _documentList[lviIndex].Number;
                for (int i = 1; i <= splitFileName.Length - 1; i++)
                {
                    wallName += KD.StringTools.Const.MinusSign + splitFileName[i];
                }
                return wallName;
            }
            return fileName;
        }

        private int GetSelectedRowIndex()
        {
            if (lvItem != null)
            {
                return lvItem.Index;
            }
            return -1;
        }
        private int GetSelectColumnIndex(int x)
        {
            if (lviSelectedRowIndex != -1)
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

        #region // Keep it for instead // loadXMLin listview_ok
        //XmlNodeList nodeList = xmlNode.SelectNodes(TechnicalDocument.viewTag);

        //// mettre 1 ligne vue de dessus et mur avec leur id , faut l 'ecrire dans xml            
        //int itemsNb = myListView_MLV.Items.Count;
        //int itemNb = 0;

        //foreach (XmlNode node in nodeList)
        //{
        //    if (itemNb == itemsNb) { return; }

        //    XmlNodeList childs = node.ChildNodes;

        //    foreach (XmlNode child in childs)
        //    {
        //        if (child.Name == TechnicalDocument.scaleFactorTag)
        //        {
        //            this.myListView_MLV.Items[itemNb].SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Text = child.InnerText;
        //        }
        //        if (child.Name == TechnicalDocument.scaleFactorValueTag)
        //        {
        //            this.myListView_MLV.Items[itemNb].SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Tag = System.Convert.ToDouble(child.InnerText);
        //        }                    
        //        if (child.Name == TechnicalDocument.formatTag)
        //        {
        //            this.myListView_MLV.Items[itemNb].SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Text = child.InnerText;
        //            this.generikPaperList_CBX.Text = child.InnerText;
        //            this.generikPaperList_CBX.SelectedItem = child.InnerText;
        //        }
        //        if (child.Name == TechnicalDocument.formatIDTag)
        //        { 
        //            var oPmzn = (object)Convert.ChangeType(child.InnerText, typeof(object));                      

        //            this.myListView_MLV.Items[itemNb].SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Tag = (System.Printing.PageMediaSizeName)oPmzn;
        //        }
        //        if (child.Name == TechnicalDocument.orientationTag)
        //        {
        //            this.myListView_MLV.Items[itemNb].SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Text = child.InnerText;
        //        }
        //        if (child.Name == TechnicalDocument.orientationIDTag)
        //        {
        //            var oPo = (object)Convert.ChangeType(child.InnerText, typeof(object));                       
        //            this.myListView_MLV.Items[itemNb].SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Tag = (System.Printing.PageOrientation)oPo;
        //        }
        //    }
        //    itemNb++;
        //} 

        //UpdateDocumentListFromListView();;
        #endregion

        private void LoadCustomInfoOrCalculateBestFormat()
        {      
            string xmlCustomInfo = this._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(TechnicalDocument.technicalDocumentsTag);
            _technicalDocument.ReadFromXml(xmlCustomInfo, out List<TechnicalDocument> documentList);

            if (documentList.Count > 0)
            { 
                this.SetGenerikPaperCombobox();

                List<int> elevationIds = new List<int>();
                List<int> wallIds = this.GetWallID();
                List<int> symbolIds = this.GetElevSymbolID();

                elevationIds.Add(KD.Const.UnknownId); //id for the top view (-1)
                elevationIds.AddRange(wallIds);
                elevationIds.AddRange(symbolIds);

                bool bFindDoc = false;
                foreach (ListViewItem lvItem in myListView_MLV.Items)
                {
                    foreach (TechnicalDocument doc in documentList)
                    {
                        for (int ids = 0; ids < elevationIds.Count; ids++)
                        {
                            if (doc.ObjectID == elevationIds[ids])
                            {
                                this.UpdateListViewItemFromDocument(doc, lvItem);
                                elevationIds.RemoveAt(ids);
                                bFindDoc = true;
                                break;
                            }
                        }
                        if (bFindDoc)
                        {
                            bFindDoc = false;
                            break;
                        }
                        else
                        {
                            this.CalculateBestFormat(lvItem);
                        }
                    }
                }                
            }
            else
            {
                foreach (ListViewItem lvItem in myListView_MLV.Items)
                {
                    this.CalculateBestFormat(lvItem);
                }
            }
        }    
        private void SaveCustomInfo()
        {            
            _technicalDocument.WriteToXml(this._documentList, out XmlNode xmlNode);
            string xmlCustomInfo = xmlNode.OuterXml;
            
            this._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(xmlCustomInfo, TechnicalDocument.technicalDocumentsTag);        

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
        private void CalculateBestFormat(ListViewItem lvItem)
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
            List<int> wallIds = this.GetWallID();
            List<int> symbolIds = this.GetElevSymbolID();

            //elevationIds.Add(KD.Const.UnknownId); //id for the top view (-1)
            elevationIds.AddRange(wallIds);
            elevationIds.AddRange(symbolIds);

            double sceneDimensionX = System.Convert.ToDouble(this._plugin.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DIMX));
            double sceneDimensionY = System.Convert.ToDouble(this._plugin.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DIMY));

            Dictionary<int, double> elevationDimensionXDict = new Dictionary<int, double>();
            Dictionary<int, double> elevationDimensionYDict = new Dictionary<int, double>();
            for (int ids = 0; ids < elevationIds.Count; ids++)
            {
                elevationDimensionXDict.Add(elevationIds[ids], System.Convert.ToDouble(this._plugin.CurrentAppli.Scene.ObjectGetInfo(elevationIds[ids], KD.SDK.SceneEnum.ObjectInfo.DIMX)));
                elevationDimensionYDict.Add(elevationIds[ids], System.Convert.ToDouble(this._plugin.CurrentAppli.Scene.ObjectGetInfo(elevationIds[ids], KD.SDK.SceneEnum.ObjectInfo.DIMZ)));
            }
                      
            double dimensionX = -1.0;
            double dimensionY = -1.0;

            double proportionX_1_20 = 0;
            double proportionY_1_20 = 0;
            double proportionX_1_50 = 0;
            double proportionY_1_50 = 0;

            if (lvItem.Index == 0)
            {
                dimensionX = sceneDimensionX;
                dimensionY = sceneDimensionY;                   
            }
            else if (lvItem.Index > 0)
            {
                TechnicalDocument infoTag = (TechnicalDocument)lvItem.Tag;
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
                proportionX_1_50 = dimensionX * SubItemsConst.scaleFactor1_50; // 0.02
                proportionY_1_50 = dimensionY * SubItemsConst.scaleFactor1_50; // 0.02

                // 1_20
                if (paperA3LWidth >= proportionX_1_20 && paperA3LHeight >= proportionY_1_20)
                {
                    this.UpdateListViewItemFromCalculate(lvItem, SubItemsConst.scaleFactor1_20, System.Printing.PageMediaSizeName.ISOA3, System.Printing.PageOrientation.Landscape);
                }
                else if (paperA4LWidth >= proportionX_1_20 && paperA4LHeight >= proportionY_1_20)
                {
                    this.UpdateListViewItemFromCalculate(lvItem, SubItemsConst.scaleFactor1_20, System.Printing.PageMediaSizeName.ISOA4, System.Printing.PageOrientation.Landscape);
                }
                else if (paperA3PWidth >= proportionX_1_20 && paperA3PHeight >= proportionY_1_20)
                {
                    this.UpdateListViewItemFromCalculate(lvItem, SubItemsConst.scaleFactor1_20, System.Printing.PageMediaSizeName.ISOA3, System.Printing.PageOrientation.Portrait);
                }
                else if (paperA4PWidth >= proportionX_1_20 && paperA4PHeight >= proportionY_1_20)
                {
                    this.UpdateListViewItemFromCalculate(lvItem, SubItemsConst.scaleFactor1_20, System.Printing.PageMediaSizeName.ISOA4, System.Printing.PageOrientation.Portrait);
                }
                // 1_50
                else if (paperA3LWidth >= proportionX_1_50 && paperA3LHeight >= proportionY_1_50)
                {
                    this.UpdateListViewItemFromCalculate(lvItem, SubItemsConst.scaleFactor1_50, System.Printing.PageMediaSizeName.ISOA3, System.Printing.PageOrientation.Landscape);
                }
                else if (paperA4LWidth >= proportionX_1_50 && paperA4LHeight >= proportionY_1_50)
                {
                    this.UpdateListViewItemFromCalculate(lvItem, SubItemsConst.scaleFactor1_50, System.Printing.PageMediaSizeName.ISOA4, System.Printing.PageOrientation.Landscape);
                }
                else if (paperA3PWidth >= proportionX_1_50 && paperA3PHeight >= proportionY_1_50)
                {
                    this.UpdateListViewItemFromCalculate(lvItem, SubItemsConst.scaleFactor1_50, System.Printing.PageMediaSizeName.ISOA3, System.Printing.PageOrientation.Portrait);
                }
                else if (paperA4PWidth >= proportionX_1_50 && paperA4PHeight >= proportionY_1_50)
                {
                    this.UpdateListViewItemFromCalculate(lvItem, SubItemsConst.scaleFactor1_50, System.Printing.PageMediaSizeName.ISOA4, System.Printing.PageOrientation.Portrait);
                }
                else
                {
                    this.UpdateListViewItemFromCalculate(lvItem, SubItemsConst.scaleFactorAuto, System.Printing.PageMediaSizeName.ISOA3, System.Printing.PageOrientation.Landscape);
                }
            }
        }
        private void UpdateListViewItemFromCalculate(ListViewItem lvItem, double scale, System.Printing.PageMediaSizeName pageMediaSizeName, System.Printing.PageOrientation pageOrientation)
        {
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Text = new ScaleFactorSubItem(scale).ToString();
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Tag = scale;

            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Text = pageMediaSizeName.ToString();
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Tag = pageMediaSizeName;

            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Text = pageOrientation.ToString();
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Tag = pageOrientation;

            generikPaperSizeText = pageMediaSizeName.ToString();
            this.SetGenerikPaperCombobox();
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
            generikPaperList_CBX.Items.Add(System.Printing.PageMediaSizeName.ISOA4);
            generikPaperList_CBX.Items.Add(System.Printing.PageMediaSizeName.ISOA3);

            generikPaperList_CBX.Text = System.Printing.PageMediaSizeName.ISOA4.ToString();
        }
        private void ScaleItems(ComboBox comboBox)
        {
            comboBox.Items.Add(ScaleFactorSubItem.Dico[SubItemsConst.scaleFactor1_20]);
            comboBox.Items.Add(ScaleFactorSubItem.Dico[SubItemsConst.scaleFactor1_50]);
            comboBox.Items.Add(ScaleFactorSubItem.Dico[SubItemsConst.scaleFactorAuto]);
        }
        private void PaperItems(ComboBox comboBox)
        {
            comboBox.Items.Add(System.Printing.PageMediaSizeName.ISOA4);
            comboBox.Items.Add(System.Printing.PageMediaSizeName.ISOA3);
        }
        private void OrientationItems(ComboBox comboBox)
        {
            comboBox.Items.Add(System.Printing.PageOrientation.Portrait);
            comboBox.Items.Add(System.Printing.PageOrientation.Landscape);  
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
        private string GetDocNameFromListViewItem(ListViewItem lvItem)
        {
            if (lvItem == null)
            {
                return String.Empty;
            }
            return lvItem.Tag.ToString();
        }
      
        private void SaveDrawingConfig()
        {
            drawingSurface2D = this._plugin.CurrentAppli.Scene.SceneGetInfo(KD.SDK.SceneEnum.SceneInfo.DRAWING_SURFACE_2D);
        }
        private void SetDrawingConfig()
        {
            this._plugin.CurrentAppli.Scene.SceneSetInfo("0", KD.SDK.SceneEnum.SceneInfo.DRAWING_SURFACE_2D);
        }
        private void RestoreDrawingConfig()
        {
            if (!String.IsNullOrEmpty(drawingSurface2D))
            {
                this._plugin.CurrentAppli.Scene.SceneSetInfo(drawingSurface2D, KD.SDK.SceneEnum.SceneInfo.DRAWING_SURFACE_2D);
            }
        }

        //
        private void InitializeDocumentList()
        {
            this._documentList.Clear();

            this.AddDocTopViewToDocumentList();
            this.AddDocElevationViewToDocumentList(GetWallID());
            this.AddDocElevationViewToDocumentList(GetElevSymbolID());
        }
        private List<int> GetWallID()
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
        private List<int> GetElevSymbolID()
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
            docTopView.Type = TechnicalDocument.Dico[(int)KD.SDK.SceneEnum.ViewMode.TOP];
            docTopView.ScaleFactorAsString = new ScaleFactorSubItem(SubItemsConst.scaleFactor1_20).ToString();
            docTopView.ScaleFactorValue = SubItemsConst.scaleFactor1_20; 
            docTopView.PageMediaSizeName = System.Printing.PageMediaSizeName.ISOA4;
            docTopView.PageMediaSizeNameID = (int)System.Printing.PageMediaSizeName.ISOA4;
            docTopView.PageOrientation = System.Printing.PageOrientation.Portrait;
            docTopView.PageOrientationID = (int)System.Printing.PageOrientation.Portrait;
            docTopView.ViewMode = KD.SDK.SceneEnum.ViewMode.TOP;
            docTopView.ObjectID = KD.Const.UnknownId;
            docTopView.Number = KD.Const.UnknownId.ToString(); 
            _documentList.Add(docTopView);
        }
        private void AddDocElevationViewToDocumentList(List<int> elevationListIds)
        {
            for (int iElevation = 0; iElevation < elevationListIds.Count; iElevation++)
            {
                TechnicalDocument docElevationView = new TechnicalDocument();
                docElevationView.Type = TechnicalDocument.Dico[(int)KD.SDK.SceneEnum.ViewMode.VECTELEVATION];
                docElevationView.ScaleFactorAsString = new ScaleFactorSubItem(SubItemsConst.scaleFactor1_20).ToString();
                docElevationView.ScaleFactorValue = SubItemsConst.scaleFactor1_20;
                docElevationView.PageMediaSizeName = System.Printing.PageMediaSizeName.ISOA4;
                docElevationView.PageMediaSizeNameID = (int)System.Printing.PageMediaSizeName.ISOA4;
                docElevationView.PageOrientation = System.Printing.PageOrientation.Portrait;
                docElevationView.PageOrientationID = (int)System.Printing.PageOrientation.Portrait;
                docElevationView.ViewMode = KD.SDK.SceneEnum.ViewMode.VECTELEVATION;
                docElevationView.ObjectID = elevationListIds[iElevation];
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
                        return doc.ToSymbol();
                    }
                }
            }
            return doc.ToString();
        }


        public void UpdateListViewFromDocumentList()
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
            this.myListView_MLV.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            int columnHeaderSize = 0;
            foreach (ColumnHeader ch in this.myListView_MLV.Columns)
            {
                this.PaperColumnWidthToZero(ch);
                columnHeaderSize += ch.Width;
            }
            this.splitContainer_SPC.SplitterDistance = this.splitContainer_SPC.Location.X + columnHeaderSize;
            this.myListView_MLV.Refresh();
        }
        private void UpdateListViewItemFromDocument(TechnicalDocument doc, ListViewItem lvItem)
        {
            string viewName = this.GetElevationViewName(doc);

            lvItem.Tag = doc;

            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Select].Text = String.Empty; // selected case with any string
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Select].Tag = String.Empty; // selected case with any string

            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.View].Text = viewName; //Select between 'Top view', 'Wall view' and 'Symbol view'
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.View].Tag = doc;

            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Text = doc.ScaleFactorAsString;
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Tag = doc.ScaleFactorValue;//new ScaleFactorSubItem(doc.ScaleFactor)

            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Text = doc.PageMediaSizeName.ToString();
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Tag = doc.PageMediaSizeName;

            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Text = doc.PageOrientation.ToString();
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Tag = doc.PageOrientation;

            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Overview].Text = String.Empty; // Preview button. It replaced by a button
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Overview].Tag = String.Empty; // Preview button. It replaced by a button
        }

        private void UpdateDocumentListFromListView()
        {           
            this._documentList.Clear();
            this.myListView_MLV.Update();
            foreach (ListViewItem lvItem in this.myListView_MLV.Items)
            {                
                this.UpdateDocumentFromListViewSubItem(lvItem);
                this._documentList.Add((TechnicalDocument)lvItem.Tag);                
            }
        }     
        private void UpdateDocumentFromListViewSubItem(ListViewItem lvItem)
        {           
            TechnicalDocument doc = (TechnicalDocument)lvItem.Tag;
           
            doc.ScaleFactorAsString = lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Text;
            doc.ScaleFactorValue = (double)lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Tag; 

            doc.PageMediaSizeName = (System.Printing.PageMediaSizeName)lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Tag;
            PageMediaSizeNameSubItem pmsnInt = new PageMediaSizeNameSubItem(doc.PageMediaSizeName);
            doc.PageMediaSizeNameID = pmsnInt.ToInt();

            doc.PageOrientation = (System.Printing.PageOrientation)lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Tag;
            PageOrientationSubItem poInt = new PageOrientationSubItem(doc.PageOrientation);
            doc.PageOrientationID = poInt.ToInt();           
                       
            lvItem.Tag = doc;

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
            foreach (ListViewItem lvItem in myListView_MLV.Items)
            {
                if (lvItem.Checked)
                {
                    this.Build(false, lvItem.Index);
                }
            }
        }
        private void SelectOrDeselectAllListViewItem()
        {
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
            }
        }
        private void DisableTechnicalPlanFileNameForm()
        {
            _technicalPlanDocumentFileNameForm = null;
        }

        // Form
        private void GenerateViewDialogForm_Load(object sender, EventArgs e)
        {
            this.InitializePreviewDirectory();
            this.InitializeListView();
            this.ClearComboBoxListView();
            this.AssignItemsInComboBox();
            this.InitializeDocumentList();
            
            this.UpdateListViewFromDocumentList();
        }
        private void GenerateViewDialogForm_Shown(object sender, EventArgs e)
        {
            this.AddTwinButton();

            //Load info from scene 
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
            if (lvItem != null && lviSelectedRowIndex != -1 && lviSelectedColumnIndex != -1)
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
            if (lvItem != null && e.Item.Index != -1 && lvItem.Index != -1)
            {
                if (lvItem.Checked)
                {                                   
                    lviSelectedRowIndex = lvItem.Index;                    
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

            this.UpdateDocumentFromListViewSubItem(lvItem);
            //this.UpdateDocumentListFromListView();
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

            this.UpdateDocumentFromListViewSubItem(lvItem);
            //this.UpdateDocumentListFromListView();
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
            this.HideComboBox(this.orientationList_CBX);
            this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Tag = (System.Printing.PageOrientation)comboBox.SelectedItem;

            this.UpdateDocumentFromListViewSubItem(lvItem);
            //this.UpdateDocumentListFromListView();
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
                this.SaveDrawingConfig();
                this.SetDrawingConfig();
                this.Build(true, lviSelectedRowIndex);
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
            this.Close();
        }
        private void Ok_BTN_Click(object sender, EventArgs e)
        {
            this.InitializeTechnicalPlanFileNameForm();
            this.ShowTechnicalPlanFileNameForm();
            this.DialogAnswerToSM2();
            this.DisableTechnicalPlanFileNameForm();

            this.DesactivatePdfViewer();
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

                this.UpdateDocumentFromListViewSubItem(lvItem);
            }
            //this.UpdateDocumentListFromListView();
        }
        private void generikPaperList_CBX_TextChanged(object sender, EventArgs e)
        {
            generikPaperSizeText = generikPaperList_CBX.Text;
            this.SetGenerikPaperCombobox();
        }

        private void openFolder_BTN_Click(object sender, EventArgs e)
        {
            string dir = this.ArchiveFileDir;
            if (System.IO.Directory.Exists(System.IO.Path.Combine(this.ArchiveFileDir, ConstFile.DocTechnicalPlanDir)))
            {
                dir = (System.IO.Path.Combine(this.ArchiveFileDir, ConstFile.DocTechnicalPlanDir));
            }
            KD.IO.Explorer.ShowDir(dir);                    
        }

        private void print_BTN_Click(object sender, EventArgs e)
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
                        string docname = this._pluginWord.GetLocalizedDocName(this._pluginWord.DocIndex2Use);//GetDocNameFromListViewItem(lvItem);
                        string pdfFilePath = this._pluginWord.GetArchiveFilePath(docname, KD.IO.File.Extension.Pdf);

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

    public class SubItemsConst
    {
        public const double scaleFactorUnknown = -1.0;
        public const double scaleFactor1_20 = 0.05;
        public const double scaleFactor1_50 = 0.02;
        public const double scaleFactorAuto = 0.0;

        public const string scaleFactorString1_20 = "1/20";
        public const string scaleFactorString1_50 = "1/50";
        public const string scaleFactorStringAuto = "Auto";
       
        public const string pageOrientationStringPortrait = "Portrait";
        public const string pageOrientationStringLandScape = "Landscape";

        public const string pageMediaSizeNameStringISOA4 = "ISOA4";
        public const string pageMediaSizeNameStringISOA3 = "ISOA3";
    }

    public class ConstFile
    {
        public const string TechnicalPlanPreviewDirName = "TECHNICAL_PLAN_PREVIEW";
        public const string DocTechnicalPlanDir = "DocTechnicalPlan";
    }

    public class ConstCustomName
    {
        public const string ScaleID = "_ScaleID";
        public const string PaperID = "_PaperID";
        public const string OrientationID = "_OrientationID";
    }

    public class ConstComboBox
    {
        public const string PartialScaleName = "SCALE";
        public const string PartialPaperName = "PAPER";
        public const string PartialOrientationName = "ORIENTATION";

    }

 

    //UI
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
                return(SubItemsConst.scaleFactorString1_20);
            }
            else if (_scaleFactor == SubItemsConst.scaleFactor1_50)

            {
                return (SubItemsConst.scaleFactorString1_50);
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
            if (_scaleFactorString == SubItemsConst.scaleFactorString1_20)
            {
                return (SubItemsConst.scaleFactor1_20);
            }
            else if (_scaleFactorString == SubItemsConst.scaleFactorString1_50)
            {
                return (SubItemsConst.scaleFactor1_50);
            }
            else if (_scaleFactorString == SubItemsConst.scaleFactorStringAuto)
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
            if (_pageMediaSizeNameString == SubItemsConst.pageMediaSizeNameStringISOA4)
            {
                return System.Printing.PageMediaSizeName.ISOA4;
            }
            else if (_pageMediaSizeNameString == SubItemsConst.pageMediaSizeNameStringISOA3)
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
            if (_pageOrientationString == SubItemsConst.pageOrientationStringPortrait)
            {
                return System.Printing.PageOrientation.Portrait;
            }
            else if (_pageOrientationString == SubItemsConst.pageOrientationStringLandScape)
            {
                return System.Printing.PageOrientation.Landscape;
            }

            return System.Printing.PageOrientation.Unknown;
        }
    }

}
