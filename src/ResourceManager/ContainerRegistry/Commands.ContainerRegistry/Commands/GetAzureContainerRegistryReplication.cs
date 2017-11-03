// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.ContainerRegistry
{
    [Cmdlet(VerbsCommon.Get, ContainerRegistryReplicationNoun, DefaultParameterSetName = NameResourceGroupParameterSet, SupportsShouldProcess = true)]
    [OutputType(typeof(PSContainerRegistryReplication), typeof(IList<PSContainerRegistryReplication>))]
    public class GetAzureContainerRegistryReplication : ContainerRegistryCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = ShowReplicationByNameResourceGroupParameterSet, HelpMessage = "Container Registry Replication Name.")]
        [Parameter(ParameterSetName = ShowReplicationByRegistryObjectParameterSet)]
        [ValidateNotNullOrEmpty]
        [Alias(ReplicationNameAlias)]
        public string Name { get; set; }

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = ShowReplicationByNameResourceGroupParameterSet, HelpMessage = "Resource Group Name.")]
        [Parameter(ParameterSetName = ListReplicationByNameResourceGroupParameterSet)]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Position = 2, Mandatory = true, ParameterSetName = ShowReplicationByNameResourceGroupParameterSet, HelpMessage = "Container Registry Name.")]
        [Parameter(ParameterSetName = ListReplicationByNameResourceGroupParameterSet)]
        [Alias(ContainerRegistryNameAlias, ResourceNameAlias)]
        [ValidateNotNullOrEmpty]
        public string RegistryName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ShowReplicationByRegistryObjectParameterSet, ValueFromPipeline = true, HelpMessage = "Container Registry Object.")]
        [Parameter(ParameterSetName = ListReplicationByRegistryObjectParameterSet)]
        [ValidateNotNullOrEmpty]
        public PSContainerRegistry Registry { get; set; }
        
        public override void ExecuteCmdlet()
        {
            if(string.Equals(ParameterSetName, ShowReplicationByRegistryObjectParameterSet) || 
                string.Equals(ParameterSetName, ListReplicationByRegistryObjectParameterSet))
            {
                ResourceGroupName = Registry.ResourceGroupName;
                RegistryName = Registry.Name;
            }

            switch (ParameterSetName)
            {
                case ShowReplicationByRegistryObjectParameterSet:
                case ShowReplicationByNameResourceGroupParameterSet:
                    ShowReplication();
                    break;
                case ListReplicationByNameResourceGroupParameterSet:
                case ListReplicationByRegistryObjectParameterSet:
                    ListReplication();
                    break;
            }
        }

        private void ListReplication()
        {
            var replications = RegistryClient.ListReplications(ResourceGroupName, RegistryName);
            var replicationList = new List<PSContainerRegistryReplication>();
            foreach(var r in replications)
            {
                replicationList.Add(new PSContainerRegistryReplication(r));
            }

            while (!string.IsNullOrEmpty(replications.NextPageLink))
            {
                replications = RegistryClient.ListReplicationsUsingNextLink(replications.NextPageLink);
                foreach (var r in replications)
                {
                    replicationList.Add(new PSContainerRegistryReplication(r));
                }
            }

            WriteObject(replicationList, true);
        }

        private void ShowReplication()
        {
            var replication = RegistryClient.GetReplication(ResourceGroupName, RegistryName, Name);
            WriteObject(new PSContainerRegistryReplication(replication));
        }
    }
}
