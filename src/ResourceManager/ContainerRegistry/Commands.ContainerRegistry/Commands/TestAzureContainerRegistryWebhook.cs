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

namespace Microsoft.Azure.Commands.ContainerRegistry
{
    [Cmdlet(VerbsDiagnostic.Test, ContainerRegistryWebhookNoun, DefaultParameterSetName = NameResourceGroupParameterSet)]
    public class TestAzureContainerRegistryWebhook : ContainerRegistryCmdletBase
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
        [Alias(ContainerRegistryNameAlias, RegistryNameAlias, ResourceNameAlias)]
        [ValidateNotNullOrEmpty]
        public string RegistryName { get; set; }

        [Parameter(
            Mandatory = true,
            ParameterSetName = RegistryObjectParameterSet,
            ValueFromPipeline = true,
            HelpMessage = "Container Registry Object.")]
        [ValidateNotNullOrEmpty]
        public PSContainerRegistry Registry { get; set; }

        public override void ExecuteCmdlet()
        {
            
        }
    }
}
