using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRIMUS_Projekat.Src
{
    class Knjiga
    {
        private string naslov;
        private string autor;
        private int kolicina;

        public event PropertyChangingEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string v)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangingEventArgs(v));
            }
        }
        public Knjiga()
        {
            this.naslov = "";
            this.autor = "";
            this.kolicina = 0;
        }
        public Knjiga(string naslov, string autor, int kolicina)
        {
            this.naslov = naslov;
            this.autor = autor;
            this.kolicina = kolicina;
        }
        public string Naslov
        {
            get { return this.naslov; }
            set
            {
                if (this.naslov != value)
                {
                    this.naslov = value;
                    this.NotifyPropertyChanged("Naslov");
                }
            }
        }
        public string Autor
        {
            get { return this.autor; }
            set
            {
                if (this.autor != value)
                {
                    this.autor = value;
                    this.NotifyPropertyChanged("Autor");
                }
            }
        }
        public int Kolicina
        {
            get { return this.kolicina; }
            set
            {
                if (this.kolicina != value)
                {
                    this.kolicina = value;
                    this.NotifyPropertyChanged("Kolicina");
                }
            }
        }
        public override string ToString()
        {
            return $"{naslov} {autor} {kolicina}";
        }
    }
}
