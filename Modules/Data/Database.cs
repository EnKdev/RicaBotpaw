using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Discord;
using MySql.Data.MySqlClient;

namespace RicaBotpaw.Modules.Data
{
	/// <summary>
	///     This is where the magic happens for the database
	/// </summary>
	public class Database
	{
		private const string server = "";
		private const string database = "";
		private const string username = "";
		private const string password = "";
		private readonly MySqlConnection dbConnection;

		/// <summary>
		///     This is the most important method, otherwise we won't have an connection
		/// </summary>
		/// <param name="table">The table.</param>
		public Database(string table)
		{
			this.table = table;
			var stringBuilder = new MySqlConnectionStringBuilder();
			stringBuilder.Server = server;
			stringBuilder.UserID = username;
			stringBuilder.Password = password;
			stringBuilder.Database = database;
			stringBuilder.SslMode = MySqlSslMode.None;
			stringBuilder.ConvertZeroDateTime = true;

			var connectionString = stringBuilder.ToString();
			dbConnection = new MySqlConnection(connectionString);
			dbConnection.Open();
		}

		private string table { get; }

		/// <summary>
		///     To fire a query to the DB
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns></returns>
		public MySqlDataReader FireCommand(string query)
		{
			if (dbConnection == null)
				return null;

			var command = new MySqlCommand(query, dbConnection);

			var mySqlReader = command.ExecuteReader();

			return mySqlReader;
		}

		/// <summary>
		///     When the connection isn't needed anymore
		/// </summary>
		public void CloseConnection()
		{
			if (dbConnection != null)
				dbConnection.Close();
		}

		/// <summary>
		///     Checks if the user is already existing inside the DB
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		public static List<string> CheckExistingUser(IUser user)
		{
			var result = new List<string>();

			var database = new Database("vampdb");

			var str = string.Format("SELECT * FROM `discord` WHERE user_id = '{0}'", user.Id);

			var tableName = database.FireCommand(str);

			while (tableName.Read())
			{
				var userId = (string) tableName["user_id"];

				result.Add(userId);
			}

			return result;
		}

		/// <summary>
		///     Checks the existing vote by user.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		/// <exception cref="Exception">You already voted on this poll</exception>
		public static List<string> CheckExistingVoteByUser(IUser user)
		{
			var result = new List<string>();
			var exists = 0; // Default. 0 = does not exist, 1 = does exist

			var database = new Database("vampdb");

			var str = string.Format("SELECT * FROM `pollvotes` WHERE user_id = '{0}'", user.Id);

			var tableName = database.FireCommand(str);

			while (tableName.Read())
			{
				var userId = (string) tableName["user_id"];
				result.Add(userId);

				if (result.Contains(user.Id.ToString()))
				{
					exists = 1;
					throw new Exception("You already voted on this poll");
				}
				exists = 0;
			}

			return result;
		}

		/// <summary>
		///     Checks if the user has opened a bank-account
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		public static List<string> CheckMoneyExistingUser(IUser user)
		{
			var result = new List<string>();

			var database = new Database("vampdb");

			var str = string.Format("SELECT * FROM `moneydiscord` WHERE user_id = '{0}'", user.Id);

			var tableName = database.FireCommand(str);

			while (tableName.Read())
			{
				var userId = (string) tableName["user_id"];

				result.Add(userId);
			}

			return result;
		}

		/// <summary>
		///     Checks for the users fursonas
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		public static List<string> CheckSonaExistingUser(IUser user)
		{
			var result = new List<string>();

			var database = new Database("vampdb");

			var str = string.Format("SELECT * FROM `sonatable` WHERE user_id = '{0}'", user.Id);

			var tableName = database.FireCommand(str);

			while (tableName.Read())
			{
				var userId = (string) tableName["user_id"];

				result.Add(userId);
			}

			return result;
		}

