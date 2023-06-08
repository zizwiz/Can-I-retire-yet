using System;
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
            Decimal sum = 0;

            for (int i = 0; i < dgv.Rows.Count; ++i)
            {
                sum += decimal.Parse(dgv.Rows[i].Cells[e.ColumnIndex].Value.ToString(), NumberStyles.Currency, CultureInfo.CreateSpecificCulture("en-GB").NumberFormat);
            }

          
            return string.Format(new CultureInfo("en-GB"), "{0:C}", sum);
        }




        public  static void OpenFile(DataGridView dgv)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = Assembly.GetEntryAssembly().Location,
                Title = "Browse CSV Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "csv",
                Filter = "csv files (*.csv)|*.csv|" + "All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (dgv.RowCount > 0) dgv.Rows.Clear(); //clear grid

                // Replace | with , especially in currency.
                //string text = File.ReadAllText(openFileDialog1.FileName);
                //text = text.Replace('|', ',');
                //File.WriteAllText(openFileDialog1.FileName, text);

                //Write the data to the Grid
                //File.ReadLines(openFileDialog1.FileName).Skip(1)
                //    .Select(x => x.Split(','))
                //    .ToList()
                //    .ForEach(line => dgv.Rows.Add(line));

                Form1.flag = false;

                foreach (var srLine in File.ReadAllLines(openFileDialog1.FileName).Skip(1))
                {
                    dgv.Rows.Add(srLine.Split(','));
                }

                

                for (int i = 0; i < dgv.RowCount; i++)
                {
                    for (int j = 0; j < dgv.ColumnCount; j++)
                    {
                        dgv[j, i].Value = dgv[j, i].Value.ToString().Replace('|', ',');
                    }

                }


                //Form1 frm = new Form1();
                //DataGridViewCellEventArgs e = new DataGridViewCellEventArgs(0, 0);

                //frm.lbl_assets.Text = CalculateTabTotal(frm.dgv_assets, e);

                Form1.flag = true;
            }
        }

        public static void SaveFile(DataGridView dgv)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Assembly.GetEntryAssembly().Location;
            saveFileDialog1.Title = "Save csv Files";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "csv";
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                int columnCount = dgv.ColumnCount;
                string columnNames = "";
                string[] output = new string[dgv.RowCount + 1];

                for (int i = 0; i < columnCount; i++)
                {
                    columnNames += dgv.Columns[i].Name + ",";
                }
                output[0] += columnNames;
                for (int i = 1; (i - 1) < dgv.RowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        dgv.CurrentCell = dgv[0, 0]; //Move cursor back to cell[0,0] incase it is resting in an updating cell.
                        output[i] += dgv.Rows[i - 1].Cells[j].Value.ToString().Replace(',', '|') + ",";
                        //output[i] += dgv.Rows[i - 1].Cells[j].Value + ",";
                    }
                }
                File.WriteAllLines(saveFileDialog1.FileName, output, Encoding.UTF8);
            }
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

            dgv.AllowUserToAddRows = false; //remove last empty row
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; //fit columns to grid view 
            dgv.RowHeadersVisible = false; //remove left hand column

            //dgv.Columns[1].DefaultCellStyle.Format = "c"; //set up column 1 for currency

            //dgv.Columns[1].DefaultCellStyle.Format = "c2";
            //dgv.Columns[1].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("en-GB");

            //dgv.Columns[1].DefaultCellStyle.Format = "£0.00  ";
            dgv.Columns[1].DefaultCellStyle.Format = "C";
        }
    }
}
