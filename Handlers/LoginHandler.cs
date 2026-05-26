using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using VehicleMangement.Data;
using System.IdentityModel.Tokens.Jwt;
using VehicleMangement.Queries;
using BC = BCrypt.Net.BCrypt;
using VehicleMangement.Models;

namespace VehicleMangement.Handlers
{
    public class LoginHandler: IRequestHandler<LoginQuery,LoginResponse>
    {
        private readonly UserDbContext _context;
        private readonly IConfiguration _configuration;
        public LoginHandler(UserDbContext context, IConfiguration configuration)
        {
            _context=context;
            _configuration=configuration;
        }
        public async Task<LoginResponse> Handle(LoginQuery query,CancellationToken cancellationToken)
        {
            var result=await _context.Users.FirstOrDefaultAsync(u=>u.Email==query.Email,cancellationToken);
            if (result == null)
                throw new KeyNotFoundException("Invalid Email or Password");
            var isValid = BC.Verify(query.Password, result.PasswordHash);

            if (!isValid)
                throw new UnauthorizedAccessException("Invalid Email or Password");

            var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var claims=new[]
            {
                new Claim(ClaimTypes.NameIdentifier,result.UserId.ToString()),
                new Claim("UserName",result.Name),
                new Claim("UserEmail",result.Email),
               new Claim(JwtRegisteredClaimNames.Iat,DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),ClaimValueTypes.Integer64)
            };
            var token=new JwtSecurityToken(
                issuer:_configuration["JWT:Issuer"],
                audience:_configuration["JWT:Audience"],
                claims:claims,
                notBefore: DateTime.UtcNow,
                expires:DateTime.UtcNow.AddHours(1),
                signingCredentials:creds
            );
            return new LoginResponse
            {
                UserId = result.UserId,
                Name = result.Name,
                Email = result.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

    }

}
