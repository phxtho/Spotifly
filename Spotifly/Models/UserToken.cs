using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Dapper;

namespace Spotifly.Models
{
    public class UserToken
    {
        public Int64 Id { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime DateCreated { get; set; }

        public static UserToken SelectToken(Int64 id)
        {
            UserToken userToken = null;
            string sql = "SELECT id, access_token AccessToken, refresh_token RefreshToken, date_created DateCreated FROM user_token WHERE id = @Id";

            using (MySqlConnection conn = SpotiflyDB.NewConnection())
            {
                userToken = conn.QueryFirst<UserToken>(sql, new {Id = id});
            }
            return userToken;
        }

        public static UserToken InsertToken(Int64 id, string accessToken, string refreshToken, DateTime dateCreated)
        {
            UserToken userToken = null;
            string insert = "INSERT INTO user_token (id, access_token, refresh_token, date_created) VALUES (@Id, @AccessToken, @RefreshToken, @DateCreated)";
            string select = "SELECT id, access_token AccessToken, refresh_token RefreshToken, date_created DateCreated FROM user_token WHERE id = @Id";

            using (MySqlConnection conn = SpotiflyDB.NewConnection())
            {
                var affectedRows = conn.Execute(insert, new { Id = id, AccessToken = accessToken, RefreshToken = refreshToken, DateCreated = dateCreated });
                if (1 == affectedRows)
                    userToken = conn.QueryFirst<UserToken>(select, new { Id = id });
            }
            return userToken;
        }

        public static UserToken UpdateToken(Int64 id, string accessToken, string refreshToken, DateTime dateCreated)
        {
            UserToken userToken = null;
            string update = "UPDATE user_token set access_token = @AccessToken, refresh_token = @RefreshToken, date_created = @DateCreated WHERE id = @Id";
            string select = "SELECT id, access_token AccessToken, refresh_token RefreshToken, date_created DateCreated FROM user_token WHERE id = @Id";

            using (MySqlConnection conn = SpotiflyDB.NewConnection())
            {
                var affectedRows = conn.Execute(update, new { Id = id, AccessToken = accessToken, RefreshToken = refreshToken, DateCreated = dateCreated });
                if (1 == affectedRows)
                    userToken = conn.QueryFirst<UserToken>(select, new { Id = id });
            }
            return userToken;
        }

        public static void DeleteToken(Int64 id)
        {
            string sql = "DELETE FROM user_token WHERE id = @Id";

            using (MySqlConnection conn = SpotiflyDB.NewConnection())
            {
                var affectedRows = conn.Execute(sql, new { Id = id });
            }
        }
    }
}