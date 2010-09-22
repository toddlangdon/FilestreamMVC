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

namespace FilestreamMVC.Controllers
{
    public class DocumentRepositoryController : Controller
    {
        private IEnumerable<Document> GetDocuments(string searchPhrase)
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
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(string phrase)
        {
            var documents = this.GetDocuments(phrase);
            return View("SearchResults", documents);
        }

        public ActionResult LinqSearch(string phrase)
        {
            //TODO Implement a Search action that uses Linq to query for documents
            throw new NotImplementedException();
        }

    }
}
