using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using ClassLibrary_LAB_04_ED2;


namespace API_LAB04.Models
{
    public class Compression
    {
        public string originalName { get; set; }
        public string compressedFilePath { get; set; }
        public double compressionRatio { get; set; }
        public double compressionFactor { get; set; }
        public double reductionPercentage { get; set; }

        public static List<Compression> GetAllCompressions()
        {
            try
            {
                List<Compression> All = new List<Compression>();
                using (var reader = new StreamReader(Directory.GetCurrentDirectory() + "\\compressions.json"))
                {
                    string registry;
                    registry = reader.ReadToEnd();
                    JsonSerializerOptions rule = new JsonSerializerOptions { IgnoreNullValues = true };
                    All = JsonSerializer.Deserialize<List<Compression>>(registry, rule);
                }
                return All;
            }
            catch { return null; }
        }

        public static void WriteRegistry(string OriginalName, string CompressedFilePath, double CompressionRatio, double CompressionFactor, double ReductionPercentage)
        {
            string path = Path.GetFullPath(Directory.GetCurrentDirectory() + "\\compressions.json");
            if (!File.Exists(path))
            {
                using (FileStream creator = File.Create(path)) { };
            }
            using (var reader = new StreamReader(path))
            {
                JsonSerializerOptions rule = new JsonSerializerOptions { IgnoreNullValues = true };
                List<Compression> All = new List<Compression>();
                Compression tempCompression = new Compression
                {
                    originalName = OriginalName,
                    compressedFilePath = CompressedFilePath,
                    compressionRatio = CompressionRatio,
                    compressionFactor = CompressionFactor,
                    reductionPercentage = ReductionPercentage,
                };
                try
                {
                    All = JsonSerializer.Deserialize<List<Compression>>(reader.ReadToEnd(), rule);
                    All.Add(tempCompression);
                    reader.Close();
                }
                catch
                {
                    All.Add(tempCompression);
                    reader.Close();
                }
                using (var writer = new StreamWriter(path))
                {
                    var AllRegistries = JsonSerializer.Serialize<List<Compression>>(All, rule);
                    writer.Write(AllRegistries);
                }
            }
        }

        public static string DecompressFile(string filePath)
        {
            
            LZW Compresor_lzw = new LZW();
            FileStream file = new FileStream(filePath, FileMode.OpenOrCreate);
            BinaryReader Lector = new BinaryReader(file);
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

            byte[] TextoComprimido = Text;
            int cant_CName = TextoComprimido[0];
            string Name_Original = "";
            for (int i = 1; i <= cant_CName; i++)
            {
                Name_Original += Convert.ToChar(TextoComprimido[i]);
            }
            byte[] Data_retorna = new byte[TextoComprimido.Length - (cant_CName + 1)];
            Array.Copy(TextoComprimido, (cant_CName + 1), Data_retorna, 0, Data_retorna.Length);

            byte[] Impresor = Compresor_lzw.Descompresion(Data_retorna);
            string FinalFileName = Directory.GetCurrentDirectory() + "\\Decompressed\\" + Name_Original;
            var extension = FinalFileName.Split('.');
            string ext = extension[extension.Length - 1];
            string name = Name_Original.Substring(0, Name_Original.Length - ext.Length - 1);
            int count = 0;
            while (File.Exists(FinalFileName))
            {
                count++;
                FinalFileName = Directory.GetCurrentDirectory() + "\\Decompressed\\" + name + count + "." + ext;
            }
            FileStream archivo = new FileStream(FinalFileName, FileMode.OpenOrCreate);
            BinaryWriter Escritor = new BinaryWriter(archivo);
            Escritor.Write(Impresor);
            Escritor.Close();
            File.Delete(filePath);
            return Name_Original;
        }


        public static void CompressFile(string filePath, string filename, string name)
        {
            LZW Compresor_lzw = new LZW();
            FileStream fileC = new FileStream(filePath, FileMode.OpenOrCreate);
            BinaryReader Lector = new BinaryReader(fileC);
            int Cant_Byte_Read = 10000;
            int Aumentar_Max = 1;
            byte[] Text = new byte[Cant_Byte_Read];
            Text = Lector.ReadBytes(Cant_Byte_Read);
            while (fileC.Position < fileC.Length)
            {
                byte[] Aux = Lector.ReadBytes(Cant_Byte_Read);
                Array.Resize(ref Text, Text.Length + Aux.Length);
                Aux.CopyTo(Text, Cant_Byte_Read * Aumentar_Max);
                Aumentar_Max++;
            }
            Lector.Close();
            byte[] Impresor = Compresor_lzw.Compresion(Text);
            byte[] Result = new byte[filename.Length + 1];
            Result[0] = Convert.ToByte(filename.Length);
            for (int i = 1; i <= filename.Length; i++)
            {
                Result[i] = Convert.ToByte(Convert.ToChar(filename[i - 1]));
            }
            Array.Resize(ref Result, filename.Length + 1 + Impresor.Length);
            Impresor.CopyTo(Result, filename.Length + 1);
            string FinalFileName = Directory.GetCurrentDirectory() + "\\Compressed\\" + name + ".lzw";
            int count = 0;
            while (File.Exists(FinalFileName))
            {
                count++;
                FinalFileName = Directory.GetCurrentDirectory() + "\\Compressed\\" + name + count + ".lzw";
            }
            FileStream archivo = new FileStream(FinalFileName, FileMode.OpenOrCreate);
            BinaryWriter Escritor = new BinaryWriter(archivo);
            Escritor.Write(Result);
            Escritor.Close();
            double[] data = Compresor_lzw.Datos_Compresion();
            Compression.WriteRegistry(filename, Directory.GetCurrentDirectory() + "\\Compressed\\" + name + ".lzw", data[0], data[1], data[2]);
            File.Delete(filePath);
        }
    }
}