		/// <summary>
		///     If not existing, enters the user into the DB
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		public static string EnterUser(IUser user)
		{
			var database = new Database("vampdb");
			var str = string.Format(
				"INSERT INTO `discord` (user_id, username, tokens, customRank) VALUES ('{0}', '{1}', '100', '')",
				user.Id, user.Username);
			var table = database.FireCommand(str);
			database.CloseConnection();
			return null;
		}

		/// <summary>
		///     Gets the poll which has the current first id.
		/// </summary>
		/// <returns></returns>
		public static List<poll> GetPoll()
		{
			var database = new Database("vampdb");

			try
			{
				var result = new List<poll>();
				var str = "SELECT * FROM `poll` WHERE poll_id = \'1\'";
				var tableName = database.FireCommand(str);

				while (tableName.Read())
				{
					var question = (string) tableName["question"];
					var nQuestion = question.Replace("_", " ");
					var voteYes = (int) tableName["voteyes"];
					var voteNo = (int) tableName["voteno"];

					result.Add(new poll
					{
						Question = nQuestion,
						YesVotes = voteYes,
						NoVotes = voteNo
					});
				}
				database.CloseConnection();

				return result;
			}
			catch (Exception e)
			{
				database.CloseConnection();
				return null;
			}
		}

		/// <summary>
		///     Returns the users status
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		public static List<discord> GetUserStatus(IUser user)
		{
			var database = new Database("vampdb");
			try
			{
				var result = new List<discord>();
				var str = string.Format("SELECT * FROM `discord` WHERE user_id = '{0}'", user.Id);
				var tableName = database.FireCommand(str);

				while (tableName.Read())
				{
					var userId = (string) tableName["user_id"];
					var userName = (string) tableName["username"];
					var currentTokens = (uint) tableName["tokens"];
					var rank = (string) tableName["customRank"];
					var daily = (DateTime) tableName["daily"];

					result.Add(new discord
					{
						UserId = userId,
						Username = userName,
						Tokens = currentTokens,
						Rank = rank,
						Daily = daily
					});
				}
				database.CloseConnection();

				return result;
			}
			catch (Exception e)
			{
				database.CloseConnection();
				return null;
			}
		}

		/// <summary>
		///     When i award others with prestige tokens
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="tokens">The tokens.</param>
		public static void ChangeTokens(IUser user, uint tokens)
		{
			var database = new Database("vampdb");

			try
			{
				var strings = string.Format("UPDATE `discord` SET tokens = tokens + '{0}' WHERE user_id = '{1}'", tokens, user.Id);
				var reader = database.FireCommand(strings);
				reader.Close();
				database.CloseConnection();
			}
			catch (Exception e)
			{
				database.CloseConnection();
			}
		}

		/// <summary>
		///     Changes the daily.
		/// </summary>
		/// <param name="user">The user.</param>
		public static void ChangeDaily(IUser user)
		{
			var database = new Database("vampdb");
			try
			{
				var strings = string.Format($"UPDATE `discord` SET daily = curtime() WHERE user_id = '{user.Id}'");
				var reader = database.FireCommand(strings);
				reader.Close();
				database.CloseConnection();
			}
			catch (Exception e)
			{
				database.CloseConnection();
			}
		}

		// Money


		/// <summary>
		///     Money money money...
		/// </summary>
		/// <param name="user">The user.</param>
		public static void AddMoney(IUser user)
		{
			var database = new Database("vampdb");

			try
			{
				var moneyToAdd = 50;
				var strings = $"UPDATE `moneydiscord` SET money = money + {moneyToAdd} WHERE user_id = {user.Id}";
				var reader = database.FireCommand(strings);
				reader.Close();
				database.CloseConnection();
			}
			catch (Exception e)
			{
				database.CloseConnection();
			}
		}

