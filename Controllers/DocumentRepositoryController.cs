using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using FilestreamMVC.Models;
using Microsoft.SqlServer.Server;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.IO;

namespace FilestreamMVC.Controllers
{
    public class DocumentRepositoryController : Controller
    {
        //These contants are passed to the OpenSqlFilestream()
        //API DesiredAccess parameter. They define the type
        //of BLOB access that is needed by the application.

        const UInt32 DESIRED_ACCESS_READ = 0x00000000;
        const UInt32 DESIRED_ACCESS_WRITE = 0x00000001;
        const UInt32 DESIRED_ACCESS_READWRITE = 0x00000002;

        //These contants are passed to the OpenSqlFilestream()
        //API OpenOptions parameter. They allow you to specify
        //how the application will access the FILESTREAM BLOB
        //data. If you do not want this ability, you can pass in
        //the value 0. In this code sample, the value 0 has
        //been defined as SQL_FILESTREAM_OPEN_NO_FLAGS.

        const UInt32 SQL_FILESTREAM_OPEN_NO_FLAGS = 0x00000000;
        const UInt32 SQL_FILESTREAM_OPEN_FLAG_ASYNC = 0x00000001;
        const UInt32 SQL_FILESTREAM_OPEN_FLAG_NO_BUFFERING = 0x00000002;
        const UInt32 SQL_FILESTREAM_OPEN_FLAG_NO_WRITE_THROUGH = 0x00000004;
        const UInt32 SQL_FILESTREAM_OPEN_FLAG_SEQUENTIAL_SCAN = 0x00000008;
        const UInt32 SQL_FILESTREAM_OPEN_FLAG_RANDOM_ACCESS = 0x00000010;

        //This structure defines the format of the final parameter to the
        //OpenSqlFilestream() API.

        //This statement imports the OpenSqlFilestream API so that it
        //can be called in the Main() method below.
        [DllImport("sqlncli10.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern SafeFileHandle OpenSqlFilestream(
                    string Filestreamath,
                    uint DesiredAccess,
                    uint OpenOptions,
                    byte[] FilestreamTransactionContext,
                    uint FilestreamTransactionContextLength,
                    Int64 AllocationSize);

        //This statement imports the Win32 API GetLastError().
        //This is necessary to check whether OpenSqlFilestream
        //succeeded in returning a valid / handle

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern UInt32 GetLastError();

        private IEnumerable<Document> SearchDocuments(string searchPhrase)
        {
            
           SqlConnection cn =
                new SqlConnection(ConfigurationManager.ConnectionStrings["NorthPoleConnectionString"].ConnectionString);
            SqlCommand cmd = 
                new SqlCommand("SELECT ID, DocumentName FROM dbo.DocumentRepository WHERE CONTAINS(Document, @phrase)", 
                cn);
            cmd.Parameters.Add("@phrase", SqlDbType.VarChar).Value = searchPhrase;
            cn.Open();
            var reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                yield return new Document {ID = reader.GetInt32(0),Name=reader.GetString(1)};
            }
        }

        private Document GetDocument(string id)
        {
            Document doc;
            SqlConnection sqlConnection =
                new SqlConnection(ConfigurationManager.ConnectionStrings["NorthPoleConnectionString"].ConnectionString);

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = sqlConnection;

            try
            {
                sqlConnection.Open();

                // Everything we do with FILESTREAM must always be in
                // the context of a transaction, so we’ll start with
                // creating one.
                SqlTransaction transaction =
                    sqlConnection.BeginTransaction("mainTranaction");

                sqlCommand.Transaction = transaction;

                // The SQL gives us 3 values. First the PathName() method of
                // the Document field is called, we’ll need it to use the API
                // Second we call a special function that will tell us what
                // the context is for the current transaction, in this case
                // the "mainTransaction" we started above. Finally it gives
                // the name of the document, which the app will use when it
                // creates the document but is not strictly required as
                // part of the FILESTREAM.

                sqlCommand.CommandText =
                    "SELECT Document.PathName()" +
                    ", GET_FILESTREAM_TRANSACTION_CONTEXT() " +
                    ", DocumentName " +
                    ", DocumentExtension " +
                    "FROM dbo.DocumentRepository " +
                    "WHERE ID=@theID ";

                sqlCommand.Parameters.Add(
                  "@theID", SqlDbType.Int).Value = id;

                SqlDataReader reader = sqlCommand.ExecuteReader();

                if (reader.Read() == false)
                {
                    throw new Exception("Unable to get BLOB data");
                }

                // OK we have some data, pull it out of the reader into locals
                doc = new Document {
                    Extension = reader[3].ToString(),
                    Name = reader[2].ToString(),
                    Path = reader[0].ToString(),
                    Context = (byte[])reader[1]
                };

                reader.Close();

                // Now we need to use the API we declared at the top of this class
                // in order to get a handle.
                SafeFileHandle handle = OpenSqlFilestream(doc.Path,
                    DESIRED_ACCESS_READ,
                    SQL_FILESTREAM_OPEN_NO_FLAGS,
                    doc.Context,
                    (UInt32)doc.Context.Length, 0);

                // Using the handle we just got, we can open up a stream from
                // the database.
                FileStream databaseStream = new FileStream(
                  handle, FileAccess.Read);

                using (FileStream fileStream = new FileStream(handle, FileAccess.Read))
                {
                    BinaryReader binaryReader = new BinaryReader(fileStream);
                    doc.Bytes = binaryReader.ReadBytes(Convert.ToInt32(fileStream.Length));    
                }

                // Close the stream from the databaseStream
                databaseStream.Close();

                // Finally we should commit the transaction.
                sqlCommand.Transaction.Commit();
            }
            finally
            {
                sqlConnection.Close();
            }

            return doc;


        }

         public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(string phrase)
        {
            var documents = this.SearchDocuments(phrase);
            return View("SearchResults", documents);
        }

        public FileContentResult LaunchDocument(string id)
        {
            Document doc = GetDocument(id);
            
            return File(doc.Bytes, doc.ContentType, Server.HtmlEncode(doc.Name));
        }
    }
}
