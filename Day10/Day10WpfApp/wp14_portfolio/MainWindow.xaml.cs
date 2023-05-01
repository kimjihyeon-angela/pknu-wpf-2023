using CefSharp.DevTools.Runtime;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
using wp14_portfolio.Logics;
using wp14_portfolio.Models;

namespace wp14_portfolio
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region < 콤보박스 선택 이벤트 >
        private void CboReqDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboReqDate.SelectedValue != null)
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"SELECT 
                                         Instit_nm
                                       , Instit_kind
                                       , Medical_instit_kind
                                       , zip_code
                                       , street_nm_addr
                                       , tel
                                       , organ_loc
                                       , monday
                                       , tuesday
                                       , wednesday
                                       , thursday
                                       , friday
                                       , saturday
                                       , sunday
                                       , holiday
                                       , sunday_oper_week
                                       , exam_part
                                       , regist_dt
                                       , update_dt
                                       , lng
                                       , lat
                                    FROM medicalinfo
                                   WHERE Instit_kind = @Instit_kind";
                    MySqlCommand cmd = new MySqlCommand(query,conn);
                    cmd.Parameters.AddWithValue("@Instit_kind", CboReqDate.SelectedValue.ToString());
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "medicalInfo");
                    List<MedicalInfo> medicalInfors = new List<MedicalInfo>();

                    foreach (DataRow row in ds.Tables["medicalInfo"].Rows)
                    {
                        medicalInfors.Add(new MedicalInfo()
                        {
                            Instit_nm = Convert.ToString(row["instit_nm"]),
                            Instit_kind = Convert.ToString(row["instit_kind"]),
                            Medical_instit_kind = Convert.ToString(row["medical_instit_kind"]),
                            Zip_code = Convert.ToInt32(row["zip_code"]),
                            Street_nm_addr = Convert.ToString(row["street_nm_addr"]),
                            Tel = Convert.ToString(row["tel"]),
                            Organ_loc = Convert.ToString(row["organ_loc"]),
                            Monday = Convert.ToString(row["Monday"]),
                            Tuesday = Convert.ToString(row["Tuesday"]),
                            Wednesday = Convert.ToString(row["Wednesday"]),
                            Thursday = Convert.ToString(row["Thursday"]),
                            Friday = Convert.ToString(row["Friday"]),
                            Saturday = "휴무",
                            Sunday = "휴무",
                            Holiday = Convert.ToString(row["holiday"]),
                            Sunday_oper_week = "일요일 휴무",
                            Exam_part = Convert.ToString(row["exam_part"]),
                            Regist_dt = Convert.ToDateTime(row["regist_dt"]),
                            Update_dt = Convert.ToDateTime(row["update_dt"]),
                            Lng = Convert.ToDouble(row["lng"]),
                            Lat = Convert.ToDouble(row["lat"])
                        });
                        
                    }

                    this.DataContext = medicalInfors;
                    StsResult.Content = $"DB {medicalInfors.Count} 건 조회 완료";
                }
            }

            else
            {
                this.DataContext = null;
                StsResult.Content = $"DB 조회 클리어";
            }
        }
        #endregion

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearch_Click(sender, e);
            }
        }

        #region < 검색 버튼 이벤트 >
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (TxtSearch.Text != null)
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"SELECT Id
                                       , Instit_nm
                                       , Instit_kind
                                       , Medical_instit_kind
                                       , zip_code
                                       , street_nm_addr
                                       , tel
                                       , organ_loc
                                       , monday
                                       , tuesday
                                       , wednesday
                                       , thursday
                                       , friday
                                       , saturday
                                       , sunday
                                       , holiday
                                       , sunday_oper_week
                                       , exam_part
                                       , regist_dt
                                       , update_dt
                                       , lng
                                       , lat
                                    FROM medicalinfo
                                   WHERE Instit_nm like @Instit_nm";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    var Instit_nms = "%" + TxtSearch.Text + "%";
                    cmd.Parameters.AddWithValue("@Instit_nm", Instit_nms);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "medicalInfo");
                    List<MedicalInfo> medicalInfos = new List<MedicalInfo>();

                    foreach (DataRow row in ds.Tables["medicalInfo"].Rows)
                    {
                        medicalInfos.Add(new MedicalInfo()
                        {
                            Instit_nm = Convert.ToString(row["instit_nm"]),
                            Instit_kind = Convert.ToString(row["instit_kind"]),
                            Medical_instit_kind = Convert.ToString(row["medical_instit_kind"]),
                            Zip_code = Convert.ToInt32(row["zip_code"]),
                            Street_nm_addr = Convert.ToString(row["street_nm_addr"]),
                            Tel = Convert.ToString(row["tel"]),
                            Organ_loc = Convert.ToString(row["organ_loc"]),
                            Monday = Convert.ToString(row["Monday"]),
                            Tuesday = Convert.ToString(row["Tuesday"]),
                            Wednesday = Convert.ToString(row["Wednesday"]),
                            Thursday = Convert.ToString(row["Thursday"]),
                            Friday = Convert.ToString(row["Friday"]),
                            Saturday = Convert.ToString(row["Saturday"]),
                            Sunday = Convert.ToString(row["Sunday"]),
                            Holiday = Convert.ToString(row["holiday"]),
                            Sunday_oper_week = Convert.ToString(row["sunday_oper_week"]),
                            Exam_part = Convert.ToString(row["exam_part"]),
                            Regist_dt = Convert.ToDateTime(row["regist_dt"]),
                            Update_dt = Convert.ToDateTime(row["update_dt"]),
                            Lng = Convert.ToDouble(row["lng"]),
                            Lat = Convert.ToDouble(row["lat"])
                        });
                    }

                    this.DataContext = medicalInfos;
                    StsResult.Content = $"DB {medicalInfos.Count} 건 조회 완료";
                }
            }
            else
            {
                SearchMedicalInfo();
            }
        }
        #endregion


        #region < 실제 검색 메소드 >
        private async void SearchMedicalInfo()
        {
            string apikey = "4Ng%2FJiC8rO2xbjXzST7yzTCSfv2%2Bw57zIYoXivUfh1VIpBkQGln74kS5%2FyQpCMoANkXUJXlHWBEqNUrhHMo3zQ%3D%3D";
        // url (병원 정보 검색)
            string url = $@"https://apis.data.go.kr/6260000/MedicInstitService/MedicalInstitInfo?serviceKey={apikey}&pageNo=1&numOfRows=1000&resultType=json";


            // 결과값 초기화
            string result = string.Empty;

            // api 실행할 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            // 공공데이터 포털 API 요청
            try
            {
                req = WebRequest.Create(url); // URL을 넣어서 객체 생성
                res = await req.GetResponseAsync(); // 요청한 결과를 응답에 할당 (비동기 응답)
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd(); // json 결과를 텍스트로 저장

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                res.Close();
            }

            var jsonResult = JObject.Parse(result);
            var resultCode = Convert.ToString(jsonResult["MedicalInstitInfo"]["header"]["resultCode"]);



            try
            {
                if (resultCode == "00")
                {
                    var item = jsonResult["MedicalInstitInfo"]["body"]["items"]["item"];
                    var json_array = item as JArray;

                    var hospitalInfo = new List<MedicalInfo>();

                    foreach (var mediInfo in json_array)
                    {
                        if ((Convert.ToString(mediInfo["sunday_oper_week"]) == "") || (Convert.ToString(mediInfo["Sunday"]) == "~"))
                        {
                            if ((Convert.ToString(mediInfo["Saturday"]) == "~"))
                            {
                                hospitalInfo.Add(new MedicalInfo
                                {
                                    Id = 0,
                                    Instit_nm = Convert.ToString(mediInfo["instit_nm"]),
                                    Instit_kind = Convert.ToString(mediInfo["instit_kind"]),
                                    Medical_instit_kind = Convert.ToString(mediInfo["medical_instit_kind"]),
                                    Zip_code = Convert.ToInt32(mediInfo["zip_code"]),
                                    Street_nm_addr = Convert.ToString(mediInfo["street_nm_addr"]),
                                    Tel = Convert.ToString(mediInfo["tel"]),
                                    Organ_loc = Convert.ToString(mediInfo["organ_loc"]),
                                    Monday = Convert.ToString(mediInfo["Monday"]),
                                    Tuesday = Convert.ToString(mediInfo["Tuesday"]),
                                    Wednesday = Convert.ToString(mediInfo["Wednesday"]),
                                    Thursday = Convert.ToString(mediInfo["Thursday"]),
                                    Friday = Convert.ToString(mediInfo["Friday"]),
                                    Saturday = "휴무",
                                    Sunday = "휴무",
                                    Holiday = Convert.ToString(mediInfo["holiday"]),
                                    Sunday_oper_week = "일요일 휴무",
                                    Exam_part = Convert.ToString(mediInfo["exam_part"]),
                                    Regist_dt = Convert.ToDateTime(mediInfo["regist_dt"]),
                                    Update_dt = Convert.ToDateTime(mediInfo["update_dt"]),
                                    Lng = Convert.ToDouble(mediInfo["lng"]),
                                    Lat = Convert.ToDouble(mediInfo["lat"])
                                });
                            }
                            else if ((Convert.ToString(mediInfo["holiday"]) == "~"))
                            {
                                hospitalInfo.Add(new MedicalInfo
                                {
                                    Id = 0,
                                    Instit_nm = Convert.ToString(mediInfo["instit_nm"]),
                                    Instit_kind = Convert.ToString(mediInfo["instit_kind"]),
                                    Medical_instit_kind = Convert.ToString(mediInfo["medical_instit_kind"]),
                                    Zip_code = Convert.ToInt32(mediInfo["zip_code"]),
                                    Street_nm_addr = Convert.ToString(mediInfo["street_nm_addr"]),
                                    Tel = Convert.ToString(mediInfo["tel"]),
                                    Organ_loc = Convert.ToString(mediInfo["organ_loc"]),
                                    Monday = Convert.ToString(mediInfo["Monday"]),
                                    Tuesday = Convert.ToString(mediInfo["Tuesday"]),
                                    Wednesday = Convert.ToString(mediInfo["Wednesday"]),
                                    Thursday = Convert.ToString(mediInfo["Thursday"]),
                                    Friday = Convert.ToString(mediInfo["Friday"]),
                                    Saturday = Convert.ToString(mediInfo["Saturday"]),
                                    Sunday = "휴무",
                                    Holiday = "휴무",
                                    Sunday_oper_week = "일요일 휴무",
                                    Exam_part = Convert.ToString(mediInfo["exam_part"]),
                                    Regist_dt = Convert.ToDateTime(mediInfo["regist_dt"]),
                                    Update_dt = Convert.ToDateTime(mediInfo["update_dt"]),
                                    Lng = Convert.ToDouble(mediInfo["lng"]),
                                    Lat = Convert.ToDouble(mediInfo["lat"])
                                });
                            }
                            else
                            {
                                hospitalInfo.Add(new MedicalInfo
                                {
                                    Id = 0,
                                    Instit_nm = Convert.ToString(mediInfo["instit_nm"]),
                                    Instit_kind = Convert.ToString(mediInfo["instit_kind"]),
                                    Medical_instit_kind = Convert.ToString(mediInfo["medical_instit_kind"]),
                                    Zip_code = Convert.ToInt32(mediInfo["zip_code"]),
                                    Street_nm_addr = Convert.ToString(mediInfo["street_nm_addr"]),
                                    Tel = Convert.ToString(mediInfo["tel"]),
                                    Organ_loc = Convert.ToString(mediInfo["organ_loc"]),
                                    Monday = Convert.ToString(mediInfo["Monday"]),
                                    Tuesday = Convert.ToString(mediInfo["Tuesday"]),
                                    Wednesday = Convert.ToString(mediInfo["Wednesday"]),
                                    Thursday = Convert.ToString(mediInfo["Thursday"]),
                                    Friday = Convert.ToString(mediInfo["Friday"]),
                                    Saturday = Convert.ToString(mediInfo["Saturday"]),
                                    Sunday = "휴무",
                                    Holiday = Convert.ToString(mediInfo["holiday"]),
                                    Sunday_oper_week = "일요일 휴무",
                                    Exam_part = Convert.ToString(mediInfo["exam_part"]),
                                    Regist_dt = Convert.ToDateTime(mediInfo["regist_dt"]),
                                    Update_dt = Convert.ToDateTime(mediInfo["update_dt"]),
                                    Lng = Convert.ToDouble(mediInfo["lng"]),
                                    Lat = Convert.ToDouble(mediInfo["lat"])
                                });
                            }
                        }

                        else 
                        {
                            hospitalInfo.Add(new MedicalInfo
                            {
                                Id = 0,
                                Instit_nm = Convert.ToString(mediInfo["instit_nm"]),
                                Instit_kind = Convert.ToString(mediInfo["instit_kind"]),
                                Medical_instit_kind = Convert.ToString(mediInfo["medical_instit_kind"]),
                                Zip_code = Convert.ToInt32(mediInfo["zip_code"]),
                                Street_nm_addr = Convert.ToString(mediInfo["street_nm_addr"]),
                                Tel = Convert.ToString(mediInfo["tel"]),
                                Organ_loc = Convert.ToString(mediInfo["organ_loc"]),
                                Monday = Convert.ToString(mediInfo["Monday"]),
                                Tuesday = Convert.ToString(mediInfo["Tuesday"]),
                                Wednesday = Convert.ToString(mediInfo["Wednesday"]),
                                Thursday = Convert.ToString(mediInfo["Thursday"]),
                                Friday = Convert.ToString(mediInfo["Friday"]),
                                Saturday = Convert.ToString(mediInfo["Saturday"]),
                                Sunday = Convert.ToString(mediInfo["Sunday"]),
                                Holiday = Convert.ToString(mediInfo["holiday"]),
                                Sunday_oper_week = Convert.ToString(mediInfo["sunday_oper_week"]),
                                Exam_part = Convert.ToString(mediInfo["exam_part"]),
                                Regist_dt = Convert.ToDateTime(mediInfo["regist_dt"]),
                                Update_dt = Convert.ToDateTime(mediInfo["update_dt"]),
                                Lng = Convert.ToDouble(mediInfo["lng"]),
                                Lat = Convert.ToDouble(mediInfo["lat"])
                            });
                        }
                    }
                    this.DataContext = hospitalInfo;
                    StsResult.Content = $"Open API {hospitalInfo.Count} 건 조회 완료";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"Json 데이터 처리 오류 {ex.Message}");
            }
        }
        #endregion

        #region < 저장 버튼 이벤트 >
        private async void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"INSERT INTO miniproject.medicalinfo
                                             (
                                              Instit_nm,
                                              Instit_kind,
                                              Medical_instit_kind,
                                              zip_code,
                                              street_nm_addr,
                                              tel,
                                              organ_loc,
                                              monday,
                                              tuesday,
                                              wednesday,
                                              thursday,
                                              friday,
                                              saturday,
                                              sunday,
                                              holiday,
                                              sunday_oper_week,
                                              exam_part,
                                              regist_dt,
                                              update_dt,
                                              lng,
                                              lat)
                                       VALUES
                                             (
                                              @Instit_nm,
                                              @Instit_kind,
                                              @Medical_instit_kind,
                                              @zip_code,
                                              @street_nm_addr,
                                              @tel,
                                              @organ_loc,
                                              @monday,
                                              @tuesday,
                                              @wednesday,
                                              @thursday,
                                              @friday,
                                              @saturday,
                                              @sunday,
                                              @holiday,
                                              @sunday_oper_week,
                                              @exam_part,
                                              @regist_dt,
                                              @update_dt,
                                              @lng,
                                              @lat)";

                    var insRes = 0;

                    foreach (var temp in GrdResult.Items)
                    {
                        if (temp is MedicalInfo)
                        {
                            var item = temp as MedicalInfo;
                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("Instit_nm", item.Instit_nm);
                            cmd.Parameters.AddWithValue("Instit_kind", item.Instit_kind);
                            cmd.Parameters.AddWithValue("Medical_instit_kind", item.Medical_instit_kind);
                            cmd.Parameters.AddWithValue("zip_code", item.Zip_code);
                            cmd.Parameters.AddWithValue("street_nm_addr", item.Street_nm_addr);
                            cmd.Parameters.AddWithValue("tel", item.Tel);
                            cmd.Parameters.AddWithValue("organ_loc", item.Organ_loc);
                            cmd.Parameters.AddWithValue("monday", item.Monday);
                            cmd.Parameters.AddWithValue("tuesday", item.Tuesday);
                            cmd.Parameters.AddWithValue("wednesday", item.Wednesday);
                            cmd.Parameters.AddWithValue("thursday", item.Thursday);
                            cmd.Parameters.AddWithValue("friday", item.Friday);
                            cmd.Parameters.AddWithValue("saturday", item.Saturday);
                            cmd.Parameters.AddWithValue("sunday", item.Sunday);
                            cmd.Parameters.AddWithValue("holiday", item.Holiday);
                            cmd.Parameters.AddWithValue("sunday_oper_week", item.Sunday_oper_week);
                            cmd.Parameters.AddWithValue("exam_part", item.Exam_part);
                            cmd.Parameters.AddWithValue("regist_dt", item.Regist_dt);
                            cmd.Parameters.AddWithValue("update_dt", item.Update_dt);
                            cmd.Parameters.AddWithValue("lng", item.Lng);
                            cmd.Parameters.AddWithValue("lat", item.Lat);

                            insRes += cmd.ExecuteNonQuery();
                        }

                    }

                    await Commons.ShowMessageAsync("저장", "DB 저장 성공");
                    StsResult.Content = $"DB 저장 {insRes} 건 성공";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB저장 오류 {ex.Message}");
            }
        }
        #endregion

        #region < 상세정보 + 지도 위치 띄우기 >
        private void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            var selItem = GrdResult.SelectedItem as MedicalInfo;

            string hosName = string.Empty;
            string exam_parts = string.Empty;
            string mon = string.Empty;
            string tues = string.Empty;
            string wed = string.Empty;
            string thur = string.Empty;
            string fri = string.Empty;
            string sat = string.Empty;
            string sun = string.Empty;
            string holi = string.Empty;
            string sun_oper = string.Empty;
            double lat = double.NaN;
            double lng = double.NaN;

            if (GrdResult.SelectedItem is MedicalInfo)
            {
                var mediInfo = GrdResult.SelectedItem as MedicalInfo;
                hosName = mediInfo.Instit_nm;
                exam_parts = mediInfo.Exam_part;
                mon = mediInfo.Monday;
                tues = mediInfo.Tuesday;
                wed = mediInfo.Wednesday;
                thur = mediInfo.Thursday;
                fri = mediInfo.Friday;
                sat = mediInfo.Saturday;
                sun = mediInfo.Sunday;
                holi = mediInfo.Holiday;
                sun_oper = mediInfo.Sunday_oper_week;
                lat = mediInfo.Lat;
                lng = mediInfo.Lng;

            }

            var DetailWindow = new DetailWindow(hosName, exam_parts, mon, tues, wed, thur, fri, sat, sun, holi, sun_oper, lat, lng);
            //var mapWindow = new mapWindow(selItem.Lat, selItem.Lng);

            DetailWindow.Owner = this;
            DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            DetailWindow.ShowDialog();

            //mapWindow.Owner = this;
            //mapWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //mapWindow.ShowDialog();
        }
        #endregion

        #region < 페이지 로딩될 때 병원 정보 불러오고, 콤보박스 채우기 >
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtSearch.Focus();
            SearchMedicalInfo();
            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                conn.Open();
                var query = @"SELECT Instit_kind FROM medicalInfo
                               GROUP BY 1";
                MySqlCommand cmd = new MySqlCommand(query,conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                List<string> saveDataList = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    saveDataList.Add(Convert.ToString(row["Instit_kind"]));
                }

                CboReqDate.ItemsSource = saveDataList;
            }
            
        }
        #endregion
    }
}
