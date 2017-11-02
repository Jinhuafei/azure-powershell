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

using System;
using System.Collections;
using System.Management.Automation;
using Microsoft.Azure.Commands.ResourceManager.Common.Tags;
using Microsoft.Azure.Management.ContainerRegistry.Models;

namespace Microsoft.Azure.Commands.ContainerRegistry
{
    [Cmdlet(VerbsCommon.New, ContainerRegistryWebhookNoun,
        DefaultParameterSetName = NameResourceGroupParameterSet,
        SupportsShouldProcess = true), OutputType(typeof(PSContainerRegistryWebhook))]
    public class NewAzureContainerRegistryWebhook : ContainerRegistryCmdletBase
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            HelpMessage = "Webhook Name.")]
        [ValidateNotNullOrEmpty]
        [Alias(WebhookNameAlias)]
        public string Name { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = NameResourceGroupParameterSet,
            HelpMessage = "Resource Group Name.")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(
            Position = 2,
            Mandatory = true,
            ParameterSetName = NameResourceGroupParameterSet,
            HelpMessage = "Container Registry Name.")]
        [Alias(ContainerRegistryNameAlias, ResourceNameAlias)]
        [ValidateNotNullOrEmpty]
        public string RegistryName { get; set; }

        [Parameter(
            Position = 3,
            Mandatory = true,
            HelpMessage = "The service URI for the webhook to post notifications.")]
        [Alias(WebhookUriAlias)]
        public Uri Uri { get; set; }

        [Parameter(
            Position = 4,
            Mandatory = true,
            HelpMessage = "Space separated list of actions that trigger the webhook to post notifications.")]
        [Alias(WebhookActionsAlias)]
        [ValidateSet(WebhookAction.Delete, WebhookAction.Push)]
        public string[] Actions { get; set; }

        [Parameter(
            Mandatory = true,
            ParameterSetName = RegistryObjectParameterSet,
            ValueFromPipeline = true,
            HelpMessage = "Container Registry Object.")]
        [ValidateNotNullOrEmpty]
        public PSContainerRegistry Registry { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Custom headers that will be added to the webhook notifications.")]
        [ValidateNotNull]
        [Alias(WebhookHeadersAlias)]
        public Hashtable Headers { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Webhook tags.")]
        [ValidateNotNull]
        [Alias(WebhookTagsAlias)]
        public Hashtable Tag { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Webhook is disabled")]
        [Alias(WebhookDisabledAlias)]
        public SwitchParameter Disabled { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Webhook scope.")]
        [ValidateNotNull]
        [Alias(WebhookScopeAlias)]
        public string Scope { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Webhook Location. Default to the location of the resource group.")]
        [ValidateNotNullOrEmpty]
        [Alias(WebhookLocationAlias)]
        public string Location { get; set; }

        public override void ExecuteCmdlet()
        {
            if (ShouldProcess(Name, "Create a webhook for the container registry"))
            {
                if (string.Equals(ParameterSetName, RegistryObjectParameterSet))
                {
                    ResourceGroupName = Registry.ResourceGroupName;
                    RegistryName = Registry.Name;
                }

                var tags = TagsConversionHelper.CreateTagDictionary(Tag, validate: true);
                var headers = ConversionUtilities.ToDictionary(Headers);

                var parameters = new WebhookCreateParameters()
                {
                    Actions = Actions,
                    CustomHeaders = headers,
                    ServiceUri = Uri?.ToString(),
                    Tags = tags,
                    Status = ConversionUtilities.ToWebhookStatus(Disabled),
                    Scope = Scope,
                    Location = Location ?? ResourceManagerClient.GetResourceGroupLocation(ResourceGroupName)
                };

                var webhook = RegistryClient.CreateWebhook(ResourceGroupName, RegistryName, Name, parameters);
                WriteObject(new PSContainerRegistryWebhook(webhook));
            }
        }
    }
}
