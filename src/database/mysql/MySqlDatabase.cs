using anqtnbot.config;
using MySql.Data.MySqlClient;

namespace anqtnbot.database.mysql;

public class MySqlDatabase(IQueries queries, IConfig config) : IDatabase, IAsyncDisposable
{
	private readonly MySqlConnection _connection = new(config.ConnectionString);

	public async Task InitAsync()
	{
		await _connection.OpenAsync();

		MySqlCommand[] commands =
		[
			new MySqlCommand(queries.Init, _connection),
			new MySqlCommand(queries.InitUsers, _connection),
			new MySqlCommand(queries.InitExpectations, _connection),
			new MySqlCommand(queries.InitBanList, _connection)
		];

		foreach (var command in commands)
		{
			await command.ExecuteNonQueryAsync();
		}
	}

	public MySqlConnection Connection() => _connection;

	public IQueries Queries() => queries;

	public async ValueTask DisposeAsync()
	{
		await _connection.CloseAsync();
		await _connection.DisposeAsync();

		GC.SuppressFinalize(this);
	}

	public async Task AddUser(long id, string language)
	{
		var query = new MySqlCommand(queries.AddUser, _connection);
		var parameters = query.Parameters;

		parameters.AddWithValue("@id", id);
		parameters.AddWithValue("@language", language);

		await query.ExecuteNonQueryAsync();
	}

	public async Task AddExpectation(long sender, long recipient, bool response)
	{
		var query = new MySqlCommand(queries.AddExpectation, _connection);
		var parameters = query.Parameters;

		parameters.AddWithValue("@sender", sender);
		parameters.AddWithValue("@recipient", recipient);
		parameters.AddWithValue("@response", response);

		await query.ExecuteNonQueryAsync();
	}

	public async Task RemoveExpectation(long sender)
	{
		var query = new MySqlCommand(queries.RemoveExpectation, _connection);

		query.Parameters.AddWithValue("@sender", sender);

		await query.ExecuteNonQueryAsync();
	}

	public async Task AddBan(long sender, long recipient)
	{
		var query = new MySqlCommand(queries.AddBan, _connection);
		var parameters = query.Parameters;

		parameters.AddWithValue("@sender", sender);
		parameters.AddWithValue("@recipient", recipient);

		await query.ExecuteNonQueryAsync();
	}

	public async Task RemoveBan(long sender, long recipient)
	{
		var query = new MySqlCommand(queries.RemoveBan, _connection);
		var parameters = query.Parameters;

		parameters.AddWithValue("@sender", sender);
		parameters.AddWithValue("@recipient", recipient);

		await query.ExecuteNonQueryAsync();
	}
}
