using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    [Authorize(Policy = "MustBeFromAntwerp")]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly CitiesDataStore _citiesDataStore;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, CitiesDataStore citiesDataStore,
            ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            ////try
            ////{
            ////    var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
            ////    if (city == null)
            ////    {
            ////        _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest");
            ////        return NotFound();
            ////    }

            ////    return Ok(city.PointOfInterest);
            ////}
            ////catch (Exception ex)
            ////{
            ////    _logger.LogCritical(
            ////            $"Exception while getting points of interest for city with city Id {cityId}",
            ////            ex
            ////        );
            ////    return StatusCode(StatusCodes.Status500InternalServerError, "A Problem happend while handling your request");
            ////}
            ///

            var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

            if (!await _cityInfoRepository.CityNameMatchesCityId(cityName, cityId))
            {
                return Forbid();
            }
            
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} wasn't found when accessing point of interest"
                    );
                return NotFound();
            }
            var pointsOfInterestForCity = await _cityInfoRepository.GetPointOfInterestsForCityAsync(cityId);
            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
        }

        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId) 
        {
            //var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
            //if (city == null)
            //{
            //    return NotFound();
            //}

            //// find point of interest
            //var pointOfInterest = city.PointOfInterest
            //                            .FirstOrDefault(x => x.Id == pointOfInterestId);
            //if (pointOfInterest == null)
            //{
            //    return NotFound();
            //}
            //return Ok(pointOfInterest);

            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository
                                    .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePointOfInterest(int cityId, 
                            PointOfInterestCreateDto newPointOfInterestData)
        {
            //var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            //if ( city == null)
            //{
            //    return NotFound();
            //}

            //var maxIdOfPointOfInterest = _citiesDataStore.Cities.SelectMany(x => x.PointOfInterest).Max(x => x.Id);

            //PointOfInterestDto newPointsOfInterest = new PointOfInterestDto()
            //{
            //    Id = ++maxIdOfPointOfInterest,
            //    Name = newPointOfInterestData.Name,
            //    Description = newPointOfInterestData.Description,
            //};

            //city.PointOfInterest.Add(newPointsOfInterest);
            //return CreatedAtRoute("GetPointOfInterest", new
            //{
            //    CityId = cityId,
            //    pointOfInterestId = newPointsOfInterest.Id,
            //}, newPointsOfInterest);

            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var finalPointOfInterest = _mapper.Map<PointOfInterest>(newPointOfInterestData);
            await _cityInfoRepository.AddPointOfInterestForCityAsync (cityId, finalPointOfInterest);
            await _cityInfoRepository.SaveChangesAsync();
            var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = createdPointOfInterestToReturn.Id
                }, createdPointOfInterestToReturn);
        }

        [HttpPut("{pointOfInterestId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId,
                        PointOfInterestForUpdateDto pointOfInterest)
        {
            //var city = _citiesDataStore.Cities
            //            .FirstOrDefault(c => c.Id == cityId);

            //if (city == null)
            //{
            //    return NotFound();
            //}

            //var pointOfInterestFromStore = city.PointOfInterest
            //                        .FirstOrDefault(x => x.Id == pointOfInterestId);
            //if (pointOfInterestFromStore == null) 
            //{
            //    return NotFound();
            //}

            //pointOfInterestFromStore.Name = pointOfInterest.Name;
            //pointOfInterestFromStore.Description = pointOfInterest.Description;

            //return NoContent();

            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }
            _mapper.Map(pointOfInterest, pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{pointofinterestid}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointofinterestid,
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            #region In Memory Region
            //var city = _citiesDataStore.Cities
            //           .FirstOrDefault(c => c.Id == cityId);

            //if (city == null)
            //{
            //    return NotFound();
            //}

            //var pointOfInterestFromStore = city.PointOfInterest
            //                        .FirstOrDefault(x => x.Id == pointofinterestid);
            //if (pointOfInterestFromStore == null)
            //{
            //    return NotFound();
            //}

            //var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            //{
            //    Name = pointOfInterestFromStore.Name,
            //    Description = pointOfInterestFromStore.Description
            //};

            //patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}

            //if (!TryValidateModel(pointOfInterestToPatch))
            //{
            //    return BadRequest(ModelState);
            //}
            //pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            //pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            //return NoContent();

            #endregion

            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointofinterestid);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }
            var pointOfInterestEntityToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);
            patchDocument.ApplyTo(pointOfInterestEntityToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestEntityToPatch))
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(pointOfInterestEntityToPatch, pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{pointofinterestid}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            #region In Memory Region
            var city = _citiesDataStore.Cities
                        .FirstOrDefault(c => c.Id == cityId);
            if (city == null)
                //{
                //    return NotFound();
                //}

                //var pointOfInterestFromStore = city.PointOfInterest
                //                                .FirstOrDefault(c => c.Id ==  pointOfInterestId);

                //if (pointOfInterestFromStore == null)
                //{
                //    return NotFound();
                //}

                //city.PointOfInterest.Remove(pointOfInterestFromStore);
                //_mailService.send(
                //    "Points of interest deleted",
                //    $"Points of interest {pointOfInterestFromStore.Name} with id {pointOfInterestId}"
                //    );
                //return NoContent();
                #endregion

            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }
            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
            _mailService.send(
                "Point of interest deleted.",
                $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} has been deleted"
                );
            return NoContent();
        }

    }
}
