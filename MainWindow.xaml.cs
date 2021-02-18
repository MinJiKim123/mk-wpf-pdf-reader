using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _300958687_kim__Lab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string tableName = "Users";
        private string btableName = "Books";
        private AmazonDynamoDBClient client;
        private DynamoDBContext context;
        public MainWindow()
        {
            client = new AmazonDynamoDBClient();
            checknCrt();
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (usernameTxt.Text != null && passPbx.Password != null)
            {
                string username = usernameTxt.Text;
                //string password = passTxt.Text;
                string password = passPbx.Password;
                context = new DynamoDBContext(client);
                //Table UserTable = Table.LoadTable(client, "Users");
                User getUser = context.Load<User>(username);
                if (username == "ADMIN" && password == "password")
                {
                    AdminWindow adw = new AdminWindow();
                    adw.Show();

                }
                else if (getUser != null && getUser.Password == password)
                {
                    errorMsgLbl.Content = "";
                    UserBookshelfWindow usbsw = new UserBookshelfWindow(getUser);
                    usbsw.Show();
                }
                else
                {
                    errorMsgLbl.Content = "**Wrong username/password.";
                } 
            }else
            {
                errorMsgLbl.Content = "**Please put username/password";
            }
        }

        private void signupBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateUserWindow cuw = new CreateUserWindow();
            cuw.Show();
        }
        private void checknCrt()
        {
            //check and create bucket
            string inputBucketName = "minjikim306bookrepo";
            createBucket(inputBucketName);

            //check and create tables
            var tableResponse = client.ListTables();
            if (!tableResponse.TableNames.Contains(tableName))
            {                
                bool res = createTable();
                if (res)
                {
                    //MessageBox.Show("Successfully Registered!");
                    Console.WriteLine("Table creation successful");
                } else
                {
                    Process p = Process.GetCurrentProcess();
                    p.Kill();
                }
            }
           
        }
        private void createBucket(string bucketName)
        {
            try
            {

                IAmazonS3 s3client = new AmazonS3Client();
                if (!(AmazonS3Util.DoesS3BucketExistV2(s3client, bucketName)))
                {
                    PutBucketRequest request = new PutBucketRequest();
                    request.BucketName = bucketName;
                    request.UseClientRegion = true;
                    PutBucketResponse putBucketResponse = s3client.PutBucket(request);
                }

            }
            catch (AmazonS3Exception amazonS3Exception)
            {
               MessageBoxResult msbr = MessageBox.Show(String.Format("An error code {0} occured when creating bucket with message {1}", amazonS3Exception.ErrorCode, amazonS3Exception.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
               if(msbr == MessageBoxResult.OK)
                {
                    Process p = Process.GetCurrentProcess();
                    p.Kill();
                }
            }
        
        }
        private bool createTable()
        {
            //create book table
            try
            {
                client.CreateTable(new CreateTableRequest
                {
                    TableName = btableName,
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 3,
                        WriteCapacityUnits = 1
                    },
                    KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "ISBN",
                        KeyType = KeyType.HASH
                    }
                },
                    AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition { AttributeName = "ISBN", AttributeType=ScalarAttributeType.S }
                }
                });
                bool isTableAvailable = false;
                while (!isTableAvailable)
                {
                    Console.WriteLine("Waiting for user table to be active...");
                    Thread.Sleep(5000);
                    var tableStatus = client.DescribeTable(btableName);
                    isTableAvailable = tableStatus.Table.TableStatus == "ACTIVE";
                }
                //create user table
                client.CreateTable(new CreateTableRequest
                {
                    TableName = tableName,
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 3,
                        WriteCapacityUnits = 1
                    },
                    KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Username",
                        KeyType = KeyType.HASH
                    }
                },
                    AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition { AttributeName = "Username", AttributeType=ScalarAttributeType.S }
                }
                });
                isTableAvailable = false;
                while (!isTableAvailable)
                {
                    Console.WriteLine("Waiting for user table to be active...");
                    Thread.Sleep(5000);
                    var tableStatus = client.DescribeTable(tableName);
                    isTableAvailable = tableStatus.Table.TableStatus == "ACTIVE";
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult msr = MessageBox.Show(String.Format("Error occured while creating table with message {0}", ex.Message));
                return false;
            }

           
            
            return true;
        }
    }
}
