using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Systeme;

namespace Interfaces
{
    /// <summary>
    /// Interaction logic for Calculateur.xaml
    /// </summary>
    public partial class Calculateur : Page
    {
        public Calculateur()
        {
            InitializeComponent();
        }

        private void TxtCidr_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtDecimal == null || TxtCidr == null) return;

            if (string.IsNullOrWhiteSpace(TxtCidr.Text) || !int.TryParse(TxtCidr.Text, out int cidr))
            {
                TxtDecimal.Text = "...";
                return;
            }

            if (cidr < 0 || cidr > 32)
            {
                TxtDecimal.Text = "Invalide";
                return;
            }

            uint bits = cidr == 0 ? 0 : uint.MaxValue << (32 - cidr);
            byte o1 = (byte)((bits & 0xFF000000) >> 24);
            byte o2 = (byte)((bits & 0x00FF0000) >> 16);
            byte o3 = (byte)((bits & 0x0000FF00) >> 8);
            byte o4 = (byte)(bits & 0x000000FF);

            TxtDecimal.Text = $"{o1}.{o2}.{o3}.{o4}";
        }

        private const string BG_COLOR_CALCULER = "#1E293B"; 
        private const string BG_HOVER_COLOR_CALCULER = "#2563EB";

        private void FadeCalculerButtonColor(Border border, string mode)
        {
            if (border == null) return;

            ColorAnimation fade = new ColorAnimation();

            fade.From = (Color)ColorConverter.ConvertFromString(mode == "In" ? BG_COLOR_CALCULER : BG_HOVER_COLOR_CALCULER);
            fade.To = (Color)ColorConverter.ConvertFromString(mode == "In" ? BG_HOVER_COLOR_CALCULER : BG_COLOR_CALCULER);
            fade.Duration = TimeSpan.FromSeconds(0.15);

            SolidColorBrush brush = new SolidColorBrush();
            border.Background = brush;
            brush.BeginAnimation(SolidColorBrush.ColorProperty, fade);
        }

        private void BtnCalculer_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            FadeCalculerButtonColor(border, "In");
        }

        private void BtnCalculer_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            FadeCalculerButtonColor(border, "Out");
        }

        private void BtnCalculer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!byte.TryParse(TxtIp1.Text, out byte ip1) ||
                !byte.TryParse(TxtIp2.Text, out byte ip2) ||
                !byte.TryParse(TxtIp3.Text, out byte ip3) ||
                !byte.TryParse(TxtIp4.Text, out byte ip4) ||
                !int.TryParse(TxtCidr.Text, out int cidr) || cidr < 0 || cidr > 32)
            {
                MessageBox.Show("Veuillez saisir une adresse IPv4 valide et un CIDR entre 0 et 32.", "Saisie invalide", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            uint ipAddress = ((uint)ip1 << 24) | ((uint)ip2 << 16) | ((uint)ip3 << 8) | ip4;

            uint subnetMask = cidr == 0 ? 0 : uint.MaxValue << (32 - cidr);

            uint networkAddress = ipAddress & subnetMask;
            uint broadcastAddress = networkAddress | ~subnetMask;

            uint firstHost = networkAddress + 1;
            uint lastHost = broadcastAddress - 1;
            uint numberOfHosts = cidr >= 31 ? 0 : broadcastAddress - networkAddress - 1;

            TxtAdresse.Text = UintToIpString(networkAddress);
            TxtBroadcast.Text = UintToIpString(broadcastAddress);

            if (cidr >= 31)
            {
                TxtPremièreAdresse.Text = "N/A";
                TxtDernièreAdresse.Text = "N/A";
                TxtNombreHôte.Text = "0";
            }
            else
            {
                TxtPremièreAdresse.Text = UintToIpString(firstHost);
                TxtDernièreAdresse.Text = UintToIpString(lastHost);
                TxtNombreHôte.Text = numberOfHosts.ToString("N0", System.Globalization.CultureInfo.InvariantCulture);
            }

            string fullBinaryMask = Convert.ToString(subnetMask, 2).PadLeft(32, '0');
            string formattedBinary = $"{fullBinaryMask.Substring(0, 8)}.{fullBinaryMask.Substring(8, 8)}.{fullBinaryMask.Substring(16, 8)}.{fullBinaryMask.Substring(24, 8)}";

            int firstZeroIndex = formattedBinary.IndexOf('0');
            if (firstZeroIndex == -1)
            {
                TxtBinMasqueUn.Text = formattedBinary;
                TxtBinMasqueZero.Text = "";
            }
            else
            {
                TxtBinMasqueUn.Text = formattedBinary.Substring(0, firstZeroIndex);
                TxtBinMasqueZero.Text = formattedBinary.Substring(firstZeroIndex);
            }


            string fullBinaryIp = Convert.ToString(ipAddress, 2).PadLeft(32, '0');

            string formattedBinaryIp = $"{fullBinaryIp.Substring(0, 8)}.{fullBinaryIp.Substring(8, 8)}.{fullBinaryIp.Substring(16, 8)}.{fullBinaryIp.Substring(24, 8)}";

            TxtBinIpComplete.Text = formattedBinaryIp;
        }

        private string UintToIpString(uint value)
        {
            return $"{(value >> 24) & 0xFF}.{(value >> 16) & 0xFF}.{(value >> 8) & 0xFF}.{value & 0xFF}";
        }
    }
}
