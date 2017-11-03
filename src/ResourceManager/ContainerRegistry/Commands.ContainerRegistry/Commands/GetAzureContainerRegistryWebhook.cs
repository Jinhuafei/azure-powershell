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
    [Cmdlet(VerbsCommon.Get, ContainerRegistryWebhookNoun, DefaultParameterSetName = ListWebhookByNameResourceGroupParameterSet)]
    [OutputType(typeof(PSContainerRegistryWebhook), typeof(PSContainerRegistryWebhookConfig), typeof(PSContainerRegistryWebhookEvent))]
    [OutputType(typeof(IList<PSContainerRegistryWebhook>), typeof(IList<PSContainerRegistryWebhookEvent>))]
    public class GetAzureContainerRegistryWebhook : ContainerRegistryCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = ShowWebhookByNameResourceGroupParameterSet, HelpMessage = "Webhook Name.")]
        [Parameter(Mandatory = true, ParameterSetName = ShowWebhookByRegistryObjectParameterSet)]
        [Parameter(Mandatory = true, ParameterSetName = GetWebhookConfigByNameResourceGroupParameterSet)]
        [Parameter(Mandatory = true, ParameterSetName = GetWebhookConfigByRegistryObjectParameterSet)]
        [Parameter(Mandatory = true, ParameterSetName = ListWebhookEventsByNameResourceGroupParameterSet)]
        [Parameter(Mandatory = true, ParameterSetName = ListWebhookEventsByRegistryObjectParameterSet)]
        [ValidateNotNullOrEmpty]
        [Alias(WebhookNameAlias)]
        public string Name { get; set; }

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = ListWebhookByNameResourceGroupParameterSet, HelpMessage = "Resource Group Name.")]
        [Parameter(Mandatory = true, ParameterSetName = ShowWebhookByNameResourceGroupParameterSet)]
        [Parameter(Mandatory = true, ParameterSetName = GetWebhookConfigByNameResourceGroupParameterSet)]
        [Parameter(Mandatory = true, ParameterSetName = ListWebhookEventsByNameResourceGroupParameterSet)]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Position = 2, Mandatory = true, ParameterSetName = ListWebhookByNameResourceGroupParameterSet, HelpMessage = "Container Registry Name.")]
        [Parameter(Mandatory = true, ParameterSetName = ShowWebhookByNameResourceGroupParameterSet)]
        [Parameter(Mandatory = true, ParameterSetName = GetWebhookConfigByNameResourceGroupParameterSet)]
        [Parameter(Mandatory = true, ParameterSetName = ListWebhookEventsByNameResourceGroupParameterSet)]
        [Alias(ContainerRegistryNameAlias, ResourceNameAlias)]
        [ValidateNotNullOrEmpty]
        public string RegistryName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ListWebhookByRegistryObjectParameterSet, ValueFromPipeline = true, HelpMessage = "Container Registry Object.")]
        [Parameter(Mandatory = true, ParameterSetName = ShowWebhookByRegistryObjectParameterSet)]
        [Parameter(Mandatory = true, ParameterSetName = GetWebhookConfigByRegistryObjectParameterSet)]
        [Parameter(Mandatory = true, ParameterSetName = ListWebhookEventsByRegistryObjectParameterSet)]
        [ValidateNotNullOrEmpty]
        public PSContainerRegistry Registry { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ListWebhookByNameResourceGroupParameterSet, HelpMessage = "List all of the webhooks for a container registry.")]
        [Parameter(Mandatory = true, ParameterSetName = ListWebhookByRegistryObjectParameterSet)]
        public SwitchParameter List { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = GetWebhookConfigByNameResourceGroupParameterSet, HelpMessage = "Get the configuration information for a webhook.")]
        [Parameter(Mandatory = true, ParameterSetName = GetWebhookConfigByRegistryObjectParameterSet)]
        public SwitchParameter GetConfig { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ListWebhookEventsByNameResourceGroupParameterSet, HelpMessage = "List recent events for a webhook.")]
        [Parameter(Mandatory = true, ParameterSetName = ListWebhookEventsByRegistryObjectParameterSet)]
        public SwitchParameter ListEvents { get; set; }

        public override void ExecuteCmdlet()
        {
            if(string.Equals(ParameterSetName, ListWebhookByRegistryObjectParameterSet) ||
                string.Equals(ParameterSetName, ShowWebhookByRegistryObjectParameterSet) ||
                string.Equals(ParameterSetName, GetWebhookConfigByRegistryObjectParameterSet) ||
                string.Equals(ParameterSetName, ListWebhookEventsByRegistryObjectParameterSet))
            {
                ResourceGroupName = Registry.ResourceGroupName;
                RegistryName = Registry.Name;
            }

            switch (ParameterSetName)
            {
                case ShowWebhookByNameResourceGroupParameterSet:
                case ShowWebhookByRegistryObjectParameterSet:
                    ShowWebhook();
                    break;
                case GetWebhookConfigByNameResourceGroupParameterSet:
                case GetWebhookConfigByRegistryObjectParameterSet:
                    GetWebhookConfig();
                    break;
                case ListWebhookEventsByNameResourceGroupParameterSet:
                case ListWebhookEventsByRegistryObjectParameterSet:
                    ListWebhookEvents();
                    break;
                case ListWebhookByNameResourceGroupParameterSet:
                case ListWebhookByRegistryObjectParameterSet:
                    ListWebhook();
                    break;
            }
        }

        private void ShowWebhook()
        {
            var webhook = RegistryClient.GetWebhook(ResourceGroupName, RegistryName, Name);
            WriteObject(new PSContainerRegistryWebhook(webhook));
        }

        private void GetWebhookConfig()
        {
            var config = RegistryClient.GetWebhookGetCallbackConfig(ResourceGroupName, RegistryName, Name);
            WriteObject(new PSContainerRegistryWebhookConfig(config));
        }

        private void ListWebhookEvents()
        {
            var webhookEvents = RegistryClient.ListWebhookEvents(ResourceGroupName, RegistryName, Name);
            var webhookEventList = new List<PSContainerRegistryWebhookEvent>();

            foreach(var webhookEvent in webhookEvents)
            {
                webhookEventList.Add(new PSContainerRegistryWebhookEvent(webhookEvent));
            }

            while(!string.IsNullOrEmpty(webhookEvents.NextPageLink))
            {
                webhookEvents = RegistryClient.ListWebhookEventsUsingNextLink(webhookEvents.NextPageLink);
                foreach (var webhookEvent in webhookEvents)
                {
                    webhookEventList.Add(new PSContainerRegistryWebhookEvent(webhookEvent));
                }
            }

            WriteObject(webhookEventList);
        }

        private void ListWebhook()
        {
            var webhooks = RegistryClient.ListWebhooks(ResourceGroupName, RegistryName);
            var webhookList = new List<PSContainerRegistryWebhook>();

            foreach(var webhook in webhooks)
            {
                webhookList.Add(new PSContainerRegistryWebhook(webhook));
            }

            while(!string.IsNullOrEmpty(webhooks.NextPageLink))
            {
                webhooks = RegistryClient.ListWebhooksUsingNextLink(webhooks.NextPageLink);

                foreach (var webhook in webhooks)
                {
                    webhookList.Add(new PSContainerRegistryWebhook(webhook));
                }
            }

            WriteObject(webhookList);
        }
    }
}
