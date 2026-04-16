using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Data.SqlClient;

namespace Accounting.Core
{
    public partial class frm_DayDetails : DevExpress.XtraEditors.XtraForm
    {
        public frm_DayDetails()
        {
            InitializeComponent();
        }
        private DateTime _date;
        private string _connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        private readonly string connectionString =
    @"Data Source=.\SQLEXPRESS;
      Initial Catalog=AccountingCoreDB;
      Integrated Security=True";
        public frm_DayDetails(DateTime date)
        {
            InitializeComponent();
            _date = date;
        }
        private void frm_DayDetails_Load(object sender, EventArgs e)
        {

       
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
SELECT 
    'فاتورة' AS Type,
    InvoiceNumber AS Number,
    TotalAfterTax AS Amount
FROM Invoices
WHERE CAST(InvoiceDate AS DATE) = @date

UNION ALL

SELECT 
    'مصروف',
    Description,
    Credit
FROM CashTransactions
WHERE RefType = 'Expense'
AND CAST(TransDate AS DATE) = @date";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@date", _date);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                gridControl1.DataSource = dt;
            }
        }
    }
}