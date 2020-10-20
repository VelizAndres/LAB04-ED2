using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary_LAB_04_ED2
{
    interface ICompressor
    {
        public byte[] Compression(byte[] NonCompressedText);
        public byte[] Descompresion(byte[] LZWCompressedText);
    }
}
