using Domain.Models;
using Exemplo_de_API_Net_Core_com_MongoDB.Controllers;
using Microsoft.AspNetCore.Mvc;
using Test.Fixtures;

namespace Test.Controllers;

public class SegurancaControllerTest
{
    private readonly ConfigurationFixture _configuration;
    private readonly SegurancaController _controller;

    public SegurancaControllerTest()
    {
        _configuration = new ConfigurationFixture();

        _controller = new SegurancaController(_configuration.Configuration);
    }

    [Fact]
    public void Login()
    {
        var loginRequest = new LoginRequest()
        {
            Senha = "testeApi",
            Usuario = "arthur"
        };

        var result = _controller.Login(loginRequest);

        Assert.NotNull(result);
        Assert.Equal(typeof(OkObjectResult), result.GetType()); 
    }

    [Fact]
    public void Login_Incorreto()
    {
        var loginRequest = new LoginRequest()
        {
            Senha = "6wf6d0f5",
            Usuario = "65sd0f"
        };

        var result = _controller.Login(loginRequest);

        Assert.NotNull(result);
        Assert.Equal(typeof(UnauthorizedResult), result.GetType());
    }
}
