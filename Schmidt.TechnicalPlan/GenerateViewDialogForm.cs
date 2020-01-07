using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KD.Plugin.Word;
using Syncfusion.DocIO.DLS;

namespace Schmidt.TechnicalPlan
{

    public partial class GenerateViewDialogForm : Form
    {
        public KD.Plugin.Word.Plugin _pluginWord = null;        
        private Dico _dico = null;
        private MyImageButton myOverViewButton = null;

        public int _callParamsBlock;
        private List<ListViewItem.ListViewSubItem> subDocList = new List<ListViewItem.ListViewSubItem>();

        const int InitialZoomValue = 99;
        private double _zoom;

        private ListViewItem lvItem = null;

        private int lviSelectedRowIndex = -1;
        private int lviSelectedColumnIndex = (int)MyListView.Enum.ListViewColumnIndex.UnKnown;

        //int _docIndex2Use = -1;
        //public int DocIndex2Use
        //{
        //    get
        //    {
        //        return _docIndex2Use;
        //    }
        //    set
        //    {
        //        _docIndex2Use = value;
        //    }
        //}

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

        //WordDocument _mergedWordDocument = null;
        //public WordDocument MergedWordDocument
        //{
        //    get
        //    {
        //        return _mergedWordDocument;
        //    }
        //    set
        //    {
        //        _mergedWordDocument = value;
        //    }
        //}

