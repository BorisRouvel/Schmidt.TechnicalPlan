using System;
using System.Collections.Generic;
using System.Xml;

namespace Schmidt.TechnicalPlan
{
    public class TechnicalDocumentEnum
    {
        public enum viewTypeId { Top = 0, WallElevation = 1, SymbolElevation = 2 }
    }
    public class TechnicalDocument
    {
        public const string technicalDocumentsTag = "TechnicalDocuments";
        public const string technicalPlanFileNameTag = "TechnicalPlanFileName";
        public const string rootTag = "<TechnicalDocument></TechnicalDocument>";
        public const string viewTag = "Vue";
        public const string typeTag = "Type";
        public const string typeIDTag = "TypeID";
        public const string fileNameTag = "FileName";
        public const string scaleFactorTag = "ScaleFactor";
        public const string scaleFactorValueTag = "ScaleFactorValue";
        public const string formatTag = "Format";
        public const string formatIDTag = "FormatID";
        public const string orientationTag = "Orientation";
        public const string orientationIDTag = "OrientationID";
        public const string objectIDTag = "ObjectId";
        public const string numberTag = "Number";
        public const string dateTag = "Date";


        static private Dictionary<int, string> _dico = new Dictionary<int, string>();

        private static string _technicalFileName;
        private string _type;
        private int _typeID;
        private string _fileName;
        private string _scaleFactorAsString; //  1/20
        private double _scaleFactorValue; // 0.05 if scale is 1/20
        private System.Printing.PageOrientation _pageOrientation;
        private int _pageOrientationID;
        private System.Printing.PageMediaSizeName _pageMediaSizeName;
        private int _pageMediaSizeNameID;

        private KD.SDK.SceneEnum.ViewMode _viewMode;
        private int _objectID;
        private string _number;

        public static Dictionary<int, string> Dico
        {
            get { return _dico; }
            set { _dico = value; }
        }

        public static string TechnicalFileName
        {
            get { return _technicalFileName; }
            set { _technicalFileName = value; }
        }
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public int TypeID
        {
            get { return _typeID; }
            set { _typeID = value; }
        }
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        public double ScaleFactorValue
        {
            get { return _scaleFactorValue; }
            set { _scaleFactorValue = value; }
        }
        public string ScaleFactorAsString
        {
            get { return _scaleFactorAsString; }
            set { _scaleFactorAsString = value; }
        }
        public System.Printing.PageOrientation PageOrientation
        {
            get { return _pageOrientation; }
            set { _pageOrientation = value; }
        }
        public int PageOrientationID
        {
            get { return _pageOrientationID; }
            set { _pageOrientationID = value; }
        }
        public System.Printing.PageMediaSizeName PageMediaSizeName
        {
            get { return _pageMediaSizeName; }
            set { _pageMediaSizeName = value; }
        }
        public int PageMediaSizeNameID
        {
            get { return _pageMediaSizeNameID; }
            set { _pageMediaSizeNameID = value; }
        }
        public string ViewName
        {
            get
            {
                if (this._viewMode != KD.SDK.SceneEnum.ViewMode.UNKNOWN)
                {
                    string number = KD.StringTools.Const.WhiteSpace + this._number;
                    if (number == KD.StringTools.Const.WhiteSpace + KD.Const.UnknownId.ToString())
                    {
                        number = String.Empty;
                    }

                    KD.Model.Article article = new KD.Model.Article(Plugin.pluginWord.CurrentAppli, _objectID);
                    if (article.Type == (int)KD.SDK.SceneEnum.ObjectType.ELEVATIONSYMBOL)
                    {
                        return TechnicalDocument.Dico[(int)this._viewMode + 200] + number;
                    }
                    else if (article.Type == (int)KD.SDK.SceneEnum.ObjectType.WALL)
                    {
                        return TechnicalDocument.Dico[(int)this._viewMode + 100] + number;
                    }
                    else
                    {
                        return TechnicalDocument.Dico[(int)this._viewMode];
                    }
                }
                return String.Empty;
            }
        }

        public KD.SDK.SceneEnum.ViewMode ViewMode
        {
            get { return _viewMode; }
            set
            {
                _viewMode = value;
            }
        }
        public int ObjectID
        {
            get { return _objectID; }
            set { _objectID = value; }
        }
        public string Number
        {
            get { return _number; }
            set { _number = value; }
        }

        private void InitMembers()
        {
            _technicalFileName = String.Empty;
            _scaleFactorAsString = ScaleFactorSubItem.Dico[SubItemsConst.scaleFactor1_20];
            _scaleFactorValue = SubItemsConst.scaleFactor1_20;

            _pageMediaSizeName = System.Printing.PageMediaSizeName.ISOA3;
            _pageMediaSizeNameID = (int)System.Printing.PageMediaSizeName.ISOA3;

            _pageOrientation = System.Printing.PageOrientation.Landscape;
            _pageOrientationID = (int)System.Printing.PageOrientation.Landscape;

            _viewMode = KD.SDK.SceneEnum.ViewMode.UNKNOWN;
            _objectID = KD.Const.UnknownId;
            _number = KD.Const.UnknownId.ToString();

            _fileName = String.Empty;
           // _fileName = Guid.NewGuid().ToString();
        }

        public TechnicalDocument()
        {
            InitMembers();
        }

        public override string ToString()
        {
            return ViewName;
        }

        public void ReadFromXml(string xmlString, out List<TechnicalDocument> documentList)
        {
            documentList = new List<TechnicalDocument>();
            XmlDocument xmlDocument = null;
            XmlNode xmlNode = null;

            try
            {
                xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlString);
            }
            catch (Exception)
            {
            }

