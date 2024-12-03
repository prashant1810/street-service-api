using NetTopologySuite.Geometries;
using PTV.Services.StreetAPI.Models;
using Street.Services.CrudAPI.Models.Dto;

namespace PTV.Services.StreetAPI.Services.Interfaces
{
    public interface IGeometryService
    {
        Task<ResponseDto> AddPointToGeometry(int id, AddPointRequest request);
    }
}
