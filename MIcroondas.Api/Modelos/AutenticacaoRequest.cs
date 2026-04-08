using System.ComponentModel.DataAnnotations;

namespace Microondas.Api.Modelos;

public class AutenticacaoRequest
{
    [Required]
    public string Usuario { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Senha { get; set; } = string.Empty;
}