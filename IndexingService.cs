using System;
using System.Configuration;
using System.Reflection;

namespace Website.Areas.Admin.Services.Indexing
{
	public interface IIndexingService
	{
		/// <summary>
		/// Indexes a document using an external BLL.
		/// </summary>
		/// <param name="documentId">The document identifier.</param>
		/// <returns>The workflow identifier</returns>
		string IndexUsingExternalBLL(Guid documentId);
	}

	public class IndexingService : IIndexingService
	{
		/// <summary>
		/// Indexes a document using an external BLL.
		/// </summary>
		/// <param name="documentId">The document identifier.</param>
		/// <returns>The workflow identifier</returns>
		public string IndexUsingExternalBLL(Guid documentId)
		{
			Assembly externalBLLAssembly = Assembly.LoadFrom(ConfigurationManager.AppSettings["ExternalBLLPath"]);

			var bootStrapperType = externalBLLAssembly.GetType("BLL.BootStrapper");
			bootStrapperType.GetMethod("Initialise", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);

			var workflowFactoryType = externalBLLAssembly.GetType("BLL.Workflows.WorkflowFactory");
			var workflowFactoryConstructor = workflowFactoryType.GetConstructors()[0];
			var workflowFactoryInstance = workflowFactoryConstructor.Invoke(new object[0]);

			var startIndexWorkflowMethod = workflowFactoryType.GetMethod("StartIndexWorkflow");
			object[] startIndexWorkflowParameters =
			{
				documentId,
				"Elasticsearch"		// dataIndexingEngine
			};

			return (string)startIndexWorkflowMethod.Invoke(workflowFactoryInstance, startIndexWorkflowParameters);
		}
	}
}