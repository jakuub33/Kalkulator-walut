using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Storage;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace Kalkulator_walut
{
    /// <summary>
    /// Strona przedstawia kalkulator walut. Autor: Jakub Szymański.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string daneNBP = "http://www.nbp.pl/kursy/xml/LastA.xml"; //dane kursowe
        List<PozycjaTabeliA> kursyAktualne = new List<PozycjaTabeliA>(); //lista pozycji z danymi dla kolejnych walut 
        public MainPage()
        {
            this.InitializeComponent();
        }
        public class PozycjaTabeliA
        {
            public string przelicznik { get; set; }
            public string kod_waluty { get; set; }
            public string kurs_sredni { get; set; }
        }

        private async void OdczytDanych_Loaded(object sender, RoutedEventArgs e)
        {
            //dane o kursach ze strony NBP
            var serwerNBP = new HttpClient();
            string dane = "";
            try
            {
                dane = await serwerNBP.GetStringAsync(new Uri(daneNBP));
            }
            catch (Exception ex) { }

            if (dane != "")
            {
                kursyAktualne.Clear();
            }

            XDocument daneKursowe = XDocument.Parse(dane);
                        
            kursyAktualne = (from item in daneKursowe.Descendants("pozycja")
                select new PozycjaTabeliA()
                {
                    przelicznik = item.Element("przelicznik").Value,
                    kod_waluty = item.Element("kod_waluty").Value,
                    kurs_sredni = item.Element("kurs_sredni").Value
                }).ToList();

            kursyAktualne.Insert(0, new PozycjaTabeliA() { kurs_sredni = "1,0000", kod_waluty = "PLN", przelicznik = "1" });
            lbxZWaluty.ItemsSource = kursyAktualne;
            lbxNaWalute.ItemsSource = kursyAktualne;
            lbxZWaluty.SelectedIndex = 0;
            lbxNaWalute.SelectedIndex = 0;            
            
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("lbxZWaluty") 
                && ApplicationData.Current.LocalSettings.Values.ContainsKey("lbxNaWalute"))
            {                
                lbxZWaluty.SelectedIndex = Convert.ToInt32(ApplicationData.Current.LocalSettings.Values["lbxZWaluty"]);
                lbxZWaluty.SelectedIndex = Convert.ToInt32(ApplicationData.Current.LocalSettings.Values["lbxNaWalute"]);                
            }
            else
            {
                lbxZWaluty.SelectedIndex = -1;
                lbxNaWalute.SelectedIndex = -1;
            }
        }
        private void PrzeliczKwote_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtKwota.Text.Length > 0)
            {
                Oblicz();
            }                
            else
            {
                tbPrzeliczona.Text = "";
                Wiadomosc("Nic nie zostało wpisane!", "Uwaga!");
            }            
        }

        private void Oblicz()
        {
            double kwotaDocelowa = 0;
            if (txtKwota.Text.Length > 0)
            {                
                double kwota = double.Parse(txtKwota.Text);
                double kursWalutyWyjściowej = double.Parse(kursyAktualne[lbxZWaluty.SelectedIndex].kurs_sredni.Replace(',', '.'), CultureInfo.InvariantCulture);
                double kursWalutyDocelowej = double.Parse(kursyAktualne[lbxNaWalute.SelectedIndex].kurs_sredni.Replace(',', '.'), CultureInfo.InvariantCulture);
                kwotaDocelowa = (kwota * kursWalutyWyjściowej) / kursWalutyDocelowej;                

                tbPrzeliczona.Text = kwotaDocelowa.ToString("0.00", CultureInfo.InvariantCulture);
            }                            
        }

        static public async void Wiadomosc(string zawartosc, string tytul)
        {
            MessageDialog msg = new MessageDialog(zawartosc, tytul);
            await msg.ShowAsync();
        }

        private void Zwaluty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Oblicz();
            tbZWaluty.Text = kursyAktualne[lbxZWaluty.SelectedIndex].kod_waluty;            
            ApplicationData.Current.LocalSettings.Values["lbxZWaluty"] = lbxZWaluty.SelectedIndex;
        }

        private void NaWalute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Oblicz();
            tbNaWalute.Text = kursyAktualne[lbxNaWalute.SelectedIndex].kod_waluty;
            ApplicationData.Current.LocalSettings.Values["lbxNaWalute"] = lbxNaWalute.SelectedIndex;
        }

        private void btnOProgramie_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(OProgramie));
        }

        private void btnPomoc_Click(object sender, RoutedEventArgs e)
        {
            string waluta = $"{kursyAktualne[lbxZWaluty.SelectedIndex].kod_waluty} na walutę {kursyAktualne[lbxNaWalute.SelectedIndex].kod_waluty}";
            this.Frame.Navigate(typeof(Pomoc), waluta);  //dodaj co ma przekazywac         
        }        
    }
}
