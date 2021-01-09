using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace notepad
{
    /// <summary>
    public partial class Window1 : Window
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    {
        bool isButtonPressed = false;
        bool isCanceled = false;

        public bool IsCanceled {
            get { return isCanceled; }
        }

        public Window1()
        {
            InitializeComponent();
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            isButtonPressed = true;
            DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            isButtonPressed = true;
            isCanceled = true;
            Close();
        }

        public string Password
        {
            get { return passwordTextBox.Password; }
        }

        private void closeFunc(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isButtonPressed)
            {
                e.Cancel = true;
            }
        }
    }
}
