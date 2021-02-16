using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

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

            foreach (var prop in RatingProperties)
            {
                overallRating += (int)prop.GetValue(this);
            };

            return overallRating;
        }

        [NotMapped]
        public PropertyInfo[] RatingProperties { get => GetType().GetProperties().Where(p => p.PropertyType == typeof(int) && p.Name != "OverallRating").ToArray(); }
    }
}
