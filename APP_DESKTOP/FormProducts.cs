using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APP_DESKTOP
{
    public partial class FormProducts : Form
    {
        WebServReference.WebService1SoapClient client =
                new WebServReference.WebService1SoapClient();

        public FormProducts()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {           
            string TableName = "Products";
            string Conditions = string.Empty;
            DataSet ValuesOfDataGridView = new DataSet();

            try
            {
                string query = "select * from Products";

                #region Code for applier filter
                if (this.pnTopTab.Controls.Count > 0)
                {
                    foreach (Control item in this.pnTopTab.Controls)
                    {
                        if (item is TextBox)
                        {
                            if (!string.IsNullOrEmpty(item.Text))
                            {
                                Conditions += item.Name.Replace("ct", "") + " like '%" + item.Text + "%' and ";
                            }
                        }

                        if (item is DateTimePicker)
                        {
                            if (((DateTimePicker)item).Value != null)
                            {
                                Conditions += item.Name.Replace("dtp", "") + " like '%" + 
                                    ((DateTimePicker)item).Value.ToString() + "%' and ";
                            }
                        }

                        if (item is CheckBox)
                        {
                            if (((CheckBox)item).Checked)
                            {
                                Conditions += item.Name.Replace("chx", "") + " like '%" +
                                    1 + "%' and ";
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(Conditions))
                    {
                        Conditions = Conditions.Substring(0, Conditions.LastIndexOf("and")) ?? string.Empty;
                        //query = string.IsNullOrEmpty(Conditions) == true ? query : query + " where " + Conditions;
                        query += " where " + Conditions;                        
                    }
                }
                #endregion

                ValuesOfDataGridView = client.GetData(query, TableName);
                dgvData.DataSource = ValuesOfDataGridView.Tables[TableName];

                #region DataGridView Configuration            
                dgvData.Columns["QuantityPerUnit"].Visible = false;
                dgvData.Columns["UnitsOnOrder"].Visible = false;
                dgvData.Columns["ReorderLevel"].Visible = false;

                dgvData.Columns["ProductID"].Width = 70;
                dgvData.Columns["ProductName"].Width = 300;
                dgvData.Columns["SupplierID"].Width = 70;
                dgvData.Columns["CategoryID"].Width = 70;
                dgvData.Columns["UnitPrice"].Width = 80;
                dgvData.Columns["UnitsInStock"].Width = 80;
                dgvData.Columns["Discontinued"].Width = 80;
                #endregion

                tssTotalRegister.Text = "Total Register (" + dgvData.Rows.Count + ")";
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
            }
        }

        private void searchCategory_DoubleClick(object sender, EventArgs e)
        {
            SearchList SearchListCategoryForm = new SearchList();
            string query = "Select CategoryID, CategoryName, Description from Categories";
            string TableName = "Categories";
            int[] SizeColumnsOfDataGridView = new int[] { 70, 150, 200 };
            string[] ValuesOfComboBox = new string[] { "CategoryID", "CategoryName", "Description" };

            #region SearchList Configuration
            SearchListCategoryForm.title = "Categories";
            SearchListCategoryForm.with = 600;
            SearchListCategoryForm.height = 500;
            SearchListCategoryForm.TableName = TableName;
            SearchListCategoryForm.ValuesOfDataGridView = client.GetData(query, TableName);
            SearchListCategoryForm.ColumnOfSortValuesForDataGridView = "CategoryName";
            SearchListCategoryForm.ValuesOfComboBox = ValuesOfComboBox;
            SearchListCategoryForm.InitializeIndexOfComboBox = 1;
            SearchListCategoryForm.SizeColumnsOfDataGridView = SizeColumnsOfDataGridView;
            SearchListCategoryForm.NameColumnForReturnValue = "CategoryID";

            SearchListCategoryForm.LiveSearch = false;
            SearchListCategoryForm.StartPosition = FormStartPosition.CenterScreen;
            SearchListCategoryForm.ShowDialog();

            this.ctCategoryID.Text = SearchListCategoryForm.ReturnValue;
            #endregion            
        }

        private void searchSupplier_DoubleClick(object sender, EventArgs e)
        {
            SearchList SearchListSupplierForm = new SearchList();
            string query = "select SupplierID, CompanyName, Phone from Suppliers";
            string TableName = "Suppliers";
            int[] SizeColumnsOfDataGridView = new int[] { 70, 300, 70 };
            string[] ValuesOfComboBox = new string[] { "SupplierID", "CompanyName", "Phone" };

            #region SearchList Configuration
            SearchListSupplierForm.title = "Suppliers";
            SearchListSupplierForm.with = 600;
            SearchListSupplierForm.height = 500;
            SearchListSupplierForm.TableName = TableName;
            SearchListSupplierForm.ValuesOfDataGridView = client.GetData(query, TableName);
            SearchListSupplierForm.ColumnOfSortValuesForDataGridView = "CompanyName";
            SearchListSupplierForm.ValuesOfComboBox = ValuesOfComboBox;
            SearchListSupplierForm.InitializeIndexOfComboBox = 1;
            SearchListSupplierForm.SizeColumnsOfDataGridView = SizeColumnsOfDataGridView;
            SearchListSupplierForm.NameColumnForReturnValue = "SupplierID";

            SearchListSupplierForm.LiveSearch = false;
            SearchListSupplierForm.StartPosition = FormStartPosition.CenterScreen;
            SearchListSupplierForm.ShowDialog();

            this.ctSupplierID.Text = SearchListSupplierForm.ReturnValue;
            #endregion
        }

        private void FormProducts_Load(object sender, EventArgs e)
        {
            #region Apply filter when press enter key
            foreach (Control item in this.pnTopTab.Controls)
            {
                if (item is TextBox)
                {
                    item.KeyPress += new KeyPressEventHandler(item_WhenKeyPressEvent);
                }
            }
            #endregion

            #region Provicional code
            foreach (var item in this.tsMenu.Items)
            {
                if (item is ToolStripButton)
                {
                    ((ToolStripButton)item).Click += new EventHandler(Item_Click);
                }
            }
            #endregion
        }

        private void Item_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The " + ((ToolStripButton)sender).Text + 
                " option has not implementation ");
        }

        private void item_WhenKeyPressEvent(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSearch.PerformClick();
            }
        }
    }
}
