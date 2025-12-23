using System;
using System.Windows.Forms;
using Gruzoperevozki.Data;
using Gruzoperevozki.Forms;

namespace Gruzoperevozki
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            try
            {
                Application.Run(new MainForm());
            }
            finally
            {
                // Гарантируем сохранение данных при закрытии приложения
                DataStorage.Instance.SaveData();
            }
        }
    }
}

