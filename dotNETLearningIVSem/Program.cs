using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace dotNETLearningIVSem
{
    // Klasa porównująca elementy w sortowanej kolekcji
    class ZerosComparer : IComparer<string>
    {
        // ZerosComparer implementuje intefejs IComparer, więc musimy zaimplementować metodę tego interfejsu, którą jest właśnie Compare()
        public int Compare(string x, string y)
        {
            int x_zeros_count = 0;
            int y_zeros_count = 0;

            for (var i = 0; i < x.Length; i++)
            {
                if (x[i] == '0')
                {
                    x_zeros_count++;
                }
            }

            foreach (var character in y)
            {
                if (character == '0')
                {
                    y_zeros_count++;
                }
            }

            // zwraca 1 jeżeli x jest większy
            if (x_zeros_count > y_zeros_count)
            {
                return 1;
            }
            // -1 jeżeli x jest mniejszy
            if (x_zeros_count < y_zeros_count)
            {
                return -1;

            }
            // 0 jeżeli są równe
            return 0;
        }
    }

    // Metody rozszerzające muszą być w klasie statycznej
    static class StringExtension
    {
        // Metody rozszerzające wyróżniają się keywordem "this" użytym przed typem parametru, który rozszerzają
        public static string NumbersOnly(this string str)
        {
            for (var i = 0; i < str.Length; i++)
            {
                // jeśli znak nie jest cyfrą, to usuń go ze stringa
                if (!char.IsDigit(str[i]))
                {
                    str = str.Remove(i, 1);
                }
            }

            return str;
        }
    }

    public class TestNumber
    {
        // Poniżej dwie implementacje, które działają w identyczny sposób
        // ReSharper pozwala na "przerobienie" property w implementacje z "zewnętrznymi" get/set (to property with backing field)
        public int PropertyNumber { get; set; }

        private int _fieldNumber;

        public int GetFieldNumber()
        {
            return _fieldNumber;
        }

        public void SetFieldNumber(int number)
        {
            _fieldNumber = number;
        }

        // property pozwalają nam też ustawiać zmienne, które mają tylko gettery, tylko settery, albo różne kombinacje dostępu
        public int PrivateSetNumber { get; private set; }
        public int PrivateGetNumber { private get; set; }
        public int OnlyGetNumber { get; }

        // nie da się stworzyć property, które ma obie metody private - w takiej sytuacji wystarczy stworzyć prywatne pole
        // public int AllPrivateNumber { private set; private get; }
        private int _allPrivateNumber;

        // nie da się stworzyć property, które ma tylko set
        // public int OnlySetNumber { set; }

        // możemy za to stworzyć property, które nie ma przypisanej żadnej zmiennej
        public bool IsPropertyNumberAbove20
        {
            get { return PropertyNumber > 20; }
        }

        // możemy też dostosowywać ciało get/set i np. dodawać warunki
        public int PropertyNumberSetter
        {
            set
            {
                // tworząc logike w metodzie "set" danego property, pod keywordem "value" mamy dostęp do wartości, która będzie przekazywana do "set"
                if (value > 5)
                {
                    PropertyNumber = value;
                }
                else
                {
                    throw new ArgumentException("Musisz podac liczbe wieksza od 5!");
                }
            }
        }
    }


    class Program
    {
        public static void valTest(int test)
        {
            test += 6;
        }

        public static void refTest(ref int test)
        {
            test += 5;
        }

        static void Main(string[] args)
        {

            // ----------------------------------------------------------------------------------------------
            // Typy proste i keyword var

            // int, double, float, decimal, char, byte, string
            // var - wspaniałe cudo
            // var pozwala nam olać wpisywanie typu zmiennej w momencie pisania kodu
            // kompilator w trakcie kompilacji zamienia var na typ, który "ukrywa się" pod magicznym keywordem
            // Dlatego, mimo używania "var", musimy dbać o zgodność 
            var test = 12; // test jest zmienną typu int
            // byte

            // Przypisanie stringa do test nie zadziała, ponieważ test jest typu int
            // test = "Abc";

            // ref - & z C/C++, wysyłamy referencje(adres) zmiennej, a nie tylko jej wartość
            // obiekty ZAWSZE są wysyłane przez referencje, natomiast typy proste standardowo są wysyłane przez wartość
            Console.WriteLine($"Zmienna przed edycja: {test}");
            valTest(test);
            Console.WriteLine($"Zmienna po valTest: {test}");
            // jeżeli typ prosty wysyłamy przez referencje, podczas podawania parametru również musimy użyć keyword ref
            refTest(ref test);
            Console.WriteLine($"Zmienna po refTest: {test}");

            // ----------------------------------------------------------------------------------------------
            // Klasy i obiekty

            var testNumber = new TestNumber();
            testNumber.PropertyNumber = 15;
            testNumber.SetFieldNumber(20);
            Console.WriteLine($"Property: {testNumber.PropertyNumber}");
            Console.WriteLine($"Field: {testNumber.GetFieldNumber()}");
            Console.WriteLine($"Czy property jest > 20: {testNumber.IsPropertyNumberAbove20}");
            try
            {
                testNumber.PropertyNumberSetter = 4;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            // ----------------------------------------------------------------------------------------------
            // Konwersja typów

            string stringTest = "test";
            int intTest = 15;
            double doubleTest = 5.24;
            bool boolTest = false;

            // automatyczny
            // object type - co to
            object obj = stringTest; // implicit casting string -> obj
            Console.WriteLine(obj); // implicit casting obj -> string
            obj = intTest; // implicit casting int -> obj
            Console.WriteLine(obj); // implicit casting obj -> string
            obj = stringTest;

            // nie działa implicit casting typu object do typów prostych
            // intTest = obj;
            // doubleTest = obj;

            // to już działa, ale jest niebezpiecznie (kompilator pozwoli, ale może
            // być runtime error, jeżeli będziemy próbowali castować coś co nie może się udać)
            // poniższe spowoduje wyrzucenie exceptiona
            // intTest = (int)obj;

            // test castowania, w ten sposób możemy sprawdzić, czy jest możliwa konwersja zmiennej na dany typ
            bool castTest = obj is int;

            // próba castowania 
            // poniższe nie uda się, ponieważ castowanie "as x" wymaga, żeby typ x mógł być nullem(ten warunek spełnia właśnie string)
            //intTest = obj as int;

            // tutaj się uda, ponieważ string może być nullem, więc gdy castowanie się nie powiedzie, to do stringTest będzie przypisana wartość null
            obj = intTest;
            stringTest = obj as string;
            Console.WriteLine(stringTest);

            //// ----------------------------------------------------------------------------------------------
            // Tablice

            // Muszą mieć predefiniowany rozmiar
            int[] tab = new int[2];
            Console.WriteLine(tab.Length); // Można wpisywać Count, intellisense podmieni
            // Ale C# nie pilnuje za nas indeksowania array-ów, więc możemy spróbować je zepsuć
            try
            {
                tab[2] = 5;
            }
            catch (Exception e)
            {
                // Interpolacja stringów - wrzucanie danych do ciągu znaków
                Console.WriteLine($"Error: {e.Message}");
                // Poniżej jest to Console.WriteLine(string.Format("Error: {0}", e.Message)), tylko kompilator pozwala na pominięcie string.Format
                // od razu widzi interpolacje
                Console.WriteLine("Error: {0}", e.Message);
            }


            // ----------------------------------------------------------------------------------------------
            // String

            string testingString = "abcdefgh";
            Console.WriteLine(testingString);

            // string jest immutable, poniższe nie zadziała
            // testingString[0] = "b";

            // Musimy stworzyć nowego stringa, w którym zajdzie ta zmiana
            string newString = testingString.Replace("a", "z");
            Console.WriteLine(newString);

            // Jeżeli chcemy zmieniać string po indeksach
            newString = testingString.Remove(0, 3).Insert(0, "111");
            Console.WriteLine(newString);
            newString = testingString.Remove(0, 3);
            newString = newString.Insert(0, "111");
            Console.WriteLine(newString);

            // Możemy też w ten sposób zamienić dłuższy fragment na krótszy
            newString = testingString.Remove(0, 3).Insert(0, "11");
            Console.WriteLine(newString);

            // Stworzenie stringa z tablicy stringów
            string[] stringArr = { "a", "1", "b", "2", "c" };
            newString = string.Concat(stringArr);
            Console.WriteLine(newString);
            // po stringu możemy iterować jak po tablicy
            for (var i = 0; i < newString.Length; i++)
            {
                Console.WriteLine(newString[i]);
            }

            // ----------------------------------------------------------------------------------------------
            // Typy generyczne

            // List<T> inicjalizacja
            List<string> stringsList = new List<string>();

            // Listy mają dynamiczny rozmiar
            stringsList.Add("abc");
            stringsList.Add("def");
            stringsList.Add("ghi");
            stringsList.Add("abc");
            // Nie zadziała, C# pilnuje type-safety
            // stringsList.Add(123);

            Console.WriteLine(stringsList); // nie wyświetli listy, tylko nazwę i namespace klasy
            Console.WriteLine($"Rozmiar listy: {stringsList.Count}");

            // Sprawdzenie czy lista zawiera element
            bool hasAbc = stringsList.Contains("abc");
            Console.WriteLine($"Czy lista zawiera \"abc\": {hasAbc}");
            if (hasAbc)
            {
                // pierwsze / ostatnie wystąpienie
                Console.WriteLine($"Pierwsze wystąpienie pod indeksem: {stringsList.IndexOf("abc")}");
                Console.WriteLine($"Ostatnie wystąpienie pod indeksem: {stringsList.LastIndexOf("abc")}");
            }

            // Usuwa element z listy
            stringsList.Remove("abc");
            Console.WriteLine($"Rozmiar listy po usunieciu: {stringsList.Count}");

            // Iterowanie po liście za pomocą iteratora
            Console.WriteLine("Iteracja za pomoca iteratora");
            for (var i = stringsList.GetEnumerator(); i.MoveNext();)
            {
                Console.WriteLine(i.Current);
            }

            //// ----------------------------------------------------------------------------------------------
            // DateTime

            // Zabawa czasem
            Console.WriteLine($"Teraz jest: {DateTime.Now}");
            Console.WriteLine($"Za 24h będzie: {DateTime.Now.AddHours(24)}");
            Console.WriteLine($"Za dobę będzie: {DateTime.Now.AddDays(1)}");
            Console.WriteLine($"Mamy rok: {DateTime.Now.Year}");
            Console.WriteLine($"Dzisiaj jest {DateTime.Now.DayOfYear} dzień roku");

            // ----------------------------------------------------------------------------------------------
            // Metody rozszerzające


            newString = "1a2b3c4d";
            Console.WriteLine($"String: {newString}");
            // Metoda rozszerzająca sprawia, że nie musimy robić StringExtensions.NumbersOnly(newString), ale możemy newString.NumbersOnly()
            Console.WriteLine($"String z samymi cyframi: {newString.NumbersOnly()}");
            
            //// ----------------------------------------------------------------------------------------------
            // Kolekcja posortowana - przykład

            // SortedDictionary to lista elementów KeyValuePair<T, T>
            SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>(new ZerosComparer());
            newString = "1,000,000";
            sortedDictionary.Add(newString, newString.NumbersOnly());
            newString = "1,000,001";
            sortedDictionary.Add(newString, newString.NumbersOnly());
            newString = "1,110,000";
            sortedDictionary.Add(newString, newString.NumbersOnly());
            newString = "0,000,000";
            sortedDictionary.Add(newString, newString.NumbersOnly());
            foreach (var keyValuePair in sortedDictionary)
            {
                // keyValuePair jest typu KeyValuePair<string, string> i zawiera properties Key oraz Value
                Console.WriteLine($"{keyValuePair.Key} -> {keyValuePair.Value}");
            }

            // ----------------------------------------------------------------------------------------------
            // FileStream

            // Zapisywanie do pliku
            var fileStream = new FileStream("plik.txt", FileMode.Create);
            var dataToSave = "Abecadlo z pieca spadlo";
            // Trzeba konwertować na tablice bajtów
            byte[] bytes = Encoding.UTF8.GetBytes(dataToSave);
            fileStream.Write(bytes, 0, bytes.Length);
            // Na końcu należy zamknąć strumień
            fileStream.Close();

            using (var fileStreamUsing = new FileStream("plik_using.txt", FileMode.Create))
            {
                // Możemy tez skorzystać z keyworda using, który stworzy dla nas powyższy filestream, i zamknie go automatycznie w momencie
                // gdy opuścimy ten blok kodu
                fileStreamUsing.Write(bytes, 0, bytes.Length);

                // fileStreamUsing zamyka się automatycznie
            }

            // Odczytywanie z pliku
            using (var fileStreamUsing = new FileStream("plik_using.txt", FileMode.Open))
            {
                byte[] buffer = new byte[1024];
                int c;
                while ((c = fileStreamUsing.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Console.WriteLine($"{Encoding.UTF8.GetString(buffer, 0, c)}");
                }
            }

            // ----------------------------------------------------------------------------------------------
            // Serializacja / Deserializacja

            var textToSerialize = "Test teST 123";
            fileStream = new FileStream("serialized.txt", FileMode.Create);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                // Metoda Serialize serializuje do podanego strumienia obiekt, który podamy
                binaryFormatter.Serialize(fileStream, textToSerialize);
            }
            catch (SerializationException e)
            {
                Console.WriteLine($"Error: {e}");
            }
            fileStream.Close();

            string readText = null;
            fileStream = new FileStream("serialized.txt", FileMode.Open);
            try
            {
                // BinaryFormatter deserializuje do typu object, więc musimy dokonać konwersji na oczekiwany typ
                readText = (string)binaryFormatter.Deserialize(fileStream);
            }
            catch (SerializationException e)
            {
                Console.WriteLine($"Error: {e}");
            }
            fileStream.Close();
            Console.WriteLine($"Deserialized: {readText}");

            //// ----------------------------------------------------------------------------------------------
            // FileSystemInfo/DirectoryInfo/FileInfo

            Console.WriteLine();
            var dirInfo = new DirectoryInfo("testdir");
            Console.WriteLine($"Nazwa: {dirInfo.Name}");
            var dirInfoChildrenDirectories = dirInfo.GetDirectories();
            var dirInfoChildrenFiles = dirInfo.GetFiles();
            // Liczba plików znajdujących się w danym katalogu
            var numberOfDirInfoChildren = dirInfoChildrenDirectories.Length + dirInfoChildrenFiles.Length;
            Console.WriteLine($"Dzieci:");
            foreach (var dirInfoChildrenDirectory in dirInfoChildrenDirectories)
            {
                Console.WriteLine($"{dirInfoChildrenDirectory.Name} - Directory");
            }
            foreach (var dirInfoChildrenFile in dirInfoChildrenFiles)
            {
                Console.WriteLine($"{dirInfoChildrenFile.Name} - File {dirInfoChildrenFile.Length}");
            }

            var testFile = new FileInfo("testfile");
            var fileSystemInfo = (FileSystemInfo)testFile;
            // FileSystemInfo attributes
            Console.WriteLine(fileSystemInfo.Attributes);
            // pokazać jak wygląda wyciąganie jednego z atrybutów RAHS
            // Wpisanie fileSystemInfo.Attributes.ReadOnly i kliknięcie spacji/tab/enter automatycznie zamieni na poniższy warunek
            // (nie mam pewności, czy poza Visual Studio też tak działa)
            // UWAGA, na Linuxie też można odczytywać atrybuty RAHS. Po prostu A i S będą zawsze równe 0, pozostałe 2 mogą być ustawione
            Console.WriteLine((fileSystemInfo.Attributes & FileAttributes.ReadOnly) != 0);

            //// ----------------------------------------------------------------------------------------------
        }
    }
}
