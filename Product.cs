
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace MachineTest
{
    public partial class Product : Form
    {
        public Product()
        {
            InitializeComponent();
        }

        Connection cn = new Connection();
        string strsql = "";
        DataTable dt = new DataTable();

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 46)
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && !char.IsWhiteSpace(e.KeyChar) || e.KeyChar == (char)Keys.Space)
            {
                e.Handled = true;
            }
        }

        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime ExpDate = dpExpiryDate.Value;
                if (txtName.Text.Length < 2)
                {
                    MessageBox.Show("Please Enter Valid Product Name.", "Values Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName.Focus();
                }
                else if (txtPrice.Text == "")
                {
                    MessageBox.Show("Please Enter Product Price.", "Values Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPrice.Focus();
                }
                else if (txtQty.Text == "")
                {
                    MessageBox.Show("Please Enter Product Quantity.", "Values Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                }
                else if (ExpDate <= DateTime.Now)
                {
                    MessageBox.Show("Please Enter Valid Product Expiry Date.", "Values Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dpExpiryDate.Focus();
                }
                else if (cbColor.SelectedIndex < 1)
                {
                    MessageBox.Show("Please Select product Color.", "Values Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cbColor.Focus();
                }
                else if (CheckDuplication(txtName.Text) == 1)
                {
                    MessageBox.Show("This product added in list,Please try with another product name.", "Duplicate Values", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    bool IsGST = (rbGST.Checked ? true : false);
                    strsql = "Insert into Product values('" + txtName.Text + "',"+txtPrice.Text+"," + txtQty.Text + ",'" + IsGST + "','" + dpPurchareSate.Value.ToString("yyyy-MM-dd") + "','" + dpExpiryDate.Value.ToString("yyyy-MM-dd") + "','" + cbColor.SelectedItem + "',1)";
                    dt = cn.operationOnDataBase(strsql);
                    MessageBox.Show("Data Inserted Successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearAll();
                    PopulateWaiterGrid("All", "");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int CheckDuplication(string ProdNm)
        {
            try
            {
                strsql = "Select Id from Product where productName='" + ProdNm + "'";
                dt = cn.operationOnDataBase(strsql);
                if (dt.Rows.Count > 0)
                {
                    return 1;
                }
                else
                { return 0; }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void Product_Load(object sender, EventArgs e)
        {
            PopulateWaiterGrid("All","");
            cbColor.SelectedIndex = 0;
            cbSort.SelectedIndex = 0;
        }
        public void PopulateWaiterGrid(string Nm,string OrderBy)
        {
            try
            {
                if (OrderBy == "")
                {
                    if (Nm == "All")
                    {
                        strsql = "Select Id,productName,Quantity,IsGST,PurchaseDate,ExpiryDate,Color from Product ";
                    }
                    else
                    {
                        strsql = "Select Id,productName,Quantity,IsGST,PurchaseDate,ExpiryDate,Color from Product where productName like  '%'+ '" + Nm + "' + '%' ";
                    }
                }
                else
                {
                    if (OrderBy == "Purchase Date")
                    {
                        strsql = "Select Id,productName,Quantity,IsGST,PurchaseDate,ExpiryDate,Color from Product where productName like  '%'+ '" + Nm + "' + '%' order by PurchaseDate";
                    }
                    else
                    {
                        strsql = "Select Id,productName,Quantity,IsGST,PurchaseDate,ExpiryDate,Color from Product where productName like  '%'+ '" + Nm + "' + '%' order by productName";
                    }
                }
                DataTable dtProd = new DataTable();
                dtProd = cn.operationOnDataBase(strsql);
                dgvProduct.DataSource = dtProd;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            PopulateWaiterGrid(txtSearch.Text,"");
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            PopulateWaiterGrid(txtSearch.Text,"");
        }

      
        public void ClearAll()
        {
            txtName.Text = "";
            txtPrice.Text = "";
            txtQty.Text = "";
            cbColor.SelectedIndex = 0;
            dpPurchareSate.Value = DateTime.Now;
            dpExpiryDate.Value = DateTime.Now;
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
        }

        private void cbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSort.SelectedIndex > 0)
            {
                PopulateWaiterGrid(txtSearch.Text, cbSort.SelectedItem.ToString());
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvProduct.Rows.Count > 0)
                {
                    this.dgvProduct = dgvProduct;
                    //Creating DataTable
                    System.Data.DataTable dt = new System.Data.DataTable();

                    //Adding the Columns
                    foreach (DataGridViewColumn column in dgvProduct.Columns)
                    {
                        dt.Columns.Add(column.HeaderText);
                    }
                    //Adding the Rows
                    foreach (DataGridViewRow row in dgvProduct.Rows)
                    {
                        dt.Rows.Add();
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.ValueType.Name == "Boolean")
                                dt.Rows[dt.Rows.Count - 1][cell.ColumnIndex] = Convert.ToBoolean(cell.Value).ToString();
                            else
                                if (cell.Value != null)
                                dt.Rows[dt.Rows.Count - 1][cell.ColumnIndex] = cell.Value.ToString();
                        }
                    }
                    try
                    {
                        SaveFileDialog saveDialog = new SaveFileDialog();
                        saveDialog.CheckPathExists = false;
                        saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                        saveDialog.FilterIndex = 2;
                        saveDialog.FileName = "ProductList-" + Convert.ToDateTime(System.DateTime.Now).ToString("dd-MM-yyyy");
                        if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                wb.Worksheets.Add(dt, "ProductInfo");
                                try
                                {
                                    wb.SaveAs(saveDialog.FileName);
                                    MessageBox.Show("Export Successful");
                                }
                                catch (IOException)
                                {
                                    MessageBox.Show("File is already open please close the file...");

                                }
                                catch (Exception ex)
                                {
                                    string ext = Path.GetExtension(saveDialog.FileName);
                                    MessageBox.Show("Extension '" + ext + "' is not supported.\nSupported extensions are '.xlsx' and '.xslm'.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("There is no data to export");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
