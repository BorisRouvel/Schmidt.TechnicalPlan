using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;


namespace Schmidt.TechnicalPlan
{

    public partial class GenerateViewDialogForm : Form
    {
        const string xmlInfoKey = "TechnicalDocuments";      

        List<TechnicalDocument> _documentList = new List<TechnicalDocument>();

        PageOrientationSubItem portrait = new PageOrientationSubItem(System.Printing.PageOrientation.Portrait);
        PageOrientationSubItem paysage = new PageOrientationSubItem(System.Printing.PageOrientation.Landscape);
        PageOrientationSubItem pageOrientationChoice = new PageOrientationSubItem(System.Printing.PageOrientation.Portrait);

        PageMediaSizeNameSubItem A4 = new PageMediaSizeNameSubItem(System.Printing.PageMediaSizeName.ISOA4);
        PageMediaSizeNameSubItem A3 = new PageMediaSizeNameSubItem(System.Printing.PageMediaSizeName.ISOA3);
        PageMediaSizeNameSubItem pageMediaSizeNameChoice = new PageMediaSizeNameSubItem(System.Printing.PageMediaSizeName.ISOA4);

        ScaleFactorSubItem scale120 = new ScaleFactorSubItem(ScaleFactorSubItem.ScaleFactorEnum.Scale120);
        ScaleFactorSubItem scale150 = new ScaleFactorSubItem(ScaleFactorSubItem.ScaleFactorEnum.Scale150);
        ScaleFactorSubItem scaleAuto = new ScaleFactorSubItem(ScaleFactorSubItem.ScaleFactorEnum.ScaleAuto);
        ScaleFactorSubItem scaleFactorChoice = new ScaleFactorSubItem(ScaleFactorSubItem.ScaleFactorEnum.Scale120);
        //
        private KD.Plugin.Word.Plugin _pluginWord = null;
        private Dico _dico = null;
        private TechnicalDocument _technicalDocument = null;
        private MyImageButton myOverViewButton = null;

        const int InitialZoomValue = 99;
        private double _zoom = 99;
        Syncfusion.Pdf.Parsing.PdfLoadedDocument _loadedDocument = null;

        private ListViewItem lvItem = null;

        private int lviSelectedRowIndex = -1;
        private int lviSelectedColumnIndex = (int)MyListView.Enum.ColumnIndex.UnKnown;

        private Plugin _plugin;
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

        private string _currentPdfFilePath;
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
            string archiveFileDir = this._pluginWord.GetRootDir();
            if (!archiveFileDir.Contains(ConstFile.DocTechnicalPlanDir))
            {
                technicalPlanDir = System.IO.Path.Combine(archiveFileDir, ConstFile.DocTechnicalPlanDir, ConstFile.TechnicalPlanPreviewDirName);
            }
            else
            {
                technicalPlanDir = System.IO.Path.Combine(archiveFileDir, ConstFile.TechnicalPlanPreviewDirName);
            }
          

