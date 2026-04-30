namespace WeConnect.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using WeConnect.Application.DTOs;

[ApiController]
[Route("api/config")]
public class ConfigController : ControllerBase
{
    [HttpGet]
     public IActionResult Get()
    {
       var config = HttpContext.Items["TenantConfig"] as TenantConfigDto;
        //   Console.WriteLine($"config: {System.Text.Json.JsonSerializer.Serialize(config)}");
        return config is null ? NotFound() : Ok(config);
    }
}