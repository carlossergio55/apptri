using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Helpers
{
    public class Base64SIN
    {

    
        private static volatile Base64SIN gInstance;

        /// <summary>
        /// Constructor privado, para no crear instancias múltiples de la clase
        /// </summary>
        private Base64SIN() { }

        /// <summary>
        /// Permite establecer un acceso directo, para generar una instancia única de acceso a la clase
        /// </summary>
        public static Base64SIN Instance
        {
            get
            {
                if (gInstance == null)
                    gInstance = new Base64SIN();
                return gInstance;
            }
        }

        /// <summary>
        /// Codificador base64
        /// <see cref="http://jc-mouse.net/"/>
        /// <param name="value">Numero a codificar</param>                
        /// <returns>Cadena codificada</returns>
        /// </summary>
        public  String convertBase64(int value)
        {
            String[] dictionary = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                                "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
                                "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d",
                                "e", "f", "g", "h", "i", "j", "k", "l", "m", "n",
                                "o", "p", "q", "r", "s", "t", "u", "v", "w", "x",
                                "y", "z", "+", "/" };
            int quotient = 1;
            int remainder;
            String word = "";
            while (quotient > 0)
            {
                quotient = value / 64;
                remainder = value % 64;
                word = dictionary[remainder] + word;
                value = quotient;
            }
            return word;
        }

    }

}
