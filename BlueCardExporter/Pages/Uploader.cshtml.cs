using System.Collections.Generic;
using System.Threading.Tasks;
using BlueCardExporter.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlueCardExporter.Pages
{
    public class UploaderModel : PageModel
    {
        public void OnGet()
        {

        }

        /// <summary>
        /// Uploads the file
        /// </summary>
        /// <param name="file">File to import</param>
        /// <param name="fileType">What the file is for</param>
        /// <returns>JSON with the result messages</returns>
        public async Task<JsonResult> OnPost(IFormFile file, int fileType)
        {
            List<string> validateMessageRows;
            (_, validateMessageRows) = await FileUtility.ImportFile(file, fileType);
            return new JsonResult(validateMessageRows);
        }
    }
}