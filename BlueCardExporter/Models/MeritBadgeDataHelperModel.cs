using System.Collections.Generic;

namespace BlueCardExporter.Models
{
    public class MeritBadgeDataHelperModel
    {
        public List<MeritBadgeClass> MeritBadgeClasses { get; set; }
        public List<MeritBadgeCounselor> MeritBadgeCounselors { get; set; }
        public List<MeritBadgeStudent> MeritBadgeStudents { get; set; }
        public List<StudentClassEntry> StudentClassEntries { get; set; }
    }
}
