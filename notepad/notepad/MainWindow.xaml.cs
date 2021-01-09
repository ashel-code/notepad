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

using Microsoft.Win32;
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
        public double kegel { get; set; }
        public int font { get; set; }
        public bool isEncrypted { get; set; }
        public string password { get; set; }

        public TextDoc (string text, double kegel, int font, bool isEncrypted)
        {
            this.text = text;
            this.kegel = kegel;
            this.font = font;
            this.isEncrypted = isEncrypted;
        }

        public TextDoc(string text, double kegel, int font, bool isEncrypted, string password)
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
        bool isEncrypted;
        double kegel = 15;
        int font = 1;
        string password = "";
        bool isChanged = false;
        string txt;
        double tmp;
        TextDoc doc;
        SaveFileDialog saveFileDialog;
        BinaryFormatter formatter = new BinaryFormatter();
        OpenFileDialog openFileDialog;
        string filename;
        Nullable<bool> resultOpenDialog;
        Window1 passwordWindow;

        public MainWindow()
        {
            InitializeComponent();
            isEncryptedLabel.Content = "не зашифрован";
            isChanged = false;
            firstFont.IsChecked = true;
        }

        private void saveFunc()
        {
            isChanged = false;
            saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "Текстовый документ (*.notepad)|*.notepad";

            if (saveFileDialog.ShowDialog() == true)
            {
                txt = new TextRange(mainRichTextBox.Document.ContentStart, mainRichTextBox.Document.ContentEnd).Text;
                if (password != "")
                {
                    doc = new TextDoc(txt, kegel, font, true, password);
                    using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate))
                    {
                        formatter.Serialize(fs, doc);
                    }
                    
                }
                else
                {
                    doc = new TextDoc(txt, kegel, font, false);
                    using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate))
                    {
                        formatter.Serialize(fs, doc);
                    }
                }

            }
        }

        private void kegelChanged()
        {
            if (kegelTextBox.Text != "" && kegelTextBox.Text != "0")
            {

                try
                {
                    if (Convert.ToInt32(kegelTextBox.Text) < 100)
                    {
                        kegel = Convert.ToInt32(kegelTextBox.Text);
                        mainRichTextBox.FontSize = kegel;
                    }
                    else
                    {
                        kegelTextBox.Text = Convert.ToString(kegel);
                        mainRichTextBox.FontSize = kegel;
                    }

                }
                catch
                {
                    kegelTextBox.Text = Convert.ToString(kegel);
                    mainRichTextBox.FontSize = kegel;
                }
            }
            else
            {
                kegelTextBox.Text = "1";
                mainRichTextBox.FontSize = 1;
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)                            // сохранить
        {
            saveFunc();
        }
         
        private void openButton_Click(object sender, RoutedEventArgs e)                            // откыть
        {
            openFileDialog = new OpenFileDialog(); // 
            // dialog.Filter = "Text documents (*.txt)|*.txt|All files (*.*)|*.*"; // шаблон записи нескольких расширений
            openFileDialog.Filter = "Текстовый документ (*.notepad)|*.notepad"; // расширения файла
            openFileDialog.FilterIndex = 0; // индекс выбранного варианта расширения по умолчанию

            resultOpenDialog = openFileDialog.ShowDialog();

            if (resultOpenDialog == true)
            {
                // Open document
                filename = openFileDialog.FileName; // установка переменной значения пути
                // mainTextBox.Text = filename; // проверка верности пути

                using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.OpenOrCreate))
                {
                    doc = (TextDoc)formatter.Deserialize(fs);
                    if (doc.isEncrypted)
                    {
                        while (true)
                        {
                            try
                            {
                                passwordWindow = new Window1();
                                passwordWindow.ShowDialog();
                                password = passwordWindow.Password;
                                isEncrypted = true;
                            }
                            catch
                            {
                                // password = "";
                                isEncrypted = false;
                            }

                            if (isEncrypted)
                            {
                                if (password == doc.password)
                                {
                                    //mainTextBox.Text = doc.text;
                                    mainRichTextBox.Document.Blocks.Clear();
                                    mainRichTextBox.Document.Blocks.Add(new Paragraph(new Run(doc.text)));
                                    mainRichTextBox.FontSize = doc.kegel;
                                    switch (doc.font)
                                    {
                                        case 1:
                                            mainRichTextBox.FontFamily = new FontFamily("Segoe UI");
                                            firstFont.IsChecked = true;
                                            break;
                                        case 2:
                                            mainRichTextBox.FontFamily = new FontFamily("Ink Free");
                                            secondFont.IsChecked = true;
                                            break;
                                        case 3:
                                            mainRichTextBox.FontFamily = new FontFamily("Source Code Pro Light");
                                            thirdFont.IsChecked = true;
                                            break;
                                    }
                                    isEncryptedLabel.Content = "зашифрован";
                                    encryptButton.Content = "дешивровать";
                                    break;

                                }
                                else
                                {
                                    if (passwordWindow.IsCanceled)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Введен неверный пароль!");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        isEncryptedLabel.Content = "не зашифрован";
                        encryptButton.Content = "шивровать";
                        mainRichTextBox.Document.Blocks.Clear();
                        mainRichTextBox.Document.Blocks.Add(new Paragraph(new Run(doc.text)));
                        mainRichTextBox.FontSize = doc.kegel;
                        switch (doc.font)
                        {
                            case 1:
                                mainRichTextBox.FontFamily = new FontFamily("Segoe UI");
                                break;
                            case 2:
                                mainRichTextBox.FontFamily = new FontFamily("Ink Free");
                                break;
                            case 3:
                                mainRichTextBox.FontFamily = new FontFamily("Source Code Pro Light");
                                break;
                        }
                    }
                }

            }
            isChanged = false;
        }

        private void encryptButton_Click(object sender, RoutedEventArgs e)                         // зашифровать
        {
            if (password == "")
            {
                passwordWindow = new Window1();
                passwordWindow.ShowDialog();
                if (!passwordWindow.IsCanceled)
                {
                    password = passwordWindow.Password;
                    isEncryptedLabel.Content = "зашифрован";
                    encryptButton.Content = "дешивровать";
                }
            }
            else
            {
                password = "";
                isEncryptedLabel.Content = "не зашифрован";
                encryptButton.Content = "шивровать";
            }
        }

        private void kegelTextBox_TextChanged(object sender, TextChangedEventArgs e)               // изменение текста
        {
            kegelChanged();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isChanged)
            {
                MessageBoxResult result = MessageBox.Show("Вы желаете сохранить документ перед закрытием?", "Сохранить документ?", MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        saveFunc();
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void mainRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            isChanged = true;
            // MessageBox.Show("");
        }

        private void kegelPlus_Click(object sender, RoutedEventArgs e)
        {
            tmp = Convert.ToDouble(kegelTextBox.Text);
            tmp++;
            kegelTextBox.Text = Convert.ToString(tmp);
            kegelChanged();
        }

        private void kegelMinus_Click(object sender, RoutedEventArgs e)
        {
            tmp = Convert.ToDouble(kegelTextBox.Text);
            tmp--;
            kegelTextBox.Text = Convert.ToString(tmp);
            kegelChanged();
        }

        private void firstFont_Checked(object sender, RoutedEventArgs e)
        {
            font = 1;
            mainRichTextBox.FontFamily = new FontFamily("Segoe UI");
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            font = 2;
            mainRichTextBox.FontFamily = new FontFamily("Ink Free");
        }

        private void thirdFont_Checked(object sender, RoutedEventArgs e)
        {
            font = 3;
            mainRichTextBox.FontFamily = new FontFamily("Source Code Pro Light");
        }
    }
}
