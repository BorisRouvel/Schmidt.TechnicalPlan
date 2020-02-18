using System;
using System.IO;

namespace Schmidt.TechnicalPlan
{
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

        public Dico(KD.SDKComponent.AppliComponent appli, string language, string templateDocDir)
        {
            this.CurrentAppli = appli;
            this._language = language;
            this._dicoFileDir = templateDocDir; //appli.DocDir;
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

    public class TranslateIdentifyId
    {
        public const string FunctionName_ID = "FunctionName_ID";

        public const string SellerMessageOkButton_ID = "SellerMessageOkButton_ID";
        public const string SellerMessageCancelButton_ID = "SellerMessageCancelButton_ID";
        public const string SellerMessageText_ID = "SellerMessageText_ID";

        public const string UITitle_ID = "UITitle_ID";
        public const string UIOkButton_ID = "UIOkButton_ID";
        public const string UICancelButton_ID = "UICancelButton_ID";
        public const string UISelectAllViewCheckBox_ID = "UISelectAllViewCheckBox_ID";

        public const string TechnicalPlanOkButton_ID = "TechnicalPlanOkButton_ID";
        public const string TechnicalPlanCancelButton_ID = "TechnicalPlanCancelButton_ID";
        public const string TechnicalPlanTitle_ID = "TechnicalPlanTitle_ID";

        public const string ColumnHeaderSelect_ID = "ColumnHeaderSelect_ID";
        public const string ColumnHeaderView_ID = "ColumnHeaderView_ID";
        public const string ColumnHeaderScale_ID = "ColumnHeaderScale_ID";
        public const string ColumnHeaderPaper_ID = "ColumnHeaderPaper_ID";
        public const string ColumnHeaderOrientation_ID = "ColumnHeaderOrientation_ID";
        public const string ColumnHeaderOverview_ID = "ColumnHeaderOverview_ID";

       
        public const string Top_ID = "Top_ID"; //Dessus (type)
        public const string Elevation_ID = "Elevation_ID"; //Elévation (type)
        public const string TopView_ID = "TopView_ID"; //Vue de dessus
        public const string WallElevation_ID = "WallElevation_ID";  //Elévation du mur...nb
        public const string SymbolElevation_ID = "SymbolElevation_ID"; //Elévation...nb (Elevation symbol)

        public const string Unknown_ID = "Unknown_ID";
        public const string Portrait_ID = "Portrait_ID";
        public const string Landscape_ID = "Landscape_ID";
        public const string _1_20_ID = "1_20_ID";
        public const string _1_25_ID = "1_25_ID";
        public const string _1_50_ID = "1_50_ID";
        public const string Auto_ID = "Auto_ID";
        public const string ISOA4_ID = "ISOA4_ID";
        public const string ISOA3_ID = "ISOA3_ID";

        public const string MessageInfo_01_ID = "MessageInfo_01_ID";

    }
}
