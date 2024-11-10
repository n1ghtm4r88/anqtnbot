using MySql.Data.MySqlClient;

namespace anqtnbot.database;

public interface IDatabase
{
	public Task InitAsync();

	public MySqlConnection Connection();
	public IQueries Queries();

	public ValueTask DisposeAsync();

	public Task AddUser(long id, string language);

	public Task AddExpectation(long sender, long recipient, bool response);
	public Task RemoveExpectation(long sender);

	public Task AddBan(long sender, long recipient);
	public Task RemoveBan(long sender, long recipient);
}
