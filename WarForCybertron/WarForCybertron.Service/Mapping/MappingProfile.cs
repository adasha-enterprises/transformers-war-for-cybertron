using AutoMapper;
using WarForCybertron.Model;
using WarForCybertron.Model.DTO;

namespace WarForCybertron.Service.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TransformerDTO, Transformer>();

            CreateMap<Transformer, TransformerDTO>();

            CreateMap<Transformer, Transformer>();
        }
    }
}
