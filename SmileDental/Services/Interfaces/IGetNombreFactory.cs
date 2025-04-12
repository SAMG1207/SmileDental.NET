using SmileDental.Utils;

namespace SmileDental.Services.Interfaces
{
    public interface IGetNombreFactory
    {
        public IGetNombre GetServicio(EnumeradorUsuarios usuarios);
    }
}
