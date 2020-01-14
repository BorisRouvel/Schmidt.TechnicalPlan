using System;
using System.Collections.Generic;
using System.Xml;

namespace Schmidt.TechnicalPlan
{
    public class TechnicalDocument
    {
        static private Dictionary<int, string> _dico = new Dictionary<int, string>();
       
        private string _type;
        private double _scaleFactor; // 0.05 if scale is 1/20
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
        public double ScaleFactor
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
            _scaleFactor = 1.0 / 20.0;
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

        public void ReadFromXml(XmlNode xmlNode)
        {

        }
        public void WriteToXml(List<TechnicalDocument> documentList, out XmlNode xmlNode)
        {
            

            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(@"D:\ic90dev\plugins\TechnicalPlan\TechnicalPlan.xml");
            }
            catch (Exception)
            {
                XmlWriter writer = XmlWriter.Create(@"D:\ic90dev\plugins\TechnicalPlan\TechnicalPlan.xml");
                writer.WriteComment("comment");

                // Write an element (this one is the root).
                writer.WriteStartElement("TechnicalDocument");

                // Write the namespace declaration.
                writer.WriteAttributeString("code", "");

                // Write the genre attribute.
                writer.WriteAttributeString("version", "1");

                writer.WriteEndElement();

                writer.Flush();
                writer.Close();
                writer.Dispose();

                WriteToXml(documentList, out xmlNode);
            }

            XmlNode root = xmlDocument.DocumentElement;

           
            //
            XmlNode xmlFirstNode = null;
            XmlNode xmlSecondNode = null;
            try
            {
                xmlDocument.SelectSingleNode("//Vues/Vue").InnerText = String.Empty;
                xmlFirstNode = xmlDocument.SelectSingleNode("//Vues");
                xmlSecondNode = xmlDocument.SelectSingleNode("//Vues/Vue");
            }
            catch (Exception)
            {
                xmlFirstNode = xmlDocument.CreateElement("Vues");
                xmlFirstNode.InnerText = String.Empty;
                root.AppendChild(xmlFirstNode);

                xmlSecondNode = xmlDocument.CreateElement("Vue");
                xmlSecondNode.InnerText = String.Empty;
                xmlFirstNode.AppendChild(xmlSecondNode);
            }


            foreach (TechnicalDocument doc in documentList)
            {
                XmlNode elem;
                elem = xmlDocument.CreateElement("Type");
                elem.InnerText = doc.Type;
                xmlSecondNode.AppendChild(elem);

                elem = xmlDocument.CreateElement("NomFichier");
                elem.InnerText = doc.FileName;
                xmlSecondNode.AppendChild(elem);

                elem = xmlDocument.CreateElement("Scale");
                elem.InnerText = doc.ScaleFactor.ToString();
                xmlSecondNode.AppendChild(elem);

                elem = xmlDocument.CreateElement("Format");
                elem.InnerText = doc.PageMediaSizeName.ToString();
                xmlSecondNode.AppendChild(elem);

                elem = xmlDocument.CreateElement("Orientation");
                elem.InnerText = doc.PageOrientation.ToString();
                xmlSecondNode.AppendChild(elem);

                elem = xmlDocument.CreateElement("Date");
                elem.InnerText = DateTime.Now.ToString();
                xmlSecondNode.AppendChild(elem);
                
            }

            xmlDocument.Save(@"D:\ic90dev\plugins\TechnicalPlan\TechnicalPlan.xml");

            xmlNode = xmlFirstNode;
        }

        
    }
}
