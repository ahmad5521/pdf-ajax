using pdf.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Mvc;

namespace pdf.Controllers
{
    public class DownloadController : Controller
    {
        #region Upload Download file  
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public FileResult DownLoadFile(int id)
        {


            List<FileDetail> ObjFiles = GetFileList();

            var FileById = (from FC in ObjFiles
                            where FC.Id.Equals(id)
                            select new { FC.FileName, FC.FileContent }).ToList().FirstOrDefault();

            return File(FileById.FileContent, "application/pdf", FileById.FileName);

        }
        #endregion

        #region View Uploaded files  
        [HttpGet]
        public PartialViewResult FileDetails()
        {
            List<FileDetail> DetList = GetFileList();

            return PartialView("FileDetails", DetList);


        }
        private List<FileDetail> GetFileList()
        {
            List<FileDetail> DetList = new List<FileDetail>();

            DbConnection();
            SqlDataReader reader;

            using (con)
            {
                try
                {
                    con.Open();

                    //read annual balance
                    SqlCommand com = new SqlCommand("GetFileDetails", con);
                    com.CommandType = CommandType.StoredProcedure;
                    reader = com.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            FileDetail evb = new FileDetail();
                            if (!(reader["Id"] is DBNull))
                                evb.Id = Convert.ToInt16(reader["Id"]);


                            if (!(reader["FileName"] is DBNull))
                                evb.FileName = Convert.ToString(reader["FileName"]);

                            if (!(reader["FileContent"] is DBNull))
                                evb.FileContent = ObjectToByteArray(reader["FileContent"]);


                            DetList.Add(evb);
                        }
                    }
                }
                catch (Exception)
                {
                    return new List<FileDetail>();
                }
            }
            return DetList;
        }

        byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        #endregion
        #region Database connection  

        private SqlConnection con;
        private string constr;
        private void DbConnection()
        {
            constr = ConfigurationManager.ConnectionStrings["Data"].ToString();
            con = new SqlConnection(constr);

        }
        #endregion
    }
}