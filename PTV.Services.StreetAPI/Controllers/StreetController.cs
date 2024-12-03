using Microsoft.AspNetCore.Mvc;
using Street.Services.CrudAPI.Models;
using PTV.Services.StreetAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http.Extensions;
using NetTopologySuite.Geometries;
using PTV.Services.StreetAPI.Models;
using PTV.Services.StreetAPI.JsonConfigData.Entities;

namespace Street.Services.CrudAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreetController : Controller
    {
        private IStreetService _streetService;
        private IGeometryService _geometryService;

        public StreetController(IStreetService streetService, IGeometryService geometryService)
        {
            _streetService = streetService;
            _geometryService = geometryService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStreet([FromBody]StreetInfo request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var events = await _streetService.CreateStreet(request);
            return Created(Request.GetDisplayUrl(), events);
        }
        [HttpGet]
        public async Task<JsonResult> GetStreets(int page = 1, int pageSize = 10)
        {
            var events = await _streetService.GetStreets(page, pageSize);
            return new JsonResult(events);
        }
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetStreet(int id, int page = 1, int pageSize = 10)
        {
            var events = await _streetService.GetStreet(id, page, pageSize);
            if (events == null)
            {
                return NotFound(BusinessTestDataConfig.Message1);
            }
            else
            {
                return new JsonResult(events);
            }

        }
        [HttpPut]
        [Route("{id:int}/geometry")]
        public async Task<IActionResult> AddPointToGeometry(int id, [FromBody] AddPointRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var events = await _geometryService.AddPointToGeometry(id, request);
            if (events == null)
            {
                return NotFound(BusinessTestDataConfig.Message1);
            }
            else
            {
                return new JsonResult(events);
            }
        }
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteStreet(int id)
        {
            var events = await _streetService.DeleteStreet(id);
            if (events == null)
            {
                return NotFound(BusinessTestDataConfig.Message1);
            }
            else
            {
                return new JsonResult(events);
            }
        }
    }
}
