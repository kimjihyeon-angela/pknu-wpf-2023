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
using System.Windows.Navigation;
using System.Windows.Shapes;
using wp05_bikeshop.Logics;

namespace wp05_bikeshop
{
    /// <summary>
    /// SupportPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TestPage : Page
    {
        Car myCar = null;

        public TestPage()
        {
            InitializeComponent();
            InitCar();
        }

        private void InitCar()
        {
            // 일반적인 C#에서 클래스 객체 인스턴스 사용방법과 동일
            myCar = new Car();
            //myCar.Names = "아이오닉";
            myCar.Colorz = Colors.White;
            myCar.Speed = 220;

            var rand = new Random(); // 랜덤 칼라를 만들기 위함
            // 리스트박스에 바인딩하기 위한 Car 리스트
            List<Car> cars = new List<Car>();
            // var cars = new List<Car>(); List<Car> 대신 var 적어도 됨
            for (int i = 0; i < 10; i++)
            {
                cars.Add(new Car()
                {
                    Speed = i * 10,
                    Colorz =  Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))
                });
            }

            this.DataContext = cars; 
            // this -> TestPage 전체를 의미함
            CtlCars.DataContext = cars;
            // this 대신 콤보박스에 바인딩을 직접하는 방법

            // DataContext
            // = 코드 비하인드에서 만든 데이터(DB, excel... )를 xaml 컨트롤에 바인딩하기 위해 값을 넣기 위함
            // 
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // 전통적인 윈폼 방식
            TxtSample.Text = myCar.Speed.ToString();
        }

        private void SldValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // 전통적인 윈폼 방식
            // 프로그레스바, 레이블 이름 다 지정해 줘야하고, 비하인드에서 코딩작업이 들어가야 하지만
            // wpf의 경우 그럴 필요 없음
            // PgbValue.Value = SldValue.Value;
            // LblValue.Content = SldValue.Value;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("이것도 버튼입니다.");
        }
    }
}
