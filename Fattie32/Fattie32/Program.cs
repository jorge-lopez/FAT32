using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Fattie32
{
    class Program
    {
        //public sbyte[] TablaDirecciones = new sbyte[10];
        //byte[,] TablaDatos = new byte[10, 20];
        List<Datos> losDatos = new List<Datos>();

        static void Main(string[] args)
        {
            sbyte[] TablaDirecciones = new sbyte[10];
            byte[,] TablaDatos = new byte[10, 20];

            InicializarDirecciones(TablaDirecciones);
            //string wuts = "1233 is it right or is it wong?? @#^&*&5^";
            var wuts = 20.0;
            Datos d = new Datos(wuts);


            AlmacenarEnDatos(d, 20, TablaDirecciones, TablaDatos);
            
            byte[] olaquehace = LeerDatos(d, 20, TablaDirecciones, TablaDatos);
            double querollo = (double)Datos.Deserialize(d.Type, olaquehace);

            Console.WriteLine(querollo);
            BorrarDatos(d, 20, TablaDirecciones, TablaDatos);

        }

        private static byte[] LeerDatos(Datos dato, int SizeOfBlock, sbyte[] DirectoryTable, byte[,] DataTable)
        {
            int SizeOfFile = dato.Size;
            int block = dato.Dir;
            byte[] File = new byte[SizeOfFile];
            

            for (int i = 0; i < SizeOfFile; i++)
            {
                if ((i % 20) == 0 && i != 0)
                {                    
                    block = DirectoryTable[block];
                }
                int localWhereAmI = i % SizeOfBlock;
                File[i] = DataTable[block, localWhereAmI] ;                
            }

            return File;
        }
        private static void BorrarDatos(Datos dato, int SizeOfBlock, sbyte[] DirectoryTable, byte[,] DataTable)
        {
            int SizeOfFile = dato.Size;
            int block = dato.Dir;
            
            while ( DirectoryTable[block] != -1)
            {
                int tempBlock = DirectoryTable[block];
                DirectoryTable[block] = -2;
                block = tempBlock;

            }       
            DirectoryTable[block] = -2;           

        }
        private static void AlmacenarEnDatos(Datos dato, int SizeOfBlock, sbyte[] DirectoryTable, byte[,] DataTable)
        {

            

            byte[] File = dato.losBytes;
            int SizeOfFile = dato.Size;
            int i = LugarVacio(DirectoryTable);
            dato.Dir = i;

            float res = (float)SizeOfFile / 20;
            if (res > (int)res)
                res = (int)res + 1;
            if(res > HowManyFree(DirectoryTable))
                return;


            for (int WhereAmI = 0; WhereAmI < SizeOfFile; WhereAmI++)
            {
                if ((WhereAmI % 20) == 0 && WhereAmI != 0)
                {
                    //asigna el 'current' bloque como usado
                    DirectoryTable[i] = -1;
                    //busca uno nuevo y se lo asigna al 'current'
                    DirectoryTable[i] = LugarVacio(DirectoryTable);
                    //finalmente tomamos uno nuevo
                    i = LugarVacio(DirectoryTable);
                }
                int localWhereAmI = WhereAmI % SizeOfBlock;
                DataTable[i, localWhereAmI] = File[WhereAmI];
            }
            DirectoryTable[i] = -1;
        }
        private static void InicializarDirecciones(sbyte[] TablaDirecciones)
        {
            for (int i = 0; i < TablaDirecciones.Length; i++)
            {
                TablaDirecciones[i] = -2;
            }
        }

        private static sbyte LugarVacio(sbyte[] TablaDirecciones)
        {
            
            for (sbyte i = 0; i < TablaDirecciones.Length; i++)
            {
                if(TablaDirecciones[i] == -2)
                    return i;
            }
            return -3;
        }
        private static sbyte HowManyFree(sbyte[] TablaDirecciones)
        {
            sbyte contador = 0 ;
            for (int i = 0; i < TablaDirecciones.Length; i++)
            {
                if(TablaDirecciones[i] == -2)
                    contador++;
            }
            return contador;
        }

        public class Datos
        {
            public int Dir;
            public int Size { get; set; }
            public byte[] losBytes { get; set; }
            public DataType Type { get; set; }
            public enum DataType
            {

                CHAR = 'c',
                INT = 'i',
                DOUBLE = 'd',
                STRING = 's'
            }


            public Datos(int i)
            {
                losBytes = BitConverter.GetBytes(i);
                this.Size = losBytes.Length;
                this.Type = DataType.INT;
            }
            public Datos(double d)
            {
                losBytes = BitConverter.GetBytes(d);
                this.Size = losBytes.Length;
                this.Type = DataType.DOUBLE;

            }
            public Datos(char c)
            {
                losBytes = BitConverter.GetBytes(c);
                this.Size = losBytes.Length;
                this.Type = DataType.CHAR;

            }
            public Datos(string s)
            {
                List<byte> Bytes = new List<byte>();
                foreach (dynamic t in s)
                {

                    Bytes.Add(BitConverter.GetBytes(t)[0]);
                    Bytes.Add(BitConverter.GetBytes(t)[1]);

                }
                losBytes = Bytes.ToArray();

                this.Size = losBytes.Length;
                this.Type = DataType.STRING;
            }


            public static object Deserialize(Datos.DataType tipoDato, byte[] byteArray)
            {

                switch (tipoDato)
                {
                    case DataType.CHAR:
                        char c = BitConverter.ToChar(byteArray, 0);
                        return c;
                    case DataType.INT:
                        int i = BitConverter.ToInt32(byteArray, 0);
                        return i;
                    case DataType.DOUBLE:
                        double d = BitConverter.ToDouble(byteArray, 0);
                        return d;
                    case DataType.STRING:
                        string s = "";
                        for (int index = 0; index < byteArray.Length; index += 2)
                        {
                            byte[] charByte = new byte[2];
                            charByte[0] = byteArray[index];
                            charByte[1] = byteArray[index + 1];
                            s += BitConverter.ToChar(charByte, 0);
                        }
                        return s;
                    default:
                        return null;
                }
            }
        }
    }
}
