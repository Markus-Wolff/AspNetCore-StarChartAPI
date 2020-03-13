using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = this._context.CelestialObjects.Find(id);
            if (celestialObject == null)
            {
                return NotFound();
            }
            celestialObject.Satellites = this._context.CelestialObjects.Where(e => e.OrbitedObjectId == id).ToList();
            return Ok(celestialObject);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = this._context.CelestialObjects.Where(z => z.Name == name);
            if (!celestialObjects.Any())
            {
                return NotFound();
            }
            foreach (CelestialObject celestialObject in celestialObjects)
            {
                celestialObject.Satellites = this._context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(celestialObjects);
        }

        [HttpGet("")]
        public IActionResult GetAll()
        {
            var celestialObjects = this._context.CelestialObjects.ToList();
            foreach (CelestialObject celestialObject in celestialObjects)
            {
                celestialObject.Satellites = this._context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]CelestialObject celestialObject)
        {
            var celestialObject2 = this._context.CelestialObjects.Find(id);
            if (celestialObject2 == null)
            {
                return NotFound();
            }
            else
            {
                celestialObject2.Name = celestialObject.Name;
                celestialObject2.OrbitalPeriod = celestialObject.OrbitalPeriod;
                celestialObject2.OrbitedObjectId = celestialObject.OrbitedObjectId;
                _context.CelestialObjects.Update(celestialObject2);
                _context.SaveChanges();
                return NoContent();
            }
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject2 = this._context.CelestialObjects.Find(id);
            if (celestialObject2 == null)
            {
                return NotFound();
            }
            else
            {
                celestialObject2.Name = name;
                _context.CelestialObjects.Update(celestialObject2);
                _context.SaveChanges();
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(e => e.Id == id || e.OrbitedObjectId == id);
            if (!celestialObjects.Any())
            {
                return NotFound();
            }
            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
