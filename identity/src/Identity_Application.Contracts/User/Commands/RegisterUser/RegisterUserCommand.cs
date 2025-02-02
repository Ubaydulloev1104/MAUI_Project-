﻿using MediatR;

namespace Identity_Application.Contracts.User.Commands.RegisterUser
{
	public class RegisterUserCommand : IRequest<Guid>
	{
		public string Email { get; set; } = "";
		public string FirstName { get; set; } = "";
		public string LastName { get; set; } = "";
		public string PhoneNumber { get; set; } = "";
		public string Username { get; set; } = "";
		public string Password { get; set; } = "";
		public string Application { get; set; } = "Math";
		public string ConfirmPassword { get; set; } = "";
		public string Role { get; set; } = "Applicant";
	}
}
