using AutoMapper;
using Database.Models;
using Domain.Models;

namespace Domain;

public class MappingProfile : Profile
{
    /// <summary>
    /// Método que define o mapeamento das classes do sistema
    /// </summary>
    public MappingProfile()
    {
        CreateMap<Documento, DocumentoResponse>()
                                        .ForMember(to => to.Situacao, from => from.MapFrom(f => f.Situacao.ToString()));

        CreateMap<Cliente, ClienteResponse>();
    }
}
