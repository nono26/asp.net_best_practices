namespace SampleApp.BackEnd.Models;
public record TurnOnLightsRPC(int RoomId);

public record TurnOnLightsREST(int RoomId, int Status);