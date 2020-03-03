using System.Linq;
using System;
using System.Reflection;

using KD.Plugin;

namespace Schmidt.TechnicalPlan
{
    public class ConstRessourceNames
    {
        public const string IconeFileName = "callplugin.png";
        public const string SM2FileName = "SM2.png";

    }

    public class Plugin 
    {
        public static SellerResponsabilityMessageForm sellerResponsabilityMessageForm = null;
        private MainAppMenuItem _technicalPlanMenu = null;       
        public static KD.Plugin.Word.Plugin pluginWord = null;

        public Plugin()
        {
            pluginWord = new KD.Plugin.Word.Plugin();

            this.InitializeTechnicalPlan();

            if (KD.Plugin.Word.TechnicalPlan.keyValueConfigurationCollection != null)
            {
                if (KD.Plugin.Word.TechnicalPlan.keyValueConfigurationCollection.AllKeys.Contains(KD.Plugin.Word.Config.Const.GenerateTechnicalPlanDocumentType))
                {
                    KD.Plugin.Word.Plugin._isCallByExterne = System.Convert.ToBoolean(KD.Plugin.Word.TechnicalPlan.keyValueConfigurationCollection[KD.Plugin.Word.Config.Const.GenerateTechnicalPlanDocumentType].Value);
                }
            }
            this.InitializeMenuItem();
        }

        private void InitializeTechnicalPlan()
        {
            if (KD.Plugin.Word.Plugin._technicalPlan == null)
            {
                KD.Plugin.Word.Plugin._technicalPlan = new KD.Plugin.Word.TechnicalPlan(pluginWord);
            }
        }
        private void InitializeMenuItem()
        {
            if (_technicalPlanMenu == null)
            {
                string assemblyName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetCallingAssembly().Location);
                _technicalPlanMenu = new MainAppMenuItem((int)KD.SDK.AppliEnum.SceneMenuItemsId.INFO,
                                                            KD.Plugin.Word.TechnicalPlan._dico.GetTranslate(KD.Plugin.Word.TranslateIdentifyId.FunctionName_ID),
                                                            assemblyName,
                                                            KD.Plugin.Const.PluginClassName,
                                                            "TechnicalPlanMainMethod");
            }
        }

        public bool OnPluginLoad(int iCallParamsBlock)
        {
            if (_technicalPlanMenu != null && KD.Plugin.Word.Plugin._isCallByExterne)
            {
                KD.Plugin.Word.Plugin._technicalPlan.PluginLoad(_technicalPlanMenu, ConstRessourceNames.IconeFileName);
            }
            return true;
        }
        public bool OnPluginUnload(int iCallParamsBlock)
        {
            if (_technicalPlanMenu != null && KD.Plugin.Word.Plugin._isCallByExterne)
            {
                KD.Plugin.Word.Plugin._technicalPlan.PluginUnload(_technicalPlanMenu);
            }
            return true;
        }

        public bool OnFileOpenBefore(int lCallParamsBlock)
        {
            if (_technicalPlanMenu != null && KD.Plugin.Word.Plugin._isCallByExterne)
            {
                KD.Plugin.Word.Plugin._technicalPlan.ActiveMenu(_technicalPlanMenu, true);
            }
            return true;
        }
        public bool OnFileNewBefore(int lCallParamsBlock)
        {
            if (_technicalPlanMenu != null && KD.Plugin.Word.Plugin._isCallByExterne)
            {
                KD.Plugin.Word.Plugin._technicalPlan.ActiveMenu(_technicalPlanMenu, true);
            }
            return true;
        }
        public bool OnFileCloseAfter(int lCallParamsBlock)
        {
            if (_technicalPlanMenu != null && KD.Plugin.Word.Plugin._isCallByExterne)
            {
                KD.Plugin.Word.Plugin._technicalPlan.ActiveMenu(_technicalPlanMenu, false);
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
