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

using System.Management.Automation;

namespace Microsoft.Azure.Commands.ContainerRegistry
{
    [Cmdlet(VerbsCommon.Get, ContainerRegistryWebhookNoun, DefaultParameterSetName = NameResourceGroupParameterSet)]
    [OutputType(typeof(PSContainerRegistryWebhook), typeof(PSContainerRegistryWebhookConfig), typeof(PSContainerRegistryWebhookEvents))]
    public class GetAzureContainerRegistryWebhook : ContainerRegistryCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = ShowWebhookByNameResourceGroupParameterSet, HelpMessage = "Webhook Name.")]
        [Parameter(ParameterSetName = ShowWebhookByRegistryObjectParameterSet)]
        [Parameter(ParameterSetName = GetWebhookConfigByNameResourceGroupParameterSet)]
        [Parameter(ParameterSetName = GetWebhookConfigByRegistryObjectParameterSet)]
        [Parameter(ParameterSetName = ListWebhookEventsByNameResourceGroupParameterSet)]
        [Parameter(ParameterSetName = ListWebhookEventsByRegistryObjectParameterSet)]
        [ValidateNotNullOrEmpty]
        [Alias(WebhookNameAlias)]
        public string Name { get; set; }

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = ListWebhookByNameResourceGroupParameterSet, HelpMessage = "Resource Group Name.")]
        [Parameter(ParameterSetName = ShowWebhookByNameResourceGroupParameterSet)]
        [Parameter(ParameterSetName = GetWebhookConfigByNameResourceGroupParameterSet)]
        [Parameter(ParameterSetName = ListWebhookEventsByNameResourceGroupParameterSet)]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Position = 2, Mandatory = true, ParameterSetName = ListWebhookByNameResourceGroupParameterSet, HelpMessage = "Container Registry Name.")]
        [Parameter(ParameterSetName = ShowWebhookByNameResourceGroupParameterSet)]
        [Parameter(ParameterSetName = GetWebhookConfigByNameResourceGroupParameterSet)]
        [Parameter(ParameterSetName = ListWebhookEventsByNameResourceGroupParameterSet)]
        [Alias(ContainerRegistryNameAlias, RegistryNameAlias, ResourceNameAlias)]
        [ValidateNotNullOrEmpty]
        public string RegistryName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ListWebhookByRegistryObjectParameterSet, ValueFromPipeline = true, HelpMessage = "Container Registry Object.")]
        [Parameter(ParameterSetName = ShowWebhookByRegistryObjectParameterSet)]
        [Parameter(ParameterSetName = GetWebhookConfigByRegistryObjectParameterSet)]
        [Parameter(ParameterSetName = ListWebhookEventsByRegistryObjectParameterSet)]
        [ValidateNotNullOrEmpty]
        public PSContainerRegistry Registry { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = GetWebhookConfigByNameResourceGroupParameterSet, HelpMessage = "Get the configuration information for a webhook.")]
        [Parameter(Mandatory = true, ParameterSetName = GetWebhookConfigByRegistryObjectParameterSet)]
        public SwitchParameter GetConfig { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ListWebhookEventsByNameResourceGroupParameterSet, HelpMessage = "List recent events for a webhook.")]
        [Parameter(Mandatory = true, ParameterSetName = ListWebhookEventsByRegistryObjectParameterSet)]
        public SwitchParameter ListEvents { get; set; }

        public override void ExecuteCmdlet()
        {
        }
    }
}
