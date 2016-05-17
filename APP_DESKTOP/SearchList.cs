using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace APP_DESKTOP
{
    public partial class SearchList : Form
    {
        #region Properties of the SearchList
        public string title { get; set; }
        public int with { get; set; }
        public int height { get; set; }
        public string[] ValuesOfComboBox { get; set; }
        public int InitializeIndexOfComboBox { get; set; }
        public int[] SizeColumnsOfDataGridView { get; set; }             
        public DataSet ValuesOfDataGridView { get; set; }
        public string ColumnOfSortValuesForDataGridView { get; set; }
        public string TableName;
        public bool LiveSearch { get; set; }
        public bool ShowAllData { get; set; }

        public string NameColumnForReturnValue { get; set; }
        public string ReturnValue { get; set; }
        WebServReference.WebService1SoapClient WebProxy =
                new WebServReference.WebService1SoapClient();
        #endregion

        public SearchList()
        {
            #region Initialize properties
            title = "Search List";
            with = 600;
            height = 500;
            LiveSearch = false;
            ShowAllData = false;     
                                    
            this.MinimumSize = new Size(300, 200);
            this.MaximumSize = new Size(600, 500);
            
            #endregion
            InitializeComponent();
        }

        public SearchList(string Title, int With, int Height, string[] ValuesOfComboBox, int InitializeIndexOfComboBox,
            int[] SizeColumnsOfDataGridView, DataSet ValuesOfDataGridView, string ColumnOfSortValuesForDataGridView,
            string TableName)
        {
            this.Text = Title;
            this.Width = With;
            this.Height = Height;
            this.dgvData.DataSource = ValuesOfDataGridView.Tables[TableName];
            DataGridViewColumn ColumnSort = dgvData.Columns[ColumnOfSortValuesForDataGridView];
            this.dgvData.Sort(ColumnSort, ListSortDirection.Ascending);
            this.cbbFieldOfSearch.Items.AddRange(ValuesOfComboBox);
            this.cbbFieldOfSearch.SelectedIndex = InitializeIndexOfComboBox;            

            NameColumnForReturnValue = string.IsNullOrEmpty(NameColumnForReturnValue) == true ? 
                dgvData.Columns[0].Name : NameColumnForReturnValue;
        }

        private void LoadDataOfDataGridViewAndSettings()
        {
            this.dgvData.DataSource = ValuesOfDataGridView.Tables[TableName];
            DataGridViewColumn ColumnSort = dgvData.Columns[ColumnOfSortValuesForDataGridView];
            this.dgvData.Sort(ColumnSort, ListSortDirection.Ascending);
            this.cbbFieldOfSearch.Items.AddRange(ValuesOfComboBox);
            this.cbbFieldOfSearch.SelectedIndex = InitializeIndexOfComboBox;
        }

        private void SearchList_Load(object sender, EventArgs e)
        {           
            this.Text = title;
            this.Width = with;
            this.Height = height;
            
            //Charging data of datagrid and settings properties
            LoadDataOfDataGridViewAndSettings();

            for (int i = 0; i < dgvData.Columns.Count - 1; i++)
            {
                dgvData.Columns[i].Width = SizeColumnsOfDataGridView[i];
            }

            if (LiveSearch)
            {
                txtInformationOfSearch.TextChanged +=
                    new EventHandler(txtInformationOfSearch_WhenTextChangedEvent);
            }
            else
            {
                txtInformationOfSearch.KeyPress +=
                    new KeyPressEventHandler(txtInformationOfSearch_WhenKeyPressEvent);
            }
                          
        }

        public void txtInformationOfSearch_WhenTextChangedEvent(object sender, EventArgs e)
        {        
            string value = txtInformationOfSearch.Text;
            string ValueSelectedOfComboBox = cbbFieldOfSearch.Text;
            DataSet ValuesFiltered = new DataSet();

            if (value == null || value == "")
            {
                dgvData.DataSource = ValuesOfDataGridView.Tables[TableName];
            }
            else
            { 
                try
                {
                    string ColumnsOfDataGridView = "";
                    for (int i = 0; i <= dgvData.Columns.Count - 1; i++)
                    {
                        ColumnsOfDataGridView += " " + dgvData.Columns[i].DataPropertyName + ", ";
                    }

                    int IndexOfSymbol = ColumnsOfDataGridView.LastIndexOf(",");
                    ColumnsOfDataGridView = ColumnsOfDataGridView.Substring(0, IndexOfSymbol);

                    string query = "Select " + ColumnsOfDataGridView + " From " + TableName +
                                   " Where " + ValueSelectedOfComboBox + " like '%" + value + "%'";

                    ValuesFiltered = WebProxy.GetData(query, TableName);
                    dgvData.DataSource = ValuesFiltered.Tables[TableName];
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: " + ex.Message);
                }
            }
        }

        public void txtInformationOfSearch_WhenKeyPressEvent(object sender, KeyPressEventArgs e)
        {
            string value = txtInformationOfSearch.Text;
            string ValueSelectedOfComboBox = cbbFieldOfSearch.Text;
            DataSet ValuesFiltered = new DataSet();

            if (e.KeyChar == (char)Keys.Enter && (value == null || value == ""))
            {
                dgvData.DataSource = ValuesOfDataGridView.Tables[TableName];
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                try
                {
                    string ColumnsOfDataGridView = "";
                    for (int i = 0; i <= dgvData.Columns.Count - 1; i++)
                    {
                        ColumnsOfDataGridView += " " + dgvData.Columns[i].DataPropertyName + ", ";
                    }

                    int IndexOfSymbol = ColumnsOfDataGridView.LastIndexOf(",");
                    ColumnsOfDataGridView = ColumnsOfDataGridView.Substring(0, IndexOfSymbol);

                    string query = "Select " + ColumnsOfDataGridView + " From " + TableName + 
                                   " Where " + ValueSelectedOfComboBox + " like '%" + value + "%'";

                    ValuesFiltered = WebProxy.GetData(query, TableName);
                    dgvData.DataSource = ValuesFiltered.Tables[TableName];
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: " + ex.Message);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbbFieldOfSearch_SelectedIndexChanged(object sender, EventArgs e)
        {          
            string ColumnName = cbbFieldOfSearch.Text;

            DataGridViewColumn ColumnSort = dgvData.Columns[ColumnName];
            this.dgvData.Sort(ColumnSort, ListSortDirection.Ascending);
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {                
                string value = dgvData.CurrentRow.Cells[NameColumnForReturnValue].Value.ToString();

                if (!string.IsNullOrEmpty(value))
                {
                    ReturnValue = value;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,"ERROR: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }        
        }

        private void dgvData_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnAccept.PerformClick();
        }
    }
}
