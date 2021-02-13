using System.ComponentModel.DataAnnotations.Schema;

namespace WarForCybertron.Model
{
    public class Transformer : TransformerBase
    {
        [NotMapped]
        public int OverallRating { get; set; }
        public bool GodMode { get; set; }
    }
}
