namespace Prima.UOData.Events.World;

public record WorldSavedEvent(TimeSpan ElapsedTime, int EntityCount);
