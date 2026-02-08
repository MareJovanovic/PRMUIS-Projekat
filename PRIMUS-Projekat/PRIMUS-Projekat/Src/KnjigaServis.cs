using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRIMUS_Projekat.Src
{
    class KnjigaServis
    {
        private List<Knjiga> knjige = new List<Knjiga> { };
        
        public KnjigaServis()
        {
            knjige.Add(new Knjiga("Na Drini ćuprija", "Ivo Andrić", 10));
            knjige.Add(new Knjiga("Zov daljine", "Ljubica Arsić", 20));
            knjige.Add(new Knjiga("Lovac u žitu", "Džerom Dejvid Selindžer", 5));
        }

        public List<Knjiga> getKnjige()
        {
            return knjige;
        }
        public bool Dodaj(Knjiga k)
        {
            if (k == null || k.Naslov == "" || k.Autor == "" || k.Kolicina == 0)
            {
                return false;
            }
            else
            {
                foreach (Knjiga knj in knjige)
                {
                    if (knj.Naslov == k.Naslov && knj.Autor == k.Autor && knj.Kolicina == k.Kolicina)
                    {
                        return false;
                    }
                }
                knjige.Add(k);
                return true;
            }
        }
    }
}
