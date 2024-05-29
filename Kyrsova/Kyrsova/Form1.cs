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
using System.Globalization;
using System.Xml.Linq;



namespace Kyrsova
{
    public partial class Form1 : Form
    {
        // Метод загрузки данных из XML файла
        private XDocument LoadXml()
        {
            string filePath = @"C:\Users\ZXC\source\repos\NewRepo\Kyrsova\Kyrsova\XMLFile1.xml";
            return XDocument.Load(filePath);
        }
        // Очистка DataGridView
        private void ClearDataGridView1()
        {
            dataGridView.Rows.Clear();
        }
        private void ClearDataGridView2()
        {
            dataGridView2.Rows.Clear();
        }
        // Метод для расчета времени разгрузки
        private TimeSpan CalculateUnloadingTime(double weight)
        {
            // На 100 кг требуется 20 минут
            double minutesPer100Kg = 20;
            double totalMinutes = (weight / 100) * minutesPer100Kg;
            return TimeSpan.FromMinutes(totalMinutes);
        }
        private void loadTable(DataGridView dataGridView1)
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
                        int n = dataGridView1.Rows.Add();
                        dataGridView1.Rows[n].Cells[0].Value = item["date"];
                        dataGridView1.Rows[n].Cells[1].Value = item["supplier_company_name"];
                        dataGridView1.Rows[n].Cells[2].Value = item["director_full_name"];
                        dataGridView1.Rows[n].Cells[3].Value = item["delivery_time"];
                        dataGridView1.Rows[n].Cells[4].Value = item["cargo_weight_kg"];
                    }
                }
                else
                {
                    MessageBox.Show("XML-file not found!", "Error!");
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            radioButton3.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            radioButton4.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            radioButton5.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
        }
        // Обработчик события изменения состояния RadioButton
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender as RadioButton).Checked)
                return;

            ClearDataGridView1();

            var doc = LoadXml();
            var today = DateTime.Today;

            if (sender == radioButton3)
            {
                // Поставки на завтра в первой половине дня (до 12:00)
                var tomorrow = today.AddDays(1);
                var supplies = doc.Descendants("Sigma")
                    .Where(s =>
                    {
                        var dateElement = s.Element("date");
                        var timeElement = s.Element("delivery_time");
                        return dateElement != null && timeElement != null &&
                               DateTime.Parse(dateElement.Value) == tomorrow &&
                               TimeSpan.Parse(timeElement.Value) < new TimeSpan(12, 0, 0);
                    })
                    .Select(s => new
                    {
                        Date = DateTime.Parse(s.Element("date").Value),
                        Supplier = s.Element("supplier_company_name")?.Value,
                        Director = s.Element("director_full_name")?.Value,
                        Time = s.Element("delivery_time")?.Value,
                        Weight = double.Parse(s.Element("cargo_weight_kg")?.Value ?? "0")
                    }).ToList();

                foreach (var supply in supplies)
                {
                    int rowIndex = dataGridView.Rows.Add();
                    dataGridView.Rows[rowIndex].Cells[0].Value = supply.Date;
                    dataGridView.Rows[rowIndex].Cells[1].Value = supply.Supplier;
                    dataGridView.Rows[rowIndex].Cells[2].Value = supply.Director;
                    dataGridView.Rows[rowIndex].Cells[3].Value = supply.Time;
                    dataGridView.Rows[rowIndex].Cells[4].Value = supply.Weight;
                }
            }
            else if (sender == radioButton4)
            {
                // Последняя поставка текущего дня
                var supplies = doc.Descendants("Sigma")
                    .Where(s =>
                    {
                        var dateElement = s.Element("date");
                        return dateElement != null && DateTime.Parse(dateElement.Value) == today;
                    })
                    .Select(s => new
                    {
                        Date = DateTime.Parse(s.Element("date").Value),
                        Supplier = s.Element("supplier_company_name")?.Value,
                        Director = s.Element("director_full_name")?.Value,
                        Time = TimeSpan.Parse(s.Element("delivery_time")?.Value ?? "00:00"),
                        Weight = double.Parse(s.Element("cargo_weight_kg")?.Value ?? "0")
                    })
                    .OrderByDescending(s => s.Time)
                    .FirstOrDefault();

                if (supplies != null)
                {
                    int rowIndex = dataGridView.Rows.Add();
                    dataGridView.Rows[rowIndex].Cells[0].Value = supplies.Date;
                    dataGridView.Rows[rowIndex].Cells[1].Value = supplies.Supplier;
                    dataGridView.Rows[rowIndex].Cells[2].Value = supplies.Director;
                    dataGridView.Rows[rowIndex].Cells[3].Value = supplies.Time;
                    dataGridView.Rows[rowIndex].Cells[4].Value = supplies.Weight;
                }
            }
            else if (sender == radioButton5)
            {
                // Сортировка и создание файлов по дате поставки
                var supplies = doc.Descendants("Sigma")
                    .Where(s => s.Element("date") != null && s.Element("supplier_company_name") != null &&
                                s.Element("director_full_name") != null && s.Element("delivery_time") != null &&
                                s.Element("cargo_weight_kg") != null)
                    .Select(s => new
                    {
                        Date = DateTime.Parse(s.Element("date").Value),
                        Supplier = s.Element("supplier_company_name").Value,
                        Director = s.Element("director_full_name").Value,
                        Time = s.Element("delivery_time").Value,
                        Weight = double.Parse(s.Element("cargo_weight_kg").Value)
                    })
                    .OrderBy(s => s.Date) // Сортировка по дате
                    .ToList();

                var groupedSupplies = supplies.GroupBy(s => s.Date).OrderBy(g => g.Key);

                // Добавление отсортированных данных в DataGridView
                foreach (var supply in supplies)
                {
                    int rowIndex = dataGridView.Rows.Add();
                    dataGridView.Rows[rowIndex].Cells[0].Value = supply.Date.ToString("dd.MM.yyyy");
                    //dataGridView.Rows[rowIndex].Cells[0].Value = supply.Date;
                    dataGridView.Rows[rowIndex].Cells[1].Value = supply.Supplier;
                    dataGridView.Rows[rowIndex].Cells[2].Value = supply.Director;
                    dataGridView.Rows[rowIndex].Cells[3].Value = supply.Time;
                    dataGridView.Rows[rowIndex].Cells[4].Value = supply.Weight;
                }

                // Создание файлов
                foreach (var group in groupedSupplies)
                {
                    string fileName = $@"C:\Users\ZXC\source\repos\NewRepo\Kyrsova\Kyrsova\{group.Key:yyyyMMdd}.txt";
                    using (StreamWriter writer = new StreamWriter(fileName))
                    {
                        foreach (var supply in group)
                        {
                            writer.WriteLine($"Date: {supply.Date}, Supplier: {supply.Supplier}, Director: {supply.Director}, Time: {supply.Time}, Weight: {supply.Weight} kg");
                        }
                    }
                }

                MessageBox.Show("Файлы созданы и упорядочены по дате поставки.");
            }
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
            ClearDataGridView2();
            loadTable(dataGridView2);
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

        private void button1_Click(object sender, EventArgs e)
        {
            ClearDataGridView1();
            loadTable(dataGridView);
            

        }

        private void button8_Click(object sender, EventArgs e)
        {

            if (dataGridView.SelectedRows.Count > 0)
            {
                // Получаем выбранную строку
                DataGridViewRow selectedRow = dataGridView.SelectedRows[0];

                // Получаем вес из ячейки 4
                double weight = Convert.ToDouble(selectedRow.Cells[4].Value);

                // Получаем текущее время из ячейки 3
                DateTime currentTime = Convert.ToDateTime(selectedRow.Cells[3].Value);

                // Вычисляем время разгрузки
                TimeSpan unloadingTime = CalculateUnloadingTime(weight);

                // Добавляем время разгрузки к текущему времени
                DateTime endTime = currentTime.Add(unloadingTime);

                // Обновляем текст лейбла
                label7.Text = $"{endTime:HH:mm:ss}";
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть рядок у таблиці.");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // Получаем выбранный год из numericUpDown1
            int year = (int)numericUpDown1.Value;
            // Указываем путь к XML файлу
            string filePath = @"C:\Users\ZXC\source\repos\NewRepo\Kyrsova\Kyrsova\XMLFile1.xml";

            try
            {
                // Загружаем XML файл
                var doc = XDocument.Load(filePath);
                // Выбираем все элементы "Sigma" и фильтруем их по указанному году
                var supplies = doc.Descendants("Sigma")
                    .Where(s =>
                    {
                        var dateElement = s.Element("date");
                        return dateElement != null && DateTime.Parse(dateElement.Value).Year == year;
                    })
                    .Select(s => new
                    {
                        Date = DateTime.Parse(s.Element("date").Value), // Преобразуем строку в дату
                        Weight = s.Element("cargo_weight_kg") != null ? double.Parse(s.Element("cargo_weight_kg").Value) : 0 // Преобразуем строку в число
                    }).ToList();

                // Считаем количество поставок
                int supplyCount = supplies.Count;
                // Суммируем общий вес всех поставок
                double totalWeight = supplies.Sum(s => s.Weight);

                // Обновляем текст label9, выводя результаты
                label9.Text = $"Рік: {year}\nКількість поставок: {supplyCount}\nЗагальна вага: {totalWeight} кг";
            }
            catch (Exception ex)
            {
                // Выводим сообщение об ошибке в случае возникновения исключения
                MessageBox.Show("Виникла помилка при обробці файлу: " + ex.Message);
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

   
}

//zxczcxzxczxc