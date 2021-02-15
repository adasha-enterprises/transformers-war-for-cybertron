using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WarForCybertron.Model
{
    public class Transformer : TransformerBase
    {
        [NotMapped]
        public int OverallRating { get => GetOverallRating(); }
        public bool GodMode { get; set; }

        private int GetOverallRating()
        {
            var overallRating = 0;

            foreach (var prop in GetType().GetProperties().Where(p => p.PropertyType == typeof(int) && p.Name != "OverallRating"))
            {
                overallRating += (int)prop.GetValue(this);
            };

            return overallRating;
        }
    }
}