		/// <summary>
		///     The second money add void for the daily command.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="amount">The amount.</param>
		public static void AddMoney2(IUser user, int amount)
		{
			var database = new Database("vampdb");

			try
			{
				var strings = $"UPDATE `moneydiscord` SET money = money + {amount} WHERE user_id = {user.Id}";
				var reader = database.FireCommand(strings);
				reader.Close();
				database.CloseConnection();
			}
			catch (Exception e)
			{
				database.CloseConnection();
			}
		}

		/// <summary>
		///     Opens your bank
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		public static string cBank(IUser user)
		{
			var database = new Database("vampdb");

			var str = string.Format("INSERT INTO `moneydiscord` (user_id, money, storeMoney) VALUES ('{0}', '150', '0')",
				user.Id);
			var table = database.FireCommand(str);

			database.CloseConnection();

			return null;
		}

		/// <summary>
		///     Gets your balance
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		public static List<moneydiscord> GetUserMoney(IUser user)
		{
			var result = new List<moneydiscord>();

			var database = new Database("vampdb");

			var str = string.Format("SELECT * FROM `moneydiscord` WHERE user_id = '{0}'", user.Id);
			var tableName = database.FireCommand(str);

			while (tableName.Read())
			{
				var userId = (string) tableName["user_id"];
				var money = (int) tableName["money"];
				var storeMoney = (int) tableName["storeMoney"];

				result.Add(new moneydiscord
				{
					UserId = userId,
					Money = money,
					StoreMoney = storeMoney
				});
			}
			database.CloseConnection();

			return result;
		}

		/// <summary>
		///     Updates your balance after being awarded
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="money">The money.</param>
		public static void UpdateMoney(IUser user, int money)
		{
			var database = new Database("vampdb");

			try
			{
				var strings = string.Format("UPDATE `moneydiscord` SET money = money + '{1}' WHERE user_id = '{0}'", user.Id,
					money);
				var reader = database.FireCommand(strings);
				reader.Close();
				database.CloseConnection();
			}
			catch (Exception e)
			{
				database.CloseConnection();
			}
		}

		/// <summary>
		///     Payment process Part 1
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="money">The money.</param>
		public static void StoreMoney(IUser user, int money)
		{
			var database = new Database("vampdb");

			try
			{
				var strings =
					$"UPDATE `moneydiscord` SET money - {money} WHERE user_id = {user.Id}";
				var reader = database.FireCommand(strings);
				reader.Close();
				database.CloseConnection();
			}
			catch (Exception e)
			{
				database.CloseConnection();
			}
		}

		/// <summary>
		///     Payment process part 2
		/// </summary>
		/// <param name="user1">The user1.</param>
		/// <param name="money">The money.</param>
		public static void PayMoney1(IUser user1, int money)
		{
			var database = new Database("vampdb");

			try
			{
				var strings = $"UPDATE `moneydiscord` SET storeMoney = storeMoney - {money} WHERE user_id = {user1.Id}";
				var reader = database.FireCommand(strings);
				reader.Close();
				database.CloseConnection();
			}
			catch (Exception e)
			{
				database.CloseConnection();
			}
		}

		/// <summary>
		///     Payment Process Part2/2
		/// </summary>
		/// <param name="user2">The user2.</param>
		/// <param name="money">The money.</param>
		public static void PayMoney2(IUser user2, int money)
		{
			var database = new Database("vampdb");

			try
			{
				var strings = $"UPDATE `moneydiscord` SET money = money + {money} WHERE user_id = {user2.Id}";
				var reader = database.FireCommand(strings);
				reader.Close();
				database.CloseConnection();
			}
			catch (Exception e)
			{
				database.CloseConnection();
			}
		}

		// Fursona related stuff

		/// <summary>
		///     Registers your fursona
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="name">The name.</param>
		/// <param name="age">The age.</param>
		/// <param name="species">The species.</param>
		/// <param name="gender">The gender.</param>
		/// <param name="sex">The sex.</param>
		public static void RegisterSona(IUser user, string name, int age, string species, string gender, string sex)
		{
			var database = new Database("vampdb");

			var str =
				$"INSERT INTO `sonatable` (user_id, sonaname, age, species, gender, sexuality) VALUES ('{user.Id}', '{name}', '{age}', '{species}', '{gender}', '{sex}')";
			var table = database.FireCommand(str);

			database.CloseConnection();
		}

