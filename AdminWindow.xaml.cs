using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Win32;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _300958687_kim__Lab2
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private DynamoDBContext context;
        private AmazonDynamoDBClient client;
        private string isbn;
        private string title;
        private string author;
        private string pdfPath;
        private readonly string bucketName = "minjikim306bookrepo";
        public AdminWindow()
        {
            client = new AmazonDynamoDBClient();
            InitializeComponent();
            loadBookRepo();
            filepathTxt.IsEnabled = false;
        }

        private void loadBookRepo()
        {
            context = new DynamoDBContext(client);
            var condition = new List<ScanCondition>();
            //var enubooks = context.Scan<Book>();
            List<Book> allBooks = context.Scan<Book>().ToList();
            //List<Book> books = new List<Book>();
            
            if (allBooks != null)
            {
                bookRepoLst.ItemsSource = allBooks;
            }
        }

        private void brwsBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "Pdf Files|*.pdf";
            if (op.ShowDialog() == true)
            {
                pdfPath = op.FileName;
                filepathTxt.Text = pdfPath;
                filepathTxt.IsEnabled = true;               
            }
        }

        private void uploadBtn_Click(object sender, RoutedEventArgs e)
        {
            isbn = isbnTxt.Text;
            title = titleTxt.Text;
            author = authorTxt.Text;
            string fileTitle = System.IO.Path.GetFileName(pdfPath);
            Book newBook = new Book
            {
                ISBN = isbn,
                Title = title,
                Author = author,
                CurrentPage = 1,
                PDFFile = S3Link.Create(context, bucketName, fileTitle, Amazon.RegionEndpoint.USWest2)
            };
            context.Save<Book>(newBook);
            newBook.PDFFile.UploadFrom(pdfPath);

            loadBookRepo();
        }
    }
}
