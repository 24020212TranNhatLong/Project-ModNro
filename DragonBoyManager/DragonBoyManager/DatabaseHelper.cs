using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace DragonBoyManager
{
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string Type { get; set; }
        public string ToaDoX { get; set; }
        public string ToaDoY { get; set; }
        public string Map { get; set; }
        public string Khu { get; set; }
        public bool GioiHan149tr { get; set; }
        public bool GioiHan1ty49 { get; set; }
        public bool AutoCoden { get; set; }
    }

    class DatabaseHelper
    {
        private static string _connectionString = "Data Source=account.db;Version=3;";

        public static void InitializeDatabase()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"CREATE TABLE IF NOT EXISTS Accounts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL,
                    Password TEXT NOT NULL,
                    Server TEXT NOT NULL,
                    Type TEXT,
                    ToaDoX TEXT,
                    ToaDoY TEXT,
                    Map TEXT,
                    Khu TEXT,
                    GioiHan149tr BOOLEAN,
                    GioiHan1ty49 BOOLEAN,
                    AutoCoden BOOLEAN
                )";

                new SQLiteCommand(sql, conn).ExecuteNonQuery();
            }
        }

        public static void AddAccount(Account acc)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Accounts 
                    (Username, Password, Server, Type, ToaDoX, ToaDoY, Map, Khu, GioiHan149tr, GioiHan1ty49, AutoCoden) 
                    VALUES 
                    (@u, @p, @s, @t, @x, @y, @m, @k, @limit149tr, @limit1ty49, @c)";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", acc.Username);
                    cmd.Parameters.AddWithValue("@p", acc.Password);
                    cmd.Parameters.AddWithValue("@s", acc.Server);
                    cmd.Parameters.AddWithValue("@t", acc.Type);
                    cmd.Parameters.AddWithValue("@x", acc.ToaDoX);
                    cmd.Parameters.AddWithValue("@y", acc.ToaDoY);
                    cmd.Parameters.AddWithValue("@m", acc.Map);
                    cmd.Parameters.AddWithValue("@k", acc.Khu);
                    cmd.Parameters.AddWithValue("@limit149tr", acc.GioiHan149tr ? 1 : 0);
                    cmd.Parameters.AddWithValue("@limit1ty49", acc.GioiHan1ty49 ? 1 : 0);
                    cmd.Parameters.AddWithValue("@c", acc.AutoCoden ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<Account> GetAllAccounts()
        {
            var list = new List<Account>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Accounts";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Account
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Username = reader["Username"].ToString(),
                            Password = reader["Password"].ToString(),
                            Server = reader["Server"].ToString(),
                            Type = reader["Type"]?.ToString(),
                            ToaDoX = reader["ToaDoX"]?.ToString(),
                            ToaDoY = reader["ToaDoY"]?.ToString(),
                            Map = reader["Map"]?.ToString(),
                            Khu = reader["Khu"]?.ToString(),
                            GioiHan149tr = Convert.ToInt32(reader["GioiHan149tr"]) != 0,
                            GioiHan1ty49 = Convert.ToInt32(reader["GioiHan1ty49"]) != 0,
                            AutoCoden = Convert.ToInt32(reader["AutoCoden"]) != 0
                        });
                    }
                }
            }
            return list;
        }

        public static void DeleteAccount(int id)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Accounts WHERE Id = @id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateAccount(Account acc)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"UPDATE Accounts 
                    SET Username=@u, Password=@p, Server=@s, Type=@t,
                        ToaDoX=@x, ToaDoY=@y, Map=@m, Khu=@k,
                        GioiHan149tr=@limit149tr, GioiHan1ty49=@limit1ty49, AutoCoden=@c
                    WHERE Id=@id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", acc.Username);
                    cmd.Parameters.AddWithValue("@p", acc.Password);
                    cmd.Parameters.AddWithValue("@s", acc.Server);
                    cmd.Parameters.AddWithValue("@t", acc.Type);
                    cmd.Parameters.AddWithValue("@x", acc.ToaDoX);
                    cmd.Parameters.AddWithValue("@y", acc.ToaDoY);
                    cmd.Parameters.AddWithValue("@m", acc.Map);
                    cmd.Parameters.AddWithValue("@k", acc.Khu);
                    cmd.Parameters.AddWithValue("@limit149tr", acc.GioiHan149tr ? 1 : 0);
                    cmd.Parameters.AddWithValue("@limit1ty49", acc.GioiHan1ty49 ? 1 : 0);
                    cmd.Parameters.AddWithValue("@c", acc.AutoCoden ? 1 : 0);
                    cmd.Parameters.AddWithValue("@id", acc.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

 

    public static Account GetAccountById(int id)
        {
            // Tạo kết nối đến database
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();

                // Câu lệnh SQL lấy tài khoản dựa trên ID
                string query = "SELECT * FROM Accounts WHERE Id = @id";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    // Thêm tham số vào câu lệnh SQL để tránh SQL injection
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())  // Nếu có kết quả
                        {
                            // Trả về đối tượng Account
                            Account acc = new Account
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Username = reader["Username"].ToString(),
                                Password = reader["Password"].ToString(),
                                Server = reader["Server"].ToString(),
                                Type = reader["Type"].ToString(),
                                ToaDoX = reader["ToaDoX"].ToString(),
                                ToaDoY = reader["ToaDoY"].ToString(),
                                Map = reader["Map"].ToString(),
                                Khu = reader["Khu"].ToString(),
                                GioiHan149tr = Convert.ToBoolean(reader["GioiHan149tr"]),
                                GioiHan1ty49 = Convert.ToBoolean(reader["GioiHan1ty49"]),
                                AutoCoden = Convert.ToBoolean(reader["AutoCoden"])
                            };
                            return acc;
                        }
                        else
                        {
                            return null;  // Không tìm thấy tài khoản
                        }
                    }
                }
            }
        }
    }
}
