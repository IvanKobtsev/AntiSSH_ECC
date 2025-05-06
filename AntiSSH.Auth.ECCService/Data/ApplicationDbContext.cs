using AntiSSH.Auth.ECC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AntiSSH.Auth.ECC.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<EncryptedKey> EncryptedKeys { get; set; }
    public DbSet<User> Users { get; set; }
}
