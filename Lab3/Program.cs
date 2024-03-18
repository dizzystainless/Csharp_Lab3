using System;
using System.IO;
using System.Linq;
using System.Text;


namespace Lab3
{
    class Program
    {

        public static BinaryReader binaryReader;

        //Arrayer för signaturerna för PNG och BMP
        public static byte[] pngSignature = { 137, 80, 78, 71, 13, 10, 26, 10 };
        public static byte[] bmpSignature = { 66, 77 };

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            string ImagePath = @"C:../../pink.jpg";

            //Kontrollera om det finns en fil för den aktuella sökvägen
            //med ett villkor om den inte existerar
            if (!File.Exists(ImagePath))
            {
                Console.WriteLine("File not found!");
                return;
            }

            //Läsa in bytes i filen
            binaryReader = new BinaryReader(new MemoryStream(File.ReadAllBytes(ImagePath)));

            //Kontrollera om signaturen stämmer med BMP..
            var imageSignature = binaryReader.ReadBytes(2); 

            if (Enumerable.SequenceEqual(imageSignature, bmpSignature))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(ImagePath, FileMode.Open)))
                {
                    //Varning för lite Frankenstein-kod!
                    //Det var inte såhär det var tänkt att man skulle göra men det funkar XD
                    var jump1 = reader.ReadUInt64();
                    var jump2 = reader.ReadUInt64();
                    var jump3 = reader.ReadUInt16();
                    var resolution1 = reader.ReadUInt16();
                    var anotherJump = reader.ReadUInt16();
                    var resolution2 = reader.ReadUInt16();

                    Console.WriteLine($"The file is of BMP format with resolution: {resolution1} x {resolution2} pixels");
                }
            }
            // ..annars kontrollera om signaturen stämmer med PNG
            else
            {
                binaryReader = new BinaryReader(new MemoryStream(File.ReadAllBytes(ImagePath)));
                
                var imageSignature2 = binaryReader.ReadBytes(8);

                if (Enumerable.SequenceEqual(imageSignature2, pngSignature))
                {   
                    using (BinaryReader reader = new BinaryReader(File.Open(ImagePath, FileMode.Open)))
                    {
                        var jump1 = reader.ReadSingle();
                        var jump2 = reader.ReadString();
                        var resolution1 = reader.ReadUInt16();
                        var anotherJump = reader.ReadUInt16();
                        var resolution2 = reader.ReadInt16();

                        //Vända ordningen på bytes (så att det går att läsa som Big Endian) 
                        //för att pixlarna ska bli rätt format
                        byte[] bytes = BitConverter.GetBytes(resolution1);

                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(bytes);
                            var width = BitConverter.ToUInt16(bytes);
                            resolution1 = width;
                        }

                        bytes = BitConverter.GetBytes(resolution2);

                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(bytes);
                            var height = BitConverter.ToUInt16(bytes);
                            resolution2 = (short)height;
                        }

                        Console.WriteLine($"The file is of PNG format with resolution: {resolution1} x {resolution2} pixels");
                    }
                }
                else
                {
                    Console.WriteLine("Not a valid .png or .bmp file!");
                }
            }
           
            Console.ReadKey();
        }
    }
}
