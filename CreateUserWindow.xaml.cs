using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Internal.Util;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for CreateUserWindow.xaml
    /// </summary>
    public partial class CreateUserWindow : Window
    {
        
        private AmazonDynamoDBClient client;
        private DynamoDBContext context;
        //private string bucketName;
        public CreateUserWindow()
        {
            client = new AmazonDynamoDBClient();
            InitializeComponent();
            crtBtn.IsEnabled = false;
        }

        private void crtBtn_Click(object sender, RoutedEventArgs e)
        {
            
           
            //before saving we need the code to check if the username is unique
            //save student info
            context = new DynamoDBContext(client);
            User userobj = new User
            {
                FirstName = firstNameTxt.Text,
                LastName = lastNameTxt.Text,
                Username = crtusernameTxt.Text,
                Password = crtpassTxt.Text,
                Books = new List<Book>()
            };
            try
            {
                context.Save<User>(userobj);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error occured while saving user info into table. Err : " + ex.Message);
            }
            MessageBoxResult msr =  MessageBox.Show("Successfully registered!","confirmation",MessageBoxButton.OK);
            if(msr == MessageBoxResult.OK)
            {
                this.Close();
            }


        }


        private void crtusernameTxt_TextChanged(object sender, TextChangedEventArgs e)
        {        
             if (crtpassTxt.Text != "")
             {
                 crtBtn.IsEnabled = true;
             }
            
            
        }

        private void crtpassTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(crtusernameTxt.Text != "")
            {
                crtBtn.IsEnabled = true;
            }
            
        }
        
    }
}
