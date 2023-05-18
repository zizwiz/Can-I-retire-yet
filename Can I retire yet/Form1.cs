using System;
using System.Windows.Forms;
using System.Reflection;
using Can_I_retire_yet.functions;


namespace Can_I_retire_yet
{
    public partial class Form1 : Form
    {
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
    }
}
