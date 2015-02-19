using System;
using System.Linq;
using System.Windows.Forms;

namespace DataMigration
{
    public partial class Migrate : Form
    {
        public Migrate()
        {
            InitializeComponent();
        }

        private void btnMigrate_Click(object sender, EventArgs e)
        {
            Helper.ProcessData(lstConfigItems.CheckedItems.Cast<string>().ToArray());
        }
    }
}
