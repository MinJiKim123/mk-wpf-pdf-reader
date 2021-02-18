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
    /// Interaction logic for UserBookshelfWindow.xaml
    /// </summary>
    public partial class UserBookshelfWindow : Window
    {
        private User user;
        private DynamoDBContext context;
        private AmazonDynamoDBClient client;
        public UserBookshelfWindow(User usr)
        {
            client = new AmazonDynamoDBClient();
            InitializeComponent();
            readBtn.Visibility = Visibility.Hidden;
            bookimgReg.Visibility = Visibility.Hidden;
            user = usr;
            loadBooks(); 
        }

        public void loadBooks()
        {
            context = new DynamoDBContext(client);
            User retrieved = context.Load<User>(user.Username);
            if(retrieved.Books != null)
            {

                userBooksLst.ItemsSource = retrieved.Books;
            }
        }

        private void addBookBtn_Click(object sender, RoutedEventArgs e)
        {
            BookStoreWindow bsw = new BookStoreWindow(user);
            bsw.Show();
            this.Close();
        }

        private void userBooksLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Book selected = (Book)userBooksLst.SelectedItem;
            readBtn.Visibility = Visibility.Visible;
            if(selected.CurrentPage > 1)
            {
                readBtn.Content = "Continue Reading";
            }
            bookimgReg.Visibility = Visibility.Visible;         
            bookcovertitleLbl.Content = selected.Title;
            string summary = String.Format("<<Book Information>>\nTitle : {0}\nISBN : {1}\nAuthor : {2}\nLast Page Read : {3}", selected.Title, selected.ISBN, selected.Author,selected.CurrentPage);
            booksumLbl.Content = summary;
        }

        private void readBtn_Click(object sender, RoutedEventArgs e)
        {
            Book selected = (Book)userBooksLst.SelectedItem;
            string filepath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), selected.PDFFile.Key);
            ReadBookWindow rbw = new ReadBookWindow(filepath, selected, user);
            rbw.Show();
            this.Close();
        }
    }
}
