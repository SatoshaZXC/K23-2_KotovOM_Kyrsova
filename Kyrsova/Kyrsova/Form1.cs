using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;



namespace Kyrsova
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

       

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void NunberOld_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                MessageBox.Show("Please, clear the field before loading new XML-file!", "Error!");
            }
            else
            {
                if (File.Exists("C:\\Users\\ZXC\\source\\repos\\NewRepo\\Kyrsova\\Kyrsova\\XMLFile1.xml"))
                {
                    DataSet dataSet = new DataSet();
                    dataSet.ReadXml("C:\\Users\\ZXC\\source\\repos\\NewRepo\\Kyrsova\\Kyrsova\\XMLFile1.xml");

                    foreach (DataRow item in dataSet.Tables[0].Rows)
                    {
                        int n = dataGridView2.Rows.Add();
                        dataGridView2.Rows[n].Cells[0].Value = item["date"];
                        dataGridView2.Rows[n].Cells[1].Value = item["supplier_company_name"];
                        dataGridView2.Rows[n].Cells[2].Value = item["director_full_name"];
                        dataGridView2.Rows[n].Cells[3].Value = item["delivery_time"];
                        dataGridView2.Rows[n].Cells[4].Value = item["cargo_weight_kg"];
                    }
                }
                else
                {
                    MessageBox.Show("XML-file not found!", "Error!");
                }
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
                int n = dataGridView2.Rows.Add();
                dataGridView2.Rows[n].Cells[0].Value = dateTimePicker1.Text;
                dataGridView2.Rows[n].Cells[1].Value = comboBox1.Text;
                dataGridView2.Rows[n].Cells[2].Value = textBox3.Text;
                dataGridView2.Rows[n].Cells[3].Value = textBox1.Text;
                dataGridView2.Rows[n].Cells[4].Value = numericUpDown2.Text;
            
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int n = dataGridView2.SelectedRows[0].Index;
            dataGridView2.Rows[n].Cells[0].Value = dateTimePicker1.Text;
            dataGridView2.Rows[n].Cells[1].Value = comboBox1.Text;
            dataGridView2.Rows[n].Cells[2].Value = textBox3.Text;
            dataGridView2.Rows[n].Cells[3].Value = textBox1.Text;
            dataGridView2.Rows[n].Cells[4].Value = numericUpDown2.Text;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                dataGridView2.Rows.RemoveAt(dataGridView2.SelectedRows[0].Index);
            }
            else
            {
                MessageBox.Show("Error!");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet dataSet = new DataSet();
                DataTable dataTable = new DataTable();
                dataTable.TableName = "Sigma";
                dataTable.Columns.Add("date");
                dataTable.Columns.Add("supplier_company_name");
                dataTable.Columns.Add("director_full_name");
                dataTable.Columns.Add("delivery_time");
                dataTable.Columns.Add("cargo_weight_kg");
               

                dataSet.Tables.Add(dataTable);

                foreach (DataGridViewRow r in dataGridView2.Rows)
                {
                    DataRow row = dataSet.Tables["Sigma"].NewRow();
                    row["date"] = r.Cells[0].Value;
                    row["supplier_company_name"] = r.Cells[1].Value;
                    row["director_full_name"] = r.Cells[2].Value;
                    row["delivery_time"] = r.Cells[3].Value;
                    row["cargo_weight_kg"] = r.Cells[4].Value;
                   

                    dataSet.Tables["Sigma"].Rows.Add(row);
                }
                dataSet.WriteXml("C:\\Users\\ZXC\\source\\repos\\NewRepo\\Kyrsova\\Kyrsova\\XMLFile1.xml");

                MessageBox.Show("XML-file is saved!", "Done!");
            }
            catch
            {
                MessageBox.Show("Could not save XML-file", "Error!");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                dataGridView2.Rows.Clear();
            }
            else
            {
                MessageBox.Show("Table is empty!", "Error!");
            }
        }
    }
}
//zxczcxzxczxc