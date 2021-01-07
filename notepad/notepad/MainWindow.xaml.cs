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
    class TextDoc // объект текстового документа
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
            isEncryptedLabel.Content = "не зашифрован";
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)                       // сохранить
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
         
        private void openButton_Click(object sender, RoutedEventArgs e)                       // откыть
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
                    if (doc.isEncrypted)
                    {
                        while (true)
                        {
                            try
                            {
                                Window1 passwordWindow = new Window1();
                                passwordWindow.ShowDialog();
                                password = passwordWindow.Password;
                            }
                            catch
                            {
                                password = "";
                            }

                            if (password != "")
                            {
                                if (password == doc.password)
                                {
                                    mainTextBox.Text = doc.text;
                                    mainTextBox.FontSize = doc.kegel;
                                    switch (doc.font)
                                    {
                                        case 1:
                                            mainTextBox.FontFamily = new FontFamily("Segoe UI");
                                            break;
                                        case 2:
                                            mainTextBox.FontFamily = new FontFamily("Ink Free");
                                            break;
                                        case 3:
                                            mainTextBox.FontFamily = new FontFamily("Source Code Pro Light");
                                            break;
                                    }
                                    isEncryptedLabel.Content = "зашифрован";
                                    break;

                                }
                                else
                                {
                                    mainTextBox.Text = "пароль неверен";
                                    mainTextBox.FontSize = 15;
                                }
                            }
                        }
                        password = "";
                    }
                    else
                    {
                        isEncryptedLabel.Content = "не зашифрован";
                    }
                }

            }
        }

        private void encryptButton_Click(object sender, RoutedEventArgs e)                    // зашифровать
        {
            if (password == "")
            {
                Window1 passwordWindow = new Window1();
                passwordWindow.ShowDialog();
                if (!passwordWindow.IsCanceled)
                {
                    password = passwordWindow.Password;
                    isEncryptedLabel.Content = "зашифрован";
                }
            }
            else
            {
                password = "";
                isEncryptedLabel.Content = "не зашифрован";
            }
        }

        private void kegelTextBox_TextChanged(object sender, TextChangedEventArgs e)          // изменение текста
        {
            if (kegelTextBox.Text != "" && kegelTextBox.Text != "0")
            {
                
                try
                {
                    if (Convert.ToInt32(kegelTextBox.Text) < 100)
                    {
                        kegel = Convert.ToInt32(kegelTextBox.Text);
                        mainTextBox.FontSize = kegel;
                    }
                    else
                    {
                        kegelTextBox.Text = Convert.ToString(kegel);
                        mainTextBox.FontSize = kegel;
                    }

                }
                catch
                {
                    kegelTextBox.Text = Convert.ToString(kegel);
                    mainTextBox.FontSize = kegel;
                }
            }
            else
            {
                kegelTextBox.Text = "1";
                mainTextBox.FontSize = 1;
            }
        }

        private void firstFont_Click(object sender, RoutedEventArgs e)                        // установка первого стиля текста
        {
            font = 1;
            mainTextBox.FontFamily = new FontFamily("Segoe UI");
        }

        private void secondFont_Click(object sender, RoutedEventArgs e)                       // установка втрого стиля текста
        {
            font = 2;
            mainTextBox.FontFamily = new FontFamily("Ink Free");
        }

        private void thirdFont_Click(object sender, RoutedEventArgs e)                        // установка третьего стиля текста
        {
            font = 3;
            mainTextBox.FontFamily = new FontFamily("Source Code Pro Light");
        }
    }
}
