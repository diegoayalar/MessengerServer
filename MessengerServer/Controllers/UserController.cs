using MessengerDomain.Entities;
using MessengerService.Service;
using Microsoft.AspNetCore.Mvc;

namespace MessengerServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPatch("update-name")]
        public async Task<IActionResult> UpdateUserName(string id, string name)
        {
            return await UpdateUserFieldAsync(id, user => user.Profile.Name = name, "User name updated successfully.");
        }

        [HttpPatch("update-description")]
        public async Task<IActionResult> UpdateUserDescription(string id, string description)
        {
            return await UpdateUserFieldAsync(id, user => user.Profile.Description = description, "User description updated successfully.");
        }

        [HttpPatch("update-phone")]
        public async Task<IActionResult> UpdateUserPhoneNumber(string id, string phone)
        {
            return await UpdateUserFieldAsync(id, user => user.Phone = phone, "User phone updated successfully.");
        }

        [HttpPatch("update-profile-pic")]
        public async Task<IActionResult> UpdateUserProfilePic(string id, IFormFile picFile)
        {
            using (Stream fileStream = picFile.OpenReadStream())
            {
                var result = await _userService.UpdateUserProfilePicAsync(id, fileStream);
                if (!result.Success)
                    return BadRequest(result.ErrorMessage);
            }

            return Ok("User profile picture updated successfully.");
        }

        [HttpPatch("update-status")]
        public async Task<IActionResult> UpdateUserStatus(string id, int status)
        {
            return await UpdateUserFieldAsync(id, user => user.Profile.Status = status, "User status updated successfully.");
        }

        private async Task<IActionResult> UpdateUserFieldAsync(string id, Action<User> updateAction, string successMessage)
        {
            var (success, response) = await _userService.UpdateUserFieldAsync(id, updateAction);
            if (!success)
            {
                return BadRequest(response);
            }

            return Ok(successMessage);
        }
    }
}