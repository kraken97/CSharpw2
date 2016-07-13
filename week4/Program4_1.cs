using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ConsoleApplication
{
    public abstract class BaseParseObject
    {

    }
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class RegexAttribute : System.Attribute
    {
        private readonly string regex;
        public string Regex => regex;

        // This is a positional argument
        public RegexAttribute(string regex)
        {
            this.regex = regex;
        }
    }
    //wrapper for function and type what it pars
    public class Parser
    {
        public Func<MatchCollection, IEnumerable<BaseParseObject>> ParseFunc { get; set; }
        public Type ParserType { get; set; }

    }

    public class StringParser
    {

        public static string GetParseRule(Type t)
        {
            return t.GetTypeInfo().GetCustomAttribute<RegexAttribute>().Regex;
        }
        //   public static MethodInfo GetParser(Type t)
        // {
        //     return t.GetTypeInfo().GetMethod("Parse");
        // }


        public static IEnumerable<BaseParseObject> Parse(string str, params Parser[] parsersList)
        {
            List<BaseParseObject> list = new List<BaseParseObject>();

            for (int i = 0; i < parsersList.Length; i++)
            {


                string rgx = GetParseRule(parsersList[i].ParserType);
                MatchCollection matches = Regex.Matches(str, rgx);
                IEnumerable<BaseParseObject> res = parsersList[i].ParseFunc(matches);
                List<BaseParseObject> listRes = res?.ToList();
                if (listRes != null)
                {
                    //System.Console.WriteLine(listRes);
                    for (int j = 0; j < listRes.Count; j++)
                    {
                        yield return listRes[j];
                    }
                }


            }
        }
    }




    [RegexAttribute(@"(\w+)\ssay\s((\w+\s{0,})+)\.")]
    internal class Phraze : BaseParseObject
    {
        public string Speaker { get; set; }
        public string Words { get; set; }
        public override string ToString()
        {
            return $"phraze {Speaker}  say {Words}";
        }


        public static IEnumerable<BaseParseObject> Parse(MatchCollection m)
        {
            for (int i = 0; i < m.Count; i++)
            {
                var item = m[i];
                var speaker = item.Groups[1].Value;
                var words = item.Groups[2].Value;
                yield return new Phraze() { Speaker = speaker, Words = words };
            }
        }

    }
    [RegexAttribute(@"my\sfirst\sname\sis\s(\w+)\sand\ssecond\sname\sis\s(\w+)\sand\smy\sage\sis\s(\d{1,2})\.")]
    internal class Name : BaseParseObject
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public static IEnumerable<BaseParseObject> Parse(MatchCollection m)
        {


            for (int i = 0; i < m.Count; i++)
            {
                var item = m[i];
                var n1 = item.Groups[1].Value;
                var n2 = item.Groups[2].Value;
                var n3 = item.Groups[3].Value;
                yield return new Name() { FirstName = n1, LastName = n2, Age = int.Parse(n3) };
            }
        }

        public override string ToString()
        {
            return $"name :{this.FirstName}  second name:{this.LastName}  age :{Age}";
        }
    }
    [RegexAttribute(@"box\ssizes\sis\s(\d{0,})\s(\d+)\s(\d+)\.")]
    internal class BoxSize : BaseParseObject
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }

        public override String ToString()
        {
            return $"box sizes {Height} {Width}  {Length}";
        }

        public static IEnumerable<BaseParseObject> Parse(MatchCollection m)
        {
            for (int i = 0; i < m.Count; i++)
            {
                var item = m[i];
                var n1 = int.Parse(item.Groups[1].Value);
                var n2 = int.Parse(item.Groups[2].Value);
                var n3 = int.Parse(item.Groups[3].Value);
                yield return new BoxSize() { Width = n1, Height = n2, Length = n3 };
            }
        }

    }
    [RegexAttribute(@"Team\s(\w+)\shas\s(\d{0,2})\splayers\swith\sav\sage\s(\d{0,2})\.")]
    internal class FootballTeam : BaseParseObject
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public int AvAge { get; set; }


        public static IEnumerable<BaseParseObject> Parse(MatchCollection m)
        {
            for (int i = 0; i < m.Count; i++)
            {
                var item = m[i];
                var n1 = item.Groups[1].Value;
                var n2 = int.Parse(item.Groups[2].Value);
                var n3 = int.Parse(item.Groups[3].Value);
                yield return new FootballTeam() { Name = n1, Size = n2, AvAge = n3 };
            }
        }

        public override string ToString()
        {
            return $"Footbal team {Name} {Size} {AvAge}";
        }
    }
    [RegexAttribute(@"(Mr|Mrs|Ms)\s(\w+)\s(\w+)\swas\sborn\son\s(\d{4}/\d\d/\d\d)\.")]
    internal class Person : BaseParseObject
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Gender { get; set; }

        public override string ToString()
        {
            string gender = Gender ? "man" : "woman";
            return $"Person {FirstName} {SecondName} {BirthDate} {gender}";
        }
        public static IEnumerable<BaseParseObject> Parse(MatchCollection m)
        {


            for (int i = 0; i < m.Count; i++)
            {
                var item = m[i];
                var n1 = item.Groups[1].Value;
                var n2 = item.Groups[2].Value;
                var n3 = item.Groups[3].Value;
                var n4 = item.Groups[4].Value;
                yield return new Person()
                {
                    FirstName = n1,
                    SecondName = n2,
                    Gender = n3.Equals("Mr.") ? true : false,
                    BirthDate = DateTime.ParseExact(n4, "yyyy/MM/dd", CultureInfo.InvariantCulture)
                };
            }
        }

    }


    //maybe all regex should be like this one,  but i am to lazy to rework they
    [RegexAttribute(@"Laptop\s(?<Model>\w+)\s+(?<Price>\d+)\s+(?<Weight>\d{1,2})\s+(?<Color>\w+)")]
    internal class Laptop : BaseParseObject
    {
        public string Model { get; set; }
        public int Price { get; set; }
        public double Weight { get; set; }
        public string Color { get; set; }


        public override string ToString()
        {
            return $"Laptop {Price}  {Weight}  {Color}  {Model}";
        }


        public static IEnumerable<BaseParseObject> Parse(MatchCollection m)
        {

            for (int i = 0; i < m.Count; i++)
            {
                var groups = m[i].Groups;
                var _Price = int.Parse(groups["Price"].Value);
                var _Model = groups["Model"].Value;
                var _Weight = double.Parse(groups["Weight"].Value);
                var _Color = groups["Color"].Value;
                yield return new Laptop() { Price = _Price, Color = _Color, Weight = _Weight, Model = _Model };
            }

        }
    }

    [RegexAttribute(@"Animal\s(?<Class>\w+)\s(?<Species>\w+)\s(?<MaxAge>\d{1,4})\s(?<Continent>\w+)")]
    internal class Animal : BaseParseObject
    {
        public string Class { get; set; }
        public string Species { get; set; }
        public int MaxAge { get; set; }
        public string Continent { get; set; }


        public override string ToString()
        {
            return $"Animal {Class}  {Species} {MaxAge} {Continent}";
        }



        public static IEnumerable<BaseParseObject> Parse(MatchCollection m)
        {

            for (int i = 0; i < m.Count; i++)
            {
                var groups = m[i].Groups;
                var _Class = groups["Class"].Value;
                var _Continent = groups["Continent"].Value;
                var _MaxAge = int.Parse(groups["MaxAge"].Value);
                var _Species = groups["Species"].Value;
                yield return new Animal() { Class = _Class, Continent = _Continent, Species = _Species, MaxAge = _MaxAge };
            }

        }
    }



    public class Program
    {
        public static void Main(string[] args)
        {

            string rawText = "i say hi. my first name is TT and second name is T and my age is 18.  box sizes is 2 4 6. Team Dynamo has 22 players with av age 30. Mr kek lol was born on 1999/12/12. Animal Monkey Primate 34 Africa Laptop Asus 3000 45 black";
            var parsers = new Parser[]{
   new Parser() { ParseFunc = BoxSize.Parse, ParserType = typeof(BoxSize) },
             new Parser() { ParseFunc = FootballTeam.Parse, ParserType = typeof(FootballTeam) },
              new Parser() { ParseFunc = Phraze.Parse, ParserType = typeof(Phraze) },
              new Parser() { ParseFunc = Person.Parse, ParserType = typeof(Person) },
              new Parser() { ParseFunc = Name.Parse, ParserType = typeof(Name) },
              new Parser() { ParseFunc = Animal.Parse, ParserType = typeof(Animal) },
              new Parser() { ParseFunc = Laptop.Parse, ParserType = typeof(Laptop) }
            };

            var res = StringParser.Parse(rawText, parsers);

            foreach (BaseParseObject item in res)
            {
                System.Console.WriteLine(item);
            }


            var types = new Type[]{
                typeof(Name),typeof(FootballTeam),typeof(Animal),typeof(Laptop),typeof(Person),typeof(Phraze),typeof(BoxSize)
            };

            System.Console.WriteLine("get list of attr name and type <Name,string >....");
            GetAttributes(types).ToList().ForEach(a => System.Console.WriteLine(a));


            System.Console.WriteLine("*************************************");

            System.Console.WriteLine("get list of type and count; string-1 ");
            GetCount(types).ToList().ForEach(a => System.Console.WriteLine(a));


        }



        public static IEnumerable<Tuple<string, Type>> GetAttributes(params Type[] listTypes)
        {

            return listTypes.SelectMany(a => a.GetProperties())
                    .Select(type => new Tuple<string, Type>(type.Name, type.PropertyType)).Distinct();
        }
        public static IEnumerable<Tuple<Type, int>> GetCount(params Type[] listTypes)
        {
            return listTypes.SelectMany(a => a.GetProperties())
                                .GroupBy(a => a.PropertyType)
                                        .Select(a => new Tuple<Type, int>(a.Key, a.Count()));
        }


    }

}
