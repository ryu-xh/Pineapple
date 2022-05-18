using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pineapple
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string GetNewPath()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.SelectedPath;
            }

            return null;
        }

        private void RenameFile(string originDirectory, string file, Encoding formatFrom, Encoding formatTo)
        {
            var fileName = file.Split('\\');

            string changedFileName = Convert(fileName[fileName.Length - 1], formatFrom, formatTo);

            Console.WriteLine(originDirectory + "\\" + fileName[fileName.Length - 1] + " -> " + originDirectory + "\\" + changedFileName);


            if (System.IO.File.Exists(originDirectory + "\\" + fileName[fileName.Length - 1]))
            {
                System.IO.File.Move(originDirectory + "\\" + fileName[fileName.Length - 1], originDirectory + "\\" + changedFileName);
                Console.WriteLine(fileName[fileName.Length - 1] + " -> " + changedFileName);
            }

        }

        private string Convert(string str, Encoding formatFrom, Encoding formatTo)
        {
            Encoding shiftJIS = formatFrom;

            EncodingInfo[] encods = Encoding.GetEncodings();

            Encoding destEnc = Encoding.UTF8;

            Encoding enc = formatTo;
            byte[] sorceBytes = enc.GetBytes(str);
            byte[] encBytes = Encoding.Convert(shiftJIS, destEnc, sorceBytes);

            return destEnc.GetString(encBytes);
        }


        private void ShowAd()
        {
            System.Diagnostics.Process.Start("https://pbs.twimg.com/media/FSLqrg7agAEEyT8?format=png&name=large");
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Encoding formatFrom = Encoding.GetEncoding(949);
            Encoding formatTo = Encoding.GetEncoding(932);

            string path = GetNewPath();

            string[] files = Directory.GetFiles(path, "*.*");

            foreach (var file in files)
            {
                RenameFile(path, file, formatFrom, formatTo);
            }

            DirectoryInfo di = new DirectoryInfo(path);
            DirectoryInfo[] dirs = di.GetDirectories("*.*", SearchOption.AllDirectories);

            foreach (DirectoryInfo d in dirs)
            {
                string[] dirctoryFiles = Directory.GetFiles(d.FullName, "*.*");

                foreach (var file in dirctoryFiles)
                {
                    RenameFile(d.FullName, file, formatFrom, formatTo);
                }
            }

            ShowAd();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Encoding formatFrom = Encoding.GetEncoding(932);
            Encoding formatTo = Encoding.GetEncoding(949);

            string path = GetNewPath();

            string[] files = Directory.GetFiles(path, "*.*");

            foreach (var file in files)
            {
                RenameFile(path, file, formatFrom, formatTo);
            }

            DirectoryInfo di = new DirectoryInfo(path);
            DirectoryInfo[] dirs = di.GetDirectories("*.*", SearchOption.AllDirectories);

            foreach (DirectoryInfo d in dirs)
            {
                string[] dirctoryFiles = Directory.GetFiles(d.FullName, "*.*");

                foreach (var file in dirctoryFiles)
                {
                    RenameFile(d.FullName, file, formatFrom, formatTo);
                }
            }

            ShowAd();
        }
    }
}
