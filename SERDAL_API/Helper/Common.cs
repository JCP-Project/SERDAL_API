namespace SERDAL_API.Helper
{
    public static class Common
    {
        public static string GetFullURL()
        {

            return "";
        }

        public static void isFolderExist(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string folder = Path.GetDirectoryName(path);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        public async static void CreateFile(IFormFile file, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            using (var pdf = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(pdf);
            }
        }

    }
}
