namespace BlueCardExporter.Models.ViewModels
{
    public class MeritBadgeCounselorViewModel : MeritBadgeCounselor
    {
        public MeritBadgeCounselorViewModel(MeritBadgeCounselor meritBadgeCounselor, MeritBadgeDataHelperModel data)
        {
            CounselorId = meritBadgeCounselor.CounselorId;
            FirstName = meritBadgeCounselor.FirstName;
            LastName = meritBadgeCounselor.LastName;
            Address = meritBadgeCounselor.Address;
            City = meritBadgeCounselor.City;
            State = meritBadgeCounselor.State;
            ZipCode = meritBadgeCounselor.ZipCode;
            Telephone = meritBadgeCounselor.Telephone;
            Email = meritBadgeCounselor.Email;
        }

        public string DisplayName => $"{FirstName} {LastName}";
    }
}