		/// <summary>
		///     Gets info about your fursona
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		public static List<sonatable> GetSona(IUser user)
		{
			var result = new List<sonatable>();

			var database = new Database("vampdb");

			var str = string.Format("SELECT * FROM `sonatable` WHERE user_id = '{0}'", user.Id);
			var tableName = database.FireCommand(str);

			while (tableName.Read())
			{
				var userId = (string) tableName["user_id"];
				var sName = (string) tableName["sonaname"];
				var nSName = sName.Replace("_", " ");
				var age = (int) tableName["age"];
				var spec = (string) tableName["species"];
				var nSpec = spec.Replace("_", " ");
				var gend = (string) tableName["gender"];
				var sex = (string) tableName["sexuality"];
				var nSex = sex.Replace("_", " ");

				result.Add(new sonatable
				{
					UserId = userId,
					SonaName = nSName,
					Age = age,
					Species = nSpec,
					Gender = gend,
					Sexuality = nSex
				});
			}
			database.CloseConnection();

			return result;
		}

		// Polls

		/// <summary>
		///     Enters the poll.
		/// </summary>
		/// <param name="question">The question.</param>
		public static void EnterPoll(string question, IUser user)
		{
			var database = new Database("vampdb");
			var str =
				$"INSERT INTO `poll` (poll_id, question, voteyes, voteno, user_id) VALUES ('1', '{question}', '0', '0', '{user.Id}')";

			var table = database.FireCommand(str);
			database.CloseConnection();
		}

		/// <summary>
		///     Enters the user vote.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="vote">The vote.</param>
		public static void EnterUserVote(IUser user, int vote)
		{
			var database = new Database("vampdb");
			var str = string.Format("INSERT INTO `pollvotes` (user_id, vote) VALUES ('{0}', '{1}')", user.Id, vote);
			var table = database.FireCommand(str);
			database.CloseConnection();
		}

		/// <summary>
		///     If a user votes yes on the current running poll, then we are adding 1 vote for yes by using this method.
		/// </summary>
		public static void AddYesToPoll()
		{
			var database = new Database("vampdb");

			try
			{
				var amountToAdd = 1;
				var strings = $"UPDATE `poll` SET voteyes = voteyes + {amountToAdd} WHERE poll_id = 1";
				var reader = database.FireCommand(strings);
				reader.Close();
				database.CloseConnection();
			}
			catch (Exception e)
			{
				database.CloseConnection();
			}
		}

		/// <summary>
		///     If a user votes no on the current running poll, then we are adding 1 vote for no by using this method.
		/// </summary>
		public static void AddNoToPoll()
		{
			var database = new Database("vampdb");

			try
			{
				var amountToAdd = 1;
				var strings = $"UPDATE `poll` SET voteno = voteno + {amountToAdd} WHERE poll_id = 1";
				var reader = database.FireCommand(strings);
				reader.Close();
				database.CloseConnection();
			}
			catch (Exception e)
			{
				database.CloseConnection();
			}
		}

		/// <summary>
		///     Deletes the poll.
		/// </summary>
		public static void DeletePoll()
		{
			var database = new Database("vampdb");
			var str = "DELETE FROM `poll` WHERE poll_id = 1";
			var table = database.FireCommand(str);
			database.CloseConnection();
		}

		/// <summary>
		///     Deletes the users in the current vote pool.
		/// </summary>
		public static void DeleteUserInVotePool()
		{
			var database = new Database("vampdb");
			var str = "DELETE FROM `pollvotes`";
			var table = database.FireCommand(str);
			database.CloseConnection();
		}
	}
}