using Org.BouncyCastle.Asn1.TeleTrust;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wp14_portfolio.Models
{
    public class MedicalInfo
    {
        /*
        instit_nm": "트레이더스약국", 의료기관명
        "instit_kind": "약국", 기관 분류
        "medical_instit_kind": "응급의료기관이외",의료기관 분류
        "zip_code": "47569", 우편번호
        "street_nm_addr": "부산광역시 연제구 좌수영로 241, 지하3층 (연산동)", 도로명주소
        "tel": "051-505-2680", 대표전화
        "organ_loc": "이마트 트레이더스 내 위치", 기관위치 설정
        "Monday": "10:00~21:00",
        "Tuesday": "10:00~21:00",
        "Wednesday": "10:00~21:00",
        "Thursday": "10:00~21:00",
        "Friday": "10:00~21:00",
        "Saturday": "10:00~21:00",
        "Sunday": "10:00~21:00",
        "holiday": "10:00~21:00",
        "sunday_oper_week": "1 2 3 4 5", 일요일 진료 주
        "exam_part": "", 진료과목
        "regist_dt": "2021-02-10", 등록일
        "update_dt": "2021-02-10", 수정일시
        "lng": "129.0802078", 경도
        "lat": "35.18408381"  위도
         */

        public int Id { get; set; }
        public string Instit_nm { get; set; }
        public string Instit_kind { get; set; }
        public string Medical_instit_kind { get; set; }
        public int Zip_code { get; set; }
        public string Street_nm_addr { get; set; }
        public string Tel { get; set; }
        public string Organ_loc { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set;}
        public string Friday { get; set;}
        public string Saturday { get;set; }
        public string Sunday { get; set; }
        public string Holiday { get; set; }
        public string Sunday_oper_week { get; set; }
        public string Exam_part { get; set; }
        public DateTime Regist_dt { get; set; }
        public DateTime Update_dt { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; } 
    }
}
