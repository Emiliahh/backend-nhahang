using backend.Data;
using backend.DTOs.User;
using backend.Exceptions;
using backend.Models;
using backend.Services.Interfaces;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IConfiguration _config;
    private readonly NhahangContext _context;

    public AuthService(UserManager<User> userManager, RoleManager<Role> roleManager, IConfiguration config, NhahangContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _config = config;
        _context = context;
    }
    // tao jwt
    public string GenerateJwt(IList<Claim> claims)
    {
        try
        {
            var setting = _config.GetSection("JWT:Setting");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:AC"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: setting["Issuer"],
                audience: setting["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(Convert.ToDouble(setting["AccessExpirationDay"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            throw new Exception("Error generating JWT token", ex);
        }
    }
    // dang ky
    public async Task<User> RegisterAsync(UserDto userDto)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(userDto.email);
            if (existingUser != null)
                throw new UserAlreadyExistException("Email already exists");

            var user = new User
            {
                Email = userDto.email,
                UserName = userDto.name,
                Phone = userDto.phone,
            };
            

            var createUserResult = await _userManager.CreateAsync(user, userDto.password);
            if (!createUserResult.Succeeded)
            {
                throw new Exception("Failed to create user: " + string.Join(", ", createUserResult.Errors.Select(e => e.Description)));
            }
            
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new Role { Name = "User" });
            }
            await _userManager.AddToRoleAsync(user, "User");

            return user;
        }
        catch (Exception ex)
        {
            throw new Exception("Error registering user", ex);
        }
    }


    // dang nhap
    public async Task<(UserResDto res, string token)> LoginAsync(LoginDto loginDto, HttpResponse response)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.email);
        if (user == null)
        {
            throw new UserNotFoundException("User not found");
        }

        if (!await _userManager.CheckPasswordAsync(user, loginDto.password))
        {
            throw new PasswordMismatchException("Invalid password");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
    };
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var refreshToken = RefreshToken(claims);
        var token = GenerateJwt(claims);
        response.Cookies.Append("auth_token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure = false,
            Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_config["JWT:Refresh:RefreshExpirationDay"]))
        });

        var res = new UserResDto
        {
            name = user.UserName,
            phone = user.Phone,
            address = user.Address,
            email = user.Email,
            roles = roles.ToList(),
        };

        return (res, token);
    }

    public async Task<(UserResDto res, string token, string rfToken)> LoginAsyncMobile(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.email);
        if (user == null)
        {
            throw new UserNotFoundException("User not found");
        }

        if (!await _userManager.CheckPasswordAsync(user, loginDto.password))
        {
            throw new PasswordMismatchException("Invalid password");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
    };
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var refreshToken = RefreshToken(claims);
        var token = GenerateJwt(claims);
        var res = new UserResDto
        {
            name = user.UserName,
            phone = user.Phone,
            address = user.Address,
            email = user.Email,
            roles = roles.ToList(),
        };

        return (res, token, refreshToken);
    }

    // dang xuat
    public async Task LogoutAsync(string id)
    {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new UserNotFoundException("User not found");

            await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "AccessToken");
    }
    // get userInfo
    public async Task<UserResDto> GetUserInfo(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new UserNotFoundException("User not found");
            var roles = await _userManager.GetRolesAsync(user);

            return new UserResDto
            {
                name = user.UserName,
                phone = user.Phone,
                address = user.Address,
                email = user.Email,
                roles =roles.ToList(),
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving user info", ex);
        }
    }
    // tao refresh token
    /*@param claims list*/
    public string RefreshToken(IList<Claim> claims)
    {
        try
        {
            var setting = _config.GetSection("JWT:Refresh");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting["KEY"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: setting["Issuer"],
                audience: setting["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(Convert.ToDouble(setting["RefreshExpirationDay"])),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            throw new Exception("Error generating refresh token", ex);
        }
    }
    // xac thuc access token
    public string ValidateRefreshToke(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["JWT:Refresh:KEY"]);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _config["JWT:Refresh:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["JWT:Refresh:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            if(principal == null)
            {
                throw new InvalidOperationException("Invalid token");
            }
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            return subClaim;
        }
        catch (Exception ex)
        {
            throw new Exception("Error validating refresh token", ex);
        }
    }

    // tao refresh token
    public async Task<string> IssueRefreshToken(string token)
    {
            var sub = ValidateRefreshToke(token);
            if (sub == null)
                throw new InvalidTokenException();
            var user = await _userManager.FindByIdAsync(sub) ?? throw new UserNotFoundException("User not found");
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            //var refreshToken = RefreshToken(claims);
            var newToken = GenerateJwt(claims);
            return newToken;
    }
    // promote thanh admin
    public async Task<User> Promote(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new InvalidOperationException("User not found");

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                throw new InvalidOperationException("User is already an admin");

            await _userManager.RemoveFromRoleAsync(user, "User");
            await _userManager.AddToRoleAsync(user, "Admin");

            return user;
        }
        catch (Exception ex)
        {
            throw new Exception("Error promoting user", ex);
        }
    }
    // promote thanh nhan vien
    public async Task<User> PromoteEmployee(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            throw new InvalidOperationException("User not found");

        if (await _userManager.IsInRoleAsync(user, "Staff"))
            throw new InvalidOperationException("User is already a staff");

        await _userManager.RemoveFromRoleAsync(user, "User");
        await _userManager.AddToRoleAsync(user, "Staff");

        return user;
    }
}
