namespace anqtnbot.database;

public interface IProvider
{
	public Task<bool> IsRegistered(long id);
	public Task<string> GetLanguage(long id);

	public Task<bool> InExpectation(long sender);
	public Task<long> GetRecipient(long sender);
	public Task<bool> IsResponse(long sender);

	public Task<bool> IsBanned(long sender, long recipient);
}
