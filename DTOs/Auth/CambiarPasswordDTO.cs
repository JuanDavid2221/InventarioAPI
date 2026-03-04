public class CambiarPasswordDTO
{
    public string PasswordActual { get; set; } = string.Empty;
    public string PasswordNueva { get; set; } = string.Empty;
    public string ConfirmarPasswordNueva { get; set; } = string.Empty; // Nuevo campo 🆕
}