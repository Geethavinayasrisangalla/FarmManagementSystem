namespace FarmManagement.Web.Events;




public record CropCreatedEvent(
    int UserId, string UserName, string Role,
    string CropName, string CropType, string Season) : IDomainEvent;

public record CropUpdatedEvent(
    int UserId, string UserName, string Role,
    string CropName, string Status) : IDomainEvent;

public record CropDeletedEvent(
    int UserId, string UserName, string Role,
    string CropName) : IDomainEvent;


public record FieldCreatedEvent(
    int UserId, string UserName, string Role,
    string FieldName, decimal Area, string SoilType) : IDomainEvent;

public record FieldUpdatedEvent(
    int UserId, string UserName, string Role,
    string FieldName, decimal Area, string SoilType) : IDomainEvent;

public record FieldDeletedEvent(
    int UserId, string UserName, string Role,
    string FieldName) : IDomainEvent;


public record ResourceCreatedEvent(
    int UserId, string UserName, string Role,
    string Name, decimal Quantity, string Unit) : IDomainEvent;

public record ResourceUpdatedEvent(
    int UserId, string UserName, string Role,
    string Name, decimal Quantity, string Unit) : IDomainEvent;

public record ResourceAllocatedEvent(
    int UserId, string UserName, string Role,
    string Name, decimal Quantity, string Unit, int ScheduleId) : IDomainEvent;

public record ResourceDeletedEvent(
    int UserId, string UserName, string Role,
    string Name) : IDomainEvent;


public record PestReportedEvent(
    int UserId, string UserName, string Role,
    string PestName) : IDomainEvent;

public record PestStatusUpdatedEvent(
    int UserId, string UserName, string Role,
    string PestName, string NewStatus) : IDomainEvent;

public record PestDeletedEvent(
    int UserId, string UserName, string Role,
    string PestName) : IDomainEvent;


public record ScheduleCreatedEvent(
    int UserId, string UserName, string Role,
    DateTime ScheduledDate, decimal ExpectedYield) : IDomainEvent;

public record HarvestRecordedEvent(
    int UserId, string UserName, string Role,
    int ScheduleId, decimal ActualYield) : IDomainEvent;

public record ScheduleDeletedEvent(
    int UserId, string UserName, string Role,
    int ScheduleId) : IDomainEvent;
