using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIHomeWork.Models;

namespace APIHomeWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        public DepartmentsController(ContosouniversityContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartment()
        {
            return await _context.Department.ToListAsync();
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Obsolete]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            #region Old Code
            //if (id != department.DepartmentId)
            //{
            //    return BadRequest();
            //}

            //_context.Entry(department).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!DepartmentExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return NoContent();
            #endregion Old Code
            if (id != department.DepartmentId)
            {
                return BadRequest();
            }
            //get RowVersion_Original
            var data = await _context.Department.FindAsync(id);

            string TDATE = Convert.ToDateTime(department.StartDate).ToString("yyyy-MM-dd HH:mm:ss");
            int DepartmentID = department.DepartmentId;
            string Name = department.Name;
            decimal Budget = department.Budget;
            int? InstructorId = department.InstructorId;
            byte[] RowVersion = data.RowVersion;
            await _context.Database.ExecuteSqlRawAsync("exec dbo.Department_Update @DepartmentID = {0}, @Name = {1}, @Budget = {2}, @StartDate = {3}, @InstructorID = {4}, @RowVersion_Original = {5}", id, Name, Budget, TDATE, InstructorId, RowVersion);
            
            return NoContent();
        }

        // POST: api/Departments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            #region Old Code
            //_context.Department.Add(department);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetDepartment", new { id = department.DepartmentId }, department);
            #endregion Old Code

            string TDATE = Convert.ToDateTime(department.StartDate).ToString("yyyy-MM-dd HH:mm:ss");

            var data = await _context.Department.FromSqlRaw($"exec dbo.Department_Insert @Name = '{department.Name}', @Budget = {department.Budget}, @StartDate = '{TDATE}', @InstructorID = {department.InstructorId}").ToListAsync();
            department = await _context.Department.FindAsync(data[0].DepartmentId);

            return CreatedAtAction("GetDepartment", new { id = department.DepartmentId }, department);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Obsolete]
        public async Task<ActionResult<Department>> DeleteDepartment(int id)
        {
            #region Old Code
            //var department = await _context.Department.FindAsync(id);
            //if (department == null)
            //{
            //    return NotFound();
            //}

            //_context.Department.Remove(department);
            //await _context.SaveChangesAsync();

            //return department;
            #endregion Old Code
            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            byte[] row = department.RowVersion;
            await _context.Database.ExecuteSqlRawAsync("exec dbo.Department_Delete @DepartmentID = {0}, @RowVersion_Original = {1}", id, row);

            return department;
        }

        [HttpGet("~/api/DepartmentCourseCount")]
        public async Task<ActionResult<IEnumerable<VwDepartmentCourseCount>>> GetDepartmentCourseCount()
        {
            return await _context.VwDepartmentCourseCount.FromSqlRaw("Select * From dbo.vwDepartmentCourseCount").ToListAsync();
        }

        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.DepartmentId == id);
        }
    }
}
