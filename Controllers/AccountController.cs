using EtaAnnouncer.Models;
using EtaAnnouncer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EtaAnnouncer.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(UserManager<Member> userManager, TokenGeneratorService tokenService, RefreshTokenService refreshTokenService) : ControllerBase
    {
        public record RegisterModel(string Username, string Email, string Password);
        [HttpPost("register")]
        public async Task<IActionResult> CreateAccount([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await userManager.FindByNameAsync(registerModel.Username) ?? await userManager.FindByEmailAsync(registerModel.Email);
            if (user != null)
            {
                return Conflict();
            }
            var newUser = new Member
            {
                UserName = registerModel.Username,
                Email = registerModel.Email
            };
            var result = await userManager.CreateAsync(newUser, registerModel.Password);
            
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok();
        }

        public record LoginModel(string Identifier, string Password);
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await userManager.FindByEmailAsync(loginModel.Identifier) ?? await userManager.FindByNameAsync(loginModel.Identifier);
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await userManager.CheckPasswordAsync(user, loginModel.Password);
            if (!result)
            {
                return Unauthorized();
            }
            var accessToken = tokenService.GenerateAccessToken(user.UserName!);
            var refreshToken = tokenService.GenerateRefreshToken();
            await refreshTokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        public record RefreshModel(string RefreshToken);
        [HttpPost("refresh")]
        public async Task<IActionResult> GetNewRefreshToken([FromBody] RefreshModel refreshModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var refreshToken = await refreshTokenService.GetRefreshTokenAsync(refreshModel.RefreshToken);
            if (refreshToken == null)
            {
                return Unauthorized();
            }

            var user = await userManager.FindByIdAsync(refreshToken.UserId);
            if (user == null)
            {
                return Unauthorized();
            }

            var newAccessToken = tokenService.GenerateAccessToken(user.UserName!);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            await refreshTokenService.RevokeRefreshTokenAsync(refreshModel.RefreshToken);
            await refreshTokenService.SaveRefreshTokenAsync(user.Id, newRefreshToken);

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshModel refreshModel)
        {
            await refreshTokenService.RevokeRefreshTokenAsync(refreshModel.RefreshToken);

            return Ok();
        }            
    }
}
