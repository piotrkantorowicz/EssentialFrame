using EssentialFrame.Core.Extensions;
using EssentialFrame.Domain.Core;
using EssentialFrame.Serialization;

namespace EssentialFrame.Domain.Events;

public static class EventExtensions
{
    public static IEvent Deserialize(this SerializedEvent x, ISerializer serializer)
    {
        var data = serializer.Deserialize<IEvent>(x.EventData, Type.GetType(x.EventClass));

        data.AggregateIdentifier = x.AggregateIdentifier;
        data.AggregateVersion = x.AggregateVersion;
        data.EventTime = x.EventTime;
        data.IdentityTenant = x.IdentityTenant;
        data.IdentityUser = x.IdentityUser;

        return data;
    }

    public static SerializedEvent Serialize(this IEvent @event,
                                            ISerializer serializer,
                                            Guid aggregateIdentifier,
                                            int version,
                                            Guid tenant,
                                            Guid user)
    {
        var data = serializer.Serialize(@event,
                                        new[]
                                        {
                                            "AggregateIdentifier",
                                            "AggregateVersion",
                                            "EventTime",
                                            "IdentityTenant",
                                            "IdentityUser"
                                        });

        var serialized = new SerializedEvent
                         {
                             AggregateIdentifier = aggregateIdentifier,
                             AggregateVersion = version,
                             EventTime = @event.EventTime,
                             EventClass = @event.GetClassName(),
                             EventType = @event.GetTypeFullName(),
                             EventData = data,
                             IdentityTenant = Guid.Empty == @event.IdentityTenant ? tenant : @event.IdentityTenant,
                             IdentityUser = Guid.Empty == @event.IdentityUser ? user : @event.IdentityUser
                         };

        @event.IdentityTenant = serialized.IdentityTenant;
        @event.IdentityUser = serialized.IdentityUser;

        return serialized;
    }
}
