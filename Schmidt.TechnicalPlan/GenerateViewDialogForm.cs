using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;


namespace Schmidt.TechnicalPlan
{

    public partial class GenerateViewDialogForm : Form
    {
        List<TechnicalDocument> _documentList = new List<TechnicalDocument>();

        PageOrientationUI portrait = new PageOrientationUI(System.Printing.PageOrientation.Portrait);
        PageOrientationUI paysage = new PageOrientationUI(System.Printing.PageOrientation.Landscape);
        PageOrientationUI pageOrientationChoice = new PageOrientationUI(System.Printing.PageOrientation.Portrait);

        PageMediaSizeNameUI A4 = new PageMediaSizeNameUI(System.Printing.PageMediaSizeName.ISOA4);
        PageMediaSizeNameUI A3 = new PageMediaSizeNameUI(System.Printing.PageMediaSizeName.ISOA3);
        PageMediaSizeNameUI pageMediaSizeNameChoice = new PageMediaSizeNameUI(System.Printing.PageMediaSizeName.ISOA4);

        ScaleFactorUI scale120 = new ScaleFactorUI(ScaleFactorUI.ScaleFactorEnum.Scale120);
        ScaleFactorUI scale150 = new ScaleFactorUI(ScaleFactorUI.ScaleFactorEnum.Scale150);
        ScaleFactorUI scaleAuto = new ScaleFactorUI(ScaleFactorUI.ScaleFactorEnum.ScaleAuto);
        ScaleFactorUI scaleFactorChoice = new ScaleFactorUI(ScaleFactorUI.ScaleFactorEnum.Scale120);
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

            //this.InitializeTechnicalDocument();
           

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

            header.Add(this._dico.GetTranslate(IdentifyConstanteId.ColumnHeaderSelectID));
            header.Add(this._dico.GetTranslate(IdentifyConstanteId.ColumnHeaderViewID));
            header.Add(this._dico.GetTranslate(IdentifyConstanteId.ColumnHeaderScaleID));
            header.Add(this._dico.GetTranslate(IdentifyConstanteId.ColumnHeaderPaperID));
            header.Add(this._dico.GetTranslate(IdentifyConstanteId.ColumnHeaderOrientationID));
            header.Add(this._dico.GetTranslate(IdentifyConstanteId.ColumnHeaderOverviewID));//Plugin.pluginWord.CurrentAppli.GetTranslatedText
            header.ForEach(name => this.myListView_MLV.Columns.Add(name));

            // Loop through and size each column header to fit the column header text.
            foreach (ColumnHeader ch in this.myListView_MLV.Columns)
            {
                ch.Width = -2;
            }
        }
     
        public void Build(bool preview)
        {
            this._pluginWord.DocIndex2Use = FindDocNameFromChoice(lvItem.Index);
            this._pluginWord.GenerateDocument(false);

            string docname = this._pluginWord.GetLocalizedDocName(this._pluginWord.DocIndex2Use);
            string pdfFilePath = this._pluginWord.GetArchiveFilePath(docname, KD.IO.File.Extension.Pdf);
            string dotFilePath = this._pluginWord.GetArchiveFilePath(docname, KD.IO.File.Extension.Dot);

            if (preview)
            {
                this.LoadPdfFile(this.MoveFile(pdfFilePath, dotFilePath, 0));
            }
            else
            {
                this.SaveViewCustomInfo(pdfFilePath);
            }
        }
        private string MoveFile(string pdfFilePath, string dotFilePath, int time)
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
                        MoveFile(pdfFilePath, dotFilePath, time++);
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

                int nbSubDoc = this._pluginWord.CurrentAppli.GetDocItemsNb((iDoc * 12) + iDoc);

