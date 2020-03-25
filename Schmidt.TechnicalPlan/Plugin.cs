using System.Linq;

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
        public static SellerResponsabilityMessageForm sellerResponsabilityMessageForm = null;
        private MainAppMenuItem _technicalPlanMenuItem= null;       
        public static KD.Plugin.Word.Plugin _pluginWord = null; 

        public Plugin()
        {
            _pluginWord = new KD.Plugin.Word.Plugin();

            KD.Plugin.Word.Config config = new KD.Plugin.Word.Config(_pluginWord.CurrentAppli, _pluginWord.CurrentAppli.Language);

            string assemblyName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            try
            {
                config.ConfigurationFile = System.Configuration.ConfigurationManager.OpenExeConfiguration(assemblyName);
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
            if (bTechnicalPlanSchmidtMenuItem)
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
                string assemblyName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
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

        public bool OnFileOpenBefore(int lCallParamsBlock)
        {
            if (_technicalPlanMenuItem != null)
            {
                KD.Plugin.Word.Plugin._technicalPlan.ActiveMenu(_technicalPlanMenuItem, true);
            }
            return true;
        }
        public bool OnFileNewBefore(int lCallParamsBlock)
        {
            if (_technicalPlanMenuItem != null)
            {
                KD.Plugin.Word.Plugin._technicalPlan.ActiveMenu(_technicalPlanMenuItem, true);
            }
            return true;
        }
        public bool OnFileCloseAfter(int lCallParamsBlock)
        {
            if (_technicalPlanMenuItem != null)
            {
                KD.Plugin.Word.Plugin._technicalPlan.ActiveMenu(_technicalPlanMenuItem, false);
            }
            return true;
        }

        public bool TechnicalPlanMainMethod(int lCallParamsBlock)
        {
            this.InitializeMessageForm();
            this.ShowSellerResponsabilityMessage();

            if (!sellerResponsabilityMessageForm.IsSellerResponsabilityMessage())
            {
                return false;
            }

            this.DisableMessageForm();

            KD.Plugin.Word.TechnicalPlan._pluginWord.TechnicalPlanMainMethod(lCallParamsBlock);
            return true;
        }

        public void InitializeMessageForm()
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

    }


    
}
