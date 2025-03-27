internal class Program
{
    public static void Main()
    {
        Statek statek1 = new Statek("statek1", 70, 5, 33);
        Statek statek2 = new Statek("statek2", 35, 8, 23);
        Menu(statek1, statek2);
    }

    public static void Menu(Statek statek1, Statek statek2)
    {
        Dictionary<string, double> produktyChlodnicze = new Dictionary<string, double>()
        {
            {"Bananas", 13.3},
            {"Chocolate", 18},
            {"Fish", -2},
            {"Meat", -15},
            {"Ice cream", -18},
            {"Frozen pizza", -30},
            {"Cheese", 7.2},
            {"Sausages", 5},
            {"Butter", 10.9},
            {"Eggs", 19}
        };

        bool dziala = true;
        while (dziala)
        {
            Console.WriteLine("\n1. Załaduj kontener\n2. Rozładuj kontener\n3. Zastąp kontener\n4. Przenieś kontener\n5. Pokaż statki\n6. Zakończ");
            string wybor = Console.ReadLine();

            switch (wybor)
            {
                case "1":
                    Console.WriteLine("Wybierz typ kontenera: C - chłodniczy, L - na płyny, G - na gaz");
                    string typ = Console.ReadLine().ToUpper();

                    try
                    {
                        switch (typ)
                        {
                            case "C":
                                Console.WriteLine("Dostępne produkty chłodnicze:");
                                foreach (var p in produktyChlodnicze)
                                    Console.WriteLine($"- {p.Key} (min. temp: {p.Value} stopni Celsjusza)");

                                Console.Write("Wybierz produkt: ");
                                string wybrany = Console.ReadLine();

                                if (!produktyChlodnicze.ContainsKey(wybrany))
                                {
                                    Console.WriteLine("Nieznany produkt.");
                                    break;
                                }

                                double wymaganaTemp = produktyChlodnicze[wybrany];
                                Console.Write($"Podaj temperaturę kontenera dla {wybrany} (min: {wymaganaTemp}stopni Celsjusza): ");
                                double temp = double.Parse(Console.ReadLine());

                                Produkt produkt = new Produkt(wybrany, wymaganaTemp);
                                KontenerChlodniczy chlodnia = new KontenerChlodniczy(220, 4500, 760, 45535, produkt, temp);
                                statek1.ZaladujKontener(chlodnia);
                                Console.WriteLine("Załadowano kontener chłodniczy.");
                                break;

                            case "L":
                                Console.Write("Czy niebezpieczny (true/false): ");
                                bool niebezpieczny = bool.Parse(Console.ReadLine());
                                KontenerNaPlyny plynny = new KontenerNaPlyny(470, 2643, 530, 2060, niebezpieczny);
                                statek1.ZaladujKontener(plynny);
                                Console.WriteLine("Załadowano kontener na płyny.");
                                break;

                            case "G":
                                Console.Write("Ciśnienie: ");
                                double cisnienie = double.Parse(Console.ReadLine());
                                KontenerNaGaz gazowy = new KontenerNaGaz(270, 2464, 746, 8460, cisnienie);
                                statek1.ZaladujKontener(gazowy);
                                Console.WriteLine("Załadowano kontener na gaz.");
                                break;

                            default:
                                Console.WriteLine("Nieznany typ.");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd: {ex.Message}");
                    }

                    break;

                case "2":
                    Console.Write("Numer seryjny: ");
                    string nr = Console.ReadLine();
                    var kontener = statek1.kontenery.FirstOrDefault(k => k.NumerSeryjny == nr);
                    if (kontener != null) statek1.RozladujKontener(kontener);
                    break;
                case "3":
                    Console.Write("Stary numer: ");
                    string stary = Console.ReadLine();
                    KontenerNaPlyny nowy = new KontenerNaPlyny(220, 8685, 757, 36855
                        , false);
                    statek1.ZastapKontener(stary, nowy);
                    break;
                case "4":
                    Console.Write("Numer kontenera: ");
                    string numer = Console.ReadLine();
                    var kon = statek1.kontenery.FirstOrDefault(k => k.NumerSeryjny == numer);
                    if (kon != null) statek1.PrzeniesKontenerDo(kon, statek2);
                    break;
                case "5":
                    Console.WriteLine("Statek 1:");
                    statek1.WyswietlInformacje();
                    Console.WriteLine("Statek 2:");
                    statek2.WyswietlInformacje();
                    break;
                case "6":
                    dziala = false;
                    break;
            }
        }
    }

    public class PrzekroczenieLadownosciException : Exception
    {
        public PrzekroczenieLadownosciException(string komunikat) : base(komunikat) { }
    }

    public class Produkt
    {
        public string Nazwa { get; set; }
        public double MinimalnaTemperatura { get; set; }
        public Produkt(string nazwa, double minimalnaTemperatura)
        {
            Nazwa = nazwa;
            MinimalnaTemperatura = minimalnaTemperatura;
        }
    }

    public class Kontener
    {
        private static int licznikNumerow = 1;
        private static HashSet<string> istniejąceNumerySeryjne = new HashSet<string>();
        public double MasaLadunku { get; set; }
        public double Wysokosc { get; set; }
        public double WagaWlasna { get; set; }
        public double Glebokosc { get; set; }
        public string NumerSeryjny { get; set; }
        public double MaksymalnaLadownosc { get; set; }

        public Kontener(double wysokosc, double wagaWlasna, double glebokosc, double maksymalnaLadownosc, string typKontenera)
        {
            Wysokosc = wysokosc;
            WagaWlasna = wagaWlasna;
            Glebokosc = glebokosc;
            MaksymalnaLadownosc = maksymalnaLadownosc;
            MasaLadunku = 0;

            string numer;
            do
            {
                numer = $"KON-{typKontenera}-{licznikNumerow++}";
            } while (istniejąceNumerySeryjne.Contains(numer));

            istniejąceNumerySeryjne.Add(numer);
            NumerSeryjny = numer;
        }

        public void ZaladujLadunek(double masa)
        {
            if (masa + MasaLadunku > MaksymalnaLadownosc)
                throw new PrzekroczenieLadownosciException($"Przeciążenie! Max ładowność: {MaksymalnaLadownosc} kg");
            MasaLadunku += masa;
        }

        public void OproznijLadunek()
        {
            MasaLadunku = 0;
        }

        public virtual string PobierzTypKontenera() => "Zwykły";

        public override string ToString()
        {
            return $"{NumerSeryjny} | Ładunek: {MasaLadunku}kg / {MaksymalnaLadownosc}kg";
        }
    }

    public class KontenerChlodniczy : Kontener
    {
        public Produkt Produkt { get; set; }
        public double TemperaturaKontenera { get; set; }

        public KontenerChlodniczy(double wysokosc, double wagaWlasna, double glebokosc, double maksLadownosc, Produkt produkt, double temperaturaKontenera)
            : base(wysokosc, wagaWlasna, glebokosc, maksLadownosc, "C")
        {
            Produkt = produkt;
            TemperaturaKontenera = temperaturaKontenera;

            if (TemperaturaKontenera < Produkt.MinimalnaTemperatura)
                throw new Exception($"Za niska temperatura dla {Produkt.Nazwa}");
        }

        public override string PobierzTypKontenera()
        {
            return "chlodniczy";
        }
    }

    public class KontenerNaPlyny : Kontener
    {
        public bool CzyNiebezpieczny { get; set; }

        public KontenerNaPlyny(double wysokosc, double wagaWlasna, double glebokosc, double maksLadownosc, bool czyNiebezpieczny)
            : base(wysokosc, wagaWlasna, glebokosc, maksLadownosc, "L")
        {
            CzyNiebezpieczny = czyNiebezpieczny;
        }

        public override string PobierzTypKontenera()
        {
            return "na plyny";
        }
    }

    public class KontenerNaGaz : Kontener
    {
        public double Cisnienie { get; set; }

        public KontenerNaGaz(double wysokosc, double wagaWlasna, double glebokosc, double maksLadownosc, double cisnienie)
            : base(wysokosc, wagaWlasna, glebokosc, maksLadownosc, "G")
        {
            Cisnienie = cisnienie;
        }

        public override string PobierzTypKontenera()
        {
            return "na gaz";
        }
    }

    public interface IHazardNotifier
    {
        void PowiadomONiebezpieczenstwie(string numerKontenera);
    }

    public class Statek : IHazardNotifier
    {
        public string Nazwa { get; set; }
        public double MaksymalnaPredkosc { get; set; }
        public int MaksymalnaLiczbaKontenerow { get; set; }
        public double MaksymalnaLadownosc { get; set; }
        public List<Kontener> kontenery;

        public Statek(string nazwa, double maksPredkosc, int maksKontenerow, double maksLadownosc)
        {
            Nazwa = nazwa;
            MaksymalnaPredkosc = maksPredkosc;
            MaksymalnaLiczbaKontenerow = maksKontenerow;
            MaksymalnaLadownosc = maksLadownosc;
            kontenery = new List<Kontener>();
        }

        public void ZaladujKontener(Kontener kontener)
        {
            if (kontenery.Count >= MaksymalnaLiczbaKontenerow)
                throw new Exception("za duzo kontenerow");

            double lacznaMasa = kontenery.Sum(k => k.MasaLadunku + k.WagaWlasna) + kontener.MasaLadunku + kontener.WagaWlasna;
            if (lacznaMasa > MaksymalnaLadownosc * 1000)
                throw new Exception("Przekroczona ladownosc");

            if (kontener is KontenerNaGaz)
            {
                int liczbaNiebezpiecznych = kontenery.OfType<KontenerNaGaz>().Count();
                double limit = MaksymalnaLiczbaKontenerow * 0.05;
                if (liczbaNiebezpiecznych >= limit)
                {
                    throw new Exception("nie mozna tak robic");
                }
            }

            kontenery.Add(kontener);

            if (kontener is KontenerNaPlyny plyny && plyny.CzyNiebezpieczny)
                PowiadomONiebezpieczenstwie(kontener.NumerSeryjny);

            if (kontener is KontenerNaGaz)
                PowiadomONiebezpieczenstwie(kontener.NumerSeryjny);
        }

        public void RozladujKontener(Kontener kontener) => kontenery.Remove(kontener);

        public void UsunKontener(string numer) => kontenery.RemoveAll(k => k.NumerSeryjny == numer);

        public void ZastapKontener(string staryNumer, Kontener nowy)
        {
            int index = kontenery.FindIndex(k => k.NumerSeryjny == staryNumer);
            if (index == -1) throw new Exception("Kontener nie znaleziony");
            kontenery[index] = nowy;
        }

        public void PrzeniesKontenerDo(Kontener kontener, Statek inny)
        {
            RozladujKontener(kontener);
            inny.ZaladujKontener(kontener);
        }

        public void WyswietlInformacje()
        {
            Console.WriteLine($"Statek: {Nazwa}, Max predkosc: {MaksymalnaPredkosc} km");
            foreach (var k in kontenery)
                Console.WriteLine($"{k} | Typ: {k.PobierzTypKontenera()}");
        }

        public void PowiadomONiebezpieczenstwie(string numer)
        {
            Console.WriteLine("Niebezpieczny kontener: {numer}");
        }
    }
}





