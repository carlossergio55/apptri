using ControlCAplicacion.HelpersodeV7;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Helpers
{
    public  class ControlCode
    {


        private static volatile ControlCode gInstance;

        /// <summary>
        /// Constructor privado, para no crear instancias múltiples de la clase
        /// </summary>
        private ControlCode() { }

        /// <summary>
        /// Permite establecer un acceso directo, para generar una instancia única de acceso a la clase
        /// </summary>
        public static ControlCode Instance
        {
            get
            {
                if (gInstance == null)
                    gInstance = new ControlCode();
                return gInstance;
            }
        }

        /// <summary>
        /// Genera codigo de control
        /// <see cref="http://jc-mouse.net/"/>
        /// <param name="authorizationNumber">Numero de autorizacion</param>        
        /// <param name="invoiceNumber">Numero de factura</param>        
        /// <param name="nitci">Número de Identificación Tributaria o Carnet de Identidad</param>        
        /// <param name="dateOfTransaction">fecha de transaccion de la forma AAAAMMDD</param>        
        /// <param name="transactionAmount">Monto de la transacción</param>        
        /// <param name="dosageKey">Llave de dosificación</param>        
        /// <returns>Codigo de Control generado de la forma 6A-DC-53-05-14</returns>
        /// </summary>
        public  String generateControlCode(String authorizationNumber, String invoiceNumber, String nitci,
                           String dateOfTransaction, String transactionAmount, String dosageKey)
        {

            //redondea monto de transaccion 
            transactionAmount = roundUp(transactionAmount);
            /* ========== PASO 1 ============= */
            invoiceNumber = addVerhoeffDigit(invoiceNumber, 2);
            nitci = addVerhoeffDigit(nitci, 2);
            dateOfTransaction = addVerhoeffDigit(dateOfTransaction, 2);
            transactionAmount = addVerhoeffDigit(transactionAmount, 2);
            //se suman todos los valores obtenidos                  
            Int64 sumOfVariables=0;
            try {
                sumOfVariables = Int64.Parse(invoiceNumber)
                                  + Int64.Parse(nitci)
                                  + Int64.Parse(dateOfTransaction)
                                  + Int64.Parse(transactionAmount);
            }
            catch (FormatException e) {
                if (e.Source != null)
                    Console.WriteLine("FormatException source: {0}", e.Source);
                throw;
            }
            
            //A la suma total se añade 5 digitos Verhoeff
            String sumOfVariables5Verhoeff = addVerhoeffDigit(sumOfVariables.ToString(), 5);

            /* ========== PASO 2 ============= */
            String fiveDigitsVerhoeff = sumOfVariables5Verhoeff.Substring(sumOfVariables5Verhoeff.Length - 5);
            int[] numbers = new int[5];
            for (int i = 0; i < 5; i++)
            {
                numbers[i] = Int32.Parse(fiveDigitsVerhoeff[i].ToString()) + 1;
            }
            
            String string1 = dosageKey.Substring(0, numbers[0]);
            String string2 = dosageKey.Substring(numbers[0], numbers[1]);
            String string3 = dosageKey.Substring(numbers[0] + numbers[1], numbers[2]);
            String string4 = dosageKey.Substring(numbers[0] + numbers[1] + numbers[2], numbers[3]);
            String string5 = dosageKey.Substring(numbers[0] + numbers[1] + numbers[2] + numbers[3], numbers[4]);

            String authorizationNumberDKey = authorizationNumber + string1;
            String invoiceNumberdKey = invoiceNumber + string2;
            String NITCIDKey = nitci + string3;
            String dateOfTransactionDKey = dateOfTransaction + string4;
            String transactionAmountDKey = transactionAmount + string5;

            /* ========== PASO 3 ============= */
            //se concatena cadenas de paso 2
            String stringDKey = authorizationNumberDKey + invoiceNumberdKey + NITCIDKey + dateOfTransactionDKey + transactionAmountDKey;
            //Llave para cifrado + 5 digitos Verhoeff generado en paso 2
            String keyForEncryption = dosageKey + fiveDigitsVerhoeff;
            //se aplica AllegedRC4
            String allegedRC4String = AllegedRC4.Instance.encryptMessageRC4(stringDKey, keyForEncryption,true);

            /* ========== PASO 4 ============= */          
            //se suman valores ascii
            int totalAmount = 0;
            int sp1 = 0;
            int sp2 = 0;
            int sp3 = 0;
            int sp4 = 0;
            int sp5 = 0;
            byte[] asciiBytes = Encoding.ASCII.GetBytes(allegedRC4String);
            int tmp = 1;
            for (int i=0; i< asciiBytes.Length;i++)
            {
                totalAmount += asciiBytes[i];
                switch (tmp)
                {
                    case 1: sp1 += asciiBytes[i]; break;
                    case 2: sp2 += asciiBytes[i]; break;
                    case 3: sp3 += asciiBytes[i]; break;
                    case 4: sp4 += asciiBytes[i]; break;
                    case 5: sp5 += asciiBytes[i]; break;
                }
                tmp = (tmp < 5) ? tmp + 1 : 1;
            }

            /* ========== PASO 5 ============= */
            //suma total * sumas parciales dividido entre resultados obtenidos 
            //entre el dígito Verhoeff correspondiente más 1 (paso 2)
            int tmp1 = totalAmount * sp1 / numbers[0];
            int tmp2 = totalAmount * sp2 / numbers[1];
            int tmp3 = totalAmount * sp3 / numbers[2];
            int tmp4 = totalAmount * sp4 / numbers[3];
            int tmp5 = totalAmount * sp5 / numbers[4];
            //se suman todos los resultados
            int sumProduct = tmp1 + tmp2 + tmp3 + tmp4 + tmp5;
            //se obtiene base64
            String base64SIN = Base64SIN.Instance.convertBase64(sumProduct);

            /* ========== PASO 6 ============= */
            //Aplicar el AllegedRC4 a la anterior expresión obtenida
            return AllegedRC4.Instance.encryptMessageRC4(base64SIN, String.Concat(dosageKey,fiveDigitsVerhoeff),false);
        }
        
        /// <summary>
        /// Añade N digitos Verhoeff a una cadena de texto
        /// <param name="value">cadena donde se añadiran digitos Verhoeff</param>        
        /// <param name="max">cantidad de digitos a agregar</param>        
        /// <returns>cadena original + N digitos Verhoeff</returns>
        /// </summary>
        private  String addVerhoeffDigit(String value, int max)
        {
            for (int i = 1; i <= max; i++) {                
                value= String.Concat(value, Verhoeff.Instance.generateVerhoeff(value));
            }                
            return value;
        }
        
        /// <summary>
        /// Transforma string a numero y redondea hacia arriba
        /// <param name="value">String numero de la forma 123 | 123.45 | 123,45 </param>        
        /// <returns>String con el numero redondeado sin decimales</returns>
        /// </summary>
        private  String roundUp(String value)
        {            
            char a = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);            
            value = (a == ',') ? value.Replace(".", ",") : (a=='.')? value.Replace(",", ".") : value;
            decimal decimalVal = System.Convert.ToDecimal(value);
            //redondea a 0 decimales                                    
            return Math.Round(decimalVal, MidpointRounding.AwayFromZero).ToString();
        }

    }
}
