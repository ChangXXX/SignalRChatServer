using SignalRChat.Models;
using SignalRChat.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace SignalRChat.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    
    private readonly UsersService _usersService;
    private readonly IConfiguration _config;

    public UsersController(UsersService usersService, IConfiguration config) {
        _usersService = usersService;   
        _config = config;
    }

    [HttpPost]
    public async Task<IActionResult> Post(User newUser)
    {
        await _usersService.CreateAsync(newUser);

        return CreatedAtAction(nameof(Get), new { id = newUser.id });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> Get(string id)
    {
        var user = await _usersService.GetAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    // 로그인 기능 담당하는 Get 함수
    [AllowAnonymous]
    [HttpGet("{name}/{pwd}")]
    public async Task<IActionResult> Get(string name, string pwd)
    {
        var user = await _usersService.GetAsync(name, pwd);
        if (user == null)
        {
            return BadRequest(new {message = "이름이나 비밀번호가 부정확합니다."});
        }
        var token = generateJwt(user);
        return Ok(new { token = token });
    }

    [HttpGet]
    [Authorize]
    public async Task<List<User>> Get() =>
        await _usersService.GetAsync();

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(string id, User updatedUser)
    {
        var user = await _usersService.GetAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        updatedUser.id = user.id;

        await _usersService.UpdateAsync(id, updatedUser);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _usersService.GetAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        await _usersService.RemoveAsync(id);
        
        return NoContent();
    }

    private string generateJwt(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Name, user.name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}