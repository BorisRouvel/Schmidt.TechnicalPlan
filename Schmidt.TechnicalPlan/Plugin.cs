using System.IO;

using Syncfusion.Licensing;

using KD.Plugin;


namespace Schmidt.TechnicalPlan
{   
    public class ConstComboBox
    {
        public const string PartialScaleName = "SCALE";
        public const string PartialPaperName = "PAPER";
        public const string PartialOrientationName = "ORIENTATION";

    }
    public class ConstRessourceNames
    {       
        public const string IconeFileName = "callplugin.png";
        public const string IconeResourceDir = "Resources";

        //public const string TechnicalPlanPreviewDirName = "TECHNICAL_PLAN_PREVIEW";
        //public const string DocTechnicalPlanDir = "DocTechnicalPlan";

        public const string TopViewFileNameHeader = "TOP";
        public const string ElevationViewFileNameHeader = "ELEV";

        public const string ConfigInSituDir = "$(InSituDir)";
        public const string ConfigSceneDir = "$(SceneDir)";

        public const string IsPrintButtonVisible = "IsPrintButtonVisible";
        public const string IsOpenFolderButtonVisible = "IsOpenFolderButtonVisible";
        public const string PreviewDirectory = "PreviewDirectory";
        public const string SM2Directory = "SM2Directory";
        public const string TemplateDocumentDirectory = "TemplateDocumentDirectory";
        public const string TechnicalPlanLayer = "TechnicalPlanLayer";
    }

    public class Plugin : KD.Plugin.PluginBase
    {
        System.Configuration.Configuration _configurationFile = null;

        private TechnicalDocument technicalDocument = null;
        private MainAppMenuItem _menu = null;
        private SellerResponsabilityMessageForm sellerResponsabilityMessageForm = null;
        private Dico _dico = null;
        public GenerateViewDialogForm viewDialogForm = null;

        public System.Configuration.Configuration ConfigurationFile
        {
            get
            {
                return _configurationFile;
            }
            set
            {
                _configurationFile = value;
            }
        }
        System.Configuration.KeyValueConfigurationCollection keyValueConfigurationCollection;

        public static KD.Plugin.Word.Plugin pluginWord = null;

        public static bool IsPrintButtonVisible;
        public static bool IsOpenFolderButtonVisible;
        public static string PreviewDirectory;
        public static string SM2Directory;
        public static string TemplateDocumentDirectory;
        public static string TechnicalPlanLayer;


