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
        private string _scaleFactor; // 0.05 if scale is 1/20
        private System.Printing.PageOrientation _pageOrientation;
        private System.Printing.PageMediaSizeName _pageMediaSizeName;
        private string _fileName;
        private KD.SDK.SceneEnum.ViewMode _viewMode;
        private int _objectID;
        private string _marq;

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
        public string ScaleFactor
        {
            get { return _scaleFactor; }
            set { _scaleFactor = value; }
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
        public string FileName
        {
            get { return TechnicalDocument.Dico[(int)this._viewMode + 100] + KD.StringTools.Const.WhiteSpace + this._marq; }
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
        public string Marq
        {
            get { return _marq; }
            set { _marq = value; }
        }

        private void InitMembers()
        {
            _scaleFactor = ScaleFactorSubItem.Dico[(int)ScaleFactorSubItem.ScaleFactorEnum.Scale120];//1.0 / 20.0; //
            _pageOrientation = System.Printing.PageOrientation.Portrait;
            _pageMediaSizeName = System.Printing.PageMediaSizeName.ISOA4;
            _viewMode = KD.SDK.SceneEnum.ViewMode.UNKNOWN;
            _objectID = KD.Const.UnknownId;
        }


        public TechnicalDocument()
        {            
            InitMembers();
        }
        public TechnicalDocument(string filePath)
        {
            _fileName = System.IO.Path.GetFileName(filePath);
           // InitMembers();
        }
     
        public override string ToString()
        {
            return FileName;
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
                elem.InnerText = doc.ScaleFactor.ToString();
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

            xmlDocument.Save(@"D:\ic90dev\plugins\TechnicalPlan\TechnicalPlan.xml");

            xmlNode = xmlDocument.FirstChild; 
        }

        
    }
}
