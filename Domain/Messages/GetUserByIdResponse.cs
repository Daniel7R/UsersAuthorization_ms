﻿namespace UsersAuthorization.Domain.Messages
{
    public class GetUserByIdResponse
    {
        public int Id { get; set; }
        public string Name {  get; set; }
        public string Email { get; set; }
    }
}
