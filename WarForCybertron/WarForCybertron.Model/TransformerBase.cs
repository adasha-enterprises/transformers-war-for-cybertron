using System;
using System.ComponentModel.DataAnnotations;

namespace WarForCybertron.Model
{
    public abstract class TransformerBase : GuidEntity
    {
        public string Name { get; set; }
        public Allegiance Allegiance { get; set; }
        [Range(1, 10, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Strength { get; set; }
        [Range(1, 10, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Intelligence { get; set; }
        [Range(1, 10, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Speed { get; set; }
        [Range(1, 10, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Endurance { get; set; }
        [Range(1, 10, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Rank { get; set; }
        [Range(1, 10, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Courage { get; set; }
        [Range(1, 10, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Firepower { get; set; }
        [Range(1, 10, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Skill { get; set; }
    }
}
