using System;
using System.Drawing;
using System.Windows.Forms;
using Gruzoperevozki.Models;

namespace Gruzoperevozki.Forms
{
    public partial class CargoItemEditForm : Form
    {
        public CargoItem? CargoItem { get; private set; }
        private TextBox _nameTextBox;
        private TextBox _unitTextBox;
        private NumericUpDown _quantityNumeric;
        private NumericUpDown _weightNumeric;
        private NumericUpDown _insuranceValueNumeric;
        private Button _saveButton;
        private Button _cancelButton;

        public CargoItemEditForm(CargoItem? cargoItem = null)
        {
            CargoItem = cargoItem ?? new CargoItem();
            InitializeComponent();
            LoadCargoItemData();
        }

        private void InitializeComponent()
        {
            this.Text = "Добавление/Редактирование груза";
            this.Size = new Size(500, 300);
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

            panel.Controls.Add(new Label { Text = "Название груза:", Location = new Point(10, y), AutoSize = true });
            _nameTextBox = new TextBox { Location = new Point(150, y), Size = new Size(300, 23) };
            panel.Controls.Add(_nameTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Единица измерения:", Location = new Point(10, y), AutoSize = true });
            _unitTextBox = new TextBox { Location = new Point(150, y), Size = new Size(300, 23) };
            panel.Controls.Add(_unitTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Количество:", Location = new Point(10, y), AutoSize = true });
            _quantityNumeric = new NumericUpDown { Location = new Point(150, y), Size = new Size(200, 23), Minimum = 0, Maximum = 1000000, DecimalPlaces = 2 };
            panel.Controls.Add(_quantityNumeric);
            y += 35;

            panel.Controls.Add(new Label { Text = "Общий вес (кг):", Location = new Point(10, y), AutoSize = true });
            _weightNumeric = new NumericUpDown { Location = new Point(150, y), Size = new Size(200, 23), Minimum = 0, Maximum = 1000000, DecimalPlaces = 2 };
            panel.Controls.Add(_weightNumeric);
            y += 35;

            panel.Controls.Add(new Label { Text = "Страховая стоимость:", Location = new Point(10, y), AutoSize = true });
            _insuranceValueNumeric = new NumericUpDown { Location = new Point(150, y), Size = new Size(200, 23), Minimum = 0, Maximum = 100000000, DecimalPlaces = 2 };
            panel.Controls.Add(_insuranceValueNumeric);
            y += 40;

            _saveButton = new Button { Text = "Сохранить", Location = new Point(150, y), Size = new Size(100, 30), DialogResult = DialogResult.OK };
            _saveButton.Click += SaveButton_Click;
            panel.Controls.Add(_saveButton);

            _cancelButton = new Button { Text = "Отмена", Location = new Point(260, y), Size = new Size(100, 30), DialogResult = DialogResult.Cancel };
            panel.Controls.Add(_cancelButton);

            this.Controls.Add(panel);
        }

        private void LoadCargoItemData()
        {
            if (CargoItem == null) return;

            _nameTextBox.Text = CargoItem.Name;
            _unitTextBox.Text = CargoItem.Unit;
            _quantityNumeric.Value = CargoItem.Quantity;
            _weightNumeric.Value = CargoItem.TotalWeight;
            _insuranceValueNumeric.Value = CargoItem.InsuranceValue;
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
            {
                MessageBox.Show("Введите название груза", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (CargoItem == null) return;

            CargoItem.Name = _nameTextBox.Text;
            CargoItem.Unit = _unitTextBox.Text;
            CargoItem.Quantity = _quantityNumeric.Value;
            CargoItem.TotalWeight = _weightNumeric.Value;
            CargoItem.InsuranceValue = _insuranceValueNumeric.Value;

            this.DialogResult = DialogResult.OK;
        }
    }
}


