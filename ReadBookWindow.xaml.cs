using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using PdfViewerNet;
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

namespace _300958687_kim__Lab2
{
    /// <summary>
    /// Interaction logic for ReadBookWindow.xaml
    /// </summary>
    public partial class ReadBookWindow : Window
    {
        private DynamoDBContext context;
        private AmazonDynamoDBClient client;
        private PdfViewer pdfViewer;
        private string filepath;
        private Book book;
        private User user;
        private int currentpage;
        private PdfViewer viewer;
    
        public ReadBookWindow(string filepath,Book book,User user)
        {
            client = new AmazonDynamoDBClient();
            InitializeComponent();
            this.filepath = filepath;
            this.book = book;
            this.user = user;
            pdfViewer = new PdfViewer();
            windowsHost.Child = pdfViewer;
            var viewer = windowsHost.Child as PdfViewer;
            if (viewer != null)
            {
                viewer.BackColor = System.Drawing.SystemColors.Control;
                viewer.BorderColor = System.Drawing.Color.FromArgb(203, 203, 203);
            }
        }
        private void readbookwindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            windowsHost.Child = pdfViewer;
            var viewer = windowsHost.Child as PdfViewer;
            viewer.SetWPFParentSize((int)this.Width, (int)this.Height);
        }

        private void readbookwindow_LocationChanged(object sender, EventArgs e)
        {
            windowsHost.Child = pdfViewer;
            var viewer = windowsHost.Child as PdfViewer;
            viewer.SetWPFParentLocation((int)this.Left, (int)this.Top);
        }

        private void readbookwindow_Loaded(object sender, RoutedEventArgs e)
        {
            windowsHost.Child = pdfViewer;
            viewer = windowsHost.Child as PdfViewer;
            viewer.SetWPFParentSize((int)this.Width, (int)this.Height);
            viewer.SetWPFParentLocation((int)this.Left, (int)this.Top);
            viewer.OpenDocument(filepath);
            viewer.GoToPage(book.CurrentPage);
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            currentpage = viewer.CurrentPageNumber;
            context = new DynamoDBContext(client);
            User retrieved = context.Load<User>(user.Username);
            retrieved.Books.Where(b => b.ISBN == book.ISBN)
                           .Select(b => b)
                           .First().CurrentPage = currentpage;
            context.Save<User>(retrieved);
            UserBookshelfWindow ubsw = new UserBookshelfWindow(retrieved);
            ubsw.Show();
            base.OnClosing(e);
        }
    }
}
