using AutoMapper;
using WebApiEventos.DTO;
using WebApiEventos.Entidades;

namespace WebApiAlumnosSeg.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UsuarioDTO, Usuario>();
            CreateMap<Usuario, GetUsuarioDTO>();
            CreateMap<UsuarioPatchDTO, Usuario>().ReverseMap();
            CreateMap<Usuario, UsuariosDTOConEventos>()
                .ForMember(UsuarioDTO => UsuarioDTO.Eventos, opciones => opciones.MapFrom(MapUsuarioDTOEventos));
            CreateMap<EventoCreacionDTO, Evento>()
                .ForMember(Evento => Evento.UsuarioEvento, opciones => opciones.MapFrom(MapUsuarioEvento));
            CreateMap<Evento, EventoDTO>();
            CreateMap<CrearEvento, Evento>();
            CreateMap<Evento, EventoDTOConUsuarios>()
                .ForMember(EventoDTO => EventoDTO.Usuarios, opciones => opciones.MapFrom(MapEventoDTOUsuarios));
            CreateMap<EventoPatchDTO, Evento>().ReverseMap();
            CreateMap<Evento, GetEventoDTO>();
            CreateMap<UsuarioEvento, UsuarioEventoDTO>().ReverseMap();
            CreateMap<Evento, MostrarUsuariosEnEventoDTO>()
            .ForMember(dest => dest.UsuarioEventos, opt => opt.MapFrom(src => src.UsuarioEvento))
            .ReverseMap();
            CreateMap<MostrarUsuariosEnEventoDTO, GetEventoDTO>();
            CreateMap<UsuarioEvento, HistorialEventoDTO>()
           .ForMember(dest => dest.EventoId, opt => opt.MapFrom(src => src.EventoId))
           .ForMember(dest => dest.NombreEvento, opt => opt.MapFrom(src => src.Evento.Nombre))
           .ForMember(dest => dest.FechaEvento, opt => opt.MapFrom(src => src.Evento.Fecha))
           .ForMember(dest => dest.Asistio, opt => opt.MapFrom(src => src.Asistio));
            CreateMap<Feedback, FeedbacksDTO>().ReverseMap();

        }

        private List<EventoDTO> MapUsuarioDTOEventos(Usuario Usuario, GetUsuarioDTO GetUsuarioDTO)
        {
            var result = new List<EventoDTO>();

            if (Usuario.UsuarioEvento == null) { return result; }

            foreach (var UsuarioEvento in Usuario.UsuarioEvento)
            {
                result.Add(new EventoDTO()
                {
                    Id = UsuarioEvento.EventoId,
                    Nombre = UsuarioEvento.Evento.Nombre
                });
            }

            return result;
        }

        private List<GetUsuarioDTO> MapEventoDTOUsuarios(Evento Evento, EventoDTO EventoDTO)
        {
            var result = new List<GetUsuarioDTO>();

            if (Evento.UsuarioEvento == null)
            {
                return result;
            }

            foreach (var UsuarioEvento in Evento.UsuarioEvento)
            {
                result.Add(new GetUsuarioDTO()
                {
                    Id = UsuarioEvento.UsuarioId,
                    Nombre = UsuarioEvento.Usuario.Nombre
                });
            }

            return result;
        }

        private List<UsuarioEvento> MapUsuarioEvento(EventoCreacionDTO EventoCreacionDTO, Evento Evento)
        {
            var resultado = new List<UsuarioEvento>();

            if (EventoCreacionDTO.UsuariosIDs == null) { return resultado; }
            foreach (var UsuarioId in EventoCreacionDTO.UsuariosIDs)
            {
                resultado.Add(new UsuarioEvento() { UsuarioId = UsuarioId });
            }
            return resultado;
        }
    }
}