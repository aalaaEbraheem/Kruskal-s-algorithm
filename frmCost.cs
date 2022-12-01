using System;

namespace Kruskal_s_algorithm
{
    public partial class frmCost : DevExpress.XtraEditors.XtraForm
    {
        public int m_nCost;
        public frmCost()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtCost.Text == string.Empty)
            {
                errorProvider1.SetError(txtCost, "Please enter value");
            }
            else
            {
                m_nCost = int.Parse(txtCost.Text);
                this.Close();
            }
        }
        private void frmCost_Load(object sender, EventArgs e)
        {
            txtCost.Select();
        }
    }
}