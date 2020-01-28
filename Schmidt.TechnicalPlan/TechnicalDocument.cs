using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Schmidt.TechnicalPlan
{
    public class TechnicalDocument
    {
        public const string technicalDocumentsTag = "TechnicalDocuments";
        public const string technicalPlanFileNameTag = "TechnicalPlanFileName";
        const string rootTag = "<TechnicalDocument></TechnicalDocument>";
        const string documentTag = "Vue";
        const string typeTag = "Type";
        const string fileNameTag = "FileName";
        const string scaleFactorTag = "ScaleFactor";
        const string scaleFactorValueTag = "ScaleFactorValue";
        const string formatTag = "Format";
        const string formatIDTag = "FormatID";
        const string orientationTag = "Orientation";
        const string orientationIDTag = "OrientationID";
        const string dateTag = "Date";


        static private Dictionary<int, string> _dico = new Dictionary<int, string>();

        private static string _technicalFileName;
        private string _type;
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
            get { return TechnicalDocument.Dico[(int)this._viewMode + 100] + KD.StringTools.Const.WhiteSpace + this._number; }
           
        }
        public KD.SDK.SceneEnum.ViewMode ViewMode
        {
            get { return _viewMode ; }
            set { _viewMode = value;
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
            _fileName = String.Empty;           
            _scaleFactorAsString = ScaleFactorSubItem.Dico[SubItemsConst.scaleFactor1_20];
            _scaleFactorValue = SubItemsConst.scaleFactor1_20;
            
            _pageMediaSizeName = System.Printing.PageMediaSizeName.ISOA4;
            _pageMediaSizeNameID = (int)System.Printing.PageMediaSizeName.ISOA4;

            _pageOrientation = System.Printing.PageOrientation.Portrait;
            _pageOrientationID = (int)System.Printing.PageOrientation.Portrait;

            _viewMode = KD.SDK.SceneEnum.ViewMode.UNKNOWN;
            _objectID = KD.Const.UnknownId;
        }

        public TechnicalDocument()
        {            
            InitMembers();
        }
        
     
        public override string ToString()
        {
            return ViewName;
        }

        public void ReadFromXml(string xmlString, out XmlNodeList xmlNodeScaleList, out XmlNodeList xmlNodeFormatList, out XmlNodeList xmlNodeOrientationList)
        {
            XmlDocument xmlDocument = null;
            xmlNodeScaleList = null;
            xmlNodeFormatList = null;
            xmlNodeOrientationList = null;

            try
            {
                xmlDocument = new XmlDocument(); //XDocument.Parse(xmlString,LoadOptions.PreserveWhitespace);
                xmlDocument.LoadXml(xmlString);
            }
            catch (Exception)
            {
                ;
            }

            if (xmlDocument != null)
            {               
                xmlNodeScaleList = xmlDocument.GetElementsByTagName(scaleFactorTag);
                xmlNodeFormatList = xmlDocument.GetElementsByTagName(formatTag);
                xmlNodeOrientationList = xmlDocument.GetElementsByTagName(orientationTag);
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
                root = xmlDocument.CreateElement(documentTag);
                xmlDocument.DocumentElement.AppendChild(root);

                //XmlNode elem;
                this.ElementAdd(xmlDocument, root, typeTag, doc.Type);
                this.ElementAdd(xmlDocument, root, fileNameTag, doc.FileName);
                this.ElementAdd(xmlDocument, root, scaleFactorTag, doc.ScaleFactorAsString);
                this.ElementAdd(xmlDocument, root, scaleFactorValueTag, doc.ScaleFactorValue.ToString());
                this.ElementAdd(xmlDocument, root, formatTag, doc.PageMediaSizeName.ToString());
                this.ElementAdd(xmlDocument, root, formatIDTag, doc.PageMediaSizeNameID.ToString());
                this.ElementAdd(xmlDocument, root, orientationTag, doc.PageOrientation.ToString());
                this.ElementAdd(xmlDocument, root, orientationIDTag, doc.PageOrientationID.ToString());
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

        
    }
}
