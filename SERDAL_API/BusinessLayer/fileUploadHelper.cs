namespace SERDAL_API.BusinessLayer
{
    public class fileUploadHelper
    {
        //public string UploadPDF(IFormFile file)
        //{
        //    List<string> ValidExtension = new List<string>() { ".pdf" };
        //    string extension = Path.GetExtension(file.FileName);
        //    if (!ValidExtension.Contains(extension))
        //    {
        //        return "Invalid filename extension";
        //    }

        //    long size = file.Length;
        //    if (size > (5 * 1024 * 1024))
        //    {
        //        return "Maximum size can be 5mb";
        //    }

        //    string filename = Guid.NewGuid().ToString() + extension;
        //    string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        //    using FileStream stream = new FileStream(Path.Combine(path, filename), FileMode.Create);
        //    file.CopyTo(stream);

        //    return filename;
        //}


        public string UploadPDFWithDetails(IFormFile file, string title, string author, string description, string pdfLink)
        {
            List<string> ValidExtension = new List<string>() { ".pdf" };
            string extension = Path.GetExtension(file.FileName);
            if (!ValidExtension.Contains(extension))
            {
                return "Invalid filename extension";
            }

            long size = file.Length;
            if (size > (5 * 1024 * 1024))
            {
                return "Maximum size can be 5mb";
            }

            // Optionally, process the additional fields here (title, author, etc.)
            string filename = Guid.NewGuid().ToString() + extension;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);  // Ensure the directory exists
            }

            using (FileStream stream = new FileStream(Path.Combine(path, filename), FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Example of processing: you can save this data to a database or log it
            // For example, return a summary message including all fields:
            return $"File uploaded successfully with the following details:\n" +
                   $"Title: {title}\nAuthor: {author}\nDescription: {description}\nPDF Link: {pdfLink}\nFile Path: {path}\\{filename}";
        }

    }
}
