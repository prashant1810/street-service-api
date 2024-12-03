using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Npgsql;
using PTV.Services.StreetAPI.JsonConfigData.Entities;
using PTV.Services.StreetAPI.Models;
using PTV.Services.StreetAPI.Services.Interfaces;
using RedLockNet;
using Street.Services.CrudAPI.Database;
using Street.Services.CrudAPI.Models.Dto;

namespace PTV.Services.StreetAPI.Services.Repository
{
    public class GeometryService: IGeometryService
    {
        private AppDbContext _context;
        private ResponseDto _response;
        private readonly GeometryFactory _geometryFactory;
        private readonly IDistributedLockFactory _LockFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<GeometryService> _logger;

        public GeometryService(AppDbContext context, GeometryFactory geometryFactory, IDistributedLockFactory lockFactory, 
            IConfiguration config, ILogger<GeometryService> logger)
        {
            _context = context;
            _response = new ResponseDto();
            _geometryFactory = geometryFactory;
            _LockFactory = lockFactory;
            _config = config;
            _logger = logger;
        }

        public async Task<ResponseDto> AddPointToGeometry(int id, AddPointRequest request)
        {
            _logger.LogInformation("Received request to add a point to street with ID: {StreetId}", id);

            using var transaction = await _context.Database.BeginTransactionAsync();

            var lockKey = $"street-{id}-lock";
            using var _lock = await _LockFactory.CreateLockAsync(lockKey, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1));

            if (!_lock.IsAcquired)
            {
                _response.IsSuccess = false;
                _response.Message = BusinessTestDataConfig.Message4;
            }
            try
            {
                var street = await _context.Streets.FindAsync(id);

                if (street is null)
                {
                    _logger.LogWarning("Street with ID: {StreetId} not found", id);
                    _response.IsSuccess = false;
                    _response.Message = BusinessTestDataConfig.Message5;
                    return _response;
                }

                if (street.Geometry == null)
                {
                    _logger.LogWarning("Street with ID: {StreetId} has no geometry data", id);
                    _response.IsSuccess = false;
                    _response.Message = BusinessTestDataConfig.Message5;
                    return _response;
                }

                if (_config.GetValue<bool>("FeatureFlags:UsePostGIS"))
                {
                    await AddPointToGeometryInDatabaseAsync(id, request);

                    _response.IsSuccess = true;
                    _response.Message = BusinessTestDataConfig.Message6;

                }
                else
                {
                    var currentGeometry = street.Geometry;
                    if (currentGeometry == null || currentGeometry.Coordinates.Length == 0)
                    {
                        _response.IsSuccess = false;
                        _response.Message = BusinessTestDataConfig.Message7;
                        return _response;
                    }
                    if (currentGeometry == null || currentGeometry.GeometryType != "LineString")
                    {
                        _response.IsSuccess = false;
                        _response.Message = BusinessTestDataConfig.Message8;
                        return _response;
                    }
                    if (currentGeometry == null || street.Capacity < 0)
                    {
                        _response.IsSuccess = false;
                        _response.Message = BusinessTestDataConfig.Message9;
                        return _response;
                    }

                    _logger.LogDebug("Existing geometry for street ID {StreetId}: {Geometry}", id, street.Geometry);

                    var coordinates = currentGeometry.Coordinates.ToList();

                    var calPoint = new Point(request.Longitude, request.Latitude);

                    if (IsPointCloserToStart(street.Geometry, calPoint))
                    {
                        coordinates.Insert(0, calPoint.Coordinate);
                        _logger.LogDebug("Point {Point} added to the start of the geometry", calPoint);

                    }
                    else
                    {
                        coordinates.Add(calPoint.Coordinate);
                        _logger.LogDebug("Point {Point} added to the end of the geometry", calPoint);

                    }

                    street.Geometry = _geometryFactory.CreateLineString(coordinates.ToArray());


                    _context.Streets.Update(street);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation("Successfully updated geometry for street ID: {StreetId}", id);

                    _response.IsSuccess = true;
                    _response.Message = BusinessTestDataConfig.Message10;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a point to the geometry for street ID: {StreetId}", id);

                await transaction.RollbackAsync();
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }
        private async Task<bool> AddPointToGeometryInDatabaseAsync(int id, AddPointRequest point)
        {
            var newPointWKT = $"POINT({point.Longitude} {point.Latitude})";
            var query = @"
            UPDATE Streets
            SET Geometry = CASE
                WHEN ST_Distance(ST_StartPoint(Geometry), ST_GeomFromText(@newPointWKT, 4326)) <
                     ST_Distance(ST_EndPoint(Geometry), ST_GeomFromText(@newPointWKT, 4326))
                THEN ST_AddPoint(Geometry, ST_GeomFromText(@newPointWKT, 4326), 0)
                ELSE ST_AddPoint(Geometry, ST_GeomFromText(@newPointWKT, 4326))
            END
            WHERE Id = @streetId";

            var parameters = new[]
            {
                new NpgsqlParameter("@newPointWKT", newPointWKT),
                new NpgsqlParameter("@streetId", id)
            };
            _logger.LogDebug("Executing database query to update geometry for street ID {StreetId}: {Query}", id, query);

            try
            {
                var result = await _context.Database.ExecuteSqlRawAsync(query, parameters);
                if (result > 0)
                {
                    _logger.LogInformation("Database geometry update successful for street ID {StreetId}", id);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Database update returned no affected rows for street ID {StreetId}", id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing database query for street ID {StreetId}", id);
                return false;
            }
        }
        private bool IsPointCloserToStart(LineString geometry, Point point)
        {
            var firstCoordinate = geometry.Coordinates.First();
            var lastCoordinate = geometry.Coordinates.Last();

            var distanceToStart = CalculateDistance(firstCoordinate.Y, firstCoordinate.X, point.Y, point.X);
            var distanceToEnd = CalculateDistance(lastCoordinate.Y, lastCoordinate.X, point.Y, point.X);

            return distanceToStart < distanceToEnd;
        }
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = lat2 - lat1;
            var dLon = lon2 - lon1;
            return Math.Sqrt(dLat * dLat + dLon * dLon);
        }
    }
}
