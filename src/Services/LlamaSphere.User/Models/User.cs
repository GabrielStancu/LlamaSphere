﻿namespace LlamaSphere.User.Models;

public abstract class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}