using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace _300958687_kim__Lab2
{
    [DynamoDBTable("Books")]
    public class Book
    {
        [DynamoDBHashKey]
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }

        [DynamoDBProperty("Last seen")]
        public int CurrentPage { get; set; }

        [DynamoDBProperty("Content")]
        public S3Link PDFFile { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
    
    [DynamoDBTable("Users")]
    public class User
    {
        [DynamoDBHashKey]
        public string Username { get; set; }
        public string Password { get; set; }
        [DynamoDBProperty("First Name")]
        public string FirstName { get; set; }
        [DynamoDBProperty("Last Name")]
        public string LastName { get; set; }
        
        public List<Book> Books { get; set; }
    }
}
