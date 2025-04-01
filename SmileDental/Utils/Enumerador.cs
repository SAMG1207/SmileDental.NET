namespace SmileDental.Utils
{
    public static class Enumerador
    {
        public static int numeroDePaginas(int citas)
        {
            int citasPorPagina = 10;

            return (int)Math.Ceiling((double)citas / citasPorPagina);
        }
    }
}
