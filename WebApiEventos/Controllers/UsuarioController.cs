using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApiEventos.DTO;
using WebApiEventos.Entidades;
using WebApiEventos.Filtros;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
    [ApiController] 
    [Route("usuarios")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IService service;
        private readonly ServiceTransient serviceTransient;
        private readonly ServiceScoped serviceScoped;
        private readonly ServiceSingleton serviceSingleton;
        private readonly ILogger<UsuarioController> logger;
        private readonly IWebHostEnvironment env;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
    

        public UsuarioController(ApplicationDbContext context, IService service,
            ServiceTransient serviceTransient, ServiceScoped serviceScoped,
            ServiceSingleton serviceSingleton, ILogger<UsuarioController> logger,
            IWebHostEnvironment env, IMapper mapper, IConfiguration configuration
            )
        {
            this.dbContext = context;
            this.service = service;
            this.serviceTransient = serviceTransient;
            this.serviceScoped = serviceScoped;
            this.serviceSingleton = serviceSingleton;
            this.logger = logger;
            this.env = env;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpGet("GUID")]
        [AllowAnonymous]
        [ResponseCache(Duration = 10)]
        [ServiceFilter(typeof(FiltroDeAccion))]
        public ActionResult ObtenerGuid()
        {
            
            logger.LogInformation("Durante la ejecucion");
            return Ok(new
            {
                UsuariosControllerTransient = serviceTransient.guid,
                ServiceA_Transient = service.GetTransient(),
                UsuariosControllerScoped = serviceScoped.guid,
                ServiceA_Scoped = service.GetScoped(),
                UsuariosControllerSingleton = serviceSingleton.guid,
                ServiceA_Singleton = service.GetSingleton()
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<GetUsuarioDTO>>> Get()
        {
            logger.LogInformation("Se obtiene el listado de usuarios");
            logger.LogWarning("Mensaje de prueba warning");
            service.EjecutarJob();
            var usuarios = await dbContext.Usuarios.ToListAsync();
            return mapper.Map<List<GetUsuarioDTO>>(usuarios);
        }

        [AllowAnonymous]
        [HttpGet("id/{id}", Name = "obtenerusuario")]
        public async Task<ActionResult<GetUsuarioDTO>> GetById(int id)
        {
            var usuario = await dbContext.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            var usuarioDTO = mapper.Map<GetUsuarioDTO>(usuario);
            return usuarioDTO;
        }

        [AllowAnonymous]
        [HttpGet("RecordatorioEvento/{id:int}", Name = "obtenerEventoRecordatorio")]
        public async Task<ActionResult<MostrarUsuariosEnEventoDTO>> GetRecordatorio(int id)
        {
            DateTime fechaActual = DateTime.Now;
            var evento = await dbContext.Eventos
                .Where(e => e.Fecha > fechaActual && e.UsuarioEvento.Any(u => u.UsuarioId == id))
                .Include(eventoDB => eventoDB.UsuarioEvento)
                .ThenInclude(usuarioEventoDB => usuarioEventoDB.Usuario)
                .ToListAsync();

            if (evento == null)
            {
                return NotFound();
            }

            var eventosDTO = mapper.Map<List<MostrarUsuariosEnEventoDTO>>(evento);
            var otroDTO = mapper.Map<List<GetEventoDTO>>(eventosDTO);
            return Ok(otroDTO);
        }

        [AllowAnonymous]
        [HttpGet("{Nombre}")]
        public async Task<ActionResult<List<GetUsuarioDTO>>> Get([FromRoute] string Nombre)
        {
            var usuarios = await dbContext.Usuarios.Where(UsuarioDB => UsuarioDB.Nombre.Contains(Nombre)).ToListAsync();

            return mapper.Map<List<GetUsuarioDTO>>(usuarios);

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UsuarioDTO usuarioDto)
        {
         

            var existeUsuarioMismoNombre = await dbContext.Usuarios.AnyAsync(x => x.Nombre == usuarioDto.Nombre);

            if (existeUsuarioMismoNombre)
            {
                return BadRequest($"Ya existe un usuario con el nombre {usuarioDto.Nombre}");
            }

            var usuario = mapper.Map<Usuario>(usuarioDto);

            dbContext.Add(usuario);
            await dbContext.SaveChangesAsync();

            var usuarioDTO = mapper.Map<GetUsuarioDTO>(usuario);

            return CreatedAtRoute("obtenerusuario", new {id = usuario.Id}, usuarioDTO);
        }

        [HttpPut("Id/{id:int}")] 
        public async Task<ActionResult> Put(UsuarioDTO usuarioCreacionDTO, int id)
        {
            var exist = await dbContext.Usuarios.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            var usuario = mapper.Map<Usuario>(usuarioCreacionDTO);
            usuario.Id = id;

            dbContext.Update(usuario);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("usuario/{usuarioId}")]
        public async Task<IActionResult> EliminarUsuario(int usuarioId)
        {
            var usuario = await dbContext.Usuarios
                .Include(u => u.EventosFavoritos)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            // Eliminar la relación con los eventos favoritos
            usuario.EventosFavoritos.Clear();

            dbContext.Usuarios.Remove(usuario);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("{usuarioId}/eventos/{eventoId}/registro")]
        public async Task<ActionResult> Post(int usuarioId, int eventoId)
        {
            var usuario = await dbContext.Usuarios.FindAsync(usuarioId);
            var evento = await dbContext.Eventos.Include(e => e.UsuarioEvento).FirstOrDefaultAsync(e => e.Id == eventoId);

            if (evento == null)
            {
                return NotFound(); // Manejar el caso cuando el evento no existe
            }

            if (!evento.UsuarioEvento.Any(ue => ue.UsuarioId == usuarioId))
            {
                if (evento.UsuarioEvento.Count < evento.CapacidadMaximaAsistentes)
                {
                    var usuarioEvento = new UsuarioEvento
                    {
                        UsuarioId = usuarioId,
                        EventoId = eventoId,
                        Orden = evento.UsuarioEvento.Count + 1,
                        FechaRegistro = DateTime.Now // Obtiene la fecha actual
                    };

                    dbContext.UsuarioEvento.Add(usuarioEvento);

                    await dbContext.SaveChangesAsync();

                    var usuarioEventoDto = mapper.Map<UsuarioEventoDTO>(usuarioEvento);
                    var eventoDto = mapper.Map<EventoDTO>(evento);

                    return Ok(new {Evento = eventoDto, UsuarioEvento = usuarioEventoDto }); 
                }
                else
                {
                    return BadRequest("No hay lugares disponibles en el evento."); 
                }
            }
            else
            {
                return BadRequest("El usuario ya está registrado en el evento."); 
            }
        }


        [HttpPost("{eventoId}/asistencia/{usuarioId}")]
        public async Task<IActionResult> MarcarAsistencia(int eventoId, int usuarioId)
        {
            var evento = await dbContext.Eventos.FindAsync(eventoId);
            var usuario = await dbContext.Usuarios.FindAsync(usuarioId);

            if (evento == null || usuario == null)
            {
                return NotFound();
            }

            var usuarioEvento = await dbContext.UsuarioEvento
                .FirstOrDefaultAsync(ue => ue.EventoId == eventoId && ue.UsuarioId == usuarioId);

            if (usuarioEvento == null)
            {
                usuarioEvento = new UsuarioEvento
                {
                    Evento = evento,
                    Usuario = usuario,
                    Orden = 0, 
                    Asistio = true
                };
                dbContext.UsuarioEvento.Add(usuarioEvento);
            }
            else
            {
                usuarioEvento.Asistio = true;
            }

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("usuario/{usuarioId}/eventos-asistidos")]
        public async Task<IActionResult> ObtenerEventosAsistidos(int usuarioId)
        {
            var usuario = await dbContext.Usuarios.FindAsync(usuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            var eventosAsistidos = await dbContext.UsuarioEvento
                .Where(ue => ue.UsuarioId == usuarioId)
                .Include(ue => ue.Evento)
                .ToListAsync();

            var eventosDTO = mapper.Map<List<HistorialEventoDTO>>(eventosAsistidos);

            return Ok(eventosDTO);
        }
        [AllowAnonymous]
        [HttpPost("{usuarioId}/eventos-favoritos/{eventoId}")]
        public async Task<IActionResult> AgregarEventoFavorito(int usuarioId, int eventoId)
        {
            var usuario = await dbContext.Usuarios.FindAsync(usuarioId);
            var evento = await dbContext.Eventos.FindAsync(eventoId);

            if (usuario == null || evento == null)
            {
                return NotFound();
            }

            usuario.EventosFavoritos.Add(evento);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
        [AllowAnonymous]
        [HttpGet("{usuarioId}/eventos-favoritos")]
        public async Task<IActionResult> ObtenerEventosFavoritos(int usuarioId)
        {
            var usuario = await dbContext.Usuarios.Include(u => u.EventosFavoritos).FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            var eventosFavoritosDTO = mapper.Map<List<EventoDTO>>(usuario.EventosFavoritos);

            return Ok(eventosFavoritosDTO);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<UsuarioPatchDTO> patchDocument)
        {
            if (patchDocument == null) { return BadRequest(); }

            var usuarioDB = await dbContext.Usuarios.FirstOrDefaultAsync(x => x.Id == id);

            if (usuarioDB == null) { return NotFound(); }

            var usuarioDTO = mapper.Map<UsuarioPatchDTO>(usuarioDB);

            patchDocument.ApplyTo(usuarioDTO);

            var isValid = TryValidateModel(usuarioDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(usuarioDTO, usuarioDB);

            await dbContext.SaveChangesAsync();
            return NoContent();
        }
        [AllowAnonymous]
        [HttpGet("usuario/{id}/codigospromocionales")]
        public async Task<ActionResult<List<CodigoPromocionalDTO>>> GetCodigosPromocionales(int id)
        {
            var usuario = await dbContext.Usuarios.Include(u => u.CodigosPromocionales)
                                                  .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            var codigosPromocionales = usuario.CodigosPromocionales
                                             .Select(codigo => new CodigoPromocionalDTO { Id = codigo.Id, Codigo = codigo.Codigo })
                                             .ToList();

            return codigosPromocionales;
        }
    }
}
