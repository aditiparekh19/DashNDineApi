using DashNDineApi.Data;
using DashNDineApi.Models;
using DashNDineApi.Models.DTO;
using DashNDineApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DashNDineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IBlobService _blobService;
        private ApiResponse _response;

        public MenuItemController(ApplicationDbContext db, IBlobService blobService)
        {
            _db = db;
            _blobService = blobService;
            _response = new ApiResponse();
        }

        [HttpGet]
        public IActionResult GetMenuItems()
        {
            _response.Result = _db.MenuItems;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("{menuId:int}", Name ="GetMenuItem")]
        public IActionResult GetMenuItemById(int menuId)
        {
            if (menuId == 0)
            {
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>()
                {
                    "Bad Request"
                };
                return BadRequest(_response);
            }

            var result = _db.MenuItems.FirstOrDefault(x => x.Id == menuId);
            if(result == null)
            {
                _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                _response.ErrorMessages = new List<string>()
                {
                    "404: Not Found"
                };
                return NotFound(_response);
            }

            _response.Result = result;
           _response.StatusCode=System.Net.HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateMenuItem([FromForm]MenuItemCreateDto menuItemCreateDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(menuItemCreateDto.File == null || menuItemCreateDto.File.Length == 0)
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        _response.ErrorMessages = new List<string>()
                        {
                            "Bad Request"
                        };
                        return BadRequest(_response);
                    }
                    else
                    {
                        string image = $"{Guid.NewGuid()}{Path.GetExtension(menuItemCreateDto.File.FileName)}";
                        MenuItem menuItemToCreate = new()
                        {
                            Name = menuItemCreateDto.Name,
                            Price = menuItemCreateDto.Price,
                            Category = menuItemCreateDto.Category,
                            SpecialTag = menuItemCreateDto.SpecialTag,
                            Description = menuItemCreateDto.Description,
                            Image = await _blobService.UploadBlob(image, Utility.Constants.ContainerNameImages, menuItemCreateDto.File)
                        };
                        _db.MenuItems.Add(menuItemToCreate);
                        _db.SaveChanges();
                        _response.IsSuccess = true;
                        _response.Result = menuItemToCreate;
                        _response.StatusCode = System.Net.HttpStatusCode.Created;
                        return CreatedAtRoute("GetMenuItem", new { menuId = menuItemToCreate.Id }, _response);
                    }
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>()
                    {
                        "Bad Request"
                    };
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess= false;
                _response.ErrorMessages
                    = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}
