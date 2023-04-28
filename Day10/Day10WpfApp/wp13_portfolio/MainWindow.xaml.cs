using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
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
using wp13_portfolio.Logics;
using wp13_portfolio.Models;

namespace wp13_portfolio
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

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SearchHospital();// 페이지 로드될 때 바로 정보 찾아오기
            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                conn.Open();
                var query = @"SELECT DISTINCT Gugun 
                                FROM animalhospital
                               ORDER BY ASC";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                List<string> saveDateList = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    saveDateList.Add(Convert.ToString(row["Gugun"]));
                }

                CboReqDate.ItemsSource = saveDateList;
            }
        }

        private async void SearchHospital()
        {
            string apikey = "4Ng%2FJiC8rO2xbjXzST7yzTCSfv2%2Bw57zIYoXivUfh1VIpBkQGln74kS5%2FyQpCMoANkXUJXlHWBEqNUrhHMo3zQ%3D%3D";
            // url (동물병원 정보 검색)
            string url = $@"https://apis.data.go.kr/6260000/BusanAnimalHospService/getTblAnimalHospital?serviceKey={apikey}&pageNo=1&numOfRows=10000&resultType=json";


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
            var resultCode = Convert.ToString(jsonResult["getTblAnimalHospital"]["header"]["resultCode"]);

           

            try
            {
                if (resultCode == "00")
                {
                    var item = jsonResult["getTblAnimalHospital"]["body"]["items"]["item"];
                    var json_array = item as JArray;

                    var hospitalInfo = new List<HospitalInfo>();

                    

                    foreach (var hosInfo in json_array)
                    {
                        if ((Convert.ToString(hosInfo["lat"]) != "") && (Convert.ToString(hosInfo["animal_hospital"])!=""))
                        {
                            hospitalInfo.Add(new HospitalInfo
                            {
                                Id = 0,
                                Gugun = Convert.ToString(hosInfo["gugun"]),
                                Animal_Hospital = Convert.ToString(hosInfo["animal_hospital"]),
                                Approval = Convert.ToString(hosInfo["approval"]),
                                Road_Address = Convert.ToString(hosInfo["road_address"]),
                                Tel = Convert.ToString(hosInfo["tel"]),
                                Lat = Convert.ToDouble(hosInfo["lat"]),
                                Lon = Convert.ToDouble(hosInfo["lon"]),
                                Basic_Date = Convert.ToString(hosInfo["basic_date"])
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

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchHospital();
        }

        private async void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var query = @"INSERT INTO animalhospital
                                        (
                                              Gugun,
                                              Animal_Hospital,
                                              Approval,
                                              Road_Address,
                                              Tel,
                                              Lat,
                                              Lon,
                                              Basic_Date)
                                      VALUES  
                                              (
                                              @Gugun,
                                              @Animal_Hospital,
                                              @Approval,
                                              @Road_Address,
                                              @Tel,
                                              @Lat,
                                              @Lon,
                                              @Basic_Date
                                              );";
                    var insRes = 0;

                    foreach (var temp in GrdResult.Items)
                    {
                        if (temp is HospitalInfo)
                        {
                            var item = temp as HospitalInfo;
                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@Gugun", item.Gugun);
                            cmd.Parameters.AddWithValue("@Animal_Hospital", item.Animal_Hospital);
                            cmd.Parameters.AddWithValue("@Approval", item.Approval);
                            cmd.Parameters.AddWithValue("@Road_Address", item.Road_Address);
                            cmd.Parameters.AddWithValue("@Tel", item.Tel);
                            cmd.Parameters.AddWithValue("@Lat", item.Lat);
                            cmd.Parameters.AddWithValue("@Lon", item.Lon);
                            cmd.Parameters.AddWithValue("@Basic_Date", item.Basic_Date);

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
    }
}
