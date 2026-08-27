using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Windows.Forms.DataVisualization.Charting;
using Can_I_retire_yet.functions;
using Can_I_retire_yet.help;
using Can_I_retire_yet.Models;
using Can_I_retire_yet.MonteCarlo;
using CenteredMessagebox;

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

        private Timer chartTimer = new Timer { Interval = 150 };
        private Timer pensionTimer = new Timer { Interval = 10 };

        // Are we looking at Scottish Tax Bands
        private bool UseScottishTaxBands => rdobtn_scotland.Checked;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text += " : v" + Assembly.GetExecutingAssembly().GetName().Version; // put in the version number

            DatagridviewFunctions.SetUpViews(dgv_expenses, 2);
            DatagridviewFunctions.SetUpViews(dgv_tax_summary, 0);
            DatagridviewFunctions.SetUpViews(dgv_cash, 2);
            DatagridviewFunctions.SetUpViews(dgv_savings, 4);
            DatagridviewFunctions.SetUpViews(dgv_bonds, 2);
            DatagridviewFunctions.SetUpViews(dgv_stocks_shares, 2);
            DatagridviewFunctions.SetUpViews(dgv_assets, 2);
            DatagridviewFunctions.SetUpViews(dgv_income, 5);
            DatagridviewFunctions.SetUpViews(dgv_future_expenses, 3);
            DatagridviewFunctions.SetUpViews(dgv_uk_state_pension, 3);

            lbl_trackbar_value.Text = $"Value: {trkbr_retirement_age.Value}";

            cmbx_currency.SelectedIndex = 0; // default to £

            txtbx_salary.Text = $"£{thinsldr_salary.Value}";
            txtbx_age.Text = $"{thinsldr_Age.Value}";
            txtbx_length.Text = $"{thinsldr_Length.Value}";

            chartTimer.Tick += (s, f) =>
            {
                chartTimer.Stop();
                DrawOverallChart();
            };

            pensionTimer.Tick += (s, ef) =>
            {
                pensionTimer.Stop();
                ExpandStatePensionRows_Safe();
                DrawOverallChart();
            };


            chart_overall.Series[0].ToolTip = "#VALY"; // fallback
            chart_overall.Series[0].SmartLabelStyle.Enabled = true;
            chart_overall.Series[0].LabelForeColor = Color.Black;
            chart_overall.Series[0].LabelBackColor = Color.White;

        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            SaveInfo();
            Close();
        }

        private void btn_help_Click(object sender, EventArgs e)
        {
            help_form help = new help_form();
            help.Show();
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
                future_income = DatagridviewFunctions.ExtractGrid(dgv_uk_state_pension),
                future_expenses = DatagridviewFunctions.ExtractGrid(dgv_future_expenses),
                salary = txtbx_salary.Text,
                inflation = txtbx_inflation.Text,
                currency = cmbx_currency.Text,
                age = txtbx_age.Text,
                length = txtbx_length.Text,
                salary_inflation = chkbx_use_inflation.Checked.ToString()
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(finalPath, json);
            MsgBox.Show("Saved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult result = MsgBox.Show("Would you also like to save the Chart image?", "Question", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes) SaveChartImage(finalPath, true);
        }

        private void lbl_expenses_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_expenses))
            {
            }
        }

        private void lbl_assets_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_assets))
            {
            }
        }

        private void lbl_income_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_income))
            {
            }
        }

        private void lbl_future_expenses_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_future_expenses))
            {
            }
        }

        private void lbl_future_income_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_uk_state_pension))
            {
            }
        }

        private void lbl_cash_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_cash))
            {
            }
        }

        private void lbl_stocks_shares_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_stocks_shares))
            {
            }
        }

        private void lbl_bonds_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_bonds))
            {
            }
        }

        private void lbl_savings_add_Click(object sender, EventArgs e)
        {
            if (DataGridViewAdd(dgv_savings))
            {
            }
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
                MsgBox.Show("Exception: " + e, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void lbl_expenses_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_expenses))
            {
            }
        }

        private void lbl_assets_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_assets))
            {
            }
        }

        private void lbl_income_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_income))
            {
            }
        }

        private void lbl_future_expenses_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_future_expenses))
            {
            }
        }

        private void lbl_future_income_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_uk_state_pension))
            {
            }
        }

        private void lbl_cash_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_cash))
            {
            }
        }

        private void lbl_savings_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_savings))
            {
            }
        }

        private void lbl_stocks_shares_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_stocks_shares))
            {
            }
        }

        private void lbl_bonds_delete_Click(object sender, EventArgs e)
        {
            if (DataGridViewDelete(dgv_bonds))
            {
            }
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
                MsgBox.Show("Exception: " + e, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if ((dgv_expenses.Columns[e.ColumnIndex].Name == "Annually") && (flag))
            {
                lbl_expenses.Text = DatagridviewFunctions.CalculateTabTotal(dgv_expenses, e);
            }
        }

        private void dgv_income_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv_income.Columns[e.ColumnIndex].Name == "AnnualIncrease")
            {
                dgv_income.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Black;
                string raw = dgv_income.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";

                if (!TryParsePercentage(raw, out decimal percent))
                {
                    dgv_income.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Red;
                    dgv_income.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Enter a number or number followed by %";
                }
                else
                {
                    dgv_income.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Black;
                    dgv_income.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "";
                }
            }

            chartTimer.Stop();
            chartTimer.Start();
        }


        private void dgv_future_expenses_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((dgv_future_expenses.Columns[e.ColumnIndex].Name == "Amount") && (flag))
            {
                //lbl_future_expenses.Text = DatagridviewFunctions.CalculateTabTotal(dgv_future_expenses, e);

            }
        }

        private void dgv_future_income_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv_uk_state_pension.Columns[e.ColumnIndex].Name == "IsStatePension" ||
                dgv_uk_state_pension.Columns[e.ColumnIndex].Name == "Year" ||
                dgv_uk_state_pension.Columns[e.ColumnIndex].Name == "Amount")
            {
                pensionTimer.Stop();
                pensionTimer.Start();   // defer update safely
            }

            if (dgv_uk_state_pension.Columns[e.ColumnIndex].Name == "IsStatePension")
            {

                bool flag = true;

                if ((bool)dgv_uk_state_pension.Rows[e.RowIndex].Cells["IsStatePension"].Value)
                {
                    // Disable ticking on all other rows
                    foreach (DataGridViewRow r in dgv_uk_state_pension.Rows)
                    {
                        if (r.Index != e.RowIndex && !r.IsNewRow)
                        {
                            if (flag)
                                MsgBox.Show("You can only have 1 state pension\rThe tickboxes will now be readonly", "Information", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                            r.Cells["IsStatePension"].ReadOnly = true;

                            flag = false; //set to false so we only show messagebox once.
                        }
                    }
                }
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
            Recalculate();
        }

        private void Recalculate()
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
                                            CultureInfo.CreateSpecificCulture("en-GB").NumberFormat))
                .ToString("C", new CultureInfo("en-GB")));
            ;


            lbl_total_minus_expenses.Text = ((decimal.Parse(lbl_1st_pass_total.Text, NumberStyles.Currency,
                                                  CultureInfo.CreateSpecificCulture("en-GB").NumberFormat)
                                              - decimal.Parse(txtbx_salary.Text, NumberStyles.Currency,
                                                  CultureInfo.CreateSpecificCulture("en-GB").NumberFormat)
                                              - decimal.Parse(lbl_expenses.Text, NumberStyles.Currency,
                                                  CultureInfo.CreateSpecificCulture("en-GB").NumberFormat))
                .ToString("C", new CultureInfo("en-GB")));
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
            DatagridviewFunctions.LoadGrid(dgv_uk_state_pension, data.future_income);
            DatagridviewFunctions.LoadGrid(dgv_future_expenses, data.future_expenses);

            txtbx_salary.Text = data.salary ?? "";
            txtbx_inflation.Text = data.inflation ?? "";
            cmbx_currency.Text = data.currency ?? "£";
            txtbx_age.Text = data.age ?? "";
            txtbx_length.Text = data.length ?? "";

            bool.TryParse(data.salary_inflation, out bool value);
            chkbx_use_inflation.Checked = value;

            ApplyCurrencyFormattingToAllGrids();
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
                rchtxtbx_monte_carlo_output.AppendText(
                    $"Probability of not running out of money: {successProbability:P2}\r");

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
            lbl_savings.Text =
                DatagridviewFunctions.CalculateTabTotal(dgv_savings, new DataGridViewCellEventArgs(1, 0));
            lbl_bonds.Text = DatagridviewFunctions.CalculateTabTotal(dgv_bonds, new DataGridViewCellEventArgs(1, 0));
            lbl_stocks_shares.Text =
                DatagridviewFunctions.CalculateTabTotal(dgv_stocks_shares, new DataGridViewCellEventArgs(1, 0));

            lbl_income.Text = DatagridviewFunctions.CalculateTabTotal(dgv_income, new DataGridViewCellEventArgs(1, 0));
            lbl_expenses.Text =
                DatagridviewFunctions.CalculateTabTotal(dgv_expenses, new DataGridViewCellEventArgs(1, 0));

            //lbl_future_income.Text =
            //    DatagridviewFunctions.CalculateTabTotal(dgv_uk_state_pension, new DataGridViewCellEventArgs(2, 0));
            //lbl_future_expenses.Text =
            //    DatagridviewFunctions.CalculateTabTotal(dgv_future_expenses, new DataGridViewCellEventArgs(2, 0));
        }

        private void btn_new_Click(object sender, EventArgs e)
        {
            DatagridviewFunctions.NewSetUp(dgv_assets, dgv_cash, dgv_savings, dgv_bonds, dgv_stocks_shares, dgv_income,
                dgv_expenses,
                dgv_uk_state_pension, dgv_future_expenses);
        }


        private void Dgv_CellLeave_FormatCurrency(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = sender as DataGridView;
            var col = dgv.Columns[e.ColumnIndex];

            // Only format currency columns
            if (col.Tag?.ToString() != "currency")
                return;

            // Force commit of edit so cell.Value is up-to-date
            dgv.EndEdit();

            var cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            string raw = cell.Value?.ToString() ?? "";

            if (DatagridviewFunctions.TryParseMoney(raw, out decimal value))
            {
                cell.Style.ForeColor = Color.Black;

                string symbol = cmbx_currency.Text;

                cell.Value = symbol + value.ToString("N2");

                // Ensure CellValueChanged fires so totals update
                dgv.NotifyCurrentCellDirty(true);
                dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
            else
            {
                cell.Style.ForeColor = Color.Red;
            }
        }



        private void ApplyCurrencyFormattingToAllGrids()
        {
            string symbol = cmbx_currency.Text;

            Action<DataGridView> formatGrid = dgv =>
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.OwningColumn.Tag?.ToString() == "currency")
                            {
                                if (DatagridviewFunctions.TryParseMoney(cell.Value?.ToString() ?? "", out decimal val))
                                {
                                    cell.Style.ForeColor = Color.Black;
                                    cell.Value = $"{symbol}{val:N2}";
                                }
                                else
                                {
                                    cell.Style.ForeColor = Color.Red;
                                }
                            }
                        }
                    }
                }
            };

            formatGrid(dgv_cash);
            formatGrid(dgv_savings);
            formatGrid(dgv_bonds);
            formatGrid(dgv_stocks_shares);
            formatGrid(dgv_assets);
            formatGrid(dgv_income);
            formatGrid(dgv_expenses);
            formatGrid(dgv_uk_state_pension);
            formatGrid(dgv_future_expenses);
        }

        private void thinsldr_salary_ValueChanged(object sender, EventArgs e)
        {
            chartTimer.Stop();
            txtbx_salary.Text = cmbx_currency.Text + thinsldr_salary.Value; //ToString("N2");
            chartTimer.Start();
        }

        private void txtbx_salary_TextChanged(object sender, EventArgs e)
        {
            chartTimer.Stop();

            string symbol = cmbx_currency.Text;
            string text = txtbx_salary.Text.Trim();

            // If empty → reset to symbol + 0
            if (text == "")
            {
                txtbx_salary.Text = symbol + "0";
                return;
            }

            // Ensure salary always starts with currency symbol
            if (!text.StartsWith(symbol))
            {
                text = symbol + text;
                txtbx_salary.Text = text;
            }

            // Extract numeric part
            string numeric = text.Substring(symbol.Length);

            // If not numeric → reset
            if (!int.TryParse(numeric, out int value))
            {
                txtbx_salary.Text = symbol + "0";
                thinsldr_salary.Value = thinsldr_salary.Minimum;
                chartTimer.Start();
                return;
            }

            // Clamp to slider range
            value = Math.Max(thinsldr_salary.Minimum, Math.Min(thinsldr_salary.Maximum, value));

            thinsldr_salary.Value = value;

            // Recalculate totals
            Recalculate();

            chartTimer.Start();
        }

        private void thinsldr_Age_ValueChanged(object sender, EventArgs e)
        {
            chartTimer.Stop();
            txtbx_age.Text = thinsldr_Age.Value.ToString();
            chartTimer.Start();
        }

        private void txtbx_age_TextChanged(object sender, EventArgs e)
        {
            chartTimer.Stop();

            string text = txtbx_age.Text.Trim();

            if (text == "")
                return;

            if (!int.TryParse(text, out int value))
            {
                txtbx_age.Text = thinsldr_Age.Minimum.ToString();
                return;
            }

            value = Math.Max(thinsldr_Age.Minimum, Math.Min(thinsldr_Age.Maximum, value));

            thinsldr_Age.Value = value;

            chartTimer.Start();
        }


        private void thinsldr_Length_ValueChanged(object sender, EventArgs e)
        {
            chartTimer.Stop();
            txtbx_length.Text = thinsldr_Length.Value.ToString();
            chartTimer.Start();
        }

        private void txtbx_length_TextChanged(object sender, EventArgs e)
        {
            chartTimer.Stop();

            string text = txtbx_length.Text.Trim();

            // If empty → do nothing yet (user may still be typing)
            if (text == "")
                return;

            // If not numeric → reset
            if (!int.TryParse(text, out int value))
            {
                txtbx_length.Text = thinsldr_Length.Minimum.ToString();
                return;
            }

            // Clamp to slider range
            value = Math.Max(thinsldr_Length.Minimum, Math.Min(thinsldr_Length.Maximum, value));

            thinsldr_Length.Value = value;

            chartTimer.Start();
        }

        private void txtbx_inflation_TextChanged(object sender, EventArgs e)
        {
            chartTimer.Stop();

            string text = txtbx_inflation.Text.Trim();

            if (text == "")
                return;

            if (!decimal.TryParse(text, out decimal value))
            {
                txtbx_inflation.Text = "0";
                return;
            }

            // Optional: clamp inflation between 0% and 100%
            value = Math.Max(0, Math.Min(100, value));

            txtbx_inflation.Text = value.ToString();

            chartTimer.Start();
        }

        private void btn_draw_overall_chart_Click(object sender, EventArgs e)
        {
            DrawOverallChart();
        }

        private void DrawOverallChart()
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(txtbx_age.Text) ||
                string.IsNullOrWhiteSpace(txtbx_length.Text))
                return;

            // Parse inputs safely
            int age = int.Parse(txtbx_age.Text);
            int length = int.Parse(txtbx_length.Text);

            decimal salary = Parse(txtbx_salary.Text);
            decimal expenses = Parse(lbl_expenses.Text);

            decimal inflation = 0;
            decimal.TryParse(txtbx_inflation.Text, out inflation);
            inflation /= 100m;

            // Starting funds
            decimal available =
                Parse(lbl_assets.Text) +
                Parse(lbl_income.Text) +
                Parse(lbl_cash.Text) +
                Parse(lbl_savings.Text) +
                Parse(lbl_bonds.Text) +
                Parse(lbl_stocks_shares.Text);

            // Prepare chart
            chart_overall.Series.Clear();
            var series = chart_overall.Series.Add("Available Funds");

            dgv_tax_summary.Rows.Clear();

            // Add a new series called Trend.
            var line = chart_overall.Series.Add("Trend");
            line.ChartType = SeriesChartType.Line;
            line.Color = Color.Blue;
            line.BorderWidth = 2;

            int startYear = DateTime.Now.Year;

            decimal baseSalary = Parse(txtbx_salary.Text);
            decimal salaryThisYear = baseSalary;

            for (int i = 0; i < length; i++)
            {
                int currentYear = startYear + i;
                int currentAge = age + i;


                decimal UKStatePension = SumRows(dgv_uk_state_pension, currentYear);
                decimal futureExpenses = SumRows(dgv_future_expenses, currentYear);
                decimal recurringIncome = SumRecurringIncome(currentYear);

                // Taxable income (excluding salary)
                decimal taxableIncome = GetTaxableIncomeForYear(currentYear);

                //Calculate for particular UK country
                TaxBreakdown tb = UseScottishTaxBands
                    ? CalculateScottishTaxBreakdown(taxableIncome)
                    : CalculateEnglandTaxBreakdown(taxableIncome);

                // Income tax as an expense
                decimal incomeTax = tb.TotalTax; 

                decimal endOfYear =
                    available +
                    UKStatePension +
                    recurringIncome -
                    futureExpenses -
                    salaryThisYear -
                    expenses -
                    incomeTax;

                int index = series.Points.AddXY(currentAge, endOfYear);
                DataPoint point = series.Points[index];

                // Tooltip
                string tip =
                    $"Year: {currentYear}\n" +
                    $"Age: {currentAge}\n" +
                    $"UK State Pension: {UKStatePension:C}\n" +
                    $"Expenses: {expenses:C}\n" +
                    $"Income: {recurringIncome:C}\n" +
                    $"Future Expenses: {futureExpenses:C}\n" +
                    $"\nTaxable Income: {taxableIncome:C}\n" +
                    $"Income Tax: {incomeTax:C}\n" +
                    $"\nRemaining: {endOfYear:C}\n" +
                    $"\nSalary: {salaryThisYear:C}\n" +
                    $"Net Change: {(endOfYear - available):C}\n" +

                    $"\nTax Breakdown:" +
                    $"\n  Personal Allowance Used: {tb.PersonalAllowanceUsed:C}" +
                    $"\n  Basic Rate: {tb.TaxBasic:C}" +
                    $"\n  Higher Rate: {tb.TaxHigher:C}" +
                    $"\n  Additional Rate: {tb.TaxAdditional:C}" +
                    $"\n  Total Tax: {tb.TotalTax:C}\n";


                if (endOfYear < 0)
                    tip += "\n⚠ Funds exhausted";

                point.ToolTip = tip;

                // Colour coding
                if (endOfYear < 0)
                    point.Color = Color.Red;
                else if (endOfYear < available)
                    point.Color = Color.Orange;
                else
                    point.Color = Color.Green;

                // Prepare next year
                available = endOfYear;

                // Expenses inflation
                expenses += expenses * inflation;

                // Salary inflation (NEW)
                if (chkbx_use_inflation.Checked)
                    salaryThisYear += salaryThisYear * inflation;

                try
                {
                    dgv_tax_summary.Rows.Add(
                    currentYear,
                    tb.TaxableIncome.ToString("C"),
                    tb.PersonalAllowanceUsed.ToString("C"),
                    tb.BasicRateUsed.ToString("C"),
                    tb.HigherRateUsed.ToString("C"),
                    tb.AdditionalRateUsed.ToString("C"),
                    tb.TaxBasic.ToString("C"),
                    tb.TaxHigher.ToString("C"),
                    tb.TaxAdditional.ToString("C"),
                    tb.TotalTax.ToString("C"),
                    (tb.TotalTax / tb.TaxableIncome * 100m).ToString("N2")
                );
                }
                catch 
                {
                    //do nothing to eliminate potential div by zero error
                }

            }

            // Axis labels
            chart_overall.ChartAreas[0].AxisX.Title = "Age";
            chart_overall.ChartAreas[0].AxisY.Title = "Available Funds";
        }

        private decimal Parse(string s)
        {
            if (DatagridviewFunctions.TryParseMoney(s, out decimal v))
                return v;
            return 0;
        }

        private decimal SumRows(DataGridView dgv, int year)
        {
            decimal total = 0;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                // Find the correct columns by name
                var yearCell = row.Cells["Year"];
                var amountCell = row.Cells["Amount"];

                if (yearCell == null || amountCell == null)
                    continue;

                // Parse the year
                if (!int.TryParse(yearCell.Value?.ToString(), out int rowYear))
                    continue;

                if (rowYear == year)
                {
                    string raw = amountCell.Value?.ToString() ?? "";
                    if (DatagridviewFunctions.TryParseMoney(raw, out decimal val))
                        total += val;
                }
            }

            return total;
        }

        private void ExpandStatePensionRows_Safe()
        {
            bool anyStatePensionTicked = false;
            DataGridViewRow pensionRow = null;

            // Find the state pension row
            foreach (DataGridViewRow row in dgv_uk_state_pension.Rows)
            {
                if (row.IsNewRow) continue;

                var chkCell = row.Cells["IsStatePension"];
                if (chkCell != null && chkCell.Value is bool b && b)
                {
                    anyStatePensionTicked = true;
                    pensionRow = row;
                    break;
                }
            }

            // If none ticked → remove auto rows and re-enable checkboxes
            if (!anyStatePensionTicked)
            {
                RemoveAutoGeneratedStatePensionRows();

                // Re-enable all checkboxes
                foreach (DataGridViewRow row in dgv_uk_state_pension.Rows)
                {
                    if (!row.IsNewRow)
                        row.Cells["IsStatePension"].ReadOnly = false;
                }

                DrawOverallChart();
                return;
            }

            // If ticked → disable other checkboxes
            foreach (DataGridViewRow row in dgv_uk_state_pension.Rows)
            {
                if (!row.IsNewRow && row != pensionRow)
                    row.Cells["IsStatePension"].ReadOnly = true;
            }

            // Validate Year + Amount
            var yearCell = pensionRow.Cells["Year"];
            var amountCell = pensionRow.Cells["Amount"];

            if (yearCell == null || yearCell.Value == null) return;
            if (amountCell == null || amountCell.Value == null) return;

            if (!int.TryParse(yearCell.Value.ToString(), out int startYear)) return;
            if (!DatagridviewFunctions.TryParseMoney(amountCell.Value.ToString(), out decimal amount)) return;

            // Remove old auto rows
            RemoveAutoGeneratedStatePensionRows();

            // Generate new auto rows
            decimal current = amount;
            int currentYear = startYear + 1;

            //Start writing for the next year
            for (int i = 1; i < 50; i++)
            {
                //if (i > 0)
                current *= 1.01875m; // April uplift

                dgv_uk_state_pension.Rows.Add(
                    "State Pension (auto)",
                    currentYear,
                    cmbx_currency.Text + current.ToString("N2"),
                    false
                );

                currentYear++;
            }

            DrawOverallChart();
        }

        private void RemoveAutoGeneratedStatePensionRows()
        {
            for (int i = dgv_uk_state_pension.Rows.Count - 1; i >= 0; i--)
            {
                var row = dgv_uk_state_pension.Rows[i];
                if (row.IsNewRow) continue;

                if (row.Cells[0].Value?.ToString() == "State Pension (auto)")
                    dgv_uk_state_pension.Rows.RemoveAt(i);
            }
        }

        private void dgv_future_income_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv_uk_state_pension.Columns[e.ColumnIndex].Name == "IsStatePension")
            {
                dgv_uk_state_pension.EndEdit();   // commit checkbox change immediately

                pensionTimer.Stop();
                pensionTimer.Start();          // trigger pension expansion safely
            }
        }

        private decimal SumRecurringIncome(int year)
        {
            decimal total = 0;

            foreach (DataGridViewRow row in dgv_income.Rows)
            {
                if (row.IsNewRow) continue;

                // Extract values safely
                string name = row.Cells["Name"].Value?.ToString() ?? "";

                // Lifetime income?
                bool lifetime = false;
                var chkCell = row.Cells["Lifetime"];
                if (chkCell != null && chkCell.Value is bool b && b)
                    lifetime = true;

                // StartYear
                int startYear = 0;
                if (!int.TryParse(row.Cells["StartYear"].Value?.ToString(), out startYear))
                    startYear = 0; // treat 0 as "starts immediately"

                // EndYear
                int endYear = 0;
                if (!int.TryParse(row.Cells["EndYear"].Value?.ToString(), out endYear))
                    endYear = 0;

                if (lifetime)
                {
                    // Lifetime income: end year = retirement end year
                    endYear = DateTime.Now.Year + int.Parse(txtbx_length.Text);
                }
                else
                {
                    if (endYear == 0)
                        endYear = int.MaxValue; // continues forever
                }

                if (startYear == 0)
                    startYear = DateTime.Now.Year; // starts immediately

                // Amount
                if (!DatagridviewFunctions.TryParseMoney(row.Cells["Annually"].Value?.ToString(), out decimal amount))
                    continue;

                // Annual increase
                // Check this is valid
                decimal increasePercent = 0;
                string incRaw = row.Cells["AnnualIncrease"].Value?.ToString() ?? "";

                row.Cells["AnnualIncrease"].ToolTipText = "Please enter a number or number followed by %";

                if (!TryParsePercentage(incRaw, out increasePercent))
                {
                    // Mark cell red to warn user
                    row.Cells["AnnualIncrease"].Style.ForeColor = Color.Red;

                    // No increase applied
                    increasePercent = 0;
                }
                else
                {
                    row.Cells["AnnualIncrease"].Style.ForeColor = Color.Black;
                    // row.Cells["AnnualIncrease"].ToolTipText = "";

                }

                decimal increase = increasePercent / 100m;


                // Check if this income applies to the current year
                if (year < startYear || year > endYear)
                    continue;

                // Apply annual increase
                int yearsSinceStart = year - startYear;
                decimal adjustedAmount = amount * (decimal)Math.Pow((double)(1 + increase), yearsSinceStart);

                total += adjustedAmount;
            }

            return total;
        }

        private bool TryParsePercentage(string input, out decimal value)
        {
            value = 0;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = input.Trim();

            // Remove trailing % if present
            if (input.EndsWith("%"))
                input = input.Substring(0, input.Length - 1).Trim();

            // Now input must be numeric
            if (!decimal.TryParse(input, out decimal parsed))
                return false;

            value = parsed;
            return true;
        }

        private void chkbx_use_inflation_CheckedChanged(object sender, EventArgs e)
        {
            chartTimer.Stop();
            chartTimer.Start();
        }

        private void btn_save_chart_Click(object sender, EventArgs e)
        {
            SaveChartImage("myChart.jpg", false);
        }

        private void SaveChartImage(string myPath, bool flag)
        {
            try

            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                    sfd.Title = "Save TableLayoutPanel as Image";
                    sfd.FileName = "Image.png";

                    // flag means call it the same as the json file name
                    if (flag)
                        sfd.FileName = GetUntilOrEmpty(myPath, "_") + DateTime.Now.ToString("_ddMMMyyyy_HHmmss");

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        SaveControlAsImage(tbl_lyot_pnl_overall, sfd.FileName);

                        MessageBox.Show("Image saved successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// Captures a control (including TableLayoutPanel) as an image and saves it.
        /// </summary>
        private void SaveControlAsImage(Control control, string filePath)
        {
            if (control.Width <= 0 || control.Height <= 0)
                throw new InvalidOperationException("Control has invalid dimensions.");

            using (Bitmap bmp = new Bitmap(control.Width, control.Height))
            {
                control.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

                ImageFormat format = ImageFormat.Png;
                string ext = Path.GetExtension(filePath)?.ToLower();
                if (ext == ".jpg" || ext == ".jpeg") format = ImageFormat.Jpeg;
                else if (ext == ".bmp") format = ImageFormat.Bmp;

                bmp.Save(filePath, format);
            }
        }

        public static string GetUntilOrEmpty(string text, string stopAt)
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }

        private decimal GetTaxableIncomeForYear(int year)
        {
            decimal total = 0;

            // 1. Taxable recurring income from dgv_income
            foreach (DataGridViewRow row in dgv_income.Rows)
            {
                if (row.IsNewRow) continue;

                bool taxable = false;
                var taxCell = row.Cells["IsTaxable"];
                if (taxCell != null && taxCell.Value is bool b && b)
                    taxable = true;

                if (!taxable) continue;

                // Use your existing recurring income logic
                // but only for this row and this year
                if (!int.TryParse(row.Cells["StartYear"].Value?.ToString(), out int startYear))
                    continue;
                if (!int.TryParse(row.Cells["EndYear"].Value?.ToString(), out int endYear))
                    endYear = int.MaxValue;

                if (year < startYear || year > endYear)
                    continue;

                if (!DatagridviewFunctions.TryParseMoney(row.Cells["Annually"].Value?.ToString(), out decimal amount))
                    continue;

                decimal increasePercent = 0;
                string incRaw = row.Cells["AnnualIncrease"].Value?.ToString() ?? "";
                TryParsePercentage(incRaw, out increasePercent);
                decimal increase = increasePercent / 100m;

                int yearsSinceStart = year - startYear;
                decimal adjustedAmount = amount * (decimal)Math.Pow((double)(1 + increase), yearsSinceStart);

                total += adjustedAmount;
            }

            // 2. UK State Pension (from dgv_uk_state_pension)
            total += SumRows(dgv_uk_state_pension, year); // you already have SumRows(year)

            return total;
        }

        
        private TaxBreakdown CalculateEnglandTaxBreakdown(decimal taxableIncome)
        {
            const decimal personalAllowance = 12570m;
            const decimal basicRateLimit = 50270m;
            const decimal higherRateLimit = 125140m;

            var result = new TaxBreakdown();
            result.TaxableIncome = taxableIncome;

            // Personal allowance taper
            decimal allowance = personalAllowance;
            if (taxableIncome > 100000m)
            {
                decimal reduction = (taxableIncome - 100000m) / 2m;
                allowance = Math.Max(0, personalAllowance - reduction);
            }

            result.PersonalAllowanceUsed = Math.Min(taxableIncome, allowance);

            decimal remaining = Math.Max(0, taxableIncome - allowance);

            // Basic rate
            decimal basicBand = Math.Min(remaining, basicRateLimit - allowance);
            result.BasicRateUsed = basicBand;
            result.TaxBasic = basicBand * 0.20m;
            remaining -= basicBand;

            // Higher rate
            decimal higherBand = Math.Min(remaining, higherRateLimit - basicRateLimit);
            result.HigherRateUsed = higherBand;
            result.TaxHigher = higherBand * 0.40m;
            remaining -= higherBand;

            // Additional rate
            result.AdditionalRateUsed = remaining;
            result.TaxAdditional = remaining * 0.45m;

            result.TotalTax = result.TaxBasic + result.TaxHigher + result.TaxAdditional;

            return result;
        }

        private TaxBreakdown CalculateScottishTaxBreakdown(decimal taxableIncome)
        {
            const decimal personalAllowance = 12570m;

            var result = new TaxBreakdown();
            result.TaxableIncome = taxableIncome;

            // Personal allowance taper (same as England)
            decimal allowance = personalAllowance;
            if (taxableIncome > 100000m)
            {
                decimal reduction = (taxableIncome - 100000m) / 2m;
                allowance = Math.Max(0, personalAllowance - reduction);
            }

            result.PersonalAllowanceUsed = Math.Min(taxableIncome, allowance);

            decimal remaining = Math.Max(0, taxableIncome - allowance);

            // Starter rate (19%)
            decimal starterBand = Math.Min(remaining, 14732m - 12570m);
            result.BasicRateUsed = starterBand; // reuse field
            result.TaxBasic = starterBand * 0.19m;
            remaining -= starterBand;

            // Basic rate (20%)
            decimal basicBand = Math.Min(remaining, 25688m - 14732m);
            result.HigherRateUsed = basicBand; // reuse field
            result.TaxHigher = basicBand * 0.20m;
            remaining -= basicBand;

            // Intermediate rate (21%)
            decimal intermediateBand = Math.Min(remaining, 43662m - 25688m);
            result.AdditionalRateUsed = intermediateBand; // reuse field
            result.TaxAdditional = intermediateBand * 0.21m;
            remaining -= intermediateBand;

            // Higher rate (42%)
            decimal higherBand = Math.Min(remaining, 75000m - 43662m);
            result.TaxHigher += higherBand * 0.42m;
            remaining -= higherBand;

            // Top rate (47%)
            result.TaxAdditional += remaining * 0.47m;

            result.TotalTax = result.TaxBasic + result.TaxHigher + result.TaxAdditional;

            return result;
        }


        private void rdobtn_eng_wal_ni_CheckedChanged(object sender, EventArgs e)
        {
            if (rdobtn_eng_wal_ni.Checked)
            {
                chartTimer.Stop();
                chartTimer.Start();   // triggers full redraw
            }
        }

        private void rdobtn_scotland_CheckedChanged(object sender, EventArgs e)
        {
            if (rdobtn_scotland.Checked)
            {
                chartTimer.Stop();
                chartTimer.Start();   // triggers full redraw
            }
        }
    }
}
