using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Exemplo_de_API_Net_Core_com_MongoDB.Controllers;

/// <summary>
/// Controlador responsável pelas operações relacionadas com a segunrança da aplicação
/// </summary>
[ApiController]
[Route("[controller]")]
public class SegurancaController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public SegurancaController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gera um novo Token para acesso ao sistema caso login e senha estejam corretos
    /// </summary>
    /// <param name="login"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Login([FromBody] LoginRequest login)
    {
        if (ValidarUsuario(login))
        {
            var tokenString = GerarNovoToken();
            return Ok(new { token = tokenString });
        }
        else
        {
            return Unauthorized();
        }
    }

    //Valida se o login e senha repassados estão dentro do esperado
    private bool ValidarUsuario(LoginRequest login)
    {
        if(login.Usuario == "arthur" && login.Senha == "testeApi")
        {
            return true;
        }

        return false;
    }

    //Gera um novo token com base nas credenciais definidas no appsettings, com duração de 30 minutos
    private string GerarNovoToken()
    {
        var securityKey = new SymmetricSecurityKey
                          (Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        var credentials = new SigningCredentials
                          (securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(issuer: _configuration["Jwt:Issuer"],
                                         audience: _configuration["Jwt:Audience"],
                                         expires: DateTime.Now.AddMinutes(30),
                                         signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        var stringToken = tokenHandler.WriteToken(token);
        return stringToken;
    }
}
