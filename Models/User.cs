﻿namespace GrpcService.Models
{
	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty;
		public int Role { get; set; }
	}
}
