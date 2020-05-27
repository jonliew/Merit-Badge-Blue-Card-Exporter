using BlueCardExporter.Models.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlueCardExporter.Utility
{
    public static class BlueCardUtility
    {
        /// <summary>
        /// Get the blue cards for a merit badge student
        /// </summary>
        /// <param name="student">The merit badge student we want to get the blue cards for</param>
        /// <param name="numberOfPDFs">The number of PDFs we are going to generate</param>
        /// <returns>A MemoryStream containing the blue cards</returns>
        public static MemoryStream GetBlueCards(MeritBadgeStudentViewModel student, int numberOfPDFs)
        {
            var blueCardPDFFileName = BlueCardFields.BlueCardFormName;
            var blueCardPDFFilePath = Path.Combine("Files", blueCardPDFFileName);
            using Stream pdfInputStream = new FileStream(path: blueCardPDFFilePath, mode: FileMode.Open);
            using var resultPDFOutputStream = new MemoryStream();
            using Stream resultPDFStream = FillForm(pdfInputStream, student, numberOfPDFs);

            resultPDFStream.Position = 0;
            resultPDFStream.CopyTo(resultPDFOutputStream);
            return resultPDFOutputStream;
        }

        /// <summary>
        /// Utility method that fills out the blank blue card
        /// </summary>
        /// <param name="inputStream">The blank blue card</param>
        /// <param name="model">The merit badge student that contains the data</param>
        /// <param name="numberOfPDFs">The number of PDFs we are going to generate for the student</param>
        /// <returns>A Stream containing the filled out PDF</returns>
        private static Stream FillForm(Stream inputStream, MeritBadgeStudentViewModel model, int numberOfPDFs)
        {
            Stream outStream = new MemoryStream();
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            try
            {
                pdfReader = new PdfReader(inputStream);
                pdfStamper = new PdfStamper(pdfReader, outStream);
                AcroFields form = pdfStamper.AcroFields;

                // Name
                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.EMBEDDED);
                var studentName = model.FirstName + " " + model.LastName;
                form.SetField(BlueCardFields.StudentName, studentName);
                float fontSize = 250 / studentName.Length;
                if (fontSize > 12)
                {
                    fontSize = 12;
                }
                if (fontSize < 5)
                {
                    fontSize = 5;
                }
                form.SetFieldProperty(BlueCardFields.StudentName, "textsize", fontSize, null);
                form.SetFieldProperty(BlueCardFields.StudentName, "textfont", bf, null);
                form.RegenerateField(BlueCardFields.StudentName);

                form.SetField(BlueCardFields.Address, model.Address ?? "");
                form.SetField(BlueCardFields.CityStateZip, string.Concat(model.City, " ", model.State, " ", model.ZipCode));
                form.SetField(BlueCardFields.UnitTypeRB, model.UnitType);
                form.SetField(BlueCardFields.UnitType, model.UnitType);
                form.SetField(BlueCardFields.UnitNumber, model.UnitNumber.ToString());
                form.SetField(BlueCardFields.District, model.District ?? "");
                form.SetField(BlueCardFields.Council, model.Council ?? "");

                var mbcclasses = model.StudentClassEntries.ToList();



                for (var i = 1; i + 3 * (numberOfPDFs - 1) <= model.StudentClassEntries.Count() && i <= 3; i++)
                {
                    var mbcclass = mbcclasses[i + 3 * (numberOfPDFs - 1) - 1];
                    var number = i.ToString();
                    if (i == 3)
                    {
                        number = "";
                    }
                    form.SetField(BlueCardFields.MeritBadgeName + number, mbcclass.MeritBadgeClass.ClassName);
                    form.SetFieldProperty(BlueCardFields.MeritBadgeName + number, "textfont", bf, null);
                    form.RegenerateField(BlueCardFields.MeritBadgeName + number);


                    form.SetField(BlueCardFields.DateApplied + number, "");

                    if (mbcclass.MeritBadgeClass.Counselor != null)
                    {
                        if (mbcclass.IsComplete && mbcclass.MeritBadgeClass.Counselor.DisplayName != null)
                        {
                            form.SetField(BlueCardFields.Counselor + number, mbcclass.MeritBadgeClass.Counselor.FirstName + " " + mbcclass.MeritBadgeClass.Counselor.LastName);
                        }
                        form.SetField(BlueCardFields.CounselorAddress + number, mbcclass.MeritBadgeClass.Counselor.Address);
                        form.SetField(BlueCardFields.CounselorCityStateZip + number, $"{mbcclass.MeritBadgeClass.Counselor.City} {mbcclass.MeritBadgeClass.Counselor.State} {mbcclass.MeritBadgeClass.Counselor.ZipCode}");
                        form.SetField(BlueCardFields.CounselorPhone + number, mbcclass.MeritBadgeClass.Counselor.Telephone);
                    }
                    
                    var requirementTitleList = mbcclass.RequirementList.Select(r=>r.RequirementTitle).ToList();
                    var requirementList = mbcclass.RequirementList.ToList();
                    for (var j = 1; j <= requirementTitleList.Count; j++)
                    {
                        var appendString = number + "." + j.ToString("00");
                        form.SetField(BlueCardFields.Requirement + appendString, requirementTitleList[j - 1]);
                        var fieldString = BlueCardFields.RequirementDate + appendString;
                        var req = requirementList[j - 1];
                        form.SetField(fieldString, req.IsComplete ? req.CompletionDate.ToString("MM/dd/yyyy") : "");
                        form.SetFieldProperty(fieldString, "textsize", (float)6, null);
                        form.RegenerateField(fieldString);

                        var counselorInitials = string.Empty;
                        if (mbcclass.MeritBadgeClass.Counselor != null)
                        {
                            counselorInitials = mbcclass.MeritBadgeClass.Counselor.FirstName.ElementAt(0).ToString().ToUpperInvariant() +
                                                mbcclass.MeritBadgeClass.Counselor.LastName.ElementAt(0).ToString().ToUpperInvariant();
                        }
                        form.SetField(BlueCardFields.RequirementInitials + appendString, req.IsComplete ? counselorInitials : "");
                    }
                    if (mbcclass.IsComplete)
                    {
                        form.SetField(BlueCardFields.DateCompleted + number, mbcclass.CompletionDate.ToString("MM/dd/yyyy"));
                    }

                }
                if (numberOfPDFs * 3 - mbcclasses.Count > 0)
                {
                    form.SetField("Void", "VOID");
                    if (numberOfPDFs * 3 - mbcclasses.Count > 1)
                    {
                        form.SetField("Void2", "VOID");
                    }
                }
                pdfStamper.FormFlattening = true;
                return outStream;
            }
            finally
            {
                pdfStamper?.Close();
                pdfReader?.Close();
            }
        }

        /// <summary>
        /// Concatenates PDFs
        /// </summary>
        /// <param name="pdfByteContent">A list of byte arrays containing the PDFs</param>
        /// <returns>A single byte array for one PDF</returns>
        public static byte[] ConcatenatePDFs(IEnumerable<byte[]> pdfByteContent)
        {
            //Console.WriteLine("Concatenating PDFs...");
            using var ms = new MemoryStream();
            var doc = new Document();
            var copy = new PdfCopy(doc, ms);
            doc.Open();

            //Loop through each byte array
            foreach (var p in pdfByteContent)
            {
                //Create a PdfReader bound to that byte array
                var reader = new PdfReader(p);
                for (var i = 1; i <= reader.NumberOfPages; i++)
                {
                    copy.AddPage(copy.GetImportedPage(reader, i));
                }
                copy.FreeReader(reader);
                reader.Close();
            }
            copy.Close();
            doc.Close();
            var allPagesContent = ms.GetBuffer();
            ms.Flush();
            return allPagesContent;
        }
    }
}
