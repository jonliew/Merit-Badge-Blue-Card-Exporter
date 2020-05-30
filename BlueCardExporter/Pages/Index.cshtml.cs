using BlueCardExporter.Models;
using BlueCardExporter.Models.ViewModels;
using BlueCardExporter.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueCardExporter.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IMemoryCache _cache;
        private List<MeritBadgeClass> MeritBadgeClasses { get; set; }
        private List<MeritBadgeCounselor> MeritBadgeCounselors { get; set; }
        private List<MeritBadgeStudent> MeritBadgeStudents { get; set; }
        private List<StudentClassEntry> StudentClassEntries { get; set; }

        private MeritBadgeDataHelperModel Data { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public void OnGet()
        {

        }

        /// <summary>
        /// Exports the blue cards
        /// </summary>
        /// <param name="classFile">File of classes</param>
        /// <param name="counselorFile">File of counselors</param>
        /// <param name="studentFile">File of students</param>
        /// <param name="studentEntryFile">File of student class entries</param>
        /// <param name="sortOrder">The sort order</param>
        /// <param name="writeVoid">Whether to separate the downloads by unit number</param>
        /// <returns>JSON to download the file through AJAX</returns>
        public async Task<IActionResult> OnPost(IFormFile classFile, IFormFile counselorFile, IFormFile studentFile, IFormFile studentEntryFile, int sortOrder, bool writeVoid)
        {
            if (sortOrder <= 0 || 5 <= sortOrder)
            {
                sortOrder = 1;
            }

            (Data, _) = await FileUtility.ImportFile(classFile, (int)FileTypeEnum.ClassFile);
            MeritBadgeClasses = Data.MeritBadgeClasses;
            (Data, _) = await FileUtility.ImportFile(counselorFile, (int)FileTypeEnum.CounselorFile);
            MeritBadgeCounselors = Data.MeritBadgeCounselors;
            (Data, _) = await FileUtility.ImportFile(studentFile, (int)FileTypeEnum.StudentFile);
            MeritBadgeStudents = Data.MeritBadgeStudents;
            (Data, _) = await FileUtility.ImportFile(studentEntryFile, (int)FileTypeEnum.StudentClassEntryFile);
            StudentClassEntries = Data.StudentClassEntries;

            if (MeritBadgeClasses == null || MeritBadgeCounselors == null || MeritBadgeStudents == null || StudentClassEntries == null)
            {
                return NotFound("Error: One or more of the import files are invalid. Please validate the files before exporting.");
            }

            if (sortOrder == 2)
            {
                MeritBadgeStudents = MeritBadgeStudents.OrderBy(s => s.FirstName).ToList();
            }
            else if (sortOrder == 3)
            {
                MeritBadgeStudents = MeritBadgeStudents.OrderBy(s => s.LastName).ToList();
            }
            else if (sortOrder == 4)
            {
                MeritBadgeStudents = MeritBadgeStudents.OrderBy(s => s.UnitNumber).ToList();
            }

            Data = new MeritBadgeDataHelperModel
            {
                MeritBadgeClasses = MeritBadgeClasses,
                MeritBadgeCounselors = MeritBadgeCounselors,
                MeritBadgeStudents = MeritBadgeStudents,
                StudentClassEntries = StudentClassEntries,
            };

            var byteArrays = new List<byte[]>();
            foreach (var student in MeritBadgeStudents)
            {
                try
                {
                    var mbcstudent = new MeritBadgeStudentViewModel(student, Data);

                    if (mbcstudent.StudentClassEntries.Any())
                    {
                        var numberOfPDFs = mbcstudent.StudentClassEntries.Count() / 3;
                        numberOfPDFs += mbcstudent.StudentClassEntries.Count() % 3 != 0 ? 1 : 0;

                        for (var i = 1; i <= numberOfPDFs; i++)
                        {
                            using var outputPDFStream = BlueCardUtility.GetBlueCards(mbcstudent, i, writeVoid);
                            byteArrays.Add(outputPDFStream.ToArray());
                        }
                    }
                }
                catch (Exception e)
                {
                    return NotFound($"Error: Unable to export blue cards for merit badge student with StudentId = {student.StudentId}. Error Message: {e.Message}");
                }
            }

            var handle = Guid.NewGuid().ToString();
            _cache.Set(handle, BlueCardUtility.ConcatenatePDFs(byteArrays), new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5)));
            var outputFileName = $"BlueCards_{DateTime.Now:yyyy-MM-dd}.pdf";
            var result = new JsonResult(new { FileGuid = handle, FileName = outputFileName });
            return result;
        }
    }
}
