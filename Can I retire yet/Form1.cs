using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using Can_I_retire_yet.functions;


namespace Can_I_retire_yet
{
    /// <summary>
    /// If I work another x years how much will that make extra per month/year.
    ///
    /// Add tab for future expenses and incomes.
    /// </summary>
    public partial class Form1 : Form
    {
        static public bool flag = true;
        //private DataGridViewCellEventArgs e = new DataGridViewCellEventArgs(0,0);


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text += " : v" + Assembly.GetExecutingAssembly().GetName().Version; // put in the version number

            DatagridviewFunctions.SetUpViews(dgv_expenses,2);
            DatagridviewFunctions.SetUpViews(dgv_assets, 2);
            DatagridviewFunctions.SetUpViews(dgv_income, 2);
            DatagridviewFunctions.SetUpViews(dgv_future_expenses, 3);
            DatagridviewFunctions.SetUpViews(dgv_future_income, 3);
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void lbl_expenses_add_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.AddRow(dgv_expenses);
        }

        private void lbl_expenses_delete_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.DeleteRow(dgv_expenses);
        }

        private void lbl_expenses_open_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.OpenFile(dgv_expenses);
        }

        private void lbl_expenses_save_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.SaveFile(dgv_expenses);
        }

        private void lbl_assets_open_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.OpenFile(dgv_assets);
        }

        private void lbl_assets_save_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.SaveFile(dgv_assets);
        }

        private void lbl_assets_add_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.AddRow(dgv_assets);
        }

        private void lbl_assets_delete_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.DeleteRow(dgv_assets);
        }

        private void lbl_income_open_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.OpenFile(dgv_income);
        }

        private void lbl_income_save_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.SaveFile(dgv_income);
        }

        private void lbl_income_add_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.AddRow(dgv_income);
        }

        private void lbl_income_delete_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.DeleteRow(dgv_income);
        }

        private void lbl_future_expenses_open_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.OpenFile(dgv_future_expenses);
        }

        private void lbl_future_expenses_save_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.SaveFile(dgv_future_expenses);
        }

        private void lbl_future_expenses_add_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.AddRow(dgv_future_expenses);
        }

        private void lbl_future_expenses_delete_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.DeleteRow(dgv_future_expenses);
        }

        private void lbl_future_income_open_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.OpenFile(dgv_future_income);
        }
    

        private void lbl_future_income_save_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.SaveFile(dgv_future_income);
        }

        private void lbl_future_income_add_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.AddRow(dgv_future_income);
        }

        private void lbl_future_income_delete_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.DeleteRow(dgv_future_income);
        }

        private void dgv_assets_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            //if (dgv_assets.CurrentCell.ColumnIndex == 1)
            //{
            //    lbl_assets_total.Text = dgv_assets.Rows.Cast<DataGridViewRow>()
            //        .AsEnumerable()
            //        .Sum(x => decimal.Parse(x.Cells[1].Value.ToString()))
            //        .ToString();

                
            //}
        }

        private void dgv_assets_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((dgv_assets.Columns[e.ColumnIndex].Name == "Amount") && (flag))
            {
                lbl_assets.Text = DatagridviewFunctions.CalculateTabTotal(dgv_assets, e);
            }
        }

        private void dgv_expenses_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((dgv_expenses.Columns[e.ColumnIndex].Name == "Monthly") && (flag))
            {
                lbl_expenses.Text = DatagridviewFunctions.CalculateTabTotal(dgv_expenses, e);
            }
        }

        private void dgv_income_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((dgv_income.Columns[e.ColumnIndex].Name == "Monthly") && (flag))
            {
                lbl_income.Text = DatagridviewFunctions.CalculateTabTotal(dgv_income, e);
            }
        }

 
    }
}
