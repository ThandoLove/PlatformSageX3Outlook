using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspace.Domain.Entities
{

    public class LoginUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // THE HASHED PASSWORD
        public string Role { get; set; } = string.Empty;
    }

}
