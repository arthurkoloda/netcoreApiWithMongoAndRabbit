﻿namespace Domain.Models;

public class LoginRequest
{
    public string Usuario { get; set; } = string.Empty;

    public string Senha { get; set; } = string.Empty;
}
