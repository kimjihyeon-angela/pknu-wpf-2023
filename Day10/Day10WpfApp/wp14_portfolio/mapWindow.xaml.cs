using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wp14_portfolio
{
    /// <summary>
    /// mapWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class mapWindow : MetroWindow
    {
        public mapWindow()
        {
            InitializeComponent();
        }

        public mapWindow(double lat, double lon) : this()
        {
            BrsLocAnimalHospital.Address = $"https://www.google.com/maps/place/{lat},{lon}";
        }
    }
}
