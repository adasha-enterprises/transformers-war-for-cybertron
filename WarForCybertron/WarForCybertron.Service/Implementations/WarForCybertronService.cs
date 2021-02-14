﻿using AutoMapper;
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
            var isSuccess = false;

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
    }
}
