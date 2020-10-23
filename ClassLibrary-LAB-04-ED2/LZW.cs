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
        Dictionary<int,Registro> Tabla_Descompres;
        int Tam_Original;
        int Tam_Comprimido;

  

        /// <summary>
        /// Implementación del Metodo de compresion de la Interfaz ICompressor
        /// </summary>
        /// <param name="Text_Original">Texto a comprimir</param>
        /// <returns>Entrada comprimida</returns>
        public byte[] Compresion(byte[] Text_Original)
        {
            Tam_Original = Text_Original.Length;
            ByteEqualityComparer RegComparer = new ByteEqualityComparer();
            Tabla = new Dictionary<byte[], Registro>(RegComparer);
            byte[] Meta_Data = Crear_Tabla(Text_Original);
            int Tam_Data = Meta_Data.Length;
            byte[] Resultado = Create_Compression(Text_Original);
            Array.Resize(ref Meta_Data, Meta_Data.Length + Resultado.Length);
            Resultado.CopyTo(Meta_Data, Tam_Data);
            return Meta_Data;
        }

        /// <summary>
        /// Metodo para obtener todos los caracteres distintos e ingrearlos al diccionario Tabla
        /// </summary>
        /// <param name="Texto">Texto de entrada a comprimir</param>
        /// <returns>Metada de la Tabla de valores iniciales</returns>
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
            //Add %256 
            Data[posicion] = Convert.ToByte(Tabla.Count%256);
            foreach (Registro Item in Datos)
            {
                posicion++;
                Data[posicion] = Item.Cadena[0];
            }
            return Data;
        }

        /// <summary>
        /// Metodo que realiza el proceso de obtención de ID del texto entrada
        /// </summary>
        /// <param name="Texto">Texto de entrada a comprimir</param>
        /// <returns>Texto Comprimido</returns>
        private byte[] Create_Compression(byte[] Texto)
        {
            int[] Result = new int[0];
            int Mayor = 0;
            int Cant_Char = 0;
            bool Pass_Max = false;
            for (int i = 0; i < Texto.Length; i++)
            {
                Cant_Char = 0;
                Registro Nuevo = new Registro() { Cadena = new byte[1] };
                Nuevo.Cadena[Cant_Char] = Texto[i];
                byte[] aux = new byte[0];
                while (Tabla.ContainsKey(Nuevo.Cadena) && !Pass_Max)
                {
                    Array.Resize(ref aux, aux.Length + 1);
                    aux[Cant_Char] = Texto[(Cant_Char+i)];
                    Array.Resize(ref Nuevo.Cadena, Nuevo.Cadena.Length + 1);
                    Cant_Char++;
                     if((Cant_Char + i) < Texto.Length)
                    {
                        Nuevo.Cadena[Cant_Char] = Texto[(Cant_Char + i)];
                    }
                    else
                    {
                        Pass_Max = true;
                    }
                }
                i += aux.Length - 1;
                if (!Pass_Max)
                {
                    Nuevo.Id = Tabla.Count + 1;
                    Tabla.Add(Nuevo.Cadena, Nuevo);
                }
                Array.Resize(ref Result, (Result.Length + 1));
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
            return Resultado_Compress;
        }

        /// <summary>
        /// Metodo que convierte el arreglo de ID a Texto Comprimido
        /// </summary>
        /// <param name="Cant_Bits_Necesarios">Cantidad de bits necesarios para la escritura del ID mayor</param>
        /// <param name="Contenedor">Arreglo de int que contine los ID</param>
        /// <returns>Texto Comprimido</returns>
        private byte[] Send_Text_Compress(int Cant_Bits_Necesarios, int[] Contenedor)
        {
            string binarios = "";
            byte[] Resultado = new byte[Convert.ToInt32(Math.Ceiling((Convert.ToDouble(Contenedor.Length) * Cant_Bits_Necesarios) / 8))];
            int posicion = 0;
            foreach (int Num in Contenedor)
            {
                string Num_binario = Convert.ToString(Convert.ToInt32(Num), 2);
                while (Num_binario.Length < Cant_Bits_Necesarios)
                {
                    Num_binario = "0" + Num_binario;
                }
                binarios += Num_binario;
                while(binarios.Length >= 8)
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
            Tam_Comprimido = Resultado.Length;
            return Resultado;
        }



        /// <summary>
        ///  Implementación del Metodo de Descompresión de la Interfaz ICompressor
        /// </summary>
        /// <param name="CompressedText">Texto Comprimido</param>
        /// <returns>Entrada de texto Descomprimida</returns>
        public byte[] Descompresion(byte[] CompressedText)
        {
            Tabla_Descompres = new Dictionary<int, Registro>();
            byte Cant_Tabla = CompressedText[0];
            int posicion = Convert.ToInt32(Cant_Tabla);
            if(posicion==0)
            {
                posicion = 256;
            }
            Get_Tabla(CompressedText, posicion);
            byte[] Result = Get_Text_Descompress(CompressedText, posicion + 1); 
            return Result;
        }

        /// <summary>
        /// Metodo para ingresar a la Tabla_Descompress los valores iniciales
        /// </summary>
        /// <param name="Text_Compress">Entrada de texto comprimida</param>
        /// <param name="Final_PosTable">Posición hasta la que se leerán los datos de la Metadata</param>
        private void Get_Tabla(byte[] Text_Compress, int Final_PosTable)
        {
            for (int i = 1; i <= Final_PosTable; i++)
            {
                Registro Nuevo = new Registro() { Cadena = new byte[1] };
                Nuevo.Cadena[0] = Text_Compress[i];
                Nuevo.Id = i;
                Tabla_Descompres.Add(Nuevo.Id, Nuevo);
            }
        }

        /// <summary>
        /// Metodo para realizar la descompresión de la entrada de Texto
        /// </summary>
        /// <param name="Text_Compress">Entrada de texto comprimida</param>
        /// <param name="Position_Start">Posición a partir de la cual se leerá el texto comprimido</param>
        /// <returns>Entrada de texto descomprimida</returns>
        private byte[]  Get_Text_Descompress(byte[] Text_Compress, int Position_Start)
        {
            int Cant_Bits = Text_Compress[Position_Start];
            int aux = 0;
            int anterior = -1;
            string aux_binario = "";
            byte[] Result = new byte[0];
            for(int i=Position_Start+1;i<Text_Compress.Length;i++)
            {
                aux_binario += Convert.ToString(Convert.ToInt32(Text_Compress[i]), 2).PadLeft(8,Convert.ToChar("0"));
                while(aux_binario.Length >= Cant_Bits && anterior!=0)
                {
                    
                    string Bits = aux_binario.Substring(0, Cant_Bits);
                    aux_binario = aux_binario.Remove(0, Cant_Bits);
                    aux = Convert.ToInt32(Bits, 2);
                    if (aux != 0)
                    {
                        if (Tabla_Descompres.ContainsKey(aux))
                        {
                            int Tam = Tabla_Descompres[aux].Cadena.Length;
                            Array.Resize(ref Result, (Result.Length + Tam));
                            Array.Copy(Tabla_Descompres[aux].Cadena, 0, Result, Result.Length - Tam, Tam);

                            if (!(anterior == -1))
                            {
                                Registro Nuevo = new Registro();
                                Nuevo.Cadena = Tabla_Descompres[anterior].Cadena;
                                Array.Resize(ref Nuevo.Cadena, (Nuevo.Cadena.Length + 1));
                                Nuevo.Cadena[Nuevo.Cadena.Length - 1] = Tabla_Descompres[aux].Cadena[0];
                                Nuevo.Id = Tabla_Descompres.Count + 1;
                                Tabla_Descompres.Add(Nuevo.Id, Nuevo);
                            }
                        }
                        else
                        {
                            Registro Nuevo = new Registro();
                            Nuevo.Cadena = Tabla_Descompres[anterior].Cadena;
                            Array.Resize(ref Nuevo.Cadena, (Nuevo.Cadena.Length + 1));
                            Nuevo.Cadena[Nuevo.Cadena.Length - 1] = Tabla_Descompres[anterior].Cadena[0];
                            Nuevo.Id = Tabla_Descompres.Count + 1;
                            Tabla_Descompres.Add(Nuevo.Id, Nuevo);
                            int Tam = Tabla_Descompres[aux].Cadena.Length;
                            Array.Resize(ref Result, (Result.Length + Tam));
                            Array.Copy(Tabla_Descompres[aux].Cadena, 0, Result, Result.Length - Tam, Tam);
                        }

                    }
                    anterior = aux;
                }
            }
            return Result;
        }



        /// <summary>
        /// Metodo para obtener los valores de compresión
        /// </summary>
        /// <returns>[0] Razón compresión, [1] Factor Compresión, [2] Porcentaje Reduccion</returns>
        public double[] Datos_Compresion()
        {
            double razon_compresion = Convert.ToDouble(Tam_Comprimido) / Convert.ToDouble(Tam_Original);
            double Factor_Compresion = Convert.ToDouble(Tam_Original) / Convert.ToDouble(Tam_Comprimido);
            double Porcentaje_Reduccion = 100 * (Convert.ToDouble((Tam_Original - Tam_Comprimido)) / Convert.ToDouble(Tam_Original));
            return new double[3] { razon_compresion, Factor_Compresion, Porcentaje_Reduccion };
        }
    }
}
