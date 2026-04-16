using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;

namespace Accounting.Core.Forms
{
    public partial class frm_ReportViewer : Form
    {
        public frm_ReportViewer(ReportDocument report)
        {
            InitializeComponent();
            crystalReportViewer1.ReportSource = report;
          
        }

        private void frm_ReportViewer_Load(object sender, EventArgs e)
        {

        }
    }
}
