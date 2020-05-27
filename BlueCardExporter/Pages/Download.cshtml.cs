using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace BlueCardExporter.Pages
{
    public class DownloadModel : PageModel
    {
        private readonly IMemoryCache _cache;
        public DownloadModel(IMemoryCache cache)
        {
            _cache = cache;
        }

        public ActionResult OnGet(string fileGuid, string filename)
        {
            var data = _cache.Get<byte[]>(fileGuid);
            if (data != null)
            {
                _cache.Remove(fileGuid);
                return File(data, "application/pdf", filename);
            }
            else
            {
                return new EmptyResult();
            }
        }
    }
}