using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;

//using Syncfusion.DocIO;
//using Syncfusion.DocIO.DLS;
//using Syncfusion.DocToPDFConverter;
using Syncfusion.Licensing;

using KD.Plugin;
using KD.Model;
using KD.StringTools;
using KD.FilterBuilder;



namespace Schmidt.TechnicalPlan
{
    public class Const
    {       
        public const string IconeFileName = "ico.png";
        public const string IconeResourceDir = "Resources";
        public const string ImageResourceSubDir = @"data\images";
        public const string TextResourceSubDir = @"data\text";
        public const string MessageRulesResourceFileName = "messagesrules.csv";
        public static string MessageDialogResourceFileName = "messagesdialogsSchmidt.TechnicalPlan.csv";        

        public const int Decimal = 2; //  1 / 100

    }
    public class Plugin : KD.Plugin.PluginBase
    {
        private TechnicalDocument technicalDocument = null;
        private MainAppMenuItem _menu = null;
        private SellerResponsabilityMessageForm sellerResponsabilityMessageForm = null;
        private Dico _dico = null;
        public GenerateViewDialogForm viewDialogForm = null;
        
        public static KD.Plugin.Word.Plugin pluginWord = null;

        public Plugin()
        {
            SyncfusionLicenseProvider.RegisterLicense("MTkzMDk4QDMxMzcyZTM0MmUzMEJjQmV2NjRGRzhPVHVnZ3hzd1RCT09vNEZIOTQvZ3lNdnIrV1N4Ui9pQWc9");
            SyncfusionLicenseProvider.RegisterLicense("MTkzMDk5QDMxMzcyZTM0MmUzMGJRQ09tR1JiSU9Td1BtTEpzOWJHTFRSelFHaVNkT2xMd0VlcmxmbVZhcFU9");

            pluginWord = new KD.Plugin.Word.Plugin();

            this.InitializeDico();
            this.InitializeMenuItem();

            TechnicalDocument.Dico.Add((int)KD.SDK.SceneEnum.ViewMode.VECTELEVATION, _dico.GetTranslate(IdentifyConstanteId.WallID));
            TechnicalDocument.Dico.Add((int)KD.SDK.SceneEnum.ViewMode.TOP, _dico.GetTranslate(IdentifyConstanteId.TopViewID));
            TechnicalDocument.Dico.Add((int)KD.SDK.SceneEnum.ViewMode.VECTELEVATION + 100, _dico.GetTranslate(IdentifyConstanteId.WallElevation));
            TechnicalDocument.Dico.Add((int)KD.SDK.SceneEnum.ViewMode.TOP + 100, _dico.GetTranslate(IdentifyConstanteId.TopView));

            PageOrientationUI.Dico.Add((int)System.Printing.PageOrientation.Portrait, _dico.GetTranslate(IdentifyConstanteId.PortraitID));
            PageOrientationUI.Dico.Add((int)System.Printing.PageOrientation.Landscape, _dico.GetTranslate(IdentifyConstanteId.LandscapeID));        

            PageMediaSizeNameUI.Dico.Add((int)System.Printing.PageMediaSizeName.ISOA4, _dico.GetTranslate(IdentifyConstanteId.FormatA4ID));
            PageMediaSizeNameUI.Dico.Add((int)System.Printing.PageMediaSizeName.ISOA3, _dico.GetTranslate(IdentifyConstanteId.FormatA3ID));

            ScaleFactorUI.Dico.Add((int)ScaleFactorUI.ScaleFactorEnum.Scale120, _dico.GetTranslate(IdentifyConstanteId.Scale120ID));
            ScaleFactorUI.Dico.Add((int)ScaleFactorUI.ScaleFactorEnum.Scale150, _dico.GetTranslate(IdentifyConstanteId.Scale150ID));
            ScaleFactorUI.Dico.Add((int)ScaleFactorUI.ScaleFactorEnum.ScaleAuto, _dico.GetTranslate(IdentifyConstanteId.ScaleAutoID));          

        }

        public new bool OnPluginLoad(int iCallParamsBlock)
        {            
            _menu.IconFileName = Path.Combine(Directory.GetCurrentDirectory(), Const.IconeResourceDir, Const.IconeFileName);
            _menu.Insert(this.CurrentAppli);            
            this.ActiveMenu(false);
            return true;
        }
        public new bool OnPluginUnload(int iCallParamsBlock)
        {
            if (_menu.IsValid())
            {
                _menu.Remove(this.CurrentAppli);
            }
            return true;
        }

        public bool OnFileOpenBefore(int lCallParamsBlock)
        {
            this.ActiveMenu(true);
            this.technicalDocument = new TechnicalDocument();
            return true;
        }
        public bool OnFileNewBefore(int lCallParamsBlock)
        {
            this.ActiveMenu(true);
            this.technicalDocument = new TechnicalDocument();
            return true;
        }
        public bool OnFileCloseAfter(int lCallParamsBlock)
        {
            this.ActiveMenu(false);
            return true;
        }

