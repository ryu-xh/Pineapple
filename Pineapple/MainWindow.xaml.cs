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
            String[] args = App.mArgs;
            if (args != null)
            {

                Progress(args);
            }
        }

        enum Lang
        {
            Ko,
            Ja,
            Unknown
        }

        private Lang DetectJAKOString(string str)
        {
            foreach (char chr in str)
            {
                if (chr >= '가' && chr < '힣')
                {
                    return Lang.Ko;
                }

                if ((chr >= 0x3040 && chr <= 0x30FF) || (chr >= 0x4E00 && chr<= 0x9FBF))
                {
                    return Lang.Ja;
                }
            }

            return Lang.Unknown;
        }

        Encoding ja = Encoding.GetEncoding(949);
        Encoding ko = Encoding.GetEncoding(932);
        private void RenameFile(string originDirectory, string file)
        {
            var fileName = file.Split('\\');

            string changedFileName = "";

            switch (DetectJAKOString(fileName[fileName.Length - 1]))
            {
                case Lang.Ja:
                    changedFileName = Convert(fileName[fileName.Length - 1], ja, ko);
                    break;
                case Lang.Ko:
                    changedFileName = Convert(fileName[fileName.Length - 1], ko, ja);
                    break;
                case Lang.Unknown:
                    changedFileName = fileName[fileName.Length - 1];
                    break;
            }

            Console.WriteLine(originDirectory + "\\" + fileName[fileName.Length - 1] + " -> " + originDirectory + "\\" + changedFileName);
            Progress_Text.Text = fileName[fileName.Length - 1] + " -> " + changedFileName;


            if (System.IO.File.Exists(originDirectory + "\\" + fileName[fileName.Length - 1]))
            {
                System.IO.File.Move(originDirectory + "\\" + fileName[fileName.Length - 1], originDirectory + "\\" + changedFileName);
                Progress_Text.Text = fileName[fileName.Length - 1] + " -> " + changedFileName;
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

            Progress_Text.Text = "완료";
        }

        private void Progress(string[] folders)
        {
            foreach (string folder in folders)
            {
                string path = folder;

                string[] files = Directory.GetFiles(path, "*.*");

                foreach (var file in files)
                {
                    RenameFile(path, file);
                    System.Windows.Forms.Application.DoEvents();
                }

                DirectoryInfo di = new DirectoryInfo(path);
                DirectoryInfo[] dirs = di.GetDirectories("*.*", SearchOption.AllDirectories);

                foreach (DirectoryInfo d in dirs)
                {
                    string[] dirctoryFiles = Directory.GetFiles(d.FullName, "*.*");

                    foreach (var file in dirctoryFiles)
                    {
                        RenameFile(d.FullName, file);
                        System.Windows.Forms.Application.DoEvents();
                    }
                }

                ShowAd();
            }
        }

        private void Grid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            var folders = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

            Progress(folders);
        }
    }
}
