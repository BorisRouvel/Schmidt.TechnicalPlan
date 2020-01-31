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

    public class TranslateIdentifyId
    {
        public const string FunctionNameID = "FunctionNameID";

        public const string OkButtonID = "OkButtonID";
        public const string CancelButtonID = "CancelButtonID";
        public const string SellerMessageID = "SellerMessageID";

        public const string OkButtonUIID = "OkButtonUIID";
        public const string CancelButtonUIID = "CancelButtonUIID";
        public const string SelectAllViewCheckBoxUIID = "SelectAllViewCheckBoxUIID";

        public const string OkTechPlanButtonUIID = "OkTechPlanButtonUIID";
        public const string CancelTechPlanButtonUIID = "CancelTechPlanButtonUIID";
        public const string TitleTechPlanButtonUIID = "TitleTechPlanButtonUIID";

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
        public const string ElevationSymbol = "ElevationSymbol";

        public const string PortraitID = "PortraitID";
        public const string LandscapeID = "LandscapeID";
        public const string Scale120ID = "Scale120ID";
        public const string Scale150ID = "Scale150ID";
        public const string ScaleAutoID = "ScaleAutoID";
        public const string FormatA4ID = "FormatA4ID";
        public const string FormatA3ID = "FormatA3ID";

    }
}
