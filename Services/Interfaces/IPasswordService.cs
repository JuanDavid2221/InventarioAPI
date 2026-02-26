namespace InventarioAPI.Services.Interfaces
{
    public interface IPasswordService
    {
        string Hash(string password);
        bool Verificar(string password, string passwordHash);
    }
}
