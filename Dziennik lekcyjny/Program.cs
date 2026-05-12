using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace DziennikLekcyjny
{
    internal class Program
    {
        static string folderDanych = Path.Combine(
     Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName,
     "Dane");

        static string plikStudenci = Path.Combine(folderDanych, "studenci.csv");
        static string plikPrzedmioty = Path.Combine(folderDanych, "przedmioty.csv");
        static string plikOceny = Path.Combine(folderDanych, "oceny.csv");

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            DaneSzkoly dane = new DaneSzkoly();
            
            WczytajDaneZPlikow(dane);
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
            ZapiszDaneDoPlikow(dane);

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
            ZapiszDaneDoPlikow(dane);

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

            // SPRAWDZENIE: Czy student istnieje?
            if (PobierzStudentaPoId(dane, idStudenta) == null)
            {
                Console.WriteLine("Błąd! Uczeń o podanym ID nie istnieje. Anulowano dodawanie oceny.");
                return; // Przerywamy działanie metody i wracamy do menu
            }

            WyswietlPrzedmioty(dane);
            int idPrzedmiotu = WczytajLiczbeCalkowita("Podaj ID przedmiotu: ");

            // SPRAWDZENIE: Czy przedmiot istnieje?
            if (PobierzPrzedmiotPoId(dane, idPrzedmiotu) == null)
            {
                Console.WriteLine("Błąd! Przedmiot o podanym ID nie istnieje. Anulowano dodawanie oceny.");
                return;
            }

            double wartoscOceny = WczytajOcene("Podaj ocenę: ");

            Ocena ocena = new Ocena(idStudenta, idPrzedmiotu, wartoscOceny);
            dane.Oceny.Add(ocena);
            ZapiszDaneDoPlikow(dane);

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
            int wynik;
            Console.Write(komunikat);

            // TryParse próbuje zamienić tekst na liczbę. Jeśli się nie uda, zwraca false i pętla trwa.
            while (!int.TryParse(Console.ReadLine(), out wynik))
            {
                Console.WriteLine("Błąd! Musisz podać poprawną liczbę całkowitą.");
                Console.Write(komunikat);
            }

            return wynik;
        }

        static string WczytajNapis(string komunikat)
        {
            Console.Write(komunikat);
            string wpis = Console.ReadLine();

            // Dopóki wpisany tekst jest pusty lub składa się z samych spacji, pytamy ponownie
            while (string.IsNullOrWhiteSpace(wpis))
            {
                Console.WriteLine("Błąd! Wartość nie może być pusta.");
                Console.Write(komunikat);
                wpis = Console.ReadLine();
            }

            return wpis;
        }

        static double WczytajOcene(string komunikat)
        {
            double wynik;
            Console.Write(komunikat);
            string wpis = Console.ReadLine().Replace(',', '.');

            // Sprawdzamy dwie rzeczy naraz: czy to w ogóle jest liczba (TryParse) 
            // oraz czy mieści się w przedziale od 1 do 6.
            while (!double.TryParse(wpis, NumberStyles.Any, CultureInfo.InvariantCulture, out wynik) || wynik < 1 || wynik > 6)
            {
                Console.WriteLine("Błąd! Podaj poprawną ocenę z przedziału od 1 do 6 (np. 4.5).");
                Console.Write(komunikat);
                wpis = Console.ReadLine().Replace(',', '.');
            }

            return wynik;
        }


        // dane poczatkowe


        static void ZapiszDaneDoPlikow(DaneSzkoly dane)
        {
            Directory.CreateDirectory(folderDanych);

            File.WriteAllLines(plikStudenci,
                dane.Studenci.Select(s => $"{s.Id};{s.Imie};{s.Nazwisko}"));

            File.WriteAllLines(plikPrzedmioty,
                dane.Przedmioty.Select(p => $"{p.Id};{p.Nazwa}"));

            File.WriteAllLines(plikOceny,
                dane.Oceny.Select(o => $"{o.IdStudenta};{o.IdPrzedmiotu};{o.Wartosc.ToString(CultureInfo.InvariantCulture)}"));
        }

        static void WczytajDaneZPlikow(DaneSzkoly dane)
        {
            Directory.CreateDirectory(folderDanych);

            if (File.Exists(plikStudenci))
            {
                foreach (string linia in File.ReadAllLines(plikStudenci))
                {
                    if (string.IsNullOrWhiteSpace(linia)) continue;

                    string[] czesci = linia.Split(';');

                    if (czesci.Length == 3)
                    {
                        dane.Studenci.Add(new Student(
                            int.Parse(czesci[0]),
                            czesci[1],
                            czesci[2]));
                    }
                }
            }

            if (File.Exists(plikPrzedmioty))
            {
                foreach (string linia in File.ReadAllLines(plikPrzedmioty))
                {
                    if (string.IsNullOrWhiteSpace(linia)) continue;

                    string[] czesci = linia.Split(';');

                    if (czesci.Length == 2)
                    {
                        dane.Przedmioty.Add(new Przedmiot(
                            int.Parse(czesci[0]),
                            czesci[1]));
                    }
                }
            }

            if (File.Exists(plikOceny))
            {
                foreach (string linia in File.ReadAllLines(plikOceny))
                {
                    if (string.IsNullOrWhiteSpace(linia)) continue;

                    string[] czesci = linia.Split(';');

                    if (czesci.Length == 3)
                    {
                        dane.Oceny.Add(new Ocena(
                            int.Parse(czesci[0]),
                            int.Parse(czesci[1]),
                            double.Parse(czesci[2], CultureInfo.InvariantCulture)));
                    }
                }
            }
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