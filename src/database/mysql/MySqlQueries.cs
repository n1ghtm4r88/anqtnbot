namespace anqtnbot.database.mysql;

public class MySqlQueries : IQueries
{
	public string Init =>
		"""
		    CREATE DATABASE IF NOT EXISTS anqtnbot;
		    USE anqtnbot
		""";

	public string InitUsers =>
		"""
		    CREATE TABLE IF NOT EXISTS users (
		        
		        id BIGINT PRIMARY KEY,
		        language VARCHAR(4) NOT NULL
		    )
		""";

	public string AddUser =>
		"""
		    REPLACE INTO users
		        (id, language)
		        
		    VALUES
		        (@id, @language)
		""";

	public string IsRegistered => "SELECT 1 FROM users WHERE id = @id";

	public string GetLanguage => "SELECT language FROM users WHERE id = @id";

	public string InitExpectations =>
		"""
		    CREATE TABLE IF NOT EXISTS expectations (
		        
		        sender BIGINT PRIMARY KEY,
		        recipient BIGINT NOT NULL,
		        response BIT NOT NULL
		    )
		""";

	public string AddExpectation =>
		"""
		    INSERT INTO expectations
		        (sender, recipient, response)
		    
		    VALUES
		        (@sender, @recipient, @response)
		""";

	public string RemoveExpectation => "DELETE FROM expectations WHERE sender = @sender";

	public string InExpectation => "SELECT 1 FROM expectations WHERE sender = @sender";

	public string GetRecipient => "SELECT recipient FROM expectations WHERE sender = @sender";

	public string IsResponse => "SELECT response FROM expectations WHERE sender = @sender";

	public string InitBanList =>
		"""
		    CREATE TABLE IF NOT EXISTS banlist (
		        
		        sender BIGINT NOT NULL,
		        recipient BIGINT NOT NULL
		    )
		""";

	public string AddBan =>
		"""
		    INSERT INTO banlist
		        (sender, recipient)
		    
		    VALUES
		        (@sender, @recipient)
		""";

	public string RemoveBan => "DELETE FROM banlist WHERE sender = @sender AND recipient = @recipient";

	public string IsBanned => "SELECT 1 FROM banlist WHERE sender = @sender AND recipient = @recipient";
}
