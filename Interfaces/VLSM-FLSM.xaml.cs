using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Systeme;

namespace Interfaces
{
    public partial class VLSM_FLSM : Page
    {
        public VLSM_FLSM()
        {
            InitializeComponent();
        }

        private void BtnCalculateVlsm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IPAddress.TryParse(TxtBaseIp.Text, out IPAddress baseIp))
                {
                    MessageBox.Show("Adresse IP invalide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(TxtBaseMask.Text, out int baseMask) || baseMask < 0 || baseMask > 32)
                {
                    MessageBox.Show("Masque CIDR invalide (entre 0 et 32).", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string inputSizes = TxtVlsmSizes.Text;
                if (string.IsNullOrWhiteSpace(inputSizes))
                {
                    MessageBox.Show("Veuillez saisir au moins une taille de sous-réseau.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                List<int> requestedHosts = inputSizes.Split(',')
                                                     .Select(s => s.Trim())
                                                     .Where(s => !string.IsNullOrEmpty(s))
                                                     .Select(int.Parse)
                                                     .ToList();

                var sortedHosts = requestedHosts.OrderByDescending(h => h).ToList();
                List<VlsmFlsmResult> vlsmResults = new List<VlsmFlsmResult>();

                uint currentIpBytes = IpToUint(baseIp);
                int subnetCounter = 1;

                foreach (int hosts in sortedHosts)
                {
                    int requiredSize = hosts + 2;
                    int power = 2;
                    while (Math.Pow(2, power) < requiredSize)
                    {
                        power++;
                    }

                    int cidr = 32 - power;
                    int realHosts = (int)Math.Pow(2, power) - 2;

                    uint networkAddr = currentIpBytes;
                    uint firstIp = networkAddr + 1;
                    uint lastIp = networkAddr + (uint)realHosts;
                    uint broadcastAddr = networkAddr + (uint)realHosts + 1;

                    vlsmResults.Add(new VlsmFlsmResult
                    {
                        Name = $"Sous-réseau {subnetCounter++}",
                        RequestedHosts = hosts,
                        RealHosts = realHosts,
                        NetworkAddress = UintToIp(networkAddr).ToString(),
                        SubnetMask = CidrToMask(cidr),
                        Cidr = $"/{cidr}",
                        BroadcastAddress = UintToIp(broadcastAddr).ToString(),
                        IpRange = $"{UintToIp(firstIp)} - {UintToIp(lastIp)}"
                    });

                    currentIpBytes = broadcastAddr + 1;
                }

                if (GridFlsmResults != null) GridFlsmResults.ItemsSource = null;

                GridVlsmResults.ItemsSource = vlsmResults;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du calcul VLSM : {ex.Message}", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnCalculateFlsm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IPAddress.TryParse(TxtBaseIp.Text, out IPAddress baseIp) ||
                    !int.TryParse(TxtBaseMask.Text, out int baseMask) || baseMask < 0 || baseMask > 32)
                {
                    MessageBox.Show("IP de base ou masque principal invalide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                bool hasSubnetCount = int.TryParse(TxtFlsmSubnetCount.Text.Trim(), out int requiredSubnets) && requiredSubnets > 0;
                bool hasHostSize = int.TryParse(TxtFlsmHostSize.Text.Trim(), out int requiredHosts) && requiredHosts > 0;

                if (!hasSubnetCount && !hasHostSize)
                {
                    MessageBox.Show("Veuillez saisir au moins le nombre de sous-réseaux ou la taille souhaitée.", "Champs vides", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int newCidr = baseMask;
                int subnetsToDisplay = 1;

                if (hasHostSize)
                {
                    int totalAddressesNeeded = requiredHosts + 2;
                    int bitsNeededForHosts = (int)Math.Ceiling(Math.Log(totalAddressesNeeded, 2));
                    if (bitsNeededForHosts < 2) bitsNeededForHosts = 2;

                    newCidr = 32 - bitsNeededForHosts;

                    if (hasSubnetCount)
                    {
                        subnetsToDisplay = requiredSubnets;

                        int maxPossibleSubnets = (int)Math.Pow(2, newCidr - baseMask);
                        if (requiredSubnets > maxPossibleSubnets)
                        {
                            MessageBox.Show($"Impossible de créer {requiredSubnets} sous-réseaux de cette taille dans ce réseau principal.", "Capacité insuffisante", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    else
                    {
                        subnetsToDisplay = (int)Math.Pow(2, newCidr - baseMask);
                    }
                }
                else if (hasSubnetCount)
                {
                    int bitsNeededForSubnets = (int)Math.Ceiling(Math.Log(requiredSubnets, 2));
                    newCidr = baseMask + bitsNeededForSubnets;
                    subnetsToDisplay = (int)Math.Pow(2, bitsNeededForSubnets);
                }

                if (newCidr > 32)
                {
                    MessageBox.Show("Le découpage demandé dépasse les limites de l'espace IPv4.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                List<VlsmFlsmResult> flsmResults = new List<VlsmFlsmResult>();
                uint currentIpBytes = IpToUint(baseIp);
                int blockSize = (int)Math.Pow(2, 32 - newCidr);
                int realHostsPerSubnet = blockSize - 2 < 0 ? 0 : blockSize - 2;

                int displayLimit = Math.Min(subnetsToDisplay, 256);

                for (int i = 0; i < displayLimit; i++)
                {
                    uint networkAddr = currentIpBytes + (uint)(i * blockSize);
                    uint firstIp = networkAddr + 1;
                    uint broadcastAddr = networkAddr + (uint)blockSize - 1;
                    uint lastIp = broadcastAddr - 1;

                    flsmResults.Add(new VlsmFlsmResult
                    {
                        Name = $"Sous-réseau {i + 1}",
                        RequestedHosts = hasHostSize ? requiredHosts : realHostsPerSubnet,
                        RealHosts = realHostsPerSubnet,
                        NetworkAddress = UintToIp(networkAddr).ToString(),
                        SubnetMask = CidrToMask(newCidr),
                        Cidr = $"/{newCidr}",
                        BroadcastAddress = UintToIp(broadcastAddr).ToString(),
                        IpRange = $"{UintToIp(firstIp)} - {UintToIp(lastIp)}"
                    });
                }

                if (GridVlsmResults != null) GridVlsmResults.ItemsSource = null;

                GridFlsmResults.ItemsSource = flsmResults;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du calcul FLSM : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private uint IpToUint(IPAddress ip)
        {
            byte[] bytes = ip.GetAddressBytes();
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        private IPAddress UintToIp(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return new IPAddress(bytes);
        }

        private string CidrToMask(int cidr)
        {
            if (cidr == 0) return "0.0.0.0";
            uint mask = uint.MaxValue << (32 - cidr);
            return UintToIp(mask).ToString();
        }
    }
}