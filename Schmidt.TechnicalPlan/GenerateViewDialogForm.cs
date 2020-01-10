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


namespace Schmidt.TechnicalPlan
{

    public partial class GenerateViewDialogForm : Form
    {
        private KD.Plugin.Word.Plugin _pluginWord = null;        
        private Dico _dico = null;
        private MyImageButton myOverViewButton = null;

        const int InitialZoomValue = 99;
        private double _zoom;

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

        ComboBoxScale120 comboBoxScale120 = null;
        ComboBoxScale150 comboBoxScale150 = null;
        ComboBoxScaleAjusted comboBoxScaleAjusted = null;

        ComboBoxPaperA4 ComboBoxPaperA4 = null;


        public GenerateViewDialogForm(Plugin plugin, KD.Plugin.Word.Plugin pluginWord, Dico dico)
        {
            InitializeComponent();

            _plugin = plugin;
            _pluginWord = pluginWord;          
            _dico = dico;

            comboBoxScale120 = new ComboBoxScale120(this._dico);
            comboBoxScale150 = new ComboBoxScale150(this._dico);
            comboBoxScaleAjusted = new ComboBoxScaleAjusted(this._dico);

            ComboBoxPaperA4 = new ComboBoxPaperA4(this._dico);
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

        //
        public void Build(bool preview)
        {
            this._pluginWord.DocIndex2Use = FindDocNameFromChoice(lvItem.Index); 
            this._pluginWord.GenerateDocument(false);

            string docname = this._pluginWord.GetLocalizedDocName(this._pluginWord.DocIndex2Use);
            string pdfFilePath = this._pluginWord.GetArchiveFilePath(docname, KD.IO.File.Extension.Pdf);
            string dotFilePath = this._pluginWord.GetArchiveFilePath(docname, KD.IO.File.Extension.Dot);

            if (preview)
            {
                this.LoadPdfFile(this.MoveFile(pdfFilePath, dotFilePath));
            }
            else
            {
               this.SaveViewCustomInfo(pdfFilePath);
            }
        }
        private string MoveFile(string pdfFilePath, string dotFilePath)
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
                System.IO.File.Move(pdfFilePath, previewPdfFilePath);
                System.IO.File.Move(dotFilePath, previewDotFilePath);

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
                System.Threading.Thread.Sleep(1000);
                pdfDocumentView_PDFV.Load(pdfFilePath); // @"D:\Ic90dev\Scenes\5DE66AB8_0226_01\DocTechnicalPlan\TECHNICAL_PLAN_PREVIEW\Elévation du mur.pdf"); 

                // Set cursor as default arrow
                Cursor.Current = Cursors.Default;

                // first load fit page
                _zoom = 20;
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
            string saveString = lvItem.Tag.ToString() + KD.StringTools.Const.SemiColon + lviSelectedColumnIndex + KD.StringTools.Const.SemiColon + comboBox.SelectedIndex.ToString();
            string InfoKey = lvItem.Tag.ToString() + customID;
            this._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(saveString, InfoKey);           
        }
        private void SaveViewCustomInfo(string pdfFilePath)
        {
            //
            string xmlCustomInfo = String.Empty;
            string xmlInfoKey = String.Empty;
            //< Vues >
            //< Vue >
            //< NomFichier > toto.pdf </ NomFichier > => {ViewMode}_{Number}_{ObjectId}.pdf
            xmlInfoKey = "Vues"; //|Vue|NomFichier
            xmlCustomInfo = System.IO.Path.GetFileName(pdfFilePath);
            this._pluginWord.CurrentAppli.Scene.SceneSetCustomInfo(xmlCustomInfo, xmlInfoKey);
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
            
            comboBox.Items.Add(comboBoxScale120.Name);            
            comboBox.Items.Add(comboBoxScale150.Name);
            comboBox.Items.Add(comboBoxScaleAjusted.Name);
        }
        private void PaperItems(ComboBox comboBox)
        {
            comboBox.Items.Add(ComboBoxPaperA4.Name);
            comboBox.Items.Add(this._dico.GetTranslate(IdentifyConstanteId.FormatA3ID));
        }
        private void OrientationItems(ComboBox comboBox)
        {
            comboBox.Items.Add(this._dico.GetTranslate(IdentifyConstanteId.PortraitID));
            comboBox.Items.Add(this._dico.GetTranslate(IdentifyConstanteId.LandscapeID));
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
                
                string docName = this._pluginWord.CurrentAppli.DocGetInfo(iDoc, KD.SDK.AppliEnum.DocInfo.NAME);                       
                string translateDocName = this._dico.GetTranslate(docName);
                AddListViewItem(translateDocName, iDoc, index++);               
            }
          
