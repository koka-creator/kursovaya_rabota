using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Gruzoperevozki.Models;

namespace Gruzoperevozki.Forms
{
    public partial class CarEditForm : Form
    {
        public Car? Car { get; private set; }
        private TextBox _stateNumberTextBox;
        private TextBox _brandTextBox;
        private TextBox _modelTextBox;
        private NumericUpDown _loadCapacityNumeric;
        private TextBox _purposeTextBox;
        private NumericUpDown _manufactureYearNumeric;
        private NumericUpDown? _overhaulYearNumeric;
        private NumericUpDown _mileageNumeric;
        private PictureBox _photoPictureBox;
        private Button _loadPhotoButton;
        private Button _saveButton;
        private Button _cancelButton;

        public CarEditForm(Car? car = null)
        {
            Car = car ?? new Car();
            InitializeComponent();
            LoadCarData();
        }

        private void InitializeComponent()
        {
            this.Text = Car?.Id != null ? "Редактирование автомобиля" : "Добавление автомобиля";
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

            panel.Controls.Add(new Label { Text = "Гос. номер:", Location = new Point(10, y), AutoSize = true });
            _stateNumberTextBox = new TextBox { Location = new Point(150, y), Size = new Size(400, 23), Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top };
            panel.Controls.Add(_stateNumberTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Марка:", Location = new Point(10, y), AutoSize = true });
            _brandTextBox = new TextBox { Location = new Point(150, y), Size = new Size(400, 23), Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top };
            panel.Controls.Add(_brandTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Модель:", Location = new Point(10, y), AutoSize = true });
            _modelTextBox = new TextBox { Location = new Point(150, y), Size = new Size(400, 23), Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top };
            panel.Controls.Add(_modelTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Грузоподъемность (т):", Location = new Point(10, y), AutoSize = true });
            _loadCapacityNumeric = new NumericUpDown { Location = new Point(150, y), Size = new Size(200, 23), Minimum = 0, Maximum = 1000, DecimalPlaces = 2 };
            panel.Controls.Add(_loadCapacityNumeric);
            y += 35;

            panel.Controls.Add(new Label { Text = "Назначение:", Location = new Point(10, y), AutoSize = true });
            _purposeTextBox = new TextBox { Location = new Point(150, y), Size = new Size(400, 23), Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top };
            panel.Controls.Add(_purposeTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Год выпуска:", Location = new Point(10, y), AutoSize = true });
            _manufactureYearNumeric = new NumericUpDown { Location = new Point(150, y), Size = new Size(200, 23), Minimum = 1900, Maximum = DateTime.Now.Year, Value = DateTime.Now.Year };
            panel.Controls.Add(_manufactureYearNumeric);
            y += 35;

            panel.Controls.Add(new Label { Text = "Год кап. ремонта:", Location = new Point(10, y), AutoSize = true });
            _overhaulYearNumeric = new NumericUpDown { Location = new Point(150, y), Size = new Size(200, 23), Minimum = 1900, Maximum = DateTime.Now.Year };
            panel.Controls.Add(_overhaulYearNumeric);
            y += 35;

            panel.Controls.Add(new Label { Text = "Пробег на начало года:", Location = new Point(10, y), AutoSize = true });
            _mileageNumeric = new NumericUpDown { Location = new Point(150, y), Size = new Size(200, 23), Minimum = 0, Maximum = 10000000, DecimalPlaces = 0 };
            panel.Controls.Add(_mileageNumeric);
            y += 35;

            panel.Controls.Add(new Label { Text = "Фотография:", Location = new Point(10, y), AutoSize = true });
            _photoPictureBox = new PictureBox { Location = new Point(150, y), Size = new Size(200, 150), BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom };
            panel.Controls.Add(_photoPictureBox);
            y += 160;

            _loadPhotoButton = new Button { Text = "Загрузить фото", Location = new Point(150, y), Size = new Size(120, 30) };
            _loadPhotoButton.Click += LoadPhotoButton_Click;
            panel.Controls.Add(_loadPhotoButton);
            y += 40;

            _saveButton = new Button { Text = "Сохранить", Location = new Point(150, y), Size = new Size(100, 30), DialogResult = DialogResult.OK };
            _saveButton.Click += SaveButton_Click;
            panel.Controls.Add(_saveButton);

            _cancelButton = new Button { Text = "Отмена", Location = new Point(260, y), Size = new Size(100, 30), DialogResult = DialogResult.Cancel };
            panel.Controls.Add(_cancelButton);

            this.Controls.Add(panel);
        }

        private void LoadCarData()
        {
            if (Car == null) return;

            _stateNumberTextBox.Text = Car.StateNumber;
            _brandTextBox.Text = Car.Brand;
            _modelTextBox.Text = Car.Model;
            
            // Безопасная установка значений в NumericUpDown
            if (Car.LoadCapacity >= _loadCapacityNumeric.Minimum && Car.LoadCapacity <= _loadCapacityNumeric.Maximum)
                _loadCapacityNumeric.Value = Car.LoadCapacity;
            else
                _loadCapacityNumeric.Value = _loadCapacityNumeric.Minimum;
            
            _purposeTextBox.Text = Car.Purpose;
            
            if (Car.ManufactureYear >= _manufactureYearNumeric.Minimum && Car.ManufactureYear <= _manufactureYearNumeric.Maximum)
                _manufactureYearNumeric.Value = Car.ManufactureYear;
            else
                _manufactureYearNumeric.Value = DateTime.Now.Year;
            
            if (_overhaulYearNumeric != null)
            {
                if (Car.OverhaulYear.HasValue && 
                    Car.OverhaulYear.Value >= _overhaulYearNumeric.Minimum && 
                    Car.OverhaulYear.Value <= _overhaulYearNumeric.Maximum)
                {
                    _overhaulYearNumeric.Value = Car.OverhaulYear.Value;
                }
                else
                {
                    _overhaulYearNumeric.Value = _overhaulYearNumeric.Minimum;
                }
            }
            
            if (Car.MileageAtYearStart >= _mileageNumeric.Minimum && Car.MileageAtYearStart <= _mileageNumeric.Maximum)
                _mileageNumeric.Value = Car.MileageAtYearStart;
            else
                _mileageNumeric.Value = _mileageNumeric.Minimum;

            if (!string.IsNullOrEmpty(Car.PhotoPath) && File.Exists(Car.PhotoPath))
            {
                try
                {
                    _photoPictureBox.Image = Image.FromFile(Car.PhotoPath);
                }
                catch { }
            }
        }

        private void LoadPhotoButton_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp|Все файлы|*.*",
                Title = "Выберите фотографию автомобиля"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var image = Image.FromFile(dialog.FileName);
                    _photoPictureBox.Image = image;
                    if (Car != null)
                    {
                        Car.PhotoPath = dialog.FileName;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_stateNumberTextBox.Text))
            {
                MessageBox.Show("Введите государственный номер", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (string.IsNullOrWhiteSpace(_brandTextBox.Text))
            {
                MessageBox.Show("Введите марку автомобиля", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (string.IsNullOrWhiteSpace(_modelTextBox.Text))
            {
                MessageBox.Show("Введите модель автомобиля", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (string.IsNullOrWhiteSpace(_purposeTextBox.Text))
            {
                MessageBox.Show("Введите назначение автомобиля", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            // Проверка года капитального ремонта (не может быть раньше года выпуска)
            if (_overhaulYearNumeric != null && _overhaulYearNumeric.Value > 0)
            {
                int manufactureYear = (int)_manufactureYearNumeric.Value;
                int overhaulYear = (int)_overhaulYearNumeric.Value;
                if (overhaulYear < manufactureYear)
                {
                    MessageBox.Show("Год капитального ремонта не может быть раньше года выпуска", "Ошибка валидации", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.None;
                    return;
                }
            }

            if (Car == null) return;

            Car.StateNumber = _stateNumberTextBox.Text;
            Car.Brand = _brandTextBox.Text;
            Car.Model = _modelTextBox.Text;
            Car.LoadCapacity = _loadCapacityNumeric.Value;
            Car.Purpose = _purposeTextBox.Text;
            Car.ManufactureYear = (int)_manufactureYearNumeric.Value;
            if (_overhaulYearNumeric != null && _overhaulYearNumeric.Value > 0)
            {
                Car.OverhaulYear = (int)_overhaulYearNumeric.Value;
            }
            else
            {
                Car.OverhaulYear = null;
            }
            Car.MileageAtYearStart = _mileageNumeric.Value;

            this.DialogResult = DialogResult.OK;
        }
    }
}

