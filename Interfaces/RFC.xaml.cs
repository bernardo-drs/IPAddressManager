using System;
using System.Collections.Generic;
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
using Systeme;

namespace Interfaces
{
    /// <summary>
    /// Interaction logic for RFC.xaml
    /// </summary>
    public partial class RFC : Page
    {
        private readonly List<PlageRFC> _toutesLesPlages;
        public RFC()
        {
            InitializeComponent();

            var plages = ChargerDonneesRFC();
            RfcDataGrid.ItemsSource = plages;
        }

        private List<PlageRFC> ChargerDonneesRFC()
        {
            return new List<PlageRFC>
            {
                new PlageRFC { PlageAdresses = "0.0.0.0/8",          Utilisation = "Ce réseau",                                    Année = 1981  },
                new PlageRFC { PlageAdresses = "10.0.0.0/8",         Utilisation = "Espace d'adressage privé",                     Année = 1996  },
                new PlageRFC { PlageAdresses = "100.64.0.0/10",      Utilisation = "Espace d'adressage partagé (CGNAT)",           Année = 2012  },
                new PlageRFC { PlageAdresses = "127.0.0.0/8",        Utilisation = "Boucle locale (localhost)",                    Année = 1986 },
                new PlageRFC { PlageAdresses = "169.254.0.0/16",     Utilisation = "Lien local (APIPA)",                           Année = 2005 },
                new PlageRFC { PlageAdresses = "172.16.0.0/12",      Utilisation = "Espace d'adressage privé",                     Année = 1996 },
                new PlageRFC { PlageAdresses = "192.0.0.0/24",       Utilisation = "Protocole IETF",                               Année = 2010 },
                new PlageRFC { PlageAdresses = "192.0.2.0/24",       Utilisation = "Documentation (TEST-NET-1)",                   Année = 2010 },
                new PlageRFC { PlageAdresses = "192.88.99.0/24",     Utilisation = "Relais IPv6 vers IPv4",                        Année = 2001 },
                new PlageRFC { PlageAdresses = "192.168.0.0/16",     Utilisation = "Espace d'adressage privé",                     Année = 1996 },
                new PlageRFC { PlageAdresses = "198.18.0.0/15",      Utilisation = "Tests de benchmark",                           Année = 1999 },
                new PlageRFC { PlageAdresses = "198.51.100.0/24",    Utilisation = "Documentation (TEST-NET-2)",                   Année = 2010 },
                new PlageRFC { PlageAdresses = "203.0.113.0/24",     Utilisation = "Documentation (TEST-NET-3)",                   Année = 2010 },
                new PlageRFC { PlageAdresses = "224.0.0.0/4",        Utilisation = "Multicast",                                    Année = 1989  },
                new PlageRFC { PlageAdresses = "240.0.0.0/4",        Utilisation = "Réservé pour un usage futur",                  Année = 1989  },
                new PlageRFC { PlageAdresses = "255.255.255.255/32", Utilisation = "Broadcast limité",                             Année = 1984 },
            };
        }
    }
}