            if (xmlDocument != null && xmlDocument.HasChildNodes)
            {
                xmlNode = xmlDocument.FirstChild;
                XmlNodeList nodeList = xmlNode.SelectNodes(TechnicalDocument.viewTag);

                // mettre 1 ligne vue de dessus et mur avec leur id , faut l 'ecrire dans xml  // Here todo          
                int index = 0;

                foreach (XmlNode node in nodeList)
                {
                    TechnicalDocument docFromXml = new TechnicalDocument();
                    XmlNodeList childs = node.ChildNodes;

                    if (index == 0)
                    {
                        docFromXml.ViewMode = KD.SDK.SceneEnum.ViewMode.TOP;
                    }
                    else
                    {
                        docFromXml.ViewMode = KD.SDK.SceneEnum.ViewMode.VECTELEVATION;
                    }

                    foreach (XmlNode child in childs)
                    {
                        if (child.Name == TechnicalDocument.typeTag)
                        {
                            docFromXml.Type = child.InnerText;
                        }
                        if (child.Name == TechnicalDocument.typeIDTag)
                        {
                            docFromXml.TypeID = System.Convert.ToInt32(child.InnerText);
                        }
                        if (child.Name == TechnicalDocument.scaleFactorTag)
                        {
                            docFromXml.ScaleFactorAsString = child.InnerText;
                        }
                        if (child.Name == TechnicalDocument.scaleFactorValueTag)
                        {
                            docFromXml.ScaleFactorValue = System.Convert.ToDouble(child.InnerText);
                        }
                        if (child.Name == TechnicalDocument.formatTag)
                        {
                            docFromXml.PageMediaSizeName = new PageMediaSizeNameSubItem(child.InnerText).ToPageMediaSizeName();
                            GenerateViewDialogForm.generikPaperSizeText = child.InnerText;
                        }
                        if (child.Name == TechnicalDocument.formatIDTag)
                        {
                            docFromXml.PageMediaSizeNameID = System.Convert.ToInt32(child.InnerText);
                        }
                        if (child.Name == TechnicalDocument.orientationTag)
                        {
                            docFromXml.PageOrientation = new PageOrientationSubItem(child.InnerText).ToPageOrientation();
                        }
                        if (child.Name == TechnicalDocument.orientationIDTag)
                        {
                            docFromXml.PageOrientationID = System.Convert.ToInt32(child.InnerText);
                        }
                        if (child.Name == TechnicalDocument.objectIDTag)
                        {
                            docFromXml.ObjectID = System.Convert.ToInt32(child.InnerText);
                        }
                        if (child.Name == TechnicalDocument.numberTag)
                        {
                            docFromXml.Number = Plugin.pluginWord.CurrentAppli.Scene.ObjectGetInfo(docFromXml.ObjectID, KD.SDK.SceneEnum.ObjectInfo.NUMBER);  //child.InnerText;
                        }
                    }
                    documentList.Add(docFromXml);
                    index++;
                }
            }
        }
        public void WriteToXml(List<TechnicalDocument> documentList, out XmlNode xmlNode)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(rootTag);
            this.ElementAdd(xmlDocument, xmlDocument.DocumentElement, technicalPlanFileNameTag, TechnicalDocument.TechnicalFileName);

            foreach (TechnicalDocument doc in documentList)
            {
                XmlNode root;
                root = xmlDocument.CreateElement(viewTag);
                xmlDocument.DocumentElement.AppendChild(root);

                //XmlNode elem;
                this.ElementAdd(xmlDocument, root, typeTag, doc.Type);
                this.ElementAdd(xmlDocument, root, typeIDTag, doc.TypeID.ToString());
                this.ElementAdd(xmlDocument, root, fileNameTag, doc.FileName);
                this.ElementAdd(xmlDocument, root, scaleFactorTag, doc.ScaleFactorAsString);
                this.ElementAdd(xmlDocument, root, scaleFactorValueTag, doc.ScaleFactorValue.ToString());
                this.ElementAdd(xmlDocument, root, formatTag, doc.PageMediaSizeName.ToString());
                this.ElementAdd(xmlDocument, root, formatIDTag, doc.PageMediaSizeNameID.ToString());
                string po = PageOrientationSubItem.Dico[(int)doc.PageOrientation];
                this.ElementAdd(xmlDocument, root, orientationTag, po); //doc.PageOrientation.ToString());
                this.ElementAdd(xmlDocument, root, orientationIDTag, doc.PageOrientationID.ToString());
                this.ElementAdd(xmlDocument, root, objectIDTag, doc.ObjectID.ToString());
                this.ElementAdd(xmlDocument, root, numberTag, doc.Number);
                this.ElementAdd(xmlDocument, root, dateTag, DateTime.Now.ToString());


            }
#if DEBUG
            xmlDocument.Save(@"D:\ic90dev\plugins\TechnicalPlan.xml");
#endif
            xmlNode = xmlDocument.FirstChild;
        }

        private void ElementAdd(XmlDocument xmlDocument, XmlNode root, string name, string str)
        {
            XmlNode elem;
            elem = xmlDocument.CreateElement(name);
            elem.InnerText = str;
            root.AppendChild(elem);
        }

        public void DeleteXml()
        {
#if DEBUG
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(@"D:\ic90dev\plugins\TechnicalPlan.xml");
            xmlDocument.LoadXml(rootTag);
            xmlDocument.DocumentElement.RemoveAll();           
            xmlDocument.Save(@"D:\ic90dev\plugins\TechnicalPlan.xml");
#endif
        }
    }
}
