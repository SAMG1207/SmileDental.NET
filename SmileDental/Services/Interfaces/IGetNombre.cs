namespace SmileDental.Services.Interfaces
{
    public interface IGetNombre
    {
       Task<string> GetNombre(int id);
    }
}