        private void ActiveMenu(bool activate)
        {
            if (this._menu != null)
            {
                _menu.Enable(this.CurrentAppli, activate);
            }
        }
        private void InitializeDico()
        {
            if (_dico == null)
            {
                _dico = new Dico(this.CurrentAppli, this.CurrentAppli.GetLanguage());
            }
        }
        private void InitializeMessageForm()
        {
            if (sellerResponsabilityMessageForm == null)
            {
                sellerResponsabilityMessageForm = new SellerResponsabilityMessageForm(_dico);
            }
        }
        private void InitializeMenuItem()
        {
            if (_menu == null)
            {
                _menu = new MainAppMenuItem((int)KD.SDK.AppliEnum.SceneMenuItemsId.INFO, _dico.GetTranslate(IdentifyConstanteId.FunctionNameID), this.AssemblyFileName, KD.Plugin.Const.PluginClassName, "Main");
            }
        }
        private void InitializeViewDialogForm()
        {
            if (viewDialogForm == null)
            {                
                viewDialogForm = new GenerateViewDialogForm(this, pluginWord, _dico, technicalDocument); //         
            }
        }

        private void DisableMessageForm()
        {
            this.sellerResponsabilityMessageForm = null;
        }

        public bool Main(int lCallParamsBlock)
        {
            this.InitializeMessageForm();
            this.ShowSellerResponsabilityMessage();
            if (!sellerResponsabilityMessageForm.IsSellerResponsabilityMessage())
            {                
                return false;
            }
            //// Do work here 
            this.DisableMessageForm();
            this.InitializeViewDialogForm();

            pluginWord.InitializeAll(lCallParamsBlock);            
            this.ShowGenerateViewDialogForm();      

            return true;
        }

        private void ShowSellerResponsabilityMessage()
        {            
            sellerResponsabilityMessageForm.ShowDialog();
        }
        private void ShowGenerateViewDialogForm()
        {
            viewDialogForm.ShowDialog(this.CurrentAppli.GetNativeIWin32Window());           
        }
    }

    public class Dico
    {
        public class Const
        {
            public const string DicoIdentColumnTitle = "ident";
            public const string keyLanguage = "FRA";
            public const string name = "DocTechnicalPlan.lng";

        }

        private string _dicoFileName;
        public string DicoFileName
        {
            get
            {
                return _dicoFileName;
            }
            set
            {
                _dicoFileName = value;
            }
        }

        private string _language;
        public string Language
        {
            get
            {
                return _language;
            }
            set
            {
                _language = value;
            }
        }

        private string _dicoFileDir;
        public string DicoFileDir
        {
            get
            {
                return _dicoFileDir;
            }
            set
            {
                _dicoFileDir = value;
            }
        }

        private string _dicoFilePath;
        public string DicoFilePath
        {
            get
            {
                return _dicoFilePath;
            }
            set
            {
                _dicoFilePath = value;
            }
        }

        private readonly KD.SDKComponent.AppliComponent CurrentAppli = null;

        public Dico(KD.SDKComponent.AppliComponent appli, string language)
        {
            this.CurrentAppli = appli;
            this._language = language;
            this._dicoFileDir = appli.DocDir;
            this._dicoFileName = Const.name;
            this._dicoFilePath = Path.Combine(this.DicoFileDir, this.DicoFileName);

            this.Load();
        }

        private bool Load()
        {            
            bool load = this.CurrentAppli.Dico.FileLoad(this.DicoFilePath);
            if (!load)
            {
                load = this.CurrentAppli.Dico.FileNew(Const.DicoIdentColumnTitle); // Const.keyLanguage);
                load = this.Save();
            }            
            return load;
        }
        private bool Save()
        {
            return this.CurrentAppli.Dico.FileSave(this.DicoFilePath);
        }

        public bool Add(string ident)
        {
            bool line = this.CurrentAppli.Dico.InsertLine(ident, ident);           
            bool result = this.CurrentAppli.Dico.SetStringFromKey(ident, Const.DicoIdentColumnTitle, ident);
            if (result)
            {                
                if (this.Save())
                {
                    System.Windows.Forms.MessageBox.Show("Un nouveau texte à été ajouté au dico !" + Environment.NewLine + ident);
                }
            }
            return true;
        }

        public string GetTranslate(string ident)
        {
            string translate = this.CurrentAppli.Dico.GetStringFromKey(ident, this.Language);
            if (String.IsNullOrEmpty(translate) || ident == translate)
            {
                bool isKeyId = this.CurrentAppli.Dico.StringExists(Const.DicoIdentColumnTitle, ident);
                if (!isKeyId)
                {
                    this.Add(ident);
                }
            }
            return translate;           
        }
    }

    public class IdentifyConstanteId
    {
        public const string FunctionNameID = "FunctionNameID";

        public const string ColumnHeaderSelectID = "ColumnHeaderSelectID";
        public const string ColumnHeaderViewID = "ColumnHeaderViewID";
        public const string ColumnHeaderScaleID = "ColumnHeaderScaleID";
        public const string ColumnHeaderPaperID = "ColumnHeaderPaperID";
        public const string ColumnHeaderOrientationID = "ColumnHeaderOrientationID";
        public const string ColumnHeaderOverviewID = "ColumnHeaderOverviewID";

        public const string TopViewID = "TopViewID";
        public const string WallID = "WallID";
        public const string TopView = "TopView";
        public const string WallElevation = "WallElevation";

        public const string PortraitID = "PortraitID";
        public const string LandscapeID = "LandscapeID";
        public const string Scale120ID = "Scale120ID";
        public const string Scale150ID = "Scale150ID";
        public const string ScaleAutoID = "ScaleAutoID";
        public const string FormatA4ID = "FormatA4ID";
        public const string FormatA3ID = "FormatA3ID";
    }
}
