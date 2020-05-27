using System.Linq;

namespace BlueCardExporter.Models
{
    public class StudentClassEntry
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public string Complete { get; set; }
        public string Requirements { get; set; }
        public string Remarks { get; set; }

        public override string ToString()
        {
            return string.Join(',', GetType().GetProperties().Select(e => e.GetValue(this)));
        }
    }
}
