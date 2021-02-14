using System.Collections.Generic;
using System.Threading.Tasks;
using WarForCybertron.Model;
using WarForCybertron.Model.DTO;

namespace WarForCybertron.Service.Interfaces
{
    public interface IWarForCybertronService
    {
        Task<ServiceResponse<List<TransformerDTO>>> GetTransformers(Allegiance? allegiance, bool sortByRank = false);

        Task<ServiceResponse<TransformerDTO>> CreateTransformer(TransformerDTO transformerDTO);
    }
}
