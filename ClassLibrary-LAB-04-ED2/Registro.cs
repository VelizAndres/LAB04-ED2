using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary_LAB_04_ED2
{
    class Registro
    {
        public byte[] Cadena;
        public int Id;

        public static bool Compardor_Bytes(byte[] Uno, byte[] Dos)
        {
            if (Uno.Length == Dos.Length)
            {
                for (int i = 0; i < Uno.Length; i++)
                {
                    if (Uno[i] != Dos[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
