using PTV.Services.StreetAPI.Services.Interfaces;
using Street.Services.CrudAPI.Database;
using Street.Services.CrudAPI.Models.Dto;
using Street.Services.CrudAPI.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PTV.Services.StreetAPI.JsonConfigData.Entities;

namespace PTV.Services.StreetAPI.Services.Repository
{
    public class StreetService : IStreetService
    {
        private AppDbContext _context;
        private IMapper _mapper;
        private ResponseDto _response;
        private readonly ILogger<StreetService> _logger;
        public StreetService(AppDbContext context, IMapper mapper, ILogger<StreetService> logger)
        {
            _context = context;
            _mapper = mapper;
            _response = new ResponseDto();
            _logger = logger;
        }

        public async Task<ResponseDto> CreateStreet(StreetInfo street)
        {
            _logger.LogInformation("Creating a new street with name {StreetName}", street.Name);
            try
            {
                if (await Exists(street.Name))
                {
                    _response.IsSuccess = false;
                    _response.Message = BusinessTestDataConfig.Message2;
                    return _response;

                }
                _context.Add(street);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully created street with ID {StreetId}", street.Id);

                _response.Result = street;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a street");
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }
        public async Task<ResponseDto> GetStreets(int page, int pageSize)
        {
            try
            {
                _logger.LogInformation("Fetching all streets from the database");
                var streets = await _context.Streets.ToListAsync();
                _logger.LogInformation("Successfully fetched streets");

                _response.Result = streets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching streets");
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }
        public async Task<ResponseDto> GetStreet(int id, int page, int pageSize)
        {
            _logger.LogInformation("Fetching street with ID {StreetId}", id);
            try
            {
                var street = await _context.Streets.FirstOrDefaultAsync(p => p.Id == id);
                if (street == null)
                {
                    _logger.LogWarning("Street with ID {StreetId} not found", id);
                    _response.IsSuccess = false;
                    _response.Message = BusinessTestDataConfig.Message5;
                }
                _response.Result = street;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching street with ID {StreetId}", id);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;

        }
        public async Task<bool> Exists(string name)
        {
            _logger.LogInformation("Searching street with Name {StreetName}", name);
            try
            {
                var street = await _context.Streets.FirstOrDefaultAsync(p => p.Name == name);
                if (street == null)
                {
                    _logger.LogWarning("Street with Name {StreetName} not found for deletion", name);
                    return false;
                }
                _logger.LogInformation("Successfully selected street with Name {StreetName}", name);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting street with Name {StreetName}", name);
                return false;
            }
        }
        public async Task<ResponseDto> DeleteStreet(int id)
        {
            _logger.LogInformation("Deleting street with ID {StreetId}", id);
            try
            {
                var street = await _context.Streets.FirstOrDefaultAsync(p => p.Id == id);

                if (street is null)
                {
                    _logger.LogWarning("Street with ID {StreetId} not found for deletion", id);
                    _response.IsSuccess = false;
                    _response.Message = BusinessTestDataConfig.Message5;
                    return _response;
                }

                _context.Streets.Remove(street);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully deleted street with ID {StreetId}", id);

                _response.IsSuccess = true;
                _response.Message = BusinessTestDataConfig.Message3;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting street with ID {StreetId}", id);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }
    }
}
