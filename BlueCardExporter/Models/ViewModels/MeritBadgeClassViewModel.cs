using System.Linq;

namespace BlueCardExporter.Models.ViewModels
{
    public class MeritBadgeClassViewModel : MeritBadgeClass
    {
        public MeritBadgeClassViewModel(MeritBadgeClass meritBadgeClass, MeritBadgeDataHelperModel data)
        {
            ClassId = meritBadgeClass.ClassId;
            ClassName = meritBadgeClass.ClassName;
            CounselorId = meritBadgeClass.CounselorId;

            Counselor = data.MeritBadgeCounselors.Where(c => CounselorId != null && c.CounselorId == CounselorId)
                .Select(c => new MeritBadgeCounselorViewModel(c, data))
                .SingleOrDefault();
        }
        
        public MeritBadgeCounselorViewModel Counselor { get; set; }
    }
}
