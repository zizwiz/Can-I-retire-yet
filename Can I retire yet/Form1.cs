using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using Can_I_retire_yet.functions;
using Can_I_retire_yet.Models;
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
        private string lastFilePath = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text += " : v" + Assembly.GetExecutingAssembly().GetName().Version; // put in the version number

            DatagridviewFunctions.SetUpViews(dgv_expenses, 2);
            DatagridviewFunctions.SetUpViews(dgv_cash, 2);
            DatagridviewFunctions.SetUpViews(dgv_savings, 2);
            DatagridviewFunctions.SetUpViews(dgv_bonds, 2);
            DatagridviewFunctions.SetUpViews(dgv_stocks_shares, 2);
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
            var saveDialog = new SaveFileDialog
            {
                InitialDirectory = string.IsNullOrEmpty(lastFilePath)
                    ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    : Path.GetDirectoryName(lastFilePath),

                Filter = "JSON files (*.json)|*.json",
                DefaultExt = "json",
                AddExtension = true,
                Title = "Save Retirement Data"
            };

            if (saveDialog.ShowDialog() != DialogResult.OK)
                return;

            string timestamp = DateTime.Now.ToString("_ddMMMyyyy_HHmmss");
            string finalPath = Path.Combine(
                Path.GetDirectoryName(saveDialog.FileName),
                Path.GetFileNameWithoutExtension(saveDialog.FileName) + timestamp + ".json"
            );

            lastFilePath = finalPath;

            var data = new SavedData
            {
                assets = DatagridviewFunctions.ExtractGrid(dgv_assets),
                cash = DatagridviewFunctions.ExtractGrid(dgv_cash),
                savings = DatagridviewFunctions.ExtractGrid(dgv_savings),
                bonds = DatagridviewFunctions.ExtractGrid(dgv_bonds),
                stocks_shares = DatagridviewFunctions.ExtractGrid(dgv_stocks_shares),
                income = DatagridviewFunctions.ExtractGrid(dgv_income),
                expenses = DatagridviewFunctions.ExtractGrid(dgv_expenses),
                future_income = DatagridviewFunctions.ExtractGrid(dgv_future_income),
                future_expenses = DatagridviewFunctions.ExtractGrid(dgv_future_expenses)
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(finalPath, json);

            MessageBox.Show("Saved successfully.");
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

        private void dgv_cash_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((dgv_cash.Columns[e.ColumnIndex].Name == "Amount") && (flag))
            {
                lbl_cash.Text = DatagridviewFunctions.CalculateTabTotal(dgv_cash, e);
            }
        }

        private void dgv_savings_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((dgv_savings.Columns[e.ColumnIndex].Name == "Amount") && (flag))
            {
                lbl_savings.Text = DatagridviewFunctions.CalculateTabTotal(dgv_savings, e);
            }
        }

        private void dgv_stocks_shares_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((dgv_stocks_shares.Columns[e.ColumnIndex].Name == "Amount") && (flag))
            {
                lbl_stocks_shares.Text = DatagridviewFunctions.CalculateTabTotal(dgv_stocks_shares, e);
            }
        }

        private void dgv_bonds_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((dgv_bonds.Columns[e.ColumnIndex].Name == "Amount") && (flag))
            {
                lbl_bonds.Text = DatagridviewFunctions.CalculateTabTotal(dgv_bonds, e);
            }
        }

        private void lbl_totals_TextChanged(object sender, EventArgs e)
        {
            lbl_1st_pass_total.Text = ((decimal.Parse(lbl_assets.Text, NumberStyles.Currency,
                                            CultureInfo.CreateSpecificCulture("en-GB").NumberFormat)
                                        + decimal.Parse(lbl_income.Text, NumberStyles.Currency,
                                            CultureInfo.CreateSpecificCulture("en-GB").NumberFormat)
                                        + decimal.Parse(lbl_stocks_shares.Text, NumberStyles.Currency,
                                            CultureInfo.CreateSpecificCulture("en-GB").NumberFormat)
                                        + decimal.Parse(lbl_bonds.Text, NumberStyles.Currency,
                                            CultureInfo.CreateSpecificCulture("en-GB").NumberFormat)
                                        + decimal.Parse(lbl_savings.Text, NumberStyles.Currency,
                                            CultureInfo.CreateSpecificCulture("en-GB").NumberFormat)
                                        + decimal.Parse(lbl_cash.Text, NumberStyles.Currency,
                                            CultureInfo.CreateSpecificCulture("en-GB").NumberFormat)).ToString("C", new CultureInfo("en-GB"))); ;


            Label23.Text = ((decimal.Parse(lbl_1st_pass_total.Text, NumberStyles.Currency, CultureInfo.CreateSpecificCulture("en-GB").NumberFormat)
                - decimal.Parse(lbl_expenses.Text, NumberStyles.Currency, CultureInfo.CreateSpecificCulture("en-GB").NumberFormat)).ToString("C", new CultureInfo("en-GB")));
        }

        private void btn_open_all_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                InitialDirectory = string.IsNullOrEmpty(lastFilePath)
                    ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    : Path.GetDirectoryName(lastFilePath),

                Filter = "JSON files (*.json)|*.json",
                DefaultExt = "json",
                Title = "Open Retirement Data"
            };

            if (openDialog.ShowDialog() != DialogResult.OK)
                return;

            lastFilePath = openDialog.FileName;

            string json = File.ReadAllText(openDialog.FileName);
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<SavedData>(json);

            flag = false;

            DatagridviewFunctions.LoadGrid(dgv_assets, data.assets);
            DatagridviewFunctions.LoadGrid(dgv_cash, data.cash);
            DatagridviewFunctions.LoadGrid(dgv_savings, data.savings);
            DatagridviewFunctions.LoadGrid(dgv_bonds, data.bonds);
            DatagridviewFunctions.LoadGrid(dgv_stocks_shares, data.stocks_shares);
            DatagridviewFunctions.LoadGrid(dgv_income, data.income);
            DatagridviewFunctions.LoadGrid(dgv_expenses, data.expenses);
            DatagridviewFunctions.LoadGrid(dgv_future_income, data.future_income);
            DatagridviewFunctions.LoadGrid(dgv_future_expenses, data.future_expenses);

            RecalculateAllTotals();

            flag = true;
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

        private void RecalculateAllTotals()
        {
            lbl_assets.Text = DatagridviewFunctions.CalculateTabTotal(dgv_assets, new DataGridViewCellEventArgs(1, 0));
            lbl_cash.Text = DatagridviewFunctions.CalculateTabTotal(dgv_cash, new DataGridViewCellEventArgs(1, 0));
            lbl_savings.Text = DatagridviewFunctions.CalculateTabTotal(dgv_savings, new DataGridViewCellEventArgs(1, 0));
            lbl_bonds.Text = DatagridviewFunctions.CalculateTabTotal(dgv_bonds, new DataGridViewCellEventArgs(1, 0));
            lbl_stocks_shares.Text = DatagridviewFunctions.CalculateTabTotal(dgv_stocks_shares, new DataGridViewCellEventArgs(1, 0));

            lbl_income.Text = DatagridviewFunctions.CalculateTabTotal(dgv_income, new DataGridViewCellEventArgs(1, 0));
            lbl_expenses.Text = DatagridviewFunctions.CalculateTabTotal(dgv_expenses, new DataGridViewCellEventArgs(1, 0));

            lbl_future_income.Text = DatagridviewFunctions.CalculateTabTotal(dgv_future_income, new DataGridViewCellEventArgs(2, 0));
            lbl_future_expenses.Text = DatagridviewFunctions.CalculateTabTotal(dgv_future_expenses, new DataGridViewCellEventArgs(2, 0));
        }

        private void btn_new_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.NewSetUp(dgv_assets, dgv_cash, dgv_savings, dgv_bonds, dgv_stocks_shares, dgv_income, dgv_expenses,
                dgv_future_income, dgv_future_expenses);
        }
    }
}