        public Plugin()
        {
            SyncfusionLicenseProvider.RegisterLicense("MTkzMDk4QDMxMzcyZTM0MmUzMEJjQmV2NjRGRzhPVHVnZ3hzd1RCT09vNEZIOTQvZ3lNdnIrV1N4Ui9pQWc9");
            SyncfusionLicenseProvider.RegisterLicense("MTkzMDk5QDMxMzcyZTM0MmUzMGJRQ09tR1JiSU9Td1BtTEpzOWJHTFRSelFHaVNkT2xMd0VlcmxmbVZhcFU9");

            pluginWord = new KD.Plugin.Word.Plugin();

            this.InitConfig();

            Plugin.IsPrintButtonVisible = System.Convert.ToBoolean(keyValueConfigurationCollection[ConstRessourceNames.IsPrintButtonVisible].Value);
            Plugin.IsOpenFolderButtonVisible = System.Convert.ToBoolean(keyValueConfigurationCollection[ConstRessourceNames.IsOpenFolderButtonVisible].Value);

            Plugin.PreviewDirectory = keyValueConfigurationCollection[ConstRessourceNames.PreviewDirectory].Value;

            Plugin.SM2Directory = keyValueConfigurationCollection[ConstRessourceNames.SM2Directory].Value;

            Plugin.TemplateDocumentDirectory = keyValueConfigurationCollection[ConstRessourceNames.TemplateDocumentDirectory].Value;
            Plugin.TemplateDocumentDirectory = TemplateDocumentDirectory.Replace(ConstRessourceNames.ConfigInSituDir, this.CurrentAppli.ExeDir);

            Plugin.TechnicalPlanLayer = keyValueConfigurationCollection[ConstRessourceNames.TechnicalPlanLayer].Value;


            this.InitializeDico();
            this.InitializeMenuItem();            

            TechnicalDocument.Dico.Add((int)KD.SDK.SceneEnum.ViewMode.VECTELEVATION, _dico.GetTranslate(TranslateIdentifyId.Elevation_ID)); //Elevation
            TechnicalDocument.Dico.Add((int)KD.SDK.SceneEnum.ViewMode.TOP, _dico.GetTranslate(TranslateIdentifyId.Top_ID)); //Dessus
            TechnicalDocument.Dico.Add((int)KD.SDK.SceneEnum.ViewMode.VECTELEVATION + 100, _dico.GetTranslate(TranslateIdentifyId.WallElevation_ID)); //Elévation du mur...nb
            TechnicalDocument.Dico.Add((int)KD.SDK.SceneEnum.ViewMode.TOP + 100, _dico.GetTranslate(TranslateIdentifyId.TopView_ID)); //Vue de dessus
            TechnicalDocument.Dico.Add((int)KD.SDK.SceneEnum.ViewMode.VECTELEVATION + 200, _dico.GetTranslate(TranslateIdentifyId.SymbolElevation_ID)); //Elévation...nb (Elevation symbol)

            PageOrientationSubItem.Dico.Add((int)System.Printing.PageOrientation.Portrait, _dico.GetTranslate(TranslateIdentifyId.Portrait_ID));
            PageOrientationSubItem.Dico.Add((int)System.Printing.PageOrientation.Landscape, _dico.GetTranslate(TranslateIdentifyId.Landscape_ID));

            PageMediaSizeNameSubItem.Dico.Add((int)System.Printing.PageMediaSizeName.ISOA4, _dico.GetTranslate(TranslateIdentifyId.ISOA4_ID));
            PageMediaSizeNameSubItem.Dico.Add((int)System.Printing.PageMediaSizeName.ISOA3, _dico.GetTranslate(TranslateIdentifyId.ISOA3_ID));

            ScaleFactorSubItem.Dico.Add(SubItemsConst.scaleFactor1_20, _dico.GetTranslate(TranslateIdentifyId._1_20_ID));
            ScaleFactorSubItem.Dico.Add(SubItemsConst.scaleFactor1_25, _dico.GetTranslate(TranslateIdentifyId._1_25_ID));
            ScaleFactorSubItem.Dico.Add(SubItemsConst.scaleFactor1_50, _dico.GetTranslate(TranslateIdentifyId._1_50_ID));
            ScaleFactorSubItem.Dico.Add(SubItemsConst.scaleFactorAuto, _dico.GetTranslate(TranslateIdentifyId.Auto_ID));

        }

        public new bool OnPluginLoad(int iCallParamsBlock)
        {            
            _menu.IconFileName = Path.Combine(Directory.GetCurrentDirectory(), ConstRessourceNames.IconeResourceDir, ConstRessourceNames.IconeFileName);
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
                _dico = new Dico(this.CurrentAppli, this.CurrentAppli.GetLanguage(), Plugin.TemplateDocumentDirectory);
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
                _menu = new MainAppMenuItem((int)KD.SDK.AppliEnum.SceneMenuItemsId.INFO, _dico.GetTranslate(TranslateIdentifyId.FunctionName_ID), this.AssemblyFileName, KD.Plugin.Const.PluginClassName, "Main");
            }
        }
        private void InitializeViewDialogForm()
        {
            if (viewDialogForm == null)
            {                
                viewDialogForm = new GenerateViewDialogForm(this, pluginWord, _dico, technicalDocument); //         
            }
        }

