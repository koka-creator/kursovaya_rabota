using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Gruzoperevozki.Data;
using Gruzoperevozki.Models;

namespace Gruzoperevozki.Forms
{
    public partial class TripEditForm : Form
    {
        public Trip? Trip { get; private set; }
        private DataStorage _storage;
        private ComboBox _orderComboBox;
        private ComboBox _carComboBox;
        private CheckedListBox _driversCheckedListBox;
        private DateTimePicker _arrivalDatePicker;
        private DateTimePicker _arrivalTimePicker;
        private ComboBox _statusComboBox;
        private Button _saveButton;
        private Button _cancelButton;

        public TripEditForm(DataStorage storage, Trip? trip = null)
        {
            _storage = storage;
            Trip = trip ?? new Trip { ArrivalDateTime = DateTime.Now };
            InitializeComponent();
            LoadTripData();
        }

        private void InitializeComponent()
        {
            this.Text = Trip?.Id != null ? "Редактирование рейса" : "Добавление рейса";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                AutoScroll = true
            };

            int y = 10;

            panel.Controls.Add(new Label { Text = "Заказ:", Location = new Point(10, y), AutoSize = true });
            _orderComboBox = new ComboBox { Location = new Point(150, y), Size = new Size(400, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            LoadOrdersToComboBox();
            panel.Controls.Add(_orderComboBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Автомобиль:", Location = new Point(10, y), AutoSize = true });
            _carComboBox = new ComboBox { Location = new Point(150, y), Size = new Size(400, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            LoadCarsToComboBox();
            panel.Controls.Add(_carComboBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Водители (экипаж):", Location = new Point(10, y), AutoSize = true });
            _driversCheckedListBox = new CheckedListBox { Location = new Point(150, y), Size = new Size(400, 150) };
            LoadDriversToCheckedListBox();
            panel.Controls.Add(_driversCheckedListBox);
            y += 160;

            panel.Controls.Add(new Label { Text = "Дата прибытия:", Location = new Point(10, y), AutoSize = true });
            _arrivalDatePicker = new DateTimePicker { Location = new Point(150, y), Size = new Size(200, 23), Format = DateTimePickerFormat.Short };
            panel.Controls.Add(_arrivalDatePicker);
            y += 35;

            panel.Controls.Add(new Label { Text = "Время прибытия:", Location = new Point(10, y), AutoSize = true });
            _arrivalTimePicker = new DateTimePicker { Location = new Point(150, y), Size = new Size(200, 23), Format = DateTimePickerFormat.Time, ShowUpDown = true };
            panel.Controls.Add(_arrivalTimePicker);
            y += 35;

            panel.Controls.Add(new Label { Text = "Статус рейса:", Location = new Point(10, y), AutoSize = true });
            _statusComboBox = new ComboBox { Location = new Point(150, y), Size = new Size(300, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            _statusComboBox.Items.AddRange(Enum.GetNames(typeof(TripStatus)));
            panel.Controls.Add(_statusComboBox);
            y += 40;

            _saveButton = new Button { Text = "Сохранить", Location = new Point(150, y), Size = new Size(100, 30), DialogResult = DialogResult.OK };
            _saveButton.Click += SaveButton_Click;
            panel.Controls.Add(_saveButton);

            _cancelButton = new Button { Text = "Отмена", Location = new Point(260, y), Size = new Size(100, 30), DialogResult = DialogResult.Cancel };
            panel.Controls.Add(_cancelButton);

            this.Controls.Add(panel);
        }

        private void LoadOrdersToComboBox()
        {
            _orderComboBox.Items.Clear();
            foreach (var order in _storage.GetOrders())
            {
                var displayName = $"Заказ от {order.OrderDate:dd.MM.yyyy} (Стоимость: {order.Cost:C})";
                _orderComboBox.Items.Add(new OrderComboBoxItem { Order = order, DisplayName = displayName });
            }
            _orderComboBox.DisplayMember = "DisplayName";
        }

        private void LoadCarsToComboBox()
        {
            _carComboBox.Items.Clear();
            foreach (var car in _storage.GetCars())
            {
                var displayName = $"{car.Brand} {car.Model} ({car.StateNumber}) - Грузоподъемность: {car.LoadCapacity}т";
                _carComboBox.Items.Add(new CarComboBoxItem { Car = car, DisplayName = displayName });
            }
            _carComboBox.DisplayMember = "DisplayName";
        }

        private void LoadDriversToCheckedListBox()
        {
            _driversCheckedListBox.Items.Clear();
            foreach (var driver in _storage.GetDrivers())
            {
                var displayName = $"{driver.FullName} ({driver.EmployeeNumber}) - {driver.Category}, {driver.Class}";
                _driversCheckedListBox.Items.Add(new DriverCheckedListItem { Driver = driver, DisplayName = displayName }, false);
            }
            _driversCheckedListBox.DisplayMember = "DisplayName";
        }

        private void LoadTripData()
        {
            if (Trip == null) return;

            var orderItem = _orderComboBox.Items.Cast<OrderComboBoxItem>()
                .FirstOrDefault(i => i.Order.Id == Trip.OrderId);
            if (orderItem != null) _orderComboBox.SelectedItem = orderItem;

            var carItem = _carComboBox.Items.Cast<CarComboBoxItem>()
                .FirstOrDefault(i => i.Car.Id == Trip.CarId);
            if (carItem != null) _carComboBox.SelectedItem = carItem;

            _arrivalDatePicker.Value = Trip.ArrivalDateTime.Date;
            _arrivalTimePicker.Value = DateTime.Today.Add(Trip.ArrivalDateTime.TimeOfDay);
            _statusComboBox.SelectedItem = Trip.Status.ToString();

            for (int i = 0; i < _driversCheckedListBox.Items.Count; i++)
            {
                var item = (DriverCheckedListItem)_driversCheckedListBox.Items[i];
                _driversCheckedListBox.SetItemChecked(i, Trip.DriverIds.Contains(item.Driver.Id));
            }
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            if (Trip == null) return;

            if (_orderComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите заказ", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_carComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите автомобиль", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var checkedDrivers = _driversCheckedListBox.CheckedItems.Cast<DriverCheckedListItem>().ToList();
            if (checkedDrivers.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одного водителя", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Trip.OrderId = ((OrderComboBoxItem)_orderComboBox.SelectedItem).Order.Id;
            Trip.CarId = ((CarComboBoxItem)_carComboBox.SelectedItem).Car.Id;
            Trip.DriverIds = checkedDrivers.Select(d => d.Driver.Id).ToList();
            Trip.ArrivalDateTime = _arrivalDatePicker.Value.Date.Add(_arrivalTimePicker.Value.TimeOfDay);
            
            if (_statusComboBox.SelectedItem != null && Enum.TryParse<TripStatus>(_statusComboBox.SelectedItem.ToString(), out var status))
            {
                // Проверка: нельзя завершить рейс раньше чем на 5 дней от текущей даты
                if (status == TripStatus.Завершен)
                {
                    DateTime tripDate = Trip.ArrivalDateTime;
                    DateTime minDate = DateTime.Now.AddDays(-5);
                    
                    if (tripDate < minDate)
                    {
                        MessageBox.Show($"Нельзя завершить рейс, который был более 5 дней назад. Минимальная дата завершения: {minDate:dd.MM.yyyy}", 
                            "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.DialogResult = DialogResult.None;
                        return;
                    }
                }
                
                Trip.Status = status;
            }

            this.DialogResult = DialogResult.OK;
        }

        private class OrderComboBoxItem
        {
            public Order Order { get; set; } = null!;
            public string DisplayName { get; set; } = string.Empty;
        }

        private class CarComboBoxItem
        {
            public Car Car { get; set; } = null!;
            public string DisplayName { get; set; } = string.Empty;
        }

        private class DriverCheckedListItem
        {
            public Driver Driver { get; set; } = null!;
            public string DisplayName { get; set; } = string.Empty;
        }
    }
}


