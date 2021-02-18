using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
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
    /// Interaction logic for BookStoreWindow.xaml
    /// </summary>
    public partial class BookStoreWindow : Window
    {
        private DynamoDBContext context;
        private AmazonDynamoDBClient client;
        private User user;
        //private UserBookshelfWindow userbookshelfwindow;
        public BookStoreWindow(User user)
        {
            client = new AmazonDynamoDBClient();
            InitializeComponent();
            //userbookshelfwindow = usrbshfwin;
            addBtn.Visibility = Visibility.Hidden;
            this.user = user;
            loadBookRepo();
        }
        private void loadBookRepo()
        {
            context = new DynamoDBContext(client);
            List<Book> allBooks = context.Scan<Book>().ToList();

            if (allBooks != null)
            {
                newbookLst.ItemsSource = allBooks;
            }
        }

        private void newbookLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            addBtn.Visibility = Visibility.Visible;
            Book selected = (Book)newbookLst.SelectedItem;
            string description = String.Format("Book Info >>\nTitle : {0}\nISBN : {1}\nAuthor : {2}", selected.Title, selected.ISBN, selected.Author);
            desciptionLbl.Content = description;
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            Book selected = (Book)newbookLst.SelectedItem;
            List<Book> userbook = new List<Book>();
            userbook.Add(selected);
            User retrieved = context.Load<User>(user.Username);
            if(retrieved.Books == null)
            {
                retrieved.Books = userbook;
            }else
            {
                retrieved.Books.Add(selected);
            }           
            context.Save<User>(retrieved);

            string userdownloadpath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), selected.PDFFile.Key);
            selected.PDFFile.DownloadTo(userdownloadpath);

           
            //userbookshelfwindow.loadBooks();
            MessageBox.Show("Book has successfully added to your bookshelf!");


        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            User retrieved = context.Load<User>(user.Username);
            UserBookshelfWindow ubs = new UserBookshelfWindow(retrieved);
            ubs.Show();
        }

    
    }
}
