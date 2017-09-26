using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using AutoMapper;
using MyCodeCamp.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCodeCamp.Controllers
{
    [Route("api/camps")]
    public class CampsController : Controller
    {
        private ICampRepository _repo;
        private IMapper _mapper;
        public CampsController(ICampRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public IActionResult Get()
        {
            var camps = _repo.GetAllCamps();

            return Ok(_mapper.Map<IEnumerable<CampModel>>(camps));
        }

        [HttpGet("{id}", Name = "CampGet")]
        public IActionResult Get(int id, bool includeSpeakers = false)
        {
            try
            {
                Camp camp = null;

                if (includeSpeakers) camp = _repo.GetCampWithSpeakers(id);
                else camp = _repo.GetCamp(id);

                if (camp == null) return NotFound($"Camp {id} was not found");
                return Ok(_mapper.Map<CampModel>(camp, opt => opt.Items["UrlHelper"] = this.Url));
            }
            catch
            {

            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Camp model)
        {
            try
            {
                _repo.Add(model);
                if (await _repo.SaveAllAsync())
                {
                    var newUri = Url.Link("CampGet", new { id = model.Id });
                    return Created(newUri, model);
                }
            }
            catch(Exception ex)
            {
                var e = ex;
            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Camp model)
        {
            try
            {
                var oldCamp = _repo.GetCamp(id);
                if (oldCamp == null) return NotFound($"Could not find a camp with ID: {id}");

                oldCamp.Name = model.Name ?? oldCamp.Name;
                oldCamp.Description = model.Description ?? oldCamp.Description;
                oldCamp.Location = model.Location ?? oldCamp.Location;
                oldCamp.Length = model.Length > 0 ? model.Length : oldCamp.Length;
                oldCamp.EventDate = model.EventDate != DateTime.MinValue ? model.EventDate : oldCamp.EventDate;

                if(await _repo.SaveAllAsync())
                {
                    return Ok(oldCamp);
                }

            }
            catch(Exception ex)
            {

            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var oldCamp = _repo.GetCamp(id);
                if (oldCamp == null) return NotFound($"Could not find a camp with ID: {id}");

                _repo.Delete(oldCamp);

                if (await _repo.SaveAllAsync())
                {
                    return Ok(oldCamp);
                }
            }
            catch
            {

            }
            return BadRequest();
        }
    }
}
