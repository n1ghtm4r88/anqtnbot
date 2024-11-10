namespace anqtnbot.database;

public interface IQueries
{
	public string Init { get; }

	public string InitUsers { get; }
	public string AddUser { get; }
	public string IsRegistered { get; }
	public string GetLanguage { get; }

	public string InitExpectations { get; }
	public string AddExpectation { get; }
	public string RemoveExpectation { get; }
	public string InExpectation { get; }
	public string GetRecipient { get; }
	public string IsResponse { get; }

	public string InitBanList { get; }
	public string AddBan { get; }
	public string RemoveBan { get; }
	public string IsBanned { get; }
}
