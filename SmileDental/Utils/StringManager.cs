﻿using System.Text.RegularExpressions;

namespace SmileDental.Utils
{
    public static class StringManager
    {
        public static bool ValidaNombre(string nombre)
        {
            string regexPattern = @"^[A-ZÁÉÍÓÚÜÑ][a-záéíóúüñ'\-]*(?:\s[A-ZÁÉÍÓÚÜÑ][a-záéíóúüñ'\-]*)*$";
            return Regex.IsMatch(nombre, regexPattern);
        }

        public static bool ValidaPassword(string password)
        {
            string regexPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,25}$";
            return Regex.IsMatch(password, regexPattern);
        }

        public static bool ValidaDni(string dni)
        {
            string dniNiePattern = @"^[XYZ]?\d{7,8}[A-Za-z]$";
            return Regex.IsMatch(dni, dniNiePattern);
        }

        public static bool ValidaTelefono(string telefono)
        {
            string regexPattern = @"^\d{9}$";
            return Regex.IsMatch(telefono, regexPattern);
        }

    }
}
