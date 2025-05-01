namespace Prima.Core.Server.Events.Account;

public record AccountCreatedEvent(string Id, string Username, bool IsAdmin);
