using System;
using System.Drawing;
using System.Windows.Forms;
using Gruzoperevozki.Models;

namespace Gruzoperevozki.Forms
{
    public partial class DriverEditForm : Form
    {
        public Driver? Driver { get; private set; }
        private TextBox _fullNameTextBox;
        private TextBox _employeeNumberTextBox;
        private NumericUpDown _birthYearNumeric;
        private NumericUpDown _workExperienceNumeric;
        private TextBox _categoryTextBox;
        private TextBox _classTextBox;
        private Button _saveButton;
        private Button _cancelButton;

        public DriverEditForm(Driver? driver = null)
        {
            Driver = driver ?? new Driver();
            InitializeComponent();
            LoadDriverData();
        }

        private void InitializeComponent()
        {
            this.Text = Driver?.Id != null ? "Редактирование водителя" : "Добавление водителя";
            this.Size = new Size(500, 350);
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

            panel.Controls.Add(new Label { Text = "ФИО:", Location = new Point(10, y), AutoSize = true });
            _fullNameTextBox = new TextBox { Location = new Point(150, y), Size = new Size(300, 23), Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top };
            panel.Controls.Add(_fullNameTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Табельный номер:", Location = new Point(10, y), AutoSize = true });
            _employeeNumberTextBox = new TextBox { Location = new Point(150, y), Size = new Size(300, 23), Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top };
            panel.Controls.Add(_employeeNumberTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Год рождения:", Location = new Point(10, y), AutoSize = true });
            _birthYearNumeric = new NumericUpDown { Location = new Point(150, y), Size = new Size(200, 23), Minimum = 1900, Maximum = DateTime.Now.Year, Value = 1980 };
            panel.Controls.Add(_birthYearNumeric);
            y += 35;

            panel.Controls.Add(new Label { Text = "Стаж работы (лет):", Location = new Point(10, y), AutoSize = true });
            _workExperienceNumeric = new NumericUpDown { Location = new Point(150, y), Size = new Size(200, 23), Minimum = 0, Maximum = 100, Value = 0 };
            panel.Controls.Add(_workExperienceNumeric);
            y += 35;

            panel.Controls.Add(new Label { Text = "Категория:", Location = new Point(10, y), AutoSize = true });
            _categoryTextBox = new TextBox { Location = new Point(150, y), Size = new Size(300, 23), Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top };
            panel.Controls.Add(_categoryTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Классность:", Location = new Point(10, y), AutoSize = true });
            _classTextBox = new TextBox { Location = new Point(150, y), Size = new Size(300, 23), Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top };
            panel.Controls.Add(_classTextBox);
            y += 40;

            _saveButton = new Button { Text = "Сохранить", Location = new Point(150, y), Size = new Size(100, 30), DialogResult = DialogResult.OK };
            _saveButton.Click += SaveButton_Click;
            panel.Controls.Add(_saveButton);

            _cancelButton = new Button { Text = "Отмена", Location = new Point(260, y), Size = new Size(100, 30), DialogResult = DialogResult.Cancel };
            panel.Controls.Add(_cancelButton);

            this.Controls.Add(panel);
        }

        private void LoadDriverData()
        {
            if (Driver == null) return;

            _fullNameTextBox.Text = Driver.FullName;
            _employeeNumberTextBox.Text = Driver.EmployeeNumber;
            
            // Безопасная установка значений в NumericUpDown
            if (Driver.BirthYear >= _birthYearNumeric.Minimum && Driver.BirthYear <= _birthYearNumeric.Maximum)
                _birthYearNumeric.Value = Driver.BirthYear;
            else
                _birthYearNumeric.Value = 1980;
            
            if (Driver.WorkExperience >= _workExperienceNumeric.Minimum && Driver.WorkExperience <= _workExperienceNumeric.Maximum)
                _workExperienceNumeric.Value = Driver.WorkExperience;
            else
                _workExperienceNumeric.Value = _workExperienceNumeric.Minimum;
            
            _categoryTextBox.Text = Driver.Category;
            _classTextBox.Text = Driver.Class;
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_fullNameTextBox.Text))
            {
                MessageBox.Show("Введите ФИО водителя", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(_employeeNumberTextBox.Text))
            {
                MessageBox.Show("Введите табельный номер", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Driver == null) return;

            Driver.FullName = _fullNameTextBox.Text;
            Driver.EmployeeNumber = _employeeNumberTextBox.Text;
            Driver.BirthYear = (int)_birthYearNumeric.Value;
            Driver.WorkExperience = (int)_workExperienceNumeric.Value;
            Driver.Category = _categoryTextBox.Text;
            Driver.Class = _classTextBox.Text;

            this.DialogResult = DialogResult.OK;
        }
    }
}

