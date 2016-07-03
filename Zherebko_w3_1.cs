using System;
using System.IO;
using System.Xml.Serialization;
namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {


            
            Serializer<int> sr = new Serializer<int>();
            System.Console.WriteLine("serialazing two numbers");
            sr.Serialize("./TestDir", "kke.xml", 0);
            sr.Serialize("./TestDir", "2.xml", 234);
            Serializer<string> s =new Serializer<string>();
            System.Console.WriteLine("serialazing string");
            s.Serialize("./TestDir", "2.xml","TestDir");
            Serializer<Test> l =new Serializer<Test>();
            l.Serialize("./TestDir","lol.xml",new Test(){Finish="kke",Start=123,Date=new DateTime(1996,1,1)});
      
      System.Console.WriteLine("Deserialize Test obj");
           var er= l.Deserialize("./TestDir","lol.xml");
           System.Console.WriteLine(er);

           System.Console.WriteLine("Serialization with dynamic");
            Serializer<dynamic> dynamic_serializer =new Serializer<dynamic>();
            string name="test";
            int id=1;
            dynamic_serializer.Serialize("./TestDir","test.xml",new {name,id});


        }
    }

    public class Test
    {
        public int Start { get; set; }
        public string Finish { get; set; }
        public DateTime Date { get; set; }
        public override string ToString(){
            return $"{Start} {Finish} {Date}";
        }

    }

    internal class Serializer<T>
    {
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(T));

        public T Deserialize(string workingDirectory, string fileName)
        {
            var filePath = prepareDir(workingDirectory, fileName, typeof(T).ToString());
            using (TextReader reader = new StreamReader(File.OpenRead(filePath)))
            {
                try
                {
                    return (T)_serializer.Deserialize(reader);

                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
            return default(T);
        }
        //create directory for file and return full filePath
        private static string prepareDir(string workingDirectory, string fileName, string typeName)
        {

            var separ = Path.DirectorySeparatorChar.ToString();
            workingDirectory = workingDirectory + separ + typeName;
            var filePath = workingDirectory + separ + fileName;
            if (!Directory.Exists(workingDirectory))
            {
                Directory.CreateDirectory(workingDirectory);
            }
            return filePath;
        }

        public void Serialize(string workingDirectory, string fileName, T data)
        {
            var filePath = prepareDir(workingDirectory, fileName, data.GetType().ToString());
            using (TextWriter writer = new StreamWriter(File.OpenWrite(filePath)))
            {

                _serializer.Serialize(writer, data);
            }
        }
    }
}