using System.Linq;

namespace BlueCardExporter.Models
{
    public class MeritBadgeStudent
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string UnitType { get; set; }
        public int? UnitNumber { get; set; }
        public string District { get; set; }
        public string Council { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            return string.Join(',', GetType().GetProperties().Select(e => e.GetValue(this)));
        }
    }
}