        public GenerateViewDialogForm(KD.Plugin.Word.Plugin pluginWord,  Dico dico)//, KD.Plugin.Word.GenerateDocumentDialog generateDocumentDialog,
        {
            InitializeComponent();

            //_callParamsBlock = callParamsBlock;
            _pluginWord = pluginWord;
            // _generateDocumentDialog = generateDocumentDialog;
            _dico = dico;
            this._pluginWord.MergedWordDocument = new WordDocument(/*firstWordTemplateFilePath*/);

            //_pluginWord.InitializeAll(_callParamsBlock);

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
        public void BuildDocument(int nDoc) //plugin Word
        {
            this._pluginWord.DocIndex2Use = FindDocNameFromChoice(nDoc); //nDoc; // 
            this._pluginWord.BuildDocument(this._pluginWord.MergedWordDocument, this._pluginWord.CurrentAppli.DocDir, this._pluginWord.DocIndex2Use/* nDoc*/, -1 /* nHeading */, false /*true*/, null);
        }
        public void EndWork()//plugin Word
        {
            this._pluginWord.EndWork();
            this.EndMerge(this.GetLocalizedDocName(this._pluginWord.DocIndex2Use));

        }
        public void EndMerge(string docname)//plugin Word
        {
            this.ActivatePdfViewer();
            //this._generateDocumentDialog.UpdateListViewItemImageIndex(docname);
            string pdfFilePath = this._pluginWord.GetArchiveFilePath(docname, KD.IO.File.Extension.Pdf);
            this.LoadPdfFile(pdfFilePath);

            //this._generateDocumentDialog.UpdateListViewItemImageIndex(docname);
            //this._generateDocumentDialog.SelectListViewItem(MyListView_MLV, lvItem, true);

        }
        private void ActivatePdfViewer() //plugin Word
        {
            //this.ViewContainer_SPC.Panel2.Show();
            this.pdfDocumentView_SyncPDFV.Enabled = true;
            this.pdfDocumentView_SyncPDFV.Visible = true;
            return;
        }

        private ListViewItem FindListViewItemFromDocName(string docName) //plugin Word
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
        private string GetDocNameFromListViewItem(ListViewItem lvi) //plugin Word
        {
            if (lvi == null)
            {
                return String.Empty;
            }
            return lvi.Tag.ToString();
        }
        public void LoadPdfFile(string pdfFilePath)//BR\\ add the last parameter //plugin Word
        {
            #region plugin Word
            //if (!this.Plugin.IsSaveAsPdfActive())
            //{
            //    return;
            //}

            //if (this.statusStrip1 != null) //B__R\\ not on my form 'statusStrip'
            //{
            //    this.UpdateStatusBar(pdfFilePath);
            //}

            //// unload current doc if needed 
            //if (String.IsNullOrEmpty(pdfFilePath) || !System.IO.File.Exists(pdfFilePath))
            //{
            //    this.DesactivatePdfViewer();
            //}
            //else
            //{
            #endregion

            this.ActivatePdfViewer();
            if (pdfFilePath != this.CurrentPdfFilePath) // optimization (not loading same doc twice)
            {
                // Set cursor as hourglass
                Cursor.Current = Cursors.WaitCursor;
                pdfDocumentView_SyncPDFV.Load(pdfFilePath);  //BR\\ pdfDocumentView replace this.pdfDocumentView1
                                                             // Set cursor as default arrow
                Cursor.Current = Cursors.Default;

                //// first load fit page
                //_zoom = 20;
                //if (_zoom == InitialZoomValue)
                //    PdfDocumentView_SyncPDFV.Invoke(new MethodInvoker(delegate
                //    {
                //        PdfDocumentView_SyncPDFV.ZoomMode = Syncfusion.Windows.Forms.PdfViewer.ZoomMode.FitPage;
                //    }));

                //else // take last zoom value
                //    PdfDocumentView_SyncPDFV.Invoke(new MethodInvoker(delegate
                //    {
                //        PdfDocumentView_SyncPDFV.ZoomTo(Convert.ToInt32(_zoom));
                //    }));

            }
            //}
            // can Update member only after load
            _currentPdfFilePath = pdfFilePath;

            #region plugin Word
            ////this.pdfDocumentView1.Refresh();

            //if (this.toolStripProgressBar1 != null) //B__R\\ not on my form 'statusStrip'
            //{
            //    this.InitProgressBar(0);
            //    this.UpdateToolbar();
            //}
            #endregion

            this.Visible = true;
            this.BringToFront();

        }
        //private enum ImageListIconsIndex
        //{
        //    None = -1,
        //    SynchronizedFalse = 0,
        //    SynchronizedTrue = 1,
        //    SubDir = 2
        //};
        private string GetLocalizedDocName(int iDoc) //plugin Word
        {
            // document localized final name : "DEVIS TTC"
            string docName = _pluginWord.CurrentAppli.DocGetInfo(iDoc, KD.SDK.AppliEnum.DocInfo.NAME);

            if (String.IsNullOrEmpty(docName))
            {
                return String.Empty;
            }

            //string wordDicoPath = this._pluginWord.Config.WordDicoPath();
            //// if word dico exist
            //if (System.IO.File.Exists(wordDicoPath))
            //{
            //    // Only if user is using different languages
            //    //if (this.CurrentAppli.SceneComponent.LanguageCode != this.CurrentAppli.Language)
            //    {
            //        docName = TranslateFromWordDico(docName);
            //    }
            //}
            // Special case. For supplier order do not take docname "Commande Fournisseur"
            // must take supplier id to differentiate them and have different generated documents 
            //if (this.CallParamData.IsSupplierOrder())
            //{
            //    docName = this.CallParamData.SupplierId;
            //}

            return docName;
        }
        //

        private List<ListViewItem.ListViewSubItem> GetSubDocNamesFromListViewItem(ListViewItem lvi)
        {
            subDocList.Clear();
            if (lvi != null)
            {
                foreach (ListViewItem.ListViewSubItem lvsi in lvi.SubItems)
                {
                    subDocList.Add(lvsi);
                }
            }
            return subDocList;
        }
        private int FindDocNameFromChoice(int iDoc)
        {
            int rank = 0;

            if (lvItem != null && lvItem.Index == iDoc)
            {
                GetSubDocNamesFromListViewItem(lvItem);
                string subDocName = lvItem.SubItems[2].Text + KD.StringTools.Const.MinusSign + lvItem.SubItems[3].Text + KD.StringTools.Const.MinusSign + lvItem.SubItems[4].Text;      //subDocList[2].Tag.ToString();

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
        private int ComboIndex(string customId)
        {
            int selectedIndex = 0;
            if (!String.IsNullOrEmpty(customId))
            {
                string[] customIndex = customId.Split(KD.CharTools.Const.SemiColon);
                if (customIndex.Length == 3)
                {
                    if (customIndex[0] == lvItem.Tag.ToString())
                    {
                        int.TryParse(customIndex[2], out selectedIndex);
                    }

                }
            }
            return selectedIndex;
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
                    if (column.Index != (int)MyListView.Enum.ListViewColumnIndex.Select && column.Index != (int)MyListView.Enum.ListViewColumnIndex.Overview)
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
            return (int)MyListView.Enum.ListViewColumnIndex.UnKnown;
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
            string saveString = lvItem.Tag.ToString() + KD.StringTools.Const.SemiColon + lviSelectedColumnIndex + KD.StringTools.Const.SemiColon + comboBox.SelectedIndex.ToString();
            string InfoKey = lvItem.Tag.ToString() + customID;
            this._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(saveString, InfoKey);
        }

        private void ClearComboBoxListView()
        {
            foreach (Control control in this.Controls) //ViewContainer_SPC.Panel1.Controls)
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
            foreach (Control control in this.Controls) //ViewContainer_SPC.Panel1.Controls)
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
            comboBox.Items.Add(this._dico.GetTranslate(IdentifyConstanteId.Scale120ID));
            comboBox.Items.Add(this._dico.GetTranslate(IdentifyConstanteId.Scale150ID));
            comboBox.Items.Add(this._dico.GetTranslate(IdentifyConstanteId.ScaleAjustedID));
        }
        private void PaperItems(ComboBox comboBox)
        {
            comboBox.Items.Add(this._dico.GetTranslate(IdentifyConstanteId.FormatA4ID));
            comboBox.Items.Add(this._dico.GetTranslate(IdentifyConstanteId.FormatA3ID));
        }
        private void OrientationItems(ComboBox comboBox)
        {
            comboBox.Items.Add(this._dico.GetTranslate(IdentifyConstanteId.PortraitID));
            comboBox.Items.Add(this._dico.GetTranslate(IdentifyConstanteId.LandscapeID));
        }

        public void UpdateListViewStandardInfo()
        {
            //this.DisableListViewEvent();
            int index = 0;

            int nbDoc = _pluginWord.CurrentAppli.GetDocsNb();
            for (int iDoc = 0; iDoc < nbDoc; iDoc++)
            {
                KD.SDK.AppliEnum.DocType doctype = _pluginWord.CurrentAppli.DocGetType(iDoc);

                #region// Supplier Order
                //if (this.SupplierIds.Count > 0)
                //{
                //if (doctype != KD.SDK.AppliEnum.DocType.SupplierOrder)
                //{
                //    continue;
                //}
                //if (this.Plugin.DocIndex2Use != iDoc)
                //{
                //    continue;
                //}
                //foreach (string supplierId in this.SupplierIds)
                //{
                //    string docName = supplierId;
                //    if (FindListViewItemFromDocName(docName) == null)
                //    {
                //        this.BindDocName2DocIndex(docName, iDoc);
                //        this.AddListViewItem(docName, iDoc);
                //    }
                //    UpdateListViewItemImageIndex(docName);
                //}
                //// the loop is different for orders, not on nbDoc, it's based on SupplierIds
                //break;
                //}
                //else
                //{
                #endregion
                if (doctype != KD.SDK.AppliEnum.DocType.Commercial && doctype != KD.SDK.AppliEnum.DocType.Management)
                {
                    continue;
                }
                string docName = this.GetLocalizedDocName(iDoc);
                // if item does not exists, create a new one 
                if (this.FindListViewItemFromDocName(docName) == null)
                {
                    AddListViewItem(docName, iDoc, index++);
                }
            }

            this.myListView_MLV.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            this.myListView_MLV.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.myListView_MLV.Refresh();
            //this.UpdateStatusBar(String.Empty);

            //this.EnableListViewEvent();
        }
        public void UpdateListViewCustomInfo()
        {
            int nbDoc = _pluginWord.CurrentAppli.GetDocsNb();
            for (int iDoc = 0; iDoc < nbDoc; iDoc++)
            {
                string docName = this.GetLocalizedDocName(iDoc);
                lvItem = this.FindListViewItemFromDocName(docName);

                if (lvItem != null)
                {
                    foreach (Control control in this.Controls) //ViewContainer_SPC.Panel1.Controls)
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

        private void AddListViewItem(string docName, int iDoc, int index)
        {
            lvItem = new ListViewItem(docName, iDoc);

            lvItem.SubItems.Add(docName);
            int nbSubDoc = _pluginWord.CurrentAppli.GetDocItemsNb(iDoc);
            if (nbSubDoc > 0)
            {
                // default image
                //lvItem.ImageIndex = Convert.ToInt32(ImageListIconsIndex.SynchronizedFalse);
                lvItem.Tag = docName;// index
                                     //for (int iSubDoc = 0; iSubDoc < nbSubDoc; iSubDoc++)
                                     //{
                                     //    string subDocName = this.GetLocalizedDocName(iDoc + iSubDoc + 1);
                                     //    lvItem.SubItems.Add(subDocName);
                                     //}
                lvItem.SubItems.Add(this.scaleList_CBX.Items[(int)MyListView.Enum.ScaleFactor.Scale120].ToString());
                lvItem.SubItems.Add(this.paperList_CBX.Items[(int)MyListView.Enum.PaperFormat.A4].ToString());
                lvItem.SubItems.Add(this.orientationList_CBX.Items[(int)MyListView.Enum.PageOrientation.Portrait].ToString());

                lvItem.SubItems.Add("");
                // Create two ImageList objects.
                //ImageList imageListSmall = new ImageList();
                //ImageList imageListLarge = new ImageList();

                // Initialize the ImageList objects with bitmaps.
                //imageListSmall.Images.Add(Bitmap.FromFile("D:\\_Travail\\Documents Fabricants\\SCHMIDT GROUPE\\PLAN TECHNIQUE\\twin.png"));
                //imageListSmall.Images.Add(Bitmap.FromFile("C:\\MySmallImage2.bmp"));
                //imageListLarge.Images.Add(Bitmap.FromFile("C:\\MyLargeImage1.bmp"));
                //imageListLarge.Images.Add(Bitmap.FromFile("C:\\MyLargeImage2.bmp"));

                //Assign the ImageList objects to the ListView.
                //this.MyListView.LargeImageList = imageListLarge;
                //this.MyListView.SmallImageList = imageListSmall;

            }
            else
            {
                ;
                lvItem.Tag = docName;// ConstListView.UnknownTag;// index
                lvItem.ImageIndex = -1; // Convert.ToInt32(ImageListIconsIndex.SubDir);
                lvItem.SubItems[KD.Plugin.Word.ConstListView.ColumnIndexDate].Text = String.Empty;
            }
            this.myListView_MLV.Items.Add(lvItem);
            lvItem.Text = String.Empty;
        }
        private void AddTwinButton()
        {
            for (int lineIndex = 0; lineIndex < this.myListView_MLV.Items.Count; lineIndex++)
            {
                Rectangle subItemImageButton = this.myListView_MLV.Items[lineIndex].SubItems[(int)MyListView.Enum.ListViewColumnIndex.Overview].Bounds;
                subItemImageButton.Y = this.myListView_MLV.Items[lineIndex].SubItems[(int)MyListView.Enum.ListViewColumnIndex.Overview].Bounds.Y;// + this.myListView.Top;
                subItemImageButton.X = this.myListView_MLV.Items[lineIndex].SubItems[(int)MyListView.Enum.ListViewColumnIndex.Overview].Bounds.X + this.myListView_MLV.Left;
                subItemImageButton.Height = this.myListView_MLV.Items[lineIndex].SubItems[(int)MyListView.Enum.ListViewColumnIndex.Overview].Bounds.Height + 4;
                subItemImageButton.Width = this.myListView_MLV.Items[lineIndex].SubItems[(int)MyListView.Enum.ListViewColumnIndex.Overview].Bounds.Height + 4;
                // Assign calculated bounds to the ComboBox.
                myOverViewButton = new MyImageButton();
                myListView_MLV.Controls.Add(myOverViewButton);
                myOverViewButton.Bounds = subItemImageButton;
                myOverViewButton.Location = new Point(subItemImageButton.X + 10, subItemImageButton.Y - 2);

                // Set default text for ComboBox to match the item that is clicked.                 
                myOverViewButton.Visible = true;
                myOverViewButton.BringToFront();
                myOverViewButton.Tag = lineIndex;//.ToString();
                myOverViewButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.myOverViewButton_MouseDown);
            }
           
        }

        private void GenerateViewDialogForm_Load(object sender, EventArgs e)
        {
            this.InitializeListView();
            this.ClearComboBoxListView();
            this.AssignItemsInComboBox();
            this.UpdateListViewStandardInfo();
            this.UpdateListViewCustomInfo();
        }
        private void GenerateViewDialogForm_Shown(object sender, EventArgs e)
        {
            this.AddTwinButton();
        }
        private void GenerateViewDialogForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            pdfDocumentView_SyncPDFV.Unload();
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
                case (int)MyListView.Enum.ListViewColumnIndex.Scale:
                    comboBox = this.scaleList_CBX;
                    break;
                case (int)MyListView.Enum.ListViewColumnIndex.Paper:
                    comboBox = this.paperList_CBX;
                    break;
                case (int)MyListView.Enum.ListViewColumnIndex.Orientation:
                    comboBox = this.orientationList_CBX;
                    break;
                case (int)MyListView.Enum.ListViewColumnIndex.Overview:
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
            if (e.Item.Index != -1 && lvItem.Index != -1)
            {
                if (lvItem.Checked)
                {
                    //here make method to delete, send SM2, Print ect...               
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
            this.SaveCustomInfo((ComboBox)sender, ConstCustomName.ScaleID);
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
            this.SaveCustomInfo((ComboBox)sender, ConstCustomName.PaperID);
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
            this.SaveCustomInfo((ComboBox)sender, ConstCustomName.OrientationID);
            this.HideComboBox(this.orientationList_CBX);
        }
        private void orientationList_CBX_Enter(object sender, EventArgs e)
        {
            this.SetTagAndTextInComboBox(this.orientationList_CBX);
        }

        private void myOverViewButton_MouseDown(object sender, MouseEventArgs e)
        {            
            Button overViewButton = (Button)sender;
            //int index = Convert.ToInt32(overViewButton.Tag.ToString());
            lvItem = this.myListView_MLV.Items[(int)overViewButton.Tag];

            if (lvItem != null)
            {
                lviSelectedRowIndex = lvItem.Index;
                MessageBox.Show(lvItem.Tag.ToString());
                this.BuildDocument(lvItem.Index);
                this.EndWork();
            }

        }
        //private void myOverViewButton_Click(object sender, EventArgs e)
        //{
        //    //Button overViewButton = (Button)sender;            
        //    //lvItem = this.myListView_MLV.GetItemAt(overViewButton.Bounds.X, overViewButton.Bounds.Y);
        //    lvItem = this.myListView_MLV.Items[lviSelectedRowIndex];
            
        //    if (lvItem != null && lvItem.Index != -1)
        //    {
        //        MessageBox.Show(lvItem.Tag.ToString());
        //        //this.BuildDocument(lvItem.Index);
        //        //this.EndWork();
        //    }            
        //}      
    }

    //public class ConstListView
    //{
    //    //static public int UnknownTag = -1;
    //    //static public int ColumnIndexName = 0;
    //    //static public int ColumnIndexDate = 1;
    //    //static public int ColumnIndexStatus = 2;
    //    static public string ColumnHeaderSelectID = "Sél.";
    //    static public string ColumnHeaderViewID = "Vue";
    //    static public string ColumnHeaderScaleID = "Echelle";
    //    static public string ColumnHeaderPaperID = "Papier";
    //    static public string ColumnHeaderOrientationID = "Orientation";
    //    static public string ColumnHeaderOverviewID = "Aperçu";
    //    //// { "Nom", "Archivé", "Date"}
    //    //static public string ButtonTextPrintSelection = "&Imprimer";
    //    //static public string ButtonStopThread = "&Arrêter\tEchap";
    //    //static public string ButtonOpenWord = "Modifier";
    //    //static public string ButtonTextOK = "Fermer";
    //    //static public string ButtonTextOpenWorkingDir = "Documents disponibles";
    //    //static public string ButtonTextDeleteFile = "&Supprimer";


    //}

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
            public enum ListViewColumnIndex
            {
                UnKnown = -1,
                Select = 0,
                View = 1,
                Scale = 2,
                Paper = 3,
                Orientation = 4,
                Overview = 5
            }
            public enum ScaleFactor
            {
                UnKnown = -1,
                Scale120 = 0,       //20
                Scale150 = 1,   //50
                ScaleAuto = 2      //0          
            }
            public enum PaperFormat //remplace PageMediaSizeName.enum
            {
                UnKnown = -1,
                A4 = 0,
                A3 = 1               
            }
            public enum PageOrientation //remplacer par system.printing.pageorientation
            {
                UnKnown = -1,
                Portrait = 0,
                Landscape = 1
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
}