            if (System.IO.Directory.Exists(technicalPlanDir))
            {
                IEnumerable<string> technicalPlanFiles = System.IO.Directory.EnumerateFiles(technicalPlanDir);
                foreach (string technicalPlanFile in technicalPlanFiles)
                {
                    if (System.IO.File.Exists(technicalPlanFile))
                    {
                        System.IO.File.Delete(technicalPlanFile);
                    }
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
            header.Add(this._dico.GetTranslate(TranslateIdentifyId.ColumnHeaderOverviewID));//Plugin.pluginWord.CurrentAppli.GetTranslatedText
            header.ForEach(name => this.myListView_MLV.Columns.Add(name));

            // Loop through and size each column header to fit the column header text.
            foreach (ColumnHeader ch in this.myListView_MLV.Columns)
            {
                ch.Width = -2;
            }
        }
        private void InitializeForm()
        {
            this.Ok_BTN.Text = _dico.GetTranslate(TranslateIdentifyId.OkButtonUIID);
            this.Cancel_BTN.Text = _dico.GetTranslate(TranslateIdentifyId.CancelButtonUIID);
        }
     
        public void Build(bool preview, int lviIndex)
        {
            Cursor.Current = Cursors.WaitCursor;

            this._pluginWord.DocIndex2Use = FindDocNameFromChoice(lviIndex);
            this._pluginWord.GenerateDocument(false);

            string docname = this._pluginWord.GetLocalizedDocName(this._pluginWord.DocIndex2Use);
            string pdfFilePath = this._pluginWord.GetArchiveFilePath(docname, KD.IO.File.Extension.Pdf);
            string dotFilePath = this._pluginWord.GetArchiveFilePath(docname, KD.IO.File.Extension.Dot);

            string dirPdfPath = this.MoveFileToDocPlanDir(pdfFilePath, dotFilePath, 0);
            string dirDotPath = dirPdfPath.Replace(KD.IO.File.Extension.Pdf, KD.IO.File.Extension.Dot);

            if (preview)
            {
                this.LoadPdfFile(this.MoveFileToPreviewDir(dirPdfPath, dirDotPath, 0));
            }

            Cursor.Current = Cursors.Arrow;
        }
        private string MoveFileToDocPlanDir(string pdfFilePath, string dotFilePath, int time)
        {
            string currentDir = System.IO.Path.GetDirectoryName(pdfFilePath);
            if (!currentDir.Contains(ConstFile.DocTechnicalPlanDir))
            {
                string currentPdfDir = System.IO.Path.Combine(currentDir, ConstFile.DocTechnicalPlanDir);
                if (!System.IO.Directory.Exists(currentPdfDir))
                {                
                    System.IO.Directory.CreateDirectory(currentPdfDir);
                }
              
                string newPdfFilePath = System.IO.Path.Combine(currentPdfDir, System.IO.Path.GetFileName(pdfFilePath));
                string newDotFilePath = System.IO.Path.Combine(currentPdfDir, System.IO.Path.GetFileName(dotFilePath));

                try
                {
                    System.Windows.Forms.Application.DoEvents();
                    System.Threading.Thread.Sleep(500);
                   
                    if (System.IO.File.Exists(pdfFilePath))
                    {
                        System.IO.File.Move(pdfFilePath, newPdfFilePath);
                    }
                    else
                    {
                        if (time <= 5)
                        {
                            MoveFileToDocPlanDir(pdfFilePath, dotFilePath, time++);
                        }
                        else
                        {
                            time = 0;
                            return pdfFilePath;
                        }
                    }
                    if (System.IO.File.Exists(dotFilePath))
                    {
                        System.IO.File.Move(dotFilePath, newDotFilePath);
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
            if (!System.IO.Directory.Exists(System.IO.Path.Combine(currentDir, ConstFile.TechnicalPlanPreviewDirName)))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(currentDir, ConstFile.TechnicalPlanPreviewDirName));
            }
            string newDir = System.IO.Path.Combine(currentDir, ConstFile.TechnicalPlanPreviewDirName);
            string previewPdfFilePath = System.IO.Path.Combine(newDir, System.IO.Path.GetFileName(pdfFilePath));
            string previewDotFilePath = System.IO.Path.Combine(newDir, System.IO.Path.GetFileName(dotFilePath));

            try
            {
                System.Windows.Forms.Application.DoEvents();
                System.Threading.Thread.Sleep(500);
                if (System.IO.File.Exists(pdfFilePath))
                {
                    System.IO.File.Move(pdfFilePath, previewPdfFilePath);
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
                    System.IO.File.Move(dotFilePath, previewDotFilePath);
                }

                return previewPdfFilePath;
            }
            catch (Exception)
            {
                return pdfFilePath;
            }
        }
        public void LoadPdfFile(string pdfFilePath)
        {

            //this.ActivatePdfViewer();
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
        private void UnLoadPdfFile()
        {
            pdfDocumentView_PDFV.Unload();
        }
        private void ActivatePdfViewer()
        {
            this.splitContainer_SPC.Panel2.Show();
            this.pdfDocumentView_PDFV.Enabled = true;
            this.pdfDocumentView_PDFV.Visible = true;
            return;
        }
        private int FindDocNameFromChoice(int iDoc)
        {
            int rank = 0;

            if (lvItem != null && lvItem.Index == iDoc)
            {
                string scale = lvItem.SubItems[2].Text.Replace(KD.StringTools.Const.Slatch, String.Empty);
                string subDocName = scale + KD.StringTools.Const.MinusSign + lvItem.SubItems[3].Text + KD.StringTools.Const.MinusSign + lvItem.SubItems[4].Text;      //subDocList[2].Tag.ToString();

                //int nbSubDoc = this._pluginWord.CurrentAppli.GetDocItemsNb((iDoc * 12) + iDoc);
                int nbSubDoc = this._pluginWord.CurrentAppli.GetDocsNb();

                for (int iSubDoc = 1; iSubDoc <= nbSubDoc; iSubDoc++)
                {
                    //rank = (iSubDoc + (iDoc * 12) + iDoc);
                    rank = iSubDoc;
                    string name = this._pluginWord.CurrentAppli.DocGetInfo(rank, KD.SDK.AppliEnum.DocInfo.NAME);

                    if (name.ToUpper().Contains(subDocName.ToUpper()))
                    {
                        break;
                    }
                }
            }
            return rank;
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
        private void SetComboBoxTagAndTextInItem(ComboBox comboBox)
        {
            this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[lviSelectedColumnIndex].Tag = comboBox.Tag;
            this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[lviSelectedColumnIndex].Text = comboBox.Text;
        }
        private void SetItemTextInComboBox(ComboBox comboBox)
        {            
            comboBox.Text = this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[lviSelectedColumnIndex].Text;
        }      

        private void LoadCustomInfo()//ComboBox comboBox, string customID
        {
            //string customId = this._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(lvItem.Tag.ToString() + customID);
            //if (!String.IsNullOrEmpty(customId))
            //{
            //    string[] custom = customId.Split(KD.CharTools.Const.SemiColon);
            //    if (custom.Length == 3)
            //    {
            //        if (custom[0] != lvItem.Tag.ToString())
            //        {
            //            return;
            //        }

            //        lviSelectedRowIndex = lvItem.Index;

            //        int.TryParse(custom[1], out int columnIndex);
            //        lviSelectedColumnIndex = columnIndex;

            //        int.TryParse(custom[2], out int selectedIndex);
            //        comboBox.SelectedIndex = selectedIndex;

            //        comboBox.Text = comboBox.SelectedItem.ToString();
            //        this.SetTextInItem(comboBox);
            //    }
            //}
            string xmlCustomInfo = this._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(xmlInfoKey);
            _technicalDocument.ReadFromXml(xmlCustomInfo, out XmlNodeList xmlNodeScaleList, out XmlNodeList xmlNodeFormatList, out XmlNodeList xmlNodeOrientationList);
            ;
        }
        //private void SaveCustomInfo(ComboBox comboBox, string customID)
        //{
        //    //vue de dessus ; 2 ; 1
        //    //vue de dessus_SCALEID
        //    string saveString = String.Empty; // lvItem.Tag.ToString() + KD.StringTools.Const.SemiColon + lviSelectedColumnIndex + KD.StringTools.Const.SemiColon + comboBox.SelectedIndex.ToString();
        //    string InfoKey = lvItem.Tag.ToString() + customID;
        //    this._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(saveString, InfoKey);
        //}
        private void SaveCustomInfo(string filePath)
        {
            _technicalDocument = new TechnicalDocument(filePath);
            _technicalDocument.WriteToXml(this._documentList, out XmlNode xmlNode);
            string xmlCustomInfo = xmlNode.OuterXml;
            
            this._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(xmlCustomInfo, xmlInfoKey);        

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
        }
        private void ScaleItems(ComboBox comboBox)
        {
            comboBox.Items.Add(ScaleFactorSubItem.Dico[(int)ScaleFactorSubItem.ScaleFactorEnum.Scale120]);   //scale120        
            comboBox.Items.Add(ScaleFactorSubItem.Dico[(int)ScaleFactorSubItem.ScaleFactorEnum.Scale150]);//scale150
            comboBox.Items.Add(ScaleFactorSubItem.Dico[(int)ScaleFactorSubItem.ScaleFactorEnum.ScaleAuto]);//scaleAuto
        }
        private void PaperItems(ComboBox comboBox)
        {
            comboBox.Items.Add(System.Printing.PageMediaSizeName.ISOA4);//A4
            comboBox.Items.Add(System.Printing.PageMediaSizeName.ISOA3);//A3
        }
        private void OrientationItems(ComboBox comboBox)
        {
            comboBox.Items.Add(System.Printing.PageOrientation.Portrait);//portrait
            comboBox.Items.Add(System.Printing.PageOrientation.Landscape);//paysage                                  
           
        }

        private ListViewItem FindListViewItemFromDocName(string docName)
        {
            ListViewItem lvi = null;

            for (int iDoc = 0; iDoc < myListView_MLV.Items.Count; iDoc++)
            {
                lvi = myListView_MLV.Items[iDoc];
                if (this.GetDocNameFromListViewItem(lvi) == docName)
                {
                    return lvi;
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
        //public void UpdateListViewStandardInfo()
        //{
            //int index = 0;

            //int nbDoc = _pluginWord.CurrentAppli.GetDocsNb();
            //for (int iDoc = 0; iDoc < nbDoc; iDoc++)
            //{
            //    KD.SDK.AppliEnum.DocType doctype = _pluginWord.CurrentAppli.DocGetType(iDoc);

            //    #region// Supplier Order
            //    //if (this.SupplierIds.Count > 0)
            //    //{
            //    //if (doctype != KD.SDK.AppliEnum.DocType.SupplierOrder)
            //    //{
            //    //    continue;
            //    //}
            //    //if (this.Plugin.DocIndex2Use != iDoc)
            //    //{
            //    //    continue;
            //    //}
            //    //foreach (string supplierId in this.SupplierIds)
            //    //{
            //    //    string docName = supplierId;
            //    //    if (FindListViewItemFromDocName(docName) == null)
            //    //    {
            //    //        this.BindDocName2DocIndex(docName, iDoc);
            //    //        this.AddListViewItem(docName, iDoc);
            //    //    }
            //    //    UpdateListViewItemImageIndex(docName);
            //    //}
            //    //// the loop is different for orders, not on nbDoc, it's based on SupplierIds
            //    //break;
            //    //}
            //    //else
            //    //{
            //    #endregion
            //    if (doctype != KD.SDK.AppliEnum.DocType.Commercial && doctype != KD.SDK.AppliEnum.DocType.Management)
            //    {
            //        continue;
            //    }

            //    string docName = this._pluginWord.CurrentAppli.DocGetInfo(iDoc, KD.SDK.AppliEnum.DocInfo.NAME);
            //    string translateDocName = this._dico.GetTranslate(docName);
            //    AddListViewItem(translateDocName, iDoc, index++);
            //}           

        //}
        //public void UpdateListViewCustomInfo()
        //{
        //    int nbDoc = _pluginWord.CurrentAppli.GetDocsNb();
        //    for (int iDoc = 0; iDoc < nbDoc; iDoc++)
        //    {
        //        string docName = this._pluginWord.GetLocalizedDocName(iDoc);
        //        lvItem = this.FindListViewItemFromDocName(docName);

        //        if (lvItem != null)
        //        {
        //            foreach (Control control in splitContainer_SPC.Panel1.Controls)
        //            {
        //                if (control.GetType().Equals(typeof(ComboBox)))
        //                {
        //                    ComboBox comboBox = (ComboBox)control;
        //                    if (comboBox.Name.ToUpper().Contains(ConstComboBox.PartialScaleName))
        //                    {
        //                        this.LoadCustomInfo(comboBox, ConstCustomName.ScaleID);
        //                    }
        //                    if (comboBox.Name.ToUpper().Contains(ConstComboBox.PartialPaperName))
        //                    {
        //                        this.LoadCustomInfo(comboBox, ConstCustomName.PaperID);
        //                    }
        //                    if (comboBox.Name.ToUpper().Contains(ConstComboBox.PartialOrientationName))
        //                    {
        //                        this.LoadCustomInfo(comboBox, ConstCustomName.OrientationID);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //private void UpdateFormatToDocumentList()
        //{
        //    foreach (ListViewItem listViewItem in this.myListView_MLV.Items)
        //    {
        //        _documentList[listViewItem.Index].ScaleFactor = (double)listViewItem.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Tag; //.ToString();
        //        _documentList[listViewItem.Index].PageMediaSizeName = (System.Printing.PageMediaSizeName)listViewItem.SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Tag;
        //        _documentList[listViewItem.Index].PageOrientation = (System.Printing.PageOrientation)listViewItem.SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Tag;
        //    }
        //}
        //
        private void InitializeDocumentList()
        {
            this._documentList.Clear();

            this.AddDocTopViewToDocumentList();
            this.AddDocWallViewToDocumentList(GetWallID());
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
        private void AddDocTopViewToDocumentList()
        {
            TechnicalDocument docTopView = new TechnicalDocument();
            docTopView.Type = TechnicalDocument.Dico[(int)KD.SDK.SceneEnum.ViewMode.TOP];
            docTopView.ScaleFactor = ScaleFactorSubItem.Dico[(int)ScaleFactorSubItem.ScaleFactorEnum.Scale120]; //1.0 / 20.0; //
            docTopView.PageMediaSizeName = System.Printing.PageMediaSizeName.ISOA4;
            docTopView.PageOrientation = System.Printing.PageOrientation.Portrait;           
            docTopView.ViewMode = KD.SDK.SceneEnum.ViewMode.TOP;
            docTopView.ObjectID = KD.Const.UnknownId;
            docTopView.Marq = _plugin.CurrentAppli.Scene.ObjectGetInfo(docTopView.ObjectID, KD.SDK.SceneEnum.ObjectInfo.NUMBER);
            _documentList.Add(docTopView);
        }
        private void AddDocWallViewToDocumentList(List<int> wallIds)
        {
            for (int iWall = 0; iWall < wallIds.Count; iWall++)
            {
                TechnicalDocument docWallView = new TechnicalDocument();
                docWallView.Type = TechnicalDocument.Dico[(int)KD.SDK.SceneEnum.ViewMode.VECTELEVATION];
                docWallView.ScaleFactor = ScaleFactorSubItem.Dico[(int)ScaleFactorSubItem.ScaleFactorEnum.Scale120]; //1.0 / 20.0; //
                docWallView.PageMediaSizeName = System.Printing.PageMediaSizeName.ISOA4;
                docWallView.PageOrientation = System.Printing.PageOrientation.Portrait;               
                docWallView.ViewMode = KD.SDK.SceneEnum.ViewMode.VECTELEVATION;
                docWallView.ObjectID = wallIds[iWall];
                docWallView.Marq = _plugin.CurrentAppli.Scene.ObjectGetInfo(docWallView.ObjectID, KD.SDK.SceneEnum.ObjectInfo.NUMBER);
                _documentList.Add(docWallView);
            }
        }
        
        private void UpdateListViewFromDocumentList()
        {
            int imageIndex = 0;            
            foreach (TechnicalDocument ithDoc in this._documentList)
            {
                lvItem = new ListViewItem(ithDoc.ToString(), imageIndex++);
                lvItem.Tag = ithDoc;
                lvItem.Text = MyListView.Enum.ColumnIndex.View.ToString(); //String.Empty
                lvItem.SubItems.Add(ithDoc.ToString()); //
                lvItem.SubItems.Add(ithDoc.ScaleFactor.ToString()); //scale120
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
                columnHeaderSize += ch.Width;
            }
            this.splitContainer_SPC.SplitterDistance = this.splitContainer_SPC.Location.X + columnHeaderSize;
            this.myListView_MLV.Refresh();
        }
        private void UpdateListViewItemFromDocument(TechnicalDocument doc, ListViewItem lvItem)
        {
            lvItem.Tag = doc;
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Select].Text = String.Empty; // selected case with any string
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Select].Tag = String.Empty; // selected case with any string
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.View].Text = doc.ToString();
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.View].Tag = doc;
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Text = doc.ScaleFactor.ToString();
            lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Tag = doc.ScaleFactor;
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
                this.UpdateDocumentInListViewItem(lvItem);
                this._documentList.Add((TechnicalDocument)lvItem.Tag);
            }
        }     

        private void UpdateDocumentInListViewItem( ListViewItem lvItem)
        {
            string sf = (string)lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Scale].Tag;
            System.Printing.PageMediaSizeName pmsn = (System.Printing.PageMediaSizeName)lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Paper].Tag;
            System.Printing.PageOrientation po = (System.Printing.PageOrientation)lvItem.SubItems[(int)MyListView.Enum.ColumnIndex.Orientation].Tag;           

            TechnicalDocument doc = (TechnicalDocument)lvItem.Tag;
            doc.ScaleFactor = sf; //.ToDouble(scaleFactorChoice.ScaleFactor);
            doc.PageMediaSizeName = pmsn; //.PageMediaSizeName;
            doc.PageOrientation = po; //.PageOrientation;

            lvItem.Tag = doc;          

        }



        //private void AddListViewItem(string docName, int iDoc, int index)
        //{
        //    lvItem = new ListViewItem(docName, iDoc);

        //    lvItem.SubItems.Add(docName);
        //    int nbSubDoc = _pluginWord.CurrentAppli.GetDocItemsNb(iDoc);
        //    if (nbSubDoc > 0)
        //    {
        //        // default image
        //        //lvItem.ImageIndex = Convert.ToInt32(ImageListIconsIndex.SynchronizedFalse);
        //        lvItem.Tag = docName;// index
        //                             //for (int iSubDoc = 0; iSubDoc < nbSubDoc; iSubDoc++)
        //                             //{
        //                             //    string subDocName = this.GetLocalizedDocName(iDoc + iSubDoc + 1);
        //                             //    lvItem.SubItems.Add(subDocName);
        //                             //}
        //        lvItem.SubItems.Add(this.scaleList_CBX.Items[(int)ComboBoxEnum.ScaleFactorIndex.Scale120].ToString());
        //        lvItem.SubItems.Add(this.paperList_CBX.Items[(int)ComboBoxEnum.PaperFormatIndex.A4].ToString());
        //        lvItem.SubItems.Add(this.orientationList_CBX.Items[(int)ComboBoxEnum.PageOrientationIndex.Portrait].ToString());

        //        lvItem.SubItems.Add("");
        //        // Create two ImageList objects.
        //        //ImageList imageListSmall = new ImageList();
        //        //ImageList imageListLarge = new ImageList();

        //        // Initialize the ImageList objects with bitmaps.
        //        //imageListSmall.Images.Add(Bitmap.FromFile("D:\\_Travail\\Documents Fabricants\\SCHMIDT GROUPE\\PLAN TECHNIQUE\\twin.png"));
        //        //imageListSmall.Images.Add(Bitmap.FromFile("C:\\MySmallImage2.bmp"));
        //        //imageListLarge.Images.Add(Bitmap.FromFile("C:\\MyLargeImage1.bmp"));
        //        //imageListLarge.Images.Add(Bitmap.FromFile("C:\\MyLargeImage2.bmp"));

        //        //Assign the ImageList objects to the ListView.
        //        //this.MyListView.LargeImageList = imageListLarge;
        //        //this.MyListView.SmallImageList = imageListSmall;

        //    }
        //    else
        //    {
        //        ;
        //        lvItem.Tag = docName;// ConstListView.UnknownTag;// index
        //        lvItem.ImageIndex = -1; // Convert.ToInt32(ImageListIconsIndex.SubDir);
        //        lvItem.SubItems[KD.Plugin.Word.ConstListView.ColumnIndexDate].Text = String.Empty;
        //    }
        //    this.myListView_MLV.Items.Add(lvItem);
        //    lvItem.Text = String.Empty;

        //}
       
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
            foreach (ListViewItem lvi in this.myListView_MLV.Items)
            {
                int x1 = this.myListView_MLV.Items[lvi.Index].SubItems[(int)MyListView.Enum.ColumnIndex.Overview].Bounds.X + this.myListView_MLV.Left + 10;
                int y1 = this.myListView_MLV.Items[lvi.Index].SubItems[(int)MyListView.Enum.ColumnIndex.Overview].Bounds.Y - 2;

                Control[] controls = myListView_MLV.Controls.Find("myOverViewButton" + lvi.Index, true);
                foreach (Control control in controls)
                {
                    control.Location = new Point(x1, y1);
                }
            }
        }

