using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Gruzoperevozki.Data;
using Gruzoperevozki.Models;

namespace Gruzoperevozki.Forms
{
    public partial class CarsForm : Form
    {
        private DataStorage _storage = DataStorage.Instance;
        private ListView _listView;
        private Button _addButton;
        private Button _editButton;
        private Button _deleteButton;
        private Button _refreshButton;

        public CarsForm()
        {
            InitializeComponent();
            this.FormClosing += (s, e) => _storage.SaveData();
            LoadCars();
        }

        private void InitializeComponent()
        {
            this.Text = "Управление автомобилями";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            _listView.Columns.Add("Гос. номер", 100);
            _listView.Columns.Add("Марка", 100);
            _listView.Columns.Add("Модель", 100);
            _listView.Columns.Add("Грузоподъемность", 120);
            _listView.Columns.Add("Назначение", 150);
            _listView.Columns.Add("Год выпуска", 100);
            _listView.Columns.Add("Год кап. ремонта", 120);
            _listView.Columns.Add("Пробег", 100);

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
            _refreshButton.Click += (s, e) => LoadCars();

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

        private void LoadCars()
        {
            _listView.Items.Clear();
            foreach (var car in _storage.GetCars())
            {
                var item = new ListViewItem(car.StateNumber);
                item.SubItems.Add(car.Brand);
                item.SubItems.Add(car.Model);
                item.SubItems.Add(car.LoadCapacity.ToString());
                item.SubItems.Add(car.Purpose);
                item.SubItems.Add(car.ManufactureYear.ToString());
                item.SubItems.Add(car.OverhaulYear?.ToString() ?? "-");
                item.SubItems.Add(car.MileageAtYearStart.ToString());
                item.Tag = car;
                _listView.Items.Add(item);
            }
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            using var form = new CarEditForm();
            if (form.ShowDialog() == DialogResult.OK && form.Car != null)
            {
                _storage.AddCar(form.Car);
                _storage.SaveData();
                LoadCars();
            }
        }

        private void EditButton_Click(object? sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите автомобиль для редактирования", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var car = (Car)_listView.SelectedItems[0].Tag;
            using var form = new CarEditForm(car);
            if (form.ShowDialog() == DialogResult.OK && form.Car != null)
            {
                _storage.UpdateCar(form.Car);
                _storage.SaveData();
                LoadCars();
            }
        }

        private void DeleteButton_Click(object? sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите автомобиль для удаления", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить этот автомобиль?", "Подтверждение", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var car = (Car)_listView.SelectedItems[0].Tag;
                _storage.DeleteCar(car.Id);
                _storage.SaveData();
                LoadCars();
            }
        }
    }
}

