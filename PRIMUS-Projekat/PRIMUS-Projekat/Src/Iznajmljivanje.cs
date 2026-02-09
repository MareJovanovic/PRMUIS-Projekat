namespace PRIMUS_Projekat.Src
{
    public class Iznajmljivanje
    {
        public string Naslov { get; set; }
        public string Autor { get; set; }
        public int ClanId { get; set; }
        public DateTime DatumVracanja { get; set; }
        public int BrojPrimeraka { get; set; }


        public Iznajmljivanje()
        {
        }

        public Iznajmljivanje(string naslov, string autor, int clanId, int broj)
        {
            Naslov = naslov;
            Autor = autor;
            ClanId = clanId;
            BrojPrimeraka = broj;
            DatumVracanja = DateTime.Now.AddDays(14);
        }


    }
}
