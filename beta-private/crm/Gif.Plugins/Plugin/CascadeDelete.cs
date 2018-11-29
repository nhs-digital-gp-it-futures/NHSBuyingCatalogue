namespace Gif.Plugins.Plugin
{
    #region

    using Business_Logic;
    using Microsoft.Xrm.Sdk;
    using Repositories;
    using System;

    #endregion

    /// <summary>
    ///     cascade delete all associated child records when a Solution record is deleted.
    /// </summary>
    public class CascadeDelete : IPlugin
    {
        #region IPlugin

        /// <summary>
        /// plugin execution
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void Execute(IServiceProvider serviceProvider)
        {
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var service = serviceFactory.CreateOrganizationService(context.UserId);
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            tracingService?.Trace($"{PluginName} started.");

            if (context.PrimaryEntityName != EntityName)
                throw new InvalidPluginExecutionException($"This plugin runs only on a {EntityName} entity.");

            var targetEntity = (context.PreEntityImages != null) && context.PreEntityImages.Contains("PreImage") ? context.PreEntityImages["PreImage"] : null;
            if (targetEntity == null)
                return;

            tracingService?.Trace($"Target id : {targetEntity.Id}");

            var repository = new SolutionRepository(service);
            var logic = new CascadeDeleteLogic(repository, PluginName);

            try
            {
                if (context.MessageName.Equals("Delete"))
                {
                    logic.OnSolutionDelete(targetEntity);
                }
            }
            catch (Exception exception)
            {
                var traceLogException = logic.GetTraceInformation();
                throw new InvalidPluginExecutionException($"An error occurred in the {PluginName} plugin: {exception.Message} Trace: {traceLogException}", exception);
            }

            var traceLog = logic.GetTraceInformation();
            tracingService?.Trace($"Successful trace log: {traceLog}");
            tracingService?.Trace($"{PluginName} finished.");
        }

        #endregion

        #region Fields

        /// <summary>
        ///     The name of the entity the plugin is designed to run on
        /// </summary>
        private const string EntityName = "cc_solution";

        /// <summary>
        ///     The plugin full name.
        /// </summary>
        private const string PluginName = "Gif.Plugins.Plugin.CascadeDelete";

        #endregion
    }
}
