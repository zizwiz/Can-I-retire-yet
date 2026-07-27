using System;
using System.Globalization;
using System.Windows.Forms;
using System.Reflection;
using Can_I_retire_yet.functions;
using Can_I_retire_yet.MonteCarlo;

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
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text += " : v" + Assembly.GetExecutingAssembly().GetName().Version; // put in the version number

            DatagridviewFunctions.SetUpViews(dgv_expenses, 2);
            DatagridviewFunctions.SetUpViews(dgv_assets, 2);
            DatagridviewFunctions.SetUpViews(dgv_income, 2);
            DatagridviewFunctions.SetUpViews(dgv_future_expenses, 3);
            DatagridviewFunctions.SetUpViews(dgv_future_income, 3);





            lbl_trackbar_value.Text = $"Value: {trkbr_retirement_age.Value}";
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            SaveInfo();
            Close();
        }

        private void btn_save_all_Click(object sender, EventArgs e)
        {
            SaveInfo();
        }

        private void SaveInfo()
        {
            //This is where we save all the info to file
            //When we press save it asks for location to save it and the name but we append name with _DDMMMYYYYY 
        }

        private void lbl_expenses_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_expenses)){}
        }

        private void lbl_assets_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_assets)) { }
        }

        private void lbl_income_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_income)) { }
        }

        private void lbl_future_expenses_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_future_expenses)) { }
        }

        private void lbl_future_income_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_future_income)) { }
        }
        private void lbl_cash_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_cash)) { }
        }

        private void lbl_stocks_shares_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_stocks_shares)) { }
        }

        private void lbl_bonds_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_bonds)) { }
        }

        private void lbl_savings_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_savings)) { }
        }

        private bool DataGridViewAdd(DataGridView dgv)
        {
            try
            {
                DatagridviewFunctions.AddRow(dgv);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception: " + e);
                return false;
            }
        }

        private void lbl_expenses_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_expenses)) { }
        }

        private void lbl_assets_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_assets)) { }
        }

        private void lbl_income_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_income)) { }
        }

        private void lbl_future_expenses_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_future_expenses)) { }
        }

        private void lbl_future_income_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_future_income)) { }
        }

        private void lbl_cash_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_cash)) { }
        }

        private void lbl_savings_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_savings)) { }
        }

        private void lbl_stocks_shares_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_stocks_shares)) { }
        }

        private void lbl_bonds_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_bonds)) { }
        }

        private bool DataGridViewDelete(DataGridView dgv)
        {
            try
            {
                DatagridviewFunctions.DeleteRow(dgv);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception: " + e);
                return false;
            }
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

        private void dgv_future_expenses_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((dgv_future_expenses.Columns[e.ColumnIndex].Name == "Amount") && (flag))
            {
                lbl_future_expenses.Text = DatagridviewFunctions.CalculateTabTotal(dgv_future_expenses, e);
            }
        }

        private void dgv_future_income_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((dgv_future_income.Columns[e.ColumnIndex].Name == "Amount") && (flag))
            {
                lbl_future_income.Text = DatagridviewFunctions.CalculateTabTotal(dgv_future_income, e);
            }
        }

        private void lbl_totals_TextChanged(object sender, EventArgs e)
        {
            lbl_1st_pass_total.Text = ((decimal.Parse(lbl_assets.Text, NumberStyles.Currency, CultureInfo.CreateSpecificCulture("en-GB").NumberFormat)
                                                                    + decimal.Parse(lbl_income.Text, NumberStyles.Currency, CultureInfo.CreateSpecificCulture("en-GB").NumberFormat))
                                                                   - decimal.Parse(lbl_expenses.Text, NumberStyles.Currency, CultureInfo.CreateSpecificCulture("en-GB").NumberFormat)).ToString("C", new CultureInfo("en-GB"));//     .Format(new CultureInfo("en-GB"), "{0:C}");
        }

        private void btn_open_all_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.OpenFile(dgv_assets);
            DatagridviewFunctions.OpenFile(dgv_income);
            DatagridviewFunctions.OpenFile(dgv_expenses);
            DatagridviewFunctions.OpenFile(dgv_future_income);
            DatagridviewFunctions.OpenFile(dgv_future_expenses);
        }

        private void btn_run_monte_carlo_Click(object sender, EventArgs e)
        {

            try
            {
                var mc = new RetirementMonteCarlo(
                    initialBalance: 1_000_000, // Starting portfolio
                    annualWithdrawal: 40_000, // Annual spending
                    stockMeanReturn: (double.Parse(txtbx_stock_returns.Text) / 100), // 7% avg stock return
                    stockStdDev: (double.Parse(txtbx_stock_volatility.Text) / 100), // 15% volatility
                    bondMeanReturn: (double.Parse(txtbx_avg_bond_return.Text) / 100), // 3% avg bond return
                    bondStdDev: (double.Parse(txtbx_stock_volatility.Text) / 100), // 5% volatility
                    stockAllocation: (double.Parse(txtbx_stock_allocation.Text) / 100), // 60% stocks, 40% bonds
                    years: 100 - trkbr_retirement_age.Value, // Retirement length 0 - 50 years
                    simulations: int.Parse(txtbx_monte_carlo_iterations.Text) // Number of Monte Carlo runs
                );

                double successProbability = mc.RunSimulation();
                rchtxtbx_monte_carlo_output.AppendText($"Probability of not running out of money: {successProbability:P2}\r");

            }
            catch (Exception ex)
            {
                rchtxtbx_monte_carlo_output.AppendText($"Error: {ex.Message}\r");
            }
        }

        private void trkbr_retirement_length_Scroll(object sender, EventArgs e)
        {
            lbl_trackbar_value.Text = $"Value: {trkbr_retirement_age.Value}";
        }





    }
}
