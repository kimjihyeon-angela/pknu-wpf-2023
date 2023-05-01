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
using wp14_portfolio.Models;

namespace wp14_portfolio
{
    /// <summary>
    /// DetailWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DetailWindow : MetroWindow
    {
        List<MedicalInfo> medicalInfos = null; 
        public DetailWindow()
        {
            InitializeComponent();
        }

        public DetailWindow(string hosName, string exam_parts, string mon, string tues, string wed, string thur, string fri, string sat, string sun, string holi, string sun_oper,
                            double lat, double lon) : this()
        {
            LblMediName.Content = $"{hosName}";
            //LblExam_part.Content = $"{exam_parts}";
            TxtExam_part.Text = $"{exam_parts}";
            LblMon.Content = $"{mon}";
            LblTue.Content = $"{tues}";
            LblWed.Content = $"{wed}";
            LblThur.Content = $"{thur}";
            LblFri.Content = $"{fri}";
            LblSat.Content = $"{sat}";
            LblSun.Content = $"{sun}";
            LblHoli.Content = $"{holi}";
            LblSun_oper.Content = $"{sun_oper}";

            BrsLocAnimalHospital.Address = $"https://www.google.com/maps/place/{lat},{lon}";

        }

        

        private Task LoadDataCollection()
        {
            throw new NotImplementedException();
        }
    }
}
