namespace Prima.UOData.Events.World;

public record WorldLoadedEvent(TimeSpan ElapsedTime, int EntityCount);
