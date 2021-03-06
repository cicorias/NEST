using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using NUnit.Framework;
using Nest.Tests.Integration.Yaml;


namespace Nest.Tests.Integration.Yaml.ClusterPendingTasks1
{
	public partial class ClusterPendingTasks1YamlTests
	{	


		[NCrunch.Framework.ExclusivelyUses("ElasticsearchYamlTests")]
		public class TestPendingTasks1Tests : YamlTestsBase
		{
			[Test]
			public void TestPendingTasks1Test()
			{	

				//do cluster.pending_tasks 
				this.Do(()=> this._client.ClusterPendingTasksGet());

				//is_true _response.tasks; 
				this.IsTrue(_response.tasks);

			}
		}

		[NCrunch.Framework.ExclusivelyUses("ElasticsearchYamlTests")]
		public class TestPendingTasksWithLocalFlag2Tests : YamlTestsBase
		{
			[Test]
			public void TestPendingTasksWithLocalFlag2Test()
			{	

				//do cluster.pending_tasks 
				this.Do(()=> this._client.ClusterPendingTasksGet(nv=>nv
					.Add("local", @"true")
				));

				//is_true _response.tasks; 
				this.IsTrue(_response.tasks);

			}
		}
	}
}

