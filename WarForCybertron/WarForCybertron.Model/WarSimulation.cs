using System.Collections.Generic;
using WarForCybertron.Model.DTO;

namespace WarForCybertron.Model
{
    public class WarSimulation
    {
        public List<TransformerDTO> Autobots { get; set; }
        public List<TransformerDTO> Decepticons { get; set; }

        public WarSimulation(List<TransformerDTO> autobots, List<TransformerDTO> decepticons)
        {
            Autobots = autobots;
            Decepticons = decepticons;
        }
    }
}
