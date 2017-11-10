using Discord;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RicaBotpaw.Modules.Data
{
	public class Database
	{
		private string table { get; set; }
		private const string server = "YOUR IP HERE";
		private const string database = "YOUR DATABASE HERE";
		private const string username = "YOUR USERNAME HERE";
		private const string password = "YOUR PASSWORD HERE";
		private MySqlConnection dbConnection;

		public Database(string table)
		{
			this.table = table;
			MySqlConnectionStringBuilder stringBuilder = new MySqlConnectionStringBuilder();
			stringBuilder.Server = server;
			stringBuilder.UserID = username;
			stringBuilder.Password = password;
			stringBuilder.Database = database;
			stringBuilder.SslMode = MySqlSslMode.None;

			var connectionString = stringBuilder.ToString();
			dbConnection = new MySqlConnection(connectionString);
			dbConnection.Open();
		}

		public MySqlDataReader FireCommand(string query)
		{
			if (dbConnection == null)
			{
				return null;
			}

			MySqlCommand command = new MySqlCommand(query, dbConnection);

			var mySqlReader = command.ExecuteReader();

			return mySqlReader;
		}

		public void CloseConnection()
		{
			if (dbConnection != null)
			{
				dbConnection.Close();
			}
		}

		public static List<String> CheckExistingUser(IUser user)
		{
			var result = new List<String>();
			var database = new Database("YOUR DATABASE HERE");

			var str = string.Format("SELECT * FROM `YOUR TABLE HERE` WHERE user_id = '{0}'", user.Id);
			var tableName = database.FireCommand(str);

			while (tableName.Read())
			{
				var userId = (string)tableName["user_id"];

				result.Add(userId);
			}

			return result;
		}

		public static string EnterUser(IUser user)
		{
			var database = new Database("YOUR DATABASE HERE");
			var str = string.Format("INSERT INTO `YOUR TABLE HERE` (user_id, username, tokens, customRank, money, level, xp) VALUES ('{0}', '{1}', '100', '', '150', '1', '1')", user.Id, user.Username);
			var table = database.FireCommand(str);
			database.CloseConnection();
			return null;
		}

		public static List<YOUR tableName.cs HERE> GetUserStatus(IUser user)
		{
			var result = new List<YOUR tableName.cs HERE>();

			var database = new Database("YOUR DATABASE HERE");

			var str = string.Format("SELECT * FROM `YOUR TABLE HERE` WHERE user_id = '{0}'", user.Id);
			var tableName = database.FireCommand(str);

			while (tableName.Read())
			{
				var userId = (string)tableName["user_id"];
				var userName = (string)tableName["username"];
				var currentTokens = (uint)tableName["tokens"];
				var rank = (string)tableName["customRank"];
				var money = (int)tableName["money"];
				var level = (int)tableName["level"];
				var xp = (int)tableName["xp"];

				result.Add(new // tableName.cs
				{
					UserId = userId,
					Username = userName,
					Tokens = currentTokens,
					Rank = rank,
					Money = money,
					Level = level,
					XP = xp
				});
			}
			database.CloseConnection();

			return result;
		}

		public static void ChangeTokens(IUser user, uint tokens)
		{
			var database = new Database("YOUR DATABASE HERE");

			try
			{
				var strings = string.Format("UPDATE `YOUR TABLE HERE` SET tokens = tokens + '{0}' WHERE user_id = '{1}'", tokens, user.Id);
				var reader = database.FireCommand(strings);
				reader.Close();
				database.CloseConnection();
				return;
			}
			catch (Exception e)
			{
				database.CloseConnection();
				return;
			}
		}

		public static void addXP(IUser user, int xp)
		{
			var database = new Database("YOUR DATABASE HERE");

			try
			{
				var strings = $"UPDATE `YOUR TABLE HERE` SET xp = xp + {xp} where user_id = {user.Id.ToString()}";
				var reader = database.FireCommand(strings);
				reader.Close();
				database.CloseConnection();
				return;
			}
			catch (Exception e)
			{
				database.CloseConnection();
				return;
			}
		}

		public static void levelUp(IUser user, int xp)
		{
			var database = new Database("YOUR DATABASE HERE");

			try
			{
				var strings = $"UPDATE `YOUR TABLE HERE` SET level = level + {1}, xp = xp + {xp} WHERE user_id = {user.Id.ToString()}";
				var reader = database.FireCommand(strings);
				reader.Close();
				database.CloseConnection();
				return;
			}
			catch (Exception e)
			{
				database.CloseConnection();
				return;
			}
		}
	}
}