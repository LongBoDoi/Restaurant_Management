using API.ML.BO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Configuration;
using System.Security.Claims;

namespace API.ML.Common
{
    public static class Session
    {
        public static string Token {  get; set; } = string.Empty;
    }
}
