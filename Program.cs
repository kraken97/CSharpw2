﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;


namespace ConsoleApplication
{


    class Example
    {


        static void Main()
        {
            var res=Parser.Parse("Person{gender=m;firstName=kek;lastName=llek;birthDate=1997/11/11;} аыв а   аыв аы ыавы а ываы Person{gender=m;firstName=kek;lastName=llek;birthDate=1997/11/11;}"
            ,PersonParser); 
            System.Console.WriteLine("**********");
            foreach (var item in res)
            {
                System.Console.WriteLine(item);
            }
            res.RemoveDuplicates();
            foreach (var item in res)
            {
                System.Console.WriteLine(item);
            }


        }

        static Person parsePerson(string[] s)
        {
            string firstName = "";
            string lastName = "";
            bool gender = false;
            DateTime birthDate = new DateTime(1996, 1, 1);
            for (int i = 0; i < 4; i++)
            {
                System.Console.WriteLine(s[i]);
                string[] propAndVal = s[i].Split('=');
                string prop = propAndVal[0];
                string val = propAndVal[1];
                System.Console.WriteLine(prop + " " + val + " " + i);
                switch (i)
                {
                    case 0:
                        if (prop == "gender")
                        {
                            if (val == "m")
                                gender = true;
                            else if (val == "f")
                                gender = false;
                            else
                                throw new FieldAccessException("wrong value of gender field");
                        }
                        else
                            throw new FieldAccessException("error while parsing file prop gender does not exist");
                        break;
                    case 1:

                        if (prop == "firstName")
                            firstName = val;
                        else
                            throw new FieldAccessException("error while parsing file prop firstName does not exist");
                        break;
                    case 2:
                        System.Console.WriteLine($"{prop}-{val}");
                        if (prop == "lastName")
                        {
                            lastName = val;
                        }
                        else
                        {
                            throw new FieldAccessException("error while parsing file prop lastName does not exist");

                        }
                        break;
                    case 3:
                        if (prop == "birthDate")
                        {
                            string format = "yyyy/MM/dd";
                            CultureInfo provider = CultureInfo.InvariantCulture;
                            birthDate = DateTime.ParseExact(val, format, provider);
                        }
                        else
                        {
                            throw new FieldAccessException("error while parsing file prop date does not exist");

                        }
                        break;
                }
            }
            return new Person(firstName, lastName, gender, birthDate);

        }
        static IList<IComparable> PersonParser(string str)
        {
            string text = str;
            string pat = @"Person{([^\{]{0,}[a-zA-z1234567890_]{1,}=[a-zA-z1234567890/_]{1,};){0,1}}";

            // Instantiate the regular expression object.
            Regex r = new Regex(pat, RegexOptions.Multiline);

            // Match the regular expression pattern against a text string.
            Match m = r.Match(text);
            List<IComparable> resultList = new List<IComparable>();

            while (m.Success)
            {
                Group g = m.Groups[1];
                Console.WriteLine(g);
                if (g.Value != null)
                {
                    string[] s = g.Value.ToString().Split(';');
                    resultList.Add(parsePerson(s));
                }

                m = m.NextMatch();
                
            }
           
            return resultList;
        }
    }





    //Person{([^\{]{0,}[a-zA-z1234567890_]{1,}=[a-zA-z1234567890_]{1,};){0,1}}



    public static class  Parser
    {

