using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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
using Wp12_finedustCheck.Logics;
using Wp12_finedustCheck.Models;

namespace Wp12_finedustCheck
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

        #region < 화면 로딩 될 때 발생 이벤트 - 콤보박스에 DB(날짜) 넣기>
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 콤보박스에 들어갈 날짜 DB에서 조회하여 출력하기
            // ** DB에 저장한 후에도 콤보박스 재조회하여 날짜 출력해야함
            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                conn.Open();
                var query = @"SELECT date_format(timestamp, '%Y-%m-%d') AS 'Save_Date' 
                                FROM dustsensor
                               GROUP BY 1
                               ORDER BY 1";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                List<string> saveDateList = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    saveDateList.Add(Convert.ToString(row["save_date"]));
                }

                CboReqDate.ItemsSource = saveDateList;
            }
        }
        #endregion

        #region < 실시간 조회 버튼 이벤트 (김해시 OpenAPI 조회) >
        private async void BtnReqRealtime_Click(object sender, RoutedEventArgs e)
        {
            string openApiUri = "https://smartcity.gimhae.go.kr/smart_tour/dashboard/api/publicData/dustSensor";
            string result = string.Empty;

            // webRequest, webResponse 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUri);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();
                
                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"OpenAPI 조회 오류 {ex.Message}");
            }

            var jsonResult = JObject.Parse(result);
            var status = Convert.ToInt32(jsonResult["status"]);
            // 정상(200)인 경우 데이터 처리
            try
            {
                if (status == 200)
                {
                    var data = jsonResult["data"];
                    var json_array = data as JArray;


                    var dustSensors = new List<DustSensor>();
                    foreach (var sensor in json_array)
                    {
                        dustSensors.Add(new DustSensor
                        {
                            Id = 0,
                            Dev_id = Convert.ToString(sensor["dev_id"]),
                            Name = Convert.ToString(sensor["name"]),
                            Loc = Convert.ToString(sensor["loc"]),
                            Coordx = Convert.ToDouble(sensor["coordx"]),
                            Coordy = Convert.ToDouble(sensor["coordy"]),
                            Ison = Convert.ToBoolean(sensor["ison"]),
                            Pm10_after = Convert.ToInt32(sensor["pm10_after"]),
                            Pm25_after = Convert.ToInt32(sensor["pm25_after"]),
                            State = Convert.ToInt32(sensor["state"]),
                            Timestamp = Convert.ToDateTime(sensor["timestamp"]),
                            Company_id = Convert.ToString(sensor["company_id"]),
                            Company_name = Convert.ToString(sensor["company_name"])
                        });
                    }

                    this.DataContext = dustSensors;
                    StsResult.Content = $"OpenApi {dustSensors.Count} 건 조회 완료";
                }

            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"Json 데이터 처리 오류 {ex.Message}");
            }

        }
        #endregion

        #region < 저장 버튼 이벤트 (검색 결과 DB (MySQL)에 저장)>
        private async void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.Items.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "조회하기 선택후 저장하세요");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var query = @"INSERT INTO miniproject.dustsensor
                                            ( Dev_id,
                                              Name,
                                              Loc,
                                              Coordx,
                                              Coordy,
                                              Ison,
                                              Pm10_after,
                                              Pm25_after,
                                              State,
                                              Timestamp,
                                              Company_id,
                                              Company_name)
                                       VALUES
                                            ( @Dev_id,
                                              @Name,
                                              @Loc,
                                              @Coordx,
                                              @Coordy,
                                              @Ison,
                                              @Pm10_after,
                                              @Pm25_after,
                                              @State,
                                              @Timestamp,
                                              @Company_id,
                                              @Company_name)";
                    var insRes = 0;

                    foreach (var temp in GrdResult.Items)
                    {
                        if (temp is DustSensor)
                        {
                            var item = temp as DustSensor;
                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@Dev_id", item.Dev_id);
                            cmd.Parameters.AddWithValue("@Name", item.Name);
                            cmd.Parameters.AddWithValue("@Loc", item.Loc);
                            cmd.Parameters.AddWithValue("@Coordx", item.Coordx);
                            cmd.Parameters.AddWithValue("@Coordy", item.Coordy);
                            cmd.Parameters.AddWithValue("@Ison", item.Ison);
                            cmd.Parameters.AddWithValue("@Pm10_after", item.Pm10_after);
                            cmd.Parameters.AddWithValue("@Pm25_after", item.Pm25_after);
                            cmd.Parameters.AddWithValue("@State", item.State);
                            cmd.Parameters.AddWithValue("@Timestamp", item.Timestamp);
                            cmd.Parameters.AddWithValue("@Company_id", item.Company_id);
                            cmd.Parameters.AddWithValue("@Company_name", item.Company_name);

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

        #region < 콤보박스 검색 날짜 선택 이벤트 (MySQL에서 조회 후 리스트 출력) >
        private void CboReqDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 콤보박스에 선택된 내용이 없을 경우 (선택했다가 취소버튼 눌렀을 경우)
            if (CboReqDate.SelectedValue != null)
            {
                // 콤보박스에 선택된 내용이 맞는지 테스트
                // MessageBox.Show(CboReqDate.SelectedValue.ToString());
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"SELECT Id
                                       , Dev_id
                                       , Name
                                       , Loc
                                       , Coordx
                                       , Coordy
                                       , Ison
                                       , Pm10_after
                                       , Pm25_after
                                       , State
                                       , Timestamp
                                       , Company_id
                                       , Company_name
                                    FROM dustsensor
                                   WHERE date_format(Timestamp, '%Y-%m-%d') = @Timestamp;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Timestamp", CboReqDate.SelectedValue.ToString());
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "dustsensor");
                    List<DustSensor> dustSensors = new List<DustSensor>();

                    foreach (DataRow row in ds.Tables["dustsensor"].Rows)
                    {
                        dustSensors.Add(new DustSensor()
                        {
                            // MySQL의 경우 컬럼이름에 대소문자 구분 x 꼭 컬럼과 같을 필요 x
                            // But, 가능하면 맞춰주기 (다른 SQL은 구분하기 때문)
                            Id = Convert.ToInt32(row["Id"]), 
                            Dev_id = Convert.ToString(row["Dev_id"]),
                            Name = Convert.ToString(row["Name"]),
                            Loc = Convert.ToString(row["Loc"]),
                            Coordx = Convert.ToDouble(row["Coordx"]),
                            Coordy = Convert.ToDouble(row["Coordy"]),
                            Ison = Convert.ToBoolean(row["Ison"]),
                            Pm10_after = Convert.ToInt32(row["Pm10_after"]),
                            Pm25_after = Convert.ToInt32(row["Pm25_after"]),
                            State = Convert.ToInt32(row["State"]),
                            Timestamp = Convert.ToDateTime(row["Timestamp"]),
                            Company_id = Convert.ToString(row["Company_id"]),
                            Company_name = Convert.ToString(row["Company_name"])
                        });
                    }

                    this.DataContext = dustSensors;
                    StsResult.Content = $"DB {dustSensors.Count} 건 조회 완료";
                }
            }

            // 콤보박스 취소버튼 클릭시 dbGrid 내용 지우기
            else
            {
                this.DataContext = null;
                StsResult.Content = $"DB 조회 클리어";
            }
        }


        #endregion

        #region < 그리드 특정 로우 더블클릭 시 지도(센서위치)창 띄우기 >
        private void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selItem = GrdResult.SelectedItem as DustSensor;

            var mapWindow = new MapWindow(selItem.Coordy, selItem.Coordx); 
            // 부모창 위치값을 자식창으로 전달
            mapWindow.Owner = this; // 메인윈도우가 부모임을 의미함
            mapWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mapWindow.ShowDialog();

        }
        #endregion
    }
}
