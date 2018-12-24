namespace ValidationWeb.Utility
{
    using System;

    using SimpleInjector;
    using SimpleInjector.Diagnostics;

    public static class SimpleInjectorExtension
    {
        public static void RegisterDisposableTransient<TService, TImplementation>(
            this Container c)
            where TImplementation : class, IDisposable, TService
            where TService : class
        {
            var scoped = Lifestyle.Scoped;
            var r = Lifestyle.Transient.CreateRegistration<TImplementation>(c);

            r.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "ignore");
            c.AddRegistration(typeof(TService), r);
            c.RegisterInitializer<TImplementation>(o => scoped.RegisterForDisposal(c, o));
        }
    }
}