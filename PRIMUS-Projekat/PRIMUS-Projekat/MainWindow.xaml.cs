using PRIMUS_Projekat.Src;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PRIMUS_Projekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        private KnjigaServis knjigaServis;
        new ObservableCollection<Knjiga> knjige;

        public MainWindow()
        {
            InitializeComponent();

            knjigaServis = new KnjigaServis();
            knjige = new ObservableCollection<Knjiga>(knjigaServis.getKnjige());

            PrikazKnjiga.ItemsSource = knjige;
            BookLIst.ItemsSource = knjige;
        }
       
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Knjiga k = new Knjiga(NaslovK.Text, AutorK.Text, int.Parse(KolicinaK.Text));
                if (knjigaServis.Dodaj(k))
                {
                    knjige.Add(k);
                    MessageBox.Show("Uspesno ste dodali knjigu");
                    NaslovK.Clear();
                    AutorK.Clear();
                    KolicinaK.Clear();
                }
                else
                {
                    MessageBox.Show("Greska pri unosu");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Neispravan format unosa");
            }
        }

        private void DLTBook_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Da li ste sigurni da zelite da obrisete knjigu", "Potvrdi brisanje", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                knjige.RemoveAt(PrikazKnjiga.SelectedIndex);
                MessageBox.Show("Knjiga obrisana");
            }
            else
            {
                MessageBox.Show("Neuspesno brisanje");
            }
        }
    }
}