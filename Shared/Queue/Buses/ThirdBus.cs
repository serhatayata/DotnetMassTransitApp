using MassTransit;

namespace Shared.Queue.Buses;

public class ThirdBus :
    BusInstance<IThirdBus>, IThirdBus
{
    public ThirdBus(IBusControl busControl)
        : base(busControl)
    {
            
    }
}
