using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using CenteredMessagebox;

namespace Can_I_retire_yet.functions
{
    class DatagridviewFunctions
    {

        public static void AddRow(DataGridView dgv)
        {
            dgv.Rows.Add();
            dgv.AllowUserToAddRows = false;

        }

        public static void DeleteRow(DataGridView dgv)
        {
            if (dgv.RowCount > 0)
            {
                //find cursor and then highlight the row it is in.
                dgv.Rows[dgv.CurrentCell.RowIndex].Selected = true;

                //delete highlighted row
                foreach (DataGridViewRow dgvr in dgv.Rows)
                {
                    if (dgvr.Selected)
                    {
                        dgv.Rows.Remove(dgvr);
                    }
                }
            }
            else
            {
                MsgBox.Show("No rows to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static string CalculateTabTotal(DataGridView dgv, DataGridViewCellEventArgs e)
        {
            decimal total = 0;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!row.IsNewRow)
                {
                    var cellValue = row.Cells[e.ColumnIndex].Value?.ToString() ?? "";

                    if (TryParseMoney(cellValue, out decimal amount))
                        total += amount;
                }
            }

            return total.ToString("C", CultureInfo.GetCultureInfo("en-GB"));
        }

        public static void SetUpViews(DataGridView dgv, int NumCols)
        {
            dgv.ColumnCount = NumCols;

            if (dgv.Name == "dgv_expenses")
            {
                dgv.Columns[0].Name = "Name";
                dgv.Columns[1].Name = "Monthly";
            }
            else if (dgv.Name == "dgv_assets")
            {
                dgv.Columns[0].Name = "Name";
                dgv.Columns[1].Name = "Amount";
            }
            else if (dgv.Name == "dgv_income")
            {
                dgv.Columns[0].Name = "Name";
                dgv.Columns[1].Name = "Monthly";
            }
            else if (dgv.Name == "dgv_future_income")
            {
                dgv.Columns[0].Name = "Year";
                dgv.Columns[1].Name = "Name";
                dgv.Columns[2].Name = "Amount";
            }
            else if (dgv.Name == "dgv_future_expenses")
            {
                dgv.Columns[0].Name = "Year";
                dgv.Columns[1].Name = "Name";
                dgv.Columns[2].Name = "Amount";
            }
            else if (dgv.Name == "dgv_cash")
            {
                dgv.Columns[0].Name = "Institution";
                dgv.Columns[1].Name = "Amount";
            }
            else if (dgv.Name == "dgv_savings")
            {
                dgv.Columns[0].Name = "Institution";
                dgv.Columns[1].Name = "Amount";
                dgv.Columns[2].Name = "Interest";
                dgv.Columns[3].Name = "Taxable";
            }
            else if (dgv.Name == "dgv_bonds")
            {
                dgv.Columns[0].Name = "Institution";
                dgv.Columns[1].Name = "Amount";
            }
            else if (dgv.Name == "dgv_stocks_shares")
            {
                dgv.Columns[0].Name = "Institution";
                dgv.Columns[1].Name = "Amount";
            }

            dgv.AllowUserToAddRows = false; //remove last empty row
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; //fit columns to grid view 
            dgv.RowHeadersVisible = false; //remove left hand column

            //dgv.Columns[1].DefaultCellStyle.Format = "c"; //set up column 1 for currency

            //dgv.Columns[1].DefaultCellStyle.Format = "c2";
            //dgv.Columns[1].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("en-GB");

            //dgv.Columns[1].DefaultCellStyle.Format = "£0.00  ";
            dgv.Columns[1].DefaultCellStyle.Format = "C";
        }

        public static List<List<string>> ExtractGrid(DataGridView dgv)
        {
            var list = new List<List<string>>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!row.IsNewRow)
                {
                    var rowList = new List<string>();
                    foreach (DataGridViewCell cell in row.Cells)
                        rowList.Add(cell.Value?.ToString() ?? "");
                    list.Add(rowList);
                }
            }

            return list;
        }

        public static void LoadGrid(DataGridView dgv, List<List<string>> data)
        {
            dgv.Rows.Clear();

            foreach (var row in data)
                dgv.Rows.Add(row.ToArray());
        }

