using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiEventos.Entidades;
using WebApiEventos.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.HttpSys;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
    [ApiController] //validaciones automaticas ponerselo a cada controller
    [Route("eventos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class EventoController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IService service;
        private readonly IConfiguration configuration;
        private readonly UserManager<IdentityUser> userManager;

        public EventoController(ApplicationDbContext context, IService service, IMapper mapper, IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            this.dbContext = context;
            this.mapper = mapper;
            this.configuration = configuration;
            this.userManager = userManager;
            this.service = service;
        }

        [AllowAnonymous]
        [HttpGet("/listadoEventos")]
        public async Task<ActionResult<List<MostrarUsuariosEnEventoDTO>>> GetAll()
        {
            var eventos = await dbContext.Eventos.Include(e => e.UsuarioEvento).ToListAsync();

            var eventosDTO = mapper.Map<List<MostrarUsuariosEnEventoDTO>>(eventos);
            return Ok(eventosDTO);
        }

        [AllowAnonymous]
        [HttpGet("/listadoEventosDestacados")]
        public async Task<ActionResult<List<MostrarUsuariosEnEventoDTO>>> Get()
        {
            var eventos = await dbContext.Eventos
                .Where(e => e.CapacidadMaximaAsistentes / 2 <= e.UsuarioEvento.Count) 
                .Include(e => e.UsuarioEvento)
                .ToListAsync();

            var eventosDTO = mapper.Map<List<MostrarUsuariosEnEventoDTO>>(eventos);
            return Ok(eventosDTO);
        }

        [AllowAnonymous]
        [HttpGet("ObtenerEventosPorid/{Id}", Name = "obtenerEvento")]
        public async Task<ActionResult<GetEventoDTO>> GetById([FromRoute] int Id)
        {
            var evento = await dbContext.Eventos.FindAsync(Id);

            if (evento == null)
            {
                return NotFound();
            }

            return mapper.Map<GetEventoDTO>(evento);
        }

        [AllowAnonymous]
        [HttpGet("ObtenerEventosPorNombre/{Nombre}")]

        public async Task<ActionResult<List<GetEventoDTO>>> GetByNombre([FromRoute] string Nombre)
        {
            var eventos = await dbContext.Eventos.Where(EventoDB => EventoDB.Nombre.Contains(Nombre)).ToListAsync();

            return mapper.Map<List<GetEventoDTO>>(eventos);

        }

        [AllowAnonymous]
        [HttpGet("ObtenerEventosPorubicacion/{Ubicacion}")]
        public async Task<ActionResult<List<GetEventoDTO>>> GetByUbicacion([FromRoute] string Ubicacion)
        {
            var eventos = await dbContext.Eventos.Where(EventoDB => EventoDB.Ubicacion.Contains(Ubicacion)).ToListAsync();

            return mapper.Map<List<GetEventoDTO>>(eventos);

        }

        [AllowAnonymous]
        [HttpGet("ObtenerEventosPorFecha/{Fecha}")]
        public async Task<ActionResult<List<GetEventoDTO>>> Get([FromRoute] string Fecha)
        {
            var fechaConsulta = DateTime.Parse(Fecha);
            var eventos = await dbContext.Eventos.Where(EventoDB => EventoDB.Fecha.Equals(fechaConsulta)).ToListAsync();

            return mapper.Map<List<GetEventoDTO>>(eventos);

        }

        [HttpPost("/PublicarUnEvento")]
        public async Task<ActionResult> Post(CrearEvento eventoCreacionDTO)
        {
            if (eventoCreacionDTO.OrganizadorID == 0)
            {
                return BadRequest("El organizadorID no puede ser 0.");
            }

            var organizador = await dbContext.Usuarios.FindAsync(eventoCreacionDTO.OrganizadorID);
            if (organizador == null)
            {
                return BadRequest("El organizadorID no es válido.");
            }


            var evento = mapper.Map<Evento>(eventoCreacionDTO);

            OrdenarPorUsuarios(evento);

            dbContext.Add(evento);
            await dbContext.SaveChangesAsync();

            var eventoDTO = mapper.Map<EventoDTO>(evento);

            return CreatedAtRoute("obtenerEvento", new { id = evento.Id }, eventoDTO);
        }

        private void OrdenarPorUsuarios(Evento evento)
        {
            if (evento.UsuarioEvento != null)
            {
                for (int i = 0; i < evento.UsuarioEvento.Count; i++)
                {
                    evento.UsuarioEvento[i].Orden = i;
                }
            }
        }


        [HttpPut("ActualizarEvento/{id:int}")]
        public async Task<ActionResult> Put(int id, int organizadorID, CrearEvento eventoCreacionDTO)
        {
            if (eventoCreacionDTO.OrganizadorID == 0)
            {
                return BadRequest("El organizadorID no puede ser 0.");
            }

            var eventoDB = await dbContext.Eventos
                .Include(x => x.UsuarioEvento)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (eventoDB == null)
            {
                return NotFound();
            }

            if (eventoDB.OrganizadorID != organizadorID)
            {
                return NotFound();
            }

            mapper.Map(eventoCreacionDTO, eventoDB);

            OrdenarPorUsuarios(eventoDB);

            await dbContext.SaveChangesAsync();
            return NoContent();
        }


        [HttpPatch("PatchEvento/{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<EventoPatchDTO> patchDocument)
        {
            if (patchDocument == null) { return BadRequest(); }

            var eventoDB = await dbContext.Eventos.FirstOrDefaultAsync(x => x.Id == id);

            if (eventoDB == null) { return NotFound(); }

            var eventoDTO = mapper.Map<EventoPatchDTO>(eventoDB);

            patchDocument.ApplyTo(eventoDTO);

            var isValid = TryValidateModel(eventoDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(eventoDTO, eventoDB);

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        [HttpDelete("BorrarEvento/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await dbContext.Eventos.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound("El Evento no fue encontrado.");
            }

            dbContext.Remove(new Evento { Id = id });
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("EventosDeUnOrganizador/{organizadorId}")]
        public async Task<ActionResult<List<GetEventoDTO>>> GetByOrganizadorId(int organizadorId)
        {
            var usuarioExistente = await dbContext.Usuarios.AnyAsync(u => u.Id == organizadorId);
            if (!usuarioExistente)
            {
                return NotFound("El organizadorID no existe.");
            }

            var eventos = await dbContext.Eventos
                .Where(e => e.OrganizadorID == organizadorId)
                .ToListAsync();

            if (eventos.Count == 0)
            {
                return NotFound("No se encontraron eventos para el organizador especificado.");
            }

            return mapper.Map<List<GetEventoDTO>>(eventos);
        } 

        [HttpPost("{id}/PublicarComentarios")]
        public async Task<ActionResult> AgregarComentario(int id, [FromBody] string comentario)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;

            var evento = await dbContext.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }
            var feedbackDto = new FeedbacksDTO
            {
                Comentario = comentario,
                EventoId = id,
                UsuarioId = usuarioId
            };

            var feedback = mapper.Map<Feedback>(feedbackDto);
            evento.Feedbacks.Add(feedback);

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{id}/ObtenerFeedbacks")]
        public ActionResult<List<FeedbacksDTO>> ObtenerFeedbacks(int id)
        {
            var evento = dbContext.Eventos.Include(e => e.Feedbacks).FirstOrDefault(e => e.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            var feedbacksDTO = mapper.Map<List<FeedbacksDTO>>(evento.Feedbacks);

            return feedbacksDTO;
        }

        [HttpPost("/AsigarCodigosPromocionales")]
        public async Task<ActionResult> GenerarCodigosPromocionales(int eventoId, int organizadorId)
        {
            var evento = await dbContext.Eventos.Include(e => e.UsuarioEvento).ThenInclude(ue => ue.Usuario)
                                                .FirstOrDefaultAsync(e => e.Id == eventoId);
            if (evento == null)
            {
                return NotFound();
            }

            if (evento.OrganizadorID != organizadorId)
            {
                return BadRequest("El organizadorID no coincide con el organizador del evento.");
            }

            var usuariosRegistrados = evento.UsuarioEvento.Select(ue => ue.Usuario).ToList();

            foreach (var usuario in usuariosRegistrados)
            {
                var codigoPromocional = service.GetTransient();
                var codigoPromocionalEntity = new CodigoPromocional { Codigo = codigoPromocional.ToString() };
                dbContext.CodigosPromocionales.Add(codigoPromocionalEntity);

                usuario.CodigosPromocionales.Add(codigoPromocionalEntity);
            }

            await dbContext.SaveChangesAsync();

            return Ok("Códigos promocionales generados y asignados exitosamente.");
        }

    }
}
