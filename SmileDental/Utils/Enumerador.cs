namespace SmileDental.Utils
{
    public static class Enumerador
    {
        /*
         *  la funcion de esta clase es definir el número de páginas por dentista que se enviarán al cliente en formato json
        */
        public static int numeroDePaginas(int citas)
        {
            int citasPorPagina = 10;

            return (int)Math.Ceiling((double)citas / citasPorPagina);
        }
    }
}
