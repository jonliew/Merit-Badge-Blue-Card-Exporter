using System.Collections.Generic;
using System.Linq;

namespace BlueCardExporter.Models.ViewModels
{
    public class MeritBadgeStudentViewModel : MeritBadgeStudent
    {
        public MeritBadgeStudentViewModel(MeritBadgeStudent meritBadgeStudent, MeritBadgeDataHelperModel data)
        {
            StudentId  = meritBadgeStudent.StudentId;
            FirstName  = meritBadgeStudent.FirstName;
            LastName   = meritBadgeStudent.LastName;
            Address    = meritBadgeStudent.Address;
            City       = meritBadgeStudent.City;
            State      = meritBadgeStudent.State;
            ZipCode    = meritBadgeStudent.ZipCode;
            UnitType   = meritBadgeStudent.UnitType;
            UnitNumber = meritBadgeStudent.UnitNumber;
            District   = meritBadgeStudent.District;
            Council    = meritBadgeStudent.Council;
            Email      = meritBadgeStudent.Email;

            StudentClassEntries = data.StudentClassEntries.Where(e => e.StudentId == StudentId).Select(e => new StudentClassEntryViewModel(e, data));
        }

        public IEnumerable<StudentClassEntryViewModel> StudentClassEntries { get; set; }
    }
}
