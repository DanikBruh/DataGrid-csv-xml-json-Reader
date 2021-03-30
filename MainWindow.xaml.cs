using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Xml;
using System.Web.Script.Serialization;

namespace Binding
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string CSVPATH = Directory.GetParent(@"..") + @"\MOCK_DATA.csv";
        private string XMLPATH = Directory.GetParent(@"..") + @"\dataset.xml";
        private string JSONPATH = Directory.GetParent(@"..") + @"\MOCK_DATA.json";

        private List<Student> _studentsList;
        private static int _pageNumber;
        private const int STUDENTSCOUNT = 10;

        public MainWindow()
        {
            InitializeComponent();
            _studentsList = new List<Student>();

            // CSV
            readCSVFile(CSVPATH, _studentsList);
            // XML
            //readXMLFile(XMLPATH, _studentsList);
            // JSON
            //readJSONFile(JSONPATH, _studentsList);

            dgStudents.IsHitTestVisible = false;
            _pageNumber = 0;
            dgStudents.ItemsSource = _studentsList.Take(STUDENTSCOUNT);
        }

        // Считывает CSV файл и заполняет список students объектами класса Student.
        private static void readCSVFile(string path, List<Student> students)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while(sr.EndOfStream == false)
                    {
                        List<string> info = sr.ReadLine().Split(',').ToList();
                        students.Add(new Student(int.Parse(info[0]), info[1], info[2], info[3], info[4]));
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        // Считывает XML файл и заполняет список students объектами класса Student.
        public static void readXMLFile(string path, List<Student> students)
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(path);
                // получим корневой элемент
                XmlElement xRoot = xDoc.DocumentElement;
                // обход всех узлов в корневом элементе
                foreach (XmlNode xnode in xRoot)
                {
                    // обходим все дочерние узлы элемента record
                    string id, first_name, last_name, email, ip_address;
                    id = first_name = last_name = email = ip_address = string.Empty;
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        if (childnode.Name == "id")
                            id = childnode.InnerText;
                        if (childnode.Name == "first_name")
                            first_name = childnode.InnerText;
                        if (childnode.Name == "last_name")
                            last_name = childnode.InnerText;
                        if (childnode.Name == "email")
                            email = childnode.InnerText;
                        if (childnode.Name == "ip_address")
                            ip_address = childnode.InnerText;
                    }
                    students.Add(new Student(int.Parse(id), first_name, last_name, email, ip_address));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        // Считывает JSON файл и заполняет список students объектами класса Student.
        public static void readJSONFile(string path, List<Student> students)
        {
            try
            {
                string json = File.ReadAllText(path);
                List<Dictionary<string, string>> json_Dictionary_list = (new JavaScriptSerializer()).Deserialize<List<Dictionary<string, string>>>(json);
                foreach(Dictionary<string, string> jsonObj in json_Dictionary_list)
                {
                    students.Add(new Student(int.Parse(jsonObj["id"]), jsonObj["first_name"], jsonObj["last_name"], jsonObj["email"], jsonObj["ip_address"]));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        // Обработчик нажатия на кнопку "Предыдущая страница"
        private void Prev_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_pageNumber - 1 >= 0) _pageNumber--;
                else throw new Exception("Выход за пределы массива!");
                var newArray = _studentsList.Skip(STUDENTSCOUNT * _pageNumber).Take(STUDENTSCOUNT);
                dgStudents.ItemsSource = newArray;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        // Обработчик нажатия на кнопку "Следующая страница"
        private void Next_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_pageNumber + 1 < _studentsList.Count / STUDENTSCOUNT) _pageNumber++;
                else throw new Exception("Выход за пределы массива!");
                var newArray = _studentsList.Skip(STUDENTSCOUNT * _pageNumber).Take(STUDENTSCOUNT);
                dgStudents.ItemsSource = newArray;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }
    }

    // Класс Студент.
    public class Student
    {
        public int ID { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Email { get; set; }
        public string IP_address { get; set; }
        public Student(int id, string fname, string lname, string email, string ip)
        {
            this.ID = id;
            this.First_name = fname;
            this.Last_name = lname;
            this.Email = email;
            this.IP_address = ip;
        }
    }
}
