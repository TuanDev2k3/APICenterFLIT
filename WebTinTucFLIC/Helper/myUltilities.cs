using WebTinTucFLIC.Models;

namespace WebTinTucFLIC.Helper
{
    public class myUltilities
    {
        public static string UploadImage(IFormFile imageFile, string folder)
        {
            try
            {
                var imagePath = Path.Combine("wwwroot", "images", folder, imageFile.FileName);
                using (FileStream myfile = new FileStream(imagePath, FileMode.Create))
                {
                    imageFile.CopyTo(myfile);
                    myfile.Close();
                }
                return imageFile.FileName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
