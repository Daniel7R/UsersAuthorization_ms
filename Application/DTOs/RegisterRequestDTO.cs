﻿namespace UsersAuthorization.Application.DTO
{
    public class RegisterRequestDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string? Role;
    }
}
