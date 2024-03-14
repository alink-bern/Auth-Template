using AutoMapper;
using Backend.Data.DTOs.Request;
using Backend.Data.DTOs.Response;
using Backend.Helpers;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;


[Authorize] // This will make sure that the user is authenticated before accessing any of the methods in this controller
[Route("api/auth")]
[ApiController]
public class AuthController: ControllerBase
{
  private readonly IConfigurationRoot _configuration;
  private readonly IInterface _repository;
  private readonly IMapper _mapper;

  public AuthController(IMapper mapper,IInterface repository)
  {
    _configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json",optional: false,reloadOnChange: true)
    .Build();
    _repository = repository;
    _mapper = mapper;

    if (_configuration == null || _repository == null || _mapper == null)
    {
      throw new ArgumentNullException();
    }
  }


  [AllowAnonymous] // This will allow the user to access this method without being authenticated (without JWT Token)
  [HttpPost, Route("login")]
  public async Task<ActionResult<LoginResponseDTO>> Login(LoginDTO userToLogin)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    User? userFromDB = await _repository.GetUserByMail(userToLogin.Email);

    if (userFromDB == null)
    {
      return Unauthorized("Invalid email or password");
    }
    if (!PasswordHasher.CompareHashAndPassword(userFromDB.HashedPassword,userToLogin.Password))
    {
      return Unauthorized("Invalid email or password");
    }

    LoginResponseDTO loginResponseDTO = _mapper.Map<LoginResponseDTO>(userFromDB);
    loginResponseDTO.Token = JwtTokens.GenerateToken(userFromDB.Id);
    return Ok(loginResponseDTO);
  }


  [AllowAnonymous]
  [HttpPost, Route("register")]
  public async Task<ActionResult<RegisterResponseDTO>> Register(RegisterDTO userRegisterRequest)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    User? userAlreadyExists = await _repository.GetUserByMail(userRegisterRequest.Email);
    if (userAlreadyExists != null)
    {
      return BadRequest(new ProblemDetails
      {
        Status = StatusCodes.Status400BadRequest,
        Type = "Bad Request",
        Title = "Email Already in Use",
        Detail = "The email address provided is already associated with an existing account."
      });
    }

    User userToAddToDB = _mapper.Map<User>(userRegisterRequest);
    userToAddToDB.HashedPassword = PasswordHasher.HashPassword(userRegisterRequest.Password);

    User? responseAddUser = await _repository.RegisterNewUser(userToAddToDB);
    if (responseAddUser != null && _repository.SaveChanges())
    {
      string token = JwtTokens.GenerateToken(responseAddUser.Id);
      RegisterResponseDTO userToReturn = _mapper.Map<RegisterResponseDTO>(responseAddUser);
      userToReturn.Token = token;
      return Ok(userToReturn);
    }
    else
    {
      return BadRequest(new ProblemDetails
      {
        Status = StatusCodes.Status400BadRequest,
        Type = "Bad Request",
        Title = "Error while registering new user",
        Detail = "There was an error while registering the new user."
      });
    }
  }


  [HttpPost, Route("change-password")]
  public async Task<IActionResult> NewPassword(NewPasswordDTO newPasswordDTO)
  {

    User? userFromDB = await _repository.GetUserById(newPasswordDTO.UserId);
    if (userFromDB == null)
    {
      return BadRequest();
    }

    if (!PasswordHasher.CompareHashAndPassword(userFromDB.HashedPassword,newPasswordDTO.OldPassword))
    {
      return Unauthorized("Invalid password");
    }

    string NewPasswordHash = PasswordHasher.HashPassword(newPasswordDTO.NewPassword);
    User? userWithNewPassword = await _repository.SetNewUserPassword(newPasswordDTO.UserId,NewPasswordHash);
    if (userWithNewPassword == null)
    {
      return NotFound("User not found.");
    }
    _repository.SaveChanges();
    return Ok("Password reset successful.");
  }
}
