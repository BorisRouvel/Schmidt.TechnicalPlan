using System;
using System.Windows.Forms;

namespace Schmidt.TechnicalPlan
{
    public partial class SellerResponsabilityMessageForm : Form
    {
        private bool ValidButton = false;

        private KD.Plugin.Word.Dico _dico = null;
        public KD.Plugin.Word.Dico Dico
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

        public SellerResponsabilityMessageForm(KD.Plugin.Word.Dico dico)
        {
            InitializeComponent();

            this._dico = dico;
        }

        private void InitializeText()
        {
            this.Ok_BTN.Text = this.Dico.GetTranslate(KD.Plugin.Word.TranslateIdentifyId.SellerMessageOkButton_ID);//, "Ok / Continuer");
            this.Cancel_BTN.Text = this.Dico.GetTranslate(KD.Plugin.Word.TranslateIdentifyId.SellerMessageCancelButton_ID);//, "Retourner dans InSitu");
            this.SellerResponsabilityText_LAB.Text = this.Dico.GetTranslate(KD.Plugin.Word.TranslateIdentifyId.SellerMessageText_ID);//, "Nous rappelons que Plan Technique engage la responsabilité du vendeur.");
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
