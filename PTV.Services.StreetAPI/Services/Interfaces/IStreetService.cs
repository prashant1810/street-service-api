using Microsoft.AspNetCore.Mvc;
using Street.Services.CrudAPI.Models;
using Street.Services.CrudAPI.Models.Dto;

namespace PTV.Services.StreetAPI.Services.Interfaces
{
    public interface IStreetService
    {
        Task<ResponseDto> CreateStreet(StreetInfo request);
        Task<ResponseDto> GetStreets(int page, int pageSize);
        Task<ResponseDto> GetStreet(int id, int page, int pageSize);
        Task<bool> Exists(string name);
        Task<ResponseDto> DeleteStreet(int id);
    }
}
