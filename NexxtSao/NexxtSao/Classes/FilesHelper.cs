using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace NexxtSao.Classes
{
    public class FilesHelper
    {
        public static bool UploadPhoto(HttpPostedFileBase file, string folder, string name)
        {
            if (file == null || string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(name))
            {
                return false;
            }

            try
            {
                string path = string.Empty;

                if (file != null)
                {
                    path = Path.Combine(HttpContext.Current.Server.MapPath(folder), name);
                    file.SaveAs(path);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }
                return true;
            }
            catch
            {

                return false;
            }

        }

        public static bool DeletePhoto(string foto)
        {           
            if (foto == null || string.IsNullOrEmpty(foto))
            {
                return false;
            }

            try
            {
                string path = string.Empty;
                path = Path.Combine(HttpContext.Current.Server.MapPath(foto));
                File.Delete(path);

                return true;
            }
            catch
            {

                return false;
            }

        }
    }
}