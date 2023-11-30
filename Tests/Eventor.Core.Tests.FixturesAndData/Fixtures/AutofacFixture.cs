using Autofac;
using Eventor.Core.Common.Seeds;
using Eventor.Core.Tests.FixturesAndData.Events;

namespace Eventor.Core.Tests.FixturesAndData.Fixtures;

public class AutofacFixture
{
    public EventAggregator EventAggregator { get; }
    public AutofacFixture()
    {
        EventAggregator = ConfigureAutofac().Resolve<EventAggregator>();
    }

    private IContainer ConfigureAutofac()
    {
        var builder = new ContainerBuilder();
        builder.Register<EventAggregator>(c =>
        {
            var context = c.Resolve<IComponentContext>();
            return new EventAggregator(type => context.Resolve(type));
        }).SingleInstance();

        return builder.Build();
    }

}
