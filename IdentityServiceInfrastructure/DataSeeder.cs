﻿using IdentityServiceApplication.Services;
using IdentityServiceDomain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServiceInfrastructure
{
    public class DataSeeder
    {
        private readonly UserDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public DataSeeder(UserDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public void Seed()
        {
            try
            {
                // Sprawdzenie, czy można połączyć się z bazą danych
                if (_dbContext.Database.CanConnect())
                {
                    // Zastosowanie wszystkich migracji, jeśli baza danych jest dostępna
                    _dbContext.Database.Migrate();

                    // Seed tylko wtedy, gdy brak użytkowników w bazie danych
                    if (!_dbContext.Users.Any())
                    {
                        var users = new List<User>
                {
                    new User()
                    {
                        Username = "Adam Nowak",
                        Email = "adamNowak@gmail.com",
                        Role = "User",
                        CreatedAt = DateTime.Now.AddDays(-1),
                        PasswordHash = _passwordHasher.HashPassword("Password123")
                    },
                    new User()
                    {
                        Username = "Jan Kowalski",
                        Email = "janKowalski@gmail.com",
                        Role = "User",
                        CreatedAt = DateTime.Now.AddDays(-2),
                        PasswordHash = _passwordHasher.HashPassword("Password456")
                    }
                };

                        _dbContext.Users.AddRange(users);
                        _dbContext.SaveChanges();
                        Console.WriteLine("Seeding completed successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Users already exist. Skipping seeding.");
                    }
                }
                else
                {
                    Console.WriteLine("Cannot connect to the database. Skipping migrations and seeding.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during seeding: {ex.Message}");
                // Rzucanie wyjątku, jeśli konieczne
                throw;
            }
        }
    }
}
