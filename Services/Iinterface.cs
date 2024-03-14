namespace Backend.Services
{
  public interface IInterface
  {
    // Save Changes
    bool SaveChanges();
    Task<int> SaveChangesAsync();

    // User
    Task<User?> GetUserById(Guid id);
    Task<IEnumerable<User>?> GetAllUsers();
    Task<User?> GetUserByMail(string mail);
    Task<User?> RegisterNewUser(User user);
    Task<User?> DeleteUserById(Guid id);

    // User Password
    Task<User?> SetNewUserPassword(Guid userId,string newPasswordHash);

  }
}