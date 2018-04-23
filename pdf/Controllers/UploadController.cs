using pdf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;

namespace pdf.Controllers
{
    public class UploadController : Controller
    {
        public ActionResult FileUpload()
        {
            return View();
        }


        [HttpPost]
        public ActionResult FileSave()
        {

            try
            {
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var pdf = System.Web.HttpContext.Current.Request.Files["HelpSectionImages"];
                    var one = JsonConvert.DeserializeObject(System.Web.HttpContext.Current.Request.Params["Data"]);
                    HttpPostedFileBase filebase = new HttpPostedFileWrapper(pdf);

                    String FileExt = Path.GetExtension(filebase.FileName).ToUpper();

                    if (FileExt == ".PDF")
                    {
                        Stream str = filebase.InputStream;
                        BinaryReader Br = new BinaryReader(str);
                        Byte[] FileDet = Br.ReadBytes((Int32)str.Length);

                        FileDetail Fd = new FileDetail();
                        Fd.FileName = filebase.FileName;
                        Fd.FileContent = FileDet;
                        if (SaveFileDetails(Fd))
                            return Json(one);
                        else
                        {
                            return Json("No File Saved.");
                        }
                    }
                    else
                    {
                        return Json("invalid format");
                    }
                }
                else
                {
                    return Json("invalid format");
                }
            }
            catch (Exception)
            {
                return Json("Error While Saving.");
            }

            

        }

        private bool SaveFileDetails(FileDetail objDet)
        {
            DbConnection();

            SqlDataReader reader;

            using (con)
            {
                try
                {
                    con.Open();

                    //read annual balance
                    SqlCommand com = new SqlCommand("AddFileDetails", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@FileName", objDet.FileName);
                    com.Parameters.AddWithValue("@FileContent", objDet.FileContent);
                    reader = com.ExecuteReader();
                    reader.Read();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }


        }


        [HttpGet]
        public JsonResult FileDetails()
        {
            List<FileDetail> DetList = GetFileList();

            return Json(DetList, JsonRequestBehavior.AllowGet);


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

                            //if (!(reader["FileContent"] is DBNull))
                            //    evb.FileContent = ObjectToByteArray(reader["FileContent"]);


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


        private List<FileDetail> GetFileList2()
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

        public FileResult DownLoadFile(int id)
        {
            List<FileDetail> ObjFiles = GetFileList2();
            var FileById = (from FC in ObjFiles
                            where FC.Id.Equals(id)
                            select new { FC.FileName, FC.FileContent }).ToList().FirstOrDefault();


            var response = new FileContentResult(FileById.FileContent, "pdf");
            response.FileDownloadName = FileById.FileName;
            return response;
            //System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
            //{
            //    Inline = true
            //};

            //Response.Headers.Add("Content-Disposition", cd.ToString());

            //return File(FileById.FileContent, "application/pdf", FileById.FileName);
        }


        private SqlConnection con;
        private string constr;
        private void DbConnection()
        {
            constr = ConfigurationManager.ConnectionStrings["Data"].ToString();
            con = new SqlConnection(constr);

        }
    }
}