using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WarForCybertron.Common.Configuration;
using WarForCybertron.Model;
using WarForCybertron.Model.DTO;
using WarForCybertron.Repository;
using WarForCybertron.Service.Helpers;
using WarForCybertron.Service.Interfaces;

namespace WarForCybertron.Service.Implementations
{
    public class WarForCybertronService : IWarForCybertronService
    {
        private readonly IMapper _mapper;
        private readonly IWarForCybertronRepository<Transformer, WarForCybertronContext> _transformers;
        private readonly ILogger<WarForCybertronService> _logger;
        private readonly ConfigSettings _configSettings;
        private readonly WarForCybertronContext _context;

        public WarForCybertronService(
                IWarForCybertronRepository<Transformer, WarForCybertronContext> transformers,
                ILoggerFactory loggerFactory,
                IOptions<ConfigSettings> configSettingsOptions,
                IMapper mapper,
                WarForCybertronContext context
            )
        {
            _transformers = transformers;
            _logger = loggerFactory.CreateLogger<WarForCybertronService>();
            _configSettings = configSettingsOptions.Value;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<TransformerDTO>>> GetTransformers(Allegiance? allegiance)
        {
            var message = string.Empty;
            var transformers = new List<TransformerDTO>();
            var isSuccess = false;

            try
            {
                var _ = await GetTransformers(allegiance, false);
                transformers = _mapper.Map<List<TransformerDTO>>(_);

                isSuccess = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                message = "Unable to get transformers";
            }

            return new ServiceResponse<List<TransformerDTO>>(transformers, message, isSuccess);
        }

        public async Task<ServiceResponse<TransformerDTO>> CreateTransformer(TransformerDTO transformerDTO)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var transformer = _mapper.Map<Transformer>(transformerDTO);

                // indicate whether or not a Transformer will be victorious in battle by virtue of their name
                transformer.GodMode = _configSettings.TRANSFORMERS_WITH_GOD_MODE.Split(',').Contains(transformer.Name);

                await _transformers.AddAsync(transformer);
                await _transformers.SaveChangesAsync();

                transformerDTO.Id = transformer.Id;

                isSuccess = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                message = "Unable to create Transformer";
            }

            return new ServiceResponse<TransformerDTO>(transformerDTO, message, isSuccess);
        }

        public async Task<ServiceResponse<TransformerDTO>> GetTransformer(Guid id)
        {
            var message = string.Empty;
            TransformerDTO transformerDTO = null;
            var isSuccess = false;

            try
            {
                var _ = _transformers.Find(t => t.Id == id);

                transformerDTO = _mapper.Map<TransformerDTO>(_);

                isSuccess = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                message = "Unable to create Transformer";
            }

            return await Task.FromResult(new ServiceResponse<TransformerDTO>(transformerDTO, message, isSuccess));
        }

        public async Task<ServiceResponse<TransformerDTO>> UpdateTransformer(TransformerDTO transformerDTO)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var existingTransformer = _transformers.Find(t => t.Id == transformerDTO.Id);
                var updatedTransformer = _mapper.Map<Transformer>(transformerDTO);

                // indicate whether or not a Transformer will be victorious in battle by virtue of their name
                updatedTransformer.GodMode = _configSettings.TRANSFORMERS_WITH_GOD_MODE.Split(',').Contains(updatedTransformer.Name);

                existingTransformer = _mapper.Map(updatedTransformer, existingTransformer);

                _transformers.Update(existingTransformer);
                await _transformers.SaveChangesAsync();

                isSuccess = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                message = "Unable to update Transformer";
            }

            return new ServiceResponse<TransformerDTO>(transformerDTO, message, isSuccess);
        }

        public async Task<bool> DeleteTransformer(Guid id)
        {
            var transformerDeleted = false;

            try
            {
                var _ = _transformers.Find(t => t.Id == id);
                await _transformers.DeleteAsync(_);
                await _transformers.SaveChangesAsync();
                transformerDeleted = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return await Task.FromResult(transformerDeleted);
        }

        public async Task<int> GetOverallScore(Guid id)
        {
            var score = 0;

            try
            {
                var idParam = new SqlParameter("Id", id);
                var scoreParam = new SqlParameter("Score", SqlDbType.Int) { Direction = ParameterDirection.Output };

                await _context.Database.ExecuteSqlRawAsync("EXEC GetTransformerScore @Id, @Score output", new[] { idParam, scoreParam });
                score = Convert.ToInt32(scoreParam.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
            }

            return await Task.FromResult(score);
        }

        public async Task<ServiceResponse<WarSimulation>> SimulateWar()
        {
            var message = string.Empty;
            var survivingAutobots = new List<TransformerDTO>();
            var survivingDecepticons = new List<TransformerDTO>();
            var isSuccess = false;

            try
            {
                var _ = await GetTransformers(null, true);
                
                var autobots = new Stack<Transformer>(_.Where(t => t.Allegiance == Allegiance.AUTOBOT).ToList());
                var decepticons = new Stack<Transformer>(_.Where(t => t.Allegiance == Allegiance.DECEPTICON).ToList());

                var battleCount = Math.Min(autobots.Count, decepticons.Count);

                for (var i = 0; i < battleCount; i++)
                {
                    // in theory, we shouldn't have a situation where either of the Transformers are null
                    var autobot = autobots.Peek() != null ? autobots.Pop() : null;
                    var decepticon = decepticons.Peek() != null ? decepticons.Pop() : null;

                    var victor = TransformerHelpers.TransformerBattle(autobot, decepticon);

                    if (victor == autobot || victor == null)
                    {
                        survivingAutobots.Add(_mapper.Map<TransformerDTO>(autobot));
                    }

                    if (victor == decepticon || victor == null)
                    {
                        survivingDecepticons.Add(_mapper.Map<TransformerDTO>(decepticon));
                    }
                }

                if (autobots.Count > 0)
                {
                    while (autobots.Count > 0)
                    {
                        survivingAutobots.Add(_mapper.Map<TransformerDTO>(autobots.Pop()));
                    }
                }
                else if (decepticons.Count > 0)
                {
                    while (autobots.Count > 0)
                    {
                        survivingDecepticons.Add(_mapper.Map<TransformerDTO>(decepticons.Pop()));
                    }
                }

                isSuccess = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                message = "Unable to get simulate war";
            }

            return new ServiceResponse<WarSimulation>(new WarSimulation(survivingAutobots, survivingDecepticons), message, isSuccess);
        }

        private void BuildSurvivorsList()
        {

        }

        private async Task<List<Transformer>> GetTransformers(Allegiance? allegiance, bool sortByRank)
        {
            var transformers = await _transformers
                                .Where(t => t.Allegiance == (allegiance != null ? allegiance : t.Allegiance))
                                .OrderBy(t => t.Name)
                                .ToListAsync();

            if (sortByRank)
            {
                transformers = transformers.OrderBy(t => t.Rank).ToList();
            }

            return transformers;
        }
    }
}
