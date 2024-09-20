using Healthy_sourse_norm.Entities;
using Healthy_sourse_norm.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Healthy_sourse.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class WorkOutController : ControllerBase
    {
        private readonly ILogger<WorkOutController> _logger;

        private IUnitOfWork _unitofWork;

        public WorkOutController(ILogger<WorkOutController> logger, IUnitOfWork unitofWork)
        {
            _logger = logger;
            _unitofWork = unitofWork;
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<WorkOut>>> GetAllWorkOutAsync()
        {
            try
            {
                var results = await _unitofWork._workoutRepository.GetAllAsync();
                _unitofWork.Commit();
                _logger.LogInformation($"Returned all works out from database.");
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Transaction Failed! Something went wrong inside GetAllWorkOutAsync() action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("GetById/{id}", Name = "GetWorkOutById")]
        public async Task<ActionResult<WorkOut>> GetByIdAsync(int id)
        {
            try
            {
                var result = await _unitofWork._workoutRepository.GetByIdAsync(id);
                _unitofWork.Commit();
                if (result == null)
                {
                    _logger.LogError($"Work out with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInformation($"Returned work out with id: {id}");
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Transaction Failed! Something went wrong inside GetByIdAsync() action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("TopThreeWorkOut")]
        public async Task<ActionResult<IEnumerable<WorkOut>>> GetTopThreeAsync()
        {
            try
            {
                var results = await _unitofWork._workoutRepository.GetTopThreeWorkOutAsync();
                _unitofWork.Commit();
                _logger.LogInformation($"Returned top three works out from database.");
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Transaction Failed! Something went wrong inside GetTopThreeAsync() action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost("PostWorkOut")]
        public async Task<ActionResult> PostWorkOutAsync([FromBody]WorkOut newWorkOut)
        {
            try
            {
                if(newWorkOut == null)
                {
                    _logger.LogError("Work out object sent from client is null.");
                    return BadRequest("Work out object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Work out object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var created_Id = await _unitofWork._workoutRepository.AddAsync(newWorkOut);
                var CreatedWorkOut = await _unitofWork._workoutRepository.GetByIdAsync(created_Id);
                _unitofWork.Commit();
                return CreatedAtRoute("GetWorkOutById", new { id = created_Id }, CreatedWorkOut);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Transaction Failed! Something went wrong inside PostWorkOutAsync() action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPut("Put/{id}")]
        public async Task<ActionResult> PutAsync(int id, [FromBody]WorkOut updateWorkOut)
        {
            try
            {
                if (updateWorkOut == null)
                {
                    _logger.LogError("Work out object sent from client is null.");
                    return BadRequest("Work out object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid work out object sent from client.");
                    return BadRequest("Invalid work out object");
                }
                var WorkOutEntity = await _unitofWork._workoutRepository.GetByIdAsync(id);
                if (WorkOutEntity == null)
                {
                    _logger.LogError($"Work out with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                await _unitofWork._workoutRepository.ReplaceAsync(updateWorkOut);
                _unitofWork.Commit();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Transaction Failed! Something went wrong inside PutAsync() action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");

            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            try
            {
                var WorkOutEntity = await _unitofWork._workoutRepository.GetByIdAsync(id);
                if (WorkOutEntity == null)
                {
                    _logger.LogError($"Work out with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                await _unitofWork._workoutRepository.DeleteAsync(id);
                _unitofWork.Commit();
                return NoContent();
            }catch (Exception ex)
            {
                _logger.LogError($"Transaction Failed! Something went wrong inside DeleteAsync() action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