        // Form
        private void GenerateViewDialogForm_Load(object sender, EventArgs e)
        {
            this.InitializePreviewDirectory();
            this.InitializeListView();
            this.ClearComboBoxListView();
            this.AssignItemsInComboBox();
            this.InitializeDocumentList();
            //this.LoadCustomInfo();
            this.UpdateListViewFromDocumentList();
           
        }
        private void GenerateViewDialogForm_Shown(object sender, EventArgs e)
        {
            this.AddTwinButton();
        }
        private void GenerateViewDialogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.UnLoadPdfFile();           
            this.Plugin.viewDialogForm = null;
        }
        private void GenerateViewDialogForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.UnLoadPdfFile();
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
            this.SetComboBoxTagAndTextInItem(this.scaleList_CBX);
            this.HideComboBox(this.scaleList_CBX);
        }
        private void scaleList_CBX_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            comboBox.Tag = comboBox.SelectedItem;
            this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[lviSelectedColumnIndex].Tag = comboBox.SelectedItem;
            this.HideComboBox(this.scaleList_CBX);
            this.UpdateDocumentListFromListView();
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
            this.SetComboBoxTagAndTextInItem(this.paperList_CBX);
            this.HideComboBox(this.paperList_CBX);
        }
        private void paperList_CBX_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            comboBox.Tag = comboBox.SelectedItem; 
            this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[lviSelectedColumnIndex].Tag = comboBox.SelectedItem;
            this.HideComboBox(this.paperList_CBX);
            this.UpdateDocumentListFromListView();
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
            this.SetComboBoxTagAndTextInItem(this.orientationList_CBX);
            this.HideComboBox(this.orientationList_CBX);
        }
        private void orientationList_CBX_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;           
            comboBox.Tag = comboBox.SelectedItem;
            this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[lviSelectedColumnIndex].Tag = comboBox.SelectedItem; 
            this.HideComboBox(this.orientationList_CBX);
            this.UpdateDocumentListFromListView();           
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
                this.Build(true, lviSelectedRowIndex);
            }

        }

        private void pdfDocumentView_PDFV_ZoomChanged(object sender, int zoomFactor)
        {
            _zoom = zoomFactor;
        }

        private void Cancel_BTN_Click(object sender, EventArgs e)
        {          
            this.Close();
        }

        private void Ok_BTN_Click(object sender, EventArgs e)
        {
            this.SaveCustomInfo(String.Empty);
            this.UnLoadPdfFile();
            this.Close();
        }

        private void transferSM2_BTN_Click(object sender, EventArgs e)
        {

            foreach (ListViewItem lvi in myListView_MLV.Items)
            {
                if (lvi.Checked)
                {
                    this.Build(false, lvi.Index);
                }                
            }
        }

       
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
        public enum ScaleFactorEnum
        {
            UnKnown = -1,
            ScaleAuto = 0,
            Scale120 = 20,
            Scale150 = 50
        }

        public static Dictionary<int, string> Dico = new Dictionary<int, string>();

        private ScaleFactorEnum _scaleFactor;
        public ScaleFactorEnum ScaleFactor
        {
            get { return _scaleFactor; }
            set { _scaleFactor = value; }
        }

        public ScaleFactorSubItem(ScaleFactorEnum scaleFactor)
        {
            this._scaleFactor = scaleFactor;
        }

        public override string ToString()
        {
            return ScaleFactorSubItem.Dico[(int)this._scaleFactor];
        }
        public double ToDouble(ScaleFactorEnum scaleFactor)
        {
            double result = (double)ScaleFactorEnum.ScaleAuto;

            try
            {
                double factor = (double)scaleFactor;
                result = (1.0 / factor);
            }
            catch (Exception)
            {
                result = (double)ScaleFactorEnum.ScaleAuto;
            }
            return result;
        }
    }

    public class PageMediaSizeNameSubItem : ListViewItem.ListViewSubItem
    {
        public static Dictionary<int, string> Dico = new Dictionary<int, string>();

        private System.Printing.PageMediaSizeName _pageMediaSizeName;
        public System.Printing.PageMediaSizeName PageMediaSizeName
        {
            get { return _pageMediaSizeName; }
            set { _pageMediaSizeName = value; }
        }

        public PageMediaSizeNameSubItem(System.Printing.PageMediaSizeName pageMediaSizeName)
        {
            this._pageMediaSizeName = pageMediaSizeName;
        }

        public override string ToString()
        {
            return PageMediaSizeNameSubItem.Dico[(int)this._pageMediaSizeName];
        }
    }

    public class PageOrientationSubItem :  ListViewItem.ListViewSubItem
    {
        public static Dictionary<int, string> Dico = new Dictionary<int, string>();

        private System.Printing.PageOrientation _pageOrientation;
        public System.Printing.PageOrientation PageOrientation
        {
            get { return _pageOrientation; }
            set { _pageOrientation = value; }
        }

        public PageOrientationSubItem(System.Printing.PageOrientation pageOrientation)
        {
            this._pageOrientation = pageOrientation;
        }

        public override string ToString()
        {
            return PageOrientationSubItem.Dico[(int)this._pageOrientation];
        }

    }

}
