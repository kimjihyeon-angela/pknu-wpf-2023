using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using wp11_movieFinder.Logics;
using wp11_movieFinder.Models;

namespace wp11_movieFinder
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        bool isFavorite = false;
        // false -> OpenAPI로 검색해온 결과,
        // true -> 즐겨찾기에서 출력된 결과
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtMovieName.Focus();
        }

        #region < 네이버 영화 버튼 클릭 -> 삭제함 >
        /*
        // 네이버 영화 버튼 클릭
        private async void BtnNaverMovie_Click(object sender, RoutedEventArgs e)
        {
            await Commons.ShowMessageAsync("네이버 영화", "네이버 영화사이트로 이동 합니다");
        }
        */
        #endregion

        #region < 검색버튼 이벤트 -> 실제 검색 이벤트에서 검색 >
        // 검색버튼, TMDB API 사용하여 영화 검색
        private async void BtnSearchMovie_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtMovieName.Text))
            {
                await Commons.ShowMessageAsync("검색", "검색할 영화명을 입력하세요.");
                return;
            }

            //if (TxtMovieName.Text.Length <= 2)
            //{
            //    await Commons.ShowMessageAsync("검색", "검색어를 2글자 이상 입력하세요.");
            //    return;
            //}

            try
            {
                SearchMovie(TxtMovieName.Text);
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"오류발생 : {ex.Message}");
            }
        }
        #endregion

        #region < 텍스트박스에서 키 입력 후 (엔터입력 시 검색 이벤트 발생) >
        // 텍스트박스에서 키 입력 후 
        private void TxtMovieName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearchMovie_Click(sender, e);
            }
        }
        #endregion

        #region < 실제 검색 메서드 >
        // 실제 검색 메서드
        private async void SearchMovie(string movieName)
        {
            string tmdb_apikey = "tmdb키 입력";
            string encoding_movieName = HttpUtility.UrlEncode(movieName, Encoding.UTF8);
            // 영화 검색 URL
            string openApiUri = $@"https://api.themoviedb.org/3/search/movie?api_key={tmdb_apikey}&language=ko-KR&page=1&include_adult=false&query={encoding_movieName}";

            // 결과값 초기화
            string result = string.Empty;

            // api 실행할 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            // TMDB API 요청
            try
            {
                req = WebRequest.Create(openApiUri); // URL을 넣어서 객체 생성
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

            // result를 json으로 변경
            var jsonResult = JObject.Parse(result); // string -> json

            var total = Convert.ToInt32(jsonResult["total_results"]); // 전체 검색 결과 수

            //await Commons.ShowMessageAsync("검색결과", total.ToString());

            var items = jsonResult["results"];

            //items를 데이터그리드에 표시
            var json_array = items as JArray;

            var movieItems = new List<MovieItem>(); // json에서 넘어온 배열 값을 담을 장소
            foreach (var val in json_array)
            {
                var MovieItem = new MovieItem()
                {
                    Adult = Convert.ToBoolean(val["adult"]),
                    Id = Convert.ToInt32(val["id"]),
                    Original_Language = Convert.ToString(val["original_language"]),
                    Original_Title = Convert.ToString(val["original_title"]),
                    Popularity = Convert.ToDouble(val["popularity"]),
                    Overview = Convert.ToString(val["overview"]),
                    Poster_Path = Convert.ToString(val["poster_path"]),
                    Release_Date = Convert.ToString(val["release_date"]),
                    Title = Convert.ToString(val["title"]),
                    Vote_Average = Convert.ToDouble(val["vote_average"])

                };
                movieItems.Add(MovieItem);
            }
            this.DataContext = movieItems;
            isFavorite = false; // 즐겨찾기에서 온 결과가 아니라는 표시
            StsResult.Content = $"OpenAPI {movieItems.Count} 건 조회완료";
        }
        #endregion

        #region < 그리드 셀 선택 시 이벤트 - 포스터 보기(OpenApi, DB) >
        // 그리드에서 셀 선택 시 발생하는 이벤트
        private async void GrdResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                string posterPath = string.Empty;

                // Open API로 검색된 영화의 포스터 보기
                if (GrdResult.SelectedItem is MovieItem)
                {
                    var movie = GrdResult.SelectedItem as MovieItem;
                    posterPath = movie.Poster_Path;
                }
                // 즐겨찾기(DB)에 등록된 영화의 포스터 보기
                else if (GrdResult.SelectedItem is FavoriteMovieItem)
                {
                    var movie = GrdResult.SelectedItem as FavoriteMovieItem;
                    posterPath = movie.Poster_Path;
                }

                Debug.WriteLine(posterPath);
                // 포스터 이미지 없을 경우 No_Picture
                if (string.IsNullOrEmpty(posterPath))
                {
                    ImgPoster.Source = new BitmapImage(new Uri("No_Picture.png", UriKind.RelativeOrAbsolute));
                }
                // 포스터 이미지 경로가 있을 경우 영화의 이미지
                else
                {
                    var base_url = "https://image.tmdb.org/t/p/w300_and_h450_bestv2";
                    ImgPoster.Source = new BitmapImage(new Uri($"{base_url}{posterPath}", UriKind.RelativeOrAbsolute));
                }
            }
            catch (Exception)
            {
                await Commons.ShowMessageAsync("오류", $"이미지로드 오류 발생");
            }

        }
        #endregion

        #region < 영화 예고편 보기 >
        // 영화 예고편 유튜브 보기
        private async void BtnWatchTrailer_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("유튜브", "영화를 선택해주세요.");
                return;
            }
            if (GrdResult.SelectedItems.Count > 1)
            {
                await Commons.ShowMessageAsync("유튜브", "영화를 하나만 선택해주세요.");
                return;
            }

            string movieName = string.Empty;
            //var movie = GrdResult.SelectedItem as MovieItem;

            if (GrdResult.SelectedItem is MovieItem)
            {
                var movie = GrdResult.SelectedItem as MovieItem;
                movieName = movie.Title;
            }
            else if (GrdResult.SelectedItem is FavoriteMovieItem)
            {
                var movie = GrdResult.SelectedItem as FavoriteMovieItem;
                movieName = movie.Title;
            }

            // await Commons.ShowMessageAsync("유튜브", $"예고편 볼 영화 {movieName}");

            //var trailerWindow = new TrailerWindow(movie); 
            var trailerWindow = new TrailerWindow(movieName);
            // 부모창에서 자식창으로 내용 넘길때 생성자에 넣어줘야함 (클래스, 스트링 다 넘길 수 있음)
            trailerWindow.Owner = this; // TrailerWindow의 부모 => MainWindow
            trailerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner; // 부모창 정중앙에 위치
            // trailerWindow.Show(); // 모달리스로 부모창 선택 가능해짐
            trailerWindow.ShowDialog(); // 모달창으로 부모창 선택 불가능
        }
        #endregion

        #region <즐겨찾기 저장 이벤트 >
        // 즐겨찾기 저장 이벤트
        private async void BtnAddFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기 등록할 영화를 선택해주세요(복수 선택 가능");
                return;
            }

            if (isFavorite)
            {
                await Commons.ShowMessageAsync("오류", "이미 즐겨찾기에 등록된 영화입니다.");
                return;
            }

            #region < 리스트 만들기 -> 필요 없음,, ㅎ>
            /*
            List<FavoriteMovieItem> list = new List<FavoriteMovieItem>();
            foreach (MovieItem item in GrdResult.SelectedItems)
            {
                var favoriteMovie = new FavoriteMovieItem
                {
                    Id = item.Id,
                    Title = item.Title,
                    Original_Title = item.Original_Title,
                    Original_Language = item.Original_Language,
                    Adult = item.Adult,
                    Overview = item.Overview,
                    Release_Date = item.Release_Date,
                    Vote_Average = item.Vote_Average,
                    Popularity = item.Popularity,
                    Poster_Path = item.Poster_Path,
                    Reg_Date = DateTime.Now // 저장하는 일시 (지금)
                };

                list.Add(favoriteMovie);
            }
            */
            #endregion

            //await Commons.ShowMessageAsync("저장할 데이터 수", list.Count.ToString());
            //return;
            #region < MySQL DB 데이터 테스트용 >
            /*
            try
            {
                // MySQL DB 데이터 INSERT(테스트용)
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var query = @"INSERT INTO FavoriteMovieItem
                                            ( Id
                                            , Title
                                            , Original_Title
                                            , Release_Date
                                            , Original_Language
                                            , Adult
                                            , Popularity
                                            , Vote_Average
                                            , Poster_Path
                                            , Overview
                                            , Reg_Date )
                                       VALUES
                                            ( @Id
                                            , @Title
                                            , @Original_Title
                                            , @Release_Date
                                            , @Original_Language
                                            , @Adult
                                            , @Popularity
                                            , @Vote_Average
                                            , @Poster_Path
                                            , @Overview
                                            , @Reg_Date )";

                    var insRes = 0;

                    foreach (FavoriteMovieItem item in list)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        //SqlParameter prmId = new SqlParameter("@Id", item.Id);
                        //cmd.Parameters.Add(prmId);
                        cmd.Parameters.AddWithValue("@Id", item.Id);
                        cmd.Parameters.AddWithValue("@Title", item.Title);
                        cmd.Parameters.AddWithValue("@Original_Title", item.Original_Title);
                        cmd.Parameters.AddWithValue("@Original_Language", item.Original_Language);
                        cmd.Parameters.AddWithValue("@Release_Date", item.Release_Date);
                        cmd.Parameters.AddWithValue("@Adult", item.Adult);
                        cmd.Parameters.AddWithValue("@Popularity", item.Popularity);
                        cmd.Parameters.AddWithValue("@Vote_Average", item.Vote_Average);
                        cmd.Parameters.AddWithValue("@Poster_Path", item.Poster_Path);
                        cmd.Parameters.AddWithValue("@Overview", item.Overview);
                        cmd.Parameters.AddWithValue("@Reg_Date", item.Reg_Date);

                        insRes += cmd.ExecuteNonQuery();
                    }
                }
            }


            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB저장 오류 : {ex.Message}");
            }
            */
            #endregion

            #region < SQL Server DB INSERT >
            try
            {
                // DB 데이터 INSERT(입력)
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var query = @"INSERT INTO [dbo].[FavoriteMovieItem]
                                            ( [Id]
                                            , [Title]
                                            , [Original_Title]
                                            , [Release_Date]
                                            , [Original_Language]
                                            , [Adult]
                                            , [Popularity]
                                            , [Vote_Average]
                                            , [Poster_Path]
                                            , [Overview]
                                            , [Reg_Date] )
                                       VALUES
                                            ( @Id
                                            , @Title
                                            , @Original_Title
                                            , @Release_Date
                                            , @Original_Language
                                            , @Adult
                                            , @Popularity
                                            , @Vote_Average
                                            , @Poster_Path
                                            , @Overview
                                            , @Reg_Date )";


                    var insRes = 0;
                    foreach (MovieItem item in GrdResult.SelectedItems)
                    // openAPI로 조회된 결과이기 때문에 FavoriteMovieItem이 아닌 MovieItem써야함
                    {
                        SqlCommand cmd = new SqlCommand(query, conn);
                        //SqlParameter prmId = new SqlParameter("@Id", item.Id);
                        //cmd.Parameters.Add(prmId);
                        cmd.Parameters.AddWithValue("@Id", item.Id);
                        cmd.Parameters.AddWithValue("@Title", item.Title);
                        cmd.Parameters.AddWithValue("@Original_Title", item.Original_Title);
                        cmd.Parameters.AddWithValue("@Original_Language", item.Original_Language);
                        cmd.Parameters.AddWithValue("@Release_Date", item.Release_Date);
                        cmd.Parameters.AddWithValue("@Adult", item.Adult);
                        cmd.Parameters.AddWithValue("@Popularity", item.Popularity);
                        cmd.Parameters.AddWithValue("@Vote_Average", item.Vote_Average);
                        cmd.Parameters.AddWithValue("@Poster_Path", item.Poster_Path);
                        cmd.Parameters.AddWithValue("@Overview", item.Overview);
                        cmd.Parameters.AddWithValue("@Reg_Date", DateTime.Now);

                        insRes += cmd.ExecuteNonQuery();
                    }

                    if (GrdResult.SelectedItems.Count == insRes)
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장 성공");
                        StsResult.Content = $"즐겨찾기 {insRes} 건 저장완료";
                    }
                    else
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장 실패, 관리자에게 문의하세요.");

                    }
                    //MessageBox.Show(insRes.ToString());
                    // await Commons.ShowMessageAsync("데이터갯수", result.ToString()); // DB연결 확인용
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB저장 오류 : {ex.Message}");
            }
        }
        #endregion

        #endregion

        #region < 즐겨찾기 보기 이벤트 >
        private async void BtnViewFavorite_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            TxtMovieName.Text = string.Empty;

            List<FavoriteMovieItem> list = new List<FavoriteMovieItem>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var query = @"SELECT Id
                                     , Title
                                     , Original_Title
                                     , Release_Date
                                     , Original_Language
                                     , Adult
                                     , Popularity
                                     , Vote_Average
                                     , Poster_Path
                                     , Overview
                                     , Reg_Date
                                  FROM FavoriteMovieItem
                                 ORDER BY Id DESC";
                    var cmd = new SqlCommand(query, conn);
                    var adapter = new SqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "FavoriteMovieItem");

                    foreach (DataRow dr in dSet.Tables["FavoriteMovieItem"].Rows)
                    {
                        list.Add(new FavoriteMovieItem
                        {
                            Id = Convert.ToInt32(dr["ID"]),
                            Title = Convert.ToString(dr["Title"]),
                            Original_Title = Convert.ToString(dr["Original_Title"]),
                            Release_Date = Convert.ToString(dr["Release_Date"]),
                            Original_Language = Convert.ToString(dr["Original_Language"]),
                            Adult = Convert.ToBoolean(dr["Adult"]),
                            Popularity = Convert.ToDouble(dr["Popularity"]),
                            Vote_Average = Convert.ToDouble(dr["Vote_Average"]),
                            Poster_Path = Convert.ToString(dr["Poster_Path"]),
                            Overview = Convert.ToString(dr["Overview"]),
                            Reg_Date = Convert.ToDateTime(dr["Reg_Date"])
                        });
                    }

                    this.DataContext = list;

                    isFavorite = true; // 즐겨찾기에서 가져온 내용임을 표시
                    StsResult.Content = $"즐겨찾기 {list.Count} 건 조회완료";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB조회 오류 {ex.Message}");
            }


        }
        #endregion

        #region < 즐겨찾기 삭제 이벤트 >
        private async void BtnDelFavorite_Click(object sender, RoutedEventArgs e)
        {
            // OpenApi로 검색해 온 결과로 삭제 불가
            if (isFavorite == false)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기만 삭제할 수 있습니다.");
                return;
            }

            // 아무것도 선택되지 않음
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "삭제할 영화를 선택해주세요");
                return;
            }

            // DB 삭제
            try
            {
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    var query = "DELETE FROM FavoriteMovieItem WHERE Id = @Id";
                    var delRes = 0;

                    foreach (FavoriteMovieItem item in GrdResult.SelectedItems)
                    {
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", item.Id);

                        delRes += cmd.ExecuteNonQuery();
                    }

                    if (delRes == GrdResult.SelectedItems.Count)
                    {
                        await Commons.ShowMessageAsync("삭제", "선택한 데이터가 삭제되었습니다.");
                        StsResult.Content = $"즐겨찾기 {delRes} 건 삭제완료";
                    }
                    else
                    {
                        await Commons.ShowMessageAsync("삭제", "선택한 데이터 중 일부 삭제되었습니다.");
                    }
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB 삭제 오류 {ex.Message}");
            }

            // 즐겨찾기 보기 이벤트 핸들러를 한번 실행
            BtnViewFavorite_Click(sender, e);
        }
        #endregion
    }
}
