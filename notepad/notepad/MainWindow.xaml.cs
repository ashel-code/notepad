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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO; // библиотека для работы с файлами
using System.Runtime.Serialization.Formatters.Binary; // библиотека для сериализации

namespace notepad
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    [Serializable]
    class TextDoc
    {
        public string text { get; set; }
        public int kegel { get; set; }
        public int font { get; set; }
        public bool isEncrypted { get; set; }
        public string password { get; set; }

        public TextDoc (string text, int kegel, int font, bool isEncrypted)
        {
            this.text = text;
            this.kegel = kegel;
            this.font = font;
            this.isEncrypted = isEncrypted;
        }

        public TextDoc(string text, int kegel, int font, bool isEncrypted, string password)
        {
            this.text = text;
            this.kegel = kegel;
            this.font = font;
            this.isEncrypted = isEncrypted;
            this.password = password;
        }
    }

    public partial class MainWindow : Window
    {
        int kegel = 15;
        int font = 0;
        string password = "";
        BinaryFormatter formatter = new BinaryFormatter();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e) // сохранить
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();

            saveFileDialog.Filter = "Текстовый документ (*.notepad)|*.notepad";

            if (saveFileDialog.ShowDialog() == true)
            {
                if (password != "")
                {
                    TextDoc doc = new TextDoc(mainTextBox.Text, kegel, font, true, password); 
                    using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate))
                    {
                        formatter.Serialize(fs, doc);
                    }
                }
                else
                {
                    TextDoc doc = new TextDoc(mainTextBox.Text, kegel, font, false);
                    using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate))
                    {
                        formatter.Serialize(fs, doc);
                    }
                }

            }
        }

        private void openButton_Click(object sender, RoutedEventArgs e) // откыть
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog(); // 
            // dialog.Filter = "Text documents (*.txt)|*.txt|All files (*.*)|*.*"; // шаблон записи нескольких расширений
            openFileDialog.Filter = "Текстовый документ (*.notepad)|*.notepad"; // расширения файла
            openFileDialog.FilterIndex = 0; // индекс выбранного варианта расширения по умолчанию

            Nullable<bool> result = openFileDialog.ShowDialog(); 

            if (result == true)
            {
                // Open document
                string filename = openFileDialog.FileName; // установка переменной значения пути
                mainTextBox.Text = filename; // проверка верности пути

                using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.OpenOrCreate))
                {
                    TextDoc doc = (TextDoc)formatter.Deserialize(fs);
                    mainTextBox.Text = doc.text;
                    mainTextBox.FontSize = doc.kegel;
                }

            }
        }

        private void encryptButton_Click(object sender, RoutedEventArgs e) // зашифровать
        {
            while (true)
            {
                
                
                    Window1 passwordWindow = new Window1();
                    passwordWindow.ShowDialog();
                    if (!passwordWindow.IsCanceled)
                    {
                        password = passwordWindow.Password;
                    }
                    break;
                
            }
        }

        private void kegelTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (kegelTextBox.Text != "" && kegelTextBox.Text != "0")
            {
                
                try
                {
                    if (Convert.ToInt32(kegelTextBox.Text) < 100)
                    {
                        kegel = Convert.ToInt32(kegelTextBox.Text);
                        
                    }
                    else
                    {
                        kegelTextBox.Text = Convert.ToString(kegel);
                    }

                }
                catch
                {
                    kegelTextBox.Text = Convert.ToString(kegel);
                }
            }
            else
            {
                kegelTextBox.Text = "1";
            }
        }
    }
}
