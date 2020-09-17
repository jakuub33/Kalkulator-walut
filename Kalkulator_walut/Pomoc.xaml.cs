﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Kalkulator_walut
{
    /// <summary>
    /// Strona wyświetla informacje jak działa program.
    /// </summary>
    public sealed partial class Pomoc : Page
    {
        public Pomoc()
        {
            this.InitializeComponent();
        }       

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string && !string.IsNullOrWhiteSpace((string)e.Parameter))
            {
                tbWaluta.Text = $"z waluty {e.Parameter.ToString()}";                
            }
            else
            {
                tbWaluta.Text = "";                
            }
            base.OnNavigatedTo(e);
        }

        private void btnPowrot_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}
