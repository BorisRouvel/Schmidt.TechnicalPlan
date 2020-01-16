using System;
using System.Windows.Forms;

namespace Schmidt.TechnicalPlan
{
    public partial class SellerResponsabilityMessageForm : Form
    {       
        private bool ValidButton = false;

        private Dico _dico = null;
        public Dico Dico
        {
            get
            {
                return _dico;
            }
            set
            {
                _dico = value;
            }
        }

        public SellerResponsabilityMessageForm(Dico dico)
        {
            InitializeComponent();

            this._dico = dico;
        }

        private void InitializeText()
        {
            this.Ok_BTN.Text = this.Dico.GetTranslate(TranslateIdentifyId.OkButtonID);//, "Ok / Continuer");
            this.Cancel_BTN.Text = this.Dico.GetTranslate(TranslateIdentifyId.CancelButtonID);//, "Retourner dans InSitu");
            this.SellerResponsabilityText_LAB.Text = this.Dico.GetTranslate(TranslateIdentifyId.SellerMessageID);//, "Nous rappelons que Plan Technique engage la responsabilité du vendeur.");
        }

        public bool IsSellerResponsabilityMessage()
        {
            return this.ValidButton;
        }

        private void Ok_BTN_Click(object sender, EventArgs e)
        {
            this.ValidButton = true;
            this.Close();
        }

        private void Cancel_BTN_Click(object sender, EventArgs e)
        {
            this.ValidButton = false;
            this.Close();
        }

        private void SellerResponsabilityMessageForm_Load(object sender, EventArgs e)
        {
            this.InitializeText();
        }
    }
}