                for (int iSubDoc = 1; iSubDoc <= nbSubDoc; iSubDoc++)
                {
                    rank = (iSubDoc + (iDoc * 12) + iDoc);
                    string name = this._pluginWord.CurrentAppli.DocGetInfo(rank, KD.SDK.AppliEnum.DocInfo.NAME);

                    if (name.Contains(subDocName))
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
        private void SetTextInItem(ComboBox comboBox)
        {
            this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[lviSelectedColumnIndex].Tag = comboBox.Text;
            this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[lviSelectedColumnIndex].Text = comboBox.Text;
        }
        private void SetTagAndTextInComboBox(ComboBox comboBox)
        {
            comboBox.Tag = this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[lviSelectedColumnIndex].Text;
            comboBox.Text = this.myListView_MLV.Items[lviSelectedRowIndex].SubItems[lviSelectedColumnIndex].Text;
        }

        private void LoadCustomInfo(ComboBox comboBox, string customID)
        {
            string customId = this._pluginWord.CurrentAppli.Scene.SceneGetCustomInfo(lvItem.Tag.ToString() + customID);
            if (!String.IsNullOrEmpty(customId))
            {
                string[] custom = customId.Split(KD.CharTools.Const.SemiColon);
                if (custom.Length == 3)
                {
                    if (custom[0] != lvItem.Tag.ToString())
                    {
                        return;
                    }

                    lviSelectedRowIndex = lvItem.Index;

                    int.TryParse(custom[1], out int columnIndex);
                    lviSelectedColumnIndex = columnIndex;

                    int.TryParse(custom[2], out int selectedIndex);
                    comboBox.SelectedIndex = selectedIndex;

                    comboBox.Text = comboBox.SelectedItem.ToString();
                    this.SetTextInItem(comboBox);
                }
            }
        }
        private void SaveCustomInfo(ComboBox comboBox, string customID)
        {
            //vue de dessus ; 2 ; 1
            //vue de dessus_SCALEID
            string saveString = String.Empty; // lvItem.Tag.ToString() + KD.StringTools.Const.SemiColon + lviSelectedColumnIndex + KD.StringTools.Const.SemiColon + comboBox.SelectedIndex.ToString();
            string InfoKey = lvItem.Tag.ToString() + customID;
            this._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(saveString, InfoKey);
        }
        private void SaveViewCustomInfo(string pdfFilePath)
        {          
            string xmlCustomInfo = String.Empty;
            string xmlInfoKey = "TechnicalDocument";

            _technicalDocument = new TechnicalDocument(pdfFilePath);
            _technicalDocument.WriteToXml(this._documentList, out XmlNode xmlNode); 
            xmlCustomInfo = xmlNode.OuterXml;           
            //this._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(xmlCustomInfo, xmlInfoKey);

            ;

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
            comboBox.Items.Add(scale120);
            comboBox.Items.Add(scale150);
            comboBox.Items.Add(scaleAuto);
        }
        private void PaperItems(ComboBox comboBox)
        {
            comboBox.Items.Add(A4);
            comboBox.Items.Add(A3);
        }
        private void OrientationItems(ComboBox comboBox)
        {
            comboBox.Items.Add(portrait);
            comboBox.Items.Add(paysage);                                  
           
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
        public void UpdateListViewStandardInfo()
        {
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
            this.CreateDocumentList();
            this.UpdateGUI();            

        }
        public void UpdateListViewCustomInfo()
        {
            int nbDoc = _pluginWord.CurrentAppli.GetDocsNb();
            for (int iDoc = 0; iDoc < nbDoc; iDoc++)
            {
                string docName = this._pluginWord.GetLocalizedDocName(iDoc);
                lvItem = this.FindListViewItemFromDocName(docName);

                if (lvItem != null)
                {
                    foreach (Control control in splitContainer_SPC.Panel1.Controls)
                    {
                        if (control.GetType().Equals(typeof(ComboBox)))
                        {
                            ComboBox comboBox = (ComboBox)control;
                            if (comboBox.Name.ToUpper().Contains(ConstComboBox.PartialScaleName))
                            {
                                this.LoadCustomInfo(comboBox, ConstCustomName.ScaleID);
                            }
                            if (comboBox.Name.ToUpper().Contains(ConstComboBox.PartialPaperName))
                            {
                                this.LoadCustomInfo(comboBox, ConstCustomName.PaperID);
                            }
                            if (comboBox.Name.ToUpper().Contains(ConstComboBox.PartialOrientationName))
                            {
                                this.LoadCustomInfo(comboBox, ConstCustomName.OrientationID);
                            }
                        }
                    }
                }
            }
        }

        //
        private void CreateDocumentList()
        {           
            int nbObject = _plugin.CurrentAppli.Scene.SceneGetObjectsNb();
            int nbWall = 0;
            List<int> wallIds = new List<int>();
            for (int iobj = 0; iobj < nbObject; iobj++)
            {
                int objectId = _plugin.CurrentAppli.Scene.SceneGetObjectId(iobj);
                KD.Model.Article article = new KD.Model.Article(_plugin.CurrentAppli, objectId);
                if (article.Type == (int)KD.SDK.SceneEnum.ObjectType.WALL)
                {
                    nbWall += 1;
                    wallIds.Add(article.ObjectId);
                }
            }

            this._documentList.Clear();
            //TOPVIEW
            TechnicalDocument docTopView = new TechnicalDocument();
            docTopView.Type = TechnicalDocument.Dico[(int)KD.SDK.SceneEnum.ViewMode.TOP];
            docTopView.ScaleFactor = 1.0 / 20.0;
            docTopView.PageOrientation = System.Printing.PageOrientation.Portrait;
            docTopView.PageMediaSizeName = System.Printing.PageMediaSizeName.ISOA4;
            docTopView.ViewMode = KD.SDK.SceneEnum.ViewMode.TOP;
            docTopView.ObjectID = KD.Const.UnknownId;
            docTopView.Marq = _plugin.CurrentAppli.Scene.ObjectGetInfo(docTopView.ObjectID, KD.SDK.SceneEnum.ObjectInfo.NUMBER);
            _documentList.Add(docTopView);

            //Wall
            for (int iWall = 0; iWall < nbWall; iWall++)
            {
                TechnicalDocument docWallView = new TechnicalDocument();
                docWallView.Type = TechnicalDocument.Dico[(int)KD.SDK.SceneEnum.ViewMode.VECTELEVATION];
                docWallView.ScaleFactor = 1.0 / 20.0;
                docWallView.PageOrientation = System.Printing.PageOrientation.Portrait;
                docWallView.PageMediaSizeName = System.Printing.PageMediaSizeName.ISOA4;
                docWallView.ViewMode = KD.SDK.SceneEnum.ViewMode.VECTELEVATION;
                docWallView.ObjectID = wallIds[iWall];
                docWallView.Marq = _plugin.CurrentAppli.Scene.ObjectGetInfo(docWallView.ObjectID, KD.SDK.SceneEnum.ObjectInfo.NUMBER);
                _documentList.Add(docWallView);
            }
        }      
        private void UpdateGUI()
        {
            int imageIndex = 0;
            List<TechnicalDocument> listView = new List<TechnicalDocument>();
            foreach (TechnicalDocument ithDoc in this._documentList)
            {
                lvItem = new ListViewItem(ithDoc.ToString(), imageIndex++);
                listView.Add(ithDoc);                
                lvItem.SubItems.Add(ithDoc.ToString());
                lvItem.Tag = ithDoc.ToString();

                lvItem.SubItems.Add(scaleFactorChoice.ToString());
                lvItem.SubItems.Add(pageMediaSizeNameChoice.ToString());
                lvItem.SubItems.Add(pageOrientationChoice.ToString());
                lvItem.SubItems.Add(String.Empty);

                lvItem.Text = String.Empty;
                this.myListView_MLV.Items.Add(lvItem);
            }
            this.myListView_MLV.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.myListView_MLV.Refresh();
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
                myOverViewButton.Tag = lineIndex;//.ToString();
                myOverViewButton.Click += new System.EventHandler(this.myOverViewButton_Click);
            }

        }

      

        private void GenerateViewDialogForm_Load(object sender, EventArgs e)
        {
            this.InitializeListView();
            this.ClearComboBoxListView();
            this.AssignItemsInComboBox();
            this.UpdateListViewStandardInfo();
            //this.UpdateListViewCustomInfo();
        }
        private void GenerateViewDialogForm_Shown(object sender, EventArgs e)
        {
            this.AddTwinButton();
        }
        private void GenerateViewDialogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.pdfDocumentView_PDFV.Unload();
            this.Hide();
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
        private void MyListView_MLV_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (lvItem != null && e.Item.Index != -1 && lvItem.Index != -1)
            {
                if (lvItem.Checked)
                {
                    //here make method to delete, send SM2, Print ect...                   
                    lviSelectedRowIndex = lvItem.Index;                    
                }
            }
        }

