using System.IO;

using Syncfusion.Licensing;

using KD.Plugin;


namespace Schmidt.TechnicalPlan
{
    public class RessourceNames
    {       
        public const string IconeFileName = "ico.png";
        public const string IconeResourceDir = "Resources";
        public const string ImageResourceSubDir = @"data\images";
        public const string TextResourceSubDir = @"data\text";
        public const string MessageRulesResourceFileName = "messagesrules.csv";
        public static string MessageDialogResourceFileName = "messagesdialogsSchmidt.TechnicalPlan.csv";        
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

            TechnicalDocument.Dico.Add((int)KD.SDK.SceneEnum.ViewMode.VECTELEVATION, _dico.GetTranslate(TranslateIdentifyId.WallID));
            TechnicalDocument.Dico.Add((int)KD.SDK.SceneEnum.ViewMode.TOP, _dico.GetTranslate(TranslateIdentifyId.TopViewID));
            TechnicalDocument.Dico.Add((int)KD.SDK.SceneEnum.ViewMode.VECTELEVATION + 100, _dico.GetTranslate(TranslateIdentifyId.WallElevation));
            TechnicalDocument.Dico.Add((int)KD.SDK.SceneEnum.ViewMode.TOP + 100, _dico.GetTranslate(TranslateIdentifyId.TopView));

            PageOrientationSubItem.Dico.Add((int)System.Printing.PageOrientation.Portrait, _dico.GetTranslate(TranslateIdentifyId.PortraitID));
            PageOrientationSubItem.Dico.Add((int)System.Printing.PageOrientation.Landscape, _dico.GetTranslate(TranslateIdentifyId.LandscapeID));        

            PageMediaSizeNameSubItem.Dico.Add((int)System.Printing.PageMediaSizeName.ISOA4, _dico.GetTranslate(TranslateIdentifyId.FormatA4ID));
            PageMediaSizeNameSubItem.Dico.Add((int)System.Printing.PageMediaSizeName.ISOA3, _dico.GetTranslate(TranslateIdentifyId.FormatA3ID));

            ScaleFactorSubItem.Dico.Add(SubItemsConst.scaleFactor1_20, _dico.GetTranslate(TranslateIdentifyId.Scale120ID));
            ScaleFactorSubItem.Dico.Add(SubItemsConst.scaleFactor1_50, _dico.GetTranslate(TranslateIdentifyId.Scale150ID));
            ScaleFactorSubItem.Dico.Add(SubItemsConst.scaleFactorAuto, _dico.GetTranslate(TranslateIdentifyId.ScaleAutoID));          

        }

        public new bool OnPluginLoad(int iCallParamsBlock)
        {            
            _menu.IconFileName = Path.Combine(Directory.GetCurrentDirectory(), RessourceNames.IconeResourceDir, RessourceNames.IconeFileName);
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
                _menu = new MainAppMenuItem((int)KD.SDK.AppliEnum.SceneMenuItemsId.INFO, _dico.GetTranslate(TranslateIdentifyId.FunctionNameID), this.AssemblyFileName, KD.Plugin.Const.PluginClassName, "Main");
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

}