            this.myListView_MLV.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.myListView_MLV.Refresh();
            
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
                lvItem.SubItems.Add(this.scaleList_CBX.Items[(int)ComboBoxEnum.ScaleFactorIndex.Scale120].ToString());
                lvItem.SubItems.Add(this.paperList_CBX.Items[(int)ComboBoxEnum.PaperFormatIndex.A4].ToString());
                lvItem.SubItems.Add(this.orientationList_CBX.Items[(int)ComboBoxEnum.PageOrientationIndex.Portrait].ToString());

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

        public void toto()
        {          

            byte[] doc = this.GetDocument();
            Syncfusion.Pdf.Parsing.PdfLoadedDocument pdf = new Syncfusion.Pdf.Parsing.PdfLoadedDocument(doc);
           
            this.pdfDocumentView_PDFV.Load(pdf);
            //this.pdfViewerControl1.Load(@"D:\A.pdf");

            this.pdfDocumentView_PDFV.ZoomTo(100);
            this.pdfDocumentView_PDFV.Update();

            this.ShowDialog();//this.Plugin.CurrentAppli.GetNativeIWin32Window()
        }
        public byte[] GetDocument()
        {
            var docBytes = System.IO.File.ReadAllBytes(@"D:\A.pdf");
           // string docBase64 =  Convert.ToBase64String(docBytes); //"data:application/pdf;base64," +
            return docBytes;
        }

        private void GenerateViewDialogForm_Load(object sender, EventArgs e)
        {
            //this.pdfDocumentView_PDFV.Load(@"D:\Ic90dev\Scenes\5DE66AB8_0226_01\DocTechnicalPlan\TECHNICAL_PLAN_PREVIEW\Elévation du mur.pdf");
            //this.pdfDocumentView_PDFV.Load(@"D:\A.pdf");
            //this.pdfDocumentView_PDFV.ZoomTo(100);
            //this.pdfDocumentView_PDFV.Update();

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
                    this.Build(false);                    
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

    public class ComboBoxEnum
    {
        public enum ScaleFactorIndex
        {
            UnKnown = -1,
            ScaleAuto = 0,
            Scale120 = 1,
            Scale150 = 2
        }
        public enum PaperFormatIndex // system.printing.PageMediaSizeName.enum
        {
            UnKnown = -1,
            A4 = 0,
            A3 = 1
        }
        public enum PageOrientationIndex// system.printing.pageorientation
        {
            UnKnown = -1,
            Portrait = 0,
            Landscape = 1
        }
    }
    public class ComboBoxScale120
    {
        private Dico _dico = null;

        public ComboBoxScale120(Dico dico)
        {
            _dico = dico;
        }

        public string Name
        {
            get
            {
                return this._dico.GetTranslate(IdentifyConstanteId.Scale120ID);
            }
        }
        public int ScaleFactor
        {
            get
            {
                return 20;
            }
        }
    }
    public class ComboBoxScale150
    {
        private Dico _dico = null;

        public ComboBoxScale150(Dico dico)
        {
            _dico = dico;
        }

        public string Name
        {
            get
            {
                return this._dico.GetTranslate(IdentifyConstanteId.Scale150ID);
            }
        }
        public int ScaleFactor
        {
            get
            {
                return 50;
            }
        }
    }
    public class ComboBoxScaleAjusted
    {
        private Dico _dico = null;

        public ComboBoxScaleAjusted(Dico dico)
        {
            _dico = dico;
        }

        public string Name
        {
            get
            {
                return this._dico.GetTranslate(IdentifyConstanteId.ScaleAjustedID);
            }
        }
        public int ScaleFactor
        {
            get
            {
                return 0;
            }
        }
    }

    public class ComboBoxPaperA4
    {
        private Dico _dico = null;

        public ComboBoxPaperA4(Dico dico)
        {
            _dico = dico;
        }

        public string Name
        {
            get
            {
                return this._dico.GetTranslate(IdentifyConstanteId.FormatA4ID);
            }
        }
        public string A4
        {
            get
            {
                return System.Printing.PageMediaSizeName.ISOA4.ToString();
            }
        }
        public int PaperFormat // System.Printing.PageMediaSizeName
        {
            get
            {
                return (int)System.Printing.PageMediaSizeName.ISOA4;
            }
        }
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
}
