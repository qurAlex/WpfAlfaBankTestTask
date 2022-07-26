﻿using System;
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
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace WpfAlfaBankTestTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Article> articles = new List<Article>();
        private byte numberOfArticle=0;
        private bool articlesWrited = false;
        public MainWindow()
        {
            InitializeComponent();
            ListXml.Items.Add("Выберете способ чтения файла");
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonSysXml_Click(object sender, RoutedEventArgs e)
        {
            ListXml.Items.Clear();
            articles.Clear();
            numberOfArticle = 0;

            XmlDocument doc = new XmlDocument();
            doc.Load("data.xml");

            foreach (XmlNode node in doc.DocumentElement)
            {
                string title = string.Format(node["title"].InnerText);
                string link = string.Format(node["link"].InnerText);
                string description = string.Format(node["description"].InnerText);
                string category = string.Format(node["category"].InnerText);
                DateTime pubDate = DateTime.Parse(node["pubDate"].InnerText);
                articles.Add( new Article(title, link, description, category, pubDate));
                ListXml.Items.Add(title + "\r\n" + link + "\r\n" + description + "\r\n" +
                   category + "\r\n" + pubDate.DayOfWeek.ToString() + "," + pubDate.ToString());
                numberOfArticle++;
            }
            articlesWrited = true;
            TextBlockNotification.Text = "файл считан с помощью System.Xml";
        }

        private void ButtonRegex_Click(object sender, RoutedEventArgs e)
        {
            ListXml.Items.Clear();
            articles.Clear();
            numberOfArticle = 0;

            Regex regex = new Regex(@">(.*?)<");
            string path = "data.xml";
            string lines = null;
            using (StreamReader reader = new StreamReader(path, false))
            {
                List<string> linesMathes = new List<string>();
                string line = null;
                do
                {
                    line = reader.ReadLine();
                    if (line != null)
                        if (regex.Match(line).Value.Length > 0)
                        {
                            linesMathes.Add(regex.Match(line).Value.Substring(1, regex.Match(line).Value.Length - 2));
                        }
                } while (line != null);
                for ( int i = 0; i < (linesMathes.Count)-4; i+=5)
                {
                    string title = linesMathes[i];
                    string link = linesMathes[i + 1];
                    string description = linesMathes[i + 2];
                    string category = linesMathes[i + 3];
                    DateTime pubDate = DateTime.Parse(linesMathes[i + 4]);
                    articles.Add( new Article(title, link, description, category, pubDate));
                    ListXml.Items.Add(title + "\r\n" + link + "\r\n" + description + "\r\n" +
                       category + "\r\n" + pubDate.DayOfWeek.ToString() + "," + pubDate.ToString());
                    numberOfArticle++;
                }
                articlesWrited = true;
                TextBlockNotification.Text = "файл считан с помощью Regex";
            }
        } 
        
        private void ButtonExcel_Click(object sender, RoutedEventArgs e)
        {
            if (articlesWrited)
            {
                ExcelOut excelOut = new ExcelOut();
                excelOut.Writer(articles);
                TextBlockNotification.Text = "данные записаны в Excel файл";
            }
            else
            {
                TextBlockNotification.Text = "выберите способ чтения файла";
            }
        }

        private void ButtonWord_Click(object sender, RoutedEventArgs e)
        {
            if (articlesWrited)
            {
                WordOut wordOut = new WordOut();
                wordOut.Writer(articles);

                TextBlockNotification.Text = "данные записаны в Word файл";
            }
            else
            {
                TextBlockNotification.Text = "выберите способ чтения файла";
            }
        }

        private  void ButtonTxt_Click(object sender, RoutedEventArgs e)
        {
            if (articlesWrited)
            {
                string path = "Articles.txt";
                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    for (int i = 0; i < articles.Count; i++)
                    {
                        writer.WriteLine(articles[i].Title());
                        writer.WriteLine(articles[i].Link());
                        writer.WriteLine(articles[i].Description());
                        writer.WriteLine(articles[i].Category());
                        writer.WriteLine(articles[i].PubDate().DayOfWeek.ToString() + ", " + articles[i].PubDate().ToString());
                        writer.WriteLine();
                    }
                    writer.Close();
                    TextBlockNotification.Text = "данные записаны в текстовый файл";
                }
            }
            else 
            {
                TextBlockNotification.Text = "выберите способ чтения файла"; 
            }
        }
    }
   

    
}