         public  static void RemoveDuplicates(this IList<IComparable> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                //element which dublicates we are looking for
                var currentElem = list[i];
                //j  = i+1  will not include  currentElem;
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (currentElem.CompareTo(list[j]) == 0)
                    {
                         list.RemoveAt(j);
                        //back to the prev index if we dont do this we skip 1 element   
                        j--;
                    }
                }
            }
        }
        public static IList<IComparable> Parse(string str, params Func<string, IList<IComparable>>[] delegatesList)
        {
            List<IComparable> list = new List<IComparable>();
            for (int i = 0; i < delegatesList.Length; i++)
            {

                try
                {
                    var res = delegatesList[i](str);
                    if (res != null)
                        for (int j = 0; j < res.Count; i++)
                        {
                            list.Add(res[i]);
                        }
                }
                catch (System.Exception)
                {
                    System.Console.Error.WriteLine("error happened while parsing file , the result may be unpredictable");
                }
            }
            return list;
        }

    }




    class Name : IComparable<Name>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Name(string FirstName, String LastName)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
        }
        public static Name parseName(string val)
        {
            var fandln = val.Split(' ');
            return new Name(fandln[0] ?? " ", fandln[1] ?? " ");
        }

        public int CompareTo(Name other)
        {
            var res = this.FirstName.CompareTo(other.FirstName);
            return res == 0 ? this.LastName.CompareTo(other.LastName) : res;
        }
    }
    internal class Person : IComparable
    {
        public bool Gender { get; set; }
        public Name Name { get; set; }
        public DateTime BirhtDate { get; set; }
        public Person(Name name, bool gender, DateTime birthDate)
        {
            this.Name = name;
            this.Gender = gender;
            this.BirhtDate = birthDate;
        }
        public Person(string fullName, bool gender, DateTime BirhtDate)
        : this(Name.parseName(fullName), gender, BirhtDate)
        { }

        public Person(string firstName, string lastName, bool gender, DateTime birthDate)
        : this(new Name(firstName, lastName), gender, birthDate)
        { }


        public override string ToString()
        {
            var now = DateTime.Now;
            var age = now.Subtract(this.BirhtDate);
            var gender = this.Gender ? "m" : "f";
            return $"{this.Name.FirstName}, {this.Name.LastName} ({gender},{age.Days / 365}) ";

        }



        public int CompareTo(Object other)
        {
            if (!(other is Person))
            {
                return -1;
            }
            Person aPerson = (other as Person);

            if (other == null)
            {
                return -1;
            }
            var res = this.Gender.CompareTo(aPerson.Gender);
            if (res == 0)
            {
                var res2 = this.Name.CompareTo(aPerson.Name);
                if (res2 == 0)
                {
                    return this.BirhtDate.CompareTo(aPerson.BirhtDate);
                }
                else
                {
                    return res2;
                }
            }
            return res;
        }
    }
    internal class UsCitizen : Person
    {
        public int SSN { get; set; }
        public UsCitizen(string name, bool gender, DateTime birthDate, int ssn) : base(name, gender, birthDate)
        {
            this.SSN = ssn;
        }

        public override string ToString()
        {

            return base.ToString() + " " + this.SSN;

        }


    }

    interface IPersonsList
    {
        void Sort();
        void Add(Person p);
        void Remove(int i);


    }

    internal class PersonsList : IPersonsList
    {
        private Person[] ListOfPersons;
        public int Count { get; private set; } = 0;
        public int Size { get; private set; }
        public PersonsList(int n = 10)
        {
            this.ListOfPersons = new Person[n];
            this.Size = n;

        }


       

        public void Sort()
        {
            Utils.QuickSort<Person>(this.ListOfPersons, this.Count - 1, (a, b) => b.CompareTo(a));
        }

        public override string ToString()
        {
            string res = "[";
            var lenght = this.Count;
            for (int i = 0; i < lenght; i++)
            {
                res += "{ " + this.ListOfPersons[i] + (i == lenght - 1 ? "}]" : " }," + Environment.NewLine);
            }
            return res;
        }

        public void Add(Person p)
        {
            if (Count < Size)
            {
                this.ListOfPersons[Count++] = p;
            }
            else
            {
                Array.Resize(ref this.ListOfPersons, this.ListOfPersons.Length * 2);
                this.ListOfPersons[Count++] = p;
            }

        }

        public void Remove(int index)
        {
            if (index > this.Count)
            {
                throw new InvalidOperationException("index must me lower than list size");
            }
            //k-index of element in old array i- index of element in new array
            Person[] newArray = new Person[this.Count];
            for (int k = 0, i = 0; i < this.Count; k++, i++)
            {
                if (k == index)
                {
                    k++;
                    Count--;
                }
                //check if is it element to remove  if no add element to new array
                newArray[i] = this.ListOfPersons[k];

            }
            this.ListOfPersons = newArray;

        }
    }

    static class Utils
    {
        public static void swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }


        //plain quick sort with generics
        public static T[] QuickSort<T>(T[] array, int end, Func<T, T, int> comparator)
        {
            System.Console.WriteLine("start sort");
            sort<T>(0, end, array, comparator);
            return array;
        }

        private static void sort<T>(int begin, int end, T[] array, Func<T, T, int> compare)
        {
            int i = begin;
            int j = end;
            int index = (i + j) / 2;
            T k = array[index];

            while (i <= j)
            {
                while (compare(array[i], k) == 1) i++;
                while (compare(array[j], k) == -1) j--;

                if (i <= j)
                {
                    swap<T>(ref array[i], ref array[j]);
                    i++;
                    j--;
                }


            }

            if (begin < j)
                sort<T>(begin, j, array, compare);
            if (i < end)
                sort<T>(i, end, array, compare);
        }


    }

}