        public static bool TryParseMoney(string input, out decimal value)
        {
            value = 0;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            string cleaned = input.Trim();

            // Remove currency symbols
            cleaned = cleaned.Replace("£", "")
                .Replace("$", "")
                .Replace("€", "");

            // Remove spaces
            cleaned = cleaned.Replace(" ", "");

            // Replace commas with nothing
            cleaned = cleaned.Replace(",", "");

            // Handle shorthand like "100k" or "£250k"
            if (cleaned.EndsWith("k", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(0, cleaned.Length - 1);
                if (decimal.TryParse(cleaned, out decimal kVal))
                {
                    value = kVal * 1000;
                    return true;
                }
            }

            // Try standard decimal parsing
            if (decimal.TryParse(cleaned,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out decimal parsed))
            {
                value = parsed;
                return true;
            }

            // Try UK culture (handles commas, decimals, currency)
            if (decimal.TryParse(cleaned,
                    NumberStyles.Any,
                    CultureInfo.GetCultureInfo("en-GB"),
                    out parsed))
            {
                value = parsed;
                return true;
            }

            return false;
        }

        public static void NewSetUp(DataGridView dgv_assets, DataGridView dgv_cash, DataGridView dgv_savings,
            DataGridView dgv_bonds, DataGridView dgv_stocks_shares, DataGridView dgv_income,
            DataGridView dgv_expenses, DataGridView dgv_future_income, DataGridView dgv_future_expenses)
        {
            //Clear the existing DataGridViews
            ResetDataGridView(dgv_assets);
            ResetDataGridView(dgv_cash);
            ResetDataGridView(dgv_savings);
            ResetDataGridView(dgv_bonds);
            ResetDataGridView(dgv_stocks_shares);
            ResetDataGridView(dgv_income);
            ResetDataGridView(dgv_expenses);
            ResetDataGridView(dgv_future_income);
            ResetDataGridView(dgv_future_expenses);

            //Add Aide Memoirs to existing DataGridViews
            dgv_expenses.Rows.Add("Rent", "");
            dgv_expenses.Rows.Add("Property service charge", "");
            dgv_expenses.Rows.Add("House maintenance", "");
            dgv_expenses.Rows.Add("Food", "");
            dgv_expenses.Rows.Add("Clothing", "");
            dgv_expenses.Rows.Add("Hair, Dental & Optical", "");
            dgv_expenses.Rows.Add("Tax", "");
            dgv_expenses.Rows.Add("Other day to day essential expenses", "");
            dgv_expenses.Rows.Add("Telephone", "");
            dgv_expenses.Rows.Add("Mobile phone", "");
            dgv_expenses.Rows.Add("Internet/Broadband subscription", "");
            dgv_expenses.Rows.Add("TV licence", "");
            dgv_expenses.Rows.Add("Gas", "");
            dgv_expenses.Rows.Add("Water", "");
            dgv_expenses.Rows.Add("Electricity", "");
            dgv_expenses.Rows.Add("Council tax", "");
            dgv_expenses.Rows.Add("Travel card", "");
            dgv_expenses.Rows.Add("Parking permit", "");
            dgv_expenses.Rows.Add("Fuel", "");
            dgv_expenses.Rows.Add("Vehicle insurance", "");
            dgv_expenses.Rows.Add("Road tax", "");
            dgv_expenses.Rows.Add("Vehicle maintenance/MOT", "");
            dgv_expenses.Rows.Add("Bicycle", "");
            dgv_expenses.Rows.Add("Other travel expenses", "");
            dgv_expenses.Rows.Add("Alcohol", "");
            dgv_expenses.Rows.Add("Tobacco", "");
            dgv_expenses.Rows.Add("Lunch", "");
            dgv_expenses.Rows.Add("Beauty", "");
            dgv_expenses.Rows.Add("Christmas", "");
            dgv_expenses.Rows.Add("Birthdays", "");
            dgv_expenses.Rows.Add("Newspapers & Subscriptions", "");
            dgv_expenses.Rows.Add("Other day to day expenses", "");
            dgv_expenses.Rows.Add("Recreation and entertainment", "");
            dgv_expenses.Rows.Add("Holiday and travel", "");
            dgv_expenses.Rows.Add("Memberships (Gym/Sports/Museum)", "");
            dgv_expenses.Rows.Add("Other leisure expenses", "");
            dgv_expenses.Rows.Add("Child care", "");
            dgv_expenses.Rows.Add("Child maintenance", "");
            dgv_expenses.Rows.Add("Education or school fees", "");
            dgv_expenses.Rows.Add("Other child expenses", "");
            dgv_expenses.Rows.Add("Mortgage", "");
            dgv_expenses.Rows.Add("Loans or hire purchase payments", "");
            dgv_expenses.Rows.Add("Credit card or store cards payments", "");
            dgv_expenses.Rows.Add("Vehicle payments", "");
            dgv_expenses.Rows.Add("Pension premium", "");
            dgv_expenses.Rows.Add("ISA premium", "");
            dgv_expenses.Rows.Add("Premium Bonds", "");
            dgv_expenses.Rows.Add("Lottery", "");
            dgv_expenses.Rows.Add("Gambling", "");
            dgv_expenses.Rows.Add("Children's savings premium", "");
            dgv_expenses.Rows.Add("Other regular savings premium", "");
            dgv_expenses.Rows.Add("Home and contents insurance", "");
            dgv_expenses.Rows.Add("Life assurance", "");
            dgv_expenses.Rows.Add("Critical illness cover", "");
            dgv_expenses.Rows.Add("Life assurance and critical illness cover", "");
            dgv_expenses.Rows.Add("Medical insurance", "");
            dgv_expenses.Rows.Add("Income protection", "");
            dgv_expenses.Rows.Add("Accident, sickness and unemployment", "");
            dgv_expenses.Rows.Add("Other regular insurance", "");
            dgv_expenses.Rows.Add("Cash spending", "");
            dgv_expenses.Rows.Add("Charitable Giving", "");
            dgv_expenses.Rows.Add("Miscellaneous Spending", "");

            dgv_income.Rows.Add("Your take home pay", "");
            dgv_income.Rows.Add("Your net bonus", "");
            dgv_income.Rows.Add("Your other income", "");
            dgv_income.Rows.Add("Your partner's take home pay", "");
            dgv_income.Rows.Add("Your partner's net bonus", "");
            dgv_income.Rows.Add("Your partner's other income", "");
        }

        private static void ResetDataGridView(DataGridView dgv)
        {
            dgv.DataSource = null;
            dgv.Rows.Clear();
            dgv.Refresh();
        }

    }
}
