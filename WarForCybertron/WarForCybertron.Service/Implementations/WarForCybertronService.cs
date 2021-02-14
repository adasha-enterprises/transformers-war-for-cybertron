using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarForCybertron.Common.Configuration;
using WarForCybertron.Model;
using WarForCybertron.Model.DTO;
using WarForCybertron.Repository;
using WarForCybertron.Service.Interfaces;

namespace WarForCybertron.Service.Implementations
{
    public class WarForCybertronService : IWarForCybertronService
    {
        private readonly IMapper _mapper;
        private readonly IWarForCybertronRepository<Transformer, WarForCybertronContext> _transformers;
        private readonly ILogger<WarForCybertronService> _logger;
        private readonly ConfigSettings _configSettings;

        public WarForCybertronService(
                IWarForCybertronRepository<Transformer, WarForCybertronContext> transformers,
                ILoggerFactory loggerFactory,
                IOptions<ConfigSettings> configSettingsOptions,
                IMapper mapper
            )
        {
            _transformers = transformers;
            _logger = loggerFactory.CreateLogger<WarForCybertronService>();
            _configSettings = configSettingsOptions.Value;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<TransformerDTO>>> GetTransformers(Allegiance? allegiance, bool sortByRank = false)
        {
            var message = string.Empty;
            var transformers = new List<TransformerDTO>();

            try
            {
                var _ = await _transformers
                    .Where(t => t.Allegiance == (allegiance != null ? allegiance : t.Allegiance))
                    .OrderBy(t => t.Name)
                    .ToListAsync();
                transformers = _mapper.Map<List<TransformerDTO>>(_);

                if (sortByRank)
                {
                    transformers = transformers.OrderByDescending(t => t.Rank).ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                message = "Unable to get transformers";
            }

            return new ServiceResponse<List<TransformerDTO>>(transformers, message);
        }

        public async Task<ServiceResponse<TransformerDTO>> CreateTransformer(TransformerDTO transformerDTO)
        {
            var message = string.Empty;

            try
            {
                var transformer = _mapper.Map<Transformer>(transformerDTO);

                // indicate whether or not a Transformer will be victorious in battle by virtue of their name
                transformer.GodMode = _configSettings.TRANSFORMERS_WITH_GOD_MODE.Split(',').Contains(transformer.Name);

                await _transformers.AddAsync(transformer);
                await _transformers.SaveChangesAsync();

                transformerDTO.Id = transformer.Id;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                message = "Unable to create Transformer";
            }

            return new ServiceResponse<TransformerDTO>(transformerDTO, message);
        }
    }
}
