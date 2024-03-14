namespace Backend.Services;

public class SQLFunctions(AppDBContext context): IInterface
{
  private readonly AppDBContext _context = context;

  public bool SaveChanges()
  {
    return _context.SaveChanges() >= 0;
  }

  public async Task<int> SaveChangesAsync()
  {
    return await _context.SaveChangesAsync();
  }


  public async Task<User?> DeleteUserById(Guid id)
  {
    User? userFromDB = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
    if (userFromDB == null)
    {
      return null;
    }
    var answer = _context.Users.Remove(userFromDB);
    await _context.SaveChangesAsync();
    if (await _context.Users.FirstOrDefaultAsync(x => x.Id == id) == null)
    {
      return userFromDB;
    }
    else
    {
      return null;
    }
  }

  public async Task<IEnumerable<User>?> GetAllUsers()
  {
    return await _context.Users.ToListAsync();
  }

  public async Task<User?> GetUserById(Guid id)
  {
    return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
  }

  public async Task<User?> GetUserByMail(string mail)
  {
    return await _context.Users.FirstOrDefaultAsync(x => x.Email == mail);
  }

  public async Task<User?> RegisterNewUser(User user)
  {
    await _context.Users.AddAsync(user);
    await SaveChangesAsync();
    return await GetUserByMail(user.Email);
  }

  public async Task<User?> SetNewUserPassword(Guid userId,string newPasswordHash)
  {
    var userInDB = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
    if (userInDB != null)
    {
      userInDB.HashedPassword = newPasswordHash;
      await _context.SaveChangesAsync();
      return userInDB;
    }
    else
    {
      return null;
    }
  }
}
