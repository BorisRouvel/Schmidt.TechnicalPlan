using System;

using KD.Plugin;

namespace Schmidt.TechnicalPlan
{
    public class ConstRessourceNames
    {
        public const string IconeFileName = "callplugin.png";     
    }
    public class ConstConfig
    {
        public const string TechnicalPlanSchmidtMenuItem = "TechnicalPlanSchmidtMenuItem";
    }

    public class Plugin 
    {
        public static KD.Plugin.Word.Plugin _pluginWord = null;
        public static SellerResponsabilityMessageForm sellerResponsabilityMessageForm = null;

        private MainAppMenuItem _technicalPlanMenuItem= null;              

        private string assemblyName = String.Empty;

        public Plugin()
        {
            _pluginWord = new KD.Plugin.Word.Plugin();

            KD.Plugin.Word.Config config = new KD.Plugin.Word.Config(_pluginWord.CurrentAppli, _pluginWord.CurrentAppli.Language);

            assemblyName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            try
            {
                string myselfPath = System.Reflection.Assembly.GetExecutingAssembly().Location;                              
                string myselfDir = System.IO.Path.GetDirectoryName(myselfPath);
                string userConfigPath = System.IO.Path.Combine(myselfDir, assemblyName);
                
                config.ConfigurationFile = System.Configuration.ConfigurationManager.OpenExeConfiguration(userConfigPath);
            }
            catch (System.Exception)
            {
                return;
            }

            if (!config.ContainsKey(ConstConfig.TechnicalPlanSchmidtMenuItem))
            {
                return;
            }

            bool bTechnicalPlanSchmidtMenuItem = config.GetBooleanConfigValue(ConstConfig.TechnicalPlanSchmidtMenuItem);
            if (bTechnicalPlanSchmidtMenuItem & System.IO.Directory.Exists(KD.Plugin.Word.TechnicalPlan.TechnicalPlanTemplateDocumentDirectory))
            {
                this.InitializeTechnicalPlan();
                this.InitializeMenuItem();
            }           
                    
        }

        private void InitializeTechnicalPlan()
        {
            if (KD.Plugin.Word.Plugin._technicalPlan == null)
            {
                KD.Plugin.Word.Plugin._technicalPlan = new KD.Plugin.Word.TechnicalPlan(_pluginWord);                       
            }
        }

        private void InitializeMenuItem()
        {
            if (_technicalPlanMenuItem == null)
            {                
                _technicalPlanMenuItem = new MainAppMenuItem((int)KD.SDK.AppliEnum.SceneMenuItemsId.INFO,
                                                            KD.Plugin.Word.TechnicalPlan._dico.GetTranslate(KD.Plugin.Word.TranslateIdentifyId.FunctionName_ID),
                                                            assemblyName,
                                                            KD.Plugin.Const.PluginClassName,
                                                            "TechnicalPlanMainMethod");
            }
        }        

        public bool OnPluginLoad(int iCallParamsBlock)
        {
            if (_technicalPlanMenuItem != null )
            {              
                KD.Plugin.Word.Plugin._technicalPlan.PluginLoad(_technicalPlanMenuItem, ConstRessourceNames.IconeFileName);
            }
            return true;
        }
        public bool OnPluginUnload(int iCallParamsBlock)
        {
            if (_technicalPlanMenuItem != null )
            {
                KD.Plugin.Word.Plugin._technicalPlan.PluginUnload(_technicalPlanMenuItem);
            }
            return true;
        }

        public bool OnFileOpenAfter(int iCallParamsBlock)
        {
            if (_technicalPlanMenuItem != null)
            {
                KD.Plugin.Word.Plugin._technicalPlan.ActiveMenu(_technicalPlanMenuItem, true);
            }
            return true;
        }
        public bool OnFileNewAfter(int iCallParamsBlock)
        {
            if (_technicalPlanMenuItem != null)
            {
                KD.Plugin.Word.Plugin._technicalPlan.ActiveMenu(_technicalPlanMenuItem, true);
            }
            return true;
        }        
        public bool OnFileCloseAfter(int iCallParamsBlock)
        {
            if (_technicalPlanMenuItem != null)
            {
                KD.Plugin.Word.Plugin._technicalPlan.ActiveMenu(_technicalPlanMenuItem, false);
            }
            return true;
        }
        public bool OnAppCommandBefore(int iCallParamsBlock)
        {
            if (_technicalPlanMenuItem != null)
            {
                string menuCommandIdAsString = KD.Plugin.Word.TechnicalPlan._pluginWord.CurrentAppli.GetCallParamsInfoDirect(iCallParamsBlock, KD.SDK.AppliEnum.CallParamId.MENUCOMMANDID);
                KD.Plugin.Word.Plugin._technicalPlan.GetCurrentObjectIDForViewElevation(menuCommandIdAsString);
            }
            return true;
        }
        public bool OnAppCommandAfter(int iCallParamsBlock)
        {
            if (_technicalPlanMenuItem != null)
            {
                string menuCommandIdAsString = KD.Plugin.Word.TechnicalPlan._pluginWord.CurrentAppli.GetCallParamsInfoDirect(iCallParamsBlock, KD.SDK.AppliEnum.CallParamId.MENUCOMMANDID);
                KD.Plugin.Word.Plugin._technicalPlan.InitializeCurrentObjectIDForViewElevation(menuCommandIdAsString);

                KD.Plugin.Word.Plugin._technicalPlan.ManageStateOfModifiedScene();
            }

            return true;
        }

        public bool TechnicalPlanMainMethod(int iCallParamsBlock)
        {
            this.InitializeSellerResponsabilityMessageForm();
            this.ShowSellerResponsabilityMessage();

            if (!sellerResponsabilityMessageForm.IsSellerResponsabilityMessage())
            {
                return false;
            }

            this.DisableMessageForm();

            KD.Plugin.Word.TechnicalPlan._pluginWord.TechnicalPlanMainMethod(iCallParamsBlock);
           
            return true;
        }

        public void InitializeSellerResponsabilityMessageForm()
        {
            if (sellerResponsabilityMessageForm == null)
            {
                sellerResponsabilityMessageForm = new SellerResponsabilityMessageForm(KD.Plugin.Word.TechnicalPlan._dico);
            }
        }
        public void ShowSellerResponsabilityMessage()
        {
            sellerResponsabilityMessageForm.ShowDialog();
        }

        public void DisableMessageForm()
        {
            sellerResponsabilityMessageForm = null;
        }

        public bool SetScaleFactorOnDocWord(int iCallParamsBlock)
        {
            return KD.Plugin.Word.TechnicalPlan._pluginWord.SetScaleFactorOnDocWord(iCallParamsBlock);
        }
        public bool SetPageMediaSizeNameOnDocWord(int iCallParamsBlock)
        {
            return KD.Plugin.Word.TechnicalPlan._pluginWord.SetPageMediaSizeNameOnDocWord(iCallParamsBlock);
        }
        public bool SetPageOrientationOnDocWord(int iCallParamsBlock)
        {
            return KD.Plugin.Word.TechnicalPlan._pluginWord.SetPageOrientationOnDocWord(iCallParamsBlock);
        }
    }
    
}
