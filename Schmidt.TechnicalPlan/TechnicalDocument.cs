using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Schmidt.TechnicalPlan
{
    public class TechnicalDocument
    {
        const string rootTag = "<TechnicalDocument></TechnicalDocument>";
        const string documentTag = "Vue";
        const string typeTag = "Type";
        const string fileNameTag = "NomFichier";
        const string scaleTag = "Scale";
        const string formatTag = "Format";
        const string orientationTag = "Orientation";
        const string dateTag = "Date";


        static private Dictionary<int, string> _dico = new Dictionary<int, string>();
       
        private string _type;
        private string _fileName;
        private string _scaleFactorAsString; // 0.05 if scale is 1/20
        private double _scaleFactor; // 0.05 if scale is 1/20
        private System.Printing.PageOrientation _pageOrientation;
        private System.Printing.PageMediaSizeName _pageMediaSizeName;
        private string _viewName;
        private KD.SDK.SceneEnum.ViewMode _viewMode;
        private int _objectID;
        private string _number;

        public static Dictionary<int, string> Dico
        {
            get { return _dico; }
            set { _dico = value; }
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
        public double ScaleFactor
        {
            get { return _scaleFactor; }
            set { _scaleFactor = value; }
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
        public System.Printing.PageMediaSizeName PageMediaSizeName
        {
            get { return _pageMediaSizeName; }
            set { _pageMediaSizeName = value; }
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
            _fileName = String.Empty;           
            _scaleFactorAsString = ScaleFactorSubItem.Dico[SubItemsConst.scaleFactor1_20];
            _pageOrientation = System.Printing.PageOrientation.Portrait;
            _pageMediaSizeName = System.Printing.PageMediaSizeName.ISOA4;
            _viewMode = KD.SDK.SceneEnum.ViewMode.UNKNOWN;
            _objectID = KD.Const.UnknownId;
        }


        public TechnicalDocument()
        {            
            InitMembers();
        }
        //public TechnicalDocument(string filePath)
        //{
        //    _fileName = System.IO.Path.GetFileName(filePath);
        //   // InitMembers();
        //}
     
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
                xmlNodeScaleList = xmlDocument.GetElementsByTagName(scaleTag);
                xmlNodeFormatList = xmlDocument.GetElementsByTagName(formatTag);
                xmlNodeOrientationList = xmlDocument.GetElementsByTagName(orientationTag);
            }
           
        }
        public void WriteToXml(List<TechnicalDocument> documentList, out XmlNode xmlNode)
        {
            XmlDocument xmlDocument = new XmlDocument();           
            xmlDocument.LoadXml(rootTag);

            foreach (TechnicalDocument doc in documentList)
            {
                XmlNode root;               
                root = xmlDocument.CreateElement(documentTag);
                xmlDocument.DocumentElement.AppendChild(root);

                XmlNode elem;
                elem = xmlDocument.CreateElement(typeTag);
                elem.InnerText = doc.Type;
                root.AppendChild(elem);                
                
                elem = xmlDocument.CreateElement(fileNameTag);
                elem.InnerText = doc.FileName;
                root.AppendChild(elem);

                elem = xmlDocument.CreateElement(scaleTag);
                elem.InnerText = doc.ScaleFactorAsString;
                root.AppendChild(elem);

                elem = xmlDocument.CreateElement(formatTag);
                elem.InnerText = doc.PageMediaSizeName.ToString();
                root.AppendChild(elem);

                elem = xmlDocument.CreateElement(orientationTag);
                elem.InnerText = doc.PageOrientation.ToString();
                root.AppendChild(elem);

                elem = xmlDocument.CreateElement(dateTag);
                elem.InnerText = DateTime.Now.ToString();
                root.AppendChild(elem);

            }
#if DEBUG
            xmlDocument.Save(@"D:\ic90dev\plugins\TechnicalPlan.xml");
#endif
            xmlNode = xmlDocument.FirstChild; 
        }

        
    }
}