        /// <summary>
        /// Find user config file and load it
        /// if user config file is not found :
        /// Find reference config file in a subfolder ("\Config") and copy it to assembly dir
        /// This process is useful in case of InSitu update. User preferences are kept if any were saved
        /// But in the same time default preferences are set at the begining for very first use
        /// </summary>
        private void InitConfig()
        {           
            string myselfPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string myselfName = System.IO.Path.GetFileName(myselfPath);
            string configFileName = System.IO.Path.ChangeExtension(myselfName, "dll.config");
            string myselfDir = System.IO.Path.GetDirectoryName(myselfPath);

            string userConfigPath = System.IO.Path.Combine(myselfDir, configFileName);
            string refConfigPath = System.IO.Path.Combine(myselfDir, @"Config\" + configFileName);

            if (!System.IO.File.Exists(userConfigPath))
            {
                if (!System.IO.File.Exists(refConfigPath))
                {
                    throw new System.Exception(refConfigPath + ". Config file does not exists. Plugin can't work w<ithout its config file");
                }
                System.IO.File.Copy(refConfigPath, userConfigPath);
            }
            // System.Configuration.ConfigurationManager.AppSettings["HasDialog"] work only from an exe

            // so we use another way by loading the config file from executing path
            // http://www.sep.com/sep-blog/2010/09/05/configuration-settings-for-net-class-libraries-dlls/
            this.ConfigurationFile = System.Configuration.ConfigurationManager.OpenExeConfiguration(myselfPath);

            keyValueConfigurationCollection = this.ConfigurationFile.AppSettings.Settings;
        }
        private void InitConfigDirectories()
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(pluginWord.CurrentAppli.SceneName);
            pluginWord.Config.SceneDocDir = System.IO.Path.Combine(pluginWord.CurrentAppli.ScenesDir, sceneName);

            Plugin.SM2Directory = Plugin.SM2Directory.Replace(ConstRessourceNames.ConfigSceneDir, pluginWord.Config.SceneDocDir);
            Plugin.PreviewDirectory = Plugin.PreviewDirectory.Replace(ConstRessourceNames.ConfigSceneDir, pluginWord.Config.SceneDocDir);
        }

        private void DisableMessageForm()
        {
            this.sellerResponsabilityMessageForm = null;
        }

        public bool Main(int lCallParamsBlock)
        {
            //this.InitializeMessageForm();
            //this.ShowSellerResponsabilityMessage();
            //if (!sellerResponsabilityMessageForm.IsSellerResponsabilityMessage())
            //{                
            //    return false;
            //}
           
            //this.DisableMessageForm();
            this.InitializeViewDialogForm();

            pluginWord.InitializeAll(lCallParamsBlock);

            this.InitConfigDirectories();
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

        public void SetScaleFactorOnDocWord(int lCallParamsBlock)
        {
            string CurrentScaleFactor = GenerateViewDialogForm.CurrentScaleFactor;
            this.CurrentAppli.SetCallParamsInfoDirect(lCallParamsBlock, CurrentScaleFactor, KD.SDK.AppliEnum.CallParamId.BUFFER);
        }
        public void SetPageMediaSizeNameOnDocWord(int lCallParamsBlock)
        {
            string CurrentPageMediaSizeName = GenerateViewDialogForm.CurrentPageMediaSizeName;
            this.CurrentAppli.SetCallParamsInfoDirect(lCallParamsBlock, CurrentPageMediaSizeName, KD.SDK.AppliEnum.CallParamId.BUFFER);
        }
        public void SetPageOrientationOnDocWord(int lCallParamsBlock)
        {
            string CurrentPageOrientation = GenerateViewDialogForm.CurrentPageOrientation;
            this.CurrentAppli.SetCallParamsInfoDirect(lCallParamsBlock, CurrentPageOrientation, KD.SDK.AppliEnum.CallParamId.BUFFER);
        }
    }

}
