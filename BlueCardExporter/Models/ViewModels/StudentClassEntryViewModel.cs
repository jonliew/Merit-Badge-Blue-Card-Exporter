using BlueCardExporter.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlueCardExporter.Models.ViewModels
{
    public class StudentClassEntryViewModel : StudentClassEntry
    {
        public StudentClassEntryViewModel(StudentClassEntry studentClassEntry, MeritBadgeDataHelperModel data)
        {
            StudentId = studentClassEntry.StudentId;
            ClassId = studentClassEntry.ClassId;
            Complete = studentClassEntry.Complete;
            Requirements = studentClassEntry.Requirements;
            Remarks = studentClassEntry.Remarks;

            MeritBadgeClass = data.MeritBadgeClasses.Where(c => c.ClassId == ClassId).Select(c => new MeritBadgeClassViewModel(c, data)).SingleOrDefault();
        }
        
        public MeritBadgeClassViewModel MeritBadgeClass { get; set; }


        public bool IsComplete => Functions.ValidateDate(Complete, out _);

        public DateTime CompletionDate
        {
            get
            {
                Functions.ValidateDate(Complete, out DateTime date);
                return date;
            }
        }

        /// <summary>
        /// Helper class
        /// </summary>
        public class Requirement
        {
            public string RequirementTitle { get; set; }
            public bool IsComplete { get; set; }
            public DateTime CompletionDate { get; set; }
        }

        /// <summary>
        /// Gets the requirements from the delimited requirement string
        /// </summary>
        public IEnumerable<Requirement> RequirementList
        {
            get
            {
                var requirements = new List<Requirement>();
                var requirementPairs = Requirements.Split('^');
                foreach (var requirementPair in requirementPairs)
                {
                    var requirementEntry = requirementPair.Split('~');
                    var isComplete = Functions.ValidateDate(requirementEntry[1], out DateTime completionDate);

                    requirements.Add(new Requirement()
                    {
                        RequirementTitle = requirementEntry[0],
                        IsComplete = isComplete,
                        CompletionDate = completionDate
                    });
                }
                return requirements;
            }
        }
    }
}
