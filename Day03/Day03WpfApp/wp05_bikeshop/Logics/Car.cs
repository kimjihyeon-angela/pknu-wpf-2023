using System.Windows.Media;

namespace wp05_bikeshop.Logics
{
    internal class Car : Notifier // 값이 변경되는 것을 인지하여 처리
    {
        private string names;
        public string Name
        {
            get => names;
            // 프로퍼티를 변경하는 것
            set
            {
                names = value;
                OnPropertyChanged("Names"); // Names 프로퍼티가 변경됨을 인지시켜 처리 유도
            }
        }
        private double speed;
        public double Speed
        {
            get => speed;
            set
            {
                speed = value;
                OnPropertyChanged(nameof(Speed));
                // OnPropertyChanged("Speed");
            }
        }
        //public string Names { get; set; } // Auto Property
        //public double Speed { get; set; }
        public Color Colorz { get; set; }  
        public Human Driver { get; set; }
    }
}
