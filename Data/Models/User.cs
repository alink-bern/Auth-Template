﻿namespace Backend.Data.Models
{
  public class User
  {
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string HashedPassword { get; set; }
    public string Email { get; set; }
    // can be added if needed
    //public string? Token { get; set; }
  }
}
