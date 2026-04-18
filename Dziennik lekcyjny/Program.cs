using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace DziennikLekcyjny
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            DaneSzkoly dane = new DaneSzkoly();
            DodajDaneStartowe(dane);
            Menu(dane);
        }

        static void Menu(DaneSzkoly dane)
        {
            bool wyjscie = false;

            while (!wyjscie)
            {
                
                Console.WriteLine("==================================");
                Console.WriteLine("     DZIENNIK LEKCYJNY - MENU    ");
                Console.WriteLine("==================================");
                Console.WriteLine("1. Dodaj studenta");
                Console.WriteLine("2. Dodaj przedmiot");
                Console.WriteLine("3. Wyświetl studentów");
                Console.WriteLine("4. Wyświetl przedmioty");
                Console.WriteLine("5. Dodaj ocenę");
                Console.WriteLine("6. Pokaż oceny studenta");
                Console.WriteLine("0. Wyjście");
                Console.WriteLine("==================================");

                int wybor = WczytajLiczbeCalkowita("Wybierz opcję: ");
                Console.WriteLine();

                switch (wybor)
                {
                    case 1:
                        DodajStudenta(dane);
                        break;
                    case 2:
                        DodajPrzedmiot(dane);
                        break;
                    case 3:
                        WyswietlStudentow(dane);
                        break;
                    case 4:
                        WyswietlPrzedmioty(dane);
                        break;
                    case 5:
                        DodajOcene(dane);
                        break;
                    case 6:
                        PokazOcenyStudenta(dane);
                        break;
                    case 0:
                        wyjscie = true;
                        Console.WriteLine("Zamykanie programu...");
                        break;
                    default:
                        Console.WriteLine("Nieprawidłowa opcja.");
                        break;
                }

                if (!wyjscie)
                {
                    Console.WriteLine("\nNaciśnij dowolny klawisz, aby wrócić do menu...");
                    Console.ReadKey();
                }
            }
        }

        // =========================
        // STUDENCI
        // =========================

        static void DodajStudenta(DaneSzkoly dane)
        {
            Console.WriteLine("=== DODAWANIE STUDENTA ===");

            string imie = WczytajNapis("Podaj imię: ");
            string nazwisko = WczytajNapis("Podaj nazwisko: ");

            // Jeśli lista jest pusta, nowe ID to 1.
            // W przeciwnym razie bierzemy największe ID i dodajemy 1.
            int noweId = dane.Studenci.Count == 0 ? 1 : dane.Studenci.Max(s => s.Id) + 1;

            Student student = new Student(noweId, imie, nazwisko);
            dane.Studenci.Add(student);

            Console.WriteLine("Dodano studenta:");
            Console.WriteLine(student);
        }

        static void WyswietlStudentow(DaneSzkoly dane)
        {
            Console.WriteLine("=== LISTA STUDENTÓW ===");

            foreach (Student student in dane.Studenci)
            {
                Console.WriteLine(student);
            }
        }

        static Student PobierzStudentaPoId(DaneSzkoly dane, int id)
        {
            // FirstOrDefault szuka pierwszego pasującego elementu.
            // Jeśli nic nie znajdzie, zwraca null.
            return dane.Studenci.FirstOrDefault(s => s.Id == id);
        }

        // =========================
        // PRZEDMIOTY
        // =========================

        static void DodajPrzedmiot(DaneSzkoly dane)
        {
            Console.WriteLine("=== DODAWANIE PRZEDMIOTU ===");

            string nazwa = WczytajNapis("Podaj nazwę przedmiotu: ");

            // Nadanie kolejnego ID dla nowego przedmiotu
            int noweId = dane.Przedmioty.Count == 0 ? 1 : dane.Przedmioty.Max(p => p.Id) + 1;

            Przedmiot przedmiot = new Przedmiot(noweId, nazwa);
            dane.Przedmioty.Add(przedmiot);

            Console.WriteLine("Dodano przedmiot:");
            Console.WriteLine(przedmiot);
        }

        static void WyswietlPrzedmioty(DaneSzkoly dane)
        {
            Console.WriteLine("=== LISTA PRZEDMIOTÓW ===");

            foreach (Przedmiot przedmiot in dane.Przedmioty)
            {
                Console.WriteLine(przedmiot);
            }
        }

        static Przedmiot PobierzPrzedmiotPoId(DaneSzkoly dane, int id)
        {
            // Szukanie przedmiotu po jego ID
            return dane.Przedmioty.FirstOrDefault(p => p.Id == id);
        }

        // 
        // Oceny
        // 

        static void DodajOcene(DaneSzkoly dane)
        {
            Console.WriteLine("=== DODAWANIE OCENY ===");

            if (dane.Studenci.Count == 0 || dane.Przedmioty.Count == 0)
            {
                Console.WriteLine("Najpierw dodaj studentów i przedmioty.");
                return;
            }

            WyswietlStudentow(dane);
            int idStudenta = WczytajLiczbeCalkowita("Podaj ID studenta: ");

            WyswietlPrzedmioty(dane);
            int idPrzedmiotu = WczytajLiczbeCalkowita("Podaj ID przedmiotu: ");

            double wartoscOceny = WczytajOcene("Podaj ocenę: ");

            Ocena ocena = new Ocena(idStudenta, idPrzedmiotu, wartoscOceny);
            dane.Oceny.Add(ocena);

            Console.WriteLine("Ocena została dodana.");
        }

        static void PokazOcenyStudenta(DaneSzkoly dane)
        {
            Console.WriteLine("=== OCENY STUDENTA ===");

            WyswietlStudentow(dane);
            int idStudenta = WczytajLiczbeCalkowita("Podaj ID studenta: ");

            Student student = PobierzStudentaPoId(dane, idStudenta);

            // Where wybiera wszystkie oceny danego studenta,
            // a ToList zamienia wynik na listę.
            List<Ocena> ocenyStudenta = dane.Oceny.Where(o => o.IdStudenta == idStudenta).ToList();

            if (student == null || ocenyStudenta.Count == 0)
            {
                Console.WriteLine("Brak danych do wyświetlenia.");
                return;
            }

            Console.WriteLine($"\nOceny studenta: {student.Imie} {student.Nazwisko}");

            foreach (Ocena ocena in ocenyStudenta)
            {
                Przedmiot przedmiot = PobierzPrzedmiotPoId(dane, ocena.IdPrzedmiotu);

                if (przedmiot != null)
                {
                    Console.WriteLine($"{przedmiot.Nazwa} - {ocena.Wartosc}");
                }
            }

            // Average oblicza średnią z wszystkich ocen na liście
            double srednia = ocenyStudenta.Average(o => o.Wartosc);
            Console.WriteLine($"Średnia ocen: {srednia:F2}");
        }

        // =========================
        // METODY POMOCNICZE
        // =========================

        static int WczytajLiczbeCalkowita(string komunikat)
        {
            Console.Write(komunikat);
            return int.Parse(Console.ReadLine());
        }

        static string WczytajNapis(string komunikat)
        {
            Console.Write(komunikat);
            return Console.ReadLine();
        }

        static double WczytajOcene(string komunikat)
        {
            Console.Write(komunikat);

            
            string wpis = Console.ReadLine().Replace(',', '.');

            // InvariantCulture pozwala poprawnie odczytać liczbę z kropką
            return double.Parse(wpis, CultureInfo.InvariantCulture);
        }

        
        // dane poczatkowe
      

        static void DodajDaneStartowe(DaneSzkoly dane)
        {
            dane.Studenci.Add(new Student(1, "Jan", "Kowalski"));
            dane.Studenci.Add(new Student(2, "Anna", "Nowak"));
            dane.Studenci.Add(new Student(3, "Piotr", "Wiśniewski"));

            dane.Przedmioty.Add(new Przedmiot(1, "Matematyka"));
            dane.Przedmioty.Add(new Przedmiot(2, "Programowanie"));
            dane.Przedmioty.Add(new Przedmiot(3, "Bazy Danych"));

            dane.Oceny.Add(new Ocena(1, 1, 4.0));
            dane.Oceny.Add(new Ocena(1, 2, 5.0));
            dane.Oceny.Add(new Ocena(2, 1, 3.5));
            dane.Oceny.Add(new Ocena(2, 3, 4.5));
            dane.Oceny.Add(new Ocena(3, 2, 3.0));
        }
    }

    internal class Student
    {
        public int Id { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }

        public Student(int id, string imie, string nazwisko)
        {
            Id = id;
            Imie = imie;
            Nazwisko = nazwisko;
        }

        public override string ToString()
        {
            return $"{Id} - {Imie} {Nazwisko}";
        }
    }

    internal class Przedmiot
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }

        public Przedmiot(int id, string nazwa)
        {
            Id = id;
            Nazwa = nazwa;
        }

        public override string ToString()
        {
            return $"{Id} - {Nazwa}";
        }
    }

    internal class Ocena
    {
        public int IdStudenta { get; set; }
        public int IdPrzedmiotu { get; set; }
        public double Wartosc { get; set; }

        public Ocena(int idStudenta, int idPrzedmiotu, double wartosc)
        {
            IdStudenta = idStudenta;
            IdPrzedmiotu = idPrzedmiotu;
            Wartosc = wartosc;
        }
    }

    internal class DaneSzkoly
    {
        public List<Student> Studenci { get; set; } = new List<Student>();
        public List<Przedmiot> Przedmioty { get; set; } = new List<Przedmiot>();
        public List<Ocena> Oceny { get; set; } = new List<Ocena>();
    }
}
