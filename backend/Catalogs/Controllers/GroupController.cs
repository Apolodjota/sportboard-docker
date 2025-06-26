using Microsoft.AspNetCore.Mvc;
using Catalogs.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalogs.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GroupController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los grupos.
        /// </summary>
        /// <returns>Una lista de grupos.</returns>
        // GET: api/groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroups()
        {
            return await _context.Groups.ToListAsync();
        }

        /// <summary>
        /// Obtiene un grupo por su ID.
        /// </summary>
        /// <param name="code">El Code del grupo.</param>
        /// <returns>El grupo solicitado.</returns>
        // GET: api/groups/id
        [HttpGet("{code}")]
        public async Task<ActionResult<Group>> GetGroup(string code)
        {
            var group = await _context.Groups.FindAsync(code);

            if (group == null)
            {
                return NotFound();
            }

            return group;
        }

        /// <summary>
        /// Crea un nuevo grupo.
        /// </summary>
        /// <param name="group">El grupo a crear.</param>
        /// <returns>El grupo creado.</returns>
        // POST: api/groups
        [HttpPost]
        public async Task<ActionResult<Group>> PostGroup(Group group)
        {
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroup", new { code = group.Code }, group);
        }

        /// <summary>
        /// Actualiza un grupo existente.
        /// </summary>
        /// <param name="code">El código del grupo a actualizar.</param>
        /// <param name="group">El grupo actualizado.</param>
        /// <returns>Una respuesta HTTP.</returns>
        // PUT: api/groups/code
        [HttpPut("{code}")]
        public async Task<IActionResult> PutGroup(string code, Group group)
        {
            if (code != group.Code)
            {
                return BadRequest();
            }

            _context.Entry(group).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Elimina un grupo por su código.
        /// </summary>
        /// <param name="code">El código del grupo a eliminar.</param>
        /// <returns>Una respuesta HTTP.</returns>
        // DELETE: api/groups/code
        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteGroup(string code)
        {
            var group = await _context.Groups.FindAsync(code);
            if (group == null)
            {
                return NotFound();
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Obtiene todos los catálogos asociados a un grupo específico.
        /// </summary>
        /// <param name="groupCode">El código del grupo.</param>
        /// <returns>Una lista de catálogos asociados al grupo.</returns>
        // GET: api/groups/{groupCode}/catalogs
        [HttpGet("{code}/catalogs")]
        public async Task<ActionResult<IEnumerable<Catalog>>> GetCatalogsByGroup(string code)
        {
            var group = await _context.Groups.FindAsync(code);

            if (group == null)
            {
                return NotFound();
            }

            var catalogs = await _context.Catalogs
                .Where(c => c.GroupCode == code)
                .Select(c => new Catalog
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    GroupCode = c.GroupCode,
                    IdCatalog = c.IdCatalog,
                    IsActive = c.IsActive,
                })
                .ToListAsync();

            return Ok(catalogs);
        }
    }
}