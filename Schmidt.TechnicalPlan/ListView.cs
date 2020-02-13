using System;
using System.Windows.Forms;


namespace Schmidt.TechnicalPlan
{
    public class MyListView : System.Windows.Forms.ListView
    {

        public class Enum
        {
            public enum ColumnIndex
            {
                UnKnown = KD.Const.UnknownId,
                Select = 0,
                View = 1,
                Scale = 2,
                Paper = 3,
                Orientation = 4,
                Overview = 5
            }
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public MyListView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call  

        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion

        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;

        protected override void WndProc(ref Message msg)
        {
            // Look for the WM_VSCROLL or the WM_HSCROLL messages.
            if ((msg.Msg == WM_VSCROLL) || (msg.Msg == WM_HSCROLL))
            {
                // Move focus to the ListView to cause ComboBox to lose focus.
                this.Focus();
            }

            // Pass message to default handler.
            base.WndProc(ref msg);
        }
    }

    public class MyImageButton : System.Windows.Forms.Button
    {
        public MyImageButton()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call  
        }

        private void InitializeComponent()
        {
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = Properties.Resources.twin;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Visible = false;
            this.Tag = String.Empty;
        }
    }
}
