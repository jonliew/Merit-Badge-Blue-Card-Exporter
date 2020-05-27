using BlueCardExporter.Models;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlueCardExporter.Utility
{
    public static class FileUtility
    {
        /// <summary>
        /// A list of message to be displayed to the user
        /// </summary>
        private static List<string> ResultMessage { get; set; }

        /// <summary>
        /// Imports the file into a data structure, validates it, and displays result messages
        /// </summary>
        /// <param name="file">The file the user submits</param>
        /// <param name="fileType">What the file is for</param>
        /// <returns>A tuple of the data and result messages</returns>
        public static async Task<(MeritBadgeDataHelperModel, List<string>)> ImportFile(IFormFile file, int fileType)
        {
            ResultMessage = new List<string>();
            if (file == null || file.Length == 0)
            {
                ResultMessage.Add("File is empty.");
                return (null, ResultMessage);
            }
            MeritBadgeDataHelperModel model = new MeritBadgeDataHelperModel();
            using (var stream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose))
            {
                try
                {
                    await file.CopyToAsync(stream).ConfigureAwait(false);
                    stream.Position = 0;
                    using var reader = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);

                    switch (fileType)
                    {
                        case (int)FileTypeEnum.ClassFile:
                            {
                                var classes = reader.GetRecords<MeritBadgeClass>();
                                if (classes == null)
                                {
                                    ResultMessage.Add("Error: Unable to validate file.");
                                }
                                else
                                {
                                    model.MeritBadgeClasses = classes.ToList();
                                    if (ValidateClassList(model.MeritBadgeClasses))
                                    {
                                        ResultMessage.Add($"Success! {model.MeritBadgeClasses.Count} records validated.");
                                    }
                                }
                                break;
                            }

                        case (int)FileTypeEnum.CounselorFile:
                            {
                                var counselors = reader.GetRecords<MeritBadgeCounselor>();
                                if (counselors == null)
                                {
                                    ResultMessage.Add("Error: Unable to validate file.");
                                }
                                else
                                {
                                    model.MeritBadgeCounselors = counselors.ToList();
                                    if (ValidateCounselorList(model.MeritBadgeCounselors))
                                    {
                                        ResultMessage.Add($"Success! {model.MeritBadgeCounselors.Count} records validated.");
                                    }
                                }
                                break;
                            }

                        case (int)FileTypeEnum.StudentFile:
                            {
                                var students = reader.GetRecords<MeritBadgeStudent>();
                                if (students == null)
                                {
                                    ResultMessage.Add("Error: Unable to validate file.");
                                }
                                else
                                {
                                    model.MeritBadgeStudents = students.ToList();
                                    if (ValidateStudentList(model.MeritBadgeStudents))
                                    {
                                        ResultMessage.Add($"Success! {model.MeritBadgeStudents.Count} records validated.");
                                    }
                                }
                                break;
                            }

                        case (int)FileTypeEnum.StudentClassEntryFile:
                            {
                                var studentClassEntries = reader.GetRecords<StudentClassEntry>();
                                if (studentClassEntries == null)
                                {
                                    ResultMessage.Add("Error: Unable to validate file.");
                                }
                                else
                                {
                                    var studentClassEntryList = studentClassEntries.ToList();
                                    model.StudentClassEntries = studentClassEntryList;
                                    if (ValidateStudentClassEntryList(model.StudentClassEntries))
                                    {
                                        ResultMessage.Add($"Success! {model.StudentClassEntries.Count} records validated.");
                                    }
                                }
                                break;
                            }

                        default:
                            ResultMessage.Add("Error: Invalid file type passed in.");
                            break;
                    }
                }
                catch (Exception e)
                {
                    ResultMessage.Add("Error: Unable to validate file.");
                    ResultMessage.Add(e.Message);
                }
            }
            return (model, ResultMessage);
        }

        /// <summary>
        /// Validates the class list. Outputs result messages
        /// </summary>
        /// <param name="classList">The list of classes to validate</param>
        /// <returns>True if valid, false in not</returns>
        private static bool ValidateClassList(List<MeritBadgeClass> classList)
        {
            var isValid = true;
            if (classList.Count != classList.Select(e=>e.ClassId).Distinct().Count())
            {
                ResultMessage.Add("Error: There are duplicate ClassIds.");
                isValid = false;
            }

            if (classList.Where(e=>string.IsNullOrWhiteSpace(e.ClassName)).Any())
            {
                ResultMessage.Add("Error: There are null or empty ClassNames.");
                isValid = false;
            }

            if (classList.Where(e => e.ClassName[0] != e.ClassName.ToUpper()[0]).Any())
            {
                ResultMessage.Add("Warning: There are ClassNames that do not begin with an upparcase letter.");
            }

            ResultMessage.Add(string.Join(',', classList[0].GetType().GetProperties().Select(p => p.Name)));
            ResultMessage.AddRange(classList.Select(e => e.ToString()));
            return isValid;
        }

        /// <summary>
        /// Validates the counselor list. Outputs result messages
        /// </summary>
        /// <param name="counselorList">The list of counselors to validate</param>
        /// <returns>True if valid, false in not</returns>
        private static bool ValidateCounselorList(List<MeritBadgeCounselor> counselorList)
        {
            var isValid = true;
            if (counselorList.Count != counselorList.Select(e => e.CounselorId).Distinct().Count())
            {
                ResultMessage.Add("Error: There are duplicate CounselorIds.");
                isValid = false;
            }

            if (counselorList.Where(e => string.IsNullOrWhiteSpace(e.FirstName)).Any())
            {
                ResultMessage.Add("Error: There are null or empty FirstNames.");
                isValid = false;
            }

            if (counselorList.Where(e => string.IsNullOrWhiteSpace(e.LastName)).Any())
            {
                ResultMessage.Add("Error: There are null or empty LastNames.");
                isValid = false;
            }

            ResultMessage.Add(string.Join(',', counselorList[0].GetType().GetProperties().Select(p => p.Name)));
            ResultMessage.AddRange(counselorList.Select(e => e.ToString()));
            return isValid;
        }

        /// <summary>
        /// Validates the student list. Outputs result messages
        /// </summary>
        /// <param name="studentList">The list of students to validate</param>
        /// <returns>True if valid, false in not</returns>
        private static bool ValidateStudentList(List<MeritBadgeStudent> studentList)
        {
            var isValid = true;
            if (studentList.Count != studentList.Select(e => e.StudentId).Distinct().Count())
            {
                ResultMessage.Add("Error: There are duplicate StudentIds.");
                isValid = false;
            }

            if (studentList.Where(e => string.IsNullOrWhiteSpace(e.FirstName)).Any())
            {
                ResultMessage.Add("Error: There are null or empty FirstNames.");
                isValid = false;
            }

            if (studentList.Where(e => string.IsNullOrWhiteSpace(e.LastName)).Any())
            {
                ResultMessage.Add("Error: There are null or empty LastNames.");
                isValid = false;
            }

            var validUnitTypes = new List<string> { "Troop", "Team", "Crew" };
            if (studentList.Where(e=> !string.IsNullOrWhiteSpace(e.UnitType) && !validUnitTypes.Contains(e.UnitType)).Any())
            {
                ResultMessage.Add("Error: There is at least one invalid UnitType. UnitTypes must be either Troop, Team, or Crew.");
                isValid = false;
            }

            ResultMessage.Add(string.Join(',', studentList[0].GetType().GetProperties().Select(p => p.Name)));
            ResultMessage.AddRange(studentList.Select(e => e.ToString()));
            return isValid;
        }

        /// <summary>
        /// Validates the student class entry list. Outputs result messages
        /// </summary>
        /// <param name="studentClassEntryList">The list of entries to validate</param>
        /// <returns>True if valid, false in not</returns>
        private static bool ValidateStudentClassEntryList(List<StudentClassEntry> studentClassEntryList)
        {
            var isValid = true;
            if (studentClassEntryList.Where(e=>string.IsNullOrWhiteSpace(e.Requirements)).Any())
            {
                ResultMessage.Add("Error: There are null or empty Requirements.");
                isValid = false;
            }

            ResultMessage.Add(string.Join(',', studentClassEntryList[0].GetType().GetProperties().Select(p => p.Name)));
            ResultMessage.AddRange(studentClassEntryList.Select(e => e.ToString()));
            return isValid;
        }
    }
}
