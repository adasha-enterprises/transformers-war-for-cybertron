using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarForCybertron.Model;
using WarForCybertron.Model.DTO;

namespace WarForCybertron.Service.Interfaces
{
    public interface IWarForCybertronService
    {
        Task<ServiceResponse<List<TransformerDTO>>> GetTransformers(Allegiance? allegiance);

        Task<ServiceResponse<TransformerDTO>> CreateTransformer(TransformerDTO transformerDTO);

        Task<ServiceResponse<TransformerDTO>> GetTransformer(Guid id);

        Task<ServiceResponse<TransformerDTO>> UpdateTransformer(TransformerDTO transformerDTO);

        Task<bool> DeleteTransformer(Guid id);

        Task<int> GetOverallScore(Guid id);

        Task<ServiceResponse<WarSimulation>> SimulateWar();
    }
}
