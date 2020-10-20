using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ClassLibrary_LAB_03_ED2_URL;

namespace ClassLibrary_LAB_04_ED2
{
    public class LZW : ICompressor
    {
        Dictionary<byte[], Registro> Tabla;
        public byte[] Compresion(byte[] Text_Original)
        {
            ByteEqualityComparer RegComparer = new ByteEqualityComparer();
            //   Dictionary<byte[], Registro> StartDiccionario = new Dictionary<byte[], Registro>();
            Tabla = new Dictionary<byte[], Registro>(RegComparer);
            byte[] Meta_Data = Crear_Tabla(Text_Original);
            int Tam_Data = Meta_Data.Length;
            byte[] Resultado = Create_Compression(Text_Original);
            Array.Resize(ref Meta_Data, Meta_Data.Length + Resultado.Length);
            Resultado.CopyTo(Meta_Data, Tam_Data);
            return Meta_Data;
        }

        /// <summary>
        /// Metodo para obtener todos los caracteres distintos e ingrearlos al diccionario
        /// </summary>
        /// <param name="Texto">Texto a Comprimir</param>
        private byte[] Crear_Tabla(byte[] Texto)
        {
            foreach (byte Caracter in Texto)
            {
                Registro Nuevo = new Registro() { Cadena = new byte[1] };
                Nuevo.Cadena[0] = Caracter;
                bool Step = Tabla.ContainsKey(Nuevo.Cadena);
                if (!Step)
                {
                    Nuevo.Id = Tabla.Count + 1;
                    Tabla.Add(Nuevo.Cadena, Nuevo);
                }
            }
            byte[] Data = new byte[1 + Tabla.Count];
            var Datos = Tabla.Values;
            int posicion = 0;
            Data[posicion] = Convert.ToByte(Tabla.Count);
            foreach (Registro Item in Datos)
            {
                posicion++;
                Data[posicion] = Item.Cadena[0];
            }
            return Data;
        }

        private byte[] Create_Compression(byte[] Texto)
        {
            int[] Result = new int[0];
            int Mayor = 0;
            int Cant_Char = 0;
            for (int i = 0; i < Texto.Length; i++)
            {
                Cant_Char = 0;
                Registro Nuevo = new Registro() { Cadena = new byte[1] };
                Nuevo.Cadena[Cant_Char] = Texto[i];
                byte[] aux = new byte[0];
                while (Tabla.ContainsKey(Nuevo.Cadena) && (Cant_Char + i) < Texto.Length)
                {
                    Array.Resize(ref aux, aux.Length + 1);
                    aux[Cant_Char] = Texto[(Cant_Char+i)];
                    Array.Resize(ref Nuevo.Cadena, Nuevo.Cadena.Length + 1);
                    Cant_Char++;
                    Nuevo.Cadena[Cant_Char] = Texto[(Cant_Char+i)];
                }
                Nuevo.Id = Tabla.Count + 1;
                Tabla.Add(Nuevo.Cadena, Nuevo);
                Array.Resize(ref Result, (Result.Length + 1));
                if(Tabla.Count==122)
                {
                    string paro = "simon"; 
                }
                Result[Result.Length - 1] = Tabla[aux].Id;
                if (Result[Result.Length - 1] > Mayor)
                {
                    Mayor = Result[Result.Length - 1];
                }
            }
            int Cant_Bits_Necesarios = Convert.ToString(Convert.ToInt32(Mayor), 2).Length;
            byte[] Txt_Compres = Send_Text_Compress(Cant_Bits_Necesarios, Result);
            byte[] Resultado_Compress = new byte[1 + Txt_Compres.Length];
            Resultado_Compress[0] = Convert.ToByte(Cant_Bits_Necesarios);
            for (int i = 1; i < Resultado_Compress.Length; i++)
            {
                Resultado_Compress[i] = Txt_Compres[i - 1];
            }
            return Txt_Compres;
        }

        private byte[] Send_Text_Compress(int Cant_Bits_Necesarios, int[] Contenedor)
        {
            string binarios = "";
            byte[] Resultado = new byte[(Contenedor.Length * Cant_Bits_Necesarios) / 8];
            int posicion = 0;
            foreach (int Num in Contenedor)
            {
                string Num_binario = Convert.ToString(Convert.ToInt32(Num), 2);
                while (Num_binario.Length < Cant_Bits_Necesarios)
                {
                    Num_binario = "0" + Num_binario;
                }
                binarios += Num_binario;
                if (binarios.Length == 8)
                {
                    string aux = binarios.Substring(0, 8);
                    Resultado[posicion] = Convert.ToByte(Convert.ToInt32(aux, 2));
                    binarios = binarios.Remove(0, 8);
                    posicion++;
                }
            }
            if (!string.IsNullOrEmpty(binarios))
            {
                while (binarios.Length < 8)
                {
                    binarios += "0";
                }
                string aux = binarios.Substring(0, 8);
                Resultado[posicion] = Convert.ToByte(Convert.ToInt32(aux, 2));
                binarios = binarios.Remove(0, 8);
            }
            return Resultado;
        }



        public byte[] Descompresion(byte[] LZWCompressedText)
        {
            throw new NotImplementedException();
        }




        /// <summary>
        /// Metodo que devuelve los valores de compresión
        /// </summary>
        /// <returns>[0] Razón compresión, [1] Factor Compresión, [2] Porcentaje Reduccion</returns>
        //public double[] Datos_Compresion()
        //{
        //    double razon_compresion = Convert.ToDouble(Tam_Compress) / Convert.ToDouble(Tam_Original);
        //    double Factor_Compresion = Convert.ToDouble(Tam_Original) / Convert.ToDouble(Tam_Compress);
        //    double Porcentaje_Reduccion = 100 * (Convert.ToDouble((Tam_Original - Tam_Compress)) / Convert.ToDouble(Tam_Original));
        //    return new double[3] { razon_compresion, Factor_Compresion, Porcentaje_Reduccion };
        //}
    }
}
