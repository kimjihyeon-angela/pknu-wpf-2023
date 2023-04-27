﻿using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows;

namespace wp11_movieFinder.Logics
{
    public class Commons
    {
        // 연결 문자열 담을 변수(SQL Server)
        public static readonly string connString = "Data Source=localhost;" +
                                                   "Initial Catalog=pknu;" +
                                                   "Persist Security Info=True;" +
                                                   "User ID=sa;Password=12345";

        // 연결 문자열 담을 변수(MySQL)
        public static readonly string myConnString = "Server=localhost;" +
                                                     "Port=3306;" +
                                                     "Database=miniproject;" +
                                                     "Uid=root;" +
                                                     "Pwd=12345;";

        // 메트로 다이얼로그창을 위한 정적 메서드
        public static async Task<MessageDialogResult> ShowMessageAsync(string title, string message, 
            MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            return await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync(title, message, style, null);
        }
    }
}
