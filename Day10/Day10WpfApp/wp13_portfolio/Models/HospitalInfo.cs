using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wp13_portfolio.Models
{
    public class HospitalInfo
    {
        /*
            "gugun": "해운대구",
            "animal_hospital": "BS조은동물병원",
            "approval": "2011-02-10",
            "road_address": "부산광역시 해운대구 윗반송로 73 (반송동)",
            "tel": "051-544-7588",
            "lat": "35.230555",
            "lon": "129.156999",
            "basic_date": "2023-01-17"

        gugun
        animal_hospital
        approval
        road-address
        tel
        lat
        lon
        basic_date
         */

        public int Id { get; set; }
        public string Gugun { get; set; }
        public string Animal_Hospital { get; set; }
        public string Approval { get; set; }
        public string Road_Address { get; set; }
        public string Tel { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Basic_Date { get; set; }
    }



    
}
