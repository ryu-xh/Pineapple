using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
            Zh,
            Unknown
        }

        string zhList = "偁偄偆偊傫偍偽傃傇傋傏傖傘偉傚偪偩偱偂偳偅傆偀偋偑偓偖偘偛偼傂傊傎偠偐偒偔偗偙傑傒傓傔傕側偵偸偹偺傁傄傉傌傐傜傝傞傟傠偝偡偣偦偟偨偰偲償傢傪傗備傛偞偫偢偤偧偮僈僎僊僑僌";
        private Lang DetectJAKOString(string str)
        {
            foreach (char chr in str)
            {
                if (chr >= '가' && chr < '힣')
                {
                    return Lang.Ko;
                }

                if (zhList.IndexOf(chr) >= 0)
                {
                    return Lang.Zh;
                }

                if ((chr >= 0x3040 && chr <= 0x30FF) || (chr >= 0x4E00 && chr<= 0x9FBF))
                {
                    return Lang.Ja;
                }
            }

            return Lang.Unknown;
        }

        Encoding ja = Encoding.GetEncoding(932);
        Encoding ko = Encoding.GetEncoding(949);
        Encoding zh = Encoding.GetEncoding("gb18030");
        private void RenameFile(string originDirectory, string file)
        {
            var fileName = file.Split('\\');

            string changedFileName = "";

            switch (DetectJAKOString(fileName[fileName.Length - 1]))
            {
                case Lang.Ja:
                    changedFileName = Convert(fileName[fileName.Length - 1], ko, ja);
                    break;
                case Lang.Ko:
                    changedFileName = Convert(fileName[fileName.Length - 1], ja, ko);
                    break;
                case Lang.Zh:
                    changedFileName = Convert(fileName[fileName.Length - 1], ja, zh);
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
            EncodingInfo[] encods = Encoding.GetEncodings();

            Encoding destEnc = Encoding.UTF8;

            Encoding enc = formatTo;
            byte[] sorceBytes = enc.GetBytes(str);
            byte[] encBytes = Encoding.Convert(formatFrom, destEnc, sorceBytes);

            return destEnc.GetString(encBytes);
        }

        private Encoding DectectEncoding(string file)
        {
            string jaStrings = "あいうえんおばびぶべぼゃゅぇょちだでぃどぅふぁぉがぎぐげごはひへほじかきくけこまみむめもなにぬねのぱぴぷぺぽらりるれろさすせそしたてとわをやゆよざぢずぜぞつ";
            using (StreamReader sr = new StreamReader(file, Encoding.UTF8, true))
            {
                string data = sr.ReadToEnd();
                sr.Close();
                for (int i = 0; i < jaStrings.Length; i++)
                {
                    if (data.Contains(jaStrings[i]))
                        return Encoding.UTF8;
                }
            }

            using (StreamReader sr = new StreamReader(file, ko))
            {
                string data = sr.ReadToEnd();
                sr.Close();
                for (int i = 0; i< jaStrings.Length; i++)
                {
                    if (data.Contains(jaStrings[i]))
                        return ko;
                }
            }

            using (StreamReader sr = new StreamReader(file, zh))
            {
                string data = sr.ReadToEnd();
                sr.Close();
                for (int i = 0; i < jaStrings.Length; i++)
                {
                    if (data.Contains(jaStrings[i]))
                        return zh;
                }
            }

            return ja;

        }

        private void Progress(string[] folders)
        {
            foreach (string folder in folders)
            {
                string path = folder;

                if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
                {
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
                } else
                {
                    if (path.EndsWith("\\oto.ini"))
                    {
                        Encoding enc = DectectEncoding(path);
                        string contents;
                        using (StreamReader sr = new StreamReader(path, enc, true))
                        {
                            enc = sr.CurrentEncoding;
                            contents = sr.ReadToEnd();
                            sr.Close();
                        }

                        using (StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding(932)))
                        {
                            sw.WriteLine(contents);
                            sw.Close();
                            sw.Dispose();
                        }

                        Progress_Text.Text = "Calibration complete oto.ini";
                    }
                }


                ShowAd();
            }
        }

        private void ShowAd()
        {
            System.Diagnostics.Process.Start("https://pbs.twimg.com/media/FSLqrg7agAEEyT8?format=png&name=large");

            Progress_Text.Text = "완료";
        }

        private void Grid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            var folders = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

            Progress(folders);
        }
    }
}
