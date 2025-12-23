using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Gruzoperevozki.Data;
using Gruzoperevozki.Models;

namespace Gruzoperevozki.Forms
{
    public partial class DriversForm : Form
    {
        private DataStorage _storage = DataStorage.Instance;
        private ListView _listView;
        private Button _addButton;
        private Button _editButton;
        private Button _deleteButton;
        private Button _refreshButton;

        public DriversForm()
        {
            InitializeComponent();
            this.FormClosing += (s, e) => _storage.SaveData();
            LoadDrivers();
        }

        private void InitializeComponent()
        {
            this.Text = "Управление водителями";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            _listView.Columns.Add("Табельный номер", 120);
            _listView.Columns.Add("ФИО", 200);
            _listView.Columns.Add("Год рождения", 100);
            _listView.Columns.Add("Стаж работы", 100);
            _listView.Columns.Add("Категория", 100);
            _listView.Columns.Add("Классность", 100);

            _addButton = new Button
            {
                Text = "Добавить",
                Size = new Size(100, 30),
                Location = new Point(10, 10)
            };
            _addButton.Click += AddButton_Click;

            _editButton = new Button
            {
                Text = "Редактировать",
                Size = new Size(100, 30),
                Location = new Point(120, 10)
            };
            _editButton.Click += EditButton_Click;

            _deleteButton = new Button
            {
                Text = "Удалить",
                Size = new Size(100, 30),
                Location = new Point(230, 10)
            };
            _deleteButton.Click += DeleteButton_Click;

            _refreshButton = new Button
            {
                Text = "Обновить",
                Size = new Size(100, 30),
                Location = new Point(340, 10)
            };
            _refreshButton.Click += (s, e) => LoadDrivers();

            var buttonPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top
            };
            buttonPanel.Controls.AddRange(new Control[] { _addButton, _editButton, _deleteButton, _refreshButton });

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            mainPanel.Controls.Add(_listView);

            this.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);
        }

        private void LoadDrivers()
        {
            _listView.Items.Clear();
            foreach (var driver in _storage.GetDrivers())
            {
                var item = new ListViewItem(driver.EmployeeNumber);
                item.SubItems.Add(driver.FullName);
                item.SubItems.Add(driver.BirthYear.ToString());
                item.SubItems.Add(driver.WorkExperience.ToString());
                item.SubItems.Add(driver.Category);
                item.SubItems.Add(driver.Class);
                item.Tag = driver;
                _listView.Items.Add(item);
            }
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            using var form = new DriverEditForm();
            if (form.ShowDialog() == DialogResult.OK && form.Driver != null)
            {
                _storage.AddDriver(form.Driver);
                _storage.SaveData();
                LoadDrivers();
            }
        }

        private void EditButton_Click(object? sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите водителя для редактирования", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var driver = (Driver)_listView.SelectedItems[0].Tag;
            using var form = new DriverEditForm(driver);
            if (form.ShowDialog() == DialogResult.OK && form.Driver != null)
            {
                _storage.UpdateDriver(form.Driver);
                _storage.SaveData();
                LoadDrivers();
            }
        }

        private void DeleteButton_Click(object? sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите водителя для удаления", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить этого водителя?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var driver = (Driver)_listView.SelectedItems[0].Tag;
                _storage.DeleteDriver(driver.Id);
                _storage.SaveData();
                LoadDrivers();
            }
        }
    }
}

