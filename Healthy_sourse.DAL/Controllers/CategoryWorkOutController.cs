using Healthy_sourse_norm.Entities;
using Healthy_sourse_norm.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Healthy_sourse.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class CategoryWorkOutController : ControllerBase
    {
        private readonly ILogger<CategoryWorkOutController> _logger;

        private IUnitOfWork _unitofWork;

        public CategoryWorkOutController(ILogger<CategoryWorkOutController> logger, IUnitOfWork unitofWork)
        {
            _logger = logger;
            _unitofWork = unitofWork;
        }

        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<IEnumerable<Category_WorkOut>>> GetAllCategoriesAsync()
        {
            try
            {
                var results = await _unitofWork._categoryWorkOutRepository.GetAllAsync();
                _unitofWork.Commit();
                _logger.LogInformation($"Returned all categories from database.");
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Transaction Failed! Something went wrong inside GetAllCategoriesAsync() action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("GetById/{IdCategory}", Name = "GetCategoryById")]
        public async Task<ActionResult<Category_WorkOut>> GetByIdAsync(int id)
        {
            try
            {
                var result = await _unitofWork._categoryWorkOutRepository.GetByIdAsync(id);
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
                _logger.LogError($"Something went wrong inside GetByIdAsync() action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("CategoryByName")]
        public async Task<ActionResult<IEnumerable<Category_WorkOut>>> CategoryByNameAsync(string category)
        {
            try
            {
                var results = await _unitofWork._categoryWorkOutRepository.GetCategoryByNameAsync(category);
                _unitofWork.Commit();
                _logger.LogInformation($"Returned top five categories from database.");
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CategoryByNameAsync() action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost("PostCategory")]
        public async Task<ActionResult> PostCategoryAsync([FromBody] Category_WorkOut newCategory)
        {
            try
            {
                if (newCategory == null)
                {
                    _logger.LogError("Category object sent from client is null.");
                    return BadRequest("Category object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Category object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var created_id = await _unitofWork._categoryWorkOutRepository.AddAsync(newCategory);
                var CreatedCategory = await _unitofWork._categoryWorkOutRepository.GetByIdAsync(created_id);
                _unitofWork.Commit();
                return CreatedAtRoute("GetCategoryById", new { id = created_id }, CreatedCategory);
                //return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Something went wrong inside PostCategoryAsync() action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
        [HttpPut("Put/{id}")]
        public async Task<ActionResult> PutAsync (int id, [FromBody] Category_WorkOut updateWorkOut)
        {
            try
            {
                if (updateWorkOut == null)
                {
                    _logger.LogError("Category object sent from client is null.");
                    return BadRequest("Category object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid category object sent from client.");
                    return BadRequest("Invalid category object");
                }
                var CategoryEntity = await _unitofWork._categoryWorkOutRepository.GetByIdAsync(id);
                if (CategoryEntity == null)
                {
                    _logger.LogError($"Category with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                await _unitofWork._categoryWorkOutRepository.ReplaceAsync(updateWorkOut);
                _unitofWork.Commit();
                return NoContent();
            }
            catch(Exception ex)
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
                var CategoryEntity = await _unitofWork._categoryWorkOutRepository.GetByIdAsync(id);
                if (CategoryEntity == null)
                {
                    _logger.LogError($"Category with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                await _unitofWork._categoryWorkOutRepository.DeleteAsync(id);
                _unitofWork.Commit();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Categories of WO action: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        
    }
}
