using System.Linq;

namespace BlueCardExporter.Models
{
    public class MeritBadgeClass
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int? CounselorId { get; set; }

        public override string ToString()
        {
            return string.Join(',', GetType().GetProperties().Select(e => e.GetValue(this)));
        }
    }
}
