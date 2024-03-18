namespace Database.Enums;

/// <summary>
/// Enum que define as situações que um documento pode ter, desde sua importação até seu processamento
/// </summary>
public enum ESituacaoDocumento
{
    AguardandoProcessamento = 1,
    Processando = 2,
    Processado = 3,
    ProcessadoComErro = 4,
    Falha = 5
}
