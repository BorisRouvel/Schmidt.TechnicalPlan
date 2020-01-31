using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Schmidt.TechnicalPlan
{
    public partial class TechnicalPlanDocumentFileNameForm : Form
    {
        private string _fileName = String.Empty;

        private KD.Plugin.Word.Plugin _pluginWord = null;
        private Dico _dico = null;

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public TechnicalPlanDocumentFileNameForm(KD.Plugin.Word.Plugin pluginWord, Dico dico)
        {
            InitializeComponent();

            _pluginWord = pluginWord;
            _dico = dico;

            this.InitializeForm();
        }


        // Event
        private void InitializeForm()
        {
            this.ok_TechPlanUI_BTN.Text = _dico.GetTranslate(TranslateIdentifyId.OkTechPlanButtonUIID);
            this.cancel_TechPlanUI_BTN.Text = _dico.GetTranslate(TranslateIdentifyId.CancelTechPlanButtonUIID);
            this.Text = _dico.GetTranslate(TranslateIdentifyId.TitleTechPlanButtonUIID);
            this.DialogResult = DialogResult.None;
        }
        private void SetTechnicalFileName()
        {
            TechnicalDocument.TechnicalFileName = this.FileName;            
        }


        // Form              
        private void TechnicalPlanDocumentFileNameForm_Load(object sender, EventArgs e)
        {            
        }

        private void ok_TechPlanUI_BTN_Click(object sender, EventArgs e)
        {
            this.SetTechnicalFileName();
            this.DialogResult = DialogResult.OK;
            //this.Close();
        }

        private void cancel_TechPlanUI_BTN_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void technicalPlanFiileName_TBX_TextChanged(object sender, EventArgs e)
        {
            _fileName = technicalPlanFiileName_TBX.Text;
        }
    }
}
