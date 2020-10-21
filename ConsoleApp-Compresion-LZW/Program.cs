using System;
using System.IO;
using ClassLibrary_LAB_04_ED2;

namespace ConsoleApp_Compresion_LZW
{
    class Program
    {
        public static void Header()
        {
            Console.Clear();
            string textToEnter = "--PRÁCTICA DE LABORATORIO #4 - ESTRUCTURA DE DATOS 2--";
            string textToEnter2 = "----- Javier Andrés Morales González - 1210219 | Diego Andrés Véliz Arauz - 1230019 -----";
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (textToEnter.Length / 2)) + "}", textToEnter));
            Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (textToEnter2.Length / 2)) + "}", textToEnter2));
            Console.WriteLine("\n");
            Console.ResetColor();
        }

        public static void TitleOption1()
        {
            string t = "--COMPRESIÓN LZW--";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (t.Length / 2)) + "}", t) + "\n");
            Console.ResetColor();
        }
        public static void TitleOption2()
        {
            string t = "--DESCOMPRESIÓN LZW--";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (t.Length / 2)) + "}", t) + "\n");
            Console.ResetColor();
        }
        static void Main(string[] args)
        {
            LZW CompresorCrack = new LZW();
            Header();
            TitleOption1();
            bool exit = false;
            while (!exit)
            {
                try
                {
                    Header();
                    TitleOption1();
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    //using FileStream file = new FileStream("C:\\Users\\Diego Veliz\\Desktop\\easy test.txt", FileMode.OpenOrCreate);
                    //using BinaryReader Lector = new BinaryReader(file);
                    //int Cant_Byte_Read = 10000;
                    //int Aumentar_Max = 1;
                    //byte[] Text = new byte[Cant_Byte_Read];
                    //Text = Lector.ReadBytes(Cant_Byte_Read);
                    //while (file.Position < file.Length)
                    //{
                    //    byte[] Aux = Lector.ReadBytes(Cant_Byte_Read);
                    //    Array.Resize(ref Text, Text.Length + Aux.Length);
                    //    Aux.CopyTo(Text, Cant_Byte_Read * Aumentar_Max);
                    //    Aumentar_Max++;
                    //}
                    //Lector.Close();
                    //byte[] Impresor = CompresorCrack.Compresion(Text);

                    //using FileStream StreFight = new FileStream("C:\\Users\\Diego Veliz\\Desktop\\EasyCompress.txt", FileMode.OpenOrCreate);
                    //using BinaryWriter Escritor = new BinaryWriter(StreFight);
                    //Escritor.Write(Impresor);
                    //Escritor.Close();

                    using FileStream file = new FileStream("C:\\Users\\Diego Veliz\\Desktop\\cuentoComprimido.txt", FileMode.OpenOrCreate);
                    using BinaryReader Lector = new BinaryReader(file);
                    int Cant_Byte_Read = 10000;
                    int Aumentar_Max = 1;
                    byte[] Text = new byte[Cant_Byte_Read];
                    Text = Lector.ReadBytes(Cant_Byte_Read);
                    while (file.Position < file.Length)
                    {
                        byte[] Aux = Lector.ReadBytes(Cant_Byte_Read);
                        Array.Resize(ref Text, Text.Length + Aux.Length);
                        Aux.CopyTo(Text, Cant_Byte_Read * Aumentar_Max);
                        Aumentar_Max++;
                    }
                    Lector.Close();
                    byte[] Impresor = CompresorCrack.Descompresion(Text);

                    using FileStream StreFight = new FileStream("C:\\Users\\Diego Veliz\\Desktop\\Resultado.txt", FileMode.OpenOrCreate);
                    using BinaryWriter Escritor = new BinaryWriter(StreFight);
                    Escritor.Write(Impresor);
                    Escritor.Close();

                    //Console.WriteLine("Ingrese el texto a comprimir:\n\n");
                    //Console.ResetColor();
                    //string Text = Console.ReadLine();
                    //if (String.IsNullOrEmpty(Text))
                    //{
                    //    throw new FormatException();
                    //}
                    //byte[] texto = new byte[Text.Length];
                    //for (int i = 0; i < Text.Length; i++)
                    //{
                    //    texto[i] = Convert.ToByte(Convert.ToChar(Text[i]));
                    //}
                    //byte[] Comprimido = CompresorCrack.Compresion(texto);
                    //string result = "";
                    //foreach (byte bit in Comprimido)
                    //{
                    //    result += Convert.ToString(Convert.ToChar(bit));
                    //}
                    //Console.ForegroundColor = ConsoleColor.Yellow;
                    //Console.WriteLine("\n\nEl texto comprimido es el siguiente:\n\n");
                    //Console.ResetColor();
                    //Console.WriteLine(result);
                    //Console.ForegroundColor = ConsoleColor.Yellow;
                    //Console.WriteLine("\n\nPresione cualquier tecla para ver el mismo texto descompreso.");
                    //Console.ResetColor();
                    //Console.ReadKey();
                    //Console.Clear();
                    //Header();
                    //TitleOption2();
                    //result = "";
                    //byte[] Descomprimido = CompresorCrack.Descompresion(Comprimido);
                    //foreach (byte bit in Descomprimido)
                    //{
                    //    result += Convert.ToString(Convert.ToChar(bit));
                    //}
                    //Console.ForegroundColor = ConsoleColor.Yellow;
                    //Console.WriteLine("\n\nEl texto descomprimido es el siguiente:");
                    //Console.ResetColor();
                    //Console.WriteLine("\n" + result + "\n\n");
                    //Console.ForegroundColor = ConsoleColor.Yellow;
                    //Console.WriteLine("Proceso finalizado. Presione una tecla.");
                    //Console.ResetColor();
                    //Console.ReadKey();
                    //Header();
                    //Console.ForegroundColor = ConsoleColor.Magenta;
                    string e = "Ingrese S para volver a comprimir o cualquier cosa para salir del programa.";
                    Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (e.Length / 2)) + "}", e) + "\n");
                    Console.ResetColor();
                    if (!Convert.ToString(Console.ReadLine()).Equals("S"))
                    {
                        exit = true;
                    }
                }
                catch
                {
                    Header();
                    string e = "Ocurrió un error. Presione una tecla para volver a comprimir.";
                    Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (e.Length / 2)) + "}", e) + "\n");
                    int Tam = CompresorCrack.TamTabl();
                    Console.ReadKey();
                }
            }
        }
    }
}
