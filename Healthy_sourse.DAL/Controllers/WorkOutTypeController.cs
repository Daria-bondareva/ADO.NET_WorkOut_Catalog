using Healthy_sourse_norm.Entities;
using Healthy_sourse_norm.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Healthy_sourse.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class WorkOutTypeController : ControllerBase
    {
        private readonly ILogger<WorkOutTypeController> _logger;

        private IUnitOfWork _unitofWork;

        public WorkOutTypeController(ILogger<WorkOutTypeController> logger, IUnitOfWork unitofWork)
        {
            _logger = logger;
            _unitofWork = unitofWork;
        }

        [HttpGet("GetAllTypes")] //+
        public async Task<ActionResult<IEnumerable<WorkOutType>>> GetAllTypesAsync()
        {
            try
            {
                var results = await _unitofWork._workoutTypeRepository.GetAllAsync();
                _unitofWork.Commit();
                _logger.LogInformation($"Returned all types from database.");
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Transaction Failed! Something went wrong inside GetAllTypesAsync() action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }



        [HttpGet("GetById/{Id}", Name = "GetTypeById")] //+
        public async Task<ActionResult<WorkOutType>> GetByIdAsync(int id)
        {
            try
            {
                var result = await _unitofWork._workoutTypeRepository.GetByIdAsync(id);
                _unitofWork.Commit();
                if (result == null)
                {
                    _logger.LogError($"Post with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInformation($"Returned post with id: {id}");
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetByIdAsync() action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }


        
        [HttpGet("TypeByName")]
        public async Task<ActionResult<IEnumerable<WorkOutType>>> TypeByNameAsync(string type)
        {
            try
            {
                var results = await _unitofWork._workoutTypeRepository.GetTypeByNameAsync(type);
                _unitofWork.Commit();
                _logger.LogInformation($"Returned name of type from database.");
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside TypeByNameAsync() action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost("PostType")]
        public async Task<ActionResult> PostTypeAsync([FromBody] WorkOutType newType)
        {
            try
            {
                if (newType == null)
                {
                    _logger.LogError("Type object sent from client is null.");
                    return BadRequest("Type object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid type object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var created_id = await _unitofWork._workoutTypeRepository.AddAsync(newType);
                var CreatedType = await _unitofWork._workoutTypeRepository.GetByIdAsync(created_id);
                _unitofWork.Commit();
                return CreatedAtRoute("GetTypeById", new { id = created_id }, CreatedType);
                //return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside PostTypeAsync() action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
        [HttpPut("Put/{id}")]//+
        public async Task<ActionResult> PutAsync(int id, [FromBody] WorkOutType updateWorkType)
        {
            try
            {
                if (updateWorkType == null)
                {
                    _logger.LogError("Type object sent from client is null.");
                    return BadRequest("Type object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid type object sent from client.");
                    return BadRequest("Invalid type object");
                }
                var TypeEntity = await _unitofWork._workoutTypeRepository.GetByIdAsync(id);
                if (TypeEntity == null)
                {
                    _logger.LogError($"Type with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                await _unitofWork._workoutTypeRepository.ReplaceAsync(updateWorkType);
                _unitofWork.Commit();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside PutAsync() action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            try
            {
                var TypeEntity = await _unitofWork._workoutTypeRepository.GetByIdAsync(id);
                if (TypeEntity == null)
                {
                    _logger.LogError($"Type with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                await _unitofWork._workoutTypeRepository.DeleteAsync(id);
                _unitofWork.Commit();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside WorkOutType action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}

