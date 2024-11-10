using MySql.Data.MySqlClient;

namespace anqtnbot.database.mysql;

public class MySqlProvider(IDatabase database) : IProvider
{
	private readonly IQueries _queries = database.Queries();
	private readonly MySqlConnection _connection = database.Connection();

	public async Task<bool> IsRegistered(long id)
	{
		var query = new MySqlCommand(_queries.IsRegistered, _connection);
		query.Parameters.AddWithValue("@id", id);

		await using var reader = await query.ExecuteReaderAsync();

		return reader.HasRows;
	}

	public async Task<string> GetLanguage(long id)
	{
		var query = new MySqlCommand(_queries.GetLanguage, _connection);

		query.Parameters.AddWithValue("@id", id);

		await using var reader = await query.ExecuteReaderAsync();
		await reader.ReadAsync();

		return reader.GetString(0);
	}

	public async Task<bool> InExpectation(long sender)
	{
		var query = new MySqlCommand(_queries.InExpectation, _connection);
		query.Parameters.AddWithValue("@sender", sender);

		await using var reader = await query.ExecuteReaderAsync();

		return reader.HasRows;
	}

	public async Task<long> GetRecipient(long sender)
	{
		var query = new MySqlCommand(_queries.GetRecipient, _connection);
		query.Parameters.AddWithValue("@sender", sender);

		await using var reader = await query.ExecuteReaderAsync();
		await reader.ReadAsync();

		return reader.GetInt64(0);
	}

	public async Task<bool> IsResponse(long sender)
	{
		var query = new MySqlCommand(_queries.IsResponse, _connection);
		query.Parameters.AddWithValue("@sender", sender);

		return await query.ExecuteScalarAsync() is ulong and 1;
	}

	public async Task<bool> IsBanned(long sender, long recipient)
	{
		var query = new MySqlCommand(_queries.IsBanned, _connection);
		var parameters = query.Parameters;

		parameters.AddWithValue("@sender", sender);
		parameters.AddWithValue("@recipient", recipient);

		await using var reader = await query.ExecuteReaderAsync();

		return reader.HasRows;
	}
}
