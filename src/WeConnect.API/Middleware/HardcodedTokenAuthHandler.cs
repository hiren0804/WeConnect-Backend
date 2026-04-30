using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace WeConnect.API.Middleware;

public class HardcodedTokenAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public HardcodedTokenAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock
    )
    : base(options, logger, encoder, clock)
    {
    }

    private const string ValidToken = "EwBYBMl6BAAU9BatlgMxts2T1B5e3Mucgfs4jcAAAX7wX4sigiTk8WapkP/k7rA8oNwC4zAP1L6Z4zJG6ZQMQZjUGCAor4FWdkc4XjFmm/G7J9/m0q8l9K7YVDxLFPGNkxAKMk1UDdT5yVsEHuPnkiqf2WYTkJ5f2OapKo1sDsygbWWMbu2sRs/S2VfXZPfICcRXhTE+1hcBt5jDj3taC44fY4nu2lwq4BQtLb3Hw6YOCKWPdVO++0R5ZQDHMZHhIm4grQHWw08ZKumr/yLzHHcQEXVwlKKD9h7p6608Bv9HU8gwmjZR465ADMu3ek2K3qVca+cphAXGeGapOIFlv7YdCp9vPS7b/jrhuZlic7WRLGfnnyzFGznfcABJj+gQZgAAEOKToA/ZT+XdUk7Wb4+lYxEgA1EGDbhcy30XQEN/uWYVumqQR9csBdVvvORezF7O13TX9qyYkrd+bSEhqZaS5tefYRmEnDSULA5JDKJNEK+ruP/f4IXQc08yFjiy+riHJX8E9qnxO47wHObBEzEb72c4ki/tO1hMpYC4g9FKXyuN3QYiz3rNYEamctN7u/t3eqSC/tqqKYSNbZkMujAkdEhfLQxQWjdDIC+q/N55Njbe9IlpP0OeRVFrDBkeDRyMScelPLv2nq7GYI0h57s7mxZ64ONM6eGrdO5qaH/hcaCfBzNprpyTq6DmvsAnz2TPTZAsdiXYG4yZjfK73gBongWIXEDRMAUMjxf1EjMp2qOSzxmd3Y3xWgbjpp2XSADxEE7I2MbinXQYPmJ/fT/t25Q+R7whYk4BeNBpL4aPMm/En+R+WqRMwnVQVpxxqlwxcIXBo8ltyeqonfQEGI20LFCF5zb0NCFAMLF4Q/y1LB2/5uwxAU167Kp2lm9KFDUwgVghUUNSxySwV3rTQyg7qEUwKdGa8BctrLrg6XSI0FqtkIeyJppxTvxsA+lbHjVkIT3Q0a0TyKVushAxaYRShTQyuqvNJfQJxs7B1/JYEsnMrpTkDwcgA/5DvVdDFOdwOOu5ZfVPUbUfhQ4loAALMwFsdarhQEWXWoDz2pPk/mtpcCifZJbuoPNRZGpg5gxMF7sx8SadxM03w9bpUjAXC9vUhKe9PAxMCFJErryfAN0USo/6SbxydhfzO/vxvbJj9V3K/GNjXyQ2bHE+xppQDpZuaumROkC5RpCtudvX3BcHBurK6XJwEeO2ujWYFYfM6Y3PW7m6GBpDV1sEJRR5gh0k6CIbqv4RjuzNkEW3HtXj5fGtM1wh+VXqx8P9wln5iQgVNMhWZBHXbsbjCgg/jfmLt57WCjLi7wHhPFIGaU+/E41Pm9Dq+ko2dV+u66FVQ+UnMquRbWMOx1vna+i9dhpjPbTHkCGeIsk9It96GfCl+gUhocZNfPzbHgn5ZrJ1RI+Z44TBPXUCBvFimuL0sXTLpoGc//Rqzw+FctJc65xmU+99DXCZH6t/bRTQzWCjhRsYVgM=";
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorizartion Token"));
        }
       
        var token = Request.Headers["Authorization"].ToString();

        if (token != $"Bearer {ValidToken}")
        {
            // Console.Write("the token i got is", token);
            return Task.FromResult(AuthenticateResult.Fail("Invalid Token"));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "HardcodedUser")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);

        var principal = new ClaimsPrincipal(identity);
  
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}