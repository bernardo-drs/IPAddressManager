using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Systeme
{
    public class PlageRFC : INotifyPropertyChanged
    {
        private string _plageAdresses;
        private string _utilisation;
        private int _annee;

        public string PlageAdresses
        {
            get => _plageAdresses;
            set { _plageAdresses = value; OnPropertyChanged(nameof(PlageAdresses)); }
        }
        public string Utilisation
        {
            get => _utilisation;
            set { _utilisation = value; OnPropertyChanged(nameof(Utilisation)); }
        }
        public int Année
        {
            get => _annee;
            set { _annee = value; OnPropertyChanged(nameof(Année)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class VlsmFlsmResult
    {
        public string Name { get; set; }
        public int RequestedHosts { get; set; }
        public int RealHosts { get; set; }
        public string NetworkAddress { get; set; }
        public string SubnetMask { get; set; }
        public string Cidr { get; set; }
        public string BroadcastAddress { get; set; }
        public string IpRange { get; set; } // Plage Utilisable
    }
}
