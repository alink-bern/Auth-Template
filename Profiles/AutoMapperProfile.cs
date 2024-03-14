using AutoMapper;


namespace CAS.Profiles;

public class AutoMapperProfile: Profile
{
  public AutoMapperProfile()
  {
    // Users
    CreateMap<LoginDTO,User>();
    CreateMap<RegisterDTO,User>();
    CreateMap<User,LoginResponseDTO>();
    CreateMap<User,RegisterResponseDTO>();
  }
}