        private void scaleList_CBX_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the user presses ESC.
            this.KeyPressEvent(e, this.scaleList_CBX);

        }
        private void scaleList_CBX_Leave(object sender, EventArgs e)
        {
            this.SetTextInItem(this.scaleList_CBX);
            this.HideComboBox(this.scaleList_CBX);
        }
        private void scaleList_CBX_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            scaleFactorChoice = (ScaleFactorUI)comboBox.SelectedItem;

            //_technicalDocument.ScaleFactor = (TechnicalDocument)scaleFactorChoice);

            this.SaveCustomInfo(comboBox, ConstCustomName.ScaleID);
            this.HideComboBox(this.scaleList_CBX);
        }
        private void scaleList_CBX_Enter(object sender, EventArgs e)
        {
            this.SetTagAndTextInComboBox(this.scaleList_CBX);
        }

        private void paperList_CBX_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.KeyPressEvent(e, this.paperList_CBX);
        }
        private void paperList_CBX_Leave(object sender, EventArgs e)
        {
            this.SetTextInItem(this.paperList_CBX);
            this.HideComboBox(this.paperList_CBX);
        }
        private void paperList_CBX_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            pageMediaSizeNameChoice = (PageMediaSizeNameUI)comboBox.SelectedItem;

            _technicalDocument.PageMediaSizeName = pageMediaSizeNameChoice.PageMediaSizeName;
            _documentList[comboBox.SelectedIndex].PageMediaSizeName = pageMediaSizeNameChoice.PageMediaSizeName;

            this.SaveCustomInfo(comboBox, ConstCustomName.PaperID);
            this.HideComboBox(this.paperList_CBX);
        }
        private void paperList_CBX_Enter(object sender, EventArgs e)
        {
            this.SetTagAndTextInComboBox(this.paperList_CBX);
        }

        private void orientationList_CBX_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.KeyPressEvent(e, this.orientationList_CBX);
        }
        private void orientationList_CBX_Leave(object sender, EventArgs e)
        {
            this.SetTextInItem(this.orientationList_CBX);
            this.HideComboBox(this.orientationList_CBX);
        }
        private void orientationList_CBX_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            pageOrientationChoice = (PageOrientationUI)comboBox.SelectedItem;           

            this.SaveCustomInfo(comboBox, ConstCustomName.OrientationID);
            this.HideComboBox(this.orientationList_CBX);
        }
        private void orientationList_CBX_Enter(object sender, EventArgs e)
        {
            this.SetTagAndTextInComboBox(this.orientationList_CBX);
        }

        private void myOverViewButton_Click(object sender, EventArgs e)
        {
            Button overViewButton = (Button)sender;
            lvItem = this.myListView_MLV.Items[(int)overViewButton.Tag];

            if (lvItem != null)
            {
                lviSelectedRowIndex = lvItem.Index;
                this.Build(true);
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
            this.Build(false);
            //this.Close();
        }
    }

    public class ConstFile
    {
        public const string TechnicalPlanPreviewDirName = "TECHNICAL_PLAN_PREVIEW";
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

   

    public class MyListView : System.Windows.Forms.ListView
    {

        public class Enum
        {
            public enum ColumnIndex
            {
                UnKnown = -1,
                Select = 0,
                View = 1,
                Scale = 2,
                Paper = 3,
                Orientation = 4,
                Overview = 5
            }

        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public MyListView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call  

        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion

        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;

        protected override void WndProc(ref Message msg)
        {
            // Look for the WM_VSCROLL or the WM_HSCROLL messages.
            if ((msg.Msg == WM_VSCROLL) || (msg.Msg == WM_HSCROLL))
            {
                // Move focus to the ListView to cause ComboBox to lose focus.
                this.Focus();
            }

            // Pass message to default handler.
            base.WndProc(ref msg);
        }
    }

    public class MyImageButton : System.Windows.Forms.Button
    {
        public MyImageButton()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call  
        }

        private void InitializeComponent()
        {
            this.BackgroundImage = Properties.Resources.twin;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Visible = false;
            this.Tag = String.Empty;
        }
    }

    //UI
    public class ScaleFactorUI
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

        public ScaleFactorUI(ScaleFactorEnum scaleFactor)
        {
            this._scaleFactor = scaleFactor;
        }

        public override string ToString()
        {
            return ScaleFactorUI.Dico[(int)this._scaleFactor];
        }
    }

    public class PageMediaSizeNameUI
    {
        public static Dictionary<int, string> Dico = new Dictionary<int, string>();

        private System.Printing.PageMediaSizeName _pageMediaSizeName;
        public System.Printing.PageMediaSizeName PageMediaSizeName
        {
            get { return _pageMediaSizeName; }
            set { _pageMediaSizeName = value; }
        }

        public PageMediaSizeNameUI(System.Printing.PageMediaSizeName pageMediaSizeName)
        {
            this._pageMediaSizeName = pageMediaSizeName;
        }

        public override string ToString()
        {
            return PageMediaSizeNameUI.Dico[(int)this._pageMediaSizeName];
        }
    }

    public class PageOrientationUI
    {
        public static Dictionary<int, string> Dico = new Dictionary<int, string>();

        private System.Printing.PageOrientation _pageOrientation;

         public PageOrientationUI(System.Printing.PageOrientation pageOrientation)
        {
            this._pageOrientation = pageOrientation;
        }

        public override string ToString()
        {
            return PageOrientationUI.Dico[(int)this._pageOrientation];
        }

    }

}
